using System.Runtime.InteropServices;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;
using VkDevice = Silk.NET.Vulkan.Device;
using VkImage = Silk.NET.Vulkan.Image;

namespace Doroti.Validation.WindowsVulkanCapability;

internal static unsafe partial class Program
{
    // NG1 output qualification uses the existing product native Composition ABI.
    // It owns no Skia pointers; all Graphite work stays in the shared session.
    private sealed class GraphiteCompositionOutput : IDisposable
    {
        private readonly Vk _vk;
        private readonly VkDevice _device;
        private readonly uint _family;
        private readonly int _size;
        private readonly VkImage[] _images = new VkImage[CompositionBufferCount];
        private readonly DeviceMemory[] _memory = new DeviceMemory[CompositionBufferCount];
        private readonly ImageLayout[] _layouts = new ImageLayout[CompositionBufferCount];
        private readonly bool[] _registered = new bool[CompositionBufferCount];
        private readonly ulong[] _events = new ulong[CompositionBufferCount];
        private readonly KhrExternalMemoryWin32 _external;
        private NativeWindow? _window;
        private nint _composition;
        private ulong _tag;
        private bool _disposed;
        internal int CompletedPresents { get; private set; }
        internal Dictionary<string, object> Retirement { get; } = new()
        {
            ["status"] = "notVerified", ["slots"] = CompositionBufferCount,
            ["authority"] = "Presentation buffer availability event and IsAvailable after retire commit",
        };

        internal GraphiteCompositionOutput(Vk vk, Instance instance, PhysicalDevice physical, VkDevice device,
            uint family, string luid, int size)
        {
            if (!OperatingSystem.IsWindows()) throw new PlatformNotSupportedException("Windows Composition requires Windows.");
            _vk = vk; _device = device; _family = family; _size = size;
            if (!vk.TryGetDeviceExtension(instance, device, out KhrExternalMemoryWin32? external) || external is null)
                throw new NotSupportedException("Missing enabled external-memory Win32 dispatch.");
            _external = external;
            try
            {
                _window = NativeWindow.Create();
                ShowWindow(_window.Child, 0);
                _window.Resize((uint)size, (uint)size);
                ShowWindow(_window.Child, 0);
                var bytes = Convert.FromHexString(luid);
                if (bytes.Length != 8) throw new ArgumentException("Vulkan device has no exact 8-byte LUID.");
                var low = BitConverter.ToUInt32(bytes, 0);
                var high = BitConverter.ToInt32(bytes, 4);
                var probe = new VulkanCompositionProbe { AbiVersion = 1, StructSize = (uint)sizeof(VulkanCompositionProbe) };
                RequireNative(CreateComposition(low, high, out _composition, out var surface, ref probe), "CreateComposition");
                if (_composition == 0 || surface == 0 || probe.AdapterLuidMatched != 1 || probe.PresentationSupported != 1 ||
                    probe.ActualAdapterLuidLow != unchecked((int)low) || probe.ActualAdapterLuidHigh != high)
                    throw new InvalidOperationException("D3D11/Graphite Vulkan adapter or Composition capability mismatch.");
                RequireNative(AttachCompositionWindow(_composition, (ulong)_window.Top), "AttachCompositionWindow");
                vk.GetPhysicalDeviceMemoryProperties(physical, out var properties);
                for (uint slot = 0; slot < CompositionBufferCount; slot++)
                {
                    var snapshot = new VulkanCompositionBuffer { AbiVersion = 1, StructSize = (uint)sizeof(VulkanCompositionBuffer) };
                    RequireNative(ReplaceCompositionBuffer(_composition, slot, (uint)size, (uint)size,
                        out var shared, out _events[slot], ref snapshot), "ReplaceCompositionBuffer");
                    _registered[slot] = true;
                    try
                    {
                        if (shared == 0 || _events[slot] == 0 || snapshot.InitiallyAvailable == 0)
                            throw new InvalidOperationException("Composition output slot was not initially available.");
                        var externalInfo = new ExternalMemoryImageCreateInfo { SType = StructureType.ExternalMemoryImageCreateInfo,
                            HandleTypes = ExternalMemoryHandleTypeFlags.D3D11TextureBit };
                        var imageInfo = new ImageCreateInfo { SType = StructureType.ImageCreateInfo, PNext = &externalInfo,
                            ImageType = ImageType.Type2D, Format = Format.B8G8R8A8Unorm, Extent = new((uint)size, (uint)size, 1),
                            MipLevels = 1, ArrayLayers = 1, Samples = SampleCountFlags.Count1Bit, Tiling = ImageTiling.Optimal,
                            Usage = ImageUsageFlags.TransferDstBit, SharingMode = SharingMode.Exclusive };
                        Check(vk.CreateImage(device, &imageInfo, null, out _images[slot]), "vkCreateImage(Composition import)");
                        vk.GetImageMemoryRequirements(device, _images[slot], out var requirements);
                        var handles = new MemoryWin32HandlePropertiesKHR { SType = StructureType.MemoryWin32HandlePropertiesKhr };
                        Check(_external.GetMemoryWin32HandleProperties(device, ExternalMemoryHandleTypeFlags.D3D11TextureBit,
                            (nint)shared, &handles), "vkGetMemoryWin32HandlePropertiesKHR");
                        uint? memoryType = null;
                        for (uint i = 0; i < properties.MemoryTypeCount; i++)
                            if ((requirements.MemoryTypeBits & handles.MemoryTypeBits & (1u << (int)i)) != 0)
                            {
                                memoryType ??= i;
                                if ((properties.MemoryTypes[(int)i].PropertyFlags & MemoryPropertyFlags.DeviceLocalBit) != 0)
                                { memoryType = i; break; }
                            }
                        var dedicated = new MemoryDedicatedAllocateInfo { SType = StructureType.MemoryDedicatedAllocateInfo, Image = _images[slot] };
                        var import = new ImportMemoryWin32HandleInfoKHR { SType = StructureType.ImportMemoryWin32HandleInfoKhr,
                            PNext = &dedicated, HandleType = ExternalMemoryHandleTypeFlags.D3D11TextureBit, Handle = (nint)shared };
                        var allocate = new MemoryAllocateInfo { SType = StructureType.MemoryAllocateInfo, PNext = &import,
                            AllocationSize = requirements.Size, MemoryTypeIndex = memoryType ?? throw new NotSupportedException("No shared memory type.") };
                        Check(vk.AllocateMemory(device, &allocate, null, out _memory[slot]), "vkAllocateMemory(Composition dedicated import)");
                        Check(vk.BindImageMemory(device, _images[slot], _memory[slot], 0), "vkBindImageMemory(Composition)");
                    }
                    finally { if (shared != 0 && !CloseHandle((nint)shared)) throw new InvalidOperationException("Shared texture handle close failed."); }
                }
            }
            catch { Dispose(); throw; }
        }

