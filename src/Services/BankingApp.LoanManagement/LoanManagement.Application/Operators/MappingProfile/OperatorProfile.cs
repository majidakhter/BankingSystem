using AutoMapper;
using BankingAppDDD.Domains.LoanApplications.Entities;
using BankingAppDDD.Domains.Operators.Models;


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
