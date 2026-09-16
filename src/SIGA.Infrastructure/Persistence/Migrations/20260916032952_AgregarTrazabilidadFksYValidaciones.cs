using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIGA.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AgregarTrazabilidadFksYValidaciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_RegistrosSanitarios_CreadoPorUsuarioId",
                table: "RegistrosSanitarios",
                column: "CreadoPorUsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosSanitarios_ModificadoPorUsuarioId",
                table: "RegistrosSanitarios",
                column: "ModificadoPorUsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosPesaje_CreadoPorUsuarioId",
                table: "RegistrosPesaje",
                column: "CreadoPorUsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosPesaje_ModificadoPorUsuarioId",
                table: "RegistrosPesaje",
                column: "ModificadoPorUsuarioId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_RegistrosPesaje_CantidadCabezasPesadas",
                table: "RegistrosPesaje",
                sql: "\"CantidadCabezasPesadas\" IS NULL OR \"CantidadCabezasPesadas\" > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_RegistrosPesaje_PesoPromedioKg",
                table: "RegistrosPesaje",
                sql: "\"PesoPromedioKg\" > 0");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosAlimentacion_CreadoPorUsuarioId",
                table: "RegistrosAlimentacion",
                column: "CreadoPorUsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosAlimentacion_ModificadoPorUsuarioId",
                table: "RegistrosAlimentacion",
                column: "ModificadoPorUsuarioId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_RegistrosAlimentacion_RacionBaseKgAnimal",
                table: "RegistrosAlimentacion",
                sql: "\"RacionBaseKgAnimal\" IS NULL OR \"RacionBaseKgAnimal\" >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_RegistrosAlimentacion_SuplementoProteicoKgAnimal",
                table: "RegistrosAlimentacion",
                sql: "\"SuplementoProteicoKgAnimal\" IS NULL OR \"SuplementoProteicoKgAnimal\" >= 0");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosGanado_CreadoPorUsuarioId",
                table: "MovimientosGanado",
                column: "CreadoPorUsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosGanado_ModificadoPorUsuarioId",
                table: "MovimientosGanado",
                column: "ModificadoPorUsuarioId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_MovimientosGanado_CantidadCabezas",
                table: "MovimientosGanado",
                sql: "\"CantidadCabezas\" > 0");

            migrationBuilder.CreateIndex(
                name: "IX_Estancias_CreadoPorUsuarioId",
                table: "Estancias",
                column: "CreadoPorUsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Estancias_ModificadoPorUsuarioId",
                table: "Estancias",
                column: "ModificadoPorUsuarioId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Estancias_HectareasTotales",
                table: "Estancias",
                sql: "\"HectareasTotales\" IS NULL OR \"HectareasTotales\" > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Estancias_Latitud",
                table: "Estancias",
                sql: "\"Latitud\" BETWEEN -90 AND 90");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Estancias_Longitud",
                table: "Estancias",
                sql: "\"Longitud\" BETWEEN -180 AND 180");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesLoteGanado_ActualizadoPor",
                table: "DetallesLoteGanado",
                column: "ActualizadoPor");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesLoteGanado_CreadoPor",
                table: "DetallesLoteGanado",
                column: "CreadoPor");

            migrationBuilder.AddCheckConstraint(
                name: "CK_DetallesLoteGanado_CantidadCabezas",
                table: "DetallesLoteGanado",
                sql: "\"CantidadCabezas\" > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_DetallesLoteGanado_PesoPromedioEstimadoKg",
                table: "DetallesLoteGanado",
                sql: "\"PesoPromedioEstimadoKg\" IS NULL OR \"PesoPromedioEstimadoKg\" > 0");

            migrationBuilder.CreateIndex(
                name: "IX_CaptacionesGanado_CreadoPorUsuarioId",
                table: "CaptacionesGanado",
                column: "CreadoPorUsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_CaptacionesGanado_ModificadoPorUsuarioId",
                table: "CaptacionesGanado",
                column: "ModificadoPorUsuarioId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_CaptacionesGanado_Latitud",
                table: "CaptacionesGanado",
                sql: "\"Latitud\" IS NULL OR \"Latitud\" BETWEEN -90 AND 90");

            migrationBuilder.AddCheckConstraint(
                name: "CK_CaptacionesGanado_Longitud",
                table: "CaptacionesGanado",
                sql: "\"Longitud\" IS NULL OR \"Longitud\" BETWEEN -180 AND 180");

            // Backfill de filas creadas antes de que el seed setee CreadoPorUsuarioId
            // (bug corregido en DbInitializer.cs) — sin esto, agregar la FK de abajo
            // fallaría contra cualquier BD de dev con datos previos a esta migración.
            migrationBuilder.Sql(
                """
                UPDATE "Estancias"
                SET "CreadoPorUsuarioId" = COALESCE(
                    (SELECT "Id" FROM "Usuarios" WHERE "Email" = 'captador@siga.com'),
                    (SELECT "Id" FROM "Usuarios" ORDER BY "FechaCreacion" LIMIT 1))
                WHERE "CreadoPorUsuarioId" = '00000000-0000-0000-0000-000000000000';

                UPDATE "CaptacionesGanado"
                SET "CreadoPorUsuarioId" = COALESCE(
                    (SELECT "Id" FROM "Usuarios" WHERE "Email" = 'captador@siga.com'),
                    (SELECT "Id" FROM "Usuarios" ORDER BY "FechaCreacion" LIMIT 1))
                WHERE "CreadoPorUsuarioId" = '00000000-0000-0000-0000-000000000000';
                """);

            migrationBuilder.AddForeignKey(
                name: "FK_CaptacionesGanado_Usuarios_CreadoPorUsuarioId",
                table: "CaptacionesGanado",
                column: "CreadoPorUsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CaptacionesGanado_Usuarios_ModificadoPorUsuarioId",
                table: "CaptacionesGanado",
                column: "ModificadoPorUsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DetallesLoteGanado_Usuarios_ActualizadoPor",
                table: "DetallesLoteGanado",
                column: "ActualizadoPor",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DetallesLoteGanado_Usuarios_CreadoPor",
                table: "DetallesLoteGanado",
                column: "CreadoPor",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Estancias_Usuarios_CreadoPorUsuarioId",
                table: "Estancias",
                column: "CreadoPorUsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Estancias_Usuarios_ModificadoPorUsuarioId",
                table: "Estancias",
                column: "ModificadoPorUsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MovimientosGanado_Usuarios_CreadoPorUsuarioId",
                table: "MovimientosGanado",
                column: "CreadoPorUsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MovimientosGanado_Usuarios_ModificadoPorUsuarioId",
                table: "MovimientosGanado",
                column: "ModificadoPorUsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RegistrosAlimentacion_Usuarios_CreadoPorUsuarioId",
                table: "RegistrosAlimentacion",
                column: "CreadoPorUsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RegistrosAlimentacion_Usuarios_ModificadoPorUsuarioId",
                table: "RegistrosAlimentacion",
                column: "ModificadoPorUsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RegistrosPesaje_Usuarios_CreadoPorUsuarioId",
                table: "RegistrosPesaje",
                column: "CreadoPorUsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RegistrosPesaje_Usuarios_ModificadoPorUsuarioId",
                table: "RegistrosPesaje",
                column: "ModificadoPorUsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RegistrosSanitarios_Usuarios_CreadoPorUsuarioId",
                table: "RegistrosSanitarios",
                column: "CreadoPorUsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RegistrosSanitarios_Usuarios_ModificadoPorUsuarioId",
                table: "RegistrosSanitarios",
                column: "ModificadoPorUsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CaptacionesGanado_Usuarios_CreadoPorUsuarioId",
                table: "CaptacionesGanado");

            migrationBuilder.DropForeignKey(
                name: "FK_CaptacionesGanado_Usuarios_ModificadoPorUsuarioId",
                table: "CaptacionesGanado");

            migrationBuilder.DropForeignKey(
                name: "FK_DetallesLoteGanado_Usuarios_ActualizadoPor",
                table: "DetallesLoteGanado");

            migrationBuilder.DropForeignKey(
                name: "FK_DetallesLoteGanado_Usuarios_CreadoPor",
                table: "DetallesLoteGanado");

            migrationBuilder.DropForeignKey(
                name: "FK_Estancias_Usuarios_CreadoPorUsuarioId",
                table: "Estancias");

            migrationBuilder.DropForeignKey(
                name: "FK_Estancias_Usuarios_ModificadoPorUsuarioId",
                table: "Estancias");

            migrationBuilder.DropForeignKey(
                name: "FK_MovimientosGanado_Usuarios_CreadoPorUsuarioId",
                table: "MovimientosGanado");

            migrationBuilder.DropForeignKey(
                name: "FK_MovimientosGanado_Usuarios_ModificadoPorUsuarioId",
                table: "MovimientosGanado");

            migrationBuilder.DropForeignKey(
                name: "FK_RegistrosAlimentacion_Usuarios_CreadoPorUsuarioId",
                table: "RegistrosAlimentacion");

            migrationBuilder.DropForeignKey(
                name: "FK_RegistrosAlimentacion_Usuarios_ModificadoPorUsuarioId",
                table: "RegistrosAlimentacion");

            migrationBuilder.DropForeignKey(
                name: "FK_RegistrosPesaje_Usuarios_CreadoPorUsuarioId",
                table: "RegistrosPesaje");

            migrationBuilder.DropForeignKey(
                name: "FK_RegistrosPesaje_Usuarios_ModificadoPorUsuarioId",
                table: "RegistrosPesaje");

            migrationBuilder.DropForeignKey(
                name: "FK_RegistrosSanitarios_Usuarios_CreadoPorUsuarioId",
                table: "RegistrosSanitarios");

            migrationBuilder.DropForeignKey(
                name: "FK_RegistrosSanitarios_Usuarios_ModificadoPorUsuarioId",
                table: "RegistrosSanitarios");

            migrationBuilder.DropIndex(
                name: "IX_RegistrosSanitarios_CreadoPorUsuarioId",
                table: "RegistrosSanitarios");

            migrationBuilder.DropIndex(
                name: "IX_RegistrosSanitarios_ModificadoPorUsuarioId",
                table: "RegistrosSanitarios");

            migrationBuilder.DropIndex(
                name: "IX_RegistrosPesaje_CreadoPorUsuarioId",
                table: "RegistrosPesaje");

            migrationBuilder.DropIndex(
                name: "IX_RegistrosPesaje_ModificadoPorUsuarioId",
                table: "RegistrosPesaje");

            migrationBuilder.DropCheckConstraint(
                name: "CK_RegistrosPesaje_CantidadCabezasPesadas",
                table: "RegistrosPesaje");

            migrationBuilder.DropCheckConstraint(
                name: "CK_RegistrosPesaje_PesoPromedioKg",
                table: "RegistrosPesaje");

            migrationBuilder.DropIndex(
                name: "IX_RegistrosAlimentacion_CreadoPorUsuarioId",
                table: "RegistrosAlimentacion");

            migrationBuilder.DropIndex(
                name: "IX_RegistrosAlimentacion_ModificadoPorUsuarioId",
                table: "RegistrosAlimentacion");

            migrationBuilder.DropCheckConstraint(
                name: "CK_RegistrosAlimentacion_RacionBaseKgAnimal",
                table: "RegistrosAlimentacion");

            migrationBuilder.DropCheckConstraint(
                name: "CK_RegistrosAlimentacion_SuplementoProteicoKgAnimal",
                table: "RegistrosAlimentacion");

            migrationBuilder.DropIndex(
                name: "IX_MovimientosGanado_CreadoPorUsuarioId",
                table: "MovimientosGanado");

            migrationBuilder.DropIndex(
                name: "IX_MovimientosGanado_ModificadoPorUsuarioId",
                table: "MovimientosGanado");

            migrationBuilder.DropCheckConstraint(
                name: "CK_MovimientosGanado_CantidadCabezas",
                table: "MovimientosGanado");

            migrationBuilder.DropIndex(
                name: "IX_Estancias_CreadoPorUsuarioId",
                table: "Estancias");

            migrationBuilder.DropIndex(
                name: "IX_Estancias_ModificadoPorUsuarioId",
                table: "Estancias");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Estancias_HectareasTotales",
                table: "Estancias");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Estancias_Latitud",
                table: "Estancias");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Estancias_Longitud",
                table: "Estancias");

            migrationBuilder.DropIndex(
                name: "IX_DetallesLoteGanado_ActualizadoPor",
                table: "DetallesLoteGanado");

            migrationBuilder.DropIndex(
                name: "IX_DetallesLoteGanado_CreadoPor",
                table: "DetallesLoteGanado");

            migrationBuilder.DropCheckConstraint(
                name: "CK_DetallesLoteGanado_CantidadCabezas",
                table: "DetallesLoteGanado");

            migrationBuilder.DropCheckConstraint(
                name: "CK_DetallesLoteGanado_PesoPromedioEstimadoKg",
                table: "DetallesLoteGanado");

            migrationBuilder.DropIndex(
                name: "IX_CaptacionesGanado_CreadoPorUsuarioId",
                table: "CaptacionesGanado");

            migrationBuilder.DropIndex(
                name: "IX_CaptacionesGanado_ModificadoPorUsuarioId",
                table: "CaptacionesGanado");

            migrationBuilder.DropCheckConstraint(
                name: "CK_CaptacionesGanado_Latitud",
                table: "CaptacionesGanado");

            migrationBuilder.DropCheckConstraint(
                name: "CK_CaptacionesGanado_Longitud",
                table: "CaptacionesGanado");
        }
    }
}
