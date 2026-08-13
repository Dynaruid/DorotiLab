#nullable enable
#pragma warning disable CS0108, CS0162, CS0168, CS4014, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622
// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/services/text_editing.dart
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

public class TextSelection : TextRange
{
    public virtual long baseOffset { get; private set; } = default!;
    public virtual long extentOffset { get; private set; } = default!;
    public virtual TextAffinity affinity { get; private set; } = default!;
    public virtual bool isDirectional { get; private set; } = default!;

    public TextSelection(long baseOffset, long extentOffset, TextAffinity affinity = TextAffinity.downstream, bool isDirectional = false) : base(start: ((baseOffset < extentOffset) ? baseOffset : extentOffset), end: ((baseOffset < extentOffset) ? extentOffset : baseOffset))
    {
        this.baseOffset = baseOffset;
        this.extentOffset = extentOffset;
        this.affinity = affinity;
        this.isDirectional = isDirectional;
    }

    public static TextSelection CreateCollapsed(long offset, TextAffinity affinity = TextAffinity.downstream)
    {
        var __instance = new TextSelection(default!, default!, default!, default!);
        __instance.baseOffset = offset;
        __instance.extentOffset = offset;
        __instance.isDirectional = false;
        return __instance;
    }

    public static TextSelection CreateFromPosition(TextPosition position)
    {
        var __instance = new TextSelection(default!, default!, default!, default!);
        __instance.baseOffset = position.offset;
        __instance.extentOffset = position.offset;
        __instance.affinity = position.affinity;
        __instance.isDirectional = false;
        return __instance;
    }

    public virtual TextPosition @base
    {
        get
        {
            global::Doroti.Flutter.Ui.TextAffinity affinity = default!;
            if ((!isValid || (baseOffset == extentOffset)))
            {
                affinity = this.affinity;
            }
            else
            {
                if ((baseOffset < extentOffset))
                {
                    affinity = TextAffinity.downstream;
                }
                else
                {
                    affinity = TextAffinity.upstream;
                }
            }
            return new global::Doroti.Flutter.Ui.TextPosition(offset: baseOffset, affinity: affinity);
        }
    }
    public virtual TextPosition extent
    {
        get
        {
            global::Doroti.Flutter.Ui.TextAffinity affinity = default!;
            if ((!isValid || (baseOffset == extentOffset)))
            {
                affinity = this.affinity;
            }
            else
            {
                if ((baseOffset < extentOffset))
                {
                    affinity = TextAffinity.upstream;
                }
                else
                {
                    affinity = TextAffinity.downstream;
                }
            }
            return new global::Doroti.Flutter.Ui.TextPosition(offset: extentOffset, affinity: affinity);
        }
    }
    public override string ToString()
    {
        string typeName = global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "TextSelection");
        if (!isValid)
        {
            return $"{typeName}.invalid";
        }
        return (isCollapsed ? $"{typeName}.collapsed(offset: {baseOffset}, affinity: {affinity}, isDirectional: {isDirectional})" : $"{typeName}(baseOffset: {baseOffset}, extentOffset: {extentOffset}, isDirectional: {isDirectional})");
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as TextSelection;
        if (__other is null) return false;
        if (ReferenceEquals(this, __other))
        {
            return true;
        }
        if ((__other is not TextSelection))
        {
            return false;
        }
        if (!isValid)
        {
            return !((TextSelection)__other).isValid;
        }
        return ((((((TextSelection)__other).baseOffset == baseOffset) && (((TextSelection)__other).extentOffset == extentOffset)) && ((!isCollapsed || (object.Equals(((TextSelection)__other).affinity, affinity))))) && (((TextSelection)__other).isDirectional == isDirectional));
    }

    public override int GetHashCode()
    {
        if (!isValid)
        {
            return FoundationRuntimePorts.ObjectHash(-1L.GetHashCode(), -1L.GetHashCode(), TextAffinity.downstream.GetHashCode());
        }
        var affinityHash = (isCollapsed ? affinity.GetHashCode() : TextAffinity.downstream.GetHashCode());
        return FoundationRuntimePorts.ObjectHash(baseOffset.GetHashCode(), extentOffset.GetHashCode(), affinityHash, isDirectional.GetHashCode());
    }
    public virtual TextSelection copyWith(long? baseOffset = null, long? extentOffset = null, TextAffinity? affinity = null, bool? isDirectional = null)
    {
        return new TextSelection(baseOffset: (baseOffset ?? this.baseOffset), extentOffset: (extentOffset ?? this.extentOffset), affinity: (affinity ?? this.affinity), isDirectional: (isDirectional ?? this.isDirectional));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual TextSelection expandTo(TextPosition position, bool extentAtIndex = false)
    {
        if (((position.offset >= start) && (position.offset <= end)))
        {
            return this;
        }
        bool normalized = (baseOffset <= extentOffset);
        if ((position.offset <= start))
        {
            if (extentAtIndex)
            {
                return copyWith(baseOffset: end, extentOffset: position.offset, affinity: position.affinity);
            }
            return copyWith(baseOffset: (normalized ? position.offset : baseOffset), extentOffset: (normalized ? extentOffset : position.offset));
        }
        if (extentAtIndex)
        {
            return copyWith(baseOffset: start, extentOffset: position.offset, affinity: position.affinity);
        }
        return copyWith(baseOffset: (normalized ? baseOffset : position.offset), extentOffset: (normalized ? position.offset : extentOffset));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual TextSelection extendTo(TextPosition position)
    {
        if ((object.Equals(extent, position)))
        {
            return this;
        }
        return copyWith(extentOffset: position.offset, affinity: position.affinity);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

