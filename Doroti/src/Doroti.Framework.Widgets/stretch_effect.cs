// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/stretch_effect.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public class StretchEffect : StatelessWidget
{
    public virtual double stretchStrength { get; private set; } = default!;
    public virtual Axis axis { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;

    public StretchEffect(Key? key = null, double stretchStrength = 0.0, Axis axis = default!, Widget child = default!) : base(key: key)
    {
        this.stretchStrength = stretchStrength;
        this.axis = axis;
        this.child = child;
        System.Diagnostics.Debug.Assert((stretchStrength >= -1.0) && (stretchStrength <= 1.0));
    }

    internal virtual AlignmentGeometry _getAlignment(TextDirection direction)
    {
        bool isForward = stretchStrength > 0L;
        if (Equals(axis, Axis.vertical))
        {
            return isForward ? AlignmentDirectional.topCenter : AlignmentDirectional.bottomCenter;
        }
        if (Equals(direction, TextDirection.rtl))
        {
            return isForward ? AlignmentDirectional.centerEnd : AlignmentDirectional.centerStart;
        }
        else
        {
            return isForward ? AlignmentDirectional.centerStart : AlignmentDirectional.centerEnd;
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        if (ImageFilter.isShaderFilterSupported)
        {
            return new _StretchOverscrollEffect__stretch_effect(stretchStrength: stretchStrength, axis: axis, child: child);
        }
        TextDirection textDirection = Directionality.of(context);
        var x = 1.0;
        var y = 1.0;
        switch (axis)
        {
            case Axis.horizontal:
                {
                    x += stretchStrength.abs();
                    break;
                }
            case Axis.vertical:
                {
                    y += stretchStrength.abs();
                    break;
                }
        }
        return new Transform(alignment: _getAlignment(textDirection), transform: Matrix4.diagonal3Values(x, y, 1.0), filterQuality: (stretchStrength == 0L) ? null : FilterQuality.medium, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _StretchOverscrollEffect__stretch_effect : StatefulWidget
{
    public virtual double stretchStrength { get; private set; } = default!;
    public virtual Axis axis { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;

    internal _StretchOverscrollEffect__stretch_effect(double stretchStrength = 0.0, Axis axis = default!, Widget child = default!)
    {
        this.stretchStrength = stretchStrength;
        this.axis = axis;
        this.child = child;
        System.Diagnostics.Debug.Assert((stretchStrength >= -1.0) && (stretchStrength <= 1.0));
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _StretchOverscrollEffectState__stretch_effect());
}

internal class _StretchOverscrollEffectState__stretch_effect : State<_StretchOverscrollEffect__stretch_effect>
{
    internal virtual FragmentShader? _fragmentShader { get; set; } = default;
    public const double maxStretchIntensity = 1.0;
    public const double interpolationStrength = 0.7;
    internal static ImageFilter _emptyFilter = new ImageFilter(Matrix4.identity().storage);

    public override void dispose()
    {
        _fragmentShader?.dispose();
        base.dispose();
    }

    public override void initState()
    {
        base.initState();
        _StretchEffectShader__stretch_effect.initializeShader(() =>
        {
            if (mounted)
                setState(() => { });
        });
    }

    public override Widget build(BuildContext context)
    {
        bool isShaderNeeded = widget.stretchStrength.abs() > Foundation.ConstantsLibrary.precisionErrorTolerance;
        ImageFilter imageFilterLocal = default!;
        if (isShaderNeeded && _StretchEffectShader__stretch_effect._initialized)
        {
            _fragmentShader?.dispose();
            _fragmentShader = _StretchEffectShader__stretch_effect._program!.fragmentShader();
            _fragmentShader!.setFloat(2L, maxStretchIntensity);
            if (Equals(widget.axis, Axis.vertical))
            {
                _fragmentShader!.setFloat(3L, 0.0);
                _fragmentShader!.setFloat(4L, widget.stretchStrength);
            }
            else
            {
                _fragmentShader!.setFloat(3L, widget.stretchStrength);
                _fragmentShader!.setFloat(4L, 0.0);
            }
            _fragmentShader!.setFloat(5L, interpolationStrength);
            imageFilterLocal = new ImageFilter(_fragmentShader!);
        }
        else
        {
            _fragmentShader?.dispose();
            _fragmentShader = null;
            imageFilterLocal = _emptyFilter;
        }
        return new ImageFiltered(imageFilter: imageFilterLocal, enabled: isShaderNeeded, child: new CustomPaint(painter: isShaderNeeded ? new _StretchEffectPainter__stretch_effect() : null, child: widget.child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _StretchEffectPainter__stretch_effect : CustomPainter
{
    public override void paint(Canvas canvas, Size size)
    {
        var paintLocal = ((Func<Paint>)(() =>
{
    var __cascade = new Paint();
    __cascade.color = Color.fromARGB(1L, 0L, 0L, 0L);
    __cascade.style = PaintingStyle.fill;
    return __cascade;
}))();
        canvas.drawPoints(PointMode.points, new List<Offset> { Offset.zero, new Offset(size.width - 1L, 0), new Offset(0, size.height - 1L), new Offset(size.width - 1L, size.height - 1L) }, paintLocal);
    }

    public override bool shouldRepaint(CustomPainter oldDelegate) => false;
}

internal class _StretchEffectShader__stretch_effect
{
    internal static bool _initCalled = false;
    internal static bool _initialized = false;
    internal static FragmentProgram? _program = default;

    public static void initializeShader(
        Action? onReady = null,
        Action<Exception>? onError = null)
    {
        if (_initialized && _program is not null)
        {
            onReady?.Invoke();
            return;
        }
        _initCalled = true;
        FrameworkShaderLoader.RegisterResourceOwner(typeof(Widget).Assembly);
        FrameworkShaderLoader.BeginLoad(
            "widgets.stretch-effect",
            program =>
            {
                _program = program;
                _initialized = true;
                onReady?.Invoke();
            },
            onError);
    }

}
