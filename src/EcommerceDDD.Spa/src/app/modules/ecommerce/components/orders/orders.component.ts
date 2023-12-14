import {
  Component,
  OnInit,
  ViewChild,
  ViewContainerRef,
} from '@angular/core';
import { faList } from '@fortawesome/free-solid-svg-icons';
import { AuthService } from '@core/services/auth.service';
import { SignalrService } from '@core/services/signalr.service';
import { StoredEventService } from '@shared/services/stored-event.service';
import { OrdersService } from '../../services/orders.service';
import { Order } from '../../models/Order';
import { firstValueFrom } from 'rxjs';
import { ORDER_STATUS_CODES } from '@ecommerce/constants/appConstants';
import { LoaderService } from '@core/services/loader.service';

@Component({
  selector: 'app-orders',
  templateUrl: './orders.component.html',
  styleUrls: ['./orders.component.scss'],
})
export class OrdersComponent implements OnInit {
  @ViewChild('storedEventViewerContainer', { read: ViewContainerRef })
  storedEventViewerContainer!: ViewContainerRef;

  faList = faList;
  customerId!: string;
  orders: Order[] = [];
  storedEventsViewerComponentRef: any;
  hubHelloMessage!: string;

  constructor(
    private ordersService: OrdersService,
    private authService: AuthService,
    private signalrService: SignalrService,
    private storedEventService: StoredEventService,
    private loaderService: LoaderService,
  ) {}

  async ngOnInit() {
    if (this.authService.currentCustomer) {
      const customer = this.authService.currentCustomer;
      this.customerId = customer.id;
      await this.loadOrders();
    }

    //SignalR
    this.addCustomerToSignalrGroup();

    this.signalrService.connection.on(
      'updateOrderStatus',
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
    await firstValueFrom(this.ordersService.getOrderStoredEvents(orderId)).then(
      (result) => {
        if (result.success) {
          this.storedEventService.showStoredEvents(
            this.storedEventViewerContainer,
            result.data
          );
        }
      }
    );
  }

  async loadOrders() {
    await firstValueFrom(this.ordersService
      .getOrders(this.customerId))
      .then((result) => {
        if (result.success) {
          this.orders = result.data;
        }
      }
    );
  }

  getStatusCssClass(statusCode: number): string {
    switch (statusCode) {
      case ORDER_STATUS_CODES.placed:
        return 'placed';
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
