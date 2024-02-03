export class Product {
  productId: string;
  name: string;
  description: string;
  category: string;
  imageUrl: string;
  unitPrice: number;
  currency: string;
  currencySymbol: string;
  quantity: number;
  quantityInStock: number;

  constructor(
    productId: string,
    name: string,
    description: string,
    category: string,
    imageUrl: string,
    unitPrice: number,
    currency: string,
    currencySymbol: string,
    quantity: number,
    quantityInStock: number
  ) {
    this.productId = productId;
    this.name = name;
    this.description = description;
    this.category = category;
    this.imageUrl = imageUrl;
    this.unitPrice = unitPrice;
    this.currency = currency;
    this.currencySymbol = currencySymbol;
    this.quantity = quantity;
    this.quantityInStock = quantityInStock;
  }
}
