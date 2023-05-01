import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HomeComponent } from './components/home/home.component';
import { OrdersComponent } from './components/orders/orders.component';
import { RouterModule } from '@angular/router';
import { CustomerDetailsComponent } from './components/customer-details/customer-details.component';
import { NgbDatepickerModule } from '@ng-bootstrap/ng-bootstrap';
import { OrderModule } from 'ngx-order-pipe';
import { ProductSelectionComponent } from './components/product-selection/product-selection.component';
import { CartComponent } from './components/cart/cart.component';
import { SharedModule } from 'src/app/shared/shared.module';

@NgModule({
  declarations: [
    HomeComponent,
    ProductSelectionComponent,
    CartComponent,
    OrdersComponent,
    CustomerDetailsComponent,
  ],
  imports: [
    CommonModule,
    SharedModule,
    RouterModule,
    NgbDatepickerModule,
    OrderModule,
  ],
  providers: [],
})
export class EcommerceModule {}
