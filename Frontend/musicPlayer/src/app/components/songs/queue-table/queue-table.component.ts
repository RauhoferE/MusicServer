import { DOCUMENT } from '@angular/common';
import { Component, EventEmitter, Inject, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { NzContextMenuService, NzDropdownMenuComponent } from 'ng-zorro-antd/dropdown';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzModalService } from 'ng-zorro-antd/modal';
import { Observable, Subject, takeUntil } from 'rxjs';
import { APIROUTES } from 'src/app/constants/api-routes';
import { ArtistShortModel } from 'src/app/models/artist-models';
import { DragDropQueueParams, DragDropSongParams, PlaylistSongModelParams } from 'src/app/models/events';
import { GuidNameModel, PlaylistSongModel, SongPaginationModel } from 'src/app/models/playlist-models';
import { PaginationModel } from 'src/app/models/storage';
import { PlaylistService } from 'src/app/services/playlist.service';
import { QueueService } from 'src/app/services/queue.service';
import { RxjsStorageService } from 'src/app/services/rxjs-storage.service';
import { SongService } from 'src/app/services/song.service';
import { environment } from 'src/environments/environment';
import { CdkDragDrop, CdkDragStart, moveItemInArray } from '@angular/cdk/drag-drop';
import { StreamingClientService } from 'src/app/services/streaming-client.service';
import { QueueWrapperService } from 'src/app/services/queue-wrapper.service';

@Component({
  selector: 'app-queue-table',
  templateUrl: './queue-table.component.html',
  styleUrls: ['./queue-table.component.scss']
})
export class QueueTableComponent implements OnInit, OnDestroy{
  @Input() queue: SongPaginationModel = {songs: [], totalCount : 0} as SongPaginationModel;

  @Input() nextSongs: SongPaginationModel = {songs: [], totalCount : 0} as SongPaginationModel;

  @Output() paginationUpdated: EventEmitter<void> = new EventEmitter<void>();
  
  @Output() playSongClicked: EventEmitter<PlaylistSongModelParams> = new EventEmitter<PlaylistSongModelParams>();

  @Output() songDropped: EventEmitter<DragDropQueueParams> = new EventEmitter<DragDropQueueParams>();

  public IsLoading: Observable<boolean> = new Observable();

  private showCheckbox: boolean = false;

  private allChecked: boolean = false;

  private indeterminate: boolean = false;

  private selectedTableItem: PlaylistSongModelParams = {
    songModel: {
      id: '',
      order: -1,
      checked: false,
      isInFavorites: false
    },
    index : -1
  } as PlaylistSongModelParams;

  private modifieablePlaylists: GuidNameModel[] = [];

  private isSongPlaying: boolean = false;

  private currentPlayingSong: PlaylistSongModel = undefined as any;

  private destroy:Subject<any> = new Subject();


  /**
   *
   */
  constructor(private rxjsStorageService: RxjsStorageService, private playlistService: PlaylistService,
    private message: NzMessageService, private modal: NzModalService, private nzContextMenuService: NzContextMenuService, private songService: SongService,
    private queueService: QueueService, private streamingService: StreamingClientService, private wrapperService: QueueWrapperService,
     @Inject(DOCUMENT) private doc: Document) {
    console.log("Construct")
    this.IsLoading = this.rxjsStorageService.currentSongTableLoading$;
  }
  ngOnDestroy(): void {
    this.destroy.next(true);
  }

  ngOnInit(): void {
    this.rxjsStorageService.isSongPlayingState.pipe(takeUntil(this.destroy)).subscribe(x => {
      this.isSongPlaying = x;
    });

    this.rxjsStorageService.currentPlayingSong.pipe(takeUntil(this.destroy)).subscribe(x => {
      this.currentPlayingSong = x;
    });

    console.log("Set pag")
    console.log(this.queue);
    // this.isSongPlaying = isSongPlaying;
    // this.currentPlayingSong = currentPlayingSong;
  }

