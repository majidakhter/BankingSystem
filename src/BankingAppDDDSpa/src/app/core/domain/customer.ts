export class Customer {
  Username: string;
  Password: string;
  RememberMe: boolean;
  constructor(username: string, password: string, rememberme: boolean){
    this.Username = username;
    this.Password = password;
    this.RememberMe = rememberme;
  }
}