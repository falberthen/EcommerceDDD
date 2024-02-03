import { firstValueFrom, Subscription } from 'rxjs';
import { AuthService } from '@core/services/auth.service';
import { TokenStorageService } from '@core/services/token-storage.service';
import { LocalStorageService } from '@core/services/local-storage.service';
import { CustomersService } from '@ecommerce/services/customers.service';
import { Customer } from '@ecommerce/models/Customer';
import { Quote } from '@ecommerce/models/Quote';
import {
  ChangeDetectorRef,
  Component,
  inject,
  OnDestroy,
  OnInit,
} from '@angular/core';
import {
  faList,
  faShoppingBasket,
  faShoppingCart,
  faSignOutAlt,
  faUser,
} from '@fortawesome/free-solid-svg-icons';
import { LOCAL_STORAGE_ENTRIES } from '@ecommerce/constants/appConstants';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.scss'],
})
export class NavMenuComponent implements OnInit, OnDestroy {

  faList = faList;
  faShoppingBasket = faShoppingBasket;
  faShoppingCart = faShoppingCart;
  faUser = faUser;
  faSignOutAlt = faSignOutAlt;
  isExpanded = false;
  isModalOpen = false;
  isLoggedIn = false;
  subscription!: Subscription;
  customer!: Customer;

  private cdr = inject(ChangeDetectorRef);
  private authService  = inject(AuthService);
  private customersService = inject(CustomersService);
  private tokenStorageService = inject(TokenStorageService);
  private localStorageService = inject(LocalStorageService);

  async ngOnInit(): Promise<void>{
    this.subscription = this.authService.isLoggedAnnounced$.subscribe(
      async (response) => {
        this.isLoggedIn = response;
        if (this.isLoggedIn) {
          await this.loadCustomerDetails();
        }
      }
    );
  }

  async ngAfterViewInit(): Promise<void>{
    this.isLoggedIn = !!this.tokenStorageService.getToken();
    this.cdr.detectChanges(); 
  }

  get quoteItems(): number{
    let quoteStr = this.localStorageService.getValueByKey('openQuote');
    if (quoteStr && quoteStr != 'undefined') {
      const quote = JSON.parse(quoteStr) as Quote;
      return quote.items.length;
    }

    return 0;
  }

  get loadStoredUser() {
    return this.authService.currentCustomer?.email;
  }

  logout(): void {
    this.localStorageService.clearAllKeys();
    this.authService.logout();
    this.isLoggedIn = !!this.tokenStorageService.getToken();
  }

  ngOnDestroy(): void{
    // prevent memory leak when component destroyed
    this.subscription.unsubscribe();
    this.isModalOpen = false;
  }

  private async storeLoadedCustomer(): Promise <void>{
    // storing customer in the localstorage
    this.localStorageService.setValue(
      LOCAL_STORAGE_ENTRIES.storedCustomer,
      JSON.stringify(this.customer)
    );
  }

  private async loadCustomerDetails(): Promise <void>{
    await firstValueFrom(this.customersService.loadCustomerDetails()).then(
      (result) => {
        if (result.success) {
          const data = result.data;
          this.customer = new Customer(
            data.id,
            data.name,
            data.email,
            data.shippingAddress,
            data.creditLimit
          );
          this.storeLoadedCustomer();
        }
      }
    );
  }
}
