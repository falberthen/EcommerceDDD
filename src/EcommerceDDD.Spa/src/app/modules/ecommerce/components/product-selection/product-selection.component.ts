import { Component, OnInit, ViewChild, inject } from '@angular/core';
import { faPlusCircle } from '@fortawesome/free-solid-svg-icons';
import { CartComponent } from '../cart/cart.component';
import { LocalStorageService } from '@core/services/local-storage.service';
import { LoaderService } from '@core/services/loader.service';
import { ProductsService } from '../../services/products.service';
import { CurrencyNotificationService } from '../../services/currency-notification.service';
import { Product } from '../../models/Product';
import { GetProductsRequest } from '../../models/requests/GetProductsRequest';
import { Quote, QuoteItem } from '../../models/Quote';
import { Observable, firstValueFrom } from 'rxjs';
import { LOCAL_STORAGE_ENTRIES } from '../../constants/appConstants';

@Component({
  selector: 'app-products',
  templateUrl: './product-selection.component.html',
  styleUrls: ['./product-selection.component.scss'],
})
export class ProductSelectionComponent implements OnInit {
  @ViewChild(CartComponent) cart!: CartComponent;
  currentCurrency!: string;
  customerId!: string;
  products: Product[] = [];
  faPlusCircle = faPlusCircle;

  private loaderService = inject(LoaderService);
  private productsService = inject(ProductsService);
  private localStorageService = inject(LocalStorageService);
  private currencyNotificationService = inject(CurrencyNotificationService);

  async ngOnInit(): Promise<void> {
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

  get isLoading(): Observable<boolean> {
    return this.loaderService.loading$;
  }

  async loadProducts(): Promise<void> {
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

  async saveCart(product: Product): Promise<void> {
    if (!this.cart.quote) {
      await this.createQuote();
    }

    await this.addQuoteItem(product);
  }

  async createQuote(): Promise<void> {
    await this.cart.createQuote().then(async (result) => {
      if (result && result.success) {
        await this.cart.getOpenQuote();
      }
    });
  }

  async addQuoteItem(product: Product): Promise<void> {
    await this.cart.addQuoteItem(product).then(async (result) => {
      if (result && result.success) {
        await this.cart.getOpenQuote();
      }
    });
  }

  syncronizeQuoteToProductList(quote: Quote): void {
    if (this.products && quote) {
      this.products.forEach((product) => {
        const productFound = quote.items.filter(
          (quoteItem: QuoteItem) => quoteItem.productId == product.productId
        );
        product.quantity =  productFound.length > 0 ? productFound[0].quantity : 0;
      });
    }
  }
}
