import { DOCUMENT } from '@angular/common';
import { Component, ElementRef, Inject, OnInit, ViewChild } from '@angular/core';
import { APIROUTES } from 'src/app/constants/api-routes';
import { PlaylistSongModel } from 'src/app/models/playlist-models';
import { QueueModel } from 'src/app/models/storage';
import { PlaylistService } from 'src/app/services/playlist.service';
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

  private currentQueue: PlaylistSongModel[] = [];

  public volumePercent: number = 100;

  public durationSlider: number = 0;

  public loopAudio: boolean = false;

  public mutedAudio: boolean = false;

  public randomizePlay: boolean = false;

  private audioElement = new Audio();

  constructor(private rxjsService: RxjsStorageService, private playlistService: PlaylistService) {
    this.audioElement.autoplay = false;
    
    this.audioElement.addEventListener("timeupdate", (x) => {
      if (isNaN(this.audioElement.currentTime) ) {
        return;
      }

      this.durationSlider = Math.round(this.audioElement.currentTime);
    });

    this.audioElement.addEventListener("ended", () => {
      if (this.CurrentQueue.length > 0) {
        this.playNextSong();
        return;
      }

      this.audioElement.currentTime = 0;
      this.rxjsService.setIsSongPlaylingState(false);
    });

  }

  ngOnInit(): void {
    
    // Get values from storage
    this.rxjsService.currentPlayingSong.subscribe(x => {
      let oldPlayingSong = JSON.parse(JSON.stringify(this.currentPlayingSong)) as PlaylistSongModel;
      this.currentPlayingSong = x;
      //this.audioElement.pause();

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
      this.audioElement.play();
    });

    this.rxjsService.currentQueueFilterAndPagination.subscribe(x => {
      this.queueModel = x;
    });

    this.rxjsService.isSongPlayingState.subscribe(x => {
      this.isSongPlaying = x;

      if (this.isSongPlaying) {
        this.audioElement.play();
      }

      if (!this.isSongPlaying) {
        this.audioElement.pause();
      }      
    });

    this.rxjsService.currentSongQueue.subscribe(x => {
      this.currentQueue = x;
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
        this.rxjsService.replaceSongInQueue(currentSong);
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
        this.rxjsService.replaceSongInQueue(currentSong);
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

  public randomizePlayback(): void{
    // TODO: Randomize queue
    this.randomizePlay = !this.randomizePlay;

  }

  public playPrevSong(): void{
    let lastPlayedSongs: PlaylistSongModel[] = [];
    this.rxjsService.currentPlayedSongs.subscribe(x => {
      lastPlayedSongs = x;
    });

    if (!Array.isArray(lastPlayedSongs)) {
      // No previous songs found.
      // Start Song from start.
      this.audioElement.currentTime = 0;
      return;
    }

    if (lastPlayedSongs.length == 0) {
      // No previous songs found.
      // Start Song from start.
      this.audioElement.currentTime = 0;
      return;
    }

    // Add current song to queue
    this.rxjsService.addSongToQueue(this.currentPlayingSong);
    // Push song to the top of the queue
    this.rxjsService.pushSongToPlaceInQueue(this.currentQueue.length - 1, 0);
    // Set last played song as current
    this.rxjsService.setCurrentPlayingSong(lastPlayedSongs[lastPlayedSongs.length - 1]);
    // Remove the last played song from played songs
    this.rxjsService.removeLastPlayedSong();
    this.rxjsService.decrementSongsInQueueFilter();
  }

  public playNextSong():void{

    if (this.currentQueue.length == 0) {
      // No next song found
      // Start Song from start.
      this.audioElement.currentTime = 0;
      return;
    }

    const nextSong = this.CurrentQueue[0];
    this.rxjsService.addSongToPlayed(this.currentPlayingSong);
    this.rxjsService.setCurrentPlayingSong(nextSong);
    this.rxjsService.removeSongWithIndexFromQueue(0);
    this.rxjsService.incrementSongsInQueueFilter();
  }

  public loopPlayback(): void{
    this.loopAudio = !this.loopAudio;
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

  get CurrentQueue(): PlaylistSongModel[]{
    return this.currentQueue;
  }

  get AudioVolume(): number{
    return this.volumePercent / 100;
  }
}
