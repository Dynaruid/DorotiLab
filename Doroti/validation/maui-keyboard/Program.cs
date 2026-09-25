using Doroti.Host.Maui;
using Doroti.Hosting;
using Doroti.Ui;

var checks = 0;
void Check(bool value, string name)
{
    if (!value) throw new InvalidOperationException(name);
    checks++;
}
Check(WindowsKeyboardMap.Physical(0x1e, 'Q') == 0x70004, "Windows layout independent physical A");
Check(WindowsKeyboardMap.Logical(0x1e, 'Q', "q") == 'q', "Windows logical follows layout");
Check(WindowsKeyboardMap.Physical(0x11c, 0x0d) == 0x70058, "Windows keypad Enter");
Check(WindowsKeyboardMap.Physical(0x36, 0x10) == 0x700e5, "Windows right Shift");
Check(WindowsKeyboardMap.Physical(0, 0xa1) == 0x700e5 && WindowsKeyboardMap.Logical(0, 0xa3, "") == 0x200000101, "Windows side-specific synthetic modifiers");
Check(WindowsKeyboardMap.Physical(0x4f, 0x23) == 0x70059, "Windows NumLock off keypad position");
Check(WindowsKeyboardMap.Logical(0x4f, 0x61, "1") == 0x200000231, "Windows numpad identity");
Check(MauiKeyMap.AndroidPhysical(45, 30) == 0x70004, "Android physical A with logical Q");
Check(MauiKeyMap.Logical("q", MauiKeyMap.AndroidPhysical(45)) == 'q', "Android logical layout");
Check(MauiKeyMap.AndroidPhysical(145, 0) == 0x70059, "Android synthetic numpad fallback");
Check(MauiKeyMap.AndroidPhysical(160, 96) == 0x70058, "Android keypad Enter");
Check(MauiKeyMap.AndroidPhysical(123, 79) == 0x70059, "Android NumLock off physical keypad");
Check(MauiKeyMap.Logical("End", MauiKeyMap.AndroidPhysical(123)) == 0x100000305, "Android NumLock off logical End");
Check(MauiKeyMap.Logical("1", MauiKeyMap.AndroidPhysical(145)) == 0x200000231, "Android numpad logical identity");
Check(MauiKeyMap.AndroidPhysical(0, 900) != MauiKeyMap.AndroidPhysical(0, 901), "Android unknown positions distinct");
Check(MauiKeyMap.AndroidPhysical(29, 0) == 0x70004, "Android ADB keycode fallback");
Check(MauiKeyMap.AndroidCharacter(0x1000a) == char.ConvertFromUtf32(0x1000a), "Supplementary scalar with control low word");
Check(MauiKeyMap.Logical(char.ConvertFromUtf32(0x1000a), 0) == 0x1000a, "Supplementary logical scalar");
Check(MauiKeyMap.AndroidCharacter(unchecked((int)0x800000b4)) is null, "Dead key is not committed text");
Check(MauiKeyMap.AndroidCharacter(0xd800) is null && MauiKeyMap.AndroidCharacter(0x110000) is null, "Invalid Unicode rejected");
Check(MauiKeyMap.AndroidCharacter(1) is null, "Control character rejected");

var keys = new MauiKeyboardState();
KeyData Key(KeyEventType type, long physical, long logical, string? text = null) =>
    new(7, TimeSpan.FromMilliseconds(10), type, physical, logical, false, text);
var down = keys.Apply(Key(KeyEventType.down, 0x70004, 'a', "a"));
var repeat = keys.Apply(Key(KeyEventType.down, 0x70004, 'q', "q"));
Check(repeat is { type: KeyEventType.repeat, logical: 'a', character: "q" }, "Repeated down retains initial logical identity");
Check(keys.Apply(Key(KeyEventType.up, 0x70004, 'q')) is { logical: 'a', character: null }, "Key up retains initial identity");
Check(keys.ReleaseAll(7, TimeSpan.Zero).Length == 0, "Normal up clears state");
keys.Apply(Key(KeyEventType.down, 0x700e1, 0x200000102));
keys.Apply(Key(KeyEventType.down, 0x700e5, 0x200000103));
keys.Apply(Key(KeyEventType.up, 0x700e1, 0x200000102));
var ups = keys.ReleaseAll(7, TimeSpan.Zero);
Check(ups.Length == 1 && ups[0] is { viewId: 7, physical: 0x700e5, synthesized: true, type: KeyEventType.up }, "Focus loss releases only remaining Shift");
Check(keys.ReleaseAll(7, TimeSpan.Zero).Length == 0, "Window and element blur do not double release");
Check(keys.Apply(Key(KeyEventType.up, 0x700e5, 0x200000103)) is null, "Late native up after blur ignored");
Check(keys.Apply(Key(KeyEventType.repeat, 0x70004, 'q')) is { type: KeyEventType.down, logical: 'q' }, "Repeat on reacquisition starts new session");
Console.WriteLine($"PASS MAUI keyboard contracts: {checks}");
