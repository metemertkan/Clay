using System;
using System.Collections.Generic;
using System.Linq;
using Clay.Models.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Clay.Data
{
    public class SeedData
    {
        private readonly IServiceScopeFactory scopeFactory;
        private UserManager<AppIdentityUser> userManager;
        private RoleManager<IdentityRole> roleManager;
        private ILogger<SeedData> _log;

        private const int LOCK_NUMBER = 100;
        private const int USER_NUMBER = 30;
        public SeedData(IServiceProvider serviceProvider)
        {
            scopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
        }

        public void EnsureSeeded()
        {
            using (var serviceScope = scopeFactory.CreateScope())
            {
                userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<AppIdentityUser>>();
                roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                _log = serviceScope.ServiceProvider.GetRequiredService<ILogger<SeedData>>();

                using (var context = serviceScope.ServiceProvider.GetService<WebDbContext>())
                {
                    _log.LogInformation("Checking if database seeded ?");
                    if (!context.Database.EnsureCreated())
                    {
                        _log.LogInformation("No need to seed");
                        return;
                    }
                    SeedRoles();
                    SeedUsers();
                    SeedLocks(context);
                    SeedUserLocks(context);
                }
            }
        }

        private void SeedUserLocks(WebDbContext context)
        {
            _log.LogInformation("Seeding user locks");
            var user1 = userManager.FindByEmailAsync("user1@user.com").Result;
            var lockList = context.Locks.Take(5).ToList();

            if (user1 == null)
                return;
            foreach (var @lock in lockList)
            {
                context.Add(new UserLock
                {
                    LockId = @lock.Id,
                    UserId = user1.Id
                });
            }
            context.SaveChanges();
        }


        private void SeedRoles()
        {
            _log.LogInformation("Seeding Roles");
            CreateRole("User");
            CreateRole("Administrator");
        }

        private void CreateRole(string roleName)
        {
            if (!roleManager.RoleExistsAsync
                (roleName).Result)
            {
                var role = new IdentityRole { Name = roleName };
                var roleResult = roleManager.CreateAsync(role).Result;
            }
        }

        private void SeedLocks(WebDbContext context)
        {
            _log.LogInformation("Seeding Locks");
            var locks = CreateLocksForSeeding();
            context.Locks.AddRange(locks);
            context.SaveChanges();
        }

        private List<Lock> CreateLocksForSeeding()
        {
            var locks = new List<Lock>();

            for (var i = 0; i < LOCK_NUMBER; i++)
            {
                locks.Add(new Lock
                {
                    Id = Guid.NewGuid(),
                    Name = $"lock{i}",
                    IsLocked = false
                });
            }
            return locks;
        }

        private void SeedUsers()
        {
            _log.LogInformation("Seeding Users");
            Createuser("mete", "mete@mete.com", "Mete123.", "Administrator");

            for (int i = 0; i < USER_NUMBER; i++)
            {
                Createuser($"user{i}", $"user{i}@user.com", "User123.", "User");
            }
        }

        private void Createuser(string username, string email, string password, string role)
        {
            if (userManager.FindByNameAsync(username).Result == null)
            {
                var user = new AppIdentityUser { UserName = username, Email = email };

                var result = userManager.CreateAsync
                    (user, password).Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user,
                        role).Wait();
                }
            }
        }
    }
}