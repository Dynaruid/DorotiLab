// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/page_scaffold.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Cupertino;

public class CupertinoPageScaffold : StatefulWidget
{
    public virtual ObstructingPreferredSizeWidget? navigationBar { get; private set; }
    public virtual Widget child { get; private set; } = default!;
    public virtual Color? backgroundColor { get; private set; }
    public virtual bool resizeToAvoidBottomInset { get; private set; } = default!;

    public CupertinoPageScaffold(Key? key = null, ObstructingPreferredSizeWidget? navigationBar = null, Color? backgroundColor = null, bool resizeToAvoidBottomInset = true, Widget child = default!) : base(key: key)
    {
        this.navigationBar = navigationBar;
        this.backgroundColor = backgroundColor;
        this.resizeToAvoidBottomInset = resizeToAvoidBottomInset;
        this.child = child;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoPageScaffoldState__page_scaffold());
}

internal class _CupertinoPageScaffoldState__page_scaffold : State<CupertinoPageScaffold>, WidgetsBindingObserver
{
    internal virtual GlobalKey<IState> _statusBarKey { get; private set; } = GlobalKey<IState>.Create();

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
        ScrollController? primaryScrollController = PrimaryScrollController.maybeOf(context);
        if ((primaryScrollController is not null) && primaryScrollController.hasClients && _HitTestableAtOrigin__page_scaffold.hitTestableAtOrigin(_statusBarKey))
        {
            DartRuntimePrimitives.Ignore(primaryScrollController.animateTo(0.0, duration: Duration.Create(milliseconds: 500L), curve: Curves.linearToEaseOut));
        }
    }

    public override Widget build(BuildContext context)
    {
        Widget paddedContent = widget.child;
        Color backgroundColorLocal = CupertinoDynamicColor.maybeResolve(widget.backgroundColor, context) ?? CupertinoTheme.of(context).scaffoldBackgroundColor;
        MediaQueryData existingMediaQuery = MediaQuery.of(context);
        if (widget.navigationBar is not null)
        {
            double topPadding = widget.navigationBar!.preferredSize.height + existingMediaQuery.padding.top;
            double bottomPadding = widget.resizeToAvoidBottomInset ? existingMediaQuery.viewInsets.bottom : 0.0;
            EdgeInsets newViewInsets = widget.resizeToAvoidBottomInset ? existingMediaQuery.viewInsets.copyWith(bottom: 0.0) : existingMediaQuery.viewInsets;
            bool fullObstruction = widget.navigationBar!.shouldFullyObstruct(context);
            if (fullObstruction)
            {
                paddedContent = DartRuntimePrimitives.ConvertValue<Widget>(new MediaQuery(data: existingMediaQuery.removePadding(removeTop: true).copyWith(viewInsets: newViewInsets), child: new Padding(padding: EdgeInsets.CreateOnly(top: topPadding, bottom: bottomPadding), child: paddedContent)));
            }
            else
            {
                paddedContent = DartRuntimePrimitives.ConvertValue<Widget>(new MediaQuery(data: existingMediaQuery.copyWith(padding: existingMediaQuery.padding.copyWith(top: topPadding), viewInsets: newViewInsets), child: new Padding(padding: EdgeInsets.CreateOnly(bottom: bottomPadding), child: paddedContent)));
            }
        }
        else
        {
            if (widget.resizeToAvoidBottomInset)
            {
                paddedContent = DartRuntimePrimitives.ConvertValue<Widget>(new MediaQuery(data: existingMediaQuery.copyWith(viewInsets: existingMediaQuery.viewInsets.copyWith(bottom: 0)), child: new Padding(padding: EdgeInsets.CreateOnly(bottom: existingMediaQuery.viewInsets.bottom), child: paddedContent)));
            }
        }
        return new ScrollNotificationObserver(child: new DecoratedBox(decoration: new BoxDecoration(color: backgroundColorLocal), child: new CupertinoPageScaffoldBackgroundColor(color: backgroundColorLocal, child: new Stack(children: ((Func<List<Widget>>)(() => { var __collection8262 = new List<Widget>(); __collection8262.Add(DartRuntimePrimitives.ConvertValue<Widget>(paddedContent)); if (widget.navigationBar is not null) { __collection8262.Add(DartRuntimePrimitives.ConvertValue<Widget>(new Positioned(top: 0.0, left: 0.0, right: 0.0, child: MediaQuery.withNoTextScaling(child: DartRuntimePrimitives.ConvertValue<Widget>(widget.navigationBar!))))); } __collection8262.Add(DartRuntimePrimitives.ConvertValue<Widget>(new Positioned(top: 0.0, left: 0.0, right: 0.0, height: existingMediaQuery.padding.top, child: new _HitTestableAtOrigin__page_scaffold(_statusBarKey)))); return __collection8262; }))()))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class CupertinoPageScaffoldBackgroundColor : InheritedWidget
{
    public virtual Color color { get; private set; } = default!;

    public CupertinoPageScaffoldBackgroundColor(Widget child, Color color, Key? key = null) : base(child: child, key: key)
    {
        this.color = color;
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget)
    {
        var __oldWidget = (CupertinoPageScaffoldBackgroundColor)oldWidget;
        return !Equals(color, __oldWidget.color);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static Color? maybeOf(BuildContext context)
    {
        CupertinoPageScaffoldBackgroundColor? scaffoldBackgroundColor = context.dependOnInheritedWidgetOfExactType<CupertinoPageScaffoldBackgroundColor>();
        return scaffoldBackgroundColor?.color;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new ColorProperty("page scaffold background color", color));
    }

}

public interface ObstructingPreferredSizeWidget : PreferredSizeWidget
{
    public bool shouldFullyObstruct(BuildContext context);
}

internal class _HitTestableAtOrigin__page_scaffold : StatelessWidget
{
    public virtual GlobalKey<IState> globalKey { get; private set; } = default!;

    internal _HitTestableAtOrigin__page_scaffold(GlobalKey<IState> globalKey)
    {
        this.globalKey = globalKey;
    }

    public static bool hitTestableAtOrigin(GlobalKey<IState> key)
    {
        var context = ((Element?)key.currentContext)!;
        if (context is null)
        {
            DartRuntimePrimitives.Assert(() => false, () => (object?)$"BuildContext associated with {key} is not mounted.");
            return false;
        }
        var renderObjectLocal = ((RenderMetaData?)context.renderObject!)!;
        long viewIdLocal = checked((long)View.of(context).viewId);
        var result = new Gestures.HitTestResult();
        WidgetsBinding.instance.hitTestInView(result, Offset.zero, viewIdLocal);
        return result.path.any((entry) => Equals(entry.target, renderObjectLocal));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        return new MetaData(key: globalKey, behavior: HitTestBehavior.translucent, child: SizedBox.CreateExpand());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
