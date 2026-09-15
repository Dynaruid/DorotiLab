// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/unique_widget.dart
namespace Doroti.Framework.Widgets;

public abstract class UniqueWidget<T> : StatefulWidget where T : IState
{
    protected UniqueWidget(GlobalKey<T> key) : base(key: key)
    {
    }

    public abstract override IState createState();
    public virtual T? currentState
    {
        get
        {
            var globalKey = ((GlobalKey<T>?)(object?)this.key!)!;
            return ((GlobalKey<T>)globalKey).currentState;
        }
    }
}

