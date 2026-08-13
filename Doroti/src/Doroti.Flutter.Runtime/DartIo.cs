namespace Doroti.Flutter.Runtime;

/// <summary>Minimal dart:io File binding used by the native framework image providers.</summary>
public sealed class DartFile(string path)
{
    public string path { get; } = Path.GetFullPath(path);

    public Future<long> length() => Future<long>.value(new FileInfo(path).Length);

    public Future<Uint8List> readAsBytes() => Future<Uint8List>.value(
        new Uint8List(File.ReadAllBytes(path).Select(value => (long)value)));

    public override string ToString() => $"File: '{path}'";
}
