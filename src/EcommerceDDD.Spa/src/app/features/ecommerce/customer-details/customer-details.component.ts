import { faList } from '@fortawesome/free-solid-svg-icons';
import { CommonModule } from '@angular/common';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { LoaderService } from '@core/services/loader.service';
import { LocalStorageService } from '@core/services/local-storage.service';
import { NotificationService } from '@core/services/notification.service';
import { StoredEventService } from '@shared/services/stored-event.service';
import { Component, OnInit, ViewContainerRef, inject, viewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { LOCAL_STORAGE_ENTRIES } from '@features/ecommerce/constants/appConstants';
import { KiotaClientService } from '@core/services/kiota-client.service';
import { CustomerDetails, UpdateCustomerRequest } from 'src/app/clients/models';
import { LoaderSkeletonComponent } from '@shared/components/loader-skeleton/loader-skeleton.component';

@Component({
  selector: 'app-customer-details',
  templateUrl: './customer-details.component.html',
  styleUrls: ['./customer-details.component.scss'],
  
  imports: [FontAwesomeModule, ReactiveFormsModule, LoaderSkeletonComponent, RouterModule, CommonModule],
})
export class CustomerDetailsComponent implements OnInit {
  private formBuilder = inject(FormBuilder);
  protected loaderService = inject(LoaderService);
  private notificationService = inject(NotificationService);
  private localStorageService = inject(LocalStorageService);
  private storedEventService = inject(StoredEventService);
  private kiotaClientService = inject(KiotaClientService);

  readonly storedEventViewerContainer = viewChild.required('storedEventViewerContainer', { read: ViewContainerRef });

  customerDetailsForm!: FormGroup;
  customer!: CustomerDetails;
  faList = faList;

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
    return this.loaderService.loading;
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.customerDetailsForm.get(fieldName);
    return field!.invalid && field!.touched;
  }

  async saveDetails() {
    if (this.customerDetailsForm.invalid) {
      return;
    }

    const customerUpdate : UpdateCustomerRequest = {
      name: this.f.name.value,
      shippingAddress: this.f.shippingAddress.value,
      creditLimit: this.f.creditLimit.value
    }

    try {
      this.loaderService.setLoading(true);
      await this.kiotaClientService.client.customerManagement.api.v2.customers.update.put(
        customerUpdate
      );

      this.notificationService.showSuccess('Customer successfully updated!');
      await this.loadCustomerDetails();
      await this.storedEventService.refreshCurrentViewer();
    } catch (error) {
      this.kiotaClientService.handleError(error);
    } finally {
      this.loaderService.setLoading(false);
    }
  }

  async showCustomerStoredEvents() {
    try {
      this.loaderService.setLoading(true);
      const refreshFn = () =>
        this.kiotaClientService.client.customerManagement.api.v2.customers.history.get();

      await refreshFn().then((result) => {
        if (result) {
          this.storedEventService.showStoredEvents(
            this.storedEventViewerContainer(),
            result,
            refreshFn
          );
        }
      });
    } catch (error) {
      this.kiotaClientService.handleError(error);
    } finally {
      this.loaderService.setLoading(false);
    }
  }

  private async storeLoadedCustomer() {
    this.localStorageService.setValue(
      LOCAL_STORAGE_ENTRIES.storedCustomer,
      JSON.stringify(this.customer)
    );
  }

  private async loadCustomerDetails() {
    try {
      this.loaderService.setLoading(true);
      await this.kiotaClientService.client.customerManagement.api.v2.customers.details
        .get()
        .then((result) => {
          if (result) {
            this.customer = result;
            this.storeLoadedCustomer();
          }
        });
    } catch (error) {
      this.kiotaClientService.handleError(error);
    } finally {
      this.loaderService.setLoading(false);
    }
  }

  private get f() {
    return this.customerDetailsForm.controls;
  }
}
