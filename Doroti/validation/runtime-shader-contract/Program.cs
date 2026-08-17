using Doroti.Skia.RuntimeEffects;
using Doroti.Runtime;
using Doroti.Ui;
using Doroti.Framework.Rendering;
using System.Reflection;

const string source = """
    uniform float2 uSize;
    uniform float uPhase;

    half4 main(float2 position) {
        float2 uv = position / max(uSize, float2(1.0));
        return half4(uv.x, 0.35 + 0.35 * sin(uPhase + uv.x * 6.2831853), uv.y, 1.0);
    }
    """;

DorotiSkiaRuntimeEffects.Validate(source, "runtime-shader-contract");
if (ImageFilter.isShaderFilterSupported)
    throw new InvalidOperationException(
        "Shader image filters must remain unadvertised until the host can bind the filtered child as an implicit texture input.");
var shader = FragmentProgram.fromSource(source, "runtime-shader-contract").fragmentShader();
shader.setFloat(0, 320);
shader.setFloat(1, 80);
shader.setFloat(2, 0.75);
using (var nativeShader = DorotiSkiaRuntimeEffects.CreateShader(
    new FragmentShaderSnapshot(shader.CaptureState()),
    _ => throw new InvalidOperationException("The scalar-uniform fixture declares no image sampler.")))
{
}
using (var cachedNativeShader = DorotiSkiaRuntimeEffects.CreateShader(
    new FragmentShaderSnapshot(shader.CaptureState()),
    _ => throw new InvalidOperationException("The scalar-uniform fixture declares no image sampler.")))
{
}

try
{
    DorotiSkiaRuntimeEffects.Validate("half4 main(float2 p) { syntax error }", "invalid-contract");
    throw new InvalidOperationException("Invalid SkSL unexpectedly compiled.");
}
catch (InvalidDataException)
{
}

shader.dispose();

var alignedScale = Matrix4.translationValues(10, 20, 0);
alignedScale.multiply(Matrix4.diagonal3Values(2, 3, 1));
alignedScale.translateByDouble(-10, -20, 0, 1);
var fixedPoint = alignedScale.transform(new System.Numerics.Vector4(10, 20, 0, 1));
var scaledPoint = alignedScale.transform(new System.Numerics.Vector4(11, 21, 0, 1));
if (fixedPoint != new System.Numerics.Vector4(10, 20, 0, 1) ||
    scaledPoint != new System.Numerics.Vector4(12, 23, 0, 1))
    throw new InvalidOperationException(
        "Matrix4 translation did not post-multiply, so aligned Flutter transforms cannot preserve their origin.");

var renderClip = new RenderClipRect(clipBehavior: Clip.hardEdge);
typeof(RenderBox).GetProperty("_size", BindingFlags.Instance | BindingFlags.NonPublic)!
    .SetValue(renderClip, new Size(320, 170));
var customClipBase = renderClip.GetType().BaseType!;
customClipBase.GetMethod("_updateClip", BindingFlags.Instance | BindingFlags.NonPublic)!
    .Invoke(renderClip, null);
var computedClip = (Rect)customClipBase
    .GetProperty("_clip", BindingFlags.Instance | BindingFlags.NonPublic)!
    .GetValue(renderClip)!;
if (computedClip != Rect.fromLTWH(0, 0, 320, 170))
    throw new InvalidOperationException(
        "Generic custom clips did not replace their value-type default sentinel with the render box bounds.");

var clampingScroll = new Doroti.Framework.Widgets.ClampingScrollSimulation(
    position: 25,
    velocity: 1000);
var clampedPosition = clampingScroll.x(0.1);
if (!double.IsFinite(clampedPosition) || clampedPosition <= 25 ||
    clampingScroll.dx(0.1) >= 1000 || clampingScroll.isDone(0))
    throw new InvalidOperationException(
        "Android clamping scroll physics did not initialize its duration and distance.");

var bouncingSpring = new Doroti.Framework.Physics.SpringDescription(
    mass: 1,
    stiffness: 100,
    damping: 20);
var bouncingScroll = new Doroti.Framework.Widgets.BouncingScrollSimulation(
    position: 50,
    velocity: 500,
    leadingExtent: 0,
    trailingExtent: 100,
    spring: bouncingSpring);
var bouncingPosition = bouncingScroll.x(0.05);
var overscrollRecovery = new Doroti.Framework.Widgets.BouncingScrollSimulation(
    position: 110,
    velocity: 0,
    leadingExtent: 0,
    trailingExtent: 100,
    spring: bouncingSpring).x(0.1);
if (!double.IsFinite(bouncingPosition) || bouncingPosition <= 50 ||
    !double.IsFinite(overscrollRecovery) || overscrollRecovery >= 110)
    throw new InvalidOperationException(
        "Bouncing scroll physics did not initialize its friction and spring simulations.");

var recorder = new PictureRecorder();
var canvas = new Canvas(recorder);
var pointsPaint = new Paint { color = new Color(0xff336699), strokeWidth = 3 };
canvas.drawPoints(PointMode.lines, [new Offset(1, 2), new Offset(3, 4)], pointsPaint);
canvas.drawRawPoints(PointMode.polygon, new Float32List([5f, 6f, 7f, 8f, 9f, 10f]), pointsPaint);
using var picture = recorder.endRecording();
if (picture.Commands.Count != 2 ||
    picture.Commands[0].HostPayload is not CanvasPointsPayload { PointMode: PointMode.lines, Points.Count: 2 } ||
    picture.Commands[1].HostPayload is not CanvasPointsPayload { PointMode: PointMode.polygon, Points.Count: 3 })
    throw new InvalidOperationException("Point drawing commands did not retain their mode, coordinates, and paint payload.");
try
{
    canvas.drawRawPoints(PointMode.points, new Float32List([1f, 2f, 3f]), pointsPaint);
    throw new InvalidOperationException("Odd raw-point coordinates unexpectedly passed validation.");
}
catch (ArgumentException)
{
}

Console.WriteLine("Doroti runtime shader contract: PASS");
