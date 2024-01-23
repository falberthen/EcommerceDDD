import { Routes } from '@angular/router';
import { HomeComponent } from '@ecommerce/components/home/home.component';
import { LoginComponent } from '@authentication/components/login/login.component';
import { canActivateGuard } from '@core/guards/auth.guard';
import { CustomerAccountComponent } from '@authentication/components/customer-account/customer-account.component';
import { ProductSelectionComponent } from '@ecommerce/components/product-selection/product-selection.component';
import { OrdersComponent } from '@ecommerce/components/orders/orders.component';
import { CustomerDetailsComponent } from '@ecommerce/components/customer-details/customer-details.component';

export const APP_ROUTES: Routes = [
  {
    path: 'login',
    component: LoginComponent,
  },
  {
    path: 'customer-account',
    component: CustomerAccountComponent,
  },
  {
    path: 'home',
    component: HomeComponent,
    canActivate: [canActivateGuard],
    pathMatch: 'full',
  },
  {
    path: 'products',
    component: ProductSelectionComponent,
    canActivate: [canActivateGuard],
    pathMatch: 'full',
  },
  {
    path: 'products/:quoteId',
    component: ProductSelectionComponent,
    canActivate: [canActivateGuard],
    pathMatch: 'full',
  },
  {
    path: 'orders',
    component: OrdersComponent,
    canActivate: [canActivateGuard],
    pathMatch: 'full',
  },
  {
    path: 'customer-details',
    component: CustomerDetailsComponent,
    canActivate: [canActivateGuard],
    pathMatch: 'full',
  },
  {
    path: '**',
    component: LoginComponent,
    canActivate: [canActivateGuard],
    pathMatch: 'full',
  },
];
