namespace SIGA.Application.DTOs.Estancias;

public record CreateEstanciaDto(
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
    DateTime FechaCreacionLocal);
