using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;

public class FighterNetManager : NetworkManager
{
    public new static FighterNetManager singleton;

    int port = 34920;

    bool IsHeadless()
    {
        return SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null;
    }

    string weburl;

    void Start()
    {
        singleton = this;
        weburl = Application.absoluteURL;
        if (IsHeadless())
        {
            networkAddress = "0.0.0.0";
            networkPort = port;
            Debug.Log("Creating match [" +networkAddress + ":" + networkPort + "]");
            mode = "server";
            StartServer();
        }
        else if (Application.isEditor)
        {
            Debug.Log("Running Local Server");
            mode = "local";
            StartHost();

        }
        else
        {
            Uri uri;
            if (weburl.StartsWith("http") && Uri.TryCreate(weburl, UriKind.Absolute, out uri))
            {
                var url = new Uri(weburl);
                networkAddress = url.Host;
            }
            else
            {
                networkAddress = "localhost";
            }
            networkPort = port;
            Debug.Log("Joining server [" + networkAddress + ":" + networkPort + "]");
            mode = "client";
            StartClient();
        }
    }

    public string mode;
    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 1000, 20), "connection:" + networkAddress + ":" + port + " mode:" + mode);
    }

    Vector2 GetSpawn()
    {
        Vector3 pos = Vector3.zero;
        var random = UnityEngine.Random.insideUnitCircle * 40;
        pos.y = random.y;
        pos.x = random.x;
        return pos;
    }

    public GameObject SpawnPlayer()
    {
        // spawn random fighter
        GameObject fighter = (GameObject)Instantiate(NetworkManager.singleton.playerPrefab, GetSpawn(), Quaternion.identity);
        FighterCombat fc = fighter.GetComponent<FighterCombat>();
        fc.InitializeFromFighterType(FighterTypeManager.Random());
        return fighter;
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        NetworkServer.AddPlayerForConnection(conn, SpawnPlayer(), playerControllerId);
    }
}
