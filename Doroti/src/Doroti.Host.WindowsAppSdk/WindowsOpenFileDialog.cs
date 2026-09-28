using System.Runtime.InteropServices;

namespace Doroti.Host.WindowsAppSdk;

/// <summary>Desktop IFileOpenDialog with an explicit Close path for cancellation.</summary>
internal sealed class WindowsOpenFileDialog : IDisposable
{
    private IFileOpenDialog? _dialog = (IFileOpenDialog)new FileOpenDialogCom();
    private const int Cancelled = unchecked((int)0x800704c7);

    public string[] Show(nint owner, bool multiple, string[] extensions)
    {
        var dialog = _dialog ?? throw new ObjectDisposedException(nameof(WindowsOpenFileDialog));
        dialog.SetOptions(0x40 | 0x1000 | 0x800 | 0x8 | (multiple ? 0x200u : 0));
        dialog.SetFileTypes(1, [new() { Name = "Files", Pattern = string.Join(';', extensions.Select(extension => extension == "*" ? "*.*" : "*" + extension)) }]);
        var result = dialog.Show(owner);
        if (result == Cancelled) return [];
        Marshal.ThrowExceptionForHR(result);
        dialog.GetResults(out var items);
        try
        {
            items.GetCount(out var count);
            var paths = new string[count];
            for (uint index = 0; index < count; index++)
            {
                items.GetItemAt(index, out var item);
                try
                {
                    item.GetDisplayName(0x80058000, out var path); // SIGDN_FILESYSPATH
                    try { paths[index] = Marshal.PtrToStringUni(path) ?? throw new IOException("Selected file has no filesystem path."); }
                    finally { Marshal.FreeCoTaskMem(path); }
                }
                finally { Marshal.FinalReleaseComObject(item); }
            }
            return paths;
        }
        finally { Marshal.FinalReleaseComObject(items); }
    }

    public void Cancel() => _dialog?.Close(Cancelled);
    public void Dispose()
    {
        if (_dialog is not { } dialog) return;
        _dialog = null;
        Marshal.FinalReleaseComObject(dialog);
    }

    [ComImport, Guid("DC1C5A9C-E88A-4DDE-A5A1-60F82A20AEF7")]
    private class FileOpenDialogCom;

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct FilterSpec
    {
        [MarshalAs(UnmanagedType.LPWStr)] public string Name;
        [MarshalAs(UnmanagedType.LPWStr)] public string Pattern;
    }

    // Full vtable, including IModalWindow/IFileDialog inherited slots.
    [ComImport, Guid("D57C7288-D4AD-4768-BE02-9D969532D960"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IFileOpenDialog
    {
        [PreserveSig] int Show(nint owner);
        void SetFileTypes(uint count, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] FilterSpec[] filters);
        void SetFileTypeIndex(uint index);
        void GetFileTypeIndex(out uint index);
        void Advise(nint events, out uint cookie);
        void Unadvise(uint cookie);
        void SetOptions(uint options);
        void GetOptions(out uint options);
        void SetDefaultFolder(IShellItem folder);
        void SetFolder(IShellItem folder);
        void GetFolder(out IShellItem folder);
        void GetCurrentSelection(out IShellItem item);
        void SetFileName([MarshalAs(UnmanagedType.LPWStr)] string name);
        void GetFileName(out nint name);
        void SetTitle([MarshalAs(UnmanagedType.LPWStr)] string title);
        void SetOkButtonLabel([MarshalAs(UnmanagedType.LPWStr)] string text);
        void SetFileNameLabel([MarshalAs(UnmanagedType.LPWStr)] string text);
        void GetResult(out IShellItem item);
        void AddPlace(IShellItem item, int placement);
        void SetDefaultExtension([MarshalAs(UnmanagedType.LPWStr)] string extension);
        void Close(int result);
        void SetClientGuid(in Guid guid);
        void ClearClientData();
        void SetFilter(nint filter);
        void GetResults(out IShellItemArray items);
        void GetSelectedItems(out IShellItemArray items);
    }

    [ComImport, Guid("43826D1E-E718-42EE-BC55-A1E261C37BFE"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IShellItem
    {
        void BindToHandler(nint context, in Guid handler, in Guid iid, out nint result);
        void GetParent(out IShellItem parent);
        void GetDisplayName(uint kind, out nint name);
        void GetAttributes(uint mask, out uint attributes);
        void Compare(IShellItem other, uint hint, out int order);
    }

    [ComImport, Guid("B63EA76D-1F85-456F-A19C-48159EFA858B"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IShellItemArray
    {
        void BindToHandler(nint context, in Guid handler, in Guid iid, out nint result);
        void GetPropertyStore(int flags, in Guid iid, out nint result);
        void GetPropertyDescriptionList(nint key, in Guid iid, out nint result);
        void GetAttributes(uint flags, uint mask, out uint attributes);
        void GetCount(out uint count);
        void GetItemAt(uint index, out IShellItem item);
        void EnumItems(out nint items);
    }
}
