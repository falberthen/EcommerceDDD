import { Injectable, Injector } from '@angular/core';
import {
  HttpEvent, HttpRequest, HttpHandler,
  HttpInterceptor, HttpErrorResponse, HTTP_INTERCEPTORS, HttpResponse
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { retry, catchError } from 'rxjs/operators';
import { ErrorService } from './../services/error-handling/error.service';
import { NotificationService } from '../services/notification.service';
import { AuthService } from '../services/auth.service';

@Injectable()
export class ServerErrorInterceptor implements HttpInterceptor {

  constructor(
    private injector: Injector,
    private authenticationService: AuthService) { }

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

    let errorResult;
    const errorService = this.injector.get(ErrorService);
    const notifier = this.injector.get(NotificationService);

    return next.handle(request).pipe(
      //retry(1),
      catchError((error: any) => {
        if (error instanceof HttpErrorResponse) {
          errorResult = errorService.getServerErrorMessage(error);
          if (errorResult.error) {
            Object.keys(errorResult.error.errors).forEach(function (key, index) {
              const error = errorResult.error.errors[key];
              notifier.showError(error.errorMessage);
            });
          }

          return throwError(error);
        }
      }));
  }
}

export const ErrorInterceptorProvider = {
  provide: HTTP_INTERCEPTORS,
  useClass: ServerErrorInterceptor,
  multi: true
};
