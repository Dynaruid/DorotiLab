// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/text_selection_toolbar_anchors.dart
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

public class TextSelectionToolbarAnchors
{
    public virtual Offset primaryAnchor { get; private set; } = default!;
    public virtual Offset? secondaryAnchor { get; private set; }

    public TextSelectionToolbarAnchors(Offset primaryAnchor, Offset? secondaryAnchor = null)
    {
        this.primaryAnchor = primaryAnchor;
        this.secondaryAnchor = secondaryAnchor;
    }

    public static TextSelectionToolbarAnchors CreateFromSelection(global::Doroti.Generated.Framework.Rendering.RenderBox renderBox, double startGlyphHeight, double endGlyphHeight, List<global::Doroti.Generated.Framework.Rendering.TextSelectionPoint> selectionEndpoints)
    {
        global::Doroti.Ui.Rect selectionRect__1187 = ((global::Doroti.Ui.Rect)(object?)TextSelectionToolbarAnchors.getSelectionRect(renderBox, startGlyphHeight, endGlyphHeight, selectionEndpoints));
        if ((object.Equals(selectionRect__1187, Rect.zero)))
        {
            return new TextSelectionToolbarAnchors(primaryAnchor: Offset.zero);
        }
        global::Doroti.Ui.Rect editingRegion__1453 = ((global::Doroti.Ui.Rect)(object?)TextSelectionToolbarAnchors._getEditingRegion(renderBox));
        return new TextSelectionToolbarAnchors(primaryAnchor: new global::Doroti.Ui.Offset((selectionRect__1187.left + (selectionRect__1187.width / 2L)), Dart_uiLibrary.clampDouble(selectionRect__1187.top, editingRegion__1453.top, editingRegion__1453.bottom)), secondaryAnchor: new global::Doroti.Ui.Offset((selectionRect__1187.left + (selectionRect__1187.width / 2L)), Dart_uiLibrary.clampDouble(selectionRect__1187.bottom, editingRegion__1453.top, editingRegion__1453.bottom)));
    }

    internal static global::Doroti.Ui.Rect _getEditingRegion(global::Doroti.Generated.Framework.Rendering.RenderBox renderBox)
    {
        return global::Doroti.Ui.Rect.fromPoints(((Offset)((dynamic)renderBox).localToGlobal(Offset.zero)), ((Offset)((dynamic)renderBox).localToGlobal(((global::Doroti.Generated.Framework.Rendering.RenderBox)renderBox).size.bottomRight(Offset.zero))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static global::Doroti.Ui.Rect getSelectionRect(global::Doroti.Generated.Framework.Rendering.RenderBox renderBox, double startGlyphHeight, double endGlyphHeight, List<global::Doroti.Generated.Framework.Rendering.TextSelectionPoint> selectionEndpoints)
    {
        global::Doroti.Ui.Rect editingRegion__2471 = ((global::Doroti.Ui.Rect)(object?)TextSelectionToolbarAnchors._getEditingRegion(renderBox));
        if ((((double.IsNaN(editingRegion__2471.left) || double.IsNaN(editingRegion__2471.top)) || double.IsNaN(editingRegion__2471.right)) || double.IsNaN(editingRegion__2471.bottom)))
        {
            return Rect.zero;
        }
        bool isMultiline__2710 = ((selectionEndpoints.Last().point.dy - selectionEndpoints.First().point.dy) > (endGlyphHeight / 2L));
        return global::Doroti.Ui.Rect.fromLTRB((isMultiline__2710 ? editingRegion__2471.left : (editingRegion__2471.left + selectionEndpoints.First().point.dx)), ((editingRegion__2471.top + selectionEndpoints.First().point.dy) - startGlyphHeight), (isMultiline__2710 ? editingRegion__2471.right : (editingRegion__2471.left + selectionEndpoints.Last().point.dx)), (editingRegion__2471.top + selectionEndpoints.Last().point.dy));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

