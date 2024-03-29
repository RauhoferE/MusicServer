import { CdkDragDrop, CdkDragStart } from '@angular/cdk/drag-drop';
import { DOCUMENT } from '@angular/common';
import { Component, EventEmitter, Inject, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { NzContextMenuService, NzDropdownMenuComponent } from 'ng-zorro-antd/dropdown';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzModalService } from 'ng-zorro-antd/modal';
import { Observable, Subject, lastValueFrom, takeUntil } from 'rxjs';
import { APIROUTES } from 'src/app/constants/api-routes';
import { LOOPMODES } from 'src/app/constants/loop-modes';
import { QUEUETYPES } from 'src/app/constants/queue-types';
import { DragDropPlaylistParams, EditPlaylistModalParams, PlaylistModelParams, PlaylistSongModelParams } from 'src/app/models/events';
import { GuidNameModel, PlaylistPaginationModel, PlaylistSongModel, PlaylistUserShortModel } from 'src/app/models/playlist-models';
import { PaginationModel, QueueModel } from 'src/app/models/storage';
import { UserModel } from 'src/app/models/user-models';
import { FileService } from 'src/app/services/file.service';
import { JwtService } from 'src/app/services/jwt.service';
import { PlaylistService } from 'src/app/services/playlist.service';
import { QueueService } from 'src/app/services/queue.service';
import { RxjsStorageService } from 'src/app/services/rxjs-storage.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-playlist-list',
  templateUrl: './playlist-list.component.html',
  styleUrls: ['./playlist-list.component.scss']
})
export class PlaylistListComponent implements OnInit, OnDestroy {

  @Input() playlistModel: PlaylistPaginationModel = {} as PlaylistPaginationModel;

  @Input() playlistsEditable: boolean = false;

  @Output() paginationUpdated: EventEmitter<void> = new EventEmitter<void>();

  @Output() playlistDropped: EventEmitter<DragDropPlaylistParams> = new EventEmitter<DragDropPlaylistParams>();

  private isSongPlaying: boolean = false;

  private currentPlayingSong: PlaylistSongModel = undefined as any;

  private queueModel: QueueModel = undefined as any;

  private modifieablePlaylists: GuidNameModel[] = [];

  public IsLoading: Observable<boolean> = new Observable();

  private destroy:Subject<any> = new Subject();

  private pagination: PaginationModel = {} as PaginationModel;

  private selectedTableItem: PlaylistModelParams = {
    playlistModel: {

    },
    index : -1
  } as PlaylistModelParams;

  private currentUserId: string = '-1';

  private showPlaylistEditModal: boolean = false;

  private searchstring: string = '';

  private songRearangeEnabled: boolean = false;
  

  /**
   *
   */
  constructor(private playlistService: PlaylistService, private rxjsService: RxjsStorageService, private modal: NzModalService,
    private nzContextMenuService: NzContextMenuService, private jwtService: JwtService, private fileService: FileService, 
    @Inject(DOCUMENT) private doc: Document, private queueService: QueueService, private message: NzMessageService) {
    this.IsLoading = this.rxjsService.currentSongTableLoading$;
    this.currentUserId = jwtService.getUserId();
    
  }

  ngOnDestroy(): void {
    this.destroy.next(true);
  }

  ngOnInit(): void {
    let pModel = {} as PaginationModel;
    this.rxjsService.currentPaginationSongModel$.pipe(takeUntil(this.destroy)).subscribe((val) => {
      
      pModel = val as PaginationModel;
    });

    this.rxjsService.isSongPlayingState.pipe(takeUntil(this.destroy)).subscribe(x => {
      this.isSongPlaying = x;
    });

    this.rxjsService.currentPlayingSong.pipe(takeUntil(this.destroy)).subscribe(x =>{
      this.currentPlayingSong = x;
    });

    this.rxjsService.currentQueueFilterAndPagination.pipe(takeUntil(this.destroy)).subscribe(x => {
      this.queueModel = x;
    });

    this.pagination = pModel;
  }

  public contextMenu($event: MouseEvent, menu: NzDropdownMenuComponent, item: PlaylistUserShortModel, index: number): void {
    this.selectedTableItem = { index: index, playlistModel: item};

    this.nzContextMenuService.create($event, menu);
    
    // Add events via jquery

  }

  public deletePlaylist(itemId: string, playlistName: string): void {
    this.modal.confirm({
      nzTitle: `<b class="error-color">You really want to delete ${playlistName}?</b>`,
      nzOkText: 'Yes',
      nzCancelText: 'No',
      nzOnOk: ()=>{
        this.playlistService.DeletePlaylist(itemId).subscribe({
          next: ()=>{
            this.paginationUpdated.emit();
          },
          error: (error)=>{
            console.log(error);
          }
        })
      }
    })


  }

