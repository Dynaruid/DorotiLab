using Doroti.Framework.Material;
using Doroti.Framework.Widgets;
using Doroti.Ui;
namespace MaterialSample;
internal sealed class PlatformEffectSample : StatelessWidget
{
    public override Widget build(BuildContext context)
    {
        var owner = View.of(context);
        var live = false;
        if (owner.registeredCapabilityIds.Contains(DorotiCapabilityIds.PlatformViews))
        {
            var native = owner.RequireCapability<IPlatformViewHostCapability>(DorotiCapabilityIds.PlatformViews,
                DorotiUiInvocation.Managed("effect sample preflight"));
            live = native.QuerySupport(new(0, "doroti/webview", PlatformViewComposition.InterleavedComposition)).NativeBackdropBlur;
        }
        // The fallback is explicit and bypasses live capture/allocation altogether.
        var style = new PlatformEffectStyle(Tint: live ? 0x88eeeeff : 0xffeeeeff,
            Match: live ? PlatformEffectMatchPolicy.MatchCommon : PlatformEffectMatchPolicy.SolidTint);
        return new Scaffold(appBar: new AppBar(title: new Text("Platform effect support")),
            body: new Center(child: new SizedBox(width: 320, height: 180,
                child: new PlatformEffect(style, new Text(live ? "Common material intent; exact Gaussian support is a separate query."
                    : "Solid tint selected: this owner cannot sample live native content.")))));
    }
}
