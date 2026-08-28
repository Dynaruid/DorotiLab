// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/search_anchor.dart
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

public static partial class Search_anchorLibrary
{
    internal static long _kOpenViewMilliseconds = 600L;
}

public static partial class Search_anchorLibrary
{
    internal static Duration _kOpenViewDuration = Duration.Create(milliseconds: Search_anchorLibrary._kOpenViewMilliseconds);
}

public static partial class Search_anchorLibrary
{
    internal static Duration _kAnchorFadeDuration = Duration.Create(milliseconds: 150L);
}

public static partial class Search_anchorLibrary
{
    internal static global::Doroti.Framework.Animation.Curve _kViewFadeOnInterval = ((global::Doroti.Framework.Animation.Curve)(object?)new global::Doroti.Framework.Animation.Interval(0.0, (1L / 2L)));
}

public static partial class Search_anchorLibrary
{
    internal static global::Doroti.Framework.Animation.Curve _kViewIconsFadeOnInterval = ((global::Doroti.Framework.Animation.Curve)(object?)new global::Doroti.Framework.Animation.Interval((1L / 6L), (2L / 6L)));
}

public static partial class Search_anchorLibrary
{
    internal static global::Doroti.Framework.Animation.Curve _kViewDividerFadeOnInterval = ((global::Doroti.Framework.Animation.Curve)(object?)new global::Doroti.Framework.Animation.Interval(0.0, (1L / 6L)));
}

public static partial class Search_anchorLibrary
{
    internal static global::Doroti.Framework.Animation.Curve _kViewListFadeOnInterval = ((global::Doroti.Framework.Animation.Curve)(object?)new global::Doroti.Framework.Animation.Interval((133L / Search_anchorLibrary._kOpenViewMilliseconds), (233L / Search_anchorLibrary._kOpenViewMilliseconds)));
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

    public static SearchAnchor CreateBar(global::Doroti.Framework.Widgets.Widget? barLeading = null, IEnumerable<global::Doroti.Framework.Widgets.Widget>? barTrailing = null, string? barHintText = null, global::System.Action? onTap = null, global::System.Action<string>? onSubmitted = null, global::System.Action<string>? onChanged = null, global::System.Action? onClose = null, global::System.Action? onOpen = null, global::Doroti.Framework.Widgets.WidgetStateProperty<double?>? barElevation = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? barBackgroundColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? barOverlayColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.BorderSide?>? barSide = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.OutlinedBorder?>? barShape = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry?>? barPadding = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? viewBarPadding = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.TextStyle?>? barTextStyle = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.TextStyle?>? barHintStyle = null, global::System.Func<IEnumerable<global::Doroti.Framework.Widgets.Widget>, global::Doroti.Framework.Widgets.Widget>? viewBuilder = null, global::Doroti.Framework.Widgets.Widget? viewLeading = null, IEnumerable<global::Doroti.Framework.Widgets.Widget>? viewTrailing = null, string? viewHintText = null, Color? viewBackgroundColor = null, double? viewElevation = null, global::Doroti.Framework.Painting.BorderSide? viewSide = null, global::Doroti.Framework.Painting.OutlinedBorder? viewShape = null, double? viewHeaderHeight = null, global::Doroti.Framework.Painting.TextStyle? viewHeaderTextStyle = null, global::Doroti.Framework.Painting.TextStyle? viewHeaderHintStyle = null, Color? dividerColor = null, global::Doroti.Framework.Rendering.BoxConstraints? constraints = null, global::Doroti.Framework.Rendering.BoxConstraints? viewConstraints = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? viewPadding = null, bool? shrinkWrap = null, bool? isFullScreen = null, SearchController searchController = default!, global::Doroti.Framework.Services.TextCapitalization textCapitalization = default!, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, SearchController, object> suggestionsBuilder = default!, global::Doroti.Framework.Services.TextInputAction? textInputAction = null, global::Doroti.Framework.Services.TextInputType? keyboardType = null, global::Doroti.Framework.Painting.EdgeInsets scrollPadding = default!, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.EditableTextState, global::Doroti.Framework.Widgets.Widget> contextMenuBuilder = default!, bool enabled = default!, global::Doroti.Framework.Services.SmartDashesType? smartDashesType = null, global::Doroti.Framework.Services.SmartQuotesType? smartQuotesType = null)
        => new _SearchAnchorWithSearchBar__search_anchor(barLeading: barLeading, barTrailing: barTrailing, barHintText: barHintText, onTap: onTap, onSubmitted: onSubmitted, onChanged: onChanged, onClose: onClose, onOpen: onOpen, barElevation: barElevation, barBackgroundColor: barBackgroundColor, barOverlayColor: barOverlayColor, barSide: barSide, barShape: barShape, barPadding: barPadding, viewBarPadding: viewBarPadding, barTextStyle: barTextStyle, barHintStyle: barHintStyle, viewBuilder: viewBuilder, viewLeading: viewLeading, viewTrailing: viewTrailing, viewHintText: viewHintText, viewBackgroundColor: viewBackgroundColor, viewElevation: viewElevation, viewSide: viewSide, viewShape: viewShape, viewHeaderHeight: viewHeaderHeight, viewHeaderTextStyle: viewHeaderTextStyle, viewHeaderHintStyle: viewHeaderHintStyle, dividerColor: dividerColor, constraints: constraints, viewConstraints: viewConstraints, viewPadding: viewPadding, shrinkWrap: shrinkWrap, isFullScreen: isFullScreen, searchController: searchController, textCapitalization: textCapitalization, suggestionsBuilder: suggestionsBuilder, textInputAction: textInputAction, keyboardType: keyboardType, scrollPadding: scrollPadding, contextMenuBuilder: contextMenuBuilder, enabled: enabled, smartDashesType: smartDashesType, smartQuotesType: smartQuotesType);

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _SearchAnchorState__search_anchor());
}

internal class _SearchAnchorState__search_anchor : global::Doroti.Framework.Widgets.State<SearchAnchor>
{
    internal virtual Size? _screenSize { get; set; } = default;
    internal virtual bool _anchorIsVisible { get; set; } = true;
    internal virtual global::Doroti.Framework.Widgets.GlobalKey<IState> _anchorKey { get; private set; } = global::Doroti.Framework.Widgets.GlobalKey<IState>.Create();
    internal virtual SearchController? _internalSearchController { get; set; } = default;
    internal virtual _SearchViewRoute__search_anchor? _route { get; set; } = default;

    internal virtual bool _viewIsOpen => !this._anchorIsVisible;
    internal virtual SearchController _searchController => DartRuntimePrimitives.ConvertValue<SearchController>((((SearchAnchor)this.widget).searchController ?? (_internalSearchController ??= new SearchController())));
    public override void initState()
    {
        base.initState();
        this._searchController._attach(this);
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        global::Doroti.Ui.Size updatedScreenSize = ((global::Doroti.Ui.Size)(object?)MediaQuery.of(this.context).size);
        if (((this._screenSize is not null) && (!object.Equals(this._screenSize, updatedScreenSize))))
        {
            if ((((SearchController)this._searchController).isOpen && !getShowFullScreenView()))
            {
                _closeView(null);
            }
        }
        _screenSize = updatedScreenSize;
    }

    public override void didUpdateWidget(SearchAnchor oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((!object.Equals(((SearchAnchor)oldWidget).searchController, ((SearchAnchor)this.widget).searchController)))
        {
            ((SearchAnchor)oldWidget).searchController?._detach(this);
            this._searchController._attach(this);
        }
    }

