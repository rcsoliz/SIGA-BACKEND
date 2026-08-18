using Microsoft.Extensions.DependencyInjection;
using SIGA.Application.Interfaces;
using SIGA.Application.Services;

namespace SIGA.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IAuditoriaService, AuditoriaService>();
        services.AddScoped<IEstanciaService, EstanciaService>();
        services.AddScoped<ICaptacionGanadoService, CaptacionGanadoService>();
        services.AddScoped<IRegistroAlimentacionService, RegistroAlimentacionService>();
        services.AddScoped<IMovimientoGanadoService, MovimientoGanadoService>();
        services.AddScoped<IRegistroSanitarioService, RegistroSanitarioService>();
        services.AddScoped<IRegistroPesajeService, RegistroPesajeService>();
        services.AddScoped<IUsuarioService, UsuarioService>();
        services.AddScoped<IRegistroCampoService, RegistroCampoService>();
        services.AddScoped<IDashboardService, DashboardService>();

        return services;
    }
}
