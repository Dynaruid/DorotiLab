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
    internal static Duration _kOpenViewDuration = Duration.Create(
        milliseconds: _kOpenViewMilliseconds
    );
}

public static partial class Search_anchorLibrary
{
    internal static Duration _kAnchorFadeDuration = Duration.Create(milliseconds: 150L);
}

public static partial class Search_anchorLibrary
{
    internal static Curve _kViewFadeOnInterval = new Interval(0.0, 1L / 2L);
}

public static partial class Search_anchorLibrary
{
    internal static Curve _kViewIconsFadeOnInterval = new Interval(1L / 6L, 2L / 6L);
}

public static partial class Search_anchorLibrary
{
    internal static Curve _kViewDividerFadeOnInterval = new Interval(0.0, 1L / 6L);
}

public static partial class Search_anchorLibrary
{
    internal static Curve _kViewListFadeOnInterval = new Interval(
        133L / _kOpenViewMilliseconds,
        233L / _kOpenViewMilliseconds
    );
}

public static partial class Search_anchorLibrary
{
    internal static double _kDisableSearchBarOpacity = 0.38;
}

public delegate Widget SearchAnchorChildBuilder(BuildContext context, SearchController controller);

public delegate object SuggestionsBuilder(BuildContext context, SearchController controller);

public delegate Widget ViewBuilder(IEnumerable<Widget> suggestions);

public class SearchAnchor : StatefulWidget
{
    public virtual bool? isFullScreen { get; private set; }
    public virtual SearchController? searchController { get; private set; }
    public virtual Func<IEnumerable<Widget>, Widget>? viewBuilder { get; private set; }
    public virtual Widget? viewLeading { get; private set; }
    public virtual IEnumerable<Widget>? viewTrailing { get; private set; }
    public virtual string? viewHintText { get; private set; }
    public virtual Color? viewBackgroundColor { get; private set; }
    public virtual double? viewElevation { get; private set; }
    public virtual Color? viewSurfaceTintColor { get; private set; }
    public virtual BorderSide? viewSide { get; private set; }
    public virtual OutlinedBorder? viewShape { get; private set; }
    public virtual EdgeInsetsGeometry? viewBarPadding { get; private set; }
    public virtual double? headerHeight { get; private set; }
    public virtual TextStyle? headerTextStyle { get; private set; }
    public virtual TextStyle? headerHintStyle { get; private set; }
    public virtual Color? dividerColor { get; private set; }
    public virtual BoxConstraints? viewConstraints { get; private set; }
    public virtual EdgeInsetsGeometry? viewPadding { get; private set; }
    public virtual bool? shrinkWrap { get; private set; }
    public virtual TextCapitalization? textCapitalization { get; private set; }
    public virtual Action<string>? viewOnChanged { get; private set; }
    public virtual Action<string>? viewOnSubmitted { get; private set; }
    public virtual Action? viewOnClose { get; private set; }
    public virtual Action? viewOnOpen { get; private set; }
    public virtual Func<BuildContext, SearchController, Widget> builder { get; private set; } =
        default!;
    public virtual Func<BuildContext, SearchController, object> suggestionsBuilder
    {
        get;
        private set;
    } = default!;
    public virtual TextInputAction? textInputAction { get; private set; }
    public virtual TextInputType? keyboardType { get; private set; }
    public virtual bool enabled { get; private set; } = default!;
    public virtual SmartDashesType? smartDashesType { get; private set; }
    public virtual SmartQuotesType? smartQuotesType { get; private set; }

    public SearchAnchor(
        Key? key = null,
        bool? isFullScreen = null,
        SearchController? searchController = null,
        Func<IEnumerable<Widget>, Widget>? viewBuilder = null,
        Widget? viewLeading = null,
        IEnumerable<Widget>? viewTrailing = null,
        string? viewHintText = null,
        Color? viewBackgroundColor = null,
        double? viewElevation = null,
        Color? viewSurfaceTintColor = null,
        BorderSide? viewSide = null,
        OutlinedBorder? viewShape = null,
        EdgeInsetsGeometry? viewBarPadding = null,
        double? headerHeight = null,
        TextStyle? headerTextStyle = null,
        TextStyle? headerHintStyle = null,
        Color? dividerColor = null,
        BoxConstraints? viewConstraints = null,
        EdgeInsetsGeometry? viewPadding = null,
        bool? shrinkWrap = null,
        TextCapitalization? textCapitalization = null,
        Action<string>? viewOnChanged = null,
        Action<string>? viewOnSubmitted = null,
        Action? viewOnClose = null,
        Action? viewOnOpen = null,
        Func<BuildContext, SearchController, Widget> builder = default!,
        Func<BuildContext, SearchController, object> suggestionsBuilder = default!,
        TextInputAction? textInputAction = null,
        TextInputType? keyboardType = null,
        bool enabled = true,
        SmartDashesType? smartDashesType = null,
        SmartQuotesType? smartQuotesType = null
    )
        : base(key: key)
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

    public static SearchAnchor CreateBar(
        Widget? barLeading = null,
        IEnumerable<Widget>? barTrailing = null,
        string? barHintText = null,
        Action? onTap = null,
        Action<string>? onSubmitted = null,
        Action<string>? onChanged = null,
        Action? onClose = null,
        Action? onOpen = null,
        WidgetStateProperty<double?>? barElevation = null,
        WidgetStateProperty<Color?>? barBackgroundColor = null,
        WidgetStateProperty<Color?>? barOverlayColor = null,
        WidgetStateProperty<BorderSide?>? barSide = null,
        WidgetStateProperty<OutlinedBorder?>? barShape = null,
        WidgetStateProperty<EdgeInsetsGeometry?>? barPadding = null,
        EdgeInsetsGeometry? viewBarPadding = null,
        WidgetStateProperty<TextStyle?>? barTextStyle = null,
        WidgetStateProperty<TextStyle?>? barHintStyle = null,
        Func<IEnumerable<Widget>, Widget>? viewBuilder = null,
        Widget? viewLeading = null,
        IEnumerable<Widget>? viewTrailing = null,
        string? viewHintText = null,
        Color? viewBackgroundColor = null,
        double? viewElevation = null,
        BorderSide? viewSide = null,
        OutlinedBorder? viewShape = null,
        double? viewHeaderHeight = null,
        TextStyle? viewHeaderTextStyle = null,
        TextStyle? viewHeaderHintStyle = null,
        Color? dividerColor = null,
        BoxConstraints? constraints = null,
        BoxConstraints? viewConstraints = null,
        EdgeInsetsGeometry? viewPadding = null,
        bool? shrinkWrap = null,
        bool? isFullScreen = null,
        SearchController searchController = default!,
        TextCapitalization textCapitalization = default!,
        Func<BuildContext, SearchController, object> suggestionsBuilder = default!,
        TextInputAction? textInputAction = null,
        TextInputType? keyboardType = null,
        EdgeInsets scrollPadding = default!,
        Func<BuildContext, EditableTextState, Widget> contextMenuBuilder = default!,
        bool enabled = true,
        SmartDashesType? smartDashesType = null,
        SmartQuotesType? smartQuotesType = null
    ) =>
        new _SearchAnchorWithSearchBar__search_anchor(
            barLeading: barLeading,
            barTrailing: barTrailing,
            barHintText: barHintText,
            onTap: onTap,
            onSubmitted: onSubmitted,
            onChanged: onChanged,
            onClose: onClose,
            onOpen: onOpen,
            barElevation: barElevation,
            barBackgroundColor: barBackgroundColor,
            barOverlayColor: barOverlayColor,
            barSide: barSide,
            barShape: barShape,
            barPadding: barPadding,
            viewBarPadding: viewBarPadding,
            barTextStyle: barTextStyle,
            barHintStyle: barHintStyle,
            viewBuilder: viewBuilder,
            viewLeading: viewLeading,
            viewTrailing: viewTrailing,
            viewHintText: viewHintText,
            viewBackgroundColor: viewBackgroundColor,
            viewElevation: viewElevation,
            viewSide: viewSide,
            viewShape: viewShape,
            viewHeaderHeight: viewHeaderHeight,
            viewHeaderTextStyle: viewHeaderTextStyle,
            viewHeaderHintStyle: viewHeaderHintStyle,
            dividerColor: dividerColor,
            constraints: constraints,
            viewConstraints: viewConstraints,
            viewPadding: viewPadding,
            shrinkWrap: shrinkWrap,
            isFullScreen: isFullScreen,
            searchController: searchController,
            textCapitalization: textCapitalization,
            suggestionsBuilder: suggestionsBuilder,
            textInputAction: textInputAction,
            keyboardType: keyboardType,
            scrollPadding: scrollPadding,
            contextMenuBuilder: contextMenuBuilder,
            enabled: enabled,
            smartDashesType: smartDashesType,
            smartQuotesType: smartQuotesType
        );

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _SearchAnchorState__search_anchor());
}