    public override void dispose()
    {
        ((SearchAnchor)this.widget).searchController?._detach(this);
        this._internalSearchController?._detach(this);
        var usingExternalController = (((SearchAnchor)this.widget).searchController is not null);
        if ((this._route?.navigator is not null))
        {
            this._route?._dismiss(disposeController: !usingExternalController);
            if (usingExternalController)
            {
                this._internalSearchController?.dispose();
            }
        }
        else
        {
            this._internalSearchController?.dispose();
        }
        base.dispose();
    }

    internal virtual void _openView()
    {
        global::Doroti.Framework.Widgets.NavigatorState navigator = ((global::Doroti.Framework.Widgets.NavigatorState)(object?)Navigator.of(this.context));
        _route = new _SearchViewRoute__search_anchor(viewOnChanged: (global::System.Action<string>?)((SearchAnchor)this.widget).viewOnChanged, viewOnSubmitted: (global::System.Action<string>?)((SearchAnchor)this.widget).viewOnSubmitted, viewOnClose: () => ((SearchAnchor)this.widget).viewOnClose(), viewOnOpen: () => ((SearchAnchor)this.widget).viewOnOpen(), viewLeading: ((SearchAnchor)this.widget).viewLeading, viewTrailing: ((SearchAnchor)this.widget).viewTrailing.Cast<global::Doroti.Framework.Widgets.Widget>(), viewHintText: ((SearchAnchor)this.widget).viewHintText, viewBackgroundColor: ((SearchAnchor)this.widget).viewBackgroundColor, viewElevation: ((SearchAnchor)this.widget).viewElevation, viewSurfaceTintColor: ((SearchAnchor)this.widget).viewSurfaceTintColor, viewSide: ((SearchAnchor)this.widget).viewSide, viewShape: ((SearchAnchor)this.widget).viewShape, viewBarPadding: ((SearchAnchor)this.widget).viewBarPadding, viewHeaderHeight: ((SearchAnchor)this.widget).headerHeight, viewHeaderTextStyle: ((SearchAnchor)this.widget).headerTextStyle, viewHeaderHintStyle: ((SearchAnchor)this.widget).headerHintStyle, dividerColor: ((SearchAnchor)this.widget).dividerColor, viewConstraints: ((SearchAnchor)this.widget).viewConstraints, viewPadding: ((SearchAnchor)this.widget).viewPadding, shrinkWrap: ((SearchAnchor)this.widget).shrinkWrap, showFullScreenView: getShowFullScreenView(), toggleVisibility: (global::System.Func<bool>)this.toggleVisibility, textDirection: Directionality.of(this.context), viewBuilder: (global::System.Func<IEnumerable<global::Doroti.Framework.Widgets.Widget>, global::Doroti.Framework.Widgets.Widget>?)((SearchAnchor)this.widget).viewBuilder, anchorKey: this._anchorKey, searchController: this._searchController, suggestionsBuilder: (global::System.Func<global::Doroti.Framework.Widgets.BuildContext, SearchController, object>)((SearchAnchor)this.widget).suggestionsBuilder, textCapitalization: ((SearchAnchor)this.widget).textCapitalization, capturedThemes: InheritedTheme.capture(from: this.context, to: navigator.context), textInputAction: ((SearchAnchor)this.widget).textInputAction, keyboardType: ((SearchAnchor)this.widget).keyboardType, smartDashesType: ((SearchAnchor)this.widget).smartDashesType, smartQuotesType: ((SearchAnchor)this.widget).smartQuotesType);
        DartRuntimePrimitives.Ignore(navigator.push(this._route!));
    }

    internal virtual void _closeView(string? selectedText)
    {
        if ((selectedText is not null))
        {
            this._searchController.value = new global::Doroti.Framework.Services.TextEditingValue(text: selectedText);
        }
        Navigator.of(this.context).pop<object>();
    }

