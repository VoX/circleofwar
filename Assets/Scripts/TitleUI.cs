using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;

public class TitleUI : MonoBehaviour {

	public Texture titleTexture;

	public string tankName;

    // detect headless mode (which has graphicsDeviceType Null)
    bool IsHeadless() {
        return SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null;
    }

    void Start()
    {
        Debug.Log("Checking for headless mode");
        if (IsHeadless())
        {
            NetworkManager.singleton.networkAddress = "0.0.0.0";
            NetworkManager.singleton.networkPort = 34920;
            Debug.Log("Creating match [" + NetworkManager.singleton.networkAddress + ":" + NetworkManager.singleton.networkPort + "]");
            NetworkManager.singleton.StartServer();
        }
    }

    void OnGUI()
	{
		GUI.DrawTexture(new Rect(
			Screen.width/2 - titleTexture.width, 
			Screen.height/2-titleTexture.height*2 - 150, 
			titleTexture.width*2,
			titleTexture.height*2), titleTexture);
		
		int posY = Screen.height/2 - 100;
		
		GUI.Label(new Rect(Screen.width/2 - 90, posY, 80, 20), "Tank Name:");
		tankName = GUI.TextField(new Rect(Screen.width/2 - 10, posY, 100, 20), tankName, 20);	
		posY += 40;
		
		if (GUI.Button(new Rect(Screen.width/2 - 100, posY, 200, 33), "Single Player Game\n[Press S]") ||
			Input.GetKeyDown(KeyCode.S))
		{
			NetworkManager.singleton.StartHost();
		}
		posY += 38;
		
		if (GUI.Button(new Rect(Screen.width/2 - 100, posY, 200, 33), "Host Multiplayer Game\n[Press H]") ||
		    Input.GetKeyDown(KeyCode.H))
		{
            SceneManager.LoadScene("host");
		}
		posY += 38;
		
		if (GUI.Button(new Rect(Screen.width/2 - 100, posY, 200, 33), "Join Multiplayer Game\n[Press J]") ||
		    Input.GetKeyDown(KeyCode.J))
		{
            SceneManager.LoadScene("join");			
		}
		posY += 38;
		
		if (GUI.Button(new Rect(Screen.width/2 - 100, posY, 200, 33), "Join Local Game\n[Press K]") ||
		    Input.GetKeyDown(KeyCode.K))
		{
			NetworkManager.singleton.StartClient();
		}		
		posY += 44;		
	}
}
