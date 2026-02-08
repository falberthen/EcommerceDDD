import {
  HttpInterceptorFn,
} from '@angular/common/http';
import { inject } from '@angular/core';
import { finalize } from 'rxjs/operators';
import { LoaderService } from '../services/loader.service';

export const loaderInterceptor: HttpInterceptorFn = (req, next) => {
  const loaderService = inject(LoaderService);

  loaderService.setLoading(true);

  return next(req).pipe(
    finalize(() => {
      loaderService.setLoading(false);
    })
  );
};
