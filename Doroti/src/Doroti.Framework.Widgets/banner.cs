// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../flutter-master/packages/flutter/lib/src/widgets/banner.dart
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
    internal static double _kBottomOffset = (BannerLibrary._kOffset + (global::Doroti.Runtime.Dart_mathLibrary.sqrt1_2 * BannerLibrary._kHeight));
}

public static partial class BannerLibrary
{
    internal static Rect _kRect = global::Doroti.Ui.Rect.fromLTWH(-BannerLibrary._kOffset, (BannerLibrary._kOffset - BannerLibrary._kHeight), (BannerLibrary._kOffset * 2.0), BannerLibrary._kHeight);
}

public static partial class BannerLibrary
{
    internal static global::Doroti.Generated.Framework.Painting.BoxShadow _kShadow = new global::Doroti.Generated.Framework.Painting.BoxShadow(color: new global::Doroti.Ui.Color(2130706432L), blurRadius: 6.0);
}

public static partial class BannerLibrary
{
    internal static Color _kColor = new global::Doroti.Ui.Color(2696354844L);
}

public static partial class BannerLibrary
{
    internal static global::Doroti.Generated.Framework.Painting.TextStyle _kTextStyle = new global::Doroti.Generated.Framework.Painting.TextStyle(color: new global::Doroti.Ui.Color(4294967295L), fontSize: (BannerLibrary._kHeight * 0.85), fontWeight: FontWeight.w900, height: 1.0);
}

public enum BannerLocation
{
    topStart,
    topEnd,
    bottomStart,
    bottomEnd
}

