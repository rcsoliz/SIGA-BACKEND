namespace SIGA.Domain.Enums;

/// <summary>
/// Estado de flujo de negocio de una captación (distinto de EstadoSync, que es técnico
/// y solo indica si el registro ya llegó al servidor).
/// </summary>
public enum EstadoCaptacion
{
    BorradorLocal,
    Registrado,
    EnPlanificacionFaena,
    Procesado
}
