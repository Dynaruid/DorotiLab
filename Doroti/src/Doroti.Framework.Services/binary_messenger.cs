// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/services/binary_messenger.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Services;

public delegate Future<ByteData?>? MessageHandler(ByteData? message);

public interface BinaryMessenger
{
    public Future handlePlatformMessage(string channel, ByteData? data, Action<ByteData?>? callback);
    public Future<ByteData?>? send(string channel, ByteData? message);
    public void setMessageHandler(string channel, Func<ByteData?, Future<ByteData?>?>? handler);
}

