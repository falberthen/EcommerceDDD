import {
  HttpEvent,
  HttpHandler,
  HttpRequest,
  HttpErrorResponse,
  HttpInterceptor,
  HTTP_INTERCEPTORS
} from '@angular/common/http';
import { Injectable, Injector } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { catchError, retry } from 'rxjs/operators';
import { ErrorService } from '../services/error-handling/error.service';
import { NotificationService } from '../services/notification.service';

@Injectable()
export class ServerErrorInterceptor implements HttpInterceptor {

  constructor(
    private injector: Injector) { }

  intercept(
      request: HttpRequest<any>,
      next: HttpHandler
  ): Observable<HttpEvent<any>> {

    let errorResult;
    const errorService = this.injector.get(ErrorService);
    const notifier = this.injector.get(NotificationService);

    return next.handle(request)
      .pipe(
        retry(1),
        catchError((error: HttpErrorResponse) => {
            errorResult = errorService.getServerErrorMessage(error);
            const errorObject = errorResult.error;

            if (errorObject) {
              if(errorObject.errors) {
                Object.keys(errorObject.errors).forEach(function (key, index) {
                  const error = errorObject.errors[key];
                  notifier.showError(error);
                });
              }

              if(errorObject.message)
                  notifier.showError(errorObject.message);
            }

            return throwError(error);
        })
      )
  }
}

export const ErrorInterceptorProvider = {
  provide: HTTP_INTERCEPTORS,
  useClass: ServerErrorInterceptor,
  multi: true
};
