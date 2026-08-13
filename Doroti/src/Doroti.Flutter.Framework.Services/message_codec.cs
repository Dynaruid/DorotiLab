#nullable enable
#pragma warning disable CS0108, CS0162, CS0168, CS4014, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622
// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/services/message_codec.dart
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

public interface MessageCodec<T>
{
    public ByteData? encodeMessage(T message);
    public T? decodeMessage(ByteData? message);
}

public class MethodCall
{
    public virtual string method { get; private set; } = default!;
    public virtual object arguments { get; private set; } = default!;

    public MethodCall(string method, object arguments = default!)
    {
        this.method = method;
        this.arguments = arguments;
    }

    public override string ToString() => $"{(global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "MethodCall"))}({method}, {arguments})";
}

public interface MethodCodec
{
    public ByteData encodeMethodCall(MethodCall methodCall);
    public MethodCall decodeMethodCall(ByteData? methodCall);
    public object decodeEnvelope(ByteData envelope);
    public ByteData encodeSuccessEnvelope(object? result);
    public ByteData encodeErrorEnvelope(string code, string? message = null, object? details = null);
}

public class PlatformException : Exception
{
    public virtual string code { get; private set; } = default!;
    public virtual string? message { get; private set; }
    public virtual object details { get; private set; } = default!;
    public virtual string? stacktrace { get; private set; }

    public PlatformException(string code, string? message = null, object details = default!, string? stacktrace = null)
    {
        this.code = code;
        this.message = message;
        this.details = details;
        this.stacktrace = stacktrace;
    }

    public override string ToString() => $"PlatformException({code}, {message}, {details}, {stacktrace})";
}

public class MissingPluginException : Exception
{
    public virtual string? message { get; private set; }

    public MissingPluginException(string? message = null)
    {
        this.message = message;
    }

    public override string ToString() => $"MissingPluginException({message})";
}

