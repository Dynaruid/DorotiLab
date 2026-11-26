// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/localizations.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

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

        List<Task<(Type Type, object Resource)>>? pending = null;
        try
        {
            foreach (var localizationDelegate in delegates)
            {
                var type = localizationDelegate.type;
                var result = localizationDelegate.loadUntyped(locale).then<(Type, object)>(value =>
                    (type, value ?? throw new InvalidOperationException($"{type.Name} returned a null resource.")));
                if (result is SynchronousFuture<(Type, object)>)
                {
                    output[type] = result.asTask().GetAwaiter().GetResult().Item2;
                }
                else
                {
                    (pending ??= []).Add(result.asTask());
                }
            }
        }
        catch
        {
            if (pending is not null) _ = ObserveFailureAsync(Task.WhenAll(pending));
            throw;
        }

        return pending is null
            ? new SynchronousFuture<DartMap<Type, object>>(output)
            : Future<DartMap<Type, object>>.fromTask(CompletePendingAsync(pending, output));
    }

    private static async Task<DartMap<Type, object>> CompletePendingAsync(
        List<Task<(Type Type, object Resource)>> pending, DartMap<Type, object> output)
    {
        try
        {
            // A failed delegate must release the frame even if another delegate never completes.
            await foreach (var completed in Task.WhenEach(pending).ConfigureAwait(false))
            {
                var (type, resource) = await completed.ConfigureAwait(false);
                output[type] = resource;
            }
            return output;
        }
        catch
        {
            _ = ObserveFailureAsync(Task.WhenAll(pending));
            throw;
        }
    }

    internal static async Task ObserveFailureAsync(Task task)
    {
        try { await task.ConfigureAwait(false); }
        catch { /* The current request reports its first error. */ }
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
        throw new InvalidOperationException("Control flow completed without returning a value.");
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
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _LocalizationsScope__localizations : InheritedWidget
{
    public virtual Locale locale { get; private set; } = default!;
    public IReadOnlyList<object> delegates { get; }
    public virtual DartMap<Type, object> typeToResources { get; private set; } = default!;

    internal _LocalizationsScope__localizations(
        Key? key = null,
        Locale locale = default!,
        IReadOnlyList<object> delegates = default!,
        DartMap<Type, object> typeToResources = default!,
        Widget child = default!
    )
        : base(key: key, child: child)
    {
        this.locale = locale;
        this.delegates = delegates;
        this.typeToResources = typeToResources;
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget)
    {
        var __old = (_LocalizationsScope__localizations)oldWidget;
        return locale != __old.locale || !ReferenceEquals(typeToResources, __old.typeToResources);
        throw new InvalidOperationException("Control flow completed without returning a value.");
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
        this.locale = locale.normalized();
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
            return true;
            throw new InvalidOperationException("Callback completed without returning a value.");
        });
        return scope!.locale;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static Locale? maybeLocaleOf(BuildContext context)
    {
        _LocalizationsScope__localizations? scope =
            context.dependOnInheritedWidgetOfExactType<_LocalizationsScope__localizations>();
        return scope?.locale;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal static List<object> _delegatesOf(BuildContext context)
    {
        _LocalizationsScope__localizations? scope =
            context.dependOnInheritedWidgetOfExactType<_LocalizationsScope__localizations>();
        DartRuntimePrimitives.Assert(
            () => scope is not null,
            () => (object?)"a Localizations ancestor was not found"
        );
        return new List<object>(scope!.delegates);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static T? of<T>(BuildContext context, Type type)
    {
        _LocalizationsScope__localizations? scope =
            context.dependOnInheritedWidgetOfExactType<_LocalizationsScope__localizations>();
        return scope is null ? default : (T?)scope.typeToResources.GetValueOrDefault(type);
        throw new InvalidOperationException("Control flow completed without returning a value.");
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
    private DartMap<Type, object> _typeToResources = new();
    private IReadOnlyList<object> _loadedDelegates = [];
    private Locale? _locale;
    private DorotiView? _ownerView;
    private FlutterErrorDetails? _initialLoadError;
    private CancellationTokenSource? _pendingLoad;
    private RendererBinding? _deferredRenderer;

    public override void initState()
    {
        base.initState();
        load((widget.locale));
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        if (!ReferenceEquals(_ownerView, View.maybeOf(context))) load(widget.locale);
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
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void didUpdateWidget(Localizations old)
    {
        base.didUpdateWidget(old);
        if (widget.locale != old.locale || widget.isApplicationLevel != old.isApplicationLevel
            || _anyDelegatesShouldReload(old))
        {
            load(widget.locale);
        }
    }

    public virtual void load(Locale locale)
    {
        CancelPendingLoad();
        var view = LookupBoundary.getInheritedWidgetOfExactType<_ViewScope__view>(context)?.view;
        _ownerView = view;
        using var viewScope = view?.EnterInvocationScope();
        var delegates = widget.delegates.ToArray();
        _initialLoadError = null;
        try
        {
            var resources = LocalizationsLibrary._loadAll(locale, delegates);
            if (resources is SynchronousFuture<DartMap<Type, object>>)
            {
                ApplyResources(locale, view, delegates, resources.asTask().GetAwaiter().GetResult());
                ReleaseFirstFrame();
                return;
            }

            var owner = DorotiExecutionContext.CaptureDispatcher()
                ?? throw new InvalidOperationException("Asynchronous localizations require a framework dispatcher.");
            if (_locale is null && _deferredRenderer is null)
            {
                _deferredRenderer = RendererBinding.instance;
                _deferredRenderer.deferFirstFrame();
            }
            var request = new CancellationTokenSource();
            _pendingLoad = request;
            DartRuntimePrimitives.ObserveTask(
                CompleteLoadAsync(resources.asTask(), request, owner, locale, view, delegates),
                "localization resource load"
            );
        }
        catch (Exception error)
        {
            ReleaseFirstFrame();
            ReportLoadError(locale, error);
        }
    }

    private async Task CompleteLoadAsync(Task<DartMap<Type, object>> loading,
        CancellationTokenSource request, DorotiCallbackDispatcher owner, Locale locale,
        DorotiView? view, IReadOnlyList<object> delegates)
    {
        var token = request.Token;
        DartMap<Type, object>? resources = null;
        Exception? failure = null;
        try { resources = await loading.WaitAsync(token).ConfigureAwait(false); }
        catch (Exception error) { failure = error; }

        if (token.IsCancellationRequested)
        {
            _ = LocalizationsLibrary.ObserveFailureAsync(loading);
            return;
        }
        try
        {
            await owner.PostAsync(() =>
            {
                if (!mounted || !ReferenceEquals(_pendingLoad, request)) return;
                _pendingLoad = null;
                request.Dispose();
                try
                {
                    setState(() =>
                    {
                        if (failure is not null) ReportLoadError(locale, failure);
                        else ApplyResources(locale, view, delegates, resources!);
                    });
                }
                finally { ReleaseFirstFrame(); }
            }).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (owner.Lifetime.IsCancellationRequested || token.IsCancellationRequested) { }
    }

    private void ApplyResources(Locale locale, DorotiView? view, IReadOnlyList<object> delegates,
        DartMap<Type, object> resources)
    {
        if (resources.GetValueOrDefault(typeof(WidgetsLocalizations)) is not WidgetsLocalizations)
        {
            ReportLoadError(locale, new InvalidOperationException("No WidgetsLocalizations resource was loaded."));
            return;
        }
        if (widget.isApplicationLevel)
            WidgetsBinding.instance.platformDispatcher.setApplicationLocale(locale, view);
        _typeToResources = resources;
        _loadedDelegates = delegates;
        _locale = locale;
    }

    private void ReportLoadError(Locale locale, Exception error)
    {
        var details = new FlutterErrorDetails(error, library: "widgets library",
            context: new ErrorDescription($"while loading localization resources for {locale.toLanguageTag()}"));
        if (_locale is null) _initialLoadError = details;
        FlutterError.reportError(details);
    }

    private void CancelPendingLoad()
    {
        var request = _pendingLoad;
        _pendingLoad = null;
        request?.Cancel();
        request?.Dispose();
    }

    private void ReleaseFirstFrame()
    {
        var renderer = _deferredRenderer;
        _deferredRenderer = null;
        renderer?.allowFirstFrame();
    }

    public override void dispose()
    {
        CancelPendingLoad();
        ReleaseFirstFrame();
        base.dispose();
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
            return _initialLoadError is { } error ? ErrorWidget.builder(error) : SizedBox.CreateShrink();
        }
        return new Semantics(
            localeForSubtree: _locale,
            container: !widget.isApplicationLevel,
            textDirection: _textDirection,
            child: new _LocalizationsScope__localizations(
                key: _localizedResourcesScopeKey,
                locale: (
                    _locale
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                ),
                delegates: _loadedDelegates,
                typeToResources: _typeToResources,
                child: new Directionality(textDirection: _textDirection, child: widget.child!)
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class LocalizationsResolver : ChangeNotifier, WidgetsBindingObserver
{
    private readonly WidgetsBinding _binding;
    private DorotiView? _view;
    private IReadOnlyList<Locale> _normalizedSupportedLocales;
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
    ) : this(null, supportedLocales, locale, localeListResolutionCallback,
        localeResolutionCallback, localizationsDelegates) { }

    internal LocalizationsResolver(
        DorotiView? view,
        IEnumerable<Locale> supportedLocales,
        Locale? locale = null,
        Func<List<Locale>?, IEnumerable<Locale>, Locale?>? localeListResolutionCallback = null,
        Func<Locale?, IEnumerable<Locale>, Locale?>? localeResolutionCallback = null,
        IEnumerable<dynamic>? localizationsDelegates = null
    )
    {
        _binding = WidgetsBinding.instance;
        _view = view;
        _locale = locale;
        _localeListResolutionCallback = localeListResolutionCallback;
        _localeResolutionCallback = localeResolutionCallback;
        _localizationsDelegates = localizationsDelegates;
        _supportedLocales = supportedLocales;
        _normalizedSupportedLocales = supportedLocales.Select(value => value.normalized()).Distinct().ToArray();
        _resolvedLocale = _resolveLocales(PreferredLocales(), _normalizedSupportedLocales);
        _binding.addObserver(this);
    }

    public override void dispose()
    {
        _binding.removeObserver(this);
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
        var supportedChanged = !ReferenceEquals(_supportedLocales, supportedLocales);
        var resolutionChanged = supportedChanged || _localeListResolutionCallback != localeListResolutionCallback
            || _localeResolutionCallback != localeResolutionCallback;
        _locale = locale;
        _localeListResolutionCallback = localeListResolutionCallback;
        _localeResolutionCallback = localeResolutionCallback;
        _localizationsDelegates = localizationsDelegates;
        if (supportedChanged)
        {
            _supportedLocales = supportedLocales;
            _normalizedSupportedLocales = supportedLocales.Select(value => value.normalized()).Distinct().ToArray();
        }
        if (resolutionChanged) _updateResolvedLocale(PreferredLocales());
    }

    public virtual Locale locale
    {
        get
        {
            Locale appLocale =
                (_locale is not null)
                    ? _resolveLocales(
                        new List<Locale>
                        {
                            (
                                _locale
                                ?? throw new global::System.NullReferenceException(
                                    "A required value was null."
                                )
                            ),
                        },
                        supportedLocales.Cast<Locale>()
                    )
                    : (
                        _resolvedLocale
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    );
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
        _normalizedSupportedLocales;

    private List<Locale> PreferredLocales() =>
        (_view?.environmentConfiguration?.locales ?? _binding.platformDispatcher.locales).ToList();

    internal void UpdateView(DorotiView? view)
    {
        if (ReferenceEquals(_view, view)) return;
        _view = view;
        _updateResolvedLocale(PreferredLocales());
    }

    public virtual void didChangeLocales(List<Locale>? locales)
    {
        if (_view is not null && _binding.platformDispatcher.CurrentInvocationView is { } sender
            && !ReferenceEquals(_view, sender)) return;
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
        preferredLocales = preferredLocales?.Select(value => value.normalized()).Distinct().ToList();
        if (localeListResolutionCallback is not null)
        {
            Locale? locale = localeListResolutionCallback!(preferredLocales, supportedLocales);
            if (locale is not null)
            {
                Locale locale__32547__value32633 = (
                    locale
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                );
                return locale__32547__value32633.normalized();
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
                Locale locale__32838__value33016 = (
                    localeLocal
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                );
                return locale__32838__value33016.normalized();
            }
        }
        return AppLibrary.basicLocaleListResolution(
            preferredLocales,
            supportedLocales.Cast<Locale>()
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
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
                    exception: $"Warning: This application's locale, {(locale)}, is not supported by all of its localization delegates.",
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
            throw new InvalidOperationException("Callback completed without returning a value.");
        });
        return true;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}
