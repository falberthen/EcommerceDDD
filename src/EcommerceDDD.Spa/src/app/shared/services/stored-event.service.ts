import { ComponentRef, Injectable, ViewContainerRef } from '@angular/core';
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
  private currentComponentRef: ComponentRef<StoredEventsViewerComponent> | null = null;
  private currentRefreshFn: (() => Promise<IEventHistory[] | null | undefined>) | null = null;

  public showStoredEvents(
    storedEventViewContainerRef: ViewContainerRef,
    eventHistory: IEventHistory[] | undefined,
    refreshFn?: () => Promise<IEventHistory[] | null | undefined>
  ) {
    storedEventViewContainerRef.clear();
    const componentRef = storedEventViewContainerRef.createComponent(
      StoredEventsViewerComponent
    );

    this.currentComponentRef = componentRef;
    this.currentRefreshFn = refreshFn ?? null;

    componentRef.instance.eventHistory = eventHistory;
    componentRef.instance.destroyComponent.subscribe(() => {
      this.currentComponentRef = null;
      this.currentRefreshFn = null;
      componentRef.destroy();
    });
  }

  public async refreshCurrentViewer(): Promise<void> {
    if (!this.currentComponentRef || !this.currentRefreshFn) return;
    const result = await this.currentRefreshFn();
    if (result && this.currentComponentRef) {
      this.currentComponentRef.instance.eventHistory = result;
    }
  }
}
