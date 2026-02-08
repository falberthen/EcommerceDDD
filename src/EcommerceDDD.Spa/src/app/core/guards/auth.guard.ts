import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { TokenStorageService } from '../services/token-storage.service';

export const authGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const tokenStorageService = inject(TokenStorageService);

  if (!tokenStorageService.getToken()) {
    authService.logout();
  }
  return true;
};
