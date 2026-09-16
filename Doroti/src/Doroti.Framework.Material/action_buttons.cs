// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/action_buttons.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public abstract class _ActionButton__action_buttons : IconButton
{
    public virtual StandardComponentType? standardComponent { get; private set; }

    internal _ActionButton__action_buttons(Key? key = null, Color? color = null, ButtonStyle? style = null, Action? onPressed = null, Widget icon = default!, StandardComponentType? standardComponent = null) : base(key: key, color: color, style: style, onPressed: onPressed, icon: icon)
    {
        this.standardComponent = standardComponent;
    }

    internal abstract string _getTooltip(BuildContext context);
    internal abstract void _onPressedCallback(BuildContext context);
    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterialLocalizations(context));
        return new IconButton(key: standardComponent is { } component ? StandardComponentTypeMembers.key(component) : null, icon: icon, style: style, color: color, tooltip: _getTooltip(context), onPressed: () =>
        {
            if (onPressed is not null)
            {
                onPressed!();
            }
            else
            {
                _onPressedCallback(context);
            }
        });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal delegate Func<BuildContext, Widget>? _ActionIconBuilderCallback__action_buttons(ActionIconThemeData? actionIconTheme);

internal delegate IconData _ActionIconDataCallback__action_buttons(BuildContext context);

internal delegate string _AndroidSemanticsLabelCallback__action_buttons(MaterialLocalizations materialLocalization);

internal class _ActionIcon__action_buttons : StatelessWidget
{
    public virtual Func<ActionIconThemeData?, Func<BuildContext, Widget>?> iconBuilderCallback { get; private set; } = default!;
    public virtual Func<BuildContext, IconData> getIcon { get; private set; } = default!;
    public virtual Func<MaterialLocalizations, string> getAndroidSemanticsLabel { get; private set; } = default!;

    internal _ActionIcon__action_buttons(Func<ActionIconThemeData?, Func<BuildContext, Widget>?> iconBuilderCallback, Func<BuildContext, IconData> getIcon, Func<MaterialLocalizations, string> getAndroidSemanticsLabel)
    {
        this.iconBuilderCallback = iconBuilderCallback;
        this.getIcon = getIcon;
        this.getAndroidSemanticsLabel = getAndroidSemanticsLabel;
    }

