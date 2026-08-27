// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/colors.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Cupertino;

public abstract class CupertinoColors
{
    public static CupertinoDynamicColor activeBlue => systemBlue;
    public static CupertinoDynamicColor activeGreen => systemGreen;
    public static CupertinoDynamicColor activeOrange => systemOrange;
    public static Color white = new global::Doroti.Ui.Color(4294967295L);
    public static Color black = new global::Doroti.Ui.Color(4278190080L);
    public static Color transparent = new global::Doroti.Ui.Color(0L);
    public static Color lightBackgroundGray = new global::Doroti.Ui.Color(4293256682L);
    public static Color extraLightBackgroundGray = new global::Doroti.Ui.Color(4293914612L);
    public static Color darkBackgroundGray = new global::Doroti.Ui.Color(4279703319L);
    public static CupertinoDynamicColor inactiveGray = CupertinoDynamicColor.CreateWithBrightness(debugLabel: "inactiveGray", color: new global::Doroti.Ui.Color(4288256409L), darkColor: new global::Doroti.Ui.Color(4285887861L));
    public static CupertinoDynamicColor destructiveRed => systemRed;
    public static CupertinoDynamicColor systemBlue = CupertinoDynamicColor.CreateWithBrightnessAndContrast(debugLabel: "systemBlue", color: global::Doroti.Ui.Color.fromARGB(255L, 0L, 122L, 255L), darkColor: global::Doroti.Ui.Color.fromARGB(255L, 10L, 132L, 255L), highContrastColor: global::Doroti.Ui.Color.fromARGB(255L, 0L, 64L, 221L), darkHighContrastColor: global::Doroti.Ui.Color.fromARGB(255L, 64L, 156L, 255L));
    public static CupertinoDynamicColor systemGreen = CupertinoDynamicColor.CreateWithBrightnessAndContrast(debugLabel: "systemGreen", color: global::Doroti.Ui.Color.fromARGB(255L, 52L, 199L, 89L), darkColor: global::Doroti.Ui.Color.fromARGB(255L, 48L, 209L, 88L), highContrastColor: global::Doroti.Ui.Color.fromARGB(255L, 36L, 138L, 61L), darkHighContrastColor: global::Doroti.Ui.Color.fromARGB(255L, 48L, 219L, 91L));
    public static CupertinoDynamicColor systemMint = CupertinoDynamicColor.CreateWithBrightnessAndContrast(debugLabel: "systemMint", color: global::Doroti.Ui.Color.fromARGB(255L, 0L, 199L, 190L), darkColor: global::Doroti.Ui.Color.fromARGB(255L, 99L, 230L, 226L), highContrastColor: global::Doroti.Ui.Color.fromARGB(255L, 12L, 129L, 123L), darkHighContrastColor: global::Doroti.Ui.Color.fromARGB(255L, 102L, 212L, 207L));
    public static CupertinoDynamicColor systemIndigo = CupertinoDynamicColor.CreateWithBrightnessAndContrast(debugLabel: "systemIndigo", color: global::Doroti.Ui.Color.fromARGB(255L, 88L, 86L, 214L), darkColor: global::Doroti.Ui.Color.fromARGB(255L, 94L, 92L, 230L), highContrastColor: global::Doroti.Ui.Color.fromARGB(255L, 54L, 52L, 163L), darkHighContrastColor: global::Doroti.Ui.Color.fromARGB(255L, 125L, 122L, 255L));
    public static CupertinoDynamicColor systemOrange = CupertinoDynamicColor.CreateWithBrightnessAndContrast(debugLabel: "systemOrange", color: global::Doroti.Ui.Color.fromARGB(255L, 255L, 149L, 0L), darkColor: global::Doroti.Ui.Color.fromARGB(255L, 255L, 159L, 10L), highContrastColor: global::Doroti.Ui.Color.fromARGB(255L, 201L, 52L, 0L), darkHighContrastColor: global::Doroti.Ui.Color.fromARGB(255L, 255L, 179L, 64L));
    public static CupertinoDynamicColor systemPink = CupertinoDynamicColor.CreateWithBrightnessAndContrast(debugLabel: "systemPink", color: global::Doroti.Ui.Color.fromARGB(255L, 255L, 45L, 85L), darkColor: global::Doroti.Ui.Color.fromARGB(255L, 255L, 55L, 95L), highContrastColor: global::Doroti.Ui.Color.fromARGB(255L, 211L, 15L, 69L), darkHighContrastColor: global::Doroti.Ui.Color.fromARGB(255L, 255L, 100L, 130L));
    public static CupertinoDynamicColor systemBrown = CupertinoDynamicColor.CreateWithBrightnessAndContrast(debugLabel: "systemBrown", color: global::Doroti.Ui.Color.fromARGB(255L, 162L, 132L, 94L), darkColor: global::Doroti.Ui.Color.fromARGB(255L, 172L, 142L, 104L), highContrastColor: global::Doroti.Ui.Color.fromARGB(255L, 127L, 101L, 69L), darkHighContrastColor: global::Doroti.Ui.Color.fromARGB(255L, 181L, 148L, 105L));
    public static CupertinoDynamicColor systemPurple = CupertinoDynamicColor.CreateWithBrightnessAndContrast(debugLabel: "systemPurple", color: global::Doroti.Ui.Color.fromARGB(255L, 175L, 82L, 222L), darkColor: global::Doroti.Ui.Color.fromARGB(255L, 191L, 90L, 242L), highContrastColor: global::Doroti.Ui.Color.fromARGB(255L, 137L, 68L, 171L), darkHighContrastColor: global::Doroti.Ui.Color.fromARGB(255L, 218L, 143L, 255L));
    public static CupertinoDynamicColor systemRed = CupertinoDynamicColor.CreateWithBrightnessAndContrast(debugLabel: "systemRed", color: global::Doroti.Ui.Color.fromARGB(255L, 255L, 59L, 48L), darkColor: global::Doroti.Ui.Color.fromARGB(255L, 255L, 69L, 58L), highContrastColor: global::Doroti.Ui.Color.fromARGB(255L, 215L, 0L, 21L), darkHighContrastColor: global::Doroti.Ui.Color.fromARGB(255L, 255L, 105L, 97L));
    public static CupertinoDynamicColor systemTeal = CupertinoDynamicColor.CreateWithBrightnessAndContrast(debugLabel: "systemTeal", color: global::Doroti.Ui.Color.fromARGB(255L, 90L, 200L, 250L), darkColor: global::Doroti.Ui.Color.fromARGB(255L, 100L, 210L, 255L), highContrastColor: global::Doroti.Ui.Color.fromARGB(255L, 0L, 113L, 164L), darkHighContrastColor: global::Doroti.Ui.Color.fromARGB(255L, 112L, 215L, 255L));
    public static CupertinoDynamicColor systemCyan = CupertinoDynamicColor.CreateWithBrightnessAndContrast(debugLabel: "systemCyan", color: global::Doroti.Ui.Color.fromARGB(255L, 50L, 173L, 230L), darkColor: global::Doroti.Ui.Color.fromARGB(255L, 100L, 210L, 255L), highContrastColor: global::Doroti.Ui.Color.fromARGB(255L, 0L, 113L, 164L), darkHighContrastColor: global::Doroti.Ui.Color.fromARGB(255L, 112L, 215L, 255L));
    public static CupertinoDynamicColor systemYellow = CupertinoDynamicColor.CreateWithBrightnessAndContrast(debugLabel: "systemYellow", color: global::Doroti.Ui.Color.fromARGB(255L, 255L, 204L, 0L), darkColor: global::Doroti.Ui.Color.fromARGB(255L, 255L, 214L, 10L), highContrastColor: global::Doroti.Ui.Color.fromARGB(255L, 160L, 90L, 0L), darkHighContrastColor: global::Doroti.Ui.Color.fromARGB(255L, 255L, 212L, 38L));
    public static CupertinoDynamicColor systemGrey = CupertinoDynamicColor.CreateWithBrightnessAndContrast(debugLabel: "systemGrey", color: global::Doroti.Ui.Color.fromARGB(255L, 142L, 142L, 147L), darkColor: global::Doroti.Ui.Color.fromARGB(255L, 142L, 142L, 147L), highContrastColor: global::Doroti.Ui.Color.fromARGB(255L, 108L, 108L, 112L), darkHighContrastColor: global::Doroti.Ui.Color.fromARGB(255L, 174L, 174L, 178L));
    public static CupertinoDynamicColor systemGrey2 = CupertinoDynamicColor.CreateWithBrightnessAndContrast(debugLabel: "systemGrey2", color: global::Doroti.Ui.Color.fromARGB(255L, 174L, 174L, 178L), darkColor: global::Doroti.Ui.Color.fromARGB(255L, 99L, 99L, 102L), highContrastColor: global::Doroti.Ui.Color.fromARGB(255L, 142L, 142L, 147L), darkHighContrastColor: global::Doroti.Ui.Color.fromARGB(255L, 124L, 124L, 128L));
    public static CupertinoDynamicColor systemGrey3 = CupertinoDynamicColor.CreateWithBrightnessAndContrast(debugLabel: "systemGrey3", color: global::Doroti.Ui.Color.fromARGB(255L, 199L, 199L, 204L), darkColor: global::Doroti.Ui.Color.fromARGB(255L, 72L, 72L, 74L), highContrastColor: global::Doroti.Ui.Color.fromARGB(255L, 174L, 174L, 178L), darkHighContrastColor: global::Doroti.Ui.Color.fromARGB(255L, 84L, 84L, 86L));
    public static CupertinoDynamicColor systemGrey4 = CupertinoDynamicColor.CreateWithBrightnessAndContrast(debugLabel: "systemGrey4", color: global::Doroti.Ui.Color.fromARGB(255L, 209L, 209L, 214L), darkColor: global::Doroti.Ui.Color.fromARGB(255L, 58L, 58L, 60L), highContrastColor: global::Doroti.Ui.Color.fromARGB(255L, 188L, 188L, 192L), darkHighContrastColor: global::Doroti.Ui.Color.fromARGB(255L, 68L, 68L, 70L));
    public static CupertinoDynamicColor systemGrey5 = CupertinoDynamicColor.CreateWithBrightnessAndContrast(debugLabel: "systemGrey5", color: global::Doroti.Ui.Color.fromARGB(255L, 229L, 229L, 234L), darkColor: global::Doroti.Ui.Color.fromARGB(255L, 44L, 44L, 46L), highContrastColor: global::Doroti.Ui.Color.fromARGB(255L, 216L, 216L, 220L), darkHighContrastColor: global::Doroti.Ui.Color.fromARGB(255L, 54L, 54L, 56L));
    public static CupertinoDynamicColor systemGrey6 = CupertinoDynamicColor.CreateWithBrightnessAndContrast(debugLabel: "systemGrey6", color: global::Doroti.Ui.Color.fromARGB(255L, 242L, 242L, 247L), darkColor: global::Doroti.Ui.Color.fromARGB(255L, 28L, 28L, 30L), highContrastColor: global::Doroti.Ui.Color.fromARGB(255L, 235L, 235L, 240L), darkHighContrastColor: global::Doroti.Ui.Color.fromARGB(255L, 36L, 36L, 38L));
    public static CupertinoDynamicColor label = new CupertinoDynamicColor(debugLabel: "label", color: global::Doroti.Ui.Color.fromARGB(255L, 0L, 0L, 0L), darkColor: global::Doroti.Ui.Color.fromARGB(255L, 255L, 255L, 255L), highContrastColor: global::Doroti.Ui.Color.fromARGB(255L, 0L, 0L, 0L), darkHighContrastColor: global::Doroti.Ui.Color.fromARGB(255L, 255L, 255L, 255L), elevatedColor: global::Doroti.Ui.Color.fromARGB(255L, 0L, 0L, 0L), darkElevatedColor: global::Doroti.Ui.Color.fromARGB(255L, 255L, 255L, 255L), highContrastElevatedColor: global::Doroti.Ui.Color.fromARGB(255L, 0L, 0L, 0L), darkHighContrastElevatedColor: global::Doroti.Ui.Color.fromARGB(255L, 255L, 255L, 255L));
    public static CupertinoDynamicColor secondaryLabel = new CupertinoDynamicColor(debugLabel: "secondaryLabel", color: global::Doroti.Ui.Color.fromARGB(153L, 60L, 60L, 67L), darkColor: global::Doroti.Ui.Color.fromARGB(153L, 235L, 235L, 245L), highContrastColor: global::Doroti.Ui.Color.fromARGB(173L, 60L, 60L, 67L), darkHighContrastColor: global::Doroti.Ui.Color.fromARGB(173L, 235L, 235L, 245L), elevatedColor: global::Doroti.Ui.Color.fromARGB(153L, 60L, 60L, 67L), darkElevatedColor: global::Doroti.Ui.Color.fromARGB(153L, 235L, 235L, 245L), highContrastElevatedColor: global::Doroti.Ui.Color.fromARGB(173L, 60L, 60L, 67L), darkHighContrastElevatedColor: global::Doroti.Ui.Color.fromARGB(173L, 235L, 235L, 245L));
    public static CupertinoDynamicColor tertiaryLabel = new CupertinoDynamicColor(debugLabel: "tertiaryLabel", color: global::Doroti.Ui.Color.fromARGB(76L, 60L, 60L, 67L), darkColor: global::Doroti.Ui.Color.fromARGB(76L, 235L, 235L, 245L), highContrastColor: global::Doroti.Ui.Color.fromARGB(96L, 60L, 60L, 67L), darkHighContrastColor: global::Doroti.Ui.Color.fromARGB(96L, 235L, 235L, 245L), elevatedColor: global::Doroti.Ui.Color.fromARGB(76L, 60L, 60L, 67L), darkElevatedColor: global::Doroti.Ui.Color.fromARGB(76L, 235L, 235L, 245L), highContrastElevatedColor: global::Doroti.Ui.Color.fromARGB(96L, 60L, 60L, 67L), darkHighContrastElevatedColor: global::Doroti.Ui.Color.fromARGB(96L, 235L, 235L, 245L));
    public static CupertinoDynamicColor quaternaryLabel = new CupertinoDynamicColor(debugLabel: "quaternaryLabel", color: global::Doroti.Ui.Color.fromARGB(45L, 60L, 60L, 67L), darkColor: global::Doroti.Ui.Color.fromARGB(40L, 235L, 235L, 245L), highContrastColor: global::Doroti.Ui.Color.fromARGB(66L, 60L, 60L, 67L), darkHighContrastColor: global::Doroti.Ui.Color.fromARGB(61L, 235L, 235L, 245L), elevatedColor: global::Doroti.Ui.Color.fromARGB(45L, 60L, 60L, 67L), darkElevatedColor: global::Doroti.Ui.Color.fromARGB(40L, 235L, 235L, 245L), highContrastElevatedColor: global::Doroti.Ui.Color.fromARGB(66L, 60L, 60L, 67L), darkHighContrastElevatedColor: global::Doroti.Ui.Color.fromARGB(61L, 235L, 235L, 245L));
    public static CupertinoDynamicColor systemFill = new CupertinoDynamicColor(debugLabel: "systemFill", color: global::Doroti.Ui.Color.fromARGB(51L, 120L, 120L, 128L), darkColor: global::Doroti.Ui.Color.fromARGB(91L, 120L, 120L, 128L), highContrastColor: global::Doroti.Ui.Color.fromARGB(71L, 120L, 120L, 128L), darkHighContrastColor: global::Doroti.Ui.Color.fromARGB(112L, 120L, 120L, 128L), elevatedColor: global::Doroti.Ui.Color.fromARGB(51L, 120L, 120L, 128L), darkElevatedColor: global::Doroti.Ui.Color.fromARGB(91L, 120L, 120L, 128L), highContrastElevatedColor: global::Doroti.Ui.Color.fromARGB(71L, 120L, 120L, 128L), darkHighContrastElevatedColor: global::Doroti.Ui.Color.fromARGB(112L, 120L, 120L, 128L));
    public static CupertinoDynamicColor secondarySystemFill = new CupertinoDynamicColor(debugLabel: "secondarySystemFill", color: global::Doroti.Ui.Color.fromARGB(40L, 120L, 120L, 128L), darkColor: global::Doroti.Ui.Color.fromARGB(81L, 120L, 120L, 128L), highContrastColor: global::Doroti.Ui.Color.fromARGB(61L, 120L, 120L, 128L), darkHighContrastColor: global::Doroti.Ui.Color.fromARGB(102L, 120L, 120L, 128L), elevatedColor: global::Doroti.Ui.Color.fromARGB(40L, 120L, 120L, 128L), darkElevatedColor: global::Doroti.Ui.Color.fromARGB(81L, 120L, 120L, 128L), highContrastElevatedColor: global::Doroti.Ui.Color.fromARGB(61L, 120L, 120L, 128L), darkHighContrastElevatedColor: global::Doroti.Ui.Color.fromARGB(102L, 120L, 120L, 128L));
    public static CupertinoDynamicColor tertiarySystemFill = new CupertinoDynamicColor(debugLabel: "tertiarySystemFill", color: global::Doroti.Ui.Color.fromARGB(30L, 118L, 118L, 128L), darkColor: global::Doroti.Ui.Color.fromARGB(61L, 118L, 118L, 128L), highContrastColor: global::Doroti.Ui.Color.fromARGB(51L, 118L, 118L, 128L), darkHighContrastColor: global::Doroti.Ui.Color.fromARGB(81L, 118L, 118L, 128L), elevatedColor: global::Doroti.Ui.Color.fromARGB(30L, 118L, 118L, 128L), darkElevatedColor: global::Doroti.Ui.Color.fromARGB(61L, 118L, 118L, 128L), highContrastElevatedColor: global::Doroti.Ui.Color.fromARGB(51L, 118L, 118L, 128L), darkHighContrastElevatedColor: global::Doroti.Ui.Color.fromARGB(81L, 118L, 118L, 128L));
    public static CupertinoDynamicColor quaternarySystemFill = new CupertinoDynamicColor(debugLabel: "quaternarySystemFill", color: global::Doroti.Ui.Color.fromARGB(20L, 116L, 116L, 128L), darkColor: global::Doroti.Ui.Color.fromARGB(45L, 118L, 118L, 128L), highContrastColor: global::Doroti.Ui.Color.fromARGB(40L, 116L, 116L, 128L), darkHighContrastColor: global::Doroti.Ui.Color.fromARGB(66L, 118L, 118L, 128L), elevatedColor: global::Doroti.Ui.Color.fromARGB(20L, 116L, 116L, 128L), darkElevatedColor: global::Doroti.Ui.Color.fromARGB(45L, 118L, 118L, 128L), highContrastElevatedColor: global::Doroti.Ui.Color.fromARGB(40L, 116L, 116L, 128L), darkHighContrastElevatedColor: global::Doroti.Ui.Color.fromARGB(66L, 118L, 118L, 128L));
    public static CupertinoDynamicColor placeholderText = new CupertinoDynamicColor(debugLabel: "placeholderText", color: global::Doroti.Ui.Color.fromARGB(76L, 60L, 60L, 67L), darkColor: global::Doroti.Ui.Color.fromARGB(76L, 235L, 235L, 245L), highContrastColor: global::Doroti.Ui.Color.fromARGB(96L, 60L, 60L, 67L), darkHighContrastColor: global::Doroti.Ui.Color.fromARGB(96L, 235L, 235L, 245L), elevatedColor: global::Doroti.Ui.Color.fromARGB(76L, 60L, 60L, 67L), darkElevatedColor: global::Doroti.Ui.Color.fromARGB(76L, 235L, 235L, 245L), highContrastElevatedColor: global::Doroti.Ui.Color.fromARGB(96L, 60L, 60L, 67L), darkHighContrastElevatedColor: global::Doroti.Ui.Color.fromARGB(96L, 235L, 235L, 245L));
    public static CupertinoDynamicColor systemBackground = new CupertinoDynamicColor(debugLabel: "systemBackground", color: global::Doroti.Ui.Color.fromARGB(255L, 255L, 255L, 255L), darkColor: global::Doroti.Ui.Color.fromARGB(255L, 0L, 0L, 0L), highContrastColor: global::Doroti.Ui.Color.fromARGB(255L, 255L, 255L, 255L), darkHighContrastColor: global::Doroti.Ui.Color.fromARGB(255L, 0L, 0L, 0L), elevatedColor: global::Doroti.Ui.Color.fromARGB(255L, 255L, 255L, 255L), darkElevatedColor: global::Doroti.Ui.Color.fromARGB(255L, 28L, 28L, 30L), highContrastElevatedColor: global::Doroti.Ui.Color.fromARGB(255L, 255L, 255L, 255L), darkHighContrastElevatedColor: global::Doroti.Ui.Color.fromARGB(255L, 36L, 36L, 38L));
    public static CupertinoDynamicColor secondarySystemBackground = new CupertinoDynamicColor(debugLabel: "secondarySystemBackground", color: global::Doroti.Ui.Color.fromARGB(255L, 242L, 242L, 247L), darkColor: global::Doroti.Ui.Color.fromARGB(255L, 28L, 28L, 30L), highContrastColor: global::Doroti.Ui.Color.fromARGB(255L, 235L, 235L, 240L), darkHighContrastColor: global::Doroti.Ui.Color.fromARGB(255L, 36L, 36L, 38L), elevatedColor: global::Doroti.Ui.Color.fromARGB(255L, 242L, 242L, 247L), darkElevatedColor: global::Doroti.Ui.Color.fromARGB(255L, 44L, 44L, 46L), highContrastElevatedColor: global::Doroti.Ui.Color.fromARGB(255L, 235L, 235L, 240L), darkHighContrastElevatedColor: global::Doroti.Ui.Color.fromARGB(255L, 54L, 54L, 56L));
    public static CupertinoDynamicColor tertiarySystemBackground = new CupertinoDynamicColor(debugLabel: "tertiarySystemBackground", color: global::Doroti.Ui.Color.fromARGB(255L, 255L, 255L, 255L), darkColor: global::Doroti.Ui.Color.fromARGB(255L, 44L, 44L, 46L), highContrastColor: global::Doroti.Ui.Color.fromARGB(255L, 255L, 255L, 255L), darkHighContrastColor: global::Doroti.Ui.Color.fromARGB(255L, 54L, 54L, 56L), elevatedColor: global::Doroti.Ui.Color.fromARGB(255L, 255L, 255L, 255L), darkElevatedColor: global::Doroti.Ui.Color.fromARGB(255L, 58L, 58L, 60L), highContrastElevatedColor: global::Doroti.Ui.Color.fromARGB(255L, 255L, 255L, 255L), darkHighContrastElevatedColor: global::Doroti.Ui.Color.fromARGB(255L, 68L, 68L, 70L));
    public static CupertinoDynamicColor systemGroupedBackground = new CupertinoDynamicColor(debugLabel: "systemGroupedBackground", color: global::Doroti.Ui.Color.fromARGB(255L, 242L, 242L, 247L), darkColor: global::Doroti.Ui.Color.fromARGB(255L, 0L, 0L, 0L), highContrastColor: global::Doroti.Ui.Color.fromARGB(255L, 235L, 235L, 240L), darkHighContrastColor: global::Doroti.Ui.Color.fromARGB(255L, 0L, 0L, 0L), elevatedColor: global::Doroti.Ui.Color.fromARGB(255L, 242L, 242L, 247L), darkElevatedColor: global::Doroti.Ui.Color.fromARGB(255L, 28L, 28L, 30L), highContrastElevatedColor: global::Doroti.Ui.Color.fromARGB(255L, 235L, 235L, 240L), darkHighContrastElevatedColor: global::Doroti.Ui.Color.fromARGB(255L, 36L, 36L, 38L));
    public static CupertinoDynamicColor secondarySystemGroupedBackground = new CupertinoDynamicColor(debugLabel: "secondarySystemGroupedBackground", color: global::Doroti.Ui.Color.fromARGB(255L, 255L, 255L, 255L), darkColor: global::Doroti.Ui.Color.fromARGB(255L, 28L, 28L, 30L), highContrastColor: global::Doroti.Ui.Color.fromARGB(255L, 255L, 255L, 255L), darkHighContrastColor: global::Doroti.Ui.Color.fromARGB(255L, 36L, 36L, 38L), elevatedColor: global::Doroti.Ui.Color.fromARGB(255L, 255L, 255L, 255L), darkElevatedColor: global::Doroti.Ui.Color.fromARGB(255L, 44L, 44L, 46L), highContrastElevatedColor: global::Doroti.Ui.Color.fromARGB(255L, 255L, 255L, 255L), darkHighContrastElevatedColor: global::Doroti.Ui.Color.fromARGB(255L, 54L, 54L, 56L));
    public static CupertinoDynamicColor tertiarySystemGroupedBackground = new CupertinoDynamicColor(debugLabel: "tertiarySystemGroupedBackground", color: global::Doroti.Ui.Color.fromARGB(255L, 242L, 242L, 247L), darkColor: global::Doroti.Ui.Color.fromARGB(255L, 44L, 44L, 46L), highContrastColor: global::Doroti.Ui.Color.fromARGB(255L, 235L, 235L, 240L), darkHighContrastColor: global::Doroti.Ui.Color.fromARGB(255L, 54L, 54L, 56L), elevatedColor: global::Doroti.Ui.Color.fromARGB(255L, 242L, 242L, 247L), darkElevatedColor: global::Doroti.Ui.Color.fromARGB(255L, 58L, 58L, 60L), highContrastElevatedColor: global::Doroti.Ui.Color.fromARGB(255L, 235L, 235L, 240L), darkHighContrastElevatedColor: global::Doroti.Ui.Color.fromARGB(255L, 68L, 68L, 70L));
    public static CupertinoDynamicColor separator = new CupertinoDynamicColor(debugLabel: "separator", color: global::Doroti.Ui.Color.fromARGB(73L, 60L, 60L, 67L), darkColor: global::Doroti.Ui.Color.fromARGB(153L, 84L, 84L, 88L), highContrastColor: global::Doroti.Ui.Color.fromARGB(94L, 60L, 60L, 67L), darkHighContrastColor: global::Doroti.Ui.Color.fromARGB(173L, 84L, 84L, 88L), elevatedColor: global::Doroti.Ui.Color.fromARGB(73L, 60L, 60L, 67L), darkElevatedColor: global::Doroti.Ui.Color.fromARGB(153L, 210L, 210L, 210L), highContrastElevatedColor: global::Doroti.Ui.Color.fromARGB(94L, 60L, 60L, 67L), darkHighContrastElevatedColor: global::Doroti.Ui.Color.fromARGB(173L, 84L, 84L, 88L));
    public static CupertinoDynamicColor opaqueSeparator = new CupertinoDynamicColor(debugLabel: "opaqueSeparator", color: global::Doroti.Ui.Color.fromARGB(255L, 198L, 198L, 200L), darkColor: global::Doroti.Ui.Color.fromARGB(255L, 56L, 56L, 58L), highContrastColor: global::Doroti.Ui.Color.fromARGB(255L, 198L, 198L, 200L), darkHighContrastColor: global::Doroti.Ui.Color.fromARGB(255L, 56L, 56L, 58L), elevatedColor: global::Doroti.Ui.Color.fromARGB(255L, 198L, 198L, 200L), darkElevatedColor: global::Doroti.Ui.Color.fromARGB(255L, 56L, 56L, 58L), highContrastElevatedColor: global::Doroti.Ui.Color.fromARGB(255L, 198L, 198L, 200L), darkHighContrastElevatedColor: global::Doroti.Ui.Color.fromARGB(255L, 56L, 56L, 58L));
    public static CupertinoDynamicColor link = new CupertinoDynamicColor(debugLabel: "link", color: global::Doroti.Ui.Color.fromARGB(255L, 0L, 122L, 255L), darkColor: global::Doroti.Ui.Color.fromARGB(255L, 9L, 132L, 255L), highContrastColor: global::Doroti.Ui.Color.fromARGB(255L, 0L, 122L, 255L), darkHighContrastColor: global::Doroti.Ui.Color.fromARGB(255L, 9L, 132L, 255L), elevatedColor: global::Doroti.Ui.Color.fromARGB(255L, 0L, 122L, 255L), darkElevatedColor: global::Doroti.Ui.Color.fromARGB(255L, 9L, 132L, 255L), highContrastElevatedColor: global::Doroti.Ui.Color.fromARGB(255L, 0L, 122L, 255L), darkHighContrastElevatedColor: global::Doroti.Ui.Color.fromARGB(255L, 9L, 132L, 255L));

}

