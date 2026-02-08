import { Routes } from '@angular/router';
import { HomeComponent } from '@ecommerce/components/home/home.component';
import { LoginComponent } from '@authentication/components/login/login.component';
import { authGuard } from '@core/guards/auth.guard';
import { CustomerAccountComponent } from '@authentication/components/customer-account/customer-account.component';
import { ProductSelectionComponent } from '@ecommerce/components/product-selection/product-selection.component';
import { OrdersComponent } from '@ecommerce/components/orders/orders.component';
import { CustomerDetailsComponent } from '@ecommerce/components/customer-details/customer-details.component';

export const routes: Routes = [
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
    canActivate: [authGuard],
    pathMatch: 'full',
  },
  {
    path: 'products',
    component: ProductSelectionComponent,
    canActivate: [authGuard],
    pathMatch: 'full',
  },
  {
    path: 'products/:quoteId',
    component: ProductSelectionComponent,
    canActivate: [authGuard],
    pathMatch: 'full',
  },
  {
    path: 'orders',
    component: OrdersComponent,
    canActivate: [authGuard],
    pathMatch: 'full',
  },
  {
    path: 'customer-details',
    component: CustomerDetailsComponent,
    canActivate: [authGuard],
    pathMatch: 'full',
  },
  {
    path: '**',
    component: LoginComponent,
    canActivate: [authGuard],
    pathMatch: 'full',
  },
];
