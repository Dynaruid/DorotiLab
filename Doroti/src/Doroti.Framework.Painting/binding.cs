// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/binding.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Painting;

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
    public Future<global::Doroti.Ui.Codec> instantiateImageCodecFromBuffer(ImmutableBuffer buffer, long? cacheWidth = null, long? cacheHeight = null, bool allowUpscaling = false);
    public Future<global::Doroti.Ui.Codec> instantiateImageCodecWithSize(ImmutableBuffer buffer, Func<long, long, TargetImageSize>? getTargetSize = null);
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
        foreach (Action callback in this._systemFontsCallbacks)
        {
            callback();
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

