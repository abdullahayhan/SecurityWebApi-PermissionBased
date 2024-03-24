using Application.Services;
using Application.Services.Identity;
using Infrastructure.Context;
using Infrastructure.Services.Employee;
using Infrastructure.Services.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabase(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(
            options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")))
            .AddTransient<ApplicationDbSeeder>();

        return services;
    }

    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddTransient<ITokenService,TokenService>();
        services.AddTransient<IEmployeeService,EmployeeService>();
        return services;
    }
}
