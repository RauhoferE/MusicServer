import { Injectable } from '@angular/core';
import { StreamingClientService } from './streaming-client.service';
import { QueueService } from './queue.service';
import { Observable, lastValueFrom } from 'rxjs';
import { RxjsStorageService } from './rxjs-storage.service';
import { PlaylistSongModel, QueueSongModel } from '../models/playlist-models';

// This class is used to combine the queue service and the streaming service to call duplicate functions from a single service
@Injectable({
  providedIn: 'root'
})
export class QueueWrapperService {

  // This is used to check if the streaming service is running
  private groupName: string = '';

  constructor(private streamingService: StreamingClientService, private queueService: QueueService, private rxjsService: RxjsStorageService) 
  {
    this.streamingService.groupNameUpdated$.subscribe(x=>{
      this.groupName = x;
    });
  }

  public async CreateQueueFromAlbum(albumId: string, randomize: boolean, loopMode: string, playFromIndex: number): Promise<void>{
    if (this.groupName == '') {
      var currentSong = await lastValueFrom(this.queueService.CreateQueueFromAlbum(albumId, randomize,loopMode, playFromIndex));
      this.rxjsService.setCurrentPlayingSong(currentSong);
      //this.rxjsService.setIsSongPlaylingState(this.loopMode == this.LoopModePlaylist&& this.isSongPlaying);
      this.updateQueue();
      return;
    }

    await this.streamingService.createQueueFromAlbum(albumId, randomize, loopMode, playFromIndex);
  }

  public async CreateQueueFromSingleSong(songId: string, randomize: boolean, loopMode: string): Promise<void>{
    if (this.groupName == '') {
      var currentSong = await lastValueFrom(this.queueService.CreateQueueFromSingleSong(songId, randomize, loopMode));
      this.rxjsService.setCurrentPlayingSong(currentSong);
      //this.rxjsService.setIsSongPlaylingState(this.loopMode == this.LoopModePlaylist&& this.isSongPlaying);
      this.updateQueue();
      return;
    }
  }

  public async CreateQueueFromPlaylist(playlistId: string, randomize: boolean,loopMode: string, sortAfter: string, asc: boolean, playFromOrder: number): Promise<void>{
    if (this.groupName == '') {
      var currentSong = await lastValueFrom(this.queueService.CreateQueueFromPlaylist(playlistId, randomize,loopMode, sortAfter, asc, playFromOrder));
      this.rxjsService.setCurrentPlayingSong(currentSong);
      //this.rxjsService.setIsSongPlaylingState(this.loopMode == this.LoopModePlaylist&& this.isSongPlaying);
      this.updateQueue();
      return;
    }

    await this.streamingService.createQueueFromPlaylist(playlistId, randomize, loopMode, sortAfter, asc, playFromOrder);
  }

  public async CreateQueueFromFavorites(randomize: boolean,loopMode: string, sortAfter: string, asc: boolean, playFromOrder: number): Promise<void>{

    if (this.groupName == '') {
      var currentSong = await lastValueFrom(this.queueService.CreateQueueFromFavorites(randomize,loopMode, sortAfter, asc, playFromOrder));
      this.rxjsService.setCurrentPlayingSong(currentSong);
      this.updateQueue();
      // This method call is outside of the method
      //this.rxjsService.setIsSongPlaylingState(this.loopMode == this.LoopModePlaylist && this.isSongPlaying);
      
      return;
    }

    await this.streamingService.createQueueFromFavorites(randomize, loopMode, sortAfter, asc, playFromOrder);
  }

  public async AddAlbumToQueue(albumId: string): Promise<void>{
    if (this.groupName == '') {
      await lastValueFrom(this.queueService.AddAlbumToQueue(albumId));
      //this.updateQueue();
      // This method call is outside of the method
      //this.rxjsService.setIsSongPlaylingState(this.loopMode == this.LoopModePlaylist && this.isSongPlaying);
      
      return;
    }

    await this.streamingService.addAlbumToQueue(albumId);
  }

  public async AddPlaylistToQueue(playlistId: string): Promise<void>{
    if (this.groupName == '') {
      await lastValueFrom(this.queueService.AddPlaylistToQueue(playlistId));
      //this.updateQueue();
      // This method call is outside of the method
      //this.rxjsService.setIsSongPlaylingState(this.loopMode == this.LoopModePlaylist && this.isSongPlaying);
      
      return;
    }

    await this.streamingService.addPlaylistToQueue(playlistId);
  }

