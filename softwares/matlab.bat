@echo off

echo Matlab support for Robot One

NET SESSION >nul 2>&1
IF %ERRORLEVEL% EQU 0 (
    ECHO. 
) ELSE (
    ECHO Error: You need to run this as admin!
    pause
    exit
)

:READ
echo Select your Matlab version: 
echo 1 - R2018b
echo 2 - R2017b or R2018a
echo 3 - Any other version
set /p VER=Version: 

if /i "%VER%"=="1" goto v6_3_0
if /i "%VER%"=="2" goto v5_3_0
if /i "%VER%"=="3" goto v4_9_2

echo Invalid version "%VER%"
goto READ

:v6_3_0
REM .\modpath.exe /del "C:\robot-one\mingw\gcc-4.9.2\bin"
REM .\modpath.exe /del "C:\robot-one\mingw\gcc-5.3.0\bin"
REM .\modpath.exe /del "C:\robot-one\mingw\gcc-6.3.0\bin"
REM .\modpath.exe /add "C:\robot-one\mingw\gcc-6.3.0\bin"
REM .\modpath.exe /del "C:\robot-one\lib\4.9.2"
REM .\modpath.exe /del "C:\robot-one\lib\5.3.0"
REM .\modpath.exe /del "C:\robot-one\lib\6.3.0"
REM .\modpath.exe /add "C:\robot-one\lib\6.3.0"
setx /m MW_MINGW64_LOC "C:\robot-one\mingw\gcc-6.3.0"
goto DONE

:v5_3_0
REM .\modpath.exe /del "C:\robot-one\mingw\gcc-4.9.2\bin"
REM .\modpath.exe /del "C:\robot-one\mingw\gcc-5.3.0\bin"
REM .\modpath.exe /del "C:\robot-one\mingw\gcc-6.3.0\bin"
REM .\modpath.exe /add "C:\robot-one\mingw\gcc-5.3.0\bin"
REM .\modpath.exe /del "C:\robot-one\lib\4.9.2"
REM .\modpath.exe /del "C:\robot-one\lib\5.3.0"
REM .\modpath.exe /del "C:\robot-one\lib\6.3.0"
REM .\modpath.exe /add "C:\robot-one\lib\5.3.0"
setx /m MW_MINGW64_LOC "C:\robot-one\mingw\gcc-5.3.0"
goto DONE

:v4_9_2
REM .\modpath.exe /del "C:\robot-one\mingw\gcc-4.9.2\bin"
REM .\modpath.exe /del "C:\robot-one\mingw\gcc-5.3.0\bin"
REM .\modpath.exe /del "C:\robot-one\mingw\gcc-6.3.0\bin"
REM .\modpath.exe /add "C:\robot-one\mingw\gcc-4.9.2\bin"
REM .\modpath.exe /del "C:\robot-one\lib\4.9.2"
REM .\modpath.exe /del "C:\robot-one\lib\5.3.0"
REM .\modpath.exe /del "C:\robot-one\lib\6.3.0"
REM .\modpath.exe /add "C:\robot-one\lib\4.9.2"
setx /m MW_MINGW64_LOC "C:\robot-one\mingw\gcc-4.9.2"
goto DONE

:DONE
echo Done!
echo Add "C:\robot-one\lib\matlab" to the Matlab path
pause
exit