using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using Doroti.Ui;

namespace Doroti.Host.WindowsAppSdk;

internal static class WindowsDropData
{
    private const short Files = 15, UnicodeText = 13;
    private static readonly short Url = unchecked((short)RegisterClipboardFormatW("UniformResourceLocatorW"));
    private static readonly short UriList = unchecked((short)RegisterClipboardFormatW("text/uri-list"));
    private const int MaximumTextBytes = 1024 * 1024;
    private static FORMATETC Format(short id) => new() { cfFormat = id, dwAspect = DVASPECT.DVASPECT_CONTENT, lindex = -1, tymed = TYMED.TYMED_HGLOBAL };
    private static bool Has(IDataObject data, short id)
    {
        var format = Format(id);
        try { return data.QueryGetData(ref format) == 0; } catch (COMException) { return false; }
    }
    internal static IReadOnlyList<string> Formats(IDataObject data)
    {
        var formats = new List<string>();
        if (Has(data, Files)) formats.Add(OsDropFormats.Files);
        if (Has(data, UnicodeText)) formats.Add(OsDropFormats.Text);
        if (Has(data, Url) || Has(data, UriList)) formats.Add(OsDropFormats.UriList);
        return Array.AsReadOnly(formats.ToArray());
    }
    private static T Read<T>(IDataObject data, short id, Func<nint, T> read)
    {
        var format = Format(id);
        data.GetData(ref format, out var medium);
        try
        {
            if (medium.tymed != TYMED.TYMED_HGLOBAL || medium.unionmember == 0) throw new InvalidDataException("Drop format requires HGLOBAL data.");
            return read(medium.unionmember);
        }
        finally { ReleaseStgMedium(ref medium); }
    }

    internal static OsDropData Acquire(IDataObject data, IReadOnlyList<string> formats)
    {
        var files = new List<IPickedFile>();
        try
        {
            if (formats.Contains(OsDropFormats.Files))
            {
                Read(data, Files, handle =>
                {
                    var count = DragQueryFileW(handle, uint.MaxValue, null, 0);
                    if (count is 0 or > 1024) throw new InvalidDataException("A drop must contain 1..1024 filesystem files.");
                    for (uint index = 0; index < count; index++)
                    {
                        var length = DragQueryFileW(handle, index, null, 0);
                        if (length is 0 or > 32767) throw new InvalidDataException("Invalid dropped filename length.");
                        var path = new StringBuilder(checked((int)length + 1));
                        if (DragQueryFileW(handle, index, path, checked((uint)path.Capacity)) != length) throw new InvalidDataException("Dropped filename changed while reading.");
                        files.Add(new WindowsReadFile(path.ToString()));
                    }
                    return 0;
                });
            }
            var text = formats.Contains(OsDropFormats.Text) ? Read(data, UnicodeText, handle => ReadString(handle, true)) : null;
            var uris = new List<Uri>();
            if (formats.Contains(OsDropFormats.UriList))
            {
                var unicode = Has(data, Url);
                var list = Read(data, unicode ? Url : UriList, handle => ReadString(handle, unicode));
                foreach (var line in list.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                {
                    if (line.StartsWith('#')) continue;
                    if (!Uri.TryCreate(line, UriKind.Absolute, out var uri)) throw new InvalidDataException("Drop contains a non-absolute URI.");
                    uris.Add(uri);
                }
                if (uris.Count == 0) throw new InvalidDataException("URI drop is empty.");
            }
            return new(files, text, uris);
        }
        catch { foreach (var file in files) file.Dispose(); throw; }
    }

    private static string ReadString(nint handle, bool unicode)
    {
        var size = GlobalSize(handle);
        if (size == 0 || size > MaximumTextBytes || (unicode && size % 2 != 0)) throw new InvalidDataException("Drop text exceeds 1 MiB or has invalid encoding length.");
        var pointer = GlobalLock(handle);
        if (pointer == 0) throw new InvalidDataException("Drop text could not be locked.");
        try
        {
            var bytes = new byte[checked((int)size)];
            Marshal.Copy(pointer, bytes, 0, bytes.Length);
            var end = -1;
            for (var index = 0; index < bytes.Length; index += unicode ? 2 : 1)
                if (bytes[index] == 0 && (!unicode || bytes[index + 1] == 0)) { end = index; break; }
            if (end < 0) throw new InvalidDataException("Drop text is not terminated.");
            Encoding encoding = unicode ? new UnicodeEncoding(false, false, true) : new UTF8Encoding(false, true);
            try { return encoding.GetString(bytes, 0, end); }
            catch (DecoderFallbackException error) { throw new InvalidDataException("Drop text contains invalid encoded characters.", error); }
        }
        finally { GlobalUnlock(handle); }
    }
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] private static extern uint RegisterClipboardFormatW(string name);
    [DllImport("shell32.dll", CharSet = CharSet.Unicode)] private static extern uint DragQueryFileW(nint drop, uint index, StringBuilder? path, uint length);
    [DllImport("ole32.dll")] private static extern void ReleaseStgMedium(ref STGMEDIUM medium);
    [DllImport("kernel32.dll")] private static extern nuint GlobalSize(nint memory);
    [DllImport("kernel32.dll")] private static extern nint GlobalLock(nint memory);
    [DllImport("kernel32.dll")] private static extern bool GlobalUnlock(nint memory);
}
