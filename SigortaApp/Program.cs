using Microsoft.EntityFrameworkCore;
using SigortaApp.Data;

namespace SigortaApp;

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        try
        {
            // Bekleyen migration'ları uygular; veritabanı yoksa oluşturur
            using var db = new AppDbContext();
            db.Database.Migrate();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString(), "Veritabanı hatası");
            return;
        }

        Application.Run(new SigortaApp.Forms.AnaForm());
    }
}