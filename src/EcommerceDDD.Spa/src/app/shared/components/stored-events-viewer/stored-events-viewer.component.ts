import { Component, Output, EventEmitter } from '@angular/core';
import { IEventHistory } from 'src/app/clients/models';

@Component({
  selector: 'app-stored-event-viewer',
  templateUrl: './stored-events-viewer.component.html',
  styleUrls: ['./stored-events-viewer.component.scss'],
})
export class StoredEventsViewerComponent {
  @Output('destroyComponent') destroyComponent: EventEmitter<any> =
    new EventEmitter();
    eventHistory: IEventHistory[] | undefined;

  close() {
    this.destroyComponent.emit();
  }
}
