// Generated from the pinned CLDR cardinal data by Doroti/eng/generate-cardinal-plurals.py.
// Copyright Unicode, Inc. SPDX-License-Identifier: Unicode-3.0.
using Doroti.Runtime;

namespace Doroti.Intl;

internal static class CardinalPluralRules
{
    internal static int Group(LocaleTag locale) => locale switch
    {
        { Language: "kok", Script: "Latn" } => 1,
        { Language: "pt", Region: "PT" } => 21,
        _ => locale.Language switch
        {
            "bm" or "bo" or "dz" or "hnj" or "id" or "ig" or "ii" or
            "in" or "ja" or "jbo" or "jv" or "jw" or "kde" or "kea" or
            "km" or "ko" or "lkt" or "lo" or "ms" or "my" or "nqo" or
            "osa" or "root" or "sah" or "ses" or "sg" or "su" or "th" or
            "to" or "tpi" or "wo" or "yo" or "yue" or "zh" => 0,
            "am" or "as" or "bn" or "doi" or "fa" or "gu" or "hi" or
            "kn" or "kok" or "pcm" or "tg" or "vi" or "zu" => 1,
            "ff" or "hy" or "kab" => 2,
            "ast" or "de" or "en" or "et" or "fi" or "fy" or "ia" or
            "ie" or "io" or "ji" or "lij" or "nl" or "sc" or "sv" or
            "sw" or "ur" or "yi" => 3,
            "si" => 4,
            "ak" or "bho" or "csw" or "guw" or "ln" or "mg" or "nso" or
            "pa" or "ti" or "wa" => 5,
            "tzm" => 6,
            "af" or "an" or "asa" or "az" or "bal" or "bem" or "bez" or
            "bg" or "brx" or "ce" or "cgg" or "chr" or "ckb" or "dv" or
            "ee" or "el" or "eo" or "eu" or "fo" or "fur" or "gsw" or
            "ha" or "haw" or "hu" or "jgo" or "jmc" or "ka" or "kaj" or
            "kcg" or "kk" or "kkj" or "kl" or "ks" or "ksb" or "ku" or
            "ky" or "lb" or "lg" or "mas" or "mgo" or "ml" or "mn" or
            "mr" or "nah" or "nb" or "nd" or "ne" or "nn" or "nnh" or
            "no" or "nr" or "ny" or "nyn" or "om" or "or" or "os" or
            "pap" or "ps" or "rm" or "rof" or "rwk" or "saq" or "sd" or
            "sdh" or "seh" or "sn" or "so" or "sq" or "ss" or "ssy" or
            "st" or "syr" or "ta" or "te" or "teo" or "tig" or "tk" or
            "tn" or "tr" or "ts" or "ug" or "uz" or "ve" or "vo" or
            "vun" or "wae" or "xh" or "xog" => 7,
            "da" => 8,
            "is" => 9,
            "mk" => 10,
            "ceb" or "fil" or "tl" => 11,
            "lv" or "prg" => 12,
            "lag" => 13,
            "blo" or "cv" or "ksh" => 14,
            "he" or "iw" => 15,
            "iu" or "naq" or "sat" or "se" or "sma" or "smi" or "smj" or
            "smn" or "sms" => 16,
            "shi" => 17,
            "mo" or "ro" => 18,
            "bs" or "hr" or "sh" or "sr" => 19,
            "fr" or "pt" => 20,
            "ca" or "gl" or "it" or "lld" or "scn" or "vec" => 21,
            "es" => 22,
            "gd" => 23,
            "sl" => 24,
            "dsb" or "hsb" => 25,
            "cs" or "sk" => 26,
            "pl" => 27,
            "be" => 28,
            "lt" => 29,
            "ru" or "uk" => 30,
            "sgs" => 31,
            "br" => 32,
            "mt" => 33,
            "ga" => 34,
            "gv" => 35,
            "kw" => 36,
            "ar" or "ars" => 37,
            "cy" => 38,
            _ => -1,
        },
    };

