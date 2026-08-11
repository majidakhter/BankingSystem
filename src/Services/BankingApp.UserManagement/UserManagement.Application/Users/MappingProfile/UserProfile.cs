using AutoMapper;
using BankingAppDDD.UserManagement.Core.Users.Entities;
using BankingAppDDD.Domains.Users.Models;

namespace BankingAppDDD.UserManagement.Application.Users.MappingProfile
{
    /// <summary>
    /// 
    /// </summary>
    public class UserProfile : Profile
    {
        /// <summary>
        /// 
        /// </summary>
        public UserProfile()
        {

            CreateMap<User, UserDTO>()
                .ForMember(dest => dest.UserId,
                            e => e.MapFrom(src => src.Id))
                .ForMember(dest => dest.FirstName,
                            e => e.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName,
                            e => e.MapFrom(src => src.LastName))
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
                .ForMember(dest => dest.UserTypeId,
                            e => e.MapFrom(src => src.UserTypeId))
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
