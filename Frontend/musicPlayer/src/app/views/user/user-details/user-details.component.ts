import { Component, ElementRef, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, ParamMap, Router } from '@angular/router';
import { NzMessageService } from 'ng-zorro-antd/message';
import { Subject, lastValueFrom, takeUntil } from 'rxjs';
import { APIROUTES } from 'src/app/constants/api-routes';
import { DragDropPlaylistParams, EditPlaylistModalParams } from 'src/app/models/events';
import { GuidNameModel, PlaylistPaginationModel } from 'src/app/models/playlist-models';
import { PaginationModel } from 'src/app/models/storage';
import { UserModel } from 'src/app/models/user-models';
import { FileService } from 'src/app/services/file.service';
import { JwtService } from 'src/app/services/jwt.service';
import { PlaylistService } from 'src/app/services/playlist.service';
import { RxjsStorageService } from 'src/app/services/rxjs-storage.service';
import { UserService } from 'src/app/services/user.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-user-details',
  templateUrl: './user-details.component.html',
  styleUrls: ['./user-details.component.scss']
})
export class UserDetailsComponent implements OnDestroy {
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
  private artists: GuidNameModel[] = [];
  private users: UserModel[] = [];
  public newProfilePic: File = {} as File;
  private fileError: string = '';

  private userModel: UserModel = {
    id: -1

  } as UserModel;

  private timeStamp = new Date();

  private showPlaylistEditModal: boolean = false;

  /**
   *
   */ 
  constructor(private route: ActivatedRoute, private message: NzMessageService, 
    private rxjsStorageService: RxjsStorageService, private playlistService: PlaylistService, private userService: UserService,
    private fileService: FileService, private jwtService: JwtService, private router: Router) {

      this.rxjsStorageService.setCurrentPaginationSongModel(this.playlistsPaginationModel); 

    this.route.paramMap.pipe(takeUntil(this.destroy)).subscribe((params: ParamMap) => {

      if (!params.has('userId')) {
        console.log("userId id not found");
        this.userId = '-1';
      }

      if (params.has('userId')) {
        console.log("userId id not found");
        this.userId = params.get('userId') as string;
      }

      if (params.has('userId') && (params.get('userId') == '-1' || this.jwtService.getUserId() == params.get('userId'))) {
        this.router.navigate(['/user']);
        return;
      }

      if (this.userId == '-1') {
        this.userModel.userName = this.jwtService.getUserName();
      }

      if (this.userId != '-1') {
        this.getUserModel();
      }

         
  
      this.rxjsStorageService.updatePlaylistViewBoolean.pipe(takeUntil(this.destroy)).subscribe(x =>{
        this.onPaginationUpdated();
      });
  
      //this.onPaginationUpdated();
      this.getFollowedArtists();
      this.getFollowedUsers();
      
    })    
  }

  ngOnDestroy(): void {
    this.destroy.next(true);
  }

  activateNotifications(): void {
    if (this.userId == '-1') {
      return;
    }

    if (this.userModel.receiveNotifications) {
      this.userService.RemoveNotficationsFromUser(this.userId).subscribe({
        error: (error)=>{
          console.log(error);
        },
        complete: ()=>{
          this.getUserModel();
        }
      });
      return;
    }

    this.userService.ReceiveNotficationsFromUser(this.userId).subscribe({
      error: (error)=>{
        console.log(error);
      },
      complete: ()=>{
        this.getUserModel();
        this.updateDashBoard();
      }
    });
  }

  subscribeToUser(user: UserModel): void {
    if (user.id == -1) {
      return;
    }

    if (user.isFollowedByUser) {
      this.userService.UnSubscribeFromUser(user.id.toString()).subscribe({
        error: (error)=>{
          console.log(error);
        },
        complete: ()=>{

          if (user.id == this.userModel.id) {
            this.getUserModel();
          }else{
            this.getFollowedUsers();
          }
          
          this.updateDashBoard();
        }
      });
      return;
    }

    this.userService.SubscribeToUser(user.id.toString()).subscribe({
      error: (error)=>{
        console.log(error);
      },
      complete: ()=>{
        if (user.id == this.userModel.id) {
          this.getUserModel();
        }else{
          this.getFollowedUsers();
        }

        this.updateDashBoard();
      }
    });
  }

  public activateNotificationsForUser(user: UserModel): void {
    if (user.id == -1) {
      return;
    }

    if (user.receiveNotifications) {
      this.userService.RemoveNotficationsFromUser(user.id.toString()).subscribe({
        error: (error)=>{
          console.log(error);
        },
        complete: ()=>{
                  if (user.id == this.userModel.id) {
          this.getUserModel();
        }else{
          this.getFollowedUsers();
        }
        }
      });
      return;
    }

    this.userService.ReceiveNotficationsFromUser(user.id.toString()).subscribe({
      error: (error)=>{
        console.log(error);
      },
      complete: ()=>{
        if (user.id == this.userModel.id) {
          this.getUserModel();
        }else{
          this.getFollowedUsers();
        }
        this.updateDashBoard();
      }
    });
    }

  public getUserModel(): void{
    if (this.userId == '-1') {
      return;
    }

    this.userService.SubscribeUserInfo(this.userId).subscribe({
      next: (model: UserModel)=>{
        this.userModel = model;
      },
      error: (error)=>{
        console.log(error);
      }
    })
  }

