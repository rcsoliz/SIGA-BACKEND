using SIGA.Application.Common.Exceptions;
using SIGA.Application.DTOs.Dashboard;
using SIGA.Application.Interfaces;
using SIGA.Domain.Entities;
using SIGA.Domain.Enums;

namespace SIGA.Application.Services;

public class DashboardService(
    ICaptacionGanadoRepository captacionRepository,
    IRegistroAlimentacionRepository alimentacionRepository,
    IRegistroPesajeRepository pesajeRepository,
    IUsuarioRepository usuarioRepository,
    IEstanciaRepository estanciaRepository,
    ILogAuditoriaRepository logAuditoriaRepository,
    ICurrentUserService currentUserService) : IDashboardService
{
    private const int MesesHistorial = 6;

    public async Task<DashboardDto> ObtenerResumenAsync(CancellationToken ct = default)
    {
        var captaciones = await captacionRepository.GetAllAsync(ct);
        var activas = captaciones.Where(c => c.Estado != EstadoCaptacion.Procesado).ToList();

        var totalCabezasActivas = activas.Sum(c => c.CalcularTotalCabezas());
        var captacionesActivas = activas.Count;
        var pendientesRevision = captaciones.Count(c => c.Estado == EstadoCaptacion.Registrado);

        var cabezasPorCategoria = activas
            .SelectMany(c => c.Detalles)
            .GroupBy(d => d.Categoria)
            .Select(g => new CabezasPorCategoriaDto(g.Key.ToString(), g.Sum(d => d.CantidadCabezas)))
            .OrderByDescending(c => c.Cantidad)
            .ToList();

        var alimentaciones = await alimentacionRepository.GetAllAsync(ct);
        var pesajes = await pesajeRepository.GetAllAsync(ct);

        var desde = DateTime.UtcNow.Date.AddMonths(-(MesesHistorial - 1));
        var meses = Enumerable.Range(0, MesesHistorial)
            .Select(offset => new DateTime(desde.Year, desde.Month, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(offset))
            .ToList();

        var consumoPorMes = alimentaciones
            .Where(a => a.RacionBaseKgAnimal is not null)
            .GroupBy(a => new DateTime(a.Fecha.Year, a.Fecha.Month, 1))
            .ToDictionary(g => g.Key, g => g.Average(a => a.RacionBaseKgAnimal!.Value));

        var pesoPorMes = pesajes
            .GroupBy(p => new DateTime(p.Fecha.Year, p.Fecha.Month, 1))
            .ToDictionary(g => g.Key, g => g.Average(p => p.PesoPromedioKg));

        var serieMensual = meses.Select(mes => new DashboardSerieMensualDto(
            mes.ToString("yyyy-MM"),
            consumoPorMes.TryGetValue(mes, out var consumo) ? consumo : null,
            pesoPorMes.TryGetValue(mes, out var peso) ? peso : null)).ToList();

        return new DashboardDto(totalCabezasActivas, captacionesActivas, pendientesRevision, cabezasPorCategoria, serieMensual);
    }

    public async Task<IReadOnlyList<CaptadorRankingDto>> ListarProductividadCaptadoresAsync(
        DateTime? desde, DateTime? hasta, CancellationToken ct = default)
    {
        var usuarios = await usuarioRepository.GetAllAsync(ct);
        var captadores = usuarios.Where(u => u.Rol == RolUsuario.Captador).ToList();

        var ranking = new List<CaptadorRankingDto>();
        foreach (var captador in captadores)
        {
            var estancias = await estanciaRepository.GetByCaptadorAsync(captador.Id, ct);
            var (estanciasReg, captacionesReg, captacionesActivas, totalCabezas) = CalcularEstadisticas(estancias, desde, hasta);

            ranking.Add(new CaptadorRankingDto(
                captador.Id, captador.Nombre, captador.Cargo,
                estanciasReg, captacionesReg, captacionesActivas, totalCabezas));
        }

        return ranking.OrderByDescending(r => r.TotalCabezasCapturadas).ToList();
    }

    public async Task<CaptadorProductividadDto> ObtenerProductividadCaptadorAsync(
        Guid captadorId, DateTime? desde, DateTime? hasta, CancellationToken ct = default)
    {
        if (currentUserService.Rol == RolUsuario.Captador.ToString() && currentUserService.UsuarioId != captadorId)
        {
            throw new UnauthorizedException("Un captador solo puede consultar su propia productividad.");
        }

        var captador = await usuarioRepository.GetConDetalleAsync(captadorId, ct)
            ?? throw new NotFoundException(nameof(Usuario), captadorId);

        if (captador.Rol != RolUsuario.Captador)
        {
            throw new ValidationException($"El usuario '{captador.Nombre}' no tiene rol Captador.");
        }

        var estancias = await estanciaRepository.GetByCaptadorAsync(captadorId, ct);
        var (estanciasReg, captacionesReg, captacionesActivas, totalCabezas) = CalcularEstadisticas(estancias, desde, hasta);

        var logs = await logAuditoriaRepository.BuscarAsync(null, null, captadorId, null, ct);
        var ultimaActividad = logs.Count > 0 ? logs[0].FechaHora : (DateTime?)null;

        return new CaptadorProductividadDto(
            captador.Id,
            captador.Nombre,
            captador.Cargo,
            captador.Estado.ToString(),
            captador.SectoresAsignados.Select(s => s.NombreSector).ToList(),
            estanciasReg,
            captacionesReg,
            captacionesActivas,
            totalCabezas,
            ultimaActividad);
    }

    private static (int EstanciasRegistradas, int CaptacionesRegistradas, int CaptacionesActivas, int TotalCabezas)
        CalcularEstadisticas(IReadOnlyList<Estancia> estancias, DateTime? desde, DateTime? hasta)
    {
        var captaciones = estancias.SelectMany(e => e.Captaciones).AsEnumerable();

        if (desde is { } fDesde) captaciones = captaciones.Where(c => c.Fecha >= fDesde);
        if (hasta is { } fHasta) captaciones = captaciones.Where(c => c.Fecha <= fHasta);

        var captacionesEnRango = captaciones.ToList();

        var estanciasConActividad = desde is null && hasta is null
            ? estancias.Count
            : estancias.Count(e => e.Captaciones.Any(c => captacionesEnRango.Contains(c)));

        return (
            estanciasConActividad,
            captacionesEnRango.Count,
            captacionesEnRango.Count(c => c.Estado != EstadoCaptacion.Procesado),
            captacionesEnRango.Sum(c => c.CalcularTotalCabezas()));
    }
}
