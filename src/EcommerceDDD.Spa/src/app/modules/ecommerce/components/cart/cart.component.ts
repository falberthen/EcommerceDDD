import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { faMinusCircle } from '@fortawesome/free-solid-svg-icons';
import { appConstants } from 'src/app/core/constants/appConstants';
import { Quote, QuoteItem } from 'src/app/core/models/Quote';
import { Product } from 'src/app/core/models/Product';
import { ChangeQuoteRequest } from 'src/app/core/models/requests/ChangeQuoteRequest';
import { AuthService } from 'src/app/core/services/auth.service';
import { ConfirmationDialogService } from 'src/app/core/services/confirmation-dialog.service';
import { CurrencyNotificationService } from 'src/app/core/services/currency-notification.service';
import { LocalStorageService } from 'src/app/core/services/local-storage.service';
import { NotificationService } from 'src/app/core/services/notification.service';
import { QuoteService } from '../../quote.service';
import { CreateQuoteRequest } from 'src/app/core/models/requests/CreateQuoteRequest';
import { PlaceOrderRequest } from 'src/app/core/models/requests/PlaceOrderRequest';
import { OrderService } from '../../order.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-cart',
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.scss']
})
export class CartComponent implements OnInit {

  @Output() sendQuoteItemsEvent = new EventEmitter();
  @Output() placeOrderEvent = new EventEmitter();

  quote!: Quote;
  storedCurrency!: string;
  customerId!: string;

  faMinusCircle = faMinusCircle;

  constructor(
    private quoteService: QuoteService,
    private localStorageService: LocalStorageService,
    private confirmationDialogService: ConfirmationDialogService,
    private currencyNotificationService: CurrencyNotificationService,
    private authService: AuthService,
    private notificationService: NotificationService,
    private orderService: OrderService,
    private router: Router) { }

  async ngOnInit() {
    this.storedCurrency = this.localStorageService
      .getValueByKey(appConstants.storedCurrency);

    if(this.authService.currentCustomer) {
      const customer = this.authService.currentCustomer;
      this.customerId = customer.id;
      let quoteId = this.localStorageService
        .getValueByKey(appConstants.storedQuoteId);

      if(quoteId || quoteId === '') {
        quoteId = await this.getCurrentQuote();
      }

      await this.getQuoteDetails(quoteId);
    }

    // Currency change listener
    this.currencyNotificationService.currentCurrency
      .subscribe(async currencyCode => {
        if(currencyCode != '') {
          this.storedCurrency = currencyCode;
          if(this.quote)
            await this.getQuoteDetails(this.quote.quoteId);
        }
      }
    );
  }

  async getCurrentQuote() {
    let quoteId = '';
    await this.quoteService
      .getCurrentQuote(this.customerId)
      .then((result: any) => {
        quoteId = result.data;
        if(!this.isGuidEmpty(quoteId)) {
          this.localStorageService.setValue(appConstants.storedQuoteId, quoteId);
        }
      },
      (error) => console.error(error)
    );

    return quoteId;
  }

  async getQuoteDetails(quoteId: string) {
    if(!this.isGuidEmpty(quoteId)) {
      await this.quoteService
      .getQuoteDetails(quoteId, this.storedCurrency)
      .then(
        (result: any) => {
          if(result.data){
            this.quote = result.data;
            this.localStorageService.setValue(appConstants.storedQuoteItems, this.quote.quoteItems.length);
            this.sendQuoteItemsEvent.emit(this.quote);
          }
        },
        (error) => console.error(error)
      );
    }
  }

  async saveQuote(product: Product) {
    if(!this.quote)
      await this.createQuote(product);
    else
      await this.changeQuote(product);
  }

  async createQuote(product: Product) {
    let request = new CreateQuoteRequest(this.customerId, product, this.storedCurrency);
    this.quoteService.createQuote(request).then(
      async (result: any) => {
        const quoteId = result.data;
        this.localStorageService.setValue(appConstants.storedQuoteId, quoteId);
        await this.getQuoteDetails(quoteId);
      },
      (error) => console.error(error)
    );
  }

  async changeQuote(product: Product) {
    let request = new ChangeQuoteRequest(this.quote.quoteId, product, this.storedCurrency);
    this.quoteService.changeQuote(request).then(
      async (result: any) => {
        const quoteId = result.data;
        await this.getQuoteDetails(quoteId);
      },
      (error) => console.error(error)
    );
  }

  async removeItem(quoteItem: QuoteItem) {
    this.confirmationDialogService
      .confirm('Please confirm', 'Do you confirm you want to remove this item?')
      .then(async (confirmed) => {
        if (confirmed) {
          let product = new Product(quoteItem.productId, "", 0, "", "", 0);
          let request = new ChangeQuoteRequest(this.quote.quoteId, product, this.storedCurrency);

          await this.quoteService.changeQuote(request)
          .then(async (result: any) => {
            const quoteId = result.data;
            await this.getQuoteDetails(quoteId);
          },
          (error) => console.error(error)
        );
      }
    });
  }

  async placeOrder() {
    let request = new PlaceOrderRequest(this.customerId, this.storedCurrency);
    await this.orderService.placeOrder(this.quote.quoteId, request)
      .then((result: any) => {
        this.localStorageService.clearKey(appConstants.storedQuoteId);
        this.localStorageService.clearKey(appConstants.storedQuoteItems);
        this.notificationService.showSuccess('Order placed with success.');
        this.router.navigate(['/orders/' + this.customerId + '/' + result.data]);
      },
      (error) => console.error(error)
    );
  }

  private isGuidEmpty(quoteId: string): boolean {
    return quoteId == '00000000-0000-0000-0000-000000000000';
  }
}
