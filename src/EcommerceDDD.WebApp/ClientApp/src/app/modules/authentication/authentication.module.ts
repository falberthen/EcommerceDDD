import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LoginComponent } from './components/login/login.component';
import { SharedModule } from 'app/shared/shared.module';
import { AccountComponent } from './components/account/account.component';
import { RouterModule } from '@angular/router';

@NgModule({
  declarations: [
    LoginComponent,
    AccountComponent
  ],
  imports: [
    RouterModule,
    CommonModule,
    SharedModule
  ]
})
export class AuthenticationModule { }
