// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/_window_positioner.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public enum WindowPositionerAnchorIo
{
    center,
    top,
    bottom,
    left,
    right,
    topLeft,
    bottomLeft,
    topRight,
    bottomRight,
}

public static class WindowPositionerAnchorIoMembers
{
    internal static WindowPositionerAnchorIo _flipX(this WindowPositionerAnchorIo value)
    {
        return value switch
        {
            WindowPositionerAnchorIo.center => WindowPositionerAnchorIo.center,
            WindowPositionerAnchorIo.top => WindowPositionerAnchorIo.top,
            WindowPositionerAnchorIo.bottom => WindowPositionerAnchorIo.bottom,
            WindowPositionerAnchorIo.left => WindowPositionerAnchorIo.right,
            WindowPositionerAnchorIo.right => WindowPositionerAnchorIo.left,
            WindowPositionerAnchorIo.topLeft => WindowPositionerAnchorIo.topRight,
            WindowPositionerAnchorIo.bottomLeft => WindowPositionerAnchorIo.bottomRight,
            WindowPositionerAnchorIo.topRight => WindowPositionerAnchorIo.topLeft,
            WindowPositionerAnchorIo.bottomRight => WindowPositionerAnchorIo.bottomLeft,
            _ => throw new InvalidOperationException(
                "Switch expression did not handle the supplied value."
            ),
        };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal static WindowPositionerAnchorIo _flipY(this WindowPositionerAnchorIo value)
    {
        return value switch
        {
            WindowPositionerAnchorIo.center => WindowPositionerAnchorIo.center,
            WindowPositionerAnchorIo.top => WindowPositionerAnchorIo.bottom,
            WindowPositionerAnchorIo.bottom => WindowPositionerAnchorIo.top,
            WindowPositionerAnchorIo.left => WindowPositionerAnchorIo.left,
            WindowPositionerAnchorIo.right => WindowPositionerAnchorIo.right,
            WindowPositionerAnchorIo.topLeft => WindowPositionerAnchorIo.bottomLeft,
            WindowPositionerAnchorIo.bottomLeft => WindowPositionerAnchorIo.topLeft,
            WindowPositionerAnchorIo.topRight => WindowPositionerAnchorIo.bottomRight,
            WindowPositionerAnchorIo.bottomRight => WindowPositionerAnchorIo.topRight,
            _ => throw new InvalidOperationException(
                "Switch expression did not handle the supplied value."
            ),
        };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal static Offset _offsetFor(this WindowPositionerAnchorIo value, Size size)
    {
        return value switch
        {
            WindowPositionerAnchorIo.center => new Offset(-size.width / 2.0, -size.height / 2.0),
            WindowPositionerAnchorIo.top => new Offset(-size.width / 2.0, 0.0),
            WindowPositionerAnchorIo.bottom => new Offset(-size.width / 2.0, -size.height),
            WindowPositionerAnchorIo.left => new Offset(0.0, -size.height / 2.0),
            WindowPositionerAnchorIo.right => new Offset(-size.width, -size.height / 2.0),
            WindowPositionerAnchorIo.topLeft => Offset.zero,
            WindowPositionerAnchorIo.bottomLeft => new Offset(0.0, -size.height),
            WindowPositionerAnchorIo.topRight => new Offset(-size.width, 0.0),
            WindowPositionerAnchorIo.bottomRight => new Offset(-size.width, -size.height),
            _ => throw new InvalidOperationException(
                "Switch expression did not handle the supplied value."
            ),
        };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal static Offset _anchorPositionFor(this WindowPositionerAnchorIo value, Rect rect)
    {
        return value switch
        {
            WindowPositionerAnchorIo.center => rect.center,
            WindowPositionerAnchorIo.top => rect.topCenter,
            WindowPositionerAnchorIo.bottom => rect.bottomCenter,
            WindowPositionerAnchorIo.left => rect.centerLeft,
            WindowPositionerAnchorIo.right => rect.centerRight,
            WindowPositionerAnchorIo.topLeft => rect.topLeft,
            WindowPositionerAnchorIo.bottomLeft => rect.bottomLeft,
            WindowPositionerAnchorIo.topRight => rect.topRight,
            WindowPositionerAnchorIo.bottomRight => rect.bottomRight,
            _ => throw new InvalidOperationException(
                "Switch expression did not handle the supplied value."
            ),
        };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class WindowPositionerConstraintAdjustmentIo
{
    public virtual bool slideX { get; private set; } = default!;
    public virtual bool slideY { get; private set; } = default!;
    public virtual bool flipX { get; private set; } = default!;
    public virtual bool flipY { get; private set; } = default!;
    public virtual bool resizeX { get; private set; } = default!;
    public virtual bool resizeY { get; private set; } = default!;

    public WindowPositionerConstraintAdjustmentIo(
        bool flipX = false,
        bool flipY = false,
        bool slideX = false,
        bool slideY = false,
        bool resizeX = false,
        bool resizeY = false
    )
    {
        this.flipX = flipX;
        this.flipY = flipY;
        this.slideX = slideX;
        this.slideY = slideY;
        this.resizeX = resizeX;
        this.resizeY = resizeY;
    }

    public override string ToString() =>
        $"WindowPositionerConstraintAdjustment(flipX: {flipX}, flipY: {flipY}, slideX: {slideX}, slideY: {slideY}, resizeX: {resizeX}, resizeY: {resizeY})";
}

public class WindowPositionerIo
{
    public virtual WindowPositionerAnchorIo parentAnchor { get; private set; } = default!;
    public virtual WindowPositionerAnchorIo childAnchor { get; private set; } = default!;
    public virtual Offset offset { get; private set; } = default!;
    public virtual WindowPositionerConstraintAdjustmentIo constraintAdjustment
    {
        get;
        private set;
    } = default!;

    public WindowPositionerIo(
        WindowPositionerAnchorIo parentAnchor = WindowPositionerAnchorIo.center,
        WindowPositionerAnchorIo childAnchor = WindowPositionerAnchorIo.center,
        Offset offset = default,
        WindowPositionerConstraintAdjustmentIo constraintAdjustment = default!
    )
    {
        WindowPositionerConstraintAdjustmentIo __constraintAdjustment =
            constraintAdjustment ?? new WindowPositionerConstraintAdjustmentIo();
        this.parentAnchor = parentAnchor;
        this.childAnchor = childAnchor;
        this.offset = offset;
        this.constraintAdjustment = __constraintAdjustment;
    }

    public virtual WindowPositionerIo copyWith(
        WindowPositionerAnchorIo? parentAnchor = null,
        WindowPositionerAnchorIo? childAnchor = null,
        Offset? offset = null,
        WindowPositionerConstraintAdjustmentIo? constraintAdjustment = null
    )
    {
        return new WindowPositionerIo(
            parentAnchor: parentAnchor ?? this.parentAnchor,
            childAnchor: childAnchor ?? this.childAnchor,
            offset: offset ?? this.offset,
            constraintAdjustment: constraintAdjustment ?? this.constraintAdjustment
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual Rect placeWindow(
        Size childSize,
        Rect anchorRect,
        Rect parentRect,
        Rect displayRect
    )
    {
        Rect defaultResult = default!;
        Offset result =
            _window_positionerLibrary._constrainTo(
                parentRect,
                parentAnchor._anchorPositionFor(anchorRect) + offset
            ) + childAnchor._offsetFor(childSize);
        defaultResult = result & childSize;
        if (_window_positionerLibrary._rectContains(displayRect, defaultResult))
        {
            return defaultResult;
        }
        if (constraintAdjustment.flipX)
        {
            Offset resultLocal =
                _window_positionerLibrary._constrainTo(
                    parentRect,
                    parentAnchor._flipX()._anchorPositionFor(anchorRect)
                        + _window_positionerLibrary._flipX((offset))
                ) + childAnchor._flipX()._offsetFor(childSize);
            if (_window_positionerLibrary._rectContains(displayRect, resultLocal & childSize))
            {
                return resultLocal & childSize;
            }
        }
        if (constraintAdjustment.flipY)
        {
            Offset resultAlternate =
                _window_positionerLibrary._constrainTo(
                    parentRect,
                    parentAnchor._flipY()._anchorPositionFor(anchorRect)
                        + _window_positionerLibrary._flipY((offset))
                ) + childAnchor._flipY()._offsetFor(childSize);
            if (_window_positionerLibrary._rectContains(displayRect, resultAlternate & childSize))
            {
                return resultAlternate & childSize;
            }
        }
        if (constraintAdjustment.flipX && constraintAdjustment.flipY)
        {
            Offset resultNested =
                _window_positionerLibrary._constrainTo(
                    parentRect,
                    parentAnchor._flipY()._flipX()._anchorPositionFor(anchorRect)
                        + _window_positionerLibrary._flipX(
                            _window_positionerLibrary._flipY((offset))
                        )
                ) + childAnchor._flipY()._flipX()._offsetFor(childSize);
            if (_window_positionerLibrary._rectContains(displayRect, resultNested & childSize))
            {
                return resultNested & childSize;
            }
        }
        Offset resultCurrent =
            _window_positionerLibrary._constrainTo(
                parentRect,
                parentAnchor._anchorPositionFor(anchorRect) + offset
            ) + childAnchor._offsetFor(childSize);
        if (constraintAdjustment.slideX)
        {
            double leftOverhang = resultCurrent.dx - displayRect.left;
            double rightOverhang = resultCurrent.dx + childSize.width - displayRect.right;
            if (leftOverhang < 0.0)
            {
                resultCurrent = resultCurrent.translate(-leftOverhang, 0.0);
            }
            else
            {
                if (rightOverhang > 0.0)
                {
                    resultCurrent = resultCurrent.translate(-rightOverhang, 0.0);
                }
            }
        }
        if (constraintAdjustment.slideY)
        {
            double topOverhang = resultCurrent.dy - displayRect.top;
            double bottomOverhang = resultCurrent.dy + childSize.height - displayRect.bottom;
            if (topOverhang < 0.0)
            {
                resultCurrent = resultCurrent.translate(0.0, -topOverhang);
            }
            else
            {
                if (bottomOverhang > 0.0)
                {
                    resultCurrent = resultCurrent.translate(0.0, -bottomOverhang);
                }
            }
        }
        if (_window_positionerLibrary._rectContains(displayRect, resultCurrent & childSize))
        {
            return resultCurrent & childSize;
        }
        Offset resultNext =
            _window_positionerLibrary._constrainTo(
                parentRect,
                parentAnchor._anchorPositionFor(anchorRect) + offset
            ) + childAnchor._offsetFor(childSize);
        if (constraintAdjustment.resizeX)
        {
            double leftOverhangLocal = resultNext.dx - displayRect.left;
            double rightOverhangLocal = resultNext.dx + childSize.width - displayRect.right;
            if (leftOverhangLocal < 0.0)
            {
                resultNext = resultNext.translate(-leftOverhangLocal, 0.0);
                childSize = new Size(childSize.width + leftOverhangLocal, childSize.height);
            }
            if (rightOverhangLocal > 0.0)
            {
                childSize = new Size(childSize.width - rightOverhangLocal, childSize.height);
            }
        }
        if (constraintAdjustment.resizeY)
        {
            double topOverhangLocal = resultNext.dy - displayRect.top;
            double bottomOverhangLocal = resultNext.dy + childSize.height - displayRect.bottom;
            if (topOverhangLocal < 0.0)
            {
                resultNext = resultNext.translate(0.0, -topOverhangLocal);
                childSize = new Size(childSize.width, childSize.height + topOverhangLocal);
            }
            if (bottomOverhangLocal > 0.0)
            {
                childSize = new Size(childSize.width, childSize.height - bottomOverhangLocal);
            }
        }
        if (_window_positionerLibrary._rectContains(displayRect, resultNext & childSize))
        {
            return resultNext & childSize;
        }
        return defaultResult;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override string ToString()
    {
        return $"WindowPositioner(parentAnchor: {parentAnchor}, childAnchor: {childAnchor}, offset: {offset}, constraintAdjustment: {constraintAdjustment})";
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public static partial class _window_positionerLibrary
{
    internal static bool _rectContains(Rect r1, Rect r2)
    {
        return (r1.left <= r2.left)
            && (r1.right >= r2.right)
            && (r1.top <= r2.top)
            && (r1.bottom >= r2.bottom);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public static partial class _window_positionerLibrary
{
    internal static Offset _constrainTo(Rect r, Offset p)
    {
        return new Offset(
            DorotiUiLibrary.clampDouble(p.dx, r.left, r.right),
            DorotiUiLibrary.clampDouble(p.dy, r.top, r.bottom)
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public static partial class _window_positionerLibrary
{
    internal static Offset _flipX(Offset offset)
    {
        return new Offset(-offset.dx, offset.dy);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public static partial class _window_positionerLibrary
{
    internal static Offset _flipY(Offset offset)
    {
        return new Offset(offset.dx, -offset.dy);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}
