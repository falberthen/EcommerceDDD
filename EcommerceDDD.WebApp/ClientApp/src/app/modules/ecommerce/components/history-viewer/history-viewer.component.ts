import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { CustomerHistoryData } from 'app/core/models/CustomerHistoryData';
import { HistoryViewerService } from '../../history-viewer.service';
import { HistoryData, HistoryDataRow } from 'app/core/models/HistoryData';

@Component({
  selector: 'app-history-viewer',
  templateUrl: './history-viewer.component.html',
  styleUrls: ['./history-viewer.component.scss']
})
export class HistoryViewerComponent implements OnInit {

  @Output("destroyComponent") destroyComponent: EventEmitter<any> = new EventEmitter();

  aggregateId: string;
  historyData: [];
  historyRows: HistoryDataRow[];
  aggregateType: string;

  constructor(private historyViewerService: HistoryViewerService) { }

  ngOnInit() {
    this.loadEventHistory();
  }

  loadEventHistory() {
    if(this.aggregateId && this.aggregateType) {
      switch(this.aggregateType) {
        case "CustomerHistoryData":
          this.getCustomerStoryData();
        break;
      }
    }
  }

  getCustomerStoryData() {
    this.historyViewerService.getCustomerEventHistory(this.aggregateId)
    .subscribe((result: any) => {
      this.historyData = result.data;
      this.historyRows = this.getObjectKeys(result.data[0]);
    },
    error => console.error(error));
  }

  getObjectKeys<T>(obj: T): HistoryDataRow[] {
    let keyList: HistoryDataRow[] = [];
    if(obj) {
      const objectKeys = Object.keys(obj) as Array<keyof T>;
      let index = 1;
      objectKeys.forEach(key => {
        let record = new HistoryDataRow();
        if(key === "action")
          record.position = 0;
        else
          record.position = index;

        record.columnName = key.toString();
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
