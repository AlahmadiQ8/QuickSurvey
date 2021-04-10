import { Component, OnInit } from '@angular/core';
import {Location} from '@angular/common';
import { FormControl } from '@angular/forms';
import * as signalR from '@microsoft/signalr';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
})
export class AppComponent implements OnInit {
  public message = new FormControl('');
  public messages: { username: string, message: string }[] = [];
  public error = '';
  public status = 'Connecting';
  public username = new Date().toISOString();

  private connection = new signalR.HubConnectionBuilder()
    .withUrl('/hub')
    .withAutomaticReconnect()
    .build();

    constructor(private location: Location) {
    }

  public ngOnInit(): void {

    // Example: /{sessionId}/participant/{username} => ["", sessionId, "participant", username]
    var path = this.location.path().split("/");
    var sessionId = path[1];
    var username = path[3];
    console.log(`sessionId = ${sessionId}, username = ${username}`)

    this.connection.onreconnecting(error => {
      console.assert(this.connection.state === signalR.HubConnectionState.Reconnecting);
      this.error = `Connection lost due to error "${error}". Reconnecting.`;
    });

    this.connection.onreconnected(connectionId => {
      console.assert(this.connection.state === signalR.HubConnectionState.Connected);
      this.status = `Connection reestablished. Connected with connectionId "${connectionId}".`;
    });

    this.connection.on('messageReceived', (username: string, message: string) => {
      this.messages.push({ username, message });
    });

    this.connection.onclose(error => {
        console.assert(this.connection.state === signalR.HubConnectionState.Disconnected);

        this.error = `Connection closed due to error "${error}". Try refreshing this page to restart the connection.`;
        this.status = '';
    });

    this.connection.start().catch(err => {
      this.error = err;
    });
  }

  public async start(): Promise<void> {
    try {
      await this.connection.start();
      console.assert(this.connection.state === signalR.HubConnectionState.Connected);
      this.status = 'SignalR Connected';
    } catch (err) {
      console.assert(this.connection.state === signalR.HubConnectionState.Disconnected);
      console.log(err);
      setTimeout(() => this.start(), 5000);
    }
  }

  public onEnter(e: Event): void {
    console.log('received keyboard input');
    this.send(this.message.value);
    this.message.setValue('');
  }

  private send(message: string): void {
    this.connection
      .send('newMessage', this.username, message)
      .catch(err => {
        console.log('error sending message');
        console.log(err);
      });
  }

  // private static IsValidPath(): boolean {

  // }
}
