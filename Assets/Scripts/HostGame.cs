using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.SceneManagement;

public class HostGame : MonoBehaviour
{
	public string address;
	public int port;

	void Start () 
	{
        address = "0.0.0.0";
        port = 34920;
	}

	void OnGUI()
	{
		int posY = Screen.height/2;
		
		GUI.Label(new Rect(Screen.width/2-100, posY, 100, 30), "Address");
		address = GUI.TextField(new Rect(Screen.width/2, posY, 200, 30), address);
		posY += 40;
		
		GUI.Label(new Rect(Screen.width/2-100, posY, 100, 30), "Port:");
		port = System.Convert.ToInt32(GUI.TextField(new Rect(Screen.width/2, posY, 200, 30), port.ToString()));
		posY += 40;
		
		if (address != "")
		{
			if(GUI.Button(new Rect(Screen.width/2-60,posY,120,30),"Host Game"))
			{
				Debug.Log("Creating match [" + address + ":" + port + "]");
                NetworkManager.singleton.networkAddress = address;
                NetworkManager.singleton.networkPort = port;
                NetworkManager.singleton.StartServer();
			}
		}
		
		posY += 140;
		if (GUI.Button (new Rect(Screen.width/2-100 , Screen.height - 50, 200, 30), "[ Back ]") || Input.GetKeyDown(KeyCode.Escape))
		{
            SceneManager.LoadScene("title");
		}
	}	
}
