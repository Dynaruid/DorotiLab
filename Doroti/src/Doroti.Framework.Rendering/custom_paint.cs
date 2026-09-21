// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/custom_paint.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Rendering;

public delegate List<CustomPainterSemantics> SemanticsBuilderCallback(Size size);

public abstract class CustomPainter : Listenable
{
    internal virtual Listenable? _repaint { get; private set; }

    protected CustomPainter(Listenable? repaint = null)
    {
        _repaint = repaint;
    }

    public virtual void addListener(Action listener) => _repaint?.addListener(listener);

    public virtual void removeListener(Action listener) => _repaint?.removeListener(listener);

    public abstract void paint(Canvas canvas, Size size);
    public virtual Func<Size, List<CustomPainterSemantics>>? semanticsBuilder => null;

    public virtual bool shouldRebuildSemantics(CustomPainter oldDelegate) =>
        shouldRepaint(oldDelegate);

    public abstract bool shouldRepaint(CustomPainter oldDelegate);

    public virtual bool? hitTest(Offset position) => null;

    public override string ToString() =>
        $"{DiagnosticsLibrary.describeIdentity(this)}({_repaint?.ToString() ?? ""})";
}

public class CustomPainterSemantics
{
    public virtual Key? key { get; private set; }
    public virtual Rect rect { get; private set; } = default!;
    public virtual Matrix4? transform { get; private set; }
    public virtual SemanticsProperties properties { get; private set; } = default!;
    public virtual HashSet<SemanticsTag>? tags { get; private set; }

    public CustomPainterSemantics(
        Key? key = null,
        Rect rect = default!,
        SemanticsProperties properties = default!,
        Matrix4? transform = null,
        HashSet<SemanticsTag>? tags = null
    )
    {
        this.key = key;
        this.rect = rect;
        this.properties = properties;
        this.transform = transform;
        this.tags = tags;
    }
}

public class RenderCustomPaint : RenderProxyBox
{
    internal virtual CustomPainter? _painter { get; set; } = default;
    internal virtual CustomPainter? _foregroundPainter { get; set; } = default;
    internal virtual Size _preferredSize { get; set; } = default!;
    public virtual bool isComplex { get; set; } = default!;
    public virtual bool willChange { get; set; } = default!;
    internal virtual Func<
        Size,
        List<CustomPainterSemantics>
    >? _backgroundSemanticsBuilder { get; set; } = default;
    internal virtual Func<
        Size,
        List<CustomPainterSemantics>
    >? _foregroundSemanticsBuilder { get; set; } = default;
    internal virtual List<SemanticsNode>? _backgroundSemanticsNodes { get; set; } = default;
    internal virtual List<SemanticsNode>? _foregroundSemanticsNodes { get; set; } = default;

    public RenderCustomPaint(
        CustomPainter? painter = null,
        CustomPainter? foregroundPainter = null,
        Size? preferredSize = null,
        bool isComplex = false,
        bool willChange = false,
        RenderBox? child = null
    )
        : base(child)
    {
        this.isComplex = isComplex;
        this.willChange = willChange;
        _painter = painter;
        _foregroundPainter = foregroundPainter;
        _preferredSize = preferredSize ?? Size.zero;
    }

    public virtual CustomPainter? painter
    {
        get => _painter;
        set
        {
            var __value = value;
            if (Equals(_painter, __value))
            {
                return;
            }
            CustomPainter? oldPainter = _painter;
            _painter = __value;
            _didUpdatePainter(_painter, oldPainter);
        }
    }
    public virtual CustomPainter? foregroundPainter
    {
        get => _foregroundPainter;
        set
        {
            var __value = value;
            if (Equals(_foregroundPainter, __value))
            {
                return;
            }
            CustomPainter? oldPainter = _foregroundPainter;
            _foregroundPainter = __value;
            _didUpdatePainter(_foregroundPainter, oldPainter);
        }
    }

