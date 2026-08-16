// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/selection_area.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Material;

public class SelectionArea : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual global::Doroti.Framework.Widgets.TextMagnifierConfiguration? magnifierConfiguration { get; private set; }
    public virtual global::Doroti.Framework.Widgets.FocusNode? focusNode { get; private set; }
    public virtual global::Doroti.Framework.Widgets.TextSelectionControls? selectionControls { get; private set; }
    public virtual global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.SelectableRegionState, global::Doroti.Framework.Widgets.Widget>? contextMenuBuilder { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Rendering.SelectedContent?>? onSelectionChanged { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget child { get; private set; } = default!;

    public SelectionArea(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Widgets.FocusNode? focusNode = null, global::Doroti.Framework.Widgets.TextSelectionControls? selectionControls = null, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.SelectableRegionState, global::Doroti.Framework.Widgets.Widget>? contextMenuBuilder = default!, global::Doroti.Framework.Widgets.TextMagnifierConfiguration? magnifierConfiguration = null, global::System.Action<global::Doroti.Framework.Rendering.SelectedContent?>? onSelectionChanged = null, global::Doroti.Framework.Widgets.Widget child = default!) : base(key: key)
    {
        global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.SelectableRegionState, global::Doroti.Framework.Widgets.Widget>? __contextMenuBuilder = contextMenuBuilder ?? _defaultContextMenuBuilder;
        this.focusNode = focusNode;
        this.selectionControls = selectionControls;
        this.contextMenuBuilder = __contextMenuBuilder;
        this.magnifierConfiguration = magnifierConfiguration;
        this.onSelectionChanged = onSelectionChanged;
        this.child = child;
    }

    internal static global::Doroti.Framework.Widgets.Widget _defaultContextMenuBuilder(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.SelectableRegionState selectableRegionState)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)AdaptiveTextSelectionToolbar.CreateSelectableRegion(selectableRegionState: selectableRegionState));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new SelectionAreaState());
}

public class SelectionAreaState : global::Doroti.Framework.Widgets.State<SelectionArea>
{
    internal virtual global::Doroti.Framework.Widgets.GlobalKey<global::Doroti.Framework.Widgets.SelectableRegionState> _selectableRegionKey { get; private set; } = global::Doroti.Framework.Widgets.GlobalKey<global::Doroti.Framework.Widgets.SelectableRegionState>.Create();

    public virtual global::Doroti.Framework.Widgets.SelectableRegionState selectableRegion => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.SelectableRegionState>(((global::Doroti.Framework.Widgets.GlobalKey<global::Doroti.Framework.Widgets.SelectableRegionState>)this._selectableRegionKey).currentState!);
    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterialLocalizations(context));
        global::Doroti.Framework.Widgets.TextSelectionControls controls__4347 = (((SelectionArea)this.widget).selectionControls ?? (Theme.of(context).platform switch { global::Doroti.Framework.Foundation.TargetPlatform.android => Text_selectionLibrary.materialTextSelectionHandleControls, global::Doroti.Framework.Foundation.TargetPlatform.fuchsia => Text_selectionLibrary.materialTextSelectionHandleControls, global::Doroti.Framework.Foundation.TargetPlatform.linux => Desktop_text_selectionLibrary.desktopTextSelectionHandleControls, global::Doroti.Framework.Foundation.TargetPlatform.windows => Desktop_text_selectionLibrary.desktopTextSelectionHandleControls, global::Doroti.Framework.Foundation.TargetPlatform.iOS => Text_selectionLibrary.materialTextSelectionHandleControls, global::Doroti.Framework.Foundation.TargetPlatform.macOS => Desktop_text_selectionLibrary.desktopTextSelectionHandleControls, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.SelectableRegion(key: this._selectableRegionKey, selectionControls: controls__4347, focusNode: ((SelectionArea)this.widget).focusNode, contextMenuBuilder: (global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.SelectableRegionState, global::Doroti.Framework.Widgets.Widget>?)((SelectionArea)this.widget).contextMenuBuilder, magnifierConfiguration: (((SelectionArea)this.widget).magnifierConfiguration ?? TextMagnifier.adaptiveMagnifierConfiguration), onSelectionChanged: (global::System.Action<global::Doroti.Framework.Rendering.SelectedContent?>?)((SelectionArea)this.widget).onSelectionChanged, child: ((SelectionArea)this.widget).child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
