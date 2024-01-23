import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { TokenStorageService } from '../services/token-storage.service';
import { CanActivateFn } from '@angular/router';

export const canActivateGuard: CanActivateFn = ()=> {
    if (!inject(TokenStorageService).getToken()) {
      // not logged in so redirect to login page with the return url
      inject(AuthService).logout();
    }
    return true;
}