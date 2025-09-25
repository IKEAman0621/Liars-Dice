@echo off
setlocal

set SERVER_PATH=%~dp0\TCPclient
set APP_PATH=%~dp0\TCPserver

:: Start server silently
powershell -WindowStyle Hidden -Command "Start-Process 'dotnet' -ArgumentList 'run' -WorkingDirectory '%SERVER_PATH%' -WindowStyle Hidden"

:: Start client silently
powershell -WindowStyle Hidden -Command "Start-Process 'dotnet' -ArgumentList 'run' -WorkingDirectory '%APP_PATH%' -WindowStyle Hidden"

endlocal