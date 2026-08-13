// Adapted from A0-pinned Avalonia ClipboardImpl; see migration/avalonia-shell/a1-source-port-provenance.json.
using System.ComponentModel;
using System.Runtime.InteropServices;
using Doroti.Shell.Core;

namespace Doroti.Vendor.Avalonia.Win32;

internal static class Win32ClipboardInterop
{
    private const uint UnicodeText = 13;
    private const uint MoveableZeroInitialized = 0x0042;

    internal static ShellClipboardResult GetText(nint owner)
    {
        if (!IsClipboardFormatAvailable(UnicodeText))
        {
            return ShellClipboardResult.FromText(null);
        }
        if (!TryOpenClipboard(owner))
        {
            return Failure("OpenClipboard");
        }
        try
        {
            var handle = GetClipboardData(UnicodeText);
            if (handle == 0)
            {
                return Failure("GetClipboardData");
            }
            var pointer = GlobalLock(handle);
            if (pointer == 0)
            {
                return Failure("GlobalLock");
            }
            try
            {
                return ShellClipboardResult.FromText(Marshal.PtrToStringUni(pointer));
            }
            finally
            {
                _ = GlobalUnlock(handle);
            }
        }
        finally
        {
            _ = CloseClipboard();
        }
    }

    internal static ShellClipboardResult SetText(nint owner, string text)
    {
        if (!TryOpenClipboard(owner))
        {
            return Failure("OpenClipboard");
        }
        nint memory = 0;
        try
        {
            if (!EmptyClipboard())
            {
                return Failure("EmptyClipboard");
            }
            memory = GlobalAlloc(MoveableZeroInitialized, checked((nuint)((text.Length + 1) * sizeof(char))));
            if (memory == 0)
            {
                return Failure("GlobalAlloc");
            }
            var pointer = GlobalLock(memory);
            if (pointer == 0)
            {
                return Failure("GlobalLock");
            }
            try
            {
                Marshal.Copy(text.ToCharArray(), 0, pointer, text.Length);
                Marshal.WriteInt16(pointer, checked(text.Length * sizeof(char)), 0);
            }
            finally
            {
                _ = GlobalUnlock(memory);
            }
            if (SetClipboardData(UnicodeText, memory) == 0)
            {
                return Failure("SetClipboardData");
            }
            memory = 0;
            return new(true);
        }
        finally
        {
            if (memory != 0)
            {
                _ = GlobalFree(memory);
            }
            _ = CloseClipboard();
        }
    }

    private static ShellClipboardResult Failure(string operation)
    {
        var error = Marshal.GetLastWin32Error();
        return ShellClipboardResult.Failure($"{operation} failed with Win32 error {error}: {new Win32Exception(error).Message}");
    }

    private static bool TryOpenClipboard(nint owner)
    {
        const int attempts = 10;
        for (var attempt = 0; attempt < attempts; attempt++)
        {
            if (OpenClipboard(owner))
            {
                return true;
            }
            if (attempt + 1 < attempts)
            {
                Thread.Sleep(10);
            }
        }
        return false;
    }

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool OpenClipboard(nint owner);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool CloseClipboard();

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool EmptyClipboard();

    [DllImport("user32.dll", SetLastError = true)]
    private static extern nint GetClipboardData(uint format);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern nint SetClipboardData(uint format, nint memory);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool IsClipboardFormatAvailable(uint format);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern nint GlobalAlloc(uint flags, nuint bytes);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern nint GlobalLock(nint memory);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GlobalUnlock(nint memory);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern nint GlobalFree(nint memory);
}
