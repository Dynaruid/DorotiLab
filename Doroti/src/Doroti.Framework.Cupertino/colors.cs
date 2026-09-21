// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/colors.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Cupertino;

public abstract class CupertinoColors
{
    public static CupertinoDynamicColor activeBlue => systemBlue;
    public static CupertinoDynamicColor activeGreen => systemGreen;
    public static CupertinoDynamicColor activeOrange => systemOrange;
    public static Color white = new Color(4294967295L);
    public static Color black = new Color(4278190080L);
    public static Color transparent = new Color(0L);
    public static Color lightBackgroundGray = new Color(4293256682L);
    public static Color extraLightBackgroundGray = new Color(4293914612L);
    public static Color darkBackgroundGray = new Color(4279703319L);
    public static CupertinoDynamicColor inactiveGray = CupertinoDynamicColor.CreateWithBrightness(
        debugLabel: "inactiveGray",
        color: new Color(4288256409L),
        darkColor: new Color(4285887861L)
    );
    public static CupertinoDynamicColor destructiveRed => systemRed;
    public static CupertinoDynamicColor systemBlue =
        CupertinoDynamicColor.CreateWithBrightnessAndContrast(
            debugLabel: "systemBlue",
            color: Color.fromARGB(255L, 0L, 122L, 255L),
            darkColor: Color.fromARGB(255L, 10L, 132L, 255L),
            highContrastColor: Color.fromARGB(255L, 0L, 64L, 221L),
            darkHighContrastColor: Color.fromARGB(255L, 64L, 156L, 255L)
        );
    public static CupertinoDynamicColor systemGreen =
        CupertinoDynamicColor.CreateWithBrightnessAndContrast(
            debugLabel: "systemGreen",
            color: Color.fromARGB(255L, 52L, 199L, 89L),
            darkColor: Color.fromARGB(255L, 48L, 209L, 88L),
            highContrastColor: Color.fromARGB(255L, 36L, 138L, 61L),
            darkHighContrastColor: Color.fromARGB(255L, 48L, 219L, 91L)
        );
    public static CupertinoDynamicColor systemMint =
        CupertinoDynamicColor.CreateWithBrightnessAndContrast(
            debugLabel: "systemMint",
            color: Color.fromARGB(255L, 0L, 199L, 190L),
            darkColor: Color.fromARGB(255L, 99L, 230L, 226L),
            highContrastColor: Color.fromARGB(255L, 12L, 129L, 123L),
            darkHighContrastColor: Color.fromARGB(255L, 102L, 212L, 207L)
        );
    public static CupertinoDynamicColor systemIndigo =
        CupertinoDynamicColor.CreateWithBrightnessAndContrast(
            debugLabel: "systemIndigo",
            color: Color.fromARGB(255L, 88L, 86L, 214L),
            darkColor: Color.fromARGB(255L, 94L, 92L, 230L),
            highContrastColor: Color.fromARGB(255L, 54L, 52L, 163L),
            darkHighContrastColor: Color.fromARGB(255L, 125L, 122L, 255L)
        );
    public static CupertinoDynamicColor systemOrange =
        CupertinoDynamicColor.CreateWithBrightnessAndContrast(
            debugLabel: "systemOrange",
            color: Color.fromARGB(255L, 255L, 149L, 0L),
            darkColor: Color.fromARGB(255L, 255L, 159L, 10L),
            highContrastColor: Color.fromARGB(255L, 201L, 52L, 0L),
            darkHighContrastColor: Color.fromARGB(255L, 255L, 179L, 64L)
        );
    public static CupertinoDynamicColor systemPink =
        CupertinoDynamicColor.CreateWithBrightnessAndContrast(
            debugLabel: "systemPink",
            color: Color.fromARGB(255L, 255L, 45L, 85L),
            darkColor: Color.fromARGB(255L, 255L, 55L, 95L),
            highContrastColor: Color.fromARGB(255L, 211L, 15L, 69L),
            darkHighContrastColor: Color.fromARGB(255L, 255L, 100L, 130L)
        );
    public static CupertinoDynamicColor systemBrown =
        CupertinoDynamicColor.CreateWithBrightnessAndContrast(
            debugLabel: "systemBrown",
            color: Color.fromARGB(255L, 162L, 132L, 94L),
            darkColor: Color.fromARGB(255L, 172L, 142L, 104L),
            highContrastColor: Color.fromARGB(255L, 127L, 101L, 69L),
            darkHighContrastColor: Color.fromARGB(255L, 181L, 148L, 105L)
        );
    public static CupertinoDynamicColor systemPurple =
        CupertinoDynamicColor.CreateWithBrightnessAndContrast(
            debugLabel: "systemPurple",
            color: Color.fromARGB(255L, 175L, 82L, 222L),
            darkColor: Color.fromARGB(255L, 191L, 90L, 242L),
            highContrastColor: Color.fromARGB(255L, 137L, 68L, 171L),
            darkHighContrastColor: Color.fromARGB(255L, 218L, 143L, 255L)
        );
    public static CupertinoDynamicColor systemRed =
        CupertinoDynamicColor.CreateWithBrightnessAndContrast(
            debugLabel: "systemRed",
            color: Color.fromARGB(255L, 255L, 59L, 48L),
            darkColor: Color.fromARGB(255L, 255L, 69L, 58L),
            highContrastColor: Color.fromARGB(255L, 215L, 0L, 21L),
            darkHighContrastColor: Color.fromARGB(255L, 255L, 105L, 97L)
        );
    public static CupertinoDynamicColor systemTeal =
        CupertinoDynamicColor.CreateWithBrightnessAndContrast(
            debugLabel: "systemTeal",
            color: Color.fromARGB(255L, 90L, 200L, 250L),
            darkColor: Color.fromARGB(255L, 100L, 210L, 255L),
            highContrastColor: Color.fromARGB(255L, 0L, 113L, 164L),
            darkHighContrastColor: Color.fromARGB(255L, 112L, 215L, 255L)
        );
    public static CupertinoDynamicColor systemCyan =
        CupertinoDynamicColor.CreateWithBrightnessAndContrast(
            debugLabel: "systemCyan",
            color: Color.fromARGB(255L, 50L, 173L, 230L),
            darkColor: Color.fromARGB(255L, 100L, 210L, 255L),
            highContrastColor: Color.fromARGB(255L, 0L, 113L, 164L),
            darkHighContrastColor: Color.fromARGB(255L, 112L, 215L, 255L)
        );
    public static CupertinoDynamicColor systemYellow =
        CupertinoDynamicColor.CreateWithBrightnessAndContrast(
            debugLabel: "systemYellow",
            color: Color.fromARGB(255L, 255L, 204L, 0L),
            darkColor: Color.fromARGB(255L, 255L, 214L, 10L),
            highContrastColor: Color.fromARGB(255L, 160L, 90L, 0L),
            darkHighContrastColor: Color.fromARGB(255L, 255L, 212L, 38L)
        );
    public static CupertinoDynamicColor systemGrey =
        CupertinoDynamicColor.CreateWithBrightnessAndContrast(
            debugLabel: "systemGrey",
            color: Color.fromARGB(255L, 142L, 142L, 147L),
            darkColor: Color.fromARGB(255L, 142L, 142L, 147L),
            highContrastColor: Color.fromARGB(255L, 108L, 108L, 112L),
            darkHighContrastColor: Color.fromARGB(255L, 174L, 174L, 178L)
        );
    public static CupertinoDynamicColor systemGrey2 =
        CupertinoDynamicColor.CreateWithBrightnessAndContrast(
            debugLabel: "systemGrey2",
            color: Color.fromARGB(255L, 174L, 174L, 178L),
            darkColor: Color.fromARGB(255L, 99L, 99L, 102L),
            highContrastColor: Color.fromARGB(255L, 142L, 142L, 147L),
            darkHighContrastColor: Color.fromARGB(255L, 124L, 124L, 128L)
        );
    public static CupertinoDynamicColor systemGrey3 =
        CupertinoDynamicColor.CreateWithBrightnessAndContrast(
            debugLabel: "systemGrey3",
            color: Color.fromARGB(255L, 199L, 199L, 204L),
            darkColor: Color.fromARGB(255L, 72L, 72L, 74L),
            highContrastColor: Color.fromARGB(255L, 174L, 174L, 178L),
            darkHighContrastColor: Color.fromARGB(255L, 84L, 84L, 86L)
        );
    public static CupertinoDynamicColor systemGrey4 =
        CupertinoDynamicColor.CreateWithBrightnessAndContrast(
            debugLabel: "systemGrey4",
            color: Color.fromARGB(255L, 209L, 209L, 214L),
            darkColor: Color.fromARGB(255L, 58L, 58L, 60L),
            highContrastColor: Color.fromARGB(255L, 188L, 188L, 192L),
            darkHighContrastColor: Color.fromARGB(255L, 68L, 68L, 70L)
        );
    public static CupertinoDynamicColor systemGrey5 =
        CupertinoDynamicColor.CreateWithBrightnessAndContrast(
            debugLabel: "systemGrey5",
            color: Color.fromARGB(255L, 229L, 229L, 234L),
            darkColor: Color.fromARGB(255L, 44L, 44L, 46L),
            highContrastColor: Color.fromARGB(255L, 216L, 216L, 220L),
            darkHighContrastColor: Color.fromARGB(255L, 54L, 54L, 56L)
        );
    public static CupertinoDynamicColor systemGrey6 =
        CupertinoDynamicColor.CreateWithBrightnessAndContrast(
            debugLabel: "systemGrey6",
            color: Color.fromARGB(255L, 242L, 242L, 247L),
            darkColor: Color.fromARGB(255L, 28L, 28L, 30L),
            highContrastColor: Color.fromARGB(255L, 235L, 235L, 240L),
            darkHighContrastColor: Color.fromARGB(255L, 36L, 36L, 38L)
        );
    public static CupertinoDynamicColor label = new CupertinoDynamicColor(
        debugLabel: "label",
        color: Color.fromARGB(255L, 0L, 0L, 0L),
        darkColor: Color.fromARGB(255L, 255L, 255L, 255L),
        highContrastColor: Color.fromARGB(255L, 0L, 0L, 0L),
        darkHighContrastColor: Color.fromARGB(255L, 255L, 255L, 255L),
        elevatedColor: Color.fromARGB(255L, 0L, 0L, 0L),
        darkElevatedColor: Color.fromARGB(255L, 255L, 255L, 255L),
        highContrastElevatedColor: Color.fromARGB(255L, 0L, 0L, 0L),
        darkHighContrastElevatedColor: Color.fromARGB(255L, 255L, 255L, 255L)
    );
    public static CupertinoDynamicColor secondaryLabel = new CupertinoDynamicColor(
        debugLabel: "secondaryLabel",
        color: Color.fromARGB(153L, 60L, 60L, 67L),
        darkColor: Color.fromARGB(153L, 235L, 235L, 245L),
        highContrastColor: Color.fromARGB(173L, 60L, 60L, 67L),
        darkHighContrastColor: Color.fromARGB(173L, 235L, 235L, 245L),
        elevatedColor: Color.fromARGB(153L, 60L, 60L, 67L),
        darkElevatedColor: Color.fromARGB(153L, 235L, 235L, 245L),
        highContrastElevatedColor: Color.fromARGB(173L, 60L, 60L, 67L),
        darkHighContrastElevatedColor: Color.fromARGB(173L, 235L, 235L, 245L)
    );
    public static CupertinoDynamicColor tertiaryLabel = new CupertinoDynamicColor(
        debugLabel: "tertiaryLabel",
        color: Color.fromARGB(76L, 60L, 60L, 67L),
        darkColor: Color.fromARGB(76L, 235L, 235L, 245L),
        highContrastColor: Color.fromARGB(96L, 60L, 60L, 67L),
        darkHighContrastColor: Color.fromARGB(96L, 235L, 235L, 245L),
        elevatedColor: Color.fromARGB(76L, 60L, 60L, 67L),
        darkElevatedColor: Color.fromARGB(76L, 235L, 235L, 245L),
        highContrastElevatedColor: Color.fromARGB(96L, 60L, 60L, 67L),
        darkHighContrastElevatedColor: Color.fromARGB(96L, 235L, 235L, 245L)
    );
    public static CupertinoDynamicColor quaternaryLabel = new CupertinoDynamicColor(
        debugLabel: "quaternaryLabel",
        color: Color.fromARGB(45L, 60L, 60L, 67L),
        darkColor: Color.fromARGB(40L, 235L, 235L, 245L),
        highContrastColor: Color.fromARGB(66L, 60L, 60L, 67L),
        darkHighContrastColor: Color.fromARGB(61L, 235L, 235L, 245L),
        elevatedColor: Color.fromARGB(45L, 60L, 60L, 67L),
        darkElevatedColor: Color.fromARGB(40L, 235L, 235L, 245L),
        highContrastElevatedColor: Color.fromARGB(66L, 60L, 60L, 67L),
        darkHighContrastElevatedColor: Color.fromARGB(61L, 235L, 235L, 245L)
    );
    public static CupertinoDynamicColor systemFill = new CupertinoDynamicColor(
        debugLabel: "systemFill",
        color: Color.fromARGB(51L, 120L, 120L, 128L),
        darkColor: Color.fromARGB(91L, 120L, 120L, 128L),
        highContrastColor: Color.fromARGB(71L, 120L, 120L, 128L),
        darkHighContrastColor: Color.fromARGB(112L, 120L, 120L, 128L),
        elevatedColor: Color.fromARGB(51L, 120L, 120L, 128L),
        darkElevatedColor: Color.fromARGB(91L, 120L, 120L, 128L),
        highContrastElevatedColor: Color.fromARGB(71L, 120L, 120L, 128L),
        darkHighContrastElevatedColor: Color.fromARGB(112L, 120L, 120L, 128L)
    );
    public static CupertinoDynamicColor secondarySystemFill = new CupertinoDynamicColor(
        debugLabel: "secondarySystemFill",
        color: Color.fromARGB(40L, 120L, 120L, 128L),
        darkColor: Color.fromARGB(81L, 120L, 120L, 128L),
        highContrastColor: Color.fromARGB(61L, 120L, 120L, 128L),
        darkHighContrastColor: Color.fromARGB(102L, 120L, 120L, 128L),
        elevatedColor: Color.fromARGB(40L, 120L, 120L, 128L),
        darkElevatedColor: Color.fromARGB(81L, 120L, 120L, 128L),
        highContrastElevatedColor: Color.fromARGB(61L, 120L, 120L, 128L),
        darkHighContrastElevatedColor: Color.fromARGB(102L, 120L, 120L, 128L)
    );
    public static CupertinoDynamicColor tertiarySystemFill = new CupertinoDynamicColor(
        debugLabel: "tertiarySystemFill",
        color: Color.fromARGB(30L, 118L, 118L, 128L),
        darkColor: Color.fromARGB(61L, 118L, 118L, 128L),
        highContrastColor: Color.fromARGB(51L, 118L, 118L, 128L),
        darkHighContrastColor: Color.fromARGB(81L, 118L, 118L, 128L),
        elevatedColor: Color.fromARGB(30L, 118L, 118L, 128L),
        darkElevatedColor: Color.fromARGB(61L, 118L, 118L, 128L),
        highContrastElevatedColor: Color.fromARGB(51L, 118L, 118L, 128L),
        darkHighContrastElevatedColor: Color.fromARGB(81L, 118L, 118L, 128L)
    );
    public static CupertinoDynamicColor quaternarySystemFill = new CupertinoDynamicColor(
        debugLabel: "quaternarySystemFill",
        color: Color.fromARGB(20L, 116L, 116L, 128L),
        darkColor: Color.fromARGB(45L, 118L, 118L, 128L),
        highContrastColor: Color.fromARGB(40L, 116L, 116L, 128L),
        darkHighContrastColor: Color.fromARGB(66L, 118L, 118L, 128L),
        elevatedColor: Color.fromARGB(20L, 116L, 116L, 128L),
        darkElevatedColor: Color.fromARGB(45L, 118L, 118L, 128L),
        highContrastElevatedColor: Color.fromARGB(40L, 116L, 116L, 128L),
        darkHighContrastElevatedColor: Color.fromARGB(66L, 118L, 118L, 128L)
    );
    public static CupertinoDynamicColor placeholderText = new CupertinoDynamicColor(
        debugLabel: "placeholderText",
        color: Color.fromARGB(76L, 60L, 60L, 67L),
        darkColor: Color.fromARGB(76L, 235L, 235L, 245L),
        highContrastColor: Color.fromARGB(96L, 60L, 60L, 67L),
        darkHighContrastColor: Color.fromARGB(96L, 235L, 235L, 245L),
        elevatedColor: Color.fromARGB(76L, 60L, 60L, 67L),
        darkElevatedColor: Color.fromARGB(76L, 235L, 235L, 245L),
        highContrastElevatedColor: Color.fromARGB(96L, 60L, 60L, 67L),
        darkHighContrastElevatedColor: Color.fromARGB(96L, 235L, 235L, 245L)
    );
    public static CupertinoDynamicColor systemBackground = new CupertinoDynamicColor(
        debugLabel: "systemBackground",
        color: Color.fromARGB(255L, 255L, 255L, 255L),
        darkColor: Color.fromARGB(255L, 0L, 0L, 0L),
        highContrastColor: Color.fromARGB(255L, 255L, 255L, 255L),
        darkHighContrastColor: Color.fromARGB(255L, 0L, 0L, 0L),
        elevatedColor: Color.fromARGB(255L, 255L, 255L, 255L),
        darkElevatedColor: Color.fromARGB(255L, 28L, 28L, 30L),
        highContrastElevatedColor: Color.fromARGB(255L, 255L, 255L, 255L),
        darkHighContrastElevatedColor: Color.fromARGB(255L, 36L, 36L, 38L)
    );
    public static CupertinoDynamicColor secondarySystemBackground = new CupertinoDynamicColor(
        debugLabel: "secondarySystemBackground",
        color: Color.fromARGB(255L, 242L, 242L, 247L),
        darkColor: Color.fromARGB(255L, 28L, 28L, 30L),
        highContrastColor: Color.fromARGB(255L, 235L, 235L, 240L),
        darkHighContrastColor: Color.fromARGB(255L, 36L, 36L, 38L),
        elevatedColor: Color.fromARGB(255L, 242L, 242L, 247L),
        darkElevatedColor: Color.fromARGB(255L, 44L, 44L, 46L),
        highContrastElevatedColor: Color.fromARGB(255L, 235L, 235L, 240L),
        darkHighContrastElevatedColor: Color.fromARGB(255L, 54L, 54L, 56L)
    );
    public static CupertinoDynamicColor tertiarySystemBackground = new CupertinoDynamicColor(
        debugLabel: "tertiarySystemBackground",
        color: Color.fromARGB(255L, 255L, 255L, 255L),
        darkColor: Color.fromARGB(255L, 44L, 44L, 46L),
        highContrastColor: Color.fromARGB(255L, 255L, 255L, 255L),
        darkHighContrastColor: Color.fromARGB(255L, 54L, 54L, 56L),
        elevatedColor: Color.fromARGB(255L, 255L, 255L, 255L),
        darkElevatedColor: Color.fromARGB(255L, 58L, 58L, 60L),
        highContrastElevatedColor: Color.fromARGB(255L, 255L, 255L, 255L),
        darkHighContrastElevatedColor: Color.fromARGB(255L, 68L, 68L, 70L)
    );
    public static CupertinoDynamicColor systemGroupedBackground = new CupertinoDynamicColor(
        debugLabel: "systemGroupedBackground",
        color: Color.fromARGB(255L, 242L, 242L, 247L),
        darkColor: Color.fromARGB(255L, 0L, 0L, 0L),
        highContrastColor: Color.fromARGB(255L, 235L, 235L, 240L),
        darkHighContrastColor: Color.fromARGB(255L, 0L, 0L, 0L),
        elevatedColor: Color.fromARGB(255L, 242L, 242L, 247L),
        darkElevatedColor: Color.fromARGB(255L, 28L, 28L, 30L),
        highContrastElevatedColor: Color.fromARGB(255L, 235L, 235L, 240L),
        darkHighContrastElevatedColor: Color.fromARGB(255L, 36L, 36L, 38L)
    );
    public static CupertinoDynamicColor secondarySystemGroupedBackground =
        new CupertinoDynamicColor(
            debugLabel: "secondarySystemGroupedBackground",
            color: Color.fromARGB(255L, 255L, 255L, 255L),
            darkColor: Color.fromARGB(255L, 28L, 28L, 30L),
            highContrastColor: Color.fromARGB(255L, 255L, 255L, 255L),
            darkHighContrastColor: Color.fromARGB(255L, 36L, 36L, 38L),
            elevatedColor: Color.fromARGB(255L, 255L, 255L, 255L),
            darkElevatedColor: Color.fromARGB(255L, 44L, 44L, 46L),
            highContrastElevatedColor: Color.fromARGB(255L, 255L, 255L, 255L),
            darkHighContrastElevatedColor: Color.fromARGB(255L, 54L, 54L, 56L)
        );
    public static CupertinoDynamicColor tertiarySystemGroupedBackground = new CupertinoDynamicColor(
        debugLabel: "tertiarySystemGroupedBackground",
        color: Color.fromARGB(255L, 242L, 242L, 247L),
        darkColor: Color.fromARGB(255L, 44L, 44L, 46L),
        highContrastColor: Color.fromARGB(255L, 235L, 235L, 240L),
        darkHighContrastColor: Color.fromARGB(255L, 54L, 54L, 56L),
        elevatedColor: Color.fromARGB(255L, 242L, 242L, 247L),
        darkElevatedColor: Color.fromARGB(255L, 58L, 58L, 60L),
        highContrastElevatedColor: Color.fromARGB(255L, 235L, 235L, 240L),
        darkHighContrastElevatedColor: Color.fromARGB(255L, 68L, 68L, 70L)
    );
    public static CupertinoDynamicColor separator = new CupertinoDynamicColor(
        debugLabel: "separator",
        color: Color.fromARGB(73L, 60L, 60L, 67L),
        darkColor: Color.fromARGB(153L, 84L, 84L, 88L),
        highContrastColor: Color.fromARGB(94L, 60L, 60L, 67L),
        darkHighContrastColor: Color.fromARGB(173L, 84L, 84L, 88L),
        elevatedColor: Color.fromARGB(73L, 60L, 60L, 67L),
        darkElevatedColor: Color.fromARGB(153L, 210L, 210L, 210L),
        highContrastElevatedColor: Color.fromARGB(94L, 60L, 60L, 67L),
        darkHighContrastElevatedColor: Color.fromARGB(173L, 84L, 84L, 88L)
    );
    public static CupertinoDynamicColor opaqueSeparator = new CupertinoDynamicColor(
        debugLabel: "opaqueSeparator",
        color: Color.fromARGB(255L, 198L, 198L, 200L),
        darkColor: Color.fromARGB(255L, 56L, 56L, 58L),
        highContrastColor: Color.fromARGB(255L, 198L, 198L, 200L),
        darkHighContrastColor: Color.fromARGB(255L, 56L, 56L, 58L),
        elevatedColor: Color.fromARGB(255L, 198L, 198L, 200L),
        darkElevatedColor: Color.fromARGB(255L, 56L, 56L, 58L),
        highContrastElevatedColor: Color.fromARGB(255L, 198L, 198L, 200L),
        darkHighContrastElevatedColor: Color.fromARGB(255L, 56L, 56L, 58L)
    );
    public static CupertinoDynamicColor link = new CupertinoDynamicColor(
        debugLabel: "link",
        color: Color.fromARGB(255L, 0L, 122L, 255L),
        darkColor: Color.fromARGB(255L, 9L, 132L, 255L),
        highContrastColor: Color.fromARGB(255L, 0L, 122L, 255L),
        darkHighContrastColor: Color.fromARGB(255L, 9L, 132L, 255L),
        elevatedColor: Color.fromARGB(255L, 0L, 122L, 255L),
        darkElevatedColor: Color.fromARGB(255L, 9L, 132L, 255L),
        highContrastElevatedColor: Color.fromARGB(255L, 0L, 122L, 255L),
        darkHighContrastElevatedColor: Color.fromARGB(255L, 9L, 132L, 255L)
    );
}

