// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/typography.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public enum ScriptCategory
{
    englishLike,
    dense,
    tall,
}

public class Typography : Diagnosticable
{
    public virtual TextTheme black { get; private set; } = default!;
    public virtual TextTheme white { get; private set; } = default!;
    public virtual TextTheme englishLike { get; private set; } = default!;
    public virtual TextTheme dense { get; private set; } = default!;
    public virtual TextTheme tall { get; private set; } = default!;
    public static TextTheme blackMountainView = new TextTheme(
        displayLarge: new TextStyle(
            debugLabel: "blackMountainView displayLarge",
            fontFamily: "Roboto",
            color: Colors.black54,
            decoration: TextDecoration.none
        ),
        displayMedium: new TextStyle(
            debugLabel: "blackMountainView displayMedium",
            fontFamily: "Roboto",
            color: Colors.black54,
            decoration: TextDecoration.none
        ),
        displaySmall: new TextStyle(
            debugLabel: "blackMountainView displaySmall",
            fontFamily: "Roboto",
            color: Colors.black54,
            decoration: TextDecoration.none
        ),
        headlineLarge: new TextStyle(
            debugLabel: "blackMountainView headlineLarge",
            fontFamily: "Roboto",
            color: Colors.black54,
            decoration: TextDecoration.none
        ),
        headlineMedium: new TextStyle(
            debugLabel: "blackMountainView headlineMedium",
            fontFamily: "Roboto",
            color: Colors.black54,
            decoration: TextDecoration.none
        ),
        headlineSmall: new TextStyle(
            debugLabel: "blackMountainView headlineSmall",
            fontFamily: "Roboto",
            color: Colors.black87,
            decoration: TextDecoration.none
        ),
        titleLarge: new TextStyle(
            debugLabel: "blackMountainView titleLarge",
            fontFamily: "Roboto",
            color: Colors.black87,
            decoration: TextDecoration.none
        ),
        titleMedium: new TextStyle(
            debugLabel: "blackMountainView titleMedium",
            fontFamily: "Roboto",
            color: Colors.black87,
            decoration: TextDecoration.none
        ),
        titleSmall: new TextStyle(
            debugLabel: "blackMountainView titleSmall",
            fontFamily: "Roboto",
            color: Colors.black,
            decoration: TextDecoration.none
        ),
        bodyLarge: new TextStyle(
            debugLabel: "blackMountainView bodyLarge",
            fontFamily: "Roboto",
            color: Colors.black87,
            decoration: TextDecoration.none
        ),
        bodyMedium: new TextStyle(
            debugLabel: "blackMountainView bodyMedium",
            fontFamily: "Roboto",
            color: Colors.black87,
            decoration: TextDecoration.none
        ),
        bodySmall: new TextStyle(
            debugLabel: "blackMountainView bodySmall",
            fontFamily: "Roboto",
            color: Colors.black54,
            decoration: TextDecoration.none
        ),
        labelLarge: new TextStyle(
            debugLabel: "blackMountainView labelLarge",
            fontFamily: "Roboto",
            color: Colors.black87,
            decoration: TextDecoration.none
        ),
        labelMedium: new TextStyle(
            debugLabel: "blackMountainView labelMedium",
            fontFamily: "Roboto",
            color: Colors.black,
            decoration: TextDecoration.none
        ),
        labelSmall: new TextStyle(
            debugLabel: "blackMountainView labelSmall",
            fontFamily: "Roboto",
            color: Colors.black,
            decoration: TextDecoration.none
        )
    );
    public static TextTheme whiteMountainView = new TextTheme(
        displayLarge: new TextStyle(
            debugLabel: "whiteMountainView displayLarge",
            fontFamily: "Roboto",
            color: Colors.white70,
            decoration: TextDecoration.none
        ),
        displayMedium: new TextStyle(
            debugLabel: "whiteMountainView displayMedium",
            fontFamily: "Roboto",
            color: Colors.white70,
            decoration: TextDecoration.none
        ),
        displaySmall: new TextStyle(
            debugLabel: "whiteMountainView displaySmall",
            fontFamily: "Roboto",
            color: Colors.white70,
            decoration: TextDecoration.none
        ),
        headlineLarge: new TextStyle(
            debugLabel: "whiteMountainView headlineLarge",
            fontFamily: "Roboto",
            color: Colors.white70,
            decoration: TextDecoration.none
        ),
        headlineMedium: new TextStyle(
            debugLabel: "whiteMountainView headlineMedium",
            fontFamily: "Roboto",
            color: Colors.white70,
            decoration: TextDecoration.none
        ),
        headlineSmall: new TextStyle(
            debugLabel: "whiteMountainView headlineSmall",
            fontFamily: "Roboto",
            color: Colors.white,
            decoration: TextDecoration.none
        ),
        titleLarge: new TextStyle(
            debugLabel: "whiteMountainView titleLarge",
            fontFamily: "Roboto",
            color: Colors.white,
            decoration: TextDecoration.none
        ),
        titleMedium: new TextStyle(
            debugLabel: "whiteMountainView titleMedium",
            fontFamily: "Roboto",
            color: Colors.white,
            decoration: TextDecoration.none
        ),
        titleSmall: new TextStyle(
            debugLabel: "whiteMountainView titleSmall",
            fontFamily: "Roboto",
            color: Colors.white,
            decoration: TextDecoration.none
        ),
        bodyLarge: new TextStyle(
            debugLabel: "whiteMountainView bodyLarge",
            fontFamily: "Roboto",
            color: Colors.white,
            decoration: TextDecoration.none
        ),
        bodyMedium: new TextStyle(
            debugLabel: "whiteMountainView bodyMedium",
            fontFamily: "Roboto",
            color: Colors.white,
            decoration: TextDecoration.none
        ),
        bodySmall: new TextStyle(
            debugLabel: "whiteMountainView bodySmall",
            fontFamily: "Roboto",
            color: Colors.white70,
            decoration: TextDecoration.none
        ),
        labelLarge: new TextStyle(
            debugLabel: "whiteMountainView labelLarge",
            fontFamily: "Roboto",
            color: Colors.white,
            decoration: TextDecoration.none
        ),
        labelMedium: new TextStyle(
            debugLabel: "whiteMountainView labelMedium",
            fontFamily: "Roboto",
            color: Colors.white,
            decoration: TextDecoration.none
        ),
        labelSmall: new TextStyle(
            debugLabel: "whiteMountainView labelSmall",
            fontFamily: "Roboto",
            color: Colors.white,
            decoration: TextDecoration.none
        )
    );
    public static TextTheme blackRedmond = new TextTheme(
        displayLarge: new TextStyle(
            debugLabel: "blackRedmond displayLarge",
            fontFamily: "Segoe UI",
            color: Colors.black54,
            decoration: TextDecoration.none
        ),
        displayMedium: new TextStyle(
            debugLabel: "blackRedmond displayMedium",
            fontFamily: "Segoe UI",
            color: Colors.black54,
            decoration: TextDecoration.none
        ),
        displaySmall: new TextStyle(
            debugLabel: "blackRedmond displaySmall",
            fontFamily: "Segoe UI",
            color: Colors.black54,
            decoration: TextDecoration.none
        ),
        headlineLarge: new TextStyle(
            debugLabel: "blackRedmond headlineLarge",
            fontFamily: "Segoe UI",
            color: Colors.black54,
            decoration: TextDecoration.none
        ),
        headlineMedium: new TextStyle(
            debugLabel: "blackRedmond headlineMedium",
            fontFamily: "Segoe UI",
            color: Colors.black54,
            decoration: TextDecoration.none
        ),
        headlineSmall: new TextStyle(
            debugLabel: "blackRedmond headlineSmall",
            fontFamily: "Segoe UI",
            color: Colors.black87,
            decoration: TextDecoration.none
        ),
        titleLarge: new TextStyle(
            debugLabel: "blackRedmond titleLarge",
            fontFamily: "Segoe UI",
            color: Colors.black87,
            decoration: TextDecoration.none
        ),
        titleMedium: new TextStyle(
            debugLabel: "blackRedmond titleMedium",
            fontFamily: "Segoe UI",
            color: Colors.black87,
            decoration: TextDecoration.none
        ),
        titleSmall: new TextStyle(
            debugLabel: "blackRedmond titleSmall",
            fontFamily: "Segoe UI",
            color: Colors.black,
            decoration: TextDecoration.none
        ),
        bodyLarge: new TextStyle(
            debugLabel: "blackRedmond bodyLarge",
            fontFamily: "Segoe UI",
            color: Colors.black87,
            decoration: TextDecoration.none
        ),
        bodyMedium: new TextStyle(
            debugLabel: "blackRedmond bodyMedium",
            fontFamily: "Segoe UI",
            color: Colors.black87,
            decoration: TextDecoration.none
        ),
        bodySmall: new TextStyle(
            debugLabel: "blackRedmond bodySmall",
            fontFamily: "Segoe UI",
            color: Colors.black54,
            decoration: TextDecoration.none
        ),
        labelLarge: new TextStyle(
            debugLabel: "blackRedmond labelLarge",
            fontFamily: "Segoe UI",
            color: Colors.black87,
            decoration: TextDecoration.none
        ),
        labelMedium: new TextStyle(
            debugLabel: "blackRedmond labelMedium",
            fontFamily: "Segoe UI",
            color: Colors.black,
            decoration: TextDecoration.none
        ),
        labelSmall: new TextStyle(
            debugLabel: "blackRedmond labelSmall",
            fontFamily: "Segoe UI",
            color: Colors.black,
            decoration: TextDecoration.none
        )
    );
    public static TextTheme whiteRedmond = new TextTheme(
        displayLarge: new TextStyle(
            debugLabel: "whiteRedmond displayLarge",
            fontFamily: "Segoe UI",
            color: Colors.white70,
            decoration: TextDecoration.none
        ),
        displayMedium: new TextStyle(
            debugLabel: "whiteRedmond displayMedium",
            fontFamily: "Segoe UI",
            color: Colors.white70,
            decoration: TextDecoration.none
        ),
        displaySmall: new TextStyle(
            debugLabel: "whiteRedmond displaySmall",
            fontFamily: "Segoe UI",
            color: Colors.white70,
            decoration: TextDecoration.none
        ),
        headlineLarge: new TextStyle(
            debugLabel: "whiteRedmond headlineLarge",
            fontFamily: "Segoe UI",
            color: Colors.white70,
            decoration: TextDecoration.none
        ),
        headlineMedium: new TextStyle(
            debugLabel: "whiteRedmond headlineMedium",
            fontFamily: "Segoe UI",
            color: Colors.white70,
            decoration: TextDecoration.none
        ),
        headlineSmall: new TextStyle(
            debugLabel: "whiteRedmond headlineSmall",
            fontFamily: "Segoe UI",
            color: Colors.white,
            decoration: TextDecoration.none
        ),
        titleLarge: new TextStyle(
            debugLabel: "whiteRedmond titleLarge",
            fontFamily: "Segoe UI",
            color: Colors.white,
            decoration: TextDecoration.none
        ),
        titleMedium: new TextStyle(
            debugLabel: "whiteRedmond titleMedium",
            fontFamily: "Segoe UI",
            color: Colors.white,
            decoration: TextDecoration.none
        ),
        titleSmall: new TextStyle(
            debugLabel: "whiteRedmond titleSmall",
            fontFamily: "Segoe UI",
            color: Colors.white,
            decoration: TextDecoration.none
        ),
        bodyLarge: new TextStyle(
            debugLabel: "whiteRedmond bodyLarge",
            fontFamily: "Segoe UI",
            color: Colors.white,
            decoration: TextDecoration.none
        ),
        bodyMedium: new TextStyle(
            debugLabel: "whiteRedmond bodyMedium",
            fontFamily: "Segoe UI",
            color: Colors.white,
            decoration: TextDecoration.none
        ),
        bodySmall: new TextStyle(
            debugLabel: "whiteRedmond bodySmall",
            fontFamily: "Segoe UI",
            color: Colors.white70,
            decoration: TextDecoration.none
        ),
        labelLarge: new TextStyle(
            debugLabel: "whiteRedmond labelLarge",
            fontFamily: "Segoe UI",
            color: Colors.white,
            decoration: TextDecoration.none
        ),
        labelMedium: new TextStyle(
            debugLabel: "whiteRedmond labelMedium",
            fontFamily: "Segoe UI",
            color: Colors.white,
            decoration: TextDecoration.none
        ),
        labelSmall: new TextStyle(
            debugLabel: "whiteRedmond labelSmall",
            fontFamily: "Segoe UI",
            color: Colors.white,
            decoration: TextDecoration.none
        )
    );
    internal static List<string> _helsinkiFontFallbacks = new List<string>
    {
        "Ubuntu",
        "Adwaita Sans",
        "Cantarell",
        "DejaVu Sans",
        "Liberation Sans",
        "Arial",
    };
    public static TextTheme blackHelsinki = new TextTheme(
        displayLarge: new TextStyle(
            debugLabel: "blackHelsinki displayLarge",
            fontFamily: "Roboto",
            fontFamilyFallback: _helsinkiFontFallbacks,
            color: Colors.black54,
            decoration: TextDecoration.none
        ),
        displayMedium: new TextStyle(
            debugLabel: "blackHelsinki displayMedium",
            fontFamily: "Roboto",
            fontFamilyFallback: _helsinkiFontFallbacks,
            color: Colors.black54,
            decoration: TextDecoration.none
        ),
        displaySmall: new TextStyle(
            debugLabel: "blackHelsinki displaySmall",
            fontFamily: "Roboto",
            fontFamilyFallback: _helsinkiFontFallbacks,
            color: Colors.black54,
            decoration: TextDecoration.none
        ),
        headlineLarge: new TextStyle(
            debugLabel: "blackHelsinki headlineLarge",
            fontFamily: "Roboto",
            fontFamilyFallback: _helsinkiFontFallbacks,
            color: Colors.black54,
            decoration: TextDecoration.none
        ),
        headlineMedium: new TextStyle(
            debugLabel: "blackHelsinki headlineMedium",
            fontFamily: "Roboto",
            fontFamilyFallback: _helsinkiFontFallbacks,
            color: Colors.black54,
            decoration: TextDecoration.none
        ),
        headlineSmall: new TextStyle(
            debugLabel: "blackHelsinki headlineSmall",
            fontFamily: "Roboto",
            fontFamilyFallback: _helsinkiFontFallbacks,
            color: Colors.black87,
            decoration: TextDecoration.none
        ),
        titleLarge: new TextStyle(
            debugLabel: "blackHelsinki titleLarge",
            fontFamily: "Roboto",
            fontFamilyFallback: _helsinkiFontFallbacks,
            color: Colors.black87,
            decoration: TextDecoration.none
        ),
        titleMedium: new TextStyle(
            debugLabel: "blackHelsinki titleMedium",
            fontFamily: "Roboto",
            fontFamilyFallback: _helsinkiFontFallbacks,
            color: Colors.black87,
            decoration: TextDecoration.none
        ),
        titleSmall: new TextStyle(
            debugLabel: "blackHelsinki titleSmall",
            fontFamily: "Roboto",
            fontFamilyFallback: _helsinkiFontFallbacks,
            color: Colors.black,
            decoration: TextDecoration.none
        ),
        bodyLarge: new TextStyle(
            debugLabel: "blackHelsinki bodyLarge",
            fontFamily: "Roboto",
            fontFamilyFallback: _helsinkiFontFallbacks,
            color: Colors.black87,
            decoration: TextDecoration.none
        ),
        bodyMedium: new TextStyle(
            debugLabel: "blackHelsinki bodyMedium",
            fontFamily: "Roboto",
            fontFamilyFallback: _helsinkiFontFallbacks,
            color: Colors.black87,
            decoration: TextDecoration.none
        ),
        bodySmall: new TextStyle(
            debugLabel: "blackHelsinki bodySmall",
            fontFamily: "Roboto",
            fontFamilyFallback: _helsinkiFontFallbacks,
            color: Colors.black54,
            decoration: TextDecoration.none
        ),
        labelLarge: new TextStyle(
            debugLabel: "blackHelsinki labelLarge",
            fontFamily: "Roboto",
            fontFamilyFallback: _helsinkiFontFallbacks,
            color: Colors.black87,
            decoration: TextDecoration.none
        ),
        labelMedium: new TextStyle(
            debugLabel: "blackHelsinki labelMedium",
            fontFamily: "Roboto",
            fontFamilyFallback: _helsinkiFontFallbacks,
            color: Colors.black,
            decoration: TextDecoration.none
        ),
        labelSmall: new TextStyle(
            debugLabel: "blackHelsinki labelSmall",
            fontFamily: "Roboto",
            fontFamilyFallback: _helsinkiFontFallbacks,
            color: Colors.black,
            decoration: TextDecoration.none
        )
    );
    public static TextTheme whiteHelsinki = new TextTheme(
        displayLarge: new TextStyle(
            debugLabel: "whiteHelsinki displayLarge",
            fontFamily: "Roboto",
            fontFamilyFallback: _helsinkiFontFallbacks,
            color: Colors.white70,
            decoration: TextDecoration.none
        ),
        displayMedium: new TextStyle(
            debugLabel: "whiteHelsinki displayMedium",
            fontFamily: "Roboto",
            fontFamilyFallback: _helsinkiFontFallbacks,
            color: Colors.white70,
            decoration: TextDecoration.none
        ),
        displaySmall: new TextStyle(
            debugLabel: "whiteHelsinki displaySmall",
            fontFamily: "Roboto",
            fontFamilyFallback: _helsinkiFontFallbacks,
            color: Colors.white70,
            decoration: TextDecoration.none
        ),
        headlineLarge: new TextStyle(
            debugLabel: "whiteHelsinki headlineLarge",
            fontFamily: "Roboto",
            fontFamilyFallback: _helsinkiFontFallbacks,
            color: Colors.white70,
            decoration: TextDecoration.none
        ),
        headlineMedium: new TextStyle(
            debugLabel: "whiteHelsinki headlineMedium",
            fontFamily: "Roboto",
            fontFamilyFallback: _helsinkiFontFallbacks,
            color: Colors.white70,
            decoration: TextDecoration.none
        ),
        headlineSmall: new TextStyle(
            debugLabel: "whiteHelsinki headlineSmall",
            fontFamily: "Roboto",
            fontFamilyFallback: _helsinkiFontFallbacks,
            color: Colors.white,
            decoration: TextDecoration.none
        ),
        titleLarge: new TextStyle(
            debugLabel: "whiteHelsinki titleLarge",
            fontFamily: "Roboto",
            fontFamilyFallback: _helsinkiFontFallbacks,
            color: Colors.white,
            decoration: TextDecoration.none
        ),
        titleMedium: new TextStyle(
            debugLabel: "whiteHelsinki titleMedium",
            fontFamily: "Roboto",
            fontFamilyFallback: _helsinkiFontFallbacks,
            color: Colors.white,
            decoration: TextDecoration.none
        ),
        titleSmall: new TextStyle(
            debugLabel: "whiteHelsinki titleSmall",
            fontFamily: "Roboto",
            fontFamilyFallback: _helsinkiFontFallbacks,
            color: Colors.white,
            decoration: TextDecoration.none
        ),
        bodyLarge: new TextStyle(
            debugLabel: "whiteHelsinki bodyLarge",
            fontFamily: "Roboto",
            fontFamilyFallback: _helsinkiFontFallbacks,
            color: Colors.white,
            decoration: TextDecoration.none
        ),
        bodyMedium: new TextStyle(
            debugLabel: "whiteHelsinki bodyMedium",
            fontFamily: "Roboto",
            fontFamilyFallback: _helsinkiFontFallbacks,
            color: Colors.white,
            decoration: TextDecoration.none
        ),
        bodySmall: new TextStyle(
            debugLabel: "whiteHelsinki bodySmall",
            fontFamily: "Roboto",
            fontFamilyFallback: _helsinkiFontFallbacks,
            color: Colors.white70,
            decoration: TextDecoration.none
        ),
        labelLarge: new TextStyle(
            debugLabel: "whiteHelsinki labelLarge",
            fontFamily: "Roboto",
            fontFamilyFallback: _helsinkiFontFallbacks,
            color: Colors.white,
            decoration: TextDecoration.none
        ),
        labelMedium: new TextStyle(
            debugLabel: "whiteHelsinki labelMedium",
            fontFamily: "Roboto",
            fontFamilyFallback: _helsinkiFontFallbacks,
            color: Colors.white,
            decoration: TextDecoration.none
        ),
        labelSmall: new TextStyle(
            debugLabel: "whiteHelsinki labelSmall",
            fontFamily: "Roboto",
            fontFamilyFallback: _helsinkiFontFallbacks,
            color: Colors.white,
            decoration: TextDecoration.none
        )
    );
    public static TextTheme blackCupertino = new TextTheme(
        displayLarge: new TextStyle(
            debugLabel: "blackCupertino displayLarge",
            fontFamily: "CupertinoSystemDisplay",
            color: Colors.black54,
            decoration: TextDecoration.none
        ),
        displayMedium: new TextStyle(
            debugLabel: "blackCupertino displayMedium",
            fontFamily: "CupertinoSystemDisplay",
            color: Colors.black54,
            decoration: TextDecoration.none
        ),
        displaySmall: new TextStyle(
            debugLabel: "blackCupertino displaySmall",
            fontFamily: "CupertinoSystemDisplay",
            color: Colors.black54,
            decoration: TextDecoration.none
        ),
        headlineLarge: new TextStyle(
            debugLabel: "blackCupertino headlineLarge",
            fontFamily: "CupertinoSystemDisplay",
            color: Colors.black54,
            decoration: TextDecoration.none
        ),
        headlineMedium: new TextStyle(
            debugLabel: "blackCupertino headlineMedium",
            fontFamily: "CupertinoSystemDisplay",
            color: Colors.black54,
            decoration: TextDecoration.none
        ),
        headlineSmall: new TextStyle(
            debugLabel: "blackCupertino headlineSmall",
            fontFamily: "CupertinoSystemDisplay",
            color: Colors.black87,
            decoration: TextDecoration.none
        ),
        titleLarge: new TextStyle(
            debugLabel: "blackCupertino titleLarge",
            fontFamily: "CupertinoSystemDisplay",
            color: Colors.black87,
            decoration: TextDecoration.none
        ),
        titleMedium: new TextStyle(
            debugLabel: "blackCupertino titleMedium",
            fontFamily: "CupertinoSystemText",
            color: Colors.black87,
            decoration: TextDecoration.none
        ),
        titleSmall: new TextStyle(
            debugLabel: "blackCupertino titleSmall",
            fontFamily: "CupertinoSystemText",
            color: Colors.black,
            decoration: TextDecoration.none
        ),
        bodyLarge: new TextStyle(
            debugLabel: "blackCupertino bodyLarge",
            fontFamily: "CupertinoSystemText",
            color: Colors.black87,
            decoration: TextDecoration.none
        ),
        bodyMedium: new TextStyle(
            debugLabel: "blackCupertino bodyMedium",
            fontFamily: "CupertinoSystemText",
            color: Colors.black87,
            decoration: TextDecoration.none
        ),
        bodySmall: new TextStyle(
            debugLabel: "blackCupertino bodySmall",
            fontFamily: "CupertinoSystemText",
            color: Colors.black54,
            decoration: TextDecoration.none
        ),
        labelLarge: new TextStyle(
            debugLabel: "blackCupertino labelLarge",
            fontFamily: "CupertinoSystemText",
            color: Colors.black87,
            decoration: TextDecoration.none
        ),
        labelMedium: new TextStyle(
            debugLabel: "blackCupertino labelMedium",
            fontFamily: "CupertinoSystemText",
            color: Colors.black,
            decoration: TextDecoration.none
        ),
        labelSmall: new TextStyle(
            debugLabel: "blackCupertino labelSmall",
            fontFamily: "CupertinoSystemText",
            color: Colors.black,
            decoration: TextDecoration.none
        )
    );
    public static TextTheme whiteCupertino = new TextTheme(
        displayLarge: new TextStyle(
            debugLabel: "whiteCupertino displayLarge",
            fontFamily: "CupertinoSystemDisplay",
            color: Colors.white70,
            decoration: TextDecoration.none
        ),
        displayMedium: new TextStyle(
            debugLabel: "whiteCupertino displayMedium",
            fontFamily: "CupertinoSystemDisplay",
            color: Colors.white70,
            decoration: TextDecoration.none
        ),
        displaySmall: new TextStyle(
            debugLabel: "whiteCupertino displaySmall",
            fontFamily: "CupertinoSystemDisplay",
            color: Colors.white70,
            decoration: TextDecoration.none
        ),
        headlineLarge: new TextStyle(
            debugLabel: "whiteCupertino headlineLarge",
            fontFamily: "CupertinoSystemDisplay",
            color: Colors.white70,
            decoration: TextDecoration.none
        ),
        headlineMedium: new TextStyle(
            debugLabel: "whiteCupertino headlineMedium",
            fontFamily: "CupertinoSystemDisplay",
            color: Colors.white70,
            decoration: TextDecoration.none
        ),
        headlineSmall: new TextStyle(
            debugLabel: "whiteCupertino headlineSmall",
            fontFamily: "CupertinoSystemDisplay",
            color: Colors.white,
            decoration: TextDecoration.none
        ),
        titleLarge: new TextStyle(
            debugLabel: "whiteCupertino titleLarge",
            fontFamily: "CupertinoSystemDisplay",
            color: Colors.white,
            decoration: TextDecoration.none
        ),
        titleMedium: new TextStyle(
            debugLabel: "whiteCupertino titleMedium",
            fontFamily: "CupertinoSystemText",
            color: Colors.white,
            decoration: TextDecoration.none
        ),
        titleSmall: new TextStyle(
            debugLabel: "whiteCupertino titleSmall",
            fontFamily: "CupertinoSystemText",
            color: Colors.white,
            decoration: TextDecoration.none
        ),
        bodyLarge: new TextStyle(
            debugLabel: "whiteCupertino bodyLarge",
            fontFamily: "CupertinoSystemText",
            color: Colors.white,
            decoration: TextDecoration.none
        ),
        bodyMedium: new TextStyle(
            debugLabel: "whiteCupertino bodyMedium",
            fontFamily: "CupertinoSystemText",
            color: Colors.white,
            decoration: TextDecoration.none
        ),
        bodySmall: new TextStyle(
            debugLabel: "whiteCupertino bodySmall",
            fontFamily: "CupertinoSystemText",
            color: Colors.white70,
            decoration: TextDecoration.none
        ),
        labelLarge: new TextStyle(
            debugLabel: "whiteCupertino labelLarge",
            fontFamily: "CupertinoSystemText",
            color: Colors.white,
            decoration: TextDecoration.none
        ),
        labelMedium: new TextStyle(
            debugLabel: "whiteCupertino labelMedium",
            fontFamily: "CupertinoSystemText",
            color: Colors.white,
            decoration: TextDecoration.none
        ),
        labelSmall: new TextStyle(
            debugLabel: "whiteCupertino labelSmall",
            fontFamily: "CupertinoSystemText",
            color: Colors.white,
            decoration: TextDecoration.none
        )
    );
    public static TextTheme blackRedwoodCity = new TextTheme(
        displayLarge: new TextStyle(
            debugLabel: "blackRedwoodCity displayLarge",
            fontFamily: ".AppleSystemUIFont",
            color: Colors.black54,
            decoration: TextDecoration.none
        ),
        displayMedium: new TextStyle(
            debugLabel: "blackRedwoodCity displayMedium",
            fontFamily: ".AppleSystemUIFont",
            color: Colors.black54,
            decoration: TextDecoration.none
        ),
        displaySmall: new TextStyle(
            debugLabel: "blackRedwoodCity displaySmall",
            fontFamily: ".AppleSystemUIFont",
            color: Colors.black54,
            decoration: TextDecoration.none
        ),
        headlineLarge: new TextStyle(
            debugLabel: "blackRedwoodCity headlineLarge",
            fontFamily: ".AppleSystemUIFont",
            color: Colors.black54,
            decoration: TextDecoration.none
        ),
        headlineMedium: new TextStyle(
            debugLabel: "blackRedwoodCity headlineMedium",
            fontFamily: ".AppleSystemUIFont",
            color: Colors.black54,
            decoration: TextDecoration.none
        ),
        headlineSmall: new TextStyle(
            debugLabel: "blackRedwoodCity headlineSmall",
            fontFamily: ".AppleSystemUIFont",
            color: Colors.black87,
            decoration: TextDecoration.none
        ),
        titleLarge: new TextStyle(
            debugLabel: "blackRedwoodCity titleLarge",
            fontFamily: ".AppleSystemUIFont",
            color: Colors.black87,
            decoration: TextDecoration.none
        ),
        titleMedium: new TextStyle(
            debugLabel: "blackRedwoodCity titleMedium",
            fontFamily: ".AppleSystemUIFont",
            color: Colors.black87,
            decoration: TextDecoration.none
        ),
        titleSmall: new TextStyle(
            debugLabel: "blackRedwoodCity titleSmall",
            fontFamily: ".AppleSystemUIFont",
            color: Colors.black,
            decoration: TextDecoration.none
        ),
        bodyLarge: new TextStyle(
            debugLabel: "blackRedwoodCity bodyLarge",
            fontFamily: ".AppleSystemUIFont",
            color: Colors.black87,
            decoration: TextDecoration.none
        ),
        bodyMedium: new TextStyle(
            debugLabel: "blackRedwoodCity bodyMedium",
            fontFamily: ".AppleSystemUIFont",
            color: Colors.black87,
            decoration: TextDecoration.none
        ),
        bodySmall: new TextStyle(
            debugLabel: "blackRedwoodCity bodySmall",
            fontFamily: ".AppleSystemUIFont",
            color: Colors.black54,
            decoration: TextDecoration.none
        ),
        labelLarge: new TextStyle(
            debugLabel: "blackRedwoodCity labelLarge",
            fontFamily: ".AppleSystemUIFont",
            color: Colors.black87,
            decoration: TextDecoration.none
        ),
        labelMedium: new TextStyle(
            debugLabel: "blackRedwoodCity labelMedium",
            fontFamily: ".AppleSystemUIFont",
            color: Colors.black,
            decoration: TextDecoration.none
        ),
        labelSmall: new TextStyle(
            debugLabel: "blackRedwoodCity labelSmall",
            fontFamily: ".AppleSystemUIFont",
            color: Colors.black,
            decoration: TextDecoration.none
        )
    );
    public static TextTheme whiteRedwoodCity = new TextTheme(
        displayLarge: new TextStyle(
            debugLabel: "whiteRedwoodCity displayLarge",
            fontFamily: ".AppleSystemUIFont",
            color: Colors.white70,
            decoration: TextDecoration.none
        ),
        displayMedium: new TextStyle(
            debugLabel: "whiteRedwoodCity displayMedium",
            fontFamily: ".AppleSystemUIFont",
            color: Colors.white70,
            decoration: TextDecoration.none
        ),
        displaySmall: new TextStyle(
            debugLabel: "whiteRedwoodCity displaySmall",
            fontFamily: ".AppleSystemUIFont",
            color: Colors.white70,
            decoration: TextDecoration.none
        ),
        headlineLarge: new TextStyle(
            debugLabel: "whiteRedwoodCity headlineLarge",
            fontFamily: ".AppleSystemUIFont",
            color: Colors.white70,
            decoration: TextDecoration.none
        ),
        headlineMedium: new TextStyle(
            debugLabel: "whiteRedwoodCity headlineMedium",
            fontFamily: ".AppleSystemUIFont",
            color: Colors.white70,
            decoration: TextDecoration.none
        ),
        headlineSmall: new TextStyle(
            debugLabel: "whiteRedwoodCity headlineSmall",
            fontFamily: ".AppleSystemUIFont",
            color: Colors.white,
            decoration: TextDecoration.none
        ),
        titleLarge: new TextStyle(
            debugLabel: "whiteRedwoodCity titleLarge",
            fontFamily: ".AppleSystemUIFont",
            color: Colors.white,
            decoration: TextDecoration.none
        ),
        titleMedium: new TextStyle(
            debugLabel: "whiteRedwoodCity titleMedium",
            fontFamily: ".AppleSystemUIFont",
            color: Colors.white,
            decoration: TextDecoration.none
        ),
        titleSmall: new TextStyle(
            debugLabel: "whiteRedwoodCity titleSmall",
            fontFamily: ".AppleSystemUIFont",
            color: Colors.white,
            decoration: TextDecoration.none
        ),
        bodyLarge: new TextStyle(
            debugLabel: "whiteRedwoodCity bodyLarge",
            fontFamily: ".AppleSystemUIFont",
            color: Colors.white,
            decoration: TextDecoration.none
        ),
        bodyMedium: new TextStyle(
            debugLabel: "whiteRedwoodCity bodyMedium",
            fontFamily: ".AppleSystemUIFont",
            color: Colors.white,
            decoration: TextDecoration.none
        ),
        bodySmall: new TextStyle(
            debugLabel: "whiteRedwoodCity bodySmall",
            fontFamily: ".AppleSystemUIFont",
            color: Colors.white70,
            decoration: TextDecoration.none
        ),
        labelLarge: new TextStyle(
            debugLabel: "whiteRedwoodCity labelLarge",
            fontFamily: ".AppleSystemUIFont",
            color: Colors.white,
            decoration: TextDecoration.none
        ),
        labelMedium: new TextStyle(
            debugLabel: "whiteRedwoodCity labelMedium",
            fontFamily: ".AppleSystemUIFont",
            color: Colors.white,
            decoration: TextDecoration.none
        ),
        labelSmall: new TextStyle(
            debugLabel: "whiteRedwoodCity labelSmall",
            fontFamily: ".AppleSystemUIFont",
            color: Colors.white,
            decoration: TextDecoration.none
        )
    );
    public static TextTheme englishLike2021 = _M3Typography__typography.englishLike;
    public static TextTheme dense2021 = _M3Typography__typography.dense;
    public static TextTheme tall2021 = _M3Typography__typography.tall;

