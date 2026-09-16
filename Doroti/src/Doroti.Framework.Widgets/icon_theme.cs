// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/icon_theme.dart
using Doroti.Runtime;

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
        return new Builder(builder: (context) =>
        {
            return new IconTheme(key: key, data: _getInheritedIconThemeData(context).merge(data), child: child);
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static IconThemeData of(BuildContext context)
    {
        IconThemeData iconThemeData = _getInheritedIconThemeData(context).resolve(context);
        return iconThemeData.isConcrete ? iconThemeData : iconThemeData.copyWith(size: iconThemeData.size ?? IconThemeData.CreateFallback().size, fill: iconThemeData.fill ?? IconThemeData.CreateFallback().fill, weight: iconThemeData.weight ?? IconThemeData.CreateFallback().weight, grade: iconThemeData.grade ?? IconThemeData.CreateFallback().grade, opticalSize: iconThemeData.opticalSize ?? IconThemeData.CreateFallback().opticalSize, color: iconThemeData.color ?? IconThemeData.CreateFallback().color, opacity: iconThemeData.opacity ?? IconThemeData.CreateFallback().opacity, shadows: iconThemeData.shadows ?? IconThemeData.CreateFallback().shadows, applyTextScaling: iconThemeData.applyTextScaling ?? IconThemeData.CreateFallback().applyTextScaling);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static IconThemeData _getInheritedIconThemeData(BuildContext context)
    {
        IconTheme? iconTheme = context.dependOnInheritedWidgetOfExactType<IconTheme>();
        return iconTheme?.data ?? IconThemeData.CreateFallback();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>(!Equals(data, ((IconTheme)oldWidget).data));
    public override Widget wrap(BuildContext context, Widget child)
    {
        return new IconTheme(data: data, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        data.debugFillProperties(properties);
    }

}

