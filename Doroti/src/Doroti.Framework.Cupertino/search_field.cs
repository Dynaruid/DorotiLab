// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/search_field.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Cupertino;

public static partial class Search_fieldLibrary
{
    internal static double _kMinHeightBeforeTotalTransparency = (4L / 5L);
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
        global::Doroti.Framework.Services.TextInputType? __keyboardType = keyboardType ?? global::Doroti.Framework.Services.TextInputType.text;
        global::Doroti.Framework.Painting.EdgeInsetsGeometry __padding = padding ?? global::Doroti.Framework.Painting.EdgeInsetsDirectional.CreateFromSTEB(5.5, 8, 5.5, 8);
        Color __itemColor = itemColor ?? CupertinoColors.secondaryLabel;
        global::Doroti.Framework.Painting.EdgeInsetsGeometry __prefixInsets = prefixInsets ?? global::Doroti.Framework.Painting.EdgeInsetsDirectional.CreateFromSTEB(6, 8, 0, 8);
        global::Doroti.Framework.Widgets.Widget __prefixIcon = prefixIcon ?? new global::Doroti.Framework.Widgets.Icon(CupertinoIcons.search);
        global::Doroti.Framework.Painting.EdgeInsetsGeometry __suffixInsets = suffixInsets ?? global::Doroti.Framework.Painting.EdgeInsetsDirectional.CreateFromSTEB(0, 8, 5, 8);
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
        System.Diagnostics.Debug.Assert(!((((decoration is not null)) && ((backgroundColor is not null)))));
        System.Diagnostics.Debug.Assert(!((((decoration is not null)) && ((borderRadius is not null)))));
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoSearchTextFieldState__search_field());
}