public class CupertinoDynamicColor : Color, Diagnosticable
{
    internal virtual Color _effectiveColor { get; private set; } = default!;
    internal virtual string? _debugLabel { get; private set; }
    internal virtual Element? _debugResolveContext { get; private set; }
    public virtual Color color { get; private set; } = default!;
    public virtual Color darkColor { get; private set; } = default!;
    public virtual Color highContrastColor { get; private set; } = default!;
    public virtual Color darkHighContrastColor { get; private set; } = default!;
    public virtual Color elevatedColor { get; private set; } = default!;
    public virtual Color darkElevatedColor { get; private set; } = default!;
    public virtual Color highContrastElevatedColor { get; private set; } = default!;
    public virtual Color darkHighContrastElevatedColor { get; private set; } = default!;

    public CupertinoDynamicColor(
        string? debugLabel = null,
        Color color = default!,
        Color darkColor = default!,
        Color? highContrastColor = null,
        Color? darkHighContrastColor = null,
        Color? elevatedColor = null,
        Color? darkElevatedColor = null,
        Color? highContrastElevatedColor = null,
        Color? darkHighContrastElevatedColor = null
    )
        : this(
            color,
            color,
            darkColor,
            highContrastColor ?? color,
            darkHighContrastColor ?? darkColor,
            elevatedColor ?? color,
            darkElevatedColor ?? darkColor,
            highContrastElevatedColor ?? highContrastColor ?? elevatedColor ?? color,
            darkHighContrastElevatedColor
                ?? darkHighContrastColor
                ?? darkElevatedColor
                ?? darkColor,
            null,
            debugLabel
        ) { }

