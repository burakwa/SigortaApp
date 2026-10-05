namespace SigortaApp.Models;

public enum Yakinlik { Es, Cocuk, Anne, Baba, Diger }

// "Vadesi Geçti" saklanmaz: bitiş tarihi geçmiş "Bağlı" poliçeden hesaplanır
public enum PoliceDurumu { Teklif, Bagli, Yenilendi, Iptal }

public enum HasarDurumu { Beklemede, Onaylandi, Reddedildi, Odendi }

public enum UrunTuru { TamamlayiciSaglik, OzelSaglik, YatarakTedavi, AyaktaTedavi }

public enum TarifeGrubu { Bireysel, Aile, Kurumsal }

public enum ZeyilTuru { TeminatEkleme, TeminatCikarma, SigortaliEkleme, SigortaliCikarma }

public enum IptalYontemi { GunEsasli, KisaDonemTarifesi }