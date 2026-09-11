"""Emit typed forwarding callbacks. Signatures mirror Silk.NET.Vulkan/Vulkan C ABI.
Generated C# is checked in and compiled; no runtime code generation/reflection emit.
"""
from pathlib import Path

rows = [
("CreateImage", "Result", "VkDevice d, ImageCreateInfo* p, AllocationCallbacks* a, VkImage* output", "if (result == Result.Success) CreatedImage(d, p, output);"),
("DestroyImage", "void", "VkDevice d, VkImage image, AllocationCallbacks* a", "Device(d); Images.Remove(image.Handle); Journal.RemoveImage(image.Handle);"),
("CreateImageView", "Result", "VkDevice d, ImageViewCreateInfo* p, AllocationCallbacks* a, ImageView* output", "if (result == Result.Success) CreatedView(d, p, output);"),
("DestroyImageView", "void", "VkDevice d, ImageView view, AllocationCallbacks* a", "Device(d); Views.Remove(view.Handle);"),
("CreateRenderPass", "Result", "VkDevice d, RenderPassCreateInfo* p, AllocationCallbacks* a, RenderPass* output", "if (result == Result.Success) CreatedPass(d, p, output);"),
("CreateRenderPass2", "Result", "VkDevice d, RenderPassCreateInfo2* p, AllocationCallbacks* a, RenderPass* output", "if (result == Result.Success) CreatedPass2(d, p, output);"),
("DestroyRenderPass", "void", "VkDevice d, RenderPass pass, AllocationCallbacks* a", "Device(d); Passes.Remove(pass.Handle);"),
("CreateFramebuffer", "Result", "VkDevice d, FramebufferCreateInfo* p, AllocationCallbacks* a, VkFramebuffer* output", "if (result == Result.Success) CreatedFramebuffer(d, p, output);"),
("DestroyFramebuffer", "void", "VkDevice d, VkFramebuffer frame, AllocationCallbacks* a", "Device(d); Framebuffers.Remove(frame.Handle);"),
("AllocateCommandBuffers", "Result", "VkDevice d, CommandBufferAllocateInfo* p, CommandBuffer* output", "if (result == Result.Success) Allocated(d, p, output);"),
("FreeCommandBuffers", "void", "VkDevice d, CommandPool pool, uint count, CommandBuffer* buffers", "Freed(d, count, buffers);"),
("BeginCommandBuffer", "Result", "CommandBuffer cb, CommandBufferBeginInfo* p", "if (result == Result.Success) { Journal.Begin(cb.Handle); ActivePasses.Remove(cb.Handle); if (p->PInheritanceInfo != null && p->PInheritanceInfo->RenderPass.Handle != 0) throw new NotSupportedException(\"Inherited render-pass secondary unsupported.\"); }"),
("EndCommandBuffer", "Result", "CommandBuffer cb", "if (result == Result.Success) { if (ActivePasses.ContainsKey(cb.Handle)) throw new InvalidOperationException(\"Open pass at EndCommandBuffer.\"); Journal.End(cb.Handle); }"),
("ResetCommandBuffer", "Result", "CommandBuffer cb, CommandBufferResetFlags flags", "if (result == Result.Success) { Journal.Begin(cb.Handle); ActivePasses.Remove(cb.Handle); }"),
("ResetCommandPool", "Result", "VkDevice d, CommandPool pool, CommandPoolResetFlags flags", "if (result == Result.Success) ResetPool(d, pool, false);"),
("DestroyCommandPool", "void", "VkDevice d, CommandPool pool, AllocationCallbacks* a", "ResetPool(d, pool, true);"),
("CmdPipelineBarrier", "void", "CommandBuffer cb, PipelineStageFlags src, PipelineStageFlags dst, DependencyFlags flags, uint memoryCount, MemoryBarrier* memory, uint bufferCount, BufferMemoryBarrier* buffers, uint imageCount, ImageMemoryBarrier* images", "Barrier(cb.Handle, imageCount, images);"),
("CmdWaitEvents", "void", "CommandBuffer cb, uint count, Silk.NET.Vulkan.Event* events, PipelineStageFlags src, PipelineStageFlags dst, uint memoryCount, MemoryBarrier* memory, uint bufferCount, BufferMemoryBarrier* buffers, uint imageCount, ImageMemoryBarrier* images", "Barrier(cb.Handle, imageCount, images);"),
("CmdPipelineBarrier2", "void", "CommandBuffer cb, DependencyInfo* dependency", "Barrier2(cb.Handle, dependency);"),
("CmdWaitEvents2", "void", "CommandBuffer cb, uint count, Silk.NET.Vulkan.Event* events, DependencyInfo* dependencies", "throw new NotSupportedException(\"Split synchronization2 events unsupported.\");"),
("CmdSetEvent2", "void", "CommandBuffer cb, Silk.NET.Vulkan.Event e, DependencyInfo* dependency", "throw new NotSupportedException(\"Split synchronization2 events unsupported.\");"),
("CmdBeginRenderPass", "void", "CommandBuffer cb, RenderPassBeginInfo* p, SubpassContents contents", "BeginPass(cb.Handle, p);"),
("CmdNextSubpass", "void", "CommandBuffer cb, SubpassContents contents", "NextPass(cb.Handle);"),
("CmdEndRenderPass", "void", "CommandBuffer cb", "EndPass(cb.Handle);"),
("CmdBeginRenderPass2", "void", "CommandBuffer cb, RenderPassBeginInfo* p, SubpassBeginInfo* begin", "BeginPass(cb.Handle, p);"),
("CmdNextSubpass2", "void", "CommandBuffer cb, SubpassBeginInfo* begin, SubpassEndInfo* end", "NextPass(cb.Handle);"),
("CmdEndRenderPass2", "void", "CommandBuffer cb, SubpassEndInfo* end", "EndPass(cb.Handle);"),
("CmdBeginRendering", "void", "CommandBuffer cb, RenderingInfo* p", "throw new NotSupportedException(\"Dynamic rendering unsupported in this observer.\");"),
("CmdEndRendering", "void", "CommandBuffer cb", "throw new NotSupportedException(\"Dynamic rendering unsupported in this observer.\");"),
("CmdExecuteCommands", "void", "CommandBuffer cb, uint count, CommandBuffer* children", "for (uint i = 0; i < count; i++) Journal.Secondary(cb.Handle, children[i].Handle);"),
("QueueSubmit", "Result", "Queue q, uint count, SubmitInfo* submits, Fence fence", "Submitted(q, count, submits, result);"),
("QueueSubmit2", "Result", "Queue q, uint count, SubmitInfo2* submits, Fence fence", "Submitted2(q, count, submits, result);"),
("QueueBindSparse", "Result", "Queue q, uint count, BindSparseInfo* binds, Fence fence", "throw new NotSupportedException(\"Sparse binding unsupported.\");"),
]
aliases = {"CreateRenderPass2", "CmdPipelineBarrier2", "CmdWaitEvents2", "CmdSetEvent2", "CmdBeginRenderPass2", "CmdNextSubpass2", "CmdEndRenderPass2", "CmdBeginRendering", "CmdEndRendering", "QueueSubmit2"}
lines = ["// Generated by generate-dispatch.py; edit the generator and regenerate.",
         "using System.Runtime.InteropServices;", "using Silk.NET.Vulkan;",
         "using VkDevice = Silk.NET.Vulkan.Device;", "using VkImage = Silk.NET.Vulkan.Image;",
         "using VkFramebuffer = Silk.NET.Vulkan.Framebuffer;", "namespace Doroti.Skia.Vulkan;", "internal sealed unsafe partial class VulkanObserver", "{"]
