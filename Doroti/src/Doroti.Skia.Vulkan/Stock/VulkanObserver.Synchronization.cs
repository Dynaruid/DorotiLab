using Silk.NET.Vulkan;

namespace Doroti.Skia.Vulkan;

internal sealed unsafe partial class VulkanObserver
{
    public long DepthStencilBarrierStageCorrections { get; private set; }

    private PipelineStageFlags CompleteDepthStencilWriteStages(
        PipelineStageFlags destination, uint imageCount, ImageMemoryBarrier* images)
    {
        // SkiaSharp 4.154.0-preview.1.26454.9 / Skia cc43af052d3d98e605bee4ddc98671dafded1c57:
        // setup_texture_layouts prepares writable depth/stencil attachments for
        // EARLY_FRAGMENT_TESTS only. Subsequent draws can write depth in LATE
        // tests, so the layout transition's destination scope must cover both.
        // Strengthen only this missing scope; keep the native binary, all access
        // masks, layouts, ownership transfers and borrowed barrier arrays intact.
        const PipelineStageFlags alreadyCovered = PipelineStageFlags.LateFragmentTestsBit |
            PipelineStageFlags.AllGraphicsBit | PipelineStageFlags.AllCommandsBit;
        if ((destination & PipelineStageFlags.EarlyFragmentTestsBit) == 0 ||
            (destination & alreadyCovered) != 0)
            return destination;

        for (uint index = 0; index < imageCount; index++)
        {
            ref readonly var barrier = ref images[index];
            if (barrier.NewLayout != ImageLayout.DepthStencilAttachmentOptimal ||
                (barrier.SubresourceRange.AspectMask & (ImageAspectFlags.DepthBit | ImageAspectFlags.StencilBit)) == 0 ||
                (barrier.DstAccessMask & AccessFlags.DepthStencilAttachmentWriteBit) == 0)
                continue;

            DepthStencilBarrierStageCorrections++;
            return destination | PipelineStageFlags.LateFragmentTestsBit;
        }
        return destination;
    }
}