    public virtual bool toggleVisibility()
    {
        setState(((global::System.Action)(() =>
        {
            _anchorIsVisible = !this._anchorIsVisible;
        })));
        return this._anchorIsVisible;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool getShowFullScreenView()
    {
        return (((SearchAnchor)this.widget).isFullScreen ?? (Theme.of(this.context).platform switch { global::Doroti.Framework.Foundation.TargetPlatform.iOS or global::Doroti.Framework.Foundation.TargetPlatform.android => true, global::Doroti.Framework.Foundation.TargetPlatform.fuchsia => true, global::Doroti.Framework.Foundation.TargetPlatform.macOS or global::Doroti.Framework.Foundation.TargetPlatform.linux => false, global::Doroti.Framework.Foundation.TargetPlatform.windows => false, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _getOpacity()
    {
        if (((SearchAnchor)this.widget).enabled)
        {
            return (this._anchorIsVisible ? 1.0 : 0.0);
        }
        return Search_anchorLibrary._kDisableSearchBarOpacity;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.AnimatedOpacity(key: this._anchorKey, opacity: _getOpacity(), duration: Search_anchorLibrary._kAnchorFadeDuration, child: new global::Doroti.Framework.Widgets.IgnorePointer(ignoring: !((SearchAnchor)this.widget).enabled, child: new global::Doroti.Framework.Widgets.GestureDetector(onTap: () => this._openView(), child: this.widget.builder(context, this._searchController)))));
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
        global::Doroti.Framework.Widgets.BuildContext? contextLocal = ((global::Doroti.Framework.Widgets.GlobalKey<IState>)this.anchorKey).currentContext;
        if ((contextLocal is not null))
        {
            var searchBarBox = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)contextLocal.findRenderObject()!)!;
            global::Doroti.Ui.Size boxSize = ((global::Doroti.Ui.Size)(object?)((global::Doroti.Framework.Rendering.RenderBox)searchBarBox).size);
            global::Doroti.Framework.Widgets.NavigatorState navigator = ((global::Doroti.Framework.Widgets.NavigatorState)(object?)Navigator.of(contextLocal));
            global::Doroti.Ui.Offset boxLocation = ((global::Doroti.Ui.Offset)(object?)((Offset)((dynamic)searchBarBox).localToGlobal(Offset.zero, ancestor: navigator.context.findRenderObject())));
            return (boxLocation & boxSize);
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Scheduler.TickerFuture didPush()
    {
        DartRuntimePrimitives.Assert(() => (((global::Doroti.Framework.Widgets.GlobalKey<IState>)this.anchorKey).currentContext is not null));
        updateViewConfig(((global::Doroti.Framework.Widgets.GlobalKey<IState>)this.anchorKey).currentContext!);
        updateTweens(((global::Doroti.Framework.Widgets.GlobalKey<IState>)this.anchorKey).currentContext!);
        this.toggleVisibility?.Invoke();
        this.viewOnOpen?.Invoke();
        return ((global::Doroti.Framework.Scheduler.TickerFuture)(object?)base.didPush());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool didPop(_SearchViewRoute__search_anchor? result)
    {
        DartRuntimePrimitives.Assert(() => (((global::Doroti.Framework.Widgets.GlobalKey<IState>)this.anchorKey).currentContext is not null));
        updateTweens(((global::Doroti.Framework.Widgets.GlobalKey<IState>)this.anchorKey).currentContext!);
        this.toggleVisibility?.Invoke();
        this.viewOnClose?.Invoke();
        global::Doroti.Framework.Widgets.WidgetsBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((_) =>
        {
            if ((((global::Doroti.Framework.Widgets.GlobalKey<IState>)this.anchorKey).currentContext is not null))
            {
                FocusScope.of(((global::Doroti.Framework.Widgets.GlobalKey<IState>)this.anchorKey).currentContext!).unfocus();
            }
        })));
        return base.didPop(result);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _dismiss(bool disposeController)
    {
        willDisposeSearchController = disposeController;
        if (this.isActive)
        {
            this.navigator?.removeRoute(this);
        }
    }

    public override void dispose()
    {
        this.curvedAnimation?.dispose();
        this.viewFadeOnIntervalCurve?.dispose();
        if (this.willDisposeSearchController)
        {
            this.searchController.dispose();
        }
        base.dispose();
    }

    public virtual void updateViewConfig(global::Doroti.Framework.Widgets.BuildContext context)
    {
        viewDefaults = DartRuntimePrimitives.ConvertValue<SearchViewThemeData>(new _SearchViewDefaultsM3__search_anchor(context, isFullScreen: this.showFullScreenView));
        viewTheme = SearchViewTheme.of(context);
    }

    public virtual void updateTweens(global::Doroti.Framework.Widgets.BuildContext context)
    {
        var navigator = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)Navigator.of(context).context.findRenderObject()!)!;
        global::Doroti.Ui.Size screenSize = ((global::Doroti.Ui.Size)(object?)((global::Doroti.Framework.Rendering.RenderBox)navigator).size);
        global::Doroti.Ui.Rect anchorRect = ((global::Doroti.Ui.Rect)(object?)(getRect() ?? Rect.zero));
        global::Doroti.Framework.Rendering.BoxConstraints effectiveConstraints = ((this.viewConstraints ?? this.viewTheme.constraints) ?? this.viewDefaults.constraints!);
        this._rectTween.begin = anchorRect;
        double viewWidth = Dart_uiLibrary.clampDouble(anchorRect.width, ((global::Doroti.Framework.Rendering.BoxConstraints)effectiveConstraints).minWidth, ((global::Doroti.Framework.Rendering.BoxConstraints)effectiveConstraints).maxWidth);
        double viewHeight = Dart_uiLibrary.clampDouble(((screenSize.height * 2L) / 3L), ((global::Doroti.Framework.Rendering.BoxConstraints)effectiveConstraints).minHeight, ((global::Doroti.Framework.Rendering.BoxConstraints)effectiveConstraints).maxHeight);
        switch ((this.textDirection ?? TextDirection.ltr))
        {
            case TextDirection.ltr:
                {
                    double viewLeftToScreenRight = (screenSize.width - anchorRect.left);
                    double viewTopToScreenBottom = (screenSize.height - anchorRect.top);
                    global::Doroti.Ui.Offset topLeftLocal = ((global::Doroti.Ui.Offset)(object?)anchorRect.topLeft);
                    if ((viewLeftToScreenRight < viewWidth))
                    {
                        topLeftLocal = new global::Doroti.Ui.Offset((screenSize.width - Math.Min(viewWidth, screenSize.width)), topLeftLocal.dy);
                    }
                    if ((viewTopToScreenBottom < viewHeight))
                    {
                        topLeftLocal = new global::Doroti.Ui.Offset(topLeftLocal.dx, (screenSize.height - Math.Min(viewHeight, screenSize.height)));
                    }
                    var endSize = new global::Doroti.Ui.Size(viewWidth, viewHeight);
                    this._rectTween.end = (this.showFullScreenView ? (Offset.zero & screenSize) : ((topLeftLocal & endSize)));
                    return;
                }
            case TextDirection.rtl:
                {
                    double viewRightToScreenLeft = anchorRect.right;
                    double viewTopToScreenBottomLocal = (screenSize.height - anchorRect.top);
                    var topLeftAlternate = new global::Doroti.Ui.Offset(Math.Max((anchorRect.right - viewWidth), 0.0), anchorRect.top);
                    if ((viewRightToScreenLeft < viewWidth))
                    {
                        topLeftAlternate = new global::Doroti.Ui.Offset(0.0, topLeftAlternate.dy);
                    }
                    if ((viewTopToScreenBottomLocal < viewHeight))
                    {
                        topLeftAlternate = new global::Doroti.Ui.Offset(topLeftAlternate.dx, (screenSize.height - Math.Min(viewHeight, screenSize.height)));
                    }
                    var endSizeLocal = new global::Doroti.Ui.Size(viewWidth, viewHeight);
                    this._rectTween.end = (this.showFullScreenView ? (Offset.zero & screenSize) : ((topLeftAlternate & endSizeLocal)));
                    break;
                }
        }
    }

    public override global::Doroti.Framework.Widgets.Widget buildPage(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation, global::Doroti.Framework.Animation.Animation<double> secondaryAnimation)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Directionality(textDirection: (this.textDirection ?? TextDirection.ltr), child: new global::Doroti.Framework.Widgets.AnimatedBuilder(animation: animation, builder: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>)((context, child) =>
        {
            curvedAnimation ??= new global::Doroti.Framework.Animation.CurvedAnimation(parent: animation, curve: global::Doroti.Framework.Animation.Curves.easeInOutCubicEmphasized, reverseCurve: global::Doroti.Framework.Animation.Curves.easeInOutCubicEmphasized.flipped);
            global::Doroti.Ui.Rect viewRectLocal = ((global::Doroti.Ui.Rect)(object?)DartRuntimePrimitives.RequireValue(this._rectTween.evaluate(this.curvedAnimation!)));
            double topPaddingLocal = (this.showFullScreenView ? DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(0.0, MediaQuery.paddingOf(context).top, this.curvedAnimation!.value)) : 0.0);
            viewFadeOnIntervalCurve ??= new global::Doroti.Framework.Animation.CurvedAnimation(parent: animation, curve: Search_anchorLibrary._kViewFadeOnInterval, reverseCurve: ((global::Doroti.Framework.Animation.Curve)Search_anchorLibrary._kViewFadeOnInterval).flipped);
            return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.FadeTransition(opacity: this.viewFadeOnIntervalCurve!, child: this.capturedThemes.wrap(new _ViewContent__search_anchor(viewOnChanged: (global::System.Action<string>?)this.viewOnChanged, viewOnSubmitted: (global::System.Action<string>?)this.viewOnSubmitted, viewLeading: this.viewLeading, viewTrailing: this.viewTrailing.Cast<global::Doroti.Framework.Widgets.Widget>(), viewHintText: this.viewHintText, viewBackgroundColor: this.viewBackgroundColor, viewElevation: this.viewElevation, viewSurfaceTintColor: this.viewSurfaceTintColor, viewSide: this.viewSide, viewShape: this.viewShape, viewBarPadding: this.viewBarPadding, viewHeaderHeight: this.viewHeaderHeight, viewHeaderTextStyle: this.viewHeaderTextStyle, viewHeaderHintStyle: this.viewHeaderHintStyle, dividerColor: this.dividerColor, viewConstraints: this.viewConstraints, viewPadding: this.viewPadding, shrinkWrap: this.shrinkWrap, showFullScreenView: this.showFullScreenView, animation: this.curvedAnimation!, topPadding: topPaddingLocal, viewMaxWidth: DartRuntimePrimitives.RequireValue(this._rectTween.end).width, viewRect: viewRectLocal, viewBuilder: (global::System.Func<IEnumerable<global::Doroti.Framework.Widgets.Widget>, global::Doroti.Framework.Widgets.Widget>?)this.viewBuilder, searchController: this.searchController, suggestionsBuilder: (global::System.Func<global::Doroti.Framework.Widgets.BuildContext, SearchController, object>)this.suggestionsBuilder, textCapitalization: this.textCapitalization, textInputAction: this.textInputAction, keyboardType: this.keyboardType, smartDashesType: this.smartDashesType, smartQuotesType: this.smartQuotesType))));
            throw new InvalidOperationException("Dart closure completed without a value.");
        })))));
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
    public virtual IEnumerable<global::Doroti.Framework.Widgets.Widget> result { get; set; } = ((IEnumerable<global::Doroti.Framework.Widgets.Widget>)(object?)new List<global::Doroti.Framework.Widgets.Widget>());
    public virtual string? searchValue { get; set; } = default;
    internal virtual Timer? _timer { get; set; } = default;

    public override void initState()
    {
        base.initState();
        _viewRect = ((_ViewContent__search_anchor)this.widget).viewRect;
        _controller = ((_ViewContent__search_anchor)this.widget).searchController;
        this._controller.addListener(this._updateSuggestionsListener);
        _setupAnimations();
    }

    public override void didUpdateWidget(_ViewContent__search_anchor oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((!object.Equals(((_ViewContent__search_anchor)this.widget).viewRect, ((_ViewContent__search_anchor)oldWidget).viewRect)))
        {
            setState(((global::System.Action)(() =>
            {
                _viewRect = ((_ViewContent__search_anchor)this.widget).viewRect;
            })));
        }
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        global::Doroti.Ui.Size updatedScreenSize = ((global::Doroti.Ui.Size)(object?)MediaQuery.of(this.context).size);
        if ((!object.Equals(this._screenSize, updatedScreenSize)))
        {
            _screenSize = updatedScreenSize;
            if (((_ViewContent__search_anchor)this.widget).showFullScreenView)
            {
                _viewRect = (Offset.zero & DartRuntimePrimitives.RequireValue(this._screenSize));
            }
        }
        if ((this.searchValue != this._controller.text))
        {
            this._timer?.cancel();
            _timer = new Timer(Duration.zero, (async () =>
            {
                searchValue = this._controller.text;
                IEnumerable<global::Doroti.Framework.Widgets.Widget> suggestions = await DartAsyncRuntime.AwaitFutureOrValue<IEnumerable<global::Doroti.Framework.Widgets.Widget>>(this.widget.suggestionsBuilder(this.context, this._controller));
                this._timer?.cancel();
                _timer = null;
                if (this.mounted)
                {
                    setState(((global::System.Action)(() =>
                    {
                        result = suggestions;
                    })));
                }
                throw new InvalidOperationException("Dart closure completed without a value.");
            }));
        }
    }

    public override void dispose()
    {
        this._controller.removeListener(this._updateSuggestionsListener);
        _disposeAnimations();
        this._timer?.cancel();
        _timer = null;
        base.dispose();
    }

    internal virtual void _setupAnimations()
    {
        viewIconsFadeCurve = new global::Doroti.Framework.Animation.CurvedAnimation(parent: ((_ViewContent__search_anchor)this.widget).animation, curve: Search_anchorLibrary._kViewIconsFadeOnInterval, reverseCurve: ((global::Doroti.Framework.Animation.Curve)Search_anchorLibrary._kViewIconsFadeOnInterval).flipped);
        viewDividerFadeCurve = new global::Doroti.Framework.Animation.CurvedAnimation(parent: ((_ViewContent__search_anchor)this.widget).animation, curve: Search_anchorLibrary._kViewDividerFadeOnInterval, reverseCurve: ((global::Doroti.Framework.Animation.Curve)Search_anchorLibrary._kViewFadeOnInterval).flipped);
        viewListFadeOnIntervalCurve = new global::Doroti.Framework.Animation.CurvedAnimation(parent: ((_ViewContent__search_anchor)this.widget).animation, curve: Search_anchorLibrary._kViewListFadeOnInterval, reverseCurve: ((global::Doroti.Framework.Animation.Curve)Search_anchorLibrary._kViewListFadeOnInterval).flipped);
    }

    internal virtual void _disposeAnimations()
    {
        this.viewIconsFadeCurve.dispose();
        this.viewDividerFadeCurve.dispose();
        this.viewListFadeOnIntervalCurve.dispose();
    }

    private void _updateSuggestionsListener() => _ = updateSuggestions();

    public async virtual Future updateSuggestions()
    {
        if ((this.searchValue != this._controller.text))
        {
            searchValue = this._controller.text;
            IEnumerable<global::Doroti.Framework.Widgets.Widget> suggestions = await DartAsyncRuntime.AwaitFutureOrValue<IEnumerable<global::Doroti.Framework.Widgets.Widget>>(this.widget.suggestionsBuilder(this.context, this._controller));
            if (this.mounted)
            {
                setState(((global::System.Action)(() =>
                {
                    result = suggestions;
                })));
            }
        }
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Framework.Widgets.Widget defaultLeading = ((global::Doroti.Framework.Widgets.Widget)(object?)new BackButton(style: new ButtonStyle(tapTargetSize: MaterialTapTargetSize.shrinkWrap), onPressed: (() =>
        {
            Navigator.of(context).pop<object>();
        })));
        var defaultTrailing = ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() =>
        {
            var __collection36360 = new List<global::Doroti.Framework.Widgets.Widget>(); if ((this._controller.text.Length != 0))
            {
                __collection36360.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new IconButton(icon: new global::Doroti.Framework.Widgets.Icon(Icons.close), tooltip: MaterialLocalizations.of(context).clearButtonTooltip, onPressed: (() =>
                {
                    this._controller.clear();
                }))));
            }
            return __collection36360;
        }))();
        SearchViewThemeData viewDefaults = ((SearchViewThemeData)(object?)new _SearchViewDefaultsM3__search_anchor(context, isFullScreen: ((_ViewContent__search_anchor)this.widget).showFullScreenView));
        SearchViewThemeData viewTheme = SearchViewTheme.of(context);
        DividerThemeData dividerTheme = DividerTheme.of(context);
        global::Doroti.Ui.Color effectiveBackgroundColor = ((global::Doroti.Ui.Color)(object?)((((_ViewContent__search_anchor)this.widget).viewBackgroundColor ?? viewTheme.backgroundColor) ?? viewDefaults.backgroundColor!));
        global::Doroti.Ui.Color effectiveSurfaceTint = ((global::Doroti.Ui.Color)(object?)((((_ViewContent__search_anchor)this.widget).viewSurfaceTintColor ?? viewTheme.surfaceTintColor) ?? viewDefaults.surfaceTintColor!));
        double effectiveElevation = ((((_ViewContent__search_anchor)this.widget).viewElevation ?? viewTheme.elevation) ?? DartRuntimePrimitives.RequireValue(viewDefaults.elevation));
        global::Doroti.Framework.Painting.BorderSide? effectiveSide = ((((_ViewContent__search_anchor)this.widget).viewSide ?? viewTheme.side) ?? viewDefaults.side);
        global::Doroti.Framework.Painting.OutlinedBorder effectiveShape = ((((_ViewContent__search_anchor)this.widget).viewShape ?? viewTheme.shape) ?? viewDefaults.shape!);
        if ((effectiveSide is not null))
        {
            effectiveShape = effectiveShape.copyWith(side: effectiveSide);
        }
        global::Doroti.Ui.Color effectiveDividerColor = ((global::Doroti.Ui.Color)(object?)(((((_ViewContent__search_anchor)this.widget).dividerColor ?? viewTheme.dividerColor) ?? dividerTheme.color) ?? viewDefaults.dividerColor!));
        double? effectiveHeaderHeight = (((_ViewContent__search_anchor)this.widget).viewHeaderHeight ?? viewTheme.headerHeight);
        global::Doroti.Framework.Rendering.BoxConstraints? headerConstraints = ((effectiveHeaderHeight is null) ? null : global::Doroti.Framework.Rendering.BoxConstraints.CreateTightFor(height: DartRuntimePrimitives.RequireValue(effectiveHeaderHeight)));
        global::Doroti.Framework.Painting.TextStyle? effectiveTextStyle = ((((_ViewContent__search_anchor)this.widget).viewHeaderTextStyle ?? viewTheme.headerTextStyle) ?? viewDefaults.headerTextStyle);
        global::Doroti.Framework.Painting.TextStyle? effectiveHintStyle = ((((((_ViewContent__search_anchor)this.widget).viewHeaderHintStyle ?? viewTheme.headerHintStyle) ?? ((_ViewContent__search_anchor)this.widget).viewHeaderTextStyle) ?? viewTheme.headerTextStyle) ?? viewDefaults.headerHintStyle);
        global::Doroti.Framework.Painting.EdgeInsetsGeometry? effectivePadding = ((((_ViewContent__search_anchor)this.widget).viewPadding ?? viewTheme.padding) ?? viewDefaults.padding);
        global::Doroti.Framework.Painting.EdgeInsetsGeometry? effectiveBarPadding = ((((_ViewContent__search_anchor)this.widget).viewBarPadding ?? viewTheme.barPadding) ?? viewDefaults.barPadding);
        global::Doroti.Framework.Rendering.BoxConstraints effectiveConstraints = ((((_ViewContent__search_anchor)this.widget).viewConstraints ?? viewTheme.constraints) ?? viewDefaults.constraints!);
        double minWidthLocal = Math.Min(((global::Doroti.Framework.Rendering.BoxConstraints)effectiveConstraints).minWidth, this._viewRect.width);
        double minHeightLocal = Math.Min(((global::Doroti.Framework.Rendering.BoxConstraints)effectiveConstraints).minHeight, this._viewRect.height);
        bool effectiveShrinkWrap = ((((_ViewContent__search_anchor)this.widget).shrinkWrap ?? viewTheme.shrinkWrap) ?? DartRuntimePrimitives.RequireValue(viewDefaults.shrinkWrap));
        global::Doroti.Framework.Widgets.Widget viewDivider = ((global::Doroti.Framework.Widgets.Widget)(object?)new DividerTheme(data: dividerTheme.copyWith(color: effectiveDividerColor), child: new Divider(height: 1)));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Align(alignment: global::Doroti.Framework.Painting.Alignment.topLeft, child: global::Doroti.Framework.Widgets.Transform.CreateTranslate(offset: this._viewRect.topLeft, child: new global::Doroti.Framework.Widgets.ConstrainedBox(constraints: new global::Doroti.Framework.Rendering.BoxConstraints(minWidth: minWidthLocal, maxWidth: this._viewRect.width, minHeight: minHeightLocal, maxHeight: this._viewRect.height), child: new global::Doroti.Framework.Widgets.Padding(padding: (((_ViewContent__search_anchor)this.widget).showFullScreenView ? global::Doroti.Framework.Painting.EdgeInsets.zero : ((effectivePadding ?? global::Doroti.Framework.Painting.EdgeInsets.zero))), child: new Material(clipBehavior: Clip.antiAlias, shape: effectiveShape, color: effectiveBackgroundColor, surfaceTintColor: effectiveSurfaceTint, elevation: effectiveElevation, child: new global::Doroti.Framework.Widgets.OverflowBox(alignment: global::Doroti.Framework.Painting.Alignment.topLeft, maxWidth: Math.Min(((_ViewContent__search_anchor)this.widget).viewMaxWidth, DartRuntimePrimitives.RequireValue(this._screenSize).width), minWidth: 0, fit: global::Doroti.Framework.Rendering.OverflowBoxFit.deferToChild, child: new global::Doroti.Framework.Widgets.FadeTransition(opacity: this.viewIconsFadeCurve, child: new global::Doroti.Framework.Widgets.Column(mainAxisSize: global::Doroti.Framework.Rendering.MainAxisSize.min, crossAxisAlignment: global::Doroti.Framework.Rendering.CrossAxisAlignment.stretch, children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() =>
        {
            var __collection40516 = new List<global::Doroti.Framework.Widgets.Widget>(); __collection40516.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsets.CreateOnly(top: ((_ViewContent__search_anchor)this.widget).topPadding), child: new global::Doroti.Framework.Widgets.SafeArea(top: false, bottom: false, child: new SearchBar(autoFocus: true, constraints: (headerConstraints ?? ((((_ViewContent__search_anchor)this.widget).showFullScreenView ? new global::Doroti.Framework.Rendering.BoxConstraints(minHeight: _SearchViewDefaultsM3__search_anchor.fullScreenBarHeight) : null))), padding: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Framework.Painting.EdgeInsetsGeometry?>(effectiveBarPadding), leading: (((_ViewContent__search_anchor)this.widget).viewLeading ?? defaultLeading), trailing: (((_ViewContent__search_anchor)this.widget).viewTrailing ?? defaultTrailing).Cast<global::Doroti.Framework.Widgets.Widget>(), hintText: ((_ViewContent__search_anchor)this.widget).viewHintText, backgroundColor: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Ui.Color>(Colors.transparent), overlayColor: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Ui.Color>(Colors.transparent), elevation: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<double?>(0.0), textStyle: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Framework.Painting.TextStyle?>(effectiveTextStyle), hintStyle: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Framework.Painting.TextStyle?>(effectiveHintStyle), controller: this._controller, onChanged: ((global::System.Action<string>)((value) =>
            {
                ((_ViewContent__search_anchor)this.widget).viewOnChanged?.Invoke(value);
                DartRuntimePrimitives.Ignore(updateSuggestions());
            })), onSubmitted: (global::System.Action<string>?)((_ViewContent__search_anchor)this.widget).viewOnSubmitted, textCapitalization: ((_ViewContent__search_anchor)this.widget).textCapitalization, textInputAction: ((_ViewContent__search_anchor)this.widget).textInputAction, keyboardType: ((_ViewContent__search_anchor)this.widget).keyboardType, smartDashesType: ((_ViewContent__search_anchor)this.widget).smartDashesType, smartQuotesType: ((_ViewContent__search_anchor)this.widget).smartQuotesType))))); if ((((!effectiveShrinkWrap || (minHeightLocal > 0L)) || ((_ViewContent__search_anchor)this.widget).showFullScreenView) || System.Linq.Enumerable.Any(this.result))) { __collection40516.AddRange(new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.FadeTransition(opacity: this.viewDividerFadeCurve, child: viewDivider)), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Flexible(fit: (((effectiveShrinkWrap && !((_ViewContent__search_anchor)this.widget).showFullScreenView)) ? global::Doroti.Framework.Rendering.FlexFit.loose : global::Doroti.Framework.Rendering.FlexFit.tight), child: new global::Doroti.Framework.Widgets.FadeTransition(opacity: this.viewListFadeOnIntervalCurve, child: ((((_ViewContent__search_anchor)this.widget).viewBuilder is null) ? global::Doroti.Framework.Widgets.MediaQuery.CreateRemovePadding(context: context, removeTop: true, child: new global::Doroti.Framework.Widgets.ListView(padding: global::Doroti.Framework.Painting.EdgeInsets.CreateOnly(bottom: MediaQuery.viewInsetsOf(context).bottom), shrinkWrap: effectiveShrinkWrap, children: this.result.ToList())) : ((_ViewContent__search_anchor)this.widget).viewBuilder!(this.result))))) }); }
            return __collection40516;
        }))())))))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _SearchAnchorWithSearchBar__search_anchor : SearchAnchor
{
    internal _SearchAnchorWithSearchBar__search_anchor(global::Doroti.Framework.Widgets.Widget? barLeading = null, IEnumerable<global::Doroti.Framework.Widgets.Widget>? barTrailing = null, string? barHintText = null, global::System.Action? onTap = null, global::Doroti.Framework.Widgets.WidgetStateProperty<double?>? barElevation = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? barBackgroundColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? barOverlayColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.BorderSide?>? barSide = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.OutlinedBorder?>? barShape = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry?>? barPadding = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? viewBarPadding = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.TextStyle?>? barTextStyle = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.TextStyle?>? barHintStyle = null, global::System.Func<IEnumerable<global::Doroti.Framework.Widgets.Widget>, global::Doroti.Framework.Widgets.Widget>? viewBuilder = null, global::Doroti.Framework.Widgets.Widget? viewLeading = null, IEnumerable<global::Doroti.Framework.Widgets.Widget>? viewTrailing = null, string? viewHintText = null, Color? viewBackgroundColor = null, double? viewElevation = null, global::Doroti.Framework.Painting.BorderSide? viewSide = null, global::Doroti.Framework.Painting.OutlinedBorder? viewShape = null, double? viewHeaderHeight = null, global::Doroti.Framework.Painting.TextStyle? viewHeaderTextStyle = null, global::Doroti.Framework.Painting.TextStyle? viewHeaderHintStyle = null, Color? dividerColor = null, global::Doroti.Framework.Rendering.BoxConstraints? constraints = null, global::Doroti.Framework.Rendering.BoxConstraints? viewConstraints = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? viewPadding = null, bool? shrinkWrap = null, bool? isFullScreen = null, SearchController? searchController = null, global::Doroti.Framework.Services.TextCapitalization? textCapitalization = null, global::System.Action<string>? onChanged = null, global::System.Action<string>? onSubmitted = null, global::System.Action? onClose = null, global::System.Action? onOpen = null, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, SearchController, object> suggestionsBuilder = default!, global::Doroti.Framework.Services.TextInputAction? textInputAction = null, global::Doroti.Framework.Services.TextInputType? keyboardType = null, global::Doroti.Framework.Painting.EdgeInsets scrollPadding = default!, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.EditableTextState, global::Doroti.Framework.Widgets.Widget> contextMenuBuilder = default!, bool enabled = true, global::Doroti.Framework.Services.SmartDashesType? smartDashesType = null, global::Doroti.Framework.Services.SmartQuotesType? smartQuotesType = null) : base(viewBarPadding: viewBarPadding, viewBuilder: viewBuilder, viewLeading: viewLeading, viewTrailing: viewTrailing, viewBackgroundColor: viewBackgroundColor, viewElevation: viewElevation, viewSide: viewSide, viewShape: viewShape, dividerColor: dividerColor, viewConstraints: viewConstraints, viewPadding: viewPadding, shrinkWrap: shrinkWrap, isFullScreen: isFullScreen, searchController: searchController, textCapitalization: textCapitalization, suggestionsBuilder: suggestionsBuilder, textInputAction: textInputAction, keyboardType: keyboardType, enabled: enabled, smartDashesType: smartDashesType, smartQuotesType: smartQuotesType, viewHintText: (viewHintText ?? barHintText), headerHeight: viewHeaderHeight, headerTextStyle: viewHeaderTextStyle, headerHintStyle: viewHeaderHintStyle, viewOnSubmitted: (global::System.Action<string>?)onSubmitted, viewOnChanged: (global::System.Action<string>?)onChanged, viewOnClose: () => onClose(), viewOnOpen: () => onOpen(), builder: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, SearchController, global::Doroti.Framework.Widgets.Widget>)((context, controller) =>
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new SearchBar(constraints: constraints, controller: controller, onTap: ((global::System.Action)(() =>
        {
            controller.openView();
            onTap?.Invoke();
        })), onChanged: ((global::System.Action<string>)((value) =>
        {
            controller.openView();
        })), onSubmitted: (global::System.Action<string>?)onSubmitted, hintText: barHintText, hintStyle: barHintStyle, textStyle: barTextStyle, elevation: barElevation, backgroundColor: barBackgroundColor, overlayColor: barOverlayColor, side: barSide, shape: barShape, padding: DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry?>>((object?)barPadding ?? new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Framework.Painting.EdgeInsetsGeometry?>(global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 16.0))), leading: (barLeading ?? new global::Doroti.Framework.Widgets.Icon(Icons.search)), trailing: barTrailing.Cast<global::Doroti.Framework.Widgets.Widget>(), textCapitalization: textCapitalization, textInputAction: textInputAction, keyboardType: keyboardType, scrollPadding: scrollPadding ?? global::Doroti.Framework.Painting.EdgeInsets.CreateAll(20.0), contextMenuBuilder: (global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.EditableTextState, global::Doroti.Framework.Widgets.Widget>)contextMenuBuilder, smartDashesType: smartDashesType, smartQuotesType: smartQuotesType));
        throw new InvalidOperationException("Dart closure completed without a value.");
    })))
    {
        global::Doroti.Framework.Painting.EdgeInsets __scrollPadding = scrollPadding ?? global::Doroti.Framework.Painting.EdgeInsets.CreateAll(20.0);
        global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.EditableTextState, global::Doroti.Framework.Widgets.Widget> __contextMenuBuilder = contextMenuBuilder ?? SearchBar._defaultContextMenuBuilder;
    }

}

