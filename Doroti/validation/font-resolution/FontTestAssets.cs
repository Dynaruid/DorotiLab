using System.Security.Cryptography;

internal static class FontTestAssets
{
    // Validation-only fixtures; not restored by production builds or shipped.
    internal static async Task<string> RestoreAsync()
    {
        var root = System.IO.Path.GetFullPath("Doroti/artifacts/font-resolution/fonts");
        Directory.CreateDirectory(root);
        foreach (var name in new[] { "Roboto-regular.ttf", "Roboto-medium.ttf", "Roboto-bold.ttf" })
            File.Copy(System.IO.Path.Combine("samples/DorotiTestbedApp/assets/fonts", name),
                System.IO.Path.Combine(root, name), true);
        const string noto = "https://raw.githubusercontent.com/notofonts/noto-cjk/f8d157532fbfaeda587e826d4cd5b21a49186f7c/Sans/SubsetOTF/KR/";
        using var http = new HttpClient();
        var assets = new[]
        {
            ("NotoSansKR-Regular.otf", noto + "NotoSansKR-Regular.otf", "69975A0AC8472717870AEFEAB0A4D52739308D90856B9955313B2AD5E0148D68"),
            ("NotoSansKR-Bold.otf", noto + "NotoSansKR-Bold.otf", "5A6CEB287ED2FC6CFC6213144EBEA68CBD94B20FC9EB873D8486493BF02D9BDA"),
            ("NanumGothic-Regular.ttf", "https://raw.githubusercontent.com/google/fonts/ade3d1533e06b2b1462ffcde8e08b129627ca360/ofl/nanumgothic/NanumGothic-Regular.ttf", "76F45EF4A6BCFF344C837C95A7DCC26E017E38B5846D5AE0CDCB5B86BE2E2D31"),
        };
        foreach (var (name, url, expected) in assets)
        {
            var path = System.IO.Path.Combine(root, name);
            if (File.Exists(path) && Convert.ToHexString(SHA256.HashData(await File.ReadAllBytesAsync(path))) == expected)
                continue;
            var bytes = await http.GetByteArrayAsync(url);
            if (Convert.ToHexString(SHA256.HashData(bytes)) != expected)
                throw new InvalidDataException($"Font fixture checksum mismatch: {name}");
            await File.WriteAllBytesAsync(path, bytes);
        }
        return root;
    }
}
