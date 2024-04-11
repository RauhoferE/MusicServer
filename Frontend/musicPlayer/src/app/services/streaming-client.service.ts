import { EventEmitter, Injectable, OnInit } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { environment } from 'src/environments/environment';
import { HUBEMITS, HUBINVOKES } from '../constants/hub-routes';
import { CurrentMediaPlayerData, SessionUserData } from '../models/hub-models';
import { BehaviorSubject, Observable, Subject, firstValueFrom, lastValueFrom } from 'rxjs';
import { PlaylistSongModel, QueueSongModel } from '../models/playlist-models';
import { RxjsStorageService } from './rxjs-storage.service';
import { QueueModel } from '../models/storage';
import { MediaPlayerParams, MediaPlayerProgressParams } from '../models/events';

@Injectable({
  providedIn: 'root'
})
export class StreamingClientService {
  private hubConnection: signalR.HubConnection;

  private groupName = new BehaviorSubject<string>('');
  groupNameUpdated$: Observable<string> = this.groupName.asObservable();

  private groupNameProp: string = '';

  private isMaster = new BehaviorSubject<boolean>(true);
  isMasterUpdated$: Observable<boolean> = this.isMaster.asObservable();

  private isMasterProp: boolean = true;

  private users = new BehaviorSubject<SessionUserData[]>([]);
  usersUpdated$: Observable<SessionUserData[]> = this.users.asObservable();

  private usersProp: SessionUserData[] = [];

  private model: QueueModel = {} as QueueModel;

  public songProgressUpdated = new EventEmitter<MediaPlayerProgressParams>();

  public updateQueueEvent = new EventEmitter<void>();

  public groupDeletedEvent = new EventEmitter<void>();

  public userJoinedEvent = new EventEmitter<string>();

  // IF the connection closed so you can get the current song and queue from the queue controller.
  public connectionClosedEvent = new EventEmitter<void>();

  public queueDataUpdated = new EventEmitter<MediaPlayerParams>();

  public playPauseStateUpdated = new EventEmitter<boolean>();

  public queueItemsReceivedEvents = new EventEmitter<QueueSongModel[]>();

  public errorReceivedEvent = new EventEmitter<string>();


  constructor(private rxjsStorage: RxjsStorageService) { 
    this.hubConnection = new signalR.HubConnectionBuilder()
    .withUrl(`${environment.hubUrl}`) // Replace with your SignalR hub URL
    .build();

    this.hubConnection.on(HUBEMITS.receiveCurrentPlayingSong, x =>{
      this.rxjsStorage.setCurrentPlayingSong(x);
    });

    this.hubConnection.on(HUBEMITS.receiveGroupName, x =>{
      this.groupName.next(x);
    });

    this.hubConnection.on(HUBEMITS.userJoinedSession, email=>{
      let users = this.usersProp;
      users.push(email);
      this.users.next(users);
      this.userJoinedEvent.emit(email);
    });

    this.hubConnection.on(HUBEMITS.receiveUserList, users =>{
      this.users.next(users);
    });

    this.hubConnection.on(HUBEMITS.userDisconnected, (email)=>{
      console.log("Disconnected")
      let users = this.usersProp;
      const index = users.findIndex(x=> x == email);
      if (index > -1) {
        users.splice(index, 1);
      }
      this.users.next(users);
    });

    this.hubConnection.on(HUBEMITS.groupDeleted, ()=>{
      this.groupName.next('');
      this.users.next([]);
      this.isMaster.next(true);
      this.disconnect().then(()=>this.groupDeletedEvent.emit());
    });

    this.hubConnection.on(HUBEMITS.receiveSongProgress, (isSongPlaying: boolean, secondsPlayed: number)=>{
      this.songProgressUpdated.emit({isSongPlaying: isSongPlaying, secondsPlayed: secondsPlayed});
    });

    this.hubConnection.on(HUBEMITS.updateQueueView, ()=>{
      this.updateQueueEvent.emit();
    });

    this.hubConnection.on(HUBEMITS.updateCurrentSong, ()=>{
      this.getCurrentSong();
    });

    this.hubConnection.on(HUBEMITS.receiveQueueData, (data: QueueModel)=>{
      console.log("Receive queue data", data);
      let oldModel = this.model;
      oldModel.random = data.random;
      oldModel.asc = data.asc;
      oldModel.target = data.target;
      oldModel.loopMode = data.loopMode;
      oldModel.sortAfter = data.sortAfter;
      oldModel.itemId = data.itemId;
      oldModel.userId = data.userId;
      this.rxjsStorage.setQueueFilterAndPagination(oldModel);
      this.queueDataUpdated.emit({random: oldModel.random, loopMode: oldModel.loopMode});
    });

    this.hubConnection.on(HUBEMITS.receivePlayPauseSongState, (state: boolean)=>{
      this.playPauseStateUpdated.emit(state);
    });

    this.hubConnection.on(HUBEMITS.receiveQueueEntities, (models: QueueSongModel[])=>{
      this.queueItemsReceivedEvents.emit(models);
    })

    this.hubConnection.on(HUBEMITS.receiveErrorMessage, (msg: string)=>{
      this.errorReceivedEvent.emit(msg);
    });

    this.usersUpdated$.subscribe(x=>{
      this.usersProp = x;
    })

    this.isMasterUpdated$.subscribe(x=>{
      this.isMasterProp = x;
    })

    this.groupNameUpdated$.subscribe(x=>{
      this.groupNameProp = x;
    })

    this.rxjsStorage.currentQueueFilterAndPagination.subscribe((x: QueueModel) => {
      this.model = x;
    });
  }

