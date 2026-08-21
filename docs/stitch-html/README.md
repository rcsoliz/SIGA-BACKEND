# HTML de referencia — SIGA DESIGN (Stitch)

Exportación de las **17 pantallas verificadas** del proyecto Stitch "SIGA DESIGN"
(34 archivos: escritorio + móvil de cada una). Cada archivo es HTML + Tailwind CSS
estático con datos de ejemplo quemados en el markup — no está conectado a ninguna
API. Es la referencia visual/estructural para construir los componentes reales del
frontend; ver `../especificacion-integracion-frontend.md` para el mapeo de cada
pantalla a su endpoint real y los contratos JSON.

**No se exportó nada más del proyecto Stitch a propósito** — el proyecto tiene
pantallas viejas/duplicadas descartadas durante el proceso de diseño (ver sección 1.2
del documento de especificación) que no deben usarse como referencia.

## Convención de nombres

`{etapa}-{pantalla}-{plataforma}.html`, en el mismo orden de etapas del plan de
desarrollo (sección 6 de la especificación):

| Archivo | Pantalla |
|---|---|
| `01-login-*` | Login |
| `02-registro-estancia-*` | Registro de Estancia |
| `02-listado-estancias-*` | Listado de Estancias |
| `03-registro-captacion-*` | Registro de Captación (multi-categoría) |
| `03-reporte-captacion-*` | Reporte de Captación |
| `04-registro-pesaje-*` | Registro de Pesaje |
| `04-registro-sanitario-*` | Registro Sanitario |
| `04-registro-movimiento-*` | Registro de Movimiento |
| `04-registro-alimentacion-*` | Registro de Alimentación |
| `05-maestro-registros-*` | Maestro de Registros |
| `05-mapa-operaciones-*` | Mapa de Operaciones |
| `06-dashboard-gestion-*` | Dashboard de Gestión |
| `06-ranking-productividad-*` | Ranking de Productividad por Captador |
| `06-perfil-productividad-*` | Perfil de Productividad de Captador |
| `06-seguridad-usuarios-*` | Seguridad y Usuarios |
| `06-detalle-usuario-*` | Detalle de Usuario |
| `06-trazabilidad-auditoria-*` | Trazabilidad y Auditoría |

`*` es `desktop` o `mobile`.

## Pendiente de limpieza en Stitch (no exportado)

Durante la exportación apareció un duplicado no revisado de "Registro de Pesaje
(Escritorio)" con sufijo "- ES Final" que no es el que se verificó y documentó — se
exportó la versión correcta ("- ES", sin sufijo), pero conviene revisar/eliminar ese
duplicado directamente en el proyecto de Stitch para no confundirlo a futuro.
