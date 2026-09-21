// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/circle_avatar.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class CircleAvatar : StatelessWidget
{
    public virtual Widget? child { get; private set; }
    public virtual Color? backgroundColor { get; private set; }
    public virtual Color? foregroundColor { get; private set; }
    public virtual IImageProvider? backgroundImage { get; private set; } = default!;
    public virtual IImageProvider? foregroundImage { get; private set; } = default!;
    public virtual Action<object, System.Diagnostics.StackTrace?>? onBackgroundImageError
    {
        get;
        private set;
    }
    public virtual Action<object, System.Diagnostics.StackTrace?>? onForegroundImageError
    {
        get;
        private set;
    }
    public virtual double? radius { get; private set; }
    public virtual double? minRadius { get; private set; }
    public virtual double? maxRadius { get; private set; }
    internal const double _defaultRadius = 20.0;
    internal const double _defaultMinRadius = 0.0;
    internal static double _defaultMaxRadius = double.PositiveInfinity;

    public CircleAvatar(
        Key? key = null,
        Widget? child = null,
        Color? backgroundColor = null,
        IImageProvider? backgroundImage = null,
        IImageProvider? foregroundImage = null,
        Action<object, System.Diagnostics.StackTrace?>? onBackgroundImageError = null,
        Action<object, System.Diagnostics.StackTrace?>? onForegroundImageError = null,
        Color? foregroundColor = null,
        double? radius = null,
        double? minRadius = null,
        double? maxRadius = null
    )
        : base(key: key)
    {
        this.child = child;
        this.backgroundColor = backgroundColor;
        this.backgroundImage = backgroundImage;
        this.foregroundImage = foregroundImage;
        this.onBackgroundImageError = onBackgroundImageError;
        this.onForegroundImageError = onForegroundImageError;
        this.foregroundColor = foregroundColor;
        this.radius = radius;
        this.minRadius = minRadius;
        this.maxRadius = maxRadius;
        System.Diagnostics.Debug.Assert(
            (radius is null) || ((minRadius is null) && (maxRadius is null))
        );
        System.Diagnostics.Debug.Assert(
            (backgroundImage is not null) || (onBackgroundImageError is null)
        );
        System.Diagnostics.Debug.Assert(
            (foregroundImage is not null) || (onForegroundImageError is null)
        );
    }

    internal virtual double _minDiameter
    {
        get
        {
            if ((radius is null) && (minRadius is null) && (maxRadius is null))
            {
                return _defaultRadius * 2.0;
            }
            return 2.0 * ((radius ?? minRadius) ?? _defaultMinRadius);
        }
    }
    internal virtual double _maxDiameter
    {
        get
        {
            if ((radius is null) && (minRadius is null) && (maxRadius is null))
            {
                return _defaultRadius * 2.0;
            }
            return 2.0 * ((radius ?? maxRadius) ?? _defaultMaxRadius);
        }
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => Widgets.DebugLibrary.debugCheckHasMediaQuery(context));
        ThemeData theme = Theme.of(context);
        Color? effectiveForegroundColor = (Color?)(
            foregroundColor ?? theme.colorScheme.onPrimaryContainer
        );
        TextStyle effectiveTextStyle = theme.textTheme.titleMedium!;
        TextStyle textStyle = effectiveTextStyle.copyWith(color: effectiveForegroundColor);
        Color? effectiveBackgroundColor = (Color?)(
            backgroundColor ?? theme.colorScheme.primaryContainer
        );
        if (effectiveBackgroundColor is null)
        {
            effectiveBackgroundColor = ThemeData.estimateBrightnessForColor(textStyle.color!) switch
            {
                Brightness.dark => theme.primaryColorLight,
                Brightness.light => theme.primaryColorDark,
                _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                    throw new InvalidOperationException(
                        "Switch expression did not handle the supplied value."
                    ),
            };
        }
        else
        {
            if (effectiveForegroundColor is null)
            {
                textStyle = ThemeData.estimateBrightnessForColor(backgroundColor!) switch
                {
                    Brightness.dark => textStyle.copyWith(color: theme.primaryColorLight),
                    Brightness.light => textStyle.copyWith(color: theme.primaryColorDark),
                    _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                        throw new InvalidOperationException(
                            "Switch expression did not handle the supplied value."
                        ),
                };
            }
        }
        double minDiameter = _minDiameter;
        double maxDiameter = _maxDiameter;
        return new AnimatedContainer(
            constraints: new BoxConstraints(
                minHeight: minDiameter,
                minWidth: minDiameter,
                maxWidth: maxDiameter,
                maxHeight: maxDiameter
            ),
            duration: ConstantsLibrary.kThemeChangeDuration,
            decoration: new BoxDecoration(
                color: effectiveBackgroundColor,
                image: (backgroundImage is not null)
                    ? new DecorationImage(
                        image: backgroundImage!,
                        onError: onBackgroundImageError,
                        fit: BoxFit.cover
                    )
                    : null,
                shape: BoxShape.circle
            ),
            foregroundDecoration: (foregroundImage is not null)
                ? new BoxDecoration(
                    image: new DecorationImage(
                        image: foregroundImage!,
                        onError: onForegroundImageError,
                        fit: BoxFit.cover
                    ),
                    shape: BoxShape.circle
                )
                : null,
            child: (child is null)
                ? null
                : new Center(
                    child: MediaQuery.withNoTextScaling(
                        child: new IconTheme(
                            data: theme.iconTheme.copyWith(color: textStyle.color),
                            child: new DefaultTextStyle(style: textStyle, child: child!)
                        )
                    )
                )
        );
    }
}