  public async ChangeQueue(randomize: boolean): Promise<void>{
    if (this.groupName == '') {
      await lastValueFrom(this.queueService.ChangeQueue(randomize));
      //this.updateSongTable();
      return;
    }

    await this.streamingService.randomizeQueue(randomize);
  }

  public async GetCurrentQueue(): Promise<QueueSongModel[]>{
    if (this.groupName == '') {
      return await lastValueFrom(this.queueService.GetCurrentQueue());
      //this.updateSongTable();
    }

    await this.streamingService.getCurrentSongQueue();
    return [];
  }

  public async GetCurrentSong(): Promise<PlaylistSongModel>{
    if (this.groupName == '') {
      return await lastValueFrom(this.queueService.GetCurrentSong());
      //this.updateSongTable();
    }

    await this.streamingService.getCurrentSongQueue();
    return {id: '-1'} as PlaylistSongModel;
  }

  public async ClearQueue(): Promise<void>{
    if (this.groupName == '') {
      await lastValueFrom(this.queueService.ClearQueue());
      // This method call is outside of the method
      //this.rxjsService.setIsSongPlaylingState(this.loopMode == this.LoopModePlaylist && this.isSongPlaying);
      
      return;
    }

    await this.streamingService.clearQueue();
  }

  public async SkipForwardInQueue(index: number): Promise<void>{
    if (this.groupName == '') {
      var nextSong = await lastValueFrom(this.queueService.SkipForwardInQueue(index));
      this.rxjsService.setCurrentPlayingSong(nextSong);
      this.updateQueue();
      return;
    }

    await this.streamingService.skipForwardInQueue(index);
  }

  public async SkipBackInQueue(): Promise<void>{
    if (this.groupName == '') {
      var lastPlayedSong = await lastValueFrom(this.queueService.SkipBackInQueue());
      this.rxjsService.setCurrentPlayingSong(lastPlayedSong);
      this.updateQueue();
      return;
    }

    await this.streamingService.skipBackInQueue();
  }

  public async RemoveSongsFromQueue(indexList: number[]): Promise<void>{
    if (this.groupName == '') {
      await lastValueFrom(this.queueService.RemoveSongsFromQueue(indexList));
      this.updateQueue();
      // This method call is outside of the method
      //this.rxjsService.setIsSongPlaylingState(this.loopMode == this.LoopModePlaylist && this.isSongPlaying);
      
      return;
    }

    await this.streamingService.removeSongsInQueue(indexList);
  }

  public async AddSongsToQueue(ids: string[]): Promise<void>{
    if (this.groupName == '') {
      await lastValueFrom(this.queueService.AddSongsToQueue(ids));
      this.updateQueue();
      // This method call is outside of the method
      //this.rxjsService.setIsSongPlaylingState(this.loopMode == this.LoopModePlaylist && this.isSongPlaying);
      
      return;
    }

    await this.streamingService.addSongsToQueue(ids);
  }

  public async PushSongInQueue(srcIndex: number, targetIndex: number, markAsManuallyAdded: number): Promise<QueueSongModel[]>{
    if (this.groupName == '') {
      return await lastValueFrom(this.queueService.PushSongInQueue(srcIndex, targetIndex, markAsManuallyAdded));
      //this.updateSongTable();
    }

    await this.streamingService.pushSongInQueue(srcIndex, targetIndex, markAsManuallyAdded);
    return [];
  }

  public async GetQueueData(): Promise<void>{

  }

  public async UpdateLoopMode(loopMode: string): Promise<void>{
    if (this.groupName == '') {
      await lastValueFrom(this.queueService.UpdateLoopMode(loopMode));
      return;
    }

    await this.streamingService.updateLoopMode(loopMode);
  }

  private updateQueue(): void{
    let queueBool = false;
    this.rxjsService.updateQueueBoolean$.subscribe(x => {
      queueBool = x;
    });

    this.rxjsService.setUpdateQueueBoolean(!queueBool);
  }

  private updateSongTable(): void{
    let tableBool = false;
    this.rxjsService.updateCurrentTableBoolean$.subscribe(x => {
      tableBool = x;
    });

    this.rxjsService.setUpdateCurrentTableBoolean(!tableBool);
  }
}
