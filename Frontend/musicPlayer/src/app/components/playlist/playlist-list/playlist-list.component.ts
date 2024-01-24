import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { NzContextMenuService, NzDropdownMenuComponent } from 'ng-zorro-antd/dropdown';
import { NzModalService } from 'ng-zorro-antd/modal';
import { Observable, Subject, takeUntil } from 'rxjs';
import { APIROUTES } from 'src/app/constants/api-routes';
import { PlaylistModelParams, PlaylistSongModelParams } from 'src/app/models/events';
import { GuidNameModel, PlaylistPaginationModel, PlaylistUserShortModel } from 'src/app/models/playlist-models';
import { PaginationModel, QueueModel } from 'src/app/models/storage';
import { UserModel } from 'src/app/models/user-models';
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

  @Input() playlistDeletable: boolean = false;

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
  

  /**
   *
   */
  constructor(private playlistService: PlaylistService, private rxjsService: RxjsStorageService, private modal: NzModalService,
    private nzContextMenuService: NzContextMenuService) {
    this.IsLoading = this.rxjsService.currentSongTableLoading$;
    
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

  contextMenu($event: MouseEvent, menu: NzDropdownMenuComponent, item: PlaylistUserShortModel, index: number): void {
    this.selectedTableItem = { index: index, playlistModel: item};
    this.playlistService.GetModifieablePlaylists(-1).subscribe((val)=>{
      this.modifieablePlaylists = val.playlists;

      if (item.id != undefined) {
        this.modifieablePlaylists = val.playlists.filter(x => x.id != item.id);
      }
    })

    this.nzContextMenuService.create($event, menu);
    
    // Add events via jquery

  }
  addSongToPlaylist(arg0: string) {
    throw new Error('Method not implemented.');
    }
    DeletePlaylist() {
    throw new Error('Method not implemented.');
    }
    AddPlaylistToLibrary() {
    throw new Error('Method not implemented.');
    }
    addPlaylistToQueue() {
    throw new Error('Method not implemented.');
    }
    addPlaylistToFavorites() {
    throw new Error('Method not implemented.');
    }

  onQueryParamsChange(event: any): void{
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
      return;
    }

    this.rxjsService.setCurrentPaginationSongModel(newPagModel);

    this.paginationUpdated.emit();
  }

  getPlaylistCoverSrc(id: string) {
    return `${environment.apiUrl}/${APIROUTES.file}/playlist/${id}`;
  }

  getPlaylistCreator(users: UserModel[]) {
    return users.find(x => x.isCreator)?.userName;
  }

  setPublic(id: string) {
    throw new Error();

  }

  receiveNotifications(id: string) {
    this.playlistService.SetPlaylistNotifications(id).subscribe({
      complete: ()=>{
        this.paginationUpdated.emit();
      },
      error: (error) =>{
        console.log(error);
      }
    })
  }

  getHeaderSortOrder(sortOrder: string): string | null{
    if (this.pagination.sortAfter == sortOrder && this.pagination.asc) {
      return 'ascend';
    }

    if (this.pagination.sortAfter == sortOrder && !this.pagination.asc) {
      return 'descend';
    }

    return null;
  }

  onSearchQueryInput(): void{
    this.rxjsService.setCurrentPaginationSongModel(this.pagination);
    this.paginationUpdated.emit();
  }

  updateDashBoard(): void{
    var currenState = false;
    this.rxjsService.updateDashboardBoolean$.subscribe((val) =>{
      currenState = val;
    })

    // Update value in rxjs so the dashboard gets updated
    this.rxjsService.setUpdateDashboardBoolean(!currenState);
  }

  public updateQueue(): void{
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

}
