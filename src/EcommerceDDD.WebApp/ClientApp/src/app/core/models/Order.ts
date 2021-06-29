
export class Order {
  orderId: string;
  orderLines: OrderLine[] = [];
  createdAt: Date;
  totalPrice: number;
  status: string;
}

export class OrderLine{
  productId: string;
  productName: string;
  productPrice: number;
  productQuantity: number;
  currencySymbol: string;
}
