using Doroti.Skia.Rendering;
using Doroti.Host.SharedWindows;
using Doroti.Ui;

internal static class NativeSceneInspectionRegression
{
    internal static void Run()
    {
        using var ordinary = Retained(24, builder => builder.addTexture(42, width: 10, height: 10));
        using var shield = Retained(24, builder => builder.addInputShield(Rect.fromLTWH(0, 0, 10, 10)));
        using var native = new Scene(0, [new SceneCommand("platformView", null)]);
        using var atLimit = Retained(256, _ => { });
        using var overLimit = Retained(257, _ => { });
        Check(NativeFrameSceneInspection.IsShaderOnly(ordinary.Commands), "retained textures use shader admission");
        Check(!NativeFrameSceneInspection.IsShaderOnly(shield.Commands), "nested input shields require native composition");
        Check(!NativeFrameSceneInspection.IsShaderOnly(native.Commands), "platform views require native composition");
        Check(NativeFrameSceneInspection.IsShaderOnly(atLimit.Commands)
            && !NativeFrameSceneInspection.IsShaderOnly(overLimit.Commands), "over-depth admission stays conservative");
        Check(!WindowsPlatformSceneInspection.HasPlatformCommands(ordinary.Commands)
            && !WindowsPlatformSceneInspection.HasPlatformCommands(atLimit.Commands), "Windows shader-only retained fast path");
        Check(WindowsPlatformSceneInspection.HasPlatformCommands(shield.Commands)
            && WindowsPlatformSceneInspection.HasPlatformCommands(native.Commands), "Windows detects native views and nested input shields");
        try
        {
            WindowsPlatformSceneInspection.HasPlatformCommands(overLimit.Commands);
            throw new InvalidOperationException("Windows retained depth guard did not reject excessive nesting");
        }
        catch (InvalidDataException) { }
        var allocated = GC.GetAllocatedBytesForCurrentThread();
        for (var i = 0; i < 20; i++)
        {
            NativeFrameSceneInspection.IsShaderOnly(ordinary.Commands);
            NativeFrameSceneInspection.IsShaderOnly(shield.Commands);
            WindowsPlatformSceneInspection.HasPlatformCommands(ordinary.Commands);
            WindowsPlatformSceneInspection.HasPlatformCommands(shield.Commands);
        }
        Check(GC.GetAllocatedBytesForCurrentThread() == allocated,
            "repeated retained scene admission creates no per-layer closure or enumerator");
        Console.WriteLine("PASS: renderer/Windows retained native/shield inspection, depth guards and allocation-free repeated inspection");
    }

    private static Scene Retained(int depth, Action<SceneBuilder> emit)
    {
        EngineLayer? child = null;
        for (var i = 0; i < depth; i++)
        {
            var builder = new SceneBuilder();
            var layer = builder.pushOffset(1, 1);
            if (child is null) emit(builder);
            else builder.addRetained(child);
            builder.pop();
            child = layer;
        }
        var root = new SceneBuilder();
        root.addRetained(child!);
        return root.build();
    }

    private static void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
