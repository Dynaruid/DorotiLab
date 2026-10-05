using Doroti.Tooling.Contracts;
using Doroti.Tooling.Extension.Sdk;
namespace Doroti.Tooling;
internal static class RepositoryCommands
{
    internal static async Task<int?> TryRunAsync(string[] args)
    {
        if (args.Length == 0 || args[0] is not ("validate" or "audit" or "release")) return null;
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !Directory.Exists(Path.Combine(directory.FullName, "Doroti", "eng"))) directory = directory.Parent;
        if (directory is null) { Console.Error.WriteLine("repository-not-found: maintenance commands require the source checkout."); return 1; }
        if (!File.Exists(Path.Combine(directory.FullName, "Doroti", "eng", "validate.py")))
        { Console.Error.WriteLine("missing-validation-runner: " + directory.FullName); return 1; }
        var suite = args[0] == "release" ? "Release" : args[0] == "audit" ? "Source" : "Developer";
        for (var i = 1; i < args.Length; i++)
        {
            if (!args[i].Equals("-ValidationSuite", StringComparison.OrdinalIgnoreCase) || i + 1 == args.Length) { Console.Error.WriteLine("invalid-argument: " + args[i]); return 1; }
            suite = args[++i];
        }
        using var cancel = new CancellationTokenSource();
        ConsoleCancelEventHandler handler = (_, e) => { e.Cancel = true; cancel.Cancel(); };
        Console.CancelKeyPress += handler;
        try
        {
            await using var session = new ExecutionSession();
            return await session.ExecuteAsync(new("repository-validation", [new("python", [Path.Combine(directory.FullName, "Doroti", "eng", "run-with-timeout.py"), "--timeout", "1200", "python", Path.Combine(directory.FullName, "Doroti", "eng", "validate.py"), suite], directory.FullName, [])]), Console.WriteLine, cancel.Token);
        }
        catch (OperationCanceledException) { return 130; }
        finally { Console.CancelKeyPress -= handler; }
    }
}
