using BankingAppDDD.Domains.Abstractions.ValueObjects;
using Newtonsoft.Json;

namespace BankingApp.LoanManagement.Core.LoanApplications.ValueObjects
{
    public sealed class Registration : ValueObject
    {
        public DateOnly RegistrationDate { get; private set; }

        public Guid RegisteredBy { get; private set; }

        public static Registration Create(DateOnly registrationDate, Guid registeredBy)
        {
            return new Registration(registrationDate, registeredBy);
        }

        [JsonConstructor]
        private Registration(DateOnly registrationDate, Guid registeredBy)
        {
            RegistrationDate = registrationDate;
            RegisteredBy = registeredBy;
        }

        private Registration()
        {
        }
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return RegistrationDate;
            yield return RegisteredBy;
        }
    }
}
