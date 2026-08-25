#pragma once

#include <algorithm>
#include <cmath>
#include <cstdint>
#include <optional>
#include <vector>

namespace doroti::resize_oracle {

struct Rect {
    int left{};
    int top{};
    int right{};
    int bottom{};
};

struct GridMetrics {
    std::optional<int> rightTail;
    std::optional<int> bottomTail;
    std::optional<double> spacingX;
    std::optional<double> spacingY;
    std::optional<double> nonUniformScaleRatio;
    std::optional<int> originOffsetX;
    std::optional<int> originOffsetY;
    bool rightEdgeMarkerDetected{};
    bool bottomEdgeMarkerDetected{};
    bool parsed{};
};

inline std::uint8_t const* Pixel(
    std::vector<std::uint8_t> const& pixels, int width, int x, int y) {
    return pixels.data() + (static_cast<std::size_t>(y) * width + x) * 4;
}

inline bool IsGridCyan(std::uint8_t const* pixel) {
    int const b = pixel[0], g = pixel[1], r = pixel[2];
    // Skia's one-logical-pixel line can cover a physical pixel fractionally at
    // non-integer DPI (for example R=96 at 125%). Keep the channel contrast
    // strict while allowing that antialiasing fringe.
    return b >= 205 && g >= 165 && r <= 115 && b - r >= 105 && g - r >= 70;
}

inline bool IsMarkerMagenta(std::uint8_t const* pixel) {
    int const b = pixel[0], g = pixel[1], r = pixel[2];
    return r >= 205 && g <= 90 && b <= 130 && r - g >= 130;
}

inline std::vector<int> ClusterCenters(std::vector<int> const& candidates) {
    std::vector<int> centers;
    if (candidates.empty()) return centers;
    int first = candidates.front();
    int last = first;
    for (std::size_t index = 1; index < candidates.size(); ++index) {
        if (candidates[index] <= last + 1) {
            last = candidates[index];
            continue;
        }
        centers.push_back((first + last) / 2);
        first = last = candidates[index];
    }
    centers.push_back((first + last) / 2);
    return centers;
}

inline std::optional<double> MedianSpacing(std::vector<int> const& centers, double expected) {
    std::vector<double> differences;
    for (std::size_t index = 1; index < centers.size(); ++index) {
        double const difference = static_cast<double>(centers[index] - centers[index - 1]);
        if (difference >= expected * 0.55 && difference <= expected * 1.55) {
            differences.push_back(difference);
        }
    }
    if (differences.size() < 2) return std::nullopt;
    std::sort(differences.begin(), differences.end());
    auto const middle = differences.size() / 2;
    if (differences.size() % 2 == 1) return differences[middle];
    return (differences[middle - 1] + differences[middle]) / 2.0;
}

inline std::vector<int> PeriodicColumns(
    std::vector<std::uint8_t> const& pixels, int width, int height, Rect client,
    int bodyTop, double expected) {
    std::vector<int> candidates;
    int const sampleTop = std::clamp(bodyTop + static_cast<int>(std::ceil(expected * 0.45)), client.top, client.bottom);
    int const sampleBottom = std::clamp(client.bottom - static_cast<int>(std::ceil(expected * 0.35)), sampleTop, client.bottom);
    int const span = sampleBottom - sampleTop;
    if (span < 8) return candidates;
    int constexpr samples = 47;
    for (int x = client.left; x < client.right; ++x) {
        int matches{};
        for (int sample = 0; sample < samples; ++sample) {
            int const y = std::min(sampleBottom - 1, sampleTop + span * sample / samples);
            if (x >= 0 && x < width && y >= 0 && y < height && IsGridCyan(Pixel(pixels, width, x, y))) ++matches;
        }
        if (matches >= samples * 3 / 5) candidates.push_back(x);
    }
    return ClusterCenters(candidates);
}

inline std::vector<int> PeriodicRows(
    std::vector<std::uint8_t> const& pixels, int width, int height, Rect client,
    int bodyTop, double expected) {
    std::vector<int> candidates;
    int const sampleLeft = std::clamp(client.left + static_cast<int>(std::ceil(expected * 0.75)), client.left, client.right);
    int const sampleRight = std::clamp(client.right - static_cast<int>(std::ceil(expected * 0.75)), sampleLeft, client.right);
    int const span = sampleRight - sampleLeft;
    if (span < 8) return candidates;
    int constexpr samples = 61;
    for (int y = bodyTop; y < client.bottom; ++y) {
        int matches{};
        for (int sample = 0; sample < samples; ++sample) {
            int const x = std::min(sampleRight - 1, sampleLeft + span * sample / samples);
            if (x >= 0 && x < width && y >= 0 && y < height && IsGridCyan(Pixel(pixels, width, x, y))) ++matches;
        }
        if (matches >= samples * 3 / 5) candidates.push_back(y);
    }
    return ClusterCenters(candidates);
}

inline bool MarkerConsensus(
    std::vector<std::uint8_t> const& pixels, int width, int height,
    int left, int top, int right, int bottom, int required) {
    left = std::clamp(left, 0, width);
    right = std::clamp(right, left, width);
    top = std::clamp(top, 0, height);
    bottom = std::clamp(bottom, top, height);
    int matches{};
    for (int y = top; y < bottom; ++y) {
        for (int x = left; x < right; ++x) {
            if (IsMarkerMagenta(Pixel(pixels, width, x, y))) ++matches;
        }
    }
    return matches >= required;
}

inline GridMetrics AnalyzeDiagnosticGrid(
    std::vector<std::uint8_t> const& pixels, int width, int height, Rect client, double scale) {
    GridMetrics result;
    if (width <= 0 || height <= 0 || pixels.size() < static_cast<std::size_t>(width) * height * 4 ||
        client.right - client.left < 96 || client.bottom - client.top < 96) return result;

    double const expected = 32.0 * std::max(1.0, scale);
    int const bodyTop = std::clamp(
        client.top + static_cast<int>(std::llround(56.0 * std::max(1.0, scale))),
        client.top, client.bottom - 1);
    auto const columns = PeriodicColumns(pixels, width, height, client, bodyTop, expected);
    auto const rows = PeriodicRows(pixels, width, height, client, bodyTop, expected);
    result.spacingX = MedianSpacing(columns, expected);
    result.spacingY = MedianSpacing(rows, expected);
    if (!columns.empty()) {
        result.rightTail = std::max(0, client.right - 1 - columns.back());
        result.originOffsetX = columns.front() - client.left;
    }
    if (!rows.empty()) {
        result.bottomTail = std::max(0, client.bottom - 1 - rows.back());
        result.originOffsetY = rows.front() - client.top;
    }
    if (result.spacingX && result.spacingY && *result.spacingY > 0.0) {
        result.nonUniformScaleRatio = *result.spacingX / *result.spacingY;
    }

    int const markerWidth = std::max(3, static_cast<int>(std::ceil(5.0 * scale)));
    result.rightEdgeMarkerDetected = MarkerConsensus(
        pixels, width, height,
        client.right - markerWidth, bodyTop,
        client.right, std::min(client.bottom, bodyTop + static_cast<int>(std::ceil(32.0 * scale))),
        std::max(3, static_cast<int>(std::ceil(8.0 * scale))));
    result.bottomEdgeMarkerDetected = MarkerConsensus(
        pixels, width, height,
        std::max(client.left, client.right - static_cast<int>(std::ceil(32.0 * scale))),
        client.bottom - markerWidth, client.right, client.bottom,
        std::max(3, static_cast<int>(std::ceil(8.0 * scale))));
    result.parsed = result.spacingX.has_value() && result.spacingY.has_value() &&
        result.rightTail.has_value() && result.bottomTail.has_value();
    return result;
}

} // namespace doroti::resize_oracle
