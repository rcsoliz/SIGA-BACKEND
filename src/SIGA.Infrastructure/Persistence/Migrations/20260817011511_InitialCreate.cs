using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIGA.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    Cargo = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Rol = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Estado = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TipoUsuario = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Dispositivos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    IdentificadorDispositivo = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    UltimaSincronizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UbicacionActual = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Estado = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dispositivos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dispositivos_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Estancias",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CaptadorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Propietario = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Representante = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Telefono = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Latitud = table.Column<double>(type: "double precision", nullable: false),
                    Longitud = table.Column<double>(type: "double precision", nullable: false),
                    Renspa = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    HectareasTotales = table.Column<double>(type: "double precision", nullable: true),
                    Departamento = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Provincia = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Municipio = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CreadoPorUsuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    FechaCreacionLocal = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaSincronizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EstadoSync = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ModificadoPorUsuarioId = table.Column<Guid>(type: "uuid", nullable: true),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estancias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Estancias_Usuarios_CaptadorId",
                        column: x => x.CaptadorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LogsAuditoria",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    Accion = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Modulo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IdRegistroAfectado = table.Column<Guid>(type: "uuid", nullable: false),
                    FechaHora = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Detalle = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogsAuditoria", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LogsAuditoria_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PermisosUsuario",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    TipoPermiso = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermisosUsuario", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PermisosUsuario_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SectoresAsignados",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    NombreSector = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Zona = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SectoresAsignados", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SectoresAsignados_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CaptacionesGanado",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EstanciaId = table.Column<Guid>(type: "uuid", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Observaciones = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Estado = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    EstadoSanitario = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Potrero = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreadoPorUsuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    FechaCreacionLocal = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaSincronizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EstadoSync = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ModificadoPorUsuarioId = table.Column<Guid>(type: "uuid", nullable: true),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaptacionesGanado", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CaptacionesGanado_Estancias_EstanciaId",
                        column: x => x.EstanciaId,
                        principalTable: "Estancias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DetallesLoteGanado",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CaptacionGanadoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Categoria = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Raza = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CantidadCabezas = table.Column<int>(type: "integer", nullable: false),
                    PesoPromedioEstimadoKg = table.Column<double>(type: "double precision", nullable: true),
                    SistemaAlimentacion = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    FechaEstimadaFaena = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    NotasZootecnicas = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreadoPor = table.Column<Guid>(type: "uuid", nullable: false),
                    CreadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ActualizadoPor = table.Column<Guid>(type: "uuid", nullable: true),
                    ActualizadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetallesLoteGanado", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DetallesLoteGanado_CaptacionesGanado_CaptacionGanadoId",
                        column: x => x.CaptacionGanadoId,
                        principalTable: "CaptacionesGanado",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MovimientosGanado",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CaptacionGanadoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TipoGanado = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CantidadCabezas = table.Column<int>(type: "integer", nullable: false),
                    Origen = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Destino = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    CreadoPorUsuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    FechaCreacionLocal = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaSincronizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EstadoSync = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ModificadoPorUsuarioId = table.Column<Guid>(type: "uuid", nullable: true),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovimientosGanado", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MovimientosGanado_CaptacionesGanado_CaptacionGanadoId",
                        column: x => x.CaptacionGanadoId,
                        principalTable: "CaptacionesGanado",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RegistrosAlimentacion",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CaptacionGanadoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TipoAlimentacion = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    RacionBaseKgAnimal = table.Column<double>(type: "double precision", nullable: true),
                    SuplementoProteicoKgAnimal = table.Column<double>(type: "double precision", nullable: true),
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
                    table.PrimaryKey("PK_RegistrosAlimentacion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistrosAlimentacion_CaptacionesGanado_CaptacionGanadoId",
                        column: x => x.CaptacionGanadoId,
                        principalTable: "CaptacionesGanado",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RegistrosSanitarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CaptacionGanadoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TipoEvento = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    ProductoTratamiento = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    RegistradoPorUsuarioId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_RegistrosSanitarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistrosSanitarios_CaptacionesGanado_CaptacionGanadoId",
                        column: x => x.CaptacionGanadoId,
                        principalTable: "CaptacionesGanado",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RegistrosSanitarios_Usuarios_RegistradoPorUsuarioId",
                        column: x => x.RegistradoPorUsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CaptacionesGanado_EstanciaId",
                table: "CaptacionesGanado",
                column: "EstanciaId");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesLoteGanado_CaptacionGanadoId",
                table: "DetallesLoteGanado",
                column: "CaptacionGanadoId");

            migrationBuilder.CreateIndex(
                name: "IX_Dispositivos_UsuarioId",
                table: "Dispositivos",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Estancias_CaptadorId",
                table: "Estancias",
                column: "CaptadorId");

            migrationBuilder.CreateIndex(
                name: "IX_LogsAuditoria_FechaHora",
                table: "LogsAuditoria",
                column: "FechaHora");

            migrationBuilder.CreateIndex(
                name: "IX_LogsAuditoria_UsuarioId",
                table: "LogsAuditoria",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosGanado_CaptacionGanadoId",
                table: "MovimientosGanado",
                column: "CaptacionGanadoId");

            migrationBuilder.CreateIndex(
                name: "IX_PermisosUsuario_UsuarioId",
                table: "PermisosUsuario",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosAlimentacion_CaptacionGanadoId",
                table: "RegistrosAlimentacion",
                column: "CaptacionGanadoId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosSanitarios_CaptacionGanadoId",
                table: "RegistrosSanitarios",
                column: "CaptacionGanadoId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosSanitarios_RegistradoPorUsuarioId",
                table: "RegistrosSanitarios",
                column: "RegistradoPorUsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_SectoresAsignados_UsuarioId",
                table: "SectoresAsignados",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DetallesLoteGanado");

            migrationBuilder.DropTable(
                name: "Dispositivos");

            migrationBuilder.DropTable(
                name: "LogsAuditoria");

            migrationBuilder.DropTable(
                name: "MovimientosGanado");

            migrationBuilder.DropTable(
                name: "PermisosUsuario");

            migrationBuilder.DropTable(
                name: "RegistrosAlimentacion");

            migrationBuilder.DropTable(
                name: "RegistrosSanitarios");

            migrationBuilder.DropTable(
                name: "SectoresAsignados");

            migrationBuilder.DropTable(
                name: "CaptacionesGanado");

            migrationBuilder.DropTable(
                name: "Estancias");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
