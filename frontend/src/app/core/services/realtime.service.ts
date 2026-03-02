import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { Subject } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class RealtimeService {
  private connection?: signalR.HubConnection;
  readonly newComment$ = new Subject<void>();

  connect(): void {
    if (this.connection) return;

    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(environment.hubUrl)
      .withAutomaticReconnect()
      .build();

    this.connection.on('NewComment', () => this.newComment$.next());

    this.connection.start()
      .then(() => console.log('[SignalR] Connected'))
      .catch(err => console.warn('[SignalR] Connection failed:', err));
  }

  disconnect(): void {
    this.connection?.stop();
    this.connection = undefined;
  }
}
