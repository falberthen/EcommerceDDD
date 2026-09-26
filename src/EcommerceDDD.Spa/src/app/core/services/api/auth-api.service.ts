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

  confirmEmail(userId: string, token: string) {
    return this.kiotaClientService.anonymousClient
      .api.v2.accounts.confirmEmail.post({ userId, token });
  }

  resendConfirmation(email: string) {
    return this.kiotaClientService.anonymousClient
      .api.v2.accounts.resendConfirmation.post({ email });
  }

  forgotPassword(email: string) {
    return this.kiotaClientService.anonymousClient
      .api.v2.accounts.forgotPassword.post({ email });
  }

  resetPassword(userId: string, token: string, password: string, passwordConfirm: string) {
    return this.kiotaClientService.anonymousClient
      .api.v2.accounts.resetPassword.post({ userId, token, password, passwordConfirm });
  }

  handleError(error: unknown): void {
    this.kiotaClientService.handleError(error);
  }
}
