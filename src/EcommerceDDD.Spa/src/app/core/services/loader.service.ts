import { Injectable, computed, signal } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class LoaderService {
  // Counts in-flight operations so nested and parallel calls don't clear each other.
  private pending = signal(0);
  loading = computed(() => this.pending() > 0);

  setLoading(loading: boolean) {
    this.pending.update((count) => Math.max(0, count + (loading ? 1 : -1)));
  }
}
