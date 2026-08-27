// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/overlay.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Widgets;

public delegate Widget OverlayChildLayoutBuilder(BuildContext context, OverlayChildLayoutInfo info);

public class OverlayChildLayoutInfo
{
    public (global::Doroti.Ui.Size, Matrix4, global::Doroti.Ui.Size) _info { get; }

    private OverlayChildLayoutInfo((global::Doroti.Ui.Size, Matrix4, global::Doroti.Ui.Size) _info)
    {
        this._info = _info;
    }

    public static OverlayChildLayoutInfo Create_((global::Doroti.Ui.Size, Matrix4, global::Doroti.Ui.Size) _info) => new OverlayChildLayoutInfo(_info);

    public static implicit operator (global::Doroti.Ui.Size, Matrix4, global::Doroti.Ui.Size)(OverlayChildLayoutInfo value) => value._info;
    public static implicit operator OverlayChildLayoutInfo((global::Doroti.Ui.Size, Matrix4, global::Doroti.Ui.Size) value) => new OverlayChildLayoutInfo(value);

    public virtual global::Doroti.Ui.Size childSize => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Size>(_info.Item1);
    public virtual Matrix4 childPaintTransform => _info.Item2;
    public virtual global::Doroti.Ui.Size overlaySize => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Size>(_info.Item3);
}

public class OverlayEntry : global::Doroti.Framework.Foundation.Listenable
{
    public virtual global::System.Func<BuildContext, Widget> builder { get; private set; } = default!;
    internal virtual bool _opaque { get; set; } = default!;
    internal virtual bool _maintainState { get; set; } = default!;
    public virtual bool canSizeOverlay { get; private set; } = default!;
    internal virtual global::Doroti.Framework.Foundation.ValueNotifier<_OverlayEntryWidgetState__overlay?>? _overlayEntryStateNotifier { get; set; } = new global::Doroti.Framework.Foundation.ValueNotifier<_OverlayEntryWidgetState__overlay?>(((_OverlayEntryWidgetState__overlay)(object)null));
    internal virtual OverlayState? _overlay { get; set; } = default;
    internal virtual GlobalKey<_OverlayEntryWidgetState__overlay> _key { get; private set; } = GlobalKey<_OverlayEntryWidgetState__overlay>.Create();
    internal virtual bool _disposedByOwner { get; set; } = false;

    public OverlayEntry(global::System.Func<BuildContext, Widget> builder, bool opaque = false, bool maintainState = false, bool canSizeOverlay = false)
    {
        this.builder = builder;
        this.canSizeOverlay = canSizeOverlay;
        this._opaque = opaque;
        this._maintainState = maintainState;
    }

    public virtual bool opaque
    {
        get => this._opaque;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => !this._disposedByOwner);
            if ((this._opaque == __value))
            {
                return;
            }
            _opaque = __value;
            this._overlay?._didChangeEntryOpacity();
        }
    }
    public virtual bool maintainState
    {
        get => this._maintainState;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => !this._disposedByOwner);
            if ((this._maintainState == __value))
            {
                return;
            }
            _maintainState = __value;
            DartRuntimePrimitives.Assert(() => (this._overlay is not null));
            this._overlay!._didChangeEntryOpacity();
        }
    }
    public virtual bool mounted => DartRuntimePrimitives.ConvertValue<bool>((this._overlayEntryStateNotifier?.value is not null));
    public virtual void addListener(global::System.Action listener)
    {
        DartRuntimePrimitives.Assert(() => !this._disposedByOwner);
        this._overlayEntryStateNotifier?.addListener(() => listener());
    }

    public virtual void removeListener(global::System.Action listener)
    {
        this._overlayEntryStateNotifier?.removeListener(() => listener());
    }

    public virtual void remove()
    {
        DartRuntimePrimitives.Assert(() => (this._overlay is not null), () => (object?)"An OverlayEntry should be removed only once.");
        DartRuntimePrimitives.Assert(() => !this._disposedByOwner);
        OverlayState overlay = this._overlay!;
        _overlay = null;
        if (!overlay.mounted)
        {
            return;
        }
        ((OverlayState)overlay)._entries.Remove(this);
        if ((object.Equals(global::Doroti.Framework.Scheduler.SchedulerBinding.instance.schedulerPhase, global::Doroti.Framework.Scheduler.SchedulerPhase.persistentCallbacks)))
        {
            global::Doroti.Framework.Scheduler.SchedulerBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((duration) =>
            {
                overlay._markDirty();
            })), debugLabel: "OverlayEntry.markDirty");
        }
        else
        {
            overlay._markDirty();
        }
    }

    public virtual void markNeedsBuild()
    {
        DartRuntimePrimitives.Assert(() => !this._disposedByOwner);
        ((GlobalKey<_OverlayEntryWidgetState__overlay>)this._key).currentState?._markNeedsBuild();
    }

    internal virtual void _didUnmount()
    {
        DartRuntimePrimitives.Assert(() => !this.mounted);
        if (this._disposedByOwner)
        {
            this._overlayEntryStateNotifier?.dispose();
            _overlayEntryStateNotifier = null;
        }
    }

    public virtual void dispose()
    {
        DartRuntimePrimitives.Assert(() => !this._disposedByOwner);
        DartRuntimePrimitives.Assert(() => (this._overlay is null), () => (object?)"An OverlayEntry must first be removed from the Overlay before dispose is called.");
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Foundation.DebugLibrary.debugMaybeDispatchDisposed(this));
        _disposedByOwner = true;
        if (!this.mounted)
        {
            this._overlayEntryStateNotifier?.dispose();
            _overlayEntryStateNotifier = null;
        }
    }

    public override string ToString() => $"{(global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}(opaque: {this.opaque}; maintainState: {this.maintainState}){(this._disposedByOwner ? "(DISPOSED)" : "")}";
}

public class _OverlayEntryWidget__overlay : StatefulWidget
{
    public virtual OverlayEntry entry { get; private set; } = default!;
    public virtual OverlayState overlayState { get; private set; } = default!;
    public virtual bool tickerEnabled { get; private set; } = default!;

