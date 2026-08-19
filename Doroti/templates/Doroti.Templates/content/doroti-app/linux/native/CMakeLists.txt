cmake_minimum_required(VERSION 3.24)
project(doroti_qt_host VERSION 0.2.0 LANGUAGES CXX)

set(CMAKE_CXX_STANDARD 20)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_AUTOMOC ON)

find_package(Qt6 6.5 REQUIRED COMPONENTS Core Gui Widgets OpenGL OpenGLWidgets)

add_library(doroti_qt_host SHARED src/doroti_qt_host.cpp)
target_compile_definitions(doroti_qt_host PRIVATE DOROTI_QT_HOST_BUILD)
target_link_libraries(doroti_qt_host PRIVATE Qt6::Core Qt6::Gui Qt6::Widgets Qt6::OpenGL Qt6::OpenGLWidgets)
set_target_properties(doroti_qt_host PROPERTIES OUTPUT_NAME doroti_qt_host)

install(TARGETS doroti_qt_host LIBRARY DESTINATION .)
