import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { StoredEventData } from 'src/app/core/models/StoredEventData';
import { StoredEventService } from 'src/app/core/services/stored-event.service';

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

  constructor(private storedEventService: StoredEventService) { }

  ngOnInit() {
    this.loadEventHistory();
  }

  loadEventHistory() {
    if(this.aggregateId && this.aggregateType) {
      switch(this.aggregateType) {
        case "Customers":
          this.getCustomersEventHistory();
        break;
        case "Quotes":
          this.getQuotesEventHistory();
        break;
        case "Orders":
          this.getOrdersEventHistory();
        break;
      }
    }
  }

  getCustomersEventHistory() {
    this.storedEventService.getCustomerStoredEvents(this.aggregateId)
    .subscribe((result: any) => {
      this.storedEventData = result.data;
    },
    error => console.error(error));
  }

  getQuotesEventHistory() {
    this.storedEventService.getQuoteStoredEvents(this.aggregateId)
    .subscribe((result: any) => {
      this.storedEventData = result.data;
    },
    error => console.error(error));
  }

  getOrdersEventHistory() {
    this.storedEventService.getOrderStoredEvents(this.aggregateId)
    .subscribe((result: any) => {
      this.storedEventData = result.data;
    },
    error => console.error(error));
  }

  close() {
    this.destroyComponent.emit();
  }
}
