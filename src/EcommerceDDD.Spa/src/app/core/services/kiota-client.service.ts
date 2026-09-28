import { Injectable, Injector, inject } from '@angular/core';
import {
  FetchRequestAdapter,
  KiotaClientFactory,
} from '@microsoft/kiota-http-fetchlibrary';
import { AnonymousAuthenticationProvider } from '@microsoft/kiota-abstractions';
import { ApiClient, createApiClient } from 'src/app/clients/apiClient';
import { environment } from '@environments/environment';
import { TokenStorageService } from './token-storage.service';
import { BearerTokenAuthProvider } from './bearer-token-auth-provider';
import { ApiErrorHandlerService } from './api-error-handler.service';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root',
})
export class KiotaClientService {
  private readonly tokenService = inject(TokenStorageService);
  private readonly apiErrorHandler = inject(ApiErrorHandlerService);
  private readonly injector = inject(Injector);

  private readonly _client: ApiClient;
  private readonly _anonymousClient: ApiClient;

  constructor() {
    // Authenticated client
    const bearerAuthProvider = new BearerTokenAuthProvider(() =>
      Promise.resolve(this.tokenService.getToken() ?? '')
    );

    // Kiota calls fetch directly, so Angular's HttpClient interceptors never see these requests.
    // An expired or invalid session is handled here: a 401 logs the customer out.
    const httpClient = KiotaClientFactory.create(async (url, init) => {
      const response = await fetch(url, init);
      if (response.status === 401) {        
        this.injector.get(AuthService).logout();
      }
      return response;
    });

    const requestAdapter = new FetchRequestAdapter(
      bearerAuthProvider, undefined, undefined, httpClient
    );
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
