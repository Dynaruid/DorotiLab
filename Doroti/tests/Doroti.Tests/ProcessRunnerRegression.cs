using System.Diagnostics;
using Doroti.Tooling;

internal static class ProcessRunnerRegression
{
    static (string File, string[] Arguments) Command(string mode)
    {
        var file = Environment.ProcessPath!;
        string[] arguments = Path.GetFileNameWithoutExtension(file).Equals("dotnet", StringComparison.OrdinalIgnoreCase)
            ? [typeof(ProcessRunnerRegression).Assembly.Location, "--full-review-process-child", mode]
            : ["--full-review-process-child", mode];
        return (file, arguments);
    }
    public static void Child(string mode)
    {
        if (mode == "pipes")
        {
            Console.Error.Write(new string('e', 1024 * 1024));
            Console.Out.Write(new string('o', 1024 * 1024));
            Console.Error.Write("ERR"); Console.Out.Write("OUT"); return;
        }
        if (mode == "error") { Console.Error.Write("expected error"); Environment.Exit(7); }
        if (mode == "tree")
        {
            var command = Command("sleep"); var info = new ProcessStartInfo(command.File) { UseShellExecute = false, CreateNoWindow = true };
            foreach (var argument in command.Arguments) info.ArgumentList.Add(argument);
            using var grandchild = Process.Start(info)!;
            File.WriteAllText(Environment.GetEnvironmentVariable("DOROTI_PROCESS_RECEIPT")!, grandchild.Id.ToString());
        }
        Thread.Sleep(Timeout.Infinite);
    }
    public static void Run()
    {
        var command = Command("pipes");
        var result = ProcessRunner.Run(command.File, command.Arguments, Environment.CurrentDirectory, timeout: TimeSpan.FromSeconds(10));
        if (result.ExitCode != 0 || !result.StandardOutput.EndsWith("OUT") || !result.StandardError.EndsWith("ERR")) throw new Exception("Concurrent pipe drain lost output.");
        command = Command("error"); result = ProcessRunner.Run(command.File, command.Arguments, Environment.CurrentDirectory);
        if (result.ExitCode != 7 || result.StandardError != "expected error") throw new Exception("Process failure result changed.");
        command = Command("sleep");
        using var canceled = new CancellationTokenSource(TimeSpan.FromMilliseconds(300));
        try { ProcessRunner.Run(command.File, command.Arguments, Environment.CurrentDirectory, cancellationToken: canceled.Token); throw new Exception("Cancellation ignored."); }
        catch (OperationCanceledException) { }
        var directory = Path.Combine(Environment.CurrentDirectory, "temp/testing/full-review/process", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory); var receipt = Path.Combine(directory, "grandchild.txt");
        command = Command("tree");
        try { ProcessRunner.Run(command.File, command.Arguments, Environment.CurrentDirectory,
            new Dictionary<string,string?> { ["DOROTI_PROCESS_RECEIPT"] = receipt }, TimeSpan.FromSeconds(2)); throw new Exception("Timeout ignored."); }
        catch (TimeoutException) { }
        if (!File.Exists(receipt)) throw new Exception("Grandchild fixture did not start.");
        try { using var child = Process.GetProcessById(int.Parse(File.ReadAllText(receipt))); if (!child.HasExited) throw new Exception("Owned grandchild survived timeout."); }
        catch (ArgumentException) { }
        Console.WriteLine("PASS: concurrent 1 MiB stderr/stdout, nonzero exit, cancellation, timeout and owned process-tree cleanup.");
    }
}