    public static CupertinoDynamicColor CreateWithBrightnessAndContrast(
        string? debugLabel = null,
        Color color = default!,
        Color darkColor = default!,
        Color highContrastColor = default!,
        Color darkHighContrastColor = default!
    )
    {
        return new CupertinoDynamicColor(
            debugLabel: debugLabel,
            color: color,
            darkColor: darkColor,
            highContrastColor: highContrastColor,
            darkHighContrastColor: darkHighContrastColor,
            elevatedColor: color,
            darkElevatedColor: darkColor,
            highContrastElevatedColor: highContrastColor,
            darkHighContrastElevatedColor: darkHighContrastColor
        );
    }

    public static CupertinoDynamicColor CreateWithBrightness(
        string? debugLabel = null,
        Color color = default!,
        Color darkColor = default!
    )
    {
        return new CupertinoDynamicColor(
            debugLabel: debugLabel,
            color: color,
            darkColor: darkColor,
            highContrastColor: color,
            darkHighContrastColor: darkColor,
            elevatedColor: color,
            darkElevatedColor: darkColor,
            highContrastElevatedColor: color,
            darkHighContrastElevatedColor: darkColor
        );
    }

    public CupertinoDynamicColor(
        Color _effectiveColor,
        Color color,
        Color darkColor,
        Color highContrastColor,
        Color darkHighContrastColor,
        Color elevatedColor,
        Color darkElevatedColor,
        Color highContrastElevatedColor,
        Color darkHighContrastElevatedColor,
        Element? _debugResolveContext,
        string? _debugLabel
    )
        : base(
            _effectiveColor.a,
            _effectiveColor.r,
            _effectiveColor.g,
            _effectiveColor.b,
            _effectiveColor.colorSpace
        )
    {
        // Color channels are nonvirtual in Doroti.Ui. Populate the immutable
        // base value too, otherwise painting through Color sees transparent black.
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

    public static Color resolve(Color resolvable, BuildContext context)
    {
        return (resolvable is CupertinoDynamicColor)
            ? ((CupertinoDynamicColor)resolvable).resolveFrom(context)
            : resolvable;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static Color? maybeResolve(Color? resolvable, BuildContext context)
    {
        return (resolvable is CupertinoDynamicColor)
            ? ((CupertinoDynamicColor)resolvable).resolveFrom(context)
            : resolvable;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _isPlatformBrightnessDependent
    {
        get
        {
            return (!Equals(color, darkColor))
                || (!Equals(elevatedColor, darkElevatedColor))
                || (!Equals(highContrastColor, darkHighContrastColor))
                || (!Equals(highContrastElevatedColor, darkHighContrastElevatedColor));
        }
    }
    internal virtual bool _isHighContrastDependent
    {
        get
        {
            return (!Equals(color, highContrastColor))
                || (!Equals(darkColor, darkHighContrastColor))
                || (!Equals(elevatedColor, highContrastElevatedColor))
                || (!Equals(darkElevatedColor, darkHighContrastElevatedColor));
        }
    }
    internal virtual bool _isInterfaceElevationDependent
    {
        get
        {
            return (!Equals(color, elevatedColor))
                || (!Equals(darkColor, darkElevatedColor))
                || (!Equals(highContrastColor, highContrastElevatedColor))
                || (!Equals(darkHighContrastColor, darkHighContrastElevatedColor));
        }
    }

    public override Color resolveFrom<TContext>(TContext context) =>
        resolveFrom((BuildContext)(object)context!);

    public virtual CupertinoDynamicColor resolveFrom(BuildContext context)
    {
        Brightness brightness = _isPlatformBrightnessDependent
            ? (CupertinoTheme.maybeBrightnessOf(context) ?? Brightness.light)
            : Brightness.light;
        CupertinoUserInterfaceLevelData level = _isInterfaceElevationDependent
            ? (
                CupertinoUserInterfaceLevel.maybeOf(context)
                ?? CupertinoUserInterfaceLevelData.@base
            )
            : CupertinoUserInterfaceLevelData.@base;
        bool highContrast =
            _isHighContrastDependent && (MediaQuery.maybeHighContrastOf(context) ?? false);
        Color resolved = (brightness, level, highContrast) switch
        {
            (Brightness.light, var __constant47383, false)
                when Equals(__constant47383, CupertinoUserInterfaceLevelData.@base) => color,
            (Brightness.light, var __constant47463, true)
                when Equals(__constant47463, CupertinoUserInterfaceLevelData.@base) =>
                highContrastColor,
            (Brightness.light, var __constant47554, false)
                when Equals(__constant47554, CupertinoUserInterfaceLevelData.elevated) =>
                elevatedColor,
            (Brightness.light, var __constant47646, true)
                when Equals(__constant47646, CupertinoUserInterfaceLevelData.elevated) =>
                highContrastElevatedColor,
            (Brightness.dark, var __constant47756, false)
                when Equals(__constant47756, CupertinoUserInterfaceLevelData.@base) => darkColor,
            (Brightness.dark, var __constant47839, true)
                when Equals(__constant47839, CupertinoUserInterfaceLevelData.@base) =>
                darkHighContrastColor,
            (Brightness.dark, var __constant47933, false)
                when Equals(__constant47933, CupertinoUserInterfaceLevelData.elevated) =>
                darkElevatedColor,
            (Brightness.dark, var __constant48028, true)
                when Equals(__constant48028, CupertinoUserInterfaceLevelData.elevated) =>
                darkHighContrastElevatedColor,
            _ => color,
        };
        Element? debugContext = default!;
        DartRuntimePrimitives.Assert(() =>
        {
            debugContext = ((Element?)context)!;
            return true;
        });
        return new CupertinoDynamicColor(
            resolved,
            color,
            darkColor,
            highContrastColor,
            darkHighContrastColor,
            elevatedColor,
            darkElevatedColor,
            highContrastElevatedColor,
            darkHighContrastElevatedColor,
            debugContext,
            _debugLabel
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as CupertinoDynamicColor;
        if (__other is null)
        {
            return false;
        }

        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is CupertinoDynamicColor)
            && (__other.value == value)
            && Equals(__other.color, color)
            && Equals(__other.darkColor, darkColor)
            && Equals(__other.highContrastColor, highContrastColor)
            && Equals(__other.darkHighContrastColor, darkHighContrastColor)
            && Equals(__other.elevatedColor, elevatedColor)
            && Equals(__other.darkElevatedColor, darkElevatedColor)
            && Equals(__other.highContrastElevatedColor, highContrastElevatedColor)
            && Equals(__other.darkHighContrastElevatedColor, darkHighContrastElevatedColor);
    }

    public override int GetHashCode() =>
        DartRuntimePrimitives.ConvertValue<int>(
            FoundationRuntimePorts.ObjectHash(
                value,
                color,
                darkColor,
                highContrastColor,
                elevatedColor,
                darkElevatedColor,
                darkHighContrastColor,
                darkHighContrastElevatedColor,
                highContrastElevatedColor
            )
        );

    public override string ToString() => ToString(DiagnosticLevel.info);

    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string toString(string name, Color color)
        {
            var marker = Equals(color, _effectiveColor) ? "*" : "";
            return $"{marker}{name} = {color}{marker}";
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        string ToString(string name, Color color) => toString(name, color);
        var xs = (
            (Func<List<string>>)(
                () =>
                {
                    var __collection49750 = new List<string>();
                    __collection49750.Add(toString("color", color));
                    if (_isPlatformBrightnessDependent)
                    {
                        __collection49750.Add(ToString("darkColor", darkColor));
                    }
                    if (_isHighContrastDependent)
                    {
                        __collection49750.Add(ToString("highContrastColor", highContrastColor));
                    }
                    if (_isPlatformBrightnessDependent && _isHighContrastDependent)
                    {
                        __collection49750.Add(
                            ToString("darkHighContrastColor", darkHighContrastColor)
                        );
                    }
                    if (_isInterfaceElevationDependent)
                    {
                        __collection49750.Add(ToString("elevatedColor", elevatedColor));
                    }
                    if (_isPlatformBrightnessDependent && _isInterfaceElevationDependent)
                    {
                        __collection49750.Add(ToString("darkElevatedColor", darkElevatedColor));
                    }
                    if (_isHighContrastDependent && _isInterfaceElevationDependent)
                    {
                        __collection49750.Add(
                            ToString("highContrastElevatedColor", highContrastElevatedColor)
                        );
                    }
                    if (
                        _isPlatformBrightnessDependent
                        && _isHighContrastDependent
                        && _isInterfaceElevationDependent
                    )
                    {
                        __collection49750.Add(
                            ToString("darkHighContrastElevatedColor", darkHighContrastElevatedColor)
                        );
                    }
                    return __collection49750;
                }
            )
        )();
        return $"{_debugLabel ?? objectRuntimeTypeFunctions.objectRuntimeType(this, "CupertinoDynamicColor")}({string.Join(", ", xs)}, resolved by: {(object?)_debugResolveContext?.widget ?? (object?)"UNRESOLVED"})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        if (_debugLabel is not null)
        {
            properties.add(new MessageProperty("debugLabel", _debugLabel));
        }
        properties.add(ColorsLibrary.createCupertinoColorProperty("color", color));
        if (_isPlatformBrightnessDependent)
        {
            properties.add(ColorsLibrary.createCupertinoColorProperty("darkColor", darkColor));
        }
        if (_isHighContrastDependent)
        {
            properties.add(
                ColorsLibrary.createCupertinoColorProperty("highContrastColor", highContrastColor)
            );
        }
        if (_isPlatformBrightnessDependent && _isHighContrastDependent)
        {
            properties.add(
                ColorsLibrary.createCupertinoColorProperty(
                    "darkHighContrastColor",
                    darkHighContrastColor
                )
            );
        }
        if (_isInterfaceElevationDependent)
        {
            properties.add(
                ColorsLibrary.createCupertinoColorProperty("elevatedColor", elevatedColor)
            );
        }
        if (_isPlatformBrightnessDependent && _isInterfaceElevationDependent)
        {
            properties.add(
                ColorsLibrary.createCupertinoColorProperty("darkElevatedColor", darkElevatedColor)
            );
        }
        if (_isHighContrastDependent && _isInterfaceElevationDependent)
        {
            properties.add(
                ColorsLibrary.createCupertinoColorProperty(
                    "highContrastElevatedColor",
                    highContrastElevatedColor
                )
            );
        }
        if (
            _isPlatformBrightnessDependent
            && _isHighContrastDependent
            && _isInterfaceElevationDependent
        )
        {
            properties.add(
                ColorsLibrary.createCupertinoColorProperty(
                    "darkHighContrastElevatedColor",
                    darkHighContrastElevatedColor
                )
            );
        }
        if (_debugResolveContext is not null)
        {
            properties.add(new DiagnosticsProperty<Element>("last resolved", _debugResolveContext));
        }
    }

    public new virtual long value => _effectiveColor.value;

    public new virtual long toARGB32() => _effectiveColor.toARGB32();

    public new virtual long alpha => _effectiveColor.alpha;
    public new virtual long blue => _effectiveColor.blue;

    public new virtual double computeLuminance() => _effectiveColor.computeLuminance();

    public new virtual long green => _effectiveColor.green;
    public new virtual double opacity => _effectiveColor.opacity;
    public new virtual long red => _effectiveColor.red;

    public new virtual Color withAlpha(long a) =>
        DartRuntimePrimitives.ConvertValue<Color>(_effectiveColor.withAlpha(a));

    public new virtual Color withBlue(long b) =>
        DartRuntimePrimitives.ConvertValue<Color>(_effectiveColor.withBlue(b));

    public new virtual Color withGreen(long g) =>
        DartRuntimePrimitives.ConvertValue<Color>(_effectiveColor.withGreen(g));

    public new virtual Color withOpacity(double opacity) =>
        DartRuntimePrimitives.ConvertValue<Color>(_effectiveColor.withOpacity(opacity));

    public new virtual Color withRed(long r) =>
        DartRuntimePrimitives.ConvertValue<Color>(_effectiveColor.withRed(r));

    public new virtual double a => _effectiveColor.a;
    public new virtual double r => _effectiveColor.r;
    public new virtual double g => _effectiveColor.g;
    public new virtual double b => _effectiveColor.b;
    public new virtual ColorSpace colorSpace =>
        DartRuntimePrimitives.ConvertValue<ColorSpace>(_effectiveColor.colorSpace);

    public new virtual Color withValues(
        double? alpha = null,
        double? red = null,
        double? green = null,
        double? blue = null,
        ColorSpace? colorSpace = null
    ) =>
        DartRuntimePrimitives.ConvertValue<Color>(
            _effectiveColor.withValues(
                alpha: alpha,
                red: red,
                green: green,
                blue: blue,
                colorSpace: colorSpace
            )
        );

    public virtual string toStringShort() => DiagnosticsLibrary.describeIdentity(this);

    public virtual DiagnosticsNode toDiagnosticsNode(
        string? name = null,
        DiagnosticsTreeStyle? style = null
    )
    {
        return new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class ColorsLibrary
{
    public static DiagnosticsProperty<Color> createCupertinoColorProperty(
        string name,
        Color? value,
        bool showName = true,
        object? defaultValue = default!,
        DiagnosticsTreeStyle style = DiagnosticsTreeStyle.singleLine,
        DiagnosticLevel level = DiagnosticLevel.info
    )
    {
        if (value is CupertinoDynamicColor)
        {
            CupertinoDynamicColor value__as55108 = (CupertinoDynamicColor)value;
            return new DiagnosticsProperty<Color>(
                name,
                value__as55108,
                description: value__as55108._debugLabel,
                showName: showName,
                defaultValue: defaultValue,
                style: style,
                level: level
            );
        }
        else
        {
            return new ColorProperty(
                name,
                value,
                showName: showName,
                defaultValue: defaultValue,
                style: style,
                level: level
            );
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}
