import { Component, OnInit } from '@angular/core';
import { PlaylistSongModel } from 'src/app/models/playlist-models';
import { QueueModel } from 'src/app/models/storage';
import { RxjsStorageService } from 'src/app/services/rxjs-storage.service';

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

  constructor(private rxjsService: RxjsStorageService) {
    
    
  }

  ngOnInit(): void {
    // Get values from storage
  }

  public onDurationChanged(value: any){
    // Is number
    console.log(value)
  }

  public onVolumeChanged(value: any){
    // Is number
    console.log(value)
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

    return this.CurrentPlayingSong.artists.join(', ');
  }



}
