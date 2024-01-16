import { DOCUMENT } from '@angular/common';
import { Component, ElementRef, Inject, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { Subject, firstValueFrom, lastValueFrom, takeUntil } from 'rxjs';
import { APIROUTES } from 'src/app/constants/api-routes';
import { LOOPMODES } from 'src/app/constants/loop-modes';
import { QUEUETYPES } from 'src/app/constants/queue-types';
import { PlaylistSongModel } from 'src/app/models/playlist-models';
import { QueueModel } from 'src/app/models/storage';
import { PlaylistService } from 'src/app/services/playlist.service';
import { QueueService } from 'src/app/services/queue.service';
import { RxjsStorageService } from 'src/app/services/rxjs-storage.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-mediaplayer',
  templateUrl: './mediaplayer.component.html',
  styleUrls: ['./mediaplayer.component.scss']
})
export class MediaplayerComponent implements OnInit, OnDestroy {

  private currentPlayingSong: PlaylistSongModel = {} as PlaylistSongModel;

  private isSongPlaying: boolean = false;

  private queueModel: QueueModel = {} as QueueModel;

  public volumePercent: number = 100;

  public durationSlider: number = 0;

  public loopAudio: boolean = false;

  public loopMode: string = LOOPMODES.none;

  public mutedAudio: boolean = false;

  public randomizePlay: boolean = false;

  private audioElement = new Audio();

  private destroy:Subject<any> = new Subject();

  constructor(private rxjsService: RxjsStorageService, private playlistService: PlaylistService, private queueService: QueueService) {
    this.audioElement.autoplay = false;
    
    this.audioElement.addEventListener("timeupdate", (x) => {
      if (isNaN(this.audioElement.currentTime) ) {
        return;
      }

      this.durationSlider = Math.round(this.audioElement.currentTime);
    });

    this.audioElement.addEventListener("ended", async () => {
      await this.playNextSong();
    });

  }



  async ngOnInit(): Promise<void> {

    this.rxjsService.isSongPlayingState.pipe(takeUntil(this.destroy)).subscribe(x => {
      this.isSongPlaying = x;

      if (this.isSongPlaying) {
        this.audioElement.play();
      }

      if (!this.isSongPlaying) {
        this.audioElement.pause();
      }      
    });
    
    // Get values from storage
    this.rxjsService.currentPlayingSong.pipe(takeUntil(this.destroy)).subscribe(x => {
      let oldPlayingSong = JSON.parse(JSON.stringify(this.currentPlayingSong)) as PlaylistSongModel;
      this.currentPlayingSong = x;

      if (!this.currentPlayingSong.id) {
        console.log("Current playing song is undef")
        return;
      }

      if (this.currentPlayingSong.id == oldPlayingSong.id) {
        console.log("Song is the same. No need to restart playback");
        return;
      }

      this.audioElement.pause();
      console.log("Play new song")
      this.audioElement.src = this.AudioSrc;
      this.audioElement.load();

      if (this.isSongPlaying) {
        this.audioElement.play();
      }
      
    });

    this.rxjsService.updateReplaySongState.pipe(takeUntil(this.destroy)).subscribe(x => {
      this.audioElement.currentTime = 0;
    })

    this.rxjsService.currentQueueFilterAndPagination.pipe(takeUntil(this.destroy)).subscribe(x => {
      this.queueModel = x;
    })

    if (!this.queueModel.random || !this.queueModel.loopMode) {
      console.log("No Loop or random found")
      this.queueModel.random = this.randomizePlay;
      this.queueModel.loopMode = this.loopMode;
      this.rxjsService.setQueueFilterAndPagination(this.queueModel);
    }

    if (this.queueModel.random) {
      this.randomizePlay = this.queueModel.random;
      console.log("Set random")
    }

    if (this.queueModel.loopMode) {
      this.loopMode = this.queueModel.loopMode;
      console.log("Set loop")
    }

  }

  ngOnDestroy(): void {
    this.destroy.next(true);
  }

  public onDurationChanged(value: any){
    // Is number
    this.audioElement.currentTime = value;
  }

  public onVolumeChanged(value: any){
    // Is number
    this.audioElement.volume = this.AudioVolume;
  }

  public muteUnmuteAudio(){
    this.mutedAudio = !this.mutedAudio;
    this.audioElement.muted = this.mutedAudio;
  }

