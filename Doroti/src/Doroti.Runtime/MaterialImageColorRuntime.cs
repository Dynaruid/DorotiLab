// Copyright 2021 Google LLC
// Copyright 2021-2022 project contributors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// Adapted Wu/Wsmeans/Score behavior: material_color_utilities 0.13.0.
// PaletteRandom: Copyright 2012, the Dart project authors (BSD-3-Clause).
// Local changes and complete licenses: ../../THIRD-PARTY-NOTICES.md.
using MaterialColorUtilities.ColorAppearance;
using MaterialColorUtilities.Quantize;

namespace Doroti.Runtime;

/// <summary>ARGB image palette extraction, independent of image decoding and host byte order.</summary>
public static class MaterialImageColorRuntime
{
    public static IReadOnlyDictionary<long, long> Quantize(IEnumerable<long> argbPixels, int maxColors = 128)
    {
        ArgumentNullException.ThrowIfNull(argbPixels);
        if (maxColors is < 1 or > 256) throw new ArgumentOutOfRangeException(nameof(maxColors));
        var pixels = argbPixels.Select(value => unchecked((uint)value)).ToArray();
        if (pixels.Length == 0) return new Dictionary<long, long>();
        return QuantizeWsmeans(pixels, maxColors);
    }

    // Pinned Dart uses round-robin assignments and five iterations. Wu ignores nonopaque
    // input; Wsmeans includes its RGB/population. Alpha is not a Lab dimension; output is opaque.
    private static IReadOnlyDictionary<long, long> QuantizeWsmeans(uint[] pixels, int maxColors)
    {
        var countsByColor = new Dictionary<uint, int>();
        foreach (var pixel in pixels) countsByColor[pixel] = countsByColor.GetValueOrDefault(pixel) + 1;
        var provider = new PointProviderLab();
        var points = countsByColor.Keys.Select(provider.FromInt).ToArray();
        var counts = countsByColor.Values.ToArray();
        var count = Math.Min(maxColors, points.Length);
        var clusters = new MaterialImageQuantizerWu().Quantize(pixels, (uint)maxColors).ColorToCount.Keys
            .Select(provider.FromInt).ToList();
        if (clusters.Count < count)
        {
            var random = new PaletteRandom(0x42688);
            var selected = new HashSet<int>();
            while (clusters.Count < count)
            {
                var index = random.Next(points.Length);
                if (selected.Add(index)) clusters.Add(points[index]);
            }
        }
        var assigned = Enumerable.Range(0, points.Length).Select(index => index % count).ToArray();
        var distances = Enumerable.Range(0, count).Select(_ => new double[count]).ToArray();
        var populations = new long[count];
        for (var iteration = 0; iteration < 5; iteration++)
        {
            for (var i = 0; i < count; i++)
            {
                for (var j = i + 1; j < count; j++)
                    distances[j][i] = distances[i][j] = provider.Distance(clusters[i], clusters[j]);
                Array.Sort(distances[i]);
            }
            var moved = 0;
            for (var i = 0; i < points.Length; i++)
            {
                var previous = assigned[i];
                var previousDistance = provider.Distance(points[i], clusters[previous]);
                var minimum = previousDistance;
                var next = -1;
                for (var j = 0; j < count; j++)
                {
                    if (distances[previous][j] >= 4 * previousDistance) continue;
                    var distance = provider.Distance(points[i], clusters[j]);
                    if (distance < minimum) { minimum = distance; next = j; }
                }
                if (next != -1) { moved++; assigned[i] = next; }
            }
            if (moved == 0 && iteration > 0) break;
            var sums = Enumerable.Range(0, count).Select(_ => new double[3]).ToArray();
            Array.Clear(populations);
            for (var i = 0; i < points.Length; i++)
            {
                var cluster = assigned[i];
                populations[cluster] += counts[i];
                for (var component = 0; component < 3; component++) sums[cluster][component] += points[i][component] * counts[i];
            }
            for (var i = 0; i < count; i++)
                clusters[i] = populations[i] == 0 ? [0, 0, 0] : sums[i].Select(value => value / populations[i]).ToArray();
        }
        var result = new Dictionary<long, long>();
        for (var i = 0; i < count; i++)
            if (populations[i] > 0) result.TryAdd(provider.ToInt(clusters[i]), populations[i]);
        return result;
    }

