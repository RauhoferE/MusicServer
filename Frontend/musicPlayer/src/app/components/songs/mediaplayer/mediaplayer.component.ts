import { DOCUMENT } from '@angular/common';
import { Component, ElementRef, Inject, OnInit, ViewChild } from '@angular/core';
import { lastValueFrom } from 'rxjs';
import { APIROUTES } from 'src/app/constants/api-routes';
import { LoopMode } from 'src/app/constants/loop-modes';
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
export class MediaplayerComponent implements OnInit {

  private currentPlayingSong: PlaylistSongModel = {} as PlaylistSongModel;

  private isSongPlaying: boolean = false;

  private queueModel: QueueModel = {} as QueueModel;

  public volumePercent: number = 100;

  public durationSlider: number = 0;

  public loopAudio: boolean = false;

  public loopMode: string = LoopMode.none;

  public mutedAudio: boolean = false;

  public randomizePlay: boolean = false;

  private audioElement = new Audio();

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

  ngOnInit(): void {

    this.rxjsService.isSongPlayingState.subscribe(x => {
      this.isSongPlaying = x;

      if (this.isSongPlaying) {
        this.audioElement.play();
      }

      if (!this.isSongPlaying) {
        this.audioElement.pause();
      }      
    });
    
    // Get values from storage
    this.rxjsService.currentPlayingSong.subscribe(x => {
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

    this.rxjsService.currentQueueFilterAndPagination.subscribe(x => {
      this.queueModel = x;
    });

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

    if (!this.queueModel.type) {
      return;
    }

    if (this.queueModel.type == QUEUETYPES.favorites && this.randomizePlay) {
      // Randomize favorite queue
      this.queueService.RandomizeQueueFromFavorites().subscribe({
        next:(songs: PlaylistSongModel[])=>{
          console.log(songs)
          this.rxjsService.setCurrentPlayingSong(songs.splice(0,1)[0]);
          // Update possible queue view
          this.updateSongTable();
        },
        error:(error: any)=>{
          //this.message.error("Error when getting queue.");
        },
        complete: () => {
        }
      });
    }

    if (this.queueModel.type == QUEUETYPES.favorites && !this.randomizePlay) {
      // Randomize favorite queue
      await this.startFavoriteQueueFromStart();
    }

    if (this.queueModel.type == QUEUETYPES.playlist && this.randomizePlay) {
      // Randomize playlist queue
      this.queueService.RandomizeQueueFromPlaylist(this.queueModel.itemGuid).subscribe({
        next:(songs: PlaylistSongModel[])=>{
          console.log(songs)
          this.rxjsService.setCurrentPlayingSong(songs.splice(0,1)[0]);
          // Update possible queue view
          this.updateSongTable();
        },
        error:(error: any)=>{
          //this.message.error("Error when getting queue.");
        },
        complete: () => {
        }
      });
    }

    if (this.queueModel.type == QUEUETYPES.playlist && !this.randomizePlay) {
      // Randomize favorite queue
      await this.startPlaylistQueueFromStart();
    }

    if (this.queueModel.type == QUEUETYPES.album && this.randomizePlay) {
      // Randomize playlist queue
      this.queueService.RandomizeQueueFromAlbum(this.queueModel.itemGuid).subscribe({
        next:(songs: PlaylistSongModel[])=>{
          console.log(songs)
          this.rxjsService.setCurrentPlayingSong(songs.splice(0,1)[0]);
          // Update possible queue view
          this.updateSongTable();
        },
        error:(error: any)=>{
          //this.message.error("Error when getting queue.");
        },
        complete: () => {
        }
      });
    }

    if (this.queueModel.type == QUEUETYPES.album && !this.randomizePlay) {
      // Randomize favorite queue
      await this.startAlbumQueueFromStart();
    }

    if (this.queueModel.type == QUEUETYPES.song && this.randomizePlay) {
      // Randomize playlist queue
      this.queueService.RandomizeQueueFromSingleSong(this.queueModel.itemGuid).subscribe({
        next:(songs: PlaylistSongModel[])=>{
          console.log(songs)
          this.rxjsService.setCurrentPlayingSong(songs.splice(0,1)[0]);
          // Update possible queue view
          this.updateSongTable();
        },
        error:(error: any)=>{
          //this.message.error("Error when getting queue.");
        },
        complete: () => {
        }
      });
    }

    if (this.queueModel.type == QUEUETYPES.song && !this.randomizePlay) {
      // Randomize favorite queue
      await this.startSingleSongQueueFromStart();
    }


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

      if (this.queueModel.type) {
        switch (this.queueModel.type) {
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
      var queue = await lastValueFrom(this.queueService.CreateQueueFromFavorites(this.randomizePlay, this.queueModel.sortAfter, this.queueModel.asc, -1));
      this.rxjsService.setCurrentPlayingSong(queue.splice(0,1)[0]);
      this.rxjsService.setIsSongPlaylingState(this.loopMode == this.LoopModePlaylist && this.isSongPlaying);
      this.updateQueue();
    } catch (error) {
      console.log(error);
    }

  }

  public async startPlaylistQueueFromStart(): Promise<void>{
    try {
      var queue = await lastValueFrom(this.queueService.CreateQueueFromPlaylist(this.queueModel.itemGuid, this.randomizePlay, this.queueModel.sortAfter, this.queueModel.asc, -1));
      this.rxjsService.setCurrentPlayingSong(queue.splice(0,1)[0]);
      this.rxjsService.setIsSongPlaylingState(this.loopMode == this.LoopModePlaylist&& this.isSongPlaying);
      this.updateQueue();
    } catch (error) {
      console.log(error);
    }
  }

  public async startAlbumQueueFromStart(): Promise<void>{
    try {
      var queue = await lastValueFrom(this.queueService.CreateQueueFromAlbum(this.queueModel.itemGuid, this.randomizePlay, -1));
      this.rxjsService.setCurrentPlayingSong(queue.splice(0,1)[0]);
      this.rxjsService.setIsSongPlaylingState(this.loopMode == this.LoopModePlaylist&& this.isSongPlaying);
      this.updateQueue();
    } catch (error) {
      console.log(error);
    }
  }

  public async startSingleSongQueueFromStart(): Promise<void>{
    try {
      var queue = await lastValueFrom(this.queueService.CreateQueueFromSingleSong(this.queueModel.itemGuid, this.randomizePlay));
      this.rxjsService.setCurrentPlayingSong(queue.splice(0,1)[0]);
      this.rxjsService.setIsSongPlaylingState(this.loopMode == this.LoopModePlaylist&& this.isSongPlaying);
      this.updateQueue();
    } catch (error) {
      console.log(error);
    }
  }

  public loopPlayback(): void{

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
    return LoopMode.none;
  }

  get LoopModeAudio(): string{
    return LoopMode.audio;
  }

  get LoopModePlaylist(): string{
    return LoopMode.playlist;
  }

  get CurrentLoopMode(): string{
    return this.loopMode;
  }
}
