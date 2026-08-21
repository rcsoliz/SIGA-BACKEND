# SIGA — Especificación Técnica de Integración Frontend ↔ Backend

**Versión:** 1.0 · **Fecha:** 2026-08-19
**Alcance:** Vincular cada pantalla ya diseñada en Stitch ("SIGA DESIGN") con su
contrato de API real en el backend .NET 8, para que el frontend pueda implementarse
con datos 100% dinámicos sin adivinar nombres de campos, sin romper el diseño visual
ya aprobado, y de forma iterativa por etapas.

Este documento asume que el lector ya tiene acceso a:
- El backend corriendo en local (ver sección 2).
- El proyecto "SIGA DESIGN" en Stitch, con las pantallas finales (sufijo "SIGA Final" /
  "ES Final v2" — ver advertencia en sección 1.2 sobre pantallas descartadas).

---

## 1. Introducción

### 1.1 Principio rector

El diseño manda en lo visual, el backend manda en los datos. El frontend traduce el
HTML/Tailwind de Stitch a componentes reales, y sustituye cada dato quemado
("Hacienda El Vergel", "450 Cabezas") por un binding a la respuesta real de la API
correspondiente — sin alterar clases de Tailwind, jerarquía visual, ni breakpoints
responsive ya definidos.

### 1.2 Pantallas descartadas — NO usar como referencia

Durante el ciclo de diseño quedaron sueltas versiones antiguas en el mismo proyecto de
Stitch. **Usa únicamente las pantallas con sufijo "SIGA Final", "ES Final" o "ES Final
v2"** (la versión más reciente de cada nombre). Están descartadas y no deben
implementarse:

- "Reporte de Lote" (reemplazada por "Reporte de Captación")
- "Reportes de Lote" / "Reportes de Desempeño" (alcance distinto, no está en el plan)
- "Mapa de Lotes" (reemplazada por "Mapa de Operaciones")
- "Registro de Alimentación (Escritorio) - ES" sin sufijo (tenía campos inventados)
- Duplicados sin sufijo de "Maestro de Registros", "Dashboard de Gestión", "Seguridad
  y Usuarios"

**Fuera de alcance (sin backend, no implementar todavía):** "Recuperar Contraseña" (no
existe endpoint de recuperación de contraseña en el backend), y las pantallas de
Sincronización / Configuración del Sistema / Perfil de Usuario propio — no tienen
controlador confirmado, ver sección 7.

---

## 2. Arquitectura y Convenciones Generales

### 2.1 Conexión

```
API base (HTTP):   http://localhost:5095/api
API base (HTTPS):  https://localhost:7084/api
Swagger:           http://localhost:5095/swagger
```

CORS ya está habilitado para `http://localhost:5173` (puerto por defecto de Vite) en
`appsettings.Development.json` → `Cors:OrigenesPermitidos`. Si el frontend corre en
otro puerto, hay que agregar ese origen ahí antes de poder hacer requests desde el
navegador.

Para levantar el backend localmente: Docker Desktop corriendo → `docker compose up -d`
(contenedor `siga-postgres`, puerto 5434) → `dotnet run --project src/SIGA.WebApi` (el
seed de datos de prueba corre automáticamente en Development, ver sección 3).

### 2.2 Autenticación

`POST /api/auth/login` es el único endpoint de autenticación. No hay registro público
ni recuperación de contraseña — las cuentas las crea un Administrador desde "Seguridad
y Usuarios".

**Request:**
```json
{ "email": "admin@siga.com", "password": "Admin123!" }
```

