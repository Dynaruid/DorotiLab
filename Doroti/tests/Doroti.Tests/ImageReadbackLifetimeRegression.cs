using Doroti.Runtime;
using Doroti.Ui;

internal static class ImageReadbackLifetimeRegression
{
    public static void Run()
    {
        foreach (var failure in new[] { false, true })
        {
            var storage = new Storage(); var handle = new Handle(storage);
            var image = new Image(1, 1, 1, handle.Release) { HostHandle = handle };
            var reading = image.toByteData().asTask(); image.Dispose(); image.Dispose();
            if (storage.References != 1 || storage.Releases != 0) throw new Exception("Readback lease did not pin disposed image storage.");
            if (failure) storage.Bytes.SetException(new InvalidOperationException("readback"));
            else storage.Bytes.SetResult(new ByteData(new Uint8List(new byte[] { 1, 2, 3, 4 })));
            try { reading.WaitAsync(TimeSpan.FromSeconds(5)).GetAwaiter().GetResult(); if (failure) throw new Exception("Readback failure lost."); }
            catch (InvalidOperationException error) when (failure && error.Message == "readback") { }
            if (storage.Releases != 1 || storage.References != 0) throw new Exception("Readback terminal did not release storage once.");
        }
        Console.WriteLine("PASS: asynchronous image readback pins storage across original disposal and releases once after success/error.");
    }
    sealed class Storage
    {
        public int References = 1, Releases;
        public TaskCompletionSource<ByteData> Bytes = new(TaskCreationOptions.RunContinuationsAsynchronously);
    }
    sealed class Handle(Storage storage) : IDorotiImageHandle
    {
        public object StorageIdentity => storage;
        public IDorotiImageHandle Clone() { Interlocked.Increment(ref storage.References); return new Handle(storage); }
        public ValueTask<ByteData> ReadBytesAsync(ImageByteFormat format) => new(storage.Bytes.Task);
        public void Release() { if (Interlocked.Decrement(ref storage.References) == 0) Interlocked.Increment(ref storage.Releases); }
    }
}
