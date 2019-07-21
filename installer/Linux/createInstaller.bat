@echo off

echo Robot One Linux Installer Creator
md data\lib
md data\examples
md data\app
xcopy.exe "../../src/lib" "data/lib" /y /i /s /e
xcopy.exe "../../src/examples" "data/examples" /y /i /s /e
copy /Y "..\..\softwares\installLinux.sh" "data\install.sh"
xcopy.exe "../../build/Linux" "data/app" /y /i /s /e

rmdir /s /q data\lib\c_cpp\build
rmdir /s /q data\lib\c_cpp\build-linux
rmdir /s /q data\examples\cpp\build
rmdir /s /q data\examples\cpp\build-linux

copy /Y "data\lib\c_cpp\robotOne_matlab.h" "data\lib\matlab\robotOne\robotOne.h"

cd data
"../../../softwares/zip.exe" -r ../robotOne.zip *
cd ..

rmdir /s /q data
