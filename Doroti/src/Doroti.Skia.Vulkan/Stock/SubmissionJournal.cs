using Silk.NET.Vulkan;

namespace Doroti.Skia.Vulkan;

// Scheduled Vulkan state, not a GPU or presentation completion receipt.
// Object identities protect against reused numerical handles in old command buffers.
internal sealed class SubmissionJournal
{
    internal sealed class ImageState(ulong handle, long epoch, ImageLayout layout, uint family)
    {
        public readonly ulong Handle = handle;
        public readonly long Epoch = epoch;
        public ImageLayout Layout = layout;
        public uint Family = family;
        public bool Alive = true;
    }
    internal sealed class Commands(nint handle, ulong pool)
    {
        public readonly nint Handle = handle;
        public readonly ulong Pool = pool;
        public int Revision;
        public bool Alive = true, Executable;
        public readonly List<Operation> Operations = [];
    }
    internal abstract record Operation;
    internal sealed record Transition(ImageState Image, ImageLayout Old, ImageLayout New, uint SourceFamily, uint DestinationFamily) : Operation;
    internal sealed record Execute(Commands Child, int Revision) : Operation;
    internal readonly Dictionary<nint, Commands> Buffers = [];
    internal readonly Dictionary<ulong, ImageState> Images = [];
    private long _epoch;
    public long SuccessfulSubmissions { get; private set; }
    public long FailedSubmissions { get; private set; }
    public long AppliedTransitions { get; private set; }
    public int PeakOperations { get; private set; }

    public ImageState Register(ulong image, ImageLayout layout, uint family)
    {
        if (Images.ContainsKey(image)) throw new InvalidOperationException("Image already registered.");
        var state = new ImageState(image, ++_epoch, layout, family); Images.Add(image, state); return state;
    }
    public void RemoveImage(ulong image)
    {
        if (Images.Remove(image, out var state)) state.Alive = false;
    }
    public void Allocate(nint handle, ulong pool)
    {
        if (Buffers.ContainsKey(handle)) throw new InvalidOperationException("Live command handle reused.");
        Buffers.Add(handle, new(handle, pool));
    }
    public void Begin(nint handle)
    {
        var b = Buffers[handle]; b.Revision++; b.Executable = false; b.Operations.Clear();
    }
    public void End(nint handle) => Buffers[handle].Executable = true;
    public void Free(nint handle)
    {
        if (Buffers.Remove(handle, out var b)) { b.Alive = false; b.Operations.Clear(); }
    }
    public void ResetPool(ulong pool) { foreach (var b in Buffers.Values.Where(b => b.Pool == pool)) Begin(b.Handle); }
    public void FreePool(ulong pool) { foreach (var b in Buffers.Values.Where(b => b.Pool == pool).ToArray()) Free(b.Handle); }
    public void Barrier(nint command, ulong image, ImageLayout oldLayout, ImageLayout newLayout, uint source, uint destination)
    {
        if (!Images.TryGetValue(image, out var state)) return; // Only registered host targets are exported.
        Add(command, new Transition(state, oldLayout, newLayout, source, destination));
    }
    public void Secondary(nint command, nint child)
    {
        var c = Buffers[child];
        if (!c.Executable) throw new InvalidOperationException("Secondary is not executable.");
        Add(command, new Execute(c, c.Revision));
    }
    private void Add(nint command, Operation operation)
    {
        var b = Buffers[command];
        if (b.Executable) throw new InvalidOperationException("Recording after EndCommandBuffer.");
        if (b.Operations.Count >= 65536) throw new NotSupportedException("Command journal limit exceeded.");
        b.Operations.Add(operation); PeakOperations = Math.Max(PeakOperations, b.Operations.Count);
    }
    // Test seam takes a real API result in the adapter. Unit tests may supply failure
    // here; no native result is fabricated or returned by this state machine.
    public void Submit(IReadOnlyList<nint> submitted, Result result)
    {
        if (result != Result.Success) { FailedSubmissions++; return; }
        var proposed = new Dictionary<ImageState, (ImageLayout Layout, uint Family)>();
        long transitions = 0;
        var active = new HashSet<Commands>();
        void Replay(Commands commands, int revision)
        {
            if (!commands.Alive || !commands.Executable || commands.Revision != revision || !active.Add(commands))
                throw new InvalidOperationException("Stale, reset, cyclic or non-executable command journal.");
            foreach (var op in commands.Operations)
            {
                if (op is Execute execute) { Replay(execute.Child, execute.Revision); continue; }
                var t = (Transition)op;
                if (!t.Image.Alive || !Images.TryGetValue(t.Image.Handle, out var live) || !ReferenceEquals(live, t.Image))
                    throw new InvalidOperationException("Stale image generation in submitted command.");
                var current = proposed.GetValueOrDefault(t.Image, (t.Image.Layout, t.Image.Family));
                if (t.Old != ImageLayout.Undefined && current.Layout != t.Old)
                    throw new InvalidOperationException($"Image {t.Image.Handle:x}/{t.Image.Epoch}: expected {t.Old}, scheduled {current.Layout}.");
                if ((t.SourceFamily == Vk.QueueFamilyIgnored) != (t.DestinationFamily == Vk.QueueFamilyIgnored))
                    throw new NotSupportedException("Partial queue ownership descriptor.");
                if (t.SourceFamily != Vk.QueueFamilyIgnored && (t.SourceFamily != current.Family || t.DestinationFamily != current.Family))
                    throw new NotSupportedException("Host Graphite target must stay in its single queue family.");
                proposed[t.Image] = (t.New, current.Family); transitions++;
            }
            active.Remove(commands);
        }
        foreach (var handle in submitted) { var b = Buffers[handle]; Replay(b, b.Revision); }
        // Commit only after the complete successful submission has been interpreted.
        foreach (var (image, state) in proposed) { image.Layout = state.Layout; image.Family = state.Family; }
        SuccessfulSubmissions++; AppliedTransitions += transitions;
    }
}
