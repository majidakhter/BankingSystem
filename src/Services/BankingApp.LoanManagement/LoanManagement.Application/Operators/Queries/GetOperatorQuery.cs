using AutoMapper;
using BankingApp.LoanManagement.Infrastructure.Repositories;
using BankingAppDDD.Applications.Abstractions.Queries;
using BankingAppDDD.Domains.Abstractions.Guards;
using BankingAppDDD.Domains.LoanApplications.Entities;
using BankingAppDDD.Domains.Operators.Models;

namespace BankingApp.LoanManagement.Application.OperatorsQueries
{
    public sealed record GetOperatorQuery() : Query<List<OperatorDTO>>;
    public sealed class GetOperatorQueryHandler : QueryHandler<GetOperatorQuery, List<OperatorDTO>>
    {
        private readonly ILoanRepository<Operator> _repository;

        public GetOperatorQueryHandler(IMapper mapper,
            ILoanRepository<Operator> repository) : base(mapper)
        {
            _repository = repository;
        }

        protected override Task<List<OperatorDTO>> HandleAsync(GetOperatorQuery request)
        {
            var operators = _repository.GetAll();
            Guard.Against.NotFound(operators);
            Task<IQueryable<Operator>> data = Task.FromResult(operators);
            var destinationObject = Mapper.Map<List<OperatorDTO>>(data);
            return Task.FromResult(destinationObject);
        }
    }
}
