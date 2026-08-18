namespace SIGA.Application.DTOs.Estancias;

public record UpdateEstanciaDto(
    string Nombre,
    string Propietario,
    string? Representante,
    string? Telefono,
    string? Renspa,
    double? HectareasTotales,
    string? Departamento,
    string? Provincia,
    string? Municipio);
