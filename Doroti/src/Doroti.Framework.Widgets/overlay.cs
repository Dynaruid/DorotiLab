// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../flutter-master/packages/flutter/lib/src/widgets/overlay.dart
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

namespace Doroti.Generated.Framework.Widgets;

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

public class OverlayEntry : global::Doroti.Generated.Framework.Foundation.Listenable
{
    public virtual global::System.Func<BuildContext, Widget> builder { get; private set; } = default!;
    internal virtual bool _opaque { get; set; } = default!;
    internal virtual bool _maintainState { get; set; } = default!;
    public virtual bool canSizeOverlay { get; private set; } = default!;
    internal virtual global::Doroti.Generated.Framework.Foundation.ValueNotifier<_OverlayEntryWidgetState__overlay?>? _overlayEntryStateNotifier { get; set; } = new global::Doroti.Generated.Framework.Foundation.ValueNotifier<_OverlayEntryWidgetState__overlay?>(((_OverlayEntryWidgetState__overlay)(object)null));
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
        OverlayState overlay__9592 = this._overlay!;
        _overlay = null;
        if (!overlay__9592.mounted)
        {
            return;
        }
        ((OverlayState)overlay__9592)._entries.Remove(this);
        if ((object.Equals(global::Doroti.Generated.Framework.Scheduler.SchedulerBinding.instance.schedulerPhase, global::Doroti.Generated.Framework.Scheduler.SchedulerPhase.persistentCallbacks)))
        {
            global::Doroti.Generated.Framework.Scheduler.SchedulerBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((duration) => {
overlay__9592._markDirty();
})), debugLabel: "OverlayEntry.markDirty");
        }
        else
        {
            overlay__9592._markDirty();
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
        DartRuntimePrimitives.Assert(() => global::Doroti.Generated.Framework.Foundation.DebugLibrary.debugMaybeDispatchDisposed(this));
        _disposedByOwner = true;
        if (!this.mounted)
        {
            this._overlayEntryStateNotifier?.dispose();
            _overlayEntryStateNotifier = null;
        }
    }

    public override string ToString() => $"{(global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}(opaque: {this.opaque}; maintainState: {this.maintainState}){(this._disposedByOwner ? "(DISPOSED)" : "")}";
}

public class _OverlayEntryWidget__overlay : StatefulWidget
{
    public virtual OverlayEntry entry { get; private set; } = default!;
    public virtual OverlayState overlayState { get; private set; } = default!;
    public virtual bool tickerEnabled { get; private set; } = default!;

