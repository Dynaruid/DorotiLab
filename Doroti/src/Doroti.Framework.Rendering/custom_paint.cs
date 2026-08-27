// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/custom_paint.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622, CS8625, CS8714
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Rendering;

public delegate List<CustomPainterSemantics> SemanticsBuilderCallback(Size size);

public abstract class CustomPainter : Listenable
{
    internal virtual Listenable? _repaint { get; private set; }

    protected CustomPainter(Listenable? repaint = null)
    {
        this._repaint = repaint;
    }

    public virtual void addListener(Action listener) => this._repaint?.addListener(listener);
    public virtual void removeListener(Action listener) => this._repaint?.removeListener(listener);
    public abstract void paint(Canvas canvas, Size size);
    public virtual Func<Size, List<CustomPainterSemantics>>? semanticsBuilder => null;
    public virtual bool shouldRebuildSemantics(CustomPainter oldDelegate) => shouldRepaint(oldDelegate);
    public abstract bool shouldRepaint(CustomPainter oldDelegate);
    public virtual bool? hitTest(Offset position) => null;
    public override string ToString() => $"{(global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}({(this._repaint?.ToString() ?? "")})";
}

public class CustomPainterSemantics
{
    public virtual Key? key { get; private set; }
    public virtual Rect rect { get; private set; } = default!;
    public virtual Matrix4? transform { get; private set; }
    public virtual global::Doroti.Framework.Semantics.SemanticsProperties properties { get; private set; } = default!;
    public virtual HashSet<global::Doroti.Framework.Semantics.SemanticsTag>? tags { get; private set; }

    public CustomPainterSemantics(Key? key = null, Rect rect = default!, global::Doroti.Framework.Semantics.SemanticsProperties properties = default!, Matrix4? transform = null, HashSet<global::Doroti.Framework.Semantics.SemanticsTag>? tags = null)
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
    internal virtual Func<Size, List<CustomPainterSemantics>>? _backgroundSemanticsBuilder { get; set; } = default;
    internal virtual Func<Size, List<CustomPainterSemantics>>? _foregroundSemanticsBuilder { get; set; } = default;
    internal virtual List<global::Doroti.Framework.Semantics.SemanticsNode>? _backgroundSemanticsNodes { get; set; } = default;
    internal virtual List<global::Doroti.Framework.Semantics.SemanticsNode>? _foregroundSemanticsNodes { get; set; } = default;

    public RenderCustomPaint(CustomPainter? painter = null, CustomPainter? foregroundPainter = null, Size preferredSize = default, bool isComplex = false, bool willChange = false, RenderBox? child = null) : base(child)
    {
        this.isComplex = isComplex;
        this.willChange = willChange;
        this._painter = painter;
        this._foregroundPainter = foregroundPainter;
        this._preferredSize = preferredSize;
    }

    public virtual CustomPainter? painter
    {
        get => this._painter;
        set
        {
            var __value = value;
            if ((object.Equals(this._painter, __value)))
            {
                return;
            }
            CustomPainter? oldPainter = this._painter;
            _painter = __value;
            _didUpdatePainter(this._painter, oldPainter);
        }
    }
    public virtual CustomPainter? foregroundPainter
    {
        get => this._foregroundPainter;
        set
        {
            var __value = value;
            if ((object.Equals(this._foregroundPainter, __value)))
            {
                return;
            }
            CustomPainter? oldPainter = this._foregroundPainter;
            _foregroundPainter = __value;
            _didUpdatePainter(this._foregroundPainter, oldPainter);
        }
    }
    internal virtual void _didUpdatePainter(CustomPainter? newPainter, CustomPainter? oldPainter)
    {
        if ((newPainter is null))
        {
            DartRuntimePrimitives.Assert(() => (oldPainter is not null));
            markNeedsPaint();
        }
        else
        {
            if ((((oldPainter is null) || (!object.Equals(DartRuntimePrimitives.RuntimeType(newPainter), DartRuntimePrimitives.RuntimeType(oldPainter)))) || newPainter.shouldRepaint(oldPainter)))
            {
                markNeedsPaint();
            }
        }
        if (attached)
        {
            oldPainter?.removeListener(markNeedsPaint);
            newPainter?.addListener(markNeedsPaint);
        }
        if ((newPainter is null))
        {
            DartRuntimePrimitives.Assert(() => (oldPainter is not null));
            if (attached)
            {
                markNeedsSemanticsUpdate();
            }
        }
        else
        {
            if ((((oldPainter is null) || (!object.Equals(DartRuntimePrimitives.RuntimeType(newPainter), DartRuntimePrimitives.RuntimeType(oldPainter)))) || newPainter.shouldRebuildSemantics(oldPainter)))
            {
                markNeedsSemanticsUpdate();
            }
        }
    }

