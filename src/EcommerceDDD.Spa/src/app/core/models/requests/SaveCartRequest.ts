import { Product } from '../Product';

export class SaveCartRequest {
  customerId: string;
  product: Product;
  currency: string;

  constructor(customerId: string, product: Product, currency: string) {
    this.customerId = customerId;
    this.product = product;
    this.currency = currency;
  }
}
