using Microsoft.AspNetCore.Identity;
using Portfolio.Data.Context;
using Portfolio.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Portfolio.Data.Seeds
{
    public static class AdminSeed
    {
        public static async Task SeedAsync(
            IServiceProvider serviceProvider,
            IConfiguration configuration)
        {
            using var scope = serviceProvider.CreateScope();

            var context = scope.ServiceProvider
                .GetRequiredService<AppDbContext>();

            var passwordHasher = scope.ServiceProvider
                .GetRequiredService<IPasswordHasher<Admin>>();

            var userName = configuration["AdminSeed:UserName"];
            var fullName = configuration["AdminSeed:FullName"];
            var email = configuration["AdminSeed:Email"];
            var password = configuration["AdminSeed:Password"];

            if (string.IsNullOrWhiteSpace(userName) ||
                string.IsNullOrWhiteSpace(fullName) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                return;
            }

            var admin = await context.Admins
                .FirstOrDefaultAsync(x => x.UserName == userName);

            if (admin is null)
            {
                admin = new Admin
                {
                    UserName = userName,
                    FullName = fullName,
                    Email = email
                };

                admin.Password = passwordHasher.HashPassword(
                    admin,
                    password);

                await context.Admins.AddAsync(admin);
                await context.SaveChangesAsync();

                return;
            }

            admin.FullName = fullName;
            admin.Email = email;

           
            if (admin.Password == password)
            {
                admin.Password = passwordHasher.HashPassword(
                    admin,
                    password);
            }

            await context.SaveChangesAsync();
        }
    }
}
