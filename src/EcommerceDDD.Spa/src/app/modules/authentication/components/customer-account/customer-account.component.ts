import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AccountService } from '../../account.service';
import { NotificationService } from 'src/app/core/services/notification.service';
import { RegisterCustomerRequest } from 'src/app/core/models/requests/RegisterCustomerRequest';
import { LoaderService } from 'src/app/core/services/loader.service';

@Component({
  selector: 'app-customer-account',
  templateUrl: './customer-account.component.html',
  styleUrls: ['./customer-account.component.scss']

})
export class CustomerAccountComponent implements OnInit {

  accountForm!: FormGroup;
  isLoading = false;
  submitted = false;
  returnUrl!: string;

  constructor(
    private router: Router,
    private formBuilder: FormBuilder,
    private route: ActivatedRoute,
    private loaderService: LoaderService,
    private accountService: AccountService,
    private notificationService: NotificationService
  ) {
  }

  ngOnInit() {
    this.accountForm = this.formBuilder.group({
      email: ['', Validators.required],
      name: ['', Validators.required],
      address: ['404 Rue Infinite Loop', Validators.required],
      password: ['', Validators.required],
      passwordConfirm: ['', Validators.required],
      availableCreditLimit: ['1000', Validators.required]
    });

    // get return url from route parameters or default to '/'
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
  }

  ngAfterViewInit() {
    this.loaderService.httpProgress().subscribe((status: boolean) => {
      this.isLoading = status;
    });
  }

  onSubmit() {
    this.submitted = true;

    // stop here if form is invalid
    if (this.accountForm.invalid) {
        return;
    }

    const customerRegistration = new RegisterCustomerRequest(
      this.f.email.value,
      this.f.name.value,
      this.f.address.value,
      this.f.password.value,
      this.f.passwordConfirm.value,
      this.f.availableCreditLimit.value);

    this.accountService.registerAccount(customerRegistration)
    .then(data => {
        this.notificationService.showSuccess("Account successfully created!");
        this.router.navigate([this.returnUrl]);
      },
      error => {});
  }

  // getter for easy access to form fields
  get f() {
    return this.accountForm.controls;
  }
}