    internal static PluralCategory Select(int group, CardinalOperands o) => group switch
    {
        1 when ((o.I is 0))
                || ((o.N is 1)) => PluralCategory.One,
        2 when ((o.I is 0 or 1)) => PluralCategory.One,
        3 when ((o.I is 1) && (o.V is 0)) => PluralCategory.One,
        4 when ((o.N is 0 or 1))
                || ((o.I is 0) && (o.F is 1)) => PluralCategory.One,
        5 when ((o.NIsInteger && o.N is >= 0 and <= 1)) => PluralCategory.One,
        6 when ((o.NIsInteger && o.N is >= 0 and <= 1))
                || ((o.NIsInteger && o.N is >= 11 and <= 99)) => PluralCategory.One,
        7 when ((o.N is 1)) => PluralCategory.One,
        8 when ((o.N is 1))
                || (!(o.T is 0) && (o.I is 0 or 1)) => PluralCategory.One,
        9 when ((o.T is 0) && (o.I % 10 is 1) && !(o.I % 100 is 11))
                || ((o.T % 10 is 1) && !(o.T % 100 is 11)) => PluralCategory.One,
        10 when ((o.V is 0) && (o.I % 10 is 1) && !(o.I % 100 is 11))
                || ((o.F % 10 is 1) && !(o.F % 100 is 11)) => PluralCategory.One,
        11 when ((o.V is 0) && (o.I is 1 or 2 or 3))
                || ((o.V is 0) && !(o.I % 10 is 4 or 6 or 9))
                || (!(o.V is 0) && !(o.F % 10 is 4 or 6 or 9)) => PluralCategory.One,
        12 when ((o.N % 10 is 0))
                || ((o.NIsInteger && o.N % 100 is >= 11 and <= 19))
                || ((o.V is 2) && (o.F % 100 is >= 11 and <= 19)) => PluralCategory.Zero,
        12 when ((o.N % 10 is 1) && !(o.N % 100 is 11))
                || ((o.V is 2) && (o.F % 10 is 1) && !(o.F % 100 is 11))
                || (!(o.V is 2) && (o.F % 10 is 1)) => PluralCategory.One,
        13 when ((o.N is 0)) => PluralCategory.Zero,
        13 when ((o.I is 0 or 1) && !(o.N is 0)) => PluralCategory.One,
        14 when ((o.N is 0)) => PluralCategory.Zero,
        14 when ((o.N is 1)) => PluralCategory.One,
        15 when ((o.I is 1) && (o.V is 0))
                || ((o.I is 0) && !(o.V is 0)) => PluralCategory.One,
        15 when ((o.I is 2) && (o.V is 0)) => PluralCategory.Two,
        16 when ((o.N is 1)) => PluralCategory.One,
        16 when ((o.N is 2)) => PluralCategory.Two,
        17 when ((o.I is 0))
                || ((o.N is 1)) => PluralCategory.One,
        17 when ((o.NIsInteger && o.N is >= 2 and <= 10)) => PluralCategory.Few,
        18 when ((o.I is 1) && (o.V is 0)) => PluralCategory.One,
        18 when (!(o.V is 0))
                || ((o.N is 0))
                || (!(o.N is 1) && (o.NIsInteger && o.N % 100 is >= 1 and <= 19)) => PluralCategory.Few,
        19 when ((o.V is 0) && (o.I % 10 is 1) && !(o.I % 100 is 11))
                || ((o.F % 10 is 1) && !(o.F % 100 is 11)) => PluralCategory.One,
        19 when ((o.V is 0) && (o.I % 10 is >= 2 and <= 4) && !(o.I % 100 is >= 12 and <= 14))
                || ((o.F % 10 is >= 2 and <= 4) && !(o.F % 100 is >= 12 and <= 14)) => PluralCategory.Few,
        20 when ((o.I is >= 0 and <= 1)) => PluralCategory.One,
        20 when (!(o.I is 0) && (o.I % 1000000 is 0) && (o.V is 0)) => PluralCategory.Many,
        21 when ((o.I is 1) && (o.V is 0)) => PluralCategory.One,
        21 when (!(o.I is 0) && (o.I % 1000000 is 0) && (o.V is 0)) => PluralCategory.Many,
        22 when ((o.N is 1)) => PluralCategory.One,
        22 when (!(o.I is 0) && (o.I % 1000000 is 0) && (o.V is 0)) => PluralCategory.Many,
        23 when ((o.N is 1 or 11)) => PluralCategory.One,
        23 when ((o.N is 2 or 12)) => PluralCategory.Two,
        23 when ((o.NIsInteger && o.N is >= 3 and <= 10 or >= 13 and <= 19)) => PluralCategory.Few,
        24 when ((o.V is 0) && (o.I % 100 is 1)) => PluralCategory.One,
        24 when ((o.V is 0) && (o.I % 100 is 2)) => PluralCategory.Two,
        24 when ((o.V is 0) && (o.I % 100 is >= 3 and <= 4))
                || (!(o.V is 0)) => PluralCategory.Few,
        25 when ((o.V is 0) && (o.I % 100 is 1))
                || ((o.F % 100 is 1)) => PluralCategory.One,
        25 when ((o.V is 0) && (o.I % 100 is 2))
                || ((o.F % 100 is 2)) => PluralCategory.Two,
        25 when ((o.V is 0) && (o.I % 100 is >= 3 and <= 4))
                || ((o.F % 100 is >= 3 and <= 4)) => PluralCategory.Few,
        26 when ((o.I is 1) && (o.V is 0)) => PluralCategory.One,
        26 when ((o.I is >= 2 and <= 4) && (o.V is 0)) => PluralCategory.Few,
        26 when (!(o.V is 0)) => PluralCategory.Many,
        27 when ((o.I is 1) && (o.V is 0)) => PluralCategory.One,
        27 when ((o.V is 0) && (o.I % 10 is >= 2 and <= 4) && !(o.I % 100 is >= 12 and <= 14)) => PluralCategory.Few,
        27 when ((o.V is 0) && !(o.I is 1) && (o.I % 10 is >= 0 and <= 1))
                || ((o.V is 0) && (o.I % 10 is >= 5 and <= 9))
                || ((o.V is 0) && (o.I % 100 is >= 12 and <= 14)) => PluralCategory.Many,
        28 when ((o.N % 10 is 1) && !(o.N % 100 is 11)) => PluralCategory.One,
        28 when ((o.NIsInteger && o.N % 10 is >= 2 and <= 4) && !(o.NIsInteger && o.N % 100 is >= 12 and <= 14)) => PluralCategory.Few,
        28 when ((o.N % 10 is 0))
                || ((o.NIsInteger && o.N % 10 is >= 5 and <= 9))
                || ((o.NIsInteger && o.N % 100 is >= 11 and <= 14)) => PluralCategory.Many,
        29 when ((o.N % 10 is 1) && !(o.NIsInteger && o.N % 100 is >= 11 and <= 19)) => PluralCategory.One,
        29 when ((o.NIsInteger && o.N % 10 is >= 2 and <= 9) && !(o.NIsInteger && o.N % 100 is >= 11 and <= 19)) => PluralCategory.Few,
        29 when (!(o.F is 0)) => PluralCategory.Many,
        30 when ((o.V is 0) && (o.I % 10 is 1) && !(o.I % 100 is 11)) => PluralCategory.One,
        30 when ((o.V is 0) && (o.I % 10 is >= 2 and <= 4) && !(o.I % 100 is >= 12 and <= 14)) => PluralCategory.Few,
        30 when ((o.V is 0) && (o.I % 10 is 0))
                || ((o.V is 0) && (o.I % 10 is >= 5 and <= 9))
                || ((o.V is 0) && (o.I % 100 is >= 11 and <= 14)) => PluralCategory.Many,
        31 when ((o.N % 10 is 1) && !(o.N % 100 is 11)) => PluralCategory.One,
        31 when ((o.N is 2)) => PluralCategory.Two,
        31 when (!(o.N is 2) && (o.NIsInteger && o.N % 10 is >= 2 and <= 9) && !(o.NIsInteger && o.N % 100 is >= 11 and <= 19)) => PluralCategory.Few,
        31 when (!(o.F is 0)) => PluralCategory.Many,
        32 when ((o.N % 10 is 1) && !(o.N % 100 is 11 or 71 or 91)) => PluralCategory.One,
        32 when ((o.N % 10 is 2) && !(o.N % 100 is 12 or 72 or 92)) => PluralCategory.Two,
        32 when ((o.NIsInteger && o.N % 10 is >= 3 and <= 4 or 9) && !(o.NIsInteger && o.N % 100 is >= 10 and <= 19 or >= 70 and <= 79 or >= 90 and <= 99)) => PluralCategory.Few,
        32 when (!(o.N is 0) && (o.N % 1000000 is 0)) => PluralCategory.Many,
        33 when ((o.N is 1)) => PluralCategory.One,
        33 when ((o.N is 2)) => PluralCategory.Two,
        33 when ((o.N is 0))
                || ((o.NIsInteger && o.N % 100 is >= 3 and <= 10)) => PluralCategory.Few,
        33 when ((o.NIsInteger && o.N % 100 is >= 11 and <= 19)) => PluralCategory.Many,
        34 when ((o.N is 1)) => PluralCategory.One,
        34 when ((o.N is 2)) => PluralCategory.Two,
        34 when ((o.NIsInteger && o.N is >= 3 and <= 6)) => PluralCategory.Few,
        34 when ((o.NIsInteger && o.N is >= 7 and <= 10)) => PluralCategory.Many,
        35 when ((o.V is 0) && (o.I % 10 is 1)) => PluralCategory.One,
        35 when ((o.V is 0) && (o.I % 10 is 2)) => PluralCategory.Two,
        35 when ((o.V is 0) && (o.I % 100 is 0 or 20 or 40 or 60 or 80)) => PluralCategory.Few,
        35 when (!(o.V is 0)) => PluralCategory.Many,
        36 when ((o.N is 0)) => PluralCategory.Zero,
        36 when ((o.N is 1)) => PluralCategory.One,
        36 when ((o.N % 100 is 2 or 22 or 42 or 62 or 82))
                || ((o.N % 1000 is 0) && (o.NIsInteger && o.N % 100000 is >= 1000 and <= 20000 or 40000 or 60000 or 80000))
                || (!(o.N is 0) && (o.N % 1000000 is 100000)) => PluralCategory.Two,
        36 when ((o.N % 100 is 3 or 23 or 43 or 63 or 83)) => PluralCategory.Few,
        36 when (!(o.N is 1) && (o.N % 100 is 1 or 21 or 41 or 61 or 81)) => PluralCategory.Many,
        37 when ((o.N is 0)) => PluralCategory.Zero,
        37 when ((o.N is 1)) => PluralCategory.One,
        37 when ((o.N is 2)) => PluralCategory.Two,
        37 when ((o.NIsInteger && o.N % 100 is >= 3 and <= 10)) => PluralCategory.Few,
        37 when ((o.NIsInteger && o.N % 100 is >= 11 and <= 99)) => PluralCategory.Many,
        38 when ((o.N is 0)) => PluralCategory.Zero,
        38 when ((o.N is 1)) => PluralCategory.One,
        38 when ((o.N is 2)) => PluralCategory.Two,
        38 when ((o.N is 3)) => PluralCategory.Few,
        38 when ((o.N is 6)) => PluralCategory.Many,
        _ => PluralCategory.Other,
    };
}
