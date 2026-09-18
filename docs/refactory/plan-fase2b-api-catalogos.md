# Plan: Fase 2b — API para catálogos + fix de email

Contexto para quien lo ejecute (Claude Code): las Fases 1-3 de la revisión de diseño de
BD (`docs/refactory/plan-completo-bd-siga.md`) solo tocaron `SIGA.Domain` y
`SIGA.Infrastructure` — entidades, `Configuration`, migraciones. Esta fase completa el
trabajo en `SIGA.Application`/`SIGA.WebApi` para que `SIGA-FRONTEND` pueda usar los
catálogos nuevos, y corrige un bug real de comparación de email que se vuelve visible
con el índice único agregado en la fase de índices.

**Regla de esta fase:** aditivo y hacia atrás compatible. Los campos string legado
(`Departamento`, `Provincia`, `Municipio`, `Raza`, `ProductoTratamiento`) se mantienen en
los DTOs tal cual están — se agregan los `*Id` nuevos al lado, no se reemplazan.

---

## 1. Nuevo `CatalogosController` (solo lectura)

### DTOs nuevos (`SIGA.Application/DTOs/Catalogos/`)

```csharp
public record DepartamentoDto(Guid Id, string Nombre);
public record ProvinciaDto(Guid Id, string Nombre, Guid DepartamentoId);
public record MunicipioDto(Guid Id, string Nombre, Guid ProvinciaId);
public record RazaDto(Guid Id, string Nombre);
public record ProductoTratamientoDto(Guid Id, string Nombre);
```

### Servicio (`ICatalogoService` + implementación en `SIGA.Application`/`SIGA.Infrastructure`)

Lectura simple contra `SigaDbContext`, sin lógica de negocio — `OrderBy(x => x.Nombre)` en
cada uno para que el frontend no tenga que ordenar.

```csharp
public interface ICatalogoService
{
    Task<IReadOnlyList<DepartamentoDto>> ListarDepartamentosAsync(CancellationToken ct);
    Task<IReadOnlyList<ProvinciaDto>> ListarProvinciasAsync(Guid departamentoId, CancellationToken ct);
    Task<IReadOnlyList<MunicipioDto>> ListarMunicipiosAsync(Guid provinciaId, CancellationToken ct);
    Task<IReadOnlyList<RazaDto>> ListarRazasAsync(CancellationToken ct);
    Task<IReadOnlyList<ProductoTratamientoDto>> ListarProductosTratamientoAsync(CancellationToken ct);
}
```

### Controller (`SIGA.WebApi/Controllers/CatalogosController.cs`)

```csharp
[ApiController]
[Route("api/catalogos")]
[Authorize] // mismo criterio de auth que el resto de los controllers — confirmar contra AuthController
public class CatalogosController(ICatalogoService catalogoService) : ControllerBase
{
    [HttpGet("departamentos")]
    public async Task<ActionResult<IReadOnlyList<DepartamentoDto>>> ListarDepartamentos(CancellationToken ct) =>
        Ok(await catalogoService.ListarDepartamentosAsync(ct));

    [HttpGet("departamentos/{id:guid}/provincias")]
    public async Task<ActionResult<IReadOnlyList<ProvinciaDto>>> ListarProvincias(Guid id, CancellationToken ct) =>
        Ok(await catalogoService.ListarProvinciasAsync(id, ct));

    [HttpGet("provincias/{id:guid}/municipios")]
    public async Task<ActionResult<IReadOnlyList<MunicipioDto>>> ListarMunicipios(Guid id, CancellationToken ct) =>
        Ok(await catalogoService.ListarMunicipiosAsync(id, ct));

    [HttpGet("razas")]
    public async Task<ActionResult<IReadOnlyList<RazaDto>>> ListarRazas(CancellationToken ct) =>
        Ok(await catalogoService.ListarRazasAsync(ct));

    [HttpGet("productos-tratamiento")]
    public async Task<ActionResult<IReadOnlyList<ProductoTratamientoDto>>> ListarProductos(CancellationToken ct) =>
        Ok(await catalogoService.ListarProductosTratamientoAsync(ct));
}
```

---

## 2. Estancias — sumar los `*Id` a los DTOs existentes

`SIGA.Application/DTOs/Estancias/`:

- `EstanciaDto`: agregar `Guid? DepartamentoId, Guid? ProvinciaId, Guid? MunicipioId` al final
  del record (no reordenar los campos existentes — el frontend deserializa por nombre, pero
  mejor no arriesgar).
- `CreateEstanciaDto`: mismo agregado.
- `UpdateEstanciaDto`: mismo agregado.

`EstanciaService` (`CrearAsync`/`ActualizarAsync`): mapear los `*Id` nuevos al crear/editar
la entidad, junto a los campos string existentes (ambos se guardan, sin reemplazarse).

