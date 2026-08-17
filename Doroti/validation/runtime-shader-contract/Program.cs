using Doroti.Skia.RuntimeEffects;
using Doroti.Runtime;
using Doroti.Ui;
using Doroti.Framework.Rendering;
using SkiaSharp;
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
using (var inkSparkleStream = typeof(Doroti.Framework.Material.InkSparkle).Assembly
           .GetManifestResourceStream("Doroti.Framework.Material.Shaders.ink_sparkle.sksl")
       ?? throw new InvalidOperationException("The Material InkSparkle shader was not embedded."))
using (var inkSparkleReader = new StreamReader(inkSparkleStream))
{
    DorotiSkiaRuntimeEffects.Validate(inkSparkleReader.ReadToEnd(), "shaders/ink_sparkle.frag");
}
var offsetTween = new Doroti.Framework.Animation.Tween<System.Numerics.Vector2>(
    begin: new System.Numerics.Vector2(10, 20),
    end: new System.Numerics.Vector2(30, 60));
if (offsetTween.transform(0.25) != new System.Numerics.Vector2(15, 30))
    throw new InvalidOperationException("Tween<Offset> did not preserve Flutter double-factor interpolation.");
if (!ImageFilter.isShaderFilterSupported)
    throw new InvalidOperationException(
        "Shader image filters must be advertised now that every product host binds the filtered child on its GPU path.");
var shader = FragmentProgram.fromSource(source, "runtime-shader-contract").fragmentShader();
shader.setFloat(0, 320);
shader.setFloat(1, 80);
shader.setFloat(2, 0.75);
var compiledBeforeScalarShader = DorotiSkiaRuntimeEffects.CompiledEffectCountForValidation;
using (var nativeShader = DorotiSkiaRuntimeEffects.CreateShader(
    new FragmentShaderSnapshot(shader.CaptureState()),
    _ => throw new InvalidOperationException("The scalar-uniform fixture declares no image sampler.")))
{
}
if (DorotiSkiaRuntimeEffects.CompiledEffectCountForValidation != compiledBeforeScalarShader + 1)
    throw new InvalidOperationException(
        "Repeated runtime shader creation recompiled identical SkSL instead of reusing the compiled effect.");
using (var cachedNativeShader = DorotiSkiaRuntimeEffects.CreateShader(
    new FragmentShaderSnapshot(shader.CaptureState()),
    _ => throw new InvalidOperationException("The scalar-uniform fixture declares no image sampler.")))
{
}

const string imageFilterSource = """
    uniform float2 uSize;
    uniform shader uTexture;

    half4 main(float2 position) {
        if (uSize.x != 2.0 || uSize.y != 1.0) {
            return half4(1.0, 1.0, 0.0, 1.0);
        }
        half4 inputColor = uTexture.eval(position);
        return half4(inputColor.b, inputColor.g, inputColor.r, inputColor.a);
    }
    """;
var filterShader = FragmentProgram.fromSource(imageFilterSource, "runtime-image-filter-contract").fragmentShader();
var shaderFilter = new ImageFilter(filterShader, FilterQuality.none);
if (shaderFilter.filterQuality != FilterQuality.none)
    throw new InvalidOperationException("ImageFilter.shader did not retain its input sampling quality.");
using (var inputBitmap = new SKBitmap(new SKImageInfo(2, 1, SKColorType.Rgba8888, SKAlphaType.Premul)))
{
    inputBitmap.SetPixel(0, 0, SKColors.Red);
    inputBitmap.SetPixel(1, 0, SKColors.Green);
    using var inputImage = SKImage.FromBitmap(inputBitmap);
    using var outputBitmap = new SKBitmap(new SKImageInfo(2, 1, SKColorType.Rgba8888, SKAlphaType.Premul));
    using var outputCanvas = new SKCanvas(outputBitmap);
    var compiledBeforeImageFilter = DorotiSkiaRuntimeEffects.CompiledEffectCountForValidation;
    using var nativeFilterShader = DorotiSkiaRuntimeEffects.CreateImageFilterShader(
        new FragmentShaderSnapshot(filterShader.CaptureState()),
        inputImage,
        SKSamplingOptions.Default,
        _ => throw new InvalidOperationException("The implicit input is the only image-filter sampler."));
    using var cachedNativeFilterShader = DorotiSkiaRuntimeEffects.CreateImageFilterShader(
        new FragmentShaderSnapshot(filterShader.CaptureState()),
        inputImage,
        SKSamplingOptions.Default,
        _ => throw new InvalidOperationException("The implicit input is the only image-filter sampler."));
    if (DorotiSkiaRuntimeEffects.CompiledEffectCountForValidation != compiledBeforeImageFilter + 1)
        throw new InvalidOperationException(
            "Repeated image-filter shader creation recompiled identical SkSL instead of reusing the compiled effect.");
    using var filterPaint = new SKPaint { Shader = nativeFilterShader };
    outputCanvas.DrawRect(SKRect.Create(2, 1), filterPaint);
    var filteredRed = outputBitmap.GetPixel(0, 0);
    var filteredGreen = outputBitmap.GetPixel(1, 0);
    if (filteredRed.Blue < 250 || filteredRed.Red > 5 || filteredGreen.Green < 120)
        throw new InvalidOperationException(
            $"ImageFilter.shader did not sample and transform its implicit child: {filteredRed}, {filteredGreen}.");
}
filterShader.dispose();

var stretchShaderType = typeof(Doroti.Framework.Widgets.StretchEffect).Assembly.GetType(
    "Doroti.Framework.Widgets._StretchEffectShader__stretch_effect",
    throwOnError: true)!;
var stretchShaderSource = (string)stretchShaderType.GetField(
    "_source",
    BindingFlags.Static | BindingFlags.NonPublic)!.GetRawConstantValue()!;
DorotiSkiaRuntimeEffects.Validate(stretchShaderSource, "doroti-framework-stretch-effect");
var stretchShader = FragmentProgram.fromSource(
    stretchShaderSource,
    "doroti-framework-stretch-effect").fragmentShader();
_ = new ImageFilter(stretchShader);
stretchShader.dispose();

try
{
    _ = new ImageFilter(shader);
    throw new InvalidOperationException("A shader image filter without an implicit sampler unexpectedly passed validation.");
}
catch (InvalidOperationException error) when (error.Message.Contains("sampler", StringComparison.Ordinal))
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
