cmake_minimum_required(VERSION 2.8)
project(circle)

set(CMAKE_BUILD_TYPE Release)
set(CMAKE_CXX_FLAGS_DEBUG "-g")
set(CMAKE_CXX_FLAGS_RELEASE "-O3")

find_package(robotOne REQUIRED)

add_executable(circle circle.cpp)
target_link_libraries(circle ${robotOne_LIBS})

find_package(bflib)
find_package( OpenCV REQUIRED )
find_package(dlib)

if(bflib_FOUND AND OpenCV_FOUND)
    if(dlib_FOUND)
        add_definitions(-DUSE_DLIB)
        set(DLIB_LIBS dlib::dlib)
        message("dlib found, using it to detect boxes.")
    else()
        set(DLIB_LIBS "")
        message("dlib not found, using inrange to detect boxes.")
    endif()

    add_executable(localization localization.cpp)
    target_link_libraries(localization ${bflib_LIBS} ${robotOne_LIBS} ${OpenCV_LIBS} ${DLIB_LIBS})
else()
    message(FATAL_ERROR "OpenCV and bflib are needed to build the localization sample. You can get bflib at https://github.com/AlexanderSilvaB/bflib")
endif()