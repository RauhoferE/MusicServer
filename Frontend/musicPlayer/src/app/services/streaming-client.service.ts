import { EventEmitter, Injectable, OnInit } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { environment } from 'src/environments/environment';
import { HUBEMITS, HUBINVOKES } from '../constants/hub-routes';
import { CurrentMediaPlayerData } from '../models/hub-models';
import { BehaviorSubject, Observable, Subject, firstValueFrom, lastValueFrom } from 'rxjs';
import { PlaylistSongModel, QueueSongModel } from '../models/playlist-models';
import { RxjsStorageService } from './rxjs-storage.service';
import { QueueModel } from '../models/storage';

@Injectable({
  providedIn: 'root'
})
export class StreamingClientService {
  private hubConnection: signalR.HubConnection;

  private groupName = new Subject<string>();
  groupNameUpdated$: Observable<string> = this.groupName.asObservable();

  private groupNameProp: string = '';

  private isMaster = new Subject<boolean>();
  isMasterUpdated$: Observable<boolean> = this.isMaster.asObservable();

  private isMasterProp: boolean = true;

  private users = new Subject<string[]>();
  usersUpdated$: Observable<string[]> = this.users.asObservable();

  private usersProp: string[] = [];

  private playerData = new Subject<CurrentMediaPlayerData>();
  playerDataUpdated$: Observable<CurrentMediaPlayerData> = this.playerData.asObservable();

  private playerDataProp: CurrentMediaPlayerData = {}as CurrentMediaPlayerData;

  private queue = new Subject<QueueSongModel[]>();
  queueUpdated$: Observable<QueueSongModel[]> = this.queue.asObservable();

  private queueProp: QueueSongModel[] = [];

  private currentPlayingSong = new Subject<PlaylistSongModel>();
  songUpdated$: Observable<PlaylistSongModel> = this.currentPlayingSong.asObservable();

  private currentPlayingSongProp: PlaylistSongModel = {} as PlaylistSongModel;

  public groupDeletedEvent = new EventEmitter<void>();

  public userJoinedEvent = new EventEmitter<string>();


  constructor(private rxjsStorage: RxjsStorageService) { 
    this.hubConnection = new signalR.HubConnectionBuilder()
    .withUrl(`${environment.hubUrl}`) // Replace with your SignalR hub URL
    .build();

    this.hubConnection.on(HUBEMITS.getGroupName, (groupName: string)=>{
      this.groupName.next(groupName);
      this.users.next([]);
      console.log("Connected with: ", groupName);
    });

    this.hubConnection.on(HUBEMITS.userJoinedSession, (email: string)=>{
      console.log("User joined: ", email);
      var groupName = this.groupNameProp;
      var playerUpdate = this.playerDataProp;
      var users = this.usersProp;
      users.push(email);
      this.users.next(users);
      console.log("Users: ", users);
      this.userJoinedEvent.emit(email);
      //this.sendCurrentSongToJoinedUser(groupName, email, playerUpdate)
    });


    this.hubConnection.on(HUBEMITS.getUserList, (users: string[])=>{
      this.users.next(users);
    })

    this.hubConnection.on(HUBEMITS.userDisconnected, (email: string)=>{
      console.log("User disconnected: ", email);
      //var users = await lastValueFrom(this.users);
      var users = this.usersProp;
      var indexToRemove = users.indexOf(email);
      if (indexToRemove > -1) {
        users = users.splice(indexToRemove, 1)
      }
      // Remove groupname
      this.users.next(users);
    });

    this.hubConnection.on(HUBEMITS.groupDeleted, ()=>{
      console.log("Group deleted: ");
      this.users.next([]);
      
      // Get current playing song, queue and data
      this.groupDeletedEvent.emit();
    });

    this.hubConnection.on(HUBEMITS.getPlayerData, (data: CurrentMediaPlayerData)=>{
      console.log("Receiving player data: ", data);
      this.playerData.next(data);
      // this.rxjsStorage.setQueueFilterAndPagination({
      //   itemId: data.itemId,
      //   loopMode: data.loopMode,


      // } as QueueModel)
    });

    this.hubConnection.on(HUBEMITS.getQueue, (queue: QueueSongModel[])=>{
      console.log("Get queue: ", queue);
      this.queue.next(queue);
    });

    this.hubConnection.on(HUBEMITS.getCurrentPlayingSong, (song: PlaylistSongModel)=>{
      console.log("Get current plaing song: ", song);
      this.currentPlayingSong.next(song);
      //var playerData = await lastValueFrom(this.playerData);
      var playerData = this.playerDataProp;
      this.rxjsStorage.setCurrentPlayingSong(song);
      playerData.secondsPlayed = 0;
      this.playerData.next(playerData);
    });

    this.rxjsStorage.currentQueueFilterAndPagination.subscribe((x: QueueModel) => {
      var playerData = this.playerDataProp;
      playerData.itemId = x.itemId;
      playerData.loopMode = x.loopMode;
      playerData.random = x.random;
      this.playerData.next(playerData);

    });

    this.rxjsStorage.isSongPlayingState.subscribe(x =>{
      var playerData = this.playerDataProp;
      playerData.isPlaying = x;
      this.playerData.next(playerData);
    })

    this.songUpdated$.subscribe(x=>{
      this.currentPlayingSongProp = x;
    })

    this.queueUpdated$.subscribe(x=>{
      this.queueProp = x;
    })

    this.playerDataUpdated$.subscribe(x=>{
      this.playerDataProp = x;
    })

    this.usersUpdated$.subscribe(x=>{
      this.usersProp = x;
    })

    this.isMasterUpdated$.subscribe(x=>{
      this.isMasterProp = x;
    })

    this.groupNameUpdated$.subscribe(x=>{
      this.groupNameProp = x;
    })
  }

