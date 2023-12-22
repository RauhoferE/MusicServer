import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NzMessageService } from 'ng-zorro-antd/message';
import { APIROUTES } from 'src/app/constants/api-routes';
import { DragDropSongParams, PlaylistSongModelParams } from 'src/app/models/events';
import { PlaylistSongModel, SongPaginationModel, PlaylistUserShortModel } from 'src/app/models/playlist-models';
import { PaginationModel, QueueModel } from 'src/app/models/storage';
import { JwtService } from 'src/app/services/jwt.service';
import { PlaylistService } from 'src/app/services/playlist.service';
import { RxjsStorageService } from 'src/app/services/rxjs-storage.service';
import { SessionStorageService } from 'src/app/services/session-storage.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-playlist-details',
  templateUrl: './playlist-details.component.html',
  styleUrls: ['./playlist-details.component.scss']
})
export class PlaylistDetailsComponent implements OnInit {

  private playlistId: string = '';

  private songsModel: SongPaginationModel = {} as SongPaginationModel;

  private userName: string = '';

  private paginationModel: PaginationModel = {
    asc: true,
    page : 1,
    query : '',
    sortAfter : '',
    take: 10
  } as PaginationModel;

  private playlistModel: PlaylistUserShortModel = {

  } as PlaylistUserShortModel;

  private isSongPlaying: boolean = false;

  private queueModel: QueueModel = undefined as any;

  private currentPlayingSong: PlaylistSongModel = undefined as any;

  /**
   *
   */
  constructor(private route: ActivatedRoute, 
    private playlistService: PlaylistService, private message: NzMessageService, 
    private sessionStorage: SessionStorageService, private jwtService: JwtService,
    private rxjsStorageService: RxjsStorageService) {
    let savedPagination = this.sessionStorage.GetLastPaginationOfPlaylist();
  
    if (savedPagination) {
      // Save pagination from session storage in rxjs storage
      // This is done 
      this.rxjsStorageService.setCurrentPaginationSongModel(savedPagination);
    }

    this.rxjsStorageService.setCurrentPaginationSongModel(this.paginationModel);

    if (!this.route.snapshot.paramMap.has('playlistId')) {
      console.log("Playlist id not found");
      return;
    }

    this.playlistId = this.route.snapshot.paramMap.get('playlistId') as string;
    this.playlistService.GetPlaylistInfo(this.playlistId).subscribe({
      next:(playlistModel: PlaylistUserShortModel)=>{
        this.playlistModel = playlistModel;
      },
      error:(error: any)=>{
        this.message.error("Error when getting playlist.");
      }
    })
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

  public onGetSongs(page: number, take: number, sortAfter: string, asc: boolean, query: string): void{
    console.log("Get Songs")
    const skipSongs = (page - 1) * take;
    this.rxjsStorageService.setSongTableLoadingState(true);
    this.playlistService.GetSongsFromPlaylist(skipSongs, take, sortAfter, asc, query, this.playlistId).subscribe({
      next:(songsModel: SongPaginationModel)=>{
        songsModel.songs.forEach(element => {
          element.checked = false;
        });
        this.songsModel = songsModel;
      },
      error:(error: any)=>{
        this.message.error("Error when getting songs from playlist.");
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

    this.sessionStorage.SaveLastPaginationOfPlaylist(pModel);
    this.onGetSongs(pModel.page, pModel.take, pModel.sortAfter, 
      pModel.asc, pModel.query);
  }

  public playSongs(): void{
    console.log("Play songs")

    // If the user previously clicked stop and wants to resume the playlist with the same queue
    if (this.QueueModel &&
      this.QueueModel.type == 'playlist' && 
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
      itemGuid : this.playlistId,
      // TOOD: Replace with interface
      type : 'playlist'
    });

    this.playlistService.GetSongsFromPlaylist(0, 31, this.paginationModel.sortAfter, this.paginationModel.asc, '', this.playlistId).subscribe({
      next:(songsModel: SongPaginationModel)=>{
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
      itemGuid : this.playlistId,
      type : 'playlist'
    });

    this.playlistService.GetSongsFromPlaylist(skipSongs, 31, this.paginationModel.sortAfter, this.paginationModel.asc, this.paginationModel.query, this.playlistId).subscribe({
      next:(songsModel: SongPaginationModel)=>{
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

  changeSongPosition(event: DragDropSongParams) {
    // TODO: Change so the query doesnt matter
    this.playlistService.ChangeOrderOfSongInPlaylist(this.playlistId, event.srcIndex, event.destIndex).subscribe({
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

  public getCreatorName(): string{
    if (this.playlistModel.users == undefined) {
      return "";
    }

    return this.playlistModel.users.filter(x => x.isCreator)[0]!.userName;
  }

  public getUserHref(): string{
    if (this.playlistModel.users == undefined) {
      return "";
    }

    const creatorId = this.playlistModel.users.filter(x => x.isCreator)[0]!.id;
    return `/user/${creatorId}`;
  }

  public getPlaylistCoverSrc(): string{
    return `${environment.apiUrl}/${APIROUTES.file}/playlist/${this.playlistId}`
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

  public get PlaylistModel(): PlaylistUserShortModel{
    return this.playlistModel;
  }



}
