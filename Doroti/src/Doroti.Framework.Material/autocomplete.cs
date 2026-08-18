// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/autocomplete.dart
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

public class Autocomplete<T> : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual global::System.Func<T, string> displayStringForOption { get; private set; } = default!;
    public virtual global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.TextEditingController, global::Doroti.Framework.Widgets.FocusNode, global::System.Action, global::Doroti.Framework.Widgets.Widget> fieldViewBuilder { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.FocusNode? focusNode { get; private set; }
    public virtual global::System.Action<T>? onSelected { get; private set; }
    public virtual global::System.Func<global::Doroti.Framework.Services.TextEditingValue, object> optionsBuilder { get; private set; } = default!;
    public virtual global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::System.Action<T>, IEnumerable<T>, global::Doroti.Framework.Widgets.Widget>? optionsViewBuilder { get; private set; }
    public virtual global::Doroti.Framework.Widgets.OptionsViewOpenDirection optionsViewOpenDirection { get; private set; } = default!;
    public virtual double optionsMaxHeight { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.TextEditingController? textEditingController { get; private set; }
    public virtual global::Doroti.Framework.Services.TextEditingValue? initialValue { get; private set; }

    public Autocomplete(global::Doroti.Framework.Foundation.Key? key = null, global::System.Func<global::Doroti.Framework.Services.TextEditingValue, object> optionsBuilder = default!, global::System.Func<T, string> displayStringForOption = default!, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.TextEditingController, global::Doroti.Framework.Widgets.FocusNode, global::System.Action, global::Doroti.Framework.Widgets.Widget> fieldViewBuilder = default!, global::Doroti.Framework.Widgets.FocusNode? focusNode = null, global::System.Action<T>? onSelected = null, double optionsMaxHeight = 200.0, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::System.Action<T>, IEnumerable<T>, global::Doroti.Framework.Widgets.Widget>? optionsViewBuilder = null, global::Doroti.Framework.Widgets.OptionsViewOpenDirection optionsViewOpenDirection = global::Doroti.Framework.Widgets.OptionsViewOpenDirection.down, global::Doroti.Framework.Widgets.TextEditingController? textEditingController = null, global::Doroti.Framework.Services.TextEditingValue? initialValue = null) : base(key: key)
    {
        global::System.Func<T, string> __displayStringForOption = displayStringForOption ?? (__option => global::Doroti.Framework.Widgets.RawAutocomplete<T>.defaultStringForOption(__option));
        global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.TextEditingController, global::Doroti.Framework.Widgets.FocusNode, global::System.Action, global::Doroti.Framework.Widgets.Widget> __fieldViewBuilder = fieldViewBuilder ?? _defaultFieldViewBuilder;
        this.optionsBuilder = optionsBuilder;
        this.displayStringForOption = __displayStringForOption;
        this.fieldViewBuilder = __fieldViewBuilder;
        this.focusNode = focusNode;
        this.onSelected = onSelected;
        this.optionsMaxHeight = optionsMaxHeight;
        this.optionsViewBuilder = optionsViewBuilder;
        this.optionsViewOpenDirection = optionsViewOpenDirection;
        this.textEditingController = textEditingController;
        this.initialValue = initialValue;
    }

    internal static global::Doroti.Framework.Widgets.Widget _defaultFieldViewBuilder(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.TextEditingController textEditingController, global::Doroti.Framework.Widgets.FocusNode focusNode, global::System.Action onFieldSubmitted)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new _AutocompleteField__autocomplete(focusNode: focusNode, textEditingController: textEditingController, onFieldSubmitted: () => onFieldSubmitted()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.RawAutocomplete<T>(displayStringForOption: (global::System.Func<T, string>)this.displayStringForOption, fieldViewBuilder: (global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.TextEditingController, global::Doroti.Framework.Widgets.FocusNode, global::System.Action, global::Doroti.Framework.Widgets.Widget>)this.fieldViewBuilder, focusNode: this.focusNode, textEditingController: this.textEditingController, initialValue: this.initialValue, optionsBuilder: (global::System.Func<global::Doroti.Framework.Services.TextEditingValue, object>)this.optionsBuilder, optionsViewOpenDirection: this.optionsViewOpenDirection, optionsViewBuilder: ((this.optionsViewBuilder ?? (global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::System.Action<T>, IEnumerable<T>, global::Doroti.Framework.Widgets.Widget>)((context, onSelected, options) =>
        {
            return new _AutocompleteOptions__autocomplete<T>(displayStringForOption: (global::System.Func<T, string>)this.displayStringForOption, onSelected: (global::System.Action<T>)onSelected, options: options.Cast<T>(), openDirection: this.optionsViewOpenDirection, optionsMaxHeight: this.optionsMaxHeight);
            throw new InvalidOperationException("Dart closure completed without a value.");
        }))), onSelected: (global::System.Action<T>?)this.onSelected));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _AutocompleteField__autocomplete : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual global::Doroti.Framework.Widgets.FocusNode focusNode { get; private set; } = default!;
    public virtual global::System.Action onFieldSubmitted { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.TextEditingController textEditingController { get; private set; } = default!;

    internal _AutocompleteField__autocomplete(global::Doroti.Framework.Widgets.FocusNode focusNode, global::Doroti.Framework.Widgets.TextEditingController textEditingController, global::System.Action onFieldSubmitted)
    {
        this.focusNode = focusNode;
        this.textEditingController = textEditingController;
        this.onFieldSubmitted = onFieldSubmitted;
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new TextFormField(controller: this.textEditingController, focusNode: this.focusNode, onFieldSubmitted: ((value) =>
        {
            this.onFieldSubmitted();
        })));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _AutocompleteOptions__autocomplete<T> : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual global::System.Func<T, string> displayStringForOption { get; private set; } = default!;
    public virtual global::System.Action<T> onSelected { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.OptionsViewOpenDirection openDirection { get; private set; } = default!;
    public virtual IEnumerable<T> options { get; private set; } = default!;
    public virtual double optionsMaxHeight { get; private set; } = default!;

    internal _AutocompleteOptions__autocomplete(global::Doroti.Framework.Foundation.Key? key = null, global::System.Func<T, string> displayStringForOption = default!, global::System.Action<T> onSelected = default!, global::Doroti.Framework.Widgets.OptionsViewOpenDirection openDirection = default!, IEnumerable<T> options = default!, double optionsMaxHeight = default!) : base(key: key)
    {
        this.displayStringForOption = displayStringForOption;
        this.onSelected = onSelected;
        this.openDirection = openDirection;
        this.options = options;
        this.optionsMaxHeight = optionsMaxHeight;
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        long highlightedIndex__6958 = AutocompleteHighlightedOption.of(context);
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new Material(elevation: 4.0, child: new global::Doroti.Framework.Widgets.ConstrainedBox(constraints: new global::Doroti.Framework.Rendering.BoxConstraints(maxHeight: this.optionsMaxHeight), child: new _AutocompleteOptionsList__autocomplete<T>(displayStringForOption: (global::System.Func<T, string>)this.displayStringForOption, highlightedIndex: highlightedIndex__6958, onSelected: (global::System.Action<T>)this.onSelected, options: this.options.Cast<T>()))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _AutocompleteOptionsList__autocomplete<T> : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual global::System.Func<T, string> displayStringForOption { get; private set; } = default!;
    public virtual long highlightedIndex { get; private set; } = default!;
    public virtual global::System.Action<T> onSelected { get; private set; } = default!;
    public virtual IEnumerable<T> options { get; private set; } = default!;

    internal _AutocompleteOptionsList__autocomplete(global::System.Func<T, string> displayStringForOption, long highlightedIndex, global::System.Action<T> onSelected, IEnumerable<T> options)
    {
        this.displayStringForOption = displayStringForOption;
        this.highlightedIndex = highlightedIndex;
        this.onSelected = onSelected;
        this.options = options;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _AutocompleteOptionsListState__autocomplete<T>());
}

internal class _AutocompleteOptionsListState__autocomplete<T> : global::Doroti.Framework.Widgets.State<_AutocompleteOptionsList__autocomplete<T>>
{
    internal virtual global::Doroti.Framework.Widgets.ScrollController _scrollController { get; private set; } = new global::Doroti.Framework.Widgets.ScrollController();

    public override void didUpdateWidget(_AutocompleteOptionsList__autocomplete<T> oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((((_AutocompleteOptionsList__autocomplete<T>)(object)this.widget).highlightedIndex != ((_AutocompleteOptionsList__autocomplete<T>)oldWidget).highlightedIndex))
        {
            global::Doroti.Framework.Scheduler.SchedulerBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((timeStamp) =>
            {
                if (!this.mounted)
                {
                    return;
                }
                global::Doroti.Framework.Widgets.BuildContext? highlightedContext__8428 = new global::Doroti.Framework.Widgets.GlobalObjectKey<IState>(((_AutocompleteOptionsList__autocomplete<T>)(object)this.widget).options.elementAt(((_AutocompleteOptionsList__autocomplete<T>)(object)this.widget).highlightedIndex)).currentContext;
                if ((highlightedContext__8428 is null))
                {
                    this._scrollController.jumpTo(((((_AutocompleteOptionsList__autocomplete<T>)(object)this.widget).highlightedIndex == 0L) ? 0.0 : ((global::Doroti.Framework.Widgets.ScrollController)this._scrollController).position.maxScrollExtent));
                }
                else
                {
                    DartRuntimePrimitives.Ignore(Scrollable.ensureVisible(highlightedContext__8428, alignment: 0.5));
                }
            })), debugLabel: "AutocompleteOptions.ensureVisible");
        }
    }

    public override void dispose()
    {
        this._scrollController.dispose();
        base.dispose();
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        long highlightedIndex__9061 = AutocompleteHighlightedOption.of(context);
        return ((global::Doroti.Framework.Widgets.Widget)(object?)global::Doroti.Framework.Widgets.ListView.CreateBuilder(padding: global::Doroti.Framework.Painting.EdgeInsets.zero, shrinkWrap: true, controller: this._scrollController, itemCount: ((_AutocompleteOptionsList__autocomplete<T>)(object)this.widget).options.Count(), itemBuilder: ((context, index) =>
        {
            T option__9357 = ((_AutocompleteOptionsList__autocomplete<T>)(object)this.widget).options.elementAt(index);
            return new global::Doroti.Framework.Widgets.Semantics(button: true, child: new InkWell(key: new global::Doroti.Framework.Widgets.GlobalObjectKey<IState>(option__9357), onTap: (() =>
            {
                this.widget.onSelected(option__9357);
            }), child: new global::Doroti.Framework.Widgets.Builder(builder: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget>)((context) =>
            {
                var highlight__9695 = (highlightedIndex__9061 == index);
                return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Container(color: (highlight__9695 ? Theme.of(context).focusColor : null), padding: global::Doroti.Framework.Painting.EdgeInsets.CreateAll(16.0), child: new global::Doroti.Framework.Widgets.Text(this.widget.displayStringForOption(option__9357))));
                throw new InvalidOperationException("Dart closure completed without a value.");
            })))));
            throw new InvalidOperationException("Dart closure completed without a value.");
        })));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
