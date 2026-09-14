using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Doroti.Hosting;
using Doroti.Ui;

[SupportedOSPlatform("windows")]
internal static unsafe class WindowsTrackpadContracts
{
    internal static void Verify()
    {
        var hr = CoInitializeEx(0, 2);
        var hwnd = CreateWindowExW(0, "STATIC", "Doroti trackpad contract", 0, 0, 0, 320, 240, 0, 0, 0, 0);
        if (hwnd == 0) throw new InvalidOperationException("Cannot create hidden trackpad fixture window");
        try
        {
            var packets = new List<PointerData>();
            using var input = new WindowsPrecisionTrackpad(hwnd, 1, packet => packets.AddRange(packet.data));
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            var type = typeof(WindowsPrecisionTrackpad);
            var viewport = (nint)type.GetField("_viewport", flags)!.GetValue(input)!;
            var status = type.GetMethod("Status", flags)!;
            var content = type.GetMethod("Content", flags)!;
            // Real COM initialization/registration above; deterministic transforms
            // below reproduce Flutter's direct_manipulation_unittests sequences.
            status.Invoke(input, [viewport, 3u, 5u]);
            nint* table = stackalloc nint[10];
            table[9] = (nint)(delegate* unmanaged[Stdcall]<FakeContent*, float*, uint, int>)&Read;
            var fake = new FakeContent { Vtable = table, Scale = 2, X = 12, Y = 24 };
            content.Invoke(input, [(nint)(&fake)]);
            Require(packets.Count >= 3 && packets[^1].change == PointerChange.panZoomUpdate &&
                packets[^1].panX == 12 && packets[^1].panY == 24 && packets[^1].scale == 2,
                "Windows COM transform reaches pan/zoom packet");
            status.Invoke(input, [viewport, 4u, 3u]);
            Require(packets[^1].change == PointerChange.panZoomEnd, "inertia transition ends contact");
            var count = packets.Count;
            fake.X = 20; content.Invoke(input, [(nint)(&fake)]);
            Require(packets.Count == count, "OS inertia must not double-scroll the framework");
            status.Invoke(input, [viewport, 5u, 4u]);
            Require(packets.Any(p => p.signalKind == PointerSignalKind.scrollInertiaCancel), "interrupted inertia signal");
            input.Dispose();
            Require(packets[^1].change == PointerChange.remove, "Windows device removed on dispose");
            Console.WriteLine("PASS: real Windows DirectManipulation COM initialization, transform/phase contract and cleanup (hidden HWND; synthetic transforms)");
            VerifyNativeContacts(hwnd);
        }
        finally { DestroyWindow(hwnd); if (hr >= 0) CoUninitialize(); }
    }

    private static void VerifyNativeContacts(nint hwnd)
    {
        var packets = new List<PointerData>();
        using var input = new WindowsNativePointerInput(hwnd, 1, packet => packets.AddRange(packet.data));
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
        var type = typeof(WindowsNativePointerInput);
        var infoType = type.GetNestedType("PointerInfo", BindingFlags.NonPublic)!;
        var penType = type.GetNestedType("PenInfo", BindingFlags.NonPublic)!;
        Require(Marshal.SizeOf(infoType) == 96 && Marshal.SizeOf(penType) == 120, "Windows pointer ABI sizes");
        var info = Activator.CreateInstance(infoType)!;
        var pen = Activator.CreateInstance(penType)!;
        var process = type.GetMethod("Process", flags)!;
        void Send(uint id, uint kind, uint message, uint pointerFlags, double x)
        {
            infoType.GetField("Type")!.SetValue(info, kind);
            infoType.GetField("Id")!.SetValue(info, id);
            infoType.GetField("Flags")!.SetValue(info, pointerFlags);
            process.Invoke(input, [message, info, pen, x, 60d]);
        }
        Send(1, 2, 0x0246, 4 | 16, 10);
        var first = packets[^1];
        Send(2, 2, 0x0246, 4 | 16, 30);
        var second = packets[^1];
        Require(first.kind == PointerDeviceKind.touch && first.pointerIdentifier != second.pointerIdentifier && first.device != second.device,
            "simultaneous Windows touch contacts have independent identities");
        Send(1, 2, 0x0247, 0, 10);
        Send(2, 2, 0x0245, 4 | 16, 35);
        Require(packets[^1].pointerIdentifier == second.pointerIdentifier && packets[^1].physicalDeltaX == 5,
            "ending one contact must not end another");
        penType.GetField("Flags")!.SetValue(pen, 6u);
        penType.GetField("Mask")!.SetValue(pen, 3u);
        penType.GetField("Pressure")!.SetValue(pen, 512u);
        penType.GetField("Rotation")!.SetValue(pen, 90u);
        Send(3, 3, 0x0246, 4 | 16, 50);
        Require(packets[^1].kind == PointerDeviceKind.invertedStylus && packets[^1].pressure == .5 &&
            Math.Abs(packets[^1].orientation - Math.PI / 2) < 1e-9, "eraser/pressure/orientation survive native input");
        input.Dispose();
        Require(packets.Count(p => p.change == PointerChange.cancel) == 2 && packets[^1].change == PointerChange.remove,
            "disposing Windows contacts cancels and removes remaining touch and pen");
        Console.WriteLine("PASS: Windows WM_POINTER contact reducer, multitouch identity, eraser/pressure and cancel/remove lifecycle (synthetic native records)");
    }
    private struct FakeContent { public nint* Vtable; public float Scale, X, Y; }
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static int Read(FakeContent* content, float* matrix, uint count)
    { matrix[0] = matrix[3] = content->Scale; matrix[1] = matrix[2] = 0; matrix[4] = content->X; matrix[5] = content->Y; return 0; }
    private static void Require(bool value, string message) { if (!value) throw new InvalidOperationException(message); }
    [DllImport("ole32.dll")] private static extern int CoInitializeEx(nint reserved, uint flags);
    [DllImport("ole32.dll")] private static extern void CoUninitialize();
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] private static extern nint CreateWindowExW(uint ex, string cls, string title, uint style, int x, int y, int width, int height, nint parent, nint menu, nint instance, nint parameter);
    [DllImport("user32.dll")] [return: MarshalAs(UnmanagedType.Bool)] private static extern bool DestroyWindow(nint window);
}
