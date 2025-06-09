@echo off
setlocal

REM Set your version here (or prompt user)
set "VERSION=0.8.5"

echo 🔧 Building framework-dependent (small) executable - version %VERSION%...

REM Define output folder
set "OUTPUT_FOLDER=publish\%VERSION%-trimmed"

REM Clean old build
rd /s /q "%OUTPUT_FOLDER%" 2>nul

REM Build
dotnet publish src/YtDlpGuiApp.csproj -c Release -r win-x64 --self-contained false ^
    -p:PublishSingleFile=true -p:PublishTrimmed=false -o "%OUTPUT_FOLDER%"

IF %ERRORLEVEL% NEQ 0 (
    echo ❌ Build failed.
) ELSE (
    echo ✅ Build succeeded. Output at %OUTPUT_FOLDER%\
)

pause