  public uploadNewCover(fileEvent: Event): void{
    const input = fileEvent.target as HTMLInputElement;

    if (input.files == null || input.files.length == 0) {
      return;
    }

    console.log("Upload Picture");
    console.log(input.files[0]);

    const targetFile = input.files[0];

    if (this.userId != '-1') {
      return;
    }

    if (targetFile.name.slice((targetFile.name.lastIndexOf(".") - 1 >>> 0) + 2) != 'png') {
      this.fileError = 'Only png files are accepted!';
      return;
    }

    if (targetFile.size > 1000000) {
      this.fileError = 'Max file size is 1 mb!';
      return;
    }

    this.fileError = '';

    this.fileService.ChangeProfilePicture(targetFile).subscribe({
      next: ()=>{
          // Clear the input
          input.value = '';
        this.timeStamp = new Date();
        this.updateProfilePicInBase();
      },
      error: (error)=>{
        console.log(error);
        this.fileError = error.message;
      }
    });
  }

  public subscribeToArtist(artist: GuidNameModel) {
    if (artist.followedByUser) {
      this.userService.UnSuscribeFromArtist(artist.id).subscribe({
        next: ()=>{
          this.getFollowedArtists();
          this.updateDashBoard();
          
        },
        error: (error)=>{
          console.log(error);
        }
      });

      return;
    }

    this.userService.SuscribeToArtist(artist.id).subscribe({
      next: ()=>{
        this.getFollowedArtists();
        this.updateDashBoard();
        
      },
      error: (error)=>{
        console.log(error);
      }
    });
  }

  public activateNotificationsForArtist(artist: GuidNameModel) {
    if (artist.receiveNotifications) {
      this.userService.RemoveNotficationsFromArtist(artist.id).subscribe({
        next: ()=>{
          this.getFollowedArtists();
          
        },
        error: (error)=>{
          console.log(error);
        }
      });

      return;
    }

    this.userService.ReceiveNotficationsFromArtist(artist.id).subscribe({
      next: ()=>{
        this.getFollowedArtists();
        this.updateDashBoard();
        
      },
      error: (error)=>{
        console.log(error);
      }
    });
  }

  public getFollowedArtists(): void{
    this.userService.GetSubscribedArtists(this.userId, '').subscribe({
      next: (artists: GuidNameModel[])=>{
        this.artists = artists;

      },
      error: (error)=>{
        console.log(error);
      }

    });
  }

  public getFollowedUsers(): void{
    this.userService.GetSubscribedUsers(this.userId, '').subscribe({
      next: (users: UserModel[])=>{
        this.users = users;

      },
      error: (error)=>{
        console.log(error);
      }

    });
  }

  public onGetPlaylists(page: number, take: number, sortAfter: string, asc: boolean, query: string): void{
    console.log("Get Playlists")
    this.rxjsStorageService.setSongTableLoadingState(true);
    this.playlistService.GetPlaylists(this.userId, page, take, query, sortAfter, asc).subscribe({
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

    this.onGetPlaylists(pModel.page, pModel.take, pModel.sortAfter, 
      pModel.asc, pModel.query);
  }

  public async createPlaylist(event: EditPlaylistModalParams): Promise<void> {
    this.ShowPlaylistEditModal = false;

    try {
      var playlistId = await lastValueFrom(this.playlistService.CreatePlaylist(event.playlistModel.name, event.playlistModel.description, event.playlistModel.isPublic,
        event.playlistModel.receiveNotifications));
  
        if (event.newCoverFile) {
          await lastValueFrom(this.fileService.ChangePlaylistCover(event.newCoverFile, playlistId));
        }

        this.onPaginationUpdated();
        this.updateDashBoard();
    } catch (error) {
      console.log(error);
      this.message.error("Error when creating playlist");
      
    }
  }

  public onPlaylistDropped(event: DragDropPlaylistParams) {
    console.log(event.srcPlaylist.order)
    console.log(event.destPlaylist.order)
    this.playlistService.ChangeOrderOfPlaylist(event.srcPlaylist.id, event.destPlaylist.order).subscribe({
      next: ()=>{
        this.onPaginationUpdated();
      },
      error: (error)=>{
        console.log(error);
      }
    })
  }

  private updateDashBoard(): void{
    var currenState = false;
    this.rxjsStorageService.updateDashboardBoolean$.subscribe((val) =>{
      currenState = val;
    })

    // Update value in rxjs so the dashboard gets updated
    this.rxjsStorageService.setUpdateDashboardBoolean(!currenState);
  }

  public getArtistPicSrc(id: number): string{
    return `${environment.apiUrl}/${APIROUTES.file}/artist/${id}`
  }

  public getProfilePicSrc(id: number): string{
    return `${environment.apiUrl}/${APIROUTES.file}/user/${id}`
  }

  public getUserCoverSrc(): string{
    return `${environment.apiUrl}/${APIROUTES.file}/user/${this.userId}?${this.timeStamp.getTime()}`
  }

  private updateProfilePicInBase(): void{
    let updateBoolean = false;
    this.rxjsStorageService.updateProfilePicBoolean$.subscribe(x =>{
      updateBoolean = x;
    })

    this.rxjsStorageService.setProfilePicBoolean(!updateBoolean);
  }

  public get UserModel(): UserModel{
    return this.userModel;
  }

  public get FileError(): string{
    return this.fileError;
  }

  public get FollowedArtists(): GuidNameModel[]{
    return this.artists;
  }

  public get FollowedUsers(): UserModel[]{
    return this.users;
  }

  public get PlaylistsModel(): PlaylistPaginationModel{
    return this.playlistsModel;
  }

  public get IsOwnUser(): boolean{
    return this.userId == '-1';
  }

  public get ShowPlaylistEditModal(): boolean{
    return this.showPlaylistEditModal;
  }

  public set ShowPlaylistEditModal(val: boolean){
    this.showPlaylistEditModal = val;
  }



}
