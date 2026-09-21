// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/selection_area.dart

using Doroti.Runtime;

namespace Doroti.Framework.Material;

public class SelectionArea : StatefulWidget
{
    public virtual TextMagnifierConfiguration? magnifierConfiguration { get; private set; }
    public virtual FocusNode? focusNode { get; private set; }
    public virtual TextSelectionControls? selectionControls { get; private set; }
    public virtual Func<BuildContext, SelectableRegionState, Widget>? contextMenuBuilder
    {
        get;
        private set;
    }
    public virtual Action<SelectedContent?>? onSelectionChanged { get; private set; }
    public virtual Widget child { get; private set; } = default!;

    public SelectionArea(
        Key? key = null,
        FocusNode? focusNode = null,
        TextSelectionControls? selectionControls = null,
        Func<BuildContext, SelectableRegionState, Widget>? contextMenuBuilder = default!,
        TextMagnifierConfiguration? magnifierConfiguration = null,
        Action<SelectedContent?>? onSelectionChanged = null,
        Widget child = default!
    )
        : base(key: key)
    {
        Func<BuildContext, SelectableRegionState, Widget>? __contextMenuBuilder =
            contextMenuBuilder ?? _defaultContextMenuBuilder;
        this.focusNode = focusNode;
        this.selectionControls = selectionControls;
        this.contextMenuBuilder = __contextMenuBuilder;
        this.magnifierConfiguration = magnifierConfiguration;
        this.onSelectionChanged = onSelectionChanged;
        this.child = child;
    }

    internal static Widget _defaultContextMenuBuilder(
        BuildContext context,
        SelectableRegionState selectableRegionState
    )
    {
        return AdaptiveTextSelectionToolbar.CreateSelectableRegion(
            selectableRegionState: selectableRegionState
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new SelectionAreaState());
}

public class SelectionAreaState : State<SelectionArea>
{
    internal virtual GlobalKey<SelectableRegionState> _selectableRegionKey { get; private set; } =
        GlobalKey<SelectableRegionState>.Create();

    public virtual SelectableRegionState selectableRegion =>
        DartRuntimePrimitives.ConvertValue<SelectableRegionState>(
            _selectableRegionKey.currentState!
        );

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() =>
            DebugLibrary.debugCheckHasMaterialLocalizations(context)
        );
        TextSelectionControls controls =
            widget.selectionControls
            ?? (
                Theme.of(context).platform switch
                {
                    TargetPlatform.android =>
                        Text_selectionLibrary.materialTextSelectionHandleControls,
                    TargetPlatform.fuchsia =>
                        Text_selectionLibrary.materialTextSelectionHandleControls,
                    TargetPlatform.linux =>
                        Desktop_text_selectionLibrary.desktopTextSelectionHandleControls,
                    TargetPlatform.windows =>
                        Desktop_text_selectionLibrary.desktopTextSelectionHandleControls,
                    TargetPlatform.iOS => Text_selectionLibrary.materialTextSelectionHandleControls,
                    TargetPlatform.macOS =>
                        Desktop_text_selectionLibrary.desktopTextSelectionHandleControls,
                    _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                        throw new InvalidOperationException(
                            "Switch expression did not handle the supplied value."
                        ),
                }
            );
        return new SelectableRegion(
            key: _selectableRegionKey,
            selectionControls: controls,
            focusNode: widget.focusNode,
            contextMenuBuilder: widget.contextMenuBuilder,
            magnifierConfiguration: widget.magnifierConfiguration
                ?? TextMagnifier.adaptiveMagnifierConfiguration,
            onSelectionChanged: widget.onSelectionChanged,
            child: widget.child
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}
