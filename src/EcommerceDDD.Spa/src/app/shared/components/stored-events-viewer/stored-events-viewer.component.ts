import { Component, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { IEventHistory } from '@shared/services/stored-event.service';

@Component({
  selector: 'app-stored-event-viewer',
  templateUrl: './stored-events-viewer.component.html',
  styleUrls: ['./stored-events-viewer.component.scss'],
  
  imports: [CommonModule],
})
export class StoredEventsViewerComponent {
  @Output('destroyComponent') destroyComponent: EventEmitter<any> =
    new EventEmitter();
  eventHistory: IEventHistory[] | undefined;

  close() {
    this.destroyComponent.emit();
  }
}
