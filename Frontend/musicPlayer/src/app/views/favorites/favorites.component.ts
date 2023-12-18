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

  private currentPlayingSong: PlaylistSongModel = undefined as any;

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
    let currentPlayingSong = undefined as any;

    this.rxjsStorageService.isSongPlayingState.subscribe(x => {
      this.isSongPlaying = x;
    });

    this.rxjsStorageService.currentQueueFilterAndPagination.subscribe(x => {
      this.queueModel = x;
    });

    this.rxjsStorageService.currentPlayingSong.subscribe(x => {
      this.currentPlayingSong = x;
    });

    this.rxjsStorageService.updateCurrentTableBoolean$.subscribe(x => {
      this.onPaginationUpdated();
    });

    // this.isSongPlaying = isSongPlaying;
    // this.queueModel = queueModel;
    // this.currentPlayingSong = currentPlayingSong;
  }

  public onGetFavorites(page: number, take: number, sortAfter: string, asc: boolean, query: string): void{
    console.log("Get Favorites")
    const skipSongs = (page - 1) * take;
    this.rxjsStorageService.setSongTableLoadingState(true);
    this.playlistService.GetFavorites(skipSongs, take, sortAfter, asc, query).subscribe({
      next:(songsModel: PlaylistSongPaginationModel)=>{
        songsModel.songs.forEach(element => {
          element.checked = false;
        });
        this.songsModel = songsModel;
        // this.rxjsStorageService.replaceSongsInQueue(this.songsModel.songs);
        // this.rxjsStorageService.checkFoReplaceingCurrentPlayingSong(this.songsModel.songs);
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

    // If the user previously clicked stop and wants to resume the playlist with the same queue
    if (this.QueueModel &&
      this.QueueModel.type == 'favorites' && 
    this.QueueModel.asc == this.paginationModel.asc && 
    this.QueueModel.query == this.paginationModel.query &&
    this.QueueModel.sortAfter == this.paginationModel.sortAfter) {
      this.rxjsStorageService.setIsSongPlaylingState(true);
      return;
    }
    this.rxjsStorageService.setQueueFilterAndPagination({
      asc : this.paginationModel.asc,
      page : 0,
      take : 31,
      query : this.paginationModel.query,
      sortAfter : this.paginationModel.sortAfter,
      itemGuid : '-1',
      type : 'favorites'
    });

    this.playlistService.GetFavorites(0, 31, this.paginationModel.sortAfter, this.paginationModel.asc, this.paginationModel.query).subscribe({
      next:(songsModel: PlaylistSongPaginationModel)=>{
        console.log(songsModel.songs)
        
        this.rxjsStorageService.setCurrentPlayingSong(songsModel.songs.splice(0,1)[0]);
        this.rxjsStorageService.setSongQueue(songsModel.songs);
        this.rxjsStorageService.setIsSongPlaylingState(true);
        this.rxjsStorageService.showMediaPlayer(true);
        console.log(songsModel.songs)
      },
      error:(error: any)=>{
        this.message.error("Error when getting queue.");
      },
      complete: () => {
      }
    });
  }

  public pauseSongs() {
    // Stop playing of song
    this.rxjsStorageService.setIsSongPlaylingState(false);
  }

  public onPlaySongClicked(songModel: PlaylistSongModel): void{
    console.log(songModel);
    const indexOfSong = this.songsModel.songs.findIndex(x => x.id == songModel.id);

    if (indexOfSong < 0) {
      return;
    }

    // IF the user wants to resume the same song
    if (this.CurrentPlayingSong && 
      this.CurrentPlayingSong.id == songModel.id) {
      this.rxjsStorageService.setIsSongPlaylingState(true);
      return;
    }

    const skipSongs = ((this.paginationModel.page - 1) * this.paginationModel.take) + indexOfSong;

    this.rxjsStorageService.setQueueFilterAndPagination({
      asc : this.paginationModel.asc,
      page : skipSongs,
      take : 31,
      query : this.paginationModel.query,
      sortAfter : this.paginationModel.sortAfter,
      itemGuid : '-1',
      type : 'favorites'
    });

    this.playlistService.GetFavorites(skipSongs, 31, this.paginationModel.sortAfter, this.paginationModel.asc, '').subscribe({
      next:(songsModel: PlaylistSongPaginationModel)=>{
        console.log(songsModel)
        this.rxjsStorageService.setCurrentPlayingSong(songsModel.songs.splice(0,1)[0]);
        this.rxjsStorageService.setSongQueue(songsModel.songs);
        this.rxjsStorageService.setIsSongPlaylingState(true);
        this.rxjsStorageService.showMediaPlayer(true);
        console.log(songsModel.songs)
      },
      error:(error: any)=>{
        this.message.error("Error when getting queue.");
      },
      complete: () => {
      }
    });
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

  public get CurrentPlayingSong(): PlaylistSongModel{
    return this.currentPlayingSong;
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
