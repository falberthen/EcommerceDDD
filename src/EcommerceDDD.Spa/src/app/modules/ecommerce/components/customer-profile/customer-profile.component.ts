import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { appConstants } from 'src/app/core/constants/appConstants';
import { Customer } from 'src/app/core/models/Customer';
import { UpdateCustomerRequest } from 'src/app/core/models/requests/UpdateCustomerRequest';
import { AuthService } from 'src/app/core/services/auth.service';
import { LocalStorageService } from 'src/app/core/services/local-storage.service';
import { NotificationService } from 'src/app/core/services/notification.service';
import { AccountService } from 'src/app/modules/authentication/account.service';

@Component({
  selector: 'app-customer-profile',
  templateUrl: './customer-profile.component.html',
  styleUrls: ['./customer-profile.component.css']
})
export class CustomerProfileComponent implements OnInit {

  customerProfileForm!: FormGroup;
  loading = false;
  submitted = false;
  customer!: Customer;

  constructor(
    private formBuilder: FormBuilder,
    private accountService: AccountService,
    private notificationService: NotificationService,
    private authService: AuthService,
    private localStorageService: LocalStorageService
    ) { }

  ngOnInit() {

    if(this.authService.currentCustomer) {
      this.customer = this.authService.currentCustomer;
      this.customerProfileForm = this.formBuilder.group({
        name: [this.customer.name, Validators.required],
      });
    }
  }

  // getter for easy access to form fields
  get f() {
    return this.customerProfileForm.controls;
  }

  saveProfile() {

    this.submitted = true;

    // stop here if form is invalid
    if (this.customerProfileForm.invalid) {
        return;
    }

    this.loading = true;
    const customerUpdate = new UpdateCustomerRequest(this.f.name.value);

    if(this.customer) {
      this.accountService.updateCustomer(this.customer.id, customerUpdate)
      .subscribe(
        data => {
          this.notificationService.showSuccess("Profile successfully updated!");
          this.setLocalStorageValues(customerUpdate.name);
        },
        error => {
            this.loading = false;
        });
    }
  }

  private setLocalStorageValues(name: string){
    let storedCustomerString = this.localStorageService.getValueByKey(appConstants.storedCustomer);
    let storedCustomer = JSON.parse(storedCustomerString) as any;

    storedCustomer.name = name;

    if(this.customer)
      this.customer.name = name;

    this.localStorageService.setValue(appConstants.storedCustomer, JSON.stringify(storedCustomer));
  }
}
