
export class Product {
  id: string;
  name: string;
  price: number;
  currency: string;
  currencySymbol: string;
  quantity: number;

  constructor(id: string, name: string, price: number, currency: string, currencySymbol: string, quantity: number) {
    this.id = id;
    this.name = name;
    this.price = price;
    this.currency = currency;
    this.currencySymbol = currencySymbol;
    this.quantity = quantity;
  }
}

