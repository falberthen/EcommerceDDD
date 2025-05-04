import { Component, OnInit, inject } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NotificationService } from '@core/services/notification.service';
import { LoaderService } from '@core/services/loader.service';
import { KiotaClientService } from '@core/services/kiota-client.service';
import { RegisterCustomerRequest } from 'src/app/clients/models';

@Component({
    selector: 'app-customer-account',
    templateUrl: './customer-account.component.html',
    styleUrls: ['./customer-account.component.scss'],
    standalone: false
})
export class CustomerAccountComponent implements OnInit {
  private router = inject(Router);
  private formBuilder = inject(FormBuilder);
  private route = inject(ActivatedRoute);
  private loaderService = inject(LoaderService);
  private kiotaClientService = inject(KiotaClientService);
  private notificationService = inject(NotificationService);

  accountForm!: FormGroup;
  returnUrl!: string;

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

  async onSubmit() {
    if (this.accountForm.invalid) return;

    const customerRegistration: RegisterCustomerRequest = {
      email: this.f.email.value,
      name: this.f.name.value,
      shippingAddress: this.f.shippingAddress.value,
      password: this.f.password.value,
      passwordConfirm: this.f.passwordConfirm.value,
      creditLimit: this.f.creditLimit.value,
    };

    try {
      await this.kiotaClientService.anonymousClient.api.v2.customers.post(
        customerRegistration
      );
      this.notificationService.showSuccess('Account successfully created!');
      this.router.navigate([this.returnUrl]);
    } catch (error) {
      this.kiotaClientService.handleError(error);
    }
  }

  // getter for easy access to form fields
  private get f() {
    return this.accountForm.controls;
  }
}
