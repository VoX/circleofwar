SET project_dir=%~dp0
SET unity_bin="C:\Program Files\Unity\Editor\Unity.exe"

rm -r "%project_dir%build"
%unity_bin% -nographics -quit -batchmode -projectPath "%project_dir%" -logFile -executeMethod Build.BuildWebGl || exit /b
%unity_bin% -nographics -quit -batchmode -projectPath "%project_dir%" -logFile -executeMethod Build.BuildWindows || exit /b
%unity_bin% -nographics -quit -batchmode -projectPath "%project_dir%" -logFile -executeMethod Build.BuildLinux || exit /b