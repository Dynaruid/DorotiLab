// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/search_anchor.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public static partial class Search_anchorLibrary
{
    internal static long _kOpenViewMilliseconds = 600L;
}

public static partial class Search_anchorLibrary
{
    internal static Duration _kOpenViewDuration = Duration.Create(milliseconds: _kOpenViewMilliseconds);
}

public static partial class Search_anchorLibrary
{
    internal static Duration _kAnchorFadeDuration = Duration.Create(milliseconds: 150L);
}

public static partial class Search_anchorLibrary
{
    internal static global::Doroti.Framework.Animation.Curve _kViewFadeOnInterval = new global::Doroti.Framework.Animation.Interval(0.0, 1L / 2L);
}

public static partial class Search_anchorLibrary
{
    internal static global::Doroti.Framework.Animation.Curve _kViewIconsFadeOnInterval = new global::Doroti.Framework.Animation.Interval(1L / 6L, 2L / 6L);
}

public static partial class Search_anchorLibrary
{
    internal static global::Doroti.Framework.Animation.Curve _kViewDividerFadeOnInterval = new global::Doroti.Framework.Animation.Interval(0.0, 1L / 6L);
}

public static partial class Search_anchorLibrary
{
    internal static global::Doroti.Framework.Animation.Curve _kViewListFadeOnInterval = new global::Doroti.Framework.Animation.Interval(133L / _kOpenViewMilliseconds, 233L / _kOpenViewMilliseconds);
}

public static partial class Search_anchorLibrary
{
    internal static double _kDisableSearchBarOpacity = 0.38;
}

public delegate global::Doroti.Framework.Widgets.Widget SearchAnchorChildBuilder(global::Doroti.Framework.Widgets.BuildContext context, SearchController controller);

public delegate object SuggestionsBuilder(global::Doroti.Framework.Widgets.BuildContext context, SearchController controller);

public delegate global::Doroti.Framework.Widgets.Widget ViewBuilder(IEnumerable<global::Doroti.Framework.Widgets.Widget> suggestions);

