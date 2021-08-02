import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import * as signalR from '@microsoft/signalr';
import { ClientMessages, SignalRService } from '../services/signalr.service';
import { AuthService } from '../services/auth.service';
import { ServerMessages } from '../services/signalr.service';
import { ApiService } from '../services/api.service';
import { Subject, Subscription } from 'rxjs';
import { concatMap } from 'rxjs/operators';
import { Choice, Session, User } from '../models';

@Component({
  selector: 'app-survey-session',
  templateUrl: './survey-session.component.html',
  providers: [AuthService, SignalRService, ApiService]
})
export class SurveySessionComponent implements OnInit, OnDestroy {
  public message = new FormControl('');
  public messages: { username: string, message: string }[] = [];
  public state: signalR.HubConnectionState = signalR.HubConnectionState.Connecting;
  public error = '';
  public user: User;
  public activeUsers: string[] = [];
  public offlineUsers: string[] = [];
  public session: Session | null = null;
  public sessionSubscription: Subscription;
  public subject = new Subject<undefined>();

  constructor(private signalRService: SignalRService, private authService: AuthService, private apiService: ApiService) {
    if (this.authService.currentUser == null) {
      throw new Error('Cannot authenticate');
    }
    this.user = this.authService.currentUser;
    this.sessionSubscription = this.subject.asObservable().pipe(concatMap(s => this.apiService.getSession()))
      .subscribe(session => {
        this.session = session;
        this.offlineUsers = session.participants.filter(u => !this.activeUsers.includes(u))
      });
  }

  public ngOnInit(): void {
    this.setupSignalR();
    this.subject.next();
  }

  public onEnter(e: Event): void {
    console.log('received keyboard input');
    this.send(this.message.value);
    this.subject.next();
  }

  private setupSignalR(): void {
    this.signalRService.createConnection({
      onReconnectedHandler: (connectionId) => {
        this.state = this.signalRService.connectionState;
        this.error = '';
        console.log(`Connection reestablished. Connected with connectionId "${connectionId}".`);
        this.subject.next();
        // this.session.
      },
      onCloseHandler: this.errorCallback,
      onReconnectionErrorHandler: this.errorCallback,
      OnReconnectingHandler: this.errorCallback
    })

    this.signalRService.OnMessageReceived('messageReceived', (username: string, message: string) => {
      this.messages.push({ username, message });
    });

    this.signalRService.OnMessageReceived(ServerMessages.VotesUpdated, (choices: Choice[]) => {
      console.log(choices);
      this.session!.choices = choices;
    });

    this.signalRService.OnMessageReceived(ServerMessages.UserAdded, (participants: string[]) => {
      this.session!.participants = participants;
    });

    this.signalRService.OnMessageReceived(ServerMessages.ActiveUsersUpdated, (activeUsers: string[]) => {
      this.activeUsers = activeUsers.filter(u => this.user.username !== u);
      this.offlineUsers = this.session!.participants.filter(u => !this.activeUsers.includes(u));
    });

    this.start().catch(err => {
      console.log('unexpected error occured')
      this.error = err;
    });
  }

  private async start(): Promise<void> {
    try {
      await this.signalRService.start()
      console.assert(this.signalRService.connectionState === signalR.HubConnectionState.Connected);
      this.state = this.signalRService.connectionState;
    } catch (err) {
      console.assert(this.signalRService.connectionState === signalR.HubConnectionState.Disconnected);
      console.log(err);
      this.state = this.signalRService.connectionState;
      setTimeout(() => this.start(), 5000);
    }
  }

  private send(message: string): void {
    this.signalRService.SendMessage('newMessage', this.user.username, message)
      .catch(err => {
        console.log('error sending message');
        console.log(err);
      });
  }

  public selectVoteHandler(choiceId: number): void {
    this.signalRService.SendMessage(ClientMessages.ParticipantVoted, choiceId)
      .catch(err => {
        console.log('error sending message');
        console.log(err);
      });
  }

  private errorCallback = (err: Error | undefined) => {
    this.state = this.signalRService.connectionState;
    this.error = err?.message || '';
  }

  ngOnDestroy(): void {
    this.sessionSubscription.unsubscribe();
  }
}
