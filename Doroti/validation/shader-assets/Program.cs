using Doroti.Framework.Material;
using Doroti.Framework.Widgets;
using Doroti.Skia.Rendering;
using Doroti.Ui;

FrameworkShaderLoader.RegisterResourceOwner(typeof(Widget).Assembly);
FrameworkShaderLoader.RegisterResourceOwner(typeof(InkSparkle).Assembly);
FrameworkShaderLoader.RegisterResourceOwner(typeof(SkiaSceneRenderer).Assembly);
foreach (var asset in FrameworkShaderManifest.Assets)
{
    // Loading verifies actual packaged bytes and uniform/sampler ABI.
    var first = await FrameworkShaderLoader.LoadProgram(asset.Id).asTask();
    var second = await FrameworkShaderLoader.LoadProgram(asset.Id).asTask();
    if (!ReferenceEquals(first, second))
        throw new Exception($"Cache miss: {asset.Id}");
    var completion = new TaskCompletionSource<FragmentProgram>(
        TaskCreationOptions.RunContinuationsAsynchronously
    );
    FrameworkShaderLoader.BeginLoad(
        asset.Id,
        p => completion.SetResult(p),
        e => completion.SetException(e)
    );
    if (!ReferenceEquals(first, await completion.Task.WaitAsync(TimeSpan.FromSeconds(10))))
        throw new Exception($"Callback did not share the cache: {asset.Id}");
    Console.WriteLine($"PASS {asset.Id}: embedded bytes, hash, ABI, cache, callback");
}
var failure = new TaskCompletionSource<Exception>(
    TaskCreationOptions.RunContinuationsAsynchronously
);
FrameworkShaderLoader.BeginLoad(
    "missing.asset",
    _ => failure.SetException(new Exception("Unexpected asset")),
    e => failure.SetResult(e)
);
if (await failure.Task.WaitAsync(TimeSpan.FromSeconds(10)) is not KeyNotFoundException)
    throw new Exception("Unknown asset lost its original error");
if (
    !FrameworkShaderLoader.Diagnostics.Any(d =>
        d.AssetId == "missing.asset" && d.Code == "DOROTI_SHADER_ASSET_LOAD_FAILED"
    )
)
    throw new Exception("Unknown asset was not diagnosed");
Console.WriteLine("PASS missing asset: error callback and diagnostic");
