using EFSoftDelete.Infrastructure.Interceptors;
using EFSoftDelete.Infrastructure.Persistence;
using EFSoftDelete.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using static System.Formats.Asn1.AsnWriter;

namespace EFSoftDelete
{

    internal class Program
    {

        static async Task Main(string[] args)
        {
            using IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((builder, services) =>
                {
                    services.AddDbContext<ApplicationDbContext>(options => 
                    {
                        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
                        options.AddInterceptors(new EntityDeleteInterceptor());
                    });

                    services.AddDbContext<AuditReaderDbContext>(options =>
                    {
                        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
                    });
                })
                .Build();

            using (IServiceScope scope = host.Services.CreateScope())
            {
                using ApplicationDbContext migrateContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                if (migrateContext.Database.IsRelational()) migrateContext.Database.Migrate();
            }

            Guid userId = await CreateUser(host);
            
            await DeleteUser(host, userId);

            await QueryUser(host, userId);

            await QueryUserForAudit(host, userId);
        }

        static async Task<Guid> CreateUser(IHost host)
        {
            using IServiceScope scope = host.Services.CreateScope();
            using ApplicationDbContext context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            User user = new("John Doe", 30);
            context.Users.Add(user);
            await context.SaveChangesAsync();

            return user.Id;
        }

        static async Task DeleteUser(IHost host, Guid userId)
        {
            using IServiceScope scope = host.Services.CreateScope();
            using ApplicationDbContext context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            User user = (await context.Users.FindAsync(userId))!;
            context.Users.Remove(user);
            await context.SaveChangesAsync();
        }

        static async Task QueryUser(IHost host, Guid userId)
        {
            using IServiceScope scope = host.Services.CreateScope();
            using ApplicationDbContext context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            User? user = await context.Users.FindAsync(userId);

            if (user == null) Console.WriteLine($"User with id {userId} has not found as expected.");
        }

        static async Task QueryUserForAudit(IHost host, Guid userId)
        {
            using IServiceScope scope = host.Services.CreateScope();
            using AuditReaderDbContext context = scope.ServiceProvider.GetRequiredService<AuditReaderDbContext>();

            User? user = await context.Users.FindAsync(userId);

            if (user != null) Console.WriteLine($"User with id {userId} has found as expected.");
        }

    }

}