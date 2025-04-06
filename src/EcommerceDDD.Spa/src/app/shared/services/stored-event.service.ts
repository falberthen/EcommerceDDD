import { Injectable, ViewContainerRef } from '@angular/core';
import { StoredEventsViewerComponent } from '@shared/components/stored-events-viewer/stored-events-viewer.component';
import { IEventHistory } from 'src/app/clients/models';

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
