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

namespace Doroti.Generated.Framework.Widgets;

public static partial class _accessibility_evaluationsLibrary
{
    internal static string _kAccessibilityEvaluationsDisabledErrorMessage = "Accessibility evaluations APIs are not enabled.\n\nAccessibility evaluations APIs are currently experimental. Do not use accessibility evaluations APIs in\nproduction applications or plugins published to pub.dev.\n\nTo try experimental accessibility evaluations APIs:\n1. Switch to Flutter's main release channel.\n2. Turn on the accessibility evaluations feature flag. (See flutter config --help)\n";
}

public class ViolationIo
{
    public virtual global::Doroti.Generated.Framework.Semantics.SemanticsNode node { get; private set; } = default!;
    public virtual string reason { get; private set; } = default!;

    public ViolationIo(global::Doroti.Generated.Framework.Semantics.SemanticsNode node, string reason)
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
        if (!global::Doroti.Generated.Framework.Foundation._featuresLibrary.isAccessibilityEvaluationsEnabled)
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
        var violations__3192 = new List<ViolationIo>();
        foreach (global::Doroti.Generated.Framework.Rendering.RenderView view__3246 in binding.renderViews)
        {
            violations__3192.AddRange(_traverse(((global::Doroti.Generated.Framework.Rendering.RenderView)view__3246).flutterView, view__3246.owner!.semanticsOwner!.rootSemanticsNode!));
        }
        return new EvaluationResultIo(violations__3192);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual List<ViolationIo> _traverse(DorotiView view, global::Doroti.Generated.Framework.Semantics.SemanticsNode node)
    {
        var violations__3530 = new List<ViolationIo>();
        node.visitChildren(((global::System.Func<global::Doroti.Generated.Framework.Semantics.SemanticsNode, bool>)((child) => {
violations__3530.AddRange(_traverse(view, child));
return true;
throw new InvalidOperationException("Dart closure completed without a value.");
})));
        if (((global::Doroti.Generated.Framework.Semantics.SemanticsNode)node).isMergedIntoParent)
        {
            return violations__3530;
        }
        if (shouldSkipNode(node))
        {
            return violations__3530;
        }
        global::Doroti.Ui.Rect paintBounds__3819 = ((global::Doroti.Ui.Rect)(object?)((global::Doroti.Generated.Framework.Semantics.SemanticsNode)node).rect);
        global::Doroti.Generated.Framework.Semantics.SemanticsNode? current__3863 = node;
        while ((current__3863 is not null))
        {
            Matrix4? transform__3931 = ((global::Doroti.Generated.Framework.Semantics.SemanticsNode)current__3863).transform;
            if ((transform__3931 is not null))
            {
                paintBounds__3819 = MatrixUtils.transformRect(transform__3931, paintBounds__3819);
            }
            if ((((global::Doroti.Generated.Framework.Semantics.SemanticsNode)current__3863).flagsCollection.hasImplicitScrolling && MinimumTapTargetEvaluationIo._isAtBoundary(paintBounds__3819, ((global::Doroti.Generated.Framework.Semantics.SemanticsNode)current__3863).rect)))
            {
                return violations__3530;
            }
            current__3863 = ((global::Doroti.Generated.Framework.Semantics.SemanticsNode)current__3863).parent;
        }
        global::Doroti.Ui.Rect viewRect__4397 = ((global::Doroti.Ui.Rect)(object?)(Offset.zero & view.physicalSize));
        if (MinimumTapTargetEvaluationIo._isAtBoundary(paintBounds__3819, viewRect__4397))
        {
            return violations__3530;
        }
        global::Doroti.Ui.Size candidateSize__4573 = ((global::Doroti.Ui.Size)(object?)(paintBounds__3819.size / view.devicePixelRatio));
        if (((candidateSize__4573.width < (this.size.width - global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance)) || (candidateSize__4573.height < (this.size.height - global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance))))
        {
            violations__3530.Add(new ViolationIo(node, $"{node}: expected tap target size of at least {this.size}, " + $"but found {candidateSize__4573}\n"));
        }
        return violations__3530;
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

    public virtual bool shouldSkipNode(global::Doroti.Generated.Framework.Semantics.SemanticsNode node)
    {
        global::Doroti.Generated.Framework.Semantics.SemanticsData data__5576 = ((global::Doroti.Generated.Framework.Semantics.SemanticsData)(object?)node.getSemanticsData());
        if ((((!data__5576.hasAction(SemanticsAction.longPress) && !data__5576.hasAction(SemanticsAction.tap))) || ((global::Doroti.Generated.Framework.Semantics.SemanticsData)data__5576).flagsCollection.isHidden))
        {
            return true;
        }
        if (((global::Doroti.Generated.Framework.Semantics.SemanticsData)data__5576).flagsCollection.isLink)
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
        var violations__6393 = new List<ViolationIo>();
        foreach (global::Doroti.Generated.Framework.Rendering.RenderView view__6448 in binding.renderViews)
        {
            violations__6393.AddRange(_traverse(view__6448.owner!.semanticsOwner!.rootSemanticsNode!));
        }
        return new EvaluationResultIo(violations__6393);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual List<ViolationIo> _traverse(global::Doroti.Generated.Framework.Semantics.SemanticsNode node)
    {
        var violations__6676 = new List<ViolationIo>();
        node.visitChildren(((global::System.Func<global::Doroti.Generated.Framework.Semantics.SemanticsNode, bool>)((child) => {
violations__6676.AddRange(_traverse(child));
return true;
throw new InvalidOperationException("Dart closure completed without a value.");
})));
        if ((((((global::Doroti.Generated.Framework.Semantics.SemanticsNode)node).isMergedIntoParent || ((global::Doroti.Generated.Framework.Semantics.SemanticsNode)node).isInvisible) || ((global::Doroti.Generated.Framework.Semantics.SemanticsNode)node).flagsCollection.isHidden) || ((global::Doroti.Generated.Framework.Semantics.SemanticsNode)node).flagsCollection.isTextField))
        {
            return violations__6676;
        }
        global::Doroti.Generated.Framework.Semantics.SemanticsData data__7024 = ((global::Doroti.Generated.Framework.Semantics.SemanticsData)(object?)node.getSemanticsData());
        if ((!data__7024.hasAction(SemanticsAction.longPress) && !data__7024.hasAction(SemanticsAction.tap)))
        {
            return violations__6676;
        }
        if ((((((global::Doroti.Generated.Framework.Semantics.SemanticsData)data__7024).label.Length == 0)) && ((((global::Doroti.Generated.Framework.Semantics.SemanticsData)data__7024).tooltip.Length == 0))))
        {
            violations__6676.Add(new ViolationIo(node, $"{node}: expected tappable node to have semantic label, " + "but none was found."));
        }
        return violations__6676;
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
        var violations__7844 = new List<ViolationIo>();
        foreach (global::Doroti.Generated.Framework.Rendering.RenderView renderView__7898 in binding.renderViews)
        {
            var layer__7947 = ((global::Doroti.Generated.Framework.Rendering.OffsetLayer?)(object?)renderView__7898.debugLayer!)!;
            global::Doroti.Generated.Framework.Semantics.SemanticsNode root__8020 = renderView__7898.owner!.semanticsOwner!.rootSemanticsNode!;
            double ratio__8101 = (1L / ((global::Doroti.Generated.Framework.Rendering.RenderView)renderView__7898).flutterView.devicePixelRatio);
            global::Doroti.Ui.Image image__8175 = await layer__7947.toImage(((global::Doroti.Generated.Framework.Rendering.RenderView)renderView__7898).paintBounds, pixelRatio: ratio__8101);
            ByteData byteData__8268 = (await image__8175.toByteData())!;
            violations__7844.AddRange((await _evaluateNode(root__8020, image__8175, byteData__8268, renderView__7898)).Cast<ViolationIo>());
            image__8175.dispose();
        }
        return new EvaluationResultIo(violations__7844);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal async virtual Future<List<ViolationIo>> _evaluateNode(global::Doroti.Generated.Framework.Semantics.SemanticsNode node, global::Doroti.Ui.Image image, ByteData byteData, global::Doroti.Generated.Framework.Rendering.RenderView renderView)
    {
        var violations__8622 = new List<ViolationIo>();
        if (_shouldSkipNodeTraversal(node))
        {
            return violations__8622;
        }
        global::Doroti.Generated.Framework.Semantics.SemanticsData data__8749 = ((global::Doroti.Generated.Framework.Semantics.SemanticsData)(object?)node.getSemanticsData());
        var children__8791 = new List<global::Doroti.Generated.Framework.Semantics.SemanticsNode>();
        node.visitChildren(((global::System.Func<global::Doroti.Generated.Framework.Semantics.SemanticsNode, bool>)((child) => {
children__8791.Add(child);
return true;
throw new InvalidOperationException("Dart closure completed without a value.");
})));
        foreach (var child__8937 in children__8791)
        {
            violations__8622.AddRange((await _evaluateNode(child__8937, image, byteData, renderView)).Cast<ViolationIo>());
        }
        if (_shouldSkipNodeEvaluation(data__8749))
        {
            return violations__8622;
        }
        return await evaluateNodeContent(node, data__8749, image, byteData, renderView);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _shouldSkipNodeTraversal(global::Doroti.Generated.Framework.Semantics.SemanticsNode node)
    {
        var isDisabled__9264 = (object.Equals(((global::Doroti.Generated.Framework.Semantics.SemanticsNode)node).flagsCollection.isEnabled, Tristate.isFalse));
        return (((((global::Doroti.Generated.Framework.Semantics.SemanticsNode)node).isInvisible || ((global::Doroti.Generated.Framework.Semantics.SemanticsNode)node).isMergedIntoParent) || ((global::Doroti.Generated.Framework.Semantics.SemanticsNode)node).flagsCollection.isHidden) || isDisabled__9264);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal abstract bool _shouldSkipNodeEvaluation(global::Doroti.Generated.Framework.Semantics.SemanticsData data);
    public abstract Future<List<ViolationIo>> evaluateNodeContent(global::Doroti.Generated.Framework.Semantics.SemanticsNode node, global::Doroti.Generated.Framework.Semantics.SemanticsData data, global::Doroti.Ui.Image image, ByteData byteData, global::Doroti.Generated.Framework.Rendering.RenderView renderView);
    internal virtual bool _isNodeOffScreen(Rect paintBounds, DorotiView window)
    {
        global::Doroti.Ui.Size windowLogicalSize__9904 = ((global::Doroti.Ui.Size)(object?)(window.physicalSize / window.devicePixelRatio));
        return ((((paintBounds.top < -50.0) || (paintBounds.left < -50.0)) || (paintBounds.bottom > (windowLogicalSize__9904.height + 50.0))) || (paintBounds.right > (windowLogicalSize__9904.width + 50.0)));
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

    internal override bool _shouldSkipNodeEvaluation(global::Doroti.Generated.Framework.Semantics.SemanticsData data) => DartRuntimePrimitives.ConvertValue<bool>((((global::Doroti.Generated.Framework.Semantics.SemanticsData)data).flagsCollection.scopesRoute || (((((global::Doroti.Generated.Framework.Semantics.SemanticsData)data).label.Trim().Length == 0) && (((global::Doroti.Generated.Framework.Semantics.SemanticsData)data).value.Trim().Length == 0)))));
    public async override Future<List<ViolationIo>> evaluateNodeContent(global::Doroti.Generated.Framework.Semantics.SemanticsNode node, global::Doroti.Generated.Framework.Semantics.SemanticsData data, global::Doroti.Ui.Image image, ByteData byteData, global::Doroti.Generated.Framework.Rendering.RenderView renderView)
    {
        var violations__12542 = new List<ViolationIo>();
        string text__12587 = ((((global::Doroti.Generated.Framework.Semantics.SemanticsData)data).label.Length == 0) ? ((global::Doroti.Generated.Framework.Semantics.SemanticsData)data).value : ((global::Doroti.Generated.Framework.Semantics.SemanticsData)data).label);
        IEnumerable<Element> elements__12668 = _accessibility_evaluationsLibrary._collectElementsByText(WidgetsBinding.instance.rootElement!, text__12587);
        foreach (var element__12781 in elements__12668)
        {
            violations__12542.AddRange((await _evaluateElement(node, element__12781, image, byteData, renderView)).Cast<ViolationIo>());
        }
        return violations__12542;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal async virtual Future<List<ViolationIo>> _evaluateElement(global::Doroti.Generated.Framework.Semantics.SemanticsNode node, Element element, global::Doroti.Ui.Image image, ByteData byteData, global::Doroti.Generated.Framework.Rendering.RenderView renderView)
    {
        bool isBold__13192 = default!;
        double? fontSize__13212 = default!;
        global::Doroti.Ui.Rect screenBounds__13243 = default!;
        global::Doroti.Ui.Rect paintBoundsWithOffset__13277 = default!;
        global::Doroti.Generated.Framework.Rendering.RenderObject? renderBox__13325 = ((Element)element).renderObject;
        if ((renderBox__13325 is not global::Doroti.Generated.Framework.Rendering.RenderBox))
        {
            throw new InvalidOperationException($"Unexpected renderObject type: {renderBox__13325}");
        }
        Matrix4 globalTransform__13487 = ((Matrix4)(object?)((global::Doroti.Generated.Framework.Rendering.RenderBox)renderBox__13325).getTransformTo(((global::Doroti.Generated.Framework.Rendering.RenderObject)(object)null)));
        paintBoundsWithOffset__13277 = MatrixUtils.transformRect(globalTransform__13487, ((global::Doroti.Generated.Framework.Rendering.RenderBox)((global::Doroti.Generated.Framework.Rendering.RenderBox)renderBox__13325)).paintBounds.inflate(4.0));
        var rootTransform__13878 = Matrix4.identity();
        renderView.applyPaintTransform(((global::Doroti.Generated.Framework.Rendering.RenderBox?)((dynamic)renderView).child)!, rootTransform__13878);
        rootTransform__13878.multiply(globalTransform__13487);
        screenBounds__13243 = MatrixUtils.transformRect(rootTransform__13878, ((global::Doroti.Generated.Framework.Rendering.RenderBox)((global::Doroti.Generated.Framework.Rendering.RenderBox)renderBox__13325)).paintBounds);
        global::Doroti.Ui.Rect nodeBounds__14122 = ((global::Doroti.Ui.Rect)(object?)((global::Doroti.Generated.Framework.Semantics.SemanticsNode)node).rect);
        global::Doroti.Generated.Framework.Semantics.SemanticsNode? current__14165 = node;
        while ((current__14165 is not null))
        {
            Matrix4? transform__14232 = ((global::Doroti.Generated.Framework.Semantics.SemanticsNode)current__14165).transform;
            if ((transform__14232 is not null))
            {
                nodeBounds__14122 = MatrixUtils.transformRect(transform__14232, nodeBounds__14122);
            }
            current__14165 = ((global::Doroti.Generated.Framework.Semantics.SemanticsNode)current__14165).parent;
        }
        global::Doroti.Ui.Rect intersection__14426 = ((global::Doroti.Ui.Rect)(object?)nodeBounds__14122.intersect(screenBounds__13243));
        if (((intersection__14426.width <= 0L) || (intersection__14426.height <= 0L)))
        {
            return new List<ViolationIo>();
        }
        Widget widget__14684 = ((Element)element).widget;
        DefaultTextStyle defaultTextStyle__14736 = ((DefaultTextStyle)(object?)DefaultTextStyle.of(element));
        if ((widget__14684 is Text))
        {
            Text widget__14684__as14793 = (Text)widget__14684;
            global::Doroti.Generated.Framework.Painting.TextStyle? style__14834 = ((Text)((Text)widget__14684__as14793)).style;
            global::Doroti.Generated.Framework.Painting.TextStyle effectiveTextStyle__14878 = (((style__14834 is null) || ((global::Doroti.Generated.Framework.Painting.TextStyle)style__14834).inherit) ? ((DefaultTextStyle)defaultTextStyle__14736).style.merge(((Text)((Text)widget__14684__as14793)).style) : style__14834);
            isBold__13192 = (object.Equals(((global::Doroti.Generated.Framework.Painting.TextStyle)effectiveTextStyle__14878).fontWeight, FontWeight.bold));
            fontSize__13212 = ((global::Doroti.Generated.Framework.Painting.TextStyle)effectiveTextStyle__14878).fontSize;
        }
        else
        {
            if ((widget__14684 is EditableText))
            {
                EditableText widget__14684__as15130 = (EditableText)widget__14684;
                isBold__13192 = (object.Equals(((EditableText)((EditableText)widget__14684__as15130)).style.fontWeight, FontWeight.bold));
                fontSize__13212 = ((EditableText)((EditableText)widget__14684__as15130)).style.fontSize;
            }
            else
            {
                throw new InvalidOperationException($"Unexpected widget type: {DartRuntimePrimitives.RuntimeType(widget__14684)}");
            }
        }
        if (_isNodeOffScreen(paintBoundsWithOffset__13277, ((global::Doroti.Generated.Framework.Rendering.RenderView)renderView).flutterView))
        {
            return new List<ViolationIo>();
        }
        DartMap<global::Doroti.Ui.Color, long> colorHistogram__15484 = _accessibility_evaluationsLibrary._colorsWithinRect(byteData, paintBoundsWithOffset__13277, DartRuntimePrimitives.RequireValue(((global::Doroti.Ui.Image)image).width), DartRuntimePrimitives.RequireValue(((global::Doroti.Ui.Image)image).height)).cast<global::Doroti.Ui.Color, long>();
        if (!System.Linq.Enumerable.Any(colorHistogram__15484))
        {
            return new List<ViolationIo>();
        }
        var report__15727 = _ContrastReport___accessibility_evaluations.Create(colorHistogram__15484);
        double contrastRatio__15787 = report__15727.contrastRatio();
        double targetContrastRatio__15844 = _targetContrastRatio(fontSize__13212, bold: isBold__13192);
        if (((contrastRatio__15787 - targetContrastRatio__15844) >= _ContrastEvaluation___accessibility_evaluations._kContrastTolerance))
        {
            return new List<ViolationIo>();
        }
        return new List<ViolationIo> { new ViolationIo(node, $"{node}:\n" + $"Expected contrast ratio of at least {targetContrastRatio__15844} " + $"but found {contrastRatio__15787.toStringAsFixed(2L)} " + $"for a font size of {fontSize__13212}.\n" + "The computed colors were:\n" + $"light - {((_ContrastReport___accessibility_evaluations)report__15727).lightColor}, dark - {((_ContrastReport___accessibility_evaluations)report__15727).darkColor}\n" + "See also: " + "https://www.w3.org/TR/UNDERSTANDING-WCAG20/visual-audio-contrast-contrast.html") };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _targetContrastRatio(double? fontSize, bool bold)
    {
        double fontSizeOrDefault__16788 = (fontSize ?? _kDefaultFontSize);
        if ((((bold && (fontSizeOrDefault__16788 >= kBoldTextMinimumSize))) || (fontSizeOrDefault__16788 >= kLargeTextMinimumSize)))
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

    internal override bool _shouldSkipNodeEvaluation(global::Doroti.Generated.Framework.Semantics.SemanticsData data)
    {
        if (((global::Doroti.Generated.Framework.Semantics.SemanticsData)data).flagsCollection.scopesRoute)
        {
            return true;
        }
        bool isControl__17918 = ((((((((global::Doroti.Generated.Framework.Semantics.SemanticsData)data).flagsCollection.isButton || ((global::Doroti.Generated.Framework.Semantics.SemanticsData)data).flagsCollection.isSlider) || ((global::Doroti.Generated.Framework.Semantics.SemanticsData)data).flagsCollection.isTextField) || (!object.Equals(((global::Doroti.Generated.Framework.Semantics.SemanticsData)data).flagsCollection.isChecked, CheckedState.none))) || (!object.Equals(((global::Doroti.Generated.Framework.Semantics.SemanticsData)data).flagsCollection.isToggled, Tristate.none))) || data.hasAction(SemanticsAction.tap)) || data.hasAction(SemanticsAction.longPress));
        return !isControl__17918;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public async override Future<List<ViolationIo>> evaluateNodeContent(global::Doroti.Generated.Framework.Semantics.SemanticsNode node, global::Doroti.Generated.Framework.Semantics.SemanticsData data, global::Doroti.Ui.Image image, ByteData byteData, global::Doroti.Generated.Framework.Rendering.RenderView renderView)
    {
        var violations__18516 = new List<ViolationIo>();
        global::Doroti.Ui.Rect nodeBounds__18553 = ((global::Doroti.Ui.Rect)(object?)((global::Doroti.Generated.Framework.Semantics.SemanticsNode)node).rect);
        global::Doroti.Generated.Framework.Semantics.SemanticsNode? current__18596 = node;
        while ((current__18596 is not null))
        {
            Matrix4? transform__18663 = ((global::Doroti.Generated.Framework.Semantics.SemanticsNode)current__18596).transform;
            if (((transform__18663 is not null) && (((global::Doroti.Generated.Framework.Semantics.SemanticsNode)current__18596).parent is not null)))
            {
                nodeBounds__18553 = MatrixUtils.transformRect(transform__18663, nodeBounds__18553);
            }
            current__18596 = ((global::Doroti.Generated.Framework.Semantics.SemanticsNode)current__18596).parent;
        }
        double devicePixelRatio__18886 = ((global::Doroti.Generated.Framework.Rendering.RenderView)renderView).flutterView.devicePixelRatio;
        var logicalBounds__18956 = global::Doroti.Ui.Rect.fromLTRB((nodeBounds__18553.left / devicePixelRatio__18886), (nodeBounds__18553.top / devicePixelRatio__18886), (nodeBounds__18553.right / devicePixelRatio__18886), (nodeBounds__18553.bottom / devicePixelRatio__18886));
        global::Doroti.Ui.Rect inflatedBounds__19180 = ((global::Doroti.Ui.Rect)(object?)logicalBounds__18956.inflate(4.0));
        if (_isNodeOffScreen(inflatedBounds__19180, ((global::Doroti.Generated.Framework.Rendering.RenderView)renderView).flutterView))
        {
            return violations__18516;
        }
        DartMap<global::Doroti.Ui.Color, long> colorHistogram__19352 = _accessibility_evaluationsLibrary._colorsWithinRect(byteData, inflatedBounds__19180, DartRuntimePrimitives.RequireValue(((global::Doroti.Ui.Image)image).width), DartRuntimePrimitives.RequireValue(((global::Doroti.Ui.Image)image).height)).cast<global::Doroti.Ui.Color, long>();
        if ((checked((long)(colorHistogram__19352.Count)) <= 1L))
        {
            return violations__18516;
        }
        var report__19553 = _ContrastReport___accessibility_evaluations.Create(colorHistogram__19352);
        double contrastRatio__19612 = report__19553.contrastRatio();
        if (((contrastRatio__19612 - _kMinimumRatioNonText) >= _ContrastEvaluation___accessibility_evaluations._kContrastTolerance))
        {
            return violations__18516;
        }
        violations__18516.Add(new ViolationIo(node, $"{node}:\n" + $"Expected non-text control contrast ratio of at least {_kMinimumRatioNonText.toStringAsFixed(1L)} " + $"but found {contrastRatio__19612.toStringAsFixed(2L)}.\n" + "The computed colors were:\n" + $"light - {((_ContrastReport___accessibility_evaluations)report__19553).lightColor}, dark - {((_ContrastReport___accessibility_evaluations)report__19553).darkColor}\n" + "See also: " + "https://www.w3.org/WAI/WCAG22/Understanding/non-text-contrast.html"));
        return violations__18516;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _ContrastReport___accessibility_evaluations
{
    public virtual Color lightColor { get; private set; } = default!;
    public virtual Color darkColor { get; private set; } = default!;

    internal static _ContrastReport___accessibility_evaluations Create(DartMap<Color, long> colorHistogram)
    {
        var totalLightness__20768 = 0.0;
        var count__20798 = 0L;
        foreach (MapEntry<global::Doroti.Ui.Color, long> entry__20845 in colorHistogram.entries)
        {
            totalLightness__20768 += (global::Doroti.Generated.Framework.Painting.HSLColor.CreateFromColor(entry__20845.key).lightness * entry__20845.value);
            count__20798 += entry__20845.value;
        }
        double averageLightness__21010 = (totalLightness__20768 / count__20798);
        DartRuntimePrimitives.Assert(() => !double.IsNaN(averageLightness__21010));
        MapEntry<global::Doroti.Ui.Color, long>? lightColor__21117 = default!;
        MapEntry<global::Doroti.Ui.Color, long>? darkColor__21155 = default!;
        foreach (MapEntry<global::Doroti.Ui.Color, long> entry__21268 in colorHistogram.entries)
        {
            double lightness__21322 = global::Doroti.Generated.Framework.Painting.HSLColor.CreateFromColor(entry__21268.key).lightness;
            long count__21391 = entry__21268.value;
            if ((lightness__21322 <= averageLightness__21010))
            {
                if ((count__21391 > ((darkColor__21155?.value ?? 0L))))
                {
                    darkColor__21155 = entry__21268;
                }
            }
            else
            {
                if ((count__21391 > ((lightColor__21117?.value ?? 0L))))
                {
                    lightColor__21117 = entry__21268;
                }
            }
        }
        return new _ContrastReport___accessibility_evaluations((lightColor__21117?.key ?? DartRuntimePrimitives.RequireValue(darkColor__21155).key), (darkColor__21155?.key ?? DartRuntimePrimitives.RequireValue(lightColor__21117).key));
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
        global::Doroti.Ui.Rect truePaintBounds__22928 = ((global::Doroti.Ui.Rect)(object?)paintBounds.intersect(global::Doroti.Ui.Rect.fromLTWH(0.0, 0.0, width.toDouble(), height.toDouble())));
        long leftX__23053 = truePaintBounds__22928.left.floor();
        long rightX__23103 = truePaintBounds__22928.right.ceil();
        long topY__23154 = truePaintBounds__22928.top.floor();
        long bottomY__23202 = truePaintBounds__22928.bottom.ceil();
        var rgbaToCount__23252 = new DartMap<long, long>();
        long getPixel(ByteData data, long x, long y)
        {
            long offset__23341 = ((((y * width) + x)) * 4L);
            return data.getUint32(offset__23341);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        for (var x__23422 = leftX__23053; (x__23422 < rightX__23103); x__23422++)
        {
            for (var y__23465 = topY__23154; (y__23465 < bottomY__23202); y__23465++)
            {
                rgbaToCount__23252.update(getPixel(data, x__23422, y__23465), ((count) => (count + 1L)), ifAbsent: (() => 1L));
            }
        }
        return rgbaToCount__23252.map<long, long, Color, long>(((rgba, count) => {
long argb__23674 = (((rgba << (int)(24L))) | (((rgba >> (int)(8L))) & 4294967295L));
return new MapEntry<global::Doroti.Ui.Color, long>(new global::Doroti.Ui.Color(argb__23674), count);
throw new InvalidOperationException("Dart closure completed without a value.");
}));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class _accessibility_evaluationsLibrary
{
    internal static IEnumerable<Element> _collectElementsByText(Element root, string text)
    {
        var result__23862 = new List<Element>();
        root.visitChildren(((global::System.Action<Element>)((child) => {
if (((((Element)child).widget is Text) && ((((Text?)(object?)((Element)child).widget)!).data == text)))
{
    result__23862.Add(child);
}
result__23862.AddRange(_accessibility_evaluationsLibrary._collectElementsByText(child, text).Cast<Element>());
})));
        return ((IEnumerable<Element>)(object?)result__23862);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class _accessibility_evaluationsLibrary
{
    internal static long _scrollingActions = ((((FoundationRuntimePorts.EnumIndex(SemanticsAction.scrollUp) | FoundationRuntimePorts.EnumIndex(SemanticsAction.scrollDown)) | FoundationRuntimePorts.EnumIndex(SemanticsAction.scrollLeft)) | FoundationRuntimePorts.EnumIndex(SemanticsAction.scrollRight)) | FoundationRuntimePorts.EnumIndex(SemanticsAction.scrollToOffset));
}

public static partial class _accessibility_evaluationsLibrary
{
    internal static bool _isImportantForAccessibility(global::Doroti.Generated.Framework.Semantics.SemanticsNode node)
    {
        if (((global::Doroti.Generated.Framework.Semantics.SemanticsNode)node).isMergedIntoParent)
        {
            return false;
        }
        global::Doroti.Generated.Framework.Semantics.SemanticsData data__25156 = ((global::Doroti.Generated.Framework.Semantics.SemanticsData)(object?)node.getSemanticsData());
        if (((global::Doroti.Generated.Framework.Semantics.SemanticsData)data__25156).flagsCollection.scopesRoute)
        {
            return false;
        }
        var hasNonScrollingAction__25412 = ((((global::Doroti.Generated.Framework.Semantics.SemanticsData)data__25156).actions & ~_accessibility_evaluationsLibrary._scrollingActions) != 0L);
        if (hasNonScrollingAction__25412)
        {
            return true;
        }
        bool hasImportantFlag__25760 = ((((((((!object.Equals(((global::Doroti.Generated.Framework.Semantics.SemanticsData)data__25156).flagsCollection.isChecked, CheckedState.none)) || (!object.Equals(((global::Doroti.Generated.Framework.Semantics.SemanticsData)data__25156).flagsCollection.isToggled, Tristate.none))) || (!object.Equals(((global::Doroti.Generated.Framework.Semantics.SemanticsData)data__25156).flagsCollection.isEnabled, Tristate.none))) || ((global::Doroti.Generated.Framework.Semantics.SemanticsData)data__25156).flagsCollection.isButton) || ((global::Doroti.Generated.Framework.Semantics.SemanticsData)data__25156).flagsCollection.isTextField) || (!object.Equals(((global::Doroti.Generated.Framework.Semantics.SemanticsData)data__25156).flagsCollection.isFocused, Tristate.none))) || ((global::Doroti.Generated.Framework.Semantics.SemanticsData)data__25156).flagsCollection.isSlider) || ((global::Doroti.Generated.Framework.Semantics.SemanticsData)data__25156).flagsCollection.isInMutuallyExclusiveGroup);
        if (hasImportantFlag__25760)
        {
            return true;
        }
        bool hasContent__26260 = ((((((global::Doroti.Generated.Framework.Semantics.SemanticsData)data__25156).label.Length != 0) || (((global::Doroti.Generated.Framework.Semantics.SemanticsData)data__25156).value.Length != 0)) || (((global::Doroti.Generated.Framework.Semantics.SemanticsData)data__25156).hint.Length != 0)) || (((global::Doroti.Generated.Framework.Semantics.SemanticsData)data__25156).tooltip.Length != 0));
        if (hasContent__26260)
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
        var violations__26836 = new List<ViolationIo>();
        foreach (global::Doroti.Generated.Framework.Rendering.RenderView view__26890 in binding.renderViews)
        {
            violations__26836.AddRange(_traverse(view__26890.owner!.semanticsOwner!.rootSemanticsNode!));
        }
        return new EvaluationResultIo(violations__26836);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual List<ViolationIo> _traverse(global::Doroti.Generated.Framework.Semantics.SemanticsNode node)
    {
        var violations__27117 = new List<ViolationIo>();
        var hasChildren__27153 = false;
        node.visitChildren(((global::System.Func<global::Doroti.Generated.Framework.Semantics.SemanticsNode, bool>)((child) => {
hasChildren__27153 = true;
violations__27117.AddRange(_traverse(child));
return true;
throw new InvalidOperationException("Dart closure completed without a value.");
})));
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsNode)node).isInvisible || ((global::Doroti.Generated.Framework.Semantics.SemanticsNode)node).flagsCollection.isHidden))
        {
            return violations__27117;
        }
        if ((hasChildren__27153 && !((global::Doroti.Generated.Framework.Semantics.SemanticsNode)node).mergeAllDescendantsIntoThisNode))
        {
            return violations__27117;
        }
        if (!_accessibility_evaluationsLibrary._isImportantForAccessibility(node))
        {
            return violations__27117;
        }
        global::Doroti.Generated.Framework.Semantics.SemanticsData data__27679 = ((global::Doroti.Generated.Framework.Semantics.SemanticsData)(object?)node.getSemanticsData());
        if (((((((global::Doroti.Generated.Framework.Semantics.SemanticsData)data__27679).label.Trim().Length == 0) && (((global::Doroti.Generated.Framework.Semantics.SemanticsData)data__27679).value.Trim().Length == 0)) && (((global::Doroti.Generated.Framework.Semantics.SemanticsData)data__27679).hint.Trim().Length == 0)) && (((global::Doroti.Generated.Framework.Semantics.SemanticsData)data__27679).tooltip.Trim().Length == 0)))
        {
            violations__27117.Add(new ViolationIo(node, $"{node}: expected leaf semantics node to have a label, value, hint, or tooltip, " + "but none was found."));
        }
        return violations__27117;
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
        var violations__28502 = new List<ViolationIo>();
        if (((((WidgetsBinding)binding).rootElement is not null) && !_hasTitleWidget(((WidgetsBinding)binding).rootElement!)))
        {
            global::Doroti.Generated.Framework.Semantics.SemanticsNode rootNode__28638 = binding.renderViews.First().owner!.semanticsOwner!.rootSemanticsNode!;
            violations__28502.Add(new ViolationIo(rootNode__28638, "Expected to find at least one Title widget, but none was found."));
        }
        return new EvaluationResultIo(violations__28502);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _hasTitleWidget(Element element)
    {
        if ((((Element)element).widget is Title))
        {
            return true;
        }
        var found__29018 = false;
        element.visitChildren(((global::System.Action<Element>)((child) => {
if (!found__29018)
{
    found__29018 = _hasTitleWidget(child);
}
})));
        return found__29018;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

