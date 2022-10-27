import { firstValueFrom, Subscription } from 'rxjs';
import { AuthService } from 'src/app/core/services/auth.service';
import { TokenStorageService } from 'src/app/core/services/token-storage.service';
import { LocalStorageService } from 'src/app/core/services/local-storage.service';
import { appConstants } from 'src/app/modules/ecommerce/constants/appConstants';
import { CustomersService } from 'src/app/modules/ecommerce/services/customers.service';
import { Customer } from 'src/app/modules/ecommerce/models/Customer';
import { Quote } from 'src/app/modules/ecommerce/models/Quote';
import { ChangeDetectorRef, Component, OnDestroy, OnInit, ViewChild, ViewContainerRef } from '@angular/core';
import { faList, faShoppingBasket, faShoppingCart, faSignOutAlt, faUser } from '@fortawesome/free-solid-svg-icons';
import { StoredEventService } from 'src/app/shared/services/stored-event.service';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.scss']
})
export class NavMenuComponent implements OnInit, OnDestroy {
  @ViewChild("storedEventViewerContainer", { read: ViewContainerRef })
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
  customer!: Customer;

  constructor (
    private cdr: ChangeDetectorRef,
    private authService: AuthService,
    private customersService: CustomersService,
    private tokenStorageService: TokenStorageService,
    private localStorageService: LocalStorageService,
    private storedEventService: StoredEventService) {
  }

  async ngOnInit(){
    this.subscription = this.authService.isLoggedAnnounced$.subscribe(
      async response => {
        this.isLoggedIn = response;
        if(this.isLoggedIn){
          await this.loadCustomerDetails();
        }
    });
  }

  async ngAfterViewInit() {
    this.isLoggedIn = !!this.tokenStorageService.getToken();
    this.cdr.detectChanges();
  }

  get quoteItems() {
    let quoteStr = this.localStorageService.getValueByKey('openQuote');
    if(quoteStr && quoteStr != "undefined"){
      var quote = JSON.parse(quoteStr) as Quote;
      return quote.items.length;
    }

    return 0;
  }

  async showCustomerStoredEvents() {
    await firstValueFrom((this.customersService
      .getCustomerStoredEvents(this.authService.currentCustomer!.id)))
      .then(result => {
        if(result.success) {
          this.storedEventService
            .showStoredEvents(this.storedEventViewerContainer, result.data);
        }
      });
  }

  logout(){
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
    this.localStorageService.setValue(appConstants.storedCustomer,
      JSON.stringify(this.customer));
  }

  private async loadCustomerDetails() {
    await firstValueFrom(this.customersService.loadCustomerDetails())
    .then(result => {
      if(result.success) {
        var data = result.data;
        this.customer = new Customer(
          data.id,
          data.name,
          data.email,
          data.shippingAddress,
          data.creditLimit);
        this.storeLoadedCustomer();
      }
    });
  }
}
