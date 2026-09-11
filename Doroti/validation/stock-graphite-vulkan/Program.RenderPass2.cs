using Silk.NET.Vulkan;
using VkDevice = Silk.NET.Vulkan.Device;

internal static unsafe partial class Program
{
    // Independently authored pass: clear attachment in subpass 0, read-only input
    // in subpass 1, finish GENERAL. The next copy validates that real Vulkan and
    // the journal agree on this deliberately non-color-attachment final layout.
    private static void ProbeRenderPass2(Vk vk, VkDevice device, Queue queue, VulkanObserver observer, CopySlot slot)
    {
        ImageView view = default;
        RenderPass pass = default;
        Framebuffer framebuffer = default;
        bool submitted = false;
        try
        {
            var vi = new ImageViewCreateInfo { SType = StructureType.ImageViewCreateInfo, Image = slot.Output,
                ViewType = ImageViewType.Type2D, Format = Format.R8G8B8A8Unorm, SubresourceRange = new(ImageAspectFlags.ColorBit, 0, 1, 0, 1) };
            Check(observer.Call<VulkanObserver.CreateImageViewDelegate>("vkCreateImageView")(device, &vi, null, &view), "RP2 view");
            var attachment = new AttachmentDescription2 { SType = StructureType.AttachmentDescription2, Format = Format.R8G8B8A8Unorm,
                Samples = SampleCountFlags.Count1Bit, LoadOp = AttachmentLoadOp.Clear, StoreOp = AttachmentStoreOp.Store,
                StencilLoadOp = AttachmentLoadOp.DontCare, StencilStoreOp = AttachmentStoreOp.DontCare,
                InitialLayout = ImageLayout.Undefined, FinalLayout = ImageLayout.General };
            var color = new AttachmentReference2 { SType = StructureType.AttachmentReference2, Attachment = 0, Layout = ImageLayout.ColorAttachmentOptimal, AspectMask = ImageAspectFlags.ColorBit };
            var input = new AttachmentReference2 { SType = StructureType.AttachmentReference2, Attachment = 0, Layout = ImageLayout.ShaderReadOnlyOptimal, AspectMask = ImageAspectFlags.ColorBit };
            var subpasses = stackalloc SubpassDescription2[2];
            subpasses[0] = new() { SType = StructureType.SubpassDescription2, PipelineBindPoint = PipelineBindPoint.Graphics, ColorAttachmentCount = 1, PColorAttachments = &color };
            subpasses[1] = new() { SType = StructureType.SubpassDescription2, PipelineBindPoint = PipelineBindPoint.Graphics, InputAttachmentCount = 1, PInputAttachments = &input };
            var dependency = new SubpassDependency2 { SType = StructureType.SubpassDependency2, SrcSubpass = 0, DstSubpass = 1,
                SrcStageMask = PipelineStageFlags.ColorAttachmentOutputBit, DstStageMask = PipelineStageFlags.FragmentShaderBit | PipelineStageFlags.ColorAttachmentOutputBit,
                SrcAccessMask = AccessFlags.ColorAttachmentWriteBit, DstAccessMask = AccessFlags.InputAttachmentReadBit | AccessFlags.ColorAttachmentWriteBit, DependencyFlags = DependencyFlags.ByRegionBit };
            var pi = new RenderPassCreateInfo2 { SType = StructureType.RenderPassCreateInfo2, AttachmentCount = 1, PAttachments = &attachment,
                SubpassCount = 2, PSubpasses = subpasses, DependencyCount = 1, PDependencies = &dependency };
            Check(observer.Call<VulkanObserver.CreateRenderPass2Delegate>("vkCreateRenderPass2KHR")(device, &pi, null, &pass), "RP2 create");
            var fi = new FramebufferCreateInfo { SType = StructureType.FramebufferCreateInfo, RenderPass = pass, AttachmentCount = 1,
                PAttachments = &view, Width = 128, Height = 128, Layers = 1 };
            Check(observer.Call<VulkanObserver.CreateFramebufferDelegate>("vkCreateFramebuffer")(device, &fi, null, &framebuffer), "RP2 framebuffer");
            var bi = new CommandBufferBeginInfo { SType = StructureType.CommandBufferBeginInfo, Flags = CommandBufferUsageFlags.OneTimeSubmitBit };
            Check(observer.Call<VulkanObserver.BeginCommandBufferDelegate>("vkBeginCommandBuffer")(slot.Command, &bi), "RP2 begin command");
            var clear = new ClearValue { Color = new ClearColorValue(0f, 0f, 1f, 1f) };
            var begin = new RenderPassBeginInfo { SType = StructureType.RenderPassBeginInfo, RenderPass = pass, Framebuffer = framebuffer,
                RenderArea = new(new(0, 0), new(128, 128)), ClearValueCount = 1, PClearValues = &clear };
            var subBegin = new SubpassBeginInfo { SType = StructureType.SubpassBeginInfo, Contents = SubpassContents.Inline };
            var subEnd = new SubpassEndInfo { SType = StructureType.SubpassEndInfo };
            observer.Call<VulkanObserver.CmdBeginRenderPass2Delegate>("vkCmdBeginRenderPass2KHR")(slot.Command, &begin, &subBegin);
            observer.Call<VulkanObserver.CmdNextSubpass2Delegate>("vkCmdNextSubpass2KHR")(slot.Command, &subBegin, &subEnd);
            observer.Call<VulkanObserver.CmdEndRenderPass2Delegate>("vkCmdEndRenderPass2KHR")(slot.Command, &subEnd);
            Check(observer.Call<VulkanObserver.EndCommandBufferDelegate>("vkEndCommandBuffer")(slot.Command), "RP2 end command");
            observer.Check();
            if (observer.State(slot.Output.Handle).Layout != ImageLayout.Undefined) throw new InvalidOperationException("RP2 record committed before submit.");
            var cbi = new CommandBufferSubmitInfo { SType = StructureType.CommandBufferSubmitInfo, CommandBuffer = slot.Command, DeviceMask = 1 };
            var si = new SubmitInfo2 { SType = StructureType.SubmitInfo2, CommandBufferInfoCount = 1, PCommandBufferInfos = &cbi };
            Check(observer.Call<VulkanObserver.QueueSubmit2Delegate>("vkQueueSubmit2KHR")(queue, 1, &si, slot.Fence), "RP2 submit2"); submitted = true;
            observer.Check();
            if (observer.State(slot.Output.Handle).Layout != ImageLayout.General) throw new InvalidOperationException("RP2 implicit final state disagrees.");
            var start = System.Diagnostics.Stopwatch.StartNew();
            while (vk.GetFenceStatus(device, slot.Fence) == Result.NotReady && start.Elapsed.TotalSeconds < 5) Thread.Sleep(1);
            Check(vk.GetFenceStatus(device, slot.Fence), "RP2 completion");
            submitted = false;
        }
        finally
        {
            if (submitted) Check(vk.DeviceWaitIdle(device), "RP2 exceptional diagnostic drain");
            if (framebuffer.Handle != 0) observer.Call<VulkanObserver.DestroyFramebufferDelegate>("vkDestroyFramebuffer")(device, framebuffer, null);
            if (pass.Handle != 0) observer.Call<VulkanObserver.DestroyRenderPassDelegate>("vkDestroyRenderPass")(device, pass, null);
            if (view.Handle != 0) observer.Call<VulkanObserver.DestroyImageViewDelegate>("vkDestroyImageView")(device, view, null);
        }
    }
}
