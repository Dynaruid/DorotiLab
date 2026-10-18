using System.Runtime.InteropServices;
using System.Text.Json;
using Doroti.Host.Qt;

try
{
    QtRuntime.Configure();
    var library = NativeLibrary.Load("libQt6Core.so.6");
    var function = Marshal.GetDelegateForFunctionPointer<Probe>(NativeLibrary.GetExport(library, "fixture"));
    Console.WriteLine(JsonSerializer.Serialize(new { value = function(), qpa = Environment.GetEnvironmentVariable("QT_QPA_PLATFORM") }));
    return 0;
}
catch (Exception error)
{
    Console.Error.WriteLine(error.Message);
    return 1;
}

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
internal delegate int Probe();
