using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using System.Collections.Generic;

public class PlayGame : NetworkBehaviour {

	static public PlayGame singleton;
    IEnumerable<FighterCombat> fighters;

    [SyncVar]
    bool complete = false;

    private void Start()
    {
        fighters = FindObjectsOfType<FighterCombat>();
        singleton = this;
    }

    void Awake () 
	{
		
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
            FighterMovement fm = autoFighter.GetComponent<FighterMovement>();
            fm.autoMove = true;
            NetworkServer.Spawn(autoFighter);
        }
    }

    [ServerCallback]
    void Update()
	{
        if(fighters.Count() > 1 && fighters.Count(x => x != null && x.GetComponent<FighterCombat>().alive) <= 1)
        {
            singleton.complete = true;
        }

        if (fighters.Count() == 1)
        {
            SpawnTestPlayers();
            fighters = FindObjectsOfType<FighterCombat>();
        }
	}
}
