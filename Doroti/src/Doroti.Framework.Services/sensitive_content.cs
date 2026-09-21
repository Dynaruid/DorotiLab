// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/services/sensitive_content.dart
using Doroti.Runtime;

namespace Doroti.Framework.Services;

public enum ContentSensitivity
{
    autoSensitive,
    sensitive,
    notSensitive,
    _unknown,
}

public class SensitiveContentService
{
    public virtual MethodChannel sensitiveContentChannel { get; set; } = default!;

    public SensitiveContentService() { }

    public virtual async Future setContentSensitivity(ContentSensitivity contentSensitivity)
    {
        await sensitiveContentChannel.invokeMethod<object?>(
            "SensitiveContent.setContentSensitivity",
            FoundationRuntimePorts.EnumIndex(contentSensitivity)
        );
    }

    public virtual async Future<ContentSensitivity> getContentSensitivity()
    {
        long? result = await sensitiveContentChannel.invokeMethod<long>(
            "SensitiveContent.getContentSensitivity"
        );
        ContentSensitivity contentSensitivity = Enum.GetValues<ContentSensitivity>().ToList()[
            (int)(
                result
                ?? throw new global::System.NullReferenceException("A required value was null.")
            )
        ];
        if (Equals(contentSensitivity, ContentSensitivity._unknown))
        {
            throw new NotSupportedException(
                "Android Flutter View has a content sensitivity mode "
                    + "that is not recognized by Flutter. If you see this error, it "
                    + "is possible that the View uses a new mode that Flutter needs to "
                    + "support; please file an issue."
            );
        }
        return contentSensitivity;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual async Future<bool> isSupported()
    {
        if (!Equals(PlatformLibrary.defaultTargetPlatform, TargetPlatform.android))
        {
            return false;
        }
        return (await sensitiveContentChannel.invokeMethod<bool>("SensitiveContent.isSupported"));
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}
