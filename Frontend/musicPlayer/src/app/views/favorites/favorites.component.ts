import { Component, OnInit } from '@angular/core';
import { NzMessageService } from 'ng-zorro-antd/message';
import { PlaylistSongPaginationModel } from 'src/app/models/playlist-models';
import { PlaylistService } from 'src/app/services/playlist.service';
import { SessionStorageService } from 'src/app/services/session-storage.service';

@Component({
  selector: 'app-favorites',
  templateUrl: './favorites.component.html',
  styleUrls: ['./favorites.component.scss']
})
export class FavoritesComponent implements OnInit{

  private songsModel: PlaylistSongPaginationModel = {} as PlaylistSongPaginationModel;

  /**
   *
   */
  constructor(private playlistService: PlaylistService, private message: NzMessageService, private sessionStorage: SessionStorageService) {
    
    
  }

  ngOnInit(): void {
    let savedPagination = this.sessionStorage.GetLastPaginationOfFavorites();
    if (!savedPagination) {
      // REplace with default values
      this.onGetFavorites(1, 10, '', true, '');
      return;
    }

    this.onGetFavorites(savedPagination.page, savedPagination.take, savedPagination.sortAfter, savedPagination.asc, savedPagination.query);
  }

  public onGetFavorites(page: number, take: number, sortAfter: string, asc: boolean, query: string): void{
    this.playlistService.GetFavorites(page, take, sortAfter, asc, query).subscribe({
      next:(songsModel: PlaylistSongPaginationModel)=>{
        this.songsModel = songsModel;
      },
      error:(error: any)=>{
        this.message.error("Error when getting favorites.");
      }
    })
  }

  public get SongsModel(){
    return this.songsModel;
  }

}
