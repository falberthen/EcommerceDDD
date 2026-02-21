import { Injectable, inject } from '@angular/core';
import { FetchRequestAdapter } from '@microsoft/kiota-http-fetchlibrary';
import { AnonymousAuthenticationProvider } from '@microsoft/kiota-abstractions';
import { ApiClient, createApiClient } from 'src/app/clients/apiClient';
import { environment } from '@environments/environment';
import { TokenStorageService } from './token-storage.service';
import { BearerTokenAuthProvider } from './bearer-token-auth-provider';
import { ApiErrorHandlerService } from './api-error-handler.service';

@Injectable({
  providedIn: 'root',
})
export class KiotaClientService {
  private readonly tokenService = inject(TokenStorageService);
  private readonly apiErrorHandler = inject(ApiErrorHandlerService);

  private readonly _client: ApiClient;
  private readonly _anonymousClient: ApiClient;

  constructor() {
    // Authenticated client
    const bearerAuthProvider = new BearerTokenAuthProvider(() =>
      Promise.resolve(this.tokenService.getToken() ?? '')
    );

    const requestAdapter = new FetchRequestAdapter(bearerAuthProvider);
    requestAdapter.baseUrl = environment.gatewayBaseUrl;
    this._client = createApiClient(requestAdapter);

    // Anonymous client
    const anonymousAdapter = new FetchRequestAdapter(
      new AnonymousAuthenticationProvider()
    );
    anonymousAdapter.baseUrl = environment.gatewayBaseUrl;
    this._anonymousClient = createApiClient(anonymousAdapter);
  }

  get client(): ApiClient {
    return this._client;
  }

  get anonymousClient(): ApiClient {
    return this._anonymousClient;
  }

  handleError(error: unknown): void {
    console.error('[KiotaClientService Error]', error);
    this.apiErrorHandler.handle(error);
  }
}
