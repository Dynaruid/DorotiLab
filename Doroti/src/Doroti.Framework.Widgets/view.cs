// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/view.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public class View : StatefulWidget
{
    public virtual DorotiView view { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;
    internal virtual PipelineOwner? _deprecatedPipelineOwner { get; private set; }
    internal virtual RenderView? _deprecatedRenderView { get; private set; }

    public View(
        Key? key = null,
        DorotiView view = default!,
        PipelineOwner? deprecatedDoNotUseWillBeRemovedWithoutNoticePipelineOwner = null,
        RenderView? deprecatedDoNotUseWillBeRemovedWithoutNoticeRenderView = null,
        Widget child = default!
    )
        : base(key: key)
    {
        this.view = view;
        this.child = child;
        _deprecatedPipelineOwner = deprecatedDoNotUseWillBeRemovedWithoutNoticePipelineOwner;
        _deprecatedRenderView = deprecatedDoNotUseWillBeRemovedWithoutNoticeRenderView;
        System.Diagnostics.Debug.Assert(
            (deprecatedDoNotUseWillBeRemovedWithoutNoticePipelineOwner is null)
                == (deprecatedDoNotUseWillBeRemovedWithoutNoticeRenderView is null)
        );
        System.Diagnostics.Debug.Assert(
            (deprecatedDoNotUseWillBeRemovedWithoutNoticeRenderView is null)
                || Equals(deprecatedDoNotUseWillBeRemovedWithoutNoticeRenderView.flutterView, view)
        );
    }

    public static DorotiView? maybeOf(BuildContext context)
    {
        return LookupBoundary.dependOnInheritedWidgetOfExactType<_ViewScope__view>(context)?.view;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static DorotiView of(BuildContext context)
    {
        DorotiView? result = maybeOf(context);
        DartRuntimePrimitives.Assert(() =>
        {
            if (result is null)
            {
                bool hiddenByBoundary =
                    LookupBoundary.debugIsHidingAncestorWidgetOfExactType<_ViewScope__view>(
                        context
                    );
                var information = new List<DiagnosticsNode>
                {
                    new ErrorDescription("The context used was:\n" + $"  {context}"),
                    new ErrorHint(
                        "This usually means that the provided context is not associated with a View."
                    ),
                };
                throw DartRuntimePrimitives.AsException(new FlutterError(information));
            }
            return true;
            throw new InvalidOperationException("Callback completed without returning a value.");
        });
        return result!;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static PipelineOwner pipelineOwnerOf(BuildContext context)
    {
        return context
                .dependOnInheritedWidgetOfExactType<_PipelineOwnerScope__view>()
                ?.pipelineOwner
            ?? RendererBinding.instance.rootPipelineOwner;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _ViewState__view());
}

internal class _ViewState__view : State<View>, WidgetsBindingObserver
{
    internal virtual FocusScopeNode _scopeNode { get; private set; } =
        new FocusScopeNode(
            debugLabel: Foundation.ConstantsLibrary.kReleaseMode ? null : "View Scope"
        );
    internal virtual FocusTraversalPolicy _policy { get; private set; } =
        new ReadingOrderTraversalPolicy();
    internal virtual bool _viewHasFocus { get; set; } = false;

    public override void initState()
    {
        base.initState();
        WidgetsBinding.instance.addObserver(this);
        _scopeNode.addListener(_scopeFocusChangeListener);
    }

    public override void dispose()
    {
        WidgetsBinding.instance.removeObserver(this);
        _scopeNode.removeListener(_scopeFocusChangeListener);
        _scopeNode.dispose();
        base.dispose();
    }

    internal virtual void _scopeFocusChangeListener()
    {
        if ((_viewHasFocus == _scopeNode.hasFocus) || !_scopeNode.hasFocus)
        {
            return;
        }
        WidgetsBinding.instance.platformDispatcher.requestViewFocusChange(
            direction: ViewFocusDirection.forward,
            state: ViewFocusState.focused,
            viewId: checked((long)widget.view.viewId)
        );
    }

    public virtual void didChangeViewFocus(ViewFocusEvent @event)
    {
        _viewHasFocus = @event.state switch
        {
            var __constant10380 when Equals(__constant10380, ViewFocusState.focused) => checked(
                @event.viewId
            ) == checked((long)widget.view.viewId),
            var __constant10448 when Equals(__constant10448, ViewFocusState.unfocused) => false,
            _ => throw new InvalidOperationException(
                "Switch expression did not handle the supplied value."
            ),
        };
        if (checked(@event.viewId) != checked((long)widget.view.viewId))
        {
            return;
        }
        FocusNode nextFocus = default!;
        switch (@event.state)
        {
            case var __constant10619 when Equals(__constant10619, ViewFocusState.focused):
            {
                switch (@event.direction)
                {
                    case var __constant10693
                        when Equals(__constant10693, ViewFocusDirection.forward):
                    {
                        nextFocus =
                            _policy.findFirstFocus(_scopeNode, ignoreCurrentFocus: true)
                            ?? _scopeNode;
                        break;
                    }
                    case var __constant10836
                        when Equals(__constant10836, ViewFocusDirection.backward):
                    {
                        nextFocus = _policy.findLastFocus(_scopeNode, ignoreCurrentFocus: true);
                        break;
                    }
                    case var __constant10965
                        when Equals(__constant10965, ViewFocusDirection.undefined):
                    {
                        nextFocus = DartRuntimePrimitives.ConvertValue<FocusNode>(_scopeNode);
                        break;
                    }
                }
                nextFocus.requestFocus();
                break;
            }
            case var __constant11086 when Equals(__constant11086, ViewFocusState.unfocused):
            {
                FocusManager.instance.rootScope.requestScopeFocus();
                break;
            }
        }
    }

    public override Widget build(BuildContext context)
    {
        return new RawView(
            view: widget.view,
            deprecatedDoNotUseWillBeRemovedWithoutNoticePipelineOwner: widget._deprecatedPipelineOwner,
            deprecatedDoNotUseWillBeRemovedWithoutNoticeRenderView: widget._deprecatedRenderView,
            child: MediaQuery.fromView(
                view: widget.view,
                child: new FocusTraversalGroup(
                    policy: _policy,
                    parentNode: FocusManager.instance.rootScope,
                    child: FocusScope.CreateWithExternalFocusNode(
                        includeSemantics: false,
                        focusScopeNode: _scopeNode,
                        child: widget.child
                    )
                )
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class RawView : StatelessWidget
{
    public virtual DorotiView view { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;
    internal virtual PipelineOwner? _deprecatedPipelineOwner { get; private set; }
    internal virtual RenderView? _deprecatedRenderView { get; private set; }

    public RawView(
        Key? key = null,
        DorotiView view = default!,
        PipelineOwner? deprecatedDoNotUseWillBeRemovedWithoutNoticePipelineOwner = null,
        RenderView? deprecatedDoNotUseWillBeRemovedWithoutNoticeRenderView = null,
        Widget child = default!
    )
        : base(key: key)
    {
        this.view = view;
        this.child = child;
        _deprecatedPipelineOwner = deprecatedDoNotUseWillBeRemovedWithoutNoticePipelineOwner;
        _deprecatedRenderView = deprecatedDoNotUseWillBeRemovedWithoutNoticeRenderView;
        System.Diagnostics.Debug.Assert(
            (deprecatedDoNotUseWillBeRemovedWithoutNoticePipelineOwner is null)
                == (deprecatedDoNotUseWillBeRemovedWithoutNoticeRenderView is null)
        );
        System.Diagnostics.Debug.Assert(
            (deprecatedDoNotUseWillBeRemovedWithoutNoticeRenderView is null)
                || Equals(deprecatedDoNotUseWillBeRemovedWithoutNoticeRenderView.flutterView, view)
        );
    }

    public override Widget build(BuildContext context)
    {
        return new _RawViewInternal__view(
            view: view,
            deprecatedPipelineOwner: _deprecatedPipelineOwner,
            deprecatedRenderView: _deprecatedRenderView,
            builder: (context, owner) =>
            {
                return new _ViewScope__view(
                    view: view,
                    child: new _PipelineOwnerScope__view(pipelineOwner: owner, child: child)
                );
                throw new InvalidOperationException(
                    "Callback completed without returning a value."
                );
            }
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal delegate Widget _RawViewContentBuilder__view(BuildContext context, PipelineOwner owner);

public class _RawViewInternal__view : RenderObjectWidget
{
    public virtual DorotiView view { get; private set; } = default!;
    public virtual Func<BuildContext, PipelineOwner, Widget> builder { get; private set; } =
        default!;
    internal virtual PipelineOwner? _deprecatedPipelineOwner { get; private set; }
    internal virtual RenderView? _deprecatedRenderView { get; private set; }

    internal _RawViewInternal__view(
        DorotiView view,
        PipelineOwner? deprecatedPipelineOwner,
        RenderView? deprecatedRenderView,
        Func<BuildContext, PipelineOwner, Widget> builder
    )
        : base(
            key: new _DeprecatedRawViewKey__view<IState>(
                view,
                deprecatedPipelineOwner,
                deprecatedRenderView
            )
        )
    {
        this.view = view;
        this.builder = builder;
        _deprecatedPipelineOwner = deprecatedPipelineOwner;
        _deprecatedRenderView = deprecatedRenderView;
        System.Diagnostics.Debug.Assert(
            (deprecatedRenderView is null) || Equals(deprecatedRenderView.flutterView, view)
        );
    }

    public override RenderObjectElement createElement() =>
        DartRuntimePrimitives.ConvertValue<RenderObjectElement>(new _RawViewElement__view(this));

    public override RenderObject createRenderObject(BuildContext context)
    {
        return _deprecatedRenderView ?? new RenderView(view: view);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _RawViewElement__view : RenderTreeRootElement
{
    private bool __late__pipelineOwner_initialized;
    private PipelineOwner __late__pipelineOwner = default!;
    internal virtual PipelineOwner _pipelineOwner
    {
        get
        {
            if (!__late__pipelineOwner_initialized)
            {
                __late__pipelineOwner = new PipelineOwner(
                    onSemanticsOwnerCreated: () => _handleSemanticsOwnerCreated(),
                    onSemanticsUpdate: _handleSemanticsUpdate,
                    onSemanticsOwnerDisposed: () => _handleSemanticsOwnerDisposed()
                );
                __late__pipelineOwner_initialized = true;
            }
            return __late__pipelineOwner;
        }
    }
    internal virtual Element? _child { get; set; } = default;
    internal virtual PipelineOwner? _parentPipelineOwner { get; set; } = default;

    internal _RawViewElement__view(RenderObjectWidget widget)
        : base(widget) { }

    internal virtual PipelineOwner _effectivePipelineOwner =>
        DartRuntimePrimitives.ConvertValue<PipelineOwner>(
            ((_RawViewInternal__view?)widget)!._deprecatedPipelineOwner ?? _pipelineOwner
        );

    internal virtual void _handleSemanticsOwnerCreated()
    {
        ((RenderView?)_effectivePipelineOwner.rootNode)!?.scheduleInitialSemantics();
    }

    internal virtual void _handleSemanticsOwnerDisposed()
    {
        ((RenderView?)_effectivePipelineOwner.rootNode)!?.clearSemantics();
    }

    internal virtual void _handleSemanticsUpdate(SemanticsUpdate update)
    {
        ((_RawViewInternal__view?)widget)!.view.updateSemantics(update);
    }

    public override RenderView renderObject => (RenderView)base.renderObject;

    internal virtual void _updateChild()
    {
        try
        {
            Widget child = ((_RawViewInternal__view?)widget)!.builder(
                this,
                _effectivePipelineOwner
            );
            _child = updateChild(_child, child, null);
        }
        catch (Exception e)
        {
            var stackLocal = new System.Diagnostics.StackTrace();
            var details = new FlutterErrorDetails(
                exception: e,
                stack: stackLocal,
                library: "widgets library",
                context: new ErrorDescription($"building {this}"),
                informationCollector: !Foundation.ConstantsLibrary.kDebugMode
                    ? null
                    : (
                        () =>
                            new List<global::Doroti.Framework.Foundation.DiagnosticsNode>
                            {
                                new global::Doroti.Framework.Rendering.DiagnosticsDebugCreator(
                                    new DebugCreator(this)
                                ),
                            }
                    )
            );
            FlutterError.reportError(details);
            Widget error = ErrorWidget.builder(details);
            _child = updateChild(null, error, slot);
        }
    }

    public override void mount(Element? parent, object? newSlot)
    {
        base.mount(parent, newSlot);
        DartRuntimePrimitives.Assert(() => _effectivePipelineOwner.rootNode is null);
        _effectivePipelineOwner.rootNode = DartRuntimePrimitives.ConvertValue<RenderObject>(
            renderObject
        );
        _attachView();
        _updateChild();
        renderObject.prepareInitialFrame();
        if (_effectivePipelineOwner.semanticsOwner is not null)
        {
            renderObject.scheduleInitialSemantics();
        }
    }

    internal virtual void _attachView(PipelineOwner? parentPipelineOwner = null)
    {
        DartRuntimePrimitives.Assert(() => _parentPipelineOwner is null);
        parentPipelineOwner ??= View.pipelineOwnerOf(this);
        parentPipelineOwner.adoptChild(_effectivePipelineOwner);
        RendererBinding.instance.addRenderView(
            DartRuntimePrimitives.ConvertValue<RenderView>(renderObject)
        );
        _parentPipelineOwner = parentPipelineOwner;
    }

    internal virtual void _detachView()
    {
        PipelineOwner? parentPipelineOwner = _parentPipelineOwner;
        if (parentPipelineOwner is not null)
        {
            RendererBinding.instance.removeRenderView(
                DartRuntimePrimitives.ConvertValue<RenderView>(renderObject)
            );
            parentPipelineOwner.dropChild(_effectivePipelineOwner);
            _parentPipelineOwner = null;
        }
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        if (_parentPipelineOwner is null)
        {
            return;
        }
        PipelineOwner newParentPipelineOwner = View.pipelineOwnerOf(this);
        if (!Equals(newParentPipelineOwner, _parentPipelineOwner))
        {
            _detachView();
            _attachView(newParentPipelineOwner);
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
        DartRuntimePrimitives.Assert(() => _effectivePipelineOwner.rootNode is null);
        _effectivePipelineOwner.rootNode = DartRuntimePrimitives.ConvertValue<RenderObject>(
            renderObject
        );
        _attachView();
    }

    public override void deactivate()
    {
        _detachView();
        DartRuntimePrimitives.Assert(() => Equals(_effectivePipelineOwner.rootNode, renderObject));
        _effectivePipelineOwner.rootNode = null;
        base.deactivate();
    }

    public override void update(Widget newWidget)
    {
        var __newWidget = (_RawViewInternal__view)newWidget;
        base.update(__newWidget);
        _updateChild();
    }

    public override void visitChildren(Action<Element> visitor)
    {
        if (_child is not null)
        {
            visitor(_child!);
        }
    }

    public override void forgetChild(Element child)
    {
        DartRuntimePrimitives.Assert(() => Equals(child, _child));
        _child = null;
        base.forgetChild(child);
    }

    public override void insertRenderObjectChild(RenderObject child, object? slot)
    {
        var __child = (RenderBox)child;
        DartRuntimePrimitives.Assert(() => slot is null);
        DartRuntimePrimitives.Assert(() => renderObject.debugValidateChild(__child));
        renderObject.child = __child;
    }

    public override void moveRenderObjectChild(RenderObject child, object? oldSlot, object? newSlot)
    {
        DartRuntimePrimitives.Assert(() => false);
    }

    public override void removeRenderObjectChild(RenderObject child, object? slot)
    {
        DartRuntimePrimitives.Assert(() => slot is null);
        DartRuntimePrimitives.Assert(() => Equals(renderObject.child, child));
        renderObject.child = null;
    }

    public override void unmount()
    {
        if (
            !Equals(
                _effectivePipelineOwner,
                ((_RawViewInternal__view?)widget)!._deprecatedPipelineOwner
            )
        )
        {
            _effectivePipelineOwner.dispose();
        }
        base.unmount();
    }
}

internal class _ViewScope__view : InheritedWidget
{
    public virtual DorotiView view { get; private set; } = default!;

    internal _ViewScope__view(DorotiView view, Widget child)
        : base(child: child)
    {
        this.view = view;
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget) =>
        DartRuntimePrimitives.ConvertValue<bool>(!Equals(view, ((_ViewScope__view)oldWidget).view));
}

internal class _PipelineOwnerScope__view : InheritedWidget
{
    public virtual PipelineOwner pipelineOwner { get; private set; } = default!;

    internal _PipelineOwnerScope__view(PipelineOwner pipelineOwner, Widget child)
        : base(child: child)
    {
        this.pipelineOwner = pipelineOwner;
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget) =>
        DartRuntimePrimitives.ConvertValue<bool>(
            !Equals(pipelineOwner, ((_PipelineOwnerScope__view)oldWidget).pipelineOwner)
        );
}

public class _MultiChildComponentWidget__view : Widget
{
    internal virtual List<Widget> _views { get; private set; } = default!;
    internal virtual Widget? _child { get; private set; }

    internal _MultiChildComponentWidget__view(
        Key? key = null,
        List<Widget> views = default!,
        Widget? child = null
    )
        : base(key: key)
    {
        List<Widget> __views = views ?? new List<Widget>();
        _views = __views;
        _child = child;
    }

    public override Element createElement() =>
        DartRuntimePrimitives.ConvertValue<Element>(new _MultiChildComponentElement__view(this));
}

public class ViewCollection : _MultiChildComponentWidget__view
{
    public ViewCollection(Key? key = null, List<Widget> views = default!)
        : base(key: key, views: views) { }

    public virtual List<Widget> views => _views;
}

public class ViewAnchor : StatelessWidget
{
    public virtual Widget? view { get; private set; }
    public virtual Widget child { get; private set; } = default!;

    public ViewAnchor(Key? key = null, Widget? view = null, Widget child = default!)
        : base(key: key)
    {
        this.view = view;
        this.child = child;
    }

    public override Widget build(BuildContext context)
    {
        return new _MultiChildComponentWidget__view(views: new List<Widget>(), child: child);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _MultiChildComponentElement__view : Element
{
    internal virtual List<Element> _viewElements { get; set; } = new List<Element>();
    internal virtual HashSet<Element> _forgottenViewElements { get; private set; } =
        new HashSet<Element>();
    internal virtual Element? _childElement { get; set; } = default;
    internal static object _viewSlot = new object();

    internal _MultiChildComponentElement__view(Widget widget)
        : base(widget) { }

    internal virtual bool _debugAssertChildren()
    {
        var typedWidget = ((_MultiChildComponentWidget__view?)widget)!;
        DartRuntimePrimitives.Assert(() =>
            checked(_viewElements.Count) == checked((long)typedWidget._views.Count)
        );
        DartRuntimePrimitives.Assert(() => (_childElement is null) == (typedWidget._child is null));
        DartRuntimePrimitives.Assert(() =>
            _childElement is null || !_viewElements.Contains(_childElement)
        );
        return true;
        throw new InvalidOperationException("Control flow completed without returning a value.");
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
        DartRuntimePrimitives.Assert(() => !Enumerable.Any(_viewElements));
        DartRuntimePrimitives.Assert(() => _childElement is null);
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
        if (
            !Foundation.ConstantsLibrary.kDebugMode
            || (((_MultiChildComponentWidget__view?)widget)!._child is not null)
        )
        {
            return true;
        }
        var hasAncestorRenderObjectElement = false;
        var ancestorWantsRenderObject = true;
        visitAncestorElements(
            (ancestor) =>
            {
                if (!ancestor.debugExpectsRenderObjectForSlot(slot))
                {
                    ancestorWantsRenderObject = false;
                    return false;
                }
                if (ancestor is RenderObjectElement)
                {
                    hasAncestorRenderObjectElement = true;
                    return false;
                }
                return true;
                throw new InvalidOperationException(
                    "Callback completed without returning a value."
                );
            }
        );
        if (hasAncestorRenderObjectElement && ancestorWantsRenderObject)
        {
            FlutterError.reportError(
                new FlutterErrorDetails(
                    exception: new FlutterError(
                        new List<DiagnosticsNode>
                        {
                            new ErrorSummary(
                                $"The Element for {toStringShort()} cannot be inserted into slot \"{slot}\" of its ancestor. "
                            ),
                            new ErrorDescription(
                                $"The ownership chain for the Element in question was:\n  {debugGetCreatorChain(10L)}"
                            ),
                            new ErrorDescription(
                                "This Element allows the creation of multiple independent render trees, which cannot "
                                    + "be attached to an ancestor in an existing render tree. However, an ancestor RenderObject "
                                    + "is expecting that a child will be attached."
                            ),
                            new ErrorHint(
                                $"Try moving the subtree that contains the {toStringShort()} widget into the "
                                    + "view property of a ViewAnchor widget or to the root of the widget tree, where "
                                    + "it is not expected to attach its RenderObject to its ancestor."
                            ),
                        }
                    )
                )
            );
        }
        return true;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void update(Widget newWidget)
    {
        var __newWidget = (_MultiChildComponentWidget__view)newWidget;
        DartRuntimePrimitives.Assert(() =>
            (__newWidget._child is null)
            == (((_MultiChildComponentWidget__view?)widget)!._child is null)
        );
        base.update(__newWidget);
        rebuild(force: true);
        DartRuntimePrimitives.Assert(() => _debugAssertChildren());
    }

    public override bool debugExpectsRenderObjectForSlot(object? slot) =>
        DartRuntimePrimitives.ConvertValue<bool>(!Equals(slot, _viewSlot));

    public override void performRebuild()
    {
        var typedWidget = ((_MultiChildComponentWidget__view?)widget)!;
        _childElement = updateChild(_childElement, typedWidget._child, slot);
        List<Widget> views = typedWidget._views.ToList();
        _viewElements = updateChildren(
            _viewElements,
            views,
            forgottenChildren: _forgottenViewElements,
            slots: new List<object>(
                Enumerable.Select(
                    Enumerable.Range(0, checked((int)checked((long)views.Count))),
                    (_) => _viewSlot
                )
            )
        );
        _forgottenViewElements.Clear();
        base.performRebuild();
        DartRuntimePrimitives.Assert(() => _debugAssertChildren());
    }

    public override void forgetChild(Element child)
    {
        if (Equals(child, _childElement))
        {
            _childElement = null;
        }
        else
        {
            DartRuntimePrimitives.Assert(() => _viewElements.Contains(child));
            DartRuntimePrimitives.Assert(() => !_forgottenViewElements.Contains(child));
            _forgottenViewElements.Add(child);
        }
        base.forgetChild(child);
    }

    public override void visitChildren(Action<Element> visitor)
    {
        if (_childElement is not null)
        {
            visitor(_childElement!);
        }
        foreach (Element child in _viewElements)
        {
            if (!_forgottenViewElements.Contains(child))
            {
                visitor(child);
            }
        }
    }

    public override bool debugDoingBuild => false;
    public override Element? renderObjectAttachingChild => _childElement;

    public override List<DiagnosticsNode> debugDescribeChildren()
    {
        return new List<DiagnosticsNode>();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _DeprecatedRawViewKey__view<T> : GlobalKey<T>
    where T : IState
{
    public virtual DorotiView view { get; private set; } = default!;
    public virtual PipelineOwner? owner { get; private set; }
    public virtual RenderView? renderView { get; private set; }

    internal _DeprecatedRawViewKey__view(
        DorotiView view,
        PipelineOwner? owner,
        RenderView? renderView
    )
    {
        this.view = view;
        this.owner = owner;
        this.renderView = renderView;
    }

    public override bool Equals(object? other)
    {
        var __other = other as _DeprecatedRawViewKey__view<T>;
        if (__other is null)
        {
            return false;
        }

        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is _DeprecatedRawViewKey__view<T>)
            && DartRuntimePrimitives.Identical(__other.view, view)
            && DartRuntimePrimitives.Identical(__other.owner, owner)
            && DartRuntimePrimitives.Identical(__other.renderView, renderView);
    }

    public override int GetHashCode() =>
        DartRuntimePrimitives.ConvertValue<int>(
            FoundationRuntimePorts.ObjectHash(view, owner, renderView)
        );

    public override string ToString() =>
        $"[_DeprecatedRawViewKey {DiagnosticsLibrary.describeIdentity(view)}]";
}
