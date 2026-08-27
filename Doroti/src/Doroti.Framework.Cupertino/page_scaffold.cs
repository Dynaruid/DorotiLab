// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/page_scaffold.dart
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

public class CupertinoPageScaffold : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual ObstructingPreferredSizeWidget? navigationBar { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget child { get; private set; } = default!;
    public virtual Color? backgroundColor { get; private set; }
    public virtual bool resizeToAvoidBottomInset { get; private set; } = default!;

    public CupertinoPageScaffold(global::Doroti.Framework.Foundation.Key? key = null, ObstructingPreferredSizeWidget? navigationBar = null, Color? backgroundColor = null, bool resizeToAvoidBottomInset = true, global::Doroti.Framework.Widgets.Widget child = default!) : base(key: key)
    {
        this.navigationBar = navigationBar;
        this.backgroundColor = backgroundColor;
        this.resizeToAvoidBottomInset = resizeToAvoidBottomInset;
        this.child = child;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoPageScaffoldState__page_scaffold());
}

internal class _CupertinoPageScaffoldState__page_scaffold : global::Doroti.Framework.Widgets.State<CupertinoPageScaffold>, global::Doroti.Framework.Widgets.WidgetsBindingObserver
{
    internal virtual global::Doroti.Framework.Widgets.GlobalKey<IState> _statusBarKey { get; private set; } = global::Doroti.Framework.Widgets.GlobalKey<IState>.Create();

    public override void initState()
    {
        base.initState();
        global::Doroti.Framework.Widgets.WidgetsBinding.instance.addObserver(this);
    }

    public override void deactivate()
    {
        global::Doroti.Framework.Widgets.WidgetsBinding.instance.removeObserver(this);
        base.deactivate();
    }

    public override void activate()
    {
        base.activate();
        global::Doroti.Framework.Widgets.WidgetsBinding.instance.addObserver(this);
    }

    public virtual void handleStatusBarTap()
    {
        global::Doroti.Framework.Widgets.ScrollController? primaryScrollController = ((global::Doroti.Framework.Widgets.ScrollController?)(object?)PrimaryScrollController.maybeOf(this.context));
        if ((((primaryScrollController is not null) && ((global::Doroti.Framework.Widgets.ScrollController)primaryScrollController).hasClients) && _HitTestableAtOrigin__page_scaffold.hitTestableAtOrigin(this._statusBarKey)))
        {
            DartRuntimePrimitives.Ignore(primaryScrollController.animateTo(0.0, duration: Duration.Create(milliseconds: 500L), curve: global::Doroti.Framework.Animation.Curves.linearToEaseOut));
        }
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Framework.Widgets.Widget paddedContent = ((CupertinoPageScaffold)this.widget).child;
        global::Doroti.Ui.Color backgroundColorLocal = ((global::Doroti.Ui.Color)(object?)(CupertinoDynamicColor.maybeResolve(((CupertinoPageScaffold)this.widget).backgroundColor, context) ?? CupertinoTheme.of(context).scaffoldBackgroundColor));
        global::Doroti.Framework.Widgets.MediaQueryData existingMediaQuery = ((global::Doroti.Framework.Widgets.MediaQueryData)(object?)MediaQuery.of(context));
        if ((((CupertinoPageScaffold)this.widget).navigationBar is not null))
        {
            double topPadding = (((CupertinoPageScaffold)this.widget).navigationBar!.preferredSize.height + ((global::Doroti.Framework.Widgets.MediaQueryData)existingMediaQuery).padding.top);
            double bottomPadding = (((CupertinoPageScaffold)this.widget).resizeToAvoidBottomInset ? ((global::Doroti.Framework.Widgets.MediaQueryData)existingMediaQuery).viewInsets.bottom : 0.0);
            global::Doroti.Framework.Painting.EdgeInsets newViewInsets = (((CupertinoPageScaffold)this.widget).resizeToAvoidBottomInset ? ((global::Doroti.Framework.Widgets.MediaQueryData)existingMediaQuery).viewInsets.copyWith(bottom: 0.0) : ((global::Doroti.Framework.Widgets.MediaQueryData)existingMediaQuery).viewInsets);
            bool fullObstruction = ((CupertinoPageScaffold)this.widget).navigationBar!.shouldFullyObstruct(context);
            if (fullObstruction)
            {
                paddedContent = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.MediaQuery(data: existingMediaQuery.removePadding(removeTop: true).copyWith(viewInsets: newViewInsets), child: new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsets.CreateOnly(top: topPadding, bottom: bottomPadding), child: paddedContent)));
            }
            else
            {
                paddedContent = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.MediaQuery(data: existingMediaQuery.copyWith(padding: ((global::Doroti.Framework.Widgets.MediaQueryData)existingMediaQuery).padding.copyWith(top: topPadding), viewInsets: newViewInsets), child: new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsets.CreateOnly(bottom: bottomPadding), child: paddedContent)));
            }
        }
        else
        {
            if (((CupertinoPageScaffold)this.widget).resizeToAvoidBottomInset)
            {
                paddedContent = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.MediaQuery(data: existingMediaQuery.copyWith(viewInsets: ((global::Doroti.Framework.Widgets.MediaQueryData)existingMediaQuery).viewInsets.copyWith(bottom: 0)), child: new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsets.CreateOnly(bottom: ((global::Doroti.Framework.Widgets.MediaQueryData)existingMediaQuery).viewInsets.bottom), child: paddedContent)));
            }
        }
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.ScrollNotificationObserver(child: new global::Doroti.Framework.Widgets.DecoratedBox(decoration: new global::Doroti.Framework.Painting.BoxDecoration(color: backgroundColorLocal), child: new CupertinoPageScaffoldBackgroundColor(color: backgroundColorLocal, child: new global::Doroti.Framework.Widgets.Stack(children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection8262 = new List<global::Doroti.Framework.Widgets.Widget>(); __collection8262.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(paddedContent)); if ((((CupertinoPageScaffold)this.widget).navigationBar is not null)) { __collection8262.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Positioned(top: 0.0, left: 0.0, right: 0.0, child: MediaQuery.withNoTextScaling(child: DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(((CupertinoPageScaffold)this.widget).navigationBar!))))); } __collection8262.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Positioned(top: 0.0, left: 0.0, right: 0.0, height: ((global::Doroti.Framework.Widgets.MediaQueryData)existingMediaQuery).padding.top, child: new _HitTestableAtOrigin__page_scaffold(this._statusBarKey)))); return __collection8262; }))())))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class CupertinoPageScaffoldBackgroundColor : global::Doroti.Framework.Widgets.InheritedWidget
{
    public virtual Color color { get; private set; } = default!;

    public CupertinoPageScaffoldBackgroundColor(global::Doroti.Framework.Widgets.Widget child, Color color, global::Doroti.Framework.Foundation.Key? key = null) : base(child: child, key: key)
    {
        this.color = color;
    }

    public override bool updateShouldNotify(global::Doroti.Framework.Widgets.InheritedWidget oldWidget)
    {
        var __oldWidget = (CupertinoPageScaffoldBackgroundColor)(object)oldWidget;
        return (!object.Equals(this.color, ((CupertinoPageScaffoldBackgroundColor)__oldWidget).color));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static global::Doroti.Ui.Color? maybeOf(global::Doroti.Framework.Widgets.BuildContext context)
    {
        CupertinoPageScaffoldBackgroundColor? scaffoldBackgroundColor = ((CupertinoPageScaffoldBackgroundColor?)(object?)context.dependOnInheritedWidgetOfExactType<CupertinoPageScaffoldBackgroundColor>());
        return ((global::Doroti.Ui.Color?)(object?)scaffoldBackgroundColor?.color);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("page scaffold background color", this.color));
    }

}