    internal virtual void _didUpdatePainter(CustomPainter? newPainter, CustomPainter? oldPainter)
    {
        if (newPainter is null)
        {
            DartRuntimePrimitives.Assert(() => oldPainter is not null);
            markNeedsPaint();
        }
        else
        {
            if (
                (oldPainter is null)
                || (
                    !Equals(
                        DartRuntimePrimitives.RuntimeType(newPainter),
                        DartRuntimePrimitives.RuntimeType(oldPainter)
                    )
                )
                || newPainter.shouldRepaint(oldPainter)
            )
            {
                markNeedsPaint();
            }
        }
        if (attached)
        {
            oldPainter?.removeListener(markNeedsPaint);
            newPainter?.addListener(markNeedsPaint);
        }
        if (newPainter is null)
        {
            DartRuntimePrimitives.Assert(() => oldPainter is not null);
            if (attached)
            {
                markNeedsSemanticsUpdate();
            }
        }
        else
        {
            if (
                (oldPainter is null)
                || (
                    !Equals(
                        DartRuntimePrimitives.RuntimeType(newPainter),
                        DartRuntimePrimitives.RuntimeType(oldPainter)
                    )
                )
                || newPainter.shouldRebuildSemantics(oldPainter)
            )
            {
                markNeedsSemanticsUpdate();
            }
        }
    }

