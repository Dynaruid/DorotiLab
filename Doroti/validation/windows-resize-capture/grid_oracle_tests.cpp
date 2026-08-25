#include "grid_oracle.h"

#include <cmath>
#include <iostream>
#include <stdexcept>
#include <string>
#include <vector>

using doroti::resize_oracle::AnalyzeDiagnosticGrid;
using doroti::resize_oracle::GridMetrics;
using doroti::resize_oracle::Rect;

namespace {

void Fail(std::string const& message) { throw std::runtime_error(message); }

void Put(std::vector<std::uint8_t>& pixels, int width, int x, int y,
    std::uint8_t b, std::uint8_t g, std::uint8_t r) {
    auto* pixel = pixels.data() + (static_cast<std::size_t>(y) * width + x) * 4;
    pixel[0] = b; pixel[1] = g; pixel[2] = r; pixel[3] = 255;
}

std::vector<std::uint8_t> Fixture(
    int width, int height, Rect client, int sceneWidth, int sceneHeight,
    int spacingX, int spacingY, int originX = 0) {
    std::vector<std::uint8_t> pixels(static_cast<std::size_t>(width) * height * 4, 255);
    int const bodyTop = client.top + 56;
    int const sceneRight = std::min(client.right, client.left + sceneWidth);
    int const sceneBottom = std::min(client.bottom, bodyTop + sceneHeight);
    for (int x = client.left + originX; x < sceneRight; x += spacingX) {
        for (int y = bodyTop; y < sceneBottom; ++y) Put(pixels, width, x, y, 255, 229, 0);
    }
    for (int y = bodyTop; y < sceneBottom; y += spacingY) {
        for (int x = client.left; x < sceneRight; ++x) Put(pixels, width, x, y, 255, 229, 0);
    }
    if (sceneRight == client.right && sceneBottom == client.bottom) {
        for (int y = bodyTop + 7; y < bodyTop + 26; ++y)
            for (int x = client.right - 4; x < client.right - 1; ++x) Put(pixels, width, x, y, 68, 23, 255);
        for (int y = client.bottom - 4; y < client.bottom - 1; ++y)
            for (int x = client.right - 28; x < client.right - 1; ++x) Put(pixels, width, x, y, 68, 23, 255);
    }
    return pixels;
}

void RequireParsed(GridMetrics const& metrics, std::string const& name) {
    if (!metrics.parsed) Fail(name + " did not parse.");
}

} // namespace

int main() {
    try {
        Rect const client{100, 80, 900, 680};
        auto exactPixels = Fixture(1000, 760, client, 800, 552, 32, 32);
        auto const exact = AnalyzeDiagnosticGrid(exactPixels, 1000, 760, client, 1.0);
        RequireParsed(exact, "exact");
        if (std::abs(*exact.spacingX - 32.0) > 1.0 || std::abs(*exact.spacingY - 32.0) > 1.0)
            Fail("Exact fixture spacing exceeded one pixel.");
        if (!exact.rightEdgeMarkerDetected || !exact.bottomEdgeMarkerDetected)
            Fail("Exact fixture edge markers were not detected.");
        if (*exact.originOffsetX != 0 || *exact.originOffsetY != 56)
            Fail("Client-local origin was confused with the screen origin.");

        auto safeFillPixels = Fixture(1000, 760, client, 608, 400, 32, 32);
        auto const safeFill = AnalyzeDiagnosticGrid(safeFillPixels, 1000, 760, client, 1.0);
        RequireParsed(safeFill, "safe-fill");
        if (*safeFill.rightTail < 180 || *safeFill.bottomTail < 130)
            Fail("Safe-fill fixture was misclassified as exact scene coverage.");
        if (safeFill.rightEdgeMarkerDetected || safeFill.bottomEdgeMarkerDetected)
            Fail("Safe-fill fixture incorrectly retained current edge markers.");

        auto stretchPixels = Fixture(1000, 760, client, 800, 552, 40, 32);
        auto const stretch = AnalyzeDiagnosticGrid(stretchPixels, 1000, 760, client, 1.0);
        RequireParsed(stretch, "full-stretch");
        if (!stretch.nonUniformScaleRatio || std::abs(*stretch.nonUniformScaleRatio - 1.0) <= 0.02)
            Fail("Full-frame non-uniform stretch did not fail the scale oracle.");

        Rect const movedClient{37, 29, 837, 629};
        auto movedPixels = Fixture(900, 700, movedClient, 800, 552, 32, 32);
        auto const moved = AnalyzeDiagnosticGrid(movedPixels, 900, 700, movedClient, 1.0);
        RequireParsed(moved, "moved-origin");
        if (*moved.originOffsetX != 0 || *moved.originOffsetY != 56)
            Fail("Moved screen origin changed the child-local grid origin.");

        std::cout << "{\"schemaVersion\":\"doroti.windows.resize-grid-fixtures/v1\","
                  << "\"status\":\"PASS\",\"parseFailures\":0,"
                  << "\"exactSpacingX\":" << *exact.spacingX << ","
                  << "\"exactSpacingY\":" << *exact.spacingY << ","
                  << "\"safeFillRightTail\":" << *safeFill.rightTail << ","
                  << "\"safeFillBottomTail\":" << *safeFill.bottomTail << ","
                  << "\"stretchRatio\":" << *stretch.nonUniformScaleRatio << ","
                  << "\"movedOriginX\":" << *moved.originOffsetX << ","
                  << "\"movedOriginY\":" << *moved.originOffsetY << "}\n";
        return 0;
    } catch (std::exception const& error) {
        std::cerr << error.what() << '\n';
        return 1;
    }
}
