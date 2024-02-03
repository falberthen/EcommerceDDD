import { Component, OnInit, inject } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NotificationService } from '@core/services/notification.service';
import { LoaderService } from '@core/services/loader.service';
import { CustomersService } from '@ecommerce/services/customers.service';
import { RegisterCustomerRequest } from '@ecommerce/models/requests/RegisterCustomerRequest';

@Component({
  selector: 'app-customer-account',
  templateUrl: './customer-account.component.html',
  styleUrls: ['./customer-account.component.scss'],
})
export class CustomerAccountComponent implements OnInit {
  accountForm!: FormGroup;
  returnUrl!: string;
  private router = inject(Router);
  private formBuilder= inject(FormBuilder);
  private route = inject(ActivatedRoute);
  private loaderService = inject(LoaderService);
  private customersService = inject(CustomersService);
  private notificationService = inject(NotificationService)

  ngOnInit() {
    this.accountForm = this.formBuilder.group({
      email: ['', Validators.required],
      name: ['', Validators.required],
      shippingAddress: ['404 Rue Infinite Loop', Validators.required],
      password: ['', Validators.required],
      passwordConfirm: ['', Validators.required],
      creditLimit: ['10000', Validators.required],
    });

    // get return url from route parameters or default to '/'
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
  }

  get isLoading() {
    return this.loaderService.loading$;
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.accountForm.get(fieldName);
    return field!.invalid && field!.touched;
  }

  onSubmit() {
    // stop here if form is invalid
    if (this.accountForm.invalid) {
      return;
    }

    const customerRegistration = new RegisterCustomerRequest(
      this.formControls.email.value,
      this.formControls.name.value,
      this.formControls.shippingAddress.value,
      this.formControls.password.value,
      this.formControls.passwordConfirm.value,
      this.formControls.creditLimit.value
    );

    this.customersService
      .registerCustomer(customerRegistration)
      .subscribe((result) => {
        if (result.success) {
          this.notificationService.showSuccess('Account successfully created!');
          this.router.navigate([this.returnUrl]);
        }
      });
  }

  // getter for easy access to form fields
  private get formControls() {
    return this.accountForm.controls;
  }
}
