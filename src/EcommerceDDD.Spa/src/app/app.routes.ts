import { Routes } from '@angular/router';
import { HomeComponent } from './modules/ecommerce/components/home/home.component';
import { LoginComponent } from './modules/authentication/components/login/login.component';
import { AuthGuard } from './core/guards/auth.guard';
import { AccountComponent } from './modules/authentication/components/account/account.component';
import { ProductSelectionComponent } from './modules/ecommerce/components/product-selection/product-selection.component';
import { OrdersComponent } from './modules/ecommerce/components/orders/orders.component';
import { CustomerProfileComponent } from './modules/ecommerce/components/customer-profile/customer-profile.component';

export const APP_ROUTES: Routes = [
  {
    path: 'login',
    component: LoginComponent
  },
  {
    path: 'account',
    component: AccountComponent
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
    path: 'orders/:customerId/:orderId',
    component: OrdersComponent,
    canActivate: [AuthGuard],
    pathMatch: 'full'
  },
  {
    path: 'customer-profile',
    component: CustomerProfileComponent,
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
