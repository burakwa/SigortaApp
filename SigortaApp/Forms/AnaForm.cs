using SigortaApp.UI;

namespace SigortaApp.Forms;

public class AnaForm : Form
{
    private readonly Panel _icerik;
    private readonly FlowLayoutPanel _menu;
    private readonly Dictionary<MenuButonu, Func<UserControl>> _sayfalar = new();

    public AnaForm()
    {
        Text = "SigortaApp";
        Size = new Size(1280, 800);
        MinimumSize = new Size(1180, 650);
        StartPosition = FormStartPosition.CenterScreen;
        Font = Tema.Normal;
        BackColor = Tema.Arkaplan;
        DoubleBuffered = true;

        // Sağ taraf: sayfaların yüklendiği alan
        _icerik = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Tema.Arkaplan,
            Padding = new Padding(36, 28, 36, 24)
        };

        // Sol taraf: sidebar
        var sidebar = new Panel { Dock = DockStyle.Left, Width = 240, BackColor = Tema.Sidebar };

        _menu = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            BackColor = Tema.Sidebar,
            Padding = new Padding(0, 8, 0, 0)
        };

        // Logo alanı
        var logoPanel = new Panel { Dock = DockStyle.Top, Height = 92, Padding = new Padding(0, 18, 0, 0) };
        var lblAlt = new Label
        {
            Text = "Sağlık Sigortası Paneli",
            Font = Tema.Kucuk,
            ForeColor = Tema.SidebarSoluk,
            Dock = DockStyle.Top,
            Height = 24,
            AutoSize = false,
            Padding = new Padding(26, 0, 0, 0)
        };
        var lblAd = new Label
        {
            Text = "SigortaApp",
            Font = new Font("Segoe UI Semibold", 16f),
            ForeColor = Color.White,
            Dock = DockStyle.Top,
            Height = 36,
            AutoSize = false,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(26, 0, 0, 0)
        };
        logoPanel.Controls.Add(lblAlt);   // önce eklenen altta kalır
        logoPanel.Controls.Add(lblAd);

        sidebar.Controls.Add(_menu);      // Fill önce
        sidebar.Controls.Add(logoPanel);  // Top sonra
        Controls.Add(_icerik);            // Fill önce
        Controls.Add(sidebar);            // Left sonra

        var ilk = MenuEkle("Müşteriler", "\uE716", () => new MusteriListeSayfasi());
        MenuEkle("Poliçeler", "\uE8A5", () => new PoliceListeSayfasi());
        MenuEkle("Hasarlar", "\uE7BA", () => new HasarListeSayfasi());

        Load += (_, _) => Git(ilk);
    }

    private MenuButonu MenuEkle(string metin, string ikon, Func<UserControl> sayfaOlustur)
    {
        var btn = new MenuButonu { Text = metin, Ikon = ikon, Width = 216 };
        btn.Click += (_, _) => Git(btn);
        _sayfalar[btn] = sayfaOlustur;
        _menu.Controls.Add(btn);
        return btn;
    }

    private void InitializeComponent()
    {

    }

    private void Git(MenuButonu hedef)
    {
        foreach (var b in _sayfalar.Keys)
            b.Aktif = b == hedef;

        foreach (Control c in _icerik.Controls) c.Dispose();
        _icerik.Controls.Clear();

        var sayfa = _sayfalar[hedef]();
        sayfa.Dock = DockStyle.Fill;
        _icerik.Controls.Add(sayfa);
    }
}