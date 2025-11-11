using AutoMapper;
using BankingApp.AccountManagement.Application.Banks.Models;
using BankingApp.AccountManagement.Core.Banks.Entities;

namespace BankingApp.AccountManagement.Application.Banks.MappingProfile
{
    public class BankProfile : Profile
    {
        public BankProfile()
        {

            CreateMap<Bank, BankDTO>()
                .ForMember(dest => dest.BankId,
                            e => e.MapFrom(src => src.Id))
                .ForMember(dest => dest.DateAdded,
                            e => e.MapFrom(src => src.DateAdded))
                .ForMember(dest => dest.PhoneNumber,
                            e => e.MapFrom(src => src.PhoneNumber.Value))
                .ForMember(dest => dest.DateAdded,
                            e => e.MapFrom(src => src.DateAdded))
                .ForMember(dest => dest.Street,
                            e => e.MapFrom(src => src.BankAddress.Street))
                 .ForMember(dest => dest.City,
                            e => e.MapFrom(src => src.BankAddress.City))
                 .ForMember(dest => dest.State,
                            e => e.MapFrom(src => src.BankAddress.State))
                 .ForMember(dest => dest.ZipCode,
                            e => e.MapFrom(src => src.BankAddress.ZipCode))
                 .ForMember(dest => dest.Country,
                            e => e.MapFrom(src => src.BankAddress.Country))
                .ForMember(dest => dest.Name,
                            e => e.MapFrom(src => src.Name));
        }
    }
}
