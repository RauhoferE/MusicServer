import { Component, OnInit } from '@angular/core';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzTableQueryParams } from 'ng-zorro-antd/table';
import { firstValueFrom, lastValueFrom } from 'rxjs';
import { LOOPMODES } from 'src/app/constants/loop-modes';
import { QUEUETYPES } from 'src/app/constants/queue-types';
import { DragDropSongParams, PlaylistSongModelParams, TableQuery } from 'src/app/models/events';
import { PlaylistSongModel, SongPaginationModel } from 'src/app/models/playlist-models';
import { PaginationModel, QueueModel } from 'src/app/models/storage';
import { JwtService } from 'src/app/services/jwt.service';
import { PlaylistService } from 'src/app/services/playlist.service';
import { QueueService } from 'src/app/services/queue.service';
import { RxjsStorageService } from 'src/app/services/rxjs-storage.service';
import { SessionStorageService } from 'src/app/services/session-storage.service';

@Component({
  selector: 'app-favorites',
  templateUrl: './favorites.component.html',
  styleUrls: ['./favorites.component.scss']
})
export class FavoritesComponent implements OnInit{

  private songsModel: SongPaginationModel = {} as SongPaginationModel;

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
  constructor(private playlistService: PlaylistService,
    private queueService: QueueService,
     private message: NzMessageService, 
    private sessionStorage: SessionStorageService, private jwtService: JwtService,
    private rxjsStorageService: RxjsStorageService) {
    
      this.userName = this.jwtService.getUserName();

      let savedPagination = this.sessionStorage.GetLastPaginationOfFavorites();
  
      if (savedPagination) {
        // Save pagination from session storage in rxjs storage
        // This is done 
        //this.rxjsStorageService.setCurrentPaginationSongModel(savedPagination);
        this.paginationModel = savedPagination;
      }
  
      this.rxjsStorageService.setCurrentPaginationSongModel(this.paginationModel);
  }

  ngOnInit(): void {
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
  }

  public onGetFavorites(page: number, take: number, sortAfter: string, asc: boolean, query: string): void{
    console.log("Get Favorites")
    const skipSongs = (page - 1) * take;
    this.rxjsStorageService.setSongTableLoadingState(true);
    this.playlistService.GetFavorites(skipSongs, take, sortAfter, asc, query).subscribe({
      next:(songsModel: SongPaginationModel)=>{
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

  public async playSongs(): Promise<void>{
    console.log("Play songs")

    console.log(this.queueModel)
    // If the user previously clicked stop and wants to resume the playlist with the same queue
    if (this.QueueModel &&
      this.QueueModel.target == QUEUETYPES.favorites) {
      this.rxjsStorageService.setIsSongPlaylingState(true);
      return;
    }
    const paginationModel = await this.getCurrentPaginationModel();

    this.rxjsStorageService.setQueueFilterAndPagination({
      asc : paginationModel.asc,
      page : 0,
      take : 0,
      query : '',
      sortAfter : this.paginationModel.sortAfter == '' ? 'name' : this.paginationModel.sortAfter,
      itemId : '-1',
      target : QUEUETYPES.favorites,
      loopMode: this.queueModel.loopMode== undefined ? LOOPMODES.none: this.queueModel.loopMode,
      random: this.queueModel.random == undefined ? false: this.queueModel.random
    });

    this.queueService.CreateQueueFromFavorites(this.queueModel.random, this.queueModel.loopMode, paginationModel.sortAfter, paginationModel.asc, -1).subscribe({
      next:(songs: PlaylistSongModel[])=>{
        this.rxjsStorageService.setCurrentPlayingSong(songs.splice(0,1)[0]);
        this.rxjsStorageService.setIsSongPlaylingState(true);
        this.rxjsStorageService.showMediaPlayer(true);
        console.log(songs)
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

  public async onPlaySongClicked(event: PlaylistSongModelParams): Promise<void>{
    console.log(event);
    const indexOfSong = event.index;
    const songModel = event.songModel;

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

    const paginationModel = await this.getCurrentPaginationModel();

    this.rxjsStorageService.setQueueFilterAndPagination({
      asc : paginationModel.asc,
      page : 0,
      take : 0,
      query : '',
      sortAfter : this.paginationModel.sortAfter == '' ? 'name' : this.paginationModel.sortAfter,
      itemId : '-1',
      target : QUEUETYPES.favorites,
      loopMode: this.queueModel.loopMode== undefined ? LOOPMODES.none: this.queueModel.loopMode,
      random: this.queueModel.random == undefined ? false: this.queueModel.random
    });

    this.queueService.CreateQueueFromFavorites(this.queueModel.random, this.queueModel.loopMode, paginationModel.sortAfter, paginationModel.asc, event.songModel.order).subscribe({
      next:(songs: PlaylistSongModel[])=>{
        console.log(songs)
        this.rxjsStorageService.setCurrentPlayingSong(songs.splice(0,1)[0]);
        this.rxjsStorageService.setIsSongPlaylingState(true);
        this.rxjsStorageService.showMediaPlayer(true);
      },
      error:(error: any)=>{
        this.message.error("Error when getting queue.");
      },
      complete: () => {
      }
    });
  }

  changeSongPosition(event: DragDropSongParams) {
    this.playlistService.ChangeOrderOfSongInFavorites(event.srcIndex, event.destIndex).subscribe({
      next:()=>{
        this.onPaginationUpdated();
      },
      error:(error: any)=>{
        this.message.error("Error when changing order of songs.");
      },
      complete: () => {
      }
    });
  }

  private async getCurrentPaginationModel(): Promise<PaginationModel>{
    let pModel = {} as PaginationModel;

    try {
      pModel = await firstValueFrom(this.rxjsStorageService.currentPaginationSongModel$);
    } catch (error) {
      
    }

    return pModel;
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

  public get SongsModel(): SongPaginationModel{
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
