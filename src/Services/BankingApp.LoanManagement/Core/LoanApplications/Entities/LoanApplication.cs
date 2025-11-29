using BankingApp.LoanManagement.Application.LoanApplicationModels;
using BankingApp.LoanManagement.Core.LoanApplications.ValueObjects;
using BankingApp.LoanManagement.Core.LoanApplicationsDomainEvents;
using BankingApp.LoanManagement.Infrastructure.Factory;
using BankingApp.LoanManagement.Infrastructure.Repositories;
using BankingAppDDD.Domains.Abstractions.Entities;
using BankingAppDDD.Domains.Abstractions.Models;
using BankingAppDDD.Domains.Abstractions.ValueObjects;

namespace BankingApp.LoanManagement.Core.LoanApplicationsEntities
{
    public sealed class LoanApplication : EntityBase, IAggregateRoot, ILoanNonGenericRepository
    {
        public LoanApplicationNumber Number { get; private set; }
        public LoanApplicationStatus Status { get; private set; }
        //public int LoanApplicationStatusId { get; private set; }
        public ScoringResult Score { get; private set; }
        public Customer Customer { get; private set; }
        public Asset Asset { get; private set; }
        public Loan Loan { get; private set; }
        public LoanUnderWriter Decision { get; private set; }
        public Registration Registration { get; private set; }
        public int LoanTypeId { get; private set; }

        // To satisfy EF Core
        private LoanApplication()
        {
        }

        public void Evaluate(ScoringRules rules)
        {
            Score = rules.Evaluate(this);
            if (Score.IsLow())
            {
                Status = LoanApplicationStatus.Rejected;
            }
        }

        public void Accept(Operator decisionBy, Guid CustomerId)
        {
            if (Status != LoanApplicationStatus.New)
            {
                throw new ApplicationException("Cannot accept application that is already accepted or rejected");
            }

            if (Score == null)
            {
                throw new ApplicationException("Cannot accept application before scoring");
            }

            if (!decisionBy.CanAccept(this.Loan.LoanAmount))
            {
                throw new ApplicationException("Operator does not have required competence level to accept application");
            }

            Status = LoanApplicationStatus.Accepted;
            Decision = LoanUnderWriter.Create(SysTime.Today(), decisionBy.Id);
            AddDomainEvent(LoanApplicationAccepted.Create(CustomerId, Status));
        }

        public void Reject(Operator decisionBy, Guid CustomerId)
        {
            if (Status != LoanApplicationStatus.New)
            {
                throw new ApplicationException("Cannot reject application that is already accepted or rejected");
            }

            Status = LoanApplicationStatus.Rejected;
            Decision = LoanUnderWriter.Create(SysTime.Today(), decisionBy.Id);
            AddDomainEvent(LoanApplicationRejected.Create(CustomerId, Status));
        }

        public static LoanApplication Create(LoanApplicationData loanApplicationData, Operator? operatorData)
        {
            var (customerData, assetData, loanData, loanTypeId) = loanApplicationData ?? throw new ArgumentNullException(nameof(loanApplicationData));
            var loanApplicationNumber = LoanApplicationNumber.NewNumber();
            if (string.IsNullOrWhiteSpace(loanApplicationNumber))
                throw new ArgumentException("Number cannot be null");
            if (customerData == null)
                throw new ArgumentException("Customer cannot be null");
            if (assetData == null)
                throw new ArgumentException("Property cannot be null");
            if (loanData == null)
                throw new ArgumentException("Loan cannot be null");


            var customer = Customer.Create(customerData.customerMonthlyIncome, customerData.customerBirthdate, customerData.customerNationalIdentifier);
            var addressData = new AddressData(assetData.propertyAddress.street, assetData.propertyAddress.city, assetData.propertyAddress.state, assetData.propertyAddress.zipCode, assetData.propertyAddress.country);
            var property = Asset.Create(assetData.PropertyValue, Address.Create(addressData));
            var loan = Loan.Create(loanData.loanAmount, loanData.loanNumberOfYears, new Percent(loanData.percent));
            var registeredBy = Operator.Create(operatorData!.CompetenceLevel, operatorData.Id);
            return new LoanApplication(loanApplicationNumber, LoanApplicationStatus.New, customer, property, loan, ScoringResult.Create(null, string.Empty), Registration.Create(SysTime.Today(), registeredBy.Id), LoanUnderWriter.Create(default, Guid.Empty), loanTypeId);

        }

        private LoanApplication(
            LoanApplicationNumber number,
            LoanApplicationStatus status,
            Customer customer,
            Asset property,
            Loan loan,
            ScoringResult score,
            Registration registration,
            LoanUnderWriter decision,
            int loanTypeId)
        {
            Number = number;
            Status = status;
            Score = score;
            Customer = customer;
            Asset = property;
            Loan = loan;
            Registration = registration;
            Decision = decision;
            LoanTypeId = loanTypeId;
        }
    }
}
