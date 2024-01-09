import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { RxjsStorageService } from 'src/app/services/rxjs-storage.service';
import { SongService } from 'src/app/services/song.service';
import {CdkVirtualScrollViewport} from '@angular/cdk/scrolling';
import { AlbumModel, AlbumPaginationModel } from 'src/app/models/artist-models';
import { environment } from 'src/environments/environment';
import { APIROUTES } from 'src/app/constants/api-routes';
import { QueueModel } from 'src/app/models/storage';
import { QUEUETYPES } from 'src/app/constants/queue-types';
import { LOOPMODES } from 'src/app/constants/loop-modes';
import { QueueService } from 'src/app/services/queue.service';
import { GuidNameModel, PlaylistSongModel } from 'src/app/models/playlist-models';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzContextMenuService, NzDropdownMenuComponent } from 'ng-zorro-antd/dropdown';
import { PlaylistService } from 'src/app/services/playlist.service';

@Component({
  selector: 'app-album-list',
  templateUrl: './album-list.component.html',
  styleUrls: ['./album-list.component.scss']
})
export class AlbumListComponent implements OnInit {

  @Input() artistId!: string;

  public albums: AlbumModel[] = [];

  private albumPage: number = 1;

  private take: number = 2;

  private isLoading: boolean = false;

  private totalAlbums: number = 0;

  private queueModel: QueueModel = {} as any;

  private isSongPlaying: boolean = false;

  private modifieablePlaylists: GuidNameModel[] = [];

  private selectedTableItem: AlbumModel = {
  } as AlbumModel;
  
  /**
   *
   */
  constructor(private songService: SongService, private queueService: QueueService, 
    private playlistService: PlaylistService,private nzContextMenuService: NzContextMenuService,
    private message: NzMessageService,  private rxjstorageService: RxjsStorageService) {
    
    
  }

  ngOnInit(): void {
    if (!this.artistId) {
      console.log("artist id not found")
      return;
    }

    this.rxjstorageService.currentQueueFilterAndPagination.subscribe(x=>{
      this.queueModel = x;
    })

    this.rxjstorageService.isSongPlayingState.subscribe(x =>{
      this.isSongPlaying = x;
    })

    
    this.getNextAlbums();
  }

  getNextAlbums(): void{
    console.log("Get albums")
    this.songService.GetArtistAlbums(this.artistId, this.albumPage, this.take).subscribe({
      next: (albumModel: AlbumPaginationModel) =>{
        this.totalAlbums = albumModel.totalCount;

        if (albumModel.albums.length == 0) {
          return;
        }

        for (let index = 0; index < albumModel.albums.length; index++) {
          this.albums.push(albumModel.albums[index])
        }
      },
      error: (error)=>{
        console.log(error);
      }
    })
  }

  addAlbumToPlaylist(id: string, params: AlbumModel): void{

    this.playlistService.AddAlbumToPlaylist(id, params.id).subscribe({
      next: ()=>{
        this.updateDashBoard();

      },
      error: (error)=>{
        console.log(error);
      }
    })

  }

  addAlbumToQueue(params: AlbumModel): void{
    this.queueService.AddAlbumToQueue(params.id).subscribe({
      next: ()=>{
        this.updateDashBoard();

      },
      error: (error)=>{
        console.log(error);
      }
    })
  }

  contextMenu($event: MouseEvent, menu: NzDropdownMenuComponent, item: AlbumModel): void {
    this.selectedTableItem = item;
    this.playlistService.GetModifieablePlaylists(-1).subscribe((val)=>{
      this.modifieablePlaylists = val.playlists;
    })

    this.nzContextMenuService.create($event, menu);
    
    // Add events via jquery

  }

  closeMenu(): void {
    this.nzContextMenuService.close();
  }

  playSong(id: string): void{
    console.log("Play songs")

    // If the user previously clicked stop and wants to resume the playlist with the same queue
    if (this.queueModel &&
      this.queueModel.target == QUEUETYPES.album && 
      this.queueModel.itemId == id) {
      this.rxjstorageService.setIsSongPlaylingState(true);
      return;
    }
    this.rxjstorageService.setQueueFilterAndPagination({
      asc : true,
      page : 0,
      take : 0,
      query : '',
      sortAfter : 'name',
      itemId : id,
      // TOOD: Replace with interface
      target : QUEUETYPES.album,
      loopMode: this.queueModel.loopMode== undefined ? LOOPMODES.none: this.queueModel.loopMode,
      random: this.queueModel.random == undefined ? false: this.queueModel.random
    });

    this.queueService.CreateQueueFromAlbum(id, this.queueModel.random, this.queueModel.loopMode,-1).subscribe({
      next:async (song: PlaylistSongModel)=>{
        console.log(song)
        
        this.rxjstorageService.setCurrentPlayingSong(song);
        await this.rxjstorageService.setUpdateSongState();
        this.rxjstorageService.setIsSongPlaylingState(true);
        this.rxjstorageService.showMediaPlayer(true);
      },
      error:(error: any)=>{
        this.message.error("Error when getting queue.");
      },
      complete: () => {
      }
    });
  }

  pauseSong(): void{
    this.rxjstorageService.setIsSongPlaylingState(false);
  }

  updateDashBoard(): void{
    var currenState = false;
    this.rxjstorageService.updateDashboardBoolean$.subscribe((val) =>{
      currenState = val;
    })

    // Update value in rxjs so the dashboard gets updated
    this.rxjstorageService.setUpdateDashboardBoolean(!currenState);
  }

  scrollEvent(event : any): void{  
    // Check if the user scrolled to the bottom and load the next page of albums
    if ((Math.abs(event.srcElement.scrollTop - (event.srcElement.scrollHeight - event.srcElement.offsetHeight)) < 5) &&
    this.totalAlbums > this.albums.length
    ) {
      this.albumPage++;
      this.getNextAlbums();
    }
  }

  trackByFn(index: number, item: AlbumModel) {
    return item.id; // or item.id
  }

  getAlbumCoverSrc(albumId: string): string{
    if (!albumId) {
      return '';
    }

    return `${environment.apiUrl}/${APIROUTES.file}/album/${albumId}`;

  }

  getYear(date: Date): number{
    return new Date(date).getFullYear();
  }

  public get Albums(): AlbumModel[]{
    return this.albums;
  }

  public get IsLoading(): boolean{
    return this.isLoading;
  }

  public get CurrentPlayingAlbumId(): string{
    if (this.queueModel.target == QUEUETYPES.album) {
      return this.queueModel.itemId;
    }

    return '';
  }

  public get IsSongPlaying(): boolean{
    return this.isSongPlaying;
  }

  public get ModifieablePlaylists(): GuidNameModel[]{
    return this.modifieablePlaylists;
  }

  public get SelectedTableItem(): AlbumModel{
    return this.selectedTableItem;
  }

}
