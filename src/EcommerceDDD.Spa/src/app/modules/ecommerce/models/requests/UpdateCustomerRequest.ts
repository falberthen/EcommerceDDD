export class UpdateCustomerRequest {
  name: string;
  shippingAddress: string;
  creditLimit: number;

  constructor(name: string, address: string, creditLimit: number) {
    this.name = name;
    this.shippingAddress = address;
    this.creditLimit = creditLimit;
  }
}
