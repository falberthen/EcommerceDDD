import { Injectable, ViewContainerRef } from '@angular/core';
import { StoredEventsViewerComponent } from 'src/app/shared/stored-events-viewer/stored-events-viewer.component';

@Injectable({
  providedIn: 'root'
})
export class StoredEventService {

  public showStoredEvents(storedEventViewContainerRef : ViewContainerRef,
    aggregateType: string, aggregateId: string) {

    storedEventViewContainerRef.clear();
    const componentRef  = storedEventViewContainerRef
      .createComponent(StoredEventsViewerComponent);

    componentRef.instance.aggregateId = aggregateId;
    componentRef.instance.aggregateType = aggregateType;
    componentRef.instance.destroyComponent.subscribe((event: any) => {
      componentRef.destroy();
    });
  }
}
