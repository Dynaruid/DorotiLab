// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/strut_style.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622, CS8625, CS8714
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

public class StrutStyle : Diagnosticable
{
    public static StrutStyle disabled = new StrutStyle(height: 0.0, leading: 0.0);
    public virtual string? fontFamily { get; private set; }
    internal virtual List<string>? _fontFamilyFallback { get; private set; }
    internal virtual string? _package { get; private set; }
    public virtual double? fontSize { get; private set; }
    public virtual double? height { get; private set; }
    public virtual TextLeadingDistribution? leadingDistribution { get; private set; }
    public virtual FontWeight? fontWeight { get; private set; }
    public virtual FontStyle? fontStyle { get; private set; }
    public virtual double? leading { get; private set; }
    public virtual bool? forceStrutHeight { get; private set; }
    public virtual string? debugLabel { get; private set; }

    public StrutStyle(string? fontFamily = null, List<string>? fontFamilyFallback = null, double? fontSize = null, double? height = null, TextLeadingDistribution? leadingDistribution = null, double? leading = null, FontWeight? fontWeight = null, FontStyle? fontStyle = null, bool? forceStrutHeight = null, string? debugLabel = null, string? package = null)
    {
        this.fontSize = fontSize;
        this.height = height;
        this.leadingDistribution = leadingDistribution;
        this.leading = leading;
        this.fontWeight = fontWeight;
        this.fontStyle = fontStyle;
        this.forceStrutHeight = forceStrutHeight;
        this.debugLabel = debugLabel;
        this.fontFamily = ((package is null) ? fontFamily : $"packages/{package}/{fontFamily}");
        this._fontFamilyFallback = fontFamilyFallback;
        this._package = package;
        System.Diagnostics.Debug.Assert(((fontSize is null) || (DartRuntimePrimitives.RequireValue(fontSize) > 0L)));
        System.Diagnostics.Debug.Assert(((leading is null) || (leading >= 0L)));
        System.Diagnostics.Debug.Assert(((package is null) || (((fontFamily is not null) || (fontFamilyFallback is not null)))));
    }

    public static StrutStyle CreateFromTextStyle(TextStyle textStyle, string? fontFamily = null, List<string>? fontFamilyFallback = null, double? fontSize = null, double? height = null, TextLeadingDistribution? leadingDistribution = null, double? leading = null, FontWeight? fontWeight = null, FontStyle? fontStyle = null, bool? forceStrutHeight = null, string? debugLabel = null, string? package = null)
    {
        return new StrutStyle(fontFamily: ((fontFamily is not null) ? (((package is null) ? fontFamily : $"packages/{package}/{fontFamily}")) : ((TextStyle)textStyle).fontFamily), fontFamilyFallback: (fontFamilyFallback ?? ((TextStyle)textStyle).fontFamilyFallback), height: (height ?? ((TextStyle)textStyle).height), leadingDistribution: (leadingDistribution ?? ((TextStyle)textStyle).leadingDistribution), fontSize: (fontSize ?? ((TextStyle)textStyle).fontSize), leading: leading, fontWeight: (fontWeight ?? ((TextStyle)textStyle).fontWeight), fontStyle: (fontStyle ?? ((TextStyle)textStyle).fontStyle), forceStrutHeight: forceStrutHeight, debugLabel: (debugLabel ?? ((TextStyle)textStyle).debugLabel), package: package);
    }

