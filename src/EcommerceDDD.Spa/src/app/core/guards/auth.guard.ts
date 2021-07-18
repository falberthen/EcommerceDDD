
import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Injectable()
export class AuthGuard implements CanActivate {
  constructor(
      private router: Router,
      private authenticationService: AuthService
  ) { }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    const currentCustomer = this.authenticationService.currentCustomer;
    if (this.authenticationService.currentCustomer) {
      return true;
    }

    // not logged in so redirect to login page with the return url
    this.authenticationService.logout();
    return false;
  }
}
