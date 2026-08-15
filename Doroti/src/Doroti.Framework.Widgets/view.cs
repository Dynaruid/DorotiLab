// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/view.dart
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

public class View : StatefulWidget
{
    public virtual DorotiView view { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;
    internal virtual global::Doroti.Generated.Framework.Rendering.PipelineOwner? _deprecatedPipelineOwner { get; private set; }
    internal virtual global::Doroti.Generated.Framework.Rendering.RenderView? _deprecatedRenderView { get; private set; }

    public View(global::Doroti.Generated.Framework.Foundation.Key? key = null, DorotiView view = default!, global::Doroti.Generated.Framework.Rendering.PipelineOwner? deprecatedDoNotUseWillBeRemovedWithoutNoticePipelineOwner = null, global::Doroti.Generated.Framework.Rendering.RenderView? deprecatedDoNotUseWillBeRemovedWithoutNoticeRenderView = null, Widget child = default!) : base(key: key)
    {
        this.view = view;
        this.child = child;
        this._deprecatedPipelineOwner = deprecatedDoNotUseWillBeRemovedWithoutNoticePipelineOwner;
        this._deprecatedRenderView = deprecatedDoNotUseWillBeRemovedWithoutNoticeRenderView;
        System.Diagnostics.Debug.Assert((((deprecatedDoNotUseWillBeRemovedWithoutNoticePipelineOwner is null)) == ((deprecatedDoNotUseWillBeRemovedWithoutNoticeRenderView is null))));
        System.Diagnostics.Debug.Assert(((deprecatedDoNotUseWillBeRemovedWithoutNoticeRenderView is null) || (object.Equals(((global::Doroti.Generated.Framework.Rendering.RenderView)deprecatedDoNotUseWillBeRemovedWithoutNoticeRenderView).flutterView, view))));
    }

    public static global::Doroti.Ui.DorotiView? maybeOf(BuildContext context)
    {
        return ((global::Doroti.Ui.DorotiView?)(object?)LookupBoundary.dependOnInheritedWidgetOfExactType<_ViewScope__view>(context)?.view);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static global::Doroti.Ui.DorotiView of(BuildContext context)
    {
        global::Doroti.Ui.DorotiView? result__7300 = ((global::Doroti.Ui.DorotiView?)(object?)View.maybeOf(context));
        DartRuntimePrimitives.Assert(() =>
            {
                if ((result__7300 is null))
                {
                    bool hiddenByBoundary__7390 = LookupBoundary.debugIsHidingAncestorWidgetOfExactType<_ViewScope__view>(context);
                    var information__7511 = new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorDescription("The context used was:\n" + $"  {context}"), new global::Doroti.Generated.Framework.Foundation.ErrorHint("This usually means that the provided context is not associated with a View.") };
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(information__7511));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return ((global::Doroti.Ui.DorotiView)(object?)result__7300!);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static global::Doroti.Generated.Framework.Rendering.PipelineOwner pipelineOwnerOf(BuildContext context)
    {
        return (context.dependOnInheritedWidgetOfExactType<_PipelineOwnerScope__view>()?.pipelineOwner ?? global::Doroti.Generated.Framework.Rendering.RendererBinding.instance.rootPipelineOwner);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _ViewState__view());
}

internal class _ViewState__view : State<View>, WidgetsBindingObserver
{
    internal virtual FocusScopeNode _scopeNode { get; private set; } = new FocusScopeNode(debugLabel: (global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kReleaseMode ? null : "View Scope"));
    internal virtual FocusTraversalPolicy _policy { get; private set; } = ((FocusTraversalPolicy)(object?)new ReadingOrderTraversalPolicy());
    internal virtual bool _viewHasFocus { get; set; } = false;

    public override void initState()
    {
        base.initState();
        WidgetsBinding.instance.addObserver(this);
        this._scopeNode.addListener(() => this._scopeFocusChangeListener());
    }

    public override void dispose()
    {
        WidgetsBinding.instance.removeObserver(this);
        this._scopeNode.removeListener(() => this._scopeFocusChangeListener());
        this._scopeNode.dispose();
        base.dispose();
    }

    internal virtual void _scopeFocusChangeListener()
    {
        if (((this._viewHasFocus == this._scopeNode.hasFocus) || !this._scopeNode.hasFocus))
        {
            return;
        }
        WidgetsBinding.instance.platformDispatcher.requestViewFocusChange(direction: global::Doroti.Ui.ViewFocusDirection.forward, state: global::Doroti.Ui.ViewFocusState.focused, viewId: checked((long)((View)(object)this.widget).view.viewId));
    }

    public virtual void didChangeViewFocus(ViewFocusEvent @event)
    {
        _viewHasFocus = (@event.state switch { var __constant10380 when (object.Equals(__constant10380, global::Doroti.Ui.ViewFocusState.focused)) => (checked((long)@event.viewId) == checked((long)((View)(object)this.widget).view.viewId)), var __constant10448 when (object.Equals(__constant10448, global::Doroti.Ui.ViewFocusState.unfocused)) => false, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        if ((checked((long)@event.viewId) != checked((long)((View)(object)this.widget).view.viewId)))
        {
            return;
        }
        FocusNode nextFocus__10570 = default!;
        switch (@event.state)
        {
            case var __constant10619 when (object.Equals(__constant10619, global::Doroti.Ui.ViewFocusState.focused)):
                {
                    switch (@event.direction)
                    {
                        case var __constant10693 when (object.Equals(__constant10693, global::Doroti.Ui.ViewFocusDirection.forward)):
                            {
                                nextFocus__10570 = (this._policy.findFirstFocus(this._scopeNode, ignoreCurrentFocus: true) ?? this._scopeNode);
                                break;
                            }
                        case var __constant10836 when (object.Equals(__constant10836, global::Doroti.Ui.ViewFocusDirection.backward)):
                            {
                                nextFocus__10570 = this._policy.findLastFocus(this._scopeNode, ignoreCurrentFocus: true);
                                break;
                            }
                        case var __constant10965 when (object.Equals(__constant10965, global::Doroti.Ui.ViewFocusDirection.undefined)):
                            {
                                nextFocus__10570 = DartRuntimePrimitives.ConvertValue<FocusNode>(this._scopeNode);
                                break;
                            }
                    }
                    nextFocus__10570.requestFocus();
                    break;
                }
            case var __constant11086 when (object.Equals(__constant11086, global::Doroti.Ui.ViewFocusState.unfocused)):
                {
                    FocusManager.instance.rootScope.requestScopeFocus();
                    break;
                }
        }
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new RawView(view: ((View)(object)this.widget).view, deprecatedDoNotUseWillBeRemovedWithoutNoticePipelineOwner: ((View)(object)this.widget)._deprecatedPipelineOwner, deprecatedDoNotUseWillBeRemovedWithoutNoticeRenderView: ((View)(object)this.widget)._deprecatedRenderView, child: MediaQuery.fromView(view: ((View)(object)this.widget).view, child: new FocusTraversalGroup(policy: this._policy, parentNode: FocusManager.instance.rootScope, child: FocusScope.CreateWithExternalFocusNode(includeSemantics: false, focusScopeNode: this._scopeNode, child: ((View)(object)this.widget).child)))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class RawView : StatelessWidget
{
    public virtual DorotiView view { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;
    internal virtual global::Doroti.Generated.Framework.Rendering.PipelineOwner? _deprecatedPipelineOwner { get; private set; }
    internal virtual global::Doroti.Generated.Framework.Rendering.RenderView? _deprecatedRenderView { get; private set; }

    public RawView(global::Doroti.Generated.Framework.Foundation.Key? key = null, DorotiView view = default!, global::Doroti.Generated.Framework.Rendering.PipelineOwner? deprecatedDoNotUseWillBeRemovedWithoutNoticePipelineOwner = null, global::Doroti.Generated.Framework.Rendering.RenderView? deprecatedDoNotUseWillBeRemovedWithoutNoticeRenderView = null, Widget child = default!) : base(key: key)
    {
        this.view = view;
        this.child = child;
        this._deprecatedPipelineOwner = deprecatedDoNotUseWillBeRemovedWithoutNoticePipelineOwner;
        this._deprecatedRenderView = deprecatedDoNotUseWillBeRemovedWithoutNoticeRenderView;
        System.Diagnostics.Debug.Assert((((deprecatedDoNotUseWillBeRemovedWithoutNoticePipelineOwner is null)) == ((deprecatedDoNotUseWillBeRemovedWithoutNoticeRenderView is null))));
        System.Diagnostics.Debug.Assert(((deprecatedDoNotUseWillBeRemovedWithoutNoticeRenderView is null) || (object.Equals(((global::Doroti.Generated.Framework.Rendering.RenderView)deprecatedDoNotUseWillBeRemovedWithoutNoticeRenderView).flutterView, view))));
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new _RawViewInternal__view(view: this.view, deprecatedPipelineOwner: this._deprecatedPipelineOwner, deprecatedRenderView: this._deprecatedRenderView, builder: ((global::System.Func<BuildContext, global::Doroti.Generated.Framework.Rendering.PipelineOwner, Widget>)((context, owner) => {
return ((Widget)(object?)new _ViewScope__view(view: this.view, child: new _PipelineOwnerScope__view(pipelineOwner: owner, child: this.child)));
throw new InvalidOperationException("Dart closure completed without a value.");
}))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal delegate Widget _RawViewContentBuilder__view(BuildContext context, global::Doroti.Generated.Framework.Rendering.PipelineOwner owner);

public class _RawViewInternal__view : RenderObjectWidget
{
    public virtual DorotiView view { get; private set; } = default!;
    public virtual global::System.Func<BuildContext, global::Doroti.Generated.Framework.Rendering.PipelineOwner, Widget> builder { get; private set; } = default!;
    internal virtual global::Doroti.Generated.Framework.Rendering.PipelineOwner? _deprecatedPipelineOwner { get; private set; }
    internal virtual global::Doroti.Generated.Framework.Rendering.RenderView? _deprecatedRenderView { get; private set; }

    internal _RawViewInternal__view(DorotiView view, global::Doroti.Generated.Framework.Rendering.PipelineOwner? deprecatedPipelineOwner, global::Doroti.Generated.Framework.Rendering.RenderView? deprecatedRenderView, global::System.Func<BuildContext, global::Doroti.Generated.Framework.Rendering.PipelineOwner, Widget> builder) : base(key: new _DeprecatedRawViewKey__view<IState>(view, deprecatedPipelineOwner, deprecatedRenderView))
    {
        this.view = view;
        this.builder = builder;
        this._deprecatedPipelineOwner = deprecatedPipelineOwner;
        this._deprecatedRenderView = deprecatedRenderView;
        System.Diagnostics.Debug.Assert(((deprecatedRenderView is null) || (object.Equals(((global::Doroti.Generated.Framework.Rendering.RenderView)deprecatedRenderView).flutterView, view))));
    }

    public override RenderObjectElement createElement() => DartRuntimePrimitives.ConvertValue<RenderObjectElement>(new _RawViewElement__view(this));
    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)(this._deprecatedRenderView ?? new global::Doroti.Generated.Framework.Rendering.RenderView(view: this.view)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _RawViewElement__view : RenderTreeRootElement
{
    private bool __late__pipelineOwner_initialized;
    private global::Doroti.Generated.Framework.Rendering.PipelineOwner __late__pipelineOwner = default!;
    internal virtual global::Doroti.Generated.Framework.Rendering.PipelineOwner _pipelineOwner
    {
        get
        {
            if (!__late__pipelineOwner_initialized)
            {
                __late__pipelineOwner = new global::Doroti.Generated.Framework.Rendering.PipelineOwner(onSemanticsOwnerCreated: () => this._handleSemanticsOwnerCreated(), onSemanticsUpdate: (global::System.Action<SemanticsUpdate>)this._handleSemanticsUpdate, onSemanticsOwnerDisposed: () => this._handleSemanticsOwnerDisposed());
                __late__pipelineOwner_initialized = true;
            }
            return __late__pipelineOwner;
        }
    }
    internal virtual Element? _child { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Rendering.PipelineOwner? _parentPipelineOwner { get; set; } = default;

    internal _RawViewElement__view(RenderObjectWidget widget) : base(widget)
    {
    }

    internal virtual global::Doroti.Generated.Framework.Rendering.PipelineOwner _effectivePipelineOwner => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Rendering.PipelineOwner>(((((_RawViewInternal__view?)(object?)this.widget)!)._deprecatedPipelineOwner ?? this._pipelineOwner));
    internal virtual void _handleSemanticsOwnerCreated()
    {
        (((global::Doroti.Generated.Framework.Rendering.RenderView?)(object?)((global::Doroti.Generated.Framework.Rendering.PipelineOwner)this._effectivePipelineOwner).rootNode)!)?.scheduleInitialSemantics();
    }

    internal virtual void _handleSemanticsOwnerDisposed()
    {
        (((global::Doroti.Generated.Framework.Rendering.RenderView?)(object?)((global::Doroti.Generated.Framework.Rendering.PipelineOwner)this._effectivePipelineOwner).rootNode)!)?.clearSemantics();
    }

    internal virtual void _handleSemanticsUpdate(SemanticsUpdate update)
    {
        (((_RawViewInternal__view?)(object?)this.widget)!).view.updateSemantics(update);
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject renderObject => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Rendering.RenderObject>(((global::Doroti.Generated.Framework.Rendering.RenderView?)(object?)base.renderObject)!);
    internal virtual void _updateChild()
    {
        try
        {
            Widget child__20598 = (((_RawViewInternal__view?)(object?)this.widget)!).builder(this, this._effectivePipelineOwner);
            _child = updateChild(this._child, child__20598, null);
        }
        catch (Exception e__20737)
        {
            var stack__20740 = new System.Diagnostics.StackTrace();
            var details__20761 = new global::Doroti.Generated.Framework.Foundation.FlutterErrorDetails(exception: e__20737, stack: stack__20740, library: "widgets library", context: new global::Doroti.Generated.Framework.Foundation.ErrorDescription($"building {this}"), informationCollector: ((InformationCollector)(!global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kDebugMode ? null : (() => new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Rendering.DiagnosticsDebugCreator(new DebugCreator(this)) }))));
            FlutterError.reportError(details__20761);
            Widget error__21139 = ErrorWidget.builder(details__20761);
            _child = updateChild(((Element)(object)null), error__21139, this.slot);
        }
    }

    public override void mount(Element? parent, object? newSlot)
    {
        base.mount(parent, newSlot);
        DartRuntimePrimitives.Assert(() => (((global::Doroti.Generated.Framework.Rendering.PipelineOwner)this._effectivePipelineOwner).rootNode is null));
        this._effectivePipelineOwner.rootNode = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Rendering.RenderObject>(this.renderObject);
        _attachView();
        _updateChild();
        ((dynamic)this.renderObject).prepareInitialFrame();
        if ((((global::Doroti.Generated.Framework.Rendering.PipelineOwner)this._effectivePipelineOwner).semanticsOwner is not null))
        {
            this.renderObject.scheduleInitialSemantics();
        }
    }

    internal virtual void _attachView(global::Doroti.Generated.Framework.Rendering.PipelineOwner? parentPipelineOwner = null)
    {
        DartRuntimePrimitives.Assert(() => (this._parentPipelineOwner is null));
        parentPipelineOwner ??= View.pipelineOwnerOf(this);
        parentPipelineOwner.adoptChild(this._effectivePipelineOwner);
        global::Doroti.Generated.Framework.Rendering.RendererBinding.instance.addRenderView(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Rendering.RenderView>(this.renderObject));
        _parentPipelineOwner = parentPipelineOwner;
    }

    internal virtual void _detachView()
    {
        global::Doroti.Generated.Framework.Rendering.PipelineOwner? parentPipelineOwner__22095 = this._parentPipelineOwner;
        if ((parentPipelineOwner__22095 is not null))
        {
            global::Doroti.Generated.Framework.Rendering.RendererBinding.instance.removeRenderView(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Rendering.RenderView>(this.renderObject));
            parentPipelineOwner__22095.dropChild(this._effectivePipelineOwner);
            _parentPipelineOwner = null;
        }
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        if ((this._parentPipelineOwner is null))
        {
            return;
        }
        global::Doroti.Generated.Framework.Rendering.PipelineOwner newParentPipelineOwner__22513 = ((global::Doroti.Generated.Framework.Rendering.PipelineOwner)(object?)View.pipelineOwnerOf(this));
        if ((!object.Equals(newParentPipelineOwner__22513, this._parentPipelineOwner)))
        {
            _detachView();
            _attachView(newParentPipelineOwner__22513);
        }
    }

    public override void performRebuild()
    {
        base.performRebuild();
        _updateChild();
    }

    public override void activate()
    {
        base.activate();
        DartRuntimePrimitives.Assert(() => (((global::Doroti.Generated.Framework.Rendering.PipelineOwner)this._effectivePipelineOwner).rootNode is null));
        this._effectivePipelineOwner.rootNode = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Rendering.RenderObject>(this.renderObject);
        _attachView();
    }

    public override void deactivate()
    {
        _detachView();
        DartRuntimePrimitives.Assert(() => (object.Equals(((global::Doroti.Generated.Framework.Rendering.PipelineOwner)this._effectivePipelineOwner).rootNode, this.renderObject)));
        this._effectivePipelineOwner.rootNode = null;
        base.deactivate();
    }

    public override void update(Widget newWidget)
    {
        var __newWidget = (_RawViewInternal__view)(object)newWidget;
        base.update(__newWidget);
        _updateChild();
    }

    public override void visitChildren(global::System.Action<Element> visitor)
    {
        if ((this._child is not null))
        {
            visitor(this._child!);
        }
    }

    public override void forgetChild(Element child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child, this._child)));
        _child = null;
        base.forgetChild(child);
    }

    public override void insertRenderObjectChild(global::Doroti.Generated.Framework.Rendering.RenderObject child, object? slot)
    {
        var __child = (global::Doroti.Generated.Framework.Rendering.RenderBox)(object)child;
        DartRuntimePrimitives.Assert(() => (slot is null));
        DartRuntimePrimitives.Assert(() => ((bool)((dynamic)this.renderObject).debugValidateChild(__child)));
        ((dynamic)this.renderObject).child = __child;
    }

    public override void moveRenderObjectChild(global::Doroti.Generated.Framework.Rendering.RenderObject child, object? oldSlot, object? newSlot)
    {
        DartRuntimePrimitives.Assert(() => false);
    }

    public override void removeRenderObjectChild(global::Doroti.Generated.Framework.Rendering.RenderObject child, object? slot)
    {
        DartRuntimePrimitives.Assert(() => (slot is null));
        DartRuntimePrimitives.Assert(() => (object.Equals(((global::Doroti.Generated.Framework.Rendering.RenderBox?)((dynamic)this.renderObject).child), child)));
        ((dynamic)this.renderObject).child = null;
    }

    public override void unmount()
    {
        if ((!object.Equals(this._effectivePipelineOwner, (((_RawViewInternal__view?)(object?)this.widget)!)._deprecatedPipelineOwner)))
        {
            this._effectivePipelineOwner.dispose();
        }
        base.unmount();
    }

}

internal class _ViewScope__view : InheritedWidget
{
    public virtual DorotiView view { get; private set; } = default!;

    internal _ViewScope__view(DorotiView view, Widget child) : base(child: child)
    {
        this.view = view;
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>((!object.Equals(this.view, ((_ViewScope__view)oldWidget).view)));
}

internal class _PipelineOwnerScope__view : InheritedWidget
{
    public virtual global::Doroti.Generated.Framework.Rendering.PipelineOwner pipelineOwner { get; private set; } = default!;

    internal _PipelineOwnerScope__view(global::Doroti.Generated.Framework.Rendering.PipelineOwner pipelineOwner, Widget child) : base(child: child)
    {
        this.pipelineOwner = pipelineOwner;
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>((!object.Equals(this.pipelineOwner, ((_PipelineOwnerScope__view)oldWidget).pipelineOwner)));
}

public class _MultiChildComponentWidget__view : Widget
{
    internal virtual List<Widget> _views { get; private set; } = default!;
    internal virtual Widget? _child { get; private set; }

    internal _MultiChildComponentWidget__view(global::Doroti.Generated.Framework.Foundation.Key? key = null, List<Widget> views = default!, Widget? child = null) : base(key: key)
    {
        List<Widget> __views = views ?? new List<Widget>();
        this._views = __views;
        this._child = child;
    }

    public override Element createElement() => DartRuntimePrimitives.ConvertValue<Element>(new _MultiChildComponentElement__view(this));
}

public class ViewCollection : _MultiChildComponentWidget__view
{
    public ViewCollection(global::Doroti.Generated.Framework.Foundation.Key? key = null, List<Widget> views = default!) : base(key: key, views: views)
    {
    }

    public virtual List<Widget> views => this._views;
}

public class ViewAnchor : StatelessWidget
{
    public virtual Widget? view { get; private set; }
    public virtual Widget child { get; private set; } = default!;

    public ViewAnchor(global::Doroti.Generated.Framework.Foundation.Key? key = null, Widget? view = null, Widget child = default!) : base(key: key)
    {
        this.view = view;
        this.child = child;
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new _MultiChildComponentWidget__view(views: new List<Widget>(), child: this.child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _MultiChildComponentElement__view : Element
{
    internal virtual List<Element> _viewElements { get; set; } = new List<Element>();
    internal virtual HashSet<Element> _forgottenViewElements { get; private set; } = new HashSet<Element>();
    internal virtual Element? _childElement { get; set; } = default;
    internal static object _viewSlot = new object();

    internal _MultiChildComponentElement__view(Widget widget) : base(widget)
    {
    }

    internal virtual bool _debugAssertChildren()
    {
        var typedWidget__29063 = ((_MultiChildComponentWidget__view?)(object?)this.widget)!;
        DartRuntimePrimitives.Assert(() => (checked((long)(this._viewElements.Count)) == checked((long)(((_MultiChildComponentWidget__view)typedWidget__29063)._views.Count))));
        DartRuntimePrimitives.Assert(() => (((this._childElement is null)) == ((((_MultiChildComponentWidget__view)typedWidget__29063)._child is null))));
        DartRuntimePrimitives.Assert(() => !this._viewElements.Contains(this._childElement));
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void attachRenderObject(object? newSlot)
    {
        base.attachRenderObject(newSlot);
        DartRuntimePrimitives.Assert(() => _debugCheckMustAttachRenderObject(newSlot));
    }

    public override void mount(Element? parent, object? newSlot)
    {
        base.mount(parent, newSlot);
        DartRuntimePrimitives.Assert(() => _debugCheckMustAttachRenderObject(newSlot));
        DartRuntimePrimitives.Assert(() => !System.Linq.Enumerable.Any(this._viewElements));
        DartRuntimePrimitives.Assert(() => (this._childElement is null));
        rebuild();
        DartRuntimePrimitives.Assert(() => _debugAssertChildren());
    }

    public override void updateSlot(object? newSlot)
    {
        base.updateSlot(newSlot);
        DartRuntimePrimitives.Assert(() => _debugCheckMustAttachRenderObject(newSlot));
    }

    internal virtual bool _debugCheckMustAttachRenderObject(object? slot)
    {
        if ((!global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kDebugMode || ((((_MultiChildComponentWidget__view?)(object?)this.widget)!)._child is not null)))
        {
            return true;
        }
        var hasAncestorRenderObjectElement__30315 = false;
        var ancestorWantsRenderObject__30363 = true;
        visitAncestorElements(((global::System.Func<Element, bool>)((ancestor) => {
if (!ancestor.debugExpectsRenderObjectForSlot(slot))
{
    ancestorWantsRenderObject__30363 = false;
    return false;
}
if ((ancestor is RenderObjectElement))
{
    hasAncestorRenderObjectElement__30315 = true;
    return false;
}
return true;
throw new InvalidOperationException("Dart closure completed without a value.");
})));
        if ((hasAncestorRenderObjectElement__30315 && ancestorWantsRenderObject__30363))
        {
            FlutterError.reportError(new global::Doroti.Generated.Framework.Foundation.FlutterErrorDetails(exception: new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary($"The Element for {toStringShort()} cannot be inserted into slot \"{slot}\" of its ancestor. "), new global::Doroti.Generated.Framework.Foundation.ErrorDescription($"The ownership chain for the Element in question was:\n  {debugGetCreatorChain(10L)}"), new global::Doroti.Generated.Framework.Foundation.ErrorDescription("This Element allows the creation of multiple independent render trees, which cannot " + "be attached to an ancestor in an existing render tree. However, an ancestor RenderObject " + "is expecting that a child will be attached."), new global::Doroti.Generated.Framework.Foundation.ErrorHint($"Try moving the subtree that contains the {toStringShort()} widget into the " + "view property of a ViewAnchor widget or to the root of the widget tree, where " + "it is not expected to attach its RenderObject to its ancestor.") })));
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void update(Widget newWidget)
    {
        var __newWidget = (_MultiChildComponentWidget__view)(object)newWidget;
        DartRuntimePrimitives.Assert(() => (((((_MultiChildComponentWidget__view)__newWidget)._child is null)) == (((((_MultiChildComponentWidget__view?)(object?)this.widget)!)._child is null))));
        base.update(__newWidget);
        rebuild(force: true);
        DartRuntimePrimitives.Assert(() => _debugAssertChildren());
    }

    public override bool debugExpectsRenderObjectForSlot(object? slot) => DartRuntimePrimitives.ConvertValue<bool>((!object.Equals(slot, _viewSlot)));
    public override void performRebuild()
    {
        var typedWidget__32409 = ((_MultiChildComponentWidget__view?)(object?)this.widget)!;
        _childElement = updateChild(this._childElement, ((_MultiChildComponentWidget__view)typedWidget__32409)._child, this.slot);
        List<Widget> views__32560 = ((_MultiChildComponentWidget__view)typedWidget__32409)._views.ToList();
        _viewElements = updateChildren(this._viewElements, views__32560, forgottenChildren: this._forgottenViewElements, slots: new List<object>(System.Linq.Enumerable.Select(System.Linq.Enumerable.Range(0, checked((int)checked((long)(views__32560.Count)))), ((_) => _viewSlot))));
        this._forgottenViewElements.Clear();
        base.performRebuild();
        DartRuntimePrimitives.Assert(() => _debugAssertChildren());
    }

    public override void forgetChild(Element child)
    {
        if ((object.Equals(child, this._childElement)))
        {
            _childElement = null;
        }
        else
        {
            DartRuntimePrimitives.Assert(() => this._viewElements.Contains(child));
            DartRuntimePrimitives.Assert(() => !this._forgottenViewElements.Contains(child));
            this._forgottenViewElements.Add(child);
        }
        base.forgetChild(child);
    }

    public override void visitChildren(global::System.Action<Element> visitor)
    {
        if ((this._childElement is not null))
        {
            visitor(this._childElement!);
        }
        foreach (Element child__33370 in this._viewElements)
        {
            if (!this._forgottenViewElements.Contains(child__33370))
            {
                visitor(child__33370);
            }
        }
    }

    public virtual bool debugDoingBuild => false;
    public override Element? renderObjectAttachingChild => this._childElement;
    public override List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> debugDescribeChildren()
    {
        return new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode>();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _DeprecatedRawViewKey__view<T> : GlobalKey<T> where T : IState
{
    public virtual DorotiView view { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Rendering.PipelineOwner? owner { get; private set; }
    public virtual global::Doroti.Generated.Framework.Rendering.RenderView? renderView { get; private set; }

    internal _DeprecatedRawViewKey__view(DorotiView view, global::Doroti.Generated.Framework.Rendering.PipelineOwner? owner, global::Doroti.Generated.Framework.Rendering.RenderView? renderView)
    {
        this.view = view;
        this.owner = owner;
        this.renderView = renderView;
    }

    public override bool Equals(object? other)
    {
        var __other = other as _DeprecatedRawViewKey__view<T>;
        if (__other is null) return false;
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((((__other is _DeprecatedRawViewKey__view<T>) && DartRuntimePrimitives.Identical(((_DeprecatedRawViewKey__view<T>)((_DeprecatedRawViewKey__view<T>)__other)).view, this.view)) && DartRuntimePrimitives.Identical(((_DeprecatedRawViewKey__view<T>)((_DeprecatedRawViewKey__view<T>)__other)).owner, this.owner)) && DartRuntimePrimitives.Identical(((_DeprecatedRawViewKey__view<T>)((_DeprecatedRawViewKey__view<T>)__other)).renderView, this.renderView));
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.view, this.owner, this.renderView));
    public override string ToString() => $"[_DeprecatedRawViewKey {(global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this.view))}]";
}

