cmake_minimum_required(VERSION 3.24)

project(DorotiWindowsResizeCapture LANGUAGES CXX)

add_executable(Doroti.WindowsResizeCapture main.cpp)
target_compile_features(Doroti.WindowsResizeCapture PRIVATE cxx_std_20)
target_compile_definitions(Doroti.WindowsResizeCapture PRIVATE NOMINMAX WIN32_LEAN_AND_MEAN UNICODE _UNICODE)
target_compile_options(Doroti.WindowsResizeCapture PRIVATE /W4 /permissive- /EHsc)
target_link_libraries(Doroti.WindowsResizeCapture PRIVATE d3d11 dxgi dwmapi ole32 windowsapp windowscodecs winmm)

add_executable(Doroti.WindowsResizeGridOracleTests grid_oracle_tests.cpp)
target_compile_features(Doroti.WindowsResizeGridOracleTests PRIVATE cxx_std_20)
target_compile_options(Doroti.WindowsResizeGridOracleTests PRIVATE /W4 /permissive- /EHsc)
