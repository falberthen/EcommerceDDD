import { Component, OnInit, inject } from '@angular/core';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { AuthApiService } from '@core/services/api/auth-api.service';

@Component({
  selector: 'app-confirm-email',
  templateUrl: './confirm-email.component.html',
  styleUrls: ['../login/login.component.scss'],
  imports: [RouterModule],
})
export class ConfirmEmailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private authApiService = inject(AuthApiService);

  status: 'confirming' | 'confirmed' | 'failed' = 'confirming';

  async ngOnInit() {
    const { userId, token } = this.route.snapshot.queryParams;
    try {
      await this.authApiService.confirmEmail(userId, token);
      this.status = 'confirmed';
    } catch (error) {
      this.status = 'failed';
      this.authApiService.handleError(error);
    }
  }
}
