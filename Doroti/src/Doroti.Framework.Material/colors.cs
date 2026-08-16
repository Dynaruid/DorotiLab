// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/colors.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Material;

public class MaterialColor : global::Doroti.Framework.Painting.ColorSwatch<long>
{
    public MaterialColor(long primary, DartMap<long, Color> swatch) : base(primary, swatch)
    {
    }

    public virtual global::Doroti.Ui.Color shade50 => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this[50L]!);
    public virtual global::Doroti.Ui.Color shade100 => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this[100L]!);
    public virtual global::Doroti.Ui.Color shade200 => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this[200L]!);
    public virtual global::Doroti.Ui.Color shade300 => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this[300L]!);
    public virtual global::Doroti.Ui.Color shade400 => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this[400L]!);
    public virtual global::Doroti.Ui.Color shade500 => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this[500L]!);
    public virtual global::Doroti.Ui.Color shade600 => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this[600L]!);
    public virtual global::Doroti.Ui.Color shade700 => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this[700L]!);
    public virtual global::Doroti.Ui.Color shade800 => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this[800L]!);
    public virtual global::Doroti.Ui.Color shade900 => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this[900L]!);
}

public class MaterialAccentColor : global::Doroti.Framework.Painting.ColorSwatch<long>
{
    public MaterialAccentColor(long primary, DartMap<long, Color> swatch) : base(primary, swatch)
    {
    }

    public virtual global::Doroti.Ui.Color shade100 => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this[100L]!);
    public virtual global::Doroti.Ui.Color shade200 => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this[200L]!);
    public virtual global::Doroti.Ui.Color shade400 => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this[400L]!);
    public virtual global::Doroti.Ui.Color shade700 => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this[700L]!);
}

