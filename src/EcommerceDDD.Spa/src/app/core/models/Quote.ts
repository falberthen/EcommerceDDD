
export class Quote {
  quoteId: string;
  quoteItems: QuoteItem[] = [];
  totalPrice: number;

  constructor(quoteId: string, totalPrice: number) {
    this.quoteId = quoteId;
    this.totalPrice = totalPrice;
  }
}

export class QuoteItem{
  productId: string;
  productName: string;
  productPrice: number;
  productQuantity: number;
  currencySymbol: string;

  constructor(productId: string, productName: string, productPrice: number, productQuantity: number, currencySymbol: string) {
    this.productId = productId;
    this.productName = productName;
    this.productQuantity = productQuantity;
    this.productPrice = productPrice;
    this.currencySymbol = currencySymbol;
  }
}
