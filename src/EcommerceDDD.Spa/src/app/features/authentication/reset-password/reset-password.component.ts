import { Component, inject } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { LoaderService } from '@core/services/loader.service';
import { AuthApiService } from '@core/services/api/auth-api.service';
import { NotificationService } from '@core/services/notification.service';

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['../login/login.component.scss'],
  imports: [ReactiveFormsModule, RouterModule],
})
export class ResetPasswordComponent {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  protected loaderService = inject(LoaderService);
  private authApiService = inject(AuthApiService);
  private notificationService = inject(NotificationService);

  form = inject(FormBuilder).group({
    password: ['', Validators.required],
    passwordConfirm: ['', Validators.required],
  });

  async onSubmit() {
    if (this.form.invalid) return;

    const { userId, token } = this.route.snapshot.queryParams;
    const { password, passwordConfirm } = this.form.value;
    try {
      this.loaderService.setLoading(true);
      await this.authApiService.resetPassword(userId, token, password!, passwordConfirm!);
      this.notificationService.showSuccess('Password changed. You can sign in now.');
      this.router.navigate(['/login']);
    } catch (error) {
      this.authApiService.handleError(error);
    } finally {
      this.loaderService.setLoading(false);
    }
  }
}
