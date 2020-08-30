export class CustomerRegistration {
  email: string;
  name: string;
  password: string;
  passwordConfirm: string;

  constructor(email: string, name: string, password: string, passwordConfirm: string) {
    this.email = email;
    this.name = name;
    this.password = password;
    this.passwordConfirm = passwordConfirm;
  }
}
