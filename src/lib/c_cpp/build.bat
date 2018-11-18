mkdir build
cd build 
g++ -c ../*.cpp
g++ -shared -o libmobilerobotics.dll *.o -lwsock32
cd ..
copy /Y build\libmobilerobotics.dll export\libmobilerobotics.dll
copy /Y mobilerobotics.h export\mobilerobotics.h