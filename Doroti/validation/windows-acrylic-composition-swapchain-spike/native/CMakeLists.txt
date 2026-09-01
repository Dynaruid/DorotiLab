cmake_minimum_required(VERSION 3.25)
project(DorotiWindowsAcrylicCompositionSwapchainNative LANGUAGES CXX)

add_library(doroti_p1cs_native SHARED p1cs_native.cpp)
target_compile_features(doroti_p1cs_native PRIVATE cxx_std_20)
target_compile_definitions(doroti_p1cs_native PRIVATE
  WIN32_LEAN_AND_MEAN
  NOMINMAX
  UNICODE
  _UNICODE)
target_link_libraries(doroti_p1cs_native PRIVATE d3d11 dxgi dcomp)
