// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/search.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Material;

public static partial class SearchLibrary
{
    public static Future<T?> showSearch<T>(global::Doroti.Framework.Widgets.BuildContext context, SearchDelegate<T> @delegate, string? query = "", bool useRootNavigator = false, bool maintainState = false)
    {
        @delegate.query = ((query ?? (string)((SearchDelegate<T>)@delegate).query));
        @delegate._currentBody = _SearchBody__search.suggestions;
        return ((Future<T?>)(object?)Navigator.of(context, rootNavigator: useRootNavigator).push(new _SearchPageRoute__search<T>(@delegate: @delegate, maintainState: maintainState)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public abstract class SearchDelegate<T>
{
    public virtual bool? automaticallyImplyLeading { get; set; } = default;
    public virtual double? leadingWidth { get; set; } = default;
    public virtual string? searchFieldLabel { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? searchFieldStyle { get; private set; }
    public virtual InputDecorationTheme? searchFieldDecorationTheme { get; private set; }
    public virtual global::Doroti.Framework.Services.TextInputType? keyboardType { get; private set; }
    public virtual bool autocorrect { get; private set; } = default!;
    public virtual bool enableSuggestions { get; private set; } = default!;
    public virtual global::Doroti.Framework.Services.TextInputAction textInputAction { get; private set; } = default!;
    internal virtual global::Doroti.Framework.Widgets.FocusNode? _focusNode { get; set; } = default;
    internal virtual global::Doroti.Framework.Widgets.TextEditingController _queryTextController { get; private set; } = new global::Doroti.Framework.Widgets.TextEditingController();
    internal virtual global::Doroti.Framework.Animation.ProxyAnimation _proxyAnimation { get; private set; } = new global::Doroti.Framework.Animation.ProxyAnimation(global::Doroti.Framework.Animation.AnimationsLibrary.kAlwaysDismissedAnimation);
    internal virtual global::Doroti.Framework.Foundation.ValueNotifier<_SearchBody__search?> _currentBodyNotifier { get; private set; } = new global::Doroti.Framework.Foundation.ValueNotifier<_SearchBody__search?>(null);
    internal virtual _SearchPageRoute__search<T>? _route { get; set; } = default;

    protected SearchDelegate(string? searchFieldLabel = null, global::Doroti.Framework.Painting.TextStyle? searchFieldStyle = null, InputDecorationTheme? searchFieldDecorationTheme = null, global::Doroti.Framework.Services.TextInputType? keyboardType = null, global::Doroti.Framework.Services.TextInputAction textInputAction = global::Doroti.Framework.Services.TextInputAction.search, bool autocorrect = true, bool enableSuggestions = true)
    {
        this.searchFieldLabel = searchFieldLabel;
        this.searchFieldStyle = searchFieldStyle;
        this.searchFieldDecorationTheme = searchFieldDecorationTheme;
        this.keyboardType = keyboardType;
        this.textInputAction = textInputAction;
        this.autocorrect = autocorrect;
        this.enableSuggestions = enableSuggestions;
        System.Diagnostics.Debug.Assert(((searchFieldStyle is null) || (searchFieldDecorationTheme is null)));
    }

    public abstract global::Doroti.Framework.Widgets.Widget buildSuggestions(global::Doroti.Framework.Widgets.BuildContext context);
    public abstract global::Doroti.Framework.Widgets.Widget buildResults(global::Doroti.Framework.Widgets.BuildContext context);
    public abstract global::Doroti.Framework.Widgets.Widget? buildLeading(global::Doroti.Framework.Widgets.BuildContext context);
    public abstract List<global::Doroti.Framework.Widgets.Widget>? buildActions(global::Doroti.Framework.Widgets.BuildContext context);
    public virtual global::Doroti.Framework.Widgets.PreferredSizeWidget? buildBottom(global::Doroti.Framework.Widgets.BuildContext context) => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.PreferredSizeWidget>(null);
    public virtual global::Doroti.Framework.Widgets.Widget? buildFlexibleSpace(global::Doroti.Framework.Widgets.BuildContext context) => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(null);
    public virtual ThemeData appBarTheme(global::Doroti.Framework.Widgets.BuildContext context)
    {
        ThemeData theme = Theme.of(context);
        ColorScheme colorSchemeLocal = theme.colorScheme;
        return theme.copyWith(appBarTheme: new AppBarThemeData(systemOverlayStyle: ((object.Equals(colorSchemeLocal.brightness, Brightness.dark)) ? global::Doroti.Framework.Services.SystemUiOverlayStyle.light : global::Doroti.Framework.Services.SystemUiOverlayStyle.dark), backgroundColor: ((object.Equals(colorSchemeLocal.brightness, Brightness.dark)) ? Colors.grey[900L] : Colors.white), iconTheme: theme.primaryIconTheme.copyWith(color: Colors.grey), titleTextStyle: theme.textTheme.titleLarge, toolbarTextStyle: theme.textTheme.bodyMedium), inputDecorationTheme: (this.searchFieldDecorationTheme ?? new InputDecorationTheme(hintStyle: (this.searchFieldStyle ?? theme.inputDecorationTheme.hintStyle), border: InputBorder.none)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string query
    {
        get => ((global::Doroti.Framework.Widgets.TextEditingController)this._queryTextController).text;
        set
        {
            var __value = value;
            this._queryTextController.value = new global::Doroti.Framework.Services.TextEditingValue(text: __value, selection: global::Doroti.Framework.Services.TextSelection.CreateCollapsed(offset: __value.Length));
        }
    }
    public virtual void showResults(global::Doroti.Framework.Widgets.BuildContext context)
    {
        this._focusNode?.unfocus();
        _currentBody = _SearchBody__search.results;
    }

    public virtual void showSuggestions(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => (this._focusNode is not null), () => (object?)"_focusNode must be set by route before showSuggestions is called.");
        this._focusNode!.requestFocus();
        _currentBody = _SearchBody__search.suggestions;
    }

    public virtual void close(global::Doroti.Framework.Widgets.BuildContext context, T result)
    {
        _currentBody = null;
        this._focusNode?.unfocus();
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Widgets.NavigatorState>)(() =>
{
    var __cascade = Navigator.of(context);
    __cascade.popUntil(((global::System.Func<dynamic, bool>)((route) => (object.Equals(route, this._route)))));
    __cascade.pop(result);
    return __cascade;
}))());
    }

    internal virtual void _pop(global::Doroti.Framework.Widgets.BuildContext context)
    {
        _currentBody = null;
        this._focusNode?.unfocus();
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Widgets.NavigatorState>)(() =>
{
    var __cascade = Navigator.of(context);
    __cascade.popUntil(((global::System.Func<dynamic, bool>)((route) => (object.Equals(route, this._route)))));
    __cascade.pop<object>(null);
    return __cascade;
}))());
    }

    public virtual global::Doroti.Framework.Animation.Animation<double> transitionAnimation => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Animation.Animation<double>>(this._proxyAnimation);
    internal virtual _SearchBody__search? _currentBody
    {
        get => ((global::Doroti.Framework.Foundation.ValueNotifier<_SearchBody__search?>)this._currentBodyNotifier).value;
        set
        {
            var __value = value;
            this._currentBodyNotifier.value = __value;
        }
    }
    public virtual void dispose()
    {
        this._currentBodyNotifier.dispose();
        this._focusNode?.dispose();
        this._queryTextController.dispose();
        this._proxyAnimation.parent = null;
    }

}

internal enum _SearchBody__search
{
    suggestions,
    results
}

internal class _SearchPageRoute__search<T> : global::Doroti.Framework.Widgets.PageRoute<T>
{
    public virtual SearchDelegate<T> @delegate { get; private set; } = default!;
    private bool __field_maintainState = default!;
    public override bool maintainState { get => __field_maintainState; }

