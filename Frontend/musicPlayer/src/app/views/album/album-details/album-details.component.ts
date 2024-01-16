import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NzMessageService } from 'ng-zorro-antd/message';
import { APIROUTES } from 'src/app/constants/api-routes';
import { LOOPMODES } from 'src/app/constants/loop-modes';
import { QUEUETYPES } from 'src/app/constants/queue-types';
import { AlbumModel } from 'src/app/models/artist-models';
import { PlaylistSongModelParams } from 'src/app/models/events';
import { PlaylistSongModel, SongPaginationModel, PlaylistUserShortModel } from 'src/app/models/playlist-models';
import { PaginationModel, QueueModel } from 'src/app/models/storage';
import { QueueService } from 'src/app/services/queue.service';
import { RxjsStorageService } from 'src/app/services/rxjs-storage.service';
import { SessionStorageService } from 'src/app/services/session-storage.service';
import { SongService } from 'src/app/services/song.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-album-details',
  templateUrl: './album-details.component.html',
  styleUrls: ['./album-details.component.scss']
})
export class AlbumDetailsComponent implements OnInit {

  private albumId: string = '';

  private songsModel: SongPaginationModel = {} as SongPaginationModel;

  private artistName: string = '';

  private paginationModel: PaginationModel = {
    asc: true,
    page : 1,
    query : '',
    sortAfter : '',
    take: 10
  } as PaginationModel;

  private albumModel: AlbumModel = {

  } as AlbumModel;

  private isSongPlaying: boolean = false;

  private queueModel: QueueModel = undefined as any;

  private currentPlayingSong: PlaylistSongModel = undefined as any;

  /**
   *
   */
  constructor(private route: ActivatedRoute, private rxjsService: RxjsStorageService, private songService: SongService,
    private message: NzMessageService, 
    private sessionStorage: SessionStorageService,
    private queueService: QueueService) {

    this.rxjsService.setCurrentPaginationSongModel(this.paginationModel);
    
    if (!this.route.snapshot.paramMap.has('albumId')) {
      console.log("Playlist id not found");
      return;
    }

    this.albumId = this.route.snapshot.paramMap.get('albumId') as string;
    this.songService.GetAlbumDetails(this.albumId).subscribe({
      next:(albumModel: AlbumModel)=>{
        this.albumModel = albumModel;
        console.log(new Date(this.albumModel.release))
      },
      error:(error: any)=>{
        this.message.error("Error when getting album.");
      }
    })
  }

  ngOnInit(): void {
    this.rxjsService.isSongPlayingState.subscribe(x => {
      this.isSongPlaying = x;
    });

    this.rxjsService.currentQueueFilterAndPagination.subscribe(x => {
      this.queueModel = x;
    });

    this.rxjsService.currentPlayingSong.subscribe(x => {
      this.currentPlayingSong = x;
    });

    this.rxjsService.updateCurrentTableBoolean$.subscribe(x => {
      this.onPaginationUpdated();
    });
  }

  public onGetSongs(page: number, take: number, sortAfter: string, asc: boolean, query: string): void{
    console.log("Get Songs")
    const skipSongs = (page - 1) * take;
    this.rxjsService.setSongTableLoadingState(true);
    this.songService.GetAlbumSongs(this.albumId, skipSongs, take).subscribe({
      next:(songsModel: SongPaginationModel)=>{
        for (let index = 0; index < songsModel.songs.length; index++) {
          songsModel.songs[index].checked = false;
          songsModel.songs[index].order = index;
          
        }

        this.songsModel = songsModel;
      },
      error:(error: any)=>{
        this.message.error("Error when getting songs from album.");
      },
      complete: () => {
        this.rxjsService.setSongTableLoadingState(false);
      }
    });
  }

