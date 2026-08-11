import { AddressData } from "./addressdata";

export class CustomerData {
  Email!: string;
  Name!: string;
  Phoneno!: string;
  DateOfBirth!: Date;
  CustomerType!: number;
  Gender!: string;
  SSNumber!: string
  constructor(email: string, name: string, phoneno: string, dateofbirth: Date, customertype: number, gender: string, ssnumber: string){
    this.Email = email;
    this.Name = name;
    this.Phoneno = phoneno;
    this.DateOfBirth = dateofbirth;
    this.CustomerType = customertype;
    this.Gender = gender;
    this.SSNumber = ssnumber;
  }
}

export interface AddCustomerModel{
  customerData: CustomerData;
  addressData: AddressData;
}