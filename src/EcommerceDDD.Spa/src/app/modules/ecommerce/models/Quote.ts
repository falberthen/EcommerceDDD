export class Quote {
  quoteId: string;
  items: QuoteItem[] = [];
  currencySymbol: string;
  totalPrice: number;

  constructor(quoteId: string, currencySymbol: string, totalPrice: number) {
    this.quoteId = quoteId;
    this.currencySymbol = currencySymbol;
    this.totalPrice = totalPrice;
  }
}

export class QuoteItem{
  productId: string;
  productName: string;
  unitPrice: number;
  quantity: number;
  currencySymbol: string;
  totalPrice: number;

  constructor(productId: string, productName: string, unitPrice: number,
    quantity: number, currencySymbol: string, totalPrice: number) {
    this.productId = productId;
    this.productName = productName;
    this.quantity = quantity;
    this.unitPrice = unitPrice;
    this.currencySymbol = currencySymbol;
    this.totalPrice = totalPrice;
  }
}
