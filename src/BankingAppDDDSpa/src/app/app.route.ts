
import { Routes } from '@angular/router';
import { AuthGuard } from './core/guards/authguard.guard';
export const routes: Routes = [
  {
    path: 'loan-list', 
    loadComponent: () => import('@modules/bankingapp/components/loans/loan-list/loan-list.component').then(m => m.LoanListComponent),
    canActivate: [AuthGuard]
  },
  {
    path: 'loan-apply',
    loadComponent: () => import('@modules/bankingapp/components/loans/loan-apply/loan-apply.component').then(m => m.LoanApplyComponent),
    canActivate: [AuthGuard]
  },
  {
    path: 'loan-payment/:id', 
    loadComponent: () => import('@modules/bankingapp/components/loans/loan-payment/loan-payment.component').then(m => m.LoanPaymentComponent),
    canActivate: [AuthGuard]
  },
  {
    path: 'transaction-list',
    loadComponent: () => import('@modules/bankingapp/components/accounts/transaction-list/transaction-list.component').then(m => m.TransactionListComponent),
    canActivate: [AuthGuard]
  },
  { path: 'deposit', 
    loadComponent: () => import('@modules/bankingapp/components/accounts/deposit/deposit.component').then(m => m.DepositComponent),
    canActivate: [AuthGuard]
  },
  {
    path: 'transfer', 
    loadComponent: () => import('@modules/bankingapp/components/accounts/transfer/transfer.component').then(m => m.TransferComponent),
    canActivate: [AuthGuard]
  },
  {
    path: 'withdraw', 
    loadComponent: () => import('@modules/bankingapp/components/accounts/withdraw/withdraw.component').then(m => m.WithdrawComponent),
    canActivate: [AuthGuard]
  },
  {
    path: 'bank-statement',
    loadComponent: () => import('@modules/bankingapp/components/accounts/bank-statement/bank-statement.component').then(m => m.BankStatementComponent),
    canActivate: [AuthGuard]
  },
  { 
    path: 'user-profile', 
    loadComponent: () => import('@modules/bankingapp/components/user-profile/user-profile.component').then(m => m.UserProfileComponent),
    canActivate: [AuthGuard] 
  },
  {
    path: 'register',
    loadComponent: () => import('@modules/authentication/reg-login/register/register.component').then(m => m.RegisterComponent)
  },
  {
    path: 'registerAdmin', 
    loadComponent: () => import('@modules/authentication/reg-login/admin-register/admin-register.component').then(m => m.AdminRegisterComponent)
  },
  {
    path: 'login', 
    loadComponent: () => import('@modules/authentication/reg-login/login/login.component').then(m => m.LoginComponent)
  },
  {
    path: '', 
    redirectTo: 'login', 
    pathMatch: 'full'
  },
  {
    path: 'home',
    loadComponent: () => import('@modules/bankingapp/components/home/home.component').then(m => m.HomeComponent)
  }
];


