#pragma once

#include <algorithm>
#include <cstdint>
#include <optional>
#include <vector>

namespace doroti::resize_oracle {

// For a border drag whose opposite right edge stays fixed, that screen-space
// edge is an epoch-independent reference. Do not infer it by walking caption
// colors: an Acrylic caption can be almost identical to the desktop beside it.
inline std::optional<int> ValidationBackgroundGapAtFixedRight(
    std::vector<std::uint8_t> const& pixels, int width, int height,
    int rightExclusive) {
    if (width <= 0 || height <= 0 || rightExclusive <= 0 ||
        rightExclusive > width ||
        pixels.size() != static_cast<std::size_t>(width) * height * 4)
        return std::nullopt;
    int backgroundRight = -1;
    std::size_t backgroundPixels = 0;
    for (int y = 0; y < height; ++y) {
        for (int x = 0; x < rightExclusive; ++x) {
            auto const* pixel = pixels.data() +
                (static_cast<std::size_t>(y) * width + x) * 4;
            if (pixel[0] == 0x3a && pixel[1] == 0x24 && pixel[2] == 0x10) {
                backgroundRight = std::max(backgroundRight, x);
                ++backgroundPixels;
            }
        }
    }
    if (backgroundPixels < 1024) return std::nullopt;
    return rightExclusive - 1 - backgroundRight;
}

} // namespace doroti::resize_oracle
