// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../flutter-master/packages/flutter/lib/src/cupertino/page_scaffold.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Cupertino;

public class CupertinoPageScaffold : global::Doroti.Generated.Framework.Widgets.StatefulWidget
{
    public virtual ObstructingPreferredSizeWidget? navigationBar { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget child { get; private set; } = default!;
    public virtual Color? backgroundColor { get; private set; }
    public virtual bool resizeToAvoidBottomInset { get; private set; } = default!;

    public CupertinoPageScaffold(global::Doroti.Generated.Framework.Foundation.Key? key = null, ObstructingPreferredSizeWidget? navigationBar = null, Color? backgroundColor = null, bool resizeToAvoidBottomInset = true, global::Doroti.Generated.Framework.Widgets.Widget child = default!) : base(key: key)
    {
        this.navigationBar = navigationBar;
        this.backgroundColor = backgroundColor;
        this.resizeToAvoidBottomInset = resizeToAvoidBottomInset;
        this.child = child;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoPageScaffoldState__page_scaffold());
}

internal class _CupertinoPageScaffoldState__page_scaffold : global::Doroti.Generated.Framework.Widgets.State<CupertinoPageScaffold>, global::Doroti.Generated.Framework.Widgets.WidgetsBindingObserver
{
    internal virtual global::Doroti.Generated.Framework.Widgets.GlobalKey<IState> _statusBarKey { get; private set; } = global::Doroti.Generated.Framework.Widgets.GlobalKey<IState>.Create();

    public override void initState()
    {
        base.initState();
        global::Doroti.Generated.Framework.Widgets.WidgetsBinding.instance.addObserver(this);
    }

    public override void deactivate()
    {
        global::Doroti.Generated.Framework.Widgets.WidgetsBinding.instance.removeObserver(this);
        base.deactivate();
    }

    public override void activate()
    {
        base.activate();
        global::Doroti.Generated.Framework.Widgets.WidgetsBinding.instance.addObserver(this);
    }

    public virtual void handleStatusBarTap()
    {
        global::Doroti.Generated.Framework.Widgets.ScrollController? primaryScrollController__4160 = ((global::Doroti.Generated.Framework.Widgets.ScrollController?)(object?)PrimaryScrollController.maybeOf(this.context));
        if ((((primaryScrollController__4160 is not null) && ((global::Doroti.Generated.Framework.Widgets.ScrollController)primaryScrollController__4160).hasClients) && _HitTestableAtOrigin__page_scaffold.hitTestableAtOrigin(this._statusBarKey)))
        {
            DartRuntimePrimitives.Ignore(primaryScrollController__4160.animateTo(0.0, duration: Duration.Create(milliseconds: 500L), curve: global::Doroti.Generated.Framework.Animation.Curves.linearToEaseOut));
        }
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Generated.Framework.Widgets.Widget paddedContent__5285 = ((CupertinoPageScaffold)this.widget).child;
        global::Doroti.Flutter.Ui.Color backgroundColor__5332 = ((global::Doroti.Flutter.Ui.Color)(object?)(CupertinoDynamicColor.maybeResolve(((CupertinoPageScaffold)this.widget).backgroundColor, context) ?? CupertinoTheme.of(context).scaffoldBackgroundColor));
        global::Doroti.Generated.Framework.Widgets.MediaQueryData existingMediaQuery__5515 = ((global::Doroti.Generated.Framework.Widgets.MediaQueryData)(object?)MediaQuery.of(context));
        if ((((CupertinoPageScaffold)this.widget).navigationBar is not null))
        {
            double topPadding__5760 = (((CupertinoPageScaffold)this.widget).navigationBar!.preferredSize.height + ((global::Doroti.Generated.Framework.Widgets.MediaQueryData)existingMediaQuery__5515).padding.top);
            double bottomPadding__5952 = (((CupertinoPageScaffold)this.widget).resizeToAvoidBottomInset ? ((global::Doroti.Generated.Framework.Widgets.MediaQueryData)existingMediaQuery__5515).viewInsets.bottom : 0.0);
            global::Doroti.Generated.Framework.Painting.EdgeInsets newViewInsets__6090 = (((CupertinoPageScaffold)this.widget).resizeToAvoidBottomInset ? ((global::Doroti.Generated.Framework.Widgets.MediaQueryData)existingMediaQuery__5515).viewInsets.copyWith(bottom: 0.0) : ((global::Doroti.Generated.Framework.Widgets.MediaQueryData)existingMediaQuery__5515).viewInsets);
            bool fullObstruction__6379 = ((CupertinoPageScaffold)this.widget).navigationBar!.shouldFullyObstruct(context);
            if (fullObstruction__6379)
            {
                paddedContent__5285 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.MediaQuery(data: existingMediaQuery__5515.removePadding(removeTop: true).copyWith(viewInsets: newViewInsets__6090), child: new global::Doroti.Generated.Framework.Widgets.Padding(padding: global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateOnly(top: topPadding__5760, bottom: bottomPadding__5952), child: paddedContent__5285)));
            }
            else
            {
                paddedContent__5285 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.MediaQuery(data: existingMediaQuery__5515.copyWith(padding: ((global::Doroti.Generated.Framework.Widgets.MediaQueryData)existingMediaQuery__5515).padding.copyWith(top: topPadding__5760), viewInsets: newViewInsets__6090), child: new global::Doroti.Generated.Framework.Widgets.Padding(padding: global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateOnly(bottom: bottomPadding__5952), child: paddedContent__5285)));
            }
        }
        else
        {
            if (((CupertinoPageScaffold)this.widget).resizeToAvoidBottomInset)
            {
                paddedContent__5285 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.MediaQuery(data: existingMediaQuery__5515.copyWith(viewInsets: ((global::Doroti.Generated.Framework.Widgets.MediaQueryData)existingMediaQuery__5515).viewInsets.copyWith(bottom: 0)), child: new global::Doroti.Generated.Framework.Widgets.Padding(padding: global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateOnly(bottom: ((global::Doroti.Generated.Framework.Widgets.MediaQueryData)existingMediaQuery__5515).viewInsets.bottom), child: paddedContent__5285)));
            }
        }
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.ScrollNotificationObserver(child: new global::Doroti.Generated.Framework.Widgets.DecoratedBox(decoration: new global::Doroti.Generated.Framework.Painting.BoxDecoration(color: backgroundColor__5332), child: new CupertinoPageScaffoldBackgroundColor(color: backgroundColor__5332, child: new global::Doroti.Generated.Framework.Widgets.Stack(children: ((Func<List<global::Doroti.Generated.Framework.Widgets.Widget>>)(() => { var __collection8262 = new List<global::Doroti.Generated.Framework.Widgets.Widget>(); __collection8262.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(paddedContent__5285)); if ((((CupertinoPageScaffold)this.widget).navigationBar is not null)) { __collection8262.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Positioned(top: 0.0, left: 0.0, right: 0.0, child: MediaQuery.withNoTextScaling(child: DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(((CupertinoPageScaffold)this.widget).navigationBar!))))); } __collection8262.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Positioned(top: 0.0, left: 0.0, right: 0.0, height: ((global::Doroti.Generated.Framework.Widgets.MediaQueryData)existingMediaQuery__5515).padding.top, child: new _HitTestableAtOrigin__page_scaffold(this._statusBarKey)))); return __collection8262; }))())))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class CupertinoPageScaffoldBackgroundColor : global::Doroti.Generated.Framework.Widgets.InheritedWidget
{
    public virtual Color color { get; private set; } = default!;

    public CupertinoPageScaffoldBackgroundColor(global::Doroti.Generated.Framework.Widgets.Widget child, Color color, global::Doroti.Generated.Framework.Foundation.Key? key = null) : base(child: child, key: key)
    {
        this.color = color;
    }

    public override bool updateShouldNotify(global::Doroti.Generated.Framework.Widgets.InheritedWidget oldWidget)
    {
        var __oldWidget = (CupertinoPageScaffoldBackgroundColor)(object)oldWidget;
        return (!object.Equals(this.color, ((CupertinoPageScaffoldBackgroundColor)__oldWidget).color));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static global::Doroti.Flutter.Ui.Color? maybeOf(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        CupertinoPageScaffoldBackgroundColor? scaffoldBackgroundColor__10034 = ((CupertinoPageScaffoldBackgroundColor?)(object?)context.dependOnInheritedWidgetOfExactType<CupertinoPageScaffoldBackgroundColor>());
        return ((global::Doroti.Flutter.Ui.Color?)(object?)scaffoldBackgroundColor__10034?.color);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("page scaffold background color", this.color));
    }

}

