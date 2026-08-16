// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/floating_action_button_location.dart
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
    public static FloatingActionButtonLocation startTop = ((FloatingActionButtonLocation)(object?)new _StartTopFabLocation__floating_action_button_location());
    public static FloatingActionButtonLocation miniStartTop = ((FloatingActionButtonLocation)(object?)new _MiniStartTopFabLocation__floating_action_button_location());
    public static FloatingActionButtonLocation centerTop = ((FloatingActionButtonLocation)(object?)new _CenterTopFabLocation__floating_action_button_location());
    public static FloatingActionButtonLocation miniCenterTop = ((FloatingActionButtonLocation)(object?)new _MiniCenterTopFabLocation__floating_action_button_location());
    public static FloatingActionButtonLocation endTop = ((FloatingActionButtonLocation)(object?)new _EndTopFabLocation__floating_action_button_location());
    public static FloatingActionButtonLocation miniEndTop = ((FloatingActionButtonLocation)(object?)new _MiniEndTopFabLocation__floating_action_button_location());
    public static FloatingActionButtonLocation startFloat = ((FloatingActionButtonLocation)(object?)new _StartFloatFabLocation__floating_action_button_location());
    public static FloatingActionButtonLocation miniStartFloat = ((FloatingActionButtonLocation)(object?)new _MiniStartFloatFabLocation__floating_action_button_location());
    public static FloatingActionButtonLocation centerFloat = ((FloatingActionButtonLocation)(object?)new _CenterFloatFabLocation__floating_action_button_location());
    public static FloatingActionButtonLocation miniCenterFloat = ((FloatingActionButtonLocation)(object?)new _MiniCenterFloatFabLocation__floating_action_button_location());
    public static FloatingActionButtonLocation endFloat = ((FloatingActionButtonLocation)(object?)new _EndFloatFabLocation__floating_action_button_location());
    public static FloatingActionButtonLocation miniEndFloat = ((FloatingActionButtonLocation)(object?)new _MiniEndFloatFabLocation__floating_action_button_location());
    public static FloatingActionButtonLocation startDocked = ((FloatingActionButtonLocation)(object?)new _StartDockedFabLocation__floating_action_button_location());
    public static FloatingActionButtonLocation miniStartDocked = ((FloatingActionButtonLocation)(object?)new _MiniStartDockedFabLocation__floating_action_button_location());
    public static FloatingActionButtonLocation centerDocked = ((FloatingActionButtonLocation)(object?)new _CenterDockedFabLocation__floating_action_button_location());
    public static FloatingActionButtonLocation miniCenterDocked = ((FloatingActionButtonLocation)(object?)new _MiniCenterDockedFabLocation__floating_action_button_location());
    public static FloatingActionButtonLocation endDocked = ((FloatingActionButtonLocation)(object?)new _EndDockedFabLocation__floating_action_button_location());
    public static FloatingActionButtonLocation miniEndDocked = ((FloatingActionButtonLocation)(object?)new _MiniEndDockedFabLocation__floating_action_button_location());
    public static FloatingActionButtonLocation endContained = ((FloatingActionButtonLocation)(object?)new _EndContainedFabLocation__floating_action_button_location());

    protected FloatingActionButtonLocation()
    {
    }

    public abstract global::Doroti.Ui.Offset getOffset(ScaffoldPrelayoutGeometry scaffoldGeometry);
    public override string ToString() => global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "FloatingActionButtonLocation");
}

public abstract class StandardFabLocation : FloatingActionButtonLocation
{
    protected StandardFabLocation()
    {
    }