public class BannerPainter : global::Doroti.Generated.Framework.Rendering.CustomPainter
{
    public virtual string message { get; private set; } = default!;
    public virtual TextDirection textDirection { get; private set; } = default!;
    public virtual BannerLocation location { get; private set; } = default!;
    public virtual TextDirection layoutDirection { get; private set; } = default!;
    public virtual Color color { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle textStyle { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.BoxShadow shadow { get; private set; } = default!;
    internal virtual bool _prepared { get; set; } = false;
    internal virtual global::Doroti.Generated.Framework.Painting.TextPainter? _textPainter { get; set; } = default;
    internal virtual Paint _paintShadow { get; set; } = default!;
    internal virtual Paint _paintBanner { get; set; } = default!;

    public BannerPainter(string message, TextDirection textDirection, BannerLocation location, TextDirection layoutDirection, Color color = default!, global::Doroti.Generated.Framework.Painting.TextStyle textStyle = default!, global::Doroti.Generated.Framework.Painting.BoxShadow shadow = default!) : base(repaint: global::Doroti.Generated.Framework.Painting.PaintingBinding.instance.systemFonts)
    {
        Color __color = color ?? BannerLibrary._kColor;
        global::Doroti.Generated.Framework.Painting.TextStyle __textStyle = textStyle ?? BannerLibrary._kTextStyle;
        global::Doroti.Generated.Framework.Painting.BoxShadow __shadow = shadow ?? BannerLibrary._kShadow;
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
        DartRuntimePrimitives.Assert(() => global::Doroti.Generated.Framework.Foundation.DebugLibrary.debugMaybeDispatchDisposed(this));
        this._textPainter?.dispose();
        _textPainter = null;
    }

    internal virtual void _prepare()
    {
        _paintShadow = this.shadow.toPaint();
        _paintBanner = ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Ui.Paint();
            __cascade.color = this.color;
            return __cascade;        }))();
        this._textPainter?.dispose();
        _textPainter = new global::Doroti.Generated.Framework.Painting.TextPainter(text: new global::Doroti.Generated.Framework.Painting.TextSpan(style: this.textStyle, text: this.message), textAlign: global::Doroti.Ui.TextAlign.center, textDirection: this.textDirection);
        _prepared = true;
    }

    public override void paint(Canvas canvas, Size size)
    {
        if (!this._prepared)
        {
            _prepare();
        }
        DartRuntimePrimitives.Ignore(((Func<Canvas>)(() =>
{            var __cascade = canvas;
            __cascade.translate(_translationX(size.width), _translationY(size.height));
            __cascade.rotate(this._rotation);
            __cascade.drawRect(BannerLibrary._kRect, this._paintShadow);
            __cascade.drawRect(BannerLibrary._kRect, this._paintBanner);
            return __cascade;        }))());
        double width__5113 = (BannerLibrary._kOffset * 2.0);
        this._textPainter!.layout(minWidth: width__5113, maxWidth: width__5113);
        this._textPainter!.paint(canvas, (BannerLibrary._kRect.topLeft + new global::Doroti.Ui.Offset(0.0, (((BannerLibrary._kRect.height - this._textPainter!.height)) / 2.0))));
    }

    public override bool shouldRepaint(global::Doroti.Generated.Framework.Rendering.CustomPainter oldDelegate)
    {
        var __oldDelegate = (BannerPainter)(object)oldDelegate;
        return ((((this.message != ((BannerPainter)__oldDelegate).message) || (!object.Equals(this.location, ((BannerPainter)__oldDelegate).location))) || (!object.Equals(this.color, ((BannerPainter)__oldDelegate).color))) || (!object.Equals(this.textStyle, ((BannerPainter)__oldDelegate).textStyle)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool? hitTest(Offset position) => false;
    internal virtual double _translationX(double width)
    {
        return ((this.layoutDirection, this.location) switch { (TextDirection.rtl, BannerLocation.topStart) => width, (TextDirection.ltr, BannerLocation.topStart) => 0.0, (TextDirection.rtl, BannerLocation.topEnd) => 0.0, (TextDirection.ltr, BannerLocation.topEnd) => width, (TextDirection.rtl, BannerLocation.bottomStart) => (width - BannerLibrary._kBottomOffset), (TextDirection.ltr, BannerLocation.bottomStart) => BannerLibrary._kBottomOffset, (TextDirection.rtl, BannerLocation.bottomEnd) => BannerLibrary._kBottomOffset, (TextDirection.ltr, BannerLocation.bottomEnd) => (width - BannerLibrary._kBottomOffset), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _translationY(double height)
    {
        return (this.location switch { BannerLocation.bottomStart => (height - BannerLibrary._kBottomOffset), BannerLocation.bottomEnd => (height - BannerLibrary._kBottomOffset), BannerLocation.topStart => 0.0, BannerLocation.topEnd => 0.0, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _rotation
    {
        get
        {
            return ((Dart_mathLibrary.pi / 4.0) * ((this.layoutDirection, this.location) switch { (TextDirection.rtl, BannerLocation.topStart or BannerLocation.bottomEnd) => 1L, (TextDirection.ltr, BannerLocation.topStart or BannerLocation.bottomEnd) => -1L, (TextDirection.rtl, BannerLocation.bottomStart or BannerLocation.topEnd) => -1L, (TextDirection.ltr, BannerLocation.bottomStart or BannerLocation.topEnd) => 1L, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
            return default!;
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
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle textStyle { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.BoxShadow shadow { get; private set; } = default!;

    public Banner(global::Doroti.Generated.Framework.Foundation.Key? key = null, Widget? child = null, string message = default!, TextDirection? textDirection = null, BannerLocation location = default!, TextDirection? layoutDirection = null, Color color = default!, global::Doroti.Generated.Framework.Painting.TextStyle textStyle = default!, global::Doroti.Generated.Framework.Painting.BoxShadow shadow = default!) : base(key: key)
    {
        Color __color = color ?? BannerLibrary._kColor;
        global::Doroti.Generated.Framework.Painting.TextStyle __textStyle = textStyle ?? BannerLibrary._kTextStyle;
        global::Doroti.Generated.Framework.Painting.BoxShadow __shadow = shadow ?? BannerLibrary._kShadow;
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
        this._painter?.dispose();
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => ((((((Banner)this.widget).textDirection is not null) && (((Banner)this.widget).layoutDirection is not null))) || global::Doroti.Generated.Framework.Widgets.DebugLibrary.debugCheckHasDirectionality(context)));
        this._painter?.dispose();
        _painter = new BannerPainter(message: ((Banner)this.widget).message, textDirection: ((((Banner)this.widget).textDirection ?? (TextDirection)Directionality.of(context))), location: ((Banner)this.widget).location, layoutDirection: ((((Banner)this.widget).layoutDirection ?? (TextDirection)Directionality.of(context))), color: ((Banner)this.widget).color, textStyle: ((Banner)this.widget).textStyle, shadow: ((Banner)this.widget).shadow);
        return ((Widget)(object?)new CustomPaint(foregroundPainter: this._painter, child: ((Banner)this.widget).child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.StringProperty("message", ((Banner)this.widget).message, showName: false));
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<global::Doroti.Ui.TextDirection>("textDirection", ((Banner)this.widget).textDirection, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<BannerLocation>("location", ((Banner)this.widget).location));
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<global::Doroti.Ui.TextDirection>("layoutDirection", ((Banner)this.widget).layoutDirection, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("color", ((Banner)this.widget).color, showName: false));
        ((Banner)this.widget).textStyle.debugFillProperties(properties, prefix: "text ");
    }

}

public class CheckedModeBanner : StatelessWidget
{
    public virtual Widget child { get; private set; } = default!;

    public CheckedModeBanner(global::Doroti.Generated.Framework.Foundation.Key? key = null, Widget child = default!) : base(key: key)
    {
        this.child = child;
    }

    public override Widget build(BuildContext context)
    {
        Widget result__11273 = this.child;
        DartRuntimePrimitives.Assert(() =>
            {
                result__11273 = DartRuntimePrimitives.ConvertValue<Widget>(new Banner(message: "DEBUG", textDirection: TextDirection.ltr, location: BannerLocation.topEnd, child: result__11273));
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return result__11273;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        var message__11654 = "disabled";
        DartRuntimePrimitives.Assert(() =>
            {
                message__11654 = "\"DEBUG\"";
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        properties.add(global::Doroti.Generated.Framework.Foundation.DiagnosticsNode.CreateMessage(message__11654));
    }

}

