import { firstValueFrom } from 'rxjs';
import { LoaderService } from 'src/app/core/services/loader.service';
import { LocalStorageService } from 'src/app/core/services/local-storage.service';
import { NotificationService } from 'src/app/core/services/notification.service';
import { Customer } from '../../models/Customer';
import { UpdateCustomerRequest } from '../../models/requests/UpdateCustomerRequest';
import { CustomersService } from '../../services/customers.service';
import { StoredEventService } from 'src/app/shared/services/stored-event.service';
import { ChangeDetectorRef, Component, OnInit, ViewChild, ViewContainerRef } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { appConstants } from '../../constants/appConstants';

@Component({
  selector: 'app-customer-details',
  templateUrl: './customer-details.component.html',
  styleUrls: ['./customer-details.component.scss']
})
export class CustomerDetailsComponent implements OnInit {
  @ViewChild("storedEventViewerContainer", { read: ViewContainerRef })
  storedEventViewerContainer!: ViewContainerRef;

  customerDetailsForm!: FormGroup;
  submitted = false;
  customer!: Customer;
  isLoading = false;

  constructor(
    private cdr: ChangeDetectorRef,
    private formBuilder: FormBuilder,
    private loaderService: LoaderService,
    private customersService: CustomersService,
    private notificationService: NotificationService,
    private localStorageService: LocalStorageService,
    private storedEventService: StoredEventService) {}

  async ngOnInit() {
    await this.loadCustomerDetails();
    if(this.customer) {
      this.customerDetailsForm = this.formBuilder.group({
        name: [this.customer.name, Validators.required],
        shippingAddress: [this.customer.shippingAddress, Validators.required],
        creditLimit: [this.customer.creditLimit, Validators.required]
      });
    }
  }

  ngAfterViewChecked() {
    this.loaderService.httpProgress().subscribe((status: boolean) => {
      this.isLoading = status;
      this.cdr.detectChanges();
    });
  }

  async saveDetails() {
    this.submitted = true;

    // stop here if form is invalid
    if (this.customerDetailsForm.invalid) {
        return;
    }

    const customerUpdate = new UpdateCustomerRequest(
      this.f.name.value,
      this.f.shippingAddress.value,
      this.f.creditLimit.value);

      this.customersService.updateCustomer(this.customer.id, customerUpdate)
        .subscribe(async data => {
            this.notificationService.showSuccess("Customer successfully updated!");
            await this.loadCustomerDetails();
          },
        error => {});
  }

  // getter for easy access to form fields
  get f() {
    return this.customerDetailsForm.controls;
  }

  private async storeLoadedCustomer() {
    // storing customer in the localstorage
    this.localStorageService.setValue(appConstants.storedCustomer,
      JSON.stringify(this.customer));
  }

  private async loadCustomerDetails() {
    await firstValueFrom(this.customersService.loadCustomerDetails())
    .then(result => {
      if(result.success) {
        var data = result.data;
        this.customer = new Customer(
          data.id,
          data.name,
          data.email,
          data.shippingAddress,
          data.creditLimit);
          this.storeLoadedCustomer();
      }
    });
  }
}
