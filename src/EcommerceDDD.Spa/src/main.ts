import { bootstrapApplication } from '@angular/platform-browser';
import { provideAnimations } from '@angular/platform-browser/animations';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { ToastrModule } from 'ngx-toastr';
import { enableProdMode, importProvidersFrom } from '@angular/core';

import { AppComponent } from './app/app.component';
import { routes } from './app/app.routes';
import { environment } from '@environments/environment';
import { authInterceptor } from './app/core/interceptors/auth.interceptor';
import { loaderInterceptor } from './app/core/interceptors/loader.interceptor';
import { serverErrorInterceptor } from './app/core/interceptors/server-error.interceptor';

export function getBaseUrl() {
  return document.getElementsByTagName('base')[0].href;
}

if (environment.production) {
  enableProdMode();
}

bootstrapApplication(AppComponent, {
  providers: [
    provideAnimations(),
    provideRouter(routes),
    provideHttpClient(
      withInterceptors([authInterceptor, loaderInterceptor, serverErrorInterceptor])
    ),
    importProvidersFrom(NgbModule),
    importProvidersFrom(ToastrModule.forRoot()),
    { provide: 'BASE_URL', useFactory: getBaseUrl, deps: [] },
  ],
}).catch((err) => console.error(err));