**Response 200:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "expiraEnUtc": "2026-08-19T20:00:00Z",
  "usuarioId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "nombre": "Admin SIGA",
  "rol": "Administrador"
}
```

Guardar `token` y adjuntarlo como `Authorization: Bearer {token}` en todas las
llamadas subsiguientes. `expiraEnUtc` sirve para saber cuándo forzar un nuevo login
(no hay refresh token — al expirar, el usuario debe volver a "Login"). `rol` es
`"Administrador"` o `"Captador"`: úsalo para las rutas/menús protegidos del frontend,
en espejo de los `[Authorize(Roles = "...")]` del backend.

### 2.3 Formato de datos

- Los nombres de propiedades en el JSON de respuesta son **camelCase**
  (comportamiento por defecto de ASP.NET Core / System.Text.Json — `EstanciaId` en el
  DTO de C# llega como `estanciaId` en el JSON).
- Los **valores de enum llegan como string en PascalCase tal cual el nombre en C#**
  (ej. `"BorradorLocal"`, `"VacaDescarte"`, `"SemiConfinamiento"`), NO en minúsculas ni
  con espacios. Ver la tabla de enums en la sección 2.5 para mapear cada valor a la
  etiqueta en español que ya usan las pantallas de Stitch.
- Las fechas son ISO 8601 UTC (`DateTime`/`DateTime?` de C#).
- Los campos opcionales del backend (`string?`, `double?`, `int?`) pueden llegar como
  `null` — cualquier componente que muestre esos campos (Representante, Teléfono,
  Observaciones, etc.) debe tener un estado por defecto (guion "—" o el componente
  colapsado), igual que se hizo en los mockups de Stitch.

### 2.4 Contrato de errores

Todas las respuestas de error usan `application/problem+json` con esta forma fija
(ver `ExceptionHandlingMiddleware.cs`):

```json
{
  "title": "Solicitud inválida",
  "status": 400,
  "detail": "El campo Nombre es obligatorio."
}
```

| status | title (fijo)              | Cuándo ocurre |
|--------|----------------------------|---------------|
| 400    | Solicitud inválida         | Validación de negocio fallida (`ValidationException`) |
| 401    | No autorizado              | Token ausente/expirado, o acción no permitida para el rol/usuario actual |
| 404    | Recurso no encontrado      | El `id` de la URL no existe |
| 409    | Conflicto                  | Conflicto de estado (`ConflictException`) |
| 500    | Error interno del servidor | `detail` siempre es genérico ("Ocurrió un error inesperado.") — no expone detalles internos |

El frontend debe mostrar `detail` directamente al usuario en los casos 400/401/404/409
(ya viene en español y listo para mostrar), y un mensaje genérico propio para 500.

### 2.5 Tabla de enums (valor backend → etiqueta de Stitch)

| Enum | Valores backend (string exacto) | Etiqueta en Stitch |
|---|---|---|
| `CategoriaGanado` | `Toro`, `Novillo`, `Vaquilla`, `VacaDescarte`, `Ternero` | Toro, Novillo, Vaquilla, **Vaca Descarte**, Ternero |
| `TipoManejoAlimentario` | `PastoreoLibre`, `SemiConfinamiento`, `Confinamiento` | **Pastoreo Libre**, **Semi-Confinamiento**, Confinamiento |
| `EstadoCaptacion` | `BorradorLocal`, `Registrado`, `EnPlanificacionFaena`, `Procesado` | Borrador Local, Registrado, En Planificación de Faena, Procesado |
| `EstadoSanitario` | `Optimo`, `EnObservacion`, `Critico` | Óptimo, En Observación, Crítico |
| `EstadoSync` | `Pendiente`, `Sincronizado`, `Error` | Pendiente, Sincronizado, Error |
| `EstadoUsuario` | `Activo`, `Pendiente`, `Suspendido` | Activo, Pendiente, Suspendido |
| `RolUsuario` | `Administrador`, `Captador` | Administrador, Captador |
| `TipoEventoSanitario` | `Vacunacion`, `Antiparasitario`, `ControlRutina`, `Tratamiento` | Vacunación, Antiparasitario, Control de Rutina, Tratamiento |
| `AccionAuditoria` | `Creacion`, `Modificacion`, `Eliminacion` | Creación, Modificación, Eliminación |
| `TipoPermiso` | `EntradaDatos`, `RegistrosHistoricos`, `ConfiguracionSistema` | Entrada de Datos, Registros Históricos, Configuración del Sistema |
| `EstadoDispositivo` | `Activo`, `Revocado` | Activo, Revocado |
| Módulos de Auditoría (string libre, no enum) | `Usuario`, `Estancia`, `CaptacionGanado`, `Pesaje`, `Sanitario`, `Movimiento`, `Alimentacion` | (mismo texto, mostrar tal cual) |

**Importante:** el frontend NO debe traducir/formatear estos valores adivinando reglas
genéricas (ej. un "insertar espacio antes de mayúscula" automático) porque casos como
`VacaDescarte` → "Vaca Descarte" y `SemiConfinamiento` → "Semi-Confinamiento" no siguen
un patrón regular. Usar un diccionario de mapeo explícito por enum, copiado de esta
tabla.

### 2.6 Estados de UI estándar (loading / empty / error)

Ninguna pantalla de Stitch modela explícitamente estos 3 estados (los mockups muestran
siempre el estado "con datos"), así que el frontend debe definirlos de forma
consistente, reutilizando los mismos componentes visuales (tarjetas, colores, bordes)
ya establecidos:

- **Loading**: skeleton con la misma forma/dimensiones que la tarjeta o fila real
  (mismo `border-radius`, mismo alto) en gris `surface-container` con animación pulse.
  No usar un spinner genérico de página completa salvo en el login.
- **Empty** (lista sin resultados, ej. una Captación sin bitácora aún): reusar el
  patrón ya usado en "Maestro de Registros" (icono + texto "No hay registros..."),
  visto también como estado oculto (`hidden`) ya presente en el HTML de "Reporte de
  Captación" — solo hay que quitarle la clase `hidden` cuando la lista real esté
  vacía.
- **Error** (falla de red o 500): banner o texto en `text-error` con el mismo estilo
  que el mensaje "Correo o contraseña incorrectos" de Login — reutilizar ese
  componente de alerta en toda la app, no crear uno nuevo por pantalla.

---

## 3. Datos de Prueba (Seed)

El seed vive en `src/SIGA.Infrastructure/Persistence/DbInitializer.cs` y corre
automáticamente cada vez que se levanta el backend en Development (es idempotente: si
ya existen los datos, no los duplica).

**Cuentas de prueba:**

| Rol | Email | Password |
|---|---|---|
| Administrador | admin@siga.com | Admin123! |
| Captador | captador@siga.com | Captador123! |
| Captador | mquispe@siga.com | Captador123! |

**Datos de campo cargados:** 2 Estancias (Hacienda El Vergel, Estancia Los Pinos), 3
Captaciones, 5 Detalles por categoría, 2 Registros de Pesaje, 2 Registros Sanitarios, 1
Movimiento, 1 Registro de Alimentación, 3 Sectores Asignados — suficiente para
renderizar cada pantalla con datos reales desde la Etapa 1.

Si en algún momento se necesitan más datos (más filas para probar paginación, más
captadores para el Ranking, etc.), ampliar `DbInitializer.cs` siguiendo el mismo
patrón (comprobaciones idempotentes por tabla) en vez de insertar a mano por SQL.

---

## 4. Mapeo Vista ↔ Endpoint

Convención de esta sección: **Request** es el body/query que el frontend envía,
**Response** es el shape que recibe. Todos los campos son obligatorios salvo que se
indique `?` (nullable/opcional). Los endpoints marcados con 🔒 requieren rol
específico; sin marca = cualquier usuario autenticado.

### 4.1 Login

| Pantalla | Método | Endpoint |
|---|---|---|
| Login (Escritorio / Móvil) | `POST` | `/api/auth/login` |

Ver contrato completo en sección 2.2.

---

### 4.2 Registro de Estancia / Listado de Estancias

| Acción | Método | Endpoint | Rol |
|---|---|---|---|
| Listar | `GET` | `/api/estancias` | — |
| Detalle | `GET` | `/api/estancias/{id}` | — |
| Crear | `POST` | `/api/estancias` | 🔒 Captador |
| Editar | `PUT` | `/api/estancias/{id}` | — |
| Eliminar | `DELETE` | `/api/estancias/{id}` | 🔒 Administrador |

**Response (`EstanciaDto`, usado en Listado y en el panel del Mapa de Operaciones):**
```json
{
  "id": "guid",
  "nombre": "Hacienda El Vergel",
  "propietario": "Roberto Salinas Montaño",
  "representante": "Carlos Ruiz",
  "telefono": "70112233",
  "latitud": -17.7833,
  "longitud": -63.1821,
  "renspa": "17-004-00123",
  "hectareasTotales": 850,
  "departamento": "Santa Cruz",
  "provincia": "Andrés Ibáñez",
  "municipio": "Santa Cruz de la Sierra",
  "cantidadCaptaciones": 8,
  "totalCabezas": 450,
  "estadoSync": "Sincronizado"
}
```
`cantidadCaptaciones` y `totalCabezas` vienen precalculados — **no** los sumes en el
frontend, úsalos tal cual llegan (así se implementó en el panel del Mapa de
Operaciones).

**Request (`POST`/`PUT` — "Registro de Estancia"):**
```json
{
  "nombre": "Hacienda El Vergel",
  "propietario": "Roberto Salinas Montaño",
  "representante": "Carlos Ruiz",
  "telefono": "70112233",
  "latitud": -17.7833,
  "longitud": -63.1821,
  "renspa": "17-004-00123",
  "hectareasTotales": 850,
  "departamento": "Santa Cruz",
  "provincia": "Andrés Ibáñez",
  "municipio": "Santa Cruz de la Sierra",
  "fechaCreacionLocal": "2026-08-19T10:00:00Z"
}
```
`fechaCreacionLocal` solo va en el Create (no en el Update) — es la marca de captura
offline, el frontend debe enviar la fecha/hora local del dispositivo en el momento de
guardar el formulario, no la fecha del servidor.

---

### 4.3 Registro de Captación / Reporte de Captación

| Acción | Método | Endpoint | Rol |
|---|---|---|---|
| Listar por estancia | `GET` | `/api/captaciones?estanciaId={id}` | — |
| Detalle | `GET` | `/api/captaciones/{id}` | — |
| Crear (con detalles) | `POST` | `/api/captaciones` | 🔒 Captador |
| Editar cabecera | `PUT` | `/api/captaciones/{id}` | — |
| Eliminar | `DELETE` | `/api/captaciones/{id}` | 🔒 Administrador |
| Agregar detalle | `POST` | `/api/captaciones/{id}/detalles` | — |
| Quitar detalle | `DELETE` | `/api/captaciones/{id}/detalles/{detalleId}` | — |

**Response (`CaptacionGanadoDto`, usado en "Reporte de Captación"):**
```json
{
  "id": "guid",
  "estanciaId": "guid",
  "nombre": "Captación Norte A - Invernada",
  "observaciones": "Grupo mixto recibido en buen estado general.",
  "estado": "Registrado",
  "estadoSanitario": "Optimo",
  "potrero": "Potrero 1 - Alfalfa",
  "fecha": "2026-07-25T00:00:00Z",
  "latitud": -17.7840,
  "longitud": -63.1815,
  "estadoSync": "Sincronizado",
  "totalCabezas": 75,
  "pesoEstimadoTotal": 25650,
  "diasEnPotrero": 25,
  "detalles": [
    {
      "id": "guid",
      "categoria": "Novillo",
      "raza": "Brangus",
      "cantidadCabezas": 45,
      "pesoPromedioEstimadoKg": 380,
      "sistemaAlimentacion": "SemiConfinamiento",
      "fechaEstimadaFaena": "2026-12-25T00:00:00Z",
      "notasZootecnicas": null,
      "pesoLoteCalculado": 17100,
      "diasRestantesFaena": 128
    }
  ]
}
```

**Request Create (cabecera + detalle en una sola llamada — patrón "Confirmar y
Guardar Todo" de la pantalla):**
```json
{
  "estanciaId": "guid",
  "nombre": "Captación Norte A - Invernada",
  "observaciones": "Grupo mixto recibido en buen estado general.",
  "potrero": "Potrero 1 - Alfalfa",
  "fecha": "2026-07-25T00:00:00Z",
  "latitud": -17.7840,
  "longitud": -63.1815,
  "fechaCreacionLocal": "2026-07-25T09:00:00Z",
  "detalles": [
    {
      "categoria": "Novillo",
      "raza": "Brangus",
      "cantidadCabezas": 45,
      "pesoPromedioEstimadoKg": 380,
      "sistemaAlimentacion": "SemiConfinamiento",
      "fechaEstimadaFaena": "2026-12-25T00:00:00Z",
      "notasZootecnicas": null
    }
  ]
}
```
El backend rechaza (`400`) un Create con `detalles: []` — el formulario debe impedir
enviar "Confirmar y Guardar Todo" sin al menos un grupo agregado a la lista, igual que
el mockup lo sugiere visualmente.

**Request Update (`PUT`, solo cabecera — el estado y estado sanitario SÍ son
editables aquí, a diferencia del Create):**
```json
{
  "nombre": "Captación Norte A - Invernada",
  "observaciones": "...",
  "estado": "EnPlanificacionFaena",
  "estadoSanitario": "EnObservacion",
  "potrero": "Potrero 1 - Alfalfa"
}
```

---

### 4.4 Registro de Pesaje

| Acción | Método | Endpoint | Rol |
|---|---|---|---|
| Listar por captación | `GET` | `/api/registros-pesaje?captacionId={id}` | — |
| Detalle | `GET` | `/api/registros-pesaje/{id}` | — |
| Crear | `POST` | `/api/registros-pesaje` | 🔒 Captador |

**Response:**
```json
{
  "id": "guid",
  "captacionGanadoId": "guid",
  "fecha": "2026-08-10T00:00:00Z",
  "pesoPromedioKg": 385.0,
  "cantidadCabezasPesadas": 45,
  "observaciones": "Control de peso mensual.",
  "estadoSync": "Sincronizado"
}
```
`cantidadCabezasPesadas` y `observaciones` son nullable — mostrar "—" cuando vengan
`null` (igual que la tabla del mockup).

**Request Create:** mismos campos sin `id`/`estadoSync`, más `fechaCreacionLocal`.

---

### 4.5 Registro Sanitario

| Acción | Método | Endpoint | Rol |
|---|---|---|---|
| Listar por captación | `GET` | `/api/registros-sanitarios?captacionId={id}` | — |
| Detalle | `GET` | `/api/registros-sanitarios/{id}` | — |
| Crear | `POST` | `/api/registros-sanitarios` | 🔒 Captador |

**Response:**
```json
{
  "id": "guid",
  "captacionGanadoId": "guid",
  "fecha": "2026-08-09T00:00:00Z",
  "tipoEvento": "Vacunacion",
  "productoTratamiento": "Vacuna Aftosa",
  "registradoPorUsuarioId": "guid",
  "registradoPorNombre": "Juan Pérez",
  "observaciones": null,
  "estadoSync": "Sincronizado"
}
```
`registradoPorNombre` ya viene resuelto por el backend — no hace falta cruzarlo con la
lista de usuarios.

**Request Create:** `captacionGanadoId`, `fecha`, `tipoEvento`, `productoTratamiento?`,
`observaciones?`, `fechaCreacionLocal` (`registradoPorUsuarioId` lo asigna el backend
del usuario autenticado, no se envía).

---

### 4.6 Registro de Movimiento

| Acción | Método | Endpoint | Rol |
|---|---|---|---|
| Listar por captación | `GET` | `/api/movimientos?captacionId={id}` | — |
| Detalle | `GET` | `/api/movimientos/{id}` | — |
| Crear | `POST` | `/api/movimientos` | 🔒 Captador |

**Response:**
```json
{
  "id": "guid",
  "captacionGanadoId": "guid",
  "fecha": "2026-08-05T00:00:00Z",
  "tipoGanado": "Ternero",
  "cantidadCabezas": 60,
  "origen": "Corral de Recepción",
  "destino": "Potrero 3 - Gatton Panic",
  "estadoSync": "Sincronizado"
}
```
`origen`/`destino` son texto libre (no un `estanciaId`/`captacionId` — son ubicaciones
físicas dentro de la misma captación, no otra captación).

**Request Create:** mismos campos sin `id`/`estadoSync`, más `fechaCreacionLocal`.

---

### 4.7 Registro de Alimentación

| Acción | Método | Endpoint | Rol |
|---|---|---|---|
| Listar por captación | `GET` | `/api/registros-alimentacion?captacionId={id}` | — |
| Detalle | `GET` | `/api/registros-alimentacion/{id}` | — |
| Crear | `POST` | `/api/registros-alimentacion` | 🔒 Captador |

**Response:**
```json
{
  "id": "guid",
  "captacionGanadoId": "guid",
  "fecha": "2026-08-04T00:00:00Z",
  "tipoAlimentacion": "SemiConfinamiento",
  "racionBaseKgAnimal": 8.5,
  "suplementoProteicoKgAnimal": 1.2,
  "observaciones": "Ración balanceada de engorde.",
  "estadoSync": "Sincronizado"
}
```
`racionBaseKgAnimal`, `suplementoProteicoKgAnimal` y `observaciones` son nullable —
mostrar "—" cuando falten.

**Request Create:** mismos campos sin `id`/`estadoSync`, más `fechaCreacionLocal`.

---

### 4.8 Maestro de Registros

| Acción | Método | Endpoint | Rol |
|---|---|---|---|
| Buscar/listar unificado | `GET` | `/api/registros?desde=&hasta=&tipo=&captacionId=&texto=` | 🔒 Administrador |

Todos los query params son opcionales. `tipo` filtra por el tipo de bitácora, valores
exactos: `Movimiento`, `Alimentacion`, `Sanitario`, `Pesaje`. `texto` busca por
coincidencia parcial (case-insensitive) en `captacionNombre`, `detalleMetrica` o el
`id`.

**Response (lista de `RegistroCampoDto` — estructura unificada de las 4 bitácoras,
verificada contra `RegistroCampoService.cs`; YA VIENE ORDENADA descendente por
`fechaHora`):**
```json
[
  {
    "id": "guid",
    "fechaHora": "2026-08-05T00:00:00Z",
    "tipo": "Movimiento",
    "captacionGanadoId": "guid",
    "captacionNombre": "Captación Norte A - Invernada",
    "detalleMetrica": "60 cabezas: Corral de Recepción -> Potrero 3 - Gatton Panic",
    "productoTratamiento": null,
    "registradoPor": "Juan Pérez",
    "estadoSync": "Sincronizado"
  }
]
```
`productoTratamiento` solo viene poblado cuando `tipo` es `"Sanitario"` (en los otros 3
tipos siempre es `null` — no es un dato que aplique). El texto de `detalleMetrica` ya
viene formateado y listo para mostrar por el backend, distinto según `tipo`:
- Movimiento: `"{cantidad} cabezas: {origen} -> {destino}"`
- Alimentacion: `"{tipoAlimentacion}: {racionBaseKgAnimal} kg/animal"` (o `"-"` si no
  hay ración)
- Sanitario: el nombre del `tipoEvento` (ej. `"Vacunacion"`)
- Pesaje: `"Promedio: {pesoPromedioKg} kg"` (+ `" ({n} cabezas)"` si hay cantidad)

El frontend no necesita reconstruir estos textos — mostrar `detalleMetrica` tal cual en
la columna "Detalles / Métrica" de la tabla.

---

### 4.9 Mapa de Operaciones

No tiene endpoint propio: consume `GET /api/estancias` (sección 4.2). Cada Estancia
trae su propio `latitud`/`longitud` para el pin, y `cantidadCaptaciones`/
`totalCabezas` ya calculados para el panel de detalle — no se necesita ninguna llamada
adicional al seleccionar un pin.

---

### 4.10 Dashboard de Gestión

| Acción | Método | Endpoint | Rol |
|---|---|---|---|
| Resumen | `GET` | `/api/dashboard` | 🔒 Administrador |

**Response (`DashboardDto`):**
```json
{
  "totalCabezasActivas": 12450,
  "captacionesActivas": 48,
  "captacionesPendientesRevision": 3,
  "cabezasPorCategoria": [
    { "categoria": "Toro", "cantidad": 320 },
    { "categoria": "Novillo", "cantidad": 4200 },
    { "categoria": "Vaquilla", "cantidad": 3100 },
    { "categoria": "VacaDescarte", "cantidad": 1830 },
    { "categoria": "Ternero", "cantidad": 3000 }
  ],
  "serieMensual": [
    { "mes": "2026-03", "consumoPromedioKg": 8.2, "pesoPromedioKg": 340.5 },
    { "mes": "2026-04", "consumoPromedioKg": null, "pesoPromedioKg": 355.0 }
  ]
}
```
`cabezasPorCategoria` puede no traer las 5 categorías si alguna no tiene cabezas
activas — el gráfico de "Cabezas por Categoría" debe iterar el array recibido, no
asumir 5 elementos fijos. `serieMensual` siempre trae 6 meses (los últimos 6 meses
calendario), pero cualquiera de los dos valores puede ser `null` si no hubo registros
ese mes — el gráfico de líneas debe cortar/saltar el punto, no mostrar 0.

---

### 4.11 Ranking de Productividad por Captador

| Acción | Método | Endpoint | Rol |
|---|---|---|---|
| Ranking | `GET` | `/api/dashboard/captadores?desde=&hasta=` | 🔒 Administrador |

`desde`/`hasta` opcionales (sin filtro = histórico completo).

**Response (lista de `CaptadorRankingDto`, YA VIENE ORDENADA descendente por
`totalCabezasCapturadas` — no reordenar en frontend):**
```json
[
  {
    "usuarioId": "guid",
    "nombre": "Roberto Salinas",
    "cargo": "Captador de Campo",
    "estanciasRegistradas": 12,
    "captacionesRegistradas": 45,
    "captacionesActivas": 38,
    "totalCabezasCapturadas": 8450
  }
]
```

---

### 4.12 Perfil de Productividad de Captador

| Acción | Método | Endpoint | Rol |
|---|---|---|---|
| Perfil individual | `GET` | `/api/dashboard/captadores/{id}?desde=&hasta=` | Administrador (cualquiera) o el propio Captador (solo el suyo) |

Si un Captador pide el `id` de otro Captador, el backend responde `401` — el frontend
debe usar el `usuarioId` del JWT propio como `id` por defecto cuando el usuario actual
es Captador (no mostrar selector de captador en ese caso).

**Response (`CaptadorProductividadDto`):**
```json
{
  "usuarioId": "guid",
  "nombre": "Roberto Salinas",
  "cargo": "Captador de Campo",
  "estado": "Activo",
  "sectoresAsignados": ["Sector Norte", "Sector Cuarentena"],
  "estanciasRegistradas": 12,
  "captacionesRegistradas": 45,
  "captacionesActivas": 38,
  "totalCabezasCapturadas": 8450,
  "ultimaActividad": "2026-08-18T14:30:00Z"
}
```
`ultimaActividad` puede ser `null` (captador sin actividad registrada aún) — mostrar
"Sin actividad registrada" como en el mockup, no una fecha vacía.

---

### 4.13 Seguridad y Usuarios / Detalle de Usuario

| Acción | Método | Endpoint | Rol |
|---|---|---|---|
| Listar | `GET` | `/api/usuarios` | 🔒 Administrador |
| Detalle (con sectores/dispositivos/permisos) | `GET` | `/api/usuarios/{id}` | 🔒 Administrador |
| Crear | `POST` | `/api/usuarios` | 🔒 Administrador |
| Editar (nombre/cargo/estado) | `PUT` | `/api/usuarios/{id}` | 🔒 Administrador |
| Asignar sector | `POST` | `/api/usuarios/{id}/sectores` | 🔒 Administrador |
| Quitar sector | `DELETE` | `/api/usuarios/{id}/sectores/{sectorId}` | 🔒 Administrador |
| Revocar dispositivo | `POST` | `/api/usuarios/{id}/dispositivos/{dispositivoId}/revocar` | 🔒 Administrador |
| Asignar permiso | `POST` | `/api/usuarios/{id}/permisos` | 🔒 Administrador |

**Response Listado (`UsuarioDto`, usado en "Seguridad y Usuarios"):**
```json
{
  "id": "guid",
  "nombre": "Roberto Soliz",
  "email": "rsoliz@siga.bo",
  "cargo": "Administrador General",
  "rol": "Administrador",
  "estado": "Activo",
  "fechaCreacion": "2023-01-10T00:00:00Z"
}
```

**Response Detalle (`UsuarioDetalleDto`, usado en "Detalle de Usuario" — mismos
campos de arriba más):**
```json
{
  "...": "...campos de UsuarioDto...",
  "sectoresAsignados": [
    { "id": "guid", "nombreSector": "Sector Norte", "zona": "Zona A" }
  ],
  "dispositivos": [
    {
      "id": "guid",
      "identificadorDispositivo": "Samsung Galaxy A54",
      "ultimaSincronizacion": "2026-08-18T10:30:00Z",
      "ubicacionActual": "Santa Cruz",
      "estado": "Activo"
    }
  ],
  "permisos": [
    { "id": "guid", "tipoPermiso": "EntradaDatos" }
  ]
}
```

**Request Crear Usuario:**
```json
{
  "nombre": "María Quispe",
  "email": "mquispe@siga.com",
  "password": "Captador123!",
  "cargo": "Captadora de Campo",
  "rol": "Captador"
}
```

**Request Editar Usuario (`PUT`, no incluye email/password/rol — esos no son
editables por este endpoint):**
```json
{ "nombre": "María Quispe", "cargo": "Captadora de Campo", "estado": "Activo" }
```

**Request Asignar Sector:** `{ "nombreSector": "Sector Sur", "zona": "Zona B" }`

**Request Asignar Permiso:** `{ "tipoPermiso": "RegistrosHistoricos" }`

---

### 4.14 Trazabilidad y Auditoría

| Acción | Método | Endpoint | Rol |
|---|---|---|---|
| Buscar | `GET` | `/api/auditoria?desde=&hasta=&usuarioId=&modulo=` | 🔒 Administrador |

Todos los query params son opcionales.

**Response (lista de `LogAuditoriaDto`):**
```json
[
  {
    "id": "guid",
    "usuarioId": "guid",
    "usuarioNombre": "Roberto Soliz",
    "accion": "Creacion",
    "modulo": "CaptacionGanado",
    "idRegistroAfectado": "guid",
    "fechaHora": "2026-08-18T14:32:00Z",
    "detalle": "Captación Norte A - Invernada"
  }
]
```

---

## 5. Instrucciones de Integración (sin romper diseño ni responsive)

1. **No tocar clases de Tailwind al conectar datos.** El HTML exportado de Stitch ya
   trae la paleta, tipografía, `border-radius` y breakpoints resueltos — al convertir
   cada bloque a componente, mover el texto/atributo a una variable pero dejar las
   clases intactas. Si un componente necesita lógica condicional (ej. color del badge
   de Estado), mapear por el valor de enum (sección 2.5) a la clase ya usada en el
   mockup para ese estado, no inventar una paleta nueva.

2. **Reutilizar componentes entre pantallas que comparten patrón.** Las 4 bitácoras
   (Pesaje, Sanitario, Movimiento, Alimentación) comparten el mismo layout de
   "formulario + historial" — conviene un único componente de bitácora parametrizado
   por tipo, no 4 implementaciones separadas, así los estados loading/empty/error de
   la sección 2.6 se implementan una sola vez.

3. **Selector de "Captación" es compartido.** Pesaje, Sanitario, Movimiento,
   Alimentación y el detalle de Captación todos necesitan un selector de Captación por
   Estancia — implementarlo como un único componente reutilizable que llama a
   `GET /api/captaciones?estanciaId=`.

4. **Respetar el orden ya calculado del backend.** El Ranking de Productividad ya
   viene ordenado por `totalCabezasCapturadas` descendente; `serieMensual` del
   Dashboard ya viene en los 6 meses correctos en orden. No reordenar ni recalcular en
   el frontend — son fuente de verdad del backend.

5. **Rutas protegidas por rol en espejo del backend.** Cada tabla de esta sección
   marca con 🔒 qué rol requiere cada acción — el frontend debe ocultar/deshabilitar
   esas acciones en la UI para el rol que no las tiene (no solo confiar en que el
   backend las rechace con 401), para no mostrar botones que fallan al hacer clic.

6. **Formularios offline-first.** Todos los `Create` de bitácoras y Estancia/Captación
   piden `fechaCreacionLocal` — es la marca de "cuándo se capturó en el dispositivo",
   pensada para uso futuro con sincronización offline. Por ahora (sin offline
   implementado) basta con enviar la fecha/hora actual del navegador al momento de
   guardar.

---

## 6. Plan de Desarrollo por Etapas

Cada etapa debe quedar **funcional y probable de punta a punta** (login real, datos
reales del seed, sin mocks) antes de pasar a la siguiente — así cada entrega es
demostrable.

### Etapa 1 — Autenticación
- Login (Escritorio + Móvil)
- Manejo de sesión: guardar token, adjuntar header `Authorization`, redirigir según
  `rol`, cerrar sesión al expirar `expiraEnUtc`
- **Criterio de salida:** login con las 3 cuentas de prueba, cada una aterriza en la
  vista correcta según su rol.

### Etapa 2 — Datos Maestros
- Listado de Estancias + Registro de Estancia (crear/editar)
- **Criterio de salida:** un Captador puede crear una Estancia nueva y verla aparecer
  en el listado sin recargar la página a mano.

### Etapa 3 — Captura Principal
- Registro de Captación (multi-categoría) + Reporte de Captación
- **Criterio de salida:** crear una Captación con 2+ categorías de detalle, verificar
  que `totalCabezas`/`pesoEstimadoTotal` calculados por el backend se reflejan
  correctamente en "Reporte de Captación".

### Etapa 4 — Bitácoras
- Registro de Pesaje, Registro Sanitario, Registro de Movimiento, Registro de
  Alimentación (las 4 comparten componente base, ver sección 5.2)
- **Criterio de salida:** cada bitácora, creada desde su formulario, aparece en el
  historial de la misma pantalla sin recargar.

### Etapa 5 — Consulta y Reportes
- Maestro de Registros, Mapa de Operaciones
- **Criterio de salida:** un registro creado en la Etapa 4 aparece en Maestro de
  Registros con el filtro `tipo` correcto; una Estancia con Captaciones muestra sus
  cifras correctas en el panel del Mapa.

### Etapa 6 — Administración y Analítica (rol Administrador)
- Dashboard de Gestión, Ranking de Productividad, Perfil de Productividad
- Seguridad y Usuarios, Detalle de Usuario
- Trazabilidad y Auditoría
- **Criterio de salida:** cada acción administrativa (crear usuario, asignar sector,
  revocar dispositivo) queda reflejada tanto en la pantalla como en Trazabilidad y
  Auditoría (por ser todas acciones auditadas por el backend).

---

## 7. Pendientes / Fuera de Alcance de este Documento

- **Mapa de Operaciones** usa solo `GET /api/estancias` por ahora — si más adelante se
  necesita mostrar pines a nivel Captación (no solo Estancia), habrá que decidir cómo
  resolver el nombre de la Estancia dueña de cada Captación en el frontend (el DTO de
  Captación solo trae `estanciaId`, no el nombre — cruzar con la lista de Estancias ya
  cargada, o pedir agregar el campo al backend).
- Pantallas sin backend confirmado (Recuperar Contraseña, Sincronización,
  Configuración del Sistema, Perfil de Usuario propio) quedan fuera de este documento
  — si se van a implementar, primero hay que decidir si construyen backend nuevo o se
  recortan del alcance del frontend.
