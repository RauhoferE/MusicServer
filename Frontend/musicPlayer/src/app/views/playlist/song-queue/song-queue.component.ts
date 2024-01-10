import { Component, OnInit } from '@angular/core';
import { lastValueFrom } from 'rxjs';
import { DragDropSongParams, PlaylistSongModelParams } from 'src/app/models/events';
import { PlaylistSongModel, SongPaginationModel } from 'src/app/models/playlist-models';
import { PaginationModel, QueueModel } from 'src/app/models/storage';
import { QueueService } from 'src/app/services/queue.service';
import { RxjsStorageService } from 'src/app/services/rxjs-storage.service';
import { SongService } from 'src/app/services/song.service';

@Component({
  selector: 'app-song-queue',
  templateUrl: './song-queue.component.html',
  styleUrls: ['./song-queue.component.scss']
})
export class SongQueueComponent implements OnInit {

  private songsModel: SongPaginationModel = {} as SongPaginationModel;

  private currentPlayingSong: PlaylistSongModel = undefined as any;

  private isSongPlaying: boolean = false;

  /**
   *
   */
  constructor(private rxjsStorageService: RxjsStorageService, private songService: SongService, private queueService: QueueService) {
    
  }

  ngOnInit(): void {
    this.rxjsStorageService.updateQueueBoolean$.subscribe(x => {
      console.log("Update queue")
      this.onQueueTablePaginationUpdated();
    });

    this.rxjsStorageService.currentPlayingSong.subscribe(x => {
      this.currentPlayingSong = x;
    });

    this.rxjsStorageService.isSongPlayingState.subscribe(x => {
      this.isSongPlaying = x;
    })

    this.rxjsStorageService.updateCurrentTableBoolean$.subscribe(x => {
      console.log("Update queue and table")
      this.onQueueTablePaginationUpdated();
      this.onCurrentSongPaginationUpdated();
    });

    this.rxjsStorageService.setCurrentPaginationSongModel({
      asc: true,
      page: 1,
      query: '',
      sortAfter: 'order',
      take: 30
    })
  }
  
  async onQueueTablePaginationUpdated() {
    this.rxjsStorageService.setSongTableLoadingState(true);

    // this.rxjsStorageService.setSongQueue(songsList);
    this.queueService.GetCurrentQueue().subscribe({
      next: (songs: PlaylistSongModel[]) => {
        songs.splice(0,1);
        this.songsModel = {
          songs: songs,
          totalCount: songs.length
        };

      },
      complete: ()=>{
        this.rxjsStorageService.setSongTableLoadingState(false);
      }
      
    })

    
  }

  async onCurrentSongPaginationUpdated(): Promise<void> {

    this.rxjsStorageService.setSongTableLoadingState(true);

    this.queueService.GetCurrentSong().subscribe({
      next: (song: PlaylistSongModel) => {
        this.rxjsStorageService.setCurrentPlayingSong(song);

      },
      complete: ()=>{
        this.rxjsStorageService.setSongTableLoadingState(false);
      }
      
    })

    this.rxjsStorageService.setSongTableLoadingState(false);
  }

  public onPlaySongClicked(event: PlaylistSongModelParams): void{
    this.rxjsStorageService.setSongTableLoadingState(true);

    this.queueService.SkipForwardInQueue(event.songModel.order).subscribe({
      next: (song: PlaylistSongModel) => {
        // Set the new song
        this.rxjsStorageService.setCurrentPlayingSong(song);

        // Update the queue
        this.onQueueTablePaginationUpdated();

      },
      complete: ()=>{
        this.rxjsStorageService.setSongTableLoadingState(false);
      }
      
    })

  }

  public resumeSong(event: PlaylistSongModelParams): void{
    this.rxjsStorageService.setIsSongPlaylingState(!this.isSongPlaying);
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

  changeSongPosition(event: DragDropSongParams) {
    this.rxjsStorageService.setSongTableLoadingState(true);

    this.queueService.PushSongInQueue(event.srcSong.order, event.destSong.order).subscribe({
      next: (songs: PlaylistSongModel[])=>{
        songs.splice(0,1);
        this.songsModel = {
          songs: songs,
          totalCount: songs.length
        };
      },
      complete: ()=>{
        //this.onQueueTablePaginationUpdated();
        this.rxjsStorageService.setSongTableLoadingState(false);
      }
    })
    
  }

  get SongsModel(): SongPaginationModel{
    return this.songsModel;
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
