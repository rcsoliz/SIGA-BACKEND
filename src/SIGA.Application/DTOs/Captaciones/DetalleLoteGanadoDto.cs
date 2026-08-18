namespace SIGA.Application.DTOs.Captaciones;

public record DetalleLoteGanadoDto(
    Guid Id,
    string Categoria,
    string? Raza,
    int CantidadCabezas,
    double? PesoPromedioEstimadoKg,
    string SistemaAlimentacion,
    DateTime? FechaEstimadaFaena,
    string? NotasZootecnicas,
    double PesoLoteCalculado,
    int? DiasRestantesFaena);

public record CreateDetalleLoteGanadoDto(
    string Categoria,
    string? Raza,
    int CantidadCabezas,
    double? PesoPromedioEstimadoKg,
    string SistemaAlimentacion,
    DateTime? FechaEstimadaFaena,
    string? NotasZootecnicas);
