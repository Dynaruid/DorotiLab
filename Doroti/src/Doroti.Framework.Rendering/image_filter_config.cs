// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/image_filter_config.dart
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
    public static ImageFilterConfig Create(ImageFilter filter)
        => new _DirectImageFilterConfig__image_filter_config(filter);

    protected ImageFilterConfig()
    {
    }

    public static ImageFilterConfig Create(ColorFilter filter) => Create(new ImageFilter(filter));

    public static ImageFilterConfig CreateBlur(double sigmaX = default!, double sigmaY = default!, TileMode tileMode = default!, bool bounded = default!)
        => new _BlurImageFilterConfig__image_filter_config(sigmaX, sigmaY, tileMode, bounded);

    public static ImageFilterConfig CreateCompose(ImageFilterConfig outer, ImageFilterConfig inner)
        => new _ComposeImageFilterConfig__image_filter_config(outer, inner);

    public abstract global::Doroti.Ui.ImageFilter resolve(ImageFilterContext context);
    public virtual global::Doroti.Ui.ImageFilter? filter => null;
    public abstract string debugShortDescription { get; }
    public override string ToString() => $"ImageFilterConfig.{this.debugShortDescription}";
}

internal class _DirectImageFilterConfig__image_filter_config : ImageFilterConfig
{
    private ImageFilter? __field_filter = default!;
    public override ImageFilter? filter { get => __field_filter; }

    internal _DirectImageFilterConfig__image_filter_config(ImageFilter filter)
    {
        this.__field_filter = filter;
    }

    public override ImageFilter resolve(ImageFilterContext context)
    {
        return this.filter;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as _DirectImageFilterConfig__image_filter_config;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((__other is _DirectImageFilterConfig__image_filter_config) && (object.Equals(((_DirectImageFilterConfig__image_filter_config)((_DirectImageFilterConfig__image_filter_config)__other)).filter, this.filter)));
    }

    public override int GetHashCode() => this.filter.GetHashCode();
    public override string debugShortDescription => this.filter.debugShortDescription;
    public override string ToString() => $"ImageFilterConfig({this.filter.debugShortDescription})";
}

internal class _BlurImageFilterConfig__image_filter_config : ImageFilterConfig
{
    public virtual double sigmaX { get; private set; } = default!;
    public virtual double sigmaY { get; private set; } = default!;
    public virtual TileMode tileMode { get; private set; } = default!;
    public virtual bool bounded { get; private set; } = default!;

    internal _BlurImageFilterConfig__image_filter_config(double sigmaX = 0.0, double sigmaY = 0.0, TileMode tileMode = TileMode.clamp, bool bounded = false)
    {
        this.sigmaX = sigmaX;
        this.sigmaY = sigmaY;
        this.tileMode = tileMode;
        this.bounded = bounded;
    }

    public override ImageFilter resolve(ImageFilterContext context)
    {
        return new global::Doroti.Ui.ImageFilter(sigmaX: this.sigmaX, sigmaY: this.sigmaY, tileMode: this.tileMode, bounds: (this.bounded ? ((ImageFilterContext)context).bounds : null));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as _BlurImageFilterConfig__image_filter_config;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((((__other is _BlurImageFilterConfig__image_filter_config) && (((_BlurImageFilterConfig__image_filter_config)((_BlurImageFilterConfig__image_filter_config)__other)).sigmaX == this.sigmaX)) && (((_BlurImageFilterConfig__image_filter_config)((_BlurImageFilterConfig__image_filter_config)__other)).sigmaY == this.sigmaY)) && (object.Equals(((_BlurImageFilterConfig__image_filter_config)((_BlurImageFilterConfig__image_filter_config)__other)).tileMode, this.tileMode))) && (((_BlurImageFilterConfig__image_filter_config)((_BlurImageFilterConfig__image_filter_config)__other)).bounded == this.bounded));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(this.sigmaX, this.sigmaY, this.tileMode, this.bounded);
    internal virtual string _modeString
    {
        get
        {
            switch (this.tileMode)
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
    internal virtual string _boundedString => (this.bounded ? "bounded" : "unbounded");
    public override string debugShortDescription => $"blur({this.sigmaX}, {this.sigmaY}, {this._modeString}, {this._boundedString})";
}

internal class _ComposeImageFilterConfig__image_filter_config : ImageFilterConfig
{
    public virtual ImageFilterConfig outer { get; private set; } = default!;
    public virtual ImageFilterConfig inner { get; private set; } = default!;

    internal _ComposeImageFilterConfig__image_filter_config(ImageFilterConfig outer, ImageFilterConfig inner)
    {
        this.outer = outer;
        this.inner = inner;
    }

    public override ImageFilter resolve(ImageFilterContext context)
    {
        return new global::Doroti.Ui.ImageFilter(outer: this.outer.resolve(context), inner: this.inner.resolve(context));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as _ComposeImageFilterConfig__image_filter_config;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((__other is _ComposeImageFilterConfig__image_filter_config) && (object.Equals(((_ComposeImageFilterConfig__image_filter_config)((_ComposeImageFilterConfig__image_filter_config)__other)).outer, this.outer))) && (object.Equals(((_ComposeImageFilterConfig__image_filter_config)((_ComposeImageFilterConfig__image_filter_config)__other)).inner, this.inner)));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(this.outer, this.inner);
    public override string debugShortDescription => $"{((ImageFilterConfig)this.inner).debugShortDescription} -> {((ImageFilterConfig)this.outer).debugShortDescription}";
    public override string ToString() => $"ImageFilterConfig.compose(source -> {this.debugShortDescription} -> result)";
}
