import { Product } from '../Product';

export class ChangeQuoteRequest {
  quoteId: string;
  product: Product;
  currency: string;

  constructor(quoteId: string, product: Product, currency: string) {
    this.quoteId = quoteId;
    this.product = product;
    this.currency = currency;
  }
}
