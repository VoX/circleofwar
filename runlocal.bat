SET project_dir=%~dp0

START /B CMD /C CALL "%project_dir%build\windows\circleofwar.exe" -batchmode -nographics
pushd "%project_dir%build\webgl\"
start index.html
popd
pause
taskkill.exe /F /IM circleofwar.exe