  public removeSongFromFavorites(): void{
    this.playlistService.RemoveSongsFromFavorites([this.currentPlayingSong.id]).subscribe({
      next: ()=>{
        // Show Modal
        let currentSong = JSON.parse(JSON.stringify(this.currentPlayingSong)) as PlaylistSongModel;
        currentSong.isInFavorites = false;
        this.rxjsService.setCurrentPlayingSong(currentSong);
        this.updateDashBoardAndSongTable();
        this.updateQueue();
      }
    });

  }
  

  public addSongToFavorites(): void{
    this.playlistService.AddSongsToFavorites([this.currentPlayingSong.id]).subscribe({
      next: ()=>{
        // Show Modal
        let currentSong = JSON.parse(JSON.stringify(this.currentPlayingSong)) as PlaylistSongModel;
        currentSong.isInFavorites = true;
        this.rxjsService.setCurrentPlayingSong(currentSong);
        this.updateDashBoardAndSongTable();
        this.updateQueue();
      }


    });
  }

  public playPauseSong(): void{
    if (this.isSongPlaying) {
      console.log("Stop playback");
      this.rxjsService.setIsSongPlaylingState(false);
      return;
    }

    console.log("Start playback");
    this.rxjsService.setIsSongPlaylingState(true);
  }

  public async randomizePlayback(): Promise<void>{
    this.randomizePlay = !this.randomizePlay;

    if (!this.queueModel.target) {
      return;
    }

    this.queueModel.random = this.randomizePlay;
    this.rxjsService.setQueueFilterAndPagination(this.queueModel);
    this.queueService.ChangeQueue(this.randomizePlay).subscribe({
      next: (song: PlaylistSongModel) => {
        this.rxjsService.setCurrentPlayingSong(song);
        console.log("Ranomdize queue")
        // Update possible queue view
        this.updateSongTable();
        this.updateQueue();
      },
      error: (error) => {
        console.log(error)
      }
    })
    


  }

  public async playPrevSong(): Promise<void>{
    // If the audio loop then restart song
    if (this.loopAudio) {
      this.audioElement.currentTime = 0;
      return;
    }

    
    // If song is not at 0:0 replay it from beginning otherwise do the thing under here
    if (this.durationSlider > 5) {
      this.onDurationChanged(0);
      return;
    }

    try {
      var lastPlayedSong = await lastValueFrom(this.queueService.SkipBackInQueue());

      this.rxjsService.setCurrentPlayingSong(lastPlayedSong);

      this.updateQueue();
    } catch (error) {
      // If there is no previous song restart current one
      this.audioElement.currentTime = 0;
    }

  }

  public async playNextSong():Promise<void>{

    // If the audio loop then restart song
    if (this.loopAudio) {
      this.audioElement.currentTime = 0;
      return;
    }

    try {
      var nextSong = await lastValueFrom(this.queueService.SkipForwardInQueue(-1));
      this.rxjsService.setCurrentPlayingSong(nextSong);
      this.updateQueue();
    } catch (error) {

      if (this.queueModel.target) {
        switch (this.queueModel.target) {
          case QUEUETYPES.favorites:
            await this.startFavoriteQueueFromStart();
            break;
          case QUEUETYPES.playlist:
            await this.startPlaylistQueueFromStart();
            break;
          case QUEUETYPES.album:
            await this.startAlbumQueueFromStart();
            break;
          case QUEUETYPES.song:
            await this.startSingleSongQueueFromStart();
            break;
          default:
            this.audioElement.currentTime = 0;
            return;
        }

        return;
      }


      // Shouldn't happen as we check it above
      this.audioElement.currentTime = 0;
    }

  }

  public async startFavoriteQueueFromStart(): Promise<void>{
    try {
      var currentSong = await lastValueFrom(this.queueService.CreateQueueFromFavorites(this.randomizePlay,this.queueModel.loopMode, this.queueModel.sortAfter, this.queueModel.asc, -1));
      this.rxjsService.setCurrentPlayingSong(currentSong);
      this.rxjsService.setIsSongPlaylingState(this.loopMode == this.LoopModePlaylist && this.isSongPlaying);
      this.updateQueue();
    } catch (error) {
      console.log(error);
    }

  }

  public async startPlaylistQueueFromStart(): Promise<void>{
    try {
      var currentSong = await lastValueFrom(this.queueService.CreateQueueFromPlaylist(this.queueModel.itemId, this.randomizePlay,this.queueModel.loopMode, this.queueModel.sortAfter, this.queueModel.asc, -1));
      this.rxjsService.setCurrentPlayingSong(currentSong);
      this.rxjsService.setIsSongPlaylingState(this.loopMode == this.LoopModePlaylist&& this.isSongPlaying);
      this.updateQueue();
    } catch (error) {
      console.log(error);
    }
  }

