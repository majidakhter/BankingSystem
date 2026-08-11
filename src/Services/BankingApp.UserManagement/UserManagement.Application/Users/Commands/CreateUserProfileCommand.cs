using BankingAppDDD.Applications.Abstractions.Commands;
using BankingAppDDD.Applications.Abstractions.Repositories;
using BankingAppDDD.Applications.Abstractions.Shared.Commands;
using BankingAppDDD.Applications.Abstractions.Shared.Events;
using BankingAppDDD.Common.Model;
using BankingAppDDD.Common.Polly;
using BankingAppDDD.Domains.Users.Models;
using BankingAppDDD.MongoService.Application.Mongo;
using BankingAppDDD.UserManagement.Core.Users.Entities;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace UserManagement.Application.Users.Commands
{
    /// <summary>
    /// CreateUserProfileCommandHandler - Handles user profile creation command and consumes CreateUserProfileCommand via MassTransit
    /// </summary>
    public sealed class CreateUserProfileCommandHandler : CommandHandler<CreateUserProfileCommand>, IConsumer<CreateUserProfileCommand>
    {
        private readonly IRepository<User> _repository;
        private readonly ILogger<CreateUserProfileCommandHandler> _logger;
        private readonly IBus _eventBus;
        private readonly IUserMongoService? _mongoService;
        IOptionsMonitor<PolyConfigSettings> _policyConfigSettings;
        private const string TypeName = "CreateUserProfileCommandHandler";
        public CreateUserProfileCommandHandler(
            IRepository<User> repository,
            ILogger<CreateUserProfileCommandHandler> logger,
            IBus eventBus,
            IOptionsMonitor<PolyConfigSettings> policyConfigSettings,
            IUnitOfWork unitOfWork,
            IUserMongoService? mongoService = null) : base(unitOfWork)
        {
            _repository = repository;
            _logger = logger;
            _eventBus = eventBus;
            _policyConfigSettings = policyConfigSettings;
            _mongoService = mongoService;
        }

        public async Task Consume(ConsumeContext<CreateUserProfileCommand> context)
        {
            _logger.LogInformation("Received CreateUserProfileCommand via MassTransit for KeycloakUserId: {KeycloakUserId}", context.Message.KeyCloakUserId);
            await HandleAsync(context.Message);
        }

        protected override async Task HandleAsync(CreateUserProfileCommand request)
        {
            var ud = request.userdata;
            string email = !string.IsNullOrWhiteSpace(ud?.Email) ? ud.Email : "user@example.com";
            string firstName = !string.IsNullOrWhiteSpace(ud?.FirstName) ? ud.FirstName : "John";
            string lastName = !string.IsNullOrWhiteSpace(ud?.LastName) ? ud.LastName : "Doe";
            string phoneNo = !string.IsNullOrWhiteSpace(ud?.PhoneNo) ? ud.PhoneNo : "1234567890";
            string userName = !string.IsNullOrWhiteSpace(ud?.UserName) ? ud.UserName : email;
            string password = !string.IsNullOrWhiteSpace(ud?.Password) ? ud.Password : "Password123!";
            string gender = !string.IsNullOrWhiteSpace(ud?.Gender) ? ud.Gender : "male";
            string ssNumber = !string.IsNullOrWhiteSpace(ud?.SSNumber) ? ud.SSNumber : "999-99-9999";
            DateOnly dob = ud != null ? ud.DateOfBirth : DateOnly.FromDateTime(DateTime.UtcNow);
            int userType = ud != null ? ud.UserType : 1;

            UserData userData = new UserData(userName, password, email, firstName, lastName, phoneNo, dob, userType, gender, ssNumber, ud?.ProfileImage)
            {
                ProfileImageBytes = ud?.ProfileImageBytes
            };
            var usercreated = User.Create(userData, request.AddressData, request.KeyCloakUserId, request.BranchId);

            
            _repository.Insert(usercreated);
            var pollyWrapper = new PollyWrap<User>(_policyConfigSettings, _logger);
            var wrapped = pollyWrapper.GetPolicyConfig(usercreated, TypeName);

            try
            {
                await wrapped.ExecuteAsync(async () =>
                {
                    await UnitOfWork!.CommitAsync();
                    if (_mongoService != null)
                    {
                        await _mongoService.SaveUserAsync(usercreated);
                    }
                }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SaveUser Save FAILURE - {Message}", ex.Message);
                throw;
            }

            // Publish UserProfileCreatedEvent to trigger UserProfileCreatedEventConsumer
            var userProfileCreatedEvent = new UserProfileCreatedEvent(request.userdata, request.AddressData, usercreated.Id, request.KeyCloakUserId, request.accountTypeId, request.amount);
            await _eventBus.Publish(userProfileCreatedEvent);
        }
    }
}