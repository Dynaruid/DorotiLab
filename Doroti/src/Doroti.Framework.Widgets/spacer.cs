// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/spacer.dart
namespace Doroti.Framework.Widgets;

public class Spacer : StatelessWidget
{
    public virtual long flex { get; private set; } = default!;

    public Spacer(Key? key = null, long flex = 1)
        : base(key: key)
    {
        this.flex = flex;
        System.Diagnostics.Debug.Assert(flex > 0L);
    }

    public override Widget build(BuildContext context)
    {
        return new Expanded(flex: flex, child: SizedBox.CreateShrink());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}
