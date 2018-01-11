using UnityEngine;
using UnityEngine.Networking;

public class HUD : NetworkBehaviour {

	Texture box;

    public FighterCombat fc;
    public AmmoUI aUI;
	
	float damageTimer;
	int damageAmount;
	int damageSide;
    

	void Start()
	{
		box = (Texture)Resources.Load("box");
		fc.EventTakeDamage += OnTakeDamage;
	}
	
	void OnTakeDamage(int side, int damage)
	{
		damageAmount = damage;
		damageSide = side;
		damageTimer = Time.time + 3;
	}
	
	void DrawBar(Color c, Vector3 pos, int offsetY, int current, int max)
	{
		GUI.color = Color.grey;
		GUI.DrawTexture (new Rect(pos.x-max/4 - 1, Screen.height - pos.y - offsetY, max/2 + 2, 7), box);
		
		GUI.color = c;
		GUI.DrawTexture (new Rect(pos.x-max/4, Screen.height - pos.y - offsetY + 1, current/2, 5), box);
	}
	
	void OnGUI()
	{
		Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);
		
		var myTeam = "-";
		if (NetworkClient.active && ClientScene.localPlayers.Count > 0 && ClientScene.localPlayers[0].gameObject != null)
		{
			FighterCombat localPlayerFc = ClientScene.localPlayers[0].gameObject.GetComponent<FighterCombat>();
			myTeam = localPlayerFc.team;
		}
		
		// your team is green, other teams are red
		Color teamColor = Color.white;
		if (myTeam == fc.team)
		{
            // draw the name with a shadow (colored for buf)
            GUI.color = Color.black;
            GUI.Label(new Rect(pos.x - 50, Screen.height - pos.y - 62, 150, 30), fc.fighterName);
            GUI.color = Color.green;
            GUI.Label(new Rect(pos.x - 51, Screen.height - pos.y - 61, 150, 30), fc.fighterName);
        }

        if (!isLocalPlayer)
            return;

        if (fc.alive && fc.ft != null)
		{
			DrawBar (teamColor, pos, 40, fc.health, fc.ft.maxHealth);
			DrawBar (Color.yellow, pos, 34, fc.energy, fc.ft.maxEnergy);
		}
		else
		{
			GUI.color = Color.black;
			GUI.Label(new Rect(pos.x-40, Screen.height - pos.y - 42, 150, 30), "YOU ARE DEAD");
			GUI.color = Color.white;
			GUI.Label(new Rect(pos.x-41, Screen.height - pos.y - 41, 150, 30), "YOU ARE DEAD");
		}


        if (damageTimer > Time.time)
		{
			Rect r = new Rect(0,0,0,0);
			if (damageSide == 0) {
				r = new Rect(40, 80, 40, 40);
			} else if (damageSide == 1) {
				r = new Rect(100, 160, 40, 40);
			} else if (damageSide == 2) {
				r = new Rect(40, 260, 40, 40);
			} else if (damageSide == 3) {
				r = new Rect(2, 160, 40, 40);
			}
			GUI.color = Color.red;
			GUI.Label(r, "[" + damageAmount + "]");
			
		}

        GUI.color = Color.grey;
        GUI.Box(new Rect(Screen.width - 80, Screen.height - 65, 80, 65), "");
        if (fc.gunController.EquippedGun != null)
        {
            GUI.color = Color.white;
            GUI.Label(new Rect(Screen.width-75, Screen.height-60, 75, 30), fc.gunController.EquippedGun.gunTypeName);
            GUI.Label(new Rect(Screen.width-75, Screen.height-30, 75, 30), fc.gunController.ammo + "/" + fc.gunController.maxAmmo);
        }



        /*GUI.Label(new Rect(10, 250, 200, 20), "Turret Ammo: " + fc.ammunitionTurret + "/" + fc.ft.maxAmmunitionTurret );
		GUI.Label(new Rect(10, 270, 200, 20), "Flame Ammo: " + fc.ammunitionMG + "/" + fc.ft.maxAmmunitionMG);*/

        GUI.color = Color.white;
		if (NetworkClient.active)
		{
			//GUI.Label(new Rect(5, 5, 180, 60), "RTT: " + NetworkClient.allClients[0].GetRTT());
		}
		
		GUI.color = Color.white;
		if (PlayGame.GetComplete())
		{
			GUI.Label (new Rect(Screen.width/2 - 60, Screen.height/2-100, 200, 40), "--- Winner Winner ---\nPRESS SPACE TO RESTART");
		}
	}
	
	void Update()
	{
		if (!isLocalPlayer)
			return;
		
		if (PlayGame.GetComplete())
		{
			if (Input.GetKeyDown(KeyCode.Space))
			{
				CmdFinishLevel();
			}
		}
    }
	
	[Command]
	public void CmdFinishLevel()
	{
		NetworkManager.singleton.ServerChangeScene("Royale");
	}
	
}
