import { Component, OnInit } from '@angular/core';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzTableQueryParams } from 'ng-zorro-antd/table';
import { TableQuery } from 'src/app/models/events';
import { PlaylistSongPaginationModel } from 'src/app/models/playlist-models';
import { PaginationModel } from 'src/app/models/storage';
import { JwtService } from 'src/app/services/jwt.service';
import { PlaylistService } from 'src/app/services/playlist.service';
import { SessionStorageService } from 'src/app/services/session-storage.service';

@Component({
  selector: 'app-favorites',
  templateUrl: './favorites.component.html',
  styleUrls: ['./favorites.component.scss']
})
export class FavoritesComponent implements OnInit{

  private songsModel: PlaylistSongPaginationModel = {} as PlaylistSongPaginationModel;

  private userName: string = '';

  private paginationModel: PaginationModel = {
    asc: true,
    page : 1,
    query : '',
    sortAfter : '',
    take: 10
  } as PaginationModel;

  /**
   *
   */
  constructor(private playlistService: PlaylistService, private message: NzMessageService, 
    private sessionStorage: SessionStorageService, private jwtService: JwtService) {
    
    
  }

  ngOnInit(): void {
    this.userName = this.jwtService.getUserName();

    let savedPagination = this.sessionStorage.GetLastPaginationOfFavorites();

    if (savedPagination) {
      this.paginationModel = savedPagination;
    }

    // this.onGetFavorites(this.paginationModel.page, this.paginationModel.take, this.paginationModel.sortAfter, 
    //   this.paginationModel.asc, this.paginationModel.query);
  }

  public onGetFavorites(page: number, take: number, sortAfter: string, asc: boolean, query: string): void{
    this.playlistService.GetFavorites(page, take, sortAfter, asc, query).subscribe({
      next:(songsModel: PlaylistSongPaginationModel)=>{
        this.songsModel = songsModel;
      },
      error:(error: any)=>{
        this.message.error("Error when getting favorites.");
      }
    })
  }

  public onQueryReceived(event: TableQuery){
    // TODO: Find better way for pagination

    var sortAfter = event.params.sort.find(x => x.value);

    if (!sortAfter) {
      sortAfter = {
        key: '',
        value: 'ascend'
      }
    }

    let newPagModel: PaginationModel = {
      query :event.query,
      page : event.params.pageIndex,
      take: event.params.pageSize,
      sortAfter: sortAfter.key,
      asc: sortAfter.value == 'ascend' ? true: false
    }

    console.log("received")
    console.log(newPagModel)

    console.log("Current")
    console.log(this.paginationModel)

    if (JSON.stringify(newPagModel) == JSON.stringify(this.PaginationModel)) {
      console.log("Is Same pagination")
      this.onGetFavorites(this.paginationModel.page, this.paginationModel.take, this.paginationModel.sortAfter, 
        this.paginationModel.asc, this.paginationModel.query);

        return;
    }

    this.paginationModel = newPagModel;

    this.sessionStorage.SaveLastPaginationOfFavorites(newPagModel);
  }

  public get SongsModel(): PlaylistSongPaginationModel{
    return this.songsModel;
  }

  public get PaginationModel(): PaginationModel{
    return this.paginationModel;
  }

  public get TotalSongCount(): number{
    return this.songsModel.totalCount;
  }

  public get UserName(): string{
    return this.userName;
  }

}
