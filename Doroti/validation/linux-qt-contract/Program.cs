using System.Runtime.InteropServices;
using Doroti.Host.Qt;

QtNativeV2.ValidateLayout();

if (QtNativeV2.AbiVersion != 2 || QtNativeV2.RequiredFeatures != 0x0f)
    throw new InvalidOperationException("doroti.qt-host/v2 feature identity drifted.");
if (Marshal.SizeOf<QtNativeV2.HostApi>() != 40)
    throw new InvalidOperationException("doroti.qt-host/v2 host API layout drifted.");
if (Marshal.SizeOf<QtNativeV2.Callbacks>() != 80)
    throw new InvalidOperationException("doroti.qt-host/v2 callback layout drifted.");

Console.WriteLine("Doroti Linux Qt ABI contract: PASS");
