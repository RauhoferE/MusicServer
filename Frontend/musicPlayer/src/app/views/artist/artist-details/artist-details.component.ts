import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, ParamMap, Router } from '@angular/router';
import { NzMessageService } from 'ng-zorro-antd/message';
import { Subject, takeUntil } from 'rxjs';
import { APIROUTES } from 'src/app/constants/api-routes';
import { AlbumModel, ArtistShortModel } from 'src/app/models/artist-models';
import { QueueModel } from 'src/app/models/storage';
import { RxjsStorageService } from 'src/app/services/rxjs-storage.service';
import { SongService } from 'src/app/services/song.service';
import { UserService } from 'src/app/services/user.service';
import { environment } from 'src/environments/environment';
import { Clipboard } from '@angular/cdk/clipboard';

@Component({
  selector: 'app-artist-details',
  templateUrl: './artist-details.component.html',
  styleUrls: ['./artist-details.component.scss']
})
export class ArtistDetailsComponent implements OnInit, OnDestroy {

  public artistId: string = '';

  private artistModel: ArtistShortModel = {

  } as ArtistShortModel;

  private albums: AlbumModel[] = [];

  private isSongPlaying: boolean = false;

  private queueModel: QueueModel = undefined as any;

  private destroy:Subject<any> = new Subject();

  /**
   *
   */
  constructor(private route: ActivatedRoute, private rxjsService: RxjsStorageService, private songService: SongService,
    private userService: UserService,private clipboard: Clipboard,
    private message: NzMessageService ) {

      this.route.paramMap.pipe(takeUntil(this.destroy)).subscribe((params: ParamMap) => {
        if (!params.has('artistId')) {
          console.log("Artist id not found");
          return;
        }
    
        this.artistId = params.get('artistId') as string;
        this.getArtistDetails();
        this.getAlbumsOfArtist();

        this.rxjsService.isSongPlayingState.pipe(takeUntil(this.destroy)).subscribe(x => {
          this.isSongPlaying = x;
        });
    
        this.rxjsService.currentQueueFilterAndPagination.pipe(takeUntil(this.destroy)).subscribe(x => {
          this.queueModel = x;
        });
      });
  }
  
  ngOnDestroy(): void {
    this.destroy.next(true);
  }

  ngOnInit(): void {

  }

  private getAlbumsOfArtist(): void{
    this.songService.GetArtistAlbums(this.artistId).pipe(takeUntil(this.destroy)).subscribe({
      next:(albums: AlbumModel[])=>{
        this.albums = albums;
      },
      error:(error: any)=>{
        this.message.error("Error when getting artists.");
      }
    })
  }

  getArtistDetails(): void{
    this.songService.GetArtistDetails(this.artistId).pipe(takeUntil(this.destroy)).subscribe({
      next:(artistModel: ArtistShortModel)=>{
        this.artistModel = artistModel;
      },
      error:(error: any)=>{
        this.message.error("Error when getting artists.");
      }
    })
  }

  public activateNotifications(): void{

    if (this.artistModel.receiveNotifications) {
      this.userService.RemoveNotficationsFromArtist(this.artistId).subscribe({
        error: (error) =>{
          console.log(error)
        },
        complete: () => {
          this.getArtistDetails();
          // Get new model
        }
      })

      return;
    }

    this.userService.ReceiveNotficationsFromArtist(this.artistId).subscribe({
      error: (error) =>{
        console.log(error)
      },
      complete: () => {
        this.getArtistDetails();
        this.updateDashBoard();
        // Get new model
      }
    })

  }

  public subscribeToArtist(): void{

    if (this.artistModel.followedByUser) {
      this.userService.UnSuscribeFromArtist(this.artistId).subscribe({
        error: (error) =>{
          console.log(error)
        },
        complete: () => {
          this.getArtistDetails();
          this.updateDashBoard();
          // Get new model
        }
      })

      return;
    }


    this.userService.SuscribeToArtist(this.artistId).subscribe({
      error: (error) =>{
        console.log(error)
      },
      complete: () => {
        this.getArtistDetails();
        this.updateDashBoard();
        // Get new model
      }
    })

  }

  private updateDashBoard(): void{
    var currenState = false;
    this.rxjsService.updateDashboardBoolean$.subscribe((val) =>{
      currenState = val;
    })

    // Update value in rxjs so the dashboard gets updated
    this.rxjsService.setUpdateDashboardBoolean(!currenState);
  }

  async copyLinkToClipboard(): Promise<void> {
    await this.clipboard.copy(window.location.href);
    this.message.success("Link successfuly copied to clipboard!");
  }

  public getAlbumCoverSrc(): string{
    return `${environment.apiUrl}/${APIROUTES.file}/artist/${this.artistId}`
  }

  public get IsSongPlaying(): boolean{
    return this.isSongPlaying;
  }

  public get QueueModel(): QueueModel{
    return this.queueModel;
  }

  public get ArtistModel(): ArtistShortModel{
    return this.artistModel;
  }

  public get ArtistId(): string{
    return this.artistId;
  }

  public get Albums(): AlbumModel[]{
    return this.albums;
  }

}
