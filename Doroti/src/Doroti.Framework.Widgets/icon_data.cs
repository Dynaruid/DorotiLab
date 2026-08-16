// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/icon_data.dart
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
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((((((__other is IconData) && (((IconData)((IconData)__other)).codePoint == this.codePoint)) && (((IconData)((IconData)__other)).fontFamily == this.fontFamily)) && (((IconData)((IconData)__other)).fontPackage == this.fontPackage)) && (((IconData)((IconData)__other)).matchTextDirection == this.matchTextDirection)) && global::Doroti.Framework.Foundation.CollectionsLibrary.listEquals(((IconData)((IconData)__other)).fontFamilyFallback, this.fontFamilyFallback));
    }

    public override int GetHashCode()
    {
        return FoundationRuntimePorts.ObjectHash(this.codePoint, this.fontFamily, this.fontPackage, this.matchTextDirection, FoundationRuntimePorts.ObjectHashAll((this.fontFamilyFallback ?? new List<string?>())));
        return default!;
    }
    public override string ToString() => $"IconData(U+{this.codePoint.toRadixString(16L).toUpperCase().padLeft(5L, "0")})";
}

public class IconDataProperty : global::Doroti.Framework.Foundation.DiagnosticsProperty<IconData>
{
    public IconDataProperty(string name, IconData? value, string? ifNull = null, bool showName = true, global::Doroti.Framework.Foundation.DiagnosticsTreeStyle style = global::Doroti.Framework.Foundation.DiagnosticsTreeStyle.singleLine, global::Doroti.Framework.Foundation.DiagnosticLevel level = global::Doroti.Framework.Foundation.DiagnosticLevel.info) : base(name, value, ifNull: ifNull, showName: showName, style: style, level: level)
    {
    }

    public virtual DartMap<string, object> toJsonMap(global::Doroti.Framework.Foundation.DiagnosticsSerializationDelegate @delegate)
    {
        DartMap<string, object?> json__4260 = ((DartMap<string, object?>)(object?)base.toJsonMap(@delegate));
        if ((this.value is not null))
        {
            json__4260["valueProperties"] = new DartMap<string, object> { ["codePoint"] = this.value!.codePoint };
        }
        return json__4260;
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

