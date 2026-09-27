// MSBuild inline task: never compiled into the native product assembly.
using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

public sealed class RestoreDorotiNativeFonts : Task
{
    [Required] public string ProjectDirectory { get; set; }
    [Required] public ITaskItem[] Assets { get; set; }
    [Required] public string Url { get; set; }
    public bool Offline { get; set; }

    public override bool Execute()
    {
        var root = Path.GetFullPath(Path.Combine(ProjectDirectory, ".doroti", "fonts"))
            + Path.DirectorySeparatorChar;
        try
        {
            Directory.CreateDirectory(root);
            using (var gate = AcquireLock(Path.Combine(root, ".restore.lock")))
            using (var http = new HttpClient { Timeout = TimeSpan.FromSeconds(60) })
            {
                byte[] archiveBytes = null;
                foreach (var asset in Assets)
                {
                    var path = Path.GetFullPath(Path.Combine(ProjectDirectory, asset.ItemSpec));
                    if (!path.StartsWith(root, StringComparison.Ordinal))
                        throw new InvalidOperationException("Native font output escapes its cache: " + path);
                    var expected = asset.GetMetadata("Sha256");
                    if (File.Exists(path) && Hash(File.ReadAllBytes(path)) == expected) continue;
                    if (Offline)
                        throw new InvalidOperationException("Offline native font asset is missing or corrupt: " + path);
                    if (archiveBytes == null)
                    {
                        Log.LogMessage(MessageImportance.High, "Restoring native Roboto fonts: {0}", Url);
                        archiveBytes = http.GetByteArrayAsync(Url).GetAwaiter().GetResult();
                    }
                    byte[] bytes;
                    using (var archive = new ZipArchive(new MemoryStream(archiveBytes), ZipArchiveMode.Read))
                    {
                        var entry = archive.GetEntry(asset.GetMetadata("ZipEntry"));
                        if (entry == null || entry.Length > 4 * 1024 * 1024)
                            throw new InvalidDataException("Invalid native font archive entry.");
                        using (var input = entry.Open())
                        using (var output = new MemoryStream())
                        {
                            input.CopyTo(output);
                            bytes = output.ToArray();
                        }
                    }
                    if (Hash(bytes) != expected)
                        throw new InvalidDataException("SHA-256 mismatch for " + asset.ItemSpec);
                    var temporary = path + "." + Guid.NewGuid().ToString("N") + ".tmp";
                    try
                    {
                        File.WriteAllBytes(temporary, bytes);
                        if (File.Exists(path)) File.Replace(temporary, path, null);
                        else File.Move(temporary, path);
                    }
                    finally { if (File.Exists(temporary)) File.Delete(temporary); }
                }
            }
            return true;
        }
        catch (Exception error)
        {
            Log.LogError("DOROTIFONT001: " + error.Message);
            return false;
        }
    }

    private static string Hash(byte[] bytes)
    {
        using (var algorithm = SHA256.Create())
            return BitConverter.ToString(algorithm.ComputeHash(bytes)).Replace("-", "");
    }

    private static FileStream AcquireLock(string path)
    {
        for (var attempt = 0; ; attempt++)
        {
            try { return new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None); }
            catch (IOException) when (attempt < 1200) { Thread.Sleep(100); }
        }
    }
}
