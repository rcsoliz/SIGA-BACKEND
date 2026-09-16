using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIGA.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCatalogosUbicacionRazaProducto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ProductoTratamientoId",
                table: "RegistrosSanitarios",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DepartamentoId",
                table: "Estancias",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "MunicipioId",
                table: "Estancias",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProvinciaId",
                table: "Estancias",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RazaId",
                table: "DetallesLoteGanado",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Departamentos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departamentos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductosTratamiento",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductosTratamiento", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Razas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Razas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Provincias",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DepartamentoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Provincias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Provincias_Departamentos_DepartamentoId",
                        column: x => x.DepartamentoId,
                        principalTable: "Departamentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Municipios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProvinciaId = table.Column<Guid>(type: "uuid", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Municipios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Municipios_Provincias_ProvinciaId",
                        column: x => x.ProvinciaId,
                        principalTable: "Provincias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosSanitarios_ProductoTratamientoId",
                table: "RegistrosSanitarios",
                column: "ProductoTratamientoId");

            migrationBuilder.CreateIndex(
                name: "IX_Estancias_DepartamentoId",
                table: "Estancias",
                column: "DepartamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_Estancias_MunicipioId",
                table: "Estancias",
                column: "MunicipioId");

            migrationBuilder.CreateIndex(
                name: "IX_Estancias_ProvinciaId",
                table: "Estancias",
                column: "ProvinciaId");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesLoteGanado_RazaId",
                table: "DetallesLoteGanado",
                column: "RazaId");

            migrationBuilder.CreateIndex(
                name: "IX_Departamentos_Nombre",
                table: "Departamentos",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Municipios_ProvinciaId_Nombre",
                table: "Municipios",
                columns: new[] { "ProvinciaId", "Nombre" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductosTratamiento_Nombre",
                table: "ProductosTratamiento",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Provincias_DepartamentoId_Nombre",
                table: "Provincias",
                columns: new[] { "DepartamentoId", "Nombre" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Razas_Nombre",
                table: "Razas",
                column: "Nombre",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DetallesLoteGanado_Razas_RazaId",
                table: "DetallesLoteGanado",
                column: "RazaId",
                principalTable: "Razas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Estancias_Departamentos_DepartamentoId",
                table: "Estancias",
                column: "DepartamentoId",
                principalTable: "Departamentos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Estancias_Municipios_MunicipioId",
                table: "Estancias",
                column: "MunicipioId",
                principalTable: "Municipios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Estancias_Provincias_ProvinciaId",
                table: "Estancias",
                column: "ProvinciaId",
                principalTable: "Provincias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RegistrosSanitarios_ProductosTratamiento_ProductoTratamient~",
                table: "RegistrosSanitarios",
                column: "ProductoTratamientoId",
                principalTable: "ProductosTratamiento",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DetallesLoteGanado_Razas_RazaId",
                table: "DetallesLoteGanado");

            migrationBuilder.DropForeignKey(
                name: "FK_Estancias_Departamentos_DepartamentoId",
                table: "Estancias");

            migrationBuilder.DropForeignKey(
                name: "FK_Estancias_Municipios_MunicipioId",
                table: "Estancias");

            migrationBuilder.DropForeignKey(
                name: "FK_Estancias_Provincias_ProvinciaId",
                table: "Estancias");

            migrationBuilder.DropForeignKey(
                name: "FK_RegistrosSanitarios_ProductosTratamiento_ProductoTratamient~",
                table: "RegistrosSanitarios");

            migrationBuilder.DropTable(
                name: "Municipios");

            migrationBuilder.DropTable(
                name: "ProductosTratamiento");

            migrationBuilder.DropTable(
                name: "Razas");

            migrationBuilder.DropTable(
                name: "Provincias");

            migrationBuilder.DropTable(
                name: "Departamentos");

            migrationBuilder.DropIndex(
                name: "IX_RegistrosSanitarios_ProductoTratamientoId",
                table: "RegistrosSanitarios");

            migrationBuilder.DropIndex(
                name: "IX_Estancias_DepartamentoId",
                table: "Estancias");

            migrationBuilder.DropIndex(
                name: "IX_Estancias_MunicipioId",
                table: "Estancias");

            migrationBuilder.DropIndex(
                name: "IX_Estancias_ProvinciaId",
                table: "Estancias");

            migrationBuilder.DropIndex(
                name: "IX_DetallesLoteGanado_RazaId",
                table: "DetallesLoteGanado");

            migrationBuilder.DropColumn(
                name: "ProductoTratamientoId",
                table: "RegistrosSanitarios");

            migrationBuilder.DropColumn(
                name: "DepartamentoId",
                table: "Estancias");

            migrationBuilder.DropColumn(
                name: "MunicipioId",
                table: "Estancias");

            migrationBuilder.DropColumn(
                name: "ProvinciaId",
                table: "Estancias");

            migrationBuilder.DropColumn(
                name: "RazaId",
                table: "DetallesLoteGanado");
        }
    }
}
