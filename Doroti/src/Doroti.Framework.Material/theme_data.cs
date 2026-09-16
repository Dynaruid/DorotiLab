// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/theme_data.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class Adaptation<T>
{
    public Adaptation()
    {
    }

    public virtual Type type => typeof(T);
    public virtual T adapt(ThemeData theme, T defaultValue) => defaultValue;
}

public abstract class ThemeExtension<T>
{
    protected ThemeExtension()
    {
    }

    public virtual object type => typeof(T);
    public abstract ThemeExtension<T> copyWith();
    public abstract ThemeExtension<T> lerp(ThemeExtension<T>? other, double t);
}

public enum MaterialTapTargetSize
{
    padded,
    shrinkWrap
}

public class ThemeData : global::Doroti.Framework.Foundation.Diagnosticable
{
    public virtual bool applyElevationOverlayColor { get; private set; } = default!;
    public virtual NoDefaultCupertinoThemeData? cupertinoOverrideTheme { get; private set; }
    public virtual DartMap<object, ThemeExtension<object>> extensions { get; private set; } = default!;
    public virtual DartMap<Type, Adaptation<object>> adaptationMap { get; private set; } = default!;
    public virtual InputDecorationThemeData inputDecorationTheme { get; private set; } = default!;
    public virtual MaterialTapTargetSize materialTapTargetSize { get; private set; } = default!;
    public virtual PageTransitionsTheme pageTransitionsTheme { get; private set; } = default!;
    public virtual global::Doroti.Framework.Foundation.TargetPlatform platform { get; private set; } = default!;
    public virtual ScrollbarThemeData scrollbarTheme { get; private set; } = default!;
    public virtual InteractiveInkFeatureFactory splashFactory { get; private set; } = default!;
    public virtual VisualDensity visualDensity { get; private set; } = default!;
    public virtual Color canvasColor { get; private set; } = default!;
    public virtual Color cardColor { get; private set; } = default!;
    public virtual ColorScheme colorScheme { get; private set; } = default!;
    public virtual Color disabledColor { get; private set; } = default!;
    public virtual Color dividerColor { get; private set; } = default!;
    public virtual Color focusColor { get; private set; } = default!;
    public virtual Color highlightColor { get; private set; } = default!;
    public virtual Color hintColor { get; private set; } = default!;
    public virtual Color hoverColor { get; private set; } = default!;
    public virtual Color primaryColor { get; private set; } = default!;
    public virtual Color primaryColorDark { get; private set; } = default!;
    public virtual Color primaryColorLight { get; private set; } = default!;
    public virtual Color scaffoldBackgroundColor { get; private set; } = default!;
    public virtual Color secondaryHeaderColor { get; private set; } = default!;
    public virtual Color shadowColor { get; private set; } = default!;
    public virtual Color splashColor { get; private set; } = default!;
    public virtual Color unselectedWidgetColor { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.IconThemeData iconTheme { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.IconThemeData primaryIconTheme { get; private set; } = default!;
    public virtual TextTheme primaryTextTheme { get; private set; } = default!;
    public virtual TextTheme textTheme { get; private set; } = default!;
    public virtual Typography typography { get; private set; } = default!;
    public virtual ActionIconThemeData? actionIconTheme { get; private set; }
    public virtual AppBarThemeData appBarTheme { get; private set; } = default!;
    public virtual BadgeThemeData badgeTheme { get; private set; } = default!;
    public virtual MaterialBannerThemeData bannerTheme { get; private set; } = default!;
    public virtual BottomAppBarThemeData bottomAppBarTheme { get; private set; } = default!;
    public virtual BottomNavigationBarThemeData bottomNavigationBarTheme { get; private set; } = default!;
    public virtual BottomSheetThemeData bottomSheetTheme { get; private set; } = default!;
    public virtual ButtonThemeData buttonTheme { get; private set; } = default!;
    public virtual CardThemeData cardTheme { get; private set; } = default!;
    public virtual CarouselViewThemeData carouselViewTheme { get; private set; } = default!;
    public virtual CheckboxThemeData checkboxTheme { get; private set; } = default!;
    public virtual ChipThemeData chipTheme { get; private set; } = default!;
    public virtual DataTableThemeData dataTableTheme { get; private set; } = default!;
    public virtual DatePickerThemeData datePickerTheme { get; private set; } = default!;
    public virtual DialogThemeData dialogTheme { get; private set; } = default!;
    public virtual DividerThemeData dividerTheme { get; private set; } = default!;
    public virtual DrawerThemeData drawerTheme { get; private set; } = default!;
    public virtual DropdownMenuThemeData dropdownMenuTheme { get; private set; } = default!;
    public virtual ElevatedButtonThemeData elevatedButtonTheme { get; private set; } = default!;
    public virtual ExpansionTileThemeData expansionTileTheme { get; private set; } = default!;
    public virtual FilledButtonThemeData filledButtonTheme { get; private set; } = default!;
    public virtual FloatingActionButtonThemeData floatingActionButtonTheme { get; private set; } = default!;
    public virtual IconButtonThemeData iconButtonTheme { get; private set; } = default!;
    public virtual ListTileThemeData listTileTheme { get; private set; } = default!;
    public virtual MenuBarThemeData menuBarTheme { get; private set; } = default!;
    public virtual MenuButtonThemeData menuButtonTheme { get; private set; } = default!;
    public virtual MenuThemeData menuTheme { get; private set; } = default!;
    public virtual NavigationBarThemeData navigationBarTheme { get; private set; } = default!;
    public virtual NavigationDrawerThemeData navigationDrawerTheme { get; private set; } = default!;
    public virtual NavigationRailThemeData navigationRailTheme { get; private set; } = default!;
    public virtual OutlinedButtonThemeData outlinedButtonTheme { get; private set; } = default!;
    public virtual PopupMenuThemeData popupMenuTheme { get; private set; } = default!;
    public virtual ProgressIndicatorThemeData progressIndicatorTheme { get; private set; } = default!;
    public virtual RadioThemeData radioTheme { get; private set; } = default!;
    public virtual SearchBarThemeData searchBarTheme { get; private set; } = default!;
    public virtual SearchViewThemeData searchViewTheme { get; private set; } = default!;
    public virtual SegmentedButtonThemeData segmentedButtonTheme { get; private set; } = default!;
    public virtual SliderThemeData sliderTheme { get; private set; } = default!;
    public virtual SnackBarThemeData snackBarTheme { get; private set; } = default!;
    public virtual SwitchThemeData switchTheme { get; private set; } = default!;
    public virtual TabBarThemeData tabBarTheme { get; private set; } = default!;
    public virtual TextButtonThemeData textButtonTheme { get; private set; } = default!;
    public virtual TextSelectionThemeData textSelectionTheme { get; private set; } = default!;
    public virtual TimePickerThemeData timePickerTheme { get; private set; } = default!;
    public virtual ToggleButtonsThemeData toggleButtonsTheme { get; private set; } = default!;
    public virtual TooltipThemeData tooltipTheme { get; private set; } = default!;
    internal virtual ButtonBarThemeData? _buttonBarTheme { get; private set; }
    public virtual Color dialogBackgroundColor { get; private set; } = default!;
    public virtual Color indicatorColor { get; private set; } = default!;
    internal const long _localizedThemeDataCacheSize = 5L;
    internal static _FifoCache__theme_data<_IdentityThemeDataCacheKey__theme_data, ThemeData> _localizedThemeDataCache = new _FifoCache__theme_data<_IdentityThemeDataCacheKey__theme_data, ThemeData>(_localizedThemeDataCacheSize);

    public static ThemeData Create(IEnumerable<Adaptation<object>>? adaptations = null, bool? applyElevationOverlayColor = null, NoDefaultCupertinoThemeData? cupertinoOverrideTheme = null, IEnumerable<ThemeExtension<object>>? extensions = null, object? inputDecorationTheme = null, MaterialTapTargetSize? materialTapTargetSize = null, PageTransitionsTheme? pageTransitionsTheme = null, global::Doroti.Framework.Foundation.TargetPlatform? platform = null, ScrollbarThemeData? scrollbarTheme = null, InteractiveInkFeatureFactory? splashFactory = null, bool? useSystemColors = null, VisualDensity? visualDensity = null, ColorScheme? colorScheme = null, Brightness? brightness = null, Color? colorSchemeSeed = null, Color? canvasColor = null, Color? cardColor = null, Color? disabledColor = null, Color? dividerColor = null, Color? focusColor = null, Color? highlightColor = null, Color? hintColor = null, Color? hoverColor = null, Color? primaryColor = null, Color? primaryColorDark = null, Color? primaryColorLight = null, MaterialColor? primarySwatch = null, Color? scaffoldBackgroundColor = null, Color? secondaryHeaderColor = null, Color? shadowColor = null, Color? splashColor = null, Color? unselectedWidgetColor = null, string? fontFamily = null, List<string>? fontFamilyFallback = null, string? package = null, global::Doroti.Framework.Widgets.IconThemeData? iconTheme = null, global::Doroti.Framework.Widgets.IconThemeData? primaryIconTheme = null, TextTheme? primaryTextTheme = null, TextTheme? textTheme = null, Typography? typography = null, ActionIconThemeData? actionIconTheme = null, object? appBarTheme = null, BadgeThemeData? badgeTheme = null, MaterialBannerThemeData? bannerTheme = null, BottomAppBarThemeData? bottomAppBarTheme = null, BottomNavigationBarThemeData? bottomNavigationBarTheme = null, BottomSheetThemeData? bottomSheetTheme = null, ButtonThemeData? buttonTheme = null, CardThemeData? cardTheme = null, CarouselViewThemeData? carouselViewTheme = null, CheckboxThemeData? checkboxTheme = null, ChipThemeData? chipTheme = null, DataTableThemeData? dataTableTheme = null, DatePickerThemeData? datePickerTheme = null, DialogThemeData? dialogTheme = null, DividerThemeData? dividerTheme = null, DrawerThemeData? drawerTheme = null, DropdownMenuThemeData? dropdownMenuTheme = null, ElevatedButtonThemeData? elevatedButtonTheme = null, ExpansionTileThemeData? expansionTileTheme = null, FilledButtonThemeData? filledButtonTheme = null, FloatingActionButtonThemeData? floatingActionButtonTheme = null, IconButtonThemeData? iconButtonTheme = null, ListTileThemeData? listTileTheme = null, MenuBarThemeData? menuBarTheme = null, MenuButtonThemeData? menuButtonTheme = null, MenuThemeData? menuTheme = null, NavigationBarThemeData? navigationBarTheme = null, NavigationDrawerThemeData? navigationDrawerTheme = null, NavigationRailThemeData? navigationRailTheme = null, OutlinedButtonThemeData? outlinedButtonTheme = null, PopupMenuThemeData? popupMenuTheme = null, ProgressIndicatorThemeData? progressIndicatorTheme = null, RadioThemeData? radioTheme = null, SearchBarThemeData? searchBarTheme = null, SearchViewThemeData? searchViewTheme = null, SegmentedButtonThemeData? segmentedButtonTheme = null, SliderThemeData? sliderTheme = null, SnackBarThemeData? snackBarTheme = null, SwitchThemeData? switchTheme = null, TabBarThemeData? tabBarTheme = null, TextButtonThemeData? textButtonTheme = null, TextSelectionThemeData? textSelectionTheme = null, TimePickerThemeData? timePickerTheme = null, ToggleButtonsThemeData? toggleButtonsTheme = null, TooltipThemeData? tooltipTheme = null, ButtonBarThemeData? buttonBarTheme = null, Color? dialogBackgroundColor = null, Color? indicatorColor = null)
    {
        cupertinoOverrideTheme = cupertinoOverrideTheme?.noDefault();
        extensions ??= new List<ThemeExtension<object>>();
        adaptations ??= new List<Adaptation<object>>();
        if (inputDecorationTheme is not null)
        {
            if (inputDecorationTheme is InputDecorationTheme)
            {
                inputDecorationTheme = ((InputDecorationTheme)inputDecorationTheme).data;
            }
            else
            {
                if (inputDecorationTheme is not InputDecorationThemeData)
                {
                    throw DartRuntimePrimitives.AsException(new DartArgumentError("inputDecorationTheme must be either a InputDecorationThemeData or a InputDecorationTheme"));
                }
            }
        }
        inputDecorationTheme ??= new InputDecorationThemeData();
        platform ??= PlatformLibrary.defaultTargetPlatform;
        switch (DartRuntimePrimitives.RequireValue(platform))
        {
            case TargetPlatform.android:
            case TargetPlatform.fuchsia:
            case TargetPlatform.iOS:
                {
                    materialTapTargetSize ??= MaterialTapTargetSize.padded;
                    break;
                }
            case TargetPlatform.linux:
            case TargetPlatform.macOS:
            case TargetPlatform.windows:
                {
                    materialTapTargetSize ??= MaterialTapTargetSize.shrinkWrap;
                    break;
                }
        }
        pageTransitionsTheme ??= new PageTransitionsTheme();
        scrollbarTheme ??= new ScrollbarThemeData();
        visualDensity ??= VisualDensity.defaultDensityForPlatform(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(platform)));
        useSystemColors ??= false;
        bool useInkSparkle = Equals(DartRuntimePrimitives.RequireValue(platform), TargetPlatform.android) && !Foundation.ConstantsLibrary.kIsWeb;
        splashFactory ??= (useInkSparkle ? InkSparkle.splashFactory : InkRipple.splashFactory);
        DartRuntimePrimitives.Assert(() => (colorScheme?.brightness is null) || (brightness is null) || Equals(colorScheme!.brightness, DartRuntimePrimitives.RequireValue(brightness)), () => (object?)"ThemeData.brightness does not match ColorScheme.brightness. " + "Either override ColorScheme.brightness or ThemeData.brightness to " + "match the other.");
        DartRuntimePrimitives.Assert(() => (colorSchemeSeed is null) || (colorScheme is null));
        DartRuntimePrimitives.Assert(() => (colorSchemeSeed is null) || (primarySwatch is null));
        DartRuntimePrimitives.Assert(() => (colorSchemeSeed is null) || (primaryColor is null));
        global::Doroti.Ui.Brightness effectiveBrightness = (brightness ?? colorScheme?.brightness) ?? Brightness.light;
        var isDark = Equals(effectiveBrightness, Brightness.dark);
        {
            if (colorSchemeSeed is not null)
            {
                colorScheme = ColorScheme.CreateFromSeed(seedColor: colorSchemeSeed, brightness: effectiveBrightness);
            }
            colorScheme ??= (isDark ? Theme_dataLibrary._colorSchemeDarkM3 : Theme_dataLibrary._colorSchemeLightM3);
            global::Doroti.Ui.Color primarySurfaceColor = isDark ? colorScheme.surface : colorScheme.primary;
            global::Doroti.Ui.Color onPrimarySurfaceColor = isDark ? colorScheme.onSurface : colorScheme.onPrimary;
            primaryColor ??= primarySurfaceColor;
            canvasColor ??= colorScheme.surface;
            scaffoldBackgroundColor ??= colorScheme.surface;
            cardColor ??= colorScheme.surface;
            dividerColor ??= colorScheme.outline;
            dialogBackgroundColor ??= colorScheme.surface;
            indicatorColor ??= onPrimarySurfaceColor;
            applyElevationOverlayColor ??= (Equals(brightness, Brightness.dark));
        }
        applyElevationOverlayColor ??= false;
        primarySwatch ??= Colors.blue;
        primaryColor ??= (isDark ? Colors.grey[900L]! : primarySwatch);
        global::Doroti.Ui.Brightness estimatedPrimaryColorBrightness = estimateBrightnessForColor(primaryColor);
        primaryColorLight ??= (isDark ? Colors.grey[500L]! : primarySwatch[100L]!);
        primaryColorDark ??= (isDark ? Colors.black : primarySwatch[700L]!);
        var primaryIsDark = Equals(estimatedPrimaryColorBrightness, Brightness.dark);
        focusColor ??= (isDark ? Colors.white.withOpacity(0.12) : Colors.black.withOpacity(0.12));
        hoverColor ??= (isDark ? Colors.white.withOpacity(0.04) : Colors.black.withOpacity(0.04));
        shadowColor ??= Colors.black;
        canvasColor ??= (isDark ? Colors.grey[850L]! : Colors.grey[50L]!);
        scaffoldBackgroundColor ??= canvasColor;
        cardColor ??= (isDark ? Colors.grey[800L]! : Colors.white);
        dividerColor ??= (isDark ? new global::Doroti.Ui.Color(536870911L) : new global::Doroti.Ui.Color(520093696L));
        colorScheme ??= ColorScheme.CreateFromSwatch(primarySwatch: primarySwatch, accentColor: isDark ? Colors.tealAccent[200L]! : primarySwatch[500L]!, cardColor: cardColor, backgroundColor: isDark ? Colors.grey[700L]! : primarySwatch[200L]!, errorColor: Colors.red[700L], brightness: effectiveBrightness);
        unselectedWidgetColor ??= (isDark ? Colors.white70 : Colors.black54);
        secondaryHeaderColor ??= (isDark ? Colors.grey[700L]! : primarySwatch[50L]!);
        hintColor ??= (isDark ? Colors.white60 : Colors.black.withOpacity(0.6));
        buttonTheme ??= new ButtonThemeData(colorScheme: colorScheme, buttonColor: isDark ? primarySwatch[600L]! : Colors.grey[300L]!, disabledColor: disabledColor, focusColor: focusColor, hoverColor: hoverColor, highlightColor: highlightColor, splashColor: splashColor, materialTapTargetSize: DartRuntimePrimitives.RequireValue(materialTapTargetSize));
        disabledColor ??= (isDark ? Colors.white38 : Colors.black38);
        highlightColor ??= (isDark ? new global::Doroti.Ui.Color(1087163596L) : new global::Doroti.Ui.Color(1723645116L));
        splashColor ??= (isDark ? new global::Doroti.Ui.Color(1087163596L) : new global::Doroti.Ui.Color(1724434632L));
        typography ??= (Typography.CreateMaterial2021(platform: DartRuntimePrimitives.RequireValue(platform), colorScheme: colorScheme));
        TextTheme defaultTextTheme = isDark ? typography.white : typography.black;
        TextTheme defaultPrimaryTextTheme = primaryIsDark ? typography.white : typography.black;
        if (fontFamily is not null)
        {
            defaultTextTheme = defaultTextTheme.apply(fontFamily: fontFamily);
            defaultPrimaryTextTheme = defaultPrimaryTextTheme.apply(fontFamily: fontFamily);
        }
        if (fontFamilyFallback is not null)
        {
            defaultTextTheme = defaultTextTheme.apply(fontFamilyFallback: fontFamilyFallback);
            defaultPrimaryTextTheme = defaultPrimaryTextTheme.apply(fontFamilyFallback: fontFamilyFallback);
        }
        if (package is not null)
        {
            defaultTextTheme = defaultTextTheme.apply(package: package);
            defaultPrimaryTextTheme = defaultPrimaryTextTheme.apply(package: package);
        }
        textTheme = defaultTextTheme.merge(textTheme);
        primaryTextTheme = defaultPrimaryTextTheme.merge(primaryTextTheme);
        iconTheme ??= (isDark ? new global::Doroti.Framework.Widgets.IconThemeData(color: ConstantsLibrary.kDefaultIconLightColor) : new global::Doroti.Framework.Widgets.IconThemeData(color: ConstantsLibrary.kDefaultIconDarkColor));
        primaryIconTheme ??= (primaryIsDark ? new global::Doroti.Framework.Widgets.IconThemeData(color: Colors.white) : new global::Doroti.Framework.Widgets.IconThemeData(color: Colors.black));
        if (appBarTheme is not null)
        {
            if (appBarTheme is AppBarTheme)
            {
                appBarTheme = ((AppBarTheme)appBarTheme).data;
            }
            else
            {
                if (appBarTheme is not AppBarThemeData)
                {
                    throw DartRuntimePrimitives.AsException(new DartArgumentError("appBarTheme must be either a AppBarThemeData or a AppBarTheme"));
                }
            }
        }
        badgeTheme ??= new BadgeThemeData();
        bannerTheme ??= new MaterialBannerThemeData();
        bottomAppBarTheme ??= new BottomAppBarThemeData();
        bottomNavigationBarTheme ??= new BottomNavigationBarThemeData();
        bottomSheetTheme ??= new BottomSheetThemeData();
        cardTheme ??= new CardThemeData();
        carouselViewTheme ??= new CarouselViewThemeData();
        checkboxTheme ??= new CheckboxThemeData();
        chipTheme ??= new ChipThemeData();
        dataTableTheme ??= new DataTableThemeData();
        datePickerTheme ??= new DatePickerThemeData();
        dialogTheme ??= new DialogThemeData();
        dividerTheme ??= new DividerThemeData();
        drawerTheme ??= new DrawerThemeData();
        dropdownMenuTheme ??= new DropdownMenuThemeData();
        elevatedButtonTheme ??= new ElevatedButtonThemeData();
        expansionTileTheme ??= new ExpansionTileThemeData();
        filledButtonTheme ??= new FilledButtonThemeData();
        floatingActionButtonTheme ??= new FloatingActionButtonThemeData();
        iconButtonTheme ??= new IconButtonThemeData();
        listTileTheme ??= new ListTileThemeData();
        menuBarTheme ??= new MenuBarThemeData();
        menuButtonTheme ??= new MenuButtonThemeData();
        menuTheme ??= new MenuThemeData();
        navigationBarTheme ??= new NavigationBarThemeData();
        navigationDrawerTheme ??= new NavigationDrawerThemeData();
        navigationRailTheme ??= new NavigationRailThemeData();
        outlinedButtonTheme ??= new OutlinedButtonThemeData();
        popupMenuTheme ??= new PopupMenuThemeData();
        progressIndicatorTheme ??= new ProgressIndicatorThemeData();
        radioTheme ??= new RadioThemeData();
        searchBarTheme ??= new SearchBarThemeData();
        searchViewTheme ??= new SearchViewThemeData();
        segmentedButtonTheme ??= new SegmentedButtonThemeData();
        sliderTheme ??= new SliderThemeData();
        snackBarTheme ??= new SnackBarThemeData();
        switchTheme ??= new SwitchThemeData();
        tabBarTheme ??= new TabBarThemeData();
        textButtonTheme ??= new TextButtonThemeData();
        textSelectionTheme ??= new TextSelectionThemeData();
        timePickerTheme ??= new TimePickerThemeData();
        toggleButtonsTheme ??= new ToggleButtonsThemeData();
        tooltipTheme ??= new TooltipThemeData();
        buttonBarTheme ??= new ButtonBarThemeData();
        dialogBackgroundColor ??= (isDark ? Colors.grey[800L]! : Colors.white);
        indicatorColor ??= (Equals(colorScheme.secondary, primaryColor) ? Colors.white : colorScheme.secondary);
        var theme = new ThemeData(adaptationMap: _createAdaptationMap(adaptations.Cast<Adaptation<object>>()), applyElevationOverlayColor: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(applyElevationOverlayColor)), cupertinoOverrideTheme: cupertinoOverrideTheme, extensions: _themeExtensionIterableToMap(extensions), inputDecorationTheme: ((InputDecorationThemeData?)inputDecorationTheme)!, materialTapTargetSize: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(materialTapTargetSize)), pageTransitionsTheme: pageTransitionsTheme, platform: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(platform)), scrollbarTheme: scrollbarTheme, splashFactory: splashFactory, visualDensity: visualDensity, canvasColor: canvasColor, cardColor: cardColor, colorScheme: colorScheme, disabledColor: disabledColor, dividerColor: dividerColor, focusColor: focusColor, highlightColor: highlightColor, hintColor: hintColor, hoverColor: hoverColor, primaryColor: primaryColor, primaryColorDark: primaryColorDark, primaryColorLight: primaryColorLight, scaffoldBackgroundColor: scaffoldBackgroundColor, secondaryHeaderColor: secondaryHeaderColor, shadowColor: shadowColor, splashColor: splashColor, unselectedWidgetColor: unselectedWidgetColor, iconTheme: iconTheme, primaryTextTheme: primaryTextTheme, textTheme: textTheme, typography: typography, primaryIconTheme: primaryIconTheme, actionIconTheme: actionIconTheme, appBarTheme: ((AppBarThemeData?)appBarTheme)! ?? new AppBarThemeData(), badgeTheme: badgeTheme, bannerTheme: bannerTheme, bottomAppBarTheme: bottomAppBarTheme, bottomNavigationBarTheme: bottomNavigationBarTheme, bottomSheetTheme: bottomSheetTheme, buttonTheme: buttonTheme, cardTheme: cardTheme, carouselViewTheme: carouselViewTheme, checkboxTheme: checkboxTheme, chipTheme: chipTheme, dataTableTheme: dataTableTheme, datePickerTheme: datePickerTheme, dialogTheme: dialogTheme, dividerTheme: dividerTheme, drawerTheme: drawerTheme, dropdownMenuTheme: dropdownMenuTheme, elevatedButtonTheme: elevatedButtonTheme, expansionTileTheme: expansionTileTheme, filledButtonTheme: filledButtonTheme, floatingActionButtonTheme: floatingActionButtonTheme, iconButtonTheme: iconButtonTheme, listTileTheme: listTileTheme, menuBarTheme: menuBarTheme, menuButtonTheme: menuButtonTheme, menuTheme: menuTheme, navigationBarTheme: navigationBarTheme, navigationDrawerTheme: navigationDrawerTheme, navigationRailTheme: navigationRailTheme, outlinedButtonTheme: outlinedButtonTheme, popupMenuTheme: popupMenuTheme, progressIndicatorTheme: progressIndicatorTheme, radioTheme: radioTheme, searchBarTheme: searchBarTheme, searchViewTheme: searchViewTheme, segmentedButtonTheme: segmentedButtonTheme, sliderTheme: sliderTheme, snackBarTheme: snackBarTheme, switchTheme: switchTheme, tabBarTheme: tabBarTheme, textButtonTheme: textButtonTheme, textSelectionTheme: textSelectionTheme, timePickerTheme: timePickerTheme, toggleButtonsTheme: toggleButtonsTheme, tooltipTheme: tooltipTheme, buttonBarTheme: buttonBarTheme, dialogBackgroundColor: dialogBackgroundColor, indicatorColor: indicatorColor);
        if (DartRuntimePrimitives.RequireValue(useSystemColors))
        {
            theme = theme._overrideWithSystemColors();
        }
        return theme;
    }

    public ThemeData(DartMap<Type, Adaptation<object>> adaptationMap, bool applyElevationOverlayColor, NoDefaultCupertinoThemeData? cupertinoOverrideTheme, DartMap<object, ThemeExtension<object>> extensions, InputDecorationThemeData inputDecorationTheme, MaterialTapTargetSize materialTapTargetSize, PageTransitionsTheme pageTransitionsTheme, global::Doroti.Framework.Foundation.TargetPlatform platform, ScrollbarThemeData scrollbarTheme, InteractiveInkFeatureFactory splashFactory, VisualDensity visualDensity, ColorScheme colorScheme, Color canvasColor, Color cardColor, Color disabledColor, Color dividerColor, Color focusColor, Color highlightColor, Color hintColor, Color hoverColor, Color primaryColor, Color primaryColorDark, Color primaryColorLight, Color scaffoldBackgroundColor, Color secondaryHeaderColor, Color shadowColor, Color splashColor, Color unselectedWidgetColor, global::Doroti.Framework.Widgets.IconThemeData iconTheme, global::Doroti.Framework.Widgets.IconThemeData primaryIconTheme, TextTheme primaryTextTheme, TextTheme textTheme, Typography typography, ActionIconThemeData? actionIconTheme, AppBarThemeData appBarTheme, BadgeThemeData badgeTheme, MaterialBannerThemeData bannerTheme, BottomAppBarThemeData bottomAppBarTheme, BottomNavigationBarThemeData bottomNavigationBarTheme, BottomSheetThemeData bottomSheetTheme, ButtonThemeData buttonTheme, CardThemeData cardTheme, CarouselViewThemeData carouselViewTheme, CheckboxThemeData checkboxTheme, ChipThemeData chipTheme, DataTableThemeData dataTableTheme, DatePickerThemeData datePickerTheme, DialogThemeData dialogTheme, DividerThemeData dividerTheme, DrawerThemeData drawerTheme, DropdownMenuThemeData dropdownMenuTheme, ElevatedButtonThemeData elevatedButtonTheme, ExpansionTileThemeData expansionTileTheme, FilledButtonThemeData filledButtonTheme, FloatingActionButtonThemeData floatingActionButtonTheme, IconButtonThemeData iconButtonTheme, ListTileThemeData listTileTheme, MenuBarThemeData menuBarTheme, MenuButtonThemeData menuButtonTheme, MenuThemeData menuTheme, NavigationBarThemeData navigationBarTheme, NavigationDrawerThemeData navigationDrawerTheme, NavigationRailThemeData navigationRailTheme, OutlinedButtonThemeData outlinedButtonTheme, PopupMenuThemeData popupMenuTheme, ProgressIndicatorThemeData progressIndicatorTheme, RadioThemeData radioTheme, SearchBarThemeData searchBarTheme, SearchViewThemeData searchViewTheme, SegmentedButtonThemeData segmentedButtonTheme, SliderThemeData sliderTheme, SnackBarThemeData snackBarTheme, SwitchThemeData switchTheme, TabBarThemeData tabBarTheme, TextButtonThemeData textButtonTheme, TextSelectionThemeData textSelectionTheme, TimePickerThemeData timePickerTheme, ToggleButtonsThemeData toggleButtonsTheme, TooltipThemeData tooltipTheme, ButtonBarThemeData? buttonBarTheme = null, Color dialogBackgroundColor = default!, Color indicatorColor = default!)
    {
        this.adaptationMap = adaptationMap;
        this.applyElevationOverlayColor = applyElevationOverlayColor;
        this.cupertinoOverrideTheme = cupertinoOverrideTheme;
        this.extensions = extensions;
        this.inputDecorationTheme = inputDecorationTheme;
        this.materialTapTargetSize = materialTapTargetSize;
        this.pageTransitionsTheme = pageTransitionsTheme;
        this.platform = platform;
        this.scrollbarTheme = scrollbarTheme;
        this.splashFactory = splashFactory;
        this.visualDensity = visualDensity;
        this.colorScheme = colorScheme;
        this.canvasColor = canvasColor;
        this.cardColor = cardColor;
        this.disabledColor = disabledColor;
        this.dividerColor = dividerColor;
        this.focusColor = focusColor;
        this.highlightColor = highlightColor;
        this.hintColor = hintColor;
        this.hoverColor = hoverColor;
        this.primaryColor = primaryColor;
        this.primaryColorDark = primaryColorDark;
        this.primaryColorLight = primaryColorLight;
        this.scaffoldBackgroundColor = scaffoldBackgroundColor;
        this.secondaryHeaderColor = secondaryHeaderColor;
        this.shadowColor = shadowColor;
        this.splashColor = splashColor;
        this.unselectedWidgetColor = unselectedWidgetColor;
        this.iconTheme = iconTheme;
        this.primaryIconTheme = primaryIconTheme;
        this.primaryTextTheme = primaryTextTheme;
        this.textTheme = textTheme;
        this.typography = typography;
        this.actionIconTheme = actionIconTheme;
        this.appBarTheme = appBarTheme;
        this.badgeTheme = badgeTheme;
        this.bannerTheme = bannerTheme;
        this.bottomAppBarTheme = bottomAppBarTheme;
        this.bottomNavigationBarTheme = bottomNavigationBarTheme;
        this.bottomSheetTheme = bottomSheetTheme;
        this.buttonTheme = buttonTheme;
        this.cardTheme = cardTheme;
        this.carouselViewTheme = carouselViewTheme;
        this.checkboxTheme = checkboxTheme;
        this.chipTheme = chipTheme;
        this.dataTableTheme = dataTableTheme;
        this.datePickerTheme = datePickerTheme;
        this.dialogTheme = dialogTheme;
        this.dividerTheme = dividerTheme;
        this.drawerTheme = drawerTheme;
        this.dropdownMenuTheme = dropdownMenuTheme;
        this.elevatedButtonTheme = elevatedButtonTheme;
        this.expansionTileTheme = expansionTileTheme;
        this.filledButtonTheme = filledButtonTheme;
        this.floatingActionButtonTheme = floatingActionButtonTheme;
        this.iconButtonTheme = iconButtonTheme;
        this.listTileTheme = listTileTheme;
        this.menuBarTheme = menuBarTheme;
        this.menuButtonTheme = menuButtonTheme;
        this.menuTheme = menuTheme;
        this.navigationBarTheme = navigationBarTheme;
        this.navigationDrawerTheme = navigationDrawerTheme;
        this.navigationRailTheme = navigationRailTheme;
        this.outlinedButtonTheme = outlinedButtonTheme;
        this.popupMenuTheme = popupMenuTheme;
        this.progressIndicatorTheme = progressIndicatorTheme;
        this.radioTheme = radioTheme;
        this.searchBarTheme = searchBarTheme;
        this.searchViewTheme = searchViewTheme;
        this.segmentedButtonTheme = segmentedButtonTheme;
        this.sliderTheme = sliderTheme;
        this.snackBarTheme = snackBarTheme;
        this.switchTheme = switchTheme;
        this.tabBarTheme = tabBarTheme;
        this.textButtonTheme = textButtonTheme;
        this.textSelectionTheme = textSelectionTheme;
        this.timePickerTheme = timePickerTheme;
        this.toggleButtonsTheme = toggleButtonsTheme;
        this.tooltipTheme = tooltipTheme;
        this.dialogBackgroundColor = dialogBackgroundColor;
        this.indicatorColor = indicatorColor;
        _buttonBarTheme = buttonBarTheme;
        System.Diagnostics.Debug.Assert(buttonBarTheme is not null);
    }

    public static ThemeData CreateFrom(ColorScheme colorScheme, TextTheme? textTheme = null)
    {
        var isDark = Equals(colorScheme.brightness, Brightness.dark);
        global::Doroti.Ui.Color primarySurfaceColor = isDark ? colorScheme.surface : colorScheme.primary;
        global::Doroti.Ui.Color onPrimarySurfaceColor = isDark ? colorScheme.onSurface : colorScheme.onPrimary;
        return Create(colorScheme: colorScheme, brightness: colorScheme.brightness, primaryColor: primarySurfaceColor, canvasColor: colorScheme.surface, scaffoldBackgroundColor: colorScheme.surface, cardColor: colorScheme.surface, dividerColor: colorScheme.onSurface.withOpacity(0.12), dialogBackgroundColor: colorScheme.surface, indicatorColor: onPrimarySurfaceColor, textTheme: textTheme, applyElevationOverlayColor: isDark);
    }

    public static ThemeData CreateLight() => Create(brightness: Brightness.light);

    public static ThemeData CreateDark() => Create(brightness: Brightness.dark);

    public static ThemeData CreateFallback() => CreateLight();

    public virtual Adaptation<T>? getAdaptation<T>() => ((Adaptation<T>?)(object?)adaptationMap.GetValueOrDefault(typeof(T)))!;
    internal static DartMap<Type, Adaptation<object>> _createAdaptationMap(IEnumerable<Adaptation<object>> adaptations)
    {
        var adaptationMap = new DartMap<Type, Adaptation<object>>();
        return adaptationMap;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.Brightness brightness => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Brightness>(colorScheme.brightness);
    public virtual T? extension<T>() => ((T?)(object?)extensions.GetValueOrDefault(typeof(T)))!;
    public virtual ButtonBarThemeData buttonBarTheme => DartRuntimePrimitives.ConvertValue<ButtonBarThemeData>(_buttonBarTheme!);
    public virtual ThemeData copyWith(IEnumerable<Adaptation<object>>? adaptations = null, bool? applyElevationOverlayColor = null, NoDefaultCupertinoThemeData? cupertinoOverrideTheme = null, IEnumerable<ThemeExtension<object>>? extensions = null, object? inputDecorationTheme = null, MaterialTapTargetSize? materialTapTargetSize = null, PageTransitionsTheme? pageTransitionsTheme = null, global::Doroti.Framework.Foundation.TargetPlatform? platform = null, ScrollbarThemeData? scrollbarTheme = null, InteractiveInkFeatureFactory? splashFactory = null, VisualDensity? visualDensity = null, ColorScheme? colorScheme = null, Brightness? brightness = null, Color? canvasColor = null, Color? cardColor = null, Color? disabledColor = null, Color? dividerColor = null, Color? focusColor = null, Color? highlightColor = null, Color? hintColor = null, Color? hoverColor = null, Color? primaryColor = null, Color? primaryColorDark = null, Color? primaryColorLight = null, Color? scaffoldBackgroundColor = null, Color? secondaryHeaderColor = null, Color? shadowColor = null, Color? splashColor = null, Color? unselectedWidgetColor = null, global::Doroti.Framework.Widgets.IconThemeData? iconTheme = null, global::Doroti.Framework.Widgets.IconThemeData? primaryIconTheme = null, TextTheme? primaryTextTheme = null, TextTheme? textTheme = null, Typography? typography = null, ActionIconThemeData? actionIconTheme = null, object? appBarTheme = null, BadgeThemeData? badgeTheme = null, MaterialBannerThemeData? bannerTheme = null, BottomAppBarThemeData? bottomAppBarTheme = null, BottomNavigationBarThemeData? bottomNavigationBarTheme = null, BottomSheetThemeData? bottomSheetTheme = null, ButtonThemeData? buttonTheme = null, CardThemeData? cardTheme = null, CarouselViewThemeData? carouselViewTheme = null, CheckboxThemeData? checkboxTheme = null, ChipThemeData? chipTheme = null, DataTableThemeData? dataTableTheme = null, DatePickerThemeData? datePickerTheme = null, DialogThemeData? dialogTheme = null, DividerThemeData? dividerTheme = null, DrawerThemeData? drawerTheme = null, DropdownMenuThemeData? dropdownMenuTheme = null, ElevatedButtonThemeData? elevatedButtonTheme = null, ExpansionTileThemeData? expansionTileTheme = null, FilledButtonThemeData? filledButtonTheme = null, FloatingActionButtonThemeData? floatingActionButtonTheme = null, IconButtonThemeData? iconButtonTheme = null, ListTileThemeData? listTileTheme = null, MenuBarThemeData? menuBarTheme = null, MenuButtonThemeData? menuButtonTheme = null, MenuThemeData? menuTheme = null, NavigationBarThemeData? navigationBarTheme = null, NavigationDrawerThemeData? navigationDrawerTheme = null, NavigationRailThemeData? navigationRailTheme = null, OutlinedButtonThemeData? outlinedButtonTheme = null, PopupMenuThemeData? popupMenuTheme = null, ProgressIndicatorThemeData? progressIndicatorTheme = null, RadioThemeData? radioTheme = null, SearchBarThemeData? searchBarTheme = null, SearchViewThemeData? searchViewTheme = null, SegmentedButtonThemeData? segmentedButtonTheme = null, SliderThemeData? sliderTheme = null, SnackBarThemeData? snackBarTheme = null, SwitchThemeData? switchTheme = null, TabBarThemeData? tabBarTheme = null, TextButtonThemeData? textButtonTheme = null, TextSelectionThemeData? textSelectionTheme = null, TimePickerThemeData? timePickerTheme = null, ToggleButtonsThemeData? toggleButtonsTheme = null, TooltipThemeData? tooltipTheme = null, ButtonBarThemeData? buttonBarTheme = null, Color? dialogBackgroundColor = null, Color? indicatorColor = null)
    {
        cupertinoOverrideTheme = cupertinoOverrideTheme?.noDefault();
        if (inputDecorationTheme is not null)
        {
            if (inputDecorationTheme is InputDecorationTheme)
            {
                inputDecorationTheme = ((InputDecorationTheme)inputDecorationTheme).data;
            }
            else
            {
                if (inputDecorationTheme is not InputDecorationThemeData)
                {
                    throw DartRuntimePrimitives.AsException(new DartArgumentError("inputDecorationTheme must be either a InputDecorationThemeData or a InputDecorationTheme"));
                }
            }
        }
        return new ThemeData(adaptationMap: (adaptations is not null) ? _createAdaptationMap(adaptations.Cast<Adaptation<object>>()) : adaptationMap, applyElevationOverlayColor: applyElevationOverlayColor ?? this.applyElevationOverlayColor, cupertinoOverrideTheme: cupertinoOverrideTheme ?? this.cupertinoOverrideTheme, extensions: (extensions is not null) ? _themeExtensionIterableToMap(extensions) : this.extensions, inputDecorationTheme: ((InputDecorationThemeData?)inputDecorationTheme)! ?? this.inputDecorationTheme, materialTapTargetSize: materialTapTargetSize ?? this.materialTapTargetSize, pageTransitionsTheme: pageTransitionsTheme ?? this.pageTransitionsTheme, platform: platform ?? this.platform, scrollbarTheme: scrollbarTheme ?? this.scrollbarTheme, splashFactory: splashFactory ?? this.splashFactory, visualDensity: visualDensity ?? this.visualDensity, canvasColor: canvasColor ?? this.canvasColor, cardColor: cardColor ?? this.cardColor, colorScheme: (colorScheme ?? this.colorScheme).copyWith(brightness: brightness), disabledColor: disabledColor ?? this.disabledColor, dividerColor: dividerColor ?? this.dividerColor, focusColor: focusColor ?? this.focusColor, highlightColor: highlightColor ?? this.highlightColor, hintColor: hintColor ?? this.hintColor, hoverColor: hoverColor ?? this.hoverColor, primaryColor: primaryColor ?? this.primaryColor, primaryColorDark: primaryColorDark ?? this.primaryColorDark, primaryColorLight: primaryColorLight ?? this.primaryColorLight, scaffoldBackgroundColor: scaffoldBackgroundColor ?? this.scaffoldBackgroundColor, secondaryHeaderColor: secondaryHeaderColor ?? this.secondaryHeaderColor, shadowColor: shadowColor ?? this.shadowColor, splashColor: splashColor ?? this.splashColor, unselectedWidgetColor: unselectedWidgetColor ?? this.unselectedWidgetColor, iconTheme: iconTheme ?? this.iconTheme, primaryIconTheme: primaryIconTheme ?? this.primaryIconTheme, primaryTextTheme: primaryTextTheme ?? this.primaryTextTheme, textTheme: textTheme ?? this.textTheme, typography: typography ?? this.typography, actionIconTheme: actionIconTheme ?? this.actionIconTheme, appBarTheme: ((global::System.Func<AppBarThemeData>)(() =>
        {
            if (appBarTheme is not null)
            {
                if (appBarTheme is AppBarTheme)
                {
                    AppBarTheme appBarTheme__as70567 = (AppBarTheme)appBarTheme;
                    return appBarTheme__as70567.data;
                }
                else
                {
                    if (appBarTheme is not AppBarThemeData)
                    {
                        throw DartRuntimePrimitives.AsException(new DartArgumentError("appBarTheme must be either a AppBarThemeData or a AppBarTheme"));
                    }
                }
            }
            return ((AppBarThemeData?)appBarTheme)! ?? this.appBarTheme;
            throw new InvalidOperationException("Dart closure completed without a value.");
        }))(), badgeTheme: badgeTheme ?? this.badgeTheme, bannerTheme: bannerTheme ?? this.bannerTheme, bottomAppBarTheme: bottomAppBarTheme ?? this.bottomAppBarTheme, bottomNavigationBarTheme: bottomNavigationBarTheme ?? this.bottomNavigationBarTheme, bottomSheetTheme: bottomSheetTheme ?? this.bottomSheetTheme, buttonTheme: buttonTheme ?? this.buttonTheme, cardTheme: cardTheme ?? this.cardTheme, carouselViewTheme: carouselViewTheme ?? this.carouselViewTheme, checkboxTheme: checkboxTheme ?? this.checkboxTheme, chipTheme: chipTheme ?? this.chipTheme, dataTableTheme: dataTableTheme ?? this.dataTableTheme, datePickerTheme: datePickerTheme ?? this.datePickerTheme, dialogTheme: dialogTheme ?? this.dialogTheme, dividerTheme: dividerTheme ?? this.dividerTheme, drawerTheme: drawerTheme ?? this.drawerTheme, dropdownMenuTheme: dropdownMenuTheme ?? this.dropdownMenuTheme, elevatedButtonTheme: elevatedButtonTheme ?? this.elevatedButtonTheme, expansionTileTheme: expansionTileTheme ?? this.expansionTileTheme, filledButtonTheme: filledButtonTheme ?? this.filledButtonTheme, floatingActionButtonTheme: floatingActionButtonTheme ?? this.floatingActionButtonTheme, iconButtonTheme: iconButtonTheme ?? this.iconButtonTheme, listTileTheme: listTileTheme ?? this.listTileTheme, menuBarTheme: menuBarTheme ?? this.menuBarTheme, menuButtonTheme: menuButtonTheme ?? this.menuButtonTheme, menuTheme: menuTheme ?? this.menuTheme, navigationBarTheme: navigationBarTheme ?? this.navigationBarTheme, navigationDrawerTheme: navigationDrawerTheme ?? this.navigationDrawerTheme, navigationRailTheme: navigationRailTheme ?? this.navigationRailTheme, outlinedButtonTheme: outlinedButtonTheme ?? this.outlinedButtonTheme, popupMenuTheme: popupMenuTheme ?? this.popupMenuTheme, progressIndicatorTheme: progressIndicatorTheme ?? this.progressIndicatorTheme, radioTheme: radioTheme ?? this.radioTheme, searchBarTheme: searchBarTheme ?? this.searchBarTheme, searchViewTheme: searchViewTheme ?? this.searchViewTheme, segmentedButtonTheme: segmentedButtonTheme ?? this.segmentedButtonTheme, sliderTheme: sliderTheme ?? this.sliderTheme, snackBarTheme: snackBarTheme ?? this.snackBarTheme, switchTheme: switchTheme ?? this.switchTheme, tabBarTheme: tabBarTheme ?? this.tabBarTheme, textButtonTheme: textButtonTheme ?? this.textButtonTheme, textSelectionTheme: textSelectionTheme ?? this.textSelectionTheme, timePickerTheme: timePickerTheme ?? this.timePickerTheme, toggleButtonsTheme: toggleButtonsTheme ?? this.toggleButtonsTheme, tooltipTheme: tooltipTheme ?? this.tooltipTheme, buttonBarTheme: buttonBarTheme ?? _buttonBarTheme, dialogBackgroundColor: dialogBackgroundColor ?? this.dialogBackgroundColor, indicatorColor: indicatorColor ?? this.indicatorColor);
    }

    public static ThemeData localize(ThemeData baseTheme, TextTheme localTextGeometry)
    {
        return _localizedThemeDataCache.putIfAbsent(new _IdentityThemeDataCacheKey__theme_data(baseTheme, localTextGeometry), () =>
        {
            return baseTheme.copyWith(primaryTextTheme: localTextGeometry.merge(baseTheme.primaryTextTheme), textTheme: localTextGeometry.merge(baseTheme.textTheme));
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static global::Doroti.Ui.Brightness estimateBrightnessForColor(Color color)
    {
        double relativeLuminance = color.computeLuminance();
        var kThreshold = 0.15;
        if (((relativeLuminance + 0.05) * (relativeLuminance + 0.05)) > kThreshold)
        {
            return Brightness.light;
        }
        return Brightness.dark;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static DartMap<object, ThemeExtension<object>> _lerpThemeExtensions(ThemeData a, ThemeData b, double t)
    {
        DartMap<object, ThemeExtension<object>> newExtensions = a.extensions.map<object, ThemeExtension<object>, object, ThemeExtension<object>>((id, extensionA) =>
        {
            ThemeExtension<object>? extensionB = b.extensions.GetValueOrDefault(id);
            return new MapEntry<object, ThemeExtension<object>>(id, extensionA.lerp(extensionB, t));
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        newExtensions.addEntries(b.extensions.entries.where((entry) => !a.extensions.ContainsKey(entry.key)).Cast<MapEntry<object, ThemeExtension<object>>>());
        return newExtensions;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static DartMap<object, ThemeExtension<object>> _themeExtensionIterableToMap(IEnumerable<ThemeExtension<object>> extensionsIterable)
    {
        return new DartMap<object, ThemeExtension<object>>(new DartMap<object, ThemeExtension<object>>());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual ThemeData _overrideWithSystemColors()
    {
        if (!SystemColor.platformProvidesSystemColors)
        {
            return this;
        }
        global::Doroti.Ui.SystemColorPalette systemColors = Equals(brightness, Brightness.dark) ? SystemColor.dark : SystemColor.light;
        var theme = this;
        theme = theme.copyWith(colorScheme: colorScheme.copyWith(secondary: systemColors.accentColor.value, onSecondary: systemColors.accentColorText.value, surface: systemColors.canvas.value, onSurface: systemColors.canvasText.value), textTheme: textTheme.apply(displayColor: systemColors.canvasText.value, bodyColor: systemColors.canvasText.value));
        bool overrideButtons = (systemColors.buttonFace.value is not null) || (systemColors.buttonBorder.value is not null) || (systemColors.buttonText.value is not null);
        if (overrideButtons)
        {
            theme = theme.copyWith(elevatedButtonTheme: new ElevatedButtonThemeData(style: ElevatedButton.styleFrom(foregroundColor: systemColors.buttonText.value, backgroundColor: systemColors.buttonFace.value, side: (systemColors.buttonBorder.value is null) ? null : new global::Doroti.Framework.Painting.BorderSide(color: systemColors.buttonBorder.value!))), textButtonTheme: new TextButtonThemeData(style: TextButton.styleFrom(foregroundColor: systemColors.buttonText.value, backgroundColor: systemColors.buttonFace.value, side: (systemColors.buttonBorder.value is null) ? null : new global::Doroti.Framework.Painting.BorderSide(color: systemColors.buttonBorder.value!))), outlinedButtonTheme: new OutlinedButtonThemeData(style: OutlinedButton.styleFrom(foregroundColor: systemColors.buttonText.value, backgroundColor: systemColors.buttonFace.value, side: (systemColors.buttonBorder.value is null) ? null : new global::Doroti.Framework.Painting.BorderSide(color: systemColors.buttonBorder.value!))), filledButtonTheme: new FilledButtonThemeData(style: FilledButton.styleFrom(foregroundColor: systemColors.buttonText.value, backgroundColor: systemColors.buttonFace.value, side: (systemColors.buttonBorder.value is null) ? null : new global::Doroti.Framework.Painting.BorderSide(color: systemColors.buttonBorder.value!))), floatingActionButtonTheme: new FloatingActionButtonThemeData(backgroundColor: systemColors.buttonFace.value, foregroundColor: systemColors.buttonText.value));
        }
        bool overrideInputDecoration = (systemColors.field.value is not null) || (systemColors.fieldText.value is not null);
        if (overrideInputDecoration)
        {
            theme = theme.copyWith(inputDecorationTheme: inputDecorationTheme.copyWith(fillColor: systemColors.field.value, labelStyle: inputDecorationTheme.labelStyle?.copyWith(color: systemColors.fieldText.value) ?? new global::Doroti.Framework.Painting.TextStyle(color: systemColors.fieldText.value), hintStyle: inputDecorationTheme.hintStyle?.copyWith(color: systemColors.fieldText.value) ?? new global::Doroti.Framework.Painting.TextStyle(color: systemColors.fieldText.value), helperStyle: inputDecorationTheme.helperStyle?.copyWith(color: systemColors.fieldText.value) ?? new global::Doroti.Framework.Painting.TextStyle(color: systemColors.fieldText.value), prefixStyle: inputDecorationTheme.prefixStyle?.copyWith(color: systemColors.fieldText.value) ?? new global::Doroti.Framework.Painting.TextStyle(color: systemColors.fieldText.value), suffixStyle: inputDecorationTheme.suffixStyle?.copyWith(color: systemColors.fieldText.value) ?? new global::Doroti.Framework.Painting.TextStyle(color: systemColors.fieldText.value), counterStyle: inputDecorationTheme.counterStyle?.copyWith(color: systemColors.fieldText.value) ?? new global::Doroti.Framework.Painting.TextStyle(color: systemColors.fieldText.value)));
        }
        return theme;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static ThemeData lerp(ThemeData a, ThemeData b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        return new ThemeData(adaptationMap: (t < 0.5) ? a.adaptationMap : b.adaptationMap, applyElevationOverlayColor: (t < 0.5) ? a.applyElevationOverlayColor : b.applyElevationOverlayColor, cupertinoOverrideTheme: (t < 0.5) ? a.cupertinoOverrideTheme : b.cupertinoOverrideTheme, extensions: _lerpThemeExtensions(a, b, t), inputDecorationTheme: (t < 0.5) ? a.inputDecorationTheme : b.inputDecorationTheme, materialTapTargetSize: (t < 0.5) ? a.materialTapTargetSize : b.materialTapTargetSize, pageTransitionsTheme: (t < 0.5) ? a.pageTransitionsTheme : b.pageTransitionsTheme, platform: (t < 0.5) ? a.platform : b.platform, scrollbarTheme: ScrollbarThemeData.lerp(a.scrollbarTheme, b.scrollbarTheme, t), splashFactory: (t < 0.5) ? a.splashFactory : b.splashFactory, visualDensity: VisualDensity.lerp(a.visualDensity, b.visualDensity, t), canvasColor: Dart_uiLibrary.Color.lerp(a.canvasColor, b.canvasColor, t)!, cardColor: Dart_uiLibrary.Color.lerp(a.cardColor, b.cardColor, t)!, colorScheme: ColorScheme.lerp(a.colorScheme, b.colorScheme, t), disabledColor: Dart_uiLibrary.Color.lerp(a.disabledColor, b.disabledColor, t)!, dividerColor: Dart_uiLibrary.Color.lerp(a.dividerColor, b.dividerColor, t)!, focusColor: Dart_uiLibrary.Color.lerp(a.focusColor, b.focusColor, t)!, highlightColor: Dart_uiLibrary.Color.lerp(a.highlightColor, b.highlightColor, t)!, hintColor: Dart_uiLibrary.Color.lerp(a.hintColor, b.hintColor, t)!, hoverColor: Dart_uiLibrary.Color.lerp(a.hoverColor, b.hoverColor, t)!, primaryColor: Dart_uiLibrary.Color.lerp(a.primaryColor, b.primaryColor, t)!, primaryColorDark: Dart_uiLibrary.Color.lerp(a.primaryColorDark, b.primaryColorDark, t)!, primaryColorLight: Dart_uiLibrary.Color.lerp(a.primaryColorLight, b.primaryColorLight, t)!, scaffoldBackgroundColor: Dart_uiLibrary.Color.lerp(a.scaffoldBackgroundColor, b.scaffoldBackgroundColor, t)!, secondaryHeaderColor: Dart_uiLibrary.Color.lerp(a.secondaryHeaderColor, b.secondaryHeaderColor, t)!, shadowColor: Dart_uiLibrary.Color.lerp(a.shadowColor, b.shadowColor, t)!, splashColor: Dart_uiLibrary.Color.lerp(a.splashColor, b.splashColor, t)!, unselectedWidgetColor: Dart_uiLibrary.Color.lerp(a.unselectedWidgetColor, b.unselectedWidgetColor, t)!, iconTheme: IconThemeData.lerp(a.iconTheme, b.iconTheme, t), primaryIconTheme: IconThemeData.lerp(a.primaryIconTheme, b.primaryIconTheme, t), primaryTextTheme: TextTheme.lerp(a.primaryTextTheme, b.primaryTextTheme, t), textTheme: TextTheme.lerp(a.textTheme, b.textTheme, t), typography: Typography.lerp(a.typography, b.typography, t), actionIconTheme: ActionIconThemeData.lerp(a.actionIconTheme, b.actionIconTheme, t), appBarTheme: AppBarThemeData.lerp(a.appBarTheme, b.appBarTheme, t), badgeTheme: BadgeThemeData.lerp(a.badgeTheme, b.badgeTheme, t), bannerTheme: MaterialBannerThemeData.lerp(a.bannerTheme, b.bannerTheme, t), bottomAppBarTheme: BottomAppBarThemeData.lerp(a.bottomAppBarTheme, b.bottomAppBarTheme, t), bottomNavigationBarTheme: BottomNavigationBarThemeData.lerp(a.bottomNavigationBarTheme, b.bottomNavigationBarTheme, t), bottomSheetTheme: BottomSheetThemeData.lerp(a.bottomSheetTheme, b.bottomSheetTheme, t)!, buttonTheme: (t < 0.5) ? a.buttonTheme : b.buttonTheme, cardTheme: CardThemeData.lerp(a.cardTheme, b.cardTheme, t), carouselViewTheme: CarouselViewThemeData.lerp(a.carouselViewTheme, b.carouselViewTheme, t), checkboxTheme: CheckboxThemeData.lerp(a.checkboxTheme, b.checkboxTheme, t), chipTheme: ChipThemeData.lerp(a.chipTheme, b.chipTheme, t)!, dataTableTheme: DataTableThemeData.lerp(a.dataTableTheme, b.dataTableTheme, t), datePickerTheme: DatePickerThemeData.lerp(a.datePickerTheme, b.datePickerTheme, t), dialogTheme: DialogThemeData.lerp(a.dialogTheme, b.dialogTheme, t), dividerTheme: DividerThemeData.lerp(a.dividerTheme, b.dividerTheme, t), drawerTheme: DrawerThemeData.lerp(a.drawerTheme, b.drawerTheme, t)!, dropdownMenuTheme: DropdownMenuThemeData.lerp(a.dropdownMenuTheme, b.dropdownMenuTheme, t), elevatedButtonTheme: ElevatedButtonThemeData.lerp(a.elevatedButtonTheme, b.elevatedButtonTheme, t)!, expansionTileTheme: ExpansionTileThemeData.lerp(a.expansionTileTheme, b.expansionTileTheme, t)!, filledButtonTheme: FilledButtonThemeData.lerp(a.filledButtonTheme, b.filledButtonTheme, t)!, floatingActionButtonTheme: FloatingActionButtonThemeData.lerp(a.floatingActionButtonTheme, b.floatingActionButtonTheme, t)!, iconButtonTheme: IconButtonThemeData.lerp(a.iconButtonTheme, b.iconButtonTheme, t)!, listTileTheme: ListTileThemeData.lerp(a.listTileTheme, b.listTileTheme, t)!, menuBarTheme: MenuBarThemeData.lerp(a.menuBarTheme, b.menuBarTheme, t)!, menuButtonTheme: MenuButtonThemeData.lerp(a.menuButtonTheme, b.menuButtonTheme, t)!, menuTheme: MenuThemeData.lerp(a.menuTheme, b.menuTheme, t)!, navigationBarTheme: NavigationBarThemeData.lerp(a.navigationBarTheme, b.navigationBarTheme, t)!, navigationDrawerTheme: NavigationDrawerThemeData.lerp(a.navigationDrawerTheme, b.navigationDrawerTheme, t)!, navigationRailTheme: NavigationRailThemeData.lerp(a.navigationRailTheme, b.navigationRailTheme, t)!, outlinedButtonTheme: OutlinedButtonThemeData.lerp(a.outlinedButtonTheme, b.outlinedButtonTheme, t)!, popupMenuTheme: PopupMenuThemeData.lerp(a.popupMenuTheme, b.popupMenuTheme, t)!, progressIndicatorTheme: ProgressIndicatorThemeData.lerp(a.progressIndicatorTheme, b.progressIndicatorTheme, t)!, radioTheme: RadioThemeData.lerp(a.radioTheme, b.radioTheme, t), searchBarTheme: SearchBarThemeData.lerp(a.searchBarTheme, b.searchBarTheme, t)!, searchViewTheme: SearchViewThemeData.lerp(a.searchViewTheme, b.searchViewTheme, t)!, segmentedButtonTheme: SegmentedButtonThemeData.lerp(a.segmentedButtonTheme, b.segmentedButtonTheme, t), sliderTheme: SliderThemeData.lerp(a.sliderTheme, b.sliderTheme, t), snackBarTheme: SnackBarThemeData.lerp(a.snackBarTheme, b.snackBarTheme, t), switchTheme: SwitchThemeData.lerp(a.switchTheme, b.switchTheme, t), tabBarTheme: TabBarThemeData.lerp(a.tabBarTheme, b.tabBarTheme, t), textButtonTheme: TextButtonThemeData.lerp(a.textButtonTheme, b.textButtonTheme, t)!, textSelectionTheme: TextSelectionThemeData.lerp(a.textSelectionTheme, b.textSelectionTheme, t)!, timePickerTheme: TimePickerThemeData.lerp(a.timePickerTheme, b.timePickerTheme, t), toggleButtonsTheme: ToggleButtonsThemeData.lerp(a.toggleButtonsTheme, b.toggleButtonsTheme, t)!, tooltipTheme: TooltipThemeData.lerp(a.tooltipTheme, b.tooltipTheme, t)!, buttonBarTheme: ButtonBarThemeData.lerp(a.buttonBarTheme, b.buttonBarTheme, t), dialogBackgroundColor: Dart_uiLibrary.Color.lerp(a.dialogBackgroundColor, b.dialogBackgroundColor, t)!, indicatorColor: Dart_uiLibrary.Color.lerp(a.indicatorColor, b.indicatorColor, t)!);
    }

    public override bool Equals(object? other)
    {
        var __other = other as ThemeData;
        if (__other is null) return false;
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is ThemeData) && CollectionsLibrary.mapEquals(__other.adaptationMap, adaptationMap) && (__other.applyElevationOverlayColor == applyElevationOverlayColor) && Equals(__other.cupertinoOverrideTheme, cupertinoOverrideTheme) && CollectionsLibrary.mapEquals(__other.extensions, extensions) && Equals(__other.inputDecorationTheme, inputDecorationTheme) && Equals(__other.materialTapTargetSize, materialTapTargetSize) && Equals(__other.pageTransitionsTheme, pageTransitionsTheme) && Equals(__other.platform, platform) && Equals(__other.scrollbarTheme, scrollbarTheme) && Equals(__other.splashFactory, splashFactory) && Equals(__other.visualDensity, visualDensity) && Equals(__other.canvasColor, canvasColor) && Equals(__other.cardColor, cardColor) && Equals(__other.colorScheme, colorScheme) && Equals(__other.disabledColor, disabledColor) && Equals(__other.dividerColor, dividerColor) && Equals(__other.focusColor, focusColor) && Equals(__other.highlightColor, highlightColor) && Equals(__other.hintColor, hintColor) && Equals(__other.hoverColor, hoverColor) && Equals(__other.primaryColor, primaryColor) && Equals(__other.primaryColorDark, primaryColorDark) && Equals(__other.primaryColorLight, primaryColorLight) && Equals(__other.scaffoldBackgroundColor, scaffoldBackgroundColor) && Equals(__other.secondaryHeaderColor, secondaryHeaderColor) && Equals(__other.shadowColor, shadowColor) && Equals(__other.splashColor, splashColor) && Equals(__other.unselectedWidgetColor, unselectedWidgetColor) && Equals(__other.iconTheme, iconTheme) && Equals(__other.primaryIconTheme, primaryIconTheme) && Equals(__other.primaryTextTheme, primaryTextTheme) && Equals(__other.textTheme, textTheme) && Equals(__other.typography, typography) && Equals(__other.actionIconTheme, actionIconTheme) && Equals(__other.appBarTheme, appBarTheme) && Equals(__other.badgeTheme, badgeTheme) && Equals(__other.bannerTheme, bannerTheme) && Equals(__other.bottomAppBarTheme, bottomAppBarTheme) && Equals(__other.bottomNavigationBarTheme, bottomNavigationBarTheme) && Equals(__other.bottomSheetTheme, bottomSheetTheme) && Equals(__other.buttonTheme, buttonTheme) && Equals(__other.cardTheme, cardTheme) && Equals(__other.carouselViewTheme, carouselViewTheme) && Equals(__other.checkboxTheme, checkboxTheme) && Equals(__other.chipTheme, chipTheme) && Equals(__other.dataTableTheme, dataTableTheme) && Equals(__other.datePickerTheme, datePickerTheme) && Equals(__other.dialogTheme, dialogTheme) && Equals(__other.dividerTheme, dividerTheme) && Equals(__other.drawerTheme, drawerTheme) && Equals(__other.dropdownMenuTheme, dropdownMenuTheme) && Equals(__other.elevatedButtonTheme, elevatedButtonTheme) && Equals(__other.expansionTileTheme, expansionTileTheme) && Equals(__other.filledButtonTheme, filledButtonTheme) && Equals(__other.floatingActionButtonTheme, floatingActionButtonTheme) && Equals(__other.iconButtonTheme, iconButtonTheme) && Equals(__other.listTileTheme, listTileTheme) && Equals(__other.menuBarTheme, menuBarTheme) && Equals(__other.menuButtonTheme, menuButtonTheme) && Equals(__other.menuTheme, menuTheme) && Equals(__other.navigationBarTheme, navigationBarTheme) && Equals(__other.navigationDrawerTheme, navigationDrawerTheme) && Equals(__other.navigationRailTheme, navigationRailTheme) && Equals(__other.outlinedButtonTheme, outlinedButtonTheme) && Equals(__other.popupMenuTheme, popupMenuTheme) && Equals(__other.progressIndicatorTheme, progressIndicatorTheme) && Equals(__other.radioTheme, radioTheme) && Equals(__other.searchBarTheme, searchBarTheme) && Equals(__other.searchViewTheme, searchViewTheme) && Equals(__other.segmentedButtonTheme, segmentedButtonTheme) && Equals(__other.sliderTheme, sliderTheme) && Equals(__other.snackBarTheme, snackBarTheme) && Equals(__other.switchTheme, switchTheme) && Equals(__other.tabBarTheme, tabBarTheme) && Equals(__other.textButtonTheme, textButtonTheme) && Equals(__other.textSelectionTheme, textSelectionTheme) && Equals(__other.timePickerTheme, timePickerTheme) && Equals(__other.toggleButtonsTheme, toggleButtonsTheme) && Equals(__other.tooltipTheme, tooltipTheme) && Equals(__other.buttonBarTheme, buttonBarTheme) && Equals(__other.dialogBackgroundColor, dialogBackgroundColor) && Equals(__other.indicatorColor, indicatorColor);
    }

    public override int GetHashCode()
    {
        var values = ((Func<List<object?>>)(() => { var __collection95592 = new List<object?>(); __collection95592.AddRange(adaptationMap.Keys); __collection95592.AddRange(adaptationMap.Values); __collection95592.Add(applyElevationOverlayColor); __collection95592.Add(cupertinoOverrideTheme); __collection95592.AddRange(extensions.Keys); __collection95592.AddRange(extensions.Values); __collection95592.Add(inputDecorationTheme); __collection95592.Add(materialTapTargetSize); __collection95592.Add(pageTransitionsTheme); __collection95592.Add(platform); __collection95592.Add(scrollbarTheme); __collection95592.Add(splashFactory); __collection95592.Add(visualDensity); __collection95592.Add(canvasColor); __collection95592.Add(cardColor); __collection95592.Add(colorScheme); __collection95592.Add(disabledColor); __collection95592.Add(dividerColor); __collection95592.Add(focusColor); __collection95592.Add(highlightColor); __collection95592.Add(hintColor); __collection95592.Add(hoverColor); __collection95592.Add(primaryColor); __collection95592.Add(primaryColorDark); __collection95592.Add(primaryColorLight); __collection95592.Add(scaffoldBackgroundColor); __collection95592.Add(secondaryHeaderColor); __collection95592.Add(shadowColor); __collection95592.Add(splashColor); __collection95592.Add(unselectedWidgetColor); __collection95592.Add(iconTheme); __collection95592.Add(primaryIconTheme); __collection95592.Add(primaryTextTheme); __collection95592.Add(textTheme); __collection95592.Add(typography); __collection95592.Add(actionIconTheme); __collection95592.Add(appBarTheme); __collection95592.Add(badgeTheme); __collection95592.Add(bannerTheme); __collection95592.Add(bottomAppBarTheme); __collection95592.Add(bottomNavigationBarTheme); __collection95592.Add(bottomSheetTheme); __collection95592.Add(buttonTheme); __collection95592.Add(cardTheme); __collection95592.Add(carouselViewTheme); __collection95592.Add(checkboxTheme); __collection95592.Add(chipTheme); __collection95592.Add(dataTableTheme); __collection95592.Add(datePickerTheme); __collection95592.Add(dialogTheme); __collection95592.Add(dividerTheme); __collection95592.Add(drawerTheme); __collection95592.Add(dropdownMenuTheme); __collection95592.Add(elevatedButtonTheme); __collection95592.Add(expansionTileTheme); __collection95592.Add(filledButtonTheme); __collection95592.Add(floatingActionButtonTheme); __collection95592.Add(iconButtonTheme); __collection95592.Add(listTileTheme); __collection95592.Add(menuBarTheme); __collection95592.Add(menuButtonTheme); __collection95592.Add(menuTheme); __collection95592.Add(navigationBarTheme); __collection95592.Add(navigationDrawerTheme); __collection95592.Add(navigationRailTheme); __collection95592.Add(outlinedButtonTheme); __collection95592.Add(popupMenuTheme); __collection95592.Add(progressIndicatorTheme); __collection95592.Add(radioTheme); __collection95592.Add(searchBarTheme); __collection95592.Add(searchViewTheme); __collection95592.Add(segmentedButtonTheme); __collection95592.Add(sliderTheme); __collection95592.Add(snackBarTheme); __collection95592.Add(switchTheme); __collection95592.Add(tabBarTheme); __collection95592.Add(textButtonTheme); __collection95592.Add(textSelectionTheme); __collection95592.Add(timePickerTheme); __collection95592.Add(toggleButtonsTheme); __collection95592.Add(tooltipTheme); __collection95592.Add(buttonBarTheme); __collection95592.Add(dialogBackgroundColor); __collection95592.Add(indicatorColor); return __collection95592; }))();
        return FoundationRuntimePorts.ObjectHashAll(values);
    }
    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        var defaultData = CreateFallback();
        properties.add(new global::Doroti.Framework.Foundation.IterableProperty<Adaptation<object>>("adaptations", adaptationMap.Values.Cast<Adaptation<object>>(), defaultValue: defaultData.adaptationMap.Values, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("applyElevationOverlayColor", applyElevationOverlayColor, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<NoDefaultCupertinoThemeData>("cupertinoOverrideTheme", cupertinoOverrideTheme, defaultValue: defaultData.cupertinoOverrideTheme, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.IterableProperty<ThemeExtension<object>>("extensions", extensions.Values.Cast<ThemeExtension<object>>(), defaultValue: defaultData.extensions.Values, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<InputDecorationThemeData>("inputDecorationTheme", inputDecorationTheme, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<MaterialTapTargetSize>("materialTapTargetSize", materialTapTargetSize, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<PageTransitionsTheme>("pageTransitionsTheme", pageTransitionsTheme, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<global::Doroti.Framework.Foundation.TargetPlatform>("platform", platform, defaultValue: PlatformLibrary.defaultTargetPlatform, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<ScrollbarThemeData>("scrollbarTheme", scrollbarTheme, defaultValue: defaultData.scrollbarTheme, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<InteractiveInkFeatureFactory>("splashFactory", splashFactory, defaultValue: defaultData.splashFactory, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<VisualDensity>("visualDensity", visualDensity, defaultValue: defaultData.visualDensity, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("canvasColor", canvasColor, defaultValue: defaultData.canvasColor, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("cardColor", cardColor, defaultValue: defaultData.cardColor, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<ColorScheme>("colorScheme", colorScheme, defaultValue: defaultData.colorScheme, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("disabledColor", disabledColor, defaultValue: defaultData.disabledColor, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("dividerColor", dividerColor, defaultValue: defaultData.dividerColor, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("focusColor", focusColor, defaultValue: defaultData.focusColor, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("highlightColor", highlightColor, defaultValue: defaultData.highlightColor, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("hintColor", hintColor, defaultValue: defaultData.hintColor, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("hoverColor", hoverColor, defaultValue: defaultData.hoverColor, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("primaryColorDark", primaryColorDark, defaultValue: defaultData.primaryColorDark, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("primaryColorLight", primaryColorLight, defaultValue: defaultData.primaryColorLight, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("primaryColor", primaryColor, defaultValue: defaultData.primaryColor, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("scaffoldBackgroundColor", scaffoldBackgroundColor, defaultValue: defaultData.scaffoldBackgroundColor, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("secondaryHeaderColor", secondaryHeaderColor, defaultValue: defaultData.secondaryHeaderColor, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("shadowColor", shadowColor, defaultValue: defaultData.shadowColor, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("splashColor", splashColor, defaultValue: defaultData.splashColor, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("unselectedWidgetColor", unselectedWidgetColor, defaultValue: defaultData.unselectedWidgetColor, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.IconThemeData>("iconTheme", iconTheme, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.IconThemeData>("primaryIconTheme", primaryIconTheme, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<TextTheme>("primaryTextTheme", primaryTextTheme, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<TextTheme>("textTheme", textTheme, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<Typography>("typography", typography, defaultValue: defaultData.typography, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<ActionIconThemeData>("actionIconTheme", actionIconTheme, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<AppBarThemeData>("appBarTheme", appBarTheme, defaultValue: defaultData.appBarTheme, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<BadgeThemeData>("badgeTheme", badgeTheme, defaultValue: defaultData.badgeTheme, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<MaterialBannerThemeData>("bannerTheme", bannerTheme, defaultValue: defaultData.bannerTheme, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<BottomAppBarThemeData>("bottomAppBarTheme", bottomAppBarTheme, defaultValue: defaultData.bottomAppBarTheme, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<BottomNavigationBarThemeData>("bottomNavigationBarTheme", bottomNavigationBarTheme, defaultValue: defaultData.bottomNavigationBarTheme, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<BottomSheetThemeData>("bottomSheetTheme", bottomSheetTheme, defaultValue: defaultData.bottomSheetTheme, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<ButtonThemeData>("buttonTheme", buttonTheme, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<CardThemeData>("cardTheme", cardTheme, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<CarouselViewThemeData>("carouselViewTheme", carouselViewTheme, defaultValue: defaultData.carouselViewTheme, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<CheckboxThemeData>("checkboxTheme", checkboxTheme, defaultValue: defaultData.checkboxTheme, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<ChipThemeData>("chipTheme", chipTheme, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<DataTableThemeData>("dataTableTheme", dataTableTheme, defaultValue: defaultData.dataTableTheme, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<DatePickerThemeData>("datePickerTheme", datePickerTheme, defaultValue: defaultData.datePickerTheme, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<DialogThemeData>("dialogTheme", dialogTheme, defaultValue: defaultData.dialogTheme, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<DividerThemeData>("dividerTheme", dividerTheme, defaultValue: defaultData.dividerTheme, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<DrawerThemeData>("drawerTheme", drawerTheme, defaultValue: defaultData.drawerTheme, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<DropdownMenuThemeData>("dropdownMenuTheme", dropdownMenuTheme, defaultValue: defaultData.dropdownMenuTheme, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<ElevatedButtonThemeData>("elevatedButtonTheme", elevatedButtonTheme, defaultValue: defaultData.elevatedButtonTheme, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<ExpansionTileThemeData>("expansionTileTheme", expansionTileTheme, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<FilledButtonThemeData>("filledButtonTheme", filledButtonTheme, defaultValue: defaultData.filledButtonTheme, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<FloatingActionButtonThemeData>("floatingActionButtonTheme", floatingActionButtonTheme, defaultValue: defaultData.floatingActionButtonTheme, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<IconButtonThemeData>("iconButtonTheme", iconButtonTheme, defaultValue: defaultData.iconButtonTheme, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<ListTileThemeData>("listTileTheme", listTileTheme, defaultValue: defaultData.listTileTheme, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<MenuBarThemeData>("menuBarTheme", menuBarTheme, defaultValue: defaultData.menuBarTheme, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<MenuButtonThemeData>("menuButtonTheme", menuButtonTheme, defaultValue: defaultData.menuButtonTheme, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<MenuThemeData>("menuTheme", menuTheme, defaultValue: defaultData.menuTheme, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<NavigationBarThemeData>("navigationBarTheme", navigationBarTheme, defaultValue: defaultData.navigationBarTheme, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<NavigationDrawerThemeData>("navigationDrawerTheme", navigationDrawerTheme, defaultValue: defaultData.navigationDrawerTheme, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<NavigationRailThemeData>("navigationRailTheme", navigationRailTheme, defaultValue: defaultData.navigationRailTheme, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<OutlinedButtonThemeData>("outlinedButtonTheme", outlinedButtonTheme, defaultValue: defaultData.outlinedButtonTheme, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<PopupMenuThemeData>("popupMenuTheme", popupMenuTheme, defaultValue: defaultData.popupMenuTheme, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<ProgressIndicatorThemeData>("progressIndicatorTheme", progressIndicatorTheme, defaultValue: defaultData.progressIndicatorTheme, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<RadioThemeData>("radioTheme", radioTheme, defaultValue: defaultData.radioTheme, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<SearchBarThemeData>("searchBarTheme", searchBarTheme, defaultValue: defaultData.searchBarTheme, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<SearchViewThemeData>("searchViewTheme", searchViewTheme, defaultValue: defaultData.searchViewTheme, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<SegmentedButtonThemeData>("segmentedButtonTheme", segmentedButtonTheme, defaultValue: defaultData.segmentedButtonTheme, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<SliderThemeData>("sliderTheme", sliderTheme, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<SnackBarThemeData>("snackBarTheme", snackBarTheme, defaultValue: defaultData.snackBarTheme, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<SwitchThemeData>("switchTheme", switchTheme, defaultValue: defaultData.switchTheme, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<TabBarThemeData>("tabBarTheme", tabBarTheme, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<TextButtonThemeData>("textButtonTheme", textButtonTheme, defaultValue: defaultData.textButtonTheme, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<TextSelectionThemeData>("textSelectionTheme", textSelectionTheme, defaultValue: defaultData.textSelectionTheme, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<TimePickerThemeData>("timePickerTheme", timePickerTheme, defaultValue: defaultData.timePickerTheme, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<ToggleButtonsThemeData>("toggleButtonsTheme", toggleButtonsTheme, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<TooltipThemeData>("tooltipTheme", tooltipTheme, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<ButtonBarThemeData>("buttonBarTheme", buttonBarTheme, defaultValue: defaultData.buttonBarTheme, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("dialogBackgroundColor", dialogBackgroundColor, defaultValue: defaultData.dialogBackgroundColor, level: DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("indicatorColor", indicatorColor, defaultValue: defaultData.indicatorColor, level: DiagnosticLevel.debug));
    }

    public virtual string toStringShort() => DiagnosticsLibrary.describeIdentity(this);
    public override string ToString() => ToString(DiagnosticLevel.info);

    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                fullString = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine).toDiagnosticsNode().toStringDeep(minLevel: minLevel);
                return true;
            });
        return fullString ?? toStringShort();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(string? name = null, DiagnosticsTreeStyle? style = null)
    {
        return new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class MaterialBasedCupertinoThemeData : CupertinoThemeData
{
    internal virtual ThemeData _materialTheme { get; private set; } = default!;
    internal virtual NoDefaultCupertinoThemeData _cupertinoOverrideTheme { get; private set; } = default!;

    public MaterialBasedCupertinoThemeData(ThemeData materialTheme) : this(materialTheme, (materialTheme.cupertinoOverrideTheme ?? new CupertinoThemeData()).noDefault())
    {
    }

    public MaterialBasedCupertinoThemeData(ThemeData _materialTheme, NoDefaultCupertinoThemeData _cupertinoOverrideTheme) : base(_cupertinoOverrideTheme.brightness, _cupertinoOverrideTheme.primaryColor, _cupertinoOverrideTheme.primaryContrastingColor, _cupertinoOverrideTheme.textTheme, _cupertinoOverrideTheme.barBackgroundColor, _cupertinoOverrideTheme.scaffoldBackgroundColor, _cupertinoOverrideTheme.selectionHandleColor ?? _materialTheme.textSelectionTheme.selectionHandleColor, _cupertinoOverrideTheme.applyThemeToAll)
    {
        this._materialTheme = _materialTheme;
        this._cupertinoOverrideTheme = _cupertinoOverrideTheme;
    }

    public override global::Doroti.Ui.Brightness? brightness => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Brightness>(_cupertinoOverrideTheme.brightness ?? _materialTheme.brightness);
    public override global::Doroti.Ui.Color primaryColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_cupertinoOverrideTheme.primaryColor ?? _materialTheme.colorScheme.primary);
    public override global::Doroti.Ui.Color primaryContrastingColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_cupertinoOverrideTheme.primaryContrastingColor ?? _materialTheme.colorScheme.onPrimary);
    public override global::Doroti.Ui.Color scaffoldBackgroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_cupertinoOverrideTheme.scaffoldBackgroundColor ?? _materialTheme.scaffoldBackgroundColor);
    public override MaterialBasedCupertinoThemeData copyWith(Brightness? brightness = null, Color? primaryColor = null, Color? primaryContrastingColor = null, CupertinoTextThemeData? textTheme = null, Color? barBackgroundColor = null, Color? scaffoldBackgroundColor = null, Color? selectionHandleColor = null, bool? applyThemeToAll = null)
    {
        return new MaterialBasedCupertinoThemeData(_materialTheme, _cupertinoOverrideTheme.copyWith(brightness: brightness, primaryColor: primaryColor, primaryContrastingColor: primaryContrastingColor, textTheme: textTheme, barBackgroundColor: barBackgroundColor, scaffoldBackgroundColor: scaffoldBackgroundColor, selectionHandleColor: selectionHandleColor, applyThemeToAll: applyThemeToAll));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override CupertinoThemeData resolveFrom(global::Doroti.Framework.Widgets.BuildContext context)
    {
        NoDefaultCupertinoThemeData cupertinoOverrideThemeWithTextTheme = _cupertinoOverrideTheme.copyWith(textTheme: textTheme);
        return new MaterialBasedCupertinoThemeData(_materialTheme, cupertinoOverrideThemeWithTextTheme.resolveFrom(context));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class CupertinoBasedMaterialThemeData
{
    public virtual ThemeData materialTheme { get; private set; } = default!;

    public CupertinoBasedMaterialThemeData(CupertinoThemeData themeData)
    {
        materialTheme = ThemeData.Create(colorScheme: ColorScheme.CreateFromSeed(seedColor: themeData.primaryColor, brightness: themeData.brightness ?? Brightness.light, primary: themeData.primaryColor, onPrimary: themeData.primaryContrastingColor));
    }

}

internal class _IdentityThemeDataCacheKey__theme_data
{
    public virtual ThemeData baseTheme { get; private set; } = default!;
    public virtual TextTheme localTextGeometry { get; private set; } = default!;

    internal _IdentityThemeDataCacheKey__theme_data(ThemeData baseTheme, TextTheme localTextGeometry)
    {
        this.baseTheme = baseTheme;
        this.localTextGeometry = localTextGeometry;
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(Dart_coreLibrary.identityHashCode(baseTheme) ^ Dart_coreLibrary.identityHashCode(localTextGeometry));
    public override bool Equals(object? other)
    {
        var __other = other as _IdentityThemeDataCacheKey__theme_data;
        if (__other is null) return false;
        return (__other is _IdentityThemeDataCacheKey__theme_data) && DartRuntimePrimitives.Identical(__other.baseTheme, baseTheme) && DartRuntimePrimitives.Identical(__other.localTextGeometry, localTextGeometry);
    }

}

internal class _FifoCache__theme_data<K, V> where K : notnull
{
    internal virtual DartMap<K, V> _cache { get; private set; } = new DartMap<K, V>();
    internal virtual long _maximumSize { get; private set; } = default!;

    internal _FifoCache__theme_data(long _maximumSize)
    {
        this._maximumSize = _maximumSize;
        System.Diagnostics.Debug.Assert(_maximumSize > 0L);
    }

    public virtual V putIfAbsent(K key, global::System.Func<V> loader)
    {
        DartRuntimePrimitives.Assert(() => key is not null);
        V? result = _cache.GetValueOrDefault(key);
        if (result is not null)
        {
            return result;
        }
        if (checked(_cache.Count) == _maximumSize)
        {
            _cache.remove(_cache.Keys.First());
        }
        return _cache[key] = loader();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class VisualDensity : global::Doroti.Framework.Foundation.Diagnosticable
{
    public static double minimumDensity = -4.0;
    public const double maximumDensity = 4.0;
    public static VisualDensity standard = new VisualDensity();
    public static VisualDensity comfortable = new VisualDensity(horizontal: -1.0, vertical: -1.0);
    public static VisualDensity compact = new VisualDensity(horizontal: -2.0, vertical: -2.0);
    public virtual double horizontal { get; private set; } = default!;
    public virtual double vertical { get; private set; } = default!;

    public VisualDensity(double horizontal = 0.0, double vertical = 0.0)
    {
        this.horizontal = horizontal;
        this.vertical = vertical;
        System.Diagnostics.Debug.Assert(vertical <= maximumDensity);
        System.Diagnostics.Debug.Assert(vertical >= minimumDensity);
        System.Diagnostics.Debug.Assert(horizontal <= maximumDensity);
        System.Diagnostics.Debug.Assert(horizontal >= minimumDensity);
    }

    public static VisualDensity adaptivePlatformDensity => defaultDensityForPlatform(PlatformLibrary.defaultTargetPlatform);
    public static VisualDensity defaultDensityForPlatform(global::Doroti.Framework.Foundation.TargetPlatform platform)
    {
        return platform switch { TargetPlatform.android or TargetPlatform.iOS => standard, TargetPlatform.fuchsia => standard, TargetPlatform.linux or TargetPlatform.macOS => compact, TargetPlatform.windows => compact, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual VisualDensity copyWith(double? horizontal = null, double? vertical = null)
    {
        return new VisualDensity(horizontal: horizontal ?? this.horizontal, vertical: vertical ?? this.vertical);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.Offset baseSizeAdjustment
    {
        get
        {
            var interval = 4.0;
            return new global::Doroti.Ui.Offset(horizontal, vertical) * interval;
        }
    }
    public static VisualDensity lerp(VisualDensity a, VisualDensity b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        return new VisualDensity(horizontal: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(a.horizontal, b.horizontal, t)), vertical: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(a.vertical, b.vertical, t)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Rendering.BoxConstraints effectiveConstraints(global::Doroti.Framework.Rendering.BoxConstraints constraints)
    {
        DartRuntimePrimitives.Assert(() => constraints.debugAssertIsValid());
        return constraints.copyWith(minWidth: Dart_uiLibrary.clampDouble(constraints.minWidth + baseSizeAdjustment.dx, 0.0, constraints.maxWidth), minHeight: Dart_uiLibrary.clampDouble(constraints.minHeight + baseSizeAdjustment.dy, 0.0, constraints.maxHeight));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as VisualDensity;
        if (__other is null) return false;
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is VisualDensity) && (__other.horizontal == horizontal) && (__other.vertical == vertical);
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(horizontal, vertical));
    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("horizontal", horizontal, defaultValue: 0.0));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("vertical", vertical, defaultValue: 0.0));
    }

    public virtual string toStringShort()
    {
        return $"{DiagnosticsLibrary.describeIdentity(this)}(h: {Foundation.DebugLibrary.debugFormatDouble(horizontal)}, v: {Foundation.DebugLibrary.debugFormatDouble(vertical)})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString() => ToString(DiagnosticLevel.info);

    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                fullString = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine).toDiagnosticsNode().toStringDeep(minLevel: minLevel);
                return true;
            });
        return fullString ?? toStringShort();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(string? name = null, DiagnosticsTreeStyle? style = null)
    {
        return new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public static partial class Theme_dataLibrary
{
    internal static ColorScheme _colorSchemeLightM3 = new ColorScheme(brightness: Brightness.light, primary: new global::Doroti.Ui.Color(4284960932L), onPrimary: new global::Doroti.Ui.Color(4294967295L), primaryContainer: new global::Doroti.Ui.Color(4293582335L), onPrimaryContainer: new global::Doroti.Ui.Color(4283381643L), primaryFixed: new global::Doroti.Ui.Color(4293582335L), primaryFixedDim: new global::Doroti.Ui.Color(4291869951L), onPrimaryFixed: new global::Doroti.Ui.Color(4280352861L), onPrimaryFixedVariant: new global::Doroti.Ui.Color(4283381643L), secondary: new global::Doroti.Ui.Color(4284636017L), onSecondary: new global::Doroti.Ui.Color(4294967295L), secondaryContainer: new global::Doroti.Ui.Color(4293451512L), onSecondaryContainer: new global::Doroti.Ui.Color(4283057240L), secondaryFixed: new global::Doroti.Ui.Color(4293451512L), secondaryFixedDim: new global::Doroti.Ui.Color(4291609308L), onSecondaryFixed: new global::Doroti.Ui.Color(4280097067L), onSecondaryFixedVariant: new global::Doroti.Ui.Color(4283057240L), tertiary: new global::Doroti.Ui.Color(4286403168L), onTertiary: new global::Doroti.Ui.Color(4294967295L), tertiaryContainer: new global::Doroti.Ui.Color(4294957284L), onTertiaryContainer: new global::Doroti.Ui.Color(4284693320L), tertiaryFixed: new global::Doroti.Ui.Color(4294957284L), tertiaryFixedDim: new global::Doroti.Ui.Color(4293900488L), onTertiaryFixed: new global::Doroti.Ui.Color(4281405725L), onTertiaryFixedVariant: new global::Doroti.Ui.Color(4284693320L), error: new global::Doroti.Ui.Color(4289930782L), onError: new global::Doroti.Ui.Color(4294967295L), errorContainer: new global::Doroti.Ui.Color(4294565596L), onErrorContainer: new global::Doroti.Ui.Color(4287372568L), background: new global::Doroti.Ui.Color(4294899711L), onBackground: new global::Doroti.Ui.Color(4280097568L), surface: new global::Doroti.Ui.Color(4294899711L), surfaceBright: new global::Doroti.Ui.Color(4294899711L), surfaceContainerLowest: new global::Doroti.Ui.Color(4294967295L), surfaceContainerLow: new global::Doroti.Ui.Color(4294439674L), surfaceContainer: new global::Doroti.Ui.Color(4294176247L), surfaceContainerHigh: new global::Doroti.Ui.Color(4293715696L), surfaceContainerHighest: new global::Doroti.Ui.Color(4293320937L), surfaceDim: new global::Doroti.Ui.Color(4292794593L), onSurface: new global::Doroti.Ui.Color(4280097568L), surfaceVariant: new global::Doroti.Ui.Color(4293386476L), onSurfaceVariant: new global::Doroti.Ui.Color(4282991951L), outline: new global::Doroti.Ui.Color(4286149758L), outlineVariant: new global::Doroti.Ui.Color(4291478736L), shadow: new global::Doroti.Ui.Color(4278190080L), scrim: new global::Doroti.Ui.Color(4278190080L), inverseSurface: new global::Doroti.Ui.Color(4281478965L), onInverseSurface: new global::Doroti.Ui.Color(4294307831L), inversePrimary: new global::Doroti.Ui.Color(4291869951L), surfaceTint: new global::Doroti.Ui.Color(4284960932L));
}

public static partial class Theme_dataLibrary
{
    internal static ColorScheme _colorSchemeDarkM3 = new ColorScheme(brightness: Brightness.dark, primary: new global::Doroti.Ui.Color(4291869951L), onPrimary: new global::Doroti.Ui.Color(4281867890L), primaryContainer: new global::Doroti.Ui.Color(4283381643L), onPrimaryContainer: new global::Doroti.Ui.Color(4293582335L), primaryFixed: new global::Doroti.Ui.Color(4293582335L), primaryFixedDim: new global::Doroti.Ui.Color(4291869951L), onPrimaryFixed: new global::Doroti.Ui.Color(4280352861L), onPrimaryFixedVariant: new global::Doroti.Ui.Color(4283381643L), secondary: new global::Doroti.Ui.Color(4291609308L), onSecondary: new global::Doroti.Ui.Color(4281544001L), secondaryContainer: new global::Doroti.Ui.Color(4283057240L), onSecondaryContainer: new global::Doroti.Ui.Color(4293451512L), secondaryFixed: new global::Doroti.Ui.Color(4293451512L), secondaryFixedDim: new global::Doroti.Ui.Color(4291609308L), onSecondaryFixed: new global::Doroti.Ui.Color(4280097067L), onSecondaryFixedVariant: new global::Doroti.Ui.Color(4283057240L), tertiary: new global::Doroti.Ui.Color(4293900488L), onTertiary: new global::Doroti.Ui.Color(4282983730L), tertiaryContainer: new global::Doroti.Ui.Color(4284693320L), onTertiaryContainer: new global::Doroti.Ui.Color(4294957284L), tertiaryFixed: new global::Doroti.Ui.Color(4294957284L), tertiaryFixedDim: new global::Doroti.Ui.Color(4293900488L), onTertiaryFixed: new global::Doroti.Ui.Color(4281405725L), onTertiaryFixedVariant: new global::Doroti.Ui.Color(4284693320L), error: new global::Doroti.Ui.Color(4294097077L), onError: new global::Doroti.Ui.Color(4284486672L), errorContainer: new global::Doroti.Ui.Color(4287372568L), onErrorContainer: new global::Doroti.Ui.Color(4294565596L), background: new global::Doroti.Ui.Color(4279505432L), onBackground: new global::Doroti.Ui.Color(4293320937L), surface: new global::Doroti.Ui.Color(4279505432L), surfaceBright: new global::Doroti.Ui.Color(4282071102L), surfaceContainerLowest: new global::Doroti.Ui.Color(4279176467L), surfaceContainerLow: new global::Doroti.Ui.Color(4280097568L), surfaceContainer: new global::Doroti.Ui.Color(4280360742L), surfaceContainerHigh: new global::Doroti.Ui.Color(4281018672L), surfaceContainerHighest: new global::Doroti.Ui.Color(4281742395L), surfaceDim: new global::Doroti.Ui.Color(4279505432L), onSurface: new global::Doroti.Ui.Color(4293320937L), surfaceVariant: new global::Doroti.Ui.Color(4282991951L), onSurfaceVariant: new global::Doroti.Ui.Color(4291478736L), outline: new global::Doroti.Ui.Color(4287860633L), outlineVariant: new global::Doroti.Ui.Color(4282991951L), shadow: new global::Doroti.Ui.Color(4278190080L), scrim: new global::Doroti.Ui.Color(4278190080L), inverseSurface: new global::Doroti.Ui.Color(4293320937L), onInverseSurface: new global::Doroti.Ui.Color(4281478965L), inversePrimary: new global::Doroti.Ui.Color(4284960932L), surfaceTint: new global::Doroti.Ui.Color(4291869951L));
}
