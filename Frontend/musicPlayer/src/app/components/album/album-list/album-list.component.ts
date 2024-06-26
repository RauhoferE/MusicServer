import { Component, Input, OnDestroy, OnInit, ViewChild } from '@angular/core';
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
import { Subject, takeUntil } from 'rxjs';
import { StreamingClientService } from 'src/app/services/streaming-client.service';
import { QueueWrapperService } from 'src/app/services/queue-wrapper.service';

@Component({
  selector: 'app-album-list',
  templateUrl: './album-list.component.html',
  styleUrls: ['./album-list.component.scss']
})
export class AlbumListComponent implements OnInit, OnDestroy {

  @Input() albums: AlbumModel[] = [];

  private queueModel: QueueModel = {} as any;

  private isSongPlaying: boolean = false;

  private modifieablePlaylists: GuidNameModel[] = [];

  private selectedTableItem: AlbumModel = {
  } as AlbumModel;

  private currentPlayingSong: PlaylistSongModel = undefined as any;

  private destroy:Subject<any> = new Subject();
  
  /**
   *
   */
  constructor(private queueService: QueueService, 
    private playlistService: PlaylistService,private nzContextMenuService: NzContextMenuService,
    private message: NzMessageService,  private rxjstorageService: RxjsStorageService,
    private streamingService: StreamingClientService, private wrapperService: QueueWrapperService) {
    
    
  }
  ngOnDestroy(): void {
    this.destroy.next(true);
  }

  ngOnInit(): void {
    this.rxjstorageService.currentQueueFilterAndPagination.pipe(takeUntil(this.destroy)).subscribe(x=>{
      this.queueModel = x;
    });

    this.rxjstorageService.isSongPlayingState.pipe(takeUntil(this.destroy)).subscribe(x =>{
      this.isSongPlaying = x;
    });

    this.rxjstorageService.currentPlayingSong.pipe(takeUntil(this.destroy)).subscribe(x =>{
      this.currentPlayingSong = x;
    });

  }


  public addAlbumToPlaylist(id: string, params: AlbumModel): void{

    this.playlistService.AddAlbumToPlaylist(id, params.id).subscribe({
      next: ()=>{
        this.updateDashBoard();

      },
      error: (error)=>{
        console.log(error);
      }
    })

  }

  public async addAlbumToQueue(params: AlbumModel): Promise<void>{
    if (params.songCount == 0) {
      this.message.error("Album doesnt have songs!");
      return;
    }

    try {
      await this.wrapperService.AddAlbumToQueue(params.id);  
    } catch (error: any) {
      if (error.message) {
        this.message.error("Error when adding album to queue");
      }
    }
    
  }

  public contextMenu($event: MouseEvent, menu: NzDropdownMenuComponent, item: AlbumModel): void {
    this.selectedTableItem = item;
    this.playlistService.GetModifieablePlaylists(-1).subscribe((val)=>{
      this.modifieablePlaylists = val.playlists;
    })

    this.nzContextMenuService.create($event, menu);
    
    // Add events via jquery

  }

  public closeMenu(): void {
    this.nzContextMenuService.close();
  }

  public async playSong(id: string): Promise<void>{
    console.log("Play songs")

    // If the user previously clicked stop and wants to resume the playlist with the same queue
    if (this.queueModel &&
      this.queueModel.target == QUEUETYPES.album && 
      this.queueModel.itemId == id) {
      this.rxjstorageService.setIsSongPlaylingState(true);
      try {
        await this.streamingService.playPauseSong(true);  
      } catch (error) {
        
      }
      
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
      random: this.queueModel.random == undefined ? false: this.queueModel.random,
      userId: this.queueModel.userId
    });

    try {
      await this.wrapperService.CreateQueueFromAlbum(id, this.queueModel.random, this.queueModel.loopMode,-1);
      this.rxjstorageService.setIsSongPlaylingState(true);
      this.rxjstorageService.showMediaPlayer(true);
      await this.rxjstorageService.setUpdateSongState();
      await this.streamingService.sendCurrentSongProgress(true, 0);
    } catch (error: any) {
      this.message.error("Error when creating queue");
    }

  }

  public async pauseSong(): Promise<void>{
    this.rxjstorageService.setIsSongPlaylingState(false);
    try {
      await this.streamingService.playPauseSong(false);  
    } catch (error) {
      
    }
    
  }

  private updateDashBoard(): void{
    var currenState = false;
    this.rxjstorageService.updateDashboardBoolean$.subscribe((val) =>{
      currenState = val;
    })

    // Update value in rxjs so the dashboard gets updated
    this.rxjstorageService.setUpdateDashboardBoolean(!currenState);
  }

  public getAlbumCoverSrc(albumId: string): string{
    if (!albumId) {
      return '';
    }

    return `${environment.apiUrl}/${APIROUTES.file}/album/${albumId}`;

  }

  public getYear(date: Date): number{
    return new Date(date).getFullYear();
  }

  public get Albums(): AlbumModel[]{
    return this.albums;
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

  public get CurrentPlayingSong(): PlaylistSongModel{
    return this.currentPlayingSong;
  }

}
