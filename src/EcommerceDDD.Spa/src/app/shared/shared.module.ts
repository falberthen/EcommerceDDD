import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { GithubButtonModule } from 'ng-github-button';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { NgbDateAdapter, NgbDateNativeAdapter, NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { ConfirmationDialogComponent } from './components/confirmation-dialog/confirmation-dialog.component';
import { CurrencyDropdownComponent } from '../modules/ecommerce/components/currency-dropdown/currency-dropdown.component';
import { StoredEventsViewerComponent } from './components/stored-events-viewer/stored-events-viewer.component';

@NgModule({
  declarations: [
    StoredEventsViewerComponent,
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
