using AutoMapper;
using BankingAppDDD.CustomerManagement.Application.Customers.Models;
using BankingAppDDD.CustomerManagement.Core.Customers.Entities;

namespace BankingAppDDD.CustomerManagement.Application.Customers.MappingProfile
{
    /// <summary>
    /// 
    /// </summary>
    public class CustomerProfile : Profile
    {
        /// <summary>
        /// 
        /// </summary>
        public CustomerProfile()
        {

            CreateMap<Customer, CustomerDTO>()
                .ForMember(dest => dest.CustomerId,
                            e => e.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name,
                            e => e.MapFrom(src => src.Name))
                .ForMember(dest => dest.PhoneNo,
                            e => e.MapFrom(src => src.PhoneNo))
                .ForMember(dest => dest.Email,
                            e => e.MapFrom(src => src.Email))
                .ForMember(dest => dest.DateOfBirth,
                            e => e.MapFrom(src => src.DateOfBirth.Value))
                .ForMember(dest => dest.LoanStatus,
                            e => e.MapFrom(src => src.LoanStatus))
                .ForMember(dest => dest.DateAdded,
                            e => e.MapFrom(src => src.DateAdded))
                .ForMember(dest => dest.UpdatedOn,
                            e => e.MapFrom(src => src.UpdatedOn))
                .ForMember(dest => dest.CustomerTypeId,
                            e => e.MapFrom(src => src.CustomerTypeId))
                .ForPath(dest => dest.PermanentAddress!.City,
                            e => e.MapFrom(src => src.PermanentAddress.City))
                .ForPath(dest => dest.PermanentAddress!.State,
                            e => e.MapFrom(src => src.PermanentAddress.State))
                .ForPath(dest => dest.PermanentAddress!.ZipCode,
                            e => e.MapFrom(src => src.PermanentAddress.ZipCode))
                .ForPath(dest => dest.PermanentAddress!.Country,
                            e => e.MapFrom(src => src.PermanentAddress.Country))
                .ForPath(dest => dest.PermanentAddress!.Street,
                            e => e.MapFrom(src => src.PermanentAddress.Street));

        }
    }
}
