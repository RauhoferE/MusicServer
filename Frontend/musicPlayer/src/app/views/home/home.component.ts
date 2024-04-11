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




  }

  ngOnDestroy(): void {
    this.destroy.next(true);
  }

}
