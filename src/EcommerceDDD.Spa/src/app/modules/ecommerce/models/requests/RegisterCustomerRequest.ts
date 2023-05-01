export class RegisterCustomerRequest {
  email: string;
  name: string;
  shippingAddress: string;
  password: string;
  passwordConfirm: string;
  creditLimit: number;

  constructor(
    email: string,
    name: string,
    shippingAddress: string,
    password: string,
    passwordConfirm: string,
    creditLimit: number
  ) {
    this.email = email;
    this.name = name;
    this.shippingAddress = shippingAddress;
    this.password = password;
    this.passwordConfirm = passwordConfirm;
    this.creditLimit = creditLimit;
  }
}
