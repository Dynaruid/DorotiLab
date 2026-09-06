namespace Doroti.Ui;

/// <summary>Registers owned font bytes in a view's text and raster backends.</summary>
public interface IFontHostCapability
{
    ValueTask RegisterFontAsync(ReadOnlyMemory<byte> bytes, string? family, CancellationToken cancellationToken = default);
}
