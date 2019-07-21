@echo off

echo Robot One Linux Installer Creator
md data
xcopy.exe "../../src/lib" "data" /y
xcopy.exe "../../src/examples" "data" /y