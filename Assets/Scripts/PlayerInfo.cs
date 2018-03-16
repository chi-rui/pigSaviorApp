using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerInfo : MonoBehaviour {

	// Account info
	private string account = null;
	private string password = null;
	private List<int> achienements = new List<int>();
	private List<string> records = new List<string>();

	// Game info
	public static int nowStage = 1;
	public static int progress = 0;
	public static int character = 0;
	
	public int i;

	// public static PlayerInfo CreateFromJSON(string jsonString){
	// 	return JsonUtility.FromJson<PlayerInfo>(jsonString);
	// }
	
	private void getInfo(){
		if(account != ""){
			Debug.Log("account: " + account + "\npassword: " + password);
			Debug.Log("\nprogress: " + progress + "\ncharacter: " + character );
			Debug.Log("\nachievements: \n");
			for(i = 0; i < achienements.Count; i++){
				Debug.Log(achienements[i] + "\n");
			}
			Debug.Log("\nrecords: \n");
			for(i = 0; i < records.Count; i++){
				Debug.Log(records[i] + "\n");
			}
		}
	}


	public GameObject Character, W;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonDown(0)){
	        if (W.GetComponent<RectTransform>().rect.Contains(Input.mousePosition)){//(0)
	        	print("In workspace, nothing will happened");
	        }else if(Input.mousePosition.x < (Screen.width / 2)){
	        	print("Left: " + Input.mousePosition);
	        }else if(Input.mousePosition.x > (Screen.width / 2)){
	        	print("Right: " + Input.mousePosition);        	
	        }else{
	        	print("Maybe center???");
	        }
		}
	}

	public void setProgress(int value){
		progress = value;
		print("Progress already change to " + progress + ".");	
	}

}
