#if DOROTI_INTL_RENDER_PROBE
using Doroti.Framework.Rendering;
using Doroti.Framework.Widgets;
using Doroti.Ui;
using M = Doroti.Material;
using FrameworkTextStyle = Doroti.Framework.Painting.TextStyle;
#if DOROTI_INTL_RENDER_ADAPTER
using Doroti.Intl;
#endif

namespace DorotiDemoApp;

// An explicitly selected P0 measurement page. The default Components gallery and delegates
// keep their existing behavior. This page tests formatting, not translated widget resources.
internal sealed class IntlRenderProbe : StatefulWidget
{
    public override IState createState() => new IntlRenderProbeState();
}

internal sealed class IntlRenderProbeState : State<IntlRenderProbe>
{
    private readonly M.ThemeData _theme = M.ThemeData.Create(
        fontFamilyFallback: ["Roboto"], colorSchemeSeed: new Color(0xff6750a4));
    private bool _alternateDate;
#if DOROTI_INTL_RENDER_ADAPTER
    private string _locale = "en-US";
#else
    private readonly M.DefaultMaterialLocalizations _legacy = new();
#endif

    public override Widget build(BuildContext context)
    {
        var date = _alternateDate ? new DateTime(2024, 3, 1) : new DateTime(2024, 2, 29);
        string fullDate, number, time;
#if DOROTI_INTL_RENDER_ADAPTER
        var formatter = new IntlFormatter(_locale);
        fullDate = formatter.FormatDate(date, WidgetDateFormat.Full);
        number = formatter.FormatInteger(1234);
        time = formatter.FormatTime(new TimeOnly(13, 5));
#else
        fullDate = _legacy.formatFullDate(date);
        number = _legacy.formatDecimal(1234);
        time = _legacy.formatTimeOfDay(new M.TimeOfDay(13, 5));
#endif
        return new M.MaterialApp(
            title: "Doroti localization format probe",
            locale: new Locale("en", "US"),
            debugShowCheckedModeBanner: false,
            theme: _theme,
            home: new M.Scaffold(
                body: new Center(
                    child: new Column(
                        mainAxisAlignment: MainAxisAlignment.center,
                        spacing: 16,
                        children:
                        [
                            new Text("Localization format probe", style: new FrameworkTextStyle(fontSize: 26)),
                            new Text(fullDate),
                            new Text(number),
                            new Text(time),
                            new M.ElevatedButton(onPressed: () => setState(() =>
                            {
#if DOROTI_INTL_RENDER_ADAPTER
                                _locale = "en-US";
#endif
                                _alternateDate = !_alternateDate;
                            }), child: new Text("English date")),
                            new M.ElevatedButton(onPressed: () => setState(() =>
                            {
#if DOROTI_INTL_RENDER_ADAPTER
                                _locale = "ko-KR";
#endif
                                _alternateDate = false;
                            }), child: new Text("Korean date")),
                        ]
                    )
                )
            )
        );
    }
}
#endif
