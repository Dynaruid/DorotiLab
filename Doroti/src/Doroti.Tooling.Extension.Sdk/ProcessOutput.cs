using System.Diagnostics;
using System.Text;
using Doroti.Tooling.Contracts;
namespace Doroti.Tooling.Extension.Sdk;
internal static class ProcessOutput
{
    internal static async Task ReadLinesAsync(StreamReader reader, Action<string>? output, CancellationToken token = default)
    {
        var buffer = new char[4096]; var line = new StringBuilder();
        while (await reader.ReadAsync(buffer.AsMemory(), token) is var count && count != 0)
            for (var index = 0; index < count; index++)
            {
                if (buffer[index] == '\n') { output?.Invoke(line.ToString().TrimEnd('\r')); line.Clear(); }
                else { if (line.Length >= 65536) throw new ToolContractException("process-output-too-large", "Process log line exceeds 64 KiB."); line.Append(buffer[index]); }
            }
        if (line.Length != 0) output?.Invoke(line.ToString().TrimEnd('\r'));
    }
    internal static async Task WaitAsync(Process process, IReadOnlyList<Task> readers, CancellationToken token)
    {
        var exit = process.WaitForExitAsync(token);
        var pending = readers.Append(exit).ToList();
        while (pending.Count != 0)
        {
            var completed = await Task.WhenAny(pending);
            await completed; // A reader failure immediately stops waiting on the child.
            pending.Remove(completed);
        }
    }
}
