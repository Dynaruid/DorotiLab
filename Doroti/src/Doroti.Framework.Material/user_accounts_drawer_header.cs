// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../flutter-master/packages/flutter/lib/src/material/user_accounts_drawer_header.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Material;

internal class _AccountPictures__user_accounts_drawer_header : global::Doroti.Generated.Framework.Widgets.StatelessWidget
{
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? currentAccountPicture { get; private set; }
    public virtual List<global::Doroti.Generated.Framework.Widgets.Widget>? otherAccountsPictures { get; private set; }
    public virtual Size? currentAccountPictureSize { get; private set; }
    public virtual Size? otherAccountsPicturesSize { get; private set; }

    internal _AccountPictures__user_accounts_drawer_header(global::Doroti.Generated.Framework.Widgets.Widget? currentAccountPicture = null, List<global::Doroti.Generated.Framework.Widgets.Widget>? otherAccountsPictures = null, Size? currentAccountPictureSize = null, Size? otherAccountsPicturesSize = null)
    {
        this.currentAccountPicture = currentAccountPicture;
        this.otherAccountsPictures = otherAccountsPictures;
        this.currentAccountPictureSize = currentAccountPictureSize;
        this.otherAccountsPicturesSize = otherAccountsPicturesSize;
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Stack(children: new List<global::Doroti.Generated.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.PositionedDirectional(top: 0.0, end: 0.0, child: new global::Doroti.Generated.Framework.Widgets.Row(children: ((this.otherAccountsPictures ?? new List<global::Doroti.Generated.Framework.Widgets.Widget>())).take(3L).map<global::Doroti.Generated.Framework.Widgets.Widget, global::Doroti.Generated.Framework.Widgets.Widget>(((picture) => {
return new global::Doroti.Generated.Framework.Widgets.Padding(padding: global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: 8.0), child: new global::Doroti.Generated.Framework.Widgets.Semantics(container: true, child: new global::Doroti.Generated.Framework.Widgets.Padding(padding: global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateOnly(left: 8.0, bottom: 8.0), child: global::Doroti.Generated.Framework.Widgets.SizedBox.CreateFromSize(size: this.otherAccountsPicturesSize, child: picture))));
throw new InvalidOperationException("Dart closure completed without a value.");
})).ToList()))), DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Positioned(top: 0.0, child: new global::Doroti.Generated.Framework.Widgets.Semantics(explicitChildNodes: true, child: global::Doroti.Generated.Framework.Widgets.SizedBox.CreateFromSize(size: this.currentAccountPictureSize, child: this.currentAccountPicture)))) }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _AccountDetails__user_accounts_drawer_header : global::Doroti.Generated.Framework.Widgets.StatefulWidget
{
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? accountName { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? accountEmail { get; private set; }
    public virtual global::System.Action? onTap { get; private set; }
    public virtual bool isOpen { get; private set; } = default!;
    public virtual Color? arrowColor { get; private set; }

    internal _AccountDetails__user_accounts_drawer_header(global::Doroti.Generated.Framework.Widgets.Widget? accountName, global::Doroti.Generated.Framework.Widgets.Widget? accountEmail, global::System.Action? onTap = null, bool isOpen = default!, Color? arrowColor = null)
    {
        this.accountName = accountName;
        this.accountEmail = accountEmail;
        this.onTap = onTap;
        this.isOpen = isOpen;
        this.arrowColor = arrowColor;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _AccountDetailsState__user_accounts_drawer_header());
}

public class _AccountDetailsState__user_accounts_drawer_header : global::Doroti.Generated.Framework.Widgets.State<_AccountDetails__user_accounts_drawer_header>, global::Doroti.Generated.Framework.Widgets.SingleTickerProviderStateMixin<_AccountDetails__user_accounts_drawer_header>
{
    internal virtual global::Doroti.Generated.Framework.Animation.CurvedAnimation _animation { get; private set; } = default!;
    internal virtual global::Doroti.Generated.Framework.Animation.AnimationController _controller { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Scheduler.Ticker? _ticker { get; set; } = default;
    public virtual global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override void initState()
    {
        base.initState();
        _controller = new global::Doroti.Generated.Framework.Animation.AnimationController(value: (((_AccountDetails__user_accounts_drawer_header)this.widget).isOpen ? 1.0 : 0.0), duration: Duration.Create(milliseconds: 200L), vsync: this);
        _animation = ((Func<global::Doroti.Generated.Framework.Animation.CurvedAnimation>)(() =>
{            var __cascade = new global::Doroti.Generated.Framework.Animation.CurvedAnimation(parent: this._controller, curve: global::Doroti.Generated.Framework.Animation.Curves.fastOutSlowIn, reverseCurve: global::Doroti.Generated.Framework.Animation.Curves.fastOutSlowIn.flipped);
            __cascade.addListener(((global::System.Action)(() => { setState(((global::System.Action)(() => {
}))); })));
            return __cascade;        }))();
    }

    public override void dispose()
    {
        this._controller.dispose();
        this._animation.dispose();
        DartRuntimePrimitives.Assert(() =>
            {
                if (((this._ticker is null) || !this._ticker!.isActive))
                {
                    return true;
                }
                throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Generated.Framework.Foundation.ErrorDescription($"{this.GetType()} created a Ticker via its SingleTickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. The Ticker must " + "be disposed before calling super.dispose()."), new global::Doroti.Generated.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), this._ticker!.describeForError("The offending ticker was") }));
            });
        this._tickerModeNotifier?.removeListener(() => this._updateTicker());
        _tickerModeNotifier = null;
        base.dispose();
    }

    public override void didUpdateWidget(_AccountDetails__user_accounts_drawer_header oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((((_AccountDetails__user_accounts_drawer_header)oldWidget).isOpen == ((_AccountDetails__user_accounts_drawer_header)this.widget).isOpen))
        {
            return;
        }
        if (((_AccountDetails__user_accounts_drawer_header)this.widget).isOpen)
        {
            this._controller.forward();
        }
        else
        {
            this._controller.reverse();
        }
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Generated.Framework.Widgets.DebugLibrary.debugCheckHasDirectionality(context));
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterialLocalizations(context));
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterialLocalizations(context));
        ThemeData theme__3760 = Theme.of(context);
        MaterialLocalizations localizations__3819 = MaterialLocalizations.of(context);
        global::Doroti.Generated.Framework.Widgets.Widget accountDetails__3882 = ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.CustomMultiChildLayout(@delegate: new _AccountDetailsLayout__user_accounts_drawer_header(textDirection: Directionality.of(context)), children: ((Func<List<global::Doroti.Generated.Framework.Widgets.Widget>>)(() => { var __collection4021 = new List<global::Doroti.Generated.Framework.Widgets.Widget>(); if ((((_AccountDetails__user_accounts_drawer_header)this.widget).accountName is not null)) { __collection4021.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.LayoutId(id: _AccountDetailsLayout__user_accounts_drawer_header.accountName, child: new global::Doroti.Generated.Framework.Widgets.Padding(padding: global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateSymmetric(vertical: 2.0), child: new global::Doroti.Generated.Framework.Widgets.DefaultTextStyle(style: theme__3760.primaryTextTheme.bodyLarge!, overflow: global::Doroti.Generated.Framework.Painting.TextOverflow.ellipsis, child: ((_AccountDetails__user_accounts_drawer_header)this.widget).accountName!))))); } if ((((_AccountDetails__user_accounts_drawer_header)this.widget).accountEmail is not null)) { __collection4021.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.LayoutId(id: _AccountDetailsLayout__user_accounts_drawer_header.accountEmail, child: new global::Doroti.Generated.Framework.Widgets.Padding(padding: global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateSymmetric(vertical: 2.0), child: new global::Doroti.Generated.Framework.Widgets.DefaultTextStyle(style: theme__3760.primaryTextTheme.bodyMedium!, overflow: global::Doroti.Generated.Framework.Painting.TextOverflow.ellipsis, child: ((_AccountDetails__user_accounts_drawer_header)this.widget).accountEmail!))))); } if ((((_AccountDetails__user_accounts_drawer_header)this.widget).onTap is not null)) { __collection4021.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.LayoutId(id: _AccountDetailsLayout__user_accounts_drawer_header.dropdownIcon, child: new global::Doroti.Generated.Framework.Widgets.Semantics(container: true, button: true, onTap: () => ((_AccountDetails__user_accounts_drawer_header)this.widget).onTap(), child: global::Doroti.Generated.Framework.Widgets.SizedBox.CreateSquare(dimension: User_accounts_drawer_headerLibrary._kAccountDetailsHeight, child: new global::Doroti.Generated.Framework.Widgets.Center(child: global::Doroti.Generated.Framework.Widgets.Transform.CreateRotate(angle: (((global::Doroti.Generated.Framework.Animation.CurvedAnimation)this._animation).value * Dart_mathLibrary.pi), child: new global::Doroti.Generated.Framework.Widgets.Icon(Icons.arrow_drop_down, color: ((_AccountDetails__user_accounts_drawer_header)this.widget).arrowColor, semanticLabel: (((_AccountDetails__user_accounts_drawer_header)this.widget).isOpen ? localizations__3819.hideAccountsLabel : localizations__3819.showAccountsLabel))))))))); } return __collection4021; }))()));
        if ((((_AccountDetails__user_accounts_drawer_header)this.widget).onTap is not null))
        {
            accountDetails__3882 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new InkWell(onTap: ((_AccountDetails__user_accounts_drawer_header)this.widget).onTap, excludeFromSemantics: true, child: accountDetails__3882));
        }
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.SizedBox(height: User_accounts_drawer_headerLibrary._kAccountDetailsHeight, child: accountDetails__3882));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Generated.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this._ticker is null))
                {
                    return true;
                }
                throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary($"{this.GetType()} is a SingleTickerProviderStateMixin but multiple tickers were created."), new global::Doroti.Generated.Framework.Foundation.ErrorDescription("A SingleTickerProviderStateMixin can only be used as a TickerProvider once."), new global::Doroti.Generated.Framework.Foundation.ErrorHint("If a State is used for multiple AnimationController objects, or if it is passed to other " + "objects and those objects might use it more than one time in total, then instead of " + "mixing in a SingleTickerProviderStateMixin, use a regular TickerProviderStateMixin.") }));
            });
        this._ticker = new global::Doroti.Generated.Framework.Scheduler.Ticker((global::System.Action<Duration>)onTick, debugLabel: (global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kDebugMode ? $"created by {(global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}" : null));
        _updateTickerModeNotifier();
        _updateTicker();
        return this._ticker!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void activate()
    {
        base.activate();
        _updateTickerModeNotifier();
        _updateTicker();
    }

    public virtual void _updateTicker()
    {
        TickerModeData values__15157 = this._tickerModeNotifier!.value;
        if ((this._ticker is not null))
        {
            this._ticker!.muted = !((TickerModeData)values__15157).enabled;
            this._ticker!.forceFrames = ((TickerModeData)values__15157).forceFrames;
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData> newNotifier__15400 = ((global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData>)(object?)TickerMode.getValuesNotifier(this.context));
        if ((object.Equals(newNotifier__15400, this._tickerModeNotifier)))
        {
            return;
        }
        this._tickerModeNotifier?.removeListener(() => this._updateTicker());
        newNotifier__15400.addListener(() => this._updateTicker());
        this._tickerModeNotifier = newNotifier__15400;
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        string? tickerDescription__15805 = ((this._ticker?.isActive, this._ticker?.muted) switch { (true, true) => "active but muted", (true, _) => "active", (false, true) => "inactive and muted", (false, _) => "inactive", (null, _) => DartRuntimePrimitives.ConvertValue<string>(null) });
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Scheduler.Ticker>("ticker", this._ticker, description: tickerDescription__15805, showSeparator: false, defaultValue: default));
    }

}