public interface ObstructingPreferredSizeWidget : global::Doroti.Generated.Framework.Widgets.PreferredSizeWidget
{
    public bool shouldFullyObstruct(global::Doroti.Generated.Framework.Widgets.BuildContext context);
}

internal class _HitTestableAtOrigin__page_scaffold : global::Doroti.Generated.Framework.Widgets.StatelessWidget
{
    public virtual global::Doroti.Generated.Framework.Widgets.GlobalKey<IState> globalKey { get; private set; } = default!;

    internal _HitTestableAtOrigin__page_scaffold(global::Doroti.Generated.Framework.Widgets.GlobalKey<IState> globalKey)
    {
        this.globalKey = globalKey;
    }

    public static bool hitTestableAtOrigin(global::Doroti.Generated.Framework.Widgets.GlobalKey<IState> key)
    {
        var context__11428 = ((global::Doroti.Generated.Framework.Widgets.Element?)(object?)((global::Doroti.Generated.Framework.Widgets.GlobalKey<IState>)key).currentContext)!;
        if ((context__11428 is null))
        {
            DartRuntimePrimitives.Assert(() => false, () => (object?)$"BuildContext associated with {key} is not mounted.");
            return false;
        }
        var renderObject__11607 = ((global::Doroti.Generated.Framework.Rendering.RenderMetaData?)(object?)((global::Doroti.Generated.Framework.Widgets.Element)context__11428).renderObject!)!;
        long viewId__11677 = checked((long)View.of(context__11428).viewId);
        var result__11721 = new global::Doroti.Generated.Framework.Gestures.HitTestResult();
        global::Doroti.Generated.Framework.Widgets.WidgetsBinding.instance.hitTestInView(result__11721, Offset.zero, viewId__11677);
        return ((global::Doroti.Generated.Framework.Gestures.HitTestResult)result__11721).path.any(((entry) => (object.Equals(((global::Doroti.Generated.Framework.Gestures.HitTestEntry<global::Doroti.Generated.Framework.Gestures.HitTestTarget>)entry).target, renderObject__11607))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.MetaData(key: this.globalKey, behavior: global::Doroti.Generated.Framework.Rendering.HitTestBehavior.translucent, child: global::Doroti.Generated.Framework.Widgets.SizedBox.CreateExpand()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
