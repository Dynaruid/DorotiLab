using Doroti.Runtime;

namespace Doroti.Ui;

/// <summary>dart:ui and vector-math value types consumed by the Widgets source port.</summary>
public enum ClipOp { difference, intersect }
public enum PointMode { points, lines, polygon }
public enum DisplayFeatureType { unknown, fold, hinge, cutout }
public enum DisplayFeatureState { unknown, postureFlat, postureHalfOpened }

public readonly record struct RSTransform(double scos, double ssin, double tx, double ty)
{
    public static RSTransform fromComponents(
        double rotation,
        double scale,
        double anchorX,
        double anchorY,
        double translateX,
        double translateY)
    {
        var sin = Math.Sin(rotation) * scale;
        var cos = Math.Cos(rotation) * scale;
        return new(cos, sin, translateX - (cos * anchorX) + (sin * anchorY),
            translateY - (sin * anchorX) - (cos * anchorY));
    }
}

public sealed record DisplayFeature(Rect bounds, DisplayFeatureType type, DisplayFeatureState state);

public sealed class Vertices;

public sealed class FragmentProgram
{
    public static Future<FragmentProgram> fromAsset(string assetKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(assetKey);
        return Future<FragmentProgram>.value(new FragmentProgram());
    }

    public FragmentShader fragmentShader() => new(this);
}

public sealed class FragmentShader(FragmentProgram program) : Shader
{
    public FragmentProgram program { get; } = program;
    public void setFloat(long index, double value) { _ = index; _ = value; }
    public void setImageSampler(long index, Image image) { _ = index; _ = image; }
    public void dispose() { }
}

public sealed record Quad(Vector3 point0, Vector3 point1, Vector3 point2, Vector3 point3);
