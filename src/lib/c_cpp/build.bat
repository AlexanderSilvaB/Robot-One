mkdir build
cd build 
x86_64-w64-mingw32-g++.exe -c ../robotOne.cpp
x86_64-w64-mingw32-g++.exe -c ../TcpUdpSocket.cpp
x86_64-w64-mingw32-g++.exe -shared -o librobotOne.dll robotOne.o TcpUdpSocket.o -lwsock32
cd ..
mkdir export
copy /Y build\librobotOne.dll export\librobotOne.dll
copy /Y robotOne_matlab.h export\robotOne.h
pause