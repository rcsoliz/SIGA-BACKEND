FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/SIGA.Domain/SIGA.Domain.csproj", "src/SIGA.Domain/"]
COPY ["src/SIGA.Application/SIGA.Application.csproj", "src/SIGA.Application/"]
COPY ["src/SIGA.Infrastructure/SIGA.Infrastructure.csproj", "src/SIGA.Infrastructure/"]
COPY ["src/SIGA.WebApi/SIGA.WebApi.csproj", "src/SIGA.WebApi/"]
RUN dotnet restore "src/SIGA.WebApi/SIGA.WebApi.csproj"

COPY src/ src/
WORKDIR /src/src/SIGA.WebApi
RUN dotnet publish "SIGA.WebApi.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

# Render inyecta $PORT en tiempo de ejecución; si no está presente (ej. docker run local) usa 8080.
ENTRYPOINT ["sh", "-c", "exec dotnet SIGA.WebApi.dll --urls http://+:${PORT:-8080}"]
