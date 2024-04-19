import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { NzModalRef } from 'ng-zorro-antd/modal';
import { PlaylistUserShortModel } from 'src/app/models/playlist-models';

@Component({
  selector: 'app-share',
  templateUrl: './share.component.html',
  styleUrls: ['./share.component.scss']
})
export class ShareComponent implements OnInit {
  @Input() visible: boolean = false;

  @Input() playlistId: string = '';

  playlistDetails: PlaylistUserShortModel = {id: '-1', name: '', description: '', isPublic: false, receiveNotifications: false} as PlaylistUserShortModel;

  @Output() onCancleModal: EventEmitter<void> = new EventEmitter<void>();



  /**
   *
   */
  constructor() {
    
    
  }
  
  ngOnInit(): void {
    
  }

  cancel(): void{
    this.onCancleModal.emit();
    
  }

}
