// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/action_buttons.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Material;

public abstract class _ActionButton__action_buttons : IconButton
{
    public virtual global::Doroti.Framework.Widgets.StandardComponentType? standardComponent { get; private set; }

    internal _ActionButton__action_buttons(global::Doroti.Framework.Foundation.Key? key = null, Color? color = null, ButtonStyle? style = null, global::System.Action? onPressed = null, global::Doroti.Framework.Widgets.Widget icon = default!, global::Doroti.Framework.Widgets.StandardComponentType? standardComponent = null) : base(key: key, color: color, style: style, onPressed: onPressed, icon: icon)
    {
        this.standardComponent = standardComponent;
    }

    internal abstract string _getTooltip(global::Doroti.Framework.Widgets.BuildContext context);
    internal abstract void _onPressedCallback(global::Doroti.Framework.Widgets.BuildContext context);
    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterialLocalizations(context));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new IconButton(key: StandardComponentTypeMembers.key(DartRuntimePrimitives.RequireValue(this.standardComponent)), icon: this.icon, style: this.style, color: this.color, tooltip: _getTooltip(context), onPressed: (() => {
if ((this.onPressed is not null))
{
    this.onPressed!();
}
else
{
    _onPressedCallback(context);
}
})));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal delegate global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget>? _ActionIconBuilderCallback__action_buttons(ActionIconThemeData? actionIconTheme);

internal delegate global::Doroti.Framework.Widgets.IconData _ActionIconDataCallback__action_buttons(global::Doroti.Framework.Widgets.BuildContext context);

internal delegate string _AndroidSemanticsLabelCallback__action_buttons(MaterialLocalizations materialLocalization);