---

## 3. Detalle de lote — sumar `RazaId`

`SIGA.Application/DTOs/Captaciones/DetalleLoteGanadoDto.cs`:
- `DetalleLoteGanadoDto`: agregar `Guid? RazaId`.
- `CreateDetalleLoteGanadoDto`: agregar `Guid? RazaId`.

El método del `CaptacionesController`/servicio que maneja
`POST /api/captaciones/{id}/detalles` necesita mapear `RazaId` al crear el
`DetalleLoteGanado`.

---

## 4. Registro sanitario — sumar `ProductoTratamientoId`

`SIGA.Application/DTOs/Sanitario/RegistroSanitarioDto.cs`:
- `RegistroSanitarioDto`: agregar `Guid? ProductoTratamientoId`.
- `CreateRegistroSanitarioDto`: agregar `Guid? ProductoTratamientoId`.

Mapear en el servicio correspondiente al crear el registro.

---

## 5. Fix: comparación de email case-insensitive

**Por qué va en esta fase y no antes:** el índice único `lower(Email)` (ya aplicado) hace
que un email que solo difiere en mayúsculas falle recién al insertar en la BD — y esa
excepción no está mapeada en `ExceptionHandlingMiddleware`, así que hoy sale como 500
genérico en vez de un 409 claro. Este fix cierra ese hueco.

`SIGA.Infrastructure/Repositories/UsuarioRepository.cs`:

```csharp
// Antes:
await DbSet.FirstOrDefaultAsync(u => u.Email == email, ct);
await DbSet.AnyAsync(u => u.Email == email, ct);

// Después — comparación case-insensitive, consistente con el índice de BD:
await DbSet.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower(), ct);
await DbSet.AnyAsync(u => u.Email.ToLower() == email.ToLower(), ct);
```

Con esto, `ExistsByEmailAsync` detecta el duplicado *antes* de llegar a la BD y sigue
lanzando el `ConflictException` (409) que ya existe en `UsuarioService.CrearAsync` — no
hace falta tocar el middleware. Y el login (`AuthService` vía `GetByEmailAsync`) empieza a
aceptar el email sin importar cómo lo haya escrito el usuario.

**Nota:** `u.Email.ToLower()` se traduce a `lower("Email")` en SQL vía Npgsql — usa el
índice `IX_Usuarios_Email_Lower` que ya existe, no hace falta un índice adicional.

---

## 6. Frontend (`SIGA-FRONTEND`) — una vez el backend esté desplegado

- `src/api/`: nuevo `catalogos.ts` con las 5 llamadas GET.
- `src/types/dto.ts`: agregar `DepartamentoDto`, `ProvinciaDto`, `MunicipioDto`, `RazaDto`,
  `ProductoTratamientoDto`, y sumar los campos `*Id` opcionales a `EstanciaDto` /
  `CreateEstanciaDto` / `UpdateEstanciaDto` / `DetalleLoteGanadoDto` /
  `CreateDetalleLoteGanadoDto` / `RegistroSanitarioDto` / `CreateRegistroSanitarioDto`
  (deben quedar idénticos a los DTOs de C# de arriba).
- `EstanciaFormView.vue`: reemplazar los 3 inputs de texto libre (Departamento, Provincia,
  Municipio) por 3 `select` en cascada — al elegir Departamento se habilita Provincia
  (fetch a `/catalogos/departamentos/{id}/provincias`), al elegir Provincia se habilita
  Municipio. Mantener el valor legado de texto libre no es necesario mostrarlo en el form,
  pero si se quiere compatibilidad con estancias viejas sin catálogo asignado, mostrar el
  string legado como texto de solo lectura cuando `DepartamentoId` es `null`.
- Formulario de captación (donde se agrega `DetalleLoteGanado`): el campo Raza pasa de
  texto libre a `select` alimentado por `/catalogos/razas`.
- Formulario de registro sanitario: el campo Producto pasa a `select` alimentado por
  `/catalogos/productos-tratamiento`.

No se listan archivos exactos del frontend más allá de `EstanciaFormView.vue` porque no se
revisaron los formularios de captación/sanitario en detalle — confirmar la ruta exacta
antes de editar.

---

## Fuera de alcance de esta fase (mencionado por completitud, no implementar aquí)

- Endpoint de búsqueda geoespacial (`GET /api/estancias/cercanas`) usando la columna
  `Ubicacion` de PostGIS — no hay ningún caso de uso pedido todavía, se agrega cuando haga
  falta.
- Manejo de la excepción de índice único de `Dispositivos.IdentificadorDispositivo` en el
  middleware — no hay ningún endpoint que cree un `Dispositivo` todavía, así que no hay
  código que dispare ese error hoy. Atender esto cuando se implemente el registro de
  dispositivos.
