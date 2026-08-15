// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/view.dart
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

namespace Doroti.Generated.Framework.Rendering;

public class ViewConfiguration
{
    public virtual BoxConstraints logicalConstraints { get; private set; } = default!;
    public virtual BoxConstraints physicalConstraints { get; private set; } = default!;
    public virtual double devicePixelRatio { get; private set; } = default!;

    public ViewConfiguration(BoxConstraints physicalConstraints = default!, BoxConstraints logicalConstraints = default!, double devicePixelRatio = 1.0)
    {
        BoxConstraints __physicalConstraints = physicalConstraints ?? new BoxConstraints(maxWidth: 0, maxHeight: 0);
        BoxConstraints __logicalConstraints = logicalConstraints ?? new BoxConstraints(maxWidth: 0, maxHeight: 0);
        this.physicalConstraints = __physicalConstraints;
        this.logicalConstraints = __logicalConstraints;
        this.devicePixelRatio = devicePixelRatio;
    }

    public static ViewConfiguration CreateFromView(DorotiView view)
    {
        var physicalConstraints__1465 = BoxConstraints.CreateFromViewConstraints(view.physicalConstraints);
        double devicePixelRatio__1566 = view.devicePixelRatio;
        return new ViewConfiguration(physicalConstraints: physicalConstraints__1465, logicalConstraints: (physicalConstraints__1465.op_Divide(devicePixelRatio__1566)), devicePixelRatio: devicePixelRatio__1566);
    }

    public virtual Matrix4 toMatrix()
    {
        return Matrix4.diagonal3Values(this.devicePixelRatio, this.devicePixelRatio, 1.0);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool shouldUpdateMatrix(ViewConfiguration oldConfiguration)
    {
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(oldConfiguration), this.GetType())))
        {
            return true;
        }
        return (((ViewConfiguration)oldConfiguration).devicePixelRatio != this.devicePixelRatio);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.Size toPhysicalSize(Size logicalSize)
    {
        return this.physicalConstraints.constrain((logicalSize * this.devicePixelRatio));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as ViewConfiguration;
        if (__other is null) return false;
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((((__other is ViewConfiguration) && (object.Equals(((ViewConfiguration)((ViewConfiguration)__other)).logicalConstraints, this.logicalConstraints))) && (object.Equals(((ViewConfiguration)((ViewConfiguration)__other)).physicalConstraints, this.physicalConstraints))) && (((ViewConfiguration)((ViewConfiguration)__other)).devicePixelRatio == this.devicePixelRatio));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(this.logicalConstraints, this.physicalConstraints, this.devicePixelRatio);
    public override string ToString() => $"{this.logicalConstraints} at {(global::Doroti.Generated.Framework.Foundation.DebugLibrary.debugFormatDouble(this.devicePixelRatio))}x";
}

public class RenderView : RenderObject, RenderObjectWithChildMixin<RenderBox>
{
    internal virtual Size _size { get; set; } = Size.zero;
    internal virtual ViewConfiguration? _configuration { get; set; } = default;
    internal virtual DorotiView _view { get; private set; } = default!;
    public virtual bool automaticSystemUiAdjustment { get; set; } = true;
    internal virtual Matrix4? _rootTransform { get; set; } = default;
    internal static List<Action<PaintingContext, Offset, RenderView>> _debugPaintCallbacks = new List<Action<PaintingContext, Offset, RenderView>>();
    public virtual RenderBox? _child { get; set; } = default;

    public RenderView(RenderBox? child = null, ViewConfiguration? configuration = null, DorotiView view = default!)
    {
        this._view = view;
    }

