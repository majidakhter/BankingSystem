using AutoMapper;
using BankingApp.AccountManagement.Application.Accounts.Models;
using BankingApp.AccountManagement.Core.Accounts.Entities;

namespace BankingApp.AccountManagement.Application.Accounts.MappingProfile
{
    public class AccountStatusMappingProfile : Profile
    {
        public AccountStatusMappingProfile()
        {
            CreateMap<AccountStatus, AccountStatusDTO>()
                .ForMember(dest => dest.AccountStatusId,
                            e => e.MapFrom(src => src.Id));
        }
    }
}
