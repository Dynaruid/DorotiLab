using System.Runtime.InteropServices;
using Doroti.Host.Qt;
using Doroti.Ui;

static void Check(bool condition, string message)
{ if (!condition) throw new InvalidOperationException(message); }

QtNativeV2.ValidateLayout();
Check(Marshal.SizeOf<QtQuickNative.Gpu>() == 48 && Marshal.SizeOf<QtQuickNative.Part>() == 96,
    "Quick C ABI layouts must match the native static assertions.");
var handle = new PlatformViewHandle(1, 2, 3);
var placement = new PlatformViewPlacement(handle, new Rect(10.25, 20.75, 110.75, 71),
    new PlatformViewTransform(1, 0, 0, 1, .125, .25), new Rect(11.5, 22.25, 100.75, 70.5), 0, true);
var quick = QtPlatformViewHost.Translate(placement, 7, quick: true);
Check(quick.Bounds.X == 10.375 && quick.Bounds.Y == 21 && quick.Bounds.Width == 100.5 && quick.Clip.X == 11.5,
    "Quick fractional transform/clip must remain logical doubles.");
var widgets = QtPlatformViewHost.Translate(placement, 7);
Check(widgets.Bounds.X == 10 && widgets.Clip.X == 12 && widgets.Clip.Width == 88,
    "Legacy Widgets preserve integer geometry and inward clipping.");
var hidden = QtPlatformViewHost.Translate(placement with { Clip = new Rect(200, 200, 210, 210) }, 7, quick: true);
Check(hidden.Visible == 0 && hidden.Clip == default, "Disjoint clip must disable native input.");
try {
    QtPlatformViewHost.Translate(placement with { Transform = new PlatformViewTransform(1, .5, 0, 1, 0, 0) }, 7, true);
    throw new InvalidOperationException("Unsupported transform was accepted.");
} catch (NotSupportedException) { }
Console.WriteLine("PASS: Qt ABI and Quick/Widgets geometry contracts");
if (args.Length == 1) GpuContract.Run(System.IO.Path.GetFullPath(args[0]));
