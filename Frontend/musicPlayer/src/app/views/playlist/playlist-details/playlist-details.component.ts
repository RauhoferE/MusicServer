import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzModalService } from 'ng-zorro-antd/modal';
import { Subject, firstValueFrom, lastValueFrom, takeUntil } from 'rxjs';
import { APIROUTES } from 'src/app/constants/api-routes';
import { LOOPMODES } from 'src/app/constants/loop-modes';
import { QUEUETYPES } from 'src/app/constants/queue-types';
import { DragDropSongParams, EditPlaylistModalParams, PlaylistSongModelParams } from 'src/app/models/events';
import { PlaylistSongModel, SongPaginationModel, PlaylistUserShortModel } from 'src/app/models/playlist-models';
import { PaginationModel, QueueModel } from 'src/app/models/storage';
import { FileService } from 'src/app/services/file.service';
import { JwtService } from 'src/app/services/jwt.service';
import { PlaylistService } from 'src/app/services/playlist.service';
import { QueueService } from 'src/app/services/queue.service';
import { RxjsStorageService } from 'src/app/services/rxjs-storage.service';
import { SessionStorageService } from 'src/app/services/session-storage.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-playlist-details',
  templateUrl: './playlist-details.component.html',
  styleUrls: ['./playlist-details.component.scss']
})
export class PlaylistDetailsComponent implements OnInit, OnDestroy {
  private playlistId: string = '';

  private songsModel: SongPaginationModel = {} as SongPaginationModel;

  private userName: string = '';

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

  private showPlaylistCreateModal: boolean = false;

  private userId: string = '-1';

  private destroy:Subject<any> = new Subject();


  /**
   *
   */
  constructor(private route: ActivatedRoute, 
    private playlistService: PlaylistService, private message: NzMessageService, 
    private sessionStorage: SessionStorageService, private jwtService: JwtService,
    private rxjsStorageService: RxjsStorageService,
    private queueService: QueueService, private fileService: FileService, private router: Router,
    private modal: NzModalService) {
    let savedPagination = this.sessionStorage.GetLastPaginationOfPlaylist();
  
    if (savedPagination) {
      // Save pagination from session storage in rxjs storage
      // This is done 
      //this.rxjsStorageService.setCurrentPaginationSongModel(savedPagination);
      this.paginationModel = savedPagination;
    }

    this.rxjsStorageService.setCurrentPaginationSongModel(this.paginationModel);

    if (!this.route.snapshot.paramMap.has('playlistId')) {
      console.log("Playlist id not found");
      return;
    }

    this.userId = this.jwtService.getUserId();

    this.playlistId = this.route.snapshot.paramMap.get('playlistId') as string;
    console.log(this.playlistId)
    this.getPlaylistInfo();
  }

  ngOnDestroy(): void {
    this.destroy.next(true);
  }

  ngOnInit(): void {
    this.rxjsStorageService.isSongPlayingState.pipe(takeUntil(this.destroy)).subscribe(x => {
      this.isSongPlaying = x;
    });

    this.rxjsStorageService.currentQueueFilterAndPagination.pipe(takeUntil(this.destroy)).subscribe(x => {
      this.queueModel = x;
    });

    this.rxjsStorageService.currentPlayingSong.pipe(takeUntil(this.destroy)).subscribe(x => {
      this.currentPlayingSong = x;
    });

    this.rxjsStorageService.updateCurrentTableBoolean$.pipe(takeUntil(this.destroy)).subscribe(x => {
      this.onPaginationUpdated();
    });
    
  }

