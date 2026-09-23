# Runtime .NET migration: current API changes

This is the source migration guide for the changes implemented so far. The
broader R3–R8 migration remains in progress; the package has not been released
with these changes. Rebuild all consumers against a future package version.

| Previous API | Current C# usage |
| --- | --- |
| `new FileImage(new DartFile(path))` | `new FileImage(new FileInfo(path))` |
| `Image.CreateFile(new DartFile(path))` | `Image.CreateFile(new FileInfo(path))` |
| Override `InlineSpan.computeToPlainText(StringBuffer buffer, ...)` | Override with `System.Text.StringBuilder buffer` and use `Append`/`AppendLine` |
| `new StringBuffer()` | Product code uses `new StringBuilder()` with `Append`, `AppendLine`, and `Append(char.ConvertFromUtf32(codePoint))` |
| `Expando<T>` for weak object attachments | Product code uses `ConditionalWeakTable<object,T>` with `TryGetValue`, `Add`, and `Remove` |
| Runtime `PriorityQueue<T>` / `HeapPriorityQueue<T>` | The scheduler uses `System.Collections.Generic.PriorityQueue<TElement,TPriority>` with an explicit priority comparer |
| Runtime `DartLinkedList<T>` / `DartLinkedListEntry<T>` | Widgets use `LinkedList<T>` and `LinkedListNode<T>` held by the owner |
| `Matrix4.storage` as `Float64List` | `double[]`; the returned array remains mutable and `clone()` copies it |
| `Canvas.drawRawPoints` / `drawRawAtlas` typed-list parameters | `IReadOnlyList<float>` coordinates and `IReadOnlyList<int>?` colors |
| Numeric `Int32List` / `Int64List` / `Float32List` / `Float64List` codec values | Pass `int[]` / `long[]` / `float[]` / `double[]`; each retains its typed-list wire tag |
| `new DartRandom(seed).nextDouble()` | `DorotiRandom.FromSeed(seed).NextDouble()`; unseeded callers may use `Random.Shared` |
| `Dart_mathLibrary` methods/constants | `System.Math` (`Pow`, `Sin`, `PI`, `E`, and so on) |

**`Duration` remains the Flutter API implementation** in `Doroti.Runtime` and
Framework signatures. Its `TimeSpan` boundary conversion remains available;
microseconds beyond the representable `TimeSpan` range now throw
`OverflowException` instead of wrapping ticks.
This follows the user's 2026-09-23 correction to the original plan.

`DorotiExecutionContext` now owns scoped time providers and callback-dispatcher
captures. A captured `DorotiCallbackDispatcher.PostAsync` faults on callback
errors and cancels when its view closes. Await that Task if the caller needs to
observe completion. The former `DartAsyncRuntime` entry points remain as a
temporary R3 bridge while `Future` consumers are migrated.

The API surface snapshot at `94d24bbd` is [api-baseline.txt](api-baseline.txt).
The current Runtime build removed three baseline types and added
`DorotiExecutionContext`, `DorotiCallbackDispatcher`, and `DorotiRandom`.
`StringBuffer`, `Expando<T>`, priority queues, and linked-list types remain as
temporary import-tool bridges with an R6 removal target. Product code no
longer calls them.
Remaining `Future`, collection, typed-data, I/O, and language helpers are not
claimed as migrated.
