
export class Product {
  id: string;
  name: string;
  unitPrice: number;
  currency: string;
  currencySymbol: string;
  quantity: number;

  constructor(id: string, name: string, unitPrice: number, currency: string, currencySymbol: string, quantity: number) {
    this.id = id;
    this.name = name;
    this.unitPrice = unitPrice;
    this.currency = currency;
    this.currencySymbol = currencySymbol;
    this.quantity = quantity;
  }
}
