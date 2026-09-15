// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/gestures/constants.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Gestures;

public static partial class ConstantsLibrary
{
    public static Duration kPressTimeout = Duration.Create(milliseconds: 100L);
}

public static partial class ConstantsLibrary
{
    public static Duration kHoverTapTimeout = Duration.Create(milliseconds: 150L);
}

public static partial class ConstantsLibrary
{
    public static double kHoverTapSlop = 20.0;
}

public static partial class ConstantsLibrary
{
    public static Duration kLongPressTimeout = Duration.Create(milliseconds: 500L);
}

public static partial class ConstantsLibrary
{
    public static Duration kDoubleTapTimeout = Duration.Create(milliseconds: 300L);
}

public static partial class ConstantsLibrary
{
    public static Duration kDoubleTapMinTime = Duration.Create(milliseconds: 40L);
}

public static partial class ConstantsLibrary
{
    public static double kDoubleTapTouchSlop = ConstantsLibrary.kTouchSlop;
}

public static partial class ConstantsLibrary
{
    public static double kDoubleTapSlop = 100.0;
}

public static partial class ConstantsLibrary
{
    public static Duration kZoomControlsTimeout = Duration.Create(milliseconds: 3000L);
}

public static partial class ConstantsLibrary
{
    public static double kTouchSlop = 18.0;
}

public static partial class ConstantsLibrary
{
    public static double kPagingTouchSlop = (ConstantsLibrary.kTouchSlop * 2.0);
}

public static partial class ConstantsLibrary
{
    public static double kPanSlop = (ConstantsLibrary.kTouchSlop * 2.0);
}

public static partial class ConstantsLibrary
{
    public static double kScaleSlop = ConstantsLibrary.kTouchSlop;
}

public static partial class ConstantsLibrary
{
    public static double kWindowTouchSlop = 16.0;
}

public static partial class ConstantsLibrary
{
    public static double kMinFlingVelocity = 50.0;
}

public static partial class ConstantsLibrary
{
    public static double kMaxFlingVelocity = 8000.0;
}

public static partial class ConstantsLibrary
{
    public static Duration kJumpTapTimeout = Duration.Create(milliseconds: 500L);
}

public static partial class ConstantsLibrary
{
    public static double kPrecisePointerHitSlop = 1.0;
}

public static partial class ConstantsLibrary
{
    public static double kPrecisePointerPanSlop = (ConstantsLibrary.kPrecisePointerHitSlop * 2.0);
}

public static partial class ConstantsLibrary
{
    public static double kPrecisePointerScaleSlop = ConstantsLibrary.kPrecisePointerHitSlop;
}

