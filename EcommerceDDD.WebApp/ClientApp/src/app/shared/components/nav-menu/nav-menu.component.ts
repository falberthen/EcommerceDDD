import { Component, OnDestroy, OnInit, AfterViewInit, ComponentFactoryResolver, ViewChild, ViewContainerRef } from '@angular/core';
import { AuthService } from 'app/core/services/auth.service';
import { TokenStorageService } from 'app/core/token-storage.service';
import { Subscription } from 'rxjs';
import { Customer } from 'app/core/models/Customer';
import { StoredEventsViewerComponent } from 'app/modules/ecommerce/components/stored-events-viewer/stored-events-viewer.component';
import { LocalStorageService } from 'app/core/services/local-storage.service';
import { faList, faShoppingBasket, faShoppingCart, faSignOutAlt, faUser } from '@fortawesome/free-solid-svg-icons';

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

  @ViewChild("storedEventViewerContainer", { read: ViewContainerRef }) storedEventViewerContainer;

  isExpanded = false;
  isModalOpen = false;
  isLoggedIn: boolean;
  subscription: Subscription;
  customer: Customer;
  storedEventsViewerComponentRef: any;

  constructor (
    private authService: AuthService,
    private resolver: ComponentFactoryResolver,
    private tokenStorageService: TokenStorageService,
    private localStorageService: LocalStorageService) {
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

  get cartItems() {
    let cartItems = this.localStorageService.getValueByKey('cartItems');
    return cartItems ? cartItems : '0';
  }

  getCurrentUserEmail(){
    if(this.isLoggedIn){
      const currentCustomer = this.authService.currentCustomer.subscribe(customer => {
        this.customer = customer;
      });
    }
  }

  showStoredEventsViewer() {
    this.isModalOpen = true;
    this.storedEventViewerContainer.clear();
    const factory = this.resolver.resolveComponentFactory(StoredEventsViewerComponent);
    this.storedEventsViewerComponentRef = this.storedEventViewerContainer.createComponent(factory);
    this.storedEventsViewerComponentRef.instance.aggregateId = this.customer.id;
    this.storedEventsViewerComponentRef.instance.aggregateType = "CustomerStoredEventData";

    this.storedEventsViewerComponentRef.instance.destroyComponent.subscribe(event => {
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
