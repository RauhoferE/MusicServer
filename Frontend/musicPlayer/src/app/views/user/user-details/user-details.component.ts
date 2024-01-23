import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NzMessageService } from 'ng-zorro-antd/message';
import { Subject, takeUntil } from 'rxjs';
import { APIROUTES } from 'src/app/constants/api-routes';
import { GuidNameModel, PlaylistPaginationModel } from 'src/app/models/playlist-models';
import { PaginationModel } from 'src/app/models/storage';
import { UserModel } from 'src/app/models/user-models';
import { PlaylistService } from 'src/app/services/playlist.service';
import { RxjsStorageService } from 'src/app/services/rxjs-storage.service';
import { UserService } from 'src/app/services/user.service';
import { environment } from 'src/environments/environment';

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
  private artists: GuidNameModel[] = [];
  private users: GuidNameModel[] = [];
  private newProfilePic: File = {} as File;
  private fileError: string = '';

  private userModel: UserModel = {
    id: -1

  } as UserModel;

  /**
   *
   */ 
  constructor(private route: ActivatedRoute, private message: NzMessageService, 
    private rxjsStorageService: RxjsStorageService, private playlistService: PlaylistService, private userService: UserService) {
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
    if (this.userId != '-1') {
      this.getUserModel();
    }

    this.onPaginationUpdated();

    this.userService.GetSubscribedArtists(this.userId, '').pipe(takeUntil(this.destroy)).subscribe({
      next: (artists: GuidNameModel[])=>{
        this.artists = artists;

      },
      error: (error)=>{
        console.log(error);
      }

    })

    this.userService.GetSubscribedUsers(this.userId, '').pipe(takeUntil(this.destroy)).subscribe({
      next: (users: GuidNameModel[])=>{
        this.users = users;

      },
      error: (error)=>{
        console.log(error);
      }

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

  subscribeToUser(): void {
    if (this.userId == '-1') {
      return;
    }

    if (this.userModel.isFollowedByUser) {
      this.userService.SuscribeToUser(this.userId).subscribe({
        error: (error)=>{
          console.log(error);
        },
        complete: ()=>{
          this.getUserModel();
          this.updateDashBoard();
        }
      });
      return;
    }

    this.userService.UnSubscribeFromUser(this.userId).subscribe({
      error: (error)=>{
        console.log(error);
      },
      complete: ()=>{
        this.getUserModel();
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

  public uploadNewCover(): void{
    console.log("Upload Picture");
    console.log(this.newProfilePic)
    // TODO: Test and then upload the new picture
    // Reload the previous picture and set the file as empty
  }

  public onGetPlaylists(page: number, take: number, sortAfter: string, asc: boolean, query: string): void{
    console.log("Get Playlists")
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

    this.onGetPlaylists(pModel.page, pModel.take, pModel.sortAfter, 
      pModel.asc, pModel.query);
  }

  updateDashBoard(): void{
    var currenState = false;
    this.rxjsStorageService.updateDashboardBoolean$.subscribe((val) =>{
      currenState = val;
    })

    // Update value in rxjs so the dashboard gets updated
    this.rxjsStorageService.setUpdateDashboardBoolean(!currenState);
  }

  public getUserCoverSrc(): string{
    return `${environment.apiUrl}/${APIROUTES.file}/user/${this.userId}`
  }

  public get UserModel(): UserModel{
    return this.userModel;
  }

  public get FileError(): string{
    return this.fileError;
  }



}
