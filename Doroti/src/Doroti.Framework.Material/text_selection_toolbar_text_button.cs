// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/text_selection_toolbar_text_button.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Material;

internal enum _TextSelectionToolbarItemPosition__text_selection_toolbar_text_button
{
    first,
    middle,
    last,
    only
}

public class TextSelectionToolbarTextButton : global::Doroti.Generated.Framework.Widgets.StatelessWidget
{
    internal const double _kMiddlePadding = 9.5;
    internal const double _kEndPadding = 14.5;
    public virtual global::Doroti.Generated.Framework.Widgets.Widget child { get; private set; } = default!;
    public virtual global::System.Action? onPressed { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry padding { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.AlignmentGeometry? alignment { get; private set; }
    internal static Color _defaultForegroundColorLight = new global::Doroti.Ui.Color(4278190080L);
    internal static Color _defaultForegroundColorDark = new global::Doroti.Ui.Color(4294967295L);
    internal static Color _defaultBackgroundColorTransparent = new global::Doroti.Ui.Color(0L);

    public TextSelectionToolbarTextButton(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Widgets.Widget child = default!, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry padding = default!, global::System.Action? onPressed = null, global::Doroti.Generated.Framework.Painting.AlignmentGeometry? alignment = null) : base(key: key)
    {
        this.child = child;
        this.padding = padding;
        this.onPressed = onPressed;
        this.alignment = alignment;
    }

    public static global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry getPadding(long index, long total)
    {
        DartRuntimePrimitives.Assert(() => (((total > 0L) && (index >= 0L)) && (index < total)));
        _TextSelectionToolbarItemPosition__text_selection_toolbar_text_button position__2640 = TextSelectionToolbarTextButton._getPosition(index, total);
        return ((global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry)(object?)global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: TextSelectionToolbarTextButton._getStartPadding(position__2640), end: TextSelectionToolbarTextButton._getEndPadding(position__2640)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static double _getStartPadding(_TextSelectionToolbarItemPosition__text_selection_toolbar_text_button position)
    {
        if (((object.Equals(position, _TextSelectionToolbarItemPosition__text_selection_toolbar_text_button.first)) || (object.Equals(position, _TextSelectionToolbarItemPosition__text_selection_toolbar_text_button.only))))
        {
            return _kEndPadding;
        }
        return _kMiddlePadding;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static double _getEndPadding(_TextSelectionToolbarItemPosition__text_selection_toolbar_text_button position)
    {
        if (((object.Equals(position, _TextSelectionToolbarItemPosition__text_selection_toolbar_text_button.last)) || (object.Equals(position, _TextSelectionToolbarItemPosition__text_selection_toolbar_text_button.only))))
        {
            return _kEndPadding;
        }
        return _kMiddlePadding;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static _TextSelectionToolbarItemPosition__text_selection_toolbar_text_button _getPosition(long index, long total)
    {
        if ((index == 0L))
        {
            return ((total == 1L) ? _TextSelectionToolbarItemPosition__text_selection_toolbar_text_button.only : _TextSelectionToolbarItemPosition__text_selection_toolbar_text_button.first);
        }
        if ((index == (total - 1L)))
        {
            return _TextSelectionToolbarItemPosition__text_selection_toolbar_text_button.last;
        }
        return _TextSelectionToolbarItemPosition__text_selection_toolbar_text_button.middle;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual TextSelectionToolbarTextButton copyWith(global::Doroti.Generated.Framework.Widgets.Widget? child = null, global::System.Action? onPressed = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? padding = null, global::Doroti.Generated.Framework.Painting.AlignmentGeometry? alignment = null)
    {
        return new TextSelectionToolbarTextButton(onPressed: ((onPressed ?? (global::System.Action)this.onPressed)), padding: (padding ?? this.padding), alignment: (alignment ?? this.alignment), child: (child ?? ((global::Doroti.Generated.Framework.Widgets.Widget)((dynamic)this).child)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static global::Doroti.Ui.Color _getForegroundColor(ColorScheme colorScheme)
    {
        bool isDefaultOnSurface__4901 = (colorScheme.brightness switch { Brightness.light => DartRuntimePrimitives.Identical(ThemeData.Create().colorScheme.onSurface, colorScheme.onSurface), Brightness.dark => DartRuntimePrimitives.Identical(ThemeData.Create().colorScheme.onSurface, colorScheme.onSurface), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        if (!isDefaultOnSurface__4901)
        {
            return ((global::Doroti.Ui.Color)(object?)colorScheme.onSurface);
        }
        return ((global::Doroti.Ui.Color)(object?)(colorScheme.brightness switch { Brightness.light => _defaultForegroundColorLight, Brightness.dark => _defaultForegroundColorDark, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        ColorScheme colorScheme__5470 = Theme.of(context).colorScheme;
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new TextButton(style: TextButton.styleFrom(backgroundColor: _defaultBackgroundColorTransparent, foregroundColor: TextSelectionToolbarTextButton._getForegroundColor(colorScheme__5470), shape: new global::Doroti.Generated.Framework.Painting.RoundedRectangleBorder(), minimumSize: new global::Doroti.Ui.Size(global::Doroti.Generated.Framework.Widgets.ConstantsLibrary.kMinInteractiveDimension, global::Doroti.Generated.Framework.Widgets.ConstantsLibrary.kMinInteractiveDimension), padding: this.padding, alignment: this.alignment, textStyle: new global::Doroti.Generated.Framework.Painting.TextStyle(fontWeight: FontWeight.w400)), onPressed: () => this.onPressed(), child: this.child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
