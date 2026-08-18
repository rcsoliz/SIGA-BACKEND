using SIGA.Application.DTOs.Registros;
using SIGA.Application.Interfaces;

namespace SIGA.Application.Services;

public class RegistroCampoService(
    IMovimientoGanadoRepository movimientoRepository,
    IRegistroAlimentacionRepository alimentacionRepository,
    IRegistroSanitarioRepository sanitarioRepository) : IRegistroCampoService
{
    public async Task<IReadOnlyList<RegistroCampoDto>> BuscarAsync(BuscarRegistrosQuery query, CancellationToken ct = default)
    {
        // Unifica las tres bitácoras en memoria. Con volumen alto de registros esto se
        // reemplaza por una vista SQL o una consulta UNION en el repositorio.
        var registros = new List<RegistroCampoDto>();

        if (query.Tipo is null or "Movimiento")
        {
            var movimientos = await movimientoRepository.GetAllAsync(ct);
            registros.AddRange(movimientos.Select(m => new RegistroCampoDto(
                m.Id,
                m.Fecha,
                "Movimiento",
                m.CaptacionGanadoId,
                m.CaptacionGanado?.Nombre ?? string.Empty,
                $"{m.CantidadCabezas} cabezas: {m.Origen} -> {m.Destino}",
                null,
                m.CaptacionGanado?.Estancia?.Captador?.Nombre ?? string.Empty,
                m.EstadoSync.ToString())));
        }

        if (query.Tipo is null or "Alimentacion")
        {
            var alimentaciones = await alimentacionRepository.GetAllAsync(ct);
            registros.AddRange(alimentaciones.Select(a => new RegistroCampoDto(
                a.Id,
                a.Fecha,
                "Alimentacion",
                a.CaptacionGanadoId,
                a.CaptacionGanado?.Nombre ?? string.Empty,
                $"{a.TipoAlimentacion}: {a.RacionBaseKgAnimal?.ToString("0.0") ?? "-"} kg/animal",
                null,
                a.CaptacionGanado?.Estancia?.Captador?.Nombre ?? string.Empty,
                a.EstadoSync.ToString())));
        }

        if (query.Tipo is null or "Sanitario")
        {
            var sanitarios = await sanitarioRepository.GetAllAsync(ct);
            registros.AddRange(sanitarios.Select(s => new RegistroCampoDto(
                s.Id,
                s.Fecha,
                "Sanitario",
                s.CaptacionGanadoId,
                s.CaptacionGanado?.Nombre ?? string.Empty,
                s.TipoEvento.ToString(),
                s.ProductoTratamiento,
                s.RegistradoPor?.Nombre ?? string.Empty,
                s.EstadoSync.ToString())));
        }

        var filtrados = registros.AsEnumerable();

        if (query.Desde is { } desde) filtrados = filtrados.Where(r => r.FechaHora >= desde);
        if (query.Hasta is { } hasta) filtrados = filtrados.Where(r => r.FechaHora <= hasta);
        if (query.CaptacionGanadoId is { } captacionId) filtrados = filtrados.Where(r => r.CaptacionGanadoId == captacionId);
        if (!string.IsNullOrWhiteSpace(query.Texto))
        {
            var texto = query.Texto.Trim();
            filtrados = filtrados.Where(r =>
                r.CaptacionNombre.Contains(texto, StringComparison.OrdinalIgnoreCase) ||
                r.DetalleMetrica.Contains(texto, StringComparison.OrdinalIgnoreCase) ||
                r.Id.ToString().Contains(texto, StringComparison.OrdinalIgnoreCase));
        }

        return filtrados.OrderByDescending(r => r.FechaHora).ToList();
    }
}
