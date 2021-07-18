
export class Order {
  orderId: string;
  orderLines: OrderLine[] = [];
  createdAt: Date;
  totalPrice: number;
  status: string;

  constructor(orderId: string, createdAt: Date, totalPrice: number, status: string) {
    this.orderId = orderId;
    this.createdAt = createdAt;
    this.totalPrice = totalPrice;
    this.status = status;
  }
}

export class OrderLine{
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
