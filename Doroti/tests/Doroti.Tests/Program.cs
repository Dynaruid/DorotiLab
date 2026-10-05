using Doroti.Framework.Widgets;
using Doroti.Testing;
using Doroti.Ui;

#if DOROTI_REPO_TESTS
if (args.FirstOrDefault() == "--full-review-process-child") { ProcessRunnerRegression.Child(args[1]); return; }
FullReviewRegression.Run();
ImageReadbackLifetimeRegression.Run();
ProcessRunnerRegression.Run();
if (Array.IndexOf(args, "--native-frame-metal-gpu") is var metalGpuIndex && metalGpuIndex >= 0)
{
    if (metalGpuIndex + 1 >= args.Length || args[metalGpuIndex + 1].StartsWith("--"))
        throw new ArgumentException("--native-frame-metal-gpu requires an output JSON path.");
    NativeFrameMetalGpuRegression.Run(args[metalGpuIndex + 1]);
    return;
}
if (Array.IndexOf(args, "--native-frame-gpu") is var nativeGpuIndex && nativeGpuIndex >= 0)
{
    NativeFrameGpuRegression.Run(args[nativeGpuIndex + 1]);
    return;
}
if (args.Contains("--shader-frame-pipeline"))
{
    ShaderFramePipelineRegression.Run();
    TextureBudgetRegression.Run();
    return;
}
if (args.Contains("--variable-blur-gpu"))
{
    VariableBlurGpuRegression.Run();
    return;
}
if (Array.IndexOf(args, "--variable-blur-quality") is var qualityIndex && qualityIndex >= 0)
{
    if (qualityIndex + 1 >= args.Length || args[qualityIndex + 1].StartsWith("--"))
        throw new ArgumentException("--variable-blur-quality requires an output directory.");
    VariableBlurGpuRegression.Run(args[qualityIndex + 1]);
    return;
}
if (args.Contains("--variable-blur-kawase"))
{
    VariableBlurKawaseRegression.Run();
    return;
}
if (args.Contains("--variable-blur-capture"))
{
    VariableBlurCaptureRegression.Run();
    return;
}
if (args.Contains("--variable-blur-kernel"))
{
    VariableBlurKernelRegression.Run();
    return;
}
#endif
await DesktopCloseRegression.Run();
PlatformPolicyRegression.Run();
#if DOROTI_REPO_TESTS
VariableBlurCaptureRegression.Run();
VariableBlurKernelRegression.Run();
VariableBlurKawaseRegression.Run();
ShaderFramePipelineRegression.Run();
TextureBudgetRegression.Run();
NavigationRegression.Run();
await PlatformRetirementRegression.Run();
#endif
