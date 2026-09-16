// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/text_selection_toolbar_button.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Cupertino;

public static partial class Text_selection_toolbar_buttonLibrary
{
    internal static global::Doroti.Framework.Painting.TextStyle _kToolbarButtonFontStyle = new global::Doroti.Framework.Painting.TextStyle(inherit: false, fontSize: 15.0, letterSpacing: -0.15, fontWeight: FontWeight.w400);
}

public static partial class Text_selection_toolbar_buttonLibrary
{
    internal static CupertinoDynamicColor _kToolbarTextColor = new CupertinoDynamicColor(color: CupertinoColors.black, darkColor: CupertinoColors.white);
}

public static partial class Text_selection_toolbar_buttonLibrary
{
    internal static CupertinoDynamicColor _kToolbarPressedColor = new CupertinoDynamicColor(color: new global::Doroti.Ui.Color(268435456L), darkColor: new global::Doroti.Ui.Color(285212671L));
}

public static partial class Text_selection_toolbar_buttonLibrary
{
    internal static global::Doroti.Framework.Painting.EdgeInsets _kToolbarButtonPadding = EdgeInsets.CreateSymmetric(vertical: 18.0, horizontal: 16.0);
}

public class CupertinoTextSelectionToolbarButton : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual global::Doroti.Framework.Widgets.Widget? child { get; private set; }
    public virtual global::System.Action? onPressed { get; private set; }
    public virtual global::Doroti.Framework.Widgets.ContextMenuButtonItem? buttonItem { get; private set; }
    public virtual string? text { get; private set; }

    public CupertinoTextSelectionToolbarButton(global::Doroti.Framework.Foundation.Key? key = null, global::System.Action? onPressed = null, global::Doroti.Framework.Widgets.Widget child = default!) : base(key: key)
    {
        this.onPressed = onPressed;
        this.child = child;
        text = null;
        buttonItem = null;
    }

    public static CupertinoTextSelectionToolbarButton CreateText(global::Doroti.Framework.Foundation.Key? key = null, global::System.Action? onPressed = null, string? text = default!)
    {
        var __instance = new CupertinoTextSelectionToolbarButton(key: key, onPressed: onPressed, child: default!);
        __instance.onPressed = onPressed;
        __instance.text = text;
        __instance.buttonItem = null;
        __instance.child = null;
        return __instance;
    }

    public static CupertinoTextSelectionToolbarButton CreateButtonItem(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Widgets.ContextMenuButtonItem buttonItem = default!)
    {
        var __instance = new CupertinoTextSelectionToolbarButton(key: key, child: default!);
        __instance.buttonItem = buttonItem;
        __instance.child = null;
        __instance.text = null;
        __instance.onPressed = buttonItem.onPressed;
        return __instance;
    }

    public static string getButtonLabel(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.ContextMenuButtonItem buttonItem)
    {
        if (buttonItem.label is not null)
        {
            return buttonItem.label!;
        }
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasCupertinoLocalizations(context));
        CupertinoLocalizations localizations = CupertinoLocalizations.of(context);
        return buttonItem.type switch { ContextMenuButtonType.cut => localizations.cutButtonLabel, ContextMenuButtonType.copy => localizations.copyButtonLabel, ContextMenuButtonType.paste => localizations.pasteButtonLabel, ContextMenuButtonType.selectAll => localizations.selectAllButtonLabel, ContextMenuButtonType.lookUp => localizations.lookUpButtonLabel, ContextMenuButtonType.searchWeb => localizations.searchWebButtonLabel, ContextMenuButtonType.share => localizations.shareButtonLabel, ContextMenuButtonType.liveTextInput or ContextMenuButtonType.delete => "", ContextMenuButtonType.custom => "", _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoTextSelectionToolbarButtonState__text_selection_toolbar_button());
}

internal class _CupertinoTextSelectionToolbarButtonState__text_selection_toolbar_button : global::Doroti.Framework.Widgets.State<CupertinoTextSelectionToolbarButton>
{
    public virtual bool isPressed { get; set; } = false;

    internal virtual void _onTapDown(global::Doroti.Framework.Gestures.TapDownDetails details)
    {
        setState(() => { _ = isPressed = true; });
    }

    internal virtual void _onTapUp(global::Doroti.Framework.Gestures.TapUpDetails details)
    {
        setState(() => { _ = isPressed = false; });
        widget.onPressed?.Invoke();
    }

