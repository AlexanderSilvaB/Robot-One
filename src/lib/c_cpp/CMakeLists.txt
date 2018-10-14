cmake_minimum_required(VERSION 2.8)
SET(CMAKE_INSTALL_PREFIX ../)
project(mobilerobotics)

include_directories(.)

set(SRC 
    mobilerobotics.h
    mobilerobotics.cpp
    TcpUdpSocket.h
    TcpUdpSocket.cpp)

if(WIN32)
    set(ADDITIONAL_LIBRARIES wsock32)
else()
    set(ADDITIONAL_LIBRARIES "")
endif()

add_library(${PROJECT_NAME} SHARED ${SRC} )
target_link_libraries(${PROJECT_NAME} ${ADDITIONAL_LIBRARIES})

install (TARGETS ${PROJECT_NAME} DESTINATION export)
install (FILES ${PROJECT_NAME}.h DESTINATION export)