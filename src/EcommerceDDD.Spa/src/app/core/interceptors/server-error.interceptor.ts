import { Injectable, Injector } from '@angular/core';
import { HttpEvent, HttpInterceptor, HttpHandler, HttpRequest, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { NotificationService } from '../services/notification.service';

@Injectable()
export class ServerErrorInterceptor implements HttpInterceptor {
  constructor(private injector: Injector) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(req).pipe(
      catchError((error) => {
        const notifier = this.injector.get(NotificationService);

        if (error.status === 400 || error.status === 500) {
          const errorResult = this.getServerErrorMessage(error).error;
          if(errorResult.errors){
            Object.keys(errorResult.errors).forEach(function (key, index) {
              const errorMsg = errorResult.errors[key];
              notifier.showError(errorMsg);
            });
          }
          else {
            notifier.showError(errorResult.message);
          }
        }
        else{
          notifier.showError(error.message);
        }
        return throwError(error);
      })
    );
  }

  private getServerErrorMessage(errorResponse: HttpErrorResponse): HttpErrorResponse {
    return errorResponse;
  }
}
