import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { APIROUTES } from 'src/app/constants/api-routes';
import { EditPlaylistModalParams } from 'src/app/models/events';
import { PlaylistUserShortModel } from 'src/app/models/playlist-models';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-playlist-edit',
  templateUrl: './playlist-edit.component.html',
  styleUrls: ['./playlist-edit.component.scss']
})
export class PlaylistEditComponent {

  @Input() playlistDetails: PlaylistUserShortModel = {id: '-1', name: '', description: '', isPublic: false, receiveNotifications: false} as PlaylistUserShortModel;

  @Input() visible: boolean = false;

  public newCoverFile: File = undefined as any;

  @Output() onCancleModal: EventEmitter<void> = new EventEmitter<void>();

  @Output() onSaveModal: EventEmitter<EditPlaylistModalParams> = new EventEmitter<EditPlaylistModalParams>();

  private possibleFileSrc: any = undefined;

  private fileError: string = '';

  public getPlaylistCoverSrc(id: string): string {
    if (this.possibleFileSrc) {
      return this.possibleFileSrc;
    }

    return `${environment.apiUrl}/${APIROUTES.file}/playlist/${id}`;
  }

  public savePlaylist(): void {
    this.onSaveModal.emit({playlistModel: this.playlistDetails, newCoverFile: this.newCoverFile});
  }

  public canceled():void {
    this.onCancleModal.emit();
  }

  public setNewCoverfile(fileEvent: Event): void{
    const input = fileEvent.target as HTMLInputElement;

    if (input.files == null || input.files.length == 0) {
      return;
    }

    console.log("Upload Picture");
    console.log(input.files[0]);

    const targetFile = input.files[0];

    if (targetFile.name.slice((targetFile.name.lastIndexOf(".") - 1 >>> 0) + 2) != 'png') {
      this.fileError = 'Only png files are accepted!';
      return;
    }

    if (targetFile.size > 1000000) {
      this.fileError = 'Max file size is 1 mb!';
      return;
    }

    this.fileError = '';

    this.newCoverFile = targetFile;

    const reader = new FileReader();
    reader.onload = (e) => {
      this.possibleFileSrc = e.target?.result;
    };
    reader.readAsDataURL(this.newCoverFile);
  }

  public get FileError(): string{
    return this.fileError;
  }

}
