// Copyright 2024 The Flutter team. All rights reserved.
// Adapted from scheme.dart and color_box.dart; BSD license in LICENSE.flutter.
using Doroti.Framework.Painting;
using Doroti.Framework.Rendering;
using Doroti.Framework.Services;
using Doroti.Framework.Widgets;
using Doroti.Ui;
using M = Doroti.Framework.Material;

namespace MaterialSample;

internal sealed class SchemePreview(string label, M.ColorScheme scheme) : StatelessWidget
{
    public override Widget build(BuildContext context)
    {
        Widget Box(string name, Color color, Color on, double height, double width) => new ColorBox(name, color, on, height, width);
        Widget RowOf(params Widget[] children) => new Row(mainAxisSize: MainAxisSize.min, crossAxisAlignment: CrossAxisAlignment.start, children: children.ToList());
        Widget ColumnOf(params Widget[] children) => new Column(mainAxisSize: MainAxisSize.min, children: children.ToList());
        Widget Quartet(string name, Color color, Color on, Color container, Color onContainer) => ColumnOf(
            Box(name, color, on, 87, 208), Box("On " + name, on, color, 40, 208), new SizedBox(height: 5),
            Box(name + " Container", container, onContainer, 87, 208), Box("On " + name + " Container", onContainer, container, 40, 208));
        var theme = M.Theme.of(context);
        return new M.Theme(data: theme.copyWith(colorScheme: scheme), child: new FittedBox(fit: BoxFit.fitWidth, child: new Container(width: 902,
            padding: EdgeInsets.CreateOnly(top: 16, left: 16, right: 16), decoration: new BoxDecoration(color: scheme.surface,
                borderRadius: BorderRadius.CreateCircular(12), border: Border.CreateAll(color: theme.brightness == scheme.brightness ? theme.colorScheme.outlineVariant : M.Colors.transparent)),
            child: new Column(mainAxisSize: MainAxisSize.min, crossAxisAlignment: CrossAxisAlignment.start, children:
            [
                new Text(label, style: theme.textTheme.titleMedium!.copyWith(color: scheme.onSurface, fontWeight: FontWeight.bold)), new SizedBox(height: 20),
                RowOf(ColumnOf(
                    RowOf(Quartet("Primary", scheme.primary, scheme.onPrimary, scheme.primaryContainer, scheme.onPrimaryContainer), new SizedBox(width: 5),
                        Quartet("Secondary", scheme.secondary, scheme.onSecondary, scheme.secondaryContainer, scheme.onSecondaryContainer), new SizedBox(width: 5),
                        Quartet("Tertiary", scheme.tertiary, scheme.onTertiary, scheme.tertiaryContainer, scheme.onTertiaryContainer)),
                    new SizedBox(height: 20),
                    RowOf(Box("Surface Dim", scheme.surfaceDim, scheme.onSurface, 105, 211.45), Box("Surface", scheme.surface, scheme.onSurface, 105, 211.45), Box("Surface Bright", scheme.surfaceBright, scheme.onSurface, 105, 211.45)),
                    new SizedBox(height: 5),
                    RowOf(Box("Surf. Container\nLowest", scheme.surfaceContainerLowest, scheme.onSurface, 105, 126.87), Box("Surf. Container\nLow", scheme.surfaceContainerLow, scheme.onSurface, 105, 126.87),
                        Box("Surf. Container", scheme.surfaceContainer, scheme.onSurface, 105, 126.87), Box("Surf. Container\nHigh", scheme.surfaceContainerHigh, scheme.onSurface, 105, 126.87), Box("Surf. Container\nHighest", scheme.surfaceContainerHighest, scheme.onSurface, 105, 126.87)),
                    new SizedBox(height: 5),
                    RowOf(Box("On Surface", scheme.onSurface, scheme.surface, 40, 158.59), Box("On Surface Var.", scheme.onSurfaceVariant, scheme.surfaceContainerHighest, 40, 158.59),
                        Box("Outline", scheme.outline, scheme.surface, 40, 158.59), Box("Outline Variant", scheme.outlineVariant, scheme.onSurface, 40, 158.59))),
                    new SizedBox(width: 20),
                    ColumnOf(Quartet("Error", scheme.error, scheme.onError, scheme.errorContainer, scheme.onErrorContainer), new SizedBox(height: 20),
                        Box("Inverse Surface", scheme.inverseSurface, scheme.onInverseSurface, 120, 208), Box("Inverse On Surface", scheme.onInverseSurface, scheme.inverseSurface, 40, 208), new SizedBox(height: 5),
                        Box("Inverse Primary", scheme.inversePrimary, scheme.onSurface, 40, 208), new SizedBox(height: 16),
                        RowOf(Box("Scrim", scheme.scrim, M.Colors.white, 40, 96.31), new SizedBox(width: 20), Box("Shadow", scheme.shadow, M.Colors.white, 40, 96.31)), new SizedBox(height: 8))),
            ]))));
    }
}

internal sealed class ColorBox(string label, Color color, Color onColor, double height, double width) : StatefulWidget
{
    internal string Label => label;
    internal Color Color => color;
    internal Color OnColor => onColor;
    internal double Height => height;
    internal double Width => width;
    public override IState createState() => new ColorBoxState();
}
internal sealed class ColorBoxState : State<ColorBox>
{
    private bool _hovered;
    private async void Copy()
    {
        var hex = $"#{widget.Color.red:x2}{widget.Color.green:x2}{widget.Color.blue:x2}";
        await Clipboard.setData(new ClipboardData(hex));
        if (!mounted) return;
        var messenger = M.ScaffoldMessenger.of(context);
        messenger.hideCurrentSnackBar(); messenger.showSnackBar(new M.SnackBar(content: new Text($"Copied {hex} to clipboard")));
    }
    public override Widget build(BuildContext context) => new MouseRegion(onEnter: _ => setState(() => _hovered = true), onExit: _ => setState(() => _hovered = false),
        child: new Container(color: widget.Color, height: widget.Height, width: widget.Width, child: new DefaultTextStyle(style: M.Theme.of(context).textTheme.labelSmall!.copyWith(color: widget.OnColor),
            child: new Stack(children: [new Positioned(top: 10, left: 10, child: new Text(widget.Label)),
                _hovered ? new Positioned(top: 0, right: 0, child: new M.IconButton(padding: EdgeInsets.zero, color: widget.OnColor, tooltip: "Copy hex color", icon: new Icon(M.Icons.copy, size: 24), onPressed: Copy)) : SizedBox.CreateShrink()]))));
}
