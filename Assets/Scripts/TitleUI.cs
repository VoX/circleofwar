using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;

public class TitleUI : MonoBehaviour {

	public Texture titleTexture;


    void OnGUI()
	{
		GUI.DrawTexture(new Rect(
			Screen.width/2 - titleTexture.width, 
			Screen.height/2-titleTexture.height*2 - 120, 
			titleTexture.width*2,
			titleTexture.height*2), titleTexture);
	}
}
