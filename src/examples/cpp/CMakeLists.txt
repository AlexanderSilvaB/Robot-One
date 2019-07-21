cmake_minimum_required(VERSION 2.8)
project(circle)

add_executable(circle circle.cpp)
target_link_libraries(circle robotOne)

find_package(KFs)
find_package( OpenCV REQUIRED )
add_executable(localization localization.cpp)
target_link_libraries(localization KFs robotOne ${OpenCV_LIBS})