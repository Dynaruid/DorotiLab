// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/text_selection_toolbar_anchors.dart
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public class TextSelectionToolbarAnchors
{
    public virtual Offset primaryAnchor { get; private set; } = default!;
    public virtual Offset? secondaryAnchor { get; private set; }

    public TextSelectionToolbarAnchors(Offset primaryAnchor, Offset? secondaryAnchor = null)
    {
        this.primaryAnchor = primaryAnchor;
        this.secondaryAnchor = secondaryAnchor;
    }

    public static TextSelectionToolbarAnchors CreateFromSelection(global::Doroti.Framework.Rendering.RenderBox renderBox, double startGlyphHeight, double endGlyphHeight, List<global::Doroti.Framework.Rendering.TextSelectionPoint> selectionEndpoints)
    {
        global::Doroti.Ui.Rect selectionRect = getSelectionRect(renderBox, startGlyphHeight, endGlyphHeight, selectionEndpoints);
        if (Equals(selectionRect, Rect.zero))
        {
            return new TextSelectionToolbarAnchors(primaryAnchor: Offset.zero);
        }
        global::Doroti.Ui.Rect editingRegion = _getEditingRegion(renderBox);
        return new TextSelectionToolbarAnchors(primaryAnchor: new global::Doroti.Ui.Offset(selectionRect.left + (selectionRect.width / 2L), Dart_uiLibrary.clampDouble(selectionRect.top, editingRegion.top, editingRegion.bottom)), secondaryAnchor: new global::Doroti.Ui.Offset(selectionRect.left + (selectionRect.width / 2L), Dart_uiLibrary.clampDouble(selectionRect.bottom, editingRegion.top, editingRegion.bottom)));
    }

    internal static global::Doroti.Ui.Rect _getEditingRegion(global::Doroti.Framework.Rendering.RenderBox renderBox)
    {
        return Rect.fromPoints(renderBox.localToGlobal(Offset.zero), renderBox.localToGlobal(renderBox.size.bottomRight(Offset.zero)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static global::Doroti.Ui.Rect getSelectionRect(global::Doroti.Framework.Rendering.RenderBox renderBox, double startGlyphHeight, double endGlyphHeight, List<global::Doroti.Framework.Rendering.TextSelectionPoint> selectionEndpoints)
    {
        global::Doroti.Ui.Rect editingRegion = _getEditingRegion(renderBox);
        if (double.IsNaN(editingRegion.left) || double.IsNaN(editingRegion.top) || double.IsNaN(editingRegion.right) || double.IsNaN(editingRegion.bottom))
        {
            return Rect.zero;
        }
        bool isMultiline = (selectionEndpoints.Last().point.dy - selectionEndpoints.First().point.dy) > (endGlyphHeight / 2L);
        return Rect.fromLTRB(isMultiline ? editingRegion.left : (editingRegion.left + selectionEndpoints.First().point.dx), editingRegion.top + selectionEndpoints.First().point.dy - startGlyphHeight, isMultiline ? editingRegion.right : (editingRegion.left + selectionEndpoints.Last().point.dx), editingRegion.top + selectionEndpoints.Last().point.dy);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

