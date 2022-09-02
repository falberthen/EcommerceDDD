import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { appConstants } from 'src/app/core/constants/appConstants';
import { Customer } from 'src/app/core/models/Customer';
import { UpdateCustomerRequest } from 'src/app/core/models/requests/UpdateCustomerRequest';
import { AuthService } from 'src/app/core/services/auth.service';
import { LoaderService } from 'src/app/core/services/loader.service';
import { LocalStorageService } from 'src/app/core/services/local-storage.service';
import { NotificationService } from 'src/app/core/services/notification.service';
import { AccountService } from 'src/app/modules/authentication/account.service';

@Component({
  selector: 'app-customer-details',
  templateUrl: './customer-details.component.html',
  styleUrls: ['./customer-details.component.css']
})
export class CustomerDetailsComponent implements OnInit {

  customerDetailsForm!: FormGroup;
  submitted = false;
  customer!: Customer;
  isLoading = false;

  constructor(
    private formBuilder: FormBuilder,
    private loaderService: LoaderService,
    private accountService: AccountService,
    private notificationService: NotificationService,
    private authService: AuthService,
    private localStorageService: LocalStorageService) {}

  async ngOnInit() {

    this.customer = this.authService.currentCustomer!;
    if(this.customer) {
      this.customerDetailsForm = this.formBuilder.group({
        name: [this.customer.name, Validators.required],
        address: [this.customer.address, Validators.required],
      });
    }
  }

  ngAfterViewInit() {
    this.loaderService.httpProgress().subscribe((status: boolean) => {
      this.isLoading = status;
    });
  }

  saveDetails() {
    this.submitted = true;

    // stop here if form is invalid
    if (this.customerDetailsForm.invalid) {
        return;
    }

    const customerUpdate = new UpdateCustomerRequest(this.f.name.value, this.f.address.value);
    if(this.customer) {
      this.accountService.updateCustomer(this.customer.id, customerUpdate)
      .then(data => {
          this.notificationService.showSuccess("Customer successfully updated!");
          this.setLocalStorageValues(customerUpdate);
        },
        error => {});
    }
  }

  // getter for easy access to form fields
  get f() {
    return this.customerDetailsForm.controls;
  }

  private setLocalStorageValues(customerUpdateRequest: UpdateCustomerRequest){
    let storedCustomerString = this.localStorageService.getValueByKey(appConstants.storedCustomer);
    let storedCustomer = JSON.parse(storedCustomerString) as any;

    storedCustomer.name = customerUpdateRequest.name;
    storedCustomer.address = customerUpdateRequest.address;

    if(this.customer) {
      this.customer.name = customerUpdateRequest.name;
      this.customer.address = customerUpdateRequest.address;
    }

    this.localStorageService.setValue(appConstants.storedCustomer, JSON.stringify(storedCustomer));
  }
}
