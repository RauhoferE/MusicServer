import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzModalService } from 'ng-zorro-antd/modal';
import { NzTableQueryParams } from 'ng-zorro-antd/table';
import { Observable } from 'rxjs';
import { APIROUTES } from 'src/app/constants/api-routes';
import { AlbumArtistModel } from 'src/app/models/artist-models';
import { TableQuery } from 'src/app/models/events';
import { PlaylistSongPaginationModel } from 'src/app/models/playlist-models';
import { PaginationModel } from 'src/app/models/storage';
import { PlaylistService } from 'src/app/services/playlist.service';
import { RxjsStorageService } from 'src/app/services/rxjs-storage.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-song-table',
  templateUrl: './song-table.component.html',
  styleUrls: ['./song-table.component.scss']
})
export class SongTableComponent implements OnInit {

  @Input() songs!: PlaylistSongPaginationModel;

  private pagination: PaginationModel = {} as PaginationModel;

  @Output() paginationUpdated: EventEmitter<void> = new EventEmitter<void>();

  public IsLoading: Observable<boolean> = new Observable();

  private showCheckbox: boolean = false;

  private allChecked: boolean = false;

  private indeterminate: boolean = false;


  /**
   *
   */
  constructor(private rxjsStorageService: RxjsStorageService, private playlistService: PlaylistService,
    private message: NzMessageService, private modal: NzModalService) {
    console.log("Construct")
    this.IsLoading = this.rxjsStorageService.currentSongTableLoading$;
  }

  ngOnInit(): void {
    let pModel = {} as PaginationModel;
    this.rxjsStorageService.currentPaginationSongModel$.subscribe((val) => {
      
      pModel = val as PaginationModel;
    });

    console.log("Set pag")
    this.pagination = pModel;
  }

  onSearchQueryInput(): void{
    this.rxjsStorageService.setCurrentPaginationSongModel(this.pagination);
    this.paginationUpdated.emit();
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

    this.rxjsStorageService.setCurrentPaginationSongModel(newPagModel);

    this.paginationUpdated.emit();
  }

  getAlbumLink(albumModel: AlbumArtistModel): string{
    return `album/${albumModel.id}`;
  }

  getAlbumCoverSrc(id: string): string{
    return `${environment.apiUrl}/${APIROUTES.file}/album/${id}`;
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

  checkAll(value: boolean): void {
    this.songs.songs.forEach(data => {
      data.checked = value;
    });

    this.refreshTableHeader();
  }

  refreshTableHeader(): void{
    const allChecked = this.songs.songs.length > 0 && this.songs.songs.every(value => value.checked === true);
    const allUnChecked = this.songs.songs.every(value => !value.checked);
    this.AllChecked = allChecked;
    this.Indeterminate = !allChecked && !allUnChecked;
  }

  removeSongFromFavorites(songId: string): void{
    // Remove Song from Favorites
    this.removeSongsFromFavorites([songId]);
  }

  addSongToFavorites(id: string): void{
    // Add Song to Favorites
    this.playlistService.AddSongsToFavorites([id]);
  }

  addSongsToFavorite(ids: string[]): void{
    this.playlistService.AddSongsToFavorites(ids).subscribe({
      next: ()=>{
        // Show Modal
        if (ids.length == 1) {
          this.message.success("Song was successfully added to favorites!");
        }

        if (ids.length > 1) {
          this.message.success("Songs were successfully added to favorites!");
        }
        
        this.paginationUpdated.emit();
      }
    })
  }

  showRemoveSongsFromFavoritesModal(songIds: string[]){
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
  
  removeSongsFromFavorites(songIds: string[]): void{
    this.playlistService.RemoveSongsFromFavorites(songIds).subscribe({
      next: ()=>{
        // Show All good modal
        if (songIds.length == 1) {
          this.message.success("Song was successfully removed from favorites!");
        }

        if (songIds.length > 1) {
          this.message.success("Songs were successfully removed from favorites!");
        }

        var currenState = false;
        this.rxjsStorageService.currentSongInTableChanged$.subscribe((val) =>{
          currenState = val;
        })

        // Update value in rxjs so the dashboard gets updated
        this.rxjsStorageService.setSongInTableChangedState(!currenState);
        this.paginationUpdated.emit();
      }
    });
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

}