    public virtual Size preferredSize
    {
        get => _preferredSize;
        set
        {
            var __value = value;
            if (
                Equals(
                    preferredSize,
                    (
                        __value
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    )
                )
            )
            {
                return;
            }
            _preferredSize = (
                __value
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
            markNeedsLayout();
        }
    }

    public override double computeMinIntrinsicWidth(double height)
    {
        if (child is null)
        {
            return double.IsFinite(preferredSize.width) ? preferredSize.width : 0;
        }
        return base.computeMinIntrinsicWidth(height);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        if (child is null)
        {
            return double.IsFinite(preferredSize.width) ? preferredSize.width : 0;
        }
        return base.computeMaxIntrinsicWidth(height);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        if (child is null)
        {
            return double.IsFinite(preferredSize.height) ? preferredSize.height : 0;
        }
        return base.computeMinIntrinsicHeight(width);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        if (child is null)
        {
            return double.IsFinite(preferredSize.height) ? preferredSize.height : 0;
        }
        return base.computeMaxIntrinsicHeight(width);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void attach(PipelineOwner owner)
    {
        base.attach(owner);
        _painter?.addListener(markNeedsPaint);
        _foregroundPainter?.addListener(markNeedsPaint);
    }

    public override void detach()
    {
        _painter?.removeListener(markNeedsPaint);
        _foregroundPainter?.removeListener(markNeedsPaint);
        base.detach();
    }

    public override bool hitTestChildren(BoxHitTestResult result, Offset position)
    {
        if ((_foregroundPainter is not null) && (_foregroundPainter!.hitTest(position) ?? false))
        {
            return true;
        }
        return base.hitTestChildren(result, position: position);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool hitTestSelf(Offset position)
    {
        return (_painter is not null) && (_painter!.hitTest(position) ?? true);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        base.performLayout();
        markNeedsSemanticsUpdate();
    }

    public override Size computeSizeForNoChild(BoxConstraints constraints)
    {
        return constraints.constrain(preferredSize);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _paintWithPainter(Canvas canvas, Offset offset, CustomPainter painter)
    {
        long debugPreviousCanvasSaveCount = default!;
        canvas.save();
        DartRuntimePrimitives.Assert(() =>
        {
            debugPreviousCanvasSaveCount = canvas.getSaveCount();
            return true;
        });
        if (!Equals(offset, Offset.zero))
        {
            canvas.translate(offset.dx, offset.dy);
        }
        painter.paint(canvas, size);
        DartRuntimePrimitives.Assert(() =>
        {
            long debugNewCanvasSaveCount = canvas.getSaveCount();
            if (debugNewCanvasSaveCount > debugPreviousCanvasSaveCount)
            {
                throw new FlutterError(
                    new List<DiagnosticsNode>
                    {
                        new ErrorSummary(
                            $"The {painter} custom painter called canvas.save() or canvas.saveLayer() at least "
                                + $"{debugNewCanvasSaveCount - debugPreviousCanvasSaveCount} more "
                                + $"time{(((debugNewCanvasSaveCount - debugPreviousCanvasSaveCount) == 1L) ? "" : "s")} "
                                + "than it called canvas.restore()."
                        ),
                        new ErrorDescription(
                            "This leaves the canvas in an inconsistent state and will probably result in a broken display."
                        ),
                        new ErrorHint(
                            "You must pair each call to save()/saveLayer() with a later matching call to restore()."
                        ),
                    }
                );
            }
            if (debugNewCanvasSaveCount < debugPreviousCanvasSaveCount)
            {
                throw new FlutterError(
                    new List<DiagnosticsNode>
                    {
                        new ErrorSummary(
                            $"The {painter} custom painter called canvas.restore() "
                                + $"{debugPreviousCanvasSaveCount - debugNewCanvasSaveCount} more "
                                + $"time{(((debugPreviousCanvasSaveCount - debugNewCanvasSaveCount) == 1L) ? "" : "s")} "
                                + "than it called canvas.save() or canvas.saveLayer()."
                        ),
                        new ErrorDescription(
                            "This leaves the canvas in an inconsistent state and will result in a broken display."
                        ),
                        new ErrorHint(
                            "You should only call restore() if you first called save() or saveLayer()."
                        ),
                    }
                );
            }
            return debugNewCanvasSaveCount == debugPreviousCanvasSaveCount;
        });
        canvas.restore();
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        if (_painter is not null)
        {
            _paintWithPainter(context.canvas, offset, _painter!);
            _setRasterCacheHints(context);
        }
        base.paint(context, offset);
        if (_foregroundPainter is not null)
        {
            _paintWithPainter(context.canvas, offset, _foregroundPainter!);
            _setRasterCacheHints(context);
        }
    }

    internal virtual void _setRasterCacheHints(PaintingContext context)
    {
        if (isComplex)
        {
            context.setIsComplexHint();
        }
        if (willChange)
        {
            context.setWillChangeHint();
        }
    }

    public override void describeSemanticsConfiguration(SemanticsConfiguration config)
    {
        base.describeSemanticsConfiguration(config);
        _backgroundSemanticsBuilder = painter?.semanticsBuilder;
        _foregroundSemanticsBuilder = foregroundPainter?.semanticsBuilder;
        config.isSemanticBoundary =
            (_backgroundSemanticsBuilder is not null) || (_foregroundSemanticsBuilder is not null);
    }

    public override void assembleSemanticsNode(
        SemanticsNode node,
        SemanticsConfiguration config,
        IEnumerable<SemanticsNode> children
    )
    {
        DartRuntimePrimitives.Assert(() =>
        {
            if ((child is null) && (children.Count() != 0))
            {
                throw new FlutterError(
                    new List<DiagnosticsNode>
                    {
                        new ErrorSummary(
                            $"{GetType()} does not have a child widget but received a non-empty list of child SemanticsNode:\n"
                                + $"{string.Join("\n", children)}"
                        ),
                    }
                );
            }
            return true;
        });
        List<CustomPainterSemantics> backgroundSemantics = _backgroundSemanticsBuilder is null
            ? new List<CustomPainterSemantics>()
            : _backgroundSemanticsBuilder.Invoke(size);
        _backgroundSemanticsNodes = _updateSemanticsChildren(
            _backgroundSemanticsNodes,
            backgroundSemantics
        );
        List<CustomPainterSemantics> foregroundSemantics = _foregroundSemanticsBuilder is null
            ? new List<CustomPainterSemantics>()
            : _foregroundSemanticsBuilder.Invoke(size);
        _foregroundSemanticsNodes = _updateSemanticsChildren(
            _foregroundSemanticsNodes,
            foregroundSemantics
        );
        bool hasBackgroundSemantics =
            (_backgroundSemanticsNodes is not null)
            && (checked((long)_backgroundSemanticsNodes!.Count) != 0);
        bool hasForegroundSemantics =
            (_foregroundSemanticsNodes is not null)
            && (checked((long)_foregroundSemanticsNodes!.Count) != 0);
        var finalChildren = new List<SemanticsNode>();
        base.assembleSemanticsNode(node, config, finalChildren);
    }

    public override void clearSemantics()
    {
        base.clearSemantics();
        _backgroundSemanticsNodes = null;
        _foregroundSemanticsNodes = null;
    }

    internal static List<SemanticsNode> _updateSemanticsChildren(
        List<SemanticsNode>? oldSemantics,
        List<CustomPainterSemantics>? newChildSemantics
    )
    {
        oldSemantics = oldSemantics ?? new List<SemanticsNode>();
        newChildSemantics = newChildSemantics ?? new List<CustomPainterSemantics>();
        DartRuntimePrimitives.Assert(() =>
        {
            DartMap<Key, long> keys = new DartMap<Key, long>();
            var information = new List<DiagnosticsNode>();
            for (var i = 0L; i < checked(newChildSemantics!.Count); i += 1L)
            {
                CustomPainterSemantics child = newChildSemantics[(int)i];
                if (child.key is not null)
                {
                    if (keys.ContainsKey(child.key))
                    {
                        information.Add(
                            new ErrorDescription(
                                $"- duplicate key {child.key} found at position {i}"
                            )
                        );
                    }
                    keys[child.key!] = i;
                }
            }
            if (checked((long)information.Count) != 0)
            {
                information.Insert(
                    checked((int)0L),
                    new ErrorSummary("Failed to update the list of CustomPainterSemantics:")
                );
                throw new FlutterError(information);
            }
            return true;
        });
        var newChildrenTop = 0L;
        var oldChildrenTop = 0L;
        long newChildrenBottom = checked(newChildSemantics.Count) - 1L;
        long oldChildrenBottom = checked(oldSemantics.Count) - 1L;
        var newChildren = new List<SemanticsNode?>(
            Enumerable.Repeat<SemanticsNode?>(
                null,
                checked((int)checked((long)newChildSemantics.Count))
            )
        );
        while (oldChildrenTop <= oldChildrenBottom && newChildrenTop <= newChildrenBottom)
        {
            SemanticsNode oldChild = oldSemantics[(int)oldChildrenTop];
            CustomPainterSemantics newSemantics = newChildSemantics[(int)newChildrenTop];
            if (!_canUpdateSemanticsChild(oldChild, newSemantics))
            {
                break;
            }
            SemanticsNode newChild = _updateSemanticsChild(oldChild, newSemantics);
            newChildren[(int)newChildrenTop] = newChild;
            newChildrenTop += 1L;
            oldChildrenTop += 1L;
        }
        while (oldChildrenTop <= oldChildrenBottom && newChildrenTop <= newChildrenBottom)
        {
            SemanticsNode oldChildLocal = oldSemantics[(int)oldChildrenBottom];
            CustomPainterSemantics newChildLocal = newChildSemantics[(int)newChildrenBottom];
            if (!_canUpdateSemanticsChild(oldChildLocal, newChildLocal))
            {
                break;
            }
            oldChildrenBottom -= 1L;
            newChildrenBottom -= 1L;
        }
        bool haveOldChildren = oldChildrenTop <= oldChildrenBottom;
        DartMap<Key, SemanticsNode> oldKeyedChildren = default!;
        if (haveOldChildren)
        {
            oldKeyedChildren = new DartMap<Key, SemanticsNode>();
            while (oldChildrenTop <= oldChildrenBottom)
            {
                SemanticsNode oldChildAlternate = oldSemantics[(int)oldChildrenTop];
                if (oldChildAlternate.key is not null)
                {
                    oldKeyedChildren[oldChildAlternate.key!] = oldChildAlternate;
                }
                oldChildrenTop += 1L;
            }
        }
        while (newChildrenTop <= newChildrenBottom)
        {
            SemanticsNode? oldChildNested = default!;
            CustomPainterSemantics newSemanticsLocal = newChildSemantics[(int)newChildrenTop];
            if (haveOldChildren)
            {
                Key? keyLocal = newSemanticsLocal.key;
                if (keyLocal is not null)
                {
                    oldChildNested = oldKeyedChildren.GetValueOrDefault(keyLocal);
                    if (oldChildNested is not null)
                    {
                        if (_canUpdateSemanticsChild(oldChildNested, newSemanticsLocal))
                        {
                            oldKeyedChildren.remove(keyLocal);
                        }
                        else
                        {
                            oldChildNested = null;
                        }
                    }
                }
            }
            DartRuntimePrimitives.Assert(() =>
                (oldChildNested is null)
                || _canUpdateSemanticsChild(oldChildNested, newSemanticsLocal)
            );
            SemanticsNode newChildAlternate = _updateSemanticsChild(
                oldChildNested,
                newSemanticsLocal
            );
            DartRuntimePrimitives.Assert(() =>
                Equals(oldChildNested, newChildAlternate) || (oldChildNested is null)
            );
            newChildren[(int)newChildrenTop] = newChildAlternate;
            newChildrenTop += 1L;
        }
        DartRuntimePrimitives.Assert(() => oldChildrenTop == (oldChildrenBottom + 1L));
        DartRuntimePrimitives.Assert(() => newChildrenTop == (newChildrenBottom + 1L));
        DartRuntimePrimitives.Assert(() =>
            (checked(newChildSemantics.Count) - newChildrenTop)
            == (checked(oldSemantics.Count) - oldChildrenTop)
        );
        newChildrenBottom = checked(newChildSemantics.Count) - 1L;
        oldChildrenBottom = checked(oldSemantics.Count) - 1L;
        while (oldChildrenTop <= oldChildrenBottom && newChildrenTop <= newChildrenBottom)
        {
            SemanticsNode oldChildCurrent = oldSemantics[(int)oldChildrenTop];
            CustomPainterSemantics newSemanticsAlternate = newChildSemantics[(int)newChildrenTop];
            DartRuntimePrimitives.Assert(() =>
                _canUpdateSemanticsChild(oldChildCurrent, newSemanticsAlternate)
            );
            SemanticsNode newChildNested = _updateSemanticsChild(
                oldChildCurrent,
                newSemanticsAlternate
            );
            DartRuntimePrimitives.Assert(() => Equals(oldChildCurrent, newChildNested));
            newChildren[(int)newChildrenTop] = newChildNested;
            newChildrenTop += 1L;
            oldChildrenTop += 1L;
        }
        DartRuntimePrimitives.Assert(() =>
        {
            foreach (var node in newChildren)
            {
                DartRuntimePrimitives.Assert(() => node is not null);
            }
            return true;
        });
        return newChildren.cast<SemanticsNode>().ToList();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static bool _canUpdateSemanticsChild(
        SemanticsNode oldChild,
        CustomPainterSemantics newSemantics
    )
    {
        return Equals(oldChild.key, newSemantics.key);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static SemanticsNode _updateSemanticsChild(
        SemanticsNode? oldChild,
        CustomPainterSemantics newSemantics
    )
    {
        DartRuntimePrimitives.Assert(() =>
            (oldChild is null) || _canUpdateSemanticsChild(oldChild, newSemantics)
        );
        SemanticsNode newChild = oldChild ?? new SemanticsNode(key: newSemantics.key);
        SemanticsProperties propertiesLocal = newSemantics.properties;
        var configLocal = new SemanticsConfiguration();
        if (propertiesLocal.role is not null)
        {
            configLocal.role = (
                propertiesLocal.role
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
        }
        if (propertiesLocal.sortKey is not null)
        {
            configLocal.sortKey = propertiesLocal.sortKey;
        }
        if (propertiesLocal.@checked is not null)
        {
            configLocal.isChecked = propertiesLocal.@checked;
        }
        if (propertiesLocal.mixed is not null)
        {
            configLocal.isCheckStateMixed = propertiesLocal.mixed;
        }
        if (propertiesLocal.selected is not null)
        {
            configLocal.isSelected = (
                propertiesLocal.selected
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
        }
        if (propertiesLocal.button is not null)
        {
            configLocal.isButton = (
                propertiesLocal.button
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
        }
        if (propertiesLocal.expanded is not null)
        {
            configLocal.isExpanded = propertiesLocal.expanded;
        }
        if (propertiesLocal.link is not null)
        {
            configLocal.isLink = (
                propertiesLocal.link
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
        }
        if (propertiesLocal.linkUrl is not null)
        {
            configLocal.linkUrl = propertiesLocal.linkUrl;
        }
        if (propertiesLocal.textField is not null)
        {
            configLocal.isTextField = (
                propertiesLocal.textField
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
        }
        if (propertiesLocal.slider is not null)
        {
            configLocal.isSlider = (
                propertiesLocal.slider
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
        }
        if (propertiesLocal.keyboardKey is not null)
        {
            configLocal.isKeyboardKey = (
                propertiesLocal.keyboardKey
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
        }
        if (propertiesLocal.readOnly is not null)
        {
            configLocal.isReadOnly = (
                propertiesLocal.readOnly
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
        }
        if (propertiesLocal.focusable is not null)
        {
            configLocal.isFocusable = (
                propertiesLocal.focusable
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
        }
        if (propertiesLocal.focused is not null)
        {
            configLocal.isFocused = propertiesLocal.focused;
        }
        if (propertiesLocal.accessibilityFocusBlockType is not null)
        {
            configLocal.accessibilityFocusBlockType = (
                propertiesLocal.accessibilityFocusBlockType
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
        }
        if (propertiesLocal.enabled is not null)
        {
            configLocal.isEnabled = propertiesLocal.enabled;
        }
        if (propertiesLocal.inMutuallyExclusiveGroup is not null)
        {
            configLocal.isInMutuallyExclusiveGroup = (
                propertiesLocal.inMutuallyExclusiveGroup
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
        }
        if (propertiesLocal.obscured is not null)
        {
            configLocal.isObscured = (
                propertiesLocal.obscured
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
        }
        if (propertiesLocal.multiline is not null)
        {
            configLocal.isMultiline = (
                propertiesLocal.multiline
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
        }
        if (propertiesLocal.hidden is not null)
        {
            configLocal.isHidden = (
                propertiesLocal.hidden
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
        }
        if (propertiesLocal.header is not null)
        {
            configLocal.isHeader = (
                propertiesLocal.header
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
        }
        if (propertiesLocal.headingLevel is not null)
        {
            configLocal.headingLevel = (
                propertiesLocal.headingLevel
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
        }
        if (propertiesLocal.scopesRoute is not null)
        {
            configLocal.scopesRoute = (
                propertiesLocal.scopesRoute
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
        }
        if (propertiesLocal.namesRoute is not null)
        {
            configLocal.namesRoute = (
                propertiesLocal.namesRoute
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
        }
        if (propertiesLocal.liveRegion is not null)
        {
            configLocal.liveRegion = (
                propertiesLocal.liveRegion
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
        }
        if (propertiesLocal.isRequired is not null)
        {
            configLocal.isRequired = propertiesLocal.isRequired;
        }
        if (propertiesLocal.maxValueLength is not null)
        {
            configLocal.maxValueLength = propertiesLocal.maxValueLength;
        }
        if (propertiesLocal.currentValueLength is not null)
        {
            configLocal.currentValueLength = propertiesLocal.currentValueLength;
        }
        if (propertiesLocal.toggled is not null)
        {
            configLocal.isToggled = propertiesLocal.toggled;
        }
        if (propertiesLocal.image is not null)
        {
            configLocal.isImage = (
                propertiesLocal.image
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
        }
        if (propertiesLocal.label is not null)
        {
            configLocal.label = propertiesLocal.label!;
        }
        if (propertiesLocal.value is not null)
        {
            configLocal.value = propertiesLocal.value!;
        }
        if (propertiesLocal.increasedValue is not null)
        {
            configLocal.increasedValue = propertiesLocal.increasedValue!;
        }
        if (propertiesLocal.decreasedValue is not null)
        {
            configLocal.decreasedValue = propertiesLocal.decreasedValue!;
        }
        if (propertiesLocal.hint is not null)
        {
            configLocal.hint = propertiesLocal.hint!;
        }
        if (propertiesLocal.identifier is not null)
        {
            configLocal.identifier = propertiesLocal.identifier!;
        }
        if (propertiesLocal.traversalParentIdentifier is not null)
        {
            configLocal.traversalParentIdentifier = propertiesLocal.traversalParentIdentifier;
        }
        if (propertiesLocal.traversalChildIdentifier is not null)
        {
            configLocal.traversalChildIdentifier = propertiesLocal.traversalChildIdentifier;
        }
        if (propertiesLocal.tooltip is not null)
        {
            configLocal.tooltip = propertiesLocal.tooltip!;
        }
        if (propertiesLocal.hintOverrides is not null)
        {
            configLocal.hintOverrides = propertiesLocal.hintOverrides;
        }
        if (propertiesLocal.tagForChildren is not null)
        {
            configLocal.addTagForChildren(propertiesLocal.tagForChildren!);
        }
        if (propertiesLocal.controlsNodes is not null)
        {
            configLocal.controlsNodes = propertiesLocal.controlsNodes;
        }
        if (propertiesLocal.hint is not null)
        {
            configLocal.hint = propertiesLocal.hint!;
        }
        if (propertiesLocal.textDirection is not null)
        {
            configLocal.textDirection = propertiesLocal.textDirection;
        }
        if (!Equals(configLocal.validationResult, propertiesLocal.validationResult))
        {
            configLocal.validationResult = propertiesLocal.validationResult;
        }
        if (propertiesLocal.hitTestBehavior is not null)
        {
            configLocal.hitTestBehavior = (
                propertiesLocal.hitTestBehavior
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
        }
        if (propertiesLocal.inputType is not null)
        {
            configLocal.inputType = (
                propertiesLocal.inputType
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
        }
        if (propertiesLocal.minValue is not null)
        {
            configLocal.minValue = propertiesLocal.minValue;
        }
        if (propertiesLocal.maxValue is not null)
        {
            configLocal.maxValue = propertiesLocal.maxValue;
        }
        if (propertiesLocal.onTap is not null)
        {
            configLocal.onTap = propertiesLocal.onTap;
        }
        if (propertiesLocal.onLongPress is not null)
        {
            configLocal.onLongPress = propertiesLocal.onLongPress;
        }
        if (propertiesLocal.onScrollLeft is not null)
        {
            configLocal.onScrollLeft = propertiesLocal.onScrollLeft;
        }
        if (propertiesLocal.onScrollRight is not null)
        {
            configLocal.onScrollRight = propertiesLocal.onScrollRight;
        }
        if (propertiesLocal.onScrollUp is not null)
        {
            configLocal.onScrollUp = propertiesLocal.onScrollUp;
        }
        if (propertiesLocal.onScrollDown is not null)
        {
            configLocal.onScrollDown = propertiesLocal.onScrollDown;
        }
        if (propertiesLocal.onIncrease is not null)
        {
            configLocal.onIncrease = propertiesLocal.onIncrease;
        }
        if (propertiesLocal.onDecrease is not null)
        {
            configLocal.onDecrease = propertiesLocal.onDecrease;
        }
        if (propertiesLocal.onCopy is not null)
        {
            configLocal.onCopy = propertiesLocal.onCopy;
        }
        if (propertiesLocal.onCut is not null)
        {
            configLocal.onCut = propertiesLocal.onCut;
        }
        if (propertiesLocal.onPaste is not null)
        {
            configLocal.onPaste = propertiesLocal.onPaste;
        }
        if (propertiesLocal.onMoveCursorForwardByCharacter is not null)
        {
            configLocal.onMoveCursorForwardByCharacter =
                propertiesLocal.onMoveCursorForwardByCharacter;
        }
        if (propertiesLocal.onMoveCursorBackwardByCharacter is not null)
        {
            configLocal.onMoveCursorBackwardByCharacter =
                propertiesLocal.onMoveCursorBackwardByCharacter;
        }
        if (propertiesLocal.onMoveCursorForwardByWord is not null)
        {
            configLocal.onMoveCursorForwardByWord = propertiesLocal.onMoveCursorForwardByWord;
        }
        if (propertiesLocal.onMoveCursorBackwardByWord is not null)
        {
            configLocal.onMoveCursorBackwardByWord = propertiesLocal.onMoveCursorBackwardByWord;
        }
        if (propertiesLocal.onSetSelection is not null)
        {
            configLocal.onSetSelection = propertiesLocal.onSetSelection;
        }
        if (propertiesLocal.onSetText is not null)
        {
            configLocal.onSetText = propertiesLocal.onSetText;
        }
        if (propertiesLocal.onDidGainAccessibilityFocus is not null)
        {
            configLocal.onDidGainAccessibilityFocus = propertiesLocal.onDidGainAccessibilityFocus;
        }
        if (propertiesLocal.onDidLoseAccessibilityFocus is not null)
        {
            configLocal.onDidLoseAccessibilityFocus = propertiesLocal.onDidLoseAccessibilityFocus;
        }
        if (propertiesLocal.onFocus is not null)
        {
            configLocal.onFocus = propertiesLocal.onFocus;
        }
        if (propertiesLocal.onDismiss is not null)
        {
            configLocal.onDismiss = propertiesLocal.onDismiss;
        }
        if (propertiesLocal.onExpand is not null)
        {
            configLocal.onExpand = propertiesLocal.onExpand;
        }
        if (propertiesLocal.onCollapse is not null)
        {
            configLocal.onCollapse = propertiesLocal.onCollapse;
        }
        newChild.updateWith(
            config: configLocal,
            childrenInInversePaintOrder: new List<SemanticsNode>()
        );
        (
            (Func<SemanticsNode>)(
                () =>
                {
                    var __cascade = newChild;
                    __cascade.rect = newSemantics.rect;
                    __cascade.transform = newSemantics.transform;
                    __cascade.tags = newSemantics.tags;
                    return __cascade;
                }
            )
        )();
        return newChild;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new MessageProperty("painter", $"{painter}"));
        properties.add(
            new MessageProperty(
                "foregroundPainter",
                $"{foregroundPainter}",
                level: (foregroundPainter is not null) ? DiagnosticLevel.info : DiagnosticLevel.fine
            )
        );
        properties.add(
            new DiagnosticsProperty<Size>("preferredSize", preferredSize, defaultValue: Size.zero)
        );
        properties.add(new DiagnosticsProperty<bool>("isComplex", isComplex, defaultValue: false));
        properties.add(
            new DiagnosticsProperty<bool>("willChange", willChange, defaultValue: false)
        );
    }
}