  public async removeSongFromQueue(songId: string, index: number): Promise<void> {
    try {
      await this.wrapperService.RemoveSongsFromQueue([index]);  
    } catch (error) {
      this.message.error("Error when removing songs from queue!");
    }
    
  }

  public async addSongToQueue(song: PlaylistSongModel): Promise<void> {
    try {
      await this.wrapperService.AddSongsToQueue([song.id]);     
    } catch (error) {
      this.message.error("Error when adding songs to queue!");
    }
    
  }

  // This method is only used when the queue is displayed
  public async removeSelectedSongsFromQueue(): Promise<void> {
    try {
      var checkedSongs = this.queue.songs.filter(x => x.checked);
      await this.wrapperService.RemoveSongsFromQueue(checkedSongs.map(x => x.order));
    } catch (error) {
      this.message.error("Error when removing songs from queue!");
    }

    this.checkAll(false);
  }

  public async addSelectedSongsToQueue(): Promise<void> {
    try {
      var checkedSongs = this.queue.songs.filter(x => x.checked);
      await this.wrapperService.AddSongsToQueue(checkedSongs.map(x => x.id));
    } catch (error) {
      this.message.error("Error when adding songs to queue!");
    }

    this.checkAll(false);
    // this.checkAll(false);
  }

  public async clearQueue(): Promise<void>{
    try {
      await this.wrapperService.ClearQueue();  
    } catch (error) {
      this.message.error("Error when clearing queue.");
    }
    
    this.paginationUpdated.emit();
  }

  public onQueryParamsChange(event: any): void{
    console.log("Query Changed")

    this.paginationUpdated.emit();
  }

  public contextMenu($event: MouseEvent, menu: NzDropdownMenuComponent, item: PlaylistSongModel, index: number): void {
    this.selectedTableItem = { index: index, songModel: item};
    this.playlistService.GetModifieablePlaylists(-1).subscribe((val)=>{
      this.modifieablePlaylists = val.playlists;
    })

    this.nzContextMenuService.create($event, menu);
    
    // Add events via jquery

  }

  closeMenu(): void {
    this.nzContextMenuService.close();
  }

  getAlbumCoverSrc(id: string): string{
    return `${environment.apiUrl}/${APIROUTES.file}/album/${id}`;
  }

  public checkAll(value: boolean): void {
    this.queue.songs.forEach(data => {
      data.checked = value;
    });

    this.refreshTableHeader();
  }

  refreshTableHeader(): void{
    const allChecked = this.queue.songs.length > 0 && this.queue.songs.every(value => value.checked === true);
    const allUnChecked = this.queue.songs.every(value => !value.checked);
    this.AllChecked = allChecked;
    this.Indeterminate = !allChecked && !allUnChecked;
  }

  public removeSongFromFavorites(songId: string): void{
    // Remove Song from Favorites
    this.removeSongsFromFavorites([songId]);
  }

  public addSongToFavorites(id: string): void{
    // Add Song to Favorites
    this.addSongsToFavorite([id]);
  }

  public addSongToPlaylist(playlistId: string): void{
    console.log(playlistId)
    this.addSongsToPlaylist([this.SelectedTableItem.id], playlistId);
  }

  public addSelectedSongsToPlaylist(playlistId: string): void{
    var checkedSongIds = this.queue.songs.filter(x => x.checked).map(x => x.id);
    this.addSongsToPlaylist(checkedSongIds, playlistId);
  }

  public removeSongsFromPlaylist(orderIds: number[], playlistId: string): void{
    this.playlistService.RemoveSongsFromPlaylist(orderIds, playlistId).subscribe({
      next: ()=>{
        // Show All good modal
        if (orderIds.length == 1) {
          this.message.success("Song was successfully removed from playlist!");
        }

        if (orderIds.length > 1) {
          this.message.success("Songs were successfully removed from playlist!");
        }

        this.checkAll(false);

        this.updateDashBoard();
        this.paginationUpdated.emit();
      }
    })
  }

  public addSelectedSongsToFavorites(): void{
    var checkedSongIds = this.queue.songs.filter(x => x.checked && !x.isInFavorites).map(x => x.id);

    if (checkedSongIds.length == 0) {
      return;
    }

    this.addSongsToFavorite(checkedSongIds);
  }

  public removeSelectedSongsFromFavorites(): void{
    var checkedSongIds = this.queue.songs.filter(x => x.checked && x.isInFavorites).map(x => x.id);

    if (checkedSongIds.length == 0) {
      return;
    }

    this.removeSongsFromFavorites(checkedSongIds);
  }

  public addSongsToFavorite(ids: string[]): void{
    this.playlistService.AddSongsToFavorites(ids).subscribe({
      next: ()=>{
        // Show Modal
        if (ids.length == 1) {
          this.message.success("Song was successfully added to favorites!");
        }

        if (ids.length > 1) {
          this.message.success("Songs were successfully added to favorites!");
        }

        this.checkAll(false);

        this.updateCurrentSong(ids);

        this.updateDashBoard();

        this.paginationUpdated.emit();
      }
    })
  }

  public addSongsToPlaylist(ids: string[], playlistId: string): void{
    this.playlistService.AddSongsToPlaylist(ids, playlistId).subscribe({
      next: ()=>{
        // Show Modal
        if (ids.length == 1) {
          this.message.success("Song was successfully added to playlist!");
        }

        if (ids.length > 1) {
          this.message.success("Songs were successfully added to playlist!");
        }

        this.updateDashBoard();

        // if (this.playlistId == undefined) {
        //   return;
        // }

        // this.indeterminate = false;
        // this.allChecked = false;
        this.checkAll(false);
        
        //this.paginationUpdated.emit();
        
        
      }
    })
  }

  public showRemoveSongsFromFavoritesModal(songIds: string[]): void{
    this.modal.confirm({
      nzTitle: 'Delete Favorites?',
      nzContent: '<b class="error-color">Are you sure you want to delete the songs from your favorites</b>',
      nzOkText: 'Delete',
      nzOkType: 'primary',
      nzOkDanger: true,
      nzOnOk: () => this.removeSongsFromFavorites(songIds),
      nzCancelText: 'Cancel',
      nzOnCancel: () => console.log('Cancel')
    });
  }
  
  public removeSongsFromFavorites(songIds: string[]): void{
    this.playlistService.RemoveSongsFromFavorites(songIds).subscribe({
      next: ()=>{
        // Show All good modal
        if (songIds.length == 1) {
          this.message.success("Song was successfully removed from favorites!");
        }

        if (songIds.length > 1) {
          this.message.success("Songs were successfully removed from favorites!");
        }

        this.checkAll(false);

        this.updateCurrentSong(songIds);

        this.updateDashBoard();
        this.paginationUpdated.emit();
      }
    });
  }

  public playSong(model: PlaylistSongModel, index: number): void{
    
    // Send event to outside component
    this.playSongClicked.emit({index: index, songModel: model} as PlaylistSongModelParams);
  }

  public async pauseSong(): Promise<void>{
    this.rxjsStorageService.setIsSongPlaylingState(false);
    try {
      await this.streamingService.playPauseSong(false);  
    } catch (error) {
      
    }
    
  }

  updateDashBoard(): void{
    var currenState = false;
    this.rxjsStorageService.updateDashboardBoolean$.subscribe((val) =>{
      currenState = val;
    })

    // Update value in rxjs so the dashboard gets updated
    this.rxjsStorageService.setUpdateDashboardBoolean(!currenState);
  }

  public updateQueue(): void{
    let queueBool = false;
    this.rxjsStorageService.updateQueueBoolean$.subscribe(x => {
      queueBool = x;
    });

    this.rxjsStorageService.setUpdateQueueBoolean(!queueBool);
  }

  updateCurrentSong(idOfChangedSongs: string[]): void{
    let currentSong = {} as PlaylistSongModel;
    this.rxjsStorageService.currentPlayingSong.subscribe(x => {
      currentSong = x;
    })

    if (!currentSong) {
      return;
    }

    const index = idOfChangedSongs.indexOf(currentSong.id);

    if (index == -1) {
      return;
    }

    this.songService.GetSongDetails(idOfChangedSongs[index]).subscribe({
      next: (x)=>{
        this.rxjsStorageService.setCurrentPlayingSong(x);
      }
    })
  }

  public getArtistsNamesAsList(artists: ArtistShortModel[]): string{
    return artists.map(x => x.name).join(', ');

  }

  public drop(event: CdkDragDrop<string[]>): void {
    console.log("drop")
    this.doc.body.classList.remove('inheritCursors');
    this.doc.body.style.cursor = 'unset'; 
    console.log(event)

    if (event.currentIndex == event.previousIndex && event.container.id == event.previousContainer.id) {
      return;
    }

    let srcSong = this.queue.songs[event.previousIndex];

    let destSong = this.queue.songs[event.currentIndex];

    let markAsManuallyAdded = -1;



    if (event.container.id == 'next-song-list' && event.currentIndex < this.nextSongs.totalCount) {
      destSong = this.nextSongs.songs[event.currentIndex];
    }

    if (event.previousContainer.id == 'next-song-list') {
      srcSong = this.nextSongs.songs[event.previousIndex];
    }

    if (event.container.id == 'queue-song-list' &&
      event.previousContainer.id == 'next-song-list' &&
      event.currentIndex == 0) {
      markAsManuallyAdded = 0;
      destSong = this.nextSongs.songs[this.nextSongs.totalCount - 1]; 
    }

    if (event.container.id == 'queue-song-list' &&
    event.previousContainer.id == 'next-song-list' &&
    event.currentIndex >= this.queue.totalCount) {
    destSong = this.queue.songs[this.queue.totalCount - 1]; 
  }

    if (event.previousContainer.id == 'queue-song-list' &&
      event.container.id == 'next-song-list' &&
      event.currentIndex >= this.nextSongs.totalCount) {
      markAsManuallyAdded = 1;
      destSong = this.queue.songs[0];
    }

    if (!srcSong || !destSong) {
      return;
    }

    this.songDropped.emit({ srcSong: srcSong, destSong: destSong, srcIndex: event.previousIndex, destIndex: event.currentIndex, markAsManuallyAdded: markAsManuallyAdded});
  }

  public drag(event: CdkDragStart<any>): void {
    console.log("drag")
    this.doc.body.classList.add('inheritCursors');
    this.doc.body.style.cursor = 'grabbing'; 

  }

  public get IsSongPlaying(): boolean{
    return this.isSongPlaying;
  }

  public get CurrentPlayingSong(): PlaylistSongModel{
    return this.currentPlayingSong;
  }

  public get ShowCheckbox(): boolean{
    return this.showCheckbox;
  }

  public set ShowCheckbox(val: boolean){
    this.showCheckbox = val;
  }

  public get AllChecked(): boolean{
    return this.allChecked;
  }

  public set AllChecked(val: boolean){
    this.allChecked = val;
  }

  public get Indeterminate(): boolean{
    return this.indeterminate;
  }

  public set Indeterminate(val: boolean){
    this.indeterminate = val;
  }

  public get SelectedTableItem(): PlaylistSongModel{
    return this.selectedTableItem.songModel;
  }

  public get SelectedTableItemIndex(): number{
    return this.selectedTableItem.index;
  }

  public get ModifieablePlaylists(): GuidNameModel[]{
    return this.modifieablePlaylists;
  }
}
