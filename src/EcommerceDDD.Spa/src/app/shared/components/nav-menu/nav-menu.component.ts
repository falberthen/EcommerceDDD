import { AuthService } from '@core/services/auth.service';
import { TokenStorageService } from '@core/services/token-storage.service';
import { LocalStorageService } from '@core/services/local-storage.service';
import {
  ChangeDetectorRef,
  Component,
  OnDestroy,
  OnInit,
  ViewChild,
  ViewContainerRef,
} from '@angular/core';
import {
  faList,
  faShoppingBasket,
  faShoppingCart,
  faSignOutAlt,
  faUser,
} from '@fortawesome/free-solid-svg-icons';
import { LOCAL_STORAGE_ENTRIES } from '@ecommerce/constants/appConstants';
import { KiotaClientService } from '@core/services/kiota-client.service';
import { Subscription } from 'rxjs';
import { CustomerDetails, QuoteViewModel } from 'src/app/clients/models';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.scss'],
})
export class NavMenuComponent implements OnInit, OnDestroy {
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

  constructor(
    private cdr: ChangeDetectorRef,
    private authService: AuthService,
    private kiotaClientService: KiotaClientService,
    private tokenStorageService: TokenStorageService,
    private localStorageService: LocalStorageService
  ) {}

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
    // prevent memory leak when component destroyed
    this.subscription.unsubscribe();
    this.isModalOpen = false;
  }

  private async storeLoadedCustomer() {
    // storing customer in the localstorage
    this.localStorageService.setValue(
      LOCAL_STORAGE_ENTRIES.storedCustomer,
      JSON.stringify(this.customer)
    );
  }

  private async loadCustomerDetails() {
    try {
      await this.kiotaClientService.client.api.customers.details
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
