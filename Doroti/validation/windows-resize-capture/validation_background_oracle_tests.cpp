#include "validation_background_oracle.h"

#include <fstream>
#include <iostream>
#include <iterator>
#include <stdexcept>
#include <string>

using doroti::resize_oracle::ValidationBackgroundGapAtFixedRight;

int main(int argc, char** argv) {
    try {
        // Replay saved, unmodified BGRA captures through the production oracle.
        if (argc == 6 && std::string(argv[1]) == "--bgra") {
            std::ifstream input(argv[2], std::ios::binary);
            if (!input) throw std::runtime_error("Could not read BGRA capture");
            std::vector<std::uint8_t> pixels{
                std::istreambuf_iterator<char>(input), std::istreambuf_iterator<char>()};
            auto gap = ValidationBackgroundGapAtFixedRight(
                pixels, std::stoi(argv[3]), std::stoi(argv[4]), std::stoi(argv[5]));
            if (!gap) throw std::runtime_error("Capture has no usable sentinel/anchor");
            std::cout << *gap << '\n';
            return 0;
        }
        constexpr int width = 240, height = 100, right = 160;
        auto makeFrame = [](int gap) {
            // The caption and desktop deliberately have the SAME material color.
            std::vector<std::uint8_t> pixels(width * height * 4, 100);
            for (int y = 30; y < 90; ++y) {
                for (int x = 20; x < right - gap; ++x) {
                    auto* pixel = pixels.data() + (y * width + x) * 4;
                    pixel[0] = 0x3a; pixel[1] = 0x24; pixel[2] = 0x10;
                }
            }
            return pixels;
        };
        for (int gap : {0, 1, 3, 17, 43, 118}) {
            if (ValidationBackgroundGapAtFixedRight(makeFrame(gap), width, height, right) != gap)
                throw std::runtime_error("Actual raster gap was lost or desktop created a false gap");
        }
        auto frame = makeFrame(17);
        // A sentinel-colored object outside the target must not hide a real gap.
        for (int y = 0; y < height; ++y)
            for (int x = right + 5; x < width; ++x) {
                auto* pixel = frame.data() + (y * width + x) * 4;
                pixel[0] = 0x3a; pixel[1] = 0x24; pixel[2] = 0x10;
            }
        if (ValidationBackgroundGapAtFixedRight(frame, width, height, right) != 17 ||
            ValidationBackgroundGapAtFixedRight(frame, width, height, width + 1) ||
            ValidationBackgroundGapAtFixedRight({}, width, height, right) ||
            ValidationBackgroundGapAtFixedRight(std::vector<std::uint8_t>(width * height * 4), width, height, right))
            throw std::runtime_error("Invalid capture/anchor was accepted");
        std::cout << "Validation background oracle PASS\n";
        return 0;
    } catch (std::exception const& error) {
        std::cerr << error.what() << '\n';
        return 1;
    }
}
