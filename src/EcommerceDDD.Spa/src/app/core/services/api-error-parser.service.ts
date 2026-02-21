import { Injectable } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';

export interface ParsedApiError {
  status?: number;
  messages: string[];
  raw?: unknown;
}

@Injectable({ providedIn: 'root' })
export class ApiErrorParserService {
  parse(error: unknown): ParsedApiError {
    // Angular HttpClient errors come as HttpErrorResponse
    if (error instanceof HttpErrorResponse) {
      return this.parseHttpError(error);
    }

    // Kiota/fetch errors usually have a different shape
    return this.parseKiotaOrUnknown(error);
  }

  private parseHttpError(error: HttpErrorResponse): ParsedApiError {
    const status = error.status;
    const payload = error.error;

    // 1) ValidationProblemDetails or validation dictionary (highest priority)
    // Example:
    // { errors: { email: ["Invalid email"], password: ["Required"] } }
    const validationMessages = this.extractValidationDictionary(payload);
    if (validationMessages.length) {
      return { status, messages: validationMessages, raw: error };
    }

    // 2) Plain string body (legacy backend or simple error responses)
    if (typeof payload === 'string' && payload.trim()) {
      return { status, messages: [payload], raw: error };
    }

    // 3) ProblemDetails / custom JSON object
    if (payload && typeof payload === 'object') {
      const message =
        this.asNonEmptyString((payload as any).detail) || // ProblemDetails
        this.asNonEmptyString((payload as any).message) || // Legacy custom shape
        this.asNonEmptyString((payload as any).title); // ProblemDetails fallback

      if (message) {
        return { status, messages: [message], raw: error };
      }
    }

    // 4) Fallback to HttpErrorResponse message
    return {
      status,
      messages: [error.message || 'An unexpected server error occurred.'],
      raw: error,
    };
  }

  private parseKiotaOrUnknown(error: unknown): ParsedApiError {
    const e = error as any;

    // Kiota error shapes vary depending on adapter and generated models
    const status =
      e?.responseStatusCode ??
      e?.statusCode ??
      e?.status ??
      e?.response?.status;

    // 1) Validation dictionary on root object (some generated error classes may expose "errors" directly)
    const validationFromRoot = this.extractValidationDictionary(e);
    if (validationFromRoot.length) {
      return { status, messages: validationFromRoot, raw: error };
    }

    // 2) Validation dictionary in additionalData (Kiota often stores unknown fields there)
    const validationFromAdditionalData = this.extractValidationDictionary(
      e?.additionalData
    );
    if (validationFromAdditionalData.length) {
      return { status, messages: validationFromAdditionalData, raw: error };
    }

    // 3) ProblemDetails/custom error object already parsed by Kiota (ROOT LEVEL)
    // Example:
    // {
    //   status: 401,
    //   detail: "Invalid credentials.",
    //   title: "Authentication failed",
    //   type: "https://httpstatuses.com/401",
    //   additionalData: { traceId: "..." }
    // }
    const rootMessage =
      this.asNonEmptyString(e?.detail) ||
      this.asNonEmptyString(e?.message) ||
      this.asNonEmptyString(e?.title);

    if (rootMessage) {
      return { status, messages: [rootMessage], raw: error };
    }

    // 4) Handle responseText (plain string or JSON string) for non-parsed errors
    if (typeof e?.responseText === 'string' && e.responseText.trim()) {
      const parsedBody = this.tryParseJson(e.responseText);

      // If not JSON, treat as plain string
      if (typeof parsedBody === 'string') {
        return { status, messages: [parsedBody], raw: error };
      }

      // ValidationProblemDetails in parsed JSON
      const validationFromParsedBody = this.extractValidationDictionary(parsedBody);
      if (validationFromParsedBody.length) {
        return { status, messages: validationFromParsedBody, raw: error };
      }

      // ProblemDetails / custom JSON object in parsed responseText
      if (parsedBody && typeof parsedBody === 'object') {
        const message =
          this.asNonEmptyString(parsedBody.detail) ||
          this.asNonEmptyString(parsedBody.message) ||
          this.asNonEmptyString(parsedBody.title);

        if (message) {
          return { status, messages: [message], raw: error };
        }
      }

      // Unexpected JSON object shape
      return {
        status,
        messages: ['Request failed with an unexpected error payload.'],
        raw: error,
      };
    }

    // 5) Sometimes Kiota stores parsed data in additionalData only
    const messageFromAdditionalData =
      this.asNonEmptyString(e?.additionalData?.detail) ||
      this.asNonEmptyString(e?.additionalData?.message) ||
      this.asNonEmptyString(e?.additionalData?.title);

    if (messageFromAdditionalData) {
      return { status, messages: [messageFromAdditionalData], raw: error };
    }

    // 6) Generic fallback for unknown shapes
    const fallback =
      this.asNonEmptyString(e?.response?.statusText) ||
      this.asNonEmptyString(e?.message) ||
      'An unknown error occurred.';

    return { status, messages: [fallback], raw: error };
  }

  private extractValidationDictionary(source: any): string[] {
    // Supports:
    // - payload.errors
    // - additionalData.errors
    // - root.errors (some Kiota-generated error models)
    const errors = source?.errors ?? source?.additionalData?.errors;

    if (!errors || typeof errors !== 'object') {
      return [];
    }

    const messages: string[] = [];

    for (const [field, value] of Object.entries(errors)) {
      // ASP.NET ValidationProblemDetails typically returns string[]
      if (Array.isArray(value)) {
        for (const item of value) {
          const msg = this.asNonEmptyString(item);
          if (msg) {
            messages.push(`${field}: ${msg}`);
          }
        }
        continue;
      }

      // Some APIs may return a single string per field
      const single = this.asNonEmptyString(value);
      if (single) {
        messages.push(`${field}: ${single}`);
      }
    }

    return messages;
  }

  private tryParseJson(text: string): any {
    try {
      return JSON.parse(text);
    } catch {
      // Return original text if parsing fails
      return text;
    }
  }

  private asNonEmptyString(value: unknown): string | null {
    return typeof value === 'string' && value.trim().length > 0 ? value : null;
  }
}
