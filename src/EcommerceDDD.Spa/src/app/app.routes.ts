import { Routes } from '@angular/router';

import { authGuard } from '@core/guards/auth.guard';

export const routes: Routes = [
  {
    path: 'login',
    loadComponent: () => import('@features/authentication/login/login.component').then(m => m.LoginComponent),
  },
  {
    path: 'customer-account',
    loadComponent: () => import('@features/authentication/customer-account/customer-account.component').then(m => m.CustomerAccountComponent),
  },
  {
    path: 'confirm-email',
    loadComponent: () => import('@features/authentication/confirm-email/confirm-email.component').then(m => m.ConfirmEmailComponent),
  },
  {
    path: 'forgot-password',
    loadComponent: () => import('@features/authentication/forgot-password/forgot-password.component').then(m => m.ForgotPasswordComponent),
  },
  {
    path: 'reset-password',
    loadComponent: () => import('@features/authentication/reset-password/reset-password.component').then(m => m.ResetPasswordComponent),
  },
  {
    path: 'home',
    loadComponent: () => import('@features/ecommerce/home/home.component').then(m => m.HomeComponent),
    canActivate: [authGuard],
    pathMatch: 'full',
  },
  {
    path: 'products',
    loadComponent: () => import('@features/ecommerce/product-selection/product-selection.component').then(m => m.ProductSelectionComponent),
    canActivate: [authGuard],
    pathMatch: 'full',
  },
  {
    path: 'products/:quoteId',
    loadComponent: () => import('@features/ecommerce/product-selection/product-selection.component').then(m => m.ProductSelectionComponent),
    canActivate: [authGuard],
    pathMatch: 'full',
  },
  {
    path: 'orders',
    loadComponent: () => import('@features/ecommerce/orders/orders.component').then(m => m.OrdersComponent),
    canActivate: [authGuard],
    pathMatch: 'full',
  },
  {
    path: 'customer-details',
    loadComponent: () => import('@features/ecommerce/customer-details/customer-details.component').then(m => m.CustomerDetailsComponent),
    canActivate: [authGuard],
    pathMatch: 'full',
  },
  {
    path: '**',
    loadComponent: () => import('@features/authentication/login/login.component').then(m => m.LoginComponent),
    canActivate: [authGuard],
    pathMatch: 'full',
  },
];
