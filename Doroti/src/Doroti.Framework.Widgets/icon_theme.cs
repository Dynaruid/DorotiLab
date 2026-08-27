// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/icon_theme.dart
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

public class IconTheme : InheritedTheme
{
    public virtual IconThemeData data { get; private set; } = default!;

    public IconTheme(global::Doroti.Framework.Foundation.Key? key = null, IconThemeData data = default!, Widget child = default!) : base(key: key, child: child)
    {
        this.data = data;
    }

    public static Widget merge(global::Doroti.Framework.Foundation.Key? key = null, IconThemeData data = default!, Widget child = default!)
    {
        return ((Widget)(object?)new Builder(builder: ((global::System.Func<BuildContext, Widget>)((context) =>
        {
            return ((Widget)(object?)new IconTheme(key: key, data: IconTheme._getInheritedIconThemeData(context).merge(data), child: child));
            throw new InvalidOperationException("Dart closure completed without a value.");
        }))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static IconThemeData of(BuildContext context)
    {
        IconThemeData iconThemeData = ((IconThemeData)(object?)IconTheme._getInheritedIconThemeData(context).resolve(context));
        return (((IconThemeData)iconThemeData).isConcrete ? iconThemeData : iconThemeData.copyWith(size: (((IconThemeData)iconThemeData).size ?? IconThemeData.CreateFallback().size), fill: (((IconThemeData)iconThemeData).fill ?? IconThemeData.CreateFallback().fill), weight: (((IconThemeData)iconThemeData).weight ?? IconThemeData.CreateFallback().weight), grade: (((IconThemeData)iconThemeData).grade ?? IconThemeData.CreateFallback().grade), opticalSize: (((IconThemeData)iconThemeData).opticalSize ?? IconThemeData.CreateFallback().opticalSize), color: (((IconThemeData)iconThemeData).color ?? IconThemeData.CreateFallback().color), opacity: (((IconThemeData)iconThemeData).opacity ?? IconThemeData.CreateFallback().opacity), shadows: (((IconThemeData)iconThemeData).shadows ?? IconThemeData.CreateFallback().shadows), applyTextScaling: (((IconThemeData)iconThemeData).applyTextScaling ?? IconThemeData.CreateFallback().applyTextScaling)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static IconThemeData _getInheritedIconThemeData(BuildContext context)
    {
        IconTheme? iconTheme = ((IconTheme?)(object?)context.dependOnInheritedWidgetOfExactType<IconTheme>());
        return (iconTheme?.data ?? IconThemeData.CreateFallback());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>((!object.Equals(this.data, ((IconTheme)oldWidget).data)));
    public override Widget wrap(BuildContext context, Widget child)
    {
        return ((Widget)(object?)new IconTheme(data: this.data, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        this.data.debugFillProperties(properties);
    }

}

