export class AddressData{
    Street!: string;
    City!: string;
    State!: string;
    Country!: string;
    ZipCode!: string;
    constructor(street: string, city: string, state: string, country: string, zipcode: string)
    {
      this.Street =street;
      this.City = city;
      this.State = state;
      this.Country = country;
      this.ZipCode = zipcode;
    }
}