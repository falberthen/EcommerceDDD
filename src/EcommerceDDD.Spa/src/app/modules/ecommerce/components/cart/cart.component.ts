import { Component, OnInit, Output, EventEmitter, ViewContainerRef, ComponentFactoryResolver, ViewChild } from '@angular/core';
import { faList, faMinusCircle } from '@fortawesome/free-solid-svg-icons';
import { appConstants } from 'src/app/core/constants/appConstants';
import { Quote, QuoteItem } from 'src/app/core/models/Quote';
import { Product } from 'src/app/core/models/Product';
import { AddQuoteItemRequest } from 'src/app/core/models/requests/AddQuoteItemRequest';
import { AuthService } from 'src/app/core/services/auth.service';
import { ConfirmationDialogService } from 'src/app/core/services/confirmation-dialog.service';
import { CurrencyNotificationService } from 'src/app/core/services/currency-notification.service';
import { LocalStorageService } from 'src/app/core/services/local-storage.service';
import { NotificationService } from 'src/app/core/services/notification.service';
import { OpenQuoteRequest } from 'src/app/core/models/requests/OpenQuoteRequest';
import { Router } from '@angular/router';
import { RemoveQuoteItemRequest } from 'src/app/core/models/requests/RemoveQuoteItemRequest';
import { StoredEventsViewerComponent } from '../../../../shared/stored-events-viewer/stored-events-viewer.component';
import { LoaderService } from 'src/app/core/services/loader.service';
import { QuoteService } from '../../services/quote.service';

@Component({
  selector: 'app-cart',
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.scss']
})
export class CartComponent implements OnInit {
  @Output() sendQuoteItemsEvent = new EventEmitter();
  @Output() placeOrderEvent = new EventEmitter();
  @Output() reloadProductsEvent = new EventEmitter();

  @ViewChild("storedEventViewerContainer", { read: ViewContainerRef })
  storedEventViewerContainer!: ViewContainerRef;

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
    private quoteService: QuoteService,
    private localStorageService: LocalStorageService,
    private confirmationDialogService: ConfirmationDialogService,
    private currencyNotificationService: CurrencyNotificationService,
    private authService: AuthService,
    private notificationService: NotificationService,
    private router: Router,
    private resolver: ComponentFactoryResolver,
    private loaderService: LoaderService) { }

  async ngOnInit() {
    this.storedCurrency = this.localStorageService
      .getValueByKey(appConstants.storedCurrency);

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
    this.loaderService.httpProgress().subscribe((status: boolean) => {
      this.isLoading = status;
    });
  }

  async saveQuote(product: Product) {
    if(!this.quote) {
      await this.createQuote()
      .then(async (result: any) => {
          await this.getOpenQuote();
          await this.addQuoteItem(product);
        },
        (error) => console.error(error)
      );
    }
    else{
      await this.addQuoteItem(product);
    }
  }

  async removeItem(quoteItem: QuoteItem) {
    this.confirmationDialogService
      .confirm('Please confirm', 'Do you confirm you want to remove this item?')
      .then(async (confirmed) => {
        if (confirmed) {
          let request = new RemoveQuoteItemRequest(this.quote!.quoteId, quoteItem.productId);
          await this.quoteService.removeQuoteItem(request)
          .then(async (result: any) => {
            await this.getOpenQuote();
          },
          (error) => console.error(error)
        );
      }
    });
  }

  async cancelQuote(quote: Quote) {
    this.confirmationDialogService
      .confirm('Please confirm', 'Do you confirm you want to cancel this quote?')
      .then(async (confirmed) => {
        if (confirmed) {
          await this.quoteService.cancelQuote(this.quote!.quoteId)
          .then(async (result: any) => {
            this.reloadProductsEvent.emit();
            this.quote = undefined;
          },
          (error) => console.error(error)
        );
      }
    });
  }

  async placeOrder() {
    await this.quoteService.placeOrder(this.quote!.quoteId, this.storedCurrency)
      .then((result: any) => {
        this.localStorageService.clearKey(appConstants.storedOpenQuote);
        this.notificationService.showSuccess('Order placed with success.');
        this.router.navigate(['/orders']);
      },
      (error) => console.error(error)
    );
  }

  showQuoteStoredEvents() {
    this.isModalOpen = true;
    this.storedEventViewerContainer.clear();
    const factory = this.resolver.resolveComponentFactory(StoredEventsViewerComponent);
    this.storedEventsViewerComponentRef = this.storedEventViewerContainer.createComponent(factory);
    this.storedEventsViewerComponentRef.instance.aggregateId = this.quote!.quoteId;
    this.storedEventsViewerComponentRef.instance.aggregateType = "Quotes";

    this.storedEventsViewerComponentRef.instance.destroyComponent.subscribe((event: any) => {
      this.storedEventsViewerComponentRef.destroy();
      this.isModalOpen = false;
    });
  }

  private async getOpenQuote() {
    await this.quoteService
      .getOpenQuote(this.customerId, this.storedCurrency)
      .then((result: any) => {
        if(result.data == null) {
          this.localStorageService.clearKey(appConstants.storedOpenQuote);
          return;
        }

        this.quote = result.data;
        this.sendQuoteItemsEvent.emit(this.quote);
        this.localStorageService
          .setValue(appConstants.storedOpenQuote, JSON.stringify(this.quote));
      },
      (error) => console.error(error)
    );
  }

  private async createQuote() {
    let request = new OpenQuoteRequest(this.customerId, this.storedCurrency);
    await this.quoteService.openQuote(request);
  }

  private async addQuoteItem(product: Product) {
    product.quantity = product.quantity == 0 ? 1 : Number(product.quantity);
    let request = new AddQuoteItemRequest(product.id, product.quantity, this.storedCurrency);
    await this.quoteService.addQuoteItem(this.quote!.quoteId, request).then(
      async (result: any) => {
        await this.getOpenQuote();
      },
      (error) => console.error(error)
    );
  }
}
