@echo off

mkdir build
cd build 
where x86_64-w64-mingw32-g++.exe >nul 2>&1 && SET /A USE64=1 || SET /A USE64=0
echo "Compiling for 64 bits: %USE64%"
IF %USE64% == 0 (
mingw32-g++.exe -c ../robotOne.cpp
mingw32-g++.exe -c ../TcpUdpSocket.cpp
mingw32-g++.exe -shared -o librobotOne.dll robotOne.o TcpUdpSocket.o -lwsock32
) ELSE (
x86_64-w64-mingw32-g++.exe -c ../robotOne.cpp
x86_64-w64-mingw32-g++.exe -c ../TcpUdpSocket.cpp
x86_64-w64-mingw32-g++.exe -shared -o librobotOne.dll robotOne.o TcpUdpSocket.o -lwsock32
)
cd ..
mkdir export
copy /Y build\librobotOne.dll export\librobotOne.dll
copy /Y robotOne_matlab.h export\robotOne.h
pause