public class SearchController : global::Doroti.Framework.Widgets.TextEditingController
{
    internal virtual _SearchAnchorState__search_anchor? _anchor { get; set; } = default;

    public virtual bool isAttached => DartRuntimePrimitives.ConvertValue<bool>((this._anchor is not null));
    public virtual bool isOpen
    {
        get
        {
            DartRuntimePrimitives.Assert(() => this.isAttached);
            return this._anchor!._viewIsOpen;
            return default!;
        }
    }
    public virtual void openView()
    {
        DartRuntimePrimitives.Assert(() => this.isAttached);
        this._anchor!._openView();
    }

    public virtual void closeView(string? selectedText)
    {
        DartRuntimePrimitives.Assert(() => this.isAttached);
        this._anchor!._closeView(selectedText);
    }

    internal virtual void _attach(_SearchAnchorState__search_anchor anchor)
    {
        _anchor = anchor;
    }

    internal virtual void _detach(_SearchAnchorState__search_anchor anchor)
    {
        if ((object.Equals(this._anchor, anchor)))
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
        global::Doroti.Framework.Painting.EdgeInsets __scrollPadding = scrollPadding ?? global::Doroti.Framework.Painting.EdgeInsets.CreateAll(20.0);
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
            return ((global::Doroti.Framework.Widgets.Widget)(object?)global::Doroti.Framework.Widgets.SystemContextMenu.CreateEditableText(editableTextState: editableTextState));
        }
        return ((global::Doroti.Framework.Widgets.Widget)(object?)AdaptiveTextSelectionToolbar.CreateEditableText(editableTextState: editableTextState));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _SearchBarState__search_anchor());
}

