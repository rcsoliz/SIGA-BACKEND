# Plan: mejoras de diseño de BD — SIGA-BACKEND

Contexto para quien lo ejecute (Claude Code): este plan viene de una revisión de diseño
de base de datos sobre `github.com/rcsoliz/SIGA-BACKEND` (.NET 8 / EF Core 8.0.10 /
Npgsql / Clean Architecture: `SIGA.Domain`, `SIGA.Application`, `SIGA.Infrastructure`,
`SIGA.WebApi`). DbContext: `SigaDbContext` en `SIGA.Infrastructure/Persistence/`.

**Reglas del proyecto a respetar en las 4 fases:**
- Cambios aditivos y hacia atrás compatibles — no se borran columnas ni tablas existentes
  salvo que se diga explícitamente.
- Cada fase es un PR separado, revisable de forma independiente. No mezclar fases.
- Cada fase termina con `dotnet ef migrations add <Nombre>` corrido localmente (no a
  mano) para que `Designer.cs` y `SigaDbContextModelSnapshot.cs` queden generados
  correctamente por la herramienta real.
- Después de cada `dotnet ef migrations add`, correr `dotnet ef migrations script` (o
  levantar la app localmente, que ya llama a `Database.MigrateAsync()` en
  `DbInitializer.SeedAsync`) para confirmar que aplica limpio sobre una BD de dev.

---

## Fase 1 — FKs de trazabilidad + CHECK constraints

**Estado: ya implementada**, en la rama `feature/db-fase1-fks-checks` / patch adjunto en
la conversación anterior. Falta solo:

1. Aplicar el patch (`git am 0001-fase1-fks-checks.patch`) o copiar los archivos ya
   editados.
2. Correr `dotnet ef migrations add AgregarTrazabilidadFksYValidaciones` — la migración
   que dejé la escribí a mano (sin acceso a NuGet en mi entorno para compilar), así que
   esto la reemplaza por la real generada por la herramienta, o confirma que coincide.
3. Verificar que el seed (`DbInitializer.cs`) sigue corriendo limpio — ya corregí ahí el
   bug de `CreadoPorUsuarioId` no seteado en `Estancia`/`CaptacionGanado`.

Archivos ya tocados: las 11 clases en `SIGA.Infrastructure/Persistence/Configurations/`,
`DbInitializer.cs`, y la migración nueva.

---

## Fase 2 — Catálogos de ubicación, raza y producto

**Nota importante:** en la sesión genérica anterior (antes de ver el código real) había
propuesto también un modelo de catálogo `Roles`/`Permisos`/`RolPermisos`. Al revisar
`SIGA.Domain.Enums` vi que **ya existen `RolUsuario` y `TipoPermiso` como enums
tipados**, y `PermisoUsuario` ya es una tabla puente Usuario↔TipoPermiso. Ese catálogo
ya no aplica — sería redundante y perdería la seguridad de tipos que ya tienes. Se
descarta esa parte del plan original.

Lo que sí sigue teniendo sentido es normalizar **datos de negocio abiertos** (no un
enum cerrado de la app): departamento/provincia/municipio, raza, producto de
tratamiento. Hoy son `string?` libres en `Estancia`, `DetalleLoteGanado` y
`RegistroSanitario`, sin ningún catálogo detrás.

### 2.1 — Nuevas entidades (en `SIGA.Domain/Entities/`)

Mantener la convención existente: heredan de `BaseEntity` (Guid Id), no de un PK
`smallint` — para no romper el patrón del resto del dominio.

```csharp
// Departamento.cs
public class Departamento : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public ICollection<Provincia> Provincias { get; set; } = new List<Provincia>();
}

// Provincia.cs
public class Provincia : BaseEntity
{
    public Guid DepartamentoId { get; set; }
    public Departamento Departamento { get; set; } = null!;
    public string Nombre { get; set; } = string.Empty;
    public ICollection<Municipio> Municipios { get; set; } = new List<Municipio>();
}

// Municipio.cs
public class Municipio : BaseEntity
{
    public Guid ProvinciaId { get; set; }
    public Provincia Provincia { get; set; } = null!;
    public string Nombre { get; set; } = string.Empty;
}

// Raza.cs
public class Raza : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
}

// ProductoTratamiento.cs
public class ProductoTratamiento : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
}
```

### 2.2 — Cambios en entidades existentes (nullable, no rompen nada)

