// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/icon_data.dart
using Doroti.Runtime;

namespace Doroti.Framework.Widgets;

public class IconData
{
    public virtual long codePoint { get; private set; } = default!;
    public virtual string? fontFamily { get; private set; }
    public virtual string? fontPackage { get; private set; }
    public virtual bool matchTextDirection { get; private set; } = default!;
    public virtual List<string>? fontFamilyFallback { get; private set; }

    public IconData(long codePoint, string? fontFamily = null, string? fontPackage = null, bool matchTextDirection = false, List<string>? fontFamilyFallback = null)
    {
        this.codePoint = codePoint;
        this.fontFamily = fontFamily;
        this.fontPackage = fontPackage;
        this.matchTextDirection = matchTextDirection;
        this.fontFamilyFallback = fontFamilyFallback;
    }

    public override bool Equals(object? other)
    {
        var __other = other as IconData;
        if (__other is null) return false;
        if ((!Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((((((__other is IconData) && (((IconData)((IconData)__other)).codePoint == this.codePoint)) && (((IconData)((IconData)__other)).fontFamily == this.fontFamily)) && (((IconData)((IconData)__other)).fontPackage == this.fontPackage)) && (((IconData)((IconData)__other)).matchTextDirection == this.matchTextDirection)) && CollectionsLibrary.listEquals(((IconData)((IconData)__other)).fontFamilyFallback, this.fontFamilyFallback));
    }

    public override int GetHashCode()
    {
        return FoundationRuntimePorts.ObjectHash(this.codePoint, this.fontFamily, this.fontPackage, this.matchTextDirection, FoundationRuntimePorts.ObjectHashAll((this.fontFamilyFallback ?? new List<string>())));
    }
    public override string ToString() => $"IconData(U+{this.codePoint.toRadixString(16L).toUpperCase().padLeft(5L, "0")})";
}

public class IconDataProperty : global::Doroti.Framework.Foundation.DiagnosticsProperty<IconData>
{
    public IconDataProperty(string name, IconData? value, string? ifNull = null, bool showName = true, global::Doroti.Framework.Foundation.DiagnosticsTreeStyle style = DiagnosticsTreeStyle.singleLine, global::Doroti.Framework.Foundation.DiagnosticLevel level = DiagnosticLevel.info) : base(name, value, ifNull: ifNull, showName: showName, style: style, level: level)
    {
    }

    public override DartMap<string, object?> toJsonMap(global::Doroti.Framework.Foundation.DiagnosticsSerializationDelegate? @delegate = null)
    {
        DartMap<string, object?> json = ((DartMap<string, object?>)base.toJsonMap(@delegate));
        if ((this.value is not null))
        {
            json["valueProperties"] = new DartMap<string, object?> { ["codePoint"] = this.value!.codePoint };
        }
        return json;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _StaticIconProvider__icon_data
{
    internal _StaticIconProvider__icon_data()
    {
    }

}

public static partial class Icon_dataLibrary
{
    public static object staticIconProvider = new _StaticIconProvider__icon_data();
}

public static partial class Icon_dataLibrary
{
    internal static object _retainForIconTreeShaker = new object();
}