  public onGetSongs(page: number, take: number, sortAfter: string, asc: boolean, query: string): void{
    console.log("Get Songs")
    const skipSongs = (page - 1) * take;
    this.rxjsStorageService.setSongTableLoadingState(true);
    this.playlistService.GetSongsFromPlaylist(skipSongs, take, sortAfter, asc, query, this.playlistId).subscribe({
      next:(songsModel: SongPaginationModel)=>{
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

  public onPaginationUpdated(): void{
    console.log("Get Elements")

    let pModel = {} as PaginationModel;

    this.rxjsStorageService.currentPaginationSongModel$.subscribe((val) => {
      pModel = val as PaginationModel;
    })

    this.sessionStorage.SaveLastPaginationOfPlaylist(pModel);
    this.onGetSongs(pModel.page, pModel.take, pModel.sortAfter, 
      pModel.asc, pModel.query);
  }

  public async playSongs(): Promise<void>{
    console.log("Play songs")

    // If the user previously clicked stop and wants to resume the playlist with the same queue
    if (this.QueueModel &&
      this.QueueModel.target == QUEUETYPES.playlist && 
      this.QueueModel.itemId == this.playlistId) {
      this.rxjsStorageService.setIsSongPlaylingState(true);
      return;
    }
    const paginationModel = await this.getCurrentPaginationModel();

    this.rxjsStorageService.setQueueFilterAndPagination({
      asc : paginationModel.asc,
      page : 0,
      take : 0,
      query : '',
      sortAfter : paginationModel.sortAfter,
      itemId : this.playlistId,
      target : QUEUETYPES.playlist,
      loopMode: this.queueModel.loopMode== undefined ? LOOPMODES.none: this.queueModel.loopMode,
      random: this.queueModel.random == undefined ? false: this.queueModel.random
    });

    this.queueService.CreateQueueFromPlaylist(this.playlistId, this.queueModel.random, this.queueModel.loopMode, paginationModel.sortAfter, paginationModel.asc, -1).subscribe({
      next:async (song: PlaylistSongModel)=>{
        console.log(song)
        
        this.rxjsStorageService.setCurrentPlayingSong(song);
        await this.rxjsStorageService.setUpdateSongState();
        this.rxjsStorageService.setIsSongPlaylingState(true);
        this.rxjsStorageService.showMediaPlayer(true);
      },
      error:(error: any)=>{
        this.message.error("Error when getting queue.");
      },
      complete: () => {
      }
    });
  }

  public pauseSongs(): void {
    // Stop playing of song
    this.rxjsStorageService.setIsSongPlaylingState(false);
  }

  public async onPlaySongClicked(event: PlaylistSongModelParams): Promise<void>{
    console.log(event);
    const indexOfSong = event.index;
    const songModel = event.songModel;

    if (indexOfSong < 0) {
      return;
    }

    // IF the user wants to resume the same song
    if (this.CurrentPlayingSong && 
      this.CurrentPlayingSong.id == songModel.id) {
      this.rxjsStorageService.setIsSongPlaylingState(true);
      return;
    }

    const paginationModel = await this.getCurrentPaginationModel();

    const skipSongs = ((this.paginationModel.page - 1) * this.paginationModel.take) + indexOfSong;

    this.rxjsStorageService.setQueueFilterAndPagination({
      asc : paginationModel.asc,
      page : 0,
      take : 0,
      query : '',
      sortAfter : paginationModel.sortAfter,
      itemId : this.playlistId,
      target : QUEUETYPES.playlist,
      loopMode: this.queueModel.loopMode== undefined ? LOOPMODES.none: this.queueModel.loopMode,
      random: this.queueModel.random == undefined ? false: this.queueModel.random
    });

    this.queueService.CreateQueueFromPlaylist(this.playlistId, this.queueModel.random, this.queueModel.loopMode, paginationModel.sortAfter, paginationModel.asc, event.songModel.order).subscribe({
      next:async (song: PlaylistSongModel)=>{
        console.log(song)
        this.rxjsStorageService.setCurrentPlayingSong(song);
        await this.rxjsStorageService.setUpdateSongState();
        this.rxjsStorageService.setIsSongPlaylingState(true);
        this.rxjsStorageService.showMediaPlayer(true);
      },
      error:(error: any)=>{
        this.message.error("Error when getting queue.");
      },
      complete: () => {
      }
    });
  }

  changeSongPosition(event: DragDropSongParams): void {
    // TODO: Change so the query doesnt matter
    this.playlistService.ChangeOrderOfSongInPlaylist(this.playlistId, event.srcIndex, event.destIndex).subscribe({
      next:()=>{
        this.onPaginationUpdated();
      },
      error:(error: any)=>{
        this.message.error("Error when changing order of songs.");
      },
      complete: () => {
      }
    });
  }

  public async createPlaylist(event: EditPlaylistModalParams): Promise<void> {
    this.ShowPlaylistCreateModal = false;

    try {
      var playlistId = await lastValueFrom(this.playlistService.CreatePlaylist(event.playlistModel.name, event.playlistModel.description, event.playlistModel.isPublic,
        event.playlistModel.receiveNotifications));
  
        if (event.newCoverFile) {
          await lastValueFrom(this.fileService.ChangePlaylistCover(event.newCoverFile, playlistId));
        }

        this.getPlaylistInfo();
        this.updateDashBoard();
    } catch (error) {
      console.log(error);
      this.message.error("Error when creating playlist");
      
    }
  }

  public addToLibrary(): void {
    this.playlistService.AddPlaylistToLibrary(this.playlistId).subscribe({
      next: ()=>{
        this.updateDashBoard();
        this.getPlaylistInfo();
      },
      error: (error)=>{
        console.log(error);
      }
    })
  }

  public copyToLibrary(): void {
    this.playlistService.CopyPlaylistToLibrary(this.playlistId).subscribe({
      next: ()=>{
        this.updateDashBoard();
      },
      error: (error)=>{
        console.log(error);
      }
    });
  }

  public setPublic(): void {
    this.playlistService.ModifyPlaylistInfo(this.playlistId, this.playlistModel.name, this.playlistModel.description, !this.playlistModel.isPublic, this.playlistModel.receiveNotifications).subscribe({
      next: ()=>{
        this.updateDashBoard();
        this.getPlaylistInfo();
      },
      error: (error)=>{
        console.log(error);
      }
    });
  }

  public activateNotifications(): void {
    this.playlistService.SetPlaylistNotifications(this.playlistId).subscribe({
      complete: ()=>{
        this.updateDashBoard();
        this.getPlaylistInfo();
      },
      error: (error) =>{
        console.log(error);
      }
    })
  }

  public deletePlaylist(): void {
    this.modal.confirm({
      nzTitle: `<b class="error-color">You really want to delete ${this.playlistModel.name}?</b>`,
      nzOkText: 'Yes',
      nzCancelText: 'No',
      nzOnOk: ()=>{
        this.playlistService.DeletePlaylist(this.playlistId).subscribe({
          next: ()=>{
            this.updateDashBoard();
            if (this.playlistModel.isCreator) {
              this.router.navigate(['/user']);
              return;
            }
              this.getPlaylistInfo();
            
          },
          error: (error)=>{
            console.log(error);
          }
        })
      }
    });
  }

  private getPlaylistInfo(): void{
    this.playlistService.GetPlaylistInfo(this.playlistId).pipe(takeUntil(this.destroy)).subscribe({
      next:(playlistModel: PlaylistUserShortModel)=>{
        this.playlistModel = playlistModel;
      },
      error:(error: any)=>{
        this.message.error("Error when getting playlist.");
      }
    })
  }

  private async getCurrentPaginationModel(): Promise<PaginationModel>{
    let pModel = {} as PaginationModel;

    try {
      pModel = await firstValueFrom(this.rxjsStorageService.currentPaginationSongModel$);
    } catch (error) {
      
    }

    return pModel;
  }

  private updateDashBoard(): void{
    var currenState = false;
    this.rxjsStorageService.updateDashboardBoolean$.subscribe((val) =>{
      currenState = val;
    })

    // Update value in rxjs so the dashboard gets updated
    this.rxjsStorageService.setUpdateDashboardBoolean(!currenState);
  }

  public getCreatorName(): string{
    if (this.playlistModel.users == undefined) {
      return "";
    }

    return this.playlistModel.users.filter(x => x.isCreator)[0]!.userName;
  }

  public getUserHref(): string{
    if (this.playlistModel.users == undefined) {
      return "";
    }

    const creatorId = this.playlistModel.users.filter(x => x.isCreator)[0]!.id;
    return `/user/${creatorId}`;
  }

  public getPlaylistCoverSrc(): string{
    return `${environment.apiUrl}/${APIROUTES.file}/playlist/${this.playlistId}`
  }

  public isUserPartOfPlaylist(): boolean{
    if (!this.playlistModel || !this.playlistModel.users) {
      return false;
    }
    
    return (this.playlistModel.users.findIndex(x => x.id == parseInt(this.userId)) > -1)
  }

  public get IsSongPlaying(): boolean{
    return this.isSongPlaying;
  }

  public get QueueModel(): QueueModel{
    return this.queueModel;
  }

  public get CurrentPlayingSong(): PlaylistSongModel{
    return this.currentPlayingSong;
  }

  public get SongsModel(): SongPaginationModel{
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

  public get PlaylistModel(): PlaylistUserShortModel{
    return this.playlistModel;
  }

  public get ShowPlaylistCreateModal(): boolean{
    return this.showPlaylistCreateModal;
  }

  public set ShowPlaylistCreateModal(val: boolean){
    this.showPlaylistCreateModal = val;
  }



}
