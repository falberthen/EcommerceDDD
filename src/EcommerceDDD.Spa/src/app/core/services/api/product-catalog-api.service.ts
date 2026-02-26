import { Injectable, inject } from '@angular/core';
import { KiotaClientService } from '../kiota-client.service';
import { ProductViewModel } from 'src/app/clients/models';

@Injectable({
  providedIn: 'root',
})
export class ProductCatalogApiService {
  private kiotaClientService = inject(KiotaClientService);

  getProducts(currencyCode: string, productIds: string[]): Promise<ProductViewModel[] | null | undefined> {
    return this.kiotaClientService.client
      .productCatalog.api.v2.products.post({ currencyCode, productIds });
  }

  handleError(error: unknown): void {
    this.kiotaClientService.handleError(error);
  }
}