    internal virtual void _onTapCancel()
    {
        setState(() => { _ = isPressed = false; });
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Framework.Widgets.Widget content = _getContentWidget(context);
        global::Doroti.Framework.Widgets.Widget childLocal = new CupertinoButton(color: isPressed ? Text_selection_toolbar_buttonLibrary._kToolbarPressedColor.resolveFrom(context) : CupertinoColors.transparent, disabledColor: CupertinoColors.transparent, onPressed: widget.onPressed, padding: Text_selection_toolbar_buttonLibrary._kToolbarButtonPadding, pressedOpacity: 1.0, child: content);
        if (widget.onPressed is not null)
        {
            return new global::Doroti.Framework.Widgets.GestureDetector(onTapDown: _onTapDown, onTapUp: _onTapUp, onTapCancel: () => _onTapCancel(), child: childLocal);
        }
        else
        {
            return childLocal;
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _getContentWidget(global::Doroti.Framework.Widgets.BuildContext context)
    {
        if (widget.child is not null)
        {
            return widget.child!;
        }
        global::Doroti.Framework.Widgets.Widget textWidget = new global::Doroti.Framework.Widgets.Text(widget.text ?? CupertinoTextSelectionToolbarButton.getButtonLabel(context, widget.buttonItem!), overflow: TextOverflow.ellipsis, style: Text_selection_toolbar_buttonLibrary._kToolbarButtonFontStyle.copyWith(color: (widget.onPressed is not null) ? Text_selection_toolbar_buttonLibrary._kToolbarTextColor.resolveFrom(context) : CupertinoColors.inactiveGray));
        switch (widget.buttonItem?.type)
        {
            case ContextMenuButtonType.cut:
            case ContextMenuButtonType.copy:
            case ContextMenuButtonType.paste:
            case ContextMenuButtonType.selectAll:
            case ContextMenuButtonType.delete:
            case ContextMenuButtonType.lookUp:
            case ContextMenuButtonType.searchWeb:
            case ContextMenuButtonType.share:
            case ContextMenuButtonType.custom:
            case null:
                {
                    return textWidget;
                }
            case ContextMenuButtonType.liveTextInput:
                {
                    return SizedBox.CreateSquare(dimension: 13.0, child: new global::Doroti.Framework.Widgets.CustomPaint(painter: new _LiveTextIconPainter__text_selection_toolbar_button(color: Text_selection_toolbar_buttonLibrary._kToolbarTextColor.resolveFrom(context))));
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _LiveTextIconPainter__text_selection_toolbar_button : global::Doroti.Framework.Rendering.CustomPainter
{
    public virtual Color color { get; private set; } = default!;
    internal virtual Paint _painter { get; private set; } = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.strokeCap = StrokeCap.round;
    __cascade.strokeJoin = StrokeJoin.round;
    __cascade.strokeWidth = 1.0;
    __cascade.style = PaintingStyle.stroke;
    return __cascade;
}))();

    internal _LiveTextIconPainter__text_selection_toolbar_button(Color color)
    {
        this.color = color;
    }

    public override void paint(Canvas canvas, Size size)
    {
        _painter.color = color;
        canvas.save();
        canvas.translate(size.width / 2.0, size.height / 2.0);
        var origin = new global::Doroti.Ui.Offset(-size.width / 2.0, -size.height / 2.0);
        var path = ((Func<Path>)(() =>
{
    var __cascade = new global::Doroti.Ui.Path();
    __cascade.moveTo(origin.dx, origin.dy + 3.5);
    __cascade.lineTo(origin.dx, origin.dy + 1.0);
    __cascade.arcToPoint(new global::Doroti.Ui.Offset(origin.dx + 1.0, origin.dy), radius: Radius.circular(1));
    __cascade.lineTo(origin.dx + 3.5, origin.dy);
    return __cascade;
}))();
        var rotationMatrix = ((Func<Matrix4>)(() =>
{
    var __cascade = Matrix4.identity();
    __cascade.rotateZ(Dart_mathLibrary.pi / 2.0);
    return __cascade;
}))();
        for (var i = 0L; i < 4L; i += 1L)
        {
            canvas.drawPath(path, _painter);
            canvas.transform(rotationMatrix.storage);
        }
        canvas.drawLine(new global::Doroti.Ui.Offset(-3.0, -3.0), new global::Doroti.Ui.Offset(3.0, -3.0), _painter);
        canvas.drawLine(new global::Doroti.Ui.Offset(-3.0, 0.0), new global::Doroti.Ui.Offset(3.0, 0.0), _painter);
        canvas.drawLine(new global::Doroti.Ui.Offset(-3.0, 3.0), new global::Doroti.Ui.Offset(1.0, 3.0), _painter);
        canvas.restore();
    }

    public override bool shouldRepaint(global::Doroti.Framework.Rendering.CustomPainter oldDelegate)
    {
        var __oldDelegate = (_LiveTextIconPainter__text_selection_toolbar_button)oldDelegate;
        return !Equals(__oldDelegate.color, color);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
