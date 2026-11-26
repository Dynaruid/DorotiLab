// Adapted from Avalonia 12.2.0 for Doroti. See paragraph-unicode-provenance.json.
namespace Doroti.Skia.Rendering.Unicode;

internal enum WordBreakClass
{
    Other, //XX
    CarriageReturn, //CR
    LineFeed, //LF
    Newline, //NL
    Extend, //Extend
    ZWJ, //ZWJ
    RegionalIndicator, //RI
    Format, //FO
    Katakana, //KA
    HebrewLetter, //HL
    ALetter, //LE
    SingleQuote, //SQ
    DoubleQuote, //DQ
    MidNumLet, //MB
    MidLetter, //ML
    MidNum, //MN
    Numeric, //NU
    ExtendNumLet, //EX
    WSegSpace, //WSegSpace
}
