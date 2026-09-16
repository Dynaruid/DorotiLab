// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/indexed_stack.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public class IndexedStack : StatelessWidget
{
    public virtual AlignmentGeometry alignment { get; private set; } = default!;
    public virtual TextDirection? textDirection { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual StackFit sizing { get; private set; } = default!;
    public virtual long? index { get; private set; }
    public virtual List<Widget> children { get; private set; } = default!;

    public IndexedStack(Key? key = null, AlignmentGeometry alignment = default!, TextDirection? textDirection = null, Clip clipBehavior = Clip.hardEdge, StackFit sizing = StackFit.loose, long? index = 0, List<Widget> children = default!) : base(key: key)
    {
        AlignmentGeometry __alignment = alignment ?? AlignmentDirectional.topStart;
        List<Widget> __children = children ?? new List<Widget>();
        this.alignment = __alignment;
        this.textDirection = textDirection;
        this.clipBehavior = clipBehavior;
        this.sizing = sizing;
        this.index = index;
        this.children = __children;
    }

    public override Widget build(BuildContext context)
    {
        var wrappedChildren = new List<Widget>(Enumerable.Select(Enumerable.Range(0, checked((int)checked((long)children.Count))), (i) =>
        {
            var isSelected = i == index;
            return new _VisibilityScope__indexed_stack(isVisible: isSelected, child: new ExcludeFocus(excluding: !isSelected, child: children[i]));
            throw new InvalidOperationException("Dart closure completed without a value.");
        }));
        return new _RawIndexedStack__indexed_stack(alignment: alignment, textDirection: textDirection, clipBehavior: clipBehavior, sizing: sizing, index: index, children: wrappedChildren);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _RawIndexedStack__indexed_stack : Stack
{
    public virtual long? index { get; private set; }

    internal _RawIndexedStack__indexed_stack(AlignmentGeometry alignment = default!, TextDirection? textDirection = null, Clip clipBehavior = Clip.hardEdge, StackFit sizing = StackFit.loose, long? index = 0, List<Widget> children = default!) : base(alignment: alignment ?? AlignmentDirectional.topStart, textDirection: textDirection, clipBehavior: clipBehavior, children: children ?? new List<Widget>(), fit: sizing)
    {
        this.index = index;
        System.Diagnostics.Debug.Assert((index is null) || (DartRuntimePrimitives.RequireValue(index) == 0L) && (checked(this.children.Count) == 0L) || (index >= 0L) && (DartRuntimePrimitives.RequireValue(index) < checked(this.children.Count)));
    }

    // Dart library-private member: distinct from the same name in the base library.
    internal new virtual bool _debugCheckHasDirectionality(BuildContext context)
    {
        if ((alignment is AlignmentDirectional) && (textDirection is null))
        {
            AlignmentDirectional alignment__as4557 = (AlignmentDirectional)alignment;
            DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasDirectionality(context, why: "to resolve the 'alignment' argument", hint: Equals(alignment, AlignmentDirectional.topStart) ? "The default value for 'alignment' is AlignmentDirectional.topStart, which requires a text direction." : null, alternative: $"Instead of providing a Directionality widget, another solution would be passing a non-directional 'alignment__as4557', or an explicit 'textDirection', to the {GetType()}."));
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => _debugCheckHasDirectionality(context));
        return new RenderIndexedStack(index: index, fit: fit, clipBehavior: clipBehavior, alignment: alignment, textDirection: textDirection ?? Directionality.maybeOf(context));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (RenderIndexedStack)renderObject;
        DartRuntimePrimitives.Assert(() => _debugCheckHasDirectionality(context));
        DartRuntimePrimitives.Ignore(((Func<RenderIndexedStack>)(() =>
{
    var __cascade = __renderObject;
    __cascade.index = index;
    __cascade.fit = fit;
    __cascade.clipBehavior = clipBehavior;
    __cascade.alignment = alignment;
    __cascade.textDirection = textDirection ?? Directionality.maybeOf(context);
    return __cascade;
}))());
    }

    public override MultiChildRenderObjectElement createElement()
    {
        return new _IndexedStackElement__indexed_stack(this);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _IndexedStackElement__indexed_stack : MultiChildRenderObjectElement
{
    internal _IndexedStackElement__indexed_stack(_RawIndexedStack__indexed_stack widget) : base(widget)
    {
    }

    public override _RawIndexedStack__indexed_stack widget => ((_RawIndexedStack__indexed_stack?)base.widget)!;
    public override void debugVisitOnstageChildren(System.Action<Element> visitor)
    {
        long? indexLocal = widget.index;
        if ((indexLocal is not null) && Enumerable.Any(children))
        {
            long index__6279__value6418 = DartRuntimePrimitives.RequireValue(indexLocal);
            visitor(children.elementAt(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(index__6279__value6418))));
        }
    }

}

public class Visibility : StatelessWidget
{
    public virtual Widget child { get; private set; } = default!;
    public virtual Widget replacement { get; private set; } = default!;
    public virtual bool visible { get; private set; } = default!;
    public virtual bool maintainState { get; private set; } = default!;
    public virtual bool maintainAnimation { get; private set; } = default!;
    public virtual bool maintainSize { get; private set; } = default!;
    public virtual bool maintainSemantics { get; private set; } = default!;
    public virtual bool maintainInteractivity { get; private set; } = default!;
    public virtual bool maintainFocusability { get; private set; } = default!;

    public Visibility(Key? key = null, Widget child = default!, Widget replacement = default!, bool visible = true, bool maintainState = false, bool maintainAnimation = false, bool maintainSize = false, bool maintainSemantics = false, bool maintainInteractivity = false, bool maintainFocusability = false) : base(key: key)
    {
        Widget __replacement = replacement ?? SizedBox.CreateShrink();
        this.child = child;
        this.replacement = __replacement;
        this.visible = visible;
        this.maintainState = maintainState;
        this.maintainAnimation = maintainAnimation;
        this.maintainSize = maintainSize;
        this.maintainSemantics = maintainSemantics;
        this.maintainInteractivity = maintainInteractivity;
        this.maintainFocusability = maintainFocusability;
        System.Diagnostics.Debug.Assert(maintainState || !maintainAnimation);
        System.Diagnostics.Debug.Assert(maintainAnimation || !maintainSize);
        System.Diagnostics.Debug.Assert(maintainSize || !maintainSemantics);
        System.Diagnostics.Debug.Assert(maintainSize || !maintainInteractivity);
        System.Diagnostics.Debug.Assert(maintainState || !maintainFocusability);
    }

    public static Visibility CreateMaintain(Key? key = null, Widget child = default!, bool visible = true)
    {
        var __instance = new Visibility(key, child, default!, visible, default!, default!, default!, default!, default!, default!);
        __instance.child = child;
        __instance.visible = visible;
        __instance.maintainState = true;
        __instance.maintainAnimation = true;
        __instance.maintainSize = true;
        __instance.maintainSemantics = true;
        __instance.maintainInteractivity = true;
        __instance.maintainFocusability = true;
        __instance.replacement = SizedBox.CreateShrink();
        return __instance;
    }

    public static bool of(BuildContext context)
    {
        var isVisibleLocal = true;
        var ancestorContext = context;
        InheritedElement? ancestor = ancestorContext.getElementForInheritedWidgetOfExactType<_VisibilityScope__indexed_stack>();
        while (isVisibleLocal && (ancestor is not null))
        {
            var scope = ((_VisibilityScope__indexed_stack?)context.dependOnInheritedElement(ancestor))!;
            isVisibleLocal = scope.isVisible;
            ancestor.visitAncestorElements((parent) =>
            {
                ancestorContext = DartRuntimePrimitives.ConvertValue<BuildContext>(parent);
                return false;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
            ancestor = ancestorContext.getElementForInheritedWidgetOfExactType<_VisibilityScope__indexed_stack>();
        }
        return isVisibleLocal;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        Widget result = new ExcludeFocus(excluding: !visible && !maintainFocusability, child: child);
        if (maintainSize)
        {
            result = DartRuntimePrimitives.ConvertValue<Widget>(new _Visibility__indexed_stack(visible: visible, maintainSemantics: maintainSemantics, child: new IgnorePointer(ignoring: !visible && !maintainInteractivity, child: result)));
        }
        else
        {
            DartRuntimePrimitives.Assert(() => !maintainInteractivity);
            DartRuntimePrimitives.Assert(() => !maintainSemantics);
            DartRuntimePrimitives.Assert(() => !maintainSize);
            if (maintainState)
            {
                if (!maintainAnimation)
                {
                    result = DartRuntimePrimitives.ConvertValue<Widget>(new TickerMode(enabled: visible, child: result));
                }
                result = DartRuntimePrimitives.ConvertValue<Widget>(new Offstage(offstage: !visible, child: result));
            }
            else
            {
                DartRuntimePrimitives.Assert(() => !maintainAnimation);
                DartRuntimePrimitives.Assert(() => !maintainState);
                result = visible ? child : replacement;
            }
        }
        return new _VisibilityScope__indexed_stack(isVisible: visible, child: result);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new FlagProperty("visible", value: visible, ifFalse: "hidden", ifTrue: "visible"));
        properties.add(new FlagProperty("maintainState", value: maintainState, ifFalse: "maintainState"));
        properties.add(new FlagProperty("maintainAnimation", value: maintainAnimation, ifFalse: "maintainAnimation"));
        properties.add(new FlagProperty("maintainSize", value: maintainSize, ifFalse: "maintainSize"));
        properties.add(new FlagProperty("maintainSemantics", value: maintainSemantics, ifFalse: "maintainSemantics"));
        properties.add(new FlagProperty("maintainInteractivity", value: maintainInteractivity, ifFalse: "maintainInteractivity"));
    }

}

internal class _VisibilityScope__indexed_stack : InheritedWidget
{
    public virtual bool isVisible { get; private set; } = default!;

    internal _VisibilityScope__indexed_stack(bool isVisible, Widget child) : base(child: child)
    {
        this.isVisible = isVisible;
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget)
    {
        var __old = (_VisibilityScope__indexed_stack)oldWidget;
        return isVisible != __old.isVisible;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class SliverVisibility : StatelessWidget
{
    public virtual Widget sliver { get; private set; } = default!;
    public virtual Widget replacementSliver { get; private set; } = default!;
    public virtual bool visible { get; private set; } = default!;
    public virtual bool maintainState { get; private set; } = default!;
    public virtual bool maintainAnimation { get; private set; } = default!;
    public virtual bool maintainSize { get; private set; } = default!;
    public virtual bool maintainSemantics { get; private set; } = default!;
    public virtual bool maintainInteractivity { get; private set; } = default!;

    public SliverVisibility(Key? key = null, Widget sliver = default!, Widget replacementSliver = default!, bool visible = true, bool maintainState = false, bool maintainAnimation = false, bool maintainSize = false, bool maintainSemantics = false, bool maintainInteractivity = false) : base(key: key)
    {
        Widget __replacementSliver = replacementSliver ?? new SliverToBoxAdapter();
        this.sliver = sliver;
        this.replacementSliver = __replacementSliver;
        this.visible = visible;
        this.maintainState = maintainState;
        this.maintainAnimation = maintainAnimation;
        this.maintainSize = maintainSize;
        this.maintainSemantics = maintainSemantics;
        this.maintainInteractivity = maintainInteractivity;
        System.Diagnostics.Debug.Assert(maintainState || !maintainAnimation);
        System.Diagnostics.Debug.Assert(maintainAnimation || !maintainSize);
        System.Diagnostics.Debug.Assert(maintainSize || !maintainSemantics);
        System.Diagnostics.Debug.Assert(maintainSize || !maintainInteractivity);
    }

    public static SliverVisibility CreateMaintain(Key? key = null, Widget sliver = default!, Widget replacementSliver = default!, bool visible = true)
    {
        var __instance = new SliverVisibility(key, sliver, replacementSliver, visible, default!, default!, default!, default!, default!);
        Widget __replacementSliver = replacementSliver ?? new SliverToBoxAdapter();
        __instance.sliver = sliver;
        __instance.replacementSliver = __replacementSliver;
        __instance.visible = visible;
        __instance.maintainState = true;
        __instance.maintainAnimation = true;
        __instance.maintainSize = true;
        __instance.maintainSemantics = true;
        __instance.maintainInteractivity = true;
        return __instance;
    }

    public override Widget build(BuildContext context)
    {
        if (maintainSize)
        {
            Widget result = sliver;
            result = DartRuntimePrimitives.ConvertValue<Widget>(new SliverIgnorePointer(ignoring: !visible && !maintainInteractivity, sliver: result));
            return new _SliverVisibility__indexed_stack(visible: visible, maintainSemantics: maintainSemantics, sliver: result);
        }
        DartRuntimePrimitives.Assert(() => !maintainInteractivity);
        DartRuntimePrimitives.Assert(() => !maintainSemantics);
        DartRuntimePrimitives.Assert(() => !maintainSize);
        if (maintainState)
        {
            Widget resultLocal = sliver;
            if (!maintainAnimation)
            {
                resultLocal = DartRuntimePrimitives.ConvertValue<Widget>(new TickerMode(enabled: visible, child: sliver));
            }
            return new SliverOffstage(sliver: resultLocal, offstage: !visible);
        }
        DartRuntimePrimitives.Assert(() => !maintainAnimation);
        DartRuntimePrimitives.Assert(() => !maintainState);
        return visible ? sliver : replacementSliver;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new FlagProperty("visible", value: visible, ifFalse: "hidden", ifTrue: "visible"));
        properties.add(new FlagProperty("maintainState", value: maintainState, ifFalse: "maintainState"));
        properties.add(new FlagProperty("maintainAnimation", value: maintainAnimation, ifFalse: "maintainAnimation"));
        properties.add(new FlagProperty("maintainSize", value: maintainSize, ifFalse: "maintainSize"));
        properties.add(new FlagProperty("maintainSemantics", value: maintainSemantics, ifFalse: "maintainSemantics"));
        properties.add(new FlagProperty("maintainInteractivity", value: maintainInteractivity, ifFalse: "maintainInteractivity"));
    }

}

internal class _Visibility__indexed_stack : SingleChildRenderObjectWidget
{
    public virtual bool visible { get; private set; } = default!;
    public virtual bool maintainSemantics { get; private set; } = default!;

    internal _Visibility__indexed_stack(bool visible, bool maintainSemantics, Widget? child = null) : base(child: child)
    {
        this.visible = visible;
        this.maintainSemantics = maintainSemantics;
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new _RenderVisibility__indexed_stack(visible, maintainSemantics);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (_RenderVisibility__indexed_stack)renderObject;
        DartRuntimePrimitives.Ignore(((Func<_RenderVisibility__indexed_stack>)(() =>
{
    var __cascade = __renderObject;
    __cascade.visible = visible;
    __cascade.maintainSemantics = maintainSemantics;
    return __cascade;
}))());
    }

}

public class _RenderVisibility__indexed_stack : RenderProxyBox
{
    internal virtual bool _visible { get; set; } = default!;
    internal virtual bool _maintainSemantics { get; set; } = default!;

    internal _RenderVisibility__indexed_stack(bool _visible, bool _maintainSemantics)
    {
        this._visible = _visible;
        this._maintainSemantics = _maintainSemantics;
    }

    public virtual bool visible
    {
        get => _visible;
        set
        {
            var __value = value;
            if (__value == visible)
            {
                return;
            }
            _visible = __value;
            markNeedsPaint();
        }
    }
    public virtual bool maintainSemantics
    {
        get => _maintainSemantics;
        set
        {
            var __value = value;
            if (__value == maintainSemantics)
            {
                return;
            }
            _maintainSemantics = __value;
            markNeedsSemanticsUpdate();
        }
    }
    public override void visitChildrenForSemantics(System.Action<RenderObject> visitor)
    {
        if (maintainSemantics || visible)
        {
            base.visitChildrenForSemantics(visitor);
        }
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        if (!visible)
        {
            return;
        }
        base.paint(context, offset);
    }

}

internal class _SliverVisibility__indexed_stack : SingleChildRenderObjectWidget
{
    public virtual bool visible { get; private set; } = default!;
    public virtual bool maintainSemantics { get; private set; } = default!;

    internal _SliverVisibility__indexed_stack(bool visible, bool maintainSemantics, Widget? sliver = null) : base(child: sliver)
    {
        this.visible = visible;
        this.maintainSemantics = maintainSemantics;
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new _RenderSliverVisibility__indexed_stack(visible, maintainSemantics);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (_RenderSliverVisibility__indexed_stack)renderObject;
        DartRuntimePrimitives.Ignore(((Func<_RenderSliverVisibility__indexed_stack>)(() =>
{
    var __cascade = __renderObject;
    __cascade.visible = visible;
    __cascade.maintainSemantics = maintainSemantics;
    return __cascade;
}))());
    }

}

public class _RenderSliverVisibility__indexed_stack : RenderProxySliver
{
    internal virtual bool _visible { get; set; } = default!;
    internal virtual bool _maintainSemantics { get; set; } = default!;

    internal _RenderSliverVisibility__indexed_stack(bool _visible, bool _maintainSemantics)
    {
        this._visible = _visible;
        this._maintainSemantics = _maintainSemantics;
    }

    public virtual bool visible
    {
        get => _visible;
        set
        {
            var __value = value;
            if (__value == visible)
            {
                return;
            }
            _visible = __value;
            markNeedsPaint();
        }
    }
    public virtual bool maintainSemantics
    {
        get => _maintainSemantics;
        set
        {
            var __value = value;
            if (__value == maintainSemantics)
            {
                return;
            }
            _maintainSemantics = __value;
            markNeedsSemanticsUpdate();
        }
    }
    public override void visitChildrenForSemantics(System.Action<RenderObject> visitor)
    {
        if (maintainSemantics || visible)
        {
            base.visitChildrenForSemantics(visitor);
        }
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        if (!visible)
        {
            return;
        }
        base.paint(context, offset);
    }

}
