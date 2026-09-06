// Copyright 2021, 2024 The Flutter team. All rights reserved.
// Adapted from reference/flutter_sample_app; BSD license in LICENSE.flutter.
using Doroti.Framework.Painting;
using Doroti.Framework.Rendering;
using Doroti.Framework.Widgets;
using Doroti.Ui;
using M = Doroti.Framework.Material;
using TextStyle = Doroti.Framework.Painting.TextStyle;

namespace MaterialSample;

internal sealed class TypographyScreen : StatelessWidget
{
    public override Widget build(BuildContext context)
    {
        var theme = M.Theme.of(context);
        var t = theme.textTheme;
        (string Name, TextStyle? Style)[] styles =
        [
            ("Display Large", t.displayLarge), ("Display Medium", t.displayMedium), ("Display Small", t.displaySmall),
            ("Headline Large", t.headlineLarge), ("Headline Medium", t.headlineMedium), ("Headline Small", t.headlineSmall),
            ("Title Large", t.titleLarge), ("Title Medium", t.titleMedium), ("Title Small", t.titleSmall),
            ("Label Large", t.labelLarge), ("Label Medium", t.labelMedium), ("Label Small", t.labelSmall),
            ("Body Large", t.bodyLarge), ("Body Medium", t.bodyMedium), ("Body Small", t.bodySmall),
        ];
        return new ListView(children: [new SizedBox(height: 8), .. styles.Select(s => (Widget)new Padding(padding: EdgeInsets.CreateAll(8),
            child: new Text(s.Name, style: s.Style!.copyWith(color: theme.colorScheme.onSurface))))]);
    }
}

internal sealed class ElevationScreen : StatelessWidget
{
    public override Widget build(BuildContext context)
    {
        var theme = M.Theme.of(context);
        double[] elevations = [0, 1, 3, 6, 8, 12];
        int[] overlays = [0, 5, 8, 11, 12, 14];
        string[] labels = ["Surface Tint Color Only", "Surface Tint Color and Shadow Color", "Shadow Color Only"];
        var slivers = new List<Widget>();
        for (var group = 0; group < 3; group++)
        {
            var mode = group;
            slivers.Add(new SliverToBoxAdapter(child: new Padding(padding: EdgeInsets.CreateFromLTRB(16, group == 0 ? 20 : 18, 16, 0), child: new Text(labels[group], style: theme.textTheme.titleLarge))));
            slivers.Add(new SliverPadding(padding: EdgeInsets.CreateAll(8), sliver: new SliverLayoutBuilder(builder: (_, constraints) =>
                SliverGrid.CreateCount(crossAxisCount: constraints.crossAxisExtent < 450 ? 3 : 6, children:
                    elevations.Select((elevation, index) => (Widget)new Padding(padding: EdgeInsets.CreateAll(8), child: new M.Material(
                        borderRadius: BorderRadius.CreateCircular(4), elevation: elevation, color: theme.colorScheme.surface,
                        shadowColor: mode == 0 ? M.Colors.transparent : theme.colorScheme.shadow,
                        surfaceTintColor: mode == 2 ? null : theme.colorScheme.primary, type: M.MaterialType.card,
                        child: new Padding(padding: EdgeInsets.CreateAll(8), child: new Column(crossAxisAlignment: CrossAxisAlignment.start,
                            children: [new Text($"Level {index}", style: theme.textTheme.labelMedium), new Text($"{elevation:0} dp", style: theme.textTheme.labelMedium),
                                mode == 2 ? SizedBox.CreateShrink() : new Expanded(child: new Align(alignment: Alignment.bottomRight,
                                    child: new Text($"{overlays[index]}%", style: theme.textTheme.bodySmall)))]))))).ToList()))));
        }
        return new CustomScrollView(slivers: slivers);
    }
}

