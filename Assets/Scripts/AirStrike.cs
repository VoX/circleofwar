using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;

public class AirStrike : NetworkBehaviour
{
	public GameObject bomber;
	public FighterCombat fc;
	public bool pendingStrike;
	
	bool mouseDown;
	List<Vector2> path;
	Vector2 last;
		
	[SyncVar]
	public int numStrikes = 3;
	
	[SyncVar]
	public float strikeTimer;
		
	[ClientCallback]
	void Update ()
	{
		
		if (Input.GetMouseButtonDown(0) && pendingStrike)
		{
			mouseDown = true;
			path = new List<Vector2>();
			Vector3 mp = Input.mousePosition;
			mp.z = 10;
			last = Camera.main.ScreenToWorldPoint(mp);
			path.Add(last);
		}
		
		if (mouseDown)
		{
			Vector3 mp = Input.mousePosition;
			mp.z = 10;
			Vector3 p = Camera.main.ScreenToWorldPoint(mp);
			if (((Vector2)p-last).magnitude > 4.0f)
			{
				path.Add(p);
				last = p;
				if (path.Count >= 5)
				{
					CmdStrike(path.ToArray());
					mouseDown = false;
					pendingStrike = false;
				}
			}
		}
		
		if (Input.GetMouseButtonUp(0))
		{
			if (mouseDown)
			{
				mouseDown = false;
				pendingStrike = false;
				CmdStrike(path.ToArray());
			}
		}
	}
	
	[Command]
	public void CmdStrike(Vector2[] strikePath)
	{
		if (!fc.alive)
			return;
			
		if (numStrikes <= 0)
			return;
			
		if (Time.time < strikeTimer)
		{
			// not allowed yet
			return;
		}
		
		numStrikes -=1;
		strikeTimer = Time.time + 10.0f;
		
		GameObject b = (GameObject)GameObject.Instantiate(bomber, strikePath[0], Quaternion.identity);
		b.GetComponent<Bomber>().FollowPath(strikePath);
		NetworkServer.Spawn(b);
	}
	
	void OnGUI()
	{
	}
	
}