    public static Typography Create(
        TargetPlatform? platform = null,
        TextTheme? black = null,
        TextTheme? white = null,
        TextTheme? englishLike = null,
        TextTheme? dense = null,
        TextTheme? tall = null
    ) =>
        CreateMaterial2021(
            platform: platform ?? PlatformLibrary.defaultTargetPlatform,
            black: black,
            white: white,
            englishLike: englishLike,
            dense: dense,
            tall: tall
        );

    public static Typography CreateMaterial2021(
        TargetPlatform? platform = TargetPlatform.android,
        ColorScheme colorScheme = default!,
        TextTheme? black = null,
        TextTheme? white = null,
        TextTheme? englishLike = null,
        TextTheme? dense = null,
        TextTheme? tall = null
    )
    {
        ColorScheme __colorScheme = colorScheme ?? ColorScheme.CreateLight();
        DartRuntimePrimitives.Assert(() =>
            (platform is not null) || ((black is not null) && (white is not null))
        );
        var @base = Create_withPlatform(
            platform,
            black,
            white,
            englishLike ?? englishLike2021,
            dense ?? dense2021,
            tall ?? tall2021
        );
        Color dark = Equals(__colorScheme.brightness, Brightness.light)
            ? __colorScheme.onSurface
            : __colorScheme.surface;
        Color lightLocal = Equals(__colorScheme.brightness, Brightness.light)
            ? __colorScheme.surface
            : __colorScheme.onSurface;
        return @base.copyWith(
            black: @base.black.apply(displayColor: dark, bodyColor: dark, decorationColor: dark),
            white: @base.white.apply(
                displayColor: lightLocal,
                bodyColor: lightLocal,
                decorationColor: lightLocal
            )
        );
    }

