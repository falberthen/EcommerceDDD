import {
  AuthenticationProvider,
  RequestInformation,
} from '@microsoft/kiota-abstractions';

export class BearerTokenAuthProvider implements AuthenticationProvider {
  constructor(private getToken: () => Promise<string>) {}

  async authenticateRequest(request: RequestInformation): Promise<void> {
    // Check for opt-out
    const requiresAuth = (request as any).requiresAuth ?? true;
    if (!requiresAuth) return;

    const token = await this.getToken();
    request.headers.set('Authorization', new Set([`Bearer ${token}`]));
  }
}