public static partial class User_accounts_drawer_headerLibrary
{
    internal static double _kAccountDetailsHeight = 56.0;
}

internal class _AccountDetailsLayout__user_accounts_drawer_header : global::Doroti.Generated.Framework.Rendering.MultiChildLayoutDelegate
{
    public const string accountName = "accountName";
    public const string accountEmail = "accountEmail";
    public const string dropdownIcon = "dropdownIcon";
    public virtual TextDirection textDirection { get; private set; } = default!;

    internal _AccountDetailsLayout__user_accounts_drawer_header(TextDirection textDirection)
    {
        this.textDirection = textDirection;
    }

    public override void performLayout(Size size)
    {
        global::Doroti.Ui.Size? iconSize__6462 = default!;
        if (hasChild(dropdownIcon))
        {
            iconSize__6462 = layoutChild(dropdownIcon, global::Doroti.Generated.Framework.Rendering.BoxConstraints.CreateLoose(size));
            positionChild(dropdownIcon, _offsetForIcon(size, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(iconSize__6462))));
        }
        string? bottomLine__6746 = (hasChild(accountEmail) ? accountEmail : ((hasChild(accountName) ? accountName : null)));
        if ((bottomLine__6746 is not null))
        {
            var constraintSize__6904 = ((iconSize__6462 is null) ? size : new global::Doroti.Ui.Size((size.width - DartRuntimePrimitives.RequireValue(iconSize__6462).width), size.height));
            iconSize__6462 ??= new global::Doroti.Ui.Size(User_accounts_drawer_headerLibrary._kAccountDetailsHeight, User_accounts_drawer_headerLibrary._kAccountDetailsHeight);
            global::Doroti.Ui.Size bottomLineSize__7176 = ((global::Doroti.Ui.Size)(object?)layoutChild(bottomLine__6746, global::Doroti.Generated.Framework.Rendering.BoxConstraints.CreateLoose(constraintSize__6904)));
            global::Doroti.Ui.Offset bottomLineOffset__7275 = ((global::Doroti.Ui.Offset)(object?)_offsetForBottomLine(size, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(iconSize__6462)), bottomLineSize__7176));
            positionChild(bottomLine__6746, bottomLineOffset__7275);
            if (((bottomLine__6746 == accountEmail) && hasChild(accountName)))
            {
                global::Doroti.Ui.Size nameSize__7532 = ((global::Doroti.Ui.Size)(object?)layoutChild(accountName, global::Doroti.Generated.Framework.Rendering.BoxConstraints.CreateLoose(constraintSize__6904)));
                positionChild(accountName, _offsetForName(size, nameSize__7532, bottomLineOffset__7275));
            }
        }
    }

    public override bool shouldRelayout(global::Doroti.Generated.Framework.Rendering.MultiChildLayoutDelegate oldDelegate) => true;
    internal virtual global::Doroti.Ui.Offset _offsetForIcon(Size size, Size iconSize)
    {
        return (this.textDirection switch { TextDirection.ltr => new global::Doroti.Ui.Offset((size.width - iconSize.width), (size.height - iconSize.height)), TextDirection.rtl => new global::Doroti.Ui.Offset(0.0, (size.height - iconSize.height)), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Offset _offsetForBottomLine(Size size, Size iconSize, Size bottomLineSize)
    {
        double y__8156 = ((size.height - (0.5 * iconSize.height)) - (0.5 * bottomLineSize.height));
        return (this.textDirection switch { TextDirection.ltr => new global::Doroti.Ui.Offset(0.0, y__8156), TextDirection.rtl => new global::Doroti.Ui.Offset((size.width - bottomLineSize.width), y__8156), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Offset _offsetForName(Size size, Size nameSize, Offset bottomLineOffset)
    {
        double y__8485 = (bottomLineOffset.dy - nameSize.height);
        return (this.textDirection switch { TextDirection.ltr => new global::Doroti.Ui.Offset(0.0, y__8485), TextDirection.rtl => new global::Doroti.Ui.Offset((size.width - nameSize.width), y__8485), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class UserAccountsDrawerHeader : global::Doroti.Generated.Framework.Widgets.StatefulWidget
{
    public virtual global::Doroti.Generated.Framework.Painting.Decoration? decoration { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? margin { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? currentAccountPicture { get; private set; }
    public virtual List<global::Doroti.Generated.Framework.Widgets.Widget>? otherAccountsPictures { get; private set; }
    public virtual Size currentAccountPictureSize { get; private set; } = default!;
    public virtual Size otherAccountsPicturesSize { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? accountName { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? accountEmail { get; private set; }
    public virtual global::System.Action? onDetailsPressed { get; private set; }
    public virtual Color arrowColor { get; private set; } = default!;

    public UserAccountsDrawerHeader(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Painting.Decoration? decoration = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? margin = default!, global::Doroti.Generated.Framework.Widgets.Widget? currentAccountPicture = null, List<global::Doroti.Generated.Framework.Widgets.Widget>? otherAccountsPictures = null, Size? currentAccountPictureSize = null, Size? otherAccountsPicturesSize = null, global::Doroti.Generated.Framework.Widgets.Widget? accountName = default!, global::Doroti.Generated.Framework.Widgets.Widget? accountEmail = default!, global::System.Action? onDetailsPressed = null, Color arrowColor = default!) : base(key: key)
    {
        global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? __margin = margin ?? global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateOnly(bottom: 8.0);
        Size __currentAccountPictureSize = currentAccountPictureSize ?? Size.CreateSquare(72.0);
        Size __otherAccountsPicturesSize = otherAccountsPicturesSize ?? Size.CreateSquare(40.0);
        Color __arrowColor = arrowColor ?? Colors.white;
        this.decoration = decoration;
        this.margin = __margin;
        this.currentAccountPicture = currentAccountPicture;
        this.otherAccountsPictures = otherAccountsPictures;
        this.currentAccountPictureSize = __currentAccountPictureSize;
        this.otherAccountsPicturesSize = __otherAccountsPicturesSize;
        this.accountName = accountName;
        this.accountEmail = accountEmail;
        this.onDetailsPressed = onDetailsPressed;
        this.arrowColor = __arrowColor;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _UserAccountsDrawerHeaderState__user_accounts_drawer_header());
}

internal class _UserAccountsDrawerHeaderState__user_accounts_drawer_header : global::Doroti.Generated.Framework.Widgets.State<UserAccountsDrawerHeader>
{
    internal virtual bool _isOpen { get; set; } = false;

    internal virtual void _handleDetailsPressed()
    {
        setState(((global::System.Action)(() => {
_isOpen = !this._isOpen;
})));
        ((UserAccountsDrawerHeader)this.widget).onDetailsPressed!();
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterial(context));
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterialLocalizations(context));
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Semantics(container: true, label: MaterialLocalizations.of(context).signedInLabel, child: new DrawerHeader(decoration: (((UserAccountsDrawerHeader)this.widget).decoration ?? new global::Doroti.Generated.Framework.Painting.BoxDecoration(color: Theme.of(context).colorScheme.primary)), margin: ((UserAccountsDrawerHeader)this.widget).margin, padding: global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional.CreateOnly(top: 16.0, start: 16.0), child: new global::Doroti.Generated.Framework.Widgets.SafeArea(bottom: false, child: new global::Doroti.Generated.Framework.Widgets.Column(crossAxisAlignment: global::Doroti.Generated.Framework.Rendering.CrossAxisAlignment.stretch, children: new List<global::Doroti.Generated.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Expanded(child: new global::Doroti.Generated.Framework.Widgets.Padding(padding: global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional.CreateOnly(end: 16.0), child: new _AccountPictures__user_accounts_drawer_header(currentAccountPicture: ((UserAccountsDrawerHeader)this.widget).currentAccountPicture, otherAccountsPictures: ((UserAccountsDrawerHeader)this.widget).otherAccountsPictures, currentAccountPictureSize: ((UserAccountsDrawerHeader)this.widget).currentAccountPictureSize, otherAccountsPicturesSize: ((UserAccountsDrawerHeader)this.widget).otherAccountsPicturesSize)))), DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new _AccountDetails__user_accounts_drawer_header(accountName: ((UserAccountsDrawerHeader)this.widget).accountName, accountEmail: ((UserAccountsDrawerHeader)this.widget).accountEmail, isOpen: this._isOpen, onTap: ((global::System.Action)((((UserAccountsDrawerHeader)this.widget).onDetailsPressed is null) ? null : this._handleDetailsPressed)), arrowColor: ((UserAccountsDrawerHeader)this.widget).arrowColor)) })))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