    public virtual global::Doroti.Ui.Size preferredSize
    {
        get => this._preferredSize;
        set
        {
            var __value = value;
            if ((object.Equals(this.preferredSize, DartRuntimePrimitives.RequireValue(__value))))
            {
                return;
            }
            _preferredSize = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    public override double computeMinIntrinsicWidth(double height)
    {
        if ((child is null))
        {
            return (double.IsFinite(this.preferredSize.width) ? this.preferredSize.width : 0);
        }
        return base.computeMinIntrinsicWidth(height);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        if ((child is null))
        {
            return (double.IsFinite(this.preferredSize.width) ? this.preferredSize.width : 0);
        }
        return base.computeMaxIntrinsicWidth(height);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        if ((child is null))
        {
            return (double.IsFinite(this.preferredSize.height) ? this.preferredSize.height : 0);
        }
        return base.computeMinIntrinsicHeight(width);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        if ((child is null))
        {
            return (double.IsFinite(this.preferredSize.height) ? this.preferredSize.height : 0);
        }
        return base.computeMaxIntrinsicHeight(width);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void attach(PipelineOwner owner)
    {
        base.attach(owner);
        this._painter?.addListener(markNeedsPaint);
        this._foregroundPainter?.addListener(markNeedsPaint);
    }

    public override void detach()
    {
        this._painter?.removeListener(markNeedsPaint);
        this._foregroundPainter?.removeListener(markNeedsPaint);
        base.detach();
    }

    public override bool hitTestChildren(BoxHitTestResult result, Offset position)
    {
        if (((this._foregroundPainter is not null) && ((this._foregroundPainter!.hitTest(position) ?? false))))
        {
            return true;
        }
        return base.hitTestChildren(result, position: position);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool hitTestSelf(Offset position)
    {
        return ((this._painter is not null) && ((this._painter!.hitTest(position) ?? true)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        base.performLayout();
        markNeedsSemanticsUpdate();
    }

    public override Size computeSizeForNoChild(BoxConstraints constraints)
    {
        return constraints.constrain(this.preferredSize);
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
        if ((!object.Equals(offset, Offset.zero)))
        {
            canvas.translate(offset.dx, offset.dy);
        }
        painter.paint(canvas, size);
        DartRuntimePrimitives.Assert(() =>
            {
                long debugNewCanvasSaveCount = canvas.getSaveCount();
                if ((debugNewCanvasSaveCount > debugPreviousCanvasSaveCount))
                {
                    throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"The {painter} custom painter called canvas.save() or canvas.saveLayer() at least " + $"{(debugNewCanvasSaveCount - debugPreviousCanvasSaveCount)} more " + $"time{(((debugNewCanvasSaveCount - debugPreviousCanvasSaveCount) == 1L) ? "" : "s")} " + "than it called canvas.restore()."), new ErrorDescription("This leaves the canvas in an inconsistent state and will probably result in a broken display."), new ErrorHint("You must pair each call to save()/saveLayer() with a later matching call to restore().") });
                }
                if ((debugNewCanvasSaveCount < debugPreviousCanvasSaveCount))
                {
                    throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"The {painter} custom painter called canvas.restore() " + $"{(debugPreviousCanvasSaveCount - debugNewCanvasSaveCount)} more " + $"time{(((debugPreviousCanvasSaveCount - debugNewCanvasSaveCount) == 1L) ? "" : "s")} " + "than it called canvas.save() or canvas.saveLayer()."), new ErrorDescription("This leaves the canvas in an inconsistent state and will result in a broken display."), new ErrorHint("You should only call restore() if you first called save() or saveLayer().") });
                }
                return (debugNewCanvasSaveCount == debugPreviousCanvasSaveCount);
            });
        canvas.restore();
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        if ((this._painter is not null))
        {
            _paintWithPainter(((PaintingContext)context).canvas, offset, this._painter!);
            _setRasterCacheHints(context);
        }
        base.paint(context, offset);
        if ((this._foregroundPainter is not null))
        {
            _paintWithPainter(((PaintingContext)context).canvas, offset, this._foregroundPainter!);
            _setRasterCacheHints(context);
        }
    }

