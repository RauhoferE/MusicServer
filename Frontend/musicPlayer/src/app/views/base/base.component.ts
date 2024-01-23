import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subject, lastValueFrom, takeUntil } from 'rxjs';
import { APIROUTES } from 'src/app/constants/api-routes';
import { ArtistShortModel } from 'src/app/models/artist-models';
import { FollowedPlaylistModel, PlaylistSongModel } from 'src/app/models/playlist-models';
import { QueueModel } from 'src/app/models/storage';
import { AllFollowedEntitiesModel, UserModel } from 'src/app/models/user-models';
import { JwtService } from 'src/app/services/jwt.service';
import { QueueService } from 'src/app/services/queue.service';
import { RxjsStorageService } from 'src/app/services/rxjs-storage.service';
import { UserService } from 'src/app/services/user.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-base',
  templateUrl: './base.component.html',
  styleUrls: ['./base.component.scss']
})
export class BaseComponent implements OnInit, OnDestroy {

  private followedEntities: AllFollowedEntitiesModel = {

  } as AllFollowedEntitiesModel;

  private userName: string = '';

  public search: string = '';

  private filterName: string = '';

  private currentPlayingSong: PlaylistSongModel = {} as PlaylistSongModel;

  private destroy:Subject<any> = new Subject();

  private timeStamp: Date = new Date();

  /**
   *
   */
  constructor(private userService: UserService, private jwtService: JwtService, private rxjsService: RxjsStorageService, private queueService: QueueService) {
    this.rxjsService.updateDashboardBoolean$.subscribe((val) => this.getFollowedEntities());
    
  }

  ngOnDestroy(): void {
    this.destroy.next(true);
  }

  async ngOnInit(): Promise<void> {
    this.userName = this.jwtService.getUserName();

    this.rxjsService.currentPlayingSong.pipe(takeUntil(this.destroy)).subscribe(x => {
      this.currentPlayingSong = x;
    });

    this.rxjsService.updateProfilePicBoolean$.pipe(takeUntil(this.destroy)).subscribe(x => {
      this.updateProfileSrc();
    });

    this.getFollowedEntities();
    await this.getQueueDataAsync();
    await this.getCurrentPlayingSongAsync();
  }

  async getQueueDataAsync(): Promise<void> {
    try {
      let queueData = await lastValueFrom(this.queueService.GetQueueData());
      this.rxjsService.setQueueFilterAndPagination(queueData);
    } catch (error) {
      
    }
  }

  async getCurrentPlayingSongAsync(): Promise<void> {
    try {
      let currentSong = await lastValueFrom(this.queueService.GetCurrentSong());
      this.rxjsService.setCurrentPlayingSong(currentSong);
      this.rxjsService.showMediaPlayer(true);
    } catch (error) {
      // No current Song found 
    }
  }

  getFollowedEntities(): void{
    this.userService.GetFollowedEntities(this.filterName,this.search).pipe(takeUntil(this.destroy)).subscribe({
      next: (element: AllFollowedEntitiesModel)=> {
        this.followedEntities = element;
      },
      error:(error)=>{
        console.log(error);
      }
    })
  }

  searchOnInput(): void{

    this.getFollowedEntities();
  }

  setFilter(event: any, filterName: string): void{
    if (this.filterName == filterName) {
      this.filterName = '';
      this.getFollowedEntities();
      return;
    }

    this.filterName = filterName;
    

    this.getFollowedEntities();
  }

  updateProfileSrc(): void{   
    this.timeStamp = new Date();
  }

  getArtistCoverSrc(id: string): string{
    return `${environment.apiUrl}/${APIROUTES.file}/artist/${id}`;
  }

  getProfilePicSrc(id: number): string{
    return `${environment.apiUrl}/${APIROUTES.file}/user/${id}`;
  }

  getPlaylistCoverSrc(id: string): string{
    return `${environment.apiUrl}/${APIROUTES.file}/playlist/${id}`;
  }

  getOwnAvatar(): string{
    return `${environment.apiUrl}/${APIROUTES.file}/user/-1?${this.timeStamp.getTime()}`;
  }

  public get CurrentPlayingSong(): PlaylistSongModel{
    return this.currentPlayingSong;
  }

  public get FollowedUser(): UserModel[]{
    return this.followedEntities.followedUsers;
  }

  public get FollowedArtists(): ArtistShortModel[]{
    return this.followedEntities.followedArtists;
  }

  public get FollowedPlaylists(): FollowedPlaylistModel[]{
    return this.followedEntities.followedPlaylists;
  }

  public get FavoriteSongsCount(): number{
    return this.followedEntities.favoritesSongCount;
  }

  public get UserName(): string{
    return this.userName;
  }

  public get FilterName(): string{
    return this.filterName;
  }
}
