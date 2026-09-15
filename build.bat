@echo off
setlocal
set "SCRIPT_DIR=%~dp0"
set "CONFIG=%~1"
if "%CONFIG%"=="" set "CONFIG=Release"

powershell -NoProfile -ExecutionPolicy Bypass -File "%SCRIPT_DIR%build.ps1" -Configuration "%CONFIG%"
set "EXIT_CODE=%ERRORLEVEL%"

if not "%EXIT_CODE%"=="0" (
    echo.
    echo Build failed with error code %EXIT_CODE%.
)

:: Pause if double clicked in Windows Explorer
echo %cmdcmdline% | find /i "%~f0" >nul
if %errorlevel% equ 0 pause

exit /b %EXIT_CODE%
