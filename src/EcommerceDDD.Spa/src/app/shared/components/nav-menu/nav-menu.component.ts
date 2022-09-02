import { Component, OnDestroy, OnInit, ComponentFactoryResolver, ViewChild, ViewContainerRef, ComponentFactory } from '@angular/core';
import { Subscription } from 'rxjs';
import { faList, faShoppingBasket, faShoppingCart, faSignOutAlt, faUser } from '@fortawesome/free-solid-svg-icons';
import { Customer } from 'src/app/core/models/Customer';
import { AuthService } from 'src/app/core/services/auth.service';
import { SignalrService } from 'src/app/core/services/signalr.service';
import { TokenStorageService } from 'src/app/core/token-storage.service';
import { LocalStorageService } from 'src/app/core/services/local-storage.service';
import { StoredEventsViewerComponent } from 'src/app/shared/stored-events-viewer/stored-events-viewer.component';
import { appConstants } from 'src/app/core/constants/appConstants';
import { AccountService } from 'src/app/modules/authentication/account.service';
import { Quote } from 'src/app/core/models/Quote';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.scss']
})
export class NavMenuComponent implements OnInit, OnDestroy {

  faList = faList;
  faShoppingBasket = faShoppingBasket;
  faShoppingCart = faShoppingCart;
  faUser = faUser;
  faSignOutAlt = faSignOutAlt;

  @ViewChild("storedEventViewerContainer", { read: ViewContainerRef })
  storedEventViewerContainer!: ViewContainerRef;
  isExpanded = false;
  isModalOpen = false;
  isLoggedIn = false;
  subscription!: Subscription;
  customer!: Customer;
  storedEventsViewerComponentRef: any;

  constructor (
    private authService: AuthService,
    private accountService: AccountService,
    private resolver: ComponentFactoryResolver,
    private tokenStorageService: TokenStorageService,
    private localStorageService: LocalStorageService,
    private signalrService: SignalrService) {
  }

  async ngOnInit(){
    this.isLoggedIn = !!this.tokenStorageService.getToken();
    if(!this.isLoggedIn) {
      this.subscription = this.authService.isLoggedAnnounced$.subscribe(
        async response => {
          this.isLoggedIn = response;
          await this.setCustomer();
      });
    }
    else{
      await this.setCustomer();
    }
  }

  async setCustomer() {
    if(this.isLoggedIn){
      await this.loadCustomerDetails();
    }
  }

  get quoteItems() {
    let quoteStr = this.localStorageService.getValueByKey('openQuote');
    if(quoteStr){
      var quote = JSON.parse(quoteStr) as Quote;
      return quote.items.length;
    }

    return 0;
  }

  //TODO: Move it to a service
  async loadCustomerDetails() {
    this.accountService.loadCustomerDetails()
    .then(result => {
        var data = result.data;
        let latestCustomerDetails = new Customer(
          data.id,
          data.name,
          data.email,
          data.address);

        this.customer = latestCustomerDetails;

        // storing customer in the localstorage
        this.localStorageService.setValue(appConstants.storedCustomer,
          JSON.stringify(this.customer));

        // SignalR
        this.signalrService
          .addCustomerToGroup(this.customer.id, environment.signalrOrdersHubUrl);
      });
  }

  showCustomerStoredEvents() {
    this.isModalOpen = true;
    this.storedEventViewerContainer.clear();
    const factory = this.resolver.resolveComponentFactory(StoredEventsViewerComponent);
    this.storedEventsViewerComponentRef = this.storedEventViewerContainer.createComponent(factory);
    this.storedEventsViewerComponentRef.instance.aggregateId = this.customer.id;
    this.storedEventsViewerComponentRef.instance.aggregateType = "Customers";
    this.storedEventsViewerComponentRef.instance.destroyComponent.subscribe((event: any) => {
      this.storedEventsViewerComponentRef.destroy();
      this.isModalOpen = false;
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
}
