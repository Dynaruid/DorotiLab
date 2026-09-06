cmake_minimum_required(VERSION 3.24)
project(doroti_qt_host VERSION 0.4.0 LANGUAGES C CXX)

set(CMAKE_CXX_STANDARD 20)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_AUTOMOC ON)
set(CMAKE_SKIP_RPATH ON)

find_package(Qt6 6.5 REQUIRED COMPONENTS Core Gui Widgets OpenGL OpenGLWidgets)
find_package(PkgConfig REQUIRED)
pkg_check_modules(WaylandClient REQUIRED IMPORTED_TARGET wayland-client)
find_program(WAYLAND_SCANNER_EXECUTABLE wayland-scanner REQUIRED)

set(DOROTI_WAYLAND_GENERATED_DIR "${CMAKE_CURRENT_BINARY_DIR}/wayland-generated")
file(MAKE_DIRECTORY "${DOROTI_WAYLAND_GENERATED_DIR}")

function(doroti_generate_wayland_client protocol_name protocol_xml)
  set(protocol_header "${DOROTI_WAYLAND_GENERATED_DIR}/${protocol_name}-client-protocol.h")
  set(protocol_code "${DOROTI_WAYLAND_GENERATED_DIR}/${protocol_name}-protocol.c")
  add_custom_command(
    OUTPUT "${protocol_header}"
    COMMAND "${WAYLAND_SCANNER_EXECUTABLE}" client-header "${protocol_xml}" "${protocol_header}"
    DEPENDS "${protocol_xml}"
    VERBATIM)
  add_custom_command(
    OUTPUT "${protocol_code}"
    COMMAND "${WAYLAND_SCANNER_EXECUTABLE}" private-code "${protocol_xml}" "${protocol_code}"
    DEPENDS "${protocol_xml}" "${protocol_header}"
    VERBATIM)
  target_sources(doroti_qt_host PRIVATE "${protocol_code}" "${protocol_header}")
endfunction()

add_library(doroti_qt_host SHARED src/doroti_qt_host.cpp)
doroti_generate_wayland_client(ext-background-effect-v1
  "${CMAKE_CURRENT_SOURCE_DIR}/protocols/ext-background-effect-v1.xml")
doroti_generate_wayland_client(kde-blur
  "${CMAKE_CURRENT_SOURCE_DIR}/protocols/kde-blur.xml")
target_include_directories(doroti_qt_host PUBLIC include PRIVATE "${DOROTI_WAYLAND_GENERATED_DIR}")
target_compile_definitions(doroti_qt_host PRIVATE DOROTI_QT_HOST_BUILD)
target_link_libraries(doroti_qt_host PRIVATE Qt6::Core Qt6::Gui Qt6::Widgets Qt6::OpenGL
  Qt6::OpenGLWidgets PkgConfig::WaylandClient)
set_target_properties(doroti_qt_host PROPERTIES OUTPUT_NAME doroti_qt_host)

install(TARGETS doroti_qt_host LIBRARY DESTINATION .)
