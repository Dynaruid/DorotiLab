#nullable enable
#pragma warning disable CS0108, CS0162, CS0168, CS4014, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622
// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/services/keyboard_inserted_content.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Services;

public class KeyboardInsertedContent
{
    public virtual string mimeType { get; private set; } = default!;
    public virtual string uri { get; private set; } = default!;
    public virtual Uint8List? data { get; private set; }

    public KeyboardInsertedContent(string mimeType, string uri, Uint8List? data = null)
    {
        this.mimeType = mimeType;
        this.uri = uri;
        this.data = data;
    }

    public static KeyboardInsertedContent CreateFromJson(DartMap<string, object> metadata)
    {
        var __instance = new KeyboardInsertedContent(default!, default!, default!);
        __instance.mimeType = ((string?)metadata.GetValueOrDefault("mimeType"))!;
        __instance.uri = ((string?)metadata.GetValueOrDefault("uri"))!;
        __instance.data = ((metadata.GetValueOrDefault("data") is not null) ? new Uint8List(new List<long>(DartRuntimePrimitives.ConvertEnumerable<long>(((IEnumerable<object>?)metadata.GetValueOrDefault("data"))!))) : null);
        return __instance;
    }

    public virtual bool hasData => (((bool?)((data?.Count != 0))) ?? false);
    public override string ToString() => $"{(global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "KeyboardInsertedContent"))}({mimeType}, {uri}, {data})";
    public override bool Equals(object? other)
    {
        var __other = other as KeyboardInsertedContent;
        if (__other is null) return false;
        if ((!object.Equals(__other.GetType(), this.GetType())))
        {
            return false;
        }
        return ((((__other is KeyboardInsertedContent) && (((KeyboardInsertedContent)__other).mimeType == mimeType)) && (((KeyboardInsertedContent)__other).uri == uri)) && (object.Equals(((KeyboardInsertedContent)__other).data, data)));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(mimeType, uri, data);
}

