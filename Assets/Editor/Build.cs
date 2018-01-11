using UnityEditor;

class Build
{
    static string[] scenes = {"Assets/Scenes/Title.unity",
                            "Assets/scenes/Royale.unity"};

    static void BuildWebGl()
    {
        string pathToDeploy = "build/webgl/";

        BuildPipeline.BuildPlayer(scenes, pathToDeploy, BuildTarget.WebGL, BuildOptions.None);
    }

    static void BuildWindows()
    {
        string pathToDeploy = "build/windows/circleofwar.exe";

        BuildPipeline.BuildPlayer(scenes, pathToDeploy, BuildTarget.StandaloneWindows, BuildOptions.None);
    }

    static void BuildLinux()
    {
        string pathToDeploy = "build/linux/circleofwar.x86_64";

        BuildPipeline.BuildPlayer(scenes, pathToDeploy, BuildTarget.StandaloneLinuxUniversal, BuildOptions.None);
    }
}