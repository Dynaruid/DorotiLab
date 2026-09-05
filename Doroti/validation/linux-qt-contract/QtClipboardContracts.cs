using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Doroti.Host.Qt;

internal static class QtClipboardContracts
{
    private static ulong _request;

    internal static unsafe void Verify()
    {
        // The native implementation queues QClipboard work on the GUI thread;
        // emulate its delayed callback while retaining the actual managed ABI.
        var bytes = new byte[Marshal.SizeOf<QtNativeV2.HostApi>()];
        fixed (byte* buffer = bytes)
            *(nint*)(buffer + (int)Marshal.OffsetOf<QtNativeV2.HostApi>("RequestClipboardText")) =
                (nint)(delegate* unmanaged[Cdecl]<nint, ulong, void>)&Request;
        var api = MemoryMarshal.Read<QtNativeV2.HostApi>(bytes);
        using var host = new QtHostAdapter(1, api, 100, 100);
        var guiThread = Environment.CurrentManagedThreadId;
        var read = Observe(host.GetClipboardTextAsync());
        if (read.IsCompleted || _request == 0) throw new InvalidOperationException("Qt clipboard read did not await its callback.");
        host.CompleteClipboard(_request, "Qt 한글");
        var result = read.GetAwaiter().GetResult();
        if (result.Text != "Qt 한글" || result.Thread != guiThread)
            throw new InvalidOperationException("Qt clipboard continuation left the GUI callback thread.");
        var pending = host.GetClipboardTextAsync().AsTask();
        host.Dispose();
        if (!pending.IsCanceled) throw new InvalidOperationException("Qt clipboard read survived host disposal.");
        Console.WriteLine("Doroti Linux Qt clipboard callback and disposal: PASS");
    }

    private static async Task<(string? Text, int Thread)> Observe(ValueTask<string?> read) =>
        (await read, Environment.CurrentManagedThreadId);

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void Request(nint view, ulong request) => _request = request;
}
