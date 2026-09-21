// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/safe_area.dart
using Doroti.Runtime;

namespace Doroti.Framework.Widgets;

public class SafeArea : StatelessWidget
{
    public virtual bool left { get; private set; } = default!;
    public virtual bool top { get; private set; } = default!;
    public virtual bool right { get; private set; } = default!;
    public virtual bool bottom { get; private set; } = default!;
    public virtual EdgeInsets minimum { get; private set; } = default!;
    public virtual bool maintainBottomViewPadding { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;

    public SafeArea(
        Key? key = null,
        bool left = true,
        bool top = true,
        bool right = true,
        bool bottom = true,
        EdgeInsets minimum = default!,
        bool maintainBottomViewPadding = false,
        Widget child = default!
    )
        : base(key: key)
    {
        EdgeInsets __minimum = minimum ?? EdgeInsets.zero;
        this.left = left;
        this.top = top;
        this.right = right;
        this.bottom = bottom;
        this.minimum = __minimum;
        this.maintainBottomViewPadding = maintainBottomViewPadding;
        this.child = child;
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMediaQuery(context));
        EdgeInsets paddingLocal = MediaQuery.paddingOf(context);
        if (maintainBottomViewPadding)
        {
            paddingLocal = paddingLocal.copyWith(bottom: MediaQuery.viewPaddingOf(context).bottom);
        }
        return new Padding(
            padding: EdgeInsets.CreateOnly(
                left: Math.Max(left ? paddingLocal.left : 0.0, minimum.left),
                top: Math.Max(top ? paddingLocal.top : 0.0, minimum.top),
                right: Math.Max(right ? paddingLocal.right : 0.0, minimum.right),
                bottom: Math.Max(bottom ? paddingLocal.bottom : 0.0, minimum.bottom)
            ),
            child: MediaQuery.CreateRemovePadding(
                context: context,
                removeLeft: left,
                removeTop: top,
                removeRight: right,
                removeBottom: bottom,
                child: child
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new FlagProperty("left", value: left, ifTrue: "avoid left padding"));
        properties.add(new FlagProperty("top", value: top, ifTrue: "avoid top padding"));
        properties.add(new FlagProperty("right", value: right, ifTrue: "avoid right padding"));
        properties.add(new FlagProperty("bottom", value: bottom, ifTrue: "avoid bottom padding"));
    }
}

public class SliverSafeArea : StatelessWidget
{
    public virtual bool left { get; private set; } = default!;
    public virtual bool top { get; private set; } = default!;
    public virtual bool right { get; private set; } = default!;
    public virtual bool bottom { get; private set; } = default!;
    public virtual EdgeInsets minimum { get; private set; } = default!;
    public virtual Widget sliver { get; private set; } = default!;

    public SliverSafeArea(
        Key? key = null,
        bool left = true,
        bool top = true,
        bool right = true,
        bool bottom = true,
        EdgeInsets minimum = default!,
        Widget sliver = default!
    )
        : base(key: key)
    {
        EdgeInsets __minimum = minimum ?? EdgeInsets.zero;
        this.left = left;
        this.top = top;
        this.right = right;
        this.bottom = bottom;
        this.minimum = __minimum;
        this.sliver = sliver;
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMediaQuery(context));
        EdgeInsets paddingLocal = MediaQuery.paddingOf(context);
        return new SliverPadding(
            padding: EdgeInsets.CreateOnly(
                left: Math.Max(left ? paddingLocal.left : 0.0, minimum.left),
                top: Math.Max(top ? paddingLocal.top : 0.0, minimum.top),
                right: Math.Max(right ? paddingLocal.right : 0.0, minimum.right),
                bottom: Math.Max(bottom ? paddingLocal.bottom : 0.0, minimum.bottom)
            ),
            sliver: MediaQuery.CreateRemovePadding(
                context: context,
                removeLeft: left,
                removeTop: top,
                removeRight: right,
                removeBottom: bottom,
                child: sliver
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new FlagProperty("left", value: left, ifTrue: "avoid left padding"));
        properties.add(new FlagProperty("top", value: top, ifTrue: "avoid top padding"));
        properties.add(new FlagProperty("right", value: right, ifTrue: "avoid right padding"));
        properties.add(new FlagProperty("bottom", value: bottom, ifTrue: "avoid bottom padding"));
    }
}
