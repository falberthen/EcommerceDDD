import { Component, OnInit, ViewChild, ComponentFactoryResolver, ViewContainerRef } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { faList } from '@fortawesome/free-solid-svg-icons';
import { orderStatusCodes } from 'src/app/core/constants/appConstants';
import { Order } from 'src/app/core/models/Order';
import { AuthService } from 'src/app/core/services/auth.service';
import { SignalrService } from 'src/app/core/services/signalr.service';
import { OrderService } from '../../services/order.service';
import { StoredEventsViewerComponent } from '../../../../shared/stored-events-viewer/stored-events-viewer.component';

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
    }

    //SignalR
    this.signalrService.connection
      .on("updateOrderStatus", (orderId: string, statusText: string, statusCode: number) => {
        this.updateOrderStatus(orderId, statusText, statusCode);
      });
  }

  loadOrders() {
    this.orderService.getOrders(this.customerId)
      .then((result: any) => {
          this.orders = result.data;
        },
        (error) => console.error(error)
      );
  }

  getStatusCssClass(statusCode: number): string {
    switch (statusCode) {
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
      .then((result: any) => {
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
    this.storedEventsViewerComponentRef.instance.aggregateType = "Orders";

    this.storedEventsViewerComponentRef.instance.destroyComponent.subscribe((event: any) => {
      this.storedEventsViewerComponentRef.destroy();
      this.isModalOpen = false;
    });
  }

  updateOrderStatus(orderId: string, statusText: string, statusCode: number){
    var order = this.orders.find(e=>e.orderId == orderId);
    if(order) {
      order.statusText = statusText;
      order.statusCode = statusCode;
    }
  }
}
