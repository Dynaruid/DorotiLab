using Doroti.Runtime;

namespace Doroti.Ui;

/// <summary>A closed command snapshot whose image leases survive producer disposal.
/// Retaining a snapshot does not acknowledge presentation or GPU completion.</summary>
public sealed class DorotiFrozenScene : IDisposable
{
    private sealed class Storage(ulong viewId, IReadOnlyList<SceneCommand> commands, IDisposable[] resources, DorotiSceneOwner? owner)
    {
        internal readonly ulong ViewId = viewId;
        internal readonly DorotiSceneOwner? Owner = owner;
        internal readonly IReadOnlyList<SceneCommand> Commands = commands;
        internal readonly IDisposable[] Resources = resources;
        internal int References = 1;
        internal object? Consumer;
    }

    private readonly Storage _storage;
    private readonly object _gate = new();
    private bool _disposed;

    private DorotiFrozenScene(Storage storage) => _storage = storage;
    public ulong ViewId => _storage.ViewId;
    public DorotiSceneOwner? Owner => _storage.Owner;
    public int CommandCount => _storage.Commands.Count;
    internal IReadOnlyList<SceneCommand> Commands
    {
        get { lock (_gate) { ObjectDisposedException.ThrowIf(_disposed, this); return _storage.Commands; } }
    }

    internal static DorotiFrozenScene Capture(Scene scene)
    {
        ArgumentNullException.ThrowIfNull(scene);
        scene.ValidateFreezeOwner();
        var copier = new Copier(scene.viewId);
        try
        {
            return new(new(scene.viewId, copier.Commands(scene.Commands), copier.Resources.ToArray(), scene.Owner));
        }
        catch (Exception failure)
        {
            try { DorotiCleanup.Run(copier.Resources.AsEnumerable().Reverse().Select<IDisposable, Action>(resource => resource.Dispose).ToArray()); }
            catch (Exception cleanup) { throw new AggregateException(failure, cleanup); }
            throw;
        }
    }

    internal DorotiFrozenScene CaptureTextures(Func<long, bool, IDisposable> capture)
    {
        var resources = new List<IDisposable> { Retain() };
        var textures = new Dictionary<(long Id, bool Freeze), IDisposable>();
        IReadOnlyList<SceneCommand> Visit(IReadOnlyList<SceneCommand> commands) => Array.AsReadOnly(commands.Select(command =>
        {
            object? payload = command.HostPayload;
            if (payload is SceneTexturePayload texture)
            {
                var key = (texture.TextureId, texture.Freeze);
                if (!textures.TryGetValue(key, out var frame))
                {
                    frame = capture(key.TextureId, key.Freeze);
                    textures.Add(key, frame);
                    resources.Add(frame);
                }
                payload = texture with { FrozenFrame = frame };
            }
            else if (payload is SceneRetainedPayload retained)
                payload = retained with { Commands = Visit(retained.Commands) };
            return command with { HostPayload = payload };
        }).ToArray());
        try { return new(new(ViewId, Visit(Commands), resources.ToArray(), Owner)); }
        catch (Exception failure)
        {
            try { DorotiCleanup.Run(resources.AsEnumerable().Reverse().Select<IDisposable, Action>(resource => resource.Dispose).ToArray()); }
            catch (Exception cleanup) { throw new AggregateException(failure, cleanup); }
            throw;
        }
    }