    internal _OverlayEntryWidget__overlay(global::Doroti.Framework.Foundation.Key key, OverlayEntry entry, OverlayState overlayState, bool tickerEnabled = true) : base(key: key)
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
        DartRuntimePrimitives.Assert(() => this.mounted);
        DartLinkedList<_OverlayEntryLocation__overlay> children = _sortedTheaterSiblings ??= new DartLinkedList<_OverlayEntryLocation__overlay>();
        DartRuntimePrimitives.Assert(() => !children.contains(child));
        _OverlayEntryLocation__overlay? insertPosition = (children.isEmpty ? null : children.last);
        while (((insertPosition is not null) && (((_OverlayEntryLocation__overlay)insertPosition)._zOrderIndex > ((_OverlayEntryLocation__overlay)child)._zOrderIndex)))
        {
            insertPosition = insertPosition.previous;
        }
        if ((insertPosition is null))
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
        DartRuntimePrimitives.Assert(() => (this._sortedTheaterSiblings is not null));
        bool wasInCollection = (this._sortedTheaterSiblings?.remove(child) ?? false);
        DartRuntimePrimitives.Assert(() => wasInCollection);
    }

    internal virtual IEnumerable<_RenderDeferredLayoutBox__overlay> _createChildIterable(bool reversed)
    {
        DartLinkedList<_OverlayEntryLocation__overlay>? children = this._sortedTheaterSiblings;
        if (((children is null) || children.isEmpty))
        {
            yield break;
        }
        _OverlayEntryLocation__overlay? candidate = (reversed ? children.last : children.first);
        while ((candidate is not null))
        {
            _RenderDeferredLayoutBox__overlay? renderBox = ((_OverlayEntryLocation__overlay)candidate)._overlayChildRenderBox;
            candidate = (reversed ? candidate.previous : candidate.next);
            if ((renderBox is not null))
            {
                yield return renderBox;
            }
        }
    }

    public override void initState()
    {
        base.initState();
        ((_OverlayEntryWidget__overlay)this.widget).entry._overlayEntryStateNotifier!.value = this;
        _theater = this.context.findAncestorRenderObjectOfType<_RenderTheater__overlay>()!;
        DartRuntimePrimitives.Assert(() => (this._sortedTheaterSiblings is null));
    }

    public override void didUpdateWidget(_OverlayEntryWidget__overlay oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        DartRuntimePrimitives.Assert(() => (object.Equals(((_OverlayEntryWidget__overlay)oldWidget).entry, ((_OverlayEntryWidget__overlay)this.widget).entry)));
        if ((!object.Equals(((_OverlayEntryWidget__overlay)oldWidget).overlayState, ((_OverlayEntryWidget__overlay)this.widget).overlayState)))
        {
            _RenderTheater__overlay newTheater = this.context.findAncestorRenderObjectOfType<_RenderTheater__overlay>()!;
            DartRuntimePrimitives.Assert(() => (!object.Equals(this._theater, newTheater)));
            _theater = newTheater;
        }
    }

    public override void dispose()
    {
        ((_OverlayEntryWidget__overlay)this.widget).entry._overlayEntryStateNotifier?.value = null;
        ((_OverlayEntryWidget__overlay)this.widget).entry._didUnmount();
        _sortedTheaterSiblings = null;
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new TickerMode(enabled: ((_OverlayEntryWidget__overlay)this.widget).tickerEnabled, child: new _RenderTheaterMarker__overlay(theater: this._theater, overlayEntryWidgetState: this, child: new Builder(builder: (global::System.Func<BuildContext, Widget>)((_OverlayEntryWidget__overlay)this.widget).entry.builder))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _markNeedsBuild()
    {
        setState(((global::System.Action)(() =>
        {
        })));
    }

}

public class Overlay : StatefulWidget
{
    public virtual List<OverlayEntry> initialEntries { get; private set; } = default!;
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual bool alwaysSizeToContent { get; private set; } = default!;

    public Overlay(global::Doroti.Framework.Foundation.Key? key = null, List<OverlayEntry> initialEntries = default!, Clip clipBehavior = Clip.hardEdge, bool alwaysSizeToContent = false) : base(key: key)
    {
        List<OverlayEntry> __initialEntries = initialEntries ?? new List<OverlayEntry>();
        this.initialEntries = __initialEntries;
        this.clipBehavior = clipBehavior;
        this.alwaysSizeToContent = alwaysSizeToContent;
    }

    public static Widget wrap(global::Doroti.Framework.Foundation.Key? key = null, Clip clipBehavior = Clip.hardEdge, bool alwaysSizeToContent = false, Widget child = default!)
    {
        return ((Widget)(object?)new _WrappingOverlay__overlay(key: key, clipBehavior: clipBehavior, alwaysSizeToContent: alwaysSizeToContent, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static OverlayState of(BuildContext context, bool rootOverlay = false, Widget? debugRequiredFor = null)
    {
        OverlayState? result = ((OverlayState?)(object?)Overlay.maybeOf(context, rootOverlay: rootOverlay));
        DartRuntimePrimitives.Assert(() =>
            {
                if ((result is null))
                {
                    bool hiddenByBoundary = LookupBoundary.debugIsHidingAncestorStateOfType<OverlayState>(context);
                    var information = new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"No Overlay widget found{(hiddenByBoundary ? " within the closest LookupBoundary" : "")}."), new global::Doroti.Framework.Foundation.ErrorDescription($"{(((object?)DartRuntimePrimitives.RuntimeType(debugRequiredFor) ?? (object?)"Some"))} widgets require an Overlay widget ancestor for correct operation."), new global::Doroti.Framework.Foundation.ErrorHint("The most common way to add an Overlay to an application is to include a MaterialApp, CupertinoApp or Navigator widget in the runApp() call.") };
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(information));
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
    public virtual HashSet<global::Doroti.Framework.Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override void initState()
    {
        base.initState();
        insertAll(((Overlay)this.widget).initialEntries.Cast<OverlayEntry>());
    }

    internal virtual long _insertionIndex(OverlayEntry? below, OverlayEntry? above)
    {
        DartRuntimePrimitives.Assert(() => ((above is null) || (below is null)));
        if ((below is not null))
        {
            return ((long)((dynamic)this._entries).IndexOf(below));
        }
        if ((above is not null))
        {
            return (((long)((dynamic)this._entries).IndexOf(above)) + 1L);
        }
        return checked((long)(this._entries.Count));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _debugCanInsertEntry(OverlayEntry entry)
    {
        var operandsInformation = new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.DiagnosticsProperty<OverlayEntry>("The OverlayEntry was", entry, style: global::Doroti.Framework.Foundation.DiagnosticsTreeStyle.errorProperty), new global::Doroti.Framework.Foundation.DiagnosticsProperty<OverlayState>("The Overlay the OverlayEntry was trying to insert to was", this, style: global::Doroti.Framework.Foundation.DiagnosticsTreeStyle.errorProperty) };
        if (!this.mounted)
        {
            throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("Attempted to insert an OverlayEntry to an already disposed Overlay.") }));
        }
        OverlayState? currentOverlay = ((OverlayEntry)entry)._overlay;
        bool alreadyContainsEntry = this._entries.Contains(entry);
        if (alreadyContainsEntry)
        {
            bool inconsistentOverlayState = !DartRuntimePrimitives.Identical(currentOverlay, this);
            throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("The specified entry is already present in the target Overlay.") }));
        }
        if ((currentOverlay is null))
        {
            return true;
        }
        throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("The specified entry is already present in a different Overlay."), new global::Doroti.Framework.Foundation.DiagnosticsProperty<OverlayState>("The OverlayEntry's current Overlay was", currentOverlay, style: global::Doroti.Framework.Foundation.DiagnosticsTreeStyle.errorProperty), new global::Doroti.Framework.Foundation.ErrorHint("Consider calling remove on the OverlayEntry before inserting it to a different Overlay, " + "or switching to the OverlayPortal API to avoid manual OverlayEntry management.") }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void insert(OverlayEntry entry, OverlayEntry? below = null, OverlayEntry? above = null)
    {
        DartRuntimePrimitives.Assert(() => _debugVerifyInsertPosition(above, below));
        DartRuntimePrimitives.Assert(() => _debugCanInsertEntry(entry));
        entry._overlay = this;
        setState(((global::System.Action)(() =>
        {
            this._entries.Insert(checked((int)_insertionIndex(below, above)), entry);
        })));
    }

    public virtual void insertAll(IEnumerable<OverlayEntry> entries, OverlayEntry? below = null, OverlayEntry? above = null)
    {
        DartRuntimePrimitives.Assert(() => _debugVerifyInsertPosition(above, below));
        DartRuntimePrimitives.Assert(() => entries.All(this._debugCanInsertEntry));
        if (!System.Linq.Enumerable.Any(entries))
        {
            return;
        }
        foreach (var entry in entries)
        {
            DartRuntimePrimitives.Assert(() => (((OverlayEntry)entry)._overlay is null));
            entry._overlay = this;
        }
        setState(((global::System.Action)(() =>
        {
            this._entries.InsertRange(checked((int)_insertionIndex(below, above)), entries.Cast<OverlayEntry>());
        })));
    }

    internal virtual bool _debugVerifyInsertPosition(OverlayEntry? above, OverlayEntry? below, IEnumerable<OverlayEntry>? newEntries = null)
    {
        DartRuntimePrimitives.Assert(() => ((above is null) || (below is null)), () => (object?)"Only one of `above` and `below` may be specified.");
        DartRuntimePrimitives.Assert(() => ((above is null) || ((((object.Equals(((OverlayEntry)above)._overlay, this)) && this._entries.Contains(above)) && ((newEntries?.contains(above) ?? true))))), () => (object?)$"The provided entry used for `above` must be present in the Overlay{((newEntries is not null) ? " and in the `newEntriesList`" : "")}.");
        DartRuntimePrimitives.Assert(() => ((below is null) || ((((object.Equals(((OverlayEntry)below)._overlay, this)) && this._entries.Contains(below)) && ((newEntries?.contains(below) ?? true))))), () => (object?)$"The provided entry used for `below` must be present in the Overlay{((newEntries is not null) ? " and in the `newEntriesList`" : "")}.");
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void rearrange(IEnumerable<OverlayEntry> newEntries, OverlayEntry? below = null, OverlayEntry? above = null)
    {
        List<OverlayEntry> newEntriesList = ((newEntries is List<OverlayEntry>) ? newEntries : newEntries.ToList()).ToList();
        DartRuntimePrimitives.Assert(() => _debugVerifyInsertPosition(above, below, newEntries: newEntriesList.Cast<OverlayEntry>()));
        DartRuntimePrimitives.Assert(() => newEntriesList.All(((entry) => ((((OverlayEntry)entry)._overlay is null) || (object.Equals(((OverlayEntry)entry)._overlay, this))))), () => (object?)"One or more of the specified entries are already present in another Overlay.");
        DartRuntimePrimitives.Assert(() => newEntriesList.All(((entry) => (((long)((dynamic)this._entries).IndexOf(entry)) == this._entries.LastIndexOf(entry)))), () => (object?)"One or more of the specified entries are specified multiple times.");
        if (!System.Linq.Enumerable.Any(newEntriesList))
        {
            return;
        }
        if (global::Doroti.Framework.Foundation.CollectionsLibrary.listEquals(this._entries, newEntriesList))
        {
            return;
        }
        var old = new HashSet<OverlayEntry>(this._entries);
        foreach (var entryLocal in newEntriesList)
        {
            entryLocal._overlay ??= this;
        }
        setState(((global::System.Action)(() =>
        {
            this._entries.Clear();
            this._entries.AddRange(newEntriesList.Cast<OverlayEntry>());
            old.ExceptWith(newEntriesList);
            this._entries.InsertRange(checked((int)_insertionIndex(below, above)), old);
        })));
    }

    internal virtual void _markDirty()
    {
        if (this.mounted)
        {
            setState(((global::System.Action)(() =>
            {
            })));
        }
    }

    public virtual bool debugIsVisible(OverlayEntry entry)
    {
        var result = false;
        DartRuntimePrimitives.Assert(() => this._entries.Contains(entry));
        DartRuntimePrimitives.Assert(() =>
            {
                for (long i = (checked((long)(this._entries.Count)) - 1L); (i > 0L); i -= 1L)
                {
                    OverlayEntry candidate = this._entries[(int)(i)];
                    if ((object.Equals(candidate, entry)))
                    {
                        result = true;
                        break;
                    }
                    if (((OverlayEntry)candidate).opaque)
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
        setState(((global::System.Action)(() =>
        {
        })));
    }

    public override Widget build(BuildContext context)
    {
        var childrenLocal = new List<_OverlayEntryWidget__overlay>();
        var onstage = true;
        var onstageCount = 0L;
        foreach (OverlayEntry entryLocal in System.Linq.Enumerable.Reverse(this._entries))
        {
            if (onstage)
            {
                onstageCount += 1L;
                childrenLocal.Add(new _OverlayEntryWidget__overlay(key: ((OverlayEntry)entryLocal)._key, overlayState: this, entry: entryLocal));
                if (((OverlayEntry)entryLocal).opaque)
                {
                    onstage = false;
                }
            }
            else
            {
                if (((OverlayEntry)entryLocal).maintainState)
                {
                    childrenLocal.Add(new _OverlayEntryWidget__overlay(key: ((OverlayEntry)entryLocal)._key, overlayState: this, entry: entryLocal, tickerEnabled: false));
                }
            }
        }
        return ((Widget)(object?)new _Theater__overlay(skipCount: (checked((long)(childrenLocal.Count)) - onstageCount), clipBehavior: ((Overlay)this.widget).clipBehavior, alwaysSizeToContent: ((Overlay)this.widget).alwaysSizeToContent, children: System.Linq.Enumerable.Reverse(childrenLocal).ToList()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<HashSet<global::Doroti.Framework.Scheduler.Ticker>>("tickers", this._tickers, description: ((this._tickers is not null) ? $"tracking {checked((long)(this._tickers!.Count))} ticker{((checked((long)(this._tickers!.Count)) == 1L) ? "" : "s")}" : null), defaultValue: default));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<List<OverlayEntry>>("entries", this._entries));
    }

    public virtual global::Doroti.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        if ((this._tickerModeNotifier is null))
        {
            _updateTickerModeNotifier();
        }
        DartRuntimePrimitives.Assert(() => (this._tickerModeNotifier is not null));
        this._tickers ??= new HashSet<global::Doroti.Framework.Scheduler.Ticker>();
        TickerModeData values = this._tickerModeNotifier!.value;
        var result = ((Func<_WidgetTicker__ticker_provider>)(() =>
{
    var __cascade = new _WidgetTicker__ticker_provider((global::System.Action<Duration>)onTick, this, debugLabel: (global::Doroti.Framework.Foundation.ConstantsLibrary.kDebugMode ? $"created by {(global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}" : null));
    __cascade.muted = !((TickerModeData)values).enabled;
    __cascade.forceFrames = ((TickerModeData)values).forceFrames;
    return __cascade;
}))();
        this._tickers!.Add(result);
        return ((global::Doroti.Framework.Scheduler.Ticker)(object?)result);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _removeTicker(_WidgetTicker__ticker_provider ticker)
    {
        DartRuntimePrimitives.Assert(() => (this._tickers is not null));
        DartRuntimePrimitives.Assert(() => this._tickers!.Contains(ticker));
        this._tickers!.Remove(ticker);
    }

    public override void activate()
    {
        base.activate();
        _updateTickerModeNotifier();
        _updateTickers();
    }

    public virtual void _updateTickers()
    {
        if ((this._tickers is not null))
        {
            TickerModeData values = this._tickerModeNotifier!.value;
            bool mutedLocal = !((TickerModeData)values).enabled;
            foreach (global::Doroti.Framework.Scheduler.Ticker ticker in this._tickers!)
            {
                ticker.muted = mutedLocal;
                ticker.forceFrames = ((TickerModeData)values).forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Framework.Foundation.ValueListenable<TickerModeData> newNotifier = ((global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>)(object?)TickerMode.getValuesNotifier(this.context));
        if ((object.Equals(newNotifier, this._tickerModeNotifier)))
        {
            return;
        }
        this._tickerModeNotifier?.removeListener(() => this._updateTickers());
        newNotifier.addListener(() => this._updateTickers());
        this._tickerModeNotifier = newNotifier;
    }

    public override void dispose()
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this._tickers is not null))
                {
                    foreach (global::Doroti.Framework.Scheduler.Ticker ticker in this._tickers!)
                    {
                        if (((global::Doroti.Framework.Scheduler.Ticker)ticker).isActive)
                        {
                            throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Framework.Foundation.ErrorDescription($"{this.GetType()} created a Ticker via its TickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. All Tickers must " + "be disposed before calling super.dispose()."), new global::Doroti.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), ticker.describeForError("The offending ticker was") }));
                        }
                    }
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        this._tickerModeNotifier?.removeListener(() => this._updateTickers());
        this._tickerModeNotifier = null;
        base.dispose();
    }

}

public class _WrappingOverlay__overlay : StatefulWidget
{
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual bool alwaysSizeToContent { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;

    internal _WrappingOverlay__overlay(global::Doroti.Framework.Foundation.Key? key = null, Clip clipBehavior = Clip.hardEdge, bool alwaysSizeToContent = default!, Widget child = default!) : base(key: key)
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
                __late__entry = new OverlayEntry(canSizeOverlay: true, opaque: true, builder: ((global::System.Func<BuildContext, Widget>)((context) =>
                {
                    return ((_WrappingOverlay__overlay)this.widget).child;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                })));
                __late__entry_initialized = true;
            }
            return __late__entry;
        }
    }

    public override void didUpdateWidget(_WrappingOverlay__overlay oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        this._entry.markNeedsBuild();
    }

    public override void dispose()
    {
        DartRuntimePrimitives.Ignore(((Func<OverlayEntry>)(() =>
{
    var __cascade = this._entry;
    __cascade.remove();
    __cascade.dispose();
    return __cascade;
}))());
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new Overlay(clipBehavior: ((_WrappingOverlay__overlay)this.widget).clipBehavior, alwaysSizeToContent: ((_WrappingOverlay__overlay)this.widget).alwaysSizeToContent, initialEntries: new List<OverlayEntry> { this._entry }));
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
        System.Diagnostics.Debug.Assert((skipCount >= 0L));
        System.Diagnostics.Debug.Assert((checked((long)(children.Count)) >= skipCount));
    }

    public override _TheaterElement__overlay createElement() => new _TheaterElement__overlay(this);
    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)new _RenderTheater__overlay(skipCount: this.skipCount, textDirection: Directionality.of(context), clipBehavior: this.clipBehavior, alwaysSizeToContent: this.alwaysSizeToContent));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderTheater__overlay)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<_RenderTheater__overlay>)(() =>
{
    var __cascade = __renderObject;
    __cascade.skipCount = this.skipCount;
    __cascade.textDirection = Directionality.of(context);
    __cascade.clipBehavior = this.clipBehavior;
    __cascade.alwaysSizeToContent = this.alwaysSizeToContent;
    return __cascade;
}))());
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.IntProperty("skipCount", this.skipCount));
    }

}

public class _TheaterElement__overlay : MultiChildRenderObjectElement
{
    internal _TheaterElement__overlay(_Theater__overlay widget) : base(widget)
    {
    }

    public override global::Doroti.Framework.Rendering.RenderObject renderObject => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.RenderObject>(((_RenderTheater__overlay?)(object?)base.renderObject)!);
    public override void insertRenderObjectChild(global::Doroti.Framework.Rendering.RenderObject child, object? slot)
    {
        var __child = (global::Doroti.Framework.Rendering.RenderBox)(object)child;
        var __slot = (IndexedSlot<Element?>)(object)slot;
        base.insertRenderObjectChild(__child, __slot);
        var parentDataLocal = ((_TheaterParentData__overlay?)(object?)__child.parentData!)!;
        parentDataLocal.overlayEntry = (((_OverlayEntryWidget__overlay?)(object?)(((_Theater__overlay?)(object?)this.widget)!).children[(int)(((IndexedSlot<Element?>)__slot).index)])!).entry;
        DartRuntimePrimitives.Assert(() => (((_TheaterParentData__overlay)parentDataLocal).overlayEntry is not null));
    }

