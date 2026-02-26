import { Injectable, inject } from '@angular/core';
import { KiotaClientService } from '../kiota-client.service';

@Injectable({
  providedIn: 'root',
})
export class AuthApiService {
  private kiotaClientService = inject(KiotaClientService);

  login(email: string, password: string) {
    return this.kiotaClientService.anonymousClient
      .api.v2.accounts.login.post({ email, password });
  }

  handleError(error: unknown): void {
    this.kiotaClientService.handleError(error);
  }
}
