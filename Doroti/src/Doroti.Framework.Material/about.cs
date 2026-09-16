// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/about.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class AboutListTile : StatelessWidget
{
    public virtual Widget? icon { get; private set; }
    public virtual Widget? child { get; private set; }
    public virtual string? applicationName { get; private set; }
    public virtual string? applicationVersion { get; private set; }
    public virtual Widget? applicationIcon { get; private set; }
    public virtual string? applicationLegalese { get; private set; }
    public virtual List<Widget>? aboutBoxChildren { get; private set; }
    public virtual bool? dense { get; private set; }

    public AboutListTile(Key? key = null, Widget? icon = null, Widget? child = null, string? applicationName = null, string? applicationVersion = null, Widget? applicationIcon = null, string? applicationLegalese = null, List<Widget>? aboutBoxChildren = null, bool? dense = null) : base(key: key)
    {
        this.icon = icon;
        this.child = child;
        this.applicationName = applicationName;
        this.applicationVersion = applicationVersion;
        this.applicationIcon = applicationIcon;
        this.applicationLegalese = applicationLegalese;
        this.aboutBoxChildren = aboutBoxChildren;
        this.dense = dense;
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterial(context));
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterialLocalizations(context));
        return new ListTile(leading: icon, title: child ?? new Text(MaterialLocalizations.of(context).aboutListTileTitle(applicationName ?? AboutLibrary._defaultApplicationName(context))), dense: dense, onTap: () =>
        {
            AboutLibrary.showAboutDialog(context: context, applicationName: applicationName, applicationVersion: applicationVersion, applicationIcon: applicationIcon, applicationLegalese: applicationLegalese, children: aboutBoxChildren);
        });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public static partial class AboutLibrary
{
    public static void showAboutDialog(BuildContext context, string? applicationName = null, string? applicationVersion = null, Widget? applicationIcon = null, string? applicationLegalese = null, List<Widget>? children = null, bool barrierDismissible = true, Color? barrierColor = null, string? barrierLabel = null, bool useRootNavigator = true, RouteSettings? routeSettings = null, Offset? anchorPoint = null)
    {
        DartRuntimePrimitives.Ignore(DialogLibrary.showDialog<object?>(context: context, barrierDismissible: barrierDismissible, barrierColor: barrierColor, barrierLabel: barrierLabel, useRootNavigator: useRootNavigator, builder: (context) =>
        {
            return new AboutDialog(applicationName: applicationName, applicationVersion: applicationVersion, applicationIcon: applicationIcon, applicationLegalese: applicationLegalese, children: children);
            throw new InvalidOperationException("Dart closure completed without a value.");
        }, routeSettings: routeSettings, anchorPoint: DartRuntimePrimitives.RequireValue(anchorPoint)));
    }
}

public static partial class AboutLibrary
{
    public static void showAdaptiveAboutDialog(BuildContext context, string? applicationName = null, string? applicationVersion = null, Widget? applicationIcon = null, string? applicationLegalese = null, List<Widget>? children = null, bool barrierDismissible = true, Color? barrierColor = null, string? barrierLabel = null, bool useRootNavigator = true, RouteSettings? routeSettings = null, Offset? anchorPoint = null)
    {
        DartRuntimePrimitives.Ignore(DialogLibrary.showAdaptiveDialog<object?>(context: context, barrierDismissible: barrierDismissible, barrierColor: barrierColor, barrierLabel: barrierLabel, useRootNavigator: useRootNavigator, builder: (context) =>
        {
            return AboutDialog.CreateAdaptive(applicationName: applicationName, applicationVersion: applicationVersion, applicationIcon: applicationIcon, applicationLegalese: applicationLegalese, children: children);
            throw new InvalidOperationException("Dart closure completed without a value.");
        }, routeSettings: routeSettings, anchorPoint: anchorPoint));
    }
}

public static partial class AboutLibrary
{
    public static void showLicensePage(BuildContext context, string? applicationName = null, string? applicationVersion = null, Widget? applicationIcon = null, string? applicationLegalese = null, bool useRootNavigator = false)
    {
        CapturedThemes themes = InheritedTheme.capture(from: context, to: Navigator.of(context, rootNavigator: useRootNavigator).context);
        DartRuntimePrimitives.Ignore(Navigator.of(context, rootNavigator: useRootNavigator).push(new MaterialPageRoute<object?>(builder: (context) => themes.wrap(new LicensePage(applicationName: applicationName, applicationVersion: applicationVersion, applicationIcon: applicationIcon, applicationLegalese: applicationLegalese)))));
    }
}

public static partial class AboutLibrary
{
    internal static double _textVerticalSeparation = 18.0;
}

public class AboutDialog : StatelessWidget
{
    public virtual string? applicationName { get; private set; }
    public virtual string? applicationVersion { get; private set; }
    public virtual Widget? applicationIcon { get; private set; }
    public virtual string? applicationLegalese { get; private set; }
    public virtual List<Widget>? children { get; private set; }

    public AboutDialog(Key? key = null, string? applicationName = null, string? applicationVersion = null, Widget? applicationIcon = null, string? applicationLegalese = null, List<Widget>? children = null) : base(key: key)
    {
        this.applicationName = applicationName;
        this.applicationVersion = applicationVersion;
        this.applicationIcon = applicationIcon;
        this.applicationLegalese = applicationLegalese;
        this.children = children;
    }

