#nullable enable
#pragma warning disable CS0108, CS0162, CS0168, CS4014, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622
// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/services/sensitive_content.dart
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

public enum ContentSensitivity
{
    autoSensitive,
    sensitive,
    notSensitive,
    _unknown
}

public class SensitiveContentService
{
    public virtual MethodChannel sensitiveContentChannel { get; set; } = default!;

    public SensitiveContentService()
    {
    }

    public async virtual Future setContentSensitivity(ContentSensitivity contentSensitivity)
    {
        await sensitiveContentChannel.invokeMethod<object?>("SensitiveContent.setContentSensitivity", FoundationRuntimePorts.EnumIndex(contentSensitivity));
    }

    public async virtual Future<ContentSensitivity> getContentSensitivity()
    {
        long? result = await sensitiveContentChannel.invokeMethod<long>("SensitiveContent.getContentSensitivity");
        ContentSensitivity contentSensitivity = System.Enum.GetValues<ContentSensitivity>().ToList()[(int)(DartRuntimePrimitives.RequireValue(result))];
        if ((object.Equals(contentSensitivity, ContentSensitivity._unknown)))
        {
            throw new NotSupportedException("Android Flutter View has a content sensitivity mode " + "that is not recognized by Flutter. If you see this error, it " + "is possible that the View uses a new mode that Flutter needs to " + "support; please file an issue.");
        }
        return contentSensitivity;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public async virtual Future<bool> isSupported()
    {
        if ((!object.Equals(global::Doroti.Generated.Framework.Foundation.PlatformLibrary.defaultTargetPlatform, TargetPlatform.android)))
        {
            return false;
        }
        return DartRuntimePrimitives.RequireValue((await sensitiveContentChannel.invokeMethod<bool>("SensitiveContent.isSupported")));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