internal class _SearchBarState__search_anchor : global::Doroti.Framework.Widgets.State<SearchBar>
{
    internal virtual global::Doroti.Framework.Widgets.WidgetStatesController _internalStatesController { get; private set; } = default!;
    internal virtual global::Doroti.Framework.Widgets.FocusNode? _internalFocusNode { get; set; } = default;

    internal virtual global::Doroti.Framework.Widgets.FocusNode _focusNode => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.FocusNode>((((SearchBar)this.widget).focusNode ?? (_internalFocusNode ??= new global::Doroti.Framework.Widgets.FocusNode())));
    public override void initState()
    {
        base.initState();
        _internalStatesController = new global::Doroti.Framework.Widgets.WidgetStatesController();
        this._internalStatesController.addListener(((global::System.Action)(() =>
        {
            setState(((global::System.Action)(() =>
            {
            })));
        })));
    }

    public override void dispose()
    {
        this._internalStatesController.dispose();
        this._internalFocusNode?.dispose();
        base.dispose();
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Ui.TextDirection textDirectionLocal = Directionality.of(context);
        ColorScheme colorSchemeLocal = Theme.of(context).colorScheme;
        SearchBarThemeData searchBarTheme = SearchBarTheme.of(context);
        SearchBarThemeData defaults = ((SearchBarThemeData)(object?)new _SearchBarDefaultsM3__search_anchor(context));
        P? resolve<P>(global::Doroti.Framework.Widgets.WidgetStateProperty<P>? widgetValue, global::Doroti.Framework.Widgets.WidgetStateProperty<P>? themeValue, global::Doroti.Framework.Widgets.WidgetStateProperty<P>? defaultValue)
        {
            HashSet<global::Doroti.Framework.Widgets.WidgetState> states = ((HashSet<global::Doroti.Framework.Widgets.WidgetState>)(object?)this._internalStatesController.value);
            return widgetValue is not null ? widgetValue.resolve(states) : themeValue is not null ? themeValue.resolve(states) : defaultValue is not null ? defaultValue.resolve(states) : default;
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        global::Doroti.Framework.Painting.TextStyle? effectiveTextStyle = resolve<global::Doroti.Framework.Painting.TextStyle?>(((SearchBar)this.widget).textStyle, searchBarTheme.textStyle, defaults.textStyle);
        double? effectiveElevation = resolve<double?>(((SearchBar)this.widget).elevation, searchBarTheme.elevation, defaults.elevation);
        global::Doroti.Ui.Color? effectiveShadowColor = ((global::Doroti.Ui.Color?)(object?)resolve<global::Doroti.Ui.Color?>(((SearchBar)this.widget).shadowColor, searchBarTheme.shadowColor, defaults.shadowColor));
        global::Doroti.Ui.Color? effectiveBackgroundColor = ((global::Doroti.Ui.Color?)(object?)resolve<global::Doroti.Ui.Color?>(((SearchBar)this.widget).backgroundColor, searchBarTheme.backgroundColor, defaults.backgroundColor));
        global::Doroti.Ui.Color? effectiveSurfaceTintColor = ((global::Doroti.Ui.Color?)(object?)resolve<global::Doroti.Ui.Color?>(((SearchBar)this.widget).surfaceTintColor, searchBarTheme.surfaceTintColor, defaults.surfaceTintColor));
        global::Doroti.Framework.Painting.OutlinedBorder? effectiveShape = resolve<global::Doroti.Framework.Painting.OutlinedBorder?>(((SearchBar)this.widget).shape, searchBarTheme.shape, defaults.shape);
        global::Doroti.Framework.Painting.BorderSide? effectiveSide = resolve<global::Doroti.Framework.Painting.BorderSide?>(((SearchBar)this.widget).side, searchBarTheme.side, defaults.side);
        global::Doroti.Framework.Painting.EdgeInsetsGeometry? effectivePadding = resolve<global::Doroti.Framework.Painting.EdgeInsetsGeometry?>(((SearchBar)this.widget).padding, searchBarTheme.padding, defaults.padding);
        global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? effectiveOverlayColor = ((global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>?)(object?)((((SearchBar)this.widget).overlayColor ?? searchBarTheme.overlayColor) ?? defaults.overlayColor));
        global::Doroti.Framework.Services.TextCapitalization effectiveTextCapitalization = ((((SearchBar)this.widget).textCapitalization ?? searchBarTheme.textCapitalization) ?? DartRuntimePrimitives.RequireValue(defaults.textCapitalization));
        HashSet<global::Doroti.Framework.Widgets.WidgetState> statesLocal = ((HashSet<global::Doroti.Framework.Widgets.WidgetState>)(object?)this._internalStatesController.value);
        global::Doroti.Framework.Painting.TextStyle? effectiveHintStyle = ((((((((((SearchBar)this.widget).hintStyle?.resolve(statesLocal) ?? (global::Doroti.Framework.Painting.TextStyle)searchBarTheme.hintStyle?.resolve(statesLocal))) ?? (global::Doroti.Framework.Painting.TextStyle)((SearchBar)this.widget).textStyle?.resolve(statesLocal))) ?? (global::Doroti.Framework.Painting.TextStyle)searchBarTheme.textStyle?.resolve(statesLocal))) ?? (global::Doroti.Framework.Painting.TextStyle)defaults.hintStyle?.resolve(statesLocal)));
        global::Doroti.Ui.Color defaultColor = ((global::Doroti.Ui.Color)(object?)(colorSchemeLocal.brightness switch { Brightness.light => ConstantsLibrary.kDefaultIconDarkColor, Brightness.dark => ConstantsLibrary.kDefaultIconLightColor, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
        global::Doroti.Framework.Widgets.IconThemeData? customTheme = (IconTheme.of(context) switch { global::Doroti.Framework.Widgets.IconThemeData iconTheme when ((!object.Equals(((global::Doroti.Framework.Widgets.IconThemeData)iconTheme).color, defaultColor))) => iconTheme, _ => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.IconThemeData>(null) });
        global::Doroti.Framework.Widgets.Widget? leadingLocal = default!;
        if ((((SearchBar)this.widget).leading is not null))
        {
            leadingLocal = IconTheme.merge(data: (customTheme ?? new global::Doroti.Framework.Widgets.IconThemeData(color: colorSchemeLocal.onSurface)), child: ((SearchBar)this.widget).leading!);
        }
        List<global::Doroti.Framework.Widgets.Widget>? trailingLocal = ((SearchBar)this.widget).trailing?.map<global::Doroti.Framework.Widgets.Widget, global::Doroti.Framework.Widgets.Widget>(((trailing) => IconTheme.merge(data: (customTheme ?? new global::Doroti.Framework.Widgets.IconThemeData(color: colorSchemeLocal.onSurfaceVariant)), child: trailing))).ToList().ToList();
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.ConstrainedBox(constraints: ((((SearchBar)this.widget).constraints ?? searchBarTheme.constraints) ?? defaults.constraints!), child: new global::Doroti.Framework.Widgets.Opacity(opacity: (((SearchBar)this.widget).enabled ? 1 : Search_anchorLibrary._kDisableSearchBarOpacity), child: new Material(elevation: DartRuntimePrimitives.RequireValue(effectiveElevation), shadowColor: effectiveShadowColor, color: effectiveBackgroundColor, surfaceTintColor: effectiveSurfaceTintColor, shape: effectiveShape?.copyWith(side: effectiveSide), child: new global::Doroti.Framework.Widgets.IgnorePointer(ignoring: !((SearchBar)this.widget).enabled, child: new InkWell(onTap: (() =>
        {
            ((SearchBar)this.widget).onTap?.Invoke();
            if (!((global::Doroti.Framework.Widgets.FocusNode)this._focusNode).hasFocus)
            {
                this._focusNode.requestFocus();
            }
        }), overlayColor: effectiveOverlayColor, customBorder: effectiveShape?.copyWith(side: effectiveSide), statesController: this._internalStatesController, child: new global::Doroti.Framework.Widgets.Padding(padding: effectivePadding!, child: new global::Doroti.Framework.Widgets.Row(textDirection: textDirectionLocal, children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection64758 = new List<global::Doroti.Framework.Widgets.Widget>(); var __collectionElement64788 = leadingLocal; if (__collectionElement64788 is { } __nonNullCollectionElement64788) { __collection64758.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(__nonNullCollectionElement64788)); } __collection64758.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Expanded(child: new global::Doroti.Framework.Widgets.Padding(padding: effectivePadding, child: new global::Doroti.Framework.Widgets.Semantics(inputType: SemanticsInputType.search, child: new TextField(readOnly: ((SearchBar)this.widget).readOnly, autofocus: ((SearchBar)this.widget).autoFocus, onTap: () => ((SearchBar)this.widget).onTap(), onTapAlwaysCalled: true, onTapOutside: (global::System.Action<global::Doroti.Framework.Gestures.PointerDownEvent>?)((SearchBar)this.widget).onTapOutside, focusNode: this._focusNode, onChanged: (global::System.Action<string>?)((SearchBar)this.widget).onChanged, onSubmitted: (global::System.Action<string>?)((SearchBar)this.widget).onSubmitted, controller: ((SearchBar)this.widget).controller, style: effectiveTextStyle, enabled: ((SearchBar)this.widget).enabled, decoration: new InputDecoration(hintText: ((SearchBar)this.widget).hintText).applyDefaults(new InputDecorationThemeData(hintStyle: effectiveHintStyle, enabledBorder: InputBorder.none, border: InputBorder.none, focusedBorder: InputBorder.none, contentPadding: global::Doroti.Framework.Painting.EdgeInsets.zero, isDense: true)), textCapitalization: effectiveTextCapitalization, textInputAction: ((SearchBar)this.widget).textInputAction, keyboardType: ((SearchBar)this.widget).keyboardType, scrollPadding: ((SearchBar)this.widget).scrollPadding, contextMenuBuilder: (global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.EditableTextState, global::Doroti.Framework.Widgets.Widget>?)((SearchBar)this.widget).contextMenuBuilder, smartDashesType: ((SearchBar)this.widget).smartDashesType, smartQuotesType: ((SearchBar)this.widget).smartQuotesType)))))); var __collectionSpread67233 = trailingLocal; if (__collectionSpread67233 is not null) { __collection64758.AddRange(__collectionSpread67233); } return __collection64758; }))()))))))));
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
                __late__colors = Theme.of(this.context).colorScheme;
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
                __late__textTheme = Theme.of(this.context).textTheme;
                __late__textTheme_initialized = true;
            }
            return __late__textTheme;
        }
    }

    internal _SearchBarDefaultsM3__search_anchor(global::Doroti.Framework.Widgets.BuildContext context)
    {
        this.context = context;
    }

    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? backgroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Ui.Color>(this._colors.surfaceContainerHigh));
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<double>? elevation => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<double>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<double?>(6.0));
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color>? shadowColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Ui.Color>(this._colors.shadow));
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color>? surfaceTintColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Ui.Color>(Colors.transparent));
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? overlayColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>(WidgetStateProperty.resolveWith((states) =>
    {
        if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.pressed))
        {
            return (this._colors.onSurface.withOpacity(0.1));
        }
        if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.hovered))
        {
            return (this._colors.onSurface.withOpacity(0.08));
        }
        if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.focused))
        {
            return (Colors.transparent);
        }
        return (Colors.transparent);
        throw new InvalidOperationException("Dart closure completed without a value.");
    }));
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.OutlinedBorder>? shape => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.OutlinedBorder>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Framework.Painting.OutlinedBorder>(new global::Doroti.Framework.Painting.StadiumBorder()));
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry>? padding => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Framework.Painting.EdgeInsetsGeometry>(global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 8.0)));
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.TextStyle?> textStyle => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.TextStyle?>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Framework.Painting.TextStyle?>(this._textTheme.bodyLarge?.copyWith(color: this._colors.onSurface)));
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.TextStyle?> hintStyle => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.TextStyle?>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Framework.Painting.TextStyle?>(this._textTheme.bodyLarge?.copyWith(color: this._colors.onSurfaceVariant)));
    public virtual global::Doroti.Framework.Rendering.BoxConstraints constraints => new global::Doroti.Framework.Rendering.BoxConstraints(minWidth: 360.0, maxWidth: 800.0, minHeight: 56.0);
    public virtual global::Doroti.Framework.Services.TextCapitalization textCapitalization => global::Doroti.Framework.Services.TextCapitalization.none;
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
                __late__colors = Theme.of(this.context).colorScheme;
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
                __late__textTheme = Theme.of(this.context).textTheme;
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

    public virtual global::Doroti.Ui.Color? backgroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.surfaceContainerHigh);
    public override double? elevation => 6.0;
    public virtual global::Doroti.Ui.Color? surfaceTintColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(Colors.transparent);
    public override global::Doroti.Framework.Painting.OutlinedBorder? shape => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.OutlinedBorder>((this.isFullScreen ? new global::Doroti.Framework.Painting.RoundedRectangleBorder() : new global::Doroti.Framework.Painting.RoundedRectangleBorder(borderRadius: global::Doroti.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Ui.Radius.circular(28.0)))));
    public override global::Doroti.Framework.Painting.TextStyle? headerTextStyle => this._textTheme.bodyLarge?.copyWith(color: this._colors.onSurface);
    public override global::Doroti.Framework.Painting.TextStyle? headerHintStyle => this._textTheme.bodyLarge?.copyWith(color: this._colors.onSurfaceVariant);
    public virtual global::Doroti.Framework.Rendering.BoxConstraints constraints => new global::Doroti.Framework.Rendering.BoxConstraints(minWidth: 360.0, minHeight: 240.0);
    public override global::Doroti.Framework.Painting.EdgeInsetsGeometry? barPadding => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.EdgeInsetsGeometry>(global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 8.0));
    public virtual bool shrinkWrap => false;
    public virtual global::Doroti.Ui.Color? dividerColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.outline);
}
