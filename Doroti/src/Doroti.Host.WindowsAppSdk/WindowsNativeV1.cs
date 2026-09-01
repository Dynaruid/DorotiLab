using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.Json;

namespace Doroti.Host.WindowsAppSdk;

internal static partial class WindowsNativeV1
{
    internal const uint AbiVersion = 1;
    internal const ulong ExperimentalAcrylicFeature = 1UL << 0;
    internal const string LibraryName = "doroti_windows_appsdk_host_v1";
    private const uint LoadLibrarySearchDllLoadDir = 0x00000100;
    private const uint LoadLibrarySearchApplicationDir = 0x00000200;
    private const uint LoadLibrarySearchUserDirs = 0x00000400;
    private const uint LoadLibrarySearchSystem32 = 0x00000800;
    private static int _resolverConfigured;
    private static int _searchPolicyRestricted;
    private static string? _nativeHostPath;
    private static string? _angleRuntimePath;
    private static string? _windowsAppRuntimePath;

    internal enum Status : uint
    {
        Ok = 0,
        InvalidArgument = 1,
        AbiMismatch = 2,
        NotImplemented = 3,
        NativeFailure = 4,
    }

    internal enum FrameTerminalKind : uint
    {
        Presented = 1,
        Superseded = 2,
        Failed = 3,
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct Utf8
    {
        internal uint AbiVersion;
        internal uint StructSize;
        internal nint Data;
        internal ulong ByteLength;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct Metrics
    {
        internal uint AbiVersion;
        internal uint StructSize;
        internal ulong ViewId;
        internal ulong Generation;
        internal uint WidthPx;
        internal uint HeightPx;
        internal double Scale;
        internal double LogicalWidth;
        internal double LogicalHeight;
        internal ulong DisplayId;
        internal long TimestampQpc;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct FrameRequest
    {
        internal uint AbiVersion;
        internal uint StructSize;
        internal ulong ViewId;
        internal ulong Generation;
        internal uint WidthPx;
        internal uint HeightPx;
        internal ulong CausalFrameId;
        internal long TimestampQpc;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct FrameTerminal
    {
        internal uint AbiVersion;
        internal uint StructSize;
        internal ulong ViewId;
        internal ulong Generation;
        internal ulong CausalFrameId;
        internal uint TerminalKind;
        internal uint ErrorCategory;
        internal long AcceptedQpc;
        internal long TerminalQpc;
        internal uint PlatformWaitTimedOut;
        internal uint Reserved;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct Pointer
    {
        internal uint AbiVersion;
        internal uint StructSize;
        internal ulong ViewId;
        internal long TimestampQpc;
        internal uint Change;
        internal uint Kind;
        internal long Device;
        internal double PhysicalX;
        internal double PhysicalY;
        internal double PhysicalDeltaX;
        internal double PhysicalDeltaY;
        internal long Buttons;
        internal double ScrollDeltaX;
        internal double ScrollDeltaY;
        internal uint SignalKind;
        internal uint PointerIdentifier;
        internal double Pressure;
        internal double Tilt;
        internal long PlatformData;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct Key
    {
        internal uint AbiVersion;
        internal uint StructSize;
        internal ulong ViewId;
        internal long TimestampQpc;
        internal uint Type;
        internal uint Repeat;
        internal long Physical;
        internal long Logical;
        internal Utf8 Character;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct TextConfiguration
    {
        internal uint AbiVersion;
        internal uint StructSize;
        internal uint InputType;
        internal uint InputAction;
        internal uint Capitalization;
        internal uint Flags;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct TextState
    {
        internal uint AbiVersion;
        internal uint StructSize;
        internal Utf8 Text;
        internal int SelectionBase;
        internal int SelectionExtent;
        internal int ComposingBase;
        internal int ComposingExtent;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct Host
    {
        internal uint AbiVersion;
        internal uint StructSize;
        internal nint HostContext;
        internal nint TopLevelHwnd;
        internal nint ChildHwnd;
        internal nint OpaqueChildHwnd;
        internal nint TaskHwnd;
        internal nint RequestFrame;
        internal nint RequestResize;
        internal nint RequestClose;
        internal nint RequestShow;
        internal nint RequestOpaqueFallback;
        internal nint SetCursor;
        internal nint SetClipboard;
        internal nint RequestClipboard;
        internal nint SetTextClient;
        internal nint UpdateTextState;
        internal nint SetCaretRect;
        internal nint ClearTextClient;
        internal nint UpdateSemantics;
        internal nint ClearSemantics;
        internal uint InitialPlatformBrightness;
        internal nint SetCompositionChild;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct Configuration
    {
        internal uint AbiVersion;
        internal uint StructSize;
        internal ulong RequiredFeatures;
        internal Utf8 ApplicationId;
        internal Utf8 Title;
        internal uint InitialWidthPx;
        internal uint InitialHeightPx;
        internal uint NCmdShow;
        internal uint Reserved;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct Callbacks
    {
        internal uint AbiVersion;
        internal uint StructSize;
        internal nint CallbackContext;
        internal nint HostReady;
        internal nint Metrics;
        internal nint Render;
        internal nint FrameTerminal;
        internal nint Log;
        internal nint Pointer;
        internal nint Key;
        internal nint Focus;
        internal nint Clipboard;
        internal nint TextEditing;
        internal nint TextAction;
        internal nint SemanticsAction;
        internal nint Lifecycle;
        internal nint PlatformBrightness;
        internal nint PlatformResourcesShutdown;
        internal nint CompositionResize;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct AbiLayout
    {
        internal uint AbiVersion;
        internal uint StructSize;
        internal uint PointerSize;
        internal uint Packing;
        internal uint Utf8Size;
        internal uint MetricsSize;
        internal uint FrameRequestSize;
        internal uint HostSize;
        internal uint FrameTerminalSize;
        internal uint ConfigurationSize;
        internal uint CallbacksSize;
        internal uint MetricsGenerationOffset;
        internal uint HostChildHwndOffset;
        internal uint TerminalKindOffset;
        internal uint CallbacksRenderOffset;
        internal uint GpuPointerCount;
        internal uint PointerPacketSize;
        internal uint KeySize;
        internal uint CallbacksPointerOffset;
        internal uint HostSetCursorOffset;
        internal uint TextConfigurationSize;
        internal uint TextStateSize;
        internal uint HostSetTextClientOffset;
        internal uint CallbacksTextEditingOffset;
        internal uint CallbacksLifecycleOffset;
        internal uint HostInitialPlatformBrightnessOffset;
        internal uint CallbacksPlatformBrightnessOffset;
    }

    [LibraryImport(LibraryName, EntryPoint = "doroti_windows_get_abi_version_v1")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial uint GetAbiVersion();

    [LibraryImport(LibraryName, EntryPoint = "doroti_windows_get_abi_layout_v1")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial Status GetAbiLayout(ref AbiLayout layout);

    [LibraryImport(LibraryName, EntryPoint = "doroti_windows_run_v1")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial Status Run(in Configuration configuration, in Callbacks callbacks);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool SetDefaultDllDirectories(uint directoryFlags);

    [LibraryImport("kernel32.dll", EntryPoint = "LoadLibraryExW", StringMarshalling = StringMarshalling.Utf16,
        SetLastError = true)]
    private static partial nint LoadLibraryEx(string fileName, nint file, uint flags);

    [LibraryImport("Microsoft.WindowsAppRuntime.dll", EntryPoint = "WindowsAppRuntime_EnsureIsLoaded")]
    private static partial int EnsureWindowsAppRuntimeLoaded();

    internal static void EnsureSelfContainedWindowsAppRuntime()
    {
        if (_windowsAppRuntimePath is null) return;
        Environment.SetEnvironmentVariable("MICROSOFT_WINDOWSAPPRUNTIME_BASE_DIRECTORY", AppContext.BaseDirectory);
        Environment.SetEnvironmentVariable("MICROSOFT_WINDOWSAPPRUNTIME_BASE_DIRECTORY_PID",
            Environment.ProcessId.ToString(System.Globalization.CultureInfo.InvariantCulture));
        var result = EnsureWindowsAppRuntimeLoaded();
        if (result < 0) Marshal.ThrowExceptionForHR(result);
    }

    internal static NativeHostProvenance ConfigureAppDirectoryLoading()
    {
        var baseDirectory = Path.GetFullPath(AppContext.BaseDirectory);
        var hostPath = Path.Combine(baseDirectory, $"{LibraryName}.dll");
        var bootstrapPath = Path.Combine(baseDirectory, "Microsoft.WindowsAppRuntime.Bootstrap.dll");
        var angleRuntimePath = Path.Combine(baseDirectory, "av_libglesv2.dll");
        var windowsAppRuntimePath = Path.Combine(baseDirectory, "Microsoft.WindowsAppRuntime.dll");
        RequireNativeFile(hostPath, "native HwndExactCpp host");
        RequireNativeFile(bootstrapPath, "Windows App Runtime bootstrap");
        RequireNativeFile(angleRuntimePath, "ANGLE EGL/GLES runtime");
        ValidateX64Pe(hostPath, "native HwndExactCpp host");
        ValidateX64Pe(bootstrapPath, "Windows App Runtime bootstrap");
        ValidateX64Pe(angleRuntimePath, "ANGLE EGL/GLES runtime");
        if (Interlocked.Exchange(ref _resolverConfigured, 1) == 0)
        {
            _nativeHostPath = hostPath;
            _angleRuntimePath = angleRuntimePath;
            _windowsAppRuntimePath = File.Exists(windowsAppRuntimePath) ? windowsAppRuntimePath : null;
            NativeLibrary.SetDllImportResolver(typeof(WindowsNativeV1).Assembly, ResolveNativeLibrary);
        }
        var auditHashes = string.Equals(
            Environment.GetEnvironmentVariable("DOROTI_WINDOWS_NATIVE_AUDIT"),
            "1",
            StringComparison.Ordinal);
        var host = new FileInfo(hostPath);
        var bootstrap = new FileInfo(bootstrapPath);
        var angleRuntime = new FileInfo(angleRuntimePath);
        var hostHash = auditHashes ? Sha256(hostPath) : null;
        var bootstrapHash = auditHashes ? Sha256(bootstrapPath) : null;
        var angleRuntimeHash = auditHashes ? Sha256(angleRuntimePath) : null;
        if (auditHashes)
            ValidateBuildProvenance(baseDirectory, hostHash!, bootstrapHash!, angleRuntimeHash!);
        return new(baseDirectory, hostPath, host.Length, host.LastWriteTimeUtc.Ticks, hostHash,
            bootstrapPath, bootstrap.Length, bootstrap.LastWriteTimeUtc.Ticks, bootstrapHash,
            angleRuntimePath, angleRuntime.Length, angleRuntime.LastWriteTimeUtc.Ticks, angleRuntimeHash,
            auditHashes,
            "app-directory + DLL-load-directory + System32 + registered user directories; PATH/current-directory excluded");
    }

    private static nint ResolveNativeLibrary(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
    {
        _ = assembly;
        _ = searchPath;
        var path = libraryName switch
        {
            var value when value.Equals(LibraryName, StringComparison.Ordinal) => _nativeHostPath,
            var value when value.Equals("av_libglesv2.dll", StringComparison.OrdinalIgnoreCase) => _angleRuntimePath,
            var value when value.Equals("Microsoft.WindowsAppRuntime.dll", StringComparison.OrdinalIgnoreCase) =>
                _windowsAppRuntimePath,
            _ => null,
        };
        if (path is null) return 0;
        var handle = LoadLibraryEx(path, 0, LoadLibrarySearchDllLoadDir | LoadLibrarySearchSystem32);
        if (handle != 0) return handle;
        var error = Marshal.GetLastPInvokeError();
        if (error == 193)
            throw new BadImageFormatException($"The native HwndExactCpp host is not a win-x64 PE image: {path}");
        throw new DllNotFoundException(
            $"The native HwndExactCpp host or one of its app-directory dependencies failed to load: {path} (Win32={error}).");
    }

    internal static void RestrictProcessDllSearch()
    {
        if (Interlocked.Exchange(ref _searchPolicyRestricted, 1) != 0) return;
        var directories = LoadLibrarySearchApplicationDir | LoadLibrarySearchUserDirs |
                          LoadLibrarySearchSystem32;
        if (!SetDefaultDllDirectories(directories))
            throw new InvalidOperationException(
                $"Failed to restrict native DLL search directories (Win32={Marshal.GetLastPInvokeError()}).");
    }

    private static void RequireNativeFile(string path, string identity)
    {
        if (!File.Exists(path))
            throw new DllNotFoundException($"Required {identity} is missing from the application directory: {path}");
    }

    internal static void ValidateX64Pe(string path, string identity)
    {
        using var stream = File.OpenRead(path);
        Span<byte> header = stackalloc byte[64];
        if (stream.Read(header) != header.Length || header[0] != (byte)'M' || header[1] != (byte)'Z')
            throw new BadImageFormatException($"The {identity} is not a PE image: {path}");
        var peOffset = BitConverter.ToInt32(header[0x3c..]);
        if (peOffset < 0 || peOffset > stream.Length - 6)
            throw new BadImageFormatException($"The {identity} has an invalid PE header: {path}");
        stream.Position = peOffset;
        Span<byte> signature = stackalloc byte[6];
        if (stream.Read(signature) != signature.Length ||
            signature[0] != (byte)'P' || signature[1] != (byte)'E' ||
            signature[2] != 0 || signature[3] != 0 || BitConverter.ToUInt16(signature[4..]) != 0x8664)
            throw new BadImageFormatException($"The {identity} is not a win-x64 PE image: {path}");
    }

    private static string Sha256(string path) =>
        Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path))).ToLowerInvariant();

    private static void ValidateBuildProvenance(
        string baseDirectory,
        string hostHash,
        string bootstrapHash,
        string angleRuntimeHash)
    {
        var path = Path.Combine(baseDirectory, "doroti-native-provenance.json");
        if (!File.Exists(path))
            throw new InvalidDataException($"Native provenance audit manifest is missing: {path}");
        var manifest = JsonSerializer.Deserialize<NativeBuildProvenance>(File.ReadAllText(path),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
            ?? throw new InvalidDataException($"Native provenance audit manifest is empty: {path}");
        if (manifest.SchemaVersion != "doroti.windows.native-provenance/v1" ||
            !string.Equals(manifest.NativeHostSha256, hostHash, StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(manifest.BootstrapSha256, bootstrapHash, StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(manifest.AngleRuntimeSha256, angleRuntimeHash, StringComparison.OrdinalIgnoreCase))
            throw new InvalidDataException("Native provenance audit hash does not match the build/publish manifest.");
    }

    internal static AbiLayout ValidateLayout()
    {
        if (GetAbiVersion() != AbiVersion)
            throw new InvalidOperationException("Native host ABI version mismatch.");
        var layout = new AbiLayout
        {
            AbiVersion = AbiVersion,
            StructSize = checked((uint)Marshal.SizeOf<AbiLayout>()),
        };
        var status = GetAbiLayout(ref layout);
        if (status != Status.Ok)
            throw new InvalidOperationException($"Native ABI layout query failed: {status}.");

        AssertEqual("pointer size", checked((uint)IntPtr.Size), layout.PointerSize);
        AssertEqual("packing", 8, layout.Packing);
        AssertEqual("UTF-8 size", SizeOf<Utf8>(), layout.Utf8Size);
        AssertEqual("metrics size", SizeOf<Metrics>(), layout.MetricsSize);
        AssertEqual("frame request size", SizeOf<FrameRequest>(), layout.FrameRequestSize);
        AssertEqual("host size", SizeOf<Host>(), layout.HostSize);
        AssertEqual("frame terminal size", SizeOf<FrameTerminal>(), layout.FrameTerminalSize);
        AssertEqual("configuration size", SizeOf<Configuration>(), layout.ConfigurationSize);
        AssertEqual("callbacks size", SizeOf<Callbacks>(), layout.CallbacksSize);
        AssertEqual("metrics generation offset", OffsetOf<Metrics>(nameof(Metrics.Generation)), layout.MetricsGenerationOffset);
        AssertEqual("host child HWND offset", OffsetOf<Host>(nameof(Host.ChildHwnd)), layout.HostChildHwndOffset);
        AssertEqual("terminal kind offset", OffsetOf<FrameTerminal>(nameof(FrameTerminal.TerminalKind)), layout.TerminalKindOffset);
        AssertEqual("callbacks render offset", OffsetOf<Callbacks>(nameof(Callbacks.Render)), layout.CallbacksRenderOffset);
        AssertEqual("GPU pointer count", 0, layout.GpuPointerCount);
        AssertEqual("pointer packet size", SizeOf<Pointer>(), layout.PointerPacketSize);
        AssertEqual("key packet size", SizeOf<Key>(), layout.KeySize);
        AssertEqual("callbacks pointer offset", OffsetOf<Callbacks>(nameof(Callbacks.Pointer)), layout.CallbacksPointerOffset);
        AssertEqual("host set-cursor offset", OffsetOf<Host>(nameof(Host.SetCursor)), layout.HostSetCursorOffset);
        AssertEqual("text configuration size", SizeOf<TextConfiguration>(), layout.TextConfigurationSize);
        AssertEqual("text state size", SizeOf<TextState>(), layout.TextStateSize);
        AssertEqual("host set-text-client offset", OffsetOf<Host>(nameof(Host.SetTextClient)), layout.HostSetTextClientOffset);
        AssertEqual("callbacks text-editing offset", OffsetOf<Callbacks>(nameof(Callbacks.TextEditing)), layout.CallbacksTextEditingOffset);
        AssertEqual("callbacks lifecycle offset", OffsetOf<Callbacks>(nameof(Callbacks.Lifecycle)), layout.CallbacksLifecycleOffset);
        AssertEqual("host initial-platform-brightness offset", OffsetOf<Host>(nameof(Host.InitialPlatformBrightness)), layout.HostInitialPlatformBrightnessOffset);
        AssertEqual("callbacks platform-brightness offset", OffsetOf<Callbacks>(nameof(Callbacks.PlatformBrightness)), layout.CallbacksPlatformBrightnessOffset);
        return layout;
    }

    private static uint SizeOf<T>() where T : struct => checked((uint)Marshal.SizeOf<T>());
    private static uint OffsetOf<T>(string field) where T : struct =>
        checked((uint)Marshal.OffsetOf<T>(field).ToInt64());

    private static void AssertEqual(string name, uint expected, uint actual)
    {
        if (expected != actual)
            throw new InvalidOperationException($"Native ABI {name} mismatch: managed={expected}, native={actual}.");
    }
}

internal sealed record NativeBuildProvenance(
    string SchemaVersion,
    string NativeHostSha256,
    string BootstrapSha256,
    string AngleRuntimeSha256);

internal sealed record NativeHostProvenance(
    string ApplicationDirectory,
    string NativeHostPath,
    long NativeHostLength,
    long NativeHostLastWriteUtcTicks,
    string? NativeHostSha256,
    string BootstrapPath,
    long BootstrapLength,
    long BootstrapLastWriteUtcTicks,
    string? BootstrapSha256,
    string AngleRuntimePath,
    long AngleRuntimeLength,
    long AngleRuntimeLastWriteUtcTicks,
    string? AngleRuntimeSha256,
    bool FullHashAudit,
    string SearchPolicy);
