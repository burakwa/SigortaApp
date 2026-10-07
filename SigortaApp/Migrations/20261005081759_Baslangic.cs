using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SigortaApp.Migrations
{
    /// <inheritdoc />
    public partial class Baslangic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ayarlar",
                columns: table => new
                {
                    Anahtar = table.Column<string>(type: "TEXT", nullable: false),
                    Deger = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ayarlar", x => x.Anahtar);
                });

            migrationBuilder.CreateTable(
                name: "KisaDonemOranlari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MaxGun = table.Column<int>(type: "INTEGER", nullable: false),
                    PrimOrani = table.Column<decimal>(type: "TEXT", precision: 5, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KisaDonemOranlari", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Musteriler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TcKimlikNo = table.Column<string>(type: "TEXT", nullable: false),
                    Ad = table.Column<string>(type: "TEXT", nullable: false),
                    Soyad = table.Column<string>(type: "TEXT", nullable: false),
                    DogumTarihi = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Telefon = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    Adres = table.Column<string>(type: "TEXT", nullable: true),
                    KayitTarihi = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Silindi = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Musteriler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Teminatlar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Kod = table.Column<string>(type: "TEXT", nullable: false),
                    Ad = table.Column<string>(type: "TEXT", nullable: false),
                    Aciklama = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teminatlar", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Urunler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Kod = table.Column<string>(type: "TEXT", nullable: false),
                    Ad = table.Column<string>(type: "TEXT", nullable: false),
                    Tur = table.Column<int>(type: "INTEGER", nullable: false),
                    Aciklama = table.Column<string>(type: "TEXT", nullable: true),
                    Aktif = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Urunler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Vergiler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Ad = table.Column<string>(type: "TEXT", nullable: false),
                    Oran = table.Column<decimal>(type: "TEXT", precision: 9, scale: 4, nullable: false),
                    UrunId = table.Column<int>(type: "INTEGER", nullable: true),
                    Aktif = table.Column<bool>(type: "INTEGER", nullable: false),
                    GecerliBaslangic = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vergiler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Policeler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PoliceNo = table.Column<string>(type: "TEXT", nullable: false),
                    MusteriId = table.Column<int>(type: "INTEGER", nullable: false),
                    UrunId = table.Column<int>(type: "INTEGER", nullable: true),
                    TanzimTarihi = table.Column<DateTime>(type: "TEXT", nullable: false),
                    BaslangicTarihi = table.Column<DateTime>(type: "TEXT", nullable: false),
                    BitisTarihi = table.Column<DateTime>(type: "TEXT", nullable: false),
                    NetPrim = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    VergiTutari = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Prim = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Durum = table.Column<int>(type: "INTEGER", nullable: false),
                    OncekiPoliceId = table.Column<int>(type: "INTEGER", nullable: true),
                    IptalTarihi = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IptalNedeni = table.Column<string>(type: "TEXT", nullable: true),
                    IptalHesapYontemi = table.Column<int>(type: "INTEGER", nullable: true),
                    KazanilanPrim = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: true),
                    IadeTutari = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Policeler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Policeler_Musteriler_MusteriId",
                        column: x => x.MusteriId,
                        principalTable: "Musteriler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Policeler_Urunler_UrunId",
                        column: x => x.UrunId,
                        principalTable: "Urunler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tarifeler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UrunId = table.Column<int>(type: "INTEGER", nullable: false),
                    Grup = table.Column<int>(type: "INTEGER", nullable: false),
                    YasMin = table.Column<int>(type: "INTEGER", nullable: false),
                    YasMax = table.Column<int>(type: "INTEGER", nullable: false),
                    YillikPrim = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    GecerliBaslangic = table.Column<DateTime>(type: "TEXT", nullable: false),
                    GecerliBitis = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tarifeler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tarifeler_Urunler_UrunId",
                        column: x => x.UrunId,
                        principalTable: "Urunler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UrunTeminatlari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UrunId = table.Column<int>(type: "INTEGER", nullable: false),
                    TeminatId = table.Column<int>(type: "INTEGER", nullable: false),
                    Limit = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: true),
                    KatilimPayiOrani = table.Column<decimal>(type: "TEXT", precision: 5, scale: 2, nullable: false),
                    BeklemeSuresiGun = table.Column<int>(type: "INTEGER", nullable: false),
                    Istisnalar = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UrunTeminatlari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UrunTeminatlari_Teminatlar_TeminatId",
                        column: x => x.TeminatId,
                        principalTable: "Teminatlar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UrunTeminatlari_Urunler_UrunId",
                        column: x => x.UrunId,
                        principalTable: "Urunler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AileBireyleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PoliceId = table.Column<int>(type: "INTEGER", nullable: false),
                    TcKimlikNo = table.Column<string>(type: "TEXT", nullable: false),
                    Ad = table.Column<string>(type: "TEXT", nullable: false),
                    Soyad = table.Column<string>(type: "TEXT", nullable: false),
                    DogumTarihi = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Yakinlik = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AileBireyleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AileBireyleri_Policeler_PoliceId",
                        column: x => x.PoliceId,
                        principalTable: "Policeler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Hasarlar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PoliceId = table.Column<int>(type: "INTEGER", nullable: false),
                    HasarTarihi = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Aciklama = table.Column<string>(type: "TEXT", nullable: false),
                    Tutar = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Durum = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hasarlar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Hasarlar_Policeler_PoliceId",
                        column: x => x.PoliceId,
                        principalTable: "Policeler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PoliceTeminatlari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PoliceId = table.Column<int>(type: "INTEGER", nullable: false),
                    TeminatId = table.Column<int>(type: "INTEGER", nullable: false),
                    TeminatAdi = table.Column<string>(type: "TEXT", nullable: false),
                    Limit = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: true),
                    KatilimPayiOrani = table.Column<decimal>(type: "TEXT", precision: 5, scale: 2, nullable: false),
                    BeklemeSuresiGun = table.Column<int>(type: "INTEGER", nullable: false),
                    Istisnalar = table.Column<string>(type: "TEXT", nullable: true),
                    Aktif = table.Column<bool>(type: "INTEGER", nullable: false),
                    EklenmeTarihi = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CikarilmaTarihi = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PoliceTeminatlari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PoliceTeminatlari_Policeler_PoliceId",
                        column: x => x.PoliceId,
                        principalTable: "Policeler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PoliceTeminatlari_Teminatlar_TeminatId",
                        column: x => x.TeminatId,
                        principalTable: "Teminatlar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Zeyiller",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PoliceId = table.Column<int>(type: "INTEGER", nullable: false),
                    ZeyilNo = table.Column<int>(type: "INTEGER", nullable: false),
                    Tur = table.Column<int>(type: "INTEGER", nullable: false),
                    YururlukTarihi = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Aciklama = table.Column<string>(type: "TEXT", nullable: false),
                    PrimFarki = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zeyiller", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Zeyiller_Policeler_PoliceId",
                        column: x => x.PoliceId,
                        principalTable: "Policeler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AileBireyleri_PoliceId",
                table: "AileBireyleri",
                column: "PoliceId");

            migrationBuilder.CreateIndex(
                name: "IX_Hasarlar_PoliceId",
                table: "Hasarlar",
                column: "PoliceId");

            migrationBuilder.CreateIndex(
                name: "IX_KisaDonemOranlari_MaxGun",
                table: "KisaDonemOranlari",
                column: "MaxGun",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Musteriler_TcKimlikNo",
                table: "Musteriler",
                column: "TcKimlikNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Policeler_MusteriId",
                table: "Policeler",
                column: "MusteriId");

            migrationBuilder.CreateIndex(
                name: "IX_Policeler_PoliceNo",
                table: "Policeler",
                column: "PoliceNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Policeler_UrunId",
                table: "Policeler",
                column: "UrunId");

            migrationBuilder.CreateIndex(
                name: "IX_PoliceTeminatlari_PoliceId",
                table: "PoliceTeminatlari",
                column: "PoliceId");

            migrationBuilder.CreateIndex(
                name: "IX_PoliceTeminatlari_TeminatId",
                table: "PoliceTeminatlari",
                column: "TeminatId");

            migrationBuilder.CreateIndex(
                name: "IX_Tarifeler_UrunId",
                table: "Tarifeler",
                column: "UrunId");

            migrationBuilder.CreateIndex(
                name: "IX_Teminatlar_Kod",
                table: "Teminatlar",
                column: "Kod",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Urunler_Kod",
                table: "Urunler",
                column: "Kod",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UrunTeminatlari_TeminatId",
                table: "UrunTeminatlari",
                column: "TeminatId");

            migrationBuilder.CreateIndex(
                name: "IX_UrunTeminatlari_UrunId_TeminatId",
                table: "UrunTeminatlari",
                columns: new[] { "UrunId", "TeminatId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Zeyiller_PoliceId_ZeyilNo",
                table: "Zeyiller",
                columns: new[] { "PoliceId", "ZeyilNo" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AileBireyleri");

            migrationBuilder.DropTable(
                name: "Ayarlar");

            migrationBuilder.DropTable(
                name: "Hasarlar");

            migrationBuilder.DropTable(
                name: "KisaDonemOranlari");

            migrationBuilder.DropTable(
                name: "PoliceTeminatlari");

            migrationBuilder.DropTable(
                name: "Tarifeler");

            migrationBuilder.DropTable(
                name: "UrunTeminatlari");

            migrationBuilder.DropTable(
                name: "Vergiler");

            migrationBuilder.DropTable(
                name: "Zeyiller");

            migrationBuilder.DropTable(
                name: "Teminatlar");

            migrationBuilder.DropTable(
                name: "Policeler");

            migrationBuilder.DropTable(
                name: "Musteriler");

            migrationBuilder.DropTable(
                name: "Urunler");
        }
    }
}
