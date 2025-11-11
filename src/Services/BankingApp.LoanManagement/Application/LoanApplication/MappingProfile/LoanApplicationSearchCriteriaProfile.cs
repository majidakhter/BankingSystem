using AutoMapper;
using BankingApp.LoanManagement.Application.LoanApplicationModels;
using BankingApp.LoanManagement.Core.LoanApplicationsEntities;

namespace BankingApp.LoanManagement.Application.LoanApplicationMappingProfile
{
    public class LoanApplicationSearchCriteriaProfile : Profile
    {
        public LoanApplicationSearchCriteriaProfile()
        {
            CreateMap<LoanApplication, LoanApplicationSearchCriteriaDto>()
                .ForMember(dest => dest.LoanApplicationNumber,
                            e => e.MapFrom(src => src.Number.Number))
                .ForMember(dest => dest.Status,
                            e => e.MapFrom(src => src.Status))
                .ForMember(dest => dest.DecisionDate,
                            e => e.MapFrom(src => src.Decision.DecisionDate))
                .ForMember(dest => dest.LoanAmount,
                            e => e.MapFrom(src => src.Loan.LoanAmount.Value))
                .ForMember(dest => dest.DecisionBy,
                            e => e.MapFrom(src => src.Decision.DecisionBy));
        }
    }
}
