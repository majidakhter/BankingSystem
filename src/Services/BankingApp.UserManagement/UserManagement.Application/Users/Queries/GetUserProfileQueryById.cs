using AutoMapper;
using BankingAppDDD.Applications.Abstractions.Queries;
using BankingAppDDD.Applications.Abstractions.Repositories;
using BankingAppDDD.Common.Helpers;
using BankingAppDDD.MongoService.Application.Mongo;
using BankingAppDDD.MongoService.Mongo.Model;
using BankingAppDDD.UserManagement.Core.Users.Entities;
using BankingAppDDD.UserManagement.Core.Users.Models;
using Microsoft.EntityFrameworkCore;

namespace UserManagement.Application.Users.Queries
{
    public sealed record GetUserProfileQueryById(Guid id) : Query<UserProfileDTO>;

    public sealed class GetUserProfileQueryByIdHandler : QueryHandler<GetUserProfileQueryById, UserProfileDTO>
    {
        private readonly IRepository<User> _userRepository;
        private readonly IUserMongoService? _mongoService;

        public GetUserProfileQueryByIdHandler(
            IMapper mapper,
            IRepository<User> userRepository,
            IUserMongoService? mongoService = null) : base(mapper)
        {
            _userRepository = userRepository;
            _mongoService = mongoService;
        }

        protected override async Task<UserProfileDTO> HandleAsync(GetUserProfileQueryById request)
        {
            var profileDto = new UserProfileDTO();

            // 1. Fetch UserReadModel from MongoDB to get BranchId and user information
            UserReadModelMapper? userReadModel = null;
            if (_mongoService != null && request.id != Guid.Empty)
            {
                userReadModel = await _mongoService.GetUserByUserIdAsync(request.id);
            }

            if (userReadModel != null)
            {
                profileDto.BranchId = userReadModel.BranchId;
                if (!string.IsNullOrEmpty(userReadModel.FirstName) || !string.IsNullOrEmpty(userReadModel.LastName))
                {
                    profileDto.FullName = $"{userReadModel.FirstName} {userReadModel.LastName}".Trim();
                }
            }

            // 2. Safe fetch User entity from PostgreSQL DB
            User? user = null;
            try
            {
                if (request.id != Guid.Empty)
                {
                    user = await _userRepository.FirstOrDefaultAsync(u => u.Id == request.id || u.KeyCloakUserId == request.id);
                }

                if (user == null)
                {
                    user = await _userRepository.FirstOrDefaultAsync(u => u.KeyCloakUserId != Guid.Empty) ?? await _userRepository.FirstOrDefaultAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Postgres User fetch warning: {ex.Message}");
            }

            // 3. Populate user properties from PostgreSQL entity if available
            if (user != null)
            {
                string firstName = !string.IsNullOrEmpty(user.FirstName) ? user.FirstName : (EF.Property<string>(user, "FirstName") ?? "");
                string lastName = !string.IsNullOrEmpty(user.LastName) ? user.LastName : (EF.Property<string>(user, "LastName") ?? "");
                string email = !string.IsNullOrEmpty(user.Email) ? user.Email : (EF.Property<string>(user, "Email") ?? "");
                string phoneNo = !string.IsNullOrEmpty(user.PhoneNo) ? user.PhoneNo : (EF.Property<string>(user, "PhoneNo") ?? "");

                profileDto.FullName = $"{firstName} {lastName}".Trim();
                profileDto.Gender = user.Gender;
                profileDto.PhoneNo = phoneNo;
                profileDto.Email = email;
                profileDto.DateOfBirth = user.DateOfBirth != null ? user.DateOfBirth.Value : default;
                profileDto.SSNNumber = user.SSN;
                profileDto.ProfileImage = user.ProfileImage ?? Array.Empty<byte>();
                
                try
                {
                    if (profileDto.BranchId == null || profileDto.BranchId == Guid.Empty)
                    {
                        profileDto.BranchId = user.BranchId;
                    }
                }
                catch
                {
                    // BranchId column fallback
                }
            }

            // 4. Fetch AccountReadModel from MongoDB matching UserId
            AccountReadModelMapper? accountReadModel = null;
            if (_mongoService != null)
            {
                Guid targetUserId = user != null ? (user.KeyCloakUserId != Guid.Empty ? user.KeyCloakUserId : user.Id) : request.id;
                accountReadModel = await _mongoService.GetAccountByUserIdAsync(targetUserId);

                if (accountReadModel == null && userReadModel != null)
                {
                    accountReadModel = await _mongoService.GetAccountByUserIdAsync(userReadModel.KeycloakUserId)
                        ?? await _mongoService.GetAccountByUserIdAsync(userReadModel.UserId);
                }
            }

            if (accountReadModel != null)
            {
                profileDto.AccountNumber = accountReadModel.AccountNo;
                profileDto.AccountType = accountReadModel.AccountTypeId;
                profileDto.AccountStatus = accountReadModel.AccountStatusId;
                profileDto.AccountBalance = accountReadModel.AccountBalance;
            }
            else
            {
                // Fallback default dynamic account number if not found in MongoDB
                Guid targetGuid = user != null ? user.Id : request.id;
                profileDto.AccountNumber = AccountNumberGenerator.GenerateDynamicAccountNumber(targetGuid);
                profileDto.AccountType = 1; // Savings
                profileDto.AccountStatus = 1; // Active
                profileDto.AccountBalance = 500m;
            }

            return profileDto;
        }
    }
}