internal class _SearchAnchorState__search_anchor : State<SearchAnchor>
{
    internal virtual Size? _screenSize { get; set; } = default;
    internal virtual bool _anchorIsVisible { get; set; } = true;
    internal virtual GlobalKey<IState> _anchorKey { get; private set; } =
        GlobalKey<IState>.Create();
    internal virtual SearchController? _internalSearchController { get; set; } = default;
    internal virtual _SearchViewRoute__search_anchor? _route { get; set; } = default;

    internal virtual bool _viewIsOpen => !_anchorIsVisible;
    internal virtual SearchController _searchController =>
        DartRuntimePrimitives.ConvertValue<SearchController>(
            widget.searchController ?? (_internalSearchController ??= new SearchController())
        );

    public override void initState()
    {
        base.initState();
        _searchController._attach(this);
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        Size updatedScreenSize = MediaQuery.of(context).size;
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
        NavigatorState navigator = Navigator.of(context);
        _route = new _SearchViewRoute__search_anchor(
            viewOnChanged: widget.viewOnChanged,
            viewOnSubmitted: widget.viewOnSubmitted,
            viewOnClose: widget.viewOnClose,
            viewOnOpen: widget.viewOnOpen,
            viewLeading: widget.viewLeading,
            viewTrailing: widget.viewTrailing,
            viewHintText: widget.viewHintText,
            viewBackgroundColor: widget.viewBackgroundColor,
            viewElevation: widget.viewElevation,
            viewSurfaceTintColor: widget.viewSurfaceTintColor,
            viewSide: widget.viewSide,
            viewShape: widget.viewShape,
            viewBarPadding: widget.viewBarPadding,
            viewHeaderHeight: widget.headerHeight,
            viewHeaderTextStyle: widget.headerTextStyle,
            viewHeaderHintStyle: widget.headerHintStyle,
            dividerColor: widget.dividerColor,
            viewConstraints: widget.viewConstraints,
            viewPadding: widget.viewPadding,
            shrinkWrap: widget.shrinkWrap,
            showFullScreenView: getShowFullScreenView(),
            toggleVisibility: toggleVisibility,
            textDirection: Directionality.of(context),
            viewBuilder: widget.viewBuilder,
            anchorKey: _anchorKey,
            searchController: _searchController,
            suggestionsBuilder: widget.suggestionsBuilder,
            textCapitalization: widget.textCapitalization,
            capturedThemes: InheritedTheme.capture(from: context, to: navigator.context),
            textInputAction: widget.textInputAction,
            keyboardType: widget.keyboardType,
            smartDashesType: widget.smartDashesType,
            smartQuotesType: widget.smartQuotesType
        );
        DartRuntimePrimitives.Ignore(navigator.push(_route!));
    }

