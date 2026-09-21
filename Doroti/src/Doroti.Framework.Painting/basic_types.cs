// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/basic_types.dart
using Doroti.Ui;

namespace Doroti.Framework.Painting;

public enum RenderComparison
{
    identical,
    metadata,
    paint,
    layout,
}

public enum Axis
{
    horizontal,
    vertical,
}

public static partial class Basic_typesLibrary
{
    public static Axis flipAxis(Axis direction)
    {
        return direction switch
        {
            Axis.horizontal => Axis.vertical,
            Axis.vertical => Axis.horizontal,
            _ => throw new InvalidOperationException(
                "Switch expression did not handle the supplied value."
            ),
        };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public enum VerticalDirection
{
    up,
    down,
}

public enum AxisDirection
{
    up,
    right,
    down,
    left,
}

public static partial class Basic_typesLibrary
{
    public static Axis axisDirectionToAxis(AxisDirection axisDirection)
    {
        return axisDirection switch
        {
            AxisDirection.up => Axis.vertical,
            AxisDirection.down => Axis.vertical,
            AxisDirection.left => Axis.horizontal,
            AxisDirection.right => Axis.horizontal,
            _ => throw new InvalidOperationException(
                "Switch expression did not handle the supplied value."
            ),
        };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public static partial class Basic_typesLibrary
{
    public static AxisDirection textDirectionToAxisDirection(TextDirection textDirection)
    {
        return textDirection switch
        {
            TextDirection.rtl => AxisDirection.left,
            TextDirection.ltr => AxisDirection.right,
            _ => throw new InvalidOperationException(
                "Switch expression did not handle the supplied value."
            ),
        };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public static partial class Basic_typesLibrary
{
    public static AxisDirection flipAxisDirection(AxisDirection axisDirection)
    {
        return axisDirection switch
        {
            AxisDirection.up => AxisDirection.down,
            AxisDirection.right => AxisDirection.left,
            AxisDirection.down => AxisDirection.up,
            AxisDirection.left => AxisDirection.right,
            _ => throw new InvalidOperationException(
                "Switch expression did not handle the supplied value."
            ),
        };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public static partial class Basic_typesLibrary
{
    public static bool axisDirectionIsReversed(AxisDirection axisDirection)
    {
        return axisDirection switch
        {
            AxisDirection.up => true,
            AxisDirection.left => true,
            AxisDirection.down => false,
            AxisDirection.right => false,
            _ => throw new InvalidOperationException(
                "Switch expression did not handle the supplied value."
            ),
        };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}
