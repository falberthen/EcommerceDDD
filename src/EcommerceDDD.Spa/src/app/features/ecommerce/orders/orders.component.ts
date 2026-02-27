import { Component, OnInit, ViewContainerRef, inject, viewChild } from '@angular/core';
import { NgClass, DatePipe } from '@angular/common';
import { faList } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { AuthService } from '@core/services/auth.service';
import { SignalrService } from '@core/services/signalr.service';
import { StoredEventService } from '@shared/services/stored-event.service';
import { ORDER_STATUS_CODES, SIGNALR } from '@features/ecommerce/constants/appConstants';
import { LoaderService } from '@core/services/loader.service';
import { OrderApiService } from '@core/services/api/order-api.service';
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
  private orderApiService = inject(OrderApiService);
  private authService = inject(AuthService);
  private signalrService = inject(SignalrService);
  private storedEventService = inject(StoredEventService);
  protected loaderService = inject(LoaderService);

  readonly storedEventViewerContainer = viewChild.required('storedEventViewerContainer', { read: ViewContainerRef });

  faList = faList;
  orders: OrderViewModel[] = [];
  readonly ORDER_STATUS_CODES = ORDER_STATUS_CODES;

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
      const refreshFn = () => this.orderApiService.getOrderHistory(orderId);

      await refreshFn().then((result) => {
        if (result) {
          this.storedEventService.showStoredEvents(
            this.storedEventViewerContainer(),
            result,
            refreshFn
          );
        }
      });
    } catch (error) {
      this.orderApiService.handleError(error);
    }
  }

  async loadOrders() {
    try {
      await this.orderApiService.getOrders().then((result) => {
        if (result) {
          this.orders = result;
        }
      });
    } catch (error) {
      this.orderApiService.handleError(error);
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
      case ORDER_STATUS_CODES.delivered:
        return 'delivered';
      default:
        return '';
    }
  }

  private async addCustomerToSignalrGroup() {
    if (!this.authService.currentCustomer) return;
    const conn = this.signalrService.connection;

    if (conn.state === 'Disconnected') {
      try {
        await conn.start();
        console.log('SignalR Connected!');
      } catch (err) {
        console.error(err);
        return;
      }
    }

    if (conn.state === 'Connected') {
      conn.invoke('JoinCustomerToGroup', this.authService.currentCustomer!.id);
    }
  }

  async confirmDelivery(orderId: string) {
    try {
      await this.orderApiService.confirmDelivery(orderId);
    } catch (error) {
      this.orderApiService.handleError(error);
    }
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
      this.storedEventService.refreshCurrentViewer();
    }
  }
}