    internal _SearchPageRoute__search(SearchDelegate<T> @delegate, bool maintainState)
    {
        this.@delegate = @delegate;
        this.__field_maintainState = maintainState;
        DartRuntimePrimitives.Assert(() => (((SearchDelegate<T>)this.@delegate)._route is null), () => (object?)$"The {DartRuntimePrimitives.RuntimeType(this.@delegate)} instance is currently used by another active " + "search. Please close that search by calling close() on the SearchDelegate " + "before opening another search with the same delegate instance.");
        this.@delegate._route = this;
    }

    public override Color? barrierColor => DartRuntimePrimitives.ConvertValue<Color>(null);
    public override string? barrierLabel => DartRuntimePrimitives.ConvertValue<string>(null);
    public override Duration transitionDuration => Duration.Create(milliseconds: 300L);
    public override global::Doroti.Framework.Widgets.Widget buildTransitions(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation, global::Doroti.Framework.Animation.Animation<double> secondaryAnimation, global::Doroti.Framework.Widgets.Widget child)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.FadeTransition(opacity: animation, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Animation.Animation<double> createAnimation()
    {
        global::Doroti.Framework.Animation.Animation<double> animation = ((global::Doroti.Framework.Animation.Animation<double>)(object?)base.createAnimation());
        ((SearchDelegate<T>)this.@delegate)._proxyAnimation.parent = animation;
        return animation;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget buildPage(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation, global::Doroti.Framework.Animation.Animation<double> secondaryAnimation)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new _SearchPage__search<T>(@delegate: this.@delegate, animation: animation));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void didComplete(T? result)
    {
        base.didComplete(result);
        DartRuntimePrimitives.Assert(() => (object.Equals(((SearchDelegate<T>)this.@delegate)._route, this)));
        this.@delegate._route = null;
        this.@delegate._currentBody = null;
    }

}

public class _SearchPage__search<T> : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual SearchDelegate<T> @delegate { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.Animation<double> animation { get; private set; } = default!;

