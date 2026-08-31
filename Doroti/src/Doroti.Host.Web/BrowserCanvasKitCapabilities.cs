using System.Text.Json;
using System.Text.Json.Serialization;
using Doroti.Graphics.DisplayList;
using Doroti.Ui;
using SkiaSharp;
using UiImage = Doroti.Ui.Image;

namespace Doroti.Host.Web;

[System.Runtime.Versioning.SupportedOSPlatform("browser")]
internal sealed class BrowserCanvasKitCapabilities :
    IBrowserGraphicsCapabilities,
    ICanvasKitSceneCallback
{
    private const long JavaScriptMaximumSafeInteger = 9_007_199_254_740_991;
    private const long SurfaceGenerationStride = 0x1_0000_0000;
    private static long _nextSceneSequence;
    private readonly object _gate = new();
    private readonly ulong _viewId;
    private readonly BrowserHostAdapter _host;
    private readonly CanvasKitResourceRegistry _resources;
    private readonly string _backendIdentity;
    private readonly uint _lightBackgroundColor;
    private readonly uint _darkBackgroundColor;
    private readonly Dictionary<long, PendingScene> _pending = [];
    private readonly DorotiFrameTerminalLedger _terminalLedger = new();
    private readonly Dictionary<int, SemanticsNodeUpdate> _semantics = [];
    private readonly Dictionary<int, SemanticsNodeUpdate> _lastSentSemantics = [];
    private DorotiFrameTrace _frameTrace = new();
    private Action<SemanticsActionEvent>? _action;
    private bool _semanticsEnabled;
    private bool _disposed;
    private long _submitted;
    private long _failed;
    private long _contextGeneration = 1;
    private long _surfaceGeneration;
    private long _lastInputSequence;

    internal BrowserCanvasKitCapabilities(
        ulong viewId,
        BrowserHostAdapter host,
        Color? backgroundColor,
        Color? darkBackgroundColor,
        string backendIdentity,
        CanvasKitResourceRegistry resources)
    {
        _viewId = viewId;
        _host = host ?? throw new ArgumentNullException(nameof(host));
        _resources = resources ?? throw new ArgumentNullException(nameof(resources));
        _backendIdentity = backendIdentity;
        _lightBackgroundColor = (backgroundColor ?? new Color(0xfffffbfeL)).value;
        _darkBackgroundColor = (darkBackgroundColor ?? backgroundColor ?? new Color(0xff141218L)).value;
        _host.SemanticsAction += HandleSemanticsAction;
    }

    public event Action<SemanticsActionEvent>? Action
    {
        add => _action += value;
        remove => _action -= value;
    }

    public bool CoalesceGeometryDuringActiveMetrics => true;

    public BrowserFrameDiagnostics Diagnostics
    {
        get
        {
            lock (_gate)
                return new(
                    _submitted,
                    0,
                    0,
                    _failed,
                    _contextGeneration,
                    _surfaceGeneration,
                    _lastInputSequence,
                    _pending.Count != 0,
                    _backendIdentity);
        }
    }

    public void AttachSurface(Action invalidate)
    {
        // CanvasKit owns the transferred visible OffscreenCanvas and drains its
        // MessagePort mailbox immediately.  A managed surface invalidation
        // callback would reintroduce the combined Skia raster path.
        _ = invalidate;
    }

    public void AttachFrameworkTrace(DorotiFrameTrace trace) =>
        _frameTrace = trace ?? throw new ArgumentNullException(nameof(trace));

    public void Submit(ulong viewId, DorotiSceneSubmission submission, DartUiInvocation invocation)
    {
        lock (_gate) ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(submission);
        ArgumentNullException.ThrowIfNull(submission.Scene);
        if (viewId != _viewId || submission.Scene.viewId != _viewId)
            throw new DorotiCapabilityException(
                DorotiCapabilityIds.GraphicsScene,
                viewId,
                invocation,
                "scene/view ownership mismatch",
                _backendIdentity);

        var sequence = Interlocked.Increment(ref _nextSceneSequence);
        _terminalLedger.Register(sequence);
        var inputSequence = _host.InputSequence;
        var submittedAt = DorotiFrameClock.Now;
        if (submission.BuildToken is not { } buildToken)
        {
            _terminalLedger.TryComplete(sequence, DorotiFrameTerminal.dropped);
            submission.FrameTransaction?.TryComplete(
                DorotiFrameTerminal.dropped,
                "scene submitted outside a framework frame");
            lock (_gate) _failed++;
            return;
        }

        var descriptor = DorotiFrameDescriptor.FromBuildToken(buildToken, sequence);
        try
        {
            submission.FrameTransaction?.SceneBuilt(buildToken, descriptor);
        }
        catch (Exception exception)
        {
            _terminalLedger.TryComplete(sequence, DorotiFrameTerminal.failed);
            submission.FrameTransaction?.TryComplete(
                DorotiFrameTerminal.failed,
                $"scene transaction admission failed: {exception.Message}");
            lock (_gate) _failed++;
            throw;
        }

        DisplayResourceReference[] sceneResources = [];
        var resourcesRetained = false;
        long admittedContextGeneration = 0;
        long admittedSurfaceGeneration = 0;
        try
        {
            var snapshot = _host.Snapshot;
            var contextGeneration = RequireContextGeneration(snapshot.Gpu.ContextGeneration);
            var surfaceGeneration = ComposeSurfaceGeneration(
                contextGeneration, descriptor.ResizeTargetGeneration);
            admittedContextGeneration = contextGeneration;
            admittedSurfaceGeneration = surfaceGeneration;
            if (snapshot.ResizeEpoch.Generation == descriptor.ResizeTargetGeneration &&
                snapshot.SurfaceGeneration != surfaceGeneration)
                throw new InvalidDataException(
                    $"CanvasKit host surface identity {snapshot.SurfaceGeneration} does not match " +
                    $"context/resize identity {surfaceGeneration}.");
            var sceneMetadata = new DisplayListSceneMetadata(
                _viewId,
                checked((ulong)sequence),
                checked((ulong)buildToken.FrameworkFrameNumber),
                checked((ulong)descriptor.ResizeTargetGeneration),
                checked((ulong)surfaceGeneration),
                checked((ulong)contextGeneration),
                checked((float)descriptor.LogicalWidth),
                checked((float)descriptor.LogicalHeight),
                checked((uint)descriptor.PhysicalWidth),
                checked((uint)descriptor.PhysicalHeight),
                checked((float)descriptor.DevicePixelRatio));
            var background = _host.Configuration.platformBrightness == Brightness.dark
                ? _darkBackgroundColor
                : _lightBackgroundColor;
            var document = BrowserDisplayListMapper.Create(
                submission.Scene, sceneMetadata, background, _resources);
            sceneResources = document.Resources.Select(value => value.Reference).ToArray();
            _resources.RetainSceneResources(sceneResources);
            resourcesRetained = true;
            var wireBytes = DisplayListEncoder.Encode(document);
            lock (_gate)
            {
                ObjectDisposedException.ThrowIf(_disposed, this);
                _pending.Add(sequence, new(
                    sequence,
                    inputSequence,
                    submittedAt,
                    descriptor,
                    contextGeneration,
                    surfaceGeneration,
                    sceneResources,
                    submission.FrameTransaction));
                try
                {
                    // Keep registration and pending ownership under the same gate so Dispose
                    // cannot forget a callback before it has been admitted.
                    BrowserCanvasKitInterop.Submit(sequence, wireBytes, this);
                }
                catch
                {
                    _pending.Remove(sequence);
                    throw;
                }
                resourcesRetained = false; // PendingScene now owns the pins.
                _contextGeneration = Math.Max(_contextGeneration, contextGeneration);
                _lastInputSequence = inputSequence;
            }
        }
        catch (Exception exception)
        {
            if (resourcesRetained) _resources.ReleaseSceneResources(sceneResources);
            _terminalLedger.TryComplete(sequence, DorotiFrameTerminal.failed);
            submission.FrameTransaction?.TryComplete(
                DorotiFrameTerminal.failed,
                $"DisplayList admission failed: {exception.Message}");
            lock (_gate) _failed++;
            throw;
        }
        _frameTrace.Record(
            DorotiFramePhase.sceneSubmitted,
            _viewId,
            submittedAt,
            inputSequence,
            sequence,
            admittedSurfaceGeneration,
            invocation.ElementId,
            resizeTargetGeneration: descriptor.ResizeTargetGeneration,
            metricsGeneration: descriptor.MetricsGeneration,
            frameworkFrameNumber: descriptor.FrameworkFrameNumber,
            contextGeneration: admittedContextGeneration);
    }

    public void CompleteScene(long sceneSequence, string terminal, string reason, string receiptJson)
    {
        PendingScene? pending;
        lock (_gate)
        {
            if (!_pending.Remove(sceneSequence, out pending)) return;
        }
        try
        {
            var receipt = ParseReceipt(receiptJson);
            var managedTerminal = terminal switch
            {
                "submitted" => DorotiFrameTerminal.submitted,
                "superseded" => DorotiFrameTerminal.superseded,
                "failed" => DorotiFrameTerminal.failed,
                _ => DorotiFrameTerminal.failed,
            };
            reason = string.IsNullOrWhiteSpace(reason)
                ? managedTerminal == DorotiFrameTerminal.submitted
                    ? "CanvasKit surface submitted"
                    : $"CanvasKit scene {terminal}"
                : reason;
            var traceSurfaceGeneration = receipt.SurfaceGeneration > 0
                ? receipt.SurfaceGeneration
                : pending.SurfaceGeneration;
            var traceContextGeneration = receipt.ContextGeneration > 0
                ? receipt.ContextGeneration
                : pending.ContextGeneration;
            if (managedTerminal == DorotiFrameTerminal.submitted &&
                (receipt.ContextGeneration != pending.ContextGeneration ||
                 receipt.SurfaceGeneration != pending.SurfaceGeneration))
            {
                managedTerminal = DorotiFrameTerminal.failed;
                reason = "CanvasKit submitted receipt does not match its immutable context/surface generation.";
            }

            if (managedTerminal == DorotiFrameTerminal.submitted)
            {
                try
                {
                    pending.FrameTransaction?.BackingStoreReady(
                        $"{_backendIdentity}/canvaskit-surface/{pending.SurfaceGeneration}",
                        pending.Descriptor.PhysicalWidth,
                        pending.Descriptor.PhysicalHeight,
                        pending.Descriptor.DeviceScaleX,
                        pending.Descriptor.DeviceScaleY);
                    pending.FrameTransaction?.VisibleSurfaceCommitted(
                        pending.FrameTransaction.VisibleTargetIdentity);
                    pending.FrameTransaction?.TryComplete(
                        DorotiFrameTerminal.submitted,
                        reason);
                }
                catch (Exception exception)
                {
                    pending.FrameTransaction?.TryComplete(
                        DorotiFrameTerminal.failed,
                        $"CanvasKit visible commit failed: {exception.Message}");
                    managedTerminal = DorotiFrameTerminal.failed;
                    reason = $"visible commit failed: {exception.Message}";
                }
            }
            else
            {
                pending.FrameTransaction?.TryComplete(
                    managedTerminal,
                    reason);
            }

            if (!_terminalLedger.TryComplete(sceneSequence, managedTerminal)) return;
            lock (_gate)
            {
                if (managedTerminal == DorotiFrameTerminal.submitted) _submitted++;
                else if (managedTerminal == DorotiFrameTerminal.failed) _failed++;
                _contextGeneration = Math.Max(_contextGeneration, traceContextGeneration);
                _surfaceGeneration = Math.Max(_surfaceGeneration, traceSurfaceGeneration);
                _lastInputSequence = pending.InputSequence;
            }

            _frameTrace.Record(
                managedTerminal == DorotiFrameTerminal.submitted
                    ? DorotiFramePhase.present
                    : managedTerminal == DorotiFrameTerminal.superseded
                        ? DorotiFramePhase.superseded
                        : DorotiFramePhase.failed,
                _viewId,
                DorotiFrameClock.Now,
                pending.InputSequence,
                sceneSequence,
                traceSurfaceGeneration,
                reason,
                resizeTargetGeneration: pending.Descriptor.ResizeTargetGeneration,
                metricsGeneration: pending.Descriptor.MetricsGeneration,
                frameworkFrameNumber: pending.Descriptor.FrameworkFrameNumber,
                contextGeneration: traceContextGeneration);
        }
        finally
        {
            _resources.ReleaseSceneResources(pending.Resources);
        }
    }

    public Paragraph Layout(ParagraphRequest request, DartUiInvocation invocation)
    {
        lock (_gate) ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(request);
        _ = invocation;
        ArgumentNullException.ThrowIfNull(request.Text);
        if (!double.IsFinite(request.FontSize) || request.FontSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(request), "Paragraph font size must be finite and positive.");
        if (double.IsNaN(request.Width) || double.IsNegativeInfinity(request.Width) || request.Width < 0)
            throw new ArgumentOutOfRangeException(nameof(request),
                "Paragraph width must be nonnegative or positive infinity.");
        if (request.Height is { } height && (!double.IsFinite(height) || height <= 0))
            throw new ArgumentOutOfRangeException(nameof(request), "Paragraph height must be finite and positive.");
        if (request.MaxLines is <= 0 or > uint.MaxValue)
            throw new ArgumentOutOfRangeException(nameof(request),
                "Paragraph maxLines must be a positive uint32 when supplied.");

        var normalizedFontSize = checked((float)request.FontSize);
        if (!float.IsFinite(normalizedFontSize) || normalizedFontSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(request),
                "Paragraph font size must fit a finite positive f32 recipe value.");
        var normalizedFontFamily = string.IsNullOrWhiteSpace(request.FontFamily)
            ? "DorotiFallback"
            : request.FontFamily.Trim();
        var locale = (request.Locale?.toLanguageTag() ??
            _host.Configuration.locales.FirstOrDefault().toLanguageTag()).Trim();
        var direction = request.TextDirection ?? TextDirection.ltr;
        var align = request.TextAlign ?? TextAlign.start;
        var heightMultiplier = checked((float)(request.Height is { } lineHeight
            ? lineHeight / request.FontSize
            : 1d));
        if (!float.IsFinite(heightMultiplier) || heightMultiplier <= 0)
            throw new ArgumentOutOfRangeException(nameof(request),
                "Paragraph height multiplier must be finite and positive.");
        // The flat DisplayList recipe stores f32 style values. Measure with those
        // same exact values in the UI CanvasKit instance so Raster reconstruction
        // cannot diverge merely because the public API supplied wider doubles.
        var normalizedRequest = request with
        {
            FontFamily = normalizedFontFamily,
            FontSize = normalizedFontSize,
            Height = (double)heightMultiplier * normalizedFontSize,
        };

        var initial = LayoutParagraphSnapshot(normalizedRequest, locale, direction, align);
        var paragraph = new Paragraph(
            normalizedRequest.Text,
            initial.Width,
            initial.Height,
            normalizedFontSize,
            normalizedRequest.MaxLines,
            normalizedFontFamily,
            normalizedRequest.Color,
            initial.CodeUnitAdvances)
        {
            CanvasKitHeightMultiplier = heightMultiplier,
            CanvasKitLocale = locale,
            CanvasKitTextDirection = direction,
            CanvasKitTextAlign = align,
            CanvasKitEllipsis = normalizedRequest.Ellipsis,
            CanvasKitRelayout = width => LayoutParagraphSnapshot(
                normalizedRequest with { Width = width }, locale, direction, align),
        };
        paragraph.ApplyHostLayout(initial);
        return paragraph;
    }

    private ParagraphHostLayoutSnapshot LayoutParagraphSnapshot(
        ParagraphRequest request,
        string locale,
        TextDirection direction,
        TextAlign align)
    {
        lock (_gate) ObjectDisposedException.ThrowIf(_disposed, this);
        var unconstrained = double.IsPositiveInfinity(request.Width);
        var layoutWidth = unconstrained ? 1_000_000d : request.Width;
        var responseJson = BrowserCanvasKitInterop.LayoutParagraph(JsonSerializer.Serialize(new
        {
            schema = "doroti.canvaskit-paragraph/v1",
            text = request.Text,
            width = layoutWidth,
            unconstrained,
            fontFamily = request.FontFamily ?? "DorotiFallback",
            fontSize = request.FontSize,
            maxLines = request.MaxLines,
            color = (request.Color ?? new Color(0xff000000L)).value,
            height = request.Height,
            locale,
            direction = direction == TextDirection.rtl ? "rtl" : "ltr",
            align = align switch
            {
                TextAlign.left => "left",
                TextAlign.right => "right",
                TextAlign.center => "center",
                TextAlign.justify => "justify",
                TextAlign.end => "end",
                _ => "start",
            },
            ellipsis = request.Ellipsis,
        }));
        var response = JsonSerializer.Deserialize<CanvasKitParagraphLayout>(responseJson, JsonOptions)
            ?? throw new InvalidDataException("CanvasKit text layout returned an empty snapshot.");
        if (response.CodeUnitAdvances is null || response.CodeUnitAdvances.Length != request.Text.Length)
            throw new InvalidDataException(
                "CanvasKit text layout code-unit advance count does not match the request text.");
        if (response.UnresolvedCodepoints is null)
            throw new InvalidDataException("CanvasKit text layout omitted unresolved-codepoint diagnostics.");
        if (response.UnresolvedCodepoints.Length != 0)
            throw new InvalidDataException(
                $"CanvasKit text layout has unresolved codepoints: {string.Join(',', response.UnresolvedCodepoints)}.");
        if (!ulong.TryParse(response.MetricsHash, out var metricsHash))
            throw new InvalidDataException("CanvasKit text layout returned an invalid metrics hash.");
        if (response.Lines is null || response.Lines.Length != response.NumberOfLines)
            throw new InvalidDataException("CanvasKit text layout line count does not match its line table.");
        if (response.Graphemes is null)
            throw new InvalidDataException("CanvasKit text layout omitted its grapheme geometry table.");
        return new ParagraphHostLayoutSnapshot(
            response.Width,
            response.Height,
            response.AlphabeticBaseline,
            response.IdeographicBaseline,
            response.MinIntrinsicWidth,
            response.MaxIntrinsicWidth,
            response.LongestLine,
            response.DidExceedMaxLines,
            metricsHash,
            response.CodeUnitAdvances,
            response.Lines.Select(line => new ParagraphHostLineSnapshot(
                line.Start,
                line.End,
                line.HardBreak,
                line.Ascent,
                line.Descent,
                line.Height,
                line.Width,
                line.Left,
                line.Baseline)).ToArray(),
            response.Graphemes.Select(grapheme => new ParagraphHostGraphemeSnapshot(
                grapheme.Start,
                grapheme.End,
                grapheme.Left,
                grapheme.Top,
                grapheme.Right,
                grapheme.Bottom,
                grapheme.Direction switch
                {
                    "ltr" => TextDirection.ltr,
                    "rtl" => TextDirection.rtl,
                    _ => throw new InvalidDataException(
                        $"CanvasKit text layout returned direction '{grapheme.Direction}'."),
                })).ToArray());
    }

    public ValueTask<UiImage> DecodeAsync(
        ReadOnlyMemory<byte> bytes,
        DartUiInvocation invocation,
        CancellationToken cancellationToken = default)
    {
        _ = invocation;
        return _resources.RegisterImageAsync(_viewId, bytes, cancellationToken);
    }

    public void SetEnabled(bool enabled, DartUiInvocation invocation)
    {
        _ = invocation;
        _semanticsEnabled = enabled;
        if (!enabled)
        {
            _semantics.Clear();
            _lastSentSemantics.Clear();
            _host.UpdateSemantics("{\"generation\":0,\"nodes\":[]}");
        }
    }

    public void Update(SemanticsUpdate update, DartUiInvocation invocation)
    {
        _ = invocation;
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (!_semanticsEnabled) return;
        foreach (var node in update.nodes) _semantics[node.id] = node;
        PruneUnreachable(_semantics);
        var orderedNodes = SemanticsGeometryProjection.ToViewCoordinates(_semantics.Values)
            .OrderBy(node => node.indexInParent ?? int.MaxValue)
            .ThenBy(node => node.id)
            .ToArray();
        var nodes = orderedNodes.Select(node =>
        {
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
                    button = node.flags.isButton,
                    textField = node.flags.isTextField,
                    header = node.flags.isHeader,
                    hidden = node.flags.isHidden,
                    image = node.flags.isImage,
                    liveRegion = node.flags.isLiveRegion,
                    multiline = node.flags.isMultiline,
                    readOnly = node.flags.isReadOnly,
                    link = node.flags.isLink,
                    slider = node.flags.isSlider,
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

    public string Paint(
        SKSurface surface,
        int pixelWidth,
        int pixelHeight,
        DorotiResizeEpoch target,
        long requestId) => throw new InvalidOperationException(
        "worker-canvaskit-webgl does not expose a managed Skia paint surface.");

    public void CompletePaint(long requestId, string terminal, string reason)
    {
        _ = requestId;
        _ = terminal;
        _ = reason;
    }

    public void InvalidateGpuContext(long requestId, string reason)
    {
        _ = requestId;
        _ = reason;
    }

    public void InvalidateWindowSurfaceResources()
    {
    }

    public void Dispose()
    {
        PendingScene[] pending;
        lock (_gate)
        {
            if (_disposed) return;
            _disposed = true;
            pending = _pending.Values.ToArray();
            _pending.Clear();
            _failed += pending.Length;
        }
        _host.SemanticsAction -= HandleSemanticsAction;
        foreach (var scene in pending)
        {
            try
            {
                BrowserCanvasKitInterop.ForgetScene(scene.SceneSequence);
                if (_terminalLedger.TryComplete(scene.SceneSequence, DorotiFrameTerminal.failed))
                    scene.FrameTransaction?.TryComplete(
                        DorotiFrameTerminal.failed,
                        "CanvasKit capability disposed before raster receipt");
            }
            finally
            {
                _resources.ReleaseSceneResources(scene.Resources);
            }
        }
        _semantics.Clear();
        _lastSentSemantics.Clear();
        _action = null;
    }

    private void HandleSemanticsAction(long nodeId, long action, string argumentsJson)
    {
        if (_disposed || nodeId is < int.MinValue or > int.MaxValue) return;
        _action?.Invoke(new(
            _viewId,
            checked((int)nodeId),
            (SemanticsAction)action,
            ParseArguments(argumentsJson)));
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
            property => property.Name,
            property => ConvertElement(property.Value),
            StringComparer.Ordinal),
        _ => null,
    };

    private static CanvasKitSceneReceipt ParseReceipt(string json)
    {
        if (string.IsNullOrWhiteSpace(json) || json == "{}") return new();
        try
        {
            return JsonSerializer.Deserialize<CanvasKitSceneReceipt>(json, JsonOptions) ?? new();
        }
        catch (JsonException)
        {
            return new();
        }
    }

    private static long RequireContextGeneration(long contextGeneration)
    {
        if (contextGeneration <= 0 || contextGeneration >= 0x20_0000)
            throw new InvalidDataException(
                $"CanvasKit context generation {contextGeneration} cannot form an exact JavaScript surface identity.");
        return contextGeneration;
    }

    private static long ComposeSurfaceGeneration(long contextGeneration, long resizeGeneration)
    {
        RequireContextGeneration(contextGeneration);
        if (resizeGeneration <= 0 || resizeGeneration >= SurfaceGenerationStride)
            throw new InvalidDataException(
                $"CanvasKit resize generation {resizeGeneration} is outside the uint32 surface-identity range.");
        var value = checked((contextGeneration * SurfaceGenerationStride) + resizeGeneration);
        if (value > JavaScriptMaximumSafeInteger)
            throw new InvalidDataException(
                $"CanvasKit surface generation {value} exceeds JavaScript's exact integer range.");
        return value;
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private static readonly JsonSerializerOptions SemanticsJsonOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    private sealed record PendingScene(
        long SceneSequence,
        long InputSequence,
        TimeSpan SubmittedAt,
        DorotiFrameDescriptor Descriptor,
        long ContextGeneration,
        long SurfaceGeneration,
        DisplayResourceReference[] Resources,
        DorotiFrameTransaction? FrameTransaction);

    private sealed record CanvasKitSceneReceipt(
        long SurfaceGeneration = 0,
        long ContextGeneration = 0);

    private sealed record CanvasKitParagraphLayout(
        double Width,
        double Height,
        double AlphabeticBaseline,
        double IdeographicBaseline,
        double MinIntrinsicWidth,
        double MaxIntrinsicWidth,
        double LongestLine,
        bool DidExceedMaxLines,
        int NumberOfLines,
        string MetricsHash,
        double[] CodeUnitAdvances,
        CanvasKitParagraphLine[] Lines,
        CanvasKitParagraphGrapheme[] Graphemes,
        int[] UnresolvedCodepoints);

    private sealed record CanvasKitParagraphLine(
        int Start,
        int End,
        bool HardBreak,
        double Ascent,
        double Descent,
        double Height,
        double Width,
        double Left,
        double Baseline);

    private sealed record CanvasKitParagraphGrapheme(
        int Start,
        int End,
        double Left,
        double Top,
        double Right,
        double Bottom,
        string Direction);
}