  public addPlaylistToLibrary(): void {
    if (this.selectedTableItem.index == -1 || this.selectedTableItem.playlistModel.id == undefined) {
      return;
    }


    this.playlistService.AddPlaylistToLibrary(this.selectedTableItem.playlistModel.id).subscribe({
      next: ()=>{
        this.paginationUpdated.emit();
        this.updateDashBoard();
      },
      error: (error)=>{
        console.log(error);
      }
    })
  }

  public addPlaylistToQueue(): void {

    if (this.selectedTableItem.playlistModel.songCount == 0) {
      this.message.error("Playlist needs to have songs to be played!");
      return;
    }
    
    this.queueService.AddPlaylistToQueue(this.selectedTableItem.playlistModel.id).subscribe({
      next: ()=>{
        //this.updateDashBoard();

      },
      error: (error)=>{
        console.log(error);
      }
    })
  }

  public async savePlaylist(event: EditPlaylistModalParams): Promise<void> {
    console.log(event)
    this.modifyPlaylist(event.playlistModel);

    if (event.newCoverFile) {
      await lastValueFrom(this.fileService.ChangePlaylistCover(event.newCoverFile, event.playlistModel.id));
    }
    this.ShowPlaylistEditModal = false;
    this.selectedTableItem = {index: -1} as PlaylistModelParams;
    
  }

  private modifyPlaylist(playlist: PlaylistUserShortModel): void{
    this.playlistService.ModifyPlaylistInfo(playlist.id, playlist.name, playlist.description, !playlist.isPublic, playlist.receiveNotifications).subscribe({
      complete: ()=>{
        this.paginationUpdated.emit();
        this.updateDashBoard();
      },
      error: (error) =>{
        console.log(error);
      }
    })
  }

  public copyPlaylistToLibrary(): void {
    if (this.selectedTableItem.index == -1 || this.selectedTableItem.playlistModel.id == undefined) {
      return;
    }


    this.playlistService.CopyPlaylistToLibrary(this.selectedTableItem.playlistModel.id).subscribe({
      next: ()=>{
        this.paginationUpdated.emit();
      },
      error: (error)=>{
        console.log(error);
      }
    })
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

    this.pagination.query = this.searchstring;

    let newPagModel: PaginationModel = {
      query :this.searchstring,
      page : event.pageIndex,
      take: event.pageSize,
      sortAfter: sortAfter.key,
      asc: sortAfter.value == 'ascend' ? true: false
    }

    // This is done because otherwise the event would be called again when the data of the table changes
    if (JSON.stringify(newPagModel) == JSON.stringify(this.pagination)) {
      console.log("Model same")
      return;
    }

    this.rxjsService.setCurrentPaginationSongModel(newPagModel);

    this.paginationUpdated.emit();
  }

  public enableSongRearange(): void {
    this.songRearangeEnabled = !this.songRearangeEnabled;

    if (this.songRearangeEnabled) {
      // rearange songs
      this.onQueryParamsChange({
        pageIndex:1,
        pageSize: this.pagination.take,
        sort:[

          {key:'order', value:'ascend'}
        
      ]});
    }
  }

  public async playPlaylist(playlistId: string, songCount: number): Promise<void>{
    console.log("Play playlist")

    if (songCount == 0) {
      this.message.error("Playlist needs to have songs to be played!");
      return;
    }

    // If the user previously clicked stop and wants to resume the playlist with the same queue
    if (this.QueueModel &&
      this.QueueModel.target == QUEUETYPES.playlist && 
      this.QueueModel.itemId == playlistId) {
      this.rxjsService.setIsSongPlaylingState(true);
      return;
    }

    this.rxjsService.setQueueFilterAndPagination({
      asc : this.pagination.asc,
      page : 0,
      take : 0,
      query : '',
      sortAfter : this.pagination.sortAfter,
      itemId : playlistId,
      target : QUEUETYPES.playlist,
      loopMode: this.queueModel.loopMode== undefined ? LOOPMODES.none: this.queueModel.loopMode,
      random: this.queueModel.random == undefined ? false: this.queueModel.random,
      userId: this.queueModel.userId
    });

    this.queueService.CreateQueueFromPlaylist(playlistId, this.queueModel.random, this.queueModel.loopMode, this.pagination.sortAfter, this.pagination.asc, -1).subscribe({
      next:async (song: PlaylistSongModel)=>{
        console.log(song)
        
        this.rxjsService.setCurrentPlayingSong(song);
        await this.rxjsService.setUpdateSongState();
        this.rxjsService.setIsSongPlaylingState(true);
        this.rxjsService.showMediaPlayer(true);
      },
      error:(error: any)=>{
        this.message.error("Error when getting queue.");
      },
      complete: () => {
      }
    });
  }