    internal _SearchPage__search(SearchDelegate<T> @delegate, global::Doroti.Framework.Animation.Animation<double> animation)
    {
        this.@delegate = @delegate;
        this.animation = animation;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _SearchPageState__search<T>());
}

internal class _SearchPageState__search<T> : global::Doroti.Framework.Widgets.State<_SearchPage__search<T>>
{
    private bool __late_focusNode_initialized;
    private global::Doroti.Framework.Widgets.FocusNode __late_focusNode = default!;
    public virtual global::Doroti.Framework.Widgets.FocusNode focusNode
    {
        get
        {
            if (!__late_focusNode_initialized)
            {
                __late_focusNode = new global::Doroti.Framework.Widgets.FocusNode(onKeyEvent: ((global::System.Func<global::Doroti.Framework.Widgets.FocusNode, global::Doroti.Framework.Services.KeyEvent, global::Doroti.Framework.Widgets.KeyEventResult>?)((node, @event) =>
                {
                    if (((@event is global::Doroti.Framework.Services.KeyDownEvent) && (object.Equals(((global::Doroti.Framework.Services.KeyDownEvent)@event).logicalKey, global::Doroti.Framework.Services.LogicalKeyboardKey.escape))))
                    {
                        ((_SearchPage__search<T>)(object)this.widget).@delegate._pop(this.context);
                        return global::Doroti.Framework.Widgets.KeyEventResult.handled;
                    }
                    return global::Doroti.Framework.Widgets.KeyEventResult.ignored;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                })));
                __late_focusNode_initialized = true;
            }
            return __late_focusNode;
        }
    }

    public override void initState()
    {
        base.initState();
        ((_SearchPage__search<T>)(object)this.widget).@delegate._queryTextController.addListener(() => this._onQueryChanged());
        ((_SearchPage__search<T>)(object)this.widget).animation.addStatusListener((AnimationStatusListener)this._onAnimationStatusChanged);
        ((_SearchPage__search<T>)(object)this.widget).@delegate._currentBodyNotifier.addListener(() => this._onSearchBodyChanged());
        this.focusNode.addListener(() => this._onFocusChanged());
        ((_SearchPage__search<T>)(object)this.widget).@delegate._focusNode = this.focusNode;
    }

    public override void dispose()
    {
        base.dispose();
        ((_SearchPage__search<T>)(object)this.widget).@delegate._queryTextController.removeListener(() => this._onQueryChanged());
        ((_SearchPage__search<T>)(object)this.widget).animation.removeStatusListener((AnimationStatusListener)this._onAnimationStatusChanged);
        ((_SearchPage__search<T>)(object)this.widget).@delegate._currentBodyNotifier.removeListener(() => this._onSearchBodyChanged());
        ((_SearchPage__search<T>)(object)this.widget).@delegate._focusNode = null;
        this.focusNode.dispose();
    }

    internal virtual void _onAnimationStatusChanged(global::Doroti.Framework.Animation.AnimationStatus status)
    {
        if (!global::Doroti.Framework.Animation.AnimationStatusMembers.isCompleted(status))
        {
            return;
        }
        ((_SearchPage__search<T>)(object)this.widget).animation.removeStatusListener((AnimationStatusListener)this._onAnimationStatusChanged);
        if ((object.Equals(((_SearchPage__search<T>)(object)this.widget).@delegate._currentBody, _SearchBody__search.suggestions)))
        {
            this.focusNode.requestFocus();
        }
    }

    public override void didUpdateWidget(_SearchPage__search<T> oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((!object.Equals(((_SearchPage__search<T>)(object)this.widget).@delegate, ((_SearchPage__search<T>)oldWidget).@delegate)))
        {
            ((_SearchPage__search<T>)oldWidget).@delegate._queryTextController.removeListener(() => this._onQueryChanged());
            ((_SearchPage__search<T>)(object)this.widget).@delegate._queryTextController.addListener(() => this._onQueryChanged());
            ((_SearchPage__search<T>)oldWidget).@delegate._currentBodyNotifier.removeListener(() => this._onSearchBodyChanged());
            ((_SearchPage__search<T>)(object)this.widget).@delegate._currentBodyNotifier.addListener(() => this._onSearchBodyChanged());
            ((_SearchPage__search<T>)oldWidget).@delegate._focusNode = null;
            ((_SearchPage__search<T>)(object)this.widget).@delegate._focusNode = this.focusNode;
        }
    }

