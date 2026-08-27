// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/_accessibility_evaluations.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Widgets;

public static partial class _accessibility_evaluationsLibrary
{
    internal static string _kAccessibilityEvaluationsDisabledErrorMessage = "Accessibility evaluations APIs are not enabled.\n\nAccessibility evaluations APIs are currently experimental. Do not use accessibility evaluations APIs in\nproduction applications or plugins published to pub.dev.\n\nTo try experimental accessibility evaluations APIs:\n1. Switch to Flutter's main release channel.\n2. Turn on the accessibility evaluations feature flag. (See flutter config --help)\n";
}

public class ViolationIo
{
    public virtual global::Doroti.Framework.Semantics.SemanticsNode node { get; private set; } = default!;
    public virtual string reason { get; private set; } = default!;

    public ViolationIo(global::Doroti.Framework.Semantics.SemanticsNode node, string reason)
    {
        this.node = node;
        this.reason = reason;
    }

}

public class EvaluationResultIo
{
    public virtual List<ViolationIo> violations { get; private set; } = default!;

    public EvaluationResultIo(List<ViolationIo> violations)
    {
        this.violations = violations;
    }

}

public abstract class AccessibilityEvaluationIo
{
    protected AccessibilityEvaluationIo()
    {
    }

    public virtual object evaluate(WidgetsBinding binding)
    {
        if (!global::Doroti.Framework.Foundation._featuresLibrary.isAccessibilityEvaluationsEnabled)
        {
            throw new NotSupportedException(_accessibility_evaluationsLibrary._kAccessibilityEvaluationsDisabledErrorMessage);
        }
        return _evaluate(binding);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal abstract object _evaluate(WidgetsBinding binding);
}

public class MinimumTapTargetEvaluationIo : AccessibilityEvaluationIo
{
    public virtual Size size { get; private set; } = default!;
    internal const double _kMinimumGapToBoundary = 0.001;

    public MinimumTapTargetEvaluationIo(Size size)
    {
        this.size = size;
    }

