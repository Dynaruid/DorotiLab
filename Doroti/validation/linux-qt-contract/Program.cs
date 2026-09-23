using System.Runtime.InteropServices;
using Doroti.Host.Qt;
using Doroti.Ui;

static void Check(bool condition, string message)
{
    if (!condition)
        throw new InvalidOperationException(message);
}

QtNativeV2.ValidateLayout();
Check(QtKeyMap.Physical(38, 'A') == QtKeyMap.Physical(38, 'Q')
    && QtKeyMap.Physical(38, 'A') == 0x00070004,
    "Physical key changed with keyboard layout.");
Check(QtKeyMap.Physical(50, 0x01000020) == 0x000700e1
    && QtKeyMap.Physical(62, 0x01000020) == 0x000700e5
    && QtKeyMap.Physical(37, 0x01000021) != QtKeyMap.Physical(105, 0x01000021),
    "Left/right modifiers collapsed.");
Check(QtKeyMap.Physical(36, 0x01000004) == 0x00070028
    && QtKeyMap.Physical(104, 0x01000005) == 0x00070058
    && QtKeyMap.Physical(87, '1') == 0x00070059,
    "Main and keypad keys collapsed.");
Check(QtKeyMap.Physical(0, 'A') == 0x00070004
    && QtKeyMap.Logical('A', "q") == 'q',
    "Synthetic fallback or logical layout changed.");
var keyHost = new QtHostAdapter(0, default, 100, 100);
var keys = new List<KeyData>();
keyHost.KeyData += keys.Add;
keyHost.ApplyKey(new QtNativeV2.Key(50, 0x01000020, 0), "");
keyHost.ApplyKey(new QtNativeV2.Key(62, 0x01000020, 0), "");
keyHost.ApplyKey(new QtNativeV2.Key(50, 0x01000020, 1), "");
keyHost.ApplyFocus(false, 0);
Check(keys.Count == 4 && keys[0].physical != keys[1].physical
    && keys[2].physical == keys[0].physical && !keys[2].synthesized
    && keys[3].physical == keys[1].physical && keys[3].synthesized,
    "Releasing one Shift lost the other held modifier on focus loss.");
Check((QtWebViewSession.RequiredFeatures & 31) == 31,
    "WebView ABI 1 required features changed without a contract update.");
Check(QtWebViewSession.SupportsFeatures(31 | (1UL << 40))
    && !QtWebViewSession.SupportsFeatures(30)
    && QtPlatformViewHost.SupportsFeatures(1 | (1UL << 40))
    && !QtPlatformViewHost.SupportsFeatures(2),
    "Optional unknown ABI bits or missing required bits were negotiated incorrectly.");
foreach (var (status, expected) in new[]
{
    (64, WebViewError.InvalidRequest), (72, WebViewError.InvalidRequest),
    (70, WebViewError.ProcessFailed), (71, WebViewError.Closed),
    (73, WebViewError.Closed), (74, WebViewError.Unsupported),
})
{
    try
    {
        QtWebViewSession.Check(status, "contract", 42);
        throw new InvalidOperationException($"Native status {status} was accepted.");
    }
    catch (WebViewException error)
    {
        Check(error.Code == expected && error.NativeStatus == status
            && error.NativeOperation == "contract" && error.NativeOwner == 42
            && error.Message.Contains($"status {status}")
            && error.Message.Contains("owner 42"), "Native WebView error lost its cause or owner.");
    }
}
Check(
    Marshal.SizeOf<QtWebViewSession.Api>() == 32
        && Marshal.OffsetOf<QtWebViewSession.Api>("Bind").ToInt32() == 16,
    "Independent WebView ABI must preserve its 32-byte layout."
);
Check(
    Marshal.SizeOf<QtQuickNative.Gpu>() == 48 && Marshal.SizeOf<QtQuickNative.Part>() == 96,
    "Quick C ABI layouts must match the native static assertions."
);
var handle = new PlatformViewHandle(1, 2, 3);
var placement = new PlatformViewPlacement(
    handle,
    new Rect(10.25, 20.75, 110.75, 71),
    new PlatformViewTransform(1, 0, 0, 1, .125, .25),
    new Rect(11.5, 22.25, 100.75, 70.5),
    0,
    true
);
var quick = QtPlatformViewHost.Translate(placement, 7, quick: true);
Check(
    quick.Bounds.X == 10.375
        && quick.Bounds.Y == 21
        && quick.Bounds.Width == 100.5
        && quick.Clip.X == 11.5,
    "Quick fractional transform/clip must remain logical doubles."
);
var widgets = QtPlatformViewHost.Translate(placement, 7);
Check(
    widgets.Bounds.X == 10 && widgets.Clip.X == 12 && widgets.Clip.Width == 88,
    "Legacy Widgets preserve integer geometry and inward clipping."
);
var hidden = QtPlatformViewHost.Translate(
    placement with
    {
        Clip = new Rect(200, 200, 210, 210),
    },
    7,
    quick: true
);
Check(hidden.Visible == 0 && hidden.Clip == default, "Disjoint clip must disable native input.");
try
{
    QtPlatformViewHost.Translate(
        placement with
        {
            Transform = new PlatformViewTransform(1, .5, 0, 1, 0, 0),
        },
        7,
        true
    );
    throw new InvalidOperationException("Unsupported transform was accepted.");
}
catch (NotSupportedException) { }
Console.WriteLine("PASS: Qt ABI and Quick/Widgets geometry contracts");
if (args.Length == 1)
    GpuContract.Run(System.IO.Path.GetFullPath(args[0]));
