using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using System.Collections.Generic;

public class PlayGame : NetworkBehaviour {

	static public PlayGame singleton;

    [SyncVar]
    bool complete = false;

    private void Start()
    {
        singleton = this;
    }

    void Awake () 
	{
		
	}
			
	public static bool GetComplete()
	{
		return singleton.complete;
	}

    void SpawnTestPlayers()
    {
        Debug.Log("Spawning Test Players");
        for (int i = 0; i < 20; i++)
        {
            var autoFighter = FighterNetManager.singleton.SpawnPlayer();
            FighterMovement fm = autoFighter.GetComponent<FighterMovement>();
            fm.autoMove = true;
            NetworkServer.Spawn(autoFighter);
        }
    }

    void Update()
	{
        var fighters = FighterNetManager.singleton.fighters;
        if(fighters.Count > 1 && fighters.Count(x => x != null && x.GetComponent<FighterCombat>().alive) <= 1)
        {
            singleton.complete = true;
        }

        if (fighters.Count == 1 && FighterNetManager.singleton.mode == "local")
        {
            SpawnTestPlayers();
        }
	}
}
