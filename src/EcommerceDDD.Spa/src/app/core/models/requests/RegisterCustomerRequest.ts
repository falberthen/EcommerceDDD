export class RegisterCustomerRequest {
  email: string;
  name: string;
  address: string;
  password: string;
  passwordConfirm: string;
  availableCreditLimit: number;

  constructor(email: string, name: string, address: string, password: string, passwordConfirm: string,
    availableCreditLimit: number) {
    this.email = email;
    this.name = name;
    this.address = address;
    this.password = password;
    this.passwordConfirm = passwordConfirm;
    this.availableCreditLimit = availableCreditLimit;
  }
}