    public virtual List<string>? fontFamilyFallback
    {
        get
        {
            if (((this._package is not null) && (this._fontFamilyFallback is not null)))
            {
                return this._fontFamilyFallback.map<string, string>(((family) => $"packages/{this._package}/{family}")).ToList();
            }
            return this._fontFamilyFallback;
            return default!;
        }
    }
    public virtual RenderComparison compareTo(StrutStyle other)
    {
        if (DartRuntimePrimitives.Identical(this, other))
        {
            return RenderComparison.identical;
        }
        if ((((((((((this.fontFamily != ((StrutStyle)other).fontFamily) || (this.fontSize != ((StrutStyle)other).fontSize)) || (!object.Equals(this.fontWeight, ((StrutStyle)other).fontWeight))) || (!object.Equals(this.fontStyle, ((StrutStyle)other).fontStyle))) || (this.height != ((StrutStyle)other).height)) || (this.leading != ((StrutStyle)other).leading)) || (this.forceStrutHeight != ((StrutStyle)other).forceStrutHeight)) || (!global::Doroti.Framework.Foundation.CollectionsLibrary.listEquals(this.fontFamilyFallback, ((StrutStyle)other).fontFamilyFallback))) || (((this.height is not null) && (!object.Equals(this.leadingDistribution, ((StrutStyle)other).leadingDistribution))))))
        {
            return RenderComparison.layout;
        }
        return RenderComparison.identical;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual StrutStyle inheritFromTextStyle(TextStyle? other)
    {
        if ((other is null))
        {
            return this;
        }
        double? effectiveHeight__22213 = (this.height ?? ((TextStyle)other).height);
        return new StrutStyle(fontFamily: (this.fontFamily ?? ((TextStyle)other).fontFamily), fontFamilyFallback: (this.fontFamilyFallback ?? ((TextStyle)other).fontFamilyFallback), fontSize: (this.fontSize ?? ((TextStyle)other).fontSize), height: effectiveHeight__22213, leading: this.leading, fontWeight: (this.fontWeight ?? ((TextStyle)other).fontWeight), fontStyle: (this.fontStyle ?? ((TextStyle)other).fontStyle), forceStrutHeight: this.forceStrutHeight, debugLabel: (this.debugLabel ?? ((TextStyle)other).debugLabel), leadingDistribution: ((effectiveHeight__22213 is not null) ? ((this.leadingDistribution ?? ((TextStyle)other).leadingDistribution)) : null));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual StrutStyle merge(StrutStyle? other)
    {
        if ((other is null))
        {
            return this;
        }
        return new StrutStyle(fontFamily: (((StrutStyle)other).fontFamily ?? this.fontFamily), fontFamilyFallback: (((StrutStyle)other).fontFamilyFallback ?? this.fontFamilyFallback), fontSize: (((StrutStyle)other).fontSize ?? this.fontSize), height: (((StrutStyle)other).height ?? this.height), leadingDistribution: (((StrutStyle)other).leadingDistribution ?? this.leadingDistribution), leading: (((StrutStyle)other).leading ?? this.leading), fontWeight: (((StrutStyle)other).fontWeight ?? this.fontWeight), fontStyle: (((StrutStyle)other).fontStyle ?? this.fontStyle), forceStrutHeight: (((StrutStyle)other).forceStrutHeight ?? this.forceStrutHeight), debugLabel: (((StrutStyle)other).debugLabel ?? this.debugLabel), package: (((StrutStyle)other)._package ?? this._package));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as StrutStyle;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((((((((((__other is StrutStyle) && (((StrutStyle)((StrutStyle)__other)).fontFamily == this.fontFamily)) && (((StrutStyle)((StrutStyle)__other)).fontSize == this.fontSize)) && (object.Equals(((StrutStyle)((StrutStyle)__other)).fontWeight, this.fontWeight))) && (object.Equals(((StrutStyle)((StrutStyle)__other)).fontStyle, this.fontStyle))) && (((StrutStyle)((StrutStyle)__other)).height == this.height)) && (((StrutStyle)((StrutStyle)__other)).leading == this.leading)) && (((StrutStyle)((StrutStyle)__other)).forceStrutHeight == this.forceStrutHeight)) && (((this.height is null) || (object.Equals(this.leadingDistribution, ((StrutStyle)((StrutStyle)__other)).leadingDistribution))))) && global::Doroti.Framework.Foundation.CollectionsLibrary.listEquals(((StrutStyle)((StrutStyle)__other)).fontFamilyFallback, this.fontFamilyFallback));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(this.fontFamily, this.fontSize, this.fontWeight, this.fontStyle, this.height, this.leading, this.forceStrutHeight);
    public virtual string toStringShort() => global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "StrutStyle");
    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties, string prefix = "")
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        if ((this.debugLabel is not null))
        {
            properties.add(new MessageProperty($"{prefix}debugLabel", this.debugLabel!));
        }
        var styles__25171 = new List<DiagnosticsNode> { new StringProperty($"{prefix}family", this.fontFamily, defaultValue: null, quoted: false), new IterableProperty<string>($"{prefix}familyFallback", this.fontFamilyFallback, defaultValue: null), new DoubleProperty($"{prefix}size", this.fontSize, defaultValue: null) };
        string? weightDescription__25474 = default!;
        if ((this.fontWeight is not null))
        {
            weightDescription__25474 = $"w{(FoundationRuntimePorts.EnumIndex(this.fontWeight!) + 1L)}00";
        }
        styles__25171.Add(new DiagnosticsProperty<global::Doroti.Ui.FontWeight>($"{prefix}weight", this.fontWeight, description: weightDescription__25474, defaultValue: null));
        styles__25171.Add(new EnumProperty<global::Doroti.Ui.FontStyle>($"{prefix}style", this.fontStyle, defaultValue: null));
        styles__25171.Add(new DoubleProperty($"{prefix}height", this.height, unit: "x", defaultValue: null));
        styles__25171.Add(new FlagProperty($"{prefix}forceStrutHeight", value: this.forceStrutHeight, ifTrue: $"{prefix}<strut height forced>", ifFalse: $"{prefix}<strut height normal>"));
        if ((this.height is not null))
        {
            double height__value26382 = DartRuntimePrimitives.RequireValue(height);
            styles__25171.Add(new EnumProperty<global::Doroti.Ui.TextLeadingDistribution>($"{prefix}leadingDistribution", this.leadingDistribution, defaultValue: null));
        }
        bool styleSpecified__26610 = styles__25171.any(((n) => !n.isFiltered(DiagnosticLevel.info)));
        styles__25171.forEach(properties.add);
        if (!styleSpecified__26610)
        {
            properties.add(new FlagProperty("forceStrutHeight", value: this.forceStrutHeight, ifTrue: $"{prefix}<strut height forced>", ifFalse: $"{prefix}<strut height normal>"));
        }
    }

}

