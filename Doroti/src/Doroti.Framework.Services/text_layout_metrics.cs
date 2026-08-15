#nullable enable
#pragma warning disable CS0108, CS0162, CS0168, CS4014, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622
// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/services/text_layout_metrics.dart
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

public interface TextLayoutMetrics
{
    public static bool isWhitespace(long codeUnit)
    {
        switch (codeUnit)
        {
            case var __case839 when object.Equals(__case839, 9L):
            case var __case873 when object.Equals(__case873, 10L):
            case var __case902 when object.Equals(__case902, 11L):
            case var __case934 when object.Equals(__case934, 12L):
            case var __case963 when object.Equals(__case963, 13L):
            case var __case998 when object.Equals(__case998, 28L):
            case var __case1033 when object.Equals(__case1033, 29L):
            case var __case1069 when object.Equals(__case1069, 30L):
            case var __case1106 when object.Equals(__case1106, 31L):
            case var __case1141 when object.Equals(__case1141, 32L):
            case var __case1167 when object.Equals(__case1167, 160L):
            case var __case1202 when object.Equals(__case1202, 5760L):
            case var __case1241 when object.Equals(__case1241, 8192L):
            case var __case1271 when object.Equals(__case1271, 8193L):
            case var __case1301 when object.Equals(__case1301, 8194L):
            case var __case1332 when object.Equals(__case1332, 8195L):
            case var __case1363 when object.Equals(__case1363, 8196L):
            case var __case1404 when object.Equals(__case1404, 8197L):
            case var __case1443 when object.Equals(__case1443, 8198L):
            case var __case1482 when object.Equals(__case1482, 8199L):
            case var __case1517 when object.Equals(__case1517, 8200L):
            case var __case1557 when object.Equals(__case1557, 8201L):
            case var __case1590 when object.Equals(__case1590, 8202L):
            case var __case1623 when object.Equals(__case1623, 8239L):
            case var __case1667 when object.Equals(__case1667, 8287L):
            case var __case1715 when object.Equals(__case1715, 12288L):
                {
                    break;
                }
            default:
                {
                    return false;
                }
        }
        return true;
    }
    public static bool isLineTerminator(long codeUnit)
    {
        switch (codeUnit)
        {
            case var __case2091 when object.Equals(__case2091, 10L):
            case var __case2121 when object.Equals(__case2121, 11L):
            case var __case2155 when object.Equals(__case2155, 12L):
            case var __case2185 when object.Equals(__case2185, 13L):
            case var __case2221 when object.Equals(__case2221, 133L):
            case var __case2250 when object.Equals(__case2250, 8232L):
            case var __case2287 when object.Equals(__case2287, 8233L):
                {
                    return true;
                }
            default:
                {
                    return false;
                }
        }
    }
    public TextSelection getLineAtOffset(TextPosition position);
    public TextRange getWordBoundary(TextPosition position);
    public TextPosition getTextPositionAbove(TextPosition position);
    public TextPosition getTextPositionBelow(TextPosition position);
}

