using System.Runtime.InteropServices;
using Silk.NET.Vulkan;
using VkDevice = Silk.NET.Vulkan.Device;

namespace Doroti.Skia.Vulkan;

internal sealed unsafe partial class VulkanObserver(Vk vk, Instance instance, VkDevice device, Queue queue, uint family,
    bool renderPass2ExtensionEnabled = false) : IDisposable
{
    public readonly SubmissionJournal Journal = new();
    private readonly Dictionary<string, Delegate> Roots = [];
    private readonly Dictionary<ulong, ImageCreateInfo> Images = [];
    private sealed record View(ulong Image, ImageSubresourceRange Range);
    private sealed record Attachment(ImageLayout Initial, ImageLayout Final);
    private sealed record Pass(Attachment[] Attachments, Dictionary<uint, ImageLayout>[] Subpasses);
    private sealed record Framebuffer(ulong[] Views);
    private sealed class PassInstance(Pass pass, View[] views)
    {
        public readonly Pass Pass = pass;
        public readonly View[] Views = views;
        public readonly ImageLayout[] Layouts = pass.Attachments.Select(a => a.Initial).ToArray();
        public readonly HashSet<uint> Used = [];
        public int Subpass;
    }
    private readonly Dictionary<ulong, View> Views = [];
    private readonly Dictionary<ulong, Pass> Passes = [];
    private readonly Dictionary<ulong, Framebuffer> Framebuffers = [];
    private readonly Dictionary<nint, PassInstance> ActivePasses = [];
    public string? Fault { get; private set; }
    public long ExplicitBarriers { get; private set; }
    public long ImplicitTransitions { get; private set; }
    public long ResolveReferences { get; private set; }
    public long NextSubpasses { get; private set; }
    public int PeakMetadata { get; private set; }
    public int MetadataCount => Images.Count + Views.Count + Passes.Count + Framebuffers.Count + Journal.Buffers.Count + ActivePasses.Count;
    private int Owner = Environment.CurrentManagedThreadId;
    private bool _disposed;
    private static readonly HashSet<string> PassiveCommands = new(StringComparer.Ordinal)
    {
        "vkCmdBeginQuery", "vkCmdEndQuery", "vkCmdBindDescriptorSets", "vkCmdBindIndexBuffer", "vkCmdBindPipeline", "vkCmdBindVertexBuffers",
        "vkCmdBlitImage", "vkCmdClearAttachments", "vkCmdClearColorImage", "vkCmdClearDepthStencilImage", "vkCmdCopyBuffer", "vkCmdCopyBufferToImage",
        "vkCmdCopyImage", "vkCmdCopyImageToBuffer", "vkCmdCopyQueryPoolResults", "vkCmdDispatch", "vkCmdDispatchIndirect", "vkCmdDraw", "vkCmdDrawIndexed",
        "vkCmdDrawIndexedIndirect", "vkCmdDrawIndirect", "vkCmdFillBuffer", "vkCmdPushConstants", "vkCmdResetEvent", "vkCmdResetQueryPool",
        "vkCmdResolveImage", "vkCmdSetBlendConstants", "vkCmdSetDepthBias", "vkCmdSetDepthBounds", "vkCmdSetEvent", "vkCmdSetLineWidth", "vkCmdSetScissor",
        "vkCmdSetStencilCompareMask", "vkCmdSetStencilReference", "vkCmdSetStencilWriteMask", "vkCmdSetViewport", "vkCmdUpdateBuffer", "vkCmdWriteTimestamp"
    };

    private void Observe(Action action)
    {
        if (Fault != null) return;
        try
        {
            if (_disposed || Environment.CurrentManagedThreadId != Owner) throw new InvalidOperationException("Dispatch owner/lifetime violation.");
            action(); PeakMetadata = Math.Max(PeakMetadata, MetadataCount);
            if (MetadataCount > 16384) throw new NotSupportedException("Observer metadata cap exceeded.");
        }
        catch (Exception e) { Fault = e.ToString(); } // Never unwind through native code.
    }
    public void Check() { if (Fault != null) throw new InvalidOperationException("Vulkan observation fault; block output/reuse. " + Fault); }
    private void Device(VkDevice d) { if (d.Handle != device.Handle) throw new InvalidOperationException("Wrong device dispatch."); }
    public nint Resolve(string name, nint inst, nint dev)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(VulkanObserver));
        if (inst != 0 && inst != instance.Handle || dev != 0 && dev != device.Handle)
            throw new InvalidOperationException("Wrong instance/device procedure lookup.");
        if (Roots.TryGetValue(name, out var known)) return Marshal.GetFunctionPointerForDelegate(known);
        // Global commands (notably vkEnumerateInstanceVersion) must keep the
        // caller's null instance. Android loaders need not accept a bound one.
        nint real = dev != 0 ? vk.GetDeviceProcAddr(new(dev), name) : vk.GetInstanceProcAddr(new(inst), name);
        // Some Android emulator loaders expose only the KHR spelling even when
        // reporting Vulkan 1.2. These are specification aliases with identical
        // signatures. Use them only when this device actually enabled the extension.
        if (real == 0 && dev != 0 && renderPass2ExtensionEnabled && name is
            "vkCreateRenderPass2" or "vkCmdBeginRenderPass2" or "vkCmdNextSubpass2" or "vkCmdEndRenderPass2")
        {
            real = vk.GetDeviceProcAddr(new(dev), name + "KHR");
            if (real != 0) Console.WriteLine($"DorotiGraphite dispatch alias {name}= {name}KHR (enabled VK_KHR_create_renderpass2)");
        }
        if (real == 0) return 0;
        var callback = Wrap(name, real);
        if (callback == null) return real;
        Roots.Add(name, callback);
        return Marshal.GetFunctionPointerForDelegate(callback);
    }
    public T Call<T>(string name) where T : Delegate
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (Roots.TryGetValue(name, out var cached) && cached is T typed) return typed;
        return Marshal.GetDelegateForFunctionPointer<T>(Resolve(name, instance.Handle, device.Handle));
    }
    public void RegisterTarget(ulong handle)
    {
        Check(); var info = Images[handle];
        if (info.MipLevels != 1 || info.ArrayLayers != 1 || info.Samples != SampleCountFlags.Count1Bit || info.Flags.HasFlag(ImageCreateFlags.CreateAliasBit))
            throw new NotSupportedException("Host target requires non-alias single mip/layer/sample.");
        Journal.Register(handle, info.InitialLayout, family);
    }
    public (ImageLayout Layout, uint Family) State(ulong image) { Check(); var s = Journal.Images[image]; return (s.Layout, s.Family); }
    internal void RegisterHostTarget(ulong image, ImageCreateInfo info)
    {
        Check();
        if (Journal.Images.ContainsKey(image)) return; // Persistent backing rewrapped after a viewport change.
        info.PNext = null; info.PQueueFamilyIndices = null;
        Images.Add(image, info); RegisterTarget(image);
    }
    internal void ForgetHostTarget(ulong image) { Images.Remove(image); Journal.RemoveImage(image); }
    /// <summary>Only after the previous owner joined and actual GPU drain/loss was established.</summary>
    internal void TakeShutdownOwnershipAfterGpuDrain() => Owner = Environment.CurrentManagedThreadId;
    private void CreatedImage(VkDevice d, ImageCreateInfo* p, Silk.NET.Vulkan.Image* result)
    {
        Device(d);
        // Copy scalar values only; never retain borrowed pNext/queue-family pointers.
        var copy = *p; copy.PNext = null; copy.PQueueFamilyIndices = null;
        if (copy.Flags.HasFlag(ImageCreateFlags.CreateAliasBit)) throw new NotSupportedException("Aliased images unsupported.");
        Images.Add(result->Handle, copy);
    }
    private void CreatedView(VkDevice d, ImageViewCreateInfo* p, ImageView* result)
    { Device(d); Views.Add(result->Handle, new(p->Image.Handle, p->SubresourceRange)); }
    private void TargetRange(ulong image, ImageSubresourceRange range)
    {
        if (!Journal.Images.ContainsKey(image)) return;
        if (range.AspectMask != ImageAspectFlags.ColorBit || range.BaseMipLevel != 0 || range.BaseArrayLayer != 0 ||
            range.LevelCount is not (1 or uint.MaxValue) || range.LayerCount is not (1 or uint.MaxValue))
            throw new NotSupportedException("Target subresource scope unsupported.");
    }
    private void Barrier(nint command, uint count, ImageMemoryBarrier* barriers)
    {
        for (uint i = 0; i < count; i++)
        {
            var b = barriers[i]; TargetRange(b.Image.Handle, b.SubresourceRange);
            Journal.Barrier(command, b.Image.Handle, b.OldLayout, b.NewLayout, b.SrcQueueFamilyIndex, b.DstQueueFamilyIndex); ExplicitBarriers++;
        }
    }
    private void Barrier2(nint command, DependencyInfo* dependency)
    {
        if (dependency->PNext != null) throw new NotSupportedException("Dependency pNext unsupported.");
        for (uint i = 0; i < dependency->ImageMemoryBarrierCount; i++)
        {
            var b = dependency->PImageMemoryBarriers[i]; TargetRange(b.Image.Handle, b.SubresourceRange);
            if (b.PNext != null) throw new NotSupportedException("Image barrier pNext unsupported.");
            Journal.Barrier(command, b.Image.Handle, b.OldLayout, b.NewLayout, b.SrcQueueFamilyIndex, b.DstQueueFamilyIndex); ExplicitBarriers++;
        }
    }
    private static void Reference(Dictionary<uint, ImageLayout> refs, uint attachment, ImageLayout layout)
    {
        if (attachment == Vk.AttachmentUnused) return;
        if (refs.TryGetValue(attachment, out var existing) && existing != layout) throw new NotSupportedException("Conflicting attachment layouts.");
        refs[attachment] = layout;
    }
    private void CreatedPass(VkDevice d, RenderPassCreateInfo* p, RenderPass* result)
    {
        Device(d); if (p->PNext != null) throw new NotSupportedException("Render pass pNext unsupported.");
        var attachments = new Attachment[p->AttachmentCount];
        for (int i = 0; i < attachments.Length; i++)
        { var a = p->PAttachments[i]; if (a.Flags != 0) throw new NotSupportedException("Attachment alias flags unsupported."); attachments[i] = new(a.InitialLayout, a.FinalLayout); }
        var subpasses = new Dictionary<uint, ImageLayout>[p->SubpassCount];
        for (int i = 0; i < subpasses.Length; i++)
        {
            var s = p->PSubpasses[i]; var refs = subpasses[i] = [];
            for (uint a = 0; a < s.InputAttachmentCount; a++) Reference(refs, s.PInputAttachments[a].Attachment, s.PInputAttachments[a].Layout);
            for (uint a = 0; a < s.ColorAttachmentCount; a++)
            {
                Reference(refs, s.PColorAttachments[a].Attachment, s.PColorAttachments[a].Layout);
                if (s.PResolveAttachments != null)
                { Reference(refs, s.PResolveAttachments[a].Attachment, s.PResolveAttachments[a].Layout); if (s.PResolveAttachments[a].Attachment != Vk.AttachmentUnused) ResolveReferences++; }
            }
            if (s.PDepthStencilAttachment != null) Reference(refs, s.PDepthStencilAttachment->Attachment, s.PDepthStencilAttachment->Layout);
        }
        Passes.Add(result->Handle, new(attachments, subpasses));
    }
    private void CreatedFramebuffer(VkDevice d, FramebufferCreateInfo* p, Silk.NET.Vulkan.Framebuffer* result)
    {
        Device(d); if (p->Flags != 0 || p->PNext != null) throw new NotSupportedException("Imageless/extended framebuffer unsupported.");
        var views = new ulong[p->AttachmentCount]; for (int i = 0; i < views.Length; i++) views[i] = p->PAttachments[i].Handle;
        Framebuffers.Add(result->Handle, new(views));
    }
    private void CreatedPass2(VkDevice d, RenderPassCreateInfo2* p, RenderPass* result)
    {
        Device(d); if (p->PNext != null) throw new NotSupportedException("Render pass2 pNext unsupported.");
        var attachments = new Attachment[p->AttachmentCount];
        for (int i = 0; i < attachments.Length; i++)
        { var a = p->PAttachments[i]; if (a.Flags != 0 || a.PNext != null) throw new NotSupportedException("Extended/alias attachment unsupported."); attachments[i] = new(a.InitialLayout, a.FinalLayout); }
        var subpasses = new Dictionary<uint, ImageLayout>[p->SubpassCount];
        for (int i = 0; i < subpasses.Length; i++)
        {
            var s = p->PSubpasses[i]; var refs = subpasses[i] = [];
            if (s.PNext != null || s.ViewMask != 0) throw new NotSupportedException("Multiview/extended subpass unsupported.");
            void Add(AttachmentReference2* r) { if (r->PNext != null) throw new NotSupportedException("Extended attachment reference unsupported."); Reference(refs, r->Attachment, r->Layout); }
            for (uint a = 0; a < s.InputAttachmentCount; a++) Add(s.PInputAttachments + a);
            for (uint a = 0; a < s.ColorAttachmentCount; a++)
            { Add(s.PColorAttachments + a); if (s.PResolveAttachments != null) { Add(s.PResolveAttachments + a); if (s.PResolveAttachments[a].Attachment != Vk.AttachmentUnused) ResolveReferences++; } }
            if (s.PDepthStencilAttachment != null) Add(s.PDepthStencilAttachment);
        }
        Passes.Add(result->Handle, new(attachments, subpasses));
    }
    private void BeginPass(nint cb, RenderPassBeginInfo* begin)
    {
        if (begin->PNext != null) throw new NotSupportedException("Render pass begin pNext unsupported.");
        var pass = Passes[begin->RenderPass.Handle]; var frame = Framebuffers[begin->Framebuffer.Handle];
        if (frame.Views.Length != pass.Attachments.Length) throw new InvalidOperationException("Attachment count mismatch.");
        var active = new PassInstance(pass, frame.Views.Select(v => Views[v]).ToArray()); ActivePasses.Add(cb, active);
        ApplySubpass(cb, active);
    }
    private void ApplySubpass(nint cb, PassInstance p)
    {
        foreach (var (index, layout) in p.Pass.Subpasses[p.Subpass])
        {
            var view = p.Views[index]; TargetRange(view.Image, view.Range);
            Journal.Barrier(cb, view.Image, p.Layouts[index], layout, Vk.QueueFamilyIgnored, Vk.QueueFamilyIgnored);
            p.Layouts[index] = layout; p.Used.Add(index); ImplicitTransitions++;
        }
    }
    private void NextPass(nint cb) { var p = ActivePasses[cb]; p.Subpass++; ApplySubpass(cb, p); NextSubpasses++; }
    private void EndPass(nint cb)
    {
        var p = ActivePasses[cb];
        foreach (var index in p.Used)
        { Journal.Barrier(cb, p.Views[index].Image, p.Layouts[index], p.Pass.Attachments[index].Final, Vk.QueueFamilyIgnored, Vk.QueueFamilyIgnored); ImplicitTransitions++; }
        ActivePasses.Remove(cb);
    }
    private void Allocated(VkDevice d, CommandBufferAllocateInfo* p, CommandBuffer* buffers)
    { Device(d); for (uint i = 0; i < p->CommandBufferCount; i++) Journal.Allocate(buffers[i].Handle, p->CommandPool.Handle); }
    private void Freed(VkDevice d, uint count, CommandBuffer* buffers)
    { Device(d); for (uint i = 0; i < count; i++) { ActivePasses.Remove(buffers[i].Handle); Journal.Free(buffers[i].Handle); } }
    private void ResetPool(VkDevice d, CommandPool pool, bool destroy)
    {
        Device(d); foreach (var b in Journal.Buffers.Values.Where(b => b.Pool == pool.Handle)) ActivePasses.Remove(b.Handle);
        if (destroy) Journal.FreePool(pool.Handle); else Journal.ResetPool(pool.Handle);
    }
    private void Submitted(Queue q, uint count, SubmitInfo* submits, Result result)
    {
        if (q.Handle != queue.Handle) throw new NotSupportedException("Multiple queues unsupported in one generation.");
        var buffers = new List<nint>();
        for (uint i = 0; i < count; i++) for (uint j = 0; j < submits[i].CommandBufferCount; j++) buffers.Add(submits[i].PCommandBuffers[j].Handle);
        Journal.Submit(buffers, result);
    }
    private void Submitted2(Queue q, uint count, SubmitInfo2* submits, Result result)
    {
        if (q.Handle != queue.Handle) throw new NotSupportedException("Multiple queues unsupported in one generation.");
        var buffers = new List<nint>();
        for (uint i = 0; i < count; i++) for (uint j = 0; j < submits[i].CommandBufferInfoCount; j++) buffers.Add(submits[i].PCommandBufferInfos[j].CommandBuffer.Handle);
        Journal.Submit(buffers, result);
    }
    public object Summary() => new { Fault, MetadataCount, PeakMetadata, ExplicitBarriers, ImplicitTransitions, ResolveReferences, NextSubpasses,
        Journal.SuccessfulSubmissions, Journal.FailedSubmissions, Journal.AppliedTransitions, Journal.PeakOperations, targets = Journal.Images.Count, buffers = Journal.Buffers.Count, rootedDispatchCount = Roots.Count };
    public object ProbeUnsupportedImage()
    {
        // Separate observer: verify a valid real Vulkan allocation call is forwarded
        // unchanged even when our supported-profile check faults. Never use this
        // image in Graphite, submit it, or merge the fault into the healthy generation.
        using var rejected = new VulkanObserver(vk, instance, device, queue, family);
        var info = new ImageCreateInfo { SType = StructureType.ImageCreateInfo, ImageType = ImageType.Type2D,
            Format = Format.R8G8B8A8Unorm, Extent = new(8, 8, 1), MipLevels = 1, ArrayLayers = 1,
            Samples = SampleCountFlags.Count1Bit, Usage = ImageUsageFlags.TransferDstBit, Flags = ImageCreateFlags.CreateAliasBit };
        Silk.NET.Vulkan.Image image = default;
        Result result;
        try
        {
            result = rejected.Call<CreateImageDelegate>("vkCreateImage")(device, &info, null, &image);
            if (result != Result.Success || image.Handle == 0 || rejected.Fault == null)
                throw new InvalidOperationException("Alias profile rejection did not preserve real driver creation.");
            bool prevented = false;
            try { rejected.Check(); } catch (InvalidOperationException) { prevented = true; }
            if (!prevented) throw new InvalidOperationException("Faulted observer allowed output.");
        }
        finally { if (image.Handle != 0) rejected.Call<DestroyImageDelegate>("vkDestroyImage")(device, image, null); }
        return new { status = "PASS-expected-rejection", driverResult = result.ToString(), hostOutputBlocked = true,
            nativeResultModified = false, failure = rejected.Fault };
    }
    public void Dispose() { _disposed = true; GC.KeepAlive(Roots); }
}
