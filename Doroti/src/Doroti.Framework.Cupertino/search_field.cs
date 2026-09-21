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

public class CupertinoSearchTextField : StatefulWidget
{
    public virtual TextEditingController? controller { get; private set; }
    public virtual Action<string>? onChanged { get; private set; }
    public virtual Action<string>? onSubmitted { get; private set; }
    public virtual TextStyle? style { get; private set; }
    public virtual string? placeholder { get; private set; }
    public virtual TextStyle? placeholderStyle { get; private set; }
    public virtual BoxDecoration? decoration { get; private set; }
    public virtual Color? backgroundColor { get; private set; }
    public virtual BorderRadius? borderRadius { get; private set; }
    public virtual TextInputType? keyboardType { get; private set; }
    public virtual EdgeInsetsGeometry padding { get; private set; } = default!;
    public virtual Color itemColor { get; private set; } = default!;
    public virtual double itemSize { get; private set; } = default!;
    public virtual EdgeInsetsGeometry prefixInsets { get; private set; } = default!;
    public virtual Widget prefixIcon { get; private set; } = default!;
    public virtual EdgeInsetsGeometry suffixInsets { get; private set; } = default!;
    public virtual Icon suffixIcon { get; private set; } = default!;
    public virtual OverlayVisibilityMode suffixMode { get; private set; } = default!;
    public virtual Action? onSuffixTap { get; private set; }
    public virtual string? restorationId { get; private set; }
    public virtual FocusNode? focusNode { get; private set; }
    public virtual bool autofocus { get; private set; } = default!;
    public virtual Action? onTap { get; private set; }
    public virtual bool autocorrect { get; private set; } = default!;
    public virtual SmartQuotesType? smartQuotesType { get; private set; }
    public virtual SmartDashesType? smartDashesType { get; private set; }
    public virtual bool enableIMEPersonalizedLearning { get; private set; } = default!;
    public virtual bool? enabled { get; private set; }
    public virtual double cursorWidth { get; private set; } = default!;
    public virtual double? cursorHeight { get; private set; }
    public virtual Radius cursorRadius { get; private set; } = default!;
    public virtual bool cursorOpacityAnimates { get; private set; } = default!;
    public virtual Color? cursorColor { get; private set; }

