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
import { KiotaClientService } from '@core/services/kiota-client.service';
import {
  AddQuoteItemRequest,
  OpenQuoteRequest,
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
  private kiotaClientService = inject(KiotaClientService);
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
  customerId?: string;
  faMinusCircle = faMinusCircle;
  faList = faList;
  isExpanded = false;
  isModalOpen = false;
  storedEventsViewerComponentRef: any;

  async ngOnInit() {
    const storedCurrency = this.localStorageService.getValueByKey(
      LOCAL_STORAGE_ENTRIES.storedCurrency
    );
    if (storedCurrency) {
      this.currentCurrency = storedCurrency;
    }

    if (this.authService.currentCustomer) {
      const customer = this.authService.currentCustomer;
      this.customerId = customer.id!;
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
      await this.kiotaClientService.client.quoteManagement.api.v2.quotes
        .byQuoteId(this.quote?.quoteId!)
        .history.get()
        .then((result) => {
          if (result) {
            this.storedEventService.showStoredEvents(
              this.storedEventViewerContainer(),
              result
            );
          }
        });
    } catch (error) {
      this.kiotaClientService.handleError(error);
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
            await this.kiotaClientService.client.quoteManagement.api.v2.quotes
              .byQuoteId(this.quote?.quoteId!)
              .delete()
              .then(async () => {
                this.reloadProductsEvent.emit();
                await this.getOpenQuote();
              });
          } catch (error) {
            this.kiotaClientService.handleError(error);
          } finally {
            this.loaderService.setLoading(false);
          }
        }
      });
  }

  async getOpenQuote() {
    try {
      this.loaderService.setLoading(true);
      await this.kiotaClientService.client.quoteManagement.api.v2.quotes.get().then((result) => {
        if (result?.quoteId) {
          this.setQuote(result);
          this.emitQuote(result);
          return;
        }

        this.quote = undefined;
      });
    } catch (error) {
      this.kiotaClientService.handleError(error);
    } finally {
      this.loaderService.setLoading(false);
    }
  }

  async createQuote() {
    const request: OpenQuoteRequest = {
      currencyCode: this.currentCurrency
    };

    this.loaderService.setLoading(true);

    try {
      await this.kiotaClientService.client.quoteManagement.api.v2.quotes.post(request);
      await this.getOpenQuote();

    } catch (error) {
      this.kiotaClientService.handleError(error);
    } finally {
      this.loaderService.setLoading(false);
    }
  }

  async addQuoteItem(product: ProductViewModel) {
    product.quantityAddedToCart =
      product.quantityAddedToCart == 0
        ? 1
        : Number(product.quantityAddedToCart);

    const request: AddQuoteItemRequest = {
      productId: product.productId!,
      quantity: product.quantityAddedToCart
    };

    this.loaderService.setLoading(true);

    try {
      await this.kiotaClientService.client.quoteManagement.api.v2.quotes
        .byQuoteId(this.quote?.quoteId!)
        .items.put(request);

      await this.getOpenQuote();

    } catch (error) {
      this.kiotaClientService.handleError(error);
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
            await this.kiotaClientService.client.quoteManagement.api.v2.quotes
              .byQuoteId(this.quote?.quoteId!)
              .items.byProductId(quoteItem.productId!)
              .delete()
              .then(async () => {
                await this.getOpenQuote();
              });
          } catch (error) {
            this.kiotaClientService.handleError(error);
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
            this.kiotaClientService.client.orderProcessing.api.v2.orders.quote
              .byQuoteId(this.quote.quoteId!)
              .post()
              .then(() => {
                this.quoteNotificationService.changeQuoteItemsCount(0);
                this.notificationService.showSuccess(
                  'Order placed with success.'
                );
                this.router.navigate(['/orders']);
              });
          } catch (error) {
            this.kiotaClientService.handleError(error);
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
