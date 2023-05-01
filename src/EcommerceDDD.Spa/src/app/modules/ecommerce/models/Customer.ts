export class Customer {
  id: string;
  name: string;
  shippingAddress: string;
  email: string;
  creditLimit: number;

  constructor(
    id: string,
    name: string,
    email: string,
    shippingAddress: string,
    creditLimit: number
  ) {
    this.id = id;
    this.name = name;
    this.email = email;
    this.shippingAddress = shippingAddress;
    this.creditLimit = creditLimit;
  }
}