internal class _ActionIcon__action_buttons : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual global::System.Func<ActionIconThemeData?, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget>?> iconBuilderCallback { get; private set; } = default!;
    public virtual global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.IconData> getIcon { get; private set; } = default!;
    public virtual global::System.Func<MaterialLocalizations, string> getAndroidSemanticsLabel { get; private set; } = default!;

    internal _ActionIcon__action_buttons(global::System.Func<ActionIconThemeData?, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget>?> iconBuilderCallback, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.IconData> getIcon, global::System.Func<MaterialLocalizations, string> getAndroidSemanticsLabel)
    {
        this.iconBuilderCallback = iconBuilderCallback;
        this.getIcon = getIcon;
        this.getAndroidSemanticsLabel = getAndroidSemanticsLabel;
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        ActionIconThemeData? actionIconTheme__2407 = ActionIconTheme.of(context);
        global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget>? iconBuilder__2479 = this.iconBuilderCallback(actionIconTheme__2407);
        if ((iconBuilder__2479 is not null))
        {
            return iconBuilder__2479(context);
        }
        global::Doroti.Framework.Widgets.IconData data__2623 = this.getIcon(context);
        string? semanticsLabel__2666 = default!;
        switch (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
        {
            case global::Doroti.Framework.Foundation.TargetPlatform.android:
                {
                    semanticsLabel__2666 = this.getAndroidSemanticsLabel(MaterialLocalizations.of(context));
                    break;
                }
            case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
            case global::Doroti.Framework.Foundation.TargetPlatform.linux:
            case global::Doroti.Framework.Foundation.TargetPlatform.windows:
            case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
            case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
                {
                    semanticsLabel__2666 = null;
                    break;
                }
        }
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Icon(data__2623, semanticLabel: semanticsLabel__2666));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class BackButtonIcon : global::Doroti.Framework.Widgets.StatelessWidget
{
    public BackButtonIcon(global::Doroti.Framework.Foundation.Key? key = null) : base(key: key)
    {
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new _ActionIcon__action_buttons(iconBuilderCallback: ((global::System.Func<ActionIconThemeData?, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget>?>)((actionIconTheme) => {
return ((Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget>?)(object?)actionIconTheme?.backButtonIconBuilder);
throw new InvalidOperationException("Dart closure completed without a value.");
})), getIcon: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.IconData>)((context) => {
if (global::Doroti.Framework.Foundation.ConstantsLibrary.kIsWeb)
{
    return Icons.arrow_back;
}
switch (Theme.of(context).platform)
{
    case global::Doroti.Framework.Foundation.TargetPlatform.android:
    case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
    case global::Doroti.Framework.Foundation.TargetPlatform.linux:
    case global::Doroti.Framework.Foundation.TargetPlatform.windows:
        {
            return Icons.arrow_back;
        }
    case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
    case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
        {
            return Icons.arrow_back_ios_new_rounded;
        }
    default:
        throw new InvalidOperationException("Non-exhaustive Dart switch value.");
}
throw new InvalidOperationException("Dart closure completed without a value.");
})), getAndroidSemanticsLabel: ((global::System.Func<MaterialLocalizations, string>)((materialLocalization) => {
return ((MaterialLocalizations)materialLocalization).backButtonTooltip;
throw new InvalidOperationException("Dart closure completed without a value.");
}))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class BackButton : _ActionButton__action_buttons
{
    public BackButton(global::Doroti.Framework.Foundation.Key? key = null, Color? color = null, ButtonStyle? style = null, global::System.Action? onPressed = null) : base(key: key, color: color, style: style, onPressed: onPressed, icon: new BackButtonIcon(), standardComponent: global::Doroti.Framework.Widgets.StandardComponentType.backButton)
    {
    }

    internal override void _onPressedCallback(global::Doroti.Framework.Widgets.BuildContext context) => Navigator.maybePop<object>(context);
    internal override string _getTooltip(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return MaterialLocalizations.of(context).backButtonTooltip;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class CloseButtonIcon : global::Doroti.Framework.Widgets.StatelessWidget
{
    public CloseButtonIcon(global::Doroti.Framework.Foundation.Key? key = null) : base(key: key)
    {
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new _ActionIcon__action_buttons(iconBuilderCallback: ((global::System.Func<ActionIconThemeData?, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget>?>)((actionIconTheme) => {
return ((Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget>?)(object?)actionIconTheme?.closeButtonIconBuilder);
throw new InvalidOperationException("Dart closure completed without a value.");
})), getIcon: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.IconData>)((context) => Icons.close)), getAndroidSemanticsLabel: ((global::System.Func<MaterialLocalizations, string>)((materialLocalization) => {
return ((MaterialLocalizations)materialLocalization).closeButtonTooltip;
throw new InvalidOperationException("Dart closure completed without a value.");
}))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class CloseButton : _ActionButton__action_buttons
{
    public CloseButton(global::Doroti.Framework.Foundation.Key? key = null, Color? color = null, global::System.Action? onPressed = null, ButtonStyle? style = null) : base(key: key, color: color, onPressed: onPressed, style: style, icon: new CloseButtonIcon(), standardComponent: global::Doroti.Framework.Widgets.StandardComponentType.closeButton)
    {
    }

    internal override void _onPressedCallback(global::Doroti.Framework.Widgets.BuildContext context) => Navigator.maybePop<object>(context);
    internal override string _getTooltip(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return MaterialLocalizations.of(context).closeButtonTooltip;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class DrawerButtonIcon : global::Doroti.Framework.Widgets.StatelessWidget
{
    public DrawerButtonIcon(global::Doroti.Framework.Foundation.Key? key = null) : base(key: key)
    {
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new _ActionIcon__action_buttons(iconBuilderCallback: ((global::System.Func<ActionIconThemeData?, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget>?>)((actionIconTheme) => {
return ((Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget>?)(object?)actionIconTheme?.drawerButtonIconBuilder);
throw new InvalidOperationException("Dart closure completed without a value.");
})), getIcon: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.IconData>)((context) => Icons.menu)), getAndroidSemanticsLabel: ((global::System.Func<MaterialLocalizations, string>)((materialLocalization) => {
return ((MaterialLocalizations)materialLocalization).openAppDrawerTooltip;
throw new InvalidOperationException("Dart closure completed without a value.");
}))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class DrawerButton : _ActionButton__action_buttons
{
    public DrawerButton(global::Doroti.Framework.Foundation.Key? key = null, Color? color = null, ButtonStyle? style = null, global::System.Action? onPressed = null) : base(key: key, color: color, style: style, onPressed: onPressed, icon: new DrawerButtonIcon(), standardComponent: global::Doroti.Framework.Widgets.StandardComponentType.drawerButton)
    {
    }

    internal override void _onPressedCallback(global::Doroti.Framework.Widgets.BuildContext context) => Scaffold.of(context).openDrawer();
    internal override string _getTooltip(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return MaterialLocalizations.of(context).openAppDrawerTooltip;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class EndDrawerButtonIcon : global::Doroti.Framework.Widgets.StatelessWidget
{
    public EndDrawerButtonIcon(global::Doroti.Framework.Foundation.Key? key = null) : base(key: key)
    {
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new _ActionIcon__action_buttons(iconBuilderCallback: ((global::System.Func<ActionIconThemeData?, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget>?>)((actionIconTheme) => {
return ((Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget>?)(object?)actionIconTheme?.endDrawerButtonIconBuilder);
throw new InvalidOperationException("Dart closure completed without a value.");
})), getIcon: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.IconData>)((context) => Icons.menu)), getAndroidSemanticsLabel: ((global::System.Func<MaterialLocalizations, string>)((materialLocalization) => {
return ((MaterialLocalizations)materialLocalization).openAppDrawerTooltip;
throw new InvalidOperationException("Dart closure completed without a value.");
}))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class EndDrawerButton : _ActionButton__action_buttons
{
    public EndDrawerButton(global::Doroti.Framework.Foundation.Key? key = null, Color? color = null, ButtonStyle? style = null, global::System.Action? onPressed = null) : base(key: key, color: color, style: style, onPressed: onPressed, icon: new EndDrawerButtonIcon())
    {
    }

    internal override void _onPressedCallback(global::Doroti.Framework.Widgets.BuildContext context) => Scaffold.of(context).openEndDrawer();
    internal override string _getTooltip(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return MaterialLocalizations.of(context).openAppDrawerTooltip;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
