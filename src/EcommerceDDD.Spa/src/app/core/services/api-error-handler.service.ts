import { Injectable, inject } from '@angular/core';
import { NotificationService } from '../services/notification.service';
import { ApiErrorParserService } from '../services/api-error-parser.service';

@Injectable({ providedIn: 'root' })
export class ApiErrorHandlerService {
  private readonly notificationService = inject(NotificationService);
  private readonly apiErrorParser = inject(ApiErrorParserService);

  handle(error: unknown): void {
    const parsed = this.apiErrorParser.parse(error);

    // Remove duplicates and empty messages to avoid notification spam
    const uniqueMessages = [...new Set(parsed.messages)].filter(
      (m) => typeof m === 'string' && m.trim().length > 0
    );

    if (!uniqueMessages.length) {
      this.notificationService.showError('An unexpected error occurred.');
      return;
    }

    // Show messages
    for (const message of uniqueMessages) {
      this.notificationService.showError(message);
    }
  }
}
