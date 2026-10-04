namespace SigortaApp.Helpers;

public static class TcKimlikDogrulayici
{
    public static bool Gecerlimi(string? tc)
    {
        if (string.IsNullOrWhiteSpace(tc) || tc.Length != 11) return false;
        if (!tc.All(c => c is >= '0' and <= '9')) return false;
        if (tc[0] == '0') return false;

        int[] d = tc.Select(c => c - '0').ToArray();

        int tekler  = d[0] + d[2] + d[4] + d[6] + d[8];
        int ciftler = d[1] + d[3] + d[5] + d[7];

        // 10. hane: (tekler*7 - çiftler) mod 10  (negatif sonuca karşı düzeltmeli)
        int hane10 = ((tekler * 7 - ciftler) % 10 + 10) % 10;
        if (d[9] != hane10) return false;

        // 11. hane: ilk 10 hanenin toplamının birler basamağı
        return d[10] == d.Take(10).Sum() % 10;
    }
}