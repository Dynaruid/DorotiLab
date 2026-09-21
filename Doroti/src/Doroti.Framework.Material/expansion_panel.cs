// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/expansion_panel.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public static partial class Expansion_panelLibrary
{
    internal static double _kPanelHeaderCollapsedHeight = ConstantsLibrary.kMinInteractiveDimension;
}

public static partial class Expansion_panelLibrary
{
    internal static EdgeInsets _kPanelHeaderExpandedDefaultPadding = EdgeInsets.CreateSymmetric(
        vertical: 64.0 - _kPanelHeaderCollapsedHeight
    );
}

public static partial class Expansion_panelLibrary
{
    internal static EdgeInsets _kExpandIconPadding = EdgeInsets.CreateAll(12.0);
}

internal class _SaltedKey__expansion_panel<S, V> : LocalKey
{
    public virtual S salt { get; private set; } = default!;
    public virtual V value { get; private set; } = default!;

    internal _SaltedKey__expansion_panel(S salt, V value)
    {
        this.salt = salt;
        this.value = value;
    }

    public override bool Equals(object? other)
    {
        var __other = other as _SaltedKey__expansion_panel<S, V>;
        if (__other is null)
        {
            return false;
        }

        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is _SaltedKey__expansion_panel<S, V>)
            && EqualityComparer<S>.Default.Equals(__other.salt, salt)
            && EqualityComparer<V>.Default.Equals(__other.value, value);
    }

    public override int GetHashCode() =>
        DartRuntimePrimitives.ConvertValue<int>(
            FoundationRuntimePorts.ObjectHash(GetType(), salt, value)
        );

    public override string ToString()
    {
        var saltString = Equals(typeof(S), typeof(string)) ? $"<'{salt}'>" : $"<{salt}>";
        var valueString = Equals(typeof(V), typeof(string)) ? $"<'{value}'>" : $"<{value}>";
        return $"[{saltString} {valueString}]";
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public delegate void ExpansionPanelCallback(long panelIndex, bool isExpanded);

public delegate Widget ExpansionPanelHeaderBuilder(BuildContext context, bool isExpanded);

public class ExpansionPanel
{
    public virtual Func<BuildContext, bool, Widget> headerBuilder { get; private set; } = default!;
    public virtual Widget body { get; private set; } = default!;
    public virtual bool isExpanded { get; private set; } = default!;
    public virtual Color? splashColor { get; private set; }
    public virtual Color? highlightColor { get; private set; }
    public virtual bool canTapOnHeader { get; private set; } = default!;
    public virtual Color? backgroundColor { get; private set; }

    public ExpansionPanel(
        Func<BuildContext, bool, Widget> headerBuilder,
        Widget body,
        bool isExpanded = false,
        bool canTapOnHeader = false,
        Color? backgroundColor = null,
        Color? splashColor = null,
        Color? highlightColor = null
    )
    {
        this.headerBuilder = headerBuilder;
        this.body = body;
        this.isExpanded = isExpanded;
        this.canTapOnHeader = canTapOnHeader;
        this.backgroundColor = backgroundColor;
        this.splashColor = splashColor;
        this.highlightColor = highlightColor;
    }
}

public class ExpansionPanelRadio : ExpansionPanel
{
    public virtual object value { get; private set; } = default!;

    public ExpansionPanelRadio(
        object value,
        Func<BuildContext, bool, Widget> headerBuilder,
        Widget body,
        bool canTapOnHeader = false,
        Color? backgroundColor = null,
        Color? splashColor = null,
        Color? highlightColor = null
    )
        : base(
            headerBuilder: headerBuilder,
            body: body,
            canTapOnHeader: canTapOnHeader,
            backgroundColor: backgroundColor,
            splashColor: splashColor,
            highlightColor: highlightColor
        )
    {
        this.value = value;
    }
}

public class ExpansionPanelList : StatefulWidget
{
    public virtual List<ExpansionPanel> children { get; private set; } = default!;
    public virtual Action<long, bool>? expansionCallback { get; private set; }
    public virtual Duration animationDuration { get; private set; } = default!;
    internal virtual bool _allowOnlyOnePanelOpen { get; private set; } = default!;
    public virtual object? initialOpenPanelValue { get; private set; }
    public virtual EdgeInsets expandedHeaderPadding { get; private set; } = default!;
    public virtual Color? dividerColor { get; private set; }
    public virtual double elevation { get; private set; } = default!;
    public virtual Color? expandIconColor { get; private set; }
    public virtual double materialGapSize { get; private set; } = default!;

    public ExpansionPanelList(
        Key? key = null,
        List<ExpansionPanel> children = default!,
        Action<long, bool>? expansionCallback = null,
        Duration? animationDuration = null,
        EdgeInsets expandedHeaderPadding = default!,
        Color? dividerColor = null,
        double elevation = 2,
        Color? expandIconColor = null,
        double materialGapSize = 16.0
    )
        : base(key: key)
    {
        List<ExpansionPanel> __children = children ?? new List<ExpansionPanel>();
        Duration __animationDuration = animationDuration ?? ThemeLibrary.kThemeAnimationDuration;
        EdgeInsets __expandedHeaderPadding =
            expandedHeaderPadding ?? Expansion_panelLibrary._kPanelHeaderExpandedDefaultPadding;
        this.children = __children;
        this.expansionCallback = expansionCallback;
        this.animationDuration = __animationDuration;
        this.expandedHeaderPadding = __expandedHeaderPadding;
        this.dividerColor = dividerColor;
        this.elevation = elevation;
        this.expandIconColor = expandIconColor;
        this.materialGapSize = materialGapSize;
        _allowOnlyOnePanelOpen = false;
        initialOpenPanelValue = null;
    }

    public static ExpansionPanelList CreateRadio(
        Key? key = null,
        List<ExpansionPanel> children = default!,
        Action<long, bool>? expansionCallback = null,
        Duration? animationDuration = null,
        object? initialOpenPanelValue = null,
        EdgeInsets expandedHeaderPadding = default!,
        Color? dividerColor = null,
        double elevation = 2,
        Color? expandIconColor = null,
        double materialGapSize = 16.0
    )
    {
        var __instance = new ExpansionPanelList(
            key: key,
            children: children,
            expansionCallback: expansionCallback,
            animationDuration: animationDuration,
            expandedHeaderPadding: expandedHeaderPadding,
            dividerColor: dividerColor,
            elevation: elevation,
            expandIconColor: expandIconColor,
            materialGapSize: materialGapSize
        );
        List<ExpansionPanel> __children = children ?? new List<ExpansionPanel>();
        Duration __animationDuration = animationDuration ?? ThemeLibrary.kThemeAnimationDuration;
        EdgeInsets __expandedHeaderPadding =
            expandedHeaderPadding ?? Expansion_panelLibrary._kPanelHeaderExpandedDefaultPadding;
        __instance.children = __children;
        __instance.expansionCallback = expansionCallback;
        __instance.animationDuration = __animationDuration;
        __instance.initialOpenPanelValue = initialOpenPanelValue;
        __instance.expandedHeaderPadding = __expandedHeaderPadding;
        __instance.dividerColor = dividerColor;
        __instance.elevation = elevation;
        __instance.expandIconColor = expandIconColor;
        __instance.materialGapSize = materialGapSize;
        __instance._allowOnlyOnePanelOpen = true;
        return __instance;
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _ExpansionPanelListState__expansion_panel());
}

internal class _ExpansionPanelListState__expansion_panel : State<ExpansionPanelList>
{
    internal virtual ExpansionPanelRadio? _currentOpenPanel { get; set; } = default;

    public override void initState()
    {
        base.initState();
        if (widget._allowOnlyOnePanelOpen)
        {
            DartRuntimePrimitives.Assert(
                () => _allIdentifiersUnique(),
                () => (object?)"All ExpansionPanelRadio identifier values must be unique."
            );
            if (widget.initialOpenPanelValue is not null)
            {
                _currentOpenPanel = searchPanelByValue(
                    widget.children.cast<ExpansionPanelRadio>().ToList(),
                    widget.initialOpenPanelValue
                );
            }
        }
    }

    public override void didUpdateWidget(ExpansionPanelList oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (widget._allowOnlyOnePanelOpen)
        {
            DartRuntimePrimitives.Assert(
                () => _allIdentifiersUnique(),
                () => (object?)"All ExpansionPanelRadio identifier values must be unique."
            );
            if (!oldWidget._allowOnlyOnePanelOpen)
            {
                _currentOpenPanel = searchPanelByValue(
                    widget.children.cast<ExpansionPanelRadio>().ToList(),
                    widget.initialOpenPanelValue
                );
            }
        }
        else
        {
            _currentOpenPanel = null;
        }
    }

    internal virtual bool _allIdentifiersUnique()
    {
        var identifierMap = new DartMap<object, bool>();
        foreach (ExpansionPanelRadio child in widget.children.cast<ExpansionPanelRadio>())
        {
            identifierMap[child.value] = true;
        }
        return checked(identifierMap.Count) == checked((long)widget.children.Count);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual bool _isChildExpanded(long index)
    {
        if (widget._allowOnlyOnePanelOpen)
        {
            var radioWidget = ((ExpansionPanelRadio?)widget.children[(int)index])!;
            return Equals(_currentOpenPanel?.value, radioWidget.value);
        }
        return widget.children[(int)index].isExpanded;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual void _handlePressed(bool isExpanded, long index)
    {
        if (widget._allowOnlyOnePanelOpen)
        {
            var pressedChild = ((ExpansionPanelRadio?)widget.children[(int)index])!;
            for (var childIndex = 0L; childIndex < checked(widget.children.Count); childIndex += 1L)
            {
                var child = ((ExpansionPanelRadio?)widget.children[(int)childIndex])!;
                if (
                    (widget.expansionCallback is not null)
                    && (childIndex != index)
                    && Equals(child.value, _currentOpenPanel?.value)
                )
                {
                    widget.expansionCallback!(childIndex, false);
                }
            }
            setState(() =>
            {
                _currentOpenPanel = isExpanded ? null : pressedChild;
            });
        }
        widget.expansionCallback?.Invoke(index, !isExpanded);
    }

    public virtual ExpansionPanelRadio? searchPanelByValue(
        List<ExpansionPanelRadio> panels,
        object? value
    )
    {
        foreach (var panel in panels)
        {
            if (Equals(panel.value, value))
            {
                return panel;
            }
        }
        return null;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(
            () => ShadowsLibrary.kElevationToShadow.ContainsKey(checked((long)widget.elevation)),
            () =>
                (object?)"Invalid value for elevation. See the kElevationToShadow constant for"
                + " possible elevation values."
        );
        var items = new List<MergeableMaterialItem>();
        for (var index = 0L; index < checked(widget.children.Count); index += 1L)
        {
            if (_isChildExpanded(index) && (index != 0L) && !_isChildExpanded(index - 1L))
            {
                items.Add(
                    new MaterialGap(
                        key: new _SaltedKey__expansion_panel<BuildContext, long>(
                            context,
                            (index * 2L) - 1L
                        ),
                        size: widget.materialGapSize
                    )
                );
            }
            ExpansionPanel childLocal = widget.children[(int)index];
            Widget headerWidget = childLocal.headerBuilder(context, _isChildExpanded(index));
            Widget expandIconPadded = new Padding(
                padding: EdgeInsetsDirectional.CreateOnly(end: 8.0),
                child: new IgnorePointer(
                    ignoring: childLocal.canTapOnHeader,
                    child: new ExpandIcon(
                        color: widget.expandIconColor,
                        isExpanded: _isChildExpanded(index),
                        padding: Expansion_panelLibrary._kExpandIconPadding,
                        splashColor: childLocal.splashColor,
                        highlightColor: childLocal.highlightColor,
                        onPressed: (isExpanded) =>
                        {
                            _handlePressed(isExpanded, index);
                        }
                    )
                )
            );
            if (!childLocal.canTapOnHeader)
            {
                MaterialLocalizations localizations = MaterialLocalizations.of(context);
                expandIconPadded = DartRuntimePrimitives.ConvertValue<Widget>(
                    new Widgets.Semantics(
                        label: _isChildExpanded(index)
                            ? localizations.expandedIconTapHint
                            : localizations.collapsedIconTapHint,
                        container: true,
                        child: expandIconPadded
                    )
                );
            }
            Widget header = new Row(
                children: new List<Widget>
                {
                    DartRuntimePrimitives.ConvertValue<Widget>(
                        new Expanded(
                            child: new AnimatedContainer(
                                duration: widget.animationDuration,
                                curve: Curves.fastOutSlowIn,
                                margin: _isChildExpanded(index)
                                    ? widget.expandedHeaderPadding
                                    : EdgeInsets.zero,
                                child: new ConstrainedBox(
                                    constraints: new BoxConstraints(
                                        minHeight: Expansion_panelLibrary._kPanelHeaderCollapsedHeight
                                    ),
                                    child: headerWidget
                                )
                            )
                        )
                    ),
                    DartRuntimePrimitives.ConvertValue<Widget>(expandIconPadded),
                }
            );
            if (childLocal.canTapOnHeader)
            {
                header = DartRuntimePrimitives.ConvertValue<Widget>(
                    new MergeSemantics(
                        child: new InkWell(
                            splashColor: childLocal.splashColor,
                            highlightColor: childLocal.highlightColor,
                            onTap: () =>
                            {
                                _handlePressed(_isChildExpanded(index), index);
                            },
                            child: header
                        )
                    )
                );
            }
            items.Add(
                new MaterialSlice(
                    key: new _SaltedKey__expansion_panel<BuildContext, long>(context, index * 2L),
                    color: childLocal.backgroundColor,
                    child: new Column(
                        children: new List<Widget>
                        {
                            DartRuntimePrimitives.ConvertValue<Widget>(header),
                            DartRuntimePrimitives.ConvertValue<Widget>(
                                new AnimatedCrossFade(
                                    firstChild: new LimitedBox(
                                        maxWidth: 0.0,
                                        child: new SizedBox(
                                            width: double.PositiveInfinity,
                                            height: 0
                                        )
                                    ),
                                    secondChild: childLocal.body,
                                    firstCurve: new Interval(0.0, 0.6, curve: Curves.fastOutSlowIn),
                                    secondCurve: new Interval(
                                        0.4,
                                        1.0,
                                        curve: Curves.fastOutSlowIn
                                    ),
                                    sizeCurve: Curves.fastOutSlowIn,
                                    crossFadeState: _isChildExpanded(index)
                                        ? CrossFadeState.showSecond
                                        : CrossFadeState.showFirst,
                                    duration: widget.animationDuration
                                )
                            ),
                        }
                    )
                )
            );
            if (_isChildExpanded(index) && (index != (checked(widget.children.Count) - 1L)))
            {
                items.Add(
                    new MaterialGap(
                        key: new _SaltedKey__expansion_panel<BuildContext, long>(
                            context,
                            (index * 2L) + 1L
                        ),
                        size: widget.materialGapSize
                    )
                );
            }
        }
        return new MergeableMaterial(
            hasDividers: true,
            dividerColor: widget.dividerColor,
            elevation: widget.elevation,
            children: items
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}
