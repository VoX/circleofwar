using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using System.IO;
using System;

public static class MultiplayersBuildAndRun {

    [MenuItem("File/Run Multiplayer/Start Server")]
    static void StartServer()
    {
        Cleanup();
        Build.BuildWindows();
        RunWindowsServer();
    }

    [MenuItem("File/Run Multiplayer/2 Players")]
	static void PerformWin64Build2 (){
        Cleanup();
        Build.BuildWindows();
        RunWindowsServer();
        RunWindowsClient();
        RunWindowsClient();
    }

	[MenuItem("File/Run Multiplayer/3 Players")]
	static void PerformWin64Build3 (){
        Cleanup();
        Build.BuildWindows();
        RunWindowsServer();
        RunWindowsClient();
        RunWindowsClient();
        RunWindowsClient();
    }

    [MenuItem("File/Run Multiplayer/Cleanup")]
    static void Cleanup()
    {
        RunProc("C:\\Windows\\System32\\taskkill.exe", "/F /IM circleofwar.exe");
    }

    static void RunProc(string path, string arguments)
    {
        var proc = new Process();
        proc.StartInfo.FileName = path;
        UnityEngine.Debug.Log("Run:" + proc.StartInfo.FileName);
        proc.StartInfo.Arguments = arguments;
        proc.StartInfo.RedirectStandardOutput = true;
        proc.StartInfo.UseShellExecute = false;
        proc.Exited += new EventHandler((o, e) => {
            UnityEngine.Debug.Log(proc.StartInfo.FileName + "finished with code " + proc.ExitCode + ":" + proc.StandardOutput.ReadToEnd());
        });
        proc.Start();
    }

    static void RunWindowsWithArguments(string arguments)
    {
        RunProc(Directory.GetParent(Application.dataPath).FullName + "\\build\\windows\\circleofwar.exe", arguments);
    }

    static void RunWindowsServer()
    {
        RunWindowsWithArguments("-batchmode -nographics");
    }

    static void RunWindowsClient()
	{
        RunWindowsWithArguments("");
    }
}