internal sealed class ColorScreen : StatelessWidget
{
    private static (string Label, Color Color, Color On)[][] Groups(M.ColorScheme c) =>
    [
        [("primary", c.primary, c.onPrimary), ("onPrimary", c.onPrimary, c.primary), ("primaryContainer", c.primaryContainer, c.onPrimaryContainer), ("onPrimaryContainer", c.onPrimaryContainer, c.primaryContainer)],
        [("primaryFixed", c.primaryFixed, c.onPrimaryFixed), ("onPrimaryFixed", c.onPrimaryFixed, c.primaryFixed), ("primaryFixedDim", c.primaryFixedDim, c.onPrimaryFixedVariant), ("onPrimaryFixedVariant", c.onPrimaryFixedVariant, c.primaryFixedDim)],
        [("secondary", c.secondary, c.onSecondary), ("onSecondary", c.onSecondary, c.secondary), ("secondaryContainer", c.secondaryContainer, c.onSecondaryContainer), ("onSecondaryContainer", c.onSecondaryContainer, c.secondaryContainer)],
        [("secondaryFixed", c.secondaryFixed, c.onSecondaryFixed), ("onSecondaryFixed", c.onSecondaryFixed, c.secondaryFixed), ("secondaryFixedDim", c.secondaryFixedDim, c.onSecondaryFixedVariant), ("onSecondaryFixedVariant", c.onSecondaryFixedVariant, c.secondaryFixedDim)],
        [("tertiary", c.tertiary, c.onTertiary), ("onTertiary", c.onTertiary, c.tertiary), ("tertiaryContainer", c.tertiaryContainer, c.onTertiaryContainer), ("onTertiaryContainer", c.onTertiaryContainer, c.tertiaryContainer)],
        [("tertiaryFixed", c.tertiaryFixed, c.onTertiaryFixed), ("onTertiaryFixed", c.onTertiaryFixed, c.tertiaryFixed), ("tertiaryFixedDim", c.tertiaryFixedDim, c.onTertiaryFixedVariant), ("onTertiaryFixedVariant", c.onTertiaryFixedVariant, c.tertiaryFixedDim)],
        [("error", c.error, c.onError), ("onError", c.onError, c.error), ("errorContainer", c.errorContainer, c.onErrorContainer), ("onErrorContainer", c.onErrorContainer, c.errorContainer)],
        [("surfaceDim", c.surfaceDim, c.onSurface), ("surface", c.surface, c.onSurface), ("surfaceBright", c.surfaceBright, c.onSurface),
         ("surfaceContainerLowest", c.surfaceContainerLowest, c.onSurface), ("surfaceContainerLow", c.surfaceContainerLow, c.onSurface),
         ("surfaceContainer", c.surfaceContainer, c.onSurface), ("surfaceContainerHigh", c.surfaceContainerHigh, c.onSurface), ("surfaceContainerHighest", c.surfaceContainerHighest, c.onSurface),
         ("onSurface", c.onSurface, c.surface), ("onSurfaceVariant", c.onSurfaceVariant, c.surfaceContainerHighest)],
        [("outline", c.outline, Contrast(c.outline)), ("shadow", c.shadow, Contrast(c.shadow)), ("inverseSurface", c.inverseSurface, c.onInverseSurface),
         ("onInverseSurface", c.onInverseSurface, c.inverseSurface), ("inversePrimary", c.inversePrimary, c.primary)],
    ];
    private static Color Contrast(Color color) => M.ThemeData.estimateBrightnessForColor(color) == Brightness.dark ? M.Colors.white : M.Colors.black;
    public override Widget build(BuildContext context)
    {
        var seed = M.Theme.of(context).colorScheme.primary;
        return new LayoutBuilder(builder: (_, constraints) =>
        {
            var wide = constraints.maxWidth >= 500;
            var children = new List<Widget>();
            if (!wide) children.Add(new M.TextButton(onPressed: () => Launch(context), child: new Text("Create platform dynamic color schemes with the dynamic_color package.")));
            foreach (var brightness in new[] { Brightness.light, Brightness.dark })
            {
                var scheme = M.ColorScheme.CreateFromSeed(seedColor: seed, brightness: brightness);
                var groups = Groups(scheme);
                var label = brightness == Brightness.light ? "Light ColorScheme" : "Dark ColorScheme";
                Widget view = wide ? new SchemePreview(label, scheme) : new Column(children: [
                    new Padding(padding: EdgeInsets.CreateSymmetric(vertical: 15), child: new Text(label)),
                    .. groups.Select(group => (Widget)new Padding(padding: EdgeInsets.CreateSymmetric(horizontal: 15, vertical: 5),
                        child: new M.Card(clipBehavior: Clip.antiAlias, child: new Column(children: group.Select(role => Chip(role, null)).ToList()))))]);
                children.Add(view); children.Add(new SizedBox(height: 16));
            }
            return new SingleChildScrollView(child: new Padding(padding: EdgeInsets.CreateAll(wide ? 8 : 0), child: new Column(children: children)));
        });
    }
    private static Widget Chip((string Label, Color Color, Color On) role, double? width) => new Container(width: width,
        color: role.Color, padding: EdgeInsets.CreateAll(16), child: new Text(role.Label, style: new TextStyle(color: role.On)));
    private static async void Launch(BuildContext context)
    {
        var result = await Doroti.Framework.Services.UrlLauncher.launchUrl("https://pub.dev/packages/dynamic_color");
        if (context.mounted && !result.Succeeded) M.ScaffoldMessenger.of(context).showSnackBar(new M.SnackBar(content: new Text(result.Message ?? "Could not open link")));
    }
}
