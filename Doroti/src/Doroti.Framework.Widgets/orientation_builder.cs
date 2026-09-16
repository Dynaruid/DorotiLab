// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/orientation_builder.dart
namespace Doroti.Framework.Widgets;

public delegate Widget OrientationWidgetBuilder(BuildContext context, Orientation orientation);

public class OrientationBuilder : StatelessWidget
{
    public virtual global::System.Func<BuildContext, Orientation, Widget> builder { get; private set; } = default!;

    public OrientationBuilder(global::Doroti.Framework.Foundation.Key? key = null, global::System.Func<BuildContext, Orientation, Widget> builder = default!) : base(key: key)
    {
        this.builder = builder;
    }

    internal virtual Widget _buildWithConstraints(BuildContext context, global::Doroti.Framework.Rendering.BoxConstraints constraints)
    {
        Orientation orientation = ((((global::Doroti.Framework.Rendering.BoxConstraints)constraints).maxWidth > ((global::Doroti.Framework.Rendering.BoxConstraints)constraints).maxHeight) ? Orientation.landscape : Orientation.portrait);
        return this.builder(context, orientation);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)new LayoutBuilder(builder: (global::System.Func<BuildContext, global::Doroti.Framework.Rendering.BoxConstraints, Widget>)this._buildWithConstraints));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class DeviceOrientationBuilder : StatelessWidget
{
    public virtual global::System.Func<BuildContext, Orientation, Widget> builder { get; private set; } = default!;

    public DeviceOrientationBuilder(global::Doroti.Framework.Foundation.Key? key = null, global::System.Func<BuildContext, Orientation, Widget> builder = default!) : base(key: key)
    {
        this.builder = builder;
    }

    public override Widget build(BuildContext context)
    {
        Orientation orientation = MediaQuery.orientationOf(context);
        return this.builder(context, orientation);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

