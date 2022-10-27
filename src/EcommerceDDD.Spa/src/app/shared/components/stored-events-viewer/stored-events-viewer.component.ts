import { Component, Output, EventEmitter } from '@angular/core';
import { StoredEventData } from 'src/app/modules/ecommerce/models/StoredEventData';

@Component({
  selector: 'app-stored-event-viewer',
  templateUrl: './stored-events-viewer.component.html',
  styleUrls: ['./stored-events-viewer.component.scss']
})
export class StoredEventsViewerComponent {
  @Output("destroyComponent") destroyComponent: EventEmitter<any> = new EventEmitter();
  storedEventData!: StoredEventData[];

  close() {
    this.destroyComponent.emit();
  }
}
