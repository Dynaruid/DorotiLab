// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/localizations.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

internal class _Pending__localizations
{
    public virtual ILocalizationsDelegate @delegate { get; private set; } = default!;
    public virtual Future<object> futureValue { get; private set; } = default!;

    internal _Pending__localizations(ILocalizationsDelegate @delegate, Future<object> futureValue)
    {
        this.@delegate = @delegate;
        this.futureValue = futureValue;
    }
}

public static partial class LocalizationsLibrary
{
    internal static ILocalizationsDelegate RequireDelegate(object value) =>
        value as ILocalizationsDelegate
        ?? throw new ArgumentException(
            "Localization delegates must implement ILocalizationsDelegate (normally through LocalizationsDelegate<T>).",
            nameof(value)
        );

    internal static Future<DartMap<Type, object>> _loadAll(
        Locale locale,
        IEnumerable<object> allDelegates
    )
    {
        var output = new DartMap<Type, object>();
        List<_Pending__localizations>? pendingList = default!;
        var types = new HashSet<Type>();
        var delegates = new List<ILocalizationsDelegate>();
        foreach (var delegateLocal in allDelegates.Select(RequireDelegate))
        {
            if (!types.Contains(delegateLocal.type) && delegateLocal.isSupported(locale))
            {
                types.Add(delegateLocal.type);
                delegates.Add(delegateLocal);
            }
        }
        foreach (var delegateAlternate in delegates)
        {
            Future inputValue = delegateAlternate.loadUntyped(locale);
            object? completedValue = null;
            Future<object> futureValueLocal = inputValue.then<object>(
                (object? value) =>
                {
                    return completedValue =
                        value
                        ?? throw new InvalidOperationException(
                            "A localization delegate must return a resource."
                        );
                    throw new InvalidOperationException("Dart closure completed without a value.");
                }
            );
            if (completedValue is not null)
            {
                Type typeLocal = delegateAlternate.type;
                DartRuntimePrimitives.Assert(() => !output.ContainsKey(typeLocal));
                output[typeLocal] = completedValue;
            }
            else
            {
                pendingList ??= new List<_Pending__localizations>();
                pendingList.Add(new _Pending__localizations(delegateAlternate, futureValueLocal));
            }
        }
        if (pendingList is null)
        {
            return new SynchronousFuture<DartMap<Type, object>>(output);
        }
        return DartAsyncRuntime
            .wait<object>(pendingList.map<_Pending__localizations, Future>((p) => p.futureValue))
            .then(
                (values) =>
                {
                    DartRuntimePrimitives.Assert(() =>
                        checked(values.Count) == checked((long)pendingList!.Count)
                    );
                    for (var i = 0L; i < checked(values.Count); i += 1L)
                    {
                        Type typeAlternate = pendingList![(int)i].@delegate.type;
                        DartRuntimePrimitives.Assert(() => !output.ContainsKey(typeAlternate));
                        output[typeAlternate] = values[(int)i];
                    }
                    return output;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                }
            );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

/// <summary>Heterogeneous localization operations; generic results remain owned by the delegate.</summary>
public interface ILocalizationsDelegate
{
    Type type { get; }
    bool isSupported(Locale locale);
    Future loadUntyped(Locale locale);
    bool shouldReload(ILocalizationsDelegate old);
}

public abstract class LocalizationsDelegate<T> : ILocalizationsDelegate
{
    protected LocalizationsDelegate() { }

    Future ILocalizationsDelegate.loadUntyped(Locale locale) => load(locale);

    bool ILocalizationsDelegate.shouldReload(ILocalizationsDelegate old) =>
        old is not LocalizationsDelegate<T> typed
        || old.GetType() != GetType()
        || shouldReload(typed);

    public abstract bool isSupported(Locale locale);
    public abstract Future<T> load(Locale locale);
    public abstract bool shouldReload(LocalizationsDelegate<T> old);
    public virtual Type type => typeof(T);

    public override string ToString() =>
        $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "LocalizationsDelegate")}[{type}]";
}

public abstract class WidgetsLocalizations
{
    public WidgetsLocalizations() { }

    public abstract TextDirection textDirection { get; }
    public abstract string reorderItemToStart { get; }
    public abstract string reorderItemToEnd { get; }
    public abstract string reorderItemUp { get; }
    public abstract string reorderItemDown { get; }
    public abstract string reorderItemLeft { get; }
    public abstract string reorderItemRight { get; }
    public virtual string searchResultsFound => "Search results found";
    public virtual string noResultsFound => "No results found";
    public abstract string copyButtonLabel { get; }
    public abstract string cutButtonLabel { get; }
    public abstract string pasteButtonLabel { get; }
    public abstract string selectAllButtonLabel { get; }
    public abstract string lookUpButtonLabel { get; }
    public abstract string searchWebButtonLabel { get; }
    public abstract string shareButtonLabel { get; }
    public abstract string radioButtonUnselectedLabel { get; }

