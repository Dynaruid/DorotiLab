using System.Runtime.InteropServices;
using Microsoft.UI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.XamlTypeInfo;
using DRect = Doroti.Ui.Rect;

namespace Doroti.Host.WindowsAppSdk;

/// <summary>UI-thread WinUI environment shared by the product's native control islands.</summary>
internal sealed class WindowsWinUiControls : IDisposable
{
    private DispatcherQueueController? _ownedQueue;
    private IslandApplication? _application;
    private WindowsXamlManager? _manager;
    private Exception? _initializationError;
    private readonly List<Island> _islands = [];

    internal Island Create(nint parent, bool editor, string text)
    {
        try
        {
            if (_initializationError is not null)
                throw new InvalidOperationException("The WinUI environment could not be initialized.", _initializationError);
            if (_manager is null)
            {
                Trace("initialize-start");
                // Activate MRT through its managed projection before native XAML
                // asks for the resource factory in this self-contained Win32 host.
                var resourceManager = new Microsoft.Windows.ApplicationModel.Resources.ResourceManager(
                    Microsoft.Windows.ApplicationModel.Resources.ResourceLoader.GetDefaultResourceFilePath());
                // Acrylic may already own the Windows App SDK queue on this thread.
                if (DispatcherQueue.GetForCurrentThread() is null)
                    _ownedQueue = DispatcherQueueController.CreateOnCurrentThread();
                if (Application.Current is null) _application = new IslandApplication(resourceManager);
                Trace("application-created");
                _manager = WindowsXamlManager.InitializeForCurrentThread();
                Trace("manager-created");
                try { Application.Current!.Resources.MergedDictionaries.Add(new XamlControlsResources()); }
                catch (Exception error) { _initializationError = error; throw; }
            }
            var island = new Island(parent, editor, text, item => _islands.Remove(item));
            _islands.Add(island);
            return island;
        }
        catch (Exception error) { throw new InvalidOperationException($"WinUI island initialization failed: {error}", error); }
    }

    internal object Snapshot() => _islands.Select(island => island.Snapshot()).ToArray();
    internal Microsoft.UI.Composition.Visual GetVisual(nint hwnd) =>
        _islands.Single(island => island.Hwnd == hwnd).Visual;
    internal void SetInputShields(nint hwnd, DRect[] shields) =>
        _islands.Single(island => island.Hwnd == hwnd).SetInputShields(shields);
    internal void CloseIslands()
    {
        foreach (var island in _islands.ToArray()) island.Dispose();
    }

