namespace SIGA.Application.DTOs.Alimentacion;

public record RegistroAlimentacionDto(
    Guid Id,
    Guid CaptacionGanadoId,
    DateTime Fecha,
    string TipoAlimentacion,
    double? RacionBaseKgAnimal,
    double? SuplementoProteicoKgAnimal,
    string? Observaciones,
    string EstadoSync);

public record CreateRegistroAlimentacionDto(
    Guid CaptacionGanadoId,
    DateTime Fecha,
    string TipoAlimentacion,
    double? RacionBaseKgAnimal,
    double? SuplementoProteicoKgAnimal,
    string? Observaciones,
    DateTime FechaCreacionLocal);