    internal _OverlayEntryWidget__overlay(global::Doroti.Generated.Framework.Foundation.Key key, OverlayEntry entry, OverlayState overlayState, bool tickerEnabled = true) : base(key: key)
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
        DartLinkedList<_OverlayEntryLocation__overlay> children__13460 = _sortedTheaterSiblings ??= new DartLinkedList<_OverlayEntryLocation__overlay>();
        DartRuntimePrimitives.Assert(() => !children__13460.contains(child));
        _OverlayEntryLocation__overlay? insertPosition__13609 = (children__13460.isEmpty ? null : children__13460.last);
        while (((insertPosition__13609 is not null) && (((_OverlayEntryLocation__overlay)insertPosition__13609)._zOrderIndex > ((_OverlayEntryLocation__overlay)child)._zOrderIndex)))
        {
            insertPosition__13609 = insertPosition__13609.previous;
        }
        if ((insertPosition__13609 is null))
        {
            children__13460.addFirst(child);
        }
        else
        {
            insertPosition__13609.insertAfter(child);
        }
        DartRuntimePrimitives.Assert(() => children__13460.contains(child));
    }

    internal virtual void _remove(_OverlayEntryLocation__overlay child)
    {
        DartRuntimePrimitives.Assert(() => (this._sortedTheaterSiblings is not null));
        bool wasInCollection__14084 = (this._sortedTheaterSiblings?.remove(child) ?? false);
        DartRuntimePrimitives.Assert(() => wasInCollection__14084);
    }

    internal virtual IEnumerable<_RenderDeferredLayoutBox__overlay> _createChildIterable(bool reversed)
    {
        DartLinkedList<_OverlayEntryLocation__overlay>? children__15285 = this._sortedTheaterSiblings;
        if (((children__15285 is null) || children__15285.isEmpty))
        {
            yield break;
        }
        _OverlayEntryLocation__overlay? candidate__15415 = (reversed ? children__15285.last : children__15285.first);
        while ((candidate__15415 is not null))
        {
            _RenderDeferredLayoutBox__overlay? renderBox__15540 = ((_OverlayEntryLocation__overlay)candidate__15415)._overlayChildRenderBox;
            candidate__15415 = (reversed ? candidate__15415.previous : candidate__15415.next);
            if ((renderBox__15540 is not null))
            {
                yield return renderBox__15540;
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
            _RenderTheater__overlay newTheater__16363 = this.context.findAncestorRenderObjectOfType<_RenderTheater__overlay>()!;
            DartRuntimePrimitives.Assert(() => (!object.Equals(this._theater, newTheater__16363)));
            _theater = newTheater__16363;
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
        setState(((global::System.Action)(() => {
})));
    }

}

public class Overlay : StatefulWidget
{
    public virtual List<OverlayEntry> initialEntries { get; private set; } = default!;
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual bool alwaysSizeToContent { get; private set; } = default!;

    public Overlay(global::Doroti.Generated.Framework.Foundation.Key? key = null, List<OverlayEntry> initialEntries = default!, Clip clipBehavior = Clip.hardEdge, bool alwaysSizeToContent = false) : base(key: key)
    {
        List<OverlayEntry> __initialEntries = initialEntries ?? new List<OverlayEntry>();
        this.initialEntries = __initialEntries;
        this.clipBehavior = clipBehavior;
        this.alwaysSizeToContent = alwaysSizeToContent;
    }

    public static Widget wrap(global::Doroti.Generated.Framework.Foundation.Key? key = null, Clip clipBehavior = Clip.hardEdge, bool alwaysSizeToContent = false, Widget child = default!)
    {
        return ((Widget)(object?)new _WrappingOverlay__overlay(key: key, clipBehavior: clipBehavior, alwaysSizeToContent: alwaysSizeToContent, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static OverlayState of(BuildContext context, bool rootOverlay = false, Widget? debugRequiredFor = null)
    {
        OverlayState? result__23195 = ((OverlayState?)(object?)Overlay.maybeOf(context, rootOverlay: rootOverlay));
        DartRuntimePrimitives.Assert(() =>
            {
                if ((result__23195 is null))
                {
                    bool hiddenByBoundary__23311 = LookupBoundary.debugIsHidingAncestorStateOfType<OverlayState>(context);
                    var information__23437 = new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary($"No Overlay widget found{(hiddenByBoundary__23311 ? " within the closest LookupBoundary" : "")}."), new global::Doroti.Generated.Framework.Foundation.ErrorDescription($"{(((object?)DartRuntimePrimitives.RuntimeType(debugRequiredFor) ?? (object?)"Some"))} widgets require an Overlay widget ancestor for correct operation."), new global::Doroti.Generated.Framework.Foundation.ErrorHint("The most common way to add an Overlay to an application is to include a MaterialApp, CupertinoApp or Navigator widget in the runApp() call.") };
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(information__23437));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return result__23195!;
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
    public virtual HashSet<global::Doroti.Generated.Framework.Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

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
        var operandsInformation__26243 = new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<OverlayEntry>("The OverlayEntry was", entry, style: global::Doroti.Generated.Framework.Foundation.DiagnosticsTreeStyle.errorProperty), new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<OverlayState>("The Overlay the OverlayEntry was trying to insert to was", this, style: global::Doroti.Generated.Framework.Foundation.DiagnosticsTreeStyle.errorProperty) };
        if (!this.mounted)
        {
            throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary("Attempted to insert an OverlayEntry to an already disposed Overlay.") }));
        }
        OverlayState? currentOverlay__26863 = ((OverlayEntry)entry)._overlay;
        bool alreadyContainsEntry__26911 = this._entries.Contains(entry);
        if (alreadyContainsEntry__26911)
        {
            bool inconsistentOverlayState__27010 = !DartRuntimePrimitives.Identical(currentOverlay__26863, this);
            throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary("The specified entry is already present in the target Overlay.") }));
        }
        if ((currentOverlay__26863 is null))
        {
            return true;
        }
        throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary("The specified entry is already present in a different Overlay."), new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<OverlayState>("The OverlayEntry's current Overlay was", currentOverlay__26863, style: global::Doroti.Generated.Framework.Foundation.DiagnosticsTreeStyle.errorProperty), new global::Doroti.Generated.Framework.Foundation.ErrorHint("Consider calling remove on the OverlayEntry before inserting it to a different Overlay, " + "or switching to the OverlayPortal API to avoid manual OverlayEntry management.") }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void insert(OverlayEntry entry, OverlayEntry? below = null, OverlayEntry? above = null)
    {
        DartRuntimePrimitives.Assert(() => _debugVerifyInsertPosition(above, below));
        DartRuntimePrimitives.Assert(() => _debugCanInsertEntry(entry));
        entry._overlay = this;
        setState(((global::System.Action)(() => {
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
        foreach (var entry__29663 in entries)
        {
            DartRuntimePrimitives.Assert(() => (((OverlayEntry)entry__29663)._overlay is null));
            entry__29663._overlay = this;
        }
        setState(((global::System.Action)(() => {
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
        List<OverlayEntry> newEntriesList__31689 = ((newEntries is List<OverlayEntry>) ? newEntries : newEntries.ToList()).ToList();
        DartRuntimePrimitives.Assert(() => _debugVerifyInsertPosition(above, below, newEntries: newEntriesList__31689.Cast<OverlayEntry>()));
        DartRuntimePrimitives.Assert(() => newEntriesList__31689.All(((entry) => ((((OverlayEntry)entry)._overlay is null) || (object.Equals(((OverlayEntry)entry)._overlay, this))))), () => (object?)"One or more of the specified entries are already present in another Overlay.");
        DartRuntimePrimitives.Assert(() => newEntriesList__31689.All(((entry) => (((long)((dynamic)this._entries).IndexOf(entry)) == this._entries.LastIndexOf(entry)))), () => (object?)"One or more of the specified entries are specified multiple times.");
        if (!System.Linq.Enumerable.Any(newEntriesList__31689))
        {
            return;
        }
        if (global::Doroti.Generated.Framework.Foundation.CollectionsLibrary.listEquals(this._entries, newEntriesList__31689))
        {
            return;
        }
        var old__32464 = new HashSet<OverlayEntry>(this._entries);
        foreach (var entry__32527 in newEntriesList__31689)
        {
            entry__32527._overlay ??= this;
        }
        setState(((global::System.Action)(() => {
this._entries.Clear();
this._entries.AddRange(newEntriesList__31689.Cast<OverlayEntry>());
old__32464.ExceptWith(newEntriesList__31689);
this._entries.InsertRange(checked((int)_insertionIndex(below, above)), old__32464);
})));
    }

    internal virtual void _markDirty()
    {
        if (this.mounted)
        {
            setState(((global::System.Action)(() => {
})));
        }
    }

    public virtual bool debugIsVisible(OverlayEntry entry)
    {
        var result__33240 = false;
        DartRuntimePrimitives.Assert(() => this._entries.Contains(entry));
        DartRuntimePrimitives.Assert(() =>
            {
                for (long i__33325 = (checked((long)(this._entries.Count)) - 1L); (i__33325 > 0L); i__33325 -= 1L)
                {
                    OverlayEntry candidate__33394 = this._entries[(int)(i__33325)];
                    if ((object.Equals(candidate__33394, entry)))
                    {
                        result__33240 = true;
                        break;
                    }
                    if (((OverlayEntry)candidate__33394).opaque)
                    {
                        break;
                    }
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return result__33240;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _didChangeEntryOpacity()
    {
        setState(((global::System.Action)(() => {
})));
    }

    public override Widget build(BuildContext context)
    {
        var children__33975 = new List<_OverlayEntryWidget__overlay>();
        var onstage__34019 = true;
        var onstageCount__34043 = 0L;
        foreach (OverlayEntry entry__34089 in System.Linq.Enumerable.Reverse(this._entries))
        {
            if (onstage__34019)
            {
                onstageCount__34043 += 1L;
                children__33975.Add(new _OverlayEntryWidget__overlay(key: ((OverlayEntry)entry__34089)._key, overlayState: this, entry: entry__34089));
                if (((OverlayEntry)entry__34089).opaque)
                {
                    onstage__34019 = false;
                }
            }
            else
            {
                if (((OverlayEntry)entry__34089).maintainState)
                {
                    children__33975.Add(new _OverlayEntryWidget__overlay(key: ((OverlayEntry)entry__34089)._key, overlayState: this, entry: entry__34089, tickerEnabled: false));
                }
            }
        }
        return ((Widget)(object?)new _Theater__overlay(skipCount: (checked((long)(children__33975.Count)) - onstageCount__34043), clipBehavior: ((Overlay)this.widget).clipBehavior, alwaysSizeToContent: ((Overlay)this.widget).alwaysSizeToContent, children: System.Linq.Enumerable.Reverse(children__33975).ToList()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<HashSet<global::Doroti.Generated.Framework.Scheduler.Ticker>>("tickers", this._tickers, description: ((this._tickers is not null) ? $"tracking {checked((long)(this._tickers!.Count))} ticker{((checked((long)(this._tickers!.Count)) == 1L) ? "" : "s")}" : null), defaultValue: default));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<List<OverlayEntry>>("entries", this._entries));
    }

    public virtual global::Doroti.Generated.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        if ((this._tickerModeNotifier is null))
        {
            _updateTickerModeNotifier();
        }
        DartRuntimePrimitives.Assert(() => (this._tickerModeNotifier is not null));
        this._tickers ??= new HashSet<global::Doroti.Generated.Framework.Scheduler.Ticker>();
        TickerModeData values__17506 = this._tickerModeNotifier!.value;
        var result__17553 = ((Func<_WidgetTicker__ticker_provider>)(() =>
{            var __cascade = new _WidgetTicker__ticker_provider((global::System.Action<Duration>)onTick, this, debugLabel: (global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kDebugMode ? $"created by {(global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}" : null));
            __cascade.muted = !((TickerModeData)values__17506).enabled;
            __cascade.forceFrames = ((TickerModeData)values__17506).forceFrames;
            return __cascade;        }))();
        this._tickers!.Add(result__17553);
        return ((global::Doroti.Generated.Framework.Scheduler.Ticker)(object?)result__17553);
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
            TickerModeData values__18318 = this._tickerModeNotifier!.value;
            bool muted__18372 = !((TickerModeData)values__18318).enabled;
            foreach (global::Doroti.Generated.Framework.Scheduler.Ticker ticker__18421 in this._tickers!)
            {
                ticker__18421.muted = muted__18372;
                ticker__18421.forceFrames = ((TickerModeData)values__18318).forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData> newNotifier__18621 = ((global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData>)(object?)TickerMode.getValuesNotifier(this.context));
        if ((object.Equals(newNotifier__18621, this._tickerModeNotifier)))
        {
            return;
        }
        this._tickerModeNotifier?.removeListener(() => this._updateTickers());
        newNotifier__18621.addListener(() => this._updateTickers());
        this._tickerModeNotifier = newNotifier__18621;
    }

    public override void dispose()
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this._tickers is not null))
                {
                    foreach (global::Doroti.Generated.Framework.Scheduler.Ticker ticker__18989 in this._tickers!)
                    {
                        if (((global::Doroti.Generated.Framework.Scheduler.Ticker)ticker__18989).isActive)
                        {
                            throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Generated.Framework.Foundation.ErrorDescription($"{this.GetType()} created a Ticker via its TickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. All Tickers must " + "be disposed before calling super.dispose()."), new global::Doroti.Generated.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), ticker__18989.describeForError("The offending ticker was") }));
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

    internal _WrappingOverlay__overlay(global::Doroti.Generated.Framework.Foundation.Key? key = null, Clip clipBehavior = Clip.hardEdge, bool alwaysSizeToContent = default!, Widget child = default!) : base(key: key)
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
                __late__entry = new OverlayEntry(canSizeOverlay: true, opaque: true, builder: ((global::System.Func<BuildContext, Widget>)((context) => {
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
{            var __cascade = this._entry;
            __cascade.remove();
            __cascade.dispose();
            return __cascade;        }))());
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
    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new _RenderTheater__overlay(skipCount: this.skipCount, textDirection: Directionality.of(context), clipBehavior: this.clipBehavior, alwaysSizeToContent: this.alwaysSizeToContent));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderTheater__overlay)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<_RenderTheater__overlay>)(() =>
{            var __cascade = __renderObject;
            __cascade.skipCount = this.skipCount;
            __cascade.textDirection = Directionality.of(context);
            __cascade.clipBehavior = this.clipBehavior;
            __cascade.alwaysSizeToContent = this.alwaysSizeToContent;
            return __cascade;        }))());
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.IntProperty("skipCount", this.skipCount));
    }

}

public class _TheaterElement__overlay : MultiChildRenderObjectElement
{
    internal _TheaterElement__overlay(_Theater__overlay widget) : base(widget)
    {
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject renderObject => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Rendering.RenderObject>(((_RenderTheater__overlay?)(object?)base.renderObject)!);
    public override void insertRenderObjectChild(global::Doroti.Generated.Framework.Rendering.RenderObject child, object? slot)
    {
        var __child = (global::Doroti.Generated.Framework.Rendering.RenderBox)(object)child;
        var __slot = (IndexedSlot<Element?>)(object)slot;
        base.insertRenderObjectChild(__child, __slot);
        var parentData__37957 = ((_TheaterParentData__overlay?)(object?)__child.parentData!)!;
        parentData__37957.overlayEntry = (((_OverlayEntryWidget__overlay?)(object?)(((_Theater__overlay?)(object?)this.widget)!).children[(int)(((IndexedSlot<Element?>)__slot).index)])!).entry;
        DartRuntimePrimitives.Assert(() => (((_TheaterParentData__overlay)parentData__37957).overlayEntry is not null));
    }

    public override void moveRenderObjectChild(global::Doroti.Generated.Framework.Rendering.RenderObject child, object? oldSlot, object? newSlot)
    {
        var __child = (global::Doroti.Generated.Framework.Rendering.RenderBox)(object)child;
        var __oldSlot = (IndexedSlot<Element?>)(object)oldSlot;
        var __newSlot = (IndexedSlot<Element?>)(object)newSlot;
        base.moveRenderObjectChild(__child, __oldSlot, __newSlot);
        DartRuntimePrimitives.Assert(() =>
            {
                var parentData__38398 = ((_TheaterParentData__overlay?)(object?)__child.parentData!)!;
                OverlayEntry entryAtNewSlot__38477 = (((_OverlayEntryWidget__overlay?)(object?)(((_Theater__overlay?)(object?)this.widget)!).children[(int)(((IndexedSlot<Element?>)__newSlot).index)])!).entry;
                DartRuntimePrimitives.Assert(() => (object.Equals(((_TheaterParentData__overlay)parentData__38398).overlayEntry, entryAtNewSlot__38477)));
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
    }

    public override void debugVisitOnstageChildren(global::System.Action<Element> visitor)
    {
        var theater__38753 = ((_Theater__overlay?)(object?)this.widget)!;
        DartRuntimePrimitives.Assert(() => (this.children.Count() >= ((_Theater__overlay)theater__38753).skipCount));
        this.children.skip(((_Theater__overlay)theater__38753).skipCount).forEach((__arg0) => ((global::System.Action<Element>)visitor)(__arg0));
    }

}

internal interface _RenderTheaterMixin__overlay
{
    public _RenderTheater__overlay theater { get; }
    public IEnumerable<global::Doroti.Generated.Framework.Rendering.RenderBox> _childrenInPaintOrder();
    public IEnumerable<global::Doroti.Generated.Framework.Rendering.RenderBox> _childrenInHitTestOrder();
    public void setupParentData(global::Doroti.Generated.Framework.Rendering.RenderObject child);
    public double? computeDistanceToActualBaseline(TextBaseline baseline);
    public static double? baselineForChild(global::Doroti.Generated.Framework.Rendering.RenderBox child, Size theaterSize, global::Doroti.Generated.Framework.Rendering.BoxConstraints nonPositionedChildConstraints, global::Doroti.Generated.Framework.Painting.Alignment alignment, TextBaseline baseline)
    {
        var childParentData__40101 = ((global::Doroti.Generated.Framework.Rendering.StackParentData?)(object?)child.parentData!)!;
        global::Doroti.Generated.Framework.Rendering.BoxConstraints childConstraints__40182 = (((global::Doroti.Generated.Framework.Rendering.StackParentData)childParentData__40101).isPositioned ? childParentData__40101.positionedChildConstraints(theaterSize) : nonPositionedChildConstraints);
        double? baselineOffset__40355 = child.getDryBaseline(childConstraints__40182, baseline);
        if ((baselineOffset__40355 is null))
        {
            return null;
        }
        double y__40498 = (childParentData__40101 switch { global::Doroti.Generated.Framework.Rendering.StackParentData { top: double top__40565 } __object40535 => top__40565, global::Doroti.Generated.Framework.Rendering.StackParentData { bottom: double bottom__40615 } __object40585 => ((theaterSize.height - bottom__40615) - child.getDryLayout(childConstraints__40182).height), global::Doroti.Generated.Framework.Rendering.StackParentData __object40716 => alignment.alongOffset((theaterSize - child.getDryLayout(childConstraints__40182))).dy, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        return (DartRuntimePrimitives.RequireValue(baselineOffset__40355) + y__40498);
    }
    public void layoutChild(global::Doroti.Generated.Framework.Rendering.RenderBox child, global::Doroti.Generated.Framework.Rendering.BoxConstraints nonPositionedChildConstraints);
    public bool hitTestChildren(global::Doroti.Generated.Framework.Rendering.BoxHitTestResult result, Offset position);
    public void paint(global::Doroti.Generated.Framework.Rendering.PaintingContext context, Offset offset);
}

internal class _TheaterParentData__overlay : global::Doroti.Generated.Framework.Rendering.StackParentData
{
    public virtual OverlayEntry? overlayEntry { get; set; } = default;

    public virtual IEnumerator<_RenderDeferredLayoutBox__overlay>? paintOrderIterator => this.overlayEntry?._overlayEntryStateNotifier?.value!._paintOrderIterable.GetEnumerator();
    public virtual IEnumerator<_RenderDeferredLayoutBox__overlay>? hitTestOrderIterator => this.overlayEntry?._overlayEntryStateNotifier?.value!._hitTestOrderIterable.GetEnumerator();
    public virtual void visitOverlayPortalChildrenOnOverlayEntry(global::System.Action<global::Doroti.Generated.Framework.Rendering.RenderObject> visitor) => this.overlayEntry?._overlayEntryStateNotifier?.value!._paintOrderIterable.forEach((__arg0) => ((global::System.Action<global::Doroti.Generated.Framework.Rendering.RenderObject>)visitor)(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Rendering.RenderObject>(__arg0)));
}

public class _RenderTheater__overlay : global::Doroti.Generated.Framework.Rendering.RenderBox, global::Doroti.Generated.Framework.Rendering.ContainerRenderObjectMixin<global::Doroti.Generated.Framework.Rendering.RenderBox, global::Doroti.Generated.Framework.Rendering.StackParentData>, _RenderTheaterMixin__overlay
{
    internal virtual global::Doroti.Generated.Framework.Painting.Alignment? _alignmentCache { get; set; } = default;
    internal virtual TextDirection _textDirection { get; set; } = default!;
    internal virtual long _skipCount { get; set; } = default!;
    internal virtual Clip _clipBehavior { get; set; } = Clip.hardEdge;
    internal virtual bool _alwaysSizeToContent { get; set; } = default!;
    internal virtual long _outstandingDeferredChildUpdateCalls { get; set; } = 0L;
    internal virtual bool _layingOutSizeDeterminingChild { get; set; } = false;
    internal virtual global::Doroti.Generated.Framework.Rendering.LayerHandle<global::Doroti.Generated.Framework.Rendering.ClipRectLayer> _clipRectLayer { get; private set; } = new global::Doroti.Generated.Framework.Rendering.LayerHandle<global::Doroti.Generated.Framework.Rendering.ClipRectLayer>();
    public virtual long _childCount { get; set; } = 0L;
    public virtual RenderBox? _firstChild { get; set; } = default;
    public virtual RenderBox? _lastChild { get; set; } = default;

    internal _RenderTheater__overlay(List<global::Doroti.Generated.Framework.Rendering.RenderBox>? children = null, TextDirection textDirection = default!, long skipCount = 0, Clip clipBehavior = Clip.hardEdge, bool alwaysSizeToContent = default!)
    {
        this._textDirection = textDirection;
        this._skipCount = skipCount;
        this._clipBehavior = clipBehavior;
        this._alwaysSizeToContent = alwaysSizeToContent;
        System.Diagnostics.Debug.Assert((skipCount >= 0L));
    }

    public virtual _RenderTheater__overlay theater => this;
    public override void setupParentData(global::Doroti.Generated.Framework.Rendering.RenderObject child)
    {
        var __child = (global::Doroti.Generated.Framework.Rendering.RenderBox)(object)child;
        if ((__child.parentData is not _TheaterParentData__overlay))
        {
            __child.parentData = new _TheaterParentData__overlay();
        }
    }

    public override void attach(global::Doroti.Generated.Framework.Rendering.PipelineOwner owner)
    {
        base.attach(owner);
        global::Doroti.Generated.Framework.Rendering.RenderBox? child__181803 = this._firstChild;
        while ((child__181803 is not null))
        {
            child__181803.attach(owner);
            var childParentData__181891 = ((global::Doroti.Generated.Framework.Rendering.StackParentData?)(object?)child__181803.parentData!)!;
            child__181803 = childParentData__181891.nextSibling;
        }
        global::Doroti.Generated.Framework.Rendering.RenderBox? child__44958 = this.firstChild;
        while ((child__44958 is not null))
        {
            var childParentData__45018 = ((_TheaterParentData__overlay?)(object?)child__44958.parentData!)!;
            IEnumerator<global::Doroti.Generated.Framework.Rendering.RenderBox>? iterator__45110 = ((IEnumerator<global::Doroti.Generated.Framework.Rendering.RenderBox>?)(object?)((_TheaterParentData__overlay)childParentData__45018).paintOrderIterator);
            if ((iterator__45110 is not null))
            {
                while (iterator__45110.MoveNext())
                {
                    iterator__45110.Current.attach(owner);
                }
            }
            child__44958 = childParentData__45018.nextSibling;
        }
    }

    internal static void _detachChild(global::Doroti.Generated.Framework.Rendering.RenderObject child) => ((dynamic)child).detach();
    public override void detach()
    {
        base.detach();
        global::Doroti.Generated.Framework.Rendering.RenderBox? child__182065 = this._firstChild;
        while ((child__182065 is not null))
        {
            child__182065.detach();
            var childParentData__182148 = ((global::Doroti.Generated.Framework.Rendering.StackParentData?)(object?)child__182065.parentData!)!;
            child__182065 = childParentData__182148.nextSibling;
        }
        global::Doroti.Generated.Framework.Rendering.RenderBox? child__45471 = this.firstChild;
        while ((child__45471 is not null))
        {
            var childParentData__45531 = ((_TheaterParentData__overlay?)(object?)child__45471.parentData!)!;
            childParentData__45531.visitOverlayPortalChildrenOnOverlayEntry((global::System.Action<global::Doroti.Generated.Framework.Rendering.RenderObject>)_detachChild);
            child__45471 = childParentData__45531.nextSibling;
        }
    }

    public override void redepthChildren() => visitChildren((global::System.Action<global::Doroti.Generated.Framework.Rendering.RenderObject>)this.redepthChild);
    internal virtual global::Doroti.Generated.Framework.Painting.Alignment _resolvedAlignment => _alignmentCache ??= global::Doroti.Generated.Framework.Painting.AlignmentDirectional.topStart.resolve(this.textDirection);
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

    internal virtual global::Doroti.Generated.Framework.Rendering.RenderBox? _firstOnstageChild
    {
        get
        {
            if ((this.skipCount == this.childCount))
            {
                return ((global::Doroti.Generated.Framework.Rendering.RenderBox)(object)null);
            }
            global::Doroti.Generated.Framework.Rendering.RenderBox? child__48824 = this.firstChild;
            for (long toSkip__48863 = this.skipCount; (toSkip__48863 > 0L); toSkip__48863--)
            {
                var childParentData__48919 = ((global::Doroti.Generated.Framework.Rendering.StackParentData?)(object?)child__48824!.parentData!)!;
                child__48824 = childParentData__48919.nextSibling;
                DartRuntimePrimitives.Assert(() => (child__48824 is not null));
            }
            return child__48824;
            return default!;
        }
    }
    internal virtual global::Doroti.Generated.Framework.Rendering.RenderBox? _lastOnstageChild => ((this.skipCount == this.childCount) ? null : this.lastChild);
    public override double computeMinIntrinsicWidth(double height)
    {
        return RenderStack.getIntrinsicDimension(this._firstOnstageChild, ((global::System.Func<global::Doroti.Generated.Framework.Rendering.RenderBox, double>)((child) => child.getMinIntrinsicWidth(height))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        return RenderStack.getIntrinsicDimension(this._firstOnstageChild, ((global::System.Func<global::Doroti.Generated.Framework.Rendering.RenderBox, double>)((child) => child.getMaxIntrinsicWidth(height))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        return RenderStack.getIntrinsicDimension(this._firstOnstageChild, ((global::System.Func<global::Doroti.Generated.Framework.Rendering.RenderBox, double>)((child) => child.getMinIntrinsicHeight(width))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        return RenderStack.getIntrinsicDimension(this._firstOnstageChild, ((global::System.Func<global::Doroti.Generated.Framework.Rendering.RenderBox, double>)((child) => child.getMaxIntrinsicHeight(width))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints, TextBaseline baseline)
    {
        global::Doroti.Ui.Size size__50115 = ((global::Doroti.Ui.Size)(object?)((!this.alwaysSizeToContent && ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints).biggest.isFinite) ? ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints).biggest : _findSizeDeterminingChild().getDryLayout(constraints)));
        var nonPositionedChildConstraints__50280 = global::Doroti.Generated.Framework.Rendering.BoxConstraints.CreateTight(this.size);
        global::Doroti.Generated.Framework.Painting.Alignment alignment__50360 = ((_RenderTheater__overlay)this.theater)._resolvedAlignment;
        global::Doroti.Generated.Framework.Rendering.BaselineOffset baselineOffset__50420 = global::Doroti.Generated.Framework.Rendering.BaselineOffset.noBaseline;
        foreach (global::Doroti.Generated.Framework.Rendering.RenderBox child__50489 in _childrenInPaintOrder())
        {
            baselineOffset__50420 = baselineOffset__50420.minOf(new global::Doroti.Generated.Framework.Rendering.BaselineOffset(_RenderTheaterMixin__overlay.baselineForChild(child__50489, this.size, nonPositionedChildConstraints__50280, alignment__50360, baseline)));
        }
        return baselineOffset__50420.offset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints)
    {
        if ((!this.alwaysSizeToContent && ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints).biggest.isFinite))
        {
            return ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints).biggest;
        }
        return _findSizeDeterminingChild().getDryLayout(constraints);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual IEnumerable<global::Doroti.Generated.Framework.Rendering.RenderBox> _childrenInPaintOrder()
    {
        global::Doroti.Generated.Framework.Rendering.RenderBox? child__51268 = this._firstOnstageChild;
        while ((child__51268 is not null))
        {
            yield return child__51268;
            var childParentData__51355 = ((_TheaterParentData__overlay?)(object?)child__51268.parentData!)!;
            IEnumerator<global::Doroti.Generated.Framework.Rendering.RenderBox>? innerIterator__51447 = ((IEnumerator<global::Doroti.Generated.Framework.Rendering.RenderBox>?)(object?)((_TheaterParentData__overlay)childParentData__51355).paintOrderIterator);
            if ((innerIterator__51447 is not null))
            {
                while (innerIterator__51447.MoveNext())
                {
                    yield return innerIterator__51447.Current;
                }
            }
            child__51268 = childParentData__51355.nextSibling;
        }
    }

    public virtual IEnumerable<global::Doroti.Generated.Framework.Rendering.RenderBox> _childrenInHitTestOrder()
    {
        global::Doroti.Generated.Framework.Rendering.RenderBox? child__51837 = this._lastOnstageChild;
        long childLeft__51872 = (this.childCount - this.skipCount);
        while ((child__51837 is not null))
        {
            var childParentData__51948 = ((_TheaterParentData__overlay?)(object?)child__51837.parentData!)!;
            IEnumerator<global::Doroti.Generated.Framework.Rendering.RenderBox>? innerIterator__52040 = ((IEnumerator<global::Doroti.Generated.Framework.Rendering.RenderBox>?)(object?)((_TheaterParentData__overlay)childParentData__51948).hitTestOrderIterator);
            if ((innerIterator__52040 is not null))
            {
                while (innerIterator__52040.MoveNext())
                {
                    yield return innerIterator__52040.Current;
                }
            }
            yield return child__51837;
            childLeft__51872 -= 1L;
            child__51837 = ((childLeft__51872 <= 0L) ? null : childParentData__51948.previousSibling);
        }
    }

    public override bool sizedByParent => false;
    public override void performLayout()
    {
        global::Doroti.Generated.Framework.Rendering.RenderBox? sizeDeterminingChild__52499 = default!;
        if ((!this.alwaysSizeToContent && ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)this.constraints).biggest.isFinite))
        {
            this.size = ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)this.constraints).biggest;
        }
        else
        {
            sizeDeterminingChild__52499 = _findSizeDeterminingChild();
            _layingOutSizeDeterminingChild = true;
            layoutChild(sizeDeterminingChild__52499, this.constraints);
            _layingOutSizeDeterminingChild = false;
            this.size = ((global::Doroti.Generated.Framework.Rendering.RenderBox)sizeDeterminingChild__52499).size;
        }
        var nonPositionedChildConstraints__52969 = global::Doroti.Generated.Framework.Rendering.BoxConstraints.CreateTight(this.size);
        foreach (global::Doroti.Generated.Framework.Rendering.RenderBox child__53054 in _childrenInPaintOrder())
        {
            if ((!object.Equals(child__53054, sizeDeterminingChild__52499)))
            {
                layoutChild(child__53054, nonPositionedChildConstraints__52969);
            }
        }
    }

    internal virtual global::Doroti.Generated.Framework.Rendering.RenderBox _findSizeDeterminingChild()
    {
        global::Doroti.Generated.Framework.Rendering.RenderBox? child__53268 = this._lastOnstageChild;
        while ((child__53268 is not null))
        {
            var childParentData__53335 = ((_TheaterParentData__overlay?)(object?)child__53268.parentData!)!;
            if ((((((_TheaterParentData__overlay)childParentData__53335).overlayEntry?.canSizeOverlay ?? false)) && !childParentData__53335.isPositioned))
            {
                return child__53268;
            }
            child__53268 = childParentData__53335.previousSibling;
        }
        if (this.alwaysSizeToContent)
        {
            throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary("Overlay was asked to size itself to content but does not have a suitable child."), new global::Doroti.Generated.Framework.Foundation.ErrorDescription("When `alwaysSizeToContent` is true, the Overlay requires at least one " + "non-positioned `OverlayEntry` with `canSizeOverlay` set to true to determine its size."), new global::Doroti.Generated.Framework.Foundation.ErrorHint("Try removing alwaysSizeToContent=true or provide a suitable child that can size the Overlay") }));
        }
        throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary("Overlay was given infinite constraints and cannot be sized by a suitable child."), new global::Doroti.Generated.Framework.Foundation.ErrorDescription($"The constraints given to the overlay ({this.constraints}) would result in an illegal " + $"infinite size ({(((global::Doroti.Generated.Framework.Rendering.BoxConstraints)this.constraints).biggest)}). To avoid that, the Overlay tried to size " + "itself to one of its children, but no suitable non-positioned child that belongs to an " + "OverlayEntry with canSizeOverlay set to true could be found."), new global::Doroti.Generated.Framework.Foundation.ErrorHint("Try wrapping the Overlay in a SizedBox to give it a finite size or " + "use an OverlayEntry with canSizeOverlay set to true.") }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(global::Doroti.Generated.Framework.Rendering.PaintingContext context, Offset offset)
    {
        if ((!object.Equals(this.clipBehavior, Clip.none)))
        {
            this._clipRectLayer.layer = context.pushClipRect(this.needsCompositing, offset, (Offset.zero & this.size), (paintContext, paintOffset) =>
            {
                foreach (global::Doroti.Generated.Framework.Rendering.RenderBox child in _childrenInPaintOrder())
                {
                    var childParentData = ((global::Doroti.Generated.Framework.Rendering.StackParentData?)(object?)child.parentData!)!;
                    paintContext.paintChild(child, childParentData.offset + paintOffset);
                }
            }, clipBehavior: this.clipBehavior, oldLayer: ((global::Doroti.Generated.Framework.Rendering.LayerHandle<global::Doroti.Generated.Framework.Rendering.ClipRectLayer>)this._clipRectLayer).layer);
        }
        else
        {
            this._clipRectLayer.layer = null;
            foreach (global::Doroti.Generated.Framework.Rendering.RenderBox child__42320 in _childrenInPaintOrder())
            {
                var childParentData__42368 = ((global::Doroti.Generated.Framework.Rendering.StackParentData?)(object?)child__42320.parentData!)!;
                context.paintChild(child__42320, (childParentData__42368.offset + offset));
            }
        }
    }

    public override void dispose()
    {
        this._clipRectLayer.layer = null;
        base.dispose();
    }

    public override void visitChildren(global::System.Action<global::Doroti.Generated.Framework.Rendering.RenderObject> visitor)
    {
        global::Doroti.Generated.Framework.Rendering.RenderBox? child__55587 = this.firstChild;
        while ((child__55587 is not null))
        {
            visitor(child__55587);
            var childParentData__55669 = ((_TheaterParentData__overlay?)(object?)child__55587.parentData!)!;
            childParentData__55669.visitOverlayPortalChildrenOnOverlayEntry((global::System.Action<global::Doroti.Generated.Framework.Rendering.RenderObject>)visitor);
            child__55587 = childParentData__55669.nextSibling;
        }
    }

    public override void visitChildrenForSemantics(global::System.Action<global::Doroti.Generated.Framework.Rendering.RenderObject> visitor)
    {
        global::Doroti.Generated.Framework.Rendering.RenderBox? child__55946 = this._firstOnstageChild;
        while ((child__55946 is not null))
        {
            visitor(child__55946);
            var childParentData__56036 = ((_TheaterParentData__overlay?)(object?)child__55946.parentData!)!;
            childParentData__56036.visitOverlayPortalChildrenOnOverlayEntry((global::System.Action<global::Doroti.Generated.Framework.Rendering.RenderObject>)visitor);
            child__55946 = childParentData__56036.nextSibling;
        }
    }

    public override Rect? describeApproximatePaintClip(global::Doroti.Generated.Framework.Rendering.RenderObject child)
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

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.IntProperty("skipCount", this.skipCount));
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<global::Doroti.Ui.TextDirection>("textDirection", this.textDirection));
    }

    public override List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> debugDescribeChildren()
    {
        var offstageChildren__56843 = new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode>();
        var onstageChildren__56893 = new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode>();
        var count__56941 = 1L;
        var onstage__56960 = false;
        global::Doroti.Generated.Framework.Rendering.RenderBox? child__56992 = this.firstChild;
        global::Doroti.Generated.Framework.Rendering.RenderBox? firstOnstageChild__57033 = this._firstOnstageChild;
        while ((child__56992 is not null))
        {
            var childParentData__57113 = ((_TheaterParentData__overlay?)(object?)child__56992.parentData!)!;
            if ((object.Equals(child__56992, firstOnstageChild__57033)))
            {
                onstage__56960 = true;
                count__56941 = 1L;
            }
            if (onstage__56960)
            {
                onstageChildren__56893.Add(((Diagnosticable)child__56992).toDiagnosticsNode(name: $"onstage {count__56941}"));
            }
            else
            {
                offstageChildren__56843.Add(((Diagnosticable)child__56992).toDiagnosticsNode(name: $"offstage {count__56941}", style: global::Doroti.Generated.Framework.Foundation.DiagnosticsTreeStyle.offstage));
            }
            var subcount__57536 = 1L;
            childParentData__57113.visitOverlayPortalChildrenOnOverlayEntry(((global::System.Action<global::Doroti.Generated.Framework.Rendering.RenderObject>)((renderObject) => {
var child__57657 = ((global::Doroti.Generated.Framework.Rendering.RenderBox?)(object?)renderObject)!;
if (onstage__56960)
{
    onstageChildren__56893.Add(((Diagnosticable)child__57657).toDiagnosticsNode(name: $"onstage {count__56941} - {subcount__57536}"));
}
else
{
    offstageChildren__56843.Add(((Diagnosticable)child__57657).toDiagnosticsNode(name: $"offstage {count__56941} - {subcount__57536}", style: global::Doroti.Generated.Framework.Foundation.DiagnosticsTreeStyle.offstage));
}
subcount__57536 += 1L;
})));
            child__56992 = childParentData__57113.nextSibling;
            count__56941 += 1L;
        }
        return new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode>();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _debugUltimatePreviousSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData__173585 = ((StackParentData?)(object?)child.parentData!)!;
        while ((childParentData__173585.previousSibling is not null))
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(childParentData__173585.previousSibling, child)));
            child = childParentData__173585.previousSibling!;
            childParentData__173585 = ((StackParentData?)(object?)child.parentData!)!;
        }
        return (object.Equals(child, equals));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _debugUltimateNextSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData__173981 = ((StackParentData?)(object?)child.parentData!)!;
        while ((childParentData__173981.nextSibling is not null))
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(childParentData__173981.nextSibling, child)));
            child = childParentData__173981.nextSibling!;
            childParentData__173981 = ((StackParentData?)(object?)child.parentData!)!;
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
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary($"A {this.GetType()} expected a child of type {typeof(RenderBox)} but received a " + $"child of type {DartRuntimePrimitives.RuntimeType(child)}."), new global::Doroti.Generated.Framework.Foundation.ErrorDescription("RenderObjects expect specific types of children because they " + "coordinate with their children during layout and paint. For " + "example, a RenderSliver cannot be the child of a RenderBox because " + "a RenderSliver does not understand the RenderBox layout protocol."), new global::Doroti.Generated.Framework.Foundation.ErrorSpacer(), new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<object?>($"The {this.GetType()} that expected a {typeof(RenderBox)} child was created by", this.debugCreator, style: global::Doroti.Generated.Framework.Foundation.DiagnosticsTreeStyle.errorProperty), new global::Doroti.Generated.Framework.Foundation.ErrorSpacer(), new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<object?>($"The {DartRuntimePrimitives.RuntimeType(child)} that did not match the expected child type " + "was created by", ((RenderObject)child).debugCreator, style: global::Doroti.Generated.Framework.Foundation.DiagnosticsTreeStyle.errorProperty) }));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _insertIntoChildList(RenderBox child, RenderBox? after = null)
    {
        var childParentData__175971 = ((StackParentData?)(object?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => (childParentData__175971.nextSibling is null));
        DartRuntimePrimitives.Assert(() => (childParentData__175971.previousSibling is null));
        this._childCount += 1L;
        DartRuntimePrimitives.Assert(() => (this._childCount > 0L));
        if ((after is null))
        {
            childParentData__175971.nextSibling = this._firstChild;
            if ((this._firstChild is not null))
            {
                var firstChildParentData__176343 = ((StackParentData?)(object?)this._firstChild!.parentData!)!;
                firstChildParentData__176343.previousSibling = child;
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
            var afterParentData__176766 = ((StackParentData?)(object?)after.parentData!)!;
            if ((afterParentData__176766.nextSibling is null))
            {
                DartRuntimePrimitives.Assert(() => (object.Equals(after, this._lastChild)));
                childParentData__175971.previousSibling = after;
                afterParentData__176766.nextSibling = child;
                this._lastChild = child;
            }
            else
            {
                childParentData__175971.nextSibling = afterParentData__176766.nextSibling;
                childParentData__175971.previousSibling = after;
                var childPreviousSiblingParentData__177424 = ((StackParentData?)(object?)childParentData__175971.previousSibling!.parentData!)!;
                var childNextSiblingParentData__177547 = ((StackParentData?)(object?)childParentData__175971.nextSibling!.parentData!)!;
                childPreviousSiblingParentData__177424.nextSibling = child;
                childNextSiblingParentData__177547.previousSibling = child;
                DartRuntimePrimitives.Assert(() => (object.Equals(afterParentData__176766.nextSibling, child)));
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
        var childParentData__179226 = ((StackParentData?)(object?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => _debugUltimatePreviousSiblingOf(child, equals: this._firstChild));
        DartRuntimePrimitives.Assert(() => _debugUltimateNextSiblingOf(child, equals: this._lastChild));
        DartRuntimePrimitives.Assert(() => (this._childCount >= 0L));
        if ((childParentData__179226.previousSibling is null))
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(this._firstChild, child)));
            this._firstChild = childParentData__179226.nextSibling;
        }
        else
        {
            var childPreviousSiblingParentData__179613 = ((StackParentData?)(object?)childParentData__179226.previousSibling!.parentData!)!;
            childPreviousSiblingParentData__179613.nextSibling = childParentData__179226.nextSibling;
        }
        if ((childParentData__179226.nextSibling is null))
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(this._lastChild, child)));
            this._lastChild = childParentData__179226.previousSibling;
        }
        else
        {
            var childNextSiblingParentData__179965 = ((StackParentData?)(object?)childParentData__179226.nextSibling!.parentData!)!;
            childNextSiblingParentData__179965.previousSibling = childParentData__179226.previousSibling;
        }
        childParentData__179226.previousSibling = null;
        childParentData__179226.nextSibling = null;
        this._childCount -= 1L;
    }

    public virtual void remove(RenderBox child)
    {
        _removeFromChildList(child);
        dropChild(child);
    }

    public virtual void removeAll()
    {
        RenderBox? child__180623 = this._firstChild;
        while ((child__180623 is not null))
        {
            var childParentData__180684 = ((StackParentData?)(object?)child__180623.parentData!)!;
            RenderBox? next__180762 = childParentData__180684.nextSibling;
            childParentData__180684.previousSibling = null;
            childParentData__180684.nextSibling = null;
            dropChild(child__180623);
            child__180623 = next__180762;
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
        var childParentData__181479 = ((StackParentData?)(object?)child.parentData!)!;
        if ((object.Equals(childParentData__181479.previousSibling, after)))
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
        var childParentData__183103 = ((StackParentData?)(object?)child.parentData!)!;
        return childParentData__183103.previousSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RenderBox? childAfter(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        var childParentData__183356 = ((StackParentData?)(object?)child.parentData!)!;
        return childParentData__183356.nextSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDistanceToActualBaseline(TextBaseline baseline)
    {
        DartRuntimePrimitives.Assert(() => !this.debugNeedsLayout);
        global::Doroti.Generated.Framework.Rendering.BaselineOffset baselineOffset__39497 = global::Doroti.Generated.Framework.Rendering.BaselineOffset.noBaseline;
        foreach (global::Doroti.Generated.Framework.Rendering.RenderBox child__39566 in _childrenInPaintOrder())
        {
            DartRuntimePrimitives.Assert(() => !child__39566.debugNeedsLayout);
            var childParentData__39653 = ((global::Doroti.Generated.Framework.Rendering.StackParentData?)(object?)child__39566.parentData!)!;
            baselineOffset__39497 = baselineOffset__39497.minOf((new global::Doroti.Generated.Framework.Rendering.BaselineOffset(child__39566.getDistanceToActualBaseline(baseline)).op_Add(childParentData__39653.offset.dy)));
        }
        return baselineOffset__39497.offset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void layoutChild(global::Doroti.Generated.Framework.Rendering.RenderBox child, global::Doroti.Generated.Framework.Rendering.BoxConstraints nonPositionedChildConstraints)
    {
        var childParentData__40970 = ((global::Doroti.Generated.Framework.Rendering.StackParentData?)(object?)child.parentData!)!;
        global::Doroti.Generated.Framework.Painting.Alignment alignment__41046 = ((_RenderTheater__overlay)this.theater)._resolvedAlignment;
        if (!((global::Doroti.Generated.Framework.Rendering.StackParentData)childParentData__40970).isPositioned)
        {
            child.layout(nonPositionedChildConstraints, parentUsesSize: true);
            childParentData__40970.offset = Offset.zero;
        }
        else
        {
            DartRuntimePrimitives.Assert(() => (child is not _RenderDeferredLayoutBox__overlay), () => (object?)"all _RenderDeferredLayoutBoxes must be non-positioned children.");
            RenderStack.layoutPositionedChild(child, childParentData__40970, this.size, alignment__41046);
        }
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parentData, childParentData__40970)));
    }

    public override bool hitTestChildren(global::Doroti.Generated.Framework.Rendering.BoxHitTestResult result, Offset position)
    {
        IEnumerator<global::Doroti.Generated.Framework.Rendering.RenderBox> iterator__41661 = _childrenInHitTestOrder().GetEnumerator();
        var isHit__41716 = false;
        while ((!isHit__41716 && iterator__41661.MoveNext()))
        {
            global::Doroti.Generated.Framework.Rendering.RenderBox child__41797 = iterator__41661.Current;
            var childParentData__41835 = ((global::Doroti.Generated.Framework.Rendering.StackParentData?)(object?)child__41797.parentData!)!;
            var localChild__41903 = child__41797;
            bool childHitTest(global::Doroti.Generated.Framework.Rendering.BoxHitTestResult result, Offset position)
            {
                return localChild__41903.hitTest(result, position: position);
                throw new InvalidOperationException("Dart control flow completed without a value.");
            }
            isHit__41716 = result.addWithPaintOffset(offset: childParentData__41835.offset, position: position, hitTest: (global::System.Func<global::Doroti.Generated.Framework.Rendering.BoxHitTestResult, Offset, bool>)childHitTest);
        }
        return isHit__41716;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class OverlayPortalController
{
    internal virtual _OverlayPortalState__overlay? _attachTarget { get; set; } = default;
    internal virtual long? _zOrderIndex { get; set; } = default;
    internal virtual string? _debugLabel { get; private set; }
    internal static long _wallTime = (global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kIsWeb ? -9007199254740992L : (-1L << (int)(63L)));

    public OverlayPortalController(string? debugLabel = null)
    {
        this._debugLabel = debugLabel;
    }

    internal virtual long _now()
    {
        long now__61102 = _wallTime += 1L;
        DartRuntimePrimitives.Assert(() => ((this._zOrderIndex is null) || (DartRuntimePrimitives.RequireValue(this._zOrderIndex) < now__61102)));
        DartRuntimePrimitives.Assert(() => ((this._attachTarget?._zOrderIndex is null) || (DartRuntimePrimitives.RequireValue(this._attachTarget!._zOrderIndex) < now__61102)));
        return now__61102;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void show()
    {
        _OverlayPortalState__overlay? state__61892 = this._attachTarget;
        if ((state__61892 is not null))
        {
            state__61892.show(_now());
        }
        else
        {
            _zOrderIndex = _now();
        }
    }

    public virtual void hide()
    {
        _OverlayPortalState__overlay? state__62418 = this._attachTarget;
        if ((state__62418 is not null))
        {
            state__62418.hide();
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
            _OverlayPortalState__overlay? state__62752 = this._attachTarget;
            return ((state__62752 is not null) ? (((_OverlayPortalState__overlay)state__62752)._zOrderIndex is not null) : (this._zOrderIndex is not null));
            return default!;
        }
    }
    public virtual void toggle() => ((Action)(() => { if (this.isShowing) { hide(); } else { show(); } }))();
    public override string ToString()
    {
        string? debugLabel__63130 = this._debugLabel;
        var label__63166 = ((debugLabel__63130 is null) ? "" : $"({debugLabel__63130})");
        var isDetached__63227 = ((this._attachTarget is not null) ? "" : " DETACHED");
        return $"{(global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "OverlayPortalController"))}{label__63166}{isDetached__63227}";
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

    public OverlayPortal(global::Doroti.Generated.Framework.Foundation.Key? key = null, OverlayPortalController controller = default!, global::System.Func<BuildContext, Widget> overlayChildBuilder = default!, OverlayChildLocation overlayLocation = OverlayChildLocation.nearestOverlay, Widget? child = null) : base(key: key)
    {
        this.controller = controller;
        this.overlayChildBuilder = overlayChildBuilder;
        this.overlayLocation = overlayLocation;
        this.child = child;
    }

    public static OverlayPortal CreateTargetsRootOverlay(global::Doroti.Generated.Framework.Foundation.Key? key = null, OverlayPortalController controller = default!, global::System.Func<BuildContext, Widget> overlayChildBuilder = default!, Widget? child = null)
    {
        var __instance = new OverlayPortal(default!, default!, default!, default!, default!);
        __instance.controller = controller;
        __instance.overlayChildBuilder = overlayChildBuilder;
        __instance.child = child;
        __instance.overlayLocation = OverlayChildLocation.rootOverlay;
        return __instance;
    }

    public static OverlayPortal CreateOverlayChildLayoutBuilder(global::Doroti.Generated.Framework.Foundation.Key? key = null, OverlayPortalController controller = default!, global::System.Func<BuildContext, OverlayChildLayoutInfo, Widget> overlayChildBuilder = default!, OverlayChildLocation overlayLocation = OverlayChildLocation.nearestOverlay, Widget? child = default!)
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
        _OverlayEntryLocation__overlay? cachedLocation__73881 = this._locationCache;
        _RenderTheaterMarker__overlay marker__73950 = ((_RenderTheaterMarker__overlay)(object?)_RenderTheaterMarker__overlay.of(this.context, targetRootOverlay: (object.Equals(overlayLocation, OverlayChildLocation.rootOverlay))));
        bool isCacheValid__74099 = ((cachedLocation__73881 is not null) && ((!this._childModelMayHaveChanged || _OverlayPortalState__overlay._isTheSameLocation(cachedLocation__73881, marker__73950))));
        _childModelMayHaveChanged = false;
        if (isCacheValid__74099)
        {
            DartRuntimePrimitives.Assert(() => (((_OverlayEntryLocation__overlay)cachedLocation__73881)._zOrderIndex == zOrderIndex));
            DartRuntimePrimitives.Assert(() => cachedLocation__73881._debugIsLocationValid());
            return cachedLocation__73881;
        }
        cachedLocation__73881?._debugMarkLocationInvalid();
        var newLocation__74566 = new _OverlayEntryLocation__overlay(zOrderIndex, ((_RenderTheaterMarker__overlay)marker__73950).overlayEntryWidgetState, ((_RenderTheaterMarker__overlay)marker__73950).theater);
        DartRuntimePrimitives.Assert(() => (((_OverlayEntryLocation__overlay)newLocation__74566)._zOrderIndex == zOrderIndex));
        return _locationCache = newLocation__74566;
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
        long? controllerZOrderIndex__75225 = ((OverlayPortalController)controller)._zOrderIndex;
        long? zOrderIndex__75289 = this._zOrderIndex;
        if (((zOrderIndex__75289 is null) || (((controllerZOrderIndex__75225 is not null) && (DartRuntimePrimitives.RequireValue(controllerZOrderIndex__75225) > DartRuntimePrimitives.RequireValue(zOrderIndex__75289))))))
        {
            _zOrderIndex = controllerZOrderIndex__75225;
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
        DartRuntimePrimitives.Assert(() => (!object.Equals(global::Doroti.Generated.Framework.Scheduler.SchedulerBinding.instance.schedulerPhase, global::Doroti.Generated.Framework.Scheduler.SchedulerPhase.persistentCallbacks)), () => (object?)$"{DartRuntimePrimitives.RuntimeType(((OverlayPortal)this.widget).controller)}.show() should not be called during build.");
        setState(((global::System.Action)(() => {
_zOrderIndex = zOrderIndex;
})));
        this._locationCache?._debugMarkLocationInvalid();
        _locationCache = null;
    }

    public virtual void hide()
    {
        DartRuntimePrimitives.Assert(() => (!object.Equals(global::Doroti.Generated.Framework.Scheduler.SchedulerBinding.instance.schedulerPhase, global::Doroti.Generated.Framework.Scheduler.SchedulerPhase.persistentCallbacks)));
        setState(((global::System.Action)(() => {
_zOrderIndex = null;
})));
        this._locationCache?._debugMarkLocationInvalid();
        _locationCache = null;
    }

    public override Widget build(BuildContext context)
    {
        long? zOrderIndex__77017 = this._zOrderIndex;
        if ((zOrderIndex__77017 is null))
        {
            return ((Widget)(object?)new _OverlayPortal__overlay(overlayLocation: ((_OverlayEntryLocation__overlay)(object)null), overlayChild: ((Widget)(object)null), child: new Semantics(traversalParentIdentifier: this, child: ((OverlayPortal)this.widget).child)));
        }
        _OverlayEntryLocation__overlay overlayLocation__77292 = ((_OverlayEntryLocation__overlay)(object?)_getLocation(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(zOrderIndex__77017)), ((OverlayPortal)this.widget).overlayLocation));
        MediaQueryData overlayData__77386 = ((MediaQueryData)(object?)MediaQuery.of(((_OverlayEntryLocation__overlay)overlayLocation__77292)._childModel.context));
        MediaQueryData data__77477 = ((MediaQueryData)(object?)MediaQuery.of(context).copyWith(padding: ((MediaQueryData)overlayData__77386).padding, viewInsets: ((MediaQueryData)overlayData__77386).viewInsets, viewPadding: ((MediaQueryData)overlayData__77386).viewPadding));
        return ((Widget)(object?)new _OverlayPortal__overlay(overlayLocation: overlayLocation__77292, overlayChild: new _DeferredLayout__overlay(childIdentifier: this, child: new MediaQuery(data: data__77477, child: new Builder(builder: (global::System.Func<BuildContext, Widget>)((OverlayPortal)this.widget).overlayChildBuilder))), child: new Semantics(traversalParentIdentifier: this, child: ((OverlayPortal)this.widget).child)));
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
        _RenderTheater__overlay fromTheater__81283 = ((_OverlayEntryLocation__overlay)fromLocation)._theater;
        _OverlayEntryWidgetState__overlay fromModel__81355 = ((_OverlayEntryLocation__overlay)fromLocation)._childModel;
        if ((!object.Equals(fromTheater__81283, this._theater)))
        {
            fromTheater__81283._removeDeferredChild(child);
            this._theater._addDeferredChild(child);
        }
        if (((!object.Equals(fromModel__81355, this._childModel)) || (((_OverlayEntryLocation__overlay)fromLocation)._zOrderIndex != this._zOrderIndex)))
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

    public override string ToString() => $"{(global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "_OverlayEntryLocation"))}[{(global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.shortHash(this))}] {((this._debugMarkLocationInvalidStackTrace is not null) ? "(INVALID)" : "")}";
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
        _RenderTheaterMarker__overlay? marker__85264 = ((_RenderTheaterMarker__overlay?)(object?)_RenderTheaterMarker__overlay.maybeOf(context, targetRootOverlay: targetRootOverlay));
        if ((marker__85264 is not null))
        {
            return marker__85264;
        }
        throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary("No Overlay widget found."), new global::Doroti.Generated.Framework.Foundation.ErrorDescription($"{DartRuntimePrimitives.RuntimeType(((BuildContext)context).widget)} widgets require an Overlay widget ancestor.\n" + "An overlay lets widgets float on top of other widget children."), new global::Doroti.Generated.Framework.Foundation.ErrorHint("To introduce an Overlay widget, you can either directly " + "include one, or use a widget that contains an Overlay itself, " + "such as a Navigator, WidgetApp, MaterialApp, or CupertinoApp.") }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static _RenderTheaterMarker__overlay? maybeOf(BuildContext context, bool targetRootOverlay = false, bool createDependency = true)
    {
        if (targetRootOverlay)
        {
            InheritedElement? ancestor__86204 = ((InheritedElement?)(object?)_RenderTheaterMarker__overlay._rootRenderTheaterMarkerOf(LookupBoundary.getElementForInheritedWidgetOfExactType<_RenderTheaterMarker__overlay>(context)));
            DartRuntimePrimitives.Assert(() => ((ancestor__86204 is null) || (ancestor__86204.widget is _RenderTheaterMarker__overlay)));
            if ((ancestor__86204 is null))
            {
                return ((_RenderTheaterMarker__overlay)(object)null);
            }
            if (createDependency)
            {
                return ((_RenderTheaterMarker__overlay?)(object?)context.dependOnInheritedElement(ancestor__86204))!;
            }
            return ((_RenderTheaterMarker__overlay?)(object?)ancestor__86204.widget)!;
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
        InheritedElement? ancestor__87166 = default!;
        theaterMarkerElement.visitAncestorElements(((global::System.Func<Element, bool>)((element) => {
ancestor__87166 = LookupBoundary.getElementForInheritedWidgetOfExactType<_RenderTheaterMarker__overlay>(element);
return false;
throw new InvalidOperationException("Dart closure completed without a value.");
})));
        return ((ancestor__87166 is null) ? theaterMarkerElement : _RenderTheaterMarker__overlay._rootRenderTheaterMarkerOf(ancestor__87166));
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
    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context) => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Rendering.RenderObject>(new _RenderLayoutSurrogateProxyBox__overlay(this.overlayLocation));
    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
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

    public override global::Doroti.Generated.Framework.Rendering.RenderObject renderObject => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Rendering.RenderObject>(((_RenderLayoutSurrogateProxyBox__overlay?)(object?)base.renderObject)!);
    public override void mount(Element? parent, object? newSlot)
    {
        base.mount(parent, newSlot);
        var widget__88917 = ((_OverlayPortal__overlay?)(object?)this.widget)!;
        _child = updateChild(this._child, ((_OverlayPortal__overlay)widget__88917).child, null);
        _overlayChild = updateChild(this._overlayChild, ((_OverlayPortal__overlay)widget__88917).overlayChild, ((_OverlayPortal__overlay)widget__88917).overlayLocation);
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
        Element? child__89756 = this._child;
        Element? overlayChild__89791 = this._overlayChild;
        if ((child__89756 is not null))
        {
            visitor(child__89756);
        }
        if ((overlayChild__89791 is not null))
        {
            visitor(overlayChild__89791);
        }
    }

    public override void insertRenderObjectChild(global::Doroti.Generated.Framework.Rendering.RenderObject child, object? slot)
    {
        var __child = (global::Doroti.Generated.Framework.Rendering.RenderBox)(object)child;
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

    public override void moveRenderObjectChild(global::Doroti.Generated.Framework.Rendering.RenderObject child, object? oldSlot, object? newSlot)
    {
        var __child = (_RenderDeferredLayoutBox__overlay)(object)child;
        var __oldSlot = (_OverlayEntryLocation__overlay)(object)oldSlot;
        var __newSlot = (_OverlayEntryLocation__overlay)(object)newSlot;
        DartRuntimePrimitives.Assert(() => __newSlot._debugIsLocationValid());
        ((dynamic)__newSlot)._moveChild(__child, __oldSlot);
        this.renderObject.markNeedsSemanticsUpdate();
    }

    public override void removeRenderObjectChild(global::Doroti.Generated.Framework.Rendering.RenderObject child, object? slot)
    {
        var __child = (global::Doroti.Generated.Framework.Rendering.RenderBox)(object)child;
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

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<Element>("child", this._child, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<Element>("overlayChild", this._overlayChild, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<object>("overlayLocation", this._overlayChild?.slot, defaultValue: null));
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

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        _RenderLayoutSurrogateProxyBox__overlay parent__92310 = ((_RenderLayoutSurrogateProxyBox__overlay)(object?)getLayoutParent(context));
        var renderObject__92355 = new _RenderDeferredLayoutBox__overlay(parent__92310, this.childIdentifier);
        ((dynamic)parent__92310)._deferredLayoutChild = renderObject__92355;
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)renderObject__92355);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderDeferredLayoutBox__overlay)(object)renderObject;
        DartRuntimePrimitives.Assert(() => (object.Equals(((_RenderDeferredLayoutBox__overlay)__renderObject)._layoutSurrogate, getLayoutParent(context))));
        DartRuntimePrimitives.Assert(() => (object.Equals(((_RenderDeferredLayoutBox__overlay?)((dynamic)getLayoutParent(context))._deferredLayoutChild), __renderObject)));
        __renderObject.childIdentifier = this.childIdentifier;
    }

}

public class _RenderDeferredLayoutBox__overlay : global::Doroti.Generated.Framework.Rendering.RenderProxyBox, _RenderTheaterMixin__overlay
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

    public virtual global::Doroti.Generated.Framework.Rendering.StackParentData stackParentData => ((global::Doroti.Generated.Framework.Rendering.StackParentData?)(object?)this.parentData!)!;
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
    public virtual IEnumerable<global::Doroti.Generated.Framework.Rendering.RenderBox> _childrenInPaintOrder()
    {
        global::Doroti.Generated.Framework.Rendering.RenderBox? child__94663 = ((global::Doroti.Generated.Framework.Rendering.RenderBox?)((dynamic)this).child);
        return ((child__94663 is null) ? System.Linq.Enumerable.Empty<global::Doroti.Generated.Framework.Rendering.RenderBox>() : System.Linq.Enumerable.Range(0, checked((int)1L)).Select(__index => ((Func<long, global::Doroti.Generated.Framework.Rendering.RenderBox>)((i) => child__94663))(checked((long)__index))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual IEnumerable<global::Doroti.Generated.Framework.Rendering.RenderBox> _childrenInHitTestOrder() => _childrenInPaintOrder();
    public virtual _RenderTheater__overlay theater => (this.parent switch { _RenderTheater__overlay parent__94994 => parent__94994, _ => throw DartRuntimePrimitives.AsException(global::Doroti.Generated.Framework.Foundation.FlutterError.Create($"{this.parent} of {this} is not a _RenderTheater")) });
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

    public override double? computeDryBaseline(global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints, TextBaseline baseline)
    {
        global::Doroti.Generated.Framework.Rendering.RenderBox? child__95899 = ((global::Doroti.Generated.Framework.Rendering.RenderBox?)((dynamic)this).child);
        if ((child__95899 is null))
        {
            return null;
        }
        return _RenderTheaterMixin__overlay.baselineForChild(child__95899, ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints).biggest, constraints, ((_RenderTheater__overlay)this.theater)._resolvedAlignment, baseline);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject? debugLayoutParent => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Rendering.RenderObject>(this._layoutSurrogate);
    internal virtual void _doLayoutFrom(global::Doroti.Generated.Framework.Rendering.RenderObject treewalkParent, global::Doroti.Generated.Framework.Rendering.Constraints constraints)
    {
        bool shouldAddToDirtyList__96504 = (this.needsLayout || (!object.Equals(this.constraints, constraints)));
        DartRuntimePrimitives.Assert(() => !this._doingLayoutFromTreeWalk);
        _doingLayoutFromTreeWalk = true;
        base.layout(constraints);
        DartRuntimePrimitives.Assert(() => this._doingLayoutFromTreeWalk);
        _doingLayoutFromTreeWalk = false;
        _needsLayout = false;
        DartRuntimePrimitives.Assert(() => !this.debugNeedsLayout);
        if (shouldAddToDirtyList__96504)
        {
            ((dynamic)treewalkParent).invokeLayoutCallback(((global::System.Action<global::Doroti.Generated.Framework.Rendering.BoxConstraints>)((_) => {
markNeedsLayout();
})));
        }
    }

    public override void layout(global::Doroti.Generated.Framework.Rendering.Constraints constraints, bool parentUsesSize = false)
    {
        _doLayoutFrom(this.parent!, constraints: constraints);
    }

    public override void performResize()
    {
        size = ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)this.constraints).biggest;
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
        global::Doroti.Generated.Framework.Rendering.RenderBox? child__98700 = ((global::Doroti.Generated.Framework.Rendering.RenderBox?)((dynamic)this).child);
        if ((child__98700 is null))
        {
            _needsLayout = false;
            return;
        }
        DartRuntimePrimitives.Assert(() => ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)this.constraints).isTight);
        layoutChild(child__98700, this.constraints);
        DartRuntimePrimitives.Assert(() =>
            {
                _debugMutationsLocked = false;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        _needsLayout = false;
    }

    public override void describeSemanticsConfiguration(global::Doroti.Generated.Framework.Semantics.SemanticsConfiguration config)
    {
        base.describeSemanticsConfiguration(config);
        if ((this.childIdentifier is not null))
        {
            config.traversalChildIdentifier = this.childIdentifier;
        }
    }

    public override void applyPaintTransform(global::Doroti.Generated.Framework.Rendering.RenderObject child, Matrix4 transform)
    {
        var __child = (global::Doroti.Generated.Framework.Rendering.RenderBox)(object)child;
        var childParentData__99299 = ((global::Doroti.Generated.Framework.Rendering.BoxParentData?)(object?)__child.parentData!)!;
        global::Doroti.Ui.Offset offset__99370 = ((global::Doroti.Ui.Offset)(object?)((global::Doroti.Generated.Framework.Rendering.BoxParentData)childParentData__99299).offset);
        transform.translateByDouble(offset__99370.dx, offset__99370.dy, 0, 1);
    }

    public override void setupParentData(global::Doroti.Generated.Framework.Rendering.RenderObject child)
    {
        var __child = (global::Doroti.Generated.Framework.Rendering.RenderBox)(object)child;
        if ((__child.parentData is not global::Doroti.Generated.Framework.Rendering.StackParentData))
        {
            __child.parentData = new global::Doroti.Generated.Framework.Rendering.StackParentData();
        }
    }

    public override double? computeDistanceToActualBaseline(TextBaseline baseline)
    {
        DartRuntimePrimitives.Assert(() => !this.debugNeedsLayout);
        global::Doroti.Generated.Framework.Rendering.BaselineOffset baselineOffset__39497 = global::Doroti.Generated.Framework.Rendering.BaselineOffset.noBaseline;
        foreach (global::Doroti.Generated.Framework.Rendering.RenderBox child__39566 in _childrenInPaintOrder())
        {
            DartRuntimePrimitives.Assert(() => !child__39566.debugNeedsLayout);
            var childParentData__39653 = ((global::Doroti.Generated.Framework.Rendering.StackParentData?)(object?)child__39566.parentData!)!;
            baselineOffset__39497 = baselineOffset__39497.minOf((new global::Doroti.Generated.Framework.Rendering.BaselineOffset(child__39566.getDistanceToActualBaseline(baseline)).op_Add(childParentData__39653.offset.dy)));
        }
        return baselineOffset__39497.offset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void layoutChild(global::Doroti.Generated.Framework.Rendering.RenderBox child, global::Doroti.Generated.Framework.Rendering.BoxConstraints nonPositionedChildConstraints)
    {
        var childParentData__40970 = ((global::Doroti.Generated.Framework.Rendering.StackParentData?)(object?)child.parentData!)!;
        global::Doroti.Generated.Framework.Painting.Alignment alignment__41046 = ((_RenderTheater__overlay)this.theater)._resolvedAlignment;
        if (!((global::Doroti.Generated.Framework.Rendering.StackParentData)childParentData__40970).isPositioned)
        {
            child.layout(nonPositionedChildConstraints, parentUsesSize: true);
            childParentData__40970.offset = Offset.zero;
        }
        else
        {
            DartRuntimePrimitives.Assert(() => (child is not _RenderDeferredLayoutBox__overlay), () => (object?)"all _RenderDeferredLayoutBoxes must be non-positioned children.");
            RenderStack.layoutPositionedChild(child, childParentData__40970, this.size, alignment__41046);
        }
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parentData, childParentData__40970)));
    }

    public override bool hitTestChildren(global::Doroti.Generated.Framework.Rendering.BoxHitTestResult result, Offset position)
    {
        IEnumerator<global::Doroti.Generated.Framework.Rendering.RenderBox> iterator__41661 = _childrenInHitTestOrder().GetEnumerator();
        var isHit__41716 = false;
        while ((!isHit__41716 && iterator__41661.MoveNext()))
        {
            global::Doroti.Generated.Framework.Rendering.RenderBox child__41797 = iterator__41661.Current;
            var childParentData__41835 = ((global::Doroti.Generated.Framework.Rendering.StackParentData?)(object?)child__41797.parentData!)!;
            var localChild__41903 = child__41797;
            bool childHitTest(global::Doroti.Generated.Framework.Rendering.BoxHitTestResult result, Offset position)
            {
                return localChild__41903.hitTest(result, position: position);
                throw new InvalidOperationException("Dart control flow completed without a value.");
            }
            isHit__41716 = result.addWithPaintOffset(offset: childParentData__41835.offset, position: position, hitTest: (global::System.Func<global::Doroti.Generated.Framework.Rendering.BoxHitTestResult, Offset, bool>)childHitTest);
        }
        return isHit__41716;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(global::Doroti.Generated.Framework.Rendering.PaintingContext context, Offset offset)
    {
        foreach (global::Doroti.Generated.Framework.Rendering.RenderBox child__42320 in _childrenInPaintOrder())
        {
            var childParentData__42368 = ((global::Doroti.Generated.Framework.Rendering.StackParentData?)(object?)child__42320.parentData!)!;
            context.paintChild(child__42320, (childParentData__42368.offset + offset));
        }
    }

}

public class _RenderLayoutSurrogateProxyBox__overlay : global::Doroti.Generated.Framework.Rendering.RenderProxyBox
{
    internal virtual _RenderDeferredLayoutBox__overlay? _deferredLayoutChild { get; set; } = default;
    public virtual _OverlayEntryLocation__overlay? overlayLocation { get; set; } = default;
    internal virtual bool _debugIsFirstAttach { get; set; } = true;
    internal virtual bool _didDetachDeferredChild { get; set; } = false;

    internal _RenderLayoutSurrogateProxyBox__overlay(_OverlayEntryLocation__overlay? overlayLocation)
    {
        this.overlayLocation = overlayLocation;
    }

    public override void attach(global::Doroti.Generated.Framework.Rendering.PipelineOwner owner)
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
        if (this._deferredLayoutChild is object deferredChild__101178 && (((_RenderDeferredLayoutBox__overlay)deferredChild__101178).theater.attached))
        {
            this.overlayLocation!._detachFromLayoutSurrogate(DartRuntimePrimitives.ConvertValue<_RenderDeferredLayoutBox__overlay>(deferredChild__101178));
            _didDetachDeferredChild = true;
        }
        base.detach();
    }

    public override void redepthChildren()
    {
        base.redepthChildren();
        _RenderDeferredLayoutBox__overlay? child__101471 = this._deferredLayoutChild;
        if (((child__101471 is not null) && child__101471.attached))
        {
            redepthChild(child__101471);
        }
    }

    public override void performLayout()
    {
        base.performLayout();
        _RenderDeferredLayoutBox__overlay? deferredChild__101817 = this._deferredLayoutChild;
        if ((deferredChild__101817 is null))
        {
            return;
        }
        var theater__102533 = ((_RenderTheater__overlay?)(object?)deferredChild__101817.parent!)!;
        if (!((_RenderTheater__overlay)theater__102533)._layingOutSizeDeterminingChild)
        {
            global::Doroti.Generated.Framework.Rendering.BoxConstraints theaterConstraints__103066 = theater__102533.constraints;
            global::Doroti.Ui.Size boxSize__103125 = ((global::Doroti.Ui.Size)(object?)(((global::Doroti.Generated.Framework.Rendering.BoxConstraints)theaterConstraints__103066).biggest.isFinite ? ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)theaterConstraints__103066).biggest : theater__102533.size));
            deferredChild__101817._doLayoutFrom(this, constraints: global::Doroti.Generated.Framework.Rendering.BoxConstraints.CreateTight(boxSize__103125));
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

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context) => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Rendering.RenderObject>(new _RenderLayoutBuilder__overlay());
}

