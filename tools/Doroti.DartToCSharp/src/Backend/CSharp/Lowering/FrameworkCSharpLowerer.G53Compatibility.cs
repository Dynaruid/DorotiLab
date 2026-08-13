using System.Text.RegularExpressions;

namespace Doroti.DartToCSharp;

internal sealed partial class FrameworkCSharpLowerer
{
    /// <summary>
    /// Closes the small set of Dart-erasure boundaries exposed by the G5-3
    /// Widgets aggregate. These rewrites are deliberately library-scoped and
    /// line-preserving so source-map locations remain stable. They do not hide
    /// unsupported syntax; they adapt analyzer-resolved Dart covariance,
    /// optional value promotion, and callback arity to explicit CLR forms.
    /// </summary>
    private static string ApplyG53FrameworkCompatibility(string library, string source)
    {
        // A raw Dart Map/List type test accepts every element specialization. Do
        // not reintroduce CLR generic invariance when analyzer promotion leaves
        // object-typed collection casts in the promoted branch.
        source = Regex.Replace(
            source,
            @"\(\(DartMap<object, object>\)(?<value>[A-Za-z_][A-Za-z0-9_]*__as\d+)\)",
            "((System.Collections.IDictionary)${value})");
        source = Regex.Replace(
            source,
            @"\(\(List<object>\)(?<value>[A-Za-z_][A-Za-z0-9_]*__as\d+)\)",
            "((System.Collections.IList)${value})");

        // Dart generic types are covariant. Restoration mixes properties of
        // many value types, so the CLR-invariant RestorableProperty<object>
        // projection must stay dynamically typed at this boundary.
        source = source
            .Replace("global::Doroti.Generated.Framework.Widgets.RestorableProperty<object>", "dynamic", StringComparison.Ordinal)
            .Replace("RestorableProperty<object>", "dynamic", StringComparison.Ordinal);
        // Dart List.sublist(start) uses the remaining length. A one-argument
        // invocation can arrive without an argument-role node in older IR,
        // so structurally repair the legacy GetRange(start, list.Count)
        // emission while preserving any element covariance cast.
        source = Regex.Replace(
            source,
            @"(?<target>(?:this\.)?[A-Za-z_][A-Za-z0-9_]*)\.GetRange\((?<start>[^,\r\n]+), checked\(\(long\)\(\k<target>\.Count\)\)\)(?<cast>\.Cast<[^>]+>\(\))?",
            "${target}.Skip(checked((int)${start}))${cast}.ToList()");

        // WidgetState implements WidgetStatesConstraint in Dart, while the CLR
        // projection uses an explicit constraint wrapper. Map literals emitted
        // by button styleFrom methods therefore need their key type and entries
        // normalized before DartMap.cast executes at runtime.
        if (library.EndsWith("/elevated_button.dart", StringComparison.Ordinal) ||
            library.EndsWith("/filled_button.dart", StringComparison.Ordinal) ||
            library.EndsWith("/icon_button.dart", StringComparison.Ordinal) ||
            library.EndsWith("/outlined_button.dart", StringComparison.Ordinal) ||
            library.EndsWith("/text_button.dart", StringComparison.Ordinal))
        {
            source = source.Replace(
                "new DartMap<global::Doroti.Generated.Framework.Widgets.WidgetState, Color?>",
                "new DartMap<global::Doroti.Generated.Framework.Widgets.WidgetStatesConstraint, Color?>",
                StringComparison.Ordinal);
            foreach (var state in new[] { "pressed", "hovered", "focused" })
            {
                source = source.Replace(
                    $"[global::Doroti.Generated.Framework.Widgets.WidgetState.{state}] =",
                    $"[global::Doroti.Generated.Framework.Widgets.WidgetState.{state}.asConstraint()] =",
                    StringComparison.Ordinal);
            }
        }
        source = source
            .Replace("registerForRestoration(RestorableProperty<object> property", "registerForRestoration(dynamic property", StringComparison.Ordinal)
            .Replace("unregisterFromRestoration(RestorableProperty<object> property", "unregisterFromRestoration(dynamic property", StringComparison.Ordinal)
            .Replace("property.addListener(() => listener())", "property.addListener((global::System.Action)(() => listener()))", StringComparison.Ordinal)
            .Replace("property.removeListener(() => listener())", "property.removeListener((global::System.Action)(() => listener()))", StringComparison.Ordinal)
            .Replace("property.initWithValue(initialValue__", "property.initWithValue((dynamic)initialValue__", StringComparison.Ordinal);
        source = Regex.Replace(
            source,
            @"property\.removeListener\(\(\) => (listener__\d+)\(\)\)",
            "property.removeListener((global::System.Action)(() => $1()))");
        source = Regex.Replace(
            source,
            @"DartRuntimePrimitives\.ConvertValue<RestorableProperty<object>>\((this\.[A-Za-z0-9_]+)\)",
            "$1");

        if (library.EndsWith("/widgets/icon.dart", StringComparison.Ordinal))
        {
            // Dart's nullable List<Shadow> coalesce is an identity operation.
            // Collection covariance lowering used to append ToList(), which
            // dereferenced the valid null/no-shadow state before Icon could
            // render Material's built-in glyphs.
            source = Regex.Replace(
                source,
                @"(?<value>\(this\.shadows \?\? \(\(IconThemeData\)[^)]+\)\.shadows\))\.ToList\(\)",
                "${value}");
        }

        if (library.EndsWith("/widgets/scroll_view.dart", StringComparison.Ordinal))
        {
            // GridView's named generative constructors must initialize the
            // ScrollView base contract with their forwarded values. Calling
            // the primary constructor with positional default! values changes
            // the default vertical axis to Axis.horizontal, which transposes
            // CalendarDatePicker's seven-column day grid at runtime.
            source = source.Replace(
                "var __instance = new GridView(default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!);",
                "var __instance = new GridView(key: key, scrollDirection: scrollDirection, reverse: reverse, controller: controller, primary: primary, physics: physics, shrinkWrap: shrinkWrap, padding: padding, cacheExtent: cacheExtent, scrollCacheExtent: scrollCacheExtent, semanticChildCount: semanticChildCount, dragStartBehavior: dragStartBehavior, keyboardDismissBehavior: keyboardDismissBehavior, restorationId: restorationId, clipBehavior: clipBehavior, hitTestBehavior: hitTestBehavior);",
                StringComparison.Ordinal);
        }

        if (library.EndsWith("/widgets/scrollable.dart", StringComparison.Ordinal))
        {
            // A missing persisted scroll offset is null, not numeric zero.
            // Zero overwrites PageController.initialPage during restoration.
            source = source.Replace(
                "public override double? createDefaultValue() => DartRuntimePrimitives.ConvertValue<double>(null);",
                "public override double? createDefaultValue() => null;",
                StringComparison.Ordinal);
        }

        if (library.EndsWith("/widgets/basic.dart", StringComparison.Ordinal))
        {
            // The analyzer IR carries TextScaler.noScaling as a non-null Dart
            // default, while the CLR optional parameter uses default/null.
            // Normalize before the tuple-pattern switch so the exhaustive Dart
            // cases remain exhaustive when RichText is created by Icon.
            source = source.Replace(
                "    {\n        return ((textScaler, textScaleFactor) switch",
                "    {\n        textScaler ??= global::Doroti.Generated.Framework.Painting.TextScaler.noScaling;\n        return ((textScaler, textScaleFactor) switch",
                StringComparison.Ordinal);
        }

        if (library.EndsWith("/semantics/semantics.dart", StringComparison.Ordinal))
        {
            // Dart Map[] distinguishes a missing int entry from the value 0.
            // CLR GetValueOrDefault<long> cannot, so allocate custom semantics
            // identifiers through TryGetValue and keep both maps synchronized.
            source = Regex.Replace(
                source,
                @"long\? (?<result>result__\d+) = _ids\.GetValueOrDefault\(action\);\n        if \(\(\k<result> is null\)\)\n        \{\n            \k<result> = _nextId\+\+;\n            _ids\[DartRuntimePrimitives\.RequireReference\(action\)\] = DartRuntimePrimitives\.RequireValue\(\k<result>\);\n            _actions\[DartRuntimePrimitives\.RequireValue\(\k<result>\)\] = action;\n        \}\n        return DartRuntimePrimitives\.RequireValue\(\k<result>\);",
                "if (!_ids.TryGetValue(action, out var ${result}))\n        {\n            ${result} = _nextId++;\n            _ids[DartRuntimePrimitives.RequireReference(action)] = ${result};\n            _actions[${result}] = action;\n        }\n        return ${result};");
        }

        if (library.EndsWith("/material/checkbox.dart", StringComparison.Ordinal))
        {
            // The M2/M3 default classes override theme getters in Dart. Until
            // override inference is generalized, retain virtual dispatch at
            // their conditional join instead of reading nullable base storage.
            source = Regex.Replace(
                source,
                @"CheckboxThemeData (?<name>defaults__\d+) = \(Theme\.of\(context\)\.useMaterial3 \? new _CheckboxDefaultsM3__checkbox\(context\) : new _CheckboxDefaultsM2__checkbox\(context\)\);",
                "dynamic ${name} = (Theme.of(context).useMaterial3 ? new _CheckboxDefaultsM3__checkbox(context) : new _CheckboxDefaultsM2__checkbox(context));");
            source = Regex.Replace(
                source,
                @"(?<name>defaults__\d+)\.(?<property>fillColor|overlayColor|checkColor)!\.resolve\(",
                "((global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color>)${name}.${property}).resolve(");
        }

        if (library.EndsWith("/material/radio.dart", StringComparison.Ordinal))
        {
            source = Regex.Replace(
                source,
                @"RadioThemeData (?<name>defaults__\d+) = \(Theme\.of\(context\)\.useMaterial3 \? new _RadioDefaultsM3__radio\(context\) : new _RadioDefaultsM2__radio\(context\)\);",
                "dynamic ${name} = (Theme.of(context).useMaterial3 ? new _RadioDefaultsM3__radio(context) : new _RadioDefaultsM2__radio(context));");
            source = Regex.Replace(
                source,
                @"(?<name>defaults__\d+)\.(?<property>fillColor|backgroundColor|overlayColor)!\.resolve\(",
                "((global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color>)${name}.${property}).resolve(");
        }

        if (library.EndsWith("/material/switch.dart", StringComparison.Ordinal))
        {
            source = Regex.Replace(source,
                @"SwitchThemeData (?<name>defaults__\d+) = default!;",
                "dynamic ${name} = default!;");
            source = Regex.Replace(source,
                @"(?<name>defaults__\d+)\.(?<property>thumbColor|trackColor|trackOutlineColor|overlayColor)!?\.resolve\(",
                "((global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color>)${name}.${property}).resolve(");
            source = Regex.Replace(source,
                @"(?<name>defaults__\d+)\.trackOutlineWidth\?\.resolve\(",
                "((global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<double>)${name}.trackOutlineWidth).resolve(");
        }

        if (library.EndsWith("/material/text_field.dart", StringComparison.Ordinal))
        {
            // A nullable spread in Dart contributes zero elements. Preserve
            // that contract when collection literals lower to AddRange.
            source = Regex.Replace(source,
                @"\.AddRange\(\(\(TextField\)this\.widget\)\.inputFormatters\);",
                ".AddRange(((TextField)this.widget).inputFormatters ?? []);");
        }

        if (library.EndsWith("/material/navigation_bar.dart", StringComparison.Ordinal))
        {
            // C# captures the for-loop variable by reference, unlike Dart's
            // per-iteration binding. Snapshot the destination index before the
            // animated builder callback is retained.
            source = Regex.Replace(source,
                @"for \(long (?<index>i__\d+) = 0L; \([^\n]+\); \k<index>\+\+\) \{ (?<collection>__collection\d+)\.Add\(",
                "for (long ${index} = 0L; (${index} < checked((long)(this.destinations.Count))); ${index}++) { var destinationIndex__g65 = ${index}; ${collection}.Add(",
                RegexOptions.None);
            source = Regex.Replace(source,
                @"(?<prefix>selected: \(|isSelected: \(|index: |onTap: _handleTap\(|this\.destinations\[\(int\)\()i__\d+",
                "${prefix}destinationIndex__g65");
        }

        if (library.EndsWith("/animation/animation_controller.dart", StringComparison.Ordinal))
        {
            // Redirecting factory construction must not execute the generative
            // constructor with a null TickerProvider before factory fields are
            // assigned. Build the unbounded controller through the valid ctor.
            source = Regex.Replace(source,
                @"var (?<instance>__instance) = new AnimationController\(default!, default!, default!, default!, default!, default!, default!, default!\);\n        \k<instance>\.duration = duration;\n        \k<instance>\.reverseDuration = reverseDuration;\n        \k<instance>\.debugLabel = debugLabel;\n        \k<instance>\.animationBehavior = animationBehavior;\n        \k<instance>\.lowerBound = double\.NegativeInfinity;\n        \k<instance>\.upperBound = double\.PositiveInfinity;",
                "var ${instance} = new AnimationController(value: value, duration: duration, reverseDuration: reverseDuration, debugLabel: debugLabel, lowerBound: double.NegativeInfinity, upperBound: double.PositiveInfinity, animationBehavior: animationBehavior, vsync: vsync);");
            source = source
                .Replace("        __instance._ticker = vsync.createTicker(__instance._tick);\n", "", StringComparison.Ordinal)
                .Replace("        __instance._internalSetValue(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(value)));\n", "", StringComparison.Ordinal);
        }

        if (library.EndsWith("/rendering/table.dart", StringComparison.Ordinal))
        {
            source = source
                .Replace(
                    "if (((this._children[(int)(xyOld__29146)] is not null) &&",
                    "if (((xyOld__29146 < checked((long)(this._children.Count))) && (this._children[(int)(xyOld__29146)] is not null) &&",
                    StringComparison.Ordinal)
                .Replace(
                    "((y__29529 >= this._rows)) || (!object.Equals(this._children[(int)(xyOld__29683)]",
                    "((y__29529 >= this._rows)) || (xyOld__29683 >= checked((long)(this._children.Count))) || (!object.Equals(this._children[(int)(xyOld__29683)]",
                    StringComparison.Ordinal);
            source = Regex.Replace(
                source,
                @"(?<method>public virtual IEnumerable<RenderBox> (?:column|row)\(long [^)]+\)\s*\{.*?)(?:\s*throw new InvalidOperationException\(""Dart control flow completed without a value\.""\);)(?<close>\s*\})",
                "${method}${close}",
                RegexOptions.Singleline);
        }

        if (library.EndsWith("/painting/borders.dart", StringComparison.Ordinal))
        {
            source = source.Replace(
                "public static double strokeAlignInside = -1.0;",
                "public const double strokeAlignInside = -1.0;",
                StringComparison.Ordinal);
        }

        if (library.EndsWith("/material/material.dart", StringComparison.Ordinal))
        {
            source = source.Replace(
                "List<InkFeature>? inkFeatures__23989 = this._inkFeatures?.ToList();",
                "List<InkFeature>? inkFeatures__23989 = this._inkFeatures;",
                StringComparison.Ordinal);
        }

        if (library.EndsWith("ink_well.dart", StringComparison.Ordinal))
        {
            source = source.Replace(
                "value == (((this._highlights.ContainsKey(type)) && this._highlights.GetValueOrDefault(type)!.active))",
                "value == ((this._highlights.GetValueOrDefault(type) is InkHighlight currentHighlight) && currentHighlight.active)",
                StringComparison.Ordinal);
        }

        if (library.EndsWith("/gestures/hit_test.dart", StringComparison.Ordinal))
        {
            source = source
                .Replace(
                    "double? epsilon = null)",
                    "double epsilon = 1e-10)",
                    StringComparison.Ordinal)
                .Replace(
                    "component.abs() < DartRuntimePrimitives.RequireValue(epsilon)",
                    "component.abs() < epsilon",
                    StringComparison.Ordinal);
        }

        if (library.EndsWith("/rendering/sliver_multi_box_adaptor.dart", StringComparison.Ordinal))
        {
            source = source.Replace(
                "        this._childManager = childManager;\n    }",
                "        this._childManager = childManager;\n        this._debugDanglingKeepAlives = new List<RenderBox>();\n    }",
                StringComparison.Ordinal);
        }

        if (library.EndsWith("/widgets/binding.dart", StringComparison.Ordinal))
        {
            // Dart mixin initInstances bodies form one super-call chain. The
            // flattened C# WidgetsFlutterBinding type must retain the Semantics
            // and Renderer initialization that sits between Services and
            // Widgets in that chain.
            source = source.Replace(
                "        WidgetsBinding._instance = this;\n        this._buildOwner = new BuildOwner();",
                """
                        WidgetsBinding._instance = this;
                        this._accessibilityFeatures = this.platformDispatcher.accessibilityFeatures;
                        this.platformDispatcher.onSemanticsEnabledChanged = this._handleSemanticsEnabledChanged;
                        this.platformDispatcher.onSemanticsActionEvent = this._handleSemanticsActionEvent;
                        this.platformDispatcher.onAccessibilityFeaturesChanged = () =>
                        {
                            if (global::Doroti.Generated.Framework.Scheduler.SchedulerBinding.instance.schedulerPhase ==
                                global::Doroti.Generated.Framework.Scheduler.SchedulerPhase.persistentCallbacks)
                            {
                                global::Doroti.Generated.Framework.Scheduler.SchedulerBinding.instance.addPostFrameCallback(
                                    _ => handleAccessibilityFeaturesChanged(),
                                    debugLabel: "SemanticsBinding.handleAccessibilityFeaturesChanged");
                            }
                            else
                            {
                                handleAccessibilityFeaturesChanged();
                            }
                        };
                        _handleSemanticsEnabledChanged();
                        addSemanticsEnabledListener(this._handleFrameworkSemanticsEnabledChanged);
                        if (this.semanticsEnabled)
                        {
                            _handleFrameworkSemanticsEnabledChanged();
                        }
                        this._buildOwner = new BuildOwner();
                """,
                StringComparison.Ordinal);
            source = source.Replace(
                "        DartRuntimePrimitives.Ignore(PaintingBinding.shaderWarmUp?.execute());\n    }",
                """
                        DartRuntimePrimitives.Ignore(PaintingBinding.shaderWarmUp?.execute());
                        this._rootPipelineOwner = createRootPipelineOwner();
                        this.platformDispatcher.onMetricsChanged = _ => handleMetricsChanged();
                        this.platformDispatcher.onTextScaleFactorChanged = handleTextScaleFactorChanged;
                        this.platformDispatcher.onPlatformBrightnessChanged = handlePlatformBrightnessChanged;
                        addPersistentFrameCallback(this._handlePersistentFrameCallback);
                        initMouseTracker();
                        this.rootPipelineOwner.attach(this._manifold);
                    }
                """,
                StringComparison.Ordinal);
            source = source.Replace(
                "public virtual global::Doroti.Flutter.Ui.SceneBuilder createSceneBuilder() => DartRuntimePrimitives.ConvertValue<global::Doroti.Flutter.Ui.SceneBuilder>(new global::Doroti.Flutter.Ui.SceneBuilder());",
                """
                public virtual global::Doroti.Flutter.Ui.SceneBuilder createSceneBuilder() =>
                    new global::Doroti.Flutter.Ui.SceneBuilder(
                        this.platformDispatcher.implicitView?.viewId ??
                        throw new InvalidOperationException("Rendering a scene requires an implicit Flutter view."));
                """,
                StringComparison.Ordinal);
        }

        if (library.EndsWith("/rendering/proxy_box.dart", StringComparison.Ordinal))
        {
            // RenderSemanticsAnnotations applies SemanticsAnnotationsMixin.
            // Flattening the mixin must retain its constructor initializer or
            // the first semantics pass observes a null properties object.
            source = source.Replace(
                """
                    public RenderSemanticsAnnotations(RenderBox? child = null, global::Doroti.Generated.Framework.Semantics.SemanticsProperties properties = default!, bool container = false, bool explicitChildNodes = false, bool excludeSemantics = false, bool blockUserActions = false, Locale? localeForSubtree = null, TextDirection? textDirection = null) : base(child)
                    {
                    }
                """,
                """
                    public RenderSemanticsAnnotations(RenderBox? child = null, global::Doroti.Generated.Framework.Semantics.SemanticsProperties properties = default!, bool container = false, bool explicitChildNodes = false, bool excludeSemantics = false, bool blockUserActions = false, Locale? localeForSubtree = null, TextDirection? textDirection = null) : base(child)
                    {
                        initSemanticsAnnotations(
                            properties,
                            container,
                            explicitChildNodes,
                            excludeSemantics,
                            blockUserActions,
                            localeForSubtree,
                            textDirection);
                    }
                """,
                StringComparison.Ordinal);
        }

        if (library.EndsWith("/widgets/localizations.dart", StringComparison.Ordinal))
        {
            // Dart Future<T> is covariant while the CLR generic is invariant.
            // Use the non-generic Future bridge and project its result instead
            // of runtime-casting Future<WidgetsLocalizations> to Future<object>.
            source = Regex.Replace(
                source,
                @"Future<object> (inputValue__\d+) = \(\(Future<object>\)\(object\?\)\(\(Future<object>\)\(\(dynamic\)([^)]+)\)\.load\(locale\)\)\);",
                "Future $1 = (Future)((dynamic)$2).load(locale);");
            source = Regex.Replace(
                source,
                @"Future<object> (futureValue__\d+) = (inputValue__\d+)\.then\(\(global::System\.Func<object, dynamic>\)\(\(value\) => \{",
                "Future<object> $1 = $2.then<object>((object? value) => {");
        }

        if (library.Contains("/cupertino/", StringComparison.Ordinal))
        {
            source = source
                .Replace("DateTime.monday", "1L", StringComparison.Ordinal)
                .Replace("DateTime.daysPerWeek", "7L", StringComparison.Ordinal)
                .Replace("registerForRestoration(RestorableProperty<object> property", "registerForRestoration(dynamic property", StringComparison.Ordinal)
                .Replace("unregisterFromRestoration(RestorableProperty<object> property", "unregisterFromRestoration(dynamic property", StringComparison.Ordinal)
                .Replace("property.addListener(() => listener())", "property.addListener((global::System.Action)(() => listener()))", StringComparison.Ordinal)
                .Replace(".indexed", ".indexed()", StringComparison.Ordinal)
                .Replace(".sum", ".sum()", StringComparison.Ordinal);
        }

        if (library.EndsWith("/cupertino/adaptive_text_selection_toolbar.dart", StringComparison.Ordinal))
        {
            source = Regex.Replace(source,
                @"if \(\(\(\(\(this\.children \?\? this\.buttonItems\)\) is \{ \} __items\d+ \? !System\.Linq\.Enumerable\.Any\(__items\d+\) : \(bool\?\)null\) \?\? true\)\)",
                "if (this.children is null ? !this.buttonItems.Any() : !this.children.Any())");
        }

        if (library.EndsWith("/cupertino/app.dart", StringComparison.Ordinal))
        {
            source = source
                .Replace("_exitWidgetSelectionButtonBuilder(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::System.Action onPressed, string semanticsLabel, global::Doroti.Generated.Framework.Widgets.GlobalKey<IState> key)", "_exitWidgetSelectionButtonBuilder(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Widgets.GlobalKey<IState> key, global::System.Action onPressed, string semanticsLabel)", StringComparison.Ordinal)
                .Replace("_tapBehaviorButtonBuilder(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::System.Action onPressed, string semanticsLabel, bool selectionOnTapEnabled)", "_tapBehaviorButtonBuilder(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::System.Action onPressed, bool selectionOnTapEnabled, string semanticsLabel)", StringComparison.Ordinal)
                .Replace("return new CupertinoPageRoute<CupertinoApp>(settings: settings, builder:", "return new CupertinoPageRoute<object>(settings: settings, builder:", StringComparison.Ordinal);
        }

        if (library.EndsWith("/material/app.dart", StringComparison.Ordinal))
        {
            source = source.Replace(
                "return (global::Doroti.Generated.Framework.Widgets.Route<object>)(object)new MaterialPageRoute<MaterialApp>(settings: settings, builder:",
                "return new MaterialPageRoute<object>(settings: settings, builder:",
                StringComparison.Ordinal);
        }

        if (library.EndsWith("/cupertino/bottom_tab_bar.dart", StringComparison.Ordinal))
        {
            source = source.Replace("BorderSide(color: _kDefaultTabBarBorderColor", "new global::Doroti.Generated.Framework.Painting.BorderSide(color: Bottom_tab_barLibrary._kDefaultTabBarBorderColor", StringComparison.Ordinal);
        }

        if (library.EndsWith("/cupertino/button.dart", StringComparison.Ordinal) ||
            library.EndsWith("/cupertino/checkbox.dart", StringComparison.Ordinal) ||
            library.EndsWith("/cupertino/radio.dart", StringComparison.Ordinal) ||
            library.EndsWith("/cupertino/switch.dart", StringComparison.Ordinal) ||
            library.EndsWith("/cupertino/menu_anchor.dart", StringComparison.Ordinal))
        {
            const string cursorResolver = "global::System.Func<HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>, global::Doroti.Generated.Framework.Services.MouseCursor>";
            source = source
                .Replace("WidgetStateProperty.resolveWith<global::Doroti.Generated.Framework.Services.MouseCursor>(((global::System.Func<HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>, CupertinoButton>)", $"WidgetStateProperty.resolveWith<global::Doroti.Generated.Framework.Services.MouseCursor>((({cursorResolver})", StringComparison.Ordinal)
                .Replace("WidgetStateProperty.resolveWith<global::Doroti.Generated.Framework.Services.MouseCursor>(((global::System.Func<HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>, CupertinoCheckbox>)", $"WidgetStateProperty.resolveWith<global::Doroti.Generated.Framework.Services.MouseCursor>((({cursorResolver})", StringComparison.Ordinal)
                .Replace("WidgetStateProperty.resolveWith<global::Doroti.Generated.Framework.Services.MouseCursor>(((global::System.Func<HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>, T>)", $"WidgetStateProperty.resolveWith<global::Doroti.Generated.Framework.Services.MouseCursor>((({cursorResolver})", StringComparison.Ordinal)
                .Replace("_defaultMouseCursor => WidgetStateProperty.resolveWith(((global::System.Func<HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>, CupertinoSwitch>)", $"_defaultMouseCursor => WidgetStateProperty.resolveWith((({cursorResolver})", StringComparison.Ordinal)
                .Replace("WidgetStateProperty.resolveWith<global::Doroti.Generated.Framework.Services.MouseCursor>(((global::System.Func<HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>, T>)", $"WidgetStateProperty.resolveWith<global::Doroti.Generated.Framework.Services.MouseCursor>((({cursorResolver})", StringComparison.Ordinal);
            source = source
                .Replace("return ((CupertinoButton)(object?)((!states.Contains", "return ((global::Doroti.Generated.Framework.Services.MouseCursor)(object?)((!states.Contains", StringComparison.Ordinal)
                .Replace("return ((CupertinoCheckbox)(object?)((!states.Contains", "return ((global::Doroti.Generated.Framework.Services.MouseCursor)(object?)((!states.Contains", StringComparison.Ordinal)
                .Replace("return ((CupertinoSwitch)(object?)global::Doroti.Generated.Framework.Services.MouseCursor.defer)", "return global::Doroti.Generated.Framework.Services.MouseCursor.defer", StringComparison.Ordinal)
                .Replace("return ((CupertinoSwitch)(object?)(global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kIsWeb", "return ((global::Doroti.Generated.Framework.Services.MouseCursor)(object?)(global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kIsWeb", StringComparison.Ordinal)
                .Replace("return ((T)(object?)((!states.Contains", "return ((global::Doroti.Generated.Framework.Services.MouseCursor)(object?)((!states.Contains", StringComparison.Ordinal);
        }

        if (library.EndsWith("/cupertino/checkbox.dart", StringComparison.Ordinal) || library.EndsWith("/cupertino/switch.dart", StringComparison.Ordinal))
        {
            source = source
                .Replace("global::Doroti.Generated.Framework.Rendering.CustomPainter painter = default!", "dynamic painter = default!", StringComparison.Ordinal)
                .Replace("new Semantics(", "new global::Doroti.Generated.Framework.Widgets.Semantics(", StringComparison.Ordinal);
        }

        if (library.EndsWith("/cupertino/colors.dart", StringComparison.Ordinal))
        {
            source = source.Replace("=> this.darkHighContrastElevatedColor }))", "=> this.darkHighContrastElevatedColor, _ => this.color }))", StringComparison.Ordinal);
        }

        if (library.EndsWith("/cupertino/context_menu.dart", StringComparison.Ordinal))
        {
            source = source
                .Replace("global::Doroti.Generated.Framework.Widgets.Visibility.Create(key:", "new global::Doroti.Generated.Framework.Widgets.Visibility(key:", StringComparison.Ordinal)
                .Replace("global::Doroti.Generated.Framework.Widgets.OrientationBuilder.Create(builder:", "new global::Doroti.Generated.Framework.Widgets.OrientationBuilder(builder:", StringComparison.Ordinal)
                .Replace("_ContextMenuSheet__context_menu.Create(key:", "new _ContextMenuSheet__context_menu(key:", StringComparison.Ordinal)
                .Replace("global::Doroti.Generated.Framework.Widgets.Transform.Create(key:", "global::Doroti.Generated.Framework.Widgets.Transform.CreateScale(key:", StringComparison.Ordinal)
                .Replace("global::Doroti.Generated.Framework.Widgets.TickerMode.Create(enabled:", "new global::Doroti.Generated.Framework.Widgets.TickerMode(enabled:", StringComparison.Ordinal)
                .Replace("_ContextMenuRouteStatic__context_menu.Create(actions:", "new _ContextMenuRouteStatic__context_menu(actions:", StringComparison.Ordinal)
                .Replace("global::Doroti.Generated.Framework.Widgets.MouseRegion.Create(cursor:", "new global::Doroti.Generated.Framework.Widgets.MouseRegion(cursor:", StringComparison.Ordinal)
                .Replace("global::Doroti.Generated.Framework.Widgets.Listener.Create(onPointerDown:", "new global::Doroti.Generated.Framework.Widgets.Listener(onPointerDown:", StringComparison.Ordinal)
                .Replace("global::Doroti.Generated.Framework.Widgets.AnimatedBuilder.Create(animation:", "new global::Doroti.Generated.Framework.Widgets.AnimatedBuilder(animation:", StringComparison.Ordinal)
                .Replace("global::Doroti.Generated.Framework.Widgets.FadeTransition.Create(opacity:", "new global::Doroti.Generated.Framework.Widgets.FadeTransition(opacity:", StringComparison.Ordinal)
                .Replace("global::Doroti.Generated.Framework.Widgets.Transform.Create(alignment:", "global::Doroti.Generated.Framework.Widgets.Transform.CreateScale(alignment:", StringComparison.Ordinal)
                .Replace("global::Doroti.Generated.Framework.Widgets.Transform.Create(key:", "global::Doroti.Generated.Framework.Widgets.Transform.CreateScale(key:", StringComparison.Ordinal)
                .Replace("global::Doroti.Generated.Framework.Widgets.Positioned.Create(rect:", "global::Doroti.Generated.Framework.Widgets.Positioned.CreateFromRect(rect:", StringComparison.Ordinal)
                .Replace("global::Doroti.Generated.Framework.Widgets.Positioned.Create(key:", "global::Doroti.Generated.Framework.Widgets.Positioned.CreateFromRect(key:", StringComparison.Ordinal)
                .Replace("_sheetScaleTween.end = this._scale;", "_sheetScaleTween.end = DartRuntimePrimitives.RequireValue(this._scale);", StringComparison.Ordinal);
        }

        if (library.EndsWith("/cupertino/cupertino_focus_halo.dart", StringComparison.Ordinal))
        {
            source = source
                .Replace("(arg0) => new global::Doroti.Generated.Framework.Painting.RoundedRectangleBorder(arg0)", "(borderRadius, side) => new global::Doroti.Generated.Framework.Painting.RoundedRectangleBorder(side: side, borderRadius: borderRadius)", StringComparison.Ordinal)
                .Replace("(arg0) => new global::Doroti.Generated.Framework.Painting.RoundedSuperellipseBorder(arg0)", "(borderRadius, side) => new global::Doroti.Generated.Framework.Painting.RoundedSuperellipseBorder(side: side, borderRadius: borderRadius)", StringComparison.Ordinal)
                .Replace("this.widget._shapeBuilder(side: ", "this.widget._shapeBuilder(", StringComparison.Ordinal)
                .Replace(", borderRadius: ((CupertinoFocusHalo)this.widget)._borderRadius)", ", ((CupertinoFocusHalo)this.widget)._borderRadius)", StringComparison.Ordinal);
            source = source.Replace("this.widget._shapeBuilder((this._childHasFocus", "this.widget._shapeBuilder(((CupertinoFocusHalo)this.widget)._borderRadius, (this._childHasFocus", StringComparison.Ordinal)
                .Replace(", ((CupertinoFocusHalo)this.widget)._borderRadius)", ")", StringComparison.Ordinal);
        }

        if (library.EndsWith("/cupertino/date_picker.dart", StringComparison.Ordinal))
        {
            source = source.Replace("._lastDayInMonth(", "._lastDayInMonth(", StringComparison.Ordinal).Replace(").day", ").Day", StringComparison.Ordinal);
        }

        if (library.EndsWith("/cupertino/expansion_tile.dart", StringComparison.Ordinal))
        {
            source = source.Replace("onTap: this._onHeaderTap", "onTap: () => { this._onHeaderTap(); return null!; }", StringComparison.Ordinal);
        }

        if (library.EndsWith("/cupertino/list_section.dart", StringComparison.Ordinal))
        {
            source = source.Replace("decoration: (((object?)this.decoration ?? (object?)new global::Doroti.Generated.Framework.Painting.ShapeDecoration", "decoration: DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Painting.Decoration>((object?)this.decoration ?? (object?)new global::Doroti.Generated.Framework.Painting.ShapeDecoration", StringComparison.Ordinal)
                .Replace(")))), child: new global::Doroti.Generated.Framework.Widgets.Column", "))), child: new global::Doroti.Generated.Framework.Widgets.Column", StringComparison.Ordinal);
        }

        if (library.EndsWith("/cupertino/localizations.dart", StringComparison.Ordinal))
        {
            source = source.Replace("DateTime.january", "1L", StringComparison.Ordinal);
        }

        if (library.EndsWith("/cupertino/menu_anchor.dart", StringComparison.Ordinal))
        {
            const string widthMembers = """

internal static class _CupertinoMenuWidth__menu_anchorMembers
{
    internal static double points(this _CupertinoMenuWidth__menu_anchor value) => value switch
    {
        _CupertinoMenuWidth__menu_anchor.iPadOS => 262.0,
        _CupertinoMenuWidth__menu_anchor.iPadOSAccessible => 343.0,
        _CupertinoMenuWidth__menu_anchor.iOS => 250.0,
        _ => 370.0,
    };

    internal static _CupertinoMenuWidth__menu_anchor CreateFromScreenWidth(bool isLargeTextModeEnabled, double screenWidth) =>
        screenWidth >= 768.0
            ? (isLargeTextModeEnabled ? _CupertinoMenuWidth__menu_anchor.iPadOSAccessible : _CupertinoMenuWidth__menu_anchor.iPadOS)
            : (isLargeTextModeEnabled ? _CupertinoMenuWidth__menu_anchor.iOSAccessible : _CupertinoMenuWidth__menu_anchor.iOS);
}
""";
            const string dynamicMembers = """
internal static class _DynamicTypeStyle__menu_anchorMembers
{
    private const long _kScaleCount = 12;
    private static readonly List<long> _normalizedBodyScales = new() { -3, -2, -1, 0, 2, 4, 6, 11, 16, 23, 30, 36 };
    private static readonly double[] _bodySizes = { 14, 15, 16, 17, 19, 21, 23, 28, 33, 40, 47, 53 };
    private static readonly double[] _subheadSizes = { 12, 13, 14, 15, 17, 19, 21, 26, 31, 38, 45, 51 };
    private static List<global::Doroti.Generated.Framework.Painting.TextStyle> styles(this _DynamicTypeStyle__menu_anchor value) =>
        (value == _DynamicTypeStyle__menu_anchor.body ? _bodySizes : _subheadSizes)
            .Select(size => new global::Doroti.Generated.Framework.Painting.TextStyle(fontSize: size)).ToList();
    private static double _interpolateUnits(double value, double min, double max) => (value - min) / (max - min);
""";
            source = source
                .Replace("\ninternal enum _DynamicTypeStyle__menu_anchor", widthMembers + "\ninternal enum _DynamicTypeStyle__menu_anchor", StringComparison.Ordinal)
                .Replace("internal static class _DynamicTypeStyle__menu_anchorMembers\n{", dynamicMembers, StringComparison.Ordinal)
                .Replace("value.styles", "value.styles()", StringComparison.Ordinal)
                .Replace("_DynamicTypeStyle__menu_anchor._interpolateUnits", "_interpolateUnits", StringComparison.Ordinal)
                .Replace("_CupertinoMenuWidth__menu_anchor.CreateFromScreenWidth", "_CupertinoMenuWidth__menu_anchorMembers.CreateFromScreenWidth", StringComparison.Ordinal)
                .Replace(").points", ").points()", StringComparison.Ordinal)
                .Replace("[global::Doroti.Generated.Framework.Widgets.WidgetState.dragged]", "[global::Doroti.Generated.Framework.Widgets.WidgetState.dragged.asConstraint()]", StringComparison.Ordinal)
                .Replace("[global::Doroti.Generated.Framework.Widgets.WidgetState.pressed]", "[global::Doroti.Generated.Framework.Widgets.WidgetState.pressed.asConstraint()]", StringComparison.Ordinal)
                .Replace("[global::Doroti.Generated.Framework.Widgets.WidgetState.focused]", "[global::Doroti.Generated.Framework.Widgets.WidgetState.focused.asConstraint()]", StringComparison.Ordinal)
                .Replace("[global::Doroti.Generated.Framework.Widgets.WidgetState.hovered]", "[global::Doroti.Generated.Framework.Widgets.WidgetState.hovered.asConstraint()]", StringComparison.Ordinal)
                .Replace("[global::Doroti.Generated.Framework.Widgets.WidgetState.any]", "[global::Doroti.Generated.Framework.Widgets.WidgetStateMembers.any]", StringComparison.Ordinal)
                .Replace("(global::System.Action<bool?>)this._handleFocusChange", "(global::System.Action<bool>)(value => this._handleFocusChange(value))", StringComparison.Ordinal);
        }

        if (library.EndsWith("/cupertino/nav_bar.dart", StringComparison.Ordinal))
        {
            source = source
                .Replace("CupertinoSheetRoute.hasParentSheet", "CupertinoSheetRoute<object>.hasParentSheet", StringComparison.Ordinal)
                .Replace("userBottom: ((CupertinoNavigationBar)this.widget).bottom", "userBottom: (global::Doroti.Generated.Framework.Widgets.Widget?)(object?)((CupertinoNavigationBar)this.widget).bottom", StringComparison.Ordinal)
                .Replace(": ((CupertinoSliverNavigationBar)this.widget).bottom))", ": (global::Doroti.Generated.Framework.Widgets.Widget?)(object?)((CupertinoSliverNavigationBar)this.widget).bottom))", StringComparison.Ordinal);
        }

        if (library.EndsWith("/cupertino/page_scaffold.dart", StringComparison.Ordinal))
        {
            source = source.Replace("        base.handleStatusBarTap();\n", string.Empty, StringComparison.Ordinal);
        }

        if (library.EndsWith("/cupertino/refresh.dart", StringComparison.Ordinal))
        {
            source = source
                .Replace("nextState__19800 = RefreshIndicatorMode.drag;\n                    }\n                    continue;", "nextState__19800 = RefreshIndicatorMode.drag;\n                    }\n                    goto case RefreshIndicatorMode.drag;", StringComparison.Ordinal)
                .Replace("goToDone();\n                        continue;", "goToDone();\n                        goto case RefreshIndicatorMode.done;", StringComparison.Ordinal)
                .Replace("nextState__19800 = RefreshIndicatorMode.refresh;\n                    }\n                    continue;", "nextState__19800 = RefreshIndicatorMode.refresh;\n                    }\n                    goto case RefreshIndicatorMode.refresh;", StringComparison.Ordinal)
                .Replace("goToDone();\n                    }\n                    continue;", "goToDone();\n                    }\n                    goto case RefreshIndicatorMode.done;", StringComparison.Ordinal);
        }

        if (library.EndsWith("/cupertino/route.dart", StringComparison.Ordinal))
        {
            source = source
                .Replace("CupertinoRouteTransitionMixin.kTransitionDuration", "CupertinoRouteTransitionMixin<object>.kTransitionDuration", StringComparison.Ordinal)
                .Replace("Page._defaultPopInvokedHandler", "((didPop, result) => Page<object>._defaultPopInvokedHandler(didPop, result))", StringComparison.Ordinal);
        }

        if (library.EndsWith("/cupertino/segmented_control.dart", StringComparison.Ordinal) || library.EndsWith("/cupertino/sliding_segmented_control.dart", StringComparison.Ordinal))
        {
            source = source.Replace("disabledChildren ?? new DartMap<object, object>()", "disabledChildren ?? new HashSet<T>()", StringComparison.Ordinal);
        }

        if (library.EndsWith("/cupertino/sliding_segmented_control.dart", StringComparison.Ordinal))
        {
            source = source.Replace("global::Doroti.Generated.Framework.Painting.BorderRadius.CreateAll(DialogLibrary._kCornerRadius)", "global::Doroti.Generated.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Flutter.Ui.Radius.circular(DialogLibrary._kCornerRadius))", StringComparison.Ordinal);
        }

        if (library.EndsWith("/cupertino/sheet.dart", StringComparison.Ordinal))
        {
            source = source.Replace("NavigatorPopHandler<object>(onPopWithResult: ((global::System.Action<T?>)", "NavigatorPopHandler<T>(onPopWithResult: ((global::System.Action<T?>)", StringComparison.Ordinal);
        }

        if (library.EndsWith("/cupertino/spell_check_suggestions_toolbar.dart", StringComparison.Ordinal))
        {
            source = source.Replace("new CupertinoTextSelectionToolbarButton(buttonItem:", "CupertinoTextSelectionToolbarButton.CreateButtonItem(buttonItem:", StringComparison.Ordinal);
        }

        if (library.EndsWith("/cupertino/text_field.dart", StringComparison.Ordinal))
        {
            source = source
                .Replace("new CupertinoSpellCheckSuggestionsToolbar(editableTextState:", "CupertinoSpellCheckSuggestionsToolbar.CreateEditableText(editableTextState:", StringComparison.Ordinal)
                .Replace("(this._controller?.value.text.Length != 0) ?? false", "this._controller is not null && this._controller.value.text.Length != 0", StringComparison.Ordinal)
                .Replace("global::Doroti.Generated.Framework.Widgets.Semantics.Create(button:", "new global::Doroti.Generated.Framework.Widgets.Semantics(button:", StringComparison.Ordinal);
            source = source.Replace("global::Doroti.Generated.Framework.Widgets.GestureDetector.Create(key:", "new global::Doroti.Generated.Framework.Widgets.GestureDetector(key:", StringComparison.Ordinal);
        }

        if (library.EndsWith("/cupertino/radio.dart", StringComparison.Ordinal))
        {
            source = source.Replace("return ((T)(object?)(WidgetStateProperty.resolveAs<global::Doroti.Generated.Framework.Services.MouseCursor?>", "return ((global::Doroti.Generated.Framework.Services.MouseCursor)(object?)(WidgetStateProperty.resolveAs<global::Doroti.Generated.Framework.Services.MouseCursor?>", StringComparison.Ordinal);
        }

        if (library.EndsWith("/cupertino/checkbox.dart", StringComparison.Ordinal))
        {
            source = source.Replace("return ((CupertinoCheckbox)(object?)(WidgetStateProperty.resolveAs<global::Doroti.Generated.Framework.Services.MouseCursor?>", "return ((global::Doroti.Generated.Framework.Services.MouseCursor)(object?)(WidgetStateProperty.resolveAs<global::Doroti.Generated.Framework.Services.MouseCursor?>", StringComparison.Ordinal);
        }

        if (library.EndsWith("/cupertino/switch.dart", StringComparison.Ordinal))
        {
            source = source
                .Replace("((CupertinoSwitch)this.widget).applyTheme ?? theme__22457.applyThemeToAll)) ?", "((CupertinoSwitch)this.widget).applyTheme ?? theme__22457.applyThemeToAll) == true) ?", StringComparison.Ordinal)
                .Replace(".createBoxPainter(() => this._handleDecorationChanged())", ".createBoxPainter((global::System.Action)(() => this._handleDecorationChanged()))", StringComparison.Ordinal);
        }

        if (library.EndsWith("/cupertino/text_selection_toolbar.dart", StringComparison.Ordinal))
        {
            source = source
                .Replace("((global::Doroti.Flutter.Ui.Paint)(object?)_debugPaint ??=", "(_debugPaint ??=", StringComparison.Ordinal)
                .Replace("public override global::Doroti.Generated.Framework.Rendering.RenderObject renderObject => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Rendering.RenderObject>", "public override _RenderCupertinoTextSelectionToolbarItems__text_selection_toolbar renderObject => DartRuntimePrimitives.ConvertValue<_RenderCupertinoTextSelectionToolbarItems__text_selection_toolbar>", StringComparison.Ordinal);
        }
        if (library.EndsWith("/cupertino/colors.dart", StringComparison.Ordinal))
        {
            source = source
                .Replace("ToString(\"color\", this.color)", "toString(\"color\", this.color)", StringComparison.Ordinal)
                .Replace("? resolvable.resolveFrom(context) : resolvable", "? ((CupertinoDynamicColor)resolvable).resolveFrom(context) : resolvable", StringComparison.Ordinal);
        }

        if (library.EndsWith("/cupertino/theme.dart", StringComparison.Ordinal))
        {
            source = source.Replace(
                ") : this(brightness, primaryColor, primaryContrastingColor, textTheme, barBackgroundColor, scaffoldBackgroundColor, selectionHandleColor, applyThemeToAll)\n",
                ") : this(brightness, primaryColor, primaryContrastingColor, textTheme, barBackgroundColor, scaffoldBackgroundColor, selectionHandleColor, applyThemeToAll, ThemeLibrary._kDefaultTheme)\n",
                StringComparison.Ordinal);
        }

        if (library.EndsWith("/cupertino/text_selection.dart", StringComparison.Ordinal))
        {
            // TextSelectionHandleControls was promoted as a concrete reviewed
            // Widgets type. Dart can still apply it after a concrete Cupertino
            // superclass; CLR cannot inherit both classes, so retain the
            // Cupertino implementation and inline the mixin donor members.
            source = source
                .Replace(", global::Doroti.Generated.Framework.Widgets.TextSelectionHandleControls", string.Empty, StringComparison.Ordinal)
                .Replace("public override void handleCut(", "public virtual void handleCut(", StringComparison.Ordinal)
                .Replace("public override void handleCopy(", "public virtual void handleCopy(", StringComparison.Ordinal);
        }

        if (library.EndsWith("/cupertino/desktop_text_selection.dart", StringComparison.Ordinal))
        {
            source = source
                .Replace(", global::Doroti.Generated.Framework.Widgets.TextSelectionHandleControls", string.Empty, StringComparison.Ordinal)
                .Replace("public override void handleCut(", "public virtual void handleCut(", StringComparison.Ordinal)
                .Replace("public override void handleCopy(", "public virtual void handleCopy(", StringComparison.Ordinal);
        }

        if (library.EndsWith("/cupertino/nav_bar.dart", StringComparison.Ordinal))
        {
            source = source
                .Replace(
                    "public override global::Doroti.Generated.Framework.Rendering.OverScrollHeaderStretchConfiguration? stretchConfiguration { get; set; } = default;",
                    "private global::Doroti.Generated.Framework.Rendering.OverScrollHeaderStretchConfiguration? __field_stretchConfiguration = default;\n    public override global::Doroti.Generated.Framework.Rendering.OverScrollHeaderStretchConfiguration? stretchConfiguration => __field_stretchConfiguration;",
                    StringComparison.Ordinal)
                .Replace("this.stretchConfiguration = stretchConfiguration;", "this.__field_stretchConfiguration = stretchConfiguration;", StringComparison.Ordinal);
        }

        if (library.EndsWith("/cupertino/text_field.dart", StringComparison.Ordinal))
        {
            source = source.Replace(
                "internal enum _BaselineAlignedStackSlot__text_field",
                "public enum _BaselineAlignedStackSlot__text_field",
                StringComparison.Ordinal);
        }

        if (library.EndsWith("/cupertino/text_form_field_row.dart", StringComparison.Ordinal))
        {
            // Dart permits a covariant parameter on the FormFieldState override.
            // Preserve the CLR base signature and keep the body casts explicit.
            source = source.Replace(
                "public override void didUpdateWidget(CupertinoTextFormFieldRow oldWidget)",
                "public override void didUpdateWidget(global::Doroti.Generated.Framework.Widgets.FormField<string> oldWidget)",
                StringComparison.Ordinal);
        }

        if (library.Contains("/material/", StringComparison.Ordinal))
        {
            source = source
                .Replace("DateTime.january", "1L", StringComparison.Ordinal)
                .Replace("DateTime.february", "2L", StringComparison.Ordinal)
                .Replace("DateTime.monday", "1L", StringComparison.Ordinal)
                .Replace("DateTime.daysPerWeek", "7L", StringComparison.Ordinal)
                .Replace("global::Doroti.Generated.Framework.Widgets.WidgetState.any", "global::Doroti.Generated.Framework.Widgets.WidgetStateMembers.any", StringComparison.Ordinal)
                .Replace("MaterialStatePropertyAll<", "global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<", StringComparison.Ordinal)
                .Replace("MaterialStateProperty<", "global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<", StringComparison.Ordinal)
                .Replace("MaterialStatesController", "global::Doroti.Generated.Framework.Widgets.WidgetStatesController", StringComparison.Ordinal);

            source = Regex.Replace(source, @"internal enum (_[A-Za-z0-9_]*Slot[A-Za-z0-9_]*)", "public enum $1");
            source = Regex.Replace(source, @"_AnimatedIconData__[A-Za-z0-9_]+", "_AnimatedIconData__animated_icons_data");
            source = Regex.Replace(
                source,
                @"(_(?:PathFrames|PathCommand|PathMoveTo|PathCubicTo|PathLineTo|PathClose)__animated_icons)__[A-Za-z0-9_]+_g",
                "$1");

            // The analyzer correctly resolves the generic return of Dart's
            // WidgetStateProperty.resolveWith, but an erased local T used to
            // leak into emitted closures. Let C# infer the concrete callback
            // result and remove only the synthetic erasure casts.
            source = Regex.Replace(
                source,
                @"WidgetStateProperty\.resolveWith\(\(\(global::System\.Func<HashSet<global::Doroti\.Generated\.Framework\.Widgets\.WidgetState>, T>\)\(\(states\) => \{(?<body>[\s\S]*?)\}\)\)\)",
                match =>
                {
                    var body = match.Groups["body"].Value
                        .Replace("((T)(object?)", "(", StringComparison.Ordinal)
                        .Replace("((T)(object)", "(", StringComparison.Ordinal);
                    return $"WidgetStateProperty.resolveWith((states) => {{{body}}})";
                });
            source = Regex.Replace(
                source,
                @"WidgetStateProperty\.resolveWith<(?<result>[^>\r\n]+)>\(\(\(global::System\.Func<HashSet<global::Doroti\.Generated\.Framework\.Widgets\.WidgetState>, (?<erased>[^>\r\n]+)>\)\(\(states\) => \{(?<body>[\s\S]*?)\}\)\)\)",
                match =>
                {
                    var erased = match.Groups["erased"].Value;
                    var body = match.Groups["body"].Value
                        .Replace($"(({erased})(object?)", "(", StringComparison.Ordinal)
                        .Replace($"(({erased}?)(object?)", "(", StringComparison.Ordinal);
                    return $"WidgetStateProperty.resolveWith<{match.Groups["result"].Value}>((states) => {{{body}}})";
                });

            source = source
                .Replace("new ThemeData()", "ThemeData.Create()", StringComparison.Ordinal)
                .Replace("List<global::Doroti.Generated.Framework.Widgets.GlobalKey<IState>>.Create(", "DartRuntimePrimitives.CreateList<global::Doroti.Generated.Framework.Widgets.GlobalKey<IState>>(", StringComparison.Ordinal)
                .Replace("[global::Doroti.Generated.Framework.Widgets.WidgetState.disabled] = disabled", "[global::Doroti.Generated.Framework.Widgets.WidgetState.disabled.asConstraint()] = disabled", StringComparison.Ordinal)
                .Replace("[global::Doroti.Generated.Framework.Widgets.WidgetState.selected] = selected", "[global::Doroti.Generated.Framework.Widgets.WidgetState.selected.asConstraint()] = selected", StringComparison.Ordinal)
                .Replace("[global::Doroti.Generated.Framework.Widgets.WidgetState.dragged] =", "[global::Doroti.Generated.Framework.Widgets.WidgetState.dragged.asConstraint()] =", StringComparison.Ordinal);
        }

        if (library.Contains("/material/animated_icons/data/", StringComparison.Ordinal) &&
            library.EndsWith(".g.dart", StringComparison.Ordinal))
        {
            source = Regex.Replace(source, @"public static partial class [A-Za-z0-9_]+Library", "public static partial class Animated_iconsLibrary");
        }

        if (library.EndsWith("/material/arc.dart", StringComparison.Ordinal))
        {
            source = source.Replace("public override Offset? begin", "public override Offset begin", StringComparison.Ordinal)
                .Replace("public override Offset? end", "public override Offset end", StringComparison.Ordinal)
                .Replace(": base(begin: begin, end: end)", ": base(begin: DartRuntimePrimitives.RequireValue(begin), end: DartRuntimePrimitives.RequireValue(end))", StringComparison.Ordinal)
                .Replace("ArcLibrary._allDiagonals.Cast<T>()", "ArcLibrary._allDiagonals.Cast<_Diagonal__arc>()", StringComparison.Ordinal)
                .Replace("DartRuntimePrimitives.Assert(() => (this.begin is not null));", string.Empty, StringComparison.Ordinal)
                .Replace("DartRuntimePrimitives.Assert(() => (this.end is not null));", string.Empty, StringComparison.Ordinal)
                .Replace("if (((this.begin is null) || (this.end is null)))", "if (false)", StringComparison.Ordinal);
        }

        if (library.EndsWith("/material/animated_icons/animated_icons.dart", StringComparison.Ordinal))
        {
            source = source
                .Replace("_interpolate<double?>(opacities, progress, Dart_uiLibrary.lerpDouble)", "_interpolate<double>(opacities, progress, (a, b, t) => Dart_uiLibrary.lerpDouble(a, b, t) ?? 0.0)", StringComparison.Ordinal)
                .Replace("_interpolate<global::Doroti.Flutter.Ui.Offset?>(points, progress, Offset.lerp)", "_interpolate<global::Doroti.Flutter.Ui.Offset>(points, progress, (a, b, t) => Offset.lerp(a, b, t)!.Value)", StringComparison.Ordinal)
                .Replace("_interpolate<global::Doroti.Flutter.Ui.Offset?>(controlPoints1, progress, Offset.lerp)", "_interpolate<global::Doroti.Flutter.Ui.Offset>(controlPoints1, progress, (a, b, t) => Offset.lerp(a, b, t)!.Value)", StringComparison.Ordinal)
                .Replace("_interpolate<global::Doroti.Flutter.Ui.Offset?>(controlPoints2, progress, Offset.lerp)", "_interpolate<global::Doroti.Flutter.Ui.Offset>(controlPoints2, progress, (a, b, t) => Offset.lerp(a, b, t)!.Value)", StringComparison.Ordinal)
                .Replace("_interpolate<global::Doroti.Flutter.Ui.Offset?>(targetPoints, progress, Offset.lerp)", "_interpolate<global::Doroti.Flutter.Ui.Offset>(targetPoints, progress, (a, b, t) => Offset.lerp(a, b, t)!.Value)", StringComparison.Ordinal);
        }

        if (library.EndsWith("/material/button_style_button.dart", StringComparison.Ordinal))
        {
            source = source
                .Replace("public abstract ButtonStyle defaultStyleOf(global::Doroti.Generated.Framework.Widgets.BuildContext context);", "public virtual ButtonStyle defaultStyleOf(global::Doroti.Generated.Framework.Widgets.BuildContext context) => default!;", StringComparison.Ordinal)
                .Replace("public abstract ButtonStyle? themeStyleOf(global::Doroti.Generated.Framework.Widgets.BuildContext context);", "public virtual ButtonStyle? themeStyleOf(global::Doroti.Generated.Framework.Widgets.BuildContext context) => default;", StringComparison.Ordinal)
                .Replace("ButtonStyleButton? effectiveValue(global::System.Func<ButtonStyle?, ButtonStyleButton?> getProperty)", "P? effectiveValue<P>(global::System.Func<ButtonStyle?, P?> getProperty)", StringComparison.Ordinal)
                .Replace("ButtonStyleButton? widgetValue", "P? widgetValue", StringComparison.Ordinal)
                .Replace("ButtonStyleButton? themeValue", "P? themeValue", StringComparison.Ordinal)
                .Replace("ButtonStyleButton? defaultValue", "P? defaultValue", StringComparison.Ordinal)
                .Replace("ButtonStyleButton? resolve(global::System.Func<ButtonStyle?, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<ButtonStyleButton>?> getProperty)", "P? resolve<P>(global::System.Func<ButtonStyle?, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<P>?> getProperty)", StringComparison.Ordinal)
                .Replace("return ((ButtonStyleButton?)(object?)getProperty(style)?.resolve(this.statesController.value));", "return getProperty(style) is { } property ? property.resolve(this.statesController.value) : default;", StringComparison.Ordinal);
            source = source.Replace("global::System.Func<HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>, ButtonStyleButton>", "global::System.Func<HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>, global::Doroti.Flutter.Ui.Color?>", StringComparison.Ordinal);
        }

        if (library.EndsWith("/material/elevated_button.dart", StringComparison.Ordinal))
        {
            source = source
                .Replace("[global::Doroti.Generated.Framework.Widgets.WidgetState.pressed] = (DartRuntimePrimitives.RequireValue(elevation", "[global::Doroti.Generated.Framework.Widgets.WidgetState.pressed.asConstraint()] = (DartRuntimePrimitives.RequireValue(elevation", StringComparison.Ordinal)
                .Replace("[global::Doroti.Generated.Framework.Widgets.WidgetState.hovered] = (DartRuntimePrimitives.RequireValue(elevation", "[global::Doroti.Generated.Framework.Widgets.WidgetState.hovered.asConstraint()] = (DartRuntimePrimitives.RequireValue(elevation", StringComparison.Ordinal)
                .Replace("[global::Doroti.Generated.Framework.Widgets.WidgetState.focused] = (DartRuntimePrimitives.RequireValue(elevation", "[global::Doroti.Generated.Framework.Widgets.WidgetState.focused.asConstraint()] = (DartRuntimePrimitives.RequireValue(elevation", StringComparison.Ordinal);
        }

        if (library.EndsWith("/material/calendar_date_picker.dart", StringComparison.Ordinal))
        {
            source = source
                .Replace("_Day__calendar_date_picker? effectiveValue(global::System.Func<DatePickerThemeData?, _Day__calendar_date_picker?> getProperty)", "P? effectiveValue<P>(global::System.Func<DatePickerThemeData?, P?> getProperty)", StringComparison.Ordinal)
                .Replace("_Day__calendar_date_picker? resolve(global::System.Func<DatePickerThemeData?, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<_Day__calendar_date_picker>?> getProperty", "P? resolve<P>(global::System.Func<DatePickerThemeData?, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<P>?> getProperty", StringComparison.Ordinal)
                .Replace("return ((_Day__calendar_date_picker?)(object?)getProperty(theme)?.resolve(states));", "return getProperty(theme) is { } property ? property.resolve(states) : default;", StringComparison.Ordinal)
                .Replace("YearPicker? effectiveValue(global::System.Func<DatePickerThemeData?, YearPicker?> getProperty)", "P? effectiveValue<P>(global::System.Func<DatePickerThemeData?, P?> getProperty)", StringComparison.Ordinal)
                .Replace("YearPicker? resolve(global::System.Func<DatePickerThemeData?, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<YearPicker>?> getProperty", "P? resolve<P>(global::System.Func<DatePickerThemeData?, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<P>?> getProperty", StringComparison.Ordinal)
                .Replace("return ((YearPicker?)(object?)getProperty(theme)?.resolve(states));", "return getProperty(theme) is { } property ? property.resolve(states) : default;", StringComparison.Ordinal);
            source = source
                .Replace("global::System.Func<HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>, _Day__calendar_date_picker>", "global::System.Func<HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>, global::Doroti.Flutter.Ui.Color?>", StringComparison.Ordinal)
                .Replace("global::System.Func<HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>, YearPicker>", "global::System.Func<HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>, global::Doroti.Flutter.Ui.Color?>", StringComparison.Ordinal);
        }

        if (library.EndsWith("/material/date_picker.dart", StringComparison.Ordinal))
        {
            source = source
                .Replace("_DayItem__date_picker? effectiveValue(global::System.Func<DatePickerThemeData?, _DayItem__date_picker?> getProperty)", "P? effectiveValue<P>(global::System.Func<DatePickerThemeData?, P?> getProperty)", StringComparison.Ordinal)
                .Replace("_DayItem__date_picker? resolve(global::System.Func<DatePickerThemeData?, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<_DayItem__date_picker>?> getProperty", "P? resolve<P>(global::System.Func<DatePickerThemeData?, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<P>?> getProperty", StringComparison.Ordinal)
                .Replace("return ((_DayItem__date_picker?)(object?)getProperty(theme)?.resolve(states));", "return getProperty(theme) is { } property ? property.resolve(states) : default;", StringComparison.Ordinal);
            source = source.Replace("global::System.Func<HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>, _DayItem__date_picker>", "global::System.Func<HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>, global::Doroti.Flutter.Ui.Color?>", StringComparison.Ordinal);
            source = source.Replace("this.value.index", "FoundationRuntimePorts.EnumIndex(this.value)", StringComparison.Ordinal);
        }

        if (library.EndsWith("/material/chip.dart", StringComparison.Ordinal) ||
            library.EndsWith("/material/input_decorator.dart", StringComparison.Ordinal) ||
            library.EndsWith("/material/list_tile.dart", StringComparison.Ordinal))
        {
            source = Regex.Replace(source, @"public virtual IEnumerable<(_[A-Za-z0-9_]*Slot[A-Za-z0-9_]*)> slots", "public override IEnumerable<$1> slots");
            source = Regex.Replace(source, @"public virtual (global::Doroti\.Generated\.Framework\.Widgets\.Widget\?) childForSlot\(", "public override $1 childForSlot(");
        }

        if (library.EndsWith("/material/date.dart", StringComparison.Ordinal))
        {
            source = source.Replace("where T : DateTime", "where T : struct", StringComparison.Ordinal)
                .Replace("return (((dateA?.year == dateB?.year) && (dateA?.month == dateB?.month)) && (dateA?.day == dateB?.day));", "return dateA.HasValue && dateB.HasValue && ((dynamic)dateA.Value).Year == ((dynamic)dateB.Value).Year && ((dynamic)dateA.Value).Month == ((dynamic)dateB.Value).Month && ((dynamic)dateA.Value).Day == ((dynamic)dateB.Value).Day;", StringComparison.Ordinal)
                .Replace("return ((dateA?.year == dateB?.year) && (dateA?.month == dateB?.month));", "return dateA.HasValue && dateB.HasValue && ((dynamic)dateA.Value).Year == ((dynamic)dateB.Value).Year && ((dynamic)dateA.Value).Month == ((dynamic)dateB.Value).Month;", StringComparison.Ordinal)
                .Replace("System.Diagnostics.Debug.Assert(!start.isAfter(end));", "System.Diagnostics.Debug.Assert(Comparer<T>.Default.Compare(start, end) <= 0);", StringComparison.Ordinal)
                .Replace("public virtual Duration duration => this.end.difference(this.start);", "public virtual Duration duration => (Duration)(((dynamic)this.end) - ((dynamic)this.start));", StringComparison.Ordinal)
                .Replace("object.Equals(((DateTimeRange<DateTime>)((DateTimeRange<DateTime>)__other)).start, this.start)", "object.Equals(((DateTimeRange<T>)__other).start, this.start)", StringComparison.Ordinal)
                .Replace("object.Equals(((DateTimeRange<DateTime>)((DateTimeRange<DateTime>)__other)).end, this.end)", "object.Equals(((DateTimeRange<T>)__other).end, this.end)", StringComparison.Ordinal);
        }

        if (library.EndsWith("/material/desktop_text_selection.dart", StringComparison.Ordinal) || library.EndsWith("/material/text_selection.dart", StringComparison.Ordinal))
        {
            source = source
                .Replace(", global::Doroti.Generated.Framework.Widgets.TextSelectionHandleControls", string.Empty, StringComparison.Ordinal)
                .Replace("public override void handleCut(", "public virtual void handleCut(", StringComparison.Ordinal)
                .Replace("public override void handleCopy(", "public virtual void handleCopy(", StringComparison.Ordinal);
        }

        if (library.EndsWith("/material/dropdown.dart", StringComparison.Ordinal))
        {
            source = source
                .Replace("internal class _DropdownRouteResult__dropdown", "public class _DropdownRouteResult__dropdown", StringComparison.Ordinal)
                .Replace("public override void didUpdateWidget(T oldWidget)", "public override void didUpdateWidget(global::Doroti.Generated.Framework.Widgets.FormField<T> oldWidget)", StringComparison.Ordinal);
        }

        if (library.EndsWith("/material/dropdown_menu_form_field.dart", StringComparison.Ordinal))
        {
            source = source.Replace("public override void didUpdateWidget(T oldWidget)", "public override void didUpdateWidget(global::Doroti.Generated.Framework.Widgets.FormField<T> oldWidget)", StringComparison.Ordinal);
        }

        if (library.EndsWith("/material/text_form_field.dart", StringComparison.Ordinal))
        {
            source = source.Replace("public override void didUpdateWidget(TextFormField oldWidget)", "public override void didUpdateWidget(global::Doroti.Generated.Framework.Widgets.FormField<string> oldWidget)", StringComparison.Ordinal);
        }

        if (library.EndsWith("/material/icons.dart", StringComparison.Ordinal))
        {
            source = source.Replace("public override global::Doroti.Generated.Framework.Widgets.IconData", "public virtual global::Doroti.Generated.Framework.Widgets.IconData", StringComparison.Ordinal);
        }

        if (library.EndsWith("/material/ink_sparkle.dart", StringComparison.Ordinal))
        {
            source = source.Replace("Vector2", "global::System.Numerics.Vector2", StringComparison.Ordinal);
        }

        if (library.EndsWith("/material/input_border.dart", StringComparison.Ordinal))
        {
            source = source.Replace("public abstract override void paint(", "public abstract void paint(", StringComparison.Ordinal);
        }

        if (library.EndsWith("/material/material.dart", StringComparison.Ordinal))
        {
            source = source.Replace("public abstract void paintFeature(Canvas canvas, Matrix4 transform);", "public virtual void paintFeature(Canvas canvas, Matrix4 transform) { }", StringComparison.Ordinal);
        }

        if (library.EndsWith("/material/no_splash.dart", StringComparison.Ordinal))
        {
            source = source.Replace("public virtual void paintFeature(Canvas canvas, Matrix4 transform)", "public override void paintFeature(Canvas canvas, Matrix4 transform)", StringComparison.Ordinal);
        }

        if (library.EndsWith("/material/page_transitions_theme.dart", StringComparison.Ordinal))
        {
            source = source.Replace("global::Doroti.Generated.Framework.Widgets.Image image", "global::Doroti.Flutter.Ui.Image image", StringComparison.Ordinal);
        }

        if (library.EndsWith("/material/scaffold.dart", StringComparison.Ordinal))
        {
            source = source.Replace("public virtual global::Doroti.Flutter.Ui.Offset getOffset(ScaffoldPrelayoutGeometry scaffoldGeometry)", "public override global::Doroti.Flutter.Ui.Offset getOffset(ScaffoldPrelayoutGeometry scaffoldGeometry)", StringComparison.Ordinal);
        }

        if (library.EndsWith("/material/slider_parts.dart", StringComparison.Ordinal))
        {
            source = source.Replace("TextDirection textDirection = false", "TextDirection textDirection = default!", StringComparison.Ordinal);
            const string sliderOverload = "\n    public global::Doroti.Flutter.Ui.Size getPreferredSize(bool isEnabled, bool isDiscrete, global::Doroti.Generated.Framework.Painting.TextPainter? labelPainter = null, double? textScaleFactor = null) => getPreferredSize(isEnabled, isDiscrete);\n";
            source = source.Replace("public class RoundSliderThumbShape : SliderComponentShape\n{", "public class RoundSliderThumbShape : SliderComponentShape\n{" + sliderOverload, StringComparison.Ordinal)
                .Replace("public class HandleThumbShape : SliderComponentShape\n{", "public class HandleThumbShape : SliderComponentShape\n{" + sliderOverload, StringComparison.Ordinal);
        }

        if (library.EndsWith("/material/range_slider_parts.dart", StringComparison.Ordinal))
        {
            // The source declarations already carry the concrete non-nullable
            // virtual contract used by the built-in range indicator shapes.
        }

        if (library.EndsWith("/material/slider_value_indicator_shape.dart", StringComparison.Ordinal))
        {
            source = source
                .Replace("public virtual global::Doroti.Flutter.Ui.Size getPreferredSize(bool isEnabled, bool isDiscrete, global::Doroti.Generated.Framework.Painting.TextPainter labelPainter, double textScaleFactor)", "public override global::Doroti.Flutter.Ui.Size getPreferredSize(bool isEnabled, bool isDiscrete, global::Doroti.Generated.Framework.Painting.TextPainter labelPainter, double textScaleFactor)", StringComparison.Ordinal)
                .Replace("public override global::Doroti.Flutter.Ui.Size getPreferredSize(bool isEnabled, bool isDiscrete, global::Doroti.Generated.Framework.Painting.TextPainter? labelPainter = null, double? textScaleFactor = null)", "public override global::Doroti.Flutter.Ui.Size getPreferredSize(bool isEnabled, bool isDiscrete, global::Doroti.Generated.Framework.Painting.TextPainter labelPainter, double textScaleFactor)", StringComparison.Ordinal)
                .Replace("public virtual void paint(global::Doroti.Generated.Framework.Rendering.PaintingContext context, Offset center, global::Doroti.Generated.Framework.Animation.Animation<double>? activationAnimation = null, global::Doroti.Generated.Framework.Animation.Animation<double>? enableAnimation = null, bool? isDiscrete = null, bool? isOnTop = null, global::Doroti.Generated.Framework.Painting.TextPainter? labelPainter = null, double? textScaleFactor = null, Size? sizeWithOverflow = null, global::Doroti.Generated.Framework.Rendering.RenderBox? parentBox = null, SliderThemeData? sliderTheme = null, TextDirection? textDirection = null, double? value = null, Thumb? thumb = null)", "public override void paint(global::Doroti.Generated.Framework.Rendering.PaintingContext context, Offset center, global::Doroti.Generated.Framework.Animation.Animation<double> activationAnimation, global::Doroti.Generated.Framework.Animation.Animation<double> enableAnimation, bool isDiscrete = default!, bool isOnTop = default!, global::Doroti.Generated.Framework.Painting.TextPainter labelPainter = default!, double textScaleFactor = default!, Size sizeWithOverflow = default!, global::Doroti.Generated.Framework.Rendering.RenderBox parentBox = default!, SliderThemeData sliderTheme = default!, TextDirection textDirection = default!, double value = default!, Thumb thumb = default!)", StringComparison.Ordinal)
                .Replace("public override void paint(global::Doroti.Generated.Framework.Rendering.PaintingContext context, Offset center, global::Doroti.Generated.Framework.Animation.Animation<double>? activationAnimation = null, global::Doroti.Generated.Framework.Animation.Animation<double>? enableAnimation = null, bool? isDiscrete = null, bool? isOnTop = null, global::Doroti.Generated.Framework.Painting.TextPainter? labelPainter = null, double? textScaleFactor = null, Size? sizeWithOverflow = null, global::Doroti.Generated.Framework.Rendering.RenderBox? parentBox = null, SliderThemeData? sliderTheme = null, TextDirection? textDirection = null, double? value = null, Thumb? thumb = null)", "public override void paint(global::Doroti.Generated.Framework.Rendering.PaintingContext context, Offset center, global::Doroti.Generated.Framework.Animation.Animation<double> activationAnimation, global::Doroti.Generated.Framework.Animation.Animation<double> enableAnimation, bool isDiscrete = default!, bool isOnTop = default!, global::Doroti.Generated.Framework.Painting.TextPainter labelPainter = default!, double textScaleFactor = default!, Size sizeWithOverflow = default!, global::Doroti.Generated.Framework.Rendering.RenderBox parentBox = default!, SliderThemeData sliderTheme = default!, TextDirection textDirection = default!, double value = default!, Thumb thumb = default!)", StringComparison.Ordinal)
                .Replace("public virtual void paint(global::Doroti.Generated.Framework.Rendering.PaintingContext context, Offset center, global::Doroti.Generated.Framework.Animation.Animation<double> activationAnimation, global::Doroti.Generated.Framework.Animation.Animation<double> enableAnimation, bool? isDiscrete = null, bool isOnTop = false, global::Doroti.Generated.Framework.Painting.TextPainter labelPainter = default!, global::Doroti.Generated.Framework.Rendering.RenderBox parentBox = default!, SliderThemeData sliderTheme = default!, TextDirection? textDirection = null, Thumb? thumb = null, double? value = null, double? textScaleFactor = null, Size? sizeWithOverflow = null)", "public override void paint(global::Doroti.Generated.Framework.Rendering.PaintingContext context, Offset center, global::Doroti.Generated.Framework.Animation.Animation<double> activationAnimation, global::Doroti.Generated.Framework.Animation.Animation<double> enableAnimation, bool isDiscrete = default!, bool isOnTop = default!, global::Doroti.Generated.Framework.Painting.TextPainter labelPainter = default!, double textScaleFactor = default!, Size sizeWithOverflow = default!, global::Doroti.Generated.Framework.Rendering.RenderBox parentBox = default!, SliderThemeData sliderTheme = default!, TextDirection textDirection = default!, double value = default!, Thumb thumb = default!)", StringComparison.Ordinal);
        }

        if (library.EndsWith("/material/color_scheme.dart", StringComparison.Ordinal))
        {
            const string materialColorUtilities = """

internal sealed class QuantizerResult
{
    internal DartMap<long, long> colorToCount { get; } = new();
}

internal sealed class QuantizerCelebi
{
    internal Future<QuantizerResult> quantize(dynamic pixels, long maxColors, bool returnInputPixelToClusterPixel = false)
    {
        var result = new QuantizerResult();
        foreach (var pixel in pixels) { var value = Convert.ToInt64(pixel); result.colorToCount[value] = result.colorToCount.GetValueOrDefault(value) + 1; }
        if (result.colorToCount.Count == 0) result.colorToCount[0xff6750a4] = 1;
        return Future<QuantizerResult>.value(result);
    }
}

internal static class Score
{
    internal static IEnumerable<long> score(DartMap<long, long> colors, long desired = 1) =>
        colors.OrderByDescending(entry => entry.Value).Take(checked((int)desired)).Select(entry => entry.Key);
}

internal sealed class Hct
{
    internal long argb { get; }
    private Hct(long argb) => this.argb = argb;
    internal static Hct fromInt(long argb) => new(argb);
}

internal class DynamicScheme
{
    internal long seedArgb { get; }
    internal bool isDark { get; }
    internal double contrastLevel { get; }
    internal string variant { get; }
    internal DynamicScheme(Hct sourceColorHct, bool isDark, double contrastLevel, string variant)
    {
        seedArgb = sourceColorHct.argb;
        this.isDark = isDark;
        this.contrastLevel = contrastLevel;
        this.variant = variant;
    }
}

internal sealed class SchemeTonalSpot(Hct sourceColorHct, bool isDark, double contrastLevel) : DynamicScheme(sourceColorHct, isDark, contrastLevel, "tonalSpot");
internal sealed class SchemeFidelity(Hct sourceColorHct, bool isDark, double contrastLevel) : DynamicScheme(sourceColorHct, isDark, contrastLevel, "fidelity");
internal sealed class SchemeContent(Hct sourceColorHct, bool isDark, double contrastLevel) : DynamicScheme(sourceColorHct, isDark, contrastLevel, "content");
internal sealed class SchemeMonochrome(Hct sourceColorHct, bool isDark, double contrastLevel) : DynamicScheme(sourceColorHct, isDark, contrastLevel, "monochrome");
internal sealed class SchemeNeutral(Hct sourceColorHct, bool isDark, double contrastLevel) : DynamicScheme(sourceColorHct, isDark, contrastLevel, "neutral");
internal sealed class SchemeVibrant(Hct sourceColorHct, bool isDark, double contrastLevel) : DynamicScheme(sourceColorHct, isDark, contrastLevel, "vibrant");
internal sealed class SchemeExpressive(Hct sourceColorHct, bool isDark, double contrastLevel) : DynamicScheme(sourceColorHct, isDark, contrastLevel, "expressive");
internal sealed class SchemeRainbow(Hct sourceColorHct, bool isDark, double contrastLevel) : DynamicScheme(sourceColorHct, isDark, contrastLevel, "rainbow");
internal sealed class SchemeFruitSalad(Hct sourceColorHct, bool isDark, double contrastLevel) : DynamicScheme(sourceColorHct, isDark, contrastLevel, "fruitSalad");

internal sealed class _MaterialDynamicColor
{
    private readonly string _role;
    internal _MaterialDynamicColor(string role) => _role = role;

    internal long getArgb(DynamicScheme scheme) => MaterialColorSchemeRuntime.GetArgb(
        scheme.seedArgb, scheme.isDark, scheme.variant, scheme.contrastLevel, _role);
}

internal static class MaterialDynamicColors
{
    internal static readonly _MaterialDynamicColor primary = new("primary"), onPrimary = new("onPrimary"), primaryContainer = new("primaryContainer"), onPrimaryContainer = new("onPrimaryContainer"), primaryFixed = new("primaryFixed"), primaryFixedDim = new("primaryFixedDim"), onPrimaryFixed = new("onPrimaryFixed"), onPrimaryFixedVariant = new("onPrimaryFixedVariant"), secondary = new("secondary"), onSecondary = new("onSecondary"), secondaryContainer = new("secondaryContainer"), onSecondaryContainer = new("onSecondaryContainer"), secondaryFixed = new("secondaryFixed"), secondaryFixedDim = new("secondaryFixedDim"), onSecondaryFixed = new("onSecondaryFixed"), onSecondaryFixedVariant = new("onSecondaryFixedVariant"), tertiary = new("tertiary"), onTertiary = new("onTertiary"), tertiaryContainer = new("tertiaryContainer"), onTertiaryContainer = new("onTertiaryContainer"), tertiaryFixed = new("tertiaryFixed"), tertiaryFixedDim = new("tertiaryFixedDim"), onTertiaryFixed = new("onTertiaryFixed"), onTertiaryFixedVariant = new("onTertiaryFixedVariant"), error = new("error"), onError = new("onError"), errorContainer = new("errorContainer"), onErrorContainer = new("onErrorContainer"), outline = new("outline"), outlineVariant = new("outlineVariant"), surface = new("surface"), surfaceDim = new("surfaceDim"), surfaceBright = new("surfaceBright"), surfaceContainerLowest = new("surfaceContainerLowest"), surfaceContainerLow = new("surfaceContainerLow"), surfaceContainer = new("surfaceContainer"), surfaceContainerHigh = new("surfaceContainerHigh"), surfaceContainerHighest = new("surfaceContainerHighest"), onSurface = new("onSurface"), onSurfaceVariant = new("onSurfaceVariant"), inverseSurface = new("inverseSurface"), inverseOnSurface = new("inverseOnSurface"), inversePrimary = new("inversePrimary"), shadow = new("shadow"), scrim = new("scrim"), background = new("background"), onBackground = new("onBackground"), surfaceVariant = new("surfaceVariant");
}
""";
            source = source.Replace("namespace Doroti.Generated.Framework.Material;", "namespace Doroti.Generated.Framework.Material;" + materialColorUtilities, StringComparison.Ordinal);
        }

        if (library.EndsWith("/material/material_state.dart", StringComparison.Ordinal))
        {
            source = source
                .Replace("public virtual T resolve(HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState> states)", "public virtual InputBorder resolve(HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState> states)", StringComparison.Ordinal)
                .Replace("public delegate void global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<T>();", string.Empty, StringComparison.Ordinal)
                .Replace("public delegate void global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<T>();", string.Empty, StringComparison.Ordinal)
                .Replace("public delegate void global::Doroti.Generated.Framework.Widgets.WidgetStatesController();", string.Empty, StringComparison.Ordinal);
        }

        if (library.EndsWith("/material/segmented_button.dart", StringComparison.Ordinal))
        {
            source = source
                .Replace("P? effectiveValue(global::System.Func<ButtonStyle?, P?> getProperty)", "P? effectiveValue<P>(global::System.Func<ButtonStyle?, P?> getProperty)", StringComparison.Ordinal)
                .Replace("P? resolve(global::System.Func<ButtonStyle?, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<P>?> getProperty", "P? resolve<P>(global::System.Func<ButtonStyle?, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<P>?> getProperty", StringComparison.Ordinal)
                .Replace("[(global::Doroti.Generated.Framework.Widgets.WidgetState.selected & global::Doroti.Generated.Framework.Widgets.WidgetState.pressed)]", "[global::Doroti.Generated.Framework.Widgets.WidgetState.selected.asConstraint().op_BitwiseAnd(global::Doroti.Generated.Framework.Widgets.WidgetState.pressed.asConstraint())]", StringComparison.Ordinal)
                .Replace("[(global::Doroti.Generated.Framework.Widgets.WidgetState.selected & global::Doroti.Generated.Framework.Widgets.WidgetState.hovered)]", "[global::Doroti.Generated.Framework.Widgets.WidgetState.selected.asConstraint().op_BitwiseAnd(global::Doroti.Generated.Framework.Widgets.WidgetState.hovered.asConstraint())]", StringComparison.Ordinal)
                .Replace("[(global::Doroti.Generated.Framework.Widgets.WidgetState.selected & global::Doroti.Generated.Framework.Widgets.WidgetState.focused)]", "[global::Doroti.Generated.Framework.Widgets.WidgetState.selected.asConstraint().op_BitwiseAnd(global::Doroti.Generated.Framework.Widgets.WidgetState.focused.asConstraint())]", StringComparison.Ordinal)
                .Replace("[global::Doroti.Generated.Framework.Widgets.WidgetState.pressed] = unselected", "[global::Doroti.Generated.Framework.Widgets.WidgetState.pressed.asConstraint()] = unselected", StringComparison.Ordinal)
                .Replace("[global::Doroti.Generated.Framework.Widgets.WidgetState.hovered] = unselected", "[global::Doroti.Generated.Framework.Widgets.WidgetState.hovered.asConstraint()] = unselected", StringComparison.Ordinal)
                .Replace("[global::Doroti.Generated.Framework.Widgets.WidgetState.focused] = unselected", "[global::Doroti.Generated.Framework.Widgets.WidgetState.focused.asConstraint()] = unselected", StringComparison.Ordinal);
        }

        if (library.EndsWith("/material/menu_anchor.dart", StringComparison.Ordinal))
        {
            source = source.Replace("public override void invoke(global::Doroti.Generated.Framework.Widgets.DirectionalFocusIntent", "public override object? invoke(global::Doroti.Generated.Framework.Widgets.DirectionalFocusIntent", StringComparison.Ordinal)
                .Replace("        base.invoke(intent, context);\n    }", "        return base.invoke(intent, context);\n    }", StringComparison.Ordinal)
                .Replace("_MenuPanel__menu_anchor? effectiveValue(global::System.Func<MenuStyle?, _MenuPanel__menu_anchor?> getProperty)", "P? effectiveValue<P>(global::System.Func<MenuStyle?, P?> getProperty)", StringComparison.Ordinal)
                .Replace("_MenuPanel__menu_anchor? resolve(global::System.Func<MenuStyle?, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<_MenuPanel__menu_anchor>?> getProperty)", "P? resolve<P>(global::System.Func<MenuStyle?, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<P>?> getProperty)", StringComparison.Ordinal)
                .Replace("return ((_MenuPanel__menu_anchor?)(object?)getProperty(style)?.resolve(new HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>()));", "return getProperty(style) is { } property ? property.resolve(new HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>()) : default;", StringComparison.Ordinal)
                .Replace("T? effectiveValue(global::System.Func<MenuStyle?, T?> getProperty)", "T? effectiveValue<T>(global::System.Func<MenuStyle?, T?> getProperty)", StringComparison.Ordinal)
                .Replace("T? resolve(global::System.Func<MenuStyle?, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<T>?> getProperty)", "T? resolve<T>(global::System.Func<MenuStyle?, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<T>?> getProperty)", StringComparison.Ordinal);
            source = Regex.Replace(
                source,
                @"(internal class _SubmenuDirectionalFocusAction__menu_anchor[\s\S]*?)(?=\ninternal class _LocalizedShortcutLabeler__menu_anchor)",
                match => match.Groups[1].Value.Replace("return;", "return null;", StringComparison.Ordinal).Replace("        Actions.maybeInvoke(this.submenu.context, intent);\n", "        return Actions.maybeInvoke(this.submenu.context, intent);\n", StringComparison.Ordinal));
        }

        if (library.EndsWith("/material/search_anchor.dart", StringComparison.Ordinal))
        {
            source = source
                .Replace("SearchBar? resolve(global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<SearchBar>? widgetValue, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<SearchBar>? themeValue, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<SearchBar>? defaultValue)", "P? resolve<P>(global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<P>? widgetValue, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<P>? themeValue, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<P>? defaultValue)", StringComparison.Ordinal)
                .Replace("return ((((widgetValue?.resolve(states__60702) ?? (SearchBar)themeValue?.resolve(states__60702))) ?? (SearchBar)defaultValue?.resolve(states__60702)));", "return widgetValue is not null ? widgetValue.resolve(states__60702) : themeValue is not null ? themeValue.resolve(states__60702) : defaultValue is not null ? defaultValue.resolve(states__60702) : default;", StringComparison.Ordinal);
            source = Regex.Replace(
                source,
                @"=> \(\(SearchAnchor\)\(object\?\)new _SearchAnchorWithSearchBar__search_anchor\([^\r\n]+\)\);",
                "=> new _SearchAnchorWithSearchBar__search_anchor(barLeading: barLeading, barTrailing: barTrailing, barHintText: barHintText, onTap: onTap, onSubmitted: onSubmitted, onChanged: onChanged, onClose: onClose, onOpen: onOpen, barElevation: barElevation, barBackgroundColor: barBackgroundColor, barOverlayColor: barOverlayColor, barSide: barSide, barShape: barShape, barPadding: barPadding, viewBarPadding: viewBarPadding, barTextStyle: barTextStyle, barHintStyle: barHintStyle, viewBuilder: viewBuilder, viewLeading: viewLeading, viewTrailing: viewTrailing, viewHintText: viewHintText, viewBackgroundColor: viewBackgroundColor, viewElevation: viewElevation, viewSide: viewSide, viewShape: viewShape, viewHeaderHeight: viewHeaderHeight, viewHeaderTextStyle: viewHeaderTextStyle, viewHeaderHintStyle: viewHeaderHintStyle, dividerColor: dividerColor, constraints: constraints, viewConstraints: viewConstraints, viewPadding: viewPadding, shrinkWrap: shrinkWrap, isFullScreen: isFullScreen, searchController: searchController, textCapitalization: textCapitalization, suggestionsBuilder: suggestionsBuilder, textInputAction: textInputAction, keyboardType: keyboardType, scrollPadding: scrollPadding, contextMenuBuilder: contextMenuBuilder, enabled: enabled, smartDashesType: smartDashesType, smartQuotesType: smartQuotesType);",
                RegexOptions.None,
                TimeSpan.FromSeconds(1));
        }

        if (library.EndsWith("/material/animated_icons/animated_icons_data.dart", StringComparison.Ordinal))
        {
            source = Regex.Replace(source, @"[A-Z][A-Za-z0-9_]*_gLibrary\.__", "Animated_iconsLibrary.__");
        }

        if (library.EndsWith("/material/about.dart", StringComparison.Ordinal))
        {
            source = source
                .Replace("(Future<_LicenseData__about>)new _LicenseData__about()", "Future<_LicenseData__about>.value(new _LicenseData__about())", StringComparison.Ordinal)
                .Replace("global::Doroti.Generated.Framework.Widgets.Flow flow", "global::Doroti.Flutter.Runtime.Flow flow", StringComparison.Ordinal)
                .Replace("Flow.begin()", "global::Doroti.Flutter.Runtime.Flow.begin()", StringComparison.Ordinal)
                .Replace("Flow.step(", "global::Doroti.Flutter.Runtime.Flow.step(", StringComparison.Ordinal)
                .Replace("Flow.end(", "global::Doroti.Flutter.Runtime.Flow.end(", StringComparison.Ordinal)
                .Replace("(global::System.Func<bool, List<global::Doroti.Generated.Framework.Foundation.LicenseParagraph>>)((global::Doroti.Generated.Framework.Foundation.LicenseEntry)license__34391).paragraphs.toList", "(global::System.Func<bool, List<global::Doroti.Generated.Framework.Foundation.LicenseParagraph>>)((_) => ((global::Doroti.Generated.Framework.Foundation.LicenseEntry)license__34391).paragraphs.toList())", StringComparison.Ordinal)
                .Replace("((_) => { _ = this._detailArguments.value = arguments; })", "((_duration) => { this._detailArguments.value = arguments; })", StringComparison.Ordinal)
                .Replace("global::Doroti.Generated.Framework.Widgets.PageRoute<_MasterDetailScaffold__about>", "global::Doroti.Generated.Framework.Widgets.PageRoute<object?>", StringComparison.Ordinal);
        }

        if (library.EndsWith("/material/carousel.dart", StringComparison.Ordinal))
        {
            source = Regex.Replace(source, @"\.max\b(?!\s*\()", ".max()");
            source = Regex.Replace(source, @"\.min\b(?!\s*\()", ".min()");
            source = Regex.Replace(source, @"\.sum\b(?!\s*\()", ".sum()");
            source = source.Replace("public new ItemExtentBuilder? itemExtentBuilder => this._buildItemExtent;", "public new ItemExtentBuilder? itemExtentBuilder => (index, dimensions) => this._buildItemExtent(index, dimensions);", StringComparison.Ordinal);
        }

        if (library.EndsWith("/material/slider_parts.dart", StringComparison.Ordinal))
        {
            source = source
                .Replace("Offset __thumbCenter = DartRuntimePrimitives.ConvertValue<Offset>(secondaryOffset);", "Offset __thumbCenter = thumbCenter;", StringComparison.Ordinal)
                .Replace("isEnabled: textDirection", "isEnabled: isEnabled", StringComparison.Ordinal)
                .Replace("((textDirection is not null))", "((secondaryOffset is not null))", StringComparison.Ordinal)
                .Replace("DartRuntimePrimitives.RequireValue(textDirection).dx", "DartRuntimePrimitives.RequireValue(secondaryOffset).dx", StringComparison.Ordinal)
                .Replace("textDirection.dx", "DartRuntimePrimitives.RequireValue(secondaryOffset).dx", StringComparison.Ordinal);
        }

        if (library.EndsWith("/material/scaffold.dart", StringComparison.Ordinal))
        {
            source = source
                .Replace("._snackBars.First", "._snackBars.First()", StringComparison.Ordinal)
                .Replace("._materialBanners.First", "._materialBanners.First()", StringComparison.Ordinal)
                .Replace("State<Scaffold>.handleStatusBarTap();", string.Empty, StringComparison.Ordinal);
        }

        if (library.EndsWith("/material/switch.dart", StringComparison.Ordinal))
        {
            const string erasedCursor = "WidgetStateProperty.resolveWith(((global::System.Func<HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>, T>)((states) => global::Doroti.Generated.Framework.Widgets.WidgetStateMouseCursor.clickable.resolve(states))))";
            source = source.Replace(erasedCursor, "WidgetStateProperty.resolveWith((states) => global::Doroti.Generated.Framework.Widgets.WidgetStateMouseCursor.clickable.resolve(states))", StringComparison.Ordinal);
        }

        if (library.EndsWith("/material/app.dart", StringComparison.Ordinal) ||
            library.EndsWith("/material/expansion_panel.dart", StringComparison.Ordinal))
        {
            source = source.Replace("?? kThemeAnimationDuration", "?? ThemeLibrary.kThemeAnimationDuration", StringComparison.Ordinal);
        }

        if (library.EndsWith("/material/text_field.dart", StringComparison.Ordinal) ||
            library.EndsWith("/material/text_form_field.dart", StringComparison.Ordinal) ||
            library.EndsWith("/material/search_anchor.dart", StringComparison.Ordinal) ||
            library.EndsWith("/material/selectable_text.dart", StringComparison.Ordinal))
        {
            source = source.Replace("new AdaptiveTextSelectionToolbar(editableTextState: editableTextState)", "AdaptiveTextSelectionToolbar.CreateEditableText(editableTextState: editableTextState)", StringComparison.Ordinal);
            if (library.EndsWith("/material/text_field.dart", StringComparison.Ordinal))
            {
                source = source
                    .Replace("new CupertinoSpellCheckSuggestionsToolbar(editableTextState: editableTextState)", "CupertinoSpellCheckSuggestionsToolbar.CreateEditableText(editableTextState: editableTextState)", StringComparison.Ordinal)
                    .Replace("unregisterFromRestoration(this._controller!)", "unregisterFromRestoration(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.RestorableProperty<object>>(this._controller!))", StringComparison.Ordinal)
                    .Replace("Text_selectionLibrary.cupertinoTextSelectionHandleControls", "Text_selectionLibrary.materialTextSelectionHandleControls", StringComparison.Ordinal)
                    .Replace("Desktop_text_selectionLibrary.cupertinoDesktopTextSelectionHandleControls", "Desktop_text_selectionLibrary.desktopTextSelectionHandleControls", StringComparison.Ordinal);
            }
        }

        if (library.EndsWith("/material/selection_area.dart", StringComparison.Ordinal))
        {
            source = source
                .Replace("new AdaptiveTextSelectionToolbar(selectableRegionState: selectableRegionState)", "AdaptiveTextSelectionToolbar.CreateSelectableRegion(selectableRegionState: selectableRegionState)", StringComparison.Ordinal)
                .Replace("Text_selectionLibrary.cupertinoTextSelectionHandleControls", "Text_selectionLibrary.materialTextSelectionHandleControls", StringComparison.Ordinal)
                .Replace("Desktop_text_selectionLibrary.cupertinoDesktopTextSelectionHandleControls", "Desktop_text_selectionLibrary.desktopTextSelectionHandleControls", StringComparison.Ordinal);
        }

        if (library.EndsWith("/material/theme_data.dart", StringComparison.Ordinal))
        {
            source = source.Replace("public abstract class ThemeExtension<T> where T : ThemeExtension<T>", "public abstract class ThemeExtension<T>", StringComparison.Ordinal);
        }

        // Nullable callback properties are Dart tear-offs, not always-present
        // forwarding closures. Preserving null here is required so losing
        // recognizers do not invoke a wrapper whose delegate target is null.
        source = Regex.Replace(
            source,
            @"(?<argument>onTap|onTapCancel|onDoubleTap|onLongPress|onLongPressUp|onSecondaryTap|onSecondaryTapCancel): \(\) => this\.\k<argument>\(\)",
            "${argument}: this.${argument}");

        if (!library.Contains("/widgets/", StringComparison.Ordinal))
        {
            return source;
        }

        source = source
            .Replace("this.onStateChange?.Invoke(this._lifecycleState!)", "this.onStateChange?.Invoke(DartRuntimePrimitives.RequireValue(this._lifecycleState))", StringComparison.Ordinal)
            .Replace("this.widget.dragAnchorStrategy(this.widget, this.context, position)", "((dynamic)this.widget.dragAnchorStrategy)(this.widget, this.context, position)", StringComparison.Ordinal)
            .Replace("orElse: (() => { })", "orElse: (() => default!)", StringComparison.Ordinal)
            .Replace("_makeOverridable((__arg0) => ((Action<TransposeCharactersIntent>)", "_makeOverridable<TransposeCharactersIntent>((__arg0) => ((Action<TransposeCharactersIntent>)", StringComparison.Ordinal)
            .Replace("((global::System.Func<object, bool>)((FocusScopeNode)nodeScope__42268)._focusedChildren.Remove)(__arg0)", "((FocusScopeNode)nodeScope__42268)._focusedChildren.Remove(DartRuntimePrimitives.ConvertValue<FocusScopeNode>(__arg0))", StringComparison.Ordinal)
            .Replace("(((object?)spellCheckService__128604 ?? (object?)new global::Doroti.Generated.Framework.Services.DefaultSpellCheckService()))", "DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Services.SpellCheckService>(spellCheckService__128604 ?? new global::Doroti.Generated.Framework.Services.DefaultSpellCheckService())", StringComparison.Ordinal)
            .Replace("((global::Doroti.Generated.Framework.Rendering.ContainerRenderObjectMixin<global::Doroti.Generated.Framework.Rendering.RenderObject, global::Doroti.Generated.Framework.Rendering.ContainerParentDataMixin<global::Doroti.Generated.Framework.Rendering.RenderObject>>?)(object?)base.renderObject)!", "DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Rendering.RenderObject>(base.renderObject)", StringComparison.Ordinal)
            .Replace("((IndexedSlot<Element?>)((IndexedSlot<Element?>)__other))", "((IndexedSlot<T>)(object)__other)", StringComparison.Ordinal)
            .Replace("gestures ?? new DartMap<Type, GestureRecognizerFactory<global::Doroti.Generated.Framework.Gestures.GestureRecognizer>>()", "gestures ?? new DartMap<Type, GestureRecognizerFactory<global::Doroti.Generated.Framework.Gestures.GestureRecognizer>>().cast<Type, dynamic>()", StringComparison.Ordinal)
            .Replace("double value__as40867 = (double)value", "double value__as40867 = (double)(object)value!", StringComparison.Ordinal)
            .Replace(".ToList().Cast<EditableText>().ToList()", ".ToList().Cast<string>().ToList()", StringComparison.Ordinal)
            .Replace("begin: value)", "begin: DartRuntimePrimitives.ConvertValue<global::Doroti.Flutter.Ui.Offset>(value))", StringComparison.Ordinal)
            .Replace("return ((T?)(object?)scope__22436?.localizationsState.resourcesFor<T?>(type));", "return scope__22436 is null ? default : scope__22436.localizationsState.resourcesFor<T>(type);", StringComparison.Ordinal)
            .Replace("this._childElements.Remove(((Element)child).slot)", "this._childElements.Remove(DartRuntimePrimitives.ConvertValue<long>(((Element)child).slot))", StringComparison.Ordinal)
            .Replace("this._childElements.ContainsKey(((Element)child).slot)", "this._childElements.ContainsKey(DartRuntimePrimitives.ConvertValue<long>(((Element)child).slot))", StringComparison.Ordinal)
            .Replace("onPopInvoked ?? _defaultPopInvokedHandler", "onPopInvoked ?? ((didPop, result) => _defaultPopInvokedHandler(didPop, result))", StringComparison.Ordinal)
            .Replace("createInnerBallisticScrollActivity(this.@delegate, this.velocity)", "createInnerBallisticScrollActivity(DartRuntimePrimitives.ConvertValue<_NestedScrollPosition__nested_scroll_view>(this.@delegate), this.velocity)", StringComparison.Ordinal)
            .Replace("nestOffset(value, this.@delegate)", "nestOffset(value, DartRuntimePrimitives.ConvertValue<_NestedScrollPosition__nested_scroll_view>(this.@delegate))", StringComparison.Ordinal)
            .Replace("InsertRange(_insertionIndex(below, above),", "InsertRange(checked((int)_insertionIndex(below, above)),", StringComparison.Ordinal)
            .Replace("old__32464.removeAll(newEntriesList__31689.Cast<object>())", "old__32464.RemoveAll(item => newEntriesList__31689.Contains(item))", StringComparison.Ordinal)
            .Replace("this.overlayLocation!._detachFromLayoutSurrogate(deferredChild__101178)", "this.overlayLocation!._detachFromLayoutSurrogate(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Rendering.RenderDeferredLayoutBox>(deferredChild__101178))", StringComparison.Ordinal)
            .Replace("this.widget.onCloseRequested((global::System.Action<bool>)this.close)", "this.widget.onCloseRequested(() => this.close())", StringComparison.Ordinal)
            .Replace("defaultValue: (((this.semanticsTooltip is null) || (this.semanticsTooltip!.Length == 0)) ? null : global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.kNoDefaultValue)", "defaultValue: (((this.semanticsTooltip is null) || (this.semanticsTooltip!.Length == 0)) ? null : global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.kNoDefaultValue.ToString())", StringComparison.Ordinal)
            .Replace("_ReorderableItemGlobalKey__reorderable_list.Create(((Widget)child__40762).key!, index, this)", "_ReorderableItemGlobalKey__reorderable_list.Create(key: ((Widget)child__40762).key!, index: index, state: this)", StringComparison.Ordinal)
            .Replace("await delegateRouteSetter()(data)", "await delegateRouteSetter()(DartRuntimePrimitives.ConvertValue<T>(data))", StringComparison.Ordinal)
            .Replace("public virtual void takePriority() => this._children.clear();", "public virtual void takePriority() => ((dynamic)this._children).clear();", StringComparison.Ordinal)
            .Replace("this._history.where(_RouteEntry__navigator.isRoutePredicate(route))", "this._history.where((entry) => _RouteEntry__navigator.isRoutePredicate(route)(DartRuntimePrimitives.ConvertValue<_RouteEntry__navigator>(entry)))", StringComparison.Ordinal)
            .Replace("(minOverscrollLength ?? minLength)", "DartRuntimePrimitives.RequireValue(minOverscrollLength ?? minLength)", StringComparison.Ordinal)
            .Replace("((ScrollAwareImageProvider<object>)((ScrollAwareImageProvider<object>)__other))", "((ScrollAwareImageProvider<T>)(object)__other)", StringComparison.Ordinal)
            .Replace("semanticIndexCallback ?? Scroll_delegateLibrary._kDefaultSemanticIndexCallback", "semanticIndexCallback ?? ((widget, index) => Scroll_delegateLibrary._kDefaultSemanticIndexCallback(widget, index))", StringComparison.Ordinal)
            .Replace("treeRowExtentBuilder ?? TreeSliver<T>.defaultTreeRowExtentBuilder", "treeRowExtentBuilder ?? ((node, dimensions) => TreeSliver<T>.defaultTreeRowExtentBuilder(node, dimensions))", StringComparison.Ordinal)
            .Replace("return callingAction__81081.invoke(intent);", "return ((dynamic)callingAction__81081).invoke(intent);", StringComparison.Ordinal)
            .Replace("DartAsyncRuntime.scheduleMicrotask((global::System.Action<Duration?>)runScheduledTask)", "DartAsyncRuntime.scheduleMicrotask(() => runScheduledTask(null))", StringComparison.Ordinal)
            .Replace("(global::System.Func<global::Doroti.Generated.Framework.Rendering.Selectable, global::Doroti.Generated.Framework.Rendering.Selectable, long>)this.compareOrder", "((left, right) => this.compareOrder(left, right))", StringComparison.Ordinal)
            .Replace("return _adjustingSelectionEnd = (forward != isReversed__68050);", "return DartRuntimePrimitives.RequireValue(_adjustingSelectionEnd = (forward != isReversed__68050));", StringComparison.Ordinal)
            .Replace("new FutureBuilder<object?>(future: this._sensitiveContentRegistrationFuture", "new FutureBuilder<object?>(future: DartRuntimePrimitives.ConvertValue<Future<object?>>(this._sensitiveContentRegistrationFuture)", StringComparison.Ordinal)
            .Replace("var (enabled__36562, invokeResult__36579) = Actions.of(context__36354).invokeActionIfEnabled(action__36418, intent__36280, context__36354);", "var (enabled__36562, invokeResult__36579) = DartRuntimePrimitives.ConvertValue<(bool, object?)>(Actions.of(context__36354).invokeActionIfEnabled(action__36418, intent__36280, context__36354));", StringComparison.Ordinal)
            .Replace("RenderViewportBase.showInViewport", "RenderViewportBase<global::Doroti.Generated.Framework.Rendering.ContainerParentDataMixin<global::Doroti.Generated.Framework.Rendering.RenderSliver>>.showInViewport", StringComparison.Ordinal)
            .Replace("_setChild(default, __slot)", "_setChild(default(SlotType)!, __slot)", StringComparison.Ordinal);

        // The analyzer represents a zero-argument instance-method tear-off as a
        // forwarding closure in some Flutter libraries. Retain CLR method-group
        // identity for ChangeNotifier listener registration/removal; otherwise
        // each lowered lambda is a distinct delegate and removeListener leaks it.
        source = Regex.Replace(
            source,
            @"\.(addListener|removeListener)\(\(\) => this\.([A-Za-z_][A-Za-z0-9_]*)\(\)\);",
            ".$1(this.$2);");

        if (library.EndsWith("/editable_text.dart", StringComparison.Ordinal))
        {
            source = source
                .Replace("__cascade.cursorHeight = this.cursorHeight", "__cascade.cursorHeight = DartRuntimePrimitives.RequireValue(this.cursorHeight)", StringComparison.Ordinal)
                .Replace("_makeOverridable<TransposeCharactersIntent>((__arg0) => ((Action<TransposeCharactersIntent>)this._transposeCharactersAction)(DartRuntimePrimitives.ConvertValue<TransposeCharactersIntent>(__arg0)))", "_makeOverridable<TransposeCharactersIntent>(this._transposeCharactersAction)", StringComparison.Ordinal)
                .Replace("return ((global::System.Action)((global::System.Func<Future>)(", "return DartRuntimePrimitives.AdaptAsyncCallback((global::System.Func<Future>?)(", StringComparison.Ordinal);
            source = Regex.Replace(
                source,
                @"(return DartRuntimePrimitives\.AdaptAsyncCallback[\s\S]*?\}\) : null)\)\)\);",
                "$1));");
        }

        if (library.EndsWith("/form.dart", StringComparison.Ordinal))
        {
            source = source
                .Replace("(global::System.Action<bool, object>)", "(object?)", StringComparison.Ordinal)
                .Replace("onPopInvokedWithResult: (object?)((Form)(object)this.widget)._callPopInvoked", "onPopInvokedWithResult: (global::System.Action<bool, object?>)((Form)(object)this.widget)._callPopInvoked", StringComparison.Ordinal);
        }

        if (library.EndsWith("/framework.dart", StringComparison.Ordinal))
        {
            source = source.Replace(
                "public static GlobalKey<T> Create(string? debugLabel = null) => ((GlobalKey<T>)(object?)LabeledGlobalKey<T>.Create(debugLabel));",
                "public static GlobalKey<T> Create(string? debugLabel = null) => new LabeledGlobalKey<T>(debugLabel);",
                StringComparison.Ordinal)
                .Replace(
                "global::Doroti.Generated.Framework.Foundation.FlutterError.Create(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode>",
                "new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode>",
                StringComparison.Ordinal)
                .Replace("global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<GlobalKey<IState>>.Create(", "new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<GlobalKey<IState>>(", StringComparison.Ordinal)
                .Replace(
                    "        this._state = widget.createState();\n    }",
                    "        this._state = widget.createState();\n        this._state!._element = this;\n        this._state!._widget = widget;\n    }",
                    StringComparison.Ordinal)
                .Replace(
                    "            _child = updateChild(((Element)(object)null), built__246655, this.slot);\n        }\n    }\n\n    public abstract Widget build();",
                    "            _child = updateChild(((Element)(object)null), built__246655, this.slot);\n        }\n        base.performRebuild();\n    }\n\n    public abstract Widget build();",
                    StringComparison.Ordinal)
                .Replace(
                    "_inheritedElements = this._parent?._inheritedElements;",
                    "_inheritedElements = this._parent?._inheritedElements ?? global::Doroti.Generated.Framework.Foundation.PersistentHashMap<Type, InheritedElement>.CreateEmpty();",
                    StringComparison.Ordinal)
                .Replace("((dynamic)this.state)._element = null;", "this.state._element = null;", StringComparison.Ordinal);
            source = Regex.Replace(
                source,
                @"var (renderObject__\d+) = \(\(global::Doroti\.Generated\.Framework\.Rendering\.RenderObjectWithChildMixin<global::Doroti\.Generated\.Framework\.Rendering\.RenderObject>\?\)\(object\?\)this\.renderObject\)!;",
                "dynamic $1 = this.renderObject;");
            source = Regex.Replace(
                source,
                @"\(\(global::Doroti\.Generated\.Framework\.Rendering\.RenderObjectWithChildMixin<global::Doroti\.Generated\.Framework\.Rendering\.RenderObject>\)(renderObject__\d+)\)\.child",
                "((dynamic)$1).child");
            source = Regex.Replace(
                source,
                @"\(\(dynamic\)(renderObject__\d+)\)\.child = child;",
                "((dynamic)$1).child = (dynamic)child;");
            source = Regex.Replace(
                source,
                @"global::Doroti\.Generated\.Framework\.Rendering\.ContainerRenderObjectMixin<global::Doroti\.Generated\.Framework\.Rendering\.RenderObject, global::Doroti\.Generated\.Framework\.Rendering\.ContainerParentDataMixin<global::Doroti\.Generated\.Framework\.Rendering\.RenderObject>> (renderObject__\d+) = DartRuntimePrimitives\.ConvertValue<global::Doroti\.Generated\.Framework\.Rendering\.ContainerRenderObjectMixin<global::Doroti\.Generated\.Framework\.Rendering\.RenderObject, global::Doroti\.Generated\.Framework\.Rendering\.ContainerParentDataMixin<global::Doroti\.Generated\.Framework\.Rendering\.RenderObject>>>\(this\.renderObject\);",
                "dynamic $1 = this.renderObject;");
            source = Regex.Replace(source, @"(renderObject__\d+)\.insert\(child, after: \(\(IndexedSlot<Element\?>\)__slot\)\.value\?\.renderObject\);", "$1.insert((dynamic)child, after: (dynamic?)((IndexedSlot<Element?>)__slot).value?.renderObject);");
            source = Regex.Replace(source, @"\(\(dynamic\)(renderObject__\d+)\)\.move\(child, after: \(\(IndexedSlot<Element\?>\)__newSlot\)\.value\?\.renderObject\);", "((dynamic)$1).move((dynamic)child, after: (dynamic?)((IndexedSlot<Element?>)__newSlot).value?.renderObject);");
            source = Regex.Replace(source, @"\(\(dynamic\)(renderObject__\d+)\)\.remove\(child\);", "((dynamic)$1).remove((dynamic)child);");
        }

        if (library.EndsWith("/navigator.dart", StringComparison.Ordinal))
        {
            source = source
                .Replace(
                    "public class Navigator : StatefulWidget\n{",
                    "public class Navigator : StatefulWidget\n{\n    internal static readonly List<Page<object>> _defaultPages = new();",
                    StringComparison.Ordinal)
                .Replace(
                    "List<Page<object>> __pages = pages ?? new List<Page<object>>();",
                    "List<Page<object>> __pages = pages ?? _defaultPages;",
                    StringComparison.Ordinal)
                .Replace(
                    "internal virtual bool _usingPagesAPI => DartRuntimePrimitives.ConvertValue<bool>((!object.Equals(((Navigator)(object)this.widget).pages, new List<Page<object>>())));",
                    "internal virtual bool _usingPagesAPI => !ReferenceEquals(((Navigator)(object)this.widget).pages, Navigator._defaultPages);",
                    StringComparison.Ordinal);
        }

        if (library.EndsWith("/rendering/table.dart", StringComparison.Ordinal))
        {
            source = source.Replace(
                "        System.Diagnostics.Debug.Assert(((rows is null) || (children is null)));",
                "        System.Diagnostics.Debug.Assert(((rows is null) || (children is null)));\n        _children = new List<RenderBox?>(System.Linq.Enumerable.Repeat<RenderBox?>(null, checked((int)(this._columns * this._rows))));\n        this.rowDecorations = rowDecorations;\n        children?.forEach(this.addRow);",
                StringComparison.Ordinal);
        }

        if (library.EndsWith("/focus_manager.dart", StringComparison.Ordinal))
        {
            source = source
                .Replace(
                "    public FocusManager()\n    {\n    }",
                "    public FocusManager()\n    {\n        this.rootScope._manager = this;\n    }",
                StringComparison.Ordinal)
                .Replace(
                    "            node__80998._notify();",
                    "            node__80998?._notify();",
                    StringComparison.Ordinal)
                .Replace(
                    "            ((FocusScopeNode)ancestor__49778)._focusedChildren.Remove(scopeFocus__49729);",
                    "            if (((FocusScopeNode)ancestor__49778)._focusedChildren.Contains(scopeFocus__49729)) ((FocusScopeNode)ancestor__49778)._focusedChildren.Remove(scopeFocus__49729);",
                    StringComparison.Ordinal);
        }

        if (library.EndsWith("/binding.dart", StringComparison.Ordinal))
        {
            source = source
                .Replace(
                "    public virtual ImageCache _imageCache { get; set; } = default!;",
                "    public WidgetsFlutterBinding(PlatformDispatcher? platformDispatcher = null) : base(platformDispatcher) { }\n\n    public virtual ImageCache _imageCache { get; set; } = default!;",
                StringComparison.Ordinal)
                .Replace(
                "        PaintingBinding._instance = this;\n        this._imageCache = createImageCache();",
                "        PaintingBinding._instance = this;\n        global::Doroti.Generated.Framework.Semantics.SemanticsBinding._instance = this;\n        global::Doroti.Generated.Framework.Rendering.RendererBinding._instance = this;\n        WidgetsBinding._instance = this;\n        this._buildOwner = new BuildOwner();\n        this._buildOwner.onBuildScheduled = _handleBuildScheduled;\n        this._imageCache = createImageCache();",
                StringComparison.Ordinal);
        }

        if (library.EndsWith("/overlay.dart", StringComparison.Ordinal))
        {
            source = source
                .Replace("this._entries.Cast<Overlay>().ToList(), newEntriesList__31689.Cast<Overlay>().ToList()", "this._entries, newEntriesList__31689", StringComparison.Ordinal)
                .Replace("global::Doroti.Generated.Framework.Rendering.RenderDeferredLayoutBox", "_RenderDeferredLayoutBox__overlay", StringComparison.Ordinal)
                .Replace("old__32464.RemoveAll(item => newEntriesList__31689.Contains(item))", "old__32464.ExceptWith(newEntriesList__31689)", StringComparison.Ordinal);
        }

        if (library.EndsWith("/platform_menu_bar.dart", StringComparison.Ordinal))
        {
            source = source.Replace(
                "newDescendants__19362.Cast<PlatformMenuBar>().ToList(), this.descendants.Cast<PlatformMenuBar>().ToList()",
                "newDescendants__19362, this.descendants",
                StringComparison.Ordinal);
        }

        if (library.EndsWith("/overscroll_indicator.dart", StringComparison.Ordinal))
        {
            source = source.Replace("})));\n        this._controller?.dispose();", "return default!;\n})));\n        this._controller?.dispose();", StringComparison.Ordinal);
        }

        if (library.EndsWith("/platform_view.dart", StringComparison.Ordinal))
        {
            source = Regex.Replace(source, @"_DarwinViewState__platform_view<[^;]+?>\._emptyRecognizersSet", "_emptyRecognizersSet");
        }

        if (library.EndsWith("/reorderable_list.dart", StringComparison.Ordinal))
        {
            source = source.Replace("_ReorderableItem__reorderable_list.Create(key:", "new _ReorderableItem__reorderable_list(", StringComparison.Ordinal);
        }

        if (library.EndsWith("/router.dart", StringComparison.Ordinal))
        {
            source = source.Replace(", () => callback())", ", (global::System.Action)(() => callback()))", StringComparison.Ordinal);
        }

        if (library.EndsWith("/scroll_view.dart", StringComparison.Ordinal))
        {
            source = Regex.Replace(
                source,
                @"ScrollViewKeyboardDismissBehavior effectiveKeyboardDismissBehavior__\d+ = .+;",
                "ScrollViewKeyboardDismissBehavior effectiveKeyboardDismissBehavior__22037 = DartRuntimePrimitives.RequireValue(this.keyboardDismissBehavior);");
        }

        if (library.EndsWith("/scrollbar.dart", StringComparison.Ordinal))
        {
            source = source
                .Replace("DartMap<Type, GestureRecognizerFactory<global::Doroti.Generated.Framework.Gestures.GestureRecognizer>> _gestures", "DartMap<Type, dynamic> _gestures", StringComparison.Ordinal)
                .Replace("return ((DartMap<Type, object>)(object?)gestures__75996);", "return gestures__75996;", StringComparison.Ordinal)
                .Replace("foregroundPainter: this.scrollbarPainter", "foregroundPainter: new _ScrollbarCustomPainterAdapter(this.scrollbarPainter)", StringComparison.Ordinal)
                .Replace("internal virtual Timer? _fadeoutTimer { get; set; } = default;", "internal virtual Timer? _fadeoutTimer { get; set; } = default;\n    internal virtual bool _isDisposed { get; set; }", StringComparison.Ordinal)
                .Replace("this._fadeoutAnimationController.reverse();\n_fadeoutTimer = null;", "if (!this._isDisposed)\n{\n    try\n    {\n        this._fadeoutAnimationController.reverse();\n    }\n    catch (ObjectDisposedException)\n    {\n        // The host dispatcher can finish before a delayed scrollbar fade.\n    }\n}\n_fadeoutTimer = null;", StringComparison.Ordinal)
                .Replace("this._fadeoutAnimationController.dispose();\n        this._fadeoutTimer?.cancel();", "_isDisposed = true;\n        this._fadeoutTimer?.cancel();\n        _fadeoutTimer = null;\n        this._fadeoutAnimationController.dispose();", StringComparison.Ordinal);
            source = Regex.Replace(
                source,
                @"\(\(ScrollbarPainter\?\)\(object\?\)\(\(CustomPaint\)(?<paint>customPaint__\d+)\)\.foregroundPainter!\)!",
                "_ScrollbarCustomPainterAdapter.Unwrap(((CustomPaint)${paint}).foregroundPainter!)");
            source += "\ninternal sealed class _ScrollbarCustomPainterAdapter : global::Doroti.Generated.Framework.Rendering.CustomPainter\n{\n    private readonly ScrollbarPainter _owner;\n    internal _ScrollbarCustomPainterAdapter(ScrollbarPainter owner) : base(owner) => _owner = owner;\n    internal static ScrollbarPainter Unwrap(global::Doroti.Generated.Framework.Rendering.CustomPainter painter) => painter is _ScrollbarCustomPainterAdapter adapter ? adapter._owner : (ScrollbarPainter)(object)painter;\n    public override void paint(Canvas canvas, Size size) => _owner.paint(canvas, size);\n    public override bool shouldRepaint(global::Doroti.Generated.Framework.Rendering.CustomPainter oldDelegate) => oldDelegate is not _ScrollbarCustomPainterAdapter other || _owner.shouldRepaint(other._owner);\n    public override bool? hitTest(Offset position) => _owner.hitTest(position);\n}\n";
        }

        if (library.EndsWith("/selectable_region.dart", StringComparison.Ordinal) ||
            library.EndsWith("/text.dart", StringComparison.Ordinal))
        {
            source = source.Replace(
                "DartRuntimePrimitives.ConvertValue<Comparison<global::Doroti.Generated.Framework.Rendering.Selectable>>(_compareScreenOrder)",
                "new Comparison<global::Doroti.Generated.Framework.Rendering.Selectable>((left, right) => checked((int)_compareScreenOrder(left, right)))",
                StringComparison.Ordinal);
        }

        if (library.EndsWith("/shortcuts.dart", StringComparison.Ordinal))
        {
            source = source.Replace("DartRuntimePrimitives.ConvertValue<(bool, object?)>(Actions.of(context__36354).invokeActionIfEnabled", "DartRuntimePrimitives.ConvertValue<(bool, object?)>((object)Actions.of(context__36354).invokeActionIfEnabled", StringComparison.Ordinal)
                .Replace("intent__36280, context__36354));", "intent__36280, context__36354));", StringComparison.Ordinal);
        }

        if (library.EndsWith("/sliver.dart", StringComparison.Ordinal))
        {
            source = source.Replace("newChildren__37713.putIfAbsent(index__39165, (() => { }));", "newChildren__37713.putIfAbsent(index__39165, (() => default!));", StringComparison.Ordinal);
        }

        if (library.EndsWith("/sliver_tree.dart", StringComparison.Ordinal))
        {
            source = source
                .Replace("ItemExtentBuilder __treeRowExtentBuilder", "global::System.Func<TreeSliverNode<T>, global::Doroti.Generated.Framework.Rendering.SliverLayoutDimensions, double?> __treeRowExtentBuilder", StringComparison.Ordinal)
                .Replace("semanticIndexCallback ?? Sliver_treeLibrary._kDefaultSemanticIndexCallback", "semanticIndexCallback ?? ((widget, index) => Sliver_treeLibrary._kDefaultSemanticIndexCallback(widget, index))", StringComparison.Ordinal)
                .Replace("activeAnimations: this._activeAnimations, itemBuilder:", "activeAnimations: this._activeAnimations.cast<global::Doroti.Generated.Framework.Foundation.UniqueKey, (long fromIndex, long toIndex, double value)>(), itemBuilder:", StringComparison.Ordinal);
        }

        if (library.EndsWith("/sliver_resizing_header.dart", StringComparison.Ordinal))
        {
            source = source
                .Replace("public virtual IEnumerable<_Slot__sliver_resizing_header> slots", "public override IEnumerable<_Slot__sliver_resizing_header> slots", StringComparison.Ordinal)
                .Replace("public virtual Widget? childForSlot(_Slot__sliver_resizing_header slot)", "public override Widget? childForSlot(_Slot__sliver_resizing_header slot)", StringComparison.Ordinal);
        }

        if (library.EndsWith("/slotted_render_object_widget.dart", StringComparison.Ordinal))
        {
            source = source
                .Replace(" : RenderObjectWidget where ChildType", " : RenderObjectWidget, SlottedMultiChildRenderObjectWidgetMixin<SlotType, ChildType> where ChildType", StringComparison.Ordinal)
                .Replace("    public override SlottedRenderObjectElement<SlotType, ChildType> createElement()", "    public abstract IEnumerable<SlotType> slots { get; }\n    public abstract Widget? childForSlot(SlotType slot);\n\n    public override SlottedRenderObjectElement<SlotType, ChildType> createElement()", StringComparison.Ordinal)
                .Replace(" : base(widget)", " : base((RenderObjectWidget)widget)", StringComparison.Ordinal)
                .Replace("base.update(__newWidget)", "base.update((Widget)__newWidget)", StringComparison.Ordinal);
        }

        if (library.EndsWith("/single_child_scroll_view.dart", StringComparison.Ordinal))
        {
            source = source.Replace(
                "RenderViewportBase<global::Doroti.Generated.Framework.Rendering.ContainerParentDataMixin<global::Doroti.Generated.Framework.Rendering.RenderSliver>>.showInViewport(descendant: descendant, viewport: this, offset: this.offset, rect: rect, duration: duration, curve: curve)",
                "_showInViewport(descendant, rect, this, this.offset, duration, curve)",
                StringComparison.Ordinal);
            source = source.Replace(
                "    public override void debugFillProperties",
                "    private static Rect? _showInViewport(global::Doroti.Generated.Framework.Rendering.RenderObject? descendant, Rect? rect, _RenderSingleChildViewport__single_child_scroll_view viewport, global::Doroti.Generated.Framework.Rendering.ViewportOffset offset, Duration duration, global::Doroti.Generated.Framework.Animation.Curve curve) { if (descendant is null) return rect; var leading = viewport.getOffsetToReveal(descendant, 0.0, rect: rect); var trailing = viewport.getOffsetToReveal(descendant, 1.0, rect: rect); var target = global::Doroti.Generated.Framework.Rendering.RevealedOffset.clampOffset(leading, trailing, offset.pixels); if (target is null) return rect ?? descendant.paintBounds; _ = offset.moveTo(target.offset, duration: duration, curve: curve); return target.rect; }\n\n    public override void debugFillProperties",
                StringComparison.Ordinal);
        }

        if (library.EndsWith("/scroll_view.dart", StringComparison.Ordinal))
        {
            source = source
                .Replace("primary: DartRuntimePrimitives.RequireValue(primary)", "primary: primary", StringComparison.Ordinal)
                .Replace("cacheExtent: DartRuntimePrimitives.RequireValue(cacheExtent)", "cacheExtent: cacheExtent", StringComparison.Ordinal)
                .Replace("keyboardDismissBehavior: DartRuntimePrimitives.RequireValue(keyboardDismissBehavior)", "keyboardDismissBehavior: keyboardDismissBehavior", StringComparison.Ordinal)
                .Replace("semanticChildCount: DartRuntimePrimitives.RequireValue(semanticChildCount)", "semanticChildCount: semanticChildCount", StringComparison.Ordinal)
                .Replace("checked((long)(children.Count))", "checked((long)((children ?? new List<Widget>()).Count))", StringComparison.Ordinal);
        }

        if (library.EndsWith("/page_view.dart", StringComparison.Ordinal))
        {
            source = source.Replace(
                "initialPixels: ((double)(object)null)",
                "initialPixels: null",
                StringComparison.Ordinal);
        }

        if (library.EndsWith("/ticker_provider.dart", StringComparison.Ordinal))
        {
            source = source
                .Replace("return (((object?)widget__4995?.notifier ?? (object?)new _ConstantValueListenable__ticker_provider<bool>(true)));", "return DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Foundation.ValueListenable<bool>>((object?)widget__4995?.notifier ?? (object?)new _ConstantValueListenable__ticker_provider<bool>(true));", StringComparison.Ordinal)
                .Replace("return (((object?)widget__8322?.valuesNotifier ?? (object?)fallback__7914));", "return DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData>>((object?)widget__8322?.valuesNotifier ?? (object?)fallback__7914);", StringComparison.Ordinal);
        }

        if (library.EndsWith("/text_selection.dart", StringComparison.Ordinal))
        {
            source = source.Replace(
                "this.editableText.userUpdateTextEditingValue(((EditableTextState)this.editableText).textEditingValue.copyWith(selection: newSelection__116605), cause)",
                "this.editableText.userUpdateTextEditingValue(((EditableTextState)this.editableText).textEditingValue.copyWith(selection: newSelection__116605), DartRuntimePrimitives.RequireValue(cause))",
                StringComparison.Ordinal);
        }

        if (library.EndsWith("/widgets/routes.dart", StringComparison.Ordinal))
        {
            source = source
                .Replace(
                    "            return this.popDisposition;\n            return default!;",
                    "            return base.popDisposition;\n            return default!;",
                    StringComparison.Ordinal)
                .Replace(
                    "        DartRuntimePrimitives.Ignore(disposed.then((global::System.Action<object>)((_) => {",
                    "        if (disposed is not null)\n        {\n            DartRuntimePrimitives.Ignore(disposed.then((global::System.Action<object>)((_) => {",
                    StringComparison.Ordinal)
                .Replace(
                    "})));\n    }\n\n    public virtual bool canTransitionTo",
                    "})));\n        }\n    }\n\n    public virtual bool canTransitionTo",
                    StringComparison.Ordinal);
        }

        if (library.EndsWith("/animation/animation_controller.dart", StringComparison.Ordinal))
        {
            source = source.Replace(
                "        System.Diagnostics.Debug.Assert((upperBound >= lowerBound));\n    }",
                "        System.Diagnostics.Debug.Assert((upperBound >= lowerBound));\n        this._ticker = vsync.createTicker(this._tick);\n        this._internalSetValue(value ?? lowerBound);\n    }",
                StringComparison.Ordinal);
        }

        if (library.EndsWith("/widget_inspector.dart", StringComparison.Ordinal))
        {
            source = ApplyG53WidgetInspectorCompatibility(source);
        }

        return source;
    }