    public static IReadOnlyList<long> Score(IReadOnlyDictionary<long, long> colors, int desired = 1)
    {
        ArgumentNullException.ThrowIfNull(colors);
        if (desired < 1) throw new ArgumentOutOfRangeException(nameof(desired));
        var entries = colors.Where(entry => entry.Value > 0)
            .Select(entry => (Color: Hct.FromInt(unchecked((uint)entry.Key)), Population: entry.Value)).ToArray();
        var population = new double[360];
        var total = entries.Sum(entry => (double)entry.Population);
        foreach (var entry in entries) population[(int)Math.Floor(entry.Color.Hue)] += entry.Population;
        var excited = new double[360];
        if (total > 0)
            for (var hue = 0; hue < 360; hue++)
                for (var neighbor = hue - 14; neighbor < hue + 16; neighbor++)
                    excited[(neighbor + 360) % 360] += population[hue] / total;
        var ranked = entries.Select(entry =>
        {
            var hct = entry.Color;
            var proportion = excited[(int)Math.Floor(hct.Hue + 0.5) % 360];
            var score = proportion * 70 + (hct.Chroma - 48) * (hct.Chroma < 48 ? 0.1 : 0.3);
            return (Color: hct, Proportion: proportion, Score: score);
        }).Where(entry => entry.Color.Chroma >= 5 && entry.Proportion > 0.01)
            .OrderByDescending(entry => entry.Score).ToArray();
        var chosen = new List<Hct>();
        for (var separation = 90; separation >= 15; separation--)
        {
            chosen.Clear();
            foreach (var entry in ranked)
            {
                if (chosen.Any(color => 180 - Math.Abs(Math.Abs(color.Hue - entry.Color.Hue) - 180) < separation)) continue;
                chosen.Add(entry.Color);
                if (chosen.Count >= desired) break;
            }
            if (chosen.Count >= desired) break;
        }
        return chosen.Count == 0 ? [0xff4285f4L] : chosen.Select(color => (long)color.ToInt()).ToArray();
    }

    // Dart VM seeded Random (BSD-3-Clause) preserves the oracle's extra-center choice.
    private sealed class PaletteRandom
    {
        private ulong state;
        public PaletteRandom(ulong seed)
        {
            unchecked
            {
                seed = ~seed + (seed << 21);
                seed ^= seed >> 24;
                seed *= 265;
                seed ^= seed >> 14;
                seed *= 21;
                seed ^= seed >> 28;
                seed += seed << 31;
                state = seed == 0 ? 0x5a17 : seed;
                for (var i = 0; i < 4; i++) NextState();
            }
        }
        private void NextState() => state = unchecked(0xffffda61UL * (uint)state + (state >> 32));
        public int Next(int max)
        {
            ulong value, result;
            do { NextState(); value = (uint)state; result = value % (uint)max; }
            while (value - result + (uint)max > 0x100000000UL);
            return (int)result;
        }
    }

    /// <summary>Reads a tightly packed RGBA byte view without platform-endian reinterpretation.</summary>
    public static long[] ArgbFromRgba(ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length % 4 != 0) throw new ArgumentException("RGBA input must contain complete pixels.", nameof(bytes));
        var pixels = new long[bytes.Length / 4];
        for (var i = 0; i < pixels.Length; i++)
            pixels[i] = ((long)bytes[i * 4 + 3] << 24) | ((long)bytes[i * 4] << 16) |
                ((long)bytes[i * 4 + 1] << 8) | bytes[i * 4 + 2];
        return pixels;
    }
}
