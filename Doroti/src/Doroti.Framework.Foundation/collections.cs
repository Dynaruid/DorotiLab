// <doroti-reviewed-framework-source />
#nullable enable
// Doroti typed semantic compiler 3.0.0; source: ../../../flutter-master/packages/flutter/lib/src/foundation/collections.dart
using System;
using System.Collections.Generic;
using System.Linq;
using Doroti.Runtime;
using static Doroti.Runtime.FoundationRuntimePorts;

namespace Doroti.Generated.Framework.Foundation;

public static partial class CollectionsLibrary
{
    public static bool setEquals<T>(HashSet<T>? a, HashSet<T>? b) where T : notnull
    {
        if ((a is null))
        {
            return (b is null);
        }
        if (((b is null) || (a.Count != b.Count)))
        {
            return false;
        }
        if (ReferenceEquals(a, b))
        {
            return true;
        }
        foreach (T value in a)
        {
            if (!b.Contains(value))
            {
                return false;
            }
        }
        return true;
    }
}

public static partial class CollectionsLibrary
{
    public static bool listEquals<T>(List<T>? a, List<T>? b)
    {
        if ((a is null))
        {
            return (b is null);
        }
        if (((b is null) || (a.Count != b.Count)))
        {
            return false;
        }
        if (ReferenceEquals(a, b))
        {
            return true;
        }
        for (var index = 0; (index < a.Count); index += 1)
        {
            if (!EqualityComparer<T>.Default.Equals(a[index], b[index]))
            {
                return false;
            }
        }
        return true;
    }
}

public static partial class CollectionsLibrary
{
    public static bool mapEquals<T, U>(IReadOnlyDictionary<T, U>? a, IReadOnlyDictionary<T, U>? b) where T : notnull
    {
        if ((a is null))
        {
            return (b is null);
        }
        if (((b is null) || (a.Count != b.Count)))
        {
            return false;
        }
        if (ReferenceEquals(a, b))
        {
            return true;
        }
        foreach (T key in a.Keys)
        {
            if ((!b.ContainsKey(key) || !EqualityComparer<U>.Default.Equals(b.GetValueOrDefault(key), a.GetValueOrDefault(key))))
            {
                return false;
            }
        }
        return true;
    }
}

public static partial class CollectionsLibrary
{
    public static int binarySearch<T>(List<T> sortedList, T value)
    {
        var min = 0;
        int max = sortedList.Count;
        while ((min < max))
        {
            int mid = (min + ((((max - min)) >> 1)));
            T element = sortedList[mid];
            int comp = Comparer<T>.Default.Compare(element, value);
            if ((comp == 0))
            {
                return mid;
            }
            if ((comp < 0))
            {
                min = (mid + 1);
            }
            else
            {
                max = mid;
            }
        }
        return -1;
    }
}

public static partial class CollectionsLibrary
{
    internal static readonly int _kMergeSortLimit = 32;
}

public static partial class CollectionsLibrary
{
    public static void mergeSort<T>(List<T> list, int start = 0, int end = -1, Func<T, T, long>? compare = null)
    {
        if (end == -1) end = list.Count;
        compare ??= _defaultCompare<T>();
        int length = (end - start);
        if ((length < 2))
        {
            return;
        }
        if ((length < _kMergeSortLimit))
        {
            _insertionSort<T>(list, compare, start, end);
            return;
        }
        int middle = (start + ((((end - start)) >> 1)));
        int firstLength = (middle - start);
        int secondLength = (end - middle);
        var scratchSpace = new List<T>(System.Linq.Enumerable.Repeat<T>(list[start], secondLength));
        _mergeSort<T>(list, compare, middle, end, scratchSpace, 0);
        int firstTarget = (end - firstLength);
        _mergeSort<T>(list, compare, start, middle, list, firstTarget);
        _merge<T>(compare, list, firstTarget, end, scratchSpace, 0, secondLength, list, start);
    }
}