public abstract class Colors
{
    public static Color transparent = new global::Doroti.Ui.Color(0L);
    public static Color black = new global::Doroti.Ui.Color(4278190080L);
    public static Color black87 = new global::Doroti.Ui.Color(3707764736L);
    public static Color black54 = new global::Doroti.Ui.Color(2315255808L);
    public static Color black45 = new global::Doroti.Ui.Color(1929379840L);
    public static Color black38 = new global::Doroti.Ui.Color(1627389952L);
    public static Color black26 = new global::Doroti.Ui.Color(1107296256L);
    public static Color black12 = new global::Doroti.Ui.Color(520093696L);
    public static Color white = new global::Doroti.Ui.Color(4294967295L);
    public static Color white70 = new global::Doroti.Ui.Color(3019898879L);
    public static Color white60 = new global::Doroti.Ui.Color(2583691263L);
    public static Color white54 = new global::Doroti.Ui.Color(2332033023L);
    public static Color white38 = new global::Doroti.Ui.Color(1660944383L);
    public static Color white30 = new global::Doroti.Ui.Color(1308622847L);
    public static Color white24 = new global::Doroti.Ui.Color(1040187391L);
    public static Color white12 = new global::Doroti.Ui.Color(536870911L);
    public static Color white10 = new global::Doroti.Ui.Color(452984831L);
    public static MaterialColor red = new MaterialColor(_redPrimaryValue, new DartMap<long, Color> { [50L] = new global::Doroti.Ui.Color(4294962158L), [100L] = new global::Doroti.Ui.Color(4294954450L), [200L] = new global::Doroti.Ui.Color(4293892762L), [300L] = new global::Doroti.Ui.Color(4293227379L), [400L] = new global::Doroti.Ui.Color(4293874512L), [500L] = new global::Doroti.Ui.Color(_redPrimaryValue), [600L] = new global::Doroti.Ui.Color(4293212469L), [700L] = new global::Doroti.Ui.Color(4292030255L), [800L] = new global::Doroti.Ui.Color(4291176488L), [900L] = new global::Doroti.Ui.Color(4290190364L) });
    internal const long _redPrimaryValue = 4294198070L;
    public static MaterialAccentColor redAccent = new MaterialAccentColor(_redAccentValue, new DartMap<long, Color> { [100L] = new global::Doroti.Ui.Color(4294937216L), [200L] = new global::Doroti.Ui.Color(_redAccentValue), [400L] = new global::Doroti.Ui.Color(4294907716L), [700L] = new global::Doroti.Ui.Color(4292149248L) });
    internal const long _redAccentValue = 4294922834L;
    public static MaterialColor pink = new MaterialColor(_pinkPrimaryValue, new DartMap<long, Color> { [50L] = new global::Doroti.Ui.Color(4294763756L), [100L] = new global::Doroti.Ui.Color(4294491088L), [200L] = new global::Doroti.Ui.Color(4294217649L), [300L] = new global::Doroti.Ui.Color(4293943954L), [400L] = new global::Doroti.Ui.Color(4293673082L), [500L] = new global::Doroti.Ui.Color(_pinkPrimaryValue), [600L] = new global::Doroti.Ui.Color(4292352864L), [700L] = new global::Doroti.Ui.Color(4290910299L), [800L] = new global::Doroti.Ui.Color(4289533015L), [900L] = new global::Doroti.Ui.Color(4287106639L) });
    internal const long _pinkPrimaryValue = 4293467747L;
    public static MaterialAccentColor pinkAccent = new MaterialAccentColor(_pinkAccentPrimaryValue, new DartMap<long, Color> { [100L] = new global::Doroti.Ui.Color(4294934699L), [200L] = new global::Doroti.Ui.Color(_pinkAccentPrimaryValue), [400L] = new global::Doroti.Ui.Color(4294246487L), [700L] = new global::Doroti.Ui.Color(4291105122L) });
    internal const long _pinkAccentPrimaryValue = 4294918273L;
    public static MaterialColor purple = new MaterialColor(_purplePrimaryValue, new DartMap<long, Color> { [50L] = new global::Doroti.Ui.Color(4294174197L), [100L] = new global::Doroti.Ui.Color(4292984551L), [200L] = new global::Doroti.Ui.Color(4291728344L), [300L] = new global::Doroti.Ui.Color(4290406600L), [400L] = new global::Doroti.Ui.Color(4289415100L), [500L] = new global::Doroti.Ui.Color(_purplePrimaryValue), [600L] = new global::Doroti.Ui.Color(4287505578L), [700L] = new global::Doroti.Ui.Color(4286259106L), [800L] = new global::Doroti.Ui.Color(4285143962L), [900L] = new global::Doroti.Ui.Color(4283045004L) });
    internal const long _purplePrimaryValue = 4288423856L;
    public static MaterialAccentColor purpleAccent = new MaterialAccentColor(_purpleAccentPrimaryValue, new DartMap<long, Color> { [100L] = new global::Doroti.Ui.Color(4293558524L), [200L] = new global::Doroti.Ui.Color(_purpleAccentPrimaryValue), [400L] = new global::Doroti.Ui.Color(4292149497L), [700L] = new global::Doroti.Ui.Color(4289331455L) });
    internal const long _purpleAccentPrimaryValue = 4292886779L;
    public static MaterialColor deepPurple = new MaterialColor(_deepPurplePrimaryValue, new DartMap<long, Color> { [50L] = new global::Doroti.Ui.Color(4293781494L), [100L] = new global::Doroti.Ui.Color(4291937513L), [200L] = new global::Doroti.Ui.Color(4289961435L), [300L] = new global::Doroti.Ui.Color(4287985101L), [400L] = new global::Doroti.Ui.Color(4286470082L), [500L] = new global::Doroti.Ui.Color(_deepPurplePrimaryValue), [600L] = new global::Doroti.Ui.Color(4284364209L), [700L] = new global::Doroti.Ui.Color(4283510184L), [800L] = new global::Doroti.Ui.Color(4282722208L), [900L] = new global::Doroti.Ui.Color(4281408402L) });
    internal const long _deepPurplePrimaryValue = 4284955319L;
    public static MaterialAccentColor deepPurpleAccent = new MaterialAccentColor(_deepPurpleAccentPrimaryValue, new DartMap<long, Color> { [100L] = new global::Doroti.Ui.Color(4289956095L), [200L] = new global::Doroti.Ui.Color(_deepPurpleAccentPrimaryValue), [400L] = new global::Doroti.Ui.Color(4284817407L), [700L] = new global::Doroti.Ui.Color(4284612842L) });
    internal const long _deepPurpleAccentPrimaryValue = 4286336511L;
    public static MaterialColor indigo = new MaterialColor(_indigoPrimaryValue, new DartMap<long, Color> { [50L] = new global::Doroti.Ui.Color(4293454582L), [100L] = new global::Doroti.Ui.Color(4291152617L), [200L] = new global::Doroti.Ui.Color(4288653530L), [300L] = new global::Doroti.Ui.Color(4286154443L), [400L] = new global::Doroti.Ui.Color(4284246976L), [500L] = new global::Doroti.Ui.Color(_indigoPrimaryValue), [600L] = new global::Doroti.Ui.Color(4281944491L), [700L] = new global::Doroti.Ui.Color(4281352095L), [800L] = new global::Doroti.Ui.Color(4280825235L), [900L] = new global::Doroti.Ui.Color(4279903102L) });
    internal const long _indigoPrimaryValue = 4282339765L;
    public static MaterialAccentColor indigoAccent = new MaterialAccentColor(_indigoAccentPrimaryValue, new DartMap<long, Color> { [100L] = new global::Doroti.Ui.Color(4287405823L), [200L] = new global::Doroti.Ui.Color(_indigoAccentPrimaryValue), [400L] = new global::Doroti.Ui.Color(4282211070L), [700L] = new global::Doroti.Ui.Color(4281356286L) });
    internal const long _indigoAccentPrimaryValue = 4283657726L;
    public static MaterialColor blue = new MaterialColor(_bluePrimaryValue, new DartMap<long, Color> { [50L] = new global::Doroti.Ui.Color(4293128957L), [100L] = new global::Doroti.Ui.Color(4290502395L), [200L] = new global::Doroti.Ui.Color(4287679225L), [300L] = new global::Doroti.Ui.Color(4284790262L), [400L] = new global::Doroti.Ui.Color(4282557941L), [500L] = new global::Doroti.Ui.Color(_bluePrimaryValue), [600L] = new global::Doroti.Ui.Color(4280191205L), [700L] = new global::Doroti.Ui.Color(4279858898L), [800L] = new global::Doroti.Ui.Color(4279592384L), [900L] = new global::Doroti.Ui.Color(4279060385L) });
    internal const long _bluePrimaryValue = 4280391411L;
    public static MaterialAccentColor blueAccent = new MaterialAccentColor(_blueAccentPrimaryValue, new DartMap<long, Color> { [100L] = new global::Doroti.Ui.Color(4286755327L), [200L] = new global::Doroti.Ui.Color(_blueAccentPrimaryValue), [400L] = new global::Doroti.Ui.Color(4280908287L), [700L] = new global::Doroti.Ui.Color(4280902399L) });
    internal const long _blueAccentPrimaryValue = 4282682111L;
    public static MaterialColor lightBlue = new MaterialColor(_lightBluePrimaryValue, new DartMap<long, Color> { [50L] = new global::Doroti.Ui.Color(4292998654L), [100L] = new global::Doroti.Ui.Color(4289979900L), [200L] = new global::Doroti.Ui.Color(4286698746L), [300L] = new global::Doroti.Ui.Color(4283417591L), [400L] = new global::Doroti.Ui.Color(4280923894L), [500L] = new global::Doroti.Ui.Color(_lightBluePrimaryValue), [600L] = new global::Doroti.Ui.Color(4278426597L), [700L] = new global::Doroti.Ui.Color(4278356177L), [800L] = new global::Doroti.Ui.Color(4278351805L), [900L] = new global::Doroti.Ui.Color(4278278043L) });
    internal const long _lightBluePrimaryValue = 4278430196L;
    public static MaterialAccentColor lightBlueAccent = new MaterialAccentColor(_lightBlueAccentPrimaryValue, new DartMap<long, Color> { [100L] = new global::Doroti.Ui.Color(4286634239L), [200L] = new global::Doroti.Ui.Color(_lightBlueAccentPrimaryValue), [400L] = new global::Doroti.Ui.Color(4278235391L), [700L] = new global::Doroti.Ui.Color(4278227434L) });
    internal const long _lightBlueAccentPrimaryValue = 4282434815L;
    public static MaterialColor cyan = new MaterialColor(_cyanPrimaryValue, new DartMap<long, Color> { [50L] = new global::Doroti.Ui.Color(4292933626L), [100L] = new global::Doroti.Ui.Color(4289915890L), [200L] = new global::Doroti.Ui.Color(4286635754L), [300L] = new global::Doroti.Ui.Color(4283289825L), [400L] = new global::Doroti.Ui.Color(4280731354L), [500L] = new global::Doroti.Ui.Color(_cyanPrimaryValue), [600L] = new global::Doroti.Ui.Color(4278234305L), [700L] = new global::Doroti.Ui.Color(4278228903L), [800L] = new global::Doroti.Ui.Color(4278223759L), [900L] = new global::Doroti.Ui.Color(4278214756L) });
    internal const long _cyanPrimaryValue = 4278238420L;
    public static MaterialAccentColor cyanAccent = new MaterialAccentColor(_cyanAccentPrimaryValue, new DartMap<long, Color> { [100L] = new global::Doroti.Ui.Color(4286906367L), [200L] = new global::Doroti.Ui.Color(_cyanAccentPrimaryValue), [400L] = new global::Doroti.Ui.Color(4278248959L), [700L] = new global::Doroti.Ui.Color(4278237396L) });
    internal const long _cyanAccentPrimaryValue = 4279828479L;
    public static MaterialColor teal = new MaterialColor(_tealPrimaryValue, new DartMap<long, Color> { [50L] = new global::Doroti.Ui.Color(4292932337L), [100L] = new global::Doroti.Ui.Color(4289912795L), [200L] = new global::Doroti.Ui.Color(4286630852L), [300L] = new global::Doroti.Ui.Color(4283283116L), [400L] = new global::Doroti.Ui.Color(4280723098L), [500L] = new global::Doroti.Ui.Color(_tealPrimaryValue), [600L] = new global::Doroti.Ui.Color(4278225275L), [700L] = new global::Doroti.Ui.Color(4278221163L), [800L] = new global::Doroti.Ui.Color(4278217052L), [900L] = new global::Doroti.Ui.Color(4278209856L) });
    internal const long _tealPrimaryValue = 4278228616L;
    public static MaterialAccentColor tealAccent = new MaterialAccentColor(_tealAccentPrimaryValue, new DartMap<long, Color> { [100L] = new global::Doroti.Ui.Color(4289200107L), [200L] = new global::Doroti.Ui.Color(_tealAccentPrimaryValue), [400L] = new global::Doroti.Ui.Color(4280150454L), [700L] = new global::Doroti.Ui.Color(4278239141L) });
    internal const long _tealAccentPrimaryValue = 4284809178L;
    public static MaterialColor green = new MaterialColor(_greenPrimaryValue, new DartMap<long, Color> { [50L] = new global::Doroti.Ui.Color(4293457385L), [100L] = new global::Doroti.Ui.Color(4291356361L), [200L] = new global::Doroti.Ui.Color(4289058471L), [300L] = new global::Doroti.Ui.Color(4286695300L), [400L] = new global::Doroti.Ui.Color(4284922730L), [500L] = new global::Doroti.Ui.Color(_greenPrimaryValue), [600L] = new global::Doroti.Ui.Color(4282622023L), [700L] = new global::Doroti.Ui.Color(4281896508L), [800L] = new global::Doroti.Ui.Color(4281236786L), [900L] = new global::Doroti.Ui.Color(4279983648L) });
    internal const long _greenPrimaryValue = 4283215696L;
    public static MaterialAccentColor greenAccent = new MaterialAccentColor(_greenAccentPrimaryValue, new DartMap<long, Color> { [100L] = new global::Doroti.Ui.Color(4290377418L), [200L] = new global::Doroti.Ui.Color(_greenAccentPrimaryValue), [400L] = new global::Doroti.Ui.Color(4278249078L), [700L] = new global::Doroti.Ui.Color(4278241363L) });
    internal const long _greenAccentPrimaryValue = 4285132974L;
    public static MaterialColor lightGreen = new MaterialColor(_lightGreenPrimaryValue, new DartMap<long, Color> { [50L] = new global::Doroti.Ui.Color(4294047977L), [100L] = new global::Doroti.Ui.Color(4292668872L), [200L] = new global::Doroti.Ui.Color(4291158437L), [300L] = new global::Doroti.Ui.Color(4289648001L), [400L] = new global::Doroti.Ui.Color(4288466021L), [500L] = new global::Doroti.Ui.Color(_lightGreenPrimaryValue), [600L] = new global::Doroti.Ui.Color(4286362434L), [700L] = new global::Doroti.Ui.Color(4285046584L), [800L] = new global::Doroti.Ui.Color(4283796271L), [900L] = new global::Doroti.Ui.Color(4281559326L) });
    internal const long _lightGreenPrimaryValue = 4287349578L;
    public static MaterialAccentColor lightGreenAccent = new MaterialAccentColor(_lightGreenAccentPrimaryValue, new DartMap<long, Color> { [100L] = new global::Doroti.Ui.Color(4291624848L), [200L] = new global::Doroti.Ui.Color(_lightGreenAccentPrimaryValue), [400L] = new global::Doroti.Ui.Color(4285988611L), [700L] = new global::Doroti.Ui.Color(4284800279L) });
    internal const long _lightGreenAccentPrimaryValue = 4289920857L;
    public static MaterialColor lime = new MaterialColor(_limePrimaryValue, new DartMap<long, Color> { [50L] = new global::Doroti.Ui.Color(4294573031L), [100L] = new global::Doroti.Ui.Color(4293981379L), [200L] = new global::Doroti.Ui.Color(4293324444L), [300L] = new global::Doroti.Ui.Color(4292667253L), [400L] = new global::Doroti.Ui.Color(4292141399L), [500L] = new global::Doroti.Ui.Color(_limePrimaryValue), [600L] = new global::Doroti.Ui.Color(4290824755L), [700L] = new global::Doroti.Ui.Color(4289705003L), [800L] = new global::Doroti.Ui.Color(4288584996L), [900L] = new global::Doroti.Ui.Color(4286740247L) });
    internal const long _limePrimaryValue = 4291681337L;
    public static MaterialAccentColor limeAccent = new MaterialAccentColor(_limeAccentPrimaryValue, new DartMap<long, Color> { [100L] = new global::Doroti.Ui.Color(4294246273L), [200L] = new global::Doroti.Ui.Color(_limeAccentPrimaryValue), [400L] = new global::Doroti.Ui.Color(4291231488L), [700L] = new global::Doroti.Ui.Color(4289653248L) });
    internal const long _limeAccentPrimaryValue = 4293852993L;
    public static MaterialColor yellow = new MaterialColor(_yellowPrimaryValue, new DartMap<long, Color> { [50L] = new global::Doroti.Ui.Color(4294966759L), [100L] = new global::Doroti.Ui.Color(4294965700L), [200L] = new global::Doroti.Ui.Color(4294964637L), [300L] = new global::Doroti.Ui.Color(4294963574L), [400L] = new global::Doroti.Ui.Color(4294962776L), [500L] = new global::Doroti.Ui.Color(_yellowPrimaryValue), [600L] = new global::Doroti.Ui.Color(4294826037L), [700L] = new global::Doroti.Ui.Color(4294688813L), [800L] = new global::Doroti.Ui.Color(4294551589L), [900L] = new global::Doroti.Ui.Color(4294278935L) });
    internal const long _yellowPrimaryValue = 4294961979L;
    public static MaterialAccentColor yellowAccent = new MaterialAccentColor(_yellowAccentPrimaryValue, new DartMap<long, Color> { [100L] = new global::Doroti.Ui.Color(4294967181L), [200L] = new global::Doroti.Ui.Color(_yellowAccentPrimaryValue), [400L] = new global::Doroti.Ui.Color(4294961664L), [700L] = new global::Doroti.Ui.Color(4294956544L) });
    internal const long _yellowAccentPrimaryValue = 4294967040L;
    public static MaterialColor amber = new MaterialColor(_amberPrimaryValue, new DartMap<long, Color> { [50L] = new global::Doroti.Ui.Color(4294965473L), [100L] = new global::Doroti.Ui.Color(4294962355L), [200L] = new global::Doroti.Ui.Color(4294959234L), [300L] = new global::Doroti.Ui.Color(4294956367L), [400L] = new global::Doroti.Ui.Color(4294953512L), [500L] = new global::Doroti.Ui.Color(_amberPrimaryValue), [600L] = new global::Doroti.Ui.Color(4294947584L), [700L] = new global::Doroti.Ui.Color(4294942720L), [800L] = new global::Doroti.Ui.Color(4294938368L), [900L] = new global::Doroti.Ui.Color(4294930176L) });
    internal const long _amberPrimaryValue = 4294951175L;
    public static MaterialAccentColor amberAccent = new MaterialAccentColor(_amberAccentPrimaryValue, new DartMap<long, Color> { [100L] = new global::Doroti.Ui.Color(4294960511L), [200L] = new global::Doroti.Ui.Color(_amberAccentPrimaryValue), [400L] = new global::Doroti.Ui.Color(4294951936L), [700L] = new global::Doroti.Ui.Color(4294945536L) });
    internal const long _amberAccentPrimaryValue = 4294956864L;
    public static MaterialColor orange = new MaterialColor(_orangePrimaryValue, new DartMap<long, Color> { [50L] = new global::Doroti.Ui.Color(4294964192L), [100L] = new global::Doroti.Ui.Color(4294959282L), [200L] = new global::Doroti.Ui.Color(4294954112L), [300L] = new global::Doroti.Ui.Color(4294948685L), [400L] = new global::Doroti.Ui.Color(4294944550L), [500L] = new global::Doroti.Ui.Color(_orangePrimaryValue), [600L] = new global::Doroti.Ui.Color(4294675456L), [700L] = new global::Doroti.Ui.Color(4294278144L), [800L] = new global::Doroti.Ui.Color(4293880832L), [900L] = new global::Doroti.Ui.Color(4293284096L) });
    internal const long _orangePrimaryValue = 4294940672L;
    public static MaterialAccentColor orangeAccent = new MaterialAccentColor(_orangeAccentPrimaryValue, new DartMap<long, Color> { [100L] = new global::Doroti.Ui.Color(4294955392L), [200L] = new global::Doroti.Ui.Color(_orangeAccentPrimaryValue), [400L] = new global::Doroti.Ui.Color(4294938880L), [700L] = new global::Doroti.Ui.Color(4294929664L) });
    internal const long _orangeAccentPrimaryValue = 4294945600L;
    public static MaterialColor deepOrange = new MaterialColor(_deepOrangePrimaryValue, new DartMap<long, Color> { [50L] = new global::Doroti.Ui.Color(4294699495L), [100L] = new global::Doroti.Ui.Color(4294954172L), [200L] = new global::Doroti.Ui.Color(4294945681L), [300L] = new global::Doroti.Ui.Color(4294937189L), [400L] = new global::Doroti.Ui.Color(4294930499L), [500L] = new global::Doroti.Ui.Color(_deepOrangePrimaryValue), [600L] = new global::Doroti.Ui.Color(4294201630L), [700L] = new global::Doroti.Ui.Color(4293282329L), [800L] = new global::Doroti.Ui.Color(4292363029L), [900L] = new global::Doroti.Ui.Color(4290721292L) });
    internal const long _deepOrangePrimaryValue = 4294924066L;
    public static MaterialAccentColor deepOrangeAccent = new MaterialAccentColor(_deepOrangeAccentPrimaryValue, new DartMap<long, Color> { [100L] = new global::Doroti.Ui.Color(4294942336L), [200L] = new global::Doroti.Ui.Color(_deepOrangeAccentPrimaryValue), [400L] = new global::Doroti.Ui.Color(4294917376L), [700L] = new global::Doroti.Ui.Color(4292684800L) });
    internal const long _deepOrangeAccentPrimaryValue = 4294929984L;
    public static MaterialColor brown = new MaterialColor(_brownPrimaryValue, new DartMap<long, Color> { [50L] = new global::Doroti.Ui.Color(4293913577L), [100L] = new global::Doroti.Ui.Color(4292332744L), [200L] = new global::Doroti.Ui.Color(4290554532L), [300L] = new global::Doroti.Ui.Color(4288776319L), [400L] = new global::Doroti.Ui.Color(4287458915L), [500L] = new global::Doroti.Ui.Color(_brownPrimaryValue), [600L] = new global::Doroti.Ui.Color(4285353025L), [700L] = new global::Doroti.Ui.Color(4284301367L), [800L] = new global::Doroti.Ui.Color(4283315246L), [900L] = new global::Doroti.Ui.Color(4282263331L) });
    internal const long _brownPrimaryValue = 4286141768L;
    public static MaterialColor grey = new MaterialColor(_greyPrimaryValue, new DartMap<long, Color> { [50L] = new global::Doroti.Ui.Color(4294638330L), [100L] = new global::Doroti.Ui.Color(4294309365L), [200L] = new global::Doroti.Ui.Color(4293848814L), [300L] = new global::Doroti.Ui.Color(4292927712L), [350L] = new global::Doroti.Ui.Color(4292269782L), [400L] = new global::Doroti.Ui.Color(4290624957L), [500L] = new global::Doroti.Ui.Color(_greyPrimaryValue), [600L] = new global::Doroti.Ui.Color(4285887861L), [700L] = new global::Doroti.Ui.Color(4284572001L), [800L] = new global::Doroti.Ui.Color(4282532418L), [850L] = new global::Doroti.Ui.Color(4281348144L), [900L] = new global::Doroti.Ui.Color(4280361249L) });
    internal const long _greyPrimaryValue = 4288585374L;
    public static MaterialColor blueGrey = new MaterialColor(_blueGreyPrimaryValue, new DartMap<long, Color> { [50L] = new global::Doroti.Ui.Color(4293718001L), [100L] = new global::Doroti.Ui.Color(4291811548L), [200L] = new global::Doroti.Ui.Color(4289773253L), [300L] = new global::Doroti.Ui.Color(4287669422L), [400L] = new global::Doroti.Ui.Color(4286091420L), [500L] = new global::Doroti.Ui.Color(_blueGreyPrimaryValue), [600L] = new global::Doroti.Ui.Color(4283723386L), [700L] = new global::Doroti.Ui.Color(4282735204L), [800L] = new global::Doroti.Ui.Color(4281812815L), [900L] = new global::Doroti.Ui.Color(4280693304L) });
    internal const long _blueGreyPrimaryValue = 4284513675L;
    public static List<MaterialColor> primaries = new List<MaterialColor> { red, pink, purple, deepPurple, indigo, blue, lightBlue, cyan, teal, green, lightGreen, lime, yellow, amber, orange, deepOrange, brown, blueGrey };
    public static List<MaterialAccentColor> accents = new List<MaterialAccentColor> { redAccent, pinkAccent, purpleAccent, deepPurpleAccent, indigoAccent, blueAccent, lightBlueAccent, cyanAccent, tealAccent, greenAccent, lightGreenAccent, limeAccent, yellowAccent, amberAccent, orangeAccent, deepOrangeAccent };

}
