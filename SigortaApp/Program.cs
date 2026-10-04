using SigortaApp.Data;
namespace SigortaApp
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            using (var db = new AppDbContext())
            db.Database.EnsureCreated();
            Application.Run(new SigortaApp.Forms.AnaForm());
        }
    }
}