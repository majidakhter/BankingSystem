using Microsoft.AspNetCore.Http;
using System;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace BankingAppDDD.Domains.Users.Models
{
    public record class UserData
    {
        public string? UserName { get; set; } = string.Empty;
        public string? Password { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string? FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; } = string.Empty;
        public string? PhoneNo { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
        public int UserType { get; set; } = 1;
        public string? Gender { get; set; } = "male";
        public string? SSNumber { get; set; } = string.Empty;

        [property: System.Text.Json.Serialization.JsonIgnore]
        [property: Newtonsoft.Json.JsonIgnore]
        public IFormFile? ProfileImage { get; set; }

        public byte[]? ProfileImageBytes { get; set; }

        public void EnsureProfileImageBytes()
        {
            if ((ProfileImageBytes == null || ProfileImageBytes.Length == 0) && ProfileImage != null && ProfileImage.Length > 0)
            {
                try
                {
                    using var stream = ProfileImage.OpenReadStream();
                    if (stream.CanSeek) stream.Position = 0;
                    using var ms = new System.IO.MemoryStream();
                    stream.CopyTo(ms);
                    ProfileImageBytes = ms.ToArray();
                }
                catch { }
            }
        }

        [System.Text.Json.Serialization.JsonConstructor]
        [Newtonsoft.Json.JsonConstructor]
        public UserData() { }

        public UserData(
            string? userName,
            string? password,
            string? email,
            string? firstName,
            string? lastName,
            string? phoneNo,
            DateOnly dateOfBirth,
            int userType,
            string? gender,
            string? ssNumber,
            IFormFile? profileImage = null)
        {
            UserName = userName ?? string.Empty;
            Password = password ?? string.Empty;
            Email = email ?? string.Empty;
            FirstName = firstName ?? string.Empty;
            LastName = lastName ?? string.Empty;
            PhoneNo = phoneNo ?? string.Empty;
            DateOfBirth = dateOfBirth;
            UserType = userType;
            Gender = gender ?? "male";
            SSNumber = ssNumber ?? string.Empty;
            ProfileImage = profileImage;
        }

        public void Deconstruct(
            out string userName,
            out string password,
            out string email,
            out string firstName,
            out string lastName,
            out string phoneNo,
            out DateOnly dateOfBirth,
            out int userType,
            out string gender,
            out string ssNumber,
            out IFormFile? profileImage)
        {
            userName = UserName ?? string.Empty;
            password = Password ?? string.Empty;
            email = Email ?? string.Empty;
            firstName = FirstName ?? string.Empty;
            lastName = LastName ?? string.Empty;
            phoneNo = PhoneNo ?? string.Empty;
            dateOfBirth = DateOfBirth;
            userType = UserType;
            gender = Gender ?? "male";
            ssNumber = SSNumber ?? string.Empty;
            profileImage = ProfileImage;
        }
    }
}
