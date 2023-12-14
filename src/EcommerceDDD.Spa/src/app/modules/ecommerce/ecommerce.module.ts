import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HomeComponent } from './components/home/home.component';
import { OrdersComponent } from './components/orders/orders.component';
import { RouterModule } from '@angular/router';
import { CustomerDetailsComponent } from './components/customer-details/customer-details.component';
import { NgbDatepickerModule } from '@ng-bootstrap/ng-bootstrap';
import { ProductSelectionComponent } from './components/product-selection/product-selection.component';
import { CartComponent } from './components/cart/cart.component';
import { SharedModule } from '@shared/shared.module';
import { SortPipe } from '@core/pipes/sort.pipe';

@NgModule({
  declarations: [
    HomeComponent,
    ProductSelectionComponent,
    CartComponent,
    OrdersComponent,
    CustomerDetailsComponent,
    SortPipe
  ],
  imports: [
    CommonModule,
    SharedModule,
    RouterModule,
    NgbDatepickerModule
  ],
  providers: [],
})
export class EcommerceModule {}
