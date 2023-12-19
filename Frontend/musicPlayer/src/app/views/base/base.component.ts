import { Component, OnInit } from '@angular/core';
import { APIROUTES } from 'src/app/constants/api-routes';
import { ArtistShortModel } from 'src/app/models/artist-models';
import { FollowedPlaylistModel, PlaylistSongModel } from 'src/app/models/playlist-models';
import { AllFollowedEntitiesModel, UserModel } from 'src/app/models/user-models';
import { JwtService } from 'src/app/services/jwt.service';
import { RxjsStorageService } from 'src/app/services/rxjs-storage.service';
import { UserService } from 'src/app/services/user.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-base',
  templateUrl: './base.component.html',
  styleUrls: ['./base.component.scss']
})
export class BaseComponent implements OnInit {

  private followedEntities: AllFollowedEntitiesModel = {

  } as AllFollowedEntitiesModel;

  private userName: string = '';

  public search: string = '';

  private filterName: string = '';

  private currentPlayingSong: PlaylistSongModel = {} as PlaylistSongModel;

  /**
   *
   */
  constructor(private userService: UserService, private jwtService: JwtService, private rxjsService: RxjsStorageService) {
    this.rxjsService.updateDashboardBoolean$.subscribe((val) => this.getFollowedEntities());
    
  }

  ngOnInit(): void {
    this.userName = this.jwtService.getUserName();

    this.rxjsService.currentPlayingSong.subscribe(x => {
      this.currentPlayingSong = x;
    });

    this.getFollowedEntities();
  }

  getFollowedEntities(): void{
    this.userService.GetFollowedEntities(this.filterName,this.search).subscribe({
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
    return `${environment.apiUrl}/${APIROUTES.file}/user/-1`;
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
