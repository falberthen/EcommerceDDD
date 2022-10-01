export class GetProductsRequest {
  currencyCode: string;
  productIds: string[] = [];

  constructor(currencyCode: string, productIds: string[] = []) {
    this.currencyCode = currencyCode;
    this.productIds = productIds;
  }
}
