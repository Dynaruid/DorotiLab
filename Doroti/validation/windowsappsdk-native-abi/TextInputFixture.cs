using System.Runtime.InteropServices;
using Doroti.Host.WindowsAppSdk;
using N = Doroti.Host.WindowsAppSdk.WindowsNativeV1;

// Exercises the shipped DLL's WM_CHAR path on a hidden HWND, without global input.
internal static unsafe class TextInputFixture
{
    private sealed record Case(string Name, string Initial, int Base, int Extent,
        string Characters, string Expected, int Caret, int Edits, bool Multiline = false, int Actions = 0);

    private static readonly Case[] Cases =
    [
        new("Ctrl+Backspace after word deletion", "one ", 4, 4, "\u007f", "one ", 4, 0),
        new("Repeated Ctrl+Backspace on empty text", "", 0, 0, "\u007f\u007f", "", 0, 0),
        new("DEL preserves selection", "one two", 4, 7, "\u007f", "one two", 7, 0),
        new("Backspace does not delete twice", "ab", 2, 2, "\b", "ab", 2, 0),
        new("Shortcut controls do not insert", "abc", 3, 3, "\u0001\u0003\u0016\u0018\u001a\t", "abc", 3, 0),
        new("Printable ASCII and Korean", "", 0, 0, "A 한글~", "A 한글~", 5, 5),
        new("Printable after DEL", "one ", 4, 4, "\u007f한", "one 한", 5, 1),
        new("Replace selection", "one two", 4, 7, "한", "one 한", 5, 1),
        new("Single line Enter action", "abc", 3, 3, "\r", "abc", 3, 0, Actions: 1),
        new("Multiline Enter", "abc", 3, 3, "\r", "abc\n", 4, 1, Multiline: true),
    ];

    private static N.Host _host;
    private static int _index, _edits, _actions, _base, _extent;
    private static string _text = "";
    private static Exception? _failure;

    internal static void Run()
    {
        if (!OperatingSystem.IsWindows()) throw new PlatformNotSupportedException();
        // No renderer/show request: both host topologies remain hidden.
        foreach (var features in new[] { 0UL, N.CompositionPresentationFeature })
        {
            _index = 0;
            _failure = null;
            var configuration = new N.Configuration
            {
                AbiVersion = 1, StructSize = Size<N.Configuration>(),
                InitialWidthPx = 320, InitialHeightPx = 200, RequiredFeatures = features,
            };
            var callbacks = new N.Callbacks
            {
                AbiVersion = 1, StructSize = Size<N.Callbacks>(),
                HostReady = (nint)(delegate* unmanaged<nint, N.Host*, void>)&Ready,
                Key = (nint)(delegate* unmanaged<nint, N.Key*, void>)&Key,
                TextEditing = (nint)(delegate* unmanaged<nint, N.TextState*, void>)&Edited,
                TextAction = (nint)(delegate* unmanaged<nint, uint, void>)&Action,
                Metrics = (nint)(delegate* unmanaged<nint, nint, void>)&IgnorePointer,
                FrameTerminal = (nint)(delegate* unmanaged<nint, nint, void>)&IgnorePointer,
                Render = (nint)(delegate* unmanaged<nint, nint, uint>)&Render,
                SemanticsAction = (nint)(delegate* unmanaged<nint, long, long, N.Utf8, void>)&Semantics,
                Lifecycle = (nint)(delegate* unmanaged<nint, ulong, uint, long, void>)&Lifecycle,
                PlatformBrightness = (nint)(delegate* unmanaged<nint, ulong, uint, void>)&Brightness,
                PlatformResourcesShutdown = (nint)(delegate* unmanaged<nint, void>)&Shutdown,
                CompositionResize = (nint)(delegate* unmanaged<nint, uint, uint, double, uint, uint, void>)&Resize,
            };
            var status = N.Run(in configuration, in callbacks);
            if (_failure != null) throw new InvalidOperationException("Native text input failed", _failure);
            if (status != N.Status.Ok || _index != Cases.Length)
                throw new InvalidOperationException($"Native text host failed: {status}, completed {_index}");
            Console.WriteLine($"PASS: {Cases.Length} WM_CHAR cases, {(features == 0 ? "child" : "top-level")} HWND");
        }
    }