    private static void Trace(string stage)
    {
        if (!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("DOROTI_PLATFORM_VIEW_EVIDENCE")))
            Console.Error.WriteLine($"doroti.windows.winui={stage}");
    }

    public void Dispose()
    {
        // All DesktopWindowXamlSources must have retired before the environment.
        if (_islands.Count != 0) throw new InvalidOperationException("WinUI islands must retire before the XAML environment.");
        Trace("shutdown-islands=0");
        _manager?.Dispose();
        _manager = null;
        _ownedQueue?.ShutdownQueue();
        _ownedQueue = null;
        GC.KeepAlive(_application);
    }

    private sealed class IslandApplication : Application, IXamlMetadataProvider
    {
        private readonly XamlControlsXamlMetaDataProvider _metadata = new();
        internal IslandApplication(Microsoft.Windows.ApplicationModel.Resources.ResourceManager resources)
        {
            UnhandledException += (_, args) => Console.Error.WriteLine($"WinUI: {args.Exception}");
            ResourceManagerRequested += (_, args) =>
            {
                args.CustomResourceManager = resources;
            };
        }
        public IXamlType GetXamlType(Type type) => _metadata.GetXamlType(type);
        public IXamlType GetXamlType(string fullName) => _metadata.GetXamlType(fullName);
        public XmlnsDefinition[] GetXmlnsDefinitions() => _metadata.GetXmlnsDefinitions();
    }

    internal sealed class Island : IDisposable
    {
        private readonly DesktopWindowXamlSource _source;
        private readonly Control _control;
        private readonly Grid _root = new();
        private readonly Canvas _shields = new();
        private DRect[] _shieldBounds = [];
        private readonly nint _parent;
        private readonly Action<Island> _disposed;
        private int _clicks;
        private bool _isDisposed;
        internal nint Hwnd { get; }
        internal Action? Focused { get; set; }
        internal Action? TakeFocus { get; set; }
        internal Action<uint, nuint, nint>? ModifierKey { get; set; }
        internal bool HasFocus => _source.HasFocus;
        internal Microsoft.UI.Composition.Visual Visual => ElementCompositionPreview.GetElementVisual(_control);

        internal Island(nint parent, bool editor, string text, Action<Island> disposed)
        {
            _disposed = disposed;
            _parent = parent;
            _source = new DesktopWindowXamlSource();
            Trace("source-created");
            try
            {
                _source.Initialize(Win32Interop.GetWindowIdFromWindow(parent));
                _source.SiteBridge.Hide();
                Trace("source-initialized");
                Hwnd = Win32Interop.GetWindowFromWindowId(_source.SiteBridge.WindowId);
                _control = editor ? new TextBox { Text = text } : new Button { Content = text };
                if (_control is Button button) button.Click += (_, _) => _clicks++;
                Trace(editor ? "textbox-created" : "button-created");
                _control.HorizontalAlignment = HorizontalAlignment.Stretch;
                _control.VerticalAlignment = VerticalAlignment.Stretch;
                _control.PreviewKeyDown += OnModifierKey;
                _control.PreviewKeyUp += OnModifierKey;
                AutomationProperties.SetName(_control, editor ? "Native editor" : text);
                _root.Children.Add(_control);
                _root.Children.Add(_shields);
                // Native controls consume pointer input before HWND subclasses
                // on Windows 11. Protect foreground UI in the XAML input tree.
                _shields.PointerPressed += OnShieldPressed;
                _shields.PointerMoved += OnShieldMoved;
                _shields.PointerReleased += OnShieldReleased;
                _shields.PointerWheelChanged += OnWheel;
                _root.PointerWheelChanged += OnWheel; // Bubble an unhandled native wheel to the surrounding list.
                _source.Content = _root;
                _source.GotFocus += OnGotFocus;
                _source.TakeFocusRequested += OnTakeFocus;
            }
            catch { _source.Dispose(); throw; }
        }

        internal void SetInputShields(DRect[] bounds)
        {
            if (_shieldBounds.SequenceEqual(bounds)) return;
            _shields.Children.Clear();
            foreach (var rect in bounds)
            {
                var shield = new Border { Width = rect.width, Height = rect.height,
                    Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Colors.Transparent) };
                Canvas.SetLeft(shield, rect.left); Canvas.SetTop(shield, rect.top);
                _shields.Children.Add(shield);
            }
            _shieldBounds = bounds;
        }
        private void OnShieldPressed(object sender, PointerRoutedEventArgs args) => ForwardPointer(args, 0x201);
        private void OnShieldMoved(object sender, PointerRoutedEventArgs args) => ForwardPointer(args, 0x200);
        private void OnShieldReleased(object sender, PointerRoutedEventArgs args) => ForwardPointer(args, 0x202);
        private void OnWheel(object sender, PointerRoutedEventArgs args)
        {
            if (!args.Handled) ForwardPointer(args, args.GetCurrentPoint(_root).Properties.IsHorizontalMouseWheel ? 0x20eu : 0x20au);
        }
        private void ForwardPointer(PointerRoutedEventArgs args, uint message)
        {
            try
            {
                var point = args.GetCurrentPoint(_root);
                var properties = point.Properties;
                var scale = _root.XamlRoot.RasterizationScale;
                var position = new Native.Point { X = (int)Math.Round(point.Position.X * scale), Y = (int)Math.Round(point.Position.Y * scale) };
                Native.ClientToScreen(Hwnd, ref position);
                var wheel = message is 0x20a or 0x20e;
                if (!wheel) Native.ScreenToClient(_parent, ref position);
                uint keys = (properties.IsLeftButtonPressed ? 1u : 0u) | (properties.IsRightButtonPressed ? 2u : 0u) |
                    (properties.IsMiddleButtonPressed ? 0x10u : 0u) |
                    (args.KeyModifiers.HasFlag(Windows.System.VirtualKeyModifiers.Shift) ? 4u : 0u) |
                    (args.KeyModifiers.HasFlag(Windows.System.VirtualKeyModifiers.Control) ? 8u : 0u);
                if (wheel) keys |= (uint)(ushort)properties.MouseWheelDelta << 16;
                if (message is 0x201 or 0x202)
                    message = properties.PointerUpdateKind switch
                    {
                        Microsoft.UI.Input.PointerUpdateKind.RightButtonPressed => 0x204,
                        Microsoft.UI.Input.PointerUpdateKind.RightButtonReleased => 0x205,
                        Microsoft.UI.Input.PointerUpdateKind.MiddleButtonPressed => 0x207,
                        Microsoft.UI.Input.PointerUpdateKind.MiddleButtonReleased => 0x208,
                        _ => message,
                    };
                args.Handled = true;
                Native.SendMessageW(_parent, message, keys, (nint)((ushort)position.X | ((uint)(ushort)position.Y << 16)));
            }
            catch (Exception error) { System.Diagnostics.Trace.TraceError(error.ToString()); }
        }

        private void OnModifierKey(object sender, KeyRoutedEventArgs args)
        {
            if ((int)args.Key is not (16 or 17 or 18 or 160 or 161 or 162 or 163 or 164 or 165)) return;
            try
            {
                var status = args.KeyStatus;
                var flags = status.RepeatCount | (status.ScanCode << 16) |
                    (status.IsExtendedKey ? 1u << 24 : 0) | (status.WasKeyDown ? 1u << 30 : 0) |
                    (status.IsKeyReleased ? 1u << 31 : 0);
                ModifierKey?.Invoke(status.IsKeyReleased ? 0x101u : 0x100u, (nuint)args.Key, unchecked((nint)flags));
            }
            catch (Exception error) { System.Diagnostics.Trace.TraceError(error.ToString()); }
        }

        private void OnGotFocus(DesktopWindowXamlSource sender, DesktopWindowXamlSourceGotFocusEventArgs args)
        {
            try { Focused?.Invoke(); }
            catch (Exception error) { System.Diagnostics.Trace.TraceError(error.ToString()); }
        }
        private void OnTakeFocus(DesktopWindowXamlSource sender, DesktopWindowXamlSourceTakeFocusRequestedEventArgs args)
        {
            try { TakeFocus?.Invoke(); }
            catch (Exception error) { System.Diagnostics.Trace.TraceError(error.ToString()); }
        }
        internal bool Focus() => _source.NavigateFocus(new XamlSourceFocusNavigationRequest(
            XamlSourceFocusNavigationReason.First)).WasFocusMoved;
        internal void DisableInput() { if (!_isDisposed) _control.IsEnabled = false; }
        internal object Snapshot() => new
        {
            type = _control.GetType().FullName, hwnd = Hwnd.ToInt64(),
            loaded = _control.IsLoaded, width = _control.ActualWidth, height = _control.ActualHeight,
            templateChildren = Microsoft.UI.Xaml.Media.VisualTreeHelper.GetChildrenCount(_control),
            theme = _control.ActualTheme.ToString(), font = _control.FontFamily.Source,
            cornerRadius = _control.CornerRadius.TopLeft, hasFocus = HasFocus,
            text = (_control as TextBox)?.Text, clicks = _clicks,
        };
        public void Dispose()
        {
            if (_isDisposed) return;
            _isDisposed = true;
            Focused = TakeFocus = null;
            ModifierKey = null;
            _control.PreviewKeyDown -= OnModifierKey;
            _control.PreviewKeyUp -= OnModifierKey;
            _shields.PointerPressed -= OnShieldPressed;
            _shields.PointerMoved -= OnShieldMoved;
            _shields.PointerReleased -= OnShieldReleased;
            _shields.PointerWheelChanged -= OnWheel;
            _root.PointerWheelChanged -= OnWheel;
            _source.GotFocus -= OnGotFocus;
            _source.TakeFocusRequested -= OnTakeFocus;
            _source.Content = null;
            _source.Dispose();
            _disposed(this);
        }
    }
    private static class Native
    {
        [StructLayout(LayoutKind.Sequential)] internal struct Point { internal int X, Y; }
        [DllImport("user32.dll")] internal static extern bool ClientToScreen(nint hwnd, ref Point point);
        [DllImport("user32.dll")] internal static extern bool ScreenToClient(nint hwnd, ref Point point);
        [DllImport("user32.dll")] internal static extern nint SendMessageW(nint hwnd, uint message, nuint wparam, nint lparam);
    }
}
