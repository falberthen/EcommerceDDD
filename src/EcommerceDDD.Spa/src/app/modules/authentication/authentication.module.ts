import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LoginComponent } from './components/login/login.component';
import { CustomerAccountComponent } from './components/customer-account/customer-account.component';
import { RouterModule } from '@angular/router';
import { SharedModule } from '@shared/shared.module';

@NgModule({
  declarations: [LoginComponent, CustomerAccountComponent],
  imports: [RouterModule, CommonModule, SharedModule],
})
export class AuthenticationModule {}
