import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class SignalrService {
  connection!: signalR.HubConnection;
  constructor() {
    this.connection = this.buildConnection(environment.signalrOrdersHubUrl);
  }

  // Start Hub Connection and Register events
  private buildConnection = (hubUrl: string) => {
    return new signalR.HubConnectionBuilder()
      //.configureLogging(signalR.LogLevel.Trace)
      .withUrl(hubUrl)
      .build();
    }
}