public class SearchAnchor : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual bool? isFullScreen { get; private set; }
    public virtual SearchController? searchController { get; private set; }
    public virtual global::System.Func<IEnumerable<global::Doroti.Framework.Widgets.Widget>, global::Doroti.Framework.Widgets.Widget>? viewBuilder { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? viewLeading { get; private set; }
    public virtual IEnumerable<global::Doroti.Framework.Widgets.Widget>? viewTrailing { get; private set; }
    public virtual string? viewHintText { get; private set; }
    public virtual Color? viewBackgroundColor { get; private set; }
    public virtual double? viewElevation { get; private set; }
    public virtual Color? viewSurfaceTintColor { get; private set; }
    public virtual global::Doroti.Framework.Painting.BorderSide? viewSide { get; private set; }
    public virtual global::Doroti.Framework.Painting.OutlinedBorder? viewShape { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? viewBarPadding { get; private set; }
    public virtual double? headerHeight { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? headerTextStyle { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? headerHintStyle { get; private set; }
    public virtual Color? dividerColor { get; private set; }
    public virtual global::Doroti.Framework.Rendering.BoxConstraints? viewConstraints { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? viewPadding { get; private set; }
    public virtual bool? shrinkWrap { get; private set; }
    public virtual global::Doroti.Framework.Services.TextCapitalization? textCapitalization { get; private set; }
    public virtual global::System.Action<string>? viewOnChanged { get; private set; }
    public virtual global::System.Action<string>? viewOnSubmitted { get; private set; }
    public virtual global::System.Action? viewOnClose { get; private set; }
    public virtual global::System.Action? viewOnOpen { get; private set; }
    public virtual global::System.Func<global::Doroti.Framework.Widgets.BuildContext, SearchController, global::Doroti.Framework.Widgets.Widget> builder { get; private set; } = default!;
    public virtual global::System.Func<global::Doroti.Framework.Widgets.BuildContext, SearchController, object> suggestionsBuilder { get; private set; } = default!;
    public virtual global::Doroti.Framework.Services.TextInputAction? textInputAction { get; private set; }
    public virtual global::Doroti.Framework.Services.TextInputType? keyboardType { get; private set; }
    public virtual bool enabled { get; private set; } = default!;
    public virtual global::Doroti.Framework.Services.SmartDashesType? smartDashesType { get; private set; }
    public virtual global::Doroti.Framework.Services.SmartQuotesType? smartQuotesType { get; private set; }

    public SearchAnchor(global::Doroti.Framework.Foundation.Key? key = null, bool? isFullScreen = null, SearchController? searchController = null, global::System.Func<IEnumerable<global::Doroti.Framework.Widgets.Widget>, global::Doroti.Framework.Widgets.Widget>? viewBuilder = null, global::Doroti.Framework.Widgets.Widget? viewLeading = null, IEnumerable<global::Doroti.Framework.Widgets.Widget>? viewTrailing = null, string? viewHintText = null, Color? viewBackgroundColor = null, double? viewElevation = null, Color? viewSurfaceTintColor = null, global::Doroti.Framework.Painting.BorderSide? viewSide = null, global::Doroti.Framework.Painting.OutlinedBorder? viewShape = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? viewBarPadding = null, double? headerHeight = null, global::Doroti.Framework.Painting.TextStyle? headerTextStyle = null, global::Doroti.Framework.Painting.TextStyle? headerHintStyle = null, Color? dividerColor = null, global::Doroti.Framework.Rendering.BoxConstraints? viewConstraints = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? viewPadding = null, bool? shrinkWrap = null, global::Doroti.Framework.Services.TextCapitalization? textCapitalization = null, global::System.Action<string>? viewOnChanged = null, global::System.Action<string>? viewOnSubmitted = null, global::System.Action? viewOnClose = null, global::System.Action? viewOnOpen = null, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, SearchController, global::Doroti.Framework.Widgets.Widget> builder = default!, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, SearchController, object> suggestionsBuilder = default!, global::Doroti.Framework.Services.TextInputAction? textInputAction = null, global::Doroti.Framework.Services.TextInputType? keyboardType = null, bool enabled = true, global::Doroti.Framework.Services.SmartDashesType? smartDashesType = null, global::Doroti.Framework.Services.SmartQuotesType? smartQuotesType = null) : base(key: key)
    {
        this.isFullScreen = isFullScreen;
        this.searchController = searchController;
        this.viewBuilder = viewBuilder;
        this.viewLeading = viewLeading;
        this.viewTrailing = viewTrailing;
        this.viewHintText = viewHintText;
        this.viewBackgroundColor = viewBackgroundColor;
        this.viewElevation = viewElevation;
        this.viewSurfaceTintColor = viewSurfaceTintColor;
        this.viewSide = viewSide;
        this.viewShape = viewShape;
        this.viewBarPadding = viewBarPadding;
        this.headerHeight = headerHeight;
        this.headerTextStyle = headerTextStyle;
        this.headerHintStyle = headerHintStyle;
        this.dividerColor = dividerColor;
        this.viewConstraints = viewConstraints;
        this.viewPadding = viewPadding;
        this.shrinkWrap = shrinkWrap;
        this.textCapitalization = textCapitalization;
        this.viewOnChanged = viewOnChanged;
        this.viewOnSubmitted = viewOnSubmitted;
        this.viewOnClose = viewOnClose;
        this.viewOnOpen = viewOnOpen;
        this.builder = builder;
        this.suggestionsBuilder = suggestionsBuilder;
        this.textInputAction = textInputAction;
        this.keyboardType = keyboardType;
        this.enabled = enabled;
        this.smartDashesType = smartDashesType;
        this.smartQuotesType = smartQuotesType;
    }

    public static SearchAnchor CreateBar(global::Doroti.Framework.Widgets.Widget? barLeading = null, IEnumerable<global::Doroti.Framework.Widgets.Widget>? barTrailing = null, string? barHintText = null, global::System.Action? onTap = null, global::System.Action<string>? onSubmitted = null, global::System.Action<string>? onChanged = null, global::System.Action? onClose = null, global::System.Action? onOpen = null, global::Doroti.Framework.Widgets.WidgetStateProperty<double?>? barElevation = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? barBackgroundColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? barOverlayColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.BorderSide?>? barSide = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.OutlinedBorder?>? barShape = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry?>? barPadding = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? viewBarPadding = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.TextStyle?>? barTextStyle = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.TextStyle?>? barHintStyle = null, global::System.Func<IEnumerable<global::Doroti.Framework.Widgets.Widget>, global::Doroti.Framework.Widgets.Widget>? viewBuilder = null, global::Doroti.Framework.Widgets.Widget? viewLeading = null, IEnumerable<global::Doroti.Framework.Widgets.Widget>? viewTrailing = null, string? viewHintText = null, Color? viewBackgroundColor = null, double? viewElevation = null, global::Doroti.Framework.Painting.BorderSide? viewSide = null, global::Doroti.Framework.Painting.OutlinedBorder? viewShape = null, double? viewHeaderHeight = null, global::Doroti.Framework.Painting.TextStyle? viewHeaderTextStyle = null, global::Doroti.Framework.Painting.TextStyle? viewHeaderHintStyle = null, Color? dividerColor = null, global::Doroti.Framework.Rendering.BoxConstraints? constraints = null, global::Doroti.Framework.Rendering.BoxConstraints? viewConstraints = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? viewPadding = null, bool? shrinkWrap = null, bool? isFullScreen = null, SearchController searchController = default!, global::Doroti.Framework.Services.TextCapitalization textCapitalization = default!, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, SearchController, object> suggestionsBuilder = default!, global::Doroti.Framework.Services.TextInputAction? textInputAction = null, global::Doroti.Framework.Services.TextInputType? keyboardType = null, global::Doroti.Framework.Painting.EdgeInsets scrollPadding = default!, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.EditableTextState, global::Doroti.Framework.Widgets.Widget> contextMenuBuilder = default!, bool enabled = true, global::Doroti.Framework.Services.SmartDashesType? smartDashesType = null, global::Doroti.Framework.Services.SmartQuotesType? smartQuotesType = null)
        => new _SearchAnchorWithSearchBar__search_anchor(barLeading: barLeading, barTrailing: barTrailing, barHintText: barHintText, onTap: onTap, onSubmitted: onSubmitted, onChanged: onChanged, onClose: onClose, onOpen: onOpen, barElevation: barElevation, barBackgroundColor: barBackgroundColor, barOverlayColor: barOverlayColor, barSide: barSide, barShape: barShape, barPadding: barPadding, viewBarPadding: viewBarPadding, barTextStyle: barTextStyle, barHintStyle: barHintStyle, viewBuilder: viewBuilder, viewLeading: viewLeading, viewTrailing: viewTrailing, viewHintText: viewHintText, viewBackgroundColor: viewBackgroundColor, viewElevation: viewElevation, viewSide: viewSide, viewShape: viewShape, viewHeaderHeight: viewHeaderHeight, viewHeaderTextStyle: viewHeaderTextStyle, viewHeaderHintStyle: viewHeaderHintStyle, dividerColor: dividerColor, constraints: constraints, viewConstraints: viewConstraints, viewPadding: viewPadding, shrinkWrap: shrinkWrap, isFullScreen: isFullScreen, searchController: searchController, textCapitalization: textCapitalization, suggestionsBuilder: suggestionsBuilder, textInputAction: textInputAction, keyboardType: keyboardType, scrollPadding: scrollPadding, contextMenuBuilder: contextMenuBuilder, enabled: enabled, smartDashesType: smartDashesType, smartQuotesType: smartQuotesType);

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _SearchAnchorState__search_anchor());
}

internal class _SearchAnchorState__search_anchor : global::Doroti.Framework.Widgets.State<SearchAnchor>
{
    internal virtual Size? _screenSize { get; set; } = default;
    internal virtual bool _anchorIsVisible { get; set; } = true;
    internal virtual global::Doroti.Framework.Widgets.GlobalKey<IState> _anchorKey { get; private set; } = GlobalKey<IState>.Create();
    internal virtual SearchController? _internalSearchController { get; set; } = default;
    internal virtual _SearchViewRoute__search_anchor? _route { get; set; } = default;

    internal virtual bool _viewIsOpen => !_anchorIsVisible;
    internal virtual SearchController _searchController => DartRuntimePrimitives.ConvertValue<SearchController>(widget.searchController ?? (_internalSearchController ??= new SearchController()));
    public override void initState()
    {
        base.initState();
        _searchController._attach(this);
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        global::Doroti.Ui.Size updatedScreenSize = MediaQuery.of(context).size;
        if ((_screenSize is not null) && (!Equals(_screenSize, updatedScreenSize)))
        {
            if (_searchController.isOpen && !getShowFullScreenView())
            {
                _closeView(null);
            }
        }
        _screenSize = updatedScreenSize;
    }

    public override void didUpdateWidget(SearchAnchor oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (!Equals(oldWidget.searchController, widget.searchController))
        {
            oldWidget.searchController?._detach(this);
            _searchController._attach(this);
        }
    }

    public override void dispose()
    {
        widget.searchController?._detach(this);
        _internalSearchController?._detach(this);
        var usingExternalController = widget.searchController is not null;
        if (_route?.navigator is not null)
        {
            _route?._dismiss(disposeController: !usingExternalController);
            if (usingExternalController)
            {
                _internalSearchController?.dispose();
            }
        }
        else
        {
            _internalSearchController?.dispose();
        }
        base.dispose();
    }

    internal virtual void _openView()
    {
        global::Doroti.Framework.Widgets.NavigatorState navigator = Navigator.of(context);
        _route = new _SearchViewRoute__search_anchor(viewOnChanged: widget.viewOnChanged, viewOnSubmitted: widget.viewOnSubmitted, viewOnClose: widget.viewOnClose, viewOnOpen: widget.viewOnOpen, viewLeading: widget.viewLeading, viewTrailing: widget.viewTrailing, viewHintText: widget.viewHintText, viewBackgroundColor: widget.viewBackgroundColor, viewElevation: widget.viewElevation, viewSurfaceTintColor: widget.viewSurfaceTintColor, viewSide: widget.viewSide, viewShape: widget.viewShape, viewBarPadding: widget.viewBarPadding, viewHeaderHeight: widget.headerHeight, viewHeaderTextStyle: widget.headerTextStyle, viewHeaderHintStyle: widget.headerHintStyle, dividerColor: widget.dividerColor, viewConstraints: widget.viewConstraints, viewPadding: widget.viewPadding, shrinkWrap: widget.shrinkWrap, showFullScreenView: getShowFullScreenView(), toggleVisibility: toggleVisibility, textDirection: Directionality.of(context), viewBuilder: widget.viewBuilder, anchorKey: _anchorKey, searchController: _searchController, suggestionsBuilder: widget.suggestionsBuilder, textCapitalization: widget.textCapitalization, capturedThemes: InheritedTheme.capture(from: context, to: navigator.context), textInputAction: widget.textInputAction, keyboardType: widget.keyboardType, smartDashesType: widget.smartDashesType, smartQuotesType: widget.smartQuotesType);
        DartRuntimePrimitives.Ignore(navigator.push(_route!));
    }

    internal virtual void _closeView(string? selectedText)
    {
        if (selectedText is not null)
        {
            _searchController.value = new global::Doroti.Framework.Services.TextEditingValue(text: selectedText);
        }
        Navigator.of(context).pop<object>();
    }

    public virtual bool toggleVisibility()
    {
        setState(() =>
        {
            _anchorIsVisible = !_anchorIsVisible;
        });
        return _anchorIsVisible;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool getShowFullScreenView()
    {
        return widget.isFullScreen ?? (Theme.of(context).platform switch { TargetPlatform.iOS or TargetPlatform.android => true, TargetPlatform.fuchsia => true, TargetPlatform.macOS or TargetPlatform.linux => false, TargetPlatform.windows => false, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _getOpacity()
    {
        if (widget.enabled)
        {
            return _anchorIsVisible ? 1.0 : 0.0;
        }
        return Search_anchorLibrary._kDisableSearchBarOpacity;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return new global::Doroti.Framework.Widgets.AnimatedOpacity(key: _anchorKey, opacity: _getOpacity(), duration: Search_anchorLibrary._kAnchorFadeDuration, child: new global::Doroti.Framework.Widgets.IgnorePointer(ignoring: !widget.enabled, child: new global::Doroti.Framework.Widgets.GestureDetector(onTap: () => _openView(), child: widget.builder(context, _searchController))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _SearchViewRoute__search_anchor : global::Doroti.Framework.Widgets.PopupRoute<_SearchViewRoute__search_anchor>
{
    public virtual global::System.Action<string>? viewOnChanged { get; private set; }
    public virtual global::System.Action<string>? viewOnSubmitted { get; private set; }
    public virtual global::System.Action? viewOnClose { get; private set; }
    public virtual global::System.Action? viewOnOpen { get; private set; }
    public virtual global::System.Func<bool>? toggleVisibility { get; private set; }
    public virtual TextDirection? textDirection { get; private set; }
    public virtual global::System.Func<IEnumerable<global::Doroti.Framework.Widgets.Widget>, global::Doroti.Framework.Widgets.Widget>? viewBuilder { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? viewLeading { get; private set; }
    public virtual IEnumerable<global::Doroti.Framework.Widgets.Widget>? viewTrailing { get; private set; }
    public virtual string? viewHintText { get; private set; }
    public virtual Color? viewBackgroundColor { get; private set; }
    public virtual double? viewElevation { get; private set; }
    public virtual Color? viewSurfaceTintColor { get; private set; }
    public virtual global::Doroti.Framework.Painting.BorderSide? viewSide { get; private set; }
    public virtual global::Doroti.Framework.Painting.OutlinedBorder? viewShape { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? viewBarPadding { get; private set; }
    public virtual double? viewHeaderHeight { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? viewHeaderTextStyle { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? viewHeaderHintStyle { get; private set; }
    public virtual Color? dividerColor { get; private set; }
    public virtual global::Doroti.Framework.Rendering.BoxConstraints? viewConstraints { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? viewPadding { get; private set; }
    public virtual bool? shrinkWrap { get; private set; }
    public virtual global::Doroti.Framework.Services.TextCapitalization? textCapitalization { get; private set; }
    public virtual bool showFullScreenView { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.GlobalKey<IState> anchorKey { get; private set; } = default!;
    public virtual SearchController searchController { get; private set; } = default!;
    public virtual global::System.Func<global::Doroti.Framework.Widgets.BuildContext, SearchController, object> suggestionsBuilder { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.CapturedThemes capturedThemes { get; private set; } = default!;
    public virtual global::Doroti.Framework.Services.TextInputAction? textInputAction { get; private set; }
    public virtual global::Doroti.Framework.Services.TextInputType? keyboardType { get; private set; }
    public virtual global::Doroti.Framework.Services.SmartDashesType? smartDashesType { get; private set; }
    public virtual global::Doroti.Framework.Services.SmartQuotesType? smartQuotesType { get; private set; }
    public virtual global::Doroti.Framework.Animation.CurvedAnimation? curvedAnimation { get; set; } = default;
    public virtual global::Doroti.Framework.Animation.CurvedAnimation? viewFadeOnIntervalCurve { get; set; } = default;
    public virtual bool willDisposeSearchController { get; set; } = false;
    public virtual SearchViewThemeData viewDefaults { get; private set; } = default!;
    public virtual SearchViewThemeData viewTheme { get; private set; } = default!;
    internal virtual global::Doroti.Framework.Animation.RectTween _rectTween { get; private set; } = new global::Doroti.Framework.Animation.RectTween();

    internal _SearchViewRoute__search_anchor(global::System.Action<string>? viewOnChanged = null, global::System.Action<string>? viewOnSubmitted = null, global::System.Action? viewOnClose = null, global::System.Action? viewOnOpen = null, global::System.Func<bool>? toggleVisibility = null, TextDirection? textDirection = null, global::System.Func<IEnumerable<global::Doroti.Framework.Widgets.Widget>, global::Doroti.Framework.Widgets.Widget>? viewBuilder = null, global::Doroti.Framework.Widgets.Widget? viewLeading = null, IEnumerable<global::Doroti.Framework.Widgets.Widget>? viewTrailing = null, string? viewHintText = null, Color? viewBackgroundColor = null, double? viewElevation = null, Color? viewSurfaceTintColor = null, global::Doroti.Framework.Painting.BorderSide? viewSide = null, global::Doroti.Framework.Painting.OutlinedBorder? viewShape = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? viewBarPadding = null, double? viewHeaderHeight = null, global::Doroti.Framework.Painting.TextStyle? viewHeaderTextStyle = null, global::Doroti.Framework.Painting.TextStyle? viewHeaderHintStyle = null, Color? dividerColor = null, global::Doroti.Framework.Rendering.BoxConstraints? viewConstraints = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? viewPadding = null, bool? shrinkWrap = null, global::Doroti.Framework.Services.TextCapitalization? textCapitalization = null, bool showFullScreenView = default!, global::Doroti.Framework.Widgets.GlobalKey<IState> anchorKey = default!, SearchController searchController = default!, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, SearchController, object> suggestionsBuilder = default!, global::Doroti.Framework.Widgets.CapturedThemes capturedThemes = default!, global::Doroti.Framework.Services.TextInputAction? textInputAction = null, global::Doroti.Framework.Services.TextInputType? keyboardType = null, global::Doroti.Framework.Services.SmartDashesType? smartDashesType = null, global::Doroti.Framework.Services.SmartQuotesType? smartQuotesType = null)
    {
        this.viewOnChanged = viewOnChanged;
        this.viewOnSubmitted = viewOnSubmitted;
        this.viewOnClose = viewOnClose;
        this.viewOnOpen = viewOnOpen;
        this.toggleVisibility = toggleVisibility;
        this.textDirection = textDirection;
        this.viewBuilder = viewBuilder;
        this.viewLeading = viewLeading;
        this.viewTrailing = viewTrailing;
        this.viewHintText = viewHintText;
        this.viewBackgroundColor = viewBackgroundColor;
        this.viewElevation = viewElevation;
        this.viewSurfaceTintColor = viewSurfaceTintColor;
        this.viewSide = viewSide;
        this.viewShape = viewShape;
        this.viewBarPadding = viewBarPadding;
        this.viewHeaderHeight = viewHeaderHeight;
        this.viewHeaderTextStyle = viewHeaderTextStyle;
        this.viewHeaderHintStyle = viewHeaderHintStyle;
        this.dividerColor = dividerColor;
        this.viewConstraints = viewConstraints;
        this.viewPadding = viewPadding;
        this.shrinkWrap = shrinkWrap;
        this.textCapitalization = textCapitalization;
        this.showFullScreenView = showFullScreenView;
        this.anchorKey = anchorKey;
        this.searchController = searchController;
        this.suggestionsBuilder = suggestionsBuilder;
        this.capturedThemes = capturedThemes;
        this.textInputAction = textInputAction;
        this.keyboardType = keyboardType;
        this.smartDashesType = smartDashesType;
        this.smartQuotesType = smartQuotesType;
    }

    public override Color? barrierColor => Colors.transparent;
    public override bool barrierDismissible => true;
    public override string? barrierLabel => "Dismiss";
    public virtual global::Doroti.Ui.Rect? getRect()
    {
        global::Doroti.Framework.Widgets.BuildContext? contextLocal = anchorKey.currentContext;
        if (contextLocal is not null)
        {
            var searchBarBox = ((global::Doroti.Framework.Rendering.RenderBox?)contextLocal.findRenderObject()!)!;
            global::Doroti.Ui.Size boxSize = searchBarBox.size;
            global::Doroti.Framework.Widgets.NavigatorState navigator = Navigator.of(contextLocal);
            global::Doroti.Ui.Offset boxLocation = searchBarBox.localToGlobal(Offset.zero, ancestor: navigator.context.findRenderObject());
            return boxLocation & boxSize;
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Scheduler.TickerFuture didPush()
    {
        DartRuntimePrimitives.Assert(() => anchorKey.currentContext is not null);
        updateViewConfig(anchorKey.currentContext!);
        updateTweens(anchorKey.currentContext!);
        toggleVisibility?.Invoke();
        viewOnOpen?.Invoke();
        return base.didPush();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool didPop(_SearchViewRoute__search_anchor? result)
    {
        DartRuntimePrimitives.Assert(() => anchorKey.currentContext is not null);
        updateTweens(anchorKey.currentContext!);
        toggleVisibility?.Invoke();
        viewOnClose?.Invoke();
        WidgetsBinding.instance.addPostFrameCallback((_) =>
        {
            if (anchorKey.currentContext is not null)
            {
                FocusScope.of(anchorKey.currentContext!).unfocus();
            }
        });
        return base.didPop(result);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _dismiss(bool disposeController)
    {
        willDisposeSearchController = disposeController;
        if (isActive)
        {
            navigator?.removeRoute(this);
        }
    }

    public override void dispose()
    {
        curvedAnimation?.dispose();
        viewFadeOnIntervalCurve?.dispose();
        if (willDisposeSearchController)
        {
            searchController.dispose();
        }
        base.dispose();
    }

    public virtual void updateViewConfig(global::Doroti.Framework.Widgets.BuildContext context)
    {
        viewDefaults = DartRuntimePrimitives.ConvertValue<SearchViewThemeData>(new _SearchViewDefaultsM3__search_anchor(context, isFullScreen: showFullScreenView));
        viewTheme = SearchViewTheme.of(context);
    }

    public virtual void updateTweens(global::Doroti.Framework.Widgets.BuildContext context)
    {
        var navigator = ((global::Doroti.Framework.Rendering.RenderBox?)Navigator.of(context).context.findRenderObject()!)!;
        global::Doroti.Ui.Size screenSize = navigator.size;
        global::Doroti.Ui.Rect anchorRect = getRect() ?? Rect.zero;
        global::Doroti.Framework.Rendering.BoxConstraints effectiveConstraints = (viewConstraints ?? viewTheme.constraints) ?? viewDefaults.constraints!;
        _rectTween.begin = anchorRect;
        double viewWidth = Dart_uiLibrary.clampDouble(anchorRect.width, effectiveConstraints.minWidth, effectiveConstraints.maxWidth);
        double viewHeight = Dart_uiLibrary.clampDouble(screenSize.height * 2L / 3L, effectiveConstraints.minHeight, effectiveConstraints.maxHeight);
        switch (textDirection ?? TextDirection.ltr)
        {
            case TextDirection.ltr:
                {
                    double viewLeftToScreenRight = screenSize.width - anchorRect.left;
                    double viewTopToScreenBottom = screenSize.height - anchorRect.top;
                    global::Doroti.Ui.Offset topLeftLocal = anchorRect.topLeft;
                    if (viewLeftToScreenRight < viewWidth)
                    {
                        topLeftLocal = new global::Doroti.Ui.Offset(screenSize.width - Math.Min(viewWidth, screenSize.width), topLeftLocal.dy);
                    }
                    if (viewTopToScreenBottom < viewHeight)
                    {
                        topLeftLocal = new global::Doroti.Ui.Offset(topLeftLocal.dx, screenSize.height - Math.Min(viewHeight, screenSize.height));
                    }
                    var endSize = new global::Doroti.Ui.Size(viewWidth, viewHeight);
                    _rectTween.end = showFullScreenView ? (Offset.zero & screenSize) : (topLeftLocal & endSize);
                    return;
                }
            case TextDirection.rtl:
                {
                    double viewRightToScreenLeft = anchorRect.right;
                    double viewTopToScreenBottomLocal = screenSize.height - anchorRect.top;
                    var topLeftAlternate = new global::Doroti.Ui.Offset(Math.Max(anchorRect.right - viewWidth, 0.0), anchorRect.top);
                    if (viewRightToScreenLeft < viewWidth)
                    {
                        topLeftAlternate = new global::Doroti.Ui.Offset(0.0, topLeftAlternate.dy);
                    }
                    if (viewTopToScreenBottomLocal < viewHeight)
                    {
                        topLeftAlternate = new global::Doroti.Ui.Offset(topLeftAlternate.dx, screenSize.height - Math.Min(viewHeight, screenSize.height));
                    }
                    var endSizeLocal = new global::Doroti.Ui.Size(viewWidth, viewHeight);
                    _rectTween.end = showFullScreenView ? (Offset.zero & screenSize) : (topLeftAlternate & endSizeLocal);
                    break;
                }
        }
    }

    public override global::Doroti.Framework.Widgets.Widget buildPage(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation, global::Doroti.Framework.Animation.Animation<double> secondaryAnimation)
    {
        return new global::Doroti.Framework.Widgets.Directionality(textDirection: textDirection ?? TextDirection.ltr, child: new global::Doroti.Framework.Widgets.AnimatedBuilder(animation: animation, builder: (context, child) =>
        {
            curvedAnimation ??= new global::Doroti.Framework.Animation.CurvedAnimation(parent: animation, curve: Curves.easeInOutCubicEmphasized, reverseCurve: Curves.easeInOutCubicEmphasized.flipped);
            global::Doroti.Ui.Rect viewRectLocal = DartRuntimePrimitives.RequireValue(_rectTween.evaluate(curvedAnimation!));
            double topPaddingLocal = showFullScreenView ? DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(0.0, MediaQuery.paddingOf(context).top, curvedAnimation!.value)) : 0.0;
            viewFadeOnIntervalCurve ??= new global::Doroti.Framework.Animation.CurvedAnimation(parent: animation, curve: Search_anchorLibrary._kViewFadeOnInterval, reverseCurve: Search_anchorLibrary._kViewFadeOnInterval.flipped);
            return new global::Doroti.Framework.Widgets.FadeTransition(opacity: viewFadeOnIntervalCurve!, child: capturedThemes.wrap(new _ViewContent__search_anchor(viewOnChanged: viewOnChanged, viewOnSubmitted: viewOnSubmitted, viewLeading: viewLeading, viewTrailing: viewTrailing, viewHintText: viewHintText, viewBackgroundColor: viewBackgroundColor, viewElevation: viewElevation, viewSurfaceTintColor: viewSurfaceTintColor, viewSide: viewSide, viewShape: viewShape, viewBarPadding: viewBarPadding, viewHeaderHeight: viewHeaderHeight, viewHeaderTextStyle: viewHeaderTextStyle, viewHeaderHintStyle: viewHeaderHintStyle, dividerColor: dividerColor, viewConstraints: viewConstraints, viewPadding: viewPadding, shrinkWrap: shrinkWrap, showFullScreenView: showFullScreenView, animation: curvedAnimation!, topPadding: topPaddingLocal, viewMaxWidth: DartRuntimePrimitives.RequireValue(_rectTween.end).width, viewRect: viewRectLocal, viewBuilder: viewBuilder, searchController: searchController, suggestionsBuilder: suggestionsBuilder, textCapitalization: textCapitalization, textInputAction: textInputAction, keyboardType: keyboardType, smartDashesType: smartDashesType, smartQuotesType: smartQuotesType)));
            throw new InvalidOperationException("Dart closure completed without a value.");
        }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Duration transitionDuration => Search_anchorLibrary._kOpenViewDuration;
}

public class _ViewContent__search_anchor : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual global::System.Action<string>? viewOnChanged { get; private set; }
    public virtual global::System.Action<string>? viewOnSubmitted { get; private set; }
    public virtual global::System.Func<IEnumerable<global::Doroti.Framework.Widgets.Widget>, global::Doroti.Framework.Widgets.Widget>? viewBuilder { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? viewLeading { get; private set; }
    public virtual IEnumerable<global::Doroti.Framework.Widgets.Widget>? viewTrailing { get; private set; }
    public virtual string? viewHintText { get; private set; }
    public virtual Color? viewBackgroundColor { get; private set; }
    public virtual double? viewElevation { get; private set; }
    public virtual Color? viewSurfaceTintColor { get; private set; }
    public virtual global::Doroti.Framework.Painting.BorderSide? viewSide { get; private set; }
    public virtual global::Doroti.Framework.Painting.OutlinedBorder? viewShape { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? viewBarPadding { get; private set; }
    public virtual double? viewHeaderHeight { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? viewHeaderTextStyle { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? viewHeaderHintStyle { get; private set; }
    public virtual Color? dividerColor { get; private set; }
    public virtual global::Doroti.Framework.Rendering.BoxConstraints? viewConstraints { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? viewPadding { get; private set; }
    public virtual bool? shrinkWrap { get; private set; }
    public virtual global::Doroti.Framework.Services.TextCapitalization? textCapitalization { get; private set; }
    public virtual bool showFullScreenView { get; private set; } = default!;
    public virtual double topPadding { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.Animation<double> animation { get; private set; } = default!;
    public virtual double viewMaxWidth { get; private set; } = default!;
    public virtual Rect viewRect { get; private set; } = default!;
    public virtual SearchController searchController { get; private set; } = default!;
    public virtual global::System.Func<global::Doroti.Framework.Widgets.BuildContext, SearchController, object> suggestionsBuilder { get; private set; } = default!;
    public virtual global::Doroti.Framework.Services.TextInputAction? textInputAction { get; private set; }
    public virtual global::Doroti.Framework.Services.TextInputType? keyboardType { get; private set; }
    public virtual global::Doroti.Framework.Services.SmartDashesType? smartDashesType { get; private set; }
    public virtual global::Doroti.Framework.Services.SmartQuotesType? smartQuotesType { get; private set; }

    internal _ViewContent__search_anchor(global::System.Action<string>? viewOnChanged = null, global::System.Action<string>? viewOnSubmitted = null, global::System.Func<IEnumerable<global::Doroti.Framework.Widgets.Widget>, global::Doroti.Framework.Widgets.Widget>? viewBuilder = null, global::Doroti.Framework.Widgets.Widget? viewLeading = null, IEnumerable<global::Doroti.Framework.Widgets.Widget>? viewTrailing = null, string? viewHintText = null, Color? viewBackgroundColor = null, double? viewElevation = null, Color? viewSurfaceTintColor = null, global::Doroti.Framework.Painting.BorderSide? viewSide = null, global::Doroti.Framework.Painting.OutlinedBorder? viewShape = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? viewBarPadding = null, double? viewHeaderHeight = null, global::Doroti.Framework.Painting.TextStyle? viewHeaderTextStyle = null, global::Doroti.Framework.Painting.TextStyle? viewHeaderHintStyle = null, Color? dividerColor = null, global::Doroti.Framework.Rendering.BoxConstraints? viewConstraints = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? viewPadding = null, bool? shrinkWrap = null, global::Doroti.Framework.Services.TextCapitalization? textCapitalization = null, bool showFullScreenView = default!, double topPadding = default!, global::Doroti.Framework.Animation.Animation<double> animation = default!, double viewMaxWidth = default!, Rect viewRect = default!, SearchController searchController = default!, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, SearchController, object> suggestionsBuilder = default!, global::Doroti.Framework.Services.TextInputAction? textInputAction = null, global::Doroti.Framework.Services.TextInputType? keyboardType = null, global::Doroti.Framework.Services.SmartDashesType? smartDashesType = null, global::Doroti.Framework.Services.SmartQuotesType? smartQuotesType = null)
    {
        this.viewOnChanged = viewOnChanged;
        this.viewOnSubmitted = viewOnSubmitted;
        this.viewBuilder = viewBuilder;
        this.viewLeading = viewLeading;
        this.viewTrailing = viewTrailing;
        this.viewHintText = viewHintText;
        this.viewBackgroundColor = viewBackgroundColor;
        this.viewElevation = viewElevation;
        this.viewSurfaceTintColor = viewSurfaceTintColor;
        this.viewSide = viewSide;
        this.viewShape = viewShape;
        this.viewBarPadding = viewBarPadding;
        this.viewHeaderHeight = viewHeaderHeight;
        this.viewHeaderTextStyle = viewHeaderTextStyle;
        this.viewHeaderHintStyle = viewHeaderHintStyle;
        this.dividerColor = dividerColor;
        this.viewConstraints = viewConstraints;
        this.viewPadding = viewPadding;
        this.shrinkWrap = shrinkWrap;
        this.textCapitalization = textCapitalization;
        this.showFullScreenView = showFullScreenView;
        this.topPadding = topPadding;
        this.animation = animation;
        this.viewMaxWidth = viewMaxWidth;
        this.viewRect = viewRect;
        this.searchController = searchController;
        this.suggestionsBuilder = suggestionsBuilder;
        this.textInputAction = textInputAction;
        this.keyboardType = keyboardType;
        this.smartDashesType = smartDashesType;
        this.smartQuotesType = smartQuotesType;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _ViewContentState__search_anchor());
}

internal class _ViewContentState__search_anchor : global::Doroti.Framework.Widgets.State<_ViewContent__search_anchor>
{
    internal virtual Size? _screenSize { get; set; } = default;
    internal virtual Rect _viewRect { get; set; } = default!;
    public virtual global::Doroti.Framework.Animation.CurvedAnimation viewIconsFadeCurve { get; set; } = default!;
    public virtual global::Doroti.Framework.Animation.CurvedAnimation viewDividerFadeCurve { get; set; } = default!;
    public virtual global::Doroti.Framework.Animation.CurvedAnimation viewListFadeOnIntervalCurve { get; set; } = default!;
    internal virtual SearchController _controller { get; private set; } = default!;
    public virtual IEnumerable<global::Doroti.Framework.Widgets.Widget> result { get; set; } = new List<global::Doroti.Framework.Widgets.Widget>();
    public virtual string? searchValue { get; set; } = default;
    internal virtual Timer? _timer { get; set; } = default;

    public override void initState()
    {
        base.initState();
        _viewRect = widget.viewRect;
        _controller = widget.searchController;
        _controller.addListener(_updateSuggestionsListener);
        _setupAnimations();
    }

    public override void didUpdateWidget(_ViewContent__search_anchor oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (!Equals(widget.viewRect, oldWidget.viewRect))
        {
            setState(() =>
            {
                _viewRect = widget.viewRect;
            });
        }
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        global::Doroti.Ui.Size updatedScreenSize = MediaQuery.of(context).size;
        if (!Equals(_screenSize, updatedScreenSize))
        {
            _screenSize = updatedScreenSize;
            if (widget.showFullScreenView)
            {
                _viewRect = Offset.zero & DartRuntimePrimitives.RequireValue(_screenSize);
            }
        }
        if (searchValue != _controller.text)
        {
            _timer?.cancel();
            _timer = new Timer(Duration.zero, async () =>
            {
                searchValue = _controller.text;
                IEnumerable<global::Doroti.Framework.Widgets.Widget> suggestions = await DartAsyncRuntime.AwaitFutureOrValue<IEnumerable<global::Doroti.Framework.Widgets.Widget>>(widget.suggestionsBuilder(context, _controller));
                _timer?.cancel();
                _timer = null;
                if (mounted)
                {
                    setState(() =>
                    {
                        result = suggestions;
                    });
                }
                return;
            });
        }
    }

    public override void dispose()
    {
        _controller.removeListener(_updateSuggestionsListener);
        _disposeAnimations();
        _timer?.cancel();
        _timer = null;
        base.dispose();
    }

    internal virtual void _setupAnimations()
    {
        viewIconsFadeCurve = new global::Doroti.Framework.Animation.CurvedAnimation(parent: widget.animation, curve: Search_anchorLibrary._kViewIconsFadeOnInterval, reverseCurve: Search_anchorLibrary._kViewIconsFadeOnInterval.flipped);
        viewDividerFadeCurve = new global::Doroti.Framework.Animation.CurvedAnimation(parent: widget.animation, curve: Search_anchorLibrary._kViewDividerFadeOnInterval, reverseCurve: Search_anchorLibrary._kViewFadeOnInterval.flipped);
        viewListFadeOnIntervalCurve = new global::Doroti.Framework.Animation.CurvedAnimation(parent: widget.animation, curve: Search_anchorLibrary._kViewListFadeOnInterval, reverseCurve: Search_anchorLibrary._kViewListFadeOnInterval.flipped);
    }

    internal virtual void _disposeAnimations()
    {
        viewIconsFadeCurve.dispose();
        viewDividerFadeCurve.dispose();
        viewListFadeOnIntervalCurve.dispose();
    }

    private void _updateSuggestionsListener() => _ = updateSuggestions();

    public async virtual Future updateSuggestions()
    {
        if (searchValue != _controller.text)
        {
            searchValue = _controller.text;
            IEnumerable<global::Doroti.Framework.Widgets.Widget> suggestions = await DartAsyncRuntime.AwaitFutureOrValue<IEnumerable<global::Doroti.Framework.Widgets.Widget>>(widget.suggestionsBuilder(context, _controller));
            if (mounted)
            {
                setState(() =>
                {
                    result = suggestions;
                });
            }
        }
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Framework.Widgets.Widget defaultLeading = new BackButton(style: new ButtonStyle(tapTargetSize: MaterialTapTargetSize.shrinkWrap), onPressed: () =>
        {
            Navigator.of(context).pop<object>();
        });
        var defaultTrailing = ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() =>
        {
            var __collection36360 = new List<global::Doroti.Framework.Widgets.Widget>(); if (_controller.text.Length != 0)
            {
                __collection36360.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new IconButton(icon: new global::Doroti.Framework.Widgets.Icon(Icons.close), tooltip: MaterialLocalizations.of(context).clearButtonTooltip, onPressed: () =>
                {
                    _controller.clear();
                })));
            }
            return __collection36360;
        }))();
        SearchViewThemeData viewDefaults = new _SearchViewDefaultsM3__search_anchor(context, isFullScreen: widget.showFullScreenView);
        SearchViewThemeData viewTheme = SearchViewTheme.of(context);
        DividerThemeData dividerTheme = DividerTheme.of(context);
        global::Doroti.Ui.Color effectiveBackgroundColor = (widget.viewBackgroundColor ?? viewTheme.backgroundColor) ?? viewDefaults.backgroundColor!;
        global::Doroti.Ui.Color effectiveSurfaceTint = (widget.viewSurfaceTintColor ?? viewTheme.surfaceTintColor) ?? viewDefaults.surfaceTintColor!;
        double effectiveElevation = (widget.viewElevation ?? viewTheme.elevation) ?? DartRuntimePrimitives.RequireValue(viewDefaults.elevation);
        global::Doroti.Framework.Painting.BorderSide? effectiveSide = (widget.viewSide ?? viewTheme.side) ?? viewDefaults.side;
        global::Doroti.Framework.Painting.OutlinedBorder effectiveShape = (widget.viewShape ?? viewTheme.shape) ?? viewDefaults.shape!;
        if (effectiveSide is not null)
        {
            effectiveShape = effectiveShape.copyWith(side: effectiveSide);
        }
        global::Doroti.Ui.Color effectiveDividerColor = ((widget.dividerColor ?? viewTheme.dividerColor) ?? dividerTheme.color) ?? viewDefaults.dividerColor!;
        double? effectiveHeaderHeight = widget.viewHeaderHeight ?? viewTheme.headerHeight;
        global::Doroti.Framework.Rendering.BoxConstraints? headerConstraints = (effectiveHeaderHeight is null) ? null : BoxConstraints.CreateTightFor(height: DartRuntimePrimitives.RequireValue(effectiveHeaderHeight));
        global::Doroti.Framework.Painting.TextStyle? effectiveTextStyle = (widget.viewHeaderTextStyle ?? viewTheme.headerTextStyle) ?? viewDefaults.headerTextStyle;
        global::Doroti.Framework.Painting.TextStyle? effectiveHintStyle = (((widget.viewHeaderHintStyle ?? viewTheme.headerHintStyle) ?? widget.viewHeaderTextStyle) ?? viewTheme.headerTextStyle) ?? viewDefaults.headerHintStyle;
        global::Doroti.Framework.Painting.EdgeInsetsGeometry? effectivePadding = (widget.viewPadding ?? viewTheme.padding) ?? viewDefaults.padding;
        global::Doroti.Framework.Painting.EdgeInsetsGeometry? effectiveBarPadding = (widget.viewBarPadding ?? viewTheme.barPadding) ?? viewDefaults.barPadding;
        global::Doroti.Framework.Rendering.BoxConstraints effectiveConstraints = (widget.viewConstraints ?? viewTheme.constraints) ?? viewDefaults.constraints!;
        double minWidthLocal = Math.Min(effectiveConstraints.minWidth, _viewRect.width);
        double minHeightLocal = Math.Min(effectiveConstraints.minHeight, _viewRect.height);
        bool effectiveShrinkWrap = (widget.shrinkWrap ?? viewTheme.shrinkWrap) ?? DartRuntimePrimitives.RequireValue(viewDefaults.shrinkWrap);
        global::Doroti.Framework.Widgets.Widget viewDivider = new DividerTheme(data: dividerTheme.copyWith(color: effectiveDividerColor), child: new Divider(height: 1));
        return new global::Doroti.Framework.Widgets.Align(alignment: Alignment.topLeft, child: Transform.CreateTranslate(offset: _viewRect.topLeft, child: new global::Doroti.Framework.Widgets.ConstrainedBox(constraints: new global::Doroti.Framework.Rendering.BoxConstraints(minWidth: minWidthLocal, maxWidth: _viewRect.width, minHeight: minHeightLocal, maxHeight: _viewRect.height), child: new global::Doroti.Framework.Widgets.Padding(padding: widget.showFullScreenView ? EdgeInsets.zero : (effectivePadding ?? EdgeInsets.zero), child: new Material(clipBehavior: Clip.antiAlias, shape: effectiveShape, color: effectiveBackgroundColor, surfaceTintColor: effectiveSurfaceTint, elevation: effectiveElevation, child: new global::Doroti.Framework.Widgets.OverflowBox(alignment: Alignment.topLeft, maxWidth: Math.Min(widget.viewMaxWidth, DartRuntimePrimitives.RequireValue(_screenSize).width), minWidth: 0, fit: OverflowBoxFit.deferToChild, child: new global::Doroti.Framework.Widgets.FadeTransition(opacity: viewIconsFadeCurve, child: new global::Doroti.Framework.Widgets.Column(mainAxisSize: MainAxisSize.min, crossAxisAlignment: CrossAxisAlignment.stretch, children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() =>
        {
            var __collection40516 = new List<global::Doroti.Framework.Widgets.Widget>(); __collection40516.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Padding(padding: EdgeInsets.CreateOnly(top: widget.topPadding), child: new global::Doroti.Framework.Widgets.SafeArea(top: false, bottom: false, child: new SearchBar(autoFocus: true, constraints: headerConstraints ?? (widget.showFullScreenView ? new global::Doroti.Framework.Rendering.BoxConstraints(minHeight: _SearchViewDefaultsM3__search_anchor.fullScreenBarHeight) : null), padding: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Framework.Painting.EdgeInsetsGeometry?>(effectiveBarPadding), leading: widget.viewLeading ?? defaultLeading, trailing: (widget.viewTrailing ?? defaultTrailing).Cast<global::Doroti.Framework.Widgets.Widget>(), hintText: widget.viewHintText, backgroundColor: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Ui.Color>(Colors.transparent), overlayColor: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Ui.Color>(Colors.transparent), elevation: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<double?>(0.0), textStyle: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Framework.Painting.TextStyle?>(effectiveTextStyle), hintStyle: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Framework.Painting.TextStyle?>(effectiveHintStyle), controller: _controller, onChanged: (value) =>
            {
                widget.viewOnChanged?.Invoke(value);
                DartRuntimePrimitives.Ignore(updateSuggestions());
            }, onSubmitted: widget.viewOnSubmitted, textCapitalization: widget.textCapitalization, textInputAction: widget.textInputAction, keyboardType: widget.keyboardType, smartDashesType: widget.smartDashesType, smartQuotesType: widget.smartQuotesType))))); if (!effectiveShrinkWrap || (minHeightLocal > 0L) || widget.showFullScreenView || Enumerable.Any(result)) { __collection40516.AddRange(new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.FadeTransition(opacity: viewDividerFadeCurve, child: viewDivider)), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Flexible(fit: (effectiveShrinkWrap && !widget.showFullScreenView) ? FlexFit.loose : FlexFit.tight, child: new global::Doroti.Framework.Widgets.FadeTransition(opacity: viewListFadeOnIntervalCurve, child: (widget.viewBuilder is null) ? MediaQuery.CreateRemovePadding(context: context, removeTop: true, child: new global::Doroti.Framework.Widgets.ListView(padding: EdgeInsets.CreateOnly(bottom: MediaQuery.viewInsetsOf(context).bottom), shrinkWrap: effectiveShrinkWrap, children: result.ToList())) : widget.viewBuilder!(result)))) }); }
            return __collection40516;
        }))()))))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _SearchAnchorWithSearchBar__search_anchor : SearchAnchor
{
    internal _SearchAnchorWithSearchBar__search_anchor(global::Doroti.Framework.Widgets.Widget? barLeading = null, IEnumerable<global::Doroti.Framework.Widgets.Widget>? barTrailing = null, string? barHintText = null, global::System.Action? onTap = null, global::Doroti.Framework.Widgets.WidgetStateProperty<double?>? barElevation = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? barBackgroundColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? barOverlayColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.BorderSide?>? barSide = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.OutlinedBorder?>? barShape = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry?>? barPadding = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? viewBarPadding = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.TextStyle?>? barTextStyle = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.TextStyle?>? barHintStyle = null, global::System.Func<IEnumerable<global::Doroti.Framework.Widgets.Widget>, global::Doroti.Framework.Widgets.Widget>? viewBuilder = null, global::Doroti.Framework.Widgets.Widget? viewLeading = null, IEnumerable<global::Doroti.Framework.Widgets.Widget>? viewTrailing = null, string? viewHintText = null, Color? viewBackgroundColor = null, double? viewElevation = null, global::Doroti.Framework.Painting.BorderSide? viewSide = null, global::Doroti.Framework.Painting.OutlinedBorder? viewShape = null, double? viewHeaderHeight = null, global::Doroti.Framework.Painting.TextStyle? viewHeaderTextStyle = null, global::Doroti.Framework.Painting.TextStyle? viewHeaderHintStyle = null, Color? dividerColor = null, global::Doroti.Framework.Rendering.BoxConstraints? constraints = null, global::Doroti.Framework.Rendering.BoxConstraints? viewConstraints = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? viewPadding = null, bool? shrinkWrap = null, bool? isFullScreen = null, SearchController? searchController = null, global::Doroti.Framework.Services.TextCapitalization? textCapitalization = null, global::System.Action<string>? onChanged = null, global::System.Action<string>? onSubmitted = null, global::System.Action? onClose = null, global::System.Action? onOpen = null, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, SearchController, object> suggestionsBuilder = default!, global::Doroti.Framework.Services.TextInputAction? textInputAction = null, global::Doroti.Framework.Services.TextInputType? keyboardType = null, global::Doroti.Framework.Painting.EdgeInsets scrollPadding = default!, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.EditableTextState, global::Doroti.Framework.Widgets.Widget> contextMenuBuilder = default!, bool enabled = true, global::Doroti.Framework.Services.SmartDashesType? smartDashesType = null, global::Doroti.Framework.Services.SmartQuotesType? smartQuotesType = null) : base(viewBarPadding: viewBarPadding, viewBuilder: viewBuilder, viewLeading: viewLeading, viewTrailing: viewTrailing, viewBackgroundColor: viewBackgroundColor, viewElevation: viewElevation, viewSide: viewSide, viewShape: viewShape, dividerColor: dividerColor, viewConstraints: viewConstraints, viewPadding: viewPadding, shrinkWrap: shrinkWrap, isFullScreen: isFullScreen, searchController: searchController, textCapitalization: textCapitalization, suggestionsBuilder: suggestionsBuilder, textInputAction: textInputAction, keyboardType: keyboardType, enabled: enabled, smartDashesType: smartDashesType, smartQuotesType: smartQuotesType, viewHintText: viewHintText ?? barHintText, headerHeight: viewHeaderHeight, headerTextStyle: viewHeaderTextStyle, headerHintStyle: viewHeaderHintStyle, viewOnSubmitted: onSubmitted, viewOnChanged: onChanged, viewOnClose: onClose, viewOnOpen: onOpen, builder: (context, controller) =>
    {
        return new SearchBar(constraints: constraints, controller: controller, onTap: () =>
        {
            controller.openView();
            onTap?.Invoke();
        }, onChanged: (value) =>
        {
            controller.openView();
        }, onSubmitted: onSubmitted, hintText: barHintText, hintStyle: barHintStyle, textStyle: barTextStyle, elevation: barElevation, backgroundColor: barBackgroundColor, overlayColor: barOverlayColor, side: barSide, shape: barShape, padding: DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry?>>((object?)barPadding ?? new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Framework.Painting.EdgeInsetsGeometry?>(EdgeInsets.CreateSymmetric(horizontal: 16.0))), leading: barLeading ?? new global::Doroti.Framework.Widgets.Icon(Icons.search), trailing: barTrailing, textCapitalization: textCapitalization, textInputAction: textInputAction, keyboardType: keyboardType, scrollPadding: scrollPadding ?? EdgeInsets.CreateAll(20.0), contextMenuBuilder: contextMenuBuilder ?? SearchBar._defaultContextMenuBuilder, smartDashesType: smartDashesType, smartQuotesType: smartQuotesType);
        throw new InvalidOperationException("Dart closure completed without a value.");
    })
    {
    }

}

public class SearchController : global::Doroti.Framework.Widgets.TextEditingController
{
    internal virtual _SearchAnchorState__search_anchor? _anchor { get; set; } = default;

    public virtual bool isAttached => DartRuntimePrimitives.ConvertValue<bool>(_anchor is not null);
    public virtual bool isOpen
    {
        get
        {
            DartRuntimePrimitives.Assert(() => isAttached);
            return _anchor!._viewIsOpen;
        }
    }
    public virtual void openView()
    {
        DartRuntimePrimitives.Assert(() => isAttached);
        _anchor!._openView();
    }

    public virtual void closeView(string? selectedText)
    {
        DartRuntimePrimitives.Assert(() => isAttached);
        _anchor!._closeView(selectedText);
    }

    internal virtual void _attach(_SearchAnchorState__search_anchor anchor)
    {
        _anchor = anchor;
    }

    internal virtual void _detach(_SearchAnchorState__search_anchor anchor)
    {
        if (Equals(_anchor, anchor))
        {
            _anchor = null;
        }
    }

}

public class SearchBar : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual global::Doroti.Framework.Widgets.TextEditingController? controller { get; private set; }
    public virtual global::Doroti.Framework.Widgets.FocusNode? focusNode { get; private set; }
    public virtual string? hintText { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? leading { get; private set; }
    public virtual IEnumerable<global::Doroti.Framework.Widgets.Widget>? trailing { get; private set; }
    public virtual global::System.Action? onTap { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Gestures.PointerDownEvent>? onTapOutside { get; private set; }
    public virtual global::System.Action<string>? onChanged { get; private set; }
    public virtual global::System.Action<string>? onSubmitted { get; private set; }
    public virtual global::Doroti.Framework.Rendering.BoxConstraints? constraints { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<double?>? elevation { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? backgroundColor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? shadowColor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? surfaceTintColor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.BorderSide?>? side { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.OutlinedBorder?>? shape { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry?>? padding { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.TextStyle?>? textStyle { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.TextStyle?>? hintStyle { get; private set; }
    public virtual global::Doroti.Framework.Services.TextCapitalization? textCapitalization { get; private set; }
    public virtual bool enabled { get; private set; } = default!;
    public virtual bool autoFocus { get; private set; } = default!;
    public virtual global::Doroti.Framework.Services.TextInputAction? textInputAction { get; private set; }
    public virtual global::Doroti.Framework.Services.TextInputType? keyboardType { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsets scrollPadding { get; private set; } = default!;
    public virtual global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.EditableTextState, global::Doroti.Framework.Widgets.Widget>? contextMenuBuilder { get; private set; }
    public virtual bool readOnly { get; private set; } = default!;
    public virtual global::Doroti.Framework.Services.SmartDashesType? smartDashesType { get; private set; }
    public virtual global::Doroti.Framework.Services.SmartQuotesType? smartQuotesType { get; private set; }

    public SearchBar(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Widgets.TextEditingController? controller = null, global::Doroti.Framework.Widgets.FocusNode? focusNode = null, string? hintText = null, global::Doroti.Framework.Widgets.Widget? leading = null, IEnumerable<global::Doroti.Framework.Widgets.Widget>? trailing = null, global::System.Action? onTap = null, global::System.Action<global::Doroti.Framework.Gestures.PointerDownEvent>? onTapOutside = null, global::System.Action<string>? onChanged = null, global::System.Action<string>? onSubmitted = null, global::Doroti.Framework.Rendering.BoxConstraints? constraints = null, global::Doroti.Framework.Widgets.WidgetStateProperty<double?>? elevation = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? backgroundColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? shadowColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? surfaceTintColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.BorderSide?>? side = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.OutlinedBorder?>? shape = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry?>? padding = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.TextStyle?>? textStyle = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.TextStyle?>? hintStyle = null, global::Doroti.Framework.Services.TextCapitalization? textCapitalization = null, bool enabled = true, bool autoFocus = false, global::Doroti.Framework.Services.TextInputAction? textInputAction = null, global::Doroti.Framework.Services.TextInputType? keyboardType = null, global::Doroti.Framework.Painting.EdgeInsets scrollPadding = default!, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.EditableTextState, global::Doroti.Framework.Widgets.Widget>? contextMenuBuilder = default!, bool readOnly = false, global::Doroti.Framework.Services.SmartDashesType? smartDashesType = null, global::Doroti.Framework.Services.SmartQuotesType? smartQuotesType = null) : base(key: key)
    {
        global::Doroti.Framework.Painting.EdgeInsets __scrollPadding = scrollPadding ?? EdgeInsets.CreateAll(20.0);
        global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.EditableTextState, global::Doroti.Framework.Widgets.Widget>? __contextMenuBuilder = contextMenuBuilder ?? _defaultContextMenuBuilder;
        this.controller = controller;
        this.focusNode = focusNode;
        this.hintText = hintText;
        this.leading = leading;
        this.trailing = trailing;
        this.onTap = onTap;
        this.onTapOutside = onTapOutside;
        this.onChanged = onChanged;
        this.onSubmitted = onSubmitted;
        this.constraints = constraints;
        this.elevation = elevation;
        this.backgroundColor = backgroundColor;
        this.shadowColor = shadowColor;
        this.surfaceTintColor = surfaceTintColor;
        this.overlayColor = overlayColor;
        this.side = side;
        this.shape = shape;
        this.padding = padding;
        this.textStyle = textStyle;
        this.hintStyle = hintStyle;
        this.textCapitalization = textCapitalization;
        this.enabled = enabled;
        this.autoFocus = autoFocus;
        this.textInputAction = textInputAction;
        this.keyboardType = keyboardType;
        this.scrollPadding = __scrollPadding;
        this.contextMenuBuilder = __contextMenuBuilder;
        this.readOnly = readOnly;
        this.smartDashesType = smartDashesType;
        this.smartQuotesType = smartQuotesType;
    }

    internal static global::Doroti.Framework.Widgets.Widget _defaultContextMenuBuilder(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.EditableTextState editableTextState)
    {
        if (SystemContextMenu.isSupportedByField(editableTextState))
        {
            return SystemContextMenu.CreateEditableText(editableTextState: editableTextState);
        }
        return AdaptiveTextSelectionToolbar.CreateEditableText(editableTextState: editableTextState);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _SearchBarState__search_anchor());
}

internal class _SearchBarState__search_anchor : global::Doroti.Framework.Widgets.State<SearchBar>
{
    internal virtual global::Doroti.Framework.Widgets.WidgetStatesController _internalStatesController { get; private set; } = default!;
    internal virtual global::Doroti.Framework.Widgets.FocusNode? _internalFocusNode { get; set; } = default;

    internal virtual global::Doroti.Framework.Widgets.FocusNode _focusNode => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.FocusNode>(widget.focusNode ?? (_internalFocusNode ??= new global::Doroti.Framework.Widgets.FocusNode()));
    public override void initState()
    {
        base.initState();
        _internalStatesController = new global::Doroti.Framework.Widgets.WidgetStatesController();
        _internalStatesController.addListener(() =>
        {
            setState(() =>
            {
            });
        });
    }

    public override void dispose()
    {
        _internalStatesController.dispose();
        _internalFocusNode?.dispose();
        base.dispose();
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Ui.TextDirection textDirectionLocal = Directionality.of(context);
        ColorScheme colorSchemeLocal = Theme.of(context).colorScheme;
        SearchBarThemeData searchBarTheme = SearchBarTheme.of(context);
        SearchBarThemeData defaults = new _SearchBarDefaultsM3__search_anchor(context);
        P? resolve<P>(global::Doroti.Framework.Widgets.WidgetStateProperty<P>? widgetValue, global::Doroti.Framework.Widgets.WidgetStateProperty<P>? themeValue, global::Doroti.Framework.Widgets.WidgetStateProperty<P>? defaultValue)
        {
            HashSet<global::Doroti.Framework.Widgets.WidgetState> states = _internalStatesController.value;
            return widgetValue is not null ? widgetValue.resolve(states) : themeValue is not null ? themeValue.resolve(states) : defaultValue is not null ? defaultValue.resolve(states) : default;
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        global::Doroti.Framework.Painting.TextStyle? effectiveTextStyle = resolve<global::Doroti.Framework.Painting.TextStyle?>(widget.textStyle, searchBarTheme.textStyle, defaults.textStyle);
        double? effectiveElevation = resolve<double?>(widget.elevation, searchBarTheme.elevation, defaults.elevation);
        global::Doroti.Ui.Color? effectiveShadowColor = resolve<global::Doroti.Ui.Color?>(widget.shadowColor, searchBarTheme.shadowColor, defaults.shadowColor);
        global::Doroti.Ui.Color? effectiveBackgroundColor = resolve<global::Doroti.Ui.Color?>(widget.backgroundColor, searchBarTheme.backgroundColor, defaults.backgroundColor);
        global::Doroti.Ui.Color? effectiveSurfaceTintColor = resolve<global::Doroti.Ui.Color?>(widget.surfaceTintColor, searchBarTheme.surfaceTintColor, defaults.surfaceTintColor);
        global::Doroti.Framework.Painting.OutlinedBorder? effectiveShape = resolve<global::Doroti.Framework.Painting.OutlinedBorder?>(widget.shape, searchBarTheme.shape, defaults.shape);
        global::Doroti.Framework.Painting.BorderSide? effectiveSide = resolve<global::Doroti.Framework.Painting.BorderSide?>(widget.side, searchBarTheme.side, defaults.side);
        global::Doroti.Framework.Painting.EdgeInsetsGeometry? effectivePadding = resolve<global::Doroti.Framework.Painting.EdgeInsetsGeometry?>(widget.padding, searchBarTheme.padding, defaults.padding);
        global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? effectiveOverlayColor = (widget.overlayColor ?? searchBarTheme.overlayColor) ?? defaults.overlayColor;
        global::Doroti.Framework.Services.TextCapitalization effectiveTextCapitalization = (widget.textCapitalization ?? searchBarTheme.textCapitalization) ?? DartRuntimePrimitives.RequireValue(defaults.textCapitalization);
        HashSet<global::Doroti.Framework.Widgets.WidgetState> statesLocal = _internalStatesController.value;
        global::Doroti.Framework.Painting.TextStyle? effectiveHintStyle = (((widget.hintStyle?.resolve(statesLocal) ?? (searchBarTheme.hintStyle?.resolve(statesLocal))) ?? (widget.textStyle?.resolve(statesLocal))) ?? (searchBarTheme.textStyle?.resolve(statesLocal))) ?? (defaults.hintStyle?.resolve(statesLocal));
        global::Doroti.Ui.Color defaultColor = colorSchemeLocal.brightness switch { Brightness.light => ConstantsLibrary.kDefaultIconDarkColor, Brightness.dark => ConstantsLibrary.kDefaultIconLightColor, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        global::Doroti.Framework.Widgets.IconThemeData? customTheme = IconTheme.of(context) switch { global::Doroti.Framework.Widgets.IconThemeData iconTheme when !Equals(iconTheme.color, defaultColor) => iconTheme, _ => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.IconThemeData>(null) };
        global::Doroti.Framework.Widgets.Widget? leadingLocal = default!;
        if (widget.leading is not null)
        {
            leadingLocal = IconTheme.merge(data: customTheme ?? new global::Doroti.Framework.Widgets.IconThemeData(color: colorSchemeLocal.onSurface), child: widget.leading!);
        }
        List<global::Doroti.Framework.Widgets.Widget>? trailingLocal = widget.trailing?.map<global::Doroti.Framework.Widgets.Widget, global::Doroti.Framework.Widgets.Widget>((trailing) => IconTheme.merge(data: customTheme ?? new global::Doroti.Framework.Widgets.IconThemeData(color: colorSchemeLocal.onSurfaceVariant), child: trailing)).ToList().ToList();
        return new global::Doroti.Framework.Widgets.ConstrainedBox(constraints: (widget.constraints ?? searchBarTheme.constraints) ?? defaults.constraints!, child: new global::Doroti.Framework.Widgets.Opacity(opacity: widget.enabled ? 1 : Search_anchorLibrary._kDisableSearchBarOpacity, child: new Material(elevation: DartRuntimePrimitives.RequireValue(effectiveElevation), shadowColor: effectiveShadowColor, color: effectiveBackgroundColor, surfaceTintColor: effectiveSurfaceTintColor, shape: effectiveShape?.copyWith(side: effectiveSide), child: new global::Doroti.Framework.Widgets.IgnorePointer(ignoring: !widget.enabled, child: new InkWell(onTap: () =>
        {
            widget.onTap?.Invoke();
            if (!_focusNode.hasFocus)
            {
                _focusNode.requestFocus();
            }
        }, overlayColor: effectiveOverlayColor, customBorder: effectiveShape?.copyWith(side: effectiveSide), statesController: _internalStatesController, child: new global::Doroti.Framework.Widgets.Padding(padding: effectivePadding!, child: new global::Doroti.Framework.Widgets.Row(textDirection: textDirectionLocal, children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection64758 = new List<global::Doroti.Framework.Widgets.Widget>(); var __collectionElement64788 = leadingLocal; if (__collectionElement64788 is { } __nonNullCollectionElement64788) { __collection64758.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(__nonNullCollectionElement64788)); } __collection64758.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Expanded(child: new global::Doroti.Framework.Widgets.Padding(padding: DartRuntimePrimitives.RequireReference(effectivePadding), child: new global::Doroti.Framework.Widgets.Semantics(inputType: SemanticsInputType.search, child: new TextField(readOnly: widget.readOnly, autofocus: widget.autoFocus, onTap: widget.onTap, onTapAlwaysCalled: true, onTapOutside: widget.onTapOutside, focusNode: _focusNode, onChanged: widget.onChanged, onSubmitted: widget.onSubmitted, controller: widget.controller, style: effectiveTextStyle, enabled: widget.enabled, decoration: new InputDecoration(hintText: widget.hintText).applyDefaults(new InputDecorationThemeData(hintStyle: effectiveHintStyle, enabledBorder: InputBorder.none, border: InputBorder.none, focusedBorder: InputBorder.none, contentPadding: EdgeInsets.zero, isDense: true)), textCapitalization: effectiveTextCapitalization, textInputAction: widget.textInputAction, keyboardType: widget.keyboardType, scrollPadding: widget.scrollPadding, contextMenuBuilder: widget.contextMenuBuilder, smartDashesType: widget.smartDashesType, smartQuotesType: widget.smartQuotesType)))))); var __collectionSpread67233 = trailingLocal; if (__collectionSpread67233 is not null) { __collection64758.AddRange(__collectionSpread67233); } return __collection64758; }))())))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _SearchBarDefaultsM3__search_anchor : SearchBarThemeData
{
    public virtual global::Doroti.Framework.Widgets.BuildContext context { get; private set; } = default!;
    private bool __late__colors_initialized;
    private ColorScheme __late__colors = default!;
    internal virtual ColorScheme _colors
    {
        get
        {
            if (!__late__colors_initialized)
            {
                __late__colors = Theme.of(context).colorScheme;
                __late__colors_initialized = true;
            }
            return __late__colors;
        }
    }
    private bool __late__textTheme_initialized;
    private TextTheme __late__textTheme = default!;
    internal virtual TextTheme _textTheme
    {
        get
        {
            if (!__late__textTheme_initialized)
            {
                __late__textTheme = Theme.of(context).textTheme;
                __late__textTheme_initialized = true;
            }
            return __late__textTheme;
        }
    }

    internal _SearchBarDefaultsM3__search_anchor(global::Doroti.Framework.Widgets.BuildContext context)
    {
        this.context = context;
    }

    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? backgroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Ui.Color>(_colors.surfaceContainerHigh));
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<double?>? elevation => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<double?>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<double?>(6.0));
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color>? shadowColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Ui.Color>(_colors.shadow));
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color>? surfaceTintColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Ui.Color>(Colors.transparent));
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? overlayColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>(WidgetStateProperty.resolveWith((states) =>
    {
        if (states.Contains(WidgetState.pressed))
        {
            return _colors.onSurface.withOpacity(0.1);
        }
        if (states.Contains(WidgetState.hovered))
        {
            return _colors.onSurface.withOpacity(0.08);
        }
        if (states.Contains(WidgetState.focused))
        {
            return Colors.transparent;
        }
        return Colors.transparent;
        throw new InvalidOperationException("Dart closure completed without a value.");
    }));
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.OutlinedBorder>? shape => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.OutlinedBorder>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Framework.Painting.OutlinedBorder>(new global::Doroti.Framework.Painting.StadiumBorder()));
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry>? padding => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Framework.Painting.EdgeInsetsGeometry>(EdgeInsets.CreateSymmetric(horizontal: 8.0)));
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.TextStyle?> textStyle => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.TextStyle?>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Framework.Painting.TextStyle?>(_textTheme.bodyLarge?.copyWith(color: _colors.onSurface)));
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.TextStyle?> hintStyle => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.TextStyle?>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Framework.Painting.TextStyle?>(_textTheme.bodyLarge?.copyWith(color: _colors.onSurfaceVariant)));
    public override global::Doroti.Framework.Rendering.BoxConstraints constraints => new global::Doroti.Framework.Rendering.BoxConstraints(minWidth: 360.0, maxWidth: 800.0, minHeight: 56.0);
    public override global::Doroti.Framework.Services.TextCapitalization? textCapitalization => TextCapitalization.none;
}

internal class _SearchViewDefaultsM3__search_anchor : SearchViewThemeData
{
    public virtual global::Doroti.Framework.Widgets.BuildContext context { get; private set; } = default!;
    public virtual bool isFullScreen { get; private set; } = default!;
    private bool __late__colors_initialized;
    private ColorScheme __late__colors = default!;
    internal virtual ColorScheme _colors
    {
        get
        {
            if (!__late__colors_initialized)
            {
                __late__colors = Theme.of(context).colorScheme;
                __late__colors_initialized = true;
            }
            return __late__colors;
        }
    }
    private bool __late__textTheme_initialized;
    private TextTheme __late__textTheme = default!;
    internal virtual TextTheme _textTheme
    {
        get
        {
            if (!__late__textTheme_initialized)
            {
                __late__textTheme = Theme.of(context).textTheme;
                __late__textTheme_initialized = true;
            }
            return __late__textTheme;
        }
    }
    public static double fullScreenBarHeight = 72.0;

    internal _SearchViewDefaultsM3__search_anchor(global::Doroti.Framework.Widgets.BuildContext context, bool isFullScreen)
    {
        this.context = context;
        this.isFullScreen = isFullScreen;
    }

    public override global::Doroti.Ui.Color? backgroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_colors.surfaceContainerHigh);
    public override double? elevation => 6.0;
    public override global::Doroti.Ui.Color? surfaceTintColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(Colors.transparent);
    public override global::Doroti.Framework.Painting.OutlinedBorder? shape => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.OutlinedBorder>(isFullScreen ? new global::Doroti.Framework.Painting.RoundedRectangleBorder() : new global::Doroti.Framework.Painting.RoundedRectangleBorder(borderRadius: BorderRadius.CreateAll(Radius.circular(28.0))));
    public override global::Doroti.Framework.Painting.TextStyle? headerTextStyle => _textTheme.bodyLarge?.copyWith(color: _colors.onSurface);
    public override global::Doroti.Framework.Painting.TextStyle? headerHintStyle => _textTheme.bodyLarge?.copyWith(color: _colors.onSurfaceVariant);
    public override global::Doroti.Framework.Rendering.BoxConstraints constraints => new global::Doroti.Framework.Rendering.BoxConstraints(minWidth: 360.0, minHeight: 240.0);
    public override global::Doroti.Framework.Painting.EdgeInsetsGeometry? barPadding => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.EdgeInsetsGeometry>(EdgeInsets.CreateSymmetric(horizontal: 8.0));
    public override bool? shrinkWrap => false;
    public override global::Doroti.Ui.Color? dividerColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_colors.outline);
}
