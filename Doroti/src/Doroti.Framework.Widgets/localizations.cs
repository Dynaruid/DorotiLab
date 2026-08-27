// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/localizations.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Widgets;

internal class _Pending__localizations
{
    public virtual dynamic @delegate { get; private set; } = default!;
    public virtual Future<object> futureValue { get; private set; } = default!;

    internal _Pending__localizations(dynamic @delegate, Future<object> futureValue)
    {
        this.@delegate = @delegate;
        this.futureValue = futureValue;
    }

}

public static partial class LocalizationsLibrary
{
    internal static Future<DartMap<Type, object>> _loadAll(Locale locale, IEnumerable<dynamic> allDelegates)
    {
        var output = new DartMap<Type, object>();
        List<_Pending__localizations>? pendingList = default!;
        var types = new HashSet<Type>();
        var delegates = new List<object>();
        foreach (var delegateLocal in allDelegates)
        {
            if ((!types.Contains(((Type)((dynamic)delegateLocal).type)) && ((bool)((dynamic)delegateLocal).isSupported(locale))))
            {
                types.Add(((Type)((dynamic)delegateLocal).type));
                delegates.Add(delegateLocal);
            }
        }
        foreach (var delegateAlternate in delegates)
        {
            Future inputValue = (Future)((dynamic)delegateAlternate).load(locale);
            dynamic completedValue = default!;
            Future<object> futureValueLocal = inputValue.then<object>((object? value) =>
            {
                return completedValue = value;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
            if ((completedValue is not null))
            {
                Type typeLocal = ((Type)((dynamic)delegateAlternate).type);
                DartRuntimePrimitives.Assert(() => !output.ContainsKey(typeLocal));
                output[typeLocal] = completedValue;
            }
            else
            {
                pendingList ??= new List<_Pending__localizations>();
                pendingList.Add(new _Pending__localizations(delegateAlternate, futureValueLocal));
            }
        }
        if ((pendingList is null))
        {
            return ((Future<DartMap<Type, object>>)(object?)new global::Doroti.Framework.Foundation.SynchronousFuture<DartMap<Type, object>>(output));
        }
        return global::Doroti.Runtime.DartAsyncRuntime.wait<object>(pendingList.map<_Pending__localizations, Future>(((p) => ((_Pending__localizations)p).futureValue))).then((global::System.Func<List<object>, DartMap<Type, object>>)((values) =>
        {
            DartRuntimePrimitives.Assert(() => (checked((long)(values.Count)) == checked((long)(pendingList!.Count))));
            for (var i = 0L; (i < checked((long)(values.Count))); i += 1L)
            {
                Type typeAlternate = ((Type)((dynamic)pendingList![(int)(i)].@delegate).type);
                DartRuntimePrimitives.Assert(() => !output.ContainsKey(typeAlternate));
                output[typeAlternate] = values[(int)(i)];
            }
            return output;
            throw new InvalidOperationException("Dart closure completed without a value.");
        }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public abstract class LocalizationsDelegate<T>
{
    protected LocalizationsDelegate()
    {
    }

    public abstract bool isSupported(Locale locale);
    public abstract Future<T> load(Locale locale);
    public abstract bool shouldReload(LocalizationsDelegate<T> old);
    public virtual Type type => typeof(T);
    public override string ToString() => $"{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "LocalizationsDelegate"))}[{this.type}]";
}

public abstract class WidgetsLocalizations
{
    public WidgetsLocalizations() { }

    public abstract global::Doroti.Ui.TextDirection textDirection { get; }
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
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Widgets.DebugLibrary.debugCheckHasWidgetsLocalizations(context));
        return Localizations.of<WidgetsLocalizations>(context, typeof(WidgetsLocalizations))!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _WidgetsLocalizationsDelegate__localizations : LocalizationsDelegate<WidgetsLocalizations>
{
    internal _WidgetsLocalizationsDelegate__localizations()
    {
    }

    public override bool isSupported(Locale locale) => true;
    public override Future<WidgetsLocalizations> load(Locale locale) => DefaultWidgetsLocalizations.load(locale);
    public override bool shouldReload(LocalizationsDelegate<WidgetsLocalizations> old) => false;
    public override string ToString() => "DefaultWidgetsLocalizations.delegate(en_US)";
}

public class DefaultWidgetsLocalizations : WidgetsLocalizations
{
    public static LocalizationsDelegate<WidgetsLocalizations> @delegate = ((LocalizationsDelegate<WidgetsLocalizations>)(object?)new _WidgetsLocalizationsDelegate__localizations());

    public DefaultWidgetsLocalizations()
    {
    }

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
        return ((Future<WidgetsLocalizations>)(object?)new global::Doroti.Framework.Foundation.SynchronousFuture<WidgetsLocalizations>(new DefaultWidgetsLocalizations()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _LocalizationsScope__localizations : InheritedWidget
{
    public virtual Locale locale { get; private set; } = default!;
    public virtual _LocalizationsState__localizations localizationsState { get; private set; } = default!;
    public virtual DartMap<Type, object> typeToResources { get; private set; } = default!;

    internal _LocalizationsScope__localizations(global::Doroti.Framework.Foundation.Key? key = null, Locale locale = default!, _LocalizationsState__localizations localizationsState = default!, DartMap<Type, object> typeToResources = default!, Widget child = default!) : base(key: key, child: child)
    {
        this.locale = locale;
        this.localizationsState = localizationsState;
        this.typeToResources = typeToResources;
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget)
    {
        var __old = (_LocalizationsScope__localizations)(object)oldWidget;
        return (!object.Equals(this.typeToResources, ((_LocalizationsScope__localizations)__old).typeToResources));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class Localizations : StatefulWidget
{
    public virtual Locale locale { get; private set; } = default!;
    public virtual List<dynamic> delegates { get; private set; } = default!;
    public virtual Widget? child { get; private set; }
    public virtual bool isApplicationLevel { get; private set; } = default!;

    public Localizations(global::Doroti.Framework.Foundation.Key? key = null, Locale locale = default!, List<dynamic> delegates = default!, Widget? child = null, bool isApplicationLevel = false) : base(key: key)
    {
        this.locale = locale;
        this.delegates = delegates;
        this.child = child;
        this.isApplicationLevel = isApplicationLevel;
        System.Diagnostics.Debug.Assert(delegates.any(((@delegate) => (@delegate is LocalizationsDelegate<WidgetsLocalizations>))));
    }

    public static Localizations CreateOverride(global::Doroti.Framework.Foundation.Key? key = null, BuildContext context = default!, Locale? locale = null, List<dynamic>? delegates = null, Widget? child = null)
    {
        List<object> mergedDelegates = ((List<object>)(object?)Localizations._delegatesOf(context));
        if ((delegates is not null))
        {
            mergedDelegates.InsertRange(checked((int)0L), delegates.Cast<dynamic>());
        }
        return new Localizations(key: key, locale: ((locale ?? (Locale)Localizations.localeOf(context))), delegates: mergedDelegates, child: child);
    }

    public static global::Doroti.Ui.Locale localeOf(BuildContext context)
    {
        _LocalizationsScope__localizations? scope = ((_LocalizationsScope__localizations?)(object?)context.dependOnInheritedWidgetOfExactType<_LocalizationsScope__localizations>());
        DartRuntimePrimitives.Assert(() =>
            {
                if ((scope is null))
                {
                    throw DartRuntimePrimitives.AsException(global::Doroti.Framework.Foundation.FlutterError.Create("Requested the Locale of a context that does not include a Localizations ancestor.\n" + "To request the Locale, the context used to retrieve the Localizations widget must " + "be that of a widget that is a descendant of a Localizations widget."));
                }
                if ((((_LocalizationsScope__localizations)scope).localizationsState.locale is null))
                {
                    throw DartRuntimePrimitives.AsException(global::Doroti.Framework.Foundation.FlutterError.Create("Localizations.localeOf found a Localizations widget that had a unexpected null locale.\n"));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return DartRuntimePrimitives.RequireValue(scope!.localizationsState.locale);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static global::Doroti.Ui.Locale? maybeLocaleOf(BuildContext context)
    {
        _LocalizationsScope__localizations? scope = ((_LocalizationsScope__localizations?)(object?)context.dependOnInheritedWidgetOfExactType<_LocalizationsScope__localizations>());
        return scope?.localizationsState.locale;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static List<object> _delegatesOf(BuildContext context)
    {
        _LocalizationsScope__localizations? scope = ((_LocalizationsScope__localizations?)(object?)context.dependOnInheritedWidgetOfExactType<_LocalizationsScope__localizations>());
        DartRuntimePrimitives.Assert(() => (scope is not null), () => (object?)"a Localizations ancestor was not found");
        return ((List<object>)(object?)new List<object>(DartRuntimePrimitives.ConvertEnumerable<object>(scope!.localizationsState.widget.delegates)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static T? of<T>(BuildContext context, Type type)
    {
        _LocalizationsScope__localizations? scope = ((_LocalizationsScope__localizations?)(object?)context.dependOnInheritedWidgetOfExactType<_LocalizationsScope__localizations>());
        return scope is null ? default : scope.localizationsState.resourcesFor<T>(type);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _LocalizationsState__localizations());
    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.Locale>("locale", this.locale));
        properties.add(new global::Doroti.Framework.Foundation.IterableProperty<object>("delegates", this.delegates.Cast<object>()));
    }

}

internal class _LocalizationsState__localizations : State<Localizations>
{
    internal virtual GlobalKey<IState> _localizedResourcesScopeKey { get; private set; } = GlobalKey<IState>.Create();
    internal virtual DartMap<Type, object> _typeToResources { get; set; } = new DartMap<Type, object>();
    internal virtual Locale? _locale { get; set; } = default;

    public virtual global::Doroti.Ui.Locale? locale
    {
        get => this._locale;
        set
        {
            var locale = value;
            DartRuntimePrimitives.Assert(() => (locale is not null));
            if ((object.Equals(this._locale, locale)))
            {
                return;
            }
            WidgetsBinding.instance.platformDispatcher.setApplicationLocale(DartRuntimePrimitives.RequireValue(locale));
            _locale = DartRuntimePrimitives.RequireValue(locale);
        }
    }
    public override void initState()
    {
        base.initState();
        load(DartRuntimePrimitives.RequireValue(((Localizations)(object)this.widget).locale));
    }

    internal virtual bool _anyDelegatesShouldReload(Localizations old)
    {
        if ((checked((long)(((Localizations)(object)this.widget).delegates.Count)) != checked((long)(((Localizations)old).delegates.Count))))
        {
            return true;
        }
        List<object> delegatesLocal = ((Localizations)(object)this.widget).delegates.ToList().Cast<object>().ToList();
        List<object> oldDelegates = ((Localizations)old).delegates.ToList().Cast<object>().ToList();
        for (var i = 0L; (i < checked((long)(delegatesLocal.Count))); i += 1L)
        {
            dynamic @delegate = delegatesLocal[(int)(i)];
            dynamic oldDelegate = oldDelegates[(int)(i)];
            if (((!object.Equals(DartRuntimePrimitives.RuntimeType(@delegate), DartRuntimePrimitives.RuntimeType(oldDelegate))) || ((bool)((dynamic)@delegate).shouldReload(oldDelegate))))
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
        if (((!object.Equals(((Localizations)(object)this.widget).locale, ((Localizations)old).locale)) || (_anyDelegatesShouldReload(old))))
        {
            load(DartRuntimePrimitives.RequireValue(((Localizations)(object)this.widget).locale));
        }
    }

    public virtual void load(Locale locale)
    {
        IEnumerable<object> delegatesLocal = ((IEnumerable<object>)(object?)((Localizations)(object)this.widget).delegates);
        if (!System.Linq.Enumerable.Any(delegatesLocal))
        {
            this.locale = DartRuntimePrimitives.RequireValue(locale);
            return;
        }
        DartMap<Type, object>? typeToResources = default!;
        Future<DartMap<Type, object>> typeToResourcesFuture = LocalizationsLibrary._loadAll(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(locale)), delegatesLocal.Cast<dynamic>()).then((global::System.Func<DartMap<Type, object>, DartMap<Type, object>>)((value) =>
        {
            return typeToResources = value.cast<Type, object>();
            throw new InvalidOperationException("Dart closure completed without a value.");
        }));
        if ((typeToResources is not null))
        {
            _typeToResources = typeToResources!;
            this.locale = DartRuntimePrimitives.RequireValue(locale);
        }
        else
        {
            global::Doroti.Framework.Rendering.RendererBinding.instance.deferFirstFrame();
            DartRuntimePrimitives.Ignore(typeToResourcesFuture.then((global::System.Action<DartMap<Type, object>>)((value) =>
            {
                if (this.mounted)
                {
                    setState(((global::System.Action)(() =>
                    {
                        _typeToResources = value;
                        this.locale = DartRuntimePrimitives.RequireValue(locale);
                    })));
                }
                global::Doroti.Framework.Rendering.RendererBinding.instance.allowFirstFrame();
            })));
        }
    }

    public virtual T resourcesFor<T>(Type type)
    {
        var resources = ((T?)(object?)this._typeToResources.GetValueOrDefault(type))!;
        return resources;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.TextDirection _textDirection
    {
        get
        {
            var resources = ((WidgetsLocalizations?)(object?)this._typeToResources.GetValueOrDefault(typeof(WidgetsLocalizations)))!;
            return ((WidgetsLocalizations)resources).textDirection;
            return default!;
        }
    }
    public override Widget build(BuildContext context)
    {
        if ((this._locale is null))
        {
            return ((Widget)(object?)SizedBox.CreateShrink());
        }
        return ((Widget)(object?)new Semantics(localeForSubtree: (((Localizations)(object)this.widget).isApplicationLevel ? null : ((Localizations)(object)this.widget).locale), container: !((Localizations)(object)this.widget).isApplicationLevel, textDirection: this._textDirection, child: new _LocalizationsScope__localizations(key: this._localizedResourcesScopeKey, locale: DartRuntimePrimitives.RequireValue(this._locale), localizationsState: this, typeToResources: this._typeToResources, child: new Directionality(textDirection: this._textDirection, child: ((Localizations)(object)this.widget).child!))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class LocalizationsResolver : global::Doroti.Framework.Foundation.ChangeNotifier, WidgetsBindingObserver
{
    internal virtual IEnumerable<dynamic>? _localizationsDelegates { get; set; } = default;
    internal virtual global::System.Func<List<Locale>?, IEnumerable<Locale>, Locale?>? _localeListResolutionCallback { get; set; } = default;
    internal virtual global::System.Func<Locale?, IEnumerable<Locale>, Locale?>? _localeResolutionCallback { get; set; } = default;
    internal virtual IEnumerable<Locale> _supportedLocales { get; set; } = default!;
    internal virtual Locale? _locale { get; set; } = default;
    internal virtual Locale? _resolvedLocale { get; set; } = default;

    public LocalizationsResolver(IEnumerable<Locale> supportedLocales, Locale? locale = null, global::System.Func<List<Locale>?, IEnumerable<Locale>, Locale?>? localeListResolutionCallback = null, global::System.Func<Locale?, IEnumerable<Locale>, Locale?>? localeResolutionCallback = null, IEnumerable<dynamic>? localizationsDelegates = null)
    {
        this._locale = locale;
        this._localeListResolutionCallback = localeListResolutionCallback;
        this._localeResolutionCallback = localeResolutionCallback;
        this._localizationsDelegates = localizationsDelegates;
        this._supportedLocales = supportedLocales;
    }

    public virtual void dispose()
    {
        WidgetsBinding.instance.removeObserver(this);
        base.dispose();
    }

    public virtual void update(Locale? locale, global::System.Func<List<Locale>?, IEnumerable<Locale>, Locale?>? localeListResolutionCallback, global::System.Func<Locale?, IEnumerable<Locale>, Locale?>? localeResolutionCallback, IEnumerable<dynamic>? localizationsDelegates, IEnumerable<Locale> supportedLocales)
    {
        _locale = locale;
        _localeListResolutionCallback = (global::System.Func<List<Locale>?, IEnumerable<Locale>, Locale?>)localeListResolutionCallback;
        _localeResolutionCallback = (global::System.Func<Locale?, IEnumerable<Locale>, Locale?>)localeResolutionCallback;
        _localizationsDelegates = localizationsDelegates;
        if ((!object.Equals(this._supportedLocales, supportedLocales)))
        {
            _supportedLocales = supportedLocales;
            _updateResolvedLocale(WidgetsBinding.instance.platformDispatcher.locales.ToList());
        }
    }

    public virtual global::Doroti.Ui.Locale locale
    {
        get
        {
            global::Doroti.Ui.Locale appLocale = ((this._locale is not null) ? _resolveLocales(new List<global::Doroti.Ui.Locale> { DartRuntimePrimitives.RequireValue(this._locale) }, this.supportedLocales.Cast<Locale>()) : DartRuntimePrimitives.RequireValue(this._resolvedLocale));
            DartRuntimePrimitives.Assert(() => _debugCheckLocalizations(appLocale));
            return appLocale;
            return default!;
        }
    }
    public virtual IEnumerable<object> localizationsDelegates
    {
        get
        {
            var delegates = new List<object>();
            if (this._localizationsDelegates is not null)
            {
                delegates.AddRange(this._localizationsDelegates.Cast<object>());
            }
            delegates.Add(DefaultWidgetsLocalizations.@delegate);
            return delegates;
            return default!;
        }
    }
    public virtual global::System.Func<List<Locale>?, IEnumerable<Locale>, Locale?>? localeListResolutionCallback => this._localeListResolutionCallback;
    public virtual global::System.Func<Locale?, IEnumerable<Locale>, Locale?>? localeResolutionCallback => this._localeResolutionCallback;
    public virtual IEnumerable<global::Doroti.Ui.Locale> supportedLocales => DartRuntimePrimitives.ConvertValue<IEnumerable<global::Doroti.Ui.Locale>>(this._supportedLocales);
    public virtual void didChangeLocales(List<Locale>? locales)
    {
        _updateResolvedLocale(locales.ToList());
    }

    internal virtual void _updateResolvedLocale(List<Locale>? preferredLocales)
    {
        global::Doroti.Ui.Locale newLocale = _resolveLocales(preferredLocales, this.supportedLocales.Cast<Locale>());
        if ((!object.Equals(newLocale, this._resolvedLocale)))
        {
            _resolvedLocale = newLocale;
            notifyListeners();
        }
    }

    internal virtual global::Doroti.Ui.Locale _resolveLocales(List<Locale>? preferredLocales, IEnumerable<Locale> supportedLocales)
    {
        if ((this.localeListResolutionCallback is not null))
        {
            global::Doroti.Ui.Locale? locale = this.localeListResolutionCallback!(preferredLocales, supportedLocales);
            if ((locale is not null))
            {
                Locale locale__32547__value32633 = DartRuntimePrimitives.RequireValue(locale);
                return DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(locale__32547__value32633));
            }
        }
        if ((this.localeResolutionCallback is not null))
        {
            global::Doroti.Ui.Locale? localeLocal = this.localeResolutionCallback!((((preferredLocales is not null) && System.Linq.Enumerable.Any(preferredLocales)) ? preferredLocales.First() : null), supportedLocales);
            if ((localeLocal is not null))
            {
                Locale locale__32838__value33016 = DartRuntimePrimitives.RequireValue(localeLocal);
                return DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(locale__32838__value33016));
            }
        }
        return global::Doroti.Framework.Widgets.AppLibrary.basicLocaleListResolution(preferredLocales, supportedLocales.Cast<Locale>());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString() => $"{typeof(LocalizationsResolver)}";
    internal virtual bool _debugCheckLocalizations(Locale locale)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                HashSet<Type> unsupportedTypes = this.localizationsDelegates.map<dynamic, Type>(((@delegate) => ((Type)((dynamic)@delegate).type))).toSet();
                foreach (dynamic delegateLocal in this.localizationsDelegates)
                {
                    if (!unsupportedTypes.Contains(((Type)((dynamic)delegateLocal).type)))
                    {
                        continue;
                    }
                    if (((bool)((dynamic)delegateLocal).isSupported(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(locale)))))
                    {
                        unsupportedTypes.Remove(((Type)((dynamic)delegateLocal).type));
                    }
                }
                if (!System.Linq.Enumerable.Any(unsupportedTypes))
                {
                    return true;
                }
                FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: $"Warning: This application's locale, {DartRuntimePrimitives.RequireValue(locale)}, is not supported by all of its localization delegates.", library: "widgets", informationCollector: ((InformationCollector)(() => new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSpacer(), new global::Doroti.Framework.Foundation.ErrorHint($"The declared supported locales for this app are: {string.Join(", ", this.supportedLocales)}"), new global::Doroti.Framework.Foundation.ErrorSpacer(), new global::Doroti.Framework.Foundation.ErrorDescription("See https://flutter.dev/to/internationalization/ for more " + "information about configuring an app's locale, supportedLocales, " + "and localizationsDelegates parameters.") }))));
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
