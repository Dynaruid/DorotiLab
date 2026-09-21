// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/proxy_sliver.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Rendering;

public abstract class RenderProxySliver : RenderSliver, RenderObjectWithChildMixin<RenderSliver>
{
    public virtual RenderSliver? _child { get; set; } = default;

    protected RenderProxySliver(RenderSliver? child = null) { }

    public override Rect semanticBounds
    {
        get
        {
            if (child is not null)
            {
                return child!.semanticBounds;
            }
            return base.semanticBounds;
        }
    }

    public override void setupParentData(RenderObject child)
    {
        if (child.parentData is not SliverPhysicalParentData)
        {
            child.parentData = new SliverPhysicalParentData();
        }
    }

    public override void performLayout()
    {
        DartRuntimePrimitives.Assert(() => child is not null);
        child!.layout(constraints, parentUsesSize: true);
        geometry = child!.geometry;
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        if (child is not null)
        {
            context.paintChild(child!, offset);
        }
    }

    public override bool hitTestChildren(
        SliverHitTestResult result,
        double mainAxisPosition,
        double crossAxisPosition
    )
    {
        return (child is not null)
            && (child!.geometry!.hitTestExtent > 0L)
            && child!.hitTest(
                result,
                mainAxisPosition: mainAxisPosition,
                crossAxisPosition: crossAxisPosition
            );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double childMainAxisPosition(RenderObject child)
    {
        var __child = (RenderSliver)child;
        DartRuntimePrimitives.Assert(() => Equals(__child, this.child));
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void applyPaintTransform(RenderObject child, Matrix4 transform)
    {
        var childParentData = ((SliverPhysicalParentData?)child.parentData!)!;
        childParentData.applyPaintTransform(transform);
    }

    public virtual bool debugValidateChild(RenderObject child)
    {
        DartRuntimePrimitives.Assert(() =>
        {
            if (child is not RenderSliver)
            {
                throw new FlutterError(
                    new List<DiagnosticsNode>
                    {
                        new ErrorSummary(
                            $"A {GetType()} expected a child of type {typeof(RenderSliver)} but received a "
                                + $"child of type {DartRuntimePrimitives.RuntimeType(child)}."
                        ),
                        new ErrorDescription(
                            "RenderObjects expect specific types of children because they "
                                + "coordinate with their children during layout and paint. For "
                                + "example, a RenderSliver cannot be the child of a RenderBox because "
                                + "a RenderSliver does not understand the RenderBox layout protocol."
                        ),
                        new ErrorSpacer(),
                        new DiagnosticsProperty<object?>(
                            $"The {GetType()} that expected a {typeof(RenderSliver)} child was created by",
                            debugCreator,
                            style: DiagnosticsTreeStyle.errorProperty
                        ),
                        new ErrorSpacer(),
                        new DiagnosticsProperty<object?>(
                            $"The {DartRuntimePrimitives.RuntimeType(child)} that did not match the expected child type "
                                + "was created by",
                            child.debugCreator,
                            style: DiagnosticsTreeStyle.errorProperty
                        ),
                    }
                );
            }
            return true;
        });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RenderSliver? child
    {
        get => _child;
        set
        {
            var __value = value;
            if (_child is not null)
            {
                dropChild(_child!);
            }
            _child = __value;
            if (_child is not null)
            {
                adoptChild(_child!);
            }
        }
    }

    public override void attach(PipelineOwner owner)
    {
        base.attach(owner);
        _child?.attach(owner);
    }

    public override void detach()
    {
        base.detach();
        _child?.detach();
    }

    public override void redepthChildren()
    {
        if (_child is not null)
        {
            redepthChild(_child!);
        }
    }

    public override void visitChildren(Action<RenderObject> visitor)
    {
        if (_child is not null)
        {
            visitor(_child!);
        }
    }

    public override List<DiagnosticsNode> debugDescribeChildren()
    {
        return (child is not null)
            ? new List<DiagnosticsNode>
            {
                ((Diagnosticable)child!).toDiagnosticsNode(name: "child"),
            }
            : new List<DiagnosticsNode>();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class RenderSliverOpacity : RenderProxySliver
{
    internal virtual long _alpha { get; set; } = default!;
    internal virtual double _opacity { get; set; } = default!;
    internal virtual bool _alwaysIncludeSemantics { get; set; } = default!;

    public RenderSliverOpacity(
        double opacity = 1.0,
        bool alwaysIncludeSemantics = false,
        RenderSliver? sliver = null
    )
    {
        _opacity = opacity;
        _alwaysIncludeSemantics = alwaysIncludeSemantics;
        _alpha = Dart_uiLibrary.Color.getAlphaFromOpacity(opacity);
        System.Diagnostics.Debug.Assert((opacity >= 0.0) && (opacity <= 1.0));
    }

    public override bool alwaysNeedsCompositing => (child is not null) && _alpha > 0L;
    public virtual double opacity
    {
        get => _opacity;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => (__value >= 0.0) && (__value <= 1.0));
            if (_opacity == __value)
            {
                return;
            }
            bool didNeedCompositing = alwaysNeedsCompositing;
            var wasVisible = _alpha != 0L;
            _opacity = __value;
            _alpha = Dart_uiLibrary.Color.getAlphaFromOpacity(_opacity);
            if (didNeedCompositing != alwaysNeedsCompositing)
            {
                markNeedsCompositingBitsUpdate();
            }
            markNeedsPaint();
            if ((wasVisible != (_alpha != 0L)) && !alwaysIncludeSemantics)
            {
                markNeedsSemanticsUpdate();
            }
        }
    }
    public virtual bool alwaysIncludeSemantics
    {
        get => _alwaysIncludeSemantics;
        set
        {
            var __value = value;
            if (__value == _alwaysIncludeSemantics)
            {
                return;
            }
            _alwaysIncludeSemantics = __value;
            markNeedsSemanticsUpdate();
        }
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        if ((child is not null) && child!.geometry!.visible)
        {
            if (_alpha == 0L)
            {
                layer = null;
                return;
            }
            DartRuntimePrimitives.Assert(() => needsCompositing);
            layer = context.pushOpacity(
                offset,
                _alpha,
                base.paint,
                oldLayer: ((OpacityLayer?)layer)!
            );
            DartRuntimePrimitives.Assert(() =>
            {
                layer!.debugCreator = debugCreator;
                return true;
            });
        }
    }

    public override void visitChildrenForSemantics(Action<RenderObject> visitor)
    {
        if ((child is not null) && ((_alpha != 0L) || alwaysIncludeSemantics))
        {
            visitor(child!);
        }
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DoubleProperty("opacity", opacity));
        properties.add(
            new FlagProperty(
                "alwaysIncludeSemantics",
                value: alwaysIncludeSemantics,
                ifTrue: "alwaysIncludeSemantics"
            )
        );
    }
}

public class RenderSliverIgnorePointer : RenderProxySliver
{
    internal virtual bool _ignoring { get; set; } = default!;
    internal virtual bool? _ignoringSemantics { get; set; } = default;

    public RenderSliverIgnorePointer(
        RenderSliver? sliver = null,
        bool ignoring = true,
        bool? ignoringSemantics = null
    )
    {
        _ignoring = ignoring;
        _ignoringSemantics = ignoringSemantics;
    }

    public virtual bool ignoring
    {
        get => _ignoring;
        set
        {
            var __value = value;
            if ((__value) == _ignoring)
            {
                return;
            }
            _ignoring = (__value);
            if (ignoringSemantics is null)
            {
                markNeedsSemanticsUpdate();
            }
        }
    }
    public virtual bool? ignoringSemantics
    {
        get => _ignoringSemantics;
        set
        {
            var __value = value;
            if (__value == _ignoringSemantics)
            {
                return;
            }
            _ignoringSemantics = __value;
            markNeedsSemanticsUpdate();
        }
    }

    public override bool hitTest(
        SliverHitTestResult result,
        double mainAxisPosition,
        double crossAxisPosition
    )
    {
        return !ignoring
            && base.hitTest(
                result,
                mainAxisPosition: mainAxisPosition,
                crossAxisPosition: crossAxisPosition
            );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void visitChildrenForSemantics(Action<RenderObject> visitor)
    {
        if (_ignoringSemantics ?? false)
        {
            return;
        }
        base.visitChildrenForSemantics(visitor);
    }

    public override void describeSemanticsConfiguration(SemanticsConfiguration config)
    {
        base.describeSemanticsConfiguration(config);
        config.isBlockingUserActions = ignoring && (_ignoringSemantics ?? true);
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<bool>("ignoring", ignoring));
        properties.add(
            new DiagnosticsProperty<bool?>(
                "ignoringSemantics",
                ignoringSemantics,
                description: (ignoringSemantics is null) ? null : $"implicitly {ignoringSemantics}"
            )
        );
    }
}

public class RenderSliverOffstage : RenderProxySliver
{
    internal virtual bool _offstage { get; set; } = default!;

    public RenderSliverOffstage(bool offstage = true, RenderSliver? sliver = null)
    {
        _offstage = offstage;
    }

    public virtual bool offstage
    {
        get => _offstage;
        set
        {
            var __value = value;
            if (__value == _offstage)
            {
                return;
            }
            _offstage = __value;
            markNeedsLayoutForSizedByParentChange();
        }
    }

    public override void performLayout()
    {
        DartRuntimePrimitives.Assert(() => child is not null);
        child!.layout(constraints, parentUsesSize: true);
        if (!offstage)
        {
            geometry = child!.geometry;
        }
        else
        {
            geometry = SliverGeometry.zero;
        }
    }

    public override bool hitTest(
        SliverHitTestResult result,
        double mainAxisPosition,
        double crossAxisPosition
    )
    {
        return !offstage
            && base.hitTest(
                result,
                mainAxisPosition: mainAxisPosition,
                crossAxisPosition: crossAxisPosition
            );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool hitTestChildren(
        SliverHitTestResult result,
        double mainAxisPosition,
        double crossAxisPosition
    )
    {
        return !offstage
            && (child is not null)
            && (child!.geometry!.hitTestExtent > 0L)
            && child!.hitTest(
                result,
                mainAxisPosition: mainAxisPosition,
                crossAxisPosition: crossAxisPosition
            );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        if (offstage)
        {
            return;
        }
        context.paintChild(child!, offset);
    }

    public override void visitChildrenForSemantics(Action<RenderObject> visitor)
    {
        if (offstage)
        {
            return;
        }
        base.visitChildrenForSemantics(visitor);
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<bool>("offstage", offstage));
    }

    public override List<DiagnosticsNode> debugDescribeChildren()
    {
        if (child is null)
        {
            return new List<DiagnosticsNode>();
        }
        return new List<DiagnosticsNode>
        {
            ((Diagnosticable)child!).toDiagnosticsNode(
                name: "child",
                style: offstage ? DiagnosticsTreeStyle.offstage : DiagnosticsTreeStyle.sparse
            ),
        };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class RenderSliverAnimatedOpacity
    : RenderProxySliver,
        RenderAnimatedOpacityMixin<RenderSliver>
{
    public virtual long? _alpha { get; set; } = default;
    public virtual bool? _currentlyIsRepaintBoundary { get; set; } = default;
    public virtual Animation<double>? _opacity { get; set; } = default;
    public virtual bool? _alwaysIncludeSemantics { get; set; } = default;

    public RenderSliverAnimatedOpacity(
        Animation<double> opacity,
        bool alwaysIncludeSemantics = false,
        RenderSliver? sliver = null
    )
    {
        this.opacity = opacity;
        this.alwaysIncludeSemantics = alwaysIncludeSemantics;
        child = sliver;
    }

    public override bool isRepaintBoundary =>
        (child is not null)
        && (
            _currentlyIsRepaintBoundary
            ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
        );

    public override OffsetLayer updateCompositedLayer(OffsetLayer? oldLayer)
    {
        var __oldLayer = oldLayer is null ? null : (OpacityLayer)oldLayer;
        OpacityLayer updatedLayer = __oldLayer ?? new OpacityLayer();
        updatedLayer.alpha = _alpha;
        return updatedLayer;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Animation<double> opacity
    {
        get => _opacity!;
        set
        {
            var __value = value;
            if (Equals(_opacity, __value))
            {
                return;
            }
            if (attached && (_opacity is not null))
            {
                opacity.removeListener(_updateOpacity);
            }
            _opacity = __value;
            if (attached)
            {
                opacity.addListener(_updateOpacity);
            }
            _updateOpacity();
        }
    }
    public virtual bool alwaysIncludeSemantics
    {
        get =>
            (
                _alwaysIncludeSemantics
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
        set
        {
            var __value = value;
            if (__value == _alwaysIncludeSemantics)
            {
                return;
            }
            _alwaysIncludeSemantics = __value;
            markNeedsSemanticsUpdate();
        }
    }

    public override void attach(PipelineOwner owner)
    {
        base.attach(owner);
        opacity.addListener(_updateOpacity);
        _updateOpacity();
    }

    public override void detach()
    {
        opacity.removeListener(_updateOpacity);
        base.detach();
    }

    public virtual void _updateOpacity()
    {
        long? oldAlpha = _alpha;
        _alpha = Dart_uiLibrary.Color.getAlphaFromOpacity(opacity.value);
        if (oldAlpha != _alpha)
        {
            bool? wasRepaintBoundary = _currentlyIsRepaintBoundary;
            _currentlyIsRepaintBoundary =
                (
                    _alpha
                    ?? throw new global::System.NullReferenceException(
                        "Dart null assertion failed."
                    )
                ) > 0L;
            if ((child is not null) && (wasRepaintBoundary != _currentlyIsRepaintBoundary))
            {
                markNeedsCompositingBitsUpdate();
            }
            markNeedsCompositedLayerUpdate();
            if ((oldAlpha == 0L) || (_alpha == 0L))
            {
                markNeedsSemanticsUpdate();
            }
        }
    }

    public override bool paintsChild(RenderObject child)
    {
        DartRuntimePrimitives.Assert(() => Equals(child.parent, this));
        return opacity.value > 0L;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        if (_alpha == 0L)
        {
            return;
        }
        base.paint(context, offset);
    }

    public override void visitChildrenForSemantics(Action<RenderObject> visitor)
    {
        if ((child is not null) && ((_alpha != 0L) || alwaysIncludeSemantics))
        {
            visitor(child!);
        }
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<Animation<double>>("opacity", opacity));
        properties.add(
            new FlagProperty(
                "alwaysIncludeSemantics",
                value: alwaysIncludeSemantics,
                ifTrue: "alwaysIncludeSemantics"
            )
        );
    }
}

public class RenderSliverConstrainedCrossAxis : RenderProxySliver
{
    internal virtual double _maxExtent { get; set; } = default!;

    public RenderSliverConstrainedCrossAxis(double maxExtent)
    {
        _maxExtent = maxExtent;
        System.Diagnostics.Debug.Assert(maxExtent >= 0.0);
    }

    public virtual double maxExtent
    {
        get => _maxExtent;
        set
        {
            var __value = value;
            if (_maxExtent == __value)
            {
                return;
            }
            _maxExtent = __value;
            markNeedsLayout();
        }
    }

    public override void performLayout()
    {
        DartRuntimePrimitives.Assert(() => child is not null);
        DartRuntimePrimitives.Assert(() => maxExtent >= 0.0);
        child!.layout(
            constraints.copyWith(
                crossAxisExtent: Math.Min(_maxExtent, constraints.crossAxisExtent)
            ),
            parentUsesSize: true
        );
        SliverGeometry childLayoutGeometry = child!.geometry!;
        geometry = childLayoutGeometry.copyWith(
            crossAxisExtent: Math.Min(_maxExtent, constraints.crossAxisExtent)
        );
    }
}

public class RenderSliverSemanticsAnnotations : RenderProxySliver, SemanticsAnnotationsMixin
{
    public virtual SemanticsProperties _properties { get; set; } = default!;
    public virtual bool _container { get; set; } = default!;
    public virtual bool _explicitChildNodes { get; set; } = default!;
    public virtual bool _excludeSemantics { get; set; } = default!;
    public virtual bool _blockUserActions { get; set; } = default!;
    public virtual Locale? _localeForSubtree { get; set; } = default;
    public virtual AttributedString? _attributedLabel { get; set; } = default;
    public virtual AttributedString? _attributedValue { get; set; } = default;
    public virtual AttributedString? _attributedIncreasedValue { get; set; } = default;
    public virtual AttributedString? _attributedDecreasedValue { get; set; } = default;
    public virtual AttributedString? _attributedHint { get; set; } = default;
    public virtual TextDirection? _textDirection { get; set; } = default;

    public RenderSliverSemanticsAnnotations(
        RenderSliver? child = null,
        SemanticsProperties properties = default!,
        bool container = false,
        bool explicitChildNodes = false,
        bool excludeSemantics = false,
        bool blockUserActions = false,
        Locale? localeForSubtree = null,
        TextDirection? textDirection = null
    )
        : base(child) { }

    public virtual void initSemanticsAnnotations(
        SemanticsProperties properties,
        bool container,
        bool explicitChildNodes,
        bool excludeSemantics,
        bool blockUserActions,
        Locale? localeForSubtree,
        TextDirection? textDirection
    )
    {
        _properties = properties;
        _container = container;
        _explicitChildNodes = explicitChildNodes;
        _excludeSemantics = excludeSemantics;
        _blockUserActions = blockUserActions;
        _localeForSubtree = localeForSubtree;
        _textDirection = textDirection;
        _updateAttributedFields(_properties);
    }

    public virtual SemanticsProperties properties
    {
        get => _properties;
        set
        {
            var __value = value;
            if (Equals(_properties, __value))
            {
                return;
            }
            _properties = __value;
            _updateAttributedFields(_properties);
            markNeedsSemanticsUpdate();
        }
    }
    public virtual bool container
    {
        get => _container;
        set
        {
            var __value = value;
            if (container == __value)
            {
                return;
            }
            _container = __value;
            markNeedsSemanticsUpdate();
        }
    }
    public virtual bool explicitChildNodes
    {
        get => _explicitChildNodes;
        set
        {
            var __value = value;
            if (_explicitChildNodes == __value)
            {
                return;
            }
            _explicitChildNodes = __value;
            markNeedsSemanticsUpdate();
        }
    }
    public virtual bool excludeSemantics
    {
        get => _excludeSemantics;
        set
        {
            var __value = value;
            if (_excludeSemantics == __value)
            {
                return;
            }
            _excludeSemantics = __value;
            markNeedsSemanticsUpdate();
        }
    }
    public virtual bool blockUserActions
    {
        get => _blockUserActions;
        set
        {
            var __value = value;
            if (_blockUserActions == __value)
            {
                return;
            }
            _blockUserActions = __value;
            markNeedsSemanticsUpdate();
        }
    }
    public virtual Locale? localeForSubtree
    {
        get => _localeForSubtree;
        set
        {
            var __value = value;
            if (Equals(_localeForSubtree, __value))
            {
                return;
            }
            _localeForSubtree = __value;
            markNeedsSemanticsUpdate();
        }
    }

    public virtual void _updateAttributedFields(SemanticsProperties value)
    {
        _attributedLabel = _effectiveAttributedLabel(value);
        _attributedValue = _effectiveAttributedValue(value);
        _attributedIncreasedValue = _effectiveAttributedIncreasedValue(value);
        _attributedDecreasedValue = _effectiveAttributedDecreasedValue(value);
        _attributedHint = _effectiveAttributedHint(value);
    }

    public virtual AttributedString? _effectiveAttributedLabel(SemanticsProperties value)
    {
        return value.attributedLabel
            ?? ((value.label is null) ? null : new AttributedString(value.label!));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual AttributedString? _effectiveAttributedValue(SemanticsProperties value)
    {
        return value.attributedValue
            ?? ((value.value is null) ? null : new AttributedString(value.value!));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual AttributedString? _effectiveAttributedIncreasedValue(SemanticsProperties value)
    {
        return value.attributedIncreasedValue
            ?? (
                (value.increasedValue is null) ? null : new AttributedString(value.increasedValue!)
            );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual AttributedString? _effectiveAttributedDecreasedValue(SemanticsProperties value)
    {
        return properties.attributedDecreasedValue
            ?? (
                (value.decreasedValue is null) ? null : new AttributedString(value.decreasedValue!)
            );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual AttributedString? _effectiveAttributedHint(SemanticsProperties value)
    {
        return value.attributedHint
            ?? ((value.hint is null) ? null : new AttributedString(value.hint!));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual TextDirection? textDirection
    {
        get => _textDirection;
        set
        {
            var __value = value;
            if (Equals(textDirection, __value))
            {
                return;
            }
            _textDirection = __value;
            markNeedsSemanticsUpdate();
        }
    }

    public override void visitChildrenForSemantics(Action<RenderObject> visitor)
    {
        if (excludeSemantics)
        {
            return;
        }
        base.visitChildrenForSemantics(visitor);
    }

    public override void describeSemanticsConfiguration(SemanticsConfiguration config)
    {
        base.describeSemanticsConfiguration(config);
        config.isSemanticBoundary = container || _properties.identifier is not null;
        config.explicitChildNodes = explicitChildNodes;
        config.isBlockingUserActions = blockUserActions;
        if (localeForSubtree is not null)
        {
            config.localeForSubtree = localeForSubtree;
        }
        DartRuntimePrimitives.Assert(() =>
            ((_properties.scopesRoute ?? false) && explicitChildNodes)
            || !(_properties.scopesRoute ?? false)
        );
        DartRuntimePrimitives.Assert(() =>
            !((_properties.toggled ?? false) && (_properties.@checked ?? false))
        );
        if (_properties.enabled is not null)
        {
            config.isEnabled = _properties.enabled;
        }
        if (_properties.@checked is not null)
        {
            config.isChecked = _properties.@checked;
        }
        if (_properties.mixed is not null)
        {
            config.isCheckStateMixed = _properties.mixed;
        }
        if (_properties.toggled is not null)
        {
            config.isToggled = _properties.toggled;
        }
        if (_properties.selected is not null)
        {
            config.isSelected = (
                _properties.selected
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
        }
        if (_properties.button is not null)
        {
            config.isButton = (
                _properties.button
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
        }
        if (_properties.expanded is not null)
        {
            config.isExpanded = _properties.expanded;
        }
        if (_properties.link is not null)
        {
            config.isLink = (
                _properties.link
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
        }
        if (_properties.linkUrl is not null)
        {
            config.linkUrl = _properties.linkUrl;
        }
        if (_properties.slider is not null)
        {
            config.isSlider = (
                _properties.slider
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
        }
        if (_properties.keyboardKey is not null)
        {
            config.isKeyboardKey = (
                _properties.keyboardKey
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
        }
        if (_properties.header is not null)
        {
            config.isHeader = (
                _properties.header
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
        }
        if (_properties.headingLevel is not null)
        {
            config.headingLevel = (
                _properties.headingLevel
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
        }
        if (_properties.textField is not null)
        {
            config.isTextField = (
                _properties.textField
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
        }
        if (_properties.readOnly is not null)
        {
            config.isReadOnly = (
                _properties.readOnly
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
        }
        if (_properties.focusable is not null)
        {
            config.isFocusable = (
                _properties.focusable
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
        }
        if (_properties.focused is not null)
        {
            config.isFocused = _properties.focused;
        }
        if (_properties.accessibilityFocusBlockType is not null)
        {
            config.accessibilityFocusBlockType = (
                _properties.accessibilityFocusBlockType
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
        }
        if (_properties.inMutuallyExclusiveGroup is not null)
        {
            config.isInMutuallyExclusiveGroup = (
                _properties.inMutuallyExclusiveGroup
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
        }
        if (_properties.obscured is not null)
        {
            config.isObscured = (
                _properties.obscured
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
        }
        if (_properties.multiline is not null)
        {
            config.isMultiline = (
                _properties.multiline
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
        }
        if (_properties.hidden is not null)
        {
            config.isHidden = (
                _properties.hidden
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
        }
        if (_properties.image is not null)
        {
            config.isImage = (
                _properties.image
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
        }
        if (_properties.isRequired is not null)
        {
            config.isRequired = _properties.isRequired;
        }
        if (_properties.identifier is not null)
        {
            config.identifier = _properties.identifier!;
        }
        if (_properties.traversalParentIdentifier is not null)
        {
            config.traversalParentIdentifier = _properties.traversalParentIdentifier;
        }
        if (_properties.traversalChildIdentifier is not null)
        {
            config.traversalChildIdentifier = _properties.traversalChildIdentifier;
        }
        if (_attributedLabel is not null)
        {
            config.attributedLabel = _attributedLabel!;
        }
        if (_attributedValue is not null)
        {
            config.attributedValue = _attributedValue!;
        }
        if (_attributedIncreasedValue is not null)
        {
            config.attributedIncreasedValue = _attributedIncreasedValue!;
        }
        if (_attributedDecreasedValue is not null)
        {
            config.attributedDecreasedValue = _attributedDecreasedValue!;
        }
        if (_attributedHint is not null)
        {
            config.attributedHint = _attributedHint!;
        }
        if (_properties.tooltip is not null)
        {
            config.tooltip = _properties.tooltip!;
        }
        if ((_properties.hintOverrides is not null) && _properties.hintOverrides!.isNotEmpty)
        {
            config.hintOverrides = _properties.hintOverrides;
        }
        if (_properties.scopesRoute is not null)
        {
            config.scopesRoute = (
                _properties.scopesRoute
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
        }
        if (_properties.namesRoute is not null)
        {
            config.namesRoute = (
                _properties.namesRoute
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
        }
        if (_properties.liveRegion is not null)
        {
            config.liveRegion = (
                _properties.liveRegion
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
        }
        if (_properties.maxValueLength is not null)
        {
            config.maxValueLength = _properties.maxValueLength;
        }
        if (_properties.currentValueLength is not null)
        {
            config.currentValueLength = _properties.currentValueLength;
        }
        if (textDirection is not null)
        {
            config.textDirection = textDirection;
        }
        if (_properties.sortKey is not null)
        {
            config.sortKey = _properties.sortKey;
        }
        if (_properties.tagForChildren is not null)
        {
            config.addTagForChildren(_properties.tagForChildren!);
        }
        if (properties.role is not null)
        {
            config.role = (
                _properties.role
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
        }
        if (_properties.controlsNodes is not null)
        {
            config.controlsNodes = _properties.controlsNodes;
        }
        if (!Equals(config.validationResult, _properties.validationResult))
        {
            config.validationResult = _properties.validationResult;
        }
        if (_properties.hitTestBehavior is not null)
        {
            config.hitTestBehavior = (
                _properties.hitTestBehavior
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
        }
        if (_properties.inputType is not null)
        {
            config.inputType = (
                _properties.inputType
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
        }
        if (_properties.minValue is not null)
        {
            config.minValue = _properties.minValue;
        }
        if (_properties.maxValue is not null)
        {
            config.maxValue = _properties.maxValue;
        }
        if (_properties.onTap is not null)
        {
            config.onTap = _performTap;
        }
        if (_properties.onLongPress is not null)
        {
            config.onLongPress = _performLongPress;
        }
        if (_properties.onDismiss is not null)
        {
            config.onDismiss = _performDismiss;
        }
        if (_properties.onScrollLeft is not null)
        {
            config.onScrollLeft = _performScrollLeft;
        }
        if (_properties.onScrollRight is not null)
        {
            config.onScrollRight = _performScrollRight;
        }
        if (_properties.onScrollUp is not null)
        {
            config.onScrollUp = _performScrollUp;
        }
        if (_properties.onScrollDown is not null)
        {
            config.onScrollDown = _performScrollDown;
        }
        if (_properties.onIncrease is not null)
        {
            config.onIncrease = _performIncrease;
        }
        if (_properties.onDecrease is not null)
        {
            config.onDecrease = _performDecrease;
        }
        if (_properties.onCopy is not null)
        {
            config.onCopy = _performCopy;
        }
        if (_properties.onCut is not null)
        {
            config.onCut = _performCut;
        }
        if (_properties.onPaste is not null)
        {
            config.onPaste = _performPaste;
        }
        if (_properties.onMoveCursorForwardByCharacter is not null)
        {
            config.onMoveCursorForwardByCharacter = _performMoveCursorForwardByCharacter;
        }
        if (_properties.onMoveCursorBackwardByCharacter is not null)
        {
            config.onMoveCursorBackwardByCharacter = _performMoveCursorBackwardByCharacter;
        }
        if (_properties.onMoveCursorForwardByWord is not null)
        {
            config.onMoveCursorForwardByWord = _performMoveCursorForwardByWord;
        }
        if (_properties.onMoveCursorBackwardByWord is not null)
        {
            config.onMoveCursorBackwardByWord = _performMoveCursorBackwardByWord;
        }
        if (_properties.onSetSelection is not null)
        {
            config.onSetSelection = _performSetSelection;
        }
        if (_properties.onSetText is not null)
        {
            config.onSetText = _performSetText;
        }
        if (_properties.onDidGainAccessibilityFocus is not null)
        {
            config.onDidGainAccessibilityFocus = _performDidGainAccessibilityFocus;
        }
        if (_properties.onDidLoseAccessibilityFocus is not null)
        {
            config.onDidLoseAccessibilityFocus = _performDidLoseAccessibilityFocus;
        }
        if (_properties.onFocus is not null)
        {
            config.onFocus = _performFocus;
        }
        if (_properties.onExpand is not null)
        {
            config.onExpand = _performExpand;
        }
        if (_properties.onCollapse is not null)
        {
            config.onCollapse = _performCollapse;
        }
        if (_properties.customSemanticsActions is not null)
        {
            config.customSemanticsActions = _properties.customSemanticsActions!;
        }
    }

    public virtual void _performTap()
    {
        _properties.onTap?.Invoke();
    }

    public virtual void _performLongPress()
    {
        _properties.onLongPress?.Invoke();
    }

    public virtual void _performDismiss()
    {
        _properties.onDismiss?.Invoke();
    }

    public virtual void _performScrollLeft()
    {
        _properties.onScrollLeft?.Invoke();
    }

    public virtual void _performScrollRight()
    {
        _properties.onScrollRight?.Invoke();
    }

    public virtual void _performScrollUp()
    {
        _properties.onScrollUp?.Invoke();
    }

    public virtual void _performScrollDown()
    {
        _properties.onScrollDown?.Invoke();
    }

    public virtual void _performIncrease()
    {
        _properties.onIncrease?.Invoke();
    }

    public virtual void _performDecrease()
    {
        _properties.onDecrease?.Invoke();
    }

    public virtual void _performCopy()
    {
        _properties.onCopy?.Invoke();
    }

    public virtual void _performCut()
    {
        _properties.onCut?.Invoke();
    }

    public virtual void _performPaste()
    {
        _properties.onPaste?.Invoke();
    }

    public virtual void _performMoveCursorForwardByCharacter(bool extendSelection)
    {
        _properties.onMoveCursorForwardByCharacter?.Invoke(extendSelection);
    }

    public virtual void _performMoveCursorBackwardByCharacter(bool extendSelection)
    {
        _properties.onMoveCursorBackwardByCharacter?.Invoke(extendSelection);
    }

    public virtual void _performMoveCursorForwardByWord(bool extendSelection)
    {
        _properties.onMoveCursorForwardByWord?.Invoke(extendSelection);
    }

    public virtual void _performMoveCursorBackwardByWord(bool extendSelection)
    {
        _properties.onMoveCursorBackwardByWord?.Invoke(extendSelection);
    }

    public virtual void _performSetSelection(TextSelection selection)
    {
        _properties.onSetSelection?.Invoke(selection);
    }

    public virtual void _performSetText(string text)
    {
        _properties.onSetText?.Invoke(text);
    }

    public virtual void _performDidGainAccessibilityFocus()
    {
        _properties.onDidGainAccessibilityFocus?.Invoke();
    }

    public virtual void _performDidLoseAccessibilityFocus()
    {
        _properties.onDidLoseAccessibilityFocus?.Invoke();
    }

    public virtual void _performFocus()
    {
        _properties.onFocus?.Invoke();
    }

    public virtual void _performExpand()
    {
        _properties.onExpand?.Invoke();
    }

    public virtual void _performCollapse()
    {
        _properties.onCollapse?.Invoke();
    }
}
