using AutoMapper;
using BankingApp.AccountManagement.Application.CustomerAccounts.Models;
using BankingAppDDD.Domains.CustomerAccounts.Entities;


namespace BankingApp.AccountManagement.Application.CustomerAccounts.MappingProfile
{
    public class CustomerAccountProfile : Profile
    {
        public CustomerAccountProfile()
        {

            CreateMap<UserAccount, CustomerAccountDTO>()
                .ForMember(dest => dest.CustomerId,
                            e => e.MapFrom(src => src.UserId))
                .ForMember(dest => dest.NoofAccount,
                            e => e.MapFrom(src => src.NumberOfAccounts));

        }
    }
}
