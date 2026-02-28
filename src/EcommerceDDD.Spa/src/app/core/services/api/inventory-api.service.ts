import { Injectable, inject } from '@angular/core';
import { KiotaClientService } from '../kiota-client.service';
import { InventoryStockUnitEventHistory } from 'src/app/clients/models';

@Injectable({
  providedIn: 'root',
})
export class InventoryApiService {
  private kiotaClientService = inject(KiotaClientService);

  getInventoryHistory(productId: string): Promise<InventoryStockUnitEventHistory[] | undefined> {
    return this.kiotaClientService.client
      .inventoryManagement.api.v2.inventory.byProductId(productId).history.get();
  }

  handleError(error: unknown): void {
    this.kiotaClientService.handleError(error);
  }
}
