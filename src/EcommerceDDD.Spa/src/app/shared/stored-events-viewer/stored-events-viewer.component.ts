import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { boundedContexts } from 'src/app/core/constants/appConstants';
import { StoredEventData } from 'src/app/modules/ecommerce/models/StoredEventData';
import { CustomersService } from 'src/app/modules/ecommerce/services/customers.service';
import { OrdersService } from 'src/app/modules/ecommerce/services/orders.service';
import { QuotesService } from 'src/app/modules/ecommerce/services/quotes.service';

@Component({
  selector: 'app-stored-event-viewer',
  templateUrl: './stored-events-viewer.component.html',
  styleUrls: ['./stored-events-viewer.component.scss']
})
export class StoredEventsViewerComponent implements OnInit {
  @Output("destroyComponent") destroyComponent: EventEmitter<any> = new EventEmitter();

  aggregateId!: string;
  storedEventData!: StoredEventData[];
  aggregateType!: string;

  constructor(
    private customersService: CustomersService,
    private quotesService: QuotesService,
    private ordersService: OrdersService) { }

  ngOnInit() {
    this.loadEventHistory();
  }

  async loadEventHistory() {
    if(this.aggregateId && this.aggregateType) {
      switch(this.aggregateType) {
        case boundedContexts.Customers:
          await this.getCustomersEventHistory();
        break;
        case boundedContexts.Quotes:
          await this.getQuotesEventHistory();
        break;
        case boundedContexts.Orders:
          await this.getOrdersEventHistory();
        break;
      }
    }
  }

  async getCustomersEventHistory() {
    await firstValueFrom(this.customersService
    .getCustomerStoredEvents(this.aggregateId))
    .then(result => {
      if(result.success) {
        this.storedEventData = result.data;
      }
    });
  }

  async getQuotesEventHistory() {
    await firstValueFrom(this.quotesService
      .getQuoteStoredEvents(this.aggregateId))
      .then(result => {
        if(result.success) {
          this.storedEventData = result.data;
        }
      });
  }

  async getOrdersEventHistory() {
    await firstValueFrom(this.ordersService
      .getOrderStoredEvents(this.aggregateId))
      .then(result => {
        if(result.success) {
          this.storedEventData = result.data;
        }
      });
  }

  close() {
    this.destroyComponent.emit();
  }
}
