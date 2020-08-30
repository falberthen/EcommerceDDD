import { Routes, CanActivate, RouterModule } from '@angular/router';
import { HomeComponent } from './modules/ecommerce/components/home/home.component';
import { LoginComponent } from './modules/authentication/components/login/login.component';
import { AuthGuard } from './core/guards/auth.guard';
import { AccountComponent } from './modules/authentication/components/account/account.component';

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
    path: '**',
    component: HomeComponent,
    canActivate: [AuthGuard],
    pathMatch: 'full'
  }
]