internal class _CupertinoSearchTextFieldState__search_field : global::Doroti.Framework.Widgets.State<CupertinoSearchTextField>, global::Doroti.Framework.Widgets.RestorationMixin<CupertinoSearchTextField>
{
    internal virtual global::Doroti.Framework.Painting.BorderRadius _kDefaultBorderRadius { get; private set; } = global::Doroti.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Ui.Radius.circular(9.0));
    internal virtual global::Doroti.Framework.Widgets.RestorableTextEditingController? _controller { get; set; } = default;
    internal virtual global::Doroti.Framework.Widgets.FocusNode? _focusNode { get; set; } = default;
    internal virtual global::Doroti.Framework.Widgets.ScrollNotificationObserverState? _scrollNotificationObserver { get; set; } = default;
    internal virtual double _scaledIconSize { get; set; } = default!;
    internal virtual double _fadeExtent { get; set; } = 0.0;
    public virtual global::Doroti.Framework.Services.RestorationBucket? _bucket { get; set; } = default;
    public virtual DartMap<dynamic, global::System.Action> _properties { get; set; } = new DartMap<dynamic, global::System.Action>();
    public virtual List<dynamic>? _debugPropertiesWaitingForReregistration { get; set; } = default;
    public virtual bool _firstRestorePending { get; set; } = true;
    public virtual global::Doroti.Framework.Services.RestorationBucket? _currentParent { get; set; } = default;

    internal virtual global::Doroti.Framework.Widgets.TextEditingController _effectiveController => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.TextEditingController>((((CupertinoSearchTextField)this.widget).controller ?? this._controller!.value));
    internal virtual global::Doroti.Framework.Widgets.FocusNode _effectiveFocusNode => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.FocusNode>((((CupertinoSearchTextField)this.widget).focusNode ?? this._focusNode!));
    public override void initState()
    {
        base.initState();
        if ((((CupertinoSearchTextField)this.widget).controller is null))
        {
            _createLocalController();
        }
        if ((((CupertinoSearchTextField)this.widget).focusNode is null))
        {
            _focusNode = new global::Doroti.Framework.Widgets.FocusNode();
        }
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        global::Doroti.Framework.Services.RestorationBucket? oldBucket = this._bucket;
        bool needsRestore = this.restorePending;
        _currentParent = RestorationScope.maybeOf(this.context);
        bool didReplaceBucket = _updateBucketIfNecessary(parent: this._currentParent, restorePending: needsRestore);
        if (needsRestore)
        {
            _doRestore(oldBucket);
        }
        if (didReplaceBucket)
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(oldBucket, this._bucket)));
            oldBucket?.dispose();
        }
        this._scrollNotificationObserver?.removeListener((global::System.Action<global::Doroti.Framework.Widgets.ScrollNotification>)this._handleScrollNotification);
        _scrollNotificationObserver = ScrollNotificationObserver.maybeOf(this.context);
        this._scrollNotificationObserver?.addListener((global::System.Action<global::Doroti.Framework.Widgets.ScrollNotification>)this._handleScrollNotification);
    }

    public override void didUpdateWidget(CupertinoSearchTextField oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        didUpdateRestorationId();
        if (((((CupertinoSearchTextField)this.widget).controller is null) && (((CupertinoSearchTextField)oldWidget).controller is not null)))
        {
            _createLocalController(((CupertinoSearchTextField)oldWidget).controller!.value);
        }
        else
        {
            if (((((CupertinoSearchTextField)this.widget).controller is not null) && (((CupertinoSearchTextField)oldWidget).controller is null)))
            {
                unregisterFromRestoration(this._controller!);
                this._controller!.dispose();
                _controller = null;
            }
        }
        if (((((CupertinoSearchTextField)this.widget).focusNode is null) && (((CupertinoSearchTextField)oldWidget).focusNode is not null)))
        {
            _focusNode = new global::Doroti.Framework.Widgets.FocusNode();
        }
        else
        {
            if (((((CupertinoSearchTextField)this.widget).focusNode is not null) && (((CupertinoSearchTextField)oldWidget).focusNode is null)))
            {
                this._focusNode!.dispose();
                _focusNode = null;
            }
        }
    }

    public virtual void restoreState(global::Doroti.Framework.Services.RestorationBucket? oldBucket, bool initialRestore)
    {
        if ((this._controller is not null))
        {
            _registerController();
        }
    }

    public override void dispose()
    {
        if ((this._scrollNotificationObserver is not null))
        {
            this._scrollNotificationObserver!.removeListener((global::System.Action<global::Doroti.Framework.Widgets.ScrollNotification>)this._handleScrollNotification);
            _scrollNotificationObserver = null;
        }
        if ((((CupertinoSearchTextField)this.widget).focusNode is null))
        {
            this._focusNode?.dispose();
        }
        if ((((CupertinoSearchTextField)this.widget).controller is null))
        {
            this._controller?.dispose();
        }
        this._properties.forEach(((global::System.Action<dynamic, global::System.Action>)((property, listener) =>
        {
            if (!((dynamic)property)._disposed)
            {
                property.removeListener(listener);
            }
        })));
        this._bucket?.dispose();
        _bucket = null;
        base.dispose();
    }

    internal virtual void _registerController()
    {
        DartRuntimePrimitives.Assert(() => (this._controller is not null));
        registerForRestoration(DartRuntimePrimitives.ConvertValue<dynamic>(this._controller!), "controller");
    }

    internal virtual void _createLocalController(global::Doroti.Framework.Services.TextEditingValue? value = null)
    {
        DartRuntimePrimitives.Assert(() => (this._controller is null));
        _controller = ((value is null) ? global::Doroti.Framework.Widgets.RestorableTextEditingController.Create() : new global::Doroti.Framework.Widgets.RestorableTextEditingController(value));
        if (!this.restorePending)
        {
            _registerController();
        }
    }

    public virtual string? restorationId => ((CupertinoSearchTextField)this.widget).restorationId;
    internal virtual void _defaultOnSuffixTap()
    {
        bool textChanged = (((global::Doroti.Framework.Widgets.TextEditingController)this._effectiveController).text.Length != 0);
        this._effectiveController.clear();
        if (((((CupertinoSearchTextField)this.widget).onChanged is not null) && textChanged))
        {
            ((CupertinoSearchTextField)this.widget).onChanged!(((global::Doroti.Framework.Widgets.TextEditingController)this._effectiveController).text);
        }
    }

    internal virtual void _handleScrollNotification(global::Doroti.Framework.Widgets.ScrollNotification notification)
    {
        if ((notification is global::Doroti.Framework.Widgets.ScrollUpdateNotification))
        {
            global::Doroti.Framework.Widgets.ScrollUpdateNotification notification__as17708 = (global::Doroti.Framework.Widgets.ScrollUpdateNotification)notification;
            double currentHeight = (((global::Doroti.Framework.Widgets.BuildContext)this.context).size?.height ?? 0.0);
            setState(((global::System.Action)(() =>
            {
                _fadeExtent = _CupertinoSearchTextFieldState__search_field._calculateScrollOpacity(currentHeight, (this._scaledIconSize + Math.Max(((CupertinoSearchTextField)this.widget).prefixInsets.vertical, ((CupertinoSearchTextField)this.widget).suffixInsets.vertical)));
            })));
        }
    }

    internal static double _calculateScrollOpacity(double currentHeight, double maxHeight)
    {
        double thresholdHeight = (maxHeight * Search_fieldLibrary._kMinHeightBeforeTotalTransparency);
        if ((currentHeight >= maxHeight))
        {
            return 0.0;
        }
        else
        {
            if ((currentHeight <= thresholdHeight))
            {
                return 1.0;
            }
            else
            {
                double range = (maxHeight - thresholdHeight);
                double progress = (((currentHeight - thresholdHeight)) / range);
                return (1.0 - progress);
            }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry _animatedInsets(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Painting.EdgeInsetsGeometry insets)
    {
        global::Doroti.Framework.Painting.EdgeInsets currentInsets = ((global::Doroti.Framework.Painting.EdgeInsets)(object?)insets.resolve(Directionality.of(context)));
        global::Doroti.Framework.Painting.EdgeInsetsGeometry? animatedInsets = ((global::Doroti.Framework.Painting.EdgeInsetsGeometry?)(object?)EdgeInsetsGeometry.lerp(insets, currentInsets.copyWith(top: (((global::Doroti.Framework.Painting.EdgeInsets)currentInsets).top / 2L)), this._fadeExtent));
        return (animatedInsets ?? insets);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        string placeholderLocal = (((CupertinoSearchTextField)this.widget).placeholder ?? CupertinoLocalizations.of(context).searchTextFieldPlaceholderLabel);
        global::Doroti.Ui.Color defaultPlaceholderColor = ((global::Doroti.Ui.Color)(object?)CupertinoDynamicColor.resolve(CupertinoColors.secondaryLabel, context));
        global::Doroti.Framework.Painting.TextStyle placeholderStyleLocal = (((CupertinoSearchTextField)this.widget).placeholderStyle ?? new global::Doroti.Framework.Painting.TextStyle(color: defaultPlaceholderColor.withAlpha(((255L * ((defaultPlaceholderColor.a * ((1L - this._fadeExtent)))))).round())));
        _scaledIconSize = MediaQuery.textScalerOf(context).scale(((CupertinoSearchTextField)this.widget).itemSize);
        global::Doroti.Framework.Painting.BoxDecoration decorationLocal = (((CupertinoSearchTextField)this.widget).decoration ?? new global::Doroti.Framework.Painting.BoxDecoration(color: (((CupertinoSearchTextField)this.widget).backgroundColor ?? CupertinoColors.tertiarySystemFill), borderRadius: (((CupertinoSearchTextField)this.widget).borderRadius ?? this._kDefaultBorderRadius)));
        global::Doroti.Ui.Color iconColor = ((global::Doroti.Ui.Color)(object?)CupertinoDynamicColor.resolve(((CupertinoSearchTextField)this.widget).itemColor, context));
        var suffixIconThemeData = new global::Doroti.Framework.Widgets.IconThemeData(color: iconColor, size: this._scaledIconSize);
        var prefixIconThemeData = new global::Doroti.Framework.Widgets.IconThemeData(color: iconColor, size: (((this._scaledIconSize >= Search_fieldLibrary._kMaxPrefixIconSize) && ((global::Doroti.Framework.Widgets.FocusNode)this._effectiveFocusNode).hasFocus) ? 0.0 : this._scaledIconSize));
        global::Doroti.Framework.Widgets.Widget prefixLocal = ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Opacity(opacity: (1.0 - this._fadeExtent), child: new global::Doroti.Framework.Widgets.Padding(padding: _animatedInsets(context, ((CupertinoSearchTextField)this.widget).prefixInsets), child: new global::Doroti.Framework.Widgets.IconTheme(data: prefixIconThemeData, child: ((CupertinoSearchTextField)this.widget).prefixIcon))));
        global::Doroti.Framework.Widgets.Widget suffixLocal = ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Opacity(opacity: (1.0 - this._fadeExtent), child: new global::Doroti.Framework.Widgets.Padding(padding: _animatedInsets(context, ((CupertinoSearchTextField)this.widget).suffixInsets), child: new CupertinoButton(onPressed: ((((CupertinoSearchTextField)this.widget).onSuffixTap ?? (global::System.Action)this._defaultOnSuffixTap)), minSize: 0, padding: global::Doroti.Framework.Painting.EdgeInsets.zero, child: new global::Doroti.Framework.Widgets.IconTheme(data: suffixIconThemeData, child: ((CupertinoSearchTextField)this.widget).suffixIcon)))));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new CupertinoTextField(controller: this._effectiveController, decoration: decorationLocal, style: ((CupertinoSearchTextField)this.widget).style, prefix: prefixLocal, suffix: suffixLocal, keyboardType: ((CupertinoSearchTextField)this.widget).keyboardType, onTap: ((CupertinoSearchTextField)this.widget).onTap, enabled: (((CupertinoSearchTextField)this.widget).enabled ?? true), cursorWidth: ((CupertinoSearchTextField)this.widget).cursorWidth, cursorHeight: ((CupertinoSearchTextField)this.widget).cursorHeight, cursorRadius: ((CupertinoSearchTextField)this.widget).cursorRadius, cursorOpacityAnimates: ((CupertinoSearchTextField)this.widget).cursorOpacityAnimates, cursorColor: ((CupertinoSearchTextField)this.widget).cursorColor, suffixMode: ((CupertinoSearchTextField)this.widget).suffixMode, placeholder: placeholderLocal, placeholderStyle: placeholderStyleLocal, padding: _animatedInsets(context, ((CupertinoSearchTextField)this.widget).padding), onChanged: ((CupertinoSearchTextField)this.widget).onChanged, onSubmitted: ((CupertinoSearchTextField)this.widget).onSubmitted, focusNode: this._effectiveFocusNode, autofocus: ((CupertinoSearchTextField)this.widget).autofocus, autocorrect: ((CupertinoSearchTextField)this.widget).autocorrect, smartQuotesType: ((CupertinoSearchTextField)this.widget).smartQuotesType, smartDashesType: ((CupertinoSearchTextField)this.widget).smartDashesType, enableIMEPersonalizedLearning: ((CupertinoSearchTextField)this.widget).enableIMEPersonalizedLearning, textInputAction: global::Doroti.Framework.Services.TextInputAction.search));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Services.RestorationBucket? bucket => this._bucket;
    public virtual void didToggleBucket(global::Doroti.Framework.Services.RestorationBucket? oldBucket)
    {
        DartRuntimePrimitives.Assert(() => (this._bucket?.isReplacing != true));
    }

    public virtual void registerForRestoration(dynamic property, string restorationId)
    {
        DartRuntimePrimitives.Assert(() => ((((dynamic)property)._restorationId is null) || ((this._debugDoingRestore && (((dynamic)property)._restorationId == restorationId)))), () => (object?)$"Property is already registered under {((dynamic)property)._restorationId}.");
        DartRuntimePrimitives.Assert(() => (this._debugDoingRestore || !this._properties.Keys.map<dynamic, string?>(((r) => ((dynamic)r)._restorationId)).contains(restorationId)), () => (object?)$"\"{restorationId}\" is already registered to another property.");
        bool hasSerializedValue = (this.bucket?.contains(restorationId) ?? false);
        object? initialValue = (hasSerializedValue ? property.fromPrimitives(this.bucket!.read<object>(restorationId)) : property.createDefaultValue());
        if (!((dynamic)property).isRegistered)
        {
            property._register(restorationId, this);
            void listener()
            {
                if ((this.bucket is null))
                {
                    return;
                }
                _updateProperty(property);
            }
            property.addListener((global::System.Action)listener);
            this._properties[property] = (global::System.Action)listener;
        }
        DartRuntimePrimitives.Assert(() => (((((dynamic)property)._restorationId == restorationId) && (object.Equals(((dynamic)property)._owner, this))) && this._properties.ContainsKey(property)));
        property.initWithValue((dynamic)initialValue);
        if (((!hasSerializedValue && ((dynamic)property).enabled) && (this.bucket is not null)))
        {
            _updateProperty(property);
        }
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugPropertiesWaitingForReregistration?.Remove(property);
                return true;
            });
    }

    public virtual void unregisterFromRestoration(dynamic property)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(((dynamic)property)._owner, this)));
        this._bucket?.remove<object?>(((dynamic)property)._restorationId!);
        _unregister(property);
    }

    public virtual void didUpdateRestorationId()
    {
        if ((((this._currentParent is null) || (this._bucket?.restorationId == this.restorationId)) || this.restorePending))
        {
            return;
        }
        global::Doroti.Framework.Services.RestorationBucket? oldBucket = this._bucket;
        DartRuntimePrimitives.Assert(() => !this.restorePending);
        bool didReplaceBucket = _updateBucketIfNecessary(parent: this._currentParent, restorePending: false);
        if (didReplaceBucket)
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(oldBucket, this._bucket)));
            DartRuntimePrimitives.Assert(() => ((this._bucket is null) || (oldBucket is null)));
            oldBucket?.dispose();
        }
    }

    public virtual bool restorePending
    {
        get
        {
            if (this._firstRestorePending)
            {
                return true;
            }
            if ((this.restorationId is null))
            {
                return false;
            }
            global::Doroti.Framework.Services.RestorationBucket? potentialNewParent = ((global::Doroti.Framework.Services.RestorationBucket?)(object?)RestorationScope.maybeOf(this.context));
            return ((!object.Equals(potentialNewParent, this._currentParent)) && ((potentialNewParent?.isReplacing ?? false)));
            return default!;
        }
    }
    public virtual bool _debugDoingRestore => DartRuntimePrimitives.ConvertValue<bool>((this._debugPropertiesWaitingForReregistration is not null));
    public virtual void _doRestore(global::Doroti.Framework.Services.RestorationBucket? oldBucket)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugPropertiesWaitingForReregistration = this._properties.Keys.ToList();
                return true;
            });
        restoreState(oldBucket, this._firstRestorePending);
        this._firstRestorePending = false;
        DartRuntimePrimitives.Assert(() =>
            {
                if (System.Linq.Enumerable.Any(this._debugPropertiesWaitingForReregistration!))
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(((Func<List<global::Doroti.Framework.Foundation.DiagnosticsNode>>)(() => { var __collection41817 = new List<global::Doroti.Framework.Foundation.DiagnosticsNode>(); __collection41817.Add(new global::Doroti.Framework.Foundation.ErrorSummary("Previously registered RestorableProperties must be re-registered in \"restoreState\".")); __collection41817.Add(new global::Doroti.Framework.Foundation.ErrorDescription($"The RestorableProperties with the following IDs were not re-registered to {this} when " + "\"restoreState\" was called:")); __collection41817.AddRange(this._debugPropertiesWaitingForReregistration!.map<dynamic, global::Doroti.Framework.Foundation.DiagnosticsNode>(((property) => new global::Doroti.Framework.Foundation.ErrorDescription($" * {((dynamic)property)._restorationId}")))); return __collection41817; }))()));
                }
                this._debugPropertiesWaitingForReregistration = null;
                return true;
            });
    }

    public virtual bool _updateBucketIfNecessary(global::Doroti.Framework.Services.RestorationBucket? parent, bool restorePending)
    {
        if (((this.restorationId is null) || (parent is null)))
        {
            bool didReplace = _setNewBucketIfNecessary(newBucket: null, restorePending: restorePending);
            DartRuntimePrimitives.Assert(() => (this._bucket is null));
            return didReplace;
        }
        DartRuntimePrimitives.Assert(() => (this.restorationId is not null));
        if ((restorePending || (this._bucket is null)))
        {
            global::Doroti.Framework.Services.RestorationBucket newBucketLocal = ((global::Doroti.Framework.Services.RestorationBucket)(object?)parent.claimChild(this.restorationId!, debugOwner: this));
            bool didReplaceLocal = _setNewBucketIfNecessary(newBucket: newBucketLocal, restorePending: restorePending);
            DartRuntimePrimitives.Assert(() => (object.Equals(this._bucket, newBucketLocal)));
            return didReplaceLocal;
        }
        DartRuntimePrimitives.Assert(() => (this._bucket is not null));
        DartRuntimePrimitives.Assert(() => !restorePending);
        this._bucket!.rename(this.restorationId!);
        parent.adoptChild(this._bucket!);
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _setNewBucketIfNecessary(global::Doroti.Framework.Services.RestorationBucket? newBucket, bool restorePending)
    {
        if ((object.Equals(newBucket, this._bucket)))
        {
            return false;
        }
        global::Doroti.Framework.Services.RestorationBucket? oldBucket = this._bucket;
        this._bucket = newBucket;
        if (!restorePending)
        {
            if ((this._bucket is not null))
            {
                this._properties.Keys.forEach((__arg0) => ((global::System.Action<dynamic>)this._updateProperty)(__arg0));
            }
            didToggleBucket(oldBucket);
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _updateProperty(dynamic property)
    {
        if (((dynamic)property).enabled)
        {
            this._bucket?.write(((dynamic)property)._restorationId!, property.toPrimitives());
        }
        else
        {
            this._bucket?.remove<object>(((dynamic)property)._restorationId!);
        }
    }

    public virtual void _unregister(dynamic property)
    {
        global::System.Action listener = this._properties.remove(property)!;
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugPropertiesWaitingForReregistration?.Remove(property);
                return true;
            });
        property.removeListener(listener);
        property._unregister();
    }

}
