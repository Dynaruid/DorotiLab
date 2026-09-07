using Doroti.Graphics.DisplayList;
using Doroti.Ui;
using Path = Doroti.Ui.Path;

namespace Doroti.Host.Web;

// Per producer, bounded CPU mappings of the draw-time snapshots owned by Picture.
// Public Path/Paragraph mutation cannot change these captured inputs. A repaint
// creates new snapshots; font-table changes separately invalidate text recipes.
internal sealed class BrowserRetainedMappingCache
{
    private const int Capacity = 512;
    private readonly Dictionary<Path, DisplayPath> _paths = new(ReferenceEqualityComparer.Instance);
    private readonly Dictionary<Paragraph, ParagraphEntry> _paragraphs = new(ReferenceEqualityComparer.Instance);
    private readonly Dictionary<(double X, double Y), DisplayMatrix> _translations = [];
    private DisplayResourceReference[] _fonts = [];

    internal void BeginFrame(IReadOnlyList<DisplayResourceReference> fonts)
    {
        if (_fonts.AsSpan().SequenceEqual(fonts.ToArray())) return;
        _fonts = fonts.ToArray();
        _paragraphs.Clear();
    }

    internal DisplayPath MapPath(Path path, Func<Path, DisplayPath> map)
    {
        if (_paths.TryGetValue(path, out var old)) return old;
        var mapped = map(path);
        // Bound both source and mapped geometry, not just handle count.
        if (path.Commands.Count > 128 || mapped.Values.Count > 256)
        { _paths.Remove(path); return mapped; }
        if (!_paths.ContainsKey(path) && _paths.Count >= Capacity) _paths.Remove(_paths.First().Key);
        _paths[path] = mapped;
        return mapped;
    }

    internal DisplayMatrix MapTranslation(double x, double y, Func<double, double, DisplayMatrix> map)
    {
        if (x == 0 && y == 0) return DisplayMatrix.Identity;
        if (_translations.TryGetValue((x, y), out var matrix)) return matrix;
        matrix = map(x, y);
        if (_translations.Count >= Capacity) _translations.Remove(_translations.First().Key);
        _translations[(x, y)] = matrix;
        return matrix;
    }

    internal DisplayParagraphRecipe MapParagraph(Paragraph paragraph, DisplayResourceReference font,
        Func<DisplayParagraphRecipe> map)
    {
        var key = new ParagraphGeometry(paragraph.width, paragraph.height, paragraph.longestLine,
            paragraph.CanvasKitMetricsHash, font);
        if (_paragraphs.TryGetValue(paragraph, out var old) && old.Geometry == key) return old.Recipe;
        var recipe = map();
        // Bound variable text/style graphs as well as the number of handles.
        if (recipe.Text.Length > 4096 || recipe.FontFamily.Length > 256 || recipe.Locale.Length > 256 ||
            recipe.Ellipsis?.Length > 4096 || recipe.TextRuns.Count > 16 || recipe.FallbackFonts.Count > 32 ||
            recipe.TextRuns.Any(r => r.Text.Length > 4096 || r.FontFamily.Length > 256 || r.Locale.Length > 256 ||
                r.FontFamilyFallback.Count > 8 || r.FontFamilyFallback.Any(f => f.Length > 256) ||
                r.Shadows.Count > 8 || r.FontFeatures.Count > 8 || r.FontVariations.Count > 8 ||
                r.FontFeatures.Any(f => f.Name.Length > 256) || r.FontVariations.Any(v => v.Axis.Length > 256)))
        { _paragraphs.Remove(paragraph); return recipe; }
        if (!_paragraphs.ContainsKey(paragraph) && _paragraphs.Count >= Capacity) _paragraphs.Remove(_paragraphs.First().Key);
        _paragraphs[paragraph] = new(key, recipe);
        return recipe;
    }

    internal void Clear() { _paths.Clear(); _paragraphs.Clear(); _translations.Clear(); _fonts = []; }

    private readonly record struct ParagraphGeometry(double Width, double Height, double LongestLine,
        ulong MetricsHash, DisplayResourceReference Font);
    private sealed record ParagraphEntry(ParagraphGeometry Geometry, DisplayParagraphRecipe Recipe);
}