public class CupertinoDynamicColor : Color, global::Doroti.Framework.Foundation.Diagnosticable
{
    internal virtual Color _effectiveColor { get; private set; } = default!;
    internal virtual string? _debugLabel { get; private set; }
    internal virtual global::Doroti.Framework.Widgets.Element? _debugResolveContext { get; private set; }
    public virtual Color color { get; private set; } = default!;
    public virtual Color darkColor { get; private set; } = default!;
    public virtual Color highContrastColor { get; private set; } = default!;
    public virtual Color darkHighContrastColor { get; private set; } = default!;
    public virtual Color elevatedColor { get; private set; } = default!;
    public virtual Color darkElevatedColor { get; private set; } = default!;
    public virtual Color highContrastElevatedColor { get; private set; } = default!;
    public virtual Color darkHighContrastElevatedColor { get; private set; } = default!;

    public CupertinoDynamicColor(string? debugLabel = null, Color color = default!, Color darkColor = default!, Color? highContrastColor = null, Color? darkHighContrastColor = null, Color? elevatedColor = null, Color? darkElevatedColor = null, Color? highContrastElevatedColor = null, Color? darkHighContrastElevatedColor = null) : this(
        color,
        color,
        darkColor,
        highContrastColor ?? color,
        darkHighContrastColor ?? darkColor,
        elevatedColor ?? color,
        darkElevatedColor ?? darkColor,
        highContrastElevatedColor ?? highContrastColor ?? elevatedColor ?? color,
        darkHighContrastElevatedColor ?? darkHighContrastColor ?? darkElevatedColor ?? darkColor,
        null,
        debugLabel)
    {
    }

