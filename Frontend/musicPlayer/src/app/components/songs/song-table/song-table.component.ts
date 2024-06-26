import { Component, EventEmitter, Inject, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { NzContextMenuService, NzDropdownMenuComponent } from 'ng-zorro-antd/dropdown';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzModalService } from 'ng-zorro-antd/modal';
import { NzTableQueryParams } from 'ng-zorro-antd/table';
import { Observable, Subject, every, lastValueFrom, takeUntil } from 'rxjs';
import { APIROUTES } from 'src/app/constants/api-routes';
import { AlbumArtistModel, ArtistShortModel } from 'src/app/models/artist-models';
import { DragDropSongParams, EditPlaylistModalParams, PlaylistSongModelParams, TableQuery } from 'src/app/models/events';
import { GuidNameModel, PlaylistSongModel, SongPaginationModel } from 'src/app/models/playlist-models';
import { PaginationModel } from 'src/app/models/storage';
import { PlaylistService } from 'src/app/services/playlist.service';
import { RxjsStorageService } from 'src/app/services/rxjs-storage.service';
import { SongService } from 'src/app/services/song.service';
import { environment } from 'src/environments/environment';
import { CdkDragDrop, CdkDragStart, moveItemInArray } from '@angular/cdk/drag-drop';
import { DOCUMENT } from '@angular/common';
import { QueueService } from 'src/app/services/queue.service';
import { FileService } from 'src/app/services/file.service';
import { QueueWrapperService } from 'src/app/services/queue-wrapper.service';
import { StreamingClientService } from 'src/app/services/streaming-client.service';

@Component({
  selector: 'app-song-table',
  templateUrl: './song-table.component.html',
  styleUrls: ['./song-table.component.scss']
})
export class SongTableComponent implements OnInit, OnDestroy {

  @Input() songs: SongPaginationModel = {songs: [], totalCount : 0} as SongPaginationModel;
  
  @Input() sortingEnabled: boolean = true;

  @Input() displayHeader: boolean = true;

  @Input() displayPagination: boolean = true;

  @Input() displaySearch: boolean = true;

  @Input() displayAlbum: boolean = true;

  @Input() disableDragDrop: boolean = false;

  private pagination: PaginationModel = {} as PaginationModel;

  @Output() paginationUpdated: EventEmitter<void> = new EventEmitter<void>();
  
  @Output() playSongClicked: EventEmitter<PlaylistSongModelParams> = new EventEmitter<PlaylistSongModelParams>();

  @Output() songDropped: EventEmitter<DragDropSongParams> = new EventEmitter<DragDropSongParams>();

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


  @Input() playlistId!: string;

  private modifieablePlaylists: GuidNameModel[] = [];

  private isSongPlaying: boolean = false;

  private currentPlayingSong: PlaylistSongModel = undefined as any;

  private showPlaylistCreateModal: boolean = false;

  private searchString: string = '';

  private songRearangeEnabled: boolean = false;

  private destroy:Subject<any> = new Subject();


  /**
   *
   */
  constructor(private rxjsStorageService: RxjsStorageService, private playlistService: PlaylistService,
    private message: NzMessageService, private modal: NzModalService, private nzContextMenuService: NzContextMenuService, private songService: SongService,
     private fileService: FileService, private wrapperService: QueueWrapperService,
     @Inject(DOCUMENT) private doc: Document, private streamingService: StreamingClientService) {
    console.log("Construct")
    this.IsLoading = this.rxjsStorageService.currentSongTableLoading$;
  }
  
  ngOnDestroy(): void {
    this.destroy.next(true);
  }

  ngOnInit(): void {
    let pModel = {} as PaginationModel;
    this.rxjsStorageService.currentPaginationSongModel$.pipe(takeUntil(this.destroy)).subscribe((val) => {
      
      pModel = val as PaginationModel;
    });

    this.rxjsStorageService.isSongPlayingState.pipe(takeUntil(this.destroy)).subscribe(x => {
      this.isSongPlaying = x;
    });

    this.rxjsStorageService.currentPlayingSong.pipe(takeUntil(this.destroy)).subscribe(x => {
      this.currentPlayingSong = x;
    });

    console.log("Set pag")
    this.pagination = pModel;
    console.log(this.songs);
    // this.isSongPlaying = isSongPlaying;
    // this.currentPlayingSong = currentPlayingSong;
  }

  public onSearchQueryInput(): void{
    this.pagination.query = this.SearchString;
    this.rxjsStorageService.setCurrentPaginationSongModel(this.pagination);
    this.paginationUpdated.emit();
  }

  public async addSongToQueue(song: PlaylistSongModel): Promise<void> {
    try {
      await this.wrapperService.AddSongsToQueue([song.id]);  
    } catch (error) {
      this.message.error("Error when adding songs to queue!");
    } 
  }

  public async addSelectedSongsToQueue(): Promise<void> {
    try {
      var checkedSongs = this.songs.songs.filter(x => x.checked);
      await this.wrapperService.AddSongsToQueue(checkedSongs.map(x => x.id));
    } catch (error) {
      this.message.error("Error when adding songs to queue!");
    }

    this.checkAll(false)
  }

