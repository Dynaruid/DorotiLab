#nullable enable
#pragma warning disable CS0108, CS0162, CS0168, CS4014, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622
// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/services/process_text.dart
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

public class ProcessTextAction
{
    public virtual string id { get; private set; } = default!;
    public virtual string label { get; private set; } = default!;

    public ProcessTextAction(string id, string label)
    {
        this.id = id;
        this.label = label;
    }

    public override bool Equals(object? other)
    {
        var __other = other as ProcessTextAction;
        if (__other is null) return false;
        if (ReferenceEquals(this, __other))
        {
            return true;
        }
        return (((__other is ProcessTextAction) && (((ProcessTextAction)__other).id == id)) && (((ProcessTextAction)__other).label == label));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(id, label);
}

public interface ProcessTextService
{
    public Future<List<ProcessTextAction>> queryTextActions();
    public Future<string?> processTextAction(string id, string text, bool readOnly);
}

public class DefaultProcessTextService : ProcessTextService
{
    internal virtual MethodChannel _processTextChannel { get; set; } = default!;

    public DefaultProcessTextService()
    {
    }

    public virtual void setChannel(MethodChannel newChannel)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                _processTextChannel = newChannel;
                return true;
            });
    }

    public async virtual Future<List<ProcessTextAction>> queryTextActions()
    {
        DartMap<object?, object?> rawResults = default!;
        try
        {
            var result__3951 = DartRuntimePrimitives.ConvertMap<object?, object?>((System.Collections.IDictionary)await _processTextChannel.invokeMethod<object>("ProcessText.queryTextActions"));
            if ((result__3951 is null))
            {
                return new List<ProcessTextAction>();
            }
            rawResults = result__3951;
        }
        catch (Exception e)
        {
            return new List<ProcessTextAction>();
        }
        return new List<ProcessTextAction>();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public async virtual Future<string?> processTextAction(string id, string text, bool readOnly)
    {
        var processedText = ((string?)await _processTextChannel.invokeMethod<object>("ProcessText.processTextAction", new List<object> { id, text, readOnly }))!;
        return processedText;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

