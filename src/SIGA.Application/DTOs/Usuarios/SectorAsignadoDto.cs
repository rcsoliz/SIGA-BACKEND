namespace SIGA.Application.DTOs.Usuarios;

public record SectorAsignadoDto(Guid Id, string NombreSector, string? Zona);

public record CreateSectorAsignadoDto(string NombreSector, string? Zona);
