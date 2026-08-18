using SIGA.Application.Common.Exceptions;

namespace SIGA.Application.Common;

internal static class EnumParser
{
    public static TEnum Parse<TEnum>(string value, string campo) where TEnum : struct, Enum
    {
        if (!Enum.TryParse<TEnum>(value, ignoreCase: true, out var resultado) || !Enum.IsDefined(resultado))
        {
            throw new ValidationException(
                $"Valor '{value}' inválido para '{campo}'. Valores permitidos: {string.Join(", ", Enum.GetNames<TEnum>())}.");
        }

        return resultado;
    }
}
