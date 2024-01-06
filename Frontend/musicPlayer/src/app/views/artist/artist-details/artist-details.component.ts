import { Component, OnInit } from '@angular/core';
import { ArtistShortModel } from 'src/app/models/artist-models';
import { QueueModel } from 'src/app/models/storage';

@Component({
  selector: 'app-artist-details',
  templateUrl: './artist-details.component.html',
  styleUrls: ['./artist-details.component.scss']
})
export class ArtistDetailsComponent implements OnInit {

  private artistId: string = '';

  private artistModel: ArtistShortModel = {

  } as ArtistShortModel;

  private isSongPlaying: boolean = false;

  private queueModel: QueueModel = undefined as any;

  /**
   *
   */
  constructor() {
    
    
  }

  ngOnInit(): void {
    throw new Error('Method not implemented.');
  }

}
