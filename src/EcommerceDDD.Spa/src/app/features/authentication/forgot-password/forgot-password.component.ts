import { Component, inject } from '@angular/core';
import { RouterModule } from '@angular/router';
import { FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { LoaderService } from '@core/services/loader.service';
import { AuthApiService } from '@core/services/api/auth-api.service';
import { environment } from '@environments/environment';

@Component({
  selector: 'app-forgot-password',
  templateUrl: './forgot-password.component.html',
  styleUrls: ['../login/login.component.scss'],
  imports: [ReactiveFormsModule, RouterModule],
})
export class ForgotPasswordComponent {
  protected loaderService = inject(LoaderService);
  private authApiService = inject(AuthApiService);

  form = inject(FormBuilder).group({
    email: ['', [Validators.required, Validators.email]],
  });
  sent = false;
  readonly mailpitUrl = environment.mailpitUrl;

  async onSubmit() {
    if (this.form.invalid) return;

    try {
      this.loaderService.setLoading(true);
      await this.authApiService.forgotPassword(this.form.value.email!);
      this.sent = true;
    } catch (error) {
      this.authApiService.handleError(error);
    } finally {
      this.loaderService.setLoading(false);
    }
  }
}
