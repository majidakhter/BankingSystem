using AutoMapper;
using BankingApp.LoanManagement.Infrastructure.Repositories;
using BankingAppDDD.Applications.Abstractions.Queries;
using BankingAppDDD.Domains.Abstractions.Guards;
using BankingAppDDD.Domains.LoanApplications.Entities;
using BankingAppDDD.Domains.LoanApplications.Models;

namespace BankingApp.LoanManagement.Application.LoanApplicationQueries
{

    public sealed record GetLoanApplicationByOtherParam(string applicationNumber, Guid customerNationalIdentifier, Guid decisionBy, Guid registeredBy) : Query<LoanApplicationSearchCriteriaDto>;

    public sealed class GetLoanApplicationByOtherParamHandler : QueryHandler<GetLoanApplicationByOtherParam, LoanApplicationSearchCriteriaDto>
    {
        private readonly ILoanRepository<LoanApplication> _repository;

        public GetLoanApplicationByOtherParamHandler(IMapper mapper,
            ILoanRepository<LoanApplication> repository) : base(mapper)
        {
            _repository = repository;
        }

        protected override  Task<LoanApplicationSearchCriteriaDto> HandleAsync(GetLoanApplicationByOtherParam request)
        {
            var loanApplication =  _repository.GetLoanApplicationBySearchParam(request.applicationNumber,request.customerNationalIdentifier,request.decisionBy,request.registeredBy);
            Guard.Against.NotFound(loanApplication);
            var destinationObject = Mapper.Map<LoanApplicationSearchCriteriaDto>(loanApplication);
            return Task.FromResult(destinationObject);

        }
    }
}
