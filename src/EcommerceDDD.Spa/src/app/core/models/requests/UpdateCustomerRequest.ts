export class UpdateCustomerRequest {
  name: string;
  address: string;
  availableCreditLimit: number;

  constructor(name: string, address: string, availableCreditLimit: number) {
    this.name = name;
    this.address = address;
    this.availableCreditLimit = availableCreditLimit;
  }
}
