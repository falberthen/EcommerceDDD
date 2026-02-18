import { Component, output } from '@angular/core';

import { IEventHistory } from '@shared/services/stored-event.service';

@Component({
  selector: 'app-stored-event-viewer',
  templateUrl: './stored-events-viewer.component.html',
  styleUrls: ['./stored-events-viewer.component.scss']
})
export class StoredEventsViewerComponent {
  readonly destroyComponent = output({ alias: 'destroyComponent' });
  eventHistory: IEventHistory[] | undefined;

  close() {
    this.destroyComponent.emit();
  }
}
