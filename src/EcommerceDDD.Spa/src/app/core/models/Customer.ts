
export class Customer {
  id: string;
  name: string;
  address: string;
  email: string;

  constructor(id: string, name: string, email: string, address: string) {
    this.id = id;
    this.name = name;
    this.email = email;
    this.address = address;
  }
}
