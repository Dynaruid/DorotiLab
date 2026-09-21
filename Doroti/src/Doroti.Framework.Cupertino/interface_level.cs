// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/interface_level.dart
using Doroti.Runtime;

namespace Doroti.Framework.Cupertino;

public enum CupertinoUserInterfaceLevelData
{
    @base,
    elevated,
}

public class CupertinoUserInterfaceLevel : InheritedWidget
{
    internal virtual CupertinoUserInterfaceLevelData _data { get; private set; } = default!;

    public CupertinoUserInterfaceLevel(
        Key? key = null,
        CupertinoUserInterfaceLevelData data = default!,
        Widget child = default!
    )
        : base(key: key, child: child)
    {
        _data = data;
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget) =>
        DartRuntimePrimitives.ConvertValue<bool>(
            !Equals(((CupertinoUserInterfaceLevel)oldWidget)._data, _data)
        );

    public static CupertinoUserInterfaceLevelData of(BuildContext context)
    {
        CupertinoUserInterfaceLevel? query =
            context.dependOnInheritedWidgetOfExactType<CupertinoUserInterfaceLevel>();
        if (query is not null)
        {
            return query._data;
        }
        throw DartRuntimePrimitives.AsException(
            FlutterError.Create(
                "CupertinoUserInterfaceLevel.of() called with a context that does not contain a CupertinoUserInterfaceLevel.\n"
                    + "No CupertinoUserInterfaceLevel ancestor could be found starting from the context that was passed "
                    + "to CupertinoUserInterfaceLevel.of(). This can happen because you do not have a WidgetsApp or "
                    + "MaterialApp widget (those widgets introduce a CupertinoUserInterfaceLevel), or it can happen "
                    + "if the context you use comes from a widget above those widgets.\n"
                    + "The context used was:\n"
                    + $"  {context}"
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static CupertinoUserInterfaceLevelData? maybeOf(BuildContext context)
    {
        CupertinoUserInterfaceLevel? query =
            context.dependOnInheritedWidgetOfExactType<CupertinoUserInterfaceLevel>();
        return query?._data;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(
            new EnumProperty<CupertinoUserInterfaceLevelData>("user interface level", _data)
        );
    }
}
