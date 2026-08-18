using System.Net;
using System.Text.Json;
using SIGA.Application.Common.Exceptions;

namespace SIGA.WebApi.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            var (status, titulo) = ex switch
            {
                NotFoundException => (HttpStatusCode.NotFound, "Recurso no encontrado"),
                ValidationException => (HttpStatusCode.BadRequest, "Solicitud inválida"),
                ConflictException => (HttpStatusCode.Conflict, "Conflicto"),
                UnauthorizedException => (HttpStatusCode.Unauthorized, "No autorizado"),
                _ => (HttpStatusCode.InternalServerError, "Error interno del servidor")
            };

            if (status == HttpStatusCode.InternalServerError)
            {
                logger.LogError(ex, "Error no controlado procesando {Path}", context.Request.Path);
            }

            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = (int)status;

            var problema = new
            {
                title = titulo,
                status = (int)status,
                detail = status == HttpStatusCode.InternalServerError ? "Ocurrió un error inesperado." : ex.Message
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(problema));
        }
    }
}
