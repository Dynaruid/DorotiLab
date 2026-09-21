// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/desktop_text_selection_toolbar.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public static partial class Desktop_text_selection_toolbarLibrary
{
    internal static double _kToolbarScreenPadding = 8.0;
}

public static partial class Desktop_text_selection_toolbarLibrary
{
    internal static double _kToolbarWidth = 222.0;
}

public class DesktopTextSelectionToolbar : StatelessWidget
{
    public virtual Offset anchor { get; private set; } = default!;
    public virtual List<Widget> children { get; private set; } = default!;

    public DesktopTextSelectionToolbar(
        Key? key = null,
        Offset anchor = default!,
        List<Widget> children = default!
    )
        : base(key: key)
    {
        this.anchor = anchor;
        this.children = children;
        System.Diagnostics.Debug.Assert(checked(children.Count) > 0L);
    }

    internal static Widget _defaultToolbarBuilder(BuildContext context, Widget child)
    {
        return new SizedBox(
            width: Desktop_text_selection_toolbarLibrary._kToolbarWidth,
            child: new Material(
                borderRadius: BorderRadius.CreateAll(Radius.circular(7.0)),
                clipBehavior: Clip.antiAlias,
                elevation: 1.0,
                type: MaterialType.card,
                child: child
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => Widgets.DebugLibrary.debugCheckHasMediaQuery(context));
        double paddingAbove =
            MediaQuery.paddingOf(context).top
            + Desktop_text_selection_toolbarLibrary._kToolbarScreenPadding;
        var localAdjustment = new Offset(
            Desktop_text_selection_toolbarLibrary._kToolbarScreenPadding,
            paddingAbove
        );
        return new Padding(
            padding: new EdgeInsets(
                Desktop_text_selection_toolbarLibrary._kToolbarScreenPadding,
                paddingAbove,
                Desktop_text_selection_toolbarLibrary._kToolbarScreenPadding,
                Desktop_text_selection_toolbarLibrary._kToolbarScreenPadding
            ),
            child: new CustomSingleChildLayout(
                @delegate: new DesktopTextSelectionToolbarLayoutDelegate(
                    anchor: anchor - localAdjustment
                ),
                child: _defaultToolbarBuilder(
                    context,
                    new Column(mainAxisSize: MainAxisSize.min, children: children)
                )
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}
