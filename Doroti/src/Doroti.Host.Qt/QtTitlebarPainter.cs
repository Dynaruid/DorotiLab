using SkiaSharp;

namespace Doroti.Host.Qt;

// The Qt host owns caption input and accessibility. Drawing in the same GPU
// surface lets Wayland blur cover the caption and body as a single region.
internal static class QtTitlebarPainter
{
    internal static void Paint(SKCanvas canvas, string title, in QtNativeV2.Surface surface)
    {
        if (surface.TitlebarHeight == 0) return;
        var scale = (float)surface.DevicePixelRatio;
        var width = surface.PixelWidth / scale;
        var height = (float)surface.TitlebarHeight;
        var dark = (surface.TitlebarState & 1) != 0;
        var active = (surface.TitlebarState & 2) != 0;
        var maximized = (surface.TitlebarState & 4) != 0;
        canvas.Save();
        try
        {
            canvas.ResetMatrix();
            canvas.Scale(scale);
            canvas.ClipRect(new(0, 0, width, height));
            using var ink = new SKPaint { IsAntialias = true, BlendMode = SKBlendMode.Src, Color = SKColors.Transparent };
            canvas.DrawRect(0, 0, width, height, ink);
            ink.BlendMode = SKBlendMode.SrcOver;
            var foreground = (dark ? SKColors.White : SKColors.Black).WithAlpha(active ? (byte)230 : (byte)150);
            using var face = SKTypeface.FromFamilyName("Sans");
            using var font = new SKFont(face, 13);
            canvas.Save();
            canvas.ClipRect(new(12, 0, Math.Max(12, width - 144), height));
            ink.Color = foreground;
            canvas.DrawText(title, 12, (height - font.Metrics.Ascent - font.Metrics.Descent) / 2, SKTextAlign.Left, font, ink);
            canvas.Restore();
            for (uint button = 1; button <= 3; button++)
            {
                var left = width - (4 - button) * 46;
                if (surface.TitlebarHovered == button)
                {
                    ink.Color = button == 3 ? new SKColor(210, 40, 45)
                        : (dark ? SKColors.White : SKColors.Black).WithAlpha(surface.TitlebarPressed == button ? (byte)55 : (byte)30);
                    canvas.DrawRect(left, 0, 46, height, ink);
                }
                ink.Color = button == 3 && surface.TitlebarHovered == button ? SKColors.White : foreground;
                ink.Style = SKPaintStyle.Stroke;
                ink.StrokeWidth = 1.25f;
                var x = left + 18;
                var y = height / 2 - 5;
                if (button == 1) canvas.DrawLine(x, y + 9, x + 10, y + 9, ink);
                else if (button == 2)
                {
                    if (maximized) canvas.DrawRect(x + 2, y - 2, 8, 8, ink);
                    canvas.DrawRect(x, y, 8, 8, ink);
                }
                else
                {
                    canvas.DrawLine(x, y, x + 10, y + 10, ink);
                    canvas.DrawLine(x, y + 10, x + 10, y, ink);
                }
                ink.Style = SKPaintStyle.Fill;
            }
        }
        finally { canvas.Restore(); }
    }
}
