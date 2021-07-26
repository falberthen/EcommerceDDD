import { Component, OnInit, ViewChild, ComponentFactoryResolver, ViewContainerRef } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { faList } from '@fortawesome/free-solid-svg-icons';
import { orderStatusCodes } from 'src/app/core/constants/appConstants';
import { Order, OrderStatus } from 'src/app/core/models/Order';
import { AuthService } from 'src/app/core/services/auth.service';
import { SignalrService } from 'src/app/core/services/signalr.service';
import { OrderService } from '../../order.service';
import { StoredEventsViewerComponent } from '../stored-events-viewer/stored-events-viewer.component';

@Component({
  selector: 'app-orders',
  templateUrl: './orders.component.html',
  styleUrls: ['./orders.component.scss']
})
export class OrdersComponent implements OnInit {

  faList = faList;
  customerId!: string;
  orderId!: string;
  orders: Order[] = [];
  isLoading = false;
  isModalOpen = false;
  storedEventsViewerComponentRef: any;
  hubHelloMessage!: string;

  @ViewChild('storedEventViewerContainer', { read: ViewContainerRef })
  storedEventViewerContainer!: ViewContainerRef;

  constructor(
    private orderService: OrderService,
    private authService: AuthService,
    private route: ActivatedRoute,
    private resolver: ComponentFactoryResolver,
    private signalrService: SignalrService) { }

  ngOnInit() {
    if(this.authService.currentCustomer) {
      const customer = this.authService.currentCustomer;
      this.customerId = customer.id;
      this.route.paramMap.subscribe(params => {
        const orderIdValue = params.get("orderId");
        if(orderIdValue) {
          this.orderId = orderIdValue;
          this.getOrderDetails();
        }
        else
          this.loadOrders();
      })

      // SignalR group
      this.signalrService
        .addCustomerToGroup(this.customerId);

      this.signalrService.connection
        .on("UpdateOrderStatus", (orderId: string, status: OrderStatus) => {
          this.updateOrderStatus(orderId, status);
        });
    }
  }

  loadOrders() {
    this.isLoading = true;
    this.orderService.getOrders(this.customerId)
      .subscribe((result: any) => {
          this.orders = result.data;
          this.isLoading = false;
        },
        (error) => console.error(error)
      );
  }

  getStatusCssClass(status: OrderStatus): string {
    switch (status.statusCode) {
      case orderStatusCodes.placed:
        return 'placed';
      case orderStatusCodes.readyToShip:
          return 'readyToShip';
          case orderStatusCodes.waitingForPayment:
          return 'waitingForPayment';
      default:
        return '';
    }
  }

  getOrderDetails() {
    this.orderService.getOrderDetails(this.customerId, this.orderId)
      .subscribe((result: any) => {
          this.orders.push(result.data);
        },
        (error) => console.error(error)
      );
  }

  showOrderStoredEvents(orderId: string) {
    this.isModalOpen = true;
    this.storedEventViewerContainer.clear();
    const factory = this.resolver.resolveComponentFactory(StoredEventsViewerComponent);
    this.storedEventsViewerComponentRef = this.storedEventViewerContainer.createComponent(factory);
    this.storedEventsViewerComponentRef.instance.aggregateId = orderId;
    this.storedEventsViewerComponentRef.instance.aggregateType = "OrderStoredEventData";

    this.storedEventsViewerComponentRef.instance.destroyComponent.subscribe((event: any) => {
      this.storedEventsViewerComponentRef.destroy();
      this.isModalOpen = false;
    });
  }

  updateOrderStatus(orderId: string, status: OrderStatus){
    var order = this.orders.find(e=>e.orderId == orderId);
    if(order)
      order.status = status;
  }
}
