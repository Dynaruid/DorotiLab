// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/overlay.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public delegate Widget OverlayChildLayoutBuilder(BuildContext context, OverlayChildLayoutInfo info);

public class OverlayChildLayoutInfo
{
    public (Size, Matrix4, Size) _info { get; }

    private OverlayChildLayoutInfo((Size, Matrix4, Size) _info)
    {
        this._info = _info;
    }

    public static OverlayChildLayoutInfo Create_((Size, Matrix4, Size) _info) => new OverlayChildLayoutInfo(_info);

    public static implicit operator (Size, Matrix4, Size)(OverlayChildLayoutInfo value) => value._info;
    public static implicit operator OverlayChildLayoutInfo((Size, Matrix4, Size) value) => new OverlayChildLayoutInfo(value);

    public virtual Size childSize => DartRuntimePrimitives.ConvertValue<Size>(_info.Item1);
    public virtual Matrix4 childPaintTransform => _info.Item2;
    public virtual Size overlaySize => DartRuntimePrimitives.ConvertValue<Size>(_info.Item3);
}

public class OverlayEntry : Listenable
{
    public virtual Func<BuildContext, Widget> builder { get; private set; } = default!;
    internal virtual bool _opaque { get; set; } = default!;
    internal virtual bool _maintainState { get; set; } = default!;
    public virtual bool canSizeOverlay { get; private set; } = default!;
    internal virtual ValueNotifier<_OverlayEntryWidgetState__overlay?>? _overlayEntryStateNotifier { get; set; } = new ValueNotifier<_OverlayEntryWidgetState__overlay?>(null);
    internal virtual OverlayState? _overlay { get; set; } = default;
    internal virtual GlobalKey<_OverlayEntryWidgetState__overlay> _key { get; private set; } = GlobalKey<_OverlayEntryWidgetState__overlay>.Create();
    internal virtual bool _disposedByOwner { get; set; } = false;

    public OverlayEntry(Func<BuildContext, Widget> builder, bool opaque = false, bool maintainState = false, bool canSizeOverlay = false)
    {
        this.builder = builder;
        this.canSizeOverlay = canSizeOverlay;
        _opaque = opaque;
        _maintainState = maintainState;
    }

    public virtual bool opaque
    {
        get => _opaque;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => !_disposedByOwner);
            if (_opaque == __value)
            {
                return;
            }
            _opaque = __value;
            _overlay?._didChangeEntryOpacity();
        }
    }
    public virtual bool maintainState
    {
        get => _maintainState;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => !_disposedByOwner);
            if (_maintainState == __value)
            {
                return;
            }
            _maintainState = __value;
            DartRuntimePrimitives.Assert(() => _overlay is not null);
            _overlay!._didChangeEntryOpacity();
        }
    }
    public virtual bool mounted => DartRuntimePrimitives.ConvertValue<bool>(_overlayEntryStateNotifier?.value is not null);
    public virtual void addListener(Action listener)
    {
        DartRuntimePrimitives.Assert(() => !_disposedByOwner);
        _overlayEntryStateNotifier?.addListener(listener);
    }

    public virtual void removeListener(Action listener)
    {
        _overlayEntryStateNotifier?.removeListener(listener);
    }

    public virtual void remove()
    {
        DartRuntimePrimitives.Assert(() => _overlay is not null, () => (object?)"An OverlayEntry should be removed only once.");
        DartRuntimePrimitives.Assert(() => !_disposedByOwner);
        OverlayState overlay = _overlay!;
        _overlay = null;
        if (!overlay.mounted)
        {
            return;
        }
        overlay._entries.Remove(this);
        if (Equals(Scheduler.SchedulerBinding.instance.schedulerPhase, Scheduler.SchedulerPhase.persistentCallbacks))
        {
            Scheduler.SchedulerBinding.instance.addPostFrameCallback((duration) =>
            {
                overlay._markDirty();
            }, debugLabel: "OverlayEntry.markDirty");
        }
        else
        {
            overlay._markDirty();
        }
    }

    public virtual void markNeedsBuild()
    {
        DartRuntimePrimitives.Assert(() => !_disposedByOwner);
        _key.currentState?._markNeedsBuild();
    }

    internal virtual void _didUnmount()
    {
        DartRuntimePrimitives.Assert(() => !mounted);
        if (_disposedByOwner)
        {
            _overlayEntryStateNotifier?.dispose();
            _overlayEntryStateNotifier = null;
        }
    }

    public virtual void dispose()
    {
        DartRuntimePrimitives.Assert(() => !_disposedByOwner);
        DartRuntimePrimitives.Assert(() => _overlay is null, () => (object?)"An OverlayEntry must first be removed from the Overlay before dispose is called.");
        DartRuntimePrimitives.Assert(() => Foundation.DebugLibrary.debugMaybeDispatchDisposed(this));
        _disposedByOwner = true;
        if (!mounted)
        {
            _overlayEntryStateNotifier?.dispose();
            _overlayEntryStateNotifier = null;
        }
    }

    public override string ToString() => $"{DiagnosticsLibrary.describeIdentity(this)}(opaque: {opaque}; maintainState: {maintainState}){(_disposedByOwner ? "(DISPOSED)" : "")}";
}

public class _OverlayEntryWidget__overlay : StatefulWidget
{
    public virtual OverlayEntry entry { get; private set; } = default!;
    public virtual OverlayState overlayState { get; private set; } = default!;
    public virtual bool tickerEnabled { get; private set; } = default!;

    internal _OverlayEntryWidget__overlay(Key key, OverlayEntry entry, OverlayState overlayState, bool tickerEnabled = true) : base(key: key)
    {
        this.entry = entry;
        this.overlayState = overlayState;
        this.tickerEnabled = tickerEnabled;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _OverlayEntryWidgetState__overlay());
}

public class _OverlayEntryWidgetState__overlay : State<_OverlayEntryWidget__overlay>
{
    internal virtual _RenderTheater__overlay _theater { get; set; } = default!;
    internal virtual DartLinkedList<_OverlayEntryLocation__overlay>? _sortedTheaterSiblings { get; set; } = default;
    private bool __late__paintOrderIterable_initialized;
    private IEnumerable<_RenderDeferredLayoutBox__overlay> __late__paintOrderIterable = default!;
    internal virtual IEnumerable<_RenderDeferredLayoutBox__overlay> _paintOrderIterable
    {
        get
        {
            if (!__late__paintOrderIterable_initialized)
            {
                __late__paintOrderIterable = _createChildIterable(reversed: false);
                __late__paintOrderIterable_initialized = true;
            }
            return __late__paintOrderIterable;
        }
    }
    private bool __late__hitTestOrderIterable_initialized;
    private IEnumerable<_RenderDeferredLayoutBox__overlay> __late__hitTestOrderIterable = default!;
    internal virtual IEnumerable<_RenderDeferredLayoutBox__overlay> _hitTestOrderIterable
    {
        get
        {
            if (!__late__hitTestOrderIterable_initialized)
            {
                __late__hitTestOrderIterable = _createChildIterable(reversed: true);
                __late__hitTestOrderIterable_initialized = true;
            }
            return __late__hitTestOrderIterable;
        }
    }

    internal virtual void _add(_OverlayEntryLocation__overlay child)
    {
        DartRuntimePrimitives.Assert(() => mounted);
        DartLinkedList<_OverlayEntryLocation__overlay> children = _sortedTheaterSiblings ??= new DartLinkedList<_OverlayEntryLocation__overlay>();
        DartRuntimePrimitives.Assert(() => !children.contains(child));
        _OverlayEntryLocation__overlay? insertPosition = children.isEmpty ? null : children.last;
        while ((insertPosition is not null) && (insertPosition._zOrderIndex > child._zOrderIndex))
        {
            insertPosition = insertPosition.previous;
        }
        if (insertPosition is null)
        {
            children.addFirst(child);
        }
        else
        {
            insertPosition.insertAfter(child);
        }
        DartRuntimePrimitives.Assert(() => children.contains(child));
    }

    internal virtual void _remove(_OverlayEntryLocation__overlay child)
    {
        DartRuntimePrimitives.Assert(() => _sortedTheaterSiblings is not null);
        bool wasInCollection = _sortedTheaterSiblings?.remove(child) ?? false;
        DartRuntimePrimitives.Assert(() => wasInCollection);
    }

    internal virtual IEnumerable<_RenderDeferredLayoutBox__overlay> _createChildIterable(bool reversed)
    {
        DartLinkedList<_OverlayEntryLocation__overlay>? children = _sortedTheaterSiblings;
        if ((children is null) || children.isEmpty)
        {
            yield break;
        }
        _OverlayEntryLocation__overlay? candidate = reversed ? children.last : children.first;
        while (candidate is not null)
        {
            _RenderDeferredLayoutBox__overlay? renderBox = candidate._overlayChildRenderBox;
            candidate = reversed ? candidate.previous : candidate.next;
            if (renderBox is not null)
            {
                yield return renderBox;
            }
        }
    }

    public override void initState()
    {
        base.initState();
        widget.entry._overlayEntryStateNotifier!.value = this;
        _theater = context.findAncestorRenderObjectOfType<_RenderTheater__overlay>()!;
        DartRuntimePrimitives.Assert(() => _sortedTheaterSiblings is null);
    }

    public override void didUpdateWidget(_OverlayEntryWidget__overlay oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        DartRuntimePrimitives.Assert(() => Equals(oldWidget.entry, widget.entry));
        if (!Equals(oldWidget.overlayState, widget.overlayState))
        {
            _RenderTheater__overlay newTheater = context.findAncestorRenderObjectOfType<_RenderTheater__overlay>()!;
            DartRuntimePrimitives.Assert(() => !Equals(_theater, newTheater));
            _theater = newTheater;
        }
    }

