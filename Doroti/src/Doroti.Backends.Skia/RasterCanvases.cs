using Doroti.Composition;
using Doroti.Graphics;
using Doroti.Vendor.Avalonia.Skia;

namespace Doroti.Backends.Skia;

internal sealed class NativeRasterCanvas(INativeRasterFrame frame) : IRasterCanvas
{
    private readonly Stack<double> _opacities = [];
    private readonly Stack<bool> _boundedLayers = [];
    private double _opacity = 1;

    public int SaveCount => _opacities.Count + 1;

    public void Save()
    {
        _opacities.Push(_opacity);
        _boundedLayers.Push(false);
        frame.Save();
    }

    public void SaveLayer(RasterLayerOptions options)
    {
        options.Validate();
        _opacities.Push(_opacity);
        var bounds = options.Bounds;
        if (bounds is { } layerBounds)
        {
            frame.Save();
            frame.ClipRect(layerBounds.Left, layerBounds.Top, layerBounds.Right, layerBounds.Bottom);
        }
        _boundedLayers.Push(bounds.HasValue);
        frame.SaveLayer(new NativeLayerOptions(
            bounds.HasValue, bounds?.Left ?? 0, bounds?.Top ?? 0, bounds?.Right ?? 0, bounds?.Bottom ?? 0,
            options.Opacity, (int)options.BlendMode, ConvertImageFilter(options.ImageFilter),
            ConvertImageFilter(options.BackdropFilter), ConvertColorFilter(options.ColorFilter)));

        static NativeImageFilterOptions? ConvertImageFilter(RasterImageFilter? filter) => filter is null ? null : new(
            (int)filter.Kind, filter.SigmaX, filter.SigmaY, (int)filter.TileMode,
            filter.Matrix?.ToArray(), ConvertImageFilter(filter.Outer), ConvertImageFilter(filter.Inner),
            ConvertColorFilter(filter.ColorFilter));
        static NativeColorFilterOptions? ConvertColorFilter(RasterColorFilter? filter) => filter is null ? null : new(
            (int)filter.Kind, filter.Color?.Value ?? 0, (int)filter.BlendMode,
            filter.Matrix?.Select(value => (float)value).ToArray());
    }

    public void Restore()
    {
        frame.Restore();
        if (_boundedLayers.Pop()) frame.Restore();
        _opacity = _opacities.Pop();
    }

    public void Transform(Matrix transform) => frame.Transform(
        [
            transform.M11, transform.M12, transform.M13, transform.M14,
            transform.M21, transform.M22, transform.M23, transform.M24,
            transform.M31, transform.M32, transform.M33, transform.M34,
            transform.M41, transform.M42, transform.M43, transform.M44,
        ]);

    public void ClipRect(Rect rect) => frame.ClipRect(rect.Left, rect.Top, rect.Right, rect.Bottom);

    public void ClipPath(PathGeometry path)
    {
        var coordinates = new double[path.Points.Count * 2];
        for (var index = 0; index < path.Points.Count; index++)
        {
            coordinates[index * 2] = path.Points[index].X;
            coordinates[(index * 2) + 1] = path.Points[index].Y;
        }
        frame.ClipPath(coordinates, path.IsClosed, path.FillRule is PathFillRule.EvenOdd);
    }

    public void MultiplyOpacity(double opacity)
    {
        ValidateOpacity(opacity);
        _opacity *= opacity;
    }

    public void DrawColor(Color color) => frame.DrawRect(
        0,
        0,
        frame.Descriptor.Width,
        frame.Descriptor.Height,
        color.Value,
        _opacity);

    public void DrawRect(Rect rect, RasterPaint paint)
    {
        paint.Validate();
        frame.DrawRect(rect.Left, rect.Top, rect.Right, rect.Bottom, paint.Color.Value, paint.Opacity * _opacity);
    }

