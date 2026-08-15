// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/about.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Material;

public class AboutListTile : global::Doroti.Generated.Framework.Widgets.StatelessWidget
{
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? icon { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? child { get; private set; }
    public virtual string? applicationName { get; private set; }
    public virtual string? applicationVersion { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? applicationIcon { get; private set; }
    public virtual string? applicationLegalese { get; private set; }
    public virtual List<global::Doroti.Generated.Framework.Widgets.Widget>? aboutBoxChildren { get; private set; }
    public virtual bool? dense { get; private set; }

    public AboutListTile(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Widgets.Widget? icon = null, global::Doroti.Generated.Framework.Widgets.Widget? child = null, string? applicationName = null, string? applicationVersion = null, global::Doroti.Generated.Framework.Widgets.Widget? applicationIcon = null, string? applicationLegalese = null, List<global::Doroti.Generated.Framework.Widgets.Widget>? aboutBoxChildren = null, bool? dense = null) : base(key: key)
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

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterial(context));
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterialLocalizations(context));
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new ListTile(leading: this.icon, title: (this.child ?? new global::Doroti.Generated.Framework.Widgets.Text(MaterialLocalizations.of(context).aboutListTileTitle((this.applicationName ?? AboutLibrary._defaultApplicationName(context))))), dense: this.dense, onTap: ((global::System.Action)(() => {
AboutLibrary.showAboutDialog(context: context, applicationName: this.applicationName, applicationVersion: this.applicationVersion, applicationIcon: this.applicationIcon, applicationLegalese: this.applicationLegalese, children: this.aboutBoxChildren);
}))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public static partial class AboutLibrary
{
    public static void showAboutDialog(global::Doroti.Generated.Framework.Widgets.BuildContext context, string? applicationName = null, string? applicationVersion = null, global::Doroti.Generated.Framework.Widgets.Widget? applicationIcon = null, string? applicationLegalese = null, List<global::Doroti.Generated.Framework.Widgets.Widget>? children = null, bool barrierDismissible = true, Color? barrierColor = null, string? barrierLabel = null, bool useRootNavigator = true, global::Doroti.Generated.Framework.Widgets.RouteSettings? routeSettings = null, Offset? anchorPoint = null)
    {
        DartRuntimePrimitives.Ignore(DialogLibrary.showDialog<object?>(context: context, barrierDismissible: barrierDismissible, barrierColor: barrierColor, barrierLabel: barrierLabel, useRootNavigator: useRootNavigator, builder: ((context) => {
return new AboutDialog(applicationName: applicationName, applicationVersion: applicationVersion, applicationIcon: applicationIcon, applicationLegalese: applicationLegalese, children: children);
throw new InvalidOperationException("Dart closure completed without a value.");
}), routeSettings: routeSettings, anchorPoint: DartRuntimePrimitives.RequireValue(anchorPoint)));
    }
}

public static partial class AboutLibrary
{
    public static void showAdaptiveAboutDialog(global::Doroti.Generated.Framework.Widgets.BuildContext context, string? applicationName = null, string? applicationVersion = null, global::Doroti.Generated.Framework.Widgets.Widget? applicationIcon = null, string? applicationLegalese = null, List<global::Doroti.Generated.Framework.Widgets.Widget>? children = null, bool barrierDismissible = true, Color? barrierColor = null, string? barrierLabel = null, bool useRootNavigator = true, global::Doroti.Generated.Framework.Widgets.RouteSettings? routeSettings = null, Offset? anchorPoint = null)
    {
        DartRuntimePrimitives.Ignore(DialogLibrary.showAdaptiveDialog<object?>(context: context, barrierDismissible: barrierDismissible, barrierColor: barrierColor, barrierLabel: barrierLabel, useRootNavigator: useRootNavigator, builder: ((context) => {
return AboutDialog.CreateAdaptive(applicationName: applicationName, applicationVersion: applicationVersion, applicationIcon: applicationIcon, applicationLegalese: applicationLegalese, children: children);
throw new InvalidOperationException("Dart closure completed without a value.");
}), routeSettings: routeSettings, anchorPoint: anchorPoint));
    }
}

public static partial class AboutLibrary
{
    public static void showLicensePage(global::Doroti.Generated.Framework.Widgets.BuildContext context, string? applicationName = null, string? applicationVersion = null, global::Doroti.Generated.Framework.Widgets.Widget? applicationIcon = null, string? applicationLegalese = null, bool useRootNavigator = false)
    {
        global::Doroti.Generated.Framework.Widgets.CapturedThemes themes__10239 = ((global::Doroti.Generated.Framework.Widgets.CapturedThemes)(object?)InheritedTheme.capture(from: context, to: Navigator.of(context, rootNavigator: useRootNavigator).context));
        DartRuntimePrimitives.Ignore(Navigator.of(context, rootNavigator: useRootNavigator).push(new MaterialPageRoute<object?>(builder: ((context) => themes__10239.wrap(new LicensePage(applicationName: applicationName, applicationVersion: applicationVersion, applicationIcon: applicationIcon, applicationLegalese: applicationLegalese))))));
    }
}

public static partial class AboutLibrary
{
    internal static double _textVerticalSeparation = 18.0;
}

public class AboutDialog : global::Doroti.Generated.Framework.Widgets.StatelessWidget
{
    public virtual string? applicationName { get; private set; }
    public virtual string? applicationVersion { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? applicationIcon { get; private set; }
    public virtual string? applicationLegalese { get; private set; }
    public virtual List<global::Doroti.Generated.Framework.Widgets.Widget>? children { get; private set; }

    public AboutDialog(global::Doroti.Generated.Framework.Foundation.Key? key = null, string? applicationName = null, string? applicationVersion = null, global::Doroti.Generated.Framework.Widgets.Widget? applicationIcon = null, string? applicationLegalese = null, List<global::Doroti.Generated.Framework.Widgets.Widget>? children = null) : base(key: key)
    {
        this.applicationName = applicationName;
        this.applicationVersion = applicationVersion;
        this.applicationIcon = applicationIcon;
        this.applicationLegalese = applicationLegalese;
        this.children = children;
    }

    public static AboutDialog CreateAdaptive(global::Doroti.Generated.Framework.Foundation.Key? key = null, string? applicationName = null, string? applicationVersion = null, global::Doroti.Generated.Framework.Widgets.Widget? applicationIcon = null, string? applicationLegalese = null, List<global::Doroti.Generated.Framework.Widgets.Widget>? children = null)
        => ((AboutDialog)(object?)new _AdaptiveAboutDialog__about(key, applicationName, applicationVersion, applicationIcon, applicationLegalese, children));

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterialLocalizations(context));
        string name__14261 = (this.applicationName ?? AboutLibrary._defaultApplicationName(context));
        string version__14338 = (this.applicationVersion ?? AboutLibrary._defaultApplicationVersion(context));
        global::Doroti.Generated.Framework.Widgets.Widget? icon__14425 = (this.applicationIcon ?? AboutLibrary._defaultApplicationIcon(context));
        ThemeData themeData__14505 = Theme.of(context);
        MaterialLocalizations localizations__14568 = ((MaterialLocalizations)(object?)MaterialLocalizations.of(context));
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new AlertDialog(content: new global::Doroti.Generated.Framework.Widgets.ListBody(children: ((Func<List<global::Doroti.Generated.Framework.Widgets.Widget>>)(() => { var __collection14686 = new List<global::Doroti.Generated.Framework.Widgets.Widget>(); __collection14686.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Row(crossAxisAlignment: global::Doroti.Generated.Framework.Rendering.CrossAxisAlignment.start, children: ((Func<List<global::Doroti.Generated.Framework.Widgets.Widget>>)(() => { var __collection14791 = new List<global::Doroti.Generated.Framework.Widgets.Widget>(); if ((icon__14425 is not null)) { __collection14791.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.IconTheme(data: themeData__14505.iconTheme, child: icon__14425))); } __collection14791.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Expanded(child: new global::Doroti.Generated.Framework.Widgets.Padding(padding: global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 24.0), child: new global::Doroti.Generated.Framework.Widgets.ListBody(children: new List<global::Doroti.Generated.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Text(name__14261, style: themeData__14505.textTheme.headlineSmall)), DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Text(version__14338, style: themeData__14505.textTheme.bodyMedium)), DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.SizedBox(height: AboutLibrary._textVerticalSeparation)), DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Text((this.applicationLegalese ?? ""), style: themeData__14505.textTheme.bodySmall)) }))))); return __collection14791; }))()))); var __collectionSpread15522 = this.children; if (__collectionSpread15522 is not null) { __collection14686.AddRange(__collectionSpread15522); } return __collection14686; }))()), actions: new List<global::Doroti.Generated.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new TextButton(child: new global::Doroti.Generated.Framework.Widgets.Text((themeData__14505.useMaterial3 ? ((MaterialLocalizations)localizations__14568).viewLicensesButtonLabel : ((MaterialLocalizations)localizations__14568).viewLicensesButtonLabel.toUpperCase())), onPressed: (() => {
AboutLibrary.showLicensePage(context: context, applicationName: this.applicationName, applicationVersion: this.applicationVersion, applicationIcon: this.applicationIcon, applicationLegalese: this.applicationLegalese);
}))), DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new TextButton(child: new global::Doroti.Generated.Framework.Widgets.Text((themeData__14505.useMaterial3 ? ((MaterialLocalizations)localizations__14568).closeButtonLabel : ((MaterialLocalizations)localizations__14568).closeButtonLabel.toUpperCase())), onPressed: (() => {
Navigator.pop<object>(context);
}))) }, scrollable: true));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _AdaptiveAboutDialog__about : AboutDialog
{
    internal _AdaptiveAboutDialog__about(global::Doroti.Generated.Framework.Foundation.Key? key = null, string? applicationName = null, string? applicationVersion = null, global::Doroti.Generated.Framework.Widgets.Widget? applicationIcon = null, string? applicationLegalese = null, List<global::Doroti.Generated.Framework.Widgets.Widget>? children = null) : base(key: key, applicationName: applicationName, applicationVersion: applicationVersion, applicationIcon: applicationIcon, applicationLegalese: applicationLegalese, children: children)
    {
    }

    internal virtual List<global::Doroti.Generated.Framework.Widgets.Widget>? _actions(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        ThemeData themeData__16774 = Theme.of(context);
        MaterialLocalizations localizations__16837 = ((MaterialLocalizations)(object?)MaterialLocalizations.of(context));
        switch (themeData__16774.platform)
        {
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.iOS:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.macOS:
                {
                    return new List<global::Doroti.Generated.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new CupertinoDialogAction(child: new global::Doroti.Generated.Framework.Widgets.Text((themeData__16774.useMaterial3 ? ((MaterialLocalizations)localizations__16837).viewLicensesButtonLabel : ((MaterialLocalizations)localizations__16837).viewLicensesButtonLabel.toUpperCase())), onPressed: (() => {
AboutLibrary.showLicensePage(context: context, applicationName: this.applicationName, applicationVersion: this.applicationVersion, applicationIcon: this.applicationIcon, applicationLegalese: this.applicationLegalese);
}))), DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new CupertinoDialogAction(child: new global::Doroti.Generated.Framework.Widgets.Text((themeData__16774.useMaterial3 ? ((MaterialLocalizations)localizations__16837).closeButtonLabel : ((MaterialLocalizations)localizations__16837).closeButtonLabel.toUpperCase())), onPressed: (() => {
Navigator.pop<object>(context);
}))) };
                }
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.android:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.fuchsia:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.linux:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.windows:
                {
                    return new List<global::Doroti.Generated.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new TextButton(child: new global::Doroti.Generated.Framework.Widgets.Text((themeData__16774.useMaterial3 ? ((MaterialLocalizations)localizations__16837).viewLicensesButtonLabel : ((MaterialLocalizations)localizations__16837).viewLicensesButtonLabel.toUpperCase())), onPressed: (() => {
AboutLibrary.showLicensePage(context: context, applicationName: this.applicationName, applicationVersion: this.applicationVersion, applicationIcon: this.applicationIcon, applicationLegalese: this.applicationLegalese);
}))), DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new TextButton(child: new global::Doroti.Generated.Framework.Widgets.Text((themeData__16774.useMaterial3 ? ((MaterialLocalizations)localizations__16837).closeButtonLabel : ((MaterialLocalizations)localizations__16837).closeButtonLabel.toUpperCase())), onPressed: (() => {
Navigator.pop<object>(context);
}))) };
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        base.build(context);
        string name__19109 = (this.applicationName ?? AboutLibrary._defaultApplicationName(context));
        string version__19186 = (this.applicationVersion ?? AboutLibrary._defaultApplicationVersion(context));
        global::Doroti.Generated.Framework.Widgets.Widget? icon__19273 = (this.applicationIcon ?? AboutLibrary._defaultApplicationIcon(context));
        ThemeData themeData__19353 = Theme.of(context);
        List<global::Doroti.Generated.Framework.Widgets.Widget>? actions__19408 = ((List<global::Doroti.Generated.Framework.Widgets.Widget>?)(object?)_actions(context));
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)AlertDialog.CreateAdaptive(content: new global::Doroti.Generated.Framework.Widgets.ListBody(children: ((Func<List<global::Doroti.Generated.Framework.Widgets.Widget>>)(() => { var __collection19514 = new List<global::Doroti.Generated.Framework.Widgets.Widget>(); __collection19514.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Row(crossAxisAlignment: global::Doroti.Generated.Framework.Rendering.CrossAxisAlignment.start, children: ((Func<List<global::Doroti.Generated.Framework.Widgets.Widget>>)(() => { var __collection19619 = new List<global::Doroti.Generated.Framework.Widgets.Widget>(); if ((icon__19273 is not null)) { __collection19619.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.IconTheme(data: themeData__19353.iconTheme, child: icon__19273))); } __collection19619.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Expanded(child: new global::Doroti.Generated.Framework.Widgets.Padding(padding: global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 24.0), child: new global::Doroti.Generated.Framework.Widgets.ListBody(children: new List<global::Doroti.Generated.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Text(name__19109, style: themeData__19353.textTheme.headlineSmall)), DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Text(version__19186, style: themeData__19353.textTheme.bodyMedium)), DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.SizedBox(height: AboutLibrary._textVerticalSeparation)), DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Text((this.applicationLegalese ?? ""), style: themeData__19353.textTheme.bodySmall)) }))))); return __collection19619; }))()))); var __collectionSpread20350 = this.children; if (__collectionSpread20350 is not null) { __collection19514.AddRange(__collectionSpread20350); } return __collection19514; }))()), actions: actions__19408, scrollable: true));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class LicensePage : global::Doroti.Generated.Framework.Widgets.StatefulWidget
{
    public virtual string? applicationName { get; private set; }
    public virtual string? applicationVersion { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? applicationIcon { get; private set; }
    public virtual string? applicationLegalese { get; private set; }

    public LicensePage(global::Doroti.Generated.Framework.Foundation.Key? key = null, string? applicationName = null, string? applicationVersion = null, global::Doroti.Generated.Framework.Widgets.Widget? applicationIcon = null, string? applicationLegalese = null) : base(key: key)
    {
        this.applicationName = applicationName;
        this.applicationVersion = applicationVersion;
        this.applicationIcon = applicationIcon;
        this.applicationLegalese = applicationLegalese;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _LicensePageState__about());
}

internal class _LicensePageState__about : global::Doroti.Generated.Framework.Widgets.State<LicensePage>
{
    public virtual global::Doroti.Generated.Framework.Foundation.ValueNotifier<long?> selectedId { get; private set; } = new global::Doroti.Generated.Framework.Foundation.ValueNotifier<long?>(null);

    public override void dispose()
    {
        this.selectedId.dispose();
        base.dispose();
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new _MasterDetailFlow__about(detailPageFABlessGutterWidth: AboutLibrary._getGutterSize(context), title: new global::Doroti.Generated.Framework.Widgets.Text(MaterialLocalizations.of(context).licensesPageTitle), detailPageBuilder: (global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, object, global::Doroti.Generated.Framework.Widgets.ScrollController?, global::Doroti.Generated.Framework.Widgets.Widget>)this._packageLicensePage, masterViewBuilder: (global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, bool, global::Doroti.Generated.Framework.Widgets.Widget>)this._packagesView));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Generated.Framework.Widgets.Widget _packageLicensePage(global::Doroti.Generated.Framework.Widgets.BuildContext __unused0, object? args, global::Doroti.Generated.Framework.Widgets.ScrollController? scrollController)
    {
        DartRuntimePrimitives.Assert(() => (args is _DetailArguments__about));
        var detailArguments__23043 = ((_DetailArguments__about?)(object?)args!)!;
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new _PackageLicensePage__about(packageName: ((_DetailArguments__about)detailArguments__23043).packageName, licenseEntries: ((_DetailArguments__about)detailArguments__23043).licenseEntries, scrollController: scrollController));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Generated.Framework.Widgets.Widget _packagesView(global::Doroti.Generated.Framework.Widgets.BuildContext __unused0, bool isLateral)
    {
        global::Doroti.Generated.Framework.Widgets.Widget about__23350 = ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new _AboutProgram__about(name: (((LicensePage)this.widget).applicationName ?? AboutLibrary._defaultApplicationName(this.context)), icon: (((LicensePage)this.widget).applicationIcon ?? AboutLibrary._defaultApplicationIcon(this.context)), version: (((LicensePage)this.widget).applicationVersion ?? AboutLibrary._defaultApplicationVersion(this.context)), legalese: ((LicensePage)this.widget).applicationLegalese));
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new _PackagesView__about(about: about__23350, isLateral: isLateral, selectedId: this.selectedId));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _AboutProgram__about : global::Doroti.Generated.Framework.Widgets.StatelessWidget
{
    public virtual string name { get; private set; } = default!;
    public virtual string version { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? icon { get; private set; }
    public virtual string? legalese { get; private set; }

    internal _AboutProgram__about(string name, string version, global::Doroti.Generated.Framework.Widgets.Widget? icon = null, string? legalese = null)
    {
        this.name = name;
        this.version = version;
        this.icon = icon;
        this.legalese = legalese;
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Padding(padding: global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: AboutLibrary._getGutterSize(context), vertical: 24.0), child: new global::Doroti.Generated.Framework.Widgets.Column(children: ((Func<List<global::Doroti.Generated.Framework.Widgets.Widget>>)(() => { var __collection24177 = new List<global::Doroti.Generated.Framework.Widgets.Widget>(); __collection24177.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Text(this.name, style: Theme.of(context).textTheme.headlineSmall, textAlign: global::Doroti.Ui.TextAlign.center))); if ((this.icon is not null)) { __collection24177.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.IconTheme(data: Theme.of(context).iconTheme, child: this.icon!))); } if ((this.version != "")) { __collection24177.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Padding(padding: global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateOnly(bottom: AboutLibrary._textVerticalSeparation), child: new global::Doroti.Generated.Framework.Widgets.Text(this.version, style: Theme.of(context).textTheme.bodyMedium, textAlign: global::Doroti.Ui.TextAlign.center)))); } if (((this.legalese is not null) && (this.legalese != ""))) { __collection24177.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Text(this.legalese!, style: Theme.of(context).textTheme.bodySmall, textAlign: global::Doroti.Ui.TextAlign.center))); } __collection24177.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.SizedBox(height: AboutLibrary._textVerticalSeparation))); __collection24177.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Text("Powered by Flutter", style: Theme.of(context).textTheme.bodyMedium, textAlign: global::Doroti.Ui.TextAlign.center))); return __collection24177; }))())));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _PackagesView__about : global::Doroti.Generated.Framework.Widgets.StatefulWidget
{
    public virtual global::Doroti.Generated.Framework.Widgets.Widget about { get; private set; } = default!;
    public virtual bool isLateral { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Foundation.ValueNotifier<long?> selectedId { get; private set; } = default!;

    internal _PackagesView__about(global::Doroti.Generated.Framework.Widgets.Widget about, bool isLateral, global::Doroti.Generated.Framework.Foundation.ValueNotifier<long?> selectedId)
    {
        this.about = about;
        this.isLateral = isLateral;
        this.selectedId = selectedId;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _PackagesViewState__about());
}

internal class _PackagesViewState__about : global::Doroti.Generated.Framework.Widgets.State<_PackagesView__about>
{
    public virtual Future<_LicenseData__about> licenses { get; private set; } = Future<_LicenseData__about>.value(new _LicenseData__about());

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.FutureBuilder<_LicenseData__about>(future: this.licenses, builder: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.AsyncSnapshot<_LicenseData__about>, global::Doroti.Generated.Framework.Widgets.Widget>)((context, snapshot) => {
return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.LayoutBuilder(key: new global::Doroti.Generated.Framework.Foundation.ValueKey<global::Doroti.Generated.Framework.Widgets.ConnectionState>(((global::Doroti.Generated.Framework.Widgets.AsyncSnapshot<_LicenseData__about>)snapshot).connectionState), builder: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Rendering.BoxConstraints, global::Doroti.Generated.Framework.Widgets.Widget>)((context, constraints) => {
switch (((global::Doroti.Generated.Framework.Widgets.AsyncSnapshot<_LicenseData__about>)snapshot).connectionState)
{
    case global::Doroti.Generated.Framework.Widgets.ConnectionState.done:
        {
            if (((global::Doroti.Generated.Framework.Widgets.AsyncSnapshot<_LicenseData__about>)snapshot).hasError)
            {
                DartRuntimePrimitives.Assert(() =>
                    {
                        FlutterError.reportError(new global::Doroti.Generated.Framework.Foundation.FlutterErrorDetails(exception: ((global::Doroti.Generated.Framework.Widgets.AsyncSnapshot<_LicenseData__about>)snapshot).error!, stack: ((global::Doroti.Generated.Framework.Widgets.AsyncSnapshot<_LicenseData__about>)snapshot).stackTrace, context: new global::Doroti.Generated.Framework.Foundation.ErrorDescription("while decoding the license file")));
                        return true;
                    });
                return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Center(child: new global::Doroti.Generated.Framework.Widgets.Text(((string)((dynamic)((global::Doroti.Generated.Framework.Widgets.AsyncSnapshot<_LicenseData__about>)snapshot).error).ToString()))));
            }
            _initDefaultDetailPage(((global::Doroti.Generated.Framework.Widgets.AsyncSnapshot<_LicenseData__about>)snapshot).data!, context);
            return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.ValueListenableBuilder<long?>(valueListenable: ((_PackagesView__about)this.widget).selectedId, builder: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, long?, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>)((context, selectedId, _) => {
return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Center(child: new Material(color: Theme.of(context).cardColor, elevation: 4.0, child: new global::Doroti.Generated.Framework.Widgets.ConstrainedBox(constraints: new global::Doroti.Generated.Framework.Rendering.BoxConstraints(maxWidth: 600.0), child: _packagesList(context, selectedId, ((global::Doroti.Generated.Framework.Widgets.AsyncSnapshot<_LicenseData__about>)snapshot).data!, ((_PackagesView__about)this.widget).isLateral)))));
throw new InvalidOperationException("Dart closure completed without a value.");
}))));
        }
    case global::Doroti.Generated.Framework.Widgets.ConnectionState.none:
    case global::Doroti.Generated.Framework.Widgets.ConnectionState.active:
    case global::Doroti.Generated.Framework.Widgets.ConnectionState.waiting:
        {
            return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new Material(color: Theme.of(context).cardColor, child: new global::Doroti.Generated.Framework.Widgets.Column(children: new List<global::Doroti.Generated.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(((_PackagesView__about)this.widget).about), DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Center(child: new CircularProgressIndicator())) })));
        }
    default:
        throw new InvalidOperationException("Non-exhaustive Dart switch value.");
}
throw new InvalidOperationException("Dart closure completed without a value.");
}))));
throw new InvalidOperationException("Dart closure completed without a value.");
}))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _initDefaultDetailPage(_LicenseData__about data, global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        if (!System.Linq.Enumerable.Any(((_LicenseData__about)data).packages))
        {
            return;
        }
        string packageName__28371 = ((_LicenseData__about)data).packages[(int)((((_PackagesView__about)this.widget).selectedId.value ?? 0L))];
        List<long> bindings__28450 = ((_LicenseData__about)data).packageLicenseBindings.GetValueOrDefault(packageName__28371)!.ToList();
        _MasterDetailFlow__about.of(context).setInitialDetailPage(new _DetailArguments__about(packageName__28371, bindings__28450.map<long, global::Doroti.Generated.Framework.Foundation.LicenseEntry>(((i) => ((_LicenseData__about)data).licenses[(int)(i)])).ToList()));
    }

    internal virtual global::Doroti.Generated.Framework.Widgets.Widget _packagesList(global::Doroti.Generated.Framework.Widgets.BuildContext context, long? selectedId, _LicenseData__about data, bool drawSelection)
    {
        global::Doroti.Generated.Framework.Painting.EdgeInsets safeAreaPadding__28846 = ((global::Doroti.Generated.Framework.Painting.EdgeInsets)(object?)MediaQuery.paddingOf(context));
        var padding__28905 = global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateOnly(left: ((global::Doroti.Generated.Framework.Painting.EdgeInsets)safeAreaPadding__28846).left, right: ((global::Doroti.Generated.Framework.Painting.EdgeInsets)safeAreaPadding__28846).right, bottom: ((global::Doroti.Generated.Framework.Painting.EdgeInsets)safeAreaPadding__28846).bottom);
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)global::Doroti.Generated.Framework.Widgets.ListView.CreateBuilder(padding: padding__28905, itemCount: (checked((long)(((_LicenseData__about)data).packages.Count)) + 1L), itemBuilder: ((context, index) => {
if ((index == 0L))
{
    return ((_PackagesView__about)this.widget).about;
}
long packageIndex__29283 = (index - 1L);
string packageName__29330 = ((_LicenseData__about)data).packages[(int)(packageIndex__29283)];
List<long> bindings__29397 = ((_LicenseData__about)data).packageLicenseBindings.GetValueOrDefault(packageName__29330)!.ToList();
return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new _PackageListTile__about(packageName: packageName__29330, index: packageIndex__29283, isSelected: (drawSelection && (packageIndex__29283 == ((selectedId ?? 0L)))), numberLicenses: checked((long)(bindings__29397.Count)), onTap: ((global::System.Action)(() => {
((_PackagesView__about)this.widget).selectedId.value = packageIndex__29283;
_MasterDetailFlow__about.of(context).openDetailPage(new _DetailArguments__about(packageName__29330, bindings__29397.map<long, global::Doroti.Generated.Framework.Foundation.LicenseEntry>(((i) => ((_LicenseData__about)data).licenses[(int)(i)])).ToList()));
}))));
throw new InvalidOperationException("Dart closure completed without a value.");
})));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _PackageListTile__about : global::Doroti.Generated.Framework.Widgets.StatelessWidget
{
    public virtual string packageName { get; private set; } = default!;
    public virtual long? index { get; private set; }
    public virtual bool isSelected { get; private set; } = default!;
    public virtual long numberLicenses { get; private set; } = default!;
    public virtual global::System.Action? onTap { get; private set; }

    internal _PackageListTile__about(string packageName, long? index = null, bool isSelected = default!, long numberLicenses = default!, global::System.Action? onTap = null)
    {
        this.packageName = packageName;
        this.index = index;
        this.isSelected = isSelected;
        this.numberLicenses = numberLicenses;
        this.onTap = onTap;
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new Ink(color: (this.isSelected ? Theme.of(context).highlightColor : Theme.of(context).cardColor), child: new ListTile(title: new global::Doroti.Generated.Framework.Widgets.Text(this.packageName), subtitle: new global::Doroti.Generated.Framework.Widgets.Text(MaterialLocalizations.of(context).licensesPackageDetailText(this.numberLicenses)), selected: this.isSelected, onTap: this.onTap)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _LicenseData__about
{
    public virtual List<global::Doroti.Generated.Framework.Foundation.LicenseEntry> licenses { get; private set; } = new List<global::Doroti.Generated.Framework.Foundation.LicenseEntry>();
    public virtual DartMap<string, List<long>> packageLicenseBindings { get; private set; } = new DartMap<string, List<long>>();
    public virtual List<string> packages { get; private set; } = new List<string>();
    public virtual string? firstPackage { get; set; } = default;

    public virtual void addLicense(global::Doroti.Generated.Framework.Foundation.LicenseEntry entry)
    {
        foreach (string package__31476 in ((global::Doroti.Generated.Framework.Foundation.LicenseEntry)entry).packages)
        {
            _addPackage(package__31476);
            this.packageLicenseBindings.GetValueOrDefault(package__31476)!.Add(checked((long)(this.licenses.Count)));
        }
        this.licenses.Add(entry);
    }

    internal virtual void _addPackage(string package)
    {
        if (!this.packageLicenseBindings.ContainsKey(package))
        {
            this.packageLicenseBindings[package] = new List<long>();
            firstPackage ??= package;
            this.packages.Add(package);
        }
    }

    public virtual void sortPackages(global::System.Func<string, string, long>? compare = null)
    {
        this.packages.sort(((compare ?? (global::System.Func<string, string, long>)((a, b) => {
if ((a == this.firstPackage))
{
    return -1L;
}
if ((b == this.firstPackage))
{
    return 1L;
}
return a.toLowerCase().CompareTo(b.toLowerCase());
throw new InvalidOperationException("Dart closure completed without a value.");
}))));
    }

}

internal class _DetailArguments__about
{
    public virtual string packageName { get; private set; } = default!;
    public virtual List<global::Doroti.Generated.Framework.Foundation.LicenseEntry> licenseEntries { get; private set; } = default!;

    internal _DetailArguments__about(string packageName, List<global::Doroti.Generated.Framework.Foundation.LicenseEntry> licenseEntries)
    {
        this.packageName = packageName;
        this.licenseEntries = licenseEntries;
    }

    public override bool Equals(object? other)
    {
        var __other = other as _DetailArguments__about;
        if (__other is null) return false;
        if ((__other is _DetailArguments__about))
        {
            _DetailArguments__about other__as33303 = (_DetailArguments__about)__other;
            return (((_DetailArguments__about)((_DetailArguments__about)other__as33303)).packageName == this.packageName);
        }
        return (object.Equals(__other, this));
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.packageName, FoundationRuntimePorts.ObjectHashAll(this.licenseEntries)));
}

internal class _PackageLicensePage__about : global::Doroti.Generated.Framework.Widgets.StatefulWidget
{
    public virtual string packageName { get; private set; } = default!;
    public virtual List<global::Doroti.Generated.Framework.Foundation.LicenseEntry> licenseEntries { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.ScrollController? scrollController { get; private set; }

    internal _PackageLicensePage__about(string packageName, List<global::Doroti.Generated.Framework.Foundation.LicenseEntry> licenseEntries, global::Doroti.Generated.Framework.Widgets.ScrollController? scrollController)
    {
        this.packageName = packageName;
        this.licenseEntries = licenseEntries;
        this.scrollController = scrollController;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _PackageLicensePageState__about());
}

internal class _PackageLicensePageState__about : global::Doroti.Generated.Framework.Widgets.State<_PackageLicensePage__about>
{
    internal virtual List<global::Doroti.Generated.Framework.Widgets.Widget> _licenses { get; private set; } = new List<global::Doroti.Generated.Framework.Widgets.Widget>();
    internal virtual bool _loaded { get; set; } = false;

    public override void initState()
    {
        base.initState();
        DartRuntimePrimitives.Ignore(_initLicenses());
    }

    internal async virtual Future _initLicenses()
    {
        var debugFlowId__34170 = -1L;
        DartRuntimePrimitives.Assert(() =>
            {
                global::Doroti.Runtime.Flow flow__34221 = global::Doroti.Runtime.Flow.begin();
                Timeline.timeSync("_initLicenses()", (() => {
}), flow: flow__34221);
                debugFlowId__34170 = flow__34221.id;
                return true;
            });
        foreach (global::Doroti.Generated.Framework.Foundation.LicenseEntry license__34391 in ((_PackageLicensePage__about)this.widget).licenseEntries)
        {
            if (!this.mounted)
            {
                return;
            }
            DartRuntimePrimitives.Assert(() =>
                {
                    Timeline.timeSync("_initLicenses()", (() => {
}), flow: global::Doroti.Runtime.Flow.step(debugFlowId__34170));
                    return true;
                });
            List<global::Doroti.Generated.Framework.Foundation.LicenseParagraph> paragraphs__34642 = (await global::Doroti.Generated.Framework.Scheduler.SchedulerBinding.instance.scheduleTask<List<global::Doroti.Generated.Framework.Foundation.LicenseParagraph>>((global::System.Func<object>)(() => ((global::Doroti.Generated.Framework.Foundation.LicenseEntry)license__34391).paragraphs.toList()), global::Doroti.Generated.Framework.Scheduler.Priority.animation, debugLabel: "License")).ToList();
            if (!this.mounted)
            {
                return;
            }
            setState(((global::System.Action)(() => {
this._licenses.Add(new global::Doroti.Generated.Framework.Widgets.Padding(padding: global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateAll(18.0), child: new Divider()));
foreach (var paragraph__35027 in paragraphs__34642)
{
    if ((((global::Doroti.Generated.Framework.Foundation.LicenseParagraph)paragraph__35027).indent == global::Doroti.Generated.Framework.Foundation.LicenseParagraph.centeredIndent))
    {
        this._licenses.Add(new global::Doroti.Generated.Framework.Widgets.Padding(padding: global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateOnly(top: 16.0), child: new global::Doroti.Generated.Framework.Widgets.Text(((global::Doroti.Generated.Framework.Foundation.LicenseParagraph)paragraph__35027).text, style: new global::Doroti.Generated.Framework.Painting.TextStyle(fontWeight: FontWeight.bold), textAlign: global::Doroti.Ui.TextAlign.center)));
    }
    else
    {
        DartRuntimePrimitives.Assert(() => (((global::Doroti.Generated.Framework.Foundation.LicenseParagraph)paragraph__35027).indent >= 0L));
        this._licenses.Add(new global::Doroti.Generated.Framework.Widgets.Padding(padding: global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional.CreateOnly(top: 8.0, start: (16.0 * ((global::Doroti.Generated.Framework.Foundation.LicenseParagraph)paragraph__35027).indent)), child: new global::Doroti.Generated.Framework.Widgets.Text(((global::Doroti.Generated.Framework.Foundation.LicenseParagraph)paragraph__35027).text)));
    }
}
})));
        }
        setState(((global::System.Action)(() => {
_loaded = true;
})));
        DartRuntimePrimitives.Assert(() =>
            {
                Timeline.timeSync("Build scheduled", (() => {
}), flow: global::Doroti.Runtime.Flow.end(debugFlowId__34170));
                return true;
            });
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterialLocalizations(context));
        MaterialLocalizations localizations__36104 = ((MaterialLocalizations)(object?)MaterialLocalizations.of(context));
        ThemeData theme__36175 = Theme.of(context);
        string title__36219 = ((_PackageLicensePage__about)this.widget).packageName;
        string subtitle__36264 = ((string)(object?)localizations__36104.licensesPackageDetailText(checked((long)(((_PackageLicensePage__about)this.widget).licenseEntries.Count))));
        double pad__36363 = AboutLibrary._getGutterSize(context);
        global::Doroti.Generated.Framework.Painting.EdgeInsets safeAreaPadding__36415 = ((global::Doroti.Generated.Framework.Painting.EdgeInsets)(object?)MediaQuery.paddingOf(context));
        var padding__36474 = global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateOnly(left: (pad__36363 + ((global::Doroti.Generated.Framework.Painting.EdgeInsets)safeAreaPadding__36415).left), right: (pad__36363 + ((global::Doroti.Generated.Framework.Painting.EdgeInsets)safeAreaPadding__36415).right), bottom: (pad__36363 + ((global::Doroti.Generated.Framework.Painting.EdgeInsets)safeAreaPadding__36415).bottom));
        var listWidgets__36644 = ((Func<List<global::Doroti.Generated.Framework.Widgets.Widget>>)(() => { var __collection36658 = new List<global::Doroti.Generated.Framework.Widgets.Widget>(); __collection36658.AddRange(this._licenses); if (!this._loaded) { __collection36658.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Padding(padding: global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateSymmetric(vertical: 24.0), child: new global::Doroti.Generated.Framework.Widgets.Center(child: new CircularProgressIndicator())))); } return __collection36658; }))();
        global::Doroti.Generated.Framework.Widgets.Widget page__36885 = default!;
        if ((((_PackageLicensePage__about)this.widget).scrollController is null))
        {
            page__36885 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new Scaffold(appBar: new AppBar(title: new _PackageLicensePageTitle__about(title: title__36219, subtitle: subtitle__36264, theme: (theme__36175.useMaterial3 ? theme__36175.textTheme : theme__36175.primaryTextTheme), titleTextStyle: theme__36175.appBarTheme.titleTextStyle, foregroundColor: theme__36175.appBarTheme.foregroundColor)), body: new global::Doroti.Generated.Framework.Widgets.Center(child: new Material(color: theme__36175.cardColor, elevation: 4.0, child: new global::Doroti.Generated.Framework.Widgets.ConstrainedBox(constraints: new global::Doroti.Generated.Framework.Rendering.BoxConstraints(maxWidth: 600.0), child: global::Doroti.Generated.Framework.Widgets.Localizations.CreateOverride(locale: new global::Doroti.Ui.Locale("en", "US"), context: context, child: new global::Doroti.Generated.Framework.Widgets.ScrollConfiguration(behavior: ScrollConfiguration.of(context).copyWith(scrollbars: false), child: new Scrollbar(child: new global::Doroti.Generated.Framework.Widgets.ListView(primary: true, padding: padding__36474, children: listWidgets__36644)))))))));
        }
        else
        {
            page__36885 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.CustomScrollView(controller: ((_PackageLicensePage__about)this.widget).scrollController, slivers: new List<global::Doroti.Generated.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new SliverAppBar(automaticallyImplyLeading: false, pinned: true, backgroundColor: theme__36175.cardColor, title: new _PackageLicensePageTitle__about(title: title__36219, subtitle: subtitle__36264, theme: theme__36175.textTheme, titleTextStyle: theme__36175.textTheme.titleLarge))), DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.SliverPadding(padding: padding__36474, sliver: global::Doroti.Generated.Framework.Widgets.SliverList.CreateBuilder(itemCount: checked((long)(listWidgets__36644.Count)), itemBuilder: ((context, index) => {
return global::Doroti.Generated.Framework.Widgets.Localizations.CreateOverride(locale: new global::Doroti.Ui.Locale("en", "US"), context: context, child: listWidgets__36644[(int)(index)]);
throw new InvalidOperationException("Dart closure completed without a value.");
})))) }));
        }
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.DefaultTextStyle(style: theme__36175.textTheme.bodySmall!, child: page__36885));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _PackageLicensePageTitle__about : global::Doroti.Generated.Framework.Widgets.StatelessWidget
{
    public virtual string title { get; private set; } = default!;
    public virtual string subtitle { get; private set; } = default!;
    public virtual TextTheme theme { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle? titleTextStyle { get; private set; }
    public virtual Color? foregroundColor { get; private set; }

    internal _PackageLicensePageTitle__about(string title, string subtitle, TextTheme theme, global::Doroti.Generated.Framework.Painting.TextStyle? titleTextStyle = null, Color? foregroundColor = null)
    {
        this.title = title;
        this.subtitle = subtitle;
        this.theme = theme;
        this.titleTextStyle = titleTextStyle;
        this.foregroundColor = foregroundColor;
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Generated.Framework.Painting.TextStyle? effectiveTitleTextStyle__39562 = (this.titleTextStyle ?? this.theme.titleLarge);
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Column(mainAxisAlignment: global::Doroti.Generated.Framework.Rendering.MainAxisAlignment.center, crossAxisAlignment: global::Doroti.Generated.Framework.Rendering.CrossAxisAlignment.start, children: new List<global::Doroti.Generated.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Text(this.title, style: effectiveTitleTextStyle__39562?.copyWith(color: this.foregroundColor))), DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Text(this.subtitle, style: this.theme.titleSmall?.copyWith(color: this.foregroundColor))) }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public static partial class AboutLibrary
{
    internal static string _defaultApplicationName(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Generated.Framework.Widgets.Title? ancestorTitle__40462 = ((global::Doroti.Generated.Framework.Widgets.Title?)(object?)context.findAncestorWidgetOfExactType<global::Doroti.Generated.Framework.Widgets.Title>());
        return (ancestorTitle__40462?.title ?? Platform.resolvedExecutable.split(Platform.pathSeparator).Last());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class AboutLibrary
{
    internal static string _defaultApplicationVersion(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return "";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class AboutLibrary
{
    internal static global::Doroti.Generated.Framework.Widgets.Widget? _defaultApplicationIcon(global::Doroti.Generated.Framework.Widgets.BuildContext context)
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
    internal static double _getGutterSize(global::Doroti.Generated.Framework.Widgets.BuildContext context) => ((MediaQuery.widthOf(context) >= AboutLibrary._materialGutterThreshold) ? AboutLibrary._wideGutterSize : AboutLibrary._narrowGutterSize);
}

internal delegate global::Doroti.Generated.Framework.Widgets.Widget _MasterViewBuilder__about(global::Doroti.Generated.Framework.Widgets.BuildContext context, bool isLateralUI);

internal delegate global::Doroti.Generated.Framework.Widgets.Widget _DetailPageBuilder__about(global::Doroti.Generated.Framework.Widgets.BuildContext context, object? arguments, global::Doroti.Generated.Framework.Widgets.ScrollController? scrollController);

internal delegate List<global::Doroti.Generated.Framework.Widgets.Widget> _ActionBuilder__about(global::Doroti.Generated.Framework.Widgets.BuildContext context, _ActionLevel__about actionLevel);

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

internal class _MasterDetailFlow__about : global::Doroti.Generated.Framework.Widgets.StatefulWidget
{
    public virtual global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, bool, global::Doroti.Generated.Framework.Widgets.Widget> masterViewBuilder { get; private set; } = default!;
    public virtual global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, object, global::Doroti.Generated.Framework.Widgets.ScrollController?, global::Doroti.Generated.Framework.Widgets.Widget> detailPageBuilder { get; private set; } = default!;
    public virtual double? detailPageFABlessGutterWidth { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? title { get; private set; }

    internal _MasterDetailFlow__about(global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, object, global::Doroti.Generated.Framework.Widgets.ScrollController?, global::Doroti.Generated.Framework.Widgets.Widget> detailPageBuilder, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, bool, global::Doroti.Generated.Framework.Widgets.Widget> masterViewBuilder, double? detailPageFABlessGutterWidth = null, global::Doroti.Generated.Framework.Widgets.Widget? title = null)
    {
        this.detailPageBuilder = detailPageBuilder;
        this.masterViewBuilder = masterViewBuilder;
        this.detailPageFABlessGutterWidth = detailPageFABlessGutterWidth;
        this.title = title;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _MasterDetailFlowState__about());
    public static _MasterDetailFlowProxy__about of(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        _PageOpener__about? pageOpener__44520 = ((_PageOpener__about?)(object?)context.findAncestorStateOfType<_MasterDetailScaffoldState__about>());
        pageOpener__44520 ??= context.findAncestorStateOfType<_MasterDetailFlowState__about>();
        DartRuntimePrimitives.Assert(() =>
            {
                if ((pageOpener__44520 is null))
                {
                    throw DartRuntimePrimitives.AsException(global::Doroti.Generated.Framework.Foundation.FlutterError.Create("Master Detail operation requested with a context that does not include a Master Detail " + "Flow.\nThe context used to open a detail page from the Master Detail Flow must be " + "that of a widget that is a descendant of a Master Detail Flow widget."));
                }
                return true;
            });
        return new _MasterDetailFlowProxy__about(pageOpener__44520!);
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

    public virtual void openDetailPage(object arguments) => this._pageOpener.openDetailPage(arguments);
    public virtual void setInitialDetailPage(object arguments) => this._pageOpener.setInitialDetailPage(arguments);
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

internal class _MasterDetailFlowState__about : global::Doroti.Generated.Framework.Widgets.State<_MasterDetailFlow__about>, _PageOpener__about
{
    public virtual _Focus__about focus { get; set; } = _Focus__about.master;
    internal virtual object? _cachedDetailArguments { get; set; } = default;
    internal virtual _LayoutMode__about? _builtLayout { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Widgets.GlobalKey<global::Doroti.Generated.Framework.Widgets.NavigatorState> _navigatorKey { get; private set; } = global::Doroti.Generated.Framework.Widgets.GlobalKey<global::Doroti.Generated.Framework.Widgets.NavigatorState>.Create();

    public virtual void openDetailPage(object arguments)
    {
        _cachedDetailArguments = arguments;
        switch (this._builtLayout)
        {
            case _LayoutMode__about.nested:
                {
                    DartRuntimePrimitives.Ignore(((global::Doroti.Generated.Framework.Widgets.GlobalKey<global::Doroti.Generated.Framework.Widgets.NavigatorState>)this._navigatorKey).currentState!.pushNamed<object>(AboutLibrary._navDetail, arguments: arguments));
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

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.LayoutBuilder(builder: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Rendering.BoxConstraints, global::Doroti.Generated.Framework.Widgets.Widget>)((context, constraints) => {
double availableWidth__47062 = ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints).maxWidth;
if ((availableWidth__47062 >= AboutLibrary._materialWideDisplayThreshold))
{
    return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)_lateralUI(context));
}
return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)_nestedUI(context));
throw new InvalidOperationException("Dart closure completed without a value.");
}))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Generated.Framework.Widgets.Widget _nestedUI(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        _builtLayout = _LayoutMode__about.nested;
        MaterialPageRoute<object?> masterPageRoute__47384 = ((MaterialPageRoute<object?>)(object?)_masterPageRoute(context));
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.NavigatorPopHandler<object>(onPop: ((global::System.Action)(() => {
DartRuntimePrimitives.Ignore(((global::Doroti.Generated.Framework.Widgets.GlobalKey<global::Doroti.Generated.Framework.Widgets.NavigatorState>)this._navigatorKey).currentState!.maybePop<object>());
})), child: new global::Doroti.Generated.Framework.Widgets.Navigator(key: this._navigatorKey, initialRoute: "initial", onGenerateInitialRoutes: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.NavigatorState, string, List<dynamic>>)((navigator, initialRoute) => {
return ((List<object>)(object?)(this.focus switch { _Focus__about.master => new List<global::Doroti.Generated.Framework.Widgets.Route<object?>> { masterPageRoute__47384 }, _Focus__about.detail => new List<global::Doroti.Generated.Framework.Widgets.Route<object?>> { masterPageRoute__47384, _detailPageRoute(this._cachedDetailArguments) }, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
throw new InvalidOperationException("Dart closure completed without a value.");
})), onGenerateRoute: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.RouteSettings, dynamic>?)((settings) => {
switch (((global::Doroti.Generated.Framework.Widgets.RouteSettings)settings).name)
{
    case var __constant48074 when (object.Equals(__constant48074, AboutLibrary._navMaster)):
        {
            focus = _Focus__about.master;
            return masterPageRoute__47384;
        }
    case var __constant48231 when (object.Equals(__constant48231, AboutLibrary._navDetail)):
        {
            focus = _Focus__about.detail;
            _cachedDetailArguments = ((global::Doroti.Generated.Framework.Widgets.RouteSettings)settings).arguments;
            return _detailPageRoute(this._cachedDetailArguments);
        }
    default:
        {
            throw new Exception($"Unknown route {(((global::Doroti.Generated.Framework.Widgets.RouteSettings)settings).name)}");
        }
}
throw new InvalidOperationException("Dart closure completed without a value.");
})))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual MaterialPageRoute<object?> _masterPageRoute(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((MaterialPageRoute<object?>)(object?)new MaterialPageRoute<object>(builder: ((c) => {
return new global::Doroti.Generated.Framework.Widgets.BlockSemantics(child: new _MasterPage__about(leading: (Navigator.of(context).canPop() ? new BackButton(onPressed: ((global::System.Action)(() => {
Navigator.of(context).pop<object>();
}))) : null), title: ((_MasterDetailFlow__about)this.widget).title, masterViewBuilder: (global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, bool, global::Doroti.Generated.Framework.Widgets.Widget>)((_MasterDetailFlow__about)this.widget).masterViewBuilder));
throw new InvalidOperationException("Dart closure completed without a value.");
})));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual MaterialPageRoute<object?> _detailPageRoute(object? arguments)
    {
        return new MaterialPageRoute<object?>(builder: ((context) => {
return new global::Doroti.Generated.Framework.Widgets.PopScope<object?>(onPopInvokedWithResult: ((global::System.Action<bool, object?>)((didPop, result) => {
focus = _Focus__about.master;
})), child: new global::Doroti.Generated.Framework.Widgets.BlockSemantics(child: this.widget.detailPageBuilder(context, arguments, null)));
throw new InvalidOperationException("Dart closure completed without a value.");
}));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Generated.Framework.Widgets.Widget _lateralUI(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        _builtLayout = _LayoutMode__about.lateral;
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new _MasterDetailScaffold__about(actionBuilder: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, _ActionLevel__about, List<global::Doroti.Generated.Framework.Widgets.Widget>>?)((_, _) => new List<global::Doroti.Generated.Framework.Widgets.Widget>())), detailPageBuilder: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, object, global::Doroti.Generated.Framework.Widgets.ScrollController?, global::Doroti.Generated.Framework.Widgets.Widget>)((context, args, scrollController) => this.widget.detailPageBuilder(context, (args ?? this._cachedDetailArguments), scrollController))), detailPageFABlessGutterWidth: ((_MasterDetailFlow__about)this.widget).detailPageFABlessGutterWidth, initialArguments: this._cachedDetailArguments, masterViewBuilder: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, bool, global::Doroti.Generated.Framework.Widgets.Widget>)((context, isLateral) => this.widget.masterViewBuilder(context, isLateral))), title: ((_MasterDetailFlow__about)this.widget).title));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _MasterPage__about : global::Doroti.Generated.Framework.Widgets.StatelessWidget
{
    public virtual global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, bool, global::Doroti.Generated.Framework.Widgets.Widget>? masterViewBuilder { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? title { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? leading { get; private set; }

    internal _MasterPage__about(global::Doroti.Generated.Framework.Widgets.Widget? leading = null, global::Doroti.Generated.Framework.Widgets.Widget? title = null, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, bool, global::Doroti.Generated.Framework.Widgets.Widget>? masterViewBuilder = null)
    {
        this.leading = leading;
        this.title = title;
        this.masterViewBuilder = masterViewBuilder;
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new Scaffold(appBar: new AppBar(title: this.title, leading: this.leading, actions: new List<global::Doroti.Generated.Framework.Widgets.Widget>()), body: this.masterViewBuilder!(context, false)));
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

internal class _MasterDetailScaffold__about : global::Doroti.Generated.Framework.Widgets.StatefulWidget
{
    public virtual global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, bool, global::Doroti.Generated.Framework.Widgets.Widget> masterViewBuilder { get; private set; } = default!;
    public virtual global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, object, global::Doroti.Generated.Framework.Widgets.ScrollController?, global::Doroti.Generated.Framework.Widgets.Widget> detailPageBuilder { get; private set; } = default!;
    public virtual global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, _ActionLevel__about, List<global::Doroti.Generated.Framework.Widgets.Widget>>? actionBuilder { get; private set; }
    public virtual object? initialArguments { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? title { get; private set; }
    public virtual double? detailPageFABlessGutterWidth { get; private set; }

    internal _MasterDetailScaffold__about(global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, object, global::Doroti.Generated.Framework.Widgets.ScrollController?, global::Doroti.Generated.Framework.Widgets.Widget> detailPageBuilder, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, bool, global::Doroti.Generated.Framework.Widgets.Widget> masterViewBuilder, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, _ActionLevel__about, List<global::Doroti.Generated.Framework.Widgets.Widget>>? actionBuilder = null, object? initialArguments = null, global::Doroti.Generated.Framework.Widgets.Widget? title = null, double? detailPageFABlessGutterWidth = null)
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

internal class _MasterDetailScaffoldState__about : global::Doroti.Generated.Framework.Widgets.State<_MasterDetailScaffold__about>, _PageOpener__about
{
    public virtual FloatingActionButtonLocation floatingActionButtonLocation { get; set; } = default!;
    public virtual double detailPageFABGutterWidth { get; set; } = default!;
    public virtual double detailPageFABlessGutterWidth { get; set; } = default!;
    public virtual double masterViewWidth { get; set; } = default!;
    internal virtual global::Doroti.Generated.Framework.Foundation.ValueNotifier<object> _detailArguments { get; private set; } = new global::Doroti.Generated.Framework.Foundation.ValueNotifier<object?>(null);

    public override void initState()
    {
        base.initState();
        detailPageFABlessGutterWidth = (((_MasterDetailScaffold__about)this.widget).detailPageFABlessGutterWidth ?? AboutLibrary._kDetailPageFABlessGutterWidth);
        detailPageFABGutterWidth = AboutLibrary._kDetailPageFABGutterWidth;
        masterViewWidth = AboutLibrary._kMasterViewWidth;
        floatingActionButtonLocation = FloatingActionButtonLocation.endTop;
    }

    public override void dispose()
    {
        this._detailArguments.dispose();
        base.dispose();
    }

    public virtual void openDetailPage(object arguments)
    {
        global::Doroti.Generated.Framework.Scheduler.SchedulerBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((_duration) => { this._detailArguments.value = arguments; })));
        _MasterDetailFlow__about.of(this.context).openDetailPage(arguments);
    }

    public virtual void setInitialDetailPage(object arguments)
    {
        global::Doroti.Generated.Framework.Scheduler.SchedulerBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((_duration) => { this._detailArguments.value = arguments; })));
        _MasterDetailFlow__about.of(this.context).setInitialDetailPage(arguments);
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Stack(children: new List<global::Doroti.Generated.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new Scaffold(floatingActionButtonLocation: this.floatingActionButtonLocation, appBar: new AppBar(title: ((_MasterDetailScaffold__about)this.widget).title, actions: ((_MasterDetailScaffold__about)this.widget).actionBuilder!(context, _ActionLevel__about.top), bottom: new global::Doroti.Generated.Framework.Widgets.PreferredSize(preferredSize: new global::Doroti.Ui.Size(ConstantsLibrary.kToolbarHeight), child: new global::Doroti.Generated.Framework.Widgets.Row(children: new List<global::Doroti.Generated.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.SizedBox(width: this.masterViewWidth, child: new global::Doroti.Generated.Framework.Widgets.IconTheme(data: Theme.of(context).primaryIconTheme, child: new global::Doroti.Generated.Framework.Widgets.Padding(padding: global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateAll(8), child: new global::Doroti.Generated.Framework.Widgets.Align(alignment: global::Doroti.Generated.Framework.Painting.AlignmentDirectional.centerEnd, child: new global::Doroti.Generated.Framework.Widgets.OverflowBar(spacing: 8, overflowAlignment: global::Doroti.Generated.Framework.Widgets.OverflowBarAlignment.end, children: ((_MasterDetailScaffold__about)this.widget).actionBuilder!(context, _ActionLevel__about.view))))))) }))), body: new global::Doroti.Generated.Framework.Widgets.Align(alignment: global::Doroti.Generated.Framework.Painting.AlignmentDirectional.centerStart, child: _masterPanel(context)))), DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.SafeArea(child: new global::Doroti.Generated.Framework.Widgets.Padding(padding: global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: (this.masterViewWidth - AboutLibrary._kCardElevation), end: this.detailPageFABlessGutterWidth), child: new global::Doroti.Generated.Framework.Widgets.ValueListenableBuilder<object?>(valueListenable: this._detailArguments, builder: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, object?, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>)((context, value, child) => {
return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.AnimatedSwitcher(transitionBuilder: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.Widget, global::Doroti.Generated.Framework.Animation.Animation<double>, global::Doroti.Generated.Framework.Widgets.Widget>)((child, animation) => new global::Doroti.Generated.Framework.Widgets.FadeUpwardsPageTransitionsBuilder().buildTransitions<object?>(null, null, animation, null, child))), duration: Duration.Create(milliseconds: 500L), child: global::Doroti.Generated.Framework.Widgets.SizedBox.CreateExpand(key: new global::Doroti.Generated.Framework.Foundation.ValueKey<object?>((value ?? ((_MasterDetailScaffold__about)this.widget).initialArguments)), child: new _DetailView__about(builder: (global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, object, global::Doroti.Generated.Framework.Widgets.ScrollController?, global::Doroti.Generated.Framework.Widgets.Widget>)((_MasterDetailScaffold__about)this.widget).detailPageBuilder, arguments: (value ?? ((_MasterDetailScaffold__about)this.widget).initialArguments)))));
throw new InvalidOperationException("Dart closure completed without a value.");
})))))) }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Generated.Framework.Widgets.ConstrainedBox _masterPanel(global::Doroti.Generated.Framework.Widgets.BuildContext context, bool needsScaffold = false)
    {
        return new global::Doroti.Generated.Framework.Widgets.ConstrainedBox(constraints: new global::Doroti.Generated.Framework.Rendering.BoxConstraints(maxWidth: this.masterViewWidth), child: (needsScaffold ? new Scaffold(appBar: new AppBar(title: ((_MasterDetailScaffold__about)this.widget).title, actions: ((_MasterDetailScaffold__about)this.widget).actionBuilder!(context, _ActionLevel__about.top)), body: this.widget.masterViewBuilder(context, true)) : this.widget.masterViewBuilder(context, true)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _DetailView__about : global::Doroti.Generated.Framework.Widgets.StatelessWidget
{
    internal virtual global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, object, global::Doroti.Generated.Framework.Widgets.ScrollController?, global::Doroti.Generated.Framework.Widgets.Widget> _builder { get; private set; } = default!;
    internal virtual object? _arguments { get; private set; }

    internal _DetailView__about(global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, object, global::Doroti.Generated.Framework.Widgets.ScrollController?, global::Doroti.Generated.Framework.Widgets.Widget> builder, object? arguments = null)
    {
        this._builder = builder;
        this._arguments = arguments;
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        if ((this._arguments is null))
        {
            return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)global::Doroti.Generated.Framework.Widgets.SizedBox.CreateShrink());
        }
        double screenHeight__56694 = MediaQuery.heightOf(context);
        double minHeight__56756 = (((screenHeight__56694 - ConstantsLibrary.kToolbarHeight)) / screenHeight__56694);
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.DraggableScrollableSheet(initialChildSize: minHeight__56756, minChildSize: minHeight__56756, expand: false, builder: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.ScrollController, global::Doroti.Generated.Framework.Widgets.Widget>)((context, controller) => {
return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.MouseRegion(child: new Card(color: Theme.of(context).cardColor, elevation: AboutLibrary._kCardElevation, clipBehavior: Clip.antiAlias, margin: new global::Doroti.Generated.Framework.Painting.EdgeInsets(AboutLibrary._kCardElevation, 0.0, AboutLibrary._kCardElevation, 0.0), shape: new global::Doroti.Generated.Framework.Painting.RoundedRectangleBorder(borderRadius: global::Doroti.Generated.Framework.Painting.BorderRadius.CreateVertical(top: global::Doroti.Ui.Radius.circular(3.0))), child: this._builder(context, this._arguments, controller))));
throw new InvalidOperationException("Dart closure completed without a value.");
}))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
