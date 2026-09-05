using System.Runtime.InteropServices;
using Doroti.Host.Qt;

QtNativeV2.ValidateLayout();

if (QtNativeV2.AbiVersion != 2 || QtNativeV2.RequiredFeatures != 0x3ff)
    throw new InvalidOperationException("doroti.qt-host/v2 feature identity drifted.");
if (Marshal.SizeOf<QtNativeV2.Configuration>() != 48)
    throw new InvalidOperationException("doroti.qt-host/v2 configuration layout drifted.");
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

// Exercise the actual adapter, including a layout/modifier change before key-up.
var host = new QtHostAdapter(1, default, 100, 100);
var keys = new List<Doroti.Ui.KeyData>();
host.KeyData += keys.Add;
host.ApplyKey(KeyPacket(30, 'A', 0), "\u0001");
host.ApplyKey(KeyPacket(30, 'A', 1), "");
if (keys.Count != 2 || keys.Any(key => key.logical != 'a') || keys[1].character is not null)
    throw new InvalidOperationException("Qt Ctrl+A down/up identity differs.");
keys.Clear();
host.ApplyKey(KeyPacket(11, '1', 0), "!");
host.ApplyKey(KeyPacket(11, '1', 1), "1");
if (keys.Any(key => key.logical != '!'))
    throw new InvalidOperationException("Qt shifted key release lost its pressed identity.");
keys.Clear();
host.ApplyKey(KeyPacket(37, 0x01000021, 0), "");
host.ApplyFocus(false, 0);
if (keys.Count != 2 || keys[1].type != Doroti.Ui.KeyEventType.up || !keys[1].synthesized || keys[1].logical != 0x200000100)
    throw new InvalidOperationException("Qt focus loss left a modifier pressed.");
Console.WriteLine("Doroti Linux Qt keyboard adapter: PASS");
QtClipboardContracts.Verify();

static QtNativeV2.Key KeyPacket(long physical, long logical, uint type)
{
    var bytes = new byte[Marshal.SizeOf<QtNativeV2.Key>()];
    System.Buffers.Binary.BinaryPrimitives.WriteUInt32LittleEndian(bytes, QtNativeV2.AbiVersion);
    System.Buffers.Binary.BinaryPrimitives.WriteUInt32LittleEndian(bytes.AsSpan(4), (uint)bytes.Length);
    System.Buffers.Binary.BinaryPrimitives.WriteInt64LittleEndian(bytes.AsSpan(8), physical);
    System.Buffers.Binary.BinaryPrimitives.WriteInt64LittleEndian(bytes.AsSpan(16), logical);
    System.Buffers.Binary.BinaryPrimitives.WriteUInt32LittleEndian(bytes.AsSpan(24), type);
    return MemoryMarshal.Read<QtNativeV2.Key>(bytes);
}
