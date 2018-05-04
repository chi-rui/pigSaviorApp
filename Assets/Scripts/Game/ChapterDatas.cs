using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChapterDatas : MonoBehaviour {

	// temp game info
	private static ChapterDatas gameDatas;
	
	public static int nowStage = 1;
	public static int progress = 0;
	
	// public GameObject Character, W;

	// Use this for initialization
	void Start () {
		// loadingBar = GameObject.Find("test").transform.GetChild(0);
		// print(GameObject.Find("test").transform.GetChild(0));
	}

	// Update is called once per frame
	void Update () {

	}

	// void Awake(){
	// 	if(saver){
	// 		DestroyImmediate(gameObject);
	// 		// comeback = true;
	// 	}else{
	// 	 	DontDestroyOnLoad(transform.gameObject);
	// 	 	saver = this;
	// 	}
	// }

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