    public CupertinoSearchTextField(
        Key? key = null,
        TextEditingController? controller = null,
        Action<string>? onChanged = null,
        Action<string>? onSubmitted = null,
        TextStyle? style = null,
        string? placeholder = null,
        TextStyle? placeholderStyle = null,
        BoxDecoration? decoration = null,
        Color? backgroundColor = null,
        BorderRadius? borderRadius = null,
        TextInputType? keyboardType = default!,
        EdgeInsetsGeometry padding = default!,
        Color itemColor = default!,
        double itemSize = 20.0,
        EdgeInsetsGeometry prefixInsets = default!,
        Widget prefixIcon = default!,
        EdgeInsetsGeometry suffixInsets = default!,
        Icon suffixIcon = default!,
        OverlayVisibilityMode suffixMode = OverlayVisibilityMode.editing,
        Action? onSuffixTap = null,
        string? restorationId = null,
        FocusNode? focusNode = null,
        SmartQuotesType? smartQuotesType = null,
        SmartDashesType? smartDashesType = null,
        bool enableIMEPersonalizedLearning = true,
        bool autofocus = false,
        Action? onTap = null,
        bool autocorrect = true,
        bool? enabled = null,
        double cursorWidth = 2.0,
        double? cursorHeight = null,
        Radius? cursorRadius = null,
        bool cursorOpacityAnimates = true,
        Color? cursorColor = null
    )
        : base(key: key)
    {
        TextInputType? __keyboardType = keyboardType ?? TextInputType.text;
        EdgeInsetsGeometry __padding = padding ?? EdgeInsetsGeometry.CreateFromSTEB(5.5, 8, 5.5, 8);
        Color __itemColor = itemColor ?? CupertinoColors.secondaryLabel;
        EdgeInsetsGeometry __prefixInsets =
            prefixInsets ?? EdgeInsetsGeometry.CreateFromSTEB(6, 8, 0, 8);
        Widget __prefixIcon = prefixIcon ?? new Icon(CupertinoIcons.search);
        EdgeInsetsGeometry __suffixInsets =
            suffixInsets ?? EdgeInsetsGeometry.CreateFromSTEB(0, 8, 5, 8);
        Icon __suffixIcon = suffixIcon ?? new Icon(CupertinoIcons.xmark_circle_fill);
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

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(
            new _CupertinoSearchTextFieldState__search_field()
        );
}

internal class _CupertinoSearchTextFieldState__search_field
    : State<CupertinoSearchTextField>,
        RestorationMixin<CupertinoSearchTextField>
{
    internal virtual BorderRadius _kDefaultBorderRadius { get; private set; } =
        BorderRadius.CreateAll(Radius.circular(9.0));
    internal virtual RestorableTextEditingController? _controller { get; set; } = default;
    internal virtual FocusNode? _focusNode { get; set; } = default;
    internal virtual ScrollNotificationObserverState? _scrollNotificationObserver { get; set; } =
        default;
    internal virtual double _scaledIconSize { get; set; } = default!;
    internal virtual double _fadeExtent { get; set; } = 0.0;
    public virtual RestorationBucket? _bucket { get; set; } = default;
    public virtual DartMap<IRestorableProperty, Action> _properties { get; set; } =
        new DartMap<IRestorableProperty, Action>();
    public virtual List<IRestorableProperty>? _debugPropertiesWaitingForReregistration { get; set; } =
        default;
    public virtual bool _firstRestorePending { get; set; } = true;
    public virtual RestorationBucket? _currentParent { get; set; } = default;

    internal virtual TextEditingController _effectiveController =>
        DartRuntimePrimitives.ConvertValue<TextEditingController>(
            widget.controller ?? _controller!.value
        );
    internal virtual FocusNode _effectiveFocusNode =>
        DartRuntimePrimitives.ConvertValue<FocusNode>(widget.focusNode ?? _focusNode!);

    public override void initState()
    {
        base.initState();
        if (widget.controller is null)
        {
            _createLocalController();
        }
        if (widget.focusNode is null)
        {
            _focusNode = new FocusNode();
        }
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        RestorationBucket? oldBucket = _bucket;
        bool needsRestore = restorePending;
        _currentParent = RestorationScope.maybeOf(context);
        bool didReplaceBucket = _updateBucketIfNecessary(
            parent: _currentParent,
            restorePending: needsRestore
        );
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
            _focusNode = new FocusNode();
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

    public virtual void restoreState(RestorationBucket? oldBucket, bool initialRestore)
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
        _properties.forEach(
            (property, listener) =>
            {
                if (!property._disposed)
                {
                    property.removeListener(listener);
                }
            }
        );
        _bucket?.dispose();
        _bucket = null;
        base.dispose();
    }

    internal virtual void _registerController()
    {
        DartRuntimePrimitives.Assert(() => _controller is not null);
        registerForRestoration(_controller!, "controller");
    }

    internal virtual void _createLocalController(TextEditingValue? value = null)
    {
        DartRuntimePrimitives.Assert(() => _controller is null);
        _controller =
            (value is null)
                ? RestorableTextEditingController.Create()
                : new RestorableTextEditingController(value);
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

    internal virtual void _handleScrollNotification(ScrollNotification notification)
    {
        if (notification is ScrollUpdateNotification)
        {
            ScrollUpdateNotification notification__as17708 = (ScrollUpdateNotification)notification;
            double currentHeight = context.size?.height ?? 0.0;
            setState(() =>
            {
                _fadeExtent = _calculateScrollOpacity(
                    currentHeight,
                    _scaledIconSize
                        + Math.Max(widget.prefixInsets.vertical, widget.suffixInsets.vertical)
                );
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

    internal virtual EdgeInsetsGeometry _animatedInsets(
        BuildContext context,
        EdgeInsetsGeometry insets
    )
    {
        EdgeInsets currentInsets = insets.resolve(Directionality.of(context));
        EdgeInsetsGeometry? animatedInsets = EdgeInsetsGeometry.lerp(
            insets,
            currentInsets.copyWith(top: currentInsets.top / 2L),
            _fadeExtent
        );
        return animatedInsets ?? insets;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        string placeholderLocal =
            widget.placeholder
            ?? CupertinoLocalizations.of(context).searchTextFieldPlaceholderLabel;
        Color defaultPlaceholderColor = CupertinoDynamicColor.resolve(
            CupertinoColors.secondaryLabel,
            context
        );
        TextStyle placeholderStyleLocal =
            widget.placeholderStyle
            ?? new TextStyle(
                color: defaultPlaceholderColor.withAlpha(
                    (255L * (defaultPlaceholderColor.a * (1L - _fadeExtent))).round()
                )
            );
        _scaledIconSize = MediaQuery.textScalerOf(context).scale(widget.itemSize);
        BoxDecoration decorationLocal =
            widget.decoration
            ?? new BoxDecoration(
                color: widget.backgroundColor ?? CupertinoColors.tertiarySystemFill,
                borderRadius: widget.borderRadius ?? _kDefaultBorderRadius
            );
        Color iconColor = CupertinoDynamicColor.resolve(widget.itemColor, context);
        var suffixIconThemeData = new IconThemeData(color: iconColor, size: _scaledIconSize);
        var prefixIconThemeData = new IconThemeData(
            color: iconColor,
            size: (
                (_scaledIconSize >= Search_fieldLibrary._kMaxPrefixIconSize)
                && _effectiveFocusNode.hasFocus
            )
                ? 0.0
                : _scaledIconSize
        );
        Widget prefixLocal = new Opacity(
            opacity: 1.0 - _fadeExtent,
            child: new Padding(
                padding: _animatedInsets(context, widget.prefixInsets),
                child: new IconTheme(data: prefixIconThemeData, child: widget.prefixIcon)
            )
        );
        Widget suffixLocal = new Opacity(
            opacity: 1.0 - _fadeExtent,
            child: new Padding(
                padding: _animatedInsets(context, widget.suffixInsets),
                child: new CupertinoButton(
                    onPressed: widget.onSuffixTap ?? _defaultOnSuffixTap,
                    minSize: 0,
                    padding: EdgeInsets.zero,
                    child: new IconTheme(data: suffixIconThemeData, child: widget.suffixIcon)
                )
            )
        );
        return new CupertinoTextField(
            controller: _effectiveController,
            decoration: decorationLocal,
            style: widget.style,
            prefix: prefixLocal,
            suffix: suffixLocal,
            keyboardType: widget.keyboardType,
            onTap: widget.onTap,
            enabled: widget.enabled ?? true,
            cursorWidth: widget.cursorWidth,
            cursorHeight: widget.cursorHeight,
            cursorRadius: widget.cursorRadius,
            cursorOpacityAnimates: widget.cursorOpacityAnimates,
            cursorColor: widget.cursorColor,
            suffixMode: widget.suffixMode,
            placeholder: placeholderLocal,
            placeholderStyle: placeholderStyleLocal,
            padding: _animatedInsets(context, widget.padding),
            onChanged: widget.onChanged,
            onSubmitted: widget.onSubmitted,
            focusNode: _effectiveFocusNode,
            autofocus: widget.autofocus,
            autocorrect: widget.autocorrect,
            smartQuotesType: widget.smartQuotesType,
            smartDashesType: widget.smartDashesType,
            enableIMEPersonalizedLearning: widget.enableIMEPersonalizedLearning,
            textInputAction: TextInputAction.search
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RestorationBucket? bucket => _bucket;

    public virtual void didToggleBucket(RestorationBucket? oldBucket)
    {
        DartRuntimePrimitives.Assert(() => _bucket?.isReplacing != true);
    }

    public virtual void registerForRestoration(IRestorableProperty property, string restorationId)
    {
        DartRuntimePrimitives.Assert(
            () =>
                (property._restorationId is null)
                || (_debugDoingRestore && (property._restorationId == restorationId)),
            () => (object?)$"Property is already registered under {property._restorationId}."
        );
        DartRuntimePrimitives.Assert(
            () =>
                _debugDoingRestore
                || !_properties.Keys.map((r) => r._restorationId).contains(restorationId),
            () => (object?)$"\"{restorationId}\" is already registered to another property."
        );
        bool hasSerializedValue = bucket?.contains(restorationId) ?? false;
        object? initialValue = hasSerializedValue
            ? property.fromPrimitivesObject(bucket!.read<object>(restorationId))
            : property.createDefaultValueObject();
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
        DartRuntimePrimitives.Assert(() =>
            (property._restorationId == restorationId)
            && Equals(property._owner, this)
            && _properties.ContainsKey(property)
        );
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

    public virtual void unregisterFromRestoration(IRestorableProperty property)
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
        RestorationBucket? oldBucket = _bucket;
        DartRuntimePrimitives.Assert(() => !restorePending);
        bool didReplaceBucket = _updateBucketIfNecessary(
            parent: _currentParent,
            restorePending: false
        );
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
            RestorationBucket? potentialNewParent = RestorationScope.maybeOf(context);
            return (!Equals(potentialNewParent, _currentParent))
                && (potentialNewParent?.isReplacing ?? false);
        }
    }
    public virtual bool _debugDoingRestore =>
        DartRuntimePrimitives.ConvertValue<bool>(
            _debugPropertiesWaitingForReregistration is not null
        );

    public virtual void _doRestore(RestorationBucket? oldBucket)
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
                throw DartRuntimePrimitives.AsException(
                    new FlutterError(
                        (
                            (Func<List<DiagnosticsNode>>)(
                                () =>
                                {
                                    var __collection41817 = new List<DiagnosticsNode>();
                                    __collection41817.Add(
                                        new ErrorSummary(
                                            "Previously registered RestorableProperties must be re-registered in \"restoreState\"."
                                        )
                                    );
                                    __collection41817.Add(
                                        new ErrorDescription(
                                            $"The RestorableProperties with the following IDs were not re-registered to {this} when "
                                                + "\"restoreState\" was called:"
                                        )
                                    );
                                    __collection41817.AddRange(
                                        _debugPropertiesWaitingForReregistration!.map<
                                            IRestorableProperty,
                                            DiagnosticsNode
                                        >(
                                            (property) =>
                                                new ErrorDescription(
                                                    $" * {property._restorationId}"
                                                )
                                        )
                                    );
                                    return __collection41817;
                                }
                            )
                        )()
                    )
                );
            }
            _debugPropertiesWaitingForReregistration = null;
            return true;
        });
    }

    public virtual bool _updateBucketIfNecessary(RestorationBucket? parent, bool restorePending)
    {
        if ((restorationId is null) || (parent is null))
        {
            bool didReplace = _setNewBucketIfNecessary(
                newBucket: null,
                restorePending: restorePending
            );
            DartRuntimePrimitives.Assert(() => _bucket is null);
            return didReplace;
        }
        DartRuntimePrimitives.Assert(() => restorationId is not null);
        if (restorePending || (_bucket is null))
        {
            RestorationBucket newBucketLocal = parent.claimChild(restorationId!, debugOwner: this);
            bool didReplaceLocal = _setNewBucketIfNecessary(
                newBucket: newBucketLocal,
                restorePending: restorePending
            );
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

    public virtual bool _setNewBucketIfNecessary(RestorationBucket? newBucket, bool restorePending)
    {
        if (Equals(newBucket, _bucket))
        {
            return false;
        }
        RestorationBucket? oldBucket = _bucket;
        _bucket = newBucket;
        if (!restorePending)
        {
            if (_bucket is not null)
            {
                _properties.Keys.forEach(
                    (__arg0) => ((Action<IRestorableProperty>)_updateProperty)(__arg0)
                );
            }
            didToggleBucket(oldBucket);
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _updateProperty(IRestorableProperty property)
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

    public virtual void _unregister(IRestorableProperty property)
    {
        Action listener = _properties.remove(property)!;
        DartRuntimePrimitives.Assert(() =>
        {
            _debugPropertiesWaitingForReregistration?.Remove(property);
            return true;
        });
        property.removeListener(listener);
        property._unregister();
    }
}
