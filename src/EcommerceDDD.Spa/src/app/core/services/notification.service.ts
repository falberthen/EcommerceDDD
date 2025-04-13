import { Injectable, inject } from '@angular/core';
import { ToastrService } from 'ngx-toastr';

@Injectable({
  providedIn: 'root',
})
export class NotificationService {
  private toastr = inject(ToastrService);


  showSuccess(message: string): void {
    this.toastr.success(message, undefined, { enableHtml: true });
  }

  showError(message: string): void {
    this.toastr.error(message, undefined, { enableHtml: true });
  }
}
