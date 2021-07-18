import { Component, OnInit, ViewChild, ComponentFactoryResolver, ViewContainerRef, ComponentFactory } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { faList } from '@fortawesome/free-solid-svg-icons';
import { Order } from 'src/app/core/models/Order';
import { AuthService } from 'src/app/core/services/auth.service';
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

  @ViewChild('storedEventViewerContainer', { read: ViewContainerRef })
  storedEventViewerContainer!: ViewContainerRef;

  constructor(
    private orderService: OrderService,
    private authService: AuthService,
    private route: ActivatedRoute,
    private resolver: ComponentFactoryResolver) { }

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
}
