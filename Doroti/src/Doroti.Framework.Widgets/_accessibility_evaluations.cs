// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/_accessibility_evaluations.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public static partial class _accessibility_evaluationsLibrary
{
    internal static string _kAccessibilityEvaluationsDisabledErrorMessage =
        "Accessibility evaluations APIs are not enabled.\n\nAccessibility evaluations APIs are currently experimental. Do not use accessibility evaluations APIs in\nproduction applications or plugins published to pub.dev.\n\nTo try experimental accessibility evaluations APIs:\n1. Switch to Flutter's main release channel.\n2. Turn on the accessibility evaluations feature flag. (See flutter config --help)\n";
}

public class ViolationIo
{
    public virtual SemanticsNode node { get; private set; } = default!;
    public virtual string reason { get; private set; } = default!;

    public ViolationIo(SemanticsNode node, string reason)
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
    protected AccessibilityEvaluationIo() { }

    public virtual object evaluate(WidgetsBinding binding)
    {
        if (!_featuresLibrary.isAccessibilityEvaluationsEnabled)
        {
            throw new NotSupportedException(
                _accessibility_evaluationsLibrary._kAccessibilityEvaluationsDisabledErrorMessage
            );
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
        foreach (RenderView view in binding.renderViews)
        {
            violations.AddRange(
                _traverse(view.flutterView, view.owner!.semanticsOwner!.rootSemanticsNode!)
            );
        }
        return new EvaluationResultIo(violations);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual List<ViolationIo> _traverse(DorotiView view, SemanticsNode node)
    {
        var violations = new List<ViolationIo>();
        node.visitChildren(
            (child) =>
            {
                violations.AddRange(_traverse(view, child));
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            }
        );
        if (node.isMergedIntoParent)
        {
            return violations;
        }
        if (shouldSkipNode(node))
        {
            return violations;
        }
        Rect paintBounds = node.rect;
        SemanticsNode? current = node;
        while (current is not null)
        {
            Matrix4? transformLocal = current.transform;
            if (transformLocal is not null)
            {
                paintBounds = MatrixUtils.transformRect(transformLocal, paintBounds);
            }
            if (
                current.flagsCollection.hasImplicitScrolling
                && _isAtBoundary(paintBounds, current.rect)
            )
            {
                return violations;
            }
            current = current.parent;
        }
        Rect viewRect = Offset.zero & view.physicalSize;
        if (_isAtBoundary(paintBounds, viewRect))
        {
            return violations;
        }
        Size candidateSize = paintBounds.size / view.devicePixelRatio;
        if (
            (
                candidateSize.width
                < (size.width - Foundation.ConstantsLibrary.precisionErrorTolerance)
            )
            || (
                candidateSize.height
                < (size.height - Foundation.ConstantsLibrary.precisionErrorTolerance)
            )
        )
        {
            violations.Add(
                new ViolationIo(
                    node,
                    $"{node}: expected tap target size of at least {size}, "
                        + $"but found {candidateSize}\n"
                )
            );
        }
        return violations;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static bool _isAtBoundary(Rect child, Rect parent)
    {
        if (
            ((child.left - parent.left) > _kMinimumGapToBoundary)
            && ((parent.right - child.right) > _kMinimumGapToBoundary)
            && ((child.top - parent.top) > _kMinimumGapToBoundary)
            && ((parent.bottom - child.bottom) > _kMinimumGapToBoundary)
        )
        {
            return false;
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool shouldSkipNode(SemanticsNode node)
    {
        SemanticsData data = node.getSemanticsData();
        if (
            (!data.hasAction(SemanticsAction.longPress) && !data.hasAction(SemanticsAction.tap))
            || data.flagsCollection.isHidden
        )
        {
            return true;
        }
        if (data.flagsCollection.isLink)
        {
            return true;
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class LabeledTapTargetEvaluationIo : AccessibilityEvaluationIo
{
    public LabeledTapTargetEvaluationIo() { }

    internal override object _evaluate(WidgetsBinding binding)
    {
        var violations = new List<ViolationIo>();
        foreach (RenderView view in binding.renderViews)
        {
            violations.AddRange(_traverse(view.owner!.semanticsOwner!.rootSemanticsNode!));
        }
        return new EvaluationResultIo(violations);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual List<ViolationIo> _traverse(SemanticsNode node)
    {
        var violations = new List<ViolationIo>();
        node.visitChildren(
            (child) =>
            {
                violations.AddRange(_traverse(child));
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            }
        );
        if (
            node.isMergedIntoParent
            || node.isInvisible
            || node.flagsCollection.isHidden
            || node.flagsCollection.isTextField
        )
        {
            return violations;
        }
        SemanticsData data = node.getSemanticsData();
        if (!data.hasAction(SemanticsAction.longPress) && !data.hasAction(SemanticsAction.tap))
        {
            return violations;
        }
        if (data.label.Length == 0 && data.tooltip.Length == 0)
        {
            violations.Add(
                new ViolationIo(
                    node,
                    $"{node}: expected tappable node to have semantic label, "
                        + "but none was found."
                )
            );
        }
        return violations;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public abstract class _ContrastEvaluation___accessibility_evaluations : AccessibilityEvaluationIo
{
    internal static double _kContrastTolerance = -0.01;

    internal _ContrastEvaluation___accessibility_evaluations() { }

    internal override async Future<EvaluationResultIo> _evaluate(WidgetsBinding binding)
    {
        var violations = new List<ViolationIo>();
        foreach (RenderView renderView in binding.renderViews)
        {
            var layer = ((OffsetLayer?)renderView.debugLayer!)!;
            SemanticsNode root = renderView.owner!.semanticsOwner!.rootSemanticsNode!;
            double ratio = 1L / renderView.flutterView.devicePixelRatio;
            Ui.Image image = await layer.toImage(renderView.paintBounds, pixelRatio: ratio);
            ByteData byteData = (await image.toByteData())!;
            violations.AddRange(
                (await _evaluateNode(root, image, byteData, renderView)).Cast<ViolationIo>()
            );
            image.dispose();
        }
        return new EvaluationResultIo(violations);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual async Future<List<ViolationIo>> _evaluateNode(
        SemanticsNode node,
        Ui.Image image,
        ByteData byteData,
        RenderView renderView
    )
    {
        var violations = new List<ViolationIo>();
        if (_shouldSkipNodeTraversal(node))
        {
            return violations;
        }
        SemanticsData data = node.getSemanticsData();
        var children = new List<SemanticsNode>();
        node.visitChildren(
            (child) =>
            {
                children.Add(child);
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            }
        );
        foreach (var childLocal in children)
        {
            violations.AddRange(
                (await _evaluateNode(childLocal, image, byteData, renderView)).Cast<ViolationIo>()
            );
        }
        if (_shouldSkipNodeEvaluation(data))
        {
            return violations;
        }
        return await evaluateNodeContent(node, data, image, byteData, renderView);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _shouldSkipNodeTraversal(SemanticsNode node)
    {
        var isDisabled = Equals(node.flagsCollection.isEnabled, Tristate.isFalse);
        return node.isInvisible
            || node.isMergedIntoParent
            || node.flagsCollection.isHidden
            || isDisabled;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal abstract bool _shouldSkipNodeEvaluation(SemanticsData data);
    public abstract Future<List<ViolationIo>> evaluateNodeContent(
        SemanticsNode node,
        SemanticsData data,
        Ui.Image image,
        ByteData byteData,
        RenderView renderView
    );

    internal virtual bool _isNodeOffScreen(Rect paintBounds, DorotiView window)
    {
        Size windowLogicalSize = window.physicalSize / window.devicePixelRatio;
        return (paintBounds.top < -50.0)
            || (paintBounds.left < -50.0)
            || (paintBounds.bottom > (windowLogicalSize.height + 50.0))
            || (paintBounds.right > (windowLogicalSize.width + 50.0));
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

    public MinimumTextContrastEvaluationIo(
        double minNormalTextContrastRatio,
        double minLargeTextContrastRatio
    )
    {
        this.minNormalTextContrastRatio = minNormalTextContrastRatio;
        this.minLargeTextContrastRatio = minLargeTextContrastRatio;
    }

    internal override bool _shouldSkipNodeEvaluation(SemanticsData data) =>
        DartRuntimePrimitives.ConvertValue<bool>(
            data.flagsCollection.scopesRoute
                || ((data.label.Trim().Length == 0) && (data.value.Trim().Length == 0))
        );

    public override async Future<List<ViolationIo>> evaluateNodeContent(
        SemanticsNode node,
        SemanticsData data,
        Ui.Image image,
        ByteData byteData,
        RenderView renderView
    )
    {
        var violations = new List<ViolationIo>();
        string text = (data.label.Length == 0) ? data.value : data.label;
        IEnumerable<Element> elements = _accessibility_evaluationsLibrary._collectElementsByText(
            WidgetsBinding.instance.rootElement!,
            text
        );
        foreach (var element in elements)
        {
            violations.AddRange(
                (
                    await _evaluateElement(node, element, image, byteData, renderView)
                ).Cast<ViolationIo>()
            );
        }
        return violations;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual async Future<List<ViolationIo>> _evaluateElement(
        SemanticsNode node,
        Element element,
        Ui.Image image,
        ByteData byteData,
        RenderView renderView
    )
    {
        bool isBold = default!;
        double? fontSizeLocal = default!;
        Rect screenBounds = default!;
        Rect paintBoundsWithOffset = default!;
        RenderObject? renderBox = element.renderObject;
        if (renderBox is not RenderBox)
        {
            throw new InvalidOperationException($"Unexpected renderObject type: {renderBox}");
        }
        Matrix4 globalTransform = ((RenderBox)renderBox).getTransformTo(null);
        paintBoundsWithOffset = MatrixUtils.transformRect(
            globalTransform,
            ((RenderBox)renderBox).paintBounds.inflate(4.0)
        );
        var rootTransform = Matrix4.identity();
        renderView.applyPaintTransform(renderView.child!, rootTransform);
        rootTransform.multiply(globalTransform);
        screenBounds = MatrixUtils.transformRect(rootTransform, ((RenderBox)renderBox).paintBounds);
        Rect nodeBounds = node.rect;
        SemanticsNode? current = node;
        while (current is not null)
        {
            Matrix4? transformLocal = current.transform;
            if (transformLocal is not null)
            {
                nodeBounds = MatrixUtils.transformRect(transformLocal, nodeBounds);
            }
            current = current.parent;
        }
        Rect intersection = nodeBounds.intersect(screenBounds);
        if ((intersection.width <= 0L) || (intersection.height <= 0L))
        {
            return new List<ViolationIo>();
        }
        Widget widgetLocal = element.widget;
        DefaultTextStyle defaultTextStyle = DefaultTextStyle.of(element);
        if (widgetLocal is Text)
        {
            Text widget__14684__as14793 = (Text)widgetLocal;
            TextStyle? styleLocal = widget__14684__as14793.style;
            TextStyle effectiveTextStyle =
                ((styleLocal is null) || styleLocal.inherit)
                    ? defaultTextStyle.style.merge(widget__14684__as14793.style)
                    : styleLocal;
            isBold = Equals(effectiveTextStyle.fontWeight, FontWeight.bold);
            fontSizeLocal = effectiveTextStyle.fontSize;
        }
        else
        {
            if (widgetLocal is EditableText)
            {
                EditableText widget__14684__as15130 = (EditableText)widgetLocal;
                isBold = Equals(widget__14684__as15130.style.fontWeight, FontWeight.bold);
                fontSizeLocal = widget__14684__as15130.style.fontSize;
            }
            else
            {
                throw new InvalidOperationException(
                    $"Unexpected widget type: {DartRuntimePrimitives.RuntimeType(widgetLocal)}"
                );
            }
        }
        if (_isNodeOffScreen(paintBoundsWithOffset, renderView.flutterView))
        {
            return new List<ViolationIo>();
        }
        DartMap<Color, long> colorHistogram = _accessibility_evaluationsLibrary
            ._colorsWithinRect(
                byteData,
                paintBoundsWithOffset,
                DartRuntimePrimitives.RequireValue(image.width),
                DartRuntimePrimitives.RequireValue(image.height)
            )
            .cast<Color, long>();
        if (!Enumerable.Any(colorHistogram))
        {
            return new List<ViolationIo>();
        }
        var report = _ContrastReport___accessibility_evaluations.Create(colorHistogram);
        double contrastRatioLocal = report.contrastRatio();
        double targetContrastRatio = _targetContrastRatio(fontSizeLocal, bold: isBold);
        if ((contrastRatioLocal - targetContrastRatio) >= _kContrastTolerance)
        {
            return new List<ViolationIo>();
        }
        return new List<ViolationIo>
        {
            new ViolationIo(
                node,
                $"{node}:\n"
                    + $"Expected contrast ratio of at least {targetContrastRatio} "
                    + $"but found {contrastRatioLocal.toStringAsFixed(2L)} "
                    + $"for a font size of {fontSizeLocal}.\n"
                    + "The computed colors were:\n"
                    + $"light - {report.lightColor}, dark - {report.darkColor}\n"
                    + "See also: "
                    + "https://www.w3.org/TR/UNDERSTANDING-WCAG20/visual-audio-contrast-contrast.html"
            ),
        };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _targetContrastRatio(double? fontSize, bool bold)
    {
        double fontSizeOrDefault = fontSize ?? _kDefaultFontSize;
        if (
            (bold && (fontSizeOrDefault >= kBoldTextMinimumSize))
            || (fontSizeOrDefault >= kLargeTextMinimumSize)
        )
        {
            return minLargeTextContrastRatio;
        }
        return minNormalTextContrastRatio;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class MinimumNonTextContrastEvaluationIo : _ContrastEvaluation___accessibility_evaluations
{
    internal const double _kMinimumRatioNonText = 3.0;

    public MinimumNonTextContrastEvaluationIo() { }

    internal override bool _shouldSkipNodeEvaluation(SemanticsData data)
    {
        if (data.flagsCollection.scopesRoute)
        {
            return true;
        }
        bool isControl =
            data.flagsCollection.isButton
            || data.flagsCollection.isSlider
            || data.flagsCollection.isTextField
            || (!Equals(data.flagsCollection.isChecked, CheckedState.none))
            || (!Equals(data.flagsCollection.isToggled, Tristate.none))
            || data.hasAction(SemanticsAction.tap)
            || data.hasAction(SemanticsAction.longPress);
        return !isControl;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override async Future<List<ViolationIo>> evaluateNodeContent(
        SemanticsNode node,
        SemanticsData data,
        Ui.Image image,
        ByteData byteData,
        RenderView renderView
    )
    {
        var violations = new List<ViolationIo>();
        Rect nodeBounds = node.rect;
        SemanticsNode? current = node;
        while (current is not null)
        {
            Matrix4? transformLocal = current.transform;
            if ((transformLocal is not null) && (current.parent is not null))
            {
                nodeBounds = MatrixUtils.transformRect(transformLocal, nodeBounds);
            }
            current = current.parent;
        }
        double devicePixelRatioLocal = renderView.flutterView.devicePixelRatio;
        var logicalBounds = Rect.fromLTRB(
            nodeBounds.left / devicePixelRatioLocal,
            nodeBounds.top / devicePixelRatioLocal,
            nodeBounds.right / devicePixelRatioLocal,
            nodeBounds.bottom / devicePixelRatioLocal
        );
        Rect inflatedBounds = logicalBounds.inflate(4.0);
        if (_isNodeOffScreen(inflatedBounds, renderView.flutterView))
        {
            return violations;
        }
        DartMap<Color, long> colorHistogram = _accessibility_evaluationsLibrary
            ._colorsWithinRect(
                byteData,
                inflatedBounds,
                DartRuntimePrimitives.RequireValue(image.width),
                DartRuntimePrimitives.RequireValue(image.height)
            )
            .cast<Color, long>();
        if (checked(colorHistogram.Count) <= 1L)
        {
            return violations;
        }
        var report = _ContrastReport___accessibility_evaluations.Create(colorHistogram);
        double contrastRatioLocal = report.contrastRatio();
        if ((contrastRatioLocal - _kMinimumRatioNonText) >= _kContrastTolerance)
        {
            return violations;
        }
        violations.Add(
            new ViolationIo(
                node,
                $"{node}:\n"
                    + $"Expected non-text control contrast ratio of at least {_kMinimumRatioNonText.toStringAsFixed(1L)} "
                    + $"but found {contrastRatioLocal.toStringAsFixed(2L)}.\n"
                    + "The computed colors were:\n"
                    + $"light - {report.lightColor}, dark - {report.darkColor}\n"
                    + "See also: "
                    + "https://www.w3.org/WAI/WCAG22/Understanding/non-text-contrast.html"
            )
        );
        return violations;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _ContrastReport___accessibility_evaluations
{
    public virtual Color lightColor { get; private set; } = default!;
    public virtual Color darkColor { get; private set; } = default!;

    internal static _ContrastReport___accessibility_evaluations Create(
        DartMap<Color, long> colorHistogram
    )
    {
        var totalLightness = 0.0;
        var count = 0L;
        foreach (MapEntry<Color, long> entry in colorHistogram.entries)
        {
            totalLightness += HSLColor.CreateFromColor(entry.key).lightness * entry.value;
            count += entry.value;
        }
        double averageLightness = totalLightness / count;
        DartRuntimePrimitives.Assert(() => !double.IsNaN(averageLightness));
        MapEntry<Color, long>? lightColor = default!;
        MapEntry<Color, long>? darkColor = default!;
        foreach (MapEntry<Color, long> entryLocal in colorHistogram.entries)
        {
            double lightnessLocal = HSLColor.CreateFromColor(entryLocal.key).lightness;
            long countLocal = entryLocal.value;
            if (lightnessLocal <= averageLightness)
            {
                if (countLocal > (darkColor?.value ?? 0L))
                {
                    darkColor = entryLocal;
                }
            }
            else
            {
                if (countLocal > (lightColor?.value ?? 0L))
                {
                    lightColor = entryLocal;
                }
            }
        }
        return new _ContrastReport___accessibility_evaluations(
            lightColor?.key ?? DartRuntimePrimitives.RequireValue(darkColor).key,
            darkColor?.key ?? DartRuntimePrimitives.RequireValue(lightColor).key
        );
    }

    internal _ContrastReport___accessibility_evaluations(Color lightColor, Color darkColor)
    {
        this.lightColor = lightColor;
        this.darkColor = darkColor;
    }

    public virtual double contrastRatio() =>
        DartRuntimePrimitives.ConvertValue<double>(
            (lightColor.computeLuminance() + 0.05) / (darkColor.computeLuminance() + 0.05)
        );
}

public static partial class _accessibility_evaluationsLibrary
{
    internal static DartMap<Color, long> _colorsWithinRect(
        ByteData data,
        Rect paintBounds,
        long width,
        long height
    )
    {
        Rect truePaintBounds = paintBounds.intersect(
            Rect.fromLTWH(0.0, 0.0, width.toDouble(), height.toDouble())
        );
        long leftX = truePaintBounds.left.floor();
        long rightX = truePaintBounds.right.ceil();
        long topY = truePaintBounds.top.floor();
        long bottomY = truePaintBounds.bottom.ceil();
        var rgbaToCount = new DartMap<long, long>();
        long getPixel(ByteData data, long x, long y)
        {
            long offset = ((y * width) + x) * 4L;
            return data.getUint32(offset);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        for (var xLocal = leftX; xLocal < rightX; xLocal++)
        {
            for (var yLocal = topY; yLocal < bottomY; yLocal++)
            {
                rgbaToCount.update(
                    getPixel(data, xLocal, yLocal),
                    (count) => count + 1L,
                    ifAbsent: () => 1L
                );
            }
        }
        return rgbaToCount.map(
            (rgba, count) =>
            {
                long argb = (rgba << (int)24L) | ((rgba >> (int)8L) & 4294967295L);
                return new MapEntry<Color, long>(new Color(argb), count);
                throw new InvalidOperationException("Dart closure completed without a value.");
            }
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class _accessibility_evaluationsLibrary
{
    internal static IEnumerable<Element> _collectElementsByText(Element root, string text)
    {
        var result = new List<Element>();
        root.visitChildren(
            (child) =>
            {
                if ((child.widget is Text) && (((Text?)child.widget)!.data == text))
                {
                    result.Add(child);
                }
                result.AddRange(_collectElementsByText(child, text).Cast<Element>());
            }
        );
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class _accessibility_evaluationsLibrary
{
    internal static long _scrollingActions =
        FoundationRuntimePorts.EnumIndex(SemanticsAction.scrollUp)
        | FoundationRuntimePorts.EnumIndex(SemanticsAction.scrollDown)
        | FoundationRuntimePorts.EnumIndex(SemanticsAction.scrollLeft)
        | FoundationRuntimePorts.EnumIndex(SemanticsAction.scrollRight)
        | FoundationRuntimePorts.EnumIndex(SemanticsAction.scrollToOffset);
}

public static partial class _accessibility_evaluationsLibrary
{
    internal static bool _isImportantForAccessibility(SemanticsNode node)
    {
        if (node.isMergedIntoParent)
        {
            return false;
        }
        SemanticsData data = node.getSemanticsData();
        if (data.flagsCollection.scopesRoute)
        {
            return false;
        }
        var hasNonScrollingAction = (data.actions & ~_scrollingActions) != 0L;
        if (hasNonScrollingAction)
        {
            return true;
        }
        bool hasImportantFlag =
            (!Equals(data.flagsCollection.isChecked, CheckedState.none))
            || (!Equals(data.flagsCollection.isToggled, Tristate.none))
            || (!Equals(data.flagsCollection.isEnabled, Tristate.none))
            || data.flagsCollection.isButton
            || data.flagsCollection.isTextField
            || (!Equals(data.flagsCollection.isFocused, Tristate.none))
            || data.flagsCollection.isSlider
            || data.flagsCollection.isInMutuallyExclusiveGroup;
        if (hasImportantFlag)
        {
            return true;
        }
        bool hasContent =
            (data.label.Length != 0)
            || (data.value.Length != 0)
            || (data.hint.Length != 0)
            || (data.tooltip.Length != 0);
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
    public UnlabeledLeafNodeEvaluationIo() { }

    internal override object _evaluate(WidgetsBinding binding)
    {
        var violations = new List<ViolationIo>();
        foreach (RenderView view in binding.renderViews)
        {
            violations.AddRange(_traverse(view.owner!.semanticsOwner!.rootSemanticsNode!));
        }
        return new EvaluationResultIo(violations);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual List<ViolationIo> _traverse(SemanticsNode node)
    {
        var violations = new List<ViolationIo>();
        var hasChildren = false;
        node.visitChildren(
            (child) =>
            {
                hasChildren = true;
                violations.AddRange(_traverse(child));
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            }
        );
        if (node.isInvisible || node.flagsCollection.isHidden)
        {
            return violations;
        }
        if (hasChildren && !node.mergeAllDescendantsIntoThisNode)
        {
            return violations;
        }
        if (!_accessibility_evaluationsLibrary._isImportantForAccessibility(node))
        {
            return violations;
        }
        SemanticsData data = node.getSemanticsData();
        if (
            (data.label.Trim().Length == 0)
            && (data.value.Trim().Length == 0)
            && (data.hint.Trim().Length == 0)
            && (data.tooltip.Trim().Length == 0)
        )
        {
            violations.Add(
                new ViolationIo(
                    node,
                    $"{node}: expected leaf semantics node to have a label, value, hint, or tooltip, "
                        + "but none was found."
                )
            );
        }
        return violations;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class TitleEvaluationIo : AccessibilityEvaluationIo
{
    public TitleEvaluationIo() { }

    internal override object _evaluate(WidgetsBinding binding)
    {
        var violations = new List<ViolationIo>();
        if ((binding.rootElement is not null) && !_hasTitleWidget(binding.rootElement!))
        {
            SemanticsNode rootNode = binding
                .renderViews.First()
                .owner!.semanticsOwner!.rootSemanticsNode!;
            violations.Add(
                new ViolationIo(
                    rootNode,
                    "Expected to find at least one Title widget, but none was found."
                )
            );
        }
        return new EvaluationResultIo(violations);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _hasTitleWidget(Element element)
    {
        if (element.widget is Title)
        {
            return true;
        }
        var found = false;
        element.visitChildren(
            (child) =>
            {
                if (!found)
                {
                    found = _hasTitleWidget(child);
                }
            }
        );
        return found;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}
