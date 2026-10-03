using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace Doroti.Hosting;

/// <summary>Explicit serializable draft checkpoint. Save on edits, not only on native close.</summary>
public sealed partial class DocumentRecoveryStore(string path)
{
    private readonly object _gate = new();
    private sealed record Draft(int Version, string Text, string Sha256);
    [JsonSerializable(typeof(Draft))]
    private partial class RecoveryJson : JsonSerializerContext;
    public void Save(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        var bytes = Encoding.UTF8.GetBytes(text);
        if (bytes.Length > 1024 * 1024) throw new ArgumentOutOfRangeException(nameof(text), "Drafts are bounded to 1 MiB.");
        lock (_gate)
        {
            var target = Path.GetFullPath(path);
            Directory.CreateDirectory(Path.GetDirectoryName(target)!);
            var temporary = target + ".tmp";
            try
            {
                File.WriteAllText(temporary, JsonSerializer.Serialize(new Draft(1, text, Convert.ToHexString(SHA256.HashData(bytes))), RecoveryJson.Default.Draft));
                File.Move(temporary, target, overwrite: true);
            }
            finally { if (File.Exists(temporary)) File.Delete(temporary); }
        }
    }
    public bool TryRead(out string text)
    {
        text = "";
        lock (_gate)
        {
            try
            {
                if (!File.Exists(path) || new FileInfo(path).Length > 8 * 1024 * 1024) return false;
                var draft = JsonSerializer.Deserialize(File.ReadAllText(path), RecoveryJson.Default.Draft);
                if (draft is null || draft.Version != 1 || draft.Text is null ||
                    Encoding.UTF8.GetByteCount(draft.Text) > 1024 * 1024 ||
                    draft.Sha256 != Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(draft.Text)))) return false;
                text = draft.Text;
                return true;
            }
            catch (Exception error) when (error is IOException or UnauthorizedAccessException or JsonException) { return false; }
        }
    }
}