public static partial class CollectionsLibrary
{
    public static Func<T, T, long> _defaultCompare<T>()
    {
        return (left, right) => Comparer<T>.Default.Compare(left, right);
    }
}

public static partial class CollectionsLibrary
{
    public static void _insertionSort<T>(List<T> list, Func<T, T, long>? compare = null, int start = 0, int end = -1)
    {
        compare ??= _defaultCompare<T>();
        if (end == -1) end = list.Count;
        for (int pos = (start + 1); (pos < end); pos++)
        {
            var min = start;
            var max = pos;
            T element = list[pos];
            while ((min < max))
            {
                int mid = (min + ((((max - min)) >> 1)));
                long comparison = compare(element, list[mid]);
                if ((comparison < 0))
                {
                    max = mid;
                }
                else
                {
                    min = (mid + 1);
                }
            }
            for (var index = pos; index > min; index--)
            {
                list[index] = list[index - 1];
            }
            list[min] = element;
        }
    }
}

public static partial class CollectionsLibrary
{
    public static void _movingInsertionSort<T>(List<T> list, Func<T, T, long> compare, int start, int end, List<T> target, int targetOffset)
    {
        int length = (end - start);
        if ((length == 0))
        {
            return;
        }
        target[targetOffset] = list[start];
        for (var i = 1; (i < length); i++)
        {
            T element = list[(start + i)];
            var min = targetOffset;
            int max = (targetOffset + i);
            while ((min < max))
            {
                int mid = (min + ((((max - min)) >> 1)));
                if ((compare(element, target[mid]) < 0))
                {
                    max = mid;
                }
                else
                {
                    min = (mid + 1);
                }
            }
            for (var index = targetOffset + i; index > min; index--)
            {
                target[index] = target[index - 1];
            }
            target[min] = element;
        }
    }
}

public static partial class CollectionsLibrary
{
    public static void _mergeSort<T>(List<T> list, Func<T, T, long> compare, int start, int end, List<T> target, int targetOffset)
    {
        int length = (end - start);
        if ((length < _kMergeSortLimit))
        {
            _movingInsertionSort<T>(list, compare, start, end, target, targetOffset);
            return;
        }
        int middle = (start + ((length >> 1)));
        int firstLength = (middle - start);
        int secondLength = (end - middle);
        int targetMiddle = (targetOffset + firstLength);
        _mergeSort<T>(list, compare, middle, end, target, targetMiddle);
        _mergeSort<T>(list, compare, start, middle, list, middle);
        _merge<T>(compare, list, middle, (middle + firstLength), target, targetMiddle, (targetMiddle + secondLength), target, targetOffset);
    }
}

public static partial class CollectionsLibrary
{
    public static void _merge<T>(Func<T, T, long> compare, List<T> firstList, int firstStart, int firstEnd, List<T> secondList, int secondStart, int secondEnd, List<T> target, int targetOffset)
    {
        DartRuntimePrimitives.Assert(() => (firstStart < firstEnd));
        DartRuntimePrimitives.Assert(() => (secondStart < secondEnd));
        var cursor1 = firstStart;
        var cursor2 = secondStart;
        T firstElement = firstList[cursor1++];
        T secondElement = secondList[cursor2++];
        while (true)
        {
            if ((compare(firstElement, secondElement) <= 0))
            {
                target[targetOffset++] = firstElement;
                if ((cursor1 == firstEnd))
                {
                    break;
                }
                firstElement = firstList[cursor1++];
            }
            else
            {
                target[targetOffset++] = secondElement;
                if ((cursor2 != secondEnd))
                {
                    secondElement = secondList[cursor2++];
                    continue;
                }
                target[targetOffset++] = firstElement;
                while (cursor1 < firstEnd)
                {
                    target[targetOffset++] = firstList[cursor1++];
                }
                return;
            }
        }
        target[targetOffset++] = secondElement;
        while (cursor2 < secondEnd)
        {
            target[targetOffset++] = secondList[cursor2++];
        }
    }
}
