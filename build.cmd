powershell -NoProfile -ExecutionPolicy Bypass -Command "& '%~dp0\build\runbuild.ps1' %*; if ($psake.build_success -eq $false) { exit 1 } else { exit 0 }"
exit /B %errorlevel%