    public DorotiFrozenScene Retain()
    {
        lock (_gate)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            Interlocked.Increment(ref _storage.References);
            return new(_storage);
        }
    }

    /// <summary>A frozen submission has one rendering consumer. GPU consumers
    /// retain additional handles through their actual completion fence.</summary>
    public DorotiFrozenScene RetainForConsumer(object consumer)
    {
        ArgumentNullException.ThrowIfNull(consumer);
        lock (_gate)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            lock (_storage)
            {
                if (_storage.Consumer is not null && !ReferenceEquals(_storage.Consumer, consumer))
                    throw new InvalidOperationException("A frozen frame cannot be imported by multiple rendering consumers.");
                _storage.Consumer = consumer;
                return Retain();
            }
        }
    }

    public void Dispose()
    {
        lock (_gate)
        {
            if (_disposed) return;
            _disposed = true;
        }
        if (Interlocked.Decrement(ref _storage.References) == 0)
            DorotiCleanup.Run(_storage.Resources.Reverse().Select<IDisposable, Action>(resource => resource.Dispose).ToArray());
    }

    private sealed class Copier(ulong viewId)
    {
        private sealed record StableIdentity(long Id);
        private static long _nextIdentity;
        private static readonly System.Runtime.CompilerServices.ConditionalWeakTable<object, StableIdentity> Identities = new();
        private static object? Identity(object? value) => value is null ? null :
            Identities.GetValue(value, _ => new(Interlocked.Increment(ref _nextIdentity)));
        private readonly Dictionary<Image, Image> _images = new(ReferenceEqualityComparer.Instance);
        internal List<IDisposable> Resources { get; } = [];
        private static IReadOnlyList<T> Copy<T>(IEnumerable<T> values) => Array.AsReadOnly(values.ToArray());

        internal IReadOnlyList<SceneCommand> Commands(IReadOnlyList<SceneCommand> commands) =>
            Copy(commands.Select(command => new SceneCommand(command.Operation, null)
            {
                // Diagnostic/anonymous payloads can contain EngineLayer or arbitrary
                // app state. The consumer receives only the closed host contract.
                HostPayload = Payload(command.HostPayload),
            }));

        private IReadOnlyList<PathCommand> PictureCommands(IReadOnlyList<PathCommand> commands) =>
            Copy(commands.Select(command => new PathCommand(command.Operation, Copy(command.Arguments))
            { HostPayload = Payload(command.HostPayload) }));

        private Image Image(Image source)
        {
            if (source.viewId != viewId)
                throw new InvalidOperationException("A frame image must belong to the submitting view.");
            if (_images.TryGetValue(source, out var captured)) return captured;
            captured = source.clone();
            _images.Add(source, captured);
            Resources.Add(captured);
            return captured;
        }

        private PaintSnapshot Paint(PaintSnapshot paint) => paint with
        { Shader = Shader(paint.Shader), ColorFilter = ColorFilter(paint.ColorFilter) };

        private ColorFilterSnapshot? ColorFilter(ColorFilterSnapshot? filter) => filter is null ? null :
            filter with { Matrix = filter.Matrix is null ? null : Copy(filter.Matrix) };

        private ImageFilterSnapshot Filter(ImageFilterSnapshot filter) => filter with
        {
            Outer = filter.Outer is null ? null : Filter(filter.Outer),
            Inner = filter.Inner is null ? null : Filter(filter.Inner),
            ColorFilter = ColorFilter(filter.ColorFilter),
            Matrix4 = filter.Matrix4 is null ? null : Copy(filter.Matrix4),
            Shader = Shader(filter.Shader),
        };

        private ShaderSnapshot? Shader(ShaderSnapshot? shader) => shader switch
        {
            null => null,
            GradientShaderSnapshot gradient => gradient with
            { Colors = Copy(gradient.Colors), Stops = Copy(gradient.Stops), Matrix4 = gradient.Matrix4 is null ? null : Copy(gradient.Matrix4) },
            ImageShaderSnapshot image => image with { Image = Image(image.Image), Matrix4 = Copy(image.Matrix4) },
            FragmentShaderSnapshot fragment => new FragmentShaderSnapshot(fragment.State with
            {
                Floats = Copy(fragment.State.Floats),
                Samplers = new System.Collections.ObjectModel.ReadOnlyDictionary<long, Image>(
                    fragment.State.Samplers.ToDictionary(pair => pair.Key, pair => Image(pair.Value))),
            }),
            UnsupportedShaderSnapshot unsupported => unsupported,
            _ => throw new NotSupportedException($"Unregistered frozen shader type: {shader.GetType().FullName}."),
        };

        private object? Payload(object? payload) => payload switch
        {
            null => null,
            ScenePicturePayload picture => picture with { Commands = PictureCommands(picture.Commands) },
            SceneRetainedPayload retained when retained.ViewId == viewId => retained with { Commands = Commands(retained.Commands) },
            SceneClipPathPayload clip => clip with { Path = clip.Path.SnapshotForPainting() },
            SceneTransformPayload transform => transform with { Matrix4 = Copy(transform.Matrix4) },
            SceneColorFilterPayload filter => filter with { Filter = ColorFilter(filter.Filter)! },
            SceneImageFilterPayload filter => filter with { Filter = Filter(filter.Filter), CacheKey = Identity(filter.CacheKey) },
            SceneShaderMaskPayload mask => mask with { Shader = Shader(mask.Shader)! },
            SceneBackdropFilterPayload backdrop => backdrop with { Filter = Filter(backdrop.Filter), BackdropId = Identity(backdrop.BackdropId) },
            CanvasSaveLayerPayload layer => layer with { Paint = Paint(layer.Paint) },
            CanvasPathPayload path => path with { Path = path.Path.SnapshotForPainting(), Paint = Paint(path.Paint) },
            CanvasRectPayload rect => rect with { Paint = Paint(rect.Paint) },
            CanvasRRectPayload rect => rect with { Paint = Paint(rect.Paint) },
            CanvasRSuperellipsePayload rect => rect with { Paint = Paint(rect.Paint) },
            CanvasDRRectPayload rect => rect with { Paint = Paint(rect.Paint) },
            CanvasClipPathPayload clip => clip with { Path = clip.Path.SnapshotForPainting() },
            CanvasImagePayload image => image with { Image = Image(image.Image), Paint = Paint(image.Paint) },
            CanvasImageNinePayload image => image with { Image = Image(image.Image), Paint = Paint(image.Paint) },
            CanvasParagraphPayload paragraph => paragraph with { Paragraph = paragraph.Paragraph.SnapshotForPainting() },
            CanvasShadowPayload shadow => shadow with { Path = shadow.Path.SnapshotForPainting() },
            CanvasCirclePayload circle => circle with { Paint = Paint(circle.Paint) },
            CanvasLinePayload line => line with { Paint = Paint(line.Paint) },
            CanvasPointsPayload points => points with { Points = Copy(points.Points), Paint = Paint(points.Paint) },
            CanvasOvalPayload oval => oval with { Paint = Paint(oval.Paint) },
            CanvasArcPayload arc => arc with { Paint = Paint(arc.Paint) },
            PaintSnapshot paint => Paint(paint),
            SceneOffsetPayload or SceneClipRectPayload or SceneClipRRectPayload or SceneClipRSuperellipsePayload or
                SceneOpacityPayload or SceneTexturePayload or ScenePlatformViewPayload or SceneInputShieldPayload or
                SceneGpuEffectPayload or CanvasClipRRectPayload or CanvasClipRSuperellipsePayload or CanvasColorPayload => payload,
            _ => throw new NotSupportedException($"Unregistered frozen command payload: {payload.GetType().FullName}."),
        };
    }
}
