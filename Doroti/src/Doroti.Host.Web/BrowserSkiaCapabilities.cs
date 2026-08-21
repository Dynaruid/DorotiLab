using System.Text.Json;
using Doroti.Skia.Rendering;
using Doroti.Skia.RuntimeEffects;
using Doroti.Ui;
using SkiaSharp;
using UiImage = Doroti.Ui.Image;

namespace Doroti.Host.Web;

[System.Runtime.Versioning.SupportedOSPlatform("browser")]
internal sealed class BrowserSkiaCapabilities :
    ISceneHostCapability, IParagraphHostCapability, IImageHostCapability,
    ISemanticsHostCapability, IDisposable
{
    private readonly HostBridge _bridge;
    private readonly BrowserHostAdapter _host;
    private readonly SkiaSceneRenderer _renderer;

    internal BrowserSkiaCapabilities(ulong viewId, BrowserHostAdapter host,
        Color? backgroundColor, Color? darkBackgroundColor)
    {
        _host = host;
        _bridge = new(host);
        _renderer = new(viewId, _bridge, backgroundColor, darkBackgroundColor,
            "browser-wasm/document-canvas-webgl2", DorotiSkiaRuntimeEffects.WebGpuBackend,
            "skiasharp-skglview-webgl2-gpu");
    }

    public event Action<SemanticsActionEvent>? Action
    {
        add => _renderer.Action += value;
        remove => _renderer.Action -= value;
    }

    internal BrowserFrameDiagnostics Diagnostics
    {
        get
        {
            var value = _renderer.Diagnostics;
            return new(value.Submitted, value.Presented, value.Replayed, value.Failed,
                value.ContextGeneration, value.SurfaceGeneration, value.PendingScene, value.Backend);
        }
    }

    internal void AttachSurface(Action invalidate)
    {
        _bridge.Invalidate = invalidate;
        _renderer.AttachSurface(invalidate);
    }

    public void Submit(ulong viewId, Scene scene, DartUiInvocation invocation) =>
        _renderer.Submit(viewId, scene, invocation);

    internal void Paint(SKSurface surface, int pixelWidth, int pixelHeight)
    {
        var started = DorotiFrameClock.Now;
        _host.RecordRaster("raster-start", pixelWidth, pixelHeight);
        try
        {
            if (_renderer.Paint(surface, pixelWidth, pixelHeight) is { } completion)
                _renderer.CompletePaint(completion);
        }
        finally
        {
            _host.RecordRaster("raster-end", pixelWidth, pixelHeight,
                DorotiFrameClock.Now - started);
        }
    }

    public Paragraph Layout(ParagraphRequest request, DartUiInvocation invocation) =>
        _renderer.Layout(request, invocation);

    public ValueTask<UiImage> DecodeAsync(ReadOnlyMemory<byte> bytes,
        DartUiInvocation invocation, CancellationToken cancellationToken = default) =>
        _renderer.DecodeAsync(bytes, invocation, cancellationToken);

    public void SetEnabled(bool enabled, DartUiInvocation invocation) =>
        _renderer.SetEnabled(enabled, invocation);

    public void Update(SemanticsUpdate update, DartUiInvocation invocation) =>
        _renderer.Update(update, invocation);

    public void Dispose()
    {
        _renderer.Dispose();
        _bridge.Dispose();
    }

    private sealed class HostBridge : ISkiaSceneRendererHost, IDisposable
    {
        private readonly BrowserHostAdapter _host;
        private readonly Dictionary<int, SemanticsNodeUpdate> _semantics = [];

        internal HostBridge(BrowserHostAdapter host)
        {
            _host = host;
            _host.SemanticsAction += HandleSemanticsAction;
        }

        internal Action? Invalidate { get; set; }
        public long InputSequence => 0;
        public long SurfaceGeneration => _host.Snapshot.SurfaceGeneration;
        public PlatformConfiguration Configuration => _host.Configuration;
        public event Action<int, SemanticsAction, object?>? SemanticsAction;
        public event Action<long, TimeSpan>? InputReceived { add { } remove { } }
        public event Action<PlatformConfiguration>? ConfigurationChanged
        {
            add => _host.ConfigurationChanged += value;
            remove => _host.ConfigurationChanged -= value;
        }

        public void UpdateSemantics(SemanticsUpdate update)
        {
            foreach (var node in update.nodes) _semantics[node.id] = node;
            PruneUnreachable(_semantics);
            var nodes = _semantics.Values
                .OrderBy(node => node.indexInParent ?? int.MaxValue).ThenBy(node => node.id)
                .Select(node => new
                {
                    node.id, node.label, node.value, role = node.role.ToString(),
                    actions = (long)node.actions, children = node.children,
                    flags = node.flags is null ? null : new
                    {
                        @checked = node.flags.isChecked.ToString(),
                        selected = node.flags.isSelected.toBoolOrNull(),
                        enabled = node.flags.isEnabled.toBoolOrNull(),
                        toggled = node.flags.isToggled.toBoolOrNull(),
                        expanded = node.flags.isExpanded.toBoolOrNull(),
                        required = node.flags.isRequired.toBoolOrNull(),
                        focused = node.flags.isFocused.toBoolOrNull(),
                        button = node.flags.isButton, textField = node.flags.isTextField,
                        header = node.flags.isHeader, hidden = node.flags.isHidden,
                        image = node.flags.isImage, liveRegion = node.flags.isLiveRegion,
                        multiline = node.flags.isMultiline, readOnly = node.flags.isReadOnly,
                        link = node.flags.isLink, slider = node.flags.isSlider,
                    },
                    node.textSelectionBase, node.textSelectionExtent,
                    rect = new[] { node.rect.left, node.rect.top, node.rect.right, node.rect.bottom },
                });
            _host.UpdateSemantics(JsonSerializer.Serialize(new { generation = update.generation, nodes }));
        }

        public void ClearSemantics()
        {
            _semantics.Clear();
            _host.UpdateSemantics("{\"generation\":0,\"nodes\":[]}");
        }

        public void RequestInvalidate() => Invalidate?.Invoke();

        public void Dispose()
        {
            _host.SemanticsAction -= HandleSemanticsAction;
            Invalidate = null;
            _semantics.Clear();
        }

        private void HandleSemanticsAction(long nodeId, long action, string argumentsJson)
        {
            if (nodeId is < int.MinValue or > int.MaxValue) return;
            SemanticsAction?.Invoke(checked((int)nodeId), (SemanticsAction)action,
                ParseArguments(argumentsJson));
        }

        private static void PruneUnreachable(Dictionary<int, SemanticsNodeUpdate> nodes)
        {
            if (!nodes.ContainsKey(0)) return;
            var reachable = new HashSet<int>();
            var pending = new Stack<int>();
            pending.Push(0);
            while (pending.TryPop(out var id))
            {
                if (!reachable.Add(id) || !nodes.TryGetValue(id, out var node)) continue;
                foreach (var child in node.children) pending.Push(child);
            }
            foreach (var stale in nodes.Keys.Where(id => !reachable.Contains(id)).ToArray())
                nodes.Remove(stale);
        }

        private static object? ParseArguments(string json)
        {
            if (string.IsNullOrWhiteSpace(json) || json == "null") return null;
            using var document = JsonDocument.Parse(json);
            return ConvertElement(document.RootElement);
        }

        private static object? ConvertElement(JsonElement element) => element.ValueKind switch
        {
            JsonValueKind.String => element.GetString(),
            JsonValueKind.Number when element.TryGetInt64(out var integer) => integer,
            JsonValueKind.Number => element.GetDouble(),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Array => element.EnumerateArray().Select(ConvertElement).ToArray(),
            JsonValueKind.Object => element.EnumerateObject().ToDictionary(
                property => property.Name, property => ConvertElement(property.Value), StringComparer.Ordinal),
            _ => null,
        };
    }
}
