@echo off

echo Robot One Linux Installer Creator
md robotOne-linux-x86_64\robotOne-linux-x86_64\lib
md robotOne-linux-x86_64\robotOne-linux-x86_64\examples
md robotOne-linux-x86_64\robotOne-linux-x86_64\app
xcopy.exe "../../src/lib" "robotOne-linux-x86_64/robotOne-linux-x86_64/lib" /y /i /s /e
xcopy.exe "../../src/examples" "robotOne-linux-x86_64/robotOne-linux-x86_64/examples" /y /i /s /e
copy /Y "..\..\softwares\installLinux.sh" "robotOne-linux-x86_64\robotOne-linux-x86_64\install.sh"
xcopy.exe "../../build/Linux" "robotOne-linux-x86_64/robotOne-linux-x86_64/app" /y /i /s /e

rmdir /s /q robotOne-linux-x86_64\robotOne-linux-x86_64\lib\c_cpp\build
rmdir /s /q robotOne-linux-x86_64\robotOne-linux-x86_64\lib\c_cpp\build-linux
rmdir /s /q robotOne-linux-x86_64\robotOne-linux-x86_64\lib\c_cpp\export
rmdir /s /q robotOne-linux-x86_64\robotOne-linux-x86_64\lib\c_cpp\.vs
rmdir /s /q robotOne-linux-x86_64\robotOne-linux-x86_64\lib\c_cpp\.vscode
rmdir /s /q robotOne-linux-x86_64\robotOne-linux-x86_64\examples\cpp\build
rmdir /s /q robotOne-linux-x86_64\robotOne-linux-x86_64\examples\cpp\build-linux
rmdir /s /q robotOne-linux-x86_64\robotOne-linux-x86_64\examples\cpp\.vs
rmdir /s /q robotOne-linux-x86_64\robotOne-linux-x86_64\examples\cpp\.vscode

copy /Y "robotOne-linux-x86_64\robotOne-linux-x86_64\lib\c_cpp\robotOne_matlab.h" "robotOne-linux-x86_64\robotOne-linux-x86_64\lib\matlab\robotOne\robotOne.h"

cd robotOne-linux-x86_64
"../../../softwares/zip.exe" -r ../robotOne-linux-x86_64.zip *
cd ..

rmdir /s /q robotOne-linux-x86_64
