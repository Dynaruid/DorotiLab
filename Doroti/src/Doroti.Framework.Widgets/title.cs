// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/title.dart
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

namespace Doroti.Generated.Framework.Widgets;

public class Title : StatefulWidget
{
    public virtual string title { get; private set; } = default!;
    public virtual Color color { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;

    public Title(global::Doroti.Generated.Framework.Foundation.Key? key = null, string title = "", Color color = default!, Widget child = default!) : base(key: key)
    {
        this.title = title;
        this.color = color;
        this.child = child;
        System.Diagnostics.Debug.Assert((((color.a * 255.0)).round().clamp(0L, 255L) == 255L));
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
        if (((((Title)oldWidget).title != ((Title)this.widget).title) || (!object.Equals(((Title)oldWidget).color, ((Title)this.widget).color))))
        {
            _updateChrome();
        }
    }

    internal virtual void _updateChrome()
    {
        DartRuntimePrimitives.Ignore(SystemChrome.setApplicationSwitcherDescription(new global::Doroti.Generated.Framework.Services.ApplicationSwitcherDescription(label: ((Title)this.widget).title, primaryColor: ((Title)this.widget).color.value)));
    }

    public override Widget build(BuildContext context)
    {
        return ((Title)this.widget).child;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.StringProperty("title", ((Title)this.widget).title, defaultValue: ""));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("color", ((Title)this.widget).color, defaultValue: null));
    }

}

