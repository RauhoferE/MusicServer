import { Component, OnInit } from '@angular/core';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzTableQueryParams } from 'ng-zorro-antd/table';
import { TableQuery } from 'src/app/models/events';
import { PlaylistSongModel, PlaylistSongPaginationModel } from 'src/app/models/playlist-models';
import { PaginationModel, QueueModel } from 'src/app/models/storage';
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

  private isSongPlaying: boolean = false;

  private queueModel: QueueModel = undefined as any;

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
    let isSongPlaying = false;
    let queueModel = undefined as any;

    this.rxjsStorageService.isSongPlayingState.subscribe(x => {
      isSongPlaying = x;
    });

    this.rxjsStorageService.currentQueueFilterAndPagination.subscribe(x => {
      queueModel = x;
    });

    this.isSongPlaying = isSongPlaying;
    this.queueModel = queueModel;
  }

  public onGetFavorites(page: number, take: number, sortAfter: string, asc: boolean, query: string): void{
    console.log("Get Favorites")
    
    this.rxjsStorageService.setSongTableLoadingState(true);
    this.playlistService.GetFavorites(page, take, sortAfter, asc, query).subscribe({
      next:(songsModel: PlaylistSongPaginationModel)=>{
        songsModel.songs.forEach(element => {
          element.checked = false;
        });
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

    let pModel = {} as PaginationModel;

    this.rxjsStorageService.currentPaginationSongModel$.subscribe((val) => {
      pModel = val as PaginationModel;
    })

    this.sessionStorage.SaveLastPaginationOfFavorites(pModel);
    this.onGetFavorites(pModel.page, pModel.take, pModel.sortAfter, 
      pModel.asc, pModel.query);
  }

  public playSongs(): void{
    console.log("Play songs")

  }

  public pauseSongs() {
    throw new Error('Method not implemented.');
  }

  public onPlaySongClicked(songModel: PlaylistSongModel): void{
    console.log(songModel);

  }

  public getUserHref(): string{
    return `/user/-1`;
  }

  public get IsSongPlaying(): boolean{
    return this.isSongPlaying;
  }

  public get QueueModel(): QueueModel{
    return this.queueModel;
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
