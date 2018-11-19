mkdir build
cd build 
g++ -c ../*.cpp
g++ -shared -o libmigo.dll *.o -lwsock32
cd ..
mkdir export
copy /Y build\libmigo.dll export\libmigo.dll
REM copy /Y migo.h export\migo.h
copy /Y migo_matlab.h export\migo.h