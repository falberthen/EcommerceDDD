import { AuthService } from '@core/services/auth.service';
import { TokenStorageService } from '@core/services/token-storage.service';
import { LocalStorageService } from '@core/services/local-storage.service';
import { Component, DestroyRef, OnDestroy, OnInit, ViewContainerRef, inject, viewChild } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
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
import { QuoteNotificationService } from '@features/ecommerce/services/quote-notification.service';
import { CustomerDetails } from 'src/app/clients/models';
import { CurrencyDropdownComponent } from '@features/ecommerce/currency-dropdown/currency-dropdown.component';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.scss'],
  imports: [RouterModule, FontAwesomeModule, CurrencyDropdownComponent],
})
export class NavMenuComponent implements OnInit, OnDestroy {
  private destroyRef = inject(DestroyRef);
  private authService = inject(AuthService);
  private kiotaClientService = inject(KiotaClientService);
  private tokenStorageService = inject(TokenStorageService);
  private localStorageService = inject(LocalStorageService);
  private quoteNotificationService = inject(QuoteNotificationService);

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
  quoteItemsCount = 0;

  async ngOnInit(): Promise<void> {
    this.isLoggedIn = !!this.tokenStorageService.getToken();
    if (this.isLoggedIn) {
      await this.loadQuoteFromServer();
    }

    this.authService.isLoggedAnnounced$
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(async (response) => {
        this.isLoggedIn = response;
        if (this.isLoggedIn) {
          await this.loadCustomerDetails();
          await this.loadQuoteFromServer();
        }
      });

    this.quoteNotificationService.currentQuoteItemsCount
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((count) => {
        this.quoteItemsCount = count;
      });
  }

  get quoteItems(): number {
    return this.quoteItemsCount;
  }

  private async loadQuoteFromServer() {
    try {
      const result = await this.kiotaClientService.client.quoteManagement.api.v2.quotes.get();
      if (result?.quoteId) {
        const count = result.items?.length ?? 0;
        this.quoteItemsCount = count;
        this.quoteNotificationService.changeQuoteItemsCount(count);
      } else {
        this.quoteItemsCount = 0;
        this.quoteNotificationService.changeQuoteItemsCount(0);
      }
    } catch (error) {
      // Silently fail - quote might not exist yet
      this.quoteItemsCount = 0;
    }
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
      JSON.stringify(this.customer)
    );
  }

  private async loadCustomerDetails(): Promise<void> {
    try {
      await this.kiotaClientService.client.customerManagement.api.v2.customers.details
        .get()
        .then((result) => {
          if (result) {
            this.customer = result;
            this.storeLoadedCustomer();
          }
        });
    } catch (error) {
      this.kiotaClientService.handleError(error);
    }
  }
}
