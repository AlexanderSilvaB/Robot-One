@echo off

copy /Y ..\..\src\lib\c_cpp\robotOne_matlab.h robotOne.h
xcopy.exe "../../src/lib/matlab" "x64/matlab" /y /i /s /e

copy /Y robotOne.h x64\matlab\robotOne\robotOne.h

copy /Y robotOne.h x64\robotOne.h
copy /Y robotOne.h x86\robotOne.h
copy /Y librobotOne.dll x64\librobotOne.dll
copy /Y librobotOne.dll x86\librobotOne.dll
REM copy /Y x86\4.9.2\librobotOne.dll x86\librobotOne.dll
REM copy /Y x86\4.9.2\librobotOne.dll x64\librobotOne.dll

copy /Y "..\..\src\lib\matlab\robotOne" x64\matlab\robotOne
copy /Y "..\..\src\lib\python\robotOne.py" robotOne.py

copy /Y robotOne.h x64\matlab\robotOne\robotOne.h

copy /Y x64\4.9.2\librobotOne.dll x64\matlab\robotOne\librobotOne_R2014a.dll
copy /Y x64\4.9.2\librobotOne.dll x64\matlab\robotOne\librobotOne_R2014b.dll
copy /Y x64\4.9.2\librobotOne.dll x64\matlab\robotOne\librobotOne_R2015a.dll
copy /Y x64\4.9.2\librobotOne.dll x64\matlab\robotOne\librobotOne_R2015b.dll
copy /Y x64\4.9.2\librobotOne.dll x64\matlab\robotOne\librobotOne_R2016a.dll
copy /Y x64\4.9.2\librobotOne.dll x64\matlab\robotOne\librobotOne_R2016b.dll
copy /Y x64\4.9.2\librobotOne.dll x64\matlab\robotOne\librobotOne_R2017a.dll
copy /Y x64\5.3.0\librobotOne.dll x64\matlab\robotOne\librobotOne_R2017b.dll
copy /Y x64\5.3.0\librobotOne.dll x64\matlab\robotOne\librobotOne_R2018a.dll
copy /Y x64\6.3.0\librobotOne.dll x64\matlab\robotOne\librobotOne_R2018b.dll

REM copy /Y x86\4.9.2\librobotOne.dll x86\matlab\robotOne\librobotOne_R2014a.dll
REM copy /Y x86\4.9.2\librobotOne.dll x86\matlab\robotOne\librobotOne_R2014b.dll
REM copy /Y x86\4.9.2\librobotOne.dll x86\matlab\robotOne\librobotOne_R2015a.dll
REM copy /Y x86\4.9.2\librobotOne.dll x86\matlab\robotOne\librobotOne_R2015b.dll
REM copy /Y x86\4.9.2\librobotOne.dll x86\matlab\robotOne\librobotOne_R2016a.dll
REM copy /Y x86\4.9.2\librobotOne.dll x86\matlab\robotOne\librobotOne_R2016b.dll
REM copy /Y x86\4.9.2\librobotOne.dll x86\matlab\robotOne\librobotOne_R2017a.dll
REM copy /Y x86\5.3.0\librobotOne.dll x86\matlab\robotOne\librobotOne_R2017b.dll
REM copy /Y x86\5.3.0\librobotOne.dll x86\matlab\robotOne\librobotOne_R2018a.dll
REM copy /Y x86\6.3.0\librobotOne.dll x86\matlab\robotOne\librobotOne_R2018b.dll

cd x64
"../../zip.exe" -r ../lib-x64.zip *
cd ../x86
"../../zip.exe" -r ../lib-x86.zip *
cd ..