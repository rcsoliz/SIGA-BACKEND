using Microsoft.EntityFrameworkCore;
using SIGA.Application.Interfaces;
using SIGA.Domain.Entities;
using SIGA.Domain.Enums;

namespace SIGA.Infrastructure.Persistence;

/// <summary>
/// Datos semilla para desarrollo local: usuarios, estancias, captaciones y bitácoras de
/// ejemplo para poder probar login y flujos completos desde el frontend sin crear datos
/// a mano.
/// </summary>
public static class DbInitializer
{
    public static async Task SeedAsync(SigaDbContext context, IPasswordHasher passwordHasher)
    {
        await context.Database.MigrateAsync();

        var captador = await SeedUsuariosAsync(context, passwordHasher);
        await SeedDatosDeCampoAsync(context, captador);
    }

    private static async Task<Captador> SeedUsuariosAsync(SigaDbContext context, IPasswordHasher passwordHasher)
    {
        if (!await context.Usuarios.AnyAsync())
        {
            context.Usuarios.Add(new Administrador
            {
                Nombre = "Admin SIGA",
                Email = "admin@siga.com",
                Cargo = "Administrador del Sistema",
                PasswordHash = passwordHasher.Hash("Admin123!"),
                Estado = EstadoUsuario.Activo
            });
            await context.SaveChangesAsync();
        }

        var captador = await context.Usuarios.OfType<Captador>().FirstOrDefaultAsync(c => c.Email == "captador@siga.com");
        if (captador is null)
        {
            captador = new Captador
            {
                Nombre = "Juan Pérez",
                Email = "captador@siga.com",
                Cargo = "Captador de Campo",
                PasswordHash = passwordHasher.Hash("Captador123!"),
                Estado = EstadoUsuario.Activo
            };
            context.Usuarios.Add(captador);
            await context.SaveChangesAsync();
        }

        var captador2 = await context.Usuarios.OfType<Captador>().FirstOrDefaultAsync(c => c.Email == "mquispe@siga.com");
        if (captador2 is null)
        {
            captador2 = new Captador
            {
                Nombre = "María Quispe",
                Email = "mquispe@siga.com",
                Cargo = "Captadora de Campo",
                PasswordHash = passwordHasher.Hash("Captador123!"),
                Estado = EstadoUsuario.Activo
            };
            context.Usuarios.Add(captador2);
            await context.SaveChangesAsync();
        }

        if (!await context.SectoresAsignados.AnyAsync())
        {
            context.SectoresAsignados.AddRange(
                new SectorAsignado { UsuarioId = captador.Id, NombreSector = "Sector Norte", Zona = "Zona A" },
                new SectorAsignado { UsuarioId = captador.Id, NombreSector = "Sector Cuarentena", Zona = "Zona C" },
                new SectorAsignado { UsuarioId = captador2.Id, NombreSector = "Sector Sur", Zona = "Zona B" });
            await context.SaveChangesAsync();
        }

        return captador;
    }

