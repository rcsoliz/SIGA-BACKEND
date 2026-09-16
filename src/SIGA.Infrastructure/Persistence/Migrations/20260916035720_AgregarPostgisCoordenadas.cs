using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIGA.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AgregarPostgisCoordenadas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Alcance a propósito: solo BD, sin tocar el modelo EF ni agregar
            // NetTopologySuite. La app .NET sigue leyendo/escribiendo Latitud/Longitud
            // exactamente igual que antes — esta columna se mantiene sincronizada por
            // trigger, para permitir consultas espaciales reales (ST_DWithin, etc.) desde
            // SQL directo cuando se necesiten.
            //
            // Los CHECK de rango de Latitud/Longitud en Estancias y CaptacionesGanado ya
            // existen desde la migración AgregarTrazabilidadFksYValidaciones (Fase 1) —
            // no se repiten aquí.
            migrationBuilder.Sql(
                """
                CREATE EXTENSION IF NOT EXISTS postgis;

                ALTER TABLE "Estancias" ADD COLUMN "Ubicacion" geography(Point, 4326);
                ALTER TABLE "CaptacionesGanado" ADD COLUMN "Ubicacion" geography(Point, 4326);

                CREATE OR REPLACE FUNCTION siga_actualizar_ubicacion_geografica() RETURNS trigger AS $$
                BEGIN
                    IF NEW."Latitud" IS NOT NULL AND NEW."Longitud" IS NOT NULL THEN
                        NEW."Ubicacion" := ST_SetSRID(ST_MakePoint(NEW."Longitud", NEW."Latitud"), 4326)::geography;
                    ELSE
                        NEW."Ubicacion" := NULL;
                    END IF;
                    RETURN NEW;
                END;
                $$ LANGUAGE plpgsql;

                CREATE TRIGGER trg_estancias_ubicacion
                    BEFORE INSERT OR UPDATE OF "Latitud", "Longitud" ON "Estancias"
                    FOR EACH ROW EXECUTE FUNCTION siga_actualizar_ubicacion_geografica();

                CREATE TRIGGER trg_captacionesganado_ubicacion
                    BEFORE INSERT OR UPDATE OF "Latitud", "Longitud" ON "CaptacionesGanado"
                    FOR EACH ROW EXECUTE FUNCTION siga_actualizar_ubicacion_geografica();

                -- Backfill de filas ya existentes (los triggers de arriba solo aplican hacia adelante).
                UPDATE "Estancias"
                SET "Ubicacion" = ST_SetSRID(ST_MakePoint("Longitud", "Latitud"), 4326)::geography
                WHERE "Latitud" IS NOT NULL AND "Longitud" IS NOT NULL;

                UPDATE "CaptacionesGanado"
                SET "Ubicacion" = ST_SetSRID(ST_MakePoint("Longitud", "Latitud"), 4326)::geography
                WHERE "Latitud" IS NOT NULL AND "Longitud" IS NOT NULL;

                CREATE INDEX "IX_Estancias_Ubicacion" ON "Estancias" USING GIST ("Ubicacion");
                CREATE INDEX "IX_CaptacionesGanado_Ubicacion" ON "CaptacionesGanado" USING GIST ("Ubicacion");
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DROP INDEX IF EXISTS "IX_Estancias_Ubicacion";
                DROP INDEX IF EXISTS "IX_CaptacionesGanado_Ubicacion";

                DROP TRIGGER IF EXISTS trg_estancias_ubicacion ON "Estancias";
                DROP TRIGGER IF EXISTS trg_captacionesganado_ubicacion ON "CaptacionesGanado";
                DROP FUNCTION IF EXISTS siga_actualizar_ubicacion_geografica();

                ALTER TABLE "Estancias" DROP COLUMN IF EXISTS "Ubicacion";
                ALTER TABLE "CaptacionesGanado" DROP COLUMN IF EXISTS "Ubicacion";
                """);
        }
    }
}
