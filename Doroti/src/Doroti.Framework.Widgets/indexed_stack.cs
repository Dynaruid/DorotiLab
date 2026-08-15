// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../flutter-master/packages/flutter/lib/src/widgets/indexed_stack.dart
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

public class IndexedStack : StatelessWidget
{
    public virtual global::Doroti.Generated.Framework.Painting.AlignmentGeometry alignment { get; private set; } = default!;
    public virtual TextDirection? textDirection { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Rendering.StackFit sizing { get; private set; } = default!;
    public virtual long? index { get; private set; }
    public virtual List<Widget> children { get; private set; } = default!;

    public IndexedStack(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Painting.AlignmentGeometry alignment = default!, TextDirection? textDirection = null, Clip clipBehavior = Clip.hardEdge, global::Doroti.Generated.Framework.Rendering.StackFit sizing = global::Doroti.Generated.Framework.Rendering.StackFit.loose, long? index = 0, List<Widget> children = default!) : base(key: key)
    {
        global::Doroti.Generated.Framework.Painting.AlignmentGeometry __alignment = alignment ?? global::Doroti.Generated.Framework.Painting.AlignmentDirectional.topStart;
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
        var wrappedChildren__3375 = new List<Widget>(System.Linq.Enumerable.Select(System.Linq.Enumerable.Range(0, checked((int)checked((long)(this.children.Count)))), ((i) => {
var isSelected__3454 = (i == this.index);
return new _VisibilityScope__indexed_stack(isVisible: isSelected__3454, child: new ExcludeFocus(excluding: !isSelected__3454, child: this.children[(int)(i)]));
throw new InvalidOperationException("Dart closure completed without a value.");
})));
        return ((Widget)(object?)new _RawIndexedStack__indexed_stack(alignment: this.alignment, textDirection: this.textDirection, clipBehavior: this.clipBehavior, sizing: this.sizing, index: this.index, children: wrappedChildren__3375));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _RawIndexedStack__indexed_stack : Stack
{
    public virtual long? index { get; private set; }

    internal _RawIndexedStack__indexed_stack(global::Doroti.Generated.Framework.Painting.AlignmentGeometry alignment = default!, TextDirection? textDirection = null, Clip clipBehavior = Clip.hardEdge, global::Doroti.Generated.Framework.Rendering.StackFit sizing = global::Doroti.Generated.Framework.Rendering.StackFit.loose, long? index = 0, List<Widget> children = default!) : base(alignment: alignment ?? global::Doroti.Generated.Framework.Painting.AlignmentDirectional.topStart, textDirection: textDirection, clipBehavior: clipBehavior, children: children ?? new List<Widget>(), fit: sizing)
    {
        this.index = index;
        System.Diagnostics.Debug.Assert((((index is null) || (((DartRuntimePrimitives.RequireValue(index) == 0L) && (checked((long)(children.Count)) == 0L)))) || (((index >= 0L) && (DartRuntimePrimitives.RequireValue(index) < checked((long)(children.Count)))))));
    }

    internal virtual bool _debugCheckHasDirectionality(BuildContext context)
    {
        if (((this.alignment is global::Doroti.Generated.Framework.Painting.AlignmentDirectional) && (this.textDirection is null)))
        {
            global::Doroti.Generated.Framework.Painting.AlignmentDirectional alignment__as4557 = (global::Doroti.Generated.Framework.Painting.AlignmentDirectional)alignment;
            DartRuntimePrimitives.Assert(() => global::Doroti.Generated.Framework.Widgets.DebugLibrary.debugCheckHasDirectionality(context, why: "to resolve the 'alignment' argument", hint: ((object.Equals(this.alignment, global::Doroti.Generated.Framework.Painting.AlignmentDirectional.topStart)) ? "The default value for 'alignment' is AlignmentDirectional.topStart, which requires a text direction." : null), alternative: $"Instead of providing a Directionality widget, another solution would be passing a non-directional 'alignment__as4557', or an explicit 'textDirection', to the {this.GetType()}."));
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => _debugCheckHasDirectionality(context));
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new global::Doroti.Generated.Framework.Rendering.RenderIndexedStack(index: this.index, fit: this.fit, clipBehavior: this.clipBehavior, alignment: this.alignment, textDirection: ((this.textDirection ?? (TextDirection)Directionality.maybeOf(context)))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Generated.Framework.Rendering.RenderIndexedStack)(object)renderObject;
        DartRuntimePrimitives.Assert(() => _debugCheckHasDirectionality(context));
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Generated.Framework.Rendering.RenderIndexedStack>)(() =>
{            var __cascade = __renderObject;
            __cascade.index = this.index;
            __cascade.fit = this.fit;
            __cascade.clipBehavior = this.clipBehavior;
            __cascade.alignment = this.alignment;
            __cascade.textDirection = ((this.textDirection ?? (TextDirection)Directionality.maybeOf(context)));
            return __cascade;        }))());
    }

