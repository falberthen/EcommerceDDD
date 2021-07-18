import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NavMenuComponent } from './components/nav-menu/nav-menu.component';
import { RouterModule } from '@angular/router';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { NgbDateAdapter, NgbDateNativeAdapter, NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { ConfirmationDialogComponent } from './components/confirmation-dialog/confirmation-dialog.component';
import { GithubButtonModule } from 'ng-github-button';
import { CurrencyDropdownComponent } from './components/currency-dropdown/currency-dropdown.component';

@NgModule({
  declarations: [
    NavMenuComponent,
    ConfirmationDialogComponent,
    CurrencyDropdownComponent,
  ],
  imports: [
    NgbModule,
    CommonModule,
    RouterModule,
    FontAwesomeModule,
    ReactiveFormsModule,
    FormsModule,
    GithubButtonModule
  ],
  exports: [
    NavMenuComponent,
    ConfirmationDialogComponent,
    CurrencyDropdownComponent,
    FontAwesomeModule,
    ReactiveFormsModule,
    FormsModule,
    GithubButtonModule,
  ],
  providers: [{
    provide: NgbDateAdapter,
    useClass: NgbDateNativeAdapter
  }]
})
export class SharedModule { }