    internal virtual void _closeView(string? selectedText)
    {
        if (selectedText is not null)
        {
            _searchController.value = new TextEditingValue(text: selectedText);
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
        return widget.isFullScreen
            ?? (
                Theme.of(context).platform switch
                {
                    TargetPlatform.iOS or TargetPlatform.android => true,
                    TargetPlatform.fuchsia => true,
                    TargetPlatform.macOS or TargetPlatform.linux => false,
                    TargetPlatform.windows => false,
                    _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                        throw new InvalidOperationException("Non-exhaustive Dart switch value."),
                }
            );
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

    public override Widget build(BuildContext context)
    {
        return new AnimatedOpacity(
            key: _anchorKey,
            opacity: _getOpacity(),
            duration: Search_anchorLibrary._kAnchorFadeDuration,
            child: new IgnorePointer(
                ignoring: !widget.enabled,
                child: new GestureDetector(
                    onTap: () => _openView(),
                    child: widget.builder(context, _searchController)
                )
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _SearchViewRoute__search_anchor : PopupRoute<_SearchViewRoute__search_anchor>
{
    public virtual Action<string>? viewOnChanged { get; private set; }
    public virtual Action<string>? viewOnSubmitted { get; private set; }
    public virtual Action? viewOnClose { get; private set; }
    public virtual Action? viewOnOpen { get; private set; }
    public virtual Func<bool>? toggleVisibility { get; private set; }
    public virtual TextDirection? textDirection { get; private set; }
    public virtual Func<IEnumerable<Widget>, Widget>? viewBuilder { get; private set; }
    public virtual Widget? viewLeading { get; private set; }
    public virtual IEnumerable<Widget>? viewTrailing { get; private set; }
    public virtual string? viewHintText { get; private set; }
    public virtual Color? viewBackgroundColor { get; private set; }
    public virtual double? viewElevation { get; private set; }
    public virtual Color? viewSurfaceTintColor { get; private set; }
    public virtual BorderSide? viewSide { get; private set; }
    public virtual OutlinedBorder? viewShape { get; private set; }
    public virtual EdgeInsetsGeometry? viewBarPadding { get; private set; }
    public virtual double? viewHeaderHeight { get; private set; }
    public virtual TextStyle? viewHeaderTextStyle { get; private set; }
    public virtual TextStyle? viewHeaderHintStyle { get; private set; }
    public virtual Color? dividerColor { get; private set; }
    public virtual BoxConstraints? viewConstraints { get; private set; }
    public virtual EdgeInsetsGeometry? viewPadding { get; private set; }
    public virtual bool? shrinkWrap { get; private set; }
    public virtual TextCapitalization? textCapitalization { get; private set; }
    public virtual bool showFullScreenView { get; private set; } = default!;
    public virtual GlobalKey<IState> anchorKey { get; private set; } = default!;
    public virtual SearchController searchController { get; private set; } = default!;
    public virtual Func<BuildContext, SearchController, object> suggestionsBuilder
    {
        get;
        private set;
    } = default!;
    public virtual CapturedThemes capturedThemes { get; private set; } = default!;
    public virtual TextInputAction? textInputAction { get; private set; }
    public virtual TextInputType? keyboardType { get; private set; }
    public virtual SmartDashesType? smartDashesType { get; private set; }
    public virtual SmartQuotesType? smartQuotesType { get; private set; }
    public virtual CurvedAnimation? curvedAnimation { get; set; } = default;
    public virtual CurvedAnimation? viewFadeOnIntervalCurve { get; set; } = default;
    public virtual bool willDisposeSearchController { get; set; } = false;
    public virtual SearchViewThemeData viewDefaults { get; private set; } = default!;
    public virtual SearchViewThemeData viewTheme { get; private set; } = default!;
    internal virtual RectTween _rectTween { get; private set; } = new RectTween();

    internal _SearchViewRoute__search_anchor(
        Action<string>? viewOnChanged = null,
        Action<string>? viewOnSubmitted = null,
        Action? viewOnClose = null,
        Action? viewOnOpen = null,
        Func<bool>? toggleVisibility = null,
        TextDirection? textDirection = null,
        Func<IEnumerable<Widget>, Widget>? viewBuilder = null,
        Widget? viewLeading = null,
        IEnumerable<Widget>? viewTrailing = null,
        string? viewHintText = null,
        Color? viewBackgroundColor = null,
        double? viewElevation = null,
        Color? viewSurfaceTintColor = null,
        BorderSide? viewSide = null,
        OutlinedBorder? viewShape = null,
        EdgeInsetsGeometry? viewBarPadding = null,
        double? viewHeaderHeight = null,
        TextStyle? viewHeaderTextStyle = null,
        TextStyle? viewHeaderHintStyle = null,
        Color? dividerColor = null,
        BoxConstraints? viewConstraints = null,
        EdgeInsetsGeometry? viewPadding = null,
        bool? shrinkWrap = null,
        TextCapitalization? textCapitalization = null,
        bool showFullScreenView = default!,
        GlobalKey<IState> anchorKey = default!,
        SearchController searchController = default!,
        Func<BuildContext, SearchController, object> suggestionsBuilder = default!,
        CapturedThemes capturedThemes = default!,
        TextInputAction? textInputAction = null,
        TextInputType? keyboardType = null,
        SmartDashesType? smartDashesType = null,
        SmartQuotesType? smartQuotesType = null
    )
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

    public virtual Rect? getRect()
    {
        BuildContext? contextLocal = anchorKey.currentContext;
        if (contextLocal is not null)
        {
            var searchBarBox = ((RenderBox?)contextLocal.findRenderObject()!)!;
            Size boxSize = searchBarBox.size;
            NavigatorState navigator = Navigator.of(contextLocal);
            Offset boxLocation = searchBarBox.localToGlobal(
                Offset.zero,
                ancestor: navigator.context.findRenderObject()
            );
            return boxLocation & boxSize;
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Scheduler.TickerFuture didPush()
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
        WidgetsBinding.instance.addPostFrameCallback(
            (_) =>
            {
                if (anchorKey.currentContext is not null)
                {
                    FocusScope.of(anchorKey.currentContext!).unfocus();
                }
            }
        );
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

    public virtual void updateViewConfig(BuildContext context)
    {
        viewDefaults = DartRuntimePrimitives.ConvertValue<SearchViewThemeData>(
            new _SearchViewDefaultsM3__search_anchor(context, isFullScreen: showFullScreenView)
        );
        viewTheme = SearchViewTheme.of(context);
    }

    public virtual void updateTweens(BuildContext context)
    {
        var navigator = ((RenderBox?)Navigator.of(context).context.findRenderObject()!)!;
        Size screenSize = navigator.size;
        Rect anchorRect = getRect() ?? Rect.zero;
        BoxConstraints effectiveConstraints =
            (viewConstraints ?? viewTheme.constraints) ?? viewDefaults.constraints!;
        _rectTween.begin = anchorRect;
        double viewWidth = Dart_uiLibrary.clampDouble(
            anchorRect.width,
            effectiveConstraints.minWidth,
            effectiveConstraints.maxWidth
        );
        double viewHeight = Dart_uiLibrary.clampDouble(
            screenSize.height * 2L / 3L,
            effectiveConstraints.minHeight,
            effectiveConstraints.maxHeight
        );
        switch (textDirection ?? TextDirection.ltr)
        {
            case TextDirection.ltr:
            {
                double viewLeftToScreenRight = screenSize.width - anchorRect.left;
                double viewTopToScreenBottom = screenSize.height - anchorRect.top;
                Offset topLeftLocal = anchorRect.topLeft;
                if (viewLeftToScreenRight < viewWidth)
                {
                    topLeftLocal = new Offset(
                        screenSize.width - Math.Min(viewWidth, screenSize.width),
                        topLeftLocal.dy
                    );
                }
                if (viewTopToScreenBottom < viewHeight)
                {
                    topLeftLocal = new Offset(
                        topLeftLocal.dx,
                        screenSize.height - Math.Min(viewHeight, screenSize.height)
                    );
                }
                var endSize = new Size(viewWidth, viewHeight);
                _rectTween.end = showFullScreenView
                    ? (Offset.zero & screenSize)
                    : (topLeftLocal & endSize);
                return;
            }
            case TextDirection.rtl:
            {
                double viewRightToScreenLeft = anchorRect.right;
                double viewTopToScreenBottomLocal = screenSize.height - anchorRect.top;
                var topLeftAlternate = new Offset(
                    Math.Max(anchorRect.right - viewWidth, 0.0),
                    anchorRect.top
                );
                if (viewRightToScreenLeft < viewWidth)
                {
                    topLeftAlternate = new Offset(0.0, topLeftAlternate.dy);
                }
                if (viewTopToScreenBottomLocal < viewHeight)
                {
                    topLeftAlternate = new Offset(
                        topLeftAlternate.dx,
                        screenSize.height - Math.Min(viewHeight, screenSize.height)
                    );
                }
                var endSizeLocal = new Size(viewWidth, viewHeight);
                _rectTween.end = showFullScreenView
                    ? (Offset.zero & screenSize)
                    : (topLeftAlternate & endSizeLocal);
                break;
            }
        }
    }

    public override Widget buildPage(
        BuildContext context,
        Animation<double> animation,
        Animation<double> secondaryAnimation
    )
    {
        return new Directionality(
            textDirection: textDirection ?? TextDirection.ltr,
            child: new AnimatedBuilder(
                animation: animation,
                builder: (context, child) =>
                {
                    curvedAnimation ??= new CurvedAnimation(
                        parent: animation,
                        curve: Curves.easeInOutCubicEmphasized,
                        reverseCurve: Curves.easeInOutCubicEmphasized.flipped
                    );
                    Rect viewRectLocal = DartRuntimePrimitives.RequireValue(
                        _rectTween.evaluate(curvedAnimation!)
                    );
                    double topPaddingLocal = showFullScreenView
                        ? DartRuntimePrimitives.RequireValue(
                            Dart_uiLibrary.lerpDouble(
                                0.0,
                                MediaQuery.paddingOf(context).top,
                                curvedAnimation!.value
                            )
                        )
                        : 0.0;
                    viewFadeOnIntervalCurve ??= new CurvedAnimation(
                        parent: animation,
                        curve: Search_anchorLibrary._kViewFadeOnInterval,
                        reverseCurve: Search_anchorLibrary._kViewFadeOnInterval.flipped
                    );
                    return new FadeTransition(
                        opacity: viewFadeOnIntervalCurve!,
                        child: capturedThemes.wrap(
                            new _ViewContent__search_anchor(
                                viewOnChanged: viewOnChanged,
                                viewOnSubmitted: viewOnSubmitted,
                                viewLeading: viewLeading,
                                viewTrailing: viewTrailing,
                                viewHintText: viewHintText,
                                viewBackgroundColor: viewBackgroundColor,
                                viewElevation: viewElevation,
                                viewSurfaceTintColor: viewSurfaceTintColor,
                                viewSide: viewSide,
                                viewShape: viewShape,
                                viewBarPadding: viewBarPadding,
                                viewHeaderHeight: viewHeaderHeight,
                                viewHeaderTextStyle: viewHeaderTextStyle,
                                viewHeaderHintStyle: viewHeaderHintStyle,
                                dividerColor: dividerColor,
                                viewConstraints: viewConstraints,
                                viewPadding: viewPadding,
                                shrinkWrap: shrinkWrap,
                                showFullScreenView: showFullScreenView,
                                animation: curvedAnimation!,
                                topPadding: topPaddingLocal,
                                viewMaxWidth: DartRuntimePrimitives
                                    .RequireValue(_rectTween.end)
                                    .width,
                                viewRect: viewRectLocal,
                                viewBuilder: viewBuilder,
                                searchController: searchController,
                                suggestionsBuilder: suggestionsBuilder,
                                textCapitalization: textCapitalization,
                                textInputAction: textInputAction,
                                keyboardType: keyboardType,
                                smartDashesType: smartDashesType,
                                smartQuotesType: smartQuotesType
                            )
                        )
                    );
                    throw new InvalidOperationException("Dart closure completed without a value.");
                }
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Duration transitionDuration => Search_anchorLibrary._kOpenViewDuration;
}

public class _ViewContent__search_anchor : StatefulWidget
{
    public virtual Action<string>? viewOnChanged { get; private set; }
    public virtual Action<string>? viewOnSubmitted { get; private set; }
    public virtual Func<IEnumerable<Widget>, Widget>? viewBuilder { get; private set; }
    public virtual Widget? viewLeading { get; private set; }
    public virtual IEnumerable<Widget>? viewTrailing { get; private set; }
    public virtual string? viewHintText { get; private set; }
    public virtual Color? viewBackgroundColor { get; private set; }
    public virtual double? viewElevation { get; private set; }
    public virtual Color? viewSurfaceTintColor { get; private set; }
    public virtual BorderSide? viewSide { get; private set; }
    public virtual OutlinedBorder? viewShape { get; private set; }
    public virtual EdgeInsetsGeometry? viewBarPadding { get; private set; }
    public virtual double? viewHeaderHeight { get; private set; }
    public virtual TextStyle? viewHeaderTextStyle { get; private set; }
    public virtual TextStyle? viewHeaderHintStyle { get; private set; }
    public virtual Color? dividerColor { get; private set; }
    public virtual BoxConstraints? viewConstraints { get; private set; }
    public virtual EdgeInsetsGeometry? viewPadding { get; private set; }
    public virtual bool? shrinkWrap { get; private set; }
    public virtual TextCapitalization? textCapitalization { get; private set; }
    public virtual bool showFullScreenView { get; private set; } = default!;
    public virtual double topPadding { get; private set; } = default!;
    public virtual Animation<double> animation { get; private set; } = default!;
    public virtual double viewMaxWidth { get; private set; } = default!;
    public virtual Rect viewRect { get; private set; } = default!;
    public virtual SearchController searchController { get; private set; } = default!;
    public virtual Func<BuildContext, SearchController, object> suggestionsBuilder
    {
        get;
        private set;
    } = default!;
    public virtual TextInputAction? textInputAction { get; private set; }
    public virtual TextInputType? keyboardType { get; private set; }
    public virtual SmartDashesType? smartDashesType { get; private set; }
    public virtual SmartQuotesType? smartQuotesType { get; private set; }

    internal _ViewContent__search_anchor(
        Action<string>? viewOnChanged = null,
        Action<string>? viewOnSubmitted = null,
        Func<IEnumerable<Widget>, Widget>? viewBuilder = null,
        Widget? viewLeading = null,
        IEnumerable<Widget>? viewTrailing = null,
        string? viewHintText = null,
        Color? viewBackgroundColor = null,
        double? viewElevation = null,
        Color? viewSurfaceTintColor = null,
        BorderSide? viewSide = null,
        OutlinedBorder? viewShape = null,
        EdgeInsetsGeometry? viewBarPadding = null,
        double? viewHeaderHeight = null,
        TextStyle? viewHeaderTextStyle = null,
        TextStyle? viewHeaderHintStyle = null,
        Color? dividerColor = null,
        BoxConstraints? viewConstraints = null,
        EdgeInsetsGeometry? viewPadding = null,
        bool? shrinkWrap = null,
        TextCapitalization? textCapitalization = null,
        bool showFullScreenView = default!,
        double topPadding = default!,
        Animation<double> animation = default!,
        double viewMaxWidth = default!,
        Rect viewRect = default!,
        SearchController searchController = default!,
        Func<BuildContext, SearchController, object> suggestionsBuilder = default!,
        TextInputAction? textInputAction = null,
        TextInputType? keyboardType = null,
        SmartDashesType? smartDashesType = null,
        SmartQuotesType? smartQuotesType = null
    )
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

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _ViewContentState__search_anchor());
}

internal class _ViewContentState__search_anchor : State<_ViewContent__search_anchor>
{
    internal virtual Size? _screenSize { get; set; } = default;
    internal virtual Rect _viewRect { get; set; } = default!;
    public virtual CurvedAnimation viewIconsFadeCurve { get; set; } = default!;
    public virtual CurvedAnimation viewDividerFadeCurve { get; set; } = default!;
    public virtual CurvedAnimation viewListFadeOnIntervalCurve { get; set; } = default!;
    internal virtual SearchController _controller { get; private set; } = default!;
    public virtual IEnumerable<Widget> result { get; set; } = new List<Widget>();
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
        Size updatedScreenSize = MediaQuery.of(context).size;
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
            _timer = new Timer(
                Duration.zero,
                async () =>
                {
                    searchValue = _controller.text;
                    IEnumerable<Widget> suggestions = await DartAsyncRuntime.AwaitFutureOrValue<
                        IEnumerable<Widget>
                    >(widget.suggestionsBuilder(context, _controller));
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
                }
            );
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
        viewIconsFadeCurve = new CurvedAnimation(
            parent: widget.animation,
            curve: Search_anchorLibrary._kViewIconsFadeOnInterval,
            reverseCurve: Search_anchorLibrary._kViewIconsFadeOnInterval.flipped
        );
        viewDividerFadeCurve = new CurvedAnimation(
            parent: widget.animation,
            curve: Search_anchorLibrary._kViewDividerFadeOnInterval,
            reverseCurve: Search_anchorLibrary._kViewFadeOnInterval.flipped
        );
        viewListFadeOnIntervalCurve = new CurvedAnimation(
            parent: widget.animation,
            curve: Search_anchorLibrary._kViewListFadeOnInterval,
            reverseCurve: Search_anchorLibrary._kViewListFadeOnInterval.flipped
        );
    }

    internal virtual void _disposeAnimations()
    {
        viewIconsFadeCurve.dispose();
        viewDividerFadeCurve.dispose();
        viewListFadeOnIntervalCurve.dispose();
    }

    private void _updateSuggestionsListener() => _ = updateSuggestions();

    public virtual async Future updateSuggestions()
    {
        if (searchValue != _controller.text)
        {
            searchValue = _controller.text;
            IEnumerable<Widget> suggestions = await DartAsyncRuntime.AwaitFutureOrValue<
                IEnumerable<Widget>
            >(widget.suggestionsBuilder(context, _controller));
            if (mounted)
            {
                setState(() =>
                {
                    result = suggestions;
                });
            }
        }
    }

    public override Widget build(BuildContext context)
    {
        Widget defaultLeading = new BackButton(
            style: new ButtonStyle(tapTargetSize: MaterialTapTargetSize.shrinkWrap),
            onPressed: () =>
            {
                Navigator.of(context).pop<object>();
            }
        );
        var defaultTrailing = (
            (Func<List<Widget>>)(
                () =>
                {
                    var __collection36360 = new List<Widget>();
                    if (_controller.text.Length != 0)
                    {
                        __collection36360.Add(
                            DartRuntimePrimitives.ConvertValue<Widget>(
                                new IconButton(
                                    icon: new Icon(Icons.close),
                                    tooltip: MaterialLocalizations.of(context).clearButtonTooltip,
                                    onPressed: () =>
                                    {
                                        _controller.clear();
                                    }
                                )
                            )
                        );
                    }
                    return __collection36360;
                }
            )
        )();
        SearchViewThemeData viewDefaults = new _SearchViewDefaultsM3__search_anchor(
            context,
            isFullScreen: widget.showFullScreenView
        );
        SearchViewThemeData viewTheme = SearchViewTheme.of(context);
        DividerThemeData dividerTheme = DividerTheme.of(context);
        Color effectiveBackgroundColor =
            (widget.viewBackgroundColor ?? viewTheme.backgroundColor)
            ?? viewDefaults.backgroundColor!;
        Color effectiveSurfaceTint =
            (widget.viewSurfaceTintColor ?? viewTheme.surfaceTintColor)
            ?? viewDefaults.surfaceTintColor!;
        double effectiveElevation =
            (widget.viewElevation ?? viewTheme.elevation)
            ?? DartRuntimePrimitives.RequireValue(viewDefaults.elevation);
        BorderSide? effectiveSide = (widget.viewSide ?? viewTheme.side) ?? viewDefaults.side;
        OutlinedBorder effectiveShape =
            (widget.viewShape ?? viewTheme.shape) ?? viewDefaults.shape!;
        if (effectiveSide is not null)
        {
            effectiveShape = effectiveShape.copyWith(side: effectiveSide);
        }
        Color effectiveDividerColor =
            ((widget.dividerColor ?? viewTheme.dividerColor) ?? dividerTheme.color)
            ?? viewDefaults.dividerColor!;
        double? effectiveHeaderHeight = widget.viewHeaderHeight ?? viewTheme.headerHeight;
        BoxConstraints? headerConstraints =
            (effectiveHeaderHeight is null)
                ? null
                : BoxConstraints.CreateTightFor(
                    height: DartRuntimePrimitives.RequireValue(effectiveHeaderHeight)
                );
        TextStyle? effectiveTextStyle =
            (widget.viewHeaderTextStyle ?? viewTheme.headerTextStyle)
            ?? viewDefaults.headerTextStyle;
        TextStyle? effectiveHintStyle =
            (
                (
                    (widget.viewHeaderHintStyle ?? viewTheme.headerHintStyle)
                    ?? widget.viewHeaderTextStyle
                ) ?? viewTheme.headerTextStyle
            ) ?? viewDefaults.headerHintStyle;
        EdgeInsetsGeometry? effectivePadding =
            (widget.viewPadding ?? viewTheme.padding) ?? viewDefaults.padding;
        EdgeInsetsGeometry? effectiveBarPadding =
            (widget.viewBarPadding ?? viewTheme.barPadding) ?? viewDefaults.barPadding;
        BoxConstraints effectiveConstraints =
            (widget.viewConstraints ?? viewTheme.constraints) ?? viewDefaults.constraints!;
        double minWidthLocal = Math.Min(effectiveConstraints.minWidth, _viewRect.width);
        double minHeightLocal = Math.Min(effectiveConstraints.minHeight, _viewRect.height);
        bool effectiveShrinkWrap =
            (widget.shrinkWrap ?? viewTheme.shrinkWrap)
            ?? DartRuntimePrimitives.RequireValue(viewDefaults.shrinkWrap);
        Widget viewDivider = new DividerTheme(
            data: dividerTheme.copyWith(color: effectiveDividerColor),
            child: new Divider(height: 1)
        );
        return new Align(
            alignment: Alignment.topLeft,
            child: Transform.CreateTranslate(
                offset: _viewRect.topLeft,
                child: new ConstrainedBox(
                    constraints: new BoxConstraints(
                        minWidth: minWidthLocal,
                        maxWidth: _viewRect.width,
                        minHeight: minHeightLocal,
                        maxHeight: _viewRect.height
                    ),
                    child: new Padding(
                        padding: widget.showFullScreenView
                            ? EdgeInsets.zero
                            : (effectivePadding ?? EdgeInsets.zero),
                        child: new Material(
                            clipBehavior: Clip.antiAlias,
                            shape: effectiveShape,
                            color: effectiveBackgroundColor,
                            surfaceTintColor: effectiveSurfaceTint,
                            elevation: effectiveElevation,
                            child: new OverflowBox(
                                alignment: Alignment.topLeft,
                                maxWidth: Math.Min(
                                    widget.viewMaxWidth,
                                    DartRuntimePrimitives.RequireValue(_screenSize).width
                                ),
                                minWidth: 0,
                                fit: OverflowBoxFit.deferToChild,
                                child: new FadeTransition(
                                    opacity: viewIconsFadeCurve,
                                    child: new Column(
                                        mainAxisSize: MainAxisSize.min,
                                        crossAxisAlignment: CrossAxisAlignment.stretch,
                                        children: (
                                            (Func<List<Widget>>)(
                                                () =>
                                                {
                                                    var __collection40516 = new List<Widget>();
                                                    __collection40516.Add(
                                                        DartRuntimePrimitives.ConvertValue<Widget>(
                                                            new Padding(
                                                                padding: EdgeInsets.CreateOnly(
                                                                    top: widget.topPadding
                                                                ),
                                                                child: new SafeArea(
                                                                    top: false,
                                                                    bottom: false,
                                                                    child: new SearchBar(
                                                                        autoFocus: true,
                                                                        constraints: headerConstraints
                                                                            ?? (
                                                                                widget.showFullScreenView
                                                                                    ? new BoxConstraints(
                                                                                        minHeight: _SearchViewDefaultsM3__search_anchor.fullScreenBarHeight
                                                                                    )
                                                                                    : null
                                                                            ),
                                                                        padding: new WidgetStatePropertyAll<EdgeInsetsGeometry?>(
                                                                            effectiveBarPadding
                                                                        ),
                                                                        leading: widget.viewLeading
                                                                            ?? defaultLeading,
                                                                        trailing: (
                                                                            widget.viewTrailing
                                                                            ?? defaultTrailing
                                                                        ).Cast<Widget>(),
                                                                        hintText: widget.viewHintText,
                                                                        backgroundColor: new WidgetStatePropertyAll<Color>(
                                                                            Colors.transparent
                                                                        ),
                                                                        overlayColor: new WidgetStatePropertyAll<Color>(
                                                                            Colors.transparent
                                                                        ),
                                                                        elevation: new WidgetStatePropertyAll<double?>(
                                                                            0.0
                                                                        ),
                                                                        textStyle: new WidgetStatePropertyAll<TextStyle?>(
                                                                            effectiveTextStyle
                                                                        ),
                                                                        hintStyle: new WidgetStatePropertyAll<TextStyle?>(
                                                                            effectiveHintStyle
                                                                        ),
                                                                        controller: _controller,
                                                                        onChanged: (value) =>
                                                                        {
                                                                            widget.viewOnChanged?.Invoke(
                                                                                value
                                                                            );
                                                                            DartRuntimePrimitives.Ignore(
                                                                                updateSuggestions()
                                                                            );
                                                                        },
                                                                        onSubmitted: widget.viewOnSubmitted,
                                                                        textCapitalization: widget.textCapitalization,
                                                                        textInputAction: widget.textInputAction,
                                                                        keyboardType: widget.keyboardType,
                                                                        smartDashesType: widget.smartDashesType,
                                                                        smartQuotesType: widget.smartQuotesType
                                                                    )
                                                                )
                                                            )
                                                        )
                                                    );
                                                    if (
                                                        !effectiveShrinkWrap
                                                        || (minHeightLocal > 0L)
                                                        || widget.showFullScreenView
                                                        || Enumerable.Any(result)
                                                    )
                                                    {
                                                        __collection40516.AddRange(
                                                            new List<Widget>
                                                            {
                                                                DartRuntimePrimitives.ConvertValue<Widget>(
                                                                    new FadeTransition(
                                                                        opacity: viewDividerFadeCurve,
                                                                        child: viewDivider
                                                                    )
                                                                ),
                                                                DartRuntimePrimitives.ConvertValue<Widget>(
                                                                    new Flexible(
                                                                        fit: (
                                                                            effectiveShrinkWrap
                                                                            && !widget.showFullScreenView
                                                                        )
                                                                            ? FlexFit.loose
                                                                            : FlexFit.tight,
                                                                        child: new FadeTransition(
                                                                            opacity: viewListFadeOnIntervalCurve,
                                                                            child: (
                                                                                widget.viewBuilder
                                                                                is null
                                                                            )
                                                                                ? MediaQuery.CreateRemovePadding(
                                                                                    context: context,
                                                                                    removeTop: true,
                                                                                    child: new ListView(
                                                                                        padding: EdgeInsets.CreateOnly(
                                                                                            bottom: MediaQuery
                                                                                                .viewInsetsOf(
                                                                                                    context
                                                                                                )
                                                                                                .bottom
                                                                                        ),
                                                                                        shrinkWrap: effectiveShrinkWrap,
                                                                                        children: result.ToList()
                                                                                    )
                                                                                )
                                                                                : widget.viewBuilder!(
                                                                                    result
                                                                                )
                                                                        )
                                                                    )
                                                                ),
                                                            }
                                                        );
                                                    }
                                                    return __collection40516;
                                                }
                                            )
                                        )()
                                    )
                                )
                            )
                        )
                    )
                )
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _SearchAnchorWithSearchBar__search_anchor : SearchAnchor
{
    internal _SearchAnchorWithSearchBar__search_anchor(
        Widget? barLeading = null,
        IEnumerable<Widget>? barTrailing = null,
        string? barHintText = null,
        Action? onTap = null,
        WidgetStateProperty<double?>? barElevation = null,
        WidgetStateProperty<Color?>? barBackgroundColor = null,
        WidgetStateProperty<Color?>? barOverlayColor = null,
        WidgetStateProperty<BorderSide?>? barSide = null,
        WidgetStateProperty<OutlinedBorder?>? barShape = null,
        WidgetStateProperty<EdgeInsetsGeometry?>? barPadding = null,
        EdgeInsetsGeometry? viewBarPadding = null,
        WidgetStateProperty<TextStyle?>? barTextStyle = null,
        WidgetStateProperty<TextStyle?>? barHintStyle = null,
        Func<IEnumerable<Widget>, Widget>? viewBuilder = null,
        Widget? viewLeading = null,
        IEnumerable<Widget>? viewTrailing = null,
        string? viewHintText = null,
        Color? viewBackgroundColor = null,
        double? viewElevation = null,
        BorderSide? viewSide = null,
        OutlinedBorder? viewShape = null,
        double? viewHeaderHeight = null,
        TextStyle? viewHeaderTextStyle = null,
        TextStyle? viewHeaderHintStyle = null,
        Color? dividerColor = null,
        BoxConstraints? constraints = null,
        BoxConstraints? viewConstraints = null,
        EdgeInsetsGeometry? viewPadding = null,
        bool? shrinkWrap = null,
        bool? isFullScreen = null,
        SearchController? searchController = null,
        TextCapitalization? textCapitalization = null,
        Action<string>? onChanged = null,
        Action<string>? onSubmitted = null,
        Action? onClose = null,
        Action? onOpen = null,
        Func<BuildContext, SearchController, object> suggestionsBuilder = default!,
        TextInputAction? textInputAction = null,
        TextInputType? keyboardType = null,
        EdgeInsets scrollPadding = default!,
        Func<BuildContext, EditableTextState, Widget> contextMenuBuilder = default!,
        bool enabled = true,
        SmartDashesType? smartDashesType = null,
        SmartQuotesType? smartQuotesType = null
    )
        : base(
            viewBarPadding: viewBarPadding,
            viewBuilder: viewBuilder,
            viewLeading: viewLeading,
            viewTrailing: viewTrailing,
            viewBackgroundColor: viewBackgroundColor,
            viewElevation: viewElevation,
            viewSide: viewSide,
            viewShape: viewShape,
            dividerColor: dividerColor,
            viewConstraints: viewConstraints,
            viewPadding: viewPadding,
            shrinkWrap: shrinkWrap,
            isFullScreen: isFullScreen,
            searchController: searchController,
            textCapitalization: textCapitalization,
            suggestionsBuilder: suggestionsBuilder,
            textInputAction: textInputAction,
            keyboardType: keyboardType,
            enabled: enabled,
            smartDashesType: smartDashesType,
            smartQuotesType: smartQuotesType,
            viewHintText: viewHintText ?? barHintText,
            headerHeight: viewHeaderHeight,
            headerTextStyle: viewHeaderTextStyle,
            headerHintStyle: viewHeaderHintStyle,
            viewOnSubmitted: onSubmitted,
            viewOnChanged: onChanged,
            viewOnClose: onClose,
            viewOnOpen: onOpen,
            builder: (context, controller) =>
            {
                return new SearchBar(
                    constraints: constraints,
                    controller: controller,
                    onTap: () =>
                    {
                        controller.openView();
                        onTap?.Invoke();
                    },
                    onChanged: (value) =>
                    {
                        controller.openView();
                    },
                    onSubmitted: onSubmitted,
                    hintText: barHintText,
                    hintStyle: barHintStyle,
                    textStyle: barTextStyle,
                    elevation: barElevation,
                    backgroundColor: barBackgroundColor,
                    overlayColor: barOverlayColor,
                    side: barSide,
                    shape: barShape,
                    padding: DartRuntimePrimitives.ConvertValue<
                        WidgetStateProperty<EdgeInsetsGeometry?>
                    >(
                        (object?)barPadding
                            ?? new WidgetStatePropertyAll<EdgeInsetsGeometry?>(
                                EdgeInsets.CreateSymmetric(horizontal: 16.0)
                            )
                    ),
                    leading: barLeading ?? new Icon(Icons.search),
                    trailing: barTrailing,
                    textCapitalization: textCapitalization,
                    textInputAction: textInputAction,
                    keyboardType: keyboardType,
                    scrollPadding: scrollPadding ?? EdgeInsets.CreateAll(20.0),
                    contextMenuBuilder: contextMenuBuilder ?? SearchBar._defaultContextMenuBuilder,
                    smartDashesType: smartDashesType,
                    smartQuotesType: smartQuotesType
                );
                throw new InvalidOperationException("Dart closure completed without a value.");
            }
        ) { }
}

public class SearchController : TextEditingController
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

public class SearchBar : StatefulWidget
{
    public virtual TextEditingController? controller { get; private set; }
    public virtual FocusNode? focusNode { get; private set; }
    public virtual string? hintText { get; private set; }
    public virtual Widget? leading { get; private set; }
    public virtual IEnumerable<Widget>? trailing { get; private set; }
    public virtual Action? onTap { get; private set; }
    public virtual Action<Gestures.PointerDownEvent>? onTapOutside { get; private set; }
    public virtual Action<string>? onChanged { get; private set; }
    public virtual Action<string>? onSubmitted { get; private set; }
    public virtual BoxConstraints? constraints { get; private set; }
    public virtual WidgetStateProperty<double?>? elevation { get; private set; }
    public virtual WidgetStateProperty<Color?>? backgroundColor { get; private set; }
    public virtual WidgetStateProperty<Color?>? shadowColor { get; private set; }
    public virtual WidgetStateProperty<Color?>? surfaceTintColor { get; private set; }
    public virtual WidgetStateProperty<Color?>? overlayColor { get; private set; }
    public virtual WidgetStateProperty<BorderSide?>? side { get; private set; }
    public virtual WidgetStateProperty<OutlinedBorder?>? shape { get; private set; }
    public virtual WidgetStateProperty<EdgeInsetsGeometry?>? padding { get; private set; }
    public virtual WidgetStateProperty<TextStyle?>? textStyle { get; private set; }
    public virtual WidgetStateProperty<TextStyle?>? hintStyle { get; private set; }
    public virtual TextCapitalization? textCapitalization { get; private set; }
    public virtual bool enabled { get; private set; } = default!;
    public virtual bool autoFocus { get; private set; } = default!;
    public virtual TextInputAction? textInputAction { get; private set; }
    public virtual TextInputType? keyboardType { get; private set; }
    public virtual EdgeInsets scrollPadding { get; private set; } = default!;
    public virtual Func<BuildContext, EditableTextState, Widget>? contextMenuBuilder
    {
        get;
        private set;
    }
    public virtual bool readOnly { get; private set; } = default!;
    public virtual SmartDashesType? smartDashesType { get; private set; }
    public virtual SmartQuotesType? smartQuotesType { get; private set; }

    public SearchBar(
        Key? key = null,
        TextEditingController? controller = null,
        FocusNode? focusNode = null,
        string? hintText = null,
        Widget? leading = null,
        IEnumerable<Widget>? trailing = null,
        Action? onTap = null,
        Action<Gestures.PointerDownEvent>? onTapOutside = null,
        Action<string>? onChanged = null,
        Action<string>? onSubmitted = null,
        BoxConstraints? constraints = null,
        WidgetStateProperty<double?>? elevation = null,
        WidgetStateProperty<Color?>? backgroundColor = null,
        WidgetStateProperty<Color?>? shadowColor = null,
        WidgetStateProperty<Color?>? surfaceTintColor = null,
        WidgetStateProperty<Color?>? overlayColor = null,
        WidgetStateProperty<BorderSide?>? side = null,
        WidgetStateProperty<OutlinedBorder?>? shape = null,
        WidgetStateProperty<EdgeInsetsGeometry?>? padding = null,
        WidgetStateProperty<TextStyle?>? textStyle = null,
        WidgetStateProperty<TextStyle?>? hintStyle = null,
        TextCapitalization? textCapitalization = null,
        bool enabled = true,
        bool autoFocus = false,
        TextInputAction? textInputAction = null,
        TextInputType? keyboardType = null,
        EdgeInsets scrollPadding = default!,
        Func<BuildContext, EditableTextState, Widget>? contextMenuBuilder = default!,
        bool readOnly = false,
        SmartDashesType? smartDashesType = null,
        SmartQuotesType? smartQuotesType = null
    )
        : base(key: key)
    {
        EdgeInsets __scrollPadding = scrollPadding ?? EdgeInsets.CreateAll(20.0);
        Func<BuildContext, EditableTextState, Widget>? __contextMenuBuilder =
            contextMenuBuilder ?? _defaultContextMenuBuilder;
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

    internal static Widget _defaultContextMenuBuilder(
        BuildContext context,
        EditableTextState editableTextState
    )
    {
        if (SystemContextMenu.isSupportedByField(editableTextState))
        {
            return SystemContextMenu.CreateEditableText(editableTextState: editableTextState);
        }
        return AdaptiveTextSelectionToolbar.CreateEditableText(
            editableTextState: editableTextState
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _SearchBarState__search_anchor());
}

internal class _SearchBarState__search_anchor : State<SearchBar>
{
    internal virtual WidgetStatesController _internalStatesController { get; private set; } =
        default!;
    internal virtual FocusNode? _internalFocusNode { get; set; } = default;

    internal virtual FocusNode _focusNode =>
        DartRuntimePrimitives.ConvertValue<FocusNode>(
            widget.focusNode ?? (_internalFocusNode ??= new FocusNode())
        );

    public override void initState()
    {
        base.initState();
        _internalStatesController = new WidgetStatesController();
        _internalStatesController.addListener(() =>
        {
            setState(() => { });
        });
    }

    public override void dispose()
    {
        _internalStatesController.dispose();
        _internalFocusNode?.dispose();
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        TextDirection textDirectionLocal = Directionality.of(context);
        ColorScheme colorSchemeLocal = Theme.of(context).colorScheme;
        SearchBarThemeData searchBarTheme = SearchBarTheme.of(context);
        SearchBarThemeData defaults = new _SearchBarDefaultsM3__search_anchor(context);
        P? resolve<P>(
            WidgetStateProperty<P>? widgetValue,
            WidgetStateProperty<P>? themeValue,
            WidgetStateProperty<P>? defaultValue
        )
        {
            HashSet<WidgetState> states = _internalStatesController.value;
            return widgetValue is not null ? widgetValue.resolve(states)
                : themeValue is not null ? themeValue.resolve(states)
                : defaultValue is not null ? defaultValue.resolve(states)
                : default;
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        TextStyle? effectiveTextStyle = resolve(
            widget.textStyle,
            searchBarTheme.textStyle,
            defaults.textStyle
        );
        double? effectiveElevation = resolve(
            widget.elevation,
            searchBarTheme.elevation,
            defaults.elevation
        );
        Color? effectiveShadowColor = resolve(
            widget.shadowColor,
            searchBarTheme.shadowColor,
            defaults.shadowColor
        );
        Color? effectiveBackgroundColor = resolve(
            widget.backgroundColor,
            searchBarTheme.backgroundColor,
            defaults.backgroundColor
        );
        Color? effectiveSurfaceTintColor = resolve(
            widget.surfaceTintColor,
            searchBarTheme.surfaceTintColor,
            defaults.surfaceTintColor
        );
        OutlinedBorder? effectiveShape = resolve(
            widget.shape,
            searchBarTheme.shape,
            defaults.shape
        );
        BorderSide? effectiveSide = resolve(widget.side, searchBarTheme.side, defaults.side);
        EdgeInsetsGeometry? effectivePadding = resolve(
            widget.padding,
            searchBarTheme.padding,
            defaults.padding
        );
        WidgetStateProperty<Color?>? effectiveOverlayColor =
            (widget.overlayColor ?? searchBarTheme.overlayColor) ?? defaults.overlayColor;
        TextCapitalization effectiveTextCapitalization =
            (widget.textCapitalization ?? searchBarTheme.textCapitalization)
            ?? DartRuntimePrimitives.RequireValue(defaults.textCapitalization);
        HashSet<WidgetState> statesLocal = _internalStatesController.value;
        TextStyle? effectiveHintStyle =
            (
                (
                    (
                        widget.hintStyle?.resolve(statesLocal)
                        ?? (searchBarTheme.hintStyle?.resolve(statesLocal))
                    ) ?? (widget.textStyle?.resolve(statesLocal))
                ) ?? (searchBarTheme.textStyle?.resolve(statesLocal))
            ) ?? (defaults.hintStyle?.resolve(statesLocal));
        Color defaultColor = colorSchemeLocal.brightness switch
        {
            Brightness.light => ConstantsLibrary.kDefaultIconDarkColor,
            Brightness.dark => ConstantsLibrary.kDefaultIconLightColor,
            _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                throw new InvalidOperationException("Non-exhaustive Dart switch value."),
        };
        IconThemeData? customTheme = IconTheme.of(context) switch
        {
            IconThemeData iconTheme when !Equals(iconTheme.color, defaultColor) => iconTheme,
            _ => DartRuntimePrimitives.ConvertValue<IconThemeData>(null),
        };
        Widget? leadingLocal = default!;
        if (widget.leading is not null)
        {
            leadingLocal = IconTheme.merge(
                data: customTheme ?? new IconThemeData(color: colorSchemeLocal.onSurface),
                child: widget.leading!
            );
        }
        List<Widget>? trailingLocal = widget
            .trailing?.map(
                (trailing) =>
                    IconTheme.merge(
                        data: customTheme
                            ?? new IconThemeData(color: colorSchemeLocal.onSurfaceVariant),
                        child: trailing
                    )
            )
            .ToList()
            .ToList();
        return new ConstrainedBox(
            constraints: (widget.constraints ?? searchBarTheme.constraints)
                ?? defaults.constraints!,
            child: new Opacity(
                opacity: widget.enabled ? 1 : Search_anchorLibrary._kDisableSearchBarOpacity,
                child: new Material(
                    elevation: DartRuntimePrimitives.RequireValue(effectiveElevation),
                    shadowColor: effectiveShadowColor,
                    color: effectiveBackgroundColor,
                    surfaceTintColor: effectiveSurfaceTintColor,
                    shape: effectiveShape?.copyWith(side: effectiveSide),
                    child: new IgnorePointer(
                        ignoring: !widget.enabled,
                        child: new InkWell(
                            onTap: () =>
                            {
                                widget.onTap?.Invoke();
                                if (!_focusNode.hasFocus)
                                {
                                    _focusNode.requestFocus();
                                }
                            },
                            overlayColor: effectiveOverlayColor,
                            customBorder: effectiveShape?.copyWith(side: effectiveSide),
                            statesController: _internalStatesController,
                            child: new Padding(
                                padding: effectivePadding!,
                                child: new Row(
                                    textDirection: textDirectionLocal,
                                    children: (
                                        (Func<List<Widget>>)(
                                            () =>
                                            {
                                                var __collection64758 = new List<Widget>();
                                                var __collectionElement64788 = leadingLocal;
                                                if (
                                                    __collectionElement64788 is
                                                    { } __nonNullCollectionElement64788
                                                )
                                                {
                                                    __collection64758.Add(
                                                        DartRuntimePrimitives.ConvertValue<Widget>(
                                                            __nonNullCollectionElement64788
                                                        )
                                                    );
                                                }
                                                __collection64758.Add(
                                                    DartRuntimePrimitives.ConvertValue<Widget>(
                                                        new Expanded(
                                                            child: new Padding(
                                                                padding: DartRuntimePrimitives.RequireReference(
                                                                    effectivePadding
                                                                ),
                                                                child: new Widgets.Semantics(
                                                                    inputType: SemanticsInputType.search,
                                                                    child: new TextField(
                                                                        readOnly: widget.readOnly,
                                                                        autofocus: widget.autoFocus,
                                                                        onTap: widget.onTap,
                                                                        onTapAlwaysCalled: true,
                                                                        onTapOutside: widget.onTapOutside,
                                                                        focusNode: _focusNode,
                                                                        onChanged: widget.onChanged,
                                                                        onSubmitted: widget.onSubmitted,
                                                                        controller: widget.controller,
                                                                        style: effectiveTextStyle,
                                                                        enabled: widget.enabled,
                                                                        decoration: new InputDecoration(
                                                                            hintText: widget.hintText
                                                                        ).applyDefaults(
                                                                            new InputDecorationThemeData(
                                                                                hintStyle: effectiveHintStyle,
                                                                                enabledBorder: InputBorder.none,
                                                                                border: InputBorder.none,
                                                                                focusedBorder: InputBorder.none,
                                                                                contentPadding: EdgeInsets.zero,
                                                                                isDense: true
                                                                            )
                                                                        ),
                                                                        textCapitalization: effectiveTextCapitalization,
                                                                        textInputAction: widget.textInputAction,
                                                                        keyboardType: widget.keyboardType,
                                                                        scrollPadding: widget.scrollPadding,
                                                                        contextMenuBuilder: widget.contextMenuBuilder,
                                                                        smartDashesType: widget.smartDashesType,
                                                                        smartQuotesType: widget.smartQuotesType
                                                                    )
                                                                )
                                                            )
                                                        )
                                                    )
                                                );
                                                var __collectionSpread67233 = trailingLocal;
                                                if (__collectionSpread67233 is not null)
                                                {
                                                    __collection64758.AddRange(
                                                        __collectionSpread67233
                                                    );
                                                }
                                                return __collection64758;
                                            }
                                        )
                                    )()
                                )
                            )
                        )
                    )
                )
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _SearchBarDefaultsM3__search_anchor : SearchBarThemeData
{
    public virtual BuildContext context { get; private set; } = default!;
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

    internal _SearchBarDefaultsM3__search_anchor(BuildContext context)
    {
        this.context = context;
    }

    public override WidgetStateProperty<Color?>? backgroundColor =>
        DartRuntimePrimitives.ConvertValue<WidgetStateProperty<Color?>>(
            new WidgetStatePropertyAll<Color>(_colors.surfaceContainerHigh)
        );
    public override WidgetStateProperty<double?>? elevation =>
        DartRuntimePrimitives.ConvertValue<WidgetStateProperty<double?>>(
            new WidgetStatePropertyAll<double?>(6.0)
        );
    public override WidgetStateProperty<Color>? shadowColor =>
        DartRuntimePrimitives.ConvertValue<WidgetStateProperty<Color>>(
            new WidgetStatePropertyAll<Color>(_colors.shadow)
        );
    public override WidgetStateProperty<Color>? surfaceTintColor =>
        DartRuntimePrimitives.ConvertValue<WidgetStateProperty<Color>>(
            new WidgetStatePropertyAll<Color>(Colors.transparent)
        );
    public override WidgetStateProperty<Color?>? overlayColor =>
        DartRuntimePrimitives.ConvertValue<WidgetStateProperty<Color?>>(
            WidgetStateProperty.resolveWith(
                (states) =>
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
                }
            )
        );
    public override WidgetStateProperty<OutlinedBorder>? shape =>
        DartRuntimePrimitives.ConvertValue<WidgetStateProperty<OutlinedBorder>>(
            new WidgetStatePropertyAll<OutlinedBorder>(new StadiumBorder())
        );
    public override WidgetStateProperty<EdgeInsetsGeometry>? padding =>
        DartRuntimePrimitives.ConvertValue<WidgetStateProperty<EdgeInsetsGeometry>>(
            new WidgetStatePropertyAll<EdgeInsetsGeometry>(
                EdgeInsets.CreateSymmetric(horizontal: 8.0)
            )
        );
    public override WidgetStateProperty<TextStyle?> textStyle =>
        DartRuntimePrimitives.ConvertValue<WidgetStateProperty<TextStyle?>>(
            new WidgetStatePropertyAll<TextStyle?>(
                _textTheme.bodyLarge?.copyWith(color: _colors.onSurface)
            )
        );
    public override WidgetStateProperty<TextStyle?> hintStyle =>
        DartRuntimePrimitives.ConvertValue<WidgetStateProperty<TextStyle?>>(
            new WidgetStatePropertyAll<TextStyle?>(
                _textTheme.bodyLarge?.copyWith(color: _colors.onSurfaceVariant)
            )
        );
    public override BoxConstraints constraints =>
        new BoxConstraints(minWidth: 360.0, maxWidth: 800.0, minHeight: 56.0);
    public override TextCapitalization? textCapitalization => TextCapitalization.none;
}

internal class _SearchViewDefaultsM3__search_anchor : SearchViewThemeData
{
    public virtual BuildContext context { get; private set; } = default!;
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

    internal _SearchViewDefaultsM3__search_anchor(BuildContext context, bool isFullScreen)
    {
        this.context = context;
        this.isFullScreen = isFullScreen;
    }

    public override Color? backgroundColor =>
        DartRuntimePrimitives.ConvertValue<Color>(_colors.surfaceContainerHigh);
    public override double? elevation => 6.0;
    public override Color? surfaceTintColor =>
        DartRuntimePrimitives.ConvertValue<Color>(Colors.transparent);
    public override OutlinedBorder? shape =>
        DartRuntimePrimitives.ConvertValue<OutlinedBorder>(
            isFullScreen
                ? new RoundedRectangleBorder()
                : new RoundedRectangleBorder(
                    borderRadius: BorderRadius.CreateAll(Radius.circular(28.0))
                )
        );
    public override TextStyle? headerTextStyle =>
        _textTheme.bodyLarge?.copyWith(color: _colors.onSurface);
    public override TextStyle? headerHintStyle =>
        _textTheme.bodyLarge?.copyWith(color: _colors.onSurfaceVariant);
    public override BoxConstraints constraints =>
        new BoxConstraints(minWidth: 360.0, minHeight: 240.0);
    public override EdgeInsetsGeometry? barPadding =>
        DartRuntimePrimitives.ConvertValue<EdgeInsetsGeometry>(
            EdgeInsets.CreateSymmetric(horizontal: 8.0)
        );
    public override bool? shrinkWrap => false;
    public override Color? dividerColor =>
        DartRuntimePrimitives.ConvertValue<Color>(_colors.outline);
}
