@echo off
set PATH=C:\WINDOWS\Microsoft.NET\Framework\v3.5;%PATH%

MSBuild Sheet.sln /m /t:Build /p:Configuration=Debug;TargetFrameworkVersion=v3.5

pause