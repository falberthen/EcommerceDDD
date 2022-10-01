export class OpenQuoteRequest {
  customerId: string;
  currencyCode: string;

  constructor(customerId: string, currencyCode: string) {
    this.customerId = customerId;
    this.currencyCode = currencyCode;
  }
}
