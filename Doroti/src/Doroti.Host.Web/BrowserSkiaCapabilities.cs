using System.Text.Json;
using System.Text.Json.Serialization;
using Doroti.Skia.Rendering;
using Doroti.Skia.RuntimeEffects;
using Doroti.Ui;
using SkiaSharp;
using UiImage = Doroti.Ui.Image;

namespace Doroti.Host.Web;

[System.Runtime.Versioning.SupportedOSPlatform("browser")]
internal sealed class BrowserSkiaCapabilities :
    IBrowserGraphicsCapabilities
{
    private readonly HostBridge _bridge;
    private readonly BrowserHostAdapter _host;
    private readonly SkiaSceneRenderer _renderer;
    private readonly object _paintGate = new();
    private readonly Dictionary<long, SkiaPaintCompletion> _pendingPaints = [];

    internal BrowserSkiaCapabilities(ulong viewId, BrowserHostAdapter host,
        Color? backgroundColor, Color? darkBackgroundColor,
        string backendIdentity,
        SkiaFallbackFontCollection? fallbackFonts = null)
    {
        _host = host;
        _bridge = new(host);
        _renderer = new(viewId, _bridge, backgroundColor, darkBackgroundColor,
            backendIdentity, DorotiSkiaRuntimeEffects.WebGpuBackend,
            "doroti-owned-canvas-webgl2-skia-gpu", fallbackFonts: fallbackFonts);
    }

    public event Action<SemanticsActionEvent>? Action
    {
        add => _renderer.Action += value;
        remove => _renderer.Action -= value;
    }

    public bool CoalesceGeometryDuringActiveMetrics => true;

    public BrowserFrameDiagnostics Diagnostics
    {
        get
        {
            var value = _renderer.Diagnostics;
            return new(value.Submitted, value.Presented, value.Replayed, value.Failed,
                value.ContextGeneration, value.SurfaceGeneration, value.LastInputSequence,
                value.PendingScene, value.Backend);
        }
    }

    public void AttachSurface(Action invalidate)
    {
        _bridge.Invalidate = invalidate;
        _renderer.AttachSurface(invalidate);
    }

    public void AttachFrameworkTrace(DorotiFrameTrace trace) =>
        _renderer.AttachFrameworkTrace(trace);

    public void Submit(ulong viewId, DorotiSceneSubmission submission, DartUiInvocation invocation)
        => _renderer.Submit(viewId, submission, invocation);

    public string Paint(
        SKSurface surface,
        int pixelWidth,
        int pixelHeight,
        DorotiResizeEpoch target,
        long requestId)
    {
        var started = DorotiFrameClock.Now;
        _host.RecordRaster("managed-raster-start", pixelWidth, pixelHeight);
        try
        {
            var result = _renderer.Paint(surface, pixelWidth, pixelHeight, target, requestId);
            if (result.ShouldPresent && result.Completion is { } completion)
            {
                lock (_paintGate)
                    _pendingPaints[requestId] = completion;
            }
            return result.Disposition switch
            {
                SkiaPaintDisposition.exact => "exact-rendered",
                SkiaPaintDisposition.replay => "replay-rendered",
                SkiaPaintDisposition.superseded => "superseded",
                _ => "empty",
            };
        }
        finally
        {
            _host.RecordRaster("managed-raster-end", pixelWidth, pixelHeight,
                DorotiFrameClock.Now - started);
        }
    }

    public void CompletePaint(long requestId, string terminal, string reason)
    {
        SkiaPaintCompletion completion;
        lock (_paintGate)
        {
            if (!_pendingPaints.Remove(requestId, out completion)) return;
        }
        switch (terminal)
        {
            case "submitted": _renderer.CompletePaint(completion, DorotiFrameTerminal.submitted); break;
            case "presented": _renderer.CompletePaint(completion, DorotiFrameTerminal.presented); break;
            case "superseded": _renderer.SupersedePaint(completion, reason); break;
            case "dropped": _renderer.DropPaint(completion, reason); break;
            case "failed": _renderer.FailPaint(completion, reason); break;
            default: throw new InvalidDataException($"Unknown browser frame terminal '{terminal}'.");
        }
    }

    public void InvalidateGpuContext(long requestId, string reason)
    {
        SkiaPaintCompletion completion;
        lock (_paintGate)
        {
            if (_pendingPaints.Remove(requestId, out completion))
                _renderer.FailPaint(completion, reason);
        }
        _renderer.InvalidateGpuContextResources();
    }

    public void InvalidateWindowSurfaceResources() =>
        _renderer.InvalidateWindowSurfaceResources();

    public Paragraph Layout(ParagraphRequest request, DartUiInvocation invocation) =>
        _renderer.Layout(request, invocation);

    public ValueTask<UiImage> DecodeAsync(ReadOnlyMemory<byte> bytes,
        DartUiInvocation invocation, CancellationToken cancellationToken = default) =>
        _renderer.DecodeAsync(bytes, invocation, cancellationToken);

    public void SetEnabled(bool enabled, DartUiInvocation invocation) =>
        _renderer.SetEnabled(enabled, invocation);

    public void Update(SemanticsUpdate update, DartUiInvocation invocation)
    {
        var started = DorotiFrameClock.Now;
        _host.RecordRaster("managed-semantics-start", 0, 0);
        try
        {
            _renderer.Update(update, invocation);
        }
        finally
        {
            _host.RecordRaster("managed-semantics-end", 0, 0, DorotiFrameClock.Now - started);
        }
    }

    public void Dispose()
    {
        lock (_paintGate)
        {
            foreach (var pending in _pendingPaints.Values)
                _renderer.SupersedePaint(pending, "browser graphics capability disposed");
            _pendingPaints.Clear();
        }
        _renderer.Dispose();
        _bridge.Dispose();
    }

    private sealed class HostBridge : ISkiaSceneRendererHost, IDisposable
    {
        private static readonly JsonSerializerOptions SemanticsJsonOptions = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };

        private readonly BrowserHostAdapter _host;
        private readonly Dictionary<int, SemanticsNodeUpdate> _semantics = [];
        private readonly Dictionary<int, SemanticsNodeUpdate> _lastSentSemantics = [];

        internal HostBridge(BrowserHostAdapter host)
        {
            _host = host;
            _host.SemanticsAction += HandleSemanticsAction;
        }

        internal Action? Invalidate { get; set; }
        public long InputSequence => _host.InputSequence;
        public long SurfaceGeneration => _host.Snapshot.SurfaceGeneration;
        public DorotiViewEpoch ViewEpoch => _host.ViewEpoch;
        public DorotiResizeEpoch ResizeTarget => _host.Snapshot.ResizeEpoch;
        public PlatformConfiguration Configuration => _host.Configuration;
        public event Action<int, SemanticsAction, object?>? SemanticsAction;
        public event Action<long, TimeSpan>? InputReceived
        {
            add => _host.InputReceived += value;
            remove => _host.InputReceived -= value;
        }
        public event Action<PlatformConfiguration>? ConfigurationChanged
        {
            add => _host.ConfigurationChanged += value;
            remove => _host.ConfigurationChanged -= value;
        }

        public void UpdateSemantics(SemanticsUpdate update)
        {
            foreach (var node in update.nodes) _semantics[node.id] = node;
            PruneUnreachable(_semantics);
            var orderedNodes = _semantics.Values
                .OrderBy(node => node.indexInParent ?? int.MaxValue).ThenBy(node => node.id)
                .ToArray();
            var nodes = orderedNodes.Select(node =>
            {
                // Projection creates a new record for geometry changes while
                // retaining the content objects of unchanged nodes. Ignore the
                // projected rectangle when deciding whether the DOM needs a
                // fresh ARIA/action payload.
                var contentUnchanged = _lastSentSemantics.TryGetValue(node.id, out var previous) &&
                    previous with { rect = node.rect } == node;
                return new
                {
                    node.id,
                    contentUnchanged,
                    label = contentUnchanged ? null : node.label,
                    value = contentUnchanged ? null : node.value,
                    role = contentUnchanged ? null : node.role.ToString(),
                    actions = contentUnchanged ? (long?)null : (long)node.actions,
                    children = node.children,
                    identifier = contentUnchanged ? null : node.identifier,
                    hint = contentUnchanged ? null : node.hint,
                    tooltip = contentUnchanged ? null : node.tooltip,
                    increasedValue = contentUnchanged ? null : node.increasedValue,
                    decreasedValue = contentUnchanged ? null : node.decreasedValue,
                    headingLevel = contentUnchanged ? null : node.headingLevel,
                    linkUrl = contentUnchanged ? null : node.linkUrl,
                    validationResult = contentUnchanged ? null : node.validationResult.ToString(),
                    hitTestBehavior = contentUnchanged ? null : node.hitTestBehavior.ToString(),
                    inputType = contentUnchanged ? null : node.inputType.ToString(),
                    minValue = contentUnchanged ? null : node.minValue,
                    maxValue = contentUnchanged ? null : node.maxValue,
                    maxValueLength = contentUnchanged ? null : node.maxValueLength,
                    currentValueLength = contentUnchanged ? null : node.currentValueLength,
                    scrollPosition = contentUnchanged ? null : node.scrollPosition,
                    scrollExtentMin = contentUnchanged ? null : node.scrollExtentMin,
                    scrollExtentMax = contentUnchanged ? null : node.scrollExtentMax,
                    scrollChildCount = contentUnchanged ? null : node.scrollChildCount,
                    scrollIndex = contentUnchanged ? null : node.scrollIndex,
                    controlsNodes = contentUnchanged ? null : node.controlsNodes,
                    locale = contentUnchanged ? null : node.locale?.ToString(),
                    flags = contentUnchanged || node.flags is null ? null : new
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
                        focusable = node.flags.isFocused != Tristate.none,
                        obscured = node.flags.isObscured,
                        mutuallyExclusive = node.flags.isInMutuallyExclusiveGroup,
                        keyboardKey = node.flags.isKeyboardKey,
                    },
                    textSelectionBase = contentUnchanged ? (long?)null : node.textSelectionBase,
                    textSelectionExtent = contentUnchanged ? (long?)null : node.textSelectionExtent,
                    rect = new[] { node.rect.left, node.rect.top, node.rect.right, node.rect.bottom },
                };
            }).ToArray();
            _host.UpdateSemantics(JsonSerializer.Serialize(
                new { generation = update.generation, nodes }, SemanticsJsonOptions));
            _lastSentSemantics.Clear();
            foreach (var node in orderedNodes) _lastSentSemantics[node.id] = node;
        }

        public void ClearSemantics()
        {
            _semantics.Clear();
            _lastSentSemantics.Clear();
            _host.UpdateSemantics("{\"generation\":0,\"nodes\":[]}");
        }

        public void RequestInvalidate() => Invalidate?.Invoke();

        public void Dispose()
        {
            _host.SemanticsAction -= HandleSemanticsAction;
            Invalidate = null;
            _semantics.Clear();
            _lastSentSemantics.Clear();
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
