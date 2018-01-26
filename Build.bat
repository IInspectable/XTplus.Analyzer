@echo off

set config=%1
if "%config%" == "" (
   set config=Debug
)

"%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\MSBuild.exe" XTplus.Analyzer.sln /t:Restore
"%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\MSBuild.exe" XTplus.Analyzer.sln /p:Configuration="%config%" /maxcpucount:4 /v:n

REM Pause