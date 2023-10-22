import { Component, OnInit } from '@angular/core';
import { APIROUTES } from 'src/app/constants/api-routes';
import { AllFollowedEntitiesModel } from 'src/app/models/user-models';
import { JwtService } from 'src/app/services/jwt.service';
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

  /**
   *
   */
  constructor(private userService: UserService, private jwtService: JwtService) {
    
    
  }

  ngOnInit(): void {
    this.userName = this.jwtService.getUserName();

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

  getArtistSite(id: string): string{
    // TODO: Return with correct frontend address
    return `/artist/${id}`;
  }

  getProfilePicSrc(id: number): string{
    return `${environment.apiUrl}/${APIROUTES.file}/user/${id}`;
  }

  getProfileSite(id: number): string{
    // TODO: Return with correct frontend address
    return `/user/${id}`;
  }

  getPlaylistCoverSrc(id: string): string{
    return `${environment.apiUrl}/${APIROUTES.file}/playlist/${id}`;
  }

  getPlaylistSite(id: string): string{
    // TODO: Return with correct frontend address
    return `/playlist/${id}`;
  }

  getOwnAvatar(): string{
    return `${environment.apiUrl}/${APIROUTES.file}/user/-1`;
  }

  public get FollowedUser(){
    return this.followedEntities.followedUsers;
  }

  public get FollowedArtists(){
    return this.followedEntities.followedArtists;
  }

  public get FollowedPlaylists(){
    return this.followedEntities.followedPlaylists;
  }

  public get FavoriteSongsCount(){
    return this.followedEntities.favoritesSongCount;
  }

  public get UserName(){
    return this.userName;
  }

  public get FilterName(){
    return this.filterName;
  }
}
