using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using System.Text.Json;
using Doroti.Host.WindowsAppSdk;

if (!OperatingSystem.IsWindows())
    throw new PlatformNotSupportedException("The native ABI fixture is Windows-only.");
if (WindowsKeyMap.Physical(0x1e, 'A') != 0x00070004 ||
    WindowsKeyMap.Logical(0x1e, 'A', "A") != 'a' ||
    WindowsKeyMap.Physical(0x14b, 0x25) != 0x00070050 ||
    WindowsKeyMap.Logical(0x14b, 0x25, string.Empty) != 0x100000302 ||
    WindowsKeyMap.Logical(0x153, 0x2e, string.Empty) != 0x10000007f)
    throw new InvalidOperationException("Win32 key map is not Flutter-compatible.");
if (args.Length != 1)
    throw new ArgumentException("Expected the app-directory native DLL path.");
if (WindowsNativeV1.CompositionPresentationFeature != 1UL << 3)
    throw new InvalidOperationException("The managed Composition-presentation feature bit differs from ABI v1.");

var expectedPath = Path.GetFullPath(args[0]);
if (!File.Exists(expectedPath))
    throw new FileNotFoundException("Native ABI fixture DLL is missing.", expectedPath);
if (!string.Equals(Path.GetDirectoryName(expectedPath), AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar), StringComparison.OrdinalIgnoreCase))
    throw new InvalidOperationException("The native ABI fixture must load its DLL from the application directory.");

Environment.SetEnvironmentVariable("PATH", string.Empty);
var handle = NativeLibrary.Load("doroti_windows_appsdk_host_v1.dll", typeof(WindowsNativeV1).Assembly, null);
try
{
    var actualPath = GetModulePath(handle);
    if (!string.Equals(expectedPath, actualPath, StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException($"Native DLL search escaped the application directory: {actualPath}");

    var exports = new[]
    {
        "doroti_windows_get_abi_version_v1",
        "doroti_windows_get_abi_layout_v1",
        "doroti_windows_run_v1",
        "doroti_windows_acrylic_create_v1",
        "doroti_windows_acrylic_destroy_v1",
        "doroti_windows_acrylic_replace_buffer_v1",
        "doroti_windows_acrylic_is_available_v1",
        "doroti_windows_acrylic_present_v1",
        "doroti_windows_acrylic_present_positioned_v1",
        "doroti_windows_acrylic_present_cropped_v1",
        "doroti_windows_acrylic_crop_v1",
        "doroti_windows_acrylic_place_v1",
        "doroti_windows_vulkan_composition_create_v1",
        "doroti_windows_vulkan_composition_attach_window_v1",
        "doroti_windows_vulkan_composition_destroy_v1",
        "doroti_windows_vulkan_composition_replace_buffer_v1",
        "doroti_windows_vulkan_composition_is_available_v1",
        "doroti_windows_vulkan_composition_present_cropped_v1",
        "doroti_windows_vulkan_composition_crop_v1",
        "doroti_windows_vulkan_composition_retire_buffers_v1",
    };
    foreach (var export in exports)
    {
        if (!NativeLibrary.TryGetExport(handle, export, out _))
            throw new EntryPointNotFoundException(export);
    }

    using var stream = File.OpenRead(expectedPath);
    using var pe = new PEReader(stream);
    if (pe.PEHeaders.CoffHeader.Machine != Machine.Amd64)
        throw new BadImageFormatException($"Expected AMD64 DLL, found {pe.PEHeaders.CoffHeader.Machine}.");

    var layout = WindowsNativeV1.ValidateLayout();
    var configuration = new WindowsNativeV1.Configuration
    {
        AbiVersion = WindowsNativeV1.AbiVersion,
        StructSize = checked((uint)Marshal.SizeOf<WindowsNativeV1.Configuration>()),
    };
    var callbacks = new WindowsNativeV1.Callbacks
    {
        AbiVersion = WindowsNativeV1.AbiVersion,
        StructSize = checked((uint)Marshal.SizeOf<WindowsNativeV1.Callbacks>()),
        CompositionResize = 1,
    };
    if (WindowsNativeV1.Run(in configuration, in callbacks) != WindowsNativeV1.Status.InvalidArgument)
        throw new InvalidOperationException("The native product ABI accepted missing callbacks.");
    configuration.StructSize--;
    if (WindowsNativeV1.Run(in configuration, in callbacks) != WindowsNativeV1.Status.AbiMismatch)
        throw new InvalidOperationException("The native ABI accepted a truncated managed configuration.");
    var hostTable = new WindowsNativeV1.Host
    {
        AbiVersion = WindowsNativeV1.AbiVersion,
        StructSize = checked((uint)Marshal.SizeOf<WindowsNativeV1.Host>()),
        HostContext = 1,
        TopLevelHwnd = 1,
        ChildHwnd = 1,
        OpaqueChildHwnd = 1,
        TaskHwnd = 1,
        RequestFrame = 1,
        RequestResize = 1,
        RequestClose = 1,
        RequestShow = 1,
        RequestOpaqueFallback = 1,
        SetCursor = 1,
        SetClipboard = 1,
        RequestClipboard = 1,
        SetTextClient = 1,
        UpdateTextState = 1,
        SetCaretRect = 1,
        ClearTextClient = 1,
        UpdateSemantics = 1,
        ClearSemantics = 1,
        InitialPlatformBrightness = (uint)Doroti.Ui.Brightness.dark,
        SetCompositionChild = 1,
    };
    using (var managedHost = new WindowsManagedProductHost(in hostTable, 640, 480))
    {
        if (managedHost.Configuration.platformBrightness != Doroti.Ui.Brightness.dark)
            throw new InvalidOperationException("The managed host discarded native initial brightness.");
        var brightnessChanges = 0;
        managedHost.ConfigurationChanged += _ => brightnessChanges++;
        managedHost.ApplyPlatformBrightness((uint)Doroti.Ui.Brightness.light);
        managedHost.ApplyPlatformBrightness((uint)Doroti.Ui.Brightness.light);
        if (managedHost.Configuration.platformBrightness != Doroti.Ui.Brightness.light || brightnessChanges != 1)
            throw new InvalidOperationException("The managed host did not publish one platform-brightness change.");
    }

    Console.WriteLine(JsonSerializer.Serialize(new
    {
        schemaVersion = "doroti.windows.native-abi-validation/v1",
        gate = "C1",
        status = "PASS",
        abiVersion = WindowsNativeV1.AbiVersion,
        architecture = pe.PEHeaders.CoffHeader.Machine.ToString(),
        modulePath = actualPath,
        pathWasEmpty = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("PATH")),
        managedToNativeValidation = "PASS",
        platformBrightnessContract = "PASS",
        compositionPresentationFeature = WindowsNativeV1.CompositionPresentationFeature,
        compositionPresentationBufferCount = 3,
        exports,
        sizes = new
        {
            layout.Utf8Size,
            layout.MetricsSize,
            layout.FrameRequestSize,
            layout.HostSize,
            layout.FrameTerminalSize,
            layout.ConfigurationSize,
            layout.CallbacksSize,
            layout.GpuPointerCount,
            layout.PointerPacketSize,
            layout.KeySize,
        },
    }, new JsonSerializerOptions { WriteIndented = true }));
}
finally
{
    NativeLibrary.Free(handle);
}

static string GetModulePath(nint module)
{
    var buffer = new char[32768];
    var length = GetModuleFileName(module, buffer, checked((uint)buffer.Length));
    if (length == 0 || length == buffer.Length)
        throw new InvalidOperationException($"GetModuleFileNameW failed: {Marshal.GetLastPInvokeError()}.");
    return Path.GetFullPath(new string(buffer, 0, checked((int)length)));
}

[DllImport("kernel32.dll", EntryPoint = "GetModuleFileNameW", SetLastError = true, CharSet = CharSet.Unicode)]
static extern uint GetModuleFileName(nint module, [Out] char[] fileName, uint size);
