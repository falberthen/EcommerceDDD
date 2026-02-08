import { AuthService } from '@core/services/auth.service';
import { TokenStorageService } from '@core/services/token-storage.service';
import { LocalStorageService } from '@core/services/local-storage.service';
import { ChangeDetectorRef, Component, OnDestroy, OnInit, ViewChild, ViewContainerRef, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import {
  faList,
  faShoppingBasket,
  faShoppingCart,
  faSignOutAlt,
  faUser,
} from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { LOCAL_STORAGE_ENTRIES } from '@ecommerce/constants/appConstants';
import { KiotaClientService } from '@core/services/kiota-client.service';
import { Subscription } from 'rxjs';
import { CustomerDetails, QuoteViewModel } from 'src/app/clients/models';
import { CurrencyDropdownComponent } from '@ecommerce/components/currency-dropdown/currency-dropdown.component';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.scss'],
  
  imports: [CommonModule, RouterModule, FontAwesomeModule, CurrencyDropdownComponent],
})
export class NavMenuComponent implements OnInit, OnDestroy {
  private cdr = inject(ChangeDetectorRef);
  private authService = inject(AuthService);
  private kiotaClientService = inject(KiotaClientService);
  private tokenStorageService = inject(TokenStorageService);
  private localStorageService = inject(LocalStorageService);

  @ViewChild('storedEventViewerContainer', { read: ViewContainerRef })
  storedEventViewerContainer!: ViewContainerRef;

  faList = faList;
  faShoppingBasket = faShoppingBasket;
  faShoppingCart = faShoppingCart;
  faUser = faUser;
  faSignOutAlt = faSignOutAlt;
  isExpanded = false;
  isModalOpen = false;
  isLoggedIn = false;
  subscription!: Subscription;
  customer!: CustomerDetails;

  async ngOnInit() {
    this.subscription = this.authService.isLoggedAnnounced$.subscribe(
      async (response) => {
        this.isLoggedIn = response;
        if (this.isLoggedIn) {
          await this.loadCustomerDetails();
        }
      }
    );
  }

  async ngAfterViewInit() {
    this.isLoggedIn = !!this.tokenStorageService.getToken();
    this.cdr.detectChanges();
  }

  get quoteItems() {
    let quoteStr = this.localStorageService.getValueByKey('openQuote');
    if (quoteStr && quoteStr != 'undefined' && quoteStr != '{}') {
      var quote = JSON.parse(quoteStr) as QuoteViewModel | undefined;
      return quote?.items!.length;
    }

    return 0;
  }

  get loadStoredUser() {
    return this.authService.currentCustomer?.name;
  }

  logout() {
    this.localStorageService.clearAllKeys();
    this.authService.logout();
    this.isLoggedIn = !!this.tokenStorageService.getToken();
  }

  ngOnDestroy() {
    this.subscription.unsubscribe();
    this.isModalOpen = false;
  }

  private async storeLoadedCustomer() {
    this.localStorageService.setValue(
      LOCAL_STORAGE_ENTRIES.storedCustomer,
      JSON.stringify(this.customer)
    );
  }

  private async loadCustomerDetails() {
    try {
      await this.kiotaClientService.client.customerManagement.api.v2.customers.details
        .get()
        .then((result) => {
          if (result!.success) {
            const data = result!.data!;

            const customerDetails: CustomerDetails = {
              id: data.id,
              name: data.name,
              email: data.email,
              shippingAddress: data.shippingAddress,
              creditLimit: data.creditLimit,
            };

            this.customer = customerDetails;
            this.storeLoadedCustomer();
          }
        });
    } catch (error) {
      this.kiotaClientService.handleError(error);
    }
  }
}
