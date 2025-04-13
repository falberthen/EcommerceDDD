import { Component, OnInit, ViewChild, inject } from '@angular/core';
import { faPlusCircle } from '@fortawesome/free-solid-svg-icons';
import { CartComponent } from '../cart/cart.component';
import { LocalStorageService } from '@core/services/local-storage.service';
import { LoaderService } from '@core/services/loader.service';
import { CurrencyNotificationService } from '../../services/currency-notification.service';
import { LOCAL_STORAGE_ENTRIES } from '../../constants/appConstants';
import { KiotaClientService } from '@core/services/kiota-client.service';
import {
  GetProductsRequest,
  ProductViewModel,
  QuoteItemViewModel,
  QuoteViewModel,
} from 'src/app/clients/models';

@Component({
    selector: 'app-products',
    templateUrl: './product-selection.component.html',
    styleUrls: ['./product-selection.component.scss'],
    standalone: false
})
export class ProductSelectionComponent implements OnInit {
  loaderService = inject(LoaderService);
  private kiotaClientService = inject(KiotaClientService);
  private localStorageService = inject(LocalStorageService);
  private currencyNotificationService = inject(CurrencyNotificationService);

  @ViewChild('cart') cart!: CartComponent;
  currentCurrency!: string;
  customerId!: string;
  products: ProductViewModel[] = [];
  faPlusCircle = faPlusCircle;

  async ngOnInit() {
    var storedCurrency = this.localStorageService.getValueByKey(
      LOCAL_STORAGE_ENTRIES.storedCurrency
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
    try {
      const request: GetProductsRequest = {
        currencyCode: this.currentCurrency,
        productIds: []
      };

      await this.kiotaClientService.client.api.products
        .post(request)
        .then((result) => {
          if (result?.data) {
            this.products = result.data!;
          }
        });
    } catch (error) {
      this.kiotaClientService.handleError(error);
    }
  }

  async saveCart(product: ProductViewModel) {
    if (!this.cart?.quote) {
      await this.createQuote();
    }

    await this.addQuoteItem(product);
  }

  async createQuote() {
    await this.cart.createQuote();
  }

  async addQuoteItem(product: ProductViewModel) {
    await this.cart.addQuoteItem(product);
  }

  syncronizeQuoteToProductList(quote: QuoteViewModel) {
    if (this.products && quote) {
      this.products.forEach((product) => {
        var productFound = quote.items!.filter(
          (quoteItem: QuoteItemViewModel) =>
            quoteItem.productId == product.productId
        );

        if (productFound.length > 0) {
          product.quantityAddedToCart = productFound[0].quantity;
        } else {
          product.quantityAddedToCart = 0;
        }
      });
    }
  }
}
