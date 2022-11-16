import { Router } from '@angular/router';
import { Component, OnInit, Output, EventEmitter, ViewContainerRef, ViewChild, ChangeDetectorRef } from '@angular/core';
import { faList, faMinusCircle } from '@fortawesome/free-solid-svg-icons';
import { appConstants } from 'src/app/modules/ecommerce/constants/appConstants';
import { AuthService } from 'src/app/core/services/auth.service';
import { ConfirmationDialogService } from 'src/app/core/services/confirmation-dialog.service';
import { CurrencyNotificationService } from 'src/app/modules/ecommerce/services/currency-notification.service';
import { LocalStorageService } from 'src/app/core/services/local-storage.service';
import { NotificationService } from 'src/app/core/services/notification.service';
import { LoaderService } from 'src/app/core/services/loader.service';
import { QuotesService } from '../../services/quotes.service';
import { Quote, QuoteItem } from '../../models/Quote';
import { Product } from '../../models/Product';
import { RemoveQuoteItemRequest } from '../../models/requests/RemoveQuoteItemRequest';
import { OpenQuoteRequest } from '../../models/requests/OpenQuoteRequest';
import { AddQuoteItemRequest } from '../../models/requests/AddQuoteItemRequest';
import { firstValueFrom } from 'rxjs';
import { ServiceResponse } from '../../services/ServiceResponse';
import { StoredEventService } from 'src/app/shared/services/stored-event.service';
import { OrdersService } from '../../services/orders.service';

@Component({
  selector: 'app-cart',
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.scss']
})
export class CartComponent implements OnInit {
  @ViewChild("storedEventViewerContainer", { read: ViewContainerRef })
  storedEventViewerContainer!: ViewContainerRef;

  @Output() sendQuoteItemsEvent = new EventEmitter();
  @Output() placeOrderEvent = new EventEmitter();
  @Output() reloadProductsEvent = new EventEmitter();

  quote?: Quote;
  storedCurrency!: string;
  customerId!: string;
  faMinusCircle = faMinusCircle;
  faList = faList;
  isExpanded = false;
  isModalOpen = false;
  storedEventsViewerComponentRef: any;
  isLoading = false;

  constructor(
    private router: Router,
    private cdr: ChangeDetectorRef,
    private authService: AuthService,
    private quotesService: QuotesService,
    private orderService: OrdersService,
    private loaderService: LoaderService,
    private localStorageService: LocalStorageService,
    private confirmationDialogService: ConfirmationDialogService,
    private currencyNotificationService: CurrencyNotificationService,
    private notificationService: NotificationService,
    private storedEventService: StoredEventService) { }

  async ngOnInit() {
    var storedCurrency = this.localStorageService
      .getValueByKey(appConstants.storedCurrency);

    if(storedCurrency) {
      this.storedCurrency = storedCurrency;
    }

    if(this.authService.currentCustomer) {
      const customer = this.authService.currentCustomer;
      this.customerId = customer.id;
      await this.getOpenQuote();
    }

    // Currency change listener
    this.currencyNotificationService.currentCurrency
      .subscribe(async currencyCode => {
        if(currencyCode != '') {
          this.storedCurrency = currencyCode;
          await this.getOpenQuote();
        }
      }
    );
  }

  ngAfterViewInit() {
    this.loaderService.httpProgress()
      .subscribe((status: boolean) => {
        this.isLoading = status;
        this.cdr.detectChanges();
    });
  }

  async showQuoteStoredEvents() {
    await firstValueFrom((this.quotesService
      .getQuoteStoredEvents(this.quote!.quoteId)))
      .then(result => {
        if(result.success) {
          this.storedEventService
            .showStoredEvents(this.storedEventViewerContainer, result.data);
        }
      });
  }

  async removeItem(quoteItem: QuoteItem) {
    this.confirmationDialogService
      .confirm('Please confirm', 'Do you confirm you want to remove this item?')
      .then(async (confirmed) => {
        if (confirmed) {
          let request = new RemoveQuoteItemRequest(this.quote!.quoteId, quoteItem.productId);
          await firstValueFrom(await this.quotesService
            .removeQuoteItem(request))
            .then(async result => {
              if(result.success){
                await this.getOpenQuote();
              }
            });
          }
        });
  }

  async cancelQuote(quote: Quote) {
    this.confirmationDialogService
      .confirm('Please confirm', 'Do you confirm you want to cancel this quote?')
      .then(async (confirmed) => {
        if (confirmed) {
          await firstValueFrom(await this.quotesService
            .cancelQuote(quote!.quoteId))
            .then(result => {
              if(result.success){
                this.reloadProductsEvent.emit();
                this.quote = undefined;
              }
            });
          }
      });
  }

  async confirmQuote() {
    await firstValueFrom(this.quotesService
      .confirmQuote(this.quote!.quoteId, this.storedCurrency));
  }

  async placeOrder() {
    await this.confirmQuote();
    await firstValueFrom(this.orderService
    .placeOrderFromQuote(this.quote!.quoteId))
    .then(result => {
      if(result.success) {
        this.localStorageService.clearKey(appConstants.storedOpenQuote);
        this.notificationService.showSuccess('Order placed with success.');
        this.router.navigate(['/orders']);
      }
    })
  }

  async getOpenQuote(): Promise<ServiceResponse | null> {
    return firstValueFrom(this.quotesService
      .getOpenQuote(this.customerId, this.storedCurrency))
      .then(result => {
        if(result.success) {
          this.setQuote(result.data);
          this.emitQuote(result.data);
          return result;
        }

        return null;
    });
  }

  async createQuote(): Promise<ServiceResponse> {
    let request = new OpenQuoteRequest(this.customerId, this.storedCurrency);
    return await firstValueFrom(this.quotesService
      .openQuote(request));
  }

  async addQuoteItem(product: Product): Promise<ServiceResponse> {
    product.quantity = product.quantity == 0 ? 1 : Number(product.quantity);
    const request = new AddQuoteItemRequest(product.productId, product.quantity, this.storedCurrency);
    return await firstValueFrom(this.quotesService
      .addQuoteItem(this.quote!.quoteId, request));
  }

  private setQuote(openQuote: Quote) {
    if(openQuote == null) {
      this.localStorageService.clearKey(appConstants.storedOpenQuote);
      return;
    }

    this.localStorageService
      .setValue(appConstants.storedOpenQuote, JSON.stringify(openQuote));
  }

  private emitQuote(openQuote: Quote) {
    // emiting quote object to product selection
    this.quote = openQuote;
    this.sendQuoteItemsEvent.emit(this.quote);
  }
}
