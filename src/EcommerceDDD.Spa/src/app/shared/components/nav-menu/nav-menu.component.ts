import { AuthService } from '@core/services/auth.service';
import { TokenStorageService } from '@core/services/token-storage.service';
import { LocalStorageService } from '@core/services/local-storage.service';
import { ChangeDetectorRef, Component, DestroyRef, OnDestroy, OnInit, ViewContainerRef, inject, viewChild } from '@angular/core';

import { RouterModule } from '@angular/router';
import {
  faList,
  faShoppingBasket,
  faShoppingCart,
  faSignOutAlt,
  faUser,
} from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { LOCAL_STORAGE_ENTRIES } from '@features/ecommerce/constants/appConstants';
import { KiotaClientService } from '@core/services/kiota-client.service';
import { CustomerDetails, QuoteViewModel } from 'src/app/clients/models';
import { CurrencyDropdownComponent } from '@features/ecommerce/currency-dropdown/currency-dropdown.component';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.scss'],
  
  imports: [RouterModule, FontAwesomeModule, CurrencyDropdownComponent],
})
export class NavMenuComponent implements OnInit, OnDestroy {
  private cdr = inject(ChangeDetectorRef);
  private authService = inject(AuthService);
  private kiotaClientService = inject(KiotaClientService);
  private tokenStorageService = inject(TokenStorageService);
  private localStorageService = inject(LocalStorageService);

  readonly storedEventViewerContainer = viewChild.required('storedEventViewerContainer', { read: ViewContainerRef });

  faList = faList;
  faShoppingBasket = faShoppingBasket;
  faShoppingCart = faShoppingCart;
  faUser = faUser;
  faSignOutAlt = faSignOutAlt;
  isExpanded = false;
  isModalOpen = false;
  isLoggedIn = false;
  customer!: CustomerDetails;

  async ngOnInit(): Promise<void> {
    this.authService.isLoggedAnnounced$.subscribe(
      async (response) => {
        this.isLoggedIn = response;
        if (this.isLoggedIn) {
          await this.loadCustomerDetails();
        }
      }
    );
  }

  async ngAfterViewInit(): Promise<void> {
    this.isLoggedIn = !!this.tokenStorageService.getToken();
    this.cdr.detectChanges();
  }

  get quoteItems(): number {
    const quoteStr = this.localStorageService.getValueByKey('openQuote');
    if (quoteStr && quoteStr !== 'undefined' && quoteStr !== '{}') {
      const quote = JSON.parse(quoteStr) as QuoteViewModel | undefined;
      return quote?.items!.length ?? 0;
    }

    return 0;
  }

  get loadStoredUser(): string | null | undefined {
    return this.authService.currentCustomer?.name ?? null;
  }

  logout(): void {
    this.localStorageService.clearAllKeys();
    this.authService.logout();
    this.isLoggedIn = !!this.tokenStorageService.getToken();
  }

  ngOnDestroy(): void {
    this.isModalOpen = false;
  }

  private async storeLoadedCustomer(): Promise<void> {
    this.localStorageService.setValue(
      LOCAL_STORAGE_ENTRIES.storedCustomer,
      this.customer
    );
  }

  private async loadCustomerDetails(): Promise<void> {
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
