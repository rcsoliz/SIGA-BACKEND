using System.Globalization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace SIGA.WebApi.ModelBinding;

/// <summary>
/// El binder por defecto de ASP.NET Core parsea un DateTime de query string con
/// Kind=Unspecified, y Npgsql exige Kind=Utc para escribir en columnas timestamptz — sin
/// esto, cualquier filtro "desde"/"hasta" en un [FromQuery] DateTime revienta con
/// ArgumentException al llegar a la BD (ver AuditoriaController, DashboardController,
/// RegistrosController).
/// </summary>
public class UtcDateTimeModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        ArgumentNullException.ThrowIfNull(bindingContext);

        var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
        if (valueProviderResult == ValueProviderResult.None)
        {
            return Task.CompletedTask;
        }

        bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);

        var value = valueProviderResult.FirstValue;
        if (string.IsNullOrEmpty(value))
        {
            return Task.CompletedTask;
        }

        if (!DateTime.TryParse(
                value,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal,
                out var parsedUtc))
        {
            bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, $"'{value}' no es una fecha válida.");
            return Task.CompletedTask;
        }

        bindingContext.Result = ModelBindingResult.Success(parsedUtc);
        return Task.CompletedTask;
    }
}

public class UtcDateTimeModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var targetType = Nullable.GetUnderlyingType(context.Metadata.ModelType) ?? context.Metadata.ModelType;
        return targetType == typeof(DateTime) ? new UtcDateTimeModelBinder() : null;
    }
}
