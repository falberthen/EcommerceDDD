
export class Cart {
  cartId: string;
  cartItems: CartItem[] = [];
  totalPrice: number;

  constructor(cartId: string, totalPrice: number) {
    this.cartId = cartId;
    this.totalPrice = totalPrice;
  }
}

export class CartItem{
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
