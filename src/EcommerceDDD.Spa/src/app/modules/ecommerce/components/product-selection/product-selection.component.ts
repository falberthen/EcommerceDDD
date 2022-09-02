import { Component, OnInit, ViewChild } from '@angular/core';
import { faPlusCircle } from '@fortawesome/free-solid-svg-icons';
import { CartComponent } from '../cart/cart.component';
import { Product } from 'src/app/core/models/Product';
import { AuthService } from 'src/app/core/services/auth.service';
import { LocalStorageService } from 'src/app/core/services/local-storage.service';
import { CurrencyNotificationService } from 'src/app/core/services/currency-notification.service';
import { appConstants } from 'src/app/core/constants/appConstants';
import { Quote, QuoteItem } from 'src/app/core/models/Quote';
import { GetProductsRequest } from 'src/app/core/models/requests/GetProductsRequest';
import { LoaderService } from 'src/app/core/services/loader.service';
import { ProductService } from '../../services/product.service';

@Component({
  selector: 'app-products',
  templateUrl: './product-selection.component.html',
  styleUrls: ['./product-selection.component.scss'],
})
export class ProductSelectionComponent implements OnInit {
  @ViewChild('cart') cart!: CartComponent;
  currentCurrency!:string;
  storedCurrency!: string;
  customerId!: string;
  products!: Product[];
  faPlusCircle = faPlusCircle;
  isLoading = false;

  constructor(
    private authService: AuthService,
    private loaderService: LoaderService,
    private productService: ProductService,
    private localStorageService: LocalStorageService,
    private currencyNotificationService: CurrencyNotificationService
  ) {}

  async ngOnInit() {
    this.storedCurrency = this.localStorageService
      .getValueByKey(appConstants.storedCurrency);

    if(this.authService.currentCustomer) {
      this.customerId = this.authService.currentCustomer.id;
      await this.loadProducts();
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

  ngAfterViewInit() {
    this.loaderService.httpProgress().subscribe((status: boolean) => {
      this.isLoading = status;
    });
  }

  async loadProducts() {
    this.productService
    .getProducts(new GetProductsRequest(this.storedCurrency))
    .then((result: any) => {
        this.products = result.data;
        this.products.forEach((product) => { product.quantity = 0 });
      },
      (error) => console.error(error)
    );
  }

  async syncronizeQuoteToProductList(quote: Quote) {
    if(this.products) {
      if(quote.items.length == 0)
        this.products.forEach((product) => { product.quantity = 0 });

      this.products.forEach((product) => {
        var productFound = quote.items.filter(
          (quoteItem: QuoteItem) => quoteItem.productId == product.id
        );

        if(productFound.length > 0)
          product.quantity = productFound[0].quantity;
        else
          product.quantity  = 0;
      });
    }
  }

  async saveCart(product: Product) {
    await this.cart.saveQuote(product);
  }

  async placeOrder() {
    await this.cart.placeOrder();
  }
}
