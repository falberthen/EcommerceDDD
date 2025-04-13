import { NgModule, inject } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { AuthService } from './services/auth.service';
import { AuthGuard } from './guards/auth.guard';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ToastrModule } from 'ngx-toastr';
import { NotificationService } from './services/notification.service';
import { LocalStorageService } from './services/local-storage.service';
import { CurrencyNotificationService } from '@ecommerce/services/currency-notification.service';
import { ConfirmationDialogService } from './services/confirmation-dialog.service';
import { SignalrService } from './services/signalr.service';

@NgModule({
  declarations: [],
  imports: [
    HttpClientModule,
    BrowserAnimationsModule, // required animations module
    ToastrModule.forRoot(), // ToastrModule added,
  ],
  providers: [
    AuthService,
    AuthGuard,
    BrowserAnimationsModule,
    ToastrModule,
    NotificationService,
    ConfirmationDialogService,
    LocalStorageService,
    CurrencyNotificationService,
    SignalrService,
  ],
})
export class CoreModule {
  constructor() {
    const core = inject(CoreModule, { optional: true, skipSelf: true })!;

    if (core) {
      throw new Error('You should import core module only in the root module');
    }
  }
}