internal class _RenderLayoutBuilder__overlay : global::Doroti.Generated.Framework.Rendering.RenderProxyBox, _RenderTheaterMixin__overlay, RenderAbstractLayoutBuilderMixin<OverlayChildLayoutInfo, global::Doroti.Generated.Framework.Rendering.RenderBox>, global::Doroti.Generated.Framework.Rendering.IRenderLayoutCallback
{
    internal virtual OverlayChildLayoutInfo? _layoutInfo { get; set; } = default;
    internal virtual long? _callbackId { get; set; } = default;
    internal const string _speculativeLayoutErrorMessage = "This RenderObject should not be reachable in intrinsic dimension calculations.";
    public virtual global::System.Action<global::Doroti.Generated.Framework.Rendering.Constraints>? _callback { get; set; } = default;

    public virtual IEnumerable<global::Doroti.Generated.Framework.Rendering.RenderBox> _childrenInPaintOrder()
    {
        global::Doroti.Generated.Framework.Rendering.RenderBox? child__104594 = ((global::Doroti.Generated.Framework.Rendering.RenderBox?)((dynamic)this).child);
        return ((child__104594 is null) ? System.Linq.Enumerable.Empty<global::Doroti.Generated.Framework.Rendering.RenderBox>() : System.Linq.Enumerable.Range(0, checked((int)1L)).Select(__index => ((Func<long, global::Doroti.Generated.Framework.Rendering.RenderBox>)((i) => child__104594))(checked((long)__index))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual IEnumerable<global::Doroti.Generated.Framework.Rendering.RenderBox> _childrenInHitTestOrder() => _childrenInPaintOrder();
    public virtual _RenderTheater__overlay theater => (this.parent switch { _RenderDeferredLayoutBox__overlay parent__104935 => ((_RenderDeferredLayoutBox__overlay)parent__104935).theater, _ => throw DartRuntimePrimitives.AsException(global::Doroti.Generated.Framework.Foundation.FlutterError.Create($"{this.parent} of {this} is not a _RenderDeferredLayoutBox")) });
    public override bool sizedByParent => true;
    public override void performResize() => size = ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)this.constraints).biggest;
    public virtual void applyPaintTransform(global::Doroti.Generated.Framework.Rendering.RenderObject child, Matrix4 transform)
    {
        var __child = (global::Doroti.Generated.Framework.Rendering.RenderBox)(object)child;
        var childParentData__105251 = ((global::Doroti.Generated.Framework.Rendering.BoxParentData?)(object?)__child.parentData!)!;
        global::Doroti.Ui.Offset offset__105322 = ((global::Doroti.Ui.Offset)(object?)((global::Doroti.Generated.Framework.Rendering.BoxParentData)childParentData__105251).offset);
        transform.translateByDouble(offset__105322.dx, offset__105322.dy, 0, 1);
    }

    public virtual OverlayChildLayoutInfo layoutInfo => DartRuntimePrimitives.ConvertValue<OverlayChildLayoutInfo>(this._layoutInfo!);
    internal virtual OverlayChildLayoutInfo _computeNewLayoutInfo()
    {
        _RenderTheater__overlay theater__105710 = this.theater;
        var parent__105744 = ((_RenderDeferredLayoutBox__overlay?)(object?)this.parent!)!;
        _RenderLayoutSurrogateProxyBox__overlay layoutSurrogate__105836 = ((_RenderDeferredLayoutBox__overlay)parent__105744)._layoutSurrogate;
        DartRuntimePrimitives.Assert(() =>
            {
                for (global::Doroti.Generated.Framework.Rendering.RenderObject? node__105929 = layoutSurrogate__105836; ((node__105929 is not null) && (!object.Equals(node__105929, theater__105710))); node__105929 = ((global::Doroti.Generated.Framework.Rendering.RenderObject)node__105929).parent)
                {
                    if ((node__105929 is global::Doroti.Generated.Framework.Rendering.RenderFollowerLayer))
                    {
                        global::Doroti.Generated.Framework.Rendering.RenderFollowerLayer node__105929__as106043 = (global::Doroti.Generated.Framework.Rendering.RenderFollowerLayer)node__105929;
                        throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary("The paint transform cannot be reliably computed because of RenderFollowerLayer(s)"), ((global::Doroti.Generated.Framework.Rendering.RenderFollowerLayer)node__105929__as106043).describeForError("The RenderFollowerLayer was"), new global::Doroti.Generated.Framework.Foundation.ErrorDescription("RenderFollowerLayer establishes its paint transform only after the layout phase."), new global::Doroti.Generated.Framework.Foundation.ErrorHint("Consider replacing the corresponding CompositedTransformFollower with OverlayPortal.overlayChildLayoutBuilder if possible.") }));
                    }
                    DartRuntimePrimitives.Assert(() => (((global::Doroti.Generated.Framework.Rendering.RenderObject)node__105929).depth > theater__105710.depth));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        DartRuntimePrimitives.Assert(() => layoutSurrogate__105836.hasSize);
        DartRuntimePrimitives.Assert(() => (((global::Doroti.Generated.Framework.Rendering.RenderBox?)((dynamic)layoutSurrogate__105836).child)?.hasSize ?? true));
        DartRuntimePrimitives.Assert(() => ((((global::Doroti.Generated.Framework.Rendering.RenderBox?)((dynamic)layoutSurrogate__105836).child) is null) || (object.Equals(((global::Doroti.Generated.Framework.Rendering.RenderBox?)((dynamic)layoutSurrogate__105836).child)!.size, layoutSurrogate__105836.size))));
        DartRuntimePrimitives.Assert(() => (object.Equals(this.size, theater__105710.size)));
        DartRuntimePrimitives.Assert(() => (((global::Doroti.Generated.Framework.Rendering.RenderBox?)((dynamic)layoutSurrogate__105836).child)?.getTransformTo(layoutSurrogate__105836).isIdentity() ?? true));
        DartRuntimePrimitives.Assert(() => getTransformTo(theater__105710).isIdentity());
        global::Doroti.Ui.Size overlayPortalSize__107271 = ((global::Doroti.Ui.Size)(object?)((_RenderDeferredLayoutBox__overlay)parent__105744)._layoutSurrogate.size);
        Matrix4 paintTransform__107339 = ((Matrix4)(object?)layoutSurrogate__105836.getTransformTo(theater__105710));
        return OverlayChildLayoutInfo.Create_((overlayPortalSize__107271, paintTransform__107339, this.size));
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
        if (this.child is global::Doroti.Generated.Framework.Rendering.RenderBox child__107737)
        {
            layoutChild(child__107737, this.constraints);
        }
        DartRuntimePrimitives.Assert(() => (this._callbackId is null));
        _callbackId ??= global::Doroti.Generated.Framework.Scheduler.SchedulerBinding.instance.scheduleFrameCallback((global::System.Action<Duration>)this._frameCallback, scheduleNewFrame: false);
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

    public virtual Size computeDryLayout(global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints)
    {
        DartRuntimePrimitives.Assert(() => debugCannotComputeDryLayout(reason: _speculativeLayoutErrorMessage));
        return Size.zero;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double? computeDryBaseline(global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints, TextBaseline baseline)
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
        if (this._callbackId is long callbackId__109858)
        {
            global::Doroti.Generated.Framework.Scheduler.SchedulerBinding.instance.cancelFrameCallbackWithId(callbackId__109858);
        }
        base.dispose();
    }

    public virtual void setupParentData(global::Doroti.Generated.Framework.Rendering.RenderObject child)
    {
        var __child = (global::Doroti.Generated.Framework.Rendering.RenderBox)(object)child;
        if ((__child.parentData is not global::Doroti.Generated.Framework.Rendering.StackParentData))
        {
            __child.parentData = new global::Doroti.Generated.Framework.Rendering.StackParentData();
        }
    }

    public virtual double? computeDistanceToActualBaseline(TextBaseline baseline)
    {
        DartRuntimePrimitives.Assert(() => !this.debugNeedsLayout);
        global::Doroti.Generated.Framework.Rendering.BaselineOffset baselineOffset__39497 = global::Doroti.Generated.Framework.Rendering.BaselineOffset.noBaseline;
        foreach (global::Doroti.Generated.Framework.Rendering.RenderBox child__39566 in _childrenInPaintOrder())
        {
            DartRuntimePrimitives.Assert(() => !child__39566.debugNeedsLayout);
            var childParentData__39653 = ((global::Doroti.Generated.Framework.Rendering.StackParentData?)(object?)child__39566.parentData!)!;
            baselineOffset__39497 = baselineOffset__39497.minOf((new global::Doroti.Generated.Framework.Rendering.BaselineOffset(child__39566.getDistanceToActualBaseline(baseline)).op_Add(childParentData__39653.offset.dy)));
        }
        return baselineOffset__39497.offset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void layoutChild(global::Doroti.Generated.Framework.Rendering.RenderBox child, global::Doroti.Generated.Framework.Rendering.BoxConstraints nonPositionedChildConstraints)
    {
        var childParentData__40970 = ((global::Doroti.Generated.Framework.Rendering.StackParentData?)(object?)child.parentData!)!;
        global::Doroti.Generated.Framework.Painting.Alignment alignment__41046 = ((_RenderTheater__overlay)this.theater)._resolvedAlignment;
        if (!((global::Doroti.Generated.Framework.Rendering.StackParentData)childParentData__40970).isPositioned)
        {
            child.layout(nonPositionedChildConstraints, parentUsesSize: true);
            childParentData__40970.offset = Offset.zero;
        }
        else
        {
            DartRuntimePrimitives.Assert(() => (child is not _RenderDeferredLayoutBox__overlay), () => (object?)"all _RenderDeferredLayoutBoxes must be non-positioned children.");
            RenderStack.layoutPositionedChild(child, childParentData__40970, this.size, alignment__41046);
        }
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parentData, childParentData__40970)));
    }

