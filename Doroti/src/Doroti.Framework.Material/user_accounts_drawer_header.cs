// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/user_accounts_drawer_header.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

internal class _AccountPictures__user_accounts_drawer_header : StatelessWidget
{
    public virtual Widget? currentAccountPicture { get; private set; }
    public virtual List<Widget>? otherAccountsPictures { get; private set; }
    public virtual Size? currentAccountPictureSize { get; private set; }
    public virtual Size? otherAccountsPicturesSize { get; private set; }

    internal _AccountPictures__user_accounts_drawer_header(Widget? currentAccountPicture = null, List<Widget>? otherAccountsPictures = null, Size? currentAccountPictureSize = null, Size? otherAccountsPicturesSize = null)
    {
        this.currentAccountPicture = currentAccountPicture;
        this.otherAccountsPictures = otherAccountsPictures;
        this.currentAccountPictureSize = currentAccountPictureSize;
        this.otherAccountsPicturesSize = otherAccountsPicturesSize;
    }

    public override Widget build(BuildContext context)
    {
        return new Stack(children: new List<Widget> { DartRuntimePrimitives.ConvertValue<Widget>(new PositionedDirectional(top: 0.0, end: 0.0, child: new Row(children: (otherAccountsPictures ?? new List<Widget>()).take(3L).map<Widget, Widget>((picture) => {
return new Padding(padding: EdgeInsetsDirectional.CreateOnly(start: 8.0), child: new Widgets.Semantics(container: true, child: new Padding(padding: EdgeInsets.CreateOnly(left: 8.0, bottom: 8.0), child: SizedBox.CreateFromSize(size: otherAccountsPicturesSize, child: picture))));
throw new InvalidOperationException("Dart closure completed without a value.");
}).ToList()))), DartRuntimePrimitives.ConvertValue<Widget>(new Positioned(top: 0.0, child: new Widgets.Semantics(explicitChildNodes: true, child: SizedBox.CreateFromSize(size: currentAccountPictureSize, child: currentAccountPicture)))) });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _AccountDetails__user_accounts_drawer_header : StatefulWidget
{
    public virtual Widget? accountName { get; private set; }
    public virtual Widget? accountEmail { get; private set; }
    public virtual Action? onTap { get; private set; }
    public virtual bool isOpen { get; private set; } = default!;
    public virtual Color? arrowColor { get; private set; }

    internal _AccountDetails__user_accounts_drawer_header(Widget? accountName, Widget? accountEmail, Action? onTap = null, bool isOpen = default!, Color? arrowColor = null)
    {
        this.accountName = accountName;
        this.accountEmail = accountEmail;
        this.onTap = onTap;
        this.isOpen = isOpen;
        this.arrowColor = arrowColor;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _AccountDetailsState__user_accounts_drawer_header());
}

public class _AccountDetailsState__user_accounts_drawer_header : State<_AccountDetails__user_accounts_drawer_header>, SingleTickerProviderStateMixin<_AccountDetails__user_accounts_drawer_header>
{
    internal virtual CurvedAnimation _animation { get; private set; } = default!;
    internal virtual AnimationController _controller { get; private set; } = default!;
    public virtual Scheduler.Ticker? _ticker { get; set; } = default;
    public virtual ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override void initState()
    {
        base.initState();
        _controller = new AnimationController(value: widget.isOpen ? 1.0 : 0.0, duration: Duration.Create(milliseconds: 200L), vsync: this);
        _animation = ((Func<CurvedAnimation>)(() =>
{
    var __cascade = new CurvedAnimation(parent: _controller, curve: Curves.fastOutSlowIn, reverseCurve: Curves.fastOutSlowIn.flipped);
    __cascade.addListener(() =>
    {
        setState(() =>
        {
        });
    });
    return __cascade;
}))();
    }

    public override void dispose()
    {
        _controller.dispose();
        _animation.dispose();
        DartRuntimePrimitives.Assert(() =>
            {
                if ((_ticker is null) || !_ticker!.isActive)
                {
                    return true;
                }
                throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"{this} was disposed with an active Ticker."), new ErrorDescription($"{GetType()} created a Ticker via its SingleTickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. The Ticker must " + "be disposed before calling super.dispose()."), new ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), _ticker!.describeForError("The offending ticker was") }));
            });
        _tickerModeNotifier?.removeListener(_updateTicker);
        _tickerModeNotifier = null;
        base.dispose();
    }

    public override void didUpdateWidget(_AccountDetails__user_accounts_drawer_header oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (oldWidget.isOpen == widget.isOpen)
        {
            return;
        }
        if (widget.isOpen)
        {
            _controller.forward();
        }
        else
        {
            _controller.reverse();
        }
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => Widgets.DebugLibrary.debugCheckHasDirectionality(context));
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterialLocalizations(context));
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterialLocalizations(context));
        ThemeData theme = Theme.of(context);
        MaterialLocalizations localizations = MaterialLocalizations.of(context);
        Widget accountDetails = new CustomMultiChildLayout(@delegate: new _AccountDetailsLayout__user_accounts_drawer_header(textDirection: Directionality.of(context)), children: ((Func<List<Widget>>)(() => { var __collection4021 = new List<Widget>(); if (widget.accountName is not null) { __collection4021.Add(DartRuntimePrimitives.ConvertValue<Widget>(new LayoutId(id: _AccountDetailsLayout__user_accounts_drawer_header.accountName, child: new Padding(padding: EdgeInsets.CreateSymmetric(vertical: 2.0), child: new DefaultTextStyle(style: theme.primaryTextTheme.bodyLarge!, overflow: TextOverflow.ellipsis, child: widget.accountName!))))); } if (widget.accountEmail is not null) { __collection4021.Add(DartRuntimePrimitives.ConvertValue<Widget>(new LayoutId(id: _AccountDetailsLayout__user_accounts_drawer_header.accountEmail, child: new Padding(padding: EdgeInsets.CreateSymmetric(vertical: 2.0), child: new DefaultTextStyle(style: theme.primaryTextTheme.bodyMedium!, overflow: TextOverflow.ellipsis, child: widget.accountEmail!))))); } if (widget.onTap is not null) { __collection4021.Add(DartRuntimePrimitives.ConvertValue<Widget>(new LayoutId(id: _AccountDetailsLayout__user_accounts_drawer_header.dropdownIcon, child: new Widgets.Semantics(container: true, button: true, onTap: () => widget.onTap(), child: SizedBox.CreateSquare(dimension: User_accounts_drawer_headerLibrary._kAccountDetailsHeight, child: new Center(child: Transform.CreateRotate(angle: _animation.value * Dart_mathLibrary.pi, child: new Icon(Icons.arrow_drop_down, color: widget.arrowColor, semanticLabel: widget.isOpen ? localizations.hideAccountsLabel : localizations.showAccountsLabel)))))))); } return __collection4021; }))());
        if (widget.onTap is not null)
        {
            accountDetails = DartRuntimePrimitives.ConvertValue<Widget>(new InkWell(onTap: widget.onTap, excludeFromSemantics: true, child: accountDetails));
        }
        return new SizedBox(height: User_accounts_drawer_headerLibrary._kAccountDetailsHeight, child: accountDetails);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Scheduler.Ticker createTicker(Action<Duration> onTick)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (_ticker is null)
                {
                    return true;
                }
                throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"{GetType()} is a SingleTickerProviderStateMixin but multiple tickers were created."), new ErrorDescription("A SingleTickerProviderStateMixin can only be used as a TickerProvider once."), new ErrorHint("If a State is used for multiple AnimationController objects, or if it is passed to other " + "objects and those objects might use it more than one time in total, then instead of " + "mixing in a SingleTickerProviderStateMixin, use a regular TickerProviderStateMixin.") }));
            });
        _ticker = new Scheduler.Ticker(onTick, debugLabel: Foundation.ConstantsLibrary.kDebugMode ? $"created by {DiagnosticsLibrary.describeIdentity(this)}" : null);
        _updateTickerModeNotifier();
        _updateTicker();
        return _ticker!;
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
        TickerModeData values = _tickerModeNotifier!.value;
        if (_ticker is not null)
        {
            _ticker!.muted = !values.enabled;
            _ticker!.forceFrames = values.forceFrames;
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        ValueListenable<TickerModeData> newNotifier = TickerMode.getValuesNotifier(context);
        if (Equals(newNotifier, _tickerModeNotifier))
        {
            return;
        }
        _tickerModeNotifier?.removeListener(_updateTicker);
        newNotifier.addListener(_updateTicker);
        _tickerModeNotifier = newNotifier;
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        string? tickerDescription = (_ticker?.isActive, _ticker?.muted) switch { (true, true) => "active but muted", (true, _) => "active", (false, true) => "inactive and muted", (false, _) => "inactive", (null, _) => DartRuntimePrimitives.ConvertValue<string>(null) };
        properties.add(new DiagnosticsProperty<Scheduler.Ticker>("ticker", _ticker, description: tickerDescription, showSeparator: false, defaultValue: default));
    }

}

