// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/theme_data.dart
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;
using Stopwatch = System.Diagnostics.Stopwatch;

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
    public virtual bool useMaterial3 { get; private set; } = default!;
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

    public static ThemeData Create(IEnumerable<Adaptation<object>>? adaptations = null, bool? applyElevationOverlayColor = null, NoDefaultCupertinoThemeData? cupertinoOverrideTheme = null, IEnumerable<ThemeExtension<object>>? extensions = null, object? inputDecorationTheme = null, MaterialTapTargetSize? materialTapTargetSize = null, PageTransitionsTheme? pageTransitionsTheme = null, global::Doroti.Framework.Foundation.TargetPlatform? platform = null, ScrollbarThemeData? scrollbarTheme = null, InteractiveInkFeatureFactory? splashFactory = null, bool? useMaterial3 = null, bool? useSystemColors = null, VisualDensity? visualDensity = null, ColorScheme? colorScheme = null, Brightness? brightness = null, Color? colorSchemeSeed = null, Color? canvasColor = null, Color? cardColor = null, Color? disabledColor = null, Color? dividerColor = null, Color? focusColor = null, Color? highlightColor = null, Color? hintColor = null, Color? hoverColor = null, Color? primaryColor = null, Color? primaryColorDark = null, Color? primaryColorLight = null, MaterialColor? primarySwatch = null, Color? scaffoldBackgroundColor = null, Color? secondaryHeaderColor = null, Color? shadowColor = null, Color? splashColor = null, Color? unselectedWidgetColor = null, string? fontFamily = null, List<string>? fontFamilyFallback = null, string? package = null, global::Doroti.Framework.Widgets.IconThemeData? iconTheme = null, global::Doroti.Framework.Widgets.IconThemeData? primaryIconTheme = null, TextTheme? primaryTextTheme = null, TextTheme? textTheme = null, Typography? typography = null, ActionIconThemeData? actionIconTheme = null, object? appBarTheme = null, BadgeThemeData? badgeTheme = null, MaterialBannerThemeData? bannerTheme = null, BottomAppBarThemeData? bottomAppBarTheme = null, BottomNavigationBarThemeData? bottomNavigationBarTheme = null, BottomSheetThemeData? bottomSheetTheme = null, ButtonThemeData? buttonTheme = null, CardThemeData? cardTheme = null, CarouselViewThemeData? carouselViewTheme = null, CheckboxThemeData? checkboxTheme = null, ChipThemeData? chipTheme = null, DataTableThemeData? dataTableTheme = null, DatePickerThemeData? datePickerTheme = null, DialogThemeData? dialogTheme = null, DividerThemeData? dividerTheme = null, DrawerThemeData? drawerTheme = null, DropdownMenuThemeData? dropdownMenuTheme = null, ElevatedButtonThemeData? elevatedButtonTheme = null, ExpansionTileThemeData? expansionTileTheme = null, FilledButtonThemeData? filledButtonTheme = null, FloatingActionButtonThemeData? floatingActionButtonTheme = null, IconButtonThemeData? iconButtonTheme = null, ListTileThemeData? listTileTheme = null, MenuBarThemeData? menuBarTheme = null, MenuButtonThemeData? menuButtonTheme = null, MenuThemeData? menuTheme = null, NavigationBarThemeData? navigationBarTheme = null, NavigationDrawerThemeData? navigationDrawerTheme = null, NavigationRailThemeData? navigationRailTheme = null, OutlinedButtonThemeData? outlinedButtonTheme = null, PopupMenuThemeData? popupMenuTheme = null, ProgressIndicatorThemeData? progressIndicatorTheme = null, RadioThemeData? radioTheme = null, SearchBarThemeData? searchBarTheme = null, SearchViewThemeData? searchViewTheme = null, SegmentedButtonThemeData? segmentedButtonTheme = null, SliderThemeData? sliderTheme = null, SnackBarThemeData? snackBarTheme = null, SwitchThemeData? switchTheme = null, TabBarThemeData? tabBarTheme = null, TextButtonThemeData? textButtonTheme = null, TextSelectionThemeData? textSelectionTheme = null, TimePickerThemeData? timePickerTheme = null, ToggleButtonsThemeData? toggleButtonsTheme = null, TooltipThemeData? tooltipTheme = null, ButtonBarThemeData? buttonBarTheme = null, Color? dialogBackgroundColor = null, Color? indicatorColor = null)
    {
        cupertinoOverrideTheme = cupertinoOverrideTheme?.noDefault();
        extensions ??= new List<ThemeExtension<object>>();
        adaptations ??= new List<Adaptation<object>>();
        if ((inputDecorationTheme is not null))
        {
            if ((inputDecorationTheme is InputDecorationTheme))
            {
                inputDecorationTheme = ((InputDecorationTheme)inputDecorationTheme).data;
            }
            else
            {
                if ((inputDecorationTheme is not InputDecorationThemeData))
                {
                    throw DartRuntimePrimitives.AsException(new DartArgumentError("inputDecorationTheme must be either a InputDecorationThemeData or a InputDecorationTheme"));
                }
            }
        }
        inputDecorationTheme ??= new InputDecorationThemeData();
        platform ??= global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform;
        switch (DartRuntimePrimitives.RequireValue(platform))
        {
            case global::Doroti.Framework.Foundation.TargetPlatform.android:
            case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
            case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
                {
                    materialTapTargetSize ??= MaterialTapTargetSize.padded;
                    break;
                }
            case global::Doroti.Framework.Foundation.TargetPlatform.linux:
            case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
            case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                {
                    materialTapTargetSize ??= MaterialTapTargetSize.shrinkWrap;
                    break;
                }
        }
        pageTransitionsTheme ??= new PageTransitionsTheme();
        scrollbarTheme ??= new ScrollbarThemeData();
        visualDensity ??= VisualDensity.defaultDensityForPlatform(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(platform)));
        useMaterial3 ??= true;
        useSystemColors ??= false;
        bool useInkSparkle = ((object.Equals(DartRuntimePrimitives.RequireValue(platform), global::Doroti.Framework.Foundation.TargetPlatform.android)) && !global::Doroti.Framework.Foundation.ConstantsLibrary.kIsWeb);
        splashFactory ??= (DartRuntimePrimitives.RequireValue(useMaterial3) ? (useInkSparkle ? InkSparkle.splashFactory : InkRipple.splashFactory) : InkSplash.splashFactory);
        DartRuntimePrimitives.Assert(() => (((colorScheme?.brightness is null) || (brightness is null)) || (object.Equals(colorScheme!.brightness, DartRuntimePrimitives.RequireValue(brightness)))), () => (object?)"ThemeData.brightness does not match ColorScheme.brightness. " + "Either override ColorScheme.brightness or ThemeData.brightness to " + "match the other.");
        DartRuntimePrimitives.Assert(() => ((colorSchemeSeed is null) || (colorScheme is null)));
        DartRuntimePrimitives.Assert(() => ((colorSchemeSeed is null) || (primarySwatch is null)));
        DartRuntimePrimitives.Assert(() => ((colorSchemeSeed is null) || (primaryColor is null)));
        global::Doroti.Ui.Brightness effectiveBrightness = ((brightness ?? colorScheme?.brightness) ?? Brightness.light);
        var isDark = (object.Equals(effectiveBrightness, Brightness.dark));
        if (((colorSchemeSeed is not null) || DartRuntimePrimitives.RequireValue(useMaterial3)))
        {
            if ((colorSchemeSeed is not null))
            {
                colorScheme = ColorScheme.CreateFromSeed(seedColor: colorSchemeSeed, brightness: effectiveBrightness);
            }
            colorScheme ??= (isDark ? Theme_dataLibrary._colorSchemeDarkM3 : Theme_dataLibrary._colorSchemeLightM3);
            global::Doroti.Ui.Color primarySurfaceColor = ((global::Doroti.Ui.Color)(object?)(isDark ? ((ColorScheme)colorScheme).surface : ((ColorScheme)colorScheme).primary));
            global::Doroti.Ui.Color onPrimarySurfaceColor = ((global::Doroti.Ui.Color)(object?)(isDark ? ((ColorScheme)colorScheme).onSurface : ((ColorScheme)colorScheme).onPrimary));
            primaryColor ??= primarySurfaceColor;
            canvasColor ??= ((ColorScheme)colorScheme).surface;
            scaffoldBackgroundColor ??= ((ColorScheme)colorScheme).surface;
            cardColor ??= ((ColorScheme)colorScheme).surface;
            dividerColor ??= ((ColorScheme)colorScheme).outline;
            dialogBackgroundColor ??= ((ColorScheme)colorScheme).surface;
            indicatorColor ??= onPrimarySurfaceColor;
            applyElevationOverlayColor ??= (object.Equals(brightness, Brightness.dark));
        }
        applyElevationOverlayColor ??= false;
        primarySwatch ??= Colors.blue;
        primaryColor ??= (isDark ? Colors.grey[900L]! : primarySwatch);
        global::Doroti.Ui.Brightness estimatedPrimaryColorBrightness = ThemeData.estimateBrightnessForColor(primaryColor);
        primaryColorLight ??= (isDark ? Colors.grey[500L]! : primarySwatch[100L]!);
        primaryColorDark ??= (isDark ? Colors.black : primarySwatch[700L]!);
        var primaryIsDark = (object.Equals(estimatedPrimaryColorBrightness, Brightness.dark));
        focusColor ??= (isDark ? Colors.white.withOpacity(0.12) : Colors.black.withOpacity(0.12));
        hoverColor ??= (isDark ? Colors.white.withOpacity(0.04) : Colors.black.withOpacity(0.04));
        shadowColor ??= Colors.black;
        canvasColor ??= (isDark ? Colors.grey[850L]! : Colors.grey[50L]!);
        scaffoldBackgroundColor ??= canvasColor;
        cardColor ??= (isDark ? Colors.grey[800L]! : Colors.white);
        dividerColor ??= (isDark ? new global::Doroti.Ui.Color(536870911L) : new global::Doroti.Ui.Color(520093696L));
        colorScheme ??= ColorScheme.CreateFromSwatch(primarySwatch: primarySwatch, accentColor: (isDark ? Colors.tealAccent[200L]! : primarySwatch[500L]!), cardColor: cardColor, backgroundColor: (isDark ? Colors.grey[700L]! : primarySwatch[200L]!), errorColor: Colors.red[700L], brightness: effectiveBrightness);
        unselectedWidgetColor ??= (isDark ? Colors.white70 : Colors.black54);
        secondaryHeaderColor ??= (isDark ? Colors.grey[700L]! : primarySwatch[50L]!);
        hintColor ??= (isDark ? Colors.white60 : Colors.black.withOpacity(0.6));
        buttonTheme ??= new ButtonThemeData(colorScheme: colorScheme, buttonColor: (isDark ? primarySwatch[600L]! : Colors.grey[300L]!), disabledColor: disabledColor, focusColor: focusColor, hoverColor: hoverColor, highlightColor: highlightColor, splashColor: splashColor, materialTapTargetSize: DartRuntimePrimitives.RequireValue(materialTapTargetSize));
        disabledColor ??= (isDark ? Colors.white38 : Colors.black38);
        highlightColor ??= (isDark ? new global::Doroti.Ui.Color(1087163596L) : new global::Doroti.Ui.Color(1723645116L));
        splashColor ??= (isDark ? new global::Doroti.Ui.Color(1087163596L) : new global::Doroti.Ui.Color(1724434632L));
        typography ??= (DartRuntimePrimitives.RequireValue(useMaterial3) ? Typography.CreateMaterial2021(platform: DartRuntimePrimitives.RequireValue(platform), colorScheme: colorScheme) : Typography.CreateMaterial2014(platform: DartRuntimePrimitives.RequireValue(platform)));
        TextTheme defaultTextTheme = (isDark ? ((Typography)typography).white : ((Typography)typography).black);
        TextTheme defaultPrimaryTextTheme = (primaryIsDark ? ((Typography)typography).white : ((Typography)typography).black);
        if ((fontFamily is not null))
        {
            defaultTextTheme = defaultTextTheme.apply(fontFamily: fontFamily);
            defaultPrimaryTextTheme = defaultPrimaryTextTheme.apply(fontFamily: fontFamily);
        }
        if ((fontFamilyFallback is not null))
        {
            defaultTextTheme = defaultTextTheme.apply(fontFamilyFallback: fontFamilyFallback);
            defaultPrimaryTextTheme = defaultPrimaryTextTheme.apply(fontFamilyFallback: fontFamilyFallback);
        }
        if ((package is not null))
        {
            defaultTextTheme = defaultTextTheme.apply(package: package);
            defaultPrimaryTextTheme = defaultPrimaryTextTheme.apply(package: package);
        }
        textTheme = defaultTextTheme.merge(textTheme);
        primaryTextTheme = defaultPrimaryTextTheme.merge(primaryTextTheme);
        iconTheme ??= (isDark ? new global::Doroti.Framework.Widgets.IconThemeData(color: ConstantsLibrary.kDefaultIconLightColor) : new global::Doroti.Framework.Widgets.IconThemeData(color: ConstantsLibrary.kDefaultIconDarkColor));
        primaryIconTheme ??= (primaryIsDark ? new global::Doroti.Framework.Widgets.IconThemeData(color: Colors.white) : new global::Doroti.Framework.Widgets.IconThemeData(color: Colors.black));
        if ((appBarTheme is not null))
        {
            if ((appBarTheme is AppBarTheme))
            {
                appBarTheme = ((AppBarTheme)appBarTheme).data;
            }
            else
            {
                if ((appBarTheme is not AppBarThemeData))
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
        indicatorColor ??= ((object.Equals(((ColorScheme)colorScheme).secondary, primaryColor)) ? Colors.white : ((ColorScheme)colorScheme).secondary);
        var theme = new ThemeData(adaptationMap: ThemeData._createAdaptationMap(adaptations.Cast<Adaptation<object>>()), applyElevationOverlayColor: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(applyElevationOverlayColor)), cupertinoOverrideTheme: cupertinoOverrideTheme, extensions: ThemeData._themeExtensionIterableToMap(extensions), inputDecorationTheme: ((InputDecorationThemeData?)(object?)inputDecorationTheme)!, materialTapTargetSize: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(materialTapTargetSize)), pageTransitionsTheme: pageTransitionsTheme, platform: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(platform)), scrollbarTheme: scrollbarTheme, splashFactory: splashFactory, useMaterial3: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(useMaterial3)), visualDensity: visualDensity, canvasColor: canvasColor, cardColor: cardColor, colorScheme: colorScheme, disabledColor: disabledColor, dividerColor: dividerColor, focusColor: focusColor, highlightColor: highlightColor, hintColor: hintColor, hoverColor: hoverColor, primaryColor: primaryColor, primaryColorDark: primaryColorDark, primaryColorLight: primaryColorLight, scaffoldBackgroundColor: scaffoldBackgroundColor, secondaryHeaderColor: secondaryHeaderColor, shadowColor: shadowColor, splashColor: splashColor, unselectedWidgetColor: unselectedWidgetColor, iconTheme: iconTheme, primaryTextTheme: primaryTextTheme, textTheme: textTheme, typography: typography, primaryIconTheme: primaryIconTheme, actionIconTheme: actionIconTheme, appBarTheme: ((((AppBarThemeData?)(object?)appBarTheme)!) ?? new AppBarThemeData()), badgeTheme: badgeTheme, bannerTheme: bannerTheme, bottomAppBarTheme: bottomAppBarTheme, bottomNavigationBarTheme: bottomNavigationBarTheme, bottomSheetTheme: bottomSheetTheme, buttonTheme: buttonTheme, cardTheme: cardTheme, carouselViewTheme: carouselViewTheme, checkboxTheme: checkboxTheme, chipTheme: chipTheme, dataTableTheme: dataTableTheme, datePickerTheme: datePickerTheme, dialogTheme: dialogTheme, dividerTheme: dividerTheme, drawerTheme: drawerTheme, dropdownMenuTheme: dropdownMenuTheme, elevatedButtonTheme: elevatedButtonTheme, expansionTileTheme: expansionTileTheme, filledButtonTheme: filledButtonTheme, floatingActionButtonTheme: floatingActionButtonTheme, iconButtonTheme: iconButtonTheme, listTileTheme: listTileTheme, menuBarTheme: menuBarTheme, menuButtonTheme: menuButtonTheme, menuTheme: menuTheme, navigationBarTheme: navigationBarTheme, navigationDrawerTheme: navigationDrawerTheme, navigationRailTheme: navigationRailTheme, outlinedButtonTheme: outlinedButtonTheme, popupMenuTheme: popupMenuTheme, progressIndicatorTheme: progressIndicatorTheme, radioTheme: radioTheme, searchBarTheme: searchBarTheme, searchViewTheme: searchViewTheme, segmentedButtonTheme: segmentedButtonTheme, sliderTheme: sliderTheme, snackBarTheme: snackBarTheme, switchTheme: switchTheme, tabBarTheme: tabBarTheme, textButtonTheme: textButtonTheme, textSelectionTheme: textSelectionTheme, timePickerTheme: timePickerTheme, toggleButtonsTheme: toggleButtonsTheme, tooltipTheme: tooltipTheme, buttonBarTheme: buttonBarTheme, dialogBackgroundColor: dialogBackgroundColor, indicatorColor: indicatorColor);
        if (DartRuntimePrimitives.RequireValue(useSystemColors))
        {
            theme = theme._overrideWithSystemColors();
        }
        return theme;
    }

    public ThemeData(DartMap<Type, Adaptation<object>> adaptationMap, bool applyElevationOverlayColor, NoDefaultCupertinoThemeData? cupertinoOverrideTheme, DartMap<object, ThemeExtension<object>> extensions, InputDecorationThemeData inputDecorationTheme, MaterialTapTargetSize materialTapTargetSize, PageTransitionsTheme pageTransitionsTheme, global::Doroti.Framework.Foundation.TargetPlatform platform, ScrollbarThemeData scrollbarTheme, InteractiveInkFeatureFactory splashFactory, bool useMaterial3, VisualDensity visualDensity, ColorScheme colorScheme, Color canvasColor, Color cardColor, Color disabledColor, Color dividerColor, Color focusColor, Color highlightColor, Color hintColor, Color hoverColor, Color primaryColor, Color primaryColorDark, Color primaryColorLight, Color scaffoldBackgroundColor, Color secondaryHeaderColor, Color shadowColor, Color splashColor, Color unselectedWidgetColor, global::Doroti.Framework.Widgets.IconThemeData iconTheme, global::Doroti.Framework.Widgets.IconThemeData primaryIconTheme, TextTheme primaryTextTheme, TextTheme textTheme, Typography typography, ActionIconThemeData? actionIconTheme, AppBarThemeData appBarTheme, BadgeThemeData badgeTheme, MaterialBannerThemeData bannerTheme, BottomAppBarThemeData bottomAppBarTheme, BottomNavigationBarThemeData bottomNavigationBarTheme, BottomSheetThemeData bottomSheetTheme, ButtonThemeData buttonTheme, CardThemeData cardTheme, CarouselViewThemeData carouselViewTheme, CheckboxThemeData checkboxTheme, ChipThemeData chipTheme, DataTableThemeData dataTableTheme, DatePickerThemeData datePickerTheme, DialogThemeData dialogTheme, DividerThemeData dividerTheme, DrawerThemeData drawerTheme, DropdownMenuThemeData dropdownMenuTheme, ElevatedButtonThemeData elevatedButtonTheme, ExpansionTileThemeData expansionTileTheme, FilledButtonThemeData filledButtonTheme, FloatingActionButtonThemeData floatingActionButtonTheme, IconButtonThemeData iconButtonTheme, ListTileThemeData listTileTheme, MenuBarThemeData menuBarTheme, MenuButtonThemeData menuButtonTheme, MenuThemeData menuTheme, NavigationBarThemeData navigationBarTheme, NavigationDrawerThemeData navigationDrawerTheme, NavigationRailThemeData navigationRailTheme, OutlinedButtonThemeData outlinedButtonTheme, PopupMenuThemeData popupMenuTheme, ProgressIndicatorThemeData progressIndicatorTheme, RadioThemeData radioTheme, SearchBarThemeData searchBarTheme, SearchViewThemeData searchViewTheme, SegmentedButtonThemeData segmentedButtonTheme, SliderThemeData sliderTheme, SnackBarThemeData snackBarTheme, SwitchThemeData switchTheme, TabBarThemeData tabBarTheme, TextButtonThemeData textButtonTheme, TextSelectionThemeData textSelectionTheme, TimePickerThemeData timePickerTheme, ToggleButtonsThemeData toggleButtonsTheme, TooltipThemeData tooltipTheme, ButtonBarThemeData? buttonBarTheme = null, Color dialogBackgroundColor = default!, Color indicatorColor = default!)
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
        this.useMaterial3 = useMaterial3;
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
        this._buttonBarTheme = buttonBarTheme;
        System.Diagnostics.Debug.Assert((buttonBarTheme is not null));
    }

    public static ThemeData CreateFrom(ColorScheme colorScheme, TextTheme? textTheme = null, bool? useMaterial3 = null)
    {
        var isDark = (object.Equals(((ColorScheme)colorScheme).brightness, Brightness.dark));
        global::Doroti.Ui.Color primarySurfaceColor = ((global::Doroti.Ui.Color)(object?)(isDark ? ((ColorScheme)colorScheme).surface : ((ColorScheme)colorScheme).primary));
        global::Doroti.Ui.Color onPrimarySurfaceColor = ((global::Doroti.Ui.Color)(object?)(isDark ? ((ColorScheme)colorScheme).onSurface : ((ColorScheme)colorScheme).onPrimary));
        return ThemeData.Create(colorScheme: colorScheme, brightness: ((ColorScheme)colorScheme).brightness, primaryColor: primarySurfaceColor, canvasColor: ((ColorScheme)colorScheme).surface, scaffoldBackgroundColor: ((ColorScheme)colorScheme).surface, cardColor: ((ColorScheme)colorScheme).surface, dividerColor: ((ColorScheme)colorScheme).onSurface.withOpacity(0.12), dialogBackgroundColor: ((ColorScheme)colorScheme).surface, indicatorColor: onPrimarySurfaceColor, textTheme: textTheme, applyElevationOverlayColor: isDark, useMaterial3: useMaterial3);
    }

    public static ThemeData CreateLight(bool? useMaterial3 = null) => ThemeData.Create(brightness: Brightness.light, useMaterial3: useMaterial3);

    public static ThemeData CreateDark(bool? useMaterial3 = null) => ThemeData.Create(brightness: Brightness.dark, useMaterial3: useMaterial3);

    public static ThemeData CreateFallback(bool? useMaterial3 = null) => ThemeData.CreateLight(useMaterial3: useMaterial3);

    public virtual Adaptation<T>? getAdaptation<T>() => ((Adaptation<T>?)(object?)this.adaptationMap.GetValueOrDefault(typeof(T)))!;
    internal static DartMap<Type, Adaptation<object>> _createAdaptationMap(IEnumerable<Adaptation<object>> adaptations)
    {
        var adaptationMap = new DartMap<Type, Adaptation<object>>();
        return adaptationMap;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.Brightness brightness => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Brightness>(((ColorScheme)this.colorScheme).brightness);
    public virtual T? extension<T>() => ((T?)(object?)this.extensions.GetValueOrDefault(typeof(T)))!;
    public virtual ButtonBarThemeData buttonBarTheme => DartRuntimePrimitives.ConvertValue<ButtonBarThemeData>(this._buttonBarTheme!);
    public virtual ThemeData copyWith(IEnumerable<Adaptation<object>>? adaptations = null, bool? applyElevationOverlayColor = null, NoDefaultCupertinoThemeData? cupertinoOverrideTheme = null, IEnumerable<ThemeExtension<object>>? extensions = null, object? inputDecorationTheme = null, MaterialTapTargetSize? materialTapTargetSize = null, PageTransitionsTheme? pageTransitionsTheme = null, global::Doroti.Framework.Foundation.TargetPlatform? platform = null, ScrollbarThemeData? scrollbarTheme = null, InteractiveInkFeatureFactory? splashFactory = null, VisualDensity? visualDensity = null, ColorScheme? colorScheme = null, Brightness? brightness = null, Color? canvasColor = null, Color? cardColor = null, Color? disabledColor = null, Color? dividerColor = null, Color? focusColor = null, Color? highlightColor = null, Color? hintColor = null, Color? hoverColor = null, Color? primaryColor = null, Color? primaryColorDark = null, Color? primaryColorLight = null, Color? scaffoldBackgroundColor = null, Color? secondaryHeaderColor = null, Color? shadowColor = null, Color? splashColor = null, Color? unselectedWidgetColor = null, global::Doroti.Framework.Widgets.IconThemeData? iconTheme = null, global::Doroti.Framework.Widgets.IconThemeData? primaryIconTheme = null, TextTheme? primaryTextTheme = null, TextTheme? textTheme = null, Typography? typography = null, ActionIconThemeData? actionIconTheme = null, object? appBarTheme = null, BadgeThemeData? badgeTheme = null, MaterialBannerThemeData? bannerTheme = null, BottomAppBarThemeData? bottomAppBarTheme = null, BottomNavigationBarThemeData? bottomNavigationBarTheme = null, BottomSheetThemeData? bottomSheetTheme = null, ButtonThemeData? buttonTheme = null, CardThemeData? cardTheme = null, CarouselViewThemeData? carouselViewTheme = null, CheckboxThemeData? checkboxTheme = null, ChipThemeData? chipTheme = null, DataTableThemeData? dataTableTheme = null, DatePickerThemeData? datePickerTheme = null, DialogThemeData? dialogTheme = null, DividerThemeData? dividerTheme = null, DrawerThemeData? drawerTheme = null, DropdownMenuThemeData? dropdownMenuTheme = null, ElevatedButtonThemeData? elevatedButtonTheme = null, ExpansionTileThemeData? expansionTileTheme = null, FilledButtonThemeData? filledButtonTheme = null, FloatingActionButtonThemeData? floatingActionButtonTheme = null, IconButtonThemeData? iconButtonTheme = null, ListTileThemeData? listTileTheme = null, MenuBarThemeData? menuBarTheme = null, MenuButtonThemeData? menuButtonTheme = null, MenuThemeData? menuTheme = null, NavigationBarThemeData? navigationBarTheme = null, NavigationDrawerThemeData? navigationDrawerTheme = null, NavigationRailThemeData? navigationRailTheme = null, OutlinedButtonThemeData? outlinedButtonTheme = null, PopupMenuThemeData? popupMenuTheme = null, ProgressIndicatorThemeData? progressIndicatorTheme = null, RadioThemeData? radioTheme = null, SearchBarThemeData? searchBarTheme = null, SearchViewThemeData? searchViewTheme = null, SegmentedButtonThemeData? segmentedButtonTheme = null, SliderThemeData? sliderTheme = null, SnackBarThemeData? snackBarTheme = null, SwitchThemeData? switchTheme = null, TabBarThemeData? tabBarTheme = null, TextButtonThemeData? textButtonTheme = null, TextSelectionThemeData? textSelectionTheme = null, TimePickerThemeData? timePickerTheme = null, ToggleButtonsThemeData? toggleButtonsTheme = null, TooltipThemeData? tooltipTheme = null, bool? useMaterial3 = null, ButtonBarThemeData? buttonBarTheme = null, Color? dialogBackgroundColor = null, Color? indicatorColor = null)
    {
        cupertinoOverrideTheme = cupertinoOverrideTheme?.noDefault();
        if ((inputDecorationTheme is not null))
        {
            if ((inputDecorationTheme is InputDecorationTheme))
            {
                inputDecorationTheme = ((InputDecorationTheme)inputDecorationTheme).data;
            }
            else
            {
                if ((inputDecorationTheme is not InputDecorationThemeData))
                {
                    throw DartRuntimePrimitives.AsException(new DartArgumentError("inputDecorationTheme must be either a InputDecorationThemeData or a InputDecorationTheme"));
                }
            }
        }
        return new ThemeData(adaptationMap: ((adaptations is not null) ? ThemeData._createAdaptationMap(adaptations.Cast<Adaptation<object>>()) : this.adaptationMap), applyElevationOverlayColor: (applyElevationOverlayColor ?? this.applyElevationOverlayColor), cupertinoOverrideTheme: (cupertinoOverrideTheme ?? this.cupertinoOverrideTheme), extensions: (((extensions is not null)) ? ThemeData._themeExtensionIterableToMap(extensions) : this.extensions), inputDecorationTheme: (((InputDecorationThemeData?)(object?)inputDecorationTheme)! ?? this.inputDecorationTheme), materialTapTargetSize: (materialTapTargetSize ?? this.materialTapTargetSize), pageTransitionsTheme: (pageTransitionsTheme ?? this.pageTransitionsTheme), platform: (platform ?? this.platform), scrollbarTheme: (scrollbarTheme ?? this.scrollbarTheme), splashFactory: (splashFactory ?? this.splashFactory), useMaterial3: (useMaterial3 ?? this.useMaterial3), visualDensity: (visualDensity ?? this.visualDensity), canvasColor: (canvasColor ?? this.canvasColor), cardColor: (cardColor ?? this.cardColor), colorScheme: ((colorScheme ?? this.colorScheme)).copyWith(brightness: brightness), disabledColor: (disabledColor ?? this.disabledColor), dividerColor: (dividerColor ?? this.dividerColor), focusColor: (focusColor ?? this.focusColor), highlightColor: (highlightColor ?? this.highlightColor), hintColor: (hintColor ?? this.hintColor), hoverColor: (hoverColor ?? this.hoverColor), primaryColor: (primaryColor ?? this.primaryColor), primaryColorDark: (primaryColorDark ?? this.primaryColorDark), primaryColorLight: (primaryColorLight ?? this.primaryColorLight), scaffoldBackgroundColor: (scaffoldBackgroundColor ?? this.scaffoldBackgroundColor), secondaryHeaderColor: (secondaryHeaderColor ?? this.secondaryHeaderColor), shadowColor: (shadowColor ?? this.shadowColor), splashColor: (splashColor ?? this.splashColor), unselectedWidgetColor: (unselectedWidgetColor ?? this.unselectedWidgetColor), iconTheme: (iconTheme ?? this.iconTheme), primaryIconTheme: (primaryIconTheme ?? this.primaryIconTheme), primaryTextTheme: (primaryTextTheme ?? this.primaryTextTheme), textTheme: (textTheme ?? this.textTheme), typography: (typography ?? this.typography), actionIconTheme: (actionIconTheme ?? this.actionIconTheme), appBarTheme: ((global::System.Func<AppBarThemeData>)(() =>
        {
            if ((appBarTheme is not null))
            {
                if ((appBarTheme is AppBarTheme))
                {
                    AppBarTheme appBarTheme__as70567 = (AppBarTheme)appBarTheme;
                    return ((AppBarTheme)appBarTheme__as70567).data;
                }
                else
                {
                    if ((appBarTheme is not AppBarThemeData))
                    {
                        throw DartRuntimePrimitives.AsException(new DartArgumentError("appBarTheme must be either a AppBarThemeData or a AppBarTheme"));
                    }
                }
            }
            return (((AppBarThemeData?)(object?)appBarTheme)! ?? this.appBarTheme);
            throw new InvalidOperationException("Dart closure completed without a value.");
        }))(), badgeTheme: (badgeTheme ?? this.badgeTheme), bannerTheme: (bannerTheme ?? this.bannerTheme), bottomAppBarTheme: (bottomAppBarTheme ?? this.bottomAppBarTheme), bottomNavigationBarTheme: (bottomNavigationBarTheme ?? this.bottomNavigationBarTheme), bottomSheetTheme: (bottomSheetTheme ?? this.bottomSheetTheme), buttonTheme: (buttonTheme ?? this.buttonTheme), cardTheme: (cardTheme ?? this.cardTheme), carouselViewTheme: (carouselViewTheme ?? this.carouselViewTheme), checkboxTheme: (checkboxTheme ?? this.checkboxTheme), chipTheme: (chipTheme ?? this.chipTheme), dataTableTheme: (dataTableTheme ?? this.dataTableTheme), datePickerTheme: (datePickerTheme ?? this.datePickerTheme), dialogTheme: (dialogTheme ?? this.dialogTheme), dividerTheme: (dividerTheme ?? this.dividerTheme), drawerTheme: (drawerTheme ?? this.drawerTheme), dropdownMenuTheme: (dropdownMenuTheme ?? this.dropdownMenuTheme), elevatedButtonTheme: (elevatedButtonTheme ?? this.elevatedButtonTheme), expansionTileTheme: (expansionTileTheme ?? this.expansionTileTheme), filledButtonTheme: (filledButtonTheme ?? this.filledButtonTheme), floatingActionButtonTheme: (floatingActionButtonTheme ?? this.floatingActionButtonTheme), iconButtonTheme: (iconButtonTheme ?? this.iconButtonTheme), listTileTheme: (listTileTheme ?? this.listTileTheme), menuBarTheme: (menuBarTheme ?? this.menuBarTheme), menuButtonTheme: (menuButtonTheme ?? this.menuButtonTheme), menuTheme: (menuTheme ?? this.menuTheme), navigationBarTheme: (navigationBarTheme ?? this.navigationBarTheme), navigationDrawerTheme: (navigationDrawerTheme ?? this.navigationDrawerTheme), navigationRailTheme: (navigationRailTheme ?? this.navigationRailTheme), outlinedButtonTheme: (outlinedButtonTheme ?? this.outlinedButtonTheme), popupMenuTheme: (popupMenuTheme ?? this.popupMenuTheme), progressIndicatorTheme: (progressIndicatorTheme ?? this.progressIndicatorTheme), radioTheme: (radioTheme ?? this.radioTheme), searchBarTheme: (searchBarTheme ?? this.searchBarTheme), searchViewTheme: (searchViewTheme ?? this.searchViewTheme), segmentedButtonTheme: (segmentedButtonTheme ?? this.segmentedButtonTheme), sliderTheme: (sliderTheme ?? this.sliderTheme), snackBarTheme: (snackBarTheme ?? this.snackBarTheme), switchTheme: (switchTheme ?? this.switchTheme), tabBarTheme: (tabBarTheme ?? this.tabBarTheme), textButtonTheme: (textButtonTheme ?? this.textButtonTheme), textSelectionTheme: (textSelectionTheme ?? this.textSelectionTheme), timePickerTheme: (timePickerTheme ?? this.timePickerTheme), toggleButtonsTheme: (toggleButtonsTheme ?? this.toggleButtonsTheme), tooltipTheme: (tooltipTheme ?? this.tooltipTheme), buttonBarTheme: (buttonBarTheme ?? this._buttonBarTheme), dialogBackgroundColor: (dialogBackgroundColor ?? this.dialogBackgroundColor), indicatorColor: (indicatorColor ?? this.indicatorColor));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static ThemeData localize(ThemeData baseTheme, TextTheme localTextGeometry)
    {
        return ((ThemeData)(object?)_localizedThemeDataCache.putIfAbsent(new _IdentityThemeDataCacheKey__theme_data(baseTheme, localTextGeometry), (() =>
        {
            return baseTheme.copyWith(primaryTextTheme: localTextGeometry.merge(((ThemeData)baseTheme).primaryTextTheme), textTheme: localTextGeometry.merge(((ThemeData)baseTheme).textTheme));
            throw new InvalidOperationException("Dart closure completed without a value.");
        })));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static global::Doroti.Ui.Brightness estimateBrightnessForColor(Color color)
    {
        double relativeLuminance = color.computeLuminance();
        var kThreshold = 0.15;
        if (((((relativeLuminance + 0.05)) * ((relativeLuminance + 0.05))) > kThreshold))
        {
            return Brightness.light;
        }
        return Brightness.dark;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static DartMap<object, ThemeExtension<object>> _lerpThemeExtensions(ThemeData a, ThemeData b, double t)
    {
        DartMap<object, ThemeExtension<object>> newExtensions = ((ThemeData)a).extensions.map<object, ThemeExtension<object>, object, ThemeExtension<object>>(((id, extensionA) =>
        {
            ThemeExtension<object>? extensionB = ((ThemeData)b).extensions.GetValueOrDefault(id);
            return new MapEntry<object, ThemeExtension<object>>(id, extensionA.lerp(extensionB, t));
            throw new InvalidOperationException("Dart closure completed without a value.");
        }));
        newExtensions.addEntries(((ThemeData)b).extensions.entries.where(((entry) => !((ThemeData)a).extensions.ContainsKey(entry.key))).Cast<MapEntry<object, ThemeExtension<object>>>());
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
        global::Doroti.Ui.SystemColorPalette systemColors = ((global::Doroti.Ui.SystemColorPalette)(object?)((object.Equals(this.brightness, Brightness.dark)) ? SystemColor.dark : SystemColor.light));
        var theme = this;
        theme = theme.copyWith(colorScheme: this.colorScheme.copyWith(secondary: systemColors.accentColor.value, onSecondary: systemColors.accentColorText.value, surface: systemColors.canvas.value, onSurface: systemColors.canvasText.value), textTheme: this.textTheme.apply(displayColor: systemColors.canvasText.value, bodyColor: systemColors.canvasText.value));
        bool overrideButtons = (((systemColors.buttonFace.value is not null) || (systemColors.buttonBorder.value is not null)) || (systemColors.buttonText.value is not null));
        if (overrideButtons)
        {
            theme = theme.copyWith(elevatedButtonTheme: new ElevatedButtonThemeData(style: ElevatedButton.styleFrom(foregroundColor: systemColors.buttonText.value, backgroundColor: systemColors.buttonFace.value, side: ((systemColors.buttonBorder.value is null) ? null : new global::Doroti.Framework.Painting.BorderSide(color: systemColors.buttonBorder.value!)))), textButtonTheme: new TextButtonThemeData(style: TextButton.styleFrom(foregroundColor: systemColors.buttonText.value, backgroundColor: systemColors.buttonFace.value, side: ((systemColors.buttonBorder.value is null) ? null : new global::Doroti.Framework.Painting.BorderSide(color: systemColors.buttonBorder.value!)))), outlinedButtonTheme: new OutlinedButtonThemeData(style: OutlinedButton.styleFrom(foregroundColor: systemColors.buttonText.value, backgroundColor: systemColors.buttonFace.value, side: ((systemColors.buttonBorder.value is null) ? null : new global::Doroti.Framework.Painting.BorderSide(color: systemColors.buttonBorder.value!)))), filledButtonTheme: new FilledButtonThemeData(style: FilledButton.styleFrom(foregroundColor: systemColors.buttonText.value, backgroundColor: systemColors.buttonFace.value, side: ((systemColors.buttonBorder.value is null) ? null : new global::Doroti.Framework.Painting.BorderSide(color: systemColors.buttonBorder.value!)))), floatingActionButtonTheme: new FloatingActionButtonThemeData(backgroundColor: systemColors.buttonFace.value, foregroundColor: systemColors.buttonText.value));
        }
        bool overrideInputDecoration = ((systemColors.field.value is not null) || (systemColors.fieldText.value is not null));
        if (overrideInputDecoration)
        {
            theme = theme.copyWith(inputDecorationTheme: this.inputDecorationTheme.copyWith(fillColor: systemColors.field.value, labelStyle: (this.inputDecorationTheme.labelStyle?.copyWith(color: systemColors.fieldText.value) ?? new global::Doroti.Framework.Painting.TextStyle(color: systemColors.fieldText.value)), hintStyle: (this.inputDecorationTheme.hintStyle?.copyWith(color: systemColors.fieldText.value) ?? new global::Doroti.Framework.Painting.TextStyle(color: systemColors.fieldText.value)), helperStyle: (this.inputDecorationTheme.helperStyle?.copyWith(color: systemColors.fieldText.value) ?? new global::Doroti.Framework.Painting.TextStyle(color: systemColors.fieldText.value)), prefixStyle: (this.inputDecorationTheme.prefixStyle?.copyWith(color: systemColors.fieldText.value) ?? new global::Doroti.Framework.Painting.TextStyle(color: systemColors.fieldText.value)), suffixStyle: (this.inputDecorationTheme.suffixStyle?.copyWith(color: systemColors.fieldText.value) ?? new global::Doroti.Framework.Painting.TextStyle(color: systemColors.fieldText.value)), counterStyle: (this.inputDecorationTheme.counterStyle?.copyWith(color: systemColors.fieldText.value) ?? new global::Doroti.Framework.Painting.TextStyle(color: systemColors.fieldText.value))));
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
        return new ThemeData(adaptationMap: ((t < 0.5) ? ((ThemeData)a).adaptationMap : ((ThemeData)b).adaptationMap), applyElevationOverlayColor: ((t < 0.5) ? ((ThemeData)a).applyElevationOverlayColor : ((ThemeData)b).applyElevationOverlayColor), cupertinoOverrideTheme: ((t < 0.5) ? ((ThemeData)a).cupertinoOverrideTheme : ((ThemeData)b).cupertinoOverrideTheme), extensions: ThemeData._lerpThemeExtensions(a, b, t), inputDecorationTheme: ((t < 0.5) ? ((ThemeData)a).inputDecorationTheme : ((ThemeData)b).inputDecorationTheme), materialTapTargetSize: ((t < 0.5) ? ((ThemeData)a).materialTapTargetSize : ((ThemeData)b).materialTapTargetSize), pageTransitionsTheme: ((t < 0.5) ? ((ThemeData)a).pageTransitionsTheme : ((ThemeData)b).pageTransitionsTheme), platform: ((t < 0.5) ? ((ThemeData)a).platform : ((ThemeData)b).platform), scrollbarTheme: ScrollbarThemeData.lerp(((ThemeData)a).scrollbarTheme, ((ThemeData)b).scrollbarTheme, t), splashFactory: ((t < 0.5) ? ((ThemeData)a).splashFactory : ((ThemeData)b).splashFactory), useMaterial3: ((t < 0.5) ? ((ThemeData)a).useMaterial3 : ((ThemeData)b).useMaterial3), visualDensity: VisualDensity.lerp(((ThemeData)a).visualDensity, ((ThemeData)b).visualDensity, t), canvasColor: Dart_uiLibrary.Color.lerp(((ThemeData)a).canvasColor, ((ThemeData)b).canvasColor, t)!, cardColor: Dart_uiLibrary.Color.lerp(((ThemeData)a).cardColor, ((ThemeData)b).cardColor, t)!, colorScheme: ColorScheme.lerp(((ThemeData)a).colorScheme, ((ThemeData)b).colorScheme, t), disabledColor: Dart_uiLibrary.Color.lerp(((ThemeData)a).disabledColor, ((ThemeData)b).disabledColor, t)!, dividerColor: Dart_uiLibrary.Color.lerp(((ThemeData)a).dividerColor, ((ThemeData)b).dividerColor, t)!, focusColor: Dart_uiLibrary.Color.lerp(((ThemeData)a).focusColor, ((ThemeData)b).focusColor, t)!, highlightColor: Dart_uiLibrary.Color.lerp(((ThemeData)a).highlightColor, ((ThemeData)b).highlightColor, t)!, hintColor: Dart_uiLibrary.Color.lerp(((ThemeData)a).hintColor, ((ThemeData)b).hintColor, t)!, hoverColor: Dart_uiLibrary.Color.lerp(((ThemeData)a).hoverColor, ((ThemeData)b).hoverColor, t)!, primaryColor: Dart_uiLibrary.Color.lerp(((ThemeData)a).primaryColor, ((ThemeData)b).primaryColor, t)!, primaryColorDark: Dart_uiLibrary.Color.lerp(((ThemeData)a).primaryColorDark, ((ThemeData)b).primaryColorDark, t)!, primaryColorLight: Dart_uiLibrary.Color.lerp(((ThemeData)a).primaryColorLight, ((ThemeData)b).primaryColorLight, t)!, scaffoldBackgroundColor: Dart_uiLibrary.Color.lerp(((ThemeData)a).scaffoldBackgroundColor, ((ThemeData)b).scaffoldBackgroundColor, t)!, secondaryHeaderColor: Dart_uiLibrary.Color.lerp(((ThemeData)a).secondaryHeaderColor, ((ThemeData)b).secondaryHeaderColor, t)!, shadowColor: Dart_uiLibrary.Color.lerp(((ThemeData)a).shadowColor, ((ThemeData)b).shadowColor, t)!, splashColor: Dart_uiLibrary.Color.lerp(((ThemeData)a).splashColor, ((ThemeData)b).splashColor, t)!, unselectedWidgetColor: Dart_uiLibrary.Color.lerp(((ThemeData)a).unselectedWidgetColor, ((ThemeData)b).unselectedWidgetColor, t)!, iconTheme: IconThemeData.lerp(((ThemeData)a).iconTheme, ((ThemeData)b).iconTheme, t), primaryIconTheme: IconThemeData.lerp(((ThemeData)a).primaryIconTheme, ((ThemeData)b).primaryIconTheme, t), primaryTextTheme: TextTheme.lerp(((ThemeData)a).primaryTextTheme, ((ThemeData)b).primaryTextTheme, t), textTheme: TextTheme.lerp(((ThemeData)a).textTheme, ((ThemeData)b).textTheme, t), typography: Typography.lerp(((ThemeData)a).typography, ((ThemeData)b).typography, t), actionIconTheme: ActionIconThemeData.lerp(((ThemeData)a).actionIconTheme, ((ThemeData)b).actionIconTheme, t), appBarTheme: AppBarThemeData.lerp(((ThemeData)a).appBarTheme, ((ThemeData)b).appBarTheme, t), badgeTheme: BadgeThemeData.lerp(((ThemeData)a).badgeTheme, ((ThemeData)b).badgeTheme, t), bannerTheme: MaterialBannerThemeData.lerp(((ThemeData)a).bannerTheme, ((ThemeData)b).bannerTheme, t), bottomAppBarTheme: BottomAppBarThemeData.lerp(((ThemeData)a).bottomAppBarTheme, ((ThemeData)b).bottomAppBarTheme, t), bottomNavigationBarTheme: BottomNavigationBarThemeData.lerp(((ThemeData)a).bottomNavigationBarTheme, ((ThemeData)b).bottomNavigationBarTheme, t), bottomSheetTheme: BottomSheetThemeData.lerp(((ThemeData)a).bottomSheetTheme, ((ThemeData)b).bottomSheetTheme, t)!, buttonTheme: ((t < 0.5) ? ((ThemeData)a).buttonTheme : ((ThemeData)b).buttonTheme), cardTheme: CardThemeData.lerp(((ThemeData)a).cardTheme, ((ThemeData)b).cardTheme, t), carouselViewTheme: CarouselViewThemeData.lerp(((ThemeData)a).carouselViewTheme, ((ThemeData)b).carouselViewTheme, t), checkboxTheme: CheckboxThemeData.lerp(((ThemeData)a).checkboxTheme, ((ThemeData)b).checkboxTheme, t), chipTheme: ChipThemeData.lerp(((ThemeData)a).chipTheme, ((ThemeData)b).chipTheme, t)!, dataTableTheme: DataTableThemeData.lerp(((ThemeData)a).dataTableTheme, ((ThemeData)b).dataTableTheme, t), datePickerTheme: DatePickerThemeData.lerp(((ThemeData)a).datePickerTheme, ((ThemeData)b).datePickerTheme, t), dialogTheme: DialogThemeData.lerp(((ThemeData)a).dialogTheme, ((ThemeData)b).dialogTheme, t), dividerTheme: DividerThemeData.lerp(((ThemeData)a).dividerTheme, ((ThemeData)b).dividerTheme, t), drawerTheme: DrawerThemeData.lerp(((ThemeData)a).drawerTheme, ((ThemeData)b).drawerTheme, t)!, dropdownMenuTheme: DropdownMenuThemeData.lerp(((ThemeData)a).dropdownMenuTheme, ((ThemeData)b).dropdownMenuTheme, t), elevatedButtonTheme: ElevatedButtonThemeData.lerp(((ThemeData)a).elevatedButtonTheme, ((ThemeData)b).elevatedButtonTheme, t)!, expansionTileTheme: ExpansionTileThemeData.lerp(((ThemeData)a).expansionTileTheme, ((ThemeData)b).expansionTileTheme, t)!, filledButtonTheme: FilledButtonThemeData.lerp(((ThemeData)a).filledButtonTheme, ((ThemeData)b).filledButtonTheme, t)!, floatingActionButtonTheme: FloatingActionButtonThemeData.lerp(((ThemeData)a).floatingActionButtonTheme, ((ThemeData)b).floatingActionButtonTheme, t)!, iconButtonTheme: IconButtonThemeData.lerp(((ThemeData)a).iconButtonTheme, ((ThemeData)b).iconButtonTheme, t)!, listTileTheme: ListTileThemeData.lerp(((ThemeData)a).listTileTheme, ((ThemeData)b).listTileTheme, t)!, menuBarTheme: MenuBarThemeData.lerp(((ThemeData)a).menuBarTheme, ((ThemeData)b).menuBarTheme, t)!, menuButtonTheme: MenuButtonThemeData.lerp(((ThemeData)a).menuButtonTheme, ((ThemeData)b).menuButtonTheme, t)!, menuTheme: MenuThemeData.lerp(((ThemeData)a).menuTheme, ((ThemeData)b).menuTheme, t)!, navigationBarTheme: NavigationBarThemeData.lerp(((ThemeData)a).navigationBarTheme, ((ThemeData)b).navigationBarTheme, t)!, navigationDrawerTheme: NavigationDrawerThemeData.lerp(((ThemeData)a).navigationDrawerTheme, ((ThemeData)b).navigationDrawerTheme, t)!, navigationRailTheme: NavigationRailThemeData.lerp(((ThemeData)a).navigationRailTheme, ((ThemeData)b).navigationRailTheme, t)!, outlinedButtonTheme: OutlinedButtonThemeData.lerp(((ThemeData)a).outlinedButtonTheme, ((ThemeData)b).outlinedButtonTheme, t)!, popupMenuTheme: PopupMenuThemeData.lerp(((ThemeData)a).popupMenuTheme, ((ThemeData)b).popupMenuTheme, t)!, progressIndicatorTheme: ProgressIndicatorThemeData.lerp(((ThemeData)a).progressIndicatorTheme, ((ThemeData)b).progressIndicatorTheme, t)!, radioTheme: RadioThemeData.lerp(((ThemeData)a).radioTheme, ((ThemeData)b).radioTheme, t), searchBarTheme: SearchBarThemeData.lerp(((ThemeData)a).searchBarTheme, ((ThemeData)b).searchBarTheme, t)!, searchViewTheme: SearchViewThemeData.lerp(((ThemeData)a).searchViewTheme, ((ThemeData)b).searchViewTheme, t)!, segmentedButtonTheme: SegmentedButtonThemeData.lerp(((ThemeData)a).segmentedButtonTheme, ((ThemeData)b).segmentedButtonTheme, t), sliderTheme: SliderThemeData.lerp(((ThemeData)a).sliderTheme, ((ThemeData)b).sliderTheme, t), snackBarTheme: SnackBarThemeData.lerp(((ThemeData)a).snackBarTheme, ((ThemeData)b).snackBarTheme, t), switchTheme: SwitchThemeData.lerp(((ThemeData)a).switchTheme, ((ThemeData)b).switchTheme, t), tabBarTheme: TabBarThemeData.lerp(((ThemeData)a).tabBarTheme, ((ThemeData)b).tabBarTheme, t), textButtonTheme: TextButtonThemeData.lerp(((ThemeData)a).textButtonTheme, ((ThemeData)b).textButtonTheme, t)!, textSelectionTheme: TextSelectionThemeData.lerp(((ThemeData)a).textSelectionTheme, ((ThemeData)b).textSelectionTheme, t)!, timePickerTheme: TimePickerThemeData.lerp(((ThemeData)a).timePickerTheme, ((ThemeData)b).timePickerTheme, t), toggleButtonsTheme: ToggleButtonsThemeData.lerp(((ThemeData)a).toggleButtonsTheme, ((ThemeData)b).toggleButtonsTheme, t)!, tooltipTheme: TooltipThemeData.lerp(((ThemeData)a).tooltipTheme, ((ThemeData)b).tooltipTheme, t)!, buttonBarTheme: ButtonBarThemeData.lerp(((ThemeData)a).buttonBarTheme, ((ThemeData)b).buttonBarTheme, t), dialogBackgroundColor: Dart_uiLibrary.Color.lerp(((ThemeData)a).dialogBackgroundColor, ((ThemeData)b).dialogBackgroundColor, t)!, indicatorColor: Dart_uiLibrary.Color.lerp(((ThemeData)a).indicatorColor, ((ThemeData)b).indicatorColor, t)!);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as ThemeData;
        if (__other is null) return false;
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((__other is ThemeData) && global::Doroti.Framework.Foundation.CollectionsLibrary.mapEquals(((ThemeData)((ThemeData)__other)).adaptationMap, this.adaptationMap)) && (((ThemeData)((ThemeData)__other)).applyElevationOverlayColor == this.applyElevationOverlayColor)) && (object.Equals(((ThemeData)((ThemeData)__other)).cupertinoOverrideTheme, this.cupertinoOverrideTheme))) && global::Doroti.Framework.Foundation.CollectionsLibrary.mapEquals(((ThemeData)((ThemeData)__other)).extensions, this.extensions)) && (object.Equals(((ThemeData)((ThemeData)__other)).inputDecorationTheme, this.inputDecorationTheme))) && (object.Equals(((ThemeData)((ThemeData)__other)).materialTapTargetSize, this.materialTapTargetSize))) && (object.Equals(((ThemeData)((ThemeData)__other)).pageTransitionsTheme, this.pageTransitionsTheme))) && (object.Equals(((ThemeData)((ThemeData)__other)).platform, this.platform))) && (object.Equals(((ThemeData)((ThemeData)__other)).scrollbarTheme, this.scrollbarTheme))) && (object.Equals(((ThemeData)((ThemeData)__other)).splashFactory, this.splashFactory))) && (((ThemeData)((ThemeData)__other)).useMaterial3 == this.useMaterial3)) && (object.Equals(((ThemeData)((ThemeData)__other)).visualDensity, this.visualDensity))) && (object.Equals(((ThemeData)((ThemeData)__other)).canvasColor, this.canvasColor))) && (object.Equals(((ThemeData)((ThemeData)__other)).cardColor, this.cardColor))) && (object.Equals(((ThemeData)((ThemeData)__other)).colorScheme, this.colorScheme))) && (object.Equals(((ThemeData)((ThemeData)__other)).disabledColor, this.disabledColor))) && (object.Equals(((ThemeData)((ThemeData)__other)).dividerColor, this.dividerColor))) && (object.Equals(((ThemeData)((ThemeData)__other)).focusColor, this.focusColor))) && (object.Equals(((ThemeData)((ThemeData)__other)).highlightColor, this.highlightColor))) && (object.Equals(((ThemeData)((ThemeData)__other)).hintColor, this.hintColor))) && (object.Equals(((ThemeData)((ThemeData)__other)).hoverColor, this.hoverColor))) && (object.Equals(((ThemeData)((ThemeData)__other)).primaryColor, this.primaryColor))) && (object.Equals(((ThemeData)((ThemeData)__other)).primaryColorDark, this.primaryColorDark))) && (object.Equals(((ThemeData)((ThemeData)__other)).primaryColorLight, this.primaryColorLight))) && (object.Equals(((ThemeData)((ThemeData)__other)).scaffoldBackgroundColor, this.scaffoldBackgroundColor))) && (object.Equals(((ThemeData)((ThemeData)__other)).secondaryHeaderColor, this.secondaryHeaderColor))) && (object.Equals(((ThemeData)((ThemeData)__other)).shadowColor, this.shadowColor))) && (object.Equals(((ThemeData)((ThemeData)__other)).splashColor, this.splashColor))) && (object.Equals(((ThemeData)((ThemeData)__other)).unselectedWidgetColor, this.unselectedWidgetColor))) && (object.Equals(((ThemeData)((ThemeData)__other)).iconTheme, this.iconTheme))) && (object.Equals(((ThemeData)((ThemeData)__other)).primaryIconTheme, this.primaryIconTheme))) && (object.Equals(((ThemeData)((ThemeData)__other)).primaryTextTheme, this.primaryTextTheme))) && (object.Equals(((ThemeData)((ThemeData)__other)).textTheme, this.textTheme))) && (object.Equals(((ThemeData)((ThemeData)__other)).typography, this.typography))) && (object.Equals(((ThemeData)((ThemeData)__other)).actionIconTheme, this.actionIconTheme))) && (object.Equals(((ThemeData)((ThemeData)__other)).appBarTheme, this.appBarTheme))) && (object.Equals(((ThemeData)((ThemeData)__other)).badgeTheme, this.badgeTheme))) && (object.Equals(((ThemeData)((ThemeData)__other)).bannerTheme, this.bannerTheme))) && (object.Equals(((ThemeData)((ThemeData)__other)).bottomAppBarTheme, this.bottomAppBarTheme))) && (object.Equals(((ThemeData)((ThemeData)__other)).bottomNavigationBarTheme, this.bottomNavigationBarTheme))) && (object.Equals(((ThemeData)((ThemeData)__other)).bottomSheetTheme, this.bottomSheetTheme))) && (object.Equals(((ThemeData)((ThemeData)__other)).buttonTheme, this.buttonTheme))) && (object.Equals(((ThemeData)((ThemeData)__other)).cardTheme, this.cardTheme))) && (object.Equals(((ThemeData)((ThemeData)__other)).carouselViewTheme, this.carouselViewTheme))) && (object.Equals(((ThemeData)((ThemeData)__other)).checkboxTheme, this.checkboxTheme))) && (object.Equals(((ThemeData)((ThemeData)__other)).chipTheme, this.chipTheme))) && (object.Equals(((ThemeData)((ThemeData)__other)).dataTableTheme, this.dataTableTheme))) && (object.Equals(((ThemeData)((ThemeData)__other)).datePickerTheme, this.datePickerTheme))) && (object.Equals(((ThemeData)((ThemeData)__other)).dialogTheme, this.dialogTheme))) && (object.Equals(((ThemeData)((ThemeData)__other)).dividerTheme, this.dividerTheme))) && (object.Equals(((ThemeData)((ThemeData)__other)).drawerTheme, this.drawerTheme))) && (object.Equals(((ThemeData)((ThemeData)__other)).dropdownMenuTheme, this.dropdownMenuTheme))) && (object.Equals(((ThemeData)((ThemeData)__other)).elevatedButtonTheme, this.elevatedButtonTheme))) && (object.Equals(((ThemeData)((ThemeData)__other)).expansionTileTheme, this.expansionTileTheme))) && (object.Equals(((ThemeData)((ThemeData)__other)).filledButtonTheme, this.filledButtonTheme))) && (object.Equals(((ThemeData)((ThemeData)__other)).floatingActionButtonTheme, this.floatingActionButtonTheme))) && (object.Equals(((ThemeData)((ThemeData)__other)).iconButtonTheme, this.iconButtonTheme))) && (object.Equals(((ThemeData)((ThemeData)__other)).listTileTheme, this.listTileTheme))) && (object.Equals(((ThemeData)((ThemeData)__other)).menuBarTheme, this.menuBarTheme))) && (object.Equals(((ThemeData)((ThemeData)__other)).menuButtonTheme, this.menuButtonTheme))) && (object.Equals(((ThemeData)((ThemeData)__other)).menuTheme, this.menuTheme))) && (object.Equals(((ThemeData)((ThemeData)__other)).navigationBarTheme, this.navigationBarTheme))) && (object.Equals(((ThemeData)((ThemeData)__other)).navigationDrawerTheme, this.navigationDrawerTheme))) && (object.Equals(((ThemeData)((ThemeData)__other)).navigationRailTheme, this.navigationRailTheme))) && (object.Equals(((ThemeData)((ThemeData)__other)).outlinedButtonTheme, this.outlinedButtonTheme))) && (object.Equals(((ThemeData)((ThemeData)__other)).popupMenuTheme, this.popupMenuTheme))) && (object.Equals(((ThemeData)((ThemeData)__other)).progressIndicatorTheme, this.progressIndicatorTheme))) && (object.Equals(((ThemeData)((ThemeData)__other)).radioTheme, this.radioTheme))) && (object.Equals(((ThemeData)((ThemeData)__other)).searchBarTheme, this.searchBarTheme))) && (object.Equals(((ThemeData)((ThemeData)__other)).searchViewTheme, this.searchViewTheme))) && (object.Equals(((ThemeData)((ThemeData)__other)).segmentedButtonTheme, this.segmentedButtonTheme))) && (object.Equals(((ThemeData)((ThemeData)__other)).sliderTheme, this.sliderTheme))) && (object.Equals(((ThemeData)((ThemeData)__other)).snackBarTheme, this.snackBarTheme))) && (object.Equals(((ThemeData)((ThemeData)__other)).switchTheme, this.switchTheme))) && (object.Equals(((ThemeData)((ThemeData)__other)).tabBarTheme, this.tabBarTheme))) && (object.Equals(((ThemeData)((ThemeData)__other)).textButtonTheme, this.textButtonTheme))) && (object.Equals(((ThemeData)((ThemeData)__other)).textSelectionTheme, this.textSelectionTheme))) && (object.Equals(((ThemeData)((ThemeData)__other)).timePickerTheme, this.timePickerTheme))) && (object.Equals(((ThemeData)((ThemeData)__other)).toggleButtonsTheme, this.toggleButtonsTheme))) && (object.Equals(((ThemeData)((ThemeData)__other)).tooltipTheme, this.tooltipTheme))) && (object.Equals(((ThemeData)((ThemeData)__other)).buttonBarTheme, this.buttonBarTheme))) && (object.Equals(((ThemeData)((ThemeData)__other)).dialogBackgroundColor, this.dialogBackgroundColor))) && (object.Equals(((ThemeData)((ThemeData)__other)).indicatorColor, this.indicatorColor)));
    }

    public override int GetHashCode()
    {
        var values = ((Func<List<object?>>)(() => { var __collection95592 = new List<object?>(); __collection95592.AddRange(this.adaptationMap.Keys); __collection95592.AddRange(this.adaptationMap.Values); __collection95592.Add(this.applyElevationOverlayColor); __collection95592.Add(this.cupertinoOverrideTheme); __collection95592.AddRange(this.extensions.Keys); __collection95592.AddRange(this.extensions.Values); __collection95592.Add(this.inputDecorationTheme); __collection95592.Add(this.materialTapTargetSize); __collection95592.Add(this.pageTransitionsTheme); __collection95592.Add(this.platform); __collection95592.Add(this.scrollbarTheme); __collection95592.Add(this.splashFactory); __collection95592.Add(this.useMaterial3); __collection95592.Add(this.visualDensity); __collection95592.Add(this.canvasColor); __collection95592.Add(this.cardColor); __collection95592.Add(this.colorScheme); __collection95592.Add(this.disabledColor); __collection95592.Add(this.dividerColor); __collection95592.Add(this.focusColor); __collection95592.Add(this.highlightColor); __collection95592.Add(this.hintColor); __collection95592.Add(this.hoverColor); __collection95592.Add(this.primaryColor); __collection95592.Add(this.primaryColorDark); __collection95592.Add(this.primaryColorLight); __collection95592.Add(this.scaffoldBackgroundColor); __collection95592.Add(this.secondaryHeaderColor); __collection95592.Add(this.shadowColor); __collection95592.Add(this.splashColor); __collection95592.Add(this.unselectedWidgetColor); __collection95592.Add(this.iconTheme); __collection95592.Add(this.primaryIconTheme); __collection95592.Add(this.primaryTextTheme); __collection95592.Add(this.textTheme); __collection95592.Add(this.typography); __collection95592.Add(this.actionIconTheme); __collection95592.Add(this.appBarTheme); __collection95592.Add(this.badgeTheme); __collection95592.Add(this.bannerTheme); __collection95592.Add(this.bottomAppBarTheme); __collection95592.Add(this.bottomNavigationBarTheme); __collection95592.Add(this.bottomSheetTheme); __collection95592.Add(this.buttonTheme); __collection95592.Add(this.cardTheme); __collection95592.Add(this.carouselViewTheme); __collection95592.Add(this.checkboxTheme); __collection95592.Add(this.chipTheme); __collection95592.Add(this.dataTableTheme); __collection95592.Add(this.datePickerTheme); __collection95592.Add(this.dialogTheme); __collection95592.Add(this.dividerTheme); __collection95592.Add(this.drawerTheme); __collection95592.Add(this.dropdownMenuTheme); __collection95592.Add(this.elevatedButtonTheme); __collection95592.Add(this.expansionTileTheme); __collection95592.Add(this.filledButtonTheme); __collection95592.Add(this.floatingActionButtonTheme); __collection95592.Add(this.iconButtonTheme); __collection95592.Add(this.listTileTheme); __collection95592.Add(this.menuBarTheme); __collection95592.Add(this.menuButtonTheme); __collection95592.Add(this.menuTheme); __collection95592.Add(this.navigationBarTheme); __collection95592.Add(this.navigationDrawerTheme); __collection95592.Add(this.navigationRailTheme); __collection95592.Add(this.outlinedButtonTheme); __collection95592.Add(this.popupMenuTheme); __collection95592.Add(this.progressIndicatorTheme); __collection95592.Add(this.radioTheme); __collection95592.Add(this.searchBarTheme); __collection95592.Add(this.searchViewTheme); __collection95592.Add(this.segmentedButtonTheme); __collection95592.Add(this.sliderTheme); __collection95592.Add(this.snackBarTheme); __collection95592.Add(this.switchTheme); __collection95592.Add(this.tabBarTheme); __collection95592.Add(this.textButtonTheme); __collection95592.Add(this.textSelectionTheme); __collection95592.Add(this.timePickerTheme); __collection95592.Add(this.toggleButtonsTheme); __collection95592.Add(this.tooltipTheme); __collection95592.Add(this.buttonBarTheme); __collection95592.Add(this.dialogBackgroundColor); __collection95592.Add(this.indicatorColor); return __collection95592; }))();
        return FoundationRuntimePorts.ObjectHashAll(values);
        return default!;
    }
    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        var defaultData = ThemeData.CreateFallback();
        properties.add(new global::Doroti.Framework.Foundation.IterableProperty<Adaptation<object>>("adaptations", this.adaptationMap.Values.Cast<Adaptation<object>>(), defaultValue: ((ThemeData)defaultData).adaptationMap.Values, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("applyElevationOverlayColor", this.applyElevationOverlayColor, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<NoDefaultCupertinoThemeData>("cupertinoOverrideTheme", this.cupertinoOverrideTheme, defaultValue: ((ThemeData)defaultData).cupertinoOverrideTheme, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.IterableProperty<ThemeExtension<object>>("extensions", this.extensions.Values.Cast<ThemeExtension<object>>(), defaultValue: ((ThemeData)defaultData).extensions.Values, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<InputDecorationThemeData>("inputDecorationTheme", this.inputDecorationTheme, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<MaterialTapTargetSize>("materialTapTargetSize", this.materialTapTargetSize, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<PageTransitionsTheme>("pageTransitionsTheme", this.pageTransitionsTheme, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<global::Doroti.Framework.Foundation.TargetPlatform>("platform", this.platform, defaultValue: global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<ScrollbarThemeData>("scrollbarTheme", this.scrollbarTheme, defaultValue: ((ThemeData)defaultData).scrollbarTheme, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<InteractiveInkFeatureFactory>("splashFactory", this.splashFactory, defaultValue: ((ThemeData)defaultData).splashFactory, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("useMaterial3", this.useMaterial3, defaultValue: ((ThemeData)defaultData).useMaterial3, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<VisualDensity>("visualDensity", this.visualDensity, defaultValue: ((ThemeData)defaultData).visualDensity, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("canvasColor", this.canvasColor, defaultValue: ((ThemeData)defaultData).canvasColor, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("cardColor", this.cardColor, defaultValue: ((ThemeData)defaultData).cardColor, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<ColorScheme>("colorScheme", this.colorScheme, defaultValue: ((ThemeData)defaultData).colorScheme, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("disabledColor", this.disabledColor, defaultValue: ((ThemeData)defaultData).disabledColor, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("dividerColor", this.dividerColor, defaultValue: ((ThemeData)defaultData).dividerColor, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("focusColor", this.focusColor, defaultValue: ((ThemeData)defaultData).focusColor, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("highlightColor", this.highlightColor, defaultValue: ((ThemeData)defaultData).highlightColor, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("hintColor", this.hintColor, defaultValue: ((ThemeData)defaultData).hintColor, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("hoverColor", this.hoverColor, defaultValue: ((ThemeData)defaultData).hoverColor, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("primaryColorDark", this.primaryColorDark, defaultValue: ((ThemeData)defaultData).primaryColorDark, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("primaryColorLight", this.primaryColorLight, defaultValue: ((ThemeData)defaultData).primaryColorLight, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("primaryColor", this.primaryColor, defaultValue: ((ThemeData)defaultData).primaryColor, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("scaffoldBackgroundColor", this.scaffoldBackgroundColor, defaultValue: ((ThemeData)defaultData).scaffoldBackgroundColor, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("secondaryHeaderColor", this.secondaryHeaderColor, defaultValue: ((ThemeData)defaultData).secondaryHeaderColor, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("shadowColor", this.shadowColor, defaultValue: ((ThemeData)defaultData).shadowColor, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("splashColor", this.splashColor, defaultValue: ((ThemeData)defaultData).splashColor, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("unselectedWidgetColor", this.unselectedWidgetColor, defaultValue: ((ThemeData)defaultData).unselectedWidgetColor, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.IconThemeData>("iconTheme", this.iconTheme, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.IconThemeData>("primaryIconTheme", this.primaryIconTheme, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<TextTheme>("primaryTextTheme", this.primaryTextTheme, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<TextTheme>("textTheme", this.textTheme, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<Typography>("typography", this.typography, defaultValue: ((ThemeData)defaultData).typography, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<ActionIconThemeData>("actionIconTheme", this.actionIconTheme, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<AppBarThemeData>("appBarTheme", this.appBarTheme, defaultValue: ((ThemeData)defaultData).appBarTheme, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<BadgeThemeData>("badgeTheme", this.badgeTheme, defaultValue: ((ThemeData)defaultData).badgeTheme, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<MaterialBannerThemeData>("bannerTheme", this.bannerTheme, defaultValue: ((ThemeData)defaultData).bannerTheme, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<BottomAppBarThemeData>("bottomAppBarTheme", this.bottomAppBarTheme, defaultValue: ((ThemeData)defaultData).bottomAppBarTheme, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<BottomNavigationBarThemeData>("bottomNavigationBarTheme", this.bottomNavigationBarTheme, defaultValue: ((ThemeData)defaultData).bottomNavigationBarTheme, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<BottomSheetThemeData>("bottomSheetTheme", this.bottomSheetTheme, defaultValue: ((ThemeData)defaultData).bottomSheetTheme, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<ButtonThemeData>("buttonTheme", this.buttonTheme, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<CardThemeData>("cardTheme", this.cardTheme, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<CarouselViewThemeData>("carouselViewTheme", this.carouselViewTheme, defaultValue: ((ThemeData)defaultData).carouselViewTheme, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<CheckboxThemeData>("checkboxTheme", this.checkboxTheme, defaultValue: ((ThemeData)defaultData).checkboxTheme, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<ChipThemeData>("chipTheme", this.chipTheme, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<DataTableThemeData>("dataTableTheme", this.dataTableTheme, defaultValue: ((ThemeData)defaultData).dataTableTheme, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<DatePickerThemeData>("datePickerTheme", this.datePickerTheme, defaultValue: ((ThemeData)defaultData).datePickerTheme, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<DialogThemeData>("dialogTheme", this.dialogTheme, defaultValue: ((ThemeData)defaultData).dialogTheme, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<DividerThemeData>("dividerTheme", this.dividerTheme, defaultValue: ((ThemeData)defaultData).dividerTheme, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<DrawerThemeData>("drawerTheme", this.drawerTheme, defaultValue: ((ThemeData)defaultData).drawerTheme, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<DropdownMenuThemeData>("dropdownMenuTheme", this.dropdownMenuTheme, defaultValue: ((ThemeData)defaultData).dropdownMenuTheme, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<ElevatedButtonThemeData>("elevatedButtonTheme", this.elevatedButtonTheme, defaultValue: ((ThemeData)defaultData).elevatedButtonTheme, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<ExpansionTileThemeData>("expansionTileTheme", this.expansionTileTheme, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<FilledButtonThemeData>("filledButtonTheme", this.filledButtonTheme, defaultValue: ((ThemeData)defaultData).filledButtonTheme, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<FloatingActionButtonThemeData>("floatingActionButtonTheme", this.floatingActionButtonTheme, defaultValue: ((ThemeData)defaultData).floatingActionButtonTheme, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<IconButtonThemeData>("iconButtonTheme", this.iconButtonTheme, defaultValue: ((ThemeData)defaultData).iconButtonTheme, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<ListTileThemeData>("listTileTheme", this.listTileTheme, defaultValue: ((ThemeData)defaultData).listTileTheme, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<MenuBarThemeData>("menuBarTheme", this.menuBarTheme, defaultValue: ((ThemeData)defaultData).menuBarTheme, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<MenuButtonThemeData>("menuButtonTheme", this.menuButtonTheme, defaultValue: ((ThemeData)defaultData).menuButtonTheme, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<MenuThemeData>("menuTheme", this.menuTheme, defaultValue: ((ThemeData)defaultData).menuTheme, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<NavigationBarThemeData>("navigationBarTheme", this.navigationBarTheme, defaultValue: ((ThemeData)defaultData).navigationBarTheme, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<NavigationDrawerThemeData>("navigationDrawerTheme", this.navigationDrawerTheme, defaultValue: ((ThemeData)defaultData).navigationDrawerTheme, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<NavigationRailThemeData>("navigationRailTheme", this.navigationRailTheme, defaultValue: ((ThemeData)defaultData).navigationRailTheme, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<OutlinedButtonThemeData>("outlinedButtonTheme", this.outlinedButtonTheme, defaultValue: ((ThemeData)defaultData).outlinedButtonTheme, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<PopupMenuThemeData>("popupMenuTheme", this.popupMenuTheme, defaultValue: ((ThemeData)defaultData).popupMenuTheme, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<ProgressIndicatorThemeData>("progressIndicatorTheme", this.progressIndicatorTheme, defaultValue: ((ThemeData)defaultData).progressIndicatorTheme, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<RadioThemeData>("radioTheme", this.radioTheme, defaultValue: ((ThemeData)defaultData).radioTheme, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<SearchBarThemeData>("searchBarTheme", this.searchBarTheme, defaultValue: ((ThemeData)defaultData).searchBarTheme, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<SearchViewThemeData>("searchViewTheme", this.searchViewTheme, defaultValue: ((ThemeData)defaultData).searchViewTheme, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<SegmentedButtonThemeData>("segmentedButtonTheme", this.segmentedButtonTheme, defaultValue: ((ThemeData)defaultData).segmentedButtonTheme, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<SliderThemeData>("sliderTheme", this.sliderTheme, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<SnackBarThemeData>("snackBarTheme", this.snackBarTheme, defaultValue: ((ThemeData)defaultData).snackBarTheme, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<SwitchThemeData>("switchTheme", this.switchTheme, defaultValue: ((ThemeData)defaultData).switchTheme, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<TabBarThemeData>("tabBarTheme", this.tabBarTheme, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<TextButtonThemeData>("textButtonTheme", this.textButtonTheme, defaultValue: ((ThemeData)defaultData).textButtonTheme, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<TextSelectionThemeData>("textSelectionTheme", this.textSelectionTheme, defaultValue: ((ThemeData)defaultData).textSelectionTheme, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<TimePickerThemeData>("timePickerTheme", this.timePickerTheme, defaultValue: ((ThemeData)defaultData).timePickerTheme, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<ToggleButtonsThemeData>("toggleButtonsTheme", this.toggleButtonsTheme, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<TooltipThemeData>("tooltipTheme", this.tooltipTheme, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<ButtonBarThemeData>("buttonBarTheme", this.buttonBarTheme, defaultValue: ((ThemeData)defaultData).buttonBarTheme, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("dialogBackgroundColor", this.dialogBackgroundColor, defaultValue: ((ThemeData)defaultData).dialogBackgroundColor, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("indicatorColor", this.indicatorColor, defaultValue: ((ThemeData)defaultData).indicatorColor, level: global::Doroti.Framework.Foundation.DiagnosticLevel.debug));
    }

    public virtual string toStringShort() => global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                fullString = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine).toDiagnosticsNode().toStringDeep(minLevel: minLevel);
                return true;
            });
        return ((fullString ?? (string)toStringShort()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(string? name = null, DiagnosticsTreeStyle? style = null)
    {
        return ((DiagnosticsNode)(object?)new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class MaterialBasedCupertinoThemeData : CupertinoThemeData
{
    internal virtual ThemeData _materialTheme { get; private set; } = default!;
    internal virtual NoDefaultCupertinoThemeData _cupertinoOverrideTheme { get; private set; } = default!;

    public MaterialBasedCupertinoThemeData(ThemeData materialTheme) : this(materialTheme, ((((ThemeData)materialTheme).cupertinoOverrideTheme ?? new CupertinoThemeData())).noDefault())
    {
    }

    public MaterialBasedCupertinoThemeData(ThemeData _materialTheme, NoDefaultCupertinoThemeData _cupertinoOverrideTheme) : base(_cupertinoOverrideTheme.brightness, _cupertinoOverrideTheme.primaryColor, _cupertinoOverrideTheme.primaryContrastingColor, _cupertinoOverrideTheme.textTheme, _cupertinoOverrideTheme.barBackgroundColor, _cupertinoOverrideTheme.scaffoldBackgroundColor, (_cupertinoOverrideTheme.selectionHandleColor ?? ((ThemeData)_materialTheme).textSelectionTheme.selectionHandleColor), _cupertinoOverrideTheme.applyThemeToAll)
    {
        this._materialTheme = _materialTheme;
        this._cupertinoOverrideTheme = _cupertinoOverrideTheme;
    }

    public virtual global::Doroti.Ui.Brightness brightness => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Brightness>(((this._cupertinoOverrideTheme.brightness ?? (Brightness)((ThemeData)this._materialTheme).brightness)));
    public virtual global::Doroti.Ui.Color primaryColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>((this._cupertinoOverrideTheme.primaryColor ?? ((ThemeData)this._materialTheme).colorScheme.primary));
    public virtual global::Doroti.Ui.Color primaryContrastingColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>((this._cupertinoOverrideTheme.primaryContrastingColor ?? ((ThemeData)this._materialTheme).colorScheme.onPrimary));
    public virtual global::Doroti.Ui.Color scaffoldBackgroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>((this._cupertinoOverrideTheme.scaffoldBackgroundColor ?? ((ThemeData)this._materialTheme).scaffoldBackgroundColor));
    public virtual MaterialBasedCupertinoThemeData copyWith(Brightness? brightness = null, Color? primaryColor = null, Color? primaryContrastingColor = null, CupertinoTextThemeData? textTheme = null, Color? barBackgroundColor = null, Color? scaffoldBackgroundColor = null, Color? selectionHandleColor = null, bool? applyThemeToAll = null)
    {
        return new MaterialBasedCupertinoThemeData(this._materialTheme, this._cupertinoOverrideTheme.copyWith(brightness: brightness, primaryColor: primaryColor, primaryContrastingColor: primaryContrastingColor, textTheme: textTheme, barBackgroundColor: barBackgroundColor, scaffoldBackgroundColor: scaffoldBackgroundColor, selectionHandleColor: selectionHandleColor, applyThemeToAll: applyThemeToAll));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual CupertinoThemeData resolveFrom(global::Doroti.Framework.Widgets.BuildContext context)
    {
        NoDefaultCupertinoThemeData cupertinoOverrideThemeWithTextTheme = ((NoDefaultCupertinoThemeData)(object?)this._cupertinoOverrideTheme.copyWith(textTheme: textTheme));
        return ((CupertinoThemeData)(object?)new MaterialBasedCupertinoThemeData(this._materialTheme, cupertinoOverrideThemeWithTextTheme.resolveFrom(context)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class CupertinoBasedMaterialThemeData
{
    public virtual ThemeData materialTheme { get; private set; } = default!;

    public CupertinoBasedMaterialThemeData(CupertinoThemeData themeData)
    {
        this.materialTheme = ThemeData.Create(colorScheme: ColorScheme.CreateFromSeed(seedColor: themeData.primaryColor, brightness: (themeData.brightness ?? Brightness.light), primary: themeData.primaryColor, onPrimary: themeData.primaryContrastingColor));
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

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>((Dart_coreLibrary.identityHashCode(this.baseTheme) ^ Dart_coreLibrary.identityHashCode(this.localTextGeometry)));
    public override bool Equals(object? other)
    {
        var __other = other as _IdentityThemeDataCacheKey__theme_data;
        if (__other is null) return false;
        return (((__other is _IdentityThemeDataCacheKey__theme_data) && DartRuntimePrimitives.Identical(((_IdentityThemeDataCacheKey__theme_data)((_IdentityThemeDataCacheKey__theme_data)__other)).baseTheme, this.baseTheme)) && DartRuntimePrimitives.Identical(((_IdentityThemeDataCacheKey__theme_data)((_IdentityThemeDataCacheKey__theme_data)__other)).localTextGeometry, this.localTextGeometry));
    }

}

internal class _FifoCache__theme_data<K, V> where K : notnull
{
    internal virtual DartMap<K, V> _cache { get; private set; } = new DartMap<K, V>();
    internal virtual long _maximumSize { get; private set; } = default!;

    internal _FifoCache__theme_data(long _maximumSize)
    {
        this._maximumSize = _maximumSize;
        System.Diagnostics.Debug.Assert((_maximumSize > 0L));
    }

    public virtual V putIfAbsent(K key, global::System.Func<V> loader)
    {
        DartRuntimePrimitives.Assert(() => (key is not null));
        V? result = this._cache.GetValueOrDefault(key);
        if ((result is not null))
        {
            return ((V)(object?)result);
        }
        if ((checked((long)(this._cache.Count)) == this._maximumSize))
        {
            this._cache.remove(this._cache.Keys.First());
        }
        return this._cache[key] = loader();
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
        System.Diagnostics.Debug.Assert((vertical <= maximumDensity));
        System.Diagnostics.Debug.Assert((vertical >= minimumDensity));
        System.Diagnostics.Debug.Assert((horizontal <= maximumDensity));
        System.Diagnostics.Debug.Assert((horizontal >= minimumDensity));
    }

    public static VisualDensity adaptivePlatformDensity => VisualDensity.defaultDensityForPlatform(global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform);
    public static VisualDensity defaultDensityForPlatform(global::Doroti.Framework.Foundation.TargetPlatform platform)
    {
        return (platform switch { global::Doroti.Framework.Foundation.TargetPlatform.android or global::Doroti.Framework.Foundation.TargetPlatform.iOS => standard, global::Doroti.Framework.Foundation.TargetPlatform.fuchsia => standard, global::Doroti.Framework.Foundation.TargetPlatform.linux or global::Doroti.Framework.Foundation.TargetPlatform.macOS => compact, global::Doroti.Framework.Foundation.TargetPlatform.windows => compact, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual VisualDensity copyWith(double? horizontal = null, double? vertical = null)
    {
        return new VisualDensity(horizontal: (horizontal ?? this.horizontal), vertical: (vertical ?? this.vertical));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.Offset baseSizeAdjustment
    {
        get
        {
            var interval = 4.0;
            return (new global::Doroti.Ui.Offset(this.horizontal, this.vertical) * interval);
            return default!;
        }
    }
    public static VisualDensity lerp(VisualDensity a, VisualDensity b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        return new VisualDensity(horizontal: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(((VisualDensity)a).horizontal, ((VisualDensity)b).horizontal, t)), vertical: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(((VisualDensity)a).vertical, ((VisualDensity)b).vertical, t)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Rendering.BoxConstraints effectiveConstraints(global::Doroti.Framework.Rendering.BoxConstraints constraints)
    {
        DartRuntimePrimitives.Assert(() => constraints.debugAssertIsValid());
        return ((global::Doroti.Framework.Rendering.BoxConstraints)(object?)constraints.copyWith(minWidth: Dart_uiLibrary.clampDouble((((global::Doroti.Framework.Rendering.BoxConstraints)constraints).minWidth + this.baseSizeAdjustment.dx), 0.0, ((global::Doroti.Framework.Rendering.BoxConstraints)constraints).maxWidth), minHeight: Dart_uiLibrary.clampDouble((((global::Doroti.Framework.Rendering.BoxConstraints)constraints).minHeight + this.baseSizeAdjustment.dy), 0.0, ((global::Doroti.Framework.Rendering.BoxConstraints)constraints).maxHeight)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as VisualDensity;
        if (__other is null) return false;
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((__other is VisualDensity) && (((VisualDensity)((VisualDensity)__other)).horizontal == this.horizontal)) && (((VisualDensity)((VisualDensity)__other)).vertical == this.vertical));
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.horizontal, this.vertical));
    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("horizontal", this.horizontal, defaultValue: 0.0));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("vertical", this.vertical, defaultValue: 0.0));
    }

    public virtual string toStringShort()
    {
        return $"{(global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}(h: {(global::Doroti.Framework.Foundation.DebugLibrary.debugFormatDouble(this.horizontal))}, v: {(global::Doroti.Framework.Foundation.DebugLibrary.debugFormatDouble(this.vertical))})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                fullString = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine).toDiagnosticsNode().toStringDeep(minLevel: minLevel);
                return true;
            });
        return ((fullString ?? (string)toStringShort()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(string? name = null, DiagnosticsTreeStyle? style = null)
    {
        return ((DiagnosticsNode)(object?)new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style));
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
