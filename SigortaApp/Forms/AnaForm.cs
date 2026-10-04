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
        MinimumSize = new Size(1000, 650);
        StartPosition = FormStartPosition.CenterScreen;
        Font = Tema.Normal;
        BackColor = Tema.Arkaplan;
        DoubleBuffered = true;

        // Sağ taraf: sayfaların yüklendiği alan
        _icerik = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Tema.Arkaplan,
            Padding = new Padding(32)
        };

        // Sol taraf: sidebar
        var sidebar = new Panel { Dock = DockStyle.Left, Width = 230, BackColor = Tema.Sidebar };

        _menu = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            BackColor = Tema.Sidebar,
            Padding = Padding.Empty
        };

        var logo = new Label
        {
            Text = "SigortaApp",
            Dock = DockStyle.Top,
            Height = 76,
            ForeColor = Color.White,
            Font = new Font("Segoe UI Semibold", 14f),
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(22, 0, 0, 0)
        };

        sidebar.Controls.Add(_menu);   // Fill önce
        sidebar.Controls.Add(logo);    // Top sonra
        Controls.Add(_icerik);         // Fill önce
        Controls.Add(sidebar);         // Left sonra

        var ilk = MenuEkle("Müşteriler", () => new BosSayfa("Müşteriler", "Müşteri ve sigortalı yönetimi"));
        MenuEkle("Poliçeler", () => new BosSayfa("Poliçeler", "Poliçe yönetimi"));
        MenuEkle("Hasarlar",  () => new BosSayfa("Hasarlar", "Hasar takibi"));

        Load += (_, _) => Git(ilk);
    }

    private MenuButonu MenuEkle(string metin, Func<UserControl> sayfaOlustur)
    {
        var btn = new MenuButonu { Text = metin, Width = 230 };
        btn.Click += (_, _) => Git(btn);
        _sayfalar[btn] = sayfaOlustur;
        _menu.Controls.Add(btn);
        return btn;
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