// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/floating_action_button_location.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public static partial class Floating_action_button_locationLibrary
{
    public static double kFloatingActionButtonMargin = 16.0;
}

public static partial class Floating_action_button_locationLibrary
{
    public static Duration kFloatingActionButtonSegue = Duration.Create(milliseconds: 200L);
}

public static partial class Floating_action_button_locationLibrary
{
    public static double kFloatingActionButtonTurnInterval = 0.125;
}

public static partial class Floating_action_button_locationLibrary
{
    public static double kMiniButtonOffsetAdjustment = 4.0;
}

public abstract class FloatingActionButtonLocation
{
    public static FloatingActionButtonLocation startTop =
        new _StartTopFabLocation__floating_action_button_location();
    public static FloatingActionButtonLocation miniStartTop =
        new _MiniStartTopFabLocation__floating_action_button_location();
    public static FloatingActionButtonLocation centerTop =
        new _CenterTopFabLocation__floating_action_button_location();
    public static FloatingActionButtonLocation miniCenterTop =
        new _MiniCenterTopFabLocation__floating_action_button_location();
    public static FloatingActionButtonLocation endTop =
        new _EndTopFabLocation__floating_action_button_location();
    public static FloatingActionButtonLocation miniEndTop =
        new _MiniEndTopFabLocation__floating_action_button_location();
    public static FloatingActionButtonLocation startFloat =
        new _StartFloatFabLocation__floating_action_button_location();
    public static FloatingActionButtonLocation miniStartFloat =
        new _MiniStartFloatFabLocation__floating_action_button_location();
    public static FloatingActionButtonLocation centerFloat =
        new _CenterFloatFabLocation__floating_action_button_location();
    public static FloatingActionButtonLocation miniCenterFloat =
        new _MiniCenterFloatFabLocation__floating_action_button_location();
    public static FloatingActionButtonLocation endFloat =
        new _EndFloatFabLocation__floating_action_button_location();
    public static FloatingActionButtonLocation miniEndFloat =
        new _MiniEndFloatFabLocation__floating_action_button_location();
    public static FloatingActionButtonLocation startDocked =
        new _StartDockedFabLocation__floating_action_button_location();
    public static FloatingActionButtonLocation miniStartDocked =
        new _MiniStartDockedFabLocation__floating_action_button_location();
    public static FloatingActionButtonLocation centerDocked =
        new _CenterDockedFabLocation__floating_action_button_location();
    public static FloatingActionButtonLocation miniCenterDocked =
        new _MiniCenterDockedFabLocation__floating_action_button_location();
    public static FloatingActionButtonLocation endDocked =
        new _EndDockedFabLocation__floating_action_button_location();
    public static FloatingActionButtonLocation miniEndDocked =
        new _MiniEndDockedFabLocation__floating_action_button_location();
    public static FloatingActionButtonLocation endContained =
        new _EndContainedFabLocation__floating_action_button_location();

    protected FloatingActionButtonLocation() { }

    public abstract Offset getOffset(ScaffoldPrelayoutGeometry scaffoldGeometry);

    public override string ToString() =>
        objectRuntimeTypeFunctions.objectRuntimeType(this, "FloatingActionButtonLocation");
}

public abstract class StandardFabLocation : FloatingActionButtonLocation
{
    protected StandardFabLocation() { }

    public abstract double getOffsetX(
        ScaffoldPrelayoutGeometry scaffoldGeometry,
        double adjustment
    );
    public abstract double getOffsetY(
        ScaffoldPrelayoutGeometry scaffoldGeometry,
        double adjustment
    );

    public virtual bool isMini() => false;

