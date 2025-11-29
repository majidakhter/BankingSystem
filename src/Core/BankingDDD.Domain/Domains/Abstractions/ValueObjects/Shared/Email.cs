using FluentValidation;

namespace BankingAppDDD.Domains.Abstractions.ValueObjects
{
    public sealed class Email : ValueObject
    {
        public string EmailId { get; private set; }
        private Email()
        {

        }
        private Email(string emailId)
        {
            EmailId = emailId;
        }
        public static Email Create(string emailId)
        {
            // validations should be placed here instead of constructor
            new EmailValidator()!.ValidateAndThrow(emailId);

            return new Email(emailId);
        }

        public static implicit operator string(Email? emailId) => emailId?.EmailId ?? string.Empty;
        public void Deconstruct(out string emailId) => emailId = EmailId;
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return EmailId;
        }
    }
    public sealed class EmailValidator : AbstractValidator<string>
    {
        public EmailValidator()
        {
            RuleFor(email => email).EmailAddress();
            RuleFor(email => email).NotNull();
            RuleFor(email => email).NotEmpty();
        }
    }
}
