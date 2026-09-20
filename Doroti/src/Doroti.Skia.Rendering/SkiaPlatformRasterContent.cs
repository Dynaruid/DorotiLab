using Doroti.Ui;
using SkiaSharp;

namespace Doroti.Skia.Rendering;

/// <summary>Conservative content checks for immutable platform raster snapshots.
/// Cull hints are never used as clipping bounds. Unknown/effect content is not cached.</summary>
public static class SkiaPlatformRasterContent
{
    public sealed record Slice(IReadOnlyList<SceneCommand> Commands, SKRectI Bounds);

    // Frame number is deliberately absent: only changes to the coordinate/resource
    // domain invalidate otherwise identical pixels. Resource owners add their own
    // generation when a GPU/compositor is recreated.
    public readonly record struct CacheScope(ulong ViewId, long ViewEpoch, long SurfaceGeneration,
        int Width, int Height, double ScaleX, double ScaleY, SKColor Background, long ResourceGeneration = 0)
    {
        public static CacheScope From(PlatformCompositionToken token, int width, int height, SKColor background,
            long resourceGeneration = 0) => new(token.OwnerViewId, token.ViewEpoch, token.SurfaceGeneration,
                width, height, token.DeviceScaleX, token.DeviceScaleY, background, resourceGeneration);
    }

    public static bool CanReuse(CacheScope previousScope, Slice previous, CacheScope nextScope, Slice next,
        bool allowTranslation = true) => previousScope == nextScope &&
        (previous.Bounds == next.Bounds && Equivalent(previous.Commands, next.Commands) ||
         allowTranslation && EquivalentTranslation(previous, next));

    /// <summary>Conservative enforced-clip coverage for one independently composited layer.</summary>
    public static SKRectI Coverage(IReadOnlyList<SceneCommand> commands, int width, int height)
    {
        var slices = Split(commands, width, height);
        return slices.Count == 0 ? default : new(slices.Min(s => s.Bounds.Left), slices.Min(s => s.Bounds.Top),
            slices.Max(s => s.Bounds.Right), slices.Max(s => s.Bounds.Bottom));
    }

    /// <summary>Removes a proven opaque pixel rectangle without overlapping the remaining pieces.
    /// The caller must establish opacity and inward-rounded native coverage.</summary>
    public static IReadOnlyList<SKRectI> SubtractOpaque(SKRectI bounds, SKRectI opaque)
    {
        if (bounds.Width <= 0 || bounds.Height <= 0) return [];
        var left = Math.Max(bounds.Left, opaque.Left); var top = Math.Max(bounds.Top, opaque.Top);
        var right = Math.Min(bounds.Right, opaque.Right); var bottom = Math.Min(bounds.Bottom, opaque.Bottom);
        if (left >= right || top >= bottom) return [bounds];
        var remaining = new List<SKRectI>(4);
        if (bounds.Top < top) remaining.Add(new(bounds.Left, bounds.Top, bounds.Right, top));
        if (bottom < bounds.Bottom) remaining.Add(new(bounds.Left, bottom, bounds.Right, bounds.Bottom));
        if (bounds.Left < left) remaining.Add(new(bounds.Left, top, left, bottom));
        if (right < bounds.Right) remaining.Add(new(right, top, bounds.Right, bottom));
        return remaining;
    }

