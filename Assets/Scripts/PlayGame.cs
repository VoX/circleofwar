using UnityEngine;
using UnityEngine.Networking;
using System.Linq;

public class PlayGame : NetworkBehaviour {

	static public PlayGame singleton;

	[SyncVar]
	bool complete;
	
	void Awake () 
	{
		singleton = this;
	}
			
	public static bool GetComplete()
	{
		return singleton.complete;
	}
	
	void Update()
	{
        var fighters = Object.FindObjectsOfType<FighterCombat>();
        if(fighters.Length > 1 && fighters.Count(x=>x.alive) <= 1)
        {
            singleton.complete = true;
        }


        if (Input.GetKeyDown(KeyCode.H))
		{
			// spawn random fighter
			GameObject fighter = (GameObject)Instantiate(NetworkManager.singleton.playerPrefab, Vector3.zero, Quaternion.identity);
			FighterCombat fc = fighter.GetComponent<FighterCombat>();
			fc.InitializeFromFighterType(FighterTypeManager.Random());
			NetworkServer.Spawn(fighter);
		}
	}
}
