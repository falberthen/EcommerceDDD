import { Injectable, ViewContainerRef } from '@angular/core';
import { StoredEventsViewerComponent } from '@shared/components/stored-events-viewer/stored-events-viewer.component';
import { StoredEventData } from 'src/app/modules/ecommerce/models/StoredEventData';

@Injectable({
  providedIn: 'root',
})
export class StoredEventService {
  public showStoredEvents(
    storedEventViewContainerRef: ViewContainerRef,
    storedEventData: StoredEventData[]
  ) {
    storedEventViewContainerRef.clear();
    const componentRef = storedEventViewContainerRef.createComponent(
      StoredEventsViewerComponent
    );

    componentRef.instance.storedEventData = storedEventData;
    componentRef.instance.destroyComponent.subscribe((event: any) => {
      componentRef.destroy();
    });
  }
}
