using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using System.Collections.Generic;

public class PlayGame : NetworkBehaviour {

	static public PlayGame singleton;
    IEnumerable<FighterController> fighters;

    [SyncVar]
    bool complete = false;

    private void Start()
    {
        fighters = FindObjectsOfType<FighterController>();
        singleton = this;
    }

    void Awake () 
	{
        fighters = FindObjectsOfType<FighterController>();
    }
			
	public static bool GetComplete()
	{
		return singleton.complete;
	}

    [Server]
    void SpawnTestPlayers()
    {
        Debug.Log("Spawning Test Players");
        for (int i = 0; i < 10; i++)
        {
            var autoFighter = FighterNetManager.singleton.SpawnPlayer();
            var fc = autoFighter.GetComponent<FighterController>();
            NetworkServer.Spawn(autoFighter);
        }
    }

    [Server]
    void SpawnWeapons()
    {
        for (int i = 0; i < 20; i++)
        {
            var spawnWeap = FighterNetManager.singleton.SpawnWeapon();
            NetworkServer.Spawn(spawnWeap);
        }
    }

    [ServerCallback]
    void FixedUpdate()
	{
        if(fighters.Count() < 1)
        {
            fighters = FindObjectsOfType<FighterController>();
        }

        if (fighters.Count() > 1 && fighters.Count(x => x != null && x.GetComponent<FighterController>().alive) <= 1)
        {
            if(singleton.complete == false)
            {
                Debug.Log("Complete");
            }
            singleton.complete = true;
        }

        if (fighters.Count() == 1)
        {
            SpawnTestPlayers();
            SpawnWeapons();
            fighters = FindObjectsOfType<FighterController>();
        }
	}
}
