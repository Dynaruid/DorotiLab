using System.Runtime.InteropServices;
using Doroti.Host.Qt;

QtNativeV2.ValidateLayout();

if (QtNativeV2.AbiVersion != 2 || QtNativeV2.RequiredFeatures != 0x3ff)
    throw new InvalidOperationException("doroti.qt-host/v2 feature identity drifted.");
if (Marshal.SizeOf<QtNativeV2.HostApi>() != 120)
    throw new InvalidOperationException("doroti.qt-host/v2 host API layout drifted.");
if (Marshal.SizeOf<QtNativeV2.Callbacks>() != 176)
    throw new InvalidOperationException("doroti.qt-host/v2 callback layout drifted.");
if (Marshal.SizeOf<QtNativeV2.Metrics>() != 56 ||
    Marshal.SizeOf<QtNativeV2.Pointer>() != 120 ||
    Marshal.SizeOf<QtNativeV2.Key>() != 56 ||
    Marshal.SizeOf<QtNativeV2.TextConfiguration>() != 40 ||
    Marshal.SizeOf<QtNativeV2.TextState>() != 40)
    throw new InvalidOperationException("doroti.qt-host/v2 extended payload layout drifted.");
if (QtKeyMap.Physical(30, 'A') != 0x00070004 ||
    QtKeyMap.Physical(28, 0x01000004) != 0x00070028 ||
    QtKeyMap.Logical('A', "A") != 'a' ||
    QtKeyMap.Logical(0x01000013, string.Empty) != 0x100000304)
    throw new InvalidOperationException("doroti.qt-host/v2 key map drifted.");

Console.WriteLine("Doroti Linux Qt ABI contract: PASS");
