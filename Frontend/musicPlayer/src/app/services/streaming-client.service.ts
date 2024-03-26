import { EventEmitter, Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { environment } from 'src/environments/environment';
import { HUBEMITS, HUBINVOKES } from '../constants/hub-routes';
import { CurrentMediaPlayerData } from '../models/hub-models';
import { BehaviorSubject, Observable, Subject, lastValueFrom } from 'rxjs';
import { PlaylistSongModel, QueueSongModel } from '../models/playlist-models';

@Injectable({
  providedIn: 'root'
})
export class StreamingClientService {
  private hubConnection: signalR.HubConnection;

  private groupName = new Subject<string>();
  groupNameUpdated$: Observable<string> = this.groupName.asObservable();

  private isMaster = new Subject<boolean>();
  isMasterUpdated$: Observable<boolean> = this.isMaster.asObservable();

  private users = new Subject<string[]>();
  usersUpdated$: Observable<string[]> = this.users.asObservable();

  private playerData = new Subject<CurrentMediaPlayerData>();
  playerDataUpdated$: Observable<CurrentMediaPlayerData> = this.playerData.asObservable();

  private queue = new Subject<QueueSongModel[]>();
  queueUpdated$: Observable<QueueSongModel[]> = this.queue.asObservable();

  private currentPlayingSong = new Subject<PlaylistSongModel>();
  songUpdated$: Observable<PlaylistSongModel> = this.currentPlayingSong.asObservable();

  private groupDeletedEvent = new EventEmitter<void>();


  constructor() { 
    this.hubConnection = new signalR.HubConnectionBuilder()
    .withUrl(`${environment.hubUrl}`) // Replace with your SignalR hub URL
    .build();

    this.hubConnection
    .start()
    .then(() => console.log('Connected to SignalR hub'))
    .catch(err => console.error('Error connecting to SignalR hub:', err));

    this.hubConnection.on(HUBEMITS.getGroupName, (groupName: string)=>{
      this.groupName.next(groupName);
      console.log("Connected with: ", groupName);
    });

    this.hubConnection.on(HUBEMITS.userJoinedSession, async (email: string)=>{
      console.log("User joined: ", email);
      var users = await lastValueFrom(this.users);
      users.push(email);
      this.users.next(users);
      console.log("Users: ", users);
      await this.sendCurrentSongToJoinedUser(await lastValueFrom(this.groupName), email, await lastValueFrom(this.playerData))
    });

    this.hubConnection.on(HUBEMITS.userDisconnected, async (email: string)=>{
      console.log("User disconnected: ", email);
      var users = await lastValueFrom(this.users);
      users.push(email);
      this.users.next(users);
    });

    this.hubConnection.on(HUBEMITS.groupDeleted, ()=>{
      console.log("Group deleted: ");
      this.groupDeletedEvent.emit();
    });

    this.hubConnection.on(HUBEMITS.getPlayerData, (data: CurrentMediaPlayerData)=>{
      console.log("Receiving player data: ", data);
      this.playerData.next(data);
    });

    this.hubConnection.on(HUBEMITS.getQueue, (queue: QueueSongModel[])=>{
      console.log("Get queue: ", queue);
      this.queue.next(queue);
    });

    this.hubConnection.on(HUBEMITS.getCurrentPlayingSong, async (song: PlaylistSongModel)=>{
      console.log("Get current plaing song: ", song);
      this.currentPlayingSong.next(song);
      var playerData = await lastValueFrom(this.playerData);
      playerData.secondsPlayed = 0;
      this.playerData.next(playerData);
    });
  }

  async disconnect(){
    this.hubConnection
    .stop()
    .then(() => console.log('Disconnected SignalR hub'))
    .catch(err => console.error('Error disconnecting SignalR hub:', err));
  }

  
  async joinSession(groupId: string) {
    await this.hubConnection.invoke(HUBINVOKES.joinSession, groupId);
  }

  async sendCurrentSongToJoinedUser(groupId: string, email: string, data: CurrentMediaPlayerData) {
    await this.hubConnection.invoke(HUBINVOKES.sendCurrentSongToJoinedUser, groupId, email, data);
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


  
}