  public async startAlbumQueueFromStart(): Promise<void>{
    try {
      var currentSong = await lastValueFrom(this.queueService.CreateQueueFromAlbum(this.queueModel.itemId, this.randomizePlay,this.queueModel.loopMode, -1));
      this.rxjsService.setCurrentPlayingSong(currentSong);
      this.rxjsService.setIsSongPlaylingState(this.loopMode == this.LoopModePlaylist&& this.isSongPlaying);
      this.updateQueue();
    } catch (error) {
      console.log(error);
    }
  }

  public async startSingleSongQueueFromStart(): Promise<void>{
    try {
      var currentSong = await lastValueFrom(this.queueService.CreateQueueFromSingleSong(this.queueModel.itemId, this.randomizePlay, this.queueModel.loopMode));
      this.rxjsService.setCurrentPlayingSong(currentSong);
      this.rxjsService.setIsSongPlaylingState(this.loopMode == this.LoopModePlaylist&& this.isSongPlaying);
      this.updateQueue();
    } catch (error) {
      console.log(error);
    }
  }

  public async loopPlayback(): Promise<void>{

    // Check the current loop mode
    switch (this.loopMode) {
      case this.LoopModeNone:
        this.loopMode = this.LoopModePlaylist;
        this.loopAudio = false;
        break;
      case this.LoopModeAudio:
        this.loopMode = this.LoopModeNone;
        this.loopAudio = false;
        break;
      case this.LoopModePlaylist:
        this.loopMode = this.LoopModeAudio;
        this.loopAudio = true;
        break;
      default:
        break;
    }

    
    this.audioElement.loop = this.loopAudio;
    this.queueModel.loopMode = this.loopMode;
    this.rxjsService.setQueueFilterAndPagination(this.queueModel);

    try {
      // Update the quemodel in the db
      var res = await lastValueFrom(this.queueService.UpdateQueueData(this.queueModel.itemId, this.queueModel.asc, this.queueModel.random, this.queueModel.target, this.queueModel.loopMode, this.queueModel.sortAfter));
    } catch (error) {
      console.log(error);
    }
  }

  public updateDashBoardAndSongTable(): void{
    let dashboardBool = false;
    let tableBool = false;

    this.rxjsService.updateDashboardBoolean$.subscribe(x => {
      dashboardBool = x;
    });

    this.rxjsService.updateCurrentTableBoolean$.subscribe(x => {
      tableBool = x;
    });

    this.rxjsService.setUpdateDashboardBoolean(!dashboardBool);
    this.rxjsService.setUpdateCurrentTableBoolean(!tableBool);
  }

  public updateSongTable(): void{
    let tableBool = false;
    this.rxjsService.updateCurrentTableBoolean$.subscribe(x => {
      tableBool = x;
    });

    this.rxjsService.setUpdateCurrentTableBoolean(!tableBool);
  }

  public updateQueue(): void{
    let queueBool = false;
    this.rxjsService.updateQueueBoolean$.subscribe(x => {
      queueBool = x;
    });

    this.rxjsService.setUpdateQueueBoolean(!queueBool);
  }

  get SongUrl(): string{
    if (!this.currentPlayingSong.id) {
      return ``;
    }

    return `/song/${this.currentPlayingSong.id}`;
  }

  getArtistUrl(id: string): string{
    return `/artist/${id}`;
  }

  get Duration(): number{
    if (!this.currentPlayingSong.duration) {
      return 100;
    }

    return this.currentPlayingSong.duration;
  }

  get AudioSrc(): string{
    if (!this.currentPlayingSong.id) {
      return ``;
    }

    return `${environment.apiUrl}/${APIROUTES.file}/song/${this.currentPlayingSong.id}`;
  }

  get AlbumCoverSrc(): string{
    if (!this.CurrentPlayingSong.album) {
      return ``
    }

    return `${environment.apiUrl}/${APIROUTES.file}/album/${this.CurrentPlayingSong.album.id}`;
  }

  get CurrentPlayingSong(): PlaylistSongModel{
    return this.currentPlayingSong;
  }

  get IsSongPlaying(): boolean{
    return this.isSongPlaying;
  }

  get QueueModel(): QueueModel{
    return this.queueModel;
  }

  get AudioVolume(): number{
    return this.volumePercent / 100;
  }

  get LoopModeNone(): string{
    return LOOPMODES.none;
  }

  get LoopModeAudio(): string{
    return LOOPMODES.audio;
  }

  get LoopModePlaylist(): string{
    return LOOPMODES.playlist;
  }

  get CurrentLoopMode(): string{
    return this.loopMode;
  }
}
