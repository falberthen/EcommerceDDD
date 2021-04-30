import { Component, OnInit, ViewChild, ComponentFactoryResolver, ViewContainerRef } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Order } from 'app/core/models/Order';
import { AuthService } from 'app/core/services/auth.service';
import { OrderService } from '../../order.service';
import { StoredEventsViewerComponent } from 'app/modules/ecommerce/components/stored-events-viewer/stored-events-viewer.component';
import { faList } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-orders',
  templateUrl: './orders.component.html',
  styleUrls: ['./orders.component.scss']
})
export class OrdersComponent implements OnInit {

  faList = faList;
  customerId: string;
  orderId: string;
  orders: Order[] = [];
  isLoading = false;
  isModalOpen = false;
  storedEventsViewerComponentRef: any;

  @ViewChild("storedEventViewerContainer", { read: ViewContainerRef }) storedEventViewerContainer;

  constructor(
    private orderService: OrderService,
    private authService: AuthService,
    private route: ActivatedRoute,
    private resolver: ComponentFactoryResolver) { }

  ngOnInit() {
    var customer = this.authService.currentCustomerValue;
    this.customerId = customer.id;

    this.route.paramMap.subscribe(params => {
      this.orderId = params.get("orderId")
      if(this.orderId)
        this.getOrderDetails();
      else
        this.loadOrders();
    })
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
    this.orderService.getOrderDetails(this.orderId)
      .subscribe((result: any) => {
          this.orders.push(result.data);
        },
        (error) => console.error(error)
      );
  }

  showOrderStoredEvents(orderId) {
    this.isModalOpen = true;
    this.storedEventViewerContainer.clear();
    const factory = this.resolver.resolveComponentFactory(StoredEventsViewerComponent);
    this.storedEventsViewerComponentRef = this.storedEventViewerContainer.createComponent(factory);
    this.storedEventsViewerComponentRef.instance.aggregateId = orderId;
    this.storedEventsViewerComponentRef.instance.aggregateType = "OrderStoredEventData";

    this.storedEventsViewerComponentRef.instance.destroyComponent.subscribe(event => {
      this.storedEventsViewerComponentRef.destroy();
      this.isModalOpen = false;
    });
  }
}
