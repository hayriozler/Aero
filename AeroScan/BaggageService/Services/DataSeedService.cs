using BaggageService.Persistence;
using Contracts.Consts;
using Domain.Aggregates.Companies;
using Domain.Aggregates.Containers;
using Domain.Aggregates.Mappings;
using Domain.Aggregates.Permissions;
using Domain.Aggregates.Roles;
using Domain.Aggregates.Settings;
using Domain.Aggregates.Users;
using Domain.Enums;
using Infrastructure.Auth;
using Microsoft.EntityFrameworkCore;

namespace BaggageService.Services;

public class DataSeedService(AeroScanDataContext dataContext)
{
    public async Task SeedAsync()
    {

        await dataContext.Database.MigrateAsync();

        // ── Seed: Roller ──────────────────────────────────────────────────────
        var allRoles = new[]
        {
        (SystemDefinedRoles.Admin,          "System Administrator"),
        (SystemDefinedRoles.RamSupervisor,  "Supervisor"),
        (SystemDefinedRoles.BaggageAgent,   "Baggage Agent"),
    };

        var existingRoleNames = await dataContext.RoleSet.Select(r => r.Name).ToHashSetAsync();
        foreach (var (name, displayName) in allRoles)
        {
            if (!existingRoleNames.Contains(name))
                dataContext.RoleSet.Add(Role.Create(name, displayName));
        }
        await dataContext.SaveChangesAsync();

        // ── Seed: Permissions ─────────────────────────────────────────────────
        var defaultPermissions = new[]
        {
            ("users.view",            "View Users",                "Users"),
            ("users.create",          "Create Users",              "Users"),
            ("users.edit",            "Edit Users",                "Users"),
            ("users.delete",          "Delete Users",              "Users"),
            ("roles.view",            "View Roles",                "Roles"),
            ("roles.manage",          "Manage Roles",              "Roles"),
            ("permissions.manage",    "Manage Permissions",        "Permissions"),
            ("flights.view",          "View Flights",              "Flights"),
            ("bags.view",             "View Bags",                 "Bags"),
            ("bags.scan",             "Scan Bags",                 "Bags"),
            ("reports.view",          "View Reports",              "Reports"),
            ("contracts.view",        "View Handling Contracts",   "Contracts"),
            ("contracts.manage",      "Manage Handling Contracts", "Contracts"),
            ("hht.view",              "View HHT Terminals",        "HHT"),
            ("hht.manage",            "Manage HHT Terminals",      "HHT"),
            ("companies.view",        "View Companies",            "Companies"),
            ("companies.manage",      "Manage Companies",          "Companies"),
        };

        var existingPermNames = await dataContext.PermissionSet.Select(p => p.Name).ToHashSetAsync();
        foreach (var (name, displayName, group) in defaultPermissions)
        {
            if (!existingPermNames.Contains(name))
                dataContext.PermissionSet.Add(Permission.Create(name, displayName, group));
        }
        await dataContext.SaveChangesAsync();

        if (!await dataContext.CompanySet.AnyAsync())
        {
            dataContext.CompanySet.Add(Company.Create("APT", "Airport Operator", CompanyType.AirportOperator));
            dataContext.CompanySet.Add(Company.Create("Celebi", "Çelebi", CompanyType.HandlingAgent));
            dataContext.CompanySet.Add(Company.Create("Tgs", "Turkish Ground Services", CompanyType.HandlingAgent));
            dataContext.CompanySet.Add(Company.Create("Havas", "Havaş A.ş", CompanyType.HandlingAgent));
            await dataContext.SaveChangesAsync();
        }

        if (!await dataContext.UserSet.AnyAsync())
        {
            var defaultCompany = await dataContext.CompanySet.FirstAsync(w => w.Type == CompanyType.AirportOperator);
            var adminRole = await dataContext.RoleSet.FirstAsync(r => r.Name == SystemDefinedRoles.Admin);

            var ramSupervisorRole = await dataContext.RoleSet.FirstAsync(r => r.Name == SystemDefinedRoles.RamSupervisor);

            var adminUser = User.Create(
                username: "admin",
                displayName: "System Administrator",
                passwordHash: PasswordHasher.Hash("Admin@12345"),
                companyCode: defaultCompany.Code);

            dataContext.UserSet.Add(adminUser);
            adminUser.AssignRole(adminRole.Name);

            var celebiCompany = await dataContext.CompanySet.FirstAsync(w => w.Type == CompanyType.HandlingAgent && w.Code == "CELEBI");
            var ramCelebiSuperVisor = User.Create(
                username: "ramCelebiSuperVisor",
                displayName: "Çelebi Ram Supervisor",
                passwordHash: PasswordHasher.Hash("Admin@12345"),
                companyCode: celebiCompany.Code);
            dataContext.UserSet.Add(ramCelebiSuperVisor);
            ramCelebiSuperVisor.AssignRole(ramSupervisorRole.Name);

            var tgsCompany = await dataContext.CompanySet.FirstAsync(w => w.Type == CompanyType.HandlingAgent && w.Code == "TGS");
            var ramTgsSuperVisor = User.Create(
            username: "ramTgsSuperVisor",
            displayName: "Tgs Ram supervisor",
            passwordHash: PasswordHasher.Hash("Admin@12345"),
            companyCode: tgsCompany.Code);
            dataContext.UserSet.Add(ramTgsSuperVisor);
            ramTgsSuperVisor.AssignRole(ramSupervisorRole.Name);

            await dataContext.SaveChangesAsync();
        }

        if (!await dataContext.ContainerTypeSet.AnyAsync())
        {
            dataContext.ContainerTypeSet.AddRange(
            [
                ContainerType.Create("F", "FIRST", false, false),
                ContainerType.Create("L", "LOCAL", false, false),
                ContainerType.Create("M", "MIXED", false, true),
                ContainerType.Create("N", "NIL", false, false),
                ContainerType.Create("P", "PRIORITY", false, false),
                ContainerType.Create("S", "SORT", false, false),
                ContainerType.Create("T", "TRANSFER", false, false)
             ]
            );
            await dataContext.SaveChangesAsync();
        }

        if (!await dataContext.ContainerTypeClassSet.AnyAsync())
        {
            dataContext.ContainerTypeClassSet.AddRange(
                ContainerTypeClass.Create("F", "F"),
                ContainerTypeClass.Create("L", "F"),
                ContainerTypeClass.Create("L", "C"),
                ContainerTypeClass.Create("L", "Y"),
                ContainerTypeClass.Create("M", "L"),
                ContainerTypeClass.Create("M", "T"),
                ContainerTypeClass.Create("N", ""),
                ContainerTypeClass.Create("P", "F"),
                ContainerTypeClass.Create("P", "C"),
                ContainerTypeClass.Create("P", "J"),
                ContainerTypeClass.Create("S", "F"),
                ContainerTypeClass.Create("S", "C"),
                ContainerTypeClass.Create("S", "Y"),
                ContainerTypeClass.Create("T", "F"),
                ContainerTypeClass.Create("T", "C"),
                ContainerTypeClass.Create("T", "Y")
            );
            await dataContext.SaveChangesAsync();
        }

        if (await dataContext.SystemConfigurationSet.FirstOrDefaultAsync(p => p.Key == "AIRPORT_CODE") is null)
        {
            dataContext.SystemConfigurationSet.Add(SystemConfiguration.Create("AIRPORT_CODE", "AYT"));
        }
        if (await dataContext.SystemConfigurationSet.FirstOrDefaultAsync(p => p.Key == "SOURCE_INDICATOR") is null)
        {
            dataContext.SystemConfigurationSet.Add(SystemConfiguration.Create("SOURCE_INDICATOR", "L"));
        }
        if (await dataContext.SystemConfigurationSet.FirstOrDefaultAsync(p => p.Key == "VERSION") is null)
        {
            dataContext.SystemConfigurationSet.Add(SystemConfiguration.Create("VERSION", "1"));
        }
        if (await dataContext.SystemConfigurationSet.FirstOrDefaultAsync(p => p.Key == "CONTAINER_STATUS.OPEN") is null)
        {
            dataContext.SystemConfigurationSet.Add(SystemConfiguration.Create("CONTAINER_STATUS.OPEN", "O", "Open"));
        }
        if (await dataContext.SystemConfigurationSet.FirstOrDefaultAsync(p => p.Key == "CONTAINER_STATUS.CLOSE") is null)
        {
            dataContext.SystemConfigurationSet.Add(SystemConfiguration.Create("CONTAINER_STATUS.CLOSE", "C", "Close"));
        }
        await dataContext.SaveChangesAsync();

        if (await dataContext.ResourceStatusMapSet.FirstOrDefaultAsync(p => p.SourceResourceName == "CheckIn" && p.SourceResourceStatus == "Open") is null)
        {
            dataContext.ResourceStatusMapSet.Add(ResourceStatusMap.Create("CheckIn", "Open", "O"));
        }
        if (await dataContext.ResourceStatusMapSet.FirstOrDefaultAsync(p => p.SourceResourceName == "CheckIn" && p.SourceResourceStatus == "Close") is null)
        {
            dataContext.ResourceStatusMapSet.Add(ResourceStatusMap.Create("CheckIn", "Close", "C"));
        }

        if (await dataContext.ResourceStatusMapSet.FirstOrDefaultAsync(p => p.SourceResourceName == "Gate" && (p.SourceResourceStatus == "Open" || p.SourceResourceStatus == "Boarding")) is null)
        {
            dataContext.ResourceStatusMapSet.Add(ResourceStatusMap.Create("Gate", "Open", "O"));
            dataContext.ResourceStatusMapSet.Add(ResourceStatusMap.Create("Gate", "Boarding", "O"));
        }
        if (await dataContext.ResourceStatusMapSet.FirstOrDefaultAsync(p => p.SourceResourceName == "Gate" && (p.SourceResourceStatus == "Open" || p.SourceResourceStatus == "Deboarding")) is null)
        {
            dataContext.ResourceStatusMapSet.Add(ResourceStatusMap.Create("Gate", "Close", "C"));
            dataContext.ResourceStatusMapSet.Add(ResourceStatusMap.Create("Gate", "Deboarding", "C"));
        }
        if (await dataContext.ResourceStatusMapSet.FirstOrDefaultAsync(p => p.SourceResourceName == "Carousel" && p.SourceResourceStatus == "Open") is null)
        {
            dataContext.ResourceStatusMapSet.Add(ResourceStatusMap.Create("Carousel", "Open", "O"));
        }
        await dataContext.SaveChangesAsync();
    }
    
}
