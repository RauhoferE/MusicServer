import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subject, takeUntil } from 'rxjs';
import { SessionUserData } from 'src/app/models/hub-models';
import { QueueService } from 'src/app/services/queue.service';
import { RxjsStorageService } from 'src/app/services/rxjs-storage.service';
import { StreamingClientService } from 'src/app/services/streaming-client.service';

@Component({
  selector: 'app-session',
  templateUrl: './session.component.html',
  styleUrls: ['./session.component.scss']
})
export class SessionComponent implements OnInit, OnDestroy {

  private destroy:Subject<any> = new Subject();
  private groupName: string = '';
  private isMaster: boolean = true;
  private userList: SessionUserData[] = [];

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

  public get Leader(): SessionUserData{
    return this.userList.find(x => x.isMaster) as SessionUserData;
  }

  public get GroupName(): string{
    return this.groupName;
  }

  public get IsMaster(): boolean{
    return this.isMaster;
  }

  public get OtherUser(): SessionUserData[]{
    return this.userList.filter(x => !x.isMaster);
  }
}
