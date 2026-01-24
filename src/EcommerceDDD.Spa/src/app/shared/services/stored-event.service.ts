import { Injectable, ViewContainerRef } from '@angular/core';
import { StoredEventsViewerComponent } from '@shared/components/stored-events-viewer/stored-events-viewer.component';

export interface IEventHistory {
  id?: string | null;
  aggregateId?: string | null;
  eventTypeName?: string | null;
  eventData?: string | null;
  timestamp?: Date | string | null;
}

@Injectable({
  providedIn: 'root',
})
export class StoredEventService {
  public showStoredEvents(
    storedEventViewContainerRef: ViewContainerRef,
    eventHistory: IEventHistory[] | undefined
  ) {
    storedEventViewContainerRef.clear();
    const componentRef = storedEventViewContainerRef.createComponent(
      StoredEventsViewerComponent
    );

    componentRef.instance.eventHistory = eventHistory;
    componentRef.instance.destroyComponent.subscribe((event: any) => {
      componentRef.destroy();
    });
  }
}
