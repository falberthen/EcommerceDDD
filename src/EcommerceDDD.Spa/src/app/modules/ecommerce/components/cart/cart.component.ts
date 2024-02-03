import { Router } from '@angular/router';
import { faList, faMinusCircle } from '@fortawesome/free-solid-svg-icons';
import { LOCAL_STORAGE_ENTRIES } from '@ecommerce/constants/appConstants';
import { AuthService } from '@core/services/auth.service';
import { ConfirmationDialogService } from '@core/services/confirmation-dialog.service';
import { CurrencyNotificationService } from '@ecommerce/services/currency-notification.service';
import { LocalStorageService } from '@core/services/local-storage.service';
import { NotificationService } from '@core/services/notification.service';
import { LoaderService } from '@core/services/loader.service';
import { QuotesService } from '../../services/quotes.service';
import { Quote, QuoteItem } from '../../models/Quote';
import { Product } from '../../models/Product';
import { RemoveQuoteItemRequest } from '../../models/requests/RemoveQuoteItemRequest';
import { OpenQuoteRequest } from '../../models/requests/OpenQuoteRequest';
import { AddQuoteItemRequest } from '../../models/requests/AddQuoteItemRequest';
import { firstValueFrom } from 'rxjs';
import { ServiceResponse } from '../../services/ServiceResponse';
import { StoredEventService } from '@shared/services/stored-event.service';
import { OrdersService } from '../../services/orders.service';
import {
  Component,
  OnInit,
  Output,
  EventEmitter,
  ViewContainerRef,
  ViewChild,
} from '@angular/core';

@Component({
  selector: 'app-cart',
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.scss'],
})
export class CartComponent implements OnInit {
  @ViewChild('storedEventViewerContainer', { read: ViewContainerRef })
  storedEventViewerContainer!: ViewContainerRef;

  @Output() sendQuoteItemsEvent = new EventEmitter();
  @Output() placeOrderEvent = new EventEmitter();
  @Output() reloadProductsEvent = new EventEmitter();

  quote?: Quote;
  currentCurrency!: string;
  customerId!: string;
  faMinusCircle = faMinusCircle;
  faList = faList;
  isExpanded = false;
  isModalOpen = false;
  storedEventsViewerComponentRef: any;

  constructor(
    private router: Router,
    private authService: AuthService,
    private quotesService: QuotesService,
    private orderService: OrdersService,
    private loaderService: LoaderService,
    private localStorageService: LocalStorageService,
    private confirmationDialogService: ConfirmationDialogService,
    private currencyNotificationService: CurrencyNotificationService,
    private notificationService: NotificationService,
    private storedEventService: StoredEventService
  ) {}

  async ngOnInit() {
    const storedCurrency = this.localStorageService.getValueByKey(
      LOCAL_STORAGE_ENTRIES.storedCurrency
    );
    if (storedCurrency) {
      this.currentCurrency = storedCurrency;
    }

    if (this.authService.currentCustomer) {
      const customer = this.authService.currentCustomer;
      this.customerId = customer.id;
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
    const result = this.quotesService
      .getQuoteStoredEvents(this.quote!.quoteId);

    await firstValueFrom(result).then((result) => {
      if (result.success) {
        this.storedEventService.showStoredEvents(
          this.storedEventViewerContainer,
          result.data
        );
      }
    });
  }

  async cancelQuote() {
    this.confirmationDialogService
      .confirm(
        'Please confirm',
        'Do you confirm you want to cancel this quote?'
      )
      .then(async (confirmed) => {
        if (confirmed) {
          const result = await this.quotesService.cancelQuote(
            this.quote!.quoteId
          );

          await firstValueFrom(result).then((result) => {
            if (result.success) {
              this.reloadProductsEvent.emit();
              this.quote = undefined;
            }
          });
        }
      });
  }

  async getOpenQuote(): Promise<ServiceResponse | undefined> {
    const result = this.quotesService.getOpenQuote(
      this.customerId
    );

    return firstValueFrom(result).then((result) => {
      if (result.success) {
        this.setQuote(result.data);
        this.emitQuote(result.data);
        return result;
      }

      return undefined;
    });
  }

  async createQuote(): Promise<ServiceResponse> {
    const request = new OpenQuoteRequest(
      this.customerId,
      this.currentCurrency);
    return await firstValueFrom(
      this.quotesService.openQuote(request)
    );
  }

  async addQuoteItem(product: Product): Promise<ServiceResponse> {
    product.quantity = product.quantity == 0 ? 1 : Number(product.quantity);
    const request = new AddQuoteItemRequest(
      product.productId,
      product.quantity,
      this.currentCurrency
    );
    return await firstValueFrom(
      this.quotesService
        .addQuoteItem(this.quote!.quoteId, request)
    );
  }

  async removeItem(quoteItem: QuoteItem) {
    this.confirmationDialogService
      .confirm('Please confirm', 'Do you confirm you want to remove this item?')
      .then(async (confirmed) => {
        if (confirmed) {
          const request = new RemoveQuoteItemRequest(
            this.quote!.quoteId,
            quoteItem.productId
          );

          const result = await this.quotesService
            .removeQuoteItem(request);

          await firstValueFrom(result).then(async (result) => {
            if (result.success) {
              await this.getOpenQuote();
            }
          });
        }
      });
  }

  async placeOrder() {
    this.confirmationDialogService
      .confirm('Please confirm', 'Do you confirm you want to place an order?')
      .then(async (confirmed) => {
        if (confirmed) {
          const result = this.orderService.placeOrder(
            this.customerId,
            this.quote?.quoteId!
          );

          await firstValueFrom(result).then((result) => {
            if (result.success) {
              this.localStorageService.clearKey(LOCAL_STORAGE_ENTRIES.storedOpenQuote);
              this.notificationService.showSuccess(
                'Order placed with success.'
              );
              this.router.navigate(['/orders']);
            }
          });
        }
      });
  }

  private setQuote(openQuote: Quote) {
    if (openQuote == null) {
      this.localStorageService.clearKey(LOCAL_STORAGE_ENTRIES.storedOpenQuote);
      return;
    }

    this.localStorageService.setValue(
      LOCAL_STORAGE_ENTRIES.storedOpenQuote,
      JSON.stringify(openQuote)
    );
  }

  private emitQuote(openQuote: Quote) {
    // emiting quote object to product selection
    this.quote = openQuote;
    this.sendQuoteItemsEvent.emit(this.quote);
  }
}