    public void DrawPath(PathGeometry path, RasterPaint paint)
    {
        paint.Validate();
        var coordinates = new double[path.Points.Count * 2];
        for (var index = 0; index < path.Points.Count; index++)
        {
            coordinates[index * 2] = path.Points[index].X;
            coordinates[(index * 2) + 1] = path.Points[index].Y;
        }
        frame.DrawPath(coordinates, path.IsClosed, path.FillRule is PathFillRule.EvenOdd,
            paint.Color.Value, paint.Opacity * _opacity, paint.BlurSigma,
            paint.Style is RasterPaintStyle.Stroke, paint.StrokeWidth);
    }

    public void DrawImage(ImageResourceSnapshot image, Rect source, Rect destination, double opacity = 1) =>
        frame.DrawImage(
            image.Pixels.AsSpan(),
            image.Width,
            image.Height,
            source.Left,
            source.Top,
            source.Right,
            source.Bottom,
            destination.Left,
            destination.Top,
            destination.Right,
            destination.Bottom,
            opacity * _opacity);

    public void DrawText(string text, Offset origin, double fontSize, RasterPaint paint, string? fontFamily = null)
    {
        paint.Validate();
        frame.DrawText(text, origin.X, origin.Y, fontSize, paint.Color.Value, paint.Opacity * _opacity, fontFamily);
    }

    private static void ValidateOpacity(double opacity)
    {
        if (!double.IsFinite(opacity) || opacity is < 0 or > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(opacity));
        }
    }
}

internal sealed class SoftwareRasterCanvas : IRasterCanvas
{
    private readonly byte[] _pixels;
    private readonly int _width;
    private readonly int _height;
    private readonly Stack<State> _states = [];
    private readonly Stack<LayerState?> _layers = [];
    private State _state;

    internal SoftwareRasterCanvas(byte[] pixels, int width, int height)
    {
        _pixels = pixels;
        _width = width;
        _height = height;
        _state = new(Matrix.Identity, new(0, 0, width, height), []);
    }

    public int SaveCount => _states.Count + 1;

    public void Save()
    {
        _states.Push(_state);
        _layers.Push(null);
    }

    public void SaveLayer(RasterLayerOptions options)
    {
        options.Validate();
        _states.Push(_state);
        var bounds = options.Bounds is { } requested
            ? _state.Transform.TransformBounds(requested).Intersect(_state.ClipBounds)
            : _state.ClipBounds;
        var original = _pixels.ToArray();
        _layers.Push(new LayerState(options, original, bounds, _state.ClipPaths));
        _pixels.AsSpan().Clear();
        if (options.BackdropFilter is { } backdrop)
            BgraRasterEffects.ApplyImageFilter(original, _width, _height, backdrop).CopyTo(_pixels, 0);
        _state = _state with { ClipBounds = bounds };
    }

    public void Restore()
    {
        if (_states.Count == 0)
        {
            throw new InvalidOperationException("Raster canvas restore is unbalanced.");
        }
        var layer = _layers.Pop();
        var restored = _states.Pop();
        if (layer is not null)
        {
            var source = _pixels.ToArray();
            if (layer.Options.ImageFilter is { } imageFilter)
                source = BgraRasterEffects.ApplyImageFilter(source, _width, _height, imageFilter);
            if (layer.Options.ColorFilter is { } colorFilter)
                source = BgraRasterEffects.ApplyColorFilter(source, colorFilter);
            MaskOutsideClip(source, layer);
            layer.Destination.CopyTo(_pixels, 0);
            BgraRasterEffects.Composite(_pixels, source, _width, _height, layer.Bounds,
                layer.Options.Opacity, layer.Options.BlendMode);
        }
        _state = restored;
    }

    public void Transform(Matrix transform)
    {
        if (!transform.IsFinite)
        {
            throw new ArgumentException("Transform must be finite.", nameof(transform));
        }
        _state = _state with { Transform = _state.Transform * transform };
    }

    public void ClipRect(Rect rect)
    {
        ValidateRect(rect);
        _state = _state with { ClipBounds = _state.ClipBounds.Intersect(_state.Transform.TransformBounds(rect)) };
    }

