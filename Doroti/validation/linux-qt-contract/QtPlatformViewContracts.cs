using System.Runtime.InteropServices;
using Doroti.Host.Qt;
using Doroti.Ui;

internal static class QtPlatformViewContracts
{
    internal static void Run()
    {
        if (Marshal.SizeOf<QtPlatformViewHost.Api>() != 64 || Marshal.SizeOf<QtPlatformViewHost.Placement>() != 80)
            throw new Exception("Qt optional attachment ABI layout changed.");
        using var host = new QtPlatformViewHost();
        foreach (var factory in host.Factories)
        {
            if (factory.QuerySupport(new(1, factory.ViewType)).Supported ||
                factory.QuerySupport(new(1, factory.ViewType, PlatformViewComposition.InterleavedComposition)).Supported)
                throw new Exception("Unbound Qt factory advertised native support.");
        }
        var placement = new PlatformViewPlacement(new(1, 1, 1), Rect.fromLTWH(1.5, 2.5, 20, 30),
            PlatformViewTransform.Identity, Rect.fromLTWH(5.2, 6.2, 9.4, 10.4), 0);
        var native = QtPlatformViewHost.Translate(placement, 42);
        if (native.Id != 42 || native.Clip.X != 6 || native.Clip.Y != 7 ||
            native.Clip.Width != 8 || native.Clip.Height != 9)
            throw new Exception("Qt fractional rect clip leaked pixels outside the requested region.");
        var hidden = QtPlatformViewHost.Translate(placement with { Clip = Rect.fromLTWH(100,100,1,1) }, 42);
        if (hidden.Visible != 0) throw new Exception("Empty Qt intersection remained visible.");
        try
        {
            QtPlatformViewHost.Translate(placement with { Transform = new(2, 0, 0, 2, 0, 0) }, 42);
            throw new Exception("Qt affine transform was accepted.");
        }
        catch (NotSupportedException) { }
        Console.WriteLine("Qt PlatformView ABI, deferred support, clip quantization and effect rejection: PASS");
    }
}
