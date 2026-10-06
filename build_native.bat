@echo off
setlocal enabledelayedexpansion
set "VSWHERE=%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe"
if not exist "%VSWHERE%" (
  echo [ERROR] vswhere not found. Install Visual Studio with "Desktop development with C++".
  exit /b 1
)
for /f "usebackq tokens=*" %%i in (`"%VSWHERE%" -latest -products * -requires Microsoft.VisualStudio.Component.VC.Tools.x86.x64 -property installationPath`) do set "VSPATH=%%i"
if not defined VSPATH (
  echo [ERROR] Visual Studio with C++ tools not found. Install VS with "Desktop development with C++".
  exit /b 1
)
for /f "usebackq tokens=1 delims=." %%v in (`"%VSWHERE%" -latest -products * -requires Microsoft.VisualStudio.Component.VC.Tools.x86.x64 -property installationVersion`) do set "VSMAJOR=%%v"
echo [INFO] Visual Studio: %VSPATH%  (major %VSMAJOR%)

rem Map the installed VS major version to its CMake generator.
set "GEN="
if "%VSMAJOR%"=="18" set "GEN=Visual Studio 18 2026"
if "%VSMAJOR%"=="17" set "GEN=Visual Studio 17 2022"
if "%VSMAJOR%"=="16" set "GEN=Visual Studio 16 2019"
if "%VSMAJOR%"=="15" set "GEN=Visual Studio 15 2017"

if defined GEN (
  echo [INFO] Generator: !GEN!
  cmake -S . -B out -G "!GEN!" -A x64 -DCMAKE_BUILD_TYPE=Release
  if errorlevel 1 exit /b %errorlevel%
  cmake --build out --config Release
  if errorlevel 1 exit /b %errorlevel%
) else (
  echo [INFO] Unknown VS major "%VSMAJOR%"; falling back to NMake.
  call "%VSPATH%\Common7\Tools\VsDevCmd.bat" -arch=x64 -host_arch=x64
  if errorlevel 1 exit /b %errorlevel%
  cmake -S . -B out -G "NMake Makefiles" -DCMAKE_BUILD_TYPE=Release
  if errorlevel 1 exit /b %errorlevel%
  cmake --build out --config Release
  if errorlevel 1 exit /b %errorlevel%
)
echo [OK] Build finished: out\Release\ChineseOCRLiteDesktop.exe
