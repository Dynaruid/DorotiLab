// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/basic_types.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622, CS8625, CS8714
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Painting;

public enum RenderComparison
{
    identical,
    metadata,
    paint,
    layout
}

public enum Axis
{
    horizontal,
    vertical
}

public static partial class Basic_typesLibrary
{
    public static Axis flipAxis(Axis direction)
    {
        return (direction switch { Axis.horizontal => Axis.vertical, Axis.vertical => Axis.horizontal, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public enum VerticalDirection
{
    up,
    down
}

public enum AxisDirection
{
    up,
    right,
    down,
    left
}

public static partial class Basic_typesLibrary
{
    public static Axis axisDirectionToAxis(AxisDirection axisDirection)
    {
        return (axisDirection switch { AxisDirection.up => Axis.vertical, AxisDirection.down => Axis.vertical, AxisDirection.left => Axis.horizontal, AxisDirection.right => Axis.horizontal, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Basic_typesLibrary
{
    public static AxisDirection textDirectionToAxisDirection(TextDirection textDirection)
    {
        return (textDirection switch { TextDirection.rtl => AxisDirection.left, TextDirection.ltr => AxisDirection.right, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Basic_typesLibrary
{
    public static AxisDirection flipAxisDirection(AxisDirection axisDirection)
    {
        return (axisDirection switch { AxisDirection.up => AxisDirection.down, AxisDirection.right => AxisDirection.left, AxisDirection.down => AxisDirection.up, AxisDirection.left => AxisDirection.right, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Basic_typesLibrary
{
    public static bool axisDirectionIsReversed(AxisDirection axisDirection)
    {
        return (axisDirection switch { AxisDirection.up => true, AxisDirection.left => true, AxisDirection.down => false, AxisDirection.right => false, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