    public override MultiChildRenderObjectElement createElement()
    {
        return ((MultiChildRenderObjectElement)(object?)new _IndexedStackElement__indexed_stack(this));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _IndexedStackElement__indexed_stack : MultiChildRenderObjectElement
{
    internal _IndexedStackElement__indexed_stack(_RawIndexedStack__indexed_stack widget) : base(widget)
    {
    }

    public override _RawIndexedStack__indexed_stack widget => ((_RawIndexedStack__indexed_stack?)(object?)base.widget)!;
    public override void debugVisitOnstageChildren(global::System.Action<Element> visitor)
    {
        long? index__6279 = ((_RawIndexedStack__indexed_stack)this.widget).index;
        if (((index__6279 is not null) && System.Linq.Enumerable.Any(this.children)))
        {
            long index__6279__value6418 = DartRuntimePrimitives.RequireValue(index__6279);
            visitor(this.children.elementAt(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(index__6279__value6418))));
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

    public Visibility(global::Doroti.Generated.Framework.Foundation.Key? key = null, Widget child = default!, Widget replacement = default!, bool visible = true, bool maintainState = false, bool maintainAnimation = false, bool maintainSize = false, bool maintainSemantics = false, bool maintainInteractivity = false, bool maintainFocusability = false) : base(key: key)
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
        System.Diagnostics.Debug.Assert((maintainState || !maintainAnimation));
        System.Diagnostics.Debug.Assert((maintainAnimation || !maintainSize));
        System.Diagnostics.Debug.Assert((maintainSize || !maintainSemantics));
        System.Diagnostics.Debug.Assert((maintainSize || !maintainInteractivity));
        System.Diagnostics.Debug.Assert((maintainState || !maintainFocusability));
    }

    public static Visibility CreateMaintain(global::Doroti.Generated.Framework.Foundation.Key? key = null, Widget child = default!, bool visible = true)
    {
        var __instance = new Visibility(default!, default!, default!, default!, default!, default!, default!, default!, default!, default!);
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
        var isVisible__16785 = true;
        var ancestorContext__16811 = context;
        InheritedElement? ancestor__16860 = ((InheritedElement?)(object?)ancestorContext__16811.getElementForInheritedWidgetOfExactType<_VisibilityScope__indexed_stack>());
        while ((isVisible__16785 && (ancestor__16860 is not null)))
        {
            var scope__17013 = ((_VisibilityScope__indexed_stack?)(object?)context.dependOnInheritedElement(ancestor__16860))!;
            isVisible__16785 = ((_VisibilityScope__indexed_stack)scope__17013).isVisible;
            ancestor__16860.visitAncestorElements(((global::System.Func<Element, bool>)((parent) => {
ancestorContext__16811 = DartRuntimePrimitives.ConvertValue<BuildContext>(parent);
return false;
throw new InvalidOperationException("Dart closure completed without a value.");
})));
            ancestor__16860 = ancestorContext__16811.getElementForInheritedWidgetOfExactType<_VisibilityScope__indexed_stack>();
        }
        return isVisible__16785;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        Widget result__17431 = ((Widget)(object?)new ExcludeFocus(excluding: (!this.visible && !this.maintainFocusability), child: this.child));
        if (this.maintainSize)
        {
            result__17431 = DartRuntimePrimitives.ConvertValue<Widget>(new _Visibility__indexed_stack(visible: this.visible, maintainSemantics: this.maintainSemantics, child: new IgnorePointer(ignoring: (!this.visible && !this.maintainInteractivity), child: result__17431)));
        }
        else
        {
            DartRuntimePrimitives.Assert(() => !this.maintainInteractivity);
            DartRuntimePrimitives.Assert(() => !this.maintainSemantics);
            DartRuntimePrimitives.Assert(() => !this.maintainSize);
            if (this.maintainState)
            {
                if (!this.maintainAnimation)
                {
                    result__17431 = DartRuntimePrimitives.ConvertValue<Widget>(new TickerMode(enabled: this.visible, child: result__17431));
                }
                result__17431 = DartRuntimePrimitives.ConvertValue<Widget>(new Offstage(offstage: !this.visible, child: result__17431));
            }
            else
            {
                DartRuntimePrimitives.Assert(() => !this.maintainAnimation);
                DartRuntimePrimitives.Assert(() => !this.maintainState);
                result__17431 = (this.visible ? this.child : this.replacement);
            }
        }
        return ((Widget)(object?)new _VisibilityScope__indexed_stack(isVisible: this.visible, child: result__17431));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.FlagProperty("visible", value: this.visible, ifFalse: "hidden", ifTrue: "visible"));
        properties.add(new global::Doroti.Generated.Framework.Foundation.FlagProperty("maintainState", value: this.maintainState, ifFalse: "maintainState"));
        properties.add(new global::Doroti.Generated.Framework.Foundation.FlagProperty("maintainAnimation", value: this.maintainAnimation, ifFalse: "maintainAnimation"));
        properties.add(new global::Doroti.Generated.Framework.Foundation.FlagProperty("maintainSize", value: this.maintainSize, ifFalse: "maintainSize"));
        properties.add(new global::Doroti.Generated.Framework.Foundation.FlagProperty("maintainSemantics", value: this.maintainSemantics, ifFalse: "maintainSemantics"));
        properties.add(new global::Doroti.Generated.Framework.Foundation.FlagProperty("maintainInteractivity", value: this.maintainInteractivity, ifFalse: "maintainInteractivity"));
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
        var __old = (_VisibilityScope__indexed_stack)(object)oldWidget;
        return (this.isVisible != ((_VisibilityScope__indexed_stack)__old).isVisible);
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

    public SliverVisibility(global::Doroti.Generated.Framework.Foundation.Key? key = null, Widget sliver = default!, Widget replacementSliver = default!, bool visible = true, bool maintainState = false, bool maintainAnimation = false, bool maintainSize = false, bool maintainSemantics = false, bool maintainInteractivity = false) : base(key: key)
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
        System.Diagnostics.Debug.Assert((maintainState || !maintainAnimation));
        System.Diagnostics.Debug.Assert((maintainAnimation || !maintainSize));
        System.Diagnostics.Debug.Assert((maintainSize || !maintainSemantics));
        System.Diagnostics.Debug.Assert((maintainSize || !maintainInteractivity));
    }

    public static SliverVisibility CreateMaintain(global::Doroti.Generated.Framework.Foundation.Key? key = null, Widget sliver = default!, Widget replacementSliver = default!, bool visible = true)
    {
        var __instance = new SliverVisibility(default!, default!, default!, default!, default!, default!, default!, default!, default!);
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
        if (this.maintainSize)
        {
            Widget result__28108 = this.sliver;
            result__28108 = DartRuntimePrimitives.ConvertValue<Widget>(new SliverIgnorePointer(ignoring: (!this.visible && !this.maintainInteractivity), sliver: result__28108));
            return ((Widget)(object?)new _SliverVisibility__indexed_stack(visible: this.visible, maintainSemantics: this.maintainSemantics, sliver: result__28108));
        }
        DartRuntimePrimitives.Assert(() => !this.maintainInteractivity);
        DartRuntimePrimitives.Assert(() => !this.maintainSemantics);
        DartRuntimePrimitives.Assert(() => !this.maintainSize);
        if (this.maintainState)
        {
            Widget result__28499 = this.sliver;
            if (!this.maintainAnimation)
            {
                result__28499 = DartRuntimePrimitives.ConvertValue<Widget>(new TickerMode(enabled: this.visible, child: this.sliver));
            }
            return ((Widget)(object?)new SliverOffstage(sliver: result__28499, offstage: !this.visible));
        }
        DartRuntimePrimitives.Assert(() => !this.maintainAnimation);
        DartRuntimePrimitives.Assert(() => !this.maintainState);
        return (this.visible ? this.sliver : this.replacementSliver);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.FlagProperty("visible", value: this.visible, ifFalse: "hidden", ifTrue: "visible"));
        properties.add(new global::Doroti.Generated.Framework.Foundation.FlagProperty("maintainState", value: this.maintainState, ifFalse: "maintainState"));
        properties.add(new global::Doroti.Generated.Framework.Foundation.FlagProperty("maintainAnimation", value: this.maintainAnimation, ifFalse: "maintainAnimation"));
        properties.add(new global::Doroti.Generated.Framework.Foundation.FlagProperty("maintainSize", value: this.maintainSize, ifFalse: "maintainSize"));
        properties.add(new global::Doroti.Generated.Framework.Foundation.FlagProperty("maintainSemantics", value: this.maintainSemantics, ifFalse: "maintainSemantics"));
        properties.add(new global::Doroti.Generated.Framework.Foundation.FlagProperty("maintainInteractivity", value: this.maintainInteractivity, ifFalse: "maintainInteractivity"));
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

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new _RenderVisibility__indexed_stack(this.visible, this.maintainSemantics));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderVisibility__indexed_stack)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<_RenderVisibility__indexed_stack>)(() =>
{            var __cascade = __renderObject;
            __cascade.visible = this.visible;
            __cascade.maintainSemantics = this.maintainSemantics;
            return __cascade;        }))());
    }

}

