import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { Observable, Subject, takeUntil } from 'rxjs';
import { APIROUTES } from 'src/app/constants/api-routes';
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

  @Output() paginationUpdated: EventEmitter<void> = new EventEmitter<void>();

  private isSongPlaying: boolean = false;

  private queueModel: QueueModel = undefined as any;

  private modifieablePlaylists: GuidNameModel[] = [];

  public IsLoading: Observable<boolean> = new Observable();

  private showCheckbox: boolean = false;

  private allChecked: boolean = false;

  private indeterminate: boolean = false;

  private destroy:Subject<any> = new Subject();

  private pagination: PaginationModel = {} as PaginationModel;
  

  /**
   *
   */
  constructor(private playlistService: PlaylistService, private rxjsService: RxjsStorageService) {
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

    this.AllChecked = false;
    this.Indeterminate = false;

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

  checkAll(value: boolean): void {
    this.playlistModel.playlists.forEach(data => {
      data.checked = value;
    });

    this.refreshTableHeader();
  }

  refreshTableHeader(): void{
    const allChecked = this.playlistModel.playlists.length > 0 && this.playlistModel.playlists.every(value => value.checked === true);
    const allUnChecked = this.playlistModel.playlists.every(value => !value.checked);
    this.AllChecked = allChecked;
    this.Indeterminate = !allChecked && !allUnChecked;
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

  public get ModifieablePlaylists(): GuidNameModel[]{
    return this.modifieablePlaylists;
  }

}
