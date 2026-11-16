using System.Globalization;
using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;
#if DOROTI_INTL_PROBE
using Doroti.Intl;
using Doroti.Intl.Tests;
#endif

namespace Doroti.Intl.Web.Tests;

[SupportedOSPlatform("browser")]
public static partial class Probe
{
    public static void Main() { }

    [JSExport]
    public static Task<string> Format(string localeName)
    {
#if DOROTI_INTL_PROBE
        return Task.FromResult(new IntlFormatter(localeName).FormatDate(new DateTime(2024, 2, 29), WidgetDateFormat.Full));
#else
        if (localeName != "en-US") throw new InvalidOperationException("The baseline is English only.");
        return Task.FromResult(new DateTime(2024, 2, 29).ToString("dddd, MMMM d, yyyy", new CultureInfo("en-US", false)));
#endif
    }

    [JSExport]
    public static Task<string> Contract()
    {
#if DOROTI_INTL_PROBE
        return Task.FromResult(GlobalizationContract.Run());
#else
        return Task.FromResult("{}");
#endif
    }

    [JSExport]
    public static Task<int> Concurrent()
    {
#if DOROTI_INTL_PROBE && DOROTI_INTL_THREADS
        return GlobalizationContract.CheckConcurrentAsync();
#else
        return Task.FromResult(0);
#endif
    }
}
