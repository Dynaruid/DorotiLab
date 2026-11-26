// Adapted from Avalonia 12.2.0 for Doroti. See paragraph-unicode-provenance.json.
namespace Doroti.Skia.Rendering.Unicode;

internal enum GraphemeBreakClass
{
    Other, //XX
    Control, //CN
    CR, //CR
    EBase, //EB
    EBaseGAZ, //EBG
    EModifier, //EM
    Extend, //EX
    GlueAfterZwj, //GAZ
    L, //L
    LF, //LF
    LV, //LV
    LVT, //LVT
    Prepend, //PP
    RegionalIndicator, //RI
    SpacingMark, //SM
    T, //T
    V, //V
    ZWJ, //ZWJ
    ExtendedPictographic
}
