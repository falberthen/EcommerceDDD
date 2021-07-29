import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class SignalrService {
  connection!: signalR.HubConnection;

  constructor() {
    this.connection = this.buildConnection();
  }

  public addCustomerToGroup(customerId: string){

    if(this.connection &&
      this.connection.state != 'Disconnected')
      return;

    this.connection.start()
    .then((data: any) => {
      console.log('SignalR Connected!');
      this.connection.invoke('JoinCustomerToGroup', customerId);
    })
    .catch(function (err) {
      return console.error(err.toString());
    });
  }

  // Start Hub Connection and Register events
  private buildConnection = () => {
    return new signalR.HubConnectionBuilder()
      .configureLogging(signalR.LogLevel.Trace)
      .withUrl(environment.apiUrl + 'orderstatushub')
      .build();
    }
}
