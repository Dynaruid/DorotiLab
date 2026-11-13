using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.InteropServices.JavaScript;
using System.Threading;
using System.Threading.Tasks;

[System.Runtime.Versioning.SupportedOSPlatform("browser")]
public static partial class Probe
{
    public static Task Main(string[] args) => Task.CompletedTask;

    [JSExport]
    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicMethods, "System.Runtime.InteropServices.JavaScript.JSWebWorker", "System.Runtime.InteropServices.JavaScript")]
    public static async Task<int> Start()
    {
        System.Console.WriteLine("probe: before Thread.Start");
        var type = typeof(JSHost).Assembly.GetType("System.Runtime.InteropServices.JavaScript.JSWebWorker")!;
        var method = type.GetMethod("RunAsyncVoid", BindingFlags.Static | BindingFlags.NonPublic)!;
        var result = 0;
        System.Func<Task> body = () => { result = System.Environment.CurrentManagedThreadId; System.Console.WriteLine("probe: owner-enter"); return Task.CompletedTask; };
        var lifetime = (Task)method.Invoke(null, new object[] { body, CancellationToken.None })!;
        System.Console.WriteLine("probe: Thread.Start returned");
        await lifetime;
        return result;
    }
}
