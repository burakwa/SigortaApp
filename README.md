# SigortaApp

Sigorta acenteleri için geliştirilen, modern ve minimal arayüzlü bir **sağlık sigortası yönetim masaüstü uygulaması**. Müşteri, poliçe, hasar ve ürün tanımlarını tek bir yerden yönetmeyi amaçlar.

> 🚧 Proje aktif geliştirme aşamasındadır.

## Özellikler

- **Müşteri yönetimi:** ekleme, düzenleme, silme (soft delete) ve arama
- **Poliçe yönetimi:** poliçe oluşturma, düzenleme, iptal ve durum takibi
- **Aile bireyleri / bağımlılar:** poliçe altında tanımlama
- **Hasar takibi:** hasar kaydı, durum yönetimi, poliçe dönemi kontrolü
- **Müşteri geçmişi:** müşterinin tüm poliçeleri ve hasarları özet kartlarıyla
- **Ürün ve teminat tanımları:** limit, katılım payı, bekleme süresi, istisnalar
- **Ana sayfa:** özet göstergeler ve yaklaşan yenileme takibi

## Yol haritası

- Yaş/grup bazlı tarife ve vergi parametreleri
- Teklif → poliçe tanzimi (tekli ve toplu)
- Zeyilname (ara dönem değişiklikleri)
- İptal işlemi: kısa dönem prim ve iade hesabı
- Poliçe yenileme ve hatırlatmalar
- Gelişmiş arama

## Teknolojiler

- C# / .NET 8, Windows Forms
- Entity Framework Core (migrations)
- SQLite

## Çalıştırma

```bash
git clone https://github.com/burakwa/SigortaApp
cd SigortaApp
dotnet run
```

Veritabanı ilk açılışta `%LocalAppData%\SigortaApp\sigorta.db` konumunda otomatik oluşturulur.


## Tasarım

Arayüz, sigorta acentesinin gün boyu açık tuttuğu bir araca yakışacak şekilde **sakin, ferah ve okunaklı** olacak biçimde tasarlandı. Amaç, yoğun veri girişi sırasında göz yormayan, dikkat dağıtmayan bir ekran sunmak.

### İlkeler

- **Minimal:** Gereksiz süs yok. Her ekranda bir başlık, bir araç çubuğu ve bir tablo kartı bulunur.
- **Tutarlı:** Tüm ekranlar aynı yerleşimi ve aynı bileşenleri kullanır, bir ekranı öğrenen diğerlerini de bilir.
- **Ferah:** Geniş satır yüksekliği, yuvarlatılmış köşeler ve bol boşluk.
- **Anlamlı renk:** Renk yalnızca bilgi taşıdığı yerde kullanılır (durum, uyarı, vurgu).

### Renk paleti

| Rol | Renk | Kullanım |
|---|---|---|
| Birincil | `#0D9488` (turkuaz) | Ana butonlar, aktif menü, vurgular |
| Menü zemini | `#0F172A` (koyu lacivert) | Sol menü |
| Sayfa zemini | `#F3F4F6` | Sayfa arka planı |
| Kart | `#FFFFFF` | Tablolar, özet kartları, formlar |
| Metin | `#111827` / `#6B7280` | Ana metin / soluk metin |
| Başarı | `#16A34A` | Aktif, ödendi, geçerli |
| Uyarı | `#D97706` | Beklemede, süresi dolmuş, yaklaşan vade |
| Tehlike | `#DC2626` | İptal, reddedildi, silme |

### Yerleşim

- **Sol menü:** İkonlu, yuvarlatılmış butonlar. Aktif sayfa turkuaz sol çizgi ve vurgulu ikonla gösterilir. "Tanımlar" gibi başlıklarla gruplanır.
- **İçerik alanı:** Başlık ve açıklama, arama kutusu ve işlem butonları, ardından yuvarlak köşeli beyaz bir kartın içinde tablo.
- **Formlar:** Sabit boyutlu diyalog pencereleri, alanlar üstte etiketli, doğrulama hatası altta kırmızı yazıyla gösterilir.

### Bileşenler

Windows Forms'un varsayılan görünümü yerine, GDI+ ile çizilen küçük bir bileşen kütüphanesi kullanılır:

- `KartPanel`: yuvarlatılmış köşeli, ince çerçeveli kart
- `YuvarlakButon`: üç stil (birincil, ikincil, tehlike), hover ve basılı durumları
- `AramaKutusu`: büyüteç ikonlu arama alanı
- `MenuButonu`: ikonlu ve aktif durumlu sol menü öğesi
- `OzetKarti`: başlık, büyük değer ve alt not içeren gösterge kartı

Tüm renkler ve fontlar tek bir `Tema` sınıfında toplanmıştır. Tema değişikliği tek dosyadan yapılır.

### Tipografi ve ikonlar

- Yazı tipi **Segoe UI** (başlıklarda Semibold). Windows'ta hazır gelir, ekstra yükleme gerektirmez.
- Menü ikonları Windows'un kendi **Segoe MDL2 Assets** ikon fontundan gelir, görsel dosya kullanılmaz.

### Tablolar

Satır yüksekliği geniş, ızgara çizgileri yalnızca yatay ve çok hafiftir. Başlıklar büyük harfli ve soluk renklidir. Durum sütunları renkle ayrılır ve seçili satır açık turkuaz ile vurgulanır.