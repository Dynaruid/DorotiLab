// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/orientation_builder.dart
namespace Doroti.Framework.Widgets;

public delegate Widget OrientationWidgetBuilder(BuildContext context, Orientation orientation);

public class OrientationBuilder : StatelessWidget
{
    public virtual Func<BuildContext, Orientation, Widget> builder { get; private set; } = default!;

    public OrientationBuilder(
        Key? key = null,
        Func<BuildContext, Orientation, Widget> builder = default!
    )
        : base(key: key)
    {
        this.builder = builder;
    }

    internal virtual Widget _buildWithConstraints(BuildContext context, BoxConstraints constraints)
    {
        Orientation orientation =
            (constraints.maxWidth > constraints.maxHeight)
                ? Orientation.landscape
                : Orientation.portrait;
        return builder(context, orientation);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Widget build(BuildContext context)
    {
        return new LayoutBuilder(builder: _buildWithConstraints);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class DeviceOrientationBuilder : StatelessWidget
{
    public virtual Func<BuildContext, Orientation, Widget> builder { get; private set; } = default!;

    public DeviceOrientationBuilder(
        Key? key = null,
        Func<BuildContext, Orientation, Widget> builder = default!
    )
        : base(key: key)
    {
        this.builder = builder;
    }

    public override Widget build(BuildContext context)
    {
        Orientation orientation = MediaQuery.orientationOf(context);
        return builder(context, orientation);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}
