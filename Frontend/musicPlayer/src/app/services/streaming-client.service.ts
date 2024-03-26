import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { environment } from 'src/environments/environment';
import { HUBEMITS, HUBINVOKES } from '../constants/hub-routes';
import { CurrentMediaPlayerData } from '../models/hub-models';
import { Observable, Subject, lastValueFrom } from 'rxjs';
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


  constructor() { 
    this.hubConnection = new signalR.HubConnectionBuilder()
    .withUrl(`${environment.apiUrl}`) // Replace with your SignalR hub URL
    .build();

    this.hubConnection
    .start()
    .then(() => console.log('Connected to SignalR hub'))
    .catch(err => console.error('Error connecting to SignalR hub:', err));

    this.hubConnection.on(HUBEMITS.getGroupName, (groupName: string)=>{
      this.groupName.next(groupName);
    });

    this.hubConnection.on(HUBEMITS.userJoinedSession, async (email: string)=>{
      var users = await lastValueFrom(this.users);
      users.push(email);
      this.users.next(users);
    });

    this.hubConnection.on(HUBEMITS.userDisconnected, async (email: string)=>{
      var users = await lastValueFrom(this.users);
      users.push(email);
      this.users.next(users);
    });

    this.hubConnection.on(HUBEMITS.groupDeleted, ()=>{
      // TODO: Emit event that group has been deleted
    });

    this.hubConnection.on(HUBEMITS.getPlayerData, (data: CurrentMediaPlayerData)=>{
      this.playerData.next(data);
    });

    this.hubConnection.on(HUBEMITS.getQueue, (queue: QueueSongModel[])=>{
      this.queue.next(queue);
    });

    this.hubConnection.on(HUBEMITS.getCurrentPlayingSong, (song: PlaylistSongModel)=>{
      this.currentPlayingSong.next(song);
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

  async sendCurrentSongToJoinedUser(groupId: string, joinedUserId: string, data: CurrentMediaPlayerData) {
    await this.hubConnection.invoke(HUBINVOKES.sendCurrentSongToJoinedUser, groupId, joinedUserId, data);
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
