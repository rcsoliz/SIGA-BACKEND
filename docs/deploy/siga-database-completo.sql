-- ============================================================================
-- SIGA (Sistema de Registro y Captacion de Ganado) -- Script completo de BD
-- Generado: 2026-08-21
-- Contenido: Esquema completo (via EF Core migrations, idempotente) + datos
--            reales actuales de la base de desarrollo (usuarios, estancias,
--            captaciones y bitacoras).
--
-- USO:
--   psql -h <host> -U <usuario> -d <basededatos> -f siga-database-completo.sql
--
-- Es SEGURO volver a correr este script sobre una base ya inicializada con
-- el mismo esquema: la parte de esquema es idempotente (usa
-- IF NOT EXISTS / chequeo contra __EFMigrationsHistory). La parte de DATOS
-- NO es idempotente -- si la corres dos veces sobre la misma base fallara
-- por violacion de llave primaria/unica. Usar solo contra una base vacia
-- (recien creada) o quitar la seccion de datos si ya tiene informacion.
--
-- *** ADVERTENCIA DE SEGURIDAD ANTES DE USAR EN UN SERVIDOR PUBLICO ***
-- Los usuarios incluidos abajo son cuentas de PRUEBA con contrasenas
-- conocidas (ver docs/especificacion-integracion-frontend.md, seccion 3):
--   admin@siga.com / Admin123!
--   captador@siga.com / Captador123!
--   mquispe@siga.com / Captador123!
-- Tambien se incluye una cuenta "Usuario Smoke Test" (Estado: Pendiente)
-- creada durante pruebas manuales del frontend -- revisar si conviene
-- borrarla antes de usar esto como demo/entrega final.
-- Si esto se va a exponer en un servidor accesible fuera de la red local
-- (no solo para demo/entrega de la materia), cambiar estas contrasenas
-- inmediatamente despues de cargar el script (son hashes bcrypt reales,
-- pero las contrasenas en texto plano ya son conocidas por estar en este
-- mismo repositorio).
-- ============================================================================


-- ============================================================================
-- PARTE 1: ESQUEMA (generado con `dotnet ef migrations script --idempotent`)
-- ============================================================================

CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260817011511_InitialCreate') THEN
    CREATE TABLE "Usuarios" (
        "Id" uuid NOT NULL,
        "Nombre" character varying(150) NOT NULL,
        "Email" character varying(200) NOT NULL,
        "PasswordHash" text NOT NULL,
        "Cargo" character varying(150),
        "Rol" character varying(30) NOT NULL,
        "Estado" character varying(30) NOT NULL,
        "FechaCreacion" timestamp with time zone NOT NULL,
        "TipoUsuario" character varying(13) NOT NULL,
        CONSTRAINT "PK_Usuarios" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260817011511_InitialCreate') THEN
    CREATE TABLE "Dispositivos" (
        "Id" uuid NOT NULL,
        "UsuarioId" uuid NOT NULL,
        "IdentificadorDispositivo" character varying(150) NOT NULL,
        "UltimaSincronizacion" timestamp with time zone,
        "UbicacionActual" character varying(150),
        "Estado" character varying(20) NOT NULL,
        CONSTRAINT "PK_Dispositivos" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_Dispositivos_Usuarios_UsuarioId" FOREIGN KEY ("UsuarioId") REFERENCES "Usuarios" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260817011511_InitialCreate') THEN
    CREATE TABLE "Estancias" (
        "Id" uuid NOT NULL,
        "CaptadorId" uuid NOT NULL,
        "Nombre" character varying(200) NOT NULL,
        "Propietario" character varying(200) NOT NULL,
        "Representante" character varying(200),
        "Telefono" character varying(30),
        "Latitud" double precision NOT NULL,
        "Longitud" double precision NOT NULL,
        "Renspa" character varying(50),
        "HectareasTotales" double precision,
        "Departamento" character varying(100),
        "Provincia" character varying(100),
        "Municipio" character varying(100),
        "CreadoPorUsuarioId" uuid NOT NULL,
        "FechaCreacionLocal" timestamp with time zone NOT NULL,
        "FechaSincronizacion" timestamp with time zone,
        "EstadoSync" character varying(20) NOT NULL,
        "ModificadoPorUsuarioId" uuid,
        "FechaModificacion" timestamp with time zone,
        CONSTRAINT "PK_Estancias" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_Estancias_Usuarios_CaptadorId" FOREIGN KEY ("CaptadorId") REFERENCES "Usuarios" ("Id") ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260817011511_InitialCreate') THEN
    CREATE TABLE "LogsAuditoria" (
        "Id" uuid NOT NULL,
        "UsuarioId" uuid NOT NULL,
        "Accion" character varying(20) NOT NULL,
        "Modulo" character varying(100) NOT NULL,
        "IdRegistroAfectado" uuid NOT NULL,
        "FechaHora" timestamp with time zone NOT NULL,
        "Detalle" character varying(500),
        CONSTRAINT "PK_LogsAuditoria" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_LogsAuditoria_Usuarios_UsuarioId" FOREIGN KEY ("UsuarioId") REFERENCES "Usuarios" ("Id") ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260817011511_InitialCreate') THEN
    CREATE TABLE "PermisosUsuario" (
        "Id" uuid NOT NULL,
        "UsuarioId" uuid NOT NULL,
        "TipoPermiso" character varying(30) NOT NULL,
        CONSTRAINT "PK_PermisosUsuario" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_PermisosUsuario_Usuarios_UsuarioId" FOREIGN KEY ("UsuarioId") REFERENCES "Usuarios" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260817011511_InitialCreate') THEN
    CREATE TABLE "SectoresAsignados" (
        "Id" uuid NOT NULL,
        "UsuarioId" uuid NOT NULL,
        "NombreSector" character varying(150) NOT NULL,
        "Zona" character varying(150),
        CONSTRAINT "PK_SectoresAsignados" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_SectoresAsignados_Usuarios_UsuarioId" FOREIGN KEY ("UsuarioId") REFERENCES "Usuarios" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260817011511_InitialCreate') THEN
    CREATE TABLE "CaptacionesGanado" (
        "Id" uuid NOT NULL,
        "EstanciaId" uuid NOT NULL,
        "Nombre" character varying(150) NOT NULL,
        "Observaciones" character varying(1000),
        "Estado" character varying(30) NOT NULL,
        "EstadoSanitario" character varying(20) NOT NULL,
        "Potrero" character varying(150),
        "Fecha" timestamp with time zone NOT NULL,
        "CreadoPorUsuarioId" uuid NOT NULL,
        "FechaCreacionLocal" timestamp with time zone NOT NULL,
        "FechaSincronizacion" timestamp with time zone,
        "EstadoSync" character varying(20) NOT NULL,
        "ModificadoPorUsuarioId" uuid,
        "FechaModificacion" timestamp with time zone,
        CONSTRAINT "PK_CaptacionesGanado" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_CaptacionesGanado_Estancias_EstanciaId" FOREIGN KEY ("EstanciaId") REFERENCES "Estancias" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260817011511_InitialCreate') THEN
    CREATE TABLE "DetallesLoteGanado" (
        "Id" uuid NOT NULL,
        "CaptacionGanadoId" uuid NOT NULL,
        "Categoria" character varying(20) NOT NULL,
        "Raza" character varying(100),
        "CantidadCabezas" integer NOT NULL,
        "PesoPromedioEstimadoKg" double precision,
        "SistemaAlimentacion" character varying(30) NOT NULL,
        "FechaEstimadaFaena" timestamp with time zone,
        "NotasZootecnicas" character varying(1000),
        "CreadoPor" uuid NOT NULL,
        "CreadoEn" timestamp with time zone NOT NULL,
        "ActualizadoPor" uuid,
        "ActualizadoEn" timestamp with time zone,
        CONSTRAINT "PK_DetallesLoteGanado" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_DetallesLoteGanado_CaptacionesGanado_CaptacionGanadoId" FOREIGN KEY ("CaptacionGanadoId") REFERENCES "CaptacionesGanado" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260817011511_InitialCreate') THEN
    CREATE TABLE "MovimientosGanado" (
        "Id" uuid NOT NULL,
        "CaptacionGanadoId" uuid NOT NULL,
        "Fecha" timestamp with time zone NOT NULL,
        "TipoGanado" character varying(20) NOT NULL,
        "CantidadCabezas" integer NOT NULL,
        "Origen" character varying(150) NOT NULL,
        "Destino" character varying(150) NOT NULL,
        "CreadoPorUsuarioId" uuid NOT NULL,
        "FechaCreacionLocal" timestamp with time zone NOT NULL,
        "FechaSincronizacion" timestamp with time zone,
        "EstadoSync" character varying(20) NOT NULL,
        "ModificadoPorUsuarioId" uuid,
        "FechaModificacion" timestamp with time zone,
        CONSTRAINT "PK_MovimientosGanado" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_MovimientosGanado_CaptacionesGanado_CaptacionGanadoId" FOREIGN KEY ("CaptacionGanadoId") REFERENCES "CaptacionesGanado" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260817011511_InitialCreate') THEN
    CREATE TABLE "RegistrosAlimentacion" (
        "Id" uuid NOT NULL,
        "CaptacionGanadoId" uuid NOT NULL,
        "Fecha" timestamp with time zone NOT NULL,
        "TipoAlimentacion" character varying(30) NOT NULL,
        "RacionBaseKgAnimal" double precision,
        "SuplementoProteicoKgAnimal" double precision,
        "Observaciones" character varying(1000),
        "CreadoPorUsuarioId" uuid NOT NULL,
        "FechaCreacionLocal" timestamp with time zone NOT NULL,
        "FechaSincronizacion" timestamp with time zone,
        "EstadoSync" character varying(20) NOT NULL,
        "ModificadoPorUsuarioId" uuid,
        "FechaModificacion" timestamp with time zone,
        CONSTRAINT "PK_RegistrosAlimentacion" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_RegistrosAlimentacion_CaptacionesGanado_CaptacionGanadoId" FOREIGN KEY ("CaptacionGanadoId") REFERENCES "CaptacionesGanado" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260817011511_InitialCreate') THEN
    CREATE TABLE "RegistrosSanitarios" (
        "Id" uuid NOT NULL,
        "CaptacionGanadoId" uuid NOT NULL,
        "Fecha" timestamp with time zone NOT NULL,
        "TipoEvento" character varying(30) NOT NULL,
        "ProductoTratamiento" character varying(200),
        "RegistradoPorUsuarioId" uuid NOT NULL,
        "Observaciones" character varying(1000),
        "CreadoPorUsuarioId" uuid NOT NULL,
        "FechaCreacionLocal" timestamp with time zone NOT NULL,
        "FechaSincronizacion" timestamp with time zone,
        "EstadoSync" character varying(20) NOT NULL,
        "ModificadoPorUsuarioId" uuid,
        "FechaModificacion" timestamp with time zone,
        CONSTRAINT "PK_RegistrosSanitarios" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_RegistrosSanitarios_CaptacionesGanado_CaptacionGanadoId" FOREIGN KEY ("CaptacionGanadoId") REFERENCES "CaptacionesGanado" ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_RegistrosSanitarios_Usuarios_RegistradoPorUsuarioId" FOREIGN KEY ("RegistradoPorUsuarioId") REFERENCES "Usuarios" ("Id") ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260817011511_InitialCreate') THEN
    CREATE INDEX "IX_CaptacionesGanado_EstanciaId" ON "CaptacionesGanado" ("EstanciaId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260817011511_InitialCreate') THEN
    CREATE INDEX "IX_DetallesLoteGanado_CaptacionGanadoId" ON "DetallesLoteGanado" ("CaptacionGanadoId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260817011511_InitialCreate') THEN
    CREATE INDEX "IX_Dispositivos_UsuarioId" ON "Dispositivos" ("UsuarioId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260817011511_InitialCreate') THEN
    CREATE INDEX "IX_Estancias_CaptadorId" ON "Estancias" ("CaptadorId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260817011511_InitialCreate') THEN
    CREATE INDEX "IX_LogsAuditoria_FechaHora" ON "LogsAuditoria" ("FechaHora");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260817011511_InitialCreate') THEN
    CREATE INDEX "IX_LogsAuditoria_UsuarioId" ON "LogsAuditoria" ("UsuarioId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260817011511_InitialCreate') THEN
    CREATE INDEX "IX_MovimientosGanado_CaptacionGanadoId" ON "MovimientosGanado" ("CaptacionGanadoId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260817011511_InitialCreate') THEN
    CREATE INDEX "IX_PermisosUsuario_UsuarioId" ON "PermisosUsuario" ("UsuarioId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260817011511_InitialCreate') THEN
    CREATE INDEX "IX_RegistrosAlimentacion_CaptacionGanadoId" ON "RegistrosAlimentacion" ("CaptacionGanadoId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260817011511_InitialCreate') THEN
    CREATE INDEX "IX_RegistrosSanitarios_CaptacionGanadoId" ON "RegistrosSanitarios" ("CaptacionGanadoId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260817011511_InitialCreate') THEN
    CREATE INDEX "IX_RegistrosSanitarios_RegistradoPorUsuarioId" ON "RegistrosSanitarios" ("RegistradoPorUsuarioId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260817011511_InitialCreate') THEN
    CREATE INDEX "IX_SectoresAsignados_UsuarioId" ON "SectoresAsignados" ("UsuarioId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260817011511_InitialCreate') THEN
    CREATE UNIQUE INDEX "IX_Usuarios_Email" ON "Usuarios" ("Email");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260817011511_InitialCreate') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260817011511_InitialCreate', '8.0.10');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260818005442_AgregarGapsUiCoherencia') THEN
    ALTER TABLE "CaptacionesGanado" ADD "Latitud" double precision;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260818005442_AgregarGapsUiCoherencia') THEN
    ALTER TABLE "CaptacionesGanado" ADD "Longitud" double precision;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260818005442_AgregarGapsUiCoherencia') THEN
    CREATE TABLE "RegistrosPesaje" (
        "Id" uuid NOT NULL,
        "CaptacionGanadoId" uuid NOT NULL,
        "Fecha" timestamp with time zone NOT NULL,
        "PesoPromedioKg" double precision NOT NULL,
        "CantidadCabezasPesadas" integer,
        "Observaciones" character varying(1000),
        "CreadoPorUsuarioId" uuid NOT NULL,
        "FechaCreacionLocal" timestamp with time zone NOT NULL,
        "FechaSincronizacion" timestamp with time zone,
        "EstadoSync" character varying(20) NOT NULL,
        "ModificadoPorUsuarioId" uuid,
        "FechaModificacion" timestamp with time zone,
        CONSTRAINT "PK_RegistrosPesaje" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_RegistrosPesaje_CaptacionesGanado_CaptacionGanadoId" FOREIGN KEY ("CaptacionGanadoId") REFERENCES "CaptacionesGanado" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260818005442_AgregarGapsUiCoherencia') THEN
    CREATE INDEX "IX_RegistrosPesaje_CaptacionGanadoId" ON "RegistrosPesaje" ("CaptacionGanadoId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260818005442_AgregarGapsUiCoherencia') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260818005442_AgregarGapsUiCoherencia', '8.0.10');
    END IF;
END $EF$;
COMMIT;



-- ============================================================================
-- PARTE 2: DATOS (snapshot real de la base de desarrollo)
-- ============================================================================

BEGIN;

--
-- PostgreSQL database dump
--


-- Dumped from database version 16.14 (Debian 16.14-1.pgdg13+1)
-- Dumped by pg_dump version 16.14 (Debian 16.14-1.pgdg13+1)

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- Data for Name: Usuarios; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public."Usuarios" ("Id", "Nombre", "Email", "PasswordHash", "Cargo", "Rol", "Estado", "FechaCreacion", "TipoUsuario") VALUES ('4ee7acd6-bf93-48ea-9c4e-7e5c78531596', 'Admin SIGA', 'admin@siga.com', '$2a$11$aSgbxk5U81Ao5rDBfVpB/eIktPqMRR/yTVQo.wz3l/zQPpOm5e1I6', 'Administrador del Sistema', 'Administrador', 'Activo', '2026-08-17 01:16:04.463427+00', 'Administrador');
INSERT INTO public."Usuarios" ("Id", "Nombre", "Email", "PasswordHash", "Cargo", "Rol", "Estado", "FechaCreacion", "TipoUsuario") VALUES ('c91ce5ff-1f1c-42fc-ac21-0899f2e76561', 'María Quispe', 'mquispe@siga.com', '$2a$11$6Jn24DXgRIA1re7CXxV.T.KgoMmClMRZnHgJ2x/6nxP.WnbsoNAOi', 'Captadora de Campo', 'Captador', 'Activo', '2026-08-19 01:38:57.609159+00', 'Captador');
INSERT INTO public."Usuarios" ("Id", "Nombre", "Email", "PasswordHash", "Cargo", "Rol", "Estado", "FechaCreacion", "TipoUsuario") VALUES ('ca3eba47-a783-444d-92b9-e6798bb78b9f', 'Usuario Smoke Test', 'smoke1787280760657@siga.com', '$2a$11$hpUAUNJ9pfAS91qgiBmYJuUpQ1qVtKNFsA98lLnIeMEPFSrxn9NDO', 'Captador de Prueba', 'Captador', 'Pendiente', '2026-08-21 02:52:41.569031+00', 'Captador');
INSERT INTO public."Usuarios" ("Id", "Nombre", "Email", "PasswordHash", "Cargo", "Rol", "Estado", "FechaCreacion", "TipoUsuario") VALUES ('dc9b1c7c-d343-4b18-b9d2-411ddf03e712', 'Juan Pérez', 'captador@siga.com', '$2a$11$G/eNvaDPIH1DSjTBSqLWYOoOTVeXUlfclSpTAjUZQPYkvcAENN4fi', 'Captador de Campo', 'Captador', 'Activo', '2026-08-17 01:16:04.862565+00', 'Captador');


--
-- Data for Name: Estancias; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public."Estancias" ("Id", "CaptadorId", "Nombre", "Propietario", "Representante", "Telefono", "Latitud", "Longitud", "Renspa", "HectareasTotales", "Departamento", "Provincia", "Municipio", "CreadoPorUsuarioId", "FechaCreacionLocal", "FechaSincronizacion", "EstadoSync", "ModificadoPorUsuarioId", "FechaModificacion") VALUES ('5b727539-0a47-480c-869c-e4c89fa8b70e', 'dc9b1c7c-d343-4b18-b9d2-411ddf03e712', 'Estancia Los Pinos', 'Ana Laura Choque', 'Luis Fernando Apaza', '70223344', -17.6489, -63.3897, '17-004-00456', 620, 'Santa Cruz', 'Warnes', 'Warnes', '00000000-0000-0000-0000-000000000000', '-infinity', NULL, 'Pendiente', NULL, NULL);
INSERT INTO public."Estancias" ("Id", "CaptadorId", "Nombre", "Propietario", "Representante", "Telefono", "Latitud", "Longitud", "Renspa", "HectareasTotales", "Departamento", "Provincia", "Municipio", "CreadoPorUsuarioId", "FechaCreacionLocal", "FechaSincronizacion", "EstadoSync", "ModificadoPorUsuarioId", "FechaModificacion") VALUES ('e2596f3b-e4f2-4911-baad-cbfc9bbcf693', 'dc9b1c7c-d343-4b18-b9d2-411ddf03e712', 'Hacienda El Vergel', 'Roberto Salinas Montaño', 'Carlos Ruiz', '70112233', -17.7833, -63.1821, '17-004-00123', 850, 'Santa Cruz', 'Andrés Ibáñez', 'Santa Cruz de la Sierra', '00000000-0000-0000-0000-000000000000', '-infinity', NULL, 'Pendiente', NULL, NULL);
INSERT INTO public."Estancias" ("Id", "CaptadorId", "Nombre", "Propietario", "Representante", "Telefono", "Latitud", "Longitud", "Renspa", "HectareasTotales", "Departamento", "Provincia", "Municipio", "CreadoPorUsuarioId", "FechaCreacionLocal", "FechaSincronizacion", "EstadoSync", "ModificadoPorUsuarioId", "FechaModificacion") VALUES ('d54ef819-a162-4bf0-a722-de50984887aa', 'dc9b1c7c-d343-4b18-b9d2-411ddf03e712', 'San Paola', 'Ralondo Perez', 'Rolando Perez', '70032451', -17.836565327280617, -63.225753254358686, 'Joel Peres', 60, 'Rerum similique veli', 'Consectetur magna q', 'Assumenda ratione ve', 'dc9b1c7c-d343-4b18-b9d2-411ddf03e712', '2026-08-21 02:00:43.969+00', NULL, 'Sincronizado', NULL, NULL);


--
-- Data for Name: CaptacionesGanado; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public."CaptacionesGanado" ("Id", "EstanciaId", "Nombre", "Observaciones", "Estado", "EstadoSanitario", "Potrero", "Fecha", "CreadoPorUsuarioId", "FechaCreacionLocal", "FechaSincronizacion", "EstadoSync", "ModificadoPorUsuarioId", "FechaModificacion", "Latitud", "Longitud") VALUES ('32c33837-52ad-4676-8180-006379270c01', 'e2596f3b-e4f2-4911-baad-cbfc9bbcf693', 'Captación Norte A - Invernada', 'Grupo mixto recibido en buen estado general.', 'Registrado', 'Optimo', 'Potrero 1 - Alfalfa', '2026-07-25 01:37:24.420124+00', '00000000-0000-0000-0000-000000000000', '-infinity', NULL, 'Pendiente', NULL, NULL, -17.784, -63.1815);
INSERT INTO public."CaptacionesGanado" ("Id", "EstanciaId", "Nombre", "Observaciones", "Estado", "EstadoSanitario", "Potrero", "Fecha", "CreadoPorUsuarioId", "FechaCreacionLocal", "FechaSincronizacion", "EstadoSync", "ModificadoPorUsuarioId", "FechaModificacion", "Latitud", "Longitud") VALUES ('37d99068-f762-4fc1-ba84-0db7096829f7', '5b727539-0a47-480c-869c-e4c89fa8b70e', 'Captación Sur Pasturas', 'Recepción de ternerada de destete.', 'Registrado', 'Optimo', 'Potrero 3 - Gatton Panic', '2026-08-14 01:37:24.420124+00', '00000000-0000-0000-0000-000000000000', '-infinity', NULL, 'Pendiente', NULL, NULL, -17.6495, -63.389);
INSERT INTO public."CaptacionesGanado" ("Id", "EstanciaId", "Nombre", "Observaciones", "Estado", "EstadoSanitario", "Potrero", "Fecha", "CreadoPorUsuarioId", "FechaCreacionLocal", "FechaSincronizacion", "EstadoSync", "ModificadoPorUsuarioId", "FechaModificacion", "Latitud", "Longitud") VALUES ('b3e535eb-e9e7-446f-b761-ada1a99065bd', 'e2596f3b-e4f2-4911-baad-cbfc9bbcf693', 'Captación Cuarentena B', 'Lote en observación post-ingreso.', 'EnPlanificacionFaena', 'EnObservacion', 'Corral de Cuarentena', '2026-08-09 01:37:24.420124+00', '00000000-0000-0000-0000-000000000000', '-infinity', NULL, 'Pendiente', NULL, NULL, -17.7855, -63.1802);
INSERT INTO public."CaptacionesGanado" ("Id", "EstanciaId", "Nombre", "Observaciones", "Estado", "EstadoSanitario", "Potrero", "Fecha", "CreadoPorUsuarioId", "FechaCreacionLocal", "FechaSincronizacion", "EstadoSync", "ModificadoPorUsuarioId", "FechaModificacion", "Latitud", "Longitud") VALUES ('2fa99fdf-a619-42e5-9eaf-491785e4f796', 'd54ef819-a162-4bf0-a722-de50984887aa', 'Romer Mendoza', 'Ganado', 'Registrado', 'Optimo', 'Planta P.I. M40 ', '2026-08-22 00:00:00+00', 'dc9b1c7c-d343-4b18-b9d2-411ddf03e712', '2026-08-22 22:25:20.814+00', NULL, 'Sincronizado', NULL, NULL, -17.836751143488247, -63.22573765202925);


--
-- Data for Name: DetallesLoteGanado; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public."DetallesLoteGanado" ("Id", "CaptacionGanadoId", "Categoria", "Raza", "CantidadCabezas", "PesoPromedioEstimadoKg", "SistemaAlimentacion", "FechaEstimadaFaena", "NotasZootecnicas", "CreadoPor", "CreadoEn", "ActualizadoPor", "ActualizadoEn") VALUES ('615b9c62-68c4-47a4-9a87-b356af90f4be', 'b3e535eb-e9e7-446f-b761-ada1a99065bd', 'Toro', 'Nelore', 6, 620, 'Confinamiento', NULL, 'Reproductores en cuarentena sanitaria de rutina.', 'dc9b1c7c-d343-4b18-b9d2-411ddf03e712', '2026-08-19 01:37:25.001317+00', NULL, NULL);
INSERT INTO public."DetallesLoteGanado" ("Id", "CaptacionGanadoId", "Categoria", "Raza", "CantidadCabezas", "PesoPromedioEstimadoKg", "SistemaAlimentacion", "FechaEstimadaFaena", "NotasZootecnicas", "CreadoPor", "CreadoEn", "ActualizadoPor", "ActualizadoEn") VALUES ('b156815b-f5a1-40b8-9841-fde3e5775fcd', '32c33837-52ad-4676-8180-006379270c01', 'Novillo', 'Brangus', 45, 380, 'SemiConfinamiento', '2026-12-19 01:37:24.420124+00', NULL, 'dc9b1c7c-d343-4b18-b9d2-411ddf03e712', '2026-08-19 01:37:24.998147+00', NULL, NULL);
INSERT INTO public."DetallesLoteGanado" ("Id", "CaptacionGanadoId", "Categoria", "Raza", "CantidadCabezas", "PesoPromedioEstimadoKg", "SistemaAlimentacion", "FechaEstimadaFaena", "NotasZootecnicas", "CreadoPor", "CreadoEn", "ActualizadoPor", "ActualizadoEn") VALUES ('d01129df-4378-4bad-b043-6e116ae65dab', '37d99068-f762-4fc1-ba84-0db7096829f7', 'VacaDescarte', 'Nelore', 12, 410, 'PastoreoLibre', NULL, NULL, 'dc9b1c7c-d343-4b18-b9d2-411ddf03e712', '2026-08-19 01:37:25.001375+00', NULL, NULL);
INSERT INTO public."DetallesLoteGanado" ("Id", "CaptacionGanadoId", "Categoria", "Raza", "CantidadCabezas", "PesoPromedioEstimadoKg", "SistemaAlimentacion", "FechaEstimadaFaena", "NotasZootecnicas", "CreadoPor", "CreadoEn", "ActualizadoPor", "ActualizadoEn") VALUES ('dd2ca3f5-76b2-4aba-ae40-860782cda5f8', '37d99068-f762-4fc1-ba84-0db7096829f7', 'Ternero', 'Cruza Comercial', 60, 160, 'PastoreoLibre', NULL, NULL, 'dc9b1c7c-d343-4b18-b9d2-411ddf03e712', '2026-08-19 01:37:25.001374+00', NULL, NULL);
INSERT INTO public."DetallesLoteGanado" ("Id", "CaptacionGanadoId", "Categoria", "Raza", "CantidadCabezas", "PesoPromedioEstimadoKg", "SistemaAlimentacion", "FechaEstimadaFaena", "NotasZootecnicas", "CreadoPor", "CreadoEn", "ActualizadoPor", "ActualizadoEn") VALUES ('f4023682-de71-4d3c-b7e3-fcbbe5ad1b01', '32c33837-52ad-4676-8180-006379270c01', 'Vaquilla', 'Brahman', 30, 290, 'PastoreoLibre', NULL, NULL, 'dc9b1c7c-d343-4b18-b9d2-411ddf03e712', '2026-08-19 01:37:25.001311+00', NULL, NULL);
INSERT INTO public."DetallesLoteGanado" ("Id", "CaptacionGanadoId", "Categoria", "Raza", "CantidadCabezas", "PesoPromedioEstimadoKg", "SistemaAlimentacion", "FechaEstimadaFaena", "NotasZootecnicas", "CreadoPor", "CreadoEn", "ActualizadoPor", "ActualizadoEn") VALUES ('2b41fcd0-c7c1-45c7-b963-781f13f6a9a4', '2fa99fdf-a619-42e5-9eaf-491785e4f796', 'Novillo', NULL, 20, NULL, 'PastoreoLibre', NULL, NULL, 'dc9b1c7c-d343-4b18-b9d2-411ddf03e712', '2026-08-22 22:25:20.858395+00', NULL, NULL);
INSERT INTO public."DetallesLoteGanado" ("Id", "CaptacionGanadoId", "Categoria", "Raza", "CantidadCabezas", "PesoPromedioEstimadoKg", "SistemaAlimentacion", "FechaEstimadaFaena", "NotasZootecnicas", "CreadoPor", "CreadoEn", "ActualizadoPor", "ActualizadoEn") VALUES ('82afd928-9c65-4b8f-97ce-56bc920cf52a', '2fa99fdf-a619-42e5-9eaf-491785e4f796', 'Toro', NULL, 10, NULL, 'SemiConfinamiento', NULL, NULL, 'dc9b1c7c-d343-4b18-b9d2-411ddf03e712', '2026-08-22 22:25:20.858545+00', NULL, NULL);
INSERT INTO public."DetallesLoteGanado" ("Id", "CaptacionGanadoId", "Categoria", "Raza", "CantidadCabezas", "PesoPromedioEstimadoKg", "SistemaAlimentacion", "FechaEstimadaFaena", "NotasZootecnicas", "CreadoPor", "CreadoEn", "ActualizadoPor", "ActualizadoEn") VALUES ('ade71e00-97cf-47c4-b755-5020344a7c52', '2fa99fdf-a619-42e5-9eaf-491785e4f796', 'Vaquilla', NULL, 50, NULL, 'SemiConfinamiento', NULL, NULL, 'dc9b1c7c-d343-4b18-b9d2-411ddf03e712', '2026-08-22 22:25:20.858544+00', NULL, NULL);


--
-- Data for Name: Dispositivos; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- Data for Name: LogsAuditoria; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public."LogsAuditoria" ("Id", "UsuarioId", "Accion", "Modulo", "IdRegistroAfectado", "FechaHora", "Detalle") VALUES ('5b98c551-bb48-4152-984a-42f45efb1049', 'dc9b1c7c-d343-4b18-b9d2-411ddf03e712', 'Creacion', 'Estancia', 'ba3f31c4-6878-4b5b-98c8-dc6cedaf97fb', '2026-08-17 01:16:45.924194+00', 'Estancia La Esperanza');
INSERT INTO public."LogsAuditoria" ("Id", "UsuarioId", "Accion", "Modulo", "IdRegistroAfectado", "FechaHora", "Detalle") VALUES ('3b5e4f19-10d6-4435-bcf9-cb962b07086f', 'dc9b1c7c-d343-4b18-b9d2-411ddf03e712', 'Creacion', 'CaptacionGanado', '86469c36-bd4e-49b6-b6d1-f28c4cb434d3', '2026-08-17 01:17:09.533789+00', 'Lote Norte A');
INSERT INTO public."LogsAuditoria" ("Id", "UsuarioId", "Accion", "Modulo", "IdRegistroAfectado", "FechaHora", "Detalle") VALUES ('807f64a8-a69c-4182-a7d9-26aefe28f624', 'dc9b1c7c-d343-4b18-b9d2-411ddf03e712', 'Creacion', 'Movimiento', '4f04bac3-f7b1-4ff5-8cdd-3d55f58bc818', '2026-08-18 00:57:01.12762+00', 'Potrero Norte -> Potrero Sur');
INSERT INTO public."LogsAuditoria" ("Id", "UsuarioId", "Accion", "Modulo", "IdRegistroAfectado", "FechaHora", "Detalle") VALUES ('7302ec32-fd00-42c0-88a3-0b5d3a6f2b0d', 'dc9b1c7c-d343-4b18-b9d2-411ddf03e712', 'Creacion', 'Pesaje', '963a94cf-0ee9-45aa-90dc-8689c5225d24', '2026-08-18 00:57:25.66794+00', '390.5 kg');
INSERT INTO public."LogsAuditoria" ("Id", "UsuarioId", "Accion", "Modulo", "IdRegistroAfectado", "FechaHora", "Detalle") VALUES ('a7ff5c05-08b0-4a13-96ce-a6a996dc7284', 'dc9b1c7c-d343-4b18-b9d2-411ddf03e712', 'Creacion', 'Estancia', 'b7800b0a-60c3-4799-8008-ed446581b1cd', '2026-08-21 01:19:51.326639+00', 'Estancia Smoke Test 1787275187437');
INSERT INTO public."LogsAuditoria" ("Id", "UsuarioId", "Accion", "Modulo", "IdRegistroAfectado", "FechaHora", "Detalle") VALUES ('d434b058-902b-400c-8699-d3bc62631a3e', 'dc9b1c7c-d343-4b18-b9d2-411ddf03e712', 'Creacion', 'Estancia', '8e61a54f-ada5-44da-bba8-baaed835ea54', '2026-08-21 01:19:54.190693+00', 'Estancia Smoke Test 1787275187437');
INSERT INTO public."LogsAuditoria" ("Id", "UsuarioId", "Accion", "Modulo", "IdRegistroAfectado", "FechaHora", "Detalle") VALUES ('aa0dbee3-6ec6-49d5-8b53-a35cf8313905', 'dc9b1c7c-d343-4b18-b9d2-411ddf03e712', 'Creacion', 'Estancia', 'e7ecd54e-77ad-41dc-b1d2-c022efbd2ea5', '2026-08-21 01:20:46.176061+00', 'Estancia Smoke desktop 1787275243236');
INSERT INTO public."LogsAuditoria" ("Id", "UsuarioId", "Accion", "Modulo", "IdRegistroAfectado", "FechaHora", "Detalle") VALUES ('b7a0bb7c-b1c0-41fb-a984-f5848404a042', 'dc9b1c7c-d343-4b18-b9d2-411ddf03e712', 'Creacion', 'Estancia', '4cf7b015-10c1-4797-ac8a-d28cdbb69e02', '2026-08-21 01:20:50.067821+00', 'Estancia Smoke mobile 1787275246323');
INSERT INTO public."LogsAuditoria" ("Id", "UsuarioId", "Accion", "Modulo", "IdRegistroAfectado", "FechaHora", "Detalle") VALUES ('8eb3d485-f8ac-49c2-ae7f-da21b91457e0', '4ee7acd6-bf93-48ea-9c4e-7e5c78531596', 'Eliminacion', 'Estancia', '4cf7b015-10c1-4797-ac8a-d28cdbb69e02', '2026-08-21 01:22:07.16863+00', 'Estancia Smoke mobile 1787275246323');
INSERT INTO public."LogsAuditoria" ("Id", "UsuarioId", "Accion", "Modulo", "IdRegistroAfectado", "FechaHora", "Detalle") VALUES ('bb68b235-db00-4005-aee6-f826d38ceddf', '4ee7acd6-bf93-48ea-9c4e-7e5c78531596', 'Eliminacion', 'Estancia', '8e61a54f-ada5-44da-bba8-baaed835ea54', '2026-08-21 01:22:07.305842+00', 'Estancia Smoke Test 1787275187437');
INSERT INTO public."LogsAuditoria" ("Id", "UsuarioId", "Accion", "Modulo", "IdRegistroAfectado", "FechaHora", "Detalle") VALUES ('36b35639-b9d3-4819-863b-fb09e318ea97', '4ee7acd6-bf93-48ea-9c4e-7e5c78531596', 'Eliminacion', 'Estancia', 'b7800b0a-60c3-4799-8008-ed446581b1cd', '2026-08-21 01:22:07.433627+00', 'Estancia Smoke Test 1787275187437');
INSERT INTO public."LogsAuditoria" ("Id", "UsuarioId", "Accion", "Modulo", "IdRegistroAfectado", "FechaHora", "Detalle") VALUES ('2955a268-5acb-48ad-98a9-9d75bfddd8fa', '4ee7acd6-bf93-48ea-9c4e-7e5c78531596', 'Eliminacion', 'Estancia', 'e7ecd54e-77ad-41dc-b1d2-c022efbd2ea5', '2026-08-21 01:22:07.581124+00', 'Estancia Smoke desktop 1787275243236');
INSERT INTO public."LogsAuditoria" ("Id", "UsuarioId", "Accion", "Modulo", "IdRegistroAfectado", "FechaHora", "Detalle") VALUES ('2cfea594-e3f0-4aae-86db-72d2deca98ff', 'dc9b1c7c-d343-4b18-b9d2-411ddf03e712', 'Creacion', 'Estancia', '583b9f41-40e7-436f-9586-0296c08567c6', '2026-08-21 01:30:01.962102+00', 'Estancia Para Eliminar 1787275801');
INSERT INTO public."LogsAuditoria" ("Id", "UsuarioId", "Accion", "Modulo", "IdRegistroAfectado", "FechaHora", "Detalle") VALUES ('d8e03bcf-ebe9-4b38-993e-79eca8105a00', '4ee7acd6-bf93-48ea-9c4e-7e5c78531596', 'Eliminacion', 'Estancia', '583b9f41-40e7-436f-9586-0296c08567c6', '2026-08-21 01:30:35.351068+00', 'Estancia Para Eliminar 1787275801');
INSERT INTO public."LogsAuditoria" ("Id", "UsuarioId", "Accion", "Modulo", "IdRegistroAfectado", "FechaHora", "Detalle") VALUES ('5c84bd50-34f4-464d-85d4-41bc1ed6a496', 'dc9b1c7c-d343-4b18-b9d2-411ddf03e712', 'Creacion', 'CaptacionGanado', 'c5c41439-adb8-449b-bd55-6cf1fc0aa308', '2026-08-21 01:40:54.386356+00', 'Captación Smoke desktop 1787276449701');
INSERT INTO public."LogsAuditoria" ("Id", "UsuarioId", "Accion", "Modulo", "IdRegistroAfectado", "FechaHora", "Detalle") VALUES ('a9e07c79-122f-4346-be5d-a0c890947f3a', 'dc9b1c7c-d343-4b18-b9d2-411ddf03e712', 'Creacion', 'CaptacionGanado', 'c796bcd9-fc6d-404e-85f3-c23c48c6cd69', '2026-08-21 01:40:59.827647+00', 'Captación Smoke mobile 1787276454655');
INSERT INTO public."LogsAuditoria" ("Id", "UsuarioId", "Accion", "Modulo", "IdRegistroAfectado", "FechaHora", "Detalle") VALUES ('59ee2e19-45cc-4649-8df7-0f21fea2771e', '4ee7acd6-bf93-48ea-9c4e-7e5c78531596', 'Eliminacion', 'CaptacionGanado', 'c5c41439-adb8-449b-bd55-6cf1fc0aa308', '2026-08-21 01:41:35.884152+00', 'Captación Smoke desktop 1787276449701');
INSERT INTO public."LogsAuditoria" ("Id", "UsuarioId", "Accion", "Modulo", "IdRegistroAfectado", "FechaHora", "Detalle") VALUES ('0fb8ec41-a308-43c5-904c-7a70d6ece045', '4ee7acd6-bf93-48ea-9c4e-7e5c78531596', 'Eliminacion', 'CaptacionGanado', 'c796bcd9-fc6d-404e-85f3-c23c48c6cd69', '2026-08-21 01:41:36.063296+00', 'Captación Smoke mobile 1787276454655');
INSERT INTO public."LogsAuditoria" ("Id", "UsuarioId", "Accion", "Modulo", "IdRegistroAfectado", "FechaHora", "Detalle") VALUES ('93178b35-2bb5-4af5-add1-3ffe1a440b02', 'dc9b1c7c-d343-4b18-b9d2-411ddf03e712', 'Creacion', 'CaptacionGanado', '731033df-3ea1-4829-bb8d-272c98a3c005', '2026-08-21 01:52:23.765066+00', 'Captacion Bitacoras Smoke');
INSERT INTO public."LogsAuditoria" ("Id", "UsuarioId", "Accion", "Modulo", "IdRegistroAfectado", "FechaHora", "Detalle") VALUES ('bd6a9015-b7e4-40c6-935b-73f2ad07b892', 'dc9b1c7c-d343-4b18-b9d2-411ddf03e712', 'Creacion', 'Pesaje', 'bd189a51-52f0-463f-85c4-a23883574532', '2026-08-21 01:53:10.088839+00', '385 kg');
INSERT INTO public."LogsAuditoria" ("Id", "UsuarioId", "Accion", "Modulo", "IdRegistroAfectado", "FechaHora", "Detalle") VALUES ('5c6ae4b8-8f1f-409a-a5f5-5e2e91260656', 'dc9b1c7c-d343-4b18-b9d2-411ddf03e712', 'Creacion', 'Pesaje', '35ab08d9-e7de-4096-8fc9-7c0277b5b53a', '2026-08-21 01:54:11.380777+00', '385 kg');
INSERT INTO public."LogsAuditoria" ("Id", "UsuarioId", "Accion", "Modulo", "IdRegistroAfectado", "FechaHora", "Detalle") VALUES ('1adfdd16-380c-42b7-be32-cf651328e2ee', 'dc9b1c7c-d343-4b18-b9d2-411ddf03e712', 'Creacion', 'Sanitario', 'aac10792-2804-4ef2-a921-31b3e4794345', '2026-08-21 01:54:12.362427+00', 'Vacuna Aftosa');
INSERT INTO public."LogsAuditoria" ("Id", "UsuarioId", "Accion", "Modulo", "IdRegistroAfectado", "FechaHora", "Detalle") VALUES ('3b4668a8-6dde-4f26-8b9f-ee8a0ae08cd3', 'dc9b1c7c-d343-4b18-b9d2-411ddf03e712', 'Creacion', 'Pesaje', '5a70ed3c-e61a-4437-93a6-ced0f7bab6e4', '2026-08-21 01:55:05.526932+00', '385 kg');
INSERT INTO public."LogsAuditoria" ("Id", "UsuarioId", "Accion", "Modulo", "IdRegistroAfectado", "FechaHora", "Detalle") VALUES ('696ddaf8-3a60-4995-ab77-1af4352cff39', 'dc9b1c7c-d343-4b18-b9d2-411ddf03e712', 'Creacion', 'Sanitario', '5832fa21-c0d6-4997-bf21-c5deb3b174a2', '2026-08-21 01:55:06.346789+00', 'Vacuna Aftosa');
INSERT INTO public."LogsAuditoria" ("Id", "UsuarioId", "Accion", "Modulo", "IdRegistroAfectado", "FechaHora", "Detalle") VALUES ('6b3572c5-93e6-45ce-b2db-27390db9ced7', 'dc9b1c7c-d343-4b18-b9d2-411ddf03e712', 'Creacion', 'Movimiento', '0de0f102-cc8f-430f-bc4e-c51f016d19e2', '2026-08-21 01:55:07.244089+00', 'Corral de Recepción -> Potrero Smoke 1');
INSERT INTO public."LogsAuditoria" ("Id", "UsuarioId", "Accion", "Modulo", "IdRegistroAfectado", "FechaHora", "Detalle") VALUES ('9588624d-5fa4-43d2-9982-d46ba6cb3fb9', 'dc9b1c7c-d343-4b18-b9d2-411ddf03e712', 'Creacion', 'Alimentacion', '31cc0950-e919-44b2-ae39-425cc5747e1a', '2026-08-21 01:55:08.344316+00', NULL);
INSERT INTO public."LogsAuditoria" ("Id", "UsuarioId", "Accion", "Modulo", "IdRegistroAfectado", "FechaHora", "Detalle") VALUES ('308bba98-d7af-46f4-851f-2c865c8046a4', '4ee7acd6-bf93-48ea-9c4e-7e5c78531596', 'Eliminacion', 'CaptacionGanado', '731033df-3ea1-4829-bb8d-272c98a3c005', '2026-08-21 01:55:46.054684+00', 'Captacion Bitacoras Smoke');
INSERT INTO public."LogsAuditoria" ("Id", "UsuarioId", "Accion", "Modulo", "IdRegistroAfectado", "FechaHora", "Detalle") VALUES ('39fe35a0-c6b5-4ef0-92c9-2fb557fa58ef', 'dc9b1c7c-d343-4b18-b9d2-411ddf03e712', 'Creacion', 'Estancia', 'd54ef819-a162-4bf0-a722-de50984887aa', '2026-08-21 02:00:44.035913+00', 'San Paola');
INSERT INTO public."LogsAuditoria" ("Id", "UsuarioId", "Accion", "Modulo", "IdRegistroAfectado", "FechaHora", "Detalle") VALUES ('2fd49c7a-1985-4806-ae2b-a93de77b2274', '4ee7acd6-bf93-48ea-9c4e-7e5c78531596', 'Creacion', 'Usuario', 'ca3eba47-a783-444d-92b9-e6798bb78b9f', '2026-08-21 02:52:41.777389+00', 'smoke1787280760657@siga.com');
INSERT INTO public."LogsAuditoria" ("Id", "UsuarioId", "Accion", "Modulo", "IdRegistroAfectado", "FechaHora", "Detalle") VALUES ('920bcd7c-0ab7-48a5-8f2f-4cc9841e539b', '4ee7acd6-bf93-48ea-9c4e-7e5c78531596', 'Modificacion', 'Usuario', 'dc9b1c7c-d343-4b18-b9d2-411ddf03e712', '2026-08-21 02:57:12.906326+00', 'captador@siga.com');
INSERT INTO public."LogsAuditoria" ("Id", "UsuarioId", "Accion", "Modulo", "IdRegistroAfectado", "FechaHora", "Detalle") VALUES ('5a825e6f-4e4b-486e-b69b-ad86e2f197ab', '4ee7acd6-bf93-48ea-9c4e-7e5c78531596', 'Modificacion', 'Usuario', 'dc9b1c7c-d343-4b18-b9d2-411ddf03e712', '2026-08-21 02:57:33.380806+00', 'captador@siga.com');
INSERT INTO public."LogsAuditoria" ("Id", "UsuarioId", "Accion", "Modulo", "IdRegistroAfectado", "FechaHora", "Detalle") VALUES ('9bb060b9-9c08-4519-87e6-68e6de0b4de5', 'dc9b1c7c-d343-4b18-b9d2-411ddf03e712', 'Creacion', 'CaptacionGanado', '269c68ae-e9dc-4e39-ba16-78c52024ffb1', '2026-08-22 01:29:30.808995+00', 'TEMP - Prueba Eliminar UI');
INSERT INTO public."LogsAuditoria" ("Id", "UsuarioId", "Accion", "Modulo", "IdRegistroAfectado", "FechaHora", "Detalle") VALUES ('3125a4dd-78c8-437e-8548-d13ef17ffac1', '4ee7acd6-bf93-48ea-9c4e-7e5c78531596', 'Eliminacion', 'CaptacionGanado', '269c68ae-e9dc-4e39-ba16-78c52024ffb1', '2026-08-22 01:30:29.783899+00', 'TEMP - Prueba Eliminar UI');
INSERT INTO public."LogsAuditoria" ("Id", "UsuarioId", "Accion", "Modulo", "IdRegistroAfectado", "FechaHora", "Detalle") VALUES ('cf08282a-4966-4a5a-a7b0-d528ceaa47aa', 'dc9b1c7c-d343-4b18-b9d2-411ddf03e712', 'Creacion', 'CaptacionGanado', '2fa99fdf-a619-42e5-9eaf-491785e4f796', '2026-08-22 22:25:21.136864+00', 'Romer Mendoza');
INSERT INTO public."LogsAuditoria" ("Id", "UsuarioId", "Accion", "Modulo", "IdRegistroAfectado", "FechaHora", "Detalle") VALUES ('96de1045-c8d1-41d0-92fa-2511cedb3c40', 'dc9b1c7c-d343-4b18-b9d2-411ddf03e712', 'Creacion', 'Pesaje', '6ba3e9ac-0268-4ac9-a561-46b69c4bb1a2', '2026-08-22 22:25:43.648201+00', '400 kg');
INSERT INTO public."LogsAuditoria" ("Id", "UsuarioId", "Accion", "Modulo", "IdRegistroAfectado", "FechaHora", "Detalle") VALUES ('b4ba2d7f-9e62-40f7-9114-7c43eaaf68f4', 'dc9b1c7c-d343-4b18-b9d2-411ddf03e712', 'Creacion', 'Pesaje', '1cb25c64-d277-44ff-9cef-954a26980c57', '2026-08-22 22:26:11.460237+00', '250 kg');
INSERT INTO public."LogsAuditoria" ("Id", "UsuarioId", "Accion", "Modulo", "IdRegistroAfectado", "FechaHora", "Detalle") VALUES ('27c8415f-ef7c-4adb-b246-110438f3a3b4', 'dc9b1c7c-d343-4b18-b9d2-411ddf03e712', 'Creacion', 'Movimiento', 'e3d3929a-e6f1-400a-87e4-e3eea268dd97', '2026-08-22 22:27:04.297879+00', 'Origen -> Planta P.I. M40 ');


--
-- Data for Name: MovimientosGanado; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public."MovimientosGanado" ("Id", "CaptacionGanadoId", "Fecha", "TipoGanado", "CantidadCabezas", "Origen", "Destino", "CreadoPorUsuarioId", "FechaCreacionLocal", "FechaSincronizacion", "EstadoSync", "ModificadoPorUsuarioId", "FechaModificacion") VALUES ('543c295f-ced0-4486-8b93-882bdd3ac031', '37d99068-f762-4fc1-ba84-0db7096829f7', '2026-08-17 01:37:24.420124+00', 'Ternero', 60, 'Corral de Recepción', 'Potrero 3 - Gatton Panic', 'dc9b1c7c-d343-4b18-b9d2-411ddf03e712', '2026-08-17 01:37:24.420124+00', NULL, 'Sincronizado', NULL, NULL);
INSERT INTO public."MovimientosGanado" ("Id", "CaptacionGanadoId", "Fecha", "TipoGanado", "CantidadCabezas", "Origen", "Destino", "CreadoPorUsuarioId", "FechaCreacionLocal", "FechaSincronizacion", "EstadoSync", "ModificadoPorUsuarioId", "FechaModificacion") VALUES ('e3d3929a-e6f1-400a-87e4-e3eea268dd97', '2fa99fdf-a619-42e5-9eaf-491785e4f796', '2026-11-11 00:00:00+00', 'Toro', 200, 'Origen', 'Planta P.I. M40 ', 'dc9b1c7c-d343-4b18-b9d2-411ddf03e712', '2026-08-22 22:27:04.192+00', NULL, 'Sincronizado', NULL, NULL);


--
-- Data for Name: PermisosUsuario; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- Data for Name: RegistrosAlimentacion; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public."RegistrosAlimentacion" ("Id", "CaptacionGanadoId", "Fecha", "TipoAlimentacion", "RacionBaseKgAnimal", "SuplementoProteicoKgAnimal", "Observaciones", "CreadoPorUsuarioId", "FechaCreacionLocal", "FechaSincronizacion", "EstadoSync", "ModificadoPorUsuarioId", "FechaModificacion") VALUES ('f67d501c-51fe-440a-b59c-fd49842dcce2', '32c33837-52ad-4676-8180-006379270c01', '2026-08-04 01:37:24.420124+00', 'SemiConfinamiento', 8.5, 1.2, 'Ración balanceada de engorde.', 'dc9b1c7c-d343-4b18-b9d2-411ddf03e712', '2026-08-04 01:37:24.420124+00', NULL, 'Sincronizado', NULL, NULL);


--
-- Data for Name: RegistrosPesaje; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public."RegistrosPesaje" ("Id", "CaptacionGanadoId", "Fecha", "PesoPromedioKg", "CantidadCabezasPesadas", "Observaciones", "CreadoPorUsuarioId", "FechaCreacionLocal", "FechaSincronizacion", "EstadoSync", "ModificadoPorUsuarioId", "FechaModificacion") VALUES ('2bc1ef3b-344a-4d10-8bf1-dbecf04a9c06', '32c33837-52ad-4676-8180-006379270c01', '2026-08-14 01:37:24.420124+00', 385, NULL, 'Pesaje parcial de grupo.', 'dc9b1c7c-d343-4b18-b9d2-411ddf03e712', '2026-08-14 01:37:24.420124+00', NULL, 'Sincronizado', NULL, NULL);
INSERT INTO public."RegistrosPesaje" ("Id", "CaptacionGanadoId", "Fecha", "PesoPromedioKg", "CantidadCabezasPesadas", "Observaciones", "CreadoPorUsuarioId", "FechaCreacionLocal", "FechaSincronizacion", "EstadoSync", "ModificadoPorUsuarioId", "FechaModificacion") VALUES ('78563420-1fd6-45b8-a11b-97faa12fa88d', '32c33837-52ad-4676-8180-006379270c01', '2026-07-30 01:37:24.420124+00', 350.2, 45, 'Control de peso mensual.', 'dc9b1c7c-d343-4b18-b9d2-411ddf03e712', '2026-07-30 01:37:24.420124+00', NULL, 'Sincronizado', NULL, NULL);
INSERT INTO public."RegistrosPesaje" ("Id", "CaptacionGanadoId", "Fecha", "PesoPromedioKg", "CantidadCabezasPesadas", "Observaciones", "CreadoPorUsuarioId", "FechaCreacionLocal", "FechaSincronizacion", "EstadoSync", "ModificadoPorUsuarioId", "FechaModificacion") VALUES ('6ba3e9ac-0268-4ac9-a561-46b69c4bb1a2', '2fa99fdf-a619-42e5-9eaf-491785e4f796', '2026-08-22 00:00:00+00', 400, NULL, NULL, 'dc9b1c7c-d343-4b18-b9d2-411ddf03e712', '2026-08-22 22:25:43.547+00', NULL, 'Sincronizado', NULL, NULL);
INSERT INTO public."RegistrosPesaje" ("Id", "CaptacionGanadoId", "Fecha", "PesoPromedioKg", "CantidadCabezasPesadas", "Observaciones", "CreadoPorUsuarioId", "FechaCreacionLocal", "FechaSincronizacion", "EstadoSync", "ModificadoPorUsuarioId", "FechaModificacion") VALUES ('1cb25c64-d277-44ff-9cef-954a26980c57', '2fa99fdf-a619-42e5-9eaf-491785e4f796', '2026-08-22 00:00:00+00', 250, 80, NULL, 'dc9b1c7c-d343-4b18-b9d2-411ddf03e712', '2026-08-22 22:26:11.42+00', NULL, 'Sincronizado', NULL, NULL);


--
-- Data for Name: RegistrosSanitarios; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public."RegistrosSanitarios" ("Id", "CaptacionGanadoId", "Fecha", "TipoEvento", "ProductoTratamiento", "RegistradoPorUsuarioId", "Observaciones", "CreadoPorUsuarioId", "FechaCreacionLocal", "FechaSincronizacion", "EstadoSync", "ModificadoPorUsuarioId", "FechaModificacion") VALUES ('aaf34245-c727-4063-96c2-52a4912c5ce4', 'b3e535eb-e9e7-446f-b761-ada1a99065bd', '2026-08-10 01:37:24.420124+00', 'Vacunacion', 'Vacuna Aftosa', 'dc9b1c7c-d343-4b18-b9d2-411ddf03e712', NULL, 'dc9b1c7c-d343-4b18-b9d2-411ddf03e712', '2026-08-10 01:37:24.420124+00', NULL, 'Sincronizado', NULL, NULL);
INSERT INTO public."RegistrosSanitarios" ("Id", "CaptacionGanadoId", "Fecha", "TipoEvento", "ProductoTratamiento", "RegistradoPorUsuarioId", "Observaciones", "CreadoPorUsuarioId", "FechaCreacionLocal", "FechaSincronizacion", "EstadoSync", "ModificadoPorUsuarioId", "FechaModificacion") VALUES ('be9ba303-886b-4028-8911-cc2b3c755c16', 'b3e535eb-e9e7-446f-b761-ada1a99065bd', '2026-08-16 01:37:24.420124+00', 'ControlRutina', NULL, 'dc9b1c7c-d343-4b18-b9d2-411ddf03e712', 'Sin novedades.', 'dc9b1c7c-d343-4b18-b9d2-411ddf03e712', '2026-08-16 01:37:24.420124+00', NULL, 'Pendiente', NULL, NULL);


--
-- Data for Name: SectoresAsignados; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public."SectoresAsignados" ("Id", "UsuarioId", "NombreSector", "Zona") VALUES ('8407ba05-c79e-4d4e-8af5-096da95b3291', 'dc9b1c7c-d343-4b18-b9d2-411ddf03e712', 'Sector Cuarentena', 'Zona C');
INSERT INTO public."SectoresAsignados" ("Id", "UsuarioId", "NombreSector", "Zona") VALUES ('c8219e3b-f2b2-4830-8363-1f27e20ccf14', 'c91ce5ff-1f1c-42fc-ac21-0899f2e76561', 'Sector Sur', 'Zona B');
INSERT INTO public."SectoresAsignados" ("Id", "UsuarioId", "NombreSector", "Zona") VALUES ('e2cbbef9-fa7e-46ad-8b4c-c43d637bf9f7', 'dc9b1c7c-d343-4b18-b9d2-411ddf03e712', 'Sector Norte', 'Zona A');


--

COMMIT;
