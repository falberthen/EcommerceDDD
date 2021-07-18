
export class Customer {
  id: string;
  name: string;
  email: string;
  token?: string;

  constructor(id: string, name: string, email: string, token: string) {
    this.id = id;
    this.name = name;
    this.email = email;
    this.token = token;
  }
}
