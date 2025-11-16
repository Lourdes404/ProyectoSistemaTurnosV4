using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoSistemaTurnosV4.Migrations
{
    /// <inheritdoc />
    public partial class Triage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Triage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PacienteId = table.Column<int>(type: "int", nullable: false),
                    ClinicaDestinoId = table.Column<int>(type: "int", nullable: false),
                    NivelPrioridad = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    MotivoConsulta = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Sintomas = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    SignosVitales = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Triage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Triage_Clinicas_ClinicaDestinoId",
                        column: x => x.ClinicaDestinoId,
                        principalTable: "Clinicas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Triage_Pacientes_PacienteId",
                        column: x => x.PacienteId,
                        principalTable: "Pacientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Triage_ClinicaDestinoId",
                table: "Triage",
                column: "ClinicaDestinoId");

            migrationBuilder.CreateIndex(
                name: "IX_Triage_PacienteId_FechaRegistro",
                table: "Triage",
                columns: new[] { "PacienteId", "FechaRegistro" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Triage");
        }
    }
}
