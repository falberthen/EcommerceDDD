import { FetchRequestAdapter } from '@microsoft/kiota-http-fetchlibrary';
import { ApiClient, createApiClient } from 'src/app/clients/apiClient';
import { TokenStorageService } from './token-storage.service';
import { BearerTokenAuthProvider } from './bearer-token-auth-provider';
import { AnonymousAuthenticationProvider } from '@microsoft/kiota-abstractions';
import { environment } from '@environments/environment';
import { Injectable } from '@angular/core';
import { NotificationService } from './notification.service';

@Injectable({
  providedIn: 'root',
})
export class KiotaClientService {
  private readonly _client: ApiClient;
  private readonly _anonClient: ApiClient;

  constructor(
    private tokenService: TokenStorageService,
    private notificationService: NotificationService
  ) {
    const bearerAuthProvider = new BearerTokenAuthProvider(() =>
      Promise.resolve(this.tokenService.getToken() ?? '')
    );

    const requestAdapter = new FetchRequestAdapter(bearerAuthProvider);
    requestAdapter.baseUrl = environment.gatewayBaseUrl;
    this._client = createApiClient(requestAdapter);

    const anonAdapter = new FetchRequestAdapter(
      new AnonymousAuthenticationProvider()
    );
    anonAdapter.baseUrl = environment.gatewayBaseUrl;
    this._anonClient = createApiClient(anonAdapter);
  }

  get client(): ApiClient {
    return this._client;
  }

  get anonymousClient(): ApiClient {
    return this._anonClient;
  }

  handleError(error: any): void {
    console.error('[KiotaClientService Error]', error);
    let message = 'An unknown error occurred.';
    // Try to parse Kiota error response body (JSON with `message`)
    if (typeof error?.responseText === 'string') {
      try {
        const parsed = JSON.parse(error.responseText);
        message = parsed?.message || JSON.stringify(parsed);
      } catch {
        message = error.responseText; // fallback to raw text
      }
    }

    message =
      message ||
      error?.additionalData?.message ||
      error?.response?.statusText ||
      error?.message ||
      message;

    this.notificationService.showError(message);
  }
}
