import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'sort'
})
export class SortPipe implements PipeTransform {
  transform(collection: any[], field: string, direction: 'asc' | 'desc'): any[] {
    if (!collection || !field) {
      return collection;
    }

    const sortedCollection = collection.sort((a, b) => {
      const valueA = a[field];
      const valueB = b[field];

      if (typeof valueA === 'string' && typeof valueB === 'string') {
        return valueA.localeCompare(valueB);
      }

      return valueA - valueB;
    });

    if (direction === 'desc') {
      return sortedCollection.reverse();
    }

    return sortedCollection;
  }
}
