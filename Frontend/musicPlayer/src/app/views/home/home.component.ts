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



  }

  ngOnDestroy(): void {
    this.destroy.next(true);
  }

  async createSession(){
    try {
      await this.streamingService.startSession();
    } catch (error) {
    }
    
  }

  async joinSession() {
    try {
      await this.streamingService.joinSession(this.groupNameInput);
    } catch (error) {
      
    }
  }

  async leaveSession() {
    try {
      await this.streamingService.disconnect();  
    } catch (error) {
      
    }
    
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
