using Microsoft.EntityFrameworkCore;
using SigortaApp.Data;

namespace SigortaApp;

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        // Bekleyen migration'ları uygular; veritabanı yoksa oluşturur
        using (var db = new AppDbContext())
            db.Database.Migrate();

        Application.Run(new SigortaApp.Forms.AnaForm());
    }
}