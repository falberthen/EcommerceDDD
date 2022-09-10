
export class Customer {
  id: string;
  name: string;
  address: string;
  email: string;
  availableCreditLimit: number;

  constructor(id: string, name: string, email: string, address: string,
    availableCreditLimit: number) {
    this.id = id;
    this.name = name;
    this.email = email;
    this.address = address;
    this.availableCreditLimit = availableCreditLimit;
  }
}
