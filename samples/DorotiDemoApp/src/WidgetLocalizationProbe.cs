#if DOROTI_WIDGET_LOCALIZATION_PROBE
using Doroti.Framework.Rendering;
using Doroti.Framework.Widgets;
using Doroti.Localizations.Material;
using Doroti.Ui;
using M = Doroti.Material;

namespace DorotiDemoApp;

internal sealed class WidgetLocalizationProbe : StatelessWidget
{
    private static readonly Locale English = new("en", "US");
    private static readonly Locale Korean = new("ko", "KR");

    public override Widget build(BuildContext context) => new M.MaterialApp(
        title: "Doroti widget localization probe",
        supportedLocales: [English, Korean],
        localizationsDelegates: [ProbeLocalizations.Delegate, .. GlobalMaterialLocalizations.Delegates],
        debugShowCheckedModeBanner: false,
        theme: M.ThemeData.Create(fontFamilyFallback: ["Roboto"]),
        home: new Builder(builder: localized => new M.Scaffold(body: new Center(child: new Column(
            mainAxisAlignment: MainAxisAlignment.center,
            spacing: 16,
            children:
            [
                Label("locale-probe-caption", ProbeLocalizations.Of(localized).caption(2)),
                Label("locale-probe-widget", M.MaterialLocalizations.of(localized).cancelButtonLabel),
                Localizations.CreateOverride(context: localized, locale: English, child: new Builder(builder: inner =>
                    Label("locale-probe-subtree", ProbeLocalizations.Of(inner).caption(1)))),
                new SizedBox(width: 300, child: new M.TextField()),
            ]))))
    );

    private static Widget Label(string id, string value) => new Semantics(identifier: id, container: true, child: new Text(value));
}
#endif
