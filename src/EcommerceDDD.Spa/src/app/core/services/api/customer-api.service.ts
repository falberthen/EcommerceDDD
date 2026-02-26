import { Injectable, inject } from '@angular/core';
import { KiotaClientService } from '../kiota-client.service';
import { RegisterCustomerRequest, UpdateCustomerRequest } from 'src/app/clients/models';

@Injectable({
  providedIn: 'root',
})
export class CustomerApiService {
  private kiotaClientService = inject(KiotaClientService);

  getCustomerDetails() {
    return this.kiotaClientService.client
      .customerManagement.api.v2.customers.details.get();
  }

  updateCustomer(request: UpdateCustomerRequest) {
    return this.kiotaClientService.client
      .customerManagement.api.v2.customers.update.put(request);
  }

  getHistory() {
    return this.kiotaClientService.client
      .customerManagement.api.v2.customers.history.get();
  }

  registerCustomer(request: RegisterCustomerRequest) {
    return this.kiotaClientService.client
      .customerManagement.api.v2.customers.post(request);
  }

  handleError(error: unknown): void {
    this.kiotaClientService.handleError(error);
  }
}
