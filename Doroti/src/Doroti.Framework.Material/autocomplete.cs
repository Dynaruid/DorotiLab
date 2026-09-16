// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/autocomplete.dart

using Doroti.Runtime;

namespace Doroti.Framework.Material;

public class Autocomplete<T> : global::Doroti.Framework.Widgets.StatelessWidget where T : notnull
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

    public Autocomplete(global::Doroti.Framework.Foundation.Key? key = null, global::System.Func<global::Doroti.Framework.Services.TextEditingValue, object> optionsBuilder = default!, global::System.Func<T, string> displayStringForOption = default!, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.TextEditingController, global::Doroti.Framework.Widgets.FocusNode, global::System.Action, global::Doroti.Framework.Widgets.Widget> fieldViewBuilder = default!, global::Doroti.Framework.Widgets.FocusNode? focusNode = null, global::System.Action<T>? onSelected = null, double optionsMaxHeight = 200.0, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::System.Action<T>, IEnumerable<T>, global::Doroti.Framework.Widgets.Widget>? optionsViewBuilder = null, global::Doroti.Framework.Widgets.OptionsViewOpenDirection optionsViewOpenDirection = OptionsViewOpenDirection.down, global::Doroti.Framework.Widgets.TextEditingController? textEditingController = null, global::Doroti.Framework.Services.TextEditingValue? initialValue = null) : base(key: key)
    {
        global::System.Func<T, string> __displayStringForOption = displayStringForOption ?? (__option => RawAutocomplete<T>.defaultStringForOption(__option));
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
        return new _AutocompleteField__autocomplete(focusNode: focusNode, textEditingController: textEditingController, onFieldSubmitted: () => onFieldSubmitted());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return new global::Doroti.Framework.Widgets.RawAutocomplete<T>(displayStringForOption: displayStringForOption, fieldViewBuilder: fieldViewBuilder, focusNode: focusNode, textEditingController: textEditingController, initialValue: initialValue, optionsBuilder: optionsBuilder, optionsViewOpenDirection: optionsViewOpenDirection, optionsViewBuilder: optionsViewBuilder ?? ((context, onSelected, options) =>
        {
            return new _AutocompleteOptions__autocomplete<T>(displayStringForOption: displayStringForOption, onSelected: onSelected, options: options.Cast<T>(), openDirection: optionsViewOpenDirection, optionsMaxHeight: optionsMaxHeight);
            throw new InvalidOperationException("Dart closure completed without a value.");
        }), onSelected: onSelected);
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
        return new TextFormField(controller: textEditingController, focusNode: focusNode, onFieldSubmitted: (value) =>
        {
            onFieldSubmitted();
        });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _AutocompleteOptions__autocomplete<T> : global::Doroti.Framework.Widgets.StatelessWidget where T : notnull
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
        long highlightedIndexLocal = AutocompleteHighlightedOption.of(context);
        return new Material(elevation: 4.0, child: new global::Doroti.Framework.Widgets.ConstrainedBox(constraints: new global::Doroti.Framework.Rendering.BoxConstraints(maxHeight: optionsMaxHeight), child: new _AutocompleteOptionsList__autocomplete<T>(displayStringForOption: displayStringForOption, highlightedIndex: highlightedIndexLocal, onSelected: onSelected, options: options.Cast<T>())));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _AutocompleteOptionsList__autocomplete<T> : global::Doroti.Framework.Widgets.StatefulWidget where T : notnull
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

internal class _AutocompleteOptionsListState__autocomplete<T> : global::Doroti.Framework.Widgets.State<_AutocompleteOptionsList__autocomplete<T>> where T : notnull
{
    internal virtual global::Doroti.Framework.Widgets.ScrollController _scrollController { get; private set; } = new global::Doroti.Framework.Widgets.ScrollController();

    public override void didUpdateWidget(_AutocompleteOptionsList__autocomplete<T> oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (widget.highlightedIndex != oldWidget.highlightedIndex)
        {
            Scheduler.SchedulerBinding.instance.addPostFrameCallback((timeStamp) =>
            {
                if (!mounted)
                {
                    return;
                }
                global::Doroti.Framework.Widgets.BuildContext? highlightedContext = new global::Doroti.Framework.Widgets.GlobalObjectKey<IState>(widget.options.elementAt(widget.highlightedIndex)).currentContext;
                if (highlightedContext is null)
                {
                    _scrollController.jumpTo((widget.highlightedIndex == 0L) ? 0.0 : _scrollController.position.maxScrollExtent);
                }
                else
                {
                    DartRuntimePrimitives.Ignore(Scrollable.ensureVisible(highlightedContext, alignment: 0.5));
                }
            }, debugLabel: "AutocompleteOptions.ensureVisible");
        }
    }

    public override void dispose()
    {
        _scrollController.dispose();
        base.dispose();
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        long highlightedIndex = AutocompleteHighlightedOption.of(context);
        return ListView.CreateBuilder(padding: EdgeInsets.zero, shrinkWrap: true, controller: _scrollController, itemCount: widget.options.Count(), itemBuilder: (context, index) =>
        {
            T option = widget.options.elementAt(index);
            return new global::Doroti.Framework.Widgets.Semantics(button: true, child: new InkWell(key: new global::Doroti.Framework.Widgets.GlobalObjectKey<IState>(option), onTap: () =>
            {
                widget.onSelected(option);
            }, child: new global::Doroti.Framework.Widgets.Builder(builder: (context) =>
            {
                var highlight = highlightedIndex == index;
                return new global::Doroti.Framework.Widgets.Container(color: highlight ? Theme.of(context).focusColor : null, padding: EdgeInsets.CreateAll(16.0), child: new global::Doroti.Framework.Widgets.Text(widget.displayStringForOption(option)));
                throw new InvalidOperationException("Dart closure completed without a value.");
            })));
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
