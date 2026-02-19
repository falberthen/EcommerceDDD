import { Injectable, signal } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class LoaderService {
  loading = signal(false);

  setLoading(loading: boolean) {
    this.loading.set(loading);
  }
}
