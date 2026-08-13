// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../flutter-master/packages/flutter/lib/src/material/expansion_panel.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Material;

public static partial class Expansion_panelLibrary
{
    internal static double _kPanelHeaderCollapsedHeight = ConstantsLibrary.kMinInteractiveDimension;
}

public static partial class Expansion_panelLibrary
{
    internal static global::Doroti.Generated.Framework.Painting.EdgeInsets _kPanelHeaderExpandedDefaultPadding = global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateSymmetric(vertical: (64.0 - Expansion_panelLibrary._kPanelHeaderCollapsedHeight));
}

public static partial class Expansion_panelLibrary
{
    internal static global::Doroti.Generated.Framework.Painting.EdgeInsets _kExpandIconPadding = global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateAll(12.0);
}

internal class _SaltedKey__expansion_panel<S, V> : global::Doroti.Generated.Framework.Foundation.LocalKey
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
        if (__other is null) return false;
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((__other is _SaltedKey__expansion_panel<S, V>) && EqualityComparer<S>.Default.Equals(((_SaltedKey__expansion_panel<S, V>)((_SaltedKey__expansion_panel<S, V>)__other)).salt, this.salt)) && EqualityComparer<V>.Default.Equals(((_SaltedKey__expansion_panel<S, V>)((_SaltedKey__expansion_panel<S, V>)__other)).value, this.value));
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.GetType(), this.salt, this.value));
    public override string ToString()
    {
        var saltString__1177 = ((object.Equals(typeof(S), typeof(string))) ? $"<'{this.salt}'>" : $"<{this.salt}>");
        var valueString__1239 = ((object.Equals(typeof(V), typeof(string))) ? $"<'{this.value}'>" : $"<{this.value}>");
        return $"[{saltString__1177} {valueString__1239}]";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public delegate void ExpansionPanelCallback(long panelIndex, bool isExpanded);

public delegate global::Doroti.Generated.Framework.Widgets.Widget ExpansionPanelHeaderBuilder(global::Doroti.Generated.Framework.Widgets.BuildContext context, bool isExpanded);

public class ExpansionPanel
{
    public virtual global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, bool, global::Doroti.Generated.Framework.Widgets.Widget> headerBuilder { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.Widget body { get; private set; } = default!;
    public virtual bool isExpanded { get; private set; } = default!;
    public virtual Color? splashColor { get; private set; }
    public virtual Color? highlightColor { get; private set; }
    public virtual bool canTapOnHeader { get; private set; } = default!;
    public virtual Color? backgroundColor { get; private set; }

    public ExpansionPanel(global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, bool, global::Doroti.Generated.Framework.Widgets.Widget> headerBuilder, global::Doroti.Generated.Framework.Widgets.Widget body, bool isExpanded = false, bool canTapOnHeader = false, Color? backgroundColor = null, Color? splashColor = null, Color? highlightColor = null)
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

    public ExpansionPanelRadio(object value, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, bool, global::Doroti.Generated.Framework.Widgets.Widget> headerBuilder, global::Doroti.Generated.Framework.Widgets.Widget body, bool canTapOnHeader = false, Color? backgroundColor = null, Color? splashColor = null, Color? highlightColor = null) : base(headerBuilder: headerBuilder, body: body, canTapOnHeader: canTapOnHeader, backgroundColor: backgroundColor, splashColor: splashColor, highlightColor: highlightColor)
    {
        this.value = value;
    }

}

public class ExpansionPanelList : global::Doroti.Generated.Framework.Widgets.StatefulWidget
{
    public virtual List<ExpansionPanel> children { get; private set; } = default!;
    public virtual global::System.Action<long, bool>? expansionCallback { get; private set; }
    public virtual Duration animationDuration { get; private set; } = default!;
    internal virtual bool _allowOnlyOnePanelOpen { get; private set; } = default!;
    public virtual object? initialOpenPanelValue { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsets expandedHeaderPadding { get; private set; } = default!;
    public virtual Color? dividerColor { get; private set; }
    public virtual double elevation { get; private set; } = default!;
    public virtual Color? expandIconColor { get; private set; }
    public virtual double materialGapSize { get; private set; } = default!;

    public ExpansionPanelList(global::Doroti.Generated.Framework.Foundation.Key? key = null, List<ExpansionPanel> children = default!, global::System.Action<long, bool>? expansionCallback = null, Duration? animationDuration = null, global::Doroti.Generated.Framework.Painting.EdgeInsets expandedHeaderPadding = default!, Color? dividerColor = null, double elevation = 2, Color? expandIconColor = null, double materialGapSize = 16.0) : base(key: key)
    {
        List<ExpansionPanel> __children = children ?? new List<ExpansionPanel>();
        Duration __animationDuration = animationDuration ?? ThemeLibrary.kThemeAnimationDuration;
        global::Doroti.Generated.Framework.Painting.EdgeInsets __expandedHeaderPadding = expandedHeaderPadding ?? Expansion_panelLibrary._kPanelHeaderExpandedDefaultPadding;
        this.children = __children;
        this.expansionCallback = expansionCallback;
        this.animationDuration = __animationDuration;
        this.expandedHeaderPadding = __expandedHeaderPadding;
        this.dividerColor = dividerColor;
        this.elevation = elevation;
        this.expandIconColor = expandIconColor;
        this.materialGapSize = materialGapSize;
        this._allowOnlyOnePanelOpen = false;
        this.initialOpenPanelValue = null;
    }

    public static ExpansionPanelList CreateRadio(global::Doroti.Generated.Framework.Foundation.Key? key = null, List<ExpansionPanel> children = default!, global::System.Action<long, bool>? expansionCallback = null, Duration? animationDuration = null, object? initialOpenPanelValue = null, global::Doroti.Generated.Framework.Painting.EdgeInsets expandedHeaderPadding = default!, Color? dividerColor = null, double elevation = 2, Color? expandIconColor = null, double materialGapSize = 16.0)
    {
        var __instance = new ExpansionPanelList(key: key, children: children, expansionCallback: expansionCallback, animationDuration: animationDuration, expandedHeaderPadding: expandedHeaderPadding, dividerColor: dividerColor, elevation: elevation, expandIconColor: expandIconColor, materialGapSize: materialGapSize);
        List<ExpansionPanel> __children = children ?? new List<ExpansionPanel>();
        Duration __animationDuration = animationDuration ?? ThemeLibrary.kThemeAnimationDuration;
        global::Doroti.Generated.Framework.Painting.EdgeInsets __expandedHeaderPadding = expandedHeaderPadding ?? Expansion_panelLibrary._kPanelHeaderExpandedDefaultPadding;
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

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _ExpansionPanelListState__expansion_panel());
}

internal class _ExpansionPanelListState__expansion_panel : global::Doroti.Generated.Framework.Widgets.State<ExpansionPanelList>
{
    internal virtual ExpansionPanelRadio? _currentOpenPanel { get; set; } = default;

    public override void initState()
    {
        base.initState();
        if (((ExpansionPanelList)this.widget)._allowOnlyOnePanelOpen)
        {
            DartRuntimePrimitives.Assert(() => _allIdentifiersUnique(), () => (object?)"All ExpansionPanelRadio identifier values must be unique.");
            if ((((ExpansionPanelList)this.widget).initialOpenPanelValue is not null))
            {
                _currentOpenPanel = searchPanelByValue(((ExpansionPanelList)this.widget).children.cast<ExpansionPanelRadio>().ToList(), ((ExpansionPanelList)this.widget).initialOpenPanelValue);
            }
        }
    }

    public override void didUpdateWidget(ExpansionPanelList oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (((ExpansionPanelList)this.widget)._allowOnlyOnePanelOpen)
        {
            DartRuntimePrimitives.Assert(() => _allIdentifiersUnique(), () => (object?)"All ExpansionPanelRadio identifier values must be unique.");
            if (!((ExpansionPanelList)oldWidget)._allowOnlyOnePanelOpen)
            {
                _currentOpenPanel = searchPanelByValue(((ExpansionPanelList)this.widget).children.cast<ExpansionPanelRadio>().ToList(), ((ExpansionPanelList)this.widget).initialOpenPanelValue);
            }
        }
        else
        {
            _currentOpenPanel = null;
        }
    }

    internal virtual bool _allIdentifiersUnique()
    {
        var identifierMap__11853 = new DartMap<object, bool>();
        foreach (ExpansionPanelRadio child__11922 in ((ExpansionPanelList)this.widget).children.cast<ExpansionPanelRadio>())
        {
            identifierMap__11853[((ExpansionPanelRadio)child__11922).value] = true;
        }
        return (checked((long)(identifierMap__11853.Count)) == checked((long)(((ExpansionPanelList)this.widget).children.Count)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _isChildExpanded(long index)
    {
        if (((ExpansionPanelList)this.widget)._allowOnlyOnePanelOpen)
        {
            var radioWidget__12179 = ((ExpansionPanelRadio?)(object?)((ExpansionPanelList)this.widget).children[(int)(index)])!;
            return (object.Equals(this._currentOpenPanel?.value, ((ExpansionPanelRadio)radioWidget__12179).value));
        }
        return ((ExpansionPanelList)this.widget).children[(int)(index)].isExpanded;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _handlePressed(bool isExpanded, long index)
    {
        if (((ExpansionPanelList)this.widget)._allowOnlyOnePanelOpen)
        {
            var pressedChild__12462 = ((ExpansionPanelRadio?)(object?)((ExpansionPanelList)this.widget).children[(int)(index)])!;
            for (var childIndex__12676 = 0L; (childIndex__12676 < checked((long)(((ExpansionPanelList)this.widget).children.Count))); childIndex__12676 += 1L)
            {
                var child__12762 = ((ExpansionPanelRadio?)(object?)((ExpansionPanelList)this.widget).children[(int)(childIndex__12676)])!;
                if ((((((ExpansionPanelList)this.widget).expansionCallback is not null) && (childIndex__12676 != index)) && (object.Equals(((ExpansionPanelRadio)child__12762).value, this._currentOpenPanel?.value))))
                {
                    ((ExpansionPanelList)this.widget).expansionCallback!(childIndex__12676, false);
                }
            }
            setState(((global::System.Action)(() => {
_currentOpenPanel = (isExpanded ? null : pressedChild__12462);
})));
        }
        ((ExpansionPanelList)this.widget).expansionCallback?.Invoke(index, !isExpanded);
    }

    public virtual ExpansionPanelRadio? searchPanelByValue(List<ExpansionPanelRadio> panels, object? value)
    {
        foreach (var panel__13415 in panels)
        {
            if ((object.Equals(((ExpansionPanelRadio)panel__13415).value, value)))
            {
                return panel__13415;
            }
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => ShadowsLibrary.kElevationToShadow.ContainsKey(checked((long)((ExpansionPanelList)this.widget).elevation)), () => (object?)"Invalid value for elevation. See the kElevationToShadow constant for" + " possible elevation values.");
        var items__13777 = new List<MergeableMaterialItem>();
        for (var index__13826 = 0L; (index__13826 < checked((long)(((ExpansionPanelList)this.widget).children.Count))); index__13826 += 1L)
        {
            if (((_isChildExpanded(index__13826) && (index__13826 != 0L)) && !_isChildExpanded((index__13826 - 1L))))
            {
                items__13777.Add(new MaterialGap(key: new _SaltedKey__expansion_panel<global::Doroti.Generated.Framework.Widgets.BuildContext, long>(context, ((index__13826 * 2L) - 1L)), size: ((ExpansionPanelList)this.widget).materialGapSize));
            }
            ExpansionPanel child__14182 = ((ExpansionPanelList)this.widget).children[(int)(index__13826)];
            global::Doroti.Generated.Framework.Widgets.Widget headerWidget__14233 = child__14182.headerBuilder(context, _isChildExpanded(index__13826));
            global::Doroti.Generated.Framework.Widgets.Widget expandIconPadded__14317 = ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Padding(padding: global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional.CreateOnly(end: 8.0), child: new global::Doroti.Generated.Framework.Widgets.IgnorePointer(ignoring: ((ExpansionPanel)child__14182).canTapOnHeader, child: new ExpandIcon(color: ((ExpansionPanelList)this.widget).expandIconColor, isExpanded: _isChildExpanded(index__13826), padding: Expansion_panelLibrary._kExpandIconPadding, splashColor: ((ExpansionPanel)child__14182).splashColor, highlightColor: ((ExpansionPanel)child__14182).highlightColor, onPressed: ((global::System.Action<bool>)((isExpanded) => { _handlePressed(isExpanded, index__13826); }))))));
            if (!((ExpansionPanel)child__14182).canTapOnHeader)
            {
                MaterialLocalizations localizations__14919 = ((MaterialLocalizations)(object?)MaterialLocalizations.of(context));
                expandIconPadded__14317 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Semantics(label: (_isChildExpanded(index__13826) ? ((MaterialLocalizations)localizations__14919).expandedIconTapHint : ((MaterialLocalizations)localizations__14919).collapsedIconTapHint), container: true, child: expandIconPadded__14317));
            }
            global::Doroti.Generated.Framework.Widgets.Widget header__15245 = ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Row(children: new List<global::Doroti.Generated.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Expanded(child: new global::Doroti.Generated.Framework.Widgets.AnimatedContainer(duration: ((ExpansionPanelList)this.widget).animationDuration, curve: global::Doroti.Generated.Framework.Animation.Curves.fastOutSlowIn, margin: (_isChildExpanded(index__13826) ? ((ExpansionPanelList)this.widget).expandedHeaderPadding : global::Doroti.Generated.Framework.Painting.EdgeInsets.zero), child: new global::Doroti.Generated.Framework.Widgets.ConstrainedBox(constraints: new global::Doroti.Generated.Framework.Rendering.BoxConstraints(minHeight: Expansion_panelLibrary._kPanelHeaderCollapsedHeight), child: headerWidget__14233)))), DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(expandIconPadded__14317) }));
            if (((ExpansionPanel)child__14182).canTapOnHeader)
            {
                header__15245 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.MergeSemantics(child: new InkWell(splashColor: ((ExpansionPanel)child__14182).splashColor, highlightColor: ((ExpansionPanel)child__14182).highlightColor, onTap: (() => { _handlePressed(_isChildExpanded(index__13826), index__13826); }), child: header__15245)));
            }
            items__13777.Add(new MaterialSlice(key: new _SaltedKey__expansion_panel<global::Doroti.Generated.Framework.Widgets.BuildContext, long>(context, (index__13826 * 2L)), color: ((ExpansionPanel)child__14182).backgroundColor, child: new global::Doroti.Generated.Framework.Widgets.Column(children: new List<global::Doroti.Generated.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(header__15245), DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.AnimatedCrossFade(firstChild: new global::Doroti.Generated.Framework.Widgets.LimitedBox(maxWidth: 0.0, child: new global::Doroti.Generated.Framework.Widgets.SizedBox(width: double.PositiveInfinity, height: 0)), secondChild: ((ExpansionPanel)child__14182).body, firstCurve: new global::Doroti.Generated.Framework.Animation.Interval(0.0, 0.6, curve: global::Doroti.Generated.Framework.Animation.Curves.fastOutSlowIn), secondCurve: new global::Doroti.Generated.Framework.Animation.Interval(0.4, 1.0, curve: global::Doroti.Generated.Framework.Animation.Curves.fastOutSlowIn), sizeCurve: global::Doroti.Generated.Framework.Animation.Curves.fastOutSlowIn, crossFadeState: (_isChildExpanded(index__13826) ? global::Doroti.Generated.Framework.Widgets.CrossFadeState.showSecond : global::Doroti.Generated.Framework.Widgets.CrossFadeState.showFirst), duration: ((ExpansionPanelList)this.widget).animationDuration)) })));
            if ((_isChildExpanded(index__13826) && (index__13826 != (checked((long)(((ExpansionPanelList)this.widget).children.Count)) - 1L))))
            {
                items__13777.Add(new MaterialGap(key: new _SaltedKey__expansion_panel<global::Doroti.Generated.Framework.Widgets.BuildContext, long>(context, ((index__13826 * 2L) + 1L)), size: ((ExpansionPanelList)this.widget).materialGapSize));
            }
        }
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new MergeableMaterial(hasDividers: true, dividerColor: ((ExpansionPanelList)this.widget).dividerColor, elevation: ((ExpansionPanelList)this.widget).elevation, children: items__13777));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
