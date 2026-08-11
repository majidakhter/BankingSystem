import { Component, forwardRef, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MemberShipService } from '../../../../core/services/membership.service';

@Component({
  selector: 'app-address',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './address.component.html',
  styleUrl: './address.component.scss',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => AddressComponent),
      multi: true
    }
  ]
})
export class AddressComponent implements ControlValueAccessor, OnInit {
  addressForm: FormGroup;
  countries: any[] = [];
  allStates: any[] = [];
  filteredStates: any[] = [];

  private membershipService = inject(MemberShipService);
  private cdr = inject(ChangeDetectorRef);
  private onChange: any = () => { };
  private onTouched: any = () => { };

  constructor(private fb: FormBuilder) {
    this.addressForm = this.fb.group({
      street: ['123 Main St', Validators.required],
      city: ['Kolkata', Validators.required],
      country: ['India', Validators.required],
      state: ['West Bengal', Validators.required],
      zipcode: ['700001', Validators.required]
    });

    this.addressForm.valueChanges.subscribe(value => {
      this.onChange(value);
      this.onTouched();
    });
  }

  ngOnInit(): void {
    this.membershipService.getCountries().subscribe({
      next: (res: any) => {
        this.countries = Array.isArray(res) ? res : res?.data || res?.result || [];
        this.filterStatesByCountry(this.addressForm.get('country')?.value);
        this.cdr.markForCheck();
        this.cdr.detectChanges();
      },
      error: (err: any) => {
        console.error('Error fetching countries:', err);
        this.cdr.markForCheck();
      }
    });

    this.membershipService.getStates().subscribe({
      next: (res: any) => {
        this.allStates = Array.isArray(res) ? res : res?.data || res?.result || [];
        this.filterStatesByCountry(this.addressForm.get('country')?.value);
        this.cdr.markForCheck();
        this.cdr.detectChanges();
      },
      error: (err: any) => {
        console.error('Error fetching states:', err);
        this.cdr.markForCheck();
      }
    });

    this.addressForm.get('country')?.valueChanges.subscribe((selectedCountry: string) => {
      this.filterStatesByCountry(selectedCountry);
      const selectedState = this.addressForm.get('state')?.value;
      if (selectedState && !this.filteredStates.some(st => st.name === selectedState || st.code === selectedState)) {
        this.addressForm.get('state')?.setValue('', { emitEvent: false });
      }
      this.cdr.markForCheck();
      this.cdr.detectChanges();
    });
  }

  filterStatesByCountry(countryVal: string): void {
    if (!countryVal) {
      this.filteredStates = this.allStates;
      return;
    }

    const matchingCountry = this.countries.find(c =>
      c.name?.toLowerCase() === countryVal.toLowerCase() ||
      c.code?.toLowerCase() === countryVal.toLowerCase() ||
      c.id?.toString() === countryVal.toString()
    );

    if (matchingCountry) {
      this.filteredStates = this.allStates.filter(st => st.countryId === matchingCountry.id);
    } else {
      this.filteredStates = this.allStates;
    }
  }

  writeValue(value: any): void {
    if (value) {
      this.addressForm.patchValue(value, { emitEvent: false });
      if (value.country) {
        this.filterStatesByCountry(value.country);
      }
      this.cdr.markForCheck();
    }
  }

  registerOnChange(fn: any): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }

  setDisabledState?(isDisabled: boolean): void {
    isDisabled ? this.addressForm.disable() : this.addressForm.enable();
  }
}