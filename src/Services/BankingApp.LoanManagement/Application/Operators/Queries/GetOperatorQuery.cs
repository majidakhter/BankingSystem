using AutoMapper;
using BankingApp.LoanManagement.Application.OperatorsModel;
using BankingApp.LoanManagement.Core.LoanApplicationsEntities;
using BankingAppDDD.Applications.Abstractions.Queries;
using BankingAppDDD.Applications.Abstractions.Repositories;
using BankingAppDDD.Domains.Abstractions.Guards;
using System.Collections.Generic;

namespace BankingApp.LoanManagement.Application.OperatorsQueries
{
    public sealed record GetOperatorQuery() : Query<List<OperatorDTO>>;
    public sealed class GetOperatorQueryHandler : QueryHandler<GetOperatorQuery, List<OperatorDTO>>
    {
        private readonly IRepository<Operator> _repository;

        public GetOperatorQueryHandler(IMapper mapper,
            IRepository<Operator> repository) : base(mapper)
        {
            _repository = repository;
        }

        protected override  Task<List<OperatorDTO>> HandleAsync(GetOperatorQuery request)
        {
            var operators =  _repository.GetAll();
            Guard.Against.NotFound(operators);
            Task<IQueryable<Operator>> data= Task.FromResult(operators);
            var destinationObject = Mapper.Map<List<OperatorDTO>>(data);
            return Task.FromResult(destinationObject);
        }
    }
}
