import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Order } from 'app/core/models/Order';
import { AuthService } from 'app/core/services/auth.service';
import { OrderService } from '../../order.service';

@Component({
  selector: 'app-orders',
  templateUrl: './orders.component.html',
  styleUrls: ['./orders.component.scss']
})
export class OrdersComponent implements OnInit {

  customerId: string;
  orderId: string;
  orders: Order[] = [];
  isLoading = false;

  constructor(
    private orderService: OrderService,
    private authService: AuthService,
    private route: ActivatedRoute) { }

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
}
