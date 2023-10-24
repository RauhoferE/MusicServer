import { Component, EventEmitter, Input, Output } from '@angular/core';
import { NzTableQueryParams } from 'ng-zorro-antd/table';
import { APIROUTES } from 'src/app/constants/api-routes';
import { AlbumArtistModel } from 'src/app/models/artist-models';
import { TableQuery } from 'src/app/models/events';
import { PlaylistSongPaginationModel } from 'src/app/models/playlist-models';
import { PaginationModel } from 'src/app/models/storage';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-song-table',
  templateUrl: './song-table.component.html',
  styleUrls: ['./song-table.component.scss']
})
export class SongTableComponent {

  @Input() songs!: PlaylistSongPaginationModel;

  @Input() pagination!: PaginationModel;

  @Output() query: EventEmitter<TableQuery> = new EventEmitter<TableQuery>();

  private isLoading : boolean = true;

  private searchString: string = '';


  /**
   *
   */
  constructor() {
    console.log("Construct")
    this.isLoading= false;

    
  }

  onQueryParamsChange(event: any): void{
    // Throw event
    console.log("Query Changed")
    this.isLoading = false;
    console.log(event)
    this.query.emit({
    params: event,
      query: this.searchString
    } as TableQuery);

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
    return this.searchString;
  }
  public set SearchString(value: string) {
    this.searchString = value;
  }



}
