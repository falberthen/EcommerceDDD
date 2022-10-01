import { Component, OnInit, ViewChild, ViewContainerRef } from '@angular/core';
import { faList } from '@fortawesome/free-solid-svg-icons';
import { orderStatusCodes } from 'src/app/core/constants/appConstants';
import { AuthService } from 'src/app/core/services/auth.service';
import { SignalrService } from 'src/app/core/services/signalr.service';
import { OrdersService } from '../../services/orders.service';
import { Order } from '../../models/Order';
import { StoredEventService } from 'src/app/core/services/stored-event.service';
import { firstValueFrom } from 'rxjs';

@Component({
  selector: 'app-orders',
  templateUrl: './orders.component.html',
  styleUrls: ['./orders.component.scss']
})
export class OrdersComponent implements OnInit {
  @ViewChild('storedEventViewerContainer', { read: ViewContainerRef })
  storedEventViewerContainer!: ViewContainerRef;

  faList = faList;
  customerId!: string;
  orderId!: string;
  orders: Order[] = [];
  storedEventsViewerComponentRef: any;
  hubHelloMessage!: string;

  constructor(
    private ordersService: OrdersService,
    private authService: AuthService,
    private signalrService: SignalrService,
    private storedEventService: StoredEventService) { }

  async ngOnInit() {
    if(this.authService.currentCustomer) {
      const customer = this.authService.currentCustomer;
      this.customerId = customer.id;
      await this.loadOrders();
    }

    //SignalR
    this.signalrService.connection
      .on("updateOrderStatus", (orderId: string, statusText: string, statusCode: number) => {
        this.updateOrderStatus(orderId, statusText, statusCode);
      });
  }

  showOrderStoredEvents(orderId : string) {
    this.storedEventService.showStoredEvents(this.storedEventViewerContainer,
      "Orders", orderId);
  }

  async loadOrders() {
    await firstValueFrom(this.ordersService.getOrders(this.customerId))
    .then(result => {
      if(result.success) {
          this.orders = result.data;
      }
    });
  }

  getStatusCssClass(statusCode: number): string {
    switch (statusCode) {
      case orderStatusCodes.placed:
        return 'placed';
        case orderStatusCodes.paid:
          return 'paid';
      case orderStatusCodes.shipped:
        return 'shipped';
      case orderStatusCodes.canceled:
        return 'canceled';
      case orderStatusCodes.completed:
        return 'completed';
      default:
        return '';
    }
  }

  private updateOrderStatus(orderId: string, statusText: string, statusCode: number){
    var order = this.orders.find(e=>e.orderId == orderId);
    if(order) {
      order.statusText = statusText;
      order.statusCode = statusCode;
    }
  }
}
