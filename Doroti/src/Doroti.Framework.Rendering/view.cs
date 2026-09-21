// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/view.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Rendering;

public class ViewConfiguration
{
    public virtual BoxConstraints logicalConstraints { get; private set; } = default!;
    public virtual BoxConstraints physicalConstraints { get; private set; } = default!;
    public virtual double devicePixelRatio { get; private set; } = default!;

    public ViewConfiguration(
        BoxConstraints physicalConstraints = default!,
        BoxConstraints logicalConstraints = default!,
        double devicePixelRatio = 1.0
    )
    {
        BoxConstraints __physicalConstraints =
            physicalConstraints ?? new BoxConstraints(maxWidth: 0, maxHeight: 0);
        BoxConstraints __logicalConstraints =
            logicalConstraints ?? new BoxConstraints(maxWidth: 0, maxHeight: 0);
        this.physicalConstraints = __physicalConstraints;
        this.logicalConstraints = __logicalConstraints;
        this.devicePixelRatio = devicePixelRatio;
    }

    public static ViewConfiguration CreateFromView(DorotiView view)
    {
        var physicalConstraintsLocal = BoxConstraints.CreateFromViewConstraints(
            view.physicalConstraints
        );
        double devicePixelRatioLocal = view.devicePixelRatio;
        return new ViewConfiguration(
            physicalConstraints: physicalConstraintsLocal,
            logicalConstraints: physicalConstraintsLocal.op_Divide(devicePixelRatioLocal),
            devicePixelRatio: devicePixelRatioLocal
        );
    }

    public virtual Matrix4 toMatrix()
    {
        return Matrix4.diagonal3Values(devicePixelRatio, devicePixelRatio, 1.0);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool shouldUpdateMatrix(ViewConfiguration oldConfiguration)
    {
        if (!Equals(DartRuntimePrimitives.RuntimeType(oldConfiguration), GetType()))
        {
            return true;
        }
        return oldConfiguration.devicePixelRatio != devicePixelRatio;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Size toPhysicalSize(Size logicalSize)
    {
        return physicalConstraints.constrain(logicalSize * devicePixelRatio);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as ViewConfiguration;
        if (__other is null)
        {
            return false;
        }

        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is ViewConfiguration)
            && Equals(__other.logicalConstraints, logicalConstraints)
            && Equals(__other.physicalConstraints, physicalConstraints)
            && (__other.devicePixelRatio == devicePixelRatio);
    }

    public override int GetHashCode() =>
        FoundationRuntimePorts.ObjectHash(
            logicalConstraints,
            physicalConstraints,
            devicePixelRatio
        );

    public override string ToString() =>
        $"{logicalConstraints} at {Foundation.DebugLibrary.debugFormatDouble(devicePixelRatio)}x";
}

public class RenderView : RenderObject, RenderObjectWithChildMixin<RenderBox>
{
    internal virtual Size _size { get; set; } = Size.zero;
    internal virtual ViewConfiguration? _configuration { get; set; } = default;
    internal virtual DorotiView _view { get; private set; } = default!;
    public virtual bool automaticSystemUiAdjustment { get; set; } = true;
    internal virtual Matrix4? _rootTransform { get; set; } = default;
    internal static List<Action<PaintingContext, Offset, RenderView>> _debugPaintCallbacks =
        new List<Action<PaintingContext, Offset, RenderView>>();
    public virtual RenderBox? _child { get; set; } = default;

    public RenderView(
        RenderBox? child = null,
        ViewConfiguration? configuration = null,
        DorotiView view = default!
    )
    {
        _view = view;
    }

    public virtual Size size => _size;
    public virtual ViewConfiguration configuration
    {
        get => _configuration!;
        set
        {
            var __value = value;
            if (Equals(_configuration, __value))
            {
                return;
            }
            ViewConfiguration? oldConfiguration = _configuration;
            _configuration = __value;
            if (_rootTransform is null)
            {
                return;
            }
            if ((oldConfiguration is null) || configuration.shouldUpdateMatrix(oldConfiguration))
            {
                replaceRootLayer(_updateMatricesAndCreateNewRootLayer());
            }
            DartRuntimePrimitives.Assert(() => _rootTransform is not null);
            markNeedsLayout();
        }
    }
    public virtual bool hasConfiguration => _configuration is not null;
    public override BoxConstraints constraints
    {
        get
        {
            if (!hasConfiguration)
            {
                throw new InvalidOperationException(
                    "Constraints are not available because RenderView has not been given a configuration yet."
                );
            }
            return configuration.logicalConstraints;
        }
    }
    public virtual DorotiView flutterView => _view;

    public virtual void prepareInitialFrame()
    {
        DartRuntimePrimitives.Assert(() => owner is not null);
        DartRuntimePrimitives.Assert(() => _rootTransform is null);
        DartRuntimePrimitives.Assert(() => hasConfiguration);
        scheduleInitialLayout();
        scheduleInitialPaint(_updateMatricesAndCreateNewRootLayer());
        DartRuntimePrimitives.Assert(() => _rootTransform is not null);
    }

