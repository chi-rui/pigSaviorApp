using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserDatasControl : MonoBehaviour {

	public UserDatas userDatas;

	// temp game info
	public static int nowStage = 1;
	public static int progress = 0;
	public static int character = 0;
	
	// public GameObject Character, W;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		// if(Input.GetMouseButtonDown(0)){
	 //        if (W.GetComponent<RectTransform>().rect.Contains(Input.mousePosition)){//(0)
	 //        	print("In workspace, nothing will happened");
	 //        }else if(Input.mousePosition.x < (Screen.width / 2)){
	 //        	print("Left: " + Input.mousePosition);
	 //        }else if(Input.mousePosition.x > (Screen.width / 2)){
	 //        	print("Right: " + Input.mousePosition);        	
	 //        }else{
	 //        	print("Maybe center???");
	 //        }
		// }
	}

	public void getUserDatas(){
		// call by login button.
		// ...
	}

	public void updateUserDatas(){
		// call php address to upload user datas.	
	}

	public void setProgress(int value){
		progress = value;
		print("Progress already change to " + progress + ".");	
	}

}
