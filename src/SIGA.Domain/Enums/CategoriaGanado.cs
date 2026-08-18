namespace SIGA.Domain.Enums;

/// <summary>
/// Clasificación del vacuno por sexo/edad, no por especie (este sistema es exclusivamente
/// para ganado vacuno, ver "Sistema de Registro y Captación de Ganado" en el documento base).
/// </summary>
public enum CategoriaGanado
{
    Toro,
    Novillo,
    Vaquilla,
    VacaDescarte,
    Ternero
}
