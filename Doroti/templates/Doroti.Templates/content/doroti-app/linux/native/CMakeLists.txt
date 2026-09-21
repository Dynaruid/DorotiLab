cmake_minimum_required(VERSION 3.24)
project(doroti_qt_host VERSION 0.4.0 LANGUAGES C CXX)

set(CMAKE_CXX_STANDARD 20)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_AUTOMOC ON)
set(CMAKE_SKIP_RPATH ON)
option(DOROTI_QT_GRAPHITE "Use Graphite Vulkan output (official profile requires a Vulkan 1.2 device)" ON)
option(DOROTI_QT_QUICK "Compose Graphite GPU images and Qt Quick native controls" OFF)
option(DOROTI_QT_WEBENGINE "Enable the optional live WebEngine Quick attachment" OFF)
if(DOROTI_QT_WEBENGINE AND NOT DOROTI_QT_QUICK)
  message(FATAL_ERROR "WebEngine Quick requires DOROTI_QT_QUICK")
endif()

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

if(DOROTI_QT_QUICK)
  if(NOT DOROTI_QT_GRAPHITE)
    message(FATAL_ERROR "Qt Quick composition requires Graphite Vulkan")
  endif()
  find_package(Qt6 6.6 REQUIRED COMPONENTS Quick Qml QuickControls2)
  add_library(doroti_qt_host SHARED src/doroti_qt_host.cpp src/doroti_qt_quick.cpp)
  target_compile_definitions(doroti_qt_host PRIVATE DOROTI_QT_QUICK)
  target_link_libraries(doroti_qt_host PRIVATE Qt6::Quick Qt6::Qml Qt6::QuickControls2)
  qt_add_resources(doroti_qt_host quick_effect PREFIX "/doroti" FILES qml/Backdrop.qml)
  # Package the portable Vulkan shader so regular builds need no ShaderTools SDK
  # or configuration-specific qsb paths. Keep the source and baked asset in sync.
  file(READ "${CMAKE_CURRENT_SOURCE_DIR}/shaders/gaussian.frag.json" gaussian_metadata)
  foreach(shader_file gaussian.frag gaussian.frag.qsb)
    string(JSON expected_hash GET "${gaussian_metadata}" hashes "${shader_file}")
    file(SHA256 "${CMAKE_CURRENT_SOURCE_DIR}/shaders/${shader_file}" actual_hash)
    if(NOT actual_hash STREQUAL expected_hash)
      message(FATAL_ERROR "${shader_file} is out of date; regenerate with shaders/bake.py (see shaders/README.md)")
    endif()
  endforeach()
  set_property(DIRECTORY APPEND PROPERTY CMAKE_CONFIGURE_DEPENDS
    shaders/gaussian.frag shaders/gaussian.frag.qsb shaders/gaussian.frag.json)
  qt_add_resources(doroti_qt_host quick_blur PREFIX "/doroti" FILES shaders/gaussian.frag.qsb)
  if(DOROTI_QT_WEBENGINE)
    find_package(Qt6 6.8 REQUIRED COMPONENTS WebEngineQuick WebChannel)
    add_library(doroti_webview_qt SHARED src/doroti_webview_qt.cpp)
    target_include_directories(doroti_webview_qt PRIVATE include)
    target_compile_definitions(doroti_webview_qt PRIVATE DOROTI_QT_WEBENGINE DOROTI_QT_HOST_BUILD)
    target_link_libraries(doroti_webview_qt PRIVATE Qt6::WebEngineQuick Qt6::WebChannel Qt6::Quick Qt6::Widgets)
    qt_add_resources(doroti_webview_qt webview PREFIX "/doroti" FILES qml/WebView.qml)
    install(TARGETS doroti_webview_qt LIBRARY DESTINATION .)
    file(GENERATE OUTPUT "${CMAKE_CURRENT_BINARY_DIR}/doroti-webview-runtime.json" CONTENT
"{\"schemaVersion\":1,\"abiVersion\":1,\"abiBytes\":32,\"minimumQtApi\":\"6.8.0\",\"builtQt\":\"${Qt6Core_VERSION}\",\"builtWebEngine\":\"${Qt6WebEngineQuick_VERSION}\",\"engineSource\":\"system-distribution\",\"shims\":[\"libdoroti_qt_host.so\",\"libdoroti_webview_qt.so\"],\"qmlModules\":[\"QtQuick\",\"QtQuick.Controls\",\"QtWebEngine\",\"QtWebChannel\"],\"runtimeFiles\":[\"QtWebEngineProcess\",\"qtwebengine_resources.pak\",\"qtwebengine_resources_100p.pak\",\"qtwebengine_resources_200p.pak\",\"qtwebengine_locales/en-US.pak\"],\"sandbox\":\"default-required\"}\n")
    target_compile_definitions(doroti_qt_host PRIVATE DOROTI_QT_WEBENGINE)
    target_link_libraries(doroti_qt_host PRIVATE doroti_webview_qt)
    # Locate only our sibling shim; Qt/Chromium remain system dependencies.
    set(CMAKE_SKIP_RPATH OFF)
    set_target_properties(doroti_qt_host PROPERTIES BUILD_RPATH "$ORIGIN" INSTALL_RPATH "$ORIGIN" BUILD_WITH_INSTALL_RPATH TRUE)
  endif()
else()
  add_library(doroti_qt_host SHARED src/doroti_qt_host.cpp src/doroti_qt_platform_views.cpp)
endif()
doroti_generate_wayland_client(ext-background-effect-v1
  "${CMAKE_CURRENT_SOURCE_DIR}/protocols/ext-background-effect-v1.xml")
doroti_generate_wayland_client(kde-blur
  "${CMAKE_CURRENT_SOURCE_DIR}/protocols/kde-blur.xml")
target_include_directories(doroti_qt_host PUBLIC include PRIVATE "${DOROTI_WAYLAND_GENERATED_DIR}")
target_compile_definitions(doroti_qt_host PRIVATE DOROTI_QT_HOST_BUILD)
if(DOROTI_QT_GRAPHITE)
  find_package(Vulkan REQUIRED)
  target_compile_definitions(doroti_qt_host PRIVATE DOROTI_QT_GRAPHITE)
  target_link_libraries(doroti_qt_host PRIVATE Vulkan::Headers)
endif()
target_link_libraries(doroti_qt_host PRIVATE Qt6::Core Qt6::Gui Qt6::Widgets Qt6::OpenGL
  Qt6::OpenGLWidgets PkgConfig::WaylandClient)
set_target_properties(doroti_qt_host PROPERTIES OUTPUT_NAME doroti_qt_host)

install(TARGETS doroti_qt_host LIBRARY DESTINATION .)

# Camera/video producer is optional; no extra dependency for ordinary apps.
option(DOROTI_GSTREAMER_TEXTURES "Build the GPU DMA-BUF camera/video adapter" OFF)
if(DOROTI_GSTREAMER_TEXTURES)
  pkg_check_modules(GStreamerTexture REQUIRED IMPORTED_TARGET
    gstreamer-1.0>=1.24 gstreamer-app-1.0>=1.24 gstreamer-video-1.0>=1.24 gstreamer-allocators-1.0>=1.24)
  add_library(doroti_texture_gstreamer SHARED src/doroti_texture_gstreamer.cpp)
  target_link_libraries(doroti_texture_gstreamer PRIVATE PkgConfig::GStreamerTexture)
  install(TARGETS doroti_texture_gstreamer LIBRARY DESTINATION .)
endif()
