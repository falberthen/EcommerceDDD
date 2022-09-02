import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HomeComponent } from './components/home/home.component';
import { StoredEventsViewerComponent } from '../../shared/stored-events-viewer/stored-events-viewer.component';
import { OrdersComponent } from './components/orders/orders.component';
import { RouterModule } from '@angular/router';
import { CustomerDetailsComponent } from './components/customer-details/customer-details.component';
import { CartComponent } from './components/cart/cart.component';
import { SharedModule } from 'src/app/shared/shared.module';
import { NgbDatepickerModule } from '@ng-bootstrap/ng-bootstrap';
import { OrderModule } from 'ngx-order-pipe';
import { ProductSelectionComponent } from './components/product-selection/product-selection.component';

@NgModule({
   declarations: [
      HomeComponent,
      StoredEventsViewerComponent,
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
      OrderModule
   ],
   providers: []
})
export class EcommerceModule { }