    public override void moveRenderObjectChild(global::Doroti.Framework.Rendering.RenderObject child, object? oldSlot, object? newSlot)
    {
        var __child = (global::Doroti.Framework.Rendering.RenderBox)(object)child;
        var __oldSlot = (IndexedSlot<Element?>)(object)oldSlot;
        var __newSlot = (IndexedSlot<Element?>)(object)newSlot;
        base.moveRenderObjectChild(__child, __oldSlot, __newSlot);
        DartRuntimePrimitives.Assert(() =>
            {
                var parentDataLocal = ((_TheaterParentData__overlay?)(object?)__child.parentData!)!;
                OverlayEntry entryAtNewSlot = (((_OverlayEntryWidget__overlay?)(object?)(((_Theater__overlay?)(object?)this.widget)!).children[(int)(((IndexedSlot<Element?>)__newSlot).index)])!).entry;
                DartRuntimePrimitives.Assert(() => (object.Equals(((_TheaterParentData__overlay)parentDataLocal).overlayEntry, entryAtNewSlot)));
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
    }

    public override void debugVisitOnstageChildren(global::System.Action<Element> visitor)
    {
        var theater = ((_Theater__overlay?)(object?)this.widget)!;
        DartRuntimePrimitives.Assert(() => (this.children.Count() >= ((_Theater__overlay)theater).skipCount));
        this.children.skip(((_Theater__overlay)theater).skipCount).forEach((__arg0) => ((global::System.Action<Element>)visitor)(__arg0));
    }

}

internal interface _RenderTheaterMixin__overlay
{
    public _RenderTheater__overlay theater { get; }
    public IEnumerable<global::Doroti.Framework.Rendering.RenderBox> _childrenInPaintOrder();
    public IEnumerable<global::Doroti.Framework.Rendering.RenderBox> _childrenInHitTestOrder();
    public void setupParentData(global::Doroti.Framework.Rendering.RenderObject child);
    public double? computeDistanceToActualBaseline(TextBaseline baseline);
    public static double? baselineForChild(global::Doroti.Framework.Rendering.RenderBox child, Size theaterSize, global::Doroti.Framework.Rendering.BoxConstraints nonPositionedChildConstraints, global::Doroti.Framework.Painting.Alignment alignment, TextBaseline baseline)
    {
        var childParentData = ((global::Doroti.Framework.Rendering.StackParentData?)(object?)child.parentData!)!;
        global::Doroti.Framework.Rendering.BoxConstraints childConstraints = (((global::Doroti.Framework.Rendering.StackParentData)childParentData).isPositioned ? childParentData.positionedChildConstraints(theaterSize) : nonPositionedChildConstraints);
        double? baselineOffset = child.getDryBaseline(childConstraints, baseline);
        if ((baselineOffset is null))
        {
            return null;
        }
        double y = (childParentData switch { global::Doroti.Framework.Rendering.StackParentData { top: double topLocal } __object40535 => topLocal, global::Doroti.Framework.Rendering.StackParentData { bottom: double bottomLocal } __object40585 => ((theaterSize.height - bottomLocal) - child.getDryLayout(childConstraints).height), global::Doroti.Framework.Rendering.StackParentData __object40716 => alignment.alongOffset((theaterSize - child.getDryLayout(childConstraints))).dy, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        return (DartRuntimePrimitives.RequireValue(baselineOffset) + y);
    }
    public void layoutChild(global::Doroti.Framework.Rendering.RenderBox child, global::Doroti.Framework.Rendering.BoxConstraints nonPositionedChildConstraints);
    public bool hitTestChildren(global::Doroti.Framework.Rendering.BoxHitTestResult result, Offset position);
    public void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset);
}

internal class _TheaterParentData__overlay : global::Doroti.Framework.Rendering.StackParentData
{
    public virtual OverlayEntry? overlayEntry { get; set; } = default;

    public virtual IEnumerator<_RenderDeferredLayoutBox__overlay>? paintOrderIterator => this.overlayEntry?._overlayEntryStateNotifier?.value!._paintOrderIterable.GetEnumerator();
    public virtual IEnumerator<_RenderDeferredLayoutBox__overlay>? hitTestOrderIterator => this.overlayEntry?._overlayEntryStateNotifier?.value!._hitTestOrderIterable.GetEnumerator();
    public virtual void visitOverlayPortalChildrenOnOverlayEntry(global::System.Action<global::Doroti.Framework.Rendering.RenderObject> visitor) => this.overlayEntry?._overlayEntryStateNotifier?.value!._paintOrderIterable.forEach((__arg0) => ((global::System.Action<global::Doroti.Framework.Rendering.RenderObject>)visitor)(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.RenderObject>(__arg0)));
}

public class _RenderTheater__overlay : global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.ContainerRenderObjectMixin<global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.StackParentData>, _RenderTheaterMixin__overlay
{
    internal virtual global::Doroti.Framework.Painting.Alignment? _alignmentCache { get; set; } = default;
    internal virtual TextDirection _textDirection { get; set; } = default!;
    internal virtual long _skipCount { get; set; } = default!;
    internal virtual Clip _clipBehavior { get; set; } = Clip.hardEdge;
    internal virtual bool _alwaysSizeToContent { get; set; } = default!;
    internal virtual long _outstandingDeferredChildUpdateCalls { get; set; } = 0L;
    internal virtual bool _layingOutSizeDeterminingChild { get; set; } = false;
    internal virtual global::Doroti.Framework.Rendering.LayerHandle<global::Doroti.Framework.Rendering.ClipRectLayer> _clipRectLayer { get; private set; } = new global::Doroti.Framework.Rendering.LayerHandle<global::Doroti.Framework.Rendering.ClipRectLayer>();
    public virtual long _childCount { get; set; } = 0L;
    public virtual RenderBox? _firstChild { get; set; } = default;
    public virtual RenderBox? _lastChild { get; set; } = default;

    internal _RenderTheater__overlay(List<global::Doroti.Framework.Rendering.RenderBox>? children = null, TextDirection textDirection = default!, long skipCount = 0, Clip clipBehavior = Clip.hardEdge, bool alwaysSizeToContent = default!)
    {
        this._textDirection = textDirection;
        this._skipCount = skipCount;
        this._clipBehavior = clipBehavior;
        this._alwaysSizeToContent = alwaysSizeToContent;
        System.Diagnostics.Debug.Assert((skipCount >= 0L));
    }

    public virtual _RenderTheater__overlay theater => this;
    public override void setupParentData(global::Doroti.Framework.Rendering.RenderObject child)
    {
        var __child = (global::Doroti.Framework.Rendering.RenderBox)(object)child;
        if ((__child.parentData is not _TheaterParentData__overlay))
        {
            __child.parentData = new _TheaterParentData__overlay();
        }
    }

    public override void attach(global::Doroti.Framework.Rendering.PipelineOwner owner)
    {
        base.attach(owner);
        global::Doroti.Framework.Rendering.RenderBox? child = this._firstChild;
        while ((child is not null))
        {
            child.attach(owner);
            var childParentData = ((global::Doroti.Framework.Rendering.StackParentData?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
        global::Doroti.Framework.Rendering.RenderBox? childLocal = this.firstChild;
        while ((childLocal is not null))
        {
            var childParentDataLocal = ((_TheaterParentData__overlay?)(object?)childLocal.parentData!)!;
            IEnumerator<global::Doroti.Framework.Rendering.RenderBox>? iterator = ((IEnumerator<global::Doroti.Framework.Rendering.RenderBox>?)(object?)((_TheaterParentData__overlay)childParentDataLocal).paintOrderIterator);
            if ((iterator is not null))
            {
                while (iterator.MoveNext())
                {
                    iterator.Current.attach(owner);
                }
            }
            childLocal = childParentDataLocal.nextSibling;
        }
    }

    internal static void _detachChild(global::Doroti.Framework.Rendering.RenderObject child) => ((dynamic)child).detach();
    public override void detach()
    {
        base.detach();
        global::Doroti.Framework.Rendering.RenderBox? child = this._firstChild;
        while ((child is not null))
        {
            child.detach();
            var childParentData = ((global::Doroti.Framework.Rendering.StackParentData?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
        global::Doroti.Framework.Rendering.RenderBox? childLocal = this.firstChild;
        while ((childLocal is not null))
        {
            var childParentDataLocal = ((_TheaterParentData__overlay?)(object?)childLocal.parentData!)!;
            childParentDataLocal.visitOverlayPortalChildrenOnOverlayEntry((global::System.Action<global::Doroti.Framework.Rendering.RenderObject>)_detachChild);
            childLocal = childParentDataLocal.nextSibling;
        }
    }

    public override void redepthChildren() => visitChildren((global::System.Action<global::Doroti.Framework.Rendering.RenderObject>)this.redepthChild);
    internal virtual global::Doroti.Framework.Painting.Alignment _resolvedAlignment => _alignmentCache ??= global::Doroti.Framework.Painting.AlignmentDirectional.topStart.resolve(this.textDirection);
    internal virtual void _markNeedResolution()
    {
        _alignmentCache = null;
        markNeedsLayout();
    }

    public virtual global::Doroti.Ui.TextDirection textDirection
    {
        get => this._textDirection;
        set
        {
            var __value = value;
            if ((object.Equals(this._textDirection, __value)))
            {
                return;
            }
            _textDirection = __value;
            _markNeedResolution();
        }
    }
    public virtual long skipCount
    {
        get => this._skipCount;
        set
        {
            var __value = value;
            if ((this._skipCount != __value))
            {
                _skipCount = __value;
                markNeedsLayout();
            }
        }
    }
    public virtual global::Doroti.Ui.Clip clipBehavior
    {
        get => this._clipBehavior;
        set
        {
            var __value = value;
            if ((!object.Equals(__value, this._clipBehavior)))
            {
                _clipBehavior = __value;
                markNeedsPaint();
                markNeedsSemanticsUpdate();
            }
        }
    }
    public virtual bool alwaysSizeToContent
    {
        get => this._alwaysSizeToContent;
        set
        {
            var __value = value;
            if ((this._alwaysSizeToContent != __value))
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
        DartRuntimePrimitives.Assert(() => (this._outstandingDeferredChildUpdateCalls >= 0L));
        ((_RenderDeferredLayoutBox__overlay)child)._layoutSurrogate.markNeedsLayout();
    }

    internal virtual void _removeDeferredChild(_RenderDeferredLayoutBox__overlay child)
    {
        _outstandingDeferredChildUpdateCalls += 1L;
        dropChild(child);
        markNeedsPaint();
        _outstandingDeferredChildUpdateCalls -= 1L;
        DartRuntimePrimitives.Assert(() => (this._outstandingDeferredChildUpdateCalls >= 0L));
    }

    public override void markNeedsLayout()
    {
        if ((this._outstandingDeferredChildUpdateCalls == 0L))
        {
            base.markNeedsLayout();
        }
    }

    internal virtual global::Doroti.Framework.Rendering.RenderBox? _firstOnstageChild
    {
        get
        {
            if ((this.skipCount == this.childCount))
            {
                return ((global::Doroti.Framework.Rendering.RenderBox)(object)null);
            }
            global::Doroti.Framework.Rendering.RenderBox? child = this.firstChild;
            for (long toSkip = this.skipCount; (toSkip > 0L); toSkip--)
            {
                var childParentData = ((global::Doroti.Framework.Rendering.StackParentData?)(object?)child!.parentData!)!;
                child = childParentData.nextSibling;
                DartRuntimePrimitives.Assert(() => (child is not null));
            }
            return child;
            return default!;
        }
    }
    internal virtual global::Doroti.Framework.Rendering.RenderBox? _lastOnstageChild => ((this.skipCount == this.childCount) ? null : this.lastChild);
    public override double computeMinIntrinsicWidth(double height)
    {
        return RenderStack.getIntrinsicDimension(this._firstOnstageChild, ((global::System.Func<global::Doroti.Framework.Rendering.RenderBox, double>)((child) => child.getMinIntrinsicWidth(height))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        return RenderStack.getIntrinsicDimension(this._firstOnstageChild, ((global::System.Func<global::Doroti.Framework.Rendering.RenderBox, double>)((child) => child.getMaxIntrinsicWidth(height))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        return RenderStack.getIntrinsicDimension(this._firstOnstageChild, ((global::System.Func<global::Doroti.Framework.Rendering.RenderBox, double>)((child) => child.getMinIntrinsicHeight(width))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        return RenderStack.getIntrinsicDimension(this._firstOnstageChild, ((global::System.Func<global::Doroti.Framework.Rendering.RenderBox, double>)((child) => child.getMaxIntrinsicHeight(width))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(global::Doroti.Framework.Rendering.BoxConstraints constraints, TextBaseline baseline)
    {
        global::Doroti.Ui.Size sizeLocal = ((global::Doroti.Ui.Size)(object?)((!this.alwaysSizeToContent && ((global::Doroti.Framework.Rendering.BoxConstraints)constraints).biggest.isFinite) ? ((global::Doroti.Framework.Rendering.BoxConstraints)constraints).biggest : _findSizeDeterminingChild().getDryLayout(constraints)));
        var nonPositionedChildConstraints = global::Doroti.Framework.Rendering.BoxConstraints.CreateTight(this.size);
        global::Doroti.Framework.Painting.Alignment alignment = ((_RenderTheater__overlay)this.theater)._resolvedAlignment;
        global::Doroti.Framework.Rendering.BaselineOffset baselineOffset = global::Doroti.Framework.Rendering.BaselineOffset.noBaseline;
        foreach (global::Doroti.Framework.Rendering.RenderBox child in _childrenInPaintOrder())
        {
            baselineOffset = baselineOffset.minOf(new global::Doroti.Framework.Rendering.BaselineOffset(_RenderTheaterMixin__overlay.baselineForChild(child, this.size, nonPositionedChildConstraints, alignment, baseline)));
        }
        return baselineOffset.offset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(global::Doroti.Framework.Rendering.BoxConstraints constraints)
    {
        if ((!this.alwaysSizeToContent && ((global::Doroti.Framework.Rendering.BoxConstraints)constraints).biggest.isFinite))
        {
            return ((global::Doroti.Framework.Rendering.BoxConstraints)constraints).biggest;
        }
        return _findSizeDeterminingChild().getDryLayout(constraints);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual IEnumerable<global::Doroti.Framework.Rendering.RenderBox> _childrenInPaintOrder()
    {
        global::Doroti.Framework.Rendering.RenderBox? child = this._firstOnstageChild;
        while ((child is not null))
        {
            yield return child;
            var childParentData = ((_TheaterParentData__overlay?)(object?)child.parentData!)!;
            IEnumerator<global::Doroti.Framework.Rendering.RenderBox>? innerIterator = ((IEnumerator<global::Doroti.Framework.Rendering.RenderBox>?)(object?)((_TheaterParentData__overlay)childParentData).paintOrderIterator);
            if ((innerIterator is not null))
            {
                while (innerIterator.MoveNext())
                {
                    yield return innerIterator.Current;
                }
            }
            child = childParentData.nextSibling;
        }
    }

    public virtual IEnumerable<global::Doroti.Framework.Rendering.RenderBox> _childrenInHitTestOrder()
    {
        global::Doroti.Framework.Rendering.RenderBox? child = this._lastOnstageChild;
        long childLeft = (this.childCount - this.skipCount);
        while ((child is not null))
        {
            var childParentData = ((_TheaterParentData__overlay?)(object?)child.parentData!)!;
            IEnumerator<global::Doroti.Framework.Rendering.RenderBox>? innerIterator = ((IEnumerator<global::Doroti.Framework.Rendering.RenderBox>?)(object?)((_TheaterParentData__overlay)childParentData).hitTestOrderIterator);
            if ((innerIterator is not null))
            {
                while (innerIterator.MoveNext())
                {
                    yield return innerIterator.Current;
                }
            }
            yield return child;
            childLeft -= 1L;
            child = ((childLeft <= 0L) ? null : childParentData.previousSibling);
        }
    }

    public override bool sizedByParent => false;
    public override void performLayout()
    {
        global::Doroti.Framework.Rendering.RenderBox? sizeDeterminingChild = default!;
        if ((!this.alwaysSizeToContent && ((global::Doroti.Framework.Rendering.BoxConstraints)this.constraints).biggest.isFinite))
        {
            this.size = ((global::Doroti.Framework.Rendering.BoxConstraints)this.constraints).biggest;
        }
        else
        {
            sizeDeterminingChild = _findSizeDeterminingChild();
            _layingOutSizeDeterminingChild = true;
            layoutChild(sizeDeterminingChild, this.constraints);
            _layingOutSizeDeterminingChild = false;
            this.size = ((global::Doroti.Framework.Rendering.RenderBox)sizeDeterminingChild).size;
        }
        var nonPositionedChildConstraints = global::Doroti.Framework.Rendering.BoxConstraints.CreateTight(this.size);
        foreach (global::Doroti.Framework.Rendering.RenderBox child in _childrenInPaintOrder())
        {
            if ((!object.Equals(child, sizeDeterminingChild)))
            {
                layoutChild(child, nonPositionedChildConstraints);
            }
        }
    }

    internal virtual global::Doroti.Framework.Rendering.RenderBox _findSizeDeterminingChild()
    {
        global::Doroti.Framework.Rendering.RenderBox? child = this._lastOnstageChild;
        while ((child is not null))
        {
            var childParentData = ((_TheaterParentData__overlay?)(object?)child.parentData!)!;
            if ((((((_TheaterParentData__overlay)childParentData).overlayEntry?.canSizeOverlay ?? false)) && !childParentData.isPositioned))
            {
                return child;
            }
            child = childParentData.previousSibling;
        }
        if (this.alwaysSizeToContent)
        {
            throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("Overlay was asked to size itself to content but does not have a suitable child."), new global::Doroti.Framework.Foundation.ErrorDescription("When `alwaysSizeToContent` is true, the Overlay requires at least one " + "non-positioned `OverlayEntry` with `canSizeOverlay` set to true to determine its size."), new global::Doroti.Framework.Foundation.ErrorHint("Try removing alwaysSizeToContent=true or provide a suitable child that can size the Overlay") }));
        }
        throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("Overlay was given infinite constraints and cannot be sized by a suitable child."), new global::Doroti.Framework.Foundation.ErrorDescription($"The constraints given to the overlay ({this.constraints}) would result in an illegal " + $"infinite size ({(((global::Doroti.Framework.Rendering.BoxConstraints)this.constraints).biggest)}). To avoid that, the Overlay tried to size " + "itself to one of its children, but no suitable non-positioned child that belongs to an " + "OverlayEntry with canSizeOverlay set to true could be found."), new global::Doroti.Framework.Foundation.ErrorHint("Try wrapping the Overlay in a SizedBox to give it a finite size or " + "use an OverlayEntry with canSizeOverlay set to true.") }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset)
    {
        if ((!object.Equals(this.clipBehavior, Clip.none)))
        {
            this._clipRectLayer.layer = context.pushClipRect(this.needsCompositing, offset, (Offset.zero & this.size), (paintContext, paintOffset) =>
            {
                foreach (global::Doroti.Framework.Rendering.RenderBox child in _childrenInPaintOrder())
                {
                    var childParentData = ((global::Doroti.Framework.Rendering.StackParentData?)(object?)child.parentData!)!;
                    paintContext.paintChild(child, childParentData.offset + paintOffset);
                }
            }, clipBehavior: this.clipBehavior, oldLayer: ((global::Doroti.Framework.Rendering.LayerHandle<global::Doroti.Framework.Rendering.ClipRectLayer>)this._clipRectLayer).layer);
        }
        else
        {
            this._clipRectLayer.layer = null;
            foreach (global::Doroti.Framework.Rendering.RenderBox childLocal in _childrenInPaintOrder())
            {
                var childParentDataLocal = ((global::Doroti.Framework.Rendering.StackParentData?)(object?)childLocal.parentData!)!;
                context.paintChild(childLocal, (childParentDataLocal.offset + offset));
            }
        }
    }

    public override void dispose()
    {
        this._clipRectLayer.layer = null;
        base.dispose();
    }

    public override void visitChildren(global::System.Action<global::Doroti.Framework.Rendering.RenderObject> visitor)
    {
        global::Doroti.Framework.Rendering.RenderBox? child = this.firstChild;
        while ((child is not null))
        {
            visitor(child);
            var childParentData = ((_TheaterParentData__overlay?)(object?)child.parentData!)!;
            childParentData.visitOverlayPortalChildrenOnOverlayEntry((global::System.Action<global::Doroti.Framework.Rendering.RenderObject>)visitor);
            child = childParentData.nextSibling;
        }
    }

    public override void visitChildrenForSemantics(global::System.Action<global::Doroti.Framework.Rendering.RenderObject> visitor)
    {
        global::Doroti.Framework.Rendering.RenderBox? child = this._firstOnstageChild;
        while ((child is not null))
        {
            visitor(child);
            var childParentData = ((_TheaterParentData__overlay?)(object?)child.parentData!)!;
            childParentData.visitOverlayPortalChildrenOnOverlayEntry((global::System.Action<global::Doroti.Framework.Rendering.RenderObject>)visitor);
            child = childParentData.nextSibling;
        }
    }

    public override Rect? describeApproximatePaintClip(global::Doroti.Framework.Rendering.RenderObject child)
    {
        switch (this.clipBehavior)
        {
            case Clip.none:
                {
                    return null;
                }
            case Clip.hardEdge:
            case Clip.antiAlias:
            case Clip.antiAliasWithSaveLayer:
                {
                    return (Offset.zero & this.size);
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.IntProperty("skipCount", this.skipCount));
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<global::Doroti.Ui.TextDirection>("textDirection", this.textDirection));
    }

    public override List<global::Doroti.Framework.Foundation.DiagnosticsNode> debugDescribeChildren()
    {
        var offstageChildren = new List<global::Doroti.Framework.Foundation.DiagnosticsNode>();
        var onstageChildren = new List<global::Doroti.Framework.Foundation.DiagnosticsNode>();
        var count = 1L;
        var onstage = false;
        global::Doroti.Framework.Rendering.RenderBox? child = this.firstChild;
        global::Doroti.Framework.Rendering.RenderBox? firstOnstageChild = this._firstOnstageChild;
        while ((child is not null))
        {
            var childParentData = ((_TheaterParentData__overlay?)(object?)child.parentData!)!;
            if ((object.Equals(child, firstOnstageChild)))
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
                offstageChildren.Add(((Diagnosticable)child).toDiagnosticsNode(name: $"offstage {count}", style: global::Doroti.Framework.Foundation.DiagnosticsTreeStyle.offstage));
            }
            var subcount = 1L;
            childParentData.visitOverlayPortalChildrenOnOverlayEntry(((global::System.Action<global::Doroti.Framework.Rendering.RenderObject>)((renderObject) =>
            {
                var childLocal = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)renderObject)!;
                if (onstage)
                {
                    onstageChildren.Add(((Diagnosticable)childLocal).toDiagnosticsNode(name: $"onstage {count} - {subcount}"));
                }
                else
                {
                    offstageChildren.Add(((Diagnosticable)childLocal).toDiagnosticsNode(name: $"offstage {count} - {subcount}", style: global::Doroti.Framework.Foundation.DiagnosticsTreeStyle.offstage));
                }
                subcount += 1L;
            })));
            child = childParentData.nextSibling;
            count += 1L;
        }
        return new List<global::Doroti.Framework.Foundation.DiagnosticsNode>();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _debugUltimatePreviousSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData = ((StackParentData?)(object?)child.parentData!)!;
        while ((childParentData.previousSibling is not null))
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(childParentData.previousSibling, child)));
            child = childParentData.previousSibling!;
            childParentData = ((StackParentData?)(object?)child.parentData!)!;
        }
        return (object.Equals(child, equals));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _debugUltimateNextSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData = ((StackParentData?)(object?)child.parentData!)!;
        while ((childParentData.nextSibling is not null))
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(childParentData.nextSibling, child)));
            child = childParentData.nextSibling!;
            childParentData = ((StackParentData?)(object?)child.parentData!)!;
        }
        return (object.Equals(child, equals));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual long childCount => this._childCount;
    public virtual bool debugValidateChild(RenderObject child)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((child is not RenderBox))
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"A {this.GetType()} expected a child of type {typeof(RenderBox)} but received a " + $"child of type {DartRuntimePrimitives.RuntimeType(child)}."), new global::Doroti.Framework.Foundation.ErrorDescription("RenderObjects expect specific types of children because they " + "coordinate with their children during layout and paint. For " + "example, a RenderSliver cannot be the child of a RenderBox because " + "a RenderSliver does not understand the RenderBox layout protocol."), new global::Doroti.Framework.Foundation.ErrorSpacer(), new global::Doroti.Framework.Foundation.DiagnosticsProperty<object?>($"The {this.GetType()} that expected a {typeof(RenderBox)} child was created by", this.debugCreator, style: global::Doroti.Framework.Foundation.DiagnosticsTreeStyle.errorProperty), new global::Doroti.Framework.Foundation.ErrorSpacer(), new global::Doroti.Framework.Foundation.DiagnosticsProperty<object?>($"The {DartRuntimePrimitives.RuntimeType(child)} that did not match the expected child type " + "was created by", ((RenderObject)child).debugCreator, style: global::Doroti.Framework.Foundation.DiagnosticsTreeStyle.errorProperty) }));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _insertIntoChildList(RenderBox child, RenderBox? after = null)
    {
        var childParentData = ((StackParentData?)(object?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => (childParentData.nextSibling is null));
        DartRuntimePrimitives.Assert(() => (childParentData.previousSibling is null));
        this._childCount += 1L;
        DartRuntimePrimitives.Assert(() => (this._childCount > 0L));
        if ((after is null))
        {
            childParentData.nextSibling = this._firstChild;
            if ((this._firstChild is not null))
            {
                var firstChildParentData = ((StackParentData?)(object?)this._firstChild!.parentData!)!;
                firstChildParentData.previousSibling = child;
            }
            this._firstChild = child;
            this._lastChild ??= child;
        }
        else
        {
            DartRuntimePrimitives.Assert(() => (this._firstChild is not null));
            DartRuntimePrimitives.Assert(() => (this._lastChild is not null));
            DartRuntimePrimitives.Assert(() => _debugUltimatePreviousSiblingOf(after, equals: this._firstChild));
            DartRuntimePrimitives.Assert(() => _debugUltimateNextSiblingOf(after, equals: this._lastChild));
            var afterParentData = ((StackParentData?)(object?)after.parentData!)!;
            if ((afterParentData.nextSibling is null))
            {
                DartRuntimePrimitives.Assert(() => (object.Equals(after, this._lastChild)));
                childParentData.previousSibling = after;
                afterParentData.nextSibling = child;
                this._lastChild = child;
            }
            else
            {
                childParentData.nextSibling = afterParentData.nextSibling;
                childParentData.previousSibling = after;
                var childPreviousSiblingParentData = ((StackParentData?)(object?)childParentData.previousSibling!.parentData!)!;
                var childNextSiblingParentData = ((StackParentData?)(object?)childParentData.nextSibling!.parentData!)!;
                childPreviousSiblingParentData.nextSibling = child;
                childNextSiblingParentData.previousSibling = child;
                DartRuntimePrimitives.Assert(() => (object.Equals(afterParentData.nextSibling, child)));
            }
        }
    }

    public virtual void insert(RenderBox child, RenderBox? after = null)
    {
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, this)), () => (object?)"A RenderObject cannot be inserted into itself.");
        DartRuntimePrimitives.Assert(() => (!object.Equals(after, this)), () => (object?)"A RenderObject cannot simultaneously be both the parent and the sibling of another RenderObject.");
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, after)), () => (object?)"A RenderObject cannot be inserted after itself.");
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, this._firstChild)));
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, this._lastChild)));
        adoptChild(child);
        DartRuntimePrimitives.Assert(() => (child.parentData is StackParentData), () => (object?)$"A child of {this.GetType()} has parentData of type {DartRuntimePrimitives.RuntimeType(child.parentData)}, " + $"which does not conform to {typeof(StackParentData)}. Class using ContainerRenderObjectMixin " + $"should override setupParentData() to set parentData to type {typeof(StackParentData)}.");
        _insertIntoChildList(child, after: after);
    }

    public virtual void add(RenderBox child)
    {
        insert(child, after: this._lastChild);
    }

    public virtual void addAll(List<RenderBox>? children)
    {
        children?.forEach((__arg0) => ((global::System.Action<RenderBox>)this.add)(__arg0));
    }

    public virtual void _removeFromChildList(RenderBox child)
    {
        var childParentData = ((StackParentData?)(object?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => _debugUltimatePreviousSiblingOf(child, equals: this._firstChild));
        DartRuntimePrimitives.Assert(() => _debugUltimateNextSiblingOf(child, equals: this._lastChild));
        DartRuntimePrimitives.Assert(() => (this._childCount >= 0L));
        if ((childParentData.previousSibling is null))
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(this._firstChild, child)));
            this._firstChild = childParentData.nextSibling;
        }
        else
        {
            var childPreviousSiblingParentData = ((StackParentData?)(object?)childParentData.previousSibling!.parentData!)!;
            childPreviousSiblingParentData.nextSibling = childParentData.nextSibling;
        }
        if ((childParentData.nextSibling is null))
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(this._lastChild, child)));
            this._lastChild = childParentData.previousSibling;
        }
        else
        {
            var childNextSiblingParentData = ((StackParentData?)(object?)childParentData.nextSibling!.parentData!)!;
            childNextSiblingParentData.previousSibling = childParentData.previousSibling;
        }
        childParentData.previousSibling = null;
        childParentData.nextSibling = null;
        this._childCount -= 1L;
    }

    public virtual void remove(RenderBox child)
    {
        _removeFromChildList(child);
        dropChild(child);
    }

    public virtual void removeAll()
    {
        RenderBox? child = this._firstChild;
        while ((child is not null))
        {
            var childParentData = ((StackParentData?)(object?)child.parentData!)!;
            RenderBox? next = childParentData.nextSibling;
            childParentData.previousSibling = null;
            childParentData.nextSibling = null;
            dropChild(child);
            child = next;
        }
        this._firstChild = null;
        this._lastChild = null;
        this._childCount = 0L;
    }

    public virtual void move(RenderBox child, RenderBox? after = null)
    {
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, this)));
        DartRuntimePrimitives.Assert(() => (!object.Equals(after, this)));
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, after)));
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        var childParentData = ((StackParentData?)(object?)child.parentData!)!;
        if ((object.Equals(childParentData.previousSibling, after)))
        {
            return;
        }
        _removeFromChildList(child);
        _insertIntoChildList(child, after: after);
        markNeedsLayout();
    }

    public virtual RenderBox? firstChild => this._firstChild;
    public virtual RenderBox? lastChild => this._lastChild;
    public virtual RenderBox? childBefore(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        var childParentData = ((StackParentData?)(object?)child.parentData!)!;
        return childParentData.previousSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RenderBox? childAfter(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        var childParentData = ((StackParentData?)(object?)child.parentData!)!;
        return childParentData.nextSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDistanceToActualBaseline(TextBaseline baseline)
    {
        DartRuntimePrimitives.Assert(() => !this.debugNeedsLayout);
        global::Doroti.Framework.Rendering.BaselineOffset baselineOffset = global::Doroti.Framework.Rendering.BaselineOffset.noBaseline;
        foreach (global::Doroti.Framework.Rendering.RenderBox child in _childrenInPaintOrder())
        {
            DartRuntimePrimitives.Assert(() => !child.debugNeedsLayout);
            var childParentData = ((global::Doroti.Framework.Rendering.StackParentData?)(object?)child.parentData!)!;
            baselineOffset = baselineOffset.minOf((new global::Doroti.Framework.Rendering.BaselineOffset(child.getDistanceToActualBaseline(baseline)).op_Add(childParentData.offset.dy)));
        }
        return baselineOffset.offset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void layoutChild(global::Doroti.Framework.Rendering.RenderBox child, global::Doroti.Framework.Rendering.BoxConstraints nonPositionedChildConstraints)
    {
        var childParentData = ((global::Doroti.Framework.Rendering.StackParentData?)(object?)child.parentData!)!;
        global::Doroti.Framework.Painting.Alignment alignment = ((_RenderTheater__overlay)this.theater)._resolvedAlignment;
        if (!((global::Doroti.Framework.Rendering.StackParentData)childParentData).isPositioned)
        {
            child.layout(nonPositionedChildConstraints, parentUsesSize: true);
            childParentData.offset = Offset.zero;
        }
        else
        {
            DartRuntimePrimitives.Assert(() => (child is not _RenderDeferredLayoutBox__overlay), () => (object?)"all _RenderDeferredLayoutBoxes must be non-positioned children.");
            RenderStack.layoutPositionedChild(child, childParentData, this.size, alignment);
        }
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parentData, childParentData)));
    }

    public override bool hitTestChildren(global::Doroti.Framework.Rendering.BoxHitTestResult result, Offset position)
    {
        IEnumerator<global::Doroti.Framework.Rendering.RenderBox> iterator = _childrenInHitTestOrder().GetEnumerator();
        var isHit = false;
        while ((!isHit && iterator.MoveNext()))
        {
            global::Doroti.Framework.Rendering.RenderBox child = iterator.Current;
            var childParentData = ((global::Doroti.Framework.Rendering.StackParentData?)(object?)child.parentData!)!;
            var localChild = child;
            bool childHitTest(global::Doroti.Framework.Rendering.BoxHitTestResult result, Offset position)
            {
                return localChild.hitTest(result, position: position);
                throw new InvalidOperationException("Dart control flow completed without a value.");
            }
            isHit = result.addWithPaintOffset(offset: childParentData.offset, position: position, hitTest: (global::System.Func<global::Doroti.Framework.Rendering.BoxHitTestResult, Offset, bool>)childHitTest);
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
    internal static long _wallTime = (global::Doroti.Framework.Foundation.ConstantsLibrary.kIsWeb ? -9007199254740992L : (-1L << (int)(63L)));

    public OverlayPortalController(string? debugLabel = null)
    {
        this._debugLabel = debugLabel;
    }

    internal virtual long _now()
    {
        long now = _wallTime += 1L;
        DartRuntimePrimitives.Assert(() => ((this._zOrderIndex is null) || (DartRuntimePrimitives.RequireValue(this._zOrderIndex) < now)));
        DartRuntimePrimitives.Assert(() => ((this._attachTarget?._zOrderIndex is null) || (DartRuntimePrimitives.RequireValue(this._attachTarget!._zOrderIndex) < now)));
        return now;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void show()
    {
        _OverlayPortalState__overlay? state = this._attachTarget;
        if ((state is not null))
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
        _OverlayPortalState__overlay? state = this._attachTarget;
        if ((state is not null))
        {
            state.hide();
        }
        else
        {
            DartRuntimePrimitives.Assert(() => (this._zOrderIndex is not null));
            _zOrderIndex = null;
        }
    }

    public virtual bool isShowing
    {
        get
        {
            _OverlayPortalState__overlay? state = this._attachTarget;
            return ((state is not null) ? (((_OverlayPortalState__overlay)state)._zOrderIndex is not null) : (this._zOrderIndex is not null));
            return default!;
        }
    }
    public virtual void toggle() => ((Action)(() => { if (this.isShowing) { hide(); } else { show(); } }))();
    public override string ToString()
    {
        string? debugLabel = this._debugLabel;
        var label = ((debugLabel is null) ? "" : $"({debugLabel})");
        var isDetached = ((this._attachTarget is not null) ? "" : " DETACHED");
        return $"{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "OverlayPortalController"))}{label}{isDetached}";
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
    public virtual global::System.Func<BuildContext, Widget> overlayChildBuilder { get; private set; } = default!;
    public virtual Widget? child { get; private set; }
    public virtual OverlayChildLocation overlayLocation { get; private set; } = default!;

    public OverlayPortal(global::Doroti.Framework.Foundation.Key? key = null, OverlayPortalController controller = default!, global::System.Func<BuildContext, Widget> overlayChildBuilder = default!, OverlayChildLocation overlayLocation = OverlayChildLocation.nearestOverlay, Widget? child = null) : base(key: key)
    {
        this.controller = controller;
        this.overlayChildBuilder = overlayChildBuilder;
        this.overlayLocation = overlayLocation;
        this.child = child;
    }

    public static OverlayPortal CreateTargetsRootOverlay(global::Doroti.Framework.Foundation.Key? key = null, OverlayPortalController controller = default!, global::System.Func<BuildContext, Widget> overlayChildBuilder = default!, Widget? child = null)
    {
        var __instance = new OverlayPortal(default!, default!, default!, default!, default!);
        __instance.controller = controller;
        __instance.overlayChildBuilder = overlayChildBuilder;
        __instance.child = child;
        __instance.overlayLocation = OverlayChildLocation.rootOverlay;
        return __instance;
    }

    public static OverlayPortal CreateOverlayChildLayoutBuilder(global::Doroti.Framework.Foundation.Key? key = null, OverlayPortalController controller = default!, global::System.Func<BuildContext, OverlayChildLayoutInfo, Widget> overlayChildBuilder = default!, OverlayChildLocation overlayLocation = OverlayChildLocation.nearestOverlay, Widget? child = default!)
    {
        return new OverlayPortal(key: key, controller: controller, overlayChildBuilder: ((global::System.Func<BuildContext, Widget>)((_) => new _OverlayChildLayoutBuilder__overlay(builder: (global::System.Func<BuildContext, OverlayChildLayoutInfo, Widget>)overlayChildBuilder))), child: child, overlayLocation: overlayLocation);
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
        return ((object.Equals(((_OverlayEntryLocation__overlay)locationCache)._childModel, ((_RenderTheaterMarker__overlay)marker).overlayEntryWidgetState)) && (object.Equals(((_OverlayEntryLocation__overlay)locationCache)._theater, ((_RenderTheaterMarker__overlay)marker).theater)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual _OverlayEntryLocation__overlay _getLocation(long zOrderIndex, OverlayChildLocation overlayLocation)
    {
        _OverlayEntryLocation__overlay? cachedLocation = this._locationCache;
        _RenderTheaterMarker__overlay marker = ((_RenderTheaterMarker__overlay)(object?)_RenderTheaterMarker__overlay.of(this.context, targetRootOverlay: (object.Equals(overlayLocation, OverlayChildLocation.rootOverlay))));
        bool isCacheValid = ((cachedLocation is not null) && ((!this._childModelMayHaveChanged || _OverlayPortalState__overlay._isTheSameLocation(cachedLocation, marker))));
        _childModelMayHaveChanged = false;
        if (isCacheValid)
        {
            DartRuntimePrimitives.Assert(() => (((_OverlayEntryLocation__overlay)cachedLocation)._zOrderIndex == zOrderIndex));
            DartRuntimePrimitives.Assert(() => cachedLocation._debugIsLocationValid());
            return cachedLocation;
        }
        cachedLocation?._debugMarkLocationInvalid();
        var newLocation = new _OverlayEntryLocation__overlay(zOrderIndex, ((_RenderTheaterMarker__overlay)marker).overlayEntryWidgetState, ((_RenderTheaterMarker__overlay)marker).theater);
        DartRuntimePrimitives.Assert(() => (((_OverlayEntryLocation__overlay)newLocation)._zOrderIndex == zOrderIndex));
        return _locationCache = newLocation;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void initState()
    {
        base.initState();
        _setupController(((OverlayPortal)this.widget).controller);
    }

    internal virtual void _setupController(OverlayPortalController controller)
    {
        DartRuntimePrimitives.Assert(() => ((object.Equals(((OverlayPortalController)controller)._attachTarget, this)) || !(((((StatefulElement?)(object?)((OverlayPortalController)controller)._attachTarget?.context)!)?.debugIsActive ?? false))), () => (object?)$"Failed to attach {controller} to {this}. It is already attached to {((OverlayPortalController)controller)._attachTarget}.");
        long? controllerZOrderIndex = ((OverlayPortalController)controller)._zOrderIndex;
        long? zOrderIndex = this._zOrderIndex;
        if (((zOrderIndex is null) || (((controllerZOrderIndex is not null) && (DartRuntimePrimitives.RequireValue(controllerZOrderIndex) > DartRuntimePrimitives.RequireValue(zOrderIndex))))))
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
        _childModelMayHaveChanged = (this._childModelMayHaveChanged || (!object.Equals(((OverlayPortal)oldWidget).overlayLocation, ((OverlayPortal)this.widget).overlayLocation)));
        if ((!object.Equals(((OverlayPortal)oldWidget).controller, ((OverlayPortal)this.widget).controller)))
        {
            ((OverlayPortal)oldWidget).controller._attachTarget = null;
            _setupController(((OverlayPortal)this.widget).controller);
        }
    }

    public override void activate()
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(((OverlayPortal)this.widget).controller._attachTarget, this)));
        base.activate();
    }

    public override void dispose()
    {
        ((OverlayPortal)this.widget).controller._attachTarget = null;
        this._locationCache?._debugMarkLocationInvalid();
        _locationCache = null;
        base.dispose();
    }

    public virtual void show(long zOrderIndex)
    {
        DartRuntimePrimitives.Assert(() => (!object.Equals(global::Doroti.Framework.Scheduler.SchedulerBinding.instance.schedulerPhase, global::Doroti.Framework.Scheduler.SchedulerPhase.persistentCallbacks)), () => (object?)$"{DartRuntimePrimitives.RuntimeType(((OverlayPortal)this.widget).controller)}.show() should not be called during build.");
        setState(((global::System.Action)(() =>
        {
            _zOrderIndex = zOrderIndex;
        })));
        this._locationCache?._debugMarkLocationInvalid();
        _locationCache = null;
    }

    public virtual void hide()
    {
        DartRuntimePrimitives.Assert(() => (!object.Equals(global::Doroti.Framework.Scheduler.SchedulerBinding.instance.schedulerPhase, global::Doroti.Framework.Scheduler.SchedulerPhase.persistentCallbacks)));
        setState(((global::System.Action)(() =>
        {
            _zOrderIndex = null;
        })));
        this._locationCache?._debugMarkLocationInvalid();
        _locationCache = null;
    }

    public override Widget build(BuildContext context)
    {
        long? zOrderIndex = this._zOrderIndex;
        if ((zOrderIndex is null))
        {
            return ((Widget)(object?)new _OverlayPortal__overlay(overlayLocation: ((_OverlayEntryLocation__overlay)(object)null), overlayChild: ((Widget)(object)null), child: new Semantics(traversalParentIdentifier: this, child: ((OverlayPortal)this.widget).child)));
        }
        _OverlayEntryLocation__overlay overlayLocationLocal = ((_OverlayEntryLocation__overlay)(object?)_getLocation(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(zOrderIndex)), ((OverlayPortal)this.widget).overlayLocation));
        MediaQueryData overlayData = ((MediaQueryData)(object?)MediaQuery.of(((_OverlayEntryLocation__overlay)overlayLocationLocal)._childModel.context));
        MediaQueryData dataLocal = ((MediaQueryData)(object?)MediaQuery.of(context).copyWith(padding: ((MediaQueryData)overlayData).padding, viewInsets: ((MediaQueryData)overlayData).viewInsets, viewPadding: ((MediaQueryData)overlayData).viewPadding));
        return ((Widget)(object?)new _OverlayPortal__overlay(overlayLocation: overlayLocationLocal, overlayChild: new _DeferredLayout__overlay(childIdentifier: this, child: new MediaQuery(data: dataLocal, child: new Builder(builder: (global::System.Func<BuildContext, Widget>)((OverlayPortal)this.widget).overlayChildBuilder))), child: new Semantics(traversalParentIdentifier: this, child: ((OverlayPortal)this.widget).child)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _OverlayEntryLocation__overlay : DartLinkedListEntry<_OverlayEntryLocation__overlay>
{
    internal virtual long _zOrderIndex { get; private set; } = default!;
    internal virtual _OverlayEntryWidgetState__overlay _childModel { get; private set; } = default!;
    internal virtual _RenderTheater__overlay _theater { get; private set; } = default!;
    internal virtual _RenderDeferredLayoutBox__overlay? _overlayChildRenderBox { get; set; } = default;
    internal virtual global::System.Diagnostics.StackTrace? _debugMarkLocationInvalidStackTrace { get; set; } = default;

    internal _OverlayEntryLocation__overlay(long _zOrderIndex, _OverlayEntryWidgetState__overlay _childModel, _RenderTheater__overlay _theater)
    {
        this._zOrderIndex = _zOrderIndex;
        this._childModel = _childModel;
        this._theater = _theater;
    }

    internal virtual void _addToChildModel(_RenderDeferredLayoutBox__overlay child)
    {
        DartRuntimePrimitives.Assert(() => (this._overlayChildRenderBox is null), () => (object?)$"Failed to add {child}. This location ({this}) is already occupied by {this._overlayChildRenderBox}.");
        _overlayChildRenderBox = child;
        this._childModel._add(this);
        this._theater.markNeedsPaint();
        this._theater.markNeedsCompositingBitsUpdate();
        this._theater.markNeedsSemanticsUpdate();
    }

    internal virtual void _removeFromChildModel(_RenderDeferredLayoutBox__overlay child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child, this._overlayChildRenderBox)));
        _overlayChildRenderBox = null;
        DartRuntimePrimitives.Assert(() => (((_OverlayEntryWidgetState__overlay)this._childModel)._sortedTheaterSiblings?.contains(this) ?? false));
        this._childModel._remove(this);
        this._theater.markNeedsPaint();
        this._theater.markNeedsCompositingBitsUpdate();
        this._theater.markNeedsSemanticsUpdate();
    }

    internal virtual void _addChild(_RenderDeferredLayoutBox__overlay child)
    {
        DartRuntimePrimitives.Assert(() => _debugIsLocationValid());
        _addToChildModel(child);
        this._theater._addDeferredChild(child);
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this._theater)));
    }

    internal virtual void _removeChild(_RenderDeferredLayoutBox__overlay child)
    {
        _removeFromChildModel(child);
        this._theater._removeDeferredChild(child);
        DartRuntimePrimitives.Assert(() => (child.parent is null));
    }

    internal virtual void _moveChild(_RenderDeferredLayoutBox__overlay child, _OverlayEntryLocation__overlay fromLocation)
    {
        DartRuntimePrimitives.Assert(() => (!object.Equals(fromLocation, this)));
        DartRuntimePrimitives.Assert(() => _debugIsLocationValid());
        _RenderTheater__overlay fromTheater = ((_OverlayEntryLocation__overlay)fromLocation)._theater;
        _OverlayEntryWidgetState__overlay fromModel = ((_OverlayEntryLocation__overlay)fromLocation)._childModel;
        if ((!object.Equals(fromTheater, this._theater)))
        {
            fromTheater._removeDeferredChild(child);
            this._theater._addDeferredChild(child);
        }
        if (((!object.Equals(fromModel, this._childModel)) || (((_OverlayEntryLocation__overlay)fromLocation)._zOrderIndex != this._zOrderIndex)))
        {
            fromLocation._removeFromChildModel(child);
            _addToChildModel(child);
        }
    }

    internal virtual void _reattachFromLayoutSurrogate(_RenderDeferredLayoutBox__overlay child)
    {
        DartRuntimePrimitives.Assert(() => (this._overlayChildRenderBox is null), () => (object?)$"{this} failed to reattach: _detachFromLayoutSurrogate must be called before _reattachFromLayoutSurrogate.");
        this._theater._addDeferredChild(child);
        _overlayChildRenderBox = child;
    }

    internal virtual void _detachFromLayoutSurrogate(_RenderDeferredLayoutBox__overlay child)
    {
        this._theater._removeDeferredChild(child);
        _overlayChildRenderBox = null;
    }

    internal virtual bool _debugIsLocationValid()
    {
        if ((this._debugMarkLocationInvalidStackTrace is null))
        {
            return true;
        }
        throw new InvalidOperationException($"{this} is already disposed. Stack trace: {this._debugMarkLocationInvalidStackTrace}");
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _debugMarkLocationInvalid()
    {
        DartRuntimePrimitives.Assert(() => _debugIsLocationValid());
        DartRuntimePrimitives.Assert(() =>
            {
                _debugMarkLocationInvalidStackTrace = new global::System.Diagnostics.StackTrace(true);
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
    }

    public override string ToString() => $"{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "_OverlayEntryLocation"))}[{(global::Doroti.Framework.Foundation.DiagnosticsLibrary.shortHash(this))}] {((this._debugMarkLocationInvalidStackTrace is not null) ? "(INVALID)" : "")}";
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
        var __oldWidget = (_RenderTheaterMarker__overlay)(object)oldWidget;
        return ((!object.Equals(((_RenderTheaterMarker__overlay)__oldWidget).theater, this.theater)) || (!object.Equals(((_RenderTheaterMarker__overlay)__oldWidget).overlayEntryWidgetState, this.overlayEntryWidgetState)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static _RenderTheaterMarker__overlay of(BuildContext context, bool targetRootOverlay = false)
    {
        _RenderTheaterMarker__overlay? marker = ((_RenderTheaterMarker__overlay?)(object?)_RenderTheaterMarker__overlay.maybeOf(context, targetRootOverlay: targetRootOverlay));
        if ((marker is not null))
        {
            return marker;
        }
        throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("No Overlay widget found."), new global::Doroti.Framework.Foundation.ErrorDescription($"{DartRuntimePrimitives.RuntimeType(((BuildContext)context).widget)} widgets require an Overlay widget ancestor.\n" + "An overlay lets widgets float on top of other widget children."), new global::Doroti.Framework.Foundation.ErrorHint("To introduce an Overlay widget, you can either directly " + "include one, or use a widget that contains an Overlay itself, " + "such as a Navigator, WidgetApp, MaterialApp, or CupertinoApp.") }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static _RenderTheaterMarker__overlay? maybeOf(BuildContext context, bool targetRootOverlay = false, bool createDependency = true)
    {
        if (targetRootOverlay)
        {
            InheritedElement? ancestor = ((InheritedElement?)(object?)_RenderTheaterMarker__overlay._rootRenderTheaterMarkerOf(LookupBoundary.getElementForInheritedWidgetOfExactType<_RenderTheaterMarker__overlay>(context)));
            DartRuntimePrimitives.Assert(() => ((ancestor is null) || (ancestor.widget is _RenderTheaterMarker__overlay)));
            if ((ancestor is null))
            {
                return ((_RenderTheaterMarker__overlay)(object)null);
            }
            if (createDependency)
            {
                return ((_RenderTheaterMarker__overlay?)(object?)context.dependOnInheritedElement(ancestor))!;
            }
            return ((_RenderTheaterMarker__overlay?)(object?)ancestor.widget)!;
        }
        if (createDependency)
        {
            return ((_RenderTheaterMarker__overlay?)(object?)LookupBoundary.dependOnInheritedWidgetOfExactType<_RenderTheaterMarker__overlay>(context));
        }
        return ((_RenderTheaterMarker__overlay?)(object?)LookupBoundary.getInheritedWidgetOfExactType<_RenderTheaterMarker__overlay>(context));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static InheritedElement? _rootRenderTheaterMarkerOf(InheritedElement? theaterMarkerElement)
    {
        DartRuntimePrimitives.Assert(() => ((theaterMarkerElement is null) || (theaterMarkerElement.widget is _RenderTheaterMarker__overlay)));
        if ((theaterMarkerElement is null))
        {
            return ((InheritedElement)(object)null);
        }
        InheritedElement? ancestor = default!;
        theaterMarkerElement.visitAncestorElements(((global::System.Func<Element, bool>)((element) =>
        {
            ancestor = LookupBoundary.getElementForInheritedWidgetOfExactType<_RenderTheaterMarker__overlay>(element);
            return false;
            throw new InvalidOperationException("Dart closure completed without a value.");
        })));
        return ((ancestor is null) ? theaterMarkerElement : _RenderTheaterMarker__overlay._rootRenderTheaterMarkerOf(ancestor));
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
        System.Diagnostics.Debug.Assert(((overlayChild is null) || (overlayLocation is not null)));
        System.Diagnostics.Debug.Assert(((overlayLocation is null) || overlayLocation._debugIsLocationValid()));
    }

    public override RenderObjectElement createElement() => DartRuntimePrimitives.ConvertValue<RenderObjectElement>(new _OverlayPortalElement__overlay(this));
    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context) => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.RenderObject>(new _RenderLayoutSurrogateProxyBox__overlay(this.overlayLocation));
    public override void updateRenderObject(BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderLayoutSurrogateProxyBox__overlay)(object)renderObject;
        __renderObject.overlayLocation = this.overlayLocation;
    }

}

internal class _OverlayPortalElement__overlay : RenderObjectElement
{
    internal virtual Element? _overlayChild { get; set; } = default;
    internal virtual Element? _child { get; set; } = default;

    internal _OverlayPortalElement__overlay(_OverlayPortal__overlay widget) : base(widget)
    {
    }

    public override global::Doroti.Framework.Rendering.RenderObject renderObject => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.RenderObject>(((_RenderLayoutSurrogateProxyBox__overlay?)(object?)base.renderObject)!);
    public override void mount(Element? parent, object? newSlot)
    {
        base.mount(parent, newSlot);
        var widgetLocal = ((_OverlayPortal__overlay?)(object?)this.widget)!;
        _child = updateChild(this._child, ((_OverlayPortal__overlay)widgetLocal).child, null);
        _overlayChild = updateChild(this._overlayChild, ((_OverlayPortal__overlay)widgetLocal).overlayChild, ((_OverlayPortal__overlay)widgetLocal).overlayLocation);
    }

    public override void update(Widget newWidget)
    {
        var __newWidget = (_OverlayPortal__overlay)(object)newWidget;
        base.update(__newWidget);
        _child = updateChild(this._child, ((_OverlayPortal__overlay)__newWidget).child, null);
        _overlayChild = updateChild(this._overlayChild, ((_OverlayPortal__overlay)__newWidget).overlayChild, ((_OverlayPortal__overlay)__newWidget).overlayLocation);
    }

    public override void forgetChild(Element child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child, this._child)));
        _child = null;
        base.forgetChild(child);
    }

    public override void visitChildren(global::System.Action<Element> visitor)
    {
        Element? child = this._child;
        Element? overlayChild = this._overlayChild;
        if ((child is not null))
        {
            visitor(child);
        }
        if ((overlayChild is not null))
        {
            visitor(overlayChild);
        }
    }

    public override void insertRenderObjectChild(global::Doroti.Framework.Rendering.RenderObject child, object? slot)
    {
        var __child = (global::Doroti.Framework.Rendering.RenderBox)(object)child;
        var __slot = slot is null ? null : (_OverlayEntryLocation__overlay)(object)slot;
        DartRuntimePrimitives.Assert(() => (__child.parent is null), () => (object?)$"{__child}'s parent is not null: {__child.parent}");
        if ((__slot is not null))
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(((_RenderLayoutSurrogateProxyBox__overlay)this.renderObject)._deferredLayoutChild, __child)));
            __slot._addChild(((_RenderDeferredLayoutBox__overlay?)(object?)__child)!);
            this.renderObject.markNeedsSemanticsUpdate();
        }
        else
        {
            ((dynamic)this.renderObject).child = __child;
        }
    }

    public override void moveRenderObjectChild(global::Doroti.Framework.Rendering.RenderObject child, object? oldSlot, object? newSlot)
    {
        var __child = (_RenderDeferredLayoutBox__overlay)(object)child;
        var __oldSlot = (_OverlayEntryLocation__overlay)(object)oldSlot;
        var __newSlot = (_OverlayEntryLocation__overlay)(object)newSlot;
        DartRuntimePrimitives.Assert(() => __newSlot._debugIsLocationValid());
        ((dynamic)__newSlot)._moveChild(__child, __oldSlot);
        this.renderObject.markNeedsSemanticsUpdate();
    }

    public override void removeRenderObjectChild(global::Doroti.Framework.Rendering.RenderObject child, object? slot)
    {
        var __child = (global::Doroti.Framework.Rendering.RenderBox)(object)child;
        var __slot = slot is null ? null : (_OverlayEntryLocation__overlay)(object)slot;
        if ((__slot is null))
        {
            ((dynamic)this.renderObject).child = null;
            return;
        }
        DartRuntimePrimitives.Assert(() => (object.Equals(((_RenderLayoutSurrogateProxyBox__overlay)this.renderObject)._deferredLayoutChild, __child)));
        ((dynamic)__slot)._removeChild(((_RenderDeferredLayoutBox__overlay?)(object?)__child)!);
        ((dynamic)this.renderObject)._deferredLayoutChild = null;
        this.renderObject.markNeedsSemanticsUpdate();
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<Element>("child", this._child, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<Element>("overlayChild", this._overlayChild, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<object>("overlayLocation", this._overlayChild?.slot, defaultValue: null));
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

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        _RenderLayoutSurrogateProxyBox__overlay parent = ((_RenderLayoutSurrogateProxyBox__overlay)(object?)getLayoutParent(context));
        var renderObject = new _RenderDeferredLayoutBox__overlay(parent, this.childIdentifier);
        ((dynamic)parent)._deferredLayoutChild = renderObject;
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)renderObject);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderDeferredLayoutBox__overlay)(object)renderObject;
        DartRuntimePrimitives.Assert(() => (object.Equals(((_RenderDeferredLayoutBox__overlay)__renderObject)._layoutSurrogate, getLayoutParent(context))));
        DartRuntimePrimitives.Assert(() => (object.Equals(((_RenderDeferredLayoutBox__overlay?)((dynamic)getLayoutParent(context))._deferredLayoutChild), __renderObject)));
        __renderObject.childIdentifier = this.childIdentifier;
    }

}