public class _RenderVisibility__indexed_stack : global::Doroti.Generated.Framework.Rendering.RenderProxyBox
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
        get => this._visible;
        set
        {
            var __value = value;
            if ((__value == this.visible))
            {
                return;
            }
            _visible = __value;
            markNeedsPaint();
        }
    }
    public virtual bool maintainSemantics
    {
        get => this._maintainSemantics;
        set
        {
            var __value = value;
            if ((__value == this.maintainSemantics))
            {
                return;
            }
            _maintainSemantics = __value;
            markNeedsSemanticsUpdate();
        }
    }
    public override void visitChildrenForSemantics(global::System.Action<global::Doroti.Generated.Framework.Rendering.RenderObject> visitor)
    {
        if ((this.maintainSemantics || this.visible))
        {
            base.visitChildrenForSemantics((global::System.Action<global::Doroti.Generated.Framework.Rendering.RenderObject>)visitor);
        }
    }

    public virtual void paint(global::Doroti.Generated.Framework.Rendering.PaintingContext context, Offset offset)
    {
        if (!this.visible)
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

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new _RenderSliverVisibility__indexed_stack(this.visible, this.maintainSemantics));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderSliverVisibility__indexed_stack)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<_RenderSliverVisibility__indexed_stack>)(() =>
{            var __cascade = __renderObject;
            __cascade.visible = this.visible;
            __cascade.maintainSemantics = this.maintainSemantics;
            return __cascade;        }))());
    }

}

public class _RenderSliverVisibility__indexed_stack : global::Doroti.Generated.Framework.Rendering.RenderProxySliver
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
        get => this._visible;
        set
        {
            var __value = value;
            if ((__value == this.visible))
            {
                return;
            }
            _visible = __value;
            markNeedsPaint();
        }
    }
    public virtual bool maintainSemantics
    {
        get => this._maintainSemantics;
        set
        {
            var __value = value;
            if ((__value == this.maintainSemantics))
            {
                return;
            }
            _maintainSemantics = __value;
            markNeedsSemanticsUpdate();
        }
    }
    public override void visitChildrenForSemantics(global::System.Action<global::Doroti.Generated.Framework.Rendering.RenderObject> visitor)
    {
        if ((this.maintainSemantics || this.visible))
        {
            base.visitChildrenForSemantics((global::System.Action<global::Doroti.Generated.Framework.Rendering.RenderObject>)visitor);
        }
    }

    public override void paint(global::Doroti.Generated.Framework.Rendering.PaintingContext context, Offset offset)
    {
        if (!this.visible)
        {
            return;
        }
        base.paint(context, offset);
    }

}
