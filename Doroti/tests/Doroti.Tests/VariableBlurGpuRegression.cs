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
    public static void Run(string? qualityOutputDirectory = null)
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
        // Exercise the production capture pool on a GPU: adaptive stage order
        // changes, simultaneous snapshots, budget eviction and owner isolation.
        var poolOwner = new object();
        const string poolBackend = "filter-pool-regression";
        var rent = pool.GetMethod("RentSceneSurface", BindingFlags.Static | BindingFlags.NonPublic)!;
        var configure = pool.GetMethod("ConfigureSurfaceBudget", BindingFlags.Static | BindingFlags.NonPublic)!;
        var memory = pool.GetMethod("CaptureSurfaceMemory", BindingFlags.Static | BindingFlags.NonPublic)!;
        var release = pool.GetMethod("ReleaseContext", BindingFlags.Static | BindingFlags.NonPublic)!;
        using (var poolTarget = Surface(64, 64))
        {
            configure.Invoke(null, [poolBackend, 0L, poolOwner, 16384L]);
            void BeginPoolFrame() => begin.Invoke(null, [poolBackend, 0L, poolOwner]);
            (IDisposable Lease, SKCanvas Canvas, bool Temporary) Rent(int w, int h, object? otherOwner = null)
            {
                var lease = rent.Invoke(null, [poolTarget.Canvas, poolBackend, 0L, w, h,
                    otherOwner ?? poolOwner, null])!;
                var type = lease.GetType();
                return ((IDisposable)lease,
                    (SKCanvas)type.GetProperty("Canvas", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(lease)!,
                    (bool)type.GetProperty("IsTemporary", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(lease)!);
            }
            (int Entries, long Pixels, long Limit) Memory() =>
                ((int, long, long))memory.Invoke(null, [poolBackend, 0L, poolOwner])!;
            try
            {
                BeginPoolFrame();
                var first = Rent(48, 48);
                var smallHandle = first.Canvas.Surface!.Handle;
                first.Canvas.Clear(SKColors.Red);
                using var red = first.Canvas.Surface.Snapshot();
                first.Lease.Dispose();
                var second = Rent(64, 64);
                var largeHandle = second.Canvas.Surface!.Handle;
                second.Lease.Dispose();
                var third = Rent(48, 48);
                if (third.Canvas.Surface!.Handle == smallHandle)
                    throw new Exception("A filter pool surface was reused twice in one frame.");
                third.Canvas.Clear(SKColors.Blue);
                using var blue = third.Canvas.Surface.Snapshot();
                third.Lease.Dispose();
                poolTarget.Canvas.Clear(SKColors.Transparent);
                poolTarget.Canvas.DrawImage(red, new SKRect(0, 0, 32, 64), SKSamplingOptions.Default);
                poolTarget.Canvas.DrawImage(blue, new SKRect(32, 0, 64, 64), SKSamplingOptions.Default);
                var pixels = Read(poolTarget, 64, 64);
                var redChannel = SKImageInfo.PlatformColorType == SKColorType.Rgba8888 ? 0 : 2;
                var blueChannel = 2 - redChannel;
                if (pixels[redChannel] != 255 || pixels[blueChannel] != 0
                    || pixels[32 * 4 + redChannel] != 0 || pixels[32 * 4 + blueChannel] != 255)
                    throw new Exception("A later filter capture overwrote an earlier snapshot.");
                BeginPoolFrame();
                var swappedLarge = Rent(64, 64);
                var swappedSmall = Rent(48, 48);
                if (swappedLarge.Canvas.Surface!.Handle != largeHandle || swappedSmall.Canvas.Surface!.Handle != smallHandle)
                    throw new Exception("Changing adaptive pass order failed to reuse exact-sized captures.");
                swappedSmall.Lease.Dispose(); swappedLarge.Lease.Dispose();
                BeginPoolFrame();
                var big = Rent(100, 100);
                var temporary = Rent(100, 100);
                if (big.Temporary || !temporary.Temporary || Memory().Pixels > Memory().Limit)
                    throw new Exception("Filter pool budget eviction or live-frame preservation failed.");
                temporary.Lease.Dispose(); big.Lease.Dispose();
                var independentOwner = new object();
                var independent = Rent(100, 100, independentOwner);
                if (independent.Canvas.Surface!.Handle == big.Canvas.Surface!.Handle)
                    throw new Exception("Filter captures escaped their context owner.");
                independent.Lease.Dispose();
                release.Invoke(null, [poolBackend, 0L, independentOwner]);
                recorder!.Snap()?.Dispose();
                SkiaGpuSurfaces.CompleteRecording(recorder, discarded: true);
            }
            finally { release.Invoke(null, [poolBackend, 0L, poolOwner]); }
            if (Memory().Entries != 0 || Memory().Pixels != 0)
                throw new Exception("Filter pool retained resources after context release.");
        }
        Console.WriteLine("PASS: GPU filter surface reuse, snapshot lifetime, mobile budget and owner release.");
        // Separate, opt-in still-image review. Never enable readbacks while
        // collecting device presentation performance. These are not pixel-
        // equivalence assertions between Full Gaussian and Fixed.
        if (qualityOutputDirectory is { } outputDirectory)
        {
            Directory.CreateDirectory(outputDirectory);
            const int qualityWidth = 1170, qualityHeight = 1080;
            using var scene = Surface(qualityWidth, qualityHeight);
            scene.Canvas.Clear(new SKColor(245, 245, 245));
            using var decodedPhoto = SKImage.FromEncodedData(
                "reference/flutter_sample_app/assets/images/mae-mu-9002s2VnOAY-unsplash.webp");
            using var photo = decodedPhoto.ToTextureImage(recorder!)
                ?? throw new Exception("Quality scene photograph upload failed.");
            using var typeface = SKTypeface.FromFamilyName("Helvetica");
            using var font = new SKFont(typeface, 27);
            using var paint = new SKPaint { Color = SKColors.Black, IsAntialias = true };
            for (var y = 0; y < qualityHeight; y += 90)
            {
                scene.Canvas.DrawText("Small text 0123456789 AaBb", 12, y + 35, SKTextAlign.Left, font, paint);
                paint.IsAntialias = false;
                for (var x = 440; x < 650; x += 4)
                    scene.Canvas.DrawRect(x, y + 5, 1, 65, paint);
                for (var yy = y + 5; yy < y + 70; yy += 2)
                for (var x = 680; x < 810; x += 2)
                    scene.Canvas.DrawRect(x + (yy % 4 == 0 ? 0 : 1), yy, 1, 1, paint);
                scene.Canvas.DrawImage(photo, new SKRect(840, y, qualityWidth, y + 90),
                    new SKSamplingOptions(SKFilterMode.Linear));
                paint.IsAntialias = true;
            }
            void SavePixels(byte[] pixels, string name)
            {
                using var bitmap = new SKBitmap(new SKImageInfo(qualityWidth, qualityHeight));
                Marshal.Copy(pixels, 0, bitmap.GetPixels(), pixels.Length);
                using var image = SKImage.FromBitmap(bitmap);
                using var data = image.Encode(SKEncodedImageFormat.Png, 100);
                using var file = File.Create(System.IO.Path.Combine(outputDirectory, name));
                data.SaveTo(file);
            }
            foreach (var sigma in new[] { 0d, 1d, 2d, 4d, 8d, 20d, 32d })
            foreach (var fixedResolution in new[] { false, true })
            {
                begin.Invoke(null, new[] { runtimeBackend, 0L, owner });
                using var input = scene.Snapshot();
                using var result = Surface(qualityWidth, qualityHeight);
                result.Canvas.DrawImage(input, 0, 0, SKSamplingOptions.Default);
                // The sample disables its BackdropFilter at exactly zero.
                if (sigma > 0)
                {
                    var settings = Activator.CreateInstance(settingsType,
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null,
                        new object[] { new Offset(0, 0), new Offset(0, 180), sigma, 0d,
                            32, fixedResolution ? .25 : 1d, false, VariableBlurKernel.gaussian }, null)!;
                    var visible = new SKRect(0, 0, qualityWidth, 540);
                    using var filtered = (SKImage)Call("ApplyVariableBlur", result.Canvas, input,
                        settings, TileMode.clamp, qualityWidth, qualityHeight,
                        SKMatrix.CreateScale(3, 3), visible, fixedResolution);
                    result.Canvas.ClipRect(visible);
                    result.Canvas.DrawImage(filtered, SKRect.Create(qualityWidth, qualityHeight),
                        new SKSamplingOptions(SKFilterMode.Linear));
                }
                SavePixels(Read(result, qualityWidth, qualityHeight),
                    $"{(fixedResolution ? "fixed" : "full")}-sigma{sigma}.png");
            }
            Console.WriteLine($"Saved 14 Full/Fixed stills at DPR 3 to {outputDirectory}; manual quality review, not equivalence or motion validation.");
            return;
        }
        // Fixed must exercise ApplyVariableBlur's reduced output and the same
        // final linear sampling as the empty-child BackdropFilter. An Adaptive
        // shader is not a reference for this path.
        var fixedCases = 0;
        var fixedMax = 0;
        foreach (var (w, h) in new[] { (385, 1536), (1536, 385), (385, 1537) })
        {
            using var fixedSource = Surface(w, h);
            fixedSource.Canvas.Clear(SKColors.Transparent);
            using (var paint = new SKPaint())
            for (var y = 0; y < h; y += 3)
            for (var x = 0; x < w; x += 3)
            {
                paint.Color = new SKColor((byte)(x * 37 + y * 11),
                    (byte)(x * 13 + y * 29), (byte)(x * 7 + y * 43),
                    (byte)(80 + (x + y) % 176));
                fixedSource.Canvas.DrawRect(x, y, 3, 3, paint);
            }
            foreach (var reverse in new[] { false, true })
            foreach (var dpr in new[] { 1d, 2d, 3d })
            foreach (var tile in new[] { TileMode.clamp, TileMode.decal })
            foreach (var edge in new[] { false, true })
            {
                var vertical = h > w;
                var start = edge ? 0 : 615;
                var end = start + 111;
                var visible = vertical
                    ? new SKRect(.25f, start + .25f, w - .25f, end - .25f)
                    : new SKRect(start + .25f, .25f, end - .25f, h - .25f);
                var settings = Activator.CreateInstance(settingsType,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null,
                    new object[] { vertical ? new Offset(0, start / dpr) : new Offset(start / dpr, 0),
                        vertical ? new Offset(0, end / dpr) : new Offset(end / dpr, 0),
                        reverse ? 0d : 20d, reverse ? 20d : 0d,
                        32, .25, false, VariableBlurKernel.gaussian }, null)!;
                var globalMatrix = SKMatrix.CreateScale((float)dpr, (float)dpr);
                var capture = (SKRectI)typeof(SkiaSceneRenderer)
                    .GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
                    .Single(m => m.Name == "VariableBlurCaptureBounds" && m.GetParameters().Length == 6)
                    .Invoke(null, new object[] { visible, settings, tile, globalMatrix, w, h })!;
                if ((w % 4 == 0 || h % 4 == 0) && capture == new SKRectI(0, 0, w, h))
                    throw new Exception("Fixed GPU comparison did not exercise partial capture.");
                byte[] RenderFixed(bool crop, bool direct, int alignment = 1, bool fullResolution = false)
                {
                    var renderSettings = (VariableBlurSettings)settings with
                    { ResolutionScale = fullResolution ? 1 : .25 };
                    begin.Invoke(null, new[] { runtimeBackend, 0L, owner });
                    var alignedCapture = (SKRectI)typeof(SkiaSceneRenderer)
                        .GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
                        .Single(m => m.Name == "VariableBlurCaptureBounds" && m.GetParameters().Length == 8)
                        .Invoke(null, [visible, renderSettings, tile, globalMatrix, w, h, null, alignment])!;
                    var rect = crop ? alignedCapture : new SKRectI(0, 0, w, h);
                    using var input = fixedSource.Snapshot(rect);
                    var localVisible = visible;
                    localVisible.Offset(-rect.Left, -rect.Top);
                    var matrix = SKMatrix.Concat(SKMatrix.CreateTranslation(-rect.Left, -rect.Top), globalMatrix);
                    using var result = Surface(w, h);
                    result.Canvas.Clear(SKColors.Transparent);
                    using var filtered = (SKImage)Call("ApplyVariableBlur", result.Canvas,
                        input, renderSettings, tile, rect.Width, rect.Height, matrix, localVisible, direct);
                    var expectedWidth = direct ? (int)Math.Ceiling(rect.Width * renderSettings.ResolutionScale) : rect.Width;
                    var expectedHeight = direct ? (int)Math.Ceiling(rect.Height * renderSettings.ResolutionScale) : rect.Height;
                    if (filtered.Width != expectedWidth || filtered.Height != expectedHeight)
                        throw new Exception("Fixed did not return the requested working/restored resolution.");
                    // Apply the fractional ROI clip once, at composition, and
                    // compare its partially covered edge pixels as well.
                    result.Canvas.ClipRect(visible, SKClipOperation.Intersect, true);
                    result.Canvas.DrawImage(filtered, rect, new SKSamplingOptions(SKFilterMode.Linear));
                    return Read(result, w, h);
                }
                int Difference(byte[] a, byte[] b)
                {
                    var maximum = 0;
                    for (var y = (int)Math.Floor(visible.Top); y < Math.Ceiling(visible.Bottom); y++)
                    for (var x = (int)Math.Floor(visible.Left); x < Math.Ceiling(visible.Right); x++)
                    for (var c = 0; c < 4; c++)
                        maximum = Math.Max(maximum, Math.Abs(a[(y * w + x) * 4 + c] - b[(y * w + x) * 4 + c]));
                    return maximum;
                }
                var fullDirect = RenderFixed(false, true);
                var cropDirect = RenderFixed(true, true);
                var fullRestored = RenderFixed(false, false);
                var cropRestored = RenderFixed(true, false);
                var directDomain = Difference(fullDirect, cropDirect);
                var restoredDomain = Difference(fullRestored, cropRestored);
                var fullComposition = Difference(fullDirect, fullRestored);
                var cropComposition = Difference(cropDirect, cropRestored);
                var alignedFixed = Difference(cropDirect, RenderFixed(true, true, alignment: 32));
                var alignedFull = Difference(RenderFixed(true, true, fullResolution: true),
                    RenderFixed(true, true, alignment: 32, fullResolution: true));
                var maximum = new[] { directDomain, restoredDomain, fullComposition, cropComposition,
                    alignedFixed, alignedFull }.Max();
                Console.WriteLine($"Fixed Gaussian {w}x{h} reverse={reverse} DPR={dpr} tile={tile} edge={edge} " +
                    $"capture={capture} direct crop/full={directDomain}/255 restored crop/full={restoredDomain}/255 " +
                    $"direct/restored full={fullComposition}/255 crop={cropComposition}/255 " +
                    $"aligned fixed/full={alignedFixed}/{alignedFull}/255");
                if (maximum > 3)
                    throw new Exception("Fixed GPU capture/composition mismatch (limit 3/255).");
                fixedMax = Math.Max(fixedMax, maximum);
                fixedCases++;
            }
        }
        Console.WriteLine($"PASS: Fixed Gaussian actual reduced-output GPU path ({fixedCases} cases, max channel error {fixedMax}/255).");
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
            byte[] Render(bool crop,bool direct, bool fullStages = false, bool fullStorage = false, int alignment = 1)
            {
                typeof(SkiaSceneRenderer).GetField("_boundedKawaseStages", BindingFlags.NonPublic|BindingFlags.Instance)!
                    .SetValue(renderer, !fullStages);
                typeof(SkiaSceneRenderer).GetField("_croppedKawaseDetail", BindingFlags.NonPublic|BindingFlags.Instance)!
                    .SetValue(renderer, !fullStorage);
                typeof(SkiaSceneRenderer).GetField("_croppedAdaptiveBands", BindingFlags.NonPublic|BindingFlags.Instance)!
                    .SetValue(renderer, !fullStorage);
                typeof(SkiaSceneRenderer).GetField("_filterCaptureAlignment", BindingFlags.NonPublic|BindingFlags.Instance)!
                    .SetValue(renderer, alignment);
                begin.Invoke(null,new[]{runtimeBackend,0L,owner});
                var alignedCapture = (SKRectI)typeof(SkiaSceneRenderer).GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
                    .Single(m => m.Name == "VariableBlurCaptureBounds" && m.GetParameters().Length == 8)
                    .Invoke(null, [visible, settings, tile, globalMatrix, width, height, null, alignment])!;
                var rect=crop?alignedCapture:new SKRectI(0,0,width,height);
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
                var aligned = Render(true, true, alignment: 32);
                var alignmentMax = 0;
                for(var y=(int)Math.Ceiling(visible.Top);y<Math.Floor(visible.Bottom);y++)
                for(var x=(int)Math.Ceiling(visible.Left);x<Math.Floor(visible.Right);x++)
                for(var c=0;c<4;c++) alignmentMax=Math.Max(alignmentMax,Math.Abs(aligned[(y*width+x)*4+c]-direct[(y*width+x)*4+c]));
                Console.WriteLine($"{kernel} mobile aligned capture maxError={alignmentMax}/255");
                if(alignmentMax>3) throw new Exception("Mobile allocation alignment changed blur pixels");
                typeof(SkiaSceneRenderer).GetField("_filterCaptureAlignment", BindingFlags.NonPublic | BindingFlags.Instance)!
                    .SetValue(renderer, 1);
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
        // Preserve two async recordings while the same renderer/recorder and
        // filter caches record the next scene. Compare with serial readbacks.
        foreach (var scale in new[] { 1d, .25 })
        {
            IReadOnlyList<SceneCommand> PipelineScene(int index)
            {
                var paths = new List<PathCommand>();
                var canvas = new Doroti.Ui.Canvas(paths);
                for (var x = 0; x < 80; x += 5)
                    canvas.drawRect(new Rect(x, 0, x + 5, 60), new Doroti.Ui.Paint
                    { color = new Color(index == 0 ? (x % 10 == 0 ? 0xffff0000 : 0xff0000ff)
                        : (x % 10 == 0 ? 0xff00ff00 : 0xffffff00)) });
                return new SceneCommand[]
                {
                    Op("picture", new ScenePicturePayload(8000 + index, new(0, 0), paths, null, false, true)),
                    Op("backdropFilter", new SceneBackdropFilterPayload(
                        ImageFilterSnapshot.Capture(ImageFilter.variableBlur(new(0, 0), new(0, 60),
                            8, 0, resolutionScale: scale)), BlendMode.srcOver, null)), new("pop", null),
                };
            }
            byte[] Serial(int index)
            {
                begin.Invoke(null, new[] { runtimeBackend, 0L, owner });
                using var output = Surface(80, 60);
                output.Canvas.Clear(SKColors.Transparent);
                renderer.DrawPlatformRasterSegment(output.Canvas, PipelineScene(index), 80, 60);
                return Read(output, 80, 60);
            }
            var reference = new[] { Serial(0), Serial(1) };
            var outputs = new[] { Surface(80, 60), Surface(80, 60) };
            var recordings = new List<SKGraphiteRecording>();
            byte[]?[] pixels = new byte[2][];
            try
            {
                for (var index = 0; index < 2; index++)
                {
                    begin.Invoke(null, new[] { runtimeBackend, 0L, owner });
                    outputs[index].Canvas.Clear(SKColors.Transparent);
                    renderer.DrawPlatformRasterSegment(outputs[index].Canvas, PipelineScene(index), 80, 60);
                    var recording = recorder!.Snap() ?? throw new Exception("Pipeline snap failed.");
                    recordings.Add(recording);
                    if (context.InsertRecording(recording) != SKGraphiteInsertStatus.Success)
                        throw new Exception("Pipeline insert failed.");
                    var capturedIndex = index;
                    context.RequestReadPixels(outputs[index], new SKImageInfo(80, 60), new SKRectI(0, 0, 80, 60),
                        SKImageRescaleGamma.Src, SKImageRescaleMode.Nearest, r => pixels[capturedIndex] = r!.ToArray(0));
                    if (!context.Submit(new SKGraphiteSubmitInfo { Sync = false })) throw new Exception("Pipeline submit failed.");
                    SkiaGpuSurfaces.CompleteRecording(recorder, false);
                }
                if (!context.Submit(new SKGraphiteSubmitInfo { Sync = true })) throw new Exception("Pipeline final fence failed.");
                context.CheckAsyncWorkCompletion();
                for (var index = 0; index < 2; index++)
                {
                    if (pixels[index] is not { } actual) throw new Exception("Pipeline readback missing.");
                    var max = actual.Zip(reference[index]).Max(p => Math.Abs(p.First - p.Second));
                    if (max > 3) throw new Exception($"Pipeline cache/snapshot reuse changed frame {index}: {max}/255.");
                    Console.WriteLine($"Async retained recordings scale={scale} frame={index} maxError={max}/255");
                }
            }
            finally
            {
                foreach (var recording in recordings) recording.Dispose();
                foreach (var output in outputs) output.Dispose();
            }
        }
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
