export class Order {
  orderId: string;
  quoteId: string;
  orderLines: OrderLine[] = [];
  createdAt: Date;
  totalPrice: number;
  currencySymbol: string;
  statusText: string;
  statusCode: number;

  constructor(
    orderId: string,
    quoteId: string,
    createdAt: Date,
    totalPrice: number,
    currencySymbol: string,
    statusText: string,
    statusCode: number
  ) {
    this.orderId = orderId;
    this.quoteId = quoteId;
    this.createdAt = createdAt;
    this.totalPrice = totalPrice;
    this.currencySymbol = currencySymbol;
    this.statusText = statusText;
    this.statusCode = statusCode;
  }
}

export class OrderLine {
  productId: string;
  productName: string;
  unitPrice: number;
  quantity: number;

  constructor(
    productId: string,
    productName: string,
    unitPrice: number,
    quantity: number
  ) {
    this.productId = productId;
    this.productName = productName;
    this.quantity = quantity;
    this.unitPrice = unitPrice;
  }
}