  async startSession(): Promise<void>{
    if (this.groupNameProp != '') {
      return;
    }

    await this.hubConnection.start();

    await this.hubConnection.invoke(HUBINVOKES.createSession);
  }

  async joinSession(groupId: string): Promise<void> {
    if (this.groupNameProp == groupId) {
      return;
    }

    if (this.hubConnection.state == signalR.HubConnectionState.Connected) {
      await this.hubConnection
      .stop();
    }

    await this.hubConnection
    .start();

    await this.hubConnection.invoke(HUBINVOKES.joinSession, groupId);
    this.isMaster.next(false);
  }

  async leaveSession(): Promise<void>{
    await this.hubConnection.stop();
    this.isMaster.next(true);
    this.groupName.next('');

    await this.startSession();
  }

  async disconnect(): Promise<void>{
    await this.hubConnection
    .stop();
    console.log('Disconnected SignalR hub')
    this.isMaster.next(true);
    this.groupName.next('');
    this.connectionClosedEvent.emit();
  }

  async sendCurrentSongProgressToNewUser(isSongPlaying: boolean, secondsPlayed: number): Promise<void>{
    //Only send to new user if the current user is master of the session
    if (!this.isMaster || this.groupNameProp == '') {
      return;
    }

    await this.hubConnection.invoke(HUBINVOKES.sendCurrentSongProgress, this.groupNameProp, isSongPlaying, secondsPlayed);
  }

  async sendCurrentSongProgress(isSongPlaying: boolean, secondsPlayed: number): Promise<void>{
    if (this.groupNameProp == '') {
      return;
    }

    await this.hubConnection.invoke(HUBINVOKES.sendCurrentSongProgress, this.groupNameProp, isSongPlaying, secondsPlayed);
  }

  async getCurrentSongQueue(): Promise<void>{
    if (this.groupNameProp == '') {
      return;
    }

    await this.hubConnection.invoke(HUBINVOKES.getCurrentSongQueue, this.groupNameProp);
  }

  async skipBackInQueue(): Promise<void>{
    if (this.groupNameProp == '') {
      return;
    }

    await this.hubConnection.invoke(HUBINVOKES.skipBackInQueue, this.groupNameProp);
  }

  async skipForwardInQueue(index: number = 0): Promise<void>{
    if (this.groupNameProp == '') {
      return;
    }

    await this.hubConnection.invoke(HUBINVOKES.skipForwardInQueue, this.groupNameProp, index);
  }

  async clearQueue(): Promise<void>{
    if (this.groupNameProp == '') {
      return;
    }

    await this.hubConnection.invoke(HUBINVOKES.clearQueue, this.groupNameProp);
  }

