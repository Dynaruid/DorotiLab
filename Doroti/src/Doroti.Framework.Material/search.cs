// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/search.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public static partial class SearchLibrary
{
    public static Future<T?> showSearch<T>(BuildContext context, SearchDelegate<T> @delegate, string? query = "", bool useRootNavigator = false, bool maintainState = false)
    {
        @delegate.query = query ?? @delegate.query;
        @delegate._currentBody = _SearchBody__search.suggestions;
        return Navigator.of(context, rootNavigator: useRootNavigator).push(new _SearchPageRoute__search<T>(@delegate: @delegate, maintainState: maintainState));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public abstract class SearchDelegate<T>
{
    public virtual bool? automaticallyImplyLeading { get; set; } = default;
    public virtual double? leadingWidth { get; set; } = default;
    public virtual string? searchFieldLabel { get; private set; }
    public virtual TextStyle? searchFieldStyle { get; private set; }
    public virtual InputDecorationTheme? searchFieldDecorationTheme { get; private set; }
    public virtual TextInputType? keyboardType { get; private set; }
    public virtual bool autocorrect { get; private set; } = default!;
    public virtual bool enableSuggestions { get; private set; } = default!;
    public virtual TextInputAction textInputAction { get; private set; } = default!;
    internal virtual FocusNode? _focusNode { get; set; } = default;
    internal virtual TextEditingController _queryTextController { get; private set; } = new TextEditingController();
    internal virtual ProxyAnimation _proxyAnimation { get; private set; } = new ProxyAnimation(AnimationsLibrary.kAlwaysDismissedAnimation);
    internal virtual ValueNotifier<_SearchBody__search?> _currentBodyNotifier { get; private set; } = new ValueNotifier<_SearchBody__search?>(null);
    internal virtual _SearchPageRoute__search<T>? _route { get; set; } = default;

    protected SearchDelegate(string? searchFieldLabel = null, TextStyle? searchFieldStyle = null, InputDecorationTheme? searchFieldDecorationTheme = null, TextInputType? keyboardType = null, TextInputAction textInputAction = TextInputAction.search, bool autocorrect = true, bool enableSuggestions = true)
    {
        this.searchFieldLabel = searchFieldLabel;
        this.searchFieldStyle = searchFieldStyle;
        this.searchFieldDecorationTheme = searchFieldDecorationTheme;
        this.keyboardType = keyboardType;
        this.textInputAction = textInputAction;
        this.autocorrect = autocorrect;
        this.enableSuggestions = enableSuggestions;
        System.Diagnostics.Debug.Assert((searchFieldStyle is null) || (searchFieldDecorationTheme is null));
    }

    public abstract Widget buildSuggestions(BuildContext context);
    public abstract Widget buildResults(BuildContext context);
    public abstract Widget? buildLeading(BuildContext context);
    public abstract List<Widget>? buildActions(BuildContext context);
    public virtual PreferredSizeWidget? buildBottom(BuildContext context) => DartRuntimePrimitives.ConvertValue<PreferredSizeWidget>(null);
    public virtual Widget? buildFlexibleSpace(BuildContext context) => DartRuntimePrimitives.ConvertValue<Widget>(null);
    public virtual ThemeData appBarTheme(BuildContext context)
    {
        ThemeData theme = Theme.of(context);
        ColorScheme colorSchemeLocal = theme.colorScheme;
        return theme.copyWith(appBarTheme: new AppBarThemeData(systemOverlayStyle: Equals(colorSchemeLocal.brightness, Brightness.dark) ? SystemUiOverlayStyle.light : SystemUiOverlayStyle.dark, backgroundColor: Equals(colorSchemeLocal.brightness, Brightness.dark) ? Colors.grey[900L] : Colors.white, iconTheme: theme.primaryIconTheme.copyWith(color: Colors.grey), titleTextStyle: theme.textTheme.titleLarge, toolbarTextStyle: theme.textTheme.bodyMedium), inputDecorationTheme: searchFieldDecorationTheme ?? new InputDecorationTheme(hintStyle: searchFieldStyle ?? theme.inputDecorationTheme.hintStyle, border: InputBorder.none));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string query
    {
        get => _queryTextController.text;
        set
        {
            var __value = value;
            _queryTextController.value = new TextEditingValue(text: __value, selection: TextSelection.CreateCollapsed(offset: __value.Length));
        }
    }
    public virtual void showResults(BuildContext context)
    {
        _focusNode?.unfocus();
        _currentBody = _SearchBody__search.results;
    }

    public virtual void showSuggestions(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => _focusNode is not null, () => (object?)"_focusNode must be set by route before showSuggestions is called.");
        _focusNode!.requestFocus();
        _currentBody = _SearchBody__search.suggestions;
    }

    public virtual void close(BuildContext context, T result)
    {
        _currentBody = null;
        _focusNode?.unfocus();
        DartRuntimePrimitives.Ignore(((Func<NavigatorState>)(() =>
{
    var __cascade = Navigator.of(context);
    __cascade.popUntil((route) => Equals((object?)route, _route));
    __cascade.pop(result);
    return __cascade;
}))());
    }

    internal virtual void _pop(BuildContext context)
    {
        _currentBody = null;
        _focusNode?.unfocus();
        DartRuntimePrimitives.Ignore(((Func<NavigatorState>)(() =>
{
    var __cascade = Navigator.of(context);
    __cascade.popUntil((route) => Equals((object?)route, _route));
    __cascade.pop<object>(null);
    return __cascade;
}))());
    }

    public virtual Animation<double> transitionAnimation => DartRuntimePrimitives.ConvertValue<Animation<double>>(_proxyAnimation);
    internal virtual _SearchBody__search? _currentBody
    {
        get => _currentBodyNotifier.value;
        set
        {
            var __value = value;
            _currentBodyNotifier.value = __value;
        }
    }
    public virtual void dispose()
    {
        _currentBodyNotifier.dispose();
        _focusNode?.dispose();
        _queryTextController.dispose();
        _proxyAnimation.parent = null;
    }

}

internal enum _SearchBody__search
{
    suggestions,
    results
}

internal class _SearchPageRoute__search<T> : PageRoute<T>
{
    public virtual SearchDelegate<T> @delegate { get; private set; } = default!;
    private bool __field_maintainState = default!;
    public override bool maintainState { get => __field_maintainState; }

    internal _SearchPageRoute__search(SearchDelegate<T> @delegate, bool maintainState)
    {
        this.@delegate = @delegate;
        __field_maintainState = maintainState;
        DartRuntimePrimitives.Assert(() => this.@delegate._route is null, () => (object?)$"The {DartRuntimePrimitives.RuntimeType(this.@delegate)} instance is currently used by another active " + "search. Please close that search by calling close() on the SearchDelegate " + "before opening another search with the same delegate instance.");
        this.@delegate._route = this;
    }

    public override Color? barrierColor => DartRuntimePrimitives.ConvertValue<Color>(null);
    public override string? barrierLabel => DartRuntimePrimitives.ConvertValue<string>(null);
    public override Duration transitionDuration => Duration.Create(milliseconds: 300L);
    public override Widget buildTransitions(BuildContext context, Animation<double> animation, Animation<double> secondaryAnimation, Widget child)
    {
        return new FadeTransition(opacity: animation, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Animation<double> createAnimation()
    {
        Animation<double> animation = base.createAnimation();
        @delegate._proxyAnimation.parent = animation;
        return animation;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget buildPage(BuildContext context, Animation<double> animation, Animation<double> secondaryAnimation)
    {
        return new _SearchPage__search<T>(@delegate: @delegate, animation: animation);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void didComplete(T? result)
    {
        base.didComplete(result);
        DartRuntimePrimitives.Assert(() => Equals(@delegate._route, this));
        @delegate._route = null;
        @delegate._currentBody = null;
    }

}

public class _SearchPage__search<T> : StatefulWidget
{
    public virtual SearchDelegate<T> @delegate { get; private set; } = default!;
    public virtual Animation<double> animation { get; private set; } = default!;

    internal _SearchPage__search(SearchDelegate<T> @delegate, Animation<double> animation)
    {
        this.@delegate = @delegate;
        this.animation = animation;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _SearchPageState__search<T>());
}

internal class _SearchPageState__search<T> : State<_SearchPage__search<T>>
{
    private bool __late_focusNode_initialized;
    private FocusNode __late_focusNode = default!;
    public virtual FocusNode focusNode
    {
        get
        {
            if (!__late_focusNode_initialized)
            {
                __late_focusNode = new FocusNode(onKeyEvent: (node, @event) =>
                {
                    if ((@event is KeyDownEvent) && Equals(((KeyDownEvent)@event).logicalKey, LogicalKeyboardKey.escape))
                    {
                        widget.@delegate._pop(context);
                        return KeyEventResult.handled;
                    }
                    return KeyEventResult.ignored;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
                __late_focusNode_initialized = true;
            }
            return __late_focusNode;
        }
    }

    public override void initState()
    {
        base.initState();
        widget.@delegate._queryTextController.addListener(_onQueryChanged);
        widget.animation.addStatusListener(_onAnimationStatusChanged);
        widget.@delegate._currentBodyNotifier.addListener(_onSearchBodyChanged);
        focusNode.addListener(_onFocusChanged);
        widget.@delegate._focusNode = focusNode;
    }

    public override void dispose()
    {
        base.dispose();
        widget.@delegate._queryTextController.removeListener(_onQueryChanged);
        widget.animation.removeStatusListener(_onAnimationStatusChanged);
        widget.@delegate._currentBodyNotifier.removeListener(_onSearchBodyChanged);
        widget.@delegate._focusNode = null;
        focusNode.dispose();
    }

    internal virtual void _onAnimationStatusChanged(AnimationStatus status)
    {
        if (!AnimationStatusMembers.isCompleted(status))
        {
            return;
        }
        widget.animation.removeStatusListener(_onAnimationStatusChanged);
        if (Equals(widget.@delegate._currentBody, _SearchBody__search.suggestions))
        {
            focusNode.requestFocus();
        }
    }

    public override void didUpdateWidget(_SearchPage__search<T> oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (!Equals(widget.@delegate, oldWidget.@delegate))
        {
            oldWidget.@delegate._queryTextController.removeListener(_onQueryChanged);
            widget.@delegate._queryTextController.addListener(_onQueryChanged);
            oldWidget.@delegate._currentBodyNotifier.removeListener(_onSearchBodyChanged);
            widget.@delegate._currentBodyNotifier.addListener(_onSearchBodyChanged);
            oldWidget.@delegate._focusNode = null;
            widget.@delegate._focusNode = focusNode;
        }
    }

    internal virtual void _onFocusChanged()
    {
        if (focusNode.hasFocus && (!Equals(widget.@delegate._currentBody, _SearchBody__search.suggestions)))
        {
            widget.@delegate.showSuggestions(context);
        }
    }

    internal virtual void _onQueryChanged()
    {
        setState(() =>
        {
        });
    }

    internal virtual void _onSearchBodyChanged()
    {
        setState(() =>
        {
        });
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterialLocalizations(context));
        ThemeData theme = widget.@delegate.appBarTheme(context);
        string searchFieldLabelLocal = widget.@delegate.searchFieldLabel ?? MaterialLocalizations.of(context).searchFieldLabel;
        Widget? bodyLocal = default!;
        switch (widget.@delegate._currentBody)
        {
            case _SearchBody__search.suggestions:
                {
                    bodyLocal = DartRuntimePrimitives.ConvertValue<Widget>(new KeyedSubtree(key: new ValueKey<_SearchBody__search>(_SearchBody__search.suggestions), child: widget.@delegate.buildSuggestions(context)));
                    break;
                }
            case _SearchBody__search.results:
                {
                    bodyLocal = DartRuntimePrimitives.ConvertValue<Widget>(new KeyedSubtree(key: new ValueKey<_SearchBody__search>(_SearchBody__search.results), child: widget.@delegate.buildResults(context)));
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
            case TargetPlatform.iOS:
            case TargetPlatform.macOS:
                {
                    routeName = "";
                    break;
                }
            case TargetPlatform.android:
            case TargetPlatform.fuchsia:
            case TargetPlatform.linux:
            case TargetPlatform.windows:
                {
                    routeName = searchFieldLabelLocal;
                    break;
                }
        }
        return new Widgets.Semantics(explicitChildNodes: true, scopesRoute: true, namesRoute: true, label: routeName, child: new Theme(data: theme, child: new Scaffold(appBar: new AppBar(leadingWidth: widget.@delegate.leadingWidth, automaticallyImplyLeading: widget.@delegate.automaticallyImplyLeading ?? true, leading: widget.@delegate.buildLeading(context), title: new Widgets.Semantics(inputType: SemanticsInputType.search, child: new TextField(controller: widget.@delegate._queryTextController, focusNode: focusNode, style: widget.@delegate.searchFieldStyle ?? theme.textTheme.titleLarge, textInputAction: widget.@delegate.textInputAction, autocorrect: widget.@delegate.autocorrect, enableSuggestions: widget.@delegate.enableSuggestions, keyboardType: widget.@delegate.keyboardType, onSubmitted: (_) => { widget.@delegate.showResults(context); }, decoration: new InputDecoration(hintText: searchFieldLabelLocal))), flexibleSpace: widget.@delegate.buildFlexibleSpace(context), actions: widget.@delegate.buildActions(context), bottom: widget.@delegate.buildBottom(context)), body: new AnimatedSwitcher(duration: Duration.Create(milliseconds: 300L), child: bodyLocal))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
