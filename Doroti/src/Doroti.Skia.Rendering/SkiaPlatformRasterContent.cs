using Doroti.Ui;
using SkiaSharp;

namespace Doroti.Skia.Rendering;

/// <summary>Conservative content checks for immutable platform raster snapshots.
/// Cull hints are never used as clipping bounds. Unknown/effect content is not cached.</summary>
public static class SkiaPlatformRasterContent
{
    public sealed record Slice(IReadOnlyList<SceneCommand> Commands, SKRectI Bounds);

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
                if (command.HostPayload is ScenePicturePayload { Commands.Count: 0 }) continue;
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
                if (aa >= (long)width * height / 4 && ba >= (long)width * height / 4 &&
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
            "picture" when command.HostPayload is ScenePicturePayload picture => picture.Commands.Count != 0,
            _ => true,
        });

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
}
