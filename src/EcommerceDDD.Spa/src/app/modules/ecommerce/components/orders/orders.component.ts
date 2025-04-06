import { Component, OnInit, ViewChild, ViewContainerRef } from '@angular/core';
import { faList } from '@fortawesome/free-solid-svg-icons';
import { AuthService } from '@core/services/auth.service';
import { SignalrService } from '@core/services/signalr.service';
import { StoredEventService } from '@shared/services/stored-event.service';
import { ORDER_STATUS_CODES, SIGNALR } from '@ecommerce/constants/appConstants';
import { LoaderService } from '@core/services/loader.service';
import { KiotaClientService } from '@core/services/kiota-client.service';
import { OrderViewModel } from 'src/app/clients/models';

@Component({
  selector: 'app-orders',
  templateUrl: './orders.component.html',
  styleUrls: ['./orders.component.scss'],
})
export class OrdersComponent implements OnInit {
  @ViewChild('storedEventViewerContainer', { read: ViewContainerRef })
  storedEventViewerContainer!: ViewContainerRef;

  faList = faList;
  customerId?: string;
  orders: OrderViewModel[] = [];
  storedEventsViewerComponentRef: any;
  hubHelloMessage!: string;

  constructor(
    private kiotaClientService: KiotaClientService,
    private authService: AuthService,
    private signalrService: SignalrService,
    private storedEventService: StoredEventService,
    private loaderService: LoaderService
  ) {}

  async ngOnInit() {
    if (this.authService.currentCustomer) {
      const customer = this.authService.currentCustomer;
      this.customerId = customer.id!;
      await this.loadOrders();
    }

    //SignalR
    this.addCustomerToSignalrGroup();

    this.signalrService.connection.on(
      SIGNALR.updateOrderStatus,
      (orderId: string, statusText: string, statusCode: number) => {
        this.updateOrderStatus(orderId, statusText, statusCode);
      }
    );
  }

  get isLoading() {
    return this.loaderService.loading$;
  }

  getOrderIdString(guid: string): string {
    const guidParts = guid.split('-');
    const firstPart = guidParts[0];
    return firstPart.toUpperCase();
  }

  async showOrderStoredEvents(orderId: string) {
    try {
      await this.kiotaClientService.client.api.orders
        .byOrderId(orderId)
        .history.get()
        .then((result) => {
          if (result!.success) {
            this.storedEventService.showStoredEvents(
              this.storedEventViewerContainer,
              result?.data!
            );
          }
        });
    } catch (error) {
      this.kiotaClientService.handleError(error);
    }
  }

  async loadOrders() {
    try {
      await this.kiotaClientService.client.api.orders.get().then((result) => {
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
    if (this.signalrService.connection.state != 'Disconnected') return;

    // SignalR
    this.signalrService.connection
      .start()
      .then(() => {
        console.log('SignalR Connected!');
        this.signalrService.connection.invoke(
          'JoinCustomerToGroup',
          this.authService.currentCustomer!.id
        );
      })
      .catch(function (err) {
        return console.error(err.toString());
      });
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
