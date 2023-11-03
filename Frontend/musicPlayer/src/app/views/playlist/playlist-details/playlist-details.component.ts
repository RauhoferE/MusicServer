import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NzMessageService } from 'ng-zorro-antd/message';
import { PlaylistSongPaginationModel } from 'src/app/models/playlist-models';
import { PaginationModel } from 'src/app/models/storage';
import { JwtService } from 'src/app/services/jwt.service';
import { PlaylistService } from 'src/app/services/playlist.service';
import { RxjsStorageService } from 'src/app/services/rxjs-storage.service';
import { SessionStorageService } from 'src/app/services/session-storage.service';

@Component({
  selector: 'app-playlist-details',
  templateUrl: './playlist-details.component.html',
  styleUrls: ['./playlist-details.component.scss']
})
export class PlaylistDetailsComponent implements OnInit {

  private playlistId: string = '';

  private songsModel: PlaylistSongPaginationModel = {} as PlaylistSongPaginationModel;

  private userName: string = '';

  private paginationModel: PaginationModel = {
    asc: true,
    page : 1,
    query : '',
    sortAfter : '',
    take: 10
  } as PaginationModel;

  /**
   *
   */
  constructor(private route: ActivatedRoute, 
    private playlistService: PlaylistService, private message: NzMessageService, 
    private sessionStorage: SessionStorageService, private jwtService: JwtService,
    private rxjsStorageService: RxjsStorageService) {
    let savedPagination = this.sessionStorage.GetLastPaginationOfPlaylist();
  
    if (savedPagination) {
      // Save pagination from session storage in rxjs storage
      // This is done 
      this.rxjsStorageService.setCurrentPaginationSongModel(savedPagination);
    }

    this.rxjsStorageService.setCurrentPaginationSongModel(this.paginationModel);

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

  public onGetSongs(page: number, take: number, sortAfter: string, asc: boolean, query: string): void{
    console.log("Get Songs")
    
    this.rxjsStorageService.setSongTableLoadingState(true);
    this.playlistService.GetSongsFromPlaylist(page, take, sortAfter, asc, query, this.playlistId).subscribe({
      next:(songsModel: PlaylistSongPaginationModel)=>{
        songsModel.songs.forEach(element => {
          element.checked = false;
        });
        this.songsModel = songsModel;
      },
      error:(error: any)=>{
        this.message.error("Error when getting songs from playlist.");
      },
      complete: () => {
        this.rxjsStorageService.setSongTableLoadingState(false);
      }
    });
  }

  public onPaginationUpdated(){
    console.log("Get Elements")

    let pModel = {} as PaginationModel;

    this.rxjsStorageService.currentPaginationSongModel$.subscribe((val) => {
      pModel = val as PaginationModel;
    })

    this.sessionStorage.SaveLastPaginationOfPlaylist(pModel);
    this.onGetSongs(pModel.page, pModel.take, pModel.sortAfter, 
      pModel.asc, pModel.query);
  }

  public get SongsModel(): PlaylistSongPaginationModel{
    return this.songsModel;
  }

  public get PaginationModel(): PaginationModel{
    return this.paginationModel;
  }

  public get TotalSongCount(): number{
    return this.songsModel.totalCount;
  }

  public get UserName(): string{
    return this.userName;
  }

}
