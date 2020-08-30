import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NavMenuComponent } from './components/nav-menu/nav-menu.component';
import { RouterModule } from '@angular/router';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { NgbDateAdapter, NgbDateNativeAdapter } from '@ng-bootstrap/ng-bootstrap';
import { ConfirmationDialogComponent } from './components/confirmation-dialog/confirmation-dialog.component';
import { GithubButtonModule } from 'ng-github-button';
import { SortByPipe } from 'app/core/pipes/sortBy.pipe';

@NgModule({
  declarations: [
    NavMenuComponent,
    ConfirmationDialogComponent,
    SortByPipe
  ],
  imports: [
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
    FontAwesomeModule,
    ReactiveFormsModule,
    FormsModule,
    GithubButtonModule,
    SortByPipe
  ],
  providers: [{
    provide: NgbDateAdapter,
    useClass: NgbDateNativeAdapter
  }]
})
export class SharedModule { }
