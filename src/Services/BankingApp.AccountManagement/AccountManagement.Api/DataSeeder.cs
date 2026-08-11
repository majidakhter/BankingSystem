using BankingAppDDD.Domains.Abstractions.Models;

using BankingAppDDD.Domains.Abstractions.ValueObjects;
using BankingAppDDD.Domains.Abstractions.ValueObjects.Shared;
using BankingAppDDD.Domains.Banks.Entities;
using BankingAppDDD.Domains.Branches.Entities;
using BankingAppDDD.MongoService.Application.Mongo;
using BankingAppDDD.MongoService.Mongo.Model;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace BankingApp.AccountManagement
{
    public class BankSeedDto
    {
        public Guid BankId { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime? DateAdded { get; set; }
    }

    public class BranchSeedDto
    {
        public Guid BranchId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string BranchCode { get; set; } = string.Empty;
        public string IfscCode { get; set; } = string.Empty;
        public int MicrCode { get; set; }
        public DateTime? DateAdded { get; set; }
        public Guid BankId { get; set; }
        public string? PhoneNumber { get; set; }
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
    }

    public static class DataSeeder
    {
        public static async Task SeedDataAsync(WebApplication app, string seedDataDir)
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AccountDbContext>();
            var mongoService = scope.ServiceProvider.GetService<IAccountMongoService>();

            try
            {
                // Ensure Database Migrated
                await context.Database.MigrateAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DB Migration check note: {ex.Message}");
            }

            if (!Directory.Exists(seedDataDir))
            {
                Console.WriteLine($"Warning: SeedData directory not found at {seedDataDir}");
                return;
            }

            // 1. Seed Bank in PostgreSQL & Mongo
            string bankJsonPath = Path.Combine(seedDataDir, "Bank.json");
            if (File.Exists(bankJsonPath) && !await context.Banks.AnyAsync())
            {
                string content = await File.ReadAllTextAsync(bankJsonPath);
                var bankDtos = JsonConvert.DeserializeObject<List<BankSeedDto>>(content);
                if (bankDtos != null)
                {
                    foreach (var dto in bankDtos)
                    {
                        var bank = Bank.Create(dto.Name, dto.DateAdded ?? DateTime.UtcNow);
                        var idProp = typeof(Bank).GetProperty("Id");
                        if (idProp != null && dto.BankId != Guid.Empty)
                        {
                            idProp.SetValue(bank, dto.BankId);
                        }

                        context.Banks.Add(bank);
                        if (mongoService != null)
                        {
                            await mongoService.SaveBankDetailAsync(bank);
                        }
                    }
                    await context.SaveChangesAsync();
                    Console.WriteLine("Bank seed data applied successfully.");
                }
            }

            // 2. Seed Branches in PostgreSQL & Mongo
            string branchJsonPath = Path.Combine(seedDataDir, "Branches.json");
            if (File.Exists(branchJsonPath) && !await context.Branches.AnyAsync())
            {
                string content = await File.ReadAllTextAsync(branchJsonPath);
                var branchDtos = JsonConvert.DeserializeObject<List<BranchSeedDto>>(content);
                if (branchDtos != null)
                {
                    foreach (var dto in branchDtos)
                    {
                        var addressData = new AddressData(
                            dto.Street ?? "Main St",
                            dto.City ?? "Kolkata",
                            dto.State ?? "West Bengal",
                            dto.ZipCode ?? "700001",
                            dto.Country ?? "India");

                        var branch = Branch.Create(
                            dto.Name,
                            dto.BranchCode,
                            dto.IfscCode,
                            dto.MicrCode,
                            dto.BankId,
                            dto.PhoneNumber ?? "9876543210",
                            addressData);

                        var idProp = typeof(Branch).GetProperty("Id");
                        if (idProp != null && dto.BranchId != Guid.Empty)
                        {
                            idProp.SetValue(branch, dto.BranchId);
                        }

                        context.Branches.Add(branch);
                        if (mongoService != null)
                        {
                            await mongoService.SaveBranchDetailAsync(branch);
                        }
                    }
                    await context.SaveChangesAsync();
                    Console.WriteLine("Branches seed data applied successfully.");
                }
            }

            // 3. Seed Country in Mongo DB ONLY
            string countryJsonPath = Path.Combine(seedDataDir, "Country.json");
            if (File.Exists(countryJsonPath) && mongoService != null)
            {
                string content = await File.ReadAllTextAsync(countryJsonPath);
                var countries = JsonConvert.DeserializeObject<List<CountryReadModel>>(content);
                if (countries != null && countries.Count > 0)
                {
                    await mongoService.SaveCountriesAsync(countries);
                    Console.WriteLine("Country seed data applied to MongoDB successfully.");
                }
            }

            // 4. Seed State in Mongo DB ONLY
            string stateJsonPath = Path.Combine(seedDataDir, "State.json");
            if (File.Exists(stateJsonPath) && mongoService != null)
            {
                string content = await File.ReadAllTextAsync(stateJsonPath);
                var states = JsonConvert.DeserializeObject<List<StateReadModel>>(content);
                if (states != null && states.Count > 0)
                {
                    await mongoService.SaveStatesAsync(states);
                    Console.WriteLine("State seed data applied to MongoDB successfully.");
                }
            }
        }
    }
}