public static partial class User_accounts_drawer_headerLibrary
{
    internal static double _kAccountDetailsHeight = 56.0;
}

internal class _AccountDetailsLayout__user_accounts_drawer_header : MultiChildLayoutDelegate
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
        Size? iconSize = default!;
        if (hasChild(dropdownIcon))
        {
            iconSize = layoutChild(dropdownIcon, BoxConstraints.CreateLoose(size));
            positionChild(dropdownIcon, _offsetForIcon(size, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(iconSize))));
        }
        string? bottomLine = hasChild(accountEmail) ? accountEmail : (hasChild(accountName) ? accountName : null);
        if (bottomLine is not null)
        {
            var constraintSize = (iconSize is null) ? size : new Size(size.width - DartRuntimePrimitives.RequireValue(iconSize).width, size.height);
            iconSize ??= new Size(User_accounts_drawer_headerLibrary._kAccountDetailsHeight, User_accounts_drawer_headerLibrary._kAccountDetailsHeight);
            Size bottomLineSize = layoutChild(bottomLine, BoxConstraints.CreateLoose(constraintSize));
            Offset bottomLineOffset = _offsetForBottomLine(size, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(iconSize)), bottomLineSize);
            positionChild(bottomLine, bottomLineOffset);
            if ((bottomLine == accountEmail) && hasChild(accountName))
            {
                Size nameSize = layoutChild(accountName, BoxConstraints.CreateLoose(constraintSize));
                positionChild(accountName, _offsetForName(size, nameSize, bottomLineOffset));
            }
        }
    }

    public override bool shouldRelayout(MultiChildLayoutDelegate oldDelegate) => true;
    internal virtual Offset _offsetForIcon(Size size, Size iconSize)
    {
        return textDirection switch { TextDirection.ltr => new Offset(size.width - iconSize.width, size.height - iconSize.height), TextDirection.rtl => new Offset(0.0, size.height - iconSize.height), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Offset _offsetForBottomLine(Size size, Size iconSize, Size bottomLineSize)
    {
        double y = size.height - (0.5 * iconSize.height) - (0.5 * bottomLineSize.height);
        return textDirection switch { TextDirection.ltr => new Offset(0.0, y), TextDirection.rtl => new Offset(size.width - bottomLineSize.width, y), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Offset _offsetForName(Size size, Size nameSize, Offset bottomLineOffset)
    {
        double y = bottomLineOffset.dy - nameSize.height;
        return textDirection switch { TextDirection.ltr => new Offset(0.0, y), TextDirection.rtl => new Offset(size.width - nameSize.width, y), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class UserAccountsDrawerHeader : StatefulWidget
{
    public virtual Decoration? decoration { get; private set; }
    public virtual EdgeInsetsGeometry? margin { get; private set; }
    public virtual Widget? currentAccountPicture { get; private set; }
    public virtual List<Widget>? otherAccountsPictures { get; private set; }
    public virtual Size currentAccountPictureSize { get; private set; } = default!;
    public virtual Size otherAccountsPicturesSize { get; private set; } = default!;
    public virtual Widget? accountName { get; private set; }
    public virtual Widget? accountEmail { get; private set; }
    public virtual Action? onDetailsPressed { get; private set; }
    public virtual Color arrowColor { get; private set; } = default!;

    public UserAccountsDrawerHeader(Key? key = null, Decoration? decoration = null, EdgeInsetsGeometry? margin = default!, Widget? currentAccountPicture = null, List<Widget>? otherAccountsPictures = null, Size? currentAccountPictureSize = null, Size? otherAccountsPicturesSize = null, Widget? accountName = default!, Widget? accountEmail = default!, Action? onDetailsPressed = null, Color arrowColor = default!) : base(key: key)
    {
        EdgeInsetsGeometry? __margin = margin ?? EdgeInsets.CreateOnly(bottom: 8.0);
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

internal class _UserAccountsDrawerHeaderState__user_accounts_drawer_header : State<UserAccountsDrawerHeader>
{
    internal virtual bool _isOpen { get; set; } = false;

    internal virtual void _handleDetailsPressed()
    {
        setState(() =>
        {
            _isOpen = !_isOpen;
        });
        widget.onDetailsPressed!();
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterial(context));
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterialLocalizations(context));
        return new Widgets.Semantics(container: true, label: MaterialLocalizations.of(context).signedInLabel, child: new DrawerHeader(decoration: widget.decoration ?? new BoxDecoration(color: Theme.of(context).colorScheme.primary), margin: widget.margin, padding: EdgeInsetsDirectional.CreateOnly(top: 16.0, start: 16.0), child: new SafeArea(bottom: false, child: new Column(crossAxisAlignment: CrossAxisAlignment.stretch, children: new List<Widget> { DartRuntimePrimitives.ConvertValue<Widget>(new Expanded(child: new Padding(padding: EdgeInsetsDirectional.CreateOnly(end: 16.0), child: new _AccountPictures__user_accounts_drawer_header(currentAccountPicture: widget.currentAccountPicture, otherAccountsPictures: widget.otherAccountsPictures, currentAccountPictureSize: widget.currentAccountPictureSize, otherAccountsPicturesSize: widget.otherAccountsPicturesSize)))), DartRuntimePrimitives.ConvertValue<Widget>(new _AccountDetails__user_accounts_drawer_header(accountName: widget.accountName, accountEmail: widget.accountEmail, isOpen: _isOpen, onTap: (widget.onDetailsPressed is null) ? null : _handleDetailsPressed, arrowColor: widget.arrowColor)) }))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
