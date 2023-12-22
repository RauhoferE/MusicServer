import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NzMessageService } from 'ng-zorro-antd/message';
import { APIROUTES } from 'src/app/constants/api-routes';
import { AlbumModel } from 'src/app/models/artist-models';
import { PlaylistSongModelParams } from 'src/app/models/events';
import { PlaylistSongModel, SongPaginationModel, PlaylistUserShortModel } from 'src/app/models/playlist-models';
import { PaginationModel, QueueModel } from 'src/app/models/storage';
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
    private sessionStorage: SessionStorageService) {

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
        songsModel.songs.forEach(element => {
          element.checked = false;
        });
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

  public onPaginationUpdated(){
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
      this.QueueModel.type == 'album' && 
    this.QueueModel.asc == this.paginationModel.asc && 
    this.QueueModel.query == this.paginationModel.query &&
    this.QueueModel.sortAfter == this.paginationModel.sortAfter) {
      this.rxjsService.setIsSongPlaylingState(true);
      return;
    }
    this.rxjsService.setQueueFilterAndPagination({
      asc : this.paginationModel.asc,
      page : 0,
      take : 31,
      query : this.paginationModel.query,
      sortAfter : this.paginationModel.sortAfter,
      itemGuid : this.albumId,
      // TOOD: Replace with interface
      type : 'album'
    });

    this.songService.GetAlbumSongs(this.albumId, 0, 31).subscribe({
      next:(songsModel: SongPaginationModel)=>{
        console.log(songsModel.songs)
        
        this.rxjsService.setCurrentPlayingSong(songsModel.songs.splice(0,1)[0]);
        this.rxjsService.setSongQueue(songsModel.songs);
        this.rxjsService.setIsSongPlaylingState(true);
        this.rxjsService.showMediaPlayer(true);
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
      page : skipSongs,
      take : 31,
      query : this.paginationModel.query,
      sortAfter : this.paginationModel.sortAfter,
      itemGuid : this.albumId,
      type : 'album'
    });

    this.songService.GetAlbumSongs(this.albumId, skipSongs, 31).subscribe({
      next:(songsModel: SongPaginationModel)=>{
        console.log(songsModel)
        this.rxjsService.setCurrentPlayingSong(songsModel.songs.splice(0,1)[0]);
        this.rxjsService.setSongQueue(songsModel.songs);
        this.rxjsService.setIsSongPlaylingState(true);
        this.rxjsService.showMediaPlayer(true);
        console.log(songsModel.songs)
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
