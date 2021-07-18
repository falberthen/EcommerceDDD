import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { faMinusCircle } from '@fortawesome/free-solid-svg-icons';
import { appConstants } from 'src/app/core/constants/appConstants';
import { Cart, CartItem } from 'src/app/core/models/Cart';
import { Product } from 'src/app/core/models/Product';
import { SaveCartRequest } from 'src/app/core/models/requests/SaveCartRequest';
import { AuthService } from 'src/app/core/services/auth.service';
import { ConfirmationDialogService } from 'src/app/core/services/confirmation-dialog.service';
import { CurrencyNotificationService } from 'src/app/core/services/currency-notification.service';
import { LocalStorageService } from 'src/app/core/services/local-storage.service';
import { NotificationService } from 'src/app/core/services/notification.service';
import { CartService } from '../../cart.service';


@Component({
  selector: 'app-cart-details',
  templateUrl: './cart-details.component.html',
  styleUrls: ['./cart-details.component.scss']
})
export class CartDetailsComponent implements OnInit {

  @Output() sendCartItemsEvent = new EventEmitter();
  @Output() placeOrderEvent = new EventEmitter();

  cart!: Cart;
  storedCurrency!: string;
  customerId!: string;

  faMinusCircle = faMinusCircle;

  constructor(
    private cartService: CartService,
    private localStorageService: LocalStorageService,
    private confirmationDialogService: ConfirmationDialogService,
    private currencyNotificationService: CurrencyNotificationService,
    private notificationService: NotificationService,
    private authService: AuthService) { }

  ngOnInit() {

    this.storedCurrency = this.localStorageService
      .getValueByKey(appConstants.storedCurrency);

    if(this.authService.currentCustomer) {
      const customer = this.authService.currentCustomer;
      this.customerId = customer.id;
      this.getCartDetails();
    }

    // Currency change listener
    this.currencyNotificationService.currentCurrency
      .subscribe(currencyCode => {
        if(currencyCode != '') {
          this.storedCurrency = currencyCode;
          this.getCartDetails();
        }
      }
    );
  }

  getCartDetails() {
    if(this.customerId) {
      this.cartService
      .getCartDetails(this.customerId, this.storedCurrency)
      .subscribe(
        (result: any) => {
          if(result.data){
            this.cart = result.data;
            this.localStorageService.setValue(appConstants.storedCart, this.cart.cartId);
            this.localStorageService.setValue(appConstants.storedCartItems, this.cart.cartItems.length);
            this.sendCartItemsEvent.emit(this.cart);
          }
        },
        (error) => console.error(error)
      );
    }
  }

  removeItem(cartItem: CartItem) {
    this.confirmationDialogService
      .confirm('Please confirm', 'Do you confirm you want to remove this item?')
      .then((confirmed) => {
        if (confirmed) {
          let product = new Product(cartItem.productId, "", 0, "", "", 0);
          let request = new SaveCartRequest(this.customerId, product, this.storedCurrency);

          this.cartService.saveCart(request)
          .subscribe((result: any) => {
              this.notificationService.showSuccess('Item removed with success!');
              this.getCartDetails();
            },
            (error) => console.error(error)
          );
        }
      });
  }

  placeOrder() {
    this.placeOrderEvent.emit();
  }
}