    private static async Task SeedDatosDeCampoAsync(SigaDbContext context, Captador captador)
    {
        if (await context.Estancias.AnyAsync())
        {
            return;
        }

        var ahora = DateTime.UtcNow;

        var estanciaElVergel = new Estancia
        {
            CaptadorId = captador.Id,
            Nombre = "Hacienda El Vergel",
            Propietario = "Roberto Salinas Montaño",
            Representante = "Carlos Ruiz",
            Telefono = "70112233",
            Latitud = -17.7833,
            Longitud = -63.1821,
            Renspa = "17-004-00123",
            HectareasTotales = 850,
            Departamento = "Santa Cruz",
            Provincia = "Andrés Ibáñez",
            Municipio = "Santa Cruz de la Sierra"
        };

        var estanciaLosPinos = new Estancia
        {
            CaptadorId = captador.Id,
            Nombre = "Estancia Los Pinos",
            Propietario = "Ana Laura Choque",
            Representante = "Luis Fernando Apaza",
            Telefono = "70223344",
            Latitud = -17.6489,
            Longitud = -63.3897,
            Renspa = "17-004-00456",
            HectareasTotales = 620,
            Departamento = "Santa Cruz",
            Provincia = "Warnes",
            Municipio = "Warnes"
        };

        context.Estancias.AddRange(estanciaElVergel, estanciaLosPinos);
        await context.SaveChangesAsync();

        var captacionNorte = new CaptacionGanado
        {
            EstanciaId = estanciaElVergel.Id,
            Nombre = "Captación Norte A - Invernada",
            Observaciones = "Grupo mixto recibido en buen estado general.",
            Estado = EstadoCaptacion.Registrado,
            EstadoSanitario = EstadoSanitario.Optimo,
            Potrero = "Potrero 1 - Alfalfa",
            Fecha = ahora.AddDays(-25),
            Latitud = -17.7840,
            Longitud = -63.1815
        };

        var captacionCuarentena = new CaptacionGanado
        {
            EstanciaId = estanciaElVergel.Id,
            Nombre = "Captación Cuarentena B",
            Observaciones = "Lote en observación post-ingreso.",
            Estado = EstadoCaptacion.EnPlanificacionFaena,
            EstadoSanitario = EstadoSanitario.EnObservacion,
            Potrero = "Corral de Cuarentena",
            Fecha = ahora.AddDays(-10),
            Latitud = -17.7855,
            Longitud = -63.1802
        };

        var captacionSur = new CaptacionGanado
        {
            EstanciaId = estanciaLosPinos.Id,
            Nombre = "Captación Sur Pasturas",
            Observaciones = "Recepción de ternerada de destete.",
            Estado = EstadoCaptacion.Registrado,
            EstadoSanitario = EstadoSanitario.Optimo,
            Potrero = "Potrero 3 - Gatton Panic",
            Fecha = ahora.AddDays(-5),
            Latitud = -17.6495,
            Longitud = -63.3890
        };

        context.CaptacionesGanado.AddRange(captacionNorte, captacionCuarentena, captacionSur);
        await context.SaveChangesAsync();

        context.DetallesLoteGanado.AddRange(
            new DetalleLoteGanado
            {
                CaptacionGanadoId = captacionNorte.Id,
                Categoria = CategoriaGanado.Novillo,
                Raza = "Brangus",
                CantidadCabezas = 45,
                PesoPromedioEstimadoKg = 380,
                SistemaAlimentacion = TipoManejoAlimentario.SemiConfinamiento,
                FechaEstimadaFaena = ahora.AddMonths(4),
                CreadoPor = captador.Id
            },
            new DetalleLoteGanado
            {
                CaptacionGanadoId = captacionNorte.Id,
                Categoria = CategoriaGanado.Vaquilla,
                Raza = "Brahman",
                CantidadCabezas = 30,
                PesoPromedioEstimadoKg = 290,
                SistemaAlimentacion = TipoManejoAlimentario.PastoreoLibre,
                CreadoPor = captador.Id
            },
            new DetalleLoteGanado
            {
                CaptacionGanadoId = captacionCuarentena.Id,
                Categoria = CategoriaGanado.Toro,
                Raza = "Nelore",
                CantidadCabezas = 6,
                PesoPromedioEstimadoKg = 620,
                SistemaAlimentacion = TipoManejoAlimentario.Confinamiento,
                NotasZootecnicas = "Reproductores en cuarentena sanitaria de rutina.",
                CreadoPor = captador.Id
            },
            new DetalleLoteGanado
            {
                CaptacionGanadoId = captacionSur.Id,
                Categoria = CategoriaGanado.Ternero,
                Raza = "Cruza Comercial",
                CantidadCabezas = 60,
                PesoPromedioEstimadoKg = 160,
                SistemaAlimentacion = TipoManejoAlimentario.PastoreoLibre,
                CreadoPor = captador.Id
            },
            new DetalleLoteGanado
            {
                CaptacionGanadoId = captacionSur.Id,
                Categoria = CategoriaGanado.VacaDescarte,
                Raza = "Nelore",
                CantidadCabezas = 12,
                PesoPromedioEstimadoKg = 410,
                SistemaAlimentacion = TipoManejoAlimentario.PastoreoLibre,
                CreadoPor = captador.Id
            });

        context.RegistrosPesaje.AddRange(
            new RegistroPesaje
            {
                CaptacionGanadoId = captacionNorte.Id,
                Fecha = ahora.AddDays(-20),
                PesoPromedioKg = 350.2,
                CantidadCabezasPesadas = 45,
                Observaciones = "Control de peso mensual.",
                CreadoPorUsuarioId = captador.Id,
                FechaCreacionLocal = ahora.AddDays(-20),
                EstadoSync = EstadoSync.Sincronizado
            },
            new RegistroPesaje
            {
                CaptacionGanadoId = captacionNorte.Id,
                Fecha = ahora.AddDays(-5),
                PesoPromedioKg = 385.0,
                Observaciones = "Pesaje parcial de grupo.",
                CreadoPorUsuarioId = captador.Id,
                FechaCreacionLocal = ahora.AddDays(-5),
                EstadoSync = EstadoSync.Sincronizado
            });

        context.RegistrosSanitarios.AddRange(
            new RegistroSanitario
            {
                CaptacionGanadoId = captacionCuarentena.Id,
                Fecha = ahora.AddDays(-9),
                TipoEvento = TipoEventoSanitario.Vacunacion,
                ProductoTratamiento = "Vacuna Aftosa",
                RegistradoPorUsuarioId = captador.Id,
                CreadoPorUsuarioId = captador.Id,
                FechaCreacionLocal = ahora.AddDays(-9),
                EstadoSync = EstadoSync.Sincronizado
            },
            new RegistroSanitario
            {
                CaptacionGanadoId = captacionCuarentena.Id,
                Fecha = ahora.AddDays(-3),
                TipoEvento = TipoEventoSanitario.ControlRutina,
                Observaciones = "Sin novedades.",
                RegistradoPorUsuarioId = captador.Id,
                CreadoPorUsuarioId = captador.Id,
                FechaCreacionLocal = ahora.AddDays(-3),
                EstadoSync = EstadoSync.Pendiente
            });

        context.MovimientosGanado.Add(new MovimientoGanado
        {
            CaptacionGanadoId = captacionSur.Id,
            Fecha = ahora.AddDays(-2),
            TipoGanado = CategoriaGanado.Ternero,
            CantidadCabezas = 60,
            Origen = "Corral de Recepción",
            Destino = "Potrero 3 - Gatton Panic",
            CreadoPorUsuarioId = captador.Id,
            FechaCreacionLocal = ahora.AddDays(-2),
            EstadoSync = EstadoSync.Sincronizado
        });

        context.RegistrosAlimentacion.Add(new RegistroAlimentacion
        {
            CaptacionGanadoId = captacionNorte.Id,
            Fecha = ahora.AddDays(-15),
            TipoAlimentacion = TipoManejoAlimentario.SemiConfinamiento,
            RacionBaseKgAnimal = 8.5,
            SuplementoProteicoKgAnimal = 1.2,
            Observaciones = "Ración balanceada de engorde.",
            CreadoPorUsuarioId = captador.Id,
            FechaCreacionLocal = ahora.AddDays(-15),
            EstadoSync = EstadoSync.Sincronizado
        });

        await context.SaveChangesAsync();
    }
}
