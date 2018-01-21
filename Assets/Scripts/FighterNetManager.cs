using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;

public class FighterNetManager : NetworkManager
{
    public new static FighterNetManager singleton;

    [SerializeField]
    GunType[] weapons;

    bool UseWebSockets()
    {
        return Environment.GetCommandLineArgs().Any(x => x.Contains("-usewebsockets")) || Application.platform == RuntimePlatform.LinuxPlayer;
    }

    bool IsServer()
    {
        return Environment.GetCommandLineArgs().Any(x=>x.Contains("-batchmode"));
    }

    string weburl;

    void Start()
    {
        networkPort = 34920;
        networkAddress = "127.0.0.1";
        singleton = this;
        weburl = Application.absoluteURL;

        if (IsServer())
        {
            useWebSockets = UseWebSockets();
            networkAddress = "0.0.0.0";
            Debug.Log("Creating match [" + networkAddress + ":" + networkPort + "]");
            mode = "server";
            StartServer();
        }
        else if (Application.isEditor)
        {
            Debug.Log("Running Manual Editor Mode");
            mode = "editor";
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
            if(Application.platform == RuntimePlatform.WebGLPlayer)
            {
                useWebSockets = true;
            }
            Debug.Log("Joining server [" + networkAddress + ":" + networkPort + "]");
            mode = "client";
            StartClient();
        }
    }

    public string mode;
    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 1000, 20), "connection:" + networkAddress + ":" + networkPort + " mode:" + mode);
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
        var fc = fighter.GetComponent<FighterController>();
        fc.InitializeFromFighterType(FighterTypeManager.Random());
        return fighter;
    }

    public GameObject SpawnWeapon()
    {
        if (weapons.Length > 0)
        {
            GunType thisWeapon = weapons[UnityEngine.Random.Range(0, weapons.Length)];
            // spawn random weapon
            GameObject weapon = Instantiate(thisWeapon.pickupPrefab, GetSpawn(), Quaternion.identity);
            return weapon;
        }
        return null;
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        NetworkServer.AddPlayerForConnection(conn, SpawnPlayer(), playerControllerId);
    }
}
