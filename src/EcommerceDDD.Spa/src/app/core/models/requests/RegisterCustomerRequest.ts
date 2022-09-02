export class RegisterCustomerRequest {
  email: string;
  name: string;
  address: string;
  password: string;
  passwordConfirm: string;

  constructor(email: string, name: string, address: string, password: string, passwordConfirm: string) {
    this.email = email;
    this.name = name;
    this.address = address;
    this.password = password;
    this.passwordConfirm = passwordConfirm;
  }
}
