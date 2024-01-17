import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NzMessageService } from 'ng-zorro-antd/message';
import { Subject, takeUntil } from 'rxjs';
import { PlaylistPaginationModel } from 'src/app/models/playlist-models';
import { PaginationModel } from 'src/app/models/storage';
import { PlaylistService } from 'src/app/services/playlist.service';
import { RxjsStorageService } from 'src/app/services/rxjs-storage.service';

@Component({
  selector: 'app-user-details',
  templateUrl: './user-details.component.html',
  styleUrls: ['./user-details.component.scss']
})
export class UserDetailsComponent implements OnInit, OnDestroy {
  private userId: string = '';

  private playlistsPaginationModel: PaginationModel = {
    asc: true,
    page : 1,
    query : '',
    sortAfter : '',
    take: 10
  } as PaginationModel;

  private destroy:Subject<any> = new Subject();
  private playlistsModel: PlaylistPaginationModel = {} as PlaylistPaginationModel;
  //TODO: Get followed users and artists

  /**
   *
   */
  constructor(private route: ActivatedRoute, private message: NzMessageService, 
    private rxjsStorageService: RxjsStorageService, private playlistService: PlaylistService) {
    if (!this.route.snapshot.paramMap.has('userId')) {
      console.log("userId id not found");
      this.userId = '-1';
    }

    if (this.route.snapshot.paramMap.has('userId')) {
      this.userId = this.route.snapshot.paramMap.get('userId') as string;
    }

    this.rxjsStorageService.setCurrentPaginationSongModel(this.playlistsPaginationModel);    
  }

  ngOnInit(): void {
    throw new Error('Method not implemented.');
  }
  ngOnDestroy(): void {
    throw new Error('Method not implemented.');
  }

  public onGetSongs(page: number, take: number, sortAfter: string, asc: boolean, query: string): void{
    console.log("Get Playlists")
    const skipSongs = (page - 1) * take;
    this.rxjsStorageService.setSongTableLoadingState(true);
    this.playlistService.GetPlaylists(this.userId, this.playlistsPaginationModel.page, this.playlistsPaginationModel.take, this.playlistsPaginationModel.query, this.playlistsPaginationModel.sortAfter, this.playlistsPaginationModel.asc).subscribe({
      next:(playlistModel: PlaylistPaginationModel)=>{
        playlistModel.playlists.forEach(element => {
          element.checked = false;
        });
        this.playlistsModel = playlistModel;
      },
      error:(error: any)=>{
        this.message.error("Error when getting songs from playlist.");
      },
      complete: () => {
        this.rxjsStorageService.setSongTableLoadingState(false);
      }
    });
  }

  public onPaginationUpdated(): void{
    console.log("Get Elements")

    let pModel = {} as PaginationModel;

    this.rxjsStorageService.currentPaginationSongModel$.subscribe((val) => {
      pModel = val as PaginationModel;
    })

    this.onGetSongs(pModel.page, pModel.take, pModel.sortAfter, 
      pModel.asc, pModel.query);
  }



}
