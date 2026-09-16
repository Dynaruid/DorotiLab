// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/services/text_layout_metrics.dart
using Doroti.Ui;

namespace Doroti.Framework.Services;

public interface TextLayoutMetrics
{
    public static bool isWhitespace(long codeUnit)
    {
        switch (codeUnit)
        {
            case var __case839 when Equals(__case839, 9L):
            case var __case873 when Equals(__case873, 10L):
            case var __case902 when Equals(__case902, 11L):
            case var __case934 when Equals(__case934, 12L):
            case var __case963 when Equals(__case963, 13L):
            case var __case998 when Equals(__case998, 28L):
            case var __case1033 when Equals(__case1033, 29L):
            case var __case1069 when Equals(__case1069, 30L):
            case var __case1106 when Equals(__case1106, 31L):
            case var __case1141 when Equals(__case1141, 32L):
            case var __case1167 when Equals(__case1167, 160L):
            case var __case1202 when Equals(__case1202, 5760L):
            case var __case1241 when Equals(__case1241, 8192L):
            case var __case1271 when Equals(__case1271, 8193L):
            case var __case1301 when Equals(__case1301, 8194L):
            case var __case1332 when Equals(__case1332, 8195L):
            case var __case1363 when Equals(__case1363, 8196L):
            case var __case1404 when Equals(__case1404, 8197L):
            case var __case1443 when Equals(__case1443, 8198L):
            case var __case1482 when Equals(__case1482, 8199L):
            case var __case1517 when Equals(__case1517, 8200L):
            case var __case1557 when Equals(__case1557, 8201L):
            case var __case1590 when Equals(__case1590, 8202L):
            case var __case1623 when Equals(__case1623, 8239L):
            case var __case1667 when Equals(__case1667, 8287L):
            case var __case1715 when Equals(__case1715, 12288L):
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
            case var __case2091 when Equals(__case2091, 10L):
            case var __case2121 when Equals(__case2121, 11L):
            case var __case2155 when Equals(__case2155, 12L):
            case var __case2185 when Equals(__case2185, 13L):
            case var __case2221 when Equals(__case2221, 133L):
            case var __case2250 when Equals(__case2250, 8232L):
            case var __case2287 when Equals(__case2287, 8233L):
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