    /// <summary>Splits independent picture draws, retaining their real clip scopes.
    /// Group effects remain together. Bounds derive only from enforced scene clips.</summary>
    public static IReadOnlyList<Slice> Split(IReadOnlyList<SceneCommand> commands, int width, int height)
    {
        var viewport = Rect.fromLTWH(0, 0, width, height);
        if (commands.Any(c => c.Operation is not ("picture" or "offset" or "transform" or "clipRect" or
            "clipRRect" or "clipRSuperellipse" or "clipPath" or "pop")))
            return [new(commands, new(0, 0, width, height))];
        var slices = new List<Slice>();
        var scopes = new List<SceneCommand>();
        var stack = new Stack<(PlatformViewTransform? Transform, Rect Clip)>();
        (PlatformViewTransform? Transform, Rect Clip) state = (PlatformViewTransform.Identity, viewport);
        foreach (var command in commands)
        {
            if (command.Operation == "pop")
            {
                if (stack.Count == 0) throw new InvalidDataException("Unbalanced platform raster scope.");
                state = stack.Pop(); scopes.RemoveAt(scopes.Count - 1); continue;
            }
            if (command.Operation == "picture")
            {
                if (command.HostPayload is ScenePicturePayload picture && !PictureHasDrawing(picture)) continue;
                // Expand for fractional/antialiased clip edges, then clamp to the actual viewport.
                var clip = state.Clip;
                if (clip.isEmpty) continue;
                var bounds = new SKRectI(Math.Max(0, (int)Math.Floor(clip.left) - 2), Math.Max(0, (int)Math.Floor(clip.top) - 2),
                    Math.Min(width, (int)Math.Ceiling(clip.right) + 2), Math.Min(height, (int)Math.Ceiling(clip.bottom) + 2));
                slices.Add(new(scopes.Append(command).Concat(scopes.Select(_ => new SceneCommand("pop", null))).ToArray(), bounds));
                continue;
            }
            stack.Push(state); scopes.Add(command);
            switch (command.HostPayload)
            {
                case SceneOffsetPayload offset:
                    state.Transform = state.Transform?.ThenLocal(new(1, 0, 0, 1, offset.Dx, offset.Dy)); break;
                case SceneTransformPayload transform:
                    var m = transform.Matrix4;
                    if (m.Count != 16 || m[2] != 0 || m[3] != 0 || m[6] != 0 || m[7] != 0 ||
                        m[8] != 0 || m[9] != 0 || m[10] != 1 || m[11] != 0 || m[14] != 0 || m[15] != 1)
                        state.Transform = null;
                    else state.Transform = state.Transform?.ThenLocal(new(m[0], m[1], m[4], m[5], m[12], m[13]));
                    break;
                case SceneClipRectPayload rect when rect.Behavior != Clip.none && state.Transform is { } matrix:
                    var points = new[] { matrix.Map(rect.Rect.topLeft), matrix.Map(rect.Rect.topRight),
                        matrix.Map(rect.Rect.bottomLeft), matrix.Map(rect.Rect.bottomRight) };
                    var mapped = new Rect(points.Min(p => p.dx), points.Min(p => p.dy), points.Max(p => p.dx), points.Max(p => p.dy));
                    if (mapped.isFinite) state.Clip = state.Clip.intersect(mapped);
                    break;
            }
        }
        if (stack.Count != 0) throw new InvalidDataException("Unclosed platform raster scope.");
        // Large, overlapping independent pictures share a raster. Scroll view clips
        // are often slightly smaller than the viewport; treating each as a separate
        // bitmap would multiply nearly full-screen allocations during scrolling.
        var compact = new List<Slice>();
        foreach (var slice in slices)
        {
            var merged = false;
            if (compact.Count != 0)
            {
                var a = compact[^1].Bounds; var b = slice.Bounds;
                var union = new SKRectI(Math.Min(a.Left, b.Left), Math.Min(a.Top, b.Top),
                    Math.Max(a.Right, b.Right), Math.Max(a.Bottom, b.Bottom));
                var aa = (long)a.Width * a.Height; var ba = (long)b.Width * b.Height;
                if (a == b || aa >= (long)width * height / 4 && ba >= (long)width * height / 4 &&
                    (long)union.Width * union.Height * 4 <= (aa + ba) * 3)
                {
                    compact[^1] = new(compact[^1].Commands.Concat(slice.Commands).ToArray(), union);
                    merged = true;
                }
            }
            if (!merged) compact.Add(slice);
        }
        return compact;
    }
    public static bool HasDrawing(IReadOnlyList<SceneCommand> commands) => commands.Any(command =>
        command.Operation switch
        {
            "offset" or "transform" or "clipRect" or "clipRRect" or "clipRSuperellipse" or "clipPath" or "opacity" or "pop" => false,
            "picture" when command.HostPayload is ScenePicturePayload picture => PictureHasDrawing(picture),
            _ => true,
        });

    // Recording transform/clip scopes around composited children produces pictures
    // containing no drawing. They must not allocate full-window transparent bitmaps.
    // saveLayer and unknown operations remain conservative (their blend can affect pixels).
    private static bool PictureHasDrawing(ScenePicturePayload picture) => picture.Commands.Any(c =>
        c.Operation is not ("save" or "restore" or "translate" or "scale" or "rotate" or "skew" or
            "transform" or "clipRect" or "clipRRect" or "clipRSuperellipse" or "clipPath"));

