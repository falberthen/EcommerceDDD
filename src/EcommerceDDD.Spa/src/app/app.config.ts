import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideAnimations } from '@angular/platform-browser/animations';
import { provideToastr } from 'ngx-toastr';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { routes } from './app.routes';
import { AuthService } from '@core/services/auth.service';
import { NotificationService } from '@core/services/notification.service';
import { ApiErrorParserService } from '@core/services/api-error-parser.service';
import { ConfirmationDialogService } from '@core/services/confirmation-dialog.service';
import { LocalStorageService } from '@core/services/local-storage.service';
import { CurrencyNotificationService } from '@features/ecommerce/services/currency-notification.service';
import { SignalrService } from '@core/services/signalr.service';
import { KiotaClientService } from '@core/services/kiota-client.service';
import { LoaderService } from '@core/services/loader.service';
import { authInterceptor } from '@core/interceptors/auth.interceptor';
import { loaderInterceptor } from '@core/interceptors/loader.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection(),
    provideAnimations(),
    provideRouter(routes),
    provideHttpClient(
      withInterceptors([authInterceptor, loaderInterceptor])
    ),
    provideToastr(),
    NgbModal,
    AuthService,
    NotificationService,
    ApiErrorParserService,
    ConfirmationDialogService,
    LocalStorageService,
    CurrencyNotificationService,
    SignalrService,
    KiotaClientService,
    LoaderService,
    { provide: 'BASE_URL', useFactory: () => document.getElementsByTagName('base')[0].href, deps: [] },
  ],
};
