#nullable enable
using Doroti.Flutter.Ui;
using Doroti.Generated.Framework.Widgets;

namespace Doroti.Generated.Framework.WidgetPreviews;

public delegate PreviewThemeData PreviewTheme();
public delegate Widget WidgetWrapper(Widget child);
public delegate PreviewLocalizationsData PreviewLocalizations();

public sealed class Preview(
    string group = "Default",
    string? name = null,
    Size? size = null,
    double? textScaleFactor = null,
    Func<Widget, Widget>? wrapper = null,
    Func<PreviewThemeData>? theme = null,
    Brightness? brightness = null,
    Func<PreviewLocalizationsData>? localizations = null)
{
    public string group { get; } = group;
    public string? name { get; } = name;
    public Size? size { get; } = size;
    public double? textScaleFactor { get; } = textScaleFactor;
    public Func<Widget, Widget>? wrapper { get; } = wrapper;
    public Func<PreviewThemeData>? theme { get; } = theme;
    public Brightness? brightness { get; } = brightness;
    public Func<PreviewLocalizationsData>? localizations { get; } = localizations;
    public Preview transform() => this;
    public PreviewBuilder toBuilder() => PreviewBuilder.fromPreview(this);
}

public abstract class MultiPreview
{
    public abstract IReadOnlyList<Preview> previews { get; }
    public virtual IReadOnlyList<Preview> transform() => previews.Select(preview => preview.transform()).ToArray();
}

public sealed class PreviewBuilder
{
    public string? group { get; set; }
    public string? name { get; set; }
    public Size? size { get; set; }
    public double? textScaleFactor { get; set; }
    public Func<Widget, Widget>? wrapper { get; set; }
    public Func<PreviewThemeData>? theme { get; set; }
    public Brightness? brightness { get; set; }
    public Func<PreviewLocalizationsData>? localizations { get; set; }

    public static PreviewBuilder fromPreview(Preview preview) => new()
    {
        group = preview.group,
        name = preview.name,
        size = preview.size,
        textScaleFactor = preview.textScaleFactor,
        wrapper = preview.wrapper,
        theme = preview.theme,
        brightness = preview.brightness,
        localizations = preview.localizations,
    };

    public void addWrapper(Func<Widget, Widget> newWrapper)
    {
        ArgumentNullException.ThrowIfNull(newWrapper);
        var previous = wrapper;
        wrapper = previous is null ? newWrapper : child => newWrapper(previous(child));
    }

    public Preview build() => new(
        group: group ?? "Default",
        name: name,
        size: size,
        textScaleFactor: textScaleFactor,
        wrapper: wrapper,
        theme: theme,
        brightness: brightness,
        localizations: localizations);
}

public sealed class PreviewLocalizationsData(
    Locale? locale = null,
    IReadOnlyList<Locale>? supportedLocales = null,
    IEnumerable<object>? localizationsDelegates = null,
    Func<IReadOnlyList<Locale>?, IEnumerable<Locale>, Locale?>? localeListResolutionCallback = null,
    Func<Locale?, IEnumerable<Locale>, Locale?>? localeResolutionCallback = null)
{
    public Locale? locale { get; } = locale;
    public IReadOnlyList<Locale> supportedLocales { get; } = supportedLocales ?? [new Locale("en", "US")];
    public IEnumerable<object>? localizationsDelegates { get; } = localizationsDelegates;
    public Func<IReadOnlyList<Locale>?, IEnumerable<Locale>, Locale?>? localeListResolutionCallback { get; } = localeListResolutionCallback;
    public Func<Locale?, IEnumerable<Locale>, Locale?>? localeResolutionCallback { get; } = localeResolutionCallback;
}

public interface PreviewThemeData
{
    Widget apply(BuildContext context, Widget child);
}

public sealed class MultiPreviewThemeData(IReadOnlyList<PreviewThemeData> themes) : PreviewThemeData
{
    public IReadOnlyList<PreviewThemeData> themes { get; } = themes;

    public Widget apply(BuildContext context, Widget child)
    {
        var result = child;
        foreach (var theme in themes.Reverse()) result = theme.apply(context, result);
        return result;
    }
}
