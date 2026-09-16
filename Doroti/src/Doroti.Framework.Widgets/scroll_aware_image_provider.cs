// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/scroll_aware_image_provider.dart
using System.Diagnostics;
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public class ScrollAwareImageProvider<T> : global::Doroti.Framework.Painting.ImageProvider<T> where T : notnull
{
    public virtual IDisposableBuildContext context { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.ImageProvider<T> imageProvider { get; private set; } = default!;

    public ScrollAwareImageProvider(IDisposableBuildContext context, global::Doroti.Framework.Painting.ImageProvider<T> imageProvider)
    {
        this.context = context;
        this.imageProvider = imageProvider;
    }

    public override void resolveStreamForKey(global::Doroti.Framework.Painting.ImageConfiguration configuration, global::Doroti.Framework.Painting.ImageStream stream, T key, global::System.Action<object, global::System.Diagnostics.StackTrace?> handleError)
    {
        if ((stream.completer is not null) || PaintingBinding.instance.imageCache.containsKey(key))
        {
            imageProvider.resolveStreamForKey(configuration, stream, key, handleError);
            return;
        }
        BuildContext? buildContext = context.context;
        if (buildContext is null)
        {
            return;
        }
        if (Scrollable.recommendDeferredLoadingForContext(buildContext))
        {
            Scheduler.SchedulerBinding.instance.scheduleFrameCallback((_) =>
            {
                DartAsyncRuntime.scheduleMicrotask(() => { resolveStreamForKey(configuration, stream, key, handleError); });
            });
            return;
        }
        imageProvider.resolveStreamForKey(configuration, stream, key, handleError);
    }

    public override global::Doroti.Framework.Painting.ImageStreamCompleter loadBuffer(T key, DecoderBufferCallback decode) => imageProvider.loadBuffer(key, decode);
    public override global::Doroti.Framework.Painting.ImageStreamCompleter loadImage(T key, ImageDecoderCallback decode) => imageProvider.loadImage(key, decode);
    public override global::Doroti.Framework.Painting.ImageStreamCompleter loadBuffer(T key, Func<ImmutableBuffer, bool, long?, long?, Future<Codec>> decode) => imageProvider.loadBuffer(key, decode);
    public override global::Doroti.Framework.Painting.ImageStreamCompleter loadImage(T key, Func<ImmutableBuffer, Func<long, long, TargetImageSize>?, Future<Codec>> decode) => imageProvider.loadImage(key, decode);
    public override Future<T> obtainKey(global::Doroti.Framework.Painting.ImageConfiguration configuration) => imageProvider.obtainKey(configuration);
    public override bool Equals(object? other)
    {
        var __other = other as ScrollAwareImageProvider<T>;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is ScrollAwareImageProvider<T>) && Equals(context, __other.context) && Equals(imageProvider, __other.imageProvider);
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(context, imageProvider));
}

/// <summary>Scroll-aware wrapper for providers with an application-defined key type.</summary>
public class ScrollAwareImageProvider : ScrollAwareImageProvider<object>
{
    public ScrollAwareImageProvider(IDisposableBuildContext context, global::Doroti.Framework.Painting.IImageProvider imageProvider)
        : base(context, new ErasedProvider(imageProvider)) { }

    private sealed class ErasedProvider(global::Doroti.Framework.Painting.IImageProvider provider) : global::Doroti.Framework.Painting.ImageProvider<object>
    {
        public override Future<object> obtainKey(global::Doroti.Framework.Painting.ImageConfiguration configuration) => provider.obtainKeyObject(configuration);
        public override void resolveStreamForKey(global::Doroti.Framework.Painting.ImageConfiguration configuration, global::Doroti.Framework.Painting.ImageStream stream, object key, Action<object, StackTrace?> handleError) => provider.resolveStreamForKeyObject(configuration, stream, key, handleError);
        public override global::Doroti.Framework.Painting.ImageStreamCompleter loadBuffer(object key, Func<ImmutableBuffer, bool, long?, long?, Future<Codec>> decode) => provider.loadBufferObject(key, decode);
        public override global::Doroti.Framework.Painting.ImageStreamCompleter loadImage(object key, Func<ImmutableBuffer, Func<long, long, TargetImageSize>?, Future<Codec>> decode) => provider.loadImageObject(key, decode);
        public override bool Equals(object? other) => other is ErasedProvider erased && Equals(provider, erased.Provider);
        public override int GetHashCode() => provider.GetHashCode();
        private global::Doroti.Framework.Painting.IImageProvider Provider => provider;
    }
}
