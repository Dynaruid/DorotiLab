using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Doroti.DartToCSharp;

internal sealed record AnalyzerCacheEntryHeader(
    string SchemaVersion,
    string CacheKey,
    string PayloadSha256,
    AnalyzerDependencyFingerprint[] Dependencies);

internal sealed record AnalyzerCacheStatus(int EntryCount, long Bytes, DateTimeOffset? OldestWriteUtc, DateTimeOffset? NewestWriteUtc);

internal sealed class AnalyzerCacheStore(string cacheDirectory, AnalyzerSessionIdentity identity, CompilerProfiler profiler)
{
    private readonly string _root = Path.GetFullPath(cacheDirectory);

    public bool TryRead(string cacheKey, out string payload)
    {
        var path = EntryPath(cacheKey);
        if (!File.Exists(path))
        {
            payload = string.Empty;
            return false;
        }
        try
        {
            byte[] uncompressed;
            using (profiler.MeasureLibrary("cache-read-decompress", cacheKey))
            using (var source = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read | FileShare.Delete))
            using (var brotli = new BrotliStream(source, CompressionMode.Decompress))
            using (var destination = new MemoryStream())
            {
                brotli.CopyTo(destination);
                uncompressed = destination.ToArray();
            }
            var separator = Array.IndexOf(uncompressed, (byte)'\n');
            if (separator <= 0) throw new InvalidDataException("Analyzer cache header delimiter is missing.");
            var header = JsonSerializer.Deserialize<AnalyzerCacheEntryHeader>(uncompressed.AsSpan(0, separator))
                ?? throw new InvalidDataException("Analyzer cache header is empty.");
            if (header.SchemaVersion != "doroti.dart-analyzer-cache/v2" || header.CacheKey != cacheKey)
            {
                payload = string.Empty;
                return false;
            }
            foreach (var dependency in header.Dependencies)
            {
                if (!File.Exists(dependency.Path) ||
                    !string.Equals(identity.HashFile(dependency.Path), dependency.Sha256, StringComparison.Ordinal))
                {
                    payload = string.Empty;
                    return false;
                }
            }
            var payloadBytes = uncompressed.AsSpan(separator + 1);
            var payloadHash = Convert.ToHexString(SHA256.HashData(payloadBytes)).ToLowerInvariant();
            if (!string.Equals(payloadHash, header.PayloadSha256, StringComparison.Ordinal))
            {
                throw new InvalidDataException("Analyzer cache payload hash mismatch.");
            }
            payload = Encoding.UTF8.GetString(payloadBytes);
            profiler.RecordCacheHit(new FileInfo(path).Length);
            return true;
        }
        catch (Exception exception) when (exception is InvalidDataException or IOException or JsonException)
        {
            Quarantine(path);
            payload = string.Empty;
            return false;
        }
    }

    public void Write(string cacheKey, string payload, IReadOnlyList<AnalyzerDependencyFingerprint> dependencies)
    {
        Directory.CreateDirectory(_root);
        var path = EntryPath(cacheKey);
        var payloadBytes = Encoding.UTF8.GetBytes(payload);
        var header = new AnalyzerCacheEntryHeader(
            "doroti.dart-analyzer-cache/v2",
            cacheKey,
            Convert.ToHexString(SHA256.HashData(payloadBytes)).ToLowerInvariant(),
            dependencies.OrderBy(item => item.Path, StringComparer.OrdinalIgnoreCase).ToArray());
        var headerBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(header));
        var temporary = path + ".tmp." + Environment.ProcessId + "." + Guid.NewGuid().ToString("N");
        try
        {
            using (profiler.MeasureLibrary("cache-compress-write", cacheKey))
            using (var destination = new FileStream(temporary, FileMode.CreateNew, FileAccess.Write, FileShare.None, 65536, FileOptions.WriteThrough))
            {
                using (var brotli = new BrotliStream(destination, CompressionLevel.Optimal, leaveOpen: true))
                {
                    brotli.Write(headerBytes);
                    brotli.WriteByte((byte)'\n');
                    brotli.Write(payloadBytes);
                }
                destination.Flush(flushToDisk: true);
            }
            File.Move(temporary, path, overwrite: true);
            profiler.RecordCacheWrite(new FileInfo(path).Length);
        }
        finally
        {
            if (File.Exists(temporary)) File.Delete(temporary);
        }
    }

    public AnalyzerCacheStatus Status()
        => ReadStatus(_root);

    public static AnalyzerCacheStatus ReadStatus(string cacheDirectory)
    {
        var root = Path.GetFullPath(cacheDirectory);
        if (!Directory.Exists(root)) return new(0, 0, null, null);
        var files = Directory.GetFiles(root, "*.entry.br", SearchOption.TopDirectoryOnly).Select(path => new FileInfo(path)).ToArray();
        return new(
            files.Length,
            files.Sum(item => item.Length),
            files.Length == 0 ? null : files.Min(item => item.LastWriteTimeUtc),
            files.Length == 0 ? null : files.Max(item => item.LastWriteTimeUtc));
    }

    public int Prune(long maximumBytes, TimeSpan maximumAge)
        => Prune(_root, maximumBytes, maximumAge);

    public static int Prune(string cacheDirectory, long maximumBytes, TimeSpan maximumAge)
    {
        var root = Path.GetFullPath(cacheDirectory);
        if (!Directory.Exists(root)) return 0;
        var cutoff = DateTime.UtcNow - maximumAge;
        var files = Directory.GetFiles(root, "*.entry.br", SearchOption.TopDirectoryOnly)
            .Select(path => new FileInfo(path))
            .OrderBy(item => item.LastAccessTimeUtc)
            .ThenBy(item => item.Name, StringComparer.Ordinal)
            .ToList();
        var bytes = files.Sum(item => item.Length);
        var removed = 0;
        foreach (var file in files)
        {
            if (file.LastWriteTimeUtc >= cutoff && bytes <= maximumBytes) continue;
            var length = file.Length;
            file.Delete();
            bytes -= length;
            removed++;
        }
        return removed;
    }

    private string EntryPath(string cacheKey) => Path.Combine(_root, cacheKey + ".entry.br");

    private static void Quarantine(string path)
    {
        try
        {
            File.Move(path, path + ".corrupt." + Guid.NewGuid().ToString("N"));
        }
        catch (IOException)
        {
        }
    }
}
