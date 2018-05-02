using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSetting : MonoBehaviour {

	public GameObject backgroundPanel;
	public Color colorA, colorB;
	float speed = 0.25f;


	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		backgroundPanel.GetComponent<Image>().color = Color.Lerp(colorA, colorB, Mathf.PingPong(speed * Time.time, 1));
		
	}

	public void ExitToMenu(){
		// ...
	}

	public void OpenCloseSound(){
		// ...
	}

	public void GameStart(){
		
	}

}