- `Estancia`: agregar `public Guid? DepartamentoId { get; set; }`,
  `public Departamento? Departamento { get; set; }` (y lo mismo para Provincia/Municipio).
  **Ojo con el nombre**: ya existe una propiedad `string? Departamento` en `Estancia` —
  no se puede tener propiedad de navegación y propiedad string con el mismo nombre.
  Usar `DepartamentoRef`/`ProvinciaRef`/`MunicipioRef` para las navegaciones, o renombrar
  las propiedades de navegación como `CatalogoDepartamento`, etc. — decidir un nombre y
  ser consistente. Las columnas string existentes (`Departamento`, `Provincia`,
  `Municipio`) se quedan intactas como respaldo/legado.
- `DetalleLoteGanado`: agregar `public Guid? RazaId { get; set; }` +
  `public Raza? RazaCatalogo { get; set; }` (mismo problema de nombre: ya existe
  `string? Raza`).
- `RegistroSanitario`: agregar `public Guid? ProductoTratamientoId { get; set; }` +
  navegación (ya existe `string? ProductoTratamiento`, mismo cuidado con el nombre).

### 2.3 — Nuevas Configuration classes

`DepartamentoConfiguration`, `ProvinciaConfiguration`, `MunicipioConfiguration`,
`RazaConfiguration`, `ProductoTratamientoConfiguration` — seguir el patrón de las
existentes (`ToTable`, `HasMaxLength`, índice único en `Nombre` con scope al padre donde
aplique: `UNIQUE(DepartamentoId, Nombre)` en Provincia, `UNIQUE(ProvinciaId, Nombre)` en
Municipio).

Actualizar `EstanciaConfiguration`, `DetalleLoteGanadoConfiguration`,
`RegistroSanitarioConfiguration` con las nuevas relaciones `HasOne(...).WithMany(...)`
(FK nullable, `OnDelete(DeleteBehavior.Restrict)` para no perder la trazabilidad si se
borra una fila de catálogo con datos ya asignados).

### 2.4 — DbSets nuevos en `SigaDbContext`

```csharp
public DbSet<Departamento> Departamentos => Set<Departamento>();
public DbSet<Provincia> Provincias => Set<Provincia>();
public DbSet<Municipio> Municipios => Set<Municipio>();
public DbSet<Raza> Razas => Set<Raza>();
public DbSet<ProductoTratamiento> ProductosTratamiento => Set<ProductoTratamiento>();
```

### 2.5 — Seed en `DbInitializer.cs`

Sembrar **solo** los valores que ya aparecen en los datos de dev actuales — no inventar
la división político-administrativa completa de Bolivia sin una fuente verificada:
- Departamento: Santa Cruz
- Provincias: Warnes, Andrés Ibáñez
- Municipios: Warnes, Santa Cruz de la Sierra
- Razas: Nelore, Brangus, Brahman, Cruza Comercial (o las que uses en el seed actual)
- Producto: Vacuna Aftosa

Y opcionalmente, en el mismo seed, asignar `DepartamentoId`/`ProvinciaId`/`MunicipioId` /
`RazaId` / `ProductoTratamientoId` a las filas de ejemplo existentes (`estanciaElVergel`,
etc.) para que el catálogo se vea usado desde el día uno.

### 2.6 — Migración

`dotnet ef migrations add AgregarCatalogosUbicacionRazaProducto`. Debería generar
`CreateTable` para las 5 tablas nuevas + `AddColumn` (nullable) en las 3 tablas
existentes + FKs + índices únicos.

**Nota de seguimiento (no bloqueante para este PR):** una vez mergeado, correr en dev/staging
una consulta para ver qué estancias/detalles/registros tienen texto libre que no matchea
ningún catálogo (`Departamento IS NOT NULL AND DepartamentoId IS NULL`, etc.) y decidir
caso por caso si se corrigen a mano.

---

## Fase 3 — PostGIS para coordenadas

**Alcance acotado a propósito:** esto es solo a nivel de base de datos, sin tocar el
modelo de EF ni agregar el paquete `Npgsql.EntityFrameworkCore.PostgreSQL.NetTopologySuite`
todavía. Razón: agregar `NetTopologySuite.Geometries.Point` como propiedad de entidad es
un cambio más invasivo (nueva dependencia, cambia el tipo de columna que ya usan
`Latitud`/`Longitud` en varios DTOs y servicios). Se deja como fase futura opcional si
en algún momento necesitas hacer consultas espaciales reales (`ST_DWithin`, etc.) desde
LINQ.

