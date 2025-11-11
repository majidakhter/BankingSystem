using AutoMapper;
using BankingApp.LoanManagement.Application.OperatorsModel;
using BankingApp.LoanManagement.Core.LoanApplicationsEntities;

namespace BankingApp.LoanManagement.Application.OperatorsMappingProfile
{
    public class OperatorProfile : Profile
    {
        public OperatorProfile()
        {

            CreateMap<Operator, OperatorDTO>()
                .ForMember(dest => dest.OperatorId,
                            e => e.MapFrom(src => src.Id))
                .ForMember(dest => dest.CompetenceLevelAmount,
                            e => e.MapFrom(src => src.CompetenceLevel.Value));
        }
    }
}