  public pausePlaylist(): void {
    // Stop playing of song
    this.rxjsService.setIsSongPlaylingState(false);
  }

  public setPublic(playlist: PlaylistUserShortModel): void {
    this.modifyPlaylist(playlist);

  }

  public addToLibrary(): void {
    this.playlistService.AddPlaylistToLibrary(this.selectedTableItem.playlistModel.id).subscribe({
      next: ()=>{
        this.updateDashBoard();
      },
      error: (error)=>{
        console.log(error);
      }
    })
  }

  public copyToLibrary(): void {
    this.playlistService.CopyPlaylistToLibrary(this.selectedTableItem.playlistModel.id).subscribe({
      next: ()=>{
        this.updateDashBoard();
      },
      error: (error)=>{
        console.log(error);
      }
    });
  }

  public receiveNotifications(id: string): void {
    this.playlistService.SetPlaylistNotifications(id).subscribe({
      complete: ()=>{
        this.paginationUpdated.emit();
      },
      error: (error) =>{
        console.log(error);
      }
    })
  }

  public getPlaylistCoverSrc(id: string): string {
    return `${environment.apiUrl}/${APIROUTES.file}/playlist/${id}`;
  }

  public getPlaylistCreator(users: UserModel[]): UserModel | undefined {
    return users.find(x => x.isCreator);
  }

  public isUserPartOfPlaylist(): boolean{
    if (!this.selectedTableItem.playlistModel || !this.selectedTableItem.playlistModel.users) {
      return false;
    }

    const users = this.selectedTableItem.playlistModel.users;
    
    return (users.findIndex(x => x.id == parseInt(this.currentUserId)) > -1)
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

  public onSearchQueryInput(): void{
    this.pagination.query = this.searchstring;
    this.rxjsService.setCurrentPaginationSongModel(this.pagination);
    this.paginationUpdated.emit();
  }

  public drop(event: CdkDragDrop<string[]>): void {
    console.log("drop")
    this.doc.body.classList.remove('inheritCursors');
    this.doc.body.style.cursor = 'unset'; 

    if (event.currentIndex == event.previousIndex) {
      return;
    }

    const srcPlaylist = this.playlistModel.playlists[event.previousIndex];

    const destPlaylist = this.playlistModel.playlists[event.currentIndex];

    if (!srcPlaylist || !destPlaylist) {
      return;
    }

    console.log(srcPlaylist.order)
    console.log(destPlaylist.order)

    this.playlistDropped.emit({ srcPlaylist: srcPlaylist, destPlaylist: destPlaylist, srcIndex: event.previousIndex, destIndex: event.currentIndex});
  }

  public drag(event: CdkDragStart<any>): void {
    console.log("drag")
    this.doc.body.classList.add('inheritCursors');
    this.doc.body.style.cursor = 'grabbing'; 

  }

  private updateDashBoard(): void{
    var currenState = false;
    this.rxjsService.updateDashboardBoolean$.subscribe((val) =>{
      currenState = val;
    })

    // Update value in rxjs so the dashboard gets updated
    this.rxjsService.setUpdateDashboardBoolean(!currenState);
  }

  private updateQueue(): void{
    let queueBool = false;
    this.rxjsService.updateQueueBoolean$.subscribe(x => {
      queueBool = x;
    });

    this.rxjsService.setUpdateQueueBoolean(!queueBool);
  }

  public get QueueModel(): QueueModel{
    return this.queueModel;
  }

  public get IsSongPlaying(): boolean{
    return this.isSongPlaying;
  }

  public get PageSize(): number{
    return this.pagination.take;
  }

  public get PageIndex(): number{
    return this.pagination.page;
  }

  public get SearchString(): string {
    return this.searchstring;
  }
  public set SearchString(value: string) {
    this.searchstring = value;
  }

  public get ModifieablePlaylists(): GuidNameModel[]{
    return this.modifieablePlaylists;
  }

  public get SelectedTableItem(): PlaylistModelParams{
    return this.selectedTableItem;
  }

  public get ShowPlaylistEditModal(): boolean{
    return this.showPlaylistEditModal;
  }

  public set ShowPlaylistEditModal(val: boolean){
    this.showPlaylistEditModal = val;
  }

  public get CurrentPlayingSong(): PlaylistSongModel{
    return this.currentPlayingSong;
  }

  public get QueueTypePlaylist(): string{
    return QUEUETYPES.playlist;
  }

  public get IsInitialTableEmpty(): boolean{
    return this.playlistModel.totalCount == 0 && this.pagination.query == '';
  }

  public get IsSongTableEmpty(): boolean{
    return this.playlistModel.totalCount == 0;
  }

  public get SongRearangeEnabled(): boolean{
    return this.songRearangeEnabled;
  }

  public set SongRearangeEnabled(val: boolean){
    this.songRearangeEnabled = val;
  }

}
