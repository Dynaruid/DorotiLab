using Doroti.Framework.Foundation;
using Doroti.Framework.Widgets;
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Localizations.Widgets;

public sealed class GlobalWidgetsLocalizations : WidgetsLocalizations
{
    private readonly WidgetsStrings _strings;
    private readonly string? _script;

    private GlobalWidgetsLocalizations(Locale locale)
    {
        _strings = WidgetsStrings.Get(locale);
        _script = locale.normalized().scriptCode;
    }

    public static IReadOnlyList<Locale> SupportedLocales => WidgetsStrings.SupportedLocales;
    public static LocalizationsDelegate<WidgetsLocalizations> Delegate { get; } = new ResourceDelegate();

    public override TextDirection textDirection => _script switch
    {
        "Arab" or "Hebr" or "Adlm" or "Nkoo" or "Rohg" or "Syrc" or "Thaa" => TextDirection.rtl,
        null => _strings.textDirection == "rtl" ? TextDirection.rtl : TextDirection.ltr,
        _ => TextDirection.ltr,
    };
    public override string reorderItemToStart => _strings.reorderItemToStart;
    public override string reorderItemToEnd => _strings.reorderItemToEnd;
    public override string reorderItemUp => _strings.reorderItemUp;
    public override string reorderItemDown => _strings.reorderItemDown;
    public override string reorderItemLeft => _strings.reorderItemLeft;
    public override string reorderItemRight => _strings.reorderItemRight;
    public override string searchResultsFound => _strings.searchResultsFound;
    public override string noResultsFound => _strings.noResultsFound;
    public override string copyButtonLabel => _strings.copyButtonLabel;
    public override string cutButtonLabel => _strings.cutButtonLabel;
    public override string pasteButtonLabel => _strings.pasteButtonLabel;
    public override string selectAllButtonLabel => _strings.selectAllButtonLabel;
    public override string lookUpButtonLabel => _strings.lookUpButtonLabel;
    public override string searchWebButtonLabel => _strings.searchWebButtonLabel;
    public override string shareButtonLabel => _strings.shareButtonLabel;
    public override string radioButtonUnselectedLabel => _strings.radioButtonUnselectedLabel;

    private sealed class ResourceDelegate : LocalizationsDelegate<WidgetsLocalizations>
    {
        public override bool isSupported(Locale locale) => WidgetsStrings.IsSupported(locale);
        public override Future<WidgetsLocalizations> load(Locale locale) =>
            new SynchronousFuture<WidgetsLocalizations>(new GlobalWidgetsLocalizations(locale));
        public override bool shouldReload(LocalizationsDelegate<WidgetsLocalizations> old) => false;
    }
}
