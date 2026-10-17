using System.Collections.Concurrent;
using Doroti.Framework.Widgets;

namespace Doroti.Cupertino;

internal static class CupertinoIconFont
{
    private static readonly IconFont Font = new(typeof(CupertinoIconFont).Assembly,
        "Doroti.Icons.Cupertino.font", "packages/cupertino_icons/CupertinoIcons");
    private static readonly ConcurrentDictionary<(long Code, string? Family, string? Package, bool Direction), IconData> Icons = new();

    internal static IconData Create(long codePoint, string? fontFamily = null,
        string? fontPackage = null, bool matchTextDirection = false) =>
        Icons.GetOrAdd((codePoint, fontFamily, fontPackage, matchTextDirection), static key =>
            new IconData(key.Code, key.Family, key.Package, key.Direction, null, Font));
}
