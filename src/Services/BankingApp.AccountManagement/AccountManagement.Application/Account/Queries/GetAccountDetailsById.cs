using AutoMapper;
using BankingApp.AccountManagement.Infrastructure.Repositories;
using BankingAppDDD.Applications.Abstractions.Queries;
using BankingAppDDD.Applications.Abstractions.Repositories;
using BankingAppDDD.Domains.Accounts.Entities;
using BankingAppDDD.Domains.Accounts.Models;
using BankingAppDDD.MongoService.Application.Mongo;
using BankingAppDDD.MongoService.Mongo.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace BankingApp.AccountManagement.Application.Accounts.Queries
{
    public sealed record GetAccountDetailsByIdQuery(Guid UserId) : Query<List<UserAccountDTO>>;

    public sealed class GetAccountDetailsByIdQueryHandler : QueryHandler<GetAccountDetailsByIdQuery, List<UserAccountDTO>>
    {
        private readonly IAccountRepository<Account> _repository;
        private readonly IRepository<BeneficiaryGroup> _beneficiaryRepository;
        private readonly IAccountMongoService? _mongoService;

        public GetAccountDetailsByIdQueryHandler(
            IMapper mapper,
            IAccountRepository<Account> repository,
            IRepository<BeneficiaryGroup> beneficiaryRepository,
            IAccountMongoService? mongoService = null) : base(mapper)
        {
            _repository = repository;
            _beneficiaryRepository = beneficiaryRepository;
            _mongoService = mongoService;
        }

        protected override async Task<List<UserAccountDTO>> HandleAsync(GetAccountDetailsByIdQuery request)
        {
            // Eagerly fetch accounts along with credits and debits in a single SQL query
            var accounts = await _repository.GetAccountsWithDetailsAsync(x => x.UserId == request.UserId || x.KeycloakUserId == request.UserId);
            
            UserReadModelMapper? userReadModel = null;
            if (_mongoService != null)
            {
                userReadModel = await _mongoService.GetUserByIdAsync(request.UserId); 
            }

            var allBeneficiaries = await _beneficiaryRepository.GetAll().ToListAsync();

            var lstuserAccountDto = new List<UserAccountDTO>();
            foreach (var account in accounts)
            {
                var userAccountDto = new UserAccountDTO();
                if (account != null)
                {
                    userAccountDto.AccountId = account.Id;
                    userAccountDto.AccountNo = account.AccountNo;
                    userAccountDto.AccountTypeId = account.AccountTypeId;
                    userAccountDto.AccountStatusId = account.AccountStatusId;
                    userAccountDto.CurrentBalance = account.GetCurrentBalance().Value;

                    var creditsForAccount = new List<CreditDTO>();
                    foreach (var credit in account.CreditsCollection)
                    {
                        var creditDTO = new CreditDTO
                        {
                            TransactionNumber = credit.TransactionNo,
                            TransactionAmount = credit.Amount != null ? credit.Amount.Value : 0,
                            TransactionDate = credit.TransactionDate
                        };
                        creditsForAccount.Add(creditDTO);
                    }
                    userAccountDto.TransactionDetail = creditsForAccount;

                    // Combine Beneficiaries for this account into UserAccountDTO
                    var beneficiariesForAccount = allBeneficiaries
                        .Where(b => b.LoginUserAccountId == account.Id || b.LoginUserAccountId == request.UserId)
                        .Select(b => new BeneficiaryDTO
                        {
                            Id = b.Id,
                            LoginUserAccountId = b.LoginUserAccountId,
                            BeneficaryName = b.Beneficiary?.BeneficaryName ?? string.Empty,
                            BeneficaryAccountNo = b.Beneficiary?.BeneficaryAccountNo ?? 0,
                            BeneficaryBankName = b.Beneficiary?.BeneficaryBankName ?? string.Empty,
                            BeneficaryIfscCode = b.Beneficiary?.BeneficaryIfscCode ?? string.Empty,
                            AddedDate = b.AddedDate
                        }).ToList();

                    userAccountDto.BeneficiariesDetail = beneficiariesForAccount;
                }

                if (userReadModel != null)
                {
                    userAccountDto.UserFullName = $"{userReadModel.FirstName} {userReadModel.LastName}";
                }
                lstuserAccountDto.Add(userAccountDto);
            }
            
            return lstuserAccountDto;
        }
    }
}