    public static Typography Create_withPlatform(
        TargetPlatform? platform,
        TextTheme? black,
        TextTheme? white,
        TextTheme englishLike,
        TextTheme dense,
        TextTheme tall
    )
    {
        DartRuntimePrimitives.Assert(() =>
            (platform is not null) || ((black is not null) && (white is not null))
        );
        var (blackResolved, whiteResolved) = platform switch
        {
            TargetPlatform.iOS => ((TextTheme, TextTheme))
                (black ?? blackCupertino, white ?? whiteCupertino),
            TargetPlatform.android => ((TextTheme, TextTheme))
                (black ?? blackMountainView, white ?? whiteMountainView),
            TargetPlatform.fuchsia => ((TextTheme, TextTheme))
                (black ?? blackMountainView, white ?? whiteMountainView),
            TargetPlatform.windows => ((TextTheme, TextTheme))
                (black ?? blackRedmond, white ?? whiteRedmond),
            TargetPlatform.macOS => ((TextTheme, TextTheme))
                (black ?? blackRedwoodCity, white ?? whiteRedwoodCity),
            TargetPlatform.linux => ((TextTheme, TextTheme))
                (black ?? blackHelsinki, white ?? whiteHelsinki),
            null => ((TextTheme, TextTheme))(black!, white!),
            _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                ),
        };
        return new Typography(blackResolved, whiteResolved, englishLike, dense, tall);
    }

    public Typography(
        TextTheme black,
        TextTheme white,
        TextTheme englishLike,
        TextTheme dense,
        TextTheme tall
    )
    {
        this.black = black;
        this.white = white;
        this.englishLike = englishLike;
        this.dense = dense;
        this.tall = tall;
    }

    public virtual TextTheme geometryThemeFor(ScriptCategory category)
    {
        return category switch
        {
            ScriptCategory.englishLike => englishLike,
            ScriptCategory.dense => dense,
            ScriptCategory.tall => tall,
            _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                ),
        };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual Typography copyWith(
        TextTheme? black = null,
        TextTheme? white = null,
        TextTheme? englishLike = null,
        TextTheme? dense = null,
        TextTheme? tall = null
    )
    {
        return new Typography(
            black ?? this.black,
            white ?? this.white,
            englishLike ?? this.englishLike,
            dense ?? this.dense,
            tall ?? this.tall
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static Typography lerp(Typography a, Typography b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        return new Typography(
            TextTheme.lerp(a.black, b.black, t),
            TextTheme.lerp(a.white, b.white, t),
            TextTheme.lerp(a.englishLike, b.englishLike, t),
            TextTheme.lerp(a.dense, b.dense, t),
            TextTheme.lerp(a.tall, b.tall, t)
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as Typography;
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
        return (__other is Typography)
            && Equals(__other.black, black)
            && Equals(__other.white, white)
            && Equals(__other.englishLike, englishLike)
            && Equals(__other.dense, dense)
            && Equals(__other.tall, tall);
    }

    public override int GetHashCode() =>
        DartRuntimePrimitives.ConvertValue<int>(
            FoundationRuntimePorts.ObjectHash(black, white, englishLike, dense, tall)
        );

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        var defaultTypography = CreateMaterial2021();
        properties.add(
            new DiagnosticsProperty<TextTheme>(
                "black",
                black,
                defaultValue: defaultTypography.black
            )
        );
        properties.add(
            new DiagnosticsProperty<TextTheme>(
                "white",
                white,
                defaultValue: defaultTypography.white
            )
        );
        properties.add(
            new DiagnosticsProperty<TextTheme>(
                "englishLike",
                englishLike,
                defaultValue: defaultTypography.englishLike
            )
        );
        properties.add(
            new DiagnosticsProperty<TextTheme>(
                "dense",
                dense,
                defaultValue: defaultTypography.dense
            )
        );
        properties.add(
            new DiagnosticsProperty<TextTheme>("tall", tall, defaultValue: defaultTypography.tall)
        );
    }

    public virtual string toStringShort() => DiagnosticsLibrary.describeIdentity(this);

    public override string ToString() => ToString(DiagnosticLevel.info);

    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString = default!;
        DartRuntimePrimitives.Assert(() =>
        {
            fullString = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine)
                .toDiagnosticsNode()
                .toStringDeep(minLevel: minLevel);
            return true;
        });
        return fullString ?? toStringShort();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(
        string? name = null,
        DiagnosticsTreeStyle? style = null
    )
    {
        return new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal abstract class _M3Typography__typography
{
    public static TextTheme englishLike = new TextTheme(
        displayLarge: new TextStyle(
            debugLabel: "englishLike displayLarge 2021",
            inherit: false,
            fontSize: 57.0,
            fontWeight: FontWeight.w400,
            letterSpacing: -0.25,
            height: 1.12,
            textBaseline: TextBaseline.alphabetic,
            leadingDistribution: TextLeadingDistribution.even
        ),
        displayMedium: new TextStyle(
            debugLabel: "englishLike displayMedium 2021",
            inherit: false,
            fontSize: 45.0,
            fontWeight: FontWeight.w400,
            letterSpacing: 0.0,
            height: 1.16,
            textBaseline: TextBaseline.alphabetic,
            leadingDistribution: TextLeadingDistribution.even
        ),
        displaySmall: new TextStyle(
            debugLabel: "englishLike displaySmall 2021",
            inherit: false,
            fontSize: 36.0,
            fontWeight: FontWeight.w400,
            letterSpacing: 0.0,
            height: 1.22,
            textBaseline: TextBaseline.alphabetic,
            leadingDistribution: TextLeadingDistribution.even
        ),
        headlineLarge: new TextStyle(
            debugLabel: "englishLike headlineLarge 2021",
            inherit: false,
            fontSize: 32.0,
            fontWeight: FontWeight.w400,
            letterSpacing: 0.0,
            height: 1.25,
            textBaseline: TextBaseline.alphabetic,
            leadingDistribution: TextLeadingDistribution.even
        ),
        headlineMedium: new TextStyle(
            debugLabel: "englishLike headlineMedium 2021",
            inherit: false,
            fontSize: 28.0,
            fontWeight: FontWeight.w400,
            letterSpacing: 0.0,
            height: 1.29,
            textBaseline: TextBaseline.alphabetic,
            leadingDistribution: TextLeadingDistribution.even
        ),
        headlineSmall: new TextStyle(
            debugLabel: "englishLike headlineSmall 2021",
            inherit: false,
            fontSize: 24.0,
            fontWeight: FontWeight.w400,
            letterSpacing: 0.0,
            height: 1.33,
            textBaseline: TextBaseline.alphabetic,
            leadingDistribution: TextLeadingDistribution.even
        ),
        titleLarge: new TextStyle(
            debugLabel: "englishLike titleLarge 2021",
            inherit: false,
            fontSize: 22.0,
            fontWeight: FontWeight.w400,
            letterSpacing: 0.0,
            height: 1.27,
            textBaseline: TextBaseline.alphabetic,
            leadingDistribution: TextLeadingDistribution.even
        ),
        titleMedium: new TextStyle(
            debugLabel: "englishLike titleMedium 2021",
            inherit: false,
            fontSize: 16.0,
            fontWeight: FontWeight.w500,
            letterSpacing: 0.15,
            height: 1.5,
            textBaseline: TextBaseline.alphabetic,
            leadingDistribution: TextLeadingDistribution.even
        ),
        titleSmall: new TextStyle(
            debugLabel: "englishLike titleSmall 2021",
            inherit: false,
            fontSize: 14.0,
            fontWeight: FontWeight.w500,
            letterSpacing: 0.1,
            height: 1.43,
            textBaseline: TextBaseline.alphabetic,
            leadingDistribution: TextLeadingDistribution.even
        ),
        labelLarge: new TextStyle(
            debugLabel: "englishLike labelLarge 2021",
            inherit: false,
            fontSize: 14.0,
            fontWeight: FontWeight.w500,
            letterSpacing: 0.1,
            height: 1.43,
            textBaseline: TextBaseline.alphabetic,
            leadingDistribution: TextLeadingDistribution.even
        ),
        labelMedium: new TextStyle(
            debugLabel: "englishLike labelMedium 2021",
            inherit: false,
            fontSize: 12.0,
            fontWeight: FontWeight.w500,
            letterSpacing: 0.5,
            height: 1.33,
            textBaseline: TextBaseline.alphabetic,
            leadingDistribution: TextLeadingDistribution.even
        ),
        labelSmall: new TextStyle(
            debugLabel: "englishLike labelSmall 2021",
            inherit: false,
            fontSize: 11.0,
            fontWeight: FontWeight.w500,
            letterSpacing: 0.5,
            height: 1.45,
            textBaseline: TextBaseline.alphabetic,
            leadingDistribution: TextLeadingDistribution.even
        ),
        bodyLarge: new TextStyle(
            debugLabel: "englishLike bodyLarge 2021",
            inherit: false,
            fontSize: 16.0,
            fontWeight: FontWeight.w400,
            letterSpacing: 0.5,
            height: 1.5,
            textBaseline: TextBaseline.alphabetic,
            leadingDistribution: TextLeadingDistribution.even
        ),
        bodyMedium: new TextStyle(
            debugLabel: "englishLike bodyMedium 2021",
            inherit: false,
            fontSize: 14.0,
            fontWeight: FontWeight.w400,
            letterSpacing: 0.25,
            height: 1.43,
            textBaseline: TextBaseline.alphabetic,
            leadingDistribution: TextLeadingDistribution.even
        ),
        bodySmall: new TextStyle(
            debugLabel: "englishLike bodySmall 2021",
            inherit: false,
            fontSize: 12.0,
            fontWeight: FontWeight.w400,
            letterSpacing: 0.4,
            height: 1.33,
            textBaseline: TextBaseline.alphabetic,
            leadingDistribution: TextLeadingDistribution.even
        )
    );
    public static TextTheme dense = new TextTheme(
        displayLarge: new TextStyle(
            debugLabel: "dense displayLarge 2021",
            inherit: false,
            fontSize: 57.0,
            fontWeight: FontWeight.w400,
            letterSpacing: -0.25,
            height: 1.12,
            textBaseline: TextBaseline.ideographic,
            leadingDistribution: TextLeadingDistribution.even
        ),
        displayMedium: new TextStyle(
            debugLabel: "dense displayMedium 2021",
            inherit: false,
            fontSize: 45.0,
            fontWeight: FontWeight.w400,
            letterSpacing: 0.0,
            height: 1.16,
            textBaseline: TextBaseline.ideographic,
            leadingDistribution: TextLeadingDistribution.even
        ),
        displaySmall: new TextStyle(
            debugLabel: "dense displaySmall 2021",
            inherit: false,
            fontSize: 36.0,
            fontWeight: FontWeight.w400,
            letterSpacing: 0.0,
            height: 1.22,
            textBaseline: TextBaseline.ideographic,
            leadingDistribution: TextLeadingDistribution.even
        ),
        headlineLarge: new TextStyle(
            debugLabel: "dense headlineLarge 2021",
            inherit: false,
            fontSize: 32.0,
            fontWeight: FontWeight.w400,
            letterSpacing: 0.0,
            height: 1.25,
            textBaseline: TextBaseline.ideographic,
            leadingDistribution: TextLeadingDistribution.even
        ),
        headlineMedium: new TextStyle(
            debugLabel: "dense headlineMedium 2021",
            inherit: false,
            fontSize: 28.0,
            fontWeight: FontWeight.w400,
            letterSpacing: 0.0,
            height: 1.29,
            textBaseline: TextBaseline.ideographic,
            leadingDistribution: TextLeadingDistribution.even
        ),
        headlineSmall: new TextStyle(
            debugLabel: "dense headlineSmall 2021",
            inherit: false,
            fontSize: 24.0,
            fontWeight: FontWeight.w400,
            letterSpacing: 0.0,
            height: 1.33,
            textBaseline: TextBaseline.ideographic,
            leadingDistribution: TextLeadingDistribution.even
        ),
        titleLarge: new TextStyle(
            debugLabel: "dense titleLarge 2021",
            inherit: false,
            fontSize: 22.0,
            fontWeight: FontWeight.w400,
            letterSpacing: 0.0,
            height: 1.27,
            textBaseline: TextBaseline.ideographic,
            leadingDistribution: TextLeadingDistribution.even
        ),
        titleMedium: new TextStyle(
            debugLabel: "dense titleMedium 2021",
            inherit: false,
            fontSize: 16.0,
            fontWeight: FontWeight.w500,
            letterSpacing: 0.15,
            height: 1.5,
            textBaseline: TextBaseline.ideographic,
            leadingDistribution: TextLeadingDistribution.even
        ),
        titleSmall: new TextStyle(
            debugLabel: "dense titleSmall 2021",
            inherit: false,
            fontSize: 14.0,
            fontWeight: FontWeight.w500,
            letterSpacing: 0.1,
            height: 1.43,
            textBaseline: TextBaseline.ideographic,
            leadingDistribution: TextLeadingDistribution.even
        ),
        labelLarge: new TextStyle(
            debugLabel: "dense labelLarge 2021",
            inherit: false,
            fontSize: 14.0,
            fontWeight: FontWeight.w500,
            letterSpacing: 0.1,
            height: 1.43,
            textBaseline: TextBaseline.ideographic,
            leadingDistribution: TextLeadingDistribution.even
        ),
        labelMedium: new TextStyle(
            debugLabel: "dense labelMedium 2021",
            inherit: false,
            fontSize: 12.0,
            fontWeight: FontWeight.w500,
            letterSpacing: 0.5,
            height: 1.33,
            textBaseline: TextBaseline.ideographic,
            leadingDistribution: TextLeadingDistribution.even
        ),
        labelSmall: new TextStyle(
            debugLabel: "dense labelSmall 2021",
            inherit: false,
            fontSize: 11.0,
            fontWeight: FontWeight.w500,
            letterSpacing: 0.5,
            height: 1.45,
            textBaseline: TextBaseline.ideographic,
            leadingDistribution: TextLeadingDistribution.even
        ),
        bodyLarge: new TextStyle(
            debugLabel: "dense bodyLarge 2021",
            inherit: false,
            fontSize: 16.0,
            fontWeight: FontWeight.w400,
            letterSpacing: 0.5,
            height: 1.5,
            textBaseline: TextBaseline.ideographic,
            leadingDistribution: TextLeadingDistribution.even
        ),
        bodyMedium: new TextStyle(
            debugLabel: "dense bodyMedium 2021",
            inherit: false,
            fontSize: 14.0,
            fontWeight: FontWeight.w400,
            letterSpacing: 0.25,
            height: 1.43,
            textBaseline: TextBaseline.ideographic,
            leadingDistribution: TextLeadingDistribution.even
        ),
        bodySmall: new TextStyle(
            debugLabel: "dense bodySmall 2021",
            inherit: false,
            fontSize: 12.0,
            fontWeight: FontWeight.w400,
            letterSpacing: 0.4,
            height: 1.33,
            textBaseline: TextBaseline.ideographic,
            leadingDistribution: TextLeadingDistribution.even
        )
    );
    public static TextTheme tall = new TextTheme(
        displayLarge: new TextStyle(
            debugLabel: "tall displayLarge 2021",
            inherit: false,
            fontSize: 57.0,
            fontWeight: FontWeight.w400,
            letterSpacing: -0.25,
            height: 1.12,
            textBaseline: TextBaseline.alphabetic,
            leadingDistribution: TextLeadingDistribution.even
        ),
        displayMedium: new TextStyle(
            debugLabel: "tall displayMedium 2021",
            inherit: false,
            fontSize: 45.0,
            fontWeight: FontWeight.w400,
            letterSpacing: 0.0,
            height: 1.16,
            textBaseline: TextBaseline.alphabetic,
            leadingDistribution: TextLeadingDistribution.even
        ),
        displaySmall: new TextStyle(
            debugLabel: "tall displaySmall 2021",
            inherit: false,
            fontSize: 36.0,
            fontWeight: FontWeight.w400,
            letterSpacing: 0.0,
            height: 1.22,
            textBaseline: TextBaseline.alphabetic,
            leadingDistribution: TextLeadingDistribution.even
        ),
        headlineLarge: new TextStyle(
            debugLabel: "tall headlineLarge 2021",
            inherit: false,
            fontSize: 32.0,
            fontWeight: FontWeight.w400,
            letterSpacing: 0.0,
            height: 1.25,
            textBaseline: TextBaseline.alphabetic,
            leadingDistribution: TextLeadingDistribution.even
        ),
        headlineMedium: new TextStyle(
            debugLabel: "tall headlineMedium 2021",
            inherit: false,
            fontSize: 28.0,
            fontWeight: FontWeight.w400,
            letterSpacing: 0.0,
            height: 1.29,
            textBaseline: TextBaseline.alphabetic,
            leadingDistribution: TextLeadingDistribution.even
        ),
        headlineSmall: new TextStyle(
            debugLabel: "tall headlineSmall 2021",
            inherit: false,
            fontSize: 24.0,
            fontWeight: FontWeight.w400,
            letterSpacing: 0.0,
            height: 1.33,
            textBaseline: TextBaseline.alphabetic,
            leadingDistribution: TextLeadingDistribution.even
        ),
        titleLarge: new TextStyle(
            debugLabel: "tall titleLarge 2021",
            inherit: false,
            fontSize: 22.0,
            fontWeight: FontWeight.w400,
            letterSpacing: 0.0,
            height: 1.27,
            textBaseline: TextBaseline.alphabetic,
            leadingDistribution: TextLeadingDistribution.even
        ),
        titleMedium: new TextStyle(
            debugLabel: "tall titleMedium 2021",
            inherit: false,
            fontSize: 16.0,
            fontWeight: FontWeight.w500,
            letterSpacing: 0.15,
            height: 1.5,
            textBaseline: TextBaseline.alphabetic,
            leadingDistribution: TextLeadingDistribution.even
        ),
        titleSmall: new TextStyle(
            debugLabel: "tall titleSmall 2021",
            inherit: false,
            fontSize: 14.0,
            fontWeight: FontWeight.w500,
            letterSpacing: 0.1,
            height: 1.43,
            textBaseline: TextBaseline.alphabetic,
            leadingDistribution: TextLeadingDistribution.even
        ),
        labelLarge: new TextStyle(
            debugLabel: "tall labelLarge 2021",
            inherit: false,
            fontSize: 14.0,
            fontWeight: FontWeight.w500,
            letterSpacing: 0.1,
            height: 1.43,
            textBaseline: TextBaseline.alphabetic,
            leadingDistribution: TextLeadingDistribution.even
        ),
        labelMedium: new TextStyle(
            debugLabel: "tall labelMedium 2021",
            inherit: false,
            fontSize: 12.0,
            fontWeight: FontWeight.w500,
            letterSpacing: 0.5,
            height: 1.33,
            textBaseline: TextBaseline.alphabetic,
            leadingDistribution: TextLeadingDistribution.even
        ),
        labelSmall: new TextStyle(
            debugLabel: "tall labelSmall 2021",
            inherit: false,
            fontSize: 11.0,
            fontWeight: FontWeight.w500,
            letterSpacing: 0.5,
            height: 1.45,
            textBaseline: TextBaseline.alphabetic,
            leadingDistribution: TextLeadingDistribution.even
        ),
        bodyLarge: new TextStyle(
            debugLabel: "tall bodyLarge 2021",
            inherit: false,
            fontSize: 16.0,
            fontWeight: FontWeight.w400,
            letterSpacing: 0.5,
            height: 1.5,
            textBaseline: TextBaseline.alphabetic,
            leadingDistribution: TextLeadingDistribution.even
        ),
        bodyMedium: new TextStyle(
            debugLabel: "tall bodyMedium 2021",
            inherit: false,
            fontSize: 14.0,
            fontWeight: FontWeight.w400,
            letterSpacing: 0.25,
            height: 1.43,
            textBaseline: TextBaseline.alphabetic,
            leadingDistribution: TextLeadingDistribution.even
        ),
        bodySmall: new TextStyle(
            debugLabel: "tall bodySmall 2021",
            inherit: false,
            fontSize: 12.0,
            fontWeight: FontWeight.w400,
            letterSpacing: 0.4,
            height: 1.33,
            textBaseline: TextBaseline.alphabetic,
            leadingDistribution: TextLeadingDistribution.even
        )
    );
}
