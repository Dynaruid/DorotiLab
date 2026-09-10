if (System.Runtime.CompilerServices.RuntimeFeature.IsDynamicCodeSupported)
    throw new InvalidOperationException("Run the published NativeAOT executable to validate this contract.");
LayoutCallbackContract.Run();
