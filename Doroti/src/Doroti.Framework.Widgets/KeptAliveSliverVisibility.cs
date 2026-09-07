using Doroti.Framework.Rendering;

namespace Doroti.Framework.Widgets;

internal static class KeptAliveSliverVisibility
{
    // Kept-alive children remain attached but are outside the sliver's layout
    // and paint walk. An OverlayPortal follows its layout anchor, not the
    // theater that owns its overlay render object.
    internal static bool IsHidden(RenderObject? renderObject)
    {
        for (var current = renderObject; current is not null;)
        {
            if (current.parentData is KeepAliveParentDataMixin { keptAlive: true }) return true;
            current = current is _RenderDeferredLayoutBox__overlay portal
                ? portal._layoutSurrogate : current.parent;
        }
        return false;
    }
}
