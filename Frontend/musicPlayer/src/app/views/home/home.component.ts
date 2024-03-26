import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subject, takeUntil } from 'rxjs';
import { CurrentMediaPlayerData } from 'src/app/models/hub-models';
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
  private playerData: CurrentMediaPlayerData = {} as CurrentMediaPlayerData;

  public groupNameInput: string = ''

  /**
   *
   */
  constructor(private streamingService: StreamingClientService) {
    
    
  }

  ngOnInit(): void {
    this.streamingService.groupNameUpdated$.pipe(takeUntil(this.destroy)).subscribe(x=>{
      this.groupName = x;
    });

    this.streamingService.usersUpdated$.pipe(takeUntil(this.destroy)).subscribe(x=>{
      this.userList = x;
    });

    this.streamingService.playerDataUpdated$.pipe(takeUntil(this.destroy)).subscribe(x=>{
      this.playerData = x;
      console.log(x);
    });

    this.streamingService.groupDeletedEvent.pipe(takeUntil(this.destroy)).subscribe(x =>{
      this.isMaster = true;

    });

    this.streamingService.queueUpdated$.pipe(takeUntil(this.destroy)).subscribe(x=>{
      console.log(x)
    })


  }

  ngOnDestroy(): void {
    this.destroy.next(true);
  }

  async joinSession() {
    console.log(this.groupNameInput)
    await this.streamingService.joinSession(this.groupNameInput);
    this.isMaster = false;
  }

  async leaveSession() {
    await this.streamingService.leaveGroup(this.groupName);
    this.isMaster = true;
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
