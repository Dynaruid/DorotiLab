// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/interface_level.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Cupertino;

public enum CupertinoUserInterfaceLevelData
{
    @base,
    elevated
}

public class CupertinoUserInterfaceLevel : global::Doroti.Generated.Framework.Widgets.InheritedWidget
{
    internal virtual CupertinoUserInterfaceLevelData _data { get; private set; } = default!;

    public CupertinoUserInterfaceLevel(global::Doroti.Generated.Framework.Foundation.Key? key = null, CupertinoUserInterfaceLevelData data = default!, global::Doroti.Generated.Framework.Widgets.Widget child = default!) : base(key: key, child: child)
    {
        this._data = data;
    }

    public override bool updateShouldNotify(global::Doroti.Generated.Framework.Widgets.InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>((!object.Equals(((CupertinoUserInterfaceLevel)oldWidget)._data, this._data)));
    public static CupertinoUserInterfaceLevelData of(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        CupertinoUserInterfaceLevel? query__2634 = ((CupertinoUserInterfaceLevel?)(object?)context.dependOnInheritedWidgetOfExactType<CupertinoUserInterfaceLevel>());
        if ((query__2634 is not null))
        {
            return ((CupertinoUserInterfaceLevel)query__2634)._data;
        }
        throw DartRuntimePrimitives.AsException(global::Doroti.Generated.Framework.Foundation.FlutterError.Create("CupertinoUserInterfaceLevel.of() called with a context that does not contain a CupertinoUserInterfaceLevel.\n" + "No CupertinoUserInterfaceLevel ancestor could be found starting from the context that was passed " + "to CupertinoUserInterfaceLevel.of(). This can happen because you do not have a WidgetsApp or " + "MaterialApp widget (those widgets introduce a CupertinoUserInterfaceLevel), or it can happen " + "if the context you use comes from a widget above those widgets.\n" + "The context used was:\n" + $"  {context}"));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static CupertinoUserInterfaceLevelData? maybeOf(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        CupertinoUserInterfaceLevel? query__4083 = ((CupertinoUserInterfaceLevel?)(object?)context.dependOnInheritedWidgetOfExactType<CupertinoUserInterfaceLevel>());
        return query__4083?._data;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<CupertinoUserInterfaceLevelData>("user interface level", this._data));
    }

}
