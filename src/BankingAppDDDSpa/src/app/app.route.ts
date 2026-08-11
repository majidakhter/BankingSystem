import { Routes } from '@angular/router';
import { AuthGuard } from './core/guards/authguard.guard';

export const routes: Routes = [
  {
    path: 'dashboard',
    loadComponent: () => import('@modules/bankingapp/components/dashboard/dashboard.component').then(m => m.DashboardComponent),
    canActivate: [AuthGuard]
  },
  {
    path: 'loans',
    loadComponent: () => import('@modules/bankingapp/components/loans/loans.component').then(m => m.LoansComponent),
    canActivate: [AuthGuard]
  },
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
    path: 'cards',
    loadComponent: () => import('@modules/bankingapp/components/cards/cards.component').then(m => m.CardsComponent),
    canActivate: [AuthGuard]
  },
  {
    path: 'cards-management',
    loadComponent: () => import('@modules/bankingapp/components/cards/cards.component').then(m => m.CardsComponent),
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
    path: 'add-payee',
    loadComponent: () => import('@modules/bankingapp/components/accounts/add-payee/add-payee.component').then(m => m.AddPayeeComponent),
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
    path: 'bills-recharges',
    loadComponent: () => import('@modules/bankingapp/components/bills-recharges/bills-recharges.component').then(m => m.BillsRechargesComponent),
    canActivate: [AuthGuard]
  },
  {
    path: 'bills',
    loadComponent: () => import('@modules/bankingapp/components/bills-recharges/bills-recharges.component').then(m => m.BillsRechargesComponent),
    canActivate: [AuthGuard]
  },
  { 
    path: 'accounts-summary', 
    loadComponent: () => import('@modules/bankingapp/components/accounts-summary/accounts-summary.component').then(m => m.AccountsSummaryComponent),
    canActivate: [AuthGuard] 
  },
  { 
    path: 'accounts', 
    loadComponent: () => import('@modules/bankingapp/components/accounts-summary/accounts-summary.component').then(m => m.AccountsSummaryComponent),
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
