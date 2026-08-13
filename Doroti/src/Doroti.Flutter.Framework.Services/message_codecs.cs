#nullable enable
#pragma warning disable CS0108, CS0162, CS0168, CS4014, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622
// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/services/message_codecs.dart
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

public static partial class Message_codecsLibrary
{
    internal static long _writeBufferStartCapacity = 64L;
}

public class BinaryCodec : MessageCodec<ByteData>
{
    public BinaryCodec()
    {
    }

    public virtual ByteData? decodeMessage(ByteData? message) => message;
    public virtual ByteData? encodeMessage(ByteData? message) => message;
}

public class StringCodec : MessageCodec<string>
{
    public StringCodec()
    {
    }

    public virtual string? decodeMessage(ByteData? message)
    {
        if ((message is null))
        {
            return null;
        }
        return global::Doroti.Flutter.Runtime.Dart_convertLibrary.utf8.decode(new Uint8List(message));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual ByteData? encodeMessage(string? message)
    {
        if ((message is null))
        {
            return null;
        }
        return new ByteData(global::Doroti.Flutter.Runtime.Dart_convertLibrary.utf8.encode(message));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class JSONMessageCodec : MessageCodec<object?>
{
    public JSONMessageCodec()
    {
    }

    public virtual ByteData? encodeMessage(object? message)
    {
        if ((message is null))
        {
            return null;
        }
        return new StringCodec().encodeMessage(Dart_convertLibrary.json.encode(message));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual object decodeMessage(ByteData? message)
    {
        if ((message is null))
        {
            return message;
        }
        return Dart_convertLibrary.json.decode(new StringCodec().decodeMessage(message)!);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class JSONMethodCodec : MethodCodec
{
    public JSONMethodCodec()
    {
    }

    public virtual ByteData encodeMethodCall(MethodCall methodCall)
    {
        return new JSONMessageCodec().encodeMessage(new DartMap<string, object?> { ["method"] = methodCall.method, ["args"] = methodCall.arguments })!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual MethodCall decodeMethodCall(ByteData? methodCall)
    {
        object? decoded = new JSONMessageCodec().decodeMessage(methodCall);
        if (!DartPatternRuntime.TryGetMapValue(decoded, "method", out var methodValue) || methodValue is not string method)
        {
            throw new FormatException($"Expected method call Map, got {decoded}");
        }
        _ = DartPatternRuntime.TryGetMapValue(decoded, "args", out var arguments);
        return new MethodCall(method, arguments);
    }

    public virtual object decodeEnvelope(ByteData envelope)
    {
        object? decoded = new JSONMessageCodec().decodeMessage(envelope);
        if ((decoded is not System.Collections.IList))
        {
            throw new FormatException($"Expected envelope List, got {decoded}");
        }
        if ((((List<object>)decoded).Count == 1L))
        {
            return ((List<object>)decoded)[(int)(0L)];
        }
        if ((((((List<object>)decoded).Count == 3L) && (((List<object>)decoded)[(int)(0L)] is string)) && (((((List<object>)decoded)[(int)(1L)] is null) || (((List<object>)decoded)[(int)(1L)] is string)))))
        {
            throw new PlatformException(code: ((string?)((List<object>)decoded)[(int)(0L)])!, message: ((string?)((List<object>)decoded)[(int)(1L)])!, details: ((List<object>)decoded)[(int)(2L)]);
        }
        if (((((((List<object>)decoded).Count == 4L) && (((List<object>)decoded)[(int)(0L)] is string)) && (((((List<object>)decoded)[(int)(1L)] is null) || (((List<object>)decoded)[(int)(1L)] is string)))) && (((((List<object>)decoded)[(int)(3L)] is null) || (((List<object>)decoded)[(int)(3L)] is string)))))
        {
            throw new PlatformException(code: ((string?)((List<object>)decoded)[(int)(0L)])!, message: ((string?)((List<object>)decoded)[(int)(1L)])!, details: ((List<object>)decoded)[(int)(2L)], stacktrace: ((string?)((List<object>)decoded)[(int)(3L)])!);
        }
        throw new FormatException($"Invalid envelope: {((List<object>)decoded)}");
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual ByteData encodeSuccessEnvelope(object? result)
    {
        return new JSONMessageCodec().encodeMessage(new List<object?> { result })!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual ByteData encodeErrorEnvelope(string code, string? message = null, object? details = null)
    {
        return new JSONMessageCodec().encodeMessage(new List<object?> { code, message, details })!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class StandardMessageCodec : MessageCodec<object?>
{
    internal const long _valueNull = 0L;
    internal const long _valueTrue = 1L;
    internal const long _valueFalse = 2L;
    internal const long _valueInt32 = 3L;
    internal const long _valueInt64 = 4L;
    internal const long _valueLargeInt = 5L;
    internal const long _valueFloat64 = 6L;
    internal const long _valueString = 7L;
    internal const long _valueUint8List = 8L;
    internal const long _valueInt32List = 9L;
    internal const long _valueInt64List = 10L;
    internal const long _valueFloat64List = 11L;
    internal const long _valueList = 12L;
    internal const long _valueMap = 13L;
    internal const long _valueFloat32List = 14L;

    public StandardMessageCodec()
    {
    }

    public virtual ByteData? encodeMessage(object? message)
    {
        if ((message is null))
        {
            return null;
        }
        var buffer = new WriteBuffer(startCapacity: Message_codecsLibrary._writeBufferStartCapacity);
        writeValue(buffer, message);
        return buffer.done();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual object decodeMessage(ByteData? message)
    {
        if ((message is null))
        {
            return null;
        }
        var buffer = new ReadBuffer(message);
        object? result = readValue(buffer);
        if (buffer.hasRemaining)
        {
            throw new FormatException("Message corrupted");
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void writeValue(WriteBuffer buffer, object? value)
    {
        if ((value is null))
        {
            buffer.putUint8(_valueNull);
        }
        else
        {
            if (value is bool value__as14776)
            {
                buffer.putUint8((((bool)value__as14776) ? _valueTrue : _valueFalse));
            }
            else
            {
                if (value is double value__as14865)
                {
                    buffer.putUint8(_valueFloat64);
                    buffer.putFloat64(((double)value__as14865));
                }
                else
                {
                    if (value is long value__as15467)
                    {
                        if ((((-2147483647L - 1L) <= ((long)value__as15467)) && (value__as15467 <= 2147483647L)))
                        {
                            buffer.putUint8(_valueInt32);
                            buffer.putInt32(((long)value__as15467));
                        }
                        else
                        {
                            buffer.putUint8(_valueInt64);
                            buffer.putInt64(((long)value__as15467));
                        }
                    }
                    else
                    {
                        if (value is string value__as15722)
                        {
                            buffer.putUint8(_valueString);
                            var asciiBytes__15790 = new Uint8List(((string)value__as15722).Length);
                            Uint8List? utf8Bytes__15845 = default!;
                            var utf8Offset__15866 = 0L;
                            for (var i = 0L; (i < ((string)value__as15722).Length); i += 1L)
                            {
                                long @char__16019 = ((string)value__as15722).codeUnitAt(i);
                                if ((@char__16019 <= 127L))
                                {
                                    asciiBytes__15790[i] = @char__16019;
                                }
                                else
                                {
                                    utf8Bytes__15845 = global::Doroti.Flutter.Runtime.Dart_convertLibrary.utf8.encode(((string)value__as15722).substring(i));
                                    utf8Offset__15866 = i;
                                    break;
                                }
                            }
                            if ((utf8Bytes__15845 is not null))
                            {
                                writeSize(buffer, (utf8Offset__15866 + utf8Bytes__15845.Count));
                                buffer.putUint8List(new Uint8List(asciiBytes__15790, 0L, utf8Offset__15866));
                                buffer.putUint8List(utf8Bytes__15845);
                            }
                            else
                            {
                                writeSize(buffer, asciiBytes__15790.Count);
                                buffer.putUint8List(asciiBytes__15790);
                            }
                        }
                        else
                        {
                            if (value is Uint8List value__as16573)
                            {
                                buffer.putUint8(_valueUint8List);
                                writeSize(buffer, ((Uint8List)value__as16573).Count);
                                buffer.putUint8List(((Uint8List)value__as16573));
                            }
                            else
                            {
                                if (value is Int32List value__as16723)
                                {
                                    buffer.putUint8(_valueInt32List);
                                    writeSize(buffer, ((Int32List)value__as16723).Count);
                                    buffer.putInt32List(((Int32List)value__as16723));
                                }
                                else
                                {
                                    if (value is Int64List value__as16873)
                                    {
                                        buffer.putUint8(_valueInt64List);
                                        writeSize(buffer, ((Int64List)value__as16873).Count);
                                        buffer.putInt64List(((Int64List)value__as16873));
                                    }
                                    else
                                    {
                                        if (value is Float32List value__as17023)
                                        {
                                            buffer.putUint8(_valueFloat32List);
                                            writeSize(buffer, ((Float32List)value__as17023).Count);
                                            buffer.putFloat32List(((Float32List)value__as17023));
                                        }
                                        else
                                        {
                                            if (value is Float64List value__as17179)
                                            {
                                                buffer.putUint8(_valueFloat64List);
                                                writeSize(buffer, ((Float64List)value__as17179).Count);
                                                buffer.putFloat64List(((Float64List)value__as17179));
                                            }
                                            else
                                            {
                                                if (value is System.Collections.IList value__as17335)
                                                {
                                                    buffer.putUint8(_valueList);
                                                    writeSize(buffer, value__as17335.Count);
                                                    foreach (object? item in value__as17335)
                                                    {
                                                        writeValue(buffer, item);
                                                    }
                                                }
                                                else
                                                {
                                                    if (value is System.Collections.IDictionary value__as17525)
                                                    {
                                                        buffer.putUint8(_valueMap);
                                                        writeSize(buffer, value__as17525.Count);
                                                        foreach (System.Collections.DictionaryEntry entry in value__as17525)
                                                        {
                                                            writeValue(buffer, entry.Key);
                                                            writeValue(buffer, entry.Value);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        throw new ArgumentException(value?.ToString());
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public virtual object? readValue(ReadBuffer buffer)
    {
        if (!buffer.hasRemaining)
        {
            throw new FormatException("Message corrupted");
        }
        long type = buffer.getUint8();
        return readValueOfType(type, buffer);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual object? readValueOfType(long type, ReadBuffer buffer)
    {
        switch (type)
        {
            case var __case18505 when object.Equals(__case18505, _valueNull):
                {
                    return null;
                }
            case var __case18549 when object.Equals(__case18549, _valueTrue):
                {
                    return true;
                }
            case var __case18593 when object.Equals(__case18593, _valueFalse):
                {
                    return false;
                }
            case var __case18639 when object.Equals(__case18639, _valueInt32):
                {
                    return buffer.getInt32();
                }
            case var __case18697 when object.Equals(__case18697, _valueInt64):
                {
                    return buffer.getInt64();
                }
            case var __case18755 when object.Equals(__case18755, _valueFloat64):
                {
                    return buffer.getFloat64();
                }
            case var __case18817 when object.Equals(__case18817, _valueLargeInt):
            case var __case18844 when object.Equals(__case18844, _valueString):
                {
                    long length__18876 = readSize(buffer);
                    return global::Doroti.Flutter.Runtime.Dart_convertLibrary.utf8.decoder.convert(buffer.getUint8List(length__18876));
                }
            case var __case18980 when object.Equals(__case18980, _valueUint8List):
                {
                    long length__19015 = readSize(buffer);
                    return buffer.getUint8List(length__19015);
                }
            case var __case19097 when object.Equals(__case19097, _valueInt32List):
                {
                    long length__19132 = readSize(buffer);
                    return buffer.getInt32List(length__19132);
                }
            case var __case19214 when object.Equals(__case19214, _valueInt64List):
                {
                    long length__19249 = readSize(buffer);
                    return buffer.getInt64List(length__19249);
                }
            case var __case19331 when object.Equals(__case19331, _valueFloat32List):
                {
                    long length__19368 = readSize(buffer);
                    return buffer.getFloat32List(length__19368);
                }
            case var __case19452 when object.Equals(__case19452, _valueFloat64List):
                {
                    long length__19489 = readSize(buffer);
                    return buffer.getFloat64List(length__19489);
                }
            case var __case19573 when object.Equals(__case19573, _valueList):
                {
                    long length__19603 = readSize(buffer);
                    var result__19644 = new List<object?>(System.Linq.Enumerable.Repeat<object?>(null, checked((int)length__19603)));
                    for (var i__19706 = 0L; (i__19706 < length__19603); i__19706++)
                    {
                        result__19644[(int)(i__19706)] = readValue(buffer);
                    }
                    return result__19644;
                }
            case var __case19817 when object.Equals(__case19817, _valueMap):
                {
                    long length__19846 = readSize(buffer);
                    var result__19887 = new DartMap<object?, object?>();
                    for (var i__19935 = 0L; (i__19935 < length__19846); i__19935++)
                    {
                        result__19887[readValue(buffer)] = readValue(buffer);
                    }
                    return result__19887;
                }
            default:
                {
                    throw new FormatException("Message corrupted");
                }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void writeSize(WriteBuffer buffer, long value)
    {
        DartRuntimePrimitives.Assert(() => ((0L <= value) && (value <= 4294967295L)));
        if ((value < 254L))
        {
            buffer.putUint8(value);
        }
        else
        {
            if ((value <= 65535L))
            {
                buffer.putUint8(254L);
                buffer.putUint16(value);
            }
            else
            {
                buffer.putUint8(255L);
                buffer.putUint32(value);
            }
        }
    }

    public virtual long readSize(ReadBuffer buffer)
    {
        long value = buffer.getUint8();
        return (value switch { var __case20966 when object.Equals(__case20966, 254L) => buffer.getUint16(), var __case20999 when object.Equals(__case20999, 255L) => buffer.getUint32(), _ => value });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class StandardMethodCodec : MethodCodec
{
    public virtual StandardMessageCodec messageCodec { get; private set; } = default!;

    public StandardMethodCodec(StandardMessageCodec messageCodec = default!)
    {
        this.messageCodec = messageCodec ?? new StandardMessageCodec();
    }

    public virtual ByteData encodeMethodCall(MethodCall methodCall)
    {
        var buffer = new WriteBuffer(startCapacity: Message_codecsLibrary._writeBufferStartCapacity);
        messageCodec.writeValue(buffer, methodCall.method);
        messageCodec.writeValue(buffer, methodCall.arguments);
        return buffer.done();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual MethodCall decodeMethodCall(ByteData? methodCall)
    {
        var buffer = new ReadBuffer(methodCall!);
        object? method = messageCodec.readValue(buffer);
        object? arguments = messageCodec.readValue(buffer);
        if (((method is string) && !buffer.hasRemaining))
        {
            return new MethodCall(((string)method), arguments);
        }
        else
        {
            throw new FormatException("Invalid method call");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual ByteData encodeSuccessEnvelope(object? result)
    {
        var buffer = new WriteBuffer(startCapacity: Message_codecsLibrary._writeBufferStartCapacity);
        buffer.putUint8(0L);
        messageCodec.writeValue(buffer, result);
        return buffer.done();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual ByteData encodeErrorEnvelope(string code, string? message = null, object? details = null)
    {
        var buffer = new WriteBuffer(startCapacity: Message_codecsLibrary._writeBufferStartCapacity);
        buffer.putUint8(1L);
        messageCodec.writeValue(buffer, code);
        messageCodec.writeValue(buffer, message);
        messageCodec.writeValue(buffer, details);
        return buffer.done();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual object decodeEnvelope(ByteData envelope)
    {
        if ((envelope.lengthInBytes == 0L))
        {
            throw new FormatException("Expected envelope, got nothing");
        }
        var buffer = new ReadBuffer(envelope);
        if ((buffer.getUint8() == 0L))
        {
            return messageCodec.readValue(buffer);
        }
        object? errorCode = messageCodec.readValue(buffer);
        object? errorMessage = messageCodec.readValue(buffer);
        object? errorDetails = messageCodec.readValue(buffer);
        string? errorStacktrace = (buffer.hasRemaining ? ((string?)messageCodec.readValue(buffer))! : null);
        if ((((errorCode is string) && (((errorMessage is null) || (errorMessage is string)))) && !buffer.hasRemaining))
        {
            throw new PlatformException(code: ((string)errorCode), message: ((string?)errorMessage)!, details: errorDetails, stacktrace: errorStacktrace);
        }
        else
        {
            throw new FormatException("Invalid envelope");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
