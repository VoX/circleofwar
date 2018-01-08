SET project_dir=%~dp0
SET unity_bin="C:\Program Files\Unity\Editor\Unity.exe"

%unity_bin% -nographics -quit -batchmode -projectPath %project_dir% -executeMethod Build.BuildWebGl
%unity_bin% -nographics -quit -batchmode -projectPath %project_dir% -executeMethod Build.BuildWindows
%unity_bin% -nographics -quit -batchmode -projectPath %project_dir% -executeMethod Build.BuildLinux