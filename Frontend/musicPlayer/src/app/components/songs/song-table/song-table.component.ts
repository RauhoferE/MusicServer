import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { NzTableQueryParams } from 'ng-zorro-antd/table';
import { APIROUTES } from 'src/app/constants/api-routes';
import { AlbumArtistModel } from 'src/app/models/artist-models';
import { TableQuery } from 'src/app/models/events';
import { PlaylistSongPaginationModel } from 'src/app/models/playlist-models';
import { PaginationModel } from 'src/app/models/storage';
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

  private isLoading : boolean = true;


  /**
   *
   */
  constructor(private rxjsStorageService: RxjsStorageService) {
    console.log("Construct")
    this.isLoading= false;


    
  }
  ngOnInit(): void {
    this.rxjsStorageService.currentPaginationSongModel$.subscribe((val) => {
      
      const pModel = val as PaginationModel;
      console.log("Set pag")
      this.pagination = pModel;
    })
  }

  onSearchQueryInput(): void{
    this.rxjsStorageService.setCurrentPaginationSongModel(this.pagination);
    this.paginationUpdated.emit();
  }



  onQueryParamsChange(event: any): void{
    console.log("Query Changed")
    this.isLoading = false;

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

  removeSongFromFavorites(id: string): void{
    // Remove Song from Favorites
    // Get new List

  }

  addSongToFavorites(id: string): void{
    // Add Song to Favorites
    // Get new List
  }

  public get PageSize(): number{
    return this.pagination.take;
  }

  public get PageIndex(): number{
    return this.pagination.page;
  }

  public get IsLoading(): boolean{
    return this.isLoading;
  }

  public get SearchString(): string {
    return this.pagination.query;
  }
  public set SearchString(value: string) {
    this.pagination.query = value;
  }



}
