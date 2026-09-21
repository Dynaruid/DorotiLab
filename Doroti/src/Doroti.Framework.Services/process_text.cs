// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/services/process_text.dart
using Doroti.Runtime;

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
        if (__other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, __other))
        {
            return true;
        }
        return (__other is ProcessTextAction) && (__other.id == id) && (__other.label == label);
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

    public DefaultProcessTextService() { }

    public virtual void setChannel(MethodChannel newChannel)
    {
        DartRuntimePrimitives.Assert(() =>
        {
            _processTextChannel = newChannel;
            return true;
        });
    }

    public virtual async Future<List<ProcessTextAction>> queryTextActions()
    {
        DartMap<object?, object?> rawResults = default!;
        try
        {
            var result = await _processTextChannel.invokeMethod<object>(
                "ProcessText.queryTextActions"
            );
            if (result is null)
            {
                return new List<ProcessTextAction>();
            }
            rawResults = result is System.Collections.IDictionary entries
                ? DartRuntimePrimitives.ConvertMap<object?, object?>(entries)
                : throw new FormatException("Process text actions require a map.");
        }
        catch (Exception)
        {
            return new List<ProcessTextAction>();
        }
        return rawResults
            .Select(entry =>
            {
                if (entry.Key is not string id || entry.Value is not string label)
                {
                    throw new FormatException(
                        "Process text action IDs and labels must be strings."
                    );
                }

                return new ProcessTextAction(id, label);
            })
            .ToList();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual async Future<string?> processTextAction(string id, string text, bool readOnly)
    {
        var processedText = (
            (string?)
                await _processTextChannel.invokeMethod<object>(
                    "ProcessText.processTextAction",
                    new List<object> { id, text, readOnly }
                )
        )!;
        return processedText;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}
