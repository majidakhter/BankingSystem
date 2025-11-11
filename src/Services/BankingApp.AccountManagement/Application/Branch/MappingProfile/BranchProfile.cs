using AutoMapper;
using BankingApp.AccountManagement.Application.Branches.Model;
using BankingApp.AccountManagement.Core.Branches.Entities;

namespace BankingApp.AccountManagement.Application.Branches.MappingProfile
{
    public class BranchProfile : Profile
    {
        public BranchProfile()
        {

            CreateMap<Branch, BranchDTO>()
                .ForMember(dest => dest.BranchId,
                            e => e.MapFrom(src => src.Id))
                .ForMember(dest => dest.BankId,
                            e => e.MapFrom(src => src.BankId))
                .ForMember(dest => dest.DateAdded,
                            e => e.MapFrom(src => src.DateAdded))
                .ForMember(dest => dest.BranchCode,
                            e => e.MapFrom(src => src.BranchCode))
                .ForMember(dest => dest.Name,
                            e => e.MapFrom(src => src.Name))
                 .ForMember(dest => dest.PhoneNumber,
                            e => e.MapFrom(src => src.PhoneNumber.Value))
                 .ForMember(dest => dest.Street,
                            e => e.MapFrom(src => src.BranchAddress.Street))
                 .ForMember(dest => dest.City,
                            e => e.MapFrom(src => src.BranchAddress.City))
                 .ForMember(dest => dest.State,
                            e => e.MapFrom(src => src.BranchAddress.State))
                 .ForMember(dest => dest.ZipCode,
                            e => e.MapFrom(src => src.BranchAddress.ZipCode))
                 .ForMember(dest => dest.Country,
                            e => e.MapFrom(src => src.BranchAddress.Country))
                ;
        }
    }
}
