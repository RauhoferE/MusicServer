import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subject, lastValueFrom, takeUntil } from 'rxjs';
import { DragDropQueueParams, DragDropSongParams, PlaylistSongModelParams } from 'src/app/models/events';
import { PlaylistSongModel, QueueSongModel, SongPaginationModel } from 'src/app/models/playlist-models';
import { PaginationModel, QueueModel } from 'src/app/models/storage';
import { QueueWrapperService } from 'src/app/services/queue-wrapper.service';
import { QueueService } from 'src/app/services/queue.service';
import { RxjsStorageService } from 'src/app/services/rxjs-storage.service';
import { SongService } from 'src/app/services/song.service';
import { StreamingClientService } from 'src/app/services/streaming-client.service';

@Component({
  selector: 'app-song-queue',
  templateUrl: './song-queue.component.html',
  styleUrls: ['./song-queue.component.scss']
})
export class SongQueueComponent implements OnInit, OnDestroy {

  private queueModel: SongPaginationModel = {} as SongPaginationModel;

  private nextSongsModel: SongPaginationModel = {} as SongPaginationModel;

  private currentPlayingSong: PlaylistSongModel = undefined as any;

  private isSongPlaying: boolean = false;

  private destroy:Subject<any> = new Subject();

  /**
   *
   */
  constructor(private rxjsStorageService: RxjsStorageService, private songService: SongService, 
    private wrapperService: QueueWrapperService, private streamingService: StreamingClientService) {

  }

  ngOnDestroy(): void {
    this.destroy.next(true);
  }

  public async ngOnInit(): Promise<void> {
    // TOOD: When changing site the subscribe gets called more often
    this.rxjsStorageService.updateQueueBoolean$.pipe(takeUntil(this.destroy)).subscribe(x => {
      console.log("Update queue")
      this.onQueueTablePaginationUpdated();
      this.onCurrentSongPaginationUpdated();
    });

    this.rxjsStorageService.currentPlayingSong.pipe(takeUntil(this.destroy)).subscribe(x => {
      this.currentPlayingSong = x;
    });

    this.rxjsStorageService.isSongPlayingState.pipe(takeUntil(this.destroy)).subscribe(x => {
      this.isSongPlaying = x;
    })

    this.streamingService.queueItemsReceivedEvents.pipe(takeUntil(this.destroy)).subscribe(songs => {
      songs.splice(0,1);
      this.queueModel = {
        songs: songs.filter(x => !x.addedManualy),
        totalCount: songs.filter(x => !x.addedManualy).length
      };

      this.nextSongsModel = {
        songs: songs.filter(x => x.addedManualy),
        totalCount: songs.filter(x => x.addedManualy).length
      };
    })

    this.streamingService.updateQueueEvent.pipe(takeUntil(this.destroy)).subscribe(async ()=>{
      await this.wrapperService.GetCurrentQueue();
    })

    // this.rxjsStorageService.updateCurrentTableBoolean$.subscribe(x => {
    //   console.log("Update queue and table")
    //   this.onQueueTablePaginationUpdated();
    //   this.onCurrentSongPaginationUpdated();
    // });

    this.rxjsStorageService.setCurrentPaginationSongModel({
      asc: true,
      page: 1,
      query: '',
      sortAfter: 'order',
      take: 30
    })
  }
  
  async onQueueTablePaginationUpdated(): Promise<void> {
    this.rxjsStorageService.setSongTableLoadingState(true);

    // this.rxjsStorageService.setSongQueue(songsList);
    let songs = await this.wrapperService.GetCurrentQueue();
    this.rxjsStorageService.setSongTableLoadingState(false);
    // Return if user is in a group, because we get the song, queueu from an event via the hub
    if (songs.length == 0) {
      return;
    }

    songs.splice(0,1);
    this.queueModel = {
      songs: songs.filter(x => !x.addedManualy),
      totalCount: songs.filter(x => !x.addedManualy).length
    };

    this.nextSongsModel = {
      songs: songs.filter(x => x.addedManualy),
      totalCount: songs.filter(x => x.addedManualy).length
    };
    
  }

  public async onCurrentSongPaginationUpdated(): Promise<void> {

    this.rxjsStorageService.setSongTableLoadingState(true);

    let song = await this.wrapperService.GetCurrentSong();
    this.rxjsStorageService.setSongTableLoadingState(false);
    // Return if user is in a group, because we get the song, queueu from an event via the hub
    if (song.id == '-1') {
      return;
    }
    this.rxjsStorageService.setCurrentPlayingSong(song);
  }

  public async onPlaySongClicked(event: PlaylistSongModelParams): Promise<void>{
    this.rxjsStorageService.setSongTableLoadingState(true);

    await this.wrapperService.SkipForwardInQueue(event.songModel.order);
    this.rxjsStorageService.setSongTableLoadingState(false);
    this.onQueueTablePaginationUpdated();
  }

  public async resumeSong(event: PlaylistSongModelParams): Promise<void>{
    this.rxjsStorageService.setIsSongPlaylingState(!this.isSongPlaying);
    await this.streamingService.playPauseSong(!this.isSongPlaying);
  }

  async getSongDetails(songId: string): Promise<PlaylistSongModel>{
    try {
      let res = await lastValueFrom(this.songService.GetSongDetails(songId));
      return res;
    } catch (error) {
      console.log(error);
      return {id : '-1'} as PlaylistSongModel;

    }
    
  }

  public async changeSongPosition(event: DragDropQueueParams): Promise<void> {
    this.rxjsStorageService.setSongTableLoadingState(true);
    console.log(event)

    let songs = await this.wrapperService.PushSongInQueue(event.srcSong.order, event.destSong.order, event.markAsManuallyAdded);
    this.rxjsStorageService.setSongTableLoadingState(false);
    if (songs.length == 0) {
      return;
    }
    songs.splice(0,1);
    this.queueModel = {
      songs: songs.filter(x => !x.addedManualy),
      totalCount: songs.filter(x => !x.addedManualy).length
    };

    this.nextSongsModel = {
      songs: songs.filter(x => x.addedManualy),
      totalCount: songs.filter(x => x.addedManualy).length
    };
    
  }

  get QueueModel(): SongPaginationModel{
    return this.queueModel;
  }

  get NextSongsModel(): SongPaginationModel{
    return this.nextSongsModel;
  }

  get CurrentPlayingSongModel(): SongPaginationModel{
    if (!this.currentPlayingSong.id || this.currentPlayingSong.id == '-1') {
      return {
        songs: [],
        totalCount: 0
      }
    }

    return {
      songs: [this.currentPlayingSong],
      totalCount: 1
    }
  }

  get CurrentlyPlayingSong(): PlaylistSongModel{
    return this.currentPlayingSong;
  }

}
