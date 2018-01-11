SET project_dir=%~dp0

START /B CMD /C CALL "%project_dir%build\windows\circleofwar.exe" -batchmode -nographics
start %project_dir%build\webgl\index.html
pause
taskkill.exe /F /IM circleofwar.exe
