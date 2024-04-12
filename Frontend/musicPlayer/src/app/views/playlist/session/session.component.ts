import { Component, OnDestroy, OnInit } from '@angular/core';
import { NzMessageService } from 'ng-zorro-antd/message';
import { Subject, takeUntil } from 'rxjs';
import { APIROUTES } from 'src/app/constants/api-routes';
import { SessionUserData } from 'src/app/models/hub-models';
import { QueueService } from 'src/app/services/queue.service';
import { RxjsStorageService } from 'src/app/services/rxjs-storage.service';
import { StreamingClientService } from 'src/app/services/streaming-client.service';
import { environment } from 'src/environments/environment';
import { Clipboard } from '@angular/cdk/clipboard';

@Component({
  selector: 'app-session',
  templateUrl: './session.component.html',
  styleUrls: ['./session.component.scss']
})
export class SessionComponent implements OnInit, OnDestroy {

  private destroy:Subject<any> = new Subject();
  private sessionId: string = '';
  private isMaster: boolean = true;
  private userList: SessionUserData[] = [];

  public sessionIdInput: string = ''

  /**
   *
   */
  constructor(private streamingService: StreamingClientService, private message: NzMessageService,
    private queueService: QueueService, private rxjsStorage: RxjsStorageService, 
    private clipboard: Clipboard) {
    
    
  }

  public ngOnInit(): void {
    this.streamingService.groupNameUpdated$.pipe(takeUntil(this.destroy)).subscribe(x=>{
      this.sessionId = x;
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
      console.log("New User joined", x.email);
    });



  }

  public ngOnDestroy(): void {
    this.destroy.next(true);
  }

  public async createSession(): Promise<void>{
    try {
      await this.streamingService.startSession();
    } catch (error) {
    }
    
  }

  public async joinSession(): Promise<void> {
    try {
      await this.streamingService.joinSession(this.sessionIdInput);
    } catch (error) {
      
    }
  }

  public async leaveSession(): Promise<void> {
    try {
      await this.streamingService.disconnect();  
    } catch (error) {
      
    }
    
  }

  public getAvatarSrc(userId: number): string{
    return `${environment.apiUrl}/${APIROUTES.file}/user/${userId}`;
  }

  public async copySessionIdToClipboard(): Promise<void>{
    await this.clipboard.copy(this.sessionId);
    this.message.success("Session Id successfuly copied to clipboard!");
  }

  public get Leader(): SessionUserData{
    return this.userList.find(x => x.isMaster) as SessionUserData;
  }

  public get SessionId(): string{
    return this.sessionId;
  }

  public get IsMaster(): boolean{
    return this.isMaster;
  }

  public get OtherUser(): SessionUserData[]{
    return this.userList.filter(x => !x.isMaster);
  }
}
