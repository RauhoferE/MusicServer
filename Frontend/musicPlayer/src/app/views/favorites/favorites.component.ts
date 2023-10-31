import { Component, OnInit } from '@angular/core';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzTableQueryParams } from 'ng-zorro-antd/table';
import { TableQuery } from 'src/app/models/events';
import { PlaylistSongPaginationModel } from 'src/app/models/playlist-models';
import { PaginationModel } from 'src/app/models/storage';
import { JwtService } from 'src/app/services/jwt.service';
import { PlaylistService } from 'src/app/services/playlist.service';
import { RxjsStorageService } from 'src/app/services/rxjs-storage.service';
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
    private sessionStorage: SessionStorageService, private jwtService: JwtService,
    private rxjsStorageService: RxjsStorageService) {
    
      this.userName = this.jwtService.getUserName();

      let savedPagination = this.sessionStorage.GetLastPaginationOfFavorites();
  
      if (savedPagination) {
        // Save pagination from session storage in rxjs storage
        // This is done 
        this.rxjsStorageService.setCurrentPaginationSongModel(savedPagination);
      }
  
      this.rxjsStorageService.setCurrentPaginationSongModel(this.paginationModel);
  }

  ngOnInit(): void {

  }

  public onGetFavorites(page: number, take: number, sortAfter: string, asc: boolean, query: string): void{
    console.log("Get Favorites")
    
    this.rxjsStorageService.setSongTableLoadingState(true);
    this.playlistService.GetFavorites(page, take, sortAfter, asc, query).subscribe({
      next:(songsModel: PlaylistSongPaginationModel)=>{
        this.songsModel = songsModel;
      },
      error:(error: any)=>{
        this.message.error("Error when getting favorites.");
      },
      complete: () => {
        this.rxjsStorageService.setSongTableLoadingState(false);
      }
    });
  }

  public onPaginationUpdated(){
    console.log("Get Elements")

    this.rxjsStorageService.currentPaginationSongModel$.subscribe((val) => {
      const pModel = val as PaginationModel;
      this.sessionStorage.SaveLastPaginationOfFavorites(pModel);
      this.onGetFavorites(pModel.page, pModel.take, pModel.sortAfter, 
        pModel.asc, pModel.query);

    })
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
