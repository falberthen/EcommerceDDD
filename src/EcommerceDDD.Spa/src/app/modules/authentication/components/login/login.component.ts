import { Component, OnInit, inject } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '@core/services/auth.service';
import { LoaderService } from '@core/services/loader.service';

@Component({
    selector: 'app-login',
    templateUrl: './login.component.html',
    styleUrls: ['./login.component.scss'],
    standalone: false
})
export class LoginComponent implements OnInit {
  private router = inject(Router);
  private formBuilder = inject(FormBuilder);
  private route = inject(ActivatedRoute);
  private loaderService = inject(LoaderService);
  private authenticationService = inject(AuthService);

  loginForm!: FormGroup;
  returnUrl!: string;

  constructor() {
    // redirect to home if already logged in
    if (this.authenticationService.currentUser) {
      this.router.navigate(['/home']);
    }
  }

  ngOnInit() {
    this.loginForm = this.formBuilder.group({
      email: ['', Validators.required],
      password: ['', Validators.required],
    });

    // get return url from route parameters or default to '/'
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
  }

  get isLoading() {
    return this.loaderService.loading$;
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.loginForm.get(fieldName);
    return field!.invalid && field!.touched;
  }

  async onSubmit() {
    if (this.loginForm.invalid) {
      return;
    }

    const success = await this.authenticationService.login(
      this.f.email.value,
      this.f.password.value
    );

    if (success) {
      this.router.navigate([this.returnUrl]);
    }
  }

  // getter for easy access to form fields
  private get f() {
    return this.loginForm.controls;
  }
}