    public static WidgetsLocalizations of(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasWidgetsLocalizations(context));
        return Localizations.of<WidgetsLocalizations>(context, typeof(WidgetsLocalizations))!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _WidgetsLocalizationsDelegate__localizations
    : LocalizationsDelegate<WidgetsLocalizations>
{
    internal _WidgetsLocalizationsDelegate__localizations() { }

    public override bool isSupported(Locale locale) => true;

    public override Future<WidgetsLocalizations> load(Locale locale) =>
        DefaultWidgetsLocalizations.load(locale);

    public override bool shouldReload(LocalizationsDelegate<WidgetsLocalizations> old) => false;

    public override string ToString() => "DefaultWidgetsLocalizations.delegate(en_US)";
}

public class DefaultWidgetsLocalizations : WidgetsLocalizations
{
    public static LocalizationsDelegate<WidgetsLocalizations> @delegate =
        new _WidgetsLocalizationsDelegate__localizations();

    public DefaultWidgetsLocalizations() { }

    public override string reorderItemUp => "Move up";
    public override string reorderItemDown => "Move down";
    public override string reorderItemLeft => "Move left";
    public override string reorderItemRight => "Move right";
    public override string reorderItemToEnd => "Move to the end";
    public override string reorderItemToStart => "Move to the start";
    public override string searchResultsFound => "Search results found";
    public override string noResultsFound => "No results found";
    public override string copyButtonLabel => "Copy";
    public override string cutButtonLabel => "Cut";
    public override string pasteButtonLabel => "Paste";
    public override string selectAllButtonLabel => "Select all";
    public override string lookUpButtonLabel => "Look Up";
    public override string searchWebButtonLabel => "Search Web";
    public override string shareButtonLabel => "Share";
    public override string radioButtonUnselectedLabel => "Not selected";
    public override TextDirection textDirection => TextDirection.ltr;