    public override Widget build(BuildContext context)
    {
        ActionIconThemeData? actionIconTheme = ActionIconTheme.of(context);
        Func<BuildContext, Widget>? iconBuilder = iconBuilderCallback(actionIconTheme);
        if (iconBuilder is not null)
        {
            return iconBuilder(context);
        }
        IconData data = getIcon(context);
        string? semanticsLabel = default!;
        switch (PlatformLibrary.defaultTargetPlatform)
        {
            case TargetPlatform.android:
                {
                    semanticsLabel = getAndroidSemanticsLabel(MaterialLocalizations.of(context));
                    break;
                }
            case TargetPlatform.fuchsia:
            case TargetPlatform.linux:
            case TargetPlatform.windows:
            case TargetPlatform.iOS:
            case TargetPlatform.macOS:
                {
                    semanticsLabel = null;
                    break;
                }
        }
        return new Icon(data, semanticLabel: semanticsLabel);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class BackButtonIcon : StatelessWidget
{
    public BackButtonIcon(Key? key = null) : base(key: key)
    {
    }

    public override Widget build(BuildContext context)
    {
        return new _ActionIcon__action_buttons(iconBuilderCallback: (actionIconTheme) =>
        {
            return actionIconTheme?.backButtonIconBuilder;
            throw new InvalidOperationException("Dart closure completed without a value.");
        }, getIcon: (context) =>
        {
            if (Foundation.ConstantsLibrary.kIsWeb)
            {
                return Icons.arrow_back;
            }
            switch (Theme.of(context).platform)
            {
                case TargetPlatform.android:
                case TargetPlatform.fuchsia:
                case TargetPlatform.linux:
                case TargetPlatform.windows:
                    {
                        return Icons.arrow_back;
                    }
                case TargetPlatform.iOS:
                case TargetPlatform.macOS:
                    {
                        return Icons.arrow_back_ios_new_rounded;
                    }
                default:
                    throw new InvalidOperationException("Non-exhaustive Dart switch value.");
            }
            throw new InvalidOperationException("Dart closure completed without a value.");
        }, getAndroidSemanticsLabel: (materialLocalization) =>
        {
            return materialLocalization.backButtonTooltip;
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class BackButton : _ActionButton__action_buttons
{
    public BackButton(Key? key = null, Color? color = null, ButtonStyle? style = null, Action? onPressed = null) : base(key: key, color: color, style: style, onPressed: onPressed, icon: new BackButtonIcon(), standardComponent: StandardComponentType.backButton)
    {
    }

    internal override void _onPressedCallback(BuildContext context) => Navigator.maybePop<object>(context);
    internal override string _getTooltip(BuildContext context)
    {
        return MaterialLocalizations.of(context).backButtonTooltip;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class CloseButtonIcon : StatelessWidget
{
    public CloseButtonIcon(Key? key = null) : base(key: key)
    {
    }

    public override Widget build(BuildContext context)
    {
        return new _ActionIcon__action_buttons(iconBuilderCallback: (actionIconTheme) =>
        {
            return actionIconTheme?.closeButtonIconBuilder;
            throw new InvalidOperationException("Dart closure completed without a value.");
        }, getIcon: (context) => Icons.close, getAndroidSemanticsLabel: (materialLocalization) =>
        {
            return materialLocalization.closeButtonTooltip;
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class CloseButton : _ActionButton__action_buttons
{
    public CloseButton(Key? key = null, Color? color = null, Action? onPressed = null, ButtonStyle? style = null) : base(key: key, color: color, onPressed: onPressed, style: style, icon: new CloseButtonIcon(), standardComponent: StandardComponentType.closeButton)
    {
    }

    internal override void _onPressedCallback(BuildContext context) => Navigator.maybePop<object>(context);
    internal override string _getTooltip(BuildContext context)
    {
        return MaterialLocalizations.of(context).closeButtonTooltip;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class DrawerButtonIcon : StatelessWidget
{
    public DrawerButtonIcon(Key? key = null) : base(key: key)
    {
    }

    public override Widget build(BuildContext context)
    {
        return new _ActionIcon__action_buttons(iconBuilderCallback: (actionIconTheme) =>
        {
            return actionIconTheme?.drawerButtonIconBuilder;
            throw new InvalidOperationException("Dart closure completed without a value.");
        }, getIcon: (context) => Icons.menu, getAndroidSemanticsLabel: (materialLocalization) =>
        {
            return materialLocalization.openAppDrawerTooltip;
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class DrawerButton : _ActionButton__action_buttons
{
    public DrawerButton(Key? key = null, Color? color = null, ButtonStyle? style = null, Action? onPressed = null) : base(key: key, color: color, style: style, onPressed: onPressed, icon: new DrawerButtonIcon(), standardComponent: StandardComponentType.drawerButton)
    {
    }

    internal override void _onPressedCallback(BuildContext context) => Scaffold.of(context).openDrawer();
    internal override string _getTooltip(BuildContext context)
    {
        return MaterialLocalizations.of(context).openAppDrawerTooltip;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class EndDrawerButtonIcon : StatelessWidget
{
    public EndDrawerButtonIcon(Key? key = null) : base(key: key)
    {
    }

    public override Widget build(BuildContext context)
    {
        return new _ActionIcon__action_buttons(iconBuilderCallback: (actionIconTheme) =>
        {
            return actionIconTheme?.endDrawerButtonIconBuilder;
            throw new InvalidOperationException("Dart closure completed without a value.");
        }, getIcon: (context) => Icons.menu, getAndroidSemanticsLabel: (materialLocalization) =>
        {
            return materialLocalization.openAppDrawerTooltip;
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class EndDrawerButton : _ActionButton__action_buttons
{
    public EndDrawerButton(Key? key = null, Color? color = null, ButtonStyle? style = null, Action? onPressed = null) : base(key: key, color: color, style: style, onPressed: onPressed, icon: new EndDrawerButtonIcon())
    {
    }

    internal override void _onPressedCallback(BuildContext context) => Scaffold.of(context).openEndDrawer();
    internal override string _getTooltip(BuildContext context)
    {
        return MaterialLocalizations.of(context).openAppDrawerTooltip;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