    public virtual global::Doroti.Ui.Size size => this._size;
    public virtual ViewConfiguration configuration
    {
        get => this._configuration!;
        set
        {
            var __value = value;
            if ((object.Equals(this._configuration, __value)))
            {
                return;
            }
            ViewConfiguration? oldConfiguration__7200 = this._configuration;
            _configuration = __value;
            if ((this._rootTransform is null))
            {
                return;
            }
            if (((oldConfiguration__7200 is null) || this.configuration.shouldUpdateMatrix(oldConfiguration__7200)))
            {
                replaceRootLayer(_updateMatricesAndCreateNewRootLayer());
            }
            DartRuntimePrimitives.Assert(() => (this._rootTransform is not null));
            markNeedsLayout();
        }
    }
    public virtual bool hasConfiguration => (this._configuration is not null);
    public override BoxConstraints constraints
    {
        get
        {
            if (!this.hasConfiguration)
            {
                throw new InvalidOperationException("Constraints are not available because RenderView has not been given a configuration yet.");
            }
            return ((ViewConfiguration)this.configuration).logicalConstraints;
            return default!;
        }
    }
    public virtual global::Doroti.Ui.DorotiView flutterView => this._view;
    public virtual void prepareInitialFrame()
    {
        DartRuntimePrimitives.Assert(() => (owner is not null));
        DartRuntimePrimitives.Assert(() => (this._rootTransform is null));
        DartRuntimePrimitives.Assert(() => this.hasConfiguration);
        scheduleInitialLayout();
        scheduleInitialPaint(_updateMatricesAndCreateNewRootLayer());
        DartRuntimePrimitives.Assert(() => (this._rootTransform is not null));
    }

    internal virtual TransformLayer _updateMatricesAndCreateNewRootLayer()
    {
        DartRuntimePrimitives.Assert(() => this.hasConfiguration);
        _rootTransform = this.configuration.toMatrix();
        var rootLayer__11030 = new TransformLayer(transform: this._rootTransform);
        rootLayer__11030.attach(this);
        DartRuntimePrimitives.Assert(() => (this._rootTransform is not null));
        return rootLayer__11030;
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
        DartRuntimePrimitives.Assert(() => (this._rootTransform is not null));
        bool sizedByChild__11541 = !((BoxConstraints)this.constraints).isTight;
        child?.layout(this.constraints, parentUsesSize: sizedByChild__11541);
        _size = ((sizedByChild__11541 && (child is not null)) ? child!.size : ((BoxConstraints)this.constraints).smallest);
        DartRuntimePrimitives.Assert(() => this.size.isFinite);
        DartRuntimePrimitives.Assert(() => this.constraints.isSatisfiedBy(this.size));
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
        if ((child is not null))
        {
            context.paintChild(child!, offset);
        }
        DartRuntimePrimitives.Assert(() =>
            {
                List<Action<PaintingContext, Offset, RenderView>> localCallbacks__12785 = _debugPaintCallbacks.ToList();
                foreach (var paintCallback__12850 in localCallbacks__12785)
                {
                    if (_debugPaintCallbacks.Contains(paintCallback__12850))
                    {
                        paintCallback__12850(context, offset, this);
                    }
                }
                return true;
            });
    }

    public override void applyPaintTransform(RenderObject child, Matrix4 transform)
    {
        var __child = (RenderBox)(object)child;
        DartRuntimePrimitives.Assert(() => (this._rootTransform is not null));
        transform.multiply(this._rootTransform!);
        base.applyPaintTransform(__child, transform);
    }

    public virtual void compositeFrame()
    {
        if (!global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kReleaseMode)
        {
            FlutterTimeline.startSync("COMPOSITING");
        }
        try
        {
            DartRuntimePrimitives.Assert(() => this.hasConfiguration);
            DartRuntimePrimitives.Assert(() => (this._rootTransform is not null));
            DartRuntimePrimitives.Assert(() => (layer is not null));
            global::Doroti.Ui.SceneBuilder builder__14028 = RendererBinding.instance.createSceneBuilder();
            global::Doroti.Ui.Scene scene__14106 = layer!.buildScene(builder__14028);
            if (this.automaticSystemUiAdjustment)
            {
                _updateSystemChrome();
            }
            DartRuntimePrimitives.Assert(() => ((ViewConfiguration)this.configuration).logicalConstraints.isSatisfiedBy(this.size));
            this._view.render(scene__14106, size: this.configuration.toPhysicalSize(this.size));
            scene__14106.dispose();
            DartRuntimePrimitives.Assert(() =>
                {
                    if ((global::Doroti.Generated.Framework.Rendering.DebugLibrary.debugRepaintRainbowEnabled || global::Doroti.Generated.Framework.Rendering.DebugLibrary.debugRepaintTextRainbowEnabled))
                    {
                        global::Doroti.Generated.Framework.Rendering.DebugLibrary.debugCurrentRepaintColor = global::Doroti.Generated.Framework.Rendering.DebugLibrary.debugCurrentRepaintColor.withHue((((((global::Doroti.Generated.Framework.Painting.HSVColor)global::Doroti.Generated.Framework.Rendering.DebugLibrary.debugCurrentRepaintColor).hue + 2.0)) % 360.0));
                    }
                    return true;
                });
        }
        finally
        {
            if (!global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kReleaseMode)
            {
                FlutterTimeline.finishSync();
            }
        }
    }