    public static bool Equivalent(IReadOnlyList<SceneCommand> previous, IReadOnlyList<SceneCommand> next)
    {
        if (previous.Count != next.Count) return false;
        for (var i = 0; i < previous.Count; i++)
        {
            var a = previous[i]; var b = next[i];
            if (a.Operation != b.Operation) return false;
            var equal = (a.Operation, a.HostPayload, b.HostPayload) switch
            {
                ("pop", null, null) => true,
                ("offset", SceneOffsetPayload x, SceneOffsetPayload y) => x == y,
                ("clipRect", SceneClipRectPayload x, SceneClipRectPayload y) => x == y,
                ("clipRRect", SceneClipRRectPayload x, SceneClipRRectPayload y) => x == y,
                ("clipRSuperellipse", SceneClipRSuperellipsePayload x, SceneClipRSuperellipsePayload y) => x == y,
                ("transform", SceneTransformPayload x, SceneTransformPayload y) => x.Matrix4.SequenceEqual(y.Matrix4),
                ("opacity", SceneOpacityPayload x, SceneOpacityPayload y) => x == y,
                ("picture", ScenePicturePayload x, ScenePicturePayload y) =>
                    !x.WillChangeHint && !y.WillChangeHint && x.SnapshotIdentity == y.SnapshotIdentity && x.Offset == y.Offset,
                _ => false,
            };
            if (!equal) return false;
        }
        return true;
    }

    /// <summary>Proves identical pixels in two same-sized bitmap-local coordinate systems.
    /// Only immutable pictures, axis-aligned transforms and hard rectangular clips qualify.
    /// Fractional movement, changed clipping, effects and unknown commands require a redraw.</summary>
    public static bool EquivalentTranslation(Slice previous, Slice next)
    {
        if (previous.Bounds.Width != next.Bounds.Width || previous.Bounds.Height != next.Bounds.Height ||
            previous.Bounds.IsEmpty || next.Bounds.IsEmpty) return false;
        var a = LocalPictures(previous);
        var b = LocalPictures(next);
        return a is { Count: > 0 } && b is not null && a.SequenceEqual(b);
    }

    private readonly record struct LocalPicture(long Identity, PlatformViewTransform Transform, Rect Clip);

    private static List<LocalPicture>? LocalPictures(Slice slice)
    {
        var pictures = new List<LocalPicture>();
        var stack = new Stack<(PlatformViewTransform Transform, Rect Clip)>();
        var bounds = slice.Bounds;
        (PlatformViewTransform Transform, Rect Clip) state =
            (new(1, 0, 0, 1, -bounds.Left, -bounds.Top), Rect.fromLTWH(0, 0, bounds.Width, bounds.Height));
        foreach (var command in slice.Commands)
        {
            if (command.Operation == "pop")
            {
                if (!stack.TryPop(out state)) return null;
                continue;
            }
            if (command.Operation == "picture" && command.HostPayload is ScenePicturePayload picture)
            {
                if (picture.WillChangeHint) return null;
                if (PictureHasDrawing(picture) && !state.Clip.isEmpty)
                    pictures.Add(new(picture.SnapshotIdentity,
                        state.Transform.ThenLocal(new(1, 0, 0, 1, picture.Offset.dx, picture.Offset.dy)), state.Clip));
                continue;
            }
            stack.Push(state);
            switch (command.Operation, command.HostPayload)
            {
                case ("offset", SceneOffsetPayload offset):
                    state.Transform = state.Transform.ThenLocal(new(1, 0, 0, 1, offset.Dx, offset.Dy));
                    break;
                case ("transform", SceneTransformPayload transform):
                    var m = transform.Matrix4;
                    if (m.Count != 16 || m[2] != 0 || m[3] != 0 || m[6] != 0 || m[7] != 0 ||
                        m[8] != 0 || m[9] != 0 || m[10] != 1 || m[11] != 0 || m[14] != 0 || m[15] != 1)
                        return null;
                    state.Transform = state.Transform.ThenLocal(new(m[0], m[1], m[4], m[5], m[12], m[13]));
                    break;
                case ("clipRect", SceneClipRectPayload { Behavior: Clip.hardEdge } clip):
                    var a = state.Transform.Map(clip.Rect.topLeft);
                    var b = state.Transform.Map(clip.Rect.bottomRight);
                    state.Clip = state.Clip.intersect(new Rect(a.dx, a.dy, b.dx, b.dy));
                    break;
                default: return null;
            }
            if (!state.Transform.IsFinite || !state.Transform.IsAxisAligned || !state.Clip.isFinite) return null;
        }
        return stack.Count == 0 ? pictures : null;
    }
}
