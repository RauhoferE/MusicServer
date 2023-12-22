import { Component, OnInit } from '@angular/core';
import { PlaylistSongModel, PlaylistSongPaginationModel, PlaylistUserShortModel } from 'src/app/models/playlist-models';
import { PaginationModel, QueueModel } from 'src/app/models/storage';

@Component({
  selector: 'app-album-details',
  templateUrl: './album-details.component.html',
  styleUrls: ['./album-details.component.scss']
})
export class AlbumDetailsComponent implements OnInit {

  private albumId: string = '';

  private songsModel: PlaylistSongPaginationModel = {} as PlaylistSongPaginationModel;

  private artistName: string = '';

  private paginationModel: PaginationModel = {
    asc: true,
    page : 1,
    query : '',
    sortAfter : '',
    take: 10
  } as PaginationModel;

  private playlistModel: PlaylistUserShortModel = {

  } as PlaylistUserShortModel;

  private isSongPlaying: boolean = false;

  private queueModel: QueueModel = undefined as any;

  private currentPlayingSong: PlaylistSongModel = undefined as any;

  /**
   *
   */
  constructor() {
    
    
  }

  ngOnInit(): void {
    throw new Error('Method not implemented.');
  }
}
