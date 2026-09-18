using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIGA.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AgregarIndicesUnicosDispositivoYEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Dispositivos_IdentificadorDispositivo",
                table: "Dispositivos",
                column: "IdentificadorDispositivo",
                unique: true);

            // Índice de expresión (Fluent API no los modela) — coexiste con el índice único
            // case-sensitive existente sobre Email (usado por el login, que hoy compara
            // exacto). Este solo impide crear cuentas nuevas con el mismo correo en
            // distintas mayúsculas/minúsculas.
            migrationBuilder.Sql("CREATE UNIQUE INDEX \"IX_Usuarios_Email_Lower\" ON \"Usuarios\" (lower(\"Email\"));");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP INDEX IF EXISTS \"IX_Usuarios_Email_Lower\";");

            migrationBuilder.DropIndex(
                name: "IX_Dispositivos_IdentificadorDispositivo",
                table: "Dispositivos");
        }
    }
}
