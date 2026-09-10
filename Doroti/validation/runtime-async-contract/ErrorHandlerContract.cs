using System.Diagnostics;
using Doroti.Runtime;

internal static class ErrorHandlerContract
{
    public static async Task Run()
    {
        var original = new InvalidOperationException("original");
        Require(await Future<int>.error(original).catchError((Func<object, int>)(_ => 7)) == 7);
        Require(await Future<int>.error(original).catchError((Func<Exception, StackTrace, int>)((e, stack) =>
        {
            Require(ReferenceEquals(e, original) && stack is not null);
            return 8;
        })) == 8);
        Require(await Future<int>.error(original).catchError((Func<Exception, Future<int>>)(_ => Future<int>.value(9))) == 9);
        var delayed = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
        var recovery = Future<int>.error(original).catchError((Func<Exception, Task<int>>)(_ => delayed.Task));
        Require(!recovery.asTask().IsCompleted);
        delayed.SetResult(10);
        Require(await recovery == 10);
        Require(await Future<int>.error(original).catchError(
            DartErrorHandlers.Adapt<InvalidOperationException, int>(e => e.Message.Length)) == 8);
        Require(await Future<int>.error(original).catchError(
            DartErrorHandlers.AdaptTask<InvalidOperationException, int>(_ => Task.FromResult(11))) == 11);
        Require(await Future<int>.error(original).then<int>((Func<int, Future<int>>)(n => Future<int>.value(n)),
            (Func<Exception, int>)(_ => 12)) == 12);
        var actions = 0;
        await Future.error(original).catchError((Action<object, StackTrace?>)((_, _) => actions++));
        Require(actions == 1);
        Require(await Future<int>.error(original).catchError((Action<Exception>)(_ => actions++)) == 0);
        Require(actions == 2); // Documented void-handler recovery for a typed Future.
        try
        {
            await Future<int>.error(original).catchError((Func<Exception, int>)(_ => 1), _ => false);
            throw new Exception("Rejected error filter recovered.");
        }
        catch (InvalidOperationException error) { Require(ReferenceEquals(error, original)); }
        var callbackError = new ApplicationException("callback");
        try
        {
            await Future<int>.error(original).catchError((Func<Exception, int>)(_ => throw callbackError));
            throw new Exception("Handler exception was lost.");
        }
        catch (ApplicationException error) { Require(ReferenceEquals(error, callbackError)); }
        try
        {
            await Future<int>.error(original).catchError((Func<Exception, string>)(_ => "wrong result"));
            throw new Exception("Invalid recovery type was silently converted.");
        }
        catch (InvalidCastException) { }
        var values = new List<int> { 1, 2, 3 };
        values.removeWhere((Delegate)(Predicate<int>)(n => n == 2));
        Require(values.SequenceEqual([1, 3]));
        values.removeWhere((Delegate)(Func<object, bool>)(value => (int)value == 3));
        Require(values.SequenceEqual([1]));
        Require(FoundationRuntimePorts.EnumIndex(new ExternalIndex()) == 42);
        Require(FoundationRuntimePorts.EnumIndex(DayOfWeek.Wednesday) == 3);
        Require(FoundationRuntimePorts.EnumIndexNullable(null) is null);
        var primary = MaterialColorSchemeRuntime.GetArgb(0xff6750a4, false, "tonalSpot", 0, "primary");
        Require(primary != 0 && primary == MaterialColorSchemeRuntime.GetArgb(0xff6750a4, false, "tonalSpot", 0, "primary"));
        Console.WriteLine("Error callback/value/MaterialColorUtilities contracts: PASS");
    }

    private static void Require(bool value)
    {
        if (!value) throw new InvalidOperationException("Error callback contract failed.");
    }

    private sealed class ExternalIndex : IDartEnumIndex
    {
        public long DartEnumIndex => 42;
    }
}
