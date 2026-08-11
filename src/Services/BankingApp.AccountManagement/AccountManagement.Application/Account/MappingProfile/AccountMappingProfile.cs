using AutoMapper;
using BankingAppDDD.Domains.Accounts.Entities;
using BankingAppDDD.Domains.Accounts.Models;


namespace BankingApp.AccountManagement.Application.Accounts.MappingProfile
{
    public class AccountMappingProfile : Profile
    {
        public AccountMappingProfile()
        {
            CreateMap<Account, AccountDTO>()
                 .ForMember(dest => dest.AccountId,
                            e => e.MapFrom(src => src.Id))
             .ForMember(dest => dest.CustomerId,
                            e => e.MapFrom(src => src.UserId))
             .ForMember(dest => dest.AccountNo,
                            e => e.MapFrom(src => src.AccountNo))
             .ForMember(dest => dest.AccountTypeId,
                            e => e.MapFrom(src => src.AccountTypeId))
                .ForMember(dest => dest.AccountStatusId,
                            e => e.MapFrom(src => src.AccountStatusId))
                .ForMember(dest => dest.DateAdded,
                            e => e.MapFrom(src => src.DateAdded))
                .ForMember(dest => dest.AccountUpdatedDate,
                            e => e.MapFrom(src => src.AccountUpdatedDate));
        }
    }
}