  public onPaginationUpdated(): void{
    console.log("Get Elements")

    let pModel = {} as PaginationModel;

    this.rxjsService.currentPaginationSongModel$.subscribe((val) => {
      pModel = val as PaginationModel;
    })

    this.sessionStorage.SaveLastPaginationOfPlaylist(pModel);
    this.onGetSongs(pModel.page, pModel.take, pModel.sortAfter, 
      pModel.asc, pModel.query);
  }

  public playSongs(): void{
    console.log("Play songs")

    // If the user previously clicked stop and wants to resume the playlist with the same queue
    if (this.QueueModel &&
      this.QueueModel.target == QUEUETYPES.album && 
      this.QueueModel.itemId == this.albumId) {
      this.rxjsService.setIsSongPlaylingState(true);
      return;
    }
    this.rxjsService.setQueueFilterAndPagination({
      asc : this.paginationModel.asc,
      page : 0,
      take : 0,
      query : '',
      sortAfter : this.paginationModel.sortAfter == '' ? 'name' : this.paginationModel.sortAfter,
      itemId : this.albumId,
      // TOOD: Replace with interface
      target : QUEUETYPES.album,
      loopMode: this.queueModel.loopMode== undefined ? LOOPMODES.none: this.queueModel.loopMode,
      random: this.queueModel.random == undefined ? false: this.queueModel.random
    });

    this.queueService.CreateQueueFromAlbum(this.albumId, this.queueModel.random, this.queueModel.loopMode,-1).subscribe({
      next:async (song: PlaylistSongModel)=>{
        console.log(song)
        
        this.rxjsService.setCurrentPlayingSong(song);
        await this.rxjsService.setUpdateSongState();
        this.rxjsService.setIsSongPlaylingState(true);
        this.rxjsService.showMediaPlayer(true);
      },
      error:(error: any)=>{
        this.message.error("Error when getting queue.");
      },
      complete: () => {
      }
    });
  }

  public pauseSongs(): void {
    // Stop playing of song
    this.rxjsService.setIsSongPlaylingState(false);
  }

  public onPlaySongClicked(event: PlaylistSongModelParams): void{
    console.log(event);
    const indexOfSong = event.index;
    const songModel = event.songModel;

    if (indexOfSong < 0) {
      return;
    }

    // IF the user wants to resume the same song
    if (this.CurrentPlayingSong && 
      this.CurrentPlayingSong.id == songModel.id) {
      this.rxjsService.setIsSongPlaylingState(true);
      return;
    }

    const skipSongs = ((this.paginationModel.page - 1) * this.paginationModel.take) + indexOfSong;

    this.rxjsService.setQueueFilterAndPagination({
      asc : this.paginationModel.asc,
      page : 0,
      take : 0,
      query : '',
      sortAfter : this.paginationModel.sortAfter == '' ? 'name' : this.paginationModel.sortAfter,
      itemId : this.albumId,
      target : QUEUETYPES.album,
      loopMode: this.queueModel.loopMode== undefined ? LOOPMODES.none: this.queueModel.loopMode,
      random: this.queueModel.random == undefined ? false: this.queueModel.random
    });

    this.queueService.CreateQueueFromAlbum(this.albumId, this.queueModel.random,this.queueModel.loopMode, skipSongs).subscribe({
      next:async (song: PlaylistSongModel)=>{
        console.log(song)
        this.rxjsService.setCurrentPlayingSong(song);
        await this.rxjsService.setUpdateSongState();
        this.rxjsService.setIsSongPlaylingState(true);
        this.rxjsService.showMediaPlayer(true);
      },
      error:(error: any)=>{
        this.message.error("Error when getting queue.");
      },
      complete: () => {
      }
    });
  }

  public getAlbumCoverSrc(): string{
    return `${environment.apiUrl}/${APIROUTES.file}/album/${this.albumId}`
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

  public get AlbumModel(): AlbumModel{
    return this.albumModel;
  }

  public get ReleaseDate(): Date{
    return new Date(this.AlbumModel.release);
  }
}