    public static Future<WidgetsLocalizations> load(Locale locale)
    {
        return new SynchronousFuture<WidgetsLocalizations>(new DefaultWidgetsLocalizations());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _LocalizationsScope__localizations : InheritedWidget
{
    public virtual Locale locale { get; private set; } = default!;
    public virtual _LocalizationsState__localizations localizationsState { get; private set; } =
        default!;
    public virtual DartMap<Type, object> typeToResources { get; private set; } = default!;

    internal _LocalizationsScope__localizations(
        Key? key = null,
        Locale locale = default!,
        _LocalizationsState__localizations localizationsState = default!,
        DartMap<Type, object> typeToResources = default!,
        Widget child = default!
    )
        : base(key: key, child: child)
    {
        this.locale = locale;
        this.localizationsState = localizationsState;
        this.typeToResources = typeToResources;
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget)
    {
        var __old = (_LocalizationsScope__localizations)oldWidget;
        return !Equals(typeToResources, __old.typeToResources);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class Localizations : StatefulWidget
{
    public virtual Locale locale { get; private set; } = default!;
    public virtual List<dynamic> delegates { get; private set; } = default!;
    public virtual Widget? child { get; private set; }
    public virtual bool isApplicationLevel { get; private set; } = default!;

    public Localizations(
        Key? key = null,
        Locale locale = default!,
        List<dynamic> delegates = default!,
        Widget? child = null,
        bool isApplicationLevel = false
    )
        : base(key: key)
    {
        this.locale = locale;
        this.delegates = delegates;
        this.child = child;
        this.isApplicationLevel = isApplicationLevel;
        System.Diagnostics.Debug.Assert(
            delegates.any((@delegate) => @delegate is LocalizationsDelegate<WidgetsLocalizations>)
        );
    }

    public static Localizations CreateOverride(
        Key? key = null,
        BuildContext context = default!,
        Locale? locale = null,
        List<dynamic>? delegates = null,
        Widget? child = null
    )
    {
        List<object> mergedDelegates = _delegatesOf(context);
        if (delegates is not null)
        {
            mergedDelegates.InsertRange(checked((int)0L), delegates.Cast<dynamic>());
        }
        return new Localizations(
            key: key,
            locale: locale ?? localeOf(context),
            delegates: mergedDelegates,
            child: child
        );
    }

    public static Locale localeOf(BuildContext context)
    {
        _LocalizationsScope__localizations? scope =
            context.dependOnInheritedWidgetOfExactType<_LocalizationsScope__localizations>();
        DartRuntimePrimitives.Assert(() =>
        {
            if (scope is null)
            {
                throw DartRuntimePrimitives.AsException(
                    FlutterError.Create(
                        "Requested the Locale of a context that does not include a Localizations ancestor.\n"
                            + "To request the Locale, the context used to retrieve the Localizations widget must "
                            + "be that of a widget that is a descendant of a Localizations widget."
                    )
                );
            }
            if (scope.localizationsState.locale is null)
            {
                throw DartRuntimePrimitives.AsException(
                    FlutterError.Create(
                        "Localizations.localeOf found a Localizations widget that had a unexpected null locale.\n"
                    )
                );
            }
            return true;
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        return DartRuntimePrimitives.RequireValue(scope!.localizationsState.locale);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static Locale? maybeLocaleOf(BuildContext context)
    {
        _LocalizationsScope__localizations? scope =
            context.dependOnInheritedWidgetOfExactType<_LocalizationsScope__localizations>();
        return scope?.localizationsState.locale;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static List<object> _delegatesOf(BuildContext context)
    {
        _LocalizationsScope__localizations? scope =
            context.dependOnInheritedWidgetOfExactType<_LocalizationsScope__localizations>();
        DartRuntimePrimitives.Assert(
            () => scope is not null,
            () => (object?)"a Localizations ancestor was not found"
        );
        return new List<object>(
            DartRuntimePrimitives.ConvertEnumerable<object>(
                scope!.localizationsState.widget.delegates
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static T? of<T>(BuildContext context, Type type)
    {
        _LocalizationsScope__localizations? scope =
            context.dependOnInheritedWidgetOfExactType<_LocalizationsScope__localizations>();
        return scope is null ? default : scope.localizationsState.resourcesFor<T>(type);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _LocalizationsState__localizations());

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<Locale>("locale", locale));
        properties.add(new IterableProperty<object>("delegates", delegates.Cast<object>()));
    }
}

internal class _LocalizationsState__localizations : State<Localizations>
{
    internal virtual GlobalKey<IState> _localizedResourcesScopeKey { get; private set; } =
        GlobalKey<IState>.Create();
    internal virtual DartMap<Type, object> _typeToResources { get; set; } =
        new DartMap<Type, object>();
    internal virtual Locale? _locale { get; set; } = default;

    public virtual Locale? locale
    {
        get => _locale;
        set
        {
            var locale = value;
            DartRuntimePrimitives.Assert(() => locale is not null);
            if (Equals(_locale, locale))
            {
                return;
            }
            WidgetsBinding.instance.platformDispatcher.setApplicationLocale(
                DartRuntimePrimitives.RequireValue(locale)
            );
            _locale = DartRuntimePrimitives.RequireValue(locale);
        }
    }

    public override void initState()
    {
        base.initState();
        load(DartRuntimePrimitives.RequireValue(widget.locale));
    }

    internal virtual bool _anyDelegatesShouldReload(Localizations old)
    {
        if (checked(widget.delegates.Count) != checked((long)old.delegates.Count))
        {
            return true;
        }
        List<object> delegatesLocal = widget.delegates.ToList().Cast<object>().ToList();
        List<object> oldDelegates = old.delegates.ToList().Cast<object>().ToList();
        for (var i = 0L; i < checked(delegatesLocal.Count); i += 1L)
        {
            ILocalizationsDelegate @delegate = LocalizationsLibrary.RequireDelegate(
                delegatesLocal[(int)i]
            );
            ILocalizationsDelegate oldDelegate = LocalizationsLibrary.RequireDelegate(
                oldDelegates[(int)i]
            );
            if (
                (
                    !Equals(
                        DartRuntimePrimitives.RuntimeType(@delegate),
                        DartRuntimePrimitives.RuntimeType(oldDelegate)
                    )
                ) || @delegate.shouldReload(oldDelegate)
            )
            {
                return true;
            }
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void didUpdateWidget(Localizations old)
    {
        base.didUpdateWidget(old);
        if ((!Equals(widget.locale, old.locale)) || _anyDelegatesShouldReload(old))
        {
            load(DartRuntimePrimitives.RequireValue(widget.locale));
        }
    }

    public virtual void load(Locale locale)
    {
        IEnumerable<object> delegatesLocal = widget.delegates;
        if (!Enumerable.Any(delegatesLocal))
        {
            this.locale = DartRuntimePrimitives.RequireValue(locale);
            return;
        }
        DartMap<Type, object>? typeToResources = default!;
        Future<DartMap<Type, object>> typeToResourcesFuture = LocalizationsLibrary
            ._loadAll(
                DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(locale)),
                delegatesLocal.Cast<dynamic>()
            )
            .then(
                (value) =>
                {
                    return typeToResources = value.cast<Type, object>();
                    throw new InvalidOperationException("Dart closure completed without a value.");
                }
            );
        if (typeToResources is not null)
        {
            _typeToResources = typeToResources!;
            this.locale = DartRuntimePrimitives.RequireValue(locale);
        }
        else
        {
            RendererBinding.instance.deferFirstFrame();
            DartRuntimePrimitives.Ignore(
                typeToResourcesFuture.then(
                    (value) =>
                    {
                        if (mounted)
                        {
                            setState(() =>
                            {
                                _typeToResources = value;
                                this.locale = DartRuntimePrimitives.RequireValue(locale);
                            });
                        }
                        RendererBinding.instance.allowFirstFrame();
                    }
                )
            );
        }
    }

    public virtual T resourcesFor<T>(Type type)
    {
        var resources = ((T?)_typeToResources.GetValueOrDefault(type))!;
        return resources;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual TextDirection _textDirection
    {
        get
        {
            var resources = (
                (WidgetsLocalizations?)
                    _typeToResources.GetValueOrDefault(typeof(WidgetsLocalizations))
            )!;
            return resources.textDirection;
        }
    }

    public override Widget build(BuildContext context)
    {
        if (_locale is null)
        {
            return SizedBox.CreateShrink();
        }
        return new Semantics(
            localeForSubtree: widget.isApplicationLevel ? null : widget.locale,
            container: !widget.isApplicationLevel,
            textDirection: _textDirection,
            child: new _LocalizationsScope__localizations(
                key: _localizedResourcesScopeKey,
                locale: DartRuntimePrimitives.RequireValue(_locale),
                localizationsState: this,
                typeToResources: _typeToResources,
                child: new Directionality(textDirection: _textDirection, child: widget.child!)
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class LocalizationsResolver : ChangeNotifier, WidgetsBindingObserver
{
    internal virtual IEnumerable<dynamic>? _localizationsDelegates { get; set; } = default;
    internal virtual Func<
        List<Locale>?,
        IEnumerable<Locale>,
        Locale?
    >? _localeListResolutionCallback { get; set; } = default;
    internal virtual Func<
        Locale?,
        IEnumerable<Locale>,
        Locale?
    >? _localeResolutionCallback { get; set; } = default;
    internal virtual IEnumerable<Locale> _supportedLocales { get; set; } = default!;
    internal virtual Locale? _locale { get; set; } = default;
    internal virtual Locale? _resolvedLocale { get; set; } = default;

    public LocalizationsResolver(
        IEnumerable<Locale> supportedLocales,
        Locale? locale = null,
        Func<List<Locale>?, IEnumerable<Locale>, Locale?>? localeListResolutionCallback = null,
        Func<Locale?, IEnumerable<Locale>, Locale?>? localeResolutionCallback = null,
        IEnumerable<dynamic>? localizationsDelegates = null
    )
    {
        _locale = locale;
        _localeListResolutionCallback = localeListResolutionCallback;
        _localeResolutionCallback = localeResolutionCallback;
        _localizationsDelegates = localizationsDelegates;
        _supportedLocales = supportedLocales;
        _resolvedLocale = _resolveLocales(
            WidgetsBinding.instance.platformDispatcher.locales.ToList(),
            supportedLocales
        );
        WidgetsBinding.instance.addObserver(this);
    }

    public override void dispose()
    {
        WidgetsBinding.instance.removeObserver(this);
        base.dispose();
    }

    public virtual void update(
        Locale? locale,
        Func<List<Locale>?, IEnumerable<Locale>, Locale?>? localeListResolutionCallback,
        Func<Locale?, IEnumerable<Locale>, Locale?>? localeResolutionCallback,
        IEnumerable<dynamic>? localizationsDelegates,
        IEnumerable<Locale> supportedLocales
    )
    {
        _locale = locale;
        _localeListResolutionCallback = localeListResolutionCallback;
        _localeResolutionCallback = localeResolutionCallback;
        _localizationsDelegates = localizationsDelegates;
        if (!Equals(_supportedLocales, supportedLocales))
        {
            _supportedLocales = supportedLocales;
            _updateResolvedLocale(WidgetsBinding.instance.platformDispatcher.locales.ToList());
        }
    }

    public virtual Locale locale
    {
        get
        {
            Locale appLocale =
                (_locale is not null)
                    ? _resolveLocales(
                        new List<Locale> { DartRuntimePrimitives.RequireValue(_locale) },
                        supportedLocales.Cast<Locale>()
                    )
                    : DartRuntimePrimitives.RequireValue(_resolvedLocale);
            DartRuntimePrimitives.Assert(() => _debugCheckLocalizations(appLocale));
            return appLocale;
        }
    }
    public virtual IEnumerable<object> localizationsDelegates
    {
        get
        {
            var delegates = new List<ILocalizationsDelegate>();
            if (_localizationsDelegates is not null)
            {
                delegates.AddRange(
                    _localizationsDelegates
                        .Cast<object>()
                        .Select(LocalizationsLibrary.RequireDelegate)
                );
            }
            delegates.Add(DefaultWidgetsLocalizations.@delegate);
            return delegates;
        }
    }
    public virtual Func<
        List<Locale>?,
        IEnumerable<Locale>,
        Locale?
    >? localeListResolutionCallback => _localeListResolutionCallback;
    public virtual Func<Locale?, IEnumerable<Locale>, Locale?>? localeResolutionCallback =>
        _localeResolutionCallback;
    public virtual IEnumerable<Locale> supportedLocales =>
        DartRuntimePrimitives.ConvertValue<IEnumerable<Locale>>(_supportedLocales);

    public virtual void didChangeLocales(List<Locale>? locales)
    {
        _updateResolvedLocale(locales);
    }

    internal virtual void _updateResolvedLocale(List<Locale>? preferredLocales)
    {
        Locale newLocale = _resolveLocales(preferredLocales, supportedLocales.Cast<Locale>());
        if (!Equals(newLocale, _resolvedLocale))
        {
            _resolvedLocale = newLocale;
            notifyListeners();
        }
    }

    internal virtual Locale _resolveLocales(
        List<Locale>? preferredLocales,
        IEnumerable<Locale> supportedLocales
    )
    {
        if (localeListResolutionCallback is not null)
        {
            Locale? locale = localeListResolutionCallback!(preferredLocales, supportedLocales);
            if (locale is not null)
            {
                Locale locale__32547__value32633 = DartRuntimePrimitives.RequireValue(locale);
                return DartRuntimePrimitives.RequireValue(
                    DartRuntimePrimitives.RequireValue(locale__32547__value32633)
                );
            }
        }
        if (localeResolutionCallback is not null)
        {
            Locale? localeLocal = localeResolutionCallback!(
                ((preferredLocales is not null) && Enumerable.Any(preferredLocales))
                    ? preferredLocales.First()
                    : null,
                supportedLocales
            );
            if (localeLocal is not null)
            {
                Locale locale__32838__value33016 = DartRuntimePrimitives.RequireValue(localeLocal);
                return DartRuntimePrimitives.RequireValue(
                    DartRuntimePrimitives.RequireValue(locale__32838__value33016)
                );
            }
        }
        return AppLibrary.basicLocaleListResolution(
            preferredLocales,
            supportedLocales.Cast<Locale>()
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString() => $"{typeof(LocalizationsResolver)}";

    internal virtual bool _debugCheckLocalizations(Locale locale)
    {
        DartRuntimePrimitives.Assert(() =>
        {
            HashSet<Type> unsupportedTypes = localizationsDelegates
                .Cast<object>()
                .Select(LocalizationsLibrary.RequireDelegate)
                .Select(item => item.type)
                .ToHashSet();
            foreach (
                ILocalizationsDelegate delegateLocal in localizationsDelegates
                    .Cast<object>()
                    .Select(LocalizationsLibrary.RequireDelegate)
            )
            {
                if (!unsupportedTypes.Contains(delegateLocal.type))
                {
                    continue;
                }
                if (delegateLocal.isSupported(locale))
                {
                    unsupportedTypes.Remove(delegateLocal.type);
                }
            }
            if (!Enumerable.Any(unsupportedTypes))
            {
                return true;
            }
            FlutterError.reportError(
                new FlutterErrorDetails(
                    exception: $"Warning: This application's locale, {DartRuntimePrimitives.RequireValue(locale)}, is not supported by all of its localization delegates.",
                    library: "widgets",
                    informationCollector: (InformationCollector)(
                        () =>
                            new List<DiagnosticsNode>
                            {
                                new ErrorSpacer(),
                                new ErrorHint(
                                    $"The declared supported locales for this app are: {string.Join(", ", supportedLocales)}"
                                ),
                                new ErrorSpacer(),
                                new ErrorDescription(
                                    "See https://flutter.dev/to/internationalization/ for more "
                                        + "information about configuring an app's locale, supportedLocales, "
                                        + "and localizationsDelegates parameters."
                                ),
                            }
                    )
                )
            );
            return true;
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}
