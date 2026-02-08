import { inject } from '@angular/core';
import {
  HttpInterceptorFn,
  HttpErrorResponse,
} from '@angular/common/http';
import { throwError } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { TokenStorageService } from '../services/token-storage.service';
import { AuthService } from '../services/auth.service';

const TOKEN_HEADER_KEY = 'Authorization';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const tokenService = inject(TokenStorageService);
  const authService = inject(AuthService);

  const token = tokenService.getToken();
  let authReq = req;

  if (token) {
    authReq = req.clone({
      headers: req.headers.set(TOKEN_HEADER_KEY, 'Bearer ' + token),
    });
  }

  return next(authReq).pipe(
    tap(
      () => {},
      (err: any) => {
        if (err instanceof HttpErrorResponse && err.status === 401) {
          authService.logout();
        }
      }
    ),
    catchError((error) => {
      return throwError(() => error);
    })
  );
};