    public static CupertinoDynamicColor CreateWithBrightnessAndContrast(string? debugLabel = null, Color color = default!, Color darkColor = default!, Color highContrastColor = default!, Color darkHighContrastColor = default!)
    {
        return new CupertinoDynamicColor(debugLabel: debugLabel, color: color, darkColor: darkColor, highContrastColor: highContrastColor, darkHighContrastColor: darkHighContrastColor, elevatedColor: color, darkElevatedColor: darkColor, highContrastElevatedColor: highContrastColor, darkHighContrastElevatedColor: darkHighContrastColor);
    }

    public static CupertinoDynamicColor CreateWithBrightness(string? debugLabel = null, Color color = default!, Color darkColor = default!)
    {
        return new CupertinoDynamicColor(debugLabel: debugLabel, color: color, darkColor: darkColor, highContrastColor: color, darkHighContrastColor: darkColor, elevatedColor: color, darkElevatedColor: darkColor, highContrastElevatedColor: color, darkHighContrastElevatedColor: darkColor);
    }

    public CupertinoDynamicColor(Color _effectiveColor, Color color, Color darkColor, Color highContrastColor, Color darkHighContrastColor, Color elevatedColor, Color darkElevatedColor, Color highContrastElevatedColor, Color darkHighContrastElevatedColor, global::Doroti.Framework.Widgets.Element? _debugResolveContext, string? _debugLabel)
    {
        this._effectiveColor = _effectiveColor;
        this.color = color;
        this.darkColor = darkColor;
        this.highContrastColor = highContrastColor;
        this.darkHighContrastColor = darkHighContrastColor;
        this.elevatedColor = elevatedColor;
        this.darkElevatedColor = darkElevatedColor;
        this.highContrastElevatedColor = highContrastElevatedColor;
        this.darkHighContrastElevatedColor = darkHighContrastElevatedColor;
        this._debugResolveContext = _debugResolveContext;
        this._debugLabel = _debugLabel;
    }

