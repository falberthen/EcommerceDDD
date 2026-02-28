import { Component, OnInit, ViewContainerRef, inject, viewChild } from '@angular/core';
import { AsyncPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { faList, faPlusCircle } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { CartComponent } from '../cart/cart.component';
import { LocalStorageService } from '@core/services/local-storage.service';
import { LoaderService } from '@core/services/loader.service';
import { ProductCatalogApiService } from '@core/services/api/product-catalog-api.service';
import { InventoryApiService } from '@core/services/api/inventory-api.service';
import { StoredEventService } from '@shared/services/stored-event.service';
import { SortPipe } from '@core/pipes/sort.pipe';
import { LoaderSkeletonComponent } from '@shared/components/loader-skeleton/loader-skeleton.component';
import {
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
  private productCatalogApiService = inject(ProductCatalogApiService);
  private localStorageService = inject(LocalStorageService);
  private currencyNotificationService = inject(CurrencyNotificationService);

  private inventoryApiService = inject(InventoryApiService);
  private storedEventService = inject(StoredEventService);

  readonly cart = viewChild.required<CartComponent>('cart');
  readonly storedEventViewerContainer = viewChild.required('storedEventViewerContainer', { read: ViewContainerRef });
  currentCurrency!: string;
  customerId!: string;
  products: ProductViewModel[] = [];
  searchTerm = '';
  faPlusCircle = faPlusCircle;
  faList = faList;

  get filteredProducts(): ProductViewModel[] {
    if (!this.searchTerm.trim()) return this.products;
    const term = this.searchTerm.toLowerCase();
    return this.products.filter(p =>
      p.name?.toLowerCase().includes(term) ||
      p.description?.toLowerCase().includes(term) ||
      p.category?.toLowerCase().includes(term)
    );
  }

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
      this.loaderService.setLoading(true);
      await this.productCatalogApiService.getProducts(this.currentCurrency, [])
        .then((result) => {
          if (result) {
            this.products = result;
            // Sync with existing quote after products load
            if (this.cart()?.quote) {
              this.syncronizeQuoteToProductList(this.cart().quote!);
            }
          }
        });
    } catch (error) {
      this.productCatalogApiService.handleError(error);
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
      this.productCatalogApiService.handleError(error);
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

  async showInventoryHistory(product: ProductViewModel) {
    try {
      const productId = product.productId!.toString();
      const refreshFn = () => this.inventoryApiService.getInventoryHistory(productId);

      await refreshFn().then((result) => {
        if (result) {
          this.storedEventService.showStoredEvents(
            this.storedEventViewerContainer(),
            result,
            refreshFn
          );
        }
      });
    } catch (error) {
      this.inventoryApiService.handleError(error);
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
