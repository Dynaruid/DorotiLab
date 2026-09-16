// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/autocomplete.dart

using Doroti.Runtime;

namespace Doroti.Framework.Material;

public class Autocomplete<T> : StatelessWidget where T : notnull
{
    public virtual Func<T, string> displayStringForOption { get; private set; } = default!;
    public virtual Func<BuildContext, TextEditingController, FocusNode, Action, Widget> fieldViewBuilder { get; private set; } = default!;
    public virtual FocusNode? focusNode { get; private set; }
    public virtual System.Action<T>? onSelected { get; private set; }
    public virtual Func<TextEditingValue, object> optionsBuilder { get; private set; } = default!;
    public virtual Func<BuildContext, System.Action<T>, IEnumerable<T>, Widget>? optionsViewBuilder { get; private set; }
    public virtual OptionsViewOpenDirection optionsViewOpenDirection { get; private set; } = default!;
    public virtual double optionsMaxHeight { get; private set; } = default!;
    public virtual TextEditingController? textEditingController { get; private set; }
    public virtual TextEditingValue? initialValue { get; private set; }

    public Autocomplete(Key? key = null, Func<TextEditingValue, object> optionsBuilder = default!, Func<T, string> displayStringForOption = default!, Func<BuildContext, TextEditingController, FocusNode, Action, Widget> fieldViewBuilder = default!, FocusNode? focusNode = null, System.Action<T>? onSelected = null, double optionsMaxHeight = 200.0, Func<BuildContext, System.Action<T>, IEnumerable<T>, Widget>? optionsViewBuilder = null, OptionsViewOpenDirection optionsViewOpenDirection = OptionsViewOpenDirection.down, TextEditingController? textEditingController = null, TextEditingValue? initialValue = null) : base(key: key)
    {
        Func<T, string> __displayStringForOption = displayStringForOption ?? (__option => RawAutocomplete<T>.defaultStringForOption(__option));
        Func<BuildContext, TextEditingController, FocusNode, Action, Widget> __fieldViewBuilder = fieldViewBuilder ?? _defaultFieldViewBuilder;
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

    internal static Widget _defaultFieldViewBuilder(BuildContext context, TextEditingController textEditingController, FocusNode focusNode, Action onFieldSubmitted)
    {
        return new _AutocompleteField__autocomplete(focusNode: focusNode, textEditingController: textEditingController, onFieldSubmitted: () => onFieldSubmitted());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        return new RawAutocomplete<T>(displayStringForOption: displayStringForOption, fieldViewBuilder: fieldViewBuilder, focusNode: focusNode, textEditingController: textEditingController, initialValue: initialValue, optionsBuilder: optionsBuilder, optionsViewOpenDirection: optionsViewOpenDirection, optionsViewBuilder: optionsViewBuilder ?? ((context, onSelected, options) =>
        {
            return new _AutocompleteOptions__autocomplete<T>(displayStringForOption: displayStringForOption, onSelected: onSelected, options: options.Cast<T>(), openDirection: optionsViewOpenDirection, optionsMaxHeight: optionsMaxHeight);
            throw new InvalidOperationException("Dart closure completed without a value.");
        }), onSelected: onSelected);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _AutocompleteField__autocomplete : StatelessWidget
{
    public virtual FocusNode focusNode { get; private set; } = default!;
    public virtual Action onFieldSubmitted { get; private set; } = default!;
    public virtual TextEditingController textEditingController { get; private set; } = default!;

    internal _AutocompleteField__autocomplete(FocusNode focusNode, TextEditingController textEditingController, Action onFieldSubmitted)
    {
        this.focusNode = focusNode;
        this.textEditingController = textEditingController;
        this.onFieldSubmitted = onFieldSubmitted;
    }

    public override Widget build(BuildContext context)
    {
        return new TextFormField(controller: textEditingController, focusNode: focusNode, onFieldSubmitted: (value) =>
        {
            onFieldSubmitted();
        });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _AutocompleteOptions__autocomplete<T> : StatelessWidget where T : notnull
{
    public virtual Func<T, string> displayStringForOption { get; private set; } = default!;
    public virtual System.Action<T> onSelected { get; private set; } = default!;
    public virtual OptionsViewOpenDirection openDirection { get; private set; } = default!;
    public virtual IEnumerable<T> options { get; private set; } = default!;
    public virtual double optionsMaxHeight { get; private set; } = default!;

    internal _AutocompleteOptions__autocomplete(Key? key = null, Func<T, string> displayStringForOption = default!, System.Action<T> onSelected = default!, OptionsViewOpenDirection openDirection = default!, IEnumerable<T> options = default!, double optionsMaxHeight = default!) : base(key: key)
    {
        this.displayStringForOption = displayStringForOption;
        this.onSelected = onSelected;
        this.openDirection = openDirection;
        this.options = options;
        this.optionsMaxHeight = optionsMaxHeight;
    }

    public override Widget build(BuildContext context)
    {
        long highlightedIndexLocal = AutocompleteHighlightedOption.of(context);
        return new Material(elevation: 4.0, child: new ConstrainedBox(constraints: new BoxConstraints(maxHeight: optionsMaxHeight), child: new _AutocompleteOptionsList__autocomplete<T>(displayStringForOption: displayStringForOption, highlightedIndex: highlightedIndexLocal, onSelected: onSelected, options: options.Cast<T>())));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _AutocompleteOptionsList__autocomplete<T> : StatefulWidget where T : notnull
{
    public virtual Func<T, string> displayStringForOption { get; private set; } = default!;
    public virtual long highlightedIndex { get; private set; } = default!;
    public virtual System.Action<T> onSelected { get; private set; } = default!;
    public virtual IEnumerable<T> options { get; private set; } = default!;

    internal _AutocompleteOptionsList__autocomplete(Func<T, string> displayStringForOption, long highlightedIndex, System.Action<T> onSelected, IEnumerable<T> options)
    {
        this.displayStringForOption = displayStringForOption;
        this.highlightedIndex = highlightedIndex;
        this.onSelected = onSelected;
        this.options = options;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _AutocompleteOptionsListState__autocomplete<T>());
}

internal class _AutocompleteOptionsListState__autocomplete<T> : State<_AutocompleteOptionsList__autocomplete<T>> where T : notnull
{
    internal virtual ScrollController _scrollController { get; private set; } = new ScrollController();

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
                BuildContext? highlightedContext = new GlobalObjectKey<IState>(widget.options.elementAt(widget.highlightedIndex)).currentContext;
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

    public override Widget build(BuildContext context)
    {
        long highlightedIndex = AutocompleteHighlightedOption.of(context);
        return ListView.CreateBuilder(padding: EdgeInsets.zero, shrinkWrap: true, controller: _scrollController, itemCount: widget.options.Count(), itemBuilder: (context, index) =>
        {
            T option = widget.options.elementAt(index);
            return new Widgets.Semantics(button: true, child: new InkWell(key: new GlobalObjectKey<IState>(option), onTap: () =>
            {
                widget.onSelected(option);
            }, child: new Builder(builder: (context) =>
            {
                var highlight = highlightedIndex == index;
                return new Container(color: highlight ? Theme.of(context).focusColor : null, padding: EdgeInsets.CreateAll(16.0), child: new Text(widget.displayStringForOption(option)));
                throw new InvalidOperationException("Dart closure completed without a value.");
            })));
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
