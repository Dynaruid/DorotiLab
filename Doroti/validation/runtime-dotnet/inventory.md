# Doroti.Runtime migration inventory

Baseline: `94d24bbd` on 2026-09-23. The [member snapshot](api-baseline.txt)
lists 89 public/internal types (including nested types) and their declared
public/internal members. [Lexical usage counts](symbol-usage-baseline.tsv)
cover product, validation, import-tool, template, and Testbed C# files in that
checkout. Counts include comments and unrelated symbols with the same name;
they are impact hints, not compiler references. Global using aliases and
extension calls still require semantic review before final removal.

| Runtime source | Types in baseline | Disposition and migration stage |
| --- | --- | --- |
| `DartAsync.cs` | `Timer`, `DartAsyncRuntime`, `DartMicrotaskQueue`, `Future`, `Future<T>`, `Completer<T>`, both `FutureMethodBuilder` types, `Stream<T>`, `StreamController<T>`, `StreamSubscription<T>` | `TimeProvider`/`ITimer` and host queue (R1–R2); `Task`/`TaskCompletionSource` (R3); explicit push subscription or `IAsyncEnumerable` (R5). Remove legacy types after consumers move. |
| `DartCoreAdapters.cs` | `Duration`, `StringBuffer`, `PriorityQueue<T>`, `HeapPriorityQueue<T>`, `DartRandom`, `DartArgumentError`, `DartCoreExtensions`, `Dart_coreLibrary`, `Characters`, `CharacterRange` | Keep `Duration` as a Flutter API implementation per user decision; use `TimeSpan` at .NET boundaries. Product callers use `StringBuilder`, BCL priority queue, and `Random`. `DartRandom` is removed; `StringBuffer` and priority queues remain temporary import-tool bridges until R6. |
| `DartCollectionsAndConvert.cs` | `Dart_ioLibrary`, `GZipCodec`, `IDartMap`, `DartPatternRuntime`, `DartCollectionRuntime`, `DartMap<TKey,TValue>`, `MapEntry<TKey,TValue>`, `MapEquality<TKey,TValue>`, `DartUri`, `HttpStatus`, `HttpClient`, `HttpClientRequest`, `DartHttpHeaders`, `HttpClientResponse`, `DartMatch`, `Pattern`, `RegExp`, `Dart_convertLibrary` and nested `base64`, `Base64Encoder`, `JsonCodec`, `utf8`, `Utf8Decoder` | Purpose-specific .NET collections, `Uri`, `System.Net.Http`, compression, JSON, encoding, and regex (R4–R6). Preserve ordered/null-key and wire contracts where needed. |
| `DartTypedData.cs` | `ByteBuffer`, `ByteData`, `Uint8List`, `DartTypedList<T>`, `Int32List`, `Int64List`, `Float32List`, `Float64List`, `ReceivePort`, `SendPort`, `Expando<T>` | Numeric typed-list product consumers now use .NET arrays/`IReadOnlyList<T>`; the codec writes numeric array tags and decodes arrays (R4). Byte views and `ByteData` remain for messaging/image paths. Typed messages (R5); `ConditionalWeakTable` for weak attachments (R2). Product weak attachments use `ConditionalWeakTable`; `Expando<T>` remains a temporary import-tool bridge until R6. Numeric Runtime wrappers also remain for optional import-tool output until R6. |
| `DartIo.cs` | `DartFile` | `FileInfo` in image APIs (R2). File removed in current changes. |
| `DartLinkedListAndInvocation.cs` | `Invocation`, `DeepCollectionEquality`, `DartLinkedListEntry<T>`, `DartLinkedList<T>` | Drop unused invocation and use explicit comparers (R6). Widgets linked-list consumers now use `LinkedList<T>` and owner-held nodes; Runtime linked-list types remain a temporary import-tool bridge until R6. |
| `DartMathLibrary.cs` | `Dart_mathLibrary` | Replaced product calls with `System.Math` and removed this Runtime file. The optional compiler emits `System.Math` for the checked fixture; wider output remains to review. |
| `DartDeveloperPorts.cs` | `Flow`, `CreationLocation`, `DartDeveloperTimeline`, `DartTimelineEventKind`, `DartTimelineEvent`, `TimelineTask`, `Timeline`, `Dart_developerLibrary` and nested `Flow`, `Timeline` | `ActivitySource`/diagnostics and concrete source locations; remove Dart timeline facade (R5–R6). |
| `DartErrorHandlers.cs` | `DartErrorHandlers` | Typed Task fault handling and product error sink (R3/R6). |
| `DartRuntimePrimitives.cs` | `AssertionError`, `NoSuchMethodError`, `TypeError`, `DartFutureDiagnostic`, `IDartTweenValue<T>`, `DartRuntimePrimitives` | BCL errors/typed operations; retain only concrete product interpolation and fault diagnostics under product ownership (R3/R6). No global null conversion. |
| `FoundationRuntimePorts.cs` | `IDartEnumIndex`, `FoundationRuntimePorts` | BCL enum/hash/text where equivalent; relocate concrete Foundation operations after call-site review (R6). |
| `MaterialColorSchemeRuntime.cs` | `MaterialColorSchemeRuntime` | Retain product color feature; change only Dart-typed dependencies (R7). |
| `MaterialImageColorRuntime.cs` | `MaterialImageColorRuntime` | Retain product image/color feature; change only Dart-typed dependencies (R7). |
| `MaterialImageQuantizerWu.cs` | `MaterialImageQuantizerWu`, nested `Box`, `CreateBoxesResult`, `Direction`, `MaximizeResult` | Retain product quantizer implementation (R7). |
| `PointerEventContracts.cs` | `IPointerEvent`, `IPointerRemovedEvent` | Retain concrete product pointer contracts; review namespace/ownership with consumers (R7). |

## Initial dependency findings

- `Duration` appears in 177 product C# files outside Runtime, `Future` in 112,
  `DartMap` in 174, and `DartRuntimePrimitives` in 513. The migration must update
  public Framework signatures, hosts, codecs, samples, and the optional compiler.
- `Doroti.Framework.Widgets/GlobalUsings.cs` and
  `Doroti.Framework.Material/GlobalUsings.cs` alias `Timer` to
  `Doroti.Runtime.Timer`; name-only search for `Timer` can undercount that path.
- `IDartEnumIndex` and `IDartTweenValue<T>` have product implementers. Their
  contracts need symbol-level review before removing Runtime declarations.
- `DartToCSharp` emits Runtime names as strings; its own successful build does
  not prove emitted C# compiles against the new packages.

## Executed baseline

At `94d24bbd`, Runtime, Material, and `DartToCSharp` Release builds each passed
with zero warnings/errors. The existing warning-remediation contract runner
passed 103 assertions. All commands used `run-with-timeout.py` (1,200 seconds).

This inventory is an R0 starting record. Roslyn-bound member usage, public API
diff after R3–R6, and target package dependencies remain to verify.
