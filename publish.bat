@echo off
echo ==============================================
echo   Building Typemetric Release...
echo ==============================================

dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true

echo.
echo ==============================================
echo   Build completed! Press any key to exit.
echo ==============================================
pause
