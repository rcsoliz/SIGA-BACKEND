# SIGA — Actualización de Requisitos y Modelo de Dominio (Backend)

> Documento de trabajo generado a partir de la revisión cruzada entre el documento base
> "ACTIVIDAD I - FASE ANÁLISIS DE REQUISITOS" (repo `SIGA-DOC`) y el diseño UI/UX definido
> en Stitch ("SIGA DESIGN"). Su propósito es servir de insumo para actualizar el documento
> de requisitos y el diagrama de clases del proyecto.

## 1. Motivo de esta actualización

El diseño de interfaz en Stitch (fuente de verdad para el frontend, ya aprobado) contiene
funcionalidad que el documento de requisitos original no contempla explícitamente. En vez de
forzar el backend a los límites del documento, se decidió ampliar el alcance para que el
backend soporte lo que la interfaz ya exige — con las excepciones que se detallan en la
sección 3 (fuera de alcance).

Adicionalmente, se identificó que el propio requisito de **operación offline con
sincronización posterior** (ya presente en el documento original, Sprint 2) exige por sí
mismo un mínimo de trazabilidad por registro, aunque el documento no lo pida de forma
explícita como "auditoría". Ver sección 4.

## 2. Alcance ampliado — módulos incluidos en el backend

Además de lo ya descrito en el documento base (estancia, coordenadas GPS, lote, tipo de
alimentación, fecha de faena, consulta admin, control de acceso por rol), se incluyen:

| Módulo | Justificación |
|---|---|
| **Movimiento de Ganado** | Registro de traslados de lotes entre potreros/corrales (origen → destino). Es una extensión natural del seguimiento de lotes que el documento ya pide, pero modelado como bitácora de eventos, no como campo estático. |
| **Registro de Alimentación como bitácora** | El documento pide "tipo de alimentación" como dato del lote. El diseño lo eleva a registro recurrente (fecha, ración base, suplemento, observaciones) — más fiel al proceso real de campo. |
| **Registro Sanitario** | Eventos de salud/tratamiento por lote (vacunación, antiparasitario, control de rutina). No tiene pantalla de captura dedicada en el diseño aún, se infiere de los datos mostrados en "Reporte de Lote" y "Maestro de Registros". |
| **Seguridad y Usuarios (RBAC ampliado)** | Va más allá de "login con rol": permisos granulares, sectores geográficos asignados por usuario, gestión de dispositivos vinculados (alta/revocación). |
| **Auditoría / Trazabilidad** | Registro de acciones (creación/modificación/eliminación) por usuario, módulo y fecha. Justificado en la sección 4. |

## 3. Explícitamente fuera de alcance (por ahora)

Estos módulos existen en el diseño de Stitch pero **no se implementan** en esta fase de backend:

- **Inventario de Insumos** (stock de forraje, medicamentos, suplementos, alertas de stock)
- **Alertas Críticas** (centro de notificaciones)
- **Estado de Carga de Trabajo** (gestión de tareas diarias)

## 4. Por qué se necesita auditoría (aunque el documento no la pida explícitamente)

Dos razones, ambas derivadas del propio documento base, no del diseño:

1. **El requisito de sincronización offline lo exige de facto.** El documento pide
   "almacenamiento local temporal... sincronizando cuando haya señal" (Sprint 2). Sin un
   mínimo de metadatos por registro (quién lo creó, cuándo se creó localmente, cuándo se
   sincronizó, si la sincronización fue exitosa) el propio mecanismo de sync no se puede
   construir ni depurar.
2. **El negocio es, por naturaleza, de trazabilidad.** El sistema alimenta decisiones
   comerciales y logísticas (compra, faena). Con varios captadores registrando datos sin
   conexión, saber "quién registró qué y cuándo" es lo que da confiabilidad al dato.

Se distingue entre dos niveles:

- **Campos de auditoría por registro (obligatorio)** — bajo costo, alto valor, derivado
  directamente del requisito de sync offline.
- **Módulo completo de "Logs de Seguridad"** (eventos AUTH/DATA/CRITICAL/SYSTEM, pantalla
  dedicada) — capa de seguridad empresarial más allá del mínimo. Se incluye porque ya fue
  aprobado como parte del alcance, pero es la parte que menos se apoya en el documento
  original.

