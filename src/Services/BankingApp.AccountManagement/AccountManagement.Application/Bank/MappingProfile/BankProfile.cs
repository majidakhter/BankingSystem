using AutoMapper;
using BankingAppDDD.Domains.Banks.Entities;
using BankingAppDDD.Domains.Banks.Models;

namespace BankingApp.AccountManagement.Application.Banks.MappingProfile
{
    public class BankProfile : Profile
    {
        public BankProfile()
        {

            CreateMap<Bank, BankDTO>()
                .ForMember(dest => dest.BankId,
                            e => e.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name,
                            e => e.MapFrom(src => src.Name))
                .ForMember(dest => dest.DateAdded,
                            e => e.MapFrom(src => src.DateAdded));
                
        }
    }
}
