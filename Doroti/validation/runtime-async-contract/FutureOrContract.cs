using Doroti.Runtime;

internal static class FutureOrContract
{
    internal static async Task Run()
    {
        static void Check(bool condition) { if (!condition) throw new Exception("FutureOr result contract"); }
        var values = new List<int> { 1, 2 };
        Check(ReferenceEquals(await DartAsyncRuntime.AwaitFutureOrValue<IEnumerable<int>>(Future<List<int>>.value(values)), values));
        Check(Equals(await DartAsyncRuntime.AwaitFutureOrValue<object>(Future<int>.value(42)), 42));
        Check(await DartAsyncRuntime.AwaitFutureOrValue<string?>(null) is null);
        Check(await DartAsyncRuntime.AwaitFutureOrValue<object?>(Future.value()) is null);
        var delayed = new TaskCompletionSource<string>();
        var pending = DartAsyncRuntime.AwaitFutureOrValue<object>(Future<string>.fromTask(delayed.Task));
        Check(!pending.IsCompleted);
        delayed.SetResult("ready");
        Check(Equals(await pending, "ready"));
        try { await DartAsyncRuntime.AwaitFutureOrValue<int>("42"); throw new Exception("incompatible direct value accepted"); }
        catch (InvalidCastException) { }
        try { await DartAsyncRuntime.AwaitFutureOrValue<int>(Future<string>.value("42")); throw new Exception("incompatible future result accepted"); }
        catch (InvalidCastException) { }
        Check((await DartAsyncRuntime.wait<object>(new Future[] { Future<int>.value(42), Future<string>.value("text") })).SequenceEqual(new object[] { 42, "text" }));
        try { await DartAsyncRuntime.wait<int>(new Future[] { Future<string>.value("42") }); throw new Exception("wait discarded invalid result"); }
        catch (InvalidCastException) { }
        var original = new FormatException("future failure");
        try { await DartAsyncRuntime.AwaitFutureOrValue<object>(Future<int>.error(original)); throw new Exception("future error swallowed"); }
        catch (FormatException exception) when (ReferenceEquals(exception, original)) { }
        Console.WriteLine("FutureOr: covariant consumer, heterogeneous result, void/null, pending completion, incompatible types and error propagation PASS");
    }
}