for name, result, args, body in rows:
    lines += ["    [UnmanagedFunctionPointer(CallingConvention.Winapi)]", f"    public delegate {result} {name}Delegate({args});"]
lines += ["    private Delegate? Wrap(string name, nint address)", "    {", "        switch (name)", "        {"]
for name, result, args, body in rows:
    names = ", ".join(arg.rsplit(" ",1)[1] for arg in args.split(", "))
    lines.append(f'            case "vk{name}":')
    if name in aliases: lines.append(f'            case "vk{name}KHR":')
    lines += ["            {", f"                var forward = Marshal.GetDelegateForFunctionPointer<{name}Delegate>(address);",
              f"                return new {name}Delegate(({names}) =>", "                {"]
    lines.append(f"                    {'var result = ' if result != 'void' else ''}forward({names});")
    lines.append(f"                    Observe(() => {{ {body} }});")
    if result != "void": lines.append("                    return result;")
    lines += ["                });", "            }"]
lines += ["            default:", '                if (name.StartsWith("vkCmd", StringComparison.Ordinal) && !PassiveCommands.Contains(name))',
          '                    Observe(() => throw new NotSupportedException("Unknown command entry point: " + name));',
          "                return null;", "        }", "    }", "}"]
Path(__file__).with_name("VulkanObserver.Dispatch.cs").write_text("\n".join(lines)+"\n", encoding="utf-8")
