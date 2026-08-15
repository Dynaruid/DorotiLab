using System.Runtime.InteropServices;

namespace Doroti.Vendor.Avalonia.Native;

internal static class NativeInterop
{
    private const string Library = "libAvalonia";

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void EventCallback(
        nint context, int kind, int phase, ulong windowId,
        double a, double b, double c, double d,
        ulong u0, ulong u1, nint text);

    [DllImport(Library, EntryPoint = "doroti_avn_app_init")]
    internal static extern void AppInit();
    [DllImport(Library, EntryPoint = "doroti_avn_app_wake")]
    internal static extern void AppWake();
    [DllImport(Library, EntryPoint = "doroti_avn_app_pump")]
    internal static extern int AppPump(int wait);
    [DllImport(Library, EntryPoint = "doroti_avn_window_create")]
    internal static extern nint WindowCreate(
        [MarshalAs(UnmanagedType.LPUTF8Str)] string title, double width, double height,
        EventCallback callback, nint context);
    [DllImport(Library, EntryPoint = "doroti_avn_window_show")]
    internal static extern void WindowShow(nint host);
    [DllImport(Library, EntryPoint = "doroti_avn_window_resize")]
    internal static extern void WindowResize(nint host, double width, double height);
    [DllImport(Library, EntryPoint = "doroti_avn_window_minimize")]
    internal static extern void WindowMinimize(nint host, int minimized);
    [DllImport(Library, EntryPoint = "doroti_avn_window_focus")]
    internal static extern void WindowFocus(nint host, int focused);
    [DllImport(Library, EntryPoint = "doroti_avn_window_close")]
    internal static extern void WindowClose(nint host);
    [DllImport(Library, EntryPoint = "doroti_avn_window_destroy")]
    internal static extern void WindowDestroy(nint host);
    [DllImport(Library, EntryPoint = "doroti_avn_window_move_to_screen")]
    internal static extern void WindowMoveToScreen(nint host, ulong screenId);
    [DllImport(Library, EntryPoint = "doroti_avn_window_metrics")]
    internal static extern void WindowMetrics(nint host, out double width, out double height,
        out double pixelWidth, out double pixelHeight, out double scale);
    [DllImport(Library, EntryPoint = "doroti_avn_screen_primary")]
    internal static extern int ScreenPrimary(out ulong screenId, out double x, out double y,
        out double width, out double height, out double scale);
    [DllImport(Library, EntryPoint = "doroti_avn_window_nswindow")]
    internal static extern nint WindowHandle(nint host);
    [DllImport(Library, EntryPoint = "doroti_avn_cursor_set")]
    internal static extern void CursorSet(int kind);
    [DllImport(Library, EntryPoint = "doroti_avn_clipboard_get")]
    internal static extern nint ClipboardGet();
    [DllImport(Library, EntryPoint = "doroti_avn_clipboard_set")]
    internal static extern int ClipboardSet([MarshalAs(UnmanagedType.LPUTF8Str)] string text);
    [DllImport(Library, EntryPoint = "doroti_avn_string_free")]
    internal static extern void StringFree(nint value);
    [DllImport(Library, EntryPoint = "doroti_avn_accessibility_set")]
    internal static extern void AccessibilitySet(nint host, int nodeId,
        [MarshalAs(UnmanagedType.LPUTF8Str)] string label, int canPress);
    [DllImport(Library, EntryPoint = "doroti_avn_text_caret")]
    internal static extern void TextCaret(nint host, double x, double y, double width, double height);
    [DllImport(Library, EntryPoint = "doroti_avn_test_pointer")]
    internal static extern void TestPointer(nint host, int phase, double x, double y, double dx, double dy);
    [DllImport(Library, EntryPoint = "doroti_avn_test_key")]
    internal static extern void TestKey(nint host, int phase, uint key);
    [DllImport(Library, EntryPoint = "doroti_avn_test_text")]
    internal static extern void TestText(nint host, int phase, [MarshalAs(UnmanagedType.LPUTF8Str)] string text);
    [DllImport(Library, EntryPoint = "doroti_avn_gl_create")]
    internal static extern nint GlCreate(nint host);
    [DllImport(Library, EntryPoint = "doroti_avn_gl_make_current")]
    internal static extern nint GlMakeCurrent(nint context);
    [DllImport(Library, EntryPoint = "doroti_avn_gl_restore")]
    internal static extern void GlRestore(nint context);
    [DllImport(Library, EntryPoint = "doroti_avn_gl_present")]
    internal static extern void GlPresent(nint context);
    [DllImport(Library, EntryPoint = "doroti_avn_gl_renderer")]
    internal static extern nint GlRenderer(nint context);
    [DllImport(Library, EntryPoint = "doroti_avn_gl_version")]
    internal static extern nint GlVersion(nint context);
    [DllImport(Library, EntryPoint = "doroti_avn_gl_destroy")]
    internal static extern void GlDestroy(nint context);
}