public class _RenderDeferredLayoutBox__overlay : global::Doroti.Framework.Rendering.RenderProxyBox, _RenderTheaterMixin__overlay
{
    internal virtual _RenderLayoutSurrogateProxyBox__overlay _layoutSurrogate { get; private set; } = default!;
    internal virtual object? _childIdentifier { get; set; } = default;
    internal virtual bool _needsLayout { get; set; } = true;
    internal virtual bool _doingLayoutFromTreeWalk { get; set; } = false;
    internal virtual bool _debugMutationsLocked { get; set; } = false;

    internal _RenderDeferredLayoutBox__overlay(_RenderLayoutSurrogateProxyBox__overlay _layoutSurrogate, object? childIdentifier)
    {
        this._layoutSurrogate = _layoutSurrogate;
        this._childIdentifier = childIdentifier;
    }

    public virtual global::Doroti.Framework.Rendering.StackParentData stackParentData => ((global::Doroti.Framework.Rendering.StackParentData?)(object?)this.parentData!)!;
    public virtual object? childIdentifier
    {
        get => this._childIdentifier;
        set
        {
            var __value = value;
            if ((object.Equals(this._childIdentifier, __value)))
            {
                return;
            }
            _childIdentifier = __value;
        }
    }
    public virtual IEnumerable<global::Doroti.Framework.Rendering.RenderBox> _childrenInPaintOrder()
    {
        global::Doroti.Framework.Rendering.RenderBox? childLocal = ((global::Doroti.Framework.Rendering.RenderBox?)((dynamic)this).child);
        return ((childLocal is null) ? System.Linq.Enumerable.Empty<global::Doroti.Framework.Rendering.RenderBox>() : System.Linq.Enumerable.Range(0, checked((int)1L)).Select(__index => ((Func<long, global::Doroti.Framework.Rendering.RenderBox>)((i) => childLocal))(checked((long)__index))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual IEnumerable<global::Doroti.Framework.Rendering.RenderBox> _childrenInHitTestOrder() => _childrenInPaintOrder();
    public virtual _RenderTheater__overlay theater => (this.parent switch { _RenderTheater__overlay parentLocal => parentLocal, _ => throw DartRuntimePrimitives.AsException(global::Doroti.Framework.Foundation.FlutterError.Create($"{this.parent} of {this} is not a _RenderTheater")) });
    public override void redepthChildren()
    {
        if (this._layoutSurrogate.attached)
        {
            this._layoutSurrogate.redepthChild(this);
        }
        base.redepthChildren();
    }

    public override bool sizedByParent => true;
    public virtual bool needsLayout
    {
        get
        {
            DartRuntimePrimitives.Assert(() => (this.debugNeedsLayout == this._needsLayout));
            return this._needsLayout;
            return default!;
        }
    }
    public override void markNeedsLayout()
    {
        _needsLayout = true;
        base.markNeedsLayout();
    }

    public override double? computeDryBaseline(global::Doroti.Framework.Rendering.BoxConstraints constraints, TextBaseline baseline)
    {
        global::Doroti.Framework.Rendering.RenderBox? childLocal = ((global::Doroti.Framework.Rendering.RenderBox?)((dynamic)this).child);
        if ((childLocal is null))
        {
            return null;
        }
        return _RenderTheaterMixin__overlay.baselineForChild(childLocal, ((global::Doroti.Framework.Rendering.BoxConstraints)constraints).biggest, constraints, ((_RenderTheater__overlay)this.theater)._resolvedAlignment, baseline);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Rendering.RenderObject? debugLayoutParent => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.RenderObject>(this._layoutSurrogate);
    internal virtual void _doLayoutFrom(global::Doroti.Framework.Rendering.RenderObject treewalkParent, global::Doroti.Framework.Rendering.Constraints constraints)
    {
        bool shouldAddToDirtyList = (this.needsLayout || (!object.Equals(this.constraints, constraints)));
        DartRuntimePrimitives.Assert(() => !this._doingLayoutFromTreeWalk);
        _doingLayoutFromTreeWalk = true;
        base.layout(constraints);
        DartRuntimePrimitives.Assert(() => this._doingLayoutFromTreeWalk);
        _doingLayoutFromTreeWalk = false;
        _needsLayout = false;
        DartRuntimePrimitives.Assert(() => !this.debugNeedsLayout);
        if (shouldAddToDirtyList)
        {
            ((dynamic)treewalkParent).invokeLayoutCallback(((global::System.Action<global::Doroti.Framework.Rendering.BoxConstraints>)((_) =>
            {
                markNeedsLayout();
            })));
        }
    }

    public override void layout(global::Doroti.Framework.Rendering.Constraints constraints, bool parentUsesSize = false)
    {
        _doLayoutFrom(this.parent!, constraints: constraints);
    }

    public override void performResize()
    {
        size = ((global::Doroti.Framework.Rendering.BoxConstraints)this.constraints).biggest;
    }

    public override void performLayout()
    {
        DartRuntimePrimitives.Assert(() => !this._debugMutationsLocked);
        if (this._doingLayoutFromTreeWalk)
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
        DartRuntimePrimitives.Assert(() => (this.parent is not null));
        global::Doroti.Framework.Rendering.RenderBox? childLocal = ((global::Doroti.Framework.Rendering.RenderBox?)((dynamic)this).child);
        if ((childLocal is null))
        {
            _needsLayout = false;
            return;
        }
        DartRuntimePrimitives.Assert(() => ((global::Doroti.Framework.Rendering.BoxConstraints)this.constraints).isTight);
        layoutChild(childLocal, this.constraints);
        DartRuntimePrimitives.Assert(() =>
            {
                _debugMutationsLocked = false;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        _needsLayout = false;
    }

    public override void describeSemanticsConfiguration(global::Doroti.Framework.Semantics.SemanticsConfiguration config)
    {
        base.describeSemanticsConfiguration(config);
        if ((this.childIdentifier is not null))
        {
            config.traversalChildIdentifier = this.childIdentifier;
        }
    }

    public override void applyPaintTransform(global::Doroti.Framework.Rendering.RenderObject child, Matrix4 transform)
    {
        var __child = (global::Doroti.Framework.Rendering.RenderBox)(object)child;
        var childParentData = ((global::Doroti.Framework.Rendering.BoxParentData?)(object?)__child.parentData!)!;
        global::Doroti.Ui.Offset offsetLocal = ((global::Doroti.Ui.Offset)(object?)((global::Doroti.Framework.Rendering.BoxParentData)childParentData).offset);
        transform.translateByDouble(offsetLocal.dx, offsetLocal.dy, 0, 1);
    }

    public override void setupParentData(global::Doroti.Framework.Rendering.RenderObject child)
    {
        var __child = (global::Doroti.Framework.Rendering.RenderBox)(object)child;
        if ((__child.parentData is not global::Doroti.Framework.Rendering.StackParentData))
        {
            __child.parentData = new global::Doroti.Framework.Rendering.StackParentData();
        }
    }

    public override double? computeDistanceToActualBaseline(TextBaseline baseline)
    {
        DartRuntimePrimitives.Assert(() => !this.debugNeedsLayout);
        global::Doroti.Framework.Rendering.BaselineOffset baselineOffset = global::Doroti.Framework.Rendering.BaselineOffset.noBaseline;
        foreach (global::Doroti.Framework.Rendering.RenderBox child in _childrenInPaintOrder())
        {
            DartRuntimePrimitives.Assert(() => !child.debugNeedsLayout);
            var childParentData = ((global::Doroti.Framework.Rendering.StackParentData?)(object?)child.parentData!)!;
            baselineOffset = baselineOffset.minOf((new global::Doroti.Framework.Rendering.BaselineOffset(child.getDistanceToActualBaseline(baseline)).op_Add(childParentData.offset.dy)));
        }
        return baselineOffset.offset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void layoutChild(global::Doroti.Framework.Rendering.RenderBox child, global::Doroti.Framework.Rendering.BoxConstraints nonPositionedChildConstraints)
    {
        var childParentData = ((global::Doroti.Framework.Rendering.StackParentData?)(object?)child.parentData!)!;
        global::Doroti.Framework.Painting.Alignment alignment = ((_RenderTheater__overlay)this.theater)._resolvedAlignment;
        if (!((global::Doroti.Framework.Rendering.StackParentData)childParentData).isPositioned)
        {
            child.layout(nonPositionedChildConstraints, parentUsesSize: true);
            childParentData.offset = Offset.zero;
        }
        else
        {
            DartRuntimePrimitives.Assert(() => (child is not _RenderDeferredLayoutBox__overlay), () => (object?)"all _RenderDeferredLayoutBoxes must be non-positioned children.");
            RenderStack.layoutPositionedChild(child, childParentData, this.size, alignment);
        }
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parentData, childParentData)));
    }

    public override bool hitTestChildren(global::Doroti.Framework.Rendering.BoxHitTestResult result, Offset position)
    {
        IEnumerator<global::Doroti.Framework.Rendering.RenderBox> iterator = _childrenInHitTestOrder().GetEnumerator();
        var isHit = false;
        while ((!isHit && iterator.MoveNext()))
        {
            global::Doroti.Framework.Rendering.RenderBox child = iterator.Current;
            var childParentData = ((global::Doroti.Framework.Rendering.StackParentData?)(object?)child.parentData!)!;
            var localChild = child;
            bool childHitTest(global::Doroti.Framework.Rendering.BoxHitTestResult result, Offset position)
            {
                return localChild.hitTest(result, position: position);
                throw new InvalidOperationException("Dart control flow completed without a value.");
            }
            isHit = result.addWithPaintOffset(offset: childParentData.offset, position: position, hitTest: (global::System.Func<global::Doroti.Framework.Rendering.BoxHitTestResult, Offset, bool>)childHitTest);
        }
        return isHit;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset)
    {
        foreach (global::Doroti.Framework.Rendering.RenderBox child in _childrenInPaintOrder())
        {
            var childParentData = ((global::Doroti.Framework.Rendering.StackParentData?)(object?)child.parentData!)!;
            context.paintChild(child, (childParentData.offset + offset));
        }
    }

}

public class _RenderLayoutSurrogateProxyBox__overlay : global::Doroti.Framework.Rendering.RenderProxyBox
{
    internal virtual _RenderDeferredLayoutBox__overlay? _deferredLayoutChild { get; set; } = default;
    public virtual _OverlayEntryLocation__overlay? overlayLocation { get; set; } = default;
    internal virtual bool _debugIsFirstAttach { get; set; } = true;
    internal virtual bool _didDetachDeferredChild { get; set; } = false;

    internal _RenderLayoutSurrogateProxyBox__overlay(_OverlayEntryLocation__overlay? overlayLocation)
    {
        this.overlayLocation = overlayLocation;
    }

    public override void attach(global::Doroti.Framework.Rendering.PipelineOwner owner)
    {
        base.attach(owner);
        if (this._didDetachDeferredChild)
        {
            _didDetachDeferredChild = false;
            DartRuntimePrimitives.Assert(() => (this._deferredLayoutChild is not null));
            DartRuntimePrimitives.Assert(() => !this._debugIsFirstAttach);
            this.overlayLocation!._reattachFromLayoutSurrogate(this._deferredLayoutChild!);
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
        if (this._deferredLayoutChild is object deferredChild && (((_RenderDeferredLayoutBox__overlay)deferredChild).theater.attached))
        {
            this.overlayLocation!._detachFromLayoutSurrogate(DartRuntimePrimitives.ConvertValue<_RenderDeferredLayoutBox__overlay>(deferredChild));
            _didDetachDeferredChild = true;
        }
        base.detach();
    }

    public override void redepthChildren()
    {
        base.redepthChildren();
        _RenderDeferredLayoutBox__overlay? child = this._deferredLayoutChild;
        if (((child is not null) && child.attached))
        {
            redepthChild(child);
        }
    }

    public override void performLayout()
    {
        base.performLayout();
        _RenderDeferredLayoutBox__overlay? deferredChild = this._deferredLayoutChild;
        if ((deferredChild is null))
        {
            return;
        }
        var theater = ((_RenderTheater__overlay?)(object?)deferredChild.parent!)!;
        if (!((_RenderTheater__overlay)theater)._layingOutSizeDeterminingChild)
        {
            global::Doroti.Framework.Rendering.BoxConstraints theaterConstraints = theater.constraints;
            global::Doroti.Ui.Size boxSize = ((global::Doroti.Ui.Size)(object?)(((global::Doroti.Framework.Rendering.BoxConstraints)theaterConstraints).biggest.isFinite ? ((global::Doroti.Framework.Rendering.BoxConstraints)theaterConstraints).biggest : theater.size));
            deferredChild._doLayoutFrom(this, constraints: global::Doroti.Framework.Rendering.BoxConstraints.CreateTight(boxSize));
        }
    }

}

internal class _OverlayChildLayoutBuilder__overlay : AbstractLayoutBuilder<OverlayChildLayoutInfo>
{
    private global::System.Func<BuildContext, OverlayChildLayoutInfo, Widget> __field_builder = default!;
    public override global::System.Func<BuildContext, OverlayChildLayoutInfo, Widget> builder { get => __field_builder; }

    internal _OverlayChildLayoutBuilder__overlay(global::System.Func<BuildContext, OverlayChildLayoutInfo, Widget> builder)
    {
        this.__field_builder = builder;
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context) => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.RenderObject>(new _RenderLayoutBuilder__overlay());
}

internal class _RenderLayoutBuilder__overlay : global::Doroti.Framework.Rendering.RenderProxyBox, _RenderTheaterMixin__overlay, RenderAbstractLayoutBuilderMixin<OverlayChildLayoutInfo, global::Doroti.Framework.Rendering.RenderBox>, global::Doroti.Framework.Rendering.IRenderLayoutCallback
{
    internal virtual OverlayChildLayoutInfo? _layoutInfo { get; set; } = default;
    internal virtual long? _callbackId { get; set; } = default;
    internal const string _speculativeLayoutErrorMessage = "This RenderObject should not be reachable in intrinsic dimension calculations.";
    public virtual global::System.Action<global::Doroti.Framework.Rendering.Constraints>? _callback { get; set; } = default;

    public virtual IEnumerable<global::Doroti.Framework.Rendering.RenderBox> _childrenInPaintOrder()
    {
        global::Doroti.Framework.Rendering.RenderBox? childLocal = ((global::Doroti.Framework.Rendering.RenderBox?)((dynamic)this).child);
        return ((childLocal is null) ? System.Linq.Enumerable.Empty<global::Doroti.Framework.Rendering.RenderBox>() : System.Linq.Enumerable.Range(0, checked((int)1L)).Select(__index => ((Func<long, global::Doroti.Framework.Rendering.RenderBox>)((i) => childLocal))(checked((long)__index))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual IEnumerable<global::Doroti.Framework.Rendering.RenderBox> _childrenInHitTestOrder() => _childrenInPaintOrder();
    public virtual _RenderTheater__overlay theater => (this.parent switch { _RenderDeferredLayoutBox__overlay parentLocal => ((_RenderDeferredLayoutBox__overlay)parentLocal).theater, _ => throw DartRuntimePrimitives.AsException(global::Doroti.Framework.Foundation.FlutterError.Create($"{this.parent} of {this} is not a _RenderDeferredLayoutBox")) });
    public override bool sizedByParent => true;
    public override void performResize() => size = ((global::Doroti.Framework.Rendering.BoxConstraints)this.constraints).biggest;
    public virtual void applyPaintTransform(global::Doroti.Framework.Rendering.RenderObject child, Matrix4 transform)
    {
        var __child = (global::Doroti.Framework.Rendering.RenderBox)(object)child;
        var childParentData = ((global::Doroti.Framework.Rendering.BoxParentData?)(object?)__child.parentData!)!;
        global::Doroti.Ui.Offset offsetLocal = ((global::Doroti.Ui.Offset)(object?)((global::Doroti.Framework.Rendering.BoxParentData)childParentData).offset);
        transform.translateByDouble(offsetLocal.dx, offsetLocal.dy, 0, 1);
    }

    public virtual OverlayChildLayoutInfo layoutInfo => DartRuntimePrimitives.ConvertValue<OverlayChildLayoutInfo>(this._layoutInfo!);
    internal virtual OverlayChildLayoutInfo _computeNewLayoutInfo()
    {
        _RenderTheater__overlay theaterLocal = this.theater;
        var parentLocal = ((_RenderDeferredLayoutBox__overlay?)(object?)this.parent!)!;
        _RenderLayoutSurrogateProxyBox__overlay layoutSurrogate = ((_RenderDeferredLayoutBox__overlay)parentLocal)._layoutSurrogate;
        DartRuntimePrimitives.Assert(() =>
            {
                for (global::Doroti.Framework.Rendering.RenderObject? node = layoutSurrogate; ((node is not null) && (!object.Equals(node, theaterLocal))); node = ((global::Doroti.Framework.Rendering.RenderObject)node).parent)
                {
                    if ((node is global::Doroti.Framework.Rendering.RenderFollowerLayer))
                    {
                        global::Doroti.Framework.Rendering.RenderFollowerLayer node__105929__as106043 = (global::Doroti.Framework.Rendering.RenderFollowerLayer)node;
                        throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("The paint transform cannot be reliably computed because of RenderFollowerLayer(s)"), ((global::Doroti.Framework.Rendering.RenderFollowerLayer)node__105929__as106043).describeForError("The RenderFollowerLayer was"), new global::Doroti.Framework.Foundation.ErrorDescription("RenderFollowerLayer establishes its paint transform only after the layout phase."), new global::Doroti.Framework.Foundation.ErrorHint("Consider replacing the corresponding CompositedTransformFollower with OverlayPortal.overlayChildLayoutBuilder if possible.") }));
                    }
                    DartRuntimePrimitives.Assert(() => (((global::Doroti.Framework.Rendering.RenderObject)node).depth > theaterLocal.depth));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        DartRuntimePrimitives.Assert(() => layoutSurrogate.hasSize);
        DartRuntimePrimitives.Assert(() => (((global::Doroti.Framework.Rendering.RenderBox?)((dynamic)layoutSurrogate).child)?.hasSize ?? true));
        DartRuntimePrimitives.Assert(() => ((((global::Doroti.Framework.Rendering.RenderBox?)((dynamic)layoutSurrogate).child) is null) || (object.Equals(((global::Doroti.Framework.Rendering.RenderBox?)((dynamic)layoutSurrogate).child)!.size, layoutSurrogate.size))));
        DartRuntimePrimitives.Assert(() => (object.Equals(this.size, theaterLocal.size)));
        DartRuntimePrimitives.Assert(() => (((global::Doroti.Framework.Rendering.RenderBox?)((dynamic)layoutSurrogate).child)?.getTransformTo(layoutSurrogate).isIdentity() ?? true));
        DartRuntimePrimitives.Assert(() => getTransformTo(theaterLocal).isIdentity());
        global::Doroti.Ui.Size overlayPortalSize = ((global::Doroti.Ui.Size)(object?)((_RenderDeferredLayoutBox__overlay)parentLocal)._layoutSurrogate.size);
        Matrix4 paintTransform = ((Matrix4)(object?)layoutSurrogate.getTransformTo(theaterLocal));
        return OverlayChildLayoutInfo.Create_((overlayPortalSize, paintTransform, this.size));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void layoutCallback()
    {
        _layoutInfo = _computeNewLayoutInfo();
        DartRuntimePrimitives.Noop();
    }

    public override void performLayout()
    {
        runLayoutCallback();
        if (this.child is global::Doroti.Framework.Rendering.RenderBox childLocal)
        {
            layoutChild(childLocal, this.constraints);
        }
        DartRuntimePrimitives.Assert(() => (this._callbackId is null));
        _callbackId ??= global::Doroti.Framework.Scheduler.SchedulerBinding.instance.scheduleFrameCallback((global::System.Action<Duration>)this._frameCallback, scheduleNewFrame: false);
    }

    public virtual double computeMinIntrinsicWidth(double height)
    {
        DartRuntimePrimitives.Assert(() => debugCannotComputeDryLayout(reason: _speculativeLayoutErrorMessage));
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double computeMaxIntrinsicWidth(double height)
    {
        DartRuntimePrimitives.Assert(() => debugCannotComputeDryLayout(reason: _speculativeLayoutErrorMessage));
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double computeMinIntrinsicHeight(double width)
    {
        DartRuntimePrimitives.Assert(() => debugCannotComputeDryLayout(reason: _speculativeLayoutErrorMessage));
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double computeMaxIntrinsicHeight(double width)
    {
        DartRuntimePrimitives.Assert(() => debugCannotComputeDryLayout(reason: _speculativeLayoutErrorMessage));
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Size computeDryLayout(global::Doroti.Framework.Rendering.BoxConstraints constraints)
    {
        DartRuntimePrimitives.Assert(() => debugCannotComputeDryLayout(reason: _speculativeLayoutErrorMessage));
        return Size.zero;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double? computeDryBaseline(global::Doroti.Framework.Rendering.BoxConstraints constraints, TextBaseline baseline)
    {
        DartRuntimePrimitives.Assert(() => debugCannotComputeDryLayout(reason: "Calculating the dry baseline would require running the layout callback " + "speculatively, which might mutate the live render object tree."));
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _frameCallback(Duration __unused0)
    {
        DartRuntimePrimitives.Assert(() => !DartRuntimePrimitives.RequireValue(this.debugDisposed));
        _callbackId = null;
        markNeedsLayout();
    }

    public override void dispose()
    {
        if (this._callbackId is long callbackId)
        {
            global::Doroti.Framework.Scheduler.SchedulerBinding.instance.cancelFrameCallbackWithId(callbackId);
        }
        base.dispose();
    }

    public virtual void setupParentData(global::Doroti.Framework.Rendering.RenderObject child)
    {
        var __child = (global::Doroti.Framework.Rendering.RenderBox)(object)child;
        if ((__child.parentData is not global::Doroti.Framework.Rendering.StackParentData))
        {
            __child.parentData = new global::Doroti.Framework.Rendering.StackParentData();
        }
    }

    public virtual double? computeDistanceToActualBaseline(TextBaseline baseline)
    {
        DartRuntimePrimitives.Assert(() => !this.debugNeedsLayout);
        global::Doroti.Framework.Rendering.BaselineOffset baselineOffset = global::Doroti.Framework.Rendering.BaselineOffset.noBaseline;
        foreach (global::Doroti.Framework.Rendering.RenderBox child in _childrenInPaintOrder())
        {
            DartRuntimePrimitives.Assert(() => !child.debugNeedsLayout);
            var childParentData = ((global::Doroti.Framework.Rendering.StackParentData?)(object?)child.parentData!)!;
            baselineOffset = baselineOffset.minOf((new global::Doroti.Framework.Rendering.BaselineOffset(child.getDistanceToActualBaseline(baseline)).op_Add(childParentData.offset.dy)));
        }
        return baselineOffset.offset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void layoutChild(global::Doroti.Framework.Rendering.RenderBox child, global::Doroti.Framework.Rendering.BoxConstraints nonPositionedChildConstraints)
    {
        var childParentData = ((global::Doroti.Framework.Rendering.StackParentData?)(object?)child.parentData!)!;
        global::Doroti.Framework.Painting.Alignment alignment = ((_RenderTheater__overlay)this.theater)._resolvedAlignment;
        if (!((global::Doroti.Framework.Rendering.StackParentData)childParentData).isPositioned)
        {
            child.layout(nonPositionedChildConstraints, parentUsesSize: true);
            childParentData.offset = Offset.zero;
        }
        else
        {
            DartRuntimePrimitives.Assert(() => (child is not _RenderDeferredLayoutBox__overlay), () => (object?)"all _RenderDeferredLayoutBoxes must be non-positioned children.");
            RenderStack.layoutPositionedChild(child, childParentData, this.size, alignment);
        }
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parentData, childParentData)));
    }

    public virtual bool hitTestChildren(global::Doroti.Framework.Rendering.BoxHitTestResult result, Offset position)
    {
        IEnumerator<global::Doroti.Framework.Rendering.RenderBox> iterator = _childrenInHitTestOrder().GetEnumerator();
        var isHit = false;
        while ((!isHit && iterator.MoveNext()))
        {
            global::Doroti.Framework.Rendering.RenderBox child = iterator.Current;
            var childParentData = ((global::Doroti.Framework.Rendering.StackParentData?)(object?)child.parentData!)!;
            var localChild = child;
            bool childHitTest(global::Doroti.Framework.Rendering.BoxHitTestResult result, Offset position)
            {
                return localChild.hitTest(result, position: position);
                throw new InvalidOperationException("Dart control flow completed without a value.");
            }
            isHit = result.addWithPaintOffset(offset: childParentData.offset, position: position, hitTest: (global::System.Func<global::Doroti.Framework.Rendering.BoxHitTestResult, Offset, bool>)childHitTest);
        }
        return isHit;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset)
    {
        foreach (global::Doroti.Framework.Rendering.RenderBox child in _childrenInPaintOrder())
        {
            var childParentData = ((global::Doroti.Framework.Rendering.StackParentData?)(object?)child.parentData!)!;
            context.paintChild(child, (childParentData.offset + offset));
        }
    }

    public virtual void _updateCallback(global::System.Action<global::Doroti.Framework.Rendering.Constraints> value)
    {
        if ((object.Equals((global::System.Action<global::Doroti.Framework.Rendering.Constraints>)value, (global::System.Action<global::Doroti.Framework.Rendering.Constraints>?)this._callback)))
        {
            return;
        }
        this._callback = (global::System.Action<global::Doroti.Framework.Rendering.Constraints>)value;
        scheduleLayoutCallback();
    }

}
