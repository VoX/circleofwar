using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using System.IO;
using System;

public static class MultiplayersBuildAndRun {

    [MenuItem("File/Run Windows Multiplayer")]
	static void TestWindowsBuild (){
        Cleanup();
        Build.BuildWindows();
        RunWindowsServer();
        RunWindowsClient();
        RunWindowsClient();
    }

	[MenuItem("File/Run WebGL Multiplayer")]
	static void TestWebGlBuild (){
        Cleanup();
        Build.BuildWindows();
        Build.BuildWebGl();
        RunWindowsServerWebSockets();
        RunWebGlClient();
        RunWebGlClient();
    }

    static void Cleanup()
    {
        RunProc("C:\\Windows\\System32\\taskkill.exe", "/F /IM circleofwar.exe");
    }

    static void RunProc(string path, string arguments, string workingDirectory = null)
    {
        var proc = new Process();
        proc.StartInfo.FileName = path;
        UnityEngine.Debug.Log("Run:" + proc.StartInfo.FileName);
        proc.StartInfo.Arguments = arguments;
        proc.StartInfo.WorkingDirectory = workingDirectory == null ? proc.StartInfo.WorkingDirectory : workingDirectory;
        proc.Exited += new EventHandler((o, e) => {
            UnityEngine.Debug.Log(proc.StartInfo.FileName + "finished with code " + proc.ExitCode);
        });
        proc.Start();
    }

    static void RunWindowsWithArguments(string arguments)
    {
        RunProc(Directory.GetParent(Application.dataPath).FullName + "\\build\\windows\\circleofwar.exe", arguments);
    }

    static void RunWindowsServerWebSockets()
    {
        RunWindowsWithArguments("-batchmode -nographics -usewebsockets");
    }

    static void RunWindowsServer()
    {
        RunWindowsWithArguments("-batchmode -nographics");
    }

    static void RunWindowsClient()
	{
        RunWindowsWithArguments("");
    }

    static void RunWebGlClient()
    {
        Process.Start(Directory.GetParent(Application.dataPath).FullName + "\\build\\webgl\\index.html");
    }
}