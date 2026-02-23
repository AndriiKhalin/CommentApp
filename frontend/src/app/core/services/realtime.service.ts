import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { Subject } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class RealtimeService {
  private connection!: signalR.HubConnection;
  public newComment$ = new Subject<void>();

  connect(): void {
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:5000/hubs/comments')
      .withAutomaticReconnect()
      .build();

    this.connection.on('NewComment', () => {
      this.newComment$.next();
    });

    this.connection.start().catch(console.error);
  }

  disconnect(): void {
    this.connection?.stop();
  }
}
