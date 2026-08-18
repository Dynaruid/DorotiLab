// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/scroll_aware_image_provider.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Widgets;

public class ScrollAwareImageProvider<T> : global::Doroti.Framework.Painting.ImageProvider<T>
{
    public virtual dynamic context { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.ImageProvider<T> imageProvider { get; private set; } = default!;

    public ScrollAwareImageProvider(dynamic context, global::Doroti.Framework.Painting.ImageProvider<T> imageProvider)
    {
        this.context = context;
        this.imageProvider = imageProvider;
    }

    public override void resolveStreamForKey(global::Doroti.Framework.Painting.ImageConfiguration configuration, global::Doroti.Framework.Painting.ImageStream stream, T key, global::System.Action<object, global::System.Diagnostics.StackTrace?> handleError)
    {
        if (((((global::Doroti.Framework.Painting.ImageStream)stream).completer is not null) || global::Doroti.Framework.Painting.PaintingBinding.instance.imageCache.containsKey(key)))
        {
            this.imageProvider.resolveStreamForKey(configuration, stream, key, (global::System.Action<object, global::System.Diagnostics.StackTrace?>)handleError);
            return;
        }
        BuildContext? buildContext = ((dynamic)this.context).context;
        if (buildContext is null)
        {
            return;
        }
        if (Scrollable.recommendDeferredLoadingForContext(buildContext))
        {
            global::Doroti.Framework.Scheduler.SchedulerBinding.instance.scheduleFrameCallback(((global::System.Action<Duration>)((_) =>
            {
                DartAsyncRuntime.scheduleMicrotask((() => { resolveStreamForKey(configuration, stream, key, (global::System.Action<object, global::System.Diagnostics.StackTrace?>)handleError); }));
            })));
            return;
        }
        this.imageProvider.resolveStreamForKey(configuration, stream, key, (global::System.Action<object, global::System.Diagnostics.StackTrace?>)handleError);
    }

    public override global::Doroti.Framework.Painting.ImageStreamCompleter loadBuffer(T key, DecoderBufferCallback decode) => this.imageProvider.loadBuffer(key, (DecoderBufferCallback)decode);
    public override global::Doroti.Framework.Painting.ImageStreamCompleter loadImage(T key, ImageDecoderCallback decode) => this.imageProvider.loadImage(key, (ImageDecoderCallback)decode);
    public override Future<T> obtainKey(global::Doroti.Framework.Painting.ImageConfiguration configuration) => this.imageProvider.obtainKey(configuration);
    public override bool Equals(object? other)
    {
        var __other = other as ScrollAwareImageProvider<T>;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((__other is ScrollAwareImageProvider<T>) && (object.Equals(this.context, ((ScrollAwareImageProvider<T>)(object)__other).context))) && (object.Equals(this.imageProvider, ((ScrollAwareImageProvider<T>)(object)__other).imageProvider)));
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.context, this.imageProvider));
}
