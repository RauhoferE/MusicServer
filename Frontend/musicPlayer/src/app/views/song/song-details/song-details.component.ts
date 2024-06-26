import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NzMessageService } from 'ng-zorro-antd/message';
import { Subject, takeUntil } from 'rxjs';
import { APIROUTES } from 'src/app/constants/api-routes';
import { LOOPMODES } from 'src/app/constants/loop-modes';
import { QUEUETYPES } from 'src/app/constants/queue-types';
import { PlaylistSongModel, SongPaginationModel } from 'src/app/models/playlist-models';
import { QueueModel } from 'src/app/models/storage';
import { QueueWrapperService } from 'src/app/services/queue-wrapper.service';
import { QueueService } from 'src/app/services/queue.service';
import { RxjsStorageService } from 'src/app/services/rxjs-storage.service';
import { SongService } from 'src/app/services/song.service';
import { StreamingClientService } from 'src/app/services/streaming-client.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-song-details',
  templateUrl: './song-details.component.html',
  styleUrls: ['./song-details.component.scss']
})
export class SongDetailsComponent implements OnInit, OnDestroy {
  private songId: string = '';

  private songModel: PlaylistSongModel = {
    album:{
      name: ''
    }
  } as PlaylistSongModel;

  private isSongPlaying: boolean = false;

  private queueModel: QueueModel = undefined as any;

  private currentPlayingSong: PlaylistSongModel = undefined as any;

  private destroy:Subject<any> = new Subject();


  /**
   *
   */
  constructor(private route: ActivatedRoute, 
    private songService: SongService, private message: NzMessageService, 
    private rxjsStorageService: RxjsStorageService, private queueService: QueueService,
    private streamingService: StreamingClientService, private wrapperService: QueueWrapperService) {
    
      if (!this.route.snapshot.paramMap.has('songId')) {
        console.log("Playlist id not found");
        return;
      }
  
      this.songId = this.route.snapshot.paramMap.get('songId') as string;
      this.songService.GetSongDetails(this.songId).pipe(takeUntil(this.destroy)).subscribe({
        next:(songModel: PlaylistSongModel)=>{
          this.songModel = songModel;
        },
        error:(error: any)=>{
          this.message.error("Error when getting song details.");
        }
      })
  }

  ngOnDestroy(): void {
    this.destroy.next(true);
  }

  ngOnInit(): void {
    this.rxjsStorageService.isSongPlayingState.pipe(takeUntil(this.destroy)).subscribe(x => {
      this.isSongPlaying = x;
    });

    this.rxjsStorageService.currentPlayingSong.pipe(takeUntil(this.destroy)).subscribe(x => {
      this.currentPlayingSong = x;
    });

    this.rxjsStorageService.currentQueueFilterAndPagination.pipe(takeUntil(this.destroy)).subscribe(x => {
      this.queueModel = x;
    });

    this.rxjsStorageService.updateCurrentTableBoolean$.pipe(takeUntil(this.destroy)).subscribe(x => {
      this.updateSong();
    });
  }

  public async playSongs(): Promise<void>{
    console.log("Play songs")

    // If the user previously clicked stop and wants to resume the playlist with the same queue
    if (this.currentPlayingSong && this.currentPlayingSong.id == this.songModel.id) {
      this.rxjsStorageService.setIsSongPlaylingState(true);
      try {
        await this.streamingService.playPauseSong(true);  
      } catch (error) {
        
      }
      
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
      random: this.queueModel.random == undefined ? false: this.queueModel.random,
      userId: this.queueModel.userId
    });

    try {
      await this.wrapperService.CreateQueueFromSingleSong(this.songId, this.queueModel.random, this.queueModel.loopMode);
      this.rxjsStorageService.setIsSongPlaylingState(true);
      this.rxjsStorageService.showMediaPlayer(true);
      await this.rxjsStorageService.setUpdateSongState();
      await this.streamingService.sendCurrentSongProgress(true, 0);
    } catch (error) {
      this.message.error("Error when creating queue.");
    }

  }

  public async pauseSongs(): Promise<void> {
    // Stop playing of song
    this.rxjsStorageService.setIsSongPlaylingState(false);
    try {
      await this.streamingService.playPauseSong(false);  
    } catch (error) {
      
    }
    
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
