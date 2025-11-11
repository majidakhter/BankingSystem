using AutoMapper;
using BankingApp.LoanManagement.Application.LoanApplicationModels;
using BankingApp.LoanManagement.Core.LoanApplicationsEntities;

namespace BankingApp.LoanManagement.Application.LoanApplicationMappingProfile
{
    public class LoanApplicationProfile : Profile
    {
        public LoanApplicationProfile()
        {
            CreateMap<LoanApplication, LoanApplicationDTO>()
                .ForMember(dest => dest.LoanId,
                            e => e.MapFrom(src => src.Id))
                .ForMember(dest => dest.LoanApplicationNumber,
                            e => e.MapFrom(src => src.Number))
                .ForMember(dest => dest.CustomerId,
                            e => e.MapFrom(src => src.Customer.CustomerId))
                .ForMember(dest => dest.CustomerBirthDate,
                            e => e.MapFrom(src => src.Customer.BirthDate))
                .ForMember(dest => dest.CustomerMonthlyIncome,
                            e => e.MapFrom(src => src.Customer.MonthlyIncome.Value))
                .ForMember(dest => dest.LoanTypeId,
                            e => e.MapFrom(src => src.LoanTypeId))
                .ForMember(dest => dest.LoanAmount,
                            e => e.MapFrom(src => src.Loan.LoanAmount.Value))
                .ForMember(dest => dest.InterestRate,
                            e => e.MapFrom(src => src.Loan.InterestRate.Value))
                .ForMember(dest => dest.LoanNoOfYears,
                            e => e.MapFrom(src => src.Loan.LoanNumberOfYears))
                .ForMember(dest => dest.Status,
                            e => e.MapFrom(src => src.Status))
                .ForMember(dest => dest.Explanation,
                            e => e.MapFrom(src => src.Score.Explanation))
                .ForMember(dest => dest.ScoringResult,
                            e => e.MapFrom(src => src.Score.Score.ToString()))
                .ForMember(dest => dest.PropertyValue,
                            e => e.MapFrom(src => src.Asset.Value.Value))
                .ForMember(dest => dest.DecisionBy,
                            e => e.MapFrom(src => src.Decision.DecisionBy))
                .ForMember(dest => dest.DecisionDate,
                            e => e.MapFrom(src => src.Decision.DecisionDate))
                .ForMember(dest => dest.RegisteredBy,
                            e => e.MapFrom(src => src.Registration.RegisteredBy))
                .ForMember(dest => dest.RegistrationDate,
                            e => e.MapFrom(src => src.Registration.RegistrationDate))
                .ForPath(dest => dest.PropertyAddress!.Street,
                            e => e.MapFrom(src => src.Asset.Address.Street))
                .ForPath(dest => dest.PropertyAddress!.City,
                            e => e.MapFrom(src => src.Asset.Address.City))
                .ForPath(dest => dest.PropertyAddress!.State,
                            e => e.MapFrom(src => src.Asset.Address.State))
                .ForPath(dest => dest.PropertyAddress!.ZipCode,
                            e => e.MapFrom(src => src.Asset.Address.ZipCode))
                .ForPath(dest => dest.PropertyAddress!.Country,
                            e => e.MapFrom(src => src.Asset.Address.Country));
        }
    }
}
