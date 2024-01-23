import { Component, Output, EventEmitter } from '@angular/core';
import { StoredEventData } from '@ecommerce/models/StoredEventData';

@Component({
  selector: 'app-stored-event-viewer',
  templateUrl: './stored-events-viewer.component.html',
  styleUrls: ['./stored-events-viewer.component.scss'],
})
export class StoredEventsViewerComponent {
   //TODO: This can rather be reworked for simplification and less coupling
  @Output('destroyComponent') destroyComponent: EventEmitter<void> =
    new EventEmitter();
  // TODO: This should be a simple @Input for less coupling
  storedEventData!: StoredEventData[];

  close() {
    this.destroyComponent.emit();
  }
}
