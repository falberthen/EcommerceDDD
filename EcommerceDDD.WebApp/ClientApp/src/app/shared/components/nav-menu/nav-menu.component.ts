import { Component, OnDestroy, OnInit, AfterViewInit } from '@angular/core';
import { AuthService } from 'app/core/services/auth.service';
import { TokenStorageService } from 'app/core/token-storage.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.scss']
})
export class NavMenuComponent implements OnInit, OnDestroy {
  isExpanded = false;
  isLoggedIn: boolean;
  subscription: Subscription;
  loggedEmail: string;

  constructor (
    private authService: AuthService,
    private tokenStorageService: TokenStorageService) {
      this.isLoggedIn = !!this.tokenStorageService.getToken();
  }

  ngOnInit(){
    this.subscription = this.authService.isLoggedAnnounced$.subscribe(
      response => {
        this.isLoggedIn = response;
        this.getCurrentUserEmail();
    });

    this.getCurrentUserEmail();
  }

  getCurrentUserEmail(){
    if(this.isLoggedIn){
      const currentCustomer = this.authService.currentCustomer.subscribe(customer => {
        this.loggedEmail = customer ? customer.email : '';
      });
    }
  }

  logout(){
    this.authService.logout();
  }

  ngOnDestroy() {
    // prevent memory leak when component destroyed
    this.subscription.unsubscribe();
  }
}
