// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/banner.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public static partial class BannerLibrary
{
    internal static double _kOffset = 40.0;
}

public static partial class BannerLibrary
{
    internal static double _kHeight = 12.0;
}

public static partial class BannerLibrary
{
    internal static double _kBottomOffset = _kOffset + (Dart_mathLibrary.sqrt1_2 * _kHeight);
}

public static partial class BannerLibrary
{
    internal static Rect _kRect = Rect.fromLTWH(-_kOffset, _kOffset - _kHeight, _kOffset * 2.0, _kHeight);
}

public static partial class BannerLibrary
{
    internal static global::Doroti.Framework.Painting.BoxShadow _kShadow = new global::Doroti.Framework.Painting.BoxShadow(color: new global::Doroti.Ui.Color(2130706432L), blurRadius: 6.0);
}

public static partial class BannerLibrary
{
    internal static Color _kColor = new global::Doroti.Ui.Color(2696354844L);
}

public static partial class BannerLibrary
{
    internal static global::Doroti.Framework.Painting.TextStyle _kTextStyle = new global::Doroti.Framework.Painting.TextStyle(color: new global::Doroti.Ui.Color(4294967295L), fontSize: _kHeight * 0.85, fontWeight: FontWeight.w900, height: 1.0);
}

public enum BannerLocation
{
    topStart,
    topEnd,
    bottomStart,
    bottomEnd
}

