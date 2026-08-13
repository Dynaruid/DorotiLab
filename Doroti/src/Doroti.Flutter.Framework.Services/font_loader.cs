#nullable enable
#pragma warning disable CS0108, CS0162, CS0168, CS4014, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622
// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/services/font_loader.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Services;

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