public interface ObstructingPreferredSizeWidget : global::Doroti.Framework.Widgets.PreferredSizeWidget
{
    public bool shouldFullyObstruct(global::Doroti.Framework.Widgets.BuildContext context);
}

internal class _HitTestableAtOrigin__page_scaffold : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual global::Doroti.Framework.Widgets.GlobalKey<IState> globalKey { get; private set; } = default!;

    internal _HitTestableAtOrigin__page_scaffold(global::Doroti.Framework.Widgets.GlobalKey<IState> globalKey)
    {
        this.globalKey = globalKey;
    }

    public static bool hitTestableAtOrigin(global::Doroti.Framework.Widgets.GlobalKey<IState> key)
    {
        var context = ((global::Doroti.Framework.Widgets.Element?)(object?)((global::Doroti.Framework.Widgets.GlobalKey<IState>)key).currentContext)!;
        if ((context is null))
        {
            DartRuntimePrimitives.Assert(() => false, () => (object?)$"BuildContext associated with {key} is not mounted.");
            return false;
        }
        var renderObjectLocal = ((global::Doroti.Framework.Rendering.RenderMetaData?)(object?)((global::Doroti.Framework.Widgets.Element)context).renderObject!)!;
        long viewIdLocal = checked((long)View.of(context).viewId);
        var result = new global::Doroti.Framework.Gestures.HitTestResult();
        global::Doroti.Framework.Widgets.WidgetsBinding.instance.hitTestInView(result, Offset.zero, viewIdLocal);
        return ((global::Doroti.Framework.Gestures.HitTestResult)result).path.any(((entry) => (object.Equals(((global::Doroti.Framework.Gestures.HitTestEntry<global::Doroti.Framework.Gestures.HitTestTarget>)entry).target, renderObjectLocal))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.MetaData(key: this.globalKey, behavior: global::Doroti.Framework.Rendering.HitTestBehavior.translucent, child: global::Doroti.Framework.Widgets.SizedBox.CreateExpand()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
