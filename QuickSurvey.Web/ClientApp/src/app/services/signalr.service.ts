import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { AuthService } from './auth.service';

export const enum ServerMessages {
  ActiveUsersUpdated = 'ActiveUsersUpdated',
  VotesUpdated = 'VotesUpdated',
  UserAdded = 'UserAdded'
}

export const enum ClientMessages {
  ParticipantVoted = 'ParticipantVoted'
}

type OnReconnectedHandler = (connectionId: string | undefined) => void;

type Callback = (error: Error | undefined) => void;

interface SignalRHandlers {
  onReconnectedHandler: OnReconnectedHandler,
  onReconnectionErrorHandler: Callback,
  OnReconnectingHandler: Callback
  onCloseHandler: Callback
}

@Injectable()
export class SignalRService {
  private connection!: signalR.HubConnection;
  private accessToken: string;

  constructor(private authService: AuthService) {
    this.accessToken = this.authService.currentUser!.accessToken;
    console.log('this.accessToken = ' + this.accessToken)
  }

  get connectionState() {
    return this.connection.state;
  }

  public start(): Promise<void> {
    console.assert(this.connection != null);
    return this.connection.start()
  }

  public createConnection(handlers: SignalRHandlers): void {
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl('/hub?access_token=' + this.accessToken)
      .withAutomaticReconnect()
      .build();


    this.connection.onreconnecting(error => {
      console.assert(this.connection.state === signalR.HubConnectionState.Reconnecting);
      handlers.onReconnectionErrorHandler(error);
    });

    this.connection.onreconnected(connectionId => {
      console.assert(this.connection.state === signalR.HubConnectionState.Connected);
      handlers.onReconnectedHandler(connectionId);
    });

    this.connection.onclose(error => {
      console.assert(this.connection.state === signalR.HubConnectionState.Disconnected);
      handlers.onCloseHandler(error);
    });
  }

  public OnMessageReceived(methodName: string, newMethod: (...args: any[]) => void): void {
    this.connection.on(methodName, newMethod);
  }

  public SendMessage(methodName: string, ...args: any[]): Promise<void> {
    return this.connection.send(methodName, ...args)
  }


}