    internal virtual void _setRasterCacheHints(PaintingContext context)
    {
        if (this.isComplex)
        {
            context.setIsComplexHint();
        }
        if (this.willChange)
        {
            context.setWillChangeHint();
        }
    }

    public override void describeSemanticsConfiguration(global::Doroti.Framework.Semantics.SemanticsConfiguration config)
    {
        base.describeSemanticsConfiguration(config);
        _backgroundSemanticsBuilder = this.painter?.semanticsBuilder;
        _foregroundSemanticsBuilder = this.foregroundPainter?.semanticsBuilder;
        config.isSemanticBoundary = ((this._backgroundSemanticsBuilder is not null) || (this._foregroundSemanticsBuilder is not null));
    }

    public override void assembleSemanticsNode(global::Doroti.Framework.Semantics.SemanticsNode node, global::Doroti.Framework.Semantics.SemanticsConfiguration config, IEnumerable<global::Doroti.Framework.Semantics.SemanticsNode> children)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (((child is null) && (children.Count() != 0)))
                {
                    throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"{this.GetType()} does not have a child widget but received a non-empty list of child SemanticsNode:\n" + $"{string.Join("\n", children)}") });
                }
                return true;
            });
        List<CustomPainterSemantics> backgroundSemantics = ((this._backgroundSemanticsBuilder is null ? new List<CustomPainterSemantics>() : this._backgroundSemanticsBuilder.Invoke(size)));
        _backgroundSemanticsNodes = _updateSemanticsChildren(this._backgroundSemanticsNodes, backgroundSemantics);
        List<CustomPainterSemantics> foregroundSemantics = ((this._foregroundSemanticsBuilder is null ? new List<CustomPainterSemantics>() : this._foregroundSemanticsBuilder.Invoke(size)));
        _foregroundSemanticsNodes = _updateSemanticsChildren(this._foregroundSemanticsNodes, foregroundSemantics);
        bool hasBackgroundSemantics = ((this._backgroundSemanticsNodes is not null) && (checked((long)(this._backgroundSemanticsNodes!.Count)) != 0));
        bool hasForegroundSemantics = ((this._foregroundSemanticsNodes is not null) && (checked((long)(this._foregroundSemanticsNodes!.Count)) != 0));
        var finalChildren = new List<global::Doroti.Framework.Semantics.SemanticsNode>();
        base.assembleSemanticsNode(node, config, finalChildren);
    }

    public override void clearSemantics()
    {
        base.clearSemantics();
        _backgroundSemanticsNodes = null;
        _foregroundSemanticsNodes = null;
    }

    internal static List<global::Doroti.Framework.Semantics.SemanticsNode> _updateSemanticsChildren(List<global::Doroti.Framework.Semantics.SemanticsNode>? oldSemantics, List<CustomPainterSemantics>? newChildSemantics)
    {
        oldSemantics = (oldSemantics ?? new List<global::Doroti.Framework.Semantics.SemanticsNode>());
        newChildSemantics = (newChildSemantics ?? new List<CustomPainterSemantics>());
        DartRuntimePrimitives.Assert(() =>
            {
                DartMap<Key, long> keys = new DartMap<Key, long>();
                var information = new List<DiagnosticsNode>();
                for (var i = 0L; (i < checked((long)(newChildSemantics!.Count))); i += 1L)
                {
                    CustomPainterSemantics child = newChildSemantics[(int)(i)];
                    if ((((CustomPainterSemantics)child).key is not null))
                    {
                        if (keys.ContainsKey(((CustomPainterSemantics)child).key))
                        {
                            information.Add(new ErrorDescription($"- duplicate key {((CustomPainterSemantics)child).key} found at position {i}"));
                        }
                        keys[((CustomPainterSemantics)child).key!] = i;
                    }
                }
                if ((checked((long)(information.Count)) != 0))
                {
                    information.Insert(checked((int)0L), new ErrorSummary("Failed to update the list of CustomPainterSemantics:"));
                    throw new FlutterError(information);
                }
                return true;
            });
        var newChildrenTop = 0L;
        var oldChildrenTop = 0L;
        long newChildrenBottom = (checked((long)(newChildSemantics.Count)) - 1L);
        long oldChildrenBottom = (checked((long)(oldSemantics.Count)) - 1L);
        var newChildren = new List<global::Doroti.Framework.Semantics.SemanticsNode?>(System.Linq.Enumerable.Repeat<global::Doroti.Framework.Semantics.SemanticsNode?>(null, checked((int)checked((long)(newChildSemantics.Count)))));
        while ((((oldChildrenTop <= oldChildrenBottom)) && ((newChildrenTop <= newChildrenBottom))))
        {
            global::Doroti.Framework.Semantics.SemanticsNode oldChild = oldSemantics[(int)(oldChildrenTop)];
            CustomPainterSemantics newSemantics = newChildSemantics[(int)(newChildrenTop)];
            if (!_canUpdateSemanticsChild(oldChild, newSemantics))
            {
                break;
            }
            global::Doroti.Framework.Semantics.SemanticsNode newChild = _updateSemanticsChild(oldChild, newSemantics);
            newChildren[(int)(newChildrenTop)] = newChild;
            newChildrenTop += 1L;
            oldChildrenTop += 1L;
        }
        while ((((oldChildrenTop <= oldChildrenBottom)) && ((newChildrenTop <= newChildrenBottom))))
        {
            global::Doroti.Framework.Semantics.SemanticsNode oldChildLocal = oldSemantics[(int)(oldChildrenBottom)];
            CustomPainterSemantics newChildLocal = newChildSemantics[(int)(newChildrenBottom)];
            if (!_canUpdateSemanticsChild(oldChildLocal, newChildLocal))
            {
                break;
            }
            oldChildrenBottom -= 1L;
            newChildrenBottom -= 1L;
        }
        bool haveOldChildren = (oldChildrenTop <= oldChildrenBottom);
        DartMap<Key, global::Doroti.Framework.Semantics.SemanticsNode> oldKeyedChildren = default!;
        if (haveOldChildren)
        {
            oldKeyedChildren = new DartMap<Key, global::Doroti.Framework.Semantics.SemanticsNode>();
            while ((oldChildrenTop <= oldChildrenBottom))
            {
                global::Doroti.Framework.Semantics.SemanticsNode oldChildAlternate = oldSemantics[(int)(oldChildrenTop)];
                if ((((global::Doroti.Framework.Semantics.SemanticsNode)oldChildAlternate).key is not null))
                {
                    oldKeyedChildren[((global::Doroti.Framework.Semantics.SemanticsNode)oldChildAlternate).key!] = oldChildAlternate;
                }
                oldChildrenTop += 1L;
            }
        }
        while ((newChildrenTop <= newChildrenBottom))
        {
            global::Doroti.Framework.Semantics.SemanticsNode? oldChildNested = default!;
            CustomPainterSemantics newSemanticsLocal = newChildSemantics[(int)(newChildrenTop)];
            if (haveOldChildren)
            {
                Key? keyLocal = ((CustomPainterSemantics)newSemanticsLocal).key;
                if ((keyLocal is not null))
                {
                    oldChildNested = oldKeyedChildren.GetValueOrDefault(keyLocal);
                    if ((oldChildNested is not null))
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
            DartRuntimePrimitives.Assert(() => ((oldChildNested is null) || _canUpdateSemanticsChild(oldChildNested, newSemanticsLocal)));
            global::Doroti.Framework.Semantics.SemanticsNode newChildAlternate = _updateSemanticsChild(oldChildNested, newSemanticsLocal);
            DartRuntimePrimitives.Assert(() => ((object.Equals(oldChildNested, newChildAlternate)) || (oldChildNested is null)));
            newChildren[(int)(newChildrenTop)] = newChildAlternate;
            newChildrenTop += 1L;
        }
        DartRuntimePrimitives.Assert(() => (oldChildrenTop == (oldChildrenBottom + 1L)));
        DartRuntimePrimitives.Assert(() => (newChildrenTop == (newChildrenBottom + 1L)));
        DartRuntimePrimitives.Assert(() => ((checked((long)(newChildSemantics.Count)) - newChildrenTop) == (checked((long)(oldSemantics.Count)) - oldChildrenTop)));
        newChildrenBottom = (checked((long)(newChildSemantics.Count)) - 1L);
        oldChildrenBottom = (checked((long)(oldSemantics.Count)) - 1L);
        while ((((oldChildrenTop <= oldChildrenBottom)) && ((newChildrenTop <= newChildrenBottom))))
        {
            global::Doroti.Framework.Semantics.SemanticsNode oldChildCurrent = oldSemantics[(int)(oldChildrenTop)];
            CustomPainterSemantics newSemanticsAlternate = newChildSemantics[(int)(newChildrenTop)];
            DartRuntimePrimitives.Assert(() => _canUpdateSemanticsChild(oldChildCurrent, newSemanticsAlternate));
            global::Doroti.Framework.Semantics.SemanticsNode newChildNested = _updateSemanticsChild(oldChildCurrent, newSemanticsAlternate);
            DartRuntimePrimitives.Assert(() => (object.Equals(oldChildCurrent, newChildNested)));
            newChildren[(int)(newChildrenTop)] = newChildNested;
            newChildrenTop += 1L;
            oldChildrenTop += 1L;
        }
        DartRuntimePrimitives.Assert(() =>
            {
                foreach (var node in newChildren)
                {
                    DartRuntimePrimitives.Assert(() => (node is not null));
                }
                return true;
            });
        return newChildren.cast<global::Doroti.Framework.Semantics.SemanticsNode>().ToList();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static bool _canUpdateSemanticsChild(global::Doroti.Framework.Semantics.SemanticsNode oldChild, CustomPainterSemantics newSemantics)
    {
        return (object.Equals(((global::Doroti.Framework.Semantics.SemanticsNode)oldChild).key, ((CustomPainterSemantics)newSemantics).key));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static global::Doroti.Framework.Semantics.SemanticsNode _updateSemanticsChild(global::Doroti.Framework.Semantics.SemanticsNode? oldChild, CustomPainterSemantics newSemantics)
    {
        DartRuntimePrimitives.Assert(() => ((oldChild is null) || _canUpdateSemanticsChild(oldChild, newSemantics)));
        global::Doroti.Framework.Semantics.SemanticsNode newChild = (oldChild ?? new global::Doroti.Framework.Semantics.SemanticsNode(key: ((CustomPainterSemantics)newSemantics).key));
        global::Doroti.Framework.Semantics.SemanticsProperties propertiesLocal = ((CustomPainterSemantics)newSemantics).properties;
        var configLocal = new global::Doroti.Framework.Semantics.SemanticsConfiguration();
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).role is not null))
        {
            configLocal.role = DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).role);
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).sortKey is not null))
        {
            configLocal.sortKey = ((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).sortKey;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).@checked is not null))
        {
            configLocal.isChecked = ((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).@checked;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).mixed is not null))
        {
            configLocal.isCheckStateMixed = ((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).mixed;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).selected is not null))
        {
            configLocal.isSelected = DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).selected);
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).button is not null))
        {
            configLocal.isButton = DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).button);
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).expanded is not null))
        {
            configLocal.isExpanded = ((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).expanded;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).link is not null))
        {
            configLocal.isLink = DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).link);
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).linkUrl is not null))
        {
            configLocal.linkUrl = ((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).linkUrl;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).textField is not null))
        {
            configLocal.isTextField = DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).textField);
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).slider is not null))
        {
            configLocal.isSlider = DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).slider);
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).keyboardKey is not null))
        {
            configLocal.isKeyboardKey = DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).keyboardKey);
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).readOnly is not null))
        {
            configLocal.isReadOnly = DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).readOnly);
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).focusable is not null))
        {
            configLocal.isFocusable = DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).focusable);
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).focused is not null))
        {
            configLocal.isFocused = ((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).focused;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).accessibilityFocusBlockType is not null))
        {
            configLocal.accessibilityFocusBlockType = DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).accessibilityFocusBlockType);
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).enabled is not null))
        {
            configLocal.isEnabled = ((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).enabled;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).inMutuallyExclusiveGroup is not null))
        {
            configLocal.isInMutuallyExclusiveGroup = DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).inMutuallyExclusiveGroup);
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).obscured is not null))
        {
            configLocal.isObscured = DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).obscured);
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).multiline is not null))
        {
            configLocal.isMultiline = DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).multiline);
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).hidden is not null))
        {
            configLocal.isHidden = DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).hidden);
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).header is not null))
        {
            configLocal.isHeader = DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).header);
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).headingLevel is not null))
        {
            configLocal.headingLevel = DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).headingLevel);
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).scopesRoute is not null))
        {
            configLocal.scopesRoute = DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).scopesRoute);
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).namesRoute is not null))
        {
            configLocal.namesRoute = DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).namesRoute);
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).liveRegion is not null))
        {
            configLocal.liveRegion = DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).liveRegion);
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).isRequired is not null))
        {
            configLocal.isRequired = ((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).isRequired;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).maxValueLength is not null))
        {
            configLocal.maxValueLength = ((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).maxValueLength;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).currentValueLength is not null))
        {
            configLocal.currentValueLength = ((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).currentValueLength;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).toggled is not null))
        {
            configLocal.isToggled = ((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).toggled;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).image is not null))
        {
            configLocal.isImage = DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).image);
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).label is not null))
        {
            configLocal.label = ((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).label!;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).value is not null))
        {
            configLocal.value = ((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).value!;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).increasedValue is not null))
        {
            configLocal.increasedValue = ((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).increasedValue!;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).decreasedValue is not null))
        {
            configLocal.decreasedValue = ((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).decreasedValue!;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).hint is not null))
        {
            configLocal.hint = ((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).hint!;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).identifier is not null))
        {
            configLocal.identifier = ((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).identifier!;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).traversalParentIdentifier is not null))
        {
            configLocal.traversalParentIdentifier = ((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).traversalParentIdentifier;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).traversalChildIdentifier is not null))
        {
            configLocal.traversalChildIdentifier = ((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).traversalChildIdentifier;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).tooltip is not null))
        {
            configLocal.tooltip = ((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).tooltip!;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).hintOverrides is not null))
        {
            configLocal.hintOverrides = ((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).hintOverrides;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).tagForChildren is not null))
        {
            configLocal.addTagForChildren(((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).tagForChildren!);
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).controlsNodes is not null))
        {
            configLocal.controlsNodes = ((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).controlsNodes;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).hint is not null))
        {
            configLocal.hint = ((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).hint!;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).textDirection is not null))
        {
            configLocal.textDirection = ((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).textDirection;
        }
        if ((!object.Equals(((global::Doroti.Framework.Semantics.SemanticsConfiguration)configLocal).validationResult, ((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).validationResult)))
        {
            configLocal.validationResult = ((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).validationResult;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).hitTestBehavior is not null))
        {
            configLocal.hitTestBehavior = DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).hitTestBehavior);
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).inputType is not null))
        {
            configLocal.inputType = DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).inputType);
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).minValue is not null))
        {
            configLocal.minValue = ((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).minValue;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).maxValue is not null))
        {
            configLocal.maxValue = ((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).maxValue;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).onTap is not null))
        {
            configLocal.onTap = ((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).onTap;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).onLongPress is not null))
        {
            configLocal.onLongPress = ((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).onLongPress;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).onScrollLeft is not null))
        {
            configLocal.onScrollLeft = ((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).onScrollLeft;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).onScrollRight is not null))
        {
            configLocal.onScrollRight = ((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).onScrollRight;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).onScrollUp is not null))
        {
            configLocal.onScrollUp = ((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).onScrollUp;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).onScrollDown is not null))
        {
            configLocal.onScrollDown = ((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).onScrollDown;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).onIncrease is not null))
        {
            configLocal.onIncrease = ((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).onIncrease;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).onDecrease is not null))
        {
            configLocal.onDecrease = ((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).onDecrease;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).onCopy is not null))
        {
            configLocal.onCopy = ((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).onCopy;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).onCut is not null))
        {
            configLocal.onCut = ((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).onCut;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).onPaste is not null))
        {
            configLocal.onPaste = ((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).onPaste;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).onMoveCursorForwardByCharacter is not null))
        {
            configLocal.onMoveCursorForwardByCharacter = ((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).onMoveCursorForwardByCharacter;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).onMoveCursorBackwardByCharacter is not null))
        {
            configLocal.onMoveCursorBackwardByCharacter = ((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).onMoveCursorBackwardByCharacter;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).onMoveCursorForwardByWord is not null))
        {
            configLocal.onMoveCursorForwardByWord = ((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).onMoveCursorForwardByWord;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).onMoveCursorBackwardByWord is not null))
        {
            configLocal.onMoveCursorBackwardByWord = ((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).onMoveCursorBackwardByWord;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).onSetSelection is not null))
        {
            configLocal.onSetSelection = ((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).onSetSelection;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).onSetText is not null))
        {
            configLocal.onSetText = ((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).onSetText;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).onDidGainAccessibilityFocus is not null))
        {
            configLocal.onDidGainAccessibilityFocus = ((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).onDidGainAccessibilityFocus;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).onDidLoseAccessibilityFocus is not null))
        {
            configLocal.onDidLoseAccessibilityFocus = ((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).onDidLoseAccessibilityFocus;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).onFocus is not null))
        {
            configLocal.onFocus = ((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).onFocus;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).onDismiss is not null))
        {
            configLocal.onDismiss = ((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).onDismiss;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).onExpand is not null))
        {
            configLocal.onExpand = ((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).onExpand;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).onCollapse is not null))
        {
            configLocal.onCollapse = ((global::Doroti.Framework.Semantics.SemanticsProperties)propertiesLocal).onCollapse;
        }
        newChild.updateWith(config: configLocal, childrenInInversePaintOrder: new List<global::Doroti.Framework.Semantics.SemanticsNode>());
        ((Func<global::Doroti.Framework.Semantics.SemanticsNode>)(() =>
{
    var __cascade = newChild;
    __cascade.rect = ((CustomPainterSemantics)newSemantics).rect;
    __cascade.transform = ((CustomPainterSemantics)newSemantics).transform;
    __cascade.tags = ((CustomPainterSemantics)newSemantics).tags;
    return __cascade;
}))();
        return newChild;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new MessageProperty("painter", $"{this.painter}"));
        properties.add(new MessageProperty("foregroundPainter", $"{this.foregroundPainter}", level: ((this.foregroundPainter is not null) ? DiagnosticLevel.info : DiagnosticLevel.fine)));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Size>("preferredSize", this.preferredSize, defaultValue: Size.zero));
        properties.add(new DiagnosticsProperty<bool>("isComplex", this.isComplex, defaultValue: false));
        properties.add(new DiagnosticsProperty<bool>("willChange", this.willChange, defaultValue: false));
    }

}

