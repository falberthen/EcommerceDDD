import { inject, Injector } from '@angular/core';
import {
  HttpInterceptorFn,
  HttpErrorResponse,
} from '@angular/common/http';
import { throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { NotificationService } from '../services/notification.service';

export const serverErrorInterceptor: HttpInterceptorFn = (req, next) => {
  const injector = inject(Injector);

  return next(req).pipe(
    catchError((error) => {
      const notifier = injector.get(NotificationService);

      if (error.status === 400 || error.status === 500) {
        const errorResult = error.error;
        if (errorResult.errors) {
          Object.keys(errorResult.errors).forEach(function (key) {
            const errorMsg = errorResult.errors[key];
            notifier.showError(errorMsg);
          });
        } else {
          notifier.showError(errorResult.message);
        }
      } else {
        notifier.showError(error.message);
      }
      return throwError(() => error);
    })
  );
};