    public abstract double getOffsetX(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment);
    public abstract double getOffsetY(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment);
    public virtual bool isMini() => false;
    public override Offset getOffset(ScaffoldPrelayoutGeometry scaffoldGeometry)
    {
        double adjustment__24552 = (isMini() ? Floating_action_button_locationLibrary.kMiniButtonOffsetAdjustment : 0.0);
        return new global::Doroti.Ui.Offset(getOffsetX(scaffoldGeometry, adjustment__24552), getOffsetY(scaffoldGeometry, adjustment__24552));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static double _leftOffsetX(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        return ((Floating_action_button_locationLibrary.kFloatingActionButtonMargin + scaffoldGeometry.minInsets.left) - adjustment);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static double _rightOffsetX(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        return ((((scaffoldGeometry.scaffoldSize.width - Floating_action_button_locationLibrary.kFloatingActionButtonMargin) - scaffoldGeometry.minInsets.right) - scaffoldGeometry.floatingActionButtonSize.width) + adjustment);
        throw new InvalidOperationException("Dart control flow completed without a value.");
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

internal class _StartTopFabLocation__floating_action_button_location : StandardFabLocation, FabStartOffsetX, FabTopOffsetY
{

    internal _StartTopFabLocation__floating_action_button_location()
    {
    }

    public override string ToString() => "FloatingActionButtonLocation.startTop";
    public override double getOffsetX(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        return (((TextDirection)((dynamic)scaffoldGeometry).textDirection) switch { TextDirection.rtl => StandardFabLocation._rightOffsetX(scaffoldGeometry, adjustment), TextDirection.ltr => StandardFabLocation._leftOffsetX(scaffoldGeometry, adjustment), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double getOffsetY(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        if ((scaffoldGeometry.contentTop > scaffoldGeometry.minViewPadding.top))
        {
            double fabHalfHeight__26280 = (scaffoldGeometry.floatingActionButtonSize.height / 2.0);
            return (scaffoldGeometry.contentTop - fabHalfHeight__26280);
        }
        return scaffoldGeometry.minViewPadding.top;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _MiniStartTopFabLocation__floating_action_button_location : StandardFabLocation, FabMiniOffsetAdjustment, FabStartOffsetX, FabTopOffsetY
{

    internal _MiniStartTopFabLocation__floating_action_button_location()
    {
    }

    public override string ToString() => "FloatingActionButtonLocation.miniStartTop";
    public override bool isMini() => true;
    public override double getOffsetX(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        return (((TextDirection)((dynamic)scaffoldGeometry).textDirection) switch { TextDirection.rtl => StandardFabLocation._rightOffsetX(scaffoldGeometry, adjustment), TextDirection.ltr => StandardFabLocation._leftOffsetX(scaffoldGeometry, adjustment), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double getOffsetY(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        if ((scaffoldGeometry.contentTop > scaffoldGeometry.minViewPadding.top))
        {
            double fabHalfHeight__26280 = (scaffoldGeometry.floatingActionButtonSize.height / 2.0);
            return (scaffoldGeometry.contentTop - fabHalfHeight__26280);
        }
        return scaffoldGeometry.minViewPadding.top;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _CenterTopFabLocation__floating_action_button_location : StandardFabLocation, FabCenterOffsetX, FabTopOffsetY
{

    internal _CenterTopFabLocation__floating_action_button_location()
    {
    }

    public override string ToString() => "FloatingActionButtonLocation.centerTop";
    public override double getOffsetX(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        return (((scaffoldGeometry.scaffoldSize.width - scaffoldGeometry.floatingActionButtonSize.width)) / 2.0);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double getOffsetY(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        if ((scaffoldGeometry.contentTop > scaffoldGeometry.minViewPadding.top))
        {
            double fabHalfHeight__26280 = (scaffoldGeometry.floatingActionButtonSize.height / 2.0);
            return (scaffoldGeometry.contentTop - fabHalfHeight__26280);
        }
        return scaffoldGeometry.minViewPadding.top;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _MiniCenterTopFabLocation__floating_action_button_location : StandardFabLocation, FabMiniOffsetAdjustment, FabCenterOffsetX, FabTopOffsetY
{

    internal _MiniCenterTopFabLocation__floating_action_button_location()
    {
    }

    public override string ToString() => "FloatingActionButtonLocation.miniCenterTop";
    public override bool isMini() => true;
    public override double getOffsetX(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        return (((scaffoldGeometry.scaffoldSize.width - scaffoldGeometry.floatingActionButtonSize.width)) / 2.0);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double getOffsetY(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        if ((scaffoldGeometry.contentTop > scaffoldGeometry.minViewPadding.top))
        {
            double fabHalfHeight__26280 = (scaffoldGeometry.floatingActionButtonSize.height / 2.0);
            return (scaffoldGeometry.contentTop - fabHalfHeight__26280);
        }
        return scaffoldGeometry.minViewPadding.top;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _EndTopFabLocation__floating_action_button_location : StandardFabLocation, FabEndOffsetX, FabTopOffsetY
{

    internal _EndTopFabLocation__floating_action_button_location()
    {
    }

    public override string ToString() => "FloatingActionButtonLocation.endTop";
    public override double getOffsetX(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        return (((TextDirection)((dynamic)scaffoldGeometry).textDirection) switch { TextDirection.rtl => StandardFabLocation._leftOffsetX(scaffoldGeometry, adjustment), TextDirection.ltr => StandardFabLocation._rightOffsetX(scaffoldGeometry, adjustment), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double getOffsetY(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        if ((scaffoldGeometry.contentTop > scaffoldGeometry.minViewPadding.top))
        {
            double fabHalfHeight__26280 = (scaffoldGeometry.floatingActionButtonSize.height / 2.0);
            return (scaffoldGeometry.contentTop - fabHalfHeight__26280);
        }
        return scaffoldGeometry.minViewPadding.top;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _MiniEndTopFabLocation__floating_action_button_location : StandardFabLocation, FabMiniOffsetAdjustment, FabEndOffsetX, FabTopOffsetY
{

    internal _MiniEndTopFabLocation__floating_action_button_location()
    {
    }

    public override string ToString() => "FloatingActionButtonLocation.miniEndTop";
    public override bool isMini() => true;
    public override double getOffsetX(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        return (((TextDirection)((dynamic)scaffoldGeometry).textDirection) switch { TextDirection.rtl => StandardFabLocation._leftOffsetX(scaffoldGeometry, adjustment), TextDirection.ltr => StandardFabLocation._rightOffsetX(scaffoldGeometry, adjustment), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double getOffsetY(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        if ((scaffoldGeometry.contentTop > scaffoldGeometry.minViewPadding.top))
        {
            double fabHalfHeight__26280 = (scaffoldGeometry.floatingActionButtonSize.height / 2.0);
            return (scaffoldGeometry.contentTop - fabHalfHeight__26280);
        }
        return scaffoldGeometry.minViewPadding.top;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _StartFloatFabLocation__floating_action_button_location : StandardFabLocation, FabStartOffsetX, FabFloatOffsetY
{

    internal _StartFloatFabLocation__floating_action_button_location()
    {
    }

    public override string ToString() => "FloatingActionButtonLocation.startFloat";
    public override double getOffsetX(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        return (((TextDirection)((dynamic)scaffoldGeometry).textDirection) switch { TextDirection.rtl => StandardFabLocation._rightOffsetX(scaffoldGeometry, adjustment), TextDirection.ltr => StandardFabLocation._leftOffsetX(scaffoldGeometry, adjustment), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double getOffsetY(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        double contentBottom__26920 = scaffoldGeometry.contentBottom;
        double bottomContentHeight__26985 = (scaffoldGeometry.scaffoldSize.height - contentBottom__26920);
        double bottomSheetHeight__27078 = scaffoldGeometry.bottomSheetSize.height;
        double fabHeight__27156 = scaffoldGeometry.floatingActionButtonSize.height;
        double snackBarHeight__27235 = scaffoldGeometry.snackBarSize.height;
        double safeMargin__27307 = Math.Max(Floating_action_button_locationLibrary.kFloatingActionButtonMargin, ((scaffoldGeometry.minViewPadding.bottom - bottomContentHeight__26985) + Floating_action_button_locationLibrary.kFloatingActionButtonMargin));
        double fabY__27482 = ((contentBottom__26920 - fabHeight__27156) - safeMargin__27307);
        if ((snackBarHeight__27235 > 0.0))
        {
            fabY__27482 = Math.Min(fabY__27482, (((contentBottom__26920 - snackBarHeight__27235) - fabHeight__27156) - Floating_action_button_locationLibrary.kFloatingActionButtonMargin));
        }
        if ((bottomSheetHeight__27078 > 0.0))
        {
            fabY__27482 = Math.Min(fabY__27482, ((contentBottom__26920 - bottomSheetHeight__27078) - (fabHeight__27156 / 2.0)));
        }
        return (fabY__27482 + adjustment);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _MiniStartFloatFabLocation__floating_action_button_location : StandardFabLocation, FabMiniOffsetAdjustment, FabStartOffsetX, FabFloatOffsetY
{

    internal _MiniStartFloatFabLocation__floating_action_button_location()
    {
    }

    public override string ToString() => "FloatingActionButtonLocation.miniStartFloat";
    public override bool isMini() => true;
    public override double getOffsetX(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        return (((TextDirection)((dynamic)scaffoldGeometry).textDirection) switch { TextDirection.rtl => StandardFabLocation._rightOffsetX(scaffoldGeometry, adjustment), TextDirection.ltr => StandardFabLocation._leftOffsetX(scaffoldGeometry, adjustment), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double getOffsetY(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        double contentBottom__26920 = scaffoldGeometry.contentBottom;
        double bottomContentHeight__26985 = (scaffoldGeometry.scaffoldSize.height - contentBottom__26920);
        double bottomSheetHeight__27078 = scaffoldGeometry.bottomSheetSize.height;
        double fabHeight__27156 = scaffoldGeometry.floatingActionButtonSize.height;
        double snackBarHeight__27235 = scaffoldGeometry.snackBarSize.height;
        double safeMargin__27307 = Math.Max(Floating_action_button_locationLibrary.kFloatingActionButtonMargin, ((scaffoldGeometry.minViewPadding.bottom - bottomContentHeight__26985) + Floating_action_button_locationLibrary.kFloatingActionButtonMargin));
        double fabY__27482 = ((contentBottom__26920 - fabHeight__27156) - safeMargin__27307);
        if ((snackBarHeight__27235 > 0.0))
        {
            fabY__27482 = Math.Min(fabY__27482, (((contentBottom__26920 - snackBarHeight__27235) - fabHeight__27156) - Floating_action_button_locationLibrary.kFloatingActionButtonMargin));
        }
        if ((bottomSheetHeight__27078 > 0.0))
        {
            fabY__27482 = Math.Min(fabY__27482, ((contentBottom__26920 - bottomSheetHeight__27078) - (fabHeight__27156 / 2.0)));
        }
        return (fabY__27482 + adjustment);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _CenterFloatFabLocation__floating_action_button_location : StandardFabLocation, FabCenterOffsetX, FabFloatOffsetY
{

    internal _CenterFloatFabLocation__floating_action_button_location()
    {
    }

    public override string ToString() => "FloatingActionButtonLocation.centerFloat";
    public override double getOffsetX(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        return (((scaffoldGeometry.scaffoldSize.width - scaffoldGeometry.floatingActionButtonSize.width)) / 2.0);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double getOffsetY(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        double contentBottom__26920 = scaffoldGeometry.contentBottom;
        double bottomContentHeight__26985 = (scaffoldGeometry.scaffoldSize.height - contentBottom__26920);
        double bottomSheetHeight__27078 = scaffoldGeometry.bottomSheetSize.height;
        double fabHeight__27156 = scaffoldGeometry.floatingActionButtonSize.height;
        double snackBarHeight__27235 = scaffoldGeometry.snackBarSize.height;
        double safeMargin__27307 = Math.Max(Floating_action_button_locationLibrary.kFloatingActionButtonMargin, ((scaffoldGeometry.minViewPadding.bottom - bottomContentHeight__26985) + Floating_action_button_locationLibrary.kFloatingActionButtonMargin));
        double fabY__27482 = ((contentBottom__26920 - fabHeight__27156) - safeMargin__27307);
        if ((snackBarHeight__27235 > 0.0))
        {
            fabY__27482 = Math.Min(fabY__27482, (((contentBottom__26920 - snackBarHeight__27235) - fabHeight__27156) - Floating_action_button_locationLibrary.kFloatingActionButtonMargin));
        }
        if ((bottomSheetHeight__27078 > 0.0))
        {
            fabY__27482 = Math.Min(fabY__27482, ((contentBottom__26920 - bottomSheetHeight__27078) - (fabHeight__27156 / 2.0)));
        }
        return (fabY__27482 + adjustment);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _MiniCenterFloatFabLocation__floating_action_button_location : StandardFabLocation, FabMiniOffsetAdjustment, FabCenterOffsetX, FabFloatOffsetY
{

    internal _MiniCenterFloatFabLocation__floating_action_button_location()
    {
    }

    public override string ToString() => "FloatingActionButtonLocation.miniCenterFloat";
    public override bool isMini() => true;
    public override double getOffsetX(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        return (((scaffoldGeometry.scaffoldSize.width - scaffoldGeometry.floatingActionButtonSize.width)) / 2.0);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double getOffsetY(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        double contentBottom__26920 = scaffoldGeometry.contentBottom;
        double bottomContentHeight__26985 = (scaffoldGeometry.scaffoldSize.height - contentBottom__26920);
        double bottomSheetHeight__27078 = scaffoldGeometry.bottomSheetSize.height;
        double fabHeight__27156 = scaffoldGeometry.floatingActionButtonSize.height;
        double snackBarHeight__27235 = scaffoldGeometry.snackBarSize.height;
        double safeMargin__27307 = Math.Max(Floating_action_button_locationLibrary.kFloatingActionButtonMargin, ((scaffoldGeometry.minViewPadding.bottom - bottomContentHeight__26985) + Floating_action_button_locationLibrary.kFloatingActionButtonMargin));
        double fabY__27482 = ((contentBottom__26920 - fabHeight__27156) - safeMargin__27307);
        if ((snackBarHeight__27235 > 0.0))
        {
            fabY__27482 = Math.Min(fabY__27482, (((contentBottom__26920 - snackBarHeight__27235) - fabHeight__27156) - Floating_action_button_locationLibrary.kFloatingActionButtonMargin));
        }
        if ((bottomSheetHeight__27078 > 0.0))
        {
            fabY__27482 = Math.Min(fabY__27482, ((contentBottom__26920 - bottomSheetHeight__27078) - (fabHeight__27156 / 2.0)));
        }
        return (fabY__27482 + adjustment);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _EndFloatFabLocation__floating_action_button_location : StandardFabLocation, FabEndOffsetX, FabFloatOffsetY
{

    internal _EndFloatFabLocation__floating_action_button_location()
    {
    }

    public override string ToString() => "FloatingActionButtonLocation.endFloat";
    public override double getOffsetX(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        return (((TextDirection)((dynamic)scaffoldGeometry).textDirection) switch { TextDirection.rtl => StandardFabLocation._leftOffsetX(scaffoldGeometry, adjustment), TextDirection.ltr => StandardFabLocation._rightOffsetX(scaffoldGeometry, adjustment), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double getOffsetY(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        double contentBottom__26920 = scaffoldGeometry.contentBottom;
        double bottomContentHeight__26985 = (scaffoldGeometry.scaffoldSize.height - contentBottom__26920);
        double bottomSheetHeight__27078 = scaffoldGeometry.bottomSheetSize.height;
        double fabHeight__27156 = scaffoldGeometry.floatingActionButtonSize.height;
        double snackBarHeight__27235 = scaffoldGeometry.snackBarSize.height;
        double safeMargin__27307 = Math.Max(Floating_action_button_locationLibrary.kFloatingActionButtonMargin, ((scaffoldGeometry.minViewPadding.bottom - bottomContentHeight__26985) + Floating_action_button_locationLibrary.kFloatingActionButtonMargin));
        double fabY__27482 = ((contentBottom__26920 - fabHeight__27156) - safeMargin__27307);
        if ((snackBarHeight__27235 > 0.0))
        {
            fabY__27482 = Math.Min(fabY__27482, (((contentBottom__26920 - snackBarHeight__27235) - fabHeight__27156) - Floating_action_button_locationLibrary.kFloatingActionButtonMargin));
        }
        if ((bottomSheetHeight__27078 > 0.0))
        {
            fabY__27482 = Math.Min(fabY__27482, ((contentBottom__26920 - bottomSheetHeight__27078) - (fabHeight__27156 / 2.0)));
        }
        return (fabY__27482 + adjustment);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _MiniEndFloatFabLocation__floating_action_button_location : StandardFabLocation, FabMiniOffsetAdjustment, FabEndOffsetX, FabFloatOffsetY
{

    internal _MiniEndFloatFabLocation__floating_action_button_location()
    {
    }

    public override string ToString() => "FloatingActionButtonLocation.miniEndFloat";
    public override bool isMini() => true;
    public override double getOffsetX(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        return (((TextDirection)((dynamic)scaffoldGeometry).textDirection) switch { TextDirection.rtl => StandardFabLocation._leftOffsetX(scaffoldGeometry, adjustment), TextDirection.ltr => StandardFabLocation._rightOffsetX(scaffoldGeometry, adjustment), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double getOffsetY(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        double contentBottom__26920 = scaffoldGeometry.contentBottom;
        double bottomContentHeight__26985 = (scaffoldGeometry.scaffoldSize.height - contentBottom__26920);
        double bottomSheetHeight__27078 = scaffoldGeometry.bottomSheetSize.height;
        double fabHeight__27156 = scaffoldGeometry.floatingActionButtonSize.height;
        double snackBarHeight__27235 = scaffoldGeometry.snackBarSize.height;
        double safeMargin__27307 = Math.Max(Floating_action_button_locationLibrary.kFloatingActionButtonMargin, ((scaffoldGeometry.minViewPadding.bottom - bottomContentHeight__26985) + Floating_action_button_locationLibrary.kFloatingActionButtonMargin));
        double fabY__27482 = ((contentBottom__26920 - fabHeight__27156) - safeMargin__27307);
        if ((snackBarHeight__27235 > 0.0))
        {
            fabY__27482 = Math.Min(fabY__27482, (((contentBottom__26920 - snackBarHeight__27235) - fabHeight__27156) - Floating_action_button_locationLibrary.kFloatingActionButtonMargin));
        }
        if ((bottomSheetHeight__27078 > 0.0))
        {
            fabY__27482 = Math.Min(fabY__27482, ((contentBottom__26920 - bottomSheetHeight__27078) - (fabHeight__27156 / 2.0)));
        }
        return (fabY__27482 + adjustment);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _StartDockedFabLocation__floating_action_button_location : StandardFabLocation, FabStartOffsetX, FabDockedOffsetY
{

    internal _StartDockedFabLocation__floating_action_button_location()
    {
    }

    public override string ToString() => "FloatingActionButtonLocation.startDocked";
    public override double getOffsetX(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        return (((TextDirection)((dynamic)scaffoldGeometry).textDirection) switch { TextDirection.rtl => StandardFabLocation._rightOffsetX(scaffoldGeometry, adjustment), TextDirection.ltr => StandardFabLocation._leftOffsetX(scaffoldGeometry, adjustment), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double getOffsetY(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        double contentBottom__28349 = scaffoldGeometry.contentBottom;
        double contentMargin__28414 = (scaffoldGeometry.scaffoldSize.height - contentBottom__28349);
        double bottomViewPadding__28501 = scaffoldGeometry.minViewPadding.bottom;
        double bottomSheetHeight__28578 = scaffoldGeometry.bottomSheetSize.height;
        double fabHeight__28656 = scaffoldGeometry.floatingActionButtonSize.height;
        double snackBarHeight__28735 = scaffoldGeometry.snackBarSize.height;
        double bottomMinInset__28807 = scaffoldGeometry.minInsets.bottom;
        double safeMargin__28871 = default!;
        if ((contentMargin__28414 > (bottomMinInset__28807 + (fabHeight__28656 / 2.0))))
        {
            safeMargin__28871 = 0.0;
        }
        else
        {
            if ((bottomMinInset__28807 == 0.0))
            {
                safeMargin__28871 = bottomViewPadding__28501;
            }
            else
            {
                safeMargin__28871 = ((fabHeight__28656 / 2.0) + Floating_action_button_locationLibrary.kFloatingActionButtonMargin);
            }
        }
        double fabY__29506 = ((contentBottom__28349 - (fabHeight__28656 / 2.0)) - safeMargin__28871);
        if ((snackBarHeight__28735 > 0.0))
        {
            fabY__29506 = Math.Min(fabY__29506, (((contentBottom__28349 - snackBarHeight__28735) - fabHeight__28656) - Floating_action_button_locationLibrary.kFloatingActionButtonMargin));
        }
        if ((bottomSheetHeight__28578 > 0.0))
        {
            fabY__29506 = Math.Min(fabY__29506, ((contentBottom__28349 - bottomSheetHeight__28578) - (fabHeight__28656 / 2.0)));
        }
        double maxFabY__30018 = ((scaffoldGeometry.scaffoldSize.height - fabHeight__28656) - safeMargin__28871);
        return Math.Min(maxFabY__30018, fabY__29506);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _MiniStartDockedFabLocation__floating_action_button_location : StandardFabLocation, FabMiniOffsetAdjustment, FabStartOffsetX, FabDockedOffsetY
{

    internal _MiniStartDockedFabLocation__floating_action_button_location()
    {
    }

    public override string ToString() => "FloatingActionButtonLocation.miniStartDocked";
    public override bool isMini() => true;
    public override double getOffsetX(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        return (((TextDirection)((dynamic)scaffoldGeometry).textDirection) switch { TextDirection.rtl => StandardFabLocation._rightOffsetX(scaffoldGeometry, adjustment), TextDirection.ltr => StandardFabLocation._leftOffsetX(scaffoldGeometry, adjustment), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double getOffsetY(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        double contentBottom__28349 = scaffoldGeometry.contentBottom;
        double contentMargin__28414 = (scaffoldGeometry.scaffoldSize.height - contentBottom__28349);
        double bottomViewPadding__28501 = scaffoldGeometry.minViewPadding.bottom;
        double bottomSheetHeight__28578 = scaffoldGeometry.bottomSheetSize.height;
        double fabHeight__28656 = scaffoldGeometry.floatingActionButtonSize.height;
        double snackBarHeight__28735 = scaffoldGeometry.snackBarSize.height;
        double bottomMinInset__28807 = scaffoldGeometry.minInsets.bottom;
        double safeMargin__28871 = default!;
        if ((contentMargin__28414 > (bottomMinInset__28807 + (fabHeight__28656 / 2.0))))
        {
            safeMargin__28871 = 0.0;
        }
        else
        {
            if ((bottomMinInset__28807 == 0.0))
            {
                safeMargin__28871 = bottomViewPadding__28501;
            }
            else
            {
                safeMargin__28871 = ((fabHeight__28656 / 2.0) + Floating_action_button_locationLibrary.kFloatingActionButtonMargin);
            }
        }
        double fabY__29506 = ((contentBottom__28349 - (fabHeight__28656 / 2.0)) - safeMargin__28871);
        if ((snackBarHeight__28735 > 0.0))
        {
            fabY__29506 = Math.Min(fabY__29506, (((contentBottom__28349 - snackBarHeight__28735) - fabHeight__28656) - Floating_action_button_locationLibrary.kFloatingActionButtonMargin));
        }
        if ((bottomSheetHeight__28578 > 0.0))
        {
            fabY__29506 = Math.Min(fabY__29506, ((contentBottom__28349 - bottomSheetHeight__28578) - (fabHeight__28656 / 2.0)));
        }
        double maxFabY__30018 = ((scaffoldGeometry.scaffoldSize.height - fabHeight__28656) - safeMargin__28871);
        return Math.Min(maxFabY__30018, fabY__29506);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _CenterDockedFabLocation__floating_action_button_location : StandardFabLocation, FabCenterOffsetX, FabDockedOffsetY
{

    internal _CenterDockedFabLocation__floating_action_button_location()
    {
    }

    public override string ToString() => "FloatingActionButtonLocation.centerDocked";
    public override double getOffsetX(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        return (((scaffoldGeometry.scaffoldSize.width - scaffoldGeometry.floatingActionButtonSize.width)) / 2.0);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double getOffsetY(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        double contentBottom__28349 = scaffoldGeometry.contentBottom;
        double contentMargin__28414 = (scaffoldGeometry.scaffoldSize.height - contentBottom__28349);
        double bottomViewPadding__28501 = scaffoldGeometry.minViewPadding.bottom;
        double bottomSheetHeight__28578 = scaffoldGeometry.bottomSheetSize.height;
        double fabHeight__28656 = scaffoldGeometry.floatingActionButtonSize.height;
        double snackBarHeight__28735 = scaffoldGeometry.snackBarSize.height;
        double bottomMinInset__28807 = scaffoldGeometry.minInsets.bottom;
        double safeMargin__28871 = default!;
        if ((contentMargin__28414 > (bottomMinInset__28807 + (fabHeight__28656 / 2.0))))
        {
            safeMargin__28871 = 0.0;
        }
        else
        {
            if ((bottomMinInset__28807 == 0.0))
            {
                safeMargin__28871 = bottomViewPadding__28501;
            }
            else
            {
                safeMargin__28871 = ((fabHeight__28656 / 2.0) + Floating_action_button_locationLibrary.kFloatingActionButtonMargin);
            }
        }
        double fabY__29506 = ((contentBottom__28349 - (fabHeight__28656 / 2.0)) - safeMargin__28871);
        if ((snackBarHeight__28735 > 0.0))
        {
            fabY__29506 = Math.Min(fabY__29506, (((contentBottom__28349 - snackBarHeight__28735) - fabHeight__28656) - Floating_action_button_locationLibrary.kFloatingActionButtonMargin));
        }
        if ((bottomSheetHeight__28578 > 0.0))
        {
            fabY__29506 = Math.Min(fabY__29506, ((contentBottom__28349 - bottomSheetHeight__28578) - (fabHeight__28656 / 2.0)));
        }
        double maxFabY__30018 = ((scaffoldGeometry.scaffoldSize.height - fabHeight__28656) - safeMargin__28871);
        return Math.Min(maxFabY__30018, fabY__29506);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _MiniCenterDockedFabLocation__floating_action_button_location : StandardFabLocation, FabMiniOffsetAdjustment, FabCenterOffsetX, FabDockedOffsetY
{

    internal _MiniCenterDockedFabLocation__floating_action_button_location()
    {
    }

    public override string ToString() => "FloatingActionButtonLocation.miniCenterDocked";
    public override bool isMini() => true;
    public override double getOffsetX(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        return (((scaffoldGeometry.scaffoldSize.width - scaffoldGeometry.floatingActionButtonSize.width)) / 2.0);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double getOffsetY(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        double contentBottom__28349 = scaffoldGeometry.contentBottom;
        double contentMargin__28414 = (scaffoldGeometry.scaffoldSize.height - contentBottom__28349);
        double bottomViewPadding__28501 = scaffoldGeometry.minViewPadding.bottom;
        double bottomSheetHeight__28578 = scaffoldGeometry.bottomSheetSize.height;
        double fabHeight__28656 = scaffoldGeometry.floatingActionButtonSize.height;
        double snackBarHeight__28735 = scaffoldGeometry.snackBarSize.height;
        double bottomMinInset__28807 = scaffoldGeometry.minInsets.bottom;
        double safeMargin__28871 = default!;
        if ((contentMargin__28414 > (bottomMinInset__28807 + (fabHeight__28656 / 2.0))))
        {
            safeMargin__28871 = 0.0;
        }
        else
        {
            if ((bottomMinInset__28807 == 0.0))
            {
                safeMargin__28871 = bottomViewPadding__28501;
            }
            else
            {
                safeMargin__28871 = ((fabHeight__28656 / 2.0) + Floating_action_button_locationLibrary.kFloatingActionButtonMargin);
            }
        }
        double fabY__29506 = ((contentBottom__28349 - (fabHeight__28656 / 2.0)) - safeMargin__28871);
        if ((snackBarHeight__28735 > 0.0))
        {
            fabY__29506 = Math.Min(fabY__29506, (((contentBottom__28349 - snackBarHeight__28735) - fabHeight__28656) - Floating_action_button_locationLibrary.kFloatingActionButtonMargin));
        }
        if ((bottomSheetHeight__28578 > 0.0))
        {
            fabY__29506 = Math.Min(fabY__29506, ((contentBottom__28349 - bottomSheetHeight__28578) - (fabHeight__28656 / 2.0)));
        }
        double maxFabY__30018 = ((scaffoldGeometry.scaffoldSize.height - fabHeight__28656) - safeMargin__28871);
        return Math.Min(maxFabY__30018, fabY__29506);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _EndDockedFabLocation__floating_action_button_location : StandardFabLocation, FabEndOffsetX, FabDockedOffsetY
{

    internal _EndDockedFabLocation__floating_action_button_location()
    {
    }

    public override string ToString() => "FloatingActionButtonLocation.endDocked";
    public override double getOffsetX(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        return (((TextDirection)((dynamic)scaffoldGeometry).textDirection) switch { TextDirection.rtl => StandardFabLocation._leftOffsetX(scaffoldGeometry, adjustment), TextDirection.ltr => StandardFabLocation._rightOffsetX(scaffoldGeometry, adjustment), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double getOffsetY(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        double contentBottom__28349 = scaffoldGeometry.contentBottom;
        double contentMargin__28414 = (scaffoldGeometry.scaffoldSize.height - contentBottom__28349);
        double bottomViewPadding__28501 = scaffoldGeometry.minViewPadding.bottom;
        double bottomSheetHeight__28578 = scaffoldGeometry.bottomSheetSize.height;
        double fabHeight__28656 = scaffoldGeometry.floatingActionButtonSize.height;
        double snackBarHeight__28735 = scaffoldGeometry.snackBarSize.height;
        double bottomMinInset__28807 = scaffoldGeometry.minInsets.bottom;
        double safeMargin__28871 = default!;
        if ((contentMargin__28414 > (bottomMinInset__28807 + (fabHeight__28656 / 2.0))))
        {
            safeMargin__28871 = 0.0;
        }
        else
        {
            if ((bottomMinInset__28807 == 0.0))
            {
                safeMargin__28871 = bottomViewPadding__28501;
            }
            else
            {
                safeMargin__28871 = ((fabHeight__28656 / 2.0) + Floating_action_button_locationLibrary.kFloatingActionButtonMargin);
            }
        }
        double fabY__29506 = ((contentBottom__28349 - (fabHeight__28656 / 2.0)) - safeMargin__28871);
        if ((snackBarHeight__28735 > 0.0))
        {
            fabY__29506 = Math.Min(fabY__29506, (((contentBottom__28349 - snackBarHeight__28735) - fabHeight__28656) - Floating_action_button_locationLibrary.kFloatingActionButtonMargin));
        }
        if ((bottomSheetHeight__28578 > 0.0))
        {
            fabY__29506 = Math.Min(fabY__29506, ((contentBottom__28349 - bottomSheetHeight__28578) - (fabHeight__28656 / 2.0)));
        }
        double maxFabY__30018 = ((scaffoldGeometry.scaffoldSize.height - fabHeight__28656) - safeMargin__28871);
        return Math.Min(maxFabY__30018, fabY__29506);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _MiniEndDockedFabLocation__floating_action_button_location : StandardFabLocation, FabMiniOffsetAdjustment, FabEndOffsetX, FabDockedOffsetY
{

    internal _MiniEndDockedFabLocation__floating_action_button_location()
    {
    }

    public override string ToString() => "FloatingActionButtonLocation.miniEndDocked";
    public override bool isMini() => true;
    public override double getOffsetX(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        return (((TextDirection)((dynamic)scaffoldGeometry).textDirection) switch { TextDirection.rtl => StandardFabLocation._leftOffsetX(scaffoldGeometry, adjustment), TextDirection.ltr => StandardFabLocation._rightOffsetX(scaffoldGeometry, adjustment), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double getOffsetY(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        double contentBottom__28349 = scaffoldGeometry.contentBottom;
        double contentMargin__28414 = (scaffoldGeometry.scaffoldSize.height - contentBottom__28349);
        double bottomViewPadding__28501 = scaffoldGeometry.minViewPadding.bottom;
        double bottomSheetHeight__28578 = scaffoldGeometry.bottomSheetSize.height;
        double fabHeight__28656 = scaffoldGeometry.floatingActionButtonSize.height;
        double snackBarHeight__28735 = scaffoldGeometry.snackBarSize.height;
        double bottomMinInset__28807 = scaffoldGeometry.minInsets.bottom;
        double safeMargin__28871 = default!;
        if ((contentMargin__28414 > (bottomMinInset__28807 + (fabHeight__28656 / 2.0))))
        {
            safeMargin__28871 = 0.0;
        }
        else
        {
            if ((bottomMinInset__28807 == 0.0))
            {
                safeMargin__28871 = bottomViewPadding__28501;
            }
            else
            {
                safeMargin__28871 = ((fabHeight__28656 / 2.0) + Floating_action_button_locationLibrary.kFloatingActionButtonMargin);
            }
        }
        double fabY__29506 = ((contentBottom__28349 - (fabHeight__28656 / 2.0)) - safeMargin__28871);
        if ((snackBarHeight__28735 > 0.0))
        {
            fabY__29506 = Math.Min(fabY__29506, (((contentBottom__28349 - snackBarHeight__28735) - fabHeight__28656) - Floating_action_button_locationLibrary.kFloatingActionButtonMargin));
        }
        if ((bottomSheetHeight__28578 > 0.0))
        {
            fabY__29506 = Math.Min(fabY__29506, ((contentBottom__28349 - bottomSheetHeight__28578) - (fabHeight__28656 / 2.0)));
        }
        double maxFabY__30018 = ((scaffoldGeometry.scaffoldSize.height - fabHeight__28656) - safeMargin__28871);
        return Math.Min(maxFabY__30018, fabY__29506);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _EndContainedFabLocation__floating_action_button_location : StandardFabLocation, FabEndOffsetX, FabContainedOffsetY
{

    internal _EndContainedFabLocation__floating_action_button_location()
    {
    }

    public override string ToString() => "FloatingActionButtonLocation.endContained";
    public override double getOffsetX(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        return (((TextDirection)((dynamic)scaffoldGeometry).textDirection) switch { TextDirection.rtl => StandardFabLocation._leftOffsetX(scaffoldGeometry, adjustment), TextDirection.ltr => StandardFabLocation._rightOffsetX(scaffoldGeometry, adjustment), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double getOffsetY(ScaffoldPrelayoutGeometry scaffoldGeometry, double adjustment)
    {
        double contentBottom__30640 = scaffoldGeometry.contentBottom;
        double contentMargin__30705 = (scaffoldGeometry.scaffoldSize.height - contentBottom__30640);
        double bottomViewPadding__30792 = scaffoldGeometry.minViewPadding.bottom;
        double fabHeight__30869 = scaffoldGeometry.floatingActionButtonSize.height;
        double safeMargin__30943 = default!;
        if ((contentMargin__30705 > (bottomViewPadding__30792 + fabHeight__30869)))
        {
            safeMargin__30943 = 0.0;
        }
        else
        {
            safeMargin__30943 = bottomViewPadding__30792;
        }
        double contentBottomToFabTop__31426 = ((((contentMargin__30705 - bottomViewPadding__30792) - fabHeight__30869)) / 2.0);
        double fabY__31522 = (contentBottom__30640 + contentBottomToFabTop__31426);
        double maxFabY__31585 = ((scaffoldGeometry.scaffoldSize.height - fabHeight__30869) - safeMargin__30943);
        return Math.Min(maxFabY__31585, fabY__31522);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public abstract class FloatingActionButtonAnimator
{
    public static FloatingActionButtonAnimator scaling = ((FloatingActionButtonAnimator)(object?)new _ScalingFabMotionAnimator__floating_action_button_location());
    public static FloatingActionButtonAnimator noAnimation = ((FloatingActionButtonAnimator)(object?)new _NoAnimationFabMotionAnimator__floating_action_button_location());

    protected FloatingActionButtonAnimator()
    {
    }

    public abstract global::Doroti.Ui.Offset getOffset(Offset begin, Offset end, double progress);
    public abstract global::Doroti.Framework.Animation.Animation<double> getScaleAnimation(global::Doroti.Framework.Animation.Animation<double> parent);
    public abstract global::Doroti.Framework.Animation.Animation<double> getRotationAnimation(global::Doroti.Framework.Animation.Animation<double> parent);
    public virtual double getAnimationRestart(double previousValue) => 0.0;
    public override string ToString() => global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "FloatingActionButtonAnimator");
}

internal class _ScalingFabMotionAnimator__floating_action_button_location : FloatingActionButtonAnimator
{
    internal static global::Doroti.Framework.Animation.Animatable<double> _rotationTween = ((global::Doroti.Framework.Animation.Animatable<double>)(object?)new global::Doroti.Framework.Animation.Tween<double>(begin: (1.0 - (Floating_action_button_locationLibrary.kFloatingActionButtonTurnInterval * 2.0)), end: 1.0));
    internal static global::Doroti.Framework.Animation.Animatable<double> _thresholdCenterTween = ((global::Doroti.Framework.Animation.Animatable<double>)(object?)new global::Doroti.Framework.Animation.CurveTween(curve: new global::Doroti.Framework.Animation.Threshold(0.5)));

    internal _ScalingFabMotionAnimator__floating_action_button_location()
    {
    }

    public override Offset getOffset(Offset begin, Offset end, double progress)
    {
        if ((progress < 0.5))
        {
            return begin;
        }
        else
        {
            return end;
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Animation.Animation<double> getScaleAnimation(global::Doroti.Framework.Animation.Animation<double> parent)
    {
        global::Doroti.Framework.Animation.Curve curve__43656 = ((global::Doroti.Framework.Animation.Curve)(object?)new global::Doroti.Framework.Animation.Interval(0.5, 1.0, curve: global::Doroti.Framework.Animation.Curves.ease));
        return ((global::Doroti.Framework.Animation.Animation<double>)(object?)new _AnimationSwap__floating_action_button_location<double>(new global::Doroti.Framework.Animation.ReverseAnimation(parent.drive(new global::Doroti.Framework.Animation.CurveTween(curve: ((global::Doroti.Framework.Animation.Curve)curve__43656).flipped))), parent.drive(new global::Doroti.Framework.Animation.CurveTween(curve: curve__43656)), parent, 0.5));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Animation.Animation<double> getRotationAnimation(global::Doroti.Framework.Animation.Animation<double> parent)
    {
        return ((global::Doroti.Framework.Animation.Animation<double>)(object?)new _AnimationSwap__floating_action_button_location<double>(parent.drive(_rotationTween), new global::Doroti.Framework.Animation.ReverseAnimation(parent.drive(_thresholdCenterTween)), parent, 0.5));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double getAnimationRestart(double previousValue) => Math.Min((1.0 - previousValue), previousValue);
}

internal class _NoAnimationFabMotionAnimator__floating_action_button_location : FloatingActionButtonAnimator
{
    internal _NoAnimationFabMotionAnimator__floating_action_button_location()
    {
    }

    public override Offset getOffset(Offset begin, Offset end, double progress)
    {
        return end;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Animation.Animation<double> getRotationAnimation(global::Doroti.Framework.Animation.Animation<double> parent)
    {
        return ((global::Doroti.Framework.Animation.Animation<double>)(object?)new global::Doroti.Framework.Animation.AlwaysStoppedAnimation<double>(1.0));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Animation.Animation<double> getScaleAnimation(global::Doroti.Framework.Animation.Animation<double> parent)
    {
        return ((global::Doroti.Framework.Animation.Animation<double>)(object?)new global::Doroti.Framework.Animation.AlwaysStoppedAnimation<double>(1.0));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _AnimationSwap__floating_action_button_location<T> : global::Doroti.Framework.Animation.CompoundAnimation<T>
{
    public virtual global::Doroti.Framework.Animation.Animation<double> parent { get; private set; } = default!;
    public virtual double swapThreshold { get; private set; } = default!;

    internal _AnimationSwap__floating_action_button_location(global::Doroti.Framework.Animation.Animation<T> first, global::Doroti.Framework.Animation.Animation<T> next, global::Doroti.Framework.Animation.Animation<double> parent, double swapThreshold) : base(first: first, next: next)
    {
        this.parent = parent;
        this.swapThreshold = swapThreshold;
    }

    public override T value => ((((global::Doroti.Framework.Animation.Animation<double>)this.parent).value < this.swapThreshold) ? ((global::Doroti.Framework.Animation.Animation<T>)this.first).value : ((global::Doroti.Framework.Animation.Animation<T>)this.next).value);
}
