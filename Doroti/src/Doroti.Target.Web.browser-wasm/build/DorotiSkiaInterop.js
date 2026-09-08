// Adapted from SkiaSharp 4.151.1 SkiaSharpInterop.js at
// mono/SkiaSharp commit 279f93f4ffa7f9fe4e9c0bc298bedc3c9e439764 (MIT).
// Exposes only the Emscripten objects required by Doroti's owned presenter.
// Mapped-range adapter derived from emdawnwebgpu library_webgpu.js:
// Copyright 2019 The Emscripten Authors. SPDX-License-Identifier: MIT.
var DorotiSkiaInterop = {
    $DorotiSkiaLibrary: {
        internal_func: function () {
        }
    },
    DorotiInterceptBrowserObjects: function () {
        globalThis.SkiaSharpGL = GL;
        globalThis.SkiaSharpModule = Module;
    },
    // emdawnwebgpu shipped with SkiaSharp 4.154 copies WASM staging memory
    // back on unmap/destroy. Device loss detaches the GPU mapped ArrayBuffer
    // first. Skip only that impossible copy, and always release its WASM
    // allocation. This link-time adapter does not patch the package cache or
    // generated runtime and retains Dawn's original map failure callbacks.
    emwgpuBufferGetMappedRange__deps: ['$WebGPU', 'memalign', 'free'],
    emwgpuBufferGetMappedRange: function (bufferPtr, offset, size) {
        var buffer = WebGPU.getJsObject(bufferPtr);
        if (size === -1 || size === 0xffffffff) size = undefined;
        var mapped;
        try {
            mapped = buffer.getMappedRange(offset, size);
        } catch (error) {
            return 0;
        }
        var length = mapped.byteLength;
        var data = _memalign(16, length);
        HEAPU8.fill(0, data, data + length);
        var counts = Module['dorotiWebGpuMappedRanges'] ||
            (Module['dorotiWebGpuMappedRanges'] = { active: 0, discarded: 0 });
        counts.active++;
        WebGPU.Internals.bufferOnUnmaps[bufferPtr].push(function () {
            try {
                if (length > 0 && mapped.byteLength === 0) {
                    counts.discarded++;
                } else {
                    new Uint8Array(mapped).set(HEAPU8.subarray(data, data + length));
                }
            } finally {
                _free(data);
                counts.active--;
            }
        });
        return data;
    },
};

autoAddDeps(DorotiSkiaInterop, '$DorotiSkiaLibrary');
if (typeof LibraryManager.library.emwgpuBufferGetMappedRange !== 'function')
    throw new Error('Doroti requires the pinned emdawnwebgpu mapped-range ABI before its native interop adapter.');
mergeInto(LibraryManager.library, DorotiSkiaInterop);
