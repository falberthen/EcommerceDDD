import { Router } from '@angular/router';
import { faList, faMinusCircle } from '@fortawesome/free-solid-svg-icons';
import { LOCAL_STORAGE_ENTRIES } from '@ecommerce/constants/appConstants';
import { AuthService } from '@core/services/auth.service';
import { ConfirmationDialogService } from '@core/services/confirmation-dialog.service';
import { CurrencyNotificationService } from '@ecommerce/services/currency-notification.service';
import { LocalStorageService } from '@core/services/local-storage.service';
import { NotificationService } from '@core/services/notification.service';
import { LoaderService } from '@core/services/loader.service';
import { StoredEventService } from '@shared/services/stored-event.service';
import { Component, OnInit, Output, EventEmitter, ViewContainerRef, ViewChild, inject } from '@angular/core';
import { KiotaClientService } from '@core/services/kiota-client.service';
import {
  AddQuoteItemRequest,
  OpenQuoteRequest,
  ProductViewModel,
  QuoteItemViewModel,
  QuoteViewModel,
} from 'src/app/clients/models';

@Component({
    selector: 'app-cart',
    templateUrl: './cart.component.html',
    styleUrls: ['./cart.component.scss'],
    standalone: false
})
export class CartComponent implements OnInit {
  private router = inject(Router);
  private authService = inject(AuthService);
  private loaderService = inject(LoaderService);
  private localStorageService = inject(LocalStorageService);
  private confirmationDialogService = inject(ConfirmationDialogService);
  private currencyNotificationService = inject(CurrencyNotificationService);
  private notificationService = inject(NotificationService);
  private storedEventService = inject(StoredEventService);
  private kiotaClientService = inject(KiotaClientService);

  @ViewChild('storedEventViewerContainer', { read: ViewContainerRef })
  storedEventViewerContainer!: ViewContainerRef;

  @Output() sendQuoteItemsEvent = new EventEmitter();
  @Output() placeOrderEvent = new EventEmitter();
  @Output() reloadProductsEvent = new EventEmitter();

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

    // Currency change listener
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
    return this.loaderService.loading$;
  }

  async showQuoteStoredEvents() {
    try {
      await this.kiotaClientService.client.api.v2.quotes
        .byQuoteId(this.quote?.quoteId!)
        .history.get()
        .then((result) => {
          if (result!.success) {
            this.storedEventService.showStoredEvents(
              this.storedEventViewerContainer,
              result!.data!
            );
          }
        });
    } catch (error) {
      this.kiotaClientService.handleError(error);
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
            await this.kiotaClientService.client.api.v2.quotes
              .byQuoteId(this.quote?.quoteId!)
              .delete()
              .then(async () => {
                this.reloadProductsEvent.emit();
                await this.getOpenQuote();
              });
          } catch (error) {
            this.kiotaClientService.handleError(error);
          }
        }
      });
  }

  async getOpenQuote() {
    try {
      await this.kiotaClientService.client.api.v2.quotes.get().then((result) => {
        if (result?.data?.quoteId) {
          this.setQuote(result.data!);
          this.emitQuote(result.data!);
          return;
        }

        this.quote = undefined;
      });
    } catch (error) {
      this.kiotaClientService.handleError(error);
    }
  }

  async createQuote() {
    const request : OpenQuoteRequest = {
      currencyCode: this.currentCurrency
    }

    await this.kiotaClientService.client.api.v2.quotes
      .post(request)
      .then(async () => {
        await this.getOpenQuote();
      })
      .catch((error) => {
        this.kiotaClientService.handleError(error);
      });
  }

  async addQuoteItem(product: ProductViewModel) {
    product.quantityAddedToCart =
      product.quantityAddedToCart == 0
        ? 1
        : Number(product.quantityAddedToCart);

    const request : AddQuoteItemRequest = {
      productId: product.productId!,
      quantity: product.quantityAddedToCart
    };

    return await this.kiotaClientService.client.api.v2.quotes
      .byQuoteId(this.quote?.quoteId!)
      .items.put(request)
      .then(async () => {
        await this.getOpenQuote();
      })
      .catch((error) => {
        this.kiotaClientService.handleError(error);
      });
  }

  async removeItem(quoteItem: QuoteItemViewModel) {
    this.confirmationDialogService
      .confirm('Please confirm', 'Do you confirm you want to remove this item?')
      .then(async (confirmed) => {
        if (confirmed) {
          try {
            await this.kiotaClientService.client.api.v2.quotes
              .byQuoteId(this.quote?.quoteId!)
              .items.byProductId(quoteItem.productId!)
              .delete()
              .then(async () => {
                await this.getOpenQuote();
              });
          } catch (error) {
            this.kiotaClientService.handleError(error);
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
            this.kiotaClientService.client.api.v2.orders.quote
              .byQuoteId(this.quote.quoteId!)
              .post()
              .then(() => {
                this.localStorageService.clearKey(
                  LOCAL_STORAGE_ENTRIES.storedOpenQuote
                );
                this.notificationService.showSuccess(
                  'Order placed with success.'
                );
                this.router.navigate(['/orders']);
              });
          } catch (error) {
            this.kiotaClientService.handleError(error);
          }
        }
      });
  }

  private setQuote(openQuote: QuoteViewModel) {
    if (!openQuote) {
      this.localStorageService.clearKey(LOCAL_STORAGE_ENTRIES.storedOpenQuote);
      return;
    }

    this.localStorageService.setValue(
      LOCAL_STORAGE_ENTRIES.storedOpenQuote,
      JSON.stringify(openQuote)
    );
  }

  private emitQuote(openQuote: QuoteViewModel) {
    // emiting quote object to product selection
    this.quote = openQuote;
    this.sendQuoteItemsEvent.emit(this.quote);
  }
}
