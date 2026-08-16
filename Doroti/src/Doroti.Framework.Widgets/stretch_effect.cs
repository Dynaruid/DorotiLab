// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/stretch_effect.dart
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

public class StretchEffect : StatelessWidget
{
    public virtual double stretchStrength { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.Axis axis { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;

    public StretchEffect(global::Doroti.Framework.Foundation.Key? key = null, double stretchStrength = 0.0, global::Doroti.Framework.Painting.Axis axis = default!, Widget child = default!) : base(key: key)
    {
        this.stretchStrength = stretchStrength;
        this.axis = axis;
        this.child = child;
        System.Diagnostics.Debug.Assert(((stretchStrength >= -1.0) && (stretchStrength <= 1.0)));
    }

    internal virtual global::Doroti.Framework.Painting.AlignmentGeometry _getAlignment(TextDirection direction)
    {
        bool isForward__2587 = (this.stretchStrength > 0L);
        if ((object.Equals(this.axis, global::Doroti.Framework.Painting.Axis.vertical)))
        {
            return ((global::Doroti.Framework.Painting.AlignmentGeometry)(object?)(isForward__2587 ? global::Doroti.Framework.Painting.AlignmentDirectional.topCenter : global::Doroti.Framework.Painting.AlignmentDirectional.bottomCenter));
        }
        if ((object.Equals(direction, TextDirection.rtl)))
        {
            return ((global::Doroti.Framework.Painting.AlignmentGeometry)(object?)(isForward__2587 ? global::Doroti.Framework.Painting.AlignmentDirectional.centerEnd : global::Doroti.Framework.Painting.AlignmentDirectional.centerStart));
        }
        else
        {
            return ((global::Doroti.Framework.Painting.AlignmentGeometry)(object?)(isForward__2587 ? global::Doroti.Framework.Painting.AlignmentDirectional.centerStart : global::Doroti.Framework.Painting.AlignmentDirectional.centerEnd));
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        if (ImageFilter.isShaderFilterSupported)
        {
            return ((Widget)(object?)new _StretchOverscrollEffect__stretch_effect(stretchStrength: this.stretchStrength, axis: this.axis, child: this.child));
        }
        global::Doroti.Ui.TextDirection textDirection__3258 = Directionality.of(context);
        var x__3310 = 1.0;
        var y__3327 = 1.0;
        switch (this.axis)
        {
            case global::Doroti.Framework.Painting.Axis.horizontal:
                {
                    x__3310 += this.stretchStrength.abs();
                    break;
                }
            case global::Doroti.Framework.Painting.Axis.vertical:
                {
                    y__3327 += this.stretchStrength.abs();
                    break;
                }
        }
        return ((Widget)(object?)new Transform(alignment: _getAlignment(textDirection__3258), transform: Matrix4.diagonal3Values(x__3310, y__3327, 1.0), filterQuality: ((this.stretchStrength == 0L) ? null : FilterQuality.medium), child: this.child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _StretchOverscrollEffect__stretch_effect : StatefulWidget
{
    public virtual double stretchStrength { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.Axis axis { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;

    internal _StretchOverscrollEffect__stretch_effect(double stretchStrength = 0.0, global::Doroti.Framework.Painting.Axis axis = default!, Widget child = default!)
    {
        this.stretchStrength = stretchStrength;
        this.axis = axis;
        this.child = child;
        System.Diagnostics.Debug.Assert(((stretchStrength >= -1.0) && (stretchStrength <= 1.0)));
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _StretchOverscrollEffectState__stretch_effect());
}

internal class _StretchOverscrollEffectState__stretch_effect : State<_StretchOverscrollEffect__stretch_effect>
{
    internal virtual global::Doroti.Ui.FragmentShader? _fragmentShader { get; set; } = default;
    public const double maxStretchIntensity = 1.0;
    public const double interpolationStrength = 0.7;
    internal static ImageFilter _emptyFilter = new global::Doroti.Ui.ImageFilter(Matrix4.identity().storage);

    public override void dispose()
    {
        this._fragmentShader?.dispose();
        base.dispose();
    }

    public override void initState()
    {
        base.initState();
        _StretchEffectShader__stretch_effect.initializeShader();
    }

    public override Widget build(BuildContext context)
    {
        bool isShaderNeeded__5894 = (((_StretchOverscrollEffect__stretch_effect)this.widget).stretchStrength.abs() > global::Doroti.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance);
        global::Doroti.Ui.ImageFilter imageFilter__5993 = default!;
        if (_StretchEffectShader__stretch_effect._initialized)
        {
            this._fragmentShader?.dispose();
            _fragmentShader = _StretchEffectShader__stretch_effect._program!.fragmentShader();
            this._fragmentShader!.setFloat(2L, maxStretchIntensity);
            if ((object.Equals(((_StretchOverscrollEffect__stretch_effect)this.widget).axis, global::Doroti.Framework.Painting.Axis.vertical)))
            {
                this._fragmentShader!.setFloat(3L, 0.0);
                this._fragmentShader!.setFloat(4L, ((_StretchOverscrollEffect__stretch_effect)this.widget).stretchStrength);
            }
            else
            {
                this._fragmentShader!.setFloat(3L, ((_StretchOverscrollEffect__stretch_effect)this.widget).stretchStrength);
                this._fragmentShader!.setFloat(4L, 0.0);
            }
            this._fragmentShader!.setFloat(5L, interpolationStrength);
            imageFilter__5993 = new global::Doroti.Ui.ImageFilter(this._fragmentShader!);
        }
        else
        {
            this._fragmentShader?.dispose();
            _fragmentShader = null;
            imageFilter__5993 = _emptyFilter;
        }
        return ((Widget)(object?)new ImageFiltered(imageFilter: imageFilter__5993, enabled: isShaderNeeded__5894, child: new CustomPaint(painter: (isShaderNeeded__5894 ? new _StretchEffectPainter__stretch_effect() : null), child: ((_StretchOverscrollEffect__stretch_effect)this.widget).child)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _StretchEffectPainter__stretch_effect : global::Doroti.Framework.Rendering.CustomPainter
{
    public override void paint(Canvas canvas, Size size)
    {
        var paint__7473 = ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Ui.Paint();
            __cascade.color = global::Doroti.Ui.Color.fromARGB(1L, 0L, 0L, 0L);
            __cascade.style = PaintingStyle.fill;
            return __cascade;        }))();
        canvas.drawPoints(global::Doroti.Ui.PointMode.points, new List<global::Doroti.Ui.Offset> { Offset.zero, new global::Doroti.Ui.Offset((size.width - 1L), 0), new global::Doroti.Ui.Offset(0, (size.height - 1L)), new global::Doroti.Ui.Offset((size.width - 1L), (size.height - 1L)) }, paint__7473);
    }

    public override bool shouldRepaint(global::Doroti.Framework.Rendering.CustomPainter oldDelegate) => false;
}

internal class _StretchEffectShader__stretch_effect
{
    internal static bool _initCalled = false;
    internal static bool _initialized = false;
    internal static global::Doroti.Ui.FragmentProgram? _program = default;

    public static void initializeShader()
    {
        if (!_initCalled)
        {
            DartRuntimePrimitives.Ignore(Dart_uiLibrary.FragmentProgram.fromAsset("shaders/stretch_effect.frag").then((global::System.Action<global::Doroti.Ui.FragmentProgram>)((program) => {
_program = program;
_initialized = true;
})));
            _initCalled = true;
        }
    }

}

