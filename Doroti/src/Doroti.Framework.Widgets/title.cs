// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/title.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public class Title : StatefulWidget
{
    public virtual string title { get; private set; } = default!;
    public virtual Color color { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;

    public Title(global::Doroti.Framework.Foundation.Key? key = null, string title = "", Color color = default!, Widget child = default!) : base(key: key)
    {
        this.title = title;
        this.color = color;
        this.child = child;
        System.Diagnostics.Debug.Assert((color.a * 255.0).round().clamp(0L, 255L) == 255L);
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _TitleState__title());
}

internal class _TitleState__title : State<Title>
{
    public override void initState()
    {
        base.initState();
        _updateChrome();
    }

    public override void didUpdateWidget(Title oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((oldWidget.title != widget.title) || (!Equals(oldWidget.color, widget.color)))
        {
            _updateChrome();
        }
    }

    internal virtual void _updateChrome()
    {
        DartRuntimePrimitives.Ignore(SystemChrome.setApplicationSwitcherDescription(new global::Doroti.Framework.Services.ApplicationSwitcherDescription(label: widget.title, primaryColor: widget.color.value)));
    }

    public override Widget build(BuildContext context)
    {
        return widget.child;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.StringProperty("title", widget.title, defaultValue: ""));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("color", widget.color, defaultValue: null));
    }

}

