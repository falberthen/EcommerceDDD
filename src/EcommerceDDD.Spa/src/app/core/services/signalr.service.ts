import { Injectable, inject } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { environment } from '@environments/environment';
import { TokenStorageService } from './token-storage.service';

@Injectable({
  providedIn: 'root',
})
export class SignalrService {
  private tokenStorageService = inject(TokenStorageService);

  connection!: signalR.HubConnection;
  constructor() {
    this.connection = this.buildConnection(environment.signalrOrdersHubUrl);
  }

  // Start Hub Connection and Register events
  private buildConnection = (hubUrl: string) => {
    return (
      new signalR.HubConnectionBuilder()        
        .withUrl(hubUrl, {
          // The hub is authenticated, so every connection carries the current access token.
          accessTokenFactory: () => this.tokenStorageService.getToken() ?? '',
        })
        .build()
    );
  };
}
