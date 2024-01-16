import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NzMessageService } from 'ng-zorro-antd/message';
import { APIROUTES } from 'src/app/constants/api-routes';
import { LOOPMODES } from 'src/app/constants/loop-modes';
import { QUEUETYPES } from 'src/app/constants/queue-types';
import { PlaylistSongModel, SongPaginationModel } from 'src/app/models/playlist-models';
import { QueueModel } from 'src/app/models/storage';
import { QueueService } from 'src/app/services/queue.service';
import { RxjsStorageService } from 'src/app/services/rxjs-storage.service';
import { SongService } from 'src/app/services/song.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-song-details',
  templateUrl: './song-details.component.html',
  styleUrls: ['./song-details.component.scss']
})
export class SongDetailsComponent implements OnInit {
  private songId: string = '';

  private songModel: PlaylistSongModel = {
    album:{
      name: ''
    }
  } as PlaylistSongModel;

  private isSongPlaying: boolean = false;

  private queueModel: QueueModel = undefined as any;

  private currentPlayingSong: PlaylistSongModel = undefined as any;

  /**
   *
   */
  constructor(private route: ActivatedRoute, 
    private songService: SongService, private message: NzMessageService, 
    private rxjsStorageService: RxjsStorageService, private queueService: QueueService) {
    
      if (!this.route.snapshot.paramMap.has('songId')) {
        console.log("Playlist id not found");
        return;
      }
  
      this.songId = this.route.snapshot.paramMap.get('songId') as string;
      this.songService.GetSongDetails(this.songId).subscribe({
        next:(songModel: PlaylistSongModel)=>{
          this.songModel = songModel;
        },
        error:(error: any)=>{
          this.message.error("Error when getting song details.");
        }
      })
  }

  ngOnInit(): void {
    this.rxjsStorageService.isSongPlayingState.subscribe(x => {
      this.isSongPlaying = x;
    });

    this.rxjsStorageService.currentPlayingSong.subscribe(x => {
      this.currentPlayingSong = x;
    });

    this.rxjsStorageService.currentQueueFilterAndPagination.subscribe(x => {
      this.queueModel = x;
    });

    this.rxjsStorageService.updateCurrentTableBoolean$.subscribe(x => {
      this.updateSong();
    });
  }

  public playSongs(): void{
    console.log("Play songs")

    // If the user previously clicked stop and wants to resume the playlist with the same queue
    if (this.currentPlayingSong && this.currentPlayingSong.id == this.songModel.id) {
      this.rxjsStorageService.setIsSongPlaylingState(true);
      return;
    }

    this.rxjsStorageService.setQueueFilterAndPagination({
      asc : true,
      page : 0,
      take : 0,
      query : '',
      sortAfter : '',
      itemId : this.songId,
      target : QUEUETYPES.song,
      loopMode: this.queueModel.loopMode== undefined ? LOOPMODES.none: this.queueModel.loopMode,
      random: this.queueModel.random == undefined ? false: this.queueModel.random
    });

    this.queueService.CreateQueueFromSingleSong(this.songId, this.queueModel.random, this.queueModel.loopMode).subscribe({
      next:(song: PlaylistSongModel)=>{
        
        this.rxjsStorageService.setCurrentPlayingSong(song);
        this.rxjsStorageService.setIsSongPlaylingState(true);
        this.rxjsStorageService.showMediaPlayer(true);
        console.log(song)
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
    this.rxjsStorageService.setIsSongPlaylingState(false);
  }

  public updateSong(): void{
    this.songService.GetSongDetails(this.songId).subscribe({
      next:(songModel: PlaylistSongModel)=>{
        this.songModel = songModel;
      },
      error:(error: any)=>{
        this.message.error("Error when getting song details.");
      }
    })
  }

  public getAlbumCoverSrc(): string{
    if (!this.songModel.album) {
      return ''
    }

    return `${environment.apiUrl}/${APIROUTES.file}/album/${this.songModel.album.id}`
  }

  public get IsSongPlaying(): boolean{
    return this.isSongPlaying;
  }

  public get CurrentPlayingSong(): PlaylistSongModel{
    return this.currentPlayingSong;
  }

  public get SongModel(): PlaylistSongModel{
    return this.songModel;
  }

  public get ReleaseDate(): Date{
    return new Date(this.songModel.album.release);
  }
}
