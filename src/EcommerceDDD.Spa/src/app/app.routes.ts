import { Routes } from '@angular/router';
import { HomeComponent } from './modules/ecommerce/components/home/home.component';
import { LoginComponent } from './modules/authentication/components/login/login.component';
import { AuthGuard } from './core/guards/auth.guard';
import { CustomerAccountComponent } from './modules/authentication/components/customer-account/customer-account.component';
import { ProductSelectionComponent } from './modules/ecommerce/components/product-selection/product-selection.component';
import { OrdersComponent } from './modules/ecommerce/components/orders/orders.component';
import { CustomerDetailsComponent } from './modules/ecommerce/components/customer-details/customer-details.component';

export const APP_ROUTES: Routes = [
  {
    path: 'login',
    component: LoginComponent
  },
  {
    path: 'customer-account',
    component: CustomerAccountComponent
  },
  {
    path: 'home',
    component: HomeComponent,
    canActivate: [AuthGuard],
    pathMatch: 'full'
  },
  {
    path: 'products',
    component: ProductSelectionComponent,
    canActivate: [AuthGuard],
    pathMatch: 'full'
  },
  {
    path: 'products/:quoteId',
    component: ProductSelectionComponent,
    canActivate: [AuthGuard],
    pathMatch: 'full'
  },
  {
    path: 'orders',
    component: OrdersComponent,
    canActivate: [AuthGuard],
    pathMatch: 'full'
  },
  {
    path: 'customer-details',
    component: CustomerDetailsComponent,
    canActivate: [AuthGuard],
    pathMatch: 'full'
  },
  {
    path: '**',
    component: LoginComponent,
    canActivate: [AuthGuard],
    pathMatch: 'full'
  }
]