  async addSongsToQueue(songIds: string[]): Promise<void>{
    if (this.groupNameProp == '') {
      return;
    }

    await this.hubConnection.invoke(HUBINVOKES.addSongsToQueue, this.groupNameProp, songIds);
  }

  async removeSongsInQueue(orderIds: number[]): Promise<void>{
    if (this.groupNameProp == '') {
      return;
    }

    await this.hubConnection.invoke(HUBINVOKES.removeSongsInQueue,this.groupNameProp, orderIds);
  }

  async pushSongInQueue(srcIndex: number, targetIndex: number, markAsAddedManually: number = -1): Promise<void>{
    if (this.groupNameProp == '') {
      return;
    }

    await this.hubConnection.invoke(HUBINVOKES.pushSongInQueue, this.groupNameProp, srcIndex, targetIndex, markAsAddedManually);
  }

  async getCurrentSong(): Promise<void>{
    if (this.groupNameProp == '') {
      return;
    }

    await this.hubConnection.invoke(HUBINVOKES.getCurrentSong, this.groupNameProp);
  }

  async getQueueData(): Promise<void>{
    if (this.groupNameProp == '') {
      return;
    }

    await this.hubConnection.invoke(HUBINVOKES.getQueueData, this.groupNameProp);
  }

  async updateLoopMode(loopMode: string): Promise<void>{
    if (this.groupNameProp == '') {
      return;
    }

    await this.hubConnection.invoke(HUBINVOKES.updateLoopMode, this.groupNameProp, loopMode);
  }

  async randomizeQueue(randomize: boolean): Promise<void>{
    if (this.groupNameProp == '') {
      return;
    }

    await this.hubConnection.invoke(HUBINVOKES.randomizeQueue, this.groupNameProp, randomize);
  }

  async createQueueFromAlbum(albumId: string, randomize: boolean, loopMode: string, playFromIndex: number = 0): Promise<void>{
    if (this.groupNameProp == '') {
      return;
    }

    await this.hubConnection.invoke(HUBINVOKES.createQueueFromAlbum, this.groupNameProp, albumId, randomize, loopMode, playFromIndex);
  }

  async createQueueFromPlaylist(playlistId: string, randomize: boolean, loopMode: string, sortAfter: string, asc: boolean = true, playFromOrder: number = 0): Promise<void>{
    if (this.groupNameProp == '') {
      return;
    }

    await this.hubConnection.invoke(HUBINVOKES.createQueueFromPlaylist, this.groupNameProp, playlistId, randomize, loopMode, sortAfter, asc, playFromOrder);
  }

  async createQueueFromFavorites(randomize: boolean, loopMode: string, sortAfter: string, asc: boolean = true, playFromOrder: number = 0): Promise<void>{
    if (this.groupNameProp == '') {
      return;
    }

    await this.hubConnection.invoke(HUBINVOKES.createQueueFromFavorites, this.groupNameProp, randomize, loopMode, sortAfter, asc, playFromOrder);
  }

  async createQueueFromSingleSong(songId: string, randomize: boolean, loopMode: string,): Promise<void>{
    if (this.groupNameProp == '') {
      return;
    }

    await this.hubConnection.invoke(HUBINVOKES.createQueueFromSingleSong, this.groupNameProp, songId, randomize, loopMode);
  }

  async addAlbumToQueue(albumId: string): Promise<void>{
    if (this.groupNameProp == '') {
      return;
    }

    await this.hubConnection.invoke(HUBINVOKES.addAlbumToQueue, this.groupNameProp, albumId);
  }

  async addPlaylistToQueue(playlistId: string): Promise<void>{
    if (this.groupNameProp == '') {
      return;
    }
    
    await this.hubConnection.invoke(HUBINVOKES.addPlaylistToQueue, this.groupNameProp, playlistId);
  }

  async playPauseSong(state: boolean): Promise<void>{
    if (this.groupNameProp == '') {
      return;
    }
    
    await this.hubConnection.invoke(HUBINVOKES.playPauseSong, this.groupNameProp, state);
  }







  
}