    public override Offset getOffset(ScaffoldPrelayoutGeometry scaffoldGeometry)
    {
        double adjustment = isMini()
            ? Floating_action_button_locationLibrary.kMiniButtonOffsetAdjustment
            : 0.0;
        return new Offset(
            getOffsetX(scaffoldGeometry, adjustment),
            getOffsetY(scaffoldGeometry, adjustment)
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal static double _leftOffsetX(
        ScaffoldPrelayoutGeometry scaffoldGeometry,
        double adjustment
    )
    {
        return Floating_action_button_locationLibrary.kFloatingActionButtonMargin
            + scaffoldGeometry.minInsets.left
            - adjustment;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal static double _rightOffsetX(
        ScaffoldPrelayoutGeometry scaffoldGeometry,
        double adjustment
    )
    {
        return scaffoldGeometry.scaffoldSize.width
            - Floating_action_button_locationLibrary.kFloatingActionButtonMargin
            - scaffoldGeometry.minInsets.right
            - scaffoldGeometry.floatingActionButtonSize.width
            + adjustment;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public interface FabTopOffsetY
{
    public double getOffsetY(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment);
}

public interface FabFloatOffsetY
{
    public double getOffsetY(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment);
}

public interface FabDockedOffsetY
{
    public double getOffsetY(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment);
}

public interface FabContainedOffsetY
{
    public double getOffsetY(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment);
}

public interface FabStartOffsetX
{
    public double getOffsetX(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment);
}

public interface FabCenterOffsetX
{
    public double getOffsetX(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment);
}

public interface FabEndOffsetX
{
    public double getOffsetX(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment);
}

public interface FabMiniOffsetAdjustment
{
    public bool isMini();
}

internal class _StartTopFabLocation__floating_action_button_location
    : StandardFabLocation,
        FabStartOffsetX,
        FabTopOffsetY
{
    internal _StartTopFabLocation__floating_action_button_location() { }

    public override string ToString() => "FloatingActionButtonLocation.startTop";

    public override double getOffsetX(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        return scaffoldGeometry.textDirection switch
        {
            TextDirection.rtl => _rightOffsetX(scaffoldGeometry, adjustment),
            TextDirection.ltr => _leftOffsetX(scaffoldGeometry, adjustment),
            _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                ),
        };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double getOffsetY(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        if (scaffoldGeometry.contentTop > scaffoldGeometry.minViewPadding.top)
        {
            double fabHalfHeight = scaffoldGeometry.floatingActionButtonSize.height / 2.0;
            return scaffoldGeometry.contentTop - fabHalfHeight;
        }
        return scaffoldGeometry.minViewPadding.top;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _MiniStartTopFabLocation__floating_action_button_location
    : StandardFabLocation,
        FabMiniOffsetAdjustment,
        FabStartOffsetX,
        FabTopOffsetY
{
    internal _MiniStartTopFabLocation__floating_action_button_location() { }

    public override string ToString() => "FloatingActionButtonLocation.miniStartTop";

    public override bool isMini() => true;

    public override double getOffsetX(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        return scaffoldGeometry.textDirection switch
        {
            TextDirection.rtl => _rightOffsetX(scaffoldGeometry, adjustment),
            TextDirection.ltr => _leftOffsetX(scaffoldGeometry, adjustment),
            _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                ),
        };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double getOffsetY(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        if (scaffoldGeometry.contentTop > scaffoldGeometry.minViewPadding.top)
        {
            double fabHalfHeight = scaffoldGeometry.floatingActionButtonSize.height / 2.0;
            return scaffoldGeometry.contentTop - fabHalfHeight;
        }
        return scaffoldGeometry.minViewPadding.top;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _CenterTopFabLocation__floating_action_button_location
    : StandardFabLocation,
        FabCenterOffsetX,
        FabTopOffsetY
{
    internal _CenterTopFabLocation__floating_action_button_location() { }

    public override string ToString() => "FloatingActionButtonLocation.centerTop";

    public override double getOffsetX(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        return (
                scaffoldGeometry.scaffoldSize.width
                - scaffoldGeometry.floatingActionButtonSize.width
            ) / 2.0;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double getOffsetY(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        if (scaffoldGeometry.contentTop > scaffoldGeometry.minViewPadding.top)
        {
            double fabHalfHeight = scaffoldGeometry.floatingActionButtonSize.height / 2.0;
            return scaffoldGeometry.contentTop - fabHalfHeight;
        }
        return scaffoldGeometry.minViewPadding.top;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _MiniCenterTopFabLocation__floating_action_button_location
    : StandardFabLocation,
        FabMiniOffsetAdjustment,
        FabCenterOffsetX,
        FabTopOffsetY
{
    internal _MiniCenterTopFabLocation__floating_action_button_location() { }

    public override string ToString() => "FloatingActionButtonLocation.miniCenterTop";

    public override bool isMini() => true;

    public override double getOffsetX(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        return (
                scaffoldGeometry.scaffoldSize.width
                - scaffoldGeometry.floatingActionButtonSize.width
            ) / 2.0;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double getOffsetY(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        if (scaffoldGeometry.contentTop > scaffoldGeometry.minViewPadding.top)
        {
            double fabHalfHeight = scaffoldGeometry.floatingActionButtonSize.height / 2.0;
            return scaffoldGeometry.contentTop - fabHalfHeight;
        }
        return scaffoldGeometry.minViewPadding.top;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _EndTopFabLocation__floating_action_button_location
    : StandardFabLocation,
        FabEndOffsetX,
        FabTopOffsetY
{
    internal _EndTopFabLocation__floating_action_button_location() { }

    public override string ToString() => "FloatingActionButtonLocation.endTop";

    public override double getOffsetX(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        return scaffoldGeometry.textDirection switch
        {
            TextDirection.rtl => _leftOffsetX(scaffoldGeometry, adjustment),
            TextDirection.ltr => _rightOffsetX(scaffoldGeometry, adjustment),
            _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                ),
        };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double getOffsetY(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        if (scaffoldGeometry.contentTop > scaffoldGeometry.minViewPadding.top)
        {
            double fabHalfHeight = scaffoldGeometry.floatingActionButtonSize.height / 2.0;
            return scaffoldGeometry.contentTop - fabHalfHeight;
        }
        return scaffoldGeometry.minViewPadding.top;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _MiniEndTopFabLocation__floating_action_button_location
    : StandardFabLocation,
        FabMiniOffsetAdjustment,
        FabEndOffsetX,
        FabTopOffsetY
{
    internal _MiniEndTopFabLocation__floating_action_button_location() { }

    public override string ToString() => "FloatingActionButtonLocation.miniEndTop";

    public override bool isMini() => true;

    public override double getOffsetX(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        return scaffoldGeometry.textDirection switch
        {
            TextDirection.rtl => _leftOffsetX(scaffoldGeometry, adjustment),
            TextDirection.ltr => _rightOffsetX(scaffoldGeometry, adjustment),
            _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                ),
        };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double getOffsetY(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        if (scaffoldGeometry.contentTop > scaffoldGeometry.minViewPadding.top)
        {
            double fabHalfHeight = scaffoldGeometry.floatingActionButtonSize.height / 2.0;
            return scaffoldGeometry.contentTop - fabHalfHeight;
        }
        return scaffoldGeometry.minViewPadding.top;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _StartFloatFabLocation__floating_action_button_location
    : StandardFabLocation,
        FabStartOffsetX,
        FabFloatOffsetY
{
    internal _StartFloatFabLocation__floating_action_button_location() { }

    public override string ToString() => "FloatingActionButtonLocation.startFloat";

    public override double getOffsetX(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        return scaffoldGeometry.textDirection switch
        {
            TextDirection.rtl => _rightOffsetX(scaffoldGeometry, adjustment),
            TextDirection.ltr => _leftOffsetX(scaffoldGeometry, adjustment),
            _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                ),
        };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double getOffsetY(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        double contentBottomLocal = scaffoldGeometry.contentBottom;
        double bottomContentHeight = scaffoldGeometry.scaffoldSize.height - contentBottomLocal;
        double bottomSheetHeight = scaffoldGeometry.bottomSheetSize.height;
        double fabHeight = scaffoldGeometry.floatingActionButtonSize.height;
        double snackBarHeight = scaffoldGeometry.snackBarSize.height;
        double safeMargin = Math.Max(
            Floating_action_button_locationLibrary.kFloatingActionButtonMargin,
            scaffoldGeometry.minViewPadding.bottom
                - bottomContentHeight
                + Floating_action_button_locationLibrary.kFloatingActionButtonMargin
        );
        double fabY = contentBottomLocal - fabHeight - safeMargin;
        if (snackBarHeight > 0.0)
        {
            fabY = Math.Min(
                fabY,
                contentBottomLocal
                    - snackBarHeight
                    - fabHeight
                    - Floating_action_button_locationLibrary.kFloatingActionButtonMargin
            );
        }
        if (bottomSheetHeight > 0.0)
        {
            fabY = Math.Min(fabY, contentBottomLocal - bottomSheetHeight - (fabHeight / 2.0));
        }
        return fabY + adjustment;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _MiniStartFloatFabLocation__floating_action_button_location
    : StandardFabLocation,
        FabMiniOffsetAdjustment,
        FabStartOffsetX,
        FabFloatOffsetY
{
    internal _MiniStartFloatFabLocation__floating_action_button_location() { }

    public override string ToString() => "FloatingActionButtonLocation.miniStartFloat";

    public override bool isMini() => true;

    public override double getOffsetX(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        return scaffoldGeometry.textDirection switch
        {
            TextDirection.rtl => _rightOffsetX(scaffoldGeometry, adjustment),
            TextDirection.ltr => _leftOffsetX(scaffoldGeometry, adjustment),
            _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                ),
        };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double getOffsetY(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        double contentBottomLocal = scaffoldGeometry.contentBottom;
        double bottomContentHeight = scaffoldGeometry.scaffoldSize.height - contentBottomLocal;
        double bottomSheetHeight = scaffoldGeometry.bottomSheetSize.height;
        double fabHeight = scaffoldGeometry.floatingActionButtonSize.height;
        double snackBarHeight = scaffoldGeometry.snackBarSize.height;
        double safeMargin = Math.Max(
            Floating_action_button_locationLibrary.kFloatingActionButtonMargin,
            scaffoldGeometry.minViewPadding.bottom
                - bottomContentHeight
                + Floating_action_button_locationLibrary.kFloatingActionButtonMargin
        );
        double fabY = contentBottomLocal - fabHeight - safeMargin;
        if (snackBarHeight > 0.0)
        {
            fabY = Math.Min(
                fabY,
                contentBottomLocal
                    - snackBarHeight
                    - fabHeight
                    - Floating_action_button_locationLibrary.kFloatingActionButtonMargin
            );
        }
        if (bottomSheetHeight > 0.0)
        {
            fabY = Math.Min(fabY, contentBottomLocal - bottomSheetHeight - (fabHeight / 2.0));
        }
        return fabY + adjustment;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _CenterFloatFabLocation__floating_action_button_location
    : StandardFabLocation,
        FabCenterOffsetX,
        FabFloatOffsetY
{
    internal _CenterFloatFabLocation__floating_action_button_location() { }

    public override string ToString() => "FloatingActionButtonLocation.centerFloat";

    public override double getOffsetX(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        return (
                scaffoldGeometry.scaffoldSize.width
                - scaffoldGeometry.floatingActionButtonSize.width
            ) / 2.0;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double getOffsetY(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        double contentBottomLocal = scaffoldGeometry.contentBottom;
        double bottomContentHeight = scaffoldGeometry.scaffoldSize.height - contentBottomLocal;
        double bottomSheetHeight = scaffoldGeometry.bottomSheetSize.height;
        double fabHeight = scaffoldGeometry.floatingActionButtonSize.height;
        double snackBarHeight = scaffoldGeometry.snackBarSize.height;
        double safeMargin = Math.Max(
            Floating_action_button_locationLibrary.kFloatingActionButtonMargin,
            scaffoldGeometry.minViewPadding.bottom
                - bottomContentHeight
                + Floating_action_button_locationLibrary.kFloatingActionButtonMargin
        );
        double fabY = contentBottomLocal - fabHeight - safeMargin;
        if (snackBarHeight > 0.0)
        {
            fabY = Math.Min(
                fabY,
                contentBottomLocal
                    - snackBarHeight
                    - fabHeight
                    - Floating_action_button_locationLibrary.kFloatingActionButtonMargin
            );
        }
        if (bottomSheetHeight > 0.0)
        {
            fabY = Math.Min(fabY, contentBottomLocal - bottomSheetHeight - (fabHeight / 2.0));
        }
        return fabY + adjustment;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _MiniCenterFloatFabLocation__floating_action_button_location
    : StandardFabLocation,
        FabMiniOffsetAdjustment,
        FabCenterOffsetX,
        FabFloatOffsetY
{
    internal _MiniCenterFloatFabLocation__floating_action_button_location() { }

    public override string ToString() => "FloatingActionButtonLocation.miniCenterFloat";

    public override bool isMini() => true;

    public override double getOffsetX(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        return (
                scaffoldGeometry.scaffoldSize.width
                - scaffoldGeometry.floatingActionButtonSize.width
            ) / 2.0;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double getOffsetY(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        double contentBottomLocal = scaffoldGeometry.contentBottom;
        double bottomContentHeight = scaffoldGeometry.scaffoldSize.height - contentBottomLocal;
        double bottomSheetHeight = scaffoldGeometry.bottomSheetSize.height;
        double fabHeight = scaffoldGeometry.floatingActionButtonSize.height;
        double snackBarHeight = scaffoldGeometry.snackBarSize.height;
        double safeMargin = Math.Max(
            Floating_action_button_locationLibrary.kFloatingActionButtonMargin,
            scaffoldGeometry.minViewPadding.bottom
                - bottomContentHeight
                + Floating_action_button_locationLibrary.kFloatingActionButtonMargin
        );
        double fabY = contentBottomLocal - fabHeight - safeMargin;
        if (snackBarHeight > 0.0)
        {
            fabY = Math.Min(
                fabY,
                contentBottomLocal
                    - snackBarHeight
                    - fabHeight
                    - Floating_action_button_locationLibrary.kFloatingActionButtonMargin
            );
        }
        if (bottomSheetHeight > 0.0)
        {
            fabY = Math.Min(fabY, contentBottomLocal - bottomSheetHeight - (fabHeight / 2.0));
        }
        return fabY + adjustment;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _EndFloatFabLocation__floating_action_button_location
    : StandardFabLocation,
        FabEndOffsetX,
        FabFloatOffsetY
{
    internal _EndFloatFabLocation__floating_action_button_location() { }

    public override string ToString() => "FloatingActionButtonLocation.endFloat";

    public override double getOffsetX(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        return scaffoldGeometry.textDirection switch
        {
            TextDirection.rtl => _leftOffsetX(scaffoldGeometry, adjustment),
            TextDirection.ltr => _rightOffsetX(scaffoldGeometry, adjustment),
            _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                ),
        };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double getOffsetY(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        double contentBottomLocal = scaffoldGeometry.contentBottom;
        double bottomContentHeight = scaffoldGeometry.scaffoldSize.height - contentBottomLocal;
        double bottomSheetHeight = scaffoldGeometry.bottomSheetSize.height;
        double fabHeight = scaffoldGeometry.floatingActionButtonSize.height;
        double snackBarHeight = scaffoldGeometry.snackBarSize.height;
        double safeMargin = Math.Max(
            Floating_action_button_locationLibrary.kFloatingActionButtonMargin,
            scaffoldGeometry.minViewPadding.bottom
                - bottomContentHeight
                + Floating_action_button_locationLibrary.kFloatingActionButtonMargin
        );
        double fabY = contentBottomLocal - fabHeight - safeMargin;
        if (snackBarHeight > 0.0)
        {
            fabY = Math.Min(
                fabY,
                contentBottomLocal
                    - snackBarHeight
                    - fabHeight
                    - Floating_action_button_locationLibrary.kFloatingActionButtonMargin
            );
        }
        if (bottomSheetHeight > 0.0)
        {
            fabY = Math.Min(fabY, contentBottomLocal - bottomSheetHeight - (fabHeight / 2.0));
        }
        return fabY + adjustment;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _MiniEndFloatFabLocation__floating_action_button_location
    : StandardFabLocation,
        FabMiniOffsetAdjustment,
        FabEndOffsetX,
        FabFloatOffsetY
{
    internal _MiniEndFloatFabLocation__floating_action_button_location() { }

    public override string ToString() => "FloatingActionButtonLocation.miniEndFloat";

    public override bool isMini() => true;

    public override double getOffsetX(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        return scaffoldGeometry.textDirection switch
        {
            TextDirection.rtl => _leftOffsetX(scaffoldGeometry, adjustment),
            TextDirection.ltr => _rightOffsetX(scaffoldGeometry, adjustment),
            _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                ),
        };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double getOffsetY(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        double contentBottomLocal = scaffoldGeometry.contentBottom;
        double bottomContentHeight = scaffoldGeometry.scaffoldSize.height - contentBottomLocal;
        double bottomSheetHeight = scaffoldGeometry.bottomSheetSize.height;
        double fabHeight = scaffoldGeometry.floatingActionButtonSize.height;
        double snackBarHeight = scaffoldGeometry.snackBarSize.height;
        double safeMargin = Math.Max(
            Floating_action_button_locationLibrary.kFloatingActionButtonMargin,
            scaffoldGeometry.minViewPadding.bottom
                - bottomContentHeight
                + Floating_action_button_locationLibrary.kFloatingActionButtonMargin
        );
        double fabY = contentBottomLocal - fabHeight - safeMargin;
        if (snackBarHeight > 0.0)
        {
            fabY = Math.Min(
                fabY,
                contentBottomLocal
                    - snackBarHeight
                    - fabHeight
                    - Floating_action_button_locationLibrary.kFloatingActionButtonMargin
            );
        }
        if (bottomSheetHeight > 0.0)
        {
            fabY = Math.Min(fabY, contentBottomLocal - bottomSheetHeight - (fabHeight / 2.0));
        }
        return fabY + adjustment;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _StartDockedFabLocation__floating_action_button_location
    : StandardFabLocation,
        FabStartOffsetX,
        FabDockedOffsetY
{
    internal _StartDockedFabLocation__floating_action_button_location() { }

    public override string ToString() => "FloatingActionButtonLocation.startDocked";

    public override double getOffsetX(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        return scaffoldGeometry.textDirection switch
        {
            TextDirection.rtl => _rightOffsetX(scaffoldGeometry, adjustment),
            TextDirection.ltr => _leftOffsetX(scaffoldGeometry, adjustment),
            _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                ),
        };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double getOffsetY(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        double contentBottomLocal = scaffoldGeometry.contentBottom;
        double contentMargin = scaffoldGeometry.scaffoldSize.height - contentBottomLocal;
        double bottomViewPadding = scaffoldGeometry.minViewPadding.bottom;
        double bottomSheetHeight = scaffoldGeometry.bottomSheetSize.height;
        double fabHeight = scaffoldGeometry.floatingActionButtonSize.height;
        double snackBarHeight = scaffoldGeometry.snackBarSize.height;
        double bottomMinInset = scaffoldGeometry.minInsets.bottom;
        double safeMargin = default!;
        if (contentMargin > (bottomMinInset + (fabHeight / 2.0)))
        {
            safeMargin = 0.0;
        }
        else
        {
            if (bottomMinInset == 0.0)
            {
                safeMargin = bottomViewPadding;
            }
            else
            {
                safeMargin =
                    (fabHeight / 2.0)
                    + Floating_action_button_locationLibrary.kFloatingActionButtonMargin;
            }
        }
        double fabY = contentBottomLocal - (fabHeight / 2.0) - safeMargin;
        if (snackBarHeight > 0.0)
        {
            fabY = Math.Min(
                fabY,
                contentBottomLocal
                    - snackBarHeight
                    - fabHeight
                    - Floating_action_button_locationLibrary.kFloatingActionButtonMargin
            );
        }
        if (bottomSheetHeight > 0.0)
        {
            fabY = Math.Min(fabY, contentBottomLocal - bottomSheetHeight - (fabHeight / 2.0));
        }
        double maxFabY = scaffoldGeometry.scaffoldSize.height - fabHeight - safeMargin;
        return Math.Min(maxFabY, fabY);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _MiniStartDockedFabLocation__floating_action_button_location
    : StandardFabLocation,
        FabMiniOffsetAdjustment,
        FabStartOffsetX,
        FabDockedOffsetY
{
    internal _MiniStartDockedFabLocation__floating_action_button_location() { }

    public override string ToString() => "FloatingActionButtonLocation.miniStartDocked";

    public override bool isMini() => true;

    public override double getOffsetX(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        return scaffoldGeometry.textDirection switch
        {
            TextDirection.rtl => _rightOffsetX(scaffoldGeometry, adjustment),
            TextDirection.ltr => _leftOffsetX(scaffoldGeometry, adjustment),
            _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                ),
        };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double getOffsetY(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        double contentBottomLocal = scaffoldGeometry.contentBottom;
        double contentMargin = scaffoldGeometry.scaffoldSize.height - contentBottomLocal;
        double bottomViewPadding = scaffoldGeometry.minViewPadding.bottom;
        double bottomSheetHeight = scaffoldGeometry.bottomSheetSize.height;
        double fabHeight = scaffoldGeometry.floatingActionButtonSize.height;
        double snackBarHeight = scaffoldGeometry.snackBarSize.height;
        double bottomMinInset = scaffoldGeometry.minInsets.bottom;
        double safeMargin = default!;
        if (contentMargin > (bottomMinInset + (fabHeight / 2.0)))
        {
            safeMargin = 0.0;
        }
        else
        {
            if (bottomMinInset == 0.0)
            {
                safeMargin = bottomViewPadding;
            }
            else
            {
                safeMargin =
                    (fabHeight / 2.0)
                    + Floating_action_button_locationLibrary.kFloatingActionButtonMargin;
            }
        }
        double fabY = contentBottomLocal - (fabHeight / 2.0) - safeMargin;
        if (snackBarHeight > 0.0)
        {
            fabY = Math.Min(
                fabY,
                contentBottomLocal
                    - snackBarHeight
                    - fabHeight
                    - Floating_action_button_locationLibrary.kFloatingActionButtonMargin
            );
        }
        if (bottomSheetHeight > 0.0)
        {
            fabY = Math.Min(fabY, contentBottomLocal - bottomSheetHeight - (fabHeight / 2.0));
        }
        double maxFabY = scaffoldGeometry.scaffoldSize.height - fabHeight - safeMargin;
        return Math.Min(maxFabY, fabY);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _CenterDockedFabLocation__floating_action_button_location
    : StandardFabLocation,
        FabCenterOffsetX,
        FabDockedOffsetY
{
    internal _CenterDockedFabLocation__floating_action_button_location() { }

    public override string ToString() => "FloatingActionButtonLocation.centerDocked";

    public override double getOffsetX(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        return (
                scaffoldGeometry.scaffoldSize.width
                - scaffoldGeometry.floatingActionButtonSize.width
            ) / 2.0;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double getOffsetY(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        double contentBottomLocal = scaffoldGeometry.contentBottom;
        double contentMargin = scaffoldGeometry.scaffoldSize.height - contentBottomLocal;
        double bottomViewPadding = scaffoldGeometry.minViewPadding.bottom;
        double bottomSheetHeight = scaffoldGeometry.bottomSheetSize.height;
        double fabHeight = scaffoldGeometry.floatingActionButtonSize.height;
        double snackBarHeight = scaffoldGeometry.snackBarSize.height;
        double bottomMinInset = scaffoldGeometry.minInsets.bottom;
        double safeMargin = default!;
        if (contentMargin > (bottomMinInset + (fabHeight / 2.0)))
        {
            safeMargin = 0.0;
        }
        else
        {
            if (bottomMinInset == 0.0)
            {
                safeMargin = bottomViewPadding;
            }
            else
            {
                safeMargin =
                    (fabHeight / 2.0)
                    + Floating_action_button_locationLibrary.kFloatingActionButtonMargin;
            }
        }
        double fabY = contentBottomLocal - (fabHeight / 2.0) - safeMargin;
        if (snackBarHeight > 0.0)
        {
            fabY = Math.Min(
                fabY,
                contentBottomLocal
                    - snackBarHeight
                    - fabHeight
                    - Floating_action_button_locationLibrary.kFloatingActionButtonMargin
            );
        }
        if (bottomSheetHeight > 0.0)
        {
            fabY = Math.Min(fabY, contentBottomLocal - bottomSheetHeight - (fabHeight / 2.0));
        }
        double maxFabY = scaffoldGeometry.scaffoldSize.height - fabHeight - safeMargin;
        return Math.Min(maxFabY, fabY);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _MiniCenterDockedFabLocation__floating_action_button_location
    : StandardFabLocation,
        FabMiniOffsetAdjustment,
        FabCenterOffsetX,
        FabDockedOffsetY
{
    internal _MiniCenterDockedFabLocation__floating_action_button_location() { }

    public override string ToString() => "FloatingActionButtonLocation.miniCenterDocked";

    public override bool isMini() => true;

    public override double getOffsetX(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        return (
                scaffoldGeometry.scaffoldSize.width
                - scaffoldGeometry.floatingActionButtonSize.width
            ) / 2.0;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double getOffsetY(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        double contentBottomLocal = scaffoldGeometry.contentBottom;
        double contentMargin = scaffoldGeometry.scaffoldSize.height - contentBottomLocal;
        double bottomViewPadding = scaffoldGeometry.minViewPadding.bottom;
        double bottomSheetHeight = scaffoldGeometry.bottomSheetSize.height;
        double fabHeight = scaffoldGeometry.floatingActionButtonSize.height;
        double snackBarHeight = scaffoldGeometry.snackBarSize.height;
        double bottomMinInset = scaffoldGeometry.minInsets.bottom;
        double safeMargin = default!;
        if (contentMargin > (bottomMinInset + (fabHeight / 2.0)))
        {
            safeMargin = 0.0;
        }
        else
        {
            if (bottomMinInset == 0.0)
            {
                safeMargin = bottomViewPadding;
            }
            else
            {
                safeMargin =
                    (fabHeight / 2.0)
                    + Floating_action_button_locationLibrary.kFloatingActionButtonMargin;
            }
        }
        double fabY = contentBottomLocal - (fabHeight / 2.0) - safeMargin;
        if (snackBarHeight > 0.0)
        {
            fabY = Math.Min(
                fabY,
                contentBottomLocal
                    - snackBarHeight
                    - fabHeight
                    - Floating_action_button_locationLibrary.kFloatingActionButtonMargin
            );
        }
        if (bottomSheetHeight > 0.0)
        {
            fabY = Math.Min(fabY, contentBottomLocal - bottomSheetHeight - (fabHeight / 2.0));
        }
        double maxFabY = scaffoldGeometry.scaffoldSize.height - fabHeight - safeMargin;
        return Math.Min(maxFabY, fabY);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _EndDockedFabLocation__floating_action_button_location
    : StandardFabLocation,
        FabEndOffsetX,
        FabDockedOffsetY
{
    internal _EndDockedFabLocation__floating_action_button_location() { }

    public override string ToString() => "FloatingActionButtonLocation.endDocked";

    public override double getOffsetX(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        return scaffoldGeometry.textDirection switch
        {
            TextDirection.rtl => _leftOffsetX(scaffoldGeometry, adjustment),
            TextDirection.ltr => _rightOffsetX(scaffoldGeometry, adjustment),
            _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                ),
        };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double getOffsetY(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        double contentBottomLocal = scaffoldGeometry.contentBottom;
        double contentMargin = scaffoldGeometry.scaffoldSize.height - contentBottomLocal;
        double bottomViewPadding = scaffoldGeometry.minViewPadding.bottom;
        double bottomSheetHeight = scaffoldGeometry.bottomSheetSize.height;
        double fabHeight = scaffoldGeometry.floatingActionButtonSize.height;
        double snackBarHeight = scaffoldGeometry.snackBarSize.height;
        double bottomMinInset = scaffoldGeometry.minInsets.bottom;
        double safeMargin = default!;
        if (contentMargin > (bottomMinInset + (fabHeight / 2.0)))
        {
            safeMargin = 0.0;
        }
        else
        {
            if (bottomMinInset == 0.0)
            {
                safeMargin = bottomViewPadding;
            }
            else
            {
                safeMargin =
                    (fabHeight / 2.0)
                    + Floating_action_button_locationLibrary.kFloatingActionButtonMargin;
            }
        }
        double fabY = contentBottomLocal - (fabHeight / 2.0) - safeMargin;
        if (snackBarHeight > 0.0)
        {
            fabY = Math.Min(
                fabY,
                contentBottomLocal
                    - snackBarHeight
                    - fabHeight
                    - Floating_action_button_locationLibrary.kFloatingActionButtonMargin
            );
        }
        if (bottomSheetHeight > 0.0)
        {
            fabY = Math.Min(fabY, contentBottomLocal - bottomSheetHeight - (fabHeight / 2.0));
        }
        double maxFabY = scaffoldGeometry.scaffoldSize.height - fabHeight - safeMargin;
        return Math.Min(maxFabY, fabY);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _MiniEndDockedFabLocation__floating_action_button_location
    : StandardFabLocation,
        FabMiniOffsetAdjustment,
        FabEndOffsetX,
        FabDockedOffsetY
{
    internal _MiniEndDockedFabLocation__floating_action_button_location() { }

    public override string ToString() => "FloatingActionButtonLocation.miniEndDocked";

    public override bool isMini() => true;

    public override double getOffsetX(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        return scaffoldGeometry.textDirection switch
        {
            TextDirection.rtl => _leftOffsetX(scaffoldGeometry, adjustment),
            TextDirection.ltr => _rightOffsetX(scaffoldGeometry, adjustment),
            _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                ),
        };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double getOffsetY(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        double contentBottomLocal = scaffoldGeometry.contentBottom;
        double contentMargin = scaffoldGeometry.scaffoldSize.height - contentBottomLocal;
        double bottomViewPadding = scaffoldGeometry.minViewPadding.bottom;
        double bottomSheetHeight = scaffoldGeometry.bottomSheetSize.height;
        double fabHeight = scaffoldGeometry.floatingActionButtonSize.height;
        double snackBarHeight = scaffoldGeometry.snackBarSize.height;
        double bottomMinInset = scaffoldGeometry.minInsets.bottom;
        double safeMargin = default!;
        if (contentMargin > (bottomMinInset + (fabHeight / 2.0)))
        {
            safeMargin = 0.0;
        }
        else
        {
            if (bottomMinInset == 0.0)
            {
                safeMargin = bottomViewPadding;
            }
            else
            {
                safeMargin =
                    (fabHeight / 2.0)
                    + Floating_action_button_locationLibrary.kFloatingActionButtonMargin;
            }
        }
        double fabY = contentBottomLocal - (fabHeight / 2.0) - safeMargin;
        if (snackBarHeight > 0.0)
        {
            fabY = Math.Min(
                fabY,
                contentBottomLocal
                    - snackBarHeight
                    - fabHeight
                    - Floating_action_button_locationLibrary.kFloatingActionButtonMargin
            );
        }
        if (bottomSheetHeight > 0.0)
        {
            fabY = Math.Min(fabY, contentBottomLocal - bottomSheetHeight - (fabHeight / 2.0));
        }
        double maxFabY = scaffoldGeometry.scaffoldSize.height - fabHeight - safeMargin;
        return Math.Min(maxFabY, fabY);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _EndContainedFabLocation__floating_action_button_location
    : StandardFabLocation,
        FabEndOffsetX,
        FabContainedOffsetY
{
    internal _EndContainedFabLocation__floating_action_button_location() { }

    public override string ToString() => "FloatingActionButtonLocation.endContained";

    public override double getOffsetX(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        return scaffoldGeometry.textDirection switch
        {
            TextDirection.rtl => _leftOffsetX(scaffoldGeometry, adjustment),
            TextDirection.ltr => _rightOffsetX(scaffoldGeometry, adjustment),
            _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                ),
        };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double getOffsetY(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        double contentBottomLocal = scaffoldGeometry.contentBottom;
        double contentMargin = scaffoldGeometry.scaffoldSize.height - contentBottomLocal;
        double bottomViewPadding = scaffoldGeometry.minViewPadding.bottom;
        double fabHeight = scaffoldGeometry.floatingActionButtonSize.height;
        double safeMargin = default!;
        if (contentMargin > (bottomViewPadding + fabHeight))
        {
            safeMargin = 0.0;
        }
        else
        {
            safeMargin = bottomViewPadding;
        }
        double contentBottomToFabTop = (contentMargin - bottomViewPadding - fabHeight) / 2.0;
        double fabY = contentBottomLocal + contentBottomToFabTop;
        double maxFabY = scaffoldGeometry.scaffoldSize.height - fabHeight - safeMargin;
        return Math.Min(maxFabY, fabY);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public abstract class FloatingActionButtonAnimator
{
    public static FloatingActionButtonAnimator scaling =
        new _ScalingFabMotionAnimator__floating_action_button_location();
    public static FloatingActionButtonAnimator noAnimation =
        new _NoAnimationFabMotionAnimator__floating_action_button_location();

    protected FloatingActionButtonAnimator() { }

    public abstract Offset getOffset(Offset begin, Offset end, double progress);
    public abstract Animation<double> getScaleAnimation(Animation<double> parent);
    public abstract Animation<double> getRotationAnimation(Animation<double> parent);

    public virtual double getAnimationRestart(double previousValue) => 0.0;

    public override string ToString() =>
        objectRuntimeTypeFunctions.objectRuntimeType(this, "FloatingActionButtonAnimator");
}

internal class _ScalingFabMotionAnimator__floating_action_button_location
    : FloatingActionButtonAnimator
{
    internal static Animatable<double> _rotationTween = new Tween<double>(
        begin: 1.0
            - (Floating_action_button_locationLibrary.kFloatingActionButtonTurnInterval * 2.0),
        end: 1.0
    );
    internal static Animatable<double> _thresholdCenterTween = new CurveTween(
        curve: new Threshold(0.5)
    );

    internal _ScalingFabMotionAnimator__floating_action_button_location() { }

    public override Offset getOffset(Offset begin, Offset end, double progress)
    {
        if (progress < 0.5)
        {
            return begin;
        }
        else
        {
            return end;
        }
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Animation<double> getScaleAnimation(Animation<double> parent)
    {
        Curve curveLocal = new Interval(0.5, 1.0, curve: Curves.ease);
        return new _AnimationSwap__floating_action_button_location<double>(
            new ReverseAnimation(parent.drive(new CurveTween(curve: curveLocal.flipped))),
            parent.drive(new CurveTween(curve: curveLocal)),
            parent,
            0.5
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Animation<double> getRotationAnimation(Animation<double> parent)
    {
        return new _AnimationSwap__floating_action_button_location<double>(
            parent.drive(_rotationTween),
            new ReverseAnimation(parent.drive(_thresholdCenterTween)),
            parent,
            0.5
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double getAnimationRestart(double previousValue) =>
        Math.Min(1.0 - previousValue, previousValue);
}

internal class _NoAnimationFabMotionAnimator__floating_action_button_location
    : FloatingActionButtonAnimator
{
    internal _NoAnimationFabMotionAnimator__floating_action_button_location() { }

    public override Offset getOffset(Offset begin, Offset end, double progress)
    {
        return end;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Animation<double> getRotationAnimation(Animation<double> parent)
    {
        return new AlwaysStoppedAnimation<double>(1.0);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Animation<double> getScaleAnimation(Animation<double> parent)
    {
        return new AlwaysStoppedAnimation<double>(1.0);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _AnimationSwap__floating_action_button_location<T> : CompoundAnimation<T>
{
    public virtual Animation<double> parent { get; private set; } = default!;
    public virtual double swapThreshold { get; private set; } = default!;

    internal _AnimationSwap__floating_action_button_location(
        Animation<T> first,
        Animation<T> next,
        Animation<double> parent,
        double swapThreshold
    )
        : base(first: first, next: next)
    {
        this.parent = parent;
        this.swapThreshold = swapThreshold;
    }

    public override T value => (parent.value < swapThreshold) ? first.value : next.value;
}