    public static AboutDialog CreateAdaptive(Key? key = null, string? applicationName = null, string? applicationVersion = null, Widget? applicationIcon = null, string? applicationLegalese = null, List<Widget>? children = null)
        => new _AdaptiveAboutDialog__about(key, applicationName, applicationVersion, applicationIcon, applicationLegalese, children);

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterialLocalizations(context));
        string name = applicationName ?? AboutLibrary._defaultApplicationName(context);
        string version = applicationVersion ?? AboutLibrary._defaultApplicationVersion(context);
        Widget? icon = applicationIcon ?? AboutLibrary._defaultApplicationIcon(context);
        ThemeData themeData = Theme.of(context);
        MaterialLocalizations localizations = MaterialLocalizations.of(context);
        return new AlertDialog(content: new ListBody(children: ((Func<List<Widget>>)(() => { var __collection14686 = new List<Widget>(); __collection14686.Add(DartRuntimePrimitives.ConvertValue<Widget>(new Row(crossAxisAlignment: CrossAxisAlignment.start, children: ((Func<List<Widget>>)(() => { var __collection14791 = new List<Widget>(); if (icon is not null) { __collection14791.Add(DartRuntimePrimitives.ConvertValue<Widget>(new IconTheme(data: themeData.iconTheme, child: icon))); } __collection14791.Add(DartRuntimePrimitives.ConvertValue<Widget>(new Expanded(child: new Padding(padding: EdgeInsets.CreateSymmetric(horizontal: 24.0), child: new ListBody(children: new List<Widget> { DartRuntimePrimitives.ConvertValue<Widget>(new Text(name, style: themeData.textTheme.headlineSmall)), DartRuntimePrimitives.ConvertValue<Widget>(new Text(version, style: themeData.textTheme.bodyMedium)), DartRuntimePrimitives.ConvertValue<Widget>(new SizedBox(height: AboutLibrary._textVerticalSeparation)), DartRuntimePrimitives.ConvertValue<Widget>(new Text(applicationLegalese ?? "", style: themeData.textTheme.bodySmall)) }))))); return __collection14791; }))()))); var __collectionSpread15522 = children; if (__collectionSpread15522 is not null) { __collection14686.AddRange(__collectionSpread15522); } return __collection14686; }))()), actions: new List<Widget> { DartRuntimePrimitives.ConvertValue<Widget>(new TextButton(child: new Text(localizations.viewLicensesButtonLabel), onPressed: () => {
AboutLibrary.showLicensePage(context: context, applicationName: applicationName, applicationVersion: applicationVersion, applicationIcon: applicationIcon, applicationLegalese: applicationLegalese);
})), DartRuntimePrimitives.ConvertValue<Widget>(new TextButton(child: new Text(localizations.closeButtonLabel), onPressed: () => {
Navigator.pop<object>(context);
})) }, scrollable: true);
    }

}

internal class _AdaptiveAboutDialog__about : AboutDialog
{
    internal _AdaptiveAboutDialog__about(Key? key = null, string? applicationName = null, string? applicationVersion = null, Widget? applicationIcon = null, string? applicationLegalese = null, List<Widget>? children = null) : base(key: key, applicationName: applicationName, applicationVersion: applicationVersion, applicationIcon: applicationIcon, applicationLegalese: applicationLegalese, children: children)
    {
    }

