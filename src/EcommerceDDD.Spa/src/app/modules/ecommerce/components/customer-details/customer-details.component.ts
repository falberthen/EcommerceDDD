import { faList } from '@fortawesome/free-solid-svg-icons';
import { LoaderService } from '@core/services/loader.service';
import { LocalStorageService } from '@core/services/local-storage.service';
import { NotificationService } from '@core/services/notification.service';
import { StoredEventService } from '@shared/services/stored-event.service';
import { Component, OnInit, ViewChild, ViewContainerRef } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { LOCAL_STORAGE_ENTRIES } from '@ecommerce/constants/appConstants';
import { KiotaClientService } from '@core/services/kiota-client.service';
import { CustomerDetails, UpdateCustomerRequest } from 'src/app/clients/models';

@Component({
  selector: 'app-customer-details',
  templateUrl: './customer-details.component.html',
  styleUrls: ['./customer-details.component.scss'],
})
export class CustomerDetailsComponent implements OnInit {
  @ViewChild('storedEventViewerContainer', { read: ViewContainerRef })
  storedEventViewerContainer!: ViewContainerRef;

  customerDetailsForm!: FormGroup;
  customer!: CustomerDetails;
  faList = faList;

  constructor(
    private formBuilder: FormBuilder,
    private loaderService: LoaderService,
    private notificationService: NotificationService,
    private localStorageService: LocalStorageService,
    private storedEventService: StoredEventService,
    private kiotaClientService: KiotaClientService
  ) {}

  async ngOnInit() {
    await this.loadCustomerDetails();
    if (this.customer) {
      this.customerDetailsForm = this.formBuilder.group({
        name: [this.customer.name, Validators.required],
        shippingAddress: [this.customer.shippingAddress, Validators.required],
        creditLimit: [this.customer.creditLimit, Validators.required],
      });
    }
  }

  get isLoading() {
    return this.loaderService.loading$;
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.customerDetailsForm.get(fieldName);
    return field!.invalid && field!.touched;
  }

  async saveDetails() {
    // stop here if form is invalid
    if (this.customerDetailsForm.invalid) {
      return;
    }

    const customerUpdate : UpdateCustomerRequest = {
      name: this.f.name.value,
      shippingAddress: this.f.shippingAddress.value,
      creditLimit: this.f.creditLimit.value
    }

    try {
      await this.kiotaClientService.client.api.customers.update.put(
        customerUpdate
      );

      this.notificationService.showSuccess('Customer successfully updated!');
      await this.loadCustomerDetails();
    } catch (error) {
      this.kiotaClientService.handleError(error);
    }
  }

  async showCustomerStoredEvents() {
    try {
      await this.kiotaClientService.client.api.customers.history
        .get()
        .then((result) => {
          if (result!.success) {
            this.storedEventService.showStoredEvents(
              this.storedEventViewerContainer,
              result!.data!
            );
          }
        });
    } catch (error) {
      this.kiotaClientService.handleError(error);
    }
  }

  private async storeLoadedCustomer() {
    // storing customer in the localstorage
    this.localStorageService.setValue(
      LOCAL_STORAGE_ENTRIES.storedCustomer,
      JSON.stringify(this.customer)
    );
  }

  private async loadCustomerDetails() {
    try {
      await this.kiotaClientService.client.api.customers.details
        .get()
        .then((result) => {
          if (result!.success) {
            const data = result!.data!;
            const customerDetails : CustomerDetails = {
              id : data.id,
              name: data.name,
              email : data.email,
              shippingAddress : data.shippingAddress,
              creditLimit : data.creditLimit
            };

            this.customer = customerDetails;
            this.storeLoadedCustomer();
          }
        });
    } catch (error) {
      this.kiotaClientService.handleError(error);
    }
  }

  // getter for easy access to form fields
  private get f() {
    return this.customerDetailsForm.controls;
  }
}
