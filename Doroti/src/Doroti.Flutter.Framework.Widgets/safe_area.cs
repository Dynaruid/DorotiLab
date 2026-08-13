// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../flutter-master/packages/flutter/lib/src/widgets/safe_area.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Widgets;

public class SafeArea : StatelessWidget
{
    public virtual bool left { get; private set; } = default!;
    public virtual bool top { get; private set; } = default!;
    public virtual bool right { get; private set; } = default!;
    public virtual bool bottom { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsets minimum { get; private set; } = default!;
    public virtual bool maintainBottomViewPadding { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;

    public SafeArea(global::Doroti.Generated.Framework.Foundation.Key? key = null, bool left = true, bool top = true, bool right = true, bool bottom = true, global::Doroti.Generated.Framework.Painting.EdgeInsets minimum = default!, bool maintainBottomViewPadding = false, Widget child = default!) : base(key: key)
    {
        global::Doroti.Generated.Framework.Painting.EdgeInsets __minimum = minimum ?? global::Doroti.Generated.Framework.Painting.EdgeInsets.zero;
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
        DartRuntimePrimitives.Assert(() => global::Doroti.Generated.Framework.Widgets.DebugLibrary.debugCheckHasMediaQuery(context));
        global::Doroti.Generated.Framework.Painting.EdgeInsets padding__3761 = ((global::Doroti.Generated.Framework.Painting.EdgeInsets)(object?)MediaQuery.paddingOf(context));
        if (this.maintainBottomViewPadding)
        {
            padding__3761 = padding__3761.copyWith(bottom: MediaQuery.viewPaddingOf(context).bottom);
        }
        return ((Widget)(object?)new Padding(padding: global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateOnly(left: Math.Max((this.left ? ((global::Doroti.Generated.Framework.Painting.EdgeInsets)padding__3761).left : 0.0), ((global::Doroti.Generated.Framework.Painting.EdgeInsets)this.minimum).left), top: Math.Max((this.top ? ((global::Doroti.Generated.Framework.Painting.EdgeInsets)padding__3761).top : 0.0), ((global::Doroti.Generated.Framework.Painting.EdgeInsets)this.minimum).top), right: Math.Max((this.right ? ((global::Doroti.Generated.Framework.Painting.EdgeInsets)padding__3761).right : 0.0), ((global::Doroti.Generated.Framework.Painting.EdgeInsets)this.minimum).right), bottom: Math.Max((this.bottom ? ((global::Doroti.Generated.Framework.Painting.EdgeInsets)padding__3761).bottom : 0.0), ((global::Doroti.Generated.Framework.Painting.EdgeInsets)this.minimum).bottom)), child: MediaQuery.CreateRemovePadding(context: context, removeLeft: this.left, removeTop: this.top, removeRight: this.right, removeBottom: this.bottom, child: this.child)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.FlagProperty("left", value: this.left, ifTrue: "avoid left padding"));
        properties.add(new global::Doroti.Generated.Framework.Foundation.FlagProperty("top", value: this.top, ifTrue: "avoid top padding"));
        properties.add(new global::Doroti.Generated.Framework.Foundation.FlagProperty("right", value: this.right, ifTrue: "avoid right padding"));
        properties.add(new global::Doroti.Generated.Framework.Foundation.FlagProperty("bottom", value: this.bottom, ifTrue: "avoid bottom padding"));
    }

}

public class SliverSafeArea : StatelessWidget
{
    public virtual bool left { get; private set; } = default!;
    public virtual bool top { get; private set; } = default!;
    public virtual bool right { get; private set; } = default!;
    public virtual bool bottom { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsets minimum { get; private set; } = default!;
    public virtual Widget sliver { get; private set; } = default!;

    public SliverSafeArea(global::Doroti.Generated.Framework.Foundation.Key? key = null, bool left = true, bool top = true, bool right = true, bool bottom = true, global::Doroti.Generated.Framework.Painting.EdgeInsets minimum = default!, Widget sliver = default!) : base(key: key)
    {
        global::Doroti.Generated.Framework.Painting.EdgeInsets __minimum = minimum ?? global::Doroti.Generated.Framework.Painting.EdgeInsets.zero;
        this.left = left;
        this.top = top;
        this.right = right;
        this.bottom = bottom;
        this.minimum = __minimum;
        this.sliver = sliver;
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Generated.Framework.Widgets.DebugLibrary.debugCheckHasMediaQuery(context));
        global::Doroti.Generated.Framework.Painting.EdgeInsets padding__7030 = ((global::Doroti.Generated.Framework.Painting.EdgeInsets)(object?)MediaQuery.paddingOf(context));
        return ((Widget)(object?)new SliverPadding(padding: global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateOnly(left: Math.Max((this.left ? ((global::Doroti.Generated.Framework.Painting.EdgeInsets)padding__7030).left : 0.0), ((global::Doroti.Generated.Framework.Painting.EdgeInsets)this.minimum).left), top: Math.Max((this.top ? ((global::Doroti.Generated.Framework.Painting.EdgeInsets)padding__7030).top : 0.0), ((global::Doroti.Generated.Framework.Painting.EdgeInsets)this.minimum).top), right: Math.Max((this.right ? ((global::Doroti.Generated.Framework.Painting.EdgeInsets)padding__7030).right : 0.0), ((global::Doroti.Generated.Framework.Painting.EdgeInsets)this.minimum).right), bottom: Math.Max((this.bottom ? ((global::Doroti.Generated.Framework.Painting.EdgeInsets)padding__7030).bottom : 0.0), ((global::Doroti.Generated.Framework.Painting.EdgeInsets)this.minimum).bottom)), sliver: MediaQuery.CreateRemovePadding(context: context, removeLeft: this.left, removeTop: this.top, removeRight: this.right, removeBottom: this.bottom, child: this.sliver)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.FlagProperty("left", value: this.left, ifTrue: "avoid left padding"));
        properties.add(new global::Doroti.Generated.Framework.Foundation.FlagProperty("top", value: this.top, ifTrue: "avoid top padding"));
        properties.add(new global::Doroti.Generated.Framework.Foundation.FlagProperty("right", value: this.right, ifTrue: "avoid right padding"));
        properties.add(new global::Doroti.Generated.Framework.Foundation.FlagProperty("bottom", value: this.bottom, ifTrue: "avoid bottom padding"));
    }

}