Lo que esta fase sí hace:
1. `CREATE EXTENSION IF NOT EXISTS postgis;`
2. Columna `geography(Point,4326)` en `Estancias` y `CaptacionesGanado`, calculada
   automáticamente por un trigger de Postgres a partir de `Latitud`/`Longitud` — la app
   .NET sigue escribiendo `Latitud`/`Longitud` exactamente igual que hoy, sin cambios de
   código.
3. Índice GiST sobre esa columna.
4. `CHECK` de rango en `Latitud` (-90 a 90) y `Longitud` (-180 a 180) en ambas tablas.

Como EF Core no puede generar triggers ni `geography` vía Fluent API, esta migración se
escribe con `migrationBuilder.Sql(...)` (no hay Fluent API que generar — no hace falta
tocar ninguna `*Configuration.cs` para esta fase). Igual hay que crear la migración con
`dotnet ef migrations add AgregarPostgisCoordenadas --output-dir Persistence/Migrations`
y meter el SQL a mano dentro de `Up()`/`Down()` (esto es normal en EF Core, no todo tiene
que salir de Fluent API).

Referencia del SQL exacto (trigger + columna + índice) está en el script
`03_postgis_coordenadas.sql` que ya te pasé antes en la conversación — se puede pegar
casi tal cual dentro de `migrationBuilder.Sql(@"...")`.

---

## Fase 4 — Particionar `LogsAuditoria` por mes

**Advertencia de diseño a tener en cuenta antes de implementar:** particionar por RANGE
en Postgres obliga a que la `PRIMARY KEY` incluya la columna de partición — la PK pasa
de `(Id)` a `(Id, FechaHora)`. El modelo de EF (`LogAuditoria : BaseEntity`) sigue
pensando que la PK es solo `Id`; en la práctica esto funciona porque `LogAuditoria` es
*append-only* (solo `Add` + `SaveChanges`, nunca `Update`/lookup por `Id` desde EF), pero
es un desajuste real entre lo que EF cree y lo que la BD tiene. Si en algún momento se
necesita hacer `context.LogsAuditoria.Find(id)` o similar, dejaría de funcionar como se
espera. Vale la pena que Claude Code confirme que no hay ningún uso de ese tipo en
`SIGA.Application`/`SIGA.WebApi` antes de aplicar esta fase (`grep -rn "LogsAuditoria\." src/SIGA.Application src/SIGA.WebApi`).

Como con la Fase 3, EF Core no modela particionamiento declarativo — todo va en
`migrationBuilder.Sql(...)` dentro de una migración (`dotnet ef migrations add
ParticionarLogsAuditoriaPorMes`), sin cambios en `LogAuditoriaConfiguration.cs`.

Pasos (detalle completo ya armado en `04_particionar_logs_auditoria.sql`, pasado antes en
la conversación — se puede adaptar directo):
1. Renombrar tabla actual a `LogsAuditoria_legacy` (respaldo).
2. Crear `LogsAuditoria` particionada por `RANGE (FechaHora)`, con PK `(Id, FechaHora)`.
3. Generar particiones mensuales (rango amplio, p.ej. 2026-01 a 2027-12) + partición
   `DEFAULT` para lo que caiga fuera de rango.
4. Copiar los datos existentes.
5. Recrear índices (`UsuarioId`, `FechaHora`) y la FK hacia `Usuarios`.
6. Verificar conteo de filas antes de borrar `LogsAuditoria_legacy`.
7. Función de mantenimiento `crear_particion_logs_auditoria_siguiente_mes()` +
   recomendación de programarla mensualmente (pg_cron, cron del servidor, o un job del
   propio backend).

**Este es el único de los 4 que reemplaza una tabla en vez de solo agregar cosas** —
aplicarlo primero en un ambiente de dev/staging y confirmar antes de tocar producción.

---

## Orden sugerido de PRs

1. Fase 1 (ya lista, solo falta el `dotnet ef migrations add` real) — mergear primero.
2. Fase 2 — catálogos.
3. Fase 3 — PostGIS (independiente de la 2, se puede hacer en paralelo si hace falta).
4. Fase 4 — particionamiento (la más delicada, dejarla para el final y probarla bien en
   staging antes de aplicar en producción).
