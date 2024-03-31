import { Injectable } from '@angular/core';
import { StreamingClientService } from './streaming-client.service';
import { QueueService } from './queue.service';

// This class is used to combine the queue service and the streaming service to call duplicate functions from a single service
@Injectable({
  providedIn: 'root'
})
export class QueueWrapperService {

  // This is used to check if the streaming service is running
  private groupName: string = '';

  constructor(private streamingService: StreamingClientService, private queueService: QueueService) 
  {
    this.streamingService.groupNameUpdated$.subscribe(x=>{
      this.groupName = x;
    });
  }


}
