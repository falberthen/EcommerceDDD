import { Component, OnInit, ViewChild } from '@angular/core';
import { ProductService } from '../../product.service';
import { faPlusCircle } from '@fortawesome/free-solid-svg-icons';
import { CartComponent } from '../cart/cart.component';
import { OrderService } from '../../order.service';
import { Router } from '@angular/router';
import { Product } from 'src/app/core/models/Product';
import { AuthService } from 'src/app/core/services/auth.service';
import { NotificationService } from 'src/app/core/services/notification.service';
import { LocalStorageService } from 'src/app/core/services/local-storage.service';
import { CurrencyNotificationService } from 'src/app/core/services/currency-notification.service';
import { appConstants } from 'src/app/core/constants/appConstants';
import { Quote, QuoteItem } from 'src/app/core/models/Quote';

@Component({
  selector: 'app-products',
  templateUrl: './product-selection.component.html',
  styleUrls: ['./product-selection.component.scss'],
})
export class ProductSelectionComponent implements OnInit {

  @ViewChild('cart') cartDetails!: CartComponent;
  currentCurrency!:string;
  storedCurrency!: string;
  customerId!: string;
  products!: Product[];
  faPlusCircle = faPlusCircle;

  constructor(
    private authService: AuthService,
    private productService: ProductService,
    private localStorageService: LocalStorageService,
    private currencyNotificationService: CurrencyNotificationService
  ) {}

  ngOnInit() {
    this.storedCurrency = this.localStorageService.getValueByKey(appConstants.storedCurrency);

    if(this.authService.currentCustomer) {
      this.customerId = this.authService.currentCustomer.id;
      this.loadProducts();
    }

    // Currency change listener
    this.currencyNotificationService.currentCurrency
      .subscribe(currencyCode => {
        if(currencyCode != '') {
          this.storedCurrency = currencyCode;
          this.loadProducts();
        }
      }
    );
  }

  async loadProducts() {
    this.productService
    .getProducts(this.storedCurrency)
    .then(
      (result: any) => {
        this.products = result.data;
        this.products.forEach((product) => { product.quantity = 0 });
      },
      (error) => console.error(error)
    );
  }

  syncronizeQuoteToProductList(quote: Quote) {
    if(this.products) {
      if(quote.quoteItems.length == 0)
        this.products.forEach((product) => { product.quantity = 0 });

      this.products.forEach((product) => {
        var productFound = quote.quoteItems.filter(
          (quoteItem: QuoteItem) => quoteItem.productId == product.id
        );

        if(productFound.length > 0)
          product.quantity = productFound[0].productQuantity;
        else
          product.quantity  = 0;
      });
    }
  }

  async saveCart(product: Product) {
    product.quantity = product.quantity == 0
      ? 1
      : Number(product.quantity);

    await this.cartDetails.saveQuote(product);
  }

  async placeOrder() {
    await this.cartDetails.placeOrder();
  }
}
