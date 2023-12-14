import { firstValueFrom } from 'rxjs';
import { faList } from '@fortawesome/free-solid-svg-icons';
import { LoaderService } from '@core/services/loader.service';
import { LocalStorageService } from '@core/services/local-storage.service';
import { NotificationService } from '@core/services/notification.service';
import { Customer } from '../../models/Customer';
import { UpdateCustomerRequest } from '../../models/requests/UpdateCustomerRequest';
import { CustomersService } from '../../services/customers.service';
import { StoredEventService } from '@shared/services/stored-event.service';
import { Component, OnInit, ViewChild, ViewContainerRef } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { LOCAL_STORAGE_ENTRIES } from '@ecommerce/constants/appConstants';
import { AuthService } from '@core/services/auth.service';

@Component({
  selector: 'app-customer-details',
  templateUrl: './customer-details.component.html',
  styleUrls: ['./customer-details.component.scss'],
})
export class CustomerDetailsComponent implements OnInit {
  @ViewChild('storedEventViewerContainer', { read: ViewContainerRef })
  storedEventViewerContainer!: ViewContainerRef;

  customerDetailsForm!: FormGroup;
  customer!: Customer;
  faList = faList;

  constructor(
    private formBuilder: FormBuilder,
    private loaderService: LoaderService,
    private customersService: CustomersService,
    private notificationService: NotificationService,
    private localStorageService: LocalStorageService,
    private storedEventService: StoredEventService,
    private authService: AuthService
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

    const customerUpdate = new UpdateCustomerRequest(
      this.f.name.value,
      this.f.shippingAddress.value,
      this.f.creditLimit.value
    );

    await firstValueFrom(
      this.customersService.updateCustomer(
        this.customer.id,
        customerUpdate)
    ).then(async () => {
      this.notificationService.showSuccess('Customer successfully updated!');
      await this.loadCustomerDetails();
    });
  }

  async showCustomerStoredEvents() {
    await firstValueFrom(
      this.customersService.getCustomerStoredEvents(
        this.authService.currentCustomer!.id
      )
    ).then((result) => {
      if (result.success) {
        this.storedEventService.showStoredEvents(
          this.storedEventViewerContainer,
          result.data
        );
      }
    });
  }

  private async storeLoadedCustomer() {
    // storing customer in the localstorage
    this.localStorageService.setValue(
      LOCAL_STORAGE_ENTRIES.storedCustomer,
      JSON.stringify(this.customer)
    );
  }

  private async loadCustomerDetails() {
    await firstValueFrom(this.customersService.loadCustomerDetails()).then(
      (result) => {
        if (result.success) {
          var data = result.data;
          this.customer = new Customer(
            data.id,
            data.name,
            data.email,
            data.shippingAddress,
            data.creditLimit
          );
          this.storeLoadedCustomer();
        }
      }
    );
  }

  // getter for easy access to form fields
  private get f() {
    return this.customerDetailsForm.controls;
  }
}