    public override void dispose()
    {
        widget.entry._overlayEntryStateNotifier?.value = null;
        widget.entry._didUnmount();
        _sortedTheaterSiblings = null;
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        return new TickerMode(enabled: widget.tickerEnabled, child: new _RenderTheaterMarker__overlay(theater: _theater, overlayEntryWidgetState: this, child: new Builder(builder: widget.entry.builder)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _markNeedsBuild()
    {
        setState(() =>
        {
        });
    }

}

public class Overlay : StatefulWidget
{
    public virtual List<OverlayEntry> initialEntries { get; private set; } = default!;
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual bool alwaysSizeToContent { get; private set; } = default!;

    public Overlay(Key? key = null, List<OverlayEntry> initialEntries = default!, Clip clipBehavior = Clip.hardEdge, bool alwaysSizeToContent = false) : base(key: key)
    {
        List<OverlayEntry> __initialEntries = initialEntries ?? new List<OverlayEntry>();
        this.initialEntries = __initialEntries;
        this.clipBehavior = clipBehavior;
        this.alwaysSizeToContent = alwaysSizeToContent;
    }

    public static Widget wrap(Key? key = null, Clip clipBehavior = Clip.hardEdge, bool alwaysSizeToContent = false, Widget child = default!)
    {
        return new _WrappingOverlay__overlay(key: key, clipBehavior: clipBehavior, alwaysSizeToContent: alwaysSizeToContent, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static OverlayState of(BuildContext context, bool rootOverlay = false, Widget? debugRequiredFor = null)
    {
        OverlayState? result = maybeOf(context, rootOverlay: rootOverlay);
        DartRuntimePrimitives.Assert(() =>
            {
                if (result is null)
                {
                    bool hiddenByBoundary = LookupBoundary.debugIsHidingAncestorStateOfType<OverlayState>(context);
                    var information = new List<DiagnosticsNode> { new ErrorSummary($"No Overlay widget found{(hiddenByBoundary ? " within the closest LookupBoundary" : "")}."), new ErrorDescription($"{(object?)DartRuntimePrimitives.RuntimeType(debugRequiredFor) ?? (object?)"Some"} widgets require an Overlay widget ancestor for correct operation."), new ErrorHint("The most common way to add an Overlay to an application is to include a MaterialApp, CupertinoApp or Navigator widget in the runApp() call.") };
                    throw DartRuntimePrimitives.AsException(new FlutterError(information));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return result!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static OverlayState? maybeOf(BuildContext context, bool rootOverlay = false)
    {
        return _RenderTheaterMarker__overlay.maybeOf(context, targetRootOverlay: rootOverlay, createDependency: false)?.overlayEntryWidgetState.widget.overlayState;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new OverlayState());
}

public class OverlayState : State<Overlay>, TickerProviderStateMixin<Overlay>
{
    internal virtual List<OverlayEntry> _entries { get; private set; } = new List<OverlayEntry>();
    public virtual HashSet<Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override void initState()
    {
        base.initState();
        insertAll(widget.initialEntries.Cast<OverlayEntry>());
    }

    internal virtual long _insertionIndex(OverlayEntry? below, OverlayEntry? above)
    {
        DartRuntimePrimitives.Assert(() => (above is null) || (below is null));
        if (below is not null)
        {
            return _entries.IndexOf(below);
        }
        if (above is not null)
        {
            return _entries.IndexOf(above) + 1L;
        }
        return checked(_entries.Count);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _debugCanInsertEntry(OverlayEntry entry)
    {
        var operandsInformation = new List<DiagnosticsNode> { new DiagnosticsProperty<OverlayEntry>("The OverlayEntry was", entry, style: DiagnosticsTreeStyle.errorProperty), new DiagnosticsProperty<OverlayState>("The Overlay the OverlayEntry was trying to insert to was", this, style: DiagnosticsTreeStyle.errorProperty) };
        if (!mounted)
        {
            throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("Attempted to insert an OverlayEntry to an already disposed Overlay.") }));
        }
        OverlayState? currentOverlay = entry._overlay;
        bool alreadyContainsEntry = _entries.Contains(entry);
        if (alreadyContainsEntry)
        {
            bool inconsistentOverlayState = !DartRuntimePrimitives.Identical(currentOverlay, this);
            throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("The specified entry is already present in the target Overlay.") }));
        }
        if (currentOverlay is null)
        {
            return true;
        }
        throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("The specified entry is already present in a different Overlay."), new DiagnosticsProperty<OverlayState>("The OverlayEntry's current Overlay was", currentOverlay, style: DiagnosticsTreeStyle.errorProperty), new ErrorHint("Consider calling remove on the OverlayEntry before inserting it to a different Overlay, " + "or switching to the OverlayPortal API to avoid manual OverlayEntry management.") }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void insert(OverlayEntry entry, OverlayEntry? below = null, OverlayEntry? above = null)
    {
        DartRuntimePrimitives.Assert(() => _debugVerifyInsertPosition(above, below));
        DartRuntimePrimitives.Assert(() => _debugCanInsertEntry(entry));
        entry._overlay = this;
        setState(() =>
        {
            _entries.Insert(checked((int)_insertionIndex(below, above)), entry);
        });
    }

    public virtual void insertAll(IEnumerable<OverlayEntry> entries, OverlayEntry? below = null, OverlayEntry? above = null)
    {
        DartRuntimePrimitives.Assert(() => _debugVerifyInsertPosition(above, below));
        DartRuntimePrimitives.Assert(() => entries.All(_debugCanInsertEntry));
        if (!Enumerable.Any(entries))
        {
            return;
        }
        foreach (var entry in entries)
        {
            DartRuntimePrimitives.Assert(() => entry._overlay is null);
            entry._overlay = this;
        }
        setState(() =>
        {
            _entries.InsertRange(checked((int)_insertionIndex(below, above)), entries.Cast<OverlayEntry>());
        });
    }

    internal virtual bool _debugVerifyInsertPosition(OverlayEntry? above, OverlayEntry? below, IEnumerable<OverlayEntry>? newEntries = null)
    {
        DartRuntimePrimitives.Assert(() => (above is null) || (below is null), () => (object?)"Only one of `above` and `below` may be specified.");
        DartRuntimePrimitives.Assert(() => (above is null) || Equals(above._overlay, this) && _entries.Contains(above) && (newEntries?.contains(above) ?? true), () => (object?)$"The provided entry used for `above` must be present in the Overlay{((newEntries is not null) ? " and in the `newEntriesList`" : "")}.");
        DartRuntimePrimitives.Assert(() => (below is null) || Equals(below._overlay, this) && _entries.Contains(below) && (newEntries?.contains(below) ?? true), () => (object?)$"The provided entry used for `below` must be present in the Overlay{((newEntries is not null) ? " and in the `newEntriesList`" : "")}.");
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void rearrange(IEnumerable<OverlayEntry> newEntries, OverlayEntry? below = null, OverlayEntry? above = null)
    {
        List<OverlayEntry> newEntriesList = ((newEntries is List<OverlayEntry>) ? newEntries : newEntries.ToList()).ToList();
        DartRuntimePrimitives.Assert(() => _debugVerifyInsertPosition(above, below, newEntries: newEntriesList.Cast<OverlayEntry>()));
        DartRuntimePrimitives.Assert(() => newEntriesList.All((entry) => (entry._overlay is null) || Equals(entry._overlay, this)), () => (object?)"One or more of the specified entries are already present in another Overlay.");
        DartRuntimePrimitives.Assert(() => newEntriesList.All((entry) => ((long)_entries.IndexOf(entry)) == _entries.LastIndexOf(entry)), () => (object?)"One or more of the specified entries are specified multiple times.");
        if (!Enumerable.Any(newEntriesList))
        {
            return;
        }
        if (CollectionsLibrary.listEquals(_entries, newEntriesList))
        {
            return;
        }
        var old = new HashSet<OverlayEntry>(_entries);
        foreach (var entryLocal in newEntriesList)
        {
            entryLocal._overlay ??= this;
        }
        setState(() =>
        {
            _entries.Clear();
            _entries.AddRange(newEntriesList.Cast<OverlayEntry>());
            old.ExceptWith(newEntriesList);
            _entries.InsertRange(checked((int)_insertionIndex(below, above)), old);
        });
    }

    internal virtual void _markDirty()
    {
        if (mounted)
        {
            setState(() =>
            {
            });
        }
    }

    public virtual bool debugIsVisible(OverlayEntry entry)
    {
        var result = false;
        DartRuntimePrimitives.Assert(() => _entries.Contains(entry));
        DartRuntimePrimitives.Assert(() =>
            {
                for (long i = checked(_entries.Count) - 1L; i > 0L; i -= 1L)
                {
                    OverlayEntry candidate = _entries[(int)i];
                    if (Equals(candidate, entry))
                    {
                        result = true;
                        break;
                    }
                    if (candidate.opaque)
                    {
                        break;
                    }
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _didChangeEntryOpacity()
    {
        setState(() =>
        {
        });
    }

    public override Widget build(BuildContext context)
    {
        var childrenLocal = new List<_OverlayEntryWidget__overlay>();
        var onstage = true;
        var onstageCount = 0L;
        foreach (OverlayEntry entryLocal in Enumerable.Reverse(_entries))
        {
            if (onstage)
            {
                onstageCount += 1L;
                childrenLocal.Add(new _OverlayEntryWidget__overlay(key: entryLocal._key, overlayState: this, entry: entryLocal));
                if (entryLocal.opaque)
                {
                    onstage = false;
                }
            }
            else
            {
                if (entryLocal.maintainState)
                {
                    childrenLocal.Add(new _OverlayEntryWidget__overlay(key: entryLocal._key, overlayState: this, entry: entryLocal, tickerEnabled: false));
                }
            }
        }
        return new _Theater__overlay(skipCount: checked(childrenLocal.Count) - onstageCount, clipBehavior: widget.clipBehavior, alwaysSizeToContent: widget.alwaysSizeToContent, children: Enumerable.Reverse(childrenLocal).ToList());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<HashSet<Scheduler.Ticker>>("tickers", _tickers, description: (_tickers is not null) ? $"tracking {checked((long)_tickers!.Count)} ticker{((checked(_tickers!.Count) == 1L) ? "" : "s")}" : null, defaultValue: default));
        properties.add(new DiagnosticsProperty<List<OverlayEntry>>("entries", _entries));
    }

    public virtual Scheduler.Ticker createTicker(Action<Duration> onTick)
    {
        if (_tickerModeNotifier is null)
        {
            _updateTickerModeNotifier();
        }
        DartRuntimePrimitives.Assert(() => _tickerModeNotifier is not null);
        _tickers ??= new HashSet<Scheduler.Ticker>();
        TickerModeData values = _tickerModeNotifier!.value;
        var result = ((Func<_WidgetTicker__ticker_provider>)(() =>
{
    var __cascade = new _WidgetTicker__ticker_provider(onTick, this, debugLabel: Foundation.ConstantsLibrary.kDebugMode ? $"created by {DiagnosticsLibrary.describeIdentity(this)}" : null);
    __cascade.muted = !values.enabled;
    __cascade.forceFrames = values.forceFrames;
    return __cascade;
}))();
        _tickers!.Add(result);
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _removeTicker(_WidgetTicker__ticker_provider ticker)
    {
        DartRuntimePrimitives.Assert(() => _tickers is not null);
        DartRuntimePrimitives.Assert(() => _tickers!.Contains(ticker));
        _tickers!.Remove(ticker);
    }

    public override void activate()
    {
        base.activate();
        _updateTickerModeNotifier();
        _updateTickers();
    }

    public virtual void _updateTickers()
    {
        if (_tickers is not null)
        {
            TickerModeData values = _tickerModeNotifier!.value;
            bool mutedLocal = !values.enabled;
            foreach (Scheduler.Ticker ticker in _tickers!)
            {
                ticker.muted = mutedLocal;
                ticker.forceFrames = values.forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        ValueListenable<TickerModeData> newNotifier = TickerMode.getValuesNotifier(context);
        if (Equals(newNotifier, _tickerModeNotifier))
        {
            return;
        }
        _tickerModeNotifier?.removeListener(_updateTickers);
        newNotifier.addListener(_updateTickers);
        _tickerModeNotifier = newNotifier;
    }

    public override void dispose()
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (_tickers is not null)
                {
                    foreach (Scheduler.Ticker ticker in _tickers!)
                    {
                        if (ticker.isActive)
                        {
                            throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"{this} was disposed with an active Ticker."), new ErrorDescription($"{GetType()} created a Ticker via its TickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. All Tickers must " + "be disposed before calling super.dispose()."), new ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), ticker.describeForError("The offending ticker was") }));
                        }
                    }
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        _tickerModeNotifier?.removeListener(_updateTickers);
        _tickerModeNotifier = null;
        base.dispose();
    }

}

public class _WrappingOverlay__overlay : StatefulWidget
{
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual bool alwaysSizeToContent { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;

    internal _WrappingOverlay__overlay(Key? key = null, Clip clipBehavior = Clip.hardEdge, bool alwaysSizeToContent = default!, Widget child = default!) : base(key: key)
    {
        this.clipBehavior = clipBehavior;
        this.alwaysSizeToContent = alwaysSizeToContent;
        this.child = child;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _WrappingOverlayState__overlay());
}

internal class _WrappingOverlayState__overlay : State<_WrappingOverlay__overlay>
{
    private bool __late__entry_initialized;
    private OverlayEntry __late__entry = default!;
    internal virtual OverlayEntry _entry
    {
        get
        {
            if (!__late__entry_initialized)
            {
                __late__entry = new OverlayEntry(canSizeOverlay: true, opaque: true, builder: (context) =>
                {
                    return widget.child;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
                __late__entry_initialized = true;
            }
            return __late__entry;
        }
    }

    public override void didUpdateWidget(_WrappingOverlay__overlay oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        _entry.markNeedsBuild();
    }

    public override void dispose()
    {
        DartRuntimePrimitives.Ignore(((Func<OverlayEntry>)(() =>
{
    var __cascade = _entry;
    __cascade.remove();
    __cascade.dispose();
    return __cascade;
}))());
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        return new Overlay(clipBehavior: widget.clipBehavior, alwaysSizeToContent: widget.alwaysSizeToContent, initialEntries: new List<OverlayEntry> { _entry });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _Theater__overlay : MultiChildRenderObjectWidget
{
    public virtual long skipCount { get; private set; } = default!;
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual bool alwaysSizeToContent { get; private set; } = default!;

    internal _Theater__overlay(long skipCount = 0, Clip clipBehavior = Clip.hardEdge, bool alwaysSizeToContent = default!, List<_OverlayEntryWidget__overlay> children = default!) : base(children: children)
    {
        this.skipCount = skipCount;
        this.clipBehavior = clipBehavior;
        this.alwaysSizeToContent = alwaysSizeToContent;
        System.Diagnostics.Debug.Assert(skipCount >= 0L);
        System.Diagnostics.Debug.Assert(checked(children.Count) >= skipCount);
    }

    public override _TheaterElement__overlay createElement() => new _TheaterElement__overlay(this);
    public override RenderObject createRenderObject(BuildContext context)
    {
        return new _RenderTheater__overlay(skipCount: skipCount, textDirection: Directionality.of(context), clipBehavior: clipBehavior, alwaysSizeToContent: alwaysSizeToContent);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (_RenderTheater__overlay)renderObject;
        DartRuntimePrimitives.Ignore(((Func<_RenderTheater__overlay>)(() =>
{
    var __cascade = __renderObject;
    __cascade.skipCount = skipCount;
    __cascade.textDirection = Directionality.of(context);
    __cascade.clipBehavior = clipBehavior;
    __cascade.alwaysSizeToContent = alwaysSizeToContent;
    return __cascade;
}))());
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new IntProperty("skipCount", skipCount));
    }

}

public class _TheaterElement__overlay : MultiChildRenderObjectElement
{
    internal _TheaterElement__overlay(_Theater__overlay widget) : base(widget)
    {
    }

    public override RenderObject renderObject => DartRuntimePrimitives.ConvertValue<RenderObject>(((_RenderTheater__overlay?)base.renderObject)!);
    public override void insertRenderObjectChild(RenderObject child, object? slot)
    {
        var __child = (RenderBox)child;
        var __slot = slot as IndexedSlot<Element?> ?? throw new ArgumentException("An overlay child requires an indexed slot.", nameof(slot));
        base.insertRenderObjectChild(__child, __slot);
        var parentDataLocal = ((_TheaterParentData__overlay?)__child.parentData!)!;
        parentDataLocal.overlayEntry = ((_OverlayEntryWidget__overlay?)((_Theater__overlay?)widget)!.children[(int)__slot.index])!.entry;
        DartRuntimePrimitives.Assert(() => parentDataLocal.overlayEntry is not null);
    }

    public override void moveRenderObjectChild(RenderObject child, object? oldSlot, object? newSlot)
    {
        var __child = (RenderBox)child;
        var __oldSlot = oldSlot as IndexedSlot<Element?> ?? throw new ArgumentException("An overlay child requires an indexed slot.", nameof(oldSlot));
        var __newSlot = newSlot as IndexedSlot<Element?> ?? throw new ArgumentException("An overlay child requires an indexed slot.", nameof(newSlot));
        base.moveRenderObjectChild(__child, __oldSlot, __newSlot);
        DartRuntimePrimitives.Assert(() =>
            {
                var parentDataLocal = ((_TheaterParentData__overlay?)__child.parentData!)!;
                OverlayEntry entryAtNewSlot = ((_OverlayEntryWidget__overlay?)((_Theater__overlay?)widget)!.children[(int)__newSlot.index])!.entry;
                DartRuntimePrimitives.Assert(() => Equals(parentDataLocal.overlayEntry, entryAtNewSlot));
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
    }

    public override void debugVisitOnstageChildren(Action<Element> visitor)
    {
        var theater = ((_Theater__overlay?)widget)!;
        DartRuntimePrimitives.Assert(() => children.Count() >= theater.skipCount);
        children.skip(theater.skipCount).forEach((__arg0) => visitor(__arg0));
    }

}

internal interface _RenderTheaterMixin__overlay
{
    public _RenderTheater__overlay theater { get; }
    public IEnumerable<RenderBox> _childrenInPaintOrder();
    public IEnumerable<RenderBox> _childrenInHitTestOrder();
    public void setupParentData(RenderObject child);
    public double? computeDistanceToActualBaseline(TextBaseline baseline);
    public static double? baselineForChild(RenderBox child, Size theaterSize, BoxConstraints nonPositionedChildConstraints, Alignment alignment, TextBaseline baseline)
    {
        var childParentData = ((StackParentData?)child.parentData!)!;
        BoxConstraints childConstraints = childParentData.isPositioned ? childParentData.positionedChildConstraints(theaterSize) : nonPositionedChildConstraints;
        double? baselineOffset = child.getDryBaseline(childConstraints, baseline);
        if (baselineOffset is null)
        {
            return null;
        }
        double y = childParentData switch { StackParentData { top: double topLocal } __object40535 => topLocal, StackParentData { bottom: double bottomLocal } __object40585 => theaterSize.height - bottomLocal - child.getDryLayout(childConstraints).height, StackParentData __object40716 => alignment.alongOffset(theaterSize - child.getDryLayout(childConstraints)).dy, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        return DartRuntimePrimitives.RequireValue(baselineOffset) + y;
    }
    public void layoutChild(RenderBox child, BoxConstraints nonPositionedChildConstraints);
    public bool hitTestChildren(BoxHitTestResult result, Offset position);
    public void paint(PaintingContext context, Offset offset);
}

internal class _TheaterParentData__overlay : StackParentData
{
    public virtual OverlayEntry? overlayEntry { get; set; } = default;

    public virtual IEnumerator<_RenderDeferredLayoutBox__overlay>? paintOrderIterator => overlayEntry?._overlayEntryStateNotifier?.value!._paintOrderIterable.GetEnumerator();
    public virtual IEnumerator<_RenderDeferredLayoutBox__overlay>? hitTestOrderIterator => overlayEntry?._overlayEntryStateNotifier?.value!._hitTestOrderIterable.GetEnumerator();
    public virtual void visitOverlayPortalChildrenOnOverlayEntry(Action<RenderObject> visitor) => overlayEntry?._overlayEntryStateNotifier?.value!._paintOrderIterable.forEach((__arg0) => visitor(DartRuntimePrimitives.ConvertValue<RenderObject>(__arg0)));
}

public class _RenderTheater__overlay : RenderBox, ContainerRenderObjectMixin<RenderBox, StackParentData>, _RenderTheaterMixin__overlay
{
    internal virtual Alignment? _alignmentCache { get; set; } = default;
    internal virtual TextDirection _textDirection { get; set; } = default!;
    internal virtual long _skipCount { get; set; } = default!;
    internal virtual Clip _clipBehavior { get; set; } = Clip.hardEdge;
    internal virtual bool _alwaysSizeToContent { get; set; } = default!;
    internal virtual long _outstandingDeferredChildUpdateCalls { get; set; } = 0L;
    internal virtual bool _layingOutSizeDeterminingChild { get; set; } = false;
    internal virtual LayerHandle<ClipRectLayer> _clipRectLayer { get; private set; } = new LayerHandle<ClipRectLayer>();
    public virtual long _childCount { get; set; } = 0L;
    public virtual RenderBox? _firstChild { get; set; } = default;
    public virtual RenderBox? _lastChild { get; set; } = default;

    internal _RenderTheater__overlay(List<RenderBox>? children = null, TextDirection textDirection = default!, long skipCount = 0, Clip clipBehavior = Clip.hardEdge, bool alwaysSizeToContent = default!)
    {
        _textDirection = textDirection;
        _skipCount = skipCount;
        _clipBehavior = clipBehavior;
        _alwaysSizeToContent = alwaysSizeToContent;
        System.Diagnostics.Debug.Assert(skipCount >= 0L);
    }

    public virtual _RenderTheater__overlay theater => this;
    public override void setupParentData(RenderObject child)
    {
        var __child = (RenderBox)child;
        if (__child.parentData is not _TheaterParentData__overlay)
        {
            __child.parentData = new _TheaterParentData__overlay();
        }
    }

    public override void attach(PipelineOwner owner)
    {
        base.attach(owner);
        RenderBox? child = _firstChild;
        while (child is not null)
        {
            child.attach(owner);
            var childParentData = ((StackParentData?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
        RenderBox? childLocal = firstChild;
        while (childLocal is not null)
        {
            var childParentDataLocal = ((_TheaterParentData__overlay?)childLocal.parentData!)!;
            IEnumerator<RenderBox>? iterator = childParentDataLocal.paintOrderIterator;
            if (iterator is not null)
            {
                while (iterator.MoveNext())
                {
                    iterator.Current.attach(owner);
                }
            }
            childLocal = childParentDataLocal.nextSibling;
        }
    }

    internal static void _detachChild(RenderObject child) => child.detach();
    public override void detach()
    {
        base.detach();
        RenderBox? child = _firstChild;
        while (child is not null)
        {
            child.detach();
            var childParentData = ((StackParentData?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
        RenderBox? childLocal = firstChild;
        while (childLocal is not null)
        {
            var childParentDataLocal = ((_TheaterParentData__overlay?)childLocal.parentData!)!;
            childParentDataLocal.visitOverlayPortalChildrenOnOverlayEntry(_detachChild);
            childLocal = childParentDataLocal.nextSibling;
        }
    }

    public override void redepthChildren() => visitChildren(redepthChild);
    internal virtual Alignment _resolvedAlignment => _alignmentCache ??= AlignmentDirectional.topStart.resolve(textDirection);
    internal virtual void _markNeedResolution()
    {
        _alignmentCache = null;
        markNeedsLayout();
    }

    public virtual TextDirection textDirection
    {
        get => _textDirection;
        set
        {
            var __value = value;
            if (Equals(_textDirection, __value))
            {
                return;
            }
            _textDirection = __value;
            _markNeedResolution();
        }
    }
    public virtual long skipCount
    {
        get => _skipCount;
        set
        {
            var __value = value;
            if (_skipCount != __value)
            {
                _skipCount = __value;
                markNeedsLayout();
            }
        }
    }
    public virtual Clip clipBehavior
    {
        get => _clipBehavior;
        set
        {
            var __value = value;
            if (!Equals(__value, _clipBehavior))
            {
                _clipBehavior = __value;
                markNeedsPaint();
                markNeedsSemanticsUpdate();
            }
        }
    }
    public virtual bool alwaysSizeToContent
    {
        get => _alwaysSizeToContent;
        set
        {
            var __value = value;
            if (_alwaysSizeToContent != __value)
            {
                _alwaysSizeToContent = __value;
                markNeedsLayout();
            }
        }
    }
    internal virtual void _addDeferredChild(_RenderDeferredLayoutBox__overlay child)
    {
        _outstandingDeferredChildUpdateCalls += 1L;
        adoptChild(child);
        markNeedsPaint();
        _outstandingDeferredChildUpdateCalls -= 1L;
        DartRuntimePrimitives.Assert(() => _outstandingDeferredChildUpdateCalls >= 0L);
        child._layoutSurrogate.markNeedsLayout();
    }

    internal virtual void _removeDeferredChild(_RenderDeferredLayoutBox__overlay child)
    {
        _outstandingDeferredChildUpdateCalls += 1L;
        dropChild(child);
        markNeedsPaint();
        _outstandingDeferredChildUpdateCalls -= 1L;
        DartRuntimePrimitives.Assert(() => _outstandingDeferredChildUpdateCalls >= 0L);
    }

    public override void markNeedsLayout()
    {
        if (_outstandingDeferredChildUpdateCalls == 0L)
        {
            base.markNeedsLayout();
        }
    }

    internal virtual RenderBox? _firstOnstageChild
    {
        get
        {
            if (skipCount == childCount)
            {
                return null;
            }
            RenderBox? child = firstChild;
            for (long toSkip = skipCount; toSkip > 0L; toSkip--)
            {
                var childParentData = ((StackParentData?)child!.parentData!)!;
                child = childParentData.nextSibling;
                DartRuntimePrimitives.Assert(() => child is not null);
            }
            return child;
        }
    }
    internal virtual RenderBox? _lastOnstageChild => (skipCount == childCount) ? null : lastChild;
    public override double computeMinIntrinsicWidth(double height)
    {
        return RenderStack.getIntrinsicDimension(_firstOnstageChild, (child) => child.getMinIntrinsicWidth(height));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        return RenderStack.getIntrinsicDimension(_firstOnstageChild, (child) => child.getMaxIntrinsicWidth(height));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        return RenderStack.getIntrinsicDimension(_firstOnstageChild, (child) => child.getMinIntrinsicHeight(width));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        return RenderStack.getIntrinsicDimension(_firstOnstageChild, (child) => child.getMaxIntrinsicHeight(width));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        Size sizeLocal = (!alwaysSizeToContent && constraints.biggest.isFinite) ? constraints.biggest : _findSizeDeterminingChild().getDryLayout(constraints);
        var nonPositionedChildConstraints = BoxConstraints.CreateTight(size);
        Alignment alignment = theater._resolvedAlignment;
        BaselineOffset baselineOffset = BaselineOffset.noBaseline;
        foreach (RenderBox child in _childrenInPaintOrder())
        {
            baselineOffset = baselineOffset.minOf(new BaselineOffset(_RenderTheaterMixin__overlay.baselineForChild(child, size, nonPositionedChildConstraints, alignment, baseline)));
        }
        return baselineOffset.offset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        if (!alwaysSizeToContent && constraints.biggest.isFinite)
        {
            return constraints.biggest;
        }
        return _findSizeDeterminingChild().getDryLayout(constraints);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual IEnumerable<RenderBox> _childrenInPaintOrder()
    {
        RenderBox? child = _firstOnstageChild;
        while (child is not null)
        {
            yield return child;
            var childParentData = ((_TheaterParentData__overlay?)child.parentData!)!;
            IEnumerator<RenderBox>? innerIterator = childParentData.paintOrderIterator;
            if (innerIterator is not null)
            {
                while (innerIterator.MoveNext())
                {
                    if (!KeptAliveSliverVisibility.IsHidden(innerIterator.Current)) yield return innerIterator.Current;
                }
            }
            child = childParentData.nextSibling;
        }
    }

    public virtual IEnumerable<RenderBox> _childrenInHitTestOrder()
    {
        RenderBox? child = _lastOnstageChild;
        long childLeft = childCount - skipCount;
        while (child is not null)
        {
            var childParentData = ((_TheaterParentData__overlay?)child.parentData!)!;
            IEnumerator<RenderBox>? innerIterator = childParentData.hitTestOrderIterator;
            if (innerIterator is not null)
            {
                while (innerIterator.MoveNext())
                {
                    if (!KeptAliveSliverVisibility.IsHidden(innerIterator.Current)) yield return innerIterator.Current;
                }
            }
            yield return child;
            childLeft -= 1L;
            child = (childLeft <= 0L) ? null : childParentData.previousSibling;
        }
    }

    public override bool sizedByParent => false;
    public override void performLayout()
    {
        RenderBox? sizeDeterminingChild = default!;
        if (!alwaysSizeToContent && constraints.biggest.isFinite)
        {
            size = constraints.biggest;
        }
        else
        {
            sizeDeterminingChild = _findSizeDeterminingChild();
            _layingOutSizeDeterminingChild = true;
            layoutChild(sizeDeterminingChild, constraints);
            _layingOutSizeDeterminingChild = false;
            size = sizeDeterminingChild.size;
        }
        var nonPositionedChildConstraints = BoxConstraints.CreateTight(size);
        foreach (RenderBox child in _childrenInPaintOrder())
        {
            if (!Equals(child, sizeDeterminingChild))
            {
                layoutChild(child, nonPositionedChildConstraints);
            }
        }
    }

    internal virtual RenderBox _findSizeDeterminingChild()
    {
        RenderBox? child = _lastOnstageChild;
        while (child is not null)
        {
            var childParentData = ((_TheaterParentData__overlay?)child.parentData!)!;
            if ((childParentData.overlayEntry?.canSizeOverlay ?? false) && !childParentData.isPositioned)
            {
                return child;
            }
            child = childParentData.previousSibling;
        }
        if (alwaysSizeToContent)
        {
            throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("Overlay was asked to size itself to content but does not have a suitable child."), new ErrorDescription("When `alwaysSizeToContent` is true, the Overlay requires at least one " + "non-positioned `OverlayEntry` with `canSizeOverlay` set to true to determine its size."), new ErrorHint("Try removing alwaysSizeToContent=true or provide a suitable child that can size the Overlay") }));
        }
        throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("Overlay was given infinite constraints and cannot be sized by a suitable child."), new ErrorDescription($"The constraints given to the overlay ({constraints}) would result in an illegal " + $"infinite size ({constraints.biggest}). To avoid that, the Overlay tried to size " + "itself to one of its children, but no suitable non-positioned child that belongs to an " + "OverlayEntry with canSizeOverlay set to true could be found."), new ErrorHint("Try wrapping the Overlay in a SizedBox to give it a finite size or " + "use an OverlayEntry with canSizeOverlay set to true.") }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        if (!Equals(clipBehavior, Clip.none))
        {
            _clipRectLayer.layer = context.pushClipRect(needsCompositing, offset, Offset.zero & size, (paintContext, paintOffset) =>
            {
                foreach (RenderBox child in _childrenInPaintOrder())
                {
                    var childParentData = ((StackParentData?)child.parentData!)!;
                    paintContext.paintChild(child, childParentData.offset + paintOffset);
                }
            }, clipBehavior: clipBehavior, oldLayer: _clipRectLayer.layer);
        }
        else
        {
            _clipRectLayer.layer = null;
            foreach (RenderBox childLocal in _childrenInPaintOrder())
            {
                var childParentDataLocal = ((StackParentData?)childLocal.parentData!)!;
                context.paintChild(childLocal, childParentDataLocal.offset + offset);
            }
        }
    }

    public override void dispose()
    {
        _clipRectLayer.layer = null;
        base.dispose();
    }

    public override void visitChildren(Action<RenderObject> visitor)
    {
        RenderBox? child = firstChild;
        while (child is not null)
        {
            visitor(child);
            var childParentData = ((_TheaterParentData__overlay?)child.parentData!)!;
            childParentData.visitOverlayPortalChildrenOnOverlayEntry(visitor);
            child = childParentData.nextSibling;
        }
    }

    public override void visitChildrenForSemantics(Action<RenderObject> visitor)
    {
        RenderBox? child = _firstOnstageChild;
        while (child is not null)
        {
            visitor(child);
            var childParentData = ((_TheaterParentData__overlay?)child.parentData!)!;
            childParentData.visitOverlayPortalChildrenOnOverlayEntry(visitor);
            child = childParentData.nextSibling;
        }
    }

    public override Rect? describeApproximatePaintClip(RenderObject child)
    {
        switch (clipBehavior)
        {
            case Clip.none:
                {
                    return null;
                }
            case Clip.hardEdge:
            case Clip.antiAlias:
            case Clip.antiAliasWithSaveLayer:
                {
                    return Offset.zero & size;
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new IntProperty("skipCount", skipCount));
        properties.add(new EnumProperty<TextDirection>("textDirection", textDirection));
    }

    public override List<DiagnosticsNode> debugDescribeChildren()
    {
        var offstageChildren = new List<DiagnosticsNode>();
        var onstageChildren = new List<DiagnosticsNode>();
        var count = 1L;
        var onstage = false;
        RenderBox? child = firstChild;
        RenderBox? firstOnstageChild = _firstOnstageChild;
        while (child is not null)
        {
            var childParentData = ((_TheaterParentData__overlay?)child.parentData!)!;
            if (Equals(child, firstOnstageChild))
            {
                onstage = true;
                count = 1L;
            }
            if (onstage)
            {
                onstageChildren.Add(((Diagnosticable)child).toDiagnosticsNode(name: $"onstage {count}"));
            }
            else
            {
                offstageChildren.Add(((Diagnosticable)child).toDiagnosticsNode(name: $"offstage {count}", style: DiagnosticsTreeStyle.offstage));
            }
            var subcount = 1L;
            childParentData.visitOverlayPortalChildrenOnOverlayEntry((renderObject) =>
            {
                var childLocal = ((RenderBox?)renderObject)!;
                if (onstage)
                {
                    onstageChildren.Add(((Diagnosticable)childLocal).toDiagnosticsNode(name: $"onstage {count} - {subcount}"));
                }
                else
                {
                    offstageChildren.Add(((Diagnosticable)childLocal).toDiagnosticsNode(name: $"offstage {count} - {subcount}", style: DiagnosticsTreeStyle.offstage));
                }
                subcount += 1L;
            });
            child = childParentData.nextSibling;
            count += 1L;
        }
        return new List<DiagnosticsNode>();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _debugUltimatePreviousSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData = ((StackParentData?)child.parentData!)!;
        while (childParentData.previousSibling is not null)
        {
            DartRuntimePrimitives.Assert(() => !Equals(childParentData.previousSibling, child));
            child = childParentData.previousSibling!;
            childParentData = ((StackParentData?)child.parentData!)!;
        }
        return Equals(child, equals);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _debugUltimateNextSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData = ((StackParentData?)child.parentData!)!;
        while (childParentData.nextSibling is not null)
        {
            DartRuntimePrimitives.Assert(() => !Equals(childParentData.nextSibling, child));
            child = childParentData.nextSibling!;
            childParentData = ((StackParentData?)child.parentData!)!;
        }
        return Equals(child, equals);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual long childCount => _childCount;
    public virtual bool debugValidateChild(RenderObject child)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (child is not RenderBox)
                {
                    throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"A {GetType()} expected a child of type {typeof(RenderBox)} but received a " + $"child of type {DartRuntimePrimitives.RuntimeType(child)}."), new ErrorDescription("RenderObjects expect specific types of children because they " + "coordinate with their children during layout and paint. For " + "example, a RenderSliver cannot be the child of a RenderBox because " + "a RenderSliver does not understand the RenderBox layout protocol."), new ErrorSpacer(), new DiagnosticsProperty<object?>($"The {GetType()} that expected a {typeof(RenderBox)} child was created by", debugCreator, style: DiagnosticsTreeStyle.errorProperty), new ErrorSpacer(), new DiagnosticsProperty<object?>($"The {DartRuntimePrimitives.RuntimeType(child)} that did not match the expected child type " + "was created by", child.debugCreator, style: DiagnosticsTreeStyle.errorProperty) }));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _insertIntoChildList(RenderBox child, RenderBox? after = null)
    {
        var childParentData = ((StackParentData?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => childParentData.nextSibling is null);
        DartRuntimePrimitives.Assert(() => childParentData.previousSibling is null);
        _childCount += 1L;
        DartRuntimePrimitives.Assert(() => _childCount > 0L);
        if (after is null)
        {
            childParentData.nextSibling = _firstChild;
            if (_firstChild is not null)
            {
                var firstChildParentData = ((StackParentData?)_firstChild!.parentData!)!;
                firstChildParentData.previousSibling = child;
            }
            _firstChild = child;
            _lastChild ??= child;
        }
        else
        {
            DartRuntimePrimitives.Assert(() => _firstChild is not null);
            DartRuntimePrimitives.Assert(() => _lastChild is not null);
            DartRuntimePrimitives.Assert(() => _debugUltimatePreviousSiblingOf(after, equals: _firstChild));
            DartRuntimePrimitives.Assert(() => _debugUltimateNextSiblingOf(after, equals: _lastChild));
            var afterParentData = ((StackParentData?)after.parentData!)!;
            if (afterParentData.nextSibling is null)
            {
                DartRuntimePrimitives.Assert(() => Equals(after, _lastChild));
                childParentData.previousSibling = after;
                afterParentData.nextSibling = child;
                _lastChild = child;
            }
            else
            {
                childParentData.nextSibling = afterParentData.nextSibling;
                childParentData.previousSibling = after;
                var childPreviousSiblingParentData = ((StackParentData?)childParentData.previousSibling!.parentData!)!;
                var childNextSiblingParentData = ((StackParentData?)childParentData.nextSibling!.parentData!)!;
                childPreviousSiblingParentData.nextSibling = child;
                childNextSiblingParentData.previousSibling = child;
                DartRuntimePrimitives.Assert(() => Equals(afterParentData.nextSibling, child));
            }
        }
    }

    public virtual void insert(RenderBox child, RenderBox? after = null)
    {
        DartRuntimePrimitives.Assert(() => !Equals(child, this), () => (object?)"A RenderObject cannot be inserted into itself.");
        DartRuntimePrimitives.Assert(() => !Equals(after, this), () => (object?)"A RenderObject cannot simultaneously be both the parent and the sibling of another RenderObject.");
        DartRuntimePrimitives.Assert(() => !Equals(child, after), () => (object?)"A RenderObject cannot be inserted after itself.");
        DartRuntimePrimitives.Assert(() => !Equals(child, _firstChild));
        DartRuntimePrimitives.Assert(() => !Equals(child, _lastChild));
        adoptChild(child);
        DartRuntimePrimitives.Assert(() => child.parentData is StackParentData, () => (object?)$"A child of {GetType()} has parentData of type {DartRuntimePrimitives.RuntimeType(child.parentData)}, " + $"which does not conform to {typeof(StackParentData)}. Class using ContainerRenderObjectMixin " + $"should override setupParentData() to set parentData to type {typeof(StackParentData)}.");
        _insertIntoChildList(child, after: after);
    }

    public virtual void add(RenderBox child)
    {
        insert(child, after: _lastChild);
    }

    public virtual void addAll(List<RenderBox>? children)
    {
        children?.forEach((__arg0) => ((Action<RenderBox>)add)(__arg0));
    }

    public virtual void _removeFromChildList(RenderBox child)
    {
        var childParentData = ((StackParentData?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => _debugUltimatePreviousSiblingOf(child, equals: _firstChild));
        DartRuntimePrimitives.Assert(() => _debugUltimateNextSiblingOf(child, equals: _lastChild));
        DartRuntimePrimitives.Assert(() => _childCount >= 0L);
        if (childParentData.previousSibling is null)
        {
            DartRuntimePrimitives.Assert(() => Equals(_firstChild, child));
            _firstChild = childParentData.nextSibling;
        }
        else
        {
            var childPreviousSiblingParentData = ((StackParentData?)childParentData.previousSibling!.parentData!)!;
            childPreviousSiblingParentData.nextSibling = childParentData.nextSibling;
        }
        if (childParentData.nextSibling is null)
        {
            DartRuntimePrimitives.Assert(() => Equals(_lastChild, child));
            _lastChild = childParentData.previousSibling;
        }
        else
        {
            var childNextSiblingParentData = ((StackParentData?)childParentData.nextSibling!.parentData!)!;
            childNextSiblingParentData.previousSibling = childParentData.previousSibling;
        }
        childParentData.previousSibling = null;
        childParentData.nextSibling = null;
        _childCount -= 1L;
    }

    public virtual void remove(RenderBox child)
    {
        _removeFromChildList(child);
        dropChild(child);
    }

    public virtual void removeAll()
    {
        RenderBox? child = _firstChild;
        while (child is not null)
        {
            var childParentData = ((StackParentData?)child.parentData!)!;
            RenderBox? next = childParentData.nextSibling;
            childParentData.previousSibling = null;
            childParentData.nextSibling = null;
            dropChild(child);
            child = next;
        }
        _firstChild = null;
        _lastChild = null;
        _childCount = 0L;
    }

    public virtual void move(RenderBox child, RenderBox? after = null)
    {
        DartRuntimePrimitives.Assert(() => !Equals(child, this));
        DartRuntimePrimitives.Assert(() => !Equals(after, this));
        DartRuntimePrimitives.Assert(() => !Equals(child, after));
        DartRuntimePrimitives.Assert(() => Equals(child.parent, this));
        var childParentData = ((StackParentData?)child.parentData!)!;
        if (Equals(childParentData.previousSibling, after))
        {
            return;
        }
        _removeFromChildList(child);
        _insertIntoChildList(child, after: after);
        markNeedsLayout();
    }

    public virtual RenderBox? firstChild => _firstChild;
    public virtual RenderBox? lastChild => _lastChild;
    public virtual RenderBox? childBefore(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => Equals(child.parent, this));
        var childParentData = ((StackParentData?)child.parentData!)!;
        return childParentData.previousSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RenderBox? childAfter(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => Equals(child.parent, this));
        var childParentData = ((StackParentData?)child.parentData!)!;
        return childParentData.nextSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDistanceToActualBaseline(TextBaseline baseline)
    {
        DartRuntimePrimitives.Assert(() => !debugNeedsLayout);
        BaselineOffset baselineOffset = BaselineOffset.noBaseline;
        foreach (RenderBox child in _childrenInPaintOrder())
        {
            DartRuntimePrimitives.Assert(() => !child.debugNeedsLayout);
            var childParentData = ((StackParentData?)child.parentData!)!;
            baselineOffset = baselineOffset.minOf(new BaselineOffset(child.getDistanceToActualBaseline(baseline)).op_Add(childParentData.offset.dy));
        }
        return baselineOffset.offset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void layoutChild(RenderBox child, BoxConstraints nonPositionedChildConstraints)
    {
        var childParentData = ((StackParentData?)child.parentData!)!;
        Alignment alignment = theater._resolvedAlignment;
        if (!childParentData.isPositioned)
        {
            child.layout(nonPositionedChildConstraints, parentUsesSize: true);
            childParentData.offset = Offset.zero;
        }
        else
        {
            DartRuntimePrimitives.Assert(() => child is not _RenderDeferredLayoutBox__overlay, () => (object?)"all _RenderDeferredLayoutBoxes must be non-positioned children.");
            RenderStack.layoutPositionedChild(child, childParentData, size, alignment);
        }
        DartRuntimePrimitives.Assert(() => Equals(child.parentData, childParentData));
    }

    public override bool hitTestChildren(BoxHitTestResult result, Offset position)
    {
        IEnumerator<RenderBox> iterator = _childrenInHitTestOrder().GetEnumerator();
        var isHit = false;
        while (!isHit && iterator.MoveNext())
        {
            RenderBox child = iterator.Current;
            var childParentData = ((StackParentData?)child.parentData!)!;
            var localChild = child;
            bool childHitTest(BoxHitTestResult result, Offset position)
            {
                return localChild.hitTest(result, position: position);
                throw new InvalidOperationException("Dart control flow completed without a value.");
            }
            isHit = result.addWithPaintOffset(offset: childParentData.offset, position: position, hitTest: childHitTest);
        }
        return isHit;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class OverlayPortalController
{
    internal virtual _OverlayPortalState__overlay? _attachTarget { get; set; } = default;
    internal virtual long? _zOrderIndex { get; set; } = default;
    internal virtual string? _debugLabel { get; private set; }
    internal static long _wallTime = Foundation.ConstantsLibrary.kIsWeb ? -9007199254740992L : (-1L << (int)63L);

    public OverlayPortalController(string? debugLabel = null)
    {
        _debugLabel = debugLabel;
    }

    internal virtual long _now()
    {
        long now = _wallTime += 1L;
        DartRuntimePrimitives.Assert(() => (_zOrderIndex is null) || (DartRuntimePrimitives.RequireValue(_zOrderIndex) < now));
        DartRuntimePrimitives.Assert(() => (_attachTarget?._zOrderIndex is null) || (DartRuntimePrimitives.RequireValue(_attachTarget!._zOrderIndex) < now));
        return now;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void show()
    {
        _OverlayPortalState__overlay? state = _attachTarget;
        if (state is not null)
        {
            state.show(_now());
        }
        else
        {
            _zOrderIndex = _now();
        }
    }

    public virtual void hide()
    {
        _OverlayPortalState__overlay? state = _attachTarget;
        if (state is not null)
        {
            state.hide();
        }
        else
        {
            DartRuntimePrimitives.Assert(() => _zOrderIndex is not null);
            _zOrderIndex = null;
        }
    }

    public virtual bool isShowing
    {
        get
        {
            _OverlayPortalState__overlay? state = _attachTarget;
            return (state is not null) ? (state._zOrderIndex is not null) : (_zOrderIndex is not null);
        }
    }
    public virtual void toggle() => ((Action)(() => { if (isShowing) { hide(); } else { show(); } }))();
    public override string ToString()
    {
        string? debugLabel = _debugLabel;
        var label = (debugLabel is null) ? "" : $"({debugLabel})";
        var isDetached = (_attachTarget is not null) ? "" : " DETACHED";
        return $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "OverlayPortalController")}{label}{isDetached}";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public enum OverlayChildLocation
{
    nearestOverlay,
    rootOverlay
}

public class OverlayPortal : StatefulWidget
{
    public virtual OverlayPortalController controller { get; private set; } = default!;
    public virtual Func<BuildContext, Widget> overlayChildBuilder { get; private set; } = default!;
    public virtual Widget? child { get; private set; }
    public virtual OverlayChildLocation overlayLocation { get; private set; } = default!;

    public OverlayPortal(Key? key = null, OverlayPortalController controller = default!, Func<BuildContext, Widget> overlayChildBuilder = default!, OverlayChildLocation overlayLocation = OverlayChildLocation.nearestOverlay, Widget? child = null) : base(key: key)
    {
        this.controller = controller;
        this.overlayChildBuilder = overlayChildBuilder;
        this.overlayLocation = overlayLocation;
        this.child = child;
    }

    public static OverlayPortal CreateTargetsRootOverlay(Key? key = null, OverlayPortalController controller = default!, Func<BuildContext, Widget> overlayChildBuilder = default!, Widget? child = null)
    {
        var __instance = new OverlayPortal(key, controller, overlayChildBuilder, default!, child);
        __instance.controller = controller;
        __instance.overlayChildBuilder = overlayChildBuilder;
        __instance.child = child;
        __instance.overlayLocation = OverlayChildLocation.rootOverlay;
        return __instance;
    }

    public static OverlayPortal CreateOverlayChildLayoutBuilder(Key? key = null, OverlayPortalController controller = default!, Func<BuildContext, OverlayChildLayoutInfo, Widget> overlayChildBuilder = default!, OverlayChildLocation overlayLocation = OverlayChildLocation.nearestOverlay, Widget? child = default!)
    {
        return new OverlayPortal(key: key, controller: controller, overlayChildBuilder: (_) => new _OverlayChildLayoutBuilder__overlay(builder: overlayChildBuilder), child: child, overlayLocation: overlayLocation);
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _OverlayPortalState__overlay());
}

internal class _OverlayPortalState__overlay : State<OverlayPortal>
{
    internal virtual long? _zOrderIndex { get; set; } = default;
    internal virtual bool _childModelMayHaveChanged { get; set; } = true;
    internal virtual _OverlayEntryLocation__overlay? _locationCache { get; set; } = default;

    internal static bool _isTheSameLocation(_OverlayEntryLocation__overlay locationCache, _RenderTheaterMarker__overlay marker)
    {
        return Equals(locationCache._childModel, marker.overlayEntryWidgetState) && Equals(locationCache._theater, marker.theater);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual _OverlayEntryLocation__overlay _getLocation(long zOrderIndex, OverlayChildLocation overlayLocation)
    {
        _OverlayEntryLocation__overlay? cachedLocation = _locationCache;
        _RenderTheaterMarker__overlay marker = _RenderTheaterMarker__overlay.of(context, targetRootOverlay: Equals(overlayLocation, OverlayChildLocation.rootOverlay));
        bool isCacheValid = (cachedLocation is not null) && (!_childModelMayHaveChanged || _isTheSameLocation(cachedLocation, marker));
        _childModelMayHaveChanged = false;
        if (isCacheValid && cachedLocation is not null)
        {
            DartRuntimePrimitives.Assert(() => cachedLocation._zOrderIndex == zOrderIndex);
            DartRuntimePrimitives.Assert(() => cachedLocation._debugIsLocationValid());
            return cachedLocation;
        }
        cachedLocation?._debugMarkLocationInvalid();
        var newLocation = new _OverlayEntryLocation__overlay(zOrderIndex, marker.overlayEntryWidgetState, marker.theater);
        DartRuntimePrimitives.Assert(() => newLocation._zOrderIndex == zOrderIndex);
        return _locationCache = newLocation;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void initState()
    {
        base.initState();
        _setupController(widget.controller);
    }

    internal virtual void _setupController(OverlayPortalController controller)
    {
        DartRuntimePrimitives.Assert(() => Equals(controller._attachTarget, this) || !(((StatefulElement?)controller._attachTarget?.context)!?.debugIsActive ?? false), () => (object?)$"Failed to attach {controller} to {this}. It is already attached to {controller._attachTarget}.");
        long? controllerZOrderIndex = controller._zOrderIndex;
        long? zOrderIndex = _zOrderIndex;
        if ((zOrderIndex is null) || (controllerZOrderIndex is not null) && (DartRuntimePrimitives.RequireValue(controllerZOrderIndex) > DartRuntimePrimitives.RequireValue(zOrderIndex)))
        {
            _zOrderIndex = controllerZOrderIndex;
        }
        controller._zOrderIndex = null;
        controller._attachTarget = this;
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        _childModelMayHaveChanged = true;
    }

    public override void didUpdateWidget(OverlayPortal oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        _childModelMayHaveChanged = _childModelMayHaveChanged || (!Equals(oldWidget.overlayLocation, widget.overlayLocation));
        if (!Equals(oldWidget.controller, widget.controller))
        {
            oldWidget.controller._attachTarget = null;
            _setupController(widget.controller);
        }
    }

    public override void activate()
    {
        DartRuntimePrimitives.Assert(() => Equals(widget.controller._attachTarget, this));
        base.activate();
    }

    public override void dispose()
    {
        widget.controller._attachTarget = null;
        _locationCache?._debugMarkLocationInvalid();
        _locationCache = null;
        base.dispose();
    }

    public virtual void show(long zOrderIndex)
    {
        DartRuntimePrimitives.Assert(() => !Equals(Scheduler.SchedulerBinding.instance.schedulerPhase, Scheduler.SchedulerPhase.persistentCallbacks), () => (object?)$"{DartRuntimePrimitives.RuntimeType(widget.controller)}.show() should not be called during build.");
        setState(() =>
        {
            _zOrderIndex = zOrderIndex;
        });
        _locationCache?._debugMarkLocationInvalid();
        _locationCache = null;
    }

    public virtual void hide()
    {
        DartRuntimePrimitives.Assert(() => !Equals(Scheduler.SchedulerBinding.instance.schedulerPhase, Scheduler.SchedulerPhase.persistentCallbacks));
        setState(() =>
        {
            _zOrderIndex = null;
        });
        _locationCache?._debugMarkLocationInvalid();
        _locationCache = null;
    }

    public override Widget build(BuildContext context)
    {
        long? zOrderIndex = _zOrderIndex;
        if (zOrderIndex is null)
        {
            return new _OverlayPortal__overlay(overlayLocation: null, overlayChild: null, child: new Semantics(traversalParentIdentifier: this, child: widget.child));
        }
        _OverlayEntryLocation__overlay overlayLocationLocal = _getLocation(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(zOrderIndex)), widget.overlayLocation);
        MediaQueryData overlayData = MediaQuery.of(overlayLocationLocal._childModel.context);
        MediaQueryData dataLocal = MediaQuery.of(context).copyWith(padding: overlayData.padding, viewInsets: overlayData.viewInsets, viewPadding: overlayData.viewPadding);
        return new _OverlayPortal__overlay(overlayLocation: overlayLocationLocal, overlayChild: new _DeferredLayout__overlay(childIdentifier: this, child: new MediaQuery(data: dataLocal, child: new Builder(builder: widget.overlayChildBuilder))), child: new Semantics(traversalParentIdentifier: this, child: widget.child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _OverlayEntryLocation__overlay : DartLinkedListEntry<_OverlayEntryLocation__overlay>
{
    internal virtual long _zOrderIndex { get; private set; } = default!;
    internal virtual _OverlayEntryWidgetState__overlay _childModel { get; private set; } = default!;
    internal virtual _RenderTheater__overlay _theater { get; private set; } = default!;
    internal virtual _RenderDeferredLayoutBox__overlay? _overlayChildRenderBox { get; set; } = default;
    internal virtual System.Diagnostics.StackTrace? _debugMarkLocationInvalidStackTrace { get; set; } = default;

    internal _OverlayEntryLocation__overlay(long _zOrderIndex, _OverlayEntryWidgetState__overlay _childModel, _RenderTheater__overlay _theater)
    {
        this._zOrderIndex = _zOrderIndex;
        this._childModel = _childModel;
        this._theater = _theater;
    }

    internal virtual void _addToChildModel(_RenderDeferredLayoutBox__overlay child)
    {
        DartRuntimePrimitives.Assert(() => _overlayChildRenderBox is null, () => (object?)$"Failed to add {child}. This location ({this}) is already occupied by {_overlayChildRenderBox}.");
        _overlayChildRenderBox = child;
        _childModel._add(this);
        _theater.markNeedsPaint();
        _theater.markNeedsCompositingBitsUpdate();
        _theater.markNeedsSemanticsUpdate();
    }

    internal virtual void _removeFromChildModel(_RenderDeferredLayoutBox__overlay child)
    {
        DartRuntimePrimitives.Assert(() => Equals(child, _overlayChildRenderBox));
        _overlayChildRenderBox = null;
        DartRuntimePrimitives.Assert(() => _childModel._sortedTheaterSiblings?.contains(this) ?? false);
        _childModel._remove(this);
        _theater.markNeedsPaint();
        _theater.markNeedsCompositingBitsUpdate();
        _theater.markNeedsSemanticsUpdate();
    }

    internal virtual void _addChild(_RenderDeferredLayoutBox__overlay child)
    {
        DartRuntimePrimitives.Assert(() => _debugIsLocationValid());
        _addToChildModel(child);
        _theater._addDeferredChild(child);
        DartRuntimePrimitives.Assert(() => Equals(child.parent, _theater));
    }

    internal virtual void _removeChild(_RenderDeferredLayoutBox__overlay child)
    {
        _removeFromChildModel(child);
        _theater._removeDeferredChild(child);
        DartRuntimePrimitives.Assert(() => child.parent is null);
    }

    internal virtual void _moveChild(_RenderDeferredLayoutBox__overlay child, _OverlayEntryLocation__overlay fromLocation)
    {
        DartRuntimePrimitives.Assert(() => !Equals(fromLocation, this));
        DartRuntimePrimitives.Assert(() => _debugIsLocationValid());
        _RenderTheater__overlay fromTheater = fromLocation._theater;
        _OverlayEntryWidgetState__overlay fromModel = fromLocation._childModel;
        if (!Equals(fromTheater, _theater))
        {
            fromTheater._removeDeferredChild(child);
            _theater._addDeferredChild(child);
        }
        if ((!Equals(fromModel, _childModel)) || (fromLocation._zOrderIndex != _zOrderIndex))
        {
            fromLocation._removeFromChildModel(child);
            _addToChildModel(child);
        }
    }

    internal virtual void _reattachFromLayoutSurrogate(_RenderDeferredLayoutBox__overlay child)
    {
        DartRuntimePrimitives.Assert(() => _overlayChildRenderBox is null, () => (object?)$"{this} failed to reattach: _detachFromLayoutSurrogate must be called before _reattachFromLayoutSurrogate.");
        _theater._addDeferredChild(child);
        _overlayChildRenderBox = child;
    }

    internal virtual void _detachFromLayoutSurrogate(_RenderDeferredLayoutBox__overlay child)
    {
        _theater._removeDeferredChild(child);
        _overlayChildRenderBox = null;
    }

    internal virtual bool _debugIsLocationValid()
    {
        if (_debugMarkLocationInvalidStackTrace is null)
        {
            return true;
        }
        throw new InvalidOperationException($"{this} is already disposed. Stack trace: {_debugMarkLocationInvalidStackTrace}");
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _debugMarkLocationInvalid()
    {
        DartRuntimePrimitives.Assert(() => _debugIsLocationValid());
        DartRuntimePrimitives.Assert(() =>
            {
                _debugMarkLocationInvalidStackTrace = new System.Diagnostics.StackTrace(true);
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
    }

    public override string ToString() => $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "_OverlayEntryLocation")}[{DiagnosticsLibrary.shortHash(this)}] {((_debugMarkLocationInvalidStackTrace is not null) ? "(INVALID)" : "")}";
}

internal class _RenderTheaterMarker__overlay : InheritedWidget
{
    public virtual _RenderTheater__overlay theater { get; private set; } = default!;
    public virtual _OverlayEntryWidgetState__overlay overlayEntryWidgetState { get; private set; } = default!;

    internal _RenderTheaterMarker__overlay(_RenderTheater__overlay theater, _OverlayEntryWidgetState__overlay overlayEntryWidgetState, Widget child) : base(child: child)
    {
        this.theater = theater;
        this.overlayEntryWidgetState = overlayEntryWidgetState;
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget)
    {
        var __oldWidget = (_RenderTheaterMarker__overlay)oldWidget;
        return (!Equals(__oldWidget.theater, theater)) || (!Equals(__oldWidget.overlayEntryWidgetState, overlayEntryWidgetState));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static _RenderTheaterMarker__overlay of(BuildContext context, bool targetRootOverlay = false)
    {
        _RenderTheaterMarker__overlay? marker = maybeOf(context, targetRootOverlay: targetRootOverlay);
        if (marker is not null)
        {
            return marker;
        }
        throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("No Overlay widget found."), new ErrorDescription($"{DartRuntimePrimitives.RuntimeType(context.widget)} widgets require an Overlay widget ancestor.\n" + "An overlay lets widgets float on top of other widget children."), new ErrorHint("To introduce an Overlay widget, you can either directly " + "include one, or use a widget that contains an Overlay itself, " + "such as a Navigator, WidgetApp, MaterialApp, or CupertinoApp.") }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static _RenderTheaterMarker__overlay? maybeOf(BuildContext context, bool targetRootOverlay = false, bool createDependency = true)
    {
        if (targetRootOverlay)
        {
            InheritedElement? ancestor = _rootRenderTheaterMarkerOf(LookupBoundary.getElementForInheritedWidgetOfExactType<_RenderTheaterMarker__overlay>(context));
            DartRuntimePrimitives.Assert(() => (ancestor is null) || (ancestor.widget is _RenderTheaterMarker__overlay));
            if (ancestor is null)
            {
                return null;
            }
            if (createDependency)
            {
                return ((_RenderTheaterMarker__overlay?)context.dependOnInheritedElement(ancestor))!;
            }
            return ((_RenderTheaterMarker__overlay?)ancestor.widget)!;
        }
        if (createDependency)
        {
            return LookupBoundary.dependOnInheritedWidgetOfExactType<_RenderTheaterMarker__overlay>(context);
        }
        return LookupBoundary.getInheritedWidgetOfExactType<_RenderTheaterMarker__overlay>(context);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static InheritedElement? _rootRenderTheaterMarkerOf(InheritedElement? theaterMarkerElement)
    {
        DartRuntimePrimitives.Assert(() => (theaterMarkerElement is null) || (theaterMarkerElement.widget is _RenderTheaterMarker__overlay));
        if (theaterMarkerElement is null)
        {
            return null;
        }
        InheritedElement? ancestor = default!;
        theaterMarkerElement.visitAncestorElements((element) =>
        {
            ancestor = LookupBoundary.getElementForInheritedWidgetOfExactType<_RenderTheaterMarker__overlay>(element);
            return false;
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        return (ancestor is null) ? theaterMarkerElement : _rootRenderTheaterMarkerOf(ancestor);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _OverlayPortal__overlay : RenderObjectWidget
{
    public virtual Widget? overlayChild { get; private set; }
    public virtual Widget? child { get; private set; }
    public virtual _OverlayEntryLocation__overlay? overlayLocation { get; private set; }

    internal _OverlayPortal__overlay(_OverlayEntryLocation__overlay? overlayLocation, Widget? overlayChild, Widget? child)
    {
        this.overlayLocation = overlayLocation;
        this.overlayChild = overlayChild;
        this.child = child;
        System.Diagnostics.Debug.Assert((overlayChild is null) || (overlayLocation is not null));
        System.Diagnostics.Debug.Assert((overlayLocation is null) || overlayLocation._debugIsLocationValid());
    }

    public override RenderObjectElement createElement() => DartRuntimePrimitives.ConvertValue<RenderObjectElement>(new _OverlayPortalElement__overlay(this));
    public override RenderObject createRenderObject(BuildContext context) => DartRuntimePrimitives.ConvertValue<RenderObject>(new _RenderLayoutSurrogateProxyBox__overlay(overlayLocation));
    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (_RenderLayoutSurrogateProxyBox__overlay)renderObject;
        __renderObject.overlayLocation = overlayLocation;
    }

}

internal class _OverlayPortalElement__overlay : RenderObjectElement
{
    internal virtual Element? _overlayChild { get; set; } = default;
    internal virtual Element? _child { get; set; } = default;

    internal _OverlayPortalElement__overlay(_OverlayPortal__overlay widget) : base(widget)
    {
    }

    public override _RenderLayoutSurrogateProxyBox__overlay renderObject => (_RenderLayoutSurrogateProxyBox__overlay)base.renderObject;
    public override void mount(Element? parent, object? newSlot)
    {
        base.mount(parent, newSlot);
        var widgetLocal = ((_OverlayPortal__overlay?)widget)!;
        _child = updateChild(_child, widgetLocal.child, null);
        _overlayChild = updateChild(_overlayChild, widgetLocal.overlayChild, widgetLocal.overlayLocation);
    }

    public override void update(Widget newWidget)
    {
        var __newWidget = (_OverlayPortal__overlay)newWidget;
        base.update(__newWidget);
        _child = updateChild(_child, __newWidget.child, null);
        _overlayChild = updateChild(_overlayChild, __newWidget.overlayChild, __newWidget.overlayLocation);
    }

    public override void forgetChild(Element child)
    {
        DartRuntimePrimitives.Assert(() => Equals(child, _child));
        _child = null;
        base.forgetChild(child);
    }

    public override void visitChildren(Action<Element> visitor)
    {
        Element? child = _child;
        Element? overlayChild = _overlayChild;
        if (child is not null)
        {
            visitor(child);
        }
        if (overlayChild is not null)
        {
            visitor(overlayChild);
        }
    }

    public override void insertRenderObjectChild(RenderObject child, object? slot)
    {
        var __child = (RenderBox)child;
        var __slot = slot is null ? null : (_OverlayEntryLocation__overlay)slot;
        DartRuntimePrimitives.Assert(() => __child.parent is null, () => (object?)$"{__child}'s parent is not null: {__child.parent}");
        if (__slot is not null)
        {
            DartRuntimePrimitives.Assert(() => Equals(renderObject._deferredLayoutChild, __child));
            __slot._addChild(((_RenderDeferredLayoutBox__overlay?)__child)!);
            renderObject.markNeedsSemanticsUpdate();
        }
        else
        {
            renderObject.child = __child;
        }
    }

    public override void moveRenderObjectChild(RenderObject child, object? oldSlot, object? newSlot)
    {
        var __child = (_RenderDeferredLayoutBox__overlay)child;
        var __oldSlot = oldSlot as _OverlayEntryLocation__overlay ?? throw new ArgumentException("An overlay child requires a location.", nameof(oldSlot));
        var __newSlot = newSlot as _OverlayEntryLocation__overlay ?? throw new ArgumentException("An overlay child requires a location.", nameof(newSlot));
        DartRuntimePrimitives.Assert(() => __newSlot._debugIsLocationValid());
        __newSlot._moveChild(__child, __oldSlot);
        renderObject.markNeedsSemanticsUpdate();
    }

    public override void removeRenderObjectChild(RenderObject child, object? slot)
    {
        var __child = (RenderBox)child;
        var __slot = slot is null ? null : (_OverlayEntryLocation__overlay)slot;
        if (__slot is null)
        {
            renderObject.child = null;
            return;
        }
        DartRuntimePrimitives.Assert(() => Equals(renderObject._deferredLayoutChild, __child));
        __slot._removeChild(((_RenderDeferredLayoutBox__overlay?)__child)!);
        renderObject._deferredLayoutChild = null;
        renderObject.markNeedsSemanticsUpdate();
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<Element>("child", _child, defaultValue: null));
        properties.add(new DiagnosticsProperty<Element>("overlayChild", _overlayChild, defaultValue: null));
        properties.add(new DiagnosticsProperty<object>("overlayLocation", _overlayChild?.slot, defaultValue: null));
    }

}

internal class _DeferredLayout__overlay : SingleChildRenderObjectWidget
{
    public virtual object? childIdentifier { get; private set; }

    internal _DeferredLayout__overlay(Widget child, object? childIdentifier = null) : base(child: child)
    {
        this.childIdentifier = childIdentifier;
    }

    public virtual _RenderLayoutSurrogateProxyBox__overlay getLayoutParent(BuildContext context)
    {
        return context.findAncestorRenderObjectOfType<_RenderLayoutSurrogateProxyBox__overlay>()!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        _RenderLayoutSurrogateProxyBox__overlay parent = getLayoutParent(context);
        var renderObject = new _RenderDeferredLayoutBox__overlay(parent, childIdentifier);
        parent._deferredLayoutChild = renderObject;
        return renderObject;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (_RenderDeferredLayoutBox__overlay)renderObject;
        DartRuntimePrimitives.Assert(() => Equals(__renderObject._layoutSurrogate, getLayoutParent(context)));
        DartRuntimePrimitives.Assert(() => Equals(getLayoutParent(context)._deferredLayoutChild, __renderObject));
        __renderObject.childIdentifier = childIdentifier;
    }

}

public class _RenderDeferredLayoutBox__overlay : RenderProxyBox, _RenderTheaterMixin__overlay
{
    internal virtual _RenderLayoutSurrogateProxyBox__overlay _layoutSurrogate { get; private set; } = default!;
    internal virtual object? _childIdentifier { get; set; } = default;
    // Dart library-private member: distinct from the same name in the base library.
    internal new virtual bool _needsLayout { get; set; } = true;
    internal virtual bool _doingLayoutFromTreeWalk { get; set; } = false;
    // Dart library-private member: distinct from the same name in the base library.
    internal new virtual bool _debugMutationsLocked { get; set; } = false;

    internal _RenderDeferredLayoutBox__overlay(_RenderLayoutSurrogateProxyBox__overlay _layoutSurrogate, object? childIdentifier)
    {
        this._layoutSurrogate = _layoutSurrogate;
        _childIdentifier = childIdentifier;
    }

    public virtual StackParentData stackParentData => ((StackParentData?)parentData!)!;
    public virtual object? childIdentifier
    {
        get => _childIdentifier;
        set
        {
            var __value = value;
            if (Equals(_childIdentifier, __value))
            {
                return;
            }
            _childIdentifier = __value;
        }
    }
    public virtual IEnumerable<RenderBox> _childrenInPaintOrder()
    {
        RenderBox? childLocal = child;
        return (childLocal is null) ? Enumerable.Empty<RenderBox>() : Enumerable.Range(0, checked((int)1L)).Select(__index => ((Func<long, RenderBox>)((i) => childLocal))(checked(__index)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual IEnumerable<RenderBox> _childrenInHitTestOrder() => _childrenInPaintOrder();
    public virtual _RenderTheater__overlay theater => parent switch { _RenderTheater__overlay parentLocal => parentLocal, _ => throw DartRuntimePrimitives.AsException(FlutterError.Create($"{parent} of {this} is not a _RenderTheater")) };
    public override void redepthChildren()
    {
        if (_layoutSurrogate.attached)
        {
            _layoutSurrogate.redepthChild(this);
        }
        base.redepthChildren();
    }

    public override bool sizedByParent => true;
    public virtual bool needsLayout
    {
        get
        {
            DartRuntimePrimitives.Assert(() => debugNeedsLayout == _needsLayout);
            return _needsLayout;
        }
    }
    public override void markNeedsLayout()
    {
        _needsLayout = true;
        base.markNeedsLayout();
    }

    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        RenderBox? childLocal = child;
        if (childLocal is null)
        {
            return null;
        }
        return _RenderTheaterMixin__overlay.baselineForChild(childLocal, constraints.biggest, constraints, theater._resolvedAlignment, baseline);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override RenderObject? debugLayoutParent => DartRuntimePrimitives.ConvertValue<RenderObject>(_layoutSurrogate);
    internal virtual void _doLayoutFrom(RenderObject treewalkParent, Constraints constraints)
    {
        bool shouldAddToDirtyList = needsLayout || (!Equals(this.constraints, constraints));
        DartRuntimePrimitives.Assert(() => !_doingLayoutFromTreeWalk);
        _doingLayoutFromTreeWalk = true;
        base.layout(constraints);
        DartRuntimePrimitives.Assert(() => _doingLayoutFromTreeWalk);
        _doingLayoutFromTreeWalk = false;
        _needsLayout = false;
        DartRuntimePrimitives.Assert(() => !debugNeedsLayout);
        if (shouldAddToDirtyList)
        {
            treewalkParent.invokeLayoutCallback((Action<BoxConstraints>)((_) =>
            {
                markNeedsLayout();
            }));
        }
    }

    public override void layout(Constraints constraints, bool parentUsesSize = false)
    {
        _doLayoutFrom(parent!, constraints: constraints);
    }

    public override void performResize()
    {
        size = constraints.biggest;
    }

    public override void performLayout()
    {
        DartRuntimePrimitives.Assert(() => !_debugMutationsLocked);
        if (_doingLayoutFromTreeWalk)
        {
            _needsLayout = false;
            return;
        }
        DartRuntimePrimitives.Assert(() =>
            {
                _debugMutationsLocked = true;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        DartRuntimePrimitives.Assert(() => parent is not null);
        RenderBox? childLocal = child;
        if (childLocal is null)
        {
            _needsLayout = false;
            return;
        }
        DartRuntimePrimitives.Assert(() => constraints.isTight);
        layoutChild(childLocal, constraints);
        DartRuntimePrimitives.Assert(() =>
            {
                _debugMutationsLocked = false;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        _needsLayout = false;
    }

    public override void describeSemanticsConfiguration(SemanticsConfiguration config)
    {
        base.describeSemanticsConfiguration(config);
        if (childIdentifier is not null)
        {
            config.traversalChildIdentifier = childIdentifier;
        }
    }

    public override void applyPaintTransform(RenderObject child, Matrix4 transform)
    {
        var __child = (RenderBox)child;
        var childParentData = ((BoxParentData?)__child.parentData!)!;
        Offset offsetLocal = childParentData.offset;
        transform.translateByDouble(offsetLocal.dx, offsetLocal.dy, 0, 1);
    }

    public override void setupParentData(RenderObject child)
    {
        var __child = (RenderBox)child;
        if (__child.parentData is not StackParentData)
        {
            __child.parentData = new StackParentData();
        }
    }

    public override double? computeDistanceToActualBaseline(TextBaseline baseline)
    {
        DartRuntimePrimitives.Assert(() => !debugNeedsLayout);
        BaselineOffset baselineOffset = BaselineOffset.noBaseline;
        foreach (RenderBox child in _childrenInPaintOrder())
        {
            DartRuntimePrimitives.Assert(() => !child.debugNeedsLayout);
            var childParentData = ((StackParentData?)child.parentData!)!;
            baselineOffset = baselineOffset.minOf(new BaselineOffset(child.getDistanceToActualBaseline(baseline)).op_Add(childParentData.offset.dy));
        }
        return baselineOffset.offset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void layoutChild(RenderBox child, BoxConstraints nonPositionedChildConstraints)
    {
        var childParentData = ((StackParentData?)child.parentData!)!;
        Alignment alignment = theater._resolvedAlignment;
        if (!childParentData.isPositioned)
        {
            child.layout(nonPositionedChildConstraints, parentUsesSize: true);
            childParentData.offset = Offset.zero;
        }
        else
        {
            DartRuntimePrimitives.Assert(() => child is not _RenderDeferredLayoutBox__overlay, () => (object?)"all _RenderDeferredLayoutBoxes must be non-positioned children.");
            RenderStack.layoutPositionedChild(child, childParentData, size, alignment);
        }
        DartRuntimePrimitives.Assert(() => Equals(child.parentData, childParentData));
    }

    public override bool hitTestChildren(BoxHitTestResult result, Offset position)
    {
        IEnumerator<RenderBox> iterator = _childrenInHitTestOrder().GetEnumerator();
        var isHit = false;
        while (!isHit && iterator.MoveNext())
        {
            RenderBox child = iterator.Current;
            var childParentData = ((StackParentData?)child.parentData!)!;
            var localChild = child;
            bool childHitTest(BoxHitTestResult result, Offset position)
            {
                return localChild.hitTest(result, position: position);
                throw new InvalidOperationException("Dart control flow completed without a value.");
            }
            isHit = result.addWithPaintOffset(offset: childParentData.offset, position: position, hitTest: childHitTest);
        }
        return isHit;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        foreach (RenderBox child in _childrenInPaintOrder())
        {
            var childParentData = ((StackParentData?)child.parentData!)!;
            context.paintChild(child, childParentData.offset + offset);
        }
    }

}

public class _RenderLayoutSurrogateProxyBox__overlay : RenderProxyBox
{
    internal virtual _RenderDeferredLayoutBox__overlay? _deferredLayoutChild { get; set; } = default;
    public virtual _OverlayEntryLocation__overlay? overlayLocation { get; set; } = default;
    internal virtual bool _debugIsFirstAttach { get; set; } = true;
    internal virtual bool _didDetachDeferredChild { get; set; } = false;

    internal _RenderLayoutSurrogateProxyBox__overlay(_OverlayEntryLocation__overlay? overlayLocation)
    {
        this.overlayLocation = overlayLocation;
    }

    public override void attach(PipelineOwner owner)
    {
        base.attach(owner);
        if (_didDetachDeferredChild)
        {
            _didDetachDeferredChild = false;
            DartRuntimePrimitives.Assert(() => _deferredLayoutChild is not null);
            DartRuntimePrimitives.Assert(() => !_debugIsFirstAttach);
            overlayLocation!._reattachFromLayoutSurrogate(_deferredLayoutChild!);
        }
        DartRuntimePrimitives.Assert(() =>
            {
                _debugIsFirstAttach = false;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
    }

    public override void detach()
    {
        if (_deferredLayoutChild is object deferredChild && ((_RenderDeferredLayoutBox__overlay)deferredChild).theater.attached)
        {
            overlayLocation!._detachFromLayoutSurrogate(DartRuntimePrimitives.ConvertValue<_RenderDeferredLayoutBox__overlay>(deferredChild));
            _didDetachDeferredChild = true;
        }
        base.detach();
    }

    public override void redepthChildren()
    {
        base.redepthChildren();
        _RenderDeferredLayoutBox__overlay? child = _deferredLayoutChild;
        if ((child is not null) && child.attached)
        {
            redepthChild(child);
        }
    }

    public override void performLayout()
    {
        base.performLayout();
        _RenderDeferredLayoutBox__overlay? deferredChild = _deferredLayoutChild;
        if (deferredChild is null)
        {
            return;
        }
        var theater = ((_RenderTheater__overlay?)deferredChild.parent!)!;
        if (!theater._layingOutSizeDeterminingChild)
        {
            BoxConstraints theaterConstraints = theater.constraints;
            Size boxSize = theaterConstraints.biggest.isFinite ? theaterConstraints.biggest : theater.size;
            deferredChild._doLayoutFrom(this, constraints: BoxConstraints.CreateTight(boxSize));
        }
    }

}

internal class _OverlayChildLayoutBuilder__overlay : AbstractLayoutBuilder<OverlayChildLayoutInfo>
{
    private Func<BuildContext, OverlayChildLayoutInfo, Widget> __field_builder = default!;
    public override Func<BuildContext, OverlayChildLayoutInfo, Widget> builder { get => __field_builder; }

    internal _OverlayChildLayoutBuilder__overlay(Func<BuildContext, OverlayChildLayoutInfo, Widget> builder)
    {
        __field_builder = builder;
    }

    public override RenderObject createRenderObject(BuildContext context) => DartRuntimePrimitives.ConvertValue<RenderObject>(new _RenderLayoutBuilder__overlay());
}

internal class _RenderLayoutBuilder__overlay : RenderProxyBox, _RenderTheaterMixin__overlay, RenderAbstractLayoutBuilderMixin<OverlayChildLayoutInfo, RenderBox>, IRenderLayoutCallback
{
    internal virtual OverlayChildLayoutInfo? _layoutInfo { get; set; } = default;
    internal virtual long? _callbackId { get; set; } = default;
    internal const string _speculativeLayoutErrorMessage = "This RenderObject should not be reachable in intrinsic dimension calculations.";
    public virtual Action<Constraints>? _callback { get; set; } = default;

    public virtual IEnumerable<RenderBox> _childrenInPaintOrder()
    {
        RenderBox? childLocal = child;
        return (childLocal is null) ? Enumerable.Empty<RenderBox>() : Enumerable.Range(0, checked((int)1L)).Select(__index => ((Func<long, RenderBox>)((i) => childLocal))(checked(__index)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual IEnumerable<RenderBox> _childrenInHitTestOrder() => _childrenInPaintOrder();
    public virtual _RenderTheater__overlay theater => parent switch { _RenderDeferredLayoutBox__overlay parentLocal => parentLocal.theater, _ => throw DartRuntimePrimitives.AsException(FlutterError.Create($"{parent} of {this} is not a _RenderDeferredLayoutBox")) };
    public override bool sizedByParent => true;
    public override void performResize() => size = constraints.biggest;
    public override void applyPaintTransform(RenderObject child, Matrix4 transform)
    {
        var __child = (RenderBox)child;
        var childParentData = ((BoxParentData?)__child.parentData!)!;
        Offset offsetLocal = childParentData.offset;
        transform.translateByDouble(offsetLocal.dx, offsetLocal.dy, 0, 1);
    }

    public virtual OverlayChildLayoutInfo layoutInfo => DartRuntimePrimitives.ConvertValue<OverlayChildLayoutInfo>(_layoutInfo!);
    internal virtual OverlayChildLayoutInfo _computeNewLayoutInfo()
    {
        _RenderTheater__overlay theaterLocal = theater;
        var parentLocal = ((_RenderDeferredLayoutBox__overlay?)parent!)!;
        _RenderLayoutSurrogateProxyBox__overlay layoutSurrogate = parentLocal._layoutSurrogate;
        DartRuntimePrimitives.Assert(() =>
            {
                for (RenderObject? node = layoutSurrogate; (node is not null) && (!Equals(node, theaterLocal)); node = node.parent)
                {
                    if (node is RenderFollowerLayer)
                    {
                        RenderFollowerLayer node__105929__as106043 = (RenderFollowerLayer)node;
                        throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("The paint transform cannot be reliably computed because of RenderFollowerLayer(s)"), node__105929__as106043.describeForError("The RenderFollowerLayer was"), new ErrorDescription("RenderFollowerLayer establishes its paint transform only after the layout phase."), new ErrorHint("Consider replacing the corresponding CompositedTransformFollower with OverlayPortal.overlayChildLayoutBuilder if possible.") }));
                    }
                    DartRuntimePrimitives.Assert(() => node.depth > theaterLocal.depth);
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        DartRuntimePrimitives.Assert(() => layoutSurrogate.hasSize);
        DartRuntimePrimitives.Assert(() => layoutSurrogate.child?.hasSize ?? true);
        DartRuntimePrimitives.Assert(() => (layoutSurrogate.child is null) || Equals(layoutSurrogate.child!.size, layoutSurrogate.size));
        DartRuntimePrimitives.Assert(() => Equals(size, theaterLocal.size));
        DartRuntimePrimitives.Assert(() => layoutSurrogate.child?.getTransformTo(layoutSurrogate).isIdentity() ?? true);
        DartRuntimePrimitives.Assert(() => getTransformTo(theaterLocal).isIdentity());
        Size overlayPortalSize = parentLocal._layoutSurrogate.size;
        Matrix4 paintTransform = layoutSurrogate.getTransformTo(theaterLocal);
        return OverlayChildLayoutInfo.Create_((overlayPortalSize, paintTransform, size));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void layoutCallback()
    {
        _layoutInfo = _computeNewLayoutInfo();
        _callback!(constraints);
    }

    public override void performLayout()
    {
        runLayoutCallback();
        if (child is RenderBox childLocal)
        {
            layoutChild(childLocal, constraints);
        }
        DartRuntimePrimitives.Assert(() => _callbackId is null);
        _callbackId ??= Scheduler.SchedulerBinding.instance.scheduleFrameCallback(_frameCallback, scheduleNewFrame: false);
    }

    public override double computeMinIntrinsicWidth(double height)
    {
        DartRuntimePrimitives.Assert(() => debugCannotComputeDryLayout(reason: _speculativeLayoutErrorMessage));
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        DartRuntimePrimitives.Assert(() => debugCannotComputeDryLayout(reason: _speculativeLayoutErrorMessage));
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        DartRuntimePrimitives.Assert(() => debugCannotComputeDryLayout(reason: _speculativeLayoutErrorMessage));
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        DartRuntimePrimitives.Assert(() => debugCannotComputeDryLayout(reason: _speculativeLayoutErrorMessage));
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        DartRuntimePrimitives.Assert(() => debugCannotComputeDryLayout(reason: _speculativeLayoutErrorMessage));
        return Size.zero;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        DartRuntimePrimitives.Assert(() => debugCannotComputeDryLayout(reason: "Calculating the dry baseline would require running the layout callback " + "speculatively, which might mutate the live render object tree."));
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _frameCallback(Duration __unused0)
    {
        DartRuntimePrimitives.Assert(() => !DartRuntimePrimitives.RequireValue(debugDisposed));
        _callbackId = null;
        markNeedsLayout();
    }

    public override void dispose()
    {
        if (_callbackId is long callbackId)
        {
            Scheduler.SchedulerBinding.instance.cancelFrameCallbackWithId(callbackId);
        }
        base.dispose();
    }

    public override void setupParentData(RenderObject child)
    {
        var __child = (RenderBox)child;
        if (__child.parentData is not StackParentData)
        {
            __child.parentData = new StackParentData();
        }
    }

    public override double? computeDistanceToActualBaseline(TextBaseline baseline)
    {
        DartRuntimePrimitives.Assert(() => !debugNeedsLayout);
        BaselineOffset baselineOffset = BaselineOffset.noBaseline;
        foreach (RenderBox child in _childrenInPaintOrder())
        {
            DartRuntimePrimitives.Assert(() => !child.debugNeedsLayout);
            var childParentData = ((StackParentData?)child.parentData!)!;
            baselineOffset = baselineOffset.minOf(new BaselineOffset(child.getDistanceToActualBaseline(baseline)).op_Add(childParentData.offset.dy));
        }
        return baselineOffset.offset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void layoutChild(RenderBox child, BoxConstraints nonPositionedChildConstraints)
    {
        var childParentData = ((StackParentData?)child.parentData!)!;
        Alignment alignment = theater._resolvedAlignment;
        if (!childParentData.isPositioned)
        {
            child.layout(nonPositionedChildConstraints, parentUsesSize: true);
            childParentData.offset = Offset.zero;
        }
        else
        {
            DartRuntimePrimitives.Assert(() => child is not _RenderDeferredLayoutBox__overlay, () => (object?)"all _RenderDeferredLayoutBoxes must be non-positioned children.");
            RenderStack.layoutPositionedChild(child, childParentData, size, alignment);
        }
        DartRuntimePrimitives.Assert(() => Equals(child.parentData, childParentData));
    }

    public override bool hitTestChildren(BoxHitTestResult result, Offset position)
    {
        IEnumerator<RenderBox> iterator = _childrenInHitTestOrder().GetEnumerator();
        var isHit = false;
        while (!isHit && iterator.MoveNext())
        {
            RenderBox child = iterator.Current;
            var childParentData = ((StackParentData?)child.parentData!)!;
            var localChild = child;
            bool childHitTest(BoxHitTestResult result, Offset position)
            {
                return localChild.hitTest(result, position: position);
                throw new InvalidOperationException("Dart control flow completed without a value.");
            }
            isHit = result.addWithPaintOffset(offset: childParentData.offset, position: position, hitTest: childHitTest);
        }
        return isHit;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        foreach (RenderBox child in _childrenInPaintOrder())
        {
            var childParentData = ((StackParentData?)child.parentData!)!;
            context.paintChild(child, childParentData.offset + offset);
        }
    }

    public virtual void _updateCallback(Action<Constraints> value)
    {
        if (Equals(value, _callback))
        {
            return;
        }
        _callback = value;
        scheduleLayoutCallback();
    }

}
