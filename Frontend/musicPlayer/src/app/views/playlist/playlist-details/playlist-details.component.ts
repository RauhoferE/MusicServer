import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-playlist-details',
  templateUrl: './playlist-details.component.html',
  styleUrls: ['./playlist-details.component.scss']
})
export class PlaylistDetailsComponent implements OnInit {

  private playlistId: string = '';

  /**
   *
   */
  constructor(private route: ActivatedRoute) {
    

    if (!this.route.snapshot.paramMap.has('playlistId')) {
      console.log("Playlist id not found");
      return;
    }

    this.playlistId = this.route.snapshot.paramMap.get('playlistId') as string;
  }

  ngOnInit(): void {
    //TODO: Get Songs for song table here 
    return;
  }

}
