using System.Linq.Expressions;
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
    private sealed record CatalogoSemilla(
        Provincia Warnes,
        Provincia AndresIbanez,
        Municipio MunicipioWarnes,
        Municipio SantaCruzDeLaSierra,
        Raza Nelore,
        Raza Brangus,
        Raza Brahman,
        Raza CruzaComercial,
        ProductoTratamiento VacunaAftosa);

    public static async Task SeedAsync(SigaDbContext context, IPasswordHasher passwordHasher)
    {
        var captador = await SeedUsuariosAsync(context, passwordHasher);
        var catalogos = await SeedCatalogosAsync(context);
        await SeedDatosDeCampoAsync(context, captador, catalogos);
    }

    private static async Task<CatalogoSemilla> SeedCatalogosAsync(SigaDbContext context)
    {
        var santaCruz = await GetOrCreateAsync(context.Departamentos,
            d => d.Nombre == "Santa Cruz",
            () => new Departamento { Nombre = "Santa Cruz" });

        var warnes = await GetOrCreateAsync(context.Provincias,
            p => p.Nombre == "Warnes" && p.DepartamentoId == santaCruz.Id,
            () => new Provincia { Nombre = "Warnes", DepartamentoId = santaCruz.Id });

        var andresIbanez = await GetOrCreateAsync(context.Provincias,
            p => p.Nombre == "Andrés Ibáñez" && p.DepartamentoId == santaCruz.Id,
            () => new Provincia { Nombre = "Andrés Ibáñez", DepartamentoId = santaCruz.Id });

        var municipioWarnes = await GetOrCreateAsync(context.Municipios,
            m => m.Nombre == "Warnes" && m.ProvinciaId == warnes.Id,
            () => new Municipio { Nombre = "Warnes", ProvinciaId = warnes.Id });

        var santaCruzDeLaSierra = await GetOrCreateAsync(context.Municipios,
            m => m.Nombre == "Santa Cruz de la Sierra" && m.ProvinciaId == andresIbanez.Id,
            () => new Municipio { Nombre = "Santa Cruz de la Sierra", ProvinciaId = andresIbanez.Id });

        var nelore = await GetOrCreateAsync(context.Razas, r => r.Nombre == "Nelore", () => new Raza { Nombre = "Nelore" });
        var brangus = await GetOrCreateAsync(context.Razas, r => r.Nombre == "Brangus", () => new Raza { Nombre = "Brangus" });
        var brahman = await GetOrCreateAsync(context.Razas, r => r.Nombre == "Brahman", () => new Raza { Nombre = "Brahman" });
        var cruzaComercial = await GetOrCreateAsync(context.Razas, r => r.Nombre == "Cruza Comercial", () => new Raza { Nombre = "Cruza Comercial" });

        var vacunaAftosa = await GetOrCreateAsync(context.ProductosTratamiento,
            p => p.Nombre == "Vacuna Aftosa",
            () => new ProductoTratamiento { Nombre = "Vacuna Aftosa" });

        await context.SaveChangesAsync();

        return new CatalogoSemilla(warnes, andresIbanez, municipioWarnes, santaCruzDeLaSierra, nelore, brangus, brahman, cruzaComercial, vacunaAftosa);
    }

    private static async Task<TEntity> GetOrCreateAsync<TEntity>(
        DbSet<TEntity> set, Expression<Func<TEntity, bool>> predicate, Func<TEntity> factory)
        where TEntity : class
    {
        var existente = await set.FirstOrDefaultAsync(predicate);
        if (existente is not null)
        {
            return existente;
        }

        var nuevo = factory();
        set.Add(nuevo);
        return nuevo;
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

    private static async Task SeedDatosDeCampoAsync(SigaDbContext context, Captador captador, CatalogoSemilla catalogos)
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
            Municipio = "Santa Cruz de la Sierra",
            DepartamentoId = catalogos.AndresIbanez.DepartamentoId,
            ProvinciaId = catalogos.AndresIbanez.Id,
            MunicipioId = catalogos.SantaCruzDeLaSierra.Id,
            CreadoPorUsuarioId = captador.Id,
            FechaCreacionLocal = ahora,
            EstadoSync = EstadoSync.Sincronizado
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
            Municipio = "Warnes",
            DepartamentoId = catalogos.Warnes.DepartamentoId,
            ProvinciaId = catalogos.Warnes.Id,
            MunicipioId = catalogos.MunicipioWarnes.Id,
            CreadoPorUsuarioId = captador.Id,
            FechaCreacionLocal = ahora,
            EstadoSync = EstadoSync.Sincronizado
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
            Longitud = -63.1815,
            CreadoPorUsuarioId = captador.Id,
            FechaCreacionLocal = ahora.AddDays(-25),
            EstadoSync = EstadoSync.Sincronizado
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
            Longitud = -63.1802,
            CreadoPorUsuarioId = captador.Id,
            FechaCreacionLocal = ahora.AddDays(-10),
            EstadoSync = EstadoSync.Sincronizado
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
            Longitud = -63.3890,
            CreadoPorUsuarioId = captador.Id,
            FechaCreacionLocal = ahora.AddDays(-5),
            EstadoSync = EstadoSync.Sincronizado
        };

        context.CaptacionesGanado.AddRange(captacionNorte, captacionCuarentena, captacionSur);
        await context.SaveChangesAsync();

        context.DetallesLoteGanado.AddRange(
            new DetalleLoteGanado
            {
                CaptacionGanadoId = captacionNorte.Id,
                Categoria = CategoriaGanado.Novillo,
                Raza = "Brangus",
                RazaId = catalogos.Brangus.Id,
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
                RazaId = catalogos.Brahman.Id,
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
                RazaId = catalogos.Nelore.Id,
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
                RazaId = catalogos.CruzaComercial.Id,
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
                RazaId = catalogos.Nelore.Id,
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
                ProductoTratamientoId = catalogos.VacunaAftosa.Id,
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
