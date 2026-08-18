namespace SIGA.Application.DTOs.Estancias;

public record EstanciaDto(
    Guid Id,
    string Nombre,
    string Propietario,
    string? Representante,
    string? Telefono,
    double Latitud,
    double Longitud,
    string? Renspa,
    double? HectareasTotales,
    string? Departamento,
    string? Provincia,
    string? Municipio,
    int CantidadCaptaciones,
    string EstadoSync);