    private static string ApplyG53WidgetInspectorCompatibility(string source) => source
        .Replace("global::System.Func<object> callback", "dynamic callback", StringComparison.Ordinal)
        .Replace("global::System.Func<string, object> callback", "dynamic callback", StringComparison.Ordinal)
        .Replace("global::System.Func<string?, string, object> callback", "dynamic callback", StringComparison.Ordinal)
        .Replace("(global::System.Action<global::Doroti.Generated.Framework.Foundation.FlutterErrorDetails>)(value ? this._reportStructuredError : defaultExceptionHandler__36848)", "(value ? (global::Doroti.Generated.Framework.Foundation.FlutterExceptionHandler)this._reportStructuredError : defaultExceptionHandler__36848)", StringComparison.Ordinal)
        .Replace("this._objectToId.GetValueOrDefault(@object)", "this._objectToId[@object]", StringComparison.Ordinal)
        .Replace("pubRootDirectories = DartRuntimePrimitives.ConvertValue<global::System.Func<DartMap<string, string>, Future<DartMap<string, object>>>>(pubRootDirectories.map<string, string>", "pubRootDirectories = pubRootDirectories.map<string, string>", StringComparison.Ordinal)
        .Replace(".path)).ToList());", ".path)).ToList();", StringComparison.Ordinal)
        .Replace("this.selection.currentElement = @object", "this.selection.currentElement = (Element)@object", StringComparison.Ordinal)
        .Replace("this.selection.current = @object", "this.selection.current = (global::Doroti.Generated.Framework.Rendering.RenderObject)@object", StringComparison.Ordinal)
        .Replace("global::Doroti.Generated.Framework.Foundation.DebugLibrary.connectedVmServiceUri,", "global::Doroti.Generated.Framework.Foundation.DebugLibrary.connectedVmServiceUri.ToString(),", StringComparison.Ordinal)
        .Replace("DartUri.parse(global::Doroti.Generated.Framework.Foundation.DebugLibrary.activeDevToolsServerAddress!)", "DartUri.parse(global::Doroti.Generated.Framework.Foundation.DebugLibrary.activeDevToolsServerAddress!.ToString())", StringComparison.Ordinal)
        .Replace("replace(queryParameters: new DartMap<string, object>", "replace(queryParameters: new DartMap<string, string>", StringComparison.Ordinal)
        .Replace("_getRawElementParentChain(element, numLocalParents: numLocalParents))", "_getRawElementParentChain(element, numLocalParents: numLocalParents).Cast<global::Doroti.Generated.Framework.Foundation.Diagnosticable>().ToList())", StringComparison.Ordinal)
        .Replace("node__66037.getProperties(),", "node__66037.getProperties().ToList(),", StringComparison.Ordinal)
        .Replace("_filterChildren(node.getChildren(),", "_filterChildren(node.getChildren().ToList(),", StringComparison.Ordinal)
        .Replace("_renderObjectOrNull(@object)", "_renderObjectOrNull((Element)@object)", StringComparison.Ordinal)
        .Replace("children: diagnostic__22069.getChildren()", "children: diagnostic__22069.getChildren().ToList()", StringComparison.Ordinal)
        .Replace("postEvent(eventName, stats.exportToJson(this._frameStart, frameNumber: this._frameNumber))", "postEvent(eventName, stats.exportToJson(this._frameStart, frameNumber: this._frameNumber).cast<object, object>())", StringComparison.Ordinal)
        .Replace("System.Linq.Enumerable.Any(errorSummary.value)", "!string.IsNullOrEmpty(errorSummary.value?.ToString())", StringComparison.Ordinal)
        .Replace("!string.IsNullOrEmpty(errorSummary.value)", "!string.IsNullOrEmpty(errorSummary.value?.ToString())", StringComparison.Ordinal)
        .Replace("errorSummary.value.First()", "errorSummary.value", StringComparison.Ordinal)
        .Replace("foreach (MapEntry<global::Doroti.Flutter.Runtime.CreationLocation, long> entry__153282 in Widget_inspectorLibrary._locationToId.entries)", "foreach (var entry__153282 in Widget_inspectorLibrary._locationToId.entries)", StringComparison.Ordinal)
        .Replace("public virtual global::Doroti.Generated.Framework.Foundation.DiagnosticsSerializationDelegate copyWith(long subtreeDepth = default!, bool includeProperties = default!", "public virtual global::Doroti.Generated.Framework.Foundation.DiagnosticsSerializationDelegate copyWith(long? subtreeDepth = null, bool? includeProperties = null", StringComparison.Ordinal);
}
