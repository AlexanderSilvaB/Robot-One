cmake_minimum_required(VERSION 2.8)
project(robotOne)

include(GNUInstallDirs)

include_directories(.)

set(SRC 
    robotOne.h
    robotOne.cpp
    TcpUdpSocket.h
    TcpUdpSocket.cpp)

if(WIN32)
    set(ADDITIONAL_LIBRARIES wsock32)
else()
    set(ADDITIONAL_LIBRARIES "")
endif()

add_library(${PROJECT_NAME} SHARED ${SRC} )
target_link_libraries(${PROJECT_NAME} ${ADDITIONAL_LIBRARIES})
target_include_directories(${PROJECT_NAME} PRIVATE ${CMAKE_CURRENT_SOURCE_DIR})

install(TARGETS ${PROJECT_NAME}
    EXPORT ${PROJECT_NAME}_Targets
    ARCHIVE DESTINATION ${CMAKE_INSTALL_LIBDIR}
    LIBRARY DESTINATION ${CMAKE_INSTALL_LIBDIR}
    RUNTIME DESTINATION ${CMAKE_INSTALL_BINDIR})

include(CMakePackageConfigHelpers)

configure_package_config_file(
    "${PROJECT_SOURCE_DIR}/cmake/${PROJECT_NAME}Config.cmake.in"
    "${PROJECT_BINARY_DIR}/${PROJECT_NAME}Config.cmake"
    INSTALL_DESTINATION
    ${CMAKE_INSTALL_DATAROOTDIR}/${PROJECT_NAME}/cmake)

install(EXPORT ${PROJECT_NAME}_Targets
    FILE ${PROJECT_NAME}Targets.cmake
    DESTINATION ${CMAKE_INSTALL_DATAROOTDIR}/${PROJECT_NAME}/cmake)

install(FILES "${PROJECT_BINARY_DIR}/${PROJECT_NAME}Config.cmake"
    DESTINATION ${CMAKE_INSTALL_DATAROOTDIR}/${PROJECT_NAME}/cmake)

install(FILES ${PROJECT_SOURCE_DIR}/${PROJECT_NAME}.h DESTINATION include/robotOne)
install (TARGETS ${PROJECT_NAME} DESTINATION lib)