    private static uint Size<T>() => checked((uint)Marshal.SizeOf<T>());

    private static void QueueCase()
    {
        var test = Cases[_index];
        _text = test.Initial;
        _base = test.Base;
        _extent = test.Extent;
        _edits = _actions = 0;
        var utf8 = Marshal.StringToCoTaskMemUTF8(test.Initial);
        try
        {
            var state = new N.TextState
            {
                AbiVersion = 1, StructSize = Size<N.TextState>(),
                Text = new N.Utf8 { AbiVersion = 1, StructSize = Size<N.Utf8>(), Data = utf8,
                    ByteLength = (ulong)System.Text.Encoding.UTF8.GetByteCount(test.Initial) },
                SelectionBase = test.Base, SelectionExtent = test.Extent,
                ComposingBase = -1, ComposingExtent = -1,
            };
            var configuration = new N.TextConfiguration
            {
                AbiVersion = 1, StructSize = Size<N.TextConfiguration>(),
                InputType = test.Multiline ? 1U : 0U, InputAction = test.Multiline ? 12U : 6U,
            };
            var status = ((delegate* unmanaged<nint, N.TextConfiguration*, N.TextState*, uint>)_host.SetTextClient)
                (_host.HostContext, &configuration, &state);
            if (status != 0) throw new InvalidOperationException($"SetTextClient: {status}");
        }
        finally { Marshal.FreeCoTaskMem(utf8); }
        // Posted messages follow the posted SetTextClient command on the same UI queue.
        foreach (var character in test.Characters) Post(0x0102, character, 1); // WM_CHAR
        Post(0x0101, 0x87, unchecked((nint)0xC0580001)); // WM_KEYUP/F24 marks the end of this case.
    }

    private static void Post(uint message, nuint wparam, nint lparam)
    {
        if (!PostMessageW(_host.ChildHwnd, message, wparam, lparam))
            throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
    }

    private static void Close() => ((delegate* unmanaged<nint, uint>)_host.RequestClose)(_host.HostContext);

    [UnmanagedCallersOnly] private static void Ready(nint context, N.Host* host)
    {
        _host = *host;
        try { QueueCase(); }
        catch (Exception ex) { _failure = ex; Close(); }
    }

    [UnmanagedCallersOnly] private static void Key(nint context, N.Key* key)
    {
        try
        {
            var test = Cases[_index];
            if (_text != test.Expected || _base != (test.Edits == 0 ? test.Base : test.Caret) ||
                _extent != test.Caret || _edits != test.Edits || _actions != test.Actions)
                throw new InvalidOperationException($"{test.Name}: text={System.Text.Json.JsonSerializer.Serialize(_text)}, " +
                    $"selection={_base}:{_extent}, edits={_edits}, actions={_actions}");
            Console.WriteLine($"PASS: {test.Name}");
            if (++_index == Cases.Length) Close();
            else QueueCase();
        }
        catch (Exception ex) { _failure = ex; Close(); }
    }

    [UnmanagedCallersOnly] private static void Edited(nint context, N.TextState* state)
    {
        _text = Marshal.PtrToStringUTF8(state->Text.Data, checked((int)state->Text.ByteLength)) ?? "";
        _base = state->SelectionBase;
        _extent = state->SelectionExtent;
        _edits++;
    }
    [UnmanagedCallersOnly] private static void Action(nint context, uint action) => _actions++;
    [UnmanagedCallersOnly] private static void IgnorePointer(nint context, nint value) { }
    [UnmanagedCallersOnly] private static uint Render(nint context, nint request) => 2; // Superseded; no present.
    [UnmanagedCallersOnly] private static void Semantics(nint context, long node, long action, N.Utf8 arguments) { }
    [UnmanagedCallersOnly] private static void Lifecycle(nint context, ulong view, uint state, long timestamp) { }
    [UnmanagedCallersOnly] private static void Brightness(nint context, ulong view, uint brightness) { }
    [UnmanagedCallersOnly] private static void Shutdown(nint context) { }
    [UnmanagedCallersOnly] private static void Resize(nint context, uint width, uint height, double scale, uint edge, uint phase) { }

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool PostMessageW(nint window, uint message, nuint wparam, nint lparam);
}
