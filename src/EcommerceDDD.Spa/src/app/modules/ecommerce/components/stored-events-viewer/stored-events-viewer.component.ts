import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { StoredEventDataRow } from 'src/app/core/models/StoredEventData';
import { StoredEventService } from '../../stored-event.service';

@Component({
  selector: 'app-stored-event-viewer',
  templateUrl: './stored-events-viewer.component.html',
  styleUrls: ['./stored-events-viewer.component.scss']
})
export class StoredEventsViewerComponent implements OnInit {

  @Output("destroyComponent") destroyComponent: EventEmitter<any> = new EventEmitter();

  aggregateId!: string;
  storedEventData!: [];
  storedEventDataRows!: StoredEventDataRow[];
  aggregateType!: string;

  constructor(private storedEventService: StoredEventService) { }

  ngOnInit() {
    this.loadEventHistory();
  }

  loadEventHistory() {
    if(this.aggregateId && this.aggregateType) {
      switch(this.aggregateType) {
        case "CustomerStoredEventData":
          this.getCustomerStoredEvents();
        break;
        case "OrderStoredEventData":
          this.getOrderStoredEvents();
        break;
      }
    }
  }

  getCustomerStoredEvents() {
    this.storedEventService.getCustomerStoredEvents(this.aggregateId)
    .subscribe((result: any) => {
      this.storedEventData = result.data;
      this.storedEventDataRows = this.getObjectKeys(result.data[0]);
    },
    error => console.error(error));
  }

  getOrderStoredEvents() {
    this.storedEventService.getOrderStoredEvents(this.aggregateId)
    .subscribe((result: any) => {
      this.storedEventData = result.data;
      this.storedEventDataRows = this.getObjectKeys(result.data[0]);
    },
    error => console.error(error));
  }

  getObjectKeys<T>(obj: T): StoredEventDataRow[] {
    let keyList: StoredEventDataRow[] = [];
    if(obj) {
      const objectKeys = Object.keys(obj) as Array<keyof T>;
      let index = 1;

      objectKeys.forEach(key => {

        let record = new StoredEventDataRow(
          key.toString(),
          key === "action" ? 0 : index
        );

        index++;
        keyList.push(record);
      })

      var action = keyList.filter(k => k.columnName == "id");
      if(action.length > 0)
        action[0].position = keyList.length + 1;

    }
    return keyList;
  }

  close() {
    this.destroyComponent.emit();
  }
}
