using System.Text.RegularExpressions;

namespace Doroti.DartToCSharp;

internal static class GeneratedLocalPatternExtensions
{
    internal static string ReplaceGeneratedLocalPattern(
        this string source,
        string before,
        string after,
        StringComparison comparison = StringComparison.Ordinal)
    {
        var locals = Regex.Matches(before, @"\b[A-Za-z_][A-Za-z0-9_]*__\d+\b")
            .Select(match => match.Value)
            .Distinct(StringComparer.Ordinal)
            .ToArray();
        if (locals.Length == 0)
            return source.Replace(before, after, comparison);

        var pattern = Regex.Escape(before);
        var groups = new Dictionary<string, string>(StringComparer.Ordinal);
        for (var index = 0; index < locals.Length; index++)
        {
            var local = locals[index];
            var baseName = local[..local.LastIndexOf("__", StringComparison.Ordinal)];
            var group = $"local{index}";
            groups.Add(local, group);
            pattern = pattern.Replace(
                Regex.Escape(local),
                $"(?<{group}>{Regex.Escape(baseName)}__\\d+)",
                StringComparison.Ordinal);
        }

        return Regex.Replace(source, pattern, match =>
        {
            var replacement = after;
            foreach (var (local, group) in groups)
                replacement = replacement.Replace(local, match.Groups[group].Value, StringComparison.Ordinal);
            return replacement;
        }, RegexOptions.CultureInvariant);
    }
}

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
            .ReplaceGeneratedLocalPattern("global::Doroti.Framework.Widgets.RestorableProperty<object>", "dynamic", StringComparison.Ordinal)
            .ReplaceGeneratedLocalPattern("RestorableProperty<object>", "dynamic", StringComparison.Ordinal);
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
            source = source.ReplaceGeneratedLocalPattern(
                "new DartMap<global::Doroti.Framework.Widgets.WidgetState, Color?>",
                "new DartMap<global::Doroti.Framework.Widgets.WidgetStatesConstraint, Color?>",
                StringComparison.Ordinal);
            foreach (var state in new[] { "pressed", "hovered", "focused" })
            {
                source = source.ReplaceGeneratedLocalPattern(
                    $"[global::Doroti.Framework.Widgets.WidgetState.{state}] =",
                    $"[global::Doroti.Framework.Widgets.WidgetState.{state}.asConstraint()] =",
                    StringComparison.Ordinal);
            }
        }
        source = source
            .ReplaceGeneratedLocalPattern("registerForRestoration(RestorableProperty<object> property", "registerForRestoration(dynamic property", StringComparison.Ordinal)
            .ReplaceGeneratedLocalPattern("unregisterFromRestoration(RestorableProperty<object> property", "unregisterFromRestoration(dynamic property", StringComparison.Ordinal)
            .ReplaceGeneratedLocalPattern("property.addListener(() => listener())", "property.addListener((global::System.Action)(() => listener()))", StringComparison.Ordinal)
            .ReplaceGeneratedLocalPattern("property.removeListener(() => listener())", "property.removeListener((global::System.Action)(() => listener()))", StringComparison.Ordinal)
            .ReplaceGeneratedLocalPattern("property.initWithValue(initialValue__", "property.initWithValue((dynamic)initialValue__", StringComparison.Ordinal);
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
            source = source.ReplaceGeneratedLocalPattern(
                "var __instance = new GridView(default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!);",
                "var __instance = new GridView(key: key, scrollDirection: scrollDirection, reverse: reverse, controller: controller, primary: primary, physics: physics, shrinkWrap: shrinkWrap, padding: padding, cacheExtent: cacheExtent, scrollCacheExtent: scrollCacheExtent, semanticChildCount: semanticChildCount, dragStartBehavior: dragStartBehavior, keyboardDismissBehavior: keyboardDismissBehavior, restorationId: restorationId, clipBehavior: clipBehavior, hitTestBehavior: hitTestBehavior);",
                StringComparison.Ordinal);
        }

        if (library.EndsWith("/widgets/scrollable.dart", StringComparison.Ordinal))
        {
            // A missing persisted scroll offset is null, not numeric zero.
            // Zero overwrites PageController.initialPage during restoration.
            source = source.ReplaceGeneratedLocalPattern(
                "public override double? createDefaultValue() => DartRuntimePrimitives.ConvertValue<double>(null);",
                "public override double? createDefaultValue() => null;",
                StringComparison.Ordinal);
        }

        if (library.EndsWith("/widgets/basic.dart", StringComparison.Ordinal))
        {
            // Named SizedBox factories must retain their forwarded key and child.
            // Dropping child removes selection-handle painters from both the
            // Material square and Cupertino fromSize paths.
            source = source.ReplaceGeneratedLocalPattern(
                "var __instance = new SizedBox(default!, default!, default!, default!);",
                "var __instance = new SizedBox(key: key, child: child);",
                StringComparison.Ordinal);
            source = source.ReplaceGeneratedLocalPattern(
                "var __instance = new Transform(default!, default!, default!, default!, default!, default!, default!);",
                "var __instance = new Transform(key: key, child: child);",
                StringComparison.Ordinal);
            // The analyzer IR carries TextScaler.noScaling as a non-null Dart
            // default, while the CLR optional parameter uses default/null.
            // Normalize before the tuple-pattern switch so the exhaustive Dart
            // cases remain exhaustive when RichText is created by Icon.
            source = source.ReplaceGeneratedLocalPattern(
                "    {\n        return ((textScaler, textScaleFactor) switch",
                "    {\n        textScaler ??= global::Doroti.Framework.Painting.TextScaler.noScaling;\n        return ((textScaler, textScaleFactor) switch",
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
                "((global::Doroti.Framework.Widgets.WidgetStateProperty<Color>)${name}.${property}).resolve(");
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
                "((global::Doroti.Framework.Widgets.WidgetStateProperty<Color>)${name}.${property}).resolve(");
        }

        if (library.EndsWith("/material/switch.dart", StringComparison.Ordinal))
        {
            source = Regex.Replace(source,
                @"SwitchThemeData (?<name>defaults__\d+) = default!;",
                "dynamic ${name} = default!;");
            source = Regex.Replace(source,
                @"(?<name>defaults__\d+)\.(?<property>thumbColor|trackColor|trackOutlineColor|overlayColor)!?\.resolve\(",
                "((global::Doroti.Framework.Widgets.WidgetStateProperty<Color>)${name}.${property}).resolve(");
            source = Regex.Replace(source,
                @"(?<name>defaults__\d+)\.trackOutlineWidth\?\.resolve\(",
                "((global::Doroti.Framework.Widgets.WidgetStateProperty<double>)${name}.trackOutlineWidth).resolve(");
        }

        if (library.EndsWith("/material/text_field.dart", StringComparison.Ordinal))
        {
            // A nullable spread in Dart contributes zero elements. Preserve
            // that contract when collection literals lower to AddRange.
            source = Regex.Replace(source,
                @"\.AddRange\(\(\(TextField\)this\.widget\)\.inputFormatters\);",
                ".AddRange(((TextField)this.widget).inputFormatters ?? []);");
            source = source.ReplaceGeneratedLocalPattern(
                "onSelectionHandleTapped: () => this._handleSelectionHandleTapped()",
                "onSelectionHandleTapped: (global::System.Action)this._handleSelectionHandleTapped",
                StringComparison.Ordinal);
        }

        if (library.EndsWith("/material/selectable_text.dart", StringComparison.Ordinal))
        {
            source = source.ReplaceGeneratedLocalPattern(
                "onSelectionHandleTapped: () => this._handleSelectionHandleTapped()",
                "onSelectionHandleTapped: (global::System.Action)this._handleSelectionHandleTapped",
                StringComparison.Ordinal);
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
                .ReplaceGeneratedLocalPattern("        __instance._ticker = vsync.createTicker(__instance._tick);\n", "", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("        __instance._internalSetValue(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(value)));\n", "", StringComparison.Ordinal);
        }

        if (library.EndsWith("/rendering/table.dart", StringComparison.Ordinal))
        {
            source = source
                .ReplaceGeneratedLocalPattern(
                    "if (((this._children[(int)(xyOld__29146)] is not null) &&",
                    "if (((xyOld__29146 < checked((long)(this._children.Count))) && (this._children[(int)(xyOld__29146)] is not null) &&",
                    StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern(
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
            source = source.ReplaceGeneratedLocalPattern(
                "public static double strokeAlignInside = -1.0;",
                "public const double strokeAlignInside = -1.0;",
                StringComparison.Ordinal);
        }

        if (library.EndsWith("/material/material.dart", StringComparison.Ordinal))
        {
            source = source.ReplaceGeneratedLocalPattern(
                "List<InkFeature>? inkFeatures__23989 = this._inkFeatures?.ToList();",
                "List<InkFeature>? inkFeatures__23989 = this._inkFeatures;",
                StringComparison.Ordinal);
        }

        if (library.EndsWith("ink_well.dart", StringComparison.Ordinal))
        {
            source = source.ReplaceGeneratedLocalPattern(
                "value == (((this._highlights.ContainsKey(type)) && this._highlights.GetValueOrDefault(type)!.active))",
                "value == ((this._highlights.GetValueOrDefault(type) is InkHighlight currentHighlight) && currentHighlight.active)",
                StringComparison.Ordinal);
        }

        if (library.EndsWith("/gestures/hit_test.dart", StringComparison.Ordinal))
        {
            source = source
                .ReplaceGeneratedLocalPattern(
                    "double? epsilon = null)",
                    "double epsilon = 1e-10)",
                    StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern(
                    "component.abs() < DartRuntimePrimitives.RequireValue(epsilon)",
                    "component.abs() < epsilon",
                    StringComparison.Ordinal);
        }

        if (library.EndsWith("/rendering/sliver_multi_box_adaptor.dart", StringComparison.Ordinal))
        {
            source = source.ReplaceGeneratedLocalPattern(
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
            source = source.ReplaceGeneratedLocalPattern(
                "        WidgetsBinding._instance = this;\n        this._buildOwner = new BuildOwner();",
                """
                        WidgetsBinding._instance = this;
                        this._accessibilityFeatures = this.platformDispatcher.accessibilityFeatures;
                        this.platformDispatcher.onSemanticsEnabledChanged = this._handleSemanticsEnabledChanged;
                        this.platformDispatcher.onSemanticsActionEvent = this._handleSemanticsActionEvent;
                        this.platformDispatcher.onAccessibilityFeaturesChanged = () =>
                        {
                            if (global::Doroti.Framework.Scheduler.SchedulerBinding.instance.schedulerPhase ==
                                global::Doroti.Framework.Scheduler.SchedulerPhase.persistentCallbacks)
                            {
                                global::Doroti.Framework.Scheduler.SchedulerBinding.instance.addPostFrameCallback(
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
            source = source.ReplaceGeneratedLocalPattern(
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
            source = source.ReplaceGeneratedLocalPattern(
                "public virtual global::Doroti.Ui.SceneBuilder createSceneBuilder() => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.SceneBuilder>(new global::Doroti.Ui.SceneBuilder());",
                """
                public virtual global::Doroti.Ui.SceneBuilder createSceneBuilder() =>
                    new global::Doroti.Ui.SceneBuilder(
                        this.platformDispatcher.implicitView?.viewId ??
                        throw new InvalidOperationException("Rendering a scene requires an implicit Flutter view."));
                """,
                StringComparison.Ordinal);
            // RendererBinding and SemanticsBinding provide the first flattened
            // implementation of these callbacks, but WidgetsBinding is the
            // right-most Dart mixin and must also notify its observers. Preserve
            // both the lower-level side effects and the WidgetsBinding contract.
            source = source.ReplaceGeneratedLocalPattern(
                """
                        this._accessibilityFeatures = this.platformDispatcher.accessibilityFeatures;
                    }
                """,
                """
                        this._accessibilityFeatures = this.platformDispatcher.accessibilityFeatures;
                        foreach (var observer in new List<WidgetsBindingObserver>(this._observers))
                        {
                            try
                            {
                                observer.didChangeAccessibilityFeatures();
                            }
                            catch (Exception exception)
                            {
                                var stack = new System.Diagnostics.StackTrace();
                                FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: exception, stack: stack, library: "widgets library", context: new global::Doroti.Framework.Foundation.ErrorDescription("while dispatching notifications for WidgetsBindingObserver.didChangeAccessibilityFeatures")));
                            }
                        }
                    }
                """,
                StringComparison.Ordinal);
            source = source.ReplaceGeneratedLocalPattern(
                """
                            scheduleForcedFrame();
                        }
                    }

                    public virtual void handleTextScaleFactorChanged()
                    {
                    }

                    public virtual void handlePlatformBrightnessChanged()
                    {
                    }
                """,
                """
                            scheduleForcedFrame();
                        }
                        foreach (var observer in new List<WidgetsBindingObserver>(this._observers))
                        {
                            try
                            {
                                observer.didChangeMetrics();
                            }
                            catch (Exception exception)
                            {
                                var stack = new System.Diagnostics.StackTrace();
                                FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: exception, stack: stack, library: "widgets library", context: new global::Doroti.Framework.Foundation.ErrorDescription("while dispatching notifications for WidgetsBindingObserver.didChangeMetrics")));
                            }
                        }
                    }

                    public virtual void handleTextScaleFactorChanged()
                    {
                        foreach (var observer in new List<WidgetsBindingObserver>(this._observers))
                        {
                            try
                            {
                                observer.didChangeTextScaleFactor();
                            }
                            catch (Exception exception)
                            {
                                var stack = new System.Diagnostics.StackTrace();
                                FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: exception, stack: stack, library: "widgets library", context: new global::Doroti.Framework.Foundation.ErrorDescription("while dispatching notifications for WidgetsBindingObserver.didChangeTextScaleFactor")));
                            }
                        }
                    }

                    public virtual void handlePlatformBrightnessChanged()
                    {
                        foreach (var observer in new List<WidgetsBindingObserver>(this._observers))
                        {
                            try
                            {
                                observer.didChangePlatformBrightness();
                            }
                            catch (Exception exception)
                            {
                                var stack = new System.Diagnostics.StackTrace();
                                FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: exception, stack: stack, library: "widgets library", context: new global::Doroti.Framework.Foundation.ErrorDescription("while dispatching notifications for WidgetsBindingObserver.didChangePlatformBrightness")));
                            }
                        }
                    }
                """,
                StringComparison.Ordinal);
        }

        if (library.EndsWith("/rendering/proxy_box.dart", StringComparison.Ordinal))
        {
            // RenderSemanticsAnnotations applies SemanticsAnnotationsMixin.
            // Flattening the mixin must retain its constructor initializer or
            // the first semantics pass observes a null properties object.
            source = source.ReplaceGeneratedLocalPattern(
                """
                    public RenderSemanticsAnnotations(RenderBox? child = null, global::Doroti.Framework.Semantics.SemanticsProperties properties = default!, bool container = false, bool explicitChildNodes = false, bool excludeSemantics = false, bool blockUserActions = false, Locale? localeForSubtree = null, TextDirection? textDirection = null) : base(child)
                    {
                    }
                """,
                """
                    public RenderSemanticsAnnotations(RenderBox? child = null, global::Doroti.Framework.Semantics.SemanticsProperties properties = default!, bool container = false, bool explicitChildNodes = false, bool excludeSemantics = false, bool blockUserActions = false, Locale? localeForSubtree = null, TextDirection? textDirection = null) : base(child)
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
                .ReplaceGeneratedLocalPattern("DateTime.monday", "1L", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("DateTime.daysPerWeek", "7L", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("registerForRestoration(RestorableProperty<object> property", "registerForRestoration(dynamic property", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("unregisterFromRestoration(RestorableProperty<object> property", "unregisterFromRestoration(dynamic property", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("property.addListener(() => listener())", "property.addListener((global::System.Action)(() => listener()))", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern(".indexed", ".indexed()", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern(".sum", ".sum()", StringComparison.Ordinal);
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
                .ReplaceGeneratedLocalPattern("_exitWidgetSelectionButtonBuilder(global::Doroti.Framework.Widgets.BuildContext context, global::System.Action onPressed, string semanticsLabel, global::Doroti.Framework.Widgets.GlobalKey<IState> key)", "_exitWidgetSelectionButtonBuilder(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.GlobalKey<IState> key, global::System.Action onPressed, string semanticsLabel)", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("_tapBehaviorButtonBuilder(global::Doroti.Framework.Widgets.BuildContext context, global::System.Action onPressed, string semanticsLabel, bool selectionOnTapEnabled)", "_tapBehaviorButtonBuilder(global::Doroti.Framework.Widgets.BuildContext context, global::System.Action onPressed, bool selectionOnTapEnabled, string semanticsLabel)", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("return new CupertinoPageRoute<CupertinoApp>(settings: settings, builder:", "return new CupertinoPageRoute<object>(settings: settings, builder:", StringComparison.Ordinal);
        }

        if (library.EndsWith("/material/app.dart", StringComparison.Ordinal))
        {
            source = source.ReplaceGeneratedLocalPattern(
                "return (global::Doroti.Framework.Widgets.Route<object>)(object)new MaterialPageRoute<MaterialApp>(settings: settings, builder:",
                "return new MaterialPageRoute<object>(settings: settings, builder:",
                StringComparison.Ordinal);
        }

        if (library.EndsWith("/cupertino/bottom_tab_bar.dart", StringComparison.Ordinal))
        {
            source = source.ReplaceGeneratedLocalPattern("BorderSide(color: _kDefaultTabBarBorderColor", "new global::Doroti.Framework.Painting.BorderSide(color: Bottom_tab_barLibrary._kDefaultTabBarBorderColor", StringComparison.Ordinal);
        }

        if (library.EndsWith("/cupertino/button.dart", StringComparison.Ordinal) ||
            library.EndsWith("/cupertino/checkbox.dart", StringComparison.Ordinal) ||
            library.EndsWith("/cupertino/radio.dart", StringComparison.Ordinal) ||
            library.EndsWith("/cupertino/switch.dart", StringComparison.Ordinal) ||
            library.EndsWith("/cupertino/menu_anchor.dart", StringComparison.Ordinal))
        {
            const string cursorResolver = "global::System.Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>, global::Doroti.Framework.Services.MouseCursor>";
            source = source
                .ReplaceGeneratedLocalPattern("WidgetStateProperty.resolveWith<global::Doroti.Framework.Services.MouseCursor>(((global::System.Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>, CupertinoButton>)", $"WidgetStateProperty.resolveWith<global::Doroti.Framework.Services.MouseCursor>((({cursorResolver})", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("WidgetStateProperty.resolveWith<global::Doroti.Framework.Services.MouseCursor>(((global::System.Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>, CupertinoCheckbox>)", $"WidgetStateProperty.resolveWith<global::Doroti.Framework.Services.MouseCursor>((({cursorResolver})", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("WidgetStateProperty.resolveWith<global::Doroti.Framework.Services.MouseCursor>(((global::System.Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>, T>)", $"WidgetStateProperty.resolveWith<global::Doroti.Framework.Services.MouseCursor>((({cursorResolver})", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("_defaultMouseCursor => WidgetStateProperty.resolveWith(((global::System.Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>, CupertinoSwitch>)", $"_defaultMouseCursor => WidgetStateProperty.resolveWith((({cursorResolver})", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("WidgetStateProperty.resolveWith<global::Doroti.Framework.Services.MouseCursor>(((global::System.Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>, T>)", $"WidgetStateProperty.resolveWith<global::Doroti.Framework.Services.MouseCursor>((({cursorResolver})", StringComparison.Ordinal);
            source = source
                .ReplaceGeneratedLocalPattern("return ((CupertinoButton)(object?)((!states.Contains", "return ((global::Doroti.Framework.Services.MouseCursor)(object?)((!states.Contains", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("return ((CupertinoCheckbox)(object?)((!states.Contains", "return ((global::Doroti.Framework.Services.MouseCursor)(object?)((!states.Contains", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("return ((CupertinoSwitch)(object?)global::Doroti.Framework.Services.MouseCursor.defer)", "return global::Doroti.Framework.Services.MouseCursor.defer", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("return ((CupertinoSwitch)(object?)(global::Doroti.Framework.Foundation.ConstantsLibrary.kIsWeb", "return ((global::Doroti.Framework.Services.MouseCursor)(object?)(global::Doroti.Framework.Foundation.ConstantsLibrary.kIsWeb", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("return ((T)(object?)((!states.Contains", "return ((global::Doroti.Framework.Services.MouseCursor)(object?)((!states.Contains", StringComparison.Ordinal);
        }

        if (library.EndsWith("/cupertino/checkbox.dart", StringComparison.Ordinal) || library.EndsWith("/cupertino/switch.dart", StringComparison.Ordinal))
        {
            source = source
                .ReplaceGeneratedLocalPattern("global::Doroti.Framework.Rendering.CustomPainter painter = default!", "dynamic painter = default!", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("new Semantics(", "new global::Doroti.Framework.Widgets.Semantics(", StringComparison.Ordinal);
        }

        if (library.EndsWith("/cupertino/colors.dart", StringComparison.Ordinal))
        {
            source = source.ReplaceGeneratedLocalPattern("=> this.darkHighContrastElevatedColor }))", "=> this.darkHighContrastElevatedColor, _ => this.color }))", StringComparison.Ordinal);
        }

        if (library.EndsWith("/cupertino/context_menu.dart", StringComparison.Ordinal))
        {
            source = source
                .ReplaceGeneratedLocalPattern("global::Doroti.Framework.Widgets.Visibility.Create(key:", "new global::Doroti.Framework.Widgets.Visibility(key:", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("global::Doroti.Framework.Widgets.OrientationBuilder.Create(builder:", "new global::Doroti.Framework.Widgets.OrientationBuilder(builder:", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("_ContextMenuSheet__context_menu.Create(key:", "new _ContextMenuSheet__context_menu(key:", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("global::Doroti.Framework.Widgets.Transform.Create(key:", "global::Doroti.Framework.Widgets.Transform.CreateScale(key:", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("global::Doroti.Framework.Widgets.TickerMode.Create(enabled:", "new global::Doroti.Framework.Widgets.TickerMode(enabled:", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("_ContextMenuRouteStatic__context_menu.Create(actions:", "new _ContextMenuRouteStatic__context_menu(actions:", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("global::Doroti.Framework.Widgets.MouseRegion.Create(cursor:", "new global::Doroti.Framework.Widgets.MouseRegion(cursor:", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("global::Doroti.Framework.Widgets.Listener.Create(onPointerDown:", "new global::Doroti.Framework.Widgets.Listener(onPointerDown:", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("global::Doroti.Framework.Widgets.AnimatedBuilder.Create(animation:", "new global::Doroti.Framework.Widgets.AnimatedBuilder(animation:", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("global::Doroti.Framework.Widgets.FadeTransition.Create(opacity:", "new global::Doroti.Framework.Widgets.FadeTransition(opacity:", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("global::Doroti.Framework.Widgets.Transform.Create(alignment:", "global::Doroti.Framework.Widgets.Transform.CreateScale(alignment:", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("global::Doroti.Framework.Widgets.Transform.Create(key:", "global::Doroti.Framework.Widgets.Transform.CreateScale(key:", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("global::Doroti.Framework.Widgets.Positioned.Create(rect:", "global::Doroti.Framework.Widgets.Positioned.CreateFromRect(rect:", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("global::Doroti.Framework.Widgets.Positioned.Create(key:", "global::Doroti.Framework.Widgets.Positioned.CreateFromRect(key:", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("_sheetScaleTween.end = this._scale;", "_sheetScaleTween.end = DartRuntimePrimitives.RequireValue(this._scale);", StringComparison.Ordinal);
        }

        if (library.EndsWith("/cupertino/cupertino_focus_halo.dart", StringComparison.Ordinal))
        {
            source = source
                .ReplaceGeneratedLocalPattern("(arg0) => new global::Doroti.Framework.Painting.RoundedRectangleBorder(arg0)", "(borderRadius, side) => new global::Doroti.Framework.Painting.RoundedRectangleBorder(side: side, borderRadius: borderRadius)", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("(arg0) => new global::Doroti.Framework.Painting.RoundedSuperellipseBorder(arg0)", "(borderRadius, side) => new global::Doroti.Framework.Painting.RoundedSuperellipseBorder(side: side, borderRadius: borderRadius)", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("this.widget._shapeBuilder(side: ", "this.widget._shapeBuilder(", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern(", borderRadius: ((CupertinoFocusHalo)this.widget)._borderRadius)", ", ((CupertinoFocusHalo)this.widget)._borderRadius)", StringComparison.Ordinal);
            source = source.ReplaceGeneratedLocalPattern("this.widget._shapeBuilder((this._childHasFocus", "this.widget._shapeBuilder(((CupertinoFocusHalo)this.widget)._borderRadius, (this._childHasFocus", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern(", ((CupertinoFocusHalo)this.widget)._borderRadius)", ")", StringComparison.Ordinal);
        }

        if (library.EndsWith("/cupertino/date_picker.dart", StringComparison.Ordinal))
        {
            source = source.ReplaceGeneratedLocalPattern("._lastDayInMonth(", "._lastDayInMonth(", StringComparison.Ordinal).ReplaceGeneratedLocalPattern(").day", ").Day", StringComparison.Ordinal);
        }

        if (library.EndsWith("/cupertino/expansion_tile.dart", StringComparison.Ordinal))
        {
            source = source.ReplaceGeneratedLocalPattern("onTap: this._onHeaderTap", "onTap: () => { this._onHeaderTap(); return null!; }", StringComparison.Ordinal);
        }

        if (library.EndsWith("/cupertino/list_section.dart", StringComparison.Ordinal))
        {
            source = source.ReplaceGeneratedLocalPattern("decoration: (((object?)this.decoration ?? (object?)new global::Doroti.Framework.Painting.ShapeDecoration", "decoration: DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.Decoration>((object?)this.decoration ?? (object?)new global::Doroti.Framework.Painting.ShapeDecoration", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern(")))), child: new global::Doroti.Framework.Widgets.Column", "))), child: new global::Doroti.Framework.Widgets.Column", StringComparison.Ordinal);
        }

        if (library.EndsWith("/cupertino/localizations.dart", StringComparison.Ordinal))
        {
            source = source.ReplaceGeneratedLocalPattern("DateTime.january", "1L", StringComparison.Ordinal);
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
    private static List<global::Doroti.Framework.Painting.TextStyle> styles(this _DynamicTypeStyle__menu_anchor value) =>
        (value == _DynamicTypeStyle__menu_anchor.body ? _bodySizes : _subheadSizes)
            .Select(size => new global::Doroti.Framework.Painting.TextStyle(fontSize: size)).ToList();
    private static double _interpolateUnits(double value, double min, double max) => (value - min) / (max - min);
""";
            source = source
                .ReplaceGeneratedLocalPattern("\ninternal enum _DynamicTypeStyle__menu_anchor", widthMembers + "\ninternal enum _DynamicTypeStyle__menu_anchor", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("internal static class _DynamicTypeStyle__menu_anchorMembers\n{", dynamicMembers, StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("value.styles", "value.styles()", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("_DynamicTypeStyle__menu_anchor._interpolateUnits", "_interpolateUnits", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("_CupertinoMenuWidth__menu_anchor.CreateFromScreenWidth", "_CupertinoMenuWidth__menu_anchorMembers.CreateFromScreenWidth", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern(").points", ").points()", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("[global::Doroti.Framework.Widgets.WidgetState.dragged]", "[global::Doroti.Framework.Widgets.WidgetState.dragged.asConstraint()]", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("[global::Doroti.Framework.Widgets.WidgetState.pressed]", "[global::Doroti.Framework.Widgets.WidgetState.pressed.asConstraint()]", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("[global::Doroti.Framework.Widgets.WidgetState.focused]", "[global::Doroti.Framework.Widgets.WidgetState.focused.asConstraint()]", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("[global::Doroti.Framework.Widgets.WidgetState.hovered]", "[global::Doroti.Framework.Widgets.WidgetState.hovered.asConstraint()]", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("[global::Doroti.Framework.Widgets.WidgetState.any]", "[global::Doroti.Framework.Widgets.WidgetStateMembers.any]", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("(global::System.Action<bool?>)this._handleFocusChange", "(global::System.Action<bool>)(value => this._handleFocusChange(value))", StringComparison.Ordinal);
        }

        if (library.EndsWith("/cupertino/nav_bar.dart", StringComparison.Ordinal))
        {
            source = source
                .ReplaceGeneratedLocalPattern("CupertinoSheetRoute.hasParentSheet", "CupertinoSheetRoute<object>.hasParentSheet", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("userBottom: ((CupertinoNavigationBar)this.widget).bottom", "userBottom: (global::Doroti.Framework.Widgets.Widget?)(object?)((CupertinoNavigationBar)this.widget).bottom", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern(": ((CupertinoSliverNavigationBar)this.widget).bottom))", ": (global::Doroti.Framework.Widgets.Widget?)(object?)((CupertinoSliverNavigationBar)this.widget).bottom))", StringComparison.Ordinal);
        }

        if (library.EndsWith("/cupertino/page_scaffold.dart", StringComparison.Ordinal))
        {
            source = source.ReplaceGeneratedLocalPattern("        base.handleStatusBarTap();\n", string.Empty, StringComparison.Ordinal);
        }

        if (library.EndsWith("/cupertino/refresh.dart", StringComparison.Ordinal))
        {
            source = source
                .ReplaceGeneratedLocalPattern("nextState__19800 = RefreshIndicatorMode.drag;\n                    }\n                    continue;", "nextState__19800 = RefreshIndicatorMode.drag;\n                    }\n                    goto case RefreshIndicatorMode.drag;", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("goToDone();\n                        continue;", "goToDone();\n                        goto case RefreshIndicatorMode.done;", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("nextState__19800 = RefreshIndicatorMode.refresh;\n                    }\n                    continue;", "nextState__19800 = RefreshIndicatorMode.refresh;\n                    }\n                    goto case RefreshIndicatorMode.refresh;", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("goToDone();\n                    }\n                    continue;", "goToDone();\n                    }\n                    goto case RefreshIndicatorMode.done;", StringComparison.Ordinal);
        }

        if (library.EndsWith("/cupertino/route.dart", StringComparison.Ordinal))
        {
            source = source
                .ReplaceGeneratedLocalPattern("CupertinoRouteTransitionMixin.kTransitionDuration", "CupertinoRouteTransitionMixin<object>.kTransitionDuration", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("Page._defaultPopInvokedHandler", "((didPop, result) => Page<object>._defaultPopInvokedHandler(didPop, result))", StringComparison.Ordinal);
        }

        if (library.EndsWith("/cupertino/segmented_control.dart", StringComparison.Ordinal) || library.EndsWith("/cupertino/sliding_segmented_control.dart", StringComparison.Ordinal))
        {
            source = source.ReplaceGeneratedLocalPattern("disabledChildren ?? new DartMap<object, object>()", "disabledChildren ?? new HashSet<T>()", StringComparison.Ordinal);
        }

        if (library.EndsWith("/cupertino/sliding_segmented_control.dart", StringComparison.Ordinal))
        {
            source = source.ReplaceGeneratedLocalPattern("global::Doroti.Framework.Painting.BorderRadius.CreateAll(DialogLibrary._kCornerRadius)", "global::Doroti.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Ui.Radius.circular(DialogLibrary._kCornerRadius))", StringComparison.Ordinal);
        }

        if (library.EndsWith("/cupertino/sheet.dart", StringComparison.Ordinal))
        {
            source = source.ReplaceGeneratedLocalPattern("NavigatorPopHandler<object>(onPopWithResult: ((global::System.Action<T?>)", "NavigatorPopHandler<T>(onPopWithResult: ((global::System.Action<T?>)", StringComparison.Ordinal);
        }

        if (library.EndsWith("/cupertino/spell_check_suggestions_toolbar.dart", StringComparison.Ordinal))
        {
            source = source.ReplaceGeneratedLocalPattern("new CupertinoTextSelectionToolbarButton(buttonItem:", "CupertinoTextSelectionToolbarButton.CreateButtonItem(buttonItem:", StringComparison.Ordinal);
        }

        if (library.EndsWith("/cupertino/text_field.dart", StringComparison.Ordinal))
        {
            source = source
                .ReplaceGeneratedLocalPattern("new CupertinoSpellCheckSuggestionsToolbar(editableTextState:", "CupertinoSpellCheckSuggestionsToolbar.CreateEditableText(editableTextState:", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("(this._controller?.value.text.Length != 0) ?? false", "this._controller is not null && this._controller.value.text.Length != 0", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("global::Doroti.Framework.Widgets.Semantics.Create(button:", "new global::Doroti.Framework.Widgets.Semantics(button:", StringComparison.Ordinal);
            source = source.ReplaceGeneratedLocalPattern("global::Doroti.Framework.Widgets.GestureDetector.Create(key:", "new global::Doroti.Framework.Widgets.GestureDetector(key:", StringComparison.Ordinal);
        }

        if (library.EndsWith("/cupertino/radio.dart", StringComparison.Ordinal))
        {
            source = source.ReplaceGeneratedLocalPattern("return ((T)(object?)(WidgetStateProperty.resolveAs<global::Doroti.Framework.Services.MouseCursor?>", "return ((global::Doroti.Framework.Services.MouseCursor)(object?)(WidgetStateProperty.resolveAs<global::Doroti.Framework.Services.MouseCursor?>", StringComparison.Ordinal);
        }

        if (library.EndsWith("/cupertino/checkbox.dart", StringComparison.Ordinal))
        {
            source = source.ReplaceGeneratedLocalPattern("return ((CupertinoCheckbox)(object?)(WidgetStateProperty.resolveAs<global::Doroti.Framework.Services.MouseCursor?>", "return ((global::Doroti.Framework.Services.MouseCursor)(object?)(WidgetStateProperty.resolveAs<global::Doroti.Framework.Services.MouseCursor?>", StringComparison.Ordinal);
        }

        if (library.EndsWith("/cupertino/switch.dart", StringComparison.Ordinal))
        {
            source = source
                .ReplaceGeneratedLocalPattern("((CupertinoSwitch)this.widget).applyTheme ?? theme__22457.applyThemeToAll)) ?", "((CupertinoSwitch)this.widget).applyTheme ?? theme__22457.applyThemeToAll) == true) ?", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern(".createBoxPainter(() => this._handleDecorationChanged())", ".createBoxPainter((global::System.Action)(() => this._handleDecorationChanged()))", StringComparison.Ordinal);
        }

        if (library.EndsWith("/cupertino/text_selection_toolbar.dart", StringComparison.Ordinal))
        {
            source = source
                .ReplaceGeneratedLocalPattern("((global::Doroti.Ui.Paint)(object?)_debugPaint ??=", "(_debugPaint ??=", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("public override global::Doroti.Framework.Rendering.RenderObject renderObject => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.RenderObject>", "public override _RenderCupertinoTextSelectionToolbarItems__text_selection_toolbar renderObject => DartRuntimePrimitives.ConvertValue<_RenderCupertinoTextSelectionToolbarItems__text_selection_toolbar>", StringComparison.Ordinal);
        }
        if (library.EndsWith("/cupertino/colors.dart", StringComparison.Ordinal))
        {
            source = source
                .ReplaceGeneratedLocalPattern("ToString(\"color\", this.color)", "toString(\"color\", this.color)", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("? resolvable.resolveFrom(context) : resolvable", "? ((CupertinoDynamicColor)resolvable).resolveFrom(context) : resolvable", StringComparison.Ordinal);
        }

        if (library.EndsWith("/cupertino/theme.dart", StringComparison.Ordinal))
        {
            source = source.ReplaceGeneratedLocalPattern(
                ") : this(brightness, primaryColor, primaryContrastingColor, textTheme, barBackgroundColor, scaffoldBackgroundColor, selectionHandleColor, applyThemeToAll)\n",
                ") : this(brightness, primaryColor, primaryContrastingColor, textTheme, barBackgroundColor, scaffoldBackgroundColor, selectionHandleColor, applyThemeToAll, ThemeLibrary._kDefaultTheme)\n",
                StringComparison.Ordinal);
        }

        if (library.EndsWith("/cupertino/text_selection.dart", StringComparison.Ordinal))
        {
            // TextSelectionHandleControls is emitted as a CLR marker interface,
            // allowing the concrete Cupertino superclass and Dart mixin marker
            // to coexist without multiple class inheritance.
            source = source
                .ReplaceGeneratedLocalPattern("public override void handleCut(", "public virtual void handleCut(", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("public override void handleCopy(", "public virtual void handleCopy(", StringComparison.Ordinal);
        }

        if (library.EndsWith("/cupertino/desktop_text_selection.dart", StringComparison.Ordinal))
        {
            source = source
                .ReplaceGeneratedLocalPattern("public override void handleCut(", "public virtual void handleCut(", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("public override void handleCopy(", "public virtual void handleCopy(", StringComparison.Ordinal);
        }

        if (library.EndsWith("/cupertino/nav_bar.dart", StringComparison.Ordinal))
        {
            source = source
                .ReplaceGeneratedLocalPattern(
                    "public override global::Doroti.Framework.Rendering.OverScrollHeaderStretchConfiguration? stretchConfiguration { get; set; } = default;",
                    "private global::Doroti.Framework.Rendering.OverScrollHeaderStretchConfiguration? __field_stretchConfiguration = default;\n    public override global::Doroti.Framework.Rendering.OverScrollHeaderStretchConfiguration? stretchConfiguration => __field_stretchConfiguration;",
                    StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("this.stretchConfiguration = stretchConfiguration;", "this.__field_stretchConfiguration = stretchConfiguration;", StringComparison.Ordinal);
        }

        if (library.EndsWith("/cupertino/text_field.dart", StringComparison.Ordinal))
        {
            source = source.ReplaceGeneratedLocalPattern(
                "internal enum _BaselineAlignedStackSlot__text_field",
                "public enum _BaselineAlignedStackSlot__text_field",
                StringComparison.Ordinal);
        }

        if (library.EndsWith("/cupertino/text_form_field_row.dart", StringComparison.Ordinal))
        {
            // Dart permits a covariant parameter on the FormFieldState override.
            // Preserve the CLR base signature and keep the body casts explicit.
            source = source.ReplaceGeneratedLocalPattern(
                "public override void didUpdateWidget(CupertinoTextFormFieldRow oldWidget)",
                "public override void didUpdateWidget(global::Doroti.Framework.Widgets.FormField<string> oldWidget)",
                StringComparison.Ordinal);
        }

        if (library.Contains("/material/", StringComparison.Ordinal))
        {
            source = source
                .ReplaceGeneratedLocalPattern("DateTime.january", "1L", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("DateTime.february", "2L", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("DateTime.monday", "1L", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("DateTime.daysPerWeek", "7L", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("global::Doroti.Framework.Widgets.WidgetState.any", "global::Doroti.Framework.Widgets.WidgetStateMembers.any", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("MaterialStatePropertyAll<", "global::Doroti.Framework.Widgets.WidgetStatePropertyAll<", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("MaterialStateProperty<", "global::Doroti.Framework.Widgets.WidgetStateProperty<", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("MaterialStatesController", "global::Doroti.Framework.Widgets.WidgetStatesController", StringComparison.Ordinal);

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
                        .ReplaceGeneratedLocalPattern("((T)(object?)", "(", StringComparison.Ordinal)
                        .ReplaceGeneratedLocalPattern("((T)(object)", "(", StringComparison.Ordinal);
                    return $"WidgetStateProperty.resolveWith((states) => {{{body}}})";
                });
            source = Regex.Replace(
                source,
                @"WidgetStateProperty\.resolveWith<(?<result>[^>\r\n]+)>\(\(\(global::System\.Func<HashSet<global::Doroti\.Generated\.Framework\.Widgets\.WidgetState>, (?<erased>[^>\r\n]+)>\)\(\(states\) => \{(?<body>[\s\S]*?)\}\)\)\)",
                match =>
                {
                    var erased = match.Groups["erased"].Value;
                    var body = match.Groups["body"].Value
                        .ReplaceGeneratedLocalPattern($"(({erased})(object?)", "(", StringComparison.Ordinal)
                        .ReplaceGeneratedLocalPattern($"(({erased}?)(object?)", "(", StringComparison.Ordinal);
                    return $"WidgetStateProperty.resolveWith<{match.Groups["result"].Value}>((states) => {{{body}}})";
                });

            source = source
                .ReplaceGeneratedLocalPattern("new ThemeData()", "ThemeData.Create()", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("List<global::Doroti.Framework.Widgets.GlobalKey<IState>>.Create(", "DartRuntimePrimitives.CreateList<global::Doroti.Framework.Widgets.GlobalKey<IState>>(", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("[global::Doroti.Framework.Widgets.WidgetState.disabled] = disabled", "[global::Doroti.Framework.Widgets.WidgetState.disabled.asConstraint()] = disabled", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("[global::Doroti.Framework.Widgets.WidgetState.selected] = selected", "[global::Doroti.Framework.Widgets.WidgetState.selected.asConstraint()] = selected", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("[global::Doroti.Framework.Widgets.WidgetState.dragged] =", "[global::Doroti.Framework.Widgets.WidgetState.dragged.asConstraint()] =", StringComparison.Ordinal);
        }

        if (library.Contains("/material/animated_icons/data/", StringComparison.Ordinal) &&
            library.EndsWith(".g.dart", StringComparison.Ordinal))
        {
            source = Regex.Replace(source, @"public static partial class [A-Za-z0-9_]+Library", "public static partial class Animated_iconsLibrary");
        }

        if (library.EndsWith("/material/arc.dart", StringComparison.Ordinal))
        {
            source = source.ReplaceGeneratedLocalPattern("public override Offset? begin", "public override Offset begin", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("public override Offset? end", "public override Offset end", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern(": base(begin: begin, end: end)", ": base(begin: DartRuntimePrimitives.RequireValue(begin), end: DartRuntimePrimitives.RequireValue(end))", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("ArcLibrary._allDiagonals.Cast<T>()", "ArcLibrary._allDiagonals.Cast<_Diagonal__arc>()", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("DartRuntimePrimitives.Assert(() => (this.begin is not null));", string.Empty, StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("DartRuntimePrimitives.Assert(() => (this.end is not null));", string.Empty, StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("if (((this.begin is null) || (this.end is null)))", "if (false)", StringComparison.Ordinal);
        }

        if (library.EndsWith("/material/animated_icons/animated_icons.dart", StringComparison.Ordinal))
        {
            source = source
                .ReplaceGeneratedLocalPattern("_interpolate<double?>(opacities, progress, Dart_uiLibrary.lerpDouble)", "_interpolate<double>(opacities, progress, (a, b, t) => Dart_uiLibrary.lerpDouble(a, b, t) ?? 0.0)", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("_interpolate<global::Doroti.Ui.Offset?>(points, progress, Offset.lerp)", "_interpolate<global::Doroti.Ui.Offset>(points, progress, (a, b, t) => Offset.lerp(a, b, t)!.Value)", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("_interpolate<global::Doroti.Ui.Offset?>(controlPoints1, progress, Offset.lerp)", "_interpolate<global::Doroti.Ui.Offset>(controlPoints1, progress, (a, b, t) => Offset.lerp(a, b, t)!.Value)", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("_interpolate<global::Doroti.Ui.Offset?>(controlPoints2, progress, Offset.lerp)", "_interpolate<global::Doroti.Ui.Offset>(controlPoints2, progress, (a, b, t) => Offset.lerp(a, b, t)!.Value)", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("_interpolate<global::Doroti.Ui.Offset?>(targetPoints, progress, Offset.lerp)", "_interpolate<global::Doroti.Ui.Offset>(targetPoints, progress, (a, b, t) => Offset.lerp(a, b, t)!.Value)", StringComparison.Ordinal);
        }

        if (library.EndsWith("/material/button_style_button.dart", StringComparison.Ordinal))
        {
            source = source
                .ReplaceGeneratedLocalPattern("public abstract ButtonStyle defaultStyleOf(global::Doroti.Framework.Widgets.BuildContext context);", "public virtual ButtonStyle defaultStyleOf(global::Doroti.Framework.Widgets.BuildContext context) => default!;", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("public abstract ButtonStyle? themeStyleOf(global::Doroti.Framework.Widgets.BuildContext context);", "public virtual ButtonStyle? themeStyleOf(global::Doroti.Framework.Widgets.BuildContext context) => default;", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("ButtonStyleButton? effectiveValue(global::System.Func<ButtonStyle?, ButtonStyleButton?> getProperty)", "P? effectiveValue<P>(global::System.Func<ButtonStyle?, P?> getProperty)", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("ButtonStyleButton? widgetValue", "P? widgetValue", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("ButtonStyleButton? themeValue", "P? themeValue", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("ButtonStyleButton? defaultValue", "P? defaultValue", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("ButtonStyleButton? resolve(global::System.Func<ButtonStyle?, global::Doroti.Framework.Widgets.WidgetStateProperty<ButtonStyleButton>?> getProperty)", "P? resolve<P>(global::System.Func<ButtonStyle?, global::Doroti.Framework.Widgets.WidgetStateProperty<P>?> getProperty)", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("return ((ButtonStyleButton?)(object?)getProperty(style)?.resolve(this.statesController.value));", "return getProperty(style) is { } property ? property.resolve(this.statesController.value) : default;", StringComparison.Ordinal);
            source = source.ReplaceGeneratedLocalPattern("global::System.Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>, ButtonStyleButton>", "global::System.Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>, global::Doroti.Ui.Color?>", StringComparison.Ordinal);
        }

        if (library.EndsWith("/material/elevated_button.dart", StringComparison.Ordinal))
        {
            source = source
                .ReplaceGeneratedLocalPattern("[global::Doroti.Framework.Widgets.WidgetState.pressed] = (DartRuntimePrimitives.RequireValue(elevation", "[global::Doroti.Framework.Widgets.WidgetState.pressed.asConstraint()] = (DartRuntimePrimitives.RequireValue(elevation", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("[global::Doroti.Framework.Widgets.WidgetState.hovered] = (DartRuntimePrimitives.RequireValue(elevation", "[global::Doroti.Framework.Widgets.WidgetState.hovered.asConstraint()] = (DartRuntimePrimitives.RequireValue(elevation", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("[global::Doroti.Framework.Widgets.WidgetState.focused] = (DartRuntimePrimitives.RequireValue(elevation", "[global::Doroti.Framework.Widgets.WidgetState.focused.asConstraint()] = (DartRuntimePrimitives.RequireValue(elevation", StringComparison.Ordinal);
        }

        if (library.EndsWith("/material/calendar_date_picker.dart", StringComparison.Ordinal))
        {
            source = source
                .ReplaceGeneratedLocalPattern("_Day__calendar_date_picker? effectiveValue(global::System.Func<DatePickerThemeData?, _Day__calendar_date_picker?> getProperty)", "P? effectiveValue<P>(global::System.Func<DatePickerThemeData?, P?> getProperty)", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("_Day__calendar_date_picker? resolve(global::System.Func<DatePickerThemeData?, global::Doroti.Framework.Widgets.WidgetStateProperty<_Day__calendar_date_picker>?> getProperty", "P? resolve<P>(global::System.Func<DatePickerThemeData?, global::Doroti.Framework.Widgets.WidgetStateProperty<P>?> getProperty", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("return ((_Day__calendar_date_picker?)(object?)getProperty(theme)?.resolve(states));", "return getProperty(theme) is { } property ? property.resolve(states) : default;", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("YearPicker? effectiveValue(global::System.Func<DatePickerThemeData?, YearPicker?> getProperty)", "P? effectiveValue<P>(global::System.Func<DatePickerThemeData?, P?> getProperty)", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("YearPicker? resolve(global::System.Func<DatePickerThemeData?, global::Doroti.Framework.Widgets.WidgetStateProperty<YearPicker>?> getProperty", "P? resolve<P>(global::System.Func<DatePickerThemeData?, global::Doroti.Framework.Widgets.WidgetStateProperty<P>?> getProperty", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("return ((YearPicker?)(object?)getProperty(theme)?.resolve(states));", "return getProperty(theme) is { } property ? property.resolve(states) : default;", StringComparison.Ordinal);
            source = source
                .ReplaceGeneratedLocalPattern("global::System.Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>, _Day__calendar_date_picker>", "global::System.Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>, global::Doroti.Ui.Color?>", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("global::System.Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>, YearPicker>", "global::System.Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>, global::Doroti.Ui.Color?>", StringComparison.Ordinal);
        }

        if (library.EndsWith("/material/date_picker.dart", StringComparison.Ordinal))
        {
            source = source
                .ReplaceGeneratedLocalPattern("_DayItem__date_picker? effectiveValue(global::System.Func<DatePickerThemeData?, _DayItem__date_picker?> getProperty)", "P? effectiveValue<P>(global::System.Func<DatePickerThemeData?, P?> getProperty)", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("_DayItem__date_picker? resolve(global::System.Func<DatePickerThemeData?, global::Doroti.Framework.Widgets.WidgetStateProperty<_DayItem__date_picker>?> getProperty", "P? resolve<P>(global::System.Func<DatePickerThemeData?, global::Doroti.Framework.Widgets.WidgetStateProperty<P>?> getProperty", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("return ((_DayItem__date_picker?)(object?)getProperty(theme)?.resolve(states));", "return getProperty(theme) is { } property ? property.resolve(states) : default;", StringComparison.Ordinal);
            source = source.ReplaceGeneratedLocalPattern("global::System.Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>, _DayItem__date_picker>", "global::System.Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>, global::Doroti.Ui.Color?>", StringComparison.Ordinal);
            source = source.ReplaceGeneratedLocalPattern("this.value.index", "FoundationRuntimePorts.EnumIndex(this.value)", StringComparison.Ordinal);
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
            source = source.ReplaceGeneratedLocalPattern("where T : DateTime", "where T : struct", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("return (((dateA?.year == dateB?.year) && (dateA?.month == dateB?.month)) && (dateA?.day == dateB?.day));", "return dateA.HasValue && dateB.HasValue && ((dynamic)dateA.Value).Year == ((dynamic)dateB.Value).Year && ((dynamic)dateA.Value).Month == ((dynamic)dateB.Value).Month && ((dynamic)dateA.Value).Day == ((dynamic)dateB.Value).Day;", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("return ((dateA?.year == dateB?.year) && (dateA?.month == dateB?.month));", "return dateA.HasValue && dateB.HasValue && ((dynamic)dateA.Value).Year == ((dynamic)dateB.Value).Year && ((dynamic)dateA.Value).Month == ((dynamic)dateB.Value).Month;", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("System.Diagnostics.Debug.Assert(!start.isAfter(end));", "System.Diagnostics.Debug.Assert(Comparer<T>.Default.Compare(start, end) <= 0);", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("public virtual Duration duration => this.end.difference(this.start);", "public virtual Duration duration => (Duration)(((dynamic)this.end) - ((dynamic)this.start));", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("object.Equals(((DateTimeRange<DateTime>)((DateTimeRange<DateTime>)__other)).start, this.start)", "object.Equals(((DateTimeRange<T>)__other).start, this.start)", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("object.Equals(((DateTimeRange<DateTime>)((DateTimeRange<DateTime>)__other)).end, this.end)", "object.Equals(((DateTimeRange<T>)__other).end, this.end)", StringComparison.Ordinal);
        }

        if (library.EndsWith("/material/desktop_text_selection.dart", StringComparison.Ordinal) || library.EndsWith("/material/text_selection.dart", StringComparison.Ordinal))
        {
            source = source
                .ReplaceGeneratedLocalPattern("public override void handleCut(", "public virtual void handleCut(", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("public override void handleCopy(", "public virtual void handleCopy(", StringComparison.Ordinal);
        }

        if (library.EndsWith("/material/dropdown.dart", StringComparison.Ordinal))
        {
            source = source
                .ReplaceGeneratedLocalPattern("internal class _DropdownRouteResult__dropdown", "public class _DropdownRouteResult__dropdown", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("public override void didUpdateWidget(T oldWidget)", "public override void didUpdateWidget(global::Doroti.Framework.Widgets.FormField<T> oldWidget)", StringComparison.Ordinal);
        }

        if (library.EndsWith("/material/dropdown_menu_form_field.dart", StringComparison.Ordinal))
        {
            source = source.ReplaceGeneratedLocalPattern("public override void didUpdateWidget(T oldWidget)", "public override void didUpdateWidget(global::Doroti.Framework.Widgets.FormField<T> oldWidget)", StringComparison.Ordinal);
        }

        if (library.EndsWith("/material/text_form_field.dart", StringComparison.Ordinal))
        {
            source = source.ReplaceGeneratedLocalPattern("public override void didUpdateWidget(TextFormField oldWidget)", "public override void didUpdateWidget(global::Doroti.Framework.Widgets.FormField<string> oldWidget)", StringComparison.Ordinal);
        }

        if (library.EndsWith("/material/icons.dart", StringComparison.Ordinal))
        {
            source = source.ReplaceGeneratedLocalPattern("public override global::Doroti.Framework.Widgets.IconData", "public virtual global::Doroti.Framework.Widgets.IconData", StringComparison.Ordinal);
        }

        if (library.EndsWith("/material/ink_sparkle.dart", StringComparison.Ordinal))
        {
            source = source.ReplaceGeneratedLocalPattern("Vector2", "global::System.Numerics.Vector2", StringComparison.Ordinal);
        }

        if (library.EndsWith("/material/input_border.dart", StringComparison.Ordinal))
        {
            source = source.ReplaceGeneratedLocalPattern("public abstract override void paint(", "public abstract void paint(", StringComparison.Ordinal);
        }

        if (library.EndsWith("/material/material.dart", StringComparison.Ordinal))
        {
            source = source.ReplaceGeneratedLocalPattern("public abstract void paintFeature(Canvas canvas, Matrix4 transform);", "public virtual void paintFeature(Canvas canvas, Matrix4 transform) { }", StringComparison.Ordinal);
        }

        if (library.EndsWith("/material/no_splash.dart", StringComparison.Ordinal))
        {
            source = source.ReplaceGeneratedLocalPattern("public virtual void paintFeature(Canvas canvas, Matrix4 transform)", "public override void paintFeature(Canvas canvas, Matrix4 transform)", StringComparison.Ordinal);
        }

        if (library.EndsWith("/material/page_transitions_theme.dart", StringComparison.Ordinal))
        {
            source = source.ReplaceGeneratedLocalPattern("global::Doroti.Framework.Widgets.Image image", "global::Doroti.Ui.Image image", StringComparison.Ordinal);
        }

        if (library.EndsWith("/material/scaffold.dart", StringComparison.Ordinal))
        {
            source = source.ReplaceGeneratedLocalPattern("public virtual global::Doroti.Ui.Offset getOffset(ScaffoldPrelayoutGeometry scaffoldGeometry)", "public override global::Doroti.Ui.Offset getOffset(ScaffoldPrelayoutGeometry scaffoldGeometry)", StringComparison.Ordinal);
        }

        if (library.EndsWith("/material/slider_parts.dart", StringComparison.Ordinal))
        {
            source = source.ReplaceGeneratedLocalPattern("TextDirection textDirection = false", "TextDirection textDirection = default!", StringComparison.Ordinal);
            const string sliderOverload = "\n    public global::Doroti.Ui.Size getPreferredSize(bool isEnabled, bool isDiscrete, global::Doroti.Framework.Painting.TextPainter? labelPainter = null, double? textScaleFactor = null) => getPreferredSize(isEnabled, isDiscrete);\n";
            source = source.ReplaceGeneratedLocalPattern("public class RoundSliderThumbShape : SliderComponentShape\n{", "public class RoundSliderThumbShape : SliderComponentShape\n{" + sliderOverload, StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("public class HandleThumbShape : SliderComponentShape\n{", "public class HandleThumbShape : SliderComponentShape\n{" + sliderOverload, StringComparison.Ordinal);
        }

        if (library.EndsWith("/material/range_slider_parts.dart", StringComparison.Ordinal))
        {
            // The source declarations already carry the concrete non-nullable
            // virtual contract used by the built-in range indicator shapes.
        }

        if (library.EndsWith("/material/slider_value_indicator_shape.dart", StringComparison.Ordinal))
        {
            source = source
                .ReplaceGeneratedLocalPattern("public virtual global::Doroti.Ui.Size getPreferredSize(bool isEnabled, bool isDiscrete, global::Doroti.Framework.Painting.TextPainter labelPainter, double textScaleFactor)", "public override global::Doroti.Ui.Size getPreferredSize(bool isEnabled, bool isDiscrete, global::Doroti.Framework.Painting.TextPainter labelPainter, double textScaleFactor)", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("public override global::Doroti.Ui.Size getPreferredSize(bool isEnabled, bool isDiscrete, global::Doroti.Framework.Painting.TextPainter? labelPainter = null, double? textScaleFactor = null)", "public override global::Doroti.Ui.Size getPreferredSize(bool isEnabled, bool isDiscrete, global::Doroti.Framework.Painting.TextPainter labelPainter, double textScaleFactor)", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("public virtual void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset center, global::Doroti.Framework.Animation.Animation<double>? activationAnimation = null, global::Doroti.Framework.Animation.Animation<double>? enableAnimation = null, bool? isDiscrete = null, bool? isOnTop = null, global::Doroti.Framework.Painting.TextPainter? labelPainter = null, double? textScaleFactor = null, Size? sizeWithOverflow = null, global::Doroti.Framework.Rendering.RenderBox? parentBox = null, SliderThemeData? sliderTheme = null, TextDirection? textDirection = null, double? value = null, Thumb? thumb = null)", "public override void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset center, global::Doroti.Framework.Animation.Animation<double> activationAnimation, global::Doroti.Framework.Animation.Animation<double> enableAnimation, bool isDiscrete = default!, bool isOnTop = default!, global::Doroti.Framework.Painting.TextPainter labelPainter = default!, double textScaleFactor = default!, Size sizeWithOverflow = default!, global::Doroti.Framework.Rendering.RenderBox parentBox = default!, SliderThemeData sliderTheme = default!, TextDirection textDirection = default!, double value = default!, Thumb thumb = default!)", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("public override void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset center, global::Doroti.Framework.Animation.Animation<double>? activationAnimation = null, global::Doroti.Framework.Animation.Animation<double>? enableAnimation = null, bool? isDiscrete = null, bool? isOnTop = null, global::Doroti.Framework.Painting.TextPainter? labelPainter = null, double? textScaleFactor = null, Size? sizeWithOverflow = null, global::Doroti.Framework.Rendering.RenderBox? parentBox = null, SliderThemeData? sliderTheme = null, TextDirection? textDirection = null, double? value = null, Thumb? thumb = null)", "public override void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset center, global::Doroti.Framework.Animation.Animation<double> activationAnimation, global::Doroti.Framework.Animation.Animation<double> enableAnimation, bool isDiscrete = default!, bool isOnTop = default!, global::Doroti.Framework.Painting.TextPainter labelPainter = default!, double textScaleFactor = default!, Size sizeWithOverflow = default!, global::Doroti.Framework.Rendering.RenderBox parentBox = default!, SliderThemeData sliderTheme = default!, TextDirection textDirection = default!, double value = default!, Thumb thumb = default!)", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("public virtual void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset center, global::Doroti.Framework.Animation.Animation<double> activationAnimation, global::Doroti.Framework.Animation.Animation<double> enableAnimation, bool? isDiscrete = null, bool isOnTop = false, global::Doroti.Framework.Painting.TextPainter labelPainter = default!, global::Doroti.Framework.Rendering.RenderBox parentBox = default!, SliderThemeData sliderTheme = default!, TextDirection? textDirection = null, Thumb? thumb = null, double? value = null, double? textScaleFactor = null, Size? sizeWithOverflow = null)", "public override void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset center, global::Doroti.Framework.Animation.Animation<double> activationAnimation, global::Doroti.Framework.Animation.Animation<double> enableAnimation, bool isDiscrete = default!, bool isOnTop = default!, global::Doroti.Framework.Painting.TextPainter labelPainter = default!, double textScaleFactor = default!, Size sizeWithOverflow = default!, global::Doroti.Framework.Rendering.RenderBox parentBox = default!, SliderThemeData sliderTheme = default!, TextDirection textDirection = default!, double value = default!, Thumb thumb = default!)", StringComparison.Ordinal);
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
            source = source.ReplaceGeneratedLocalPattern("namespace Doroti.Framework.Material;", "namespace Doroti.Framework.Material;" + materialColorUtilities, StringComparison.Ordinal);
        }

        if (library.EndsWith("/material/material_state.dart", StringComparison.Ordinal))
        {
            source = source
                .ReplaceGeneratedLocalPattern("public virtual T resolve(HashSet<global::Doroti.Framework.Widgets.WidgetState> states)", "public virtual InputBorder resolve(HashSet<global::Doroti.Framework.Widgets.WidgetState> states)", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("public delegate void global::Doroti.Framework.Widgets.WidgetStateProperty<T>();", string.Empty, StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("public delegate void global::Doroti.Framework.Widgets.WidgetStatePropertyAll<T>();", string.Empty, StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("public delegate void global::Doroti.Framework.Widgets.WidgetStatesController();", string.Empty, StringComparison.Ordinal);
        }

        if (library.EndsWith("/material/segmented_button.dart", StringComparison.Ordinal))
        {
            source = source
                .ReplaceGeneratedLocalPattern("P? effectiveValue(global::System.Func<ButtonStyle?, P?> getProperty)", "P? effectiveValue<P>(global::System.Func<ButtonStyle?, P?> getProperty)", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("P? resolve(global::System.Func<ButtonStyle?, global::Doroti.Framework.Widgets.WidgetStateProperty<P>?> getProperty", "P? resolve<P>(global::System.Func<ButtonStyle?, global::Doroti.Framework.Widgets.WidgetStateProperty<P>?> getProperty", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("[(global::Doroti.Framework.Widgets.WidgetState.selected & global::Doroti.Framework.Widgets.WidgetState.pressed)]", "[global::Doroti.Framework.Widgets.WidgetState.selected.asConstraint().op_BitwiseAnd(global::Doroti.Framework.Widgets.WidgetState.pressed.asConstraint())]", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("[(global::Doroti.Framework.Widgets.WidgetState.selected & global::Doroti.Framework.Widgets.WidgetState.hovered)]", "[global::Doroti.Framework.Widgets.WidgetState.selected.asConstraint().op_BitwiseAnd(global::Doroti.Framework.Widgets.WidgetState.hovered.asConstraint())]", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("[(global::Doroti.Framework.Widgets.WidgetState.selected & global::Doroti.Framework.Widgets.WidgetState.focused)]", "[global::Doroti.Framework.Widgets.WidgetState.selected.asConstraint().op_BitwiseAnd(global::Doroti.Framework.Widgets.WidgetState.focused.asConstraint())]", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("[global::Doroti.Framework.Widgets.WidgetState.pressed] = unselected", "[global::Doroti.Framework.Widgets.WidgetState.pressed.asConstraint()] = unselected", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("[global::Doroti.Framework.Widgets.WidgetState.hovered] = unselected", "[global::Doroti.Framework.Widgets.WidgetState.hovered.asConstraint()] = unselected", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("[global::Doroti.Framework.Widgets.WidgetState.focused] = unselected", "[global::Doroti.Framework.Widgets.WidgetState.focused.asConstraint()] = unselected", StringComparison.Ordinal);
        }

        if (library.EndsWith("/material/menu_anchor.dart", StringComparison.Ordinal))
        {
            source = source.ReplaceGeneratedLocalPattern("public override void invoke(global::Doroti.Framework.Widgets.DirectionalFocusIntent", "public override object? invoke(global::Doroti.Framework.Widgets.DirectionalFocusIntent", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("        base.invoke(intent, context);\n    }", "        return base.invoke(intent, context);\n    }", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("_MenuPanel__menu_anchor? effectiveValue(global::System.Func<MenuStyle?, _MenuPanel__menu_anchor?> getProperty)", "P? effectiveValue<P>(global::System.Func<MenuStyle?, P?> getProperty)", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("_MenuPanel__menu_anchor? resolve(global::System.Func<MenuStyle?, global::Doroti.Framework.Widgets.WidgetStateProperty<_MenuPanel__menu_anchor>?> getProperty)", "P? resolve<P>(global::System.Func<MenuStyle?, global::Doroti.Framework.Widgets.WidgetStateProperty<P>?> getProperty)", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("return ((_MenuPanel__menu_anchor?)(object?)getProperty(style)?.resolve(new HashSet<global::Doroti.Framework.Widgets.WidgetState>()));", "return getProperty(style) is { } property ? property.resolve(new HashSet<global::Doroti.Framework.Widgets.WidgetState>()) : default;", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("T? effectiveValue(global::System.Func<MenuStyle?, T?> getProperty)", "T? effectiveValue<T>(global::System.Func<MenuStyle?, T?> getProperty)", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("T? resolve(global::System.Func<MenuStyle?, global::Doroti.Framework.Widgets.WidgetStateProperty<T>?> getProperty)", "T? resolve<T>(global::System.Func<MenuStyle?, global::Doroti.Framework.Widgets.WidgetStateProperty<T>?> getProperty)", StringComparison.Ordinal);
            source = Regex.Replace(
                source,
                @"(internal class _SubmenuDirectionalFocusAction__menu_anchor[\s\S]*?)(?=\ninternal class _LocalizedShortcutLabeler__menu_anchor)",
                match => match.Groups[1].Value.ReplaceGeneratedLocalPattern("return;", "return null;", StringComparison.Ordinal).ReplaceGeneratedLocalPattern("        Actions.maybeInvoke(this.submenu.context, intent);\n", "        return Actions.maybeInvoke(this.submenu.context, intent);\n", StringComparison.Ordinal));
        }

        if (library.EndsWith("/material/search_anchor.dart", StringComparison.Ordinal))
        {
            source = source
                .ReplaceGeneratedLocalPattern("SearchBar? resolve(global::Doroti.Framework.Widgets.WidgetStateProperty<SearchBar>? widgetValue, global::Doroti.Framework.Widgets.WidgetStateProperty<SearchBar>? themeValue, global::Doroti.Framework.Widgets.WidgetStateProperty<SearchBar>? defaultValue)", "P? resolve<P>(global::Doroti.Framework.Widgets.WidgetStateProperty<P>? widgetValue, global::Doroti.Framework.Widgets.WidgetStateProperty<P>? themeValue, global::Doroti.Framework.Widgets.WidgetStateProperty<P>? defaultValue)", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("return ((((widgetValue?.resolve(states__60702) ?? (SearchBar)themeValue?.resolve(states__60702))) ?? (SearchBar)defaultValue?.resolve(states__60702)));", "return widgetValue is not null ? widgetValue.resolve(states__60702) : themeValue is not null ? themeValue.resolve(states__60702) : defaultValue is not null ? defaultValue.resolve(states__60702) : default;", StringComparison.Ordinal);
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
                .ReplaceGeneratedLocalPattern("(Future<_LicenseData__about>)new _LicenseData__about()", "Future<_LicenseData__about>.value(new _LicenseData__about())", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("global::Doroti.Framework.Widgets.Flow flow", "global::Doroti.Runtime.Flow flow", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("Flow.begin()", "global::Doroti.Runtime.Flow.begin()", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("Flow.step(", "global::Doroti.Runtime.Flow.step(", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("Flow.end(", "global::Doroti.Runtime.Flow.end(", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("(global::System.Func<bool, List<global::Doroti.Framework.Foundation.LicenseParagraph>>)((global::Doroti.Framework.Foundation.LicenseEntry)license__34391).paragraphs.toList", "(global::System.Func<bool, List<global::Doroti.Framework.Foundation.LicenseParagraph>>)((_) => ((global::Doroti.Framework.Foundation.LicenseEntry)license__34391).paragraphs.toList())", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("((_) => { _ = this._detailArguments.value = arguments; })", "((_duration) => { this._detailArguments.value = arguments; })", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("global::Doroti.Framework.Widgets.PageRoute<_MasterDetailScaffold__about>", "global::Doroti.Framework.Widgets.PageRoute<object?>", StringComparison.Ordinal);
        }

        if (library.EndsWith("/material/carousel.dart", StringComparison.Ordinal))
        {
            source = Regex.Replace(source, @"\.max\b(?!\s*\()", ".max()");
            source = Regex.Replace(source, @"\.min\b(?!\s*\()", ".min()");
            source = Regex.Replace(source, @"\.sum\b(?!\s*\()", ".sum()");
            source = source.ReplaceGeneratedLocalPattern("public new ItemExtentBuilder? itemExtentBuilder => this._buildItemExtent;", "public new ItemExtentBuilder? itemExtentBuilder => (index, dimensions) => this._buildItemExtent(index, dimensions);", StringComparison.Ordinal);
        }

        if (library.EndsWith("/material/slider_parts.dart", StringComparison.Ordinal))
        {
            source = source
                .ReplaceGeneratedLocalPattern("Offset __thumbCenter = DartRuntimePrimitives.ConvertValue<Offset>(secondaryOffset);", "Offset __thumbCenter = thumbCenter;", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("isEnabled: textDirection", "isEnabled: isEnabled", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("((textDirection is not null))", "((secondaryOffset is not null))", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("DartRuntimePrimitives.RequireValue(textDirection).dx", "DartRuntimePrimitives.RequireValue(secondaryOffset).dx", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("textDirection.dx", "DartRuntimePrimitives.RequireValue(secondaryOffset).dx", StringComparison.Ordinal);
        }

        if (library.EndsWith("/material/scaffold.dart", StringComparison.Ordinal))
        {
            source = source
                .ReplaceGeneratedLocalPattern("._snackBars.First", "._snackBars.First()", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("._materialBanners.First", "._materialBanners.First()", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("State<Scaffold>.handleStatusBarTap();", string.Empty, StringComparison.Ordinal);
        }

        if (library.EndsWith("/material/switch.dart", StringComparison.Ordinal))
        {
            const string erasedCursor = "WidgetStateProperty.resolveWith(((global::System.Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>, T>)((states) => global::Doroti.Framework.Widgets.WidgetStateMouseCursor.clickable.resolve(states))))";
            source = source.ReplaceGeneratedLocalPattern(erasedCursor, "WidgetStateProperty.resolveWith((states) => global::Doroti.Framework.Widgets.WidgetStateMouseCursor.clickable.resolve(states))", StringComparison.Ordinal);
        }

        if (library.EndsWith("/material/app.dart", StringComparison.Ordinal) ||
            library.EndsWith("/material/expansion_panel.dart", StringComparison.Ordinal))
        {
            source = source.ReplaceGeneratedLocalPattern("?? kThemeAnimationDuration", "?? ThemeLibrary.kThemeAnimationDuration", StringComparison.Ordinal);
        }

        if (library.EndsWith("/material/text_field.dart", StringComparison.Ordinal) ||
            library.EndsWith("/material/text_form_field.dart", StringComparison.Ordinal) ||
            library.EndsWith("/material/search_anchor.dart", StringComparison.Ordinal) ||
            library.EndsWith("/material/selectable_text.dart", StringComparison.Ordinal))
        {
            source = source.ReplaceGeneratedLocalPattern("new AdaptiveTextSelectionToolbar(editableTextState: editableTextState)", "AdaptiveTextSelectionToolbar.CreateEditableText(editableTextState: editableTextState)", StringComparison.Ordinal);
            if (library.EndsWith("/material/text_field.dart", StringComparison.Ordinal))
            {
                source = source
                    .ReplaceGeneratedLocalPattern("new CupertinoSpellCheckSuggestionsToolbar(editableTextState: editableTextState)", "CupertinoSpellCheckSuggestionsToolbar.CreateEditableText(editableTextState: editableTextState)", StringComparison.Ordinal)
                    .ReplaceGeneratedLocalPattern("unregisterFromRestoration(this._controller!)", "unregisterFromRestoration(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.RestorableProperty<object>>(this._controller!))", StringComparison.Ordinal)
                    .ReplaceGeneratedLocalPattern("Text_selectionLibrary.cupertinoTextSelectionHandleControls", "Text_selectionLibrary.materialTextSelectionHandleControls", StringComparison.Ordinal)
                    .ReplaceGeneratedLocalPattern("Desktop_text_selectionLibrary.cupertinoDesktopTextSelectionHandleControls", "Desktop_text_selectionLibrary.desktopTextSelectionHandleControls", StringComparison.Ordinal);
            }
        }

        if (library.EndsWith("/material/selection_area.dart", StringComparison.Ordinal))
        {
            source = source
                .ReplaceGeneratedLocalPattern("new AdaptiveTextSelectionToolbar(selectableRegionState: selectableRegionState)", "AdaptiveTextSelectionToolbar.CreateSelectableRegion(selectableRegionState: selectableRegionState)", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("Text_selectionLibrary.cupertinoTextSelectionHandleControls", "Text_selectionLibrary.materialTextSelectionHandleControls", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("Desktop_text_selectionLibrary.cupertinoDesktopTextSelectionHandleControls", "Desktop_text_selectionLibrary.desktopTextSelectionHandleControls", StringComparison.Ordinal);
        }

        if (library.EndsWith("/material/theme_data.dart", StringComparison.Ordinal))
        {
            source = source.ReplaceGeneratedLocalPattern("public abstract class ThemeExtension<T> where T : ThemeExtension<T>", "public abstract class ThemeExtension<T>", StringComparison.Ordinal);
        }

        if (library.EndsWith("/rendering/animated_size.dart", StringComparison.Ordinal))
        {
            source = source.ReplaceGeneratedLocalPattern(
                "this._clipBehavior = clipBehavior;",
                "this._clipBehavior = clipBehavior;\n        this._onEnd = onEnd;\n        this._controller = new AnimationController(duration: duration, reverseDuration: reverseDuration, vsync: vsync);\n        this._controller.addListener(() => { if (this._controller.value != this._lastValue) markNeedsLayout(); });\n        this._animation = new CurvedAnimation(parent: this._controller, curve: __curve);",
                StringComparison.Ordinal);
        }

        if (library.EndsWith("/painting/shape_decoration.dart", StringComparison.Ordinal))
        {
            source = source
                .ReplaceGeneratedLocalPattern("_shadowPaints = new List<global::Doroti.Ui.Paint>();", "_shadowPaints = ((ShapeDecoration)this._decoration).shadows!.Select(shadow => shadow.toPaint()).ToList();", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("_shadowBounds = new List<global::Doroti.Ui.Rect>();", "_shadowBounds = ((ShapeDecoration)this._decoration).shadows!.Select(shadow => rect.shift(shadow.offset).inflate(shadow.spreadRadius)).ToList();", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("_shadowPaths = new List<global::Doroti.Ui.Path>();", "_shadowPaths = ((ShapeDecoration)this._decoration).shadows!.Select(shadow => ((ShapeDecoration)this._decoration).shape.getOuterPath(rect.shift(shadow.offset).inflate(shadow.spreadRadius), textDirection: textDirection)).ToList();", StringComparison.Ordinal);
        }

        if (library.EndsWith("/widgets/restoration.dart", StringComparison.Ordinal))
        {
            source = source
                .ReplaceGeneratedLocalPattern(
                    "((dynamic)this._owner)?._unregister(this);",
                    "if (this._owner is RestorationPropertyOwner owner)\n        {\n            owner._unregister(this);\n        }\n        else\n        {\n            ((dynamic)this._owner)?._unregister(this);\n        }",
                    StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern(
                    "public interface RestorationMixin<S> where S : StatefulWidget",
                    "public interface RestorationPropertyOwner\n{\n    public void _unregister(dynamic property);\n}\n\npublic interface RestorationMixin<S> : RestorationPropertyOwner where S : StatefulWidget",
                    StringComparison.Ordinal);
        }

        if (library.EndsWith("/cupertino/colors.dart", StringComparison.Ordinal))
        {
            source = source.ReplaceGeneratedLocalPattern(
                "public CupertinoDynamicColor(string? debugLabel = null, Color color = default!, Color darkColor = default!, Color highContrastColor = default!, Color darkHighContrastColor = default!, Color elevatedColor = default!, Color darkElevatedColor = default!, Color highContrastElevatedColor = default!, Color darkHighContrastElevatedColor = default!) : this(color, color, darkColor, highContrastColor, darkHighContrastColor, elevatedColor, darkElevatedColor, highContrastElevatedColor, darkHighContrastElevatedColor, null, debugLabel)",
                "public CupertinoDynamicColor(string? debugLabel = null, Color color = default!, Color darkColor = default!, Color? highContrastColor = null, Color? darkHighContrastColor = null, Color? elevatedColor = null, Color? darkElevatedColor = null, Color? highContrastElevatedColor = null, Color? darkHighContrastElevatedColor = null) : this(\n        color,\n        color,\n        darkColor,\n        highContrastColor ?? color,\n        darkHighContrastColor ?? darkColor,\n        elevatedColor ?? color,\n        darkElevatedColor ?? darkColor,\n        highContrastElevatedColor ?? highContrastColor ?? elevatedColor ?? color,\n        darkHighContrastElevatedColor ?? darkHighContrastColor ?? darkElevatedColor ?? darkColor,\n        null,\n        debugLabel)",
                StringComparison.Ordinal);
        }

        if (library.EndsWith("/widgets/implicit_animations.dart", StringComparison.Ordinal))
        {
            source = source.ReplaceGeneratedLocalPattern(
                "{            var __cascade = tween;\n            __cascade.begin = tween.evaluate(this._animation);",
                "{            var __cascade = tween;\n            if (__cascade is null) return null;\n            __cascade.begin = tween.evaluate(this._animation);",
                StringComparison.Ordinal);
        }

        if (library.EndsWith("/cupertino/spell_check_suggestions_toolbar.dart", StringComparison.Ordinal))
        {
            source = source
                .ReplaceGeneratedLocalPattern("return ((List<global::Doroti.Framework.Widgets.Widget>)(object?)this.buttonItems.map<global::Doroti.Framework.Widgets.ContextMenuButtonItem, CupertinoTextSelectionToolbarButton>", "return this.buttonItems.map<global::Doroti.Framework.Widgets.ContextMenuButtonItem, CupertinoTextSelectionToolbarButton>", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("})).ToList());", "})).Cast<global::Doroti.Framework.Widgets.Widget>().ToList();", StringComparison.Ordinal);
        }

        // Nullable callback properties are Dart tear-offs, not always-present
        // forwarding closures. Preserving null here is required so losing
        // recognizers do not invoke a wrapper whose delegate target is null.
        source = Regex.Replace(
            source,
            @"(?<argument>onTap|onTapCancel|onDoubleTap|onLongPress|onLongPressUp|onSecondaryTap|onSecondaryTapCancel|onEditingComplete|onVisible): \(\) => this\.\k<argument>\(\)",
            "${argument}: this.${argument}");
        source = Regex.Replace(
            source,
            @"(?<argument>onTap|onTapCancel|onDoubleTap|onLongPress|onLongPressUp|onSecondaryTap|onSecondaryTapCancel|onEditingComplete|onVisible): \(\) => \k<argument>\(\)",
            "${argument}: ${argument}");

        if (library.EndsWith("/material/text_form_field.dart", StringComparison.Ordinal))
        {
            source = source.ReplaceGeneratedLocalPattern(
                "autofillHints: autofillHints.Cast<string>()",
                "autofillHints: autofillHints",
                StringComparison.Ordinal);
        }

        if (!library.Contains("/widgets/", StringComparison.Ordinal))
        {
            return source;
        }

        source = source
            .ReplaceGeneratedLocalPattern("this.onStateChange?.Invoke(this._lifecycleState!)", "this.onStateChange?.Invoke(DartRuntimePrimitives.RequireValue(this._lifecycleState))", StringComparison.Ordinal)
            .ReplaceGeneratedLocalPattern("this.widget.dragAnchorStrategy(this.widget, this.context, position)", "((dynamic)this.widget.dragAnchorStrategy)(this.widget, this.context, position)", StringComparison.Ordinal)
            .ReplaceGeneratedLocalPattern("orElse: (() => { })", "orElse: (() => default!)", StringComparison.Ordinal)
            .ReplaceGeneratedLocalPattern("_makeOverridable((__arg0) => ((Action<TransposeCharactersIntent>)", "_makeOverridable<TransposeCharactersIntent>((__arg0) => ((Action<TransposeCharactersIntent>)", StringComparison.Ordinal)
            .ReplaceGeneratedLocalPattern("((global::System.Func<object, bool>)((FocusScopeNode)nodeScope__42268)._focusedChildren.Remove)(__arg0)", "((FocusScopeNode)nodeScope__42268)._focusedChildren.Remove(DartRuntimePrimitives.ConvertValue<FocusScopeNode>(__arg0))", StringComparison.Ordinal)
            .ReplaceGeneratedLocalPattern("(((object?)spellCheckService__128604 ?? (object?)new global::Doroti.Framework.Services.DefaultSpellCheckService()))", "DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Services.SpellCheckService>(spellCheckService__128604 ?? new global::Doroti.Framework.Services.DefaultSpellCheckService())", StringComparison.Ordinal)
            .ReplaceGeneratedLocalPattern("((global::Doroti.Framework.Rendering.ContainerRenderObjectMixin<global::Doroti.Framework.Rendering.RenderObject, global::Doroti.Framework.Rendering.ContainerParentDataMixin<global::Doroti.Framework.Rendering.RenderObject>>?)(object?)base.renderObject)!", "DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.RenderObject>(base.renderObject)", StringComparison.Ordinal)
            .ReplaceGeneratedLocalPattern("((IndexedSlot<Element?>)((IndexedSlot<Element?>)__other))", "((IndexedSlot<T>)(object)__other)", StringComparison.Ordinal)
            .ReplaceGeneratedLocalPattern("gestures ?? new DartMap<Type, GestureRecognizerFactory<global::Doroti.Framework.Gestures.GestureRecognizer>>()", "gestures ?? new DartMap<Type, GestureRecognizerFactory<global::Doroti.Framework.Gestures.GestureRecognizer>>().cast<Type, dynamic>()", StringComparison.Ordinal)
            .ReplaceGeneratedLocalPattern("double value__as40867 = (double)value", "double value__as40867 = (double)(object)value!", StringComparison.Ordinal)
            .ReplaceGeneratedLocalPattern(".ToList().Cast<EditableText>().ToList()", ".ToList().Cast<string>().ToList()", StringComparison.Ordinal)
            .ReplaceGeneratedLocalPattern("begin: value)", "begin: DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Offset>(value))", StringComparison.Ordinal)
            .ReplaceGeneratedLocalPattern("return ((T?)(object?)scope__22436?.localizationsState.resourcesFor<T?>(type));", "return scope__22436 is null ? default : scope__22436.localizationsState.resourcesFor<T>(type);", StringComparison.Ordinal)
            .ReplaceGeneratedLocalPattern("this._childElements.Remove(((Element)child).slot)", "this._childElements.Remove(DartRuntimePrimitives.ConvertValue<long>(((Element)child).slot))", StringComparison.Ordinal)
            .ReplaceGeneratedLocalPattern("this._childElements.ContainsKey(((Element)child).slot)", "this._childElements.ContainsKey(DartRuntimePrimitives.ConvertValue<long>(((Element)child).slot))", StringComparison.Ordinal)
            .ReplaceGeneratedLocalPattern("((dynamic)this.renderObject).child = DartRuntimePrimitives.ConvertValue<Element>(__child)", "((dynamic)this.renderObject).child = __child", StringComparison.Ordinal)
            .ReplaceGeneratedLocalPattern("internal virtual GlobalKey<IState> _anchorKey { get; private set; } = ((GlobalKey<IState>)(object?)GlobalKey<_RawMenuAnchorState__raw_menu_anchor>.Create(debugLabel: (global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode ? null : \"MenuAnchor\")));", "internal virtual GlobalKey<_RawMenuAnchorState__raw_menu_anchor> _anchorKey { get; private set; } = GlobalKey<_RawMenuAnchorState__raw_menu_anchor>.Create(debugLabel: (global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode ? null : \"MenuAnchor\"));", StringComparison.Ordinal)
            .ReplaceGeneratedLocalPattern("onPopInvoked ?? _defaultPopInvokedHandler", "onPopInvoked ?? ((didPop, result) => _defaultPopInvokedHandler(didPop, result))", StringComparison.Ordinal)
            .ReplaceGeneratedLocalPattern("createInnerBallisticScrollActivity(this.@delegate, this.velocity)", "createInnerBallisticScrollActivity(DartRuntimePrimitives.ConvertValue<_NestedScrollPosition__nested_scroll_view>(this.@delegate), this.velocity)", StringComparison.Ordinal)
            .ReplaceGeneratedLocalPattern("nestOffset(value, this.@delegate)", "nestOffset(value, DartRuntimePrimitives.ConvertValue<_NestedScrollPosition__nested_scroll_view>(this.@delegate))", StringComparison.Ordinal)
            .ReplaceGeneratedLocalPattern("InsertRange(_insertionIndex(below, above),", "InsertRange(checked((int)_insertionIndex(below, above)),", StringComparison.Ordinal)
            .ReplaceGeneratedLocalPattern("old__32464.removeAll(newEntriesList__31689.Cast<object>())", "old__32464.RemoveAll(item => newEntriesList__31689.Contains(item))", StringComparison.Ordinal)
            .ReplaceGeneratedLocalPattern("this.overlayLocation!._detachFromLayoutSurrogate(deferredChild__101178)", "this.overlayLocation!._detachFromLayoutSurrogate(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.RenderDeferredLayoutBox>(deferredChild__101178))", StringComparison.Ordinal)
            .ReplaceGeneratedLocalPattern("this.widget.onCloseRequested((global::System.Action<bool>)this.close)", "this.widget.onCloseRequested(() => this.close())", StringComparison.Ordinal)
            .ReplaceGeneratedLocalPattern("defaultValue: (((this.semanticsTooltip is null) || (this.semanticsTooltip!.Length == 0)) ? null : global::Doroti.Framework.Foundation.DiagnosticsLibrary.kNoDefaultValue)", "defaultValue: (((this.semanticsTooltip is null) || (this.semanticsTooltip!.Length == 0)) ? null : global::Doroti.Framework.Foundation.DiagnosticsLibrary.kNoDefaultValue.ToString())", StringComparison.Ordinal)
            .ReplaceGeneratedLocalPattern("_ReorderableItemGlobalKey__reorderable_list.Create(((Widget)child__40762).key!, index, this)", "_ReorderableItemGlobalKey__reorderable_list.Create(key: ((Widget)child__40762).key!, index: index, state: this)", StringComparison.Ordinal)
            .ReplaceGeneratedLocalPattern("await delegateRouteSetter()(data)", "await delegateRouteSetter()(DartRuntimePrimitives.ConvertValue<T>(data))", StringComparison.Ordinal)
            .ReplaceGeneratedLocalPattern("public virtual void takePriority() => this._children.clear();", "public virtual void takePriority() => ((dynamic)this._children).clear();", StringComparison.Ordinal)
            .ReplaceGeneratedLocalPattern("this._history.where(_RouteEntry__navigator.isRoutePredicate(route))", "this._history.where((entry) => _RouteEntry__navigator.isRoutePredicate(route)(DartRuntimePrimitives.ConvertValue<_RouteEntry__navigator>(entry)))", StringComparison.Ordinal)
            .ReplaceGeneratedLocalPattern("(minOverscrollLength ?? minLength)", "DartRuntimePrimitives.RequireValue(minOverscrollLength ?? minLength)", StringComparison.Ordinal)
            .ReplaceGeneratedLocalPattern("((ScrollAwareImageProvider<object>)((ScrollAwareImageProvider<object>)__other))", "((ScrollAwareImageProvider<T>)(object)__other)", StringComparison.Ordinal)
            .ReplaceGeneratedLocalPattern("semanticIndexCallback ?? Scroll_delegateLibrary._kDefaultSemanticIndexCallback", "semanticIndexCallback ?? ((widget, index) => Scroll_delegateLibrary._kDefaultSemanticIndexCallback(widget, index))", StringComparison.Ordinal)
            .ReplaceGeneratedLocalPattern("treeRowExtentBuilder ?? TreeSliver<T>.defaultTreeRowExtentBuilder", "treeRowExtentBuilder ?? ((node, dimensions) => TreeSliver<T>.defaultTreeRowExtentBuilder(node, dimensions))", StringComparison.Ordinal)
            .ReplaceGeneratedLocalPattern("return callingAction__81081.invoke(intent);", "return ((dynamic)callingAction__81081).invoke(intent);", StringComparison.Ordinal)
            .ReplaceGeneratedLocalPattern("DartAsyncRuntime.scheduleMicrotask((global::System.Action<Duration?>)runScheduledTask)", "DartAsyncRuntime.scheduleMicrotask(() => runScheduledTask(null))", StringComparison.Ordinal)
            .ReplaceGeneratedLocalPattern("(global::System.Func<global::Doroti.Framework.Rendering.Selectable, global::Doroti.Framework.Rendering.Selectable, long>)this.compareOrder", "((left, right) => this.compareOrder(left, right))", StringComparison.Ordinal)
            .ReplaceGeneratedLocalPattern("return _adjustingSelectionEnd = (forward != isReversed__68050);", "return DartRuntimePrimitives.RequireValue(_adjustingSelectionEnd = (forward != isReversed__68050));", StringComparison.Ordinal)
            .ReplaceGeneratedLocalPattern("new FutureBuilder<object?>(future: this._sensitiveContentRegistrationFuture", "new FutureBuilder<object?>(future: DartRuntimePrimitives.ConvertValue<Future<object?>>(this._sensitiveContentRegistrationFuture)", StringComparison.Ordinal)
            .ReplaceGeneratedLocalPattern("var (enabled__36562, invokeResult__36579) = Actions.of(context__36354).invokeActionIfEnabled(action__36418, intent__36280, context__36354);", "var (enabled__36562, invokeResult__36579) = DartRuntimePrimitives.ConvertValue<(bool, object?)>(Actions.of(context__36354).invokeActionIfEnabled(action__36418, intent__36280, context__36354));", StringComparison.Ordinal)
            .ReplaceGeneratedLocalPattern("RenderViewportBase.showInViewport", "RenderViewportBase<global::Doroti.Framework.Rendering.ContainerParentDataMixin<global::Doroti.Framework.Rendering.RenderSliver>>.showInViewport", StringComparison.Ordinal)
            .ReplaceGeneratedLocalPattern("_setChild(default, __slot)", "_setChild(default(SlotType)!, __slot)", StringComparison.Ordinal);

        // The analyzer represents a zero-argument instance-method tear-off as a
        // forwarding closure in some Flutter libraries. Retain CLR method-group
        // identity for ChangeNotifier listener registration/removal; otherwise
        // each lowered lambda is a distinct delegate and removeListener leaks it.
        source = Regex.Replace(
            source,
            @"\.(addListener|removeListener)\(\(\) => this\.([A-Za-z_][A-Za-z0-9_]*)\(\)\);",
            ".$1(this.$2);");
        source = Regex.Replace(
            source,
            @"this\._overscrollNotifier\.(addListener|removeListener)\(\(\) => listener\(\)\);",
            "this._overscrollNotifier.$1(listener);");

        if (library.EndsWith("/editable_text.dart", StringComparison.Ordinal))
        {
            source = source
                .ReplaceGeneratedLocalPattern("__cascade.cursorHeight = this.cursorHeight", "__cascade.cursorHeight = DartRuntimePrimitives.RequireValue(this.cursorHeight)", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("_makeOverridable<TransposeCharactersIntent>((__arg0) => ((Action<TransposeCharactersIntent>)this._transposeCharactersAction)(DartRuntimePrimitives.ConvertValue<TransposeCharactersIntent>(__arg0)))", "_makeOverridable<TransposeCharactersIntent>(this._transposeCharactersAction)", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("return ((global::System.Action)((global::System.Func<Future>)(", "return DartRuntimePrimitives.AdaptAsyncCallback((global::System.Func<Future>?)(", StringComparison.Ordinal);
            source = Regex.Replace(
                source,
                @"(return DartRuntimePrimitives\.AdaptAsyncCallback[\s\S]*?\}\) : null)\)\)\);",
                "$1));");
        }

        if (library.EndsWith("/form.dart", StringComparison.Ordinal))
        {
            source = source
                .ReplaceGeneratedLocalPattern("(global::System.Action<bool, object>)", "(object?)", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("onPopInvokedWithResult: (object?)((Form)(object)this.widget)._callPopInvoked", "onPopInvokedWithResult: (global::System.Action<bool, object?>)((Form)(object)this.widget)._callPopInvoked", StringComparison.Ordinal);
        }

        if (library.EndsWith("/framework.dart", StringComparison.Ordinal))
        {
            source = source.ReplaceGeneratedLocalPattern(
                "public static GlobalKey<T> Create(string? debugLabel = null) => ((GlobalKey<T>)(object?)LabeledGlobalKey<T>.Create(debugLabel));",
                "public static GlobalKey<T> Create(string? debugLabel = null) => new LabeledGlobalKey<T>(debugLabel);",
                StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern(
                "global::Doroti.Framework.Foundation.FlutterError.Create(new List<global::Doroti.Framework.Foundation.DiagnosticsNode>",
                "new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode>",
                StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("global::Doroti.Framework.Foundation.DiagnosticsProperty<GlobalKey<IState>>.Create(", "new global::Doroti.Framework.Foundation.DiagnosticsProperty<GlobalKey<IState>>(", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern(
                    "        this._state = widget.createState();\n    }",
                    "        this._state = widget.createState();\n        this._state!._element = this;\n        this._state!._widget = widget;\n    }",
                    StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern(
                    "            _child = updateChild(((Element)(object)null), built__246655, this.slot);\n        }\n    }\n\n    public abstract Widget build();",
                    "            _child = updateChild(((Element)(object)null), built__246655, this.slot);\n        }\n        base.performRebuild();\n    }\n\n    public abstract Widget build();",
                    StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern(
                    "_inheritedElements = this._parent?._inheritedElements;",
                    "_inheritedElements = this._parent?._inheritedElements ?? global::Doroti.Framework.Foundation.PersistentHashMap<Type, InheritedElement>.CreateEmpty();",
                    StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("((dynamic)this.state)._element = null;", "this.state._element = null;", StringComparison.Ordinal);
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
                .ReplaceGeneratedLocalPattern(
                    "public class Navigator : StatefulWidget\n{",
                    "public class Navigator : StatefulWidget\n{\n    internal static readonly List<Page<object>> _defaultPages = new();",
                    StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern(
                    "List<Page<object>> __pages = pages ?? new List<Page<object>>();",
                    "List<Page<object>> __pages = pages ?? _defaultPages;",
                    StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern(
                    "internal virtual bool _usingPagesAPI => DartRuntimePrimitives.ConvertValue<bool>((!object.Equals(((Navigator)(object)this.widget).pages, new List<Page<object>>())));",
                    "internal virtual bool _usingPagesAPI => !ReferenceEquals(((Navigator)(object)this.widget).pages, Navigator._defaultPages);",
                    StringComparison.Ordinal);
        }

        if (library.EndsWith("/rendering/table.dart", StringComparison.Ordinal))
        {
            source = source.ReplaceGeneratedLocalPattern(
                "        System.Diagnostics.Debug.Assert(((rows is null) || (children is null)));",
                "        System.Diagnostics.Debug.Assert(((rows is null) || (children is null)));\n        _children = new List<RenderBox?>(System.Linq.Enumerable.Repeat<RenderBox?>(null, checked((int)(this._columns * this._rows))));\n        this.rowDecorations = rowDecorations;\n        children?.forEach(this.addRow);",
                StringComparison.Ordinal);
        }

        if (library.EndsWith("/focus_manager.dart", StringComparison.Ordinal))
        {
            source = source
                .ReplaceGeneratedLocalPattern(
                "    public FocusManager()\n    {\n    }",
                "    public FocusManager()\n    {\n        this.rootScope._manager = this;\n    }",
                StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern(
                    "            node__80998._notify();",
                    "            node__80998?._notify();",
                    StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern(
                    "            ((FocusScopeNode)ancestor__49778)._focusedChildren.Remove(scopeFocus__49729);",
                    "            if (((FocusScopeNode)ancestor__49778)._focusedChildren.Contains(scopeFocus__49729)) ((FocusScopeNode)ancestor__49778)._focusedChildren.Remove(scopeFocus__49729);",
                    StringComparison.Ordinal);
        }

        if (library.EndsWith("/binding.dart", StringComparison.Ordinal))
        {
            source = source
                .ReplaceGeneratedLocalPattern(
                "    public virtual ImageCache _imageCache { get; set; } = default!;",
                "    public WidgetsFlutterBinding(PlatformDispatcher? platformDispatcher = null) : base(platformDispatcher) { }\n\n    public virtual ImageCache _imageCache { get; set; } = default!;",
                StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern(
                "        PaintingBinding._instance = this;\n        this._imageCache = createImageCache();",
                "        PaintingBinding._instance = this;\n        global::Doroti.Framework.Semantics.SemanticsBinding._instance = this;\n        global::Doroti.Framework.Rendering.RendererBinding._instance = this;\n        WidgetsBinding._instance = this;\n        this._buildOwner = new BuildOwner();\n        this._buildOwner.onBuildScheduled = _handleBuildScheduled;\n        this._imageCache = createImageCache();",
                StringComparison.Ordinal);
        }

        if (library.EndsWith("/overlay.dart", StringComparison.Ordinal))
        {
            source = source
                .ReplaceGeneratedLocalPattern("this._entries.Cast<Overlay>().ToList(), newEntriesList__31689.Cast<Overlay>().ToList()", "this._entries, newEntriesList__31689", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("global::Doroti.Framework.Rendering.RenderDeferredLayoutBox", "_RenderDeferredLayoutBox__overlay", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("old__32464.RemoveAll(item => newEntriesList__31689.Contains(item))", "old__32464.ExceptWith(newEntriesList__31689)", StringComparison.Ordinal);
        }

        if (library.EndsWith("/platform_menu_bar.dart", StringComparison.Ordinal))
        {
            source = source.ReplaceGeneratedLocalPattern(
                "newDescendants__19362.Cast<PlatformMenuBar>().ToList(), this.descendants.Cast<PlatformMenuBar>().ToList()",
                "newDescendants__19362, this.descendants",
                StringComparison.Ordinal);
        }

        if (library.EndsWith("/overscroll_indicator.dart", StringComparison.Ordinal))
        {
            source = source.ReplaceGeneratedLocalPattern("})));\n        this._controller?.dispose();", "return default!;\n})));\n        this._controller?.dispose();", StringComparison.Ordinal);
        }

        if (library.EndsWith("/platform_view.dart", StringComparison.Ordinal))
        {
            source = Regex.Replace(source, @"_DarwinViewState__platform_view<[^;]+?>\._emptyRecognizersSet", "_emptyRecognizersSet");
        }

        if (library.EndsWith("/reorderable_list.dart", StringComparison.Ordinal))
        {
            source = source.ReplaceGeneratedLocalPattern("_ReorderableItem__reorderable_list.Create(key:", "new _ReorderableItem__reorderable_list(", StringComparison.Ordinal);
        }

        if (library.EndsWith("/router.dart", StringComparison.Ordinal))
        {
            source = source.ReplaceGeneratedLocalPattern(", () => callback())", ", (global::System.Action)(() => callback()))", StringComparison.Ordinal);
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
                .ReplaceGeneratedLocalPattern("DartMap<Type, GestureRecognizerFactory<global::Doroti.Framework.Gestures.GestureRecognizer>> _gestures", "DartMap<Type, dynamic> _gestures", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("return ((DartMap<Type, object>)(object?)gestures__75996);", "return gestures__75996;", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("foregroundPainter: this.scrollbarPainter", "foregroundPainter: new _ScrollbarCustomPainterAdapter(this.scrollbarPainter)", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("internal virtual Timer? _fadeoutTimer { get; set; } = default;", "internal virtual Timer? _fadeoutTimer { get; set; } = default;\n    internal virtual bool _isDisposed { get; set; }", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("this._fadeoutAnimationController.reverse();\n_fadeoutTimer = null;", "if (!this._isDisposed)\n{\n    try\n    {\n        this._fadeoutAnimationController.reverse();\n    }\n    catch (ObjectDisposedException)\n    {\n        // The host dispatcher can finish before a delayed scrollbar fade.\n    }\n}\n_fadeoutTimer = null;", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("this._fadeoutAnimationController.dispose();\n        this._fadeoutTimer?.cancel();", "_isDisposed = true;\n        this._fadeoutTimer?.cancel();\n        _fadeoutTimer = null;\n        this._fadeoutAnimationController.dispose();", StringComparison.Ordinal);
            source = Regex.Replace(
                source,
                @"\(\(ScrollbarPainter\?\)\(object\?\)\(\(CustomPaint\)(?<paint>customPaint__\d+)\)\.foregroundPainter!\)!",
                "_ScrollbarCustomPainterAdapter.Unwrap(((CustomPaint)${paint}).foregroundPainter!)");
            source += "\ninternal sealed class _ScrollbarCustomPainterAdapter : global::Doroti.Framework.Rendering.CustomPainter\n{\n    private readonly ScrollbarPainter _owner;\n    internal _ScrollbarCustomPainterAdapter(ScrollbarPainter owner) : base(owner) => _owner = owner;\n    internal static ScrollbarPainter Unwrap(global::Doroti.Framework.Rendering.CustomPainter painter) => painter is _ScrollbarCustomPainterAdapter adapter ? adapter._owner : (ScrollbarPainter)(object)painter;\n    public override void paint(Canvas canvas, Size size) => _owner.paint(canvas, size);\n    public override bool shouldRepaint(global::Doroti.Framework.Rendering.CustomPainter oldDelegate) => oldDelegate is not _ScrollbarCustomPainterAdapter other || _owner.shouldRepaint(other._owner);\n    public override bool? hitTest(Offset position) => _owner.hitTest(position);\n}\n";
        }

        if (library.EndsWith("/selectable_region.dart", StringComparison.Ordinal) ||
            library.EndsWith("/text.dart", StringComparison.Ordinal))
        {
            source = source.ReplaceGeneratedLocalPattern(
                "DartRuntimePrimitives.ConvertValue<Comparison<global::Doroti.Framework.Rendering.Selectable>>(_compareScreenOrder)",
                "new Comparison<global::Doroti.Framework.Rendering.Selectable>((left, right) => checked((int)_compareScreenOrder(left, right)))",
                StringComparison.Ordinal);
        }

        if (library.EndsWith("/shortcuts.dart", StringComparison.Ordinal))
        {
            source = source.ReplaceGeneratedLocalPattern("DartRuntimePrimitives.ConvertValue<(bool, object?)>(Actions.of(context__36354).invokeActionIfEnabled", "DartRuntimePrimitives.ConvertValue<(bool, object?)>((object)Actions.of(context__36354).invokeActionIfEnabled", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("intent__36280, context__36354));", "intent__36280, context__36354));", StringComparison.Ordinal);
        }

        if (library.EndsWith("/sliver.dart", StringComparison.Ordinal))
        {
            source = source.ReplaceGeneratedLocalPattern("newChildren__37713.putIfAbsent(index__39165, (() => { }));", "newChildren__37713.putIfAbsent(index__39165, (() => default!));", StringComparison.Ordinal);
        }

        if (library.EndsWith("/sliver_tree.dart", StringComparison.Ordinal))
        {
            source = source
                .ReplaceGeneratedLocalPattern("ItemExtentBuilder __treeRowExtentBuilder", "global::System.Func<TreeSliverNode<T>, global::Doroti.Framework.Rendering.SliverLayoutDimensions, double?> __treeRowExtentBuilder", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("semanticIndexCallback ?? Sliver_treeLibrary._kDefaultSemanticIndexCallback", "semanticIndexCallback ?? ((widget, index) => Sliver_treeLibrary._kDefaultSemanticIndexCallback(widget, index))", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("activeAnimations: this._activeAnimations, itemBuilder:", "activeAnimations: this._activeAnimations.cast<global::Doroti.Framework.Foundation.UniqueKey, (long fromIndex, long toIndex, double value)>(), itemBuilder:", StringComparison.Ordinal);
        }

        if (library.EndsWith("/sliver_resizing_header.dart", StringComparison.Ordinal))
        {
            source = source
                .ReplaceGeneratedLocalPattern("public virtual IEnumerable<_Slot__sliver_resizing_header> slots", "public override IEnumerable<_Slot__sliver_resizing_header> slots", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("public virtual Widget? childForSlot(_Slot__sliver_resizing_header slot)", "public override Widget? childForSlot(_Slot__sliver_resizing_header slot)", StringComparison.Ordinal);
        }

        if (library.EndsWith("/slotted_render_object_widget.dart", StringComparison.Ordinal))
        {
            source = source
                .ReplaceGeneratedLocalPattern(" : RenderObjectWidget where ChildType", " : RenderObjectWidget, SlottedMultiChildRenderObjectWidgetMixin<SlotType, ChildType> where ChildType", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("    public override SlottedRenderObjectElement<SlotType, ChildType> createElement()", "    public abstract IEnumerable<SlotType> slots { get; }\n    public abstract Widget? childForSlot(SlotType slot);\n\n    public override SlottedRenderObjectElement<SlotType, ChildType> createElement()", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern(" : base(widget)", " : base((RenderObjectWidget)widget)", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("base.update(__newWidget)", "base.update((Widget)__newWidget)", StringComparison.Ordinal);
        }

        if (library.EndsWith("/single_child_scroll_view.dart", StringComparison.Ordinal))
        {
            source = source.ReplaceGeneratedLocalPattern(
                "RenderViewportBase<global::Doroti.Framework.Rendering.ContainerParentDataMixin<global::Doroti.Framework.Rendering.RenderSliver>>.showInViewport(descendant: descendant, viewport: this, offset: this.offset, rect: rect, duration: duration, curve: curve)",
                "_showInViewport(descendant, rect, this, this.offset, duration, curve)",
                StringComparison.Ordinal);
            source = source.ReplaceGeneratedLocalPattern(
                "    public override void debugFillProperties",
                "    private static Rect? _showInViewport(global::Doroti.Framework.Rendering.RenderObject? descendant, Rect? rect, _RenderSingleChildViewport__single_child_scroll_view viewport, global::Doroti.Framework.Rendering.ViewportOffset offset, Duration duration, global::Doroti.Framework.Animation.Curve curve) { if (descendant is null) return rect; var leading = viewport.getOffsetToReveal(descendant, 0.0, rect: rect); var trailing = viewport.getOffsetToReveal(descendant, 1.0, rect: rect); var target = global::Doroti.Framework.Rendering.RevealedOffset.clampOffset(leading, trailing, offset.pixels); if (target is null) return rect ?? descendant.paintBounds; _ = offset.moveTo(target.offset, duration: duration, curve: curve); return target.rect; }\n\n    public override void debugFillProperties",
                StringComparison.Ordinal);
        }

        if (library.EndsWith("/scroll_view.dart", StringComparison.Ordinal))
        {
            source = source
                .ReplaceGeneratedLocalPattern("primary: DartRuntimePrimitives.RequireValue(primary)", "primary: primary", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("cacheExtent: DartRuntimePrimitives.RequireValue(cacheExtent)", "cacheExtent: cacheExtent", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("keyboardDismissBehavior: DartRuntimePrimitives.RequireValue(keyboardDismissBehavior)", "keyboardDismissBehavior: keyboardDismissBehavior", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("semanticChildCount: DartRuntimePrimitives.RequireValue(semanticChildCount)", "semanticChildCount: semanticChildCount", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("checked((long)(children.Count))", "checked((long)((children ?? new List<Widget>()).Count))", StringComparison.Ordinal);
        }

        if (library.EndsWith("/page_view.dart", StringComparison.Ordinal))
        {
            source = source.ReplaceGeneratedLocalPattern(
                "initialPixels: ((double)(object)null)",
                "initialPixels: null",
                StringComparison.Ordinal);
        }

        if (library.EndsWith("/ticker_provider.dart", StringComparison.Ordinal))
        {
            source = source
                .ReplaceGeneratedLocalPattern("return (((object?)widget__4995?.notifier ?? (object?)new _ConstantValueListenable__ticker_provider<bool>(true)));", "return DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Foundation.ValueListenable<bool>>((object?)widget__4995?.notifier ?? (object?)new _ConstantValueListenable__ticker_provider<bool>(true));", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("return (((object?)widget__8322?.valuesNotifier ?? (object?)fallback__7914));", "return DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>>((object?)widget__8322?.valuesNotifier ?? (object?)fallback__7914);", StringComparison.Ordinal);
        }

        if (library.EndsWith("/text_selection.dart", StringComparison.Ordinal))
        {
            source = source
                .ReplaceGeneratedLocalPattern(
                    "this.editableText.userUpdateTextEditingValue(((EditableTextState)this.editableText).textEditingValue.copyWith(selection: newSelection__116605), cause)",
                    "this.editableText.userUpdateTextEditingValue(((EditableTextState)this.editableText).textEditingValue.copyWith(selection: newSelection__116605), DartRuntimePrimitives.RequireValue(cause))",
                    StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern(
                    "        this._value = value;\n    }\n\n    public virtual global::Doroti.Framework.Services.TextEditingValue value",
                    "        this._value = value;\n        this.renderObject.selectionStartInViewport.addListener(this._updateTextSelectionOverlayVisibilities);\n        this.renderObject.selectionEndInViewport.addListener(this._updateTextSelectionOverlayVisibilities);\n        this._updateTextSelectionOverlayVisibilities();\n        this._selectionOverlay = new SelectionOverlay(\n            magnifierConfiguration: magnifierConfiguration, context: context, debugRequiredFor: debugRequiredFor,\n            startHandleType: global::Doroti.Framework.Rendering.TextSelectionHandleType.collapsed,\n            startHandlesVisible: this._effectiveStartHandleVisibility, lineHeightAtStart: 0.0,\n            onStartHandleDragStart: this._handleSelectionStartHandleDragStart, onStartHandleDragUpdate: this._handleSelectionStartHandleDragUpdate,\n            onStartHandleDragEnd: this._handleAnyDragEnd, endHandleType: global::Doroti.Framework.Rendering.TextSelectionHandleType.collapsed,\n            endHandlesVisible: this._effectiveEndHandleVisibility, lineHeightAtEnd: 0.0,\n            onEndHandleDragStart: this._handleSelectionEndHandleDragStart, onEndHandleDragUpdate: this._handleSelectionEndHandleDragUpdate,\n            onEndHandleDragEnd: this._handleAnyDragEnd, toolbarVisible: this._effectiveToolbarVisibility,\n            selectionEndpoints: new List<global::Doroti.Framework.Rendering.TextSelectionPoint>(), selectionControls: selectionControls,\n            selectionDelegate: selectionDelegate, clipboardStatus: clipboardStatus, startHandleLayerLink: startHandleLayerLink,\n            endHandleLayerLink: endHandleLayerLink, toolbarLayerLink: toolbarLayerLink, onSelectionHandleTapped: onSelectionHandleTapped,\n            dragStartBehavior: dragStartBehavior, toolbarLocation: renderObject.lastSecondaryTapDownPosition);\n    }\n\n    public virtual global::Doroti.Framework.Services.TextEditingValue value",
                    StringComparison.Ordinal);
        }

        if (library.EndsWith("/widgets/text_selection.dart", StringComparison.Ordinal))
        {
            // Dart's mixin has no state or callable contract; only its runtime
            // type identity is observed. Emit it as a CLR marker interface so
            // Material/Cupertino controls can keep their concrete superclass.
            source = Regex.Replace(
                source,
                @"public abstract class TextSelectionHandleControls : TextSelectionControls\s*\{[\s\S]*?\n\}\s*$",
                "public interface TextSelectionHandleControls\n{\n}\n");
            source = source
                .ReplaceGeneratedLocalPattern(
                    "visibility?.addListener(() => this._handleVisibilityChanged())",
                    "visibility?.addListener(this._handleVisibilityChanged)",
                    StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern(
                    "visibility?.removeListener(() => this._handleVisibilityChanged())",
                    "visibility?.removeListener(this._handleVisibilityChanged)",
                    StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern(
                    "onSelectionHandleTapped: () => this.onSelectionHandleTapped()",
                    "onSelectionHandleTapped: this.onSelectionHandleTapped",
                    StringComparison.Ordinal);
        }

        if (library.EndsWith("/widgets/editable_text.dart", StringComparison.Ordinal))
        {
            source = source.ReplaceGeneratedLocalPattern(
                "onSelectionHandleTapped: () => ((EditableText)(object)this.widget).onSelectionHandleTapped()",
                "onSelectionHandleTapped: ((EditableText)(object)this.widget).onSelectionHandleTapped",
                StringComparison.Ordinal);
        }

        if (library.EndsWith("/widgets/magnifier.dart", StringComparison.Ordinal))
        {
            source = source.ReplaceGeneratedLocalPattern(
                "public virtual void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset)",
                "public override void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset)",
                StringComparison.Ordinal);
        }

        if (library.EndsWith("/material/text_selection_toolbar.dart", StringComparison.Ordinal))
        {
            // These are RenderObject overrides in Flutter. If emitted as new
            // virtual methods, the render pipeline dispatches to RenderProxyBox
            // and the toolbar never installs/uses ToolbarItemsParentData.
            source = source
                .ReplaceGeneratedLocalPattern("public virtual void performLayout()", "public override void performLayout()", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("public virtual void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset)", "public override void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset)", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("public virtual bool hitTestChildren(global::Doroti.Framework.Rendering.BoxHitTestResult result, Offset position)", "public override bool hitTestChildren(global::Doroti.Framework.Rendering.BoxHitTestResult result, Offset position)", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("public virtual void setupParentData(global::Doroti.Framework.Rendering.RenderObject child)", "public override void setupParentData(global::Doroti.Framework.Rendering.RenderObject child)", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("public virtual void applyPaintTransform(global::Doroti.Framework.Rendering.RenderObject child, Matrix4 transform)", "public override void applyPaintTransform(global::Doroti.Framework.Rendering.RenderObject child, Matrix4 transform)", StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern("children.Cast<_TextSelectionToolbarOverflowable__text_selection_toolbar>().ToList()", "children", StringComparison.Ordinal);
        }

        if (library.EndsWith("/widgets/routes.dart", StringComparison.Ordinal))
        {
            source = source
                .ReplaceGeneratedLocalPattern(
                    "            return this.popDisposition;\n            return default!;",
                    "            return base.popDisposition;\n            return default!;",
                    StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern(
                    "        DartRuntimePrimitives.Ignore(disposed.then((global::System.Action<object>)((_) => {",
                    "        if (disposed is not null)\n        {\n            DartRuntimePrimitives.Ignore(disposed.then((global::System.Action<object>)((_) => {",
                    StringComparison.Ordinal)
                .ReplaceGeneratedLocalPattern(
                    "})));\n    }\n\n    public virtual bool canTransitionTo",
                    "})));\n        }\n    }\n\n    public virtual bool canTransitionTo",
                    StringComparison.Ordinal);
        }

        if (library.EndsWith("/animation/animation_controller.dart", StringComparison.Ordinal))
        {
            source = source.ReplaceGeneratedLocalPattern(
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
        .ReplaceGeneratedLocalPattern("global::System.Func<object> callback", "dynamic callback", StringComparison.Ordinal)
        .ReplaceGeneratedLocalPattern("global::System.Func<string, object> callback", "dynamic callback", StringComparison.Ordinal)
        .ReplaceGeneratedLocalPattern("global::System.Func<string?, string, object> callback", "dynamic callback", StringComparison.Ordinal)
        .ReplaceGeneratedLocalPattern("(global::System.Action<global::Doroti.Framework.Foundation.FlutterErrorDetails>)(value ? this._reportStructuredError : defaultExceptionHandler__36848)", "(value ? (global::Doroti.Framework.Foundation.FlutterExceptionHandler)this._reportStructuredError : defaultExceptionHandler__36848)", StringComparison.Ordinal)
        .ReplaceGeneratedLocalPattern("this._objectToId.GetValueOrDefault(@object)", "this._objectToId[@object]", StringComparison.Ordinal)
        .ReplaceGeneratedLocalPattern("pubRootDirectories = DartRuntimePrimitives.ConvertValue<global::System.Func<DartMap<string, string>, Future<DartMap<string, object>>>>(pubRootDirectories.map<string, string>", "pubRootDirectories = pubRootDirectories.map<string, string>", StringComparison.Ordinal)
        .ReplaceGeneratedLocalPattern(".path)).ToList());", ".path)).ToList();", StringComparison.Ordinal)
        .ReplaceGeneratedLocalPattern("this.selection.currentElement = @object", "this.selection.currentElement = (Element)@object", StringComparison.Ordinal)
        .ReplaceGeneratedLocalPattern("this.selection.current = @object", "this.selection.current = (global::Doroti.Framework.Rendering.RenderObject)@object", StringComparison.Ordinal)
        .ReplaceGeneratedLocalPattern("global::Doroti.Framework.Foundation.DebugLibrary.connectedVmServiceUri,", "global::Doroti.Framework.Foundation.DebugLibrary.connectedVmServiceUri.ToString(),", StringComparison.Ordinal)
        .ReplaceGeneratedLocalPattern("DartUri.parse(global::Doroti.Framework.Foundation.DebugLibrary.activeDevToolsServerAddress!)", "DartUri.parse(global::Doroti.Framework.Foundation.DebugLibrary.activeDevToolsServerAddress!.ToString())", StringComparison.Ordinal)
        .ReplaceGeneratedLocalPattern("replace(queryParameters: new DartMap<string, object>", "replace(queryParameters: new DartMap<string, string>", StringComparison.Ordinal)
        .ReplaceGeneratedLocalPattern("_getRawElementParentChain(element, numLocalParents: numLocalParents))", "_getRawElementParentChain(element, numLocalParents: numLocalParents).Cast<global::Doroti.Framework.Foundation.Diagnosticable>().ToList())", StringComparison.Ordinal)
        .ReplaceGeneratedLocalPattern("node__66037.getProperties(),", "node__66037.getProperties().ToList(),", StringComparison.Ordinal)
        .ReplaceGeneratedLocalPattern("_filterChildren(node.getChildren(),", "_filterChildren(node.getChildren().ToList(),", StringComparison.Ordinal)
        .ReplaceGeneratedLocalPattern("_renderObjectOrNull(@object)", "_renderObjectOrNull((Element)@object)", StringComparison.Ordinal)
        .ReplaceGeneratedLocalPattern("children: diagnostic__22069.getChildren()", "children: diagnostic__22069.getChildren().ToList()", StringComparison.Ordinal)
        .ReplaceGeneratedLocalPattern("postEvent(eventName, stats.exportToJson(this._frameStart, frameNumber: this._frameNumber))", "postEvent(eventName, stats.exportToJson(this._frameStart, frameNumber: this._frameNumber).cast<object, object>())", StringComparison.Ordinal)
        .ReplaceGeneratedLocalPattern("System.Linq.Enumerable.Any(errorSummary.value)", "!string.IsNullOrEmpty(errorSummary.value?.ToString())", StringComparison.Ordinal)
        .ReplaceGeneratedLocalPattern("!string.IsNullOrEmpty(errorSummary.value)", "!string.IsNullOrEmpty(errorSummary.value?.ToString())", StringComparison.Ordinal)
        .ReplaceGeneratedLocalPattern("errorSummary.value.First()", "errorSummary.value", StringComparison.Ordinal)
        .ReplaceGeneratedLocalPattern("foreach (MapEntry<global::Doroti.Runtime.CreationLocation, long> entry__153282 in Widget_inspectorLibrary._locationToId.entries)", "foreach (var entry__153282 in Widget_inspectorLibrary._locationToId.entries)", StringComparison.Ordinal)
        .ReplaceGeneratedLocalPattern("public virtual global::Doroti.Framework.Foundation.DiagnosticsSerializationDelegate copyWith(long subtreeDepth = default!, bool includeProperties = default!", "public virtual global::Doroti.Framework.Foundation.DiagnosticsSerializationDelegate copyWith(long? subtreeDepth = null, bool? includeProperties = null", StringComparison.Ordinal);
}
