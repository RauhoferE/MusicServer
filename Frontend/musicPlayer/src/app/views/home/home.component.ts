import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subject, lastValueFrom, takeUntil } from 'rxjs';
import { CurrentMediaPlayerData } from 'src/app/models/hub-models';
import { QueueModel } from 'src/app/models/storage';
import { QueueService } from 'src/app/services/queue.service';
import { RxjsStorageService } from 'src/app/services/rxjs-storage.service';
import { StreamingClientService } from 'src/app/services/streaming-client.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit, OnDestroy {



  private destroy:Subject<any> = new Subject();
  private groupName: string = '';
  private isMaster: boolean = true;
  private userList: string[] = [];

  public groupNameInput: string = ''

  /**
   *
   */
  constructor(private streamingService: StreamingClientService, private queueService: QueueService, private rxjsStorage: RxjsStorageService) {
    
    
  }

  ngOnInit(): void {
    this.streamingService.groupNameUpdated$.pipe(takeUntil(this.destroy)).subscribe(x=>{
      this.groupName = x;
    });

    this.streamingService.usersUpdated$.pipe(takeUntil(this.destroy)).subscribe(x=>{
      this.userList = x;
    });

    this.streamingService.isMasterUpdated$.pipe(takeUntil(this.destroy)).subscribe(x=>{
      this.isMaster = x;
    });

    this.streamingService.groupDeletedEvent.pipe(takeUntil(this.destroy)).subscribe(() =>{
      console.log("Group has been deleted");
    });

    this.streamingService.userJoinedEvent.pipe(takeUntil(this.destroy)).subscribe(x=>{
      console.log("New User joined", x);
    });

    this.streamingService.connectionClosedEvent.pipe(takeUntil(this.destroy)).subscribe(async ()=>{
      try {
        var song = await lastValueFrom(this.queueService.GetCurrentSong());
        this.rxjsStorage.setCurrentPlayingSong(song);
        this.rxjsStorage.showMediaPlayer(true);
      } catch (error) {
        this.rxjsStorage.setCurrentPlayingSong({} as any);
        this.rxjsStorage.showMediaPlayer(false);
      }

      try {
        var data = await lastValueFrom(this.queueService.GetQueueData());
        this.rxjsStorage.setQueueFilterAndPagination(data);
      } catch (error) {
        this.rxjsStorage.setQueueFilterAndPagination({} as any);
      }
      
    });

  }

  ngOnDestroy(): void {
    this.destroy.next(true);
  }

  async createSession(){
    await this.streamingService.startSession();
  }

  async joinSession() {
    console.log(this.groupNameInput)
    await this.streamingService.joinSession(this.groupNameInput);
  }

  async leaveSession() {
    await this.streamingService.disconnect();
  }

  public get GroupName(){
    return this.groupName;
  }

  public get IsMaster(){
    return this.isMaster;
  }

  public get Emails(){
    return this.userList;
  }

}
