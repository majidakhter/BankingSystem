using AutoMapper;
using BankingApp.AccountManagement.Application.Customers.Models;
using BankingApp.AccountManagement.Core.Customers.Entities;

namespace BankingApp.AccountManagement.Application.Customers.MappingProfile
{
    public class CustomerAccountProfile : Profile
    {
        public CustomerAccountProfile()
        {

            CreateMap<Customer, CustomerAccountDTO>()
                .ForMember(dest => dest.CustomerId,
                            e => e.MapFrom(src => src.CustomerId))
                .ForMember(dest => dest.NoofAccount,
                            e => e.MapFrom(src => src.NumberOfAccounts));

        }
    }
}
