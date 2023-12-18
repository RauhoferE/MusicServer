import { Component, OnInit } from '@angular/core';
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

  private currentSecondsPlayed: number = 0;

  public volumePercent: number = 100;

  public durationSlider: number = 0;

  constructor(private rxjsService: RxjsStorageService, private playlistService: PlaylistService) {
    
    
  }

  ngOnInit(): void {
    // Get values from storage
    this.rxjsService.currentPlayingSong.subscribe(x => {
      this.currentPlayingSong = x;
    });

    this.rxjsService.currentQueueFilterAndPagination.subscribe(x => {
      this.queueModel = x;
    });

    this.rxjsService.isSongPlayingState.subscribe(x => {
      this.isSongPlaying = x;
    });

    this.rxjsService.currentSongQueue.subscribe(x => {
      this.currentQueue = x;
    });
  }

  public onDurationChanged(value: any){
    // Is number
    console.log(value)
  }

  public onVolumeChanged(value: any){
    // Is number
    console.log(value)
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

  }

  public playPrevSong(): void{
    let lastPlayedSongs: PlaylistSongModel[] = [];
    this.rxjsService.currentPlayedSongs.subscribe(x => {
      lastPlayedSongs = x;
    });

    if (!Array.isArray(lastPlayedSongs)) {
      // No previous songs found.
      // TODO: Start Song from start.
      return;
    }

    if (lastPlayedSongs.length == 0) {
      // No previous songs found.
      // TODO: Start Song from start.
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
      // TODO: Start Song from start.
      return;
    }

    const nextSong = this.CurrentQueue[0];
    this.rxjsService.addSongToPlayed(this.currentPlayingSong);
    this.rxjsService.setCurrentPlayingSong(nextSong);
    this.rxjsService.removeSongWithIndexFromQueue(0);
    this.rxjsService.incrementSongsInQueueFilter();
  }

  public loopPlayback(): void{
    // TOOD: Loop audio

  }

  // TOOD: Add events for audio end/start/stop/volume

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

  get getAlbumCoverSrc(): string{
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

  get CurrentSecondsPlayed(): number{
    return this.currentSecondsPlayed;
  }

  get CurrentPercentagePlayed(): number{
    if (!this.currentPlayingSong.duration) {
      return 0;
    }

    return (this.currentSecondsPlayed / this.currentPlayingSong.duration) * 100;
  }

  get VolumePercent(): number{
    return this.volumePercent;
  }

  get ArtistsAsString(): string{
    if (!this.currentPlayingSong.artists) {
      return '';
    }

    return this.CurrentPlayingSong.artists.map(x => x.name).join(', ');
  }





}
