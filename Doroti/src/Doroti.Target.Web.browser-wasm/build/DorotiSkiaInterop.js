// Adapted from SkiaSharp 4.151.1 SkiaSharpInterop.js at
// mono/SkiaSharp commit 279f93f4ffa7f9fe4e9c0bc298bedc3c9e439764 (MIT).
// Exposes only the Emscripten objects required by Doroti's owned presenter.
var DorotiSkiaInterop = {
    $DorotiSkiaLibrary: {
        internal_func: function () {
        }
    },
    DorotiInterceptBrowserObjects: function () {
        globalThis.SkiaSharpGL = GL;
        globalThis.SkiaSharpModule = Module;
    }
};

autoAddDeps(DorotiSkiaInterop, '$DorotiSkiaLibrary');
mergeInto(LibraryManager.library, DorotiSkiaInterop);
