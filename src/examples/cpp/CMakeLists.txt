cmake_minimum_required(VERSION 2.8)
project(circle)

set(CMAKE_BUILD_TYPE Release)
set(CMAKE_CXX_FLAGS_DEBUG "-g")
set(CMAKE_CXX_FLAGS_RELEASE "-O3")

add_executable(circle circle.cpp)
target_link_libraries(circle robotOne)

find_package(KFs)
find_package( OpenCV REQUIRED )
find_package(dlib)

if(dlib_FOUND)
    add_definitions(-DUSE_DLIB)
    set(DLIB_LIBS dlib::dlib)
    message("dlib found.")
else()
    set(DLIB_LIBS "")
    message("dlib not found. Using inrange to detect boxes.")
endif()


add_executable(localization localization.cpp)
target_link_libraries(localization KFs robotOne ${OpenCV_LIBS} ${DLIB_LIBS})