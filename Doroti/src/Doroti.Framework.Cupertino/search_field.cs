// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/search_field.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Cupertino;

public static partial class Search_fieldLibrary
{
    internal static double _kMinHeightBeforeTotalTransparency = 4L / 5L;
}

public static partial class Search_fieldLibrary
{
    internal static double _kMaxPrefixIconSize = 30.0;
}

public class CupertinoSearchTextField : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual global::Doroti.Framework.Widgets.TextEditingController? controller { get; private set; }
    public virtual global::System.Action<string>? onChanged { get; private set; }
    public virtual global::System.Action<string>? onSubmitted { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? style { get; private set; }
    public virtual string? placeholder { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? placeholderStyle { get; private set; }
    public virtual global::Doroti.Framework.Painting.BoxDecoration? decoration { get; private set; }
    public virtual Color? backgroundColor { get; private set; }
    public virtual global::Doroti.Framework.Painting.BorderRadius? borderRadius { get; private set; }
    public virtual global::Doroti.Framework.Services.TextInputType? keyboardType { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry padding { get; private set; } = default!;
    public virtual Color itemColor { get; private set; } = default!;
    public virtual double itemSize { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry prefixInsets { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget prefixIcon { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry suffixInsets { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Icon suffixIcon { get; private set; } = default!;
    public virtual OverlayVisibilityMode suffixMode { get; private set; } = default!;
    public virtual global::System.Action? onSuffixTap { get; private set; }
    public virtual string? restorationId { get; private set; }
    public virtual global::Doroti.Framework.Widgets.FocusNode? focusNode { get; private set; }
    public virtual bool autofocus { get; private set; } = default!;
    public virtual global::System.Action? onTap { get; private set; }
    public virtual bool autocorrect { get; private set; } = default!;
    public virtual global::Doroti.Framework.Services.SmartQuotesType? smartQuotesType { get; private set; }
    public virtual global::Doroti.Framework.Services.SmartDashesType? smartDashesType { get; private set; }
    public virtual bool enableIMEPersonalizedLearning { get; private set; } = default!;
    public virtual bool? enabled { get; private set; }
    public virtual double cursorWidth { get; private set; } = default!;
    public virtual double? cursorHeight { get; private set; }
    public virtual Radius cursorRadius { get; private set; } = default!;
    public virtual bool cursorOpacityAnimates { get; private set; } = default!;
    public virtual Color? cursorColor { get; private set; }

    public CupertinoSearchTextField(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Widgets.TextEditingController? controller = null, global::System.Action<string>? onChanged = null, global::System.Action<string>? onSubmitted = null, global::Doroti.Framework.Painting.TextStyle? style = null, string? placeholder = null, global::Doroti.Framework.Painting.TextStyle? placeholderStyle = null, global::Doroti.Framework.Painting.BoxDecoration? decoration = null, Color? backgroundColor = null, global::Doroti.Framework.Painting.BorderRadius? borderRadius = null, global::Doroti.Framework.Services.TextInputType? keyboardType = default!, global::Doroti.Framework.Painting.EdgeInsetsGeometry padding = default!, Color itemColor = default!, double itemSize = 20.0, global::Doroti.Framework.Painting.EdgeInsetsGeometry prefixInsets = default!, global::Doroti.Framework.Widgets.Widget prefixIcon = default!, global::Doroti.Framework.Painting.EdgeInsetsGeometry suffixInsets = default!, global::Doroti.Framework.Widgets.Icon suffixIcon = default!, OverlayVisibilityMode suffixMode = OverlayVisibilityMode.editing, global::System.Action? onSuffixTap = null, string? restorationId = null, global::Doroti.Framework.Widgets.FocusNode? focusNode = null, global::Doroti.Framework.Services.SmartQuotesType? smartQuotesType = null, global::Doroti.Framework.Services.SmartDashesType? smartDashesType = null, bool enableIMEPersonalizedLearning = true, bool autofocus = false, global::System.Action? onTap = null, bool autocorrect = true, bool? enabled = null, double cursorWidth = 2.0, double? cursorHeight = null, Radius? cursorRadius = null, bool cursorOpacityAnimates = true, Color? cursorColor = null) : base(key: key)
    {
        global::Doroti.Framework.Services.TextInputType? __keyboardType = keyboardType ?? TextInputType.text;
        global::Doroti.Framework.Painting.EdgeInsetsGeometry __padding = padding ?? EdgeInsetsGeometry.CreateFromSTEB(5.5, 8, 5.5, 8);
        Color __itemColor = itemColor ?? CupertinoColors.secondaryLabel;
        global::Doroti.Framework.Painting.EdgeInsetsGeometry __prefixInsets = prefixInsets ?? EdgeInsetsGeometry.CreateFromSTEB(6, 8, 0, 8);
        global::Doroti.Framework.Widgets.Widget __prefixIcon = prefixIcon ?? new global::Doroti.Framework.Widgets.Icon(CupertinoIcons.search);
        global::Doroti.Framework.Painting.EdgeInsetsGeometry __suffixInsets = suffixInsets ?? EdgeInsetsGeometry.CreateFromSTEB(0, 8, 5, 8);
        global::Doroti.Framework.Widgets.Icon __suffixIcon = suffixIcon ?? new global::Doroti.Framework.Widgets.Icon(CupertinoIcons.xmark_circle_fill);
        Radius __cursorRadius = cursorRadius ?? Radius.CreateCircular(2.0);
        this.controller = controller;
        this.onChanged = onChanged;
        this.onSubmitted = onSubmitted;
        this.style = style;
        this.placeholder = placeholder;
        this.placeholderStyle = placeholderStyle;
        this.decoration = decoration;
        this.backgroundColor = backgroundColor;
        this.borderRadius = borderRadius;
        this.keyboardType = __keyboardType;
        this.padding = __padding;
        this.itemColor = __itemColor;
        this.itemSize = itemSize;
        this.prefixInsets = __prefixInsets;
        this.prefixIcon = __prefixIcon;
        this.suffixInsets = __suffixInsets;
        this.suffixIcon = __suffixIcon;
        this.suffixMode = suffixMode;
        this.onSuffixTap = onSuffixTap;
        this.restorationId = restorationId;
        this.focusNode = focusNode;
        this.smartQuotesType = smartQuotesType;
        this.smartDashesType = smartDashesType;
        this.enableIMEPersonalizedLearning = enableIMEPersonalizedLearning;
        this.autofocus = autofocus;
        this.onTap = onTap;
        this.autocorrect = autocorrect;
        this.enabled = enabled;
        this.cursorWidth = cursorWidth;
        this.cursorHeight = cursorHeight;
        this.cursorRadius = __cursorRadius;
        this.cursorOpacityAnimates = cursorOpacityAnimates;
        this.cursorColor = cursorColor;
        System.Diagnostics.Debug.Assert(!(decoration is not null && backgroundColor is not null));
        System.Diagnostics.Debug.Assert(!(decoration is not null && borderRadius is not null));
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoSearchTextFieldState__search_field());
}

internal class _CupertinoSearchTextFieldState__search_field : global::Doroti.Framework.Widgets.State<CupertinoSearchTextField>, global::Doroti.Framework.Widgets.RestorationMixin<CupertinoSearchTextField>
{
    internal virtual global::Doroti.Framework.Painting.BorderRadius _kDefaultBorderRadius { get; private set; } = BorderRadius.CreateAll(Radius.circular(9.0));
    internal virtual global::Doroti.Framework.Widgets.RestorableTextEditingController? _controller { get; set; } = default;
    internal virtual global::Doroti.Framework.Widgets.FocusNode? _focusNode { get; set; } = default;
    internal virtual global::Doroti.Framework.Widgets.ScrollNotificationObserverState? _scrollNotificationObserver { get; set; } = default;
    internal virtual double _scaledIconSize { get; set; } = default!;
    internal virtual double _fadeExtent { get; set; } = 0.0;
    public virtual global::Doroti.Framework.Services.RestorationBucket? _bucket { get; set; } = default;
    public virtual DartMap<global::Doroti.Framework.Widgets.IRestorableProperty, global::System.Action> _properties { get; set; } = new DartMap<global::Doroti.Framework.Widgets.IRestorableProperty, global::System.Action>();
    public virtual List<global::Doroti.Framework.Widgets.IRestorableProperty>? _debugPropertiesWaitingForReregistration { get; set; } = default;
    public virtual bool _firstRestorePending { get; set; } = true;
    public virtual global::Doroti.Framework.Services.RestorationBucket? _currentParent { get; set; } = default;

    internal virtual global::Doroti.Framework.Widgets.TextEditingController _effectiveController => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.TextEditingController>(widget.controller ?? _controller!.value);
    internal virtual global::Doroti.Framework.Widgets.FocusNode _effectiveFocusNode => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.FocusNode>(widget.focusNode ?? _focusNode!);
    public override void initState()
    {
        base.initState();
        if (widget.controller is null)
        {
            _createLocalController();
        }
        if (widget.focusNode is null)
        {
            _focusNode = new global::Doroti.Framework.Widgets.FocusNode();
        }
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        global::Doroti.Framework.Services.RestorationBucket? oldBucket = _bucket;
        bool needsRestore = restorePending;
        _currentParent = RestorationScope.maybeOf(context);
        bool didReplaceBucket = _updateBucketIfNecessary(parent: _currentParent, restorePending: needsRestore);
        if (needsRestore)
        {
            _doRestore(oldBucket);
        }
        if (didReplaceBucket)
        {
            DartRuntimePrimitives.Assert(() => !Equals(oldBucket, _bucket));
            oldBucket?.dispose();
        }
        _scrollNotificationObserver?.removeListener(_handleScrollNotification);
        _scrollNotificationObserver = ScrollNotificationObserver.maybeOf(context);
        _scrollNotificationObserver?.addListener(_handleScrollNotification);
    }

    public override void didUpdateWidget(CupertinoSearchTextField oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        didUpdateRestorationId();
        if ((widget.controller is null) && (oldWidget.controller is not null))
        {
            _createLocalController(oldWidget.controller!.value);
        }
        else
        {
            if ((widget.controller is not null) && (oldWidget.controller is null))
            {
                unregisterFromRestoration(_controller!);
                _controller!.dispose();
                _controller = null;
            }
        }
        if ((widget.focusNode is null) && (oldWidget.focusNode is not null))
        {
            _focusNode = new global::Doroti.Framework.Widgets.FocusNode();
        }
        else
        {
            if ((widget.focusNode is not null) && (oldWidget.focusNode is null))
            {
                _focusNode!.dispose();
                _focusNode = null;
            }
        }
    }

    public virtual void restoreState(global::Doroti.Framework.Services.RestorationBucket? oldBucket, bool initialRestore)
    {
        if (_controller is not null)
        {
            _registerController();
        }
    }

    public override void dispose()
    {
        if (_scrollNotificationObserver is not null)
        {
            _scrollNotificationObserver!.removeListener(_handleScrollNotification);
            _scrollNotificationObserver = null;
        }
        if (widget.focusNode is null)
        {
            _focusNode?.dispose();
        }
        if (widget.controller is null)
        {
            _controller?.dispose();
        }
        _properties.forEach((property, listener) =>
        {
            if (!property._disposed)
            {
                property.removeListener(listener);
            }
        });
        _bucket?.dispose();
        _bucket = null;
        base.dispose();
    }

    internal virtual void _registerController()
    {
        DartRuntimePrimitives.Assert(() => _controller is not null);
        registerForRestoration(_controller!, "controller");
    }

    internal virtual void _createLocalController(global::Doroti.Framework.Services.TextEditingValue? value = null)
    {
        DartRuntimePrimitives.Assert(() => _controller is null);
        _controller = (value is null) ? RestorableTextEditingController.Create() : new global::Doroti.Framework.Widgets.RestorableTextEditingController(value);
        if (!restorePending)
        {
            _registerController();
        }
    }

    public virtual string? restorationId => widget.restorationId;
    internal virtual void _defaultOnSuffixTap()
    {
        bool textChanged = _effectiveController.text.Length != 0;
        _effectiveController.clear();
        if ((widget.onChanged is not null) && textChanged)
        {
            widget.onChanged!(_effectiveController.text);
        }
    }

    internal virtual void _handleScrollNotification(global::Doroti.Framework.Widgets.ScrollNotification notification)
    {
        if (notification is global::Doroti.Framework.Widgets.ScrollUpdateNotification)
        {
            global::Doroti.Framework.Widgets.ScrollUpdateNotification notification__as17708 = (global::Doroti.Framework.Widgets.ScrollUpdateNotification)notification;
            double currentHeight = context.size?.height ?? 0.0;
            setState(() =>
            {
                _fadeExtent = _calculateScrollOpacity(currentHeight, _scaledIconSize + Math.Max(widget.prefixInsets.vertical, widget.suffixInsets.vertical));
            });
        }
    }

    internal static double _calculateScrollOpacity(double currentHeight, double maxHeight)
    {
        double thresholdHeight = maxHeight * Search_fieldLibrary._kMinHeightBeforeTotalTransparency;
        if (currentHeight >= maxHeight)
        {
            return 0.0;
        }
        else
        {
            if (currentHeight <= thresholdHeight)
            {
                return 1.0;
            }
            else
            {
                double range = maxHeight - thresholdHeight;
                double progress = (currentHeight - thresholdHeight) / range;
                return 1.0 - progress;
            }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry _animatedInsets(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Painting.EdgeInsetsGeometry insets)
    {
        global::Doroti.Framework.Painting.EdgeInsets currentInsets = insets.resolve(Directionality.of(context));
        global::Doroti.Framework.Painting.EdgeInsetsGeometry? animatedInsets = EdgeInsetsGeometry.lerp(insets, currentInsets.copyWith(top: currentInsets.top / 2L), _fadeExtent);
        return animatedInsets ?? insets;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        string placeholderLocal = widget.placeholder ?? CupertinoLocalizations.of(context).searchTextFieldPlaceholderLabel;
        global::Doroti.Ui.Color defaultPlaceholderColor = CupertinoDynamicColor.resolve(CupertinoColors.secondaryLabel, context);
        global::Doroti.Framework.Painting.TextStyle placeholderStyleLocal = widget.placeholderStyle ?? new global::Doroti.Framework.Painting.TextStyle(color: defaultPlaceholderColor.withAlpha((255L * (defaultPlaceholderColor.a * (1L - _fadeExtent))).round()));
        _scaledIconSize = MediaQuery.textScalerOf(context).scale(widget.itemSize);
        global::Doroti.Framework.Painting.BoxDecoration decorationLocal = widget.decoration ?? new global::Doroti.Framework.Painting.BoxDecoration(color: widget.backgroundColor ?? CupertinoColors.tertiarySystemFill, borderRadius: widget.borderRadius ?? _kDefaultBorderRadius);
        global::Doroti.Ui.Color iconColor = CupertinoDynamicColor.resolve(widget.itemColor, context);
        var suffixIconThemeData = new global::Doroti.Framework.Widgets.IconThemeData(color: iconColor, size: _scaledIconSize);
        var prefixIconThemeData = new global::Doroti.Framework.Widgets.IconThemeData(color: iconColor, size: ((_scaledIconSize >= Search_fieldLibrary._kMaxPrefixIconSize) && _effectiveFocusNode.hasFocus) ? 0.0 : _scaledIconSize);
        global::Doroti.Framework.Widgets.Widget prefixLocal = new global::Doroti.Framework.Widgets.Opacity(opacity: 1.0 - _fadeExtent, child: new global::Doroti.Framework.Widgets.Padding(padding: _animatedInsets(context, widget.prefixInsets), child: new global::Doroti.Framework.Widgets.IconTheme(data: prefixIconThemeData, child: widget.prefixIcon)));
        global::Doroti.Framework.Widgets.Widget suffixLocal = new global::Doroti.Framework.Widgets.Opacity(opacity: 1.0 - _fadeExtent, child: new global::Doroti.Framework.Widgets.Padding(padding: _animatedInsets(context, widget.suffixInsets), child: new CupertinoButton(onPressed: widget.onSuffixTap ?? _defaultOnSuffixTap, minSize: 0, padding: EdgeInsets.zero, child: new global::Doroti.Framework.Widgets.IconTheme(data: suffixIconThemeData, child: widget.suffixIcon))));
        return new CupertinoTextField(controller: _effectiveController, decoration: decorationLocal, style: widget.style, prefix: prefixLocal, suffix: suffixLocal, keyboardType: widget.keyboardType, onTap: widget.onTap, enabled: widget.enabled ?? true, cursorWidth: widget.cursorWidth, cursorHeight: widget.cursorHeight, cursorRadius: widget.cursorRadius, cursorOpacityAnimates: widget.cursorOpacityAnimates, cursorColor: widget.cursorColor, suffixMode: widget.suffixMode, placeholder: placeholderLocal, placeholderStyle: placeholderStyleLocal, padding: _animatedInsets(context, widget.padding), onChanged: widget.onChanged, onSubmitted: widget.onSubmitted, focusNode: _effectiveFocusNode, autofocus: widget.autofocus, autocorrect: widget.autocorrect, smartQuotesType: widget.smartQuotesType, smartDashesType: widget.smartDashesType, enableIMEPersonalizedLearning: widget.enableIMEPersonalizedLearning, textInputAction: TextInputAction.search);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Services.RestorationBucket? bucket => _bucket;
    public virtual void didToggleBucket(global::Doroti.Framework.Services.RestorationBucket? oldBucket)
    {
        DartRuntimePrimitives.Assert(() => _bucket?.isReplacing != true);
    }

    public virtual void registerForRestoration(global::Doroti.Framework.Widgets.IRestorableProperty property, string restorationId)
    {
        DartRuntimePrimitives.Assert(() => (property._restorationId is null) || _debugDoingRestore && (property._restorationId == restorationId), () => (object?)$"Property is already registered under {property._restorationId}.");
        DartRuntimePrimitives.Assert(() => _debugDoingRestore || !_properties.Keys.map<global::Doroti.Framework.Widgets.IRestorableProperty, string?>((r) => r._restorationId).contains(restorationId), () => (object?)$"\"{restorationId}\" is already registered to another property.");
        bool hasSerializedValue = bucket?.contains(restorationId) ?? false;
        object? initialValue = hasSerializedValue ? property.fromPrimitivesObject(bucket!.read<object>(restorationId)) : property.createDefaultValueObject();
        if (!property.isRegistered)
        {
            property._register(restorationId, this);
            void listener()
            {
                if (bucket is null)
                {
                    return;
                }
                _updateProperty(property);
            }
            property.addListener(listener);
            _properties[property] = listener;
        }
        DartRuntimePrimitives.Assert(() => (property._restorationId == restorationId) && Equals(property._owner, this) && _properties.ContainsKey(property));
        property.initWithValueObject(initialValue);
        if (!hasSerializedValue && property.enabled && (bucket is not null))
        {
            _updateProperty(property);
        }
        DartRuntimePrimitives.Assert(() =>
            {
                _debugPropertiesWaitingForReregistration?.Remove(property);
                return true;
            });
    }

    public virtual void unregisterFromRestoration(global::Doroti.Framework.Widgets.IRestorableProperty property)
    {
        DartRuntimePrimitives.Assert(() => Equals(property._owner, this));
        _bucket?.remove<object?>(property._restorationId!);
        _unregister(property);
    }

    public virtual void didUpdateRestorationId()
    {
        if ((_currentParent is null) || (_bucket?.restorationId == restorationId) || restorePending)
        {
            return;
        }
        global::Doroti.Framework.Services.RestorationBucket? oldBucket = _bucket;
        DartRuntimePrimitives.Assert(() => !restorePending);
        bool didReplaceBucket = _updateBucketIfNecessary(parent: _currentParent, restorePending: false);
        if (didReplaceBucket)
        {
            DartRuntimePrimitives.Assert(() => !Equals(oldBucket, _bucket));
            DartRuntimePrimitives.Assert(() => (_bucket is null) || (oldBucket is null));
            oldBucket?.dispose();
        }
    }

    public virtual bool restorePending
    {
        get
        {
            if (_firstRestorePending)
            {
                return true;
            }
            if (restorationId is null)
            {
                return false;
            }
            global::Doroti.Framework.Services.RestorationBucket? potentialNewParent = RestorationScope.maybeOf(context);
            return (!Equals(potentialNewParent, _currentParent)) && (potentialNewParent?.isReplacing ?? false);
        }
    }
    public virtual bool _debugDoingRestore => DartRuntimePrimitives.ConvertValue<bool>(_debugPropertiesWaitingForReregistration is not null);
    public virtual void _doRestore(global::Doroti.Framework.Services.RestorationBucket? oldBucket)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                _debugPropertiesWaitingForReregistration = _properties.Keys.ToList();
                return true;
            });
        restoreState(oldBucket, _firstRestorePending);
        _firstRestorePending = false;
        DartRuntimePrimitives.Assert(() =>
            {
                if (Enumerable.Any(_debugPropertiesWaitingForReregistration!))
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(((Func<List<global::Doroti.Framework.Foundation.DiagnosticsNode>>)(() => { var __collection41817 = new List<global::Doroti.Framework.Foundation.DiagnosticsNode>(); __collection41817.Add(new global::Doroti.Framework.Foundation.ErrorSummary("Previously registered RestorableProperties must be re-registered in \"restoreState\".")); __collection41817.Add(new global::Doroti.Framework.Foundation.ErrorDescription($"The RestorableProperties with the following IDs were not re-registered to {this} when " + "\"restoreState\" was called:")); __collection41817.AddRange(_debugPropertiesWaitingForReregistration!.map<global::Doroti.Framework.Widgets.IRestorableProperty, global::Doroti.Framework.Foundation.DiagnosticsNode>((property) => new global::Doroti.Framework.Foundation.ErrorDescription($" * {property._restorationId}"))); return __collection41817; }))()));
                }
                _debugPropertiesWaitingForReregistration = null;
                return true;
            });
    }

    public virtual bool _updateBucketIfNecessary(global::Doroti.Framework.Services.RestorationBucket? parent, bool restorePending)
    {
        if ((restorationId is null) || (parent is null))
        {
            bool didReplace = _setNewBucketIfNecessary(newBucket: null, restorePending: restorePending);
            DartRuntimePrimitives.Assert(() => _bucket is null);
            return didReplace;
        }
        DartRuntimePrimitives.Assert(() => restorationId is not null);
        if (restorePending || (_bucket is null))
        {
            global::Doroti.Framework.Services.RestorationBucket newBucketLocal = parent.claimChild(restorationId!, debugOwner: this);
            bool didReplaceLocal = _setNewBucketIfNecessary(newBucket: newBucketLocal, restorePending: restorePending);
            DartRuntimePrimitives.Assert(() => Equals(_bucket, newBucketLocal));
            return didReplaceLocal;
        }
        DartRuntimePrimitives.Assert(() => _bucket is not null);
        DartRuntimePrimitives.Assert(() => !restorePending);
        _bucket!.rename(restorationId!);
        parent.adoptChild(_bucket!);
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _setNewBucketIfNecessary(global::Doroti.Framework.Services.RestorationBucket? newBucket, bool restorePending)
    {
        if (Equals(newBucket, _bucket))
        {
            return false;
        }
        global::Doroti.Framework.Services.RestorationBucket? oldBucket = _bucket;
        _bucket = newBucket;
        if (!restorePending)
        {
            if (_bucket is not null)
            {
                _properties.Keys.forEach((__arg0) => ((global::System.Action<global::Doroti.Framework.Widgets.IRestorableProperty>)_updateProperty)(__arg0));
            }
            didToggleBucket(oldBucket);
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _updateProperty(global::Doroti.Framework.Widgets.IRestorableProperty property)
    {
        if (property.enabled)
        {
            _bucket?.write(property._restorationId!, property.toPrimitives());
        }
        else
        {
            _bucket?.remove<object>(property._restorationId!);
        }
    }

    public virtual void _unregister(global::Doroti.Framework.Widgets.IRestorableProperty property)
    {
        global::System.Action listener = _properties.remove(property)!;
        DartRuntimePrimitives.Assert(() =>
            {
                _debugPropertiesWaitingForReregistration?.Remove(property);
                return true;
            });
        property.removeListener(listener);
        property._unregister();
    }

}
