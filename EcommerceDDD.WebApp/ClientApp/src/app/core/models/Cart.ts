
export class Cart {
  cartId: string;
  cartItems: CartItem[] = [];
  totalPrice: number;
}

export class CartItem{
  productId: string;
  productName: string;
  productPrice: number;
  productQuantity: number;
  currencySymbol: string;
}
