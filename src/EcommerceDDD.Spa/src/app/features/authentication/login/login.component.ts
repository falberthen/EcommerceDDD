import { Component, OnInit, inject } from '@angular/core';
import { Router, ActivatedRoute, RouterModule } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AuthService } from '@core/services/auth.service';
import { LoaderService } from '@core/services/loader.service';
import { AuthApiService } from '@core/services/api/auth-api.service';
import { NotificationService } from '@core/services/notification.service';
import { environment } from '@environments/environment';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
  imports: [ReactiveFormsModule, RouterModule, CommonModule],
})
export class LoginComponent implements OnInit {
  private router = inject(Router);
  private formBuilder = inject(FormBuilder);
  private route = inject(ActivatedRoute);
  protected loaderService = inject(LoaderService);
  private authenticationService = inject(AuthService);
  private authApiService = inject(AuthApiService);
  private notificationService = inject(NotificationService);

  loginForm!: FormGroup;
  returnUrl!: string;
  emailNotConfirmed = false;
  awaitingConfirmation = false;
  readonly mailpitUrl = environment.mailpitUrl;

  constructor() {
    if (this.authenticationService.currentUser) {
      this.router.navigate(['/home']);
    }
  }

  ngOnInit() {
    this.loginForm = this.formBuilder.group({
      email: ['', Validators.required],
      password: ['', Validators.required],
    });

    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
    this.awaitingConfirmation = !!this.route.snapshot.queryParams['registered'];
  }

  get isLoading() {
    return this.loaderService.loading;
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.loginForm.get(fieldName);
    return field!.invalid && field!.touched;
  }

  async onSubmit() {
    if (this.loginForm.invalid) {
      return;
    }

    try {
      this.loaderService.setLoading(true);
      const outcome = await this.authenticationService.login(
        this.f.email.value,
        this.f.password.value
      );

      this.emailNotConfirmed = outcome === 'emailNotConfirmed';
      this.awaitingConfirmation ||= this.emailNotConfirmed;
      if (outcome === 'loggedIn') {
        this.router.navigate([this.returnUrl]);
      }
    } finally {
      this.loaderService.setLoading(false);
    }
  }

  async resendConfirmation() {
    try {
      this.loaderService.setLoading(true);
      await this.authApiService.resendConfirmation(this.f.email.value);
      this.emailNotConfirmed = false;
      this.notificationService.showSuccess('A new confirmation link was sent. Check your inbox.');
    } catch (error) {
      this.authApiService.handleError(error);
    } finally {
      this.loaderService.setLoading(false);
    }
  }

  private get f() {
    return this.loginForm.controls;
  }
}