    public static global::Doroti.Ui.Color resolve(Color resolvable, global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Ui.Color)(object?)(((resolvable is CupertinoDynamicColor)) ? ((CupertinoDynamicColor)resolvable).resolveFrom(context) : resolvable));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static global::Doroti.Ui.Color? maybeResolve(Color? resolvable, global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Ui.Color?)(object?)(((resolvable is CupertinoDynamicColor)) ? ((CupertinoDynamicColor)resolvable).resolveFrom(context) : resolvable));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _isPlatformBrightnessDependent
    {
        get
        {
            return ((((!object.Equals(this.color, this.darkColor)) || (!object.Equals(this.elevatedColor, this.darkElevatedColor))) || (!object.Equals(this.highContrastColor, this.darkHighContrastColor))) || (!object.Equals(this.highContrastElevatedColor, this.darkHighContrastElevatedColor)));
            return default!;
        }
    }
    internal virtual bool _isHighContrastDependent
    {
        get
        {
            return ((((!object.Equals(this.color, this.highContrastColor)) || (!object.Equals(this.darkColor, this.darkHighContrastColor))) || (!object.Equals(this.elevatedColor, this.highContrastElevatedColor))) || (!object.Equals(this.darkElevatedColor, this.darkHighContrastElevatedColor)));
            return default!;
        }
    }
    internal virtual bool _isInterfaceElevationDependent
    {
        get
        {
            return ((((!object.Equals(this.color, this.elevatedColor)) || (!object.Equals(this.darkColor, this.darkElevatedColor))) || (!object.Equals(this.highContrastColor, this.highContrastElevatedColor))) || (!object.Equals(this.darkHighContrastColor, this.darkHighContrastElevatedColor)));
            return default!;
        }
    }
    public virtual CupertinoDynamicColor resolveFrom(global::Doroti.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Ui.Brightness brightness = (this._isPlatformBrightnessDependent ? (CupertinoTheme.maybeBrightnessOf(context) ?? Brightness.light) : Brightness.light);
        CupertinoUserInterfaceLevelData level = (this._isInterfaceElevationDependent ? (CupertinoUserInterfaceLevel.maybeOf(context) ?? CupertinoUserInterfaceLevelData.@base) : CupertinoUserInterfaceLevelData.@base);
        bool highContrast = (this._isHighContrastDependent && ((MediaQuery.maybeHighContrastOf(context) ?? false)));
        global::Doroti.Ui.Color resolved = ((global::Doroti.Ui.Color)(object?)((brightness, level, highContrast) switch { (Brightness.light, var __constant47383, false) when (object.Equals(__constant47383, CupertinoUserInterfaceLevelData.@base)) => this.color, (Brightness.light, var __constant47463, true) when (object.Equals(__constant47463, CupertinoUserInterfaceLevelData.@base)) => this.highContrastColor, (Brightness.light, var __constant47554, false) when (object.Equals(__constant47554, CupertinoUserInterfaceLevelData.elevated)) => this.elevatedColor, (Brightness.light, var __constant47646, true) when (object.Equals(__constant47646, CupertinoUserInterfaceLevelData.elevated)) => this.highContrastElevatedColor, (Brightness.dark, var __constant47756, false) when (object.Equals(__constant47756, CupertinoUserInterfaceLevelData.@base)) => this.darkColor, (Brightness.dark, var __constant47839, true) when (object.Equals(__constant47839, CupertinoUserInterfaceLevelData.@base)) => this.darkHighContrastColor, (Brightness.dark, var __constant47933, false) when (object.Equals(__constant47933, CupertinoUserInterfaceLevelData.elevated)) => this.darkElevatedColor, (Brightness.dark, var __constant48028, true) when (object.Equals(__constant48028, CupertinoUserInterfaceLevelData.elevated)) => this.darkHighContrastElevatedColor, _ => this.color }));
        global::Doroti.Framework.Widgets.Element? debugContext = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                debugContext = ((global::Doroti.Framework.Widgets.Element?)(object?)context)!;
                return true;
            });
        return new CupertinoDynamicColor(resolved, this.color, this.darkColor, this.highContrastColor, this.darkHighContrastColor, this.elevatedColor, this.darkElevatedColor, this.highContrastElevatedColor, this.darkHighContrastElevatedColor, debugContext, this._debugLabel);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as CupertinoDynamicColor;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((((((((((__other is CupertinoDynamicColor) && (((CupertinoDynamicColor)((CupertinoDynamicColor)__other)).value == this.value)) && (object.Equals(((CupertinoDynamicColor)((CupertinoDynamicColor)__other)).color, this.color))) && (object.Equals(((CupertinoDynamicColor)((CupertinoDynamicColor)__other)).darkColor, this.darkColor))) && (object.Equals(((CupertinoDynamicColor)((CupertinoDynamicColor)__other)).highContrastColor, this.highContrastColor))) && (object.Equals(((CupertinoDynamicColor)((CupertinoDynamicColor)__other)).darkHighContrastColor, this.darkHighContrastColor))) && (object.Equals(((CupertinoDynamicColor)((CupertinoDynamicColor)__other)).elevatedColor, this.elevatedColor))) && (object.Equals(((CupertinoDynamicColor)((CupertinoDynamicColor)__other)).darkElevatedColor, this.darkElevatedColor))) && (object.Equals(((CupertinoDynamicColor)((CupertinoDynamicColor)__other)).highContrastElevatedColor, this.highContrastElevatedColor))) && (object.Equals(((CupertinoDynamicColor)((CupertinoDynamicColor)__other)).darkHighContrastElevatedColor, this.darkHighContrastElevatedColor)));
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.value, this.color, this.darkColor, this.highContrastColor, this.elevatedColor, this.darkElevatedColor, this.darkHighContrastColor, this.darkHighContrastElevatedColor, this.highContrastElevatedColor));
    public virtual string ToString(global::Doroti.Framework.Foundation.DiagnosticLevel minLevel = global::Doroti.Framework.Foundation.DiagnosticLevel.info)
    {
        string toString(string name, Color color)
        {
            var marker = ((object.Equals(color, this._effectiveColor)) ? "*" : "");
            return $"{marker}{name} = {color}{marker}";
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        string ToString(string name, Color color) => toString(name, color);
        var xs = ((Func<List<string>>)(() => { var __collection49750 = new List<string>(); __collection49750.Add(toString("color", this.color)); if (this._isPlatformBrightnessDependent) { __collection49750.Add(ToString("darkColor", this.darkColor)); } if (this._isHighContrastDependent) { __collection49750.Add(ToString("highContrastColor", this.highContrastColor)); } if ((this._isPlatformBrightnessDependent && this._isHighContrastDependent)) { __collection49750.Add(ToString("darkHighContrastColor", this.darkHighContrastColor)); } if (this._isInterfaceElevationDependent) { __collection49750.Add(ToString("elevatedColor", this.elevatedColor)); } if ((this._isPlatformBrightnessDependent && this._isInterfaceElevationDependent)) { __collection49750.Add(ToString("darkElevatedColor", this.darkElevatedColor)); } if ((this._isHighContrastDependent && this._isInterfaceElevationDependent)) { __collection49750.Add(ToString("highContrastElevatedColor", this.highContrastElevatedColor)); } if (((this._isPlatformBrightnessDependent && this._isHighContrastDependent) && this._isInterfaceElevationDependent)) { __collection49750.Add(ToString("darkHighContrastElevatedColor", this.darkHighContrastElevatedColor)); } return __collection49750; }))();
        return $"{((this._debugLabel ?? global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "CupertinoDynamicColor")))}({string.Join(", ", xs)}, resolved by: {(((object?)this._debugResolveContext?.widget ?? (object?)"UNRESOLVED"))})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        if ((this._debugLabel is not null))
        {
            properties.add(new global::Doroti.Framework.Foundation.MessageProperty("debugLabel", this._debugLabel));
        }
        properties.add(ColorsLibrary.createCupertinoColorProperty("color", this.color));
        if (this._isPlatformBrightnessDependent)
        {
            properties.add(ColorsLibrary.createCupertinoColorProperty("darkColor", this.darkColor));
        }
        if (this._isHighContrastDependent)
        {
            properties.add(ColorsLibrary.createCupertinoColorProperty("highContrastColor", this.highContrastColor));
        }
        if ((this._isPlatformBrightnessDependent && this._isHighContrastDependent))
        {
            properties.add(ColorsLibrary.createCupertinoColorProperty("darkHighContrastColor", this.darkHighContrastColor));
        }
        if (this._isInterfaceElevationDependent)
        {
            properties.add(ColorsLibrary.createCupertinoColorProperty("elevatedColor", this.elevatedColor));
        }
        if ((this._isPlatformBrightnessDependent && this._isInterfaceElevationDependent))
        {
            properties.add(ColorsLibrary.createCupertinoColorProperty("darkElevatedColor", this.darkElevatedColor));
        }
        if ((this._isHighContrastDependent && this._isInterfaceElevationDependent))
        {
            properties.add(ColorsLibrary.createCupertinoColorProperty("highContrastElevatedColor", this.highContrastElevatedColor));
        }
        if (((this._isPlatformBrightnessDependent && this._isHighContrastDependent) && this._isInterfaceElevationDependent))
        {
            properties.add(ColorsLibrary.createCupertinoColorProperty("darkHighContrastElevatedColor", this.darkHighContrastElevatedColor));
        }
        if ((this._debugResolveContext is not null))
        {
            properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.Element>("last resolved", this._debugResolveContext));
        }
    }

    public virtual long value => this._effectiveColor.value;
    public virtual long toARGB32() => this._effectiveColor.toARGB32();
    public virtual long alpha => this._effectiveColor.alpha;
    public virtual long blue => this._effectiveColor.blue;
    public virtual double computeLuminance() => this._effectiveColor.computeLuminance();
    public virtual long green => this._effectiveColor.green;
    public virtual double opacity => this._effectiveColor.opacity;
    public virtual long red => this._effectiveColor.red;
    public virtual global::Doroti.Ui.Color withAlpha(long a) => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._effectiveColor.withAlpha(a));
    public virtual global::Doroti.Ui.Color withBlue(long b) => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._effectiveColor.withBlue(b));
    public virtual global::Doroti.Ui.Color withGreen(long g) => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._effectiveColor.withGreen(g));
    public virtual global::Doroti.Ui.Color withOpacity(double opacity) => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._effectiveColor.withOpacity(opacity));
    public virtual global::Doroti.Ui.Color withRed(long r) => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._effectiveColor.withRed(r));
    public virtual double a => this._effectiveColor.a;
    public virtual double r => this._effectiveColor.r;
    public virtual double g => this._effectiveColor.g;
    public virtual double b => this._effectiveColor.b;
    public virtual global::Doroti.Ui.ColorSpace colorSpace => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.ColorSpace>(this._effectiveColor.colorSpace);
    public virtual global::Doroti.Ui.Color withValues(double? alpha = null, double? red = null, double? green = null, double? blue = null, ColorSpace? colorSpace = null) => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._effectiveColor.withValues(alpha: alpha, red: red, green: green, blue: blue, colorSpace: colorSpace));
    public virtual string toStringShort() => global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
    public virtual DiagnosticsNode toDiagnosticsNode(string? name = null, DiagnosticsTreeStyle? style = null)
    {
        return ((DiagnosticsNode)(object?)new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public static partial class ColorsLibrary
{
    public static global::Doroti.Framework.Foundation.DiagnosticsProperty<Color> createCupertinoColorProperty(string name, Color? value, bool showName = true, object? defaultValue = default!, global::Doroti.Framework.Foundation.DiagnosticsTreeStyle style = global::Doroti.Framework.Foundation.DiagnosticsTreeStyle.singleLine, global::Doroti.Framework.Foundation.DiagnosticLevel level = global::Doroti.Framework.Foundation.DiagnosticLevel.info)
    {
        if ((value is CupertinoDynamicColor))
        {
            CupertinoDynamicColor value__as55108 = (CupertinoDynamicColor)value;
            return ((global::Doroti.Framework.Foundation.DiagnosticsProperty<Color>)(object?)new global::Doroti.Framework.Foundation.DiagnosticsProperty<CupertinoDynamicColor>(name, value__as55108, description: ((CupertinoDynamicColor)value__as55108)._debugLabel, showName: showName, defaultValue: defaultValue, style: style, level: level));
        }
        else
        {
            return ((global::Doroti.Framework.Foundation.DiagnosticsProperty<Color>)(object?)new global::Doroti.Framework.Painting.ColorProperty(name, value, showName: showName, defaultValue: defaultValue, style: style, level: level));
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}