  async startSession(){
    this.hubConnection
    .start()
    .then(() => console.log('Connected to SignalR hub'))
    .catch(err => console.error('Error connecting to SignalR hub:', err));
  }

  async joinSession(groupId: string) {
    this.hubConnection
    .start()
    .then(() => console.log('Connected to SignalR hub'))
    .catch(err => console.error('Error connecting to SignalR hub:', err));

    await this.hubConnection.invoke(HUBINVOKES.joinSession, groupId);
  }

  async disconnect(){
    this.hubConnection
    .stop()
    .then(() => console.log('Disconnected SignalR hub'))
    .catch(err => console.error('Error disconnecting SignalR hub:', err));
  }

  async sendCurrentSongToJoinedUser(groupId: string, email: string, data: CurrentMediaPlayerData) {
    await this.hubConnection.invoke(HUBINVOKES.sendCurrentSongToJoinedUser, groupId, email, data);
  }

  async sendPlayerDataToUser(email: string){
    await this.sendCurrentSongToJoinedUser(this.groupNameProp, email, this.playerDataProp);
  }

  async getCurrentQueue(groupId: string) {
    await this.hubConnection.invoke(HUBINVOKES.getCurrentQueue, groupId);
  }

  async addSongsToQueue(groupId: string, songIds: string[]) {
    await this.hubConnection.invoke(HUBINVOKES.addSongsToQueue, groupId, songIds);
  }

  async skipBackInQueue(groupId: string) {
    await this.hubConnection.invoke(HUBINVOKES.skipBackInQueue, groupId);
  }

  async skipForwardInQueue(groupId: string, index: number = 0) {
    await this.hubConnection.invoke(HUBINVOKES.skipForwardInQueue, groupId, index);
  }

  async clearQueue(groupId: string) {
    await this.hubConnection.invoke(HUBINVOKES.clearQueue, groupId);
  }

  async removeSongsInQueue(groupId: string, orderIds: number[]) {
    await this.hubConnection.invoke(HUBINVOKES.removeSongsInQueue, groupId, orderIds);
  }

  async pushSongInQueue(groupId: string, srcIndex: number, targetIndex: number, markAsAddedManually: number = -1) {
    await this.hubConnection.invoke(HUBINVOKES.pushSongInQueue, groupId, srcIndex, targetIndex, markAsAddedManually);
  }

  async updatePlayerData(groupId: string, data: CurrentMediaPlayerData) {
    await this.hubConnection.invoke(HUBINVOKES.clearQueue, groupId, data);
  }

  async leaveGroup(groupId: string){
    await this.hubConnection.invoke(HUBINVOKES.leaveGroup, groupId);
  }

  async setPlayedSongDuration(played: number){
    let playerData = this.playerDataProp;
    playerData.secondsPlayed = played;
    this.playerData.next(playerData);
  }




  
}