    public virtual void updateSemantics(SemanticsUpdate update)
    {
        this._view.updateSemantics(update);
    }

    internal virtual void _updateSystemChrome()
    {
        global::Doroti.Ui.Rect bounds__16131 = this.paintBounds;
        var top__16195 = new global::Doroti.Ui.Offset(bounds__16131.center.dx, (this._view.padding.top / 2.0));
        var bottom__16482 = new global::Doroti.Ui.Offset(bounds__16131.center.dx, ((bounds__16131.bottom - 1.0) - (this._view.padding.bottom / 2.0)));
        SystemUiOverlayStyle? upperOverlayStyle__17026 = layer!.find<SystemUiOverlayStyle>(top__16195);
        SystemUiOverlayStyle? lowerOverlayStyle__17174 = default!;
        switch (global::Doroti.Generated.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
        {
            case var __constant17241 when object.Equals(__constant17241, TargetPlatform.android):
                {
                    lowerOverlayStyle__17174 = layer!.find<SystemUiOverlayStyle>(bottom__16482);
                    break;
                }
            case var __constant17347 when object.Equals(__constant17347, TargetPlatform.fuchsia):
            case var __constant17382 when object.Equals(__constant17382, TargetPlatform.iOS):
            case var __constant17413 when object.Equals(__constant17413, TargetPlatform.linux):
            case var __constant17446 when object.Equals(__constant17446, TargetPlatform.macOS):
            case var __constant17479 when object.Equals(__constant17479, TargetPlatform.windows):
                {
                    break;
                }
        }
        if (((upperOverlayStyle__17026 is null) && (lowerOverlayStyle__17174 is null)))
        {
            return;
        }
        if (((upperOverlayStyle__17026 is not null) && (lowerOverlayStyle__17174 is not null)))
        {
            var overlayStyle__18175 = new SystemUiOverlayStyle(statusBarBrightness: upperOverlayStyle__17026.statusBarBrightness, statusBarIconBrightness: upperOverlayStyle__17026.statusBarIconBrightness, statusBarColor: upperOverlayStyle__17026.statusBarColor, systemStatusBarContrastEnforced: upperOverlayStyle__17026.systemStatusBarContrastEnforced, systemNavigationBarColor: lowerOverlayStyle__17174.systemNavigationBarColor, systemNavigationBarDividerColor: lowerOverlayStyle__17174.systemNavigationBarDividerColor, systemNavigationBarIconBrightness: lowerOverlayStyle__17174.systemNavigationBarIconBrightness, systemNavigationBarContrastEnforced: lowerOverlayStyle__17174.systemNavigationBarContrastEnforced);
            SystemChrome.setSystemUIOverlayStyle(overlayStyle__18175);
            return;
        }
        var isAndroid__19296 = (object.Equals(global::Doroti.Generated.Framework.Foundation.PlatformLibrary.defaultTargetPlatform, TargetPlatform.android));
        SystemUiOverlayStyle definedOverlayStyle__19388 = ((upperOverlayStyle__17026 ?? lowerOverlayStyle__17174))!;
        var overlayStyle__19463 = new SystemUiOverlayStyle(statusBarBrightness: definedOverlayStyle__19388.statusBarBrightness, statusBarIconBrightness: definedOverlayStyle__19388.statusBarIconBrightness, statusBarColor: definedOverlayStyle__19388.statusBarColor, systemStatusBarContrastEnforced: definedOverlayStyle__19388.systemStatusBarContrastEnforced, systemNavigationBarColor: (isAndroid__19296 ? definedOverlayStyle__19388.systemNavigationBarColor : null), systemNavigationBarDividerColor: (isAndroid__19296 ? definedOverlayStyle__19388.systemNavigationBarDividerColor : null), systemNavigationBarIconBrightness: (isAndroid__19296 ? definedOverlayStyle__19388.systemNavigationBarIconBrightness : null), systemNavigationBarContrastEnforced: (isAndroid__19296 ? definedOverlayStyle__19388.systemNavigationBarContrastEnforced : null));
        SystemChrome.setSystemUIOverlayStyle(overlayStyle__19463);
    }

    public override Rect paintBounds => (Offset.zero & ((this.size * ((ViewConfiguration)this.configuration).devicePixelRatio)));
    public override Rect semanticBounds
    {
        get
        {
            DartRuntimePrimitives.Assert(() => (this._rootTransform is not null));
            return MatrixUtils.transformRect(this._rootTransform!, (Offset.zero & this.size));
            return default!;
        }
    }
    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                properties.add(new DiagnosticsNode($"debug mode enabled - {((global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kIsWeb ? "Web" : Platform.operatingSystem))}"));
                return true;
            });
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Size>("view size", this._view.physicalSize, tooltip: "in physical pixels"));
        properties.add(new DoubleProperty("device pixel ratio", this._view.devicePixelRatio, tooltip: "physical pixels per logical pixel"));
        properties.add(new DiagnosticsProperty<ViewConfiguration>("configuration", this.configuration, tooltip: "in logical pixels"));
        if (this._view.platformDispatcher.semanticsEnabled)
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

    public static void debugRemovePaintCallback(Action<PaintingContext, Offset, RenderView> callback)
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
                if ((child is not RenderBox))
                {
                    throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"A {this.GetType()} expected a child of type {typeof(RenderBox)} but received a " + $"child of type {DartRuntimePrimitives.RuntimeType(child)}."), new ErrorDescription("RenderObjects expect specific types of children because they " + "coordinate with their children during layout and paint. For " + "example, a RenderSliver cannot be the child of a RenderBox because " + "a RenderSliver does not understand the RenderBox layout protocol."), new ErrorSpacer(), new DiagnosticsProperty<object?>($"The {this.GetType()} that expected a {typeof(RenderBox)} child was created by", debugCreator, style: DiagnosticsTreeStyle.errorProperty), new ErrorSpacer(), new DiagnosticsProperty<object?>($"The {DartRuntimePrimitives.RuntimeType(child)} that did not match the expected child type " + "was created by", ((RenderObject)child).debugCreator, style: DiagnosticsTreeStyle.errorProperty) });
                }
                return true;
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RenderBox? child
    {
        get => this._child;
        set
        {
            var __value = value;
            if ((this._child is not null))
            {
                dropChild(this._child!);
            }
            this._child = __value;
            if ((this._child is not null))
            {
                adoptChild(this._child!);
            }
        }
    }
    public override void attach(PipelineOwner owner)
    {
        base.attach(owner);
        this._child?.attach(owner);
    }

    public override void detach()
    {
        base.detach();
        this._child?.detach();
    }

    public override void redepthChildren()
    {
        if ((this._child is not null))
        {
            redepthChild(this._child!);
        }
    }

    public override void visitChildren(Action<RenderObject> visitor)
    {
        if ((this._child is not null))
        {
            visitor(this._child!);
        }
    }

    public override List<DiagnosticsNode> debugDescribeChildren()
    {
        return ((this.child is not null) ? new List<DiagnosticsNode> { ((Diagnosticable)this.child!).toDiagnosticsNode(name: "child") } : new List<DiagnosticsNode>());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public delegate void DebugPaintCallback(PaintingContext context, Offset offset, RenderView renderView);

