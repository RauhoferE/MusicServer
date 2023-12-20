import { Component, OnInit } from '@angular/core';
import { lastValueFrom } from 'rxjs';
import { PlaylistSongModelParams } from 'src/app/models/events';
import { PlaylistSongModel, PlaylistSongPaginationModel } from 'src/app/models/playlist-models';
import { PaginationModel, QueueModel } from 'src/app/models/storage';
import { RxjsStorageService } from 'src/app/services/rxjs-storage.service';
import { SongService } from 'src/app/services/song.service';

@Component({
  selector: 'app-song-queue',
  templateUrl: './song-queue.component.html',
  styleUrls: ['./song-queue.component.scss']
})
export class SongQueueComponent implements OnInit {

  private songsModel: PlaylistSongPaginationModel = {} as PlaylistSongPaginationModel;

  private currentPlayingSong: PlaylistSongModel = undefined as any;

  private queueModel: QueueModel = undefined as any;

  /**
   *
   */
  constructor(private rxjsStorageService: RxjsStorageService, private songService: SongService) {
    
  }

  ngOnInit(): void {
    this.rxjsStorageService.currentSongQueue.subscribe(x => {
      if (!Array.isArray(x)) {
        this.songsModel = {
          songs : [],
          totalCount : 0
        } as PlaylistSongPaginationModel;
        return;
      }

      this.songsModel = {
        songs : x,
        totalCount : x.length
      } as PlaylistSongPaginationModel;
    });

    this.rxjsStorageService.currentPlayingSong.subscribe(x => {
      this.currentPlayingSong = x;
    });

    this.rxjsStorageService.currentQueueFilterAndPagination.subscribe(x => {
      this.queueModel = x;
    });

    this.rxjsStorageService.updateCurrentTableBoolean$.subscribe(x => {
      this.onQueueTablePaginationUpdated();
      this.onCurrentSongPaginationUpdated();
    });
  }
  
  async onQueueTablePaginationUpdated() {
    if (this.songsModel.songs.length == 0) {
      this.rxjsStorageService.setSongTableLoadingState(false);
      return;
    }

    this.rxjsStorageService.setSongTableLoadingState(true);
    let songsList: PlaylistSongModel[] = [];
    for (let index = 0; index < this.SongsModel.songs.length; index++) {
      var songElement = await this.getSongDetails(this.SongsModel.songs[index].id);
      if (songElement.id != '-1') {
        songElement.order = index + 2;
        songsList.push(songElement);
      }
    }

    this.rxjsStorageService.setSongQueue(songsList);
    this.rxjsStorageService.setSongTableLoadingState(false);
  }

  async onCurrentSongPaginationUpdated(): Promise<void> {
    if (!this.currentPlayingSong.id || this.currentPlayingSong.id == '-1') {
      this.rxjsStorageService.setSongTableLoadingState(false);
      return;
    }

    this.rxjsStorageService.setSongTableLoadingState(true);
    let newCurrentSong: PlaylistSongModel = this.currentPlayingSong;
    var songElement = await this.getSongDetails(this.currentPlayingSong.id);
    
    if (songElement.id != '-1') {
      songElement.order = 1;
      newCurrentSong = songElement;
    }

    this.rxjsStorageService.setCurrentPlayingSong(songElement);
    this.rxjsStorageService.setSongTableLoadingState(false);
  }

  public onPlaySongClicked(event: PlaylistSongModelParams): void{
    console.log(event);
    const indexOfSong = event.index;
    const songModel = event.songModel;

    if (indexOfSong < 0) {
      return;
    }

    // IF the user wants to resume the same song
    if (this.CurrentlyPlayingSong && 
      this.CurrentlyPlayingSong.id == songModel.id) {
      this.rxjsStorageService.setIsSongPlaylingState(true);
      return;
    }

    // Set the clicked song as the currently playing song but keep the rest of the queue as is

    this.rxjsStorageService.setCurrentPlayingSong(songModel);
    this.rxjsStorageService.removeSongWithIndexFromQueue(indexOfSong);
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

  get SongsModel(): PlaylistSongPaginationModel{
    return this.songsModel;
  }

  get CurrentPlayingSongModel(): PlaylistSongPaginationModel{
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