    internal virtual void _onFocusChanged()
    {
        if ((((global::Doroti.Framework.Widgets.FocusNode)this.focusNode).hasFocus && (!object.Equals(((_SearchPage__search<T>)(object)this.widget).@delegate._currentBody, _SearchBody__search.suggestions))))
        {
            ((_SearchPage__search<T>)(object)this.widget).@delegate.showSuggestions(this.context);
        }
    }

    internal virtual void _onQueryChanged()
    {
        setState(((global::System.Action)(() =>
        {
        })));
    }

    internal virtual void _onSearchBodyChanged()
    {
        setState(((global::System.Action)(() =>
        {
        })));
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterialLocalizations(context));
        ThemeData theme = ((ThemeData)(object?)((_SearchPage__search<T>)(object)this.widget).@delegate.appBarTheme(context));
        string searchFieldLabelLocal = (((_SearchPage__search<T>)(object)this.widget).@delegate.searchFieldLabel ?? MaterialLocalizations.of(context).searchFieldLabel);
        global::Doroti.Framework.Widgets.Widget? bodyLocal = default!;
        switch (((_SearchPage__search<T>)(object)this.widget).@delegate._currentBody)
        {
            case _SearchBody__search.suggestions:
                {
                    bodyLocal = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.KeyedSubtree(key: new global::Doroti.Framework.Foundation.ValueKey<_SearchBody__search>(_SearchBody__search.suggestions), child: ((_SearchPage__search<T>)(object)this.widget).@delegate.buildSuggestions(context)));
                    break;
                }
            case _SearchBody__search.results:
                {
                    bodyLocal = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.KeyedSubtree(key: new global::Doroti.Framework.Foundation.ValueKey<_SearchBody__search>(_SearchBody__search.results), child: ((_SearchPage__search<T>)(object)this.widget).@delegate.buildResults(context)));
                    break;
                }
            case null:
                {
                    break;
                }
        }
        string routeName = default!;
        switch (theme.platform)
        {
            case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
            case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
                {
                    routeName = "";
                    break;
                }
            case global::Doroti.Framework.Foundation.TargetPlatform.android:
            case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
            case global::Doroti.Framework.Foundation.TargetPlatform.linux:
            case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                {
                    routeName = searchFieldLabelLocal;
                    break;
                }
        }
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Semantics(explicitChildNodes: true, scopesRoute: true, namesRoute: true, label: routeName, child: new Theme(data: theme, child: new Scaffold(appBar: new AppBar(leadingWidth: ((_SearchPage__search<T>)(object)this.widget).@delegate.leadingWidth, automaticallyImplyLeading: (((_SearchPage__search<T>)(object)this.widget).@delegate.automaticallyImplyLeading ?? true), leading: ((_SearchPage__search<T>)(object)this.widget).@delegate.buildLeading(context), title: new global::Doroti.Framework.Widgets.Semantics(inputType: SemanticsInputType.search, child: new TextField(controller: ((_SearchPage__search<T>)(object)this.widget).@delegate._queryTextController, focusNode: this.focusNode, style: (((_SearchPage__search<T>)(object)this.widget).@delegate.searchFieldStyle ?? theme.textTheme.titleLarge), textInputAction: ((_SearchPage__search<T>)(object)this.widget).@delegate.textInputAction, autocorrect: ((_SearchPage__search<T>)(object)this.widget).@delegate.autocorrect, enableSuggestions: ((_SearchPage__search<T>)(object)this.widget).@delegate.enableSuggestions, keyboardType: ((_SearchPage__search<T>)(object)this.widget).@delegate.keyboardType, onSubmitted: ((global::System.Action<string>)((_) => { ((_SearchPage__search<T>)(object)this.widget).@delegate.showResults(context); })), decoration: new InputDecoration(hintText: searchFieldLabelLocal))), flexibleSpace: ((_SearchPage__search<T>)(object)this.widget).@delegate.buildFlexibleSpace(context), actions: ((_SearchPage__search<T>)(object)this.widget).@delegate.buildActions(context), bottom: ((_SearchPage__search<T>)(object)this.widget).@delegate.buildBottom(context)), body: new global::Doroti.Framework.Widgets.AnimatedSwitcher(duration: Duration.Create(milliseconds: 300L), child: bodyLocal)))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
