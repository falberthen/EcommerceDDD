import { Component, OnInit, ViewContainerRef, inject, viewChild } from '@angular/core';
import { NgClass, DatePipe } from '@angular/common';
import { faList } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { AuthService } from '@core/services/auth.service';
import { SignalrService } from '@core/services/signalr.service';
import { IEventHistory, StoredEventService } from '@shared/services/stored-event.service';
import { ORDER_STATUS_CODES, SIGNALR } from '@features/ecommerce/constants/appConstants';
import { LoaderService } from '@core/services/loader.service';
import { KiotaClientService } from '@core/services/kiota-client.service';
import { SortPipe } from '@core/pipes/sort.pipe';
import { LoaderSkeletonComponent } from '@shared/components/loader-skeleton/loader-skeleton.component';
import { OrderViewModel } from 'src/app/clients/models';

@Component({
  selector: 'app-orders',
  templateUrl: './orders.component.html',
  styleUrls: ['./orders.component.scss'],
  
  imports: [FontAwesomeModule, SortPipe, LoaderSkeletonComponent, DatePipe, NgClass],
})
export class OrdersComponent implements OnInit {
  private kiotaClientService = inject(KiotaClientService);
  private authService = inject(AuthService);
  private signalrService = inject(SignalrService);
  private storedEventService = inject(StoredEventService);
  protected loaderService = inject(LoaderService);

  readonly storedEventViewerContainer = viewChild.required('storedEventViewerContainer', { read: ViewContainerRef });

  faList = faList;
  customerId?: string;
  orders: OrderViewModel[] = [];
  storedEventsViewerComponentRef: any;
  hubHelloMessage!: string;
  eventHistory: IEventHistory[] = [];

  async ngOnInit() {
    await this.loadOrders();
    this.addCustomerToSignalrGroup();

    this.signalrService.connection.on(
      SIGNALR.updateOrderStatus,
      (orderId: string, statusText: string, statusCode: number) => {
        this.updateOrderStatus(orderId, statusText, statusCode);
      }
    );
  }

  get isLoading(): boolean {
    return this.loaderService.loading();
  }

  getOrderIdString(guid: string): string {
    const guidParts = guid.split('-');
    const firstPart = guidParts[0];
    return firstPart.toUpperCase();
  }

  async showOrderStoredEvents(orderId: string) {
    try {
      await this.kiotaClientService.client.orderProcessing.api.v2.orders
        .byOrderId(orderId)
        .history.get()
        .then((result) => {
          if (result!.success) {
            this.storedEventService.showStoredEvents(
              this.storedEventViewerContainer(),
              result?.data ?? []
            );
          }
        });
    } catch (error) {
      this.kiotaClientService.handleError(error);
    }
  }

  async loadOrders() {
    try {
      await this.kiotaClientService.client.orderProcessing.api.v2.orders.get().then((result) => {
        if (result!.success) {
          this.orders = result?.data!;
        }
      });
    } catch (error) {
      this.kiotaClientService.handleError(error);
    }
  }

  getStatusCssClass(statusCode: number): string {
    switch (statusCode) {
      case ORDER_STATUS_CODES.placed:
        return 'placed';
      case ORDER_STATUS_CODES.processed:
        return 'processed';
      case ORDER_STATUS_CODES.paid:
        return 'paid';
      case ORDER_STATUS_CODES.shipped:
        return 'shipped';
      case ORDER_STATUS_CODES.canceled:
        return 'canceled';
      case ORDER_STATUS_CODES.completed:
        return 'completed';
      default:
        return '';
    }
  }

  private async addCustomerToSignalrGroup() {
    if (!this.authService.currentCustomer) return;
    if (this.signalrService.connection.state !== 'Disconnected') return;

    this.signalrService.connection
      .start()
      .then(() => {
        console.log('SignalR Connected!');
        this.signalrService.connection.invoke(
          'JoinCustomerToGroup',
          this.authService.currentCustomer!.id
        );
      })
      .catch((err: Error) => console.error(err.toString()));
  }

  private updateOrderStatus(
    orderId: string,
    statusText: string,
    statusCode: number
  ) {
    var order = this.orders.find((e) => e.orderId == orderId);
    if (order) {
      order.statusText = statusText;
      order.statusCode = statusCode;
    }
  }
}
