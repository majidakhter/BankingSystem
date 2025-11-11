using AutoMapper;
using BankingApp.LoanManagement.Application.LoanApplicationModels;
using BankingApp.LoanManagement.Core.LoanApplicationsEntities;
using BankingApp.LoanManagement.Infrastructure.Repositories;
using BankingAppDDD.Applications.Abstractions.Queries;
using BankingAppDDD.Domains.Abstractions.Guards;

namespace BankingApp.LoanManagement.Application.LoanApplicationQueries
{
   
    public sealed record GetLoanApplicationsQueryById(Guid loanApplicationId) : Query<LoanApplicationDTO>;

    public sealed class GetLoanApplicationsQueryByIdHandler : QueryHandler<GetLoanApplicationsQueryById, LoanApplicationDTO>
    {
        private readonly ILoanRepository<LoanApplication> _repository;

        public GetLoanApplicationsQueryByIdHandler(IMapper mapper,
            ILoanRepository<LoanApplication> repository) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<LoanApplicationDTO> HandleAsync(GetLoanApplicationsQueryById request)
        {
            var account = await _repository.GetByIdAsync(request.loanApplicationId);
            Guard.Against.NotFound(account);
            return Mapper.Map<LoanApplicationDTO>(account);

        }
    }
}
