// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/binding.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622, CS8625, CS8714
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Painting;

public interface PaintingBinding
{
    public static PaintingBinding? _instance = default;
    public static ShaderWarmUp? shaderWarmUp = default;
    ImageCache _imageCache { get; set; }
    _SystemFontsNotifier__binding _systemFonts { get; }

    public static PaintingBinding instance
    {
        get => BindingBase.checkInstance(_instance);
    }
    public ImageCache imageCache { get; }
    public ImageCache createImageCache();
    public Future<global::Doroti.Flutter.Ui.Codec> instantiateImageCodecFromBuffer(ImmutableBuffer buffer, long? cacheWidth = null, long? cacheHeight = null, bool allowUpscaling = false);
    public Future<global::Doroti.Flutter.Ui.Codec> instantiateImageCodecWithSize(ImmutableBuffer buffer, Func<long, long, TargetImageSize>? getTargetSize = null);
    public void evict(string asset);
    public void handleMemoryPressure();
    public Listenable systemFonts { get; }
    public Future handleSystemMessage(object systemMessage);
    PlatformDispatcher platformDispatcher { get; }
}

public class _SystemFontsNotifier__binding : Listenable
{
    internal virtual HashSet<Action> _systemFontsCallbacks { get; private set; } = new HashSet<Action>();

    public virtual void notifyListeners()
    {
        foreach (Action callback__7695 in this._systemFontsCallbacks)
        {
            callback__7695();
        }
    }

    public virtual void addListener(Action listener)
    {
        this._systemFontsCallbacks.Add(listener);
    }

    public virtual void removeListener(Action listener)
    {
        this._systemFontsCallbacks.Remove(listener);
    }

}

public static partial class BindingLibrary
{
    public static ImageCache imageCache => PaintingBinding.instance.imageCache;
}