        private void WaitAvailable(int slot)
        {
            if (WaitForSingleObject((nint)_events[slot], 5000) != 0)
                throw new TimeoutException("Composition buffer availability did not return within five seconds.");
            RequireNative(IsCompositionBufferAvailable(_composition, (uint)slot, out var available), "IsCompositionBufferAvailable");
            if (available != 1) throw new InvalidOperationException("Composition event and availability disagree.");
        }

        internal void RecordCopy(CommandBuffer command, VkImage source, int slot)
        {
            WaitAvailable(slot);
            var barrier = new ImageMemoryBarrier { SType = StructureType.ImageMemoryBarrier,
                OldLayout = _layouts[slot], NewLayout = ImageLayout.TransferDstOptimal,
                SrcQueueFamilyIndex = Vk.QueueFamilyExternal, DstQueueFamilyIndex = _family,
                DstAccessMask = AccessFlags.TransferWriteBit, Image = _images[slot],
                SubresourceRange = new(ImageAspectFlags.ColorBit, 0, 1, 0, 1) };
            _vk.CmdPipelineBarrier(command, PipelineStageFlags.TopOfPipeBit, PipelineStageFlags.TransferBit,
                0, 0, null, 0, null, 1, &barrier);
            var copy = new ImageCopy { SrcSubresource = new(ImageAspectFlags.ColorBit, 0, 0, 1),
                DstSubresource = new(ImageAspectFlags.ColorBit, 0, 0, 1) };
            copy.Extent = new((uint)_size, (uint)_size, 1);
            _vk.CmdCopyImage(command, source, ImageLayout.TransferSrcOptimal, _images[slot], ImageLayout.TransferDstOptimal, 1, &copy);
            barrier.OldLayout = ImageLayout.TransferDstOptimal;
            barrier.NewLayout = ImageLayout.General;
            barrier.SrcQueueFamilyIndex = _family;
            barrier.DstQueueFamilyIndex = Vk.QueueFamilyExternal;
            barrier.SrcAccessMask = AccessFlags.TransferWriteBit;
            barrier.DstAccessMask = 0;
            _vk.CmdPipelineBarrier(command, PipelineStageFlags.TransferBit, PipelineStageFlags.BottomOfPipeBit,
                0, 0, null, 0, null, 1, &barrier);
            _layouts[slot] = ImageLayout.General;
        }

        // Caller has completed the Vulkan copy fence. A GPU submit alone never enters here.
        internal void Present(int slot)
        {
            RequireNative(PresentCropped(_composition, (uint)slot, 0, 0, (uint)_size, (uint)_size,
                ++_tag, 1, 1000, out var observed, out var presentId, out _), "PresentCropped");
            if (observed != 1 || presentId == 0)
                throw new InvalidOperationException("No DwmFlush boundary completion for the Graphite output.");
            CompletedPresents++;
        }

        public void Dispose()
        {
            if (_disposed) return;
            // The outer probe establishes GPU drain before disposal even on failure.
            // Retire platform consumption independently before destroying imported memory.
            if (_composition != 0)
            {
                RequireNative(UnbindCompositionBuffer(_composition, ++_tag, out _), "RetireCompositionBuffers");
                for (var slot = 0; slot < CompositionBufferCount; slot++)
                    if (_registered[slot] && _events[slot] != 0) WaitAvailable(slot);
            }
            for (var slot = 0; slot < CompositionBufferCount; slot++)
            {
                if (_images[slot].Handle != 0) _vk.DestroyImage(_device, _images[slot], null);
                if (_memory[slot].Handle != 0) _vk.FreeMemory(_device, _memory[slot], null);
            }
            if (_composition != 0) DestroyComposition(_composition);
            _external.Dispose();
            _window?.Dispose();
            _disposed = true;
            Retirement["status"] = "PASS";
            Retirement["importedImagesDestroyed"] = _images.Count(image => image.Handle != 0);
            Retirement["completedPresents"] = CompletedPresents;
        }

        private static void RequireNative(int result, string operation)
        {
            Marshal.ThrowExceptionForHR(result);
            if (result != 0) throw new InvalidOperationException($"{operation} did not complete: {result}.");
        }
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool CloseHandle(nint handle);
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern uint WaitForSingleObject(nint handle, uint timeout);
}
