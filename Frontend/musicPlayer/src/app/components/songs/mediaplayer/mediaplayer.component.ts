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
      }


    });
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
