import { Component, Input } from '@angular/core';
import { PlaylistSongPaginationModel } from 'src/app/models/playlist-models';

@Component({
  selector: 'app-song-table',
  templateUrl: './song-table.component.html',
  styleUrls: ['./song-table.component.scss']
})
export class SongTableComponent {

  @Input() songs!: PlaylistSongPaginationModel;

  /**
   *
   */
  constructor() {

    
  }

}
