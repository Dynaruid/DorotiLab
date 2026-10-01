using System.Reflection;
using System.Runtime.InteropServices;
using Doroti.Skia.Rendering;
using Doroti.Skia.RuntimeEffects;
using Doroti.Testing;
using Doroti.Ui;
using SkiaSharp;

// Opt-in production-renderer pixel checks on an actual macOS Metal GPU.
// Private entry points isolate filter coordinates/composition from UI layout.
// Readbacks and completion waits exist only in this validation command.
internal static class VariableBlurGpuRegression
{
    public static void Run()
    {
        if (!OperatingSystem.IsMacOS())
            throw new PlatformNotSupportedException("Variable Blur GPU validation requires macOS Metal.");
        var device = Native.MTLCreateSystemDefaultDevice();
        var queue = Native.Send(device, Native.sel_registerName("newCommandQueue"));
        using var metalObjects = new MetalObjects(device, queue);
        Console.WriteLine($"Graphite/Metal available={SKGraphiteContext.IsBackendAvailable(SKGraphiteBackend.Metal)}");
        using var backend = new SKGraphiteMtlBackendContext { MtlDevice = device, MtlQueue = queue };
        using var context = SKGraphiteContext.CreateMetal(backend, new SKGraphiteContextOptions { GpuBudgetInBytes = 256*1024*1024 });
        using var recorder = context!.CreateRecorder(64*1024*1024, (r, img, mip) => img);
        using var tester = new WidgetTester();
        var renderer = (SkiaSceneRenderer)typeof(WidgetTester).GetField("_renderer", BindingFlags.NonPublic|BindingFlags.Instance)!.GetValue(tester)!;
        typeof(SkiaSceneRenderer).GetField("_runtimeEffectBackend", BindingFlags.NonPublic|BindingFlags.Instance)!.SetValue(renderer, DorotiSkiaRuntimeEffects.NativeGraphiteMetalBackend);
        var settingsType = typeof(ImageFilter).Assembly.GetType("Doroti.Ui.VariableBlurSettings")!;
        var pool = typeof(DorotiSkiaRuntimeEffects).Assembly.GetType("Doroti.Skia.RuntimeEffects.DorotiSkiaImageFilterRenderer")!;
        var begin = pool.GetMethod("BeginFrame", BindingFlags.Static|BindingFlags.NonPublic)!;
        var owner = typeof(SkiaSceneRenderer).GetField("_runtimeEffectContextOwner", BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(renderer);
        var runtimeBackend = typeof(SkiaSceneRenderer).GetProperty("RuntimeEffectBackend", BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(renderer);
        object Call(string method, params object?[] args) => typeof(SkiaSceneRenderer).GetMethod(method, BindingFlags.NonPublic|BindingFlags.Instance)!.Invoke(renderer,args)!;
        SKSurface Surface(int w, int h) => SkiaGpuSurfaces.Register(SKSurface.Create(recorder!,new SKImageInfo(w,h))!,recorder!);
        byte[] Read(SKSurface surface, int width, int height)
        {
            using var recording = recorder!.Snap();
            if(context.InsertRecording(recording!)!=SKGraphiteInsertStatus.Success) throw new Exception("Insert failed");
            byte[]? pixels=null;
            context.RequestReadPixels(surface,new SKImageInfo(width,height),new SKRectI(0,0,width,height),SKImageRescaleGamma.Src,SKImageRescaleMode.Nearest,r=>pixels=r!.ToArray(0));
            if(!context.Submit(new SKGraphiteSubmitInfo { Sync=true })) throw new Exception("Submit failed");
            context.CheckAsyncWorkCompletion();
            SkiaGpuSurfaces.CompleteRecording(recorder!,false);
            return pixels ?? throw new Exception("Readback failed");
        }
        const int width=385,height=1536;
        using var source=Surface(width,height);
        source.Canvas.Clear(new SKColor(30,40,60,128));
        using(var p=new SKPaint())
        for(var y=0;y<height;y+=3) for(var x=0;x<width;x+=3)
        {
            p.Color=new SKColor((byte)(x*37+y*11),(byte)(x*13+y*29),(byte)(x*7+y*43),(byte)(80+(x+y)%176));
            source.Canvas.DrawRect(x,y,3,3,p);
        }
        foreach(var kernel in new[]{VariableBlurKernel.gaussian,VariableBlurKernel.fastGaussian,VariableBlurKernel.dualKawase})
        foreach(var reverse in new[]{false,true})
        foreach(var dpr in new[]{1d,2d,3d})
        foreach(var tile in new[]{TileMode.clamp,TileMode.decal})
        foreach(var constantSigma in new[]{false,true})
        {
            if(kernel==VariableBlurKernel.dualKawase && tile!=TileMode.clamp) continue;
            var firstSigma=constantSigma?20d:reverse?0d:20d;
            var lastSigma=constantSigma?20d:reverse?20d:0d;
            var visible=new SKRect(4.25f,615.25f,380.75f,725.75f);
            var settings=Activator.CreateInstance(settingsType,BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic,null,
                new object[]{new Offset(0,615/dpr),new Offset(0,726/dpr),firstSigma,lastSigma,32,.25,true,kernel},null)!;
            var globalMatrix=SKMatrix.CreateScale((float)dpr,(float)dpr);
            var capture=(SKRectI)typeof(SkiaSceneRenderer).GetMethods(BindingFlags.Static|BindingFlags.NonPublic)
                .Single(m=>m.Name=="VariableBlurCaptureBounds"&&m.GetParameters().Length==6)
                .Invoke(null,new object[]{visible,settings,tile,globalMatrix,width,height})!;
            byte[] Render(bool crop,bool direct, bool fullStages = false, bool fullStorage = false)
            {
                typeof(SkiaSceneRenderer).GetField("_boundedKawaseStages", BindingFlags.NonPublic|BindingFlags.Instance)!
                    .SetValue(renderer, !fullStages);
                typeof(SkiaSceneRenderer).GetField("_croppedKawaseDetail", BindingFlags.NonPublic|BindingFlags.Instance)!
                    .SetValue(renderer, !fullStorage);
                typeof(SkiaSceneRenderer).GetField("_croppedAdaptiveBands", BindingFlags.NonPublic|BindingFlags.Instance)!
                    .SetValue(renderer, !fullStorage);
                begin.Invoke(null,new[]{runtimeBackend,0L,owner});
                var rect=crop?capture:new SKRectI(0,0,width,height);
                using var input=source.Snapshot(rect);
                var localVisible=visible;
                localVisible.Offset(-rect.Left,-rect.Top);
                var matrix=SKMatrix.Concat(SKMatrix.CreateTranslation(-rect.Left,-rect.Top),globalMatrix);
                using var result=Surface(width,height);
                result.Canvas.Clear(SKColors.Transparent);
                if(direct && kernel == VariableBlurKernel.dualKawase)
                {
                    using var shader=(SKShader)Call("CreateDualKawaseVariableBlurShader",result.Canvas,input,settings,rect.Width,rect.Height,matrix,localVisible,dpr,dpr==1?5:6);
                    using var translated=shader.WithLocalMatrix(SKMatrix.CreateTranslation(rect.Left,rect.Top));
                    using var p=new SKPaint {Shader=translated,BlendMode=SKBlendMode.Src};
                    result.Canvas.DrawRect(rect,p);
                }
                else if(direct)
                {
                    matrix.TryInvert(out var inv);
                    using var shader=(SKShader)Call("CreateAdaptiveVariableBlurShader",result.Canvas,input,settings,tile,rect.Width,rect.Height,matrix,inv,localVisible);
                    using var translated=shader.WithLocalMatrix(SKMatrix.CreateTranslation(rect.Left,rect.Top));
                    using var p=new SKPaint {Shader=translated,BlendMode=SKBlendMode.Src};
                    result.Canvas.DrawRect(rect,p);
                }
                else
                {
                    using var filtered=(SKImage)Call("ApplyVariableBlur",result.Canvas,input,settings,tile,rect.Width,rect.Height,matrix,localVisible,false);
                    result.Canvas.DrawImage(filtered,rect,new SKSamplingOptions(SKFilterMode.Linear));
                }
                return Read(result,width,height);
            }
            var full=Render(false,false);
            var cropped=Render(true,false);
            var fullStorageReference=Render(true,false,false,true);
            var storageMax=0;
            for(var y=(int)Math.Ceiling(visible.Top);y<Math.Floor(visible.Bottom);y++)
            for(var x=(int)Math.Ceiling(visible.Left);x<Math.Floor(visible.Right);x++)
            for(var c=0;c<4;c++)storageMax=Math.Max(storageMax,Math.Abs(fullStorageReference[(y*width+x)*4+c]-cropped[(y*width+x)*4+c]));
            Console.WriteLine($"{kernel} cropped/full working storage maxError={storageMax}/255");
            if(storageMax>3)throw new Exception("Working storage shifted sampling coordinates");
            var maxError=0;
            for(var y=(int)Math.Ceiling(visible.Top);y<Math.Floor(visible.Bottom);y++)
            for(var x=(int)Math.Ceiling(visible.Left);x<Math.Floor(visible.Right);x++)
            for(var c=0;c<4;c++) maxError=Math.Max(maxError,Math.Abs(full[(y*width+x)*4+c]-cropped[(y*width+x)*4+c]));
            Console.WriteLine($"{kernel} reverse={reverse} DPR={dpr} tile={tile} constant={constantSigma} capture={capture} crop/full maxError={maxError}/255");
            if(maxError>3) throw new Exception("GPU crop/full mismatch");
            {
                var direct=Render(true,true);maxError=0;
                for(var y=(int)Math.Ceiling(visible.Top);y<Math.Floor(visible.Bottom);y++)
                for(var x=(int)Math.Ceiling(visible.Left);x<Math.Floor(visible.Right);x++)
                for(var c=0;c<4;c++) maxError=Math.Max(maxError,Math.Abs(direct[(y*width+x)*4+c]-cropped[(y*width+x)*4+c]));
                Console.WriteLine($"{kernel} direct/intermediate maxError={maxError}/255");
                if(maxError>3) throw new Exception("GPU direct composition mismatch");
            }
            if(kernel==VariableBlurKernel.dualKawase)
            {
                var fullStages = Render(true, false, true);
                var stageMax = 0;
                for(var y=(int)Math.Ceiling(visible.Top);y<Math.Floor(visible.Bottom);y++)
                for(var x=(int)Math.Ceiling(visible.Left);x<Math.Floor(visible.Right);x++)
                for(var c=0;c<4;c++) stageMax=Math.Max(stageMax,Math.Abs(fullStages[(y*width+x)*4+c]-cropped[(y*width+x)*4+c]));
                Console.WriteLine($"Kawase bounded/full reconstruction maxError={stageMax}/255");
                if(stageMax>3)throw new Exception("Kawase halo/reconstruction mismatch");
                var gaussianSettings=Activator.CreateInstance(settingsType,BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic,null,
                    new object[]{new Offset(0,615/dpr),new Offset(0,726/dpr),firstSigma,lastSigma,32,1d,false,VariableBlurKernel.gaussian},null)!;
                begin.Invoke(null,new[]{runtimeBackend,0L,owner});
                using var gaussianResult=Surface(width,height);
                using var input=source.Snapshot();
                using var filtered=(SKImage)Call("ApplyVariableBlur",gaussianResult.Canvas,input,gaussianSettings,tile,width,height,globalMatrix,visible,false);
                gaussianResult.Canvas.DrawImage(filtered,0,0,new SKSamplingOptions(SKFilterMode.Linear));
                var weakReference=Read(gaussianResult,width,height);
                var weakMax=0;
                for(var y=(int)Math.Ceiling(visible.Top);y<Math.Floor(visible.Bottom);y++)
                {
                    var sigma=constantSigma?20*dpr:20*dpr*Math.Clamp(reverse?(y+.5-615)/111:(726-y-.5)/111,0,1);
                    if(sigma>1.8)continue;
                    for(var x=(int)Math.Ceiling(visible.Left);x<Math.Floor(visible.Right);x++)
                    for(var c=0;c<4;c++) weakMax=Math.Max(weakMax,Math.Abs(full[(y*width+x)*4+c]-weakReference[(y*width+x)*4+c]));
                }
                Console.WriteLine($"Kawase/Gaussian weak-end maxError={weakMax}/255");
                if(weakMax>3)throw new Exception("Kawase changed weak detail");
            }
        }
        // Native sibling scopes must leave shader ancestors owned. Compare a
        // mixed scene containing ordinary opacity/blur and a nested VariableBlur.
        var drawing = new List<PathCommand>();
        var drawingCanvas = new Doroti.Ui.Canvas(drawing);
        drawingCanvas.drawRect(new Rect(30,580,350,760),new Doroti.Ui.Paint { color=new Color(0x99ee8844) });
        var picture = new SceneCommand("picture", null) { HostPayload=new ScenePicturePayload(
            991,new Offset(0,0),drawing,null,false,true) };
        SceneCommand Op(string operation, object payload) => new(operation,null) {HostPayload=payload};
        var ordinary = ImageFilterSnapshot.Capture(new ImageFilter(1.5,1.5));
        var variableFilter = ImageFilterSnapshot.Capture(ImageFilter.variableBlur(new(0,600),new(0,740),
            8,0,bounds:new Rect(10,600,380,740),resolutionScale:.25));
        var commands = new List<SceneCommand>
        {
            Op("opacity",new SceneOpacityPayload(.45,new(12,18))),picture,new("pop",null),
            Op("imageFilter",new SceneImageFilterPayload(ordinary,new(0,0),null)),picture,new("pop",null),
            Op("opacity",new SceneOpacityPayload(.7,new(0,0))),picture,
            Op("backdropFilter",new SceneBackdropFilterPayload(variableFilter,BlendMode.srcOver,null)),
            new("pop",null),new("pop",null),
        };
        byte[] Scene(bool native)
        {
            typeof(SkiaSceneRenderer).GetField("_nativeVariableBlurSubtrees",BindingFlags.Instance|BindingFlags.NonPublic)!
                .SetValue(renderer,native);
            begin.Invoke(null,new[]{runtimeBackend,0L,owner});
            using var result=Surface(width,height);
            result.Canvas.Clear(new SKColor(40,80,120,160));
            renderer.DrawPlatformRasterSegment(result.Canvas,commands,width,height);
            return Read(result,width,height);
        }
        var ownedScene=Scene(false);
        var nativeScene=Scene(true);
        var sceneMax=ownedScene.Zip(nativeScene).Max(p=>Math.Abs(p.First-p.Second));
        Console.WriteLine($"Native/owned sibling scopes maxError={sceneMax}/255");
        if(sceneMax>3)throw new Exception("Native sibling routing changed nested shader composition");
        Console.WriteLine("PASS: production renderer Graphite-Metal crop/full and direct/intermediate pixels on macOS GPU.");
    }

    private sealed class MetalObjects(nint device, nint queue) : IDisposable
    {
        public void Dispose()
        {
            Native.Send(queue, Native.sel_registerName("release"));
            Native.Send(device, Native.sel_registerName("release"));
        }
    }

    private static class Native
    {
        [DllImport("/System/Library/Frameworks/Metal.framework/Metal")] public static extern nint MTLCreateSystemDefaultDevice();
        [DllImport("/usr/lib/libobjc.A.dylib")] public static extern nint sel_registerName(string name);
        [DllImport("/usr/lib/libobjc.A.dylib",EntryPoint="objc_msgSend")] public static extern nint Send(nint obj,nint selector);
    }
}
