import { Role } from "./role.model";
import { Token } from "./token.model";
export interface User {
  id: string;
  email: string;
  name: string;
  accountNumber : string;
  firstName : string;
  lastName : string;
  password : string;
  gender : string;
  address : string;
  mobileNo : string;
  nid : string;
  dob : string;
  image : string;
  accountType : string;
  createDate : string;
  status : boolean;
  balance : number;
  role : Role
}

export class UserModel implements User{
  id!: string;
  email!: string;
  name!: string;
  accountNumber!: string;
  firstName!: string;
  lastName!: string;
  password!: string;
  gender!: string;
  address!: string;
  mobileNo!: string;
  nid!: string;
  dob!: string;
  image!: string;
  accountType!: string;
  createDate!: string;
  status!: boolean;
  balance!: number;
  role!: Role;
  token!: Token;
}