    public void ClipPath(PathGeometry path)
    {
        ArgumentNullException.ThrowIfNull(path);
        var points = path.Points.Select(_state.Transform.Transform).ToArray();
        _state = _state with
        {
            ClipBounds = BoundsOf(points).Intersect(_state.ClipBounds),
            ClipPaths = [.. _state.ClipPaths, new ClipMask(points, path.FillRule)],
        };
    }

    public void MultiplyOpacity(double opacity)
    {
        if (!double.IsFinite(opacity) || opacity is < 0 or > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(opacity));
        }
        _state = _state with { Opacity = _state.Opacity * opacity };
    }

    public void DrawColor(Color color) => FillRect(_state.ClipBounds, color, _state.Opacity);

    public void DrawRect(Rect rect, RasterPaint paint)
    {
        ValidateRect(rect);
        paint.Validate();
        var target = _state.Transform.TransformBounds(rect).Intersect(_state.ClipBounds);
        if (paint.Style == RasterPaintStyle.Fill)
        {
            FillRect(target, paint.Color, paint.Opacity * _state.Opacity);
            return;
        }
        var width = Math.Max(1, paint.StrokeWidth);
        var middleTop = Math.Min(target.Bottom, target.Top + width);
        var middleBottom = Math.Max(middleTop, target.Bottom - width);
        FillRect(new(target.Left, target.Top, target.Right, Math.Min(target.Bottom, target.Top + width)), paint.Color, paint.Opacity * _state.Opacity);
        FillRect(new(target.Left, Math.Max(target.Top, target.Bottom - width), target.Right, target.Bottom), paint.Color, paint.Opacity * _state.Opacity);
        FillRect(new(target.Left, middleTop, Math.Min(target.Right, target.Left + width), middleBottom), paint.Color, paint.Opacity * _state.Opacity);
        FillRect(new(Math.Max(target.Left, target.Right - width), middleTop, target.Right, middleBottom), paint.Color, paint.Opacity * _state.Opacity);
    }

    public void DrawPath(PathGeometry path, RasterPaint paint)
    {
        ArgumentNullException.ThrowIfNull(path);
        paint.Validate();
        var points = path.Points.Select(_state.Transform.Transform).ToArray();
        var pathBounds = BoundsOf(points);
        var blurExtent = paint.BlurSigma > 0 ? paint.BlurSigma * 3 : 0;
        var bounds = new Rect(
            pathBounds.Left - blurExtent,
            pathBounds.Top - blurExtent,
            pathBounds.Right + blurExtent,
            pathBounds.Bottom + blurExtent).Intersect(_state.ClipBounds);
        VisitPixels(bounds, (x, y) =>
        {
            var centerX = x + 0.5;
            var centerY = y + 0.5;
            var hit = paint.Style == RasterPaintStyle.Stroke
                ? IsNearEdge(points, path.IsClosed, centerX, centerY, Math.Max(1, paint.StrokeWidth) / 2)
                : Contains(points, centerX, centerY, path.FillRule);
            var coverage = hit ? 1 : BlurCoverage(points, path.IsClosed, centerX, centerY, paint.BlurSigma);
            if (coverage > 0.001)
            {
                Blend(x, y, paint.Color, paint.Opacity * _state.Opacity * coverage);
            }
        });
    }

    public void DrawImage(ImageResourceSnapshot image, Rect source, Rect destination, double opacity = 1)
    {
        ArgumentNullException.ThrowIfNull(image);
        ValidateRect(source);
        ValidateRect(destination);
        if (!double.IsFinite(opacity) || opacity is < 0 or > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(opacity));
        }
        var target = _state.Transform.TransformBounds(destination).Intersect(_state.ClipBounds);
        var pixels = image.Pixels;
        VisitPixels(target, (x, y) =>
        {
            var u = (x + 0.5 - target.Left) / Math.Max(1, target.Width);
            var v = (y + 0.5 - target.Top) / Math.Max(1, target.Height);
            var sourceX = Math.Clamp((int)(source.Left + (u * source.Width)), 0, image.Width - 1);
            var sourceY = Math.Clamp((int)(source.Top + (v * source.Height)), 0, image.Height - 1);
            var index = ((sourceY * image.Width) + sourceX) * 4;
            Blend(
                x,
                y,
                Color.FromArgb(pixels[index + 3], pixels[index + 2], pixels[index + 1], pixels[index]),
                opacity * _state.Opacity);
        });
    }

    public void DrawText(string text, Offset origin, double fontSize, RasterPaint paint, string? fontFamily = null)
    {
        ArgumentNullException.ThrowIfNull(text);
        if (!origin.IsFinite || !double.IsFinite(fontSize) || fontSize <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(fontSize), "Text geometry must be finite and positive.");
        }
        paint.Validate();
        var width = fontSize * 0.6;
        var cursor = origin.X;
        foreach (var character in text)
        {
            if (!char.IsWhiteSpace(character))
            {
                DrawRect(Rect.FromLeftTopWidthHeight(cursor, origin.Y - fontSize, width * 0.8, fontSize), paint);
            }
            cursor += width;
        }
    }

    internal void Clear(Color color)
    {
        for (var index = 0; index < _pixels.Length; index += 4)
        {
            _pixels[index] = color.Blue;
            _pixels[index + 1] = color.Green;
            _pixels[index + 2] = color.Red;
            _pixels[index + 3] = color.Alpha;
        }
    }

    private static bool Contains(IReadOnlyList<Offset> points, double x, double y, PathFillRule fillRule = PathFillRule.EvenOdd)
    {
        if (points.Count < 3) return false;
        if (fillRule == PathFillRule.NonZero)
        {
            var winding = 0;
            for (int current = 0, previous = points.Count - 1; current < points.Count; previous = current++)
            {
                var start = points[previous];
                var end = points[current];
                if (start.Y <= y)
                {
                    if (end.Y > y && IsLeft(start, end, x, y) > 0) winding++;
                }
                else if (end.Y <= y && IsLeft(start, end, x, y) < 0)
                {
                    winding--;
                }
            }
            return winding != 0;
        }
        var inside = false;
        for (int current = 0, previous = points.Count - 1; current < points.Count; previous = current++)
        {
            var a = points[current];
            var b = points[previous];
            if (((a.Y > y) != (b.Y > y)) && x < ((b.X - a.X) * (y - a.Y) / (b.Y - a.Y)) + a.X)
            {
                inside = !inside;
            }
        }
        return inside;
    }

    private static double IsLeft(Offset start, Offset end, double x, double y) =>
        ((end.X - start.X) * (y - start.Y)) - ((x - start.X) * (end.Y - start.Y));

    private static double BlurCoverage(IReadOnlyList<Offset> points, bool closed, double x, double y, double sigma)
    {
        if (sigma <= 0 || points.Count < 2) return 0;
        var distanceSquared = double.PositiveInfinity;
        for (var index = 1; index < points.Count; index++)
            distanceSquared = Math.Min(distanceSquared, DistanceSquaredToSegment(points[index - 1], points[index], x, y));
        if (closed) distanceSquared = Math.Min(distanceSquared, DistanceSquaredToSegment(points[^1], points[0], x, y));
        return 0.5 * Math.Exp(-distanceSquared / (2 * sigma * sigma));
    }

    private static bool IsNearEdge(IReadOnlyList<Offset> points, bool closed, double x, double y, double radius)
    {
        var radiusSquared = radius * radius;
        for (var current = 1; current < points.Count; current++)
        {
            if (NearSegment(points[current - 1], points[current])) return true;
        }
        return closed && points.Count > 1 && NearSegment(points[^1], points[0]);

        bool NearSegment(Offset start, Offset end)
        {
            return DistanceSquaredToSegment(start, end, x, y) <= radiusSquared;
        }
    }

    private static double DistanceSquaredToSegment(Offset start, Offset end, double x, double y)
    {
        var dx = end.X - start.X;
        var dy = end.Y - start.Y;
        var lengthSquared = (dx * dx) + (dy * dy);
        var t = lengthSquared == 0 ? 0 : Math.Clamp((((x - start.X) * dx) + ((y - start.Y) * dy)) / lengthSquared, 0, 1);
        var offsetX = x - (start.X + (t * dx));
        var offsetY = y - (start.Y + (t * dy));
        return (offsetX * offsetX) + (offsetY * offsetY);
    }

    private static Rect BoundsOf(IReadOnlyList<Offset> points)
    {
        if (points.Count == 0) return Rect.Zero;
        return new Rect(points.Min(point => point.X), points.Min(point => point.Y), points.Max(point => point.X), points.Max(point => point.Y));
    }

    private void FillRect(Rect rect, Color color, double opacity) => VisitPixels(rect, (x, y) => Blend(x, y, color, opacity));

    private void VisitPixels(Rect rect, Action<int, int> action)
    {
        var left = Math.Clamp((int)Math.Floor(rect.Left), 0, _width);
        var top = Math.Clamp((int)Math.Floor(rect.Top), 0, _height);
        var right = Math.Clamp((int)Math.Ceiling(rect.Right), 0, _width);
        var bottom = Math.Clamp((int)Math.Ceiling(rect.Bottom), 0, _height);
        for (var y = top; y < bottom; y++)
        {
            for (var x = left; x < right; x++)
            {
                if (_state.ClipPaths.All(path => Contains(path.Points, x + 0.5, y + 0.5, path.FillRule)))
                    action(x, y);
            }
        }
    }

    private void Blend(int x, int y, Color color, double opacity)
    {
        var index = ((y * _width) + x) * 4;
        var alpha = (color.Alpha / 255d) * opacity;
        var inverse = 1 - alpha;
        _pixels[index] = (byte)Math.Round((color.Blue * alpha) + (_pixels[index] * inverse));
        _pixels[index + 1] = (byte)Math.Round((color.Green * alpha) + (_pixels[index + 1] * inverse));
        _pixels[index + 2] = (byte)Math.Round((color.Red * alpha) + (_pixels[index + 2] * inverse));
        _pixels[index + 3] = (byte)Math.Round((255 * alpha) + (_pixels[index + 3] * inverse));
    }

    private static void ValidateRect(Rect rect)
    {
        if (!rect.IsFinite)
        {
            throw new ArgumentException("Rectangle must be finite.", nameof(rect));
        }
    }

    private void MaskOutsideClip(byte[] pixels, LayerState layer)
    {
        if (layer.ClipPaths.Count == 0) return;
        var left = Math.Clamp((int)Math.Floor(layer.Bounds.Left), 0, _width);
        var top = Math.Clamp((int)Math.Floor(layer.Bounds.Top), 0, _height);
        var right = Math.Clamp((int)Math.Ceiling(layer.Bounds.Right), 0, _width);
        var bottom = Math.Clamp((int)Math.Ceiling(layer.Bounds.Bottom), 0, _height);
        for (var y = top; y < bottom; y++)
        for (var x = left; x < right; x++)
        {
            if (layer.ClipPaths.All(path => Contains(path.Points, x + 0.5, y + 0.5, path.FillRule))) continue;
            pixels.AsSpan(((y * _width) + x) * 4, 4).Clear();
        }
    }

    private sealed record ClipMask(IReadOnlyList<Offset> Points, PathFillRule FillRule);
    private sealed record LayerState(
        RasterLayerOptions Options,
        byte[] Destination,
        Rect Bounds,
        IReadOnlyList<ClipMask> ClipPaths);
    private readonly record struct State(Matrix Transform, Rect ClipBounds, IReadOnlyList<ClipMask> ClipPaths, double Opacity = 1);
}