    internal virtual List<Widget>? _actions(BuildContext context)
    {
        ThemeData themeData = Theme.of(context);
        MaterialLocalizations localizations = MaterialLocalizations.of(context);
        switch (themeData.platform)
        {
            case TargetPlatform.iOS:
            case TargetPlatform.macOS:
                {
                    return new List<Widget> { DartRuntimePrimitives.ConvertValue<Widget>(new CupertinoDialogAction(child: new Text(localizations.viewLicensesButtonLabel), onPressed: () => {
AboutLibrary.showLicensePage(context: context, applicationName: applicationName, applicationVersion: applicationVersion, applicationIcon: applicationIcon, applicationLegalese: applicationLegalese);
})), DartRuntimePrimitives.ConvertValue<Widget>(new CupertinoDialogAction(child: new Text(localizations.closeButtonLabel), onPressed: () => {
Navigator.pop<object>(context);
})) };
                }
            case TargetPlatform.android:
            case TargetPlatform.fuchsia:
            case TargetPlatform.linux:
            case TargetPlatform.windows:
                {
                    return new List<Widget> { DartRuntimePrimitives.ConvertValue<Widget>(new TextButton(child: new Text(localizations.viewLicensesButtonLabel), onPressed: () => {
AboutLibrary.showLicensePage(context: context, applicationName: applicationName, applicationVersion: applicationVersion, applicationIcon: applicationIcon, applicationLegalese: applicationLegalese);
})), DartRuntimePrimitives.ConvertValue<Widget>(new TextButton(child: new Text(localizations.closeButtonLabel), onPressed: () => {
Navigator.pop<object>(context);
})) };
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        base.build(context);
        string name = applicationName ?? AboutLibrary._defaultApplicationName(context);
        string version = applicationVersion ?? AboutLibrary._defaultApplicationVersion(context);
        Widget? icon = applicationIcon ?? AboutLibrary._defaultApplicationIcon(context);
        ThemeData themeData = Theme.of(context);
        List<Widget>? actionsLocal = _actions(context);
        return AlertDialog.CreateAdaptive(content: new ListBody(children: ((Func<List<Widget>>)(() => { var __collection19514 = new List<Widget>(); __collection19514.Add(DartRuntimePrimitives.ConvertValue<Widget>(new Row(crossAxisAlignment: CrossAxisAlignment.start, children: ((Func<List<Widget>>)(() => { var __collection19619 = new List<Widget>(); if (icon is not null) { __collection19619.Add(DartRuntimePrimitives.ConvertValue<Widget>(new IconTheme(data: themeData.iconTheme, child: icon))); } __collection19619.Add(DartRuntimePrimitives.ConvertValue<Widget>(new Expanded(child: new Padding(padding: EdgeInsets.CreateSymmetric(horizontal: 24.0), child: new ListBody(children: new List<Widget> { DartRuntimePrimitives.ConvertValue<Widget>(new Text(name, style: themeData.textTheme.headlineSmall)), DartRuntimePrimitives.ConvertValue<Widget>(new Text(version, style: themeData.textTheme.bodyMedium)), DartRuntimePrimitives.ConvertValue<Widget>(new SizedBox(height: AboutLibrary._textVerticalSeparation)), DartRuntimePrimitives.ConvertValue<Widget>(new Text(applicationLegalese ?? "", style: themeData.textTheme.bodySmall)) }))))); return __collection19619; }))()))); var __collectionSpread20350 = children; if (__collectionSpread20350 is not null) { __collection19514.AddRange(__collectionSpread20350); } return __collection19514; }))()), actions: actionsLocal, scrollable: true);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class LicensePage : StatefulWidget
{
    public virtual string? applicationName { get; private set; }
    public virtual string? applicationVersion { get; private set; }
    public virtual Widget? applicationIcon { get; private set; }
    public virtual string? applicationLegalese { get; private set; }

    public LicensePage(Key? key = null, string? applicationName = null, string? applicationVersion = null, Widget? applicationIcon = null, string? applicationLegalese = null) : base(key: key)
    {
        this.applicationName = applicationName;
        this.applicationVersion = applicationVersion;
        this.applicationIcon = applicationIcon;
        this.applicationLegalese = applicationLegalese;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _LicensePageState__about());
}

internal class _LicensePageState__about : State<LicensePage>
{
    public virtual ValueNotifier<long?> selectedId { get; private set; } = new ValueNotifier<long?>(null);

    public override void dispose()
    {
        selectedId.dispose();
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        return new _MasterDetailFlow__about(detailPageFABlessGutterWidth: AboutLibrary._getGutterSize(context), title: new Text(MaterialLocalizations.of(context).licensesPageTitle), detailPageBuilder: _packageLicensePage, masterViewBuilder: _packagesView);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Widget _packageLicensePage(BuildContext __unused0, object? args, ScrollController? scrollController)
    {
        DartRuntimePrimitives.Assert(() => args is _DetailArguments__about);
        var detailArguments = ((_DetailArguments__about?)args!)!;
        return new _PackageLicensePage__about(packageName: detailArguments.packageName, licenseEntries: detailArguments.licenseEntries, scrollController: scrollController);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Widget _packagesView(BuildContext __unused0, bool isLateral)
    {
        Widget aboutLocal = new _AboutProgram__about(name: widget.applicationName ?? AboutLibrary._defaultApplicationName(context), icon: widget.applicationIcon ?? AboutLibrary._defaultApplicationIcon(context), version: widget.applicationVersion ?? AboutLibrary._defaultApplicationVersion(context), legalese: widget.applicationLegalese);
        return new _PackagesView__about(about: aboutLocal, isLateral: isLateral, selectedId: selectedId);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _AboutProgram__about : StatelessWidget
{
    public virtual string name { get; private set; } = default!;
    public virtual string version { get; private set; } = default!;
    public virtual Widget? icon { get; private set; }
    public virtual string? legalese { get; private set; }

    internal _AboutProgram__about(string name, string version, Widget? icon = null, string? legalese = null)
    {
        this.name = name;
        this.version = version;
        this.icon = icon;
        this.legalese = legalese;
    }

    public override Widget build(BuildContext context)
    {
        return new Padding(padding: EdgeInsets.CreateSymmetric(horizontal: AboutLibrary._getGutterSize(context), vertical: 24.0), child: new Column(children: ((Func<List<Widget>>)(() => { var __collection24177 = new List<Widget>(); __collection24177.Add(DartRuntimePrimitives.ConvertValue<Widget>(new Text(name, style: Theme.of(context).textTheme.headlineSmall, textAlign: TextAlign.center))); if (icon is not null) { __collection24177.Add(DartRuntimePrimitives.ConvertValue<Widget>(new IconTheme(data: Theme.of(context).iconTheme, child: icon!))); } if (version != "") { __collection24177.Add(DartRuntimePrimitives.ConvertValue<Widget>(new Padding(padding: EdgeInsets.CreateOnly(bottom: AboutLibrary._textVerticalSeparation), child: new Text(version, style: Theme.of(context).textTheme.bodyMedium, textAlign: TextAlign.center)))); } if ((legalese is not null) && (legalese != "")) { __collection24177.Add(DartRuntimePrimitives.ConvertValue<Widget>(new Text(legalese!, style: Theme.of(context).textTheme.bodySmall, textAlign: TextAlign.center))); } __collection24177.Add(DartRuntimePrimitives.ConvertValue<Widget>(new SizedBox(height: AboutLibrary._textVerticalSeparation))); __collection24177.Add(DartRuntimePrimitives.ConvertValue<Widget>(new Text("Powered by Flutter", style: Theme.of(context).textTheme.bodyMedium, textAlign: TextAlign.center))); return __collection24177; }))()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _PackagesView__about : StatefulWidget
{
    public virtual Widget about { get; private set; } = default!;
    public virtual bool isLateral { get; private set; } = default!;
    public virtual ValueNotifier<long?> selectedId { get; private set; } = default!;

    internal _PackagesView__about(Widget about, bool isLateral, ValueNotifier<long?> selectedId)
    {
        this.about = about;
        this.isLateral = isLateral;
        this.selectedId = selectedId;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _PackagesViewState__about());
}

internal class _PackagesViewState__about : State<_PackagesView__about>
{
    public virtual Future<_LicenseData__about> licenses { get; private set; } = Future<_LicenseData__about>.value(new _LicenseData__about());

    public override Widget build(BuildContext context)
    {
        return new FutureBuilder<_LicenseData__about>(future: licenses, builder: (context, snapshot) =>
        {
            return new LayoutBuilder(key: new ValueKey<ConnectionState>(snapshot.connectionState), builder: (context, constraints) =>
            {
                switch (snapshot.connectionState)
                {
                    case ConnectionState.done:
                        {
                            if (snapshot.hasError)
                            {
                                DartRuntimePrimitives.Assert(() =>
                                    {
                                        FlutterError.reportError(new FlutterErrorDetails(exception: snapshot.error!, stack: snapshot.stackTrace, context: new ErrorDescription("while decoding the license file")));
                                        return true;
                                    });
                                return new Center(child: new Text(snapshot.error!.ToString()!));
                            }
                            _initDefaultDetailPage(snapshot.data!, context);
                            return new ValueListenableBuilder<long?>(valueListenable: widget.selectedId, builder: (context, selectedId, _) =>
                            {
                                return new Center(child: new Material(color: Theme.of(context).cardColor, elevation: 4.0, child: new ConstrainedBox(constraints: new BoxConstraints(maxWidth: 600.0), child: _packagesList(context, selectedId, snapshot.data!, widget.isLateral))));
                                throw new InvalidOperationException("Dart closure completed without a value.");
                            });
                        }
                    case ConnectionState.none:
                    case ConnectionState.active:
                    case ConnectionState.waiting:
                        {
                            return new Material(color: Theme.of(context).cardColor, child: new Column(children: new List<Widget> { DartRuntimePrimitives.ConvertValue<Widget>(widget.about), DartRuntimePrimitives.ConvertValue<Widget>(new Center(child: new CircularProgressIndicator())) }));
                        }
                    default:
                        throw new InvalidOperationException("Non-exhaustive Dart switch value.");
                }
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _initDefaultDetailPage(_LicenseData__about data, BuildContext context)
    {
        if (!Enumerable.Any(data.packages))
        {
            return;
        }
        string packageName = data.packages[(int)(widget.selectedId.value ?? 0L)];
        List<long> bindings = data.packageLicenseBindings.GetValueOrDefault(packageName)!.ToList();
        _MasterDetailFlow__about.of(context).setInitialDetailPage(new _DetailArguments__about(packageName, bindings.map((i) => data.licenses[(int)i]).ToList()));
    }

    internal virtual Widget _packagesList(BuildContext context, long? selectedId, _LicenseData__about data, bool drawSelection)
    {
        EdgeInsets safeAreaPadding = MediaQuery.paddingOf(context);
        var paddingLocal = EdgeInsets.CreateOnly(left: safeAreaPadding.left, right: safeAreaPadding.right, bottom: safeAreaPadding.bottom);
        return ListView.CreateBuilder(padding: paddingLocal, itemCount: checked(data.packages.Count) + 1L, itemBuilder: (context, index) =>
        {
            if (index == 0L)
            {
                return widget.about;
            }
            long packageIndex = index - 1L;
            string packageNameLocal = data.packages[(int)packageIndex];
            List<long> bindings = data.packageLicenseBindings.GetValueOrDefault(packageNameLocal)!.ToList();
            return new _PackageListTile__about(packageName: packageNameLocal, index: packageIndex, isSelected: drawSelection && (packageIndex == (selectedId ?? 0L)), numberLicenses: checked(bindings.Count), onTap: () =>
            {
                widget.selectedId.value = packageIndex;
                _MasterDetailFlow__about.of(context).openDetailPage(new _DetailArguments__about(packageNameLocal, bindings.map((i) => data.licenses[(int)i]).ToList()));
            });
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _PackageListTile__about : StatelessWidget
{
    public virtual string packageName { get; private set; } = default!;
    public virtual long? index { get; private set; }
    public virtual bool isSelected { get; private set; } = default!;
    public virtual long numberLicenses { get; private set; } = default!;
    public virtual Action? onTap { get; private set; }

    internal _PackageListTile__about(string packageName, long? index = null, bool isSelected = default!, long numberLicenses = default!, Action? onTap = null)
    {
        this.packageName = packageName;
        this.index = index;
        this.isSelected = isSelected;
        this.numberLicenses = numberLicenses;
        this.onTap = onTap;
    }

    public override Widget build(BuildContext context)
    {
        return new Ink(color: isSelected ? Theme.of(context).highlightColor : Theme.of(context).cardColor, child: new ListTile(title: new Text(packageName), subtitle: new Text(MaterialLocalizations.of(context).licensesPackageDetailText(numberLicenses)), selected: isSelected, onTap: onTap));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _LicenseData__about
{
    public virtual List<LicenseEntry> licenses { get; private set; } = new List<LicenseEntry>();
    public virtual DartMap<string, List<long>> packageLicenseBindings { get; private set; } = new DartMap<string, List<long>>();
    public virtual List<string> packages { get; private set; } = new List<string>();
    public virtual string? firstPackage { get; set; } = default;

    public virtual void addLicense(LicenseEntry entry)
    {
        foreach (string package in entry.packages)
        {
            _addPackage(package);
            packageLicenseBindings.GetValueOrDefault(package)!.Add(checked(licenses.Count));
        }
        licenses.Add(entry);
    }

    internal virtual void _addPackage(string package)
    {
        if (!packageLicenseBindings.ContainsKey(package))
        {
            packageLicenseBindings[package] = new List<long>();
            firstPackage ??= package;
            packages.Add(package);
        }
    }

    public virtual void sortPackages(Func<string, string, long>? compare = null)
    {
        packages.sort(compare ?? ((a, b) =>
        {
            if (a == firstPackage)
            {
                return -1L;
            }
            if (b == firstPackage)
            {
                return 1L;
            }
            return a.toLowerCase().CompareTo(b.toLowerCase());
            throw new InvalidOperationException("Dart closure completed without a value.");
        }));
    }

}

internal class _DetailArguments__about
{
    public virtual string packageName { get; private set; } = default!;
    public virtual List<LicenseEntry> licenseEntries { get; private set; } = default!;

    internal _DetailArguments__about(string packageName, List<LicenseEntry> licenseEntries)
    {
        this.packageName = packageName;
        this.licenseEntries = licenseEntries;
    }

    public override bool Equals(object? other)
    {
        var __other = other as _DetailArguments__about;
        if (__other is null) return false;
        if (__other is _DetailArguments__about)
        {
            _DetailArguments__about other__as33303 = __other;
            return other__as33303.packageName == packageName;
        }
        return Equals(__other, this);
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(packageName, FoundationRuntimePorts.ObjectHashAll(licenseEntries)));
}

internal class _PackageLicensePage__about : StatefulWidget
{
    public virtual string packageName { get; private set; } = default!;
    public virtual List<LicenseEntry> licenseEntries { get; private set; } = default!;
    public virtual ScrollController? scrollController { get; private set; }

    internal _PackageLicensePage__about(string packageName, List<LicenseEntry> licenseEntries, ScrollController? scrollController)
    {
        this.packageName = packageName;
        this.licenseEntries = licenseEntries;
        this.scrollController = scrollController;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _PackageLicensePageState__about());
}

internal class _PackageLicensePageState__about : State<_PackageLicensePage__about>
{
    internal virtual List<Widget> _licenses { get; private set; } = new List<Widget>();
    internal virtual bool _loaded { get; set; } = false;

    public override void initState()
    {
        base.initState();
        DartRuntimePrimitives.Ignore(_initLicenses());
    }

    internal async virtual Future _initLicenses()
    {
        var debugFlowId = -1L;
        DartRuntimePrimitives.Assert(() =>
            {
                Runtime.Flow flowLocal = Runtime.Flow.begin();
                Timeline.timeSync("_initLicenses()", () =>
                {
                }, flow: flowLocal);
                debugFlowId = flowLocal.id;
                return true;
            });
        foreach (LicenseEntry license in widget.licenseEntries)
        {
            if (!mounted)
            {
                return;
            }
            DartRuntimePrimitives.Assert(() =>
                {
                    Timeline.timeSync("_initLicenses()", () =>
                    {
                    }, flow: Runtime.Flow.step(debugFlowId));
                    return true;
                });
            List<LicenseParagraph> paragraphsLocal = (await Scheduler.SchedulerBinding.instance.scheduleTask<List<LicenseParagraph>>((Func<object>)(() => license.paragraphs.toList()), Scheduler.Priority.animation, debugLabel: "License")).ToList();
            if (!mounted)
            {
                return;
            }
            setState(() =>
            {
                _licenses.Add(new Padding(padding: EdgeInsets.CreateAll(18.0), child: new Divider()));
                foreach (var paragraph in paragraphsLocal)
                {
                    if (paragraph.indent == LicenseParagraph.centeredIndent)
                    {
                        _licenses.Add(new Padding(padding: EdgeInsets.CreateOnly(top: 16.0), child: new Text(paragraph.text, style: new TextStyle(fontWeight: FontWeight.bold), textAlign: TextAlign.center)));
                    }
                    else
                    {
                        DartRuntimePrimitives.Assert(() => paragraph.indent >= 0L);
                        _licenses.Add(new Padding(padding: EdgeInsetsDirectional.CreateOnly(top: 8.0, start: 16.0 * paragraph.indent), child: new Text(paragraph.text)));
                    }
                }
            });
        }
        setState(() =>
        {
            _loaded = true;
        });
        DartRuntimePrimitives.Assert(() =>
            {
                Timeline.timeSync("Build scheduled", () =>
                {
                }, flow: Runtime.Flow.end(debugFlowId));
                return true;
            });
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterialLocalizations(context));
        MaterialLocalizations localizations = MaterialLocalizations.of(context);
        ThemeData themeLocal = Theme.of(context);
        string titleLocal = widget.packageName;
        string subtitleLocal = localizations.licensesPackageDetailText(checked(widget.licenseEntries.Count));
        double pad = AboutLibrary._getGutterSize(context);
        EdgeInsets safeAreaPadding = MediaQuery.paddingOf(context);
        var paddingLocal = EdgeInsets.CreateOnly(left: pad + safeAreaPadding.left, right: pad + safeAreaPadding.right, bottom: pad + safeAreaPadding.bottom);
        var listWidgets = ((Func<List<Widget>>)(() => { var __collection36658 = new List<Widget>(); __collection36658.AddRange(_licenses); if (!_loaded) { __collection36658.Add(DartRuntimePrimitives.ConvertValue<Widget>(new Padding(padding: EdgeInsets.CreateSymmetric(vertical: 24.0), child: new Center(child: new CircularProgressIndicator())))); } return __collection36658; }))();
        Widget page = default!;
        if (widget.scrollController is null)
        {
            page = DartRuntimePrimitives.ConvertValue<Widget>(new Scaffold(appBar: new AppBar(title: new _PackageLicensePageTitle__about(title: titleLocal, subtitle: subtitleLocal, theme: themeLocal.textTheme, titleTextStyle: themeLocal.appBarTheme.titleTextStyle, foregroundColor: themeLocal.appBarTheme.foregroundColor)), body: new Center(child: new Material(color: themeLocal.cardColor, elevation: 4.0, child: new ConstrainedBox(constraints: new BoxConstraints(maxWidth: 600.0), child: Localizations.CreateOverride(locale: new Locale("en", "US"), context: context, child: new ScrollConfiguration(behavior: ScrollConfiguration.of(context).copyWith(scrollbars: false), child: new Scrollbar(child: new ListView(primary: true, padding: paddingLocal, children: listWidgets)))))))));
        }
        else
        {
            page = DartRuntimePrimitives.ConvertValue<Widget>(new CustomScrollView(controller: widget.scrollController, slivers: new List<Widget> { DartRuntimePrimitives.ConvertValue<Widget>(new SliverAppBar(automaticallyImplyLeading: false, pinned: true, backgroundColor: themeLocal.cardColor, title: new _PackageLicensePageTitle__about(title: titleLocal, subtitle: subtitleLocal, theme: themeLocal.textTheme, titleTextStyle: themeLocal.textTheme.titleLarge))), DartRuntimePrimitives.ConvertValue<Widget>(new SliverPadding(padding: paddingLocal, sliver: SliverList.CreateBuilder(itemCount: checked(listWidgets.Count), itemBuilder: (context, index) => {
return Localizations.CreateOverride(locale: new Locale("en", "US"), context: context, child: listWidgets[(int)index]);
throw new InvalidOperationException("Dart closure completed without a value.");
}))) }));
        }
        return new DefaultTextStyle(style: themeLocal.textTheme.bodySmall!, child: page);
    }

}

internal class _PackageLicensePageTitle__about : StatelessWidget
{
    public virtual string title { get; private set; } = default!;
    public virtual string subtitle { get; private set; } = default!;
    public virtual TextTheme theme { get; private set; } = default!;
    public virtual TextStyle? titleTextStyle { get; private set; }
    public virtual Color? foregroundColor { get; private set; }

    internal _PackageLicensePageTitle__about(string title, string subtitle, TextTheme theme, TextStyle? titleTextStyle = null, Color? foregroundColor = null)
    {
        this.title = title;
        this.subtitle = subtitle;
        this.theme = theme;
        this.titleTextStyle = titleTextStyle;
        this.foregroundColor = foregroundColor;
    }

    public override Widget build(BuildContext context)
    {
        TextStyle? effectiveTitleTextStyle = titleTextStyle ?? theme.titleLarge;
        return new Column(mainAxisAlignment: MainAxisAlignment.center, crossAxisAlignment: CrossAxisAlignment.start, children: new List<Widget> { DartRuntimePrimitives.ConvertValue<Widget>(new Text(title, style: effectiveTitleTextStyle?.copyWith(color: foregroundColor))), DartRuntimePrimitives.ConvertValue<Widget>(new Text(subtitle, style: theme.titleSmall?.copyWith(color: foregroundColor))) });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public static partial class AboutLibrary
{
    internal static string _defaultApplicationName(BuildContext context)
    {
        Title? ancestorTitle = context.findAncestorWidgetOfExactType<Title>();
        return ancestorTitle?.title ?? Platform.resolvedExecutable.split(Platform.pathSeparator).Last();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class AboutLibrary
{
    internal static string _defaultApplicationVersion(BuildContext context)
    {
        return "";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class AboutLibrary
{
    internal static Widget? _defaultApplicationIcon(BuildContext context)
    {
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class AboutLibrary
{
    internal static long _materialGutterThreshold = 720L;
}

public static partial class AboutLibrary
{
    internal static double _wideGutterSize = 24.0;
}

public static partial class AboutLibrary
{
    internal static double _narrowGutterSize = 12.0;
}

public static partial class AboutLibrary
{
    internal static double _getGutterSize(BuildContext context) => (MediaQuery.widthOf(context) >= _materialGutterThreshold) ? _wideGutterSize : _narrowGutterSize;
}

internal delegate Widget _MasterViewBuilder__about(BuildContext context, bool isLateralUI);

internal delegate Widget _DetailPageBuilder__about(BuildContext context, object? arguments, ScrollController? scrollController);

internal delegate List<Widget> _ActionBuilder__about(BuildContext context, _ActionLevel__about actionLevel);

internal enum _ActionLevel__about
{
    top,
    view
}

internal enum _LayoutMode__about
{
    lateral,
    nested
}

public static partial class AboutLibrary
{
    internal static string _navMaster = "master";
}

public static partial class AboutLibrary
{
    internal static string _navDetail = "detail";
}

public enum _Focus__about
{
    master,
    detail
}

internal class _MasterDetailFlow__about : StatefulWidget
{
    public virtual Func<BuildContext, bool, Widget> masterViewBuilder { get; private set; } = default!;
    public virtual Func<BuildContext, object?, ScrollController?, Widget> detailPageBuilder { get; private set; } = default!;
    public virtual double? detailPageFABlessGutterWidth { get; private set; }
    public virtual Widget? title { get; private set; }

    internal _MasterDetailFlow__about(Func<BuildContext, object?, ScrollController?, Widget> detailPageBuilder, Func<BuildContext, bool, Widget> masterViewBuilder, double? detailPageFABlessGutterWidth = null, Widget? title = null)
    {
        this.detailPageBuilder = detailPageBuilder;
        this.masterViewBuilder = masterViewBuilder;
        this.detailPageFABlessGutterWidth = detailPageFABlessGutterWidth;
        this.title = title;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _MasterDetailFlowState__about());
    public static _MasterDetailFlowProxy__about of(BuildContext context)
    {
        _PageOpener__about? pageOpener = context.findAncestorStateOfType<_MasterDetailScaffoldState__about>();
        pageOpener ??= context.findAncestorStateOfType<_MasterDetailFlowState__about>();
        DartRuntimePrimitives.Assert(() =>
            {
                if (pageOpener is null)
                {
                    throw DartRuntimePrimitives.AsException(FlutterError.Create("Master Detail operation requested with a context that does not include a Master Detail " + "Flow.\nThe context used to open a detail page from the Master Detail Flow must be " + "that of a widget that is a descendant of a Master Detail Flow widget."));
                }
                return true;
            });
        return new _MasterDetailFlowProxy__about(pageOpener!);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _MasterDetailFlowProxy__about : _PageOpener__about
{
    internal virtual _PageOpener__about _pageOpener { get; private set; } = default!;

    internal _MasterDetailFlowProxy__about(_PageOpener__about _pageOpener)
    {
        this._pageOpener = _pageOpener;
    }

    public virtual void openDetailPage(object arguments) => _pageOpener.openDetailPage(arguments);
    public virtual void setInitialDetailPage(object arguments) => _pageOpener.setInitialDetailPage(arguments);
}

internal interface _PageOpener__about
{
    public void openDetailPage(object arguments);
    public void setInitialDetailPage(object arguments);
}

public static partial class AboutLibrary
{
    internal static long _materialWideDisplayThreshold = 840L;
}

internal class _MasterDetailFlowState__about : State<_MasterDetailFlow__about>, _PageOpener__about
{
    public virtual _Focus__about focus { get; set; } = _Focus__about.master;
    internal virtual object? _cachedDetailArguments { get; set; } = default;
    internal virtual _LayoutMode__about? _builtLayout { get; set; } = default;
    internal virtual GlobalKey<NavigatorState> _navigatorKey { get; private set; } = GlobalKey<NavigatorState>.Create();

    public virtual void openDetailPage(object arguments)
    {
        _cachedDetailArguments = arguments;
        switch (_builtLayout)
        {
            case _LayoutMode__about.nested:
                {
                    DartRuntimePrimitives.Ignore(_navigatorKey.currentState!.pushNamed<object>(AboutLibrary._navDetail, arguments: arguments));
                    break;
                }
            case _LayoutMode__about.lateral or null:
                {
                    focus = _Focus__about.detail;
                    break;
                }
        }
    }

    public virtual void setInitialDetailPage(object arguments)
    {
        _cachedDetailArguments = arguments;
    }

    public override Widget build(BuildContext context)
    {
        return new LayoutBuilder(builder: (context, constraints) =>
        {
            double availableWidth = constraints.maxWidth;
            if (availableWidth >= AboutLibrary._materialWideDisplayThreshold)
            {
                return _lateralUI(context);
            }
            return _nestedUI(context);
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Widget _nestedUI(BuildContext context)
    {
        _builtLayout = _LayoutMode__about.nested;
        MaterialPageRoute<object?> masterPageRoute = _masterPageRoute(context);
        return new NavigatorPopHandler<object>(onPop: () =>
        {
            DartRuntimePrimitives.Ignore(_navigatorKey.currentState!.maybePop<object>());
        }, child: new Navigator(key: _navigatorKey, initialRoute: "initial", onGenerateInitialRoutes: (navigator, initialRoute) =>
        {
            return focus switch { _Focus__about.master => new List<object> { masterPageRoute }, _Focus__about.detail => new List<object> { masterPageRoute, _detailPageRoute(_cachedDetailArguments) }, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
            throw new InvalidOperationException("Dart closure completed without a value.");
        }, onGenerateRoute: (settings) =>
        {
            switch (settings.name)
            {
                case var __constant48074 when Equals(__constant48074, AboutLibrary._navMaster):
                    {
                        focus = _Focus__about.master;
                        return masterPageRoute;
                    }
                case var __constant48231 when Equals(__constant48231, AboutLibrary._navDetail):
                    {
                        focus = _Focus__about.detail;
                        _cachedDetailArguments = settings.arguments;
                        return _detailPageRoute(_cachedDetailArguments);
                    }
                default:
                    {
                        throw new Exception($"Unknown route {settings.name}");
                    }
            }
            throw new InvalidOperationException("Dart closure completed without a value.");
        }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual MaterialPageRoute<object?> _masterPageRoute(BuildContext context)
    {
        return new MaterialPageRoute<object?>(builder: (c) =>
        {
            return new BlockSemantics(child: new _MasterPage__about(leading: Navigator.of(context).canPop() ? new BackButton(onPressed: () =>
            {
                Navigator.of(context).pop<object>();
            }) : null, title: widget.title, masterViewBuilder: widget.masterViewBuilder));
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual MaterialPageRoute<object?> _detailPageRoute(object? arguments)
    {
        return new MaterialPageRoute<object?>(builder: (context) =>
        {
            return new PopScope<object?>(onPopInvokedWithResult: (didPop, result) =>
            {
                focus = _Focus__about.master;
            }, child: new BlockSemantics(child: widget.detailPageBuilder(context, arguments, null)));
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Widget _lateralUI(BuildContext context)
    {
        _builtLayout = _LayoutMode__about.lateral;
        return new _MasterDetailScaffold__about(actionBuilder: (_, _) => new List<Widget>(), detailPageBuilder: (context, args, scrollController) => widget.detailPageBuilder(context, args ?? _cachedDetailArguments, scrollController), detailPageFABlessGutterWidth: widget.detailPageFABlessGutterWidth, initialArguments: _cachedDetailArguments, masterViewBuilder: (context, isLateral) => widget.masterViewBuilder(context, isLateral), title: widget.title);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _MasterPage__about : StatelessWidget
{
    public virtual Func<BuildContext, bool, Widget>? masterViewBuilder { get; private set; }
    public virtual Widget? title { get; private set; }
    public virtual Widget? leading { get; private set; }

    internal _MasterPage__about(Widget? leading = null, Widget? title = null, Func<BuildContext, bool, Widget>? masterViewBuilder = null)
    {
        this.leading = leading;
        this.title = title;
        this.masterViewBuilder = masterViewBuilder;
    }

    public override Widget build(BuildContext context)
    {
        return new Scaffold(appBar: new AppBar(title: title, leading: leading, actions: new List<Widget>()), body: masterViewBuilder!(context, false));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public static partial class AboutLibrary
{
    internal static double _kCardElevation = 4.0;
}

public static partial class AboutLibrary
{
    internal static double _kMasterViewWidth = 320.0;
}

public static partial class AboutLibrary
{
    internal static double _kDetailPageFABlessGutterWidth = 40.0;
}

public static partial class AboutLibrary
{
    internal static double _kDetailPageFABGutterWidth = 84.0;
}

internal class _MasterDetailScaffold__about : StatefulWidget
{
    public virtual Func<BuildContext, bool, Widget> masterViewBuilder { get; private set; } = default!;
    public virtual Func<BuildContext, object?, ScrollController?, Widget> detailPageBuilder { get; private set; } = default!;
    public virtual Func<BuildContext, _ActionLevel__about, List<Widget>>? actionBuilder { get; private set; }
    public virtual object? initialArguments { get; private set; }
    public virtual Widget? title { get; private set; }
    public virtual double? detailPageFABlessGutterWidth { get; private set; }

    internal _MasterDetailScaffold__about(Func<BuildContext, object?, ScrollController?, Widget> detailPageBuilder, Func<BuildContext, bool, Widget> masterViewBuilder, Func<BuildContext, _ActionLevel__about, List<Widget>>? actionBuilder = null, object? initialArguments = null, Widget? title = null, double? detailPageFABlessGutterWidth = null)
    {
        this.detailPageBuilder = detailPageBuilder;
        this.masterViewBuilder = masterViewBuilder;
        this.actionBuilder = actionBuilder;
        this.initialArguments = initialArguments;
        this.title = title;
        this.detailPageFABlessGutterWidth = detailPageFABlessGutterWidth;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _MasterDetailScaffoldState__about());
}

internal class _MasterDetailScaffoldState__about : State<_MasterDetailScaffold__about>, _PageOpener__about
{
    public virtual FloatingActionButtonLocation floatingActionButtonLocation { get; set; } = default!;
    public virtual double detailPageFABGutterWidth { get; set; } = default!;
    public virtual double detailPageFABlessGutterWidth { get; set; } = default!;
    public virtual double masterViewWidth { get; set; } = default!;
    internal virtual ValueNotifier<object?> _detailArguments { get; private set; } = new ValueNotifier<object?>(null);

    public override void initState()
    {
        base.initState();
        detailPageFABlessGutterWidth = widget.detailPageFABlessGutterWidth ?? AboutLibrary._kDetailPageFABlessGutterWidth;
        detailPageFABGutterWidth = AboutLibrary._kDetailPageFABGutterWidth;
        masterViewWidth = AboutLibrary._kMasterViewWidth;
        floatingActionButtonLocation = FloatingActionButtonLocation.endTop;
    }

    public override void dispose()
    {
        _detailArguments.dispose();
        base.dispose();
    }

    public virtual void openDetailPage(object arguments)
    {
        Scheduler.SchedulerBinding.instance.addPostFrameCallback((_duration) => { _detailArguments.value = arguments; });
        _MasterDetailFlow__about.of(context).openDetailPage(arguments);
    }

    public virtual void setInitialDetailPage(object arguments)
    {
        Scheduler.SchedulerBinding.instance.addPostFrameCallback((_duration) => { _detailArguments.value = arguments; });
        _MasterDetailFlow__about.of(context).setInitialDetailPage(arguments);
    }

    public override Widget build(BuildContext context)
    {
        return new Stack(children: new List<Widget> { DartRuntimePrimitives.ConvertValue<Widget>(new Scaffold(floatingActionButtonLocation: floatingActionButtonLocation, appBar: new AppBar(title: widget.title, actions: widget.actionBuilder!(context, _ActionLevel__about.top), bottom: new PreferredSize(preferredSize: new Size(ConstantsLibrary.kToolbarHeight), child: new Row(children: new List<Widget> { DartRuntimePrimitives.ConvertValue<Widget>(new SizedBox(width: masterViewWidth, child: new IconTheme(data: Theme.of(context).primaryIconTheme, child: new Padding(padding: EdgeInsets.CreateAll(8), child: new Align(alignment: AlignmentDirectional.centerEnd, child: new OverflowBar(spacing: 8, overflowAlignment: OverflowBarAlignment.end, children: widget.actionBuilder!(context, _ActionLevel__about.view))))))) }))), body: new Align(alignment: AlignmentDirectional.centerStart, child: _masterPanel(context)))), DartRuntimePrimitives.ConvertValue<Widget>(new SafeArea(child: new Padding(padding: EdgeInsetsDirectional.CreateOnly(start: masterViewWidth - AboutLibrary._kCardElevation, end: detailPageFABlessGutterWidth), child: new ValueListenableBuilder<object?>(valueListenable: _detailArguments, builder: (context, value, child) => {
return new AnimatedSwitcher(transitionBuilder: (child, animation) => new FadeUpwardsPageTransitionsBuilder().buildTransitions<object?>(null, null, animation, null, child), duration: Duration.Create(milliseconds: 500L), child: SizedBox.CreateExpand(key: new ValueKey<object?>(value ?? widget.initialArguments), child: new _DetailView__about(builder: widget.detailPageBuilder, arguments: value ?? widget.initialArguments)));
throw new InvalidOperationException("Dart closure completed without a value.");
})))) });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual ConstrainedBox _masterPanel(BuildContext context, bool needsScaffold = false)
    {
        return new ConstrainedBox(constraints: new BoxConstraints(maxWidth: masterViewWidth), child: needsScaffold ? new Scaffold(appBar: new AppBar(title: widget.title, actions: widget.actionBuilder!(context, _ActionLevel__about.top)), body: widget.masterViewBuilder(context, true)) : widget.masterViewBuilder(context, true));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _DetailView__about : StatelessWidget
{
    internal virtual Func<BuildContext, object?, ScrollController?, Widget> _builder { get; private set; } = default!;
    internal virtual object? _arguments { get; private set; }

    internal _DetailView__about(Func<BuildContext, object?, ScrollController?, Widget> builder, object? arguments = null)
    {
        _builder = builder;
        _arguments = arguments;
    }

    public override Widget build(BuildContext context)
    {
        if (_arguments is null)
        {
            return SizedBox.CreateShrink();
        }
        double screenHeight = MediaQuery.heightOf(context);
        double minHeight = (screenHeight - ConstantsLibrary.kToolbarHeight) / screenHeight;
        return new DraggableScrollableSheet(initialChildSize: minHeight, minChildSize: minHeight, expand: false, builder: (context, controller) =>
        {
            return new MouseRegion(child: new Card(color: Theme.of(context).cardColor, elevation: AboutLibrary._kCardElevation, clipBehavior: Clip.antiAlias, margin: new EdgeInsets(AboutLibrary._kCardElevation, 0.0, AboutLibrary._kCardElevation, 0.0), shape: new RoundedRectangleBorder(borderRadius: BorderRadius.CreateVertical(top: Radius.circular(3.0))), child: _builder(context, _arguments, controller)));
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
