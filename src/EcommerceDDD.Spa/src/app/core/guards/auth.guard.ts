
import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { TokenStorageService } from '../services/token-storage.service';

@Injectable()
export class AuthGuard implements CanActivate {
  constructor(
      private authService: AuthService,
      private tokenStorageToken: TokenStorageService
  ) { }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    if (!this.tokenStorageToken.getToken()) {
      // not logged in so redirect to login page with the return url
      this.authService.logout();
    }
    return true;
  }
}
