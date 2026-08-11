using AutoMapper;
using BankingAppDDD.Domains.Accounts.Entities;
using BankingAppDDD.Domains.Accounts.Models;


namespace BankingApp.AccountManagement.Application.Accounts.MappingProfile
{
    public class AccountStatusMappingProfile : Profile
    {
        public AccountStatusMappingProfile()
        {
            CreateMap<Account, AccountStatusDTO>()
                .ForMember(dest => dest.AccountStatusId,
                            e => e.MapFrom(src => src.AccountStatus.Id));
        }
    }
}
