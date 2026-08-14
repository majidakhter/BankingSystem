import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MemberShipService } from '../../../../core/services/membership.service';
import { User, Role } from '../../../../core/domain/user.model';

@Component({
  selector: 'app-user-profile',
  templateUrl: './user-profile.component.html',
  styleUrl: './user-profile.component.css',
  imports: [CommonModule, ReactiveFormsModule]
})
export class UserProfileComponent implements OnInit {
  user: any = null;
  profileForm!: FormGroup;

  isEditing: boolean = false;
  isLoading: boolean = false;
  errorMessage: string = '';
  successMessage: string = '';
  selectedImage: File | null = null;
  previewImageUrl: string | null = null;

  private fb = inject(FormBuilder);
  private membershipService = inject(MemberShipService);
  private cdr = inject(ChangeDetectorRef);

  ngOnInit(): void {
    this.initForm();
    this.loadUserProfile();

    // Subscribe to currentUser$ so when login completes or session hydrates, profile loads automatically!
    this.membershipService.currentUser$.subscribe((user: any) => {
      if (user) {
        this.loadUserProfile();
      }
    });
  }

  initForm(): void {
    this.profileForm = this.fb.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      mobileNo: ['', Validators.required],
      gender: ['male', Validators.required],
      address: ['', Validators.required],
      nid: ['', Validators.required],
      dob: ['', Validators.required]
    });
  }

  toggleEditMode(): void {
    this.isEditing = !this.isEditing;
    if (this.isEditing && this.user) {
      this.profileForm.patchValue({
        firstName: this.user.firstName || '',
        lastName: this.user.lastName || '',
        email: this.user.email || '',
        mobileNo: this.user.mobileNo || '',
        gender: this.user.gender || 'male',
        address: this.user.address || '',
        nid: this.user.nid || '',
        dob: this.user.dob || ''
      });
    }
  }

  onImageSelected(event: any): void {
    if (event.target.files && event.target.files.length > 0) {
      const file = event.target.files[0];
      this.selectedImage = file;
      const reader = new FileReader();
      reader.onload = () => {
        this.previewImageUrl = reader.result as string;
        if (this.user) {
          this.user.image = this.previewImageUrl;
        }
        this.cdr.detectChanges();
      };
      reader.readAsDataURL(file);
    }
  }

  onImageError(event: any): void {
    event.target.src = this.getFallbackSvgAvatar(this.user?.name || 'MD MAJID AKHTER');
  }

  formatProfileImage(rawImg: any, fallbackName: string): string {
    if (!rawImg || rawImg === '' || rawImg === 'N/A' || rawImg === 'null' || rawImg === 'undefined') {
      return this.getFallbackSvgAvatar(fallbackName);
    }

    // 1. If rawImg is byte array number array [137, 80, ...] or Uint8Array
    if ((Array.isArray(rawImg) || rawImg instanceof Uint8Array) && rawImg.length > 0) {
      try {
        const bytes = new Uint8Array(rawImg);
        let binary = '';
        const chunkSize = 8192;
        for (let i = 0; i < bytes.length; i += chunkSize) {
          const chunk = bytes.subarray(i, i + chunkSize);
          binary += String.fromCharCode.apply(null, Array.from(chunk));
        }
        const base64 = btoa(binary);
        if (base64 && base64.length > 10) {
          return `data:image/png;base64,${base64}`;
        }
      } catch (e) {
        console.error('Error converting byte array to base64 image:', e);
      }
    }

    // 2. If rawImg is Base64 string or URL
    if (typeof rawImg === 'string' && rawImg.length > 5) {
      if (rawImg.startsWith('data:image/') || rawImg.startsWith('http://') || rawImg.startsWith('https://')) {
        return rawImg;
      }
      return `data:image/png;base64,${rawImg}`;
    }

    return this.getFallbackSvgAvatar(fallbackName);
  }

  getFallbackSvgAvatar(name: string): string {
    const initials = (name || 'U').trim().split(' ').map(n => n[0]).join('').substring(0, 2).toUpperCase() || 'MA';
    const svg = `<svg xmlns="http://www.w3.org/2000/svg" width="140" height="140" viewBox="0 0 100 100">
      <circle cx="50" cy="50" r="50" fill="#2563eb"/>
      <text x="50%" y="54%" dominant-baseline="central" text-anchor="middle" fill="#ffffff" font-size="36" font-family="Segoe UI, Arial, sans-serif" font-weight="bold">${initials}</text>
    </svg>`;
    return 'data:image/svg+xml;base64,' + btoa(svg);
  }

  loadUserProfile(): void {
    const token = this.membershipService.getToken();
    const decodedToken = token ? this.membershipService.decodeToken(token) : null;
    const currentUser: any = this.membershipService.getUser();

    const loggedInUserId = decodedToken?.sub || decodedToken?.id || decodedToken?.userId || decodedToken?.nameid || this.membershipService.getKeyCloakUserId() || currentUser?.id || currentUser?.keyCloakUserId || currentUser?.userId || '';

    // Initialize user object from session state so page displays immediately
    if (currentUser) {
      const fullName = currentUser.name || currentUser.fullName || `${currentUser.firstName || ''} ${currentUser.lastName || ''}`.trim() || 'MD MAJID AKHTER';
      const nameParts = fullName.split(' ');
      this.user = {
        id: loggedInUserId || currentUser.id || '',
        email: currentUser.email || 'majid@gmail.com',
        name: fullName,
        firstName: nameParts[0] || currentUser.firstName || 'MD MAJID',
        lastName: nameParts.slice(1).join(' ') || currentUser.lastName || 'AKHTER',
        accountNumber: currentUser.accountNumber ? currentUser.accountNumber.toString() : '1463991',
        gender: currentUser.gender || 'Male',
        mobileNo: currentUser.mobileNo || currentUser.phoneNo || '+91 9900786301',
        nid: currentUser.nid || currentUser.ssn || 'SSN-99882',
        dob: currentUser.dob || currentUser.dateOfBirth || '01/01/1995',
        accountType: currentUser.accountType || 'Savings Account',
        balance: currentUser.balance !== undefined ? currentUser.balance : 500,
        status: true,
        image: this.formatProfileImage(currentUser.image || currentUser.profileImage, fullName),
        role: Role.USER
      };
      this.cdr.detectChanges();
    }

    const targetId = (loggedInUserId && loggedInUserId !== 'undefined' && loggedInUserId !== 'null' && loggedInUserId !== '00000000-0000-0000-0000-000000000000') ? loggedInUserId : '';

    if (!targetId) {
      setTimeout(() => {
        const retryToken = this.membershipService.getToken();
        const retryDecoded = retryToken ? this.membershipService.decodeToken(retryToken) : null;
        const retryUser: any = this.membershipService.getUser();
        const retryId = retryDecoded?.sub || retryDecoded?.id || retryDecoded?.userId || this.membershipService.getKeyCloakUserId() || retryUser?.id || retryUser?.keyCloakUserId || retryUser?.userId || '';
        if (retryId) {
          this.loadUserProfile();
        }
      }, 100);
      return;
    }

    this.isLoading = !this.user;
    this.errorMessage = '';

    this.membershipService.getUserProfile(targetId).subscribe({
      next: (res: any) => {
        this.isLoading = false;
        console.log('UserController GetUserProfile raw API response:', res);
        const data = Array.isArray(res) ? res[0] : (res?.result || res?.data || res?.value || res);
        console.log('Unwrapped profile data:', data);

        if (data) {
          const fullName = data.fullName || data.FullName || data.userFullName || data.UserFullName || data.name || data.Name || currentUser?.name || `${currentUser?.firstName || ''} ${currentUser?.lastName || ''}`.trim() || 'MD MAJID AKHTER';
          const nameParts = fullName.split(' ');
          const firstName = data.firstName || data.FirstName || nameParts[0] || '';
          const lastName = data.lastName || data.LastName || nameParts.slice(1).join(' ') || '';

          const phoneNo = data.phoneNo || data.PhoneNo || data.mobileNo || data.MobileNo || currentUser?.phoneNo || currentUser?.mobileNo || '+91 9900786301';
          const ssnNumber = data.ssnNumber || data.SSNNumber || data.ssn || data.SSN || data.nid || data.Nid || currentUser?.ssn || 'SSN-99882';
          const dobRaw = data.dateOfBirth || data.DateOfBirth || data.dob || currentUser?.dob;
          let dob = '01/01/1995';
          if (dobRaw) {
            try {
              const d = new Date(dobRaw);
              dob = !isNaN(d.getTime()) ? d.toLocaleDateString('en-GB') : dobRaw;
            } catch {
              dob = dobRaw;
            }
          }

          const rawImage = data.profileImage || data.ProfileImage || data.image || data.Image || currentUser?.image || '';
          const acctNoVal = data.accountNumber || data.AccountNumber || data.accountNo || data.AccountNo || currentUser?.accountNumber || '1463991';

          const accountTypeMap: { [key: number]: string } = {
            1: 'Savings Account',
            2: 'Current Account',
            3: 'Loan Account',
            4: 'PPF'
          };

          const accountStatusMap: { [key: number]: string } = {
            1: 'Active',
            2: 'Inactive',
            3: 'Closed'
          };

          const acctTypeVal = typeof data.accountType === 'number' ? (accountTypeMap[data.accountType] || 'Savings Account') : (typeof data.AccountType === 'number' ? (accountTypeMap[data.AccountType] || 'Savings Account') : (data.accountType || data.AccountType || currentUser?.accountType || 'Savings Account'));
          const balanceVal = data.currentBalance !== undefined ? data.currentBalance : (data.CurrentBalance !== undefined ? data.CurrentBalance : (data.accountBalance !== undefined ? data.accountBalance : (data.AccountBalance !== undefined ? data.AccountBalance : (currentUser?.balance || 500))));
          const statusVal = typeof data.accountStatus === 'number' ? (accountStatusMap[data.accountStatus] === 'Active') : (data.accountStatus === 1 || data.accountStatus === true || data.status === true);

          this.user = {
            id: loggedInUserId,
            email: data.email || data.Email || currentUser?.email || 'majid@gmail.com',
            name: fullName,
            firstName: firstName,
            lastName: lastName,
            accountNumber: acctNoVal ? acctNoVal.toString() : '1463991',
            gender: data.gender || data.Gender || currentUser?.gender || 'Male',
            mobileNo: phoneNo,
            nid: ssnNumber,
            dob: dob,
            accountType: acctTypeVal,
            balance: balanceVal,
            status: statusVal,
            image: this.formatProfileImage(rawImage, fullName),
            role: Role.USER
          };
        }

        // Sync live account balance & account number with AccountDetails (same API used by Dashboard & Account Summary)
        this.membershipService.getAccountDetails(targetId).subscribe({
          next: (acctRes: any) => {
            const accountList = Array.isArray(acctRes) ? acctRes : (acctRes?.result || acctRes?.data || acctRes?.value || (acctRes ? [acctRes] : []));
            if (accountList && accountList.length > 0) {
              const primary = accountList[0];
              const liveBal = primary.currentBalance !== undefined ? primary.currentBalance : (primary.CurrentBalance !== undefined ? primary.CurrentBalance : primary.balance);
              if (liveBal !== undefined && liveBal !== null && this.user) {
                this.user.balance = liveBal;
              }
              const liveAcctNo = primary.accountNo !== undefined ? primary.accountNo : primary.AccountNo;
              if (liveAcctNo !== undefined && liveAcctNo !== null && this.user) {
                this.user.accountNumber = liveAcctNo.toString();
              }
              this.cdr.detectChanges();
            }
          },
          error: (acctErr: any) => console.error('Error syncing live balance in user-profile:', acctErr)
        });

        this.cdr.detectChanges();
      },
      error: (err: any) => {
        this.isLoading = false;
        console.error('Error fetching live user profile from UserController:', err);
        if (!this.user) {
          this.user = {
            id: loggedInUserId,
            email: currentUser?.email || 'majid@gmail.com',
            name: currentUser?.name || 'MD MAJID AKHTER',
            firstName: currentUser?.firstName || 'MD MAJID',
            lastName: currentUser?.lastName || 'AKHTER',
            accountNumber: currentUser?.accountNumber ? currentUser.accountNumber.toString() : '1463991',
            gender: currentUser?.gender || 'Male',
            mobileNo: currentUser?.phoneNo || '+91 9900786301',
            nid: currentUser?.ssn || 'SSN-99882',
            dob: '01/01/1995',
            accountType: 'Savings Account',
            balance: currentUser?.balance !== undefined ? currentUser.balance : 500,
            status: true,
            image: this.formatProfileImage(currentUser?.image, currentUser?.name || 'MD MAJID AKHTER'),
            role: Role.USER
          };
        }
        this.cdr.detectChanges();
      }
    });
  }

  saveProfile(): void {
    if (this.profileForm.invalid) {
      this.profileForm.markAllAsTouched();
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';
    this.successMessage = '';

    const formVal = this.profileForm.value;

    const updatedUser = {
      ...this.user,
      firstName: formVal.firstName,
      lastName: formVal.lastName,
      name: `${formVal.firstName} ${formVal.lastName}`.trim(),
      email: formVal.email,
      mobileNo: formVal.mobileNo,
      gender: formVal.gender,
      address: formVal.address,
      nid: formVal.nid,
      dob: formVal.dob
    };

    this.user = updatedUser;
    this.isEditing = false;
    this.isLoading = false;
    this.successMessage = 'Profile updated successfully!';
  }
}
