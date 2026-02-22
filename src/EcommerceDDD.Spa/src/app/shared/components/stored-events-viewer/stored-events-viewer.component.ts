import { Component, output } from '@angular/core';
import { DatePipe } from '@angular/common';

import { IEventHistory } from '@shared/services/stored-event.service';

@Component({
  selector: 'app-stored-event-viewer',
  templateUrl: './stored-events-viewer.component.html',
  styleUrls: ['./stored-events-viewer.component.scss'],
  imports: [DatePipe]
})
export class StoredEventsViewerComponent {
  readonly destroyComponent = output();
  eventHistory: IEventHistory[] | undefined;

  close() {
    this.destroyComponent.emit();
  }

  formatEventName(name: string | null | undefined): string {
    if (!name) return '';
    return name
      .replace(/Event$/, '')
      .replace(/([A-Z])/g, ' $1')
      .trim();
  }

  parseEventData(data: string | null | undefined): { key: string; value: string }[] {
    if (!data) return [];
    try {
      const parsed = JSON.parse(data);
      return this.flattenObject(parsed);
    } catch {
      return [{ key: 'data', value: data }];
    }
  }

  formatKey(key: string): string {
    return key
      .replace(/([A-Z])/g, ' $1')
      .replace(/\./g, ' › ')
      .replace(/\s+/g, ' ')
      .trim();
  }

  private flattenObject(obj: any, prefix = ''): { key: string; value: string }[] {
    return Object.entries(obj).flatMap(([key, value]) => {
      const fullKey = prefix ? `${prefix}.${key}` : key;
      if (value !== null && typeof value === 'object') {
        if (Array.isArray(value)) {
          if (value.length === 0) return [];
          if (typeof value[0] === 'object' && value[0] !== null) {
            return value.flatMap((item: any, i: number) =>
              this.flattenObject(item, `${fullKey}.${i}`)
            );
          }
          return [{ key: fullKey, value: value.join(', ') }];
        }
        return this.flattenObject(value, fullKey);
      }
      return [{ key: fullKey, value: String(value) }];
    });
  }
}
