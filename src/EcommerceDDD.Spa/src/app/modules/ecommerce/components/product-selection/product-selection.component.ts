import { ChangeDetectorRef, Component, OnInit, ViewChild } from '@angular/core';
import { faPlusCircle } from '@fortawesome/free-solid-svg-icons';
import { CartComponent } from '../cart/cart.component';
import { AuthService } from 'src/app/core/services/auth.service';
import { LocalStorageService } from 'src/app/core/services/local-storage.service';
import { CurrencyNotificationService } from 'src/app/core/services/currency-notification.service';
import { appConstants } from 'src/app/core/constants/appConstants';
import { LoaderService } from 'src/app/core/services/loader.service';
import { ProductsService } from '../../services/products.service';
import { Product } from '../../models/Product';
import { GetProductsRequest } from '../../models/requests/GetProductsRequest';
import { Quote, QuoteItem } from '../../models/Quote';
import { firstValueFrom } from 'rxjs';

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
    private cdr: ChangeDetectorRef,
    private authService: AuthService,
    private loaderService: LoaderService,
    private productsService: ProductsService,
    private localStorageService: LocalStorageService,
    private currencyNotificationService: CurrencyNotificationService,
  ) {}

  async ngOnInit() {
    var storedCurrency = this.localStorageService
      .getValueByKey(appConstants.storedCurrency);

    if(storedCurrency) {
      this.storedCurrency = storedCurrency;
    }

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
      this.cdr.detectChanges();
    });
  }

  syncronizeQuoteToProductList(quote: Quote) {
    if(this.products && quote) {
      this.products.forEach((product) => {
        var productFound = quote.items.filter(
          (quoteItem: QuoteItem) => quoteItem.productId == product.productId
        );

        if(productFound.length > 0) {
          product.quantity = productFound[0].quantity;
        }
      });
    }
  }

  async loadProducts() {
    await firstValueFrom(this.productsService
      .getProducts(new GetProductsRequest(this.storedCurrency)))
      .then(result => {
        this.products = result.data;
        this.products.forEach((product) => { product.quantity = 0 });
      });
  }

  async saveCart(product: Product) {
    if(!this.cart.quote) {
      await this.createQuote();
      await this.addQuoteItem(product);
    }
    else{
      await this.addQuoteItem(product);
    }
  }

  async createQuote() {
    await this.cart.createQuote()
    .then(async result => {
      if(result && result.success) {
        await this.cart.getOpenQuote();
      }
    });
  }

  async addQuoteItem(product: Product) {
    await this.cart.addQuoteItem(product)
    .then(async result => {
      if(result && result.success) {
        await this.cart.getOpenQuote();
      }
    });
  }

  async placeOrder() {
    await this.cart.placeOrder();
  }
}