    public virtual bool hitTestChildren(global::Doroti.Generated.Framework.Rendering.BoxHitTestResult result, Offset position)
    {
        IEnumerator<global::Doroti.Generated.Framework.Rendering.RenderBox> iterator__41661 = _childrenInHitTestOrder().GetEnumerator();
        var isHit__41716 = false;
        while ((!isHit__41716 && iterator__41661.MoveNext()))
        {
            global::Doroti.Generated.Framework.Rendering.RenderBox child__41797 = iterator__41661.Current;
            var childParentData__41835 = ((global::Doroti.Generated.Framework.Rendering.StackParentData?)(object?)child__41797.parentData!)!;
            var localChild__41903 = child__41797;
            bool childHitTest(global::Doroti.Generated.Framework.Rendering.BoxHitTestResult result, Offset position)
            {
                return localChild__41903.hitTest(result, position: position);
                throw new InvalidOperationException("Dart control flow completed without a value.");
            }
            isHit__41716 = result.addWithPaintOffset(offset: childParentData__41835.offset, position: position, hitTest: (global::System.Func<global::Doroti.Generated.Framework.Rendering.BoxHitTestResult, Offset, bool>)childHitTest);
        }
        return isHit__41716;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void paint(global::Doroti.Generated.Framework.Rendering.PaintingContext context, Offset offset)
    {
        foreach (global::Doroti.Generated.Framework.Rendering.RenderBox child__42320 in _childrenInPaintOrder())
        {
            var childParentData__42368 = ((global::Doroti.Generated.Framework.Rendering.StackParentData?)(object?)child__42320.parentData!)!;
            context.paintChild(child__42320, (childParentData__42368.offset + offset));
        }
    }

    public virtual void _updateCallback(global::System.Action<global::Doroti.Generated.Framework.Rendering.Constraints> value)
    {
        if ((object.Equals((global::System.Action<global::Doroti.Generated.Framework.Rendering.Constraints>)value, (global::System.Action<global::Doroti.Generated.Framework.Rendering.Constraints>?)this._callback)))
        {
            return;
        }
        this._callback = (global::System.Action<global::Doroti.Generated.Framework.Rendering.Constraints>)value;
        scheduleLayoutCallback();
    }

}
