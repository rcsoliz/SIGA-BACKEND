using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SIGA.Application.Interfaces;
using SIGA.Infrastructure.Persistence;
using SIGA.Infrastructure.Repositories;
using SIGA.Infrastructure.Security;

namespace SIGA.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<SigaDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("SigaDatabase")));

        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IEstanciaRepository, EstanciaRepository>();
        services.AddScoped<ICaptacionGanadoRepository, CaptacionGanadoRepository>();
        services.AddScoped<IRegistroAlimentacionRepository, RegistroAlimentacionRepository>();
        services.AddScoped<IMovimientoGanadoRepository, MovimientoGanadoRepository>();
        services.AddScoped<IRegistroSanitarioRepository, RegistroSanitarioRepository>();
        services.AddScoped<ILogAuditoriaRepository, LogAuditoriaRepository>();

        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        return services;
    }
}