    internal virtual TransformLayer _updateMatricesAndCreateNewRootLayer()
    {
        DartRuntimePrimitives.Assert(() => hasConfiguration);
        _rootTransform = configuration.toMatrix();
        var rootLayer = new TransformLayer(transform: _rootTransform);
        rootLayer.attach(this);
        DartRuntimePrimitives.Assert(() => _rootTransform is not null);
        return rootLayer;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugAssertDoesMeetConstraints()
    {
        DartRuntimePrimitives.Assert(() => false);
    }

    public override void performResize()
    {
        DartRuntimePrimitives.Assert(() => false);
    }

    public override void performLayout()
    {
        DartRuntimePrimitives.Assert(() => _rootTransform is not null);
        bool sizedByChild = !constraints.isTight;
        child?.layout(constraints, parentUsesSize: sizedByChild);
        _size = (sizedByChild && (child is not null)) ? child!.size : constraints.smallest;
        DartRuntimePrimitives.Assert(() => size.isFinite);
        DartRuntimePrimitives.Assert(() => constraints.isSatisfiedBy(size));
    }

    public virtual bool hitTest(HitTestResult result, Offset position)
    {
        child?.hitTest(BoxHitTestResult.CreateWrap(result), position: position);
        result.add(new HitTestEntry<HitTestTarget>(this));
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool isRepaintBoundary => true;

    public override void paint(PaintingContext context, Offset offset)
    {
        if (child is not null)
        {
            context.paintChild(child!, offset);
        }
        DartRuntimePrimitives.Assert(() =>
        {
            List<Action<PaintingContext, Offset, RenderView>> localCallbacks =
                _debugPaintCallbacks.ToList();
            foreach (var paintCallback in localCallbacks)
            {
                if (_debugPaintCallbacks.Contains(paintCallback))
                {
                    paintCallback(context, offset, this);
                }
            }
            return true;
        });
    }

    public override void applyPaintTransform(RenderObject child, Matrix4 transform)
    {
        var __child = (RenderBox)(object)child;
        DartRuntimePrimitives.Assert(() => _rootTransform is not null);
        transform.multiply(_rootTransform!);
        base.applyPaintTransform(__child, transform);
    }

    public virtual void compositeFrame()
    {
        if (!Foundation.ConstantsLibrary.kReleaseMode)
        {
            FlutterTimeline.startSync("COMPOSITING");
        }
        try
        {
            DartRuntimePrimitives.Assert(() => hasConfiguration);
            DartRuntimePrimitives.Assert(() => _rootTransform is not null);
            DartRuntimePrimitives.Assert(() => layer is not null);
            SceneBuilder builder =
                _view.viewId == 0
                    ? RendererBinding.instance.createSceneBuilder()
                    : new SceneBuilder(_view.viewId);
            Scene scene = layer!.buildScene(builder);
            if (automaticSystemUiAdjustment)
            {
                _updateSystemChrome();
            }
            DartRuntimePrimitives.Assert(() =>
                configuration.logicalConstraints.isSatisfiedBy(size)
            );
            _view.render(scene, size: configuration.toPhysicalSize(size));
            scene.dispose();
            DartRuntimePrimitives.Assert(() =>
            {
                if (
                    DebugLibrary.debugRepaintRainbowEnabled
                    || DebugLibrary.debugRepaintTextRainbowEnabled
                )
                {
                    DebugLibrary.debugCurrentRepaintColor =
                        DebugLibrary.debugCurrentRepaintColor.withHue(
                            (DebugLibrary.debugCurrentRepaintColor.hue + 2.0) % 360.0
                        );
                }
                return true;
            });
        }
        finally
        {
            if (!Foundation.ConstantsLibrary.kReleaseMode)
            {
                FlutterTimeline.finishSync();
            }
        }
    }

    public virtual void updateSemantics(SemanticsUpdate update)
    {
        _view.updateSemantics(update);
    }

    internal virtual void _updateSystemChrome()
    {
        Rect bounds = paintBounds;
        var topLocal = new Offset(bounds.center.dx, _view.padding.top / 2.0);
        var bottomLocal = new Offset(
            bounds.center.dx,
            bounds.bottom - 1.0 - (_view.padding.bottom / 2.0)
        );
        SystemUiOverlayStyle? upperOverlayStyle = layer!.find<SystemUiOverlayStyle>(topLocal);
        SystemUiOverlayStyle? lowerOverlayStyle = default!;
        switch (PlatformLibrary.defaultTargetPlatform)
        {
            case var __constant17241 when Equals(__constant17241, TargetPlatform.android):
            {
                lowerOverlayStyle = layer!.find<SystemUiOverlayStyle>(bottomLocal);
                break;
            }
            case var __constant17347 when Equals(__constant17347, TargetPlatform.fuchsia):
            case var __constant17382 when Equals(__constant17382, TargetPlatform.iOS):
            case var __constant17413 when Equals(__constant17413, TargetPlatform.linux):
            case var __constant17446 when Equals(__constant17446, TargetPlatform.macOS):
            case var __constant17479 when Equals(__constant17479, TargetPlatform.windows):
            {
                break;
            }
        }
        if ((upperOverlayStyle is null) && (lowerOverlayStyle is null))
        {
            return;
        }
        if ((upperOverlayStyle is not null) && (lowerOverlayStyle is not null))
        {
            var overlayStyle = new SystemUiOverlayStyle(
                statusBarBrightness: upperOverlayStyle.statusBarBrightness,
                statusBarIconBrightness: upperOverlayStyle.statusBarIconBrightness,
                statusBarColor: upperOverlayStyle.statusBarColor,
                systemStatusBarContrastEnforced: upperOverlayStyle.systemStatusBarContrastEnforced,
                systemNavigationBarColor: lowerOverlayStyle.systemNavigationBarColor,
                systemNavigationBarDividerColor: lowerOverlayStyle.systemNavigationBarDividerColor,
                systemNavigationBarIconBrightness: lowerOverlayStyle.systemNavigationBarIconBrightness,
                systemNavigationBarContrastEnforced: lowerOverlayStyle.systemNavigationBarContrastEnforced
            );
            SystemChrome.setSystemUIOverlayStyle(overlayStyle);
            return;
        }
        var isAndroid = Equals(PlatformLibrary.defaultTargetPlatform, TargetPlatform.android);
        SystemUiOverlayStyle definedOverlayStyle = (upperOverlayStyle ?? lowerOverlayStyle)!;
        var overlayStyleLocal = new SystemUiOverlayStyle(
            statusBarBrightness: definedOverlayStyle.statusBarBrightness,
            statusBarIconBrightness: definedOverlayStyle.statusBarIconBrightness,
            statusBarColor: definedOverlayStyle.statusBarColor,
            systemStatusBarContrastEnforced: definedOverlayStyle.systemStatusBarContrastEnforced,
            systemNavigationBarColor: isAndroid
                ? definedOverlayStyle.systemNavigationBarColor
                : null,
            systemNavigationBarDividerColor: isAndroid
                ? definedOverlayStyle.systemNavigationBarDividerColor
                : null,
            systemNavigationBarIconBrightness: isAndroid
                ? definedOverlayStyle.systemNavigationBarIconBrightness
                : null,
            systemNavigationBarContrastEnforced: isAndroid
                ? definedOverlayStyle.systemNavigationBarContrastEnforced
                : null
        );
        SystemChrome.setSystemUIOverlayStyle(overlayStyleLocal);
    }

    public override Rect paintBounds => Offset.zero & (size * configuration.devicePixelRatio);
    public override Rect semanticBounds
    {
        get
        {
            DartRuntimePrimitives.Assert(() => _rootTransform is not null);
            return MatrixUtils.transformRect(_rootTransform!, Offset.zero & size);
        }
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DartRuntimePrimitives.Assert(() =>
        {
            properties.add(
                new DiagnosticsNode(
                    $"debug mode enabled - {(Foundation.ConstantsLibrary.kIsWeb ? "Web" : Platform.operatingSystem)}"
                )
            );
            return true;
        });
        properties.add(
            new DiagnosticsProperty<Size>(
                "view size",
                _view.physicalSize,
                tooltip: "in physical pixels"
            )
        );
        properties.add(
            new DoubleProperty(
                "device pixel ratio",
                _view.devicePixelRatio,
                tooltip: "physical pixels per logical pixel"
            )
        );
        properties.add(
            new DiagnosticsProperty<ViewConfiguration>(
                "configuration",
                configuration,
                tooltip: "in logical pixels"
            )
        );
        if (_view.platformDispatcher.semanticsEnabled)
        {
            properties.add(new DiagnosticsNode("semantics enabled"));
        }
    }

    public static void debugAddPaintCallback(Action<PaintingContext, Offset, RenderView> callback)
    {
        DartRuntimePrimitives.Assert(() =>
        {
            _debugPaintCallbacks.Add(callback);
            return true;
        });
    }

    public static void debugRemovePaintCallback(
        Action<PaintingContext, Offset, RenderView> callback
    )
    {
        DartRuntimePrimitives.Assert(() =>
        {
            _debugPaintCallbacks.Remove(callback);
            return true;
        });
    }

    public virtual bool debugValidateChild(RenderObject child)
    {
        DartRuntimePrimitives.Assert(() =>
        {
            if (child is not RenderBox)
            {
                throw new FlutterError(
                    new List<DiagnosticsNode>
                    {
                        new ErrorSummary(
                            $"A {GetType()} expected a child of type {typeof(RenderBox)} but received a "
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
                            $"The {GetType()} that expected a {typeof(RenderBox)} child was created by",
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

    public virtual RenderBox? child
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

public delegate void DebugPaintCallback(
    PaintingContext context,
    Offset offset,
    RenderView renderView
);
