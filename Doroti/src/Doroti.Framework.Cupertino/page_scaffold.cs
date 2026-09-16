// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/page_scaffold.dart

using Doroti.Runtime;
using Doroti.Ui;

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
    internal virtual global::Doroti.Framework.Widgets.GlobalKey<IState> _statusBarKey { get; private set; } = GlobalKey<IState>.Create();

    public override void initState()
    {
        base.initState();
        WidgetsBinding.instance.addObserver(this);
    }

    public override void deactivate()
    {
        WidgetsBinding.instance.removeObserver(this);
        base.deactivate();
    }

    public override void activate()
    {
        base.activate();
        WidgetsBinding.instance.addObserver(this);
    }

    public virtual void handleStatusBarTap()
    {
        global::Doroti.Framework.Widgets.ScrollController? primaryScrollController = PrimaryScrollController.maybeOf(context);
        if ((primaryScrollController is not null) && primaryScrollController.hasClients && _HitTestableAtOrigin__page_scaffold.hitTestableAtOrigin(_statusBarKey))
        {
            DartRuntimePrimitives.Ignore(primaryScrollController.animateTo(0.0, duration: Duration.Create(milliseconds: 500L), curve: Curves.linearToEaseOut));
        }
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Framework.Widgets.Widget paddedContent = widget.child;
        global::Doroti.Ui.Color backgroundColorLocal = CupertinoDynamicColor.maybeResolve(widget.backgroundColor, context) ?? CupertinoTheme.of(context).scaffoldBackgroundColor;
        global::Doroti.Framework.Widgets.MediaQueryData existingMediaQuery = MediaQuery.of(context);
        if (widget.navigationBar is not null)
        {
            double topPadding = widget.navigationBar!.preferredSize.height + existingMediaQuery.padding.top;
            double bottomPadding = widget.resizeToAvoidBottomInset ? existingMediaQuery.viewInsets.bottom : 0.0;
            global::Doroti.Framework.Painting.EdgeInsets newViewInsets = widget.resizeToAvoidBottomInset ? existingMediaQuery.viewInsets.copyWith(bottom: 0.0) : existingMediaQuery.viewInsets;
            bool fullObstruction = widget.navigationBar!.shouldFullyObstruct(context);
            if (fullObstruction)
            {
                paddedContent = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.MediaQuery(data: existingMediaQuery.removePadding(removeTop: true).copyWith(viewInsets: newViewInsets), child: new global::Doroti.Framework.Widgets.Padding(padding: EdgeInsets.CreateOnly(top: topPadding, bottom: bottomPadding), child: paddedContent)));
            }
            else
            {
                paddedContent = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.MediaQuery(data: existingMediaQuery.copyWith(padding: existingMediaQuery.padding.copyWith(top: topPadding), viewInsets: newViewInsets), child: new global::Doroti.Framework.Widgets.Padding(padding: EdgeInsets.CreateOnly(bottom: bottomPadding), child: paddedContent)));
            }
        }
        else
        {
            if (widget.resizeToAvoidBottomInset)
            {
                paddedContent = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.MediaQuery(data: existingMediaQuery.copyWith(viewInsets: existingMediaQuery.viewInsets.copyWith(bottom: 0)), child: new global::Doroti.Framework.Widgets.Padding(padding: EdgeInsets.CreateOnly(bottom: existingMediaQuery.viewInsets.bottom), child: paddedContent)));
            }
        }
        return new global::Doroti.Framework.Widgets.ScrollNotificationObserver(child: new global::Doroti.Framework.Widgets.DecoratedBox(decoration: new global::Doroti.Framework.Painting.BoxDecoration(color: backgroundColorLocal), child: new CupertinoPageScaffoldBackgroundColor(color: backgroundColorLocal, child: new global::Doroti.Framework.Widgets.Stack(children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection8262 = new List<global::Doroti.Framework.Widgets.Widget>(); __collection8262.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(paddedContent)); if (widget.navigationBar is not null) { __collection8262.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Positioned(top: 0.0, left: 0.0, right: 0.0, child: MediaQuery.withNoTextScaling(child: DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(widget.navigationBar!))))); } __collection8262.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Positioned(top: 0.0, left: 0.0, right: 0.0, height: existingMediaQuery.padding.top, child: new _HitTestableAtOrigin__page_scaffold(_statusBarKey)))); return __collection8262; }))()))));
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
        var __oldWidget = (CupertinoPageScaffoldBackgroundColor)oldWidget;
        return !Equals(color, __oldWidget.color);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static global::Doroti.Ui.Color? maybeOf(global::Doroti.Framework.Widgets.BuildContext context)
    {
        CupertinoPageScaffoldBackgroundColor? scaffoldBackgroundColor = context.dependOnInheritedWidgetOfExactType<CupertinoPageScaffoldBackgroundColor>();
        return scaffoldBackgroundColor?.color;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("page scaffold background color", color));
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
        var context = ((global::Doroti.Framework.Widgets.Element?)key.currentContext)!;
        if (context is null)
        {
            DartRuntimePrimitives.Assert(() => false, () => (object?)$"BuildContext associated with {key} is not mounted.");
            return false;
        }
        var renderObjectLocal = ((global::Doroti.Framework.Rendering.RenderMetaData?)context.renderObject!)!;
        long viewIdLocal = checked((long)View.of(context).viewId);
        var result = new global::Doroti.Framework.Gestures.HitTestResult();
        WidgetsBinding.instance.hitTestInView(result, Offset.zero, viewIdLocal);
        return result.path.any((entry) => Equals(entry.target, renderObjectLocal));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return new global::Doroti.Framework.Widgets.MetaData(key: globalKey, behavior: HitTestBehavior.translucent, child: SizedBox.CreateExpand());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
