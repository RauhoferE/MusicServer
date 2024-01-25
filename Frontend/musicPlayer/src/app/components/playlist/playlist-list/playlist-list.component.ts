import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { NzContextMenuService, NzDropdownMenuComponent } from 'ng-zorro-antd/dropdown';
import { NzModalService } from 'ng-zorro-antd/modal';
import { Observable, Subject, lastValueFrom, takeUntil } from 'rxjs';
import { APIROUTES } from 'src/app/constants/api-routes';
import { EditPlaylistModalParams, PlaylistModelParams, PlaylistSongModelParams } from 'src/app/models/events';
import { GuidNameModel, PlaylistPaginationModel, PlaylistUserShortModel } from 'src/app/models/playlist-models';
import { PaginationModel, QueueModel } from 'src/app/models/storage';
import { UserModel } from 'src/app/models/user-models';
import { FileService } from 'src/app/services/file.service';
import { JwtService } from 'src/app/services/jwt.service';
import { PlaylistService } from 'src/app/services/playlist.service';
import { RxjsStorageService } from 'src/app/services/rxjs-storage.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-playlist-list',
  templateUrl: './playlist-list.component.html',
  styleUrls: ['./playlist-list.component.scss']
})
export class PlaylistListComponent implements OnInit, OnDestroy {

  @Input() playlistModel: PlaylistPaginationModel = {} as PlaylistPaginationModel;

  @Input() draggable: boolean = false;

  @Input() playlistEditable: boolean = false;

  @Input() addCopyPlaylistAction: boolean = false;

  @Output() paginationUpdated: EventEmitter<void> = new EventEmitter<void>();

  private isSongPlaying: boolean = false;

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
  

  /**
   *
   */
  constructor(private playlistService: PlaylistService, private rxjsService: RxjsStorageService, private modal: NzModalService,
    private nzContextMenuService: NzContextMenuService, private jwtService: JwtService, private fileService: FileService) {
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
      },
      error: (error)=>{
        console.log(error);
      }
    })
  }

  public addPlaylistToQueue(): void {
    // TODO: 
  throw new Error('Method not implemented.');
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

  public canPlaylistAddedToLibrary(): boolean {
    if (this.selectedTableItem.index == -1) {
      return false;
    }

    return this.selectedTableItem.playlistModel.users.findIndex(x => x.id == parseInt(this.currentUserId)) == -1;
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

    let newPagModel: PaginationModel = {
      query :this.pagination.query,
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

  public setPublic(playlist: PlaylistUserShortModel): void {
    this.modifyPlaylist(playlist);

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
    this.rxjsService.setCurrentPaginationSongModel(this.pagination);
    this.paginationUpdated.emit();
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
    return this.pagination.query;
  }
  public set SearchString(value: string) {
    this.pagination.query = value;
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

}