public class BannerPainter : global::Doroti.Framework.Rendering.CustomPainter
{
    public virtual string message { get; private set; } = default!;
    public virtual TextDirection textDirection { get; private set; } = default!;
    public virtual BannerLocation location { get; private set; } = default!;
    public virtual TextDirection layoutDirection { get; private set; } = default!;
    public virtual Color color { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.TextStyle textStyle { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.BoxShadow shadow { get; private set; } = default!;
    internal virtual bool _prepared { get; set; } = false;
    internal virtual global::Doroti.Framework.Painting.TextPainter? _textPainter { get; set; } = default;
    internal virtual Paint _paintShadow { get; set; } = default!;
    internal virtual Paint _paintBanner { get; set; } = default!;

    public BannerPainter(string message, TextDirection textDirection, BannerLocation location, TextDirection layoutDirection, Color color = default!, global::Doroti.Framework.Painting.TextStyle textStyle = default!, global::Doroti.Framework.Painting.BoxShadow shadow = default!) : base(repaint: PaintingBinding.instance.systemFonts)
    {
        Color __color = color ?? BannerLibrary._kColor;
        global::Doroti.Framework.Painting.TextStyle __textStyle = textStyle ?? BannerLibrary._kTextStyle;
        global::Doroti.Framework.Painting.BoxShadow __shadow = shadow ?? BannerLibrary._kShadow;
        this.message = message;
        this.textDirection = textDirection;
        this.location = location;
        this.layoutDirection = layoutDirection;
        this.color = __color;
        this.textStyle = __textStyle;
        this.shadow = __shadow;
    }

    public virtual void dispose()
    {
        DartRuntimePrimitives.Assert(() => Foundation.DebugLibrary.debugMaybeDispatchDisposed(this));
        _textPainter?.dispose();
        _textPainter = null;
    }

    internal virtual void _prepare()
    {
        _paintShadow = shadow.toPaint();
        _paintBanner = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = color;
    return __cascade;
}))();
        _textPainter?.dispose();
        _textPainter = new global::Doroti.Framework.Painting.TextPainter(text: new global::Doroti.Framework.Painting.TextSpan(style: textStyle, text: message), textAlign: TextAlign.center, textDirection: textDirection);
        _prepared = true;
    }

    public override void paint(Canvas canvas, Size size)
    {
        if (!_prepared)
        {
            _prepare();
        }
        DartRuntimePrimitives.Ignore(((Func<Canvas>)(() =>
{
    var __cascade = canvas;
    __cascade.translate(_translationX(size.width), _translationY(size.height));
    __cascade.rotate(_rotation);
    __cascade.drawRect(BannerLibrary._kRect, _paintShadow);
    __cascade.drawRect(BannerLibrary._kRect, _paintBanner);
    return __cascade;
}))());
        double widthLocal = BannerLibrary._kOffset * 2.0;
        _textPainter!.layout(minWidth: widthLocal, maxWidth: widthLocal);
        _textPainter!.paint(canvas, BannerLibrary._kRect.topLeft + new global::Doroti.Ui.Offset(0.0, (BannerLibrary._kRect.height - _textPainter!.height) / 2.0));
    }

    public override bool shouldRepaint(global::Doroti.Framework.Rendering.CustomPainter oldDelegate)
    {
        var __oldDelegate = (BannerPainter)oldDelegate;
        return (message != __oldDelegate.message) || (!Equals(location, __oldDelegate.location)) || (!Equals(color, __oldDelegate.color)) || (!Equals(textStyle, __oldDelegate.textStyle));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool? hitTest(Offset position) => false;
    internal virtual double _translationX(double width)
    {
        return (layoutDirection, location) switch { (TextDirection.rtl, BannerLocation.topStart) => width, (TextDirection.ltr, BannerLocation.topStart) => 0.0, (TextDirection.rtl, BannerLocation.topEnd) => 0.0, (TextDirection.ltr, BannerLocation.topEnd) => width, (TextDirection.rtl, BannerLocation.bottomStart) => width - BannerLibrary._kBottomOffset, (TextDirection.ltr, BannerLocation.bottomStart) => BannerLibrary._kBottomOffset, (TextDirection.rtl, BannerLocation.bottomEnd) => BannerLibrary._kBottomOffset, (TextDirection.ltr, BannerLocation.bottomEnd) => width - BannerLibrary._kBottomOffset, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _translationY(double height)
    {
        return location switch { BannerLocation.bottomStart => height - BannerLibrary._kBottomOffset, BannerLocation.bottomEnd => height - BannerLibrary._kBottomOffset, BannerLocation.topStart => 0.0, BannerLocation.topEnd => 0.0, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _rotation
    {
        get
        {
            return Dart_mathLibrary.pi / 4.0 * ((layoutDirection, location) switch { (TextDirection.rtl, BannerLocation.topStart or BannerLocation.bottomEnd) => 1L, (TextDirection.ltr, BannerLocation.topStart or BannerLocation.bottomEnd) => -1L, (TextDirection.rtl, BannerLocation.bottomStart or BannerLocation.topEnd) => -1L, (TextDirection.ltr, BannerLocation.bottomStart or BannerLocation.topEnd) => 1L, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        }
    }
}

public class Banner : StatefulWidget
{
    public virtual Widget? child { get; private set; }
    public virtual string message { get; private set; } = default!;
    public virtual TextDirection? textDirection { get; private set; }
    public virtual BannerLocation location { get; private set; } = default!;
    public virtual TextDirection? layoutDirection { get; private set; }
    public virtual Color color { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.TextStyle textStyle { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.BoxShadow shadow { get; private set; } = default!;

    public Banner(global::Doroti.Framework.Foundation.Key? key = null, Widget? child = null, string message = default!, TextDirection? textDirection = null, BannerLocation location = default!, TextDirection? layoutDirection = null, Color color = default!, global::Doroti.Framework.Painting.TextStyle textStyle = default!, global::Doroti.Framework.Painting.BoxShadow shadow = default!) : base(key: key)
    {
        Color __color = color ?? BannerLibrary._kColor;
        global::Doroti.Framework.Painting.TextStyle __textStyle = textStyle ?? BannerLibrary._kTextStyle;
        global::Doroti.Framework.Painting.BoxShadow __shadow = shadow ?? BannerLibrary._kShadow;
        this.child = child;
        this.message = message;
        this.textDirection = textDirection;
        this.location = location;
        this.layoutDirection = layoutDirection;
        this.color = __color;
        this.textStyle = __textStyle;
        this.shadow = __shadow;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _BannerState__banner());
}

internal class _BannerState__banner : State<Banner>
{
    internal virtual BannerPainter? _painter { get; set; } = default;

    public override void dispose()
    {
        _painter?.dispose();
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => (widget.textDirection is not null) && (widget.layoutDirection is not null) || DebugLibrary.debugCheckHasDirectionality(context));
        _painter?.dispose();
        _painter = new BannerPainter(message: widget.message, textDirection: widget.textDirection ?? Directionality.of(context), location: widget.location, layoutDirection: widget.layoutDirection ?? Directionality.of(context), color: widget.color, textStyle: widget.textStyle, shadow: widget.shadow);
        return new CustomPaint(foregroundPainter: _painter, child: widget.child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.StringProperty("message", widget.message, showName: false));
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<global::Doroti.Ui.TextDirection>("textDirection", widget.textDirection, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<BannerLocation>("location", widget.location));
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<global::Doroti.Ui.TextDirection>("layoutDirection", widget.layoutDirection, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("color", widget.color, showName: false));
        widget.textStyle.debugFillProperties(properties, prefix: "text ");
    }

}

public class CheckedModeBanner : StatelessWidget
{
    public virtual Widget child { get; private set; } = default!;

    public CheckedModeBanner(global::Doroti.Framework.Foundation.Key? key = null, Widget child = default!) : base(key: key)
    {
        this.child = child;
    }

    public override Widget build(BuildContext context)
    {
        Widget result = child;
        DartRuntimePrimitives.Assert(() =>
            {
                result = DartRuntimePrimitives.ConvertValue<Widget>(new Banner(message: "DEBUG", textDirection: TextDirection.ltr, location: BannerLocation.topEnd, child: result));
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        var message = "disabled";
        DartRuntimePrimitives.Assert(() =>
            {
                message = "\"DEBUG\"";
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        properties.add(DiagnosticsNode.CreateMessage(message));
    }

}

