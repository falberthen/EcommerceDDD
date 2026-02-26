import { Router } from '@angular/router';
import { faList, faMinusCircle } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { LOCAL_STORAGE_ENTRIES } from '@features/ecommerce/constants/appConstants';
import { AuthService } from '@core/services/auth.service';
import { ConfirmationDialogService } from '@core/services/confirmation-dialog.service';
import { CurrencyNotificationService } from '@features/ecommerce/services/currency-notification.service';
import { QuoteNotificationService } from '@features/ecommerce/services/quote-notification.service';
import { LocalStorageService } from '@core/services/local-storage.service';
import { NotificationService } from '@core/services/notification.service';
import { LoaderService } from '@core/services/loader.service';
import { StoredEventService } from '@shared/services/stored-event.service';
import { Component, DestroyRef, OnInit, ViewContainerRef, inject, output, viewChild } from '@angular/core';
import { QuoteApiService } from '@core/services/api/quote-api.service';
import { OrderApiService } from '@core/services/api/order-api.service';
import {
  ProductViewModel,
  QuoteItemViewModel,
  QuoteViewModel,
} from 'src/app/clients/models';
import { LoaderSkeletonComponent } from '@shared/components/loader-skeleton/loader-skeleton.component';

@Component({
  selector: 'app-cart',
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.scss'],
  
  imports: [FontAwesomeModule, LoaderSkeletonComponent],
})
export class CartComponent implements OnInit {
  private router = inject(Router);
  private authService = inject(AuthService);
  protected loaderService = inject(LoaderService);
  private localStorageService = inject(LocalStorageService);
  private confirmationDialogService = inject(ConfirmationDialogService);
  private currencyNotificationService = inject(CurrencyNotificationService);
  private quoteNotificationService = inject(QuoteNotificationService);
  private notificationService = inject(NotificationService);
  private storedEventService = inject(StoredEventService);
  private quoteApiService = inject(QuoteApiService);
  private orderApiService = inject(OrderApiService);
  private destroyed = false;

  constructor() {
    inject(DestroyRef).onDestroy(() => this.destroyed = true);
  }

  readonly storedEventViewerContainer = viewChild.required('storedEventViewerContainer', { read: ViewContainerRef });

  readonly sendQuoteItemsEvent = output<QuoteViewModel>();
  readonly placeOrderEvent = output();
  readonly reloadProductsEvent = output();

  quote?: QuoteViewModel | undefined;
  currentCurrency!: string;
  faMinusCircle = faMinusCircle;
  faList = faList;

  async ngOnInit() {
    const storedCurrency = this.localStorageService.getValueByKey(
      LOCAL_STORAGE_ENTRIES.storedCurrency
    );
    if (storedCurrency) {
      this.currentCurrency = storedCurrency;
    }

    if (this.authService.currentCustomer) {
      await this.getOpenQuote();
    }

    this.currencyNotificationService.currentCurrency.subscribe(
      async (currencyCode) => {
        if (currencyCode != '') {
          this.currentCurrency = currencyCode;
          await this.getOpenQuote();
        }
      }
    );
  }

  get isLoading() {
    return this.loaderService.loading;
  }

  async showQuoteStoredEvents() {
    try {
      this.loaderService.setLoading(true);
      const quoteId = this.quote?.quoteId!;
      const refreshFn = () => this.quoteApiService.getHistory(quoteId);

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
      this.quoteApiService.handleError(error);
    } finally {
      this.loaderService.setLoading(false);
    }
  }

  async cancelQuote() {
    this.confirmationDialogService
      .confirm(
        'Please confirm',
        'Do you confirm you want to cancel this quote?'
      )
      .then(async (confirmed) => {
        if (confirmed) {
          try {
            this.loaderService.setLoading(true);
            await this.quoteApiService.cancelQuote(this.quote?.quoteId!)
              .then(async () => {
                this.reloadProductsEvent.emit();
                await this.storedEventService.refreshCurrentViewer();
                await this.getOpenQuote();
              });
          } catch (error) {
            this.quoteApiService.handleError(error);
          } finally {
            this.loaderService.setLoading(false);
          }
        }
      });
  }

  async getOpenQuote() {
    try {
      this.loaderService.setLoading(true);
      await this.quoteApiService.getOpenQuote().then((result) => {
        if (result?.quoteId) {
          this.setQuote(result);
          this.emitQuote(result);
          return;
        }

        this.quote = undefined;
      });
    } catch (error) {
      this.quoteApiService.handleError(error);
    } finally {
      this.loaderService.setLoading(false);
    }
  }

  async createQuote() {
    this.loaderService.setLoading(true);

    try {
      await this.quoteApiService.createQuote(this.currentCurrency);
      await this.getOpenQuote();

    } catch (error) {
      this.quoteApiService.handleError(error);
    } finally {
      this.loaderService.setLoading(false);
    }
  }

  async addQuoteItem(product: ProductViewModel) {
    product.quantityAddedToCart =
      product.quantityAddedToCart == 0
        ? 1
        : Number(product.quantityAddedToCart);

    this.loaderService.setLoading(true);

    try {
      await this.quoteApiService.addItem(this.quote?.quoteId!, product.productId!, product.quantityAddedToCart);

      await this.getOpenQuote();
      await this.storedEventService.refreshCurrentViewer();

    } catch (error) {
      this.quoteApiService.handleError(error);
    } finally {
      this.loaderService.setLoading(false);
    }
  }

  async removeItem(quoteItem: QuoteItemViewModel) {
    this.confirmationDialogService
      .confirm('Please confirm', 'Do you confirm you want to remove this item?')
      .then(async (confirmed) => {
        if (confirmed) {
          try {
            this.loaderService.setLoading(true);
            await this.quoteApiService.removeItem(this.quote?.quoteId!, quoteItem.productId!)
              .then(async () => {
                await this.getOpenQuote();
                await this.storedEventService.refreshCurrentViewer();
              });
          } catch (error) {
            this.quoteApiService.handleError(error);
          } finally {
            this.loaderService.setLoading(false);
          }
        }
      });
  }

  async placeOrder() {
    this.confirmationDialogService
      .confirm('Please confirm', 'Do you confirm you want to place an order?')
      .then(async (confirmed) => {
        if (this.quote && confirmed) {
          try {
            this.loaderService.setLoading(true);
            await this.orderApiService.placeOrder(this.quote.quoteId!);
            this.quoteNotificationService.changeQuoteItemsCount(0);
            this.notificationService.showSuccess('Order placed with success.');
            this.router.navigate(['/orders']);
          } catch (error) {
            this.orderApiService.handleError(error);
          } finally {
            this.loaderService.setLoading(false);
          }
        }
      });
  }

  private setQuote(openQuote: QuoteViewModel) {
    if (!openQuote) {
      this.quoteNotificationService.changeQuoteItemsCount(0);
      return;
    }

    // Notify navbar about the change
    this.quoteNotificationService.changeQuoteItemsCount(openQuote.items?.length ?? 0);
  }

  private emitQuote(openQuote: QuoteViewModel) {
    this.quote = openQuote;
    if (!this.destroyed) {
      this.sendQuoteItemsEvent.emit(openQuote);
    }
  }
}
