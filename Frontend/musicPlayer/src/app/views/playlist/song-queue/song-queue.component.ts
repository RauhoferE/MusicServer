import { Component, OnInit } from '@angular/core';
import { PlaylistSongModel, PlaylistSongPaginationModel } from 'src/app/models/playlist-models';
import { PaginationModel } from 'src/app/models/storage';
import { RxjsStorageService } from 'src/app/services/rxjs-storage.service';

@Component({
  selector: 'app-song-queue',
  templateUrl: './song-queue.component.html',
  styleUrls: ['./song-queue.component.scss']
})
export class SongQueueComponent implements OnInit {

  private songsModel: PlaylistSongPaginationModel = {} as PlaylistSongPaginationModel;

  private currentPlayingSong: PlaylistSongModel = undefined as any;

  /**
   *
   */
  constructor(private rxjsStorageService: RxjsStorageService) {
    
  }

  ngOnInit(): void {
    this.rxjsStorageService.currentSongQueue.subscribe(x => {
      this.songsModel = {
        songs : x,
        totalCount : x.length
      } as PlaylistSongPaginationModel;
    });

    this.rxjsStorageService.currentPlayingSong.subscribe(x => {
      this.currentPlayingSong = x;
    });

    this.rxjsStorageService.updateCurrentTableBoolean$.subscribe(x => {
      this.onPaginationUpdated();
    });
  }
  
  onPaginationUpdated() {
    throw new Error('Method not implemented.');
  }

}
