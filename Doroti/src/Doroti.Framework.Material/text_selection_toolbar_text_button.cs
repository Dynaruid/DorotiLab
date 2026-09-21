// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/text_selection_toolbar_text_button.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

internal enum _TextSelectionToolbarItemPosition__text_selection_toolbar_text_button
{
    first,
    middle,
    last,
    only,
}

public class TextSelectionToolbarTextButton : StatelessWidget
{
    internal const double _kMiddlePadding = 9.5;
    internal const double _kEndPadding = 14.5;
    public virtual Widget child { get; private set; } = default!;
    public virtual Action? onPressed { get; private set; }
    public virtual EdgeInsetsGeometry padding { get; private set; } = default!;
    public virtual AlignmentGeometry? alignment { get; private set; }
    internal static Color _defaultForegroundColorLight = new Color(4278190080L);
    internal static Color _defaultForegroundColorDark = new Color(4294967295L);
    internal static Color _defaultBackgroundColorTransparent = new Color(0L);

    public TextSelectionToolbarTextButton(
        Key? key = null,
        Widget child = default!,
        EdgeInsetsGeometry padding = default!,
        Action? onPressed = null,
        AlignmentGeometry? alignment = null
    )
        : base(key: key)
    {
        this.child = child;
        this.padding = padding;
        this.onPressed = onPressed;
        this.alignment = alignment;
    }

    public static EdgeInsetsGeometry getPadding(long index, long total)
    {
        DartRuntimePrimitives.Assert(() => (total > 0L) && (index >= 0L) && (index < total));
        _TextSelectionToolbarItemPosition__text_selection_toolbar_text_button position =
            _getPosition(index, total);
        return EdgeInsetsDirectional.CreateOnly(
            start: _getStartPadding(position),
            end: _getEndPadding(position)
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal static double _getStartPadding(
        _TextSelectionToolbarItemPosition__text_selection_toolbar_text_button position
    )
    {
        if (
            Equals(
                position,
                _TextSelectionToolbarItemPosition__text_selection_toolbar_text_button.first
            )
            || Equals(
                position,
                _TextSelectionToolbarItemPosition__text_selection_toolbar_text_button.only
            )
        )
        {
            return _kEndPadding;
        }
        return _kMiddlePadding;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal static double _getEndPadding(
        _TextSelectionToolbarItemPosition__text_selection_toolbar_text_button position
    )
    {
        if (
            Equals(
                position,
                _TextSelectionToolbarItemPosition__text_selection_toolbar_text_button.last
            )
            || Equals(
                position,
                _TextSelectionToolbarItemPosition__text_selection_toolbar_text_button.only
            )
        )
        {
            return _kEndPadding;
        }
        return _kMiddlePadding;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal static _TextSelectionToolbarItemPosition__text_selection_toolbar_text_button _getPosition(
        long index,
        long total
    )
    {
        if (index == 0L)
        {
            return (total == 1L)
                ? _TextSelectionToolbarItemPosition__text_selection_toolbar_text_button.only
                : _TextSelectionToolbarItemPosition__text_selection_toolbar_text_button.first;
        }
        if (index == (total - 1L))
        {
            return _TextSelectionToolbarItemPosition__text_selection_toolbar_text_button.last;
        }
        return _TextSelectionToolbarItemPosition__text_selection_toolbar_text_button.middle;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual TextSelectionToolbarTextButton copyWith(
        Widget? child = null,
        Action? onPressed = null,
        EdgeInsetsGeometry? padding = null,
        AlignmentGeometry? alignment = null
    )
    {
        return new TextSelectionToolbarTextButton(
            onPressed: onPressed ?? this.onPressed,
            padding: padding ?? this.padding,
            alignment: alignment ?? this.alignment,
            child: child ?? this.child
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal static Color _getForegroundColor(ColorScheme colorScheme)
    {
        bool isDefaultOnSurface = colorScheme.brightness switch
        {
            Brightness.light => DartRuntimePrimitives.Identical(
                ThemeData.Create().colorScheme.onSurface,
                colorScheme.onSurface
            ),
            Brightness.dark => DartRuntimePrimitives.Identical(
                ThemeData.Create().colorScheme.onSurface,
                colorScheme.onSurface
            ),
            _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                ),
        };
        if (!isDefaultOnSurface)
        {
            return colorScheme.onSurface;
        }
        return colorScheme.brightness switch
        {
            Brightness.light => _defaultForegroundColorLight,
            Brightness.dark => _defaultForegroundColorDark,
            _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                ),
        };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Widget build(BuildContext context)
    {
        ColorScheme colorSchemeLocal = Theme.of(context).colorScheme;
        return new TextButton(
            style: TextButton.styleFrom(
                backgroundColor: _defaultBackgroundColorTransparent,
                foregroundColor: _getForegroundColor(colorSchemeLocal),
                shape: new RoundedRectangleBorder(),
                minimumSize: new Size(
                    Widgets.ConstantsLibrary.kMinInteractiveDimension,
                    Widgets.ConstantsLibrary.kMinInteractiveDimension
                ),
                padding: padding,
                alignment: alignment,
                textStyle: new TextStyle(fontWeight: FontWeight.w400)
            ),
            onPressed: onPressed,
            child: child
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}
