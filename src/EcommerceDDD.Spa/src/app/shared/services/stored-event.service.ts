import { DestroyRef, Injectable, ViewContainerRef, inject } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { StoredEventData } from 'src/app/modules/ecommerce/models/StoredEventData';
import { StoredEventsViewerComponent } from 'src/app/shared/components/stored-events-viewer/stored-events-viewer.component';

@Injectable({
  providedIn: 'root',
})
export class StoredEventService {
  
  private destroryRef = inject(DestroyRef);

  public showStoredEvents(
    storedEventViewContainerRef: ViewContainerRef,
    storedEventData: StoredEventData[]
  ) {
    storedEventViewContainerRef.clear();
    const componentRef = storedEventViewContainerRef.createComponent(
      StoredEventsViewerComponent
    );

    componentRef.instance.storedEventData = storedEventData;
    componentRef.instance.destroyComponent.pipe(takeUntilDestroyed(this.destroryRef)).subscribe(() => {
      componentRef.destroy();
    });
  }
}