    internal override object _evaluate(WidgetsBinding binding)
    {
        var violations = new List<ViolationIo>();
        foreach (global::Doroti.Framework.Rendering.RenderView view in binding.renderViews)
        {
            violations.AddRange(_traverse(((global::Doroti.Framework.Rendering.RenderView)view).flutterView, view.owner!.semanticsOwner!.rootSemanticsNode!));
        }
        return new EvaluationResultIo(violations);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual List<ViolationIo> _traverse(DorotiView view, global::Doroti.Framework.Semantics.SemanticsNode node)
    {
        var violations = new List<ViolationIo>();
        node.visitChildren(((global::System.Func<global::Doroti.Framework.Semantics.SemanticsNode, bool>)((child) =>
        {
            violations.AddRange(_traverse(view, child));
            return true;
            throw new InvalidOperationException("Dart closure completed without a value.");
        })));
        if (((global::Doroti.Framework.Semantics.SemanticsNode)node).isMergedIntoParent)
        {
            return violations;
        }
        if (shouldSkipNode(node))
        {
            return violations;
        }
        global::Doroti.Ui.Rect paintBounds = ((global::Doroti.Ui.Rect)(object?)((global::Doroti.Framework.Semantics.SemanticsNode)node).rect);
        global::Doroti.Framework.Semantics.SemanticsNode? current = node;
        while ((current is not null))
        {
            Matrix4? transformLocal = ((global::Doroti.Framework.Semantics.SemanticsNode)current).transform;
            if ((transformLocal is not null))
            {
                paintBounds = MatrixUtils.transformRect(transformLocal, paintBounds);
            }
            if ((((global::Doroti.Framework.Semantics.SemanticsNode)current).flagsCollection.hasImplicitScrolling && MinimumTapTargetEvaluationIo._isAtBoundary(paintBounds, ((global::Doroti.Framework.Semantics.SemanticsNode)current).rect)))
            {
                return violations;
            }
            current = ((global::Doroti.Framework.Semantics.SemanticsNode)current).parent;
        }
        global::Doroti.Ui.Rect viewRect = ((global::Doroti.Ui.Rect)(object?)(Offset.zero & view.physicalSize));
        if (MinimumTapTargetEvaluationIo._isAtBoundary(paintBounds, viewRect))
        {
            return violations;
        }
        global::Doroti.Ui.Size candidateSize = ((global::Doroti.Ui.Size)(object?)(paintBounds.size / view.devicePixelRatio));
        if (((candidateSize.width < (this.size.width - global::Doroti.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance)) || (candidateSize.height < (this.size.height - global::Doroti.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance))))
        {
            violations.Add(new ViolationIo(node, $"{node}: expected tap target size of at least {this.size}, " + $"but found {candidateSize}\n"));
        }
        return violations;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static bool _isAtBoundary(Rect child, Rect parent)
    {
        if ((((((child.left - parent.left) > _kMinimumGapToBoundary) && ((parent.right - child.right) > _kMinimumGapToBoundary)) && ((child.top - parent.top) > _kMinimumGapToBoundary)) && ((parent.bottom - child.bottom) > _kMinimumGapToBoundary)))
        {
            return false;
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool shouldSkipNode(global::Doroti.Framework.Semantics.SemanticsNode node)
    {
        global::Doroti.Framework.Semantics.SemanticsData data = ((global::Doroti.Framework.Semantics.SemanticsData)(object?)node.getSemanticsData());
        if ((((!data.hasAction(SemanticsAction.longPress) && !data.hasAction(SemanticsAction.tap))) || ((global::Doroti.Framework.Semantics.SemanticsData)data).flagsCollection.isHidden))
        {
            return true;
        }
        if (((global::Doroti.Framework.Semantics.SemanticsData)data).flagsCollection.isLink)
        {
            return true;
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class LabeledTapTargetEvaluationIo : AccessibilityEvaluationIo
{
    public LabeledTapTargetEvaluationIo()
    {
    }

    internal override object _evaluate(WidgetsBinding binding)
    {
        var violations = new List<ViolationIo>();
        foreach (global::Doroti.Framework.Rendering.RenderView view in binding.renderViews)
        {
            violations.AddRange(_traverse(view.owner!.semanticsOwner!.rootSemanticsNode!));
        }
        return new EvaluationResultIo(violations);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual List<ViolationIo> _traverse(global::Doroti.Framework.Semantics.SemanticsNode node)
    {
        var violations = new List<ViolationIo>();
        node.visitChildren(((global::System.Func<global::Doroti.Framework.Semantics.SemanticsNode, bool>)((child) =>
        {
            violations.AddRange(_traverse(child));
            return true;
            throw new InvalidOperationException("Dart closure completed without a value.");
        })));
        if ((((((global::Doroti.Framework.Semantics.SemanticsNode)node).isMergedIntoParent || ((global::Doroti.Framework.Semantics.SemanticsNode)node).isInvisible) || ((global::Doroti.Framework.Semantics.SemanticsNode)node).flagsCollection.isHidden) || ((global::Doroti.Framework.Semantics.SemanticsNode)node).flagsCollection.isTextField))
        {
            return violations;
        }
        global::Doroti.Framework.Semantics.SemanticsData data = ((global::Doroti.Framework.Semantics.SemanticsData)(object?)node.getSemanticsData());
        if ((!data.hasAction(SemanticsAction.longPress) && !data.hasAction(SemanticsAction.tap)))
        {
            return violations;
        }
        if ((((((global::Doroti.Framework.Semantics.SemanticsData)data).label.Length == 0)) && ((((global::Doroti.Framework.Semantics.SemanticsData)data).tooltip.Length == 0))))
        {
            violations.Add(new ViolationIo(node, $"{node}: expected tappable node to have semantic label, " + "but none was found."));
        }
        return violations;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public abstract class _ContrastEvaluation___accessibility_evaluations : AccessibilityEvaluationIo
{
    internal static double _kContrastTolerance = -0.01;

    internal _ContrastEvaluation___accessibility_evaluations()
    {
    }

    internal async override Future<EvaluationResultIo> _evaluate(WidgetsBinding binding)
    {
        var violations = new List<ViolationIo>();
        foreach (global::Doroti.Framework.Rendering.RenderView renderView in binding.renderViews)
        {
            var layer = ((global::Doroti.Framework.Rendering.OffsetLayer?)(object?)renderView.debugLayer!)!;
            global::Doroti.Framework.Semantics.SemanticsNode root = renderView.owner!.semanticsOwner!.rootSemanticsNode!;
            double ratio = (1L / ((global::Doroti.Framework.Rendering.RenderView)renderView).flutterView.devicePixelRatio);
            global::Doroti.Ui.Image image = await layer.toImage(((global::Doroti.Framework.Rendering.RenderView)renderView).paintBounds, pixelRatio: ratio);
            ByteData byteData = (await image.toByteData())!;
            violations.AddRange((await _evaluateNode(root, image, byteData, renderView)).Cast<ViolationIo>());
            image.dispose();
        }
        return new EvaluationResultIo(violations);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal async virtual Future<List<ViolationIo>> _evaluateNode(global::Doroti.Framework.Semantics.SemanticsNode node, global::Doroti.Ui.Image image, ByteData byteData, global::Doroti.Framework.Rendering.RenderView renderView)
    {
        var violations = new List<ViolationIo>();
        if (_shouldSkipNodeTraversal(node))
        {
            return violations;
        }
        global::Doroti.Framework.Semantics.SemanticsData data = ((global::Doroti.Framework.Semantics.SemanticsData)(object?)node.getSemanticsData());
        var children = new List<global::Doroti.Framework.Semantics.SemanticsNode>();
        node.visitChildren(((global::System.Func<global::Doroti.Framework.Semantics.SemanticsNode, bool>)((child) =>
        {
            children.Add(child);
            return true;
            throw new InvalidOperationException("Dart closure completed without a value.");
        })));
        foreach (var childLocal in children)
        {
            violations.AddRange((await _evaluateNode(childLocal, image, byteData, renderView)).Cast<ViolationIo>());
        }
        if (_shouldSkipNodeEvaluation(data))
        {
            return violations;
        }
        return await evaluateNodeContent(node, data, image, byteData, renderView);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _shouldSkipNodeTraversal(global::Doroti.Framework.Semantics.SemanticsNode node)
    {
        var isDisabled = (object.Equals(((global::Doroti.Framework.Semantics.SemanticsNode)node).flagsCollection.isEnabled, Tristate.isFalse));
        return (((((global::Doroti.Framework.Semantics.SemanticsNode)node).isInvisible || ((global::Doroti.Framework.Semantics.SemanticsNode)node).isMergedIntoParent) || ((global::Doroti.Framework.Semantics.SemanticsNode)node).flagsCollection.isHidden) || isDisabled);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal abstract bool _shouldSkipNodeEvaluation(global::Doroti.Framework.Semantics.SemanticsData data);
    public abstract Future<List<ViolationIo>> evaluateNodeContent(global::Doroti.Framework.Semantics.SemanticsNode node, global::Doroti.Framework.Semantics.SemanticsData data, global::Doroti.Ui.Image image, ByteData byteData, global::Doroti.Framework.Rendering.RenderView renderView);
    internal virtual bool _isNodeOffScreen(Rect paintBounds, DorotiView window)
    {
        global::Doroti.Ui.Size windowLogicalSize = ((global::Doroti.Ui.Size)(object?)(window.physicalSize / window.devicePixelRatio));
        return ((((paintBounds.top < -50.0) || (paintBounds.left < -50.0)) || (paintBounds.bottom > (windowLogicalSize.height + 50.0))) || (paintBounds.right > (windowLogicalSize.width + 50.0)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class MinimumTextContrastEvaluationIo : _ContrastEvaluation___accessibility_evaluations
{
    public virtual double minNormalTextContrastRatio { get; private set; } = default!;
    public virtual double minLargeTextContrastRatio { get; private set; } = default!;
    public const long kLargeTextMinimumSize = 18L;
    public const long kBoldTextMinimumSize = 14L;
    public const double kMinimumRatioNormalText = 4.5;
    public const double kMinimumRatioLargeText = 3.0;
    internal const double _kDefaultFontSize = 12.0;

    public MinimumTextContrastEvaluationIo(double minNormalTextContrastRatio, double minLargeTextContrastRatio)
    {
        this.minNormalTextContrastRatio = minNormalTextContrastRatio;
        this.minLargeTextContrastRatio = minLargeTextContrastRatio;
    }

    internal override bool _shouldSkipNodeEvaluation(global::Doroti.Framework.Semantics.SemanticsData data) => DartRuntimePrimitives.ConvertValue<bool>((((global::Doroti.Framework.Semantics.SemanticsData)data).flagsCollection.scopesRoute || (((((global::Doroti.Framework.Semantics.SemanticsData)data).label.Trim().Length == 0) && (((global::Doroti.Framework.Semantics.SemanticsData)data).value.Trim().Length == 0)))));
    public async override Future<List<ViolationIo>> evaluateNodeContent(global::Doroti.Framework.Semantics.SemanticsNode node, global::Doroti.Framework.Semantics.SemanticsData data, global::Doroti.Ui.Image image, ByteData byteData, global::Doroti.Framework.Rendering.RenderView renderView)
    {
        var violations = new List<ViolationIo>();
        string text = ((((global::Doroti.Framework.Semantics.SemanticsData)data).label.Length == 0) ? ((global::Doroti.Framework.Semantics.SemanticsData)data).value : ((global::Doroti.Framework.Semantics.SemanticsData)data).label);
        IEnumerable<Element> elements = _accessibility_evaluationsLibrary._collectElementsByText(WidgetsBinding.instance.rootElement!, text);
        foreach (var element in elements)
        {
            violations.AddRange((await _evaluateElement(node, element, image, byteData, renderView)).Cast<ViolationIo>());
        }
        return violations;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal async virtual Future<List<ViolationIo>> _evaluateElement(global::Doroti.Framework.Semantics.SemanticsNode node, Element element, global::Doroti.Ui.Image image, ByteData byteData, global::Doroti.Framework.Rendering.RenderView renderView)
    {
        bool isBold = default!;
        double? fontSizeLocal = default!;
        global::Doroti.Ui.Rect screenBounds = default!;
        global::Doroti.Ui.Rect paintBoundsWithOffset = default!;
        global::Doroti.Framework.Rendering.RenderObject? renderBox = ((Element)element).renderObject;
        if ((renderBox is not global::Doroti.Framework.Rendering.RenderBox))
        {
            throw new InvalidOperationException($"Unexpected renderObject type: {renderBox}");
        }
        Matrix4 globalTransform = ((Matrix4)(object?)((global::Doroti.Framework.Rendering.RenderBox)renderBox).getTransformTo(((global::Doroti.Framework.Rendering.RenderObject)(object)null)));
        paintBoundsWithOffset = MatrixUtils.transformRect(globalTransform, ((global::Doroti.Framework.Rendering.RenderBox)((global::Doroti.Framework.Rendering.RenderBox)renderBox)).paintBounds.inflate(4.0));
        var rootTransform = Matrix4.identity();
        renderView.applyPaintTransform(((global::Doroti.Framework.Rendering.RenderBox?)((dynamic)renderView).child)!, rootTransform);
        rootTransform.multiply(globalTransform);
        screenBounds = MatrixUtils.transformRect(rootTransform, ((global::Doroti.Framework.Rendering.RenderBox)((global::Doroti.Framework.Rendering.RenderBox)renderBox)).paintBounds);
        global::Doroti.Ui.Rect nodeBounds = ((global::Doroti.Ui.Rect)(object?)((global::Doroti.Framework.Semantics.SemanticsNode)node).rect);
        global::Doroti.Framework.Semantics.SemanticsNode? current = node;
        while ((current is not null))
        {
            Matrix4? transformLocal = ((global::Doroti.Framework.Semantics.SemanticsNode)current).transform;
            if ((transformLocal is not null))
            {
                nodeBounds = MatrixUtils.transformRect(transformLocal, nodeBounds);
            }
            current = ((global::Doroti.Framework.Semantics.SemanticsNode)current).parent;
        }
        global::Doroti.Ui.Rect intersection = ((global::Doroti.Ui.Rect)(object?)nodeBounds.intersect(screenBounds));
        if (((intersection.width <= 0L) || (intersection.height <= 0L)))
        {
            return new List<ViolationIo>();
        }
        Widget widgetLocal = ((Element)element).widget;
        DefaultTextStyle defaultTextStyle = ((DefaultTextStyle)(object?)DefaultTextStyle.of(element));
        if ((widgetLocal is Text))
        {
            Text widget__14684__as14793 = (Text)widgetLocal;
            global::Doroti.Framework.Painting.TextStyle? styleLocal = ((Text)((Text)widget__14684__as14793)).style;
            global::Doroti.Framework.Painting.TextStyle effectiveTextStyle = (((styleLocal is null) || ((global::Doroti.Framework.Painting.TextStyle)styleLocal).inherit) ? ((DefaultTextStyle)defaultTextStyle).style.merge(((Text)((Text)widget__14684__as14793)).style) : styleLocal);
            isBold = (object.Equals(((global::Doroti.Framework.Painting.TextStyle)effectiveTextStyle).fontWeight, FontWeight.bold));
            fontSizeLocal = ((global::Doroti.Framework.Painting.TextStyle)effectiveTextStyle).fontSize;
        }
        else
        {
            if ((widgetLocal is EditableText))
            {
                EditableText widget__14684__as15130 = (EditableText)widgetLocal;
                isBold = (object.Equals(((EditableText)((EditableText)widget__14684__as15130)).style.fontWeight, FontWeight.bold));
                fontSizeLocal = ((EditableText)((EditableText)widget__14684__as15130)).style.fontSize;
            }
            else
            {
                throw new InvalidOperationException($"Unexpected widget type: {DartRuntimePrimitives.RuntimeType(widgetLocal)}");
            }
        }
        if (_isNodeOffScreen(paintBoundsWithOffset, ((global::Doroti.Framework.Rendering.RenderView)renderView).flutterView))
        {
            return new List<ViolationIo>();
        }
        DartMap<global::Doroti.Ui.Color, long> colorHistogram = _accessibility_evaluationsLibrary._colorsWithinRect(byteData, paintBoundsWithOffset, DartRuntimePrimitives.RequireValue(((global::Doroti.Ui.Image)image).width), DartRuntimePrimitives.RequireValue(((global::Doroti.Ui.Image)image).height)).cast<global::Doroti.Ui.Color, long>();
        if (!System.Linq.Enumerable.Any(colorHistogram))
        {
            return new List<ViolationIo>();
        }
        var report = _ContrastReport___accessibility_evaluations.Create(colorHistogram);
        double contrastRatioLocal = report.contrastRatio();
        double targetContrastRatio = _targetContrastRatio(fontSizeLocal, bold: isBold);
        if (((contrastRatioLocal - targetContrastRatio) >= _ContrastEvaluation___accessibility_evaluations._kContrastTolerance))
        {
            return new List<ViolationIo>();
        }
        return new List<ViolationIo> { new ViolationIo(node, $"{node}:\n" + $"Expected contrast ratio of at least {targetContrastRatio} " + $"but found {contrastRatioLocal.toStringAsFixed(2L)} " + $"for a font size of {fontSizeLocal}.\n" + "The computed colors were:\n" + $"light - {((_ContrastReport___accessibility_evaluations)report).lightColor}, dark - {((_ContrastReport___accessibility_evaluations)report).darkColor}\n" + "See also: " + "https://www.w3.org/TR/UNDERSTANDING-WCAG20/visual-audio-contrast-contrast.html") };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _targetContrastRatio(double? fontSize, bool bold)
    {
        double fontSizeOrDefault = (fontSize ?? _kDefaultFontSize);
        if ((((bold && (fontSizeOrDefault >= kBoldTextMinimumSize))) || (fontSizeOrDefault >= kLargeTextMinimumSize)))
        {
            return this.minLargeTextContrastRatio;
        }
        return this.minNormalTextContrastRatio;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class MinimumNonTextContrastEvaluationIo : _ContrastEvaluation___accessibility_evaluations
{
    internal const double _kMinimumRatioNonText = 3.0;

    public MinimumNonTextContrastEvaluationIo()
    {
    }

    internal override bool _shouldSkipNodeEvaluation(global::Doroti.Framework.Semantics.SemanticsData data)
    {
        if (((global::Doroti.Framework.Semantics.SemanticsData)data).flagsCollection.scopesRoute)
        {
            return true;
        }
        bool isControl = ((((((((global::Doroti.Framework.Semantics.SemanticsData)data).flagsCollection.isButton || ((global::Doroti.Framework.Semantics.SemanticsData)data).flagsCollection.isSlider) || ((global::Doroti.Framework.Semantics.SemanticsData)data).flagsCollection.isTextField) || (!object.Equals(((global::Doroti.Framework.Semantics.SemanticsData)data).flagsCollection.isChecked, CheckedState.none))) || (!object.Equals(((global::Doroti.Framework.Semantics.SemanticsData)data).flagsCollection.isToggled, Tristate.none))) || data.hasAction(SemanticsAction.tap)) || data.hasAction(SemanticsAction.longPress));
        return !isControl;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public async override Future<List<ViolationIo>> evaluateNodeContent(global::Doroti.Framework.Semantics.SemanticsNode node, global::Doroti.Framework.Semantics.SemanticsData data, global::Doroti.Ui.Image image, ByteData byteData, global::Doroti.Framework.Rendering.RenderView renderView)
    {
        var violations = new List<ViolationIo>();
        global::Doroti.Ui.Rect nodeBounds = ((global::Doroti.Ui.Rect)(object?)((global::Doroti.Framework.Semantics.SemanticsNode)node).rect);
        global::Doroti.Framework.Semantics.SemanticsNode? current = node;
        while ((current is not null))
        {
            Matrix4? transformLocal = ((global::Doroti.Framework.Semantics.SemanticsNode)current).transform;
            if (((transformLocal is not null) && (((global::Doroti.Framework.Semantics.SemanticsNode)current).parent is not null)))
            {
                nodeBounds = MatrixUtils.transformRect(transformLocal, nodeBounds);
            }
            current = ((global::Doroti.Framework.Semantics.SemanticsNode)current).parent;
        }
        double devicePixelRatioLocal = ((global::Doroti.Framework.Rendering.RenderView)renderView).flutterView.devicePixelRatio;
        var logicalBounds = global::Doroti.Ui.Rect.fromLTRB((nodeBounds.left / devicePixelRatioLocal), (nodeBounds.top / devicePixelRatioLocal), (nodeBounds.right / devicePixelRatioLocal), (nodeBounds.bottom / devicePixelRatioLocal));
        global::Doroti.Ui.Rect inflatedBounds = ((global::Doroti.Ui.Rect)(object?)logicalBounds.inflate(4.0));
        if (_isNodeOffScreen(inflatedBounds, ((global::Doroti.Framework.Rendering.RenderView)renderView).flutterView))
        {
            return violations;
        }
        DartMap<global::Doroti.Ui.Color, long> colorHistogram = _accessibility_evaluationsLibrary._colorsWithinRect(byteData, inflatedBounds, DartRuntimePrimitives.RequireValue(((global::Doroti.Ui.Image)image).width), DartRuntimePrimitives.RequireValue(((global::Doroti.Ui.Image)image).height)).cast<global::Doroti.Ui.Color, long>();
        if ((checked((long)(colorHistogram.Count)) <= 1L))
        {
            return violations;
        }
        var report = _ContrastReport___accessibility_evaluations.Create(colorHistogram);
        double contrastRatioLocal = report.contrastRatio();
        if (((contrastRatioLocal - _kMinimumRatioNonText) >= _ContrastEvaluation___accessibility_evaluations._kContrastTolerance))
        {
            return violations;
        }
        violations.Add(new ViolationIo(node, $"{node}:\n" + $"Expected non-text control contrast ratio of at least {_kMinimumRatioNonText.toStringAsFixed(1L)} " + $"but found {contrastRatioLocal.toStringAsFixed(2L)}.\n" + "The computed colors were:\n" + $"light - {((_ContrastReport___accessibility_evaluations)report).lightColor}, dark - {((_ContrastReport___accessibility_evaluations)report).darkColor}\n" + "See also: " + "https://www.w3.org/WAI/WCAG22/Understanding/non-text-contrast.html"));
        return violations;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _ContrastReport___accessibility_evaluations
{
    public virtual Color lightColor { get; private set; } = default!;
    public virtual Color darkColor { get; private set; } = default!;

    internal static _ContrastReport___accessibility_evaluations Create(DartMap<Color, long> colorHistogram)
    {
        var totalLightness = 0.0;
        var count = 0L;
        foreach (MapEntry<global::Doroti.Ui.Color, long> entry in colorHistogram.entries)
        {
            totalLightness += (global::Doroti.Framework.Painting.HSLColor.CreateFromColor(entry.key).lightness * entry.value);
            count += entry.value;
        }
        double averageLightness = (totalLightness / count);
        DartRuntimePrimitives.Assert(() => !double.IsNaN(averageLightness));
        MapEntry<global::Doroti.Ui.Color, long>? lightColor = default!;
        MapEntry<global::Doroti.Ui.Color, long>? darkColor = default!;
        foreach (MapEntry<global::Doroti.Ui.Color, long> entryLocal in colorHistogram.entries)
        {
            double lightnessLocal = global::Doroti.Framework.Painting.HSLColor.CreateFromColor(entryLocal.key).lightness;
            long countLocal = entryLocal.value;
            if ((lightnessLocal <= averageLightness))
            {
                if ((countLocal > ((darkColor?.value ?? 0L))))
                {
                    darkColor = entryLocal;
                }
            }
            else
            {
                if ((countLocal > ((lightColor?.value ?? 0L))))
                {
                    lightColor = entryLocal;
                }
            }
        }
        return new _ContrastReport___accessibility_evaluations((lightColor?.key ?? DartRuntimePrimitives.RequireValue(darkColor).key), (darkColor?.key ?? DartRuntimePrimitives.RequireValue(lightColor).key));
    }

    internal _ContrastReport___accessibility_evaluations(Color lightColor, Color darkColor)
    {
        this.lightColor = lightColor;
        this.darkColor = darkColor;
    }

    public virtual double contrastRatio() => DartRuntimePrimitives.ConvertValue<double>((((this.lightColor.computeLuminance() + 0.05)) / ((this.darkColor.computeLuminance() + 0.05))));
}

public static partial class _accessibility_evaluationsLibrary
{
    internal static DartMap<Color, long> _colorsWithinRect(ByteData data, Rect paintBounds, long width, long height)
    {
        global::Doroti.Ui.Rect truePaintBounds = ((global::Doroti.Ui.Rect)(object?)paintBounds.intersect(global::Doroti.Ui.Rect.fromLTWH(0.0, 0.0, width.toDouble(), height.toDouble())));
        long leftX = truePaintBounds.left.floor();
        long rightX = truePaintBounds.right.ceil();
        long topY = truePaintBounds.top.floor();
        long bottomY = truePaintBounds.bottom.ceil();
        var rgbaToCount = new DartMap<long, long>();
        long getPixel(ByteData data, long x, long y)
        {
            long offset = ((((y * width) + x)) * 4L);
            return data.getUint32(offset);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        for (var xLocal = leftX; (xLocal < rightX); xLocal++)
        {
            for (var yLocal = topY; (yLocal < bottomY); yLocal++)
            {
                rgbaToCount.update(getPixel(data, xLocal, yLocal), ((count) => (count + 1L)), ifAbsent: (() => 1L));
            }
        }
        return rgbaToCount.map<long, long, Color, long>(((rgba, count) =>
        {
            long argb = (((rgba << (int)(24L))) | (((rgba >> (int)(8L))) & 4294967295L));
            return new MapEntry<global::Doroti.Ui.Color, long>(new global::Doroti.Ui.Color(argb), count);
            throw new InvalidOperationException("Dart closure completed without a value.");
        }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class _accessibility_evaluationsLibrary
{
    internal static IEnumerable<Element> _collectElementsByText(Element root, string text)
    {
        var result = new List<Element>();
        root.visitChildren(((global::System.Action<Element>)((child) =>
        {
            if (((((Element)child).widget is Text) && ((((Text?)(object?)((Element)child).widget)!).data == text)))
            {
                result.Add(child);
            }
            result.AddRange(_accessibility_evaluationsLibrary._collectElementsByText(child, text).Cast<Element>());
        })));
        return ((IEnumerable<Element>)(object?)result);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class _accessibility_evaluationsLibrary
{
    internal static long _scrollingActions = ((((FoundationRuntimePorts.EnumIndex(SemanticsAction.scrollUp) | FoundationRuntimePorts.EnumIndex(SemanticsAction.scrollDown)) | FoundationRuntimePorts.EnumIndex(SemanticsAction.scrollLeft)) | FoundationRuntimePorts.EnumIndex(SemanticsAction.scrollRight)) | FoundationRuntimePorts.EnumIndex(SemanticsAction.scrollToOffset));
}

public static partial class _accessibility_evaluationsLibrary
{
    internal static bool _isImportantForAccessibility(global::Doroti.Framework.Semantics.SemanticsNode node)
    {
        if (((global::Doroti.Framework.Semantics.SemanticsNode)node).isMergedIntoParent)
        {
            return false;
        }
        global::Doroti.Framework.Semantics.SemanticsData data = ((global::Doroti.Framework.Semantics.SemanticsData)(object?)node.getSemanticsData());
        if (((global::Doroti.Framework.Semantics.SemanticsData)data).flagsCollection.scopesRoute)
        {
            return false;
        }
        var hasNonScrollingAction = ((((global::Doroti.Framework.Semantics.SemanticsData)data).actions & ~_accessibility_evaluationsLibrary._scrollingActions) != 0L);
        if (hasNonScrollingAction)
        {
            return true;
        }
        bool hasImportantFlag = ((((((((!object.Equals(((global::Doroti.Framework.Semantics.SemanticsData)data).flagsCollection.isChecked, CheckedState.none)) || (!object.Equals(((global::Doroti.Framework.Semantics.SemanticsData)data).flagsCollection.isToggled, Tristate.none))) || (!object.Equals(((global::Doroti.Framework.Semantics.SemanticsData)data).flagsCollection.isEnabled, Tristate.none))) || ((global::Doroti.Framework.Semantics.SemanticsData)data).flagsCollection.isButton) || ((global::Doroti.Framework.Semantics.SemanticsData)data).flagsCollection.isTextField) || (!object.Equals(((global::Doroti.Framework.Semantics.SemanticsData)data).flagsCollection.isFocused, Tristate.none))) || ((global::Doroti.Framework.Semantics.SemanticsData)data).flagsCollection.isSlider) || ((global::Doroti.Framework.Semantics.SemanticsData)data).flagsCollection.isInMutuallyExclusiveGroup);
        if (hasImportantFlag)
        {
            return true;
        }
        bool hasContent = ((((((global::Doroti.Framework.Semantics.SemanticsData)data).label.Length != 0) || (((global::Doroti.Framework.Semantics.SemanticsData)data).value.Length != 0)) || (((global::Doroti.Framework.Semantics.SemanticsData)data).hint.Length != 0)) || (((global::Doroti.Framework.Semantics.SemanticsData)data).tooltip.Length != 0));
        if (hasContent)
        {
            return true;
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class UnlabeledLeafNodeEvaluationIo : AccessibilityEvaluationIo
{
    public UnlabeledLeafNodeEvaluationIo()
    {
    }

    internal override object _evaluate(WidgetsBinding binding)
    {
        var violations = new List<ViolationIo>();
        foreach (global::Doroti.Framework.Rendering.RenderView view in binding.renderViews)
        {
            violations.AddRange(_traverse(view.owner!.semanticsOwner!.rootSemanticsNode!));
        }
        return new EvaluationResultIo(violations);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual List<ViolationIo> _traverse(global::Doroti.Framework.Semantics.SemanticsNode node)
    {
        var violations = new List<ViolationIo>();
        var hasChildren = false;
        node.visitChildren(((global::System.Func<global::Doroti.Framework.Semantics.SemanticsNode, bool>)((child) =>
        {
            hasChildren = true;
            violations.AddRange(_traverse(child));
            return true;
            throw new InvalidOperationException("Dart closure completed without a value.");
        })));
        if ((((global::Doroti.Framework.Semantics.SemanticsNode)node).isInvisible || ((global::Doroti.Framework.Semantics.SemanticsNode)node).flagsCollection.isHidden))
        {
            return violations;
        }
        if ((hasChildren && !((global::Doroti.Framework.Semantics.SemanticsNode)node).mergeAllDescendantsIntoThisNode))
        {
            return violations;
        }
        if (!_accessibility_evaluationsLibrary._isImportantForAccessibility(node))
        {
            return violations;
        }
        global::Doroti.Framework.Semantics.SemanticsData data = ((global::Doroti.Framework.Semantics.SemanticsData)(object?)node.getSemanticsData());
        if (((((((global::Doroti.Framework.Semantics.SemanticsData)data).label.Trim().Length == 0) && (((global::Doroti.Framework.Semantics.SemanticsData)data).value.Trim().Length == 0)) && (((global::Doroti.Framework.Semantics.SemanticsData)data).hint.Trim().Length == 0)) && (((global::Doroti.Framework.Semantics.SemanticsData)data).tooltip.Trim().Length == 0)))
        {
            violations.Add(new ViolationIo(node, $"{node}: expected leaf semantics node to have a label, value, hint, or tooltip, " + "but none was found."));
        }
        return violations;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class TitleEvaluationIo : AccessibilityEvaluationIo
{
    public TitleEvaluationIo()
    {
    }

    internal override object _evaluate(WidgetsBinding binding)
    {
        var violations = new List<ViolationIo>();
        if (((((WidgetsBinding)binding).rootElement is not null) && !_hasTitleWidget(((WidgetsBinding)binding).rootElement!)))
        {
            global::Doroti.Framework.Semantics.SemanticsNode rootNode = binding.renderViews.First().owner!.semanticsOwner!.rootSemanticsNode!;
            violations.Add(new ViolationIo(rootNode, "Expected to find at least one Title widget, but none was found."));
        }
        return new EvaluationResultIo(violations);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _hasTitleWidget(Element element)
    {
        if ((((Element)element).widget is Title))
        {
            return true;
        }
        var found = false;
        element.visitChildren(((global::System.Action<Element>)((child) =>
        {
            if (!found)
            {
                found = _hasTitleWidget(child);
            }
        })));
        return found;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

