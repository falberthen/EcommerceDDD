import { Component, OnInit, inject, viewChild } from '@angular/core';
import { AsyncPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { faPlusCircle } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { CartComponent } from '../cart/cart.component';
import { LocalStorageService } from '@core/services/local-storage.service';
import { LoaderService } from '@core/services/loader.service';
import { KiotaClientService } from '@core/services/kiota-client.service';
import { SortPipe } from '@core/pipes/sort.pipe';
import { LoaderSkeletonComponent } from '@shared/components/loader-skeleton/loader-skeleton.component';
import {
  GetProductsRequest,
  ProductViewModel,
  QuoteItemViewModel,
  QuoteViewModel,
} from 'src/app/clients/models';
import { CurrencyNotificationService } from '@features/ecommerce/services/currency-notification.service';
import { LOCAL_STORAGE_ENTRIES } from '@features/ecommerce/constants/appConstants';

@Component({
  selector: 'app-products',
  templateUrl: './product-selection.component.html',
  styleUrls: ['./product-selection.component.scss'],
  
  imports: [FontAwesomeModule, CartComponent, FormsModule, SortPipe, LoaderSkeletonComponent],
})
export class ProductSelectionComponent implements OnInit {
  protected loaderService = inject(LoaderService);
  private kiotaClientService = inject(KiotaClientService);
  private localStorageService = inject(LocalStorageService);
  private currencyNotificationService = inject(CurrencyNotificationService);

  readonly cart = viewChild.required<CartComponent>('cart');
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
    return this.loaderService.loading;
  }

  async loadProducts() {
    try {
      const request: GetProductsRequest = {
        currencyCode: this.currentCurrency,
        productIds: []
      };

      this.loaderService.setLoading(true);
      await this.kiotaClientService.client.productCatalog.api.v2.products
        .post(request)
        .then((result) => {
          if (result?.data) {
            this.products = result.data!;
            // Sync with existing quote after products load
            if (this.cart()?.quote) {
              this.syncronizeQuoteToProductList(this.cart().quote!);
            }
          }
        });
    } catch (error) {
      this.kiotaClientService.handleError(error);
    } finally {
      this.loaderService.setLoading(false);
    }
  }

  async saveCart(product: ProductViewModel) {
    try {
      if (!this.cart()?.quote) {
        this.loaderService.setLoading(true);
        await this.createQuote();
      }

      await this.addQuoteItem(product);
    } catch (error) {
      this.kiotaClientService.handleError(error);
    } finally {
      this.loaderService.setLoading(false);
    }
  }

  async createQuote() {
    try {
      this.loaderService.setLoading(true);
      await this.cart().createQuote();
    } finally {
      this.loaderService.setLoading(false);
    }
  }

  async addQuoteItem(product: ProductViewModel) {
    try {
      this.loaderService.setLoading(true);
      await this.cart().addQuoteItem(product);
    } finally {
      this.loaderService.setLoading(false);
    }
  }

  syncronizeQuoteToProductList(quote: QuoteViewModel) {
    if (!quote || !this.products) return;
    
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
