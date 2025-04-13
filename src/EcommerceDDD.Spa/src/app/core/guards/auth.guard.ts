import { Injectable, inject } from '@angular/core';
import { ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { TokenStorageService } from '../services/token-storage.service';

@Injectable()
export class AuthGuard {
  private authService = inject(AuthService);
  private tokenStorageToken = inject(TokenStorageService);


  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    if (!this.tokenStorageToken.getToken()) {
      // not logged in so redirect to login page with the return url
      this.authService.logout();
    }
    return true;
  }
}
