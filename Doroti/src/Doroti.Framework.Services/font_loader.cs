// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/services/font_loader.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Services;

public class FontLoader
{
    public virtual string family { get; private set; } = default!;
    internal virtual bool _loaded { get; set; } = default!;
    internal virtual List<Future<Uint8List>> _fontFutures { get; private set; } = default!;

    public FontLoader(string family)
    {
        this.family = family;
        this._loaded = false;
        this._fontFutures = new List<Future<Uint8List>>();
    }

    public virtual void addFont(Future<ByteData> bytes)
    {
        if (_loaded)
        {
            throw new InvalidOperationException("FontLoader is already loaded");
        }
        _fontFutures.Add(bytes.then<Uint8List>(((data) => new Uint8List(data.buffer, data.offsetInBytes, data.lengthInBytes))));
    }

    public async virtual Future load()
    {
        if (_loaded)
        {
            throw new InvalidOperationException("FontLoader is already loaded");
        }
        _loaded = true;
        foreach (Future<Uint8List> fontFuture in _fontFutures)
        {
            await loadFont(await fontFuture, family);
        }
    }

    public virtual Future loadFont(Uint8List list, string family)
    {
        return Dart_uiLibrary.loadFontFromList(list, fontFamily: family);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