## 5. Modelo de dominio actualizado

### 5.1 Entidades base (del documento original)

- **Usuario** (abstracta/base) — correo, contraseñaHash, rol
  - **Captador** (hereda de Usuario) — 0..* Estancias
  - **Administrador** (hereda de Usuario)
- **Estancia** — nombre, propietario, representante, teléfono, latitud, longitud, **RENSPA**,
  hectáreasTotales, ubicación/región
  - Composición: Estancia "1" *-- "1..*" Lote
- **Lote (LoteGanado)** — tipo/categoría de ganado, raza, cantidadCabezas, pesoPromedio,
  tipoAlimentación (actual/default), estadoSanitario, potrero/ubicación actual, fechaIngreso,
  fechaFaenaEstimada

### 5.2 Entidades nuevas (bitácoras, ligadas a Lote)

- **RegistroAlimentacion** — loteId (FK), fecha, tipoAlimentación (enum), raciónBaseKgAnimal,
  suplementoProteicoKgAnimal, observaciones
- **MovimientoGanado** — loteOrigenId (FK), loteDestinoId (FK), fecha, tipoGanado/especie,
  cantidadCabezas
- **RegistroSanitario** — loteId (FK), fecha, tipoEvento (vacunación/antiparasitario/control),
  productoTratamiento, registradoPorUsuarioId (FK), observaciones

### 5.3 Seguridad y Usuarios (RBAC ampliado)

- **Usuario** extendido con: cargo/puesto, estado (Activo/Pendiente/Suspendido)
- **SectorAsignado** — usuarioId (FK), nombreSector, zona (relación muchos-a-muchos
  Usuario↔Sector)
- **Dispositivo** — usuarioId (FK), identificadorDispositivo, últimaSincronización,
  ubicaciónActual, estado (Activo/Revocado)
- **PermisoUsuario** — usuarioId (FK), tipoPermiso (EntradaDatos / RegistrosHistóricos /
  ConfiguraciónSistema)

### 5.4 Auditoría (campos transversales)

Aplicados a **Estancia, Lote, RegistroAlimentacion, MovimientoGanado, RegistroSanitario**:

| Campo | Tipo | Propósito |
|---|---|---|
| `creadoPorUsuarioId` | FK → Usuario | Quién capturó el dato (Captador) |
| `fechaCreacionLocal` | datetime | Timestamp del dispositivo al capturar (puede ser offline) |
| `fechaSincronización` | datetime, nullable | Cuándo llegó al servidor |
| `estadoSync` | enum (Pendiente / Sincronizado / Error) | Estado de sincronización |
| `modificadoPorUsuarioId` | FK → Usuario, nullable | Última edición |
| `fechaModificación` | datetime, nullable | Fecha de última edición |

Adicionalmente, un log de acciones transversal (más liviano que un sistema de seguridad
completo):

- **LogAuditoria** — usuarioId (FK), acción (Creación/Modificación/Eliminación), módulo,
  idRegistroAfectado, fechaHora

### 5.5 Enumeraciones

- `CategoriaGanado` (Bovino, Ovino, Porcino, Otros)
- `TipoManejoAlimentario` (Pastoreo libre, Semi-confinamiento, Confinamiento)
- `RolUsuario` (Captador, Administrador)
- `EstadoSync` (Pendiente, Sincronizado, Error)
- `EstadoUsuario` (Activo, Pendiente, Suspendido)

## 6. Pendiente de decisión

Existía una bifurcación de diseño en Stitch entre una versión "Final" (con FKs explícitas de
lote origen/destino en Movimiento, columnas de auditoría en el Maestro de Registros) y una
versión "Requisitos Base" (más simple, apegada literalmente al documento, sin esas
extensiones). **Con la confirmación de que la auditoría es necesaria, se adopta la rama
"Final" como referencia** para el modelo de dominio descrito arriba.

## 7. Siguiente paso

Con este modelo de dominio confirmado, el siguiente paso es diseñar los contratos de API
(DTOs de request/response) por cada pantalla del diseño Stitch, y luego la implementación en
.NET Core con Clean Architecture (Domain / Application / Infrastructure / WebAPI) sobre
PostgreSQL, según lo definido en el documento base.
