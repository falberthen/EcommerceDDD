import { Component, OnInit, ViewChild } from '@angular/core';
import { faPlusCircle } from '@fortawesome/free-solid-svg-icons';
import { CartComponent } from '../cart/cart.component';
import { LocalStorageService } from 'src/app/core/services/local-storage.service';
import { CurrencyNotificationService } from 'src/app/modules/ecommerce/services/currency-notification.service';
import { appConstants } from 'src/app/modules/ecommerce/constants/appConstants';
import { LoaderService } from 'src/app/core/services/loader.service';
import { ProductsService } from '../../services/products.service';
import { Product } from '../../models/Product';
import { GetProductsRequest } from '../../models/requests/GetProductsRequest';
import { Quote, QuoteItem } from '../../models/Quote';
import { firstValueFrom, of } from 'rxjs';

@Component({
  selector: 'app-products',
  templateUrl: './product-selection.component.html',
  styleUrls: ['./product-selection.component.scss'],
})
export class ProductSelectionComponent implements OnInit {
  @ViewChild('cart') cart!: CartComponent;
  currentCurrency!: string;
  customerId!: string;
  products: Product[] = [];
  faPlusCircle = faPlusCircle;

  constructor(
    public loaderService: LoaderService,
    private productsService: ProductsService,
    private localStorageService: LocalStorageService,
    private currencyNotificationService: CurrencyNotificationService
  ) {}

  async ngOnInit() {
    var storedCurrency = this.localStorageService.getValueByKey(
      appConstants.storedCurrency
    );

    if (storedCurrency) {
      this.currentCurrency = storedCurrency;
    }

    await this.loadProducts();

    // Currency change listener
    this.currencyNotificationService.currentCurrency.subscribe(
      (currencyCode) => {
        if (currencyCode != '') {
          this.currentCurrency = currencyCode;
          this.loadProducts();
        }
      }
    );
  }

  get isLoading() {
    return this.loaderService.loading$;
  }

  async loadProducts() {
    await firstValueFrom(
      this.productsService.getProducts(
        new GetProductsRequest(this.currentCurrency)
      )
    ).then((result) => {
      this.products = result.data;
      this.products.forEach((product) => {
        product.quantity = 0;
      });
    });
  }

  async saveCart(product: Product) {
    if (!this.cart.quote) {
      await this.createQuote();
    }

    await this.addQuoteItem(product);
  }

  async createQuote() {
    await this.cart.createQuote().then(async (result) => {
      if (result && result.success) {
        await this.cart.getOpenQuote();
      }
    });
  }

  async addQuoteItem(product: Product) {
    await this.cart.addQuoteItem(product).then(async (result) => {
      if (result && result.success) {
        await this.cart.getOpenQuote();
      }
    });
  }

  syncronizeQuoteToProductList(quote: Quote) {
    if (this.products && quote) {
      this.products.forEach((product) => {
        var productFound = quote.items.filter(
          (quoteItem: QuoteItem) => quoteItem.productId == product.productId
        );

        if (productFound.length > 0) {
          product.quantity = productFound[0].quantity;
        }
        else{
          product.quantity = 0;
        }
      });
    }
  }
}
