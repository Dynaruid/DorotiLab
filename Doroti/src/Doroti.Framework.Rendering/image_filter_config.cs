// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/image_filter_config.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Rendering;

public class ImageFilterContext
{
    public virtual Rect bounds { get; private set; } = default!;

    public ImageFilterContext(Rect bounds)
    {
        this.bounds = bounds;
    }
}

public abstract class ImageFilterConfig
{
    public static ImageFilterConfig Create(ImageFilter filter) =>
        new _DirectImageFilterConfig__image_filter_config(filter);

    protected ImageFilterConfig() { }

    public static ImageFilterConfig Create(ColorFilter filter) => Create(new ImageFilter(filter));

    public static ImageFilterConfig CreateBlur(
        double sigmaX = default!,
        double sigmaY = default!,
        TileMode tileMode = default!,
        bool bounded = default!
    ) => new _BlurImageFilterConfig__image_filter_config(sigmaX, sigmaY, tileMode, bounded);

    public static ImageFilterConfig CreateCompose(
        ImageFilterConfig outer,
        ImageFilterConfig inner
    ) => new _ComposeImageFilterConfig__image_filter_config(outer, inner);

    public abstract ImageFilter resolve(ImageFilterContext context);
    public virtual ImageFilter? filter => null;
    public abstract string debugShortDescription { get; }

    public override string ToString() => $"ImageFilterConfig.{debugShortDescription}";
}

internal class _DirectImageFilterConfig__image_filter_config : ImageFilterConfig
{
    private readonly ImageFilter __field_filter;
    public override ImageFilter filter
    {
        get => __field_filter;
    }

    internal _DirectImageFilterConfig__image_filter_config(ImageFilter filter)
    {
        __field_filter = filter;
    }

    public override ImageFilter resolve(ImageFilterContext context)
    {
        return filter;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as _DirectImageFilterConfig__image_filter_config;
        if (__other is null)
        {
            return false;
        }

        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is _DirectImageFilterConfig__image_filter_config)
            && Equals(__other.filter, filter);
    }

    public override int GetHashCode() => filter.GetHashCode();

    public override string debugShortDescription => filter.debugShortDescription;

    public override string ToString() => $"ImageFilterConfig({filter.debugShortDescription})";
}

internal class _BlurImageFilterConfig__image_filter_config : ImageFilterConfig
{
    public virtual double sigmaX { get; private set; } = default!;
    public virtual double sigmaY { get; private set; } = default!;
    public virtual TileMode tileMode { get; private set; } = default!;
    public virtual bool bounded { get; private set; } = default!;

    internal _BlurImageFilterConfig__image_filter_config(
        double sigmaX = 0.0,
        double sigmaY = 0.0,
        TileMode tileMode = TileMode.clamp,
        bool bounded = false
    )
    {
        this.sigmaX = sigmaX;
        this.sigmaY = sigmaY;
        this.tileMode = tileMode;
        this.bounded = bounded;
    }

    public override ImageFilter resolve(ImageFilterContext context)
    {
        return new ImageFilter(
            sigmaX: sigmaX,
            sigmaY: sigmaY,
            tileMode: tileMode,
            bounds: bounded ? context.bounds : null
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as _BlurImageFilterConfig__image_filter_config;
        if (__other is null)
        {
            return false;
        }

        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is _BlurImageFilterConfig__image_filter_config)
            && (__other.sigmaX == sigmaX)
            && (__other.sigmaY == sigmaY)
            && Equals(__other.tileMode, tileMode)
            && (__other.bounded == bounded);
    }

    public override int GetHashCode() =>
        FoundationRuntimePorts.ObjectHash(sigmaX, sigmaY, tileMode, bounded);

    internal virtual string _modeString
    {
        get
        {
            switch (tileMode)
            {
                case Dart_uiLibrary.TileMode.clamp:
                {
                    return "clamp";
                }
                case Dart_uiLibrary.TileMode.mirror:
                {
                    return "mirror";
                }
                case Dart_uiLibrary.TileMode.repeated:
                {
                    return "repeated";
                }
                case Dart_uiLibrary.TileMode.decal:
                {
                    return "decal";
                }
            }
            return default!;
        }
    }
    internal virtual string _boundedString => bounded ? "bounded" : "unbounded";
    public override string debugShortDescription =>
        $"blur({sigmaX}, {sigmaY}, {_modeString}, {_boundedString})";
}

internal class _ComposeImageFilterConfig__image_filter_config : ImageFilterConfig
{
    public virtual ImageFilterConfig outer { get; private set; } = default!;
    public virtual ImageFilterConfig inner { get; private set; } = default!;

    internal _ComposeImageFilterConfig__image_filter_config(
        ImageFilterConfig outer,
        ImageFilterConfig inner
    )
    {
        this.outer = outer;
        this.inner = inner;
    }

    public override ImageFilter resolve(ImageFilterContext context)
    {
        return new ImageFilter(outer: outer.resolve(context), inner: inner.resolve(context));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as _ComposeImageFilterConfig__image_filter_config;
        if (__other is null)
        {
            return false;
        }

        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is _ComposeImageFilterConfig__image_filter_config)
            && Equals(__other.outer, outer)
            && Equals(__other.inner, inner);
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(outer, inner);

    public override string debugShortDescription =>
        $"{inner.debugShortDescription} -> {outer.debugShortDescription}";

    public override string ToString() =>
        $"ImageFilterConfig.compose(source -> {debugShortDescription} -> result)";
}
