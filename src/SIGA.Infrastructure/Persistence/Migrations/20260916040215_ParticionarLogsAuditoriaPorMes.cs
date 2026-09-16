using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIGA.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ParticionarLogsAuditoriaPorMes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // LogAuditoria es append-only (solo Add + SaveChanges, nunca Update/Find por Id
            // — confirmado en AuditoriaService/LogAuditoriaRepository antes de escribir esta
            // migración), así que el desajuste entre la PK que EF cree que existe (Id) y la
            // PK real en BD (Id, FechaHora) es seguro en la práctica. Si en algún momento se
            // agrega un lookup por Id sobre esta tabla, revisar esto primero.
            //
            // A propósito NO se borra "LogsAuditoria_legacy" al final de esta migración: se
            // deja como respaldo de rollback una vez aplicado en dev/staging, hasta confirmar
            // visualmente (ej. GET /api/auditoria) que todo se copió bien. Borrarla es un paso
            // manual explícito, no automático.
            migrationBuilder.Sql(
                """
                ALTER TABLE "LogsAuditoria" RENAME TO "LogsAuditoria_legacy";
                ALTER TABLE "LogsAuditoria_legacy" RENAME CONSTRAINT "PK_LogsAuditoria" TO "PK_LogsAuditoria_legacy";
                ALTER TABLE "LogsAuditoria_legacy" RENAME CONSTRAINT "FK_LogsAuditoria_Usuarios_UsuarioId" TO "FK_LogsAuditoria_legacy_Usuarios_UsuarioId";
                ALTER INDEX "IX_LogsAuditoria_FechaHora" RENAME TO "IX_LogsAuditoria_legacy_FechaHora";
                ALTER INDEX "IX_LogsAuditoria_UsuarioId" RENAME TO "IX_LogsAuditoria_legacy_UsuarioId";

                CREATE TABLE "LogsAuditoria" (
                    "Id" uuid NOT NULL,
                    "UsuarioId" uuid NOT NULL,
                    "Accion" character varying(20) NOT NULL,
                    "Modulo" character varying(100) NOT NULL,
                    "IdRegistroAfectado" uuid NOT NULL,
                    "FechaHora" timestamp with time zone NOT NULL,
                    "Detalle" character varying(500),
                    CONSTRAINT "PK_LogsAuditoria" PRIMARY KEY ("Id", "FechaHora")
                ) PARTITION BY RANGE ("FechaHora");

                CREATE TABLE "LogsAuditoria_default" PARTITION OF "LogsAuditoria" DEFAULT;

                -- Particiones mensuales para un rango amplio (2026-01 a 2027-12); lo que caiga
                -- fuera va a la partición DEFAULT de arriba.
                DO $$
                DECLARE
                    mes date := DATE '2026-01-01';
                BEGIN
                    WHILE mes < DATE '2028-01-01' LOOP
                        EXECUTE format(
                            'CREATE TABLE %I PARTITION OF "LogsAuditoria" FOR VALUES FROM (%L) TO (%L);',
                            'LogsAuditoria_' || to_char(mes, 'YYYY_MM'),
                            mes,
                            mes + INTERVAL '1 month'
                        );
                        mes := mes + INTERVAL '1 month';
                    END LOOP;
                END $$;

                INSERT INTO "LogsAuditoria" ("Id", "UsuarioId", "Accion", "Modulo", "IdRegistroAfectado", "FechaHora", "Detalle")
                SELECT "Id", "UsuarioId", "Accion", "Modulo", "IdRegistroAfectado", "FechaHora", "Detalle"
                FROM "LogsAuditoria_legacy";

                -- Si el conteo no coincide, se aborta la migración completa (transaccional) en
                -- vez de dejar una copia parcial.
                DO $$
                DECLARE
                    conteo_legacy bigint := (SELECT count(*) FROM "LogsAuditoria_legacy");
                    conteo_nuevo bigint := (SELECT count(*) FROM "LogsAuditoria");
                BEGIN
                    IF conteo_legacy <> conteo_nuevo THEN
                        RAISE EXCEPTION 'Particionamiento de LogsAuditoria: conteo no coincide (legacy=%, nuevo=%)', conteo_legacy, conteo_nuevo;
                    END IF;
                END $$;

                CREATE INDEX "IX_LogsAuditoria_FechaHora" ON "LogsAuditoria" ("FechaHora");
                CREATE INDEX "IX_LogsAuditoria_UsuarioId" ON "LogsAuditoria" ("UsuarioId");

                ALTER TABLE "LogsAuditoria"
                    ADD CONSTRAINT "FK_LogsAuditoria_Usuarios_UsuarioId"
                    FOREIGN KEY ("UsuarioId") REFERENCES "Usuarios" ("Id") ON DELETE RESTRICT;

                -- Mantenimiento: crea la partición del mes siguiente si todavía no existe. No
                -- se programa automáticamente aquí (pg_cron no está disponible en Render free
                -- tier) — correrla manualmente o desde un job del backend antes de que se
                -- acaben las particiones pre-creadas (hay margen hasta 2027-12).
                CREATE OR REPLACE FUNCTION crear_particion_logs_auditoria_siguiente_mes() RETURNS void AS $$
                DECLARE
                    inicio date := date_trunc('month', now() + interval '1 month');
                    fin date := inicio + interval '1 month';
                    nombre text := 'LogsAuditoria_' || to_char(inicio, 'YYYY_MM');
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM pg_class WHERE relname = nombre) THEN
                        EXECUTE format(
                            'CREATE TABLE %I PARTITION OF "LogsAuditoria" FOR VALUES FROM (%L) TO (%L);',
                            nombre, inicio, fin
                        );
                    END IF;
                END;
                $$ LANGUAGE plpgsql;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DROP FUNCTION IF EXISTS crear_particion_logs_auditoria_siguiente_mes();
                DROP TABLE IF EXISTS "LogsAuditoria" CASCADE;

                ALTER TABLE "LogsAuditoria_legacy" RENAME TO "LogsAuditoria";
                ALTER TABLE "LogsAuditoria" RENAME CONSTRAINT "PK_LogsAuditoria_legacy" TO "PK_LogsAuditoria";
                ALTER TABLE "LogsAuditoria" RENAME CONSTRAINT "FK_LogsAuditoria_legacy_Usuarios_UsuarioId" TO "FK_LogsAuditoria_Usuarios_UsuarioId";
                ALTER INDEX "IX_LogsAuditoria_legacy_FechaHora" RENAME TO "IX_LogsAuditoria_FechaHora";
                ALTER INDEX "IX_LogsAuditoria_legacy_UsuarioId" RENAME TO "IX_LogsAuditoria_UsuarioId";
                """);
        }
    }
}
