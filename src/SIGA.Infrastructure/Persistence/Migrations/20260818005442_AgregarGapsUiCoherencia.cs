using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIGA.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AgregarGapsUiCoherencia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Latitud",
                table: "CaptacionesGanado",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitud",
                table: "CaptacionesGanado",
                type: "double precision",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RegistrosPesaje",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CaptacionGanadoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PesoPromedioKg = table.Column<double>(type: "double precision", nullable: false),
                    CantidadCabezasPesadas = table.Column<int>(type: "integer", nullable: true),
                    Observaciones = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreadoPorUsuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    FechaCreacionLocal = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaSincronizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EstadoSync = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ModificadoPorUsuarioId = table.Column<Guid>(type: "uuid", nullable: true),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrosPesaje", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistrosPesaje_CaptacionesGanado_CaptacionGanadoId",
                        column: x => x.CaptacionGanadoId,
                        principalTable: "CaptacionesGanado",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosPesaje_CaptacionGanadoId",
                table: "RegistrosPesaje",
                column: "CaptacionGanadoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RegistrosPesaje");

            migrationBuilder.DropColumn(
                name: "Latitud",
                table: "CaptacionesGanado");

            migrationBuilder.DropColumn(
                name: "Longitud",
                table: "CaptacionesGanado");
        }
    }
}
