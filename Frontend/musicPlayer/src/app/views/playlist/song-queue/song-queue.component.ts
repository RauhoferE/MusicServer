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

  //private queueModel: QueueModel = undefined as any;

  private isSongPlaying: boolean = false;

  /**
   *
   */
  constructor(private rxjsStorageService: RxjsStorageService, private songService: SongService, private queueService: QueueService) {
    
  }

  ngOnInit(): void {
    // this.rxjsStorageService.currentSongQueue.subscribe(x => {
    //   if (!Array.isArray(x)) {
    //     this.songsModel = {
    //       songs : [],
    //       totalCount : 0
    //     } as SongPaginationModel;
    //     return;
    //   }

    //   this.songsModel = {
    //     songs : x,
    //     totalCount : x.length
    //   } as SongPaginationModel;
    // });

    this.rxjsStorageService.updateQueueBoolean$.subscribe(x => {
      this.onQueueTablePaginationUpdated();
    });

    this.rxjsStorageService.currentPlayingSong.subscribe(x => {
      this.currentPlayingSong = x;
    });

    // this.rxjsStorageService.currentQueueFilterAndPagination.subscribe(x => {
    //   this.queueModel = x;
    // });

    this.rxjsStorageService.isSongPlayingState.subscribe(x => {
      this.isSongPlaying = x;
    })

    this.rxjsStorageService.updateCurrentTableBoolean$.subscribe(x => {
      this.onQueueTablePaginationUpdated();
      this.onCurrentSongPaginationUpdated();
    });
  }
  
  async onQueueTablePaginationUpdated() {
    this.rxjsStorageService.setSongTableLoadingState(true);
    // if (this.songsModel.songs.length == 0) {
    //   this.rxjsStorageService.setSongTableLoadingState(false);
    //   return;
    // }

    // this.rxjsStorageService.setSongTableLoadingState(true);
    // let songsList: PlaylistSongModel[] = [];
    // for (let index = 0; index < this.SongsModel.songs.length; index++) {
    //   var songElement = await this.getSongDetails(this.SongsModel.songs[index].id);
    //   if (songElement.id != '-1') {
    //     songElement.order = index + 2;
    //     songsList.push(songElement);
    //   }
    // }

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
    // if (!this.currentPlayingSong.id || this.currentPlayingSong.id == '-1') {
    //   this.rxjsStorageService.setSongTableLoadingState(false);
    //   return;
    // }

    // this.rxjsStorageService.setSongTableLoadingState(true);
    // let newCurrentSong: PlaylistSongModel = this.currentPlayingSong;
    // var songElement = await this.getSongDetails(this.currentPlayingSong.id);
    
    // if (songElement.id != '-1') {
    //   songElement.order = 1;
    //   newCurrentSong = songElement;
    // }

    this.rxjsStorageService.setSongTableLoadingState(true);

    this.queueService.GetCurrentSong().subscribe({
      next: (song: PlaylistSongModel) => {
        this.rxjsStorageService.setCurrentPlayingSong(song);

      },
      complete: ()=>{
        this.rxjsStorageService.setSongTableLoadingState(false);
      }
      
    })

    //this.rxjsStorageService.setCurrentPlayingSong(songElement);
    this.rxjsStorageService.setSongTableLoadingState(false);
  }

  public onPlaySongClicked(event: PlaylistSongModelParams): void{
    // console.log(event);
    // const indexOfSong = event.index;
    // const songModel = event.songModel;

    // if (indexOfSong < 0) {
    //   return;
    // }

    // // IF the user wants to resume the same song
    // if (this.CurrentlyPlayingSong && 
    //   this.CurrentlyPlayingSong.id == songModel.id) {
    //   this.rxjsStorageService.setIsSongPlaylingState(true);
    //   return;
    // }

    // // Set the clicked song as the currently playing song but keep the rest of the queue as is

    // this.rxjsStorageService.setCurrentPlayingSong(songModel);
    // this.rxjsStorageService.removeSongWithIndexFromQueue(indexOfSong);
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

    //this.rxjsStorageService.setCurrentPlayingSong(songElement);
    //this.rxjsStorageService.setSongTableLoadingState(false);
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

    //this.rxjsStorageService.pushSongToPlaceInQueue(event.srcIndex, event.destIndex);
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
