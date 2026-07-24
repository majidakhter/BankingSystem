import { Injectable } from '@angular/core';

@Injectable()
export class Configuration {
    //public ApiServer: string = "http://localhost:6001/";  // when you run with IIS
    // if you run the project with IIS express, please use the address http://localhost:51214/

    //public ApiUrl: string = "api/notes";
    public _accountRegisterAPI: string = 'api/customer/register/';
    public _accountLoginAPI: string = 'api/customer/authenticate/';
    public _accountLogoutAPI: string = 'api/customer/logout/';
    public _accountAddressAPI: string = 'api/customer/saveaddress/';
    //public ServerWithApiUrl: string = this.ApiServer + this.ApiUrl;
}