  public onQueryParamsChange(event: any): void{
    console.log("Query Changed")

    var sortAfter = event.sort.find((x: any) => x.value);

    if (!sortAfter) {
      sortAfter = {
        key: '',
        value: 'ascend'
      }
    }
    console.log(event)
    console.log(this.SearchString)

    this.pagination.query = this.SearchString;

    let newPagModel: PaginationModel = {
      query :this.SearchString,
      page : event.pageIndex,
      take: event.pageSize,
      sortAfter: sortAfter.key,
      asc: sortAfter.value == 'ascend' ? true: false
    }

    // This is done because otherwise the event would be called again when the data of the table changes
    if (JSON.stringify(newPagModel) == JSON.stringify(this.pagination)) {
      return;
    }

    this.AllChecked = false;
    this.Indeterminate = false;

    this.rxjsStorageService.setCurrentPaginationSongModel(newPagModel);

    this.paginationUpdated.emit();
  }

  public enableSongRearange(): void {
    this.songRearangeEnabled = !this.songRearangeEnabled;

    if (this.songRearangeEnabled) {
      // rearange songs
      this.showCheckbox = false;
      this.onQueryParamsChange({
        pageIndex:1,
        pageSize: this.pagination.take,
        sort:[

          {key:'order', value:'ascend'}
        
      ]});
    }
  }

  public contextMenu($event: MouseEvent, menu: NzDropdownMenuComponent, item: PlaylistSongModel, index: number): void {
    this.selectedTableItem = { index: index, songModel: item};
    this.getPlaylists();

    this.nzContextMenuService.create($event, menu);
    
    // Add events via jquery

  }

  public getPlaylists(): void{
    this.playlistService.GetModifieablePlaylists(-1).subscribe((val)=>{
      this.modifieablePlaylists = val.playlists;

      if (this.playlistId != undefined) {
        this.modifieablePlaylists = val.playlists.filter(x => x.id != this.playlistId);
      }
    });
  }

  public async createPlaylist(event: EditPlaylistModalParams): Promise<void> {
    this.ShowPlaylistCreateModal = false;

    try {
      var playlistId = await lastValueFrom(this.playlistService.CreatePlaylist(event.playlistModel.name, event.playlistModel.description, event.playlistModel.isPublic,
        event.playlistModel.receiveNotifications));
  
        if (event.newCoverFile) {
          await lastValueFrom(this.fileService.ChangePlaylistCover(event.newCoverFile, playlistId));
        }

        this.getPlaylists();
        this.updateDashBoard();
    } catch (error) {
      console.log(error);
      this.message.error("Error when creating playlist");
      
    }
  }

  public closeMenu(): void {
    this.nzContextMenuService.close();
  }

  public getAlbumCoverSrc(id: string): string{
    return `${environment.apiUrl}/${APIROUTES.file}/album/${id}`;
  }

  public getHeaderSortOrder(sortOrder: string): string | null{
    if (this.pagination.sortAfter == sortOrder && this.pagination.asc) {
      return 'ascend';
    }

    if (this.pagination.sortAfter == sortOrder && !this.pagination.asc) {
      return 'descend';
    }

    return null;
  }

  public checkAll(value: boolean): void {
    this.songs.songs.forEach(data => {
      data.checked = value;
    });

    this.refreshTableHeader();
  }

  public refreshTableHeader(): void{
    const allChecked = this.songs.songs.length > 0 && this.songs.songs.every(value => value.checked === true);
    const allUnChecked = this.songs.songs.every(value => !value.checked);
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
    var checkedSongIds = this.songs.songs.filter(x => x.checked).map(x => x.id);
    this.addSongsToPlaylist(checkedSongIds, playlistId);
  }

  public removeSongFromPlaylist(): void{
    if (this.playlistId == undefined) {
      return;
    }

    this.removeSongsFromPlaylist([this.SelectedTableItem.order], this.playlistId);
  }

  public removeSelectedSongsFromPlaylist(): void{
    if (this.playlistId == undefined) {
      return;
    }

    var checkedSongIds = this.songs.songs.filter(x => x.checked).map(x => x.order);
    this.removeSongsFromPlaylist(checkedSongIds, this.playlistId);
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
    var checkedSongIds = this.songs.songs.filter(x => x.checked && !x.isInFavorites).map(x => x.id);

    if (checkedSongIds.length == 0) {
      return;
    }

    this.addSongsToFavorite(checkedSongIds);
  }

  public removeSelectedSongsFromFavorites(): void{
    var checkedSongIds = this.songs.songs.filter(x => x.checked && x.isInFavorites).map(x => x.id);

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

    if (event.currentIndex == event.previousIndex) {
      return;
    }

    const srcSong = this.songs.songs[event.previousIndex];

    const destSong = this.songs.songs[event.currentIndex];

    if (!srcSong || !destSong) {
      return;
    }

    this.songDropped.emit({ srcSong: srcSong, destSong: destSong, srcIndex: event.previousIndex, destIndex: event.currentIndex});
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

  public get PageSize(): number{
    return this.pagination.take;
  }

  public get PageIndex(): number{
    return this.pagination.page;
  }

  public get SearchString(): string {
    return this.searchString;
  }
  public set SearchString(value: string) {
    this.searchString = value;
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

  public get ShowPlaylistCreateModal(): boolean{
    return this.showPlaylistCreateModal;
  }

  public set ShowPlaylistCreateModal(val: boolean){
    this.showPlaylistCreateModal = val;
  }

  public get IsInitialTableEmpty(): boolean{
    return this.songs.totalCount == 0 && this.pagination.query == '';
  }

  public get IsSongTableEmpty(): boolean{
    return this.songs.totalCount == 0;
  }

  public get SongRearangeEnabled(): boolean{
    return this.songRearangeEnabled;
  }

  public set SongRearangeEnabled(val: boolean){
    this.songRearangeEnabled = val;
  }

}
