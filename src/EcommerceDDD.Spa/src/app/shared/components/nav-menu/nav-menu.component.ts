import { Component, OnDestroy, OnInit, ComponentFactoryResolver, ViewChild, ViewContainerRef, ComponentFactory } from '@angular/core';
import { Subscription } from 'rxjs';
import { faList, faShoppingBasket, faShoppingCart, faSignOutAlt, faUser } from '@fortawesome/free-solid-svg-icons';
import { Customer } from 'src/app/core/models/Customer';
import { AuthService } from 'src/app/core/services/auth.service';
import { TokenStorageService } from 'src/app/core/token-storage.service';
import { LocalStorageService } from 'src/app/core/services/local-storage.service';
import { StoredEventsViewerComponent } from 'src/app/modules/ecommerce/components/stored-events-viewer/stored-events-viewer.component';

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
  isLoggedIn: boolean;
  subscription!: Subscription;
  customer!: Customer;
  storedEventsViewerComponentRef: any;

  constructor (
    private authService: AuthService,
    private resolver: ComponentFactoryResolver,
    private tokenStorageService: TokenStorageService,
    private localStorageService: LocalStorageService) {
      this.isLoggedIn = !!this.tokenStorageService.getToken();
      this.setCustomer();
  }

  ngOnInit(){
    this.subscription = this.authService.isLoggedAnnounced$.subscribe(
      response => {
        this.isLoggedIn = response;
        this.setCustomer();
    });
  }

  setCustomer() {
    if(this.isLoggedIn && this.authService.currentCustomer){
        this.customer = this.authService.currentCustomer;
    }
  }

  get cartItems() {
    let cartItems = this.localStorageService.getValueByKey('cartItems');
    return cartItems ? cartItems : '0';
  }

  showCustomerStoredEvents() {
    this.isModalOpen = true;
    this.storedEventViewerContainer.clear();
    const factory = this.resolver.resolveComponentFactory(StoredEventsViewerComponent);
    this.storedEventsViewerComponentRef = this.storedEventViewerContainer.createComponent(factory);
    this.storedEventsViewerComponentRef.instance.aggregateId = this.customer.id;
    this.storedEventsViewerComponentRef.instance.aggregateType = "CustomerStoredEventData";

    this.storedEventsViewerComponentRef.instance.destroyComponent.subscribe((event: any) => {
      this.storedEventsViewerComponentRef.destroy();
      this.isModalOpen = false;
    });
  }

  logout(){
    this.localStorageService.clearAllKeys();
    this.authService.logout();
  }

  ngOnDestroy() {
    // prevent memory leak when component destroyed
    this.subscription.unsubscribe();
    this.isModalOpen = false;
  }
}
