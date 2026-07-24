import { Component, OnInit } from '@angular/core';
import { UserModel } from '../../../../core/domain/user.model';
import { Role } from '@core/domain/role.model';
import {CommonModule} from "@angular/common";

@Component({
  selector: 'app-user-profile',
  templateUrl: './user-profile.component.html',
  styleUrl: './user-profile.component.css',
  imports: [CommonModule],
})
export class UserProfileComponent implements OnInit{
  user: UserModel | null = null;
  errorMessage: string = '';
  ngOnInit(): void {
    this.loadUserProfile();
    
  }
  loadUserProfile(): void {
    this.user = this.userDetails;
  }
  public userDetails: UserModel = {
      id: '10001',
      email: 'test@gmail.com',
      name: 'John Doe',
      accountNumber : '123456789',
      firstName : 'John',
      lastName : 'Doe',
      password : 'password123',
      gender : 'male',
      address : 'metcalfe street kolkata',
      mobileNo : '9900877656',
      nid : '9876A8768',
      dob : '18/09/1987',
      image : 'string',
      accountType : 'savings',
      createDate : '23/07/2026',
      status : true,
      balance : 10000,
      role : Role.USER,
      token : {id:1212, token:'ab12kh4566tr453jhg775jjj'}
  };
  //public tokens : Token ={id:1212, token:'